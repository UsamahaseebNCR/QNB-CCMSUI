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
using Avanza.CCMS.DAL;
using System.Reflection;
using System.Diagnostics;

using System.Collections.Generic;
using System.Data.SqlClient;

namespace CCMSUI.admin
{
    public partial class AddEditNoteSetType : BasePage
    {
        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        }

        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            string regionID = (string)Session[SessionVars.SelectedRegions.ToString()];
            if (regionID != null /*&& !regionID.Contains(",")*/
                && !regionID.StartsWith("1,"))
            {

                Region region = Region.LoadRegionByPk(regionID.Contains(",") ? int.Parse(regionID.Substring(0, regionID.IndexOf(","))) : int.Parse(regionID));
                if (region.IsOrganization)
                {
                    Label_SelectedOrganization.Text = region.RegionName;
                    Session[SessionVars.SelectedRegions.ToString()] = region.RegionId.ToString();
                }

            }
            else
                Label_SelectedOrganization.Text = "";

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.SetFocus(TextBox_NoteSetTypeName);
            if (Page.IsPostBack)
                return;


            if (Request.QueryString["note_set_type_id"] == null)
            {
                if (!CheckRight((int)Permissions.CreateNoteSetType))
                    Response.Redirect("AccessDenied.aspx");

                refresh_ServerClick(this, new EventArgs());

                //if (Session[SessionVars.SelectedRegions.ToString()] == null)
                //{
                //    Label_SelectedOrganization.Text = "";
                //}
                //else if ((string)Session[SessionVars.SelectedRegions.ToString()] == "1"
                //    || Session[SessionVars.SelectedRegions.ToString()].ToString().Contains(","))
                //{
                //    Label_SelectedOrganization.Text = "";
                //}
                //else
                //{
                //   Region reg = Region.LoadRegionByPk(int.Parse(Session[SessionVars.SelectedRegions.ToString()].ToString()));
                //   if (reg.IsOrganization)
                //       Label_SelectedOrganization.Text = reg.RegionName;
                //   else
                //       Label_SelectedOrganization.Text = "";
                //}


            }
            else
            {
                if (!CheckRight((int)Permissions.ModifyNoteSetType))
                    Response.Redirect("AccessDenied.aspx");

                refresh_ServerClick(this, new EventArgs());

                Avanza.CCMS.DAL.NoteSetType noteSetType = Avanza.CCMS.DAL.NoteSetType.LoadNoteSetTypeByPk(int.Parse(Request.QueryString["note_set_type_id"]));
                CcmsOrgNoteSet orgNoteSet = CcmsOrgNoteSet.LoadCcmsOrgNoteSet("name='" + noteSetType.NoteSetTypeName + "'");
                if (orgNoteSet != null)
                    CheckBox_DefaultOrganization.Checked = orgNoteSet.IsDefault.Value;

                Label_SelectedOrganization.Text = Region.LoadRegionByPk(noteSetType.RegionId).RegionName;

                Session[SessionVars.SelectedRegions.ToString()] = noteSetType.RegionId.ToString();
                TextBox_NoteSetTypeName.Text = noteSetType.NoteSetTypeName;
                TextBox_NoteTitleType1.Text = noteSetType.DenominationType1Title;
                TextBox_NoteTitleType2.Text = noteSetType.DenominationType2Title;
                TextBox_NoteTitleType3.Text = noteSetType.DenominationType3Title;
                TextBox_NoteTitleType4.Text = noteSetType.DenominationType4Title;
                TextBox_NoteTitleType5.Text = noteSetType.DenominationType5Title;
                TextBox_NoteTitleType6.Text = noteSetType.DenominationType6Title;
                TextBox_NoteTitleType7.Text = noteSetType.DenominationType7Title;

                TextBox_DenominationType1.Text = noteSetType.DenominationType1.ToString();
                TextBox_DenominationType2.Text = noteSetType.DenominationType2.ToString();
                TextBox_DenominationType3.Text = noteSetType.DenominationType3.ToString();
                TextBox_DenominationType4.Text = noteSetType.DenominationType4.ToString();
                TextBox_DenominationType5.Text = noteSetType.DenominationType5.ToString();
                TextBox_DenominationType6.Text = noteSetType.DenominationType6.ToString();
                TextBox_DenominationType7.Text = noteSetType.DenominationType7.ToString();

                if (noteSetType.IsType1MultiCurrency.HasValue)
                {
                    if (noteSetType.IsType1MultiCurrency.Value)
                        CheckBox_isType1MultiCurrency.Checked = true;
                }
                if (noteSetType.IsType2MultiCurrency.HasValue)
                {
                    if (noteSetType.IsType2MultiCurrency.Value)
                        CheckBox_isType2MultiCurrency.Checked = true;
                }
                if (noteSetType.IsType3MultiCurrency.HasValue)
                {
                    if (noteSetType.IsType3MultiCurrency.Value)
                        CheckBox_isType3MultiCurrency.Checked = true;
                }
                if (noteSetType.IsType4MultiCurrency.HasValue)
                {
                    if (noteSetType.IsType4MultiCurrency.Value)
                        CheckBox_isType4MultiCurrency.Checked = true;
                }
                if (noteSetType.IsType5MultiCurrency.HasValue)
                {
                    if (noteSetType.IsType5MultiCurrency.Value)
                        CheckBox_isType5MultiCurrency.Checked = true;
                }
                if (noteSetType.IsType6MultiCurrency.HasValue)
                {
                    if (noteSetType.IsType6MultiCurrency.Value)
                        CheckBox_isType6MultiCurrency.Checked = true;
                }
                if (noteSetType.IsType7MultiCurrency.HasValue)
                {
                    if (noteSetType.IsType7MultiCurrency.Value)
                        CheckBox_isType7MultiCurrency.Checked = true;
                }
                if (noteSetType.IsType1Recycler==true)
                {
                    if (noteSetType.IsType1Recycler)
                        CheckBox_isType1Recycler.Checked = true;
                }
                if (noteSetType.IsType2Recycler == true)
                {
                    if (noteSetType.IsType2Recycler)
                        CheckBox_isType2Recycler.Checked = true;
                }
                if (noteSetType.IsType3Recycler == true)
                {
                    if (noteSetType.IsType3Recycler)
                        CheckBox_isType3Recycler.Checked = true;
                }
                if (noteSetType.IsType4Recycler == true)
                {
                    if (noteSetType.IsType4Recycler)
                        CheckBox_isType4Recycler.Checked = true;
                }
                if (noteSetType.IsType5Recycler == true)
                {
                    if (noteSetType.IsType5Recycler)
                        CheckBox_isType5Recycler.Checked = true;
                }
                if (noteSetType.IsType6Recycler == true)
                {
                    if (noteSetType.IsType6Recycler)
                        CheckBox_isType6Recycler.Checked = true;
                }
                if (noteSetType.IsType7Recycler == true)
                {
                    if (noteSetType.IsType7Recycler)
                        CheckBox_isType7Recycler.Checked = true;
                }
                
            }
        }

        protected void Button_SaveUpdateNoteSetType_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("AddEditNoteSetType");
            Avanza.CCMS.DAL.NoteSetType noteSetType;
            CcmsOrgNoteSet orgNoteSet = null;
            bool editMode = false;
            bool createConfigTask = false;
            List<string> denominations = new List<string>();
            SqlConnection conn = null;
            SqlTransaction trxn = null;
            bool trxnCommitted = false;
            bool result = false;
            try
            {
               result = ValidateInputs();
               if (result == false)
               {
                   return;
               }
               else
               {
                   conn = ConnectionFactory.GetNewConnection();
                   conn.Open();
                   trxn = conn.BeginTransaction();
                   string regionID = (string)Session[SessionVars.SelectedRegions.ToString()];
                   if (regionID != null /*&& !regionID.Contains(",")*/
                       && !regionID.StartsWith("1,"))
                   {

                       Region region = Region.LoadRegionByPk(regionID.Contains(",") ? int.Parse(regionID.Substring(0, regionID.IndexOf(","))) : int.Parse(regionID));
                       if (!region.IsOrganization)
                           throw new Exception("Please select organization from the tree");
                       else
                           Session[SessionVars.SelectedRegions.ToString()] = region.RegionId.ToString();
                   }
                   else
                       throw new Exception("Please select organization from the tree");

                   if (TextBox_NoteSetTypeName.Text.Trim().Length == 0)
                       throw new Exception("Please enter note set type name");

                   if (Request.QueryString["note_set_type_id"] != null) // if new
                       editMode = true;

                   if (TextBox_DenominationType1.Text.Length == 0)
                       TextBox_DenominationType1.Text = "0";
                   if (TextBox_DenominationType2.Text.Length == 0)
                       TextBox_DenominationType2.Text = "0";
                   if (TextBox_DenominationType3.Text.Length == 0)
                       TextBox_DenominationType3.Text = "0";
                   if (TextBox_DenominationType4.Text.Length == 0)
                       TextBox_DenominationType4.Text = "0";
                   if (TextBox_DenominationType5.Text.Length == 0)
                       TextBox_DenominationType5.Text = "0";
                   if (TextBox_DenominationType6.Text.Length == 0)
                       TextBox_DenominationType6.Text = "0";
                   if (TextBox_DenominationType7.Text.Length == 0)
                       TextBox_DenominationType7.Text = "0";





                   if (editMode)
                   {
                       noteSetType = Avanza.CCMS.DAL.NoteSetType.LoadNoteSetTypeByPk(int.Parse(Request.QueryString["note_set_type_id"]));
                       orgNoteSet = CcmsOrgNoteSet.LoadCcmsOrgNoteSet("name='" + noteSetType.NoteSetTypeName + "'");
                       if (orgNoteSet == null)
                           orgNoteSet = new CcmsOrgNoteSet();

                       if (noteSetType.DenominationType1 != int.Parse(TextBox_DenominationType1.Text)
                          || noteSetType.DenominationType2 != int.Parse(TextBox_DenominationType2.Text)
                          || noteSetType.DenominationType3 != int.Parse(TextBox_DenominationType3.Text)
                          || noteSetType.DenominationType4 != int.Parse(TextBox_DenominationType4.Text)
                          || noteSetType.DenominationType5 != int.Parse(TextBox_DenominationType5.Text)
                          || noteSetType.DenominationType6 != int.Parse(TextBox_DenominationType6.Text)
                          || noteSetType.DenominationType7 != int.Parse(TextBox_DenominationType7.Text))
                           createConfigTask = true;


                   }
                   else
                   {
                       noteSetType = new Avanza.CCMS.DAL.NoteSetType();
                       orgNoteSet = new CcmsOrgNoteSet();
                   }

                   noteSetType.RegionId = int.Parse(Session[SessionVars.SelectedRegions.ToString()].ToString());
                   object res = ConnectionFactory.ExecuteScalar("select top 1 note_set_type_name from note_set_type where note_set_type_name='" + TextBox_NoteSetTypeName.Text + "' and region_id = " + noteSetType.RegionId
                        + (editMode ? " and note_set_type_id != " + noteSetType.NoteSetTypeId : ""));
                   if (res != null)
                   {
                       Literal_error.Text = "<li>This name is already defined for note set type name '" + res + "'. Please, provide different note set type name.</li>";
                       return;
                   }


                   if (!editMode)
                   {
                       noteSetType.CreatedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                       noteSetType.CreationTime = DateTime.Now;
                   }

                   noteSetType.NoteSetTypeName = TextBox_NoteSetTypeName.Text;
                   orgNoteSet.Name = TextBox_NoteSetTypeName.Text;

                   if (TextBox_DenominationType1.Text.Length > 0 && TextBox_NoteTitleType1.Text.Length > 0)
                   {
                       noteSetType.DenominationType1 = int.Parse(TextBox_DenominationType1.Text);
                       noteSetType.DenominationType1Title = TextBox_NoteTitleType1.Text;
                       denominations.Add(noteSetType.DenominationType1Title);
                   }
                   else
                   {
                       noteSetType.DenominationType1 = 0;
                       noteSetType.DenominationType1Title = "";
                   }

                   if (TextBox_DenominationType2.Text.Length > 0 && TextBox_NoteTitleType2.Text.Length > 0)
                   {
                       noteSetType.DenominationType2 = int.Parse(TextBox_DenominationType2.Text);
                       noteSetType.DenominationType2Title = TextBox_NoteTitleType2.Text;
                       //if (denominations.Contains(noteSetType.DenominationType2Title))
                       //    throw new Exception("Cannot define same denomination for multiple types");
                       //else
                       denominations.Add(noteSetType.DenominationType2Title);
                   }
                   else
                   {
                       noteSetType.DenominationType2 = 0;
                       noteSetType.DenominationType2Title = "";
                   }

                   if (TextBox_DenominationType3.Text.Length > 0 && TextBox_NoteTitleType3.Text.Length > 0)
                   {
                       noteSetType.DenominationType3 = int.Parse(TextBox_DenominationType3.Text);
                       noteSetType.DenominationType3Title = TextBox_NoteTitleType3.Text;
                       //    if (denominations.Contains(noteSetType.DenominationType3Title))
                       //        throw new Exception("Cannot define same denomination for multiple types");
                       //    else
                       denominations.Add(noteSetType.DenominationType3Title);
                       //
                   }
                   else
                   {
                       noteSetType.DenominationType3 = 0;
                       noteSetType.DenominationType3Title = "";
                   }

                   if (TextBox_DenominationType4.Text.Length > 0 && TextBox_NoteTitleType4.Text.Length > 0)
                   {
                       noteSetType.DenominationType4 = int.Parse(TextBox_DenominationType4.Text);
                       noteSetType.DenominationType4Title = TextBox_NoteTitleType4.Text;
                       //    if (denominations.Contains(noteSetType.DenominationType4Title))
                       //        throw new Exception("Cannot define same denomination for multiple types");
                       //    else
                       denominations.Add(noteSetType.DenominationType4Title);
                   }
                   else
                   {
                       noteSetType.DenominationType4 = 0;
                       noteSetType.DenominationType4Title = "";
                   }

                   if (TextBox_DenominationType5.Text.Length > 0 && TextBox_NoteTitleType5.Text.Length > 0)
                   {
                       noteSetType.DenominationType5 = int.Parse(TextBox_DenominationType5.Text);
                       noteSetType.DenominationType5Title = TextBox_NoteTitleType5.Text;
                       //    if (denominations.Contains(noteSetType.DenominationType5Title))
                       //        throw new Exception("Cannot define same denomination for multiple types");
                       //    else
                       denominations.Add(noteSetType.DenominationType5Title);
                       //
                   }
                   else
                   {
                       noteSetType.DenominationType5 = 0;
                       noteSetType.DenominationType5Title = "";
                   }

                   if (TextBox_DenominationType6.Text.Length > 0 && TextBox_NoteTitleType6.Text.Length > 0)
                   {
                       noteSetType.DenominationType6 = int.Parse(TextBox_DenominationType6.Text);
                       noteSetType.DenominationType6Title = TextBox_NoteTitleType6.Text;
                       //if (denominations.Contains(noteSetType.DenominationType6Title))
                       //    throw new Exception("Cannot define same denomination for multiple types");
                       //else
                       denominations.Add(noteSetType.DenominationType6Title);
                   }
                   else
                   {
                       noteSetType.DenominationType6 = 0;
                       noteSetType.DenominationType6Title = "";
                   }

                   if (TextBox_DenominationType7.Text.Length > 0 && TextBox_NoteTitleType7.Text.Length > 0)
                   {
                       noteSetType.DenominationType7 = int.Parse(TextBox_DenominationType7.Text);
                       noteSetType.DenominationType7Title = TextBox_NoteTitleType7.Text;
                       //if (denominations.Contains(noteSetType.DenominationType7Title))
                       //    throw new Exception("Cannot define same denomination for multiple types");
                       //else
                       denominations.Add(noteSetType.DenominationType7Title);
                   }
                   else
                   {
                       noteSetType.DenominationType7 = 0;
                       noteSetType.DenominationType7Title = "";
                   }

                   //List<int> list = new List<int>();
                   //List<int> listMultiCurrency = new List<int>();

                   //if (CheckBox_isType1MultiCurrency.Checked)
                   //   listMultiCurrency.Add(noteSetType.DenominationType1.Value);
                   //else
                   //   list.Add(noteSetType.DenominationType1.Value);

                   ////if (noteSetType.DenominationType1 != 0)
                   ////{
                   ////    if (!CheckBox_isType1MultiCurrency.Checked)
                   ////        list.Add(noteSetType.DenominationType1.Value);
                   ////}
                   //if (CheckBox_isType2MultiCurrency.Checked)
                   //{
                   //    if (listMultiCurrency.Contains(noteSetType.DenominationType2.Value) && (noteSetType.DenominationType2 != 0))
                   //        throw new Exception("Cannot define same denomination for multiple types");
                   //    listMultiCurrency.Add(noteSetType.DenominationType1.Value);
                   //}
                   //else
                   //{
                   //    if (list.Contains(noteSetType.DenominationType2.Value) && (noteSetType.DenominationType2 != 0))
                   //        throw new Exception("Cannot define same denomination for multiple types");
                   //    list.Add(noteSetType.DenominationType1.Value);
                   //}

                   //if (CheckBox_isType3MultiCurrency.Checked)
                   //{
                   //    if (listMultiCurrency.Contains(noteSetType.DenominationType3.Value) && (noteSetType.DenominationType3 != 0))
                   //        throw new Exception("Cannot define same denomination for multiple types");
                   //    listMultiCurrency.Add(noteSetType.DenominationType1.Value);
                   //}
                   //else
                   //{
                   //    if (list.Contains(noteSetType.DenominationType3.Value) && (noteSetType.DenominationType3 != 0))
                   //        throw new Exception("Cannot define same denomination for multiple types");
                   //    list.Add(noteSetType.DenominationType1.Value);
                   //}

                   //if (CheckBox_isType4MultiCurrency.Checked)
                   //{
                   //    if (listMultiCurrency.Contains(noteSetType.DenominationType4.Value) && (noteSetType.DenominationType4 != 0))
                   //        throw new Exception("Cannot define same denomination for multiple types");
                   //    listMultiCurrency.Add(noteSetType.DenominationType1.Value);
                   //}
                   //else
                   //{
                   //    if (list.Contains(noteSetType.DenominationType4.Value) && (noteSetType.DenominationType4 != 0))
                   //        throw new Exception("Cannot define same denomination for multiple types");
                   //    list.Add(noteSetType.DenominationType1.Value);

                   //}

                   //if (CheckBox_isType5MultiCurrency.Checked)
                   //{
                   //    if (listMultiCurrency.Contains(noteSetType.DenominationType5.Value) && (noteSetType.DenominationType5 != 0))
                   //        throw new Exception("Cannot define same denomination for multiple types");
                   //    listMultiCurrency.Add(noteSetType.DenominationType1.Value);
                   //}
                   //else
                   //{
                   //    if (list.Contains(noteSetType.DenominationType5.Value) && (noteSetType.DenominationType5 != 0))
                   //        throw new Exception("Cannot define same denomination for multiple types");
                   //    list.Add(noteSetType.DenominationType1.Value);
                   //}


                   //if (CheckBox_isType6MultiCurrency.Checked)
                   //{
                   //    if (listMultiCurrency.Contains(noteSetType.DenominationType6.Value) && (noteSetType.DenominationType6 != 0))
                   //        throw new Exception("Cannot define same denomination for multiple types");
                   //    listMultiCurrency.Add(noteSetType.DenominationType1.Value);
                   //}
                   //else
                   //{
                   //    if (list.Contains(noteSetType.DenominationType6.Value) && (noteSetType.DenominationType6 != 0))
                   //        throw new Exception("Cannot define same denomination for multiple types");
                   //    list.Add(noteSetType.DenominationType1.Value);
                   //}

                   //if (CheckBox_isType7MultiCurrency.Checked)
                   //{
                   //    if (listMultiCurrency.Contains(noteSetType.DenominationType7.Value) && (noteSetType.DenominationType7 != 0))
                   //        throw new Exception("Cannot define same denomination for multiple types");
                   //    listMultiCurrency.Add(noteSetType.DenominationType1.Value);
                   //}
                   //else
                   //{
                   //    if (list.Contains(noteSetType.DenominationType7.Value) && (noteSetType.DenominationType7 != 0))
                   //        throw new Exception("Cannot define same denomination for multiple types");
                   //    list.Add(noteSetType.DenominationType1.Value);
                   //}



                   //if (list.Contains(noteSetType.DenominationType2.Value) && (noteSetType.DenominationType2 != 0))
                   //    throw new Exception("Cannot define same denomination for multiple types");
                   //else
                   //{
                   //    if (!CheckBox_isType2MultiCurrency.Checked)
                   //        list.Add(noteSetType.DenominationType2.Value);
                   //}

                   //if (list.Contains(noteSetType.DenominationType3.Value) && (noteSetType.DenominationType3 != 0))
                   //    throw new Exception("Cannot define same denomination for multiple types");
                   //else
                   //{
                   //    if (!CheckBox_isType3MultiCurrency.Checked)
                   //        list.Add(noteSetType.DenominationType3.Value);
                   //}

                   //if (list.Contains(noteSetType.DenominationType4.Value) && (noteSetType.DenominationType4 != 0))
                   //    throw new Exception("Cannot define same denomination for multiple types");
                   //else
                   //{
                   //    if (!CheckBox_isType4MultiCurrency.Checked)
                   //        list.Add(noteSetType.DenominationType4.Value);
                   //}



                   //if (list.Contains(noteSetType.DenominationType5.Value) && (noteSetType.DenominationType5 != 0))
                   //    throw new Exception("Cannot define same denomination for multiple types");
                   //else
                   //{
                   //    if (!CheckBox_isType5MultiCurrency.Checked)
                   //    list.Add(noteSetType.DenominationType5.Value);
                   //}


                   //if (list.Contains(noteSetType.DenominationType6.Value) && (noteSetType.DenominationType6 != 0))
                   //    throw new Exception("Cannot define same denomination for multiple types");
                   //else
                   //{
                   //    if (!CheckBox_isType6MultiCurrency.Checked)
                   //    list.Add(noteSetType.DenominationType6.Value);

                   //}


                   //if (list.Contains(noteSetType.DenominationType7.Value) && (noteSetType.DenominationType7 != 0))
                   //    throw new Exception("Cannot define same denomination for multiple types");

                   if (editMode && createConfigTask)
                   {
                       List<Task> taskList = new List<Task>();
                       Atm.AtmReader reader = Atm.ExecuteReader("note_set_type_id = " + noteSetType.NoteSetTypeId);
                       while (reader.Read())
                       {
                           if ((int)ConnectionFactory.ExecuteScalar("select count(task_id) from task where task_type_id=" + (int)EnumTaskType.Configuration + " and status in ('scheduled','uploadingDisconnected') and atm_id =" + reader.CurrentAtm.ATMId) == 0)
                           {

                               Task configTask = new Task();
                               configTask.ATMId = reader.CurrentAtm.ATMId;
                               configTask.BytesTransferred = 0;
                               configTask.CreatedBy = 1;
                               configTask.CreationTime = DateTime.Now;
                               configTask.Parsed = false;
                               configTask.RetryRemaining = (byte)(reader.CurrentAtm.RetryCountConfUpload);
                               configTask.Status = UploadStates.scheduled.ToString();
                               configTask.TaskTypeId = (int)EnumTaskType.Configuration;
                               configTask.UploadTime = new DateTime(1900, 1, 1);
                               taskList.Add(configTask);

                           }

                       }

                       reader.Close();
                       foreach (Task t in taskList)
                           t.Save(conn, trxn);
                   }

                   if (CheckBox_isType1MultiCurrency.Checked)
                       noteSetType.IsType1MultiCurrency = true;
                   else
                       noteSetType.IsType1MultiCurrency = false;

                   if (CheckBox_isType2MultiCurrency.Checked)
                       noteSetType.IsType2MultiCurrency = true;
                   else
                       noteSetType.IsType2MultiCurrency = false;

                   if (CheckBox_isType3MultiCurrency.Checked)
                       noteSetType.IsType3MultiCurrency = true;
                   else
                       noteSetType.IsType3MultiCurrency = false;

                   if (CheckBox_isType4MultiCurrency.Checked)
                       noteSetType.IsType4MultiCurrency = true;
                   else
                       noteSetType.IsType4MultiCurrency = false;

                   if (CheckBox_isType5MultiCurrency.Checked)
                       noteSetType.IsType5MultiCurrency = true;
                   else
                       noteSetType.IsType5MultiCurrency = false;

                   if (CheckBox_isType6MultiCurrency.Checked)
                       noteSetType.IsType6MultiCurrency = true;
                   else
                       noteSetType.IsType6MultiCurrency = false;

                   if (CheckBox_isType7MultiCurrency.Checked)
                       noteSetType.IsType7MultiCurrency = true;
                   else
                       noteSetType.IsType7MultiCurrency = false;

                   if (CheckBox_isType1Recycler.Checked)
                       noteSetType.IsType1Recycler = true;
                   else
                       noteSetType.IsType1Recycler = false;

                   if (CheckBox_isType2Recycler.Checked)
                       noteSetType.IsType2Recycler = true;
                   else
                       noteSetType.IsType2Recycler = false;

                   if (CheckBox_isType3Recycler.Checked)
                       noteSetType.IsType3Recycler = true;
                   else
                       noteSetType.IsType3Recycler = false;

                   if (CheckBox_isType4Recycler.Checked)
                       noteSetType.IsType4Recycler = true;
                   else
                       noteSetType.IsType4Recycler = false;

                   if (CheckBox_isType5Recycler.Checked)
                       noteSetType.IsType5Recycler = true;
                   else
                       noteSetType.IsType5Recycler = false;

                   if (CheckBox_isType6Recycler.Checked)
                       noteSetType.IsType6Recycler = true;
                   else
                       noteSetType.IsType6Recycler = false;

                   if (CheckBox_isType7Recycler.Checked)
                       noteSetType.IsType7Recycler = true;
                   else
                       noteSetType.IsType7Recycler = false;

                   noteSetType.Save(conn, trxn);
                   orgNoteSet.IsDefault = false;
                   orgNoteSet.OrganizationId = Avanza.CCMS.DAL.CcmsOrganization.LoadCcmsOrganization("region_id=" + noteSetType.RegionId).Id;
                   orgNoteSet.IsDefault = CheckBox_DefaultOrganization.Checked;
                   if (orgNoteSet.IsDefault.Value)
                       ConnectionFactory.ExecuteQuery("update ccms_org_note_set set is_default=0 where organization_id = " + orgNoteSet.OrganizationId, trxn);
                   orgNoteSet.Save(conn, trxn);
                   ConnectionFactory.ExecuteQuery("delete ccms_org_note_set_item where org_note_set_id=" + orgNoteSet.Id, trxn);

                   foreach (string denomination in denominations)
                   {
                       if (denomination.Length < 3)
                       {
                           continue;
                       }

                       CcmsDenomination ccmsDenomination = CcmsDenomination.LoadCcmsDenomination("name='" + denomination + "'");
                       if (ccmsDenomination == null)
                       {
                           ccmsDenomination = new CcmsDenomination();
                           ccmsDenomination.Name = denomination;
                           ccmsDenomination.CurrencyCode = denomination.Substring(0, 3);
                           ccmsDenomination.OrganizationId = orgNoteSet.OrganizationId;
                           ccmsDenomination.Value = int.Parse(denomination.Substring(3));
                           ccmsDenomination.Save(conn, trxn);
                       }
                       CcmsOrgNoteSetItem ccmsOrgNoteSetItem = new CcmsOrgNoteSetItem();
                       ccmsOrgNoteSetItem.DenominationName = ccmsDenomination.Name;
                       ccmsOrgNoteSetItem.DenominationId = ccmsDenomination.Id;
                       ccmsOrgNoteSetItem.OrgNoteSetId = orgNoteSet.Id;
                       ccmsOrgNoteSetItem.Save(conn, trxn);

                   }

                   if (!editMode)
                       Utility.BuildAuditLog(noteSetType.NoteSetTypeName + " created", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.CreateNoteSetType);
                   else
                       Utility.BuildAuditLog(noteSetType.NoteSetTypeName + " modified", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.ModifyNoteSetType);
                   trxn.Commit();
                   trxnCommitted = true;
                   Response.Redirect("NoteSetType.aspx", false);
               }
            }
            catch (Exception ex)
            {
                if (!trxnCommitted)
                    trxn.Rollback();

                Literal_error.Text = "<li>An error occured while saving note set type." + ex.Message + "</li>";
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);

            }
            finally
            {
                if (conn != null)
                    conn.Close();
                task.EndTask();
            }
        }

        public bool ValidateInputs()
        {
            bool result = true;
            bool isNoteSetTypeNameContainSpecialChars = CheckSpecialChars(TextBox_NoteSetTypeName.Text);
            if (isNoteSetTypeNameContainSpecialChars == true)
            {
                Literal_error.Text = "<img src='../images/icon_err.gif'>&nbsp;Note Set Type Name has invalid character</img> </li>";
                return result = false;
            }
            bool isDenominationType1ContainSpecialChars = CheckSpecialChars(TextBox_DenominationType1.Text);
            if (isDenominationType1ContainSpecialChars == true)
            {
                Literal_error.Text = "<img src='../images/icon_err.gif'>&nbsp;Denomination Type 1 has invalid character</img> </li>";
                return result = false;
            }
            bool isDenominationType2ContainSpecialChars = CheckSpecialChars(TextBox_DenominationType2.Text);
            if (isDenominationType2ContainSpecialChars == true)
            {
                Literal_error.Text = "<img src='../images/icon_err.gif'>&nbsp;Denomination Type 2 has invalid character</img> </li>";
                return result = false;
            }
            bool isDenominationType3ContainSpecialChars = CheckSpecialChars(TextBox_DenominationType3.Text);
            if (isDenominationType3ContainSpecialChars == true)
            {
                Literal_error.Text = "<img src='../images/icon_err.gif'>&nbsp;Denomination Type 3 has invalid character</img> </li>";
                return result = false;
            }
            bool isDenominationType4ContainSpecialChars = CheckSpecialChars(TextBox_DenominationType4.Text);
            if (isDenominationType4ContainSpecialChars == true)
            {
                Literal_error.Text = "<img src='../images/icon_err.gif'>&nbsp;Denomination Type 4 has invalid character</img> </li>";
                return result = false;
            }
            bool isDenominationType5ContainSpecialChars = CheckSpecialChars(TextBox_DenominationType5.Text);
            if (isDenominationType5ContainSpecialChars == true)
            {
                Literal_error.Text = "<img src='../images/icon_err.gif'>&nbsp;Denomination Type 5 has invalid character</img> </li>";
                return result = false;
            }
            bool isDenominationType6ContainSpecialChars = CheckSpecialChars(TextBox_DenominationType6.Text);
            if (isDenominationType6ContainSpecialChars == true)
            {
                Literal_error.Text = "<img src='../images/icon_err.gif'>&nbsp;Denomination Type 6 has invalid character</img> </li>";
                return result = false;
            }
            bool isDenominationType7ContainSpecialChars = CheckSpecialChars(TextBox_DenominationType7.Text);
            if (isDenominationType7ContainSpecialChars == true)
            {
                Literal_error.Text = "<img src='../images/icon_err.gif'>&nbsp;Denomination Type 7 has invalid character</img> </li>";
                return result = false;
            }
            bool isNoteTitleType1ContainSpecialChars = CheckSpecialChars(TextBox_NoteTitleType1.Text);
            if (isNoteTitleType1ContainSpecialChars == true)
            {
                Literal_error.Text = "<img src='../images/icon_err.gif'>&nbsp;Note Title Type 1 has invalid character</img> </li>";
                return result = false;
            }
            bool isNoteTitleType2ContainSpecialChars = CheckSpecialChars(TextBox_NoteTitleType2.Text);
            if (isNoteTitleType2ContainSpecialChars == true)
            {
                Literal_error.Text = "<img src='../images/icon_err.gif'>&nbsp;Note Title Type 2 has invalid character</img> </li>";
                return result = false;
            }
            bool isNoteTitleType3ContainSpecialChars = CheckSpecialChars(TextBox_NoteTitleType3.Text);
            if (isNoteTitleType3ContainSpecialChars == true)
            {
                Literal_error.Text = "<img src='../images/icon_err.gif'>&nbsp;Note Title Type 3 has invalid character</img> </li>";
                return result = false;
            }
            bool isNoteTitleType4ContainSpecialChars = CheckSpecialChars(TextBox_NoteTitleType4.Text);
            if (isNoteTitleType4ContainSpecialChars == true)
            {
                Literal_error.Text = "<img src='../images/icon_err.gif'>&nbsp;Note Title Type 4 has invalid character</img> </li>";
                return result = false;
            }
            bool isNoteTitleType5ContainSpecialChars = CheckSpecialChars(TextBox_NoteTitleType5.Text);
            if (isNoteTitleType5ContainSpecialChars == true)
            {
                Literal_error.Text = "<img src='../images/icon_err.gif'>&nbsp;Note Title Type 5 has invalid character</img> </li>";
                return result = false;
            }
            bool isNoteTitleType6ContainSpecialChars = CheckSpecialChars(TextBox_NoteTitleType6.Text);
            if (isNoteTitleType6ContainSpecialChars == true)
            {
                Literal_error.Text = "<img src='../images/icon_err.gif'>&nbsp;Note Title Type 6 has invalid character</img> </li>";
                return result = false;
            }
            bool isNoteTitleType7ContainSpecialChars = CheckSpecialChars(TextBox_NoteTitleType7.Text);
            if (isNoteTitleType7ContainSpecialChars == true)
            {
                Literal_error.Text = "<img src='../images/icon_err.gif'>&nbsp;Note Title Type 7 has invalid character</img> </li>";
                return result = false;
            }

            return result;
        }

      
    }
}