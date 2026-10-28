using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Avanza.iSuite.UI;
using Avanza.CCMS.DAL;
using System.Reflection;
using System.Diagnostics;
using System.Data.SqlClient;

using System.Data;

namespace CCMSUI.CCMS
{
    public partial class BankSettlementReport : BasePage
    {


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (!CheckRight((int)Permissions.ViewBankSettlementReport))
                    Response.Redirect("~/admin/AccessDenied.aspx");

                //if (WebConfigurationManager.AppSettings["isScheduleCreationEnabled"] == "0")
                //    Button_GenerateReportSchedule.Enabled = false;

                TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                //TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                DropDownList_Organization.Items.Add(new ListItem("Select", "Select"));
                Region.RegionReader OrganizationReader = Region.ExecuteReader("region_id > 1 and is_organization=1");
                while (OrganizationReader.Read())
                    DropDownList_Organization.Items.Add(new ListItem(OrganizationReader.CurrentRegion.RegionName, OrganizationReader.CurrentRegion.RegionId.ToString()));
                OrganizationReader.Close();

            }

        }
        string Filter()
        {
            string filter = "";

            if (TextBox_FromDate.Text.Length > 0)
                filter += " and report_date >= convert(datetime, '" + TextBox_FromDate.Text + "', 103) and report_date <= convert(datetime, '" + TextBox_FromDate.Text + " 23:59:59', 103)";

            //if (TextBox_ToDate.Text.Length > 0)
            //    filter += " and report_date <= convert(datetime, '" + TextBox_ToDate.Text + " 23:59:59', 103) ";

            filter += " and atm.is_active =1 and atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and atm_id in (select atm_id from vault_atm where vault_id =" + DropDownList_Vault.SelectedItem.Value + ")";

            return filter;
        }



        protected void DropDownList_Organization_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (DropDownList_Organization.SelectedIndex == 0)
            {
                Literal_error.Text = "<li>Please select Organization</li>";
                return;
            }


            DropDownList_CIT.Items.Clear();
            DropDownList_CIT.Items.Add(new ListItem("Select", "Select"));
            RegionCit.RegionCitReader RegionCitReader = RegionCit.ExecuteReader("region_id=" + DropDownList_Organization.SelectedItem.Value);
            while (RegionCitReader.Read())
            {
                CcmsCit ccmsCIT = CcmsCit.LoadCcmsCitByPk(RegionCitReader.CurrentRegionCit.CitId);
                DropDownList_CIT.Items.Add(new ListItem(ccmsCIT.Name, ccmsCIT.Id.ToString()));
            }
            RegionCitReader.Close();


        }
        protected void DropDownList_CIT_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList_Vault.Items.Clear();
            DropDownList_Vault.Items.Add(new ListItem("Select", "Select"));

            CcmsVault.CcmsVaultReader reader = CcmsVault.ExecuteReader("cit_id = " + DropDownList_CIT.SelectedItem.Value + "  and is_deleted=0");
            while (reader.Read())
            {
                DropDownList_Vault.Items.Add(new ListItem(reader.CurrentCcmsVault.Title, reader.CurrentCcmsVault.Id.ToString()));
            }
            reader.Close();

        }

        protected void Button_search_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Search");
            try
            {
                string URL = null;
                DataRow newRow2 = null;
                int atmSettlementId = 0;
                int atmSettlementInfoId = 0;
                string reportDate = "";
                decimal creditAmount = 0;
                decimal debitAmount = 0;
                decimal cashRecvFromBank = 0;
                decimal cashRetToBank = 0;
                DSBankSettlement Ds = new DSBankSettlement();
                AppUser objUser = (AppUser)Session[SessionVars.user.ToString()];
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 100;
                if (System.Configuration.ConfigurationManager.AppSettings["isRunningForNasserBank"] == "1")                    
                cmd.CommandText = @"
                                    select title,cit_atm_title,atm_settlement_id,rep_datetime,total_rejected,report_date,atm_site_and_number,total_replenished,atm_settlement_info.atm_settlement_info_id
                                    from atm_settlement inner join atm_settlement_info on atm_settlement.atm_settlement_info_id = atm_settlement_info.atm_settlement_info_id
                                    inner join atm on atm.cit_atm_title = REPLACE(STR(atm_settlement.atm_no, 8), SPACE(1), '0')
                                    " + Filter();
                else
                    cmd.CommandText = @"
                                    select title,cit_atm_title,atm_settlement_id,rep_datetime,total_rejected,report_date,atm_site_and_number,total_replenished,atm_settlement_info.atm_settlement_info_id
                                    from atm_settlement inner join atm_settlement_info on atm_settlement.atm_settlement_info_id = atm_settlement_info.atm_settlement_info_id
                                    inner join atm on atm.cit_atm_title = atm_settlement.atm_no " + Filter();

                DataTable dt = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Rows returned");

                    reportDate = DateTime.Parse(dt.Rows[0]["report_date"].ToString()).ToString("dd-MMM-yy");
                    atmSettlementInfoId = int.Parse(dt.Rows[0]["atm_settlement_info_id"].ToString());
                    atmSettlementId = int.Parse(dt.Rows[0]["atm_settlement_id"].ToString());

                    cmd.CommandText = @"select cash_recieved_from_bank,unfit_notes_delivered_to_bank 
                                        from vault_settlement_detail inner join vault_settlement on vault_settlement.vault_settlement_id = vault_settlement_detail.vault_settlement_id
                                        where len(denomination_name)=0 
                                        and atm_settlement_info_id = " + dt.Rows[0]["atm_settlement_info_id"];
                    DataTable dtVaultSettlementDetail = new DataTable();
                    adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dtVaultSettlementDetail);

                    if (dtVaultSettlementDetail.Rows.Count > 0)
                    {
                        cashRecvFromBank = decimal.Parse(dtVaultSettlementDetail.Rows[0]["cash_recieved_from_bank"].ToString());
                        cashRetToBank = decimal.Parse(dtVaultSettlementDetail.Rows[0]["unfit_notes_delivered_to_bank"].ToString());
                    }
                }
                foreach (DataRow dr in dt.Rows)
                {

                    newRow2 = null;
                    cmd.CommandText = "select * from ccms_vault_ledger where atm_settlement_id = " + dr["atm_settlement_id"];
                    DataTable dt1 = new DataTable();
                    adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt1);
                    if (dt1.Rows.Count > 0)
                    {
                        foreach (DataRow dr1 in dt1.Rows)
                        {
                            if (dr1["transaction_type"].ToString() == "Cr")
                                creditAmount = decimal.Parse(dr1["ledger_amount"].ToString());
                            else
                                debitAmount = decimal.Parse(dr1["ledger_amount"].ToString());
                        }

                        DataRow newRow = Ds.Tables[0].NewRow();
                        newRow["atm"] = dr["atm_site_and_number"];
                        newRow["rep_datetime"] = dr["rep_datetime"];
                        newRow["cit_atm_title"] = dr["cit_atm_title"];
                        newRow["cash_replenished"] = decimal.Parse(dr["Total_Replenished"].ToString()); //debitAmount;
                        newRow["cash_returned"] = decimal.Parse(dr["Total_Rejected"].ToString());//creditAmount;

                        newRow["variance"] = decimal.Parse(dr["Total_Rejected"].ToString()) - creditAmount;


                        //Reading other details.
                        cmd.CommandText = "select * from atm_settlement_other_details where atm_settlement_id = " + dr["atm_settlement_id"];
                        dt1 = new DataTable();
                        adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(dt1);

                        if (dt1.Rows.Count > 0)
                        {
                            task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "other details found for atm_settlement_id = " + dr["atm_settlement_id"]);
                            newRow["inventory"] = dt1.Rows[0]["inventory"];
                            newRow["comments"] = dt1.Rows[0]["comments"];
                            newRow["no_of_replenishments"] = dt1.Rows[0]["no_of_replenishment"];

                            if (newRow["inventory"].ToString().Length > 0)
                            {
                                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "inventory found so creating another row");


                                newRow2 = Ds.Tables[0].NewRow();
                                newRow2["atm"] = dr["atm_site_and_number"];
                                newRow2["cash_replenished"] = newRow["cash_replenished"];
                                newRow2["cash_returned"] = 0;
                                newRow2["variance"] = 0;

                                newRow["cash_replenished"] = 0;
                            }
                        }
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "row added to table");

                        Ds.Tables[0].Rows.Add(newRow);
                        if (newRow2 != null)
                        {
                            Ds.Tables[0].Rows.Add(newRow2);
                            task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "new row added to table");

                        }
                    }
                    else
                        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "no ledger found for atm_settlement_id = " + dr["atm_settlement_id"]);

                }


                if (Ds.Tables["DataTable1"].Rows.Count > 0)
                {
                    //computing opening balance...
                    string title = "";
                    decimal openingBalance = 0;
                    cmd.CommandText = "select closing_balance,denomination_name from ccms_vault_day_balance_summary where vault_id =" + DropDownList_Vault.SelectedItem.Value + " and  transaction_date = (select max(transaction_date) from ccms_vault_day_balance_summary where transaction_date < convert(datetime,'" + TextBox_FromDate.Text + "',103))";
                    adapter = new SqlDataAdapter(cmd);
                    dt = new DataTable();
                    adapter.Fill(dt);
                    foreach (DataRow dr in dt.Rows)
                    {
                        openingBalance += decimal.Parse(dr["closing_balance"].ToString()) * int.Parse(dr["denomination_name"].ToString().Substring(3));
                    }
                    //
                    Session[SessionVars.dtReportDataSet.ToString()] = Ds;

                    if (System.Configuration.ConfigurationManager.AppSettings["isRunningForNasserBank"] == "0")
                    {
                        if (DropDownList_CIT.SelectedItem.Text.ToLower().Contains("speed"))
                            title = "Speed service";
                        else //if (DropDownList_CIT.SelectedItem.Text.ToLower().Contains("speed"))
                            title = "Falcon";

                        URL = "ReportPopup.aspx?Report=BankSettlementsDR.rpt&ReportTitle=ATM Summary Report&balance=" + openingBalance + "&rDate=" + reportDate + "&total=" + Ds.Tables[0].Rows.Count + "&cashRecv=" + cashRecvFromBank + "&cashRet=0" + "&title=" + title;
                    }
                    else
                    {
                        URL = "ReportPopup.aspx?Report=NasserBankSettlementsDR.rpt&ReportTitle=ATM Summary Report&balance=" + openingBalance + "&rDate=" + reportDate + "&total=" + Ds.Tables[0].Rows.Count + "&cashRecv=" + cashRecvFromBank + "&cashRet=0" + "&title=" + title;
                    }
                        Literal_ok.Text = "<script>window.open(\"" + URL + "\");</script>";
                }
                else
                    Literal_ok.Text = "<li>No record(s) found</li>";
            }

            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_error.Text = "<li> An error occurred while generating Report::" + ex.Message;
            }
            finally
            {
                task.EndTask();
            }
        }

        protected void Button_reset_Click(object sender, EventArgs e)
        {
            TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            // TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");

        }


        protected void Button_GenerateReportSchedule_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Button_GenerateReportSchedule_Click");
            ReportSchedule reportSchedule = null;
            SqlCommand cmd = null;
            SqlTransaction trxn = null;
            try
            {
                if (Session["isOrgSelected"] != null)
                {
                    if (bool.Parse(Session["isOrgSelected"].ToString()))
                    {
                        cmd = ConnectionFactory.GetNewCommand(true);
                        trxn = cmd.Connection.BeginTransaction();


                        string orgID = Session["selectedOrgID"].ToString();
                        ConnectionFactory.ExecuteScalar(@"delete report_generation_schedule
                                                          where report_schedule_id in
                                                          (select  report_schedule_id 
                                                          from report_schedule 
                                                          where report_name='BankSettlementReport' and organization_id = " + orgID + "); " +

                                                         " delete report_schedule where report_name='BankSettlementReport' and organization_id = " + orgID, trxn);

                        reportSchedule = new ReportSchedule();
                        reportSchedule.ReportName = "BankSettlementReport";
                        reportSchedule.ReportPhysicalPath = Server.MapPath(@"BankSettlementsDR.rpt");
                        reportSchedule.ReportReceipients = "";
                        AppSetting appSetting = (AppSetting)Application[ApplicationVars.AppSettings.ToString()];
                        if (appSetting != null)
                            reportSchedule.ReportTempPath = appSetting.TemporaryFolder;
                        else
                            reportSchedule.ReportTempPath = @"c:\view360LiveData\temp\";
                        reportSchedule.RetryCount = 5;
                        reportSchedule.ReportNextGeneratedAt = DateTime.Today;
                        reportSchedule.ReportFriendlyName = "Bank Settlement Report";
                        reportSchedule.MinutesToScheduleAgain = 1440;
                        reportSchedule.ReportExportType = 3;
                        reportSchedule.ReportDataAge = 1;
                        reportSchedule.ScheduleType = false;
                        reportSchedule.OrganizationId = int.Parse(orgID);
                        reportSchedule.IsEjEnabled = false;
                        reportSchedule.Save(trxn.Connection, trxn);
                        trxn.Commit();
                        Literal_ok.Text = "Schedule added successfully";
                        Literal_error.Text = "";
                        //
                        Utility.BuildAuditLog("Schedule added for Bank Settlement Report", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.ViewBankSettlementReport);
                    }
                    else
                        Literal_error.Text = "Please select organization from treeview.";
                }
                else
                    Literal_error.Text = "Please select organization from treeview.";
            }

            catch (Exception ex)
            {
                if (trxn != null)
                    trxn.Rollback();
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, ex);

            }
            finally
            {
                task.EndTask();
                if (cmd != null)
                    if (cmd.Connection != null)
                        cmd.Connection.Close();
            }
        }

    }
}