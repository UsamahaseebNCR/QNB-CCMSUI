using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using Avanza.CCMS.DAL;

using Avanza.iSuite.UI;
using System.Text;
using System.Drawing;
using System.Data;
using System.Web.UI.DataVisualization.Charting;

namespace CCMSUI
{
    public partial class ReplenishmentAnalysis : BasePage
    {
        protected void Chart1_Load(object sender, EventArgs e)
        {
            ChartWeeklyLast.Width = int.Parse(Session["width"].ToString()) / 3;
            ChartWeeklyLast.Height = int.Parse(Session["height"].ToString()) / 2;
        }

        protected void Chart2_Load(object sender, EventArgs e)
        {
            ChartWeeklyPrevious.Width = int.Parse(Session["width"].ToString()) / 3;
            ChartWeeklyPrevious.Height = int.Parse(Session["height"].ToString()) / 2;
        }

        protected void Chart3_Load(object sender, EventArgs e)
        {
            ChartWeeklyDuration.Width = int.Parse(Session["width"].ToString()) / 3;
            ChartWeeklyDuration.Height = int.Parse(Session["height"].ToString());
            divRegionComparison.Style.Add("height", ChartWeeklyDuration.Height.ToString());
        }

        protected void Chart4_Load(object sender, EventArgs e)
        {
            ChartRegion.Width = int.Parse(Session["width"].ToString()) / 2;
            ChartRegion.Height = int.Parse(Session["height"].ToString());
            divRepAnalysis.Style.Add("height", ChartRegion.Height.ToString());
        }

        protected void Chart15_Load(object sender, EventArgs e)
        {
            ChartMultiRegion.Width = int.Parse(Session["width"].ToString());
            ChartMultiRegion.Height = int.Parse(Session["height"].ToString());
        }

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
            LogableTask task = LogableTask.NewTask("DrawGraph");
            Response.CacheControl = "no-cache";
            try
            {
                if (id == "Button1")
                {
                    GenerateReplenishmentAnalysis(DateTime.Today.AddDays(-7).ToString("dd/MM/yyyy"), DateTime.Today.AddDays(-1).ToString("dd/MM/yyyy"), "Replenishments", "day", "total", ref ChartWeeklyLast);
                    GenerateReplenishmentAnalysis(DateTime.Today.AddDays(-14).ToString("dd/MM/yyyy"), DateTime.Today.AddDays(-8).ToString("dd/MM/yyyy"), "Replenishments", "day", "total", ref ChartWeeklyPrevious);
                    GenerateReplenishmentAnalysis(TextBox_From.Text, TextBox_To.Text, "Replenishments", "day", "total", ref ChartWeeklyDuration);

                    Literal1.Text = "<b>Last Week [" + DateTime.Today.AddDays(-7).ToString("dd/MM/yyyy") + "-" + DateTime.Today.AddDays(-1).ToString("dd/MM/yyyy") + "]</b><br/>Replenishments for last week";
                    Literal2.Text = "<b>Previous Week [" + DateTime.Today.AddDays(-14).ToString("dd/MM/yyyy") + "-" + DateTime.Today.AddDays(-8).ToString("dd/MM/yyyy") + "]</b><br/>Replenishments for previous week"; ;
                    Literal3.Text = "<b>Selected Criteria [" + TextBox_From.Text + "-" + TextBox4.Text + "]</b><br/>Replenishments for specified duration"; ;
                    BindWeeklyRepleninshmentAnalysisGrid((string)ViewState["orderby1"]);
                }
                else if (id == "Button2")
                {
                    Literal4.Text = "<b>Selected Criteria [" + TextBox3.Text + "-" + TextBox4.Text + "]</b><br/>Region wise breakup of replenishments for specified duration";
                    GenerateReplenishmentAnalysis(TextBox3.Text, TextBox4.Text, "RegionWiseReplenishment", "region_name", "total", ref ChartRegion);
                    BindRegionRepleninshmentAnalysisGrid((string)ViewState["orderby"]);
                }
                else if (id == "Button7")
                {
                    GenerateReplenishmentAnalysisMultiRegion(TextBox9.Text, TextBox10.Text, "MultiRegionReplenishmentAnalysis", "date", "total", ref ChartMultiRegion);
                    Literal15.Text = "<b>Selected Criteria [" + TextBox9.Text + "-" + TextBox10.Text + "]</b><br/>Multi Region Analysis";
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

        private void GenerateReplenishmentAnalysis(string fromDate, string toDate, string spName, string xAxisTag, string yAxisTag, ref Chart chart, long? region_id = null)
        {
            SqlConnection conn = ConnectionFactory.GetNewConnection();
            SqlCommand cmd;
            SqlDataAdapter adapter;
            try
            {
                chart.Series[0].Points.Clear();
                cmd = conn.CreateCommand();
                InitializeCmd(fromDate, toDate, spName, ref cmd, region_id);
                adapter = new System.Data.SqlClient.SqlDataAdapter(cmd);
                DataSet dtTrxn = new DataSet();
                adapter.Fill(dtTrxn);
                DataTable ChartData = dtTrxn.Tables[0];
                string[] XPointMember = new string[ChartData.Rows.Count];
                int[] YPointMember = new int[ChartData.Rows.Count];
                for (int count = 0; count < ChartData.Rows.Count; count++)
                {
                    //storing Values for X axis  
                    XPointMember[count] = ChartData.Rows[count][xAxisTag].ToString();
                    //storing values for Y Axis  
                    YPointMember[count] = Convert.ToInt32(ChartData.Rows[count][yAxisTag]);
                }
                //binding chart control  
                chart.Series[0].Points.DataBindXY(XPointMember, YPointMember);

                chart.Series[0].BorderWidth = 1;
                chart.Series[0].BorderDashStyle = ChartDashStyle.Solid;
                chart.Series[0].BorderColor = System.Drawing.Color.FromArgb(200, 26, 59, 105);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }
        }

        private void GenerateReplenishmentAnalysisMultiRegion(string fromDate, string toDate, string spName, string xAxisTag, string yAxisTag, ref Chart chart)
        {
            SqlConnection conn = ConnectionFactory.GetNewConnection();
            SqlCommand cmd;
            SqlDataAdapter adapter;
            try
            {
                cmd = conn.CreateCommand();
                chart.Series.Clear();
                DataTable ChartData;
                DataSet dtTrxn;
                Avanza.CCMS.DAL.Region.RegionReader reader = Avanza.CCMS.DAL.Region.ExecuteReader("is_organization = 0");
                while (reader.Read())
                {
                    InitializeCmd(fromDate, toDate, spName, ref cmd, reader.CurrentRegion.RegionId);
                    adapter = new System.Data.SqlClient.SqlDataAdapter(cmd);
                    dtTrxn = new DataSet();
                    adapter.Fill(dtTrxn);
                    ChartData = dtTrxn.Tables[0];
                    string[] XPointMember = new string[ChartData.Rows.Count];
                    int[] YPointMember = new int[ChartData.Rows.Count];
                    for (int count = 0; count < ChartData.Rows.Count; count++)
                    {
                        XPointMember[count] = ChartData.Rows[count][xAxisTag].ToString();
                        YPointMember[count] = Convert.ToInt32(ChartData.Rows[count][yAxisTag]);
                    }
                    chart.Series.Add(new Series(reader.CurrentRegion.RegionName));
                    chart.Series[reader.CurrentRegion.RegionName].Points.DataBindXY(XPointMember, YPointMember);
                    chart.Series[reader.CurrentRegion.RegionName].ToolTip = reader.CurrentRegion.RegionName;
                    chart.Series[reader.CurrentRegion.RegionName].BorderWidth = 1;
                    chart.Series[reader.CurrentRegion.RegionName].BorderDashStyle = ChartDashStyle.Solid;
                    chart.Series[reader.CurrentRegion.RegionName].BorderColor = System.Drawing.Color.FromArgb(200, 26, 59, 105);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }
        }

        private void InitializeCmd(string fromDate, string toDate, string spName, ref SqlCommand cmd, long? region_id = null)
        {
            if (cmd == null)
                return;

            cmd.CommandText = spName;
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Clear();
            cmd.Parameters.Add("@FromDate", SqlDbType.DateTime);
            cmd.Parameters[0].Value = DateTime.ParseExact(fromDate, "dd/MM/yyyy", null);
            cmd.Parameters.Add("@ToDate", SqlDbType.DateTime);
            cmd.Parameters[1].Value = DateTime.ParseExact(toDate, "dd/MM/yyyy", null);

            switch (spName)
            {
                case "Replenishments":
                    cmd.Parameters.Add("@AtmId", SqlDbType.VarChar);
                    cmd.Parameters[2].Value = Session[SessionVars.SelectedATMs.ToString()].ToString();
                    break;
                case "RegionWiseReplenishment":
                    cmd.Parameters.Add("@AtmIds", SqlDbType.VarChar);
                    cmd.Parameters[2].Value = Session[SessionVars.SelectedATMs.ToString()];
                    break;
                case "MultiRegionReplenishmentAnalysis":
                    cmd.Parameters.Add("@RegionId", SqlDbType.BigInt);
                    cmd.Parameters[2].Value = region_id;
                    break;
                default:
                    break;
            }

        }

        protected void Button_Submit_Click(object sender, EventArgs e)
        {
            GenerateGraph(((System.Web.UI.WebControls.Button)sender).ID);
        }

        protected void GridView_RepAnalysis_Sorting(object sender, GridViewSortEventArgs e)
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
            GridView1.DataSource = dt;
            GridView1.DataBind();
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
"and atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] +")" +

" group by title,convert(varchar,rep_datetime,103),region_name order by " + orderBy;

            SqlDataAdapter adapter = new System.Data.SqlClient.SqlDataAdapter(cmd);
            System.Data.DataTable dt = new System.Data.DataTable();
            adapter.Fill(dt);
            GridView_RepAnalysis.DataSource = dt;
            GridView_RepAnalysis.DataBind();
        }

    }
}