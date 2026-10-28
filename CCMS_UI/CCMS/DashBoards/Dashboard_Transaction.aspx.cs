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
    public partial class Dashboard_Transaction : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
            }
        }


        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Search");
            StringBuilder builder = new StringBuilder();
            try
            {
                string pageName = "TransactionDashboard.aspx";

                pageName += "?to=" + TextBox_ToDate.Text + "&from=" + TextBox_FromDate.Text;

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