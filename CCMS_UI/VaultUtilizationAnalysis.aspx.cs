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
using Avanza.iSuite.DAL;
using System.Data.SqlClient;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using System.Web.UI.DataVisualization.Charting;
using System.Text;

namespace CCMSUI
{
    public partial class VaultUtilizationAnalysis : BasePage
    {

        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
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
                    Label_Org.Text = region.RegionName;
                    Session[SessionVars.SelectedRegions.ToString()] = region.RegionId.ToString();
                    PopulateCIT(region.RegionId);

                }

            }
            else
                Label_Org.Text = "Please select Organization";

        }
        private void PopulateCIT(int orgID)
        {
            DropDownList_CIT.Items.Clear();
            DropDownList_CIT.Items.Add("Select");
            RegionCit.RegionCitReader reader = RegionCit.ExecuteReader("region_id = " + orgID);
            while (reader.Read())
            {
                CcmsCit ccmsCIT = CcmsCit.LoadCcmsCitByPk(reader.CurrentRegionCit.CitId);
                if (ccmsCIT != null)
                    DropDownList_CIT.Items.Add(new ListItem(ccmsCIT.Name, ccmsCIT.Id.ToString()));
            }
            reader.Close();
        }
        protected void DropDownList_Vault_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }
        protected void DropDownList_CIT_Change(object sender, EventArgs e)
        {
            DropDownList_Vault.Items.Clear();
            DropDownList_Vault.Items.Add("Select");
            CcmsVault.CcmsVaultReader reader = CcmsVault.ExecuteReader("cit_id = " + long.Parse(DropDownList_CIT.SelectedItem.Value) + "  and is_deleted=0");
            while (reader.Read())
            {
                DropDownList_Vault.Items.Add(new ListItem(reader.CurrentCcmsVault.Title, reader.CurrentCcmsVault.Id.ToString()));
            }
            reader.Close();
        }


        protected void Button_Submit_Click(object sender, EventArgs e)
        {
            if (DropDownList_Vault.Items.Count > 0)
            {

                decimal totalOrdered=0;
                SqlCommand cmd=ConnectionFactory.GetNewCommand(false);
                cmd.CommandText = @"select title,current_order_received_at,current_order_suggested_amount-(current_order_suggested_amount-current_ordeR_remaining_amount) total
                                    from cash_ordeR_monitoring inner join atm on cash_ordeR_monitoring.atm_id = atm.atm_id 
                                    where atm.is_active =1 and current_order_received_at>=convert(datetime,'" +TextBox_From.Text+"',103)"+
                " and current_order_received_at<=convert(datetime,'"+TextBox_To.Text+" 23:59:59',103) " +
                " and atm.atm_id in (select atm_id from vault_atm where vault_id = " + DropDownList_Vault.SelectedItem.Value + ")";

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                foreach (DataRow dr in dt.Rows)
                    totalOrdered += totalOrdered + decimal.Parse(dr["total"].ToString());

                decimal currentBal =decimal.Parse(ConnectionFactory.ExecuteScalar(@"
                select isnull(sum(total_value),0)
                from CCMS_vault_day_balance outerVaultBalance inner join CCMS_vault on CCMS_vault.id = outerVaultBalance.vault_id 
                where ledger_date in (select max(ledger_date) from CCMS_vault_day_balance
                where vault_transaction_Type = outerVaultBalance.vault_transaction_type
                and vault_transaction_Type = 'Total'
                and vault_id = outerVaultBalance.vault_id)
                and outerVaultBalance.vault_id = " + DropDownList_Vault.SelectedItem.Value).ToString());

                decimal utilization = Math.Round((totalOrdered / currentBal) * 100);

                this.ChartVaultUtil.Series[0].Points.AddXY("Utilizated", utilization);
                this.ChartVaultUtil.Series[0].Points.AddXY("Not Utilizated", 100-utilization);
                this.ChartVaultUtil.Series["Default"].ChartType = SeriesChartType.Pie;
                this.ChartVaultUtil.Series["Default"]["PieLabelStyle"] = "Outside";
                this.ChartVaultUtil.Titles[0].Text = "Vault Utilization Analysis";
                this.ChartVaultUtil.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = true;
                this.ChartVaultUtil.Series[0]["PieDrawingStyle"] = "SoftEdge";
                this.ChartVaultUtil.Legends[0].Enabled = true;

                    

                //StringBuilder builder = new StringBuilder();
                //VaultAtm.VaultAtmReader vaultAtmsReader = VaultAtm.ExecuteReader("vault_id = " + DropDownList_Vault.SelectedItem.Value);
                //while (vaultAtmsReader.Read())
                //{
                        
                //}
                //vaultAtmsReader.Close();


                this.GridView_VaultATMs.DataSource = dt;
                this.GridView_VaultATMs.DataBind();
                Literal_Message.Text = "";
            }
            else
            {
                Literal_Message.Text = "<span style='color:red'><b>Vault is not selected</b></span>";
            }
        }


       

        //private DataTable DrawGraph()
        //{
        //    List<double> listNotes1DispensedPerc = new List<double>();
        //    List<double> listNotes2DispensedPerc = new List<double>();
        //    List<double> listNotes3DispensedPerc = new List<double>();
        //    List<double> listNotes4DispensedPerc = new List<double>();
        //    int notesDispensed1 = 0;
        //    int notesDispensed2 = 0;
        //    int notesDispensed3 = 0;
        //    int notesDispensed4 = 0;
        //    double notesDispensed1Perc = 0.0;
        //    double notesDispensed2Perc = 0.0;
        //    double notesDispensed3Perc = 0.0;
        //    double notesDispensed4Perc = 0.0;
        //    int totalNotesDispensed = 0;

        //    NoteSetType noteSetType = NoteSetType.LoadNoteSetTypeByPk(int.Parse(DropDownList_NoteSetType.SelectedItem.Value));
        //    string[] parts = { noteSetType.DenominationType1Title.Length==0?"Undefined1":noteSetType.DenominationType1Title,
        //                       noteSetType.DenominationType2Title.Length==0?"Undefined2":noteSetType.DenominationType2Title,
        //                       noteSetType.DenominationType3Title.Length==0?"Undefined3":noteSetType.DenominationType3Title,
        //                       noteSetType.DenominationType4Title.Length==0?"Undefined4":noteSetType.DenominationType4Title,
        //                     };
        //        //ConfigurationManager.AppSettings["ConfiguredCassettesDenomination"].Split(new char[] { ',' });

        //    this.GridView1.Columns[1].HeaderText = parts[0];
        //    this.GridView1.Columns[2].HeaderText = parts[1];
        //    this.GridView1.Columns[3].HeaderText = parts[2];
        //    this.GridView1.Columns[4].HeaderText = parts[3];
        //    DataTable dt = this.GetDataTable("");
        //    this.Chart1.Titles[0].Text = "Denomination Utilization";
        //    this.Chart1.ChartAreas[0].AxisY.Title = "Utilization %";
        //    this.Chart1.ChartAreas[0].AxisX.Title = "Dates";
        //    for (int j = 0; j < dt.Rows.Count; j++)
        //    {
        //        notesDispensed1 = int.Parse(dt.Rows[j][1].ToString());
        //        notesDispensed2 = int.Parse(dt.Rows[j][2].ToString());
        //        notesDispensed3 = int.Parse(dt.Rows[j][3].ToString());
        //        notesDispensed4 = int.Parse(dt.Rows[j][4].ToString());
        //        totalNotesDispensed = ((notesDispensed1 + notesDispensed2) + notesDispensed3) + notesDispensed4;
        //        if (totalNotesDispensed != 0)
        //        {
        //            notesDispensed1Perc = Math.Round((double)((((double)notesDispensed1) / ((double)totalNotesDispensed)) * 100.0));
        //        }
        //        else
        //        {
        //            notesDispensed1Perc = -1.0;
        //        }
        //        if (totalNotesDispensed != 0)
        //        {
        //            notesDispensed2Perc = Math.Round((double)((((double)notesDispensed2) / ((double)totalNotesDispensed)) * 100.0));
        //        }
        //        else
        //        {
        //            notesDispensed2Perc = -1.0;
        //        }
        //        if (totalNotesDispensed != 0)
        //        {
        //            notesDispensed3Perc = Math.Round((double)((((double)notesDispensed3) / ((double)totalNotesDispensed)) * 100.0));
        //        }
        //        else
        //        {
        //            notesDispensed3Perc = -1.0;
        //        }
        //        if (totalNotesDispensed != 0)
        //        {
        //            notesDispensed4Perc = Math.Round((double)((((double)notesDispensed4) / ((double)totalNotesDispensed)) * 100.0));
        //        }
        //        else
        //        {
        //            notesDispensed4Perc = -1.0;
        //        }
        //        if (notesDispensed1Perc > -1.0)
        //        {
        //            this.Chart1.Series[0].Points.AddXY(DateTime.ParseExact(dt.Rows[j][0].ToString(), "dd/MM/yyyy", null).ToString("dd/MM/yy"), new object[] { notesDispensed1Perc });
        //            listNotes1DispensedPerc.Add(notesDispensed1Perc);
        //        }
        //        if (notesDispensed2Perc > -1.0)
        //        {
        //            this.Chart1.Series[1].Points.AddXY(DateTime.ParseExact(dt.Rows[j][0].ToString(), "dd/MM/yyyy", null).ToString("dd/MM/yy"), new object[] { notesDispensed2Perc });
        //            listNotes2DispensedPerc.Add(notesDispensed2Perc);
        //        }
        //        if (notesDispensed3Perc > -1.0)
        //        {
        //            this.Chart1.Series[2].Points.AddXY(DateTime.ParseExact(dt.Rows[j][0].ToString(), "dd/MM/yyyy", null).ToString("dd/MM/yy"), new object[] { notesDispensed3Perc });
        //            listNotes3DispensedPerc.Add(notesDispensed3Perc);
        //        }
        //        if (notesDispensed4Perc > -1.0)
        //        {
        //            this.Chart1.Series[3].Points.AddXY(DateTime.ParseExact(dt.Rows[j][0].ToString(), "dd/MM/yyyy", null).ToString("dd/MM/yy"), new object[] { notesDispensed4Perc });
        //            listNotes4DispensedPerc.Add(notesDispensed4Perc);
        //        }
        //    }
        //    this.Chart1.Legends[0].Enabled = true;
        //    this.Chart1.Series[0].Name = parts[0];
        //    this.Chart1.Series[1].Name = parts[1];
        //    this.Chart1.Series[2].Name = parts[2];
        //    this.Chart1.Series[3].Name = parts[3];
        //    float piePiece1 = 0f;
        //    float piePiece2 = 0f;
        //    float piePiece3 = 0f;
        //    float piePiece4 = 0f;
        //    double var = 0.0;
        //    int l = 0;
        //    l = 0;
        //    while (l < listNotes1DispensedPerc.Count)
        //    {
        //        var += listNotes1DispensedPerc[l];
        //        l++;
        //    }
        //    if (l != 0)
        //    {
        //        piePiece1 = (float)Math.Round((double)(var / ((double)l)));
        //    }
        //    var = 0.0;
        //    l = 0;
        //    while (l < listNotes2DispensedPerc.Count)
        //    {
        //        var += listNotes2DispensedPerc[l];
        //        l++;
        //    }
        //    if (l != 0)
        //    {
        //        piePiece2 = (float)Math.Round((double)(var / ((double)l)));
        //    }
        //    var = 0.0;
        //    l = 0;
        //    while (l < listNotes3DispensedPerc.Count)
        //    {
        //        var += listNotes3DispensedPerc[l];
        //        l++;
        //    }
        //    if (l != 0)
        //    {
        //        piePiece3 = (float)Math.Round((double)(var / ((double)l)));
        //    }
        //    var = 0.0;
        //    l = 0;
        //    while (l < listNotes4DispensedPerc.Count)
        //    {
        //        var += listNotes4DispensedPerc[l];
        //        l++;
        //    }
        //    if (l != 0)
        //    {
        //        piePiece4 = (float)Math.Round((double)(var / ((double)l)));
        //    }
        //    this.Chart2.Series["Default"].Points.AddXY(parts[0], new object[] { piePiece1 });
        //    this.Chart2.Series["Default"].Points.AddXY(parts[1], new object[] { piePiece2 });
        //    this.Chart2.Series["Default"].Points.AddXY(parts[2], new object[] { piePiece3 });
        //    this.Chart2.Series["Default"].Points.AddXY(parts[3], new object[] { piePiece4 });
        //    this.Chart3.Series["Default"].Points.AddXY(parts[0], new object[] { piePiece1 });
        //    this.Chart3.Series["Default"].Points.AddXY(parts[1], new object[] { piePiece2 });
        //    this.Chart3.Series["Default"].Points.AddXY(parts[2], new object[] { piePiece3 });
        //    this.Chart3.Series["Default"].Points.AddXY(parts[3], new object[] { piePiece4 });
        //    this.Chart2.Series["Default"].ChartType = SeriesChartType.Pie;
        //    this.Chart2.Series["Default"]["PieLabelStyle"] = "Outside";
        //    this.Chart2.Titles[0].Text = "Overall Denomination Analysis";
        //    this.Chart2.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = true;
        //    this.Chart2.Series[0]["PieDrawingStyle"] = "SoftEdge";
        //    this.Chart2.Legends[0].Enabled = true;
        //    this.Chart3.Titles[0].Text = "Overall Denomination Analysis";
        //    this.Chart3.Series["Default"]["ShowMarkerLines"] = "true";
        //    this.Chart3.Series["Default"]["DrawingStyle"] = "Cylinder";
        //    this.Chart3.Series["Default"]["PointWidth"] = "0.8";
        //    return dt;
        //}

//        private DataTable GetDataTable(string orderby)
//        {
//            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
//            cmd.CommandText = @"select CONVERT(varchar,trxn_datetime,103) trxn_datetime ,sum(cash_dispensed1) notesDispensed1,sum(cash_dispensed2) notesDispensed2,
//                                sum(cash_dispensed3) notesDispensed3,sum(cash_dispensed4) notesDispensed4
//                                from parsed_transaction inner join atm on parsed_transaction.atm_id = atm.atm_id 
//                                and atm.note_set_type_id = "+DropDownList_NoteSetType.SelectedItem.Value + 
//                                " where trxn_datetime>=convert(datetime,'" + this.TextBox_From.Text + "',103)  " +
//                                " and trxn_datetime<=convert(datetime,'" + this.TextBox_To.Text + " 23:59:59',103) "+
//                                "group by CONVERT(varchar,trxn_datetime,103)  order by " + ((orderby.Length > 0) ? orderby : "trxn_datetime");
//            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
//            DataTable dt = new DataTable();
//            adapter.Fill(dt);
//            return dt;
//        }

        //protected void GridView1_Sorting(object sender, GridViewSortEventArgs e)
        //{
        //    if (this.ViewState["SortExpression"].ToString().IndexOf(e.SortExpression) >= 0)
        //    {
        //        if (this.ViewState["sortDirection"].ToString() == " asc")
        //        {
        //            this.ViewState["sortDirection"] = " desc";
        //        }
        //        else
        //        {
        //            this.ViewState["sortDirection"] = " asc";
        //        }
        //    }
        //    else
        //    {
        //        this.ViewState["SortExpression"] = e.SortExpression;
        //        this.ViewState["sortDirection"] = " asc";
        //    }
        //    this.ViewState["orderby"] = this.ViewState["SortExpression"] + " " + this.ViewState["sortDirection"];
        //    this.GridView1.DataSource = this.GetDataTable((string)this.ViewState["orderby"]);
        //    this.GridView1.DataBind();
        //    this.DrawGraph();
        //}

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