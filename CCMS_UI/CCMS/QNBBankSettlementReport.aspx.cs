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
using Avanza.iSuite.DAL;
using System.Data;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using System.Xml;
using System.Collections;
using System.Web.Configuration;

namespace CCMSUI.CCMS
{
    public partial class QNBBankSettlementReport : BasePage
    {

        List<string> listSharedString = new List<string>();
        List<string> listATMsToBeUpdated = new List<string>();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (!CheckRight((int)Permissions.ViewQNBBankSettlementReport))
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
                DataRow newRow2 = null;
                int atmSettlementId = 0;
                int atmSettlementInfoId = 0;
                DateTime reportDate = DateTime.Now;
                decimal creditAmount = 0;
                decimal debitAmount = 0;
                decimal cashRecvFromBank = 0;
                decimal cashRetToBank = 0;
                DSBankSettlement Ds = new DSBankSettlement();
                AppUser objUser = (AppUser)Session[SessionVars.user.ToString()];
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 100;
                cmd.CommandText = @"
                                    select title,cit_atm_title,atm_settlement_id,rep_datetime,total_rejected,report_date,atm_site_and_number,total_replenished,atm_settlement_info.atm_settlement_info_id
                                    from atm_settlement inner join atm_settlement_info on atm_settlement.atm_settlement_info_id = atm_settlement_info.atm_settlement_info_id
                                    inner join atm on atm.cit_atm_title = atm_settlement.atm_no
                                    " + Filter();
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, cmd.CommandText);

                DataTable dt = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Rows returned");

                    reportDate = DateTime.Parse(dt.Rows[0]["report_date"].ToString());
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
                        newRow["atm"] = dr["cit_atm_title"].ToString().PadLeft(4, '0'); // dr["atm_site_and_number"];
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
                                newRow2["atm"] = dr["cit_atm_title"].ToString().PadLeft(4, '0');// dr["atm_site_and_number"];
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
                    AppSetting appSetting = (AppSetting)Application[ApplicationVars.AppSettings.ToString()];
                    string zipFilePath = appSetting.TemporaryFolder + "\\Template.zip";
                    if (!File.Exists(zipFilePath))
                        throw new Exception("Template file does not exists.Please upload template file before generating settlement report");
                    string extractFilePath = appSetting.TemporaryFolder + "\\Template1";

                    if (Directory.Exists(extractFilePath))
                        Directory.Delete(extractFilePath, true);

                    if (!Directory.Exists(extractFilePath))
                        Directory.CreateDirectory(extractFilePath);

                    Avanza.CCMS.DAL.Utility.ExpandFolder(zipFilePath, extractFilePath);

                    foreach (DataRow dr in Ds.Tables["DataTable1"].Rows)
                        listATMsToBeUpdated.Add(dr["atm"].ToString());

                    ReadSharedStringFile(extractFilePath + "\\xl\\sharedStrings.xml");
                    UpdateSheet1(extractFilePath + "\\xl\\worksheets\\sheet1.xml", 5, 380, reportDate, openingBalance, cashRecvFromBank, Ds.Tables["DataTable1"]);

                    if (File.Exists(extractFilePath + "\\xl\\calcChain.xml"))
                        File.Delete(extractFilePath + "\\xl\\calcChain.xml");

                    Avanza.CCMS.DAL.Utility.UpdateWorkSheet(extractFilePath + "\\xl\\workbook.xml");
                    Avanza.CCMS.DAL.Utility.ZipFolder(extractFilePath);
                    if (File.Exists(extractFilePath + ".xlsx"))
                        File.Delete(extractFilePath + ".xlsx");
                    File.Move(extractFilePath + ".zip", extractFilePath + ".xlsx");

