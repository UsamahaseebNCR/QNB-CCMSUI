using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using System.Text;

using System.Data.SqlClient;
using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using Avanza.iSuite.UI;
using Avanza.CCMS.DAL;
using System.Globalization;

namespace CCMSUI.CCMS
{
    public partial class FtpFileTransferViewer : BasePage
    {
        protected System.Web.UI.WebControls.Button Button_Apply;
        Regex formattingRegex = new Regex(Utility.GetFormatRegex());
        protected string formattedStr_RawView, formattedStr_Exception, selectedTransactionEntry;
        protected StringBuilder result;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["file_id"] != null)
                Session["file_id"] = Request.QueryString["file_id"];
                

            byte[] bytes = null;
            FileStream file = null;
            string path = "", path_unprocessed = "";

            FtpFileInfo fileInfo = FtpFileInfo.LoadFtpFileInfoByPk(int.Parse(Session["file_id"].ToString()));
            TaskType taskType = TaskType.LoadTaskTypeByPk(fileInfo.TaskTypeId);

            if (taskType.TaskTypeId == Convert.ToInt32(Avanza.CCMS.DAL.EnumTaskType.DailyFeedUpload))
            {
                tdHeading.InnerText = "Daily Feed File Viewer";
                Button_DisplayException.Visible = false;
                Button_DisplayCashOrder.Visible = false;
                spanTitle.InnerText = "Daily Feed File";
                if (File.Exists(fileInfo.FtpFilename))
                    path = fileInfo.FtpFilename;
                else
                    path = fileInfo.FtpFilename.Replace("PendingUpload","OutputArchive");
                //path_unprocessed = fileInfo.FtpFilename.Insert(fileInfo.FtpFilename.LastIndexOf("."), "_unprocessed");
            }
            else
            {
                path = Region.LoadRegionByPk(fileInfo.RegionId).CashOrderDownloadedFilePath + "\\" + fileInfo.FtpFilename;
                path_unprocessed = Region.LoadRegionByPk(fileInfo.RegionId).CashOrderDownloadedFilePath + "\\" + fileInfo.FtpFilename+ "_unprocessed";
            }
            
            file = new FileStream(path, FileMode.Open, FileAccess.Read);
            bytes = new byte[file.Length];
            file.Read(bytes, 0, bytes.Length);
            formattedStr_RawView = Encoding.ASCII.GetString(bytes);
            file.Close();
            if (taskType.TaskTypeId != Convert.ToInt32(Avanza.CCMS.DAL.EnumTaskType.DailyFeedUpload))
            {
                if (File.Exists(path_unprocessed))
                {
                    file = new FileStream(path_unprocessed, FileMode.Open, FileAccess.Read);
                    bytes = new byte[file.Length];
                    file.Read(bytes, 0, bytes.Length);
                    formattedStr_Exception = Encoding.ASCII.GetString(bytes);
                    file.Close();
                }
                else
                    formattedStr_Exception = "";
            }
            else
            {
                formattedStr_Exception = "";
               // Div_Exception.Visible = false;
               // Div_Exception_Label.Visible = false;
            }
        }

    }
}