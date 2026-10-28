using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Reflection;
using System.Diagnostics;
using System.Data.SqlClient;

using Avanza.iSuite.UI;
using Avanza.CCMS.DAL;
using System.IO;

namespace CCMSUI.admin
{
    public partial class Package : BasePage
    {
        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {

                //if (!CheckRight((int)Permissions.ModifyCIT))
                //    GridView_CIT.Columns[6].Visible = false;

                if (!CheckRight((int)Permissions.DeletePackage))
                    GridView_Packages.Columns[7].Visible = false;

                if (!CheckRight((int)Permissions.CreatePackage))
                    HyperLink_newPackage.Visible = false;

                ViewState["sortDirection"] = " desc";
                ViewState["SortExpression"] = "package_creation_time";
                //BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());

                //   AppSetting objappsetting = (AppSetting)Application[ApplicationVars.AppSettings.ToString()];
                //if (!objappsetting.IsGasperexchangeConfigured.Value)
                //  GridView_ATMs.Columns[3].Visible = true;
                AppUser.AppUserReader userReader = AppUser.ExecuteReader("1=1");
                DropDownList_creator.Items.Add(new ListItem("Any", "Any"));
                while (userReader.Read())
                    DropDownList_creator.Items.Add(new ListItem(userReader.CurrentAppUser.UserLogin, userReader.CurrentAppUser.UserId.ToString()));
                userReader.Close();


                BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
            }

        }
        string Filter()
        {
            string filter = "";
            if (TextBox_name.Text.Length > 0)
                filter += "  package_title ='" + TextBox_name.Text + "' and ";
            //if (TextBox_Code.Text.Length > 0)
            //    filter += "  code ='" + TextBox_Code.Text + "' and ";

            if (DropDownList_creator.SelectedIndex > 0)
                filter += "  created_by =" + DropDownList_creator.SelectedValue + " and ";

            //filter += " is_active in (" + DropDownList_status.SelectedValue + ") and ";
            if (filter.Length > 0)
                return "and " + filter.Substring(0, filter.LastIndexOf("and "));
            else
                return filter;
        }


        private void BindGrid(string orderby)
        {
            LogableTask task = LogableTask.NewTask("BindGrid");
            try
            {
                string selectedRegion = null;
                if (Session[SessionVars.SelectedRegions.ToString()] == null)
                    selectedRegion = "-1";
                else
                    selectedRegion = (string)Session[SessionVars.SelectedRegions.ToString()];
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);

                cmd.CommandText = @"select * from package inner join app_user on app_user.user_id = package.created_by where created_by = app_user.user_id  " + Filter() + "  order by " + orderby;


                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                cmd.Connection.Close();
                this.GridView_Packages.DataSource = dt; ;
                this.GridView_Packages.DataBind();
                Literal_count.Text = dt.Rows.Count + " records.";
            }
            catch (Exception ex)
            {
                Literal_error.Text = ex.Message;
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            { task.EndTask(); }
        }

        protected void GridView_Packages_Sorting(object sender, GridViewSortEventArgs e)
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

        protected void GridView_Packages_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            LogableTask task = LogableTask.NewTask("GridView_Packages_RowCommand");

