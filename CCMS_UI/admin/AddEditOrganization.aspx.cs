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

using Avanza.iSuite.UI;
using Encryption;
using System.IO;
using Newtonsoft.Json;

namespace CCMSUI.admin
{
    public partial class AddEditOrganization : BasePage
    {
        protected void RadioButtonList_DownloadTime_Selected(object sender, EventArgs e)
        {
            ViewState["scheduleModified"] = 1;
            ListBox_CashDataSchedules.Items.Clear();
            string item = RadioButtonList_DownloadTime.SelectedItem.Value;
            DateTime newDatetime = DateTime.Today.AddHours(double.Parse(DropDownList_CashDataHour.SelectedItem.Value)).AddMinutes(double.Parse(DropDownList_CashDataMin.SelectedItem.Value));
            while (newDatetime < DateTime.Today.AddDays(1))
            {
                ListBox_CashDataSchedules.Items.Add(new ListItem(newDatetime.Hour + ":" + newDatetime.Minute));
                newDatetime = newDatetime.AddMinutes(double.Parse(item));
            }
            RadioButtonList_DownloadTime.SelectedIndex = -1;
        }

        protected void Button_AddSchedule_Click(object sender, EventArgs e)
        {
            ViewState["scheduleModified"] = 1;
            string item = DropDownList_CashDataHour.SelectedItem.Value + ":" + DropDownList_CashDataMin.SelectedItem.Value;
            if (!ListBox_CashDataSchedules.Items.Contains(new ListItem(item)))
                ListBox_CashDataSchedules.Items.Add(item);
        }

