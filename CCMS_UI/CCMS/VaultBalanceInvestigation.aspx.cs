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

namespace CCMSUI.CCMS
{
    public partial class VaultBalanceInvestigation : BasePage
    {
        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (!CheckRight((int)Permissions.VaultBalanceInvestigation))
                    Response.Redirect("~/admin/AccessDenied.aspx");

                ViewState["sortDirection"] = " desc";
                ViewState["SortExpression"] = " ledger_date";

                TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");

                BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());


            }
        }
        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            if (!((bool)Session[SessionVars.isRegionSelected.ToString()] || Session[SessionVars.SelectedATMs.ToString()].ToString().IndexOf(',') > -1 ||
                     Session[SessionVars.SelectedATMs.ToString()].ToString().Length == 0))
            {
                //TextBox_ip.Text = Atm.LoadAtm("atm_id =" + Session[SessionVars.SelectedATMs.ToString()].ToString()).IP;
                Button_search_Click(this, null);
            }
            else
            {
                //TextBox_ip.Text = "";
            }
            Button_search_Click(null, null);


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


        string Filter()
        {
            StringBuilder builder = new StringBuilder();

            if (TextBox_FromDate.Text.Length > 0)
                builder.Append(" and ledger_date >= Convert(datetime,'" + TextBox_FromDate.Text + "',103) ");
            if (TextBox_ToDate.Text.Length > 0)
                builder.Append(" and ledger_date <= Convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103) ");

            return builder.ToString();
        }

        private void BindGrid(string orderby)
        {
            LogableTask task = LogableTask.NewTask("BindGrid");
            SqlCommand cmd = null;
            DataTable dt = null;
            bool isTitlePrinted = false;
            try
            {
                DataTable dtGrid = new DataTable();
                dtGrid.Columns.Add("Title");
                dtGrid.Columns.Add("Date");
                dtGrid.Columns.Add("Type");
                cmd = ConnectionFactory.GetNewCommand(false);
                StringBuilder builder = new StringBuilder();
                cmd.CommandText = @"select title,ledger_date,vault_transaction_type,transaction_type,denomination_name,total_quantity,total_value,CCMS_vault.id vaultID
                                    from CCMS_vault_day_balance inner join CCMS_vault on CCMS_vault.id = CCMS_vault_day_balance.vault_id where 1=1 " + Filter() + " order by " + orderby;
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
                    DataRow[] dataRowVault = dt.Select("vaultID="+dr[0]);
                    DataTable dtTemp = dataRowVault.CopyToDataTable();
                    DataTable dtLedgerDates= dtTemp.DefaultView.ToTable(true, new string[] { "ledger_date" });

                    foreach (DataRow dataRow in dtLedgerDates.Rows)
                    {
                        //Inbound...
                        int total = 0;
                        int totalVal = 0;
                        DataRow newRow = dtGrid.NewRow();
                        newRow["type"] = "Inbound Bulk";
                        DataRow[] result = dt.Select("vault_transaction_type='Bulk Cash' and ledger_date =#" + dataRow["ledger_date"] + "# and vaultID="+dr[0]);
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
                            newRow["Date"] = result[0]["ledger_date"];
                            isTitlePrinted = true;

                        }
                        dtGrid.Rows.Add(newRow);
                        dtGrid.Rows.Add(dtGrid.NewRow());

                        //Inbound...
                        total = 0;
                        totalVal = 0;
                        newRow = dtGrid.NewRow();
                        newRow["type"] = "Inbound (Residual)";
                        result = dt.Select("vault_transaction_type='Inbound' and ledger_date =#" + dataRow["ledger_date"] + "# and vaultID=" + dr[0]);
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
                            {
                                newRow["Title"] = "";
                                newRow["Date"] = "";
                            }
                            else
                            {
                                newRow["Title"] = result[0]["title"];
                                newRow["Date"] = result[0]["ledger_date"];
                                isTitlePrinted = true;
                            }

                        }
                        dtGrid.Rows.Add(newRow);


                        //Outbound...
                        total = 0;
                        totalVal = 0;
                        newRow = dtGrid.NewRow();
                        newRow["type"] = "Outbound (Replenishment)";
                        result = dt.Select("vault_transaction_type='Outbound' and ledger_date =#" + dataRow["ledger_date"] + "# and vaultID=" + dr[0]);
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
                            {
                                newRow["Title"] = "";
                                newRow["Date"] = "";
                            }
                            else
                            {
                                newRow["Title"] = result[0]["title"];
                                newRow["Date"] = result[0]["ledger_date"];
                                isTitlePrinted = true;
                            }

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
                        result = dt.Select("vault_transaction_type='Adjustment Credit' and ledger_date =#" + dataRow["ledger_date"] + "# and vaultID=" + dr[0]);
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
                            {
                                newRow["Title"] = "";
                                newRow["Date"] = "";
                            }
                            else
                            {
                                newRow["Title"] = result[0]["title"];
                                newRow["Date"] = result[0]["ledger_date"];
                                isTitlePrinted = true;
                            }

                        }
                        dtGrid.Rows.Add(newRow);

                        total = 0;
                        totalVal = 0;
                        newRow = dtGrid.NewRow();
                        newRow["type"] = "Net Adjustments (-)";
                        result = dt.Select("vault_transaction_type='Adjustment Debit' and ledger_date =#" + dataRow["ledger_date"] + "# and vaultID=" + dr[0]);
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
                            {
                                newRow["Title"] = "";
                                newRow["Date"] = "";
                            }
                            else
                            {
                                newRow["Title"] = result[0]["title"];
                                newRow["Date"] = result[0]["ledger_date"];
                                isTitlePrinted = true;
                            }

                        }
                        dtGrid.Rows.Add(newRow);


                        dtGrid.Rows.Add(dtGrid.NewRow());
                        //Inbound...
                        total = 0;
                        totalVal = 0;
                        newRow = dtGrid.NewRow();
                        newRow["type"] = "Total Available";
                        result = dt.Select("vault_transaction_type='Total' and ledger_date =#" + dataRow["ledger_date"] + "# and vaultID=" + dr[0]);
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
                            {
                                newRow["Title"] = "";
                                newRow["Date"] = "";
                            }
                            else
                            {
                                newRow["Title"] = result[0]["title"];
                                newRow["Date"] = result[0]["ledger_date"];
                                isTitlePrinted = true;
                            }

                        }
                        dtGrid.Rows.Add(newRow);
                    }
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
                this.GridView_VaultBalanceInvestigation.DataSource = dtGrid;
                this.GridView_VaultBalanceInvestigation.DataBind();

            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_error.Text = "<li>" + ex.Message + "</li>";
            }
            finally
            {
                task.EndTask();
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
                else if (e.Row.Cells[2].Text=="Net")
                {
                    e.Row.Cells[2].ForeColor = System.Drawing.Color.White;
                    e.Row.Cells[2].BackColor = System.Drawing.ColorTranslator.FromHtml("#8766a5");
                }


            }
        }


    }
}