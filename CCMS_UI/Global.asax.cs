using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.IO;
using Avanza.CCMS.DAL;
using System.Diagnostics;

using Microsoft.Win32;
using System.Collections.Generic;

namespace CCMSUI
{
    public class Global : System.Web.HttpApplication
    {
        private static object sync = new object();
        static string ZipDir = (string)Registry.LocalMachine.OpenSubKey(@"SOFTWARE\NCR\CCMS", false).GetValue("ZipDir", "");
        protected void Application_Start(object sender, EventArgs e)
        {
            try
            {
                string connStr = (string)Registry.LocalMachine.OpenSubKey(@"SOFTWARE\NCR\CCMS", false).GetValue("ConnectionString", "");
                //File.WriteAllText("C:\\view360LiveData\\logs\\conn", connStr);
                //connStr = Encryption.Cryptic.DecryptString(connStr);
                if (connStr == "")
                    throw new Exception("Connection string is not initialized");
                else
                    connStr = Encryption.Cryptic.DecryptString(connStr);
               
                ConnectionFactory.Initialize(connStr, true);
            
                AppSetting appSettings = AppSetting.LoadAppSetting("1=1");
                //File.WriteAllText("C:\\view360LiveData\\logs\\conn3", "app settings loaded");

                if (appSettings != null)
                {
                    XmlLogWriter.InitXmlLogWriter(String.Format("{0}\\CCMS_UI_{1:yyMMMdd_HHmmss}.txt", appSettings.LogFilePath, DateTime.Now));//                          
                    Application["DBConnected"] = true;
                    Application[ApplicationVars.AppSettings.ToString()] = appSettings;
                    Application["sync"] = sync;
                    Application["ActiveSessions"] = new List<ActiveSessions>();
                    LogableTask.LogMonoActivityTask("DisplayVersion", System.Reflection.MethodBase.GetCurrentMethod(), TraceLevel.Info, "Version : CCMS Build 1.0.0.0 Modified Date :5 June 2013");
                }

                ConnectionFactory.ExecuteQuery("update app_user set is_currently_logged_in=0");
                //else
                  //  File.WriteAllText("C:\\view360LiveData\\logs\\conn4", "app settings is null");

            }
            catch(Exception ex)
            {
                Application[ApplicationVars.initError.ToString()] = ex.Message + " " + ex.StackTrace;
                XmlLogWriter.InitXmlLogWriter("C:\\CCMSLiveData\\Logs\\CCMS_UI_" + DateTime.Now.ToString("yyMMMdd_HHmmss") + ".txt");
                LogableTask.LogMonoActivityTask("Loading", System.Reflection.MethodBase.GetCurrentMethod(), TraceLevel.Error, ex.Message + " " + ex.StackTrace);
                try
                {
                    LogableTask.LogMonoActivityTask("Loading", System.Reflection.MethodBase.GetCurrentMethod(), TraceLevel.Error, ex.Message + " " + ex.StackTrace);
                }
                catch (Exception ex1)
                {
                    Application[ApplicationVars.initError.ToString()] = (string)Application[ApplicationVars.initError.ToString()] + " " + ex1.Message;
                    LogableTask.LogMonoActivityTask("Loading", System.Reflection.MethodBase.GetCurrentMethod(), TraceLevel.Error, ex1.Message);
                }

                try
                {
                    EventLog.WriteEntry("ccms", ex.Message, EventLogEntryType.Error);
                    LogableTask.LogMonoActivityTask("Loading", System.Reflection.MethodBase.GetCurrentMethod(), TraceLevel.Error, ex.Message);
                }
                catch 
                {         }
                Application["DBConnected"] = false;
            }
        }

        protected void Application_End(object sender, EventArgs e)
        {

        }

        protected void Session_Start(object sender, EventArgs e)
        {           
            Session[SessionVars.SelectedATMs.ToString()] = "-1";
            Session[SessionVars.isRegionSelected.ToString()] = false;
          
        }

        protected void Session_End(object sender, EventArgs e)
        {
            Session.Abandon();
            Session.RemoveAll();
        }


        //protected void Application_PreSendRequestHeaders()
        //{
        //    // Response.Headers.Remove("Server");
        //    Response.Headers.Remove("Server");
        //    Request.Headers.Remove("X-HTTP-METHOD");
        //    Request.Headers.Remove("X-HTTP-Method-Override");
        //    Request.Headers.Remove("X-METHOD-OVERRIDE");
        //    Response.Headers.Remove("Expires");
        //    Response.Headers.Remove("Cache-Control");
        //    Response.Headers.Remove("Connection");
        //    Response.Headers.Remove("Date");
        //    Response.Headers.Remove("Content-Type");
        //}

        //protected void Application_BeginRequest(object sender, EventArgs e)
        //{
        //    var application = sender as HttpApplication;
        //    if (application != null && application.Context != null)
        //    {
        //        application.Context.Response.Headers.Remove("Server");
        //    }
        //}


        void Application_BeginRequest(object sender, EventArgs e)
        {
            this.Response.Headers["X-Content-Type-Options"] = "nosniff";
        }

        protected void Application_PreSendRequestHeaders()
        {
            Response.Headers.Remove("Server");
        }

       
    }
}