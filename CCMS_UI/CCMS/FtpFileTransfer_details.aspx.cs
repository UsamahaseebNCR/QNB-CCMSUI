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
using Avanza.CCMS.DAL;
using Avanza.iSuite.UI;

namespace CCMSUI.CCMS
{
    public partial class FtpFileTransfer_details : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                FtpFileInfo fileInfo = FtpFileInfo.LoadFtpFileInfoByPk(int.Parse(Request.QueryString["file_id"]));
                
                Label_organization.Text = Region.LoadRegionByPk(fileInfo.RegionId).RegionName;
                Label_tasktype.Text = TaskType.LoadTaskTypeByPk(fileInfo.TaskTypeId).TaskTypeName;
                Label_filename.Text = fileInfo.FtpFilename;
                Label_status.Text = fileInfo.Status;
                Label_creationtime.Text = fileInfo.CreationTime.ToString("dd/MM/yyyy HH:mm:ss");
                if (fileInfo.EndTime.HasValue)
                    Label_endtime.Text = fileInfo.EndTime.Value.ToString("dd/MM/yyyy HH:mm:ss");
                if (fileInfo.LastInvokedAt.HasValue)
                    Label_lastinvoked.Text = fileInfo.LastInvokedAt.Value.ToString("dd/MM/yyyy HH:mm:ss");
                Label_retry.Text = fileInfo.RetryCount.ToString();
                Label_reason.Text = fileInfo.FailureReason;
            }
        }
    }
}