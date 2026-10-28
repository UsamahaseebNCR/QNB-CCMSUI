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

//using LicenseVerifierLib;
using System.IO;
//

namespace Avanza.iSuite.UI
{
    public partial class AddEditATM : BasePage
    {
        AppSetting appSetting = null;
        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        }

        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            string regionID = (string)Session[SessionVars.SelectedRegions.ToString()];
            if (regionID != null /*&& !regionID.Contains(",")*/
                && regionID != "1")
            {
                DropDownList_NoteSetTypeID.Items.Clear();
                Avanza.CCMS.DAL.NoteSetType.NoteSetTypeReader reader =
                Avanza.CCMS.DAL.NoteSetType.ExecuteReader("region_id=" + GetOrganization(regionID.Contains(",") ? int.Parse(regionID.Substring(0, regionID.IndexOf(","))) : int.Parse(regionID)));
                DropDownList_NoteSetTypeID.Items.Add("Select");
                while (reader.Read())
                {
                    DropDownList_NoteSetTypeID.Items.Add(new ListItem(reader.CurrentNoteSetType.NoteSetTypeName, reader.CurrentNoteSetType.NoteSetTypeId.ToString()));
                }
                reader.Close();
                if (Session["noteSetId"] != null)
                    DropDownList_NoteSetTypeID.SelectedValue = Session["noteSetId"].ToString();
                else
                {
                    if (DropDownList_NoteSetTypeID.Items.Count > 1)
                        DropDownList_NoteSetTypeID.SelectedIndex = 1;
                }



            }

        }

        //  Avanza.CCMS.DAL.AppUser appUser;
        public static void PopulateATMTypes(DropDownList dropDownList)
        {
            TerminalType.TerminalTypeReader terminalReader = null;
            dropDownList.Items.Clear();
            try
            {
                terminalReader = TerminalType.ExecuteReader("1=1");
                while (terminalReader.Read())
                    dropDownList.Items.Add(new ListItem(terminalReader.CurrentTerminalType.AtmType, terminalReader.CurrentTerminalType.AtmType.ToString()));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                terminalReader.Close();
            }

        }
        //public void DisplayLicensedCount()
        //{
        //    VerifyLicenseClass verifier = null;
        //    StringBuilder strBuilder = new StringBuilder("308204BB020100300D06092A864886F70D0101010500048204A5308204A10201000282010100D0C5CA088B167902F054E4940BEEE2CA294EA3B1E3C3A3F38BD3D136B795ACC57F6536E4BBE92300F7503F75067DB4049ECC82B52D0FD0197F900C8844506B7D5F2865A95C7DA007672990C7E0A39F43E7CBFAD54C3E8E46716506DC869DB07388DDD58298390037379BBDC3FF426753FA3C80F24FCE9178BBA80ACB690AE850572B373D90B45FADAE9A235A62B9300FAA1C33F06B4DF3CBF878CF35785D50038AC47AEAC330F8A1872D73F147D317EF2F72879CCF52270D5FE18523238459026490F238D3D3FB16CF13A0B9C657959091D51CD29165112F94CB48FB87C74BBB022422A0AD6B1B757FD9D8BE13B644C844734851C3A234B82B75C39871AA5B15020111028201000311F7AD4D56EB30FC013F98C3F0B082F913FAE0BBD1EF9667B40E5EEC1C8DD5BA5813A17B3BE6296D0C79675E368DF10255D4BEE64F4A96F694F1201F1F4CDFF61AF26FAAC59CB4D0936F4E3BC62BC0FFA4E1CE6E4C36CC547DB8924D46156B1C5D9D7E27E2B8794858E0E88784BDC13C26896CF9A4EB8D11D187B02AF4DDC4EB6C6D9B7F9C009FA30827065C66F0A43CEFD3469C21259AE7C953E715BDBBF16E93BEDA5B6FC9C51D78253D9AEE3022B6A5B2988A9DC71859626BC74235ADBF52F514405C8DA376082BE71DD1D038A4F3D1A282EC0C8691F94BA9865BE64E0DEC757B020ABEDA19BF9B3F756490C7C08E2A361F9ECCB41A7A735AF58985C05102818100E6C9D5EAD0726390B670580F5A3FB40746F65F92B0459286178B1F7DB35B8C6EA85E8780EDAA1528544C4E6AAED9A535C21BDEA510C670548BF741CA08E31CDCC771AAD58E66F649278FADF7D8B1FE437DD1F7C0CCD0D937AD2E900C7109F4232A6CFD781EB5733F52DAD70CB88EB71A25186263E17C91D0742B8539515606E102818100E7944404DAD1D1B5A9FF6D9A7D2190683371B59C423A64204F736655FE9BD773832741698BD54B1D5EF7412971B4A980E9533872ECA2D44115C5A26B845F15539405E64AAFCD99737BDA8ED632568B7C5053FA4B11407931AF83B13EAD909BE50882789DB403B95F47C22682A4B27E885C248587B3C5CBDF30A6152698CD3EB502818043E102AE798B0E399F11FBC847B86220420C3A3A33D83A277056185225B183A8136718CB9132063909BC1710518B4EB575537DB813FE2109CED04086B751DB4FFE6CB9C6570F3960CF667E76128EB431F7D457ED696A9A3D8D49EE21C6E4CF55A310FF4172719A5DEB314E4F091AEA8F38163B0E5160DF97A9B2727A45194D510281810095D8683F6069A5CFE67819BE50F799ACD5FE48560CBC5EE7BAF05146B3CE401D9119668F96B7309A88BE1B1AD11A8BCBE244E84A5CE1D4A2959DFFAEFB4C9554329A67D5F948CCB422F6D4E4F3652D1433FA1A6CCEED7B989EBE9FDD432155D06EEB02C0656BD24CB5F618EB1F4651DFC326B0C1381692F9D42F3ADCBD3982ED02818046D5A6F2222A81CD74C0C4FA1354E2B0B6AB79CE6CAEF7F9531B0E190AC5AD4E7D4C197FE56906F247922B03D66E2B02F9465C667416CBEF817235328BE3EFF949E68DEBC61029B2658BF7E283FE989CFD34C57D499788DAF53D3182130BB493E82182187B339EC8B381F95A73C73DDF8D881D0F2892E58CA75BBED10EE4CEB9");
        //    verifier = new VerifyLicenseClass();
        //    Label_ATMCount.Text = ValidateLic(verifier, strBuilder.ToString(), ((AppSetting)Application[ApplicationVars.AppSettings.ToString()]).LicenseKey).ToString();

        //}
        //public int RemainingDays(VerifyLicenseClass verifier)
        //{
        //    try
        //    {
        //        if (verifier.IsTrialExpired())
        //            throw new Exception("Trial has Expired");

        //        string str = verifier.GetFromDate();
        //        string[] parts = str.Split('/');
        //        DateTime fromTime = new DateTime(int.Parse(parts[2].Split(':')[0]), int.Parse(parts[1]), int.Parse(parts[0]));
        //        TimeSpan datediff = DateTime.Today - fromTime;
        //        return (verifier.GetTotalDays() - (int)datediff.TotalDays);
        //    }
        //    catch
        //    {
        //        return -1;
        //    }
        //}
        //private int ValidateLic(VerifyLicenseClass verifier, string privateKey, string licenseKey)
        //{
        //    string fileName = string.Empty;
        //    try
        //    {
        //        int maxAtms = 0;
        //        fileName = ((AppSetting)Application[ApplicationVars.AppSettings.ToString()]).TemporaryFolder + "\\" + "privateKey.lcn";
        //        verifier.InitializeText(licenseKey, privateKey);
        //        //File.WriteAllText(fileName, privateKey);

        //        if (!verifier.ValidateLicenseByFile(fileName))
        //            throw new Exception("could not validate license");
        //        if (verifier.IsLicensedVersion())
        //            maxAtms = verifier.GetTotalAtmAllowed();
        //        else
        //        {
        //            maxAtms = verifier.GetTotalAtmAllowed();
        //            if (this.RemainingDays(verifier) < 0)
        //                throw new Exception("Trial Expired");
        //        }
        //        return maxAtms;
        //    }
        //    catch (Exception ex)
        //    {
        //        if (File.Exists(fileName))
        //            File.Delete(fileName);
        //        throw ex;
        //    }
        //}
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!Page.IsPostBack)
                try
                {

                    CheckBox_SDM.Visible = TabStripTab10.Visible = TabStripTab12.Visible =TabStripTab11.Visible  = TabStripTab1.Visible =TabStripTab_CPMConfig.Visible  = TabStripTab_CDMConfig.Visible = TabStripTab_physicalConfig.Visible = AdvancedATMConfiguration.Visible = tdRetryConfiguration.Visible = CheckRight((int)Permissions.AdvancedATMConfiguration);
                     

                    AtmBandwidth.AtmBandwidthReader bandwidthReader = AtmBandwidth.ExecuteReader("1=1");
                    //DropDownList_Package.Items.Add(new ListItem("Any", "Any"));
                    while (bandwidthReader.Read())
                        DropDownList_Bandwidth.Items.Add(new ListItem(bandwidthReader.CurrentAtmBandwidth.AtmBandwidthName, bandwidthReader.CurrentAtmBandwidth.AtmBandwidthId.ToString()));
                    bandwidthReader.Close();

                    AppUser.AppUserReader userReader = AppUser.ExecuteReader("user_is_active =1 and user_id > 1 order by user_full_name");
                    checkbox_list.DataSource = userReader;
                    checkbox_list.DataBind();
                    userReader.Close();

                    DropDownList_FromDay1Hour.Items.Add(new ListItem("Select"));
                    DropDownList_ToDay1Hour.Items.Add(new ListItem("Select"));
                    DropDownList_FromDay1Min.Items.Add(new ListItem("Select"));
                    DropDownList_ToDay1Min.Items.Add(new ListItem("Select"));

                    DropDownList_FromDay2Hour.Items.Add(new ListItem("Select"));
                    DropDownList_ToDay2Hour.Items.Add(new ListItem("Select"));
                    DropDownList_FromDay2Min.Items.Add(new ListItem("Select"));
                    DropDownList_ToDay2Min.Items.Add(new ListItem("Select"));

                    DropDownList_FromDay3Hour.Items.Add(new ListItem("Select"));
                    DropDownList_ToDay3Hour.Items.Add(new ListItem("Select"));
                    DropDownList_FromDay3Min.Items.Add(new ListItem("Select"));
                    DropDownList_ToDay3Min.Items.Add(new ListItem("Select"));

                    DropDownList_FromDay4Hour.Items.Add(new ListItem("Select"));
                    DropDownList_ToDay4Hour.Items.Add(new ListItem("Select"));
                    DropDownList_FromDay4Min.Items.Add(new ListItem("Select"));
                    DropDownList_ToDay4Min.Items.Add(new ListItem("Select"));

                    DropDownList_FromDay5Hour.Items.Add(new ListItem("Select"));
                    DropDownList_ToDay5Hour.Items.Add(new ListItem("Select"));
                    DropDownList_FromDay5Min.Items.Add(new ListItem("Select"));
                    DropDownList_ToDay5Min.Items.Add(new ListItem("Select"));

                    DropDownList_FromDay6Hour.Items.Add(new ListItem("Select"));
                    DropDownList_ToDay6Hour.Items.Add(new ListItem("Select"));
                    DropDownList_FromDay6Min.Items.Add(new ListItem("Select"));
                    DropDownList_ToDay6Min.Items.Add(new ListItem("Select"));

                    DropDownList_FromDay7Hour.Items.Add(new ListItem("Select"));
                    DropDownList_ToDay7Hour.Items.Add(new ListItem("Select"));
                    DropDownList_FromDay7Min.Items.Add(new ListItem("Select"));
                    DropDownList_ToDay7Min.Items.Add(new ListItem("Select"));



                    for (int i = 0; i < 24; i++)
                    {
                        //DropDownList_CashDataHour.Items.Add(new ListItem(i.ToString(), i.ToString()));
                        DropDownList_FromDay1Hour.Items.Add(new ListItem(i.ToString(), i.ToString()));
                        DropDownList_FromDay2Hour.Items.Add(new ListItem(i.ToString(), i.ToString()));
                        DropDownList_FromDay3Hour.Items.Add(new ListItem(i.ToString(), i.ToString()));
                        DropDownList_FromDay4Hour.Items.Add(new ListItem(i.ToString(), i.ToString()));
                        DropDownList_FromDay5Hour.Items.Add(new ListItem(i.ToString(), i.ToString()));
                        DropDownList_FromDay6Hour.Items.Add(new ListItem(i.ToString(), i.ToString()));
                        DropDownList_FromDay7Hour.Items.Add(new ListItem(i.ToString(), i.ToString()));


                        DropDownList_ToDay1Hour.Items.Add(new ListItem(i.ToString(), i.ToString()));
                        DropDownList_ToDay2Hour.Items.Add(new ListItem(i.ToString(), i.ToString()));
                        DropDownList_ToDay3Hour.Items.Add(new ListItem(i.ToString(), i.ToString()));
                        DropDownList_ToDay4Hour.Items.Add(new ListItem(i.ToString(), i.ToString()));
                        DropDownList_ToDay5Hour.Items.Add(new ListItem(i.ToString(), i.ToString()));
                        DropDownList_ToDay6Hour.Items.Add(new ListItem(i.ToString(), i.ToString()));
                        DropDownList_ToDay7Hour.Items.Add(new ListItem(i.ToString(), i.ToString()));
                    }

                    for (int i = 0; i < 60; i++)
                    {
                        //DropDownList_CashDataMin.Items.Add(new ListItem(i.ToString(), i.ToString()));

                        DropDownList_FromDay1Min.Items.Add(new ListItem(i.ToString(), i.ToString()));
                        DropDownList_FromDay2Min.Items.Add(new ListItem(i.ToString(), i.ToString()));
                        DropDownList_FromDay3Min.Items.Add(new ListItem(i.ToString(), i.ToString()));
                        DropDownList_FromDay4Min.Items.Add(new ListItem(i.ToString(), i.ToString()));
                        DropDownList_FromDay5Min.Items.Add(new ListItem(i.ToString(), i.ToString()));
                        DropDownList_FromDay6Min.Items.Add(new ListItem(i.ToString(), i.ToString()));
                        DropDownList_FromDay7Min.Items.Add(new ListItem(i.ToString(), i.ToString()));

                        DropDownList_ToDay1Min.Items.Add(new ListItem(i.ToString(), i.ToString()));
                        DropDownList_ToDay2Min.Items.Add(new ListItem(i.ToString(), i.ToString()));
                        DropDownList_ToDay3Min.Items.Add(new ListItem(i.ToString(), i.ToString()));
                        DropDownList_ToDay4Min.Items.Add(new ListItem(i.ToString(), i.ToString()));
                        DropDownList_ToDay5Min.Items.Add(new ListItem(i.ToString(), i.ToString()));
                        DropDownList_ToDay6Min.Items.Add(new ListItem(i.ToString(), i.ToString()));
                        DropDownList_ToDay7Min.Items.Add(new ListItem(i.ToString(), i.ToString()));
                    }

                    //DropDownList_ToDay1Hour.SelectedValue = "23";
                    //DropDownList_ToDay1Min.SelectedValue = "59";

                    //DropDownList_ToDay2Hour.SelectedValue = "23";
                    //DropDownList_ToDay2Min.SelectedValue = "59";

                    //DropDownList_ToDay3Hour.SelectedValue = "23";
                    //DropDownList_ToDay3Min.SelectedValue = "59";

                    //DropDownList_ToDay4Hour.SelectedValue = "23";
                    //DropDownList_ToDay4Min.SelectedValue = "59";

                    //DropDownList_ToDay5Hour.SelectedValue = "23";
                    //DropDownList_ToDay5Min.SelectedValue = "59";

                    //DropDownList_ToDay6Hour.SelectedValue = "23";
                    //DropDownList_ToDay6Min.SelectedValue = "59";

                    //DropDownList_ToDay7Hour.SelectedValue = "23";
                    //DropDownList_ToDay7Min.SelectedValue = "59";

                    if (Session["i"] != null)
                    {
                        TabStripTab_general.Visible = false;
                        TabStripTab_physicalConfig.Visible = false;
                        TabStripTab_CDMConfig.Visible = false;
                        TabStripTab_CPMConfig.Visible = false;
                        TabStripTab1.Visible = false;
                        TabStripTab11.Visible = false;
                        TabStripTab12.Visible = false;
                        PageView_general.Visible = false;
                        Button_SaveUpdateATM.Visible = false;
                        TabStripTab10.Visible = false;
                        //MultiPageInventory.Focus();
                        //MultiPageInventory.SelectedIndex = 0;

                        //PageView8.Focus();
                        //TabStrip1.SelectedTab = TabStripTab7;
                        Atm atm = Atm.LoadAtm("atm_id = " + int.Parse(Session["atm_id"].ToString()));

                        GetGeneralInventory(atm.ATMId);
                        GetHardwareInventory(atm.ATMId);
                        GetSoftwareInventory(atm.ATMId);

                        return;

                    }
                    else
                        TabStripTab6.Visible = false;


                    appSetting = Application[ApplicationVars.AppSettings.ToString()] as AppSetting;
                    if (appSetting.IsGoogleMapEnabled.HasValue)
                        if (appSetting.IsGoogleMapEnabled.Value)
                        {
                            // pageBody.Attributes.Add("key1","loadScript();ShowLatLong(document.getElementById('<%=TextBox_Location.ClientID%>').value)");

                            Literal_GoogleScript.Text = "  <script>$(document).ready(function () { loadScript();     });  </script> ";
                        }
                    //   Literal_GoogleScript.Text = "<script type=\"text/javascript\" src=\"http://maps.googleapis.com/maps/api/js?sensor=false\"></script><script type=\"text/javascript\" src=\"https://maps.googleapis.com/maps/api/js?key=AIzaSyB59hfEUsise1ksMJ0yDOC_8zIV6CznAuE\">    </script>";



                    //TextBox_Port.Text = (Application[ApplicationVars.AppSettings.ToString()] as AppSetting).DefaltAtmPort.ToString();

                    //DisplayLicensedCount();
                    Session["dt"] = null;
                    Session["dtBatch"] = null;

                    Page.SetFocus(TextBox_ATMFriendlyName);
                    PopulateATMTypes(DropDownList_Type);
                    // AppSetting objappsetting = (AppSetting) Application[ApplicationVars.AppSettings.ToString()];
                    if (!CheckRight((int)Permissions.ViewATMInventory))
                        TabStripTab6.Visible = false;
                    if (!CheckRight((int)Permissions.ViewPowerMonitoringModule))

                        //if (System.Configuration.ConfigurationManager.AppSettings["isPowerMonitoringDisabled"] == "1")
                        TabStripTab11.Visible = false;


                    DropDownList_Hour.Items.Add("Select");
                    for (int i = 0; i < 24; i++)
                    {
                        //DropDownList_CashDataHour.Items.Add(new ListItem(i.ToString(), i.ToString()));
                        DropDownList_Hour.Items.Add(new ListItem(i.ToString().PadLeft(2, '0'), i.ToString().PadLeft(2, '0')));
                    }
                    DropDownList_Min.Items.Add("Select");
                    for (int i = 0; i < 60; i++)
                    {
                        //DropDownList_CashDataMin.Items.Add(new ListItem(i.ToString(), i.ToString()));
                        DropDownList_Min.Items.Add(new ListItem(i.ToString().PadLeft(2, '0'), i.ToString().PadLeft(2, '0')));
                    }


                    //Load ATM Classification 
                    Classification.ClassificationReader classificationReader = Classification.ExecuteReader("");

                    CheckBoxList_Classification.DataSource = classificationReader;
                    CheckBoxList_Classification.DataTextField = "ClassificationName";
                    CheckBoxList_Classification.DataValueField = "ClassificationId";
                    CheckBoxList_Classification.DataBind();
                    classificationReader.Close();

                    //

                    if (Session["atm_id"] == null)
                    {
                        if (!CheckRight((int)Permissions.CreateATMs))
                            Response.Redirect("AccessDenied.aspx");

                        TextBox_Port.Text = appSetting.DefaltAtmPort.ToString();
                        TextBox_DistributionPort.Text = "12603";

                        //string regionID = (string)Session[SessionVars.SelectedRegions.ToString()];
                        //if (regionID != null && regionID != "1")
                        //{
                        Avanza.CCMS.DAL.NoteSetType.NoteSetTypeReader reader = Avanza.CCMS.DAL.NoteSetType.ExecuteReader("region_id=" + Session["selectedOrgID"].ToString());



                        //  Avanza.CCMS.DAL.NoteSetType.ExecuteReader("region_id=" + GetOrganization(regionID.Contains(",") ? int.Parse(regionID.Substring(0, regionID.IndexOf(","))) : int.Parse(regionID)));

                        while (reader.Read())
                        {
                            DropDownList_NoteSetTypeID.Items.Add(new ListItem(reader.CurrentNoteSetType.NoteSetTypeName, reader.CurrentNoteSetType.NoteSetTypeId.ToString()));
                        }
                        reader.Close();
                        // }
                        Avanza.CCMS.DAL.CcmsCit.CcmsCitReader citReader = Avanza.CCMS.DAL.CcmsCit.ExecuteReader("is_active = 1");
                        while (citReader.Read())
                        {
                            DropDownList_CIT.Items.Add(new ListItem(citReader.CurrentCcmsCit.Name, citReader.CurrentCcmsCit.Id.ToString()));
                        }
                        citReader.Close();

                    }
                    else
                    {
                        if (!CheckRight((int)Permissions.ModifyATM))
                            Response.Redirect("AccessDenied.aspx");

                        Atm atm = Atm.LoadAtm("atm_id = " + int.Parse(Session["atm_id"].ToString()));
                        if (atm.AtmBandwidthId.HasValue)
                        DropDownList_Bandwidth.SelectedValue = atm.AtmBandwidthId.Value.ToString();
                        OperationalHour.OperationalHourReader operationalHourReader = OperationalHour.ExecuteReader("atm_id = " + atm.ATMId);
                        while (operationalHourReader.Read())
                        {
                            string[] parts1 = operationalHourReader.CurrentOperationalHour.OperationalHourFrom.Split(':');
                            string[] parts2 = operationalHourReader.CurrentOperationalHour.OperationalHourTo.Split(':');

                            if (operationalHourReader.CurrentOperationalHour.OperationalDayId == 1)
                            {
                                DropDownList_FromDay1Hour.SelectedValue = int.Parse(parts1[0]).ToString();
                                DropDownList_FromDay1Min.SelectedValue = int.Parse(parts1[1]).ToString();
                                DropDownList_ToDay1Hour.SelectedValue = int.Parse(parts2[0]).ToString();
                                DropDownList_ToDay1Min.SelectedValue = int.Parse(parts2[1]).ToString();
                            }
                            else if (operationalHourReader.CurrentOperationalHour.OperationalDayId == 2)
                            {
                                DropDownList_FromDay2Hour.SelectedValue = int.Parse(parts1[0]).ToString();
                                DropDownList_FromDay2Min.SelectedValue = int.Parse(parts1[1]).ToString();
                                DropDownList_ToDay2Hour.SelectedValue = int.Parse(parts2[0]).ToString();
                                DropDownList_ToDay2Min.SelectedValue = int.Parse(parts2[1]).ToString();
                            }
                            else if (operationalHourReader.CurrentOperationalHour.OperationalDayId == 3)
                            {
                                DropDownList_FromDay3Hour.SelectedValue = int.Parse(parts1[0]).ToString();
                                DropDownList_FromDay3Min.SelectedValue = int.Parse(parts1[1]).ToString();
                                DropDownList_ToDay3Hour.SelectedValue = int.Parse(parts2[0]).ToString();
                                DropDownList_ToDay3Min.SelectedValue = int.Parse(parts2[1]).ToString();
                            }

                            else if (operationalHourReader.CurrentOperationalHour.OperationalDayId == 4)
                            {
                                DropDownList_FromDay4Hour.SelectedValue = int.Parse(parts1[0]).ToString();
                                DropDownList_FromDay4Min.SelectedValue = int.Parse(parts1[1]).ToString();
                                DropDownList_ToDay4Hour.SelectedValue = int.Parse(parts2[0]).ToString();
                                DropDownList_ToDay4Min.SelectedValue = int.Parse(parts2[1]).ToString();
                            }

                            else if (operationalHourReader.CurrentOperationalHour.OperationalDayId == 5)
                            {
                                DropDownList_FromDay5Hour.SelectedValue = int.Parse(parts1[0]).ToString();
                                DropDownList_FromDay5Min.SelectedValue = int.Parse(parts1[1]).ToString();
                                DropDownList_ToDay5Hour.SelectedValue = int.Parse(parts2[0]).ToString();
                                DropDownList_ToDay5Min.SelectedValue = int.Parse(parts2[1]).ToString();
                            }

                            else if (operationalHourReader.CurrentOperationalHour.OperationalDayId == 6)
                            {
                                DropDownList_FromDay6Hour.SelectedValue = int.Parse(parts1[0]).ToString();
                                DropDownList_FromDay6Min.SelectedValue = int.Parse(parts1[1]).ToString();
                                DropDownList_ToDay6Hour.SelectedValue = int.Parse(parts2[0]).ToString();
                                DropDownList_ToDay6Min.SelectedValue = int.Parse(parts2[1]).ToString();
                            }

                            else if (operationalHourReader.CurrentOperationalHour.OperationalDayId == 7)
                            {
                                DropDownList_FromDay7Hour.SelectedValue = int.Parse(parts1[0]).ToString();
                                DropDownList_FromDay7Min.SelectedValue = int.Parse(parts1[1]).ToString();
                                DropDownList_ToDay7Hour.SelectedValue = int.Parse(parts2[0]).ToString();
                                DropDownList_ToDay7Min.SelectedValue = int.Parse(parts2[1]).ToString();
                            }


                        }
                        operationalHourReader.Close();

                        if (atm.InitEjExecTime != null)
                        {
                            DropDownList_Hour.SelectedValue = atm.InitEjExecTime.Substring(9, 2);
                            DropDownList_Min.SelectedValue = atm.InitEjExecTime.Substring(11, 2);
                        }

                        if (atm.IsSwapDefaultReplenishment.HasValue)
                            RadioButtonList_Type.SelectedValue = atm.IsSwapDefaultReplenishment.Value ? "1" : "0";

                        if (atm.ChequeAllowedInactivityPeriod.HasValue)
                            TextBox_ChequeInactivityPeriod.Text = atm.ChequeAllowedInactivityPeriod.Value.ToString();

                        if (atm.BnaAllowedInactivityPeriod.HasValue)
                            TextBox_BNAInactivityPeriod.Text = atm.BnaAllowedInactivityPeriod.Value.ToString();


                        if (atm.AllowedInactivityPeriod.HasValue)
                            TextBox_allowInactivityPeriod.Text = atm.AllowedInactivityPeriod.Value.ToString();

                        if (atm.IsWin7Machine.HasValue)
                        {
                            if (atm.IsWin7Machine.Value)
                                CheckBox_Win7OS.Checked = true;
                            else
                                CheckBox_Win7OS.Checked = false;
                        }
                        else
                            CheckBox_Win7OS.Checked = false;

                        //Populate Heart Beat Configuration
                        HeartBeatSchedule.HeartBeatScheduleReader heartBeatScheduleReader = HeartBeatSchedule.ExecuteReader("atm_id=" + atm.ATMId);
                        DataTable dt = GetHeartBeatDataTable();
                        while (heartBeatScheduleReader.Read())
                        {
                            DataRow dr = dt.NewRow();
                            dr[0] = heartBeatScheduleReader.CurrentHeartBeatSchedule.EventName;
                            dr[1] = heartBeatScheduleReader.CurrentHeartBeatSchedule.Interval;
                            dt.Rows.Add(dr);
                        }
                        heartBeatScheduleReader.Close();
                        Session["dt"] = dt;
                        GridView_HeartBeatSchedules.DataSource = dt;
                        GridView_HeartBeatSchedules.DataBind();


                        BatchSchedule.BatchScheduleReader batchScheduleReader = BatchSchedule.ExecuteReader("atm_id=" + atm.ATMId);
                        dt = GetHeartBeatDataTable();
                        while (batchScheduleReader.Read())
                        {
                            DataRow dr = dt.NewRow();
                            dr[0] = batchScheduleReader.CurrentBatchSchedule.EventName;
                            dr[1] = batchScheduleReader.CurrentBatchSchedule.Interval;
                            dt.Rows.Add(dr);
                        }
                        batchScheduleReader.Close();
                        Session["dtBatch"] = dt;
                        GridView_Batchschedule.DataSource = dt;
                        GridView_Batchschedule.DataBind();



                        DispensedLogic.DispensedLogicReader dispensedLogicReader = DispensedLogic.ExecuteReader("atm_id=" + atm.ATMId);
                        dt = GetDispensedConfDataTable();
                        while (dispensedLogicReader.Read())
                        {
                            DataRow dr = dt.NewRow();
                            dr["amount"] = dispensedLogicReader.CurrentDispensedLogic.Amount;
                            dr["operator"] = (dispensedLogicReader.CurrentDispensedLogic.OperatorId == 0 ? "Greator than equal to" :
                                dispensedLogicReader.CurrentDispensedLogic.OperatorId == 1 ? "Greator than" :
                                dispensedLogicReader.CurrentDispensedLogic.OperatorId == 2 ? "Less than equal to" :
                                    dispensedLogicReader.CurrentDispensedLogic.OperatorId == 3 ? "Less than" : "-1");


                            dr["type1"] = dispensedLogicReader.CurrentDispensedLogic.Type1;
                            dr["type2"] = dispensedLogicReader.CurrentDispensedLogic.Type2;
                            dr["type3"] = dispensedLogicReader.CurrentDispensedLogic.Type3;
                            dr["type4"] = dispensedLogicReader.CurrentDispensedLogic.Type4;
                            dt.Rows.Add(dr);
                        }
                        batchScheduleReader.Close();
                        Session["dtDispensedConf"] = dt;
                        GridView_DispensedConf.DataSource = dt;
                        GridView_DispensedConf.DataBind();


                        //if (atm.HeartBeatCashWithdrawalInterval.HasValue)
                        //    TextBox_CashWithdrawal_Interval.Text = atm.HeartBeatCashWithdrawalInterval.Value.ToString();

                        //if (atm.HeartBeatCashDepositInterval.HasValue)
                        //    TextBox_CashDeposit_Interval.Text = atm.HeartBeatCashDepositInterval.Value.ToString();

                        //if (atm.HeartBeatChequeDepositInterval.HasValue)
                        //    TextBox_ChequeDeposit_Interval.Text = atm.HeartBeatChequeDepositInterval.Value.ToString();

                        //if (atm.HeartBeatOrderCountersInterval.HasValue)
                        //    TextBox_OrderCounters_Interval.Text = atm.HeartBeatOrderCountersInterval.Value.ToString();

                        //if (atm.HeartBeatOrderIdInterval.HasValue)
                        //    TextBox_OrderId_Interval.Text = atm.HeartBeatOrderIdInterval.Value.ToString();

                        //if (atm.HeartBeatOutOfCashInterval.HasValue)
                        //    TextBox_OutOfCash_Interval.Text = atm.HeartBeatOutOfCashInterval.Value.ToString();

                        //Populate Batch Configuration

                        //if (atm.BatchCashDepositInterval.HasValue)
                        //    TextBox_BatchCashDeposit_Interval.Text = atm.BatchCashDepositInterval.Value.ToString();

                        //if (atm.BatchCashWithdrawalInterval.HasValue)
                        //    TextBox_BatchCashWithdrawal_Interval.Text = atm.BatchCashWithdrawalInterval.Value.ToString();

                        //if (atm.BatchChequeDepositInterval.HasValue)
                        //    TextBox_BatchChequeDeposit_Interval.Text = atm.BatchChequeDepositInterval.Value.ToString();

                        //if (atm.BatchReplenishmentInterval.HasValue)
                        //    TextBox_BatchReplenishment_Interval.Text = atm.BatchReplenishmentInterval.Value.ToString();


                        if (atm.Type1MinNotesThresholdValue.HasValue)
                            TextBox_Type1MinNotesThresholdValue.Text = atm.Type1MinNotesThresholdValue.ToString();

                        if (atm.Type2MinNotesThresholdValue.HasValue)
                            TextBox_Type2MinNotesThresholdValue.Text = atm.Type2MinNotesThresholdValue.ToString();

                        if (atm.Type3MinNotesThresholdValue.HasValue)
                            TextBox_Type3MinNotesThresholdValue.Text = atm.Type3MinNotesThresholdValue.ToString();

                        if (atm.Type4MinNotesThresholdValue.HasValue)
                            TextBox_Type4MinNotesThresholdValue.Text = atm.Type4MinNotesThresholdValue.ToString();

                        if (atm.InactivityPeriodNormalDays.HasValue)
                            TextBox_InactivityPeriodNormalDays.Text = atm.InactivityPeriodNormalDays.ToString();

                        if (atm.InactivityPeriodSalaryDays.HasValue)
                            TextBox_InactivityPeriodSalaryDays.Text = atm.InactivityPeriodSalaryDays.ToString();


                        if (atm.Type1MinNotesThreshold.HasValue)
                            TextBox_Type1MinNotesThreshold.Text = atm.Type1MinNotesThreshold.ToString();

                        if (atm.Type2MinNotesThreshold.HasValue)
                            TextBox_Type2MinNotesThreshold.Text = atm.Type2MinNotesThreshold.ToString();

                        if (atm.Type3MinNotesThreshold.HasValue)
                            TextBox_Type3MinNotesThreshold.Text = atm.Type3MinNotesThreshold.ToString();

                        if (atm.Type4MinNotesThreshold.HasValue)
                            TextBox_Type4MinNotesThreshold.Text = atm.Type4MinNotesThreshold.ToString();



                        if (atm.NoOfDispensedTransactionsToMonitor.HasValue)
                            TextBox_SampleSize.Text = atm.NoOfDispensedTransactionsToMonitor.Value.ToString();

                        if (atm.IsDffGenerationHalt.HasValue)
                            CheckBox_DisableDFFGeneration.Checked = atm.IsDffGenerationHalt.Value;

                        TextBox_desc.Text = atm.Description;
                        TextBox_SleepInterval.Text = atm.StartupSleepInterval.ToString();

                        TextBox_retryCountAlert.Text = atm.RetryCountAlert.ToString();
                        TextBox_CountsClearRetries.Text = atm.CountsClearRetries.ToString();
                        TextBox_TCPTimeout.Text = atm.TCPTimeout.ToString();
                        TextBox_DelayIteration.Text = atm.SleepInterval.ToString();
                        TextBox_CPMCommandWait.Text = atm.CPMCommandWait.ToString();
                        TextBox_CPMCommandSleep.Text = atm.CPMCommandSleep.ToString();
                        TextBox_AANDCApplications1.Text = atm.AANDCApplications1;
                        TextBox_AANDCApplications2.Text = atm.AANDCApplications2;
                        TextBox_AANDCApplications3.Text = atm.AANDCApplications3;
                        TextBox_AANDCApplications4.Text = atm.AANDCApplications4;
                        TextBox_AANDCApplications5.Text = atm.AANDCApplications5;
                        TextBox_MonitoringRetries.Text = atm.MonitoringRetries.ToString();
                        TextBox_WindowSwitch_Sleep.Text = atm.WindowSwitchSleep.ToString();
                        TextBox_AppSwitch_Sleep.Text = atm.AppSwitchSleep.ToString();
                        TextBox_MonitoringCycle_Sleep.Text = atm.MonitoringCycleSleep.ToString();
                        TextBox_CPMLogLevel.Text = atm.CPMLogLevel.ToString();
                        CheckBox_DispenserRealTimeEnabled.Checked = atm.IsDispenserRealTimeNotificationEnabled;
                        CheckBox_BNARealTimeEnabled.Checked = atm.IsBNARealTimeNotificationEnabled;
                        CheckBox_CPMRealTimeEnabled.Checked = atm.IsCPMRealTimeNotificationEnabled;
                        CheckBox_RepRealTimeEnabled.Checked = atm.IsReplenishmentRealTimeNotificationEnabled;
                        CheckBox_OutOfCashRealTimeEnabled.Checked = atm.IsOutOfCashRealTimeNotificationEnabled;
                        CheckBox_DisMismatchRealTimeEnabled.Checked = atm.IsDispenserMismatchRealTimeNotificationEnabled;
                        CheckBox_BNAMismatchRealTimeEnabled.Checked = atm.IsBNAMismatchRealTimeNotificationEnabled;
                        CheckBox_CPMMismatchRealTimeEnabled.Checked = atm.IsCPMMismatchRealTimeNotificationEnabled;
                        CheckBox_CounterExplodedRealTimeEnabled.Checked = atm.IsCounterExplodedRealTimeNotificationEnabled;
                        TextBox_Type1MinimumNotes.Text = atm.Type1MinimumNotes.ToString();
                        TextBox_Type2MinimumNotes.Text = atm.Type2MinimumNotes.ToString();
                        TextBox_Type3MinimumNotes.Text = atm.Type3MinimumNotes.ToString();
                        TextBox_Type4MinimumNotes.Text = atm.Type4MinimumNotes.ToString();
                        TextBox_Type5MinimumNotes.Text = atm.Type5MinimumNotes.ToString();
                        TextBox_Type6MinimumNotes.Text = atm.Type6MinimumNotes.ToString();
                        TextBox_Type7MinimumNotes.Text = atm.Type7MinimumNotes.ToString();
                        TextBox_OutOfCashThreshold.Text = atm.OutOfCashThreshold.ToString();

                        CheckBox_SDM.Checked = atm.IsSdm;




                        TextBox_NoOfRetryCounterFile.Text = atm.RetryCountCounterFile.ToString();
                        TextBox_NoOfRetryDateTimeSync.Text = atm.RetryCountDatetimeSchedule.ToString();
                        TextBox_NoOfRetryRestart.Text = atm.RetryCountRestartSchedule.ToString();
                        //TextBox_NumberOfRetryCashOrderDownload.Text = atm.RetryCountCashOrderDownload.ToString();
                        TextBox_NumberOfRetryCashOrderUpload.Text = atm.RetryCountCashOrderUpload.ToString();
                        TextBox_NumberOfRetryConfUpload.Text = atm.RetryCountConfUpload.ToString();
                        //TextBox_NumberOfRetryDFFUpload.Text = atm.RetryCountDffUpload.ToString();
                        if (atm.StartupSleepInterval.HasValue)
                            TextBox_SleepInterval.Text = atm.StartupSleepInterval.Value.ToString();
                        if (atm.DebugLevel.HasValue)
                            DropDownList_DebugLevel.SelectedIndex = atm.DebugLevel.Value;

                        Avanza.CCMS.DAL.NoteSetType.NoteSetTypeReader reader =
                        Avanza.CCMS.DAL.NoteSetType.ExecuteReader("region_id=" + GetOrganization(atm.RegionId));
                        //int index = 0;
                        //int noteSetTypeSelectedIndex =0;
                        while (reader.Read())
                        {
                            //  index++;
                            DropDownList_NoteSetTypeID.Items.Add(new ListItem(reader.CurrentNoteSetType.NoteSetTypeName, reader.CurrentNoteSetType.NoteSetTypeId.ToString()));
                            //if (atm.NoteSetTypeId == reader.CurrentNoteSetType.NoteSetTypeId)
                            //    noteSetTypeSelectedIndex = index;

                        }
                        reader.Close();

                        Avanza.CCMS.DAL.CcmsCit.CcmsCitReader citReader = Avanza.CCMS.DAL.CcmsCit.ExecuteReader("is_active = 1");
                        while (citReader.Read())
                        {
                            DropDownList_CIT.Items.Add(new ListItem(citReader.CurrentCcmsCit.Name, citReader.CurrentCcmsCit.Id.ToString()));
                        }
                        citReader.Close();

                        TextBox_SpareCash.Text = atm.SpareCash.ToString();
                        TextBox_AvgDispensed.Text = atm.AvgDispensed.ToString();
                        TextBox_AvgDispensedSalaryDays.Text = atm.AvgDispensedSalaryDays.ToString();

                        TextBox_NormalDaySuggestedNotesForType1.Text = atm.Type1SuggestedNotesNormalDays.ToString();
                        TextBox_NormalDaySuggestedNotesForType2.Text = atm.Type2SuggestedNotesNormalDays.ToString();
                        TextBox_NormalDaySuggestedNotesForType3.Text = atm.Type3SuggestedNotesNormalDays.ToString();
                        TextBox_NormalDaySuggestedNotesForType4.Text = atm.Type4SuggestedNotesNormalDays.ToString();
                        TextBox_NormalDaySuggestedNotesForType5.Text = atm.Type5SuggestedNotesNormalDays.ToString();
                        TextBox_NormalDaySuggestedNotesForType6.Text = atm.Type6SuggestedNotesNormalDays.ToString();
                        TextBox_NormalDaySuggestedNotesForType7.Text = atm.Type7SuggestedNotesNormalDays.ToString();

                        TextBox_SalaryDaySuggestedNotesForType1.Text = atm.Type1SuggestedNotesSalaryDays.ToString();
                        TextBox_SalaryDaySuggestedNotesForType2.Text = atm.Type2SuggestedNotesSalaryDays.ToString();
                        TextBox_SalaryDaySuggestedNotesForType3.Text = atm.Type3SuggestedNotesSalaryDays.ToString();
                        TextBox_SalaryDaySuggestedNotesForType4.Text = atm.Type4SuggestedNotesSalaryDays.ToString();
                        TextBox_SalaryDaySuggestedNotesForType5.Text = atm.Type5SuggestedNotesSalaryDays.ToString();
                        TextBox_SalaryDaySuggestedNotesForType6.Text = atm.Type6SuggestedNotesSalaryDays.ToString();
                        TextBox_SalaryDaySuggestedNotesForType7.Text = atm.Type7SuggestedNotesSalaryDays.ToString();

                        DropDownList_DispensingBehavior.SelectedItem.Value = atm.DispensingBehavior.ToString();





                        TextBox_Location.Text = atm.Location;
                        if (atm.MinOperatingBalanceNormalDays.HasValue)
                            TextBox_MinOperatingBalanceNormalDays.Text = atm.MinOperatingBalanceNormalDays.Value.ToString();

                        if (atm.MinOperatingBalanceSalaryDays.HasValue)
                            TextBox_MinOperatingBalanceSalaryDays.Text = atm.MinOperatingBalanceSalaryDays.Value.ToString();



                        //if (atm.Location.Length == 0)
                        //    RadioButtonList_Search.SelectedIndex = 1;
                        //else
                        //    RadioButtonList_Search.SelectedIndex = 0;
                        //RadioButtonList_Search_SelectedIndexChanged(this, null);

                        if (atm.ExcludeDff.HasValue)
                            CheckBox_ExcludeDFFGeneration.Checked = atm.ExcludeDff.Value;

                        if (atm.IsRecycler.HasValue)
                            CheckBox_isRecycler.Checked = atm.IsRecycler.Value;

                        if (atm.SuspendCashOrder)
                            CheckBox_SuspendCashOrder.Checked = atm.SuspendCashOrder;
                        ViewState["atmStatus"] = atm.IsActive;
                        TextBox_ATMFriendlyName.Text = atm.Title;
                        //TextBox_Title.Text = atm.Title;
                        if (atm.NotOnUsAmount.HasValue)
                            TextBox_NotOnUsAmount.Text = atm.NotOnUsAmount.Value.ToString();
                        if (atm.OnUsAmount.HasValue)
                            TextBox_OnUsAmount.Text = atm.OnUsAmount.Value.ToString();
                        if (atm.NormalOrderCost.HasValue)
                            TextBox_NormalOrderCost.Text = atm.NormalOrderCost.ToString();
                        if (atm.EmergencyOrderCost.HasValue)
                            TextBox_EmergencyOrderCost.Text = atm.EmergencyOrderCost.ToString();

                        TextBox_Port.Text = atm.Port.ToString();
                        TextBox_DistributionPort.Text = atm.DistributionPort.HasValue ? atm.DistributionPort.ToString() : null;
                        TextBox_IP.Text = atm.IP;
                        TextBox_Port.Text = atm.Port.ToString();
                        TextBox_Cassette1_Denominations.Text = atm.Cassette1Denomination.ToString();
                        TextBox_Cassette1_Capacity.Text = atm.Cassette1Capacity.ToString();

                        TextBox_Cassette2_Denominations.Text = atm.Cassette2Denomination.ToString();
                        TextBox_Cassette2_Capacity.Text = atm.Cassette2Capacity.ToString();
                        TextBox_Cassette3_Denominations.Text = atm.Cassette3Denomination.ToString();
                        TextBox_Cassette3_Capacity.Text = atm.Cassette3Capacity.ToString();
                        TextBox_Cassette4_Denominations.Text = atm.Cassette4Denomination.ToString();
                        TextBox_Cassette4_Capacity.Text = atm.Cassette4Capacity.ToString();
                        TextBox_Cassette5_Denominations.Text = atm.Cassette5Denomination.ToString();
                        TextBox_Cassette5_Capacity.Text = atm.Cassette5Capacity.ToString();
                        TextBox_Cassette6_Denominations.Text = atm.Cassette6Denomination.ToString();
                        TextBox_Cassette6_Capacity.Text = atm.Cassette6Capacity.ToString();
                        TextBox_Cassette7_Denominations.Text = atm.Cassette7Denomination.ToString();
                        TextBox_Cassette7_Capacity.Text = atm.Cassette7Capacity.ToString();


                        TextBox_CPMCommand.Text = atm.CpmCommand;

                        if (atm.Purge1Threshold.HasValue)
                            TextBox_Purge1_Threshold.Text = atm.Purge1Threshold.Value.ToString();
                        if (atm.Purge2Threshold.HasValue)
                            TextBox_Purge2_Threshold.Text = atm.Purge2Threshold.Value.ToString();
                        if (atm.Purge3Threshold.HasValue)
                            TextBox_Purge3_Threshold.Text = atm.Purge3Threshold.Value.ToString();
                        if (atm.Purge4Threshold.HasValue)
                            TextBox_Purge4_Threshold.Text = atm.Purge4Threshold.Value.ToString();
                        if (atm.Purge5Threshold.HasValue)
                            TextBox_Purge5_Threshold.Text = atm.Purge5Threshold.Value.ToString();
                        if (atm.Purge6Threshold.HasValue)
                            TextBox_Purge6_Threshold.Text = atm.Purge6Threshold.Value.ToString();
                        if (atm.Purge7Threshold.HasValue)
                            TextBox_Purge7_Threshold.Text = atm.Purge7Threshold.Value.ToString();

                        if (atm.IsPurge1ThresholdSelected.HasValue)
                            CheckBox_Purge1_Threshold.Checked = atm.IsPurge1ThresholdSelected.Value;

                        if (atm.IsPurge2ThresholdSelected.HasValue)
                            CheckBox_Purge2_Threshold.Checked = atm.IsPurge2ThresholdSelected.Value;
                        if (atm.IsPurge3ThresholdSelected.HasValue)
                            CheckBox_Purge3_Threshold.Checked = atm.IsPurge3ThresholdSelected.Value;
                        if (atm.IsPurge4ThresholdSelected.HasValue)
                            CheckBox_Purge4_Threshold.Checked = atm.IsPurge4ThresholdSelected.Value;
                        if (atm.IsPurge5ThresholdSelected.HasValue)
                            CheckBox_Purge5_Threshold.Checked = atm.IsPurge5ThresholdSelected.Value;
                        if (atm.IsPurge6ThresholdSelected.HasValue)
                            CheckBox_Purge6_Threshold.Checked = atm.IsPurge6ThresholdSelected.Value;
                        if (atm.IsPurge7ThresholdSelected.HasValue)
                            CheckBox_Purge7_Threshold.Checked = atm.IsPurge7ThresholdSelected.Value;

                        if (atm.MinAmountForNormalDelivery.HasValue)
                            TextBox_minNormalDelivery.Text = atm.MinAmountForNormalDelivery.Value.ToString();
                        if (atm.MinOperatingBalance.HasValue)
                            TextBox_minOperatingBalance.Text = atm.MinOperatingBalance.Value.ToString();


                        if (atm.IsAtm.HasValue)
                            CheckBox_ATM.Checked = atm.IsAtm.Value;
                        if (atm.IsCcdm.HasValue)
                            CheckBox_CCDM.Checked = atm.IsCcdm.Value;
                        if (atm.IsCdm.HasValue)
                            CheckBox_CDM.Checked = atm.IsCdm.Value;

                        if (atm.CcdmCassette1Capacity.HasValue)
                            TextBox_CCDM1Capacity.Text = atm.CcdmCassette1Capacity.ToString();

                        if (atm.CcdmCassette1Threshold.HasValue)
                            TextBox_CCDM1Threshold.Text = atm.CcdmCassette1Threshold.ToString(); ;

                        if (atm.CcdmCassette2Capacity.HasValue)
                            TextBox_CCDM2Capacity.Text = atm.CcdmCassette2Capacity.ToString();

                        if (atm.CcdmCassette2Threshold.HasValue)
                            TextBox_CCDM2Threshold.Text = atm.CcdmCassette2Threshold.ToString(); ;

                        if (atm.CcdmCassette3Capacity.HasValue)
                            TextBox_CCDM3Capacity.Text = atm.CcdmCassette3Capacity.ToString();

                        if (atm.CcdmCassette3Threshold.HasValue)
                            TextBox_CCDM3Threshold.Text = atm.CcdmCassette3Threshold.ToString(); ;

                        if (atm.CcdmCassette4Capacity.HasValue)
                            TextBox_CCDM4Capacity.Text = atm.CcdmCassette4Capacity.ToString();

                        if (atm.CcdmCassette4Threshold.HasValue)
                            TextBox_CCDM4Threshold.Text = atm.CcdmCassette4Threshold.ToString(); ;



                        if (atm.CdmCassette1Capacity.HasValue)
                            TextBox_CDM1.Text = atm.CdmCassette1Capacity.ToString();

                        if (atm.CdmCassette2Capacity.HasValue)
                            TextBox_CDM2.Text = atm.CdmCassette2Capacity.ToString();

                        if (atm.CdmCassette3Capacity.HasValue)
                            TextBox_CDM3.Text = atm.CdmCassette3Capacity.ToString();

                        if (atm.CdmCassette4Capacity.HasValue)
                            TextBox_CDM4.Text = atm.CdmCassette4Capacity.ToString();




                        if (atm.CdmCassette1Threshold.HasValue)
                            TextBox_CDM1Threshold.Text = atm.CdmCassette1Threshold.ToString();


                        if (atm.CdmCassette2Threshold.HasValue)
                            TextBox_CDM2Threshold.Text = atm.CdmCassette2Threshold.ToString();
                        if (atm.CdmCassette3Threshold.HasValue)
                            TextBox_CDM3Threshold.Text = atm.CdmCassette3Threshold.ToString();
                        if (atm.CdmCassette4Threshold.HasValue)
                            TextBox_CDM4Threshold.Text = atm.CdmCassette4Threshold.ToString();

                        TextBox_GLNo.Text = atm.GlNumber;

                        TextBox_Longitude.Text = atm.Longitude;
                        TextBox_Latitude.Text = atm.Latitude;
                        TextBox_CITATMID.Text = atm.CitAtmTitle;

                        //TextBox_Capacity.Text = atm.Capacity.ToString();
                        //TextBox_cas1.Text = atm.Cassette1Denomination.ToString();
                        //TextBox_cas2.Text = atm.Cassette2Denomination.ToString();
                        //TextBox_cas3.Text = atm.Cassette3Denomination.ToString();
                        //TextBox_cas4.Text = atm.Cassette4Denomination.ToString();
                        //TextBox_cas5.Text = atm.Cassette5Denomination.ToString();
                        //TextBox_cas6.Text = atm.Cassette6Denomination.ToString();
                        //TextBox_cas7.Text = atm.Cassette7Denomination.ToString();


                        //TerminalType.TerminalTypeReader terminalReader = TerminalType.ExecuteReader("1=1");
                        //while (terminalReader.Read())
                        //    DropDownList_Type.Items.Add(new ListItem(terminalReader.CurrentTerminalType.AtmType, terminalReader.CurrentTerminalType.AtmType.ToString()));

                        DropDownList_Type.SelectedValue = atm.AtmType.ToString();

                        /*   if ((int)ConnectionFactory.ExecuteScalar("select count(*) from scheduled_downloads where atm_id = " + atm.ATMId + " and file_type_id=1") > 0)
                               CheckBox_ScheduleEJ.Checked = true;
                           else
                               CheckBox_ScheduleEJ.Checked = false;
                           */
                        RadioButton_inactive.Checked = !atm.IsActive;
                        DropDownList_NoteSetTypeID.SelectedValue = atm.NoteSetTypeId.ToString();
                        SetNoteSetTypeTitles();
                        DropDownList_CIT.SelectedValue = atm.CitId.ToString();
                        Session["noteSetId"] = atm.NoteSetTypeId;
                        //DropDownList_NoteSetType_Change(this, null);




                        CheckBox_Counter.Checked = atm.IsCounterEnabled;
                        CheckBox_EJ.Checked = atm.IsEjEnabled;
                        if (atm.Priority == 1) //1-Counter 0-EJ
                            RadioButtonList_Priority.SelectedIndex = 1;
                        else
                            RadioButtonList_Priority.SelectedIndex = 0;

                        GetGeneralInventory(atm.ATMId);
                        GetHardwareInventory(atm.ATMId);
                        GetSoftwareInventory(atm.ATMId);



                        AtmInterfaceInfo.AtmInterfaceInfoReader interfaceReader = AtmInterfaceInfo.ExecuteReader("atm_id=" + atm.ATMId);
                        int counter = 0;
                        while (interfaceReader.Read())
                        {

                            if (counter == 0)
                            {
                                TextBox_InterfaceFriendlyName1.Text = interfaceReader.CurrentAtmInterfaceInfo.InterfaceFriendlyName;
                                TextBox_IP1.Text = interfaceReader.CurrentAtmInterfaceInfo.Ip;
                            }
                            else if (counter == 1)
                            {
                                TextBox_InterfaceFriendlyName2.Text = interfaceReader.CurrentAtmInterfaceInfo.InterfaceFriendlyName;
                                TextBox_IP2.Text = interfaceReader.CurrentAtmInterfaceInfo.Ip;
                            }
                            else if (counter == 2)
                            {
                                TextBox_InterfaceFriendlyName3.Text = interfaceReader.CurrentAtmInterfaceInfo.InterfaceFriendlyName;
                                TextBox_IP3.Text = interfaceReader.CurrentAtmInterfaceInfo.Ip;
                            }
                            else if (counter == 3)
                            {
                                TextBox_InterfaceFriendlyName4.Text = interfaceReader.CurrentAtmInterfaceInfo.InterfaceFriendlyName;
                                TextBox_IP4.Text = interfaceReader.CurrentAtmInterfaceInfo.Ip;
                            }
                            else if (counter == 4)
                            {
                                TextBox_InterfaceFriendlyName5.Text = interfaceReader.CurrentAtmInterfaceInfo.InterfaceFriendlyName;
                                TextBox_IP5.Text = interfaceReader.CurrentAtmInterfaceInfo.Ip;
                            }

                            counter++;
                        }

                        interfaceReader.Close();

                        Response.Write("<script>window.parent.SelectNode('r" + atm.RegionId + "');</script>");

                        AtmClassification.AtmClassificationReader atmClassificationReader = AtmClassification.ExecuteReader("atm_id =" + atm.ATMId);
                        while (atmClassificationReader.Read())
                        {
                            foreach (ListItem listItem in CheckBoxList_Classification.Items)
                            {
                                if (listItem.Value == atmClassificationReader.CurrentAtmClassification.ClassificationId.ToString())
                                {
                                    listItem.Selected = true;
                                    break;
                                }
                            }
                        }
                        atmClassificationReader.Close();
                    }



         

                    SetAssignedUsers();

                }
                catch (Exception ex)
                {
                    Literal_error.Text = "<li>" + ex.Message + "</li>";
                    LogableTask.LogMonoActivityTask("GetATMForEditing", MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                }
        }

        private void SetAssignedUsers()
        {
            List<int> assignedUser = new List<int>(10);


            try
            {
                checkbox_list.Items.FindByValue("2").Enabled = false;
                checkbox_list.Items.FindByValue("2").Selected = true;
            }
            catch (Exception ex)
            {
                LogableTask.LogMonoActivityTask("DisableAdmin", MethodBase.GetCurrentMethod(), TraceLevel.Warning, ex);
            }


            if (Session["atm_id"] != null)
            {
                UserATMs.UserATMsReader r = UserATMs.ExecuteReader("atm_id = " + int.Parse(Session["atm_id"].ToString()));
                while (r.Read())
                    assignedUser.Add(r.CurrentUserATMs.UserId);
                r.Close();
            }
            else
                assignedUser.Add(this.appUser.UserId);


            foreach (ListItem item in checkbox_list.Items)
            {
                if (assignedUser.IndexOf(int.Parse(item.Value)) != -1 || Session["atm_id"] == null)
                {
                    item.Selected = true;
                    if (item.Value == appUser.UserId.ToString())
                        item.Enabled = false;
                }
            }

        }
        //NodeChangedFromMasterPage <-- remove



        protected void GridView_DispensedConf_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            DataTable dt = (DataTable)Session["dtDispensedConf"];
            dt.Rows[e.RowIndex].Delete();
            GridView_DispensedConf.DataSource = dt;
            GridView_DispensedConf.DataBind();
            ViewState["confChanged"] = 1;

        }

        protected void GridView_HeartBeatSchedule_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            DataTable dt = (DataTable)Session["dt"];
            dt.Rows[e.RowIndex].Delete();
            GridView_HeartBeatSchedules.DataSource = dt;
            GridView_HeartBeatSchedules.DataBind();
            ViewState["heartBeatChanged"] = 1;

        }
        protected void GridView_BatchSchedule_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            DataTable dt = (DataTable)Session["dtBatch"];
            dt.Rows[e.RowIndex].Delete();
            GridView_Batchschedule.DataSource = dt;
            GridView_Batchschedule.DataBind();
            ViewState["batchChanged"] = 1;

        }
        private DataTable GetHeartBeatDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Type");
            dt.Columns.Add("Interval", typeof(int));
            return dt;
        }
        private DataTable GetDispensedConfDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("amount", typeof(int));
            dt.Columns.Add("operator");
            dt.Columns.Add("type1", typeof(bool));
            dt.Columns.Add("type2", typeof(bool));
            dt.Columns.Add("type3", typeof(bool));
            dt.Columns.Add("type4", typeof(bool));
            return dt;
        }
        protected void Button_AddHeartBeatSchedule_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = null;

                if (Session["dt"] == null)
                {
                    Session["dt"] = GetHeartBeatDataTable();
                }
                dt = (DataTable)Session["dt"];
                StringBuilder builder = new StringBuilder();
                foreach (ListItem item in ListBox_Events.Items)
                {
                    if (item.Selected)
                    {
                        builder.Append(item.Text + ",");

                    }
                }

                if (builder.Length > 0)
                {
                    if (TextBox_Interval.Text.Length > 0)
                    {
                        bool found = false;
                        string item = builder.ToString().Remove(builder.Length - 1);
                        int interval = int.Parse(TextBox_Interval.Text);
                        foreach (DataRow dataRow in dt.Rows)
                        {
                            if (dataRow["type"].ToString() == item && int.Parse(dataRow["interval"].ToString()) == interval)
                            {
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                        {
                            DataRow dr = dt.NewRow();
                            dr[0] = item;
                            dr[1] = interval;
                            dt.Rows.Add(dr);
                            ViewState["heartBeatChanged"] = 1;
                        }
                        else
                        {
                            Literal_error.Text = "<li>Schedule already added.</li>";
                        }


                        Session["dt"] = dt;
                        GridView_HeartBeatSchedules.DataSource = dt;
                        GridView_HeartBeatSchedules.DataBind();
                    }
                    else
                    {
                        Literal_error.Text = "<li>Please enter interval for the selected event(s)</li>";
                    }
                }
                else
                {
                    Literal_error.Text = "<li>Please select at least one event type.</li>";
                }
            }
            catch (Exception ex)
            {
                Literal_error.Text = "<li>" + ex.Message + "</li>";
                LogableTask.LogMonoActivityTask("AddHeartBeatSchedule", MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
        }

        protected void Button_AddBatchSchedule_Click(object sender, EventArgs e)
        {
            try
            {

                DataTable dt = null;
                if (Session["dtBatch"] == null)
                {
                    Session["dtBatch"] = GetHeartBeatDataTable();
                }
                dt = (DataTable)Session["dtBatch"];
                StringBuilder builder = new StringBuilder();
                foreach (ListItem item in ListBox_Batch.Items)
                {
                    if (item.Selected)
                    {
                        builder.Append(item.Text + ",");

                    }
                }
                if (builder.Length > 0)
                {
                    if (TextBox_BatchInterval.Text.Length > 0)
                    {

                        bool found = false;
                        string item = builder.ToString().Remove(builder.Length - 1);
                        int interval = int.Parse(TextBox_BatchInterval.Text);
                        foreach (DataRow dataRow in dt.Rows)
                        {
                            if (dataRow["type"].ToString() == item && int.Parse(dataRow["interval"].ToString()) == interval)
                            {
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                        {
                            DataRow dr = dt.NewRow();
                            dr[0] = item;
                            dr[1] = interval;
                            dt.Rows.Add(dr);
                            ViewState["batchChanged"] = 1;
                        }
                        else
                        {
                            Literal_error.Text = "<li>Schedule already added.</li>";
                        }


                        Session["dtBatch"] = dt;
                        GridView_Batchschedule.DataSource = dt;
                        GridView_Batchschedule.DataBind();
                    }
                    else
                    {
                        Literal_error.Text = "<li>Please enter interval for the selected event(s)</li>";
                    }

                }
                else
                {
                    Literal_error.Text = "<li>Please select at least one event type.</li>";
                }




            }

            catch (Exception ex)
            {
                Literal_error.Text = "<li>" + ex.Message + "</li>";
                LogableTask.LogMonoActivityTask("AddBatchSchedule", MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }

        }
        protected void Button_Search(object sender, EventArgs e)
        {
            //Session["UserCheckBoxList"] = checkbox_list;
            //checkbox_list.Items.Clear();

            foreach (ListItem listItem in checkbox_list.Items)
            {
                if (textbox_search.Text == null || textbox_search.Text == "")
                {
                    //listItem.Enabled = true;
                    //listItem.Attributes.CssStyle.Add("visibility", "visible");
                    listItem.Attributes.CssStyle.Add("display", "true");
                }
                else
                {
                    if (listItem.Text.ToLower().Contains(textbox_search.Text.ToLower()))
                    {
                        //listItem.Attributes.CssStyle.Add("visibility", "visible");
                        listItem.Attributes.CssStyle.Add("display", "true");
                    }
                    else
                    {
                        listItem.Attributes.CssStyle.Add("display", "none");
                        //listItem.Attributes.CssStyle.Add("visibility", "hidden");
                    }
                }
               

                //if (listItem.Selected)
                //{
                //    string selectedValue = listItem.Value;
                //}
                //else
                //{
                //    //do something else 
                //}
            }

            //string d = textbox_search.Text != null && textbox_search.Text != "" ? "and user_full_name like '%" + textbox_search.Text+ "%'" : "";
            //string query = "user_is_active =1 and user_id > 1 "+ d+ " order by user_full_name";
            //AppUser.AppUserReader userReader = AppUser.ExecuteReader(query);
            //checkbox_list.DataSource = userReader;
            //checkbox_list.DataBind();
            //userReader.Close();
        }

        
        protected void Button_AddDispensedConf_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = null;

                if (Session["dtDispensedConf"] == null)
                    Session["dtDispensedConf"] = GetDispensedConfDataTable();

                dt = (DataTable)Session["dtDispensedConf"];

                DataRow[] drArray = dt.Select("amount = " + TextBox_Amount.Text + " and operator='" + DropDownList_Operator.SelectedItem.Value + "'");
                if (drArray.Length > 0)
                    Literal_error.Text = "<li>Dispensed configuration already defined for amount " + TextBox_Amount.Text + "</li>";

                else
                {
                    DataRow dr = dt.NewRow();
                    dr[0] = TextBox_Amount.Text;
                    dr[1] = DropDownList_Operator.SelectedItem.Value;
                    dr[2] = CheckBox_NotesDispensedType1.Checked;
                    dr[3] = CheckBox_NotesDispensedType2.Checked;
                    dr[4] = CheckBox_NotesDispensedType3.Checked;
                    dr[5] = CheckBox_NotesDispensedType4.Checked;
                    dt.Rows.Add(dr);

                    ViewState["confChanged"] = 1;
                    Session["dtDispensedConf"] = dt;
                    GridView_DispensedConf.DataSource = dt;
                    GridView_DispensedConf.DataBind();
                }
            }
            catch (Exception ex)
            {
                Literal_error.Text = "<li>" + ex.Message + "</li>";
                LogableTask.LogMonoActivityTask("AddDispensedConf", MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }

        }

        //ATMs.aspx
        protected void Button_Cancel_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["i"] == null)
                Response.Redirect("Atms.aspx");
            else
                Response.Redirect("Atms.aspx?i=1");
        }

        private void AddOperationalHour(int id, int atmID, string from, string to, SqlTransaction trxn)
        {
            OperationalHour operationalHour = new OperationalHour();
            operationalHour.OperationalDayId = id;
            operationalHour.OperationalHourFrom = from;
            operationalHour.OperationalHourTo = to;
            operationalHour.AtmId = atmID;
            operationalHour.Save(trxn.Connection, trxn);

        }


        protected void Button_SaveUpdateATM_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("AddATM");
            SqlTransaction trx = null;
            bool isHeartBeatConfigurationUpdated = false;
            bool isBatchConfigurationUpdated = false;

            try
            {
                string[] parts = null;
                try
                {
                    parts = Session[SessionVars.SelectedRegions.ToString()].ToString().Split(',');
                }
                catch (Exception ex)
                {
                    Literal_error.Text = "<li>Please select the parent region for the ATM.</li>";
                    return;
                }
                if (Region.LoadRegionByPk(int.Parse(parts[0])).IsOrganization == true
                    || !(bool)Session[SessionVars.isRegionSelected.ToString()])
                {
                    Literal_error.Text = "<li>Please select the parent region for the ATM.</li>";
                    return;

                }



                if (DropDownList_NoteSetTypeID.SelectedIndex == 0)
                {
                    Literal_error.Text = "<li>Please select note set type for the ATM.</li>";
                    return;

                }

                Atm atm;
                bool editMode = false;
                if (Session["atm_id"] != null) // if new
                    editMode = true;

                if (editMode)
                {
                    atm = Atm.LoadAtmByPk(int.Parse(Session["atm_id"].ToString()));
                }
                else
                {
                    atm = new Atm();
                }
                //object res = ConnectionFactory.ExecuteScalar("select top 1 region.region_name from atm inner join region on atm.region_id = region.region_id "
                //    + " where atm.title='" + TextBox_Title.Text + "' " + (editMode ? "and atm_id != " + atm.ATMId : ""));
                //if (res != null)
                //{
                //    Literal_error.Text = "<li>This ATM name already exists in region '" + res + "'. Please, provide new name.</li>";
                //    return;
                //}

                //if (TextBox_CashWithdrawal_Interval.Text.Length > 0)
                //    atm.HeartBeatCashWithdrawalInterval = int.Parse(TextBox_CashWithdrawal_Interval.Text);

                //if (TextBox_CashDeposit_Interval.Text.Length>0)
                //    atm.HeartBeatCashDepositInterval=int.Parse(TextBox_CashDeposit_Interval.Text);

                //if (TextBox_ChequeDeposit_Interval.Text.Length>0)
                //    atm.HeartBeatChequeDepositInterval=int.Parse(TextBox_ChequeDeposit_Interval.Text );

                //if (TextBox_OrderCounters_Interval.Text.Length > 0)
                //    atm.HeartBeatOrderCountersInterval = int.Parse(TextBox_OrderCounters_Interval.Text);

                //if (TextBox_OrderId_Interval.Text.Length > 0)
                //    atm.HeartBeatOrderIdInterval = int.Parse(TextBox_OrderId_Interval.Text);

                //if (TextBox_OutOfCash_Interval.Text.Length > 0)
                //    atm.HeartBeatOutOfCashInterval = int.Parse(TextBox_OutOfCash_Interval.Text);

                //Populate Batch Configuration

                //if (TextBox_BatchCashDeposit_Interval.Text.Length > 0)
                //    atm.BatchCashDepositInterval = int.Parse(TextBox_BatchCashDeposit_Interval.Text);

                //if (TextBox_BatchCashWithdrawal_Interval.Text.Length > 0)
                //    atm.BatchCashWithdrawalInterval = int.Parse(TextBox_BatchCashWithdrawal_Interval.Text);

                //if (TextBox_BatchChequeDeposit_Interval.Text.Length > 0)
                //    atm.BatchChequeDepositInterval = int.Parse(TextBox_BatchChequeDeposit_Interval.Text);

                //if (TextBox_BatchReplenishment_Interval.Text.Length > 0)
                //    atm.BatchReplenishmentInterval = int.Parse(TextBox_BatchReplenishment_Interval.Text);

                if (DropDownList_Bandwidth.SelectedIndex > -1)
                    atm.AtmBandwidthId = int.Parse(DropDownList_Bandwidth.SelectedItem.Value);

                if (TextBox_MinOperatingBalanceNormalDays.Text.Length > 0)
                    atm.MinOperatingBalanceNormalDays = decimal.Parse(TextBox_MinOperatingBalanceNormalDays.Text);

                if (TextBox_MinOperatingBalanceSalaryDays.Text.Length > 0)
                    atm.MinOperatingBalanceSalaryDays = decimal.Parse(TextBox_MinOperatingBalanceSalaryDays.Text);

                if (TextBox_Type1MinNotesThresholdValue.Text.Length > 0)
                    atm.Type1MinNotesThresholdValue = int.Parse(TextBox_Type1MinNotesThresholdValue.Text);

                if (TextBox_Type2MinNotesThresholdValue.Text.Length > 0)
                    atm.Type2MinNotesThresholdValue = int.Parse(TextBox_Type2MinNotesThresholdValue.Text);

                if (TextBox_Type3MinNotesThresholdValue.Text.Length > 0)
                    atm.Type3MinNotesThresholdValue = int.Parse(TextBox_Type3MinNotesThresholdValue.Text);

                if (TextBox_Type4MinNotesThresholdValue.Text.Length > 0)
                    atm.Type4MinNotesThresholdValue = int.Parse(TextBox_Type4MinNotesThresholdValue.Text);


                if (TextBox_Type1MinNotesThreshold.Text.Length > 0)
                    atm.Type1MinNotesThreshold = int.Parse(TextBox_Type1MinNotesThreshold.Text);

                if (TextBox_Type2MinNotesThreshold.Text.Length > 0)
                    atm.Type2MinNotesThreshold = int.Parse(TextBox_Type2MinNotesThreshold.Text);

                if (TextBox_Type3MinNotesThreshold.Text.Length > 0)
                    atm.Type3MinNotesThreshold = int.Parse(TextBox_Type3MinNotesThreshold.Text);

                if (TextBox_Type4MinNotesThreshold.Text.Length > 0)
                    atm.Type4MinNotesThreshold = int.Parse(TextBox_Type4MinNotesThreshold.Text);

                atm.IsWin7Machine = CheckBox_Win7OS.Checked;
                atm.IsSdm = CheckBox_SDM.Checked;
                atm.OnUsAmount = decimal.Parse(TextBox_OnUsAmount.Text);
                atm.NotOnUsAmount = decimal.Parse(TextBox_NotOnUsAmount.Text);
                atm.NormalOrderCost = decimal.Parse(TextBox_NormalOrderCost.Text);
                atm.EmergencyOrderCost = decimal.Parse(TextBox_EmergencyOrderCost.Text);
                atm.ReceiptTransactionCutoff = int.Parse(TextBox_ReceiptCutOff.Text);

                atm.CitAtmTitle = TextBox_CITATMID.Text;
                atm.OutOfCashThreshold = int.Parse(TextBox_OutOfCashThreshold.Text);

                if (TextBox_ChequeInactivityPeriod.Text.Length > 0)
                    atm.ChequeAllowedInactivityPeriod = int.Parse(TextBox_ChequeInactivityPeriod.Text);
                else
                    atm.ChequeAllowedInactivityPeriod = null;

                if (TextBox_BNAInactivityPeriod.Text.Length > 0)
                    atm.BnaAllowedInactivityPeriod = int.Parse(TextBox_BNAInactivityPeriod.Text);
                else
                    atm.BnaAllowedInactivityPeriod = null;

                atm.IsDffGenerationHalt = CheckBox_DisableDFFGeneration.Checked;

                atm.Description = TextBox_desc.Text;
                atm.GlNumber = TextBox_GLNo.Text;
                bool isReqParamChanged = false;
                if (editMode)
                {
                    if (atm.Cassette1Denomination != int.Parse(TextBox_Cassette1_Denominations.Text.Length == 0 ? "0" : TextBox_Cassette1_Denominations.Text)
                        || atm.Cassette1Capacity != int.Parse(TextBox_Cassette1_Capacity.Text.Length == 0 ? "0" : TextBox_Cassette1_Capacity.Text)

                        || atm.Cassette2Denomination != int.Parse(TextBox_Cassette2_Denominations.Text.Length == 0 ? "0" : TextBox_Cassette2_Denominations.Text)
                        || atm.Cassette2Capacity != int.Parse(TextBox_Cassette2_Capacity.Text.Length == 0 ? "0" : TextBox_Cassette2_Capacity.Text)

                        || atm.Cassette3Denomination != int.Parse(TextBox_Cassette3_Denominations.Text.Length == 0 ? "0" : TextBox_Cassette3_Denominations.Text)
                        || atm.Cassette3Capacity != int.Parse(TextBox_Cassette3_Capacity.Text.Length == 0 ? "0" : TextBox_Cassette3_Capacity.Text)

                        || atm.Cassette4Denomination != int.Parse(TextBox_Cassette4_Denominations.Text.Length == 0 ? "0" : TextBox_Cassette4_Denominations.Text)
                        || atm.Cassette4Capacity != int.Parse(TextBox_Cassette4_Capacity.Text.Length == 0 ? "0" : TextBox_Cassette4_Capacity.Text)

                        || atm.Cassette5Denomination != int.Parse(TextBox_Cassette5_Denominations.Text.Length == 0 ? "0" : TextBox_Cassette5_Denominations.Text)
                        || atm.Cassette5Capacity != int.Parse(TextBox_Cassette5_Capacity.Text.Length == 0 ? "0" : TextBox_Cassette5_Capacity.Text)

                        || atm.Cassette6Denomination != int.Parse(TextBox_Cassette6_Denominations.Text.Length == 0 ? "0" : TextBox_Cassette6_Denominations.Text)
                        || atm.Cassette6Capacity != int.Parse(TextBox_Cassette6_Capacity.Text.Length == 0 ? "0" : TextBox_Cassette6_Capacity.Text)

                        || atm.Cassette7Denomination != int.Parse(TextBox_Cassette7_Denominations.Text.Length == 0 ? "0" : TextBox_Cassette7_Denominations.Text)
                        || atm.Cassette7Capacity != int.Parse(TextBox_Cassette7_Capacity.Text.Length == 0 ? "0" : TextBox_Cassette7_Capacity.Text)
                        || (atm.DebugLevel == null) || atm.DebugLevel.Value != DropDownList_DebugLevel.SelectedIndex
                        || (atm.StartupSleepInterval == null) || atm.StartupSleepInterval.Value != int.Parse(TextBox_SleepInterval.Text)
                        || atm.CountsClearRetries != int.Parse(TextBox_CountsClearRetries.Text)
                        || atm.TCPTimeout != int.Parse(TextBox_TCPTimeout.Text)
                        || atm.SleepInterval != int.Parse(TextBox_DelayIteration.Text)
                        || atm.CPMCommandWait != int.Parse(TextBox_CPMCommandWait.Text)
                        || atm.CPMCommandSleep != int.Parse(TextBox_CPMCommandSleep.Text)
                        || atm.AANDCApplications1 != TextBox_AANDCApplications1.Text
                        || atm.AANDCApplications2 != TextBox_AANDCApplications2.Text
                        || atm.AANDCApplications3 != TextBox_AANDCApplications3.Text
                        || atm.AANDCApplications4 != TextBox_AANDCApplications4.Text
                        || atm.AANDCApplications5 != TextBox_AANDCApplications5.Text
                        || atm.MonitoringRetries != int.Parse(TextBox_MonitoringRetries.Text)
                        || atm.WindowSwitchSleep != int.Parse(TextBox_WindowSwitch_Sleep.Text)
                        || atm.AppSwitchSleep != int.Parse(TextBox_AppSwitch_Sleep.Text)
                        || atm.MonitoringCycleSleep != int.Parse(TextBox_MonitoringCycle_Sleep.Text)
                        || atm.CPMLogLevel != int.Parse(TextBox_CPMLogLevel.Text)
                        || atm.IsDispenserRealTimeNotificationEnabled != CheckBox_DispenserRealTimeEnabled.Checked
                        || atm.IsBNARealTimeNotificationEnabled != CheckBox_BNARealTimeEnabled.Checked
                        || atm.IsCPMRealTimeNotificationEnabled != CheckBox_CPMRealTimeEnabled.Checked
                        || atm.IsReplenishmentRealTimeNotificationEnabled != CheckBox_RepRealTimeEnabled.Checked
                        || atm.IsOutOfCashRealTimeNotificationEnabled != CheckBox_OutOfCashRealTimeEnabled.Checked
                        || atm.IsDispenserMismatchRealTimeNotificationEnabled != CheckBox_DisMismatchRealTimeEnabled.Checked
                        || atm.IsBNAMismatchRealTimeNotificationEnabled != CheckBox_BNAMismatchRealTimeEnabled.Checked
                        || atm.IsCPMMismatchRealTimeNotificationEnabled != CheckBox_CPMMismatchRealTimeEnabled.Checked
                        || atm.IsCounterExplodedRealTimeNotificationEnabled != CheckBox_CounterExplodedRealTimeEnabled.Checked
                        || atm.Type1MinimumNotes != int.Parse(TextBox_Type1MinimumNotes.Text)
                        || atm.Type2MinimumNotes != int.Parse(TextBox_Type2MinimumNotes.Text)
                        || atm.Type3MinimumNotes != int.Parse(TextBox_Type3MinimumNotes.Text)
                        || atm.Type4MinimumNotes != int.Parse(TextBox_Type4MinimumNotes.Text)
                        || atm.Type5MinimumNotes != int.Parse(TextBox_Type5MinimumNotes.Text)
                        || atm.Type6MinimumNotes != int.Parse(TextBox_Type6MinimumNotes.Text)
                        || atm.Type7MinimumNotes != int.Parse(TextBox_Type7MinimumNotes.Text)



                        )
                        isReqParamChanged = true;
                }
                else
                    isReqParamChanged = true;
                if (TextBox_allowInactivityPeriod.Text.Length > 0)
                {
                    int allowedInactivity = int.Parse(TextBox_allowInactivityPeriod.Text);
                    int defaultAllowedInactivity = int.Parse(System.Configuration.ConfigurationManager.AppSettings["DefaultInactivityPeriod"]);

                    if (allowedInactivity < defaultAllowedInactivity)
                        atm.AllowedInactivityPeriod = defaultAllowedInactivity;
                    else
                        atm.AllowedInactivityPeriod = int.Parse(TextBox_allowInactivityPeriod.Text);
                }
                else
                    atm.AllowedInactivityPeriod = null;


                atm.RetryCountAlert = int.Parse(TextBox_retryCountAlert.Text);
                atm.CpmCommand = TextBox_CPMCommand.Text;

                atm.CountsClearRetries = int.Parse(TextBox_CountsClearRetries.Text);
                atm.TCPTimeout = int.Parse(TextBox_TCPTimeout.Text);
                atm.SleepInterval = int.Parse(TextBox_DelayIteration.Text);
                atm.CPMCommandWait = int.Parse(TextBox_CPMCommandWait.Text);
                atm.CPMCommandSleep = int.Parse(TextBox_CPMCommandSleep.Text);
                atm.AANDCApplications1 = TextBox_AANDCApplications1.Text;
                atm.AANDCApplications2 = TextBox_AANDCApplications2.Text;
                atm.AANDCApplications3 = TextBox_AANDCApplications3.Text;
                atm.AANDCApplications4 = TextBox_AANDCApplications4.Text;
                atm.AANDCApplications5 = TextBox_AANDCApplications5.Text;
                atm.MonitoringRetries = int.Parse(TextBox_MonitoringRetries.Text);
                atm.WindowSwitchSleep = int.Parse(TextBox_WindowSwitch_Sleep.Text);
                atm.AppSwitchSleep = int.Parse(TextBox_AppSwitch_Sleep.Text);
                atm.MonitoringCycleSleep = int.Parse(TextBox_MonitoringCycle_Sleep.Text);
                atm.CPMLogLevel = int.Parse(TextBox_CPMLogLevel.Text);
                atm.IsDispenserRealTimeNotificationEnabled = CheckBox_DispenserRealTimeEnabled.Checked;
                atm.IsBNARealTimeNotificationEnabled = CheckBox_BNARealTimeEnabled.Checked;
                atm.IsCPMRealTimeNotificationEnabled = CheckBox_CPMRealTimeEnabled.Checked;
                atm.IsReplenishmentRealTimeNotificationEnabled = CheckBox_RepRealTimeEnabled.Checked;
                atm.IsOutOfCashRealTimeNotificationEnabled = CheckBox_OutOfCashRealTimeEnabled.Checked;
                atm.IsDispenserMismatchRealTimeNotificationEnabled = CheckBox_DisMismatchRealTimeEnabled.Checked;
                atm.IsBNAMismatchRealTimeNotificationEnabled = CheckBox_BNAMismatchRealTimeEnabled.Checked;
                atm.IsCPMMismatchRealTimeNotificationEnabled = CheckBox_CPMMismatchRealTimeEnabled.Checked;
                atm.IsCounterExplodedRealTimeNotificationEnabled = CheckBox_CounterExplodedRealTimeEnabled.Checked;
                atm.Type1MinimumNotes = int.Parse(TextBox_Type1MinimumNotes.Text);
                atm.Type2MinimumNotes = int.Parse(TextBox_Type2MinimumNotes.Text);
                atm.Type3MinimumNotes = int.Parse(TextBox_Type3MinimumNotes.Text);
                atm.Type4MinimumNotes = int.Parse(TextBox_Type4MinimumNotes.Text);
                atm.Type5MinimumNotes = int.Parse(TextBox_Type5MinimumNotes.Text);
                atm.Type6MinimumNotes = int.Parse(TextBox_Type6MinimumNotes.Text);
                atm.Type7MinimumNotes = int.Parse(TextBox_Type7MinimumNotes.Text);
                //if (RadioButtonList_Search.SelectedIndex==1)
                //    atm.Location = "";
                //else
                atm.Location = TextBox_Location.Text;
                atm.RetryCountCounterFile = int.Parse(TextBox_NoOfRetryCounterFile.Text);
                atm.RetryCountDatetimeSchedule = int.Parse(TextBox_NoOfRetryDateTimeSync.Text);
                atm.RetryCountRestartSchedule = int.Parse(TextBox_NoOfRetryRestart.Text);
                //atm.RetryCountCashOrderDownload = int.Parse(TextBox_NumberOfRetryCashOrderDownload.Text);
                atm.RetryCountCashOrderUpload = int.Parse(TextBox_NumberOfRetryCashOrderUpload.Text);
                atm.RetryCountConfUpload = int.Parse(TextBox_NumberOfRetryConfUpload.Text);
                ///atm.RetryCountDffUpload = int.Parse(TextBox_NumberOfRetryDFFUpload.Text);

                StringBuilder builder = new StringBuilder();
                int organizationID = GetOrganization(int.Parse(Session[SessionVars.SelectedRegion.ToString()].ToString()));
                McnAtms.McnAtmsReader McnReader = McnAtms.ExecuteReader("region_id=" + organizationID);

                while (McnReader.Read())
                {
                    builder.Append(McnReader.CurrentMcnAtms.AtmId + ",");
                }

                McnReader.Close();

                object res = ConnectionFactory.ExecuteScalar("select top 1 title from atm where atm.ip='" + TextBox_IP.Text + "' and atm.ip in (select ip from atm where atm_id in (" + builder.ToString() + "-1)) "
                    + (editMode ? "and atm_id != " + atm.ATMId : ""));
                if (res != null)
                {
                    Literal_error.Text = "<li>This IP is already defined for ATM '" + res + "'. Please, provide correct IP.</li>";
                    return;
                }


                res = ConnectionFactory.ExecuteScalar("select top 1 title from atm where title='" + TextBox_ATMFriendlyName.Text + "' and atm.title in (select title from atm where atm_id in (" + builder.ToString() + "-1)) "
                  + (editMode ? "and atm_id != " + atm.ATMId : ""));
                if (res != null)
                {
                    Literal_error.Text = "<li>This friendly name already defined for ATM '" + res + "'. Please, provide different friendly name.</li>";
                    return;
                }



                if (editMode)
                {
                    atm.ModifiedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                    //atm.ModificationTime = DateTime.Now;
                }
                else
                {
                    atm.CreatedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                    atm.CreationTime = DateTime.Now;
                    atm.LastWincorSent = DateTime.Now.AddDays(-2);
                }
                atm.NoteSetTypeId = int.Parse(DropDownList_NoteSetTypeID.SelectedItem.Value);
                atm.CitId = !string.IsNullOrEmpty(DropDownList_CIT.SelectedItem.Value) ? int.Parse(DropDownList_CIT.SelectedItem.Value) : (int?)null;

                // AppSetting objappsetting = (AppSetting)Application[ApplicationVars.AppSettings.ToString()];

                atm.IsAtm = CheckBox_ATM.Checked;
                atm.IsCcdm = CheckBox_CCDM.Checked;
                atm.IsCdm = CheckBox_CDM.Checked;
                atm.ExcludeDff = CheckBox_ExcludeDFFGeneration.Checked;
                atm.IsRecycler = CheckBox_isRecycler.Checked;

                atm.IsSwapDefaultReplenishment = bool.Parse(RadioButtonList_Type.SelectedItem.Value);

                if (TextBox_CCDM1Capacity.Text.Length > 0)
                    atm.CcdmCassette1Capacity = int.Parse(TextBox_CCDM1Capacity.Text);
                else
                    atm.CcdmCassette1Capacity = null;

                if (TextBox_CCDM1Threshold.Text.Length > 0)
                    atm.CcdmCassette1Threshold = int.Parse(TextBox_CCDM1Threshold.Text);
                else
                    atm.CcdmCassette1Threshold = null;

                if (TextBox_CCDM2Capacity.Text.Length > 0)
                    atm.CcdmCassette2Capacity = int.Parse(TextBox_CCDM2Capacity.Text);
                else
                    atm.CcdmCassette2Capacity = null;

                if (TextBox_CCDM2Threshold.Text.Length > 0)
                    atm.CcdmCassette2Threshold = int.Parse(TextBox_CCDM2Threshold.Text);
                else
                    atm.CcdmCassette2Threshold = null;

                if (TextBox_CCDM3Capacity.Text.Length > 0)
                    atm.CcdmCassette3Capacity = int.Parse(TextBox_CCDM3Capacity.Text);
                else
                    atm.CcdmCassette3Capacity = null;

                if (TextBox_CCDM3Threshold.Text.Length > 0)
                    atm.CcdmCassette3Threshold = int.Parse(TextBox_CCDM3Threshold.Text);
                else
                    atm.CcdmCassette3Threshold = null;

                if (TextBox_CCDM4Capacity.Text.Length > 0)
                    atm.CcdmCassette4Capacity = int.Parse(TextBox_CCDM3Capacity.Text);
                else
                    atm.CcdmCassette4Capacity = null;

                if (TextBox_CCDM4Threshold.Text.Length > 0)
                    atm.CcdmCassette4Threshold = int.Parse(TextBox_CCDM4Threshold.Text);
                else
                    atm.CcdmCassette4Threshold = null;


                if (TextBox_CDM1.Text.Length > 0)
                    atm.CdmCassette1Capacity = int.Parse(TextBox_CDM1.Text);
                else
                    atm.CdmCassette1Capacity = null;

                if (TextBox_CDM2.Text.Length > 0)
                    atm.CdmCassette2Capacity = int.Parse(TextBox_CDM2.Text);
                else
                    atm.CdmCassette2Capacity = null;

                if (TextBox_CDM3.Text.Length > 0)
                    atm.CdmCassette3Capacity = int.Parse(TextBox_CDM3.Text);
                else
                    atm.CdmCassette3Capacity = null;

                if (TextBox_CDM4.Text.Length > 0)
                    atm.CdmCassette4Capacity = int.Parse(TextBox_CDM4.Text);
                else
                    atm.CdmCassette4Capacity = null;

                if (TextBox_CDM1Threshold.Text.Length > 0)
                    atm.CdmCassette1Threshold = int.Parse(TextBox_CDM1Threshold.Text);
                else
                    atm.CdmCassette1Threshold = null;

                if (TextBox_CDM2Threshold.Text.Length > 0)
                    atm.CdmCassette2Threshold = int.Parse(TextBox_CDM2Threshold.Text);
                else
                    atm.CdmCassette2Threshold = null;

                if (TextBox_CDM3Threshold.Text.Length > 0)
                    atm.CdmCassette3Threshold = int.Parse(TextBox_CDM3Threshold.Text);
                else
                    atm.CdmCassette3Threshold = null;

                if (TextBox_CDM4Threshold.Text.Length > 0)
                    atm.CdmCassette4Threshold = int.Parse(TextBox_CDM4Threshold.Text);
                else
                    atm.CdmCassette4Threshold = null;





                if (TextBox_minNormalDelivery.Text.Length > 0)
                    atm.MinAmountForNormalDelivery = decimal.Parse(TextBox_minNormalDelivery.Text);
                if (TextBox_minOperatingBalance.Text.Length > 0)
                    atm.MinOperatingBalance = decimal.Parse(TextBox_minOperatingBalance.Text);






                atm.Port = int.Parse(TextBox_Port.Text);
                atm.DistributionPort = int.Parse(string.IsNullOrEmpty(TextBox_DistributionPort.Text) ? "0" : TextBox_DistributionPort.Text);

                atm.SuspendCashOrder = CheckBox_SuspendCashOrder.Checked;
                atm.IsActive = RadioButton_active.Checked;
                //atm.AtmFriendlyName = TextBox_ATMFriendlyName.Text;
                atm.IP = TextBox_IP.Text;
                atm.Port = int.Parse(TextBox_Port.Text);
                atm.RegionId = int.Parse(Session[SessionVars.SelectedRegion.ToString()].ToString());
                atm.Title = TextBox_ATMFriendlyName.Text;
                //atm.HasBNA = CheckBox_bna.Checked;
                atm.AtmType = DropDownList_Type.SelectedValue;
                //atm.HasForEx = false;
                //atm.LastWincorSent = DateTime.Now.AddDays(-2);


                atm.DebugLevel = byte.Parse(DropDownList_DebugLevel.SelectedIndex.ToString());
                atm.StartupSleepInterval = int.Parse(TextBox_SleepInterval.Text);


                if (TextBox_Cassette1_Denominations.Text.Length > 0)
                    atm.Cassette1Denomination = int.Parse(TextBox_Cassette1_Denominations.Text);
                else
                    atm.Cassette1Denomination = 0;

                if (TextBox_Cassette1_Capacity.Text.Length > 0)
                    atm.Cassette1Capacity = int.Parse(TextBox_Cassette1_Capacity.Text);
                else
                    atm.Cassette1Capacity = 0;




                if (TextBox_Cassette2_Denominations.Text.Length > 0)
                    atm.Cassette2Denomination = int.Parse(TextBox_Cassette2_Denominations.Text);
                else
                    atm.Cassette2Denomination = 0;

                if (TextBox_Cassette2_Capacity.Text.Length > 0)
                    atm.Cassette2Capacity = int.Parse(TextBox_Cassette2_Capacity.Text);
                else
                    atm.Cassette2Capacity = 0;


                if (TextBox_Cassette3_Denominations.Text.Length > 0)
                    atm.Cassette3Denomination = int.Parse(TextBox_Cassette3_Denominations.Text);
                else
                    atm.Cassette3Denomination = 0;

                if (TextBox_Cassette3_Capacity.Text.Length > 0)
                    atm.Cassette3Capacity = int.Parse(TextBox_Cassette3_Capacity.Text);
                else
                    atm.Cassette3Capacity = 0;


                if (TextBox_Cassette4_Denominations.Text.Length > 0)
                    atm.Cassette4Denomination = int.Parse(TextBox_Cassette4_Denominations.Text);
                else
                    atm.Cassette4Denomination = 0;

                if (TextBox_Cassette4_Capacity.Text.Length > 0)
                    atm.Cassette4Capacity = int.Parse(TextBox_Cassette4_Capacity.Text);
                else
                    atm.Cassette4Capacity = 0;



                if (TextBox_Cassette5_Denominations.Text.Length > 0)
                    atm.Cassette5Denomination = int.Parse(TextBox_Cassette5_Denominations.Text);
                else
                    atm.Cassette5Denomination = 0;

                if (TextBox_Cassette5_Capacity.Text.Length > 0)
                    atm.Cassette5Capacity = int.Parse(TextBox_Cassette5_Capacity.Text);
                else
                    atm.Cassette5Capacity = 0;



                if (TextBox_Cassette6_Denominations.Text.Length > 0)
                    atm.Cassette6Denomination = int.Parse(TextBox_Cassette6_Denominations.Text);
                else
                    atm.Cassette6Denomination = 0;

                if (TextBox_Cassette6_Capacity.Text.Length > 0)
                    atm.Cassette6Capacity = int.Parse(TextBox_Cassette6_Capacity.Text);
                else
                    atm.Cassette6Capacity = 0;

                if (TextBox_Cassette7_Denominations.Text.Length > 0)
                    atm.Cassette7Denomination = int.Parse(TextBox_Cassette7_Denominations.Text);
                else
                    atm.Cassette7Denomination = 0;

                if (TextBox_Cassette7_Capacity.Text.Length > 0)
                    atm.Cassette7Capacity = int.Parse(TextBox_Cassette7_Capacity.Text);
                else
                    atm.Cassette7Capacity = 0;

                if (TextBox_Purge1_Threshold.Text.Length > 0)
                    atm.Purge1Threshold = int.Parse(TextBox_Purge1_Threshold.Text);
                //if (CheckBox_Purge1_Threshold.Checked)
                atm.IsPurge1ThresholdSelected = CheckBox_Purge1_Threshold.Checked;


                if (TextBox_Purge2_Threshold.Text.Length > 0)
                    atm.Purge2Threshold = int.Parse(TextBox_Purge2_Threshold.Text);
                //if (CheckBox_Purge2_Threshold.Checked)
                atm.IsPurge2ThresholdSelected = CheckBox_Purge2_Threshold.Checked;

                if (TextBox_Purge3_Threshold.Text.Length > 0)
                    atm.Purge3Threshold = int.Parse(TextBox_Purge3_Threshold.Text);
                //    if (CheckBox_Purge3_Threshold.Checked)
                atm.IsPurge3ThresholdSelected = CheckBox_Purge3_Threshold.Checked;


                if (TextBox_Purge4_Threshold.Text.Length > 0)
                    atm.Purge4Threshold = int.Parse(TextBox_Purge4_Threshold.Text);
                //if (CheckBox_Purge4_Threshold.Checked)
                atm.IsPurge4ThresholdSelected = CheckBox_Purge4_Threshold.Checked;

                if (TextBox_Purge5_Threshold.Text.Length > 0)
                    atm.Purge5Threshold = int.Parse(TextBox_Purge5_Threshold.Text);
                //if (CheckBox_Purge5_Threshold.Checked)
                atm.IsPurge5ThresholdSelected = CheckBox_Purge5_Threshold.Checked;


                if (TextBox_Purge6_Threshold.Text.Length > 0)
                    atm.Purge6Threshold = int.Parse(TextBox_Purge6_Threshold.Text);
                //if (CheckBox_Purge6_Threshold.Checked)
                atm.IsPurge6ThresholdSelected = CheckBox_Purge6_Threshold.Checked;


                if (TextBox_Purge7_Threshold.Text.Length > 0)
                    atm.Purge7Threshold = int.Parse(TextBox_Purge7_Threshold.Text);
                //if (CheckBox_Purge7_Threshold.Checked)
                atm.IsPurge7ThresholdSelected = CheckBox_Purge7_Threshold.Checked;




                if (TextBox_SampleSize.Text.Length > 0)
                    atm.NoOfDispensedTransactionsToMonitor = int.Parse(TextBox_SampleSize.Text);






                //if (TextBox_Capacity.Text.Length>0)
                //    atm.Capacity = int.Parse(TextBox_Capacity.Text);
                //if (TextBox_cas1.Text.Length > 0)
                //    atm.Cassette1Denomination = short.Parse(TextBox_cas1.Text);
                //if (TextBox_cas2.Text.Length > 0)
                //    atm.Cassette2Denomination = short.Parse(TextBox_cas2.Text);
                //if (TextBox_cas3.Text.Length > 0)
                //    atm.Cassette3Denomination = short.Parse(TextBox_cas3.Text);
                //if (TextBox_cas4.Text.Length > 0)
                //    atm.Cassette4Denomination = short.Parse(TextBox_cas4.Text);
                //if (TextBox_cas5.Text.Length > 0)
                //    atm.Cassette5Denomination = short.Parse(TextBox_cas5.Text);
                //if (TextBox_cas6.Text.Length > 0)
                //    atm.Cassette6Denomination = short.Parse(TextBox_cas6.Text);
                //if (TextBox_cas7.Text.Length > 0)
                //    atm.Cassette7Denomination = short.Parse(TextBox_cas7.Text);
                atm.IsCounterEnabled = CheckBox_Counter.Checked;
                atm.IsEjEnabled = CheckBox_EJ.Checked;
                atm.Priority = int.Parse(RadioButtonList_Priority.SelectedItem.Value);


                atm.Longitude = TextBox_Longitude.Text;

                atm.Latitude = TextBox_Latitude.Text;


                SqlConnection conn = ConnectionFactory.GetNewConnection();
                conn.Open();
                trx = conn.BeginTransaction();
                if (atm.MessageProcessorId == null)
                {
                    atm.MessageProcessorId = (int)ConnectionFactory.ExecuteScalar("select isnull(max(message_processor_id),1) from atm");
                }
                if (DropDownList_Hour.SelectedIndex > 0 && DropDownList_Min.SelectedIndex > 0)
                {
                    atm.InitEjExecTime = DateTime.Today.ToString("ddMMyyyy") + DropDownList_Hour.SelectedItem.Value + DropDownList_Min.SelectedItem.Value + "00";
                }
                else
                {
                    atm.InitEjExecTime = null;
                }


                if (TextBox_InactivityPeriodNormalDays.Text.Length > 0)
                    atm.InactivityPeriodNormalDays = int.Parse(TextBox_InactivityPeriodNormalDays.Text);
                else
                    atm.InactivityPeriodNormalDays = null;

                if (TextBox_InactivityPeriodSalaryDays.Text.Length > 0)
                    atm.InactivityPeriodSalaryDays = int.Parse(TextBox_InactivityPeriodSalaryDays.Text);
                else
                    atm.InactivityPeriodSalaryDays = null;



                if (TextBox_NormalDaySuggestedNotesForType1.Text.Length > 0)
                    atm.Type1SuggestedNotesNormalDays = int.Parse(TextBox_NormalDaySuggestedNotesForType1.Text);
                else
                    atm.Type1SuggestedNotesNormalDays = 0;

                if (TextBox_NormalDaySuggestedNotesForType2.Text.Length > 0)
                    atm.Type2SuggestedNotesNormalDays = int.Parse(TextBox_NormalDaySuggestedNotesForType2.Text);
                else
                    atm.Type2SuggestedNotesNormalDays = 0;

                if (TextBox_NormalDaySuggestedNotesForType3.Text.Length > 0)
                    atm.Type3SuggestedNotesNormalDays = int.Parse(TextBox_NormalDaySuggestedNotesForType3.Text);
                else
                    atm.Type3SuggestedNotesNormalDays = 0;

                if (TextBox_NormalDaySuggestedNotesForType4.Text.Length > 0)
                    atm.Type4SuggestedNotesNormalDays = int.Parse(TextBox_NormalDaySuggestedNotesForType4.Text);
                else
                    atm.Type4SuggestedNotesNormalDays = 0;

                if (TextBox_NormalDaySuggestedNotesForType5.Text.Length > 0)
                    atm.Type5SuggestedNotesNormalDays = int.Parse(TextBox_NormalDaySuggestedNotesForType5.Text);
                else
                    atm.Type5SuggestedNotesNormalDays = 0;

                if (TextBox_NormalDaySuggestedNotesForType6.Text.Length > 0)
                    atm.Type6SuggestedNotesNormalDays = int.Parse(TextBox_NormalDaySuggestedNotesForType6.Text);
                else
                    atm.Type6SuggestedNotesNormalDays = 0;

                if (TextBox_NormalDaySuggestedNotesForType7.Text.Length > 0)
                    atm.Type7SuggestedNotesNormalDays = int.Parse(TextBox_NormalDaySuggestedNotesForType7.Text);
                else
                    atm.Type7SuggestedNotesNormalDays = 0;



                //
                if (TextBox_SalaryDaySuggestedNotesForType1.Text.Length > 0)
                    atm.Type1SuggestedNotesSalaryDays = int.Parse(TextBox_SalaryDaySuggestedNotesForType1.Text);
                else
                    atm.Type1SuggestedNotesSalaryDays = 0;

                if (TextBox_SalaryDaySuggestedNotesForType2.Text.Length > 0)
                    atm.Type2SuggestedNotesSalaryDays = int.Parse(TextBox_SalaryDaySuggestedNotesForType2.Text);
                else
                    atm.Type2SuggestedNotesSalaryDays = 0;

                if (TextBox_SalaryDaySuggestedNotesForType3.Text.Length > 0)
                    atm.Type3SuggestedNotesSalaryDays = int.Parse(TextBox_SalaryDaySuggestedNotesForType3.Text);
                else
                    atm.Type3SuggestedNotesSalaryDays = 0;

                if (TextBox_SalaryDaySuggestedNotesForType4.Text.Length > 0)
                    atm.Type4SuggestedNotesSalaryDays = int.Parse(TextBox_SalaryDaySuggestedNotesForType4.Text);
                else
                    atm.Type4SuggestedNotesSalaryDays = 0;

                if (TextBox_SalaryDaySuggestedNotesForType5.Text.Length > 0)
                    atm.Type5SuggestedNotesSalaryDays = int.Parse(TextBox_SalaryDaySuggestedNotesForType5.Text);
                else
                    atm.Type5SuggestedNotesSalaryDays = 0;

                if (TextBox_SalaryDaySuggestedNotesForType6.Text.Length > 0)
                    atm.Type6SuggestedNotesSalaryDays = int.Parse(TextBox_SalaryDaySuggestedNotesForType6.Text);
                else
                    atm.Type6SuggestedNotesSalaryDays = 0;

                if (TextBox_SalaryDaySuggestedNotesForType7.Text.Length > 0)
                    atm.Type7SuggestedNotesSalaryDays = int.Parse(TextBox_SalaryDaySuggestedNotesForType7.Text);
                else
                    atm.Type7SuggestedNotesSalaryDays = 0;

                if (TextBox_AvgDispensedSalaryDays.Text.Length > 0)
                    atm.AvgDispensedSalaryDays = decimal.Parse(TextBox_AvgDispensedSalaryDays.Text);


                if (TextBox_AvgDispensed.Text.Length > 0)
                    atm.AvgDispensed = decimal.Parse(TextBox_AvgDispensed.Text);

                if (TextBox_SpareCash.Text.Length > 0)
                    atm.SpareCash = decimal.Parse(TextBox_SpareCash.Text);

                if (DropDownList_DispensingBehavior.SelectedIndex == 0)
                    atm.DispensingBehavior = 0;
                else
                    atm.DispensingBehavior = 1;






                atm.Save(conn, trx);
                //ATM operational hour handling...

                ConnectionFactory.ExecuteQuery("delete operational_hour where atm_id =" + atm.ATMId, trx);

                if (DropDownList_FromDay1Hour.SelectedIndex > 0)
                    AddOperationalHour(1, atm.ATMId, DropDownList_FromDay1Hour.SelectedItem.Value.PadLeft(2, '0') + ":" + DropDownList_FromDay1Min.SelectedItem.Value.PadLeft(2, '0'),
                        DropDownList_ToDay1Hour.SelectedItem.Value.PadLeft(2, '0') + ":" + DropDownList_ToDay1Min.SelectedItem.Value.PadLeft(2, '0'), trx);

                if (DropDownList_FromDay2Hour.SelectedIndex > 0)
                    AddOperationalHour(2, atm.ATMId, DropDownList_FromDay2Hour.SelectedItem.Value.PadLeft(2, '0') + ":" + DropDownList_FromDay2Min.SelectedItem.Value.PadLeft(2, '0'),
                        DropDownList_ToDay2Hour.SelectedItem.Value.PadLeft(2, '0') + ":" + DropDownList_ToDay2Min.SelectedItem.Value.PadLeft(2, '0'), trx);

                if (DropDownList_FromDay3Hour.SelectedIndex > 0)
                    AddOperationalHour(3, atm.ATMId, DropDownList_FromDay3Hour.SelectedItem.Value.PadLeft(2, '0') + ":" + DropDownList_FromDay3Min.SelectedItem.Value.PadLeft(2, '0'),
                        DropDownList_ToDay3Hour.SelectedItem.Value.PadLeft(2, '0') + ":" + DropDownList_ToDay3Min.SelectedItem.Value.PadLeft(2, '0'), trx);

                if (DropDownList_FromDay4Hour.SelectedIndex > 0)
                    AddOperationalHour(4, atm.ATMId, DropDownList_FromDay4Hour.SelectedItem.Value.PadLeft(2, '0') + ":" + DropDownList_FromDay4Min.SelectedItem.Value.PadLeft(2, '0'),
                        DropDownList_ToDay4Hour.SelectedItem.Value.PadLeft(2, '0') + ":" + DropDownList_ToDay4Min.SelectedItem.Value.PadLeft(2, '0'), trx);

                if (DropDownList_FromDay5Hour.SelectedIndex > 0)
                    AddOperationalHour(5, atm.ATMId, DropDownList_FromDay5Hour.SelectedItem.Value.PadLeft(2, '0') + ":" + DropDownList_FromDay5Min.SelectedItem.Value.PadLeft(2, '0'),
                        DropDownList_ToDay5Hour.SelectedItem.Value.PadLeft(2, '0') + ":" + DropDownList_ToDay5Min.SelectedItem.Value.PadLeft(2, '0'), trx);

                if (DropDownList_FromDay6Hour.SelectedIndex > 0)
                    AddOperationalHour(6, atm.ATMId, DropDownList_FromDay6Hour.SelectedItem.Value.PadLeft(2, '0') + ":" + DropDownList_FromDay6Min.SelectedItem.Value.PadLeft(2, '0'),
                        DropDownList_ToDay6Hour.SelectedItem.Value.PadLeft(2, '0') + ":" + DropDownList_ToDay6Min.SelectedItem.Value.PadLeft(2, '0'), trx);


                if (DropDownList_FromDay7Hour.SelectedIndex > 0)
                    AddOperationalHour(7, atm.ATMId, DropDownList_FromDay7Hour.SelectedItem.Value.PadLeft(2, '0') + ":" + DropDownList_FromDay7Min.SelectedItem.Value.PadLeft(2, '0'),
                        DropDownList_ToDay7Hour.SelectedItem.Value.PadLeft(2, '0') + ":" + DropDownList_ToDay7Min.SelectedItem.Value.PadLeft(2, '0'), trx);


                //////Save Heartbeat schedules;


                if (ViewState["heartBeatChanged"] != null)
                {
                    ConnectionFactory.ExecuteQuery("delete heart_beat_schedule where atm_id =" + atm.ATMId, trx);
                    for (int i = 0; i < GridView_HeartBeatSchedules.Rows.Count; i++)
                    {
                        HeartBeatSchedule heartBeatSchedule = new HeartBeatSchedule();
                        heartBeatSchedule.EventName = GridView_HeartBeatSchedules.Rows[i].Cells[1].Text;
                        heartBeatSchedule.Interval = int.Parse(GridView_HeartBeatSchedules.Rows[i].Cells[2].Text);
                        heartBeatSchedule.AtmId = atm.ATMId;
                        heartBeatSchedule.Save(conn, trx);
                        isHeartBeatConfigurationUpdated = true;
                    }
                }
                if (ViewState["batchChanged"] != null)
                {
                    ConnectionFactory.ExecuteQuery("delete batch_schedule where atm_id =" + atm.ATMId, trx);
                    for (int i = 0; i < GridView_Batchschedule.Rows.Count; i++)
                    {
                        BatchSchedule batchSchedule = new BatchSchedule();
                        batchSchedule.EventName = GridView_Batchschedule.Rows[i].Cells[1].Text;
                        batchSchedule.Interval = int.Parse(GridView_Batchschedule.Rows[i].Cells[2].Text);
                        batchSchedule.AtmId = atm.ATMId;
                        batchSchedule.Save(conn, trx);
                        isBatchConfigurationUpdated = true;
                    }
                }


                if (ViewState["confChanged"] != null)
                {
                    ConnectionFactory.ExecuteQuery("delete dispensed_logic where atm_id =" + atm.ATMId, trx);
                    DataTable dt = (DataTable)Session["dtDispensedConf"];

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DispensedLogic dispensedLogic = new DispensedLogic();
                        dispensedLogic.OperatorId =
                            (dt.Rows[i]["operator"].ToString() == "Greator than equal to" ? 0 :
                            dt.Rows[i]["operator"].ToString() == "Greator than" ? 1 :
                            dt.Rows[i]["operator"].ToString() == "Less than equal to" ? 2 :
                            dt.Rows[i]["operator"].ToString() == "Less than" ? 3 : -1);
                        dispensedLogic.Amount = int.Parse(dt.Rows[i]["amount"].ToString());
                        dispensedLogic.Type1 = bool.Parse(dt.Rows[i]["type1"].ToString());
                        dispensedLogic.Type2 = bool.Parse(dt.Rows[i]["type2"].ToString());
                        dispensedLogic.Type3 = bool.Parse(dt.Rows[i]["type3"].ToString());

                        dispensedLogic.Type4 = bool.Parse(dt.Rows[i]["type4"].ToString());

                        dispensedLogic.AtmId = atm.ATMId;
                        dispensedLogic.Save(conn, trx);
                    }
                }
                ///////


                CcmsAtmNoteSet ccmsAtmNoteSet = CcmsAtmNoteSet.LoadCcmsAtmNoteSet("atm_id=" + atm.ATMId);
                if (ccmsAtmNoteSet == null)
                    ccmsAtmNoteSet = new CcmsAtmNoteSet();
                ccmsAtmNoteSet.AtmId = atm.ATMId;
                CcmsOrgNoteSet ccmsOrgNoteSet = CcmsOrgNoteSet.LoadCcmsOrgNoteSet("name='" + DropDownList_NoteSetTypeID.SelectedItem.Text + "'");
                if (ccmsOrgNoteSet != null)
                    ccmsAtmNoteSet.OrgNoteSetId = ccmsOrgNoteSet.Id;
                else
                    throw new Exception("Org Note Set Type does not exists : " + DropDownList_NoteSetTypeID.SelectedItem.Text);


                ccmsAtmNoteSet.Save(conn, trx);

                if (isReqParamChanged)
                {
                    if ((int)ConnectionFactory.ExecuteScalar("select count(task_id) from task where task_type_id=" + (int)EnumTaskType.Configuration + " and status in ('scheduled','uploadingDisconnected') and atm_id =" + atm.ATMId) == 0)
                    {
                        CreateTask(trx, atm, conn, ((int)EnumTaskType.Configuration));
                    }
                }
                if (isHeartBeatConfigurationUpdated)
                {
                    if ((int)ConnectionFactory.ExecuteScalar("select count(task_id) from task where task_type_id=" + (int)EnumTaskType.HeartbeatConfiguration + " and status in ('scheduled','uploadingDisconnected') and atm_id =" + atm.ATMId) == 0)
                    {
                        CreateTask(trx, atm, conn, ((int)EnumTaskType.HeartbeatConfiguration));
                    }
                }
                if (isBatchConfigurationUpdated)
                {
                    if ((int)ConnectionFactory.ExecuteScalar("select count(task_id) from task where task_type_id=" + (int)EnumTaskType.BatchConfiguration + " and status in ('scheduled','uploadingDisconnected') and atm_id =" + atm.ATMId) == 0)
                    {
                        CreateTask(trx, atm, conn, ((int)EnumTaskType.BatchConfiguration));
                    }
                }

                if (CheckBox_Win7OS.Checked)
                {
                    FileType fileType = FileType.LoadFileType("file_type_title like '%win7%'");
                    if (fileType != null)
                    {
                        if ((int)ConnectionFactory.ExecuteScalar("select count(*) from scheduled_downloads where atm_id = " + atm.ATMId + " and file_type_id=" + fileType.FileTypeId) == 0)
                        {
                            ScheduledDownloads objScheduledDwnlds = new ScheduledDownloads();
                            objScheduledDwnlds.ATMId = atm.ATMId;
                            objScheduledDwnlds.FileTypeId = fileType.FileTypeId;
                            objScheduledDwnlds.Save(conn, trx);
                        }
                    }
                }
                else
                {
                    if ((int)ConnectionFactory.ExecuteScalar("select count(*) from scheduled_downloads where atm_id = " + atm.ATMId + " and file_type_id=1") == 0)
                    {
                        ScheduledDownloads objScheduledDwnlds = new ScheduledDownloads();
                        objScheduledDwnlds.ATMId = atm.ATMId;
                        objScheduledDwnlds.FileTypeId = 1;
                        objScheduledDwnlds.Save(conn, trx);
                    }
                }

                int organization_id = GetOrganization(atm.RegionId);
                ConnectionFactory.ExecuteScalar("delete mcn_atms where atm_id = " + atm.ATMId);
                McnAtms region_atm = new McnAtms();
                region_atm.AtmId = atm.ATMId;
                region_atm.Mcn = Region.LoadRegionByPk(organization_id).MCN;
                region_atm.RegionId = organization_id;
                region_atm.Save(conn, trx);

                /*if (CheckBox_ScheduleEJ.Checked && !objappsetting.IsGasperexchangeConfigured.Value)
                {
                    if ((int)ConnectionFactory.ExecuteScalar("select count(*) from scheduled_downloads where atm_id = " + atm.ATMId + " and file_type_id=1") == 0)
                    {
                        ScheduledDownloads objScheduledDwnlds = new ScheduledDownloads();
                        objScheduledDwnlds.ATMId = atm.ATMId;
                        objScheduledDwnlds.FileTypeId = 1;
                        objScheduledDwnlds.Save(conn, trx);
                    }
                }
                else*/
                // ConnectionFactory.ExecuteScalar("delete scheduled_downloads where atm_id = " + atm.ATMId + " and file_type_id=1", trx);


                //(new UserATMs(this.appUser.UserId, atm.ATMId)).Save(conn, trx);
                //(new UserATMs(1, atm.ATMId)).Save(conn, trx);
                //(new UserATMs(2, atm.ATMId)).Save(conn, trx);

                //try
                //{
                ConnectionFactory.ExecuteQuery("delete from user_ATMs where atm_id = " + atm.ATMId, trx);
                if (System.Configuration.ConfigurationManager.AppSettings["AssignATMToAllUsers"] == "1")
                {

                    Avanza.CCMS.DAL.AppUser.AppUserReader userReader = Avanza.CCMS.DAL.AppUser.ExecuteReader("1=1");
                    try
                    {
                        while (userReader.Read())
                            (new UserATMs(userReader.CurrentAppUser.UserId, atm.ATMId)).Save(conn, trx);
                    }
                    finally
                    {
                        userReader.Close();
                    }
                }
                else
                {
                    foreach (ListItem item in checkbox_list.Items)
                    {
                        if (item.Selected)
                            (new UserATMs(int.Parse(item.Value), atm.ATMId)).Save(conn, trx);
                    }
                }

                ConnectionFactory.ExecuteQuery("delete from atm_interface_info where atm_id = " + atm.ATMId, trx);

                if (TextBox_InterfaceFriendlyName1.Text.Length > 0 && TextBox_IP1.Text.Length > 0)
                {
                    AddNetworkInterface(TextBox_InterfaceFriendlyName1.Text, TextBox_IP1.Text, atm.ATMId, trx);
                }
                if (TextBox_InterfaceFriendlyName2.Text.Length > 0 && TextBox_IP2.Text.Length > 0)
                {
                    AddNetworkInterface(TextBox_InterfaceFriendlyName2.Text, TextBox_IP2.Text, atm.ATMId, trx);
                }
                if (TextBox_InterfaceFriendlyName3.Text.Length > 0 && TextBox_IP3.Text.Length > 0)
                {
                    AddNetworkInterface(TextBox_InterfaceFriendlyName3.Text, TextBox_IP3.Text, atm.ATMId, trx);
                }
                if (TextBox_InterfaceFriendlyName4.Text.Length > 0 && TextBox_IP4.Text.Length > 0)
                {
                    AddNetworkInterface(TextBox_InterfaceFriendlyName4.Text, TextBox_IP4.Text, atm.ATMId, trx);
                }
                if (TextBox_InterfaceFriendlyName5.Text.Length > 0 && TextBox_IP5.Text.Length > 0)
                {
                    AddNetworkInterface(TextBox_InterfaceFriendlyName5.Text, TextBox_IP5.Text, atm.ATMId, trx);
                }


                ConnectionFactory.ExecuteQuery("delete atm_classification where atm_id = " + atm.ATMId, trx);
                foreach (ListItem listItem in CheckBoxList_Classification.Items)
                {
                    if (listItem.Selected)
                    {
                        AtmClassification atmClassification = new AtmClassification();
                        atmClassification.AtmId = atm.ATMId;
                        atmClassification.ClassificationId = int.Parse(listItem.Value);
                        atmClassification.Save(trx.Connection, trx);
                    }
                }

                trx.Commit();

                //                }
                //catch (Exception ex)
                //{

                //Literal_error.Text = "<li>An error occured while assigning users! " + ex.Message + "</li>";
                //                    throw new Exception("error while assigning users", ex);
                //                }
                //finally { conn.Close(); }

                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Verbose, "ATM added/edited. id=" + atm.ATMId);
                Session[SessionVars.noticeForMain.ToString()] = "ATM successfully saved!";

                if (editMode)
                {
                    if (!((bool)ViewState["atmStatus"]) && atm.IsActive)
                        Session[SessionVars.notice.ToString()] = "<script>parent.window.ShowSuccess('ATM successfully saved!');parent.window.AddATM('" + atm.Title + "'," + atm.ATMId + ");window.location='atms.aspx'</script>";
                    //RenameATM
                    else// if (atm.IsActive)
                    {
                        Session["currentPage"] = Session["currentPage1"];
                        Session[SessionVars.notice.ToString()] = "<script>parent.window.location.reload(); </script>";
                    }
                    //    Session[SessionVars.notice.ToString()] = "<script>parent.window.ShowSuccess('ATM successfully saved!');parent.window.RenameATM('" + atm.Title + "'," + atm.ATMId + ");window.location='atms.aspx'</script>";
                    //else
                    //    Session[SessionVars.notice.ToString()] = "<script>parent.window.ShowSuccess('ATM deleted successfully!');parent.window.DeleteATM(" + atm.ATMId + ");window.location='atms.aspx' </script>";

                    Utility.BuildAuditLog(atm.Title + " modified", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.ModifyATM);
                }
                else
                {
                    //Session[SessionVars.notice.ToString()] = "<script>parent.window.location.reload(); </script>";
                    Session[SessionVars.notice.ToString()] = "<script>parent.window.ShowSuccess('ATM successfully saved!');parent.window.AddATM('" + atm.Title + "'," + atm.ATMId + ");window.location='atms.aspx'</script>";
                    Utility.BuildAuditLog(atm.Title + " created", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.CreateATMs);
                    UpdateSelectedATMs();
                    //Response.Redirect("ATMs.aspx", false);
                }
                Session["dt"] = null;
                Session["dtBatch"] = null;
                Session["dtDispensedConf"] = null;


            }
            catch (Exception ex)
            {
                if (trx != null)
                    trx.Rollback();
                Literal_error.Text = "<li>An error occured while saving ATM." + ex.Message + "</li>";
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                try { SetAssignedUsers(); }
                catch { }
            }
            finally
            {
                task.EndTask();
            }
        }
        private void AddNetworkInterface(string name, string ip, int atmID, SqlTransaction trxn)
        {
            AtmInterfaceInfo atmInterfaceInfo = new AtmInterfaceInfo();
            atmInterfaceInfo.AtmId = atmID;
            atmInterfaceInfo.CreatedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
            atmInterfaceInfo.CreationTime = DateTime.Now;
            atmInterfaceInfo.InterfaceFriendlyName = name;
            atmInterfaceInfo.Ip = ip;
            atmInterfaceInfo.Save(trxn.Connection, trxn);
        }
        private static void CreateTask(SqlTransaction trx, Atm atm, SqlConnection conn, int taskTypeId)
        {
            Task configTask = new Task();
            configTask.ATMId = atm.ATMId;
            configTask.BytesTransferred = 0;
            configTask.CreatedBy = 1;
            configTask.CreationTime = DateTime.Now;
            configTask.Parsed = false;
            configTask.RetryRemaining = (byte)(atm.RetryCountConfUpload);
            configTask.Status = UploadStates.scheduled.ToString();
            configTask.TaskTypeId = taskTypeId;
            configTask.UploadTime = new DateTime(1900, 1, 1);
            configTask.Save(conn, trx);
        }

        public static int GetOrganization(int region_id)
        {
            Region region = Region.LoadRegionByPk(region_id);
            if (region.IsOrganization)
            {
                return region.RegionId;
            }
            else
                return GetOrganization(region.ParentRegionId.Value);

        }


        void UpdateSelectedATMs()
        {
            Queue<int> regionQue = new Queue<int>(10);
            regionQue.Enqueue(int.Parse((string)Session[SessionVars.SelectedRegion.ToString()]));
            string selectedATMs = "-1";
            while (regionQue.Count > 0)
            {
                Atm.AtmReader atmReader = Atm.ExecuteReader("region_id=" + regionQue.Peek() +
                    " and atm_id in (select atm_id from user_ATMs where user_id = " + this.appUser.UserId + ")");
                while (atmReader.Read())
                    selectedATMs += "," + atmReader.CurrentAtm.ATMId.ToString();
                atmReader.Close();
                Region.RegionReader regionReader = Region.ExecuteReader("parent_region_id=" + regionQue.Peek(), Region.Columns.region_id);
                while (regionReader.Read())
                    regionQue.Enqueue(regionReader.CurrentRegion.RegionId);
                regionReader.Close();
                regionQue.Dequeue();
            }
            Session[SessionVars.SelectedATMs.ToString()] = selectedATMs;
        }

        private void SetNoteSetTypeTitles()
        {
            NoteSetType noteSetType = NoteSetType.LoadNoteSetTypeByPk(int.Parse(DropDownList_NoteSetTypeID.SelectedItem.Value));
            if (noteSetType != null)
            {
                Label_NoteSetType1Title.Text = noteSetType.DenominationType1Title;
                Label_NoteSetType2Title.Text = noteSetType.DenominationType2Title;
                Label_NoteSetType3Title.Text = noteSetType.DenominationType3Title;
                Label_NoteSetType4Title.Text = noteSetType.DenominationType4Title;

                TextBox_Cassette1_Denominations.Text = noteSetType.DenominationType1.ToString();
                TextBox_Cassette2_Denominations.Text = noteSetType.DenominationType2.ToString();
                TextBox_Cassette3_Denominations.Text = noteSetType.DenominationType3.ToString();
                TextBox_Cassette4_Denominations.Text = noteSetType.DenominationType4.ToString();

            }
        }
        protected void DropDownList_NoteSetType_Change(object sender, EventArgs e)
        {
            if (DropDownList_NoteSetTypeID.SelectedIndex > 0)
            {
                SetNoteSetTypeTitles();
                //NoteSetType noteSetType = NoteSetType.LoadNoteSetTypeByPk(int.Parse(DropDownList_NoteSetTypeID.SelectedItem.Value));
                //if (noteSetType != null)
                //{
                //    Label_NoteSetType1Title.Text = noteSetType.DenominationType1Title;
                //    Label_NoteSetType2Title.Text = noteSetType.DenominationType2Title;
                //    Label_NoteSetType3Title.Text = noteSetType.DenominationType3Title;
                //    Label_NoteSetType4Title.Text = noteSetType.DenominationType4Title;

                //if (Request.QueryString["atm_id"] == null)
                //{
                TextBox_Cassette1_Capacity.Text = "2000";

                if (TextBox_Purge1_Threshold.Text.Length == 0)
                    TextBox_Purge1_Threshold.Text = "10";
                CheckBox_Purge1_Threshold.Checked = true;

                TextBox_Cassette2_Capacity.Text = "2000";
                if (TextBox_Purge2_Threshold.Text.Length == 0)
                    TextBox_Purge2_Threshold.Text = "10";
                CheckBox_Purge2_Threshold.Checked = true;

                TextBox_Cassette3_Capacity.Text = "2000";
                if (TextBox_Purge3_Threshold.Text.Length == 0)
                    TextBox_Purge3_Threshold.Text = "10";
                CheckBox_Purge3_Threshold.Checked = true;

                TextBox_Cassette4_Capacity.Text = "2000";
                if (TextBox_Purge4_Threshold.Text.Length == 0)
                    TextBox_Purge4_Threshold.Text = "10";

                TextBox_Purge4_Threshold.Text = "10";
                CheckBox_Purge4_Threshold.Checked = true;

                //}

                //}
            }
            else
            {
                TextBox_Cassette1_Denominations.Text = "";
                TextBox_Cassette2_Denominations.Text = "";
                TextBox_Cassette3_Denominations.Text = "";
                TextBox_Cassette4_Denominations.Text = "";

                TextBox_Cassette1_Capacity.Text = "";
                TextBox_Cassette2_Capacity.Text = "";
                TextBox_Cassette3_Capacity.Text = "";
                TextBox_Cassette4_Capacity.Text = "";
            }
        }

        private void GetGeneralInventory(int atmId)
        {
            StringBuilder builder = new StringBuilder();
            AtmGeneralInventory.AtmGeneralInventoryReader reader = AtmGeneralInventory.ExecuteReader("atm_id = " + atmId);
            try
            {
                while (reader.Read())
                    builder.Append(string.Format("<tr><td>{0}</td><td>{1}</td></tr>", reader.CurrentAtmGeneralInventory.Name, reader.CurrentAtmGeneralInventory.FieldValue));
                Literal_GeneralInfo.Text = builder.ToString();
            }
            finally
            {
                reader.Close();
            }
        }
        private void GetHardwareInventory(int atmId)
        {
            StringBuilder builder = new StringBuilder();
            AtmHardwareInventory.AtmHardwareInventoryReader reader = AtmHardwareInventory.ExecuteReader("atm_id = " + atmId);
            try
            {
                while (reader.Read())
                    builder.Append(string.Format("<tr><td>{0}</td></tr>", reader.CurrentAtmHardwareInventory.InstalledHardware));
                Literal_HardwareInfo.Text = builder.ToString();
            }
            finally
            {
                reader.Close();
            }
        }
        private void GetSoftwareInventory(int atmId)
        {
            StringBuilder builder = new StringBuilder();
            AtmSoftwareInventory.AtmSoftwareInventoryReader reader = AtmSoftwareInventory.ExecuteReader("atm_id = " + atmId);
            try
            {
                while (reader.Read())
                    builder.Append(string.Format("<tr><td>{0}</td></tr>", reader.CurrentAtmSoftwareInventory.InstalledProgram));
                Literal_InstalledSoftware.Text = builder.ToString();
            }
            finally
            {
                reader.Close();
            }
        }
        //protected void RadioButtonList_Search_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (RadioButtonList_Search.SelectedIndex == 0)
        //    {
        //        trAddress.Visible = true;
        //        //TextBox_Location.Text = "";
        //        //trLong.Visible = false;
        //        //trLat.Visible = false;
        //    }
        //    else
        //    {
        //        trLong.Visible = true;
        //        trLat.Visible = true;
        //        //trAddress.Visible = false;
        //        //TextBox_Latitude.Text = "";
        //        //TextBox_Longitude.Text = "";
        //    }
        //}

    }
}