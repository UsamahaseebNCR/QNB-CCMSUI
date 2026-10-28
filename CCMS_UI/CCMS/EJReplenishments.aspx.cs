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
using System.Configuration;

namespace CCMSUI.CCMS
{
    public partial class EJReplenishments : BasePage
    {
        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!CheckRight((int)Permissions.ViewEJReplenishments))
                    Response.Redirect("../admin/AccessDenied.aspx");

                TextBox_FromDate.Text = DateTime.Today.AddMonths(-1).ToString("dd/MM/yyyy");
                TextBox_ToDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
                ViewState["SortExpression"] = "title";
                ViewState["sortDirection"] = " asc";
                ViewState["SortExpressionCC"] = "title";
                ViewState["sortDirectionCC"] = " asc";

            }


        }
        private string buildSearchCriteria()
        {
            StringBuilder builder = new StringBuilder();

            // atm_id is already in where clause
            if (TextBox_FromDate.Text.Length > 0)
                builder.Append(" AND rep_datetime >= Convert(datetime,'" + TextBox_FromDate.Text + "',103) ");

            if (TextBox_ToDate.Text.Length > 0)
                builder.Append(@"AND rep_datetime <=Convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103)");


            return builder.ToString();
        }
        private string buildSearchCriteria1()
        {
            StringBuilder builder = new StringBuilder();

            // atm_id is already in where clause
            if (TextBox_FromDate.Text.Length > 0)
                builder.Append(" AND clearing_datetime >= Convert(datetime,'" + TextBox_FromDate.Text + "',103) ");

            if (TextBox_ToDate.Text.Length > 0)
                builder.Append(@"AND clearing_datetime <=Convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103)");


            return builder.ToString();
        }


        void BindGrid()
        {
            EnableGridRespectiveColumns();
            LogableTask task = LogableTask.NewTask("BindGrid");
            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            SqlCommand cmd2 = ConnectionFactory.GetNewCommand(false);
            try
            {
                string ATMIds = Convert.ToString(Session[SessionVars.SelectedATMs.ToString()]);
                // Data Archiving Work
//                cmd.CommandText = @"select title,location, notes_added_type1,notes_added_type2,notes_added_type3,notes_added_type4,rep_datetime, last_tsn
//from ATM inner join vEjParsedReplenishments ON ATM.ATM_ID = vEjParsedReplenishments.ATM_ID "
//                + "where ATM.atm_id in (" + ATMIds + ") " + buildSearchCriteria() + "order by " + ViewState["SortExpression"] + " " + ViewState["sortDirection"];
                cmd.CommandText = @"select title,location, notes_added_type1,notes_added_type2,notes_added_type3,notes_added_type4,notes_added_type5,notes_added_type6,notes_added_type7,rep_datetime, last_tsn
from ATM inner join vEjParsedReplenishments ON ATM.ATM_ID = vEjParsedReplenishments.ATM_ID "
                + "where ATM.atm_id in (" + ATMIds + ") " + buildSearchCriteria() + "order by " + ViewState["SortExpression"] + " " + ViewState["sortDirection"];

//                cmd2.CommandText = @"select title,location,notes_dispensed_type1,notes_dispensed_type2,notes_dispensed_type3,notes_dispensed_type4,clearing_datetime,
//notes_remaining_type1,notes_remaining_type2,notes_remaining_type3,notes_remaining_type4
//from ATM inner join ej_notes_dispensed ON ATM.ATM_ID = ej_notes_dispensed.ATM_ID "
//                + "where ATM.atm_id in (" + ATMIds + ") " + buildSearchCriteria1() + "order by " + ViewState["SortExpressionCC"] + " " + ViewState["sortDirectionCC"]; ;

                cmd2.CommandText = @"select title,location,notes_dispensed_type1,notes_dispensed_type2,notes_dispensed_type3,notes_dispensed_type4,notes_dispensed_type5,notes_dispensed_type6,notes_dispensed_type7,clearing_datetime,
notes_remaining_type1,notes_remaining_type2,notes_remaining_type3,notes_remaining_type4,notes_remaining_type5,notes_remaining_type6,notes_remaining_type7
from ATM inner join ej_notes_dispensed ON ATM.ATM_ID = ej_notes_dispensed.ATM_ID "
                + "where ATM.atm_id in (" + ATMIds + ") " + buildSearchCriteria1() + "order by " + ViewState["SortExpressionCC"] + " " + ViewState["sortDirectionCC"]; ;


                DataTable dt = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Verbose, "loading data");
                adapter.Fill(dt);
                GridView_CashAdded.DataSource = dt;
                GridView_CashAdded.DataBind();
                Literal_CashAddedCount.Text = dt.Rows.Count + " records.";

                dt = new DataTable();
                adapter.SelectCommand = cmd2;
                adapter.Fill(dt);
                GridView_CountsCleared.DataSource = dt;
                GridView_CountsCleared.DataBind();
                Literal_CountsClearedCount.Text = dt.Rows.Count + " records.";
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
        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            btnSearch_Click(this, null);
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindGrid();
        }

        protected void Button_reset_Click(object sender, EventArgs e)
        {
            TextBox_FromDate.Text = DateTime.Today.AddMonths(-1).ToString("dd/MM/yyyy");
            TextBox_ToDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
        }
        protected void GridView_CashAdded_Sorting(object sender, GridViewSortEventArgs e)
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
            BindGrid();
        }




        protected void GridView_CashAdded_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.GridView_CashAdded.PageIndex = e.NewPageIndex;
            BindGrid();
        }
        protected void GridView_CountsCleared_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (ViewState["SortExpressionCC"].ToString() == e.SortExpression)
            {
                if (ViewState["sortDirectionCC"].ToString() == " asc")
                    ViewState["sortDirectionCC"] = " desc";
                else
                    ViewState["sortDirectionCC"] = " asc";
            }
            else
            {
                ViewState["SortExpressionCC"] = e.SortExpression;
                ViewState["sortDirectionCC"] = " asc";
            }
            BindGrid();
        }





        protected void GridView_CountsCleared_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.GridView_CountsCleared.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        private void EnableGridRespectiveColumns()
        {
            //Enable when Last Dispensed 24 Hour column need to display(ADIB)
            bool isShowLastTsn = ConfigurationManager.AppSettings["IsShowLastTsn"] != null ? bool.Parse(ConfigurationManager.AppSettings["IsShowLastTsn"]) : false;

            int noOfCassettes = ValidateNoOfCassettes(ConfigurationManager.AppSettings["NoOfCassettes"]);
            int columnIndexReplenishment = -1;
            int columnIndexDispensed = -1;
            int columnIndexRemaining = -1;
            columnIndexReplenishment = GetColumnIndexByName(this.GridView_CashAdded, "Type 5");
            columnIndexDispensed = GetColumnIndexByName(this.GridView_CountsCleared, "Dispensed Type 5");
            columnIndexRemaining = GetColumnIndexByName(this.GridView_CountsCleared, "Remaining Type 5");

            //Enable when Last Dispensed 24 Hour column need to display(ADIB)
            if (isShowLastTsn)
            {
                int columnIndexLastTsn = GetColumnIndexByName(this.GridView_CashAdded, "Last TSN");
                if (columnIndexLastTsn >= 0)
                    this.GridView_CashAdded.Columns[columnIndexLastTsn].Visible = true;
            }

            for(int i = 5; i <= noOfCassettes; i++)
            {
                if (columnIndexReplenishment >= 0)
                {
                    this.GridView_CashAdded.Columns[columnIndexReplenishment].Visible = true;
                    columnIndexReplenishment++;
                }

                if (columnIndexDispensed >= 0)
                {
                    this.GridView_CountsCleared.Columns[columnIndexDispensed].Visible = true;
                    columnIndexDispensed++;
                }
                if (columnIndexRemaining >= 0)
                {
                    this.GridView_CountsCleared.Columns[columnIndexRemaining].Visible = true;
                    columnIndexRemaining++;
                }
            }
            
            //for (int i = 5; i <= noOfCassettes; i++)
            //{
            //    switch(i)
            //    {
            //        case 5:
            //            columnIndexReplenishment = GetColumnIndexByName(this.GridView_CashAdded, "Type 5");
            //            columnIndexDispensed = GetColumnIndexByName(this.GridView_CountsCleared, "Dispensed Type 5");
            //            columnIndexRemaining = GetColumnIndexByName(this.GridView_CountsCleared, "Remaining Type 5");
            //            if (columnIndexReplenishment >= 0)
            //                this.GridView_CashAdded.Columns[columnIndexReplenishment].Visible = true;
            //            if (columnIndexDispensed >= 0)
            //                this.GridView_CountsCleared.Columns[columnIndexDispensed].Visible = true;
            //            if (columnIndexRemaining >= 0)
            //                this.GridView_CountsCleared.Columns[columnIndexRemaining].Visible = true;
            //            //BoundField newColumnName= new BoundField();

            //            //newColumnName.DataField = "notes_added_type5";
            //            //newColumnName.HeaderText = "notes_added_type5";
            //            //newColumnName.

            //            //GridView_CashAdded.Columns.Add(newColumnName);
            //            break;
            //        case 6:
            //            //this.GridView_CashAdded.Columns[8].Visible =true;
            //            //columnIndexReplenishment = GetColumnIndexByName(this.GridView_CashAdded, "Type 6");
            //            if (columnIndexReplenishment >= 0)
            //            {
            //                columnIndexReplenishment++;
            //                this.GridView_CashAdded.Columns[columnIndexReplenishment].Visible = true;
            //            }
            //            if (columnIndexDispensed >= 0)
            //            {
            //                columnIndexDispensed++;
            //                this.GridView_CountsCleared.Columns[columnIndexDispensed].Visible = true;
            //            }
            //            if (columnIndexRemaining >= 0)
            //            {
            //                columnIndexRemaining++;
            //                this.GridView_CountsCleared.Columns[columnIndexRemaining].Visible = true;
            //            }
            //            break;
            //        case 7:
            //            //this.GridView_CashAdded.Columns[9].Visible =true;
            //            //columnIndexReplenishment = GetColumnIndexByName(this.GridView_CashAdded, "Type 7");
            //            if (columnIndexReplenishment >= 0)
            //            {
            //                columnIndexReplenishment++;
            //                this.GridView_CashAdded.Columns[columnIndexReplenishment].Visible = true;
            //            }
            //            if (columnIndexDispensed >= 0)
            //            {
            //                columnIndexDispensed++;
            //                this.GridView_CountsCleared.Columns[columnIndexDispensed].Visible = true;
            //            }
            //            if (columnIndexRemaining >= 0)
            //            {
            //                columnIndexRemaining++;
            //                this.GridView_CountsCleared.Columns[columnIndexRemaining].Visible = true;
            //            }
            //            break;
            //        default:
            //            i = noOfCassettes;
            //            break;
            //    }
            //}
        }

        //private int GetColumnIndexByName(GridView grid, string name)
        //{
        //    foreach (DataControlField col in grid.Columns)
        //    {
        //        if (col.HeaderText.ToLower().Trim() == name.ToLower().Trim())
        //        {
        //            return grid.Columns.IndexOf(col);
        //        }
        //    }

        //    return -1;
        //}
    }
}