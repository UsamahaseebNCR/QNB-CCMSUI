using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data.SqlClient;
using Microsoft.Office.Interop.Excel;

namespace CCMSUI.CCMS
{
    public partial class Web2SQL : System.Web.UI.Page
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
                    System.Data.DataTable dt = new System.Data.DataTable();
                    SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                    cmd.CommandText = Textbox_SQL.Text;
                    cmd.CommandTimeout = int.Parse(TextBox_Timeout.Text);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                    GridView_Result.DataSource = dt;
                    GridView_Result.DataBind();
                    Literal_err.Text = "";
                    Literal_ok.Text = "Query Executed Successfully";
                    string item = Textbox_SQL.Text;
                    if (!ListBox_ExcecutedQueries.Items.Contains(new ListItem(item)))
                        ListBox_ExcecutedQueries.Items.Add(Textbox_SQL.Text);
                    Textbox_SQL.Text = "";
                }
            }
            catch (Exception ex)
            {
                Literal_err.Text = ex.Message;
                Literal_ok.Text = "";
            }

        }

      
        protected void Button_ReExecute_Click(object sender, EventArgs e)
        {
            if (ListBox_ExcecutedQueries.SelectedIndex >= 0)
            {
                Textbox_SQL.Text = ListBox_ExcecutedQueries.SelectedItem.Text;
                Button_Execute_Click(this,null);
            }
        }
    }
}