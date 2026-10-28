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
namespace CCMSUI.CCMS.DashBoards
{
    public partial class DashBoardGrid_VaultSummary : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                if (!CheckRight((int)Permissions.ViewVaultSummaryDashboard, true))
                    Response.Redirect("../../admin/aspx/AccessDenied.aspx");


                ViewState["SortExpression"] = "title ";
                ViewState["sortDirection"] = "asc ";

                ViewState["SortExpressionCIT"] = "title ";
                ViewState["sortDirectionCIT"] = "asc ";
                BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
                BindTempVaultGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
                BindGridCIT(ViewState["SortExpressionCIT"].ToString() + ViewState["sortDirectionCIT"].ToString());

            }

        }
        protected void GridView_VaultBalanceInvestigation_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.GridView_VaultBalanceInvestigation.PageIndex = e.NewPageIndex;
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }

        protected void GridView_VaultBalanceInvestigation_Sorting(object sender, GridViewSortEventArgs e)
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


        //protected void GridView_CITVaultBalances_PageIndexChanging(object sender, GridViewPageEventArgs e)
        //{
        //    GridView_CITVaultBalances.PageIndex = e.NewPageIndex;
        //    BindGridCIT(ViewState["SortExpressionCIT"].ToString() + ViewState["sortDirectionCIT"].ToString());
        //}

        protected void GridView_CITVaultBalances_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (ViewState["SortExpressionCIT"].ToString() == e.SortExpression)
            {
                if (ViewState["sortDirectionCIT"].ToString() == " asc")
                    ViewState["sortDirectionCIT"] = " desc";
                else
                    ViewState["sortDirectionCIT"] = " asc";
            }
            else
            {
                ViewState["SortExpressionCIT"] = e.SortExpression;
                ViewState["sortDirectionCIT"] = " asc";
            }
            BindGridCIT(ViewState["SortExpressionCIT"].ToString() + ViewState["sortDirectionCIT"].ToString());
        }

        string Filter()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(" and ledger_date >= Convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + "',103) ");
            builder.Append(" and ledger_date <= Convert(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + " 23:59:59',103) ");

            return builder.ToString();
        }


        //        private void BindGrid(string orderBy)
        //        {
        //            SqlCommand cmd = null;
        //            LogableTask task = LogableTask.NewTask("BindGrid");
        //            try
        //            {
        //                metaRefresh.Content = (((AppSetting)Application[ApplicationVars.AppSettings.ToString()]).DashboardRefreshInterval * 60).ToString();
        //                Page.RegisterStartupScript("script", "<script>function init() {cd(\"" + (int.Parse(metaRefresh.Content) / 60) + "\");} init();</script>");

        //                cmd = ConnectionFactory.GetNewCommand(false);
        //                cmd.CommandTimeout = 500;
        //                cmd.CommandText = @"select title,opening_balance,closing_balance,bulk_cash,cash_delivered_to_atm,cash_returned_to_atm,denomination_name,transaction_date,generated_at
        //                                    from CCMS_vault_day_balance_summary inner join CCMS_vault on CCMS_vault.id = ccms_vault_day_balance_summary.vault_id where transaction_date =(select max(transaction_date) from CCMS_vault_day_balance_summary)  order by " + orderBy;
        //                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
        //                DataTable dtMonitoring = new DataTable();
        //                adapter.Fill(dtMonitoring);
        //                GridView_VaultBalanceInvestigation.DataSource = dtMonitoring;
        //                GridView_VaultBalanceInvestigation.DataBind();

        //            }
        //            catch (Exception ex)
        //            {
        //                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
        //                Literal_errorMsg.Text = "<li>" + ex.Message + "</li>";

        //            }
        //            finally
        //            {
        //                task.EndTask();
        //            }

        //        }
        private void BindGridCIT(string orderBy)
        {
//            SqlCommand cmd = null;
//            LogableTask task = LogableTask.NewTask("BindGrid");
//            try
//            {
//                cmd = ConnectionFactory.GetNewCommand(false);
//                cmd.CommandTimeout = 500;
//                cmd.CommandText = @"select title, denomination_name,opening_balance,cash_recieved_from_bank,cash_delivered_to_atm,cash_returned_from_atm,closing_balance,generated_at,vault_summary_date
//                                    from vault_settlement inner join CCMS_vault outerVault on outerVault.id = vault_settlement.vault_id
//                                    inner join vault_settlement_detail on vault_settlement.vault_settlement_id =vault_settlement_detail.vault_settlement_id
//                                    and vault_summary_date in (select MAX(vault_summary_date) from vault_settlement where vault_id = outerVault.id)";
//                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
//                DataTable dtMonitoring = new DataTable();
//                adapter.Fill(dtMonitoring);
//                GridView_CITVaultBalances.DataSource = dtMonitoring;
//                GridView_CITVaultBalances.DataBind();

//            }
//            catch (Exception ex)
//            {
//                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
//                Literal_errorMsg.Text = "<li>" + ex.Message + "</li>";

//            }
//            finally
//            {
//                task.EndTask();

//            }
        }
        private void BindTempVaultGrid(string orderby)
        {
            LogableTask task = LogableTask.NewTask("BindTempVaultGrid");
            SqlCommand cmd = null;
            DataTable dt = null;
            try
            {
                bool isTitlePrinted = false;
                DataTable dtGrid = new DataTable();
                dtGrid.Columns.Add("Title");
                dtGrid.Columns.Add("Date");
                dtGrid.Columns.Add("Type");
                cmd = ConnectionFactory.GetNewCommand(false);
                StringBuilder builder = new StringBuilder();
                cmd.CommandText = @"select title,ledger_date,vault_transaction_type,transaction_type,denomination_name,total_quantity,total_value, outerVaultBalance.vault_id vaultID
                                    from CCMS_temp_vault_day_balance outerVaultBalance inner join CCMS_vault on CCMS_vault.id = outerVaultBalance.vault_id where ledger_date in (select max(ledger_date) from CCMS_temp_vault_day_balance
where vault_transaction_Type = outerVaultBalance.vault_transaction_type
and vault_id = outerVaultBalance.vault_id) order by " + orderby;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                dt = new DataTable();
                adapter.Fill(dt);

                DataTable dtDenom = dt.DefaultView.ToTable(true, new string[] { "denomination_name" });
                foreach (DataRow dr in dtDenom.Rows)
                    dtGrid.Columns.Add(dr[0].ToString());
                dtGrid.Columns.Add("Total");
                dtGrid.Columns.Add("Value");

                DataTable dtVaultIDs = dt.DefaultView.ToTable(true, new string[] { "vaultID" });

                foreach (DataRow dr in dtVaultIDs.Rows)
                {
                    isTitlePrinted = false;
                    //Inbound...
                    int total = 0;
                    int totalVal = 0;
                    DataRow newRow = dtGrid.NewRow();
                    newRow["type"] = "Inbound Bulk";
                    DataRow[] result = dt.Select("vault_transaction_type='Bulk Cash' and vaultID=" + dr[0]);
                    if (result.Length > 0)
                    {
                        for (int k = 0; k < result.Length; k++)
                        {
                            newRow[k + 3] = VaultLedger.parseValueIntoCurrency(decimal.Parse(result[k]["total_quantity"].ToString()));
                            total += int.Parse(result[k]["total_quantity"].ToString());
                            totalVal += int.Parse(result[k]["total_value"].ToString());
                        }
                        newRow["total"] = VaultLedger.parseValueIntoCurrency(decimal.Parse(total.ToString()));
                        newRow["Value"] = VaultLedger.parseValueIntoCurrency(decimal.Parse(totalVal.ToString()));
                        newRow["Title"] = result[0]["title"];
                        newRow["Date"] = DateTime.Parse(result[0]["ledger_date"].ToString()).ToString("dd/MM/yyyy");
                        isTitlePrinted = true;
                    }
                    dtGrid.Rows.Add(newRow);
                    dtGrid.Rows.Add(dtGrid.NewRow());

                    //Inbound...
                    total = 0;
                    totalVal = 0;
                    newRow = dtGrid.NewRow();
                    newRow["type"] = "Inbound (Residual)";
                    result = dt.Select("vault_transaction_type='Inbound' and vaultID=" + dr[0]);
                    if (result.Length > 0)
                    {
                        for (int k = 0; k < result.Length; k++)
                        {
                            newRow[k + 3] = VaultLedger.parseValueIntoCurrency(decimal.Parse(result[k]["total_quantity"].ToString()));
                            total += int.Parse(result[k]["total_quantity"].ToString());
                            totalVal += int.Parse(result[k]["total_value"].ToString());
                        }
                        newRow["total"] = VaultLedger.parseValueIntoCurrency(decimal.Parse(total.ToString()));
                        newRow["Value"] = VaultLedger.parseValueIntoCurrency(decimal.Parse(totalVal.ToString()));
                        if (isTitlePrinted)
                            newRow["Title"] = "";
                        else
                        {
                            newRow["Title"] = result[0]["title"];
                            isTitlePrinted = true;
                        }

                        newRow["Date"] = DateTime.Parse(result[0]["ledger_date"].ToString()).ToString("dd/MM/yyyy");
                    }
                    dtGrid.Rows.Add(newRow);


                    //Outbound...
                    total = 0;
                    totalVal = 0;
                    newRow = dtGrid.NewRow();
                    newRow["type"] = "Outbound (Replenishment)";
                    result = dt.Select("vault_transaction_type='Outbound' and vaultID=" + dr[0]);
                    if (result.Length > 0)
                    {
                        for (int k = 0; k < result.Length; k++)
                        {
                            newRow[k + 3] = VaultLedger.parseValueIntoCurrency(decimal.Parse(result[k]["total_quantity"].ToString()));
                            total += int.Parse(result[k]["total_quantity"].ToString());
                            totalVal += int.Parse(result[k]["total_value"].ToString());
                        }
                        newRow["total"] = VaultLedger.parseValueIntoCurrency(decimal.Parse(total.ToString()));
                        newRow["Value"] = VaultLedger.parseValueIntoCurrency(decimal.Parse(totalVal.ToString()));
                        if (isTitlePrinted)
                            newRow["Title"] = "";
                        else
                        {
                            newRow["Title"] = result[0]["title"];
                            isTitlePrinted = true;
                        }

                        newRow["Date"] = DateTime.Parse(result[0]["ledger_date"].ToString()).ToString("dd/MM/yyyy");
                    }
                    dtGrid.Rows.Add(newRow);

                    //
                    dtGrid.Rows.Add(dtGrid.NewRow());
                    newRow = dtGrid.NewRow();
                    newRow["type"] = "Net";
                    dtGrid.Rows.Add(newRow);
                    dtGrid.Rows.Add(dtGrid.NewRow());

                    total = 0;
                    totalVal = 0;
                    newRow = dtGrid.NewRow();
                    newRow["type"] = "Net Adjustments (+)";
                    result = dt.Select("vault_transaction_type='Adjustment Credit' and vaultID=" + dr[0]);
                    if (result.Length > 0)
                    {
                        for (int k = 0; k < result.Length; k++)
                        {
                            newRow[k + 3] = VaultLedger.parseValueIntoCurrency(decimal.Parse(result[k]["total_quantity"].ToString()));
                            total += int.Parse(result[k]["total_quantity"].ToString());
                            totalVal += int.Parse(result[k]["total_value"].ToString());
                        }
                        newRow["total"] = VaultLedger.parseValueIntoCurrency(decimal.Parse(total.ToString()));
                        newRow["Value"] = VaultLedger.parseValueIntoCurrency(decimal.Parse(totalVal.ToString()));
                        if (isTitlePrinted)
                            newRow["Title"] = "";
                        else
                        {
                            newRow["Title"] = result[0]["title"];
                            isTitlePrinted = true;
                        }

                        newRow["Date"] = DateTime.Parse(result[0]["ledger_date"].ToString()).ToString("dd/MM/yyyy");
                    }
                    dtGrid.Rows.Add(newRow);

                    total = 0;
                    totalVal = 0;
                    newRow = dtGrid.NewRow();
                    newRow["type"] = "Net Adjustments (-)";
                    result = dt.Select("vault_transaction_type='Adjustment Debit' and vaultID=" + dr[0]);
                    if (result.Length > 0)
                    {
                        for (int k = 0; k < result.Length; k++)
                        {
                            newRow[k + 3] = VaultLedger.parseValueIntoCurrency(decimal.Parse(result[k]["total_quantity"].ToString()));
                            total += int.Parse(result[k]["total_quantity"].ToString());
                            totalVal += int.Parse(result[k]["total_value"].ToString());
                        }
                        newRow["total"] = VaultLedger.parseValueIntoCurrency(decimal.Parse(total.ToString()));
                        newRow["Value"] = VaultLedger.parseValueIntoCurrency(decimal.Parse(totalVal.ToString()));
                        if (isTitlePrinted)
                            newRow["Title"] = "";
                        else
                        {
                            newRow["Title"] = result[0]["title"];
                            isTitlePrinted = true;
                        }

                        newRow["Date"] = DateTime.Parse(result[0]["ledger_date"].ToString()).ToString("dd/MM/yyyy");
                    }
                    dtGrid.Rows.Add(newRow);


                    dtGrid.Rows.Add(dtGrid.NewRow());
                    //Inbound...
                    total = 0;
                    totalVal = 0;
                    newRow = dtGrid.NewRow();
                    newRow["type"] = "Total Available";
                    result = dt.Select("vault_transaction_type='Total' and vaultID=" + dr[0]);
                    if (result.Length > 0)
                    {
                        for (int k = 0; k < result.Length; k++)
                        {
                            newRow[k + 3] = VaultLedger.parseValueIntoCurrency(decimal.Parse(result[k]["total_quantity"].ToString()));
                            total += int.Parse(result[k]["total_quantity"].ToString());
                            totalVal += int.Parse(result[k]["total_value"].ToString());
                        }
                        newRow["total"] = VaultLedger.parseValueIntoCurrency(decimal.Parse(total.ToString()));
                        newRow["Value"] = VaultLedger.parseValueIntoCurrency(decimal.Parse(totalVal.ToString()));
                        if (isTitlePrinted)
                            newRow["Title"] = "";
                        else
                        {
                            newRow["Title"] = result[0]["title"];
                            isTitlePrinted = true;
                        }

                        newRow["Date"] = DateTime.Parse(result[0]["ledger_date"].ToString()).ToString("dd/MM/yyyy");
                    }
                    dtGrid.Rows.Add(newRow);
                }
                //newRow = dtGrid.NewRow();
                //newRow["type"] = "Total Available";
                //for (int i = dt.Rows.Count - 4; i < dt.Rows.Count ; i++)
                //{
                //    newRow[i + 1] = dt.Rows[i]["total_quantity"];
                //    total += int.Parse(dt.Rows[i]["total_quantity"].ToString());
                //}
                //newRow["total"] = total;
                //dtGrid.Rows.Add(newRow);
                //Literal_summary.Text = dt.Rows.Count + " records.";
                this.GridView_TempVaultBalanceInvestigation.DataSource = dtGrid;
                this.GridView_TempVaultBalanceInvestigation.DataBind();

            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_errorMsg.Text = "<li>" + ex.Message + "</li>";
            }
            finally
            {
                task.EndTask();
            }
        }
        private void BindGrid(string orderby)
        {
            LogableTask task = LogableTask.NewTask("BindGrid");
            SqlCommand cmd = null;
            DataTable dt = null;
            try
            {
                bool isTitlePrinted = false;
                metaRefresh.Content = (((AppSetting)Application[ApplicationVars.AppSettings.ToString()]).DashboardRefreshInterval * 60).ToString();
                Page.RegisterStartupScript("script", "<script>function init() {cd(\"" + (int.Parse(metaRefresh.Content) / 60) + "\");} init();</script>");

                DataTable dtGrid = new DataTable();
                dtGrid.Columns.Add("Title");
                dtGrid.Columns.Add("Date");
                dtGrid.Columns.Add("Type");
                cmd = ConnectionFactory.GetNewCommand(false);
                StringBuilder builder = new StringBuilder();
                cmd.CommandText = @"select title,ledger_date,vault_transaction_type,transaction_type,denomination_name,total_quantity,total_value, outerVaultBalance.vault_id vaultID
from CCMS_vault_day_balance outerVaultBalance inner join CCMS_vault on CCMS_vault.id = outerVaultBalance.vault_id 
where ledger_date in (select max(ledger_date) from CCMS_vault_day_balance
where vault_transaction_Type = outerVaultBalance.vault_transaction_type
and vault_id = outerVaultBalance.vault_id) order by " + orderby;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                dt = new DataTable();
                adapter.Fill(dt);

                DataTable dtDenom = dt.DefaultView.ToTable(true, new string[] { "denomination_name" });
                foreach (DataRow dr in dtDenom.Rows)
                    dtGrid.Columns.Add(dr[0].ToString());
                dtGrid.Columns.Add("Total");
                dtGrid.Columns.Add("Value");

               DataTable dtVaultIDs = dt.DefaultView.ToTable(true, new string[] { "vaultID" });

               foreach (DataRow dr in dtVaultIDs.Rows)
               {
                   isTitlePrinted = false;
                   //Inbound...
                   int total = 0;
                   int totalVal = 0;
                   DataRow newRow = dtGrid.NewRow();
                   newRow["type"] = "Inbound Bulk";
                   DataRow[] result = dt.Select("vault_transaction_type='Bulk Cash' and vaultID=" + dr[0]);
                   if (result.Length > 0)
                   {
                       for (int k = 0; k < result.Length; k++)
                       {
                           newRow[k + 3] = VaultLedger.parseValueIntoCurrency(decimal.Parse(result[k]["total_quantity"].ToString()));
                           total += int.Parse(result[k]["total_quantity"].ToString());
                           totalVal += int.Parse(result[k]["total_value"].ToString());
                       }
                       newRow["total"] = VaultLedger.parseValueIntoCurrency(decimal.Parse(total.ToString()));
                       newRow["Value"] = VaultLedger.parseValueIntoCurrency(decimal.Parse(totalVal.ToString()));
                       newRow["Title"] = result[0]["title"];
                       newRow["Date"] = DateTime.Parse(result[0]["ledger_date"].ToString()).ToString("dd/MM/yyyy");
                       isTitlePrinted = true;

                   }
                   dtGrid.Rows.Add(newRow);
                   dtGrid.Rows.Add(dtGrid.NewRow());

                   //Inbound...
                   total = 0;
                   totalVal = 0;
                   newRow = dtGrid.NewRow();
                   newRow["type"] = "Inbound (Residual)";
                   result = dt.Select("vault_transaction_type='Inbound' and vaultID=" + dr[0]);
                   if (result.Length > 0)
                   {
                       for (int k = 0; k < result.Length; k++)
                       {
                           newRow[k + 3] = VaultLedger.parseValueIntoCurrency(decimal.Parse(result[k]["total_quantity"].ToString()));
                           total += int.Parse(result[k]["total_quantity"].ToString());
                           totalVal += int.Parse(result[k]["total_value"].ToString());
                       }
                       newRow["total"] = VaultLedger.parseValueIntoCurrency(decimal.Parse(total.ToString()));
                       newRow["Value"] = VaultLedger.parseValueIntoCurrency(decimal.Parse(totalVal.ToString()));
                       if (isTitlePrinted)
                           newRow["Title"] = "";
                       else
                       {
                           newRow["Title"] = result[0]["title"];
                           isTitlePrinted = true;
                       }

                       newRow["Date"] = DateTime.Parse(result[0]["ledger_date"].ToString()).ToString("dd/MM/yyyy");
                   }
                   dtGrid.Rows.Add(newRow);


                   //Outbound...
                   total = 0;
                   totalVal = 0;
                   newRow = dtGrid.NewRow();
                   newRow["type"] = "Outbound (Replenishment)";
                   result = dt.Select("vault_transaction_type='Outbound' and vaultID=" + dr[0]);
                   if (result.Length > 0)
                   {
                       for (int k = 0; k < result.Length; k++)
                       {
                           newRow[k + 3] = VaultLedger.parseValueIntoCurrency(decimal.Parse(result[k]["total_quantity"].ToString()));
                           total += int.Parse(result[k]["total_quantity"].ToString());
                           totalVal += int.Parse(result[k]["total_value"].ToString());
                       }
                       newRow["total"] = VaultLedger.parseValueIntoCurrency(decimal.Parse(total.ToString()));
                       newRow["Value"] = VaultLedger.parseValueIntoCurrency(decimal.Parse(totalVal.ToString()));
                       if (isTitlePrinted)
                           newRow["Title"] = "";
                       else
                       {
                           newRow["Title"] = result[0]["title"];
                           isTitlePrinted = true;
                       }

                       newRow["Date"] = DateTime.Parse(result[0]["ledger_date"].ToString()).ToString("dd/MM/yyyy");
                   }
                   dtGrid.Rows.Add(newRow);

                   //
                   dtGrid.Rows.Add(dtGrid.NewRow());
                   newRow = dtGrid.NewRow();
                   newRow["type"] = "Net";
                   dtGrid.Rows.Add(newRow);
                   dtGrid.Rows.Add(dtGrid.NewRow());

                   total = 0;
                   totalVal = 0;
                   newRow = dtGrid.NewRow();
                   newRow["type"] = "Net Adjustments (+)";
                   result = dt.Select("vault_transaction_type='Adjustment Credit' and vaultID=" + dr[0]);
                   if (result.Length > 0)
                   {
                       for (int k = 0; k < result.Length; k++)
                       {
                           newRow[k + 3] = VaultLedger.parseValueIntoCurrency(decimal.Parse(result[k]["total_quantity"].ToString()));
                           total += int.Parse(result[k]["total_quantity"].ToString());
                           totalVal += int.Parse(result[k]["total_value"].ToString());
                       }
                       newRow["total"] = VaultLedger.parseValueIntoCurrency(decimal.Parse(total.ToString()));
                       newRow["Value"] = VaultLedger.parseValueIntoCurrency(decimal.Parse(totalVal.ToString()));
                       if (isTitlePrinted)
                           newRow["Title"] = "";
                       else
                       {
                           newRow["Title"] = result[0]["title"];
                           isTitlePrinted = true;
                       }

                       newRow["Date"] = DateTime.Parse(result[0]["ledger_date"].ToString()).ToString("dd/MM/yyyy");
                   }
                   dtGrid.Rows.Add(newRow);

                   total = 0;
                   totalVal = 0;
                   newRow = dtGrid.NewRow();
                   newRow["type"] = "Net Adjustments (-)";
                   result = dt.Select("vault_transaction_type='Adjustment Debit' and vaultID=" + dr[0]);
                   if (result.Length > 0)
                   {
                       for (int k = 0; k < result.Length; k++)
                       {
                           newRow[k + 3] = VaultLedger.parseValueIntoCurrency(decimal.Parse(result[k]["total_quantity"].ToString()));
                           total += int.Parse(result[k]["total_quantity"].ToString());
                           totalVal += int.Parse(result[k]["total_value"].ToString());
                       }
                       newRow["total"] = VaultLedger.parseValueIntoCurrency(decimal.Parse(total.ToString()));
                       newRow["Value"] = VaultLedger.parseValueIntoCurrency(decimal.Parse(totalVal.ToString()));
                       if (isTitlePrinted)
                           newRow["Title"] = "";
                       else
                       {
                           newRow["Title"] = result[0]["title"];
                           isTitlePrinted = true;
                       }

                       newRow["Date"] = DateTime.Parse(result[0]["ledger_date"].ToString()).ToString("dd/MM/yyyy");
                   }
                   dtGrid.Rows.Add(newRow);


                   dtGrid.Rows.Add(dtGrid.NewRow());
                   //Inbound...
                   total = 0;
                   totalVal = 0;
                   newRow = dtGrid.NewRow();
                   newRow["type"] = "Total Available";
                   result = dt.Select("vault_transaction_type='Total' and vaultID=" + dr[0]);
                   if (result.Length > 0)
                   {
                       for (int k = 0; k < result.Length; k++)
                       {
                           newRow[k + 3] = VaultLedger.parseValueIntoCurrency(decimal.Parse(result[k]["total_quantity"].ToString()));
                           total += int.Parse(result[k]["total_quantity"].ToString());
                           totalVal += int.Parse(result[k]["total_value"].ToString());
                       }
                       newRow["total"] = VaultLedger.parseValueIntoCurrency(decimal.Parse(total.ToString()));
                       newRow["Value"] = VaultLedger.parseValueIntoCurrency(decimal.Parse(totalVal.ToString()));
                       if (isTitlePrinted)
                           newRow["Title"] = "";
                       else
                       {
                           newRow["Title"] = result[0]["title"];
                           isTitlePrinted = true;
                       }

                       newRow["Date"] = DateTime.Parse(result[0]["ledger_date"].ToString()).ToString("dd/MM/yyyy");
                   }
                   dtGrid.Rows.Add(newRow);

                   //newRow = dtGrid.NewRow();
                   //newRow["type"] = "Total Available";
                   //for (int i = dt.Rows.Count - 4; i < dt.Rows.Count ; i++)
                   //{
                   //    newRow[i + 1] = dt.Rows[i]["total_quantity"];
                   //    total += int.Parse(dt.Rows[i]["total_quantity"].ToString());
                   //}
                   //newRow["total"] = total;
                   //dtGrid.Rows.Add(newRow);
                   //Literal_summary.Text = dt.Rows.Count + " records.";
               }
                this.GridView_VaultBalanceInvestigation.DataSource = dtGrid;
                this.GridView_VaultBalanceInvestigation.DataBind();
                //GridView1.DataSource = dtGrid;
                //GridView1.DataBind();

            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_errorMsg.Text = "<li>" + ex.Message + "</li>";
            }
            finally
            {
                task.EndTask();
            }
        }
        protected void GridView_VaultBalanceInvestigation_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.Cells[2].Text.Contains("Inbound"))
                {
                    e.Row.Cells[2].ForeColor = System.Drawing.Color.White;
                    e.Row.Cells[2].BackColor = System.Drawing.ColorTranslator.FromHtml("#BF4b4f");
                }
                else if (e.Row.Cells[2].Text.Contains("Outbound"))
                {
                    e.Row.Cells[2].ForeColor = System.Drawing.Color.White;
                    e.Row.Cells[2].BackColor = System.Drawing.ColorTranslator.FromHtml("#527fb8");
                }
                else if (e.Row.Cells[2].Text == "Net")
                {
                    e.Row.Cells[2].ForeColor = System.Drawing.Color.White;
                    e.Row.Cells[2].BackColor = System.Drawing.ColorTranslator.FromHtml("#8766a5");
                }
                else if (e.Row.Cells[2].Text.Contains("Total"))
                {
                    e.Row.Cells[2].Style.Add("font-weight", "bold");
                    e.Row.Cells[2].ForeColor = System.Drawing.Color.White;
                    e.Row.Cells[2].BackColor = System.Drawing.ColorTranslator.FromHtml("#FCb441");
                }


            }
        }
    }
}