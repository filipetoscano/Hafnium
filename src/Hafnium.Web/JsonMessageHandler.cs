using Hafnium.Runtime;
using Hafnium.WebServices.Json;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Web;

namespace Hafnium.WebServices
{
    public class JsonMessageHandler : IHttpHandler
    {
        public void ProcessRequest( HttpContext context )
        {
            string urlPath = context.Request.AppRelativeCurrentExecutionFilePath.Substring( "~/api/".Length ) + context.Request.PathInfo;


            /*
             * 
             */
            JsonRequest request = new JsonRequest();
            request.ExecutionId = Guid.NewGuid();
            request.Rule = urlPath.Replace( "/", "." );
            request.MomentStart = DateTime.UtcNow;
            


            /*
             * 
             */
            using ( var sr = new StreamReader( HttpContext.Current.Request.InputStream ) )
            {
                sr.BaseStream.Seek( 0, SeekOrigin.Begin );
                request.Message = sr.ReadToEnd();
            }


            /*
             * 
             */
            RuleRunner rr = new RuleRunner();
            IRule rule = rr.Get( request.Rule );

            if ( rule == null )
            {
                context.Response.StatusCode = 404;
                context.Response.End();
            }


            /*
             * 
             */
            object oreq = JsonConvert.DeserializeObject( request.Message, rule.RequestType );


            /*
             * 
             */
            object oresp = rr.Run( rule, oreq );


            /*
             * 
             */
            JsonResponse response = new JsonResponse();
            response.IsFault = false;
            response.Message = JsonConvert.SerializeObject( oresp );


            /*
             * 
             */
            context.Response.StatusCode = 200;
            context.Response.CacheControl = "private";
            context.Response.ContentType = "application/json; charset=utf-8";

            context.Response.Headers.Add( "X-Hafnium-ExecutionId", request.ExecutionId.ToString( "D" ) );
            context.Response.Headers.Add( "X-Hafnium-MomentStart", request.MomentStart.ToString( "s" ) );
            context.Response.Headers.Add( "X-Hafnium-MomentEnd", DateTime.UtcNow.ToString( "s" ) );

            context.Response.Write( response.Message );
            context.Response.End();
        }


        public bool IsReusable
        {
            get { return true; }
        }
    }
}
