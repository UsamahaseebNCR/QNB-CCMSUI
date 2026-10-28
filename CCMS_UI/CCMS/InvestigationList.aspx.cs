using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Reflection;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using Avanza.iSuite.DAL;
using Avanza.CCMS.DAL;


namespace Avanza.iSuite.UI
{
    public partial class InvestigationList : BasePage
    {

        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {


                if (!CheckRight((int)Permissions.ViewInvestigation))
                    Response.Redirect("~/admin/AccessDenied.aspx");

                if (!CheckRight((int)Permissions.CreateInvestigation))
                    this.HyperLink_newInvestigation.Visible = false;

                if (!CheckRight((int)Permissions.EditInvestigation))
                    GridView_Investigation.Columns[8].Visible = false;



                ViewState["sortDirection"] = " asc";
                ViewState["SortExpression"] = "title";
                 
                AppUser.AppUserReader userReader = AppUser.ExecuteReader("1=1");
                Dropdownlist_user.Items.Add(new ListItem("Any", "Any"));
                while (userReader.Read())
                    Dropdownlist_user.Items.Add(new ListItem(userReader.CurrentAppUser.UserLogin, userReader.CurrentAppUser.UserId.ToString()));
                userReader.Close();

                TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");

                BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
            }

        }
        string Filter()
        {
            string filter = "";

            if (Dropdownlist_user.SelectedIndex > 0)
                filter += " and investigation.user_id = " + Dropdownlist_user.SelectedValue;
            if (Dropdownlist_Status.SelectedIndex > 0)
                filter += " and investigation.investigation_status = " + Dropdownlist_Status.SelectedValue;

            if (TextBox_FromDate.Text.Length > 0)
                filter += " and investigation.creation_time >= convert(datetime,'" + TextBox_FromDate.Text + "',103)";

            if (TextBox_ToDate.Text.Length > 0)
                filter += " and investigation.creation_time <= convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103)";

            filter += " and is_active =1 and atm.atm_id in ("
                    + Session[SessionVars.SelectedATMs.ToString()] + ")";

            return filter;
        }

        private void BindGrid(string orderby)
        {
            LogableTask task = LogableTask.NewTask("BindGrid");
            try
            {
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandText = @"SELECT title,investigation.creation_time,order_number, case when investigation_reason_id =0 then 'Order was missed' 
when  investigation_reason_id =1 then 'Order was replenished delayed/earlier' 
when  investigation_reason_id =2 then 'Replenished mode was different'
end  investigation_reason_text  ,modification_time,investigation_id,case when investigation_status = 0 then 'Open' else 'Closed' end status,user_full_name,user_task_id,investigation.resolution_time
                                    from investigation inner join atm on atm.atm_id = investigation.atm_id inner join app_user on app_user.user_id =investigation.user_id left join user_task on user_task.entity_id = investigation_id   where atm.is_active=1 " + Filter() + " order by " + orderby;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                cmd.Connection.Close();
                this.GridView_Investigation.DataSource = dt;
                this.GridView_Investigation.DataBind();
                Literal_count.Text = dt.Rows.Count + " records.";
            }
            catch (Exception ex)
            { task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex); }
            finally
            { task.EndTask(); }
        }

        protected void GridViewInvestigation_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (ViewState["SortExpression"].ToString() == e.SortExpression)
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
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }



        protected void GridView_Investigation_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.GridView_Investigation.PageIndex = e.NewPageIndex;
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }

      

        protected void Button_search_Click(object sender, EventArgs e)
        {
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }

        protected void Button_reset_Click(object sender, EventArgs e)
        {
            TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
        }

        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            Button_search_Click(this, null);
        }


    }
}