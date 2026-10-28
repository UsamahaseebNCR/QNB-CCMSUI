using Avanza.iSuite.DAL;
using Avanza.iSuite.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.EnterpriseServices.CompensatingResourceManager;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static ClosedXML.Excel.XLPredefinedFormat;

namespace CCMSUI.CCMS
{
    public partial class ViewUploadedData : BasePage
    {
        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {

                if (!CheckRight((int)Permissions.ViewUploadedData))
                    Response.Redirect("../admin/aspx/AccessDenied.aspx");

                ViewState["sortDirection"] = " desc";
                ViewState["SortExpression"] = "transaction_date";
                //BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
                TextBox_FromDate.Text  = TextBox_ToDate.Text = System.DateTime.Today.ToString("dd/MM/yyyy");

            }

        }
        string Filter()
        {
            string filter = "";
            if (TextBox_ATMID.Text.Length > 0)
                filter += " and title like '%" + TextBox_ATMID.Text + "%'";
            if (TextBox_PAN.Text.Length > 0)
                filter += " and card_number like '%" + TextBox_PAN.Text + "%'";
            if (TextBox_AcctNo.Text.Length > 0)
                filter += " and customer_account_no = " + TextBox_AcctNo.Text;
            if (TextBox_FromDate.Text.Length > 0)
                filter += " and CONVERT(datetime, transaction_date, 103)  >= CONVERT(datetime,'" + TextBox_FromDate.Text + "',103)";
            if (TextBox_ToDate.Text.Length > 0)
                filter += " and CONVERT(datetime, transaction_date, 103)  <= CONVERT(datetime,'" + TextBox_ToDate.Text + "',103)";
            if (TextBox_Amount.Text.Length > 0)
                filter += " and transaction_amount <= '" + TextBox_Amount.Text + "'";
            if (TextBox_TSN.Text.Length > 0)
                filter += " and transaction_sequence = '" + TextBox_TSN.Text + "'";
            if (TextBox_Response.Text.Length > 0)
                filter += " and transaction_response <= '" + TextBox_Response.Text + "'";

            filter += " and atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ")";

            return filter;
        }

        private void BindGrid(string orderby)
        {
            LogableTask task = LogableTask.NewTask("BindGrid");
            try
            {

                int batchID = 0;
                if (Request.QueryString["id"] != null)
                    batchID = int.Parse(Request.QueryString["id"]);

                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);

                cmd.CommandText = @"select title,card_number,customer_account_no,transaction_date,transaction_time,transaction_amount,transaction_currency,transaction_sequence,
                                    transaction_type, transaction_response,transaction_settlement_date,card_network,card_issuer from " + DropDownList_Source.SelectedItem.Value +
                                    " inner join atm on atm.title = "+ DropDownList_Source.SelectedItem.Value+ ".atm_id " +
                                   (batchID > 0 ? " where reconciliation_batch_id=" + batchID : " where 1=1 ") + Filter() + " order by " + orderby;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dtGrid = new DataTable();
                adapter.Fill(dtGrid);
                cmd.Connection.Close();
                GridView_Trxn.DataSource = dtGrid;
                GridView_Trxn.DataBind();
                Literal_count.Text = dtGrid.Rows.Count + " records.";
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);

                GridView_Trxn.DataSource = null;
                GridView_Trxn.DataBind();
                Literal_count.Text = "0 records.";
            }
            finally
            { task.EndTask(); }
        }

        protected void GridView_Trxn_Sorting(object sender, GridViewSortEventArgs e)
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




        protected void GridView_Trxn_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.GridView_Trxn.PageIndex = e.NewPageIndex;
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }


        protected void Button_search_Click(object sender, EventArgs e)
        {
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }

        protected void Button_reset_Click(object sender, EventArgs e)
        {
            TextBox_ATMID.Text = "";
            TextBox_AcctNo.Text = "";
            DropDownList_Source.SelectedIndex = 0;
            TextBox_Amount.Text = "";
            TextBox_FromDate.Text = System.DateTime.Today.ToString("dd/MM/yyyy");
            TextBox_ToDate.Text = System.DateTime.Today.ToString("dd/MM/yyyy");
            TextBox_TSN.Text = "";
            TextBox_Response.Text = "";
            TextBox_PAN.Text = "";
        }

        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            Button_search_Click(this, null);
        }


    }
}