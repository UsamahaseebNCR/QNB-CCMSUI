
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Avanza.CCMS.DAL;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;
using System.Data.OleDb;
using System.Data;
using System.Reflection;
using System.Diagnostics;

namespace CCMSUI.CCMS
{
    public partial class ViewTransactionChunk : System.Web.UI.Page
    {
        //static Regex formattingChars = new Regex(@"([[]00p)|([[]05p)|([[]020t)|([[]0r[(]1[)]2[[]000p[[]040qe1w3h162)|([(]\D)|([(]\d)|([(]C)|([[]040)|([[]000p)|(/)|(q[(]I)|(q[(]1)|([(]7)|([(]>)|()|([(]>)|([(]C)|([(]1)|([(]D)|(\+)");
        static Regex formattingChars = new Regex(@"([[]00p)|([[]05p)|([[]020t)|([[]0r[(]1[)]2[[]000p[[]040qe1w3h162)|([(]\D)|([(]\d)|([(]C)|([[]040)|([[]000p)|(/)|(q[(]I)|(q[(]1)|([(]7)|([(]>)|()|([(]>)|([(]C)|([(]1)|([(]D)|(\+)|()");

        Avanza.CCMS.DAL.VEjParsedTransactions trxn = null;
        Avanza.CCMS.DAL.VEjParsedBNATransaction ejParsedBnaTrxn = null;
        Avanza.CCMS.DAL.VEjParsedCPMtransaction ejParsedCpmTrxn = null;
        Avanza.CCMS.DAL.EjCapturedCard ejCapturedCard = null;

