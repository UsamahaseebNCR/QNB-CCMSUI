using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Avanza.iSuite.UI;
using System.Data.SqlClient;

using System.Data;
using System.Reflection;
using System.Diagnostics;
using Avanza.CCMS.DAL;

namespace CCMSUI.admin
{
    public partial class Classification : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (!CheckRight((int)Permissions.ViewATMClassification))
                {
                    Response.Redirect("AccessDenied.aspx");
                    Response.StatusCode = 401;
                }

                if (!CheckRight((int)Permissions.ModifyATMClassification))
                    GridView_Classification.Columns[1].Visible = false;

                if (!CheckRight((int)Permissions.DeleteATMClassification))
                    GridView_Classification.Columns[2].Visible = false;

                if (!CheckRight((int)Permissions.CreateATMClassification))
                    this.HyperLink_newClassfication.Visible = false;

                ViewState["sortDirection"] = " asc";
                ViewState["SortExpression"] = "classification_name";
                BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
            }


        }
        string Filter()
        {
            string filter = "";
            if (TextBox_name.Text.Length > 0)
                filter += " classification_name like '%" + TextBox_name.Text + "%' and ";
        
            if (filter.Length > 0)
                return "where " + filter.Substring(0, filter.LastIndexOf("and"));
            return filter;
        }

        private void BindGrid(string orderby)
        {
            LogableTask task = LogableTask.NewTask("BindGrid");
            try
            {
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandText = @"SELECT * FROM classification inner join app_user on app_user.user_id =classification.created_by  " + Filter() + " order by " + orderby;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dtFileType = new DataTable();
                adapter.Fill(dtFileType);
                cmd.Connection.Close();
                this.GridView_Classification.DataSource = dtFileType;
                this.GridView_Classification.DataBind();
                Literal_count.Text = GridView_Classification.Rows.Count + " records.";
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_error.Text = ex.Message;

            }
            finally
            { task.EndTask(); }
        }

        protected void GridView_Classification_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (ViewState["SortExpression"].ToString() == e.SortExpression)
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
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }


        protected void GridView_Classification_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.FindControl("Button_deleteClassification") != null)
            {
                if (!CheckRight((int)Permissions.DeleteATMClassification))
                {
                    Literal_error.Text = "<li>Un Authorized. You are not allowed to delete Classification.</li>";
                    Response.StatusCode = 401;
                    return;
                }


                ((ImageButton)e.Row.FindControl("Button_deleteClassification")).Attributes.Add("onClick",
                    "return confirm('Are you sure you want to delete ATM Classification " + ((DataRowView)e.Row.DataItem).Row["classification_name"] + "?','CCMS')");

            }
        }



        protected void GridView_Classification_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            LogableTask task = null;
            if (e.CommandName == "deleteClassification")
            {

                try
                {
                    if (!CheckRight((int)Permissions.DeleteATMClassification))
                    {
                        Literal_error.Text = "<li>Un Authorized. You are not allowed to delete Classification.</li>";
                        task = LogableTask.NewTask("Un Authorized. You are not allowed to delete Classification");
                        Response.StatusCode = 401;
                        return;
                    }
                    task = LogableTask.NewTask("delete atm classification");
                    Avanza.CCMS.DAL.Classification classification= Avanza.CCMS.DAL.Classification.LoadClassificationByPk(int.Parse(e.CommandArgument.ToString()));
                    classification.Delete();

                    if (classification != null)
                    {
                        Utility.BuildAuditLog("ATM Classification: "+classification.ClassificationName + " deleted", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.DeleteATMClassification);

                        //Literal_ok.Text = "<li>Selected atm was deleted!</li>";
                        Session[SessionVars.notice.ToString()] = "Selected atm classification deleted!";
                    }

                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Selected atm classification deleted");
                    BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
                }
                catch (Exception ex)
                {
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                    Literal_error.Text = "<li>An Error occured while deleting atm classification. " + ex.Message + "</li>";

                }
                finally
                {

                    task.EndTask();

                }
            }
        }

        protected void GridView_Classification_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.GridView_Classification.PageIndex = e.NewPageIndex;
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }



        protected void Button_search_Click(object sender, EventArgs e)
        {
            try
            {
                bool isNameContainSpecialChars = CheckSpecialChars(TextBox_name.Text);
                if (isNameContainSpecialChars == true)
                {
                    throw new Exception("Name contain invalid character");
                    
                }
                BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("invalid"))
                {
                    throw new Exception(ex.Message);
                }
                else
                {
                    Literal_error.Text = ex.Message;
                }
            }
           
        }

        protected void Button_reset_Click(object sender, EventArgs e)
        {
            TextBox_name.Text = "";

        }

        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            Button_search_Click(this, null);
        }
       
    }
}