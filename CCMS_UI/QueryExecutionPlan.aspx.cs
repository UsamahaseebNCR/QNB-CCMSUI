
using System;
using System.Data;
using System.Data.SqlClient;

namespace CCMSUI
{
    public partial class QueryExecutionPlan : System.Web.UI.Page
    {
        protected void getActualPlan_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(query.Text))
            {
                errorMessage.Visible = true;
                errorMessage.InnerText = "Enter your query to continue.";
            }
            else
            {
                var command = ConnectionFactory.GetNewCommand(true);
                try
                {
                    command.CommandTimeout = 0;
                    command.CommandText = @"SET STATISTICS XML ON;";
                    command.ExecuteNonQuery();

                    command.CommandText = query.Text;
                    var commandDataSet = new DataSet();
                    var commandDataAdapter = new SqlDataAdapter(command);
                    commandDataAdapter.Fill(commandDataSet);
                    var actualPlanXml = Convert.ToString(commandDataSet.Tables[1].Rows[0][0]);

                    command.CommandText = @"SET STATISTICS XML OFF;";
                    command.ExecuteNonQuery();

                    errorMessage.Visible = false;

                    string sqplPlanFile = "attachment; filename=ActualQueryExecutionPlan.sqlplan";
                    Response.ClearContent();
                    Response.ContentType = "application/xml";
                    Response.AddHeader("content-disposition", sqplPlanFile);
                    Response.Write(actualPlanXml);
                    Response.End();
                }
                catch (Exception ex)
                {
                    errorMessage.Visible = true;
                    errorMessage.InnerText = ex.Message;
                }
                finally
                {
                    command.Connection.Close();
                    command.Dispose();
                }
            }
        }

        protected void getEstimatedPlan_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(query.Text))
            {
                errorMessage.Visible = true;
                errorMessage.InnerText = "Enter your query to continue.";
            }
            else
            {
                var command = ConnectionFactory.GetNewCommand(true);
                try
                {
                    command.CommandTimeout = 0;
                    command.CommandText = @"SET SHOWPLAN_XML ON;";
                    command.ExecuteNonQuery();

                    command.CommandText = query.Text;
                    var estimatedPlamXml = Convert.ToString(command.ExecuteScalar());

                    command.CommandText = @"SET SHOWPLAN_XML OFF;";
                    command.ExecuteNonQuery();

                    errorMessage.Visible = false;

                    string sqplPlanFile = "attachment; filename=EstimatedQueryExecutionPlan.sqlplan";
                    Response.ClearContent();
                    Response.ContentType = "application/xml";
                    Response.AddHeader("content-disposition", sqplPlanFile);
                    Response.Write(estimatedPlamXml);
                    Response.End();
                }
                catch (Exception ex)
                {
                    errorMessage.Visible = true;
                    errorMessage.InnerText = ex.Message;
                }
                finally
                {
                    command.Connection.Close();
                    command.Dispose();
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            errorMessage.Visible = false;
        }
    }
}