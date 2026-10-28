using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Avanza.iSuite.UI;
using System.Data.SqlClient;
using System.Data;

using Avanza.CCMS.DAL;
using System.Reflection;
using System.Diagnostics;
using System.Drawing;

namespace CCMSUI.CCMS
{
    public partial class ViewTransactionsHourlyAnalysis : BasePage
    {

        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (!CheckRight((int)Permissions.ViewTransactionsHourlyAnalysis, true))
                    Response.Redirect("~/admin/AccessDenied.aspx");
                ViewState["SortExpressionThreshold16"] = "title";
                ViewState["sortDirectionThreshold16"] = " asc";                    
             }
        }


        protected void GridView_Trxn_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView_Threshold.PageIndex = e.NewPageIndex;
            GetThresholdOfHour((string)ViewState["SortExpressionThreshold16"] + (string)ViewState["sortDirectionThreshold16"]);
            //BindGridWithCache();
        }

        protected void GridView_Threshold16_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (ViewState["SortExpressionThreshold16"].ToString().IndexOf(e.SortExpression) >= 0)
            {
                if (ViewState["sortDirectionThreshold16"].ToString() == " asc")
                    ViewState["sortDirectionThreshold16"] = " desc";
                else
                    ViewState["sortDirectionThreshold16"] = " asc";
            }
            else
            {
                ViewState["SortExpressionThreshold16"] = e.SortExpression;
                ViewState["sortDirectionThreshold16"] = " asc";
            }
            ViewState["orderby"] = ViewState["SortExpressionThreshold16"] + " " + ViewState["sortDirectionThreshold16"];
            GetThresholdOfHour((string)ViewState["orderby"]);
        }


        

        protected void Button_Reset_Click(object sender, EventArgs e)
        {
            TextBox_TrxnThreshold.Text = "";
        }
       
        
        protected void button_search_Click(object sender, EventArgs e)
        {
            try
            {
                GetThresholdOfHour((string)ViewState["SortExpressionThreshold16"] + (string)ViewState["sortDirectionThreshold16"]);   
            }
            catch (Exception ex)
            {
                Literal_error.Text = ex.Message;
            }

        }
        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            button_search_Click(this, null);
        }

        protected void GridView_Threshold_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (!Convert.ToBoolean(((DataRowView)e.Row.DataItem).Row["is_healthy"].ToString()))
                    e.Row.Cells[0].ForeColor = Color.Red;
            }
        }
        private void GetThresholdOfHour(string orderby)
        {
            LogableTask task = LogableTask.NewTask("GetThresholdOfHour");
            SqlConnection conn = null;
            SqlDataReader reader = null;
            DataTable dt = new DataTable();
            dt.Columns.Add("title");
            dt.Columns.Add("generated_at", typeof(DateTime));
            dt.Columns.Add("amount");
            dt.Columns.Add("last_heart_beat_at");
            dt.Columns.Add("is_healthy", typeof(bool));
            try
            {
                conn = ConnectionFactory.GetNewConnection();
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandTimeout = 180;
                cmd.CommandText = @"
                                        select outerATM.atm_id, max(trxn_datetime) trxn_datetime,
(select max(heart_beat_received_at) from heart_beat where atm_id = outerATM.atm_id) last_heart_beat_at
                                        from atm outerATM left join parsed_transaction
                                        on parsed_transaction.atm_id = outerATM.atm_id 
                                        where is_active = 1 and allowed_inactivity_period is not null
                                        group by outerATM.atm_id                                        ";
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Atm atm = Atm.LoadAtmByPk(reader.GetInt32(0));
                    if (reader[1] != DBNull.Value)
                    {
                        DateTime maxDateTime = reader.GetDateTime(1);

                        if (maxDateTime.AddHours(double.Parse(TextBox_TrxnThreshold.Text)) < DateTime.Now)
                        {
                            ParsedTransaction parsedTransaction = ParsedTransaction.LoadParsedTransaction(
                               "trxn_datetime=convert(datetime,'" + maxDateTime.ToString("dd/MM/yyyy HH:mm:ss") + "',103) and atm_id = " + atm.ATMId);
                            DataRow dr = dt.NewRow();
                            dr[0] = atm.Title;
                            dr[1] = maxDateTime;
                            if (parsedTransaction != null)
                                dr[2] = parsedTransaction.Amount;
                            if (reader[2] != DBNull.Value)
                                dr[3] = reader.GetDateTime(2).ToString("dd/MM/yyyy HH:mm:ss");
                            else
                                dr[3] = "-";

                            dr[4] = atm.IsHealthy;
                            dt.Rows.Add(dr);
                        }
                    }
                }
                reader.Close();
                dt.DefaultView.Sort = orderby;
                Literal_count.Text = dt.Rows.Count + " records.";
                GridView_Threshold.DataSource = dt;
                GridView_Threshold.DataBind();
               
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
               Literal_error.Text = "<SPAN STYLE='COLOR: red'>There was an error while processing request. Details : " + ex.Message + "</SPAN>";
            }
            finally
            {
                if (conn != null)
                    if (conn.State == ConnectionState.Open)
                        conn.Close();
                task.EndTask();

            }
        }
    }
}