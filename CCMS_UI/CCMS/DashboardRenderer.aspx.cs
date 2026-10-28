using Avanza.CCMS.DAL;
using Avanza.iSuite.DAL;
using Avanza.iSuite.UI;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.WebControls;

namespace CCMSUI.CCMS
{
    public partial class DashboardRenderer : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                try
                {
                    if (string.IsNullOrEmpty(Request.QueryString["id"]))
                        throw new Exception("No dashboard selected");
                    var groupList = new Dictionary<int, List<DashboardGraph>>();
                    int width = 750;

                    DashboardTemplate template = DashboardTemplate.LoadDashboardTemplateByPk(Convert.ToInt32(Request.QueryString["id"]));
                    Label_Title.Text = template.Title;
                    Title.Text = template.Title;
                    metaRefresh.Content = template.RefreshDurationSec.HasValue ? template.RefreshDurationSec.Value.ToString() : int.MaxValue.ToString();

                    DashboardGraph.DashboardGraphReader templateReader = DashboardGraph.ExecuteReader(
                            string.Format(@"is_active = 1 AND dashboard_id = {0} ORDER BY row_number", 
                            Convert.ToInt32(Request.QueryString["id"]))
                        );
                    while (templateReader.Read())
                    {
                        List<DashboardGraph> list = new List<DashboardGraph>();
                        if (groupList.TryGetValue(templateReader.CurrentDashboardGraph.RowNumber, out list))
                            list.Add(templateReader.CurrentDashboardGraph);
                        else
                            groupList.Add(templateReader.CurrentDashboardGraph.RowNumber, new List<DashboardGraph>() { templateReader.CurrentDashboardGraph });
                    }
                    templateReader.Close();

                    Table tb = new Table();
                    tb.ID = "tblContainer";
                    foreach (KeyValuePair<int, List<DashboardGraph>> row in groupList)
                    {
                        TableRow rowNew = new TableRow();
                        tb.Controls.Add(rowNew);

                        width = 750 / row.Value.Count;
                        foreach (DashboardGraph dashboard in row.Value)
                        {
                            TableCell cellNew = new TableCell();
                            Label lblNew = new Label();
                            rowNew.Controls.Add(cellNew);

                            System.Data.DataTable dt = new System.Data.DataTable();
                            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);

                            cmd.CommandText = dashboard.Query.Replace('"', '\'');
                            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                            adapter.Fill(dt);

                            Chart Chart1 = new Chart();
                            Chart1.Width = 380;
                            //Chart1.BackColor = System.Drawing.Color.FromArgb(255, 219, 183);
                            //Chart1.BackColor = System.Drawing.ColorTranslator.FromHtml("#ffffff");
                            Chart1.BackColor = System.Drawing.ColorTranslator.FromHtml(string.IsNullOrEmpty(dashboard.BackgroundColor) ? "#ffffff" : dashboard.BackgroundColor);
                            Chart1.BackGradientStyle = GradientStyle.TopBottom;

                            Chart1.Series.Add(new Series());
                            for (int x = 0; x < dt.Rows.Count; x++)
                            {
                                Chart1.Series[0].Points.AddXY(dt.Rows[x][dashboard.XValue], dt.Rows[x][dashboard.YValue]);
                            }

                            Chart1.BorderSkin.SkinStyle = BorderSkinStyle.Emboss;
                            Chart1.Titles.Add(dashboard.Name);
                            Chart1.Titles[0].Font = new System.Drawing.Font("Trebuchet MS", 14, FontStyle.Bold);
                            Chart1.Titles[0].ForeColor = System.Drawing.Color.FromArgb(26, 59, 105);
                            Chart1.Series[0].IsValueShownAsLabel = true;
                            Chart1.Series[0].ChartType = GetChartType(dashboard.Type);
                            Chart1.ChartAreas.Add(new ChartArea());
                            Chart1.ChartAreas[0].BorderColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
                            Chart1.ChartAreas[0].BorderDashStyle = ChartDashStyle.Solid;
                            Chart1.ChartAreas[0].BackColor = System.Drawing.Color.FromArgb(64, 165, 191, 228);
                            Chart1.ChartAreas[0].BackGradientStyle = GradientStyle.TopBottom;
                            Chart1.ChartAreas[0].BackSecondaryColor = Color.Transparent;
                            Chart1.ChartAreas[0].ShadowColor = Color.Transparent;
                            Chart1.ChartAreas[0].Area3DStyle.Enable3D = true;
                            Chart1.ChartAreas[0].Area3DStyle.WallWidth = 0;

                            Chart1.Legends.Add(new Legend());
                            Chart1.Legends[0].Enabled = dashboard.Type == "PieChart";
                            cellNew.Controls.Add(Chart1);
                        }
                    }

                    chart.Controls.Add(tb);
                }
                catch (Exception ex)
                {
                    Literal_Message.Text = ex.Message;
                }
            }
        }

        private SeriesChartType GetChartType(string charType)
        {
            switch (charType)
            {
                case "BarGraph":
                    return System.Web.UI.DataVisualization.Charting.SeriesChartType.Bar;
                case "PieChart":
                    return System.Web.UI.DataVisualization.Charting.SeriesChartType.Pie;
                case "ColumnChart":
                    return System.Web.UI.DataVisualization.Charting.SeriesChartType.Column;
                default:
                    return System.Web.UI.DataVisualization.Charting.SeriesChartType.Bar;
            }
        }
    }
}