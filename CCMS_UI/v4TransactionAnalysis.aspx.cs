using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.Web.UI.DataVisualization.Charting;

namespace CCMSUI
{
    public partial class v4TransactionAnalysis : System.Web.UI.Page
    {
        SqlCommand cmd;
        DateTime fromDate = DateTime.Now.AddDays(-1);
        DateTime toDate = DateTime.Now;
        int regionId = 0;
        string graphId = "";
        int lowerBound = 0;
        int upperBound = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            graphId = "1";
            //WeeklyATMTransactions
            List<double> yValues = new List<double>();
            List<string> xValues = new List<string>();
            SqlDataReader reader = GetReader("TransactionVersusHour", true);
            while (reader.Read())
            {
                xValues.Add(reader.GetInt32(1).ToString());
                yValues.Add(reader.GetInt32(0));
                Chart4.Series["Series1"].Points.AddXY(reader.GetInt32(1), reader.GetInt32(0));
            }
            
            Chart1.Series["Default"].Points.DataBindXY(xValues.ToArray(), yValues.ToArray());
            Chart2.Series["Series1"].Points.DataBindXY(xValues.ToArray(), yValues.ToArray());
            Chart3.Series["Series1"].Points.DataBindXY(xValues.ToArray(), yValues.ToArray());

            
//            Chart4.Series["Series1"].Points.AddY(yValues.ToArray());


            Chart1.Series["Default"].ChartType = SeriesChartType.Pie;
            Chart2.Series["Series1"].ChartType = SeriesChartType.Bar;
            Chart3.Series["Series1"].ChartType = SeriesChartType.Bar;
            

            Chart1.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = true;
            Chart1.Series[0]["PieDrawingStyle"] = "SoftEdge";
            Chart1.Series["Default"]["PieLabelStyle"] = "Outside";
            foreach (DataPoint point in Chart1.Series["Default"].Points)
            {
                point["Exploded"] = "false";
                if (point.AxisLabel == "3")
                {
                    point["Exploded"] = "true";
                }
            }
            reader.Close();
            cmd.Connection.Close();
            //this.Chart1.Legends[0].Enabled = true;
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
                    //if (excludeUser)
                    //    cmd.Parameters[0].Value = -1;
                    //else
                    //    cmd.Parameters[0].Value = 2;//(int)Session[SessionVars.user.ToString()];
                }
            }

            return cmd.ExecuteReader();
        }
    }
}