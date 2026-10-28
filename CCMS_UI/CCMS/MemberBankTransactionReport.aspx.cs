using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Avanza.iSuite.UI;
using Avanza.CCMS.DAL;
using System.Data.SqlClient;

using System.Data;
using System.Reflection;
using System.Diagnostics;
using System.Text;
using System.Web.Configuration;

namespace CCMSUI.CCMS
{
    public partial class MemberBankTransactionReport : BasePage
    {
        bool isMultiInstitution = System.Web.Configuration.WebConfigurationManager.AppSettings["isMultiInstitution"] == "1" ? true : false;

        protected void Page_Load(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Page_Load");
            try
            {
                if (!Page.IsPostBack)
                {
                    if (!CheckRight((int)Permissions.MemberBankTransactionReport))
                        Response.Redirect("../admin/AccessDenied.aspx");

                    if (WebConfigurationManager.AppSettings["isScheduleCreationEnabled"] == "0")
                        Button_GenerateReportSchedule.Enabled = false;


                    TransactionType.TransactionTypeReader trxnReader = TransactionType.ExecuteReader("1=1");
                    while (trxnReader.Read())
                    {
                        TrxTypeList.Items.Add(new ListItem(trxnReader.CurrentTransactionType.TransactionTypeName, trxnReader.CurrentTransactionType.TransactionTypeId.ToString()));
                    }
                    trxnReader.Close();

                    ///<Summary>
                    ///Code edited by Ali Faiyaz Shah on 4th Sep, 15   15:30
                    ///To include "Filter by comment" feature while generating MemberBankTransactionReport
                    Avanza.CCMS.DAL.Comment.CommentReader commentReader = Avanza.CCMS.DAL.Comment.ExecuteReader("comment_id>0");
                    while (commentReader.Read())
                    {
                        DropDownList_Comment.Items.Add(new ListItem(commentReader.CurrentComment.CommentText, commentReader.CurrentComment.CommentId.ToString()));
                    }
                    DropDownList_Comment.DataTextField = "comment_text";
                    DropDownList_Comment.DataValueField = "comment_id";
                    DropDownList_Comment.DataBind();
                    commentReader.Close();
                    ///</Summary>

                    SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                    cmd.CommandText = @"select PAN_prefix, bank_name from fitmapping ";
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.SelectCommand = cmd;
                    DataTable dtFIT = new DataTable();
                    adapter.Fill(dtFIT);



                    DataTable temp = CCMSUI.admin.FITMapping.MergeFIT(dtFIT);

                    CheckBoxList_FIT.DataSource = temp;
                    CheckBoxList_FIT.DataTextField = "bank_name";
                    CheckBoxList_FIT.DataValueField = "pan_prefix";
                    CheckBoxList_FIT.DataBind();

                    TextBox_FromDate.Text = DateTime.Today.AddDays(-1).ToString("dd/MM/yyyy");
                    TextBox_ToDate.Text = DateTime.Today.ToString("dd/MM/yyyy");

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
            StringBuilder selectQuery = new StringBuilder(" transaction_type.transaction_type_id in (-1");

            for (int i = 0; i < TrxTypeList.Items.Count; i++)
                if (TrxTypeList.Items[i].Selected)
                    selectQuery.Append("," + TrxTypeList.Items[i].Value);

            selectQuery.Append(") AND ");

            if (TextBox_FromDate.Text.Length > 0)
                selectQuery.Append(@" trxn_datetime >= Convert(datetime,'" + TextBox_FromDate.Text + "',103) AND ");

            if (TextBox_ToDate.Text.Length > 0)
                selectQuery.Append(@"  trxn_datetime <=Convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103) AND ");
            if (TextBox_TSN.Text.Length > 0)
                selectQuery.Append(@"  tsn>='" + TextBox_TSN.Text + "' AND ");
            if (TextBox_ToTSN.Text.Length > 0)
                selectQuery.Append(@"  tsn<='" + TextBox_ToTSN.Text + "' AND ");
            ///<Summary>
            ///Code edited by Ali Faiyaz Shah on 4th Sep, 15   15:15
            ///To include "Filter by comment" feature while generating MemberBankTransactionReport
            if (DropDownList_Comment.SelectedIndex > 0)
                selectQuery.Append(@"vEjParsedTransactions.comment_id= " + DropDownList_Comment.SelectedItem.Value + " AND ");
            ///</Summary>
    
            //if (DropDownList_Status.SelectedIndex>0)
                //selectQuery.Append(@"  status=" + DropDownList_Status.SelectedItem.Value + " AND ");

            StringBuilder builder = new StringBuilder();
            foreach (ListItem item in CheckBoxList_Status.Items)
            {
                if (item.Selected)
                    builder.Append(item.Value + ",");
            }
            builder.Append("-1");
            //  if (CheckBoxList_Status.SelectedIndex > 0)
            selectQuery.Append(" status in (" + builder.ToString() + ") AND ");


            string ATMIds = Convert.ToString(Session[SessionVars.SelectedATMs.ToString()]);
            selectQuery.Append(@" atm.atm_id in (" + ATMIds + ") AND ");

            string query = selectQuery.ToString();

            if (query.EndsWith("AND "))
                query = query.Substring(0, query.LastIndexOf("AND"));

            return " and " + query;
        }
        private void PopulateDataTable(DataTable dt)
        {
            int region_id = 0;
            int id = -1;
            Region region = null;

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
                if (isMultiInstitution)
                {
                    region = Region.LoadRegionByPk(id);
                    dt.Rows[i][9] = region.RegionName;
                }
                else
                {
                    if (region == null)
                        region = Region.LoadRegionByPk(id);

                    dt.Rows[i][9] = region.RegionName;
                }
               // dt.Rows[i][4] = region.BankLogo;

            }

        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Search");
            try
            {

                ValidateInputs();
                
                    string URL = "";
                    string[] fitList;
                    AppUser objUser = (AppUser)Session[SessionVars.user.ToString()];
                    StringBuilder builder = new StringBuilder();
                    foreach (ListItem selectedItem in CheckBoxList_FIT.Items)
                    {
                        if (selectedItem.Selected)
                        {
                            fitList = selectedItem.Value.Split(',');
                            foreach (string fitItem in fitList)
                                builder.Append("'" + fitItem + "',");
                        }
                    }

                    if (builder.Length > 0)
                        builder.Remove(builder.Length - 1, 1);
                    // Data Archiving Work
                    // means Banks and summary and Transactions and ATMs only
                    string Query = "select case when status =0 then 'Successful' when status =1 then 'Failed' when status = 2 then 'Suspicious' end status ,  title,transaction_type_name, vEjParsedTransactions.bank_name, pan,tsn,amount,donation_amount,transferred_amount,trxn_datetime, " +
                                    " region.region_name,region.region_id, parent_region_id,atm.location,notes_dispensed_type1,notes_dispensed_type2,notes_dispensed_type3,notes_dispensed_type4 " +
                                    " from atm inner join vEjParsedTransactions " +
                                    " on atm.atm_id = vEjParsedTransactions.atm_id " +
                                    " inner join transaction_type " +
                                    " on transaction_type.transaction_type_id =vEjParsedTransactions.transaction_type_id " +
                                    " inner join FITMapping " +
                                    " on substring(vEjParsedTransactions.PAN,1,6) = FITMapping.PAN_prefix " +
                                    " inner join region on atm.region_id = region.region_id " +
                                    " where atm.is_active = 1 ";
                    if (builder.Length > 0)
                        Query += " and  FITMapping.PAN_prefix in (" + builder.ToString() + ")";
                    Query += buildSearchCriteria();

                    SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                    cmd.CommandTimeout = 100;
                    cmd.CommandText = Query;
                    dsMemberBankTransaction Ds = new dsMemberBankTransaction();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(Ds.Tables[0]);
                    if (Ds.Tables[0].Rows.Count > 0)
                    {

                        Session[SessionVars.dtReportDataSet.ToString()] = Ds;
                        URL = "ReportPopup.aspx?Report=EJTransactions.rpt&ReportTitle=Member Bank Transaction Report&FromDate=" + Utility.GetDateTime(TextBox_FromDate.Text) + "&ToDate=" + Utility.GetDateTime(TextBox_ToDate.Text) + "&GeneratedBy=" + objUser.UserLogin + "&total=" + Ds.Tables[0].Rows.Count;
                    }
                    else
                        Literal_ok.Text = "<li>No record(s) found</li>";



                    if (URL != "")
                        Literal_ok.Text = "<script>window.open(\"" + URL + "\");</script>";

                
            }
            catch (Exception ex)
            {
                Literal_error.Text = "<li> An error occurred while generating Report</li>";
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex + ", Ex Message : "+ex.Message);
                if (ex.Message.ToLower().Contains("invalid"))
                    throw;
            }
            finally
            {
                task.EndTask();
            }
        }


        protected void btnReset_Click(object sender, EventArgs e)
        {
            TextBox_FromDate.Text = DateTime.Today.AddMonths(-1).ToString("dd/MM/yyyy");
            TextBox_ToDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
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
                                                          where report_name='MemberBankTransactionReport' and organization_id = " + orgID + "); " +

                                                         " delete report_schedule where report_name='MemberBankTransactionReport' and organization_id = " + orgID, trxn);
                       
                        reportSchedule = new ReportSchedule();
                        reportSchedule.ReportName = "MemberBankTransactionReport";
                        reportSchedule.ReportPhysicalPath = Server.MapPath(@"EJTransactions.rpt");
                        reportSchedule.ReportReceipients = "";
                        AppSetting appSetting = (AppSetting)Application[ApplicationVars.AppSettings.ToString()];
                        if (appSetting != null)
                            reportSchedule.ReportTempPath = appSetting.TemporaryFolder;
                        else
                            reportSchedule.ReportTempPath = @"c:\view360LiveData\temp\";
                        reportSchedule.RetryCount = 5;
                        reportSchedule.ReportNextGeneratedAt = DateTime.Today;
                        reportSchedule.ReportFriendlyName = "Member Bank Transaction Report";
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
                        Utility.BuildAuditLog("Schedule added for Member Bank Transaction Report", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.MemberBankTransactionReport);
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

        public void ValidateInputs()
        {
            
            bool isTSNContainSpecialChars = CheckSpecialChars(TextBox_TSN.Text);
            if (isTSNContainSpecialChars == true)
            {
                throw new Exception("TSN contain invalid character");
            
            }
            bool isToTSNContainSpecialChars = CheckSpecialChars(TextBox_ToTSN.Text);
            if (isToTSNContainSpecialChars == true)
            {
                throw new Exception("To TSN contain invalid character");
                
            }
            
        }
    }
}