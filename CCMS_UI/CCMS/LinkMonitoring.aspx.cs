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
using Avanza.CCMS.DAL;
using Avanza.iSuite.DAL;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using System.Web.Caching;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;


namespace CCMSUI.CCMS
{
    public partial class LinkMonitoring : BasePage
    {
        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        }
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!Page.IsPostBack)
            {
                if (!CheckRight((int)Permissions.TaskStatusMonitoringHistory, true))
                    Response.Redirect("~/admin/AccessDenied.aspx");
                button_search.Attributes.Add("onclick", " this.disabled = true; " + ClientScript.GetPostBackEventReference(button_search, null) + ";");

                Dropdownlist_Link.Items.Add(new ListItem("All", "All"));
                AtmInterfaceInfo.AtmInterfaceInfoReader reader = AtmInterfaceInfo.ExecuteReader("1=1");
                while (reader.Read())
                    Dropdownlist_Link.Items.Add(new ListItem(reader.CurrentAtmInterfaceInfo.InterfaceFriendlyName, reader.CurrentAtmInterfaceInfo.AtmInterfaceInfoId.ToString()));
                reader.Close();

                // trFileTypes.Visible = true;
                //GridView_Files.Columns[3].Visible = true;
                TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy") + " 00:00:00";

                TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy") + " 23:59:59";


                ViewState["SortExpression"] = "title";
                ViewState["sortDirection"] = " asc";
                ViewState["orderby"] = ViewState["SortExpression"] + " " + ViewState["sortDirection"];


                //if (Request.Params["file_id"] != null)
                //    BinxdGrid(int.Parse(Request.Params["file_id"]));
                //else
                //BindGrid(null);
            }
        }


        protected void GridView_Link_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView_Link.PageIndex = e.NewPageIndex;
            //BindGrid(null);
            BindGridWithCache();
        }

        protected void GridView_Link_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (ViewState["SortExpression"].ToString().IndexOf(e.SortExpression) >= 0)
            {
                if (ViewState["sortDirection"].ToString() == " asc")
                    ViewState["sortDirection"] = " desc";
                else
                    ViewState["sortDirection"] = " asc";
            }
            else
            {
                ViewState["SortExpression"] = e.SortExpression;
                ViewState["sortDirection"] = " asc";
            }
            //if (ViewState["SortExpression"].ToString() == "creation_time")
            //    ViewState["SortExpression"] = "f.creation_time";

            ViewState["orderby"] = ViewState["SortExpression"] + " " + ViewState["sortDirection"];
            ///BindGrid(null);
            BindGridWithCache();
        }



        private void BindGridWithCache()
        {
            DataTable dtTask = (DataTable)Cache["dtTasks"];
            dtTask.DefaultView.Sort = ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString();
            this.GridView_Link.DataSource = dtTask;
            this.GridView_Link.DataBind();
            Literal_count.Text = dtTask.Rows.Count + " records.";

        }
        void BindGrid(int? file_id)
        {
            LogableTask task = LogableTask.NewTask("BindDownloadedFilesGrid");
            try
            {
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;
                cmd.CommandText = @"select title,interface_friendly_name,atm_interface_info.ip,from_date,to_date,interface_status 
                                    from atm_interface_info inner join atm_interface_info_history
                                    on atm_interface_info.atm_interface_info_id = atm_interface_info_history.atm_interface_info_id
                                    inner join atm on atm.atm_id=atm_interface_info.atm_id
                                     where ATM.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and atm.IS_ACTIVE =1 " + GetFilter() + " order by " + ViewState["orderby"];

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dtATMs = new DataTable();
                adapter.Fill(dtATMs);
                Cache["dtTasks"] = dtATMs;
                Literal_count.Text = dtATMs.Rows.Count + " records.";
                this.GridView_Link.DataSource = dtATMs;
                this.GridView_Link.DataBind();


            }



            catch (Exception ex)
            {
                Literal_error.Text = ex.Message;
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            { task.EndTask(); }
        }



        protected void Button_Reset_Click(object sender, EventArgs e)
        {
            TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");



        }
        string GetFilter()
        {
            string filter = "";
            if (TextBox_FromDate.Text.Length > 0)
                filter += " and from_date >= convert(datetime,'" + TextBox_FromDate.Text + "',103)";

            if (TextBox_ToDate.Text.Length > 0)
                filter += " and to_date <= convert(datetime,'" + TextBox_ToDate.Text + "',103)";
            if (Dropdownlist_Link.SelectedIndex > 0)
                filter += " and atm_interface_info.atm_interface_info_id= " + Dropdownlist_Link.SelectedItem.Value;
            

            return filter;
        }
        protected void button_search_Click(object sender, EventArgs e)
        {
            BindGrid(null);
        }
        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            button_search_Click(this, null);
        }









    }
}