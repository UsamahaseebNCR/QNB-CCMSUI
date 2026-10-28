using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;
using System.Text;
using System.Drawing;

namespace iJournalUI
{
    public partial class Data : System.Web.UI.Page
    {
        SqlCommand cmd = null;
        ArrayList maxYList = new ArrayList();
        OpenFlashChart.Graph chart = new OpenFlashChart.Graph();
        bool excludeUser = true;
        DateTime fromDate = DateTime.Now;
        DateTime toDate = DateTime.Now;
        int regionId = 0;
        string graphId = "";
        int lowerBound = 0;
        int upperBound = 0;


        private string GetFriendlyName(int month)
        {
            if (month == 1)
                return "Jan";
            else if (month == 2)
                return "Feb";
            else if (month == 3)
                return "Mar";
            else if (month == 4)
                return "Apr";
            else if (month == 5)
                return "May";
            else if (month == 6)
                return "Jun";
            else if (month == 7)
                return "Jul";
            else if (month == 8)
                return "Aug";
            else if (month == 9)
                return "Sep";
            else if (month == 10)
                return "Oct";
            else if (month == 11)
                return "Nov";
            else if (month == 12)
                return "Dec";
            return "Not Defined";
        }


        private void InitGraph(string graphTitle, string yAxisTitle, string xAxisTitle)
        {


            chart.Title = new OpenFlashChart.Title(graphTitle, "{font-family:verdana;font-size:10px;}");

            chart.LegendY = new OpenFlashChart.LegendY(yAxisTitle, 15, Color.Navy.ToString());
            chart.LegendX = new OpenFlashChart.LegendX(xAxisTitle, 15, Color.Navy.ToString());
            chart.BgColor = "#FFFFFF";
            chart.StepsY = 5;
            chart.GridColorX = "#FFFFFF";
            //chart.GridColorY = "#FFFFFF";

            chart.TickSizeX = 7;
            chart.AxisStepsX = 1;

        }
        private SqlDataReader GetReader(string storedProcedureName, bool includeParameter)
        {
            cmd = ConnectionFactory.GetNewCommand(true);
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
                    cmd.Parameters[0].Value = fromDate;

                    cmd.Parameters.Add("@ToDate", SqlDbType.DateTime);
                    cmd.Parameters[1].Value = toDate;

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
                    cmd.Parameters[0].Value = fromDate;

                    cmd.Parameters.Add("@ToDate", SqlDbType.DateTime);
                    cmd.Parameters[1].Value = toDate;
                }
                else if (graphId == "22" || graphId == "24" || graphId == "26")
                {

                    cmd.Parameters.Add("@FromDate", SqlDbType.DateTime);
                    cmd.Parameters[0].Value = fromDate;

                    cmd.Parameters.Add("@ToDate", SqlDbType.DateTime);
                    cmd.Parameters[1].Value = toDate;

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
        private void BuildGraphData(string dataTitle, string storedProcedureName, bool printLabel,
                                    string color, bool includeParameter, int graphType)
        {
            OpenFlashChart.Charts.ChartData chartData = null;


            if (graphType == 0)
            {
                chartData = new OpenFlashChart.Charts.Bar(100, color, dataTitle, 70);
                //chartData1 = new OpenFlashChart.Charts.Bar(10, "#000000", "test", 80);
                //chartData2 = new OpenFlashChart.Charts.Bar(50, "#ECE2B0", "test", 80);
            }

            else
                chartData = new OpenFlashChart.Charts.Line(2, color, dataTitle, 10, 3);

            List<string> day = new List<string>();
            List<float> dataPoints = new List<float>();

            SqlDataReader reader = null;
            try
            {
                reader = GetReader(storedProcedureName, includeParameter);
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
                        if (counter % 2 == 0)
                            chart.LabelsX.Add("\r\n" + reader.GetString(0));
                        else
                            chart.LabelsX.Add(reader.GetString(0));

                        chartData.Data.Add(float.Parse(reader[3].ToString()));
                        if (maxFloat < float.Parse(reader[3].ToString()))
                            maxFloat = float.Parse(reader[3].ToString());


                    }

                    else if (graphId == "31")
                    {
                        counter++;
                        //if (counter % 2 == 0)
                        //    chart.LabelsX.Add("\r\n" + reader.GetInt32(0));
                        //else
                        chart.LabelsX.Add(reader.GetInt32(0));

                        chartData.Data.Add(float.Parse(reader[1].ToString()));
                        if (maxFloat < float.Parse(reader[1].ToString()))
                            maxFloat = float.Parse(reader[1].ToString());


                    }

                    else if (graphId == "22" || graphId == "32" || graphId == "33")
                    {
                        counter++;
                        if (counter % 2 == 0)
                            chart.LabelsX.Add("\r\n" + reader.GetString(1));
                        else
                            chart.LabelsX.Add(reader.GetString(1));

                        chartData.Data.Add(reader.GetInt32(0));

                        if (max < reader.GetInt32(0))
                            max = reader.GetInt32(0);

                    }
                    else if (graphId == "34" || graphId == "35")
                    {
                        counter++;
                        if (counter % 2 == 0)
                            chart.LabelsX.Add("\r\n" + reader.GetString(0));
                        else
                            chart.LabelsX.Add(reader.GetString(0));
                        float dataPoint = float.Parse(reader[1].ToString());
                        chartData.Data.Add(dataPoint);

                        if (maxFloat < dataPoint)
                            maxFloat = dataPoint;
                    }
                    else if (graphId == "20")
                    {
                        counter++;
                        if (counter % 2 == 0)
                            chart.LabelsX.Add("\r\n" + reader.GetString(0));
                        else
                            chart.LabelsX.Add(reader.GetString(0));
                        float var = 0;
                        if (reader.GetDataTypeName(1).ToLower() == "decimal")
                            var = float.Parse(reader[1].ToString());
                        else
                            var = int.Parse(reader[1].ToString());

                        chartData.Data.Add(var);
                        if (max < var)
                            max = var;

                    }
                    else
                    {

                        hours = reader.GetInt32(1);
                        while (counter != hours)
                        {
                            if (printLabel)
                                chart.LabelsX.Add(counter);

                            chartData.Data.Add(0);
                            counter++;
                        }
                        if (printLabel)
                            chart.LabelsX.Add(hours);
                        float val = 0;
                        if (reader.GetDataTypeName(0).ToLower() == "decimal")
                            val = float.Parse(reader.GetDecimal(0).ToString());
                        else
                            val = reader.GetInt32(0);
                        //chartData.Data.Add(float.Parse(reader.GetDecimal(0).ToString()));

                        chartData.Data.Add(val);

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
                        if (printLabel)
                            chart.LabelsX.Add(counter);
                        chartData.Data.Add(0);

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
                        if (labelCounter % 2 == 0)
                            chart.LabelsX.Add(dt.Day + " " + GetFriendlyName(dt.Month));
                        else
                            chart.LabelsX.Add("\r\n" + dt.Day + " " + GetFriendlyName(dt.Month));
                        //}
                        //else
                        //    chart.LabelsX.Add("");
                        labelCounter++;
                        chartData.Data.Add(dataPoints[i]);
                        //                        temp = dt.AddDays(1);
                    }
                }
                chart.Data.Add(chartData);
                //chartData1.Data.Add(10);
                //chartData2.Data.Add(20);
                //chart.Data.Add(chartData1);
                //chart.Data.Add(chartData2);
                if (maxFloat == -1)
                    maxYList.Add(max);
                else
                    maxYList.Add(maxFloat);
            }

            finally
            {
                if (reader != null)
                    reader.Close();
                if (cmd != null)
                    if (cmd.Connection != null)
                        cmd.Connection.Close();
            }
        }




        private void BuildMultiRegionChart(string proc)
        {

            SqlDataReader reader = null;
            OpenFlashChart.Charts.ChartData chartData = null;
            OpenFlashChart.Charts.ChartData chartData1 = null;
            OpenFlashChart.Charts.ChartData chartData2 = null;
            Hashtable ht1 = new Hashtable(7);
            Hashtable ht2 = new Hashtable(7);
            Hashtable ht3 = new Hashtable(7);
            int max = 0;


            string[] regionIds = Session["regionIds"].ToString().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            regionId = int.Parse(regionIds[0]);
            try
            {
                reader = GetReader(proc, true);
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
                    reader = GetReader(proc, true);
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
                    reader = GetReader(proc, true);
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

                chartData = new OpenFlashChart.Charts.Bar(100, "#167AD4", Avanza.CCMS.DAL.Region.LoadRegionByPk(int.Parse(regionIds[0])).RegionName, 12);
                if (regionIds.Length > 1)
                    chartData1 = new OpenFlashChart.Charts.Bar(100, "#2A9c0A", Avanza.CCMS.DAL.Region.LoadRegionByPk(int.Parse(regionIds[1])).RegionName, 12);
                else
                    chartData1 = new OpenFlashChart.Charts.Bar(100);

                if (regionIds.Length > 2)
                    chartData2 = new OpenFlashChart.Charts.Bar(100, "#F59C0B", Avanza.CCMS.DAL.Region.LoadRegionByPk(int.Parse(regionIds[2])).RegionName, 12);
                else
                    chartData2 = new OpenFlashChart.Charts.Bar(100);
                TimeSpan timeSpan = toDate - fromDate;
                bool pointFound = false;
                string dateString = "";
                for (int i = 0; i < timeSpan.Days; i++)
                {
                    dateString = fromDate.ToString("dd/MM/yyyy");
                    if (ht1.Contains(dateString))
                        pointFound = true;
                    else if (ht2.Contains(dateString))
                        pointFound = true;
                    else if (ht3.Contains(dateString))
                        pointFound = true;
                    if (pointFound)
                    {
                        chart.LabelsX.Add(dateString);
                        if (ht1.Contains(dateString))
                        {
                            float val = float.Parse(ht1[fromDate.ToString("dd/MM/yyyy")].ToString());
                            chartData.Data.Add(val);
                            if (val > max)
                                max = (int)val;
                        }
                        else
                            chartData.Data.Add(0);

                        if (ht2.Contains(dateString))
                        {
                            float val = float.Parse(ht2[fromDate.ToString("dd/MM/yyyy")].ToString());
                            chartData1.Data.Add(val);
                            if (val > max)
                                max = (int)val;
                        }
                        else
                            chartData1.Data.Add(0);


                        if (ht3.Contains(dateString))
                        {
                            float val = float.Parse(ht3[fromDate.ToString("dd/MM/yyyy")].ToString());
                            chartData2.Data.Add(val);
                            if (val > max)
                                max = (int)val;
                        }
                        else
                            chartData2.Data.Add(0);
                    }
                    fromDate = fromDate.AddDays(1);
                }
                chart.Data.Add(chartData);
                chart.Data.Add(chartData1);
                chart.Data.Add(chartData2);

                maxYList.Add(max);
            }

            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }
        private void BuildPie(string dataTitle, string SQL, string color)
        {
            string[] sliceColor = { "#3334AD", "#189570", "#C6E2CF" };
            OpenFlashChart.Charts.Pie pie = new OpenFlashChart.Charts.Pie(70, color, "{font-size:8px; color: #ff0000;}");
            int counter = 0;
            SqlDataReader reader = GetReader(SQL, true);
            while (reader.Read())
            {

                OpenFlashChart.Charts.Pie.Piece piePiece = new OpenFlashChart.Charts.Pie.Piece(reader.GetInt32(0), reader.GetString(1), sliceColor[counter % sliceColor.Length], reader.GetString(1));
                //OpenFlashChart.Charts.Pie.Piece piePiece = new OpenFlashChart.Charts.Pie.Piece(reader.GetInt32(0), reader.GetString(1), sliceColor[counter % sliceColor.Length]);

                pie.Data.Add(piePiece);
                counter++;
            }
            chart.Pie = pie;
        }

        private void BuildPieTest(string dataTitle, string SQL, string color)
        {
            string[] sliceColor = { "#167AD4", "#2A9c0A", "#F59C0B", "#F03C2C", "#EFE4B0" };
            OpenFlashChart.Charts.Pie pie = new OpenFlashChart.Charts.Pie(70, color, "{font-size:10px; color: Navy;}");
            SqlDataReader reader = null;
            try
            {
                reader = GetReader(SQL, true);
                OpenFlashChart.Charts.Pie.Piece piePiece = null;
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
                        if (graphId == "34" || graphId == "35")
                            piePiece = new OpenFlashChart.Charts.Pie.Piece(float.Parse(reader[1].ToString()), reader.GetString(0), sliceColor[counter % sliceColor.Length], reader[1].ToString());
                        else if (graphId == "32" || graphId == "33")
                            piePiece = new OpenFlashChart.Charts.Pie.Piece(reader.GetInt32(0), reader.GetString(1), sliceColor[counter % sliceColor.Length], reader.GetInt32(0).ToString());

                        else if (graphId == "27" )
                            piePiece = new OpenFlashChart.Charts.Pie.Piece(float.Parse(reader[1].ToString()), "Hr[" + reader.GetInt32(0) + "]", sliceColor[counter % sliceColor.Length], reader[1].ToString());
                        else if ( graphId == "31")
                            piePiece = new OpenFlashChart.Charts.Pie.Piece(reader.GetInt32(1), "Hr[" + reader.GetInt32(0) + "]", sliceColor[counter % sliceColor.Length], reader.GetInt32(1).ToString());

                        //piePiece = new OpenFlashChart.Charts.Pie.Piece(reader.GetInt32(1), "Hr[" + reader.GetInt32(0) + "]", sliceColor[counter % sliceColor.Length]);
                        else
                            piePiece = new OpenFlashChart.Charts.Pie.Piece(float.Parse(reader[0].ToString()), reader.GetInt32(1).ToString(), sliceColor[counter % sliceColor.Length], reader[1].ToString());
                        //piePiece = new OpenFlashChart.Charts.Pie.Piece(reader.GetInt32(0), reader.GetInt32(1).ToString(), sliceColor[counter % sliceColor.Length]);
                        counter++;
                        pie.Data.Add(piePiece);
                    }

                }
                if (graphId == "21" || graphId == "23" || graphId == "25")
                {
                    for (int i = 0; i < keys.Count; i++)
                    {

                        piePiece = new OpenFlashChart.Charts.Pie.Piece(float.Parse(ht[keys[i]].ToString()), keys[i].ToString() + "[" + Math.Round((decimal.Parse(ht[keys[i]].ToString()) / totalTrxn) * 100, 2) + "]", sliceColor[counter % sliceColor.Length], keys[i].ToString());
                        //piePiece = new OpenFlashChart.Charts.Pie.Piece(float.Parse(ht[keys[i]].ToString()), keys[i].ToString() + "[" + Math.Round((decimal.Parse(ht[keys[i]].ToString()) / totalTrxn) * 100, 2) + "]", sliceColor[counter % sliceColor.Length]);
                        counter++;
                        pie.Data.Add(piePiece);

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
                        piePiece = new OpenFlashChart.Charts.Pie.Piece(val, val.ToString() + " Utilization", sliceColor[counter % sliceColor.Length], "");
                        counter++;
                        pie.Data.Add(piePiece);

                    }
                    if (keys.Count > 0)
                    {
                        val = float.Parse((100 - decimal.Parse(ht[keys[0]].ToString())).ToString());
                        piePiece = new OpenFlashChart.Charts.Pie.Piece(val, val.ToString() + " Unutilized", sliceColor[counter % sliceColor.Length], "");
                        pie.Data.Add(piePiece);
                    }

                }

                chart.Pie = pie;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }



        //private void BuildGraph(string dataTitle, string storedProcedureName, string color)
        //{
        //    OpenFlashChart.Charts.ChartData chartData = new OpenFlashChart.Charts.Line(2, color, dataTitle, 10, 3);
        //    SqlDataReader reader = null;
        //    try
        //    {
        //        int max = 0;
        //        string lastValue = String.Empty;
        //        reader = GetReader(storedProcedureName, true);
        //        string atmName = String.Empty;
        //        while (reader.Read())
        //        {
        //            atmName = reader.GetString(1);
        //            if (reader.GetString(1) != lastValue)
        //            {
        //                chart.LabelsX.Add(reader.GetString(1));
        //                lastValue = atmName;
        //            }

        //            if (reader.GetInt32(0) > max)
        //                max = reader.GetInt32(0);

        //            chartData.Data.Add(reader.GetInt32(0));
        //        }
        //        maxYList.Add(max);
        //        chart.Data.Add(chartData);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //    finally
        //    {
        //        reader.Close();
        //    }
        //}

        //private void SetLinks(int limit, int type)
        //{
        //    for (int counter = 0; counter < limit; counter++)
        //    {
        //        if (counter == 0)
        //            chart.customString = "&links=javascript:window.location('webform1.aspx?type=" + type + "')";
        //        else
        //            chart.customString += ",javascript:window.location('webform1.aspx?type=" + type + "')";
        //    }
        //}

        protected void Page_Load(object sender, EventArgs e)
        {

            Response.CacheControl = "no-cache";
            try
            {
                int type = 1;
                string color = "";
                bool isAmountAnalysis = false;
                if (Request.QueryString["Parameters"] != null)
                    isAmountAnalysis = true;

                string[] parts = null;

                if (isAmountAnalysis)
                    parts = Request.QueryString["Parameters"].Split('@');
                else
                    parts = Request.QueryString["param"].Split('@');
                string isEJ = parts[parts.Length - 1];
                graphId = parts[0];
                fromDate = DateTime.ParseExact(parts[1], "dd/MM/yyyy", null);
                toDate = DateTime.ParseExact(parts[2], "dd/MM/yyyy", null);
                //atmId = parts[3];
                type = int.Parse(parts[3]);
                color = "#" + parts[4];
                //OpenFlashChart.Graph chart = new OpenFlashChart.Graph();

                if (graphId == "1")
                {
                    if (isAmountAnalysis)
                        InitGraph("", "Amount", "Hours");
                    else
                        InitGraph("", "Number Of Transactions", "Hours");
                    if (type == 2)
                    {
                        if (isEJ == "1")
                            BuildPieTest("", "EJTransactionVersusHour", color);
                        else
                        {
                            if (isAmountAnalysis)
                                BuildPieTest("", "AmountVersusHour", color);
                            else
                                BuildPieTest("", "TransactionVersusHour", color);
                        }
                    }
                    else
                    {
                        if (isEJ == "1")
                            BuildGraphData("", "EJTransactionVersusHour", true, color, true, type);
                        else
                        {
                            if (isAmountAnalysis)
                                BuildGraphData("", "AmountVersusHour", true, color, true, type);
                            else
                                BuildGraphData("", "TransactionVersusHour", true, color, true, type);
                        }
                    }
                    if (isAmountAnalysis)
                        chart.ToolTip = "#val# amount dispensed between #x_label#:00 and #x_label#:59";
                    else
                        chart.ToolTip = "#val# transactions occured between #x_label#:00 and #x_label#:59";
                }

                else if (graphId == "24")
                {
                    InitGraph("", "Utilization", "Week Day");
                    if (type == 2)
                    {
                        BuildPieTest("", "ATMUtilization", color);
                        chart.ToolTip = "#val# ";
                    }
                    else
                    {
                        BuildGraphData("", "ATMUtilization", true, color, true, type);
                        chart.ToolTip = "#val# utlization on #x_label#";
                    }
                }

                else if (graphId == "22")
                {
                    InitGraph("", "Number Of Replenishments", "Week Days");
                    if (isEJ == "1")
                        BuildGraphData("", "EJReplenishments", true, color, true, type);
                    else
                        BuildGraphData("", "Replenishments", true, color, true, type);
                    chart.ToolTip = "#val# replenishments occured on #x_label#";
                }

                else if (graphId == "20")
                {
                    lowerBound = int.Parse(parts[5]);
                    upperBound = int.Parse(parts[6]);

                    if (isAmountAnalysis)
                        InitGraph("", "Amount", "ATM");
                    else
                        InitGraph("", "Transactions", "ATM");

                    if (isEJ == "1")
                        BuildGraphData("", "EJATMTransactions", true, color, true, type);
                    else
                        if (isAmountAnalysis)
                            BuildGraphData("", "ATMTransactionsByAmount", true, color, true, type);
                        else
                            BuildGraphData("", "ATMTransactions", true, color, true, type);
                    if (isAmountAnalysis)
                        chart.ToolTip = "#val# amount dispensed on #x_label#";
                    else
                        chart.ToolTip = "#val# transactions occured on #x_label#";

                }
                else if (graphId == "21")
                {

                    InitGraph("", "Transactions", "ATM");
                    //BuildGraphData("", "ATMTransactions", true, color, true, type);
                    if (isEJ == "1")
                        BuildPieTest("", "EJRegionWiseTransaction", color);
                    else
                        if (isAmountAnalysis)
                            BuildPieTest("", "RegionWiseTransactionByAmount", color);
                        else
                            BuildPieTest("", "RegionWiseTransaction", color);
                    //chart.ToolTip = "#val# transactions occured between #x_label#:00 and #x_label#:59";

                }

                else if (graphId == "23")
                {

                    InitGraph("", "Transactions", "ATM");
                    //BuildGraphData("", "ATMTransactions", true, color, true, type);

                    BuildPieTest("", "RegionWiseReplenishment", color);
                    //chart.ToolTip = "#val# transactions occured between #x_label#:00 and #x_label#:59";

                }

                else if (graphId == "27")
                {
                    regionId = int.Parse(parts[5]);
                    if (isAmountAnalysis)
                        InitGraph("", "Amount", "ATM");
                    else
                        InitGraph("", "Transactions", "ATM");
                    //BuildGraphData("", "ATMTransactions", true, color, true, type);
                    if (isEJ == "1")
                        BuildPieTest("", "EJRegionTransactionsHourAnalysis", color);
                    else
                        if (isAmountAnalysis)
                            BuildPieTest("", "RegionTransactionsHourAnalysisByAmount", color);
                        else
                            BuildPieTest("", "RegionTransactionsHourAnalysis", color);
                    //chart.ToolTip = "#val# transactions occured between #x_label#:00 and #x_label#:59";

                }

                else if (graphId == "28")
                {
                    //regionId = int.Parse(parts[5]);
                    if (isAmountAnalysis)
                        InitGraph("", "Amount", "Region");
                    else
                        InitGraph("", "Transactions", "Region");
                    if (isEJ == "1")
                        BuildMultiRegionChart("EJMultiRegionTransactionAnalysis");
                    else
                        if (isAmountAnalysis)
                            BuildMultiRegionChart("MultiRegionTransactionAnalysisByAmount");
                        else
                            BuildMultiRegionChart("MultiRegionTransactionAnalysis");
                    //BuildGraphData("", "ATMTransactions", true, color, true, type);

                    //BuildPieTest("", "RegionTransactionsHourAnalysis", color);
                    if (isAmountAnalysis)
                        chart.ToolTip = "#val# amount dispensed on #x_label#";
                    else
                        chart.ToolTip = "#val# transactions occured on #x_label#";

                }
                else if (graphId == "29")
                {
                    //regionId = int.Parse(parts[5]);
                    if (isEJ == "1")
                    {
                        InitGraph("", "Replenishments", "Region");
                        BuildMultiRegionChart("MultiRegionReplenishmentAnalysis");
                    }
                    else
                    {
                        InitGraph("", "EJReplenishments", "Region");
                        BuildMultiRegionChart("EJMultiRegionReplenishmentAnalysis");
                    }
                    //BuildGraphData("", "ATMTransactions", true, color, true, type);

                    //BuildPieTest("", "RegionTransactionsHourAnalysis", color);
                    chart.ToolTip = "#val# replenishments occured on #x_label#";

                }
                else if (graphId == "30")
                {
                    //regionId = int.Parse(parts[5]);
                    InitGraph("", "Utlization", "Region");
                    BuildMultiRegionChart("MultiRegionUtilizationAnalysis");
                    //BuildGraphData("", "ATMTransactions", true, color, true, type);

                    //BuildPieTest("", "RegionTransactionsHourAnalysis", color);
                    chart.ToolTip = "#val# utilization on #x_label#";

                }




                else if (graphId == "25")
                {

                    InitGraph("", "Transactions", "ATM");
                    //BuildGraphData("", "ATMTransactions", true, color, true, type);

                    BuildPieTest("", "ATMUtilizationByRegion", color);
                    //chart.ToolTip = "#val# transactions occured between #x_label#:00 and #x_label#:59";

                }
                else if (graphId == "26")
                {
                    if (isAmountAnalysis)
                        InitGraph("", "Amount", "Day");
                    else
                        InitGraph("", "Number Of Transactions", "Day");
                    if (type == 2)
                    {
                        if (isEJ == "1")
                            BuildPieTest("", "EJWeeklyATMTransactions", color);
                        else
                        {
                            if (isAmountAnalysis)
                                BuildPieTest("", "WeeklyATMTransactionsByAmount", color);
                            else
                                BuildPieTest("", "WeeklyATMTransactions", color);
                        }
                    }
                    else
                    {
                        if (isEJ == "1")

                            BuildGraphData("", "EJWeeklyATMTransactions", true, color, true, type);
                        else
                        {
                            if (isAmountAnalysis)
                                BuildGraphData("", "WeeklyATMTransactionsByAmount", true, color, true, type);
                            else
                                BuildGraphData("", "WeeklyATMTransactions", true, color, true, type);

                        }
                    }
                    if (isAmountAnalysis)
                        chart.ToolTip = "#val# amount dispensed on #x_label#";
                    else
                        chart.ToolTip = "#val# transactions occured on #x_label#";
                    // WeeklyATMTransactions

                }
                else if (graphId == "31" || graphId == "32" || graphId == "33" || graphId == "34" || graphId == "35")
                {

                    if (type == 2)
                    {
                        if (graphId == "31")
                        {
                            InitGraph("", "Call Logs", "Hour");
                            BuildPieTest("", "CallOutOccurence", color);
                            chart.ToolTip = "#val# calls logs between #x_label#:00 and #x_label#:59";
                        }
                        else if (graphId == "32")
                        {
                            InitGraph("", "Call Logs", "Problem");
                            BuildPieTest("", "CallOutByProblem", color);
                            chart.ToolTip = "#val# calls logged";
                        }
                        else if (graphId == "33")
                        {
                            InitGraph("", "Call Logs", "Location");
                            BuildPieTest("", "CallOutByLocation", color);
                            chart.ToolTip = "#val# calls logged";
                        }
                        else if (graphId == "34")
                        {
                            InitGraph("", "Minutes", "Problem");
                            BuildPieTest("", "CallOutActualAvgResolution", color);
                            chart.ToolTip = "On average #val# minutes taken to resolve this issue";
                        }
                        else if (graphId == "35")
                        {
                            InitGraph("", "Hour", "Problem");
                            BuildPieTest("", "CallOutOverallAvgResolution", color);
                            chart.ToolTip = "On average #val# hour taken to resolve this issue";
                        }


                    }
                    else
                    {
                        if (graphId == "31")
                        {
                            InitGraph("", "Call Logs", "Hour");
                            BuildGraphData("", "CallOutOccurence", true, color, true, type);
                            chart.ToolTip = "#val# calls logs between #x_label#:00 and #x_label#:59";
                        }
                        else if (graphId == "32")
                        {
                            InitGraph("", "Call Logs", "Problem");
                            BuildGraphData("", "CallOutByProblem", true, color, true, type);
                            chart.ToolTip = "#val# calls logged";
                        }
                        else if (graphId == "33")
                        {
                            InitGraph("", "Call Logs", "Location");
                            BuildGraphData("", "CallOutByLocation", true, color, true, type);
                            chart.ToolTip = "#val# calls logged";
                        }
                        else if (graphId == "34")
                        {
                            InitGraph("", "Minutes", "Problem");
                            BuildGraphData("", "CallOutActualAvgResolution", true, color, true, type);
                            chart.ToolTip = "On average #val# minutes taken to resolve this issue";
                        }
                        else if (graphId == "35")
                        {
                            InitGraph("", "Hour", "Problem");
                            BuildGraphData("", "CallOutOverallAvgResolution", true, color, true, type);
                            chart.ToolTip = "On average #val# hour taken to resolve this issue";
                        }


                    }
                }



                if (maxYList.Count > 0)
                {
                    maxYList.Sort();

                    chart.MaxY = Convert.ToInt32(maxYList[maxYList.Count - 1].ToString().Split('.')[0]);
                    int mod = chart.MaxY % 5;
                    if (mod != 0)
                        chart.MaxY = chart.MaxY + (chart.StepsY - mod);
                    else//zubaer -> quick jugarh
                        chart.MaxY += 25;
                }
                Response.Write(chart.ToString());
            }
            catch (Exception ex)
            {
                LogableTask.LogMonoActivityTask("Graph", System.Reflection.MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Error, ex.Message);
                //throw ex;
                //  Session["Exception"] = ex.Message;

                //throw new Exception(ex.Message);
            }
        }
    }
}