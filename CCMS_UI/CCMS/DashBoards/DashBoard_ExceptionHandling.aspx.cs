using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Reflection;
using System.Diagnostics;
using Avanza.iSuite.UI;
using System.Text;

namespace CCMSUI.CCMS.DashBoards
{
    public partial class DashBoard_ExceptionHandling : BasePage
    {
        private void BindNoteSetType()
        {
            DropDownList_NoteSetType.Items.Clear();
            string regionID = (string)Session[SessionVars.SelectedRegions.ToString()];
            if (regionID != null /*&& !regionID.Contains(",")*/
                && regionID != "1")
            {
                DropDownList_NoteSetType.Items.Add("Select");
                Avanza.CCMS.DAL.NoteSetType.NoteSetTypeReader reader =
                Avanza.CCMS.DAL.NoteSetType.ExecuteReader("region_id=" + AddEditATM.GetOrganization(regionID.Contains(",") ? int.Parse(regionID.Substring(0, regionID.IndexOf(","))) : int.Parse(regionID)));
                while (reader.Read())
                {
                    DropDownList_NoteSetType.Items.Add(new ListItem(reader.CurrentNoteSetType.NoteSetTypeName, reader.CurrentNoteSetType.NoteSetTypeId.ToString()));
                }
                reader.Close();
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //string page_id = Session["pageID"].ToString();
                BindNoteSetType();
                Label_Title.Text = "Exception Handling Dashboard";

                trTrxnStatus.Visible = true;

                if (CheckRight((int)Permissions.ViewEjBNADisputedTransactions))
                    CheckBoxList_Status.Items.Add(new ListItem("BNA Disputed", "0"));

                if (CheckRight((int)Permissions.ViewEjBNAFailedTransactions))
                    CheckBoxList_Status.Items.Add(new ListItem("BNA Failed", "1"));

                if (CheckRight((int)Permissions.ViewEjCPMDisputedTransactions))
                    CheckBoxList_Status.Items.Add(new ListItem("CPM Disputed", "2"));

                if (CheckRight((int)Permissions.ViewEjCPMFailedTransactions))
                    CheckBoxList_Status.Items.Add(new ListItem("CPM Failed", "3"));

                if (CheckRight((int)Permissions.ViewEjCapturedCardTransactions))
                    CheckBoxList_Status.Items.Add(new ListItem("Captured Card", "4"));

                if (CheckRight((int)Permissions.ViewEjNormalTransactions))
                    CheckBoxList_Status.Items.Add(new ListItem("Normal Transactions", "5"));
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            bool isCheck = false;
            LogableTask task = LogableTask.NewTask("Search");
            StringBuilder builder = new StringBuilder();
            try
            {
                string pageName = "DashBoardGrid_EJFraudDetected.aspx";
                if (DropDownList_NoteSetType.SelectedIndex > 0)
                    pageName += "?n=" + DropDownList_NoteSetType.SelectedItem.Value + (CheckBox_ThresholdReached.Checked ? "&f=1" : "&f=0");
                else
                    pageName += (CheckBox_ThresholdReached.Checked ? "?f=1" : "?f=0");

                
                    foreach (ListItem item in CheckBoxList_Status.Items)
                    {
                        if (item.Selected)
                        {
                            builder.Append(item.Text + ",");
                            isCheck = true;
                        }
                    }
                    ///edited by shariq for the purpose of check selection on 30aug 2015
                    if (isCheck == false)
                        throw new Exception("Status not selected");
                    //******
                
                if (builder.Length > 0)
                {
                    builder.Append("-1");
                    pageName += "&c=" + builder.ToString();
                }


                Session["Grid1"] = null;
                Session["Grid2"] = null;
                Session["Grid3"] = null;
                Session["Grid4"] = null;

                Literal_Scr.Text = "<script>javascript:window.open('" + pageName + "','','location=0,status=1,resizable=yes,scrollbars=yes');</script>";
            }

            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_ok.Text = "<li>" + ex.Message + "</li>";
            }
            finally
            {
                task.EndTask();
            }
        }
        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        }
        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            BindNoteSetType();
        }
    }
}