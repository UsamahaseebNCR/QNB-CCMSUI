using Avanza.CCMS.DAL;

using Avanza.iSuite.UI;
using ComponentArt.Web.UI;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web.UI;
using System.Security.Cryptography;
using System.Web.UI.WebControls;
using System.Web.Script.Services;
using System.Web.Services;
using Avanza.iSuite.DAL;
using System.Linq;
using System.Collections.Generic;
using System.IO.Compression;
using System.Web.Configuration;
//using Avanza.iSuite.DAL;

namespace CCMSUI.CCMS
{
    public partial class AddEditPackage : BasePage
    {
        DataSet atmsDS;
        string zipFilePath = null;
        FileInfo fileToUploadInfo = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            //Edited By Muhammad Shaharyar added Extract and Replace
            int counter = 0;
            Label_Error.Text = "";
            ChunkSize.Attributes["MaxChunkLimit"] = WebConfigurationManager.AppSettings["PackageMngtMaxChunkLimit"] != null ? WebConfigurationManager.AppSettings["PackageMngtMaxChunkLimit"] : "30"; 
            if (!Page.IsPostBack)
            {
                if (!CheckRight((int)Permissions.UploadResource))
                    Response.Redirect("AccessDenied.aspx");


                if (Request.QueryString["id"] != null)
                {
                    Button_Submit.Enabled = false;
                    Avanza.CCMS.DAL.Package package = Avanza.CCMS.DAL.Package.LoadPackageByPk(int.Parse(Request.QueryString["id"]));
                    TextBox_PackageTitle.Text = package.PackageTitle;

                    PackageInstallationDetail.PackageInstallationDetailReader reader = PackageInstallationDetail.ExecuteReader("package_id=" + package.PackageId);
                    while (reader.Read())
                    {
                        counter++;

                        if (counter == 1)
                        {
                            TextBox_InstallCommand1.Text = reader.CurrentPackageInstallationDetail.ProcessInstallCommand;
                            TextBox_InstallCommandConfirmationLogFilePath1.Text = reader.CurrentPackageInstallationDetail.ProcessInstallLogfile;
                            TextBox_KeywordToSearchForSuccessfulInstallation1.Text = reader.CurrentPackageInstallationDetail.ProcessInstallKeyword;
                            TextBox_UninstallCommand1.Text = reader.CurrentPackageInstallationDetail.ProcessUninstallCommand;
                            TextBox_UninstallCommandConfirmationLogFilePath1.Text = reader.CurrentPackageInstallationDetail.ProcessUninstallLogfile;
                            TextBox_KeywordToSearchForUnsuccessfulInstallation1.Text = reader.CurrentPackageInstallationDetail.ProcessUninstallKeyword;
                            Checkbox_RequiresRestartBeforeExecuting.Checked = reader.CurrentPackageInstallationDetail.ProcessRequiresRestart.Value;
                            ExtractReplacePath.Text = reader.CurrentPackageInstallationDetail.ExtractReplacePath;
                            if(reader.CurrentPackageInstallationDetail.ExtractReplacePath=="")
                            {
                                SelectionProcess1.Checked = true;
                                Table2.Visible = true;
                                Table1.Visible = false;
                            }
                            else
                            {
                                SelectionProcess2.Checked = true;
                                Table2.Visible = false;
                                Table1.Visible = true;
                            }

                        }
                        SelectionProcess1.Enabled = false;
                        SelectionProcess2.Enabled = false;
                        
                    }
                    reader.Close();

                }
                else if (Request.QueryString["PckgName"] != null && Request.QueryString["FileName"] != null)
                {
                    TextBox_PackageTitle.Text = Request.QueryString["PckgName"];
                    TextBox_PackageTitle.Enabled = false;
                    fileLabel.Text = "Kindly upload file '" + Request.QueryString["FileName"] + "' to resume..";
                    fileLabel.CssClass = Request.QueryString["FileName"];
                    fileLabel.Attributes["style"] = "color: royalblue;font-weight: bold;}";
                    fileLabel.Attributes["FileSize"] = Request.QueryString["fileSize"] != null ? Request.QueryString["fileSize"] : "";

                    if (Request.QueryString["Conf"] != null)
                    {
                        SelectionProcess1.Enabled = false;
                        SelectionProcess2.Enabled = false;

                        if (Request.QueryString["Conf"] == "extract")
                            SelectionProcess2.Checked = true;
                        else
                            SelectionProcess1.Checked = true;
                    }
                    if (Request.QueryString["chunkSize"] != null)
                    {
                        ChunkSize.Text = Request.QueryString["chunkSize"];
                        ChunkSize.Enabled = false;
                    }
                }
                else
                {
                        hdnRadioFirstTime.Value = "true";
                }

                fileLabel.Attributes["CreatedBy"] = ((AppUser)Session[SessionVars.user.ToString()]).UserId.ToString();
                Page.SetFocus(TextBox_PackageTitle);
                
            }
            else
            {
                hdnRadioFirstTime.Value = "false";
            }

        }


        private void BuildDirectory(string dirPath, ComponentArt.Web.UI.TreeViewNode parentNode, ref bool CheckFilesExistsInFolder)
        {
            string[] subDirectories = Directory.GetDirectories(dirPath);
            string[] files = Directory.GetFiles(dirPath);

            foreach (string directory in subDirectories)
            {
                ComponentArt.Web.UI.TreeViewNode node = new ComponentArt.Web.UI.TreeViewNode();
                node.ID = directory;
                node.Text = Path.GetFileName(directory);
                parentNode.Nodes.Add(node);
                node.Selectable = false;
                node.Expanded = true;
                node.ImageUrl = "images/Tree/files/folder.gif";
                node.ExpandedImageUrl = "images/Tree/files/folder_open.gif";
                BuildDirectory(directory, node, ref CheckFilesExistsInFolder);
            }
            foreach (string file in files)
            {
                CheckFilesExistsInFolder = true;
                ComponentArt.Web.UI.TreeViewNode node = new ComponentArt.Web.UI.TreeViewNode();
                node.ID = file;
                node.Text = Path.GetFileName(file);
                FileInfo fi = new FileInfo(file);
                if (File.Exists(Server.MapPath(@"~/images\Tree\files\" + fi.Extension.Substring(1) + ".gif")))
                    node.ImageUrl = @"images\Tree\files\" + fi.Extension.Substring(1) + ".gif";
                //node.Value = Path.GetFileName(txtResourcePath.Text) + fi.FullName.Replace(txtResourcePath.Text, "") + "|overwrite";
                parentNode.Nodes.Add(node);
            }
        }


        string GetInstallerContent()
        {
            StringBuilder builder = new StringBuilder();
            string content = "ID:{0}|PrcoessName:{1}|Install:{2}|Confirm:{3}|KeywordToCheck:{4}|Uninstall:{5}|Confirm:{6}|KeywordToCheckUnistallation:{7}|restart:{8}|overwrite-old-files:{9}|overwrite-latest-version:{10}|backup:{11}|restartAfterExecuting:{12}{13};";

            if (TextBox_InstallCommand1.Text.Length > 0 || ExtractReplacePath.Text.Length > 0)
                builder.Append(string.Format(content, 1, TextBox_InstallCommand1.Text,
                                                    TextBox_InstallCommand1.Text,
                                                    TextBox_InstallCommandConfirmationLogFilePath1.Text,
                                                    TextBox_KeywordToSearchForSuccessfulInstallation1.Text,
                                                    TextBox_UninstallCommand1.Text,
                                                    TextBox_UninstallCommandConfirmationLogFilePath1.Text,
                                                    TextBox_KeywordToSearchForUnsuccessfulInstallation1.Text,
                                                    Checkbox_RequiresRestartBeforeExecuting.Checked ? "Y" : "N", "N", "N", "N", 
                                                    Checkbox_RequiresRestartAfterExecuting.Checked ? "Y" : "N", 
                                                    SelectionProcess2.Checked ? string.Format("|ExtractAndReplace:{0}", ExtractReplacePath.Text) : ""));

            
            return builder.ToString();
        }

        void AddFileToZipFile(ZipOutputStream stream, string file)
        {
            ZipEntry entry;
            entry = new ZipEntry(Path.GetFileName(file));
            byte[] filebytes = File.ReadAllBytes(file);
            entry.Size = filebytes.LongLength;
            stream.PutNextEntry(entry);
            stream.Write(filebytes, 0, filebytes.Length);
            stream.CloseEntry();
        }

        void AddFileToZipFile(ZipArchive archive, string file)
        {
            ZipArchiveEntry entry = archive.CreateEntry(Path.GetFileName(file));
            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
            using (Stream entryStream = entry.Open())
            {
                byte[] buffer = new byte[4096]; // 4KB buffer size
                int bytesRead;
                while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) > 0)
                {
                    entryStream.Write(buffer, 0, bytesRead);
                }
            }
        }

        byte[] ReadFileInChunks(string filePath)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (MemoryStream ms = new MemoryStream())
            {
                byte[] buffer = new byte[4096]; // 4KB buffer size
                int bytesRead;
                while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, bytesRead);
                }
                return ms.ToArray();
            }
        }

        void CreateZipFile(LogableTask task)
        {
            FileStream fileStream = null;
            try
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Verbose, "going to create Zipped file");
                zipFilePath = Path.GetDirectoryName(fileToUploadInfo.FullName) + "\\" + TextBox_PackageTitle.Text + ".zip";
                fileStream = new FileStream(zipFilePath, FileMode.Create, FileAccess.Write);

                using (ZipArchive archive = new ZipArchive(fileStream, ZipArchiveMode.Create))
                {
                    AddFileToZipFile(archive, fileToUploadInfo.FullName);

                    // Add Helper file
                    string helperFileName = Path.GetDirectoryName(fileToUploadInfo.FullName) + "\\install.view360dt";
                    File.WriteAllText(helperFileName, GetInstallerContent());
                    AddFileToZipFile(archive, helperFileName);
                    File.Delete(helperFileName);
                }

                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Verbose, "Zipped Created Successfully.");
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Zipped Created Successfully.");

                //return File.ReadAllBytes(zipFilePath);
            }
            finally
            {
                fileStream?.Close();
            }
        }

        //byte[] CreateZipFile(LogableTask task)
        //{
        //    FileStream fileStream = null;
        //    try
        //    {
        //        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Verbose, "going to create Zipped file");
        //        zipFilePath = Path.GetDirectoryName(fileToUploadInfo.FullName) + "\\" + TextBox_PackageTitle.Text + ".zip";
        //        fileStream = new FileStream(zipFilePath, FileMode.Create, FileAccess.Write);
        //        ZipOutputStream stream = new ZipOutputStream(fileStream);
        //        AddFileToZipFile(stream, fileToUploadInfo.FullName);


        //        //Add Helper file.d 4ew
        //        string helperFileName = Path.GetDirectoryName(fileToUploadInfo.FullName) + "\\install.view360dt";
        //        File.WriteAllText(helperFileName, GetInstallerContent());
        //        AddFileToZipFile(stream, helperFileName);
        //        File.Delete(helperFileName);
        //        //length += filebytes.Length;
        //        stream.Close();
        //        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Verbose, "Zipped Created Successufully.");
        //        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "Zipped Created Successufully.");
        //        byte[] bytes = System.IO.File.ReadAllBytes(zipFilePath);
        //        //bool t = ByteArrayToFile(zipFilePath, bytes);
        //        return bytes;

        //    }
        //    finally
        //    {
        //        if (fileStream != null)
        //            fileStream.Close();
        //    }
        //}

        private void SavePackageDetail(string extractreplacepath,string installCmd, string InstallLogFile, string InstallKeyword, string uninstallCmd, string UninstallLogFile, string UninstallKeyword, bool requiresRestart, int packageID, SqlTransaction trxn)
        {
            PackageInstallationDetail installationDetail1 = new PackageInstallationDetail();
            installationDetail1.PackageId = packageID;
            installationDetail1.ExtractReplacePath = extractreplacepath;
            installationDetail1.ProcessInstallCommand = installCmd;
            installationDetail1.ProcessInstallLogfile = InstallLogFile;
            installationDetail1.ProcessInstallKeyword = InstallKeyword;
            installationDetail1.ProcessUninstallCommand = uninstallCmd;
            installationDetail1.ProcessUninstallLogfile = UninstallLogFile;
            installationDetail1.ProcessUninstallKeyword = UninstallKeyword;
            installationDetail1.ProcessRequiresRestart = requiresRestart;
            installationDetail1.Save(trxn.Connection, trxn);
        }

        private static void MergeFiles(string file1, string file2)
        {
            FileStream fs1 = null;
            FileStream fs2 = null;
            try
            {
                fs1 = System.IO.File.Open(file1, FileMode.Append);
                fs2 = System.IO.File.Open(file2, FileMode.Open);
                byte[] fs2Content = new byte[fs2.Length];
                fs2.Read(fs2Content, 0, (int)fs2.Length);
                fs1.Write(fs2Content, 0, (int)fs2.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " : " + ex.StackTrace);
            }
            finally
            {
                if (fs1 != null) fs1.Close();
                if (fs2 != null) fs2.Close();
                System.IO.File.Delete(file2);
            }
        }

        static int GetChunkIndex(string filePath)
        {
            // Extract the chunk index from the file name
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string[] parts = fileName.Split('_');
            if (parts.Length >= 3)
            {
                if (int.TryParse(parts[3], out int index))
                {
                    return index;
                }
            }
            // Return a default index if extraction fails
            return int.MaxValue;
        }

        protected void Button_Submit_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("PrepareUpload");
            SqlTransaction SqlTrx = null;
            try
            {
                Label myLabel = (Label)FindControl("lblMessage");
                myLabel.Text = "The file's chunk/s merging in process...";
                AppSetting appSetting = (AppSetting)Application[ApplicationVars.AppSettings.ToString()];
                string fileToUploadTempFolder = appSetting.TemporaryFolder + "\\PendingUploads\\" + TextBox_PackageTitle.Text + "_" + DateTime.Now.ToString("ddMMyyy");

                List<string> allChunks = Directory.GetFiles(fileToUploadTempFolder).OrderBy(GetChunkIndex).ToList();
                string firstChunkFile = allChunks.FirstOrDefault();
                allChunks.Remove(firstChunkFile);

                foreach (string filePath in allChunks)
                {
                    MergeFiles(firstChunkFile, filePath);
                }

                FileInfo fileInfo = new FileInfo(firstChunkFile);
                fileInfo.MoveTo(fileToUploadTempFolder + "\\" + TextBox_FileName.Text);
                fileToUploadInfo = new FileInfo(fileToUploadTempFolder + "\\" + Path.GetFileName(TextBox_FileName.Text));                
                CreateZipFile(task);
                
                SqlConnection SqlConn = ConnectionFactory.GetNewConnection();
                SqlConn.Open();
                SqlTrx = SqlConn.BeginTransaction();


                Package package = Package.LoadPackage(" progress = 'pending' and package_title = '" + TextBox_PackageTitle.Text + "'");
                package.PackageTitle = TextBox_PackageTitle.Text;
                package.PackagePhyscialPath = zipFilePath;
                package.PackageSize = new FileInfo(package.PackagePhyscialPath).Length;
                package.UploadedBytes = new FileInfo(package.PackagePhyscialPath).Length;
                package.PackageHash = GetHashCode(package.PackagePhyscialPath, new MD5CryptoServiceProvider());
                package.Progress = "completed";
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "zipFilePath logging : " + zipFilePath + " , Package Hash : " + package.PackageHash);                
                package.Save(SqlTrx.Connection, SqlTrx);

                File.Delete(fileToUploadInfo.FullName);

                Utility.BuildAuditLog("Package: "+package.PackageTitle +" Created", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.CreatePackage);
                if (System.Configuration.ConfigurationManager.AppSettings["DBFileUploadCheck"] != null && System.Configuration.ConfigurationManager.AppSettings["DBFileUploadCheck"] == "1")
                {
                    byte[] bytes = ReadFileInChunks(zipFilePath);
                    Directory.Delete(fileToUploadTempFolder,true);
                
                    SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                    cmd.Connection.Open();
                    cmd.Connection.ChangeDatabase(System.Configuration.ConfigurationManager.AppSettings["ViewDataPackagesDB"]);

                    cmd.CommandText = "insert into FileUpload ([PackageID], [FileBytes]) values(@packageID,@fileBytes)";
                    cmd.Parameters.AddWithValue("@packageID", package.PackageId);
                    cmd.Parameters.AddWithValue("@fileBytes", bytes);
                    cmd.ExecuteNonQuery();
                    cmd.Connection.Close();
                }

                if (TextBox_InstallCommand1.Text.Length > 0 || ExtractReplacePath.Text.Length > 0)
                    SavePackageDetail(ExtractReplacePath.Text, TextBox_InstallCommand1.Text, TextBox_InstallCommandConfirmationLogFilePath1.Text, TextBox_KeywordToSearchForSuccessfulInstallation1.Text, TextBox_UninstallCommand1.Text,
                   TextBox_UninstallCommandConfirmationLogFilePath1.Text, TextBox_KeywordToSearchForUnsuccessfulInstallation1.Text, Checkbox_RequiresRestartBeforeExecuting.Checked, package.PackageId, SqlTrx);


                SqlTrx.Commit();
                //    //Session.Remove(SessionVars.FileName.ToString());
                //    Session[SessionVars.notice.ToString()] = "<li> Schedule for the selected ATM created </li>";
                Response.Redirect("~/admin/Package.aspx");
                //}
            }

            catch (Exception ex)
            {
                Label_Error.Text = "<li>" + ex.Message + "</li>";

                if (SqlTrx?.Connection?.State == ConnectionState.Open)
                    SqlTrx.Rollback();
            }
            finally
            {
                try
                {
                    task.EndTask();
                }
                catch (Exception)
                {
                }
            }
        }


        protected ComponentArt.Web.UI.TreeViewNode CreateRegionNode(string text, bool expanded, int id, bool isOrganization)
        {
            ComponentArt.Web.UI.TreeViewNode node = new ComponentArt.Web.UI.TreeViewNode();

            node.Text = text;
            node.ID = "r" + id;
            if (isOrganization)
                node.ImageUrl = "icon_organization.png";
            else
                node.ImageUrl = "icon_ATMRegionList.gif";
            node.DroppingEnabled = false;
            node.Expanded = expanded;
            node.ShowCheckBox = true;

            return node;
        }

        protected ComponentArt.Web.UI.TreeViewNode CreateATMNode(string text, int id)
        {
            ComponentArt.Web.UI.TreeViewNode node = new ComponentArt.Web.UI.TreeViewNode();

            node.Text = text;
            node.ID = "a" + id;
            node.ImageUrl = "icon_atm.jpg";
            node.DroppingEnabled = false;
            node.ShowCheckBox = true;

            return node;
        }

        protected string TraceOrganizationMCN(int region_id)
        {
            if (Region.LoadRegionByPk(region_id).IsOrganization == true)
                return Region.LoadRegionByPk(region_id).MCN;
            else
                return TraceOrganizationMCN(Region.LoadRegionByPk(region_id).ParentRegionId.Value);
        }
        protected void DropDownList_User_Type_SelectedIndexChanged(object sender, EventArgs e)
        {
            StringBuilder builder = new StringBuilder();
            string selectedUserType = String.Empty;
            int userId = 0;

            AppUser loggedInUser = (AppUser)Session[SessionVars.user.ToString()];

            if (loggedInUser == null)
                return;



            if (!IsPostBack)
            {
                if (Session["u"] != null)
                {
                    userId = int.Parse(Session["u"].ToString());
                    AppUser user = AppUser.LoadAppUserByPk(userId);
                    // DropDownList_User_Type.SelectedValue = user.UserType;
                    builder.Append(" and user_id <> " + user.UserId);

                }
            }

            

        }

        private long GetOrganizationIdFromMCN(string organizationMCN)
        {
            long organizationId = 0;
            SqlCommand cmd = null;
            try
            {
                cmd = ConnectionFactory.GetNewCommand(true);
                cmd.CommandText = "select region_id from region where mcn = '" + organizationMCN + "'";
                object id = cmd.ExecuteScalar();
                if (id != null)
                {
                    organizationId = long.Parse(id.ToString());
                }

            }
            catch (Exception ex)
            {
                LogableTask.LogMonoActivityTask("GetOrganizationIdFromMCN", MethodBase.GetCurrentMethod(), TraceLevel.Info, ex);
            }
            finally
            {
                if (cmd != null)
                    if (cmd.Connection != null)
                        cmd.Connection.Close();
            }

            return organizationId;

        }
        protected void Button_Load_Click(object sender, EventArgs e)
        {

            LogableTask task = LogableTask.NewTask("LoadTree");

            try
            {

            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_error.Text = "<li>Error while loading tree. [" + ex.Message + "]</li>";
            }
            finally
            {
                task.EndTask();
            }
        }


        protected void SetCheck(TreeViewNode node, ArrayList selectedItem)
        {
            TreeViewNodeCollection nodeCol = node.Nodes;
            int atms = 0;

            for (int counter = 0; counter < nodeCol.Count; counter++)
            {
                if (nodeCol[counter].ID.IndexOf('r') < 0)
                {
                    if (selectedItem.IndexOf(int.Parse(nodeCol[counter].ID.Substring(1))) >= 0)
                    {
                        nodeCol[counter].Checked = true;
                        atms++;
                    }
                }
                else if (nodeCol[counter].Nodes.Count > 0)
                {
                    SetCheck(nodeCol[counter], selectedItem);

                    if (nodeCol[counter].Checked)
                        atms++;
                }
                else
                {
                    nodeCol[counter].Checked = true;
                    atms++;
                }
            }

            if (atms == nodeCol.Count && atms != 0)
                nodeCol[0].ParentNode.Checked = true;
        }


        protected void PopulateSubTree(DataRow parentRegionRow, ComponentArt.Web.UI.TreeViewNode node, bool allowEditingRegion, bool allowEditingATM, string parentMCN)
        {

            foreach (DataRow childRow in atmsDS.Tables[1].Select("parent_region_id=" + parentRegionRow["region_id"]))
            {

                ComponentArt.Web.UI.TreeViewNode childNode = CreateRegionNode(childRow["region_name"].ToString(), false, (int)childRow["region_id"], bool.Parse(childRow["is_organization"].ToString()));
                node.Nodes.Add(childNode);
                PopulateSubTree(childRow, childNode, false, false, parentMCN);
            }

            foreach (DataRow childRow in parentRegionRow.GetChildRows("ATMRegionRelation"))
            {
                ComponentArt.Web.UI.TreeViewNode childNode = CreateATMNode(childRow["title"].ToString(), (int)childRow["atm_id"]);
                node.Nodes.Add(childNode);
            }
        }



        internal static string GetHashCode(string filePath, HashAlgorithm cryptoService)
        {
            // create or use the instance of the crypto service provider
            // this can be either MD5, SHA1, SHA256, SHA384 or SHA512
            using (cryptoService)
            {
                using (var fileStream = new FileStream(filePath,
                                                       FileMode.Open,
                                                       FileAccess.Read,
                                                       FileShare.ReadWrite))
                {
                    var hash = cryptoService.ComputeHash(fileStream);
                    var hashString = Convert.ToBase64String(hash);
                    return hashString.TrimEnd('=');
                }
            }
        }


        


    }
}