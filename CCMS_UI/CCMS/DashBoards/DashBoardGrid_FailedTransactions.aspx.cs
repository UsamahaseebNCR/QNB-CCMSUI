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
using Avanza.iSuite.DAL;
using System.Reflection;
using System.Diagnostics;
using System.Text;
using Avanza.iSuite.UI;
using Avanza.CCMS.DAL;
using ceTe.DynamicPDF;

namespace CCMSUI.CCMS.DashBoards
{
    public partial class DashBoardGrid_EJCashPositions : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!CheckRight((int)Permissions.ViewEJCashPositionsDashBoard, true))
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
                this.metaRefresh.Content = (((AppSetting)base.Application[ApplicationVars.AppSettings.ToString()]).DashboardRefreshInterval * 60).ToString();
                this.Page.RegisterStartupScript("script", "<script>function init() {cd(\"" + (int.Parse(this.metaRefresh.Content) / 60) + "\");} init();</script>");
                cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;
                cmd.CommandText = "select top 1000 outerATM.atm_id,outerATM.title title, note_set_type.denomination_type_1, "+
                    " note_set_type.denomination_type_2, note_set_type.denomination_type_3,note_set_type.denomination_type_4 " + 
                " , note_set_type.denomination_type_5, note_set_type.denomination_type_6, note_set_type.denomination_type_7, " + 
                " (select max(trxn_datetime) from ej_parsed_transactions where atm_id = outerATM.atm_id and amount>0 )last_trxn_at, " + 
                " p.cassette1_notes, p.cassette2_notes, " + 
                 " p.cassette3_notes, p.cassette4_notes, p.cassette5_notes, p.cassette6_notes, " + 
                " p.cassette7_notes, note_set_type.denomination_type_1 * p.cassette1_notes +  " +
                " note_set_type.denomination_type_2 * p.cassette2_notes + note_set_type.denomination_type_3 * p.cassette3_notes  " +
                " + note_set_type.denomination_type_4 * p.cassette4_notes + note_set_type.denomination_type_5 * p.cassette5_notes + "+
                " note_set_type.denomination_type_6 * p.cassette6_notes + note_set_type.denomination_type_7 * p.cassette7_notes total ,"+
                " (p.purge_cassette1_notes+p.purge_cassette2_notes+p.purge_cassette3_notes+p.purge_cassette4_notes+p.purge_cassette5_notes+"+
                " p.purge_cassette6_notes+p.purge_cassette7_notes) purged_counts,"+
                " (p.purge_cassette1_notes*note_set_type.denomination_type_1+p.purge_cassette2_notes*note_set_type.denomination_type_2+"+
                " p.purge_cassette3_notes*note_set_type.denomination_type_3+p.purge_cassette4_notes*note_set_type.denomination_type_4 "+
                " +p.purge_cassette5_notes*note_set_type.denomination_type_5+p.purge_cassette6_notes*note_set_type.denomination_type_6 " + 
                " +p.purge_cassette7_notes*note_set_type.denomination_type_7) purged_amount,( select max(rep_datetime) from ej_parsed_replenishments where atm_id = outerATM.atm_id )last_replenished_at, " + 
                " ( select min(cash_order_datetime) from cash_order_monitoring where  atm_id = outerATM.atm_id and cash_order_datetime>=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + "',103))next_replenished_at,  "+
                " ( select min(current_order_suggested_amount) from cash_order_monitoring where  atm_id = outerATM.atm_id   and cash_order_datetime>=convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + "',103))amount from atm outerATM , ej_cash_position p ,note_set_type where outerATM.atm_id = p.atm_id and outerATM.is_active = 1 "+
                " and outerATM.exclude_dff = 0 and outerATM.note_set_type_id = note_set_type.note_set_type_id "+
                " and last_trxn_at = (select max(last_trxn_at)from ej_cash_position where ej_cash_position.atm_id = outerATM.atm_id)  order by " + orderBy;
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
                if (!atm.IsHealthy)
                    e.Row.Cells[1].ForeColor = System.Drawing.Color.Red;
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


            }

        }
    }

}