                    Response.AddHeader("Content-Disposition", "attachment;filename=" + "QNBSettlementReport.xlsx");
                    Response.WriteFile(extractFilePath + ".xlsx");
                    Response.Flush();
                    Response.Close();

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
        //public static void UpdateWorkSheet(string filePath)
        //{
        //    XmlDocument xmlDoc = new XmlDocument();
        //    xmlDoc.Load(filePath);
        //    xmlDoc.ChildNodes[1].RemoveChild(xmlDoc.ChildNodes[1].ChildNodes[0]);
        //    xmlDoc.Save(filePath);
        //}
        public void ReadSharedStringFile(string filePath)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filePath);
            XmlNodeList childNodes = xmlDoc.GetElementsByTagName("si");
            for (int i = 0; i < childNodes.Count; i++)
            {
                listSharedString.Add(childNodes[i].FirstChild.InnerText);
            }
        }
        public void UpdateSheet1(string filePath, int atmStartRowIndex, int atmEndRowIndex, DateTime balancingDate,
                                        decimal vaultOpeningBalance, decimal cashRecv, DataTable dt)
        {
            DataRow[] drArray = null;
            bool isFirstColProcessed = false;
            bool isAtmFound = false;
            int startRowIndex = atmStartRowIndex;
            int endRowIndex = atmEndRowIndex;
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filePath);
            XmlNodeList childNodes = xmlDoc.GetElementsByTagName("row");
            for (int i = 0; i < childNodes.Count; i++)
            {
                isFirstColProcessed = false;
                isAtmFound = false;

                if (i == 1)
                    childNodes[i].ChildNodes[2].FirstChild.InnerText = balancingDate.ToOADate().ToString();
                else if (i == 2)
                {
                    if (childNodes[i].ChildNodes[4].FirstChild == null)
                    {
                        XmlElement element = xmlDoc.CreateElement("v", "http://schemas.openxmlformats.org/spreadsheetml/2006/main");
                        element.InnerText = vaultOpeningBalance.ToString();
                        childNodes[i].ChildNodes[4].InsertAfter(element, childNodes[i].ChildNodes[4].FirstChild);
                    }
                    else
                        childNodes[i].ChildNodes[4].FirstChild.InnerText = vaultOpeningBalance.ToString();
                }
                else if (i == 3)
                {
                    if (childNodes[i].ChildNodes[4].FirstChild == null)
                    {
                        XmlElement element = xmlDoc.CreateElement("v", "http://schemas.openxmlformats.org/spreadsheetml/2006/main");
                        element.InnerText = cashRecv.ToString();
                        childNodes[i].ChildNodes[4].InsertAfter(element, childNodes[i].ChildNodes[4].FirstChild);

                        //childNodes[i].ChildNodes[4].InsertAfter(element, childNodes[i].ChildNodes[4]);
                    }
                    else
                        childNodes[i].ChildNodes[4].FirstChild.InnerText = cashRecv.ToString();
                }
                else if (i >= startRowIndex && i <= endRowIndex)
                {
                    XmlNodeList colList = childNodes[i].ChildNodes;//<c></c>
                    for (int j = 0; j < 7; j++)
                    {
                        ///////////////////////////////////////////////////////////////////////////////////////////
                        //                                  Processing Title                                     //
                        if (colList[0].FirstChild != null && !isFirstColProcessed)
                        {
                            int vTagVal = int.Parse(colList[0].FirstChild.InnerText);
                            for (int k = 0; k < listATMsToBeUpdated.Count; k++)
                            {
                                if (listSharedString[vTagVal] == listATMsToBeUpdated[k])
                                {
                                    isAtmFound = true;
                                    drArray = dt.Select("atm='" + listSharedString[vTagVal] + "'");
                                    break;
                                }
                            }
                            isFirstColProcessed = true;
                            j++;//to skip processing location column;
                            continue;
                        }

                        ///////////////////////////////////////////////////////////////////////////////////////////

                        if (colList[j].FirstChild != null)
                            colList[j].RemoveChild(colList[j].FirstChild);

                        if (colList[j].FirstChild != null)
                            colList[j].RemoveChild(colList[j].FirstChild);

                        if (isAtmFound)
                        {
                            if (j == 2) //Replenished column
                            {
                                XmlElement element = xmlDoc.CreateElement("v", "http://schemas.openxmlformats.org/spreadsheetml/2006/main");
                                if (drArray != null)
                                    if (drArray.Length > 0)
                                        element.InnerText = drArray[0]["cash_replenished"].ToString();
                                colList[j].InsertAfter(element, colList[j].FirstChild);
                            }
                            else if (j == 3) //Returned column
                            {
                                XmlElement element = xmlDoc.CreateElement("v", "http://schemas.openxmlformats.org/spreadsheetml/2006/main");

                                if (drArray != null)
                                    if (drArray.Length > 0)
                                        element.InnerText = drArray[0]["cash_returned"].ToString();

                                colList[j].InsertAfter(element, colList[j].FirstChild);
                            }
                            else if (j == 4) //Variance column
                            {
                                XmlElement element = xmlDoc.CreateElement("v", "http://schemas.openxmlformats.org/spreadsheetml/2006/main");

                                if (drArray != null)
                                    if (drArray.Length > 0)
                                        element.InnerText = drArray[0]["variance"].ToString();

                                colList[j].InsertAfter(element, colList[j].FirstChild);
                            }
                        }
                    }
                }
            }
            xmlDoc.Save(filePath);
        }
       

        //public static void zipFolder(string basePath)
        //{
        //    zipFolder(basePath, false);
        //}

        //public static void zipFolder(string basePath, bool deleteAfter)
        //{
        //    string zipFileName = basePath + ".zip";
        //    ZipOutputStream os = new ZipOutputStream(File.OpenWrite(zipFileName));

        //    FileStream fs;

        //    _Files = new ArrayList();
        //    BuildList(basePath, 0);

        //    foreach (string s in _Files)
        //    {
        //        string sourceFileName = s;

        //        //string path = Path.GetFileName(sourceFileName);
        //        string path = Path.GetFullPath(sourceFileName).Replace(basePath + "\\", "");
        //        //string path1 = Path.GetExtension(path2);

        //        ZipEntry ze = new ZipEntry(path);
        //        ze.CompressionMethod = CompressionMethod.Deflated;
        //        os.PutNextEntry(ze);

        //        fs = File.OpenRead(sourceFileName);

        //        byte[] buff = new byte[1024];
        //        int n = 0;
        //        while ((n = fs.Read(buff, 0, buff.Length)) > 0)
        //        {
        //            os.Write(buff, 0, n);

        //        }
        //        fs.Close();
        //    }

        //    os.CloseEntry();
        //    os.Close();

        //    if (deleteAfter)
        //    {
        //        Directory.Delete(basePath, true);
        //    }
        //}
        //public static  void expandFolder(string zipFile, string baseFolder)
        //{
        //    if (!Directory.Exists(baseFolder))
        //    {
        //        Directory.CreateDirectory(baseFolder);
        //    }
        //    FileStream fr = File.OpenRead(zipFile);
        //    ZipInputStream ins = new ZipInputStream(fr);
        //    //ZipFile zf = new ZipFile(zipFile);
        //    ZipEntry ze = ins.GetNextEntry();
        //    while (ze != null)
        //    {
        //        if (ze.IsDirectory)
        //        {
        //            Directory.CreateDirectory(baseFolder + "\\" + ze.Name);
        //        }
        //        else if (ze.IsFile)
        //        {
        //            if (!Directory.Exists(baseFolder + "\\" + Path.GetDirectoryName(ze.Name)))
        //            {
        //                Directory.CreateDirectory(baseFolder + "\\" + Path.GetDirectoryName(ze.Name));
        //            }

        //            FileStream fs = File.Create(baseFolder + "\\" + ze.Name);

        //            byte[] writeData = new byte[ze.Size];
        //            int iteration = 0;
        //            while (true)
        //            {
        //                int size = 2048;
        //                size = ins.Read(writeData, (int)Math.Min(ze.Size, (iteration * 2048)), (int)Math.Min(ze.Size - (int)Math.Min(ze.Size, (iteration * 2048)), 2048));
        //                if (size > 0)
        //                {
        //                    fs.Write(writeData, (int)Math.Min(ze.Size, (iteration * 2048)), size);
        //                }
        //                else
        //                {
        //                    break;
        //                }
        //                iteration++;
        //            }
        //            fs.Close();
        //        }
        //        ze = ins.GetNextEntry();
        //    }
        //    ins.Close();
        //    fr.Close();
        //}
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
                                                          where report_name='ATMSummaryReport' and organization_id = " + orgID + "); " +

                                                         " delete report_schedule where report_name='ATMSummaryReport' and organization_id = " + orgID, trxn);

                        reportSchedule = new ReportSchedule();
                        reportSchedule.ReportName = "ATMSummaryReport";
                        reportSchedule.ReportPhysicalPath = Server.MapPath(@"ATMSummaryRpt.rpt");
                        reportSchedule.ReportReceipients = "";
                        reportSchedule.ReportTempPath = @"c:\ccmsLiveData\temp\";
                        reportSchedule.RetryCount = 5;
                        reportSchedule.ReportNextGeneratedAt = DateTime.Today;
                        reportSchedule.ReportFriendlyName = "ATM Summary Report";
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
                        Utility.BuildAuditLog("Schedule added for Bank Settlement Report", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.ViewQNBBankSettlementReport);
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