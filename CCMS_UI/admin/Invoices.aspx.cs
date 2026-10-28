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
    public partial class Invoices : BasePage
    {

        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        }
        private void PopulateCIT()
        {
            DropDownList_CIT.Items.Clear();
            DropDownList_CIT.Items.Add("Select");
            RegionCit.RegionCitReader reader = RegionCit.ExecuteReader("1=1");
            while (reader.Read())
            {
                CcmsCit ccmsCIT = CcmsCit.LoadCcmsCitByPk(reader.CurrentRegionCit.CitId);
                if (ccmsCIT != null)
                    DropDownList_CIT.Items.Add(new ListItem(ccmsCIT.Name, ccmsCIT.Id.ToString()));
            }
            reader.Close();
        }



        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {

                if (!CheckRight((int)Permissions.ModifyInvoice))
                    GridView_Invoice.Columns[4].Visible = false;

                if (!CheckRight((int)Permissions.DeleteInvoice))
                    GridView_Invoice.Columns[5].Visible = false;

                if (!CheckRight((int)Permissions.CreateInvoice))
                    addNewInvoiceLink.Visible = false;



                ViewState["sortDirection"] = " desc";
                ViewState["SortExpression"] = " ccms_invoice.created_on";

                DropDownList_Status.Items.Add(new ListItem("Any", "Any"));
                CcmsInvoiceStatus.CcmsInvoiceStatusReader CcmsInvoiceReader = CcmsInvoiceStatus.ExecuteReader("1=1");
                while (CcmsInvoiceReader.Read())
                    DropDownList_Status.Items.Add(new ListItem(CcmsInvoiceReader.CurrentCcmsInvoiceStatus.Name, CcmsInvoiceReader.CurrentCcmsInvoiceStatus.Id.ToString()));
                CcmsInvoiceReader.Close();

                PopulateCIT();

                TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
            }

        }
        string Filter()
        {
            string filter = "";

            if (DropDownList_Status.SelectedIndex > 0)
                filter += " and status = '" + DropDownList_Status.SelectedValue + "'";
            if (DropDownList_CIT.SelectedIndex > 0)
                filter += " and cit_id = '" + DropDownList_CIT.SelectedValue + "'";
            if (TextBox_FromDate.Text.Length > 0)
                filter += " and ccms_invoice.created_on >= convert(datetime,'" + TextBox_FromDate.Text + "',103)";
            if (TextBox_ToDate.Text.Length > 0)
                filter += " and ccms_invoice.created_on<= convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103)";

            return filter;
        }

        private void BindGrid(string orderby)
        {
            LogableTask task = LogableTask.NewTask("BindGrid");
            try
            {
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandText = @"SELECT ccms_invoice.code,name,amount,ccms_invoice.id,ccms_invoice.created_on,status,invoice_date from ccms_invoice inner join ccms_cit on ccms_invoice.cit_id = ccms_cit.id where ccms_invoice.is_deleted=0 and is_active=1 " + Filter() + " order by " + orderby;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dtInvoice = new DataTable();
                adapter.Fill(dtInvoice);
                cmd.Connection.Close();
                GridView_Invoice.DataSource = dtInvoice;
                GridView_Invoice.DataBind();
                Literal_count.Text = dtInvoice.Rows.Count + " records.";
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_error.Text = ex.Message;
            }
            finally
            { task.EndTask(); }
        }

        protected void GridView_Invoice_Sorting(object sender, GridViewSortEventArgs e)
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


        protected void GridView_Invoice_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            LogableTask task = null;
            if (e.CommandName == "deleteInvoice")
            {
                SqlCommand cmd = null;
                SqlTransaction trx = null;
                try
                {
                    task = LogableTask.NewTask("delete invoice");
                    cmd = ConnectionFactory.GetNewCommand(true);
                    trx = cmd.Connection.BeginTransaction();
                    cmd.Transaction = trx;
                    int invoiceID = int.Parse(e.CommandArgument.ToString());
                    CcmsInvoice ccmsInvoice = CcmsInvoice.LoadCcmsInvoiceByPk(invoiceID);
                    try
                    {
                        ConnectionFactory.ExecuteQuery("delete ccms_invoice_line_item where invoice_id = " + e.CommandArgument, trx);
                        ConnectionFactory.ExecuteQuery("delete ccms_invoice_line_order where invoice_id = " + e.CommandArgument, trx);
                        ConnectionFactory.ExecuteQuery("delete ccms_invoice where id = " + e.CommandArgument, trx);
                        trx.Commit();
                    }
                    catch (Exception ex)
                    {
                        trx.Rollback();
                        throw;
                    }
 
                    
                    if (ccmsInvoice != null)
                    {
                        Utility.BuildAuditLog("Invoice " + ccmsInvoice.Code + " deleted", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.DeleteInvoice);
                        Session[SessionVars.notice.ToString()] = "Selected Invoice Deleted!";
                    }

                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Invoice deleted");
                    BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
                }
                catch (Exception ex)
                {
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                    Literal_error.Text = "<li>An Error occured while deleting invoice. " + ex.Message + "</li>";
                    
                }
                finally
                {
                    if (cmd != null)
                        if (cmd.Connection != null)
                            cmd.Connection.Close();
                    task.EndTask();

                }
            }
        }

        protected void GridView_Invoice_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.GridView_Invoice.PageIndex = e.NewPageIndex;
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }

        protected void GridView_Invoice_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.FindControl("Button_deleteInvoice") != null)


                ((ImageButton)e.Row.FindControl("Button_deleteInvoice")).Attributes.Add("onClick",
                    "return confirm('Are you sure you want to delete Invoice " + ((DataRowView)e.Row.DataItem).Row["code"] + "?','CCMS')");


        }

        protected void Button_search_Click(object sender, EventArgs e)
        {
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }

        protected void Button_reset_Click(object sender, EventArgs e)
        {
            TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            DropDownList_Status.SelectedIndex = 0;
            DropDownList_CIT.SelectedIndex = 0;
        }

        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            Button_search_Click(this, null);
        }


    }
}