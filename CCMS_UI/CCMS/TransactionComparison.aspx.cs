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
using Avanza.iSuite.UI;

namespace CCMSUI.CCMS
{
    public partial class TransactionComparison : BasePage
    {
        //static Regex formattingChars = new Regex(@"([[]00p)|([[]05p)|([[]020t)|([[]0r[(]1[)]2[[]000p[[]040qe1w3h162)|([(]\D)|([(]\d)|([(]C)|([[]040)|([[]000p)|(/)|(q[(]I)|(q[(]1)|([(]7)|([(]>)|()|([(]>)|([(]C)|([(]1)|([(]D)|(\+)");
        static Regex formattingChars = new Regex(@"([[]00p)|([[]05p)|([[]020t)|([[]0r[(]1[)]2[[]000p[[]040qe1w3h162)|([(]\D)|([(]\d)|([(]C)|([[]040)|([[]000p)|(/)|(q[(]I)|(q[(]1)|([(]7)|([(]>)|()|([(]>)|([(]C)|([(]1)|([(]D)|(\+)|()");

        protected void Page_Load(object sender, EventArgs e)
        {
            Avanza.CCMS.DAL.VEjParsedTransactions trxn = null;
            Avanza.CCMS.DAL.VEjParsedBNATransaction BNATrxn = null;
            Avanza.CCMS.DAL.VEjParsedCPMtransaction CPMTrxn = null;
            Avanza.CCMS.DAL.VTask task = null;
            string ej = null;
            if (!IsPostBack)
            {
                object result = Session["d"];
                if (result != null)
                {
                    if (result.ToString() == "2")
                        CPMTrxn = Avanza.CCMS.DAL.VEjParsedCPMtransaction.LoadVEjParsedCPMtransactionByPk(int.Parse(Session["ej"].ToString()));

                    else if (result.ToString() == "1")
                        BNATrxn = Avanza.CCMS.DAL.VEjParsedBNATransaction.LoadVEjParsedBNATransactionsByPk(int.Parse(Session["ej"].ToString()));
                }
                else
                    trxn = Avanza.CCMS.DAL.VEjParsedTransactions.LoadVEjParsedTransactionsByPk(int.Parse(Session["ej"].ToString()));



                if (trxn != null || BNATrxn != null || CPMTrxn != null)
                {
                    if (CPMTrxn != null)
                        task = Avanza.CCMS.DAL.VTask.LoadvTaskByPk(CPMTrxn.TaskId);
                    else if (BNATrxn != null)
                        task = Avanza.CCMS.DAL.VTask.LoadvTaskByPk(BNATrxn.TaskId);
                     else
                        task = Avanza.CCMS.DAL.VTask.LoadvTaskByPk(trxn.TaskId.Value);

                    int uncompressedLength;

                    if (task.FileTypeId == 3)
                    {
                        byte[] data = CashDataViewer.GetUnCompressedBytes(task, FileType.LoadFileTypeByPk(task.FileTypeId.Value), out uncompressedLength);

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
                    {
                        ej = CashDataViewer.GetDecodedString(task, FileType.LoadFileTypeByPk(task.FileTypeId.Value), out uncompressedLength);
                        if (task.TaskInfo != null && task.FileTypeId==2)
                        {
                            int temp = task.TaskId;
                            task.TaskId = int.Parse(task.TaskInfo);
                            ej = CashDataViewer.GetDecodedString(task, FileType.LoadFileTypeByPk(task.FileTypeId.Value), out uncompressedLength)+ej;
                            task.TaskId = temp;
                            //reverting back task id;
                            
                        }

                    }
                    if (task.FileTypeId == 2 || task.FileTypeId ==19)
                        ej = formattingChars.Replace(ej, "");
                    
                    if (trxn != null)
                    {
                        Literal_EJ.Text = ej.Substring(trxn.StartIndex.Value, trxn.EndIndex.Value - trxn.StartIndex.Value).Replace("\r\n", "<br/>").Replace("\r", "<br/>").Replace("\n", "<br/>"); 
                        Label_Datetime.Text = trxn.TrxnDatetime.Value.ToString("dd/MM/yyyy HH:mm:ss");
                        Label_PAN.Text = trxn.Pan;
                        Label_TSN.Text = trxn.Tsn;
                        Label_HostTSN.Text = trxn.HostTsn;
                        Label_Amount.Text = trxn.Amount.ToString();
                        if (trxn.Status == 0)
                            Label_Status.Text = "Successful";
                        else if (trxn.Status == 1)
                            Label_Status.Text = "Failed";
                        else if (trxn.Status == 2)
                            Label_Status.Text = "Suspicious";

                        //ParsedTransaction.ParsedTransactionReader CCMSParsedTrxn = ParsedTransaction.ExecuteReader("trxn_datetime>=convert(datetime,'" + trxn.TrxnDatetime.Value.ToString("dd/MM/yyyy HH:mm:ss") + "',103) and " +
                        //                                                                     "trxn_datetime<=convert(datetime,'" + trxn.TrxnDatetime.Value.AddSeconds(double.Parse(System.Configuration.ConfigurationManager.AppSettings["SecondsBetweenTransacionsInEJAndCCMS"])).ToString("dd/MM/yyyy HH:mm:ss") + "',103)" +
                        //                                                                     " and atm_id =" + trxn.AtmId.Value);
                        //StringBuilder builder = new StringBuilder();
                        //builder.Append("<table>");
                        //while (CCMSParsedTrxn.Read())
                        //{
                        //    builder.Append("<tr><td>DateTime</td><td>" + CCMSParsedTrxn.CurrentParsedTransaction.TrxnDatetime.ToString("dd/MM/yyyy HH:mm:ss") + "</td></tr>");
                        //    builder.Append("<tr><td>Amount</td><td>" + CCMSParsedTrxn.CurrentParsedTransaction.Amount + "</td></tr>");
                        //}
                        //builder.Append("</table>");
                        //CCMSParsedTrxn.Close();
                        //Literal_TrxnInCCMS.Text = builder.ToString();
                    }
                    else
                    {
                        if (BNATrxn != null)
                        {
                            Literal_EJ.Text = ej.Substring(BNATrxn.StartIndex, BNATrxn.EndIndex - BNATrxn.StartIndex).Replace("\r\n", "<br/>").Replace("\r", "<br/>").Replace("\n", "<br/>");
                            Label_Datetime.Text = BNATrxn.TrxnDatetime.Value.ToString("dd/MM/yyyy HH:mm:ss");
                            Label_PAN.Text = BNATrxn.Pan;
                            Label_HostTSN.Text = BNATrxn.HostTsn;
                            Label_TSN.Text = BNATrxn.Seq;
                            Label_Amount.Text = BNATrxn.AmountAuthorized.ToString();
                            Label_Status.Text = BNATrxn.Status;
                            //if (BNATrxn.Status == "0")
                            //    Label_Status.Text = "Successful";
                            //else if (BNATrxn.Status == "1")
                            //    Label_Status.Text = "Failed";
                            //else if (BNATrxn.Status == "2")
                            //    Label_Status.Text = "Suspicious";
                        }
                        else
                        {
                            Literal_EJ.Text = ej.Substring(CPMTrxn.StartIndex, CPMTrxn.EndIndex - CPMTrxn.StartIndex).Replace("\r\n", "<br/>").Replace("\r", "<br/>").Replace("\n", "<br/>");
                            Label_Datetime.Text = CPMTrxn.TrxnDatetime.Value.ToString("dd/MM/yyyy HH:mm:ss");
                            Label_PAN.Text = CPMTrxn.Pan;
                            Label_HostTSN.Text = CPMTrxn.HostTsn;
                            Label_TSN.Text = CPMTrxn.Seq;
                            Label_Amount.Text = CPMTrxn.DepositAmount.ToString();
                            Label_Status.Text = CPMTrxn.Status;
                            //if (CPMTrxn.Status == "0")
                            //    Label_Status.Text = "Successful";
                            //else if (CPMTrxn.Status == "1")
                            //    Label_Status.Text = "Failed";
                            //else if (CPMTrxn.Status == "2")
                            //    Label_Status.Text = "Suspicious";
                        }
                        // ParsedTransaction.ParsedTransactionReader CCMSParsedTrxn = ParsedTransaction.ExecuteReader("trxn_datetime>=convert(datetime,'" + trxn.TrxnDatetime.Value.ToString("dd/MM/yyyy HH:mm:ss") + "',103) and " +
                        //                                                                     "trxn_datetime<=convert(datetime,'" + trxn.TrxnDatetime.Value.AddSeconds(double.Parse(System.Configuration.ConfigurationManager.AppSettings["SecondsBetweenTransacionsInEJAndCCMS"])).ToString("dd/MM/yyyy HH:mm:ss") + "',103)" +
                        //                                                                     " and atm_id =" + trxn.AtmId.Value);
                        //StringBuilder builder = new StringBuilder();
                        //builder.Append("<table>");
                        //while (CCMSParsedTrxn.Read())
                        //{
                        //    builder.Append("<tr><td>DateTime</td><td>" + CCMSParsedTrxn.CurrentParsedTransaction.TrxnDatetime.ToString("dd/MM/yyyy HH:mm:ss") + "</td></tr>");
                        //    builder.Append("<tr><td>Amount</td><td>" + CCMSParsedTrxn.CurrentParsedTransaction.Amount + "</td></tr>");
                        //}
                        //builder.Append("</table>");
                        //CCMSParsedTrxn.Close();
                        //Literal_TrxnInCCMS.Text = builder.ToString();
                    }





                   
                }


            }
        }
    }
}