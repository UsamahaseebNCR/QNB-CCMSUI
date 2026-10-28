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
using Avanza.CCMS.DAL;

using System.Data.SqlClient;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using System.Web.UI.DataVisualization.Charting;
using System.Text;
using System.Globalization;

namespace CCMSUI
{
    public partial class PerformanceDashboard : BasePage
    {
        //string[] colors = { "#BF4b4f", "#527fb8", "#8766a5", "#9ab353" };
        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        }

        protected void GridView_CITPerfTable_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.CssClass = "HeadingCell";
                e.Row.Cells[0].BackColor = System.Drawing.Color.Aqua;
            }
        }

        private void PopulateCIT(int orgID)
        {
            //DropDownList_RepPerfCIT.Items.Clear();
            RegionCit.RegionCitReader reader = RegionCit.ExecuteReader("region_id = " + orgID);
            while (reader.Read())
            {
                CcmsCit ccmsCIT = CcmsCit.LoadCcmsCitByPk(reader.CurrentRegionCit.CitId);
                //if (ccmsCIT != null)
                //{
                //    DropDownList_RepPerfCIT.Items.Add(new ListItem(ccmsCIT.Name, ccmsCIT.Id.ToString()));
                //}
            }
            reader.Close();
        }

        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            string regionID = Session[SessionVars.SelectedRegions.ToString()].ToString();
            if (regionID != null /*&& !regionID.Contains(",")*/
                && !regionID.StartsWith("1,"))
            {

                Region region = Region.LoadRegionByPk(regionID.Contains(",") ? int.Parse(regionID.Substring(0, regionID.IndexOf(","))) : int.Parse(regionID));
                if (region.IsOrganization)
                {
                    //Label_Org.Text = region.RegionName;
                    Session[SessionVars.SelectedRegions.ToString()] = region.RegionId.ToString();
                    PopulateCIT(region.RegionId);
                }
            }
           // else
             //   Label_Org.Text = "Please select Organization";
        }

//        private DataTable GetVaultSummary()
//        {
//            DataRow newOrExisingRow = null;
//            bool isNew = false;
//            DataTable dt = new DataTable();
//            List<int> vaultIds = new List<int>();
//            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
//            dt.Columns.Add("Type");
//            CcmsVault.CcmsVaultReader vaultReader = CcmsVault.ExecuteReader("is_deleted=0 and organization_id = (select region_id from region where region_name='" + Label_Org.Text + "')");
//            while (vaultReader.Read())
//            {
//                vaultIds.Add(vaultReader.CurrentCcmsVault.Id);
//                dt.Columns.Add(vaultReader.CurrentCcmsVault.Title);
//            }
//            vaultReader.Close();

