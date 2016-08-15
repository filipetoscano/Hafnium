using Hafnium.Runtime;
using Hafnium.WebServices.Soap;
using Platinum;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;
using Zinc.WebServices;

namespace Hafnium.WebServices
{
    public class SoapMessageHandler : IHttpHandler
    {
        public void ProcessRequest( HttpContext context )
        {
            #region Validations

            if ( context == null )
                throw new ArgumentNullException( "context" );

            #endregion


            /*
             * 
             */
            ExecutionHeader execution = new ExecutionHeader();
            execution.ExecutionId = Guid.NewGuid();
            execution.MomentStart = DateTime.UtcNow;


            /*
             * 
             */
            SoapRequest request = null;

            if ( context.Request.Headers[ "SOAPAction" ] != null )
            {
                request = new SoapRequest();
                request.Version = SoapVersion.Soap11;
                request.SoapAction = context.Request.Headers[ "SOAPAction" ];
                request.SoapAction = request.SoapAction.Substring( 1, request.SoapAction.Length - 2 );
            }
            else
            {
                var values = context.Request.ContentType
                    .Split( ';' )
                    .Select( s =>
                    {
                        var p = s.Split( new char[] { '=' }, 2 );

                        if ( p.Length == 1 )
                            return new { Key = "content-type", Value = p[ 0 ] };
                        else
                            return new { Key = p[ 0 ], Value = p[ 1 ] };
                    } ).ToList();

                var contentType = values.FirstOrDefault( x => x.Key == "content-type" );
                var action = values.FirstOrDefault( x => x.Key == "action" );

                if ( contentType?.Value == "application/soap+xml" && action != null )
                {
                    request = new SoapRequest();
                    request.Version = SoapVersion.Soap12;
                    request.SoapAction = action.Value.Substring( 1, action.Value.Length - 2 );
                }
            }

            if ( request == null )
            {
                ActorException error = new WebException( ER.Soap_RequestNotSoap );

                var fault = ToFault( request.Version, execution, error );
                ResponseEmit( context, fault );
                return;
            }


            /*
             * Derive the rule being invoked, based on the URL of the request.
             */
            if ( request.SoapAction.StartsWith( WebServicesConfiguration.Current.Namespace ) == false )
            {
                ActorException error = new WebException( ER.Soap_ActionUnsupported, request.SoapAction, WebServicesConfiguration.Current.Namespace );

                var fault = ToFault( request.Version, execution, error );
                ResponseEmit( context, fault );
                return;
            }

            string ruleName = request.SoapAction.Substring( WebServicesConfiguration.Current.Namespace.Length ).Replace( "/", "." );


            /*
             * 
             */
            using ( var sr = new StreamReader( HttpContext.Current.Request.InputStream ) )
            {
                sr.BaseStream.Seek( 0, SeekOrigin.Begin );
                request.Message = sr.ReadToEnd();
            }


            /*
             * Service NS - derived from the name of the rule :)
             */
            string[] parts = ruleName.Split( '.' );
            string serviceNs = string.Join( "/", parts.Take( parts.Length - 2 ) ) + "/";


            /*
             * 
             */
            RuleRunner rr = new RuleRunner();
            IRule rule = rr.Get( ruleName );

            if ( rule == null )
            {
                ActorException error = new WebException( ER.Soap_ActionUnknown, request.SoapAction );

                var fault = ToFault( request.Version, execution, error );
                ResponseEmit( context, fault );
                return;
            }


            /*
             * 
             */
            XNamespace soapNs;

            if ( request.Version == SoapVersion.Soap11 )
                soapNs = "http://schemas.xmlsoap.org/soap/envelope/";
            else
                soapNs = "http://www.w3.org/2003/05/soap-envelope";

            XmlNamespaceManager manager = new XmlNamespaceManager( new NameTable() );
            manager.AddNamespace( "soap", soapNs.NamespaceName );


            /*
             *
             */
            XDocument requestDoc;

            try
            {
                using ( StringReader sr = new StringReader( request.Message ) )
                {
                    requestDoc = XDocument.Load( sr );
                }
            }
            catch ( XmlException ex )
            {
                ActorException error = new WebException( ER.Soap_RequestNotXml, ex );

                var fault = ToFault( request.Version, execution, error );
                ResponseEmit( context, fault );
                return;
            }

            XElement element = requestDoc.XPathSelectElement( " /soap:Envelope/soap:Body/*[ 1 ] ", manager );

            if ( element == null )
            {
                ActorException error = new WebException( ER.Soap_BodyNotFound );

                var fault = ToFault( request.Version, execution, error );
                ResponseEmit( context, fault );
                return;
            }


            /*
             * 
             */
            object oreq;
            XmlSerializer der = XmlSerializerFor( rule.RequestType, serviceNs );

            try
            {
                oreq = der.Deserialize( element.CreateReader() );
            }
            catch ( InvalidOperationException ex )
            {
                ActorException error = new WebException( ER.Soap_RequestInvalid, ex, rule.Name, rule.RequestType.FullName );

                var fault = ToFault( request.Version, execution, error );
                ResponseEmit( context, fault );
                return;
            }


            /*
             * 
             */
            SoapResponse response;

            try
            {
                object oresp;

                oresp = rr.Run( rule, oreq );

                response = ToResponse( request.Version, execution, oresp, serviceNs );
            }
            catch ( ActorException ex )
            {
                response = ToFault( request.Version, execution, ex );
            }
            catch ( Exception ex )
            {
                ActorException aex = new WebException( ER.Soap_UnhandledException, ex );

                response = ToFault( request.Version, execution, aex );
            }


            /*
             * 
             */
            ResponseEmit( context, response );
            return;
        }


