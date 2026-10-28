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
 
namespace CCMSUI.CCMS
{
    public partial class UploadResource : BasePage
    {
        DataSet atmsDS;
        string zipFilePath = null;
        //FileInfo fileToUploadInfo2 = null;
        //FileInfo fileToUploadInfo3 = null;
        //FileInfo fileToUploadInfo4 = null;
        FileInfo fileToUploadInfo = null;

          protected void Page_Load(object sender, EventArgs e)
        {
            Label_Error.Text = "";
            if (!Page.IsPostBack)
            {
                if (!CheckRight((int)Permissions.UploadResource))
                    Response.Redirect("AccessDenied.aspx");
                 //DropDownList_User_Type.DataSource = GetUserTypes();
                //DropDownList_User_Type.DataTextField = "User_Type";
                //DropDownList_User_Type.DataValueField = "User_Type";
                //DropDownList_User_Type.DataBind();

                //DropDownList_User_Type_SelectedIndexChanged(sender, null);

                Package.PackageReader packageReader = Package.ExecuteReader("1=1");
                //DropDownList_Package.Items.Add(new ListItem("Any", "Any"));
                while (packageReader.Read())
                    DropDownList_Package.Items.Add(new ListItem(packageReader.CurrentPackage.PackageTitle, packageReader.CurrentPackage.PackageId.ToString()));
                packageReader.Close();


                //panelSingleFile.Enabled = true;
                 Page.SetFocus(DropDownList_Package);
                TextBox_ExecutionTime.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                TextBox_UploadTime.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
  
                Button_Load_Click(this, null);
            }

            //Label_Error.Text = Directory.Exists (@"C:\Documents and Settings\ikhan\desktop").ToString ();

        }

        //private void MakeSingleFileTreeRoot(string FileName, ref bool CheckFilesExistsInFolder)
        //{
        //    ComponentArt.Web.UI.TreeViewNode rootNode = new ComponentArt.Web.UI.TreeViewNode();
        //    ComponentArt.Web.UI.TreeViewNode Childnode = new ComponentArt.Web.UI.TreeViewNode();

        //    CheckFilesExistsInFolder = true;
        //    rootNode.ID = "root";
        //    rootNode.Text = "Resource File";
        //    rootNode.Expanded = true;
        //    rootNode.ImageUrl = @"images\Tree\files\root.gif";
        //    rootNode.Selectable = false;
        //    TreeView1.Nodes.Add(rootNode);

        //    Childnode.ID = FileName;
        //    Childnode.Text = Path.GetFileName(FileName);
        //    Childnode.Value = Path.GetFileName(FileName) + "|" + new FileInfo(FileName).Length;
        //    rootNode.Nodes.Add(Childnode);
        //}

        //private void BuildTree(ref bool CheckFilesExistsInFolder)
        //{
        //    LogableTask task = LogableTask.NewTask("BuildFileHierarchyTree");
        //    CheckedNodeID.Value = "";
        //    try
        //    {
        //        //if (RadioSingleFile.Checked)
        //            //MakeSingleFileTreeRoot(Session[SessionVars.FileName.ToString()].ToString(), ref CheckFilesExistsInFolder);
        //        //else if (RadioNetworkFile.Checked)
        //        //{
        //        //    if (File.Exists(txtResourcePath.Text)) // means it is a file
        //        //        MakeSingleFileTreeRoot(txtResourcePath.Text, ref CheckFilesExistsInFolder);
        //        //    else
        //        //    {
        //        //        ComponentArt.Web.UI.TreeViewNode rootNode = new ComponentArt.Web.UI.TreeViewNode();
        //        //        rootNode.ID = "root";
        //        //        rootNode.Text = Path.GetFileName(txtResourcePath.Text);
        //        //        rootNode.Expanded = true;
        //        //        rootNode.ImageUrl = @"images\Tree\files\root.gif";
        //        //        rootNode.Selectable = false;
        //        //        TreeView1.Nodes.Add(rootNode);

        //        //        BuildDirectory(txtResourcePath.Text, rootNode, ref CheckFilesExistsInFolder);
        //        //    }
        //        //}
        //    }
        //    catch (Exception exp)
        //    {
        //        task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, exp);
        //        Label_Error.Text = "<li>" + exp.Message + "</li>";
        //    }
        //    finally
        //    {C:\Program Files\NCR APTRA\Advance NDC\CCMSAgent\CCMSAgentSetup-BBQ.msi.zip\install.view360dt
        //        task.EndTask();
        //    }
        //}

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

        //protected void Wizard_ActiveStepChanged(object sender, EventArgs e)
        //{
        //    if (Wizard.ActiveStep.ID == "WizardStep2") // Next Button of First Step of Wizard
        //        if (Label_Error.Text != "") Wizard.ActiveStepIndex = 0;
        //}

        //public bool d()
        //{
        //    return Directory.Exists(txtResourcePath.Text);
        //}

   
      


         protected void Button_Submit_Click(object sender, EventArgs e)
         {
            LogableTask task = LogableTask.NewTask("PrepareUpload");
            try
            {
                 StringBuilder builder = new StringBuilder();
                foreach (ComponentArt.Web.UI.TreeViewNode node in UserATMsTreeView.CheckedNodes)
                    if (node.ID.Substring(0, 1) != "r")
                        builder.Append(int.Parse(node.ID.Substring(1)) + ",");

                //if (builder.Length > 0)
                //    builder.Append("-1");

                //Session["selectedATMs"] = builder.ToString();



                ////    selectedATMs = (string)Session["selectedATMs"];
                //if (builder.Length > 0)
                //{
                //     string[] selectedATMsParts = builder.ToString().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                //    SqlConnection SqlConn = ConnectionFactory.GetNewConnection();
                //    SqlConn.Open();
                //    SqlTransaction SqlTrx = SqlConn.BeginTransaction();


                //    //                    int SeqNo = 1;
                //    int i = 0;
                //    for (i = 0; i < selectedATMsParts.Length; i++)
                //     CreateZipFile(task, fileToUploadInfo.FullName);
                //    File.Delete(fileToUploadInfo.FullName);
                //}




                //Session[SessionVars.FileName.ToString()] = FileUploadCtrl.PostedFile.FileName;
                //ViewState["sourceFilePath"] = TargetFile.FullName;
                //if (Path.GetExtension(TargetFile.FullName).Contains("zip"))
                //{
                //    ViewState["sourceFileSize"] = new FileInfo(TargetFile.FullName).Length;
                //    ViewState["sourceFilePath"] = TargetFile.FullName;
                //}
                //else
                //{
                //    ViewState["sourceFileSize"] = new FileInfo(TargetFile.FullName + ".zip").Length;
                //    ViewState["sourceFilePath"] = TargetFile.FullName+".zip";
                //}

                //StringBuilder builder = new StringBuilder();
                //foreach (ComponentArt.Web.UI.TreeViewNode node in UserATMsTreeView.CheckedNodes)
                //    if (node.ID.Substring(0, 1) != "r")
                //        builder.Append(int.Parse(node.ID.Substring(1)) + ",");

                //if (builder.Length > 0)
                //    builder.Append("-1");

                //Session["selectedATMs"] = builder.ToString();



                //    selectedATMs = (string)Session["selectedATMs"];
                if (builder.Length > 0)
                {
                    string[] selectedATMsParts = builder.ToString().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    SqlConnection SqlConn = ConnectionFactory.GetNewConnection();
                    SqlConn.Open();
                    SqlTransaction SqlTrx = SqlConn.BeginTransaction();


                    //                    int SeqNo = 1;
                    int i = 0;
                    for (i = 0; i < selectedATMsParts.Length; i++)
                     {
                        //if (selectedATMsParts[i] == "-1")
                        //    continue;

                         Upload objUpload = objUpload = new Upload();
                        objUpload.PackageId = int.Parse(DropDownList_Package.SelectedItem.Value);
                        objUpload.RetryRemaining = 10;
                        objUpload.CreationTime = DateTime.Now;
                        objUpload.Status = StatusOptions.scheduled.ToString();
                        objUpload.EndTime = null;
                        objUpload.CreatedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                        objUpload.ExecutionTime = DateTime.ParseExact(TextBox_ExecutionTime.Text, "dd/MM/yyyy HH:mm:ss", null);
                        objUpload.StartTime = objUpload.ExecutionTime;
                        objUpload.AtmId = int.Parse(selectedATMsParts[i]);
                        objUpload.PathAtAtm = @"c:\program files\ncr aptra\advance NDC\CCMSAgent\Packages\";
                        //if (Path.GetExtension(srcFileName).StartsWith("zip"))
                        //    objUpload.SourceFilePath = srcFileName;
                        //else
                        //    
                        //objUpload.SourceFilePath = zipFilePath;
                        //objUpload.FileSize = int.Parse(new FileInfo(objUpload.SourceFilePath).Length.ToString());
                        // Upload objUpload = objUpload = new Upload();
                        //objUpload.Title = txtUploadtitle.Text;
                        //objUpload.RetryRemaining = 10;
                        //objUpload.CreationTime = DateTime.Now;
                        //objUpload.Status = StatusOptions.scheduled.ToString();
                        //objUpload.EndTime = null;
                        //objUpload.CreatedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                        //objUpload.ExecutionTime = DateTime.ParseExact(TextBox_ExecutionTime.Text, "dd/MM/yyyy HH:mm:ss", null);
                        //objUpload.StartTime = objUpload.ExecutionTime;
                        //objUpload.AtmId = int.Parse(selectedATMsParts[i]);
                        //objUpload.PathAtAtm = txtATMPath.Text;
                        //if (Path.GetExtension(srcFileName).StartsWith("zip"))
                        //    objUpload.SourceFilePath = srcFileName;
                        //else
                        //    objUpload.SourceFilePath = srcFileName + ".zip";
                        //objUpload.FileSize = int.Parse(new FileInfo(objUpload.SourceFilePath).Length.ToString()); 

                         objUpload.UploadTime = DateTime.ParseExact(TextBox_UploadTime.Text, "dd/MM/yyyy HH:mm:ss", null);
                        //objUpload.PackageHash = GetHashCode(objUpload.SourceFilePath, new MD5CryptoServiceProvider());
                        try
                        {
                            objUpload.Timeout = int.Parse(TextBox_ExecutionTimeout.Text.Trim());// 10;//seconds
                        }
                        catch { objUpload.Timeout = 10; }
                        objUpload.Save(SqlConn, SqlTrx);
                     }

                    SqlTrx.Commit();
                    //Session.Remove(SessionVars.FileName.ToString());
                    Session[SessionVars.notice.ToString()] = "<li> Schedule for the selected ATM created </li>";
                    Response.Redirect("~/CCMS/UploadStatus.aspx", false);
                }
            }

            catch (Exception ex)
            {
                Label_Error.Text = "<li>" + ex.Message + "</li>";
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

            //            selectedUserType = DropDownList_User_Type.SelectedValue;

            //DropDownList_Manager.Items.Clear();
            //DropDownList_Manager.Items.Add("Select");
            //AppUser.AppUserReader userReader = AppUser.ExecuteReader("user_type = '" + selectedUserType + "' and user_id>2 " + builder.ToString());
            //while (userReader.Read())
            //{
            //    DropDownList_Manager.Items.Add(new ListItem(userReader.CurrentAppUser.UserFullName, userReader.CurrentAppUser.UserId.ToString()));
            //}
            //userReader.Close();


            //CheckBoxList_UserGroups.Items.Clear();
            //CheckBoxList_Organization.Items.Clear();
            // CheckBoxList_CIT.Items.Clear();
            UserATMsTreeView.Nodes.Clear();

            //PopulateGroups(selectedUserType, loggedInUser);
            //            PopulateOrganizations(selectedUserType, loggedInUser);

        }
        //private void PopulateOrganizations(string userType, AppUser loggedInUser)
        //{
        //   // ShowOrganizationForUserType(userType);

        //    if (userType.Equals("CIT"))
        //    {
        //        //LoadCIT(loggedInUser);
        //    }
        //    else
        //    {
        //        LoadOrganization(loggedInUser);
        //    }


        //}
        //private void LoadOrganization(AppUser loggedInUser)
        //{
        //    SqlCommand cmd = null;
        //    try
        //    {
        //        //CheckBoxList_Organization.Items.Clear();

        //        String commandText = "select mcn,region_name from region where is_organization = 1 and parent_region_id is not null";

        //        if (!loggedInUser.UserType.Equals("NCR"))
        //        {
        //            CcmsUserOrganization.CcmsUserOrganizationReader reader = CcmsUserOrganization.ExecuteReader(String.Format("user_id = {0}", loggedInUser.UserId));
        //            if (reader.Read())
        //            {
        //                commandText += String.Format(" and region_id = {0}", reader.CurrentCcmsUserOrganization.OrganizationId);
        //            }
        //            reader.Close();
        //        }

        //        cmd = ConnectionFactory.GetNewCommand(true);
        //        cmd.CommandText = commandText;
        //        SqlDataReader SqlReader = cmd.ExecuteReader();
        //        while (SqlReader.Read())
        //            CheckBoxList_Organization.Items.Add(new ListItem(SqlReader.GetString(0) + "[" + SqlReader.GetString(1) + "]", SqlReader.GetString(0)));
        //        SqlReader.Close();
        //        cmd.Connection.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //    finally
        //    {
        //        if (cmd != null)
        //            if (cmd.Connection != null)
        //                cmd.Connection.Close();
        //    }
        //}





        //private long GetSelectedOrganizationId()
        //{
        //    long selectedOrganizationId = 0;
        //    string organizationMCN = null;

        //    // Add user case
        //    if (Session["u"] != null)
        //    {
        //        AppUser user = AppUser.LoadAppUserByPk(int.Parse(Session["u"].ToString()));

        //        CcmsUserOrganization.CcmsUserOrganizationReader reader = CcmsUserOrganization.ExecuteReader("user_id = " + user.UserId);

        //        if (reader.Read())
        //        {
        //            selectedOrganizationId = reader.CurrentCcmsUserOrganization.OrganizationId;
        //        }

        //        reader.Close();

        //    }
        //    else
        //    {
        //        int selectedOrganiztaionCount = 1;

        //        foreach (System.Web.UI.WebControls.ListItem listItem in CheckBoxList_Organization.Items)
        //        {
        //            if (listItem.Selected)
        //            {
        //                if (selectedOrganiztaionCount > 1)
        //                {
        //                    throw new Exception("Multiple organizations selected, Please select only one organization.");
        //                }
        //                organizationMCN = listItem.Value;
        //                selectedOrganiztaionCount++;
        //            }
        //        }

        //        if (organizationMCN == null)
        //        {
        //            throw new Exception("Please select an organization.");
        //        }

        //        selectedOrganizationId = GetOrganizationIdFromMCN(organizationMCN);
        //    }
        //    return selectedOrganizationId;
        //}

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

            //if (CheckMultipleOrganizationSelection(selectedUserType))
            //{
            //    Literal_error.Text = "<li>Please select only one organization.</li>";
            //    return;
            //}

            LogableTask task = LogableTask.NewTask("LoadTree");

            try
            {
                //if (selectedUserType.Equals("BANK"))
                //{
                //    selectedOrganizationId = GetSelectedOrganizationId();
                //    BuildTreeForBank(selectedOrganizationId);
                //}
                //else
                //{
                //    
                BuildTree();
                //}


                //SelectTreeNodesFromUserState();
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
        protected void BuildTreeForBank(long
            organizationId)
        {
            UserATMsTreeView.Nodes.Clear();


            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            cmd.CommandText = @"select region_name, region_id, parent_region_id, is_organization, mcn from region 
                                  where region_id = " + organizationId + "; " +
                                " select region_name, region_id, parent_region_id, is_organization, mcn from region " +
                                " where parent_region_id = " + organizationId +
                                " select title, atm_id, region_id from atm " +
                                " where region_id in (select region_id from region where parent_region_id = " + organizationId + ");";
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            atmsDS = new DataSet();
            adapter.Fill(atmsDS);

            //Adding Organization Node
            foreach (DataRow dbRow in atmsDS.Tables[0].Rows)
            {
                ComponentArt.Web.UI.TreeViewNode organizationNode = CreateRegionNode(dbRow["region_name"].ToString(), true, (int)dbRow["region_id"], bool.Parse(dbRow["is_organization"].ToString()));
                UserATMsTreeView.Nodes.Add(organizationNode);

                //Adding Regions in Organization and ATMs in Region Node
                foreach (DataRow dbRegionRow in atmsDS.Tables[1].Rows)
                {
                    ComponentArt.Web.UI.TreeViewNode regionNode = CreateRegionNode(dbRegionRow["region_name"].ToString(), true, (int)dbRegionRow["region_id"], bool.Parse(dbRegionRow["is_organization"].ToString()));
                    organizationNode.Nodes.Add(regionNode);

                    foreach (DataRow dbATMRow in atmsDS.Tables[2].Select(" region_id = " + (int)dbRegionRow["region_id"]))
                    {
                        ComponentArt.Web.UI.TreeViewNode atmNode = CreateATMNode(dbATMRow["title"].ToString(), (int)dbATMRow["atm_id"]);
                        regionNode.Nodes.Add(atmNode);
                    }
                }
            }
        }

        private void SelectTreeNodesFromUserState()
        {
            if (Session["u"] != null)
            {
                SqlCommand cmd = null;
                try
                {
                    ArrayList atmsList = new ArrayList();
                    cmd = ConnectionFactory.GetNewCommand(true);
                    cmd.CommandText = "SELECT atm_ID FROM USER_atms WHERE USER_ID = " + Session["u"];
                    SqlDataReader SqlReader = cmd.ExecuteReader();
                    while (SqlReader.Read())
                        atmsList.Add(SqlReader.GetInt32(0));
                    SqlReader.Close();
                    cmd.Connection.Close();

                    if (atmsList.Count > 0)
                        for (int i = 0; i < UserATMsTreeView.Nodes.Count; i++)
                            SetCheck(UserATMsTreeView.Nodes[i], atmsList);
                }
                catch (Exception)
                {
                }
                finally
                {
                    if (cmd != null)
                        if (cmd.Connection != null)
                            cmd.Connection.Close();
                }
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


        protected void BuildTree()
        {
            UserATMsTreeView.Nodes.Clear();

            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            cmd.CommandText = @"select title, atm_id, region_id from atm where is_active = 1 and atm_id in 
            (select atm_id from user_atms where user_id = " + ((AppUser)Session[SessionVars.user.ToString()]).UserId + ") order by title;" +
            "select region_name, region_id, parent_region_id, is_organization, mcn from region where is_active = 1 order by region_name";
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            atmsDS = new DataSet();
            adapter.Fill(atmsDS);
            atmsDS.Relations.Add("ATMRegionRelation", atmsDS.Tables[1].Columns["region_id"], atmsDS.Tables[0].Columns["region_id"]);
            atmsDS.Relations.Add("RegionRegionRelation", atmsDS.Tables[1].Columns["parent_region_id"], atmsDS.Tables[1].Columns["region_id"], false);

            foreach (DataRow dbRow in atmsDS.Tables[1].Rows)
            {
                if (dbRow.IsNull("parent_region_id"))
                {
                    ComponentArt.Web.UI.TreeViewNode newNode = CreateRegionNode(dbRow["region_name"].ToString(), true, (int)dbRow["region_id"], bool.Parse(dbRow["is_organization"].ToString()));
                    UserATMsTreeView.Nodes.Add(newNode);
                    PopulateSubTree(dbRow, newNode, false, false, dbRow["mcn"].ToString());
                }
            }

            foreach (DataRow dbRow in atmsDS.Tables[0].Rows)
            {
                if (dbRow.IsNull("region_id"))
                {
                    ComponentArt.Web.UI.TreeViewNode childNode = CreateATMNode(dbRow["title"].ToString(), (int)dbRow["atm_id"]);
                    childNode.Nodes.Add(childNode);
                }
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