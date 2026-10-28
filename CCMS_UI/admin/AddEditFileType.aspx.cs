using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Avanza.iSuite.UI;

using Avanza.CCMS.DAL;
using System.Reflection;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Data.SqlClient;

namespace CCMSUI.admin
{
    public partial class AddEditFileType : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Page.SetFocus(TextBox_title);
            if (Page.IsPostBack)
                return;


            //if (Request.QueryString["file_type_id"] == null)
            //{
            //    if (!CheckRight((int)Permissions.CreateFileType))
            //        Response.Redirect("AccessDenied.aspx");

            //}
            if (Session["file_type_id"] == null)
            {
                if (!CheckRight((int)Permissions.CreateFileType))
                    Response.Redirect("AccessDenied.aspx");
            }
            else
            {
                if (!CheckRight((int)Permissions.EditFileType))
                    Response.Redirect("AccessDenied.aspx");

                Avanza.CCMS.DAL.FileType fileType = Avanza.CCMS.DAL.FileType.LoadFileTypeByPk(int.Parse(Session["file_type_id"].ToString()));
                TextBox_title.Text = fileType.FileTypeTitle;
                TextBox_path.Text = fileType.PathAtATM;
                DropDownList_CopyType.SelectedValue = fileType.CopyType;
                CheckBox_DoSaveInDatabase.Checked = fileType.IsEJLog;
            }
        }

        protected void Button_SaveUpdateFileType_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Button_SaveUpdateFileType_Click");
            Avanza.CCMS.DAL.FileType fileType;
            Avanza.CCMS.DAL.FileType oldfileType = null;
            SqlConnection conn = null;
            SqlTransaction trx = null;
            bool editMode = false;
            bool result = false;
            try
            {
                conn = ConnectionFactory.GetNewConnection();
                conn.Open();
                trx = conn.BeginTransaction();

                result = ValidateInputs();
                if (result == false)
                {
                    return;
                }
                else
                {
                    if (Session["file_type_id"] == null)
                    {
                        fileType = new Avanza.CCMS.DAL.FileType();
                    }
                    else
                    {
                        fileType = Avanza.CCMS.DAL.FileType.LoadFileTypeByPk(int.Parse(Session["file_type_id"].ToString()));
                        oldfileType = Avanza.CCMS.DAL.FileType.LoadFileTypeByPk(int.Parse(Session["file_type_id"].ToString()));
                        editMode = true;
                    }

                    object res = ConnectionFactory.ExecuteScalar("select top 1 file_type_title from file_type where file_type_title='" + TextBox_title.Text + "'"
                         + (editMode ? " and file_type_id != " + fileType.FileTypeId : ""));
                    if (res != null)
                    {
                        Literal_error.Text = "<li>This name is already defined for file type title '" + res + "'. Please, provide different file type title.</li>";
                        return;
                    }

                    fileType.FileTypeTitle = TextBox_title.Text;
                    fileType.PathAtATM = TextBox_path.Text;
                    fileType.IsEJLog = CheckBox_DoSaveInDatabase.Checked;
                    fileType.CopyType = DropDownList_CopyType.SelectedValue;
                    fileType.Save();

                    if (!editMode)
                        Utility.BuildAuditLog("File: " + fileType.FileTypeTitle + " created", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.CreateFileType);
                    else
                        //Utility.BuildAuditLog(fileType.FileTypeTitle + " modified", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.EditFileType);
                        editAuditLog(fileType, oldfileType, trx);
                    if (trx != null)
                    {
                        trx.Commit();
                    }
                    Response.Redirect("FileType.aspx", false);
                }
            }
            catch (Exception ex)
            {
                if (trx != null)
                {
                    trx.Rollback();
                }
                Literal_error.Text = "<li>An error occured while saving file type." + ex.Message + "</li>";
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);

            }
            finally
            {
                if (conn != null)
                    conn.Close();
                task.EndTask();
            }
        }

        public void editAuditLog(Avanza.CCMS.DAL.FileType objCurrent, Avanza.CCMS.DAL.FileType objPrevious, SqlTransaction trx)
        {
            string outputCurrent = JsonConvert.SerializeObject(objCurrent);
            string outputPrevious = JsonConvert.SerializeObject(objPrevious);

            if (outputCurrent != outputPrevious)
            {
                Utility.editCaseAuditLogInserts(objCurrent, objPrevious, "File: " + objPrevious.FileTypeTitle + " modified", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.EditFileType, trx);
            }

        }

        protected void Button_CancelUpdateFileType_Click(object sender, EventArgs e)
        {
            Session["file_type_id"] = null;
            Response.Redirect("FileType.aspx");
        }

        public bool ValidateInputs()
        {
            bool result = true;
            bool isPathContainSpecialChars = CheckSpecialChars(TextBox_path.Text);
            if (isPathContainSpecialChars == true)
            {
                Literal_error.Text = "<img src='../images/icon_err.gif'>&nbsp;Path has invalid character</img> </li>";
                return result = false;
            }
            bool istitleContainSpecialChars = CheckSpecialChars(TextBox_title.Text);
            if (istitleContainSpecialChars == true)
            {
                Literal_error.Text = "<img src='../images/icon_err.gif'>&nbsp;Title has invalid character</img> </li>";
                return result = false;
            }
            return result;
        }

      
    }
}