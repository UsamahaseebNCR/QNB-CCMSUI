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
using System.Reflection;
using System.Diagnostics;
using Avanza.CCMS.DAL;

using System.Data.SqlClient;
using System.Text;
using System.Web.Configuration;

namespace CCMSUI.CCMS
{
    public partial class NoCashWithdrawalReport : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Page_Load");
            try
            {
                if (!Page.IsPostBack)
                {
                    if (!CheckRight((int)Permissions.NoCashWithdrawalReport))
                        Response.Redirect("../admin/AccessDenied.aspx");

                    TextBox_FromDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
                    TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");


                    for (int i = 0; i < 24; i++)
                    {
                        DropDownList_FromHour.Items.Add(new ListItem(i.ToString(), i.ToString()));
                        DropDownList_ToHour.Items.Add(new ListItem(i.ToString(), i.ToString()));
                    }

                    for (int i = 0; i < 60; i++)
                    {
                        DropDownList_FromMin.Items.Add(new ListItem(i.ToString(), i.ToString()));
                        DropDownList_ToMin.Items.Add(new ListItem(i.ToString(), i.ToString()));
                    }


                }
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            {
                task.EndTask();
            }
        }


        private string buildSearchCriteria()
        {
            string filter = "";
            if (TextBox_FromDate.Text.Length > 0)
                filter += " and trxn_datetime >= convert(datetime,'" + TextBox_FromDate.Text + " " + DropDownList_FromHour.SelectedItem.Value.PadLeft(2,'0')+":"+DropDownList_FromMin.SelectedItem.Value.PadLeft(2,'0')+ ":00',103)";

            if (TextBox_ToDate.Text.Length > 0)
                filter += " and trxn_datetime <= convert(datetime,'" + TextBox_ToDate.Text + " " + DropDownList_ToHour.SelectedItem.Value.PadLeft(2, '0') + ":" + DropDownList_ToMin.SelectedItem.Value.PadLeft(2, '0') + ":00',103)";

           
            
            return filter;
        }
        private void PopulateDataTable(DataTable dt)
        {
            int region_id = 0;
            int id = -1;

            DataTable dtRegions = new DataTable();
            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            cmd.CommandText = "select region_id, parent_region_id, is_organization, bank_logo from region where parent_region_id is not null";
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dtRegions);



            for (int i = 0; i < dt.Rows.Count; i++)
            {
                region_id = int.Parse(dt.Rows[i][3].ToString()); //parent

                while (true)
                {
                    DataRow[] innerArray = dtRegions.Select("region_id=" + region_id);
                    if (bool.Parse(innerArray[0][2].ToString()))
                    {
                        id = int.Parse(innerArray[0][0].ToString());
                        break;
                    }
                    else
                        region_id = int.Parse(innerArray[0][1].ToString());

                }
                //organization id found
                Region region = Region.LoadRegionByPk(id);
                dt.Rows[i][1] = region.RegionName;
                dt.Rows[i][4] = region.BankLogo;

            }

        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Search");
            SqlCommand cmd = null;
            try
            {
                DataSet Ds = null;
                string filter = "";
                if (WebConfigurationManager.AppSettings["isDeadATMExcluded"] == "1")
                    filter = " and ATM_id not in (select ATM_id from vHeartBeat where heart_beat_received_at >=convert(datetime,'" + TextBox_FromDate.Text + "',103) and " +
                                         "heart_beat_received_at <=convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103))";
           
                //dsCashWithdrawalSummary Ds1 = null; 

                AppUser objUser = (AppUser)Session[SessionVars.user.ToString()];
                cmd = ConnectionFactory.GetNewCommand(true);
                cmd.CommandTimeout = 100;
                cmd.CommandText = @"select atm_id 
                                    from atm where is_active = 1 
                                    and atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] +")" +
                                    " and atm_id not in ( " +

                                    " select vParsedTransaction.atm_id " +

                                    " from vParsedTransaction , user_atms " +
                                   "  where user_atms.atm_id = vParsedTransaction.atm_id " + buildSearchCriteria() +
                                
                                    "  and user_atms.user_id = " + ((AppUser)Session[SessionVars.user.ToString()]).UserId + " and amount>0 and is_eligible is null " +
                                    " group by vParsedTransaction.atm_id " +
                                    " having count(parsed_transaction_id)>0)";

                SqlDataReader reader = cmd.ExecuteReader();
                StringBuilder strBuilder = new StringBuilder();
                int count = 0;
                while (reader.Read())
                {
                    strBuilder.Append(reader.GetInt32(0) + ",");
                    count++;
                }
                reader.Close();
                strBuilder.Append("-1");

                if (count > 0)
                {
                    cmd.CommandText = @"select region.region_name,region.region_id, parent_region_id, 
outerATM.title, trxn_datetime last_trxn_at, amount ,outerATM.location
	from atm  outerATM inner join region 
on outerATM.region_id = region.region_id 
left join vParsedTransaction
	on vParsedTransaction.atm_id = outerATM.atm_id 

	
and trxn_datetime in 
	(
	select max(trxn_datetime)
		
from vParsedTransaction 


	where atm_id = outerATM.atm_id and amount>0)
     where outerATM.atm_id in (" + strBuilder.ToString() + ")" + filter;


                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    Ds = new dsNoCashWithdrawals();



                    adapter.Fill(Ds.Tables["DataTable1"]);

                    PopulateDataTable(Ds.Tables["DataTable1"]);

                    //adapter.SelectCommand.CommandText = "select bank_logo from app_setting";
                    //adapter.Fill(Ds.Tables["DataTable2"]);

                    if (Ds.Tables["DataTable1"].Rows.Count > 0)
                    {
                        string URL = null;
                        Session[SessionVars.dtReportDataSet.ToString()] = Ds;

                        URL = "ReportPopup.aspx?Report=NoCashWithdrawals.rpt&ReportTitle=No Cash Withdrawals Report&FromDate=" + Utility.GetDateTime(TextBox_FromDate.Text) + "&ToDate=" + Utility.GetDateTime(TextBox_ToDate.Text) + "&GeneratedBy=" + objUser.UserLogin + "&total=" + Ds.Tables["DataTable1"].Rows.Count;
                        Literal_ok.Text = "<script>window.open(\"" + URL + "\");</script>";
                    }
                    else
                        Literal_ok.Text = "<li>No record(s) found</li>";

                }
                else
                    Literal_ok.Text = "<li>No record(s) found</li>";

            }

            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_errorMsg.Text = "<li> An error occurred while generating Report::" + ex.Message;
            }
            finally
            {
                if (cmd != null)
                    if (cmd.Connection != null)
                        cmd.Connection.Close();

                task.EndTask();
            }


        }
        
    }
}