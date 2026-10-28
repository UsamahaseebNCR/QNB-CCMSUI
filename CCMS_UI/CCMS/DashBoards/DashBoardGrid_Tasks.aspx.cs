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
    public partial class DashBoardGrid_Tasks : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
            if (!IsPostBack)
            {
                if (!CheckRight((int)Permissions.ViewTaskDashBoard, true))
                    Response.Redirect("../../admin/aspx/AccessDenied.aspx");


                ViewState["SortExpression"] = "title ";
                ViewState["sortDirection"] = "asc ";
                BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());

            }

        }
        private string Filter()
        {
            AppSetting appSetting = (AppSetting)Application[ApplicationVars.AppSettings.ToString()];
            if (appSetting.ThresholdForTask == -1)
                return "";
            else

                return " and F.creation_time >=convert(datetime,'" + DateTime.Today.AddDays(-appSetting.ThresholdForTask.Value).ToString("dd/MM/yyyy") + "',103) " +
                           " and F.creation_time <=convert(datetime,'" + DateTime.Today.AddDays(-appSetting.ThresholdForTask.Value).ToString("dd/MM/yyyy") + " 23:59:59',103) ";

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


//                cmd.CommandText = @"SELECT  ATM.ATM_ID,last_invoked, atm_type,IP, TITLE, F.creation_time ,task_type.task_type_name, dbo.GetStatusFriendlyName(status) status 
//             , end_time, retry_remaining, failure_reason FROM ATM inner join task F ON ATM.ATM_ID = F.ATM_ID 
//                inner join task_type on f.task_type_id = task_type.task_type_id where 
//                atm.IS_ACTIVE =1 " +
//               (Request.QueryString["n"] != null ? " and ATM.note_set_type_id = " + Request.QueryString["n"] : "") +
//                Filter() +
//                " and f.creation_time = (select MAX(creation_time) from task where atm_id = f.ATM_id) " +
//                " order by " + orderBy;



                DataTable dtMonitoring = new DataTable();
                cmd.CommandText = @"GetTasksDashboard";
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "SP -> " + cmd.CommandText);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@NoteSetFilter", SqlDbType.VarChar));
                cmd.Parameters[0].Value = (Request.QueryString["n"] != null ? " and ATM.note_set_type_id = " + Request.QueryString["n"] : "");
                cmd.Parameters.Add(new SqlParameter("@Filter", SqlDbType.VarChar));
                cmd.Parameters[1].Value = Filter();
                cmd.Parameters.Add(new SqlParameter("@OrderBy", SqlDbType.VarChar));
                cmd.Parameters[2].Value = orderBy;

                dsCashPositions ds = new dsCashPositions();
                //cmd.Connection.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                //SqlDataReader rdr = cmd.ExecuteReader();
                adapter.Fill(dtMonitoring);
                //dtMonitoring = QueryModifier.TaskQuery.DataTableQueryModify(cmd.CommandText);
                //adapter.Fill(dtMonitoring);

                //DataTable dt1 = new DataTable();
                //dt1.Columns.Add("atm_type", typeof(string)); //1
                //dt1.Columns.Add("ip", typeof(string));//2
                //dt1.Columns.Add("title", typeof(string));//3
                //dt1.Columns.Add("creation_time", typeof(DateTime));
                //dt1.Columns.Add("task_type_name", typeof(string));
                //dt1.Columns.Add("status", typeof(string));


                //DataTable dt2 = new DataTable();
                //dt2 = dt1.Clone();
                //for (int i = 0; i < dtMonitoring.Rows.Count / 2; i++)
                //{
                //    DataRow dr = dt1.NewRow();
                //    dr[0] = dtMonitoring.Rows[i][1];
                //    dr[1] = dtMonitoring.Rows[i][2];
                //    dr[2] = dtMonitoring.Rows[i][3];
                //    dr[3] = dtMonitoring.Rows[i][4];
                //    dr[4] = dtMonitoring.Rows[i][5];
                //    dr[5] = dtMonitoring.Rows[i][6];

                //    dt1.Rows.Add(dr);

                //}
                //for (int i = dtMonitoring.Rows.Count / 2; i < dtMonitoring.Rows.Count; i++)
                //{
                //    DataRow dr = dt2.NewRow();
                //    dr[0] = dtMonitoring.Rows[i][1];
                //    dr[1] = dtMonitoring.Rows[i][2];
                //    dr[2] = dtMonitoring.Rows[i][3];
                //    dr[3] = dtMonitoring.Rows[i][4];
                //    dr[4] = dtMonitoring.Rows[i][5];
                //    dr[5] = dtMonitoring.Rows[i][6];

                //    dt2.Rows.Add(dr);

                //}


                GridView_Tasks.DataSource = dtMonitoring;
                //GridView2.DataSource = dt2;
                GridView_Tasks.DataBind();
               // GridView2.DataBind();


            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex + ", Ex Message : " + ex.Message);
                Literal_errorMsg.Text = "<li>Error while generating tasks dashboard</li>";

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
        protected void GridView_Task_Sorting(object sender, GridViewSortEventArgs e)
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