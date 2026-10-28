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
using System.Reflection;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Text;
using System.Collections.Generic;
using Avanza.CCMS.DAL;

using ComponentArt.Web.UI;
using Newtonsoft.Json;

namespace Avanza.iSuite.UI
{
    public partial class EditExceptionHandlingCriteria : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            TransactionType.TransactionTypeReader transactionTypeReader = null;
            try
            {
                string exceptionCriteriaID = "";
                if (!Page.IsPostBack)
                {
                    //Populate Transaction Type drop down
                    //********************************************************************************************************************************************************//
                    //transactionTypeReader = TransactionType.ExecuteReader("1=1");
                    //while (transactionTypeReader.Read())
                    //    DropDownList_TrxnType.Items.Add(new ListItem(transactionTypeReader.CurrentTransactionType.TransactionTypeName, transactionTypeReader.CurrentTransactionType.TransactionTypeId.ToString()));
                    //transactionTypeReader.Close();
                    //********************************************************************************************************************************************************//
                    exceptionCriteriaID = Session["exceptionCritID"]!=null?  Session["exceptionCritID"].ToString() : "";
                    //bool injectionCheck = SQLInjectionCheck(Request.QueryString["exceptionCriteria_id"]);
                    //if (injectionCheck)
                    //{
                    //    Literal_error.Text = "<li>Invalid value</li>";
                    //    LogableTask.LogMonoActivityTask("LoadExceptionCriteria", MethodBase.GetCurrentMethod(), TraceLevel.Error, "Invalid value. Exception criteria id issue in url");
                    //    return;
                    //}
                    if (exceptionCriteriaID != null && exceptionCriteriaID != "")
                    {
                        Avanza.CCMS.DAL.ExceptionHandlingCriteria exceptionHandlingCriteria = Avanza.CCMS.DAL.ExceptionHandlingCriteria.LoadExceptionHandlingCriteriaByPk(int.Parse(exceptionCriteriaID));
                        if (exceptionHandlingCriteria != null)
                        {
                            TextBox_Message.Text = exceptionHandlingCriteria.Message;
                            DropDownList_TrxnType.SelectedValue = exceptionHandlingCriteria.TransactionTypeId.ToString();
                            CheckBox_Conditions.Items[0].Selected = exceptionHandlingCriteria.IsPrecondition.Value;
                            CheckBox_Conditions.Items[1].Selected = exceptionHandlingCriteria.IsDisputed.Value;
                            CheckBox_Conditions.Items[2].Selected = exceptionHandlingCriteria.IsSmsTaskRequired.Value;

                        }
                    }


                    //if (Request.QueryString["exceptionCriteria_id"] != null)
                    //    Session["Criteria_id"] = Request.QueryString["exceptionCriteria_id"];
                    //else
                    //    Session["Criteria_id"] = null;


                    //cmd = ConnectionFactory.GetNewCommand(true);
                    //cmd.CommandText = @"select exception_handling_criteria_id,message,is_precondition,is_disputed,is_sms_task_required from exception_handling_criteria" +
                    //            " where exception_handling_criteria_id='" + (string)Session["Criteria_id"] + "'";
                    //reader = cmd.ExecuteReader();
                    //if (reader.Read())
                    //{  
                    //   // Label_Template.Text = reader[2].ToString();// +": ";
                    //    TextBox_Message.Text = reader["message"].ToString();
                    //    DropDownList_TrxnType.SelectedValue = reader["transaction_type_id"].ToString();

                    //    foreach (ListItem items in CheckBox_Conditions.Items)
                    //    {
                    //        items.Selected = (bool)(reader[index]);
                    //        index++;
                    //    }
                    //}
                }
            }
            catch (Exception ex)
            {
                Literal_error.Text = "<li>" + ex.Message + "</li>";
                LogableTask.LogMonoActivityTask("LoadExceptionCriteria", MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            {
                if (transactionTypeReader != null)
                    transactionTypeReader.Close();

            }

        }
        protected void Button_SaveUpdateCriteria_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Button_SaveUpdateCriteria_Click");
            Avanza.CCMS.DAL.ExceptionHandlingCriteria exceptionHandlingCriteria = null;
            Avanza.CCMS.DAL.ExceptionHandlingCriteria oldExceptionHandlingCriteria = null;
            bool editMode = false;
            SqlConnection conn = null;
            //SqlCommand cmd = null;
            SqlTransaction trx = null;
            //AppUser loggedInUser = (AppUser)Session[SessionVars.user.ToString()];

            try
            {
                conn = ConnectionFactory.GetNewConnection();
                conn.Open();
                trx = conn.BeginTransaction();

                //bool injectionCheck = SQLInjectionCheck(Request.QueryString["exceptionCriteria_id"]);
                //if (injectionCheck)
                //{
                //    Literal_error.Text = "<li>Invalid value</li>";
                //    LogableTask.LogMonoActivityTask("SaveExceptionCriteria", MethodBase.GetCurrentMethod(), TraceLevel.Error, "Invalid value. Exception criteria id issue in url");
                //    return;
                //}
                string exceptionCriteriaID = Session["exceptionCritID"] != null ? Session["exceptionCritID"].ToString() : "";
                if (exceptionCriteriaID != null && exceptionCriteriaID != "")
                {
                    exceptionHandlingCriteria = Avanza.CCMS.DAL.ExceptionHandlingCriteria.LoadExceptionHandlingCriteriaByPk(int.Parse(exceptionCriteriaID));
                    oldExceptionHandlingCriteria = Avanza.CCMS.DAL.ExceptionHandlingCriteria.LoadExceptionHandlingCriteriaByPk(int.Parse(exceptionCriteriaID));
                    Session[SessionVars.notice.ToString()] = "Exception Handling Criteria updated successfully.";
                    editMode = true;
                }
                else
                {
                    exceptionHandlingCriteria = new Avanza.CCMS.DAL.ExceptionHandlingCriteria();
                    Session[SessionVars.notice.ToString()] = "Exception Handling Criteria Added successfully.";
                }
                exceptionHandlingCriteria.Message = TextBox_Message.Text;

                exceptionHandlingCriteria.IsPrecondition = CheckBox_Conditions.Items[0].Selected;
                exceptionHandlingCriteria.IsDisputed = CheckBox_Conditions.Items[1].Selected;
                exceptionHandlingCriteria.IsSmsTaskRequired = CheckBox_Conditions.Items[2].Selected;
                exceptionHandlingCriteria.TransactionTypeId = int.Parse(DropDownList_TrxnType.SelectedItem.Value);
                //exceptionHandlingCriteria.
                //foreach (ListItem items in CheckBox_Conditions.Items)
                //{
                //    if (items.Value == "0")
                //        exceptionHandlingCriteria.IsPrecondition = items.Selected;
                //    else if (items.Value == "1")
                //        exceptionHandlingCriteria.IsDisputed = items.Selected;
                //    else if (items.Value == "2")
                //        exceptionHandlingCriteria.IsSmsTaskRequired = items.Selected;
                //}


                exceptionHandlingCriteria.Save();
                if (editMode)
                {
                    editAuditLog(exceptionHandlingCriteria, oldExceptionHandlingCriteria, trx);
                }
                else
                {
                    Utility.BuildAuditLog("Exception Criteria: " + exceptionHandlingCriteria.ExceptionHandlingCriteriaId.ToString() + " created", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.AddException);
                }
                trx.Commit();
                Response.Redirect("ExceptionHandlingCriteria.aspx", false);


            }
            catch (Exception ex)
            {
                if (trx != null)
                    trx.Rollback();

                Literal_error.Text = "<li>An error occured while saving Criteria." + ex.Message + "</li>";
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);

            }
            finally
            {
                task.EndTask();
                if (conn != null)
                    conn.Close();
                //if (cmd != null)
                //    if (cmd.Connection != null)
                //        cmd.Connection.Close();



            }
        }

        public void editAuditLog(Avanza.CCMS.DAL.ExceptionHandlingCriteria objCurrent, Avanza.CCMS.DAL.ExceptionHandlingCriteria objPrevious, SqlTransaction trx)
        {
            string outputCurrent = JsonConvert.SerializeObject(objCurrent);
            string outputPrevious = JsonConvert.SerializeObject(objPrevious);

            if (outputCurrent != outputPrevious)
            {
                Utility.editCaseAuditLogInserts(objCurrent, objPrevious, "Exception Criteria: " + objPrevious.ExceptionHandlingCriteriaId + " modified", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.ModifyException, trx);
            }

        }

        //protected bool SQLInjectionCheck(string QueryString)
        //{
        //    bool check = false;
        //    string[] chars = new string[] { "select", "and", "drop", "create", "update", "delete", "from", "or", ".", "%" };
        //    foreach (string x in chars)
        //    {
        //        if (QueryString.Contains(x))
        //        {
        //            check = true;
        //            break;
        //        }
        //    }
        //    return check;
        //}
    }
}