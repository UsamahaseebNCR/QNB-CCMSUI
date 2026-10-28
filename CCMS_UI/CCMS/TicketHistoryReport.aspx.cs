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
using Avanza.iSuite.DAL;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using ICSharpCode.SharpZipLib.Zip;
using System.Linq;

namespace CCMSUI.CCMS
{
    public partial class TicketHistoryReport : BasePage
    {

        int idx = 4;

        public enum DowntimeType
        {
            CARD_READER,
            DISPENSER,
            CASH_OUT,
            SUPERVISOR,
            OTHER
        }
        System.Data.DataTable helperDt = null;
        ArrayList _Files = null;
        DateTime reportDate;
        List<Atm> listATMsInDb = new List<Atm>();
        List<string> listATMsWithIncidents = new List<string>();


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
                    //groupList.DataSource = dt;
                    //groupList.DataTextField = "note_set_type_name";
                    //groupList.DataValueField = "region_id";
                    //groupList.DataBind();
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

                    //if (WebConfigurationManager.AppSettings["isScheduleCreationEnabled"] == "0")
                    //    Button_GenerateReportSchedule.Enabled = false;

                    TextBox_FromDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
                    //TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

                    //if (Request.QueryString["ej"] == "1" || Request.QueryString["ej"] == "2")
                    //{
                    //    TextBox_purgeFrom.Enabled = false;
                    //    TextBox_purge_to.Enabled = false;

                    //    if (Request.QueryString["ej"] == "2")
                    //        reportType.Visible = false;
                    //}
                    //BindNoteSetType();
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
            //StringBuilder groupbuilder = new StringBuilder();
            //foreach (ListItem item in groupList.Items)
            //{
            //    if (item.Selected)
            //    {
            //        if (groupbuilder.Length > 0)
            //            groupbuilder.Append(",");
            //        groupbuilder.Append(item.Value);
            //    }

            //}
            string filter = "";

            //if (groupbuilder.Length > 0)
            //    filter = " and atm.note_set_type_id  in ( " + groupbuilder.ToString() + " ) ";


            if (TextBox_FromDate.Text.Length > 0)
                filter += " and trxn_datetime >= convert(datetime,'" + TextBox_FromDate.Text + "',103)";

            //if (TextBox_ToDate.Text.Length > 0)
            //    filter += " and trxn_datetime <= convert(datetime,'" + TextBox_ToDate.Text + "',103)";


            //if (RadioButtonList_Withdrawals.SelectedIndex == 0)
            //{
            //    if (Request.QueryString["ej"] == "1")
            //    {
            //        if (TextBox_Dispensed1.Text.Length > 0)
            //            filter += " and notes_dispensed_type1 = " + int.Parse(TextBox_Dispensed1.Text);

            //        if (TextBox_Dispensed2.Text.Length > 0)
            //            filter += " and notes_dispensed_type2 = " + int.Parse(TextBox_Dispensed2.Text);

            //        if (TextBox_Dispensed3.Text.Length > 0)
            //            filter += " and notes_dispensed_type3 = " + int.Parse(TextBox_Dispensed3.Text);

            //        if (TextBox_Dispensed4.Text.Length > 0)
            //            filter += " and notes_dispensed_type4 = " + int.Parse(TextBox_Dispensed4.Text);
            //    }
            //    else
            //    {
            //        if (TextBox_purgeFrom.Text.Length > 0)
            //            filter += " and (cash_purged1+cash_purged2+cash_purged3+cash_purged4+cash_purged5+cash_purged6+cash_purged7)>=" + int.Parse(TextBox_purgeFrom.Text);

            //        if (TextBox_purge_to.Text.Length > 0)
            //            filter += " and (cash_purged1+cash_purged2+cash_purged3+cash_purged4+cash_purged5+cash_purged6+cash_purged7)<=" + int.Parse(TextBox_purge_to.Text);



            //        if (TextBox_Dispensed1.Text.Length > 0)
            //            filter += " and cash_dispensed1 = " + int.Parse(TextBox_Dispensed1.Text);

            //        if (TextBox_Dispensed2.Text.Length > 0)
            //            filter += " and cash_dispensed2 = " + int.Parse(TextBox_Dispensed2.Text);

            //        if (TextBox_Dispensed3.Text.Length > 0)
            //            filter += " and cash_dispensed3 = " + int.Parse(TextBox_Dispensed3.Text);

            //        if (TextBox_Dispensed4.Text.Length > 0)
            //            filter += " and cash_dispensed4 = " + int.Parse(TextBox_Dispensed4.Text);
            //    }


            //    if (TextBox_FromAmount.Text.Length > 0)
            //        filter += " and amount >= " + TextBox_FromAmount.Text;

