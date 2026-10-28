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

namespace CCMSUI
{
    public partial class DenominationUtilizationAnalysis : BasePage
    {

        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        }
        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            string regionID = (string)Session[SessionVars.SelectedRegions.ToString()];
            if (regionID != null /*&& !regionID.Contains(",")*/
                && regionID != "1")
            {
                DropDownList_NoteSetType.Items.Clear();
                Avanza.CCMS.DAL.NoteSetType.NoteSetTypeReader reader =
                Avanza.CCMS.DAL.NoteSetType.ExecuteReader("region_id=" + AddEditATM.GetOrganization(regionID.Contains(",") ? int.Parse(regionID.Substring(0, regionID.IndexOf(","))) : int.Parse(regionID)));
                while (reader.Read())
                {
                    DropDownList_NoteSetType.Items.Add(new ListItem(reader.CurrentNoteSetType.NoteSetTypeName, reader.CurrentNoteSetType.NoteSetTypeId.ToString()));
                }
                reader.Close();
                
            }
            else
                Literal_Message.Text = "<span style='color:red'><b>Please select only one organization to do denomination analysis</b></span>";

        }

        protected void Button_Submit_Click(object sender, EventArgs e)
        {
            if (DropDownList_NoteSetType.Items.Count > 0)
            {
                this.GridView1.DataSource = this.DrawGraph();
                this.GridView1.DataBind();
                Literal_Message.Text = "";
            }
            else
            {
                Literal_Message.Text = "<span style='color:red'><b>Note set type is not selected</b></span>";
            }
        }


        private void BindNoteSetDropDownList()
        {
            NoteSetType.NoteSetTypeReader reader = NoteSetType.ExecuteReader("region_id in (" + Session["org"] + ")");
            while (reader.Read())
                DropDownList_NoteSetType.Items.Add(new ListItem(reader.CurrentNoteSetType.NoteSetTypeName, reader.CurrentNoteSetType.NoteSetTypeId.ToString()));
            reader.Close();
        }

        private DataTable DrawGraph()
        {
            List<double> listNotes1DispensedPerc = new List<double>();
            List<double> listNotes2DispensedPerc = new List<double>();
            List<double> listNotes3DispensedPerc = new List<double>();
            List<double> listNotes4DispensedPerc = new List<double>();
            int notesDispensed1 = 0;
            int notesDispensed2 = 0;
            int notesDispensed3 = 0;
            int notesDispensed4 = 0;
            double notesDispensed1Perc = 0.0;
            double notesDispensed2Perc = 0.0;
            double notesDispensed3Perc = 0.0;
            double notesDispensed4Perc = 0.0;
            int totalNotesDispensed = 0;

            NoteSetType noteSetType = NoteSetType.LoadNoteSetTypeByPk(int.Parse(DropDownList_NoteSetType.SelectedItem.Value));
            string[] parts = { noteSetType.DenominationType1Title.Length==0?"Undefined1":noteSetType.DenominationType1Title,
                               noteSetType.DenominationType2Title.Length==0?"Undefined2":noteSetType.DenominationType2Title,
                               noteSetType.DenominationType3Title.Length==0?"Undefined3":noteSetType.DenominationType3Title,
                               noteSetType.DenominationType4Title.Length==0?"Undefined4":noteSetType.DenominationType4Title,
                             };
                //ConfigurationManager.AppSettings["ConfiguredCassettesDenomination"].Split(new char[] { ',' });

            this.GridView1.Columns[1].HeaderText = parts[0];
            this.GridView1.Columns[2].HeaderText = parts[1];
            this.GridView1.Columns[3].HeaderText = parts[2];
            this.GridView1.Columns[4].HeaderText = parts[3];
            DataTable dt = this.GetDataTable("");
            this.Chart1.Titles[0].Text = "Denomination Utilization";
            this.Chart1.ChartAreas[0].AxisY.Title = "Utilization %";
            this.Chart1.ChartAreas[0].AxisX.Title = "Dates";
            for (int j = 0; j < dt.Rows.Count; j++)
            {
                notesDispensed1 = int.Parse(dt.Rows[j][1].ToString());
                notesDispensed2 = int.Parse(dt.Rows[j][2].ToString());
                notesDispensed3 = int.Parse(dt.Rows[j][3].ToString());
                notesDispensed4 = int.Parse(dt.Rows[j][4].ToString());
                totalNotesDispensed = ((notesDispensed1 + notesDispensed2) + notesDispensed3) + notesDispensed4;
                if (totalNotesDispensed != 0)
                {
                    notesDispensed1Perc = Math.Round((double)((((double)notesDispensed1) / ((double)totalNotesDispensed)) * 100.0));
                }
                else
                {
                    notesDispensed1Perc = -1.0;
                }
                if (totalNotesDispensed != 0)
                {
                    notesDispensed2Perc = Math.Round((double)((((double)notesDispensed2) / ((double)totalNotesDispensed)) * 100.0));
                }
                else
                {
                    notesDispensed2Perc = -1.0;
                }
                if (totalNotesDispensed != 0)
                {
                    notesDispensed3Perc = Math.Round((double)((((double)notesDispensed3) / ((double)totalNotesDispensed)) * 100.0));
                }
                else
                {
                    notesDispensed3Perc = -1.0;
                }
                if (totalNotesDispensed != 0)
                {
                    notesDispensed4Perc = Math.Round((double)((((double)notesDispensed4) / ((double)totalNotesDispensed)) * 100.0));
                }
                else
                {
                    notesDispensed4Perc = -1.0;
                }
                if (notesDispensed1Perc > -1.0)
                {
                    this.Chart1.Series[0].Points.AddXY(DateTime.ParseExact(dt.Rows[j][0].ToString(), "dd/MM/yyyy", null).ToString("dd/MM/yy"), new object[] { notesDispensed1Perc });
                    listNotes1DispensedPerc.Add(notesDispensed1Perc);
                }
                if (notesDispensed2Perc > -1.0)
                {
                    this.Chart1.Series[1].Points.AddXY(DateTime.ParseExact(dt.Rows[j][0].ToString(), "dd/MM/yyyy", null).ToString("dd/MM/yy"), new object[] { notesDispensed2Perc });
                    listNotes2DispensedPerc.Add(notesDispensed2Perc);
                }
                if (notesDispensed3Perc > -1.0)
                {
                    this.Chart1.Series[2].Points.AddXY(DateTime.ParseExact(dt.Rows[j][0].ToString(), "dd/MM/yyyy", null).ToString("dd/MM/yy"), new object[] { notesDispensed3Perc });
                    listNotes3DispensedPerc.Add(notesDispensed3Perc);
                }
                if (notesDispensed4Perc > -1.0)
                {
                    this.Chart1.Series[3].Points.AddXY(DateTime.ParseExact(dt.Rows[j][0].ToString(), "dd/MM/yyyy", null).ToString("dd/MM/yy"), new object[] { notesDispensed4Perc });
                    listNotes4DispensedPerc.Add(notesDispensed4Perc);
                }
            }
            this.Chart1.Legends[0].Enabled = true;
            this.Chart1.Series[0].Name = parts[0];
            this.Chart1.Series[1].Name = parts[1];
            this.Chart1.Series[2].Name = parts[2];
            this.Chart1.Series[3].Name = parts[3];
            float piePiece1 = 0f;
            float piePiece2 = 0f;
            float piePiece3 = 0f;
            float piePiece4 = 0f;
            double var = 0.0;
            int l = 0;
            l = 0;
            while (l < listNotes1DispensedPerc.Count)
            {
                var += listNotes1DispensedPerc[l];
                l++;
            }
            if (l != 0)
            {
                piePiece1 = (float)Math.Round((double)(var / ((double)l)));
            }
            var = 0.0;
            l = 0;
            while (l < listNotes2DispensedPerc.Count)
            {
                var += listNotes2DispensedPerc[l];
                l++;
            }
            if (l != 0)
            {
                piePiece2 = (float)Math.Round((double)(var / ((double)l)));
            }
            var = 0.0;
            l = 0;
            while (l < listNotes3DispensedPerc.Count)
            {
                var += listNotes3DispensedPerc[l];
                l++;
            }
            if (l != 0)
            {
                piePiece3 = (float)Math.Round((double)(var / ((double)l)));
            }
            var = 0.0;
            l = 0;
            while (l < listNotes4DispensedPerc.Count)
            {
                var += listNotes4DispensedPerc[l];
                l++;
            }
            if (l != 0)
            {
                piePiece4 = (float)Math.Round((double)(var / ((double)l)));
            }
            this.Chart2.Series["Default"].Points.AddXY(parts[0], new object[] { piePiece1 });
            this.Chart2.Series["Default"].Points.AddXY(parts[1], new object[] { piePiece2 });
            this.Chart2.Series["Default"].Points.AddXY(parts[2], new object[] { piePiece3 });
            this.Chart2.Series["Default"].Points.AddXY(parts[3], new object[] { piePiece4 });
            this.Chart3.Series["Default"].Points.AddXY(parts[0], new object[] { piePiece1 });
            this.Chart3.Series["Default"].Points.AddXY(parts[1], new object[] { piePiece2 });
            this.Chart3.Series["Default"].Points.AddXY(parts[2], new object[] { piePiece3 });
            this.Chart3.Series["Default"].Points.AddXY(parts[3], new object[] { piePiece4 });
            this.Chart2.Series["Default"].ChartType = SeriesChartType.Pie;
            this.Chart2.Series["Default"]["PieLabelStyle"] = "Outside";
            this.Chart2.Titles[0].Text = "Overall Denomination Analysis";
            this.Chart2.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = true;
            this.Chart2.Series[0]["PieDrawingStyle"] = "SoftEdge";
            this.Chart2.Legends[0].Enabled = true;
            this.Chart3.Titles[0].Text = "Overall Denomination Analysis";
            this.Chart3.Series["Default"]["ShowMarkerLines"] = "true";
            this.Chart3.Series["Default"]["DrawingStyle"] = "Cylinder";
            this.Chart3.Series["Default"]["PointWidth"] = "0.8";
            return dt;
        }

        private DataTable GetDataTable(string orderby)
        {
            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            cmd.CommandText = @"select CONVERT(varchar,trxn_datetime,103) trxn_datetime ,sum(cash_dispensed1) notesDispensed1,sum(cash_dispensed2) notesDispensed2,
                                sum(cash_dispensed3) notesDispensed3,sum(cash_dispensed4) notesDispensed4
                                from vParsedTransaction inner join atm on vParsedTransaction.atm_id = atm.atm_id 
                                and atm.atm_id IN (" + Session[SessionVars.SelectedATMs.ToString()] + @")
                                and atm.note_set_type_id = " + DropDownList_NoteSetType.SelectedItem.Value + 
                                " where trxn_datetime>=convert(datetime,'" + this.TextBox_From.Text + "',103)  " +
                                " and trxn_datetime<=convert(datetime,'" + this.TextBox_To.Text + " 23:59:59',103) "+
                                "group by CONVERT(varchar,trxn_datetime,103)  order by " + ((orderby.Length > 0) ? orderby : "trxn_datetime");
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }

        protected void GridView1_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (this.ViewState["SortExpression"].ToString().IndexOf(e.SortExpression) >= 0)
            {
                if (this.ViewState["sortDirection"].ToString() == " asc")
                {
                    this.ViewState["sortDirection"] = " desc";
                }
                else
                {
                    this.ViewState["sortDirection"] = " asc";
                }
            }
            else
            {
                this.ViewState["SortExpression"] = e.SortExpression;
                this.ViewState["sortDirection"] = " asc";
            }
            this.ViewState["orderby"] = this.ViewState["SortExpression"] + " " + this.ViewState["sortDirection"];
            this.GridView1.DataSource = this.GetDataTable((string)this.ViewState["orderby"]);
            this.GridView1.DataBind();
            this.DrawGraph();
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
            }
        }
    }


}