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
using System.Text.RegularExpressions;
using System.Text;

using System.Data.SqlClient;
using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using Avanza.iSuite.UI;
using Avanza.CCMS.DAL;
using System.Globalization;
using System.Data.OleDb;
using System.Xml;

namespace CCMSUI.CCMS
{
    public partial class CashDataViewer : BasePage
    {
        protected System.Web.UI.WebControls.Button Button_Apply;
        protected string selectedTransactionEntry;
        protected StringBuilder result;

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                Button_ShowOriginalView_Click(this, new EventArgs());
                string newHeight = (int.Parse(Session["height"].ToString()) - 350).ToString() + "px";
                string newWidth = (int.Parse(Session["width"].ToString()) - 60).ToString() + "px";
                Div_DisplayCounterFile.Style.Add("height", newHeight);
                Div_DisplayCounterFile.Style.Add("width", newWidth);
                Session["LoadGrid"] = true;
            }

        }

        public static String GetStringForFileType3(VTask task, FileType fileTypeInfo, out int uncompressedLength, AppSetting appSetting)
        {

            byte[] data = CashDataViewer.GetUnCompressedBytes(task, FileType.LoadFileTypeByPk(task.FileTypeId.Value), out uncompressedLength);
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
            return builder1.ToString();

        }

        public static String GetDecodedString(VTask task, FileType fileTypeInfo, out int uncompressedLength)
        {
            if (bool.Parse(ConfigurationManager.AppSettings["UseASCIIEncodingForEj"]))
                return Encoding.ASCII.GetString(GetUnCompressedBytes(task, fileTypeInfo, out uncompressedLength), 0, uncompressedLength);
            else if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["UseUnicodeForEj"]) && 
                bool.Parse(ConfigurationManager.AppSettings["UseUnicodeForEj"]))
                return Encoding.Unicode.GetString(GetUnCompressedBytes(task, fileTypeInfo, out uncompressedLength), 0, uncompressedLength);
            else
                return Encoding.UTF8.GetString(GetUnCompressedBytes(task, fileTypeInfo, out uncompressedLength), 0, uncompressedLength);
        }

        public static string GetUnCompressedBytes(byte[] cashDataFile, out int uncompressedLength)
        {
            MemoryStream baseInputStream = new MemoryStream(cashDataFile);
            ZipInputStream zipInputStream = new ZipInputStream((Stream)baseInputStream);
            string appSetting = ConfigurationManager.AppSettings["zipPassword"];
            if (appSetting.Length > 0)
                zipInputStream.Password = appSetting;
            byte[] numArray = new byte[zipInputStream.GetNextEntry().Size];
            byte[] buffer = new byte[2048];
            MemoryStream memoryStream = new MemoryStream();
            if (numArray.Length != 0)
            {
                int count;
                do
                {
                    count = zipInputStream.Read(buffer, 0, buffer.Length);
                    memoryStream.Write(buffer, 0, count);
                }
                while (count > 0);
            }
            zipInputStream.Close();
            baseInputStream.Close();
            uncompressedLength = (int)memoryStream.Length;
            if (bool.Parse(ConfigurationManager.AppSettings["UseASCIIEncodingForEj"]))
                return Encoding.ASCII.GetString(memoryStream.GetBuffer());
            return !string.IsNullOrEmpty(ConfigurationManager.AppSettings["UseUnicodeForEj"]) && bool.Parse(ConfigurationManager.AppSettings["UseUnicodeForEj"]) ? Encoding.Unicode.GetString(memoryStream.GetBuffer()) : Encoding.UTF8.GetString(memoryStream.GetBuffer());
        }

        public static byte[] GetUnCompressedBytes(VTask task, FileType fileTypeInfo, out int uncompressedLength)
        {
            MemoryStream ms = null;

            if (fileTypeInfo.IsEJLog)
            {
                SqlConnection conn = ConnectionFactory.GetNewConnection();
                conn.Open();
                string name = ConfigurationManager.AppSettings["keepOneCashDataStoreName"];
                if (name.Length == 0)
                    conn.ChangeDatabase("CashDataStore_" + task.CreationTime.Year.ToString());
                else
                    conn.ChangeDatabase(name);
                SqlCommand cmd = new SqlCommand("select cash_Data_file from CashDataFiles where task_id = " + task.TaskId, conn);
                ms = new MemoryStream((byte[])cmd.ExecuteScalar());
                conn.Close();
            }
            else
            {
                byte[] buff = new byte[task.ZippedFileSize.Value];
                FileStream fs = new FileStream(task.ServerFilepath, FileMode.Open);
                fs.Read(buff, 0, buff.Length);
                fs.Close();
                ms = new MemoryStream(buff);
            }

            ZipInputStream compressedBytes = new ZipInputStream(ms);
            string pwd = System.Configuration.ConfigurationManager.AppSettings["zipPassword"];
            if (pwd.Length > 0)
                compressedBytes.Password = pwd;
            ZipEntry entry = compressedBytes.GetNextEntry();
            byte[] uncompressedBytes = new byte[entry.Size];
            byte[] tempBuffer = new byte[2048];
            MemoryStream tempMemoryStream = new MemoryStream();
            int bytesRead = 0;

            if (uncompressedBytes.Length > 0)
            {
                do
                {
                    bytesRead = compressedBytes.Read(tempBuffer, 0, tempBuffer.Length);
                    tempMemoryStream.Write(tempBuffer, 0, bytesRead);
                } while (bytesRead > 0);
            }

            compressedBytes.Close();
            ms.Close();
            uncompressedLength = (int)tempMemoryStream.Length;

            return tempMemoryStream.GetBuffer();
        }

        private static bool IsNumber(string Value)
        {
            int value;

            return (int.TryParse(Value, out value));
        }

        public static string FormatInformation(string Data)
        {
            EntryTypes type;
            string[] tokens;
            string orderid, orderdate;
            StringBuilder builder;

            builder = new StringBuilder();
            tokens = Data.Split(new char[] { '|', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < tokens.Length; )
            {
                builder.Append(tokens[i++]);
                builder.Append(' ');

                type = (EntryTypes)Enum.Parse(typeof(EntryTypes), tokens[i++]);

                builder.Append(type);
                builder.Append(' ');

                switch (type)
                {
                    case EntryTypes.ServiceState:
                        builder.AppendFormat("[{0}]", tokens[i++]);
                        break;

                    case EntryTypes.CurrentMode:
                        builder.AppendFormat("[{0}]", tokens[i++]);
                        break;

                    case EntryTypes.CassetteMode:
                        builder.AppendFormat("[{0}] NoOfCassettes[{1}]", tokens[i++], tokens[i++]);
                        break;

                    case EntryTypes.ModeChanged:
                        builder.AppendFormat("CurrentMode[{0}] LastMode[{1}]", tokens[i++], tokens[i++]);
                        builder.AppendFormat("\r\n\tLastRemaining:");
                        for (int j = 0; j < 7; j++)
                        {
                            builder.AppendFormat("\tCassette{0}[{1}]", j + 1, tokens[i++]);
                        }
                        builder.AppendFormat("\r\n\tLastDispensed:");
                        for (int j = 0; j < 7; j++)
                        {
                            builder.AppendFormat("\tCassette{0}[{1}]", j + 1, tokens[i++]);
                        }
                        builder.AppendFormat("\r\n\tLastPurged:");
                        for (int j = 0; j < 7; j++)
                        {
                            builder.AppendFormat("\tCassette{0}[{1}]", j + 1, tokens[i++]);
                        }
                        break;

                    case EntryTypes.CashWithdrawal:
                        builder.AppendFormat("Amount[{0}]\r\nLastRemaining:\tCassette1[{1}]\tCassette2[{2}]\tCassette3[{3}]\tCassette4[{4}]\tCassette5[{5}]\tCassette6[{6}]\tCassette7[{7}]\r\nDispensed:\t\tCassette1[{8}]\tCassette2[{9}]\tCassette3[{10}]\tCassette4[{11}]\tCassette5[{12}]\tCassette6[{13}]\tCassette7[{14}]\r\nPurged:\t\t\tCassette1[{15}]\tCassette2[{16}]\tCassette3[{17}]\tCassette4[{18}]\tCassette5[{19}]\tCassette6[{20}]\tCassette7[{21}]\r\nDenomination:\tCassette1[{22}]\tCassette2[{23}]\tCassette3[{24}]\tCassette4[{25}]\tCassette5[{26}]\tCassette6[{27}]\tCassette7[{28}]", tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++]);
                        break;

                    case EntryTypes.CountsCleared:
                        builder.AppendFormat("\r\nRemaining:\t\tCassette1[{0}]\tCassette2[{1}]\tCassette3[{2}]\tCassette4[{3}]\tCassette5[{4}]\tCassette6[{5}]\tCassette7[{6}]\r\nDispensed:\t\tCassette1[{7}]\tCassette2[{8}]\tCassette3[{9}]\tCassette4[{10}]\tCassette5[{11}]\tCassette6[{12}]\tCassette7[{13}]\r\nPurged:\t\t\tCassette1[{14}]\tCassette2[{15}]\tCassette3[{16}]\tCassette4[{17}]\tCassette5[{18}]\tCassette6[{19}]\tCassette7[{20}]\r\nDenomination:\tCassette1[{21}]\tCassette2[{22}]\tCassette3[{23}]\tCassette4[{24}]\tCassette5[{25}]\tCassette6[{26}]\tCassette7[{27}]", tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++]);
                        break;

                    case EntryTypes.AddCash:
                        builder.AppendFormat("LastRemaining:\tCassette1[{0}]\tCassette2[{1}]\tCassette3[{2}]\tCassette4[{3}]\tCassette5[{4}]\tCassette6[{5}]\tCassette7[{6}]\r\nAdded:\t\t\tCassette1[{7}]\tCassette2[{8}]\tCassette3[{9}]\tCassette4[{10}]\tCassette5[{11}]\tCassette6[{12}]\tCassette7[{13}]\r\nDenomination:\tCassette1[{14}]\tCassette2[{15}]\tCassette3[{16}]\tCassette4[{17}]\tCassette5[{18}]\tCassette6[{19}]\tCassette7[{20}]", tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++]);
                        break;

                    case EntryTypes.CountsSet:
                        builder.AppendFormat("OrderId[{0}]\r\nSet:\t\t\tCassette1[{1}]\tCassette2[{2}]\tCassette3[{3}]\tCassette4[{4}]\tCassette5[{5}]\tCassette6[{6}]\tCassette7[{7}]\r\nDenomination:\tCassette1[{8}]\tCassette2[{9}]\tCassette3[{10}]\tCassette4[{11}]\tCassette5[{12}]\tCassette6[{13}]\tCassette7[{14}]", tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++]);
                        break;

                    case EntryTypes.RetriesExhausted:
                        builder.AppendFormat("Threshold[{0}] ActualTries[{1}]", tokens[i++], tokens[i++]);
                        break;

                    case EntryTypes.OrderMissing:
                        orderid = tokens[i++];
                        // Work around for checking whether the order date is present or not.
                        orderdate = string.Empty;
                        if (i < tokens.Length && tokens[i].Length == 8 && IsNumber(tokens[i]))
                        {
                            orderdate = tokens[i++];
                        }

                        builder.AppendFormat("LastOrderId[{0}] LastOrderDate[{1}]", orderid, ((string.IsNullOrEmpty(orderdate)) ? string.Empty : DateTime.ParseExact(orderdate, "yyyyMMdd", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy")));
                        break;

                    case EntryTypes.TestCash:
                        builder.AppendFormat("\r\nLastRemaining:\tCassette1[{0}]\tCassette2[{1}]\tCassette3[{2}]\tCassette4[{3}]\tCassette5[{4}]\tCassette6[{5}]\tCassette7[{6}]\r\nPurged:\t\t\tCassette1[{7}]\tCassette2[{8}]\tCassette3[{9}]\tCassette4[{10}]\tCassette5[{11}]\tCassette6[{12}]\tCassette7[{13}]", tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++]);
                        break;

                    case EntryTypes.EODBalance:
                        builder.AppendFormat("Date[{0:MM/dd/yyyy}]\r\nRemaining:\t\tCassette1[{1}]\tCassette2[{2}]\tCassette3[{3}]\tCassette4[{4}]\tCassette5[{5}]\tCassette6[{6}]\tCassette7[{7}]\r\nDispensed:\t\tCassette1[{8}]\tCassette2[{9}]\tCassette3[{10}]\tCassette4[{11}]\tCassette5[{12}]\tCassette6[{13}]\tCassette7[{14}]\r\nPurged:\t\t\tCassette1[{15}]\tCassette2[{16}]\tCassette3[{17}]\tCassette4[{18}]\tCassette5[{19}]\tCassette6[{20}]\tCassette7[{21}]\r\nDenomination:\tCassette1[{22}]\tCassette2[{23}]\tCassette3[{24}]\tCassette4[{25}]\tCassette5[{26}]\tCassette6[{27}]\tCassette7[{28}]", DateTime.ParseExact(tokens[i++], "yyyyMMdd", CultureInfo.InvariantCulture), tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++], tokens[i++]);
                        break;

                    case EntryTypes.Replenishment:
                        builder.AppendFormat("Mode[{0}] StartTime[{1:MM/dd/yyyy HH:mm:ss}] EndTime[{2:MM/dd/yyyy HH:mm:ss}] OrderId[{3}]", tokens[i++], DateTime.ParseExact(tokens[i++], "yyyyMMddHHmmss", CultureInfo.InvariantCulture), DateTime.ParseExact(tokens[i++], "yyyyMMddHHmmss", CultureInfo.InvariantCulture), tokens[i++]);
                        builder.AppendFormat("\r\n\tLastRemaining:");
                        for (int j = 0; j < 7; j++)
                        {
                            builder.AppendFormat("\tCassette{0}[{1}]", j + 1, tokens[i++]);
                        }
                        builder.AppendFormat("\r\n\tLastDispensed:");
                        for (int j = 0; j < 7; j++)
                        {
                            builder.AppendFormat("\tCassette{0}[{1}]", j + 1, tokens[i++]);
                        }
                        builder.AppendFormat("\r\n\tLastPurged:");
                        for (int j = 0; j < 7; j++)
                        {
                            builder.AppendFormat("\tCassette{0}[{1}]", j + 1, tokens[i++]);
                        }
                        builder.AppendFormat("\r\n\tReplenished:");
                        for (int j = 0; j < 7; j++)
                        {
                            builder.AppendFormat("\tCassette{0}[{1}]", j + 1, tokens[i++]);
                        }
                        break;

                    case EntryTypes.Mismatch:
                        builder.AppendFormat("ServiceState[{0}] CurrentMode[{1}]", tokens[i++], tokens[i++]);
                        builder.AppendFormat("\r\n\tLastTotal:");
                        for (int j = 0; j < 7; j++)
                        {
                            builder.AppendFormat("\tCassette{0}[{1}]", j + 1, tokens[i++]);
                        }
                        builder.AppendFormat("\r\n\tLastRemaining:");
                        for (int j = 0; j < 7; j++)
                        {
                            builder.AppendFormat("\tCassette{0}[{1}]", j + 1, tokens[i++]);
                        }
                        builder.AppendFormat("\r\n\tLastDispensed:");
                        for (int j = 0; j < 7; j++)
                        {
                            builder.AppendFormat("\tCassette{0}[{1}]", j + 1, tokens[i++]);
                        }
                        builder.AppendFormat("\r\n\tLastPurged:");
                        for (int j = 0; j < 7; j++)
                        {
                            builder.AppendFormat("\tCassette{0}[{1}]", j + 1, tokens[i++]);
                        }
                        builder.AppendFormat("\r\n\tCurrentTotal:");
                        for (int j = 0; j < 7; j++)
                        {
                            builder.AppendFormat("\tCassette{0}[{1}]", j + 1, tokens[i++]);
                        }
                        builder.AppendFormat("\r\n\tCurrentRemaining:");
                        for (int j = 0; j < 7; j++)
                        {
                            builder.AppendFormat("\tCassette{0}[{1}]", j + 1, tokens[i++]);
                        }
                        builder.AppendFormat("\r\n\tCurrentDispensed:");
                        for (int j = 0; j < 7; j++)
                        {
                            builder.AppendFormat("\tCassette{0}[{1}]", j + 1, tokens[i++]);
                        }
                        builder.AppendFormat("\r\n\tCurrentPurged:");
                        for (int j = 0; j < 7; j++)
                        {
                            builder.AppendFormat("\tCassette{0}[{1}]", j + 1, tokens[i++]);
                        }
                        break;

                    case EntryTypes.BNAMismatch:
                        builder.AppendFormat("ServiceState[{0}] CurrentMode[{1}]\r\nLast Counts:", tokens[i++], tokens[i++]);
                        for (int j = 0; j < 5; j++)
                        {
                            for (int k = 0; k < 50; k++)
                            {
                                builder.AppendFormat("\r\nCassette[{0}] Denomination[{1}]\t\tCount[{2}]", j + 1, k + 1, tokens[i++]);
                            }
                        }
                        builder.AppendFormat("\r\nCurrent Counts:");
                        for (int j = 0; j < 5; j++)
                        {
                            for (int k = 0; k < 50; k++)
                            {
                                builder.AppendFormat("\r\nCassette[{0}] Denomination[{1}]\t\tCount[{2}]", j + 1, k + 1, tokens[i++]);
                            }
                        }
                        break;

                    case EntryTypes.CPMMismatch:
                        builder.AppendFormat("ServiceState[{0}] CurrentMode[{1}]\r\nLast Counts:", tokens[i++], tokens[i++]);
                        for (int j = 0; j < 3; j++)
                        {
                            builder.AppendFormat("\r\nBIN[{0}]\t\tCount[{1}]", j + 1, tokens[i++]);
                        }
                        builder.AppendFormat("\r\nCurrent Counts:");
                        for (int j = 0; j < 3; j++)
                        {
                            builder.AppendFormat("\r\nBIN[{0}]\t\tCount[{1}]", j + 1, tokens[i++]);
                        }
                        break;

                    case EntryTypes.CashDeposit:
                        for (int j = 0; j < 5; j++)
                        {
                            for (int k = 0; k < 50; k++)
                            {
                                builder.AppendFormat("\r\nCassette[{0}] Denomination[{1}]\t\tLastCount[{2}]", j + 1, k + 1, tokens[i++]);
                            }
                        }
                        for (int j = 0; j < 5; j++)
                        {
                            for (int k = 0; k < 50; k++)
                            {
                                builder.AppendFormat("\r\nCassette[{0}] Denomination[{1}]\t\tDepositedNotes[{2}]", j + 1, k + 1, tokens[i++]);
                            }
                        }
                        break;

                    case EntryTypes.ChequeDeposit:
                        for (int j = 0; j < 3; j++)
                        {
                            builder.AppendFormat("\r\nBIN[{0}]\t\tLastCount[{1}]", j + 1, tokens[i++]);
                        }
                        for (int j = 0; j < 3; j++)
                        {
                            builder.AppendFormat("\r\nBIN[{0}]\t\tDepositedCheques[{1}]", j + 1, tokens[i++]);
                        }
                        break;

                    case EntryTypes.BNACountsCleared:
                        for (int j = 0; j < 5; j++)
                        {
                            for (int k = 0; k < 50; k++)
                            {
                                builder.AppendFormat("\r\nCassette[{0}] Denomination[{1}]\t\tLastCount[{2}]", j + 1, k + 1, tokens[i++]);
                            }
                        }
                        break;

                    case EntryTypes.CPMCountsCleared:
                        for (int j = 0; j < 3; j++)
                        {
                            builder.AppendFormat("\r\nBIN[{0}]\t\tLastCount[{1}]", j + 1, tokens[i++]);
                        }
                        break;

                    case EntryTypes.BNAEODBalance:
                        builder.AppendFormat("Date[{0:MM/dd/yyyy}]", DateTime.ParseExact(tokens[i++], "yyyyMMdd", CultureInfo.InvariantCulture));
                        for (int j = 0; j < 5; j++)
                        {
                            for (int k = 0; k < 50; k++)
                            {
                                builder.AppendFormat("\r\nCassette[{0}] Denomination[{1}]\t\tLastCount[{2}]", j + 1, k + 1, tokens[i++]);
                            }
                        }
                        break;

                    case EntryTypes.CPMEODBalance:
                        builder.AppendFormat("Date[{0:MM/dd/yyyy}]", DateTime.ParseExact(tokens[i++], "yyyyMMdd", CultureInfo.InvariantCulture));
                        for (int j = 0; j < 3; j++)
                        {
                            builder.AppendFormat("\r\nBIN[{0}]\t\tLastCount[{1}]", j + 1, tokens[i++]);
                        }
                        break;
                }

                builder.Append("\r\n\r\n");
            }

            return builder.ToString();
        }

        private string GetFileFromCashDataStore()
        {
           // if (Session["task_id"] != null)
             //   Session["file_id"] = Request.Params["file_id"];
            VTask objatmlog = VTask.LoadvTaskByPk(Convert.ToInt32(Session["task_id"]));
            int uncompressedLength;

            if (objatmlog.FileTypeId == 3 || objatmlog.FileTypeId == 5 || objatmlog.FileTypeId == 13 || objatmlog.FileTypeId == 14 || objatmlog.FileTypeId == 15)
            {
                byte[] data = CashDataViewer.GetUnCompressedBytes(objatmlog, FileType.LoadFileTypeByPk(objatmlog.FileTypeId.Value), out uncompressedLength);
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
                return builder1.ToString();
            }
            else
                return CashDataViewer.GetDecodedString(objatmlog, FileType.LoadFileTypeByPk(objatmlog.FileTypeId.Value), out uncompressedLength);
        }
        protected void Button_ShowOriginalView_Click(object sender, EventArgs e)
        {
            string isXml = System.Configuration.ConfigurationManager.AppSettings["isXmlFile"];
            if (isXml == "1")
            {
                Literal_data.Text = Server.HtmlEncode(GetFileFromCashDataStore()).Replace("\r\n", "<br/>");
                Button_ShowFriendlyView.Enabled = false;
                Button_ShowOriginalView.Enabled = false;
            }
            else
                Literal_data.Text = GetFileFromCashDataStore().Replace("\r\n", "<br/>");
        }

        protected void Button_ShowFriendlyView_Click(object sender, EventArgs e)
        {
            Literal_data.Text = FormatInformation(GetFileFromCashDataStore()).Replace("\r\n", "<br/>");
        }
    }
}