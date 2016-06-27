using Hafnium.Runtime;
using Platinum;
using Platinum.Resolver;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Xml.XPath;
using System.Xml.Xsl;
using Zinc.WebServices;

namespace Hafnium.WebServices
{
    /// <summary>
    /// Non-production documentation handler.
    /// </summary>
    public class DocumentationHandler : IHttpHandler
    {
        /// <summary>
        /// Handles HTTP GET requests to Hafnium resources.
        /// </summary>
        /// <param name="context">
        /// An <see cref="HttpContext"/> object that provides references to the intrinsic server
        /// objects (for example, Request, Response, Session, and Server) used to service
        /// HTTP requests.
        /// </param>
        public void ProcessRequest( HttpContext context )
        {
            if ( IsIndex( context ) )
            {
                ProcessIndex( context );
            }
            else
            {
                ProcessService( context );
            }
        }


        /// <summary>
        /// Gets whether the current request is requesting the index file.
        /// </summary>
        /// <param name="context">HTTP execution context.</param>
        /// <returns>True if the index file is being requested, False otherwise.</returns>
        private bool IsIndex( HttpContext context )
        {
            #region Validations

            if ( context == null )
                throw new ArgumentNullException( nameof( context ) );

            #endregion

            if ( context.Request.AppRelativeCurrentExecutionFilePath == "~/index.hf" )
                return true;

            return false;
        }


        /// <summary>
        /// Renders the index page.
        /// </summary>
        /// <param name="context">HTTP execution context.</param>
        private void ProcessIndex( HttpContext context )
        {
            #region Validations

            if ( context == null )
                throw new ArgumentNullException( nameof( context ) );

            #endregion


            /*
             * 
             */
            var services = RuleCatalogue.Services;


            /*
             * Build index document
             */
            XDocument doc = new XDocument();

            XElement index = new XElement( "index", new XAttribute( "application", App.Name ) );
            doc.Add( index );

            foreach ( var s in services )
            {
                index.Add(
                    new XElement( "service",
                        new XAttribute( "name", s.Name ),
                        new XAttribute( "namespace", s.Namespace )
                    )
                );
            }


            /*
             * 
             */
            RenderDocument( context, doc, "assembly:///Hafnium.WebServices/~/Transforms/ToIndex.xslt" );
        }


        private void ProcessService( HttpContext context )
        {
            #region Validations

            if ( context == null )
                throw new ArgumentNullException( nameof( context ) );

            #endregion

            /*
             * 
             */
            string appPath = ToApplicationPath( context.Request );

            string requestPath = context.Request.Path.Substring( appPath.Length );
            requestPath = requestPath.Substring( 0, requestPath.LastIndexOf( ".hf" ) );

            int lastSlash = requestPath.LastIndexOf( '/' );
            string serviceName;
            string serviceNs;

            if ( lastSlash > -1 )
            {
                serviceName = requestPath.Substring( lastSlash + 1 );
                serviceNs = requestPath.Substring( 0, lastSlash + 1 );
            }
            else
            {
                serviceName = requestPath;
                serviceNs = "/";
            }


            RuleService service = RuleCatalogue.Services.Where( s => s.Name == serviceName && s.Namespace == serviceNs ).FirstOrDefault();

            if ( service == null )
            {
                context.Response.StatusCode = 404;
                context.Response.End();
                return;
            }

            if ( context.Request.QueryString.Count > 0 && context.Request.QueryString[ 0 ] == "wsdl" )
            {
                ProcessWsdl( context, service );
            }
            else if ( context.Request.QueryString.Count > 0 && context.Request.QueryString[ 0 ] == "disco" )
            {
                ProcessDisco( context, service );
            }
            else if ( context.Request.QueryString[ "op" ] != null )
            {
                IRule rule;

                string ruleName = context.Request.QueryString[ "op" ];
                if ( service.Rules.TryGetValue( ruleName, out rule ) == false )
                {
                    context.Response.StatusCode = 404;
                    context.Response.End();
                    return;
                }

                ProcessRule( context, service, rule );
            }
            else
            {
                ProcessServiceIndex( context, service );
            }
        }



