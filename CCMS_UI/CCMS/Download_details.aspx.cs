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
using Avanza.CCMS.DAL;
using Avanza.iSuite.UI;

namespace CCMSUI.CCMS
{
    public partial class Download_details : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Task task = Task.LoadTaskByPk(int.Parse(Request.QueryString["file_id"]));
                string Params = null;

                if (task.TaskTypeId == 1)
                {
                    string fileTypeTitle = null;
                    if (task.FileTypeId != null)
                    {
                        FileType fileType = FileType.LoadFileTypeByPk(task.FileTypeId.Value);

                        fileTypeTitle = fileType.FileTypeTitle;
                    }
                    Params = String.Format(@"<tr>
                                    <td>File Type:</td>
                                    <td>{0}</td>" +
                                    @"<td>Zipped File Size:</td>
                                    <td>{1}</td>
                                    </tr>" +
                                    @"<tr>
                                    <td>Unzipped File Size:</td>
                                    <td>{2}</td>" +
                                    @"<td>Parsed:</td>
                                    <td>{3}</td>
                                    </tr>
                                    <tr>
                                    <td>File Name:</td>
                                    <td>{4}</td>
                                    </tr>", fileTypeTitle, task.ZippedFileSize.ToString(), task.Parsed.Value ? "Yes" : "No",
                                          task.Parsed.Value ? "Yes" : "No", task.ServerFilepath);
                    Button_Cancel.Text = "Cancel this download";
                }
                else
                    Button_Cancel.Text = "Cancel this upload";
                if (task.TaskTypeId == 4)
                {
                    Avanza.CCMS.DAL.CashOrders cashOrders = Avanza.CCMS.DAL.CashOrders.LoadCashOrdersByPk((int)task.CashOrderId);
                    string UploadTime = task.UploadTime.HasValue ? task.UploadTime.Value.ToString() : "";
                    Params = @"<tr>
                                    <td>Cash Order Date:</td>
                                    <td>" + cashOrders.CashOrderDatetime.ToString("dd/MM/yyyy") + @"</td>
                                     <td>Upload Finished Time:</td>
                                    <td>" + UploadTime + "</td>" +
                                    @"</tr>

                                    <tr>
                                    <td>Cassette1 Suggested Notes:</td>
                                    <td>" + cashOrders.Cassette1SuggestedNotes.Value.ToString() + "</td>" +
                                       @"<td>Cassette1 Remaining Notes:</td>
                                   <td>" + cashOrders.Cassette1RemainingNotes.Value.ToString() + "</td>" +
                                    @"</tr>
                                    <tr>
                                    <td>Cassette2 Suggested Notes:</td>
                                    <td>" + cashOrders.Cassette2SuggestedNotes.Value.ToString() + "</td>" +
                                       @"<td>Cassette2 Remaining Notes:</td>
                                    <td>" + cashOrders.Cassette2RemainingNotes.Value.ToString() + "</td>" +
                                    @"</tr>
                                    <tr>
                                    <td>Cassette3 Suggested Notes:</td>
                                    <td>" + cashOrders.Cassette3SuggestedNotes.Value.ToString() + "</td>" +
                                    @"<td>Cassette3 Remaining Notes:</td>
                                    <td>" + cashOrders.Cassette3RemainingNotes.Value.ToString() + "</td>" +
                                    @"</tr>
                                    <tr>
                                    <td>Cassette4 Suggested Notes:</td>
                                    <td>" + cashOrders.Cassette4SuggestedNotes.Value.ToString() + "</td>" +
                                    "<td>Cassette4 Remaining Notes:</td>" +
                                    "<td>" + cashOrders.Cassette4RemainingNotes.Value.ToString() + "</td>" +
                                    "</tr>" +
                                    "<tr>" +
                                    @"<td>Cassette5 Suggested Notes:</td>
                                    <td>" + cashOrders.Cassette5SuggestedNotes.Value.ToString() + "</td>" +
                                    @"<td>Cassette5 Remaining Notes:</td>
                                    <td>" + cashOrders.Cassette5RemainingNotes.Value.ToString() + "</td>" +
                                    @"</tr>" +
                                    "<tr>" +
                                    @"<td>Cassette6 Suggested Notes:</td>
                                    <td>" + cashOrders.Cassette6SuggestedNotes.Value.ToString() + "</td>" +
                                    @"<td>Cassette6 Remaining Notes:</td>
                                    <td>" + cashOrders.Cassette6RemainingNotes.Value.ToString() + "</td>" +
                                    @"</tr>" +
                                    "<tr>" +
                                    @"<td>Cassette7 Suggested Notes:</td>
                                    <td>" + cashOrders.Cassette7SuggestedNotes.Value.ToString() + "</td>" +
                                    @"<td>Cassette7 Remaining Notes:</td>
                                    <td>" + cashOrders.Cassette7RemainingNotes.Value.ToString() + "</td>" +
                                    @"</tr>";
                }

                Literal_Heading.Text = "Task Details";

                Label_RetryCount2.Text = task.RetryRemaining.ToString();
                if (task.FileTypeId != null)
                    Label_Duration.Text = Math.Round(((TimeSpan)task.DownloadTime.Value.Subtract(new DateTime(1900, 1, 1))).TotalMinutes, 3) + " minutes";
                else
                {
                    if (task.UploadTime.HasValue)
                        Label_Duration.Text = Math.Round(((TimeSpan)task.UploadTime.Value.Subtract(new DateTime(1900, 1, 1))).TotalMinutes, 3) + " minutes";
                }

                Label_BytesDownloaded.Text = task.BytesTransferred.ToString();
                Label_CreationTimestamp.Text = task.CreationTime.ToString("dd/MM/yyyy HH:mm:ss");
                if (task.EndTime.HasValue)
                    Label_FinishedTimestamp2.Text = task.EndTime.Value.ToString("dd/MM/yyyy HH:mm:ss");
                Label_Status.Text = task.Status;
                if (task.LastInvoked.HasValue)
                    Label_LastInvoked2.Text = task.LastInvoked.Value.ToString("dd/MM/yyyy HH:mm:ss");



                Label_LastAttemptFailedReason2.Text = task.FailureReason;

                if (Label_LastAttemptFailedReason2.Text.Length == 0) Label_LastAttemptFailedReason2.Text = "-";

                if (task.Status == DownloadStates.scheduled.ToString() ||
                    task.Status == DownloadStates.downloadingDisconnected.ToString() ||
                    //task.Status == DownloadStates.downloadedParsePending.ToString() ||
                    task.Status == UploadStates.uploadingDisconnected.ToString())
                {
                    Button_Cancel.Visible = true;
                    //  if (task.Status == DownloadStates.downloadedParsePending.ToString())
                    //     Button_Cancel.Text = "Cancel Parsing";
                }
                else
                    Button_Cancel.Visible = false;
                literal_trial.Text = Params;
                Session["LoadGrid"] = true;
            }
        }

        protected void Button_Cancel_Click(object sender, EventArgs e)
        {
            Task task = Task.LoadTaskByPk(int.Parse(Request.QueryString["file_id"]));
            task.Status = DownloadStates.cancelled.ToString();
            task.Save();
            Response.Redirect("TerminalFileStatusMonitoring.aspx");
        }
    }
}