        private void ViewChunk()
        {
            int startIndex = 0;
            int endIndex = 0;
            DateTime trxnDatetime = DateTime.MinValue;
            string pan = null;
            string tsn = null;
            string host_tsn = null;
            decimal amount = 0;
            string status = "";
            string ej = null;
            int taskID = 0;
            Avanza.CCMS.DAL.CapturedTransactions capturedTrxn = Avanza.CCMS.DAL.CapturedTransactions.LoadCapturedTransactionsByPk(int.Parse(Request.QueryString["id"]));
            if (capturedTrxn.EjParsedTransactionsId > 0)
            {
                trxn = VEjParsedTransactions.LoadVEjParsedTransactionsByPk(capturedTrxn.EjParsedTransactionsId.Value);
                if (trxn != null)
                {
                    taskID = trxn.TaskId.Value;
                    pan = trxn.Pan;
                    tsn = trxn.Tsn;
                    host_tsn = trxn.HostTsn;
                    trxnDatetime = trxn.TrxnDatetime.Value;
                    amount = trxn.Amount.HasValue ? trxn.Amount.Value : 0;
                    startIndex = trxn.StartIndex.Value;
                    endIndex = trxn.EndIndex.Value;
                    status = trxn.Status == 0 ? "Successful" : trxn.Status == 1 ? "Failed" : trxn.Status == 2 ? "Suspicious" : "";
                }
            }

            else if (capturedTrxn.EjParsedBnaTransactionsId.HasValue)
            {
                ejParsedBnaTrxn = VEjParsedBNATransaction.LoadVEjParsedBNATransactionsByPk(capturedTrxn.EjParsedBnaTransactionsId.Value);
                if (ejParsedBnaTrxn != null)
                {
                    taskID = ejParsedBnaTrxn.TaskId;
                    pan = ejParsedBnaTrxn.Pan;
                    tsn = ejParsedBnaTrxn.Seq;
                    host_tsn = ejParsedBnaTrxn.HostTsn;
                    trxnDatetime = ejParsedBnaTrxn.TrxnDatetime.Value;
                    amount = ejParsedBnaTrxn.AmountAuthorized.HasValue ? ejParsedBnaTrxn.AmountAuthorized.Value : 0;
                    startIndex = ejParsedBnaTrxn.StartIndex;
                    endIndex = ejParsedBnaTrxn.EndIndex;
                    status = ejParsedBnaTrxn.Status;
                }
            }
            else if (capturedTrxn.EjParsedCpmTransactionsId.HasValue)
            {
                ejParsedCpmTrxn = VEjParsedCPMtransaction.LoadVEjParsedCPMtransactionByPk(capturedTrxn.EjParsedCpmTransactionsId.Value);
                if (ejParsedCpmTrxn != null)
                {
                    taskID = ejParsedCpmTrxn.TaskId;
                    pan = ejParsedCpmTrxn.Pan;
                    tsn = ejParsedCpmTrxn.Seq;
                    host_tsn = ejParsedCpmTrxn.HostTsn;
                    trxnDatetime = ejParsedCpmTrxn.TrxnDatetime.Value;
                    if (ejParsedCpmTrxn.DepositAmount.HasValue)
                        amount = ejParsedCpmTrxn.DepositAmount.Value;
                    startIndex = ejParsedCpmTrxn.StartIndex;
                    endIndex = ejParsedCpmTrxn.EndIndex;
                }


            }
            else if (capturedTrxn.EjCapturedCardId.HasValue)
            {
                ejCapturedCard = EjCapturedCard.LoadEjCapturedCardByPk(capturedTrxn.EjCapturedCardId.Value);
                if (ejCapturedCard != null)
                {
                    taskID = ejCapturedCard.TaskId;
                    pan = ejCapturedCard.PAN;
                    tsn = ejCapturedCard.TSN.ToString();
                    trxnDatetime = ejCapturedCard.CaptureTime;
                    startIndex = ejCapturedCard.StartIndex.Value;
                    endIndex = ejCapturedCard.EndIndex.Value;
                }
                //status = ejParsedCpmTrxn.Status;
            }

            // Avanza.CCMS.DAL.EjParsedTransactions trxn = Avanza.CCMS.DAL.EjParsedTransactions.LoadEjParsedTransactionsByPk(int.Parse(Request.QueryString["id"]));
            if (taskID > 0)
            {
                Avanza.CCMS.DAL.VTask task = Avanza.CCMS.DAL.VTask.LoadvTaskByPk(taskID);
                int uncompressedLength;

                if (task.FileTypeId == 3)
                {
                    AppSetting appSetting = (AppSetting)Application[ApplicationVars.AppSettings.ToString()];
                    string filePath = appSetting.TemporaryFolder + "\\ejdata.mdb";
                    if (File.Exists(filePath))
                        File.Delete(filePath);
                    byte[] data = CashDataViewer.GetUnCompressedBytes(task, FileType.LoadFileTypeByPk(task.FileTypeId.Value), out uncompressedLength);

                    File.WriteAllBytes(filePath, data);

                    StringBuilder builder1 = new StringBuilder();
                    OleDbCommand command = new OleDbCommand(@"select text from events", new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";"));
                    OleDbDataAdapter adapter = new OleDbDataAdapter(command);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    foreach (DataRow dr in dt.Rows)
                        builder1.Append(dr[0] + "\r\n");
                    ej = builder1.ToString();

                }
                else
                {

                    ej = CashDataViewer.GetDecodedString(task, FileType.LoadFileTypeByPk(task.FileTypeId.Value), out uncompressedLength);
                    if (task.TaskInfo != null && task.FileTypeId == 2)
                    {
                        if (task.TaskInfo.Length > 0)
                        {
                            int temp = task.TaskId;
                            task.TaskId = int.Parse(task.TaskInfo);
                            ej = CashDataViewer.GetDecodedString(task, FileType.LoadFileTypeByPk(task.FileTypeId.Value), out uncompressedLength) + ej;
                            task.TaskId = temp;
                        }
                        //reverting back task id;


                    }
                }
                //editing done by shariq 31Aug2015 if check work in substring below
                if (task.FileTypeId == 2 || task.FileTypeId == 19)
                    ej = formattingChars.Replace(ej, "");
                //ej = formattingChars.Replace(ej, "");
                Literal_EJ.Text = ej.Substring(startIndex, endIndex - startIndex).Replace("\r\n", "<br/>").Replace("\r", "<br/>").Replace("\n", "<br/>");
                Label_Datetime.Text = trxnDatetime.ToString("dd/MM/yyyy HH:mm:ss");
                Label_PAN.Text = pan;
                Label_TSN.Text = tsn;
                Label_HostTSN.Text = host_tsn;
                Label_Amount.Text = amount.ToString();
                Label_Status.Text = status;
                chunk.Visible = true;
                completeFile.Visible = false;
                //******


                //ParsedTransaction.ParsedTransactionReader CCMSParsedTrxn = ParsedTransaction.ExecuteReader("trxn_datetime>=convert(datetime,'" + trxn.TrxnDatetime.Value.ToString("dd/MM/yyyy HH:mm:ss") + "',103) and " +
                //                                                                            "trxn_datetime<=convert(datetime,'" + trxn.TrxnDatetime.Value.AddSeconds(double.Parse(System.Configuration.ConfigurationManager.AppSettings["SecondsBetweenTransacionsInEJAndCCMS"])).ToString("dd/MM/yyyy HH:mm:ss") + "',103)" +
                //                                                                            " and atm_id =" + trxn.AtmId.Value);
                //StringBuilder builder = new StringBuilder();
                //builder.Append("<table>");
                //while (CCMSParsedTrxn.Read())
                //{
                //    builder.Append("<tr><td>DateTime</td><td>" + CCMSParsedTrxn.CurrentParsedTransaction.TrxnDatetime.ToString("dd/MM/yyyy HH:mm:ss") + "</td></tr>");
                //    builder.Append("<tr><td>Amount</td><td>" + CCMSParsedTrxn.CurrentParsedTransaction.Amount + "</td></tr>");
                //}
                //builder.Append("</table>");
                //CCMSParsedTrxn.Close();
                //Literal_CompleteEJFile.Text = builder.ToString();

            }
        }

