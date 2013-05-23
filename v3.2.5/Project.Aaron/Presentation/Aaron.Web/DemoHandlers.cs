using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Aaron.Web
{
    public class DemoHandlers : IHttpHandler
    {
        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Write("The page request is " + context.Request.RawUrl.ToString());
            StreamWriter sw = new StreamWriter(@"C:\handlers.txt", true);
            sw.WriteLine("The page request at " + DateTime.Now.ToString()  +" is " + context.Request.RawUrl);
            sw.Close();
        }
    }
}