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
using Avanza.CCMS.DAL;

using System.Data.SqlClient;
using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using Avanza.iSuite.UI;
using System.Diagnostics;


namespace CCMSUI.CCMS
{
    public partial class EjViewerWithOptions : BasePage
    {
        //protected System.Web.UI.WebControls.CheckBoxList CHECKBOXLIST_devices;
        protected System.Web.UI.WebControls.Button Button_Apply;
        //    Regex formattingRegex = new Regex("([[]00p)|([[]05p)|([[]020t)|([[]0r[(]1[)]2[[]000p[[]040qe1w3h162)|([(]D)|([(]1)|([(]C)" +
        //"|([[]040)|([[]000p)|(/)|(q[(]I)|(q[(]1)|([(]7)|([(]>)");
        static string dropdownValue;

        Regex formattingRegex = new Regex(Utility.GetFormatRegex());


        //Regex formattingRegex = new Regex("([[]00p)|([[]05p)|([[]020t)|([[]0r[(]1[)]2[[]000p[[]040qe1w3h162)|([(]D)|([(]1)|([(]C)" +
        //    "|([[]040)|([[]000p)");
        protected string formattedStr, selectedTransactionEntry;
        //Logger Log = Logger.getInstance();
        //protected System.Web.UI.WebControls.Literal Literal_js;
        protected StringBuilder result;

        protected void Page_Load(object sender, EventArgs e)
        {


        }
        private void ShowAnalysis(ref string formattedStr)
        {
            StringBuilder pattern = new StringBuilder(100);
            //*0001*1*D*0,M-09,R-0
            //*0001*1*D
            pattern.Append(@"\*\d{4}\*\d{1}\*(?<Device>\-"); // dummy cahr \-

            for (int i = 0; i < CHECKBOXLIST_devices.Items.Count; i++)
            {
                if (CHECKBOXLIST_devices.Items[i].Selected)
                    pattern.Append("|" + CHECKBOXLIST_devices.Items[i].Value);
            }
            //*0,M-09,R-0
            pattern.Append(@")[*](?<DeviceCode>\d)\d*[,][ ]*(?<mState>M[-]\d+)[,][ ]*R[-]\d+");
            //lstAnalysis.Items.Add("Line no :"+(i+1));
            Regex exp = new Regex(pattern.ToString());
            Match match = exp.Match(formattedStr);
            result = new StringBuilder(1000);
            if (!match.Success)
                result.Append("<b>No Match found.</b>");


            while (match.Success)
            {
                result.AppendFormat("<li><div><a href='ejlogviewer2.aspx?s={0}&l={1}'><b>{2}</b><br/>", match.Index, match.Length, "<a href=#" + match.ToString().Split('*')[1] + ">" + match.ToString() + "</a>");
                result.Append("index=" + match.Index + " length = " + match.Length + "<br/>");

                MState mstate = MState.LoadMState("device_id = '" + match.Groups["Device"].Captures[0]
                    + "' and mstate_code ='" + match.Groups["mState"].Captures[0] + "'");
                if (mstate == null)
                    result.Append("No m-state entry found in database<br/>");
                else
                {
                    result.Append("Status:  " + mstate.MstateStatus + "<br/>");
                    result.Append("Description:  " + mstate.MstateDesc + "<br/>");
                }

                if ((match.Groups["Device"].Captures[0].Value.Equals("E")) && (match.Groups["DeviceCode"].Captures[0].Value.Equals("5")))
                    result.Append("Bills Retracted<br/>");

                result.Append("</a><br/></div></li>");

                formattedStr = formattedStr.Replace(match.ToString(), "<a name = " + match.ToString().Split('*')[1] + ">" + match.ToString() + "</a>");
                match = match.NextMatch();

            }
        }
        public static String GetDecodedString(int taskID, out int uncompressedLength)
        {
            return Encoding.ASCII.GetString(GetUnCompressedBytes(taskID, out uncompressedLength), 0, uncompressedLength);
        }

        protected void button_search_Click(object sender, EventArgs e)
        {
            //task id issue here..... from where to get it.......

            //We have to read from ijournal3....

            SqlConnection conn = ConnectionFactory.GetNewConnection();
            try
            {
                conn.Open();
                conn.ChangeDatabase("ijournal3");
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "select file_download_info_id from file_download_info where file_path_at_atm = '" + TextBox_FilePath.Text + "'";
                object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    //Task objatmlog = Task.LoadTask("file_path_at_atm = '" + TextBox_FilePath.Text + "'");
                    // Convert.ToInt32(Request.Params["file_id"]));
                    dropdownValue = Dropdownlist_ejstoreYear.SelectedValue.ToString();
                    int uncompressedLength;
                    //byte[] buff = GetUnCompressedBytes(objatmlog, FileType.LoadFileTypeByPk(objatmlog.FileTypeId.Value), out uncompressedLength);
                    string ejLog = EjViewerWithOptions.GetDecodedString(int.Parse(result.ToString()), out uncompressedLength);
                    Regex regex = new Regex(@"\d+aLUNO123a");
                    Match match = regex.Match(ejLog);
                    if (match.Success)
                        formattedStr = regex.Replace(ejLog, "").Replace('a', '\r');
                    else
                        formattedStr = formattingRegex.Replace(ejLog, "");

                    ShowAnalysis(ref formattedStr);
                }
                else
                    Literal_Msg.Text = "<li><span style='color:red'>No record found</span></li>";
            }
            catch (Exception ex)
            {
                Literal_Msg.Text = "<li><span style='color:red'>" + ex.Message + "</span></li>";
                LogableTask.LogMonoActivityTask("displayEJ", System.Reflection.MethodBase.GetCurrentMethod(), TraceLevel.Info, ex);
            }
            finally
            {
                conn.Close();
            }
        }

        public static byte[] GetUnCompressedBytes(int taskID, out int uncompressedLength)
        {
            MemoryStream ms = null;
            SqlConnection conn = ConnectionFactory.GetNewConnection();
            conn.Open();

            conn.ChangeDatabase("ejstore_" + dropdownValue);
            SqlCommand cmd = new SqlCommand("select ej_file from EJ_Files where file_download_info_id = " + taskID, conn);
            ms = new MemoryStream((byte[])cmd.ExecuteScalar());
            conn.Close();

            ZipInputStream compressedBytes = new ZipInputStream(ms);
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
    }
}