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

namespace CCMSUI.CCMS
{
    public partial class DashBoardGrid_Deposits : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                if (!CheckRight((int)Permissions.ViewDepositDashboard, true))
                    Response.Redirect("../../admin/aspx/AccessDenied.aspx");

                ViewState["SortExpression"] = "title ";
                ViewState["sortDirection"] = "asc ";
                BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
                if (System.Web.Configuration.WebConfigurationManager.AppSettings["IsDisplayDepositDenomination"] == "true")
                    GridView_Deposit.Columns[4].Visible = GridView_Deposit.Columns[6].Visible = GridView_Deposit.Columns[8].Visible = GridView_Deposit.Columns[10].Visible = GridView_Deposit.Columns[12].Visible = true;
                else
                    GridView_Deposit.Columns[4].Visible = GridView_Deposit.Columns[6].Visible = GridView_Deposit.Columns[8].Visible = GridView_Deposit.Columns[10].Visible = GridView_Deposit.Columns[12].Visible = false;
             
            }

        }


        private void BindGrid(string orderBy)
        {
            ///<Summary>Edited by Ali Shah on 09th Oct, 2019
            ///To dynamically handling of cassettes more than 4
            //EnableGridRespectiveColumns();

            SqlCommand cmd = null;
            LogableTask task = LogableTask.NewTask("BindGrid");
            try
            {


                metaRefresh.Content = (((AppSetting)Application[ApplicationVars.AppSettings.ToString()]).DashboardRefreshInterval * 60).ToString();
                Page.RegisterStartupScript("script", "<script>function init() {cd(\"" + (int.Parse(metaRefresh.Content) / 60) + "\");} init();</script>");

                cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;
                cmd.CommandText = @"select * , (select max(counts_cleared_at) from bna_counts_cleared where atm_id = outerATM.atm_id) last_bna_cleared_at, 
                                    (select max(counts_cleared_at) from cpm_counts_cleared where atm_id = outerATM.atm_id)last_cpm_cleared_at,'' total
                                    from deposit_position, atm outerATM, region 
                                    where outerATM.region_id = region.region_id 
                                    and outerATM.atm_id = deposit_position.atm_id 
                                    and outerATM.is_active =1 " + 
                                    (Request.QueryString["n"] != null ? " and outerATM.note_set_type_id = " + Request.QueryString["n"] : "") +
                                    " order by " + orderBy;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dtDeposit = new DataTable();
                adapter.Fill(dtDeposit);

                ///<Summary>Edited by Ali Faiyaz Shah on 1st June, 2020
                ///To calculate Amount field from denomination details
                SetDepositsAmount(dtDeposit);


                //DataTable dt1 = new DataTable();
                //dt1.Columns.Add("generated_at", typeof(DateTime));
                //dt1.Columns.Add("title", typeof(string));
                //dt1.Columns.Add("ip", typeof(string));
                //dt1.Columns.Add("region_name", typeof(string));


                //DataTable dt2 = new DataTable();
                //dt2 = dt1.Clone();
                //for (int i = 0; i < dtAgentMonitoring.Rows.Count / 2; i++)
                //{
                //    DataRow dr = dt1.NewRow();
                //    dr[0] = dtAgentMonitoring.Rows[i][0];
                //    dr[1] = dtAgentMonitoring.Rows[i][1];
                //    dr[2] = dtAgentMonitoring.Rows[i][2];
                //    dr[3] = dtAgentMonitoring.Rows[i][3];
                //    dt1.Rows.Add(dr);

                //}
                //for (int i = dtAgentMonitoring.Rows.Count / 2; i < dtAgentMonitoring.Rows.Count; i++)
                //{
                //    DataRow dr = dt2.NewRow();
                //    dr[0] = dtAgentMonitoring.Rows[i][0];
                //    dr[1] = dtAgentMonitoring.Rows[i][1];
                //    dr[2] = dtAgentMonitoring.Rows[i][2];
                //    dr[3] = dtAgentMonitoring.Rows[i][3];
                //    dt2.Rows.Add(dr);

                //}


                GridView_Deposit.DataSource = dtDeposit;
                //GridView2.DataSource = dt2;
                GridView_Deposit.DataBind();
                //GridView2.DataBind();


            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex + " , Ex Message  :" +ex.Message);
                Literal_errorMsg.Text = "<li>Unable to generate dashboard</li>";
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

        

        protected void GridView_Deposit_Sorting(object sender, GridViewSortEventArgs e)
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
        protected void GridView_Deposit_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Image img = (Image)e.Row.FindControl("img");
                Image img1 = (Image)e.Row.FindControl("img1");
                string atm_id = this.GridView_Deposit.DataKeys[e.Row.DataItemIndex].Values["atm_id"].ToString();
                Atm atm = Atm.LoadAtmByPk(int.Parse(atm_id));
                bool isThresholdOfAnyCassetteReached = false;
                bool isThresholdOfAnyCPMReached = false;

                if (atm.CdmCassette1Threshold != null)
                {
                    if (e.Row.Cells[3].Text != "&nbsp;")
                    {
                        if (int.Parse(e.Row.Cells[3].Text) >= atm.CdmCassette1Threshold)
                            img.Attributes.Add("src", "../../images/attention_soon.jpg");
                        if (int.Parse(e.Row.Cells[3].Text) >= 2000)
                            img.Attributes.Add("src", "../../images/attention_now.jpg");
                    }
                        isThresholdOfAnyCassetteReached = true;

                }
                if (atm.CdmCassette2Threshold != null)
                {
                    if (e.Row.Cells[5].Text != "&nbsp;")
                    {
                        if (int.Parse(e.Row.Cells[5].Text) >= atm.CdmCassette2Threshold)
                            img.Attributes.Add("src", "../../images/attention_soon.jpg");
                        if (int.Parse(e.Row.Cells[5].Text) >= 2000)
                            img.Attributes.Add("src", "../../images/attention_now.jpg");
                    }
                    isThresholdOfAnyCassetteReached = true;
                }
                if (atm.CdmCassette3Threshold != null)
                {
                    if (e.Row.Cells[7].Text != "&nbsp;")
                    {
                        if (int.Parse(e.Row.Cells[7].Text) >= atm.CdmCassette3Threshold)
                            img.Attributes.Add("src", "../../images/attention_soon.jpg");
                        if (int.Parse(e.Row.Cells[7].Text) >= 2000)
                            img.Attributes.Add("src", "../../images/attention_now.jpg");
                    }
                    isThresholdOfAnyCassetteReached = true;
                }
                if (atm.CdmCassette4Threshold != null)
                {
                    if (e.Row.Cells[9].Text != "&nbsp;")
                    {

                        if (int.Parse(e.Row.Cells[9].Text) >= atm.CdmCassette4Threshold)
                            img.Attributes.Add("src", "../../images/attention_soon.jpg");
                        if (int.Parse(e.Row.Cells[9].Text) >= 2000)
                            img.Attributes.Add("src", "../../images/attention_now.jpg");
                    }
                    isThresholdOfAnyCassetteReached = true;
                }
                if (!isThresholdOfAnyCassetteReached)
                {
                    if (atm.CdmCassette1Threshold == null && atm.CdmCassette2Threshold == null &&
                        atm.CdmCassette3Threshold == null && atm.CdmCassette4Threshold == null)
                    {
                        img.Attributes.Add("src", "../../images/help.jpg");
                        
                    }
                }

                ///<Summary>
                //Edited by Ali Faiyaz Shah on 24th June, 2020
                //As it was picking wrong index. It was searching on 16th column index while 'CPM Pkt 1' was on 18th column index
                int indexCCDM = 0;
                for (int i = 0; i < GridView_Deposit.Columns.Count; i++)
                {
                    if (GridView_Deposit.Columns[i].HeaderText == "CPM Pkt 1")
                    { indexCCDM = i; break; }

                }
                if (atm.CcdmCassette1Threshold != null)
                {
                    

                    if (e.Row.Cells[indexCCDM].Text != "&nbsp;")
                    {

                        if (int.Parse(e.Row.Cells[indexCCDM].Text) >= atm.CcdmCassette1Threshold)
                            img1.Attributes.Add("src1", "../../images/attention_soon.jpg");
                        if (int.Parse(e.Row.Cells[indexCCDM].Text) >= 2000)
                            img1.Attributes.Add("src1", "../../images/attention_now.jpg");
                    }
                    isThresholdOfAnyCPMReached = true;

                }
                
                if (atm.CcdmCassette2Threshold != null)
                {
                    if (e.Row.Cells[indexCCDM+1].Text != "&nbsp;")
                    {

                        if (int.Parse(e.Row.Cells[indexCCDM+1].Text) >= atm.CcdmCassette2Threshold)
                            img1.Attributes.Add("src1", "../../images/attention_soon.jpg");
                        if (int.Parse(e.Row.Cells[indexCCDM+1].Text) >= 2000)
                            img1.Attributes.Add("src1", "../../images/attention_now.jpg");
                    }
                    isThresholdOfAnyCPMReached = true;
                }
                if (atm.CcdmCassette3Threshold != null)
                {
                    if (e.Row.Cells[indexCCDM+2].Text != "&nbsp;")
                    {
                        if (int.Parse(e.Row.Cells[indexCCDM+2].Text) >= atm.CcdmCassette3Threshold)
                            img1.Attributes.Add("src1", "../../images/attention_soon.jpg");
                        if (int.Parse(e.Row.Cells[indexCCDM+2].Text) >= 2000)
                            img1.Attributes.Add("src1", "../../images/attention_now.jpg");
                    }
                    isThresholdOfAnyCPMReached = true;
                }
                if (atm.CcdmCassette4Threshold != null)
                {
                    if (e.Row.Cells[indexCCDM+3].Text != "&nbsp;")
                    {
                        if (int.Parse(e.Row.Cells[indexCCDM+3].Text) >= atm.CcdmCassette4Threshold)
                            img1.Attributes.Add("src1", "../../images/attention_soon.jpg");
                        if (int.Parse(e.Row.Cells[indexCCDM+3].Text) >= 2000)
                            img1.Attributes.Add("src1", "../../images/attention_now.jpg");
                    }
                    isThresholdOfAnyCPMReached = true;
                }
                if (!isThresholdOfAnyCPMReached)
                {
                    if (atm.CcdmCassette1Threshold == null && atm.CcdmCassette2Threshold == null &&
                        atm.CcdmCassette3Threshold == null && atm.CcdmCassette4Threshold == null)
                    {
                        img1.Attributes.Add("src", "../../images/help.jpg");
                        
                    }


                }





            }

        }

        private void SetDepositsAmount(DataTable pDataTable)
        {
            for(int i=0; i < pDataTable.Rows.Count; i++)
            {
                pDataTable.Rows[i]["total"] = ExtractDepositAmount(pDataTable.Rows[i]["cassette1_deposit_value"].ToString(), pDataTable.Rows[i]["cassette2_deposit_value"].ToString(), pDataTable.Rows[i]["cassette3_deposit_value"].ToString(), pDataTable.Rows[i]["cassette4_deposit_value"].ToString());
            }
        }
        //Edited by Muhammad Shaharyar for QIB
        private string ExtractDepositAmount(string pCassette1Detail, string pCassette2Detail, string pCassette3Detail, string pCassette4Detail)
        {

            int[] cassette1 = !string.IsNullOrEmpty(pCassette1Detail) ? ParseCassetteDetail(pCassette1Detail) : new int[0];
            int[] cassette2 = !string.IsNullOrEmpty(pCassette2Detail) ? ParseCassetteDetail(pCassette2Detail) : new int[0];
            int[] cassette3 = !string.IsNullOrEmpty(pCassette3Detail) ? ParseCassetteDetail(pCassette3Detail) : new int[0];
            int[] cassette4 = !string.IsNullOrEmpty(pCassette4Detail) ? ParseCassetteDetail(pCassette4Detail) : new int[0];
            int cassette1total = !string.IsNullOrEmpty(pCassette1Detail) ? ParseCassettetotal(pCassette1Detail) : 0;
            int cassette2total = !string.IsNullOrEmpty(pCassette2Detail) ? ParseCassettetotal(pCassette2Detail) : 0;
            int cassette3total = !string.IsNullOrEmpty(pCassette3Detail) ? ParseCassettetotal(pCassette3Detail) : 0;
            int cassette4total = !string.IsNullOrEmpty(pCassette4Detail) ? ParseCassettetotal(pCassette4Detail) : 0;
            int[] cassette = ParseCassette(pCassette1Detail);

            int[] cassetteDetailTotal = new int[cassette1.Length];
            for(int i=0;i<cassette1.Length;i++)
            {
                cassetteDetailTotal[i] = cassette1[i] + cassette2[i] + cassette3[i] + cassette4[i];
            }

            int cassettetotal = cassette1total + cassette2total + cassette3total + cassette4total;

            string data = null ;

            if (System.Web.Configuration.WebConfigurationManager.AppSettings["IsDisplayDepositDenomination"] == "true")
            {

                for (int i = 0; i < cassetteDetailTotal.Length - 1; i++)
                {
                    data += cassette[i].ToString() + "*" + cassetteDetailTotal[i].ToString() + "<br>";
                }

                data += "=" + cassettetotal.ToString();
            }
            else
            {
                data = cassettetotal.ToString();
            }

            return data;


            
        }
        //Added by Muhammad Shaharyar for QIB
        private int[] ParseCassetteDetail(string pCassetteDetail)
        {
            
            string[] cassetteDetails = null;
            string[] seperator = {"<br>"}; 
            cassetteDetails = pCassetteDetail.Split(seperator, StringSplitOptions.RemoveEmptyEntries);
            int[] cassettecount = new int[cassetteDetails.Length];
            for (int i=0; i < cassetteDetails.Length-1; i++ )
            {

                    string[] temp = cassetteDetails[i].Split('*');
                    cassettecount[i] = int.Parse(temp[1]);
            }

            return cassettecount;
        }
        //Added by Muhammad Shaharyar for QIB
        private int ParseCassettetotal(string pCassetteDetail)
        {
            int cassetteAmount=0;
            string[] cassetteDetails = null;

            if (pCassetteDetail.Contains("="))
            {
                cassetteDetails = pCassetteDetail.Split('=');
                cassetteAmount = int.Parse(cassetteDetails[1].Trim());
            }


            return cassetteAmount;
        }

        //Added by Muhammad Shaharyar for QIB
        private int[] ParseCassette(string pCassetteDetail)
        {

            string[] cassetteDetails = null;
            string[] seperator = { "<br>" };
            cassetteDetails = pCassetteDetail.Split(seperator, StringSplitOptions.RemoveEmptyEntries);
            int[] cassettecount = new int[cassetteDetails.Length];
            for (int i = 0; i < cassetteDetails.Length - 1; i++)
            {

                string[] temp = cassetteDetails[i].Split('*');
                cassettecount[i] = int.Parse(temp[0]);
            }

            return cassettecount;
        } 

        private void EnableGridRespectiveColumns()
        {
            //Enable when Deposit Denomination Detail columns need to display(QIB)
            bool isDisplayDepositDenomination = ConfigurationManager.AppSettings["IsDisplayDepositDenomination"] != null ? bool.Parse(ConfigurationManager.AppSettings["IsDisplayDepositDenomination"]) : false;

            if (isDisplayDepositDenomination)
            {
                int columnIndexDepositDenomination = -1;
                int columnIndexPurgeBinDenomination = -1;
                columnIndexDepositDenomination = GetColumnIndexByName(this.GridView_Deposit, "Cassette1 Detail");
                columnIndexPurgeBinDenomination = GetColumnIndexByName(this.GridView_Deposit, "Reject Bin Detail");

                if (columnIndexDepositDenomination >= 0)
                {
                    this.GridView_Deposit.Columns[columnIndexDepositDenomination].Visible = true;
                    this.GridView_Deposit.Columns[columnIndexDepositDenomination + 2].Visible = true;
                    this.GridView_Deposit.Columns[columnIndexDepositDenomination + 4].Visible = true;
                    this.GridView_Deposit.Columns[columnIndexDepositDenomination + 6].Visible = true;
                }

                if(columnIndexPurgeBinDenomination >= 0)
                    this.GridView_Deposit.Columns[columnIndexPurgeBinDenomination].Visible = true;
            }

            //int columnIndexDepositDenomination = -1;
            ////int columnIndexDispensed = -1;
            //int columnIndexRemaining = -1;

            //columnIndexDepositDenomination = GetColumnIndexByName(this.GridView_CashPositions, "Cassette1 Detail");
            ////columnIndexDispensed = GetColumnIndexByName(this.GridView_CountsCleared, "Dispensed Type 5");
            //columnIndexRemaining = columnIndexDenomination + 1;// GetColumnIndexByName(this.GridView_CashPositions, "Remaining");

            ////Enable when Last Dispensed 24 Hour column need to display(ADIB)
            //if (isShowDispensed24Hour)
            //{
            //    int columnIndexDispensed24Hour = GetColumnIndexByName(this.GridView_CashPositions, "Total Dispensed in Last 24 hour");
            //    if (columnIndexDispensed24Hour >= 0)
            //        this.GridView_CashPositions.Columns[columnIndexDispensed24Hour].Visible = true;
            //}

            //for (int i = 5; i <= noOfCassettes; i++)
            //{
            //    if (columnIndexDenomination >= 0)
            //    {
            //        this.GridView_CashPositions.Columns[columnIndexDenomination].Visible = true;
            //        columnIndexDenomination += 2;
            //    }

            //    //if (columnIndexDispensed >= 0)
            //    //{
            //    //    this.GridView_CountsCleared.Columns[columnIndexDispensed].Visible = true;
            //    //    columnIndexDispensed++;
            //    //}
            //    if (columnIndexRemaining >= 0)
            //    {
            //        this.GridView_CashPositions.Columns[columnIndexRemaining].Visible = true;
            //        columnIndexRemaining += 2;
            //    }
            //}
        }

    }
}