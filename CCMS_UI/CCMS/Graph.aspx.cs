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
using Avanza.iSuite.UI;
using System.Drawing;

namespace iJournalUI
{
    public partial class Graph : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                if (!CheckRight((int)Permissions.ViewGraphList))
                    Response.Redirect("admin/AccessDenied.aspx");
            }
            LogableTask task = LogableTask.NewTask("DrawGraph");
            try
            {
                if (Request.QueryString["gid"] != "")
                {
                    Literal_Title.Text = Request.QueryString["title"];
                    task.Log(System.Reflection.MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Info, "Graph Id = " + Request.QueryString["gid"]);
                    Response.CacheControl = "no-cache";
                    Chart_ShowGraph.Url = "Data.aspx?graphId=" + Request.QueryString["gid"];
                    Chart_ShowGraph.BorderWidth = 2;
                    Chart_ShowGraph.BorderStyle = BorderStyle.Dotted;
                    Chart_ShowGraph.BorderColor = Color.Black;
                }
            }
            catch (Exception ex)
            {
                task.Log(System.Reflection.MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Error, ex.Message);
                Literal_Message.Text = "<li>" + ex.Message + "</li>";
            }
            finally
            {
                task.EndTask();
            }
        }
    }
}