        protected void Button_RemoveSchedule_Click(object sender, EventArgs e)
        {
            ViewState["scheduleModified"] = 1;
            for (int i = 0; i < ListBox_CashDataSchedules.Items.Count; i++)
            {
                if (ListBox_CashDataSchedules.Items[i].Selected)
                    ListBox_CashDataSchedules.Items.Remove(ListBox_CashDataSchedules.Items[i]);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                SqlCommand cmd = null;

                try
                {
                    CcmsCit.CcmsCitReader reader = null;
                    StringBuilder userMCNs = new StringBuilder();
                    DataTable dt = null;
                    SqlDataReader rdr = null;

                    Page.SetFocus(TextBox_OrganizationName);

                    reader = CcmsCit.ExecuteReader("1=1");
                    while (reader.Read())
                        ListBox_Cit.Items.Add(new ListItem(reader.CurrentCcmsCit.Name, reader.CurrentCcmsCit.Id.ToString()));
                    reader.Close();



                    for (int i = 0; i < 24; i++)
                    {
                        DropDownList_DFGenTime_Hour.Items.Add(new ListItem(i.ToString(), i.ToString()));
                        DropDownList_CashDataHour.Items.Add(new ListItem(i.ToString(), i.ToString()));
                    }
                    for (int i = 0; i < 60; i++)
                    {
                        DropDownList_DFGenTime_Min.Items.Add(new ListItem(i.ToString(), i.ToString()));
                        DropDownList_CashDataMin.Items.Add(new ListItem(i.ToString(), i.ToString()));
                    }

                    cmd = ConnectionFactory.GetNewCommand(true);
                    dt = ((DataTable)Session[SessionVars.dtUserOrgs.ToString()]);
                    foreach (DataRow dr in dt.Rows)
                        userMCNs.Append("'" + dr["mcn"] + "',");

                    if (Request.QueryString["organization_id"] == null)
                    {
                        if (!CheckRight((int)Permissions.CreateOrganization))
                            Response.Redirect("AccessDenied.aspx");

                        cmd.CommandText = @"select distinct(user_email), u.user_id from app_user u inner join user_organization o on u.user_id = o.user_id where user_is_active = 1 and u.user_id != 1";
                        if (userMCNs.ToString().Length > 0)
                            cmd.CommandText += " and org_mcn in (" + userMCNs.ToString().TrimEnd(',') + ")";
                        rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                            if (rdr.GetString(0).Length > 0)
                                ListBox_UsersEmail.Items.Add(new ListItem(rdr.GetString(0), rdr.GetInt32(1).ToString()));
                        rdr.Close();
                    }
                    else
                    {
                        Region organization = null;
                        RegionCit.RegionCitReader readerRegionCIT = null;
                        RegionReceipients.RegionReceipientsReader readerRegionRec = null;

                        if (!CheckRight((int)Permissions.ModifyOrganization))
                            Response.Redirect("AccessDenied.aspx");

                        organization = Region.LoadRegionByPk(int.Parse(Request.QueryString["organization_id"]));
                        DownloadingSchedule.DownloadingScheduleReader OrgReader = DownloadingSchedule.ExecuteReader("organization_id = " + organization.RegionId);
                        while (OrgReader.Read())
                            ListBox_CashDataSchedules.Items.Add(new ListItem(OrgReader.CurrentDownloadingSchedule.NextDownloadAt.Hour + ":" + OrgReader.CurrentDownloadingSchedule.NextDownloadAt.Minute));
                        OrgReader.Close();
                        if (organization.IsSecuredAccess.HasValue)
                            CheckBox_SecuredAccess.Checked = organization.IsSecuredAccess.Value;
                        cmd.CommandText = @"select distinct(user_email), u.user_id from app_user u inner join user_organization o on u.user_id = o.user_id where user_is_active = 1 and u.user_id != 1";
                        if (userMCNs.ToString().Length > 0)
                            cmd.CommandText += " and org_mcn like '" + organization.MCN + "'";
                        rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                            if (rdr.GetString(0).Length > 0)
                                ListBox_UsersEmail.Items.Add(new ListItem(rdr.GetString(0), rdr.GetInt32(1).ToString()));
                        rdr.Close();

                        TextBox_OrganizationName.Text = organization.RegionName;
                        TextBox_Location.Text = organization.Location;
                        TextBox_Country.Text = organization.Country;
                        TextBox_MCN.Text = organization.MCN;

                        TextBox_SMTPServer.Text = organization.SmtpServer;
                        TextBox_Username.Text = organization.SmtpUsername;
                        if (organization.SmtpPassword != null)
                        {
                            TextBox_Password.Attributes.Add("value", organization.SmtpPassword);
                            TextBox_Password.Attributes.Add("onchange", "setDirtyFlagToUpdateSMTPPasswordInDb();");
                        }
                        TextBox_SmtpPort.Text = organization.SmtpPort.ToString();

                        readerRegionCIT = RegionCit.ExecuteReader("region_id=" + organization.RegionId);
                        while (readerRegionCIT.Read())
                        {
                            for (int i = 0; i < ListBox_Cit.Items.Count; i++)
                            {
                                if (ListBox_Cit.Items[i].Value == readerRegionCIT.CurrentRegionCit.CitId.ToString())
                                    ListBox_Cit.Items[i].Selected = true;
                            }
                        }
                        readerRegionCIT.Close();

                        RadioButton_inactive.Checked = !organization.IsActive;
                        ViewState["organizationStatus"] = organization.IsActive;

                        readerRegionRec = RegionReceipients.ExecuteReader("region_id=" + organization.RegionId);
                        while (readerRegionRec.Read())
                        {
                            for (int i = 0; i < ListBox_UsersEmail.Items.Count; i++)
                            {
                                if (ListBox_UsersEmail.Items[i].Value == readerRegionRec.CurrentRegionReceipients.ReceipientsId.ToString())
                                    ListBox_UsersEmail.Items[i].Selected = true;
                            }
                        }
                        readerRegionRec.Close();

                        if (organization.SuspendCashOrder.HasValue)
                            CheckBox_SuspendCashOrders.Checked = organization.SuspendCashOrder.Value;

                        TextBox_DFUri.Text = organization.DailyFeedFtpUri;
                        TextBox_DFUsername.Text = organization.DailyFeedFtpUsername;
                        if (organization.DailyFeedFtpPassword != null)
                        {
                            TextBox_DFPassword.Attributes.Add("value", organization.DailyFeedFtpPassword);
                            TextBox_DFPassword.Attributes.Add("onchange", "setDirtyFlagToUpdateDffPasswordInDb();");

                        }
                        if (organization.DailyFeedGenerationTime.HasValue)
                        {
                            DropDownList_DFGenTime_Hour.SelectedIndex = organization.DailyFeedGenerationTime.Value.Hour;
                            DropDownList_DFGenTime_Min.SelectedIndex = organization.DailyFeedGenerationTime.Value.Minute;
                        }
                        TextBox_DFFilePath.Text = organization.DailyFeedOutputFilePath;
                        if (organization.DailyFeedGenerationDelay.HasValue)
                            TextBox_LagInterval.Text = organization.DailyFeedGenerationDelay.Value.ToString();
                        TextBox_OptiFTPUri.Text = organization.CashOrderFtpUri;
                        TextBox_OptiFTPUsername.Text = organization.CashOrderFtpUsername;
                        if (organization.CashOrderFtpPassword != null)
                        {
                            TextBox_OptiFTPPassword.Attributes.Add("value", organization.CashOrderFtpPassword);
                            TextBox_OptiFTPPassword.Attributes.Add("onchange", "setDirtyFlagToUpdateOCPasswordInDb();");

                        }
                        TextBox_OptiFTPArchiveFolder.Text = organization.CashOrderArchiveUrl;
                        TextBox_CashOrderFilePath.Text = organization.CashOrderDownloadedFilePath;

                        CheckBox_SuspendDFF.Checked = organization.IsDffSuspeded;
                        CheckBox_Counter.Checked = organization.IsCounterEnabled;
                        CheckBox_EJ.Checked = organization.IsEjEnabled;
                        if (organization.Priority == 1) //1-Counter 0-EJ
                            RadioButtonList_Priority.SelectedIndex = 1;
                        else
                            RadioButtonList_Priority.SelectedIndex = 0;

                        TextBox_OfflineFolderPath.Text = organization.OfflineAtmSettlementsFilePath;
                        TextBox_OfflineFolderPathForMulticurrency.Text = organization.OfflineAtmSettlementsMulticurrencyFilePath;
                        Response.Write("<script>window.parent.SelectNode('r" + organization.ParentRegionId + "')</script>");
                    }
                }
                catch (Exception ex)
                {
                    Literal_error.Text = "<li>" + ex.Message + "</li>";
                    LogableTask.LogMonoActivityTask("AddEditOrganization", MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                }
                finally
                {
                    if (cmd != null)
                        if (cmd.Connection.State == ConnectionState.Open)
                            cmd.Connection.Close();
                }
            }
        }

        protected void Button_SaveUpdateOrganization_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("SaveOrganization");
            SqlConnection conn = null;
            SqlTransaction trx = null;
            bool isMCNChanged = false;
            //bool result = false;
            try
            {

                ValidateInputs();

                Avanza.CCMS.DAL.Region organization = null;
                Avanza.CCMS.DAL.Region oldorganization = null;
                CcmsOrganization ccmsOrganization = null;
                bool editMode = false;
                string query = "select count(region_name) from region where region_name = '" + TextBox_OrganizationName.Text + "'";

                if (Request.QueryString["organization_id"] != null)
                {
                    editMode = true;
                    query += " and region_id <> " + Avanza.CCMS.DAL.Region.LoadRegionByPk(int.Parse(Request.QueryString["organization_id"])).RegionId;
                }
                if ((int)ConnectionFactory.ExecuteScalar(query) != 0)
                    throw new Exception("Organization with the same name already exists. Please provide different organization name");

                conn = ConnectionFactory.GetNewConnection();
                conn.Open();
                trx = conn.BeginTransaction();

                if (editMode)
                {
                    oldorganization = Avanza.CCMS.DAL.Region.LoadRegionByPk(int.Parse(Request.QueryString["organization_id"]));
                    organization = Avanza.CCMS.DAL.Region.LoadRegionByPk(int.Parse(Request.QueryString["organization_id"]));
                    ccmsOrganization = CcmsOrganization.LoadCcmsOrganization("region_id=" + organization.RegionId);
                    //incase organization does not exists in CCMS or its deleted.
                    if (ccmsOrganization == null)
                    {
                        ccmsOrganization = new CcmsOrganization();
                        ccmsOrganization.CreatedBy = organization.CreatedBy;
                        ccmsOrganization.CreatedOn = organization.CreationTime;
                    }
                }
                else
                {
                    organization = new Avanza.CCMS.DAL.Region();
                    ccmsOrganization = new CcmsOrganization();
                }
                ccmsOrganization.IsDeleted = false;
                organization.IsOrganization = true;
                organization.IsSecuredAccess = CheckBox_SecuredAccess.Checked;
                if (editMode)
                {
                    organization.ModifiedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                    ccmsOrganization.ModifiedBy = organization.ModifiedBy;
                    ccmsOrganization.ModifiedOn = DateTime.Now;
                }
                else
                {
                    organization.CreatedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                    organization.CreationTime = DateTime.Now;
                    ccmsOrganization.CreatedBy = organization.CreatedBy;
                    ccmsOrganization.CreatedOn = DateTime.Now;
                }

                string selectedRegion = null;
                if (Session[SessionVars.SelectedRegion.ToString()] == null)
                {
                    selectedRegion = "null";
                    organization.ParentRegionId = null;
                }
                else
                {
                    selectedRegion = "r" + Session[SessionVars.SelectedRegion.ToString()].ToString();
                    if (Session[SessionVars.SelectedRegion.ToString()].ToString() != organization.RegionId.ToString())
                        organization.ParentRegionId = int.Parse(Session[SessionVars.SelectedRegion.ToString()].ToString());
                }

                organization.RegionName = TextBox_OrganizationName.Text;
                ccmsOrganization.Name = organization.RegionName;

                organization.Location = TextBox_Location.Text;
                ccmsOrganization.Location = organization.Location;

                organization.Country = TextBox_Country.Text;
                ccmsOrganization.Country = organization.Country;

                if (organization.MCN != TextBox_MCN.Text)
                    isMCNChanged = true;
                ccmsOrganization.Mcn = organization.MCN = TextBox_MCN.Text;

                organization.IsDffSuspeded = CheckBox_SuspendDFF.Checked;
                organization.IsCounterEnabled = CheckBox_Counter.Checked;
                organization.IsEjEnabled = CheckBox_EJ.Checked;
                organization.Priority = int.Parse(RadioButtonList_Priority.SelectedItem.Value);
                organization.SuspendCashOrder = CheckBox_SuspendCashOrders.Checked;

                //if (FileUpload_BankLogo.HasFile)
                //{
                //    if ((FileUpload_BankLogo.FileName.Contains(".jpg") || (FileUpload_BankLogo.FileName.Contains(".JPG"))))
                //    {
                //        organization.BankLogo = FileUpload_BankLogo.FileBytes;   
                //    }
                //    else
                //    {
                //        Literal_error.Text = "<li>Please upload only .jpg file only</li>";
                //        return;
                //    }
                //}
                //else
                //    organization.BankLogo =new byte[100];

                organization.IsActive = RadioButton_active.Checked;
                ccmsOrganization.IsDeleted = !organization.IsActive;

                StringBuilder criteria = new StringBuilder();

                if (CheckBox_Counter.Checked)
                    criteria.Append(" is_counter_enabled = 1,");
                else
                    criteria.Append(" is_counter_enabled = 0,");

                if (CheckBox_EJ.Checked)
                    criteria.Append(" is_ej_enabled = 1,");
                else
                    criteria.Append(" is_ej_enabled = 0,");


                if (CheckBox_SuspendCashOrders.Checked)
                    criteria.Append(" suspend_cash_order = 1,");
                else
                    criteria.Append(" suspend_cash_order = 0,");


                if (CheckBox_SuspendDFF.Checked)
                    criteria.Append(" exclude_dff = 1,");
                else
                    criteria.Append(" exclude_dff = 0,");

                criteria = criteria.Remove(criteria.Length - 1, 1);
                ConnectionFactory.ExecuteQuery("update atm set " + criteria + "  where is_active = 1 and atm_id in (select atm_id from mcn_atms where mcn='" + TextBox_MCN.Text + "')", trx);

                //if (CheckBox_SuspendCashOrders.Checked)
                //{
                //    ConnectionFactory.ExecuteQuery("update atm set suspend_cash_order = 1 where is_active = 1 and atm_id in (select atm_id from mcn_atms where mcn='" + TextBox_MCN.Text + "')", trx);
                //    organization.SuspendCashOrder = true;
                //}
                //else
                //{
                //    ConnectionFactory.ExecuteQuery("update atm set suspend_cash_order = 0 where is_active = 1 and atm_id in (select atm_id from mcn_atms where mcn='" + TextBox_MCN.Text + "')", trx);
                //    organization.SuspendCashOrder = false;
                //}



                //if (CheckBox_SuspendDFF.Checked)
                //{
                //    ConnectionFactory.ExecuteQuery("update atm set exclude_dff = 1 where is_active = 1 and atm_id in (select atm_id from mcn_atms where mcn='" + TextBox_MCN.Text + "')", trx);
                //    //organization.IsDffSuspeded = true;
                //}
                //else
                //{
                //    ConnectionFactory.ExecuteQuery("update atm set exclude_dff = 0 where is_active = 1 and atm_id in (select atm_id from mcn_atms where mcn='" + TextBox_MCN.Text + "')", trx);
                //    //organization.SuspendCashOrder = false;
                //}


                organization.DailyFeedFtpUri = TextBox_DFUri.Text;
                organization.DailyFeedFtpUsername = TextBox_DFUsername.Text;
                if (TextBox_DFPassword.Text.Length > 0 && hidden_dirtyFlagForDffFTPPassword.Value == "1")
                    organization.DailyFeedFtpPassword = Cryptic.EncryptString(TextBox_DFPassword.Text);
                organization.DailyFeedGenerationTime = DateTime.Today.AddHours(double.Parse(DropDownList_DFGenTime_Hour.SelectedItem.Value)).AddMinutes(double.Parse(DropDownList_DFGenTime_Min.SelectedItem.Value));
                organization.DailyFeedOutputFilePath = TextBox_DFFilePath.Text;
                if (TextBox_LagInterval.Text.Length > 0)
                    organization.DailyFeedGenerationDelay = int.Parse(TextBox_LagInterval.Text);
                else
                    organization.DailyFeedGenerationDelay = 1;
                organization.CashOrderFtpUri = TextBox_OptiFTPUri.Text;
                organization.CashOrderFtpUsername = TextBox_OptiFTPUsername.Text;
                if (TextBox_OptiFTPPassword.Text.Length > 0 && hidden_dirtyFlagForCashOrderPassword.Value == "1")
                    organization.CashOrderFtpPassword = Cryptic.EncryptString(TextBox_OptiFTPPassword.Text);
                organization.CashOrderArchiveUrl = TextBox_OptiFTPArchiveFolder.Text;
                organization.CashOrderDownloadedFilePath = TextBox_CashOrderFilePath.Text;

                organization.SmtpServer = TextBox_SMTPServer.Text;
                organization.SmtpUsername = TextBox_Username.Text;
                if (TextBox_Password.Text.Length > 0 && hidden_dirtyFlagForSMTPPassword.Value == "1")
                    organization.SmtpPassword = Cryptic.EncryptString(TextBox_Password.Text);
                if (TextBox_SmtpPort.Text.Length > 0)
                    organization.SmtpPort = short.Parse(TextBox_SmtpPort.Text);

                if (TextBox_OfflineFolderPath.Text.Length > 0)
                    organization.OfflineAtmSettlementsFilePath = TextBox_OfflineFolderPath.Text;

                if (TextBox_OfflineFolderPathForMulticurrency.Text.Length > 0)
                    organization.OfflineAtmSettlementsMulticurrencyFilePath = TextBox_OfflineFolderPathForMulticurrency.Text;

                organization.Save(conn, trx);
                ccmsOrganization.Code = "Dummy";

                ccmsOrganization.RegionId = organization.RegionId;


                ccmsOrganization.Save(conn, trx);


                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Verbose, "Organization added/edited. Id = " + organization.RegionId);
                if (isMCNChanged)
                    ConnectionFactory.ExecuteQuery("update mcn_atms set mcn = '" + organization.MCN + "' where region_id = " + organization.RegionId, trx);
                ConnectionFactory.ExecuteQuery("delete region_receipients where region_id =" + organization.RegionId, trx);
                for (int i = 0; i < ListBox_UsersEmail.Items.Count; i++)
                {
                    if (ListBox_UsersEmail.Items[i].Selected)
                    {
                        RegionReceipients regionReceipients = new RegionReceipients();
                        regionReceipients.RegionId = organization.RegionId;
                        regionReceipients.ReceipientsId = int.Parse(ListBox_UsersEmail.Items[i].Value);
                        regionReceipients.Save(conn, trx);
                    }
                }

                ConnectionFactory.ExecuteQuery("delete region_cit where region_id = " + organization.RegionId, trx);
                for (int i = 0; i < ListBox_Cit.Items.Count; i++)
                {
                    if (ListBox_Cit.Items[i].Selected)
                    {
                        RegionCit regionCIT = new RegionCit();
                        regionCIT.RegionId = organization.RegionId;
                        regionCIT.CitId = int.Parse(ListBox_Cit.Items[i].Value);
                        regionCIT.Save(conn, trx);
                    }
                }

                UserOrganization userOrganization = null;
                if ((int)ConnectionFactory.ExecuteScalar("select count(*) from user_organization where user_id = 2 and org_mcn ='" + organization.MCN + "'") == 0)
                {
                    //   ConnectionFactory.ExecuteQuery("delete user_organization where org_mcn = '" + organization.MCN + "'", trx);

                    userOrganization = new UserOrganization();
                    userOrganization.UserId = 2;
                    userOrganization.OrgMcn = organization.MCN;
                    userOrganization.Save(conn, trx);

                }




                //Changes done on 17/12/2015 to prevent login issue.
                //if (((AppUser)Session[SessionVars.user.ToString()]).UserId != 2)
                //{
                //    userOrganization = new UserOrganization();
                //    userOrganization.UserId = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                //    userOrganization.OrgMcn = organization.MCN;
                //    userOrganization.Save(conn, trx);
                //}
                //for (int i = 0; i < ListBox_UsersEmail.Items.Count; i++)
                //{
                //    if (ListBox_UsersEmail.Items[i].Selected && int.Parse(ListBox_UsersEmail.Items[i].Value) != 2 && int.Parse(ListBox_UsersEmail.Items[i].Value) != ((AppUser)Session[SessionVars.user.ToString()]).UserId)
                //    {
                //        userOrganization = new UserOrganization();
                //        userOrganization.UserId = int.Parse(ListBox_UsersEmail.Items[i].Value);
                //        userOrganization.OrgMcn = organization.MCN;
                //        userOrganization.Save(conn, trx);
                //    }
                //}
                if (ViewState["scheduleModified"] != null)
                {
                    ConnectionFactory.ExecuteScalar("delete downloading_schedule where organization_id = " + organization.RegionId);
                    for (int i = 0; i < ListBox_CashDataSchedules.Items.Count; i++)
                    {
                        string[] parts = ListBox_CashDataSchedules.Items[i].Value.Split(':');
                        DownloadingSchedule newSchedule = new DownloadingSchedule();
                        newSchedule.OrganizationId = organization.RegionId;
                        newSchedule.NextDownloadAt = DateTime.Today.AddHours(double.Parse(parts[0])).AddMinutes(double.Parse(parts[1]));
                        newSchedule.Save(conn, trx);
                    }
                }



                

                DataTable dtUserOrgs = ((DataTable)Session[SessionVars.dtUserOrgs.ToString()]);
                if (dtUserOrgs != null)
                {
                    if (!dtUserOrgs.Rows.Contains(organization.MCN))
                    {
                        dtUserOrgs.Rows.Add(new object[] { organization.MCN });
                        Session[SessionVars.dtUserOrgs.ToString()] = dtUserOrgs;
                    }
                }

                if (editMode)
                {
                    if (!((bool)ViewState["organizationStatus"]) && organization.IsActive)
                        Session[SessionVars.notice.ToString()] = "Organization successfully saved!<script>window.parent.AddNode('" + organization.RegionName + "','" + selectedRegion + "','" + organization.RegionId + "',false,true);window.location='organization.aspx';</script>";
                    else if (organization.IsActive)
                        Session[SessionVars.notice.ToString()] = "<script>parent.window.ShowSuccess('Organization successfully saved!');parent.window.RenameRegion('" + organization.RegionName + "'," + organization.RegionId + ");window.location='organization.aspx';</script>";
                    else
                        Session[SessionVars.notice.ToString()] = "<script>parent.window.ShowSuccess('Organization deleted successfully!');parent.window.DeleteNode(" + organization.RegionId + ");window.location='organization.aspx';</script>";

                    editAuditLog(organization, oldorganization, trx);
                    //Utility.BuildAuditLog(organization.RegionName + " modified", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.ModifyOrganization);
                }
                else
                {
                    Session[SessionVars.notice.ToString()] = "<script>parent.window.ShowSuccess('Organization successfully saved!');window.parent.AddNode('" + organization.RegionName + "','" + selectedRegion + "','" + organization.RegionId + "',false,true);window.location='organization.aspx';</script>";
                    Utility.BuildAuditLog("Organization "+ organization.RegionName + " created", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.CreateOrganization);
                    string selectedRegions = (string)Session[SessionVars.SelectedRegions.ToString()];
                    selectedRegions += "," + organization.RegionId;
                    Session[SessionVars.SelectedRegions.ToString()] = selectedRegions;
                }

                if (trx != null)
                    trx.Commit();

            }
            catch (Exception ex)
            {
                if (trx != null)
                    trx.Rollback();

                Literal_error.Text = "<li>An error occured while saving Organization. [" + ex.Message + "]</li>";
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                if (ex.Message.ToLower().Contains("invalid"))
                    throw;
            }
            finally
            {
                task.EndTask();

                if (conn != null)
                    if (conn.State == ConnectionState.Open)
                        conn.Close();
            }
        }

