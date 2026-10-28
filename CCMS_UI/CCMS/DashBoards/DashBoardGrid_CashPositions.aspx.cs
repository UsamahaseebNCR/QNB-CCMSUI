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
using ceTe.DynamicPDF;
using System.Threading;

namespace CCMSUI.CCMS.DashBoards
{
    public partial class DashBoardGrid_CashPositions : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!CheckRight((int)Permissions.ViewCashPositionsDashBoard, true))
                    Response.Redirect("../../admin/aspx/AccessDenied.aspx");

                ViewState["SortExpression"] = "total ";
                ViewState["sortDirection"] = "asc ";
                BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());

            }
        }

        private void BindGrid(string orderBy)
        {
            ///<Summary>Edited by Ali Shah on 09th Oct, 2019
            ///To dynamically handling of cassettes more than 4
            EnableGridRespectiveColumns();
            SqlCommand cmd = null;
            LogableTask task = LogableTask.NewTask("BindGrid");

            try
            {
                this.metaRefresh.Content = (((AppSetting)base.Application[ApplicationVars.AppSettings.ToString()]).DashboardRefreshInterval * 60).ToString();
                this.Page.RegisterStartupScript("script", "<script>function init() {cd(\"" + (int.Parse(this.metaRefresh.Content) / 60) + "\");} init();</script>");
                cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetCashPositions";
                cmd.Parameters.Add("@Date", SqlDbType.VarChar);
                cmd.Parameters.Add("@NoteSetTypeId", SqlDbType.Int);
                cmd.Parameters.Add("@MinNotesAlertExists", SqlDbType.Int);
                cmd.Parameters.Add("@OrderBy", SqlDbType.VarChar);
                cmd.Parameters.Add("@AtmId", SqlDbType.VarChar);

                cmd.Parameters[0].Value = DateTime.Today.ToString("dd/MM/yyyy");
                
                if (Request.QueryString["n"] != null)
                    cmd.Parameters[1].Value = int.Parse(Request.QueryString["n"]);
                else
                    cmd.Parameters[1].Value = 0;

                if (Request.QueryString["f"] != null)
                    cmd.Parameters[2].Value = int.Parse(Request.QueryString["f"]);
                else
                    cmd.Parameters[2].Value = 0;

                cmd.Parameters[3].Value = orderBy;
                //cmd.Parameters[4].Value = "";
                cmd.Parameters[4].Value = Session[SessionVars.SelectedATMs.ToString()];

                //cmd.CommandText = "select top 1000 outerATM.atm_id,outerATM.title title, note_set_type.denomination_type_1, " +
                //    " note_set_type.denomination_type_2, note_set_type.denomination_type_3,note_set_type.denomination_type_4 " +
                //" , note_set_type.denomination_type_5, note_set_type.denomination_type_6, note_set_type.denomination_type_7, " +
                //" (select max(trxn_datetime) from parsed_transaction where atm_id = outerATM.atm_id and amount>0 )last_trxn_at, " +
                //" p.cassette1_notes, p.cassette2_notes, " +
                // " p.cassette3_notes, p.cassette4_notes, p.cassette5_notes, p.cassette6_notes, " +
                //" p.cassette7_notes, note_set_type.denomination_type_1 * p.cassette1_notes +  " +
                //" note_set_type.denomination_type_2 * p.cassette2_notes + note_set_type.denomination_type_3 * p.cassette3_notes  " +
                //" + note_set_type.denomination_type_4 * p.cassette4_notes + note_set_type.denomination_type_5 * p.cassette5_notes + " +
                //" note_set_type.denomination_type_6 * p.cassette6_notes + note_set_type.denomination_type_7 * p.cassette7_notes total ," +
                //" (p.purge_cassette1_notes+p.purge_cassette2_notes+p.purge_cassette3_notes+p.purge_cassette4_notes+p.purge_cassette5_notes+" +
                //" p.purge_cassette6_notes+p.purge_cassette7_notes) purged_counts," +
                //" (p.purge_cassette1_notes*note_set_type.denomination_type_1+p.purge_cassette2_notes*note_set_type.denomination_type_2+" +
                //" p.purge_cassette3_notes*note_set_type.denomination_type_3+p.purge_cassette4_notes*note_set_type.denomination_type_4 " +
                //" +p.purge_cassette5_notes*note_set_type.denomination_type_5+p.purge_cassette6_notes*note_set_type.denomination_type_6 " +
                //" +p.purge_cassette7_notes*note_set_type.denomination_type_7) purged_amount,( select max(rep_datetime) from replenishment where atm_id = outerATM.atm_id )last_replenished_at, " +
                //" ( select min(cash_order_datetime) from cash_order_monitoring where  atm_id = outerATM.atm_id and cash_order_datetime>=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + "',103))next_replenished_at,  " +
                //" ( select min(current_order_suggested_amount) from cash_order_monitoring where  atm_id = outerATM.atm_id   and cash_order_datetime>=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + "',103))amount from atm outerATM , cash_position p ,note_set_type where outerATM.atm_id = p.atm_id and outerATM.is_active = 1 " +

                //" and outerATM.exclude_dff = 0 and outerATM.note_set_type_id = note_set_type.note_set_type_id " +
                //(Request.QueryString["n"] != null ? " and outerATM.note_set_type_id = " + Request.QueryString["n"] : "") +
                //" and last_trxn_at = (select max(last_trxn_at)from cash_position where cash_position.atm_id = outerATM.atm_id)  order by " + orderBy;



                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                this.GridView_CashPositions.DataSource = dt;
                this.GridView_CashPositions.DataBind();
                Literal_AddHeading.Text = "NCR Cash Positions Monitoring DashBoard - Total No.Of ATMs : " + dt.Rows.Count;
            }


//            try
            //            {
            //                metaRefresh.Content = (((AppSetting)Application[ApplicationVars.AppSettings.ToString()]).DashboardRefreshInterval * 60).ToString();
            //                Page.RegisterStartupScript("script", "<script>function init() {cd(\"" + (int.Parse(metaRefresh.Content) / 60) + "\");} init();</script>");

//                cmd = ConnectionFactory.GetNewCommand(false);
            //                cmd.CommandTimeout = 500;
            //                //                cmd.CommandText = @"select top 1000 atm.title, note_set_type.denomination_type_1, note_set_type.denomination_type_2, note_set_type.denomination_type_3,
            //                //                                    note_set_type.denomination_type_4, note_set_type.denomination_type_5, 
            //                //                                    note_set_type.denomination_type_6, note_set_type.denomination_type_7, 
            //                //                                    p.last_trxn_at, p.cassette1_notes, p.cassette2_notes,
            //                //                                    p.cassette3_notes, p.cassette4_notes, p.cassette5_notes, p.cassette6_notes,
            //                //                                    p.cassette7_notes, task.last_invoked ,
            //                //note_set_type.denomination_type_1 * p.cassette1_notes + 
            //                //note_set_type.denomination_type_2 * p.cassette2_notes + 
            //                //note_set_type.denomination_type_3 * p.cassette3_notes + 
            //                //note_set_type.denomination_type_4 * p.cassette4_notes + 
            //                //note_set_type.denomination_type_5 * p.cassette5_notes + 
            //                //note_set_type.denomination_type_6 * p.cassette6_notes + 
            //                //note_set_type.denomination_type_7 * p.cassette7_notes total 
            //                //                                    from atm inner join cash_position p on atm.ATM_id = p.atm_id inner join task on p.task_id = task.task_id
            //                //                                    inner join note_set_type on atm.note_set_type_id = note_set_type.note_set_type_id where atm.IS_ACTIVE = 1 order by atm.title";


//                cmd.CommandText = @"select top 1000 outerATM.atm_id,outerATM.title title, note_set_type.denomination_type_1, note_set_type.denomination_type_2, note_set_type.denomination_type_3,
            //                                    note_set_type.denomination_type_4, note_set_type.denomination_type_5, 
            //                                    note_set_type.denomination_type_6, note_set_type.denomination_type_7, 
            //                                    p.last_trxn_at, p.cassette1_notes, p.cassette2_notes,
            //                                    p.cassette3_notes, p.cassette4_notes, p.cassette5_notes, p.cassette6_notes,
            //                                    p.cassette7_notes, task.last_invoked ,
            //note_set_type.denomination_type_1 * p.cassette1_notes + 
            //note_set_type.denomination_type_2 * p.cassette2_notes + 
            //note_set_type.denomination_type_3 * p.cassette3_notes + 
            //note_set_type.denomination_type_4 * p.cassette4_notes + 
            //note_set_type.denomination_type_5 * p.cassette5_notes + 
            //note_set_type.denomination_type_6 * p.cassette6_notes + 
            //note_set_type.denomination_type_7 * p.cassette7_notes total ,
            //(p.purge_cassette1_notes+p.purge_cassette2_notes+p.purge_cassette3_notes+p.purge_cassette4_notes+p.purge_cassette5_notes+
            //p.purge_cassette6_notes+p.purge_cassette7_notes) purged_counts,
            //
            //(p.purge_cassette1_notes*note_set_type.denomination_type_1+p.purge_cassette2_notes*note_set_type.denomination_type_2+
            //p.purge_cassette3_notes*note_set_type.denomination_type_3+p.purge_cassette4_notes*note_set_type.denomination_type_4
            //+p.purge_cassette5_notes*note_set_type.denomination_type_5+p.purge_cassette6_notes*note_set_type.denomination_type_6
            //+p.purge_cassette7_notes*note_set_type.denomination_type_7) purged_amount,
            //( select max(rep_datetime) from replenishment where atm_id = outerATM.atm_id )last_replenished_at,
            //
            //( select min(cash_order_datetime) from cash_order_monitoring where  atm_id = outerATM.atm_id    
            //and cash_order_datetime>=convert(datetime,'"+DateTime.Today.ToString("dd/MM/yyyy")+"',103))next_replenished_at, " +

//" ( select min(current_order_suggested_amount) from cash_order_monitoring where  atm_id = outerATM.atm_id  " +
            //" and cash_order_datetime>=convert(datetime,'"+DateTime.Today.ToString("dd/MM/yyyy")+"',103))amount"+

//" from atm outerATM , cash_position p ,task,note_set_type " +
            //"where outerATM.atm_id = p.atm_id and outerATM.is_active = 1 and outerATM.exclude_dff = 0 " +
            //"and p.task_id = task.task_id " +
            //"and outerATM.note_set_type_id = note_set_type.note_set_type_id " +
            //"and last_trxn_at = (select max(last_trxn_at)" +
            //"from cash_position " +
            //"where cash_position.atm_id = outerATM.atm_id)  order by " + orderBy;

//                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            //                DataTable dt = new DataTable();
            //                adapter.Fill(dt);

//                GridView_CashPositions.DataSource = dt;
            //                GridView_CashPositions.DataBind();
            //            }
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
        protected void GridView_CashPositions_Sorting(object sender, GridViewSortEventArgs e)
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

        protected void GridView_CashPositions_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Image img = (Image)e.Row.FindControl("img");
                string atm_id = this.GridView_CashPositions.DataKeys[e.Row.DataItemIndex].Values["atm_id"].ToString();
                Atm atm = Atm.LoadAtmByPk(int.Parse(atm_id));
                //if (!atm.IsHealthy)
                //    e.Row.Cells[1].ForeColor = System.Drawing.Color.Red;
                if (atm.MinOperatingBalance == null)
                {
                    img.Attributes.Add("src", "../../images/help.jpg");
                    img.Attributes.Add("title", "header=[Min Operating Balance Details] body=[Minimum operating balance is not defined]");
                }
                else if (decimal.Parse(e.Row.Cells[18].Text) <= atm.MinOperatingBalance)
                {
                    if (decimal.Parse(e.Row.Cells[18].Text) <= 50000)
                    {
                        img.Attributes.Add("src", "../../images/attention_now.jpg");

                        img.Attributes.Add("title", "header=[Min Operating Balance Details] body=[Current balance is less than equal to 50K.Minimum Operating balance (" + atm.MinOperatingBalance + ")]");
                    }
                    else
                    {
                        img.Attributes.Add("src", "../../images/attention_soon.jpg");

                        img.Attributes.Add("title", "header=[Min Operating Balance Details] body=[Current balance is less then min operating balance.Minimum operating balance (" + atm.MinOperatingBalance + ")]");
                    }
                }
                else
                {
                    img.Attributes.Add("src", "../../images/healthy.jpg");

                    img.Attributes.Add("title", "header=[Min Operating Balance Details] body=[ATM is in healthy state.Minimum operating balance (" + atm.MinOperatingBalance + ")]");
                }

                if (Request.QueryString["f"] == "1")
                {
                    SqlCommand cmd =  ConnectionFactory.GetNewCommand(false);
                    cmd.CommandText = "select * from atm_alert where alert_type_id in (46,47,48,49) and resolve_at is null and atm_id = "+atm.ATMId;
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    foreach (DataRow dr in dt.Rows)
                    {
                        if (int.Parse(dr["alert_type_id"].ToString()) == 46)
                        {
                            e.Row.Cells[5].ForeColor = System.Drawing.Color.Yellow;
                           
                        }
                        else if (int.Parse(dr["alert_type_id"].ToString()) == 47)
                        {
                            e.Row.Cells[7].ForeColor = System.Drawing.Color.Yellow;
                        }
                        else if (int.Parse(dr["alert_type_id"].ToString()) == 48)
                        {
                            e.Row.Cells[9].ForeColor = System.Drawing.Color.Yellow;
                        }
                        else if (int.Parse(dr["alert_type_id"].ToString()) == 49)
                        {
                            e.Row.Cells[11].ForeColor = System.Drawing.Color.Yellow;
                        }

                    }

                }
            }

        }
        public static string parseValueIntoCurrency(decimal number)
        {
            // set currency format
            string curCulture = Thread.CurrentThread.CurrentCulture.ToString();
            System.Globalization.NumberFormatInfo currencyFormat = new
                System.Globalization.CultureInfo(curCulture).NumberFormat;

            currencyFormat.CurrencyNegativePattern = 1;
            currencyFormat.CurrencySymbol = "";
            return number.ToString("c", currencyFormat);
        }

        private void EnableGridRespectiveColumns()
        {
            //Enable when Last Dispensed 24 Hour column need to display(ADIB)
            bool isShowDispensed24Hour = ConfigurationManager.AppSettings["IsShowDispensed24Hour"] != null ? bool.Parse(ConfigurationManager.AppSettings["IsShowDispensed24Hour"]) : false;
            int noOfCassettes = ValidateNoOfCassettes(ConfigurationManager.AppSettings["NoOfCassettes"]);
            int columnIndexDenomination = -1;
            //int columnIndexDispensed = -1;
            int columnIndexRemaining = -1;

            columnIndexDenomination = GetColumnIndexByName(this.GridView_CashPositions, "Type 5");
            //columnIndexDispensed = GetColumnIndexByName(this.GridView_CountsCleared, "Dispensed Type 5");
            columnIndexRemaining = columnIndexDenomination + 1;// GetColumnIndexByName(this.GridView_CashPositions, "Remaining");

            //Enable when Last Dispensed 24 Hour column need to display(ADIB)
            if (isShowDispensed24Hour)
            {
                int columnIndexDispensed24Hour = GetColumnIndexByName(this.GridView_CashPositions, "Total Dispensed in Last 24 hour");
                if(columnIndexDispensed24Hour >=0 )
                    this.GridView_CashPositions.Columns[columnIndexDispensed24Hour].Visible = true;
            }

            for (int i = 5; i <= noOfCassettes; i++)
            {
                if (columnIndexDenomination >= 0)
                {
                    this.GridView_CashPositions.Columns[columnIndexDenomination].Visible = true;
                    columnIndexDenomination+=2;
                }

                //if (columnIndexDispensed >= 0)
                //{
                //    this.GridView_CountsCleared.Columns[columnIndexDispensed].Visible = true;
                //    columnIndexDispensed++;
                //}
                if (columnIndexRemaining >= 0)
                {
                    this.GridView_CashPositions.Columns[columnIndexRemaining].Visible = true;
                    columnIndexRemaining+=2;
                }
            }
        }
    }
}