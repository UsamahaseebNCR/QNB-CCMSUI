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


namespace Avanza.iSuite.UI
{
    public partial class SmsConfiguration : BasePage
    {
        string query;
        string orderby;


        protected void GridView_SMSTemplate_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.FindControl("Button_deleteConfiguration") != null)
                ((ImageButton)e.Row.FindControl("Button_deleteConfiguration")).Attributes.Add("onClick", "return confirm('Are you sure you want to delete SMS Configuration for template ID:" + ((DataRowView)e.Row.DataItem).Row["tibco_template_id"] + "?','CCMS')");
        }


        protected void Page_Load(object sender, EventArgs e)
        {

            if (!Page.IsPostBack)
            {
                if (!CheckRight((int)Permissions.ViewSmsConfiguration))
                    Response.Redirect("AccessDenied.aspx");

                //if (!CheckRight((int)Permissions.ModifyAlertTemplate))
                //  GridView_AlertTemplate.Columns[2].Visible = false;
                DropDownList_template.Items.Add(new ListItem("Any", "Any"));

                DropDownList_template.AppendDataBoundItems = true;

                PopulateDropdownList("select ccms_sms_template_id,ccms_sms_template_name from ccms_sms_template", DropDownList_template, "ccms_sms_template_name", "ccms_sms_template_id");

                ViewState["sortDirection"] = " asc";
                ViewState["SortExpression"] = " tt.transaction_type_name";
                orderby = ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString();
                //                query = @"select sms_template_configuration_id,status,c.ccms_sms_template_name,s.alert_type_id
                //                                    ,(case s.channel_id when 0 then 'SMS' when 1 then 'Email' else 'Both' end) as Channel
                //                                    ,(case  when s.alert_type_id is null  then 'Customers' else 'Staff' end) as AlertType
                //                                    ,tt.transaction_type_name,st.action_code
                //                                    from sms_template_configuration s inner join sms_transaction_type_detail st
                //                                             on st.sms_transaction_type_detail_id = s.sms_transaction_type_detail_id
                //                                             inner join transaction_type tt on tt.transaction_type_id = st.transaction_type_id
                //                                    inner join ccms_sms_template c on c.ccms_sms_template_id = s.template_id 
                //                                    order by "+orderby+" ";
                BindGrid("", orderby);
            }

        }


        private void BindGrid(string Query, string orderby)
        {
            LogableTask task = LogableTask.NewTask("BindGrid");
            try
            {
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                query = @"select sms_template_configuration_id,status,c.ccms_sms_template_name,s.alert_type_id
                                    ,(case s.channel_id when 0 then 'SMS' when 1 then 'Email' else 'Both' end) as Channel
                                    ,(case  when s.alert_type_id is null  then 'Customer' else 'Staff' end) as AlertType
                                    ,tt.transaction_type_name,st.action_code,c.tibco_template_id,alert_type_name
                                    from sms_template_configuration s left outer join sms_transaction_type_detail st
                                             on st.sms_transaction_type_detail_id = s.sms_transaction_type_detail_id
                                             left outer join transaction_type tt on tt.transaction_type_id = st.transaction_type_id
                                    left outer join ccms_sms_template c on c.ccms_sms_template_id = s.template_id 
                                    left join alert_type on alert_type.alert_type_id = s.alert_type_id
                                    
                                    " + Clause() + " order by " + orderby + "";

                cmd.CommandText = query;

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dtAlertTemplate = new DataTable();
                adapter.Fill(dtAlertTemplate);
                cmd.Connection.Close();
                this.GridView_SMSTemplate.DataSource = dtAlertTemplate; ;
                this.GridView_SMSTemplate.DataBind();
                Literal_count.Text = dtAlertTemplate.Rows.Count + " records.";
            }
            catch (Exception ex)
            { task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            Literal_error.Text = ex.Message;
            }
            finally
            { task.EndTask(); }
        }

        protected void GridView_AlertTemplate_PageIndexChanged(object sender, EventArgs e)
        {

        }

        protected void GridView_AlertTemplate_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView_SMSTemplate.PageIndex = e.NewPageIndex;
            BindGrid(query, ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }

        protected void GridView_SMSTemplate_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "deleteSMSConfiguration")
            {
                ConnectionFactory.ExecuteScalar("delete sms_template_configuration where sms_template_configuration_id = " + e.CommandArgument);
                Literal_ok.Text = "Selected Template configuration deleted!";
                BindGrid("", "tt.transaction_type_name");

            }
        }

        protected void Button_reset_Click(object sender, EventArgs e)
        {

        }

        protected void Button_search_Click(object sender, EventArgs e)
        {

            BindGrid("", ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }
        string Clause()
        {
            string clause;
            bool isCheck = false;
            if (DropDownList_template.Text == "Any" && DropDownList_status.Text == "Select" && DropDownList_channel.Text == "Any")
            {
                clause = "";
            }
            else
            {
                clause = " where ";
                if (DropDownList_template.Text != "Any")
                {
                    clause += " c.ccms_sms_template_id = '" + DropDownList_template.SelectedValue + "'";
                    isCheck = true;
                }
                if (DropDownList_status.Text != "Select")
                {
                    if (isCheck == true)
                        clause += " and ";

                    clause += " status =  '" + DropDownList_status.SelectedValue + "'";
                    isCheck = true;
                }
                if (DropDownList_channel.Text != "Any")
                {
                    if (isCheck == true)
                        clause += " and ";

                    clause += " s.channel_id =  '" + DropDownList_channel.SelectedValue + "'";

                }


            }
            //            query = @"select sms_template_configuration_id,status,c.ccms_sms_template_name,s.alert_type_id
            //                                    ,(case s.channel_id when 0 then 'SMS' when 1 then 'Email' else 'Both' end) as Channel
            //                                    ,(case  when s.alert_type_id is null  then 'Customers' else 'Staff' end) as AlertType
            //                                    ,tt.transaction_type_name,st.action_code
            //                                    from sms_template_configuration s inner join sms_transaction_type_detail st
            //                                             on st.sms_transaction_type_detail_id = s.sms_transaction_type_detail_id
            //                                             inner join transaction_type tt on tt.transaction_type_id = st.transaction_type_id
            //                                    inner join ccms_sms_template c on c.ccms_sms_template_id = s.template_id 
            //                                     " + clause + "";

            return clause;
        }
        //string Filter()
        //{
        //    string filter = "";

        //    if (DropDownList_creator.SelectedIndex > 0)
        //        filter += " and created_by = " + DropDownList_creator.SelectedValue;
        //    if (DropDownList_ATMType.SelectedIndex > 0)
        //        filter += " and atm_type = '" + DropDownList_ATMType.SelectedValue + "'";

        //    if (DropDownList_status.SelectedValue == "1")
        //        filter += " and is_active =1 and atm_id in ("
        //                + Session[SessionVars.SelectedATMs.ToString()] + ")";
        //    else if (DropDownList_status.SelectedValue == "0")
        //    {
        //        filter += " and is_active = 0 and atm_id in ("
        //            + Session[SessionVars.InactiveATMs.ToString()] + ")";
        //        //and atm_id in (select atm_id from user_ATMS where user_id=" + ((AppUser)Session[SessionVars.user.ToString()]).UserId + ")";
        //    }
        //    else
        //    {
        //        filter += " and atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + "," + Session[SessionVars.InactiveATMs.ToString()] + ")";
        //    }

        //    if (Request.QueryString["i"] != null)
        //        filter += " and atm_id in (select distinct(atm_id) from atm_general_inventory) ";

        //    return filter;
        //}
        private void PopulateDropdownList(string query, DropDownList ddl, string textField, string valueField)
        {
            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);

            cmd.CommandText = query;

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dtAlertType = new DataTable();

            adapter.Fill(dtAlertType);

            ddl.DataSource = dtAlertType;
            ddl.DataTextField = textField;
            ddl.DataValueField = valueField;
            ddl.DataBind();
        }

        protected void GridView_SMSTemplate_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (ViewState["SortExpression"].ToString() == e.SortExpression)
            {
                if (ViewState["sortDirection"].ToString() == " asc")
                    ViewState["sortDirection"] = " desc";
                else
                    ViewState["sortDirection"] = " asc";
            }
            else
            {
                ViewState["SortExpression"] = e.SortExpression;
                ViewState["sortDirection"] = " asc";
            }
            BindGrid("", ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }

    }
}