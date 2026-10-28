using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Reflection;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

using Avanza.CCMS.DAL;
using Avanza.iSuite.UI;
using System.IO;
using System.Web.Caching;

namespace CCMSUI.CCMS
    {
        public partial class ATMReconciliationDataFeedFiles : BasePage
        {

            protected void Page_Load(object sender, EventArgs e)
            {
                if (!Page.IsPostBack)
                {
                    ViewState["sortDirection"] = " desc";
                    ViewState["SortExpression"] = "creation_time";

                    TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                    TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                    Array downloadStates = Enum.GetValues(typeof(FTPDownloadStatus));
                    for (int counter = 0; counter < downloadStates.Length; counter++)
                    {
                        string tempString = downloadStates.GetValue(counter).ToString();
                    }

                    BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());




                }
            }

            protected string Filter()
            {
                string filter = "";

                if (TextBox_FromDate.Text.Length > 0)
                    filter += " creation_time >= convert(datetime,'" + TextBox_FromDate.Text + "',103)";
                else
                    filter += " and creation_time >= getdate() - 3";
                if (TextBox_ToDate.Text.Length > 0)
                    filter += " and creation_time <= convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103)";
                if (DropDownList_TaskType.SelectedIndex > 0)
                    filter += " and file_type in ('" + DropDownList_TaskType.SelectedItem.Value + "')";

                return filter;
            }

            private void BindGridWithCache()
            {
                DataTable dtFtp = (DataTable)Cache["dtFtp"];
                dtFtp.DefaultView.Sort = ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString();
                GridView_FileTransferStatus.DataSource = dtFtp;
                GridView_FileTransferStatus.DataBind();
                Literal_count.Text = dtFtp.Rows.Count + " records.";
            }

            private void BindGrid(string orderby)
            {
                LogableTask task = LogableTask.NewTask("BindGrid");

                try
                {
                    SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                    cmd.CommandTimeout = 500;
                    cmd.CommandText = @"select * from reconciliation_data_feed_files where" + Filter() + " order by " + orderby;
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dtFTPStatus = new DataTable();
                    adapter.Fill(dtFTPStatus);

                    Cache["dtFtp"] = dtFTPStatus;
                    GridView_FileTransferStatus.DataSource = dtFTPStatus;
                    GridView_FileTransferStatus.DataBind();

                    Literal_count.Text = dtFTPStatus.Rows.Count + " records.";
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

            protected void GridView_FileTransferStatus_Sorting(object sender, GridViewSortEventArgs e)
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

                BindGridWithCache();
            }

            protected void GridView_FileTransferStatus_PageIndexChanging(object sender, GridViewPageEventArgs e)
            {
                GridView_FileTransferStatus.PageIndex = e.NewPageIndex;
                BindGridWithCache();
            }

            protected void Button_search_Click(object sender, EventArgs e)
            {
                BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
            }

            protected void Button_reset_Click(object sender, EventArgs e)
            {
                TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                DropDownList_TaskType.SelectedIndex = 0;
            }

            protected void SaveAsTXT_Click(object sender, ImageClickEventArgs e)
            {
               
                try
                {
                    string filePath = ((ImageButton)sender).CommandArgument.ToString();
                    Response.Clear();
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + Path.GetFileName(filePath));
                    Response.TransmitFile(filePath);
                    Response.Flush();
                    Response.Close();
                    //Cache.Add(tempFilename, tempFilename, null, Cache.NoAbsoluteExpiration, new TimeSpan(0, 1, 0), CacheItemPriority.Normal, new CacheItemRemovedCallback(DeleteFile));
                }
                catch (Exception ex)
                {
                    Literal_error.Text = "<li>" + ex.Message + "</li>";
                }
            }
        }
    }