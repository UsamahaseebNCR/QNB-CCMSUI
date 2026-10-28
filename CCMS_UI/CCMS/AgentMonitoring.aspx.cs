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
using System.Data.SqlClient;
using System.Reflection;
using System.Diagnostics;
using Avanza.CCMS.DAL;

using System.Text;

namespace CCMSUI.CCMS
{
    public partial class AgentMonitoring : BasePage
    {
        protected void Page_PreInit(object sender, EventArgs e)
        {
            refresh.ServerClick += new System.EventHandler(refresh_ServerClick);
        }

        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            Button_search_Click(this, null);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (!CheckRight((int)Permissions.AlertMonitoringHistory))
                    Response.Redirect("~/admin/AccessDenied.aspx");

                ViewState["sortDirection"] = " asc";
                ViewState["SortExpression"] = "title";

                TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");

                DropDownList_type.Items.Add(new ListItem("Any", "Any"));
                AlertType.AlertTypeReader alertTypeReader = AlertType.ExecuteReader("1=1");
                while (alertTypeReader.Read())
                    DropDownList_type.Items.Add(new ListItem(alertTypeReader.CurrentAlertType.AlertTypeName, alertTypeReader.CurrentAlertType.AlertTypeId.ToString()));
                alertTypeReader.Close();


                ////Array alertTypes = Enum.GetValues(typeof(EnumAlertType));
                ////for (int i = 0; i < alertTypes.Length; i++)
                ////    DropDownList_type.Items.Add(new ListItem(SpaceCreator(alertTypes.GetValue(i).ToString()), ((int)alertTypes.GetValue(i)).ToString()));

                //if (DropDownList_type.Items.Count > 0)
                //{
                //    DropDownList_type.SelectedIndex = 4;
                //    DropDownList_type_SelectedIndexChanged(sender, e);
                //}

                //BindGrid("title asc");
            }
        }

        protected string Filter()
        {
            string filter = "";
          //  int alertType = Convert.ToInt32(DropDownList_type.SelectedValue);

            //if (TextBox_ip.Text.Length > 0 && TextBox_ip.Visible == true)
             //   filter += "atm.ip = '" + TextBox_ip.Text + "' and ";
            if (TextBox_FromDate.Text.Length > 0)
                filter += "generated_at >= convert(datetime, '" + TextBox_FromDate.Text + " 00:00:00', 103) and ";
            if (TextBox_ToDate.Text.Length > 0)
                filter += "generated_at <= convert(datetime, '" + TextBox_ToDate.Text + " 23:59:59', 103) and ";
            
            if (DropDownList_type.SelectedIndex > 0)
                filter += "alert_type.alert_type_id = " + DropDownList_type.SelectedValue + " and ";

            filter += " atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and ";

            return filter.Remove(filter.LastIndexOf(" and "));
        }

        string GetGridSQL(int alertTypeID)
        {
            AlertType alertType = AlertType.LoadAlertTypeByPk(alertTypeID);

            if (alertType.AlertTypeId == (int)EnumAlertType.TCPConnectionDown
                || alertType.AlertTypeId == (int)EnumAlertType.TCPConnectionUp
                                 || alertType.AlertTypeId == (int)EnumAlertType.CashOrderField20Missing
                                 || alertType.AlertTypeId == (int)EnumAlertType.ConfigurationMismatch
                                 || alertType.AlertTypeId == (int)EnumAlertType.ConfigurationUploadFailed
                                 || alertType.AlertTypeId == (int)EnumAlertType.DenominationMissing
                                 || alertType.AlertTypeId == (int)EnumAlertType.TerminalNotLicensed
                                 || alertType.AlertTypeId == (int)EnumAlertType.ATMCashLevelFileDownloadFailed
                                 || alertType.AlertTypeId == (int)EnumAlertType.ATMOutOfCash

                                 || alertType.AlertTypeId == (int)EnumAlertType.MinOperatingBalance
                                 || alertType.AlertTypeId == (int)EnumAlertType.PurgeBinThresholdReached
                                 || alertType.AlertTypeId == (int)EnumAlertType.CashOrderUploadFailed)
                return @"select top 1000 atm_alert_id alert_id, generated_at, resolve_at, atm.title, atm.ip, region.region_name,
                         alert_type_name, generate_notification_sent,resolve_notification_sent,
                         generate_at_retry_remaining , resolve_at_retry_remaining,failure_reason
                         from atm_alert inner join alert_type on atm_alert.alert_type_id = alert_type.alert_type_id 
                         inner join atm on atm_alert.atm_id = atm.atm_id inner join region on atm.region_id = region.region_id";

            else if (alertType.AlertTypeId == (int)EnumAlertType.Licensing
                  || alertType.AlertTypeId == (int)EnumAlertType.HardDiskFreeSpace
                  || alertType.AlertTypeId == (int)EnumAlertType.ATMFriendlyNameMissing
                || alertType.AlertTypeId == (int)EnumAlertType.VaultLowBalanceThresholdReached)
                return @"select top 1000 general_alert_id alert_id, generated_at,  retry_remaining,
                         alert_type_name, alert_msg,generate_notification_sent,failure_reason
                         from general_alert inner join alert_type on general_alert.alert_type_id = alert_type.alert_type_id ";

            else if (alertType.AlertTypeId == (int)EnumAlertType.DailyFeedUpload
        || alertType.AlertTypeId == (int)EnumAlertType.CashOrderDownload
        || alertType.AlertTypeId == (int)EnumAlertType.CashOrderImportFailed
                || alertType.AlertTypeId == (int)EnumAlertType.DFFSuspect
      || alertType.AlertTypeId == (int)EnumAlertType.CashOrderImportCompletedWithExceptions)
                return @"select top 1000 organization_alert_id alert_id, generated_at, retry_remaining,failure_reason
                         alert_type_name, alert_msg,generate_notification_sent,failure_reason
                         from  organization_alert inner join alert_type on organization_alert.alert_type_id = alert_type.alert_type_id
                         inner join on ftp_file_info on organization_alert.ftp_file_info_id = ftp_file_info.ftp_file_info_id 
                         inner join on region on ftp_file_info.region_id = region.region_id ";

            return "";

        }
        //protected void ShowHideGridColumns()
        //{
        //    int alertType = Convert.ToInt32(DropDownList_type.SelectedValue);

        //    GridView_AgentMonitoring.Columns[0].Visible = true;

        //    if (DropDownList_type.Items.Count > 0)
        //    {
        //        switch (alertType)
        //        {
        //            case (int)EnumAlertType.Licensing:
        //            case (int)EnumAlertType.HardDiskFreeSpace:
        //                GridView_AgentMonitoring.Columns[0].Visible = true;
        //                GridView_AgentMonitoring.Columns[1].Visible = true;
        //                GridView_AgentMonitoring.Columns[2].Visible = false;
        //                GridView_AgentMonitoring.Columns[3].Visible = false;
        //                GridView_AgentMonitoring.Columns[4].Visible = false;
        //                //GridView_AgentMonitoring.Columns[5].Visible = false;
        //                GridView_AgentMonitoring.Columns[5].Visible = true;
        //                GridView_AgentMonitoring.Columns[6].Visible = false;
        //                GridView_AgentMonitoring.Columns[7].Visible = false; //Generation
        //                GridView_AgentMonitoring.Columns[8].Visible = true; // Resolu
        //                GridView_AgentMonitoring.Columns[9].Visible = false;    // Alert               
        //                break;
        //            case (int)EnumAlertType.DailyFeedUpload:
        //            case (int)EnumAlertType.CashOrderDownload:
        //            case (int)EnumAlertType.CashOrderImportFailed:
        //            case (int)EnumAlertType.CashOrderImportCompletedWithExceptions:
        //                GridView_AgentMonitoring.Columns[0].Visible = true;
        //                GridView_AgentMonitoring.Columns[1].Visible = true;
        //                GridView_AgentMonitoring.Columns[2].Visible = false;
        //                GridView_AgentMonitoring.Columns[3].Visible = false;
        //                GridView_AgentMonitoring.Columns[4].Visible = true;
        //                //GridView_AgentMonitoring.Columns[5].Visible = false;
        //                GridView_AgentMonitoring.Columns[5].Visible = true;
        //                GridView_AgentMonitoring.Columns[6].Visible = false;
        //                GridView_AgentMonitoring.Columns[7].Visible = false;
        //                GridView_AgentMonitoring.Columns[8].Visible = true;
        //                GridView_AgentMonitoring.Columns[9].Visible = false;
        //                break;
        //            case (int)EnumAlertType.TCPConnectionDown:
        //                GridView_AgentMonitoring.Columns[0].Visible = true;
        //                GridView_AgentMonitoring.Columns[1].Visible = true;
        //                GridView_AgentMonitoring.Columns[2].Visible = true;
        //                GridView_AgentMonitoring.Columns[3].Visible = true;
        //                GridView_AgentMonitoring.Columns[4].Visible = true;
        //                //GridView_AgentMonitoring.Columns[5].Visible = true;
        //                GridView_AgentMonitoring.Columns[5].Visible = true;
        //                GridView_AgentMonitoring.Columns[6].Visible = false;
        //                GridView_AgentMonitoring.Columns[7].Visible = false;
        //                GridView_AgentMonitoring.Columns[8].Visible = true;
        //                GridView_AgentMonitoring.Columns[9].Visible = false;
        //                break;

        //            case (int)EnumAlertType.TCPConnectionUp:
        //                GridView_AgentMonitoring.Columns[0].Visible = true;
        //                GridView_AgentMonitoring.Columns[1].Visible = true;
        //                GridView_AgentMonitoring.Columns[2].Visible = true;
        //                GridView_AgentMonitoring.Columns[3].Visible = true;
        //                GridView_AgentMonitoring.Columns[4].Visible = true;
        //                //GridView_AgentMonitoring.Columns[5].Visible = true;
        //                GridView_AgentMonitoring.Columns[5].Visible = true;
        //                GridView_AgentMonitoring.Columns[6].Visible = true;
        //                GridView_AgentMonitoring.Columns[7].Visible = false;
        //                GridView_AgentMonitoring.Columns[8].Visible = true;
        //                GridView_AgentMonitoring.Columns[9].Visible = true;
        //                break;

        //            case (int)EnumAlertType.TerminalNotLicensed:
        //            case (int)EnumAlertType.DenominationMissing:
        //            case (int)EnumAlertType.ConfigurationUploadFailed:
        //            case (int)EnumAlertType.ConfigurationMismatch:
        //            case (int)EnumAlertType.CashOrderField20Missing:
        //            case (int)EnumAlertType.ATMCashLevelFileDownloadFailed:
        //                GridView_AgentMonitoring.Columns[0].Visible = true;
        //                GridView_AgentMonitoring.Columns[1].Visible = true;
        //                GridView_AgentMonitoring.Columns[2].Visible = true;
        //                GridView_AgentMonitoring.Columns[3].Visible = true;
        //                GridView_AgentMonitoring.Columns[4].Visible = true;
        //                //GridView_AgentMonitoring.Columns[5].Visible = false;
        //                GridView_AgentMonitoring.Columns[5].Visible = true;
        //                GridView_AgentMonitoring.Columns[6].Visible = false;
        //                GridView_AgentMonitoring.Columns[7].Visible = false;
        //                GridView_AgentMonitoring.Columns[8].Visible = true;
        //                GridView_AgentMonitoring.Columns[9].Visible = false;
        //                break;
        //            case (int)EnumAlertType.ATMFriendlyNameMissing:
        //                GridView_AgentMonitoring.Columns[0].Visible = true;
        //                GridView_AgentMonitoring.Columns[1].Visible = false;
        //                GridView_AgentMonitoring.Columns[2].Visible = false;
        //                GridView_AgentMonitoring.Columns[3].Visible = false;
        //                GridView_AgentMonitoring.Columns[4].Visible = false;
        //                //GridView_AgentMonitoring.Columns[5].Visible = false;
        //                GridView_AgentMonitoring.Columns[5].Visible = false;
        //                GridView_AgentMonitoring.Columns[6].Visible = false;
        //                GridView_AgentMonitoring.Columns[7].Visible = true;
        //                GridView_AgentMonitoring.Columns[8].Visible = true;
        //                GridView_AgentMonitoring.Columns[9].Visible = false;
        //                break;
        //        }
        //    }
        //}

        protected void GridView_AgentMonitoring_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView_AgentMonitoring.PageIndex = e.NewPageIndex;
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }

        protected void GridView_AgentMonitoring_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (ViewState["SortExpression"].ToString() == e.SortExpression)
            {
                if (ViewState["sortDirection"].ToString() == " asc")
                    ViewState["sortDirection"] = " desc";
                else
                    ViewState["sortDirection"] = " asc";
            }
            else
            {
                ViewState["SortExpression"] = e.SortExpression;
                ViewState["sortDirection"] = " asc";
            }

            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }

        protected void GridView_AgentMonitoring_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            LogableTask task = LogableTask.NewTask("GridView_AgentMonitoring_RowDataBound");
            string body = null;
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    //if (!e.Row.Cells[0].Text.Equals(String.Empty))
                    //{

                    if (bool.Parse(((DataRowView)e.Row.DataItem)["generate_notification_sent"].ToString()))
                        ((Image)e.Row.FindControl("image_emailNotification1")).ImageUrl = "~/images/msg_sent.gif";

                    else
                    {
                        ((Image)e.Row.FindControl("image_emailNotification1")).ImageUrl = "~/images/msg_notsent.gif";
                        body = "Retry Count : " + ((DataRowView)e.Row.DataItem)["generate_at_retry_remaining"] + "<br/>Failure Reason " + ((DataRowView)e.Row.DataItem)["failure_reason"];
                        ((Image)e.Row.FindControl("image_emailNotification1")).Attributes.Add("title", "header=[Message Failure Details] body=[" + body + ".]");

                    }
                    if (((DataRowView)e.Row.DataItem)["resolve_notification_sent"] != DBNull.Value)
                    {
                        if (bool.Parse(((DataRowView)e.Row.DataItem)["resolve_notification_sent"].ToString()))
                            ((Image)e.Row.FindControl("image_emailNotification2")).ImageUrl = "~/images/msg_sent.gif";
                        else
                        {
                            ((Image)e.Row.FindControl("image_emailNotification2")).ImageUrl = "~/images/msg_notsent.gif";
                            body = "Retry Count : " + ((DataRowView)e.Row.DataItem)["generate_at_retry_remaining"] + "<br/>Failure Reason " + ((DataRowView)e.Row.DataItem)["failure_reason"];
                            ((Image)e.Row.FindControl("image_emailNotification2")).Attributes.Add("title", "header=[Message Failure Details] body=[" + body + ".]");

                        }
                    }
                    // }
                }

            }
            catch (Exception ex)
            {
                Literal_error.Text = ex.Message;
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            {
                task.EndTask();
            }
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            LogableTask task = LogableTask.NewTask("GridView1_RowDataBound");
            string body = null;
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    //if (!e.Row.Cells[0].Text.Equals(String.Empty))
                    //{

                    if (bool.Parse(((DataRowView)e.Row.DataItem)["generate_notification_sent"].ToString()))
                        ((Image)e.Row.FindControl("image_emailNotification1")).ImageUrl = "~/images/msg_sent.gif";

                    else
                    {
                        ((Image)e.Row.FindControl("image_emailNotification1")).ImageUrl = "~/images/msg_notsent.gif";
                        body = "Retry Count : " + ((DataRowView)e.Row.DataItem)["retry_remaining"] + "<br/>Failure Reason " + ((DataRowView)e.Row.DataItem)["failure_reason"];
                        ((Image)e.Row.FindControl("image_emailNotification1")).Attributes.Add("title", "header=[Message Failure Details] body=[" + body + ".]");

                    }


                }

            }
            catch (Exception ex)
            {
                Literal_error.Text = ex.Message;
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            {
                task.EndTask();
            }
        }

        protected void BindGridWithCache()
        {
            //ShowHideGridColumns();

            DataTable dtAgentMonitoring = (DataTable)Cache["dtAgentMonitoring"];

            dtAgentMonitoring.DefaultView.Sort = ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString();

            GridView_AgentMonitoring.DataSource = dtAgentMonitoring;
            GridView_AgentMonitoring.DataBind();

            GridView_AgentMonitoring.Columns[0].Visible = false;

            Literal_count.Text = dtAgentMonitoring.Rows.Count + " records.";
        }

        protected void BindGrid(string orderby)
        {
            LogableTask task = LogableTask.NewTask("BindGrid");

            try
            {
                //ShowHideGridColumns();

                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetAlert";
                cmd.Parameters.Add(new SqlParameter("Filter",Filter()));
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dtAgentMonitoring = new DataTable();
                adapter.Fill(dtAgentMonitoring);
                 
                GridView_AgentMonitoring.DataSource = dtAgentMonitoring;
                GridView_AgentMonitoring.DataBind();
                 Cache["dtAgentMonitoring"] = dtAgentMonitoring;
                 
                Literal_count.Text = dtAgentMonitoring.Rows.Count + " records.";
            }
            catch (Exception ex)
            {
                Literal_error.Text = ex.Message;
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            {
                task.EndTask();
            }
        }

        protected void Button_search_Click(object sender, EventArgs e)
        {
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }

        protected void Button_Reset_Click(object sender, EventArgs e)
        {
            TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            if (DropDownList_type.Items.Count > 0)
            {
                DropDownList_type.SelectedIndex = 0;
                //DropDownList_type_SelectedIndexChanged(sender, e);
            }
            
        }

       
        protected string SpaceCreator(string alertTypeName)
        {
            StringBuilder builder = new StringBuilder(100);
            builder.Append(alertTypeName[0]);

            for (int counter = 1; counter < alertTypeName.Length; counter++)
            {
                if (char.IsUpper(alertTypeName, counter) && char.IsLower(alertTypeName, counter - 1))
                    builder.Append(" " + alertTypeName[counter]);
                else
                    builder.Append(alertTypeName[counter]);
            }

            return builder.ToString();
        }
    }
}