        public void editAuditLog(Avanza.CCMS.DAL.Region objCurrent, Avanza.CCMS.DAL.Region objPrevious, SqlTransaction trx)
        {
            string outputCurrent = JsonConvert.SerializeObject(objCurrent);
            string outputPrevious = JsonConvert.SerializeObject(objPrevious);

            if (outputCurrent != outputPrevious)
            {
                Utility.editCaseAuditLogInserts(objCurrent, objPrevious, "Organization " + objPrevious.RegionName + " modified", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.ModifyOrganization, trx);
            }

        }

      

        public void ValidateInputs()
        {
            
                //bool result = true;
                bool isCashOrderFilePathContainSpecialChars = CheckSpecialChars(TextBox_CashOrderFilePath.Text);
                if (isCashOrderFilePathContainSpecialChars == true)
                {
                    throw new Exception("Cash Order File Path has invalid character");
                    //return result = false;
                }
                bool isCountryContainSpecialChars = CheckSpecialChars(TextBox_Country.Text);
                if (isCountryContainSpecialChars == true)
                {
                    throw new Exception("Country has invalid character");
                    //return result = false;
                }
                bool isDFFilePathContainSpecialChars = CheckSpecialChars(TextBox_DFFilePath.Text);
                if (isDFFilePathContainSpecialChars == true)
                {
                    throw new Exception("DFFile Path has invalid character");
                    //return result = false;
                }
                bool isDFUriContainSpecialChars = CheckSpecialChars(TextBox_DFUri.Text);
                if (isDFUriContainSpecialChars == true)
                {
                    throw new Exception("DF Uri has invalid character");
                    //return result = false;
                }
                bool isDFUsernameContainSpecialChars = CheckSpecialChars(TextBox_DFUsername.Text);
                if (isDFUsernameContainSpecialChars == true)
                {
                    throw new Exception("DF User name has invalid character");
                    //return result = false;
                }
                bool isLagIntervalContainSpecialChars = CheckSpecialChars(TextBox_LagInterval.Text);
                if (isLagIntervalContainSpecialChars == true)
                {
                    throw new Exception("Lag Interval has invalid character");
                    //return result = false;
                }
                bool isLocationContainSpecialChars = CheckSpecialChars(TextBox_Location.Text);
                if (isLocationContainSpecialChars == true)
                {
                    throw new Exception("Location has invalid character");
                    //return result = false;
                }
                bool isMCNContainSpecialChars = CheckSpecialChars(TextBox_MCN.Text);
                if (isMCNContainSpecialChars == true)
                {
                    throw new Exception("MCN has invalid character");
                    //return result = false;
                }
                bool isOfflineFolderPathContainSpecialChars = CheckSpecialChars(TextBox_OfflineFolderPath.Text);
                if (isOfflineFolderPathContainSpecialChars == true)
                {
                    throw new Exception("Offline Folder Path has invalid character");
                    //return result = false;
                }
                bool isOfflineFolderPathForMulticurrencyContainSpecialChars = CheckSpecialChars(TextBox_OfflineFolderPathForMulticurrency.Text);
                if (isOfflineFolderPathForMulticurrencyContainSpecialChars == true)
                {
                    throw new Exception("Offline Folder Path For Multicurrency has invalid character");
                    //return result = false;
                }
                bool isOptiFTPArchiveFolderContainSpecialChars = CheckSpecialChars(TextBox_OptiFTPArchiveFolder.Text);
                if (isOptiFTPArchiveFolderContainSpecialChars == true)
                {
                    throw new Exception("Opti FTP Archive Folder For Multicurrency has invalid character");
                    //return result = false;
                }
                bool isOptiFTPUriContainSpecialChars = CheckSpecialChars(TextBox_OptiFTPUri.Text);
                if (isOptiFTPUriContainSpecialChars == true)
                {
                    throw new Exception("Opti FTP Uri has invalid character");
                    //return result = false;
                }
                bool isOptiFTPUsernameContainSpecialChars = CheckSpecialChars(TextBox_OptiFTPUsername.Text);
                if (isOptiFTPUsernameContainSpecialChars == true)
                {
                    throw new Exception("Opti FTP User Name has invalid character");
                    //return result = false;
                }
                bool isOrganizationNameContainSpecialChars = CheckSpecialChars(TextBox_OrganizationName.Text);
                if (isOrganizationNameContainSpecialChars == true)
                {
                    throw new Exception("Organization Name has invalid character");
                    //return result = false;
                }
                bool isSmtpPortContainSpecialChars = CheckSpecialChars(TextBox_SmtpPort.Text);
                if (isSmtpPortContainSpecialChars == true)
                {
                    throw new Exception("Smtp Port has invalid character");
                    //return result = false;
                }
                bool isSMTPServerContainSpecialChars = CheckSpecialChars(TextBox_SMTPServer.Text);
                if (isSMTPServerContainSpecialChars == true)
                {
                    throw new Exception("SMTP Server has invalid character");
                    //return result = false;
                }
                bool isUsernameContainSpecialChars = CheckSpecialChars(TextBox_Username.Text);
                if (isUsernameContainSpecialChars == true)
                {
                    throw new Exception("User Name has invalid character");
                    //return result = false;
                }
                //return result;
           
        }
    }
}