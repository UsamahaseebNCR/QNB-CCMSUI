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
using System.Reflection;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Text;
using System.Collections.Generic;
using Avanza.CCMS.DAL;

using System.IO;
using ComponentArt.Web.UI;



namespace Avanza.iSuite.UI
{
    public partial class AddEditInvoice : BasePage
    {
        CcmsInvoice invoice = null;
        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        }

        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            string regionID = Session[SessionVars.SelectedRegions.ToString()].ToString();
            if (regionID != null /*&& !regionID.Contains(",")*/
                && !regionID.StartsWith("1,"))
            {

                Region region = Region.LoadRegionByPk(regionID.Contains(",") ? int.Parse(regionID.Substring(0, regionID.IndexOf(","))) : int.Parse(regionID));
                if (region.IsOrganization)
                {
                    Label_Org.Text = region.RegionName;
                    Session[SessionVars.SelectedRegions.ToString()] = region.RegionId.ToString();
                    PopulateCIT(region.RegionId);

                }

            }
            else
                Label_Org.Text = "Please select Organization";

        }
        protected void GridView_OtherItems_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            DataTable dt = (DataTable)Session["dt"];
            dt.Rows[e.RowIndex].Delete();
            GridView_OtherItems.DataSource = dt;
            GridView_OtherItems.DataBind();
            ViewState["otherItemsChanged"] = 1;
        }
        private void PopulateCIT(int orgID)
        {
            DropDownList_CIT.Items.Clear();
            DropDownList_CIT.Items.Add("Select");
            RegionCit.RegionCitReader reader = RegionCit.ExecuteReader("region_id = " + orgID);
            while (reader.Read())
            {
                CcmsCit ccmsCIT = CcmsCit.LoadCcmsCitByPk(reader.CurrentRegionCit.CitId);
                if (ccmsCIT != null)
                    DropDownList_CIT.Items.Add(new ListItem(ccmsCIT.Name, ccmsCIT.Id.ToString()));
            }
            reader.Close();
        }


        private void PopulateInvoiceStatus()
        {
            DropDownList_Status.Items.Clear();
            DropDownList_Status.Items.Add("Select");
            CcmsInvoiceStatus.CcmsInvoiceStatusReader reader = CcmsInvoiceStatus.ExecuteReader("1=1");
            while (reader.Read())
                DropDownList_Status.Items.Add(new ListItem(reader.CurrentCcmsInvoiceStatus.Name, reader.CurrentCcmsInvoiceStatus.Id.ToString()));
            reader.Close();

        }
        private void LoadInvoice()
        {
            if (Request.QueryString["i"] != null)
            {
                invoice = CcmsInvoice.LoadCcmsInvoiceByPk(int.Parse(Request.QueryString["i"]));

                TextBox_ChequeNo.Text = invoice.ChequeNumber;
                TextBox_ChequeDate.Text = invoice.ChequeDate.Value.ToString("dd/MM/yyyy");
                if (invoice.Status == "Paid")
                    DropDownList_Status.SelectedIndex = 1;
                else if (invoice.Status == "Pending")
                    DropDownList_Status.SelectedIndex = 2;

                TextBox_Desc.Text = invoice.Remarks;
                TextBox_Amount.Text = invoice.Amount.ToString();

                TextBox_InvoiceDate.Text = invoice.InvoiceDate.ToString("dd/MM/yyyy");
                //DropDownList_CIT_Change(this, null);
                Label_InvoiceNo.Text = invoice.Code;
                Session[SessionVars.SelectedRegions.ToString()] = invoice.OrganizationId;
                Label_Org.Text = Region.LoadRegionByPk((int)invoice.OrganizationId).RegionName;
                PopulateCIT(invoice.OrganizationId.Value);
                DropDownList_CIT.SelectedValue = invoice.CitId.ToString();

                Button_SearchOrders_Click(this, null);

                CcmsInvoiceLineItem.CcmsInvoiceLineItemReader otherItemsReader = CcmsInvoiceLineItem.ExecuteReader("invoice_id = " + invoice.Id);
                DataTable dt = GetOtherItemDataTable();
                while (otherItemsReader.Read())
                {
                    DataRow dr = dt.NewRow();
                    dr[0] = dt.Rows.Count;
                    dr[1] = otherItemsReader.CurrentCcmsInvoiceLineItem.Name;
                    dr[2] = otherItemsReader.CurrentCcmsInvoiceLineItem.Description;
                    dr[3] = otherItemsReader.CurrentCcmsInvoiceLineItem.Amount;
                    dt.Rows.Add(dr);
                }
                otherItemsReader.Close();
                Session["dt"] = dt;
                GridView_OtherItems.DataSource = dt;
                GridView_OtherItems.DataBind();
            }
        }
        protected void GridView_CashOrders_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string cashOrderID = this.GridView_CashOrders.DataKeys[e.Row.DataItemIndex].Values["cash_order_id"].ToString();
                string invoiceId = this.GridView_CashOrders.DataKeys[e.Row.DataItemIndex].Values["invoice_id"].ToString();
                CcmsInvoiceLineOrder invoiceLineOrder = CcmsInvoiceLineOrder.LoadCcmsInvoiceLineOrder("order_id=" + cashOrderID);
                //if ((int)ConnectionFactory.ExecuteScalar("select count(id) from ccms_invoice_line_order where order_id = " + cashOrderID) > 0)
                if (invoiceLineOrder != null)
                {
                    ((CheckBox)e.Row.FindControl("chkbx")).Checked = true;
                    ((TextBox)e.Row.FindControl("TextBox_OrderDescription")).Text = invoiceLineOrder.Description;
                    ((TextBox)e.Row.FindControl("TextBox_Amount")).Text = invoiceLineOrder.Amount.ToString();
                }

                if (Request.QueryString["i"] == null)
                {
                    //CcmsCit cit = CcmsCit.LoadCcmsCitByPk(int.Parse(DropDownList_CIT.SelectedItem.Value));
                    //if (cit != null)
                    //{
                    CashOrders cashOrder = CashOrders.LoadCashOrdersByPk(int.Parse(cashOrderID));
                    if (cashOrder != null)
                    {
                        if (cashOrder.OrderNumber.StartsWith("RS"))
                        {
                            Atm atm = Atm.LoadAtmByPk(cashOrder.AtmId);
                            if (atm != null)
                                if (atm.NormalOrderCost.HasValue)
                                    ((TextBox)e.Row.FindControl("TextBox_Amount")).Text = atm.NormalOrderCost.ToString();
                        }
                        else
                        {
                            Atm atm = Atm.LoadAtmByPk(cashOrder.AtmId);
                            if (atm != null)
                                if (atm.EmergencyOrderCost.HasValue)
                                    ((TextBox)e.Row.FindControl("TextBox_Amount")).Text = atm.EmergencyOrderCost.ToString();
                        }
                    }

                    //}
                }


                CashOrderMonitoring cashOrderMonitoring = CashOrderMonitoring.LoadCashOrderMonitoring("current_order_id = " + cashOrderID);
                if (cashOrderMonitoring != null)
                {
                    if (!cashOrderMonitoring.CurrentOrderExecutedAt.HasValue)
                        ((HtmlImage)e.Row.FindControl("imgAlert")).Visible = true;
                    else
                        ((HtmlImage)e.Row.FindControl("imgAlert")).Visible = false;
                }
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (DialogCallBack.CausedCallback)
                return;
            if (!Page.IsPostBack)
            {
                Session["dt"] = null;
                TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                ViewState["SortExpression"] = "cash_order_datetime";
                ViewState["SortDirection"] = " desc";
                ViewState["SortExpressionOtherItems"] = "item";
                ViewState["SortDirectionOtherItems"] = " asc";

                PopulateInvoiceStatus();
                if (Request.QueryString["i"] != null)
                    LoadInvoice();
                else
                    refresh_ServerClick(this, null);

            }
        }
        private DataTable GetOtherItemDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("id", typeof(int));
            dt.Columns.Add("item");
            dt.Columns.Add("description");
            dt.Columns.Add("cost", typeof(decimal));
            return dt;
        }
        protected void Button_AddOtherItem_Click(object sender, EventArgs e)
        {
            bool found = false;
            try
            {
                DataTable dt = null;
                if (Session["dt"] == null)
                    Session["dt"] = GetOtherItemDataTable();

                dt = (DataTable)Session["dt"];

                foreach (DataRow dataRow in dt.Rows)
                {
                    if (dataRow["item"].ToString() == TextBox_Item.Text &&
                        dataRow["description"].ToString() == TextBox_Description.Text &&
                        dataRow["cost"].ToString() == TextBox_Cost.Text)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    DataRow dr = dt.NewRow();
                    dr[0] = dt.Rows.Count;
                    dr[1] = TextBox_Item.Text;
                    dr[2] = TextBox_Description.Text;
                    dr[3] = decimal.Parse(TextBox_Cost.Text);
                    dt.Rows.Add(dr);
                    ViewState["otherItemsChanged"] = 1;
                    Session["dt"] = dt;
                    GridView_OtherItems.DataSource = dt;
                    GridView_OtherItems.DataBind();
                    ClearOtherItemsInputControl();

                }

                else
                    Literal_error.Text = "<li>Other Item already added.</li>";


            }
            catch (Exception ex)
            {
                Literal_error.Text = "<li>" + ex.Message + "</li>";
                LogableTask.LogMonoActivityTask("AddOtherItems", MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }

        }

        private string Filter()
        {
            StringBuilder builder = new StringBuilder();
            if (TextBox_FromDate.Text.Length > 0)
            {
                builder.Append(" and cash_order_datetime>=convert(datetime,'" + TextBox_FromDate.Text + "',103)");
            }
            if (TextBox_ToDate.Text.Length > 0)
            {
                builder.Append(" and cash_order_datetime<=convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103)");
            }
            return builder.ToString();

        }
        private void BindOrderGrid(string orderBy)
        {
            if (DropDownList_CIT.SelectedIndex > 0)
            {
                DataTable dt = new DataTable();
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandText = @"select order_number,cash_order_id,cash_order_datetime,0 invoice_id
                                   from cash_orders inner join atm on cash_orders.atm_id = atm.atm_id 
                                   where region_id in (
                                   select region_id from region_cit where cit_id = " + DropDownList_CIT.SelectedItem.Value + ") " +
                                   " and cash_order_id not in (select order_id from ccms_invoice_line_order)" + Filter();
                if (Request.QueryString["i"] != null)
                {
                    cmd.CommandText += @" union select ccms_invoice_line_order.order_number,order_id cash_order_id,cash_order_datetime,ccms_invoice_line_order.invoice_id
                                   from ccms_invoice_line_order inner join cash_orders on cash_orders.cash_order_id = ccms_invoice_line_order.order_id 
                                where invoice_id = " + Request.QueryString["i"];
                }

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
                dt.DefaultView.Sort = orderBy;
                GridView_CashOrders.DataSource = dt.DefaultView;
                GridView_CashOrders.DataBind();
            }
        }

        protected void DropDownList_CIT_Change(object sender, EventArgs e)
        {
            Label_InvoiceNo.Text = (string)ConnectionFactory.ExecuteScalar("select prefix+'-'+convert(varchar,current_counter)+'-'+postfix from ccms_invoice_code where cit_id =" +
                   DropDownList_CIT.SelectedItem.Value + " and organization_id = " + Session[SessionVars.SelectedRegions.ToString()]);
            Button_SearchOrders_Click(this, null);
        }
        private void SaveOrderItems(int invoiceID, SqlTransaction trxn)
        {
            foreach (GridViewRow row in GridView_CashOrders.Rows)
            {
                string orderID = this.GridView_CashOrders.DataKeys[row.DataItemIndex].Values["cash_order_id"].ToString();
                CheckBox chk = (CheckBox)row.FindControl("chkbx");
                if (chk.Checked)
                {
                    CcmsInvoiceLineOrder ccmsInvoiceLineOrder = new CcmsInvoiceLineOrder();
                    ccmsInvoiceLineOrder.InvoiceId = invoiceID;
                    ccmsInvoiceLineOrder.OrderId = int.Parse(orderID);
                    ccmsInvoiceLineOrder.OrderNumber = CashOrders.LoadCashOrdersByPk(int.Parse(orderID)).OrderNumber;
                    TextBox txtAmount = (TextBox)row.FindControl("TextBox_Amount");
                    ccmsInvoiceLineOrder.Amount = decimal.Parse(txtAmount.Text);

                    TextBox txtOrderDesc = (TextBox)row.FindControl("TextBox_OrderDescription");
                    ccmsInvoiceLineOrder.Description = txtOrderDesc.Text;
                    ccmsInvoiceLineOrder.Save(trxn.Connection, trxn);
                }
            }
        }
        private void SaveOtherItems(int invoiceID, SqlTransaction trxn)
        {
            if (Session["dt"] != null)
            {
                DataTable dt = (DataTable)Session["dt"];
                foreach (DataRow dr in dt.Rows)
                {
                    CcmsInvoiceLineItem ccmsInvoiceLineItem = new CcmsInvoiceLineItem();
                    ccmsInvoiceLineItem.Name = dr[1].ToString();
                    ccmsInvoiceLineItem.Description = dr[2].ToString();
                    ccmsInvoiceLineItem.Amount = decimal.Parse(dr[3].ToString());
                    ccmsInvoiceLineItem.InvoiceId = invoiceID;
                    ccmsInvoiceLineItem.Save(trxn.Connection, trxn);
                }
            }
        }
        protected void Button_SearchOrders_Click(object sender, EventArgs e)
        {
            BindOrderGrid(ViewState["SortExpression"] + " " + ViewState["SortDirection"]);
        }

        private void ClearOtherItemsInputControl()
        {
            TextBox_Item.Text = "";
            TextBox_Description.Text = "";
            TextBox_Cost.Text = "";
        }
        protected void Button_Clear_Click(object sender, EventArgs e)
        {
            ClearOtherItemsInputControl();

        }
        protected void Button_Reset_Click(object sender, EventArgs e)
        {
            TextBox_FromDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
            TextBox_ToDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
        }
        protected void Button_SaveUpdateInvoice_Click(object sender, EventArgs e)
        {

            SqlTransaction trxn = null;
            SqlCommand cmd = null;
            string msg = null;
            try
            {
                cmd = ConnectionFactory.GetNewCommand(true);
                if (Request.QueryString["i"] != null)
                {
                    CcmsInvoice invoice = CcmsInvoice.LoadCcmsInvoiceByPk(int.Parse(Request.QueryString["i"]));
                    //Cheque input
                    invoice.ChequeNumber = TextBox_ChequeNo.Text;
                    invoice.ChequeDate = DateTime.ParseExact(TextBox_ChequeDate.Text, "dd/MM/yyyy", null);
                    invoice.CreatedOn = DateTime.Now;
                    invoice.CreatedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                    invoice.IsDeleted = false;
                    invoice.Status = DropDownList_Status.SelectedItem.Text;
                    invoice.Remarks = TextBox_Desc.Text;
                    invoice.Amount = decimal.Parse(TextBox_Amount.Text);
                    invoice.CitId = int.Parse(DropDownList_CIT.SelectedItem.Value);
                    invoice.ModifiedOn = DateTime.Now;
                    invoice.ModifiedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                    invoice.OrganizationId = int.Parse(Session[SessionVars.SelectedRegions.ToString()].ToString());
                    invoice.Code = Label_InvoiceNo.Text;
                    invoice.InvoiceDate = DateTime.ParseExact(TextBox_InvoiceDate.Text, "dd/MM/yyyy", null);
                    trxn = cmd.Connection.BeginTransaction();
                    invoice.Save(trxn.Connection, trxn);

                    ConnectionFactory.ExecuteQuery("delete ccms_invoice_line_item where invoice_id = " + invoice.Id, trxn);
                    ConnectionFactory.ExecuteQuery("delete ccms_invoice_line_order where invoice_id = " + invoice.Id, trxn);

                    SaveOrderItems(invoice.Id, trxn);
                    SaveOtherItems(invoice.Id, trxn);
                    msg = "Invoice Edited Successfully";
                    Utility.BuildAuditLog("Invoice " + invoice.Code + " modified", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.ModifyInvoice, trxn.Connection, trxn);
                }
                else
                {
                    CcmsInvoice newInvoice = new CcmsInvoice();
                    //Cheque input
                    newInvoice.ChequeNumber = TextBox_ChequeNo.Text;
                    newInvoice.ChequeDate = DateTime.ParseExact(TextBox_ChequeDate.Text, "dd/MM/yyyy", null);
                    newInvoice.CreatedOn = DateTime.Now;
                    newInvoice.CreatedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                    newInvoice.IsDeleted = false;
                    newInvoice.Status = DropDownList_Status.SelectedItem.Text;
                    newInvoice.Remarks = TextBox_Desc.Text;
                    newInvoice.Amount = decimal.Parse(TextBox_Amount.Text);
                    newInvoice.CitId = int.Parse(DropDownList_CIT.SelectedItem.Value);
                    newInvoice.OrganizationId = int.Parse(Session[SessionVars.SelectedRegions.ToString()].ToString());
                    newInvoice.Code = Label_InvoiceNo.Text;
                    newInvoice.InvoiceDate = DateTime.ParseExact(TextBox_InvoiceDate.Text, "dd/MM/yyyy", null);
                    trxn = cmd.Connection.BeginTransaction();
                    newInvoice.Save(trxn.Connection, trxn);
                    SaveOrderItems(newInvoice.Id, trxn);
                    SaveOtherItems(newInvoice.Id, trxn);
                    msg = "Invoice Created Successfully";
                    Utility.BuildAuditLog("Invoice " + newInvoice.Code + " created", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.CreateInvoice, trxn.Connection, trxn);
                    ConnectionFactory.ExecuteQuery(" update ccms_invoice_code set  current_counter = current_counter+1 where cit_id =" +
                    DropDownList_CIT.SelectedItem.Value + " and organization_id = " + Session[SessionVars.SelectedRegions.ToString()], trxn);
                }
                trxn.Commit();

                Session[SessionVars.notice.ToString()] = msg;
                Response.Redirect("~/admin/Invoices.aspx");
            }
            catch (Exception ex)
            {
                trxn.Rollback();
                Literal_error.Text = ex.Message;
                LogableTask.LogMonoActivityTask("SaveInvoice", MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            {
                if (cmd != null)
                    if (cmd.Connection != null)
                        cmd.Connection.Close();
            }

        }
        private void BindOtherItemsGrid(string orderBy)
        {
            DataTable dt = (DataTable)Session["dt"];
            dt.DefaultView.Sort = orderBy;
            GridView_OtherItems.DataSource = dt.DefaultView;
            GridView_OtherItems.DataBind();
        }
        protected void GridView_OtherItems_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (ViewState["SortExpressionOtherItems"].ToString() == e.SortExpression)
            {
                if (ViewState["SortDirectionOtherItems"].ToString() == " asc")
                    ViewState["SortDirectionOtherItems"] = " desc";
                else
                    ViewState["SortDirectionOtherItems"] = " asc";
            }
            else
            {
                ViewState["SortExpressionOtherItems"] = e.SortExpression;
                ViewState["SortDirectionOtherItems"] = " asc";
            }

            BindOtherItemsGrid(ViewState["SortExpressionOtherItems"].ToString() + ViewState["SortDirectionOtherItems"].ToString());
        }
        protected void GridView_CashOrders_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (ViewState["SortExpression"].ToString() == e.SortExpression)
            {
                if (ViewState["SortDirection"].ToString() == " asc")
                    ViewState["SortDirection"] = " desc";
                else
                    ViewState["SortDirection"] = " asc";
            }
            else
            {
                ViewState["SortExpression"] = e.SortExpression;
                ViewState["SortDirection"] = " asc";
            }
            BindOrderGrid(ViewState["SortExpression"] + " " + ViewState["SortDirection"]);
        }
        protected void DialogCallBack_Callback(object sender, CallBackEventArgs e)
        {
            LogableTask task = LogableTask.NewTask("DialogCallBack_Callback");
            StringBuilder builder = new StringBuilder();
            try
            {

                CashOrders cashOrders = CashOrders.LoadCashOrdersByPk(int.Parse(e.Parameter.ToString()));
                if (cashOrders != null)
                {
                    builder.Append("<tr><td><b>Cash Order Datetime</b></td><td><b>" + cashOrders.CashOrderDatetime.ToString("dd/MM/yyyy") + "</b></td></tr>");
                    builder.Append("<tr><td><b>Order Number</b></td><td><b>" + cashOrders.OrderNumber + "</b></td></tr>");
                    builder.Append("<tr><td><b>Cash Order Type</b></td><td><b>" + cashOrders.CashOrderType + "</b></td></tr>");
                    builder.Append("<tr><td><b>Dispatch Time</b></td><td><b>" + cashOrders.DispatchTime + "</b></td></tr>");
                    builder.Append("<tr><td><b>Is Dispatch On ATM</b></td><td><b>" + (cashOrders.IsUploaded ? "True" : "False") + "</b></td></tr>");
                    builder.Append("<tr><td><b>Cassette1 Denomination</b></td><td><b>" + cashOrders.Cassette1Denomination + "</b></td></tr>");
                    builder.Append("<tr><td><b>Cassette1 Suggested Notes</b></td><td><b>" + cashOrders.Cassette1SuggestedNotes + "</b></td></tr>");
                    builder.Append("<tr><td><b>Cassette1 Remaining Notes</b></td><td><b>" + cashOrders.Cassette1RemainingNotes + "</b></td></tr>");

                    builder.Append("<tr><td><b>Cassette2 Denomination</b></td><td><b>" + cashOrders.Cassette2Denomination + "</b></td></tr>");
                    builder.Append("<tr><td><b>Cassette2 Suggested Notes</b></td><td><b>" + cashOrders.Cassette2SuggestedNotes + "</b></td></tr>");
                    builder.Append("<tr><td><b>Cassette2 Remaining Notes</b></td><td><b>" + cashOrders.Cassette2RemainingNotes + "</b></td></tr>");

                    builder.Append("<tr><td><b>Cassette3 Denomination</b></td><td><b>" + cashOrders.Cassette3Denomination + "</b></td></tr>");
                    builder.Append("<tr><td><b>Cassette3 Suggested Notes</b></td><td><b>" + cashOrders.Cassette3SuggestedNotes + "</b></td></tr>");
                    builder.Append("<tr><td><b>Cassette3 Remaining Notes</b></td><td><b>" + cashOrders.Cassette3RemainingNotes + "</b></td></tr>");

                    builder.Append("<tr><td><b>Cassette4 Denomination</b></td><td><b>" + cashOrders.Cassette4Denomination + "</b></td></tr>");
                    builder.Append("<tr><td><b>Cassette4 Suggested Notes</b></td><td><b>" + cashOrders.Cassette4SuggestedNotes + "</b></td></tr>");
                    builder.Append("<tr><td><b>Cassette4 Remaining Notes</b></td><td><b>" + cashOrders.Cassette4RemainingNotes + "</b></td></tr>");










                }

                e.Output.Write("<script> receiveCallBackResult('<table border=1 width=100%>" + builder.ToString() + "</table>');</script>");
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, ex);
            }

            finally
            {
                task.EndTask();
            }

        }

    }
}