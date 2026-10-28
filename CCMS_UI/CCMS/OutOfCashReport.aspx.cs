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
using System.Text;

using System.Data.SqlClient;
using Avanza.CCMS.DAL;
using System.Text.RegularExpressions;
using System.Web.Configuration;

namespace CCMSUI.CCMS
{
    public partial class OutOfCashReport : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Page_Load");
            try
            {
                if (!Page.IsPostBack)
                {
                    if (!CheckRight((int)Permissions.OutOfCashReport))
                        Response.Redirect("../admin/AccessDenied.aspx");

                    TextBox_fromdate.Text = DateTime.Today.ToString("dd/MM/yyyy");
                    TextBox_toDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
                
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
            StringBuilder selectQuery = new StringBuilder();

            if ((TextBox_fromdate.Text.Length > 0))
                selectQuery.Append(@" and last_trxn_at >= Convert(datetime,'" + TextBox_fromdate.Text + " 00:00:00',103) ");

            if ((TextBox_toDate.Text.Length > 0))
                selectQuery.Append(@" and last_trxn_at <=Convert(datetime,'" + TextBox_toDate.Text + " 23:59:59',103)  ");


            string ATMIds = Convert.ToString(Session[SessionVars.SelectedATMs.ToString()]);
            selectQuery.Append(@" and outerATM.atm_id in (" + ATMIds + ") ");

            if (WebConfigurationManager.AppSettings["isDeadATMExcluded"] == "1")
                selectQuery.Append(" and outerATM.atm_id in (select ATM_id from heart_beat where heart_beat_received_at >=convert(datetime,'" + TextBox_fromdate.Text + "',103) and " +
                                     "heart_beat_received_at <=convert(datetime,'" + TextBox_toDate.Text + " 23:59:59',103))");
        

            string query = selectQuery.ToString();
            
            return query;
      
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Search");
            try
            {
                bool result = ValidateInputs();
                if (result == false)
                {
                    return;
                }
                else
                {
                    AppUser objUser = (AppUser)Session[SessionVars.user.ToString()];
                    SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                    cmd.CommandTimeout = 100;
                    cmd.CommandText = @"
select region_name, parent_region_id,region.region_id, bank_logo, outerATM.title title, note_set_type.denomination_type_1, note_set_type.denomination_type_2, note_set_type.denomination_type_3,
                                    note_set_type.denomination_type_4, note_set_type.denomination_type_5, 
                                    note_set_type.denomination_type_6, note_set_type.denomination_type_7, 
                                    p.last_trxn_at, p.cassette1_notes, p.cassette2_notes,
                                    p.cassette3_notes, p.cassette4_notes, p.cassette5_notes, p.cassette6_notes,
                                    p.cassette7_notes, task.last_invoked ,
note_set_type.denomination_type_1 * p.cassette1_notes + 
note_set_type.denomination_type_2 * p.cassette2_notes + 
note_set_type.denomination_type_3 * p.cassette3_notes + 
note_set_type.denomination_type_4 * p.cassette4_notes + 
note_set_type.denomination_type_5 * p.cassette5_notes + 
note_set_type.denomination_type_6 * p.cassette6_notes + 
note_set_type.denomination_type_7 * p.cassette7_notes total 
 from atm outerATM , cash_position p ,task,note_set_type,region 
where outerATM.atm_id = p.atm_id and outerATM.is_active = 1 and outerATM.is_atm=1 and outerATM.region_id = region.region_id 
and p.task_id = task.task_id  and outerATM.note_set_type_id = note_set_type.note_set_type_id  " + buildSearchCriteria() +
    " and note_set_type.denomination_type_1 * p.cassette1_notes + " +
    " note_set_type.denomination_type_2 * p.cassette2_notes + " +
    " note_set_type.denomination_type_3 * p.cassette3_notes + " +
    " note_set_type.denomination_type_4 * p.cassette4_notes + " +
    " note_set_type.denomination_type_5 * p.cassette5_notes + " +
    " note_set_type.denomination_type_6 * p.cassette6_notes + " +
    " note_set_type.denomination_type_7 * p.cassette7_notes = " + TextBox_Threshold.Text;


                    dsOutOfCashReport Ds = new CCMSUI.CCMS.dsOutOfCashReport();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(Ds.Tables["DataTable1"]);
                    //adapter.SelectCommand.CommandText = "select bank_logo from app_setting";
                    //adapter.Fill(Ds.Tables[1]);

                    PopulateDataTable(Ds.Tables["DataTable1"]);

                    /*    AppSetting objappsetting = (AppSetting)Application[ApplicationVars.AppSettings.ToString()];

                        SqlCommand new_cmd = ConnectionFactory.GetNewCommand(true);
                        new_cmd.CommandText = "select den_value from denomination where den_name in ('dispense_type1','dispense_type2','dispense_type3','dispense_type4')";
                        SqlDataAdapter adap = new SqlDataAdapter(new_cmd);
                        DataTable dt = new DataTable();
                        adap.Fill(dt);*/

                    if (Ds.Tables[0].Rows.Count > 0)
                    {
                        Session[SessionVars.dtReportDataSet.ToString()] = Ds;
                        string URL = "ReportPopup.aspx?Report=OutOfCash.rpt&ReportTitle=Out Of Cash Report&FromDate=" + Utility.GetDateTime(TextBox_fromdate.Text) + "&ToDate=" + Utility.GetDateTime(TextBox_toDate.Text) + "&GeneratedBy=" + objUser.UserLogin + "&total=" + Ds.Tables[0].Rows.Count;

                        //  Literal_ok.Text = "<script> try { window.open(\"" + URL + "\"); } catch { alert('Popup cannot be opened'); }</script>";
                        Literal_ok.Text = "<script>window.open(\"" + URL + "\");</script>";
                    }
                    else
                        Literal_ok.Text = "<li>No record(s) found</li>";
                }
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_errorMsg.Text = "<li> An error occurred while generating Report::" + ex.Message;
            }
            finally
            {
                task.EndTask();
            }
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
                region_id = int.Parse(dt.Rows[i][4].ToString()); //parent

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
                dt.Rows[i][2] = region.BankLogo;

            }

        }

        public bool ValidateInputs()
        {
            bool result = true;
            bool isThresholdContainSpecialChars = CheckSpecialChars(TextBox_Threshold.Text);
            if (isThresholdContainSpecialChars == true)
            {
                Literal_errorMsg.Text = "<img src='../images/icon_err.gif'>&nbsp;Name contain invalid character</img> </li>";
                return result = false;
            }
            return result;
        }
     
    }

}