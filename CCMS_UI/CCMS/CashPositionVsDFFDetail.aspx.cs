using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data.SqlClient;
using System.Data;

namespace CCMSUI.CCMS
{
    public partial class CashPositionVsDFFDetail : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //BindReplenishmentGrid
                SqlCommand cmd= ConnectionFactory.GetNewCommand(false);
                cmd.CommandText = "select top 10 * from replenishment where atm_id = "+Request.QueryString["a"] + " order by rep_datetime desc";
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                GridView_Replenishment.DataSource = dt;
                GridView_Replenishment.DataBind();

                //BindCashPosition

                
                cmd.CommandText = "select top 10 * from cash_position where atm_id = " + Request.QueryString["a"] + " order by last_trxn_at desc";
                adapter = new SqlDataAdapter(cmd);
                dt = new DataTable();
                adapter.Fill(dt);
                GridView_CashPosition.DataSource = dt;
                GridView_CashPosition.DataBind();

                //Display User Task

                cmd.CommandText = @"select creation_time,status,generated_at,alert_msg 
                                from user_task inner join atm_alert on user_task.atm_alert_id=atm_alert.atm_alert_id
                                where status = 'Pending' and user_task.atm_id = " + Request.QueryString["a"] + " order by user_task_id";
                adapter = new SqlDataAdapter(cmd);
                dt = new DataTable();
                adapter.Fill(dt);
                foreach (DataRow dr in dt.Rows)
                    dr["alert_msg"] = CashDataViewer.FormatInformation(dr["alert_msg"].ToString()).Replace("Last","<br/>Last").Replace("Current","<br/>Current");
                GridView_UserTasks.DataSource = dt;
                GridView_UserTasks.DataBind();

                //Bind Test Cash

                cmd.CommandText = @"select top 10 * from test_cash_purged_notes where atm_id = " + Request.QueryString["a"] + " order by test_Cash_datetime desc ";
                adapter = new SqlDataAdapter(cmd);
                dt = new DataTable();
                adapter.Fill(dt);
                GridView_TestCash.DataSource = dt;
                GridView_TestCash.DataBind();




            }
        }
    }
}