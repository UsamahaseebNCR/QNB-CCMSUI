using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Avanza.iSuite.UI;
using System.Data.SqlClient;

using System.Data;
using System.Reflection;
using System.Diagnostics;
using System.Text;

namespace CCMSUI.CCMS
{
    public partial class EJBNATransactions : BasePage
    {
        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        }
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!Page.IsPostBack)
            {
                if (!CheckRight((int)Permissions.ViewEJBNATransactions, true))
                    Response.Redirect("~/admin/AccessDenied.aspx");

                ViewState["SortExpression"] = "trxn_datetime";
                ViewState["sortDirection"] = " desc";
                ViewState["orderby"] = ViewState["SortExpression"] + " " + ViewState["sortDirection"];

                TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
            }
        }


        protected void GridView_Trxn_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView_Trxn.PageIndex = e.NewPageIndex;
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
            //BindGridWithCache();
        }

        protected void GridView_Trxn_Sorting(object sender, GridViewSortEventArgs e)
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
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
            //BindGridWithCache();
        }



        void BindGrid(string orderBy)
        {
            LogableTask task = LogableTask.NewTask("BindGrid");
            try
            {
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;

                cmd.CommandText = @"select notes_deposited_type1,notes_deposited_type2,notes_deposited_type3,notes_deposited_type4,transaction_start_time,transaction_end_time,card_taken_time,ej_parsed_bna_transactions_id,title,trxn_datetime,tsn,pan,amount,case when status =0 then 'Successful' 
			when status=1 then 'Failed'
			when status =2 then 'Suspicious' end status,account_no,host_tsn
                                    from ej_parsed_bna_transactions inner join atm 
                                    on ej_parsed_bna_transactions.atm_id = atm.atm_id  
                                    where atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") " + GetFilter()
                + " order by " + ViewState["orderby"];

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dtTrxn = new DataTable();
                adapter.Fill(dtTrxn);
                Literal_count.Text = dtTrxn.Rows.Count + " records.";
                this.GridView_Trxn.DataSource = dtTrxn;
                this.GridView_Trxn.DataBind();
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
                filter += " and trxn_Datetime >= convert(datetime,'" + TextBox_FromDate.Text + "',103)";

            if (TextBox_ToDate.Text.Length > 0)
                filter += " and trxn_Datetime<= convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103)";
            
            if (TextBox_TSN.Text.Length > 0)
                filter += " and tsn = " + int.Parse(TextBox_TSN.Text);

            if (TextBox_PAN.Text.Length > 0)
                filter += " and pan like '%" + TextBox_PAN.Text + "%'";

            if (TextBox_FromAmount.Text.Length > 0)
            filter += " and amount >= " + int.Parse(TextBox_FromAmount.Text);

            if (TextBox_ToAmount.Text.Length > 0)
                filter += " and amount <= " + int.Parse(TextBox_ToAmount.Text);

            if (TextBox_HostTsn.Text.Length > 0)
                filter += " and host_tsn = " + TextBox_HostTsn.Text;

            StringBuilder builder = new StringBuilder();
            foreach (ListItem item in CheckBoxList_Status.Items)
            {
                if (item.Selected)
                    builder.Append(item.Value + ",");
            }
            if (builder.Length > 0)
            {
                builder.Append("-1");
                //  if (CheckBoxList_Status.SelectedIndex > 0)
                filter += " and status in (" + builder.ToString() + ")";
            }

            return filter;
        }

        protected void button_search_Click(object sender, EventArgs e)
        {
            try
            {
                if (TextBox_PAN.Text.Length > 0)
                    if (TextBox_PAN.Text.Length < 4)
                        throw new Exception("Please enter at least 4 characters for PAN search.");
                BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
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


    }
}