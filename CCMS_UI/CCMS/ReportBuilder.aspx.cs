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
    public partial class ReportBuilder : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button_Execute_Click(object sender, EventArgs e)
        {
            try
            {
                if (Textbox_SQL.Text.Length > 0)
                {
                    string[] blockKeyWords = new string[] { "INSERT", "UPDATE", "DELETE", "DROP", "CREATE", "ALTER" };
                    System.Data.DataTable dt = new System.Data.DataTable();
                    SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                    var command = Textbox_SQL.Text.Split(';')[0];
                    foreach (string x in blockKeyWords)
                    {
                        if (command.Contains(x))
                        {
                            throw new Exception("Only query expression is allowed");
                        }
                    }
                    cmd.CommandTimeout = int.Parse(TextBox_Timeout.Text);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                    GridView_Result.DataSource = dt;
                    GridView_Result.DataBind();
                    Literal_err.Text = "";
                }
            }
            catch (Exception ex)
            {
                Literal_err.Text = ex.Message;
            }
        }
    }
}