        private static SoapResponse ToResponse( SoapVersion version, ExecutionHeader execution, object response, string serviceNs )
        {
            /*
             * 
             */
            XNamespace soapNs;

            if ( version == SoapVersion.Soap11 )
                soapNs = "http://schemas.xmlsoap.org/soap/envelope/";
            else
                soapNs = "http://www.w3.org/2003/05/soap-envelope";


            /*
             * 
             */
            XDocument responseDoc = new XDocument();

            var envelope = new XElement( soapNs + "Envelope" );
            envelope.Add( new XAttribute( XNamespace.Xmlns + "soap", soapNs.NamespaceName ) );

            var header = new XElement( soapNs + "Header" );
            var body = new XElement( soapNs + "Body" );

            envelope.Add( header, body );
            responseDoc.Add( envelope );


            /*
             * 
             */
            execution.MomentEnd = DateTime.UtcNow;

            XmlSerializer serHeader = new XmlSerializer( typeof( ExecutionHeader ) );
            header.Add( serHeader.SerializeAsXElement( execution ) );


            /*
             * 
             */
            Type t = response.GetType();

            XmlSerializer serBody = XmlSerializerFor( t, serviceNs );
            body.Add( serBody.SerializeAsXElement( response ) );


            /*
             * 
             */
            SoapResponse resp = new SoapResponse();
            resp.Version = version;
            resp.IsFault = false;
            resp.Message = responseDoc.ToString( SaveOptions.DisableFormatting );

            return resp;
        }


        private static SoapResponse ToFault( SoapVersion version, ExecutionHeader execution, ActorException exception )
        {
            #region Validations

            if ( execution == null )
                throw new ArgumentNullException( nameof( execution ) );

            if ( exception == null )
                throw new ArgumentNullException( nameof( exception ) );

            #endregion


            /*
             * 
             */
            XNamespace fwkNs = "https://github.com/filipetoscano/Zinc";
            XNamespace soapNs;

            if ( version == SoapVersion.Soap11 )
                soapNs = "http://schemas.xmlsoap.org/soap/envelope/";
            else
                soapNs = "http://www.w3.org/2003/05/soap-envelope";

            XDocument responseDoc = new XDocument();

            var envelope = new XElement( soapNs + "Envelope" );
            envelope.Add( new XAttribute( XNamespace.Xmlns + "soap", soapNs.NamespaceName ) );

            var header = new XElement( soapNs + "Header" );
            var body = new XElement( soapNs + "Body" );
            var fault = new XElement( soapNs + "Fault" );

            body.Add( fault );
            envelope.Add( header, body );
            responseDoc.Add( envelope );


            /*
             * Header
             */
            execution.MomentEnd = DateTime.UtcNow;

            XmlSerializer serHeader = new XmlSerializer( typeof( ExecutionHeader ) );
            header.Add( serHeader.SerializeAsXElement( execution ) );


            /*
             * Fault
             */
            if ( version == SoapVersion.Soap11 )
            {
                // TODO: recurse through exception stack

                XElement x = new XElement( fwkNs + "ActorFault",
                    new XElement( fwkNs + "Actor", new XText( exception.Actor ) ),
                    new XElement( fwkNs + "Code", new XText( exception.Code.ToString( CultureInfo.InvariantCulture ) ) ),
                    new XElement( fwkNs + "Message", new XText( exception.Description ) ),
                    new XElement( fwkNs + "ExceptionType", new XText( exception.GetType().FullName ) )
                );

                if ( exception.StackTrace != null )
                    x.Add( new XElement( fwkNs + "StackTrace", new XText( exception.StackTrace ) ) );

                fault.Add(
                    new XElement( "faultcode", new XText( exception.Actor.EndsWith( "Client", StringComparison.Ordinal ) == true ? "soap:Client" : "soap:Server" ) ),
                    new XElement( "faultstring", new XText( exception.Description ) ),
                    new XElement( "detail", x )
                );
            }
            else
            {
                // TODO: implemented
                throw new NotImplementedException();
            }


            /*
             * 
             */
            SoapResponse response = new SoapResponse();
            response.Version = version;
            response.IsFault = true;
            response.Message = responseDoc.ToString( SaveOptions.DisableFormatting );

            return response;
        }


        private static void ResponseEmit( HttpContext context, SoapResponse response )
        {
            context.Response.TrySkipIisCustomErrors = true;

            if ( response.IsFault == true )
                context.Response.StatusCode = 500;
            else
                context.Response.StatusCode = 200;

            context.Response.CacheControl = "private";

            if ( response.Version == SoapVersion.Soap12 )
                context.Response.ContentType = "text/xml; charset=utf-8";
            else
                context.Response.ContentType = "application/soap+xml; charset=utf-8";

            context.Response.Write( response.Message );
        }



        private static Dictionary<Type, XmlSerializer> _xsc = new Dictionary<Type, XmlSerializer>();

        private static XmlSerializer XmlSerializerFor( Type messageType, string serviceNs )
        {
            XmlSerializer xs;

            if ( _xsc.TryGetValue( messageType, out xs ) == true )
                return xs;

            lock ( _xsc )
            {
                XmlAttributes xa = new XmlAttributes();
                xa.XmlRoot = new XmlRootAttribute() { Namespace = WebServicesConfiguration.Current.Namespace + serviceNs };

                XmlAttributeOverrides xao = new XmlAttributeOverrides();
                xao.Add( messageType, xa );

                xs = new XmlSerializer( messageType, xao );

                _xsc.Add( messageType, xs );
            }

            return xs;
        }


        public bool IsReusable
        {
            get { return true; }
        }
    }
}
