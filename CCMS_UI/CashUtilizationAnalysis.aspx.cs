using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;

using Avanza.iSuite.UI;
using System.Text;
using System.Data;
using System.Collections;
using System.Web.UI.DataVisualization.Charting;

namespace CCMSUI
{
    public partial class CashUtilizationAnalysis : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                ViewState["SortExpression"] = "title ";
                ViewState["sortDirection"] = "asc";
                ViewState["orderby"] = ViewState["SortExpression"] + " " + ViewState["sortDirection"];

                ViewState["SortExpression1"] = "title ";
                ViewState["sortDirection1"] = "asc";
                ViewState["orderby1"] = ViewState["SortExpression1"] + " " + ViewState["sortDirection1"];

                TextBox_To.Text = TextBox4.Text = TextBox10.Text = DateTime.Now.ToString("dd/MM/yyyy");
                TextBox_From.Text = TextBox3.Text = TextBox9.Text = DateTime.Now.AddDays(-7).ToString("dd/MM/yyyy");
            }
        }

        private void GenerateGraph(string id)
        {
            string from = "";
            string to = "";

            LogableTask task = LogableTask.NewTask("DrawGraph");
            Response.CacheControl = "no-cache";
            try
            {
                if (id == "Button1")
                {
                    from = DateTime.Today.AddDays(-7).ToString("dd/MM/yyyy");
                    to = DateTime.Today.AddDays(-1).ToString("dd/MM/yyyy");
                    ChartUtilizationLastWeek.Series[0].Points.Clear();
                    ChartUtilizationLastWeek.Series[0].ChartType = SeriesChartType.Column;
                    BuildGraphData(from, to, "Utilization", "ATMUtilization", true, "167AD4", true, 0, 0, "24",ChartUtilizationLastWeek);

                    from = DateTime.Today.AddDays(-14).ToString("dd/MM/yyyy");
                    to = DateTime.Today.AddDays(-8).ToString("dd/MM/yyyy");
                    ChartUtilizationPreviousWeek.Series[0].ChartType = SeriesChartType.Column;
                    ChartUtilizationPreviousWeek.Series[0].Points.Clear();
                    BuildGraphData(from, to, "Utilization", "ATMUtilization", true, "189570", true, 0, 0, "24",ChartUtilizationPreviousWeek);

                    from = TextBox_From.Text;
                    to = TextBox_To.Text;
                    ChartUtilizationSelectedPeriod.Series[0].ChartType = SeriesChartType.Spline;
                    ChartUtilizationSelectedPeriod.Series[0].Points.Clear();
                    BuildGraphData(from, to, "Utilization", "ATMUtilization", true, "167AD4", true, 1, 0, "24", ChartUtilizationSelectedPeriod);

                    from = DateTime.Today.ToString("dd/MM/yyyy");
                    to = DateTime.Today.ToString("dd/MM/yyyy");
                    ChartUtilizationCurrentDay.Series[0].Points.Clear();
                    ChartUtilizationCurrentDay.Series[0].ChartType = SeriesChartType.Pie;
                    BuildPieTest(from, to, "Utilization", "ATMUtilization", "167AD4", "24", 0, 0, true,0);

                    //Chart1.Url = "Data.aspx?param=24@" + + "@" + + "@0@167AD4@0";
                    //Chart2.Url = "Data.aspx?param=24@" + + "@" + + "@0@189570@0";
                    //Chart3.Url = "Data.aspx?param=24@" + + "@" + + "@1@167AD4@0";
                    //Chart5.Url = "Data.aspx?param=24@" + + "@" + + "@2@167AD4@0";

                    Literal1.Text = "<b>Last Week [" + DateTime.Today.AddDays(-7).ToString("dd/MM/yyyy") + "-" + DateTime.Today.AddDays(-1).ToString("dd/MM/yyyy") + "]</b><br/>ATM Cash Utilization for last week";
                    Literal2.Text = "<b>Previous Week [" + DateTime.Today.AddDays(-14).ToString("dd/MM/yyyy") + "-" + DateTime.Today.AddDays(-8).ToString("dd/MM/yyyy") + "]</b><br/>ATM Cash Utilization for previous week"; ;
                    Literal3.Text = "<b>Selected Criteria [" + TextBox_From.Text + "-" + TextBox_To.Text + "]</b><br/>ATM Cash Utilization for specified duration";
                    Literal5.Text = "<b>Today [" + DateTime.Now.ToString("dd/MM/yyyy") + "]</b><br/>ATM Cash Utilization for today"; ;
                    //BindWeeklyRepleninshmentAnalysisGrid((string)ViewState["orderby1"]);

                }
                else if (id == "Button2")
                {
                    Literal4.Text = "<b>Selected Criteria [" + TextBox3.Text + "-" + TextBox4.Text + "]</b><br/>Region wise breakup of Utilization for specified duration"; ;
                    //       Chart4.Url = "Data.aspx?param=25@" + + "@" + + "@0@0@167AD4@0";
                    //BindRegionRepleninshmentAnalysisGrid((string)ViewState["orderby"]);
                    from = TextBox3.Text; to = TextBox4.Text;
                    ChartUtilRegionWise.Series[0].Points.Clear();
                    BuildPieTest(from, to, "Transactions", "ATMUtilizationByRegion", "167AD4", "25", 0, 0, true, 0);
                }
                else if (id == "Button7")
                {
                    StringBuilder builder = new StringBuilder();
                    Avanza.CCMS.DAL.Region.RegionReader reader = Avanza.CCMS.DAL.Region.ExecuteReader("parent_region_id = (select region_id from region where parent_region_id = 1)");
                    while (reader.Read())
                        builder.Append(reader.CurrentRegion.RegionId + ",");
                    Session["regionIds"] = builder.ToString();
                    from = TextBox9.Text;
                    to = TextBox10.Text;
                    ChartUtilMutiRegionWise.Series[0].Points.Clear();
                    ChartUtilMutiRegionWise.Series[1].Points.Clear();
                    ChartUtilMutiRegionWise.Series[2].Points.Clear();

                    BuildMultiRegionChart("MultiRegionUtilizationAnalysis", from, to, "30", 0, 0, true);

                    //  Chart15.Url = "Data.aspx?param=30@" + TextBox9.Text + "@" + TextBox10.Text + "@0@167AD4@0";
                    Literal15.Text = "<b>Selected Criteria [" + TextBox9.Text + "-" + TextBox10.Text + "]</b><br/>Multi Region Analysis";
                    reader.Close();
                }

            }
            catch (Exception ex)
            {
                task.Log(System.Reflection.MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Error, ex.Message);
                Literal_Message.Text = "<li>" + ex.Message + "</li>";
            }
            finally
            {
                task.EndTask();
            }

        }
        protected void Button_Submit_Click(object sender, EventArgs e)
        {
            GenerateGraph(((System.Web.UI.WebControls.Button)sender).ID);
        }





        private SqlDataReader GetReader(string storedProcedureName, string fromDate, string toDate, bool includeParameter, string graphId, int upperBound, int lowerBound, bool excludeUser, int regionId)
        {
            SqlCommand cmd = ConnectionFactory.GetNewCommand(true);
            cmd.CommandText = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED ";
            cmd.ExecuteNonQuery();

            cmd.CommandTimeout = 180;
            cmd.CommandText = storedProcedureName;
            cmd.CommandType = CommandType.StoredProcedure;
            if (includeParameter)
            {
                if (Request.QueryString["graphId"] == "6" || Request.QueryString["graphId"] == "8")
                {
                    //All transactions
                    cmd.Parameters.Add("@status", SqlDbType.Int);
                    cmd.Parameters[0].Value = 3;
                }
                else if (Request.QueryString["graphId"] == "7" || Request.QueryString["graphId"] == "10"
                    || Request.QueryString["graphId"] == "5")
                {
                    //only failed transactions
                    cmd.Parameters.Add("@status", SqlDbType.Int);
                    cmd.Parameters[0].Value = 1;
                }
                else if (Request.QueryString["graphId"] == "11" || Request.QueryString["graphId"] == "14"
               || Request.QueryString["graphId"] == "15")
                {
                    //only suspicious transactions
                    cmd.Parameters.Add("@status", SqlDbType.Int);
                    cmd.Parameters[0].Value = 2;
                }
                else if (Request.QueryString["graphId"] == "12")
                {
                    //get all successful transaction
                    cmd.Parameters.Add("@status", SqlDbType.Int);
                    cmd.Parameters[0].Value = 0;
                }
                else if (Request.QueryString["graphId"] == "13")
                {
                    cmd.Parameters.Add("@region_name", SqlDbType.VarChar);
                    cmd.Parameters[0].Value = Session["region_id"];
                }
                else if (graphId == "1")
                {

                    cmd.Parameters.Add("@FromDate", SqlDbType.DateTime);
                    cmd.Parameters[0].Value = fromDate;

                    cmd.Parameters.Add("@ToDate", SqlDbType.DateTime);
                    cmd.Parameters[1].Value = toDate;

                    cmd.Parameters.Add("@AtmId", SqlDbType.VarChar);
                    cmd.Parameters[2].Value = Session[SessionVars.SelectedATMs.ToString()].ToString();
                }

                else if (graphId == "20")
                {

                    cmd.Parameters.Add("@FromDate", SqlDbType.DateTime);
                    cmd.Parameters[0].Value = fromDate;

                    cmd.Parameters.Add("@ToDate", SqlDbType.DateTime);
                    cmd.Parameters[1].Value = toDate;

                    cmd.Parameters.Add("@AtmId", SqlDbType.VarChar);
                    cmd.Parameters[2].Value = Session[SessionVars.SelectedATMs.ToString()].ToString();

                    cmd.Parameters.Add("@LowerBound", SqlDbType.Int);
                    cmd.Parameters[3].Value = lowerBound;

                    cmd.Parameters.Add("@UpperBound", SqlDbType.Int);
                    cmd.Parameters[4].Value = upperBound;

                }
                else if (graphId == "27" || graphId == "28" || graphId == "29" || graphId == "30")
                {

                    cmd.Parameters.Add("@FromDate", SqlDbType.DateTime);
                    cmd.Parameters[0].Value = DateTime.ParseExact(fromDate,"dd/MM/yyyy",null);

                    cmd.Parameters.Add("@ToDate", SqlDbType.DateTime);
                    cmd.Parameters[1].Value = DateTime.ParseExact(toDate, "dd/MM/yyyy", null);

                    cmd.Parameters.Add("@RegionId", SqlDbType.Int);
                    cmd.Parameters[2].Value = regionId;

                }
                else if (graphId == "21" || graphId == "23")
                {

                    cmd.Parameters.Add("@FromDate", SqlDbType.DateTime);
                    cmd.Parameters[0].Value = fromDate;

                    cmd.Parameters.Add("@ToDate", SqlDbType.DateTime);
                    cmd.Parameters[1].Value = toDate;

                    cmd.Parameters.Add("@OrganizationId", SqlDbType.Int);
                    cmd.Parameters[2].Value = 1;

                }
                else if (graphId == "25" || graphId == "31" || graphId == "32" || graphId == "33" || graphId == "34" || graphId == "35")
                {
                    cmd.Parameters.Add("@FromDate", SqlDbType.DateTime);
                    cmd.Parameters[0].Value = DateTime.ParseExact(fromDate, "dd/MM/yyyy", null);

                    cmd.Parameters.Add("@ToDate", SqlDbType.DateTime);
                    cmd.Parameters[1].Value = DateTime.ParseExact(toDate, "dd/MM/yyyy", null);
                }
                else if (graphId == "22" || graphId == "24" || graphId == "26")
                {

                    cmd.Parameters.Add("@FromDate", SqlDbType.DateTime);
                    cmd.Parameters[0].Value = DateTime.ParseExact(fromDate, "dd/MM/yyyy", null);

                    cmd.Parameters.Add("@ToDate", SqlDbType.DateTime);
                    cmd.Parameters[1].Value = DateTime.ParseExact(toDate, "dd/MM/yyyy", null);

                    cmd.Parameters.Add("@AtmId", SqlDbType.VarChar);
                    cmd.Parameters[2].Value = Session[SessionVars.SelectedATMs.ToString()].ToString();

                }


                else if (Request.QueryString["graphId"] == "2")
                {
                    cmd.Parameters.Add("@user_id", SqlDbType.Int);
                    if (excludeUser)
                        cmd.Parameters[0].Value = -1;
                    else
                        cmd.Parameters[0].Value = 2;//(int)Session[SessionVars.user.ToString()];
                }
            }

            return cmd.ExecuteReader();
        }

        private void BuildPieTest(string from, string to, string dataTitle, string SQL, string color, string graphId, int upperBound, int lowerBound, bool excludeUser, int regionId)
        {
            string[] sliceColor = { "#167AD4", "#2A9c0A", "#F59C0B", "#F03C2C", "#EFE4B0" };
            //OpenFlashChart.Charts.Pie pie = new OpenFlashChart.Charts.Pie(70, color, "{font-size:10px; color: Navy;}");
            SqlDataReader reader = null;
            try
            {
                reader = GetReader(SQL, from, to, true, graphId, upperBound, lowerBound, excludeUser, regionId);
                //OpenFlashChart.Charts.Pie.Piece piePiece = null;
                decimal totalTrxn = 0;
                Hashtable ht = new Hashtable();
                List<string> keys = new List<string>();
                int counter = 0;

                while (reader.Read())
                {
                    if (graphId == "25")
                    {
                        ht.Add(reader.GetString(0), reader[3]);
                        keys.Add(reader.GetString(0));
                        totalTrxn += decimal.Parse(reader[3].ToString());
                    }
                    else if (graphId == "24")
                    {
                        ht.Add(reader.GetString(0), reader[3]);
                        keys.Add(reader.GetString(0));
                        totalTrxn += decimal.Parse(reader[3].ToString());
                    }

                    else if (graphId == "21" || graphId == "23")
                    {
                        ht.Add(reader.GetString(1), reader[0]);
                        keys.Add(reader.GetString(1));
                        totalTrxn += decimal.Parse(reader[0].ToString());
                    }
                    //    piePiece = new OpenFlashChart.Charts.Pie.Piece(reader.GetInt32(0), reader.GetString(1), sliceColor[counter % sliceColor.Length], reader.GetString(1));
                    else
                    {
                        //if (graphId == "34" || graphId == "35")
                        //    piePiece = new OpenFlashChart.Charts.Pie.Piece(float.Parse(reader[1].ToString()), reader.GetString(0), sliceColor[counter % sliceColor.Length], reader[1].ToString());
                        //else if (graphId == "32" || graphId == "33")
                        //    piePiece = new OpenFlashChart.Charts.Pie.Piece(reader.GetInt32(0), reader.GetString(1), sliceColor[counter % sliceColor.Length], reader.GetInt32(0).ToString());

                        //else if (graphId == "27")
                        //    piePiece = new OpenFlashChart.Charts.Pie.Piece(float.Parse(reader[1].ToString()), "Hr[" + reader.GetInt32(0) + "]", sliceColor[counter % sliceColor.Length], reader[1].ToString());
                        //else if (graphId == "31")
                        //    piePiece = new OpenFlashChart.Charts.Pie.Piece(reader.GetInt32(1), "Hr[" + reader.GetInt32(0) + "]", sliceColor[counter % sliceColor.Length], reader.GetInt32(1).ToString());

                        ////piePiece = new OpenFlashChart.Charts.Pie.Piece(reader.GetInt32(1), "Hr[" + reader.GetInt32(0) + "]", sliceColor[counter % sliceColor.Length]);
                        //else
                        //    piePiece = new OpenFlashChart.Charts.Pie.Piece(float.Parse(reader[0].ToString()), reader.GetInt32(1).ToString(), sliceColor[counter % sliceColor.Length], reader[1].ToString());
                        ////piePiece = new OpenFlashChart.Charts.Pie.Piece(reader.GetInt32(0), reader.GetInt32(1).ToString(), sliceColor[counter % sliceColor.Length]);
                        //counter++;
                        //pie.Data.Add(piePiece);
                    }

                }
                if (graphId == "21" || graphId == "23" || graphId == "25")
                {
                    for (int i = 0; i < keys.Count; i++)
                    {

               //         piePiece = new OpenFlashChart.Charts.Pie.Piece(float.Parse(ht[keys[i]].ToString()), keys[i].ToString() + "[" + Math.Round((decimal.Parse(ht[keys[i]].ToString()) / totalTrxn) * 100, 2) + "]", sliceColor[counter % sliceColor.Length], keys[i].ToString());
                        //piePiece = new OpenFlashChart.Charts.Pie.Piece(float.Parse(ht[keys[i]].ToString()), keys[i].ToString() + "[" + Math.Round((decimal.Parse(ht[keys[i]].ToString()) / totalTrxn) * 100, 2) + "]", sliceColor[counter % sliceColor.Length]);
                        counter++;
                        ChartUtilRegionWise.Series[0].Points.AddXY(keys[i].ToString(), Math.Round(float.Parse(ht[keys[i]].ToString()),2));
                 //       pie.Data.Add(piePiece);

                    }
                    //if (graphId == "24")
                    //{
                    //    float val = float.Parse((100 - decimal.Parse(ht[keys[0]].ToString())).ToString());
                    //    piePiece = new OpenFlashChart.Charts.Pie.Piece(val,val.ToString() , sliceColor[counter % sliceColor.Length], keys[0].ToString());
                    //    pie.Data.Add(piePiece);
                    //}
                }

                else if (graphId == "24")
                {
                    float val = 0;
                    for (int i = 0; i < keys.Count; i++)
                    {

                        val = float.Parse(ht[keys[i]].ToString());
                        //piePiece = new OpenFlashChart.Charts.Pie.Piece(val, val.ToString() + " Utilization", sliceColor[counter % sliceColor.Length], "");
                       // counter++;
                        ChartUtilizationCurrentDay.Series[0].Points.AddXY(val, val);
                        //pie.Data.Add(piePiece);

                    }
                    if (keys.Count > 0)
                    {
                        val = float.Parse((100 - decimal.Parse(ht[keys[0]].ToString())).ToString());
                        //piePiece = new OpenFlashChart.Charts.Pie.Piece(val, val.ToString() + " Unutilized", sliceColor[counter % sliceColor.Length], "");
                        //pie.Data.Add(piePiece);
                        ChartUtilizationCurrentDay.Series[0].Points.AddXY(val, val);

                    }

                }

                //chart.Pie = pie;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }
        private void BuildMultiRegionChart(string proc, string fromDate, string toDate, string graphId, int upperBound, int lowerBound, bool excludeUser)
        {

            ChartUtilMutiRegionWise.Legends[0].Enabled = true;
            SqlDataReader reader = null;
            Hashtable ht1 = new Hashtable(7);
            Hashtable ht2 = new Hashtable(7);
            Hashtable ht3 = new Hashtable(7);
            string region1 = "";
            string region2 = "";
            string region3 = "";

            int max = 0;


            string[] regionIds = Session["regionIds"].ToString().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            int regionId = int.Parse(regionIds[0]);
            try
            {
                reader = GetReader(proc, fromDate, toDate, true, graphId, upperBound, lowerBound, excludeUser, regionId);
                while (reader.Read())
                {
                    if (graphId == "28")
                        ht1.Add(reader.GetString(1), float.Parse(reader[0].ToString()));
                    else if (graphId == "30")
                        ht1.Add(reader.GetString(1), reader.GetDouble(0));
                    else
                        ht1.Add(reader.GetString(1), reader.GetInt32(0));
                }
                reader.Close();
                if (regionIds.Length > 1)
                {
                    regionId = int.Parse(regionIds[1]);
                    reader = GetReader(proc, fromDate, toDate, true, graphId, upperBound, lowerBound, excludeUser, regionId);
                    while (reader.Read())
                    {
                        if (graphId == "30")
                            ht2.Add(reader.GetString(1), reader.GetDouble(0));
                        else
                            ht2.Add(reader.GetString(1), reader.GetInt32(0));
                    }
                    reader.Close();
                }
                if (regionIds.Length > 2)
                {
                    regionId = int.Parse(regionIds[2]);
                    reader = GetReader(proc, fromDate, toDate, true, graphId, upperBound, lowerBound, excludeUser, regionId);
                    while (reader.Read())
                    {
                        if (graphId == "30")
                            ht3.Add(reader.GetString(1), reader.GetDouble(0));
                        else
                            ht3.Add(reader.GetString(1), reader.GetInt32(0));
                    }
                    reader.Close();
                }
                //"#167AD4", "#2A9c0A", "#F59C0B", "#F03C2C", "#EFE4B0"

                if (regionIds.Length > 0)
                {
                    region1 = Avanza.CCMS.DAL.Region.LoadRegionByPk(int.Parse(regionIds[0])).RegionName;
                    ChartUtilMutiRegionWise.Series[0].Name = region1;
                   
                
                }
                if (regionIds.Length > 1)
                {
                    region2 = Avanza.CCMS.DAL.Region.LoadRegionByPk(int.Parse(regionIds[1])).RegionName;
                    ChartUtilMutiRegionWise.Series[1].Name = region2;

                }
                if (regionIds.Length > 2)
                {
                    region3 = Avanza.CCMS.DAL.Region.LoadRegionByPk(int.Parse(regionIds[2])).RegionName;
                    ChartUtilMutiRegionWise.Series[2].Name = region3;

                }


                //chartData = new OpenFlashChart.Charts.Bar(100, "#167AD4", Avanza.CCMS.DAL.Region.LoadRegionByPk(int.Parse(regionIds[0])).RegionName, 12);
                //if (regionIds.Length > 1)
                //    chartData1 = new OpenFlashChart.Charts.Bar(100, "#2A9c0A", Avanza.CCMS.DAL.Region.LoadRegionByPk(int.Parse(regionIds[1])).RegionName, 12);
                //else
                //    chartData1 = new OpenFlashChart.Charts.Bar(100);

                //if (regionIds.Length > 2)
                //    chartData2 = new OpenFlashChart.Charts.Bar(100, "#F59C0B", Avanza.CCMS.DAL.Region.LoadRegionByPk(int.Parse(regionIds[2])).RegionName, 12);
                //else
                //    chartData2 = new OpenFlashChart.Charts.Bar(100);
                TimeSpan timeSpan = DateTime.ParseExact(toDate,"dd/MM/yyyy",null) - DateTime.ParseExact(fromDate,"dd/MM/yyyy",null);
                bool pointFound = false;
                string dateString = "";
                for (int i = 0; i < timeSpan.Days; i++)
                {
                    dateString = fromDate;
                    if (ht1.Contains(dateString))
                        pointFound = true;
                    else if (ht2.Contains(dateString))
                        pointFound = true;
                    else if (ht3.Contains(dateString))
                        pointFound = true;
                    if (pointFound)
                    {
                        //chart.LabelsX.Add(dateString);
                        if (ht1.Contains(dateString))
                        {
                            double val = Math.Round(float.Parse(ht1[fromDate].ToString()), 2);
                            if (val > 0)
                            {
                                ChartUtilMutiRegionWise.Series[0].Points.AddXY(dateString, val);
                                //  chartData.Data.Add(val);
                             }
                        }
                        //else
                        //    ChartUtilMutiRegionWise.Series[0].Points.AddXY(dateString, 0);

                        if (ht2.Contains(dateString))
                        {
                            double val = Math.Round(float.Parse(ht2[fromDate].ToString()), 2);
                            //ChartUtilMutiRegionWise.Series[0].Points.Add(region1, val);
                            if (val > 0)
                                ChartUtilMutiRegionWise.Series[1].Points.AddXY(dateString, val);

                            //chartData1.Data.Add(val);
                         }
                        //else
                        //    ChartUtilMutiRegionWise.Series[1].Points.AddXY(dateString, 0);



                        if (ht3.Contains(dateString))
                        {
                            double val = Math.Round(float.Parse(ht3[fromDate].ToString()),2);
                            if (val > 0)
                                ChartUtilMutiRegionWise.Series[2].Points.AddXY(dateString, val);

                         }
                        //else
                        //    ChartUtilMutiRegionWise.Series[0].Points.AddXY(dateString, 0);

                    }
                    fromDate = DateTime.ParseExact(fromDate,"dd/MM/yyyy",null).AddDays(1).ToString("dd/MM/yyyy");
                }
                //chart.Data.Add(chartData);
                //chart.Data.Add(chartData1);
                //chart.Data.Add(chartData2);

                //maxYList.Add(max);
            }

            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }

        private void BuildGraphData(string from, string to, string dataTitle, string storedProcedureName, bool printLabel,
                                   string color, bool includeParameter, int graphType, int regionId, string graphId, Chart chart)
        {


            if (graphType == 0)
            {
            }

            else
            { }

            List<string> day = new List<string>();
            List<float> dataPoints = new List<float>();
            float dataVal = 0;
            SqlDataReader reader = null;
            try
            {
                reader = GetReader(storedProcedureName, from, to, includeParameter, graphId, 0, 0, false, regionId);
                int hours = 0;
                int counter = 0;
                float max = -1;
                float maxFloat = -1;
                while (reader.Read())
                {
                    if (graphId == "26")
                    {
                        counter++;
                        day.Add(reader.GetString(0));
                        if (maxFloat < float.Parse(reader[1].ToString()))
                            maxFloat = float.Parse(reader[1].ToString());
                        dataPoints.Add(float.Parse(reader[1].ToString()));

                        //chart.LabelsX.Add(reader.GetString(0));
                        //chartData.Data.Add(float.Parse(reader[1].ToString()));

                    }
                    else if (graphId == "24")
                    {
                        counter++;
                        //  if (counter % 2 == 0)
                        //    ChartPieCurrentDay.LabelsX.Add("\r\n" + reader.GetString(0));
                        //  ChartPieCurrentDay.Series["Default"].Points.AddY(reader.GetString(0));

                        // else
                        //    chart.LabelsX.Add(reader.GetString(0));
                        dataVal = float.Parse(reader[3].ToString());

                        ///chartData.Data.Add(float.Parse(reader[3].ToString()));
                        ///
                        if (dataVal > 0) {
                            chart.Series[0].Points.AddXY(reader.GetString(0), Math.Round(dataVal,2));

                            if (maxFloat < dataVal)
                                maxFloat = dataVal;
                        }

                    }

                    else if (graphId == "31")
                    {
                        counter++;
                        //if (counter % 2 == 0)
                        //    chart.LabelsX.Add("\r\n" + reader.GetInt32(0));
                        //else
                        //                        chart.LabelsX.Add(reader.GetInt32(0));

                        //                      chartData.Data.Add(float.Parse(reader[1].ToString()));
                        if (maxFloat < float.Parse(reader[1].ToString()))
                            maxFloat = float.Parse(reader[1].ToString());


                    }

                    else if (graphId == "22" || graphId == "32" || graphId == "33")
                    {
                        counter++;
                        if (counter % 2 == 0)
                            //    chart.LabelsX.Add("\r\n" + reader.GetString(1));
                            //else
                            //    chart.LabelsX.Add(reader.GetString(1));

                            //chartData.Data.Add(reader.GetInt32(0));

                            if (max < reader.GetInt32(0))
                                max = reader.GetInt32(0);

                    }
                    else if (graphId == "34" || graphId == "35")
                    {
                        counter++;
                        //if (counter % 2 == 0)
                        //    chart.LabelsX.Add("\r\n" + reader.GetString(0));
                        //else
                        //    chart.LabelsX.Add(reader.GetString(0));
                        //float dataPoint = float.Parse(reader[1].ToString());
                        //chartData.Data.Add(dataPoint);

                        //if (maxFloat < dataPoint)
                        //    maxFloat = dataPoint;
                    }
                    else if (graphId == "20")
                    {
                        counter++;
                        //if (counter % 2 == 0)
                        //    chart.LabelsX.Add("\r\n" + reader.GetString(0));
                        //else
                        //    chart.LabelsX.Add(reader.GetString(0));
                        float var = 0;
                        if (reader.GetDataTypeName(1).ToLower() == "decimal")
                            var = float.Parse(reader[1].ToString());
                        else
                            var = int.Parse(reader[1].ToString());

                        //chartData.Data.Add(var);
                        if (max < var)
                            max = var;

                    }
                    else
                    {

                        hours = reader.GetInt32(1);
                        while (counter != hours)
                        {
                            //if (printLabel)
                            //    chart.LabelsX.Add(counter);

                            //chartData.Data.Add(0);
                            counter++;
                        }
                        //if (printLabel)
                        //    chart.LabelsX.Add(hours);
                        float val = 0;
                        if (reader.GetDataTypeName(0).ToLower() == "decimal")
                            val = float.Parse(reader.GetDecimal(0).ToString());
                        else
                            val = reader.GetInt32(0);
                        //chartData.Data.Add(float.Parse(reader.GetDecimal(0).ToString()));

                        //  chartData.Data.Add(val);

                        if (val > max)
                            max = val;
                        counter++;
                    }
                }
                if (graphId != "20" && graphId != "22" && graphId != "24" && graphId != "26" && graphId != "32" && graphId != "33" && graphId != "34" && graphId != "35")
                {
                    while (counter != 24)
                    {
                        counter++;
                        //if (printLabel)
                        //    chart.LabelsX.Add(counter);
                        //chartData.Data.Add(0);

                    }
                }
                if (graphId == "26")
                {
                    int[] indexes = new int[5];
                    int i = 0;
                    int labelCounter = 0;

                    //if (counter > 7)
                    //{
                    //    Random r = new Random();
                    //    while (i < 3)
                    //    {
                    //        indexes[i++] = r.Next(2, counter - 1);
                    //    }

                    //}
                    //                  DateTime temp = fromDate;
                    for (i = 0; i < dataPoints.Count; i++)
                    {

                        //if (Array.IndexOf(indexes, i) >= 0 && counter > 7)
                        //{
                        DateTime dt = DateTime.ParseExact(day[i], "dd/MM/yyyy", null);
                        //while (temp.Date != dt.Date)
                        //{
                        //    if (labelCounter % 2 == 0)
                        //        chart.LabelsX.Add(temp.Day + " " + GetFriendlyName(temp.Month));
                        //    else
                        //        chart.LabelsX.Add("\r\n" + temp.Day + " " + GetFriendlyName(temp.Month));
                        //    chartData.Data.Add(0);
                        //    temp = temp.AddDays(1);

                        //}
                        //if (labelCounter % 2 == 0)
                        //    chart.LabelsX.Add(dt.Day + " " + GetFriendlyName(dt.Month));
                        //else
                        //    chart.LabelsX.Add("\r\n" + dt.Day + " " + GetFriendlyName(dt.Month));
                        //}
                        //else
                        //    chart.LabelsX.Add("");
                        labelCounter++;
                        // chartData.Data.Add(dataPoints[i]);
                        //                        temp = dt.AddDays(1);
                    }
                }
                // chart.Data.Add(chartData);
                //chartData1.Data.Add(10);
                //chartData2.Data.Add(20);
                //chart.Data.Add(chartData1);
                //chart.Data.Add(chartData2);
                //if (maxFloat == -1)
                //    maxYList.Add(max);
                //else
                //    maxYList.Add(maxFloat);
            }

            finally
            {
                if (reader != null)
                    reader.Close();
                //    if (cmd != null)
                //        if (cmd.Connection != null)
                //            cmd.Connection.Close();
                //
            }
        }


        protected void GridView_CashUtilizationAnalysis_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (ViewState["SortExpression"].ToString().IndexOf(e.SortExpression) >= 0)
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


            ViewState["orderby"] = ViewState["SortExpression"] + " " + ViewState["sortDirection"];
            ///BindGrid(null);
            BindRegionRepleninshmentAnalysisGrid((string)ViewState["orderby"]);
        }

        protected void GridView1_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (ViewState["SortExpression1"].ToString().IndexOf(e.SortExpression) >= 0)
            {
                if (ViewState["sortDirection1"].ToString() == " asc")
                    ViewState["sortDirection1"] = " desc";
                else
                    ViewState["sortDirection1"] = " asc";
            }
            else
            {
                ViewState["SortExpression1"] = e.SortExpression;
                ViewState["sortDirection1"] = " asc";
            }


            ViewState["orderby1"] = ViewState["SortExpression1"] + " " + ViewState["sortDirection1"];
            ///BindGrid(null);
            BindWeeklyRepleninshmentAnalysisGrid((string)ViewState["orderby1"]);
        }

        private void BindWeeklyRepleninshmentAnalysisGrid(string orderBy)
        {
            SqlConnection conn = ConnectionFactory.GetNewConnection();
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"select title, convert(varchar,rep_datetime,103) as rep_dateTime,
        sum(cash_added1*denomination_type_1+cash_added2*denomination_type_2+cash_added3*denomination_type_3+
cash_added4*denomination_type_4+cash_added5*denomination_type_5+cash_added6*denomination_type_6+cash_added7*denomination_type_7) as amount
from vReplenishment inner join atm on vReplenishment.atm_id = atm.atm_id 
inner join region on atm.region_id = region.region_id 
inner join note_set_type on atm.note_set_type_id = note_set_type.note_set_type_id
where rep_datetime>=convert(datetime,'" + TextBox_From.Text + "',103) and rep_datetime<=convert(datetime,'" + TextBox_To.Text + " 23:59:59',103)  " +
            " and atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") " +


" group by title,convert(varchar,rep_datetime,103) order by " + orderBy;

            SqlDataAdapter adapter = new System.Data.SqlClient.SqlDataAdapter(cmd);
            System.Data.DataTable dt = new System.Data.DataTable();
            adapter.Fill(dt);
            //GridView1.DataSource = dt;
            //GridView1.DataBind();
        }

        private void BindRegionRepleninshmentAnalysisGrid(string orderBy)
        {
            SqlConnection conn = ConnectionFactory.GetNewConnection();
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"select title, convert(varchar,rep_datetime,103) as rep_dateTime, region_name, 
        sum(cash_added1*denomination_type_1+cash_added2*denomination_type_2+cash_added3*denomination_type_3+
cash_added4*denomination_type_4+cash_added5*denomination_type_5+cash_added6*denomination_type_6+cash_added7*denomination_type_7) as amount
from vReplenishment inner join atm on vReplenishment.atm_id = atm.atm_id 
inner join region on atm.region_id = region.region_id 
inner join note_set_type on atm.note_set_type_id = note_set_type.note_set_type_id
where rep_datetime>=convert(datetime,'" + TextBox3.Text + "',103) and rep_datetime<=convert(datetime,'" + TextBox4.Text + " 23:59:59',103) " +

" group by title,convert(varchar,rep_datetime,103),region_name order by " + orderBy;

            SqlDataAdapter adapter = new System.Data.SqlClient.SqlDataAdapter(cmd);
            System.Data.DataTable dt = new System.Data.DataTable();
            adapter.Fill(dt);

            //GridView_RepAnalysis.DataSource = dt;
            //GridView_RepAnalysis.DataBind();
        }

    }
}