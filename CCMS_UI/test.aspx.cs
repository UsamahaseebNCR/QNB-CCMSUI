using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Avanza.CCMS.DAL;
using Avanza.iSuite.DAL;
using Microsoft.Win32;

namespace CCMSUI
{
    public partial class test : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            string connStr = (string)Registry.LocalMachine.OpenSubKey(@"SOFTWARE\NCR\CCMS", false).GetValue("ConnectionString", "");
            connStr = Encryption.Cryptic.DecryptString(connStr);
            ConnectionFactory.Initialize(connStr, true);
             
            AppSetting appSetting = AppSetting.LoadAppSetting("1=1");
            Response.Write(appSetting.LogFilePath);
            XmlLogWriter.InitXmlLogWriter(String.Format("{0}\\CCMS_UI_{1:yyMMMdd_HHmmss}.txt", appSetting.LogFilePath, DateTime.Now));//                          
               
        }
    }
}