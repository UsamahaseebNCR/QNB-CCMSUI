using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Reflection;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

using Avanza.CCMS.DAL;
using System.Text;
using Newtonsoft.Json;

namespace Avanza.iSuite.UI
{
    public partial class AddEditRule : BasePage
    {
        private void PopulateControls(int id)
        {
            TransactionRule trxnRule = TransactionRule.LoadTransactionRule("transaction_rule_id = " + id);
            if (trxnRule != null)
            {
                if (trxnRule.TransactionRuleFilteringCriteria != null)
                {
                    string[] parts = trxnRule.TransactionRuleFilteringCriteria.Split(',');
                    foreach (string part in parts)
                    {
                        ListBox_Items.Items.Add(new ListItem(part));
                    }
                }

                TextBox_RuleColor.Text = trxnRule.Color;
                TextBox_RuleName.Text = trxnRule.TransactionRuleFriendlyName;
                CheckBox_Active.Checked = trxnRule.IsActive;
            }

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            int ruleID = 0;
            if (!Page.IsPostBack)
            {
                if (!CheckRight((int)Permissions.AddEditRules))
                    Response.Redirect("AccessDenied.aspx");

                if (Request.QueryString["r"] != null)
                    ViewState["r"] = Request.QueryString["r"];
                ruleID = int.Parse(ViewState["r"].ToString());
                PopulateControls(ruleID);
            }
        }

        protected void Button_Add_Click(object sender, EventArgs e)
        {
            if (TextBox_Criteria.Text.Length > 0)
            {
                if (!ListBox_Items.Items.Contains(new ListItem(TextBox_Criteria.Text)))
                {
                    ListBox_Items.Items.Add(new ListItem(TextBox_Criteria.Text));
                    TextBox_Criteria.Text = "";
                }
            }
        }
        protected void Button_Remove_Click(object sender, EventArgs e)
        {
            if (ListBox_Items.SelectedIndex > -1)
            {
                ListBox_Items.Items.RemoveAt(ListBox_Items.SelectedIndex);
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

                if (ViewState["r"] != null)
                {
                    TransactionRule trxnRule = TransactionRule.LoadTransactionRule("transaction_rule_id = " + ViewState["r"]);
                    TransactionRule oldtrxnRule = TransactionRule.LoadTransactionRule("transaction_rule_id = " + ViewState["r"]);
                    trxnRule.TransactionRuleFriendlyName = TextBox_RuleName.Text;
                    trxnRule.ModificationTime = DateTime.Now;
                    trxnRule.ModifiedBy = ((AppUser)Session[SessionVars.user.ToString()]).UserId;
                    if (ListBox_Items.Items.Count > 0)
                        trxnRule.TransactionRuleFilteringCriteria = GetAllListItem(ListBox_Items);
                    if (TextBox_RuleColor.Text.Length > 0)
                        trxnRule.Color = TextBox_RuleColor.Text;
                    trxnRule.IsActive = CheckBox_Active.Checked;
                    trxnRule.Save();
                    editAuditLog(trxnRule, oldtrxnRule, trx);
                    if (trx != null)
                    {
                        trx.Commit();
                    }
                    Response.Redirect("RulesList.aspx");
                }
                Session[SessionVars.notice.ToString()] = "Rules Added/Updated Successfully!";
            }catch (Exception ex)
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

        public void editAuditLog(TransactionRule objCurrent, TransactionRule objPrevious, SqlTransaction trx)
        {
            string outputCurrent = JsonConvert.SerializeObject(objCurrent);
            string outputPrevious = JsonConvert.SerializeObject(objPrevious);

            if (outputCurrent != outputPrevious)
            {
                Utility.editCaseAuditLogInserts(objCurrent, objPrevious, "Transaction Rule " + objPrevious.TransactionRuleFriendlyName + " modified", (int)((AppUser)Session[SessionVars.user.ToString()]).UserId, (int)Permissions.AddEditRules, trx);
            }

        }

        private string GetAllListItem(ListBox listBoxItems)
        {
            StringBuilder builder = new StringBuilder();
            foreach (ListItem item in listBoxItems.Items)
            {
                builder.Append(item.Text + ",");

            }
            string temp = builder.ToString();
            return temp.Substring(0, temp.Length - 1);
        }

    }
}