            try
            {
                if (e.CommandName == "downloadPackage")
                {
                    // Download through DB
                    if (System.Configuration.ConfigurationManager.AppSettings["DBFileUploadCheck"] != null && System.Configuration.ConfigurationManager.AppSettings["DBFileUploadCheck"] == "1")
                    {
                        // New Work of saving file in Database
                        //byte[] bytes = System.IO.File.ReadAllBytes(zipFilePath);
                        SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                        cmd.Connection.Open();
                        cmd.Connection.ChangeDatabase(System.Configuration.ConfigurationManager.AppSettings["ViewDataPackagesDB"]);

                        cmd.CommandText = @"select [FileBytes] from [FileUpload] where  [PackageID] = '" + int.Parse(e.CommandArgument.ToString()) + "'";

                        SqlCommand selectCommand = new SqlCommand(cmd.CommandText, cmd.Connection);
                        SqlDataReader reader = selectCommand.ExecuteReader();
                        if (reader.Read())
                        {
                            byte[] byteData = (byte[])reader[0];

                            Avanza.CCMS.DAL.Package package = Avanza.CCMS.DAL.Package.LoadPackageByPk(int.Parse(e.CommandArgument.ToString()));
                            Response.Clear();
                            Response.AddHeader("Cache-Control", "no-cache, must-revalidate, post-check=0, pre-check=0");
                            Response.AddHeader("Pragma", "no-cache");
                            Response.AddHeader("Content-Description", "File Download");
                            Response.AddHeader("Content-Type", "application/force-download");
                            Response.AddHeader("Content-Transfer-Encoding", "binary\n");
                            Response.AddHeader("content-disposition", "attachment;filename=" + package.PackageTitle + ".zip");
                            Response.BinaryWrite(byteData);
                            Response.Flush();
                            Response.Close();
                        }
                    }
                    else
                    {
                        Avanza.CCMS.DAL.Package package = Avanza.CCMS.DAL.Package.LoadPackageByPk(int.Parse(e.CommandArgument.ToString()));
                        string tempFilename = package.PackagePhyscialPath;
                        Response.Clear();
                        Response.ContentType = "text/plain";
                        Response.AddHeader("Content-Disposition", "attachment; filename=" + Path.GetFileName(package.PackagePhyscialPath));
                        //Response.AddHeader("Content-Disposition", "attachment; filename=" + package.PackageTitle);

                        Response.AddHeader("Content-Length", new FileInfo(package.PackagePhyscialPath).Length.ToString());
                        Response.TransmitFile(tempFilename);
                        Response.Flush();
                        Response.Close();

                    }

                    //Session[SessionVars.notice.ToString()] = "Selected cit was deleted!";
                    //task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "cit deleted");
                    //BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
                }
                else if (e.CommandName == "deletePackage")
                {
                    Avanza.CCMS.DAL.Package package = Avanza.CCMS.DAL.Package.LoadPackageByPk(int.Parse(e.CommandArgument.ToString()));

                    if (System.Configuration.ConfigurationManager.AppSettings["DBFileUploadCheck"] != null && System.Configuration.ConfigurationManager.AppSettings["DBFileUploadCheck"] == "1")
                    {
                        SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                        cmd.Connection.Open();
                        cmd.Connection.ChangeDatabase(System.Configuration.ConfigurationManager.AppSettings["ViewDataPackagesDB"]);
                        cmd.CommandText = "delete from FileUpload where [PackageID] =" + int.Parse(e.CommandArgument.ToString());
                        cmd.ExecuteNonQuery();
                        cmd.Connection.Close();
                    }
                    else
                    {
                        if (File.Exists(package.PackagePhyscialPath))
                            Directory.Delete(Path.GetDirectoryName(package.PackagePhyscialPath), true);

                    }
                    ConnectionFactory.ExecuteQuery("delete package where package_id = " + package.PackageId + ";delete package_installation_detail where package_id = " + package.PackageId + ";delete upload where package_id = " + package.PackageId);
                    Session[SessionVars.notice.ToString()] = "Selected package deleted successfully!";
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Selected package deleted successfully!");
                    BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());

                    Utility.BuildAuditLog("Package [" + package.PackageTitle + "] deleted", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.DeleteATM);

                    //Session[SessionVars.notice.ToString()] = "Selected cit was deleted!";
                    //task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "cit deleted");
                    //BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
                }
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_error.Text = "<li>An Error occured while downloading package. " + ex.Message + "</li>";

            }
            finally
            {

                task.EndTask();

            }

        }

        protected void GridView_Packages_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.GridView_Packages.PageIndex = e.NewPageIndex;
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }

        protected void GridView_Packages_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            string progress = string.Empty;
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Get the data item for the current row
                DataRowView rowView = (DataRowView)e.Row.DataItem;
                if (rowView != null)
                {
                    // Check the value of the "Progress" column
                    progress = rowView["progress"].ToString();

                    // Find the "View" and "Download" buttons in the current row
                    HyperLink viewLink = (HyperLink)e.Row.FindControl("ViewLink");
                    ImageButton downloadButton = (ImageButton)e.Row.FindControl("Button_downloadPackage");

                    // Hide the buttons if the progress is "pending"
                    if (progress.Equals("pending", StringComparison.OrdinalIgnoreCase))
                    {
                        TableCell progressCell = e.Row.Cells[6];
                        progressCell.ForeColor = System.Drawing.Color.Blue;
                        viewLink.Visible = false;
                        downloadButton.Visible = false;
                    }
                }
            }

            if (e.Row.FindControl("Button_deletePackage") != null)
            {
                ((ImageButton)e.Row.FindControl("Button_deletePackage")).Attributes.Add("onClick",
                    "return confirm('Are you sure you want to delete package " + ((DataRowView)e.Row.DataItem).Row["package_title"] + "?','CCMS')");
            }
        }


        protected void Button_search_Click(object sender, EventArgs e)
        {
            //ValidateInput();

            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());


        }

        protected void Button_reset_Click(object sender, EventArgs e)
        {
            TextBox_name.Text = "";
            //TextBox_Code.Text = "";
            DropDownList_creator.SelectedIndex = 0;
            //DropDownList_status.SelectedIndex = 0;



        }

        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            Button_search_Click(this, null);
        }

       

        //public void ValidateInput()
        //{

        //        bool isNameContainSpecialChars = CheckSpecialChars(TextBox_name.Text);
        //        if (isNameContainSpecialChars == true)
        //        {
        //            throw new Exception("Name contain invalid character");
        //            //return result = false;
        //        }
        //        bool isCodeContainSpecialChars = CheckSpecialChars(TextBox_Code.Text);
        //        if (isCodeContainSpecialChars == true)
        //        {
        //            throw new Exception("Code contain invalid character");
        //            //return result = false;
        //        }

        //}

    }
}