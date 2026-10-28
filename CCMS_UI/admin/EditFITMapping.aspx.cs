using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Avanza.iSuite.UI;
using System.Text;
using System.Data.SqlClient;

using System.Data;
using Newtonsoft.Json;
using Avanza.CCMS.DAL;

namespace CCMSUI.admin
{
    public partial class EditFITMapping : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!CheckRight((int)Permissions.EditFITMapping))
                    Response.Redirect("AccessDenied.aspx");

                Page.SetFocus(TextBox_BankName);
                

                Avanza.CCMS.DAL.FITMapping FITMapping = Avanza.CCMS.DAL.FITMapping.LoadFITMapping("PAN_prefix in ('" + Request.QueryString["PAN_prefix"].Split(',')[0] + "')");
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandText = @"select PAN_prefix, bank_name from fitmapping ";
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.SelectCommand = cmd;
                DataTable dtFIT = new DataTable();
                adapter.Fill(dtFIT);

                CheckBoxList_FIT.DataSource = dtFIT;
                CheckBoxList_FIT.DataTextField = "pan_prefix";
                CheckBoxList_FIT.DataBind();

                Avanza.CCMS.DAL.FITMapping.FITMappingReader FITreader = Avanza.CCMS.DAL.FITMapping.ExecuteReader("bank_name!=' '");

                string disabledFIT = "";

                while (FITreader.Read())
                {
                    disabledFIT += FITreader.CurrentFITMapping.PANPrefix + ",";
                }
                foreach (ListItem item in CheckBoxList_FIT.Items)
                {
                    if (Request.QueryString["PAN_prefix"].Contains(item.ToString()))
                        item.Selected = true;
                    else if (disabledFIT.Contains(item.ToString()))
                        item.Enabled = false;
                }
                TextBox_BankName.Text = FITMapping.BankName;
                if (FITMapping.OnUs.HasValue)
                    CheckBox_OnUs.Checked = FITMapping.OnUs.Value;
            }
        }

        protected void Button_Save_Click(object sender, EventArgs e)
        {
            SqlConnection conn = null;
            SqlTransaction trx = null;
            try
            {
                conn = ConnectionFactory.GetNewConnection();
                conn.Open();
                trx = conn.BeginTransaction();

                Avanza.CCMS.DAL.FITMapping oldFITMapping = Avanza.CCMS.DAL.FITMapping.LoadFITMapping("PAN_prefix in ('" + Request.QueryString["PAN_prefix"].Split(',')[0] + "')");
                bool isBankNameContainSpecialChars = CheckSpecialChars(TextBox_BankName.Text);
                if (isBankNameContainSpecialChars == true)
                {
                    Literal_error.Text = "<img src='../images/icon_err.gif'>&nbsp;Bank Name contain invalid character</img> </li>";
                    return;
                }
                else
                {
                    StringBuilder builder = new StringBuilder();
                    string[] fitList;
                    foreach (System.Web.UI.WebControls.ListItem fit in CheckBoxList_FIT.Items)
                    {
                        if (fit.Selected)
                        {
                            fitList = fit.Text.Split(',');
                            foreach (string fitItem in fitList)
                                builder.Append("'" + fitItem + "',");
                        }
                    }
                    if (builder.Length > 0)
                    {
                        builder.Remove(builder.Length - 1, 1);
                        //  SqlCommand cmd = ConnectionFactory.GetNewCommand(true);
                        ConnectionFactory.ExecuteScalar("update fitmapping set on_us = " + (CheckBox_OnUs.Checked ? 1 : 0) + ", bank_name='" + TextBox_BankName.Text + "' where pan_prefix in (" + builder.ToString() + ");");
                        Avanza.CCMS.DAL.FITMapping newFITMapping = Avanza.CCMS.DAL.FITMapping.LoadFITMapping("PAN_prefix in ('" + Request.QueryString["PAN_prefix"].Split(',')[0] + "')");
                        editAuditLog(newFITMapping, oldFITMapping, trx);
                        if (trx != null) { 
                            trx.Commit();
                        }
                        Response.Redirect("FITMapping.aspx");
                        Session[SessionVars.notice.ToString()] = "Daily feed updated Successfully!";
                    }
                    else
                        Literal_error.Text = "<li>Please select FIT from the list</li>";
                }
            }catch(Exception ex)
            {
                if (trx != null)
                {
                    trx.Rollback();
                }
                Session[SessionVars.notice.ToString()] = ex.Message.ToString();
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }

        public void editAuditLog(dynamic objCurrent, dynamic objPrevious, SqlTransaction trx)
        {
            string outputCurrent = JsonConvert.SerializeObject(objCurrent);
            string outputPrevious = JsonConvert.SerializeObject(objPrevious);

            if (outputCurrent != outputPrevious)
            {
                Utility.editCaseAuditLogInserts(objCurrent, objPrevious, "Fit Mapping " + objPrevious.PANPrefix + " modified", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.EditFITMapping, trx);
            }

        }
    }
}