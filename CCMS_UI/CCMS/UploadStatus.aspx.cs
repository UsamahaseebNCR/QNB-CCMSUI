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
using System.Data.SqlClient;
using System.Reflection;
using System.Diagnostics;
using System.Text;
using Avanza.iSuite.UI;

using Avanza.CCMS.DAL;

namespace CCMSUI.CCMS
{
    public partial class UploadStatus : BasePage
    {
        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        }
        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            Button_Search_Click(this, null);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (!CheckRight((int)Permissions.UploadStatus))
                    Response.Redirect("AccessDenied.aspx");
                
                txtFromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                txtToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");

                Package.PackageReader packageReader = Package.ExecuteReader("1=1");
                DropDownList_Package.Items.Add(new ListItem("Any", "Any"));
                while (packageReader.Read())
                    DropDownList_Package.Items.Add(new ListItem(packageReader.CurrentPackage.PackageTitle, packageReader.CurrentPackage.PackageId.ToString()));
                packageReader.Close();
                ViewState["sortDirection"] = " desc";
                ViewState["SortExpression"] = "creation_time";
          
            }
        }

        private void BindGrid(string orderBy)
        {
            LogableTask task = LogableTask.NewTask("BindGrid");
            try
            {
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandText = @"select atm.title as atmtitle, package_title, dbo.GetStatusFriendlyName(upld.status) status, upld.creation_time,appuser.user_login,
                                    upld.end_time,upld.uploaded_bytes,package_physcial_path,retry_remaining,last_invoked,failure_reason,execution_time,upload_id,package_size ,atm_bandwidth_name   
                        from UPLOAD upld inner join ATM atm 
                        on upld.atm_id = atm.atm_id inner join app_user appuser on
                        upld.created_by = appuser.user_id  inner join package on package.package_id = upld.package_id inner join atm_bandwidth on atm_bandwidth.atm_bandwidth_id = atm.atm_bandwidth_id " + buildSearchCriteria() + "order by " + orderBy;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                //adapter.SelectCommand = cmd;
                DataTable dtUpload = new DataTable();
                adapter.Fill(dtUpload);
                //cmd.Connection.Close();
                GridView_Uploads.DataSource = dtUpload;
                GridView_Uploads.DataBind();
                Literal_count.Text = dtUpload.Rows.Count.ToString();
            }

            catch (Exception ex)
            {
                Literal_error.Text = ex.Message;
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            {
                task.EndTask();
            }
        }
        //private string buildSearchCriteria(string Status, string IP, string FromDate, string ToDate)
        //{
        //    StringBuilder selectQuery = new StringBuilder();
        //    if (Status != "Select")
        //    {
        //        selectQuery.Append(" file_upload_status.STATUS = '" + Status + "' AND ");
        //    }

        //    if (IP.Length > 0)
        //        selectQuery.Append(" IP like '" + IP + "%' AND ");

        //    if (FromDate.Length > 0)
        //        selectQuery.Append(@" upld.creation_time >= Convert(datetime,'" + FromDate + " 00:00:00',103) AND ");

        //    if (txtToDate.Text.Length > 0)
        //        selectQuery.Append(@"  upld.creation_time <=Convert(datetime,'" + ToDate + " 23:59:59',103) AND ");

        //    selectQuery.Append(@" ATM.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") AND ");

        //    string query = selectQuery.ToString();
        //    query = query.Trim();

        //    if (query.EndsWith("AND"))
        //        query = query.Substring(0, query.LastIndexOf("AND"));
        //    if (query == "")
        //        return query;
        //    else
        //        return "AND " + query;
        //}

        protected void GridView_Uploads_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            if (e.Row.FindControl("Button_deleteUpload") != null)
            {
                if (e.Row.Cells[5].Text != "Scheduled" && e.Row.Cells[5].Text != "Uploading disconnected")          
                ((ImageButton)e.Row.FindControl("Button_deleteUpload")).Visible = false;
            }
        }

        private string buildSearchCriteria()
        {
            StringBuilder selectQuery = new StringBuilder();
            if (DDLUploadStatus.SelectedIndex>0)
            {
                selectQuery.Append(" upld.STATUS = '" + DDLUploadStatus.SelectedItem.Value + "' AND ");
            }
            if (DropDownList_Package.SelectedIndex>0)
            {
                selectQuery.Append(" upld.package_id = '" + DropDownList_Package.SelectedItem.Value + "' AND ");
            }

            //if (txtATMIP.Text.Length > 0)
            //    selectQuery.Append(" IP like '" + txtATMIP.Text + "%' AND ");

            if (txtFromDate.Text.Length > 0)
                selectQuery.Append(@" upld.creation_time >= Convert(datetime,'" + txtFromDate.Text + " 00:00:00',103) AND ");

            if (txtToDate.Text.Length > 0)
                selectQuery.Append(@"  upld.creation_time <=Convert(datetime,'" + txtToDate.Text + " 23:59:59',103) AND ");

            selectQuery.Append(@" ATM.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") AND ");

            string query = selectQuery.ToString();
            query = query.Trim();

            if (query.EndsWith("AND"))
                query = query.Substring(0, query.LastIndexOf("AND"));
            if (query == "")
                return query;
            else
                return "where " + query;
        }

        protected void GridView_Uploads_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            LogableTask task = null;
            if (e.CommandName == "deleteUpload")
            {
                task = LogableTask.NewTask("deleteUpload");

                try
                {



                    //if ((int)ConnectionFactory.ExecuteScalar("select count(cit_id) from app_user where cit_id=" + e.CommandArgument.ToString()) != 0)
                    //    throw new Exception ("Cannot delete this cit as it is assigned to some users");

                    //if ((int)ConnectionFactory.ExecuteScalar("select count(cit_id) from region_cit where cit_id=" + e.CommandArgument.ToString()) != 0)
                    //    throw new Exception("Cannot delete this cit as it is assigned to some organizations");
                    AppUser user = ((AppUser)Session[SessionVars.user.ToString()]);
                    Upload upload = Upload.LoadUploadByPk(int.Parse(e.CommandArgument.ToString()));
                    upload.Delete();
                    //ccmsCit.IsDeleted = true;
                    //ccmsCit.ModifiedOn = DateTime.Now;
                    //ccmsCit.ModifiedBy = (int)((AppUser)Session[SessionVars.user.ToString()]).UserId;
                    //ccmsCit.Save();
                    Package package = Package.LoadPackageByPk(upload.PackageId.Value);
                    Utility.BuildAuditLog(package.PackageTitle +" scheduled job deleted", user.UserId, (int)Permissions.DeletePackageScheduledJob);

                    //Literal_ok.Text = "<li>Selected atm was deleted!</li>";
                    Session[SessionVars.notice.ToString()] = "Selected package schedule deleted!";
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "scheduled job deleted");
                    BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
                }
                catch (Exception ex)
                {
                    task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                    Literal_error.Text = "<li>An Error occured while deleting. " + ex.Message + "</li>";

                }
                finally
                {

                    task.EndTask();

                }
            }
        }


//        protected void GridCallBack_Callback(object sender, ComponentArt.Web.UI.CallBackEventArgs e)
//        {
//            BindGrid(@"select atm.title as atmtitle, upld.title, upld.status, upld.creation_time,appuser.user_login,upld.end_time,uploaded_bytes,connection_breaks,source_path
//                        from UPLOAD upld inner join ATMs_TO_UPLOAD atmupld
//                        on upld.upload_id = atmupld.upload_id inner join ATM atm 
//                        on atmupld.atm_id = atm.atm_id inner join app_user appuser on
//                        upld.created_by = appuser.user_id inner join file_upload_status on file_upload_status.upload_id=upld.upload_id 
//                        inner join files_to_upload on files_to_upload.upload_id = upld.upload_id " + buildSearchCriteria(e.Parameters[0], e.Parameters[1], e.Parameters[2], e.Parameters[3]));
//            GridUpload.RenderControl(e.Output);
//            Literal_count.RenderControl(e.Output);
//        }
        protected void GridView_Uploads_Sorting(object sender, GridViewSortEventArgs e)
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
        protected void GridView_Uploads_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.GridView_Uploads.PageIndex = e.NewPageIndex;
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }

        protected void Button_Search_Click(object sender, EventArgs e)
        {
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }

        protected void Button_Reset_Click(object sender, EventArgs e)
        {
            txtFromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            txtToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            DDLUploadStatus.SelectedIndex = 0;
           // txtATMIP.Text = "";
            
        }




    }
}