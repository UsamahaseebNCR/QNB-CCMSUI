using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Avanza.iSuite.UI;
using System.Text;
using System.Data.SqlClient;

using System.Data;
using System.Reflection;
using System.Diagnostics;
using Avanza.CCMS.DAL;
using System.Configuration;

namespace CCMSUI.CCMS
{
    public partial class ATMSettlementList : BasePage
    {
        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!CheckRight((int)Permissions.ViewATMSettlement))
                    Response.Redirect("../admin/AccessDenied.aspx");

                TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                ViewState["sortDirection"] = " asc";
                ViewState["SortExpression"] = "rep_datetime";
                BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
            }

        }


        protected void GridView_ATMSettlement_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                int atmSettlementID = int.Parse(((DataRowView)e.Row.DataItem).Row["atm_settlement_id"].ToString());
                if ((int)ConnectionFactory.ExecuteScalar("select count(*) from ccms_vault_ledger where atm_settlement_id = " + atmSettlementID) == 0)
                {
                    if (e.Row.FindControl("Button_deleteATMSettlement") != null)
                        ((ImageButton)e.Row.FindControl("Button_deleteATMSettlement")).Attributes.Add("onClick",
                            "return confirm('Are you sure you want to delete settlement for the ATM:" + ((DataRowView)e.Row.DataItem).Row["Title"] + "?','CCMS')");
                }
                else
                    ((ImageButton)e.Row.FindControl("Button_deleteATMSettlement")).Visible = false;

            }
        }


        private string buildSearchCriteria()
        {
            StringBuilder builder = new StringBuilder();
            string selectQuery = String.Empty;
            builder.Append(" where is_active = 1 AND");

            if (TextBox_FromDate.Text.Length > 0)
                builder.Append(" rep_datetime >= Convert(datetime,'" + TextBox_FromDate.Text + "',103) AND");

            if (TextBox_ToDate.Text.Length > 0)
                builder.Append("  rep_datetime <=Convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103) AND");


            builder.Append("  atm_site_and_number in (select cit_atm_title from atm where atm_id in  ("
                    + Session[SessionVars.SelectedATMs.ToString()] + ")) AND");

            //builder.Append("  gl_no in (select gl_number from atm where atm_id in  ("
            //    + Session[SessionVars.SelectedATMs.ToString()] + ")) AND");

            selectQuery = builder.ToString();
            selectQuery = selectQuery.Trim();
            if (selectQuery.EndsWith("AND"))
                selectQuery = selectQuery.Substring(0, selectQuery.LastIndexOf("AND"));
            return selectQuery;
        }

        void BindGrid(string orderBy)
        {
            LogableTask task = LogableTask.NewTask("BindGrid");
            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            try
            {
                ///<Summary>Edited by Ali Shah on 16th Oct, 2019
                ///To dynamically handling of cassettes more than 4
                EnableGridRespectiveColumns();

                cmd.CommandTimeout = 100;
                cmd.CommandText = @"select *,dbo.GetTotalText(note_set_type.denomination_type_1_title,note_set_type.denomination_type_2_title,note_set_type.denomination_type_3_title,note_set_type.denomination_type_4_title,note_set_type.denomination_type_1, note_set_type.denomination_type_2, note_set_type.denomination_type_3, note_set_type.denomination_type_4,cash_rep_counters_type1, cash_rep_counters_type2, cash_rep_counters_type3, cash_rep_counters_type4) total_Replenishment_text,
dbo.GetTotalText(note_set_type.denomination_type_1_title,note_set_type.denomination_type_2_title,note_set_type.denomination_type_3_title,note_set_type.denomination_type_4_title,note_set_type.denomination_type_1, note_set_type.denomination_type_2, note_set_type.denomination_type_3, note_set_type.denomination_type_4,cash_return_counters_type1, cash_return_counters_type2, cash_return_counters_type3, cash_return_counters_type4) total_returned_text,
dbo.GetTotalText(note_set_type.denomination_type_1_title,note_set_type.denomination_type_2_title,note_set_type.denomination_type_3_title,note_set_type.denomination_type_4_title,note_set_type.denomination_type_1, note_set_type.denomination_type_2, note_set_type.denomination_type_3, note_set_type.denomination_type_4,cash_rejected_counters_type1, cash_rejected_counters_type2, cash_rejected_counters_type3, cash_rejected_counters_type4) total_rejected_text,
dbo.GetTotalText(note_set_type.denomination_type_1_title,note_set_type.denomination_type_2_title,note_set_type.denomination_type_3_title,note_set_type.denomination_type_4_title,note_set_type.denomination_type_1, note_set_type.denomination_type_2, note_set_type.denomination_type_3, note_set_type.denomination_type_4,cash_rem_counters_type1, cash_rem_counters_type2, cash_rem_counters_type3, cash_rem_counters_type4) total_remaining_text,
dbo.GetTotalText(note_set_type.denomination_type_1_title,note_set_type.denomination_type_2_title,note_set_type.denomination_type_3_title,note_set_type.denomination_type_4_title,note_set_type.denomination_type_1, note_set_type.denomination_type_2, note_set_type.denomination_type_3, note_set_type.denomination_type_4,cash_dep_total_type1, cash_dep_total_type2, cash_dep_total_type3, cash_dep_total_type4) total_deposit_text
from atm_settlement
inner join atm on atm.cit_atm_title= atm_settlement.atm_site_and_number inner join note_set_type on atm.note_set_type_id = note_set_type.note_set_type_id   " + buildSearchCriteria() + " Order by " + orderBy;


                DataTable dt = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
                GridView_ATMSettlement.DataSource = dt;
                GridView_ATMSettlement.DataBind();
                Literal_count.Text = dt.Rows.Count + " records.";
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_error.Text = "<li>An Error occured. " + ex.Message + "</li>";
            }
            finally
            {
                task.EndTask();
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }


        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            btnSearch_Click(this, null);
        }

        protected void Button_reset_Click(object sender, EventArgs e)
        {
            TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");

        }


        protected void GridView_ATMSettlement_Sorting(object sender, GridViewSortEventArgs e)
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

        protected void GridView_ATMSettlement_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            LogableTask task = null;
            if (e.CommandName == "deleteATMSettlement")
            {
                SqlCommand cmd = null;
                SqlTransaction trx = null;
                try
                {
                    task = LogableTask.NewTask("delete atm settlement");
                    cmd = ConnectionFactory.GetNewCommand(true);
                    AtmSettlement atmSettlement = AtmSettlement.LoadAtmSettlementByPk(int.Parse(e.CommandArgument.ToString()));
                    trx = cmd.Connection.BeginTransaction();

                    cmd.Transaction = trx;
                    cmd.CommandText = "delete atm_settlement where atm_settlement_id = " + atmSettlement.AtmSettlementId + ";delete user_task_assignment where user_task_id in (select user_task_id from user_task where atm_settlement_id = " + atmSettlement.AtmSettlementId + ");delete user_task where atm_settlement_id = " + atmSettlement.AtmSettlementId + ";delete summary_correction_data where atm_settlement_id = " + atmSettlement.AtmSettlementId;
                    // cmd.CommandText = "delete atm_settlement where atm_settlement_id = " + atmSettlement.AtmSettlementId + ";delete user_task where atm_settlement_id = " + atmSettlement.AtmSettlementId + ";delete user_task_assignment where atm_settlement_id = " + atmSettlement.AtmSettlementId;
                    cmd.ExecuteNonQuery();

                    trx.Commit();

                    if (atmSettlement != null)
                    {
                        string msg = "Atm settlement for the atm " + atmSettlement.AtmNo + " for the date " + atmSettlement.RepDatetime + " deleted";
                        Utility.BuildAuditLog(msg, (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.DeleteATMSettlement);

                        //Literal_ok.Text = "<li>Selected atm was deleted!</li>";
                        Session[SessionVars.notice.ToString()] = "Selected atm settlement was deleted!";
                    }

                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "atm settlement deleted");
                    BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
                }
                catch (Exception ex)
                {
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                    Literal_error.Text = "<li>An Error occured while deleting ATM settlement. " + ex.Message + "</li>";
                    trx.Rollback();
                }
                finally
                {
                    cmd.Connection.Close();
                    task.EndTask();

                }
            }
        }



        protected void GridView_ATMSettlement_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.GridView_ATMSettlement.PageIndex = e.NewPageIndex;
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }

        private void EnableGridRespectiveColumns()
        {
            int noOfCassettes = ValidateNoOfCassettes(ConfigurationManager.AppSettings["NoOfCassettes"]);
            int columnIndexDenomination = -1;
            int columnIndexReplenishment = -1;
            int columnIndexRemaining = -1;
            columnIndexDenomination = GetColumnIndexByName(this.GridView_ATMSettlement, "Type 5");
            columnIndexReplenishment = columnIndexDenomination >= 0 ? (columnIndexDenomination + 1) : -1;
            columnIndexRemaining = columnIndexDenomination >= 0 ? (columnIndexDenomination + 3) : -1;

            for (int i = 5; i <= noOfCassettes; i++)
            {
                if (columnIndexDenomination >= 0)
                {
                    this.GridView_ATMSettlement.Columns[columnIndexDenomination].Visible = true;
                    columnIndexDenomination += 5;
                }

                if (columnIndexReplenishment >= 0)
                {
                    this.GridView_ATMSettlement.Columns[columnIndexReplenishment].Visible = true;
                    columnIndexReplenishment += 5;
                }
                if (columnIndexRemaining >= 0)
                {
                    this.GridView_ATMSettlement.Columns[columnIndexRemaining].Visible = true;
                    columnIndexRemaining += 5;
                }
            }


        }
    }
}