using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Avanza.iSuite.UI;
using System.Data.SqlClient;

using Avanza.CCMS.DAL;
using System.Reflection;
using System.Data;
using System.Diagnostics;

namespace CCMSUI.CCMS.DashBoards
{
    public partial class DashBoardGird_MinimumThreshold : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!CheckRight((int)Permissions.ViewMinThresholdDashboard, true))
                    Response.Redirect("../../admin/aspx/AccessDenied.aspx");

                ViewState["SortExpression"] = "total ";
                ViewState["sortDirection"] = "asc ";
                BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());

            }
        }

        private void BindGrid(string orderBy)
        {
            SqlCommand cmd = null;
            LogableTask task = LogableTask.NewTask("BindGrid");

            try
            {
                metaRefresh.Content = (((AppSetting)Application[ApplicationVars.AppSettings.ToString()]).DashboardRefreshInterval * 60).ToString();
                Page.RegisterStartupScript("script", "<script>function init() {cd(\"" + (int.Parse(metaRefresh.Content) / 60) + "\");} init();</script>");

                cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;
                cmd.CommandText = @"select top 1000 outerATM.min_operating_balance,outerATM.atm_id,outerATM.title title, note_set_type.denomination_type_1, note_set_type.denomination_type_2, note_set_type.denomination_type_3,
note_set_type.denomination_type_4, note_set_type.denomination_type_5, 
note_set_type.denomination_type_6, note_set_type.denomination_type_7, 
p.last_trxn_at, p.cassette1_notes, p.cassette2_notes,
p.cassette3_notes, p.cassette4_notes, p.cassette5_notes, p.cassette6_notes,
p.cassette7_notes,
note_set_type.denomination_type_1 * p.cassette1_notes + 
note_set_type.denomination_type_2 * p.cassette2_notes + 
note_set_type.denomination_type_3 * p.cassette3_notes + 
note_set_type.denomination_type_4 * p.cassette4_notes + 
note_set_type.denomination_type_5 * p.cassette5_notes + 
note_set_type.denomination_type_6 * p.cassette6_notes + 
note_set_type.denomination_type_7 * p.cassette7_notes total 
from atm outerATM , cash_position p ,note_set_type
where outerATM.atm_id = p.atm_id and outerATM.is_active = 1 and outerATM.exclude_dff = 0 
and outerATM.note_set_type_id = note_set_type.note_set_type_id 
and cash_position_id in (select max(cash_position_id)
from cash_position 
where cash_position.atm_id = outerATM.atm_id)
and note_set_type.denomination_type_1 * p.cassette1_notes + 
note_set_type.denomination_type_2 * p.cassette2_notes + 
note_set_type.denomination_type_3 * p.cassette3_notes + 
note_set_type.denomination_type_4 * p.cassette4_notes + 
note_set_type.denomination_type_5 * p.cassette5_notes + 
note_set_type.denomination_type_6 * p.cassette6_notes + 
note_set_type.denomination_type_7 * p.cassette7_notes < outerATM.min_operating_balance
and outerATM.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ")  order by " + orderBy;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                GridView_ATM.DataSource = dt;
                GridView_ATM.DataBind();

                
               
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_errorMsg.Text = "<li>" + ex.Message + "</li>";
            }
            finally
            {
                if (cmd != null)
                    if (cmd.Connection != null)
                        if (cmd.Connection.State == ConnectionState.Open)
                            cmd.Connection.Close();

                task.EndTask();
            }
        }
        protected void GridView_ATM_Sorting(object sender, GridViewSortEventArgs e)
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

       
    }
}