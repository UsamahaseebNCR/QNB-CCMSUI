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
    public partial class DashBoard_Home : BasePage
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
                string page_id = Session["pageID"].ToString();
                BindNoteSetType();
                Label_Title.Text = PageIdToTitle(int.Parse(Request.QueryString["pageID"])) + " Dashboard";
                if (Request.QueryString["pageID"] == "3")
                    trMinThreshold.Visible = true;
                else
                    if (Request.QueryString["pageID"] == "8")
                    {
                        trTrxnStatus.Visible = true;


                        /*<asp:ListItem Text="BNA Disputed" Value="0" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="BNA Failed" Value="1"></asp:ListItem>
                                                <asp:ListItem Text="CPM Disputed" Value="2"></asp:ListItem>
                                                <asp:ListItem Text="CPM Failed" Value="3"></asp:ListItem>
                                                <asp:ListItem Text="Captured Card" Value="4"></asp:ListItem>
                                                <asp:ListItem Text="Normal Transactions" Value="5"></asp:ListItem>*/
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
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            bool isCheck = false;
            LogableTask task = LogableTask.NewTask("Search");
            StringBuilder builder = new StringBuilder();
            try
            {
                string pageName = PageIdToPageName(int.Parse(Request.QueryString["pageID"]));
                if (DropDownList_NoteSetType.SelectedIndex > 0)
                    pageName += "?n=" + DropDownList_NoteSetType.SelectedItem.Value + (CheckBox_ThresholdReached.Checked ? "&f=1" : "&f=0");
                else
                    pageName += (CheckBox_ThresholdReached.Checked ? "?f=1" : "?f=0");

                if (Request.QueryString["pageID"] == "8")
                {
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
                }
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
        private string PageIdToPageName(int id)
        {
            string[] pages = { "DashBoard_Summary.aspx", "DashBoardGrid_Alerts.aspx","DashBoardGrid_CashOrderMonitoring.aspx",
                                 "DashBoardGrid_CashPositions.aspx","DashBoardGrid_Deposits.aspx","DashBoardGrid_FTP.aspx","DashBoardGrid_Tasks.aspx" ,"DashBoardGrid_BWACCashPositions.aspx" ,"DashBoardGrid_EJFraudDetected.aspx"
                             };

            return pages[id];


        }
        private string PageIdToTitle(int id)
        {
            string[] pages = { "Summary", 
                               "Alerts",
                               "Cash Order Monitoring",
                               "Cash Position",
                               "Deposits",
                               "Order Download And Daily Feed File",
                               "Tasks",
                               "BWAC Cash Position",
                               "Exception Handling"
                             };

            return pages[id];


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