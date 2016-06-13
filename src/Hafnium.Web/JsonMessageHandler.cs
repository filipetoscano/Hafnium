using System.Web;

namespace Hafnium.Web
{
    public class JsonMessageHandler : IHttpHandler
    {
        public void ProcessRequest( HttpContext context )
        {
        }


        public bool IsReusable
        {
            get { return true; }
        }
    }
}
