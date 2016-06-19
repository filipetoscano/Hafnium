using Hafnium.Runtime;
using Hafnium.Web.Soap;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace Hafnium.Web
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
                context.Response.StatusCode = 500;
                context.Response.CacheControl = "private";
                context.Response.ContentType = "text/xml; charset=utf-8";
                context.Response.Write( @"<s:Envelope xmlns:s='http://schemas.xmlsoap.org/soap/envelope/'>
   <s:Header />
   <s:Body>
      <s:Fault>
         <faultcode>s:Client</faultcode>
         <faultstring>Was that SOAP? You sure? :)</faultstring>
      </s:Fault>
   </s:Body>
</s:Envelope>" );
                context.Response.End();
            }


            /*
             * 
             */
            using ( var sr = new StreamReader( HttpContext.Current.Request.InputStream ) )
            {
                sr.BaseStream.Seek( 0, SeekOrigin.Begin );
                request.Message = sr.ReadToEnd();
            }


            /*
             * Derive the rule being invoked, based on the URL of the request.
             */
            string ruleName = "";


            /*
             * 
             */
            RuleRunner rr = new RuleRunner();
            IRule rule = rr.Get( ruleName );


            /*
             * 
             */
            XNamespace soapNs;

            if ( request.Version == SoapVersion.Soap11 )
                soapNs = "http://schemas.xmlsoap.org/soap/envelope/";
            else
                soapNs = "http://www.w3.org/2003/05/soap-envelope";


            /*
             * 
             */
            XmlNamespaceManager manager = new XmlNamespaceManager( new NameTable() );
            manager.AddNamespace( "soap", soapNs.NamespaceName );


            /*
             * 
             */
            object oreq;

            XDocument doc = XDocument.Load( request.Message );
            XElement element = (XElement) doc.XPathEvaluate( " /soap:Envelope/soap:Body/*[ 1 ] ", manager );

            XmlSerializer der = new XmlSerializer( rule.RequestType );
            oreq = der.Deserialize( element.CreateReader() );


            /*
             * 
             */
            object oresp = rr.Run( rule, oreq );



            /*
             * 
             */
            XDocument rdoc = new XDocument();
            doc.Add( soapNs + "Envelope" );
            

            XmlSerializer ser = new XmlSerializer( rule.ResponseType );
            //string resp = ser.Serialize( oresp );



            /*
             * 
             */
            SoapResponse response = new SoapResponse();
            response.IsFault = false;
            response.Message = "";


            /*
             * 
             */
            if ( request.Version == SoapVersion.Soap11 )
            {
                if ( response.IsFault == true )
                    context.Response.StatusCode = 500;
                else
                    context.Response.StatusCode = 200;

                context.Response.CacheControl = "private";
                context.Response.ContentType = "text/xml; charset=utf-8";

                context.Response.Write( response.Message );
                context.Response.End();
            }

            if ( request.Version == SoapVersion.Soap12 )
            {
                if ( response.IsFault == true )
                    context.Response.StatusCode = 500;
                else
                    context.Response.StatusCode = 200;

                context.Response.CacheControl = "private";
                context.Response.ContentType = "application/soap+xml; charset=utf-8";

                context.Response.Write( response.Message );
                context.Response.End();
            }
        }


        public bool IsReusable
        {
            get { return true; }
        }
    }
}
