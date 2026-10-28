using Avanza.CCMS.DAL;
using Avanza.iSuite.DAL;
using Avanza.iSuite.UI;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.WebControls;

namespace CCMSUI.admin
{
    public partial class AddEditDashboardGraph : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                try
                {
                    Page.SetFocus(TextBox_Name);
                    Avanza.CCMS.DAL.DashboardTemplate.DashboardTemplateReader templateReader = 
                        Avanza.CCMS.DAL.DashboardTemplate.ExecuteReader("is_active = 1");
                    DropDownList_Dashboard.Items.Add(new ListItem("Select", ""));
                    while (templateReader.Read())
                    {
                        DropDownList_Dashboard.Items.Add(new ListItem(templateReader.CurrentDashboardTemplate.Title.ToString(), 
                            templateReader.CurrentDashboardTemplate.Id.ToString()));

                    }
                    templateReader.Close();

                    if (Request.QueryString["dashboard_id"] != null)
                        DropDownList_Dashboard.SelectedValue = Request.QueryString["dashboard_id"];

                    if (Request.QueryString["id"] == null)
                    {
                        RadioButton_Active.SelectedIndex = 0;
                        TextBox_BackColor.Text = "#ffffff";
                    }
                    else
                    {
                        Avanza.CCMS.DAL.DashboardGraph graph = Avanza.CCMS.DAL.DashboardGraph.
                            LoadDashboardGraphByPk(int.Parse(Request.QueryString["id"]));
                        DropDownList_Dashboard.SelectedValue = graph.DashboardId.ToString();
                        TextBox_Name.Text = graph.Name;
                        TextBox_Query.Text = graph.Query.Replace('"', '\'');
                        DropDownList_RowNumber.SelectedValue = graph.RowNumber.ToString();
                        DropDownList_Type.SelectedValue = graph.Type.ToString();
                        TextBox_XValue.Text = graph.XValue.ToString();
                        TextBox_YValue.Text = graph.YValue.ToString();
                        TextBox_BackColor.Text = string.IsNullOrEmpty(graph.BackgroundColor) ? "#ffffff" : graph.BackgroundColor;
                        RadioButton_Active.SelectedIndex = graph.IsActive ? 0 : 1;
                    }
                }
                catch (Exception ex)
                {
                    Literal_error.Text = "<li>" + ex.Message + "</li>";
                }
            }
        }

        protected void Button_SaveUpdateTemplate_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("AddDashboardTemplate");
            SqlConnection conn = null;
            SqlTransaction trx = null;
            Avanza.CCMS.DAL.DashboardGraph template;
            try
            {
                conn = ConnectionFactory.GetNewConnection();
                conn.Open();
                trx = conn.BeginTransaction();
                if (Request.QueryString["id"] != null)
                {
                    template = Avanza.CCMS.DAL.DashboardGraph.LoadDashboardGraphByPk(int.Parse(Request.QueryString["id"]));
                    template.ModifiedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                    template.ModifiedAt = DateTime.Now;
                }
                else
                {
                    template = new Avanza.CCMS.DAL.DashboardGraph();
                    template.CreatedAt = DateTime.Now;
                    template.CreatedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                }

                object res = ConnectionFactory.ExecuteScalar("select top 1 name from dashboard_graph where name='" + 
                    TextBox_Name.Text + "' and dashboard_id = " + DropDownList_Dashboard.SelectedValue
                    + ((Request.QueryString["id"] != null) ? " and id != " + template.Id : ""));
                if (res != null)
                {
                    Literal_error.Text = "<li>This name is already defined for dashboard content '" + res + "'. Please, provide different dashboard content name.</li>";
                    return;
                }

                res = ConnectionFactory.ExecuteScalar("select COUNT(*) from dashboard_graph where row_number=" +
                    DropDownList_RowNumber.SelectedValue + " and dashboard_id = " + DropDownList_Dashboard.SelectedValue
                    + ((Request.QueryString["id"] != null) ? " and id != " + template.Id : ""));
                if (res != null && (int)res >= 3)
                {
                    Literal_error.Text = "<li>Max 3 dashboards can be initialized in a row. Please, select different row.</li>";
                    return;
                }

                var command = TextBox_Query.Text.Split(';')[0];
                if (!IsValidQuery(command))
                    throw new Exception("Only query expression is allowed");

                template.Name = TextBox_Name.Text;
                template.DashboardId = Convert.ToInt32(DropDownList_Dashboard.SelectedValue);
                template.Query = command.Replace('\'', '"').Trim();
                template.RowNumber = Convert.ToInt32(DropDownList_RowNumber.SelectedValue);
                template.Type = DropDownList_Type.SelectedValue;
                template.XValue = TextBox_XValue.Text.Trim();
                template.YValue = TextBox_YValue.Text.Trim();
                template.BackgroundColor = TextBox_BackColor.Text.Trim();
                template.IsActive = (RadioButton_Active.SelectedIndex == 0) ? true : false;
                template.Save(conn, trx);

                if (Request.QueryString["id"] == null)
                    Utility.BuildAuditLog(template.Name + " created", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.CreateCIT);
                else
                    Utility.BuildAuditLog(template.Name + " modified", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.ModifyCIT);

                if (trx != null)
                {
                    trx.Commit();
                }

                Response.Redirect("dashboardGraphtemplate.aspx", false);
            }
            catch (Exception ex)
            {
                if (trx != null)
                {
                    trx.Rollback();
                }

                Literal_error.Text = "<li>An error occured while saving template." + ex.Message + "</li>";
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            {
                if (conn != null)
                    conn.Close();
                task.EndTask();
            }
        }

        protected void Button_PreviewQuery_Click(object sender, EventArgs e)
        {
            try
            {
                if (DropDownList_Type.SelectedValue == "")
                    throw new Exception("Select graph type");
                
                if(string.IsNullOrEmpty(TextBox_XValue.Text))
                    throw new Exception("Enter X Value");

                if (string.IsNullOrEmpty(TextBox_YValue.Text))
                    throw new Exception("Enter Y Value");

                if (string.IsNullOrEmpty(TextBox_Query.Text))
                    throw new Exception("Enter query");

                System.Data.DataTable dt = new System.Data.DataTable();
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                var command = TextBox_Query.Text.Split(';')[0];
                if (!IsValidQuery(command))
                    throw new Exception("Only query expression is allowed");

                cmd.CommandText = command;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);

                Chart Chart1 = new Chart();
                Chart1.Series.Add(new Series());
                for (int x = 0; x < dt.Rows.Count; x++)
                {
                    Chart1.Series[0].Points.AddXY(dt.Rows[x][TextBox_XValue.Text], dt.Rows[x][TextBox_YValue.Text]);
                }

                Chart1.Series[0].IsValueShownAsLabel = true;
                Chart1.Series[0].ChartType = GetChartType(DropDownList_Type.SelectedValue);
                Chart1.ChartAreas.Add(new ChartArea());
                Chart1.Legends.Add(new Legend());
                Chart1.Legends[0].Enabled = DropDownList_Type.SelectedValue == "PieChart";

                this.chart.Controls.Add(Chart1);
                preview.Visible = true;
            }
            catch (Exception ex)
            {
                Literal_error.Text = ex.Message;
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
                default:
                    return System.Web.UI.DataVisualization.Charting.SeriesChartType.Bar;
            }
        }

        private bool IsValidQuery(string command)
        {
            string[] blockKeyWords = new string[] { "INSERT", "UPDATE", "DELETE", "DROP", "CREATE", "ALTER" };
            foreach (string x in blockKeyWords)
            {
                if (command.Contains(x))
                {
                    return false;
                }
            }

            return true;
        }
    }
}