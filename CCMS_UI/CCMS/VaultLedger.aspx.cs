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

using Avanza.CCMS.DAL;
using System.Text;
using System.Threading;


namespace Avanza.iSuite.UI
{
    public partial class VaultLedger : BasePage
    {

        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        }
        private void PopulateVaultTransactionType()
        {
            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            cmd.CommandText = "select transaction_type,id from ccms_vault_transaction_type";
            DataTable dt = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
            DropDownList_VaultTrxnType.DataTextField = "transaction_type";
            DropDownList_VaultTrxnType.DataValueField = "id";
            DropDownList_VaultTrxnType.DataSource = dt;
            DropDownList_VaultTrxnType.DataBind();
        }
        private void PopulateCIT()
        {
            DropDownList_CIT.Items.Clear();
            DropDownList_CIT.Items.Add("Select");
            CcmsCit.CcmsCitReader reader = CcmsCit.ExecuteReader("1=1");
            while (reader.Read())
                DropDownList_CIT.Items.Add(new ListItem(reader.CurrentCcmsCit.Name, reader.CurrentCcmsCit.Id.ToString()));
            reader.Close();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (!CheckRight((int)Permissions.ViewLedger, true))
                    Response.Redirect("~/admin/AccessDenied.aspx");

                //PopulateCIT();
                PopulateCIT();
                PopulateVaultTransactionType();
                ResetAllControls();

                ViewState["sortDirection"] = " desc";
                ViewState["SortExpression"] = "transaction_date";

                BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
            }

        }
        //private void PopulateCIT()
        //{
        //    DropDownList_CIT.Items.Clear();
        //    DropDownList_CIT.Items.Add("Select");
        //    CcmsCit.CcmsCitReader reader = CcmsCit.ExecuteReader("1=1");
        //    while (reader.Read())
        //        DropDownList_CIT.Items.Add(new ListItem(reader.CurrentCcmsCit.Name, reader.CurrentCcmsCit.Id.ToString()));
        //    reader.Close();
        //}

        string Filter()
        {
            string filter = "";

            if (TextBox_FromDate.Text.Length > 0)
                filter += " and transaction_date >= convert(datetime,'" + TextBox_FromDate.Text + "',103)";

            if (TextBox_ToDate.Text.Length > 0)
                filter += " and transaction_date <= convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103)";

            //if (TextBox_ChequeNo.Text.Length > 0)
            //    filter += " and cheque_number like '%" + TextBox_ChequeNo.Text + "%'";

            if (DropDownList_CIT.SelectedIndex > 0)
                filter += " and cit_id = " + DropDownList_CIT.SelectedItem.Value;

            if (DropDownList_VaultTrxnType.SelectedIndex > 0)
                filter += " and ccms_vault_transaction_type.transaction_type = '" + DropDownList_VaultTrxnType.SelectedItem.Text + "'";

            if (DropDownList_TransactionType.SelectedIndex > 0)
                filter += " and ccms_vault_ledger.transaction_type = '" + DropDownList_TransactionType.SelectedItem.Value + "'";



            //if (DropDownList_Vault.SelectedIndex > 0)
            //    filter += " and vault_id = " + DropDownList_Vault.SelectedValue;

            return filter;
        }

        private void BindGrid(string orderby)
        {
            LogableTask task = LogableTask.NewTask("BindGrid");
            SqlCommand cmd = null;
            try
            {
                DataTable dtGrid = new DataTable();
                dtGrid.Columns.Add("name");
                dtGrid.Columns.Add("title");
                dtGrid.Columns.Add("transaction_date", typeof(DateTime));
                dtGrid.Columns.Add("debit");
                dtGrid.Columns.Add("credit");
                dtGrid.Columns.Add("vault_transaction_type");
                dtGrid.Columns.Add("balance");
                dtGrid.Columns.Add("ledger_amount_detail");
                dtGrid.Columns.Add("description");
                dtGrid.Columns.Add("user_full_name");





                cmd = ConnectionFactory.GetNewCommand(true);
                cmd.CommandText = @"SELECT transaction_date,ccms_vault_ledger.description,ccms_vault_ledger.transaction_type,ledger_amount,title,vault_transaction_type,ccms_cit.name,quantity,ledger_id,denomination_name,ccms_vault_ledger.balance,user_full_name
                                    FROM ccms_vault_ledger inner join ccms_vault_ledger_detail on ccms_vault_ledger.id = ccms_vault_ledger_detail.ledger_id
                                    inner join ccms_vault_note_type on ccms_vault_note_type.id = ccms_vault_ledger_detail.vault_note_type_id
                                     inner join ccms_vault on ccms_vault_ledger.vault_id= ccms_vault.id 
                                    inner join ccms_vault_transaction_type on ccms_vault_transaction_type.transaction_type = ccms_vault_ledger.vault_transaction_type
                                    inner join ccms_cit on ccms_cit.id = ccms_vault.cit_id 
                                    inner join app_user on app_user.user_id = ccms_vault_ledger.posted_by
                                    where ccms_vault.is_deleted=0 " + Filter() + " order by " + orderby;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dtLedger = new DataTable();
                adapter.Fill(dtLedger);
                cmd.Connection.Close();

                DataTable dt = dtLedger.DefaultView.ToTable(true, new string[] { "ledger_id" });
                foreach (DataRow dr in dt.Rows)
                {
                    DataRow[] drLedger = dtLedger.Select("ledger_id =" + dr["ledger_id"]);
                    if (drLedger.Length > 0)
                    {
                        DataRow drNew = dtGrid.NewRow();
                        drNew["user_full_name"] = drLedger[0]["user_full_name"];
                        drNew["title"] = drLedger[0]["title"];
                        drNew["name"] = drLedger[0]["name"];
                        drNew["vault_transaction_type"] = drLedger[0]["vault_transaction_type"];
                        drNew["transaction_date"] = drLedger[0]["transaction_date"];
                        drNew["description"] = drLedger[0]["description"];

                        if (drLedger[0]["transaction_type"].ToString() == "Cr")
                            drNew["credit"] = drLedger[0]["ledger_amount"];
                        else
                            drNew["debit"] = drLedger[0]["ledger_amount"];
                        drNew["balance"] = drLedger[0]["balance"];
                        StringBuilder builder = new StringBuilder();
                        for (int k = 0; k < drLedger.Length;k++ )
                            builder.Append("Type " + (k + 1) + ":" + drLedger[k]["quantity"].ToString() + "*" + drLedger[k]["denomination_name"].ToString() + ",");
                        drNew["ledger_amount_detail"] = builder.ToString();
                        dtGrid.Rows.Add(drNew);
                    }
                }

                this.GridView_VaultLedger.DataSource = dtGrid;
                this.GridView_VaultLedger.DataBind();
                Literal_count.Text = dtGrid.Rows.Count + " records.";
            }
            catch (Exception ex)
            {
                Literal_error.Text = ex.Message;
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            {
                if (cmd != null)
                    if (cmd.Connection != null)
                        cmd.Connection.Close();
                task.EndTask(); }
        }

        protected void GridView_VaultLedger_Sorting(object sender, GridViewSortEventArgs e)
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




        protected void GridView_VaultLedger_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.GridView_VaultLedger.PageIndex = e.NewPageIndex;
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }


        protected void Button_search_Click(object sender, EventArgs e)
        {
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }


        private void ResetAllControls()
        {
            TextBox_FromDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
            TextBox_ToDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
            DropDownList_CIT.SelectedIndex = 0;
            DropDownList_VaultTrxnType.SelectedIndex = 0;
        }

        protected void Button_reset_Click(object sender, EventArgs e)
        {
            ResetAllControls();
        }

        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            Button_search_Click(this, null);
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