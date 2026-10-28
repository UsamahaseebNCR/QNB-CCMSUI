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
using System.Data.SqlClient;
using System.Reflection;
using System.Diagnostics;

using System.Text;
using Avanza.CCMS.DAL;
using System.Threading;

namespace CCMSUI.CCMS
{
    public partial class EJBalanceInvestigation : BasePage
    {
        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (!CheckRight((int)Permissions.EJBalanceInvestigation))
                    Response.Redirect("~/admin/AccessDenied.aspx");

                ViewState["sortDirection"] = " asc";
                ViewState["SortExpression"] = "trxn_datetime";

                TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");

                refresh_ServerClick(this, EventArgs.Empty);
                //BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());

                //if ((bool)Session[SessionVars.isRegionSelected.ToString()] || Session[SessionVars.SelectedATMs.ToString()].ToString().IndexOf(',') > -1 ||
                //   Session[SessionVars.SelectedATMs.ToString()].ToString().Length == 0)
                //    Literal_error.Text =  "Please, select an ATM";
                //else
                //{
                //    if ((string)Session[SessionVars.SelectedATMs.ToString()] == "-1")
                //    {
                //        Literal_error.Text = "<li>ATM not selected.</li>";
                //        //Button_Download.Enabled = false;
                //    }
                //    else
                //       TextBox_ip.Text =  Atm.LoadAtm("atm_id =" + Session[SessionVars.SelectedATMs.ToString()].ToString()).IP;
                //}


            }
        }
        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            if (!((bool)Session[SessionVars.isRegionSelected.ToString()] || Session[SessionVars.SelectedATMs.ToString()].ToString().IndexOf(',') > -1 ||
                     Session[SessionVars.SelectedATMs.ToString()].ToString().Length == 0))
            {
                TextBox_ip.Text = Atm.LoadAtm("atm_id =" + Session[SessionVars.SelectedATMs.ToString()].ToString()).IP;
                Button_search_Click(this, null);
            }
            else
                TextBox_ip.Text = "";
            Button_search_Click(null, null);


        }

        protected void GridView_OutOfBalanceInvestigation_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.GridView_OutOfBalanceInvestigation.PageIndex = e.NewPageIndex;
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }

        protected void GridView_OutOfBalanceInvestigation_Sorting(object sender, GridViewSortEventArgs e)
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
        //protected void GridView_OutOfBalanceInvestigation_RowDataBound(object sender, GridViewRowEventArgs e)
        //{
        //    SqlCommand cmd = null;
        //    string body = null;
        //    if (e.Row.RowType == DataControlRowType.DataRow)
        //    {
                
        //        int cashPurged1 = Convert.ToInt32(((DataRowView)e.Row.DataItem).Row["cash_purged1"].ToString());
        //        int cashPurged2 = Convert.ToInt32(((DataRowView)e.Row.DataItem).Row["cash_purged2"].ToString());
        //        int cashPurged3 = Convert.ToInt32(((DataRowView)e.Row.DataItem).Row["cash_purged3"].ToString());
        //        int cashPurged4 = Convert.ToInt32(((DataRowView)e.Row.DataItem).Row["cash_purged4"].ToString());
        //        int cashPurged5 = Convert.ToInt32(((DataRowView)e.Row.DataItem).Row["cash_purged5"].ToString());
        //        int cashPurged6 = Convert.ToInt32(((DataRowView)e.Row.DataItem).Row["cash_purged6"].ToString());
        //        int cashPurged7 = Convert.ToInt32(((DataRowView)e.Row.DataItem).Row["cash_purged7"].ToString());
        //        //int atm_id = Convert.ToInt32(((DataRowView)e.Row.DataItem).Row["atm_id"].ToString());
        //        //NoteSetType noteSetType = NoteSetType.LoadNoteSetTypeByPk(Atm.LoadAtmByPk(atm_id).NoteSetTypeId);

        //        //totalPurgedAmount += (decimal)(noteSetType.DenominationType1 * cashPurged1 +
        //        //    noteSetType.DenominationType2 * cashPurged2 +
        //        //    noteSetType.DenominationType3 * cashPurged3 +
        //        //    noteSetType.DenominationType4 * cashPurged4 +
        //        //    noteSetType.DenominationType5 * cashPurged5 +
        //        //    noteSetType.DenominationType6 * cashPurged6 +
        //        //    noteSetType.DenominationType7 * cashPurged7);
        //        //totalDispensedAmount += Convert.ToInt32(((DataRowView)e.Row.DataItem).Row["amount"].ToString());


        //        if (cashPurged1 > 0 || cashPurged2 > 0 || cashPurged3 > 0 || cashPurged4 > 0 || cashPurged5 > 0 || cashPurged6 > 0 || cashPurged7 > 0)
        //        {
        //            e.Row.BackColor = Color.LightGreen;
        //        }
        //        try
        //        {
        //            Atm atm = Atm.LoadAtm("title='" + ((DataRowView)e.Row.DataItem).Row["title"].ToString() + "'");
        //            if (!ht.Contains(atm.Title))
        //            {

        //                cmd = ConnectionFactory.GetNewCommand(true);
        //                cmd.CommandText = "select top 10 status from task where atm_id = " + atm.ATMId + " order by creation_time desc";
        //                SqlDataReader reader = cmd.ExecuteReader();
        //                while (reader.Read())
        //                {
        //                    body += reader.GetString(0) + "<br/>";
        //                }
        //                reader.Close();
        //                ht.Add(atm.Title, body);
        //            }
        //            else
        //            {
        //                body += (string)ht[atm.Title];
        //            }
        //            ((System.Web.UI.WebControls.Image)e.Row.FindControl("image_history")).Attributes.Add("title", "header=[ATM Top 10 File Download Status] body=[" + body + ".]");
        //        }
        //        catch (Exception ex)
        //        {
        //        }
        //        finally
        //        {
        //            if (cmd != null)
        //                cmd.Connection.Close();
        //        }

        //    }
        //    else if (e.Row.RowType == DataControlRowType.Footer)
        //    {
        //        Literal_count.Text += " &nbsp;,Total Purged Amount = " + totalPurgedAmount.ToString() + ",Total Dispensed Amount=" + totalDispensedAmount.ToString();
        //    }
        //}

        string Filter()
        {
            StringBuilder builder = new StringBuilder();
            if (TextBox_ip.Text.Length > 0)
                builder.Append(" and Atm1.ip='" + TextBox_ip.Text+"'");
            else
            {
                if (Session[SessionVars.SelectedATMs.ToString()] != null)
                    builder.Append(" and atm1.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ")");
            }

            if (TextBox_FromDate.Text.Length > 0)
                builder.Append(" and trxn_datetime >= Convert(datetime,'" + TextBox_FromDate.Text + @"',103) ");
            if (TextBox_ToDate.Text.Length > 0)
                builder.Append(" and trxn_datetime <= Convert(datetime,'" + TextBox_ToDate.Text + @" 23:59:59',103) ");

            return builder.ToString();
        }

        private void BindGrid(string orderby)
        {
            LogableTask task = LogableTask.NewTask("BindGrid");
            SqlCommand cmd = null;
            DataTable dt = null;
            try
            {
                cmd = ConnectionFactory.GetNewCommand(true);

                //                cmd.CommandText = @"select cash_added1,cash_added2,cash_added3,cash_added4,cash_added5,cash_added6,
                //                                    cash_added7,denomination_type_1,denomination_type_2,
                //                                    denomination_type_3,denomination_type_4,denomination_type_5,
                //                                    denomination_type_6,denomination_type_7 
                //                                    from replenishment,atm Atm1,note_set_type
                //                                    where rep_datetime >= Convert(datetime,'" + TextBox_Date.Text + @"',103)
                //                                    and rep_datetime<isnull(Convert(datetime,(
                //                                    select min(rep_datetime) 
                //                                    from replenishment 
                //                                    where rep_datetime>=Convert(datetime,'" + TextBox_Date.Text + @"',103)+1 
                //                                    and Atm1.atm_id=replenishment.atm_id),103),Convert(datetime,'" + TextBox_Date.Text + @"',103)+1)
                //                                    and Atm1.ip='" + TextBox_ip.Text + "'" +
                //                                  " and Atm1.note_set_type_id = note_set_type.note_set_type_id order by " + orderby;
                //                adapter.SelectCommand = cmd;
                //                DataTable dt = new DataTable();
                //                adapter.Fill(dt);

                //                foreach (DataRow row in dt.Rows)
                //                {
                //                    totalReplenishment += (int)row[0] * (int)row[7] + (int)row[1] * (int)row[8] +
                //                        (int)row[2] * (int)row[9] + (int)row[3] * (int)row[10] + (int)row[4] * (int)row[11]
                //                        + (int)row[5] * (int)row[12] + (int)row[6] * (int)row[13];
                //                }

                //                cmd.CommandText = @"select replenishment_amount from summary,atm 
                //                                    where summary.atm_id = atm.atm_id and
                //                                    trxn_datetime >= convert(datetime,'" + TextBox_Date.Text+"',103) and trxn_datetime <= convert(datetime,'"+TextBox_Date.Text+" 23:59:59',103) and atm.ip='"+TextBox_ip.Text+"'";
                //                object res =  cmd.ExecuteScalar();
                //                if (res != DBNull.Value
                //                    && res != null)
                //                    totalReplenishment = int.Parse(res.ToString());
                //                else
                //                    totalReplenishment = 0;
                //                Literal_summary.Text = "ATM Total Amount on " + TextBox_Date.Text + " was " + totalReplenishment;
                //                if (totalReplenishment > 0)
                //      {


                StringBuilder builder = new StringBuilder();

                cmd.CommandText = @"select title,trxn_datetime,closing_balance,withdrawals, pre_withdrawals,replenishment_amount,return_amount
                    from ej_summary,   atm Atm1
                    where ej_summary.atm_id = Atm1.atm_id " + Filter() + " order by " + orderby;




                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = cmd;

                dt = new DataTable();
                adapter.Fill(dt);


                //}
                //else 
                //    dt = new DataTable();
                Literal_summary.Text = dt.Rows.Count + " records.";
                this.GridView_OutOfBalanceInvestigation.DataSource = dt;
                this.GridView_OutOfBalanceInvestigation.DataBind();

            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_error.Text = "<li>" + ex.Message + "</li>";
            }
            finally
            {
                task.EndTask();
                if (cmd != null)
                    if (cmd.Connection != null)
                        cmd.Connection.Close();
            }
        }

        protected void Button_search_Click(object sender, EventArgs e)
        {
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());

        }
        protected void Button_reset_Click(object sender, EventArgs e)
        {
            TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            TextBox_ip.Text = "";
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
            /*    DataTable dt = new DataTable();
                this.GridView_OutOfBalanceInvestigation.DataSource = dt;
                this.GridView_OutOfBalanceInvestigation.DataBind();*/

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


    }
}