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

namespace CCMSUI
{
    public partial class PackageDistributionAnalytics : BasePage
    {
        string[] colors = new string[] { "#54B948", "#00A77E", "#008D9B", "#0099BF", "#ED9A22", "#F1B51C", "#E0D120", "#A6CE39" };

        protected void Button_Submit_Click(object sender, EventArgs e)
        {
            DrawGraph();
        }

        private DataTable GetDataTable(string SQL)
        {
            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            cmd.CommandText = SQL;
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }



        private string buildSearchCriteria()
        {
            StringBuilder selectQuery = new StringBuilder();
            if (DropDownList_Package.SelectedIndex>1)
            {
                selectQuery.Append(" upload.package_id = " + DropDownList_Package.SelectedItem.Value + " AND ");
            }


            if (TextBox_From.Text.Length > 0)
                selectQuery.Append(@" upload.creation_time >= Convert(datetime,'" + TextBox_From.Text + " 00:00:00',103) AND ");

            if (TextBox_To.Text.Length > 0)
                selectQuery.Append(@"  upload.creation_time <=Convert(datetime,'" + TextBox_To.Text + " 23:59:59',103) AND ");

            selectQuery.Append(@" upload.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") AND ");

            string query = selectQuery.ToString();
            query = query.Trim();

            if (query.EndsWith("AND"))
                query = query.Substring(0, query.LastIndexOf("AND"));
            if (query == "")
                return query;
            else
                return "where " + query;
        }
        private void DrawGraph()
        {

            string SQL = @"select dbo.GetStatusFriendlyName(status) status, count(*) counter from upload "+buildSearchCriteria()+" group by status";
            DataTable dt = GetDataTable(SQL);
            int total = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                this.Chart3.Series["Default"].Points.AddXY(dt.Rows[i]["status"], dt.Rows[i]["counter"]);
                total += int.Parse(dt.Rows[i]["counter"].ToString());
                Chart3.Series[0].Points[Chart3.Series[0].Points.Count - 1].Color = System.Drawing.ColorTranslator.FromHtml(colors[i % colors.Length].ToString());
   
            }

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                 this.Chart2.Series["Default"].Points.AddXY(dt.Rows[i]["status"], Math.Round((decimal)int.Parse(dt.Rows[i]["counter"].ToString())/total,2));
                 Chart2.Series[0].Points[Chart2.Series[0].Points.Count - 1].Color = System.Drawing.ColorTranslator.FromHtml(colors[i % colors.Length].ToString());
   
            }

            this.Chart2.Series["Default"].ChartType = SeriesChartType.Pie;
            this.Chart2.Series["Default"]["PieLabelStyle"] = "Outside";
            this.Chart2.Titles[0].Text = "Overall Distribution Analysis";
            this.Chart2.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = true;
            this.Chart2.Series[0]["PieDrawingStyle"] = "SoftEdge";
            this.Chart2.Legends[0].Enabled = true;
            this.Chart3.Titles[0].Text = "Analysis By Status";
            this.Chart3.Series["Default"]["ShowMarkerLines"] = "true";
            this.Chart3.Series["Default"]["DrawingStyle"] = "Cylinder";
            this.Chart3.Series["Default"]["PointWidth"] = "0.8";

