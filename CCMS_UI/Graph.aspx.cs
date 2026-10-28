using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Avanza.iSuite.UI;
using System.Drawing;

namespace CCMSUI
{
    public partial class Graph : BasePage
    {
        protected void Page_PreInit(object sender, EventArgs e)
        {
            refresh.ServerClick += new System.EventHandler(refresh_ServerClick);
        }
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                if (!CheckRight((int)Permissions.ViewGraphList))
                    Response.Redirect("admin/AccessDenied.aspx");
                TextBox_From.Text = DateTime.Now.ToString("dd/MM/yyyy");
                TextBox_To.Text = DateTime.Now.ToString("dd/MM/yyyy");

                //GenerateGraph();
            }
        }
        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            //Session[SessionVars.SelectedATMs.ToString()] = e.Parameter.Substring(1);

            //if (bool.Parse(Session[SessionVars.isRegionSelected.ToString()].ToString()))
            //{
            //    //  Session[SessionVars.SelectedRegion.ToString()] = e.Parameter.Substring(1);
            //}
            //else
            //{
            if (Session[SessionVars.SelectedATMs.ToString()] == null)
                Literal_Message.Text = "Please select an ATM or region from the tree";
            else
            {
                GenerateGraph(Session[SessionVars.SelectedATMs.ToString()].ToString());
            }
            //            }




            //Button_search_Click(this, null);
        }

        private void GenerateGraph(string atmId)
        {
            LogableTask task = LogableTask.NewTask("DrawGraph");
            try
            {
                if (Request.QueryString["gid"] != "")
                {
                    Literal_Title.Text = Request.QueryString["title"];
                    task.Log(System.Reflection.MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Info, "Graph Id = " + Request.QueryString["gid"]);
                    Response.CacheControl = "no-cache";
                    Chart_ShowGraph.Url = "Data.aspx?param=" + Request.QueryString["gid"] + "@" + DateTime.Today.ToString("dd/MM/yyyy") + "@" + DateTime.Today.AddDays(1).ToString("dd/MM/yyyy") + "@" + atmId + "@2@167AD4";
                    Chart1.Url = "Data.aspx?param=" + Request.QueryString["gid"] + "@" + DateTime.Today.AddDays(-7).ToString("dd/MM/yyyy") + "@" + DateTime.Today.AddDays(-1).ToString("dd/MM/yyyy") + "@" + atmId + "@0@167AD4";
                    Chart2.Url = "Data.aspx?param=" + Request.QueryString["gid"] + "@" + DateTime.Today.AddDays(-14).ToString("dd/MM/yyyy") + "@" + DateTime.Today.AddDays(-8).ToString("dd/MM/yyyy") + "@" + atmId + "@0@2A9c0A";
                    Chart3.Url = "Data.aspx?param=" + Request.QueryString["gid"] + "@" + TextBox_From.Text + "@" + TextBox_To.Text + "@" + atmId + "@1@2A9c0A";
                    if (Request.QueryString["gid"] == "1")
                    {
                        Literal1.Text = "<b>Today [" + DateTime.Today.ToString("dd/MM/yyyy") + "]</b><br/>ATM transactions for today";
                        Literal2.Text = "<b>Last Week [" + DateTime.Today.AddDays(-7).ToString("dd/MM/yyyy") + "-" + DateTime.Today.AddDays(-1).ToString("dd/MM/yyyy") + "]</b><br/>ATM transactions for last week";
                        Literal3.Text = "<b>Previous Week [" + DateTime.Today.AddDays(-14).ToString("dd/MM/yyyy") + "-" + DateTime.Today.AddDays(-8).ToString("dd/MM/yyyy") + "]</b><br/>ATM transactions for previous week"; ;
                        Literal4.Text = "<b>Selected Criteria [" + TextBox_From.Text + "-" + TextBox_To.Text + "]</b><br/>ATM transactions for specified duration"; ;
                     }


                    //Chart1.Url = "Data.aspx?graphId=" + Request.QueryString["gid"] + "&&fromDate=21/08/2011&&toDate=21/08/2011?AtmId=1041";
                    //Chart2.Url = "Data.aspx?graphId=" + Request.QueryString["gid"] + "&fromDate=21/08/2011&toDate=21/08/2011?AtmId=1041";
                    //Chart3.Url = "Data.aspx?graphId=" + Request.QueryString["gid"] + "&fromDate=21/08/2011&toDate=21/08/2011?AtmId=1041";
                    //Chart_ShowGraph.BorderWidth = 2;
                    //Chart_ShowGraph.BorderStyle = BorderStyle.Dotted;
                    // Chart_ShowGraph.BorderColor = Color.Black;
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
        protected void Button_Submit_Click(object sender, EventArgs e)
        {
            //Session[SessionVars.SelectedATMs.ToString()] = e.Parameter.Substring(1);

            if (Session[SessionVars.SelectedATMs.ToString()] == null)
                Literal_Message.Text = "Please select an ATM or region from the tree";
            else
            {
                GenerateGraph(Session[SessionVars.SelectedATMs.ToString()].ToString());
            }


        }
    }
}