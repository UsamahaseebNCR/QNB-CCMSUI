using Avanza.CCMS.DAL;
using Avanza.iSuite.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CCMSUI.CCMS
{
    public partial class CustomReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                try
                {
                    if (Request.QueryString["id"] == null)
                    {
                    }
                    else
                    {
                        ReportTemplate reportTemplate = ReportTemplate.LoadReportTemplateByPk(int.Parse(Request.QueryString["id"]));
                        System.Data.DataTable dt = new System.Data.DataTable();
                        SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                        cmd.CommandText = reportTemplate.ReportQuery.Replace('"', '\'');
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(dt);
                        GridView_Result.DataSource = dt;
                        GridView_Result.DataBind();
                        ReportHeader.Text = reportTemplate.ReportName;
                        ReportTitle.Text = reportTemplate.ReportName;
                        if (GridView_Result.Rows.Count > 0)
                        {
                            //This replaces <td> with <th> and adds the scope attribute
                            GridView_Result.UseAccessibleHeader = true;

                            //This will add the <thead> and <tbody> elements
                            GridView_Result.HeaderRow.TableSection = TableRowSection.TableHeader;

                            //This adds the <tfoot> element. 
                            //Remove if you don't have a footer row
                            GridView_Result.FooterRow.TableSection = TableRowSection.TableFooter;
                        }
                    }
                }
                catch (Exception ex)
                {
                    //Literal_error.Text = "<li>" + ex.Message + "</li>";
                }

            }
        }
    }
}