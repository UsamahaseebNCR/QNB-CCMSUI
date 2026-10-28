using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;


namespace CCMSUI.CCMS
{
    public partial class Exceptions : System.Web.UI.Page
    {

        private void AddRow(DataTable dtGrid, string name, int counter1, int counter2)
        {
            DataRow drNewRow = dtGrid.NewRow();
            drNewRow[0] = name;
            drNewRow[1] = counter1;
            drNewRow[2] = counter2;
            drNewRow[3] = counter1 + counter2;
            dtGrid.Rows.Add(drNewRow);

        }
        //private void UpdateCounters(ref int counterEI, ref int counterEmirates, string columnName, DataTable dt)
        //{
        //    DataRow[] drEI = dt.Select(columnName+"=1");
        //    foreach (DataRow dr in drEI)
        //        counterEI += int.Parse(dr.ToString());

        //    DataRow[] drEmirates = dt.Select(columnName+"=0");
        //        counterEmirates += int.Parse(drEmirates.ToString());            
        //}
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                BindGrid();
        }

        private void BindGrid()
        {
            int isATMEI = 0;
            int isATMEmirates = 0;

            int isCDMEI = 0;
            int isCDMEmirates = 0;

            int isCCDMEI = 0;
            int isCCDMEmirates = 0;

            int isSDMEI = 0;
            int isSDMEmirates = 0;

            int isComboEI = 0;
            int isComboEmirates = 0;

            int isITMEI = 0;
            int isITMEmirates = 0;

            int isBulkCashEI = 0;
            int isBulkCashEmirates = 0;


            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            cmd.CommandText = @"select isnull(is_atm,0) is_atm,isnull(is_cdm,0) is_cdm,isnull(is_ccdm,0) is_ccdm,isnull(is_sdm,0) is_sdm,isnull(is_combo,0) is_combo,isnull(is_itm,0) is_itm
                               ,isnull(is_bulk_cash_deposit,0) is_bulk_cash_deposit,isnull(is_emirate_islamic,0) is_emirate_islamic
                               from atm  where is_active=1";

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            //UpdateCounters (isATMEI,isATMEmirates,"is_emirate_islamic"
            DataRow[] drIsAtmEI = dt.Select("is_emirate_islamic=1");
            foreach (DataRow dr in drIsAtmEI)
            {
                isATMEI += bool.Parse(dr["is_atm"].ToString()) ? 1 : 0;
                isCDMEI += bool.Parse(dr["is_cdm"].ToString()) ? 1 : 0;
                isCCDMEI += bool.Parse(dr["is_ccdm"].ToString()) ? 1 : 0;
                isSDMEI += bool.Parse(dr["is_sdm"].ToString()) ? 1 : 0;
                isComboEI += bool.Parse(dr["is_combo"].ToString()) ? 1 : 0;
                isITMEI += bool.Parse(dr["is_itm"].ToString()) ? 1 : 0;
                isBulkCashEI += bool.Parse(dr["is_bulk_cash_deposit"].ToString()) ? 1 : 0;
            }
            DataRow[] drIsAtmEmirates = dt.Select("is_emirate_islamic=0");
            foreach (DataRow dr in drIsAtmEmirates)
            {
                isATMEmirates += bool.Parse(dr["is_atm"].ToString()) ? 1 : 0;
                isCDMEmirates += bool.Parse(dr["is_cdm"].ToString()) ? 1 : 0;
                isCCDMEmirates += bool.Parse(dr["is_ccdm"].ToString()) ? 1 : 0;
                isSDMEmirates += bool.Parse(dr["is_sdm"].ToString()) ? 1 : 0;
                isComboEmirates += bool.Parse(dr["is_combo"].ToString()) ? 1 : 0;
                isITMEmirates += bool.Parse(dr["is_itm"].ToString()) ? 1 : 0;
                isBulkCashEmirates += bool.Parse(dr["is_bulk_cash_deposit"].ToString()) ? 1 : 0;
            }

            DataTable dtGrid = new DataTable();
            dtGrid.Columns.Add("function");
            dtGrid.Columns.Add("ei");
            dtGrid.Columns.Add("enbd");
            dtGrid.Columns.Add("total");

            AddRow(dtGrid, "Solo ATM", isATMEI, isATMEmirates);
            AddRow(dtGrid, "Solo CDM", isCDMEI, isCDMEmirates);
            AddRow(dtGrid, "Cheque Deposit", isCCDMEI, isCCDMEmirates);
            AddRow(dtGrid, "SDM", isSDMEI, isSDMEmirates);
            AddRow(dtGrid, "Combo", isComboEI, isComboEmirates);
            AddRow(dtGrid, "ITM", isITMEI, isITMEmirates);
            AddRow(dtGrid, "Bulk Cash Deposit", isBulkCashEI, isBulkCashEmirates);

            Chart_MachineType.Series[0].Points.AddXY("Solo ATM", isATMEI);
            Chart_MachineType.Series[1].Points.AddXY("Solo ATM", isATMEmirates);

            Chart_MachineType.Series[0].Points.AddXY("Solo CDM", isCDMEI);
            Chart_MachineType.Series[1].Points.AddXY("Solo CDM", isCDMEmirates);

            Chart_MachineType.Series[0].Points.AddXY("Cheque Deposit", isCCDMEI);
            Chart_MachineType.Series[1].Points.AddXY("Cheque Deposit", isCCDMEmirates);

            Chart_MachineType.Series[0].Points.AddXY("SDM", isSDMEI);
            Chart_MachineType.Series[1].Points.AddXY("SDM", isSDMEmirates);

            Chart_MachineType.Series[0].Points.AddXY("Combo", isComboEI);
            Chart_MachineType.Series[1].Points.AddXY("Combo", isComboEmirates);

            Chart_MachineType.Series[0].Points.AddXY("ITM", isITMEI);
            Chart_MachineType.Series[1].Points.AddXY("ITM", isITMEmirates);

            Chart_MachineType.Series[0].Points.AddXY("Bulk Cash Deposit", isBulkCashEI);
            Chart_MachineType.Series[1].Points.AddXY("Bulk Cash Deposit", isBulkCashEmirates);


            //DataRow drNewRow = dtGrid.NewRow();
            //drNewRow[0] = "Solo ATM";
            //drNewRow[1] = isATMEI;
            //drNewRow[2] = isATMEmirates;
            //drNewRow[3] = isATMEI + isATMEmirates;
            //dtGrid.Rows.Add(drNewRow);





            GridView_MachineType.DataSource = dtGrid;
            GridView_MachineType.DataBind();




            DataTable dt1 = new DataTable();
            dt1.Columns.Add("emirate");
            dt1.Columns.Add("branch");
            dt1.Columns.Add("offsite");
            dt1.Columns.Add("total");
            int offsite = 0;
            int branch = 0;

            dt = new DataTable();
            cmd.CommandText = @"select region_name,isnull(is_branch_atm,0),COUNT(atm_id)
                               from atm inner join region on atm.region_id = region.region_id where atm.is_active=1
                               group by region_name,is_branch_atm";
            adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);

            DataTable regionNames = dt.DefaultView.ToTable(true, new string[] { "region_name" });
            for (int i = 0; i < regionNames.Rows.Count; i++)
            {
                offsite = 0;
                branch = 0;
                DataRow[] drArray = dt.Select("region_name='" + regionNames.Rows[i][0] + "'");
                for (int j = 0; j < drArray.Length; j++)
                {
                    if (!bool.Parse(drArray[j][1].ToString()))
                        offsite += int.Parse(drArray[j][2].ToString());
                    else if (bool.Parse(drArray[j][1].ToString()))
                        branch += int.Parse(drArray[j][2].ToString());
                }

                DataRow dr1 = dt1.NewRow();
                dr1[0] = regionNames.Rows[i][0];
                dr1[1] = branch;
                dr1[2] = offsite;
                dr1[3] = branch + offsite;
                dt1.Rows.Add(dr1);

                Chart_EmiratesWise.Series[0].Points.AddXY(regionNames.Rows[i][0], branch);
                Chart_EmiratesWise.Series[1].Points.AddXY(regionNames.Rows[i][0], offsite);
            }



            GridView_EmiratesWise.DataSource = dt1;
            GridView_EmiratesWise.DataBind();






        }
    }
}