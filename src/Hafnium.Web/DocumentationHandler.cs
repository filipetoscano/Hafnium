﻿using System.Web;

namespace Hafnium.WebServices
{
    public class DocumentationHandler : IHttpHandler
    {
        public void ProcessRequest( HttpContext context )
        {
            context.Response.StatusCode = 200;
            context.Response.Write( "<html></html>" );
        }


        public bool IsReusable
        {
            get { return true; }
        }
    }
}
