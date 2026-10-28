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
    public partial class AddEditATMClassification : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Page.SetFocus(TextBox_title);
            if (Page.IsPostBack)
                return;


            if (Request.QueryString["id"] == null)
            {
                if (!CheckRight((int)Permissions.CreateATMClassification))
                    Response.Redirect("AccessDenied.aspx");
          
            }
            else
            {
                if (!CheckRight((int)Permissions.ModifyATMClassification))
                    Response.Redirect("AccessDenied.aspx");

                Avanza.CCMS.DAL.Classification classification = Avanza.CCMS.DAL.Classification.LoadClassificationByPk(int.Parse(Request.QueryString["id"]));
                TextBox_title.Text = classification.ClassificationName;
             }
        }

        protected void Button_SaveUpdateClassification_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Button_SaveUpdateClassification_Click");
            Avanza.CCMS.DAL.Classification classification;
            Avanza.CCMS.DAL.Classification oldclassification = null;
            bool editMode = false;
            bool isNameContainSpecialChars = CheckSpecialChars(TextBox_title.Text);
            SqlConnection conn = null;
            SqlTransaction trx = null;

            try
            {
                conn = ConnectionFactory.GetNewConnection();
                conn.Open();
                trx = conn.BeginTransaction();

                if (isNameContainSpecialChars == true)
                {
                    Literal_error.Text = "<img src='../images/icon_err.gif'>&nbsp;Name contain invalid character</img> </li>";
                    return;
                }
                else
                {
                    if (Request.QueryString["id"] == null)
                    {
                        classification = new Avanza.CCMS.DAL.Classification();
                        classification.CreationTime = DateTime.Now;
                        classification.CreatedBy = (int)((AppUser)Session[SessionVars.user.ToString()]).UserId;
                    }
                    else
                    {
                        oldclassification = Avanza.CCMS.DAL.Classification.LoadClassificationByPk(int.Parse(Request.QueryString["id"]));
                        classification = Avanza.CCMS.DAL.Classification.LoadClassificationByPk(int.Parse(Request.QueryString["id"]));
                        classification.ModificationTime = DateTime.Now;
                        classification.ModifiedBy = (int)((AppUser)Session[SessionVars.user.ToString()]).UserId;
                        editMode = true;
                    }

                    object res = ConnectionFactory.ExecuteScalar("select top 1 classification_name from classification where classification_name ='" + TextBox_title.Text + "'"
                         + (editMode ? " and classification_id != " + classification.ClassificationId : ""));
                    if (res != null)
                    {
                        Literal_error.Text = "<li>This name is already defined for ATM Classification '" + res + "'. Please, provide different classification name.</li>";
                        return;
                    }

                    classification.ClassificationName = TextBox_title.Text;

                    classification.Save();

                    if (!editMode)
                        Utility.BuildAuditLog("ATM Classification: "+classification.ClassificationName + " created", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.CreateATMClassification);
                    else
                        editAuditLog(classification, oldclassification, trx);
                    //Utility.BuildAuditLog("ATM Classification: " + classification.ClassificationName + " modified", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.ModifyATMClassification);
                    if (trx != null)
                    {
                        trx.Commit();
                    }
                    Response.Redirect("Classification.aspx", false);
                }
                
            }
            catch (Exception ex)
            {               
                Literal_error.Text = "<li>An error occured while saving ATM Classification." + ex.Message + "</li>";
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                if (trx != null)
                {
                    trx.Rollback();
                }
            }
            finally
            {
             
                task.EndTask();
                if (conn != null)
                    conn.Close();
            }
        }
        public void editAuditLog(Avanza.CCMS.DAL.Classification objCurrent, Avanza.CCMS.DAL.Classification objPrevious, SqlTransaction trx)
        {
            string outputCurrent = JsonConvert.SerializeObject(objCurrent);
            string outputPrevious = JsonConvert.SerializeObject(objPrevious);

            if (outputCurrent != outputPrevious)
            {
                Utility.editCaseAuditLogInserts(objCurrent, objPrevious, "ATM Classification " + objPrevious.ClassificationName + " modified", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.AddEditRules, trx);
            }

        }
       
    }
}