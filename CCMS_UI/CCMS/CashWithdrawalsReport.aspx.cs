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
using System.Reflection;
using System.Diagnostics;
using Avanza.CCMS.DAL;

using System.Data.SqlClient;
using System.Web.Configuration;
using System.Text;

namespace CCMSUI.CCMS
{
    public partial class CashWithdrawalsReport : BasePage
    {
        private void BindNoteSetType()
        {
            string regionID = (string)Session[SessionVars.SelectedRegions.ToString()];
            if (regionID != null /*&& !regionID.Contains(",")*/
                && regionID != "1")
            {
                var dt = new DataTable();
                dt.Columns.Add("note_set_type_name");
                dt.Columns.Add("region_id");
                Avanza.CCMS.DAL.NoteSetType.NoteSetTypeReader reader =
                Avanza.CCMS.DAL.NoteSetType.ExecuteReader("region_id=" + AddEditATM.GetOrganization(regionID.Contains(",") ? int.Parse(regionID.Substring(0, regionID.IndexOf(","))) : int.Parse(regionID)));
                while (reader.Read())
                {
                    // DropDownList_NoteSetTypeID.Items.Add(new ListItem(reader.CurrentNoteSetType.NoteSetTypeName, reader.CurrentNoteSetType.NoteSetTypeId.ToString()));
                    dt.Rows.Add(reader.CurrentNoteSetType.NoteSetTypeName, reader.CurrentNoteSetType.NoteSetTypeId.ToString());

                }

                if (dt.Rows.Count > 0)
                {
                    groupList.DataSource = dt;
                    groupList.DataTextField = "note_set_type_name";
                    groupList.DataValueField = "region_id";
                    groupList.DataBind();
                }

                reader.Close();
            }
        }
        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            BindNoteSetType();
        }
        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Page_Load");
            try
            {
                if (!Page.IsPostBack)
                {
                    if (!CheckRight((int)Permissions.CashWithdrawalReport))
                        Response.Redirect("../admin/AccessDenied.aspx");

                    if (WebConfigurationManager.AppSettings["isScheduleCreationEnabled"] == "0")
                        Button_GenerateReportSchedule.Enabled = false;

                    TextBox_FromDate.Text = DateTime.Today.ToString("dd/MM/yyyy HH:mm:ss");
                    TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

                    if (Request.QueryString["ej"] == "1" || Request.QueryString["ej"] == "2")
                    {
                        TextBox_purgeFrom.Enabled = false;
                        TextBox_purge_to.Enabled = false;

                        if (Request.QueryString["ej"] == "2")
                            reportType.Visible = false;
                    }
                    BindNoteSetType();
                }
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            {
                task.EndTask();
            }
        }


        private string buildSearchCriteria()
        {
            StringBuilder groupbuilder = new StringBuilder();
            foreach (ListItem item in groupList.Items)
            {
                if (item.Selected)
                {
                    if (groupbuilder.Length > 0)
                        groupbuilder.Append(",");
                    groupbuilder.Append(item.Value);
                }

            }
            string filter = "";

            if (groupbuilder.Length > 0)
                filter = " and atm.note_set_type_id  in ( " + groupbuilder.ToString() + " ) ";


            if (TextBox_FromDate.Text.Length > 0)
                filter += " and trxn_datetime >= convert(datetime,'" + TextBox_FromDate.Text + "',103)";

            if (TextBox_ToDate.Text.Length > 0)
                filter += " and trxn_datetime <= convert(datetime,'" + TextBox_ToDate.Text + "',103)";

            if (WebConfigurationManager.AppSettings["isDeadATMExcluded"] == "1")
                filter += " and atm.atm_id in (select ATM_id from heart_beat where heart_beat_received_at >=convert(datetime,'" + TextBox_FromDate.Text + "',103) and " +
                                     "heart_beat_received_at <=convert(datetime,'" + TextBox_ToDate.Text + "',103))";
   
            if (RadioButtonList_Withdrawals.SelectedIndex == 0)
            {
                if (Request.QueryString["ej"] == "1")
                {
                    if (TextBox_Dispensed1.Text.Length > 0)
                        filter += " and notes_dispensed_type1 = " + int.Parse(TextBox_Dispensed1.Text);

                    if (TextBox_Dispensed2.Text.Length > 0)
                        filter += " and notes_dispensed_type2 = " + int.Parse(TextBox_Dispensed2.Text);

                    if (TextBox_Dispensed3.Text.Length > 0)
                        filter += " and notes_dispensed_type3 = " + int.Parse(TextBox_Dispensed3.Text);

                    if (TextBox_Dispensed4.Text.Length > 0)
                        filter += " and notes_dispensed_type4 = " + int.Parse(TextBox_Dispensed4.Text);
                }
                else
                {
                    if (TextBox_purgeFrom.Text.Length > 0)
                        filter += " and (cash_purged1+cash_purged2+cash_purged3+cash_purged4+cash_purged5+cash_purged6+cash_purged7)>=" + int.Parse(TextBox_purgeFrom.Text);

                    if (TextBox_purge_to.Text.Length > 0)
                        filter += " and (cash_purged1+cash_purged2+cash_purged3+cash_purged4+cash_purged5+cash_purged6+cash_purged7)<=" + int.Parse(TextBox_purge_to.Text);



                    if (TextBox_Dispensed1.Text.Length > 0)
                        filter += " and cash_dispensed1 = " + int.Parse(TextBox_Dispensed1.Text);

                    if (TextBox_Dispensed2.Text.Length > 0)
                        filter += " and cash_dispensed2 = " + int.Parse(TextBox_Dispensed2.Text);

                    if (TextBox_Dispensed3.Text.Length > 0)
                        filter += " and cash_dispensed3 = " + int.Parse(TextBox_Dispensed3.Text);

                    if (TextBox_Dispensed4.Text.Length > 0)
                        filter += " and cash_dispensed4 = " + int.Parse(TextBox_Dispensed4.Text);
                }


                if (TextBox_FromAmount.Text.Length > 0)
                    filter += " and amount >= " + TextBox_FromAmount.Text;

                if (TextBox_ToAmount.Text.Length > 0)
                    filter += " and amount <= " + TextBox_ToAmount.Text;
            }
            return filter;
        }
        private void PopulateDataTable(DataTable dt)
        {
            int region_id = 0;
            int id = -1;

            DataTable dtRegions = new DataTable();
            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            cmd.CommandText = "select region_id, parent_region_id, is_organization, bank_logo from region where parent_region_id is not null";
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dtRegions);



            for (int i = 0; i < dt.Rows.Count; i++)
            {
                region_id = int.Parse(dt.Rows[i][3].ToString()); //parent

                while (true)
                {
                    DataRow[] innerArray = dtRegions.Select("region_id=" + region_id);
                    if (bool.Parse(innerArray[0][2].ToString()))
                    {
                        id = int.Parse(innerArray[0][0].ToString());
                        break;
                    }
                    else
                        region_id = int.Parse(innerArray[0][1].ToString());

                }
                //organization id found
                Region region = Region.LoadRegionByPk(id);
                dt.Rows[i][1] = region.RegionName;
                //       dt.Rows[i][4] = region.BankLogo;

            }

        }

        protected void Button_GenerateReportSchedule_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Button_GenerateReportSchedule_Click");
            ReportSchedule reportSchedule = null;
            SqlCommand cmd = null;
            SqlTransaction trxn = null;
            string reportName = "";
            string friendlyName = "";
            string rptName = "";
            int ejEnabled = 0;

            if (RadioButtonList_Withdrawals.SelectedIndex == 0)
            {
                if (reportType.Visible)
                {
                    reportName = "CashWithdrawalsReport";
                    friendlyName = "Cash Withdrawals Report";
                    rptName = "CashWithdrawals.rpt";
                }
                else
                {
                    reportName = "FraudTransactionsReport";
                    friendlyName = "Exception Handling Report";
                    rptName = "CashWithdrawals.rpt";
                }

            }

            else
            {
                reportName = "CashWithdrawalsSummaryReport";
                friendlyName = "Cash Withdrawals Summary Report";
                rptName = "CashWithdrawalSummary.rpt";
            }
            try
            {
                if (Request.QueryString["ej"] == "1")
                    ejEnabled = 1;
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
                                                          where report_name='" + reportName + "' and organization_id = " + orgID + "); " +

                                                         " delete report_schedule where report_name='" + reportName + "' and is_ej_enabled=" + ejEnabled + " and organization_id = " + orgID, trxn);

                        reportSchedule = new ReportSchedule();
                        reportSchedule.ReportName = reportName;
                        reportSchedule.ReportPhysicalPath = Server.MapPath(rptName);
                        reportSchedule.ReportReceipients = "";
                        AppSetting appSetting = (AppSetting)Application[ApplicationVars.AppSettings.ToString()];
                        if (appSetting != null)
                            reportSchedule.ReportTempPath = appSetting.TemporaryFolder;
                        else
                            reportSchedule.ReportTempPath = @"c:\view360LiveData\temp\";
                        reportSchedule.RetryCount = 5;
                        reportSchedule.ReportNextGeneratedAt = DateTime.Today;
                        reportSchedule.ReportFriendlyName = friendlyName;
                        reportSchedule.MinutesToScheduleAgain = 1440;
                        reportSchedule.ReportExportType = 3;
                        reportSchedule.ReportDataAge = 1;
                        reportSchedule.ScheduleType = false;
                        reportSchedule.OrganizationId = int.Parse(orgID);
                        if (ejEnabled == 1)
                            reportSchedule.IsEjEnabled = true;
                        else
                            reportSchedule.IsEjEnabled = false;
                        reportSchedule.ApplicableNoteSetType = GetNoteSetTypeId();
                        if (reportSchedule.ApplicableNoteSetType == "-1")
                            reportSchedule.ApplicableNoteSetType = null;                                            

                        reportSchedule.Save(trxn.Connection, trxn);
                        trxn.Commit();
                        Literal_ok.Text = "Schedule added successfully";
                        Literal_errorMsg.Text = "";
                        Utility.BuildAuditLog("Schedule added for Cash Withdrawal Report", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.CashWithdrawalReport);

                    }
                    else
                        Literal_errorMsg.Text = "Please select organization from treeview.";
                }
                else
                    Literal_errorMsg.Text = "Please select organization from treeview.";
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
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Search");
            try
            {

                ValidateInputs();
                

                    DataSet Ds = null;
                    //dsCashWithdrawalSummary Ds1 = null; 

                    AppUser objUser = (AppUser)Session[SessionVars.user.ToString()];
                    SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                    cmd.CommandTimeout = 1000;
                    if (RadioButtonList_Withdrawals.SelectedIndex == 0 || RadioButtonList_Withdrawals.SelectedIndex == 2)
                    {
                        // Data Archiving Work
                        if (Request.QueryString["ej"] == "2")
                        {
                            cmd.CommandText = @"select region.region_name,region.region_id, parent_region_id, title,trxn_datetime,
notes_dispensed_type1 cash_dispensed1, notes_dispensed_type2 cash_dispensed2,notes_dispensed_type3 cash_dispensed3,
notes_dispensed_type4 cash_dispensed4,notes_dispensed_type5 cash_dispensed5,notes_dispensed_type6 cash_dispensed6,
notes_dispensed_type7 cash_dispensed7,amount,
notes_remaining_type1 cash_remaining1, notes_remaining_type2 cash_remaining2,notes_remaining_type3 cash_remaining3,notes_remaining_type4 cash_remaining4,atm.location
                        from captured_transactions inner join  vEjParsedTransactions
on captured_transactions.transaction_id = vEjParsedTransactions.ej_parsed_transactions_id
inner join atm on vEjParsedTransactions.atm_id = atm.atm_id 
inner join region on atm.region_id = region.region_id where 
 + ATM.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") " + buildSearchCriteria() + " order by trxn_datetime desc";
                        }
                        else if (Request.QueryString["ej"] == "1")
                        {
                            cmd.CommandText = @"select region.region_name,region.region_id, parent_region_id, title,trxn_datetime,
notes_dispensed_type1, notes_dispensed_type2,notes_dispensed_type3,notes_dispensed_type4,transaction_type_name,
tsn, pan, case when vEjParsedTransactions.status =0 then 'Successful' 
			        when vEjParsedTransactions.status=1 then 'Failed'
			        when vEjParsedTransactions.status =2 then 'Suspicious' end status,
notes_dispensed_type1 cash_dispensed1, notes_dispensed_type2 cash_dispensed2,notes_dispensed_type3 cash_dispensed3,
notes_dispensed_type4 cash_dispensed4,notes_dispensed_type5 cash_dispensed5,notes_dispensed_type6 cash_dispensed6,
notes_dispensed_type7 cash_dispensed7,amount,
notes_remaining_type1 cash_remaining1, notes_remaining_type2 cash_remaining2,notes_remaining_type3 cash_remaining3,notes_remaining_type4 cash_remaining4,atm.location,
n.denomination_type_1_title, n.denomination_type_2_title, n.denomination_type_3_title, n.denomination_type_4_title
                        from atm, vEjParsedTransactions, region, note_set_type n, transaction_type 
where atm.atm_id = vEjParsedTransactions.atm_id and atm.note_set_type_id = n.note_set_type_id 
and atm.region_id = region.region_id 
and transaction_type.transaction_type_id =vEjParsedTransactions.transaction_type_id
and ATM.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and amount is not null " + buildSearchCriteria() + " order by trxn_datetime desc";
                        }
                        else
                            cmd.CommandText = @"select ( isnull((select region_name + '-' from region where region_id = r.parent_region_id and is_organization=0),'') + r.region_name) region_name,r.region_id, r.parent_region_id, title,trxn_datetime,
cash_dispensed1, cash_dispensed2,cash_dispensed3,cash_dispensed4,cash_dispensed5,cash_dispensed6,cash_dispensed7,amount,
cash_remaining1,cash_remaining2,cash_remaining3,cash_remaining4,atm.location
                        from atm, vParsedTransaction, region r where atm.atm_id = vParsedTransaction.atm_id and atm.region_id = r.region_id and 
 + ATM.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") " + buildSearchCriteria() + " order by trxn_datetime desc";

                        Ds = new dsCashWithdrawals();
                    }
                    else
                    {
                        if (Request.QueryString["ej"] == "1")
                        {
                            cmd.CommandText = @"select ej_summary.atm_id,region.region_name,region.region_id, parent_region_id, title,trxn_datetime,
withdrawals, closing_balance, replenishment_amount,return_amount ,atm.location,(select count(ej_parsed_transactions_id) from 
vEjParsedTransactions where trxn_datetime>=ej_summary.trxn_datetime and trxn_datetime<=convert(datetime,convert(varchar,ej_summary.trxn_datetime,103)+ '',103)
and atm_id = ej_summary.atm_id ) transaction_count
                        from atm, ej_summary, region  where atm.atm_id = ej_summary.atm_id and atm.region_id = region.region_id and ";

                            if (TextBox_threshold.Text.Length > 0)
                                cmd.CommandText += " withdrawals <=" + TextBox_threshold.Text + " and ";

                            cmd.CommandText += " ATM.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") " + buildSearchCriteria() + " order by trxn_datetime desc";

                        }
                        else
                        {
                            StringBuilder groupbuilder = new StringBuilder();
                            foreach (ListItem item in groupList.Items)
                            {
                                if (item.Selected)
                                {
                                    if (groupbuilder.Length > 0)
                                        groupbuilder.Append(",");
                                    groupbuilder.Append(item.Value);
                                }

                            }
                            cmd.CommandText = "ExtractWtihdrawalSummary";
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("@FromDate", SqlDbType.VarChar);
                            cmd.Parameters.Add("@ToDate", SqlDbType.VarChar);
                            cmd.Parameters.Add("@AtmIds", SqlDbType.VarChar);
                            cmd.Parameters.Add("@Threshold", SqlDbType.Decimal);
                            cmd.Parameters.Add("@NoteSetTypeID", SqlDbType.Int);


                            cmd.Parameters[0].Value = TextBox_FromDate.Text.Split(' ')[0];
                            cmd.Parameters[1].Value = TextBox_ToDate.Text.Split(' ')[0];
                            cmd.Parameters[2].Value = Session[SessionVars.SelectedATMs.ToString()];
                            if (TextBox_threshold.Text.Length > 0)
                                cmd.Parameters[3].Value = decimal.Parse(TextBox_threshold.Text);
                            else
                                cmd.Parameters[3].Value = -1;

                            if (groupbuilder.Length > 0)
                                cmd.Parameters[4].Value = groupbuilder.ToString();
                            else
                                cmd.Parameters[4].Value = -1;
                        }
                        Ds = new dsCashWithdrawalSummary();
                    }
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                    adapter.Fill(Ds.Tables["DataTable1"]);

                    PopulateDataTable(Ds.Tables["DataTable1"]);

                    //adapter.SelectCommand.CommandText = "select bank_logo from app_setting";
                    //adapter.Fill(Ds.Tables["DataTable2"]);

                    if (Ds.Tables["DataTable1"].Rows.Count > 0)
                    {
                        string URL = null;
                        Session[SessionVars.dtReportDataSet.ToString()] = Ds;
                        if (RadioButtonList_Withdrawals.SelectedIndex == 0 || RadioButtonList_Withdrawals.SelectedIndex == 2)
                        {
                            if (reportType.Visible)
                            {
                                if (Request.QueryString["ej"] == "1")
                                    URL = "ReportPopup.aspx?Report=ExportEjTransactions.rpt&ReportTitle=Cash Withdrawals Report&FromDate=" + Utility.GetDateTime(TextBox_FromDate.Text) + "&ToDate=" + Utility.GetDateTime(TextBox_ToDate.Text) + "&GeneratedBy=" + objUser.UserLogin + "&total=" + Ds.Tables["DataTable1"].Rows.Count + "&suppressDetail=" + RadioButtonList_Withdrawals.SelectedIndex;
                                else
                                    URL = "ReportPopup.aspx?Report=CashWithdrawals.rpt&ReportTitle=Cash Withdrawals Report&FromDate=" + Utility.GetDateTime(TextBox_FromDate.Text) + "&ToDate=" + Utility.GetDateTime(TextBox_ToDate.Text) + "&GeneratedBy=" + objUser.UserLogin + "&total=" + Ds.Tables["DataTable1"].Rows.Count + "&suppressDetail=" + RadioButtonList_Withdrawals.SelectedIndex;
                            }
                            else
                                URL = "ReportPopup.aspx?Report=CashWithdrawals.rpt&ReportTitle=Exception Handling Report&FromDate=" + Utility.GetDateTime(TextBox_FromDate.Text) + "&ToDate=" + Utility.GetDateTime(TextBox_ToDate.Text) + "&GeneratedBy=" + objUser.UserLogin + "&total=" + Ds.Tables["DataTable1"].Rows.Count + "&suppressDetail=" + RadioButtonList_Withdrawals.SelectedIndex;
                        }
                        else
                            URL = "ReportPopup.aspx?Report=CashWithdrawalSummary.rpt&ReportTitle=Cash Withdrawal Summary Report&FromDate=" + Utility.GetDateTime(TextBox_FromDate.Text) + "&ToDate=" + Utility.GetDateTime(TextBox_ToDate.Text) + "&GeneratedBy=" + objUser.UserLogin + "&total=" + Ds.Tables["DataTable1"].Rows.Count;

                        Literal_ok.Text = "<script>window.open(\"" + URL + "\");</script>";
                    }
                    else
                        Literal_ok.Text = "<li>No record(s) found</li>";
                
            }

            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_errorMsg.Text = "<li> An error occurred while generating Report::" + ex.Message;
                if (ex.Message.ToLower().Contains("invalid"))
                    throw;
            }
            finally
            {
                task.EndTask();
            }


        }
        protected void RadioButtonList_Withdrawals_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (RadioButtonList_Withdrawals.SelectedIndex == 0)
            {
                Panel1.Visible = false;
                TextBox_Dispensed1.Text = TextBox_Dispensed2.Text = TextBox_Dispensed3.Text = TextBox_Dispensed4.Text = "";
                TextBox_FromAmount.Text = TextBox_ToAmount.Text = "";
                TextBox_purgeFrom.Text = TextBox_purge_to.Text = "";

                TextBox_Dispensed1.Enabled = TextBox_Dispensed2.Enabled = TextBox_Dispensed3.Enabled = TextBox_Dispensed4.Enabled = true;
                TextBox_FromAmount.Enabled = TextBox_ToAmount.Enabled = true;
                TextBox_purgeFrom.Enabled = TextBox_purge_to.Enabled = true;
            }
            else
            {
                Panel1.Visible = !Panel1.Visible;
                TextBox_Dispensed1.Text = TextBox_Dispensed2.Text = TextBox_Dispensed3.Text = TextBox_Dispensed4.Text = "";
                TextBox_FromAmount.Text = TextBox_ToAmount.Text = "";
                TextBox_purgeFrom.Text = TextBox_purge_to.Text = "";

                TextBox_Dispensed1.Enabled = TextBox_Dispensed2.Enabled = TextBox_Dispensed3.Enabled = TextBox_Dispensed4.Enabled = false;
                TextBox_FromAmount.Enabled = TextBox_ToAmount.Enabled = false;
                TextBox_purgeFrom.Enabled = TextBox_purge_to.Enabled = false;

            }
        }
        private string GetNoteSetTypeId()
        {
            StringBuilder builder = new StringBuilder();
            foreach (ListItem chk in groupList.Items)
            {
                if (chk.Selected)
                    builder.Append(chk.Value + ",");
            }
            builder.Append("-1");
            return builder.ToString();
        }
      

        public void ValidateInputs()
        {
            
            bool isFromAmountContainSpecialChars = CheckSpecialChars(TextBox_FromAmount.Text);
            if (isFromAmountContainSpecialChars == true)
            {
                throw new Exception("From Amount contain invalid character");
                
            }
            bool isToAmountContainSpecialChars = CheckSpecialChars(TextBox_ToAmount.Text);
            if (isToAmountContainSpecialChars == true)
            {
                throw new Exception("To Amount contain invalid character");
                
            }
            bool isPurgeFromContainSpecialChars = CheckSpecialChars(TextBox_purgeFrom.Text);
            if (isPurgeFromContainSpecialChars == true)
            {
                throw new Exception("Purge From contain invalid character");
                
            }
            bool isPurgeToContainSpecialChars = CheckSpecialChars(TextBox_purge_to.Text);
            if (isPurgeToContainSpecialChars == true)
            {
               throw new Exception("Purge To contain invalid character");
                
            }
            bool isDispensed1ContainSpecialChars = CheckSpecialChars(TextBox_Dispensed1.Text);
            if (isDispensed1ContainSpecialChars == true)
            {
                throw new Exception("Dispensed 1 contain invalid character");
                
            }
            bool isDispensed2ContainSpecialChars = CheckSpecialChars(TextBox_Dispensed2.Text);
            if (isDispensed2ContainSpecialChars == true)
            {
                throw new Exception("Dispensed 2 contain invalid character");
                
            }
            bool isDispensed3ContainSpecialChars = CheckSpecialChars(TextBox_Dispensed3.Text);
            if (isDispensed3ContainSpecialChars == true)
            {
               throw new Exception("Dispensed 3 contain invalid character");
                
            }
            bool isDispensed4ContainSpecialChars = CheckSpecialChars(TextBox_Dispensed4.Text);
            if (isDispensed4ContainSpecialChars == true)
            {
            throw new Exception("Dispensed 4 contain invalid character");
                
            }
            
        }
    }
}