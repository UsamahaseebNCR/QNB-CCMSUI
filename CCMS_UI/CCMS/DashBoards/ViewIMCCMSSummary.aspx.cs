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
using System.Data.SqlClient;

using System.Reflection;
using System.Diagnostics;
using System.Text;
using Avanza.iSuite.UI;
using Avanza.CCMS.DAL;
using ceTe.DynamicPDF;
using System.Threading;
using System.Collections.Generic;

namespace CCMSUI.CCMS.DashBoards
{
    public partial class ViewIMCCMSSummary : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //if (!CheckRight((int)Permissions.ViewIMCCMSDashboard, true))
                //    Response.Redirect("../../admin/aspx/AccessDenied.aspx");

                //  ViewState["SortExpression"] = "total ";
                //  ViewState["sortDirection"] = "asc ";
                //  ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString()
                BindGrid();

            }
        }
        //string orderBy
     //   public static
         void BindGrid()
        {
            SqlCommand cmd = null;
            LogableTask task = LogableTask.NewTask("BindGrid");

            try
            {
                int val = Convert.ToInt32(Request.QueryString["value"]);
                int type = Convert.ToInt32(Request.QueryString["type"]);
                this.metaRefresh.Content = (((AppSetting)base.Application[ApplicationVars.AppSettings.ToString()]).DashboardRefreshInterval * 60).ToString();
                this.Page.RegisterStartupScript("script", "<script>function init() {cd(\"" + (int.Parse(this.metaRefresh.Content) / 60) + "\");} init();</script>");
                cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;
                cmd.CommandType = CommandType.StoredProcedure;
                if (type == 0)
                {
                    cmd.CommandText = "GetTotalAvailabilityDetail";
                    cmd.Parameters.Add("@atm_type", SqlDbType.Int);
                    cmd.Parameters[0].Value = val;

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                  
                    GridView_MachinesAvailability.DataSource = dt;
                    GridView_MachinesAvailability.DataBind();
                    Panel1.Visible = true;
                    Panel2.Visible = false;
                    Panel3.Visible = false;
                    Panel4.Visible = false;
                    Panel5.Visible = false;
                    Panel6.Visible = false;
                }
                else if (type == 1)
                {
                    cmd.CommandText = "GetDeadAtmsDetails";
                    cmd.Parameters.Add("@type", SqlDbType.Int);
                    cmd.Parameters[0].Value = val;

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);


                    GridView_DeadATMsDetail.DataSource = dt;
                    GridView_DeadATMsDetail.DataBind();
                    Panel1.Visible = false;
                    Panel2.Visible = true;
                    Panel3.Visible = false;
                    Panel4.Visible = false;
                    Panel5.Visible = false;
                    Panel6.Visible = false;
                }
                else if (type == 2)
                {
                    cmd.CommandText = "GetNoTransactionMonitoringDetails";
                    cmd.Parameters.Add("@alerttype", SqlDbType.Int);
                    cmd.Parameters[0].Value = val;

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (dt.Rows[i][1].ToString() != "" && dt.Rows[i][5].ToString() != "")
                        {
                              int inactivityCount = GetInactivityPeriodCount((DateTime)dt.Rows[i][1], (int)dt.Rows[i][5]);
                              dt.Rows[i][2] = inactivityCount;
                        }
                    }

                    GridView_NoTransactionMonitoringDetails.DataSource = dt;
                    GridView_NoTransactionMonitoringDetails.DataBind();
                    Panel1.Visible = false;
                    Panel2.Visible = false;
                    Panel3.Visible = true;
                    Panel4.Visible = false;
                    Panel5.Visible = false;
                    Panel6.Visible = false;
                }
                else if (type == 3)
                {
                    cmd.CommandText = "GetCashMonitoringSummaryDetail";
                    //cmd.Parameters.Add("@type", SqlDbType.Int);
                    //cmd.Parameters[0].Value = val;

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);


                    GridView_CashMonitoringDetail.DataSource = dt;
                    GridView_CashMonitoringDetail.DataBind();
                    Panel1.Visible = false;
                    Panel2.Visible = false;
                    Panel3.Visible = false;
                    Panel4.Visible = true;
                    Panel5.Visible = false;
                    Panel6.Visible = false;
                }
                else if (type == 4)
                {
                    cmd = null;
                    cmd = ConnectionFactory.GetNewCommand(false);
                    cmd.CommandTimeout = 500;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "GetTMDandMachineDownBeyondNHoursSummaryDetail";
                    cmd.Parameters.Add("@ID", SqlDbType.Int);
                    cmd.Parameters[0].Value = val;
                    cmd.Parameters.Add("@alerttype", SqlDbType.Int);
                    cmd.Parameters[1].Value = 67;

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);


                    GridView_TMDMonitoringDetails.DataSource = dt;
                    GridView_TMDMonitoringDetails.DataBind();
                    Panel1.Visible = false;
                    Panel2.Visible = false;
                    Panel3.Visible = false;
                    Panel4.Visible = false;
                    Panel5.Visible = true;
                    Panel6.Visible = false;
                }
                else if (type == 5)
                {
                    cmd = null;
                    cmd = ConnectionFactory.GetNewCommand(false);
                    cmd.CommandTimeout = 500;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "GetTMDandMachineDownBeyondNHoursSummaryDetail";
                    cmd.Parameters.Add("@ID", SqlDbType.Int);
                    cmd.Parameters[0].Value = val;
                    cmd.Parameters.Add("@alerttype", SqlDbType.Int);
                    cmd.Parameters[1].Value = 68;

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);


                    GridView_MachineDownNHoursDetails.DataSource = dt;
                    GridView_MachineDownNHoursDetails.DataBind();
                    Panel1.Visible = false;
                    Panel2.Visible = false;
                    Panel3.Visible = false;
                    Panel4.Visible = false;
                    Panel6.Visible = true;
                    Panel5.Visible = false;
                }
                Literal_AddHeading.Text = "IM CCMS Summary Detail";

            }


            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_errorMsg.Text = "<li>" + ex.Message + "</li>";
            }
            finally
            {
                if (cmd != null)
                    if (cmd.Connection != null)
                        if (cmd.Connection.State == ConnectionState.Open)
                            cmd.Connection.Close();

                task.EndTask();
            }
        }

          //List<object> lst = new List<object>();
          //          for(int i= 0 ;i < dt.Rows.Count ;i++){
          //               DataRow dr = dt.Rows[i];
          //              object newObject = new { MachineID = dr[""],Address = dr[""]};
          //              lst.Add(newObject);
          //          }
        public int GetInactivityPeriodCount(DateTime generationTime, int inactivityPeriod)
        {

            DateTime startTime = generationTime;
            DateTime endTime = DateTime.Now; //DateTime.Now.AddSeconds( 75 );
            TimeSpan span = endTime.Subtract(startTime);
            int hours = (span.Days * 24) + span.Hours;
            hours /= (inactivityPeriod / 3600);


            return hours + 1;
        }
    }

}