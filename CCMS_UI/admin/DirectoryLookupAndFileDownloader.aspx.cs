using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net.Sockets;
using Avanza.CCMS.DAL;
using System.Reflection;
using System.Diagnostics;
using System.Text;
using ceTe.DynamicPDF.PageElements;
using Avanza.iSuite.UI;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Data;
using System.Web.Services;
using System.Web.Script.Services;

namespace CCMSUI.admin
{
    public partial class DirectoryLookupAndFileDownloader : BasePage
    {
        Socket soc = null;
        string version = "CCMS";
        LogableTask task;
        string[] lines;
        byte[] buff = new byte[2048];
        int threadTimeOut = string.IsNullOrEmpty(System.Web.Configuration.WebConfigurationManager.AppSettings["threadTimeout"]) ? 2000 : int.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["threadTimeout"]);
        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        }
        private void UpdateLabel()
        {
            try
            {
                if ((bool)Session[SessionVars.isRegionSelected.ToString()] || Session[SessionVars.SelectedATMs.ToString()].ToString().IndexOf(',') > -1 ||
                        Session[SessionVars.SelectedATMs.ToString()].ToString().Length == 0)
                    Label_SelectedATM.Text = "Please, select an ATM";
                else
                {
                    if ((string)Session[SessionVars.SelectedATMs.ToString()] == "-1")
                    {
                        Literal_error.Text = "<li>ATM not selected.</li>";
                        //Button_Download.Enabled = false;
                    }
                    else
                        Label_SelectedATM.Text = Atm.LoadAtm("atm_id =" + Session[SessionVars.SelectedATMs.ToString()].ToString()).Title;
                }
            }
            catch (Exception ex)
            {
                Literal_error.Text = "<li>" + ex.Message + "</li>";
                LogableTask task = LogableTask.NewTask("UpdateLabel");
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                task.EndTask();
            }

        }

        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            UpdateLabel();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    Avanza.CCMS.DAL.FileType.FileTypeReader fileTypeReader = Avanza.CCMS.DAL.FileType.ExecuteReader("1=1");
                    while (fileTypeReader.Read())
                    {
                        //DropDownList_FileType.Items.Add(new System.Web.UI.WebControls.ListItem(fileTypeReader.CurrentFileType.FileTypeTitle, fileTypeReader.CurrentFileType.FileTypeId.ToString()));
                    }
                    fileTypeReader.Close();
                    UpdateLabel();

                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                Literal_error.Text = "<li>" + ex.Message + "</li>";
                LogableTask task = LogableTask.NewTask("Page_Load");
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                task.EndTask();
            }

        }
        private Socket GetConnectedSocket()
        {
            soc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            soc.ReceiveTimeout = 60000;
            Atm atm = Atm.LoadAtm("atm_id =" + Session[SessionVars.SelectedATMs.ToString()].ToString());
            soc.Connect(atm.IP, (int)atm.Port);
            return soc;
        }
        private void CloseSocket()
        {
            if (soc != null)
            {
                SendString("quit;");
                soc.Shutdown(SocketShutdown.Both);
                soc.Close(1000);
            }
        }
        
        protected void Button_GetFiles_Click(object sender, EventArgs e)
        {
            task = LogableTask.NewTask("GetFiles");
           
            try
            {
                if (Label_SelectedATM.Text.StartsWith("Please"))
                    throw new Exception("Please select ATM from the tree.");

                if (TextBox_FolderPath.Text.Length == 0)
                    throw new Exception("Please enter folder path.");

                
                string reply = DownloadFile();
                lines = reply.Split('\n');
                ViewState["sortDirection"] = " asc";
                ViewState["SortExpression"] = "file_type";
                BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());

                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "ATM Reply:" + reply);
                //if (!reply.Contains("no"))
                //{
                //    string[] parts = reply.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                //    foreach (string part in parts)
                //        generateNode(part);

                //}
                //else
                //    Literal_error.Text = "<li>" + reply + "</li>";
            }
            catch (Exception ex)
            {
                Literal_error.Text = "<li>" + ex.Message + "</li>";
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, ex);
            }
            finally
            {
                CloseSocket();
                task.EndTask();
            }
        }
        protected string DownloadFile()
        {
            
            soc = GetConnectedSocket();
            version = ReceiveVersion();
            SendString("INVENTORY|dir \"" + TextBox_FolderPath.Text + "\" > " + "\"C:\\Program Files\\NCR Aptra\\Advance NDC\\CCMSAgent\\Temporary\\OUT.TXT\"");
            string reply = ReceiveString();
            SendString("SEND|C:/Program Files/NCR Aptra/Advance NDC/CCMSAgent/Temporary/OUT.TXT" );
            reply = ReceiveString();
            SendString("OK");
            reply = ReceiveString();
            SendString("OK");
            reply = ReceiveString();
            return reply;
        }
        protected void lnkView_Click(object sender, EventArgs e)
        {
            task = LogableTask.NewTask("Right Click");
            string filetype = hdnFileType.Value;
            //filetype = filetype.Replace(" ", "");
            if (filetype.Length > 1 )
            {
                GetFileDownload(hdnFileName.Value);
            }
            else
            {
                //Folder Download logic here
            }

        }
        private void BindGrid( string orderby)
        {
            LogableTask task = LogableTask.NewTask("BindGrid");
            try
            {

                Regex rowregex = new Regex(@"(?<Date>\d{2}/\d{2}/\d{4})[ ]+(?<Time>\d{2}[:]\d{2})[ ]\w{2}[ ]+(<DIR>[ ]+)*(?<Size>\d+([,]\d+)*[ ]+)*(?<Filename>((\w*[ ]*[.]*[(]*[)]*[-]*)*)|[.]+\w*)");
                DataTable dtFile_Copy = new DataTable();
                dtFile_Copy.Columns.Add("file_type", typeof(string));
                dtFile_Copy.Columns.Add("file_name", typeof(string));
                dtFile_Copy.Columns.Add("file_date_modified", typeof(DateTime));
                dtFile_Copy.Columns.Add("file_size", typeof(string));

                Match row = null;

                foreach (string line in lines)
                {
                    if (rowregex.IsMatch(line))
                    {
                        string file_name, file_size, file_type, datetime;
                        row = rowregex.Match(line);
                        file_name = row.Groups["Filename"].Value;
                        datetime = row.Groups["Date"].Value + " " + row.Groups["Time"].Value;

                        if (row.Groups["Size"].Success)
                        {
                            file_size = row.Groups["Size"].Value;
                            file_type = "../images/signs.png";
                        }
                        else
                        {
                            file_size = null;
                            file_type = "../images/interface.png";
                        }

                        dtFile_Copy.Rows.Add(new object[] { file_type, file_name, datetime, file_size});
                    }
                }

                DataView dv = dtFile_Copy.AsDataView();
                dv.Sort = orderby;
                GridView_LookupFile.DataSource = dv;
                GridView_LookupFile.DataBind();
                GridView_LookupFile.UseAccessibleHeader = true;
                GridView_LookupFile.HeaderRow.TableSection = TableRowSection.TableHeader;  
            }
            catch (Exception ex)
            { task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex); }
            finally
            { task.EndTask(); }
        }


        //protected void Button_Download_Click(object sender, EventArgs e)
        //{

        //    task = LogableTask.NewTask("Download");
        //    try
        //    {
        //        string reply = null;
        //        if (!TreeView_Dir.SelectedNode.Value.Contains('|'))
        //            throw new Exception("Please select GET FILE SIZE before downloading.");
        //        string[] parts = TreeView_Dir.SelectedNode.Value.Split('|', '=');

        //        try
        //        {
        //            soc = GetConnectedSocket();
        //            version = ReceiveVersion();
        //            SendString("PREPAREFORDOWNLOAD|" + parts[1]);
        //            reply = ReceiveString();
        //        }
        //        finally
        //        {
        //            CloseSocket();
        //        }
        //        if (reply.Contains("successfully"))
        //        {

        //            Atm atm = Atm.LoadAtm("atm_id =" + Session[SessionVars.SelectedATMs.ToString()].ToString());
        //            Task newTask = new Task();
        //            newTask.ATMId = atm.ATMId;
        //            newTask.BytesTransferred = 0;
        //            newTask.CreatedBy = 1;
        //            newTask.CreationTime = DateTime.Now;
        //            newTask.DownloadTime = new DateTime(1900, 1, 1);
        //            //newTask.FileTypeId = int.Parse(DropDownList_FileType.SelectedItem.Value);
        //            newTask.Parsed = false;
        //            newTask.RetryRemaining = (byte)atm.RetryCountCounterFile;//(byte)((AppSetting)Application[ApplicationVars.AppSettings.ToString()]).MaxTries;
        //            newTask.Status = DownloadStates.downloadingDisconnected.ToString();
        //            newTask.ZippedFileSize = int.Parse(parts[3]);
        //            newTask.FilePathAtATM = System.IO.Path.GetFileName(parts[1]);
        //            newTask.TaskTypeId = 1;
        //            newTask.Save();
        //            Session[SessionVars.notice.ToString()] = "Task scheduled for selected ATM";
        //        }
        //        else
        //        {
        //            Literal_error.Text = "<li>"+reply+"</li>";
        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        Literal_error.Text = "<li>" + ex.Message + "</li>";
        //        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, ex);
        //    }
        //    finally
        //    {

        //        task.EndTask();

        //    }

        //}


        void SimpleEncrypt(ref byte[] sourceArray)
        {
            for (int counter = 0; counter < sourceArray.Length; counter++)
                sourceArray[counter] = (byte)(sourceArray[counter] - ((counter + 1) % 10));
        }

        void SimpleDecrypt(ref byte[] sourceArray)
        {
            for (int counter = 0; counter < sourceArray.Length; counter++)
                sourceArray[counter] = (byte)(sourceArray[counter] + ((counter + 1) % 10));
        }

        public byte[] Receive(Socket socket, byte[] buffer, int offset, int size, int timeout)
        {

            Thread.Sleep(2000);
            // byte[] arr = null;
            //int length = this.soc.Receive(buff, buff.Length, SocketFlags.None);
            //string str = ASCIIEncoding.ASCII.GetString(buff, 0, size);
            int received = 0;  // how many bytes is already received
            do
            {
                try
                {
                    received += socket.Receive(buffer, offset + received, size - received, SocketFlags.None);
                    //str += ASCIIEncoding.ASCII.GetString(buffer, 0, size);
                }
                catch (SocketException ex)
                {
                    if (ex.SocketErrorCode == SocketError.WouldBlock ||
                        ex.SocketErrorCode == SocketError.IOPending ||
                        ex.SocketErrorCode == SocketError.NoBufferSpaceAvailable)
                    {
                        // socket buffer is probably empty, wait and try again
                        Thread.Sleep(30);
                    }
                    else
                        throw ex;
                }

            } while (received < size);
            //str = ASCIIEncoding.ASCII.GetString(buffer, 0, buffer.Length);
            if (!version.Contains("CCMS"))
            {
                //arr = Encoding.ASCII.GetBytes(str);
                SimpleDecrypt(ref buffer);
                //str = Encoding.ASCII.GetString(arr);
            }
            return buffer;
        }

        string ReceiveString()
        {
            Thread.Sleep(threadTimeOut);
            byte[] arr = null;
            int length = this.soc.Receive(buff, buff.Length, SocketFlags.None);
            string str = ASCIIEncoding.ASCII.GetString(buff, 0, length);
            while (this.soc.Available > 0 && length > 0)
            {
                length = soc.Receive(buff, buff.Length, SocketFlags.None);
                str += ASCIIEncoding.ASCII.GetString(buff, 0, length);
            }

            if (!version.Contains("CCMS"))
            {
                arr = Encoding.ASCII.GetBytes(str);
                SimpleDecrypt(ref arr);
                str = Encoding.ASCII.GetString(arr);
            }

            task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "received string = " + str);
            return str;
        }
        string ReceiveVersion()
        {
            byte[] arr = null;
            int length = this.soc.Receive(buff, buff.Length, SocketFlags.None);
            string str = ASCIIEncoding.ASCII.GetString(buff, 0, length);
            while (this.soc.Available > 0 && length > 0)
            {
                length = soc.Receive(buff, buff.Length, SocketFlags.None);
                str += ASCIIEncoding.ASCII.GetString(buff, 0, length);
            }

            //if (!str.Contains("CCMS"))
            //{
            //    arr = Encoding.ASCII.GetBytes(str);
            //    SimpleDecrypt(ref arr);
            //    str = Encoding.ASCII.GetString(arr);
            //}


            return str;
        }

        void SendString(string stringToSend)
        {
            byte[] stringToSendInBytes = Encoding.ASCII.GetBytes(stringToSend);
            if (!version.Contains("CCMS"))
            {
                SimpleEncrypt(ref stringToSendInBytes);

            }
            soc.Send(stringToSendInBytes);
            task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Verbose, "sent message = " + stringToSend);

        }

        protected void GridView_LookupFile_SelectedIndexChanged(object sender, EventArgs e)
        {
            task = LogableTask.NewTask("Double Click");
            if(GridView_LookupFile.SelectedRow.Cells[3].Text=="&nbsp;")
            {
                string str = TextBox_FolderPath.Text;
                if (GridView_LookupFile.SelectedRow.Cells[1].Text == ".")
                {
                    TextBox_FolderPath.Text = @"C:\";
                }
                else if(GridView_LookupFile.SelectedRow.Cells[1].Text == "..")
                {
                    int index = str.LastIndexOf(@"\");
                    TextBox_FolderPath.Text = str.Substring(0, index+1);
                }
                else
                {
                    if (str.Substring(str.Length - 1) == "\\")
                    {
                        TextBox_FolderPath.Text = TextBox_FolderPath.Text + GridView_LookupFile.SelectedRow.Cells[1].Text;
                    }
                    else
                    {
                        TextBox_FolderPath.Text = TextBox_FolderPath.Text + "\\" + GridView_LookupFile.SelectedRow.Cells[1].Text;
                    }
                }
                GridView_LookupFile.SelectedRow.BackColor = System.Drawing.Color.Red;
                string reply = DownloadFile();
                lines = reply.Split('\n');
            ViewState["sortDirection"] = " asc";
            ViewState["SortExpression"] = "file_type";

            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
            }
            else
            {
                //literal_script.Text = "<script type='text/javascript'>hideProgressBar()</script>";
                GetFileDownload(GridView_LookupFile.SelectedRow.Cells[1].Text);
                //literal_script.Text = "<script type='text/javascript'>hideProgressBar();</script>";
                
            }
            
        }
        protected void GetFileDownload(string filename)
        {
            try
            {
                AppSetting appSetting = (AppSetting)Application[ApplicationVars.AppSettings.ToString()];
                soc = GetConnectedSocket();
                version = ReceiveVersion();
                string reply = null;
                SendString("SEND|" + TextBox_FolderPath.Text.Replace('\\', '/') + "/" + filename);
                reply = ReceiveString();
                int totalbytes = Int32.Parse(string.IsNullOrEmpty(reply) ? reply : reply.Split('\0')[0]);
                reply = null;
                SendString("OK");
                reply = ReceiveString();
                int chunksize = Int32.Parse(string.IsNullOrEmpty(reply) ? reply : reply.Split('\0')[0]);
                SendString("OK");
                byte[] buffer = new byte[totalbytes];
                byte[] bytes = Receive(soc, buffer, 0, totalbytes, 10000);
                string tempPath = appSetting.TemporaryFolder + "\\";
                // byte[] bytes = ASCIIEncoding.ASCII.GetBytes(str);
                File.WriteAllBytes(tempPath + filename, bytes);
                Response.Redirect("Download.ashx?file=" + filename + "&tempPath=" + tempPath + "&bufferlength=" + Convert.ToString(buffer.Length), false);
                //Response.Clear();
                //Response.ContentType = "application/octet-stream";
                ////Response.ContentType = "application/x-zip-compressed";
                //Response.AddHeader("Content-Disposition", "attachment; filename=\"" + filename + "\"");
                //Response.AddHeader("Content-Length", Convert.ToString(buffer.Length));
                ////HttpContext.Current.Response.BinaryWrite(buffer);
                //Response.TransmitFile(tempPath + filename);
                //Response.Flush();
                //HttpContext.Current.Response.End();
            }
            catch (Exception ex)
            {
                Literal_error.Text = "<li>" + ex.Message + "</li>";
                LogableTask.LogMonoActivityTask("DownloadFile", MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
        }

        protected void GridView_LookupFile_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.Cells[3].Text == "&nbsp;")
                {
                    e.Row.Attributes.Add("ondblclick", "__doPostBack('GridView_LookupFile','Select$" + e.Row.RowIndex + "'); parent.document.getElementById('divWait').style.visibility = 'visible';");
                }
                else
                {
                    e.Row.Attributes.Add("ondblclick", "__doPostBack('GridView_LookupFile','Select$" + e.Row.RowIndex + "');");
                }
                e.Row.Cells[3].HorizontalAlign = HorizontalAlign.Right;
                e.Row.Cells[0].Width = Unit.Pixel(1);
            }
        }
    }
}