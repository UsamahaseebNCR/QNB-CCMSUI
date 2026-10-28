using Avanza.CCMS.DAL;
using Avanza.iSuite.UI;
using ClosedXML.Excel;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CCMSUI.CCMS
{
    public partial class CurrentCashPositionsReport : BasePage
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
            if (!Page.IsPostBack)
            {
                if (!CheckRight((int)Permissions.ViewCashPositionReport))
                    Response.Redirect("~/admin/AccessDenied.aspx");

                if (WebConfigurationManager.AppSettings["isScheduleCreationEnabled"] == "0")
                    Button_GenerateReportSchedule.Enabled = false;


                TextBox_FromDate.Text = DateTime.Today.ToString();
                TextBox_ToDate.Text = DateTime.Now.ToString();
                BindNoteSetType();
            }
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
                region_id = int.Parse(dt.Rows[i][14].ToString()); //parent

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
                dt.Rows[i][15] = region.RegionName;
                dt.Rows[i][17] = region.BankLogo;

            }

        }
        private void SplitAlertMessageIntoColumns(DataTable dataTable)
        {
            foreach (DataRow dr in dataTable.Rows)
            {
                string[] parts = dr["alert_msg"].ToString().Split(',');
                dr["type1"] = parts[0];
                dr["type2"] = parts[1];
                dr["type3"] = parts[2];
                dr["type4"] = parts[3];
                dr["balance"] = parts[7];
            }
        }
        string Filter()
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
                filter += " and note_set_type.note_set_type_id in ( " + groupbuilder.ToString() + " ) ";

            if (TextBox_FromDate.Text.Length > 0 && TextBox_FromDate.Enabled)
                filter += " and last_trxn_at >= convert(datetime,'" + TextBox_FromDate.Text + "',103)";

            if (TextBox_ToDate.Text.Length > 0 && TextBox_ToDate.Enabled)
                filter += " and last_trxn_at<= convert(datetime,'" + TextBox_ToDate.Text + "',103)";



            return filter;
        }
        protected void btnExport_Click(object sender, EventArgs e)
        {

            LogableTask task = LogableTask.NewTask("Search");
            SqlCommand cmd = null;
            DataTable dt = new DataTable();
            try
            {

                string filter = "";
                if (WebConfigurationManager.AppSettings["isDeadATMExcluded"] == "1")
                    filter = " and outerATM.ATM_id in (select ATM_id from vHeartBeat where heart_beat_received_at >=convert(datetime,'" + TextBox_FromDate.Text + "',103) and " +
                                         "heart_beat_received_at <=convert(datetime,'" + TextBox_ToDate.Text + "',103))";


                cmd = ConnectionFactory.GetNewCommand(true);
                cmd.CommandTimeout = 600;
                cmd.CommandText = @"GetCurrentCashPositionsForQNB";
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "SP -> " + cmd.CommandText);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@CurrentDate", SqlDbType.DateTime));
                cmd.Parameters[0].Value = DateTime.Today;
                cmd.Parameters.Add(new SqlParameter("@ATMsList", SqlDbType.VarChar));
                cmd.Parameters[1].Value = Session[SessionVars.SelectedATMs.ToString()];
                cmd.Parameters.Add(new SqlParameter("@Filter", SqlDbType.VarChar));
                cmd.Parameters[2].Value = Filter() + " " + filter;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
                ExportToExcel(dt);

            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_error.Text = "<li> An error occurred while generating Report::" + ex.Message;
            }
            finally
            {
                if (cmd != null)
                    if (cmd.Connection != null)
                        cmd.Connection.Close();
                task.EndTask();
            }

        }

        protected void ExportToExcel(DataTable dt)
        {

            if (dt != null && dt.Rows.Count > 0)
            {
                using (XLWorkbook workbook = new XLWorkbook())
                {
                    string fileName = $"CPReport_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}";
                    var worksheet = workbook.Worksheets.Add(fileName);
                    worksheet.Cell(1, 1).InsertTable(dt); // Insert DataTable starting from Cell A1

                    worksheet.Columns().AdjustToContents(); // Auto-fit the columns

                    Response.Clear();
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", $"attachment; filename={fileName}.xlsx");

                    // Write the Excel workbook to the response stream
                    using (MemoryStream stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        stream.WriteTo(Response.OutputStream);
                        Response.AppendHeader("Set-Cookie", "fileDownload=true; path=/");
                        Response.Flush();
                        Response.End();
                    }
                }
            }
            else
            {
                Literal_ok.Text = "<li>No data available to export.</li>";
            }
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Search");
            SqlCommand cmd = null;
            dsCashPositions ds = new dsCashPositions();
            try
            {

                string filter = "";
                //if (DropDownList_NoteSetTypeID.SelectedIndex > 0)
                //    filter = " and outerATM.note_set_type_id = " + DropDownList_NoteSetTypeID.SelectedItem.Value;
                if (WebConfigurationManager.AppSettings["isDeadATMExcluded"] == "1")
                    filter = " and outerATM.ATM_id in (select ATM_id from vHeartBeat where heart_beat_received_at >=convert(datetime,'" + TextBox_FromDate.Text + "',103) and " +
                                         "heart_beat_received_at <=convert(datetime,'" + TextBox_ToDate.Text + "',103))";


                cmd = ConnectionFactory.GetNewCommand(true);
                cmd.CommandTimeout = 600;
                //  cmd.CommandText = "set transaction isolation level SERIALIZABLE";
                //    cmd.ExecuteNonQuery();
                if (CheckBox_Current.Checked)
                {
                    cmd.CommandText = @"GetCurrentCashPositions";
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "SP -> " + cmd.CommandText);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@CurrentDate", SqlDbType.DateTime));
                    cmd.Parameters[0].Value = DateTime.Today;
                    cmd.Parameters.Add(new SqlParameter("@ATMsList", SqlDbType.VarChar));
                    cmd.Parameters[1].Value = Session[SessionVars.SelectedATMs.ToString()];
                    cmd.Parameters.Add(new SqlParameter("@Filter", SqlDbType.VarChar));
                    cmd.Parameters[2].Value = Filter() + " " + filter;

                    //dsCashPositions ds = new dsCashPositions();
                    //cmd.Connection.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    //SqlDataReader rdr = cmd.ExecuteReader();
                    adapter.Fill(ds.Tables["DataTable1"]);
                    //rdr.Close();
                }

                else
                {

                    cmd.CommandText = @"select note_set_type_name,outerATM.description ,outerATM.location,outerATM.title title, note_set_type.denomination_type_1_title denomination_type_1, note_set_type.denomination_type_2_title denomination_type_2, note_set_type.denomination_type_3_title denomination_type_3,
                                    note_set_type.denomination_type_4_title denomination_type_4, note_set_type.denomination_type_5_title denomination_type_5, 
                                    note_set_type.denomination_type_6_title denomination_type_6, note_set_type.denomination_type_7_title denomination_type_7, 
                                    p.cassette1_notes, p.cassette2_notes,
                                    p.cassette3_notes, p.cassette4_notes, p.cassette5_notes, p.cassette6_notes,
                                    p.cassette7_notes,v.cash_added1,v.cash_added2,v.cash_added3,v.cash_added4, (select max(trxn_datetime) from vParsedTransaction where atm_id = outerATM.atm_id and amount>0 )last_successful_trxn_at,last_trxn_at, " +
       " note_set_type.denomination_type_1 * p.cassette1_notes +  " +
       " note_set_type.denomination_type_2 * p.cassette2_notes + " +
       " note_set_type.denomination_type_3 * p.cassette3_notes + " +
       " note_set_type.denomination_type_4 * p.cassette4_notes + " +
       " note_set_type.denomination_type_5 * p.cassette5_notes + " +
       " note_set_type.denomination_type_6 * p.cassette6_notes + " +
       " note_set_type.denomination_type_7 * p.cassette7_notes total ,'' organization," +
       " ( select max(rep_datetime) from vReplenishment where atm_id = outerATM.atm_id)last_replenishment_at,( isnull((select region_name + '-' from region where region_id = r.parent_region_id and is_organization=0),'') + r.region_name) region_name,r.region_id,parent_region_id,bank_logo " +
       " ,  ( select min(cash_order_datetime) from cash_order_monitoring where  atm_id = outerATM.atm_id    " +
       " and cash_order_datetime>=convert(datetime,'" + DateTime.Today.ToString() + "',103))next_replenishment_at, " +

       " ( select min(current_order_suggested_amount) from cash_order_monitoring where  atm_id = outerATM.atm_id  " +
       " and cash_order_datetime>=convert(datetime,'" + DateTime.Today.ToString() + "',103))amount, dbo.GetTotalText(note_set_type.denomination_type_1_title,note_set_type.denomination_type_2_title,note_set_type.denomination_type_3_title,note_set_type.denomination_type_4_title,note_set_type.denomination_type_1, note_set_type.denomination_type_2, note_set_type.denomination_type_3, note_set_type.denomination_type_4,p.cassette1_notes, p.cassette2_notes, p.cassette3_notes, p.cassette4_notes) total_text, outerATM.location " +
        " from atm outerATM , vCashPosition p ,note_set_type,region r, vReplenishment v" +
       " where outerATM.atm_id = p.atm_id and outerATM.atm_id  = v.atm_id and outerATM.is_active = 1 and outerATM.exclude_dff= 0  and outerATM.is_atm = 1  " +
       " and outerATM.note_set_type_id = note_set_type.note_set_type_id " + Filter() +
       "  and outerATM.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and outerATM.region_id = r.region_id" + filter;


                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "query ->" + cmd.CommandText);


                    //dsCashPositions ds = new dsCashPositions();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(ds.Tables["DataTable1"]);

                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "total count ->" + ds.Tables["DataTable1"].Rows.Count);
                }
                PopulateDataTable(ds.Tables["DataTable1"]);
                //SplitAlertMessageIntoColumns(ds.Tables["DataTable1"]);
                //task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "total count ->" + ds.Tables["DataTable1"].Rows.Count);

                AppUser objUser = (AppUser)Session[SessionVars.user.ToString()];


                if (ds.Tables["DataTable1"].Rows.Count > 0)
                {
                    Session[SessionVars.dtReportDataSet.ToString()] = ds;
                    string URL = "ReportPopup.aspx?Report=CashPositionsRpt.rpt&ReportTitle=Cash Position Report&FromDate=" + Utility.GetDateTime(TextBox_FromDate.Text) + "&ToDate=" + Utility.GetDateTime(TextBox_ToDate.Text) + "&GeneratedBy=" + objUser.UserLogin + "&total=" + ds.Tables["DataTable1"].Rows.Count + "&suppressDetail=" + (CheckBox_SuppressDetail.Checked ? "1" : "0");
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
                if (cmd != null)
                    if (cmd.Connection != null)
                        cmd.Connection.Close();
                task.EndTask();
            }
        }
        protected void Button_Reset_Click(object sender, EventArgs e)
        {
            TextBox_FromDate.Text = DateTime.Now.ToString();
            TextBox_ToDate.Text = DateTime.Now.ToString();
        }

        protected void CheckBox_Current_CheckedChanged(object sender, EventArgs e)
        {

            TextBox_FromDate.Enabled = (!TextBox_FromDate.Enabled);
            TextBox_ToDate.Enabled = (!TextBox_ToDate.Enabled);

        }
        protected void Button_GenerateReportSchedule_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Button_GenerateReportSchedule_Click");
            ReportSchedule reportSchedule = null;
            SqlCommand cmd = null;
            SqlTransaction trxn = null;
            string reportName = null;
            int reportAge = CheckBox_Current.Checked ? 0 : 1;
            reportName = reportAge == 0 ? "CurrentCashPositionsReport" : "CashPositionsReport";
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
                                                          where report_name='" + reportName + "' and organization_id = " + orgID + "); " +

                                                         " delete report_schedule where report_name='" + reportName + "' and report_data_age=" + reportAge + " and organization_id = " + orgID, trxn);

                        reportSchedule = new ReportSchedule();
                        reportSchedule.ReportName = reportName;
                        reportSchedule.ReportPhysicalPath = Server.MapPath(@"CashPositionsRpt.rpt");
                        reportSchedule.ReportReceipients = "";
                        AppSetting appSetting = (AppSetting)Application[ApplicationVars.AppSettings.ToString()];
                        if (appSetting != null)
                            reportSchedule.ReportTempPath = appSetting.TemporaryFolder;
                        else
                            reportSchedule.ReportTempPath = @"c:\view360LiveData\temp\";
                        reportSchedule.RetryCount = 5;
                        reportSchedule.ReportNextGeneratedAt = DateTime.Today;
                        reportSchedule.ReportFriendlyName = "Cash Position Report";
                        reportSchedule.MinutesToScheduleAgain = 1440;
                        reportSchedule.ReportExportType = 3;
                        reportSchedule.ReportDataAge = reportAge;
                        reportSchedule.ScheduleType = false;
                        reportSchedule.OrganizationId = int.Parse(orgID);
                        reportSchedule.IsEjEnabled = false;
                        reportSchedule.ApplicableNoteSetType = GetNoteSetTypeId();
                        if (reportSchedule.ApplicableNoteSetType == "-1")
                            reportSchedule.ApplicableNoteSetType = null;
                        reportSchedule.Save(trxn.Connection, trxn);
                        trxn.Commit();
                        Literal_ok.Text = "Schedule added successfully";
                        Literal_error.Text = "";
                        Utility.BuildAuditLog("Schedule added for Cash Position Report", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.ViewCashPositionReport);
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
    }
}