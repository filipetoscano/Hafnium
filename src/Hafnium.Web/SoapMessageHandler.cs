using System;
using System.Web;

namespace Hafnium.Web
{
    public class SoapMessageHandler : IHttpHandler
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