        /// <summary>
        /// Generates the HTML documentation for the specific service.
        /// </summary>
        /// <param name="context">HTTP execution context.</param>
        /// <param name="service">Service description.</param>
        private void ProcessServiceIndex( HttpContext context, RuleService service )
        {
            #region Validations

            if ( context == null )
                throw new ArgumentNullException( nameof( context ) );

            if ( service == null )
                throw new ArgumentNullException( nameof( service ) );

            #endregion


            /*
             * Build index document
             */
            XDocument doc = new XDocument();

            XElement serviceIndex = new XElement( "service",
                new XAttribute( "name", service.Name ),
                new XAttribute( "namespace", service.Namespace )
            );
            doc.Add( serviceIndex );

            foreach ( var r in service.Rules )
            {
                serviceIndex.Add(
                    new XElement( "rule",
                        new XAttribute( "name", r.Key ),
                        new XAttribute( "publicName", r.Value.Metadata.Name ?? "" ),
                        new XAttribute( "description", r.Value.Metadata.Description ?? "" )
                    )
                );
            }


            /*
             * 
             */
            RenderDocument( context, doc, "assembly:///Hafnium.WebServices/~/Transforms/ToService.xslt" );
        }



        /// <summary>
        /// Generates the HTML documentation for the specific service method / rule.
        /// </summary>
        /// <param name="context">HTTP execution context.</param>
        /// <param name="service">Service description.</param>
        /// <param name="rule">Rule description.</param>
        private void ProcessRule( HttpContext context, RuleService service, IRule rule )
        {
            #region Validations

            if ( context == null )
                throw new ArgumentNullException( nameof( context ) );

            if ( service == null )
                throw new ArgumentNullException( nameof( service ) );

            if ( rule == null )
                throw new ArgumentNullException( nameof( rule ) );

            #endregion


            /*
             * 
             */
            XDocument doc = new XDocument();

            XElement methodIndex = new XElement( "rule",
                new XAttribute( "service", service.Name ),
                new XAttribute( "name", rule.Name ),
                new XAttribute( "publicName", rule.Metadata.Name ?? "" ),
                new XAttribute( "description", rule.Metadata.Description ?? "" ),
                new XAttribute( "requestType", rule.RequestType.FullName ),
                new XAttribute( "responseType", rule.ResponseType.FullName )
            );
            doc.Add( methodIndex );


            /*
             * 
             */
            RenderDocument( context, doc, "assembly:///Hafnium.WebServices/~/Transforms/ToRule.xslt" );
        }


        /// <summary>
        /// Emits the DISCO document for the current service.
        /// </summary>
        /// <param name="context">HTTP execution context.</param>
        /// <param name="service">Service description.</param>
        private void ProcessDisco( HttpContext context, RuleService service )
        {
            #region Validations

            if ( context == null )
                throw new ArgumentNullException( nameof( context ) );

            if ( service == null )
                throw new ArgumentNullException( nameof( service ) );

            #endregion


            /*
             * 
             */
            string serviceUrl =
                context.Request.Url.Scheme + "://" +
                context.Request.Url.Authority +
                context.Request.Url.LocalPath;


            /*
             * 
             */
            XElement root = new XElement( "service",
                new XAttribute( "name", service.Name ),
                new XAttribute( "url", serviceUrl )
            );

            XDocument doc = new XDocument( root );


            /*
             * 
             */
            RenderDocument( context, doc, "assembly:///Hafnium.WebServices/~/Transforms/ToDisco.xslt" );
        }


        /// <summary>
        /// Emits the WSDL document for the current service.
        /// </summary>
        /// <param name="context">HTTP execution context.</param>
        /// <param name="service">Service description.</param>
        private void ProcessWsdl( HttpContext context, RuleService service )
        {
            #region Validations

            if ( context == null )
                throw new ArgumentNullException( nameof( context ) );

            if ( service == null )
                throw new ArgumentNullException( nameof( service ) );

            #endregion

            /*
             * 
             */
            string serviceNs = WebEndpointConfiguration.Namespace + service.Namespace;


            /*
             * 
             */
            var schemas = new XmlSchemas();
            var exporter = new XmlSchemaExporter( schemas );
            var importer = new XmlReflectionImporter();

            exporter.ExportTypeMapping( importer.ImportTypeMapping( typeof( EndpointHeader ) ) );
            exporter.ExportTypeMapping( importer.ImportTypeMapping( typeof( ExecutionHeader ) ) );
            exporter.ExportTypeMapping( importer.ImportTypeMapping( typeof( ActorFault ) ) );
            exporter.ExportTypeMapping( importer.ImportTypeMapping( typeof( ActorFault ), new XmlRootAttribute()
            {
                ElementName = "ActorFault",
                Namespace = "https://github.com/filipetoscano/Zinc"
            } ) );

            foreach ( var m in service.Rules )
            {
                XmlRootAttribute ra = new XmlRootAttribute();
                ra.Namespace = serviceNs;

                exporter.ExportTypeMapping( importer.ImportTypeMapping( m.Value.RequestType, ra ) );
                exporter.ExportTypeMapping( importer.ImportTypeMapping( m.Value.ResponseType, ra ) );
            }


            /*
             * 
             */
            string serviceUrl =
                context.Request.Url.Scheme + "://" +
                context.Request.Url.Authority +
                ToApplicationPath( context.Request ) +
                "run.hf";


            /*
             * Build index document
             */
            XElement serviceIndex = new XElement( "service",
                new XAttribute( "name", service.Name ),
                new XAttribute( "namespace", serviceNs ),
                new XAttribute( "url", serviceUrl )
            );

            foreach ( var m in service.Rules )
            {
                string action = serviceNs + service.Name + "/" + m.Key;

                serviceIndex.Add(
                    new XElement( "method",
                        new XAttribute( "name", m.Key ),
                        new XAttribute( "action", action )
                    )
                );
            }


            /*
             * 
             */
            XElement types = new XElement( "types" );

            foreach ( XmlSchema schema in schemas )
            {
                StringWriter sw = new StringWriter();
                schema.Write( sw );

                types.Add( XElement.Parse( sw.ToString() ) );
            }

            serviceIndex.Add( types );
            XDocument doc = new XDocument( serviceIndex );


            /*
             * 
             */
            RenderDocument( context, doc, "assembly:///Hafnium.WebServices/~/Transforms/ToWsdl.xslt" );
        }



        /// <summary>
        /// Applies the XSLT transformation to the document, writing the output
        /// directly to the HTTP response.
        /// </summary>
        /// <param name="context">HTTP execution context.</param>
        /// <param name="document">Input document.</param>
        /// <param name="xsltUri">XSLT transformation.</param>
        private static void RenderDocument( HttpContext context, XDocument document, string xsltUri )
        {
            #region Validations

            if ( context == null )
                throw new ArgumentNullException( nameof( context ) );

            if ( document == null )
                throw new ArgumentNullException( nameof( document ) );

            if ( xsltUri == null )
                throw new ArgumentNullException( nameof( xsltUri ) );

            #endregion


            /*
             * Load
             */
            UrlResolver r = new UrlResolver( false );

            XmlReaderSettings xrs = new XmlReaderSettings();
            xrs.XmlResolver = r;

            XsltSettings xs = new XsltSettings();
            xs.EnableDocumentFunction = false;
            xs.EnableScript = false;

            XslCompiledTransform xslt = new XslCompiledTransform();

            using ( XmlReader xr = XmlReader.Create( xsltUri, xrs ) )
            {
                xslt.Load( xr, xs, r );
            }


            /*
             *
             */
            context.Response.StatusCode = 200;

            if ( xslt.OutputSettings.OutputMethod == XmlOutputMethod.Html )
                context.Response.ContentType = "text/html";
            else if ( xslt.OutputSettings.OutputMethod == XmlOutputMethod.Text )
                context.Response.ContentType = "text/plain";
            else
                context.Response.ContentType = "text/xml";


            /*
             * Apply
             */
            XmlWriterSettings xws = xslt.OutputSettings.Clone();
            xws.Indent = false;

            XsltArgumentList args = new XsltArgumentList();
            args.AddParam( "ApplicationPath", "", ToApplicationPath( context.Request ) );

            using ( XmlWriter xw = XmlWriter.Create( context.Response.OutputStream, xws ) )
            {
                xslt.Transform( document.CreateNavigator(), args, xw );
            }

            context.Response.End();
        }


        /// <summary>
        /// Returns the slash-terminated path to the current application.
        /// </summary>
        /// <param name="request">HTTP execution context.</param>
        /// <returns>Slash terminated application path.</returns>
        private static string ToApplicationPath( HttpRequest request )
        {
            if ( request.ApplicationPath.EndsWith( "/", StringComparison.Ordinal ) == true )
                return request.ApplicationPath;
            else
                return request.ApplicationPath + "/";
        }


        public bool IsReusable
        {
            get { return true; }
        }
    }
}