            Chart3.ChartAreas[0].AxisX.Title = "Status";
            Chart3.ChartAreas[0].AxisY.Title = "Count";
            Chart3.ChartAreas[0].AxisX.IsStartedFromZero = true;
            Chart3.ChartAreas[0].AxisX.Minimum = 0;
            Chart3.ChartAreas[0].AxisY.LabelAutoFitMinFontSize = 10;
            Chart3.ChartAreas[0].AxisX.LabelAutoFitMinFontSize = 10;
            Chart3.ChartAreas[0].AxisX.TitleFont = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
            Chart3.ChartAreas[0].AxisY.TitleFont = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);


            string SQLFailureAnalysis = @"select substring(failure_reason,0,100), count(*) counter from upload "+buildSearchCriteria()+" and LEN(failure_reason)>0 group by substring(failure_reason,0,100)";
            dt = GetDataTable(SQLFailureAnalysis);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                ChartFailureAnalysis.Series["Default"].Points.AddXY(dt.Rows[i][0], dt.Rows[i]["counter"]);
                ChartFailureAnalysis.Series[0].Points[ChartFailureAnalysis.Series[0].Points.Count - 1].Color = System.Drawing.ColorTranslator.FromHtml(colors[i % colors.Length].ToString());
   
            }

            ChartFailureAnalysis.Titles[0].Text = "Failure Analysis";
            ChartFailureAnalysis.Series["Default"]["ShowMarkerLines"] = "true";
            ChartFailureAnalysis.Series["Default"]["DrawingStyle"] = "Cylinder";
            ChartFailureAnalysis.Series["Default"]["PointWidth"] = "0.8";

            ChartFailureAnalysis.ChartAreas[0].AxisX.Title = "Reason";
            ChartFailureAnalysis.ChartAreas[0].AxisY.Title = "Count";
            ChartFailureAnalysis.ChartAreas[0].AxisX.IsStartedFromZero = true;
            ChartFailureAnalysis.ChartAreas[0].AxisX.Minimum = 0;
            ChartFailureAnalysis.ChartAreas[0].AxisY.LabelAutoFitMinFontSize = 10;
            ChartFailureAnalysis.ChartAreas[0].AxisX.LabelAutoFitMinFontSize = 10;
            ChartFailureAnalysis.ChartAreas[0].AxisX.TitleFont = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
            ChartFailureAnalysis.ChartAreas[0].AxisY.TitleFont = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);


            string SQLWostATMsByExecution = @"select top 10 title, count(failure_reason) counter from upload inner join atm on atm.ATM_id = upload.atm_id "+buildSearchCriteria()+" and status ='package-executed-with-exceptions' and LEN(failure_reason)>0 group by title order by count(failure_reason) desc";
            dt = GetDataTable(SQLWostATMsByExecution);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                ChartWorstATMsByExecution.Series["Default"].Points.AddXY(dt.Rows[i][0], dt.Rows[i]["counter"]);
                ChartWorstATMsByExecution.Series[0].Points[ChartWorstATMsByExecution.Series[0].Points.Count - 1].Color = System.Drawing.ColorTranslator.FromHtml(colors[i % colors.Length].ToString());
   
            }

            ChartWorstATMsByExecution.Titles[0].Text = "Top 10 Worst ATMs By Execution";
            ChartWorstATMsByExecution.Series["Default"]["ShowMarkerLines"] = "true";
            ChartWorstATMsByExecution.Series["Default"]["DrawingStyle"] = "Cylinder";
            ChartWorstATMsByExecution.Series["Default"]["PointWidth"] = "0.8";

            ChartWorstATMsByExecution.ChartAreas[0].AxisX.Title = "ATM";
            ChartWorstATMsByExecution.ChartAreas[0].AxisY.Title = "Count";
            ChartWorstATMsByExecution.ChartAreas[0].AxisX.IsStartedFromZero = true;
            ChartWorstATMsByExecution.ChartAreas[0].AxisX.Minimum = 0;
            ChartWorstATMsByExecution.ChartAreas[0].AxisY.LabelAutoFitMinFontSize = 10;
            ChartWorstATMsByExecution.ChartAreas[0].AxisX.LabelAutoFitMinFontSize = 10;
            ChartWorstATMsByExecution.ChartAreas[0].AxisX.TitleFont = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
            ChartWorstATMsByExecution.ChartAreas[0].AxisY.TitleFont = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);



            string SQLWostATMsByNetwork = @"select top 10 title, count(failure_reason) counter from upload inner join atm on atm.ATM_id = upload.atm_id "+buildSearchCriteria()+" and status in ('uploading-failed','uploading-disconnected') and LEN(failure_reason)>0 group by title order by count(failure_reason) desc";
            dt = GetDataTable(SQLWostATMsByNetwork);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                ChartWorstATMsByNetwork.Series["Default"].Points.AddXY(dt.Rows[i][0], dt.Rows[i]["counter"]);
                ChartWorstATMsByNetwork.Series[0].Points[ChartWorstATMsByNetwork.Series[0].Points.Count - 1].Color = System.Drawing.ColorTranslator.FromHtml(colors[i%colors.Length].ToString());
                      
            }

            ChartWorstATMsByNetwork.Titles[0].Text = "Top 10 Worst ATMs By Network";
            ChartWorstATMsByNetwork.Series["Default"]["ShowMarkerLines"] = "true";
            ChartWorstATMsByNetwork.Series["Default"]["DrawingStyle"] = "Cylinder";
            ChartWorstATMsByNetwork.Series["Default"]["PointWidth"] = "0.8";

            ChartWorstATMsByNetwork.ChartAreas[0].AxisX.Title = "ATM";
            ChartWorstATMsByNetwork.ChartAreas[0].AxisY.Title = "Count";
            ChartWorstATMsByNetwork.ChartAreas[0].AxisX.IsStartedFromZero = true;
            ChartWorstATMsByNetwork.ChartAreas[0].AxisX.Minimum = 0;
            ChartWorstATMsByNetwork.ChartAreas[0].AxisY.LabelAutoFitMinFontSize = 10;
            ChartWorstATMsByNetwork.ChartAreas[0].AxisX.LabelAutoFitMinFontSize = 10;
            ChartWorstATMsByNetwork.ChartAreas[0].AxisX.TitleFont = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
            ChartWorstATMsByNetwork.ChartAreas[0].AxisY.TitleFont = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
 
        
        }

        

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
        //    //this.GridView1.DataSource = this.GetDataTable((string)this.ViewState["orderby"]);
        //    //this.GridView1.DataBind();
        //    this.DrawGraph();
        //}

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!base.IsPostBack)
            {
                this.TextBox_To.Text = DateTime.Now.ToString("dd/MM/yyyy");
                this.TextBox_From.Text = DateTime.Now.ToString("dd/MM/yyyy");
                this.ViewState["SortExpression"] = " trxn_datetime";
                this.ViewState["sortDirection"] = " desc";
                this.ViewState["orderby"] = this.ViewState["SortExpression"] + " " + this.ViewState["sortDirection"];

                DropDownList_Package.Items.Add(new ListItem("Any", "Any"));
                Package.PackageReader packageReader = Package.ExecuteReader("1=1");
                //DropDownList_Package.Items.Add(new ListItem("Any", "Any"));
                while (packageReader.Read())
                    DropDownList_Package.Items.Add(new ListItem(packageReader.CurrentPackage.PackageTitle, packageReader.CurrentPackage.PackageId.ToString()));
                packageReader.Close();

            }
        }
    }


}