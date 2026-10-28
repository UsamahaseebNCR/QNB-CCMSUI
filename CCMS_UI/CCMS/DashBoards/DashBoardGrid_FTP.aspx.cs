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
namespace CCMSUI.CCMS.DashBoards
{
    public partial class DashBoardGrid_FTP : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                if (!CheckRight((int)Permissions.ViewOrderDownloadAndDailyFeedStatus, true))
                    Response.Redirect("../../admin/aspx/AccessDenied.aspx");

                ViewState["SortExpression"] = "region_name ";
                ViewState["sortDirection"] = "asc ";
                BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());

            }

        }

        private string Filter()
        {
            AppSetting appSetting = (AppSetting)Application[ApplicationVars.AppSettings.ToString()];
            if (appSetting.ThresholdForFtp == -1)
                return "";
            else

            return " and f.creation_time >= convert(datetime,'" + DateTime.Today.AddDays(-appSetting.ThresholdForFtp.Value).ToString("dd/MM/yyyy") + "',103) " +
                       " and f.creation_time <=convert(datetime,'" + DateTime.Today.AddDays(-appSetting.ThresholdForFtp.Value).ToString("dd/MM/yyyy") + " 23:59:59',103) ";


        }

        private void BindGrid(string orderBy)
        {
            SqlCommand cmd = null;
            LogableTask task = LogableTask.NewTask("BindGrid");
            try
            {


                metaRefresh.Content = (((AppSetting)Application[ApplicationVars.AppSettings.ToString()]).DashboardRefreshInterval * 60).ToString();
                Page.RegisterStartupScript("script", "<script>function init() {cd(\"" + (int.Parse(metaRefresh.Content) / 60) + "\");} init();</script>");

                cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;
                cmd.CommandText = @"select f.last_invoked_at,f.creation_time,failure_reason reason, retry_count, end_time , dbo.GetFTPStatusFriendlyName(status) status,task_type_name, region_name
                    from ftp_file_info f,task_type t,region
                    where f.region_id = region.region_id 
                    and f.task_type_id=t.task_type_id and status in ('failed','completedWithExceptions') "+
                    Filter() + " order by " + orderBy;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dtMonitoring = new DataTable();
                adapter.Fill(dtMonitoring);

                //DataTable dt1 = new DataTable();

                //dt1.Columns.Add("creation_time", typeof(DateTime));
                //dt1.Columns.Add("reason", typeof(string));
                //dt1.Columns.Add("retry_count", typeof(string));
                //dt1.Columns.Add("server_name", typeof(string));
                //dt1.Columns.Add("status", typeof(string));
                //dt1.Columns.Add("task_type_name", typeof(string));
                //dt1.Columns.Add("region_name", typeof(string));
                
                //DataTable dt2 = new DataTable();
                //dt2 = dt1.Clone();
                //for (int i = 0; i < dtMonitoring.Rows.Count / 2; i++)
                //{
                //    DataRow dr = dt1.NewRow();
                //    dr[0] = dtMonitoring.Rows[i][0];
                //    dr[1] = dtMonitoring.Rows[i][1];
                //    dr[2] = dtMonitoring.Rows[i][2];
                //    dr[3] = dtMonitoring.Rows[i][3];
                //    dr[4] = dtMonitoring.Rows[i][4];
                //    dr[5] = dtMonitoring.Rows[i][5];
                //    dr[6] = dtMonitoring.Rows[i][6];
                    
                //    dt1.Rows.Add(dr);

                //}
                //for (int i = dtMonitoring.Rows.Count / 2; i < dtMonitoring.Rows.Count; i++)
                //{
                //    DataRow dr = dt2.NewRow();
                //    dr[0] = dtMonitoring.Rows[i][0];
                //    dr[1] = dtMonitoring.Rows[i][1];
                //    dr[2] = dtMonitoring.Rows[i][2];
                //    dr[3] = dtMonitoring.Rows[i][3];
                //    dr[4] = dtMonitoring.Rows[i][4];
                //    dr[5] = dtMonitoring.Rows[i][5];
                //    dr[6] = dtMonitoring.Rows[i][6];
                //    dt2.Rows.Add(dr);

                //}


                GridView_Ftp.DataSource = dtMonitoring;
               // GridView2.DataSource = dt2;
                GridView_Ftp.DataBind();
               // GridView2.DataBind();


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
        protected void GridView_Ftp_Sorting(object sender, GridViewSortEventArgs e)
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
    }
}