        protected bool CheckRight(int right)
        {
            DataTable dtRights = (DataTable)Session[SessionVars.dtUserRights.ToString()];
            if (dtRights == null)
            {
                Response.Redirect("../signin.aspx");
                //Response.Write("<script> if (window.parent) window.parent.location ='../signin.aspx'; else window.location ='../signin.aspx'</script>");
                //Response.End();
            }

            Permissions currentPermission = (Permissions)Enum.ToObject(typeof(Permissions), right);
            String permissionName = currentPermission.ToString();

            bool hasRight = false;

            hasRight = dtRights.Select(String.Format("NAME ='{0}'", permissionName)).Length > 0;

            return hasRight;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    if (!CheckRight((int)Permissions.ViewPNCBNATransactions) && !CheckRight((int)Permissions.ViewEJBNATransactions) && !CheckRight((int)Permissions.ViewEJTransactions))
                        Response.Redirect("AccessDenied.aspx");
                    ViewChunk();
                }
            }
            catch (Exception ex)
            {
                Literal_error.Text = ex.Message;
                LogableTask.LogMonoActivityTask("", MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }


        }
        protected void Button_ViewChunk_Click(object sender, EventArgs e)
        {
            ViewChunk();
        }

        protected void Button_ViewEJ_Click(object sender, EventArgs e)
        {
            string ej = null;
            if (Request.QueryString["taskID"] != null)
            {
                int taskID = int.Parse(Request.QueryString["taskID"]);
                Avanza.CCMS.DAL.VTask task = Avanza.CCMS.DAL.VTask.LoadvTaskByPk(taskID);
                int uncompressedLength;

                byte[] data = CashDataViewer.GetUnCompressedBytes(task, FileType.LoadFileTypeByPk(task.FileTypeId.Value), out uncompressedLength);
                if (task.FileTypeId == 3)
                {
                    AppSetting appSetting = (AppSetting)Application[ApplicationVars.AppSettings.ToString()];
                    string filePath = appSetting.TemporaryFolder + "\\ejdata.mdb";
                    if (File.Exists(filePath))
                        File.Delete(filePath);
                    File.WriteAllBytes(filePath, data);

                    StringBuilder builder1 = new StringBuilder();
                    OleDbCommand command = new OleDbCommand(@"select text from events", new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";"));
                    OleDbDataAdapter adapter = new OleDbDataAdapter(command);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    foreach (DataRow dr in dt.Rows)
                        builder1.Append(dr[0] + "\r\n");
                    ej = builder1.ToString();

                }
                else
                    ej = CashDataViewer.GetDecodedString(task, FileType.LoadFileTypeByPk(task.FileTypeId.Value), out uncompressedLength);

                //ej = formattingChars.Replace(ej, "");
                Literal_CompleteEJFile.Text = ej.Replace("\r\n", "<br/>").Replace("\r", "<br/>").Replace("\n", "<br/>"); ;
                chunk.Visible = false;
                completeFile.Visible = true;

            }
        }
    }
}