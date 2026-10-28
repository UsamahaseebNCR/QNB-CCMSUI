using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;

using Avanza.iSuite.UI;
using System.Text;

namespace CCMSUI
{
    public partial class CalOutAnalysis : BasePage
    {
        protected void Chart1_Load(object sender, EventArgs e)
        {
            Chart1.Width = int.Parse(Session["width"].ToString()) / 3;
            Chart1.Height = int.Parse(Session["height"].ToString()) / 2;
        }
        protected void Chart2_Load(object sender, EventArgs e)
        {
            Chart2.Width = int.Parse(Session["width"].ToString()) / 3;
            Chart2.Height = int.Parse(Session["height"].ToString()) / 2;
        }
        protected void Chart3_Load(object sender, EventArgs e)
        {
            Chart3.Width = int.Parse(Session["width"].ToString()) / 2;
            Chart3.Height = int.Parse(Session["height"].ToString());
        }
        protected void Chart4_Load(object sender, EventArgs e)
        {
            Chart4.Width = int.Parse(Session["width"].ToString()) / 3;
            Chart4.Height = int.Parse(Session["height"].ToString());
            //divUtilizationComparison.Style.Add("height", Chart4.Height.ToString());
        }
        protected void Chart5_Load(object sender, EventArgs e)
        {
            Chart5.Width = int.Parse(Session["width"].ToString()) / 3;
            Chart5.Height = int.Parse(Session["height"].ToString());
            Chart5.Attributes.Add("wmode", "opaque");
        }
        protected void Chart6_Load(object sender, EventArgs e)
        {
            Chart6.Width = int.Parse(Session["width"].ToString()) / 3;
            Chart6.Height = int.Parse(Session["height"].ToString()) / 2;
        }
        protected void Chart7_Load(object sender, EventArgs e)
        {
            Chart7.Width = int.Parse(Session["width"].ToString()) / 3;
            Chart7.Height = int.Parse(Session["height"].ToString()) / 2;
        }
        protected void Chart8_Load(object sender, EventArgs e)
        {
            Chart8.Width = int.Parse(Session["width"].ToString()) / 3;
            Chart8.Height = int.Parse(Session["height"].ToString());
            //divUtilizationComparison.Style.Add("height", Chart4.Height.ToString());
        }
        protected void Chart9_Load(object sender, EventArgs e)
        {
            Chart9.Width = int.Parse(Session["width"].ToString()) / 3;
            Chart9.Height = int.Parse(Session["height"].ToString());

        }
        protected void Chart10_Load(object sender, EventArgs e)
        {
            Chart10.Width = int.Parse(Session["width"].ToString()) / 3;
            Chart10.Height = int.Parse(Session["height"].ToString()) / 2;
        }
        protected void Chart11_Load(object sender, EventArgs e)
        {
            Chart11.Width = int.Parse(Session["width"].ToString()) / 3;
            Chart11.Height = int.Parse(Session["height"].ToString()) / 2;
        }
        protected void Chart12_Load(object sender, EventArgs e)
        {
            Chart12.Width = int.Parse(Session["width"].ToString()) / 3;
            Chart12.Height = int.Parse(Session["height"].ToString());
            //divUtilizationComparison.Style.Add("height", Chart4.Height.ToString());
        }
        protected void Chart13_Load(object sender, EventArgs e)
        {
            Chart13.Width = int.Parse(Session["width"].ToString()) / 3;
            Chart13.Height = int.Parse(Session["height"].ToString());

        }
        protected void Chart14_Load(object sender, EventArgs e)
        {
            Chart14.Width = int.Parse(Session["width"].ToString()) / 3;
            Chart14.Height = int.Parse(Session["height"].ToString()) / 2;
        }
        protected void Chart15_Load(object sender, EventArgs e)
        {
            Chart15.Width = int.Parse(Session["width"].ToString()) / 3;
            Chart15.Height = int.Parse(Session["height"].ToString()) / 2;
        }
        protected void Chart16_Load(object sender, EventArgs e)
        {
            Chart16.Width = int.Parse(Session["width"].ToString()) / 3;
            Chart16.Height = int.Parse(Session["height"].ToString());
            //divUtilizationComparison.Style.Add("height", Chart4.Height.ToString());
        }
        protected void Chart17_Load(object sender, EventArgs e)
        {
            Chart17.Width = int.Parse(Session["width"].ToString()) / 3;
            Chart17.Height = int.Parse(Session["height"].ToString());

        }
        protected void Chart18_Load(object sender, EventArgs e)
        {
            Chart18.Width = int.Parse(Session["width"].ToString()) / 3;
            Chart18.Height = int.Parse(Session["height"].ToString()) / 2;
        }
        protected void Chart19_Load(object sender, EventArgs e)
        {
            Chart19.Width = int.Parse(Session["width"].ToString()) / 3;
            Chart19.Height = int.Parse(Session["height"].ToString()) / 2;
        }
        protected void Chart20_Load(object sender, EventArgs e)
        {
            Chart20.Width = int.Parse(Session["width"].ToString()) / 3;
            Chart20.Height = int.Parse(Session["height"].ToString());
            //divUtilizationComparison.Style.Add("height", Chart4.Height.ToString());
        }

        //protected void Chart15_Load(object sender, EventArgs e)
        //{
        //    Chart15.Width = int.Parse(Session["width"].ToString());
        //    Chart15.Height = int.Parse(Session["height"].ToString());

        //}
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                //ViewState["SortExpression"] = "title ";
                //ViewState["sortDirection"] = "asc";
                //ViewState["orderby"] = ViewState["SortExpression"] + " " + ViewState["sortDirection"];

                //ViewState["SortExpression1"] = "title ";
                //ViewState["sortDirection1"] = "asc";
                //ViewState["orderby1"] = ViewState["SortExpression1"] + " " + ViewState["sortDirection1"];

                TextBox6.Text = TextBox2.Text = TextBox4.Text = TextBox10.Text = TextBox_To.Text = DateTime.Now.ToString("dd/MM/yyyy");
                TextBox5.Text = TextBox1.Text = TextBox3.Text = TextBox9.Text = TextBox_From.Text = DateTime.Now.AddDays(-7).ToString("dd/MM/yyyy");

            }

            //Chart1.Url = "Data.aspx?param=" + Request.QueryString["gid"] + "@" + DateTime.Today.AddDays(-7).ToString("dd/MM/yyyy") + "@" + DateTime.Today.AddDays(-1).ToString("dd/MM/yyyy") + "@0@0@167AD4";
        }

        private void GenerateGraph(string id)
        {
            LogableTask task = LogableTask.NewTask("DrawGraph");
            Response.CacheControl = "no-cache";
            try
            {
                if (id == "Button1")
                {
                    Chart3.Url = "Data.aspx?param=31@" + TextBox_From.Text + "@" + TextBox_To.Text + "@1@167AD4";
                    Chart1.Url = "Data.aspx?param=31@" + DateTime.Today.AddDays(-7).ToString("dd/MM/yyyy") + "@" + DateTime.Today.AddDays(-1).ToString("dd/MM/yyyy") + "@0@167AD4";
                    Chart2.Url = "Data.aspx?param=31@" + DateTime.Today.AddDays(-14).ToString("dd/MM/yyyy") + "@" + DateTime.Today.AddDays(-8).ToString("dd/MM/yyyy") + "@0@189570";
                    Chart5.Url = "Data.aspx?param=31@" + DateTime.Today.ToString("dd/MM/yyyy") + "@" + DateTime.Today.ToString("dd/MM/yyyy") + "@2@167AD4";

                    Literal1.Text = "<b>Last Week [" + DateTime.Today.AddDays(-7).ToString("dd/MM/yyyy") + "-" + DateTime.Today.AddDays(-1).ToString("dd/MM/yyyy") + "]</b><br/>Call Out for last week";
                    Literal2.Text = "<b>Previous Week [" + DateTime.Today.AddDays(-14).ToString("dd/MM/yyyy") + "-" + DateTime.Today.AddDays(-8).ToString("dd/MM/yyyy") + "]</b><br/>Call Out for previous week"; ;
                    Literal3.Text = "<b>Selected Criteria [" + TextBox_From.Text + "-" + TextBox_To.Text + "]</b><br/>Call Out for specified duration";
                    Literal5.Text = "<b>Today [" + DateTime.Now.ToString("dd/MM/yyyy") + "]</b><br/>Call Out for today";
                    //BindWeeklyRepleninshmentAnalysisGrid((string)ViewState["orderby1"]);

                }
                else if (id == "Button2")
                {
                    Chart4.Url = "Data.aspx?param=32@" + DateTime.Today.ToString("dd/MM/yyyy") + "@" + DateTime.Today.ToString("dd/MM/yyyy") + "@2@167AD4";
                    Chart6.Url = "Data.aspx?param=32@" + DateTime.Today.AddDays(-7).ToString("dd/MM/yyyy") + "@" + DateTime.Today.AddDays(-1).ToString("dd/MM/yyyy") + "@0@167AD4";
                    Chart7.Url = "Data.aspx?param=32@" + DateTime.Today.AddDays(-14).ToString("dd/MM/yyyy") + "@" + DateTime.Today.AddDays(-8).ToString("dd/MM/yyyy") + "@0@189570";
                    Chart8.Url = "Data.aspx?param=32@" + TextBox_From.Text + "@" + TextBox_To.Text + "@1@167AD4";

                    Literal4.Text = "<b>Today [" + DateTime.Now.ToString("dd/MM/yyyy") + "]</b><br/>Call Out for today"; 
                    Literal6.Text = "<b>Last Week [" + DateTime.Today.AddDays(-7).ToString("dd/MM/yyyy") + "-" + DateTime.Today.AddDays(-1).ToString("dd/MM/yyyy") + "]</b><br/>Call Out for last week";
                    Literal7.Text = "<b>Previous Week [" + DateTime.Today.AddDays(-14).ToString("dd/MM/yyyy") + "-" + DateTime.Today.AddDays(-8).ToString("dd/MM/yyyy") + "]</b><br/>Call Out for previous week"; ;
                    Literal8.Text = "<b>Selected Criteria [" + TextBox_From.Text + "-" + TextBox_To.Text + "]</b><br/>Call Out for specified duration";
                    

                    //BindRegionRepleninshmentAnalysisGrid((string)ViewState["orderby"]);

                }
                else if (id == "Button7")
                {
                    Chart9.Url = "Data.aspx?param=33@" + DateTime.Today.ToString("dd/MM/yyyy") + "@" + DateTime.Today.ToString("dd/MM/yyyy") + "@2@167AD4";
                    Chart10.Url = "Data.aspx?param=33@" + DateTime.Today.AddDays(-7).ToString("dd/MM/yyyy") + "@" + DateTime.Today.AddDays(-1).ToString("dd/MM/yyyy") + "@0@167AD4";
                    Chart11.Url = "Data.aspx?param=33@" + DateTime.Today.AddDays(-14).ToString("dd/MM/yyyy") + "@" + DateTime.Today.AddDays(-8).ToString("dd/MM/yyyy") + "@0@189570";
                    Chart12.Url = "Data.aspx?param=33@" + TextBox_From.Text + "@" + TextBox_To.Text + "@1@167AD4";

                    Literal9.Text = "<b>Today [" + DateTime.Now.ToString("dd/MM/yyyy") + "]</b><br/>Call Out for today"; ;
                    Literal10.Text = "<b>Last Week [" + DateTime.Today.AddDays(-7).ToString("dd/MM/yyyy") + "-" + DateTime.Today.AddDays(-1).ToString("dd/MM/yyyy") + "]</b><br/>Call Out for last week";
                    Literal11.Text = "<b>Previous Week [" + DateTime.Today.AddDays(-14).ToString("dd/MM/yyyy") + "-" + DateTime.Today.AddDays(-8).ToString("dd/MM/yyyy") + "]</b><br/>Call Out for previous week"; ;
                    Literal12.Text = "<b>Selected Criteria [" + TextBox_From.Text + "-" + TextBox_To.Text + "]</b><br/>Call Out for specified duration";
                    
                }
                else if (id == "Button3")
                {
                    Chart13.Url = "Data.aspx?param=34@" + DateTime.Today.ToString("dd/MM/yyyy") + "@" + DateTime.Today.ToString("dd/MM/yyyy") + "@2@167AD4";
                    Chart14.Url = "Data.aspx?param=34@" + DateTime.Today.AddDays(-7).ToString("dd/MM/yyyy") + "@" + DateTime.Today.AddDays(-1).ToString("dd/MM/yyyy") + "@0@167AD4";
                    Chart15.Url = "Data.aspx?param=34@" + DateTime.Today.AddDays(-14).ToString("dd/MM/yyyy") + "@" + DateTime.Today.AddDays(-8).ToString("dd/MM/yyyy") + "@0@189570";
                    Chart16.Url = "Data.aspx?param=34@" + TextBox_From.Text + "@" + TextBox_To.Text + "@1@167AD4";

                    Literal3.Text = "<b>Today [" + DateTime.Now.ToString("dd/MM/yyyy") + "]</b><br/>Call Out for today"; ;
                    Literal14.Text = "<b>Last Week [" + DateTime.Today.AddDays(-7).ToString("dd/MM/yyyy") + "-" + DateTime.Today.AddDays(-1).ToString("dd/MM/yyyy") + "]</b><br/>Call Out for last week";
                    Literal15.Text = "<b>Previous Week [" + DateTime.Today.AddDays(-14).ToString("dd/MM/yyyy") + "-" + DateTime.Today.AddDays(-8).ToString("dd/MM/yyyy") + "]</b><br/>Call Out for previous week"; ;
                    Literal16.Text = "<b>Selected Criteria [" + TextBox_From.Text + "-" + TextBox_To.Text + "]</b><br/>Call Out for specified duration";

                }
                else if (id == "Button4")
                {
                    Chart17.Url = "Data.aspx?param=35@" + DateTime.Today.ToString("dd/MM/yyyy") + "@" + DateTime.Today.ToString("dd/MM/yyyy") + "@2@167AD4";
                    Chart18.Url = "Data.aspx?param=35@" + DateTime.Today.AddDays(-7).ToString("dd/MM/yyyy") + "@" + DateTime.Today.AddDays(-1).ToString("dd/MM/yyyy") + "@0@167AD4";
                    Chart19.Url = "Data.aspx?param=35@" + DateTime.Today.AddDays(-14).ToString("dd/MM/yyyy") + "@" + DateTime.Today.AddDays(-8).ToString("dd/MM/yyyy") + "@0@189570";
                    Chart20.Url = "Data.aspx?param=35@" + TextBox_From.Text + "@" + TextBox_To.Text + "@1@167AD4";

                    Literal17.Text = "<b>Today [" + DateTime.Now.ToString("dd/MM/yyyy") + "]</b><br/>Call Out for today"; ;
                    Literal18.Text = "<b>Last Week [" + DateTime.Today.AddDays(-7).ToString("dd/MM/yyyy") + "-" + DateTime.Today.AddDays(-1).ToString("dd/MM/yyyy") + "]</b><br/>Call Out for last week";
                    Literal19.Text = "<b>Previous Week [" + DateTime.Today.AddDays(-14).ToString("dd/MM/yyyy") + "-" + DateTime.Today.AddDays(-8).ToString("dd/MM/yyyy") + "]</b><br/>Call Out for previous week"; ;
                    Literal20.Text = "<b>Selected Criteria [" + TextBox_From.Text + "-" + TextBox_To.Text + "]</b><br/>Call Out for specified duration";

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

        //protected void GridView_CashUtilizationAnalysis_Sorting(object sender, GridViewSortEventArgs e)
        //{
        //    if (ViewState["SortExpression"].ToString().IndexOf(e.SortExpression) >= 0)
        //    {
        //        if (ViewState["sortDirection"].ToString() == " asc")
        //            ViewState["sortDirection"] = " desc";
        //        else
        //            ViewState["sortDirection"] = " asc";
        //    }
        //    else
        //    {
        //        ViewState["SortExpression"] = e.SortExpression;
        //        ViewState["sortDirection"] = " asc";
        //    }


        //    ViewState["orderby"] = ViewState["SortExpression"] + " " + ViewState["sortDirection"];
        //    ///BindGrid(null);
        //    //BindRegionRepleninshmentAnalysisGrid((string)ViewState["orderby"]);
        //}

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

        //        private void BindRegionRepleninshmentAnalysisGrid(string orderBy)
        //        {
        //            SqlConnection conn = ConnectionFactory.GetNewConnection();
        //            SqlCommand cmd = conn.CreateCommand();
        //            cmd.CommandText = @"select title, convert(varchar,rep_datetime,103) as rep_dateTime, region_name, 
        //        sum(cash_added1*denomination_type_1+cash_added2*denomination_type_2+cash_added3*denomination_type_3+
        //cash_added4*denomination_type_4+cash_added5*denomination_type_5+cash_added6*denomination_type_6+cash_added7*denomination_type_7) as amount
        //from replenishment inner join atm on replenishment.atm_id = atm.atm_id 
        //inner join region on atm.region_id = region.region_id 
        //inner join note_set_type on atm.note_set_type_id = note_set_type.note_set_type_id
        //where rep_datetime>=convert(datetime,'" + TextBox3.Text + "',103) and rep_datetime<=convert(datetime,'" + TextBox4.Text + " 23:59:59',103) " +

        //" group by title,convert(varchar,rep_datetime,103),region_name order by " + orderBy;

        //            SqlDataAdapter adapter = new System.Data.SqlClient.SqlDataAdapter(cmd);
        //            System.Data.DataTable dt = new System.Data.DataTable();
        //            adapter.Fill(dt);

        //            //GridView_RepAnalysis.DataSource = dt;
        //            //GridView_RepAnalysis.DataBind();
        //        }

    }
}