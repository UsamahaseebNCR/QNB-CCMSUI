using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace CCMSUI.admin
{
    /// <summary>
    /// Summary description for Download
    /// </summary>
    public class Download : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string file = context.Request.QueryString["file"];
            string tempPath = context.Request.QueryString["tempPath"];
            string bufferlength = context.Request.QueryString["bufferlength"];

            if (!string.IsNullOrEmpty(file))
            {
                context.Response.Clear();
                context.Response.ContentType = "application/octet-stream";
                context.Response.AddHeader("Content-Disposition", "attachment; filename=\"" + file + "\"");
                context.Response.AddHeader("Content-Length", bufferlength);
                //HttpContext.Current.Response.BinaryWrite(buffer);
                context.Response.TransmitFile(tempPath + file);
                context.Response.Flush();
                context.Response.End();

            }
            else
            {
                context.Response.ContentType = "text/plain";
                context.Response.Write("File not be found!");


            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}