            //    if (TextBox_ToAmount.Text.Length > 0)
            //        filter += " and amount <= " + TextBox_ToAmount.Text;
            //}
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

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Search");
            try
            {
                listATMsWithIncidents.Clear();
                listATMsInDb.Clear();
                reportDate = DateTime.ParseExact(TextBox_FromDate.Text,"dd/MM/yyyy",null); // DateTime.Today.AddDays(-1);

                Atm.AtmReader atmReader = Atm.ExecuteReader("is_active=1 and title like '%MCB%'");
                while (atmReader.Read())
                    listATMsInDb.Add(atmReader.CurrentAtm);
                atmReader.Close();
                GenerateTicketHistory(reportDate);
            }

            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_errorMsg.Text = "<li> An error occurred while generating Report::" + ex.Message;
            }
            finally
            {
                task.EndTask();
            }


        }
        #region availabilityReportGeneration
        private string GenerateSheetHeaderAndDefaultSheetData(string filePath)
        {
            return File.ReadAllText(filePath);
            //StringBuilder builder = new StringBuilder();
            //builder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>");
            //builder.Append("<worksheet xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\" xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\">");
            //builder.Append("    <dimension ref=\"A1:L40\"/>");
            //builder.Append("    <sheetViews>");
            //builder.Append("        <sheetView tabSelected=\"1\" view=\"pageBreakPreview\" topLeftCell=\"A2\" zoomScale=\"60\" zoomScaleNormal=\"100\" workbookViewId=\"0\">");
            //builder.Append("		            <selection activeCell=\"K38\" sqref=\"K38\"/>");
            //builder.Append("        </sheetView>");
            //builder.Append("    </sheetViews>");
            //builder.Append("    <sheetFormatPr defaultRowHeight=\"13.5\"/>");
            //builder.Append("    <cols>");
            //builder.Append("    <col min=\"1\" max=\"1\" width=\"7.7109375\" style=\"140\" customWidth=\"1\"/>");
            //builder.Append("    <col min=\"2\" max=\"2\" width=\"9.140625\" style=\"140\"/>");

            //builder.Append("            <col min=\"3\" max=\"3\" width=\"11.140625\" style=\"140\" customWidth=\"1\"/>");
            //builder.Append("    <col min=\"4\" max=\"4\" width=\"21.7109375\" style=\"140\" customWidth=\"1\"/>");
            //builder.Append("             <col min=\"5\" max=\"5\" width=\"11.7109375\" style=\"140\" customWidth=\"1\"/>");
            //builder.Append("            <col min=\"6\" max=\"6\" width=\"10.85546875\" style=\"140\" customWidth=\"1\"/>");
            //builder.Append("            <col min=\"7\" max=\"7\" width=\"14.28515625\" style=\"140\" customWidth=\"1\"/>");
            //builder.Append("            <col min=\"8\" max=\"8\" width=\"10.5703125\" style=\"140\" customWidth=\"1\"/>");
            //builder.Append("            <col min=\"9\" max=\"9\" width=\"14\" style=\"140\" customWidth=\"1\"/>");
            //builder.Append("           <col min=\"10\" max=\"10\" width=\"14.140625\" style=\"140\" customWidth=\"1\"/>");
            //builder.Append("           <col min=\"11\" max=\"11\" width=\"21.7109375\" style=\"140\" customWidth=\"1\"/>");
            //builder.Append("           <col min=\"12\" max=\"16384\" width=\"9.140625\" style=\"140\"/>");
            //builder.Append("       </cols>");
            //builder.Append("       <sheetData>");
            //builder.Append("           <row r=\"1\" spans=\"1:12\">");
            //builder.Append("              <c r=\"K1\" s=\"141\"/>");
            //builder.Append("          </row>");
            //builder.Append("          <row r=\"2\" spans=\"1:12\">");
            //builder.Append("              <c r=\"F2\" s=\"142\" t=\"s\">");
            //builder.Append("                <v>7</v>");
            //builder.Append("              </c><c r=\"G2\" s=\"143\"/>");
            //builder.Append("              <c r=\"H2\" s=\"143\"/>");
            //builder.Append("             <c r=\"I2\" s=\"144\"/>");
            //builder.Append("              <c r=\"J2\" s=\"145\" t=\"s\">");
            //builder.Append("                <v>0</v>");
            //builder.Append("              </c><c r=\"K2\" s=\"174\">");
            //builder.Append("                <v>" + orderDate.ToOADate() + "</v>");
            //builder.Append("              </c>");
            //builder.Append("          </row>");
            //builder.Append("          <row r=\"3\" spans=\"1:12\">");
            //builder.Append("              <c r=\"I3\" s=\"147\"/>");
            //builder.Append("              <c r=\"J3\" s=\"147\" t=\"s\">");
            //builder.Append("                <v>1</v>");
            //builder.Append("              </c>");

            //builder.Append("              <c r=\"K3\" s=\"175\" t=\"inlineStr\">");
            //builder.Append("                 <is>");
            //builder.Append("                     <t>" + orderDate.DayOfWeek + "</t>");
            //builder.Append("                  </is>");
            //builder.Append("             </c>");
            //builder.Append("          </row>");
            //builder.Append("          <row r=\"4\" spans=\"1:12\">");
            //builder.Append("              <c r=\"J4\" s=\"148\"/>");
            //builder.Append("          </row>");
            //builder.Append("          <row r=\"5\" spans=\"1:12\">");
            //builder.Append("              <c r=\"A5\" s=\"141\"/>");
            //builder.Append("              <c r=\"F5\" s=\"149\" t=\"s\">");
            //builder.Append("                <v>5</v>");
            //builder.Append("             </c><c r=\"G5\" s=\"150\"/>");
            //builder.Append("              <c r=\"J5\" s=\"151\"/>");
            //builder.Append("              <c r=\"K5\" s=\"152\"/>");
            //builder.Append("              <c r=\"L5\" s=\"141\"/>");
            //builder.Append("          </row>");
            //builder.Append("          <row r=\"6\" spans=\"1:12\">");
            //builder.Append("              <c r=\"A6\" s=\"153\" t=\"s\">");
            //builder.Append("                <v>90</v>");
            //builder.Append("             </c><c r=\"B6\" s=\"154\" t=\"s\">");
            //builder.Append("                <v>2</v>");
            //builder.Append("             </c><c r=\"C6\" s=\"154\" t=\"s\">");
            //builder.Append("                <v>3</v>");
            //builder.Append("             </c><c r=\"D6\" s=\"154\" t=\"s\">");
            //builder.Append("                <v>4</v>");
            //builder.Append("            </c><c r=\"E6\" s=\"150\">");
            //builder.Append("                <v>1000</v>");
            //builder.Append("            </c><c r=\"F6\" s=\"154\">");
            //builder.Append("               <v>500</v>");
            //builder.Append("           </c><c r=\"G6\" s=\"154\">");
            //builder.Append("               <v>200</v>");
            //builder.Append("          </c><c r=\"H6\" s=\"145\">");
            //builder.Append("              <v>100</v>");
            //builder.Append("          </c><c r=\"I6\" s=\"150\" t=\"s\">");
            //builder.Append("              <v>6</v>");
            //builder.Append("          </c><c r=\"J6\" s=\"144\" t=\"s\">");
            //builder.Append("              <v>24</v>");
            //builder.Append("          </c><c r=\"K6\" s=\"146\" t=\"s\">");
            //builder.Append("              <v>301</v>");
            //builder.Append("          </c><c r=\"L6\" s=\"145\" t=\"s\">");
            //builder.Append("               <v>423</v>");
            //builder.Append("           </c>");
            //builder.Append("       </row>");
            //return builder.ToString();
        }
        private string GenerateFormulaRowAndSheetEndTag(int idx)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(string.Format("<row r=\"{0}\" spans=\"1:12\" s=\"160\" customFormat=\"1\">", idx, idx - 1));
            builder.Append(string.Format("<c r=\"E{0}\" s=\"176\"><f>SUM(E7:E{1})</f><v>25100000</v></c>", idx, idx - 1));
            builder.Append(string.Format("<c r=\"F{0}\" s=\"176\"><f>SUM(F7:F{1})</f><v>25100000</v></c>", idx, idx - 1));
            builder.Append(string.Format("<c r=\"G{0}\" s=\"176\"><f>SUM(G7:G{1})</f><v>25100000</v></c>", idx, idx - 1));
            builder.Append(string.Format("<c r=\"H{0}\" s=\"176\"><f>SUM(H7:H{1})</f><v>25100000</v></c>", idx, idx - 1));
            builder.Append(string.Format("<c r=\"I{0}\" s=\"176\"><f>SUM(I7:I{1})</f><v>25100000</v></c>", idx, idx - 1));
            builder.Append("</row>");
            builder.Append(string.Format("<row r=\"{0}\" spans=\"1:12\" s=\"160\" customFormat=\"1\"/>", idx + 1));
            builder.Append(string.Format("<row r=\"{0}\" spans=\"1:12\" s=\"160\" customFormat=\"1\"/>", idx + 2));
            builder.Append("</sheetData>");
            return builder.ToString();
        }

        private void GetDowntime(DataTable dt, string title)
        {
            DataRow[] drATMTickets = dt.Select("ATMID='" + title + "' and duration>0 and problem_priority>-1");
            //drATMTickets.OrderBy("order by STARTDATE,STARTTime");
            foreach (DataRow dr in drATMTickets)
            {
                DateTime dtCurrentTicketStartDateTime = DateTime.ParseExact(dr["STARTDATE"].ToString() + dr["STARTTIME"].ToString(), "yyyy-MM-ddHH:mm:ss", null);
                if (DBNull.Value == dr["ENDDATE"])
                    continue;
                DateTime dtCurrentTicketEndDateTime = DateTime.ParseExact(dr["ENDDATE"].ToString() + dr["ENDTIME"].ToString(), "yyyy-MM-ddHH:mm:ss", null);
                int priorityID = int.Parse(dr["problem_priority"].ToString());//GetProblemPriority(helperDt,currentRow["PROBLEMBUCKET"].ToString()) ; // Read from datatable.
                double currentTicketDowntimeInMin = double.Parse(dr["DURATION"].ToString());

                DataRow[] drArrayOverlapCompletely = dt.Select(string.Format("ticket_start_datetime >=#{0}# and ticket_end_datetime<=#{1}# and problem_priority<{2} and ATMID = '{3}' and duration>0",
                    dtCurrentTicketStartDateTime.ToString(), dtCurrentTicketEndDateTime.ToString(), priorityID, title));

                DataRow[] drArrayOverlapExceeded = dt.Select(string.Format("ticket_start_datetime >=#{0}# and ticket_start_datetime <=#{1}# and ticket_end_datetime>#{1}# and problem_priority<{2} and ATMID = '{3}' and duration>0",
                    dtCurrentTicketStartDateTime.ToString(), dtCurrentTicketEndDateTime.ToString(), priorityID, title));

                //                DataRow[] drArrayOverlapBetween = dt.Select(string.Format("ticket_start_datetime <=#{0}# and ticket_end_datetime>=#{0}# and ticket_end_datetime<=#{1}# and problem_priority>{2} and ATMID = '{3}'",
                //                  dtCurrentTicketStartDateTime.ToString(), dtCurrentTicketEndDateTime.ToString(), priorityID, title));

                //DataRow[] drArrayOverlapBetween = dt.Select(string.Format("ticket_start_datetime >=#{0}# and ticket_end_datetime>=#{0}# and ticket_end_datetime<=#{1}# and problem_priority<{2} and ATMID = '{3}' and duration>0",
                //  dtCurrentTicketStartDateTime.ToString(), dtCurrentTicketEndDateTime.ToString(), priorityID, title));

                DataRow[] drArrayOverlapBetween = dt.Select(string.Format("ticket_start_datetime <#{0}# and ticket_end_datetime>#{0}# and ticket_end_datetime<=#{1}# and problem_priority<{2} and ATMID = '{3}' and duration>0",
               dtCurrentTicketStartDateTime.ToString(), dtCurrentTicketEndDateTime.ToString(), priorityID, title));


                for (int i = 0; i < drArrayOverlapCompletely.Length; i++)
                {
                    if (drArrayOverlapCompletely[i]["DURATION"].ToString() != "")
                        currentTicketDowntimeInMin -= double.Parse(drArrayOverlapCompletely[i]["DURATION"].ToString());
                }

                for (int i = 0; i < drArrayOverlapExceeded.Length; i++)
                    currentTicketDowntimeInMin -= (int)(DateTime.Parse(drArrayOverlapExceeded[i]["ticket_end_datetime"].ToString()) - dtCurrentTicketEndDateTime).TotalMinutes;

                for (int i = 0; i < drArrayOverlapBetween.Length; i++)
                    //currentTicketDowntimeInMin -= (int)(DateTime.Parse(drArrayOverlapBetween[i]["ticket_end_datetime"].ToString()) - dtCurrentTicketStartDateTime).TotalMinutes;
                    // currentTicketDowntimeInMin -= (int)(dtCurrentTicketEndDateTime - DateTime.Parse(drArrayOverlapBetween[i]["ticket_start_datetime"].ToString())).TotalMinutes;
                    currentTicketDowntimeInMin -= (int)(DateTime.Parse(drArrayOverlapBetween[i]["ticket_end_datetime"].ToString()) - dtCurrentTicketStartDateTime).TotalMinutes;

                if (dtCurrentTicketStartDateTime.Date == reportDate.Date && dtCurrentTicketStartDateTime.Date < dtCurrentTicketEndDateTime.Date)
                    dr["actual_duration"] = (new DateTime(dtCurrentTicketStartDateTime.Year, dtCurrentTicketStartDateTime.Month, dtCurrentTicketStartDateTime.Day + 1) - dtCurrentTicketStartDateTime).TotalMinutes;
                else if (currentTicketDowntimeInMin > 1440 || dtCurrentTicketStartDateTime.Date != dtCurrentTicketEndDateTime.Date)
                    dr["actual_duration"] = (dtCurrentTicketEndDateTime - new DateTime(dtCurrentTicketEndDateTime.Year, dtCurrentTicketEndDateTime.Month, dtCurrentTicketEndDateTime.Day)).TotalMinutes;
                else
                    dr["actual_duration"] = currentTicketDowntimeInMin;
            }

        }
        private string GenerateFooter(string filePath)
        {
            return File.ReadAllText(filePath);
            //StringBuilder builder = new StringBuilder();s
            //builder.Append("<sortState ref=\"A7:M422\">");
            //builder.Append("                <sortCondition ref=\"B7\"/>");
            //builder.Append("            </sortState>");
            //builder.Append("<pageMargins left=\"0.75\" right=\"0.75\" top=\"1\" bottom=\"1\" header=\"0.5\" footer=\"0.5\"/>");
            //builder.Append("<pageSetup scale=\"79\" orientation=\"landscape\" horizontalDpi=\"4294967293\" r:id=\"rId1\"/>");
            //builder.Append("<headerFooter alignWithMargins=\"0\"/>");
            //builder.Append("<drawing r:id=\"rId2\"/>");
            //builder.Append("</worksheet>");
            //return builder.ToString();
        }
        public void UpdateSheet(string filePath, System.Data.DataTable dt)
        {
            StringBuilder builder = new StringBuilder();
            string headerWithDefaultRow = GenerateSheetHeaderAndDefaultSheetData(System.AppDomain.CurrentDomain.BaseDirectory + "\\header_sheet_row.txt");
            string footer = GenerateFooter(System.AppDomain.CurrentDomain.BaseDirectory + "\\footer.txt");
            int idx = 2;
            foreach (DataRow dr in dt.Rows)
            {//c.cassette1_denomination,c.cassette1_suggested_notes



                builder.Append(string.Format("<row r=\"{0}\" spans=\"1:19\" s=\"1\" customFormat=\"1\" ht=\"14.85\" customHeight=\"1\" x14ac:dyDescent=\"0.15\">", idx));
                builder.Append(string.Format("<c r=\"A{0}\" s=\"3\" t=\"inlineStr\"><is><t>{1}</t></is></c>", idx, dr["ATMID"]));
                builder.Append(string.Format("<c r=\"B{0}\" s=\"3\" t=\"inlineStr\"><is><t>{1}</t></is></c>", idx, dr["ID2"]));
                builder.Append(string.Format("<c r=\"C{0}\" s=\"3\" t=\"inlineStr\"><is><t>{1}</t></is></c>", idx, dr["TICKETID"]));
                builder.Append(string.Format("<c r=\"D{0}\" s=\"3\"  t=\"inlineStr\"><is><t>{1}</t></is></c>", idx, dr["BRANCHNAME"]));



                builder.Append(string.Format("<c r=\"E{0}\" s=\"3\"  t=\"inlineStr\"><is><t>{1}</t></is></c>", idx, dr["ADDRESS"]));
                builder.Append(string.Format("<c r=\"F{0}\" s=\"3\"  t=\"inlineStr\"><is><t>{1}</t></is></c>", idx, dr["CITY"]));
                builder.Append(string.Format("<c r=\"G{0}\" s=\"3\"  t=\"inlineStr\"><is><t>{1}</t></is></c>", idx, dr["OBJECTTYPEDESC"]));
                builder.Append(string.Format("<c r=\"H{0}\" s=\"3\"  t=\"inlineStr\"><is><t>{1}</t></is></c>", idx, dr["LOCATIONTYPE"]));
                builder.Append(string.Format("<c r=\"I{0}\" s=\"3\"  t=\"inlineStr\"><is><t>{1}</t></is></c>", idx, dr["STATUSCODEKEY"]));
                builder.Append(string.Format("<c r=\"J{0}\" s=\"3\"  t=\"inlineStr\"><is><t>{1}</t></is></c>", idx, dr["STATUSDESC"]));
                builder.Append(string.Format("<c r=\"K{0}\" s=\"3\"  t=\"inlineStr\"><is><t>{1}</t></is></c>", idx, dr["SHAREDCOMMENTS"]));
                builder.Append(string.Format("<c r=\"L{0}\" s=\"3\"  t=\"inlineStr\"><is><t>{1}</t></is></c>", idx, dr["STARTDATE"]));



                builder.Append(string.Format("<c r=\"M{0}\" s=\"3\"  t=\"inlineStr\"><is><t>{1}</t></is></c>", idx, dr["STARTTIME"]));
                builder.Append(string.Format("<c r=\"N{0}\" s=\"3\"  t=\"inlineStr\"><is><t>{1}</t></is></c>", idx, dr["SERVICETEAM"]));
                builder.Append(string.Format("<c r=\"O{0}\" s=\"3\"  t=\"inlineStr\"><is><t>{1}</t></is></c>", idx, dr["ENDDATE"]));
                builder.Append(string.Format("<c r=\"P{0}\" s=\"3\"  t=\"inlineStr\"><is><t>{1}</t></is></c>", idx, dr["ENDTIME"]));
                builder.Append(string.Format("<c r=\"Q{0}\" s=\"3\"  t=\"inlineStr\"><is><t>{1}</t></is></c>", idx, dr["PROBLEMBUCKET"]));
                builder.Append(string.Format("<c r=\"R{0}\" s=\"3\"><v>{1}</v></c>", idx, dr["DURATION"]));

                builder.Append(string.Format("<c r=\"S{0}\" s=\"3\"  t=\"inlineStr\"><is><t>{1}</t></is></c>", idx, dr["REFNO"]));



                //builder.Append(string.Format("<c r=\"E{0}\" s=\"157\"><v>{1}</v></c>", idx, int.Parse(dr["cassette1_denomination"].ToString()) * int.Parse(dr["cassette1_suggested_notes"].ToString())));
                //builder.Append(string.Format("<c r=\"F{0}\" s=\"157\"><v>{1}</v></c>", idx, int.Parse(dr["cassette2_denomination"].ToString()) * int.Parse(dr["cassette2_suggested_notes"].ToString())));
                //builder.Append(string.Format("<c r=\"G{0}\" s=\"157\"><v>{1}</v></c>", idx, int.Parse(dr["cassette3_denomination"].ToString()) * int.Parse(dr["cassette3_suggested_notes"].ToString())));
                //builder.Append(string.Format("<c r=\"H{0}\" s=\"157\"><v>{1}</v></c>", idx, int.Parse(dr["cassette4_denomination"].ToString()) * int.Parse(dr["cassette4_suggested_notes"].ToString())));
                //builder.Append(string.Format("<c r=\"I{0}\" s=\"158\"><f>SUM(E{0}:H{0})</f><v>2600000</v></c>", idx));
                //builder.Append(string.Format("<c r=\"J{0}\" s=\"159\" t=\"inlineStr\"><is><t>{1}</t></is></c>", idx, dr["city"]));

                //if (dr["cash_order_datetime"].ToString() != dr["replenishment_datetime"].ToString())
                //{
                //    builder.Append(string.Format("<c r=\"K{0}\" s=\"156\" t=\"inlineStr\"><is><t>{1}</t></is></c>", idx, DateTime.Parse(dr["replenishment_datetime"].ToString()).ToString("dd-MMM")));
                //    builder.Append(string.Format("<c r=\"L{0}\" s=\"156\" t=\"inlineStr\"><is><t>{1}</t></is></c>", idx, DateTime.Parse(dr["replenishment_datetime"].ToString()).ToString("HH:mm")));
                //}
                //else
                //{
                //    builder.Append(string.Format("<c r=\"K{0}\" s=\"156\"/>", idx));
                //    builder.Append(string.Format("<c r=\"L{0}\" s=\"156\"/>", idx));
                //}


                builder.Append("</row>");
                idx++;
            }
            File.WriteAllText(filePath, headerWithDefaultRow + builder.ToString() + footer);
        }


        public void Update24HourAvailabilitySheet(string filePath, System.Data.DataTable dt)
        {
            StringBuilder builder = new StringBuilder();
            string headerWithDefaultRow = GenerateSheetHeaderAndDefaultSheetData(System.AppDomain.CurrentDomain.BaseDirectory + "\\24_header_sheet_row.txt").Replace("[date]", reportDate.ToString("dd-MMM-yyyy"));
            string footer = GenerateFooter(System.AppDomain.CurrentDomain.BaseDirectory + "\\24_footer.txt");
            foreach (DataRow dr in dt.Rows)
            {//c.cassette1_denomination,c.cassette1_suggested_notes

                listATMsWithIncidents.Add(dr["ATMID"].ToString());

                builder.Append(string.Format("<row r=\"{0}\" spans=\"1:43\" s=\"4\" customFormat=\"1\" ht=\"15.4\" customHeight=\"1\" x14ac:dyDescent=\"0.15\">", idx));
                builder.Append(string.Format("<c r=\"A{0}\" s=\"9\"><v>{1}</v></c>", idx, idx - 3));
                builder.Append(string.Format("<c r=\"B{0}\" s=\"10\" ><v>{1}</v></c>", idx, dr["BRCODE"]));
                builder.Append(string.Format("<c r=\"C{0}\" s=\"10\" t=\"inlineStr\"><is><t>{1}</t></is></c>", idx, dr["ATMID"]));
                builder.Append(string.Format("<c r=\"D{0}\" s=\"10\"  t=\"inlineStr\"><is><t>{1}</t></is></c>", idx, dr["ENID"]));
                builder.Append(string.Format("<c r=\"E{0}\" s=\"11\"  t=\"inlineStr\"><is><t>{1}</t></is></c>", idx, dr["LOCATION"]));
                builder.Append(string.Format("<c r=\"F{0}\" s=\"12\"  t=\"inlineStr\"><is><t>{1}</t></is></c>", idx, dr["BUSINESSAREA"]));
                builder.Append(string.Format("<c r=\"G{0}\" s=\"12\"  t=\"inlineStr\"><is><t>{1}</t></is></c>", idx, dr["CIRCLE"]));
                builder.Append(string.Format("<c r=\"H{0}\" s=\"12\"  t=\"inlineStr\"><is><t>{1}</t></is></c>", idx, dr["REGION"]));
                builder.Append(string.Format("<c r=\"I{0}\" s=\"12\"  t=\"inlineStr\"><is><t>{1}</t></is></c>", idx, dr["REMARKS"]));

                builder.Append(string.Format("<c r=\"J{0}\" s=\"25\"  ><v>{1}</v></c>", idx, dr["INSERVICEPERC"]));
                builder.Append(string.Format("<c r=\"K{0}\" s=\"14\"  ><v>{1}</v></c>", idx, dr["INSERVICEMIN"]));
                builder.Append(string.Format("<c r=\"L{0}\" s=\"13\"  ><v>{1}</v></c>", idx, dr["DOWNTIMEPERC"]));



                builder.Append(string.Format("<c r=\"M{0}\" s=\"14\"  ><v>{1}</v></c>", idx, dr["DOWNTIMEMIN"]));
                builder.Append(string.Format("<c r=\"N{0}\" s=\"13\"  ><v>{1}</v></c>", idx, dr["BRANCH_FLM_CARD_READER_PERC"]));
                builder.Append(string.Format("<c r=\"O{0}\" s=\"13\"  ><v>{1}</v></c>", idx, dr["BRANCH_FLM_DISPENSER_PERC"]));
                builder.Append(string.Format("<c r=\"P{0}\" s=\"13\"  ><v>{1}</v></c>", idx, dr["BRANCH_FLM_CASH_OUT_PERC"]));
                builder.Append(string.Format("<c r=\"Q{0}\" s=\"13\"  ><v>{1}</v></c>", idx, dr["BRANCH_FLM_SUPERVISOR_PERC"]));

                builder.Append(string.Format("<c r=\"R{0}\" s=\"13\"  ><v>{1}</v></c>", idx, dr["BRANCH_FLM_OTHER_PERC"]));
                builder.Append(string.Format("<c r=\"S{0}\" s=\"14\"  ><v>{1}</v></c>", idx, dr["BRANCH_FLM_TOTAL_PERC"]));


                builder.Append(string.Format("<c r=\"T{0}\" s=\"14\"  ><v>{1}</v></c>", idx, dr["BRANCH_FLM_CARD_READER_MIN"]));
                builder.Append(string.Format("<c r=\"U{0}\" s=\"14\"  ><v>{1}</v></c>", idx, dr["BRANCH_FLM_DISPENSER_MIN"]));
                builder.Append(string.Format("<c r=\"V{0}\" s=\"14\"  ><v>{1}</v></c>", idx, dr["BRANCH_FLM_CASH_OUT_MIN"]));
                builder.Append(string.Format("<c r=\"W{0}\" s=\"14\"  ><v>{1}</v></c>", idx, dr["BRANCH_FLM_SUPERVISOR_MIN"]));
                builder.Append(string.Format("<c r=\"X{0}\" s=\"14\"  ><v>{1}</v></c>", idx, dr["BRANCH_FLM_OTHER_MIN"]));

                builder.Append(string.Format("<c r=\"Y{0}\" s=\"14\"  ><v>{1}</v></c>", idx, dr["BRANCH_FLM_TOTAL_MIN"]));


                builder.Append(string.Format("<c r=\"Z{0}\" s=\"13\"  ><v>{1}</v></c>", idx, dr["VENDOR_SLM_CARD_READER_PERC"]));


                //
                builder.Append(string.Format("<c r=\"AA{0}\" s=\"13\" ><v>{1}</v></c>", idx, dr["VENDOR_SLM_DISPENSER_PERC"]));

                builder.Append(string.Format("<c r=\"AB{0}\" s=\"13\"  ><v>{1}</v></c>", idx, dr["VENDOR_SLM_OTHER_PERC"]));



                builder.Append(string.Format("<c r=\"AC{0}\" s=\"13\"  ><v>{1}</v></c>", idx, dr["VENDOR_SLM_TOTAL_PERC"]));
                builder.Append(string.Format("<c r=\"AD{0}\" s=\"14\"  ><v>{1}</v></c>", idx, dr["VENDOR_SLM_CARD_READER_MIN"]));
                builder.Append(string.Format("<c r=\"AE{0}\" s=\"14\"  ><v>{1}</v></c>", idx, dr["VENDOR_SLM_DISPENSER_MIN"]));

                builder.Append(string.Format("<c r=\"AF{0}\" s=\"14\"  ><v>{1}</v></c>", idx, dr["VENDOR_SLM_OTHER_MIN"]));
                builder.Append(string.Format("<c r=\"AG{0}\" s=\"14\"  ><v>{1}</v></c>", idx, dr["VENDOR_SLM_TOTAL_MIN"]));
                builder.Append(string.Format("<c r=\"AH{0}\" s=\"13\"  ><v>{1}</v></c>", idx, dr["HOST_DOWN_PERC"]));



                builder.Append(string.Format("<c r=\"AI{0}\" s=\"14\"  ><v>{1}</v></c>", idx, dr["HOST_DOWN_MIN"]));
                builder.Append(string.Format("<c r=\"AJ{0}\" s=\"15\"  ><v>{1}</v></c>", idx, dr["POWER_DOWN_PERC"]));
                builder.Append(string.Format("<c r=\"AK{0}\" s=\"13\"  ><v>{1}</v></c>", idx, dr["COMMAND_FAIL_PERC"].ToString()));
                builder.Append(string.Format("<c r=\"AL{0}\" s=\"14\"  ><v>{1}</v></c>", idx, dr["TOTAL_PERC"]));


                builder.Append(string.Format("<c r=\"AM{0}\" s=\"16\"  ><v>{1}</v></c>", idx, dr["POWER_DOWN_MIN"]));
                builder.Append(string.Format("<c r=\"AN{0}\" s=\"14\"  ><v>{1}</v></c>", idx, dr["COMMAND_FAIL_MIN"]));
                builder.Append(string.Format("<c r=\"AO{0}\" s=\"15\"  ><v>{1}</v></c>", idx, dr["TOTAL_MIN"]));


                builder.Append(string.Format("<c r=\"AP{0}\" s=\"13\"  ><v>{1}</v></c>", idx, dr["VANDALISM_PERC"]));
                builder.Append(string.Format("<c r=\"AQ{0}\" s=\"14\"  ><v>{1}</v></c>", idx, dr["VANDALISM_MIN"]));



                builder.Append("</row>");
                idx++;

            }
            //Removev for timebeing

            for (int j = 0; j < listATMsInDb.Count; j++)
            {
                //SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                //SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                //DataTable dtATMsin = new DataTable();
                //adapter.Fill(dt);

                if (!listATMsWithIncidents.Contains(listATMsInDb[j].Title.ToString()))
                {



                    builder.Append(string.Format("<row r=\"{0}\" spans=\"1:43\" s=\"4\" customFormat=\"1\" ht=\"15.4\" customHeight=\"1\" x14ac:dyDescent=\"0.15\">", idx));
                    builder.Append(string.Format("<c r=\"A{0}\" s=\"9\"><v>{1}</v></c>", idx, idx - 3));
                    builder.Append(string.Format("<c r=\"B{0}\" s=\"10\"><v>{1}</v></c>", idx, listATMsInDb[j].ImBranchCode));
                    builder.Append(string.Format("<c r=\"C{0}\" s=\"10\" t=\"inlineStr\"><is><t>{1}</t></is></c>", idx, listATMsInDb[j].Title));
                    builder.Append(string.Format("<c r=\"D{0}\" s=\"10\"  t=\"inlineStr\"><is><t>{1}</t></is></c>", idx, listATMsInDb[j].ImEnId));
                    builder.Append(string.Format("<c r=\"E{0}\" s=\"11\"  t=\"inlineStr\"><is><t>{1}</t></is></c>", idx, listATMsInDb[j].ImLocation));
                    builder.Append(string.Format("<c r=\"F{0}\" s=\"12\"  t=\"inlineStr\"><is><t>{1}</t></is></c>", idx, listATMsInDb[j].ImBusinessArea));
                    builder.Append(string.Format("<c r=\"G{0}\" s=\"12\"  t=\"inlineStr\"><is><t>{1}</t></is></c>", idx, listATMsInDb[j].ImCircle));
                    builder.Append(string.Format("<c r=\"H{0}\" s=\"12\"  t=\"inlineStr\"><is><t>{1}</t></is></c>", idx, listATMsInDb[j].ImCircle));
                    builder.Append(string.Format("<c r=\"I{0}\" s=\"12\"  t=\"inlineStr\"><is><t>{1}</t></is></c>", idx, ""));

                    builder.Append(string.Format("<c r=\"J{0}\" s=\"25\" ><v>{1}</v></c>", idx, "1"));
                    builder.Append(string.Format("<c r=\"K{0}\" s=\"14\" ><v>{1}</v></c>", idx, "1440"));
                    builder.Append(string.Format("<c r=\"L{0}\" s=\"13\" ><v>{1}</v></c>", idx, "0"));
                    builder.Append(string.Format("<c r=\"M{0}\" s=\"14\" ><v>{1}</v></c>", idx, "0"));
                    //*************************************************************************************************************************************************
                    //Branch Related Downtime FLM%
                    //*************************************************************************************************************************************************
                    builder.Append(string.Format("<c r=\"N{0}\" s=\"13\"><v>{1}</v></c>", idx, "0"));
                    builder.Append(string.Format("<c r=\"O{0}\" s=\"13\"><v>{1}</v></c>", idx, "0"));
                    builder.Append(string.Format("<c r=\"P{0}\" s=\"13\"><v>{1}</v></c>", idx, "0"));
                    builder.Append(string.Format("<c r=\"Q{0}\" s=\"13\"><v>{1}</v></c>", idx, "0"));
                    builder.Append(string.Format("<c r=\"R{0}\" s=\"13\"><v>{1}</v></c>", idx, "0"));
                    builder.Append(string.Format("<c r=\"S{0}\" s=\"14\"><v>{1}</v></c>", idx, "0"));


                    //*************************************************************************************************************************************************
                    //Branch Related Downtime FLM Minutes
                    //*************************************************************************************************************************************************

                    builder.Append(string.Format("<c r=\"T{0}\" s=\"14\"  ><v>{1}</v></c>", idx, 0));
                    builder.Append(string.Format("<c r=\"U{0}\" s=\"14\"  ><v>{1}</v></c>", idx, 0));
                    builder.Append(string.Format("<c r=\"V{0}\" s=\"14\"  ><v>{1}</v></c>", idx, 0));
                    builder.Append(string.Format("<c r=\"W{0}\" s=\"14\"  ><v>{1}</v></c>", idx, 0));
                    builder.Append(string.Format("<c r=\"X{0}\" s=\"14\"  ><v>{1}</v></c>", idx, 0));
                    builder.Append(string.Format("<c r=\"Y{0}\" s=\"14\"  ><v>{1}</v></c>", idx, 0));

                    //*************************************************************************************************************************************************
                    //Vendor Related Downtime SLM%
                    //*************************************************************************************************************************************************

                    builder.Append(string.Format("<c r=\"Z{0}\" s=\"13\"  ><v>{1}</v></c>", idx, "0"));
                    builder.Append(string.Format("<c r=\"AA{0}\" s=\"13\" ><v>{1}</v></c>", idx, "0"));
                    builder.Append(string.Format("<c r=\"AB{0}\" s=\"13\" ><v>{1}</v></c>", idx, "0"));
                    builder.Append(string.Format("<c r=\"AC{0}\" s=\"13\" ><v>{1}</v></c>", idx, "0"));

                    //*************************************************************************************************************************************************
                    //Vendor Related Downtime SLM Minutes%
                    //*************************************************************************************************************************************************

                    builder.Append(string.Format("<c r=\"AD{0}\" s=\"14\" ><v>{1}</v></c>", idx, 0));
                    builder.Append(string.Format("<c r=\"AE{0}\" s=\"14\" ><v>{1}</v></c>", idx, 0));
                    builder.Append(string.Format("<c r=\"AF{0}\" s=\"14\" ><v>{1}</v></c>", idx, 0));
                    builder.Append(string.Format("<c r=\"AG{0}\" s=\"14\" ><v>{1}</v></c>", idx, 0));

                    //*************************************************************************************************************************************************
                    //Vendor Related Downtime SLM Minutes%
                    //*************************************************************************************************************************************************

                    builder.Append(string.Format("<c r=\"AH{0}\" s=\"13\" ><v>{1}</v></c>", idx, "0"));
                    builder.Append(string.Format("<c r=\"AI{0}\" s=\"14\"><v>{1}</v></c>", idx, 0));
                    //*************************************************************************************************************************************************
                    //Comms Down%
                    //*************************************************************************************************************************************************

                    builder.Append(string.Format("<c r=\"AJ{0}\" s=\"15\" ><v>{1}</v></c>", idx, "0"));
                    builder.Append(string.Format("<c r=\"AK{0}\" s=\"13\" ><v>{1}</v></c>", idx, "0"));
                    builder.Append(string.Format("<c r=\"AL{0}\" s=\"14\" ><v>{1}</v></c>", idx, "0"));
                    //*************************************************************************************************************************************************
                    //Comms Down Min
                    //*************************************************************************************************************************************************

                    builder.Append(string.Format("<c r=\"AM{0}\" s=\"16\" ><v>{1}</v></c>", idx, 0));
                    builder.Append(string.Format("<c r=\"AN{0}\" s=\"14\" ><v>{1}</v></c>", idx, 0));
                    builder.Append(string.Format("<c r=\"AO{0}\" s=\"15\" ><v>{1}</v></c>", idx, 0));

                    //*************************************************************************************************************************************************
                    //Vandalism%
                    //*************************************************************************************************************************************************
                    builder.Append(string.Format("<c r=\"AP{0}\" s=\"13\" ><v>{1}</v></c>", idx, "0"));
                    builder.Append(string.Format("<c r=\"AQ{0}\" s=\"14\" ><v>{1}</v></c>", idx, 0));



                    builder.Append("</row>");
                    idx++;

                }
            }
            //Add formula row...
            builder.Append(string.Format("<row r=\"{0}\" spans=\"1:43\" x14ac:dyDescent=\"0.2\">", idx));

            builder.Append(string.Format("<c r=\"J{0}\" s=\"18\" ><f>AVERAGE({1}:{2})</f></c>", idx, "J4", "J" + (idx - 1)));

            builder.Append(string.Format("<c r=\"K{0}\" s=\"19\" ><f>SUM({1}:{2})</f></c>", idx, "K4", "K" + (idx - 1)));
            builder.Append(string.Format("<c r=\"L{0}\" s=\"18\" ><f>AVERAGE({1}:{2})</f></c>", idx, "L4", "L" + (idx - 1)));
            builder.Append(string.Format("<c r=\"M{0}\" s=\"19\" ><f>SUM({1}:{2})</f></c>", idx, "M4", "M" + (idx - 1)));
            //*************************************************************************************************************************************************
            //Branch Related Downtime FLM%
            //*************************************************************************************************************************************************
            builder.Append(string.Format("<c r=\"N{0}\" s=\"18\"><f>AVERAGE({1}:{2})</f></c>", idx, "N4", "N" + (idx - 1)));
            builder.Append(string.Format("<c r=\"O{0}\" s=\"18\"><f>AVERAGE({1}:{2})</f></c>", idx, "O4", "O" + (idx - 1)));
            builder.Append(string.Format("<c r=\"P{0}\" s=\"18\"><f>AVERAGE({1}:{2})</f></c>", idx, "P4", "P" + (idx - 1)));
            builder.Append(string.Format("<c r=\"Q{0}\" s=\"18\"><f>AVERAGE({1}:{2})</f></c>", idx, "Q4", "Q" + (idx - 1)));
            builder.Append(string.Format("<c r=\"R{0}\" s=\"18\"><f>AVERAGE({1}:{2})</f></c>", idx, "R4", "R" + (idx - 1)));
            builder.Append(string.Format("<c r=\"S{0}\" s=\"18\"><f>SUM({1}:{2})</f></c>", idx, "N" + idx, "R" + idx));


            //*************************************************************************************************************************************************
            //Branch Related Downtime FLM Minutes
            //*************************************************************************************************************************************************

            builder.Append(string.Format("<c r=\"T{0}\" s=\"19\"  ><f>SUM({1}:{2})</f></c>", idx, "T4", "T" + (idx - 1)));
            builder.Append(string.Format("<c r=\"U{0}\" s=\"19\"  ><f>SUM({1}:{2})</f></c>", idx, "U4", "U" + (idx - 1)));
            builder.Append(string.Format("<c r=\"V{0}\" s=\"19\"  ><f>SUM({1}:{2})</f></c>", idx, "V4", "V" + (idx - 1)));
            builder.Append(string.Format("<c r=\"W{0}\" s=\"19\"  ><f>SUM({1}:{2})</f></c>", idx, "W4", "W" + (idx - 1)));
            builder.Append(string.Format("<c r=\"X{0}\" s=\"19\"  ><f>SUM({1}:{2})</f></c>", idx, "X4", "X" + (idx - 1)));
            builder.Append(string.Format("<c r=\"Y{0}\" s=\"19\"  ><f>SUM({1}:{2})</f></c>", idx, "Y4", "Y" + (idx - 1)));

            //*************************************************************************************************************************************************
            //Vendor Related Downtime SLM%
            //*************************************************************************************************************************************************

            builder.Append(string.Format("<c r=\"Z{0}\" s=\"18\"  ><f>AVERAGE({1}:{2})</f></c>", idx, "Z4", "Z" + (idx - 1)));
            builder.Append(string.Format("<c r=\"AA{0}\" s=\"18\" ><f>AVERAGE({1}:{2})</f></c>", idx, "AA4", "AA" + (idx - 1)));
            builder.Append(string.Format("<c r=\"AB{0}\" s=\"18\" ><f>AVERAGE({1}:{2})</f></c>", idx, "AB4", "AB" + (idx - 1)));
            builder.Append(string.Format("<c r=\"AC{0}\" s=\"18\" ><f>SUM({1}:{2})</f></c>", idx, "Z" + idx, "AB" + idx));

            //*************************************************************************************************************************************************
            //Vendor Related Downtime SLM Minutes%
            //*************************************************************************************************************************************************

            builder.Append(string.Format("<c r=\"AD{0}\" s=\"19\" ><f>SUM({1}:{2})</f></c>", idx, "AD4", "AD" + (idx - 1)));
            builder.Append(string.Format("<c r=\"AE{0}\" s=\"19\" ><f>SUM({1}:{2})</f></c>  ", idx, "AE4", "AE" + (idx - 1)));
            builder.Append(string.Format("<c r=\"AF{0}\" s=\"19\" ><f>SUM({1}:{2})</f></c>", idx, "AF4", "AF" + (idx - 1)));
            builder.Append(string.Format("<c r=\"AG{0}\" s=\"19\" ><f>SUM({1}:{2})</f></c>", idx, "AD" + idx, "AF" + idx));

            //*************************************************************************************************************************************************
            //Vendor Related Downtime SLM Minutes%
            //*************************************************************************************************************************************************

            builder.Append(string.Format("<c r=\"AH{0}\" s=\"18\" ><f>AVERAGE({1}:{2})</f></c>", idx, "AH4", "AH" + (idx - 1)));
            builder.Append(string.Format("<c r=\"AI{0}\" s=\"19\"><f>SUM({1}:{2})</f></c>", idx, "AI4", "AI" + (idx - 1)));
            //*************************************************************************************************************************************************
            //Comms Down%
            //*************************************************************************************************************************************************

            builder.Append(string.Format("<c r=\"AJ{0}\" s=\"19\" ><f>SUM({1}:{2})</f></c>", idx, "AJ4", "AJ" + (idx - 1)));
            builder.Append(string.Format("<c r=\"AK{0}\" s=\"18\" ><f>AVERAGE({1}:{2})</f></c>", idx, "AK4", "AK" + (idx - 1)));
            builder.Append(string.Format("<c r=\"AL{0}\" s=\"18\" ><f>AVERAGE({1}:{2})</f></c>", idx, "AL4", "AL" + (idx - 1)));
            //*************************************************************************************************************************************************
            //Comms Down Min
            //*************************************************************************************************************************************************

            builder.Append(string.Format("<c r=\"AM{0}\" s=\"19\" ><f>SUM({1}:{2})</f></c>", idx, "AM4", "AM" + (idx - 1)));
            builder.Append(string.Format("<c r=\"AN{0}\" s=\"19\" ><f>SUM({1}:{2})</f></c>", idx, "AN4", "AN" + (idx - 1)));
            builder.Append(string.Format("<c r=\"AO{0}\" s=\"19\" ><f>SUM({1}:{2})</f></c>", idx, "AO4", "AO" + (idx - 1)));

            //*************************************************************************************************************************************************
            //Vandalism%
            //*************************************************************************************************************************************************
            builder.Append(string.Format("<c r=\"AP{0}\" s=\"18\" ><f>SUM({1}:{2})</f></c>", idx, "AP4", "AP" + (idx - 1)));
            builder.Append(string.Format("<c r=\"AQ{0}\" s=\"19\" ><f>SUM({1}:{2})</f></c>", idx, "AQ4", "AQ" + (idx - 1)));



            builder.Append("</row>");

            File.WriteAllText(filePath, headerWithDefaultRow + builder.ToString() + footer);
        }

        public void UpdateWorkSheet(string filePath)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filePath);
            xmlDoc.ChildNodes[1].RemoveChild(xmlDoc.ChildNodes[1].ChildNodes[0]);
            xmlDoc.Save(filePath);
        }
        public void ZipFolder(string basePath)
        {
            zipFolder(basePath, false);
        }
        public void BuildList(string SearchPath, int RecursionLevel)
        {
            //new ArrayList();
            DirectoryInfo ThisLevel = new DirectoryInfo(SearchPath);
            DirectoryInfo[] ChildLevel = ThisLevel.GetDirectories();
            if (RecursionLevel != 1)
            {
                foreach (DirectoryInfo Child in ChildLevel)
                {
                    BuildList(Child.FullName, RecursionLevel - 1);
                }
            }
            FileInfo[] ChildFiles = ThisLevel.GetFiles();
            foreach (FileInfo ChildFile in ChildFiles)
            {
                _Files.Add(ChildFile.FullName);
            }
        }
        public void zipFolder(string basePath, bool deleteAfter)
        {
            string zipFileName = basePath + ".zip";
            ZipOutputStream os = new ZipOutputStream(File.OpenWrite(zipFileName));

            FileStream fs;

            _Files = new ArrayList();
            BuildList(basePath, 0);

            foreach (string s in _Files)
            {
                string sourceFileName = s;

                //string path = Path.GetFileName(sourceFileName);
                string path = Path.GetFullPath(sourceFileName).Replace(basePath + "\\", "");
                //string path1 = Path.GetExtension(path2);

                ZipEntry ze = new ZipEntry(path);
                ze.CompressionMethod = CompressionMethod.Deflated;
                os.PutNextEntry(ze);

                fs = File.OpenRead(sourceFileName);

                byte[] buff = new byte[1024];
                int n = 0;
                while ((n = fs.Read(buff, 0, buff.Length)) > 0)
                {
                    os.Write(buff, 0, n);

                }
                fs.Close();
            }

            os.CloseEntry();
            os.Close();

            if (deleteAfter)
            {
                Directory.Delete(basePath, true);
            }
        }
        public void ExpandFolder(string zipFile, string baseFolder)
        {
            if (!Directory.Exists(baseFolder))
            {
                Directory.CreateDirectory(baseFolder);
            }
            FileStream fr = File.OpenRead(zipFile);
            ZipInputStream ins = new ZipInputStream(fr);
            //ZipFile zf = new ZipFile(zipFile);
            ZipEntry ze = ins.GetNextEntry();
            while (ze != null)
            {
                if (ze.IsDirectory)
                {
                    Directory.CreateDirectory(baseFolder + "\\" + ze.Name);
                }
                else if (ze.IsFile)
                {
                    if (!Directory.Exists(baseFolder + "\\" + Path.GetDirectoryName(ze.Name)))
                    {
                        Directory.CreateDirectory(baseFolder + "\\" + Path.GetDirectoryName(ze.Name));
                    }

                    FileStream fs = File.Create(baseFolder + "\\" + ze.Name);

                    byte[] writeData = new byte[ze.Size];
                    int iteration = 0;
                    while (true)
                    {
                        int size = 2048;
                        size = ins.Read(writeData, (int)Math.Min(ze.Size, (iteration * 2048)), (int)Math.Min(ze.Size - (int)Math.Min(ze.Size, (iteration * 2048)), 2048));
                        if (size > 0)
                        {
                            fs.Write(writeData, (int)Math.Min(ze.Size, (iteration * 2048)), size);
                        }
                        else
                        {
                            break;
                        }
                        iteration++;
                    }
                    fs.Close();
                }
                ze = ins.GetNextEntry();
            }
            ins.Close();
            fr.Close();
        }

        public DataTable GetGasperTicketSchema()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ATMID", typeof(string));
            dt.Columns.Add("ID2", typeof(string));
            dt.Columns.Add("TICKETID", typeof(string));
            dt.Columns.Add("BRANCHNAME", typeof(string));
            dt.Columns.Add("ADDRESS", typeof(string));
            dt.Columns.Add("CITY", typeof(string));
            dt.Columns.Add("OBJECTTYPEDESC", typeof(string));
            dt.Columns.Add("LOCATIONTYPE", typeof(string));
            dt.Columns.Add("STATUSCODEKEY", typeof(string));
            dt.Columns.Add("STATUSDESC", typeof(string));
            dt.Columns.Add("SHAREDCOMMENTS", typeof(string));
            dt.Columns.Add("STARTDATE", typeof(string));
            dt.Columns.Add("STARTTIME", typeof(string));
            dt.Columns.Add("SERVICETEAM", typeof(string));
            dt.Columns.Add("ENDDATE", typeof(string));
            dt.Columns.Add("ENDTIME", typeof(string));
            dt.Columns.Add("PROBLEMBUCKET", typeof(string));
            dt.Columns.Add("DURATION", typeof(decimal));
            dt.Columns.Add("REFNO", typeof(string));
            dt.Columns.Add("actual_duration", typeof(decimal));
            dt.Columns.Add("ticket_start_datetime", typeof(DateTime));
            dt.Columns.Add("ticket_end_datetime", typeof(DateTime));
            dt.Columns.Add("problem_priority", typeof(int));
            dt.Columns.Add("category", typeof(string));
            dt.Columns.Add("problem_desc", typeof(string));

            dt.Columns.Add("im_branch_code", typeof(string));
            dt.Columns.Add("im_en_id", typeof(string));
            dt.Columns.Add("im_location", typeof(string));
            dt.Columns.Add("im_business_area", typeof(string));
            dt.Columns.Add("im_circle", typeof(string));


            return dt;

        }


        public DataTable Get24HourAvailabilitySchema()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("SNO", typeof(string));
            dt.Columns.Add("BRCODE", typeof(string));
            dt.Columns.Add("ATMID", typeof(string));
            dt.Columns.Add("ENID", typeof(string));
            dt.Columns.Add("LOCATION", typeof(string));
            dt.Columns.Add("BUSINESSAREA", typeof(string));
            dt.Columns.Add("CIRCLE", typeof(string));
            dt.Columns.Add("REGION", typeof(string));
            dt.Columns.Add("REMARKS", typeof(string));
            dt.Columns.Add("INSERVICEPERC", typeof(string));
            dt.Columns.Add("INSERVICEMIN", typeof(string));
            dt.Columns.Add("DOWNTIMEPERC", typeof(string));
            dt.Columns.Add("DOWNTIMEMIN", typeof(string));
            dt.Columns.Add("BRANCH_FLM_CARD_READER_PERC", typeof(string));
            dt.Columns.Add("BRANCH_FLM_DISPENSER_PERC", typeof(string));
            dt.Columns.Add("BRANCH_FLM_CASH_OUT_PERC", typeof(string));
            dt.Columns.Add("BRANCH_FLM_SUPERVISOR_PERC", typeof(string));
            dt.Columns.Add("BRANCH_FLM_OTHER_PERC", typeof(string));
            dt.Columns.Add("BRANCH_FLM_TOTAL_PERC", typeof(string));

            dt.Columns.Add("BRANCH_FLM_CARD_READER_MIN", typeof(string));
            dt.Columns.Add("BRANCH_FLM_DISPENSER_MIN", typeof(string));
            dt.Columns.Add("BRANCH_FLM_CASH_OUT_MIN", typeof(string));
            dt.Columns.Add("BRANCH_FLM_SUPERVISOR_MIN", typeof(string));
            dt.Columns.Add("BRANCH_FLM_OTHER_MIN", typeof(string));
            dt.Columns.Add("BRANCH_FLM_TOTAL_MIN", typeof(string));

            //
            dt.Columns.Add("VENDOR_SLM_CARD_READER_PERC", typeof(string));
            dt.Columns.Add("VENDOR_SLM_DISPENSER_PERC", typeof(string));
            dt.Columns.Add("VENDOR_SLM_OTHER_PERC", typeof(string));
            dt.Columns.Add("VENDOR_SLM_TOTAL_PERC", typeof(string));

            dt.Columns.Add("VENDOR_SLM_CARD_READER_MIN", typeof(string));
            dt.Columns.Add("VENDOR_SLM_DISPENSER_MIN", typeof(string));
            dt.Columns.Add("VENDOR_SLM_OTHER_MIN", typeof(string));
            dt.Columns.Add("VENDOR_SLM_TOTAL_MIN", typeof(string));


            dt.Columns.Add("HOST_DOWN_PERC", typeof(string));
            dt.Columns.Add("HOST_DOWN_MIN", typeof(string));

            dt.Columns.Add("POWER_DOWN_PERC", typeof(string));
            dt.Columns.Add("COMMAND_FAIL_PERC", typeof(string));
            dt.Columns.Add("TOTAL_PERC", typeof(string));

            dt.Columns.Add("POWER_DOWN_MIN", typeof(string));
            dt.Columns.Add("COMMAND_FAIL_MIN", typeof(string));
            dt.Columns.Add("TOTAL_MIN", typeof(string));

            dt.Columns.Add("VANDALISM_PERC", typeof(string));
            dt.Columns.Add("VANDALISM_MIN", typeof(string));


            //


            return dt;

        }
        private string GetFieldValByName(DataTable dt, string problem, string columnName)
        {
            string val = null;
            try
            {
                val = dt.Select("problem_desc='" + problem + "'")[0][columnName].ToString();
            }
            catch (Exception ex)
            {
                int a = 0;
            }
            return val;
        }
        private string GetProblemBucket(DataTable dt, string problem)
        {
            string result = null;
            if (problem.IndexOf("New Incident") > -1)
            {
                problem = problem.Replace("New Incident:", "");
                if (problem.Length > 50)
                    problem = problem.Substring(0, 50);
            }
            if (problem.IndexOf("ATM ID") > 0)
                problem = problem.Substring(0, problem.IndexOf(",  ATM ID"));

            if (problem.Length > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["problem_bucket_desc"].ToString().IndexOf(problem) >= 0)
                    {
                        result = dt.Rows[i]["problem_desc"].ToString();
                        break;
                    }
                }
            }

            return result;
        }


        private DataTable GetTicketHistoryDataRows(DateTime dateTime)
        {
            DataTable gasperDataTable = GetGasperTicketSchema();
            DateTime ticketStartDateTime = DateTime.MinValue;
            DateTime ticketEndDateTime = DateTime.MinValue;
            //bool resolvedAtRead = false;

            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            //  cmd.CommandText = @"select 'CM1' ATMID, '2' ID2, '3' TICKETID,'4' BRANCHNAME,'5' ADDRESS,'6' CITY,'7' OBJECTTYPEDESC,'8' LOCATIONTYPE,'9' STATUSCODEKEY,'10' STATUSDESC,'11' SHAREDCOMMENTS,'12' STARTDATE, '13' STARTTIME,'14' SERVICETEAM,'15' ENDDATE,'16' ENDTIME,'17' PROBLEMBUCKET,'18' DURATION, '19' REFNO FROM ATM WHERE IS_ACTIVE=1";
            cmd.CommandText = @"select * from incident inner join incident_detail on incident.incident_id = incident_detail.incident_id
                                inner join atm_alert on atm_alert.atm_alert_id = incident.atm_alert_id
                                inner join atm on atm.atm_id = atm_alert.atm_id
                                where atm_alert.resolve_at >=CONVERT(datetime,'" + dateTime.ToString("dd/MM/yyyy") + "',103) and atm_alert.resolve_at <=CONVERT(datetime,'" + dateTime.ToString("dd/MM/yyyy") + " 23:59:59',103) and atm.is_active=1 ORDER BY ATM.ATM_ID,atm_alert.GENERATED_AT";
            //where  incident.creation_time <=CONVERT(datetime,'" + dateTime.ToString("dd/MM/yyyy") + " 23:59:59',103) and atm.is_active=1";
            //incident.creation_time >=CONVERT(datetime,'" + dateTime.ToString("dd/MM/yyyy") + "',103) and
            //cmd.CommandText = @"select 'CM1' ATMID, '2' ID2, '3' TICKETID,'4' BRANCHNAME,'5' ADDRESS,'6' CITY,'7' OBJECTTYPEDESC,'8' LOCATIONTYPE,'9' STATUSCODEKEY,'10' STATUSDESC,'11' SHAREDCOMMENTS,'12' STARTDATE, '13' STARTTIME,'14' SERVICETEAM,'15' ENDDATE,'16' ENDTIME,'17' PROBLEMBUCKET,'18' DURATION, '19' REFNO FROM ATM WHERE IS_ACTIVE=1";
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            System.Data.DataTable dt = new System.Data.DataTable();
            adapter.Fill(dt);


            //       System.Data.DataTable dt = (System.Data.DataTable)Cache["dtCashOrders"];
            if (dt.Rows.Count > 0)
            {
                cmd.CommandText = "select * from problem_bucket_priority inner join problem_bucket_desc on problem_bucket_priority.problem_bucket_priority_id = problem_bucket_desc.problem_bucket_priority_id";
                adapter = new SqlDataAdapter(cmd);
                helperDt = new System.Data.DataTable();
                adapter.Fill(helperDt);


                DataTable dtAlerts = dt.DefaultView.ToTable(true, new string[] { "atm_alert_id" });
                for (int i = 0; i < dtAlerts.Rows.Count; i++)
                {
                    //resolvedAtRead = false;
                    DataRow newRow = gasperDataTable.NewRow();
                    DataRow[] drArray = dt.Select("atm_alert_id=" + dtAlerts.Rows[i][0]);

                    //if (dtAlerts.Rows[i][0].ToString() == "19694294")
                    //{
                    //    int b = 0;
                    //}

                    for (int j = 0; j < drArray.Length; j++)
                    {
                        if (drArray[j]["token_name"].ToString() == "Escalation")
                        {
                            newRow["actual_duration"] = 0;
                            newRow["STATUSDESC"] = drArray[j]["token_value"];
                            newRow["PROBLEMBUCKET"] = GetProblemBucket(helperDt, drArray[j]["token_value"].ToString());
                            if (newRow["PROBLEMBUCKET"].ToString().Length > 0)
                            {
                                newRow["problem_priority"] = GetFieldValByName(helperDt, newRow["PROBLEMBUCKET"].ToString(), "problem_priority");
                                if (newRow["PROBLEMBUCKET"].ToString().Length > 0)

                                    newRow["category"] = GetFieldValByName(helperDt, newRow["PROBLEMBUCKET"].ToString(), "category");
                                newRow["problem_desc"] = GetFieldValByName(helperDt, newRow["PROBLEMBUCKET"].ToString(), "problem_desc");
                            }
                        }
                        else if (drArray[j]["token_name"].ToString() == "ATMID")
                            newRow["ATMID"] = drArray[j]["token_value"];
                        else if (drArray[j]["token_name"].ToString() == "Ticket Start")
                        {
                            ticketStartDateTime = DateTime.ParseExact(drArray[j]["token_value"].ToString(), "dd/MM/yyyy HH:mm:ss", null);
                            newRow["STARTDATE"] = ticketStartDateTime.ToString("yyyy-MM-dd");
                            newRow["STARTTIME"] = ticketStartDateTime.ToString("HH:mm:ss");
                            newRow["ticket_start_datetime"] = ticketStartDateTime;
                        }
                        else if (drArray[j]["token_name"].ToString() == "Model")
                            newRow["OBJECTTYPEDESC"] = drArray[j]["token_value"];
                        //else if (drArray[j]["token_name"].ToString() == "ConnectionType")
                        //    newRow["OBJECTTYPEDESC"] = drArray[j]["token_value"];
                        else if (drArray[j]["token_name"].ToString() == "TicketNo")
                            newRow["TICKETID"] = drArray[j]["token_value"];



                        //else if (drArray[j]["token_name"].ToString() == "SerialNo")
                        //    newRow["OBJECTTYPEDESC"] = drArray[j]["token_value"];
                        //else if (drArray[j]["token_name"].ToString() == "State")
                        //    newRow["OBJECTTYPEDESC"] = drArray[j]["token_value"];
                        //else if (drArray[j]["token_name"].ToString() == "ZipCode")
                        //    newRow["OBJECTTYPEDESC"] = drArray[j]["token_value"];

                    }


                    if (drArray[0]["city"].ToString().Length > 0)
                    {
                        newRow["ADDRESS"] = drArray[0]["address1"];
                        newRow["CITY"] = drArray[0]["city"];
                    }



                    if (drArray[0]["resolve_at"] != DBNull.Value)
                    {

                        //To be changed as per country.
                        ticketEndDateTime = DateTime.Parse(drArray[0]["resolve_at"].ToString());
                        newRow["ENDDATE"] = ticketEndDateTime.ToString("yyyy-MM-dd");
                        newRow["ENDTIME"] = ticketEndDateTime.ToString("HH:mm:ss");
                        newRow["ticket_end_datetime"] = ticketEndDateTime;
                        newRow["DURATION"] = Math.Round((ticketEndDateTime - ticketStartDateTime).TotalMinutes, 2);
                        //resolvedAtRead = true;

                    }
                    newRow["im_branch_code"] = drArray[0]["im_branch_code"];
                    newRow["im_en_id"] = drArray[0]["im_en_id"];
                    newRow["im_location"] = drArray[0]["im_location"];
                    newRow["im_business_area"] = drArray[0]["im_business_area"];
                    newRow["im_circle"] = drArray[0]["im_circle"];


                    gasperDataTable.Rows.Add(newRow);

                }
            }

            return gasperDataTable;
        }
        private void GenerateTicketHistory(DateTime datetime)
        {
            DataTable dt = GetTicketHistoryDataRows(datetime);
            if (dt.Rows.Count > 0)
            {
                AppSetting appSetting = AppSetting.LoadAppSetting("1=1");
                //Application[ApplicationVars.AppSettings.ToString()];
                string zipFilePath = appSetting.TemporaryFolder + "\\TicketHistoryTemplate.zip";
                if (!File.Exists(zipFilePath))
                    throw new Exception("Template file does not exists.Please upload template file before generating settlement report");
                string extractFilePath = appSetting.TemporaryFolder + "\\TicketHistoryTemplate1";

                if (Directory.Exists(extractFilePath))
                    Directory.Delete(extractFilePath, true);

                if (!Directory.Exists(extractFilePath))
                    Directory.CreateDirectory(extractFilePath);

                ExpandFolder(zipFilePath, extractFilePath);

                UpdateSheet(extractFilePath + "\\xl\\worksheets\\sheet1.xml", dt);

                if (File.Exists(extractFilePath + "\\xl\\calcChain.xml"))
                    File.Delete(extractFilePath + "\\xl\\calcChain.xml");

                Avanza.CCMS.DAL.Utility.UpdateWorkSheet(extractFilePath + "\\xl\\workbook.xml");
                Avanza.CCMS.DAL.Utility.ZipFolder(extractFilePath);
                if (File.Exists(extractFilePath + ".xlsx"))
                    File.Delete(extractFilePath + ".xlsx");
                File.Move(extractFilePath + ".zip", extractFilePath + ".xlsx");
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("Content-Disposition", "attachment; filename=TicketHistory.xlsx");
                Response.AddHeader("Content-Length",File.ReadAllBytes(extractFilePath + ".xlsx").LongLength.ToString());
                Response.TransmitFile(extractFilePath + ".xlsx");
                Response.Flush();
                Response.Close();
               

            }
            else
                Literal_errorMsg.Text="No records found to generate report";
            
        }

        private void UpdateDrawingSheet(string path)
        {
            string content = File.ReadAllText(System.AppDomain.CurrentDomain.BaseDirectory + "\\24_drawing.txt");
            //Detail!J11 ->uptime
            //Detail!L11 ->downtime
            //Detail!A10 ->total atms

            content = content.Replace("Detail!J11", "Detail!J" + idx).Replace("Detail!L11", "Detail!L" + idx).Replace("Detail!A10", "Detail!A" + (idx - 1));
            File.WriteAllText(path + "\\xl\\drawings\\drawing1.xml", content);

        }

        private void UpdateChartSheet(string path)
        {
            string content = File.ReadAllText(System.AppDomain.CurrentDomain.BaseDirectory + "\\24_chart1.txt");
            content = content.Replace("Detail!$S$9,Detail!$AH$9,Detail!$AK$9", "Detail!$S$" + idx + ",Detail!$AH$" + idx + ",Detail!$AK$" + idx);
            File.WriteAllText(path + "\\xl\\charts\\chart1.xml", content);

            content = File.ReadAllText(System.AppDomain.CurrentDomain.BaseDirectory + "\\24_chart2.txt");
            content = content.Replace("Detail!$J$9,Detail!$L$9", "Detail!$J$" + idx + ",Detail!$L$" + idx);
            File.WriteAllText(path + "\\xl\\charts\\chart2.xml", content);

        }
        private void Generate24HourAvailabilityReport(DateTime datetime)
        {

            System.Data.DataTable dt = GetTicketHistoryDataRows(datetime);
            //SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            ////  cmd.CommandText = @"select 'CM1' ATMID, '2' ID2, '3' TICKETID,'4' BRANCHNAME,'5' ADDRESS,'6' CITY,'7' OBJECTTYPEDESC,'8' LOCATIONTYPE,'9' STATUSCODEKEY,'10' STATUSDESC,'11' SHAREDCOMMENTS,'12' STARTDATE, '13' STARTTIME,'14' SERVICETEAM,'15' ENDDATE,'16' ENDTIME,'17' PROBLEMBUCKET,'18' DURATION, '19' REFNO FROM ATM WHERE IS_ACTIVE=1";
            ////            cmd.CommandText = @"select * from incident inner join incident_detail on incident.incident_id = incident_detail.incident_id
            ////                                inner join atm_alert on atm_alert.atm_alert_id = incident.atm_alert_id
            ////                                inner join atm on atm.atm_id = atm_alert.atm_id
            ////                                where incident.creation_time >=CONVERT(datetime,'" + DateTime.Today.ToString("dd/MM/yyyy") + "',103)";

            //cmd.CommandText = @"select 'CM1' ATMID, '2' ID2, '3' TICKETID,'4' BRANCHNAME,'5' ADDRESS,'6' CITY,'7' OBJECTTYPEDESC,'8' LOCATIONTYPE,'9' STATUSCODEKEY,'10' STATUSDESC,'11' SHAREDCOMMENTS,'12' STARTDATE, '13' STARTTIME,'14' SERVICETEAM,'15' ENDDATE,'16' ENDTIME,'17' PROBLEMBUCKET,'18' DURATION, '19' REFNO FROM ATM WHERE IS_ACTIVE=1";
            //SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            //System.Data.DataTable dt = new System.Data.DataTable();
            //adapter.Fill(dt);


            //       System.Data.DataTable dt = (System.Data.DataTable)Cache["dtCashOrders"];
            if (dt.Rows.Count > 0)
            {
                DataTable dtATMs = dt.DefaultView.ToTable(true, new string[] { "ATMID" });
                DataTable dt24HourAvailabilityData = Get24HourAvailabilitySchema();
                for (int i = 0; i < dtATMs.Rows.Count; i++)
                {
                    //DataRow[] drTicketHistory = dt.Select("ATMID='"+dtATMs.Rows[i][0].ToString()+"'");
                    GetDowntime(dt, dtATMs.Rows[i][0].ToString());
                    DataRow[] drATM = dt.Select("ATMID = '" + dtATMs.Rows[i][0].ToString() + "'");
                    DataRow drNewRow = dt24HourAvailabilityData.NewRow();
                    drNewRow["SNO"] = i + 1;
                    drNewRow["BRCODE"] = drATM[0]["im_branch_code"];
                    drNewRow["ATMID"] = dtATMs.Rows[i]["ATMID"];
                    drNewRow["ENID"] = drATM[0]["im_en_id"];
                    drNewRow["LOCATION"] = drATM[0]["im_location"];
                    drNewRow["BUSINESSAREA"] = drATM[0]["im_business_area"];
                    drNewRow["CIRCLE"] = drATM[0]["im_circle"];
                    drNewRow["REGION"] = drATM[0]["im_circle"];
                    drNewRow["REMARKS"] = "";

                    //drNewRow["DOWNTIMEPERC"] = i + 1;
                    drNewRow["DOWNTIMEMIN"] = Math.Round(dt.Select("ATMID='" + dtATMs.Rows[i][0].ToString() + "'").AsEnumerable().Sum(r => r.Field<decimal>("actual_duration")));
                    drNewRow["INSERVICEMIN"] = Math.Round(1440 - decimal.Parse(drNewRow["DOWNTIMEMIN"].ToString()));
                    drNewRow["DOWNTIMEPERC"] = Math.Round(decimal.Parse(drNewRow["DOWNTIMEMIN"].ToString()) / 1440, 4);
                    drNewRow["INSERVICEPERC"] = Math.Round(decimal.Parse(drNewRow["INSERVICEMIN"].ToString()) / 1440, 4);




                    drNewRow["BRANCH_FLM_CARD_READER_MIN"] = Math.Round((decimal)dt.Select("ATMID='" + dtATMs.Rows[i][0].ToString() + "' and category in ('CARD READER') and problem_desc='FLM P1'").AsEnumerable().Sum(r => r.Field<decimal>("actual_duration")));//i + 1;//i + 1;;
                    drNewRow["BRANCH_FLM_CARD_READER_PERC"] = Math.Round(decimal.Parse(drNewRow["BRANCH_FLM_CARD_READER_MIN"].ToString()) / 1440, 4);

                    drNewRow["BRANCH_FLM_DISPENSER_MIN"] = Math.Round((decimal)dt.Select("ATMID='" + dtATMs.Rows[i][0].ToString() + "' and category in ('CASH DISPENSER') and problem_desc='FLM P1'").AsEnumerable().Sum(r => r.Field<decimal>("actual_duration")));//i + 1;//i + 1;;
                    drNewRow["BRANCH_FLM_DISPENSER_PERC"] = Math.Round(decimal.Parse(drNewRow["BRANCH_FLM_DISPENSER_MIN"].ToString()) / 1440, 4);

                    drNewRow["BRANCH_FLM_CASH_OUT_MIN"] = Math.Round((decimal)dt.Select("ATMID='" + dtATMs.Rows[i][0].ToString() + "' and category in ('CASH OUT') ").AsEnumerable().Sum(r => r.Field<decimal>("actual_duration")));//i + 1;//i + 1;;
                    drNewRow["BRANCH_FLM_CASH_OUT_PERC"] = Math.Round(decimal.Parse(drNewRow["BRANCH_FLM_CASH_OUT_MIN"].ToString()) / 1440, 4);

                    drNewRow["BRANCH_FLM_SUPERVISOR_MIN"] = Math.Round((decimal)dt.Select("ATMID='" + dtATMs.Rows[i][0].ToString() + "' and category in ('ATM in Supervisor') ").AsEnumerable().Sum(r => r.Field<decimal>("actual_duration")));//i + 1;//i + 1;
                    drNewRow["BRANCH_FLM_SUPERVISOR_PERC"] = Math.Round(decimal.Parse(drNewRow["BRANCH_FLM_SUPERVISOR_MIN"].ToString()) / 1440, 4);

                    drNewRow["BRANCH_FLM_OTHER_MIN"] = 0;
                    drNewRow["BRANCH_FLM_OTHER_PERC"] = 0;

                    drNewRow["BRANCH_FLM_TOTAL_MIN"] = int.Parse(drNewRow["BRANCH_FLM_CARD_READER_MIN"].ToString()) + int.Parse(drNewRow["BRANCH_FLM_DISPENSER_MIN"].ToString()) +
                                                       int.Parse(drNewRow["BRANCH_FLM_CASH_OUT_MIN"].ToString()) + int.Parse(drNewRow["BRANCH_FLM_SUPERVISOR_MIN"].ToString());

                    drNewRow["BRANCH_FLM_TOTAL_PERC"] = Math.Round((decimal.Parse(drNewRow["BRANCH_FLM_CARD_READER_MIN"].ToString()) + decimal.Parse(drNewRow["BRANCH_FLM_DISPENSER_MIN"].ToString()) +
                                                       decimal.Parse(drNewRow["BRANCH_FLM_CASH_OUT_MIN"].ToString()) + decimal.Parse(drNewRow["BRANCH_FLM_SUPERVISOR_MIN"].ToString())) / 1440, 4);


                    drNewRow["VENDOR_SLM_CARD_READER_MIN"] = Math.Round((decimal)dt.Select("ATMID='" + dtATMs.Rows[i][0].ToString() + "' and category in ('CARD READER') and problem_desc in ('SLM P1','SLM P1 manual')").AsEnumerable().Sum(r => r.Field<decimal>("actual_duration")));//i + 1;//i + 1;;
                    drNewRow["VENDOR_SLM_CARD_READER_PERC"] = Math.Round(decimal.Parse(drNewRow["VENDOR_SLM_CARD_READER_MIN"].ToString()) / 1440, 4);


                    drNewRow["VENDOR_SLM_DISPENSER_MIN"] = Math.Round((decimal)dt.Select("ATMID='" + dtATMs.Rows[i][0].ToString() + "' and category in ('CASH DISPENSER') and problem_desc in ('SLM P1','SLM P1 manual')").AsEnumerable().Sum(r => r.Field<decimal>("actual_duration")));//i + 1;//i + 1;;
                    drNewRow["VENDOR_SLM_DISPENSER_PERC"] = Math.Round(decimal.Parse(drNewRow["VENDOR_SLM_DISPENSER_MIN"].ToString()) / 1440, 4);

                    drNewRow["VENDOR_SLM_OTHER_MIN"] = 0;
                    drNewRow["VENDOR_SLM_OTHER_PERC"] = 0;

                    drNewRow["VENDOR_SLM_TOTAL_MIN"] = int.Parse(drNewRow["VENDOR_SLM_CARD_READER_MIN"].ToString()) + int.Parse(drNewRow["VENDOR_SLM_DISPENSER_MIN"].ToString());
                    drNewRow["VENDOR_SLM_TOTAL_PERC"] = (decimal.Parse(drNewRow["VENDOR_SLM_CARD_READER_PERC"].ToString().Replace("%", "")) + decimal.Parse(drNewRow["VENDOR_SLM_DISPENSER_PERC"].ToString().Replace("%", "")));


                    drNewRow["HOST_DOWN_MIN"] = Math.Round((decimal)dt.Select("ATMID='" + dtATMs.Rows[i][0].ToString() + "' and category in ('Host Down')").AsEnumerable().Sum(r => r.Field<decimal>("actual_duration")));//i + 1;
                    drNewRow["HOST_DOWN_PERC"] = Math.Round(decimal.Parse(drNewRow["HOST_DOWN_MIN"].ToString()) / 1440, 4);

                    drNewRow["POWER_DOWN_PERC"] = 0;

                    drNewRow["TOTAL_PERC"] = 0;
                    drNewRow["POWER_DOWN_MIN"] = 0;

                    drNewRow["COMMAND_FAIL_MIN"] = Math.Round((decimal)dt.Select("ATMID='" + dtATMs.Rows[i][0].ToString() + "' and category in ('Communication Failure')").AsEnumerable().Sum(r => r.Field<decimal>("actual_duration")));//i + 1;
                    drNewRow["COMMAND_FAIL_PERC"] = Math.Round(decimal.Parse(drNewRow["COMMAND_FAIL_MIN"].ToString()) / 1440, 4);

                    drNewRow["TOTAL_MIN"] = 0;
                    drNewRow["VANDALISM_PERC"] = 0;
                    drNewRow["VANDALISM_MIN"] = 0;

                    dt24HourAvailabilityData.Rows.Add(drNewRow);
                }
                dt = dt24HourAvailabilityData;
                //cmd.CommandText = "select * from problem_bucket_priority inner join problem_bucket_desc on problem_bucket_priority.problem_bucket_priority_id = problem_bucket_desc.problem_bucket_priority_id";
                //adapter = new SqlDataAdapter(cmd);
                //System.Data.DataTable helperDt = new System.Data.DataTable();
                //adapter.Fill(helperDt);

                //DataTable gasperDataTable = GetGasperTicketSchema();
                //DataTable dtAlerts = dt.DefaultView.ToTable(true, new string[] { "atm_alert_id" });
                //for (int i = 0; i < dtAlerts.Rows.Count; i++)
                //{
                //    resolvedAtRead = false;
                //    DataRow newRow = gasperDataTable.NewRow();
                //    DataRow[] drArray = dt.Select("atm_alert_id=" + dtAlerts.Rows[i][0]);
                //    for (int j = 0; j < drArray.Length; j++)
                //    {
                //        if (drArray[j]["token_name"].ToString() == "Escalation")
                //        {
                //            newRow["STATUSDESC"] = drArray[j]["token_value"];
                //            newRow["PROBLEMBUCKET"] = GetProblemBucket(helperDt, drArray[j]["token_value"].ToString());
                //        }
                //        else if (drArray[j]["token_name"].ToString() == "ATMID")
                //            newRow["ATMID"] = drArray[j]["token_value"];
                //        else if (drArray[j]["token_name"].ToString() == "Ticket Start")
                //        {
                //            ticketStartDateTime = DateTime.ParseExact(drArray[j]["token_value"].ToString(), "dd/MM/yyyy HH:mm:ss", null);
                //            newRow["STARTDATE"] = ticketStartDateTime.ToString("yyyy-MM-dd");
                //            newRow["STARTTIME"] = ticketStartDateTime.ToString("HH:mm:ss");
                //        }
                //        else if (drArray[j]["token_name"].ToString() == "Model")
                //            newRow["OBJECTTYPEDESC"] = drArray[j]["token_value"];
                //        //else if (drArray[j]["token_name"].ToString() == "ConnectionType")
                //        //    newRow["OBJECTTYPEDESC"] = drArray[j]["token_value"];
                //        else if (drArray[j]["token_name"].ToString() == "TicketNo")
                //            newRow["TICKETID"] = drArray[j]["token_value"];

                //        else if (drArray[j]["resolve_at"] != DBNull.Value)
                //        {
                //            if (!resolvedAtRead)
                //            {
                //                ticketEndDateTime = DateTime.Parse(drArray[j]["resolve_at"].ToString());
                //                newRow["ENDDATE"] = ticketEndDateTime.ToString("yyyy-MM-dd");
                //                newRow["ENDTIME"] = ticketEndDateTime.ToString("HH:mm:ss");

                //                newRow["DURATION"] = Math.Round((ticketEndDateTime - ticketStartDateTime).TotalMinutes, 2);
                //                resolvedAtRead = true;
                //            }
                //        }

                //    }


                //    if (drArray[0]["city"].ToString().Length > 0)
                //    {
                //        newRow["ADDRESS"] = drArray[0]["address1"];
                //        newRow["CITY"] = drArray[0]["city"];
                //    }
                //    gasperDataTable.Rows.Add(newRow);

                //}
                //dt = gasperDataTable;

                AppSetting appSetting = AppSetting.LoadAppSetting("1=1");
                //Application[ApplicationVars.AppSettings.ToString()];
                string zipFilePath = appSetting.TemporaryFolder + "\\24HourAvailabilityReportTemplate.zip";
                if (!File.Exists(zipFilePath))
                    throw new Exception("Template file does not exists.Please upload template file before generating settlement report");
                string extractFilePath = appSetting.TemporaryFolder + "\\24HourAvailabilityReportTemplate1";

                if (Directory.Exists(extractFilePath))
                    Directory.Delete(extractFilePath, true);

                if (!Directory.Exists(extractFilePath))
                    Directory.CreateDirectory(extractFilePath);

                ExpandFolder(zipFilePath, extractFilePath);

                Update24HourAvailabilitySheet(extractFilePath + "\\xl\\worksheets\\sheet2.xml", dt);

                if (File.Exists(extractFilePath + "\\xl\\calcChain.xml"))
                    File.Delete(extractFilePath + "\\xl\\calcChain.xml");

                Avanza.CCMS.DAL.Utility.UpdateWorkSheet(extractFilePath + "\\xl\\workbook.xml");
                UpdateDrawingSheet(extractFilePath);
                UpdateChartSheet(extractFilePath);

                Avanza.CCMS.DAL.Utility.ZipFolder(extractFilePath);
                if (File.Exists(extractFilePath + ".xlsx"))
                    File.Delete(extractFilePath + ".xlsx");
                File.Move(extractFilePath + ".zip", extractFilePath + ".xlsx");
                //Response.AddHeader("Content-Disposition", "attachment;filename=" + "CashOrders.xlsx");
                //Response.WriteFile(extractFilePath + ".xlsx");
                //Response.Flush();
                //Response.Close();
            }
            else
                Console.WriteLine("No records found to generate report");
        }

        #endregion

    }
}