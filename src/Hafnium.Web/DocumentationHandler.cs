using Hafnium.Runtime;
using Platinum;
using Platinum.Resolver;
using System;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml.Xsl;

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