//            for (int v = 0; v < vaultIds.Count; v++)
//            {
//                cmd.CommandText = @"
//                select denomination_name,total_quantity
//                from CCMS_vault_day_balance outerVaultBalance inner join CCMS_vault on CCMS_vault.id = outerVaultBalance.vault_id 
//                where   ledger_date in (select max(ledger_date) from CCMS_vault_day_balance
//                where vault_transaction_Type = outerVaultBalance.vault_transaction_type
//                and vault_transaction_Type = 'Total' 
//                and vault_id = outerVaultBalance.vault_id and ledger_date>=convert(datetime,'" + TextBox_From.Text + "',103) " +
//                                        "and ledger_date <= convert(datetime,'" + TextBox_To.Text + " 23:59:59',103))" +
//                " and outerVaultBalance.vault_id=" + vaultIds[v] + " and vault_transaction_Type = 'Total'" +
//                "and vault_id = outerVaultBalance.vault_id and ledger_date>=convert(datetime,'" + TextBox_From.Text + "',103) " +
//                                        "and ledger_date <= convert(datetime,'" + TextBox_To.Text + " 23:59:59',103)";
//                DataTable temp = new DataTable();
//                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
//                adapter.Fill(temp);
//                foreach (DataRow dr in temp.Rows)
//                {
//                    DataRow[] dataRow = dt.Select("Type='" + dr["denomination_name"] + "'");
//                    if (dataRow.Length == 0)
//                    {
//                        newOrExisingRow = dt.NewRow();
//                        newOrExisingRow["Type"] = dr["denomination_name"];
//                        isNew = true;
//                    }
//                    else
//                    {
//                        newOrExisingRow = dataRow[0];
//                        isNew = false;
//                    }
//                    newOrExisingRow[v + 1] = dr["total_quantity"];
//                    if (isNew)
//                    {
//                        dt.Rows.Add(newOrExisingRow);
//                        isNew = false;
//                    }
//                }
//            }
//            return dt;
//        }

        private void DrawCashUtilGraph()
        {
            double util = 0;
            double totalReplenished = 0;
            double totalReturned = 0;
            int counter = 0;
            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
//            cmd.CommandText = @"select sum(replenishment_amount) repAmount,sum(return_amount) retAmount,trxn_datetime
//
//                                from vSummary inner join atm on vSummary.atm_id = atm.atm_id 
//                                where trxn_datetime >= convert(datetime,'" + TextBox_From.Text + "',103) " +
//                                "and trxn_datetime <= convert(datetime,'" + TextBox_To.Text + " 23:59:59',103)  " +
//                                " and atm.IS_ACTIVE =1 and atm.atm_id in (select atm.atm_id " +
//                                " from ccms_vault inner join vault_atm on ccms_vault.id = vault_atm.vault_id " +
//                                " inner join atm on vault_atm.atm_id = atm.atm_id  " +
//                                " where ccms_vault.cit_id in ( " +
//                                " select cit_id from region_cit where region_id =" + "(select region_id from region where region_name='" + Label_Org.Text + "') " +
//                                " and atm.is_active=1 and ATM.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ")) and replenishment_amount > 0  " +
//                                ") group by trxn_datetime order by trxn_datetime";
                


            cmd.CommandText = @"select sum(replenishment_amount) repAmount,sum(return_amount) retAmount,trxn_datetime

                                from vSummary inner join atm on vSummary.atm_id = atm.atm_id 
                                where trxn_datetime >= convert(datetime,'" + TextBox_From.Text + "',103) " +
                             "and trxn_datetime <= convert(datetime,'" + TextBox_To.Text + " 23:59:59',103)  " +
                             " and atm.IS_ACTIVE =1 and atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and replenishment_amount > 0  " +
                             " group by trxn_datetime order by trxn_datetime";

            
            DataTable dt = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);

            double replenishedAmount = 0;
            double returnedAmount = 0;
            for (int x = 0; x < dt.Rows.Count - 1; x++)
            {
                replenishedAmount = double.Parse(dt.Rows[x]["repAmount"].ToString());
                returnedAmount = x + 1 < dt.Rows.Count ? double.Parse(dt.Rows[x + 1]["retAmount"].ToString()) : 0;
                totalReplenished += replenishedAmount;
                totalReturned += returnedAmount;
                int i = ChartCashUtil.Series.Count;
                ChartCashUtil.Series["Utilization"].Points.AddXY(dt.Rows[x]["trxn_datetime"], Math.Round((replenishedAmount - returnedAmount) / replenishedAmount * 100, 2));
            }

            Label_CashUtil.Text = Math.Round((totalReplenished - totalReturned) / totalReplenished * 100, 2).ToString();
            ChartCashUtil.Titles[0].Text += ":" + Label_CashUtil.Text + "%";
        }

        private void DrawCitRepPerfGraph()
        {
            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            StringBuilder builderAtmIds = null;
            //List<int> vaultIds = null;

            //vaultIds = new List<int>();
            //CcmsVault.CcmsVaultReader vaultReader = CcmsVault.ExecuteReader("cit_id =" + DropDownList_RepPerfCIT.SelectedItem.Value);
            //while (vaultReader.Read())
            //{
            //    vaultIds.Add(vaultReader.CurrentCcmsVault.Id);
            //} vaultReader.Close();

            //foreach (int vaultID in vaultIds)
            //{
            //    builderAtmIds = new StringBuilder();
            //    VaultAtm.VaultAtmReader vaultAtmReader = VaultAtm.ExecuteReader("vault_id = " + vaultID);
            //    while (vaultAtmReader.Read())
            //    {
            //        builderAtmIds.Append(vaultAtmReader.CurrentVaultAtm.AtmId + ",");
            //    }
            //    builderAtmIds.Append("-1");
            //    vaultAtmReader.Close();
            //}
            cmd.CommandText = @"select rep_datetime, convert(varchar,rep_datetime,103) rep_Datetime,cash_order_datetime, 
                                        cash_added1*note_set_type.denomination_type_1+cash_added2*denomination_type_2+cash_added3*denomination_type_3+cash_added4*denomination_type_4+cash_added5*denomination_type_5 
                                         +cash_added6*denomination_type_6+cash_added7*denomination_type_7 replenished,
                                        cassette1_suggested_notes*note_set_type.denomination_type_1+ 
                                        cassette2_suggested_notes*note_set_type.denomination_type_2+ 
                                        cassette3_suggested_notes*note_set_type.denomination_type_3+ 
                                        cassette4_suggested_notes*note_set_type.denomination_type_4 suggested, dispatch_time
                                        from vReplenishment 
                                        inner join vCashOrders on vCashOrders.cash_order_id = vReplenishment.cash_order_id
                                        inner join atm on vReplenishment.atm_id = atm.atm_id 
                                        inner join note_set_type on atm.note_set_type_id = note_set_type.note_set_type_id 
                                        inner join region on atm.region_id = region.region_id 
                                         where rep_datetime >= convert(datetime,'" + TextBox_From.Text + "',103) " +
                                    "and rep_datetime <= convert(datetime,'" + TextBox_To.Text + " 23:59:59',103)  " +
                                        " and atm.IS_ACTIVE =1 and atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ")";

            DataTable dt = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);

            DataTable dtRepDates = dt.DefaultView.ToTable(true, new string[] { "rep_Datetime" });
            int correct = 0;
            int timeExceptions = 0;
            int amountExceptions = 0;
            int total = 0;
            foreach (DataRow dr in dtRepDates.Rows)
            {
                correct = 0;
                timeExceptions = 0;
                amountExceptions = 0;
                DataRow[] dataRows = dt.Select("rep_datetime='" + dr[0] + "'");
                foreach (DataRow drInner in dataRows)
                {
                    if (DateTime.ParseExact(drInner["dispatch_time"].ToString(), "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)
                            >= DateTime.ParseExact(drInner["rep_datetime"].ToString(), "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)
                        && drInner["replenished"].ToString() == drInner["suggested"].ToString())
                        correct++;
                    else
                    {
                        if (DateTime.ParseExact(drInner["dispatch_time"].ToString(), "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)
                            < DateTime.ParseExact(drInner["rep_datetime"].ToString(), "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture))
                            timeExceptions++;
                        if (drInner["replenished"].ToString() != drInner["suggested"].ToString())
                            amountExceptions++;
                    }
                }
                total += (timeExceptions + amountExceptions);

                ChartRepPerf.Series["Correct"].Points.AddXY(dr[0], correct);
                if (correct < 5)
                    ChartRepPerf.Series["Correct"].Points[ChartRepPerf.Series["Correct"].Points.Count - 1].BorderColor = System.Drawing.ColorTranslator.FromHtml("#9ab353");
                else if (correct >= 10 && correct <= 15)
                    ChartRepPerf.Series["Correct"].Points[ChartRepPerf.Series["Correct"].Points.Count - 1].BorderColor = System.Drawing.ColorTranslator.FromHtml("#8766a5");
                else if (correct >= 5 && correct < 10)
                    ChartRepPerf.Series["Correct"].Points[ChartRepPerf.Series["Correct"].Points.Count - 1].BorderColor = System.Drawing.ColorTranslator.FromHtml("#527fb8");
                else if (correct > 15)
                    ChartRepPerf.Series["Correct"].Points[ChartRepPerf.Series["Correct"].Points.Count - 1].BorderColor = System.Drawing.ColorTranslator.FromHtml("#BF4b4f");


                ChartRepPerf.Series["Time Exceptions"].Points.AddXY(dr[0], timeExceptions);

                if (timeExceptions < 5)
                    ChartRepPerf.Series["Time Exceptions"].Points[ChartRepPerf.Series["Time Exceptions"].Points.Count - 1].BorderColor = System.Drawing.ColorTranslator.FromHtml("#9ab353");
                else if (timeExceptions >= 10 && timeExceptions <= 15)
                    ChartRepPerf.Series["Time Exceptions"].Points[ChartRepPerf.Series["Time Exceptions"].Points.Count - 1].BorderColor = System.Drawing.ColorTranslator.FromHtml("#8766a5");
                else if (timeExceptions >= 5 && timeExceptions < 10)
                    ChartRepPerf.Series["Time Exceptions"].Points[ChartRepPerf.Series["Time Exceptions"].Points.Count - 1].BorderColor = System.Drawing.ColorTranslator.FromHtml("#527fb8");
                else if (timeExceptions > 15)
                    ChartRepPerf.Series["Time Exceptions"].Points[ChartRepPerf.Series["Time Exceptions"].Points.Count - 1].BorderColor = System.Drawing.ColorTranslator.FromHtml("#BF4b4f");

                ChartRepPerf.Series["Amount Exceptions"].Points.AddXY(dr[0], amountExceptions);
                if (amountExceptions < 5)
                    ChartRepPerf.Series["Amount Exceptions"].Points[ChartRepPerf.Series["Amount Exceptions"].Points.Count - 1].BorderColor = System.Drawing.ColorTranslator.FromHtml("#9ab353");
                else if (amountExceptions >= 10 && amountExceptions <= 15)
                    ChartRepPerf.Series["Amount Exceptions"].Points[ChartRepPerf.Series["Amount Exceptions"].Points.Count - 1].BorderColor = System.Drawing.ColorTranslator.FromHtml("#8766a5");
                else if (amountExceptions >= 5 && amountExceptions < 10)
                    ChartRepPerf.Series["Amount Exceptions"].Points[ChartRepPerf.Series["Amount Exceptions"].Points.Count - 1].BorderColor = System.Drawing.ColorTranslator.FromHtml("#527fb8");
                else if (amountExceptions > 15)
                    ChartRepPerf.Series["Amount Exceptions"].Points[ChartRepPerf.Series["Amount Exceptions"].Points.Count - 1].BorderColor = System.Drawing.ColorTranslator.FromHtml("#BF4b4f");
            }
            ChartRepPerf.Titles[0].Text += ":" + total;
            Label_TotalExceptions.Text = total.ToString();
        }

        private void DrawOutOfCashGraph()
        {
            int totalOutOfCash = 0;
            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            cmd.CommandText = @"select count(id),convert(varchar,generated_at,103) generated_at,region_name

from vCcmsIntegratedAlert inner join atm on vCcmsIntegratedAlert.entity_id = atm.atm_id 
inner join region on atm.region_id = region.region_id 

                                         where alert_type_id =21 and generated_at >= convert(datetime,'" + TextBox_From.Text + "',103) " +
                                    "and generated_at <= convert(datetime,'" + TextBox_To.Text + " 23:59:59',103)  " +
                                        " and atm.IS_ACTIVE =1 and atm.atm_id in ("+Session[SessionVars.SelectedATMs.ToString()]+")"+
                            " group by convert(varchar,generated_at,103),region_name order by convert(varchar,generated_at,103)";

            DataTable dt = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);

            DataTable dtRegions = dt.DefaultView.ToTable(true, new string[] { "region_name" });
            foreach (DataRow dr in dtRegions.Rows)
            {
                ChartOutOfCash.Series.Add(dr[0].ToString());
                ChartOutOfCash.Series[ChartOutOfCash.Series.Count - 1].ChartType = SeriesChartType.Bar;
                ChartOutOfCash.Series[ChartOutOfCash.Series.Count - 1].BorderWidth = 1;
                ChartOutOfCash.Series[ChartOutOfCash.Series.Count - 1].MarkerSize = 8;
                ChartOutOfCash.Series[ChartOutOfCash.Series.Count - 1].MarkerStyle = MarkerStyle.Circle;
            }

            DataTable dtGeneratedAt = dt.DefaultView.ToTable(true, new string[] { "generated_at" });

            foreach (DataRow dr in dtGeneratedAt.Rows)
            {

                DataRow[] dataRows = dt.Select("generated_at='" + dr[0] + "'");
                foreach (DataRow drInner in dataRows)
                {
                    ChartOutOfCash.Series[drInner["region_name"].ToString()].Points.AddXY(dr[0], drInner[0]);
                    totalOutOfCash += int.Parse(drInner[0].ToString());
                }
            }
            for (int i = 0; i < ChartOutOfCash.Series.Count; i++)
                ChartOutOfCash.Series[i]["StackedGroupName"] = "Group1";
            label_totalOutOfIncidents.Text = totalOutOfCash.ToString();
            ChartOutOfCash.Titles[0].Text = ChartOutOfCash.Titles[0].Text.Replace("{0}", totalOutOfCash.ToString());
        }

//        private DataTable GetCitTotalVisits()
//        {

//            int totalExceptions = 0;
//            DataTable dt = new DataTable();
//            List<int> citIds = new List<int>();
//            StringBuilder builderAtmIds = null;
//            List<int> vaultIds = null;
//            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
//            dt.Columns.Add("Type");
//            CcmsCit.CcmsCitReader ccmsCitReader = CcmsCit.ExecuteReader("id in (select cit_id from ccms_vault where organization_id = (select region_id from region where region_name='" + Label_Org.Text + "'))");
//            while (ccmsCitReader.Read())
//            {
//                citIds.Add(ccmsCitReader.CurrentCcmsCit.Id);
//                dt.Columns.Add(ccmsCitReader.CurrentCcmsCit.Name);
//            }
//            ccmsCitReader.Close();

//            DataRow dr = dt.NewRow();
//            dr[0] = "Cash Replenishments";
//            DataRow drSettlements = dt.NewRow();
//            drSettlements[0] = "No. Of Settlements";
//            DataRow drExceptions = dt.NewRow();
//            drExceptions[0] = "Cash Exceptions";
//            for (int j = 0; j < citIds.Count; j++)
//            {
//                vaultIds = new List<int>();
//                CcmsVault.CcmsVaultReader vaultReader = CcmsVault.ExecuteReader("cit_id =" + citIds[j]);
//                while (vaultReader.Read())
//                {
//                    vaultIds.Add(vaultReader.CurrentCcmsVault.Id);
//                } vaultReader.Close();

//                foreach (int vaultID in vaultIds)
//                {
//                    builderAtmIds = new StringBuilder();
//                    VaultAtm.VaultAtmReader vaultAtmReader = VaultAtm.ExecuteReader("vault_id = " + vaultID);
//                    while (vaultAtmReader.Read())
//                    {
//                        builderAtmIds.Append(vaultAtmReader.CurrentVaultAtm.AtmId + ",");
//                    }
//                    builderAtmIds.Append("-1");
//                    vaultAtmReader.Close();
//                }
//                dr[j + 1] = ConnectionFactory.ExecuteScalar(string.Format(@"select count(replenishment_id) from vReplenishment where atm_id in ({0}) 
//                                    and rep_datetime>=convert(datetime,'{1}',103) and rep_datetime<=convert(datetime,'{2} 23:59:59',103)", builderAtmIds.ToString(), TextBox_From.Text, TextBox_To.Text));

//                drSettlements[j + 1] = ConnectionFactory.ExecuteScalar(string.Format(@"select count(atm_settlement_id) from atm_settlement inner join atm on atm_settlement.atm_no = atm.cit_atm_title  where atm_id in ({0}) 
//                                        and rep_datetime>=convert(datetime,'{1}',103) and rep_datetime<=convert(datetime,'{2} 23:59:59',103)", builderAtmIds.ToString(), TextBox_From.Text, TextBox_To.Text));

//                drExceptions[j + 1] = ConnectionFactory.ExecuteScalar(@"select count(replenishment_id)
//                                        from vReplenishment 
//                                        inner join atm on vReplenishment.atm_id = atm.atm_id 
//                                        inner join note_set_type on atm.note_set_type_id = note_set_type.note_set_type_id 
//                                        inner join region on atm.region_id = region.region_id 
//                                       inner join vCashOrders on vCashOrders.atm_id = atm.atm_id
//                                        where rep_datetime >= convert(datetime,'" + TextBox_From.Text + "',103) " +
//                                        "and rep_datetime <= convert(datetime,'" + TextBox_To.Text + " 23:59:59',103)  " +
//                                        " and atm.IS_ACTIVE =1 and atm.atm_id in (" + builderAtmIds.ToString() + ") and (convert(varchar,rep_datetime,103) <> convert(varchar,cash_order_datetime,103) " +
//                                        " or " +
//                                        " cash_added1*note_set_type.denomination_type_1+cash_added2*denomination_type_2+cash_added3*denomination_type_3+cash_added4*denomination_type_4+cash_added5*denomination_type_5 " +
//                                        " +cash_added6*denomination_type_6+cash_added7*denomination_type_7 " +
//                                         "<> " +
//                                         " cassette1_suggested_notes*note_set_type.denomination_type_1+ " +
//                                         " cassette2_suggested_notes*note_set_type.denomination_type_2+ " +
//                                        " cassette3_suggested_notes*note_set_type.denomination_type_3+ " +
//                                        " cassette4_suggested_notes*note_set_type.denomination_type_4)");

//                totalExceptions += int.Parse(drExceptions[j + 1].ToString());
//            }
//            dt.Rows.Add(dr);
//            dt.Rows.Add(drSettlements);
//            dt.Rows.Add(drExceptions);
//            return dt;
//        }
        protected void Button_Submit_Click(object sender, EventArgs e)
        {
            //if (!Label_Org.Text.ToLower().Contains("select"))
            //{
                DrawCitRepPerfGraph();
                DrawOutOfCashGraph();
                DrawCashUtilGraph();
            //}
            //else
            //{
            //    Literal_Message.Text = "<span style='color:red'><b>Organization is not selected</b></span>";
            //}
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!base.IsPostBack)
            {
                this.TextBox_To.Text = DateTime.Now.ToString("dd/MM/yyyy");
                this.TextBox_From.Text = DateTime.Now.AddDays(-7.0).ToString("dd/MM/yyyy");
                this.ViewState["SortExpression"] = " trxn_datetime";
                this.ViewState["sortDirection"] = " desc";
                this.ViewState["orderby"] = this.ViewState["SortExpression"] + " " + this.ViewState["sortDirection"];
                refresh_ServerClick(this, null);
            }
        }
    }
}