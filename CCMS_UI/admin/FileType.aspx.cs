using Avanza.CCMS.DAL;
using Avanza.iSuite.UI;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CCMSUI.admin
{
    public partial class FileType : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (!CheckRight((int)Permissions.ViewFileType))
                {
                    Response.StatusCode = 401;
                    Response.Redirect("AccessDenied.aspx");
                }

                if (!CheckRight((int)Permissions.CreateFileType))
                    GridView_FileType.Columns[4].Visible = false;

                if (!CheckRight((int)Permissions.DeleteFileType))
                    GridView_FileType.Columns[5].Visible = false;

                if (!CheckRight((int)Permissions.CreateFileType))
                    this.HyperLink_newFileType.Visible = false;

                ViewState["sortDirection"] = " asc";
                ViewState["SortExpression"] = "file_type_title";
                BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
            }


        }
        string Filter()
        {
            string filter = "";
            if (TextBox_title.Text.Length > 0)
                filter += " file_type_title like '%" + TextBox_title.Text + "%' and ";
            if (TextBox_path.Text.Length > 0)
                filter += " path_at_atm like '%" + TextBox_path.Text + "%' and ";
            if (DropDownList_CopyType.SelectedIndex > 0)
                filter += " copy_type = '" + DropDownList_CopyType.SelectedValue + "' and ";
            if (filter.Length > 0)
                return "where " + filter.Substring(0, filter.LastIndexOf("and"));
            return filter;
        }

        private void BindGrid(string orderby)
        {
            LogableTask task = LogableTask.NewTask("BindGrid");
            try
            {
                ValidateInputs();
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandText = @"SELECT file_type_id,path_at_atm,file_type_title,copy_type,is_ejlog FROM file_type " + Filter() + " order by " + orderby;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dtFileType = new DataTable();
                adapter.Fill(dtFileType);
                cmd.Connection.Close();
                this.GridView_FileType.DataSource = dtFileType;
                this.GridView_FileType.DataBind();
                Literal_count.Text = dtFileType.Rows.Count + " records.";
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                if (ex.Message.Contains("invalid"))
                {
                    throw new Exception(ex.Message);
                }
                else
                {
                    Literal_error.Text = ex.Message;
                }

            }
            finally
            { task.EndTask(); }
        }

        protected void GridView_FileType_Sorting(object sender, GridViewSortEventArgs e)
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


        protected void GridView_FileType_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            LogableTask task = null;
            if (e.CommandName == "deleteFileType")
            {

                try
                {
                    task = LogableTask.NewTask("delete file type");
                    Avanza.CCMS.DAL.FileType fileType = Avanza.CCMS.DAL.FileType.LoadFileTypeByPk(int.Parse(e.CommandArgument.ToString()));
                    fileType.Delete();

                    if (fileType != null)
                    {
                        Utility.BuildAuditLog("File: " + fileType.FileTypeTitle + " deleted", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.DeleteFileType);

                        //Literal_ok.Text = "<li>Selected atm was deleted!</li>";
                        Session[SessionVars.notice.ToString()] = "Selected file type was deleted!";
                    }

                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "file type deleted");
                    BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
                }
                catch (Exception ex)
                {
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                    Literal_error.Text = "<li>An Error occured while deleting file type. " + ex.Message + "</li>";

                }
                finally
                {

                    task.EndTask();

                }
            }
            if (e.CommandName == "editFileType")
            {
                task = null;

                try
                {
                    task = LogableTask.NewTask("edit File Type");

                    Session["file_type_id"] = e.CommandArgument.ToString();
                    Response.Redirect("AddEditFileType.aspx");
                }
                catch (Exception ex)
                {
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                    Literal_error.Text = "<li>An Error occured while Edit File Type. " + ex.Message + "</li>";

                }
                finally
                {
                    task.EndTask();
                }
            }
        }

        protected void GridView_FileType_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.GridView_FileType.PageIndex = e.NewPageIndex;
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }



        protected void Button_search_Click(object sender, EventArgs e)
        {
            try
            {
                BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
            }
            catch (Exception ex)
            {
                Literal_error.Text = ex.Message;
            }

        }

        protected void Button_reset_Click(object sender, EventArgs e)
        {
            TextBox_title.Text = "";
            TextBox_path.Text = "";
            DropDownList_CopyType.SelectedIndex = 0;

        }

        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            Button_search_Click(this, null);
        }


        public void ValidateInputs()
        {

            bool isTitleContainSpecialChars = CheckSpecialChars(TextBox_title.Text);
            if (isTitleContainSpecialChars == true)
            {
                throw new Exception("Title contain invalid character");

            }
            bool isPathContainSpecialChars = CheckSpecialChars(TextBox_path.Text);
            if (isPathContainSpecialChars == true)
            {
                throw new Exception("Path contain invalid character");

            }

        }
    }
}