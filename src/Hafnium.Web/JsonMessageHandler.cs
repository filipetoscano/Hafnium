using Hafnium.Runtime;
using Hafnium.Web.Json;
using Newtonsoft.Json;
using System.IO;
using System.Web;

namespace Hafnium.Web
{
    public class JsonMessageHandler : IHttpHandler
    {
        public void ProcessRequest( HttpContext context )
        {
            JsonRequest request = new JsonRequest();

            
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
        }


        public bool IsReusable
        {
            get { return true; }
        }
    }
}
