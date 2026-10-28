using Avanza.CCMS.DAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.Script.Serialization;

namespace CCMSUI.admin
{
    /// <summary>
    /// Summary description for Handler
    /// </summary>
    public class Handler : IHttpHandler
    {
        //private static SemaphoreSlim uploadSemaphore = new SemaphoreSlim(1);

        public void ProcessRequest(HttpContext context)
        {
            LogableTask.LogMonoActivityTask("", MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Info, $"packageTitle[{context.Request.QueryString["packageTitle"]}]");
            LogableTask.LogMonoActivityTask("", MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Info, $"fileName[{context.Request.Form["fileName"]}]");
            LogableTask.LogMonoActivityTask("", MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Info, $"filecount[{context.Request.Files.Count}]");

            string json = string.Empty;
            //object insertLock = new object();

            if (context.Request.Files.Count > 0)
            {
                try
                {
                    FileInfo fileToUploadInfo = null;
                    string packageTitle = context.Request.QueryString["packageTitle"];

                    string checkForDuplicatesSQL = null;
                    checkForDuplicatesSQL = "select count(*) from package where progress = 'completed' and package_title ='" + packageTitle + "'";

                    HttpPostedFile postedFile = context.Request.Files[0];
                    LogableTask.LogMonoActivityTask("", MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Info, "posted file len:" + postedFile.ContentLength);

                    string fileName = context.Request.Form["fileName"];

                    if ((int)ConnectionFactory.ExecuteScalar(checkForDuplicatesSQL) <= 0)
                    {
                        AppSetting appSetting = AppSetting.LoadAppSetting("1=1");
                        string fileToUploadTempFolder = appSetting.TemporaryFolder + "\\PendingUploads\\" + packageTitle + "_" + DateTime.Now.ToString("ddMMyyy");

                        fileToUploadInfo = new FileInfo(fileToUploadTempFolder + "\\" + Path.GetFileName(fileName));
                        if (!Directory.Exists(fileToUploadInfo.DirectoryName))
                            Directory.CreateDirectory(fileToUploadInfo.DirectoryName);

                        try
                        {
                            //using (uploadSemaphore.WaitAsync()) // Acquire semaphore
                            //{
                            Avanza.CCMS.DAL.Package package = Avanza.CCMS.DAL.Package.LoadPackage(" progress = 'pending' and package_title = '" + packageTitle + "'");
                            if (package != null)
                            {
                                package.UploadedBytes += postedFile.ContentLength;
                                package.Save();
                            }
                            else
                            {
                                package = new Avanza.CCMS.DAL.Package();
                                package.PackageTitle = packageTitle;
                                package.PackageCreationTime = DateTime.Now;
                                package.CreatedBy = int.Parse(context.Request.Form["CreatedBy"]);
                                package.PackagePhyscialPath = fileToUploadTempFolder + "\\" + GetFilenameBeforeChunk(fileName);
                                package.PackageSize = long.Parse(context.Request.Form["fileSize"]);
                                package.UploadedBytes = long.Parse(context.Request.Form["uploadedBytes"]);
                                package.Progress = "pending";
                                package.Save();
                            }
                            //}
                        }
                        catch (Exception ex)
                        {
                            LogableTask.LogMonoActivityTask("", MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Error, ex);

                        }

                        postedFile.SaveAs(fileToUploadInfo.FullName);

                        json = new JavaScriptSerializer().Serialize(
                        new
                        {
                            response = "OK"
                        });
                    }
                    else
                    {
                        json = new JavaScriptSerializer().Serialize(
                            new
                            {
                                response = "Package with the same title already exists."
                            });
              
                    }
                }
                catch (Exception ex)
                {
                    json = new JavaScriptSerializer().Serialize(
                        new
                        {
                            response = ex.Message
                        });
                    LogableTask.LogMonoActivityTask("", MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Error, ex);

                }
                context.Response.StatusCode = (int)HttpStatusCode.OK;
                context.Response.ContentType = "text/json";
                context.Response.Write(json);
                LogableTask.LogMonoActivityTask("", MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Info, "content len:" + context.Request.ContentLength);

                context.Response.End();
            }
        }

        public static string GetFilenameBeforeChunk(string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                return filename;
            }

            const string chunkSeparator = "_chunk";
            int chunkIndex = filename.IndexOf(chunkSeparator, StringComparison.Ordinal);

            return chunkIndex > -1 ? filename.Substring(0, chunkIndex) : filename;
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