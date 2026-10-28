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
using System.Data.SqlClient;

using System.Reflection;
using System.Diagnostics;
using System.Text;
using Avanza.iSuite.UI;
using Avanza.CCMS.DAL;

namespace CCMSUI.CCMS.DashBoards
{
    public partial class Dashboard_Executive : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                TextBox_FromDate.Text = DateTime.Today.ToString("dd/MM/yyyy HH:mm:ss");
                TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Search");
            StringBuilder builder = new StringBuilder();
            try
            {
                string pageName = "../ExecDashboard.aspx?fromDate=" + DateTime.ParseExact(TextBox_FromDate.Text, "dd/MM/yyyy HH:mm:ss", null) +
                    "&toDate=" + DateTime.ParseExact(TextBox_ToDate.Text, "dd/MM/yyyy HH:mm:ss", null);

                Literal_Scr.Text = "<script>javascript:window.open('" + pageName + "','','location=0,status=1,resizable=yes,scrollbars=yes');</script>";
            }

            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_ok.Text = "<li>" + ex.Message + "</li>";
            }
            finally
            {
                task.EndTask();
            }
        }
    }
}