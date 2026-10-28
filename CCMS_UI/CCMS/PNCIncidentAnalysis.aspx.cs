using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Avanza.iSuite.UI;
using System.Data;
using System.Data.SqlClient;

using Avanza.CCMS.DAL;
using System.Reflection;
using System.Diagnostics;
using System.Text;

namespace CCMSUI.CCMS
{
    public partial class PNCIncidentAnalysis : BasePage
    {
        string GetFilter()
        {
            string filter = " and status = 'Failed'";

            filter += " and atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ")";


            //if (TextBox_FromDate.Text.Length > 0)
            //    filter += " and trxn_datetime >= convert(datetime,'" + TextBox_FromDate.Text + "',103)";

            //if (TextBox_ToDate.Text.Length > 0)
            //    filter += " and trxn_datetime<= convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103)";

            //if (TextBox_FromAmount.Text.Length > 0)
            //{
            //    if (RadioButtonList_Module.SelectedIndex == 0 || RadioButtonList_Module.SelectedIndex == 1 || RadioButtonList_Module.SelectedIndex == 2)
            //        filter += " and deposit_amount >= " + int.Parse(TextBox_FromAmount.Text);
            //    else
            //        filter += " and amount_authorized >= " + int.Parse(TextBox_FromAmount.Text);
            //}
            //if (TextBox_ToAmount.Text.Length > 0)
            //{
            //    if (RadioButtonList_Module.SelectedIndex == 0 || RadioButtonList_Module.SelectedIndex == 1 || RadioButtonList_Module.SelectedIndex == 2)
            //        filter += " and deposit_amount <= " + int.Parse(TextBox_FromAmount.Text);
            //    else
            //        filter += " and amount_authorized <= " + int.Parse(TextBox_FromAmount.Text);

            //    //filter += " and amount_authorized <= " + int.Parse(TextBox_ToAmount.Text);
            //}

            //if (TextBox_TSN.Text.Length > 0)
            //    filter += " and seq = '" + TextBox_TSN.Text + "'";
            ////filter += " and tsn = " + int.Parse(TextBox_TSN.Text);

            //if (TextBox_PAN.Text.Length > 0)
            //    filter += " and pan like '%" + TextBox_PAN.Text + "%'";

            //if (CheckBoxList_DisputeStatus.Items[0].Selected && CheckBoxList_DisputeStatus.Items[1].Selected)
            //{
            //}
            //else if (CheckBoxList_DisputeStatus.Items[0].Selected)
            //{
            //    filter += " and dispute_status is not null ";
            //}
            //else if (CheckBoxList_DisputeStatus.Items[1].Selected)
            //{
            //    filter += " and dispute_status is null ";
            //}


            //if (RadioButtonList_Module.SelectedIndex == 0)
            //    filter += " and (is_ccdm = 1 or is_sdm = 1) ";
            //else if (RadioButtonList_Module.SelectedIndex == 1)
            //    filter += " and is_ccdm = 1 ";
            //else if (RadioButtonList_Module.SelectedIndex == 2)
            //    filter += " and is_sdm = 1 ";
            //else if (RadioButtonList_Module.SelectedIndex == 3)
            //    filter += " and is_cdm = 1 ";
            //filter += " and is_eligible=1  ";
            ////if (DropDownList_Comment.SelectedIndex>0)
            ////    filter += " and comment = '"+DropDownList_Comment.SelectedItem.Text+"'";


            //return filter;
            if (TextBox_FromDate.Text.Length > 0)
                filter += " and trxn_datetime >= convert(datetime,'" + TextBox_FromDate.Text + "',103)";

            if (TextBox_ToDate.Text.Length > 0)
                filter += " and trxn_datetime<= convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103)";

            if (TextBox_FromAmount.Text.Length > 0)
            {
                if (RadioButtonList_Module.SelectedIndex == 0)
                    filter += " and deposit_amount >= " + int.Parse(TextBox_FromAmount.Text);
                else if (RadioButtonList_Module.SelectedIndex == 1 || RadioButtonList_Module.SelectedIndex == 2)
                    filter += " and amount_authorized >= " + int.Parse(TextBox_FromAmount.Text);
            }
            if (TextBox_ToAmount.Text.Length > 0)
            {
                if (RadioButtonList_Module.SelectedIndex == 0)
                    filter += " and deposit_amount <= " + int.Parse(TextBox_FromAmount.Text);
                else if (RadioButtonList_Module.SelectedIndex == 1 || RadioButtonList_Module.SelectedIndex == 2)
                    filter += " and amount_authorized <= " + int.Parse(TextBox_FromAmount.Text);

                //filter += " and amount_authorized <= " + int.Parse(TextBox_ToAmount.Text);
            }

            if (TextBox_TSN.Text.Length > 0)
                filter += " and seq = '" + TextBox_TSN.Text + "'";
            //filter += " and tsn = " + int.Parse(TextBox_TSN.Text);

            if (TextBox_PAN.Text.Length > 0)
                filter += " and pan like '%" + TextBox_PAN.Text + "%'";

            if (CheckBoxList_DisputeStatus.Items[0].Selected && CheckBoxList_DisputeStatus.Items[1].Selected)
            {
            }
            else if (CheckBoxList_DisputeStatus.Items[0].Selected)
            {
                filter += " and dispute_status is not null ";
            }
            else if (CheckBoxList_DisputeStatus.Items[1].Selected)
            {
                //filter += " and dispute_status is null ";
                filter += " and (dispute_status is null or dispute_status = '000') ";
            }


            if (RadioButtonList_Module.SelectedIndex == 0 || RadioButtonList_Module.SelectedIndex == 1 || RadioButtonList_Module.SelectedIndex == 4)
                filter += " and is_sdm = 1 ";
            else if (RadioButtonList_Module.SelectedIndex == 2)
                filter += " and is_cdm = 1 ";
            else if (RadioButtonList_Module.SelectedIndex == 3)
                filter += " and is_ccdm = 1 ";


            //if (DropDownList_Comment.SelectedIndex>0)
            //    filter += " and comment = '"+DropDownList_Comment.SelectedItem.Text+"'";



            filter += " and is_eligible=1  ";
            return filter;
        }
        private DataTable GetDataTable(string SQL)
        {
            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            cmd.CommandText = SQL;
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (!CheckRight((int)Permissions.PNCIncidentAnalysis))
                    Response.Redirect("~/admin/AccessDenied.aspx");

                TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");

            }
        }

        private DataTable GetCPMBusinessRuleRejectCausesDataTable()
        {
            string SQL = "select comment comment_text,count(*) counter from ej_parsed_cpm_transaction inner join atm on  ej_parsed_cpm_transaction.atm_id = atm.atm_id" +
                       " where dispute_status <>'000' and dispute_status is not null and status = 'Failed' "+GetFilter()+" and len(comment)>1 group by comment";
            DataTable dt = GetDataTable(SQL);

            return dt;



        }
        private DataTable GetCPMNonBusinessRuleRejectCausesDataTable()
        {
            string SQL = "select comment comment_text ,count(*) counter from ej_parsed_cpm_transaction inner join atm on  ej_parsed_cpm_transaction.atm_id = atm.atm_id" +
                       " where dispute_status is null and status = 'Failed' "+ GetFilter() + " and len(comment)>1 group by comment";
            DataTable dt = GetDataTable(SQL);

            return dt;

        }

        private DataTable GetNoteBusinessRuleRejectCausesDataTable()
        {
            string SQL = "select comment comment_text,count(*) counter from ej_parsed_bna_transaction inner join atm on  ej_parsed_bna_transaction.atm_id = atm.atm_id" +
                       " where dispute_status <>'000' and dispute_status is not null and status = 'Failed' " + GetFilter() + " and len(comment)>1 group by comment";
            DataTable dt = GetDataTable(SQL);

            return dt;



        }
        private DataTable GetNoteNonBusinessRuleRejectCausesDataTable()
        {
            string SQL = "select comment comment_text ,count(*) counter from ej_parsed_bna_transaction inner join atm on  ej_parsed_bna_transaction.atm_id = atm.atm_id" +
                       " where dispute_status is null and status = 'Failed' " + GetFilter() + " and len(comment)>1 group by comment";
            DataTable dt = GetDataTable(SQL);

            return dt;

        }
        
        private DataTable GetCPMRejectCausesDataTable()
        {
            string SQL = "select comment comment_text ,count(*) counter from ej_parsed_cpm_transaction inner join atm on  ej_parsed_cpm_transaction.atm_id = atm.atm_id" +
                       " where status = 'Failed' " + GetFilter() + " and len(comment)>1 group by comment";
            DataTable dt = GetDataTable(SQL);

            return dt;

        }
        private DataTable GetNoteRejectReasonsDataTable()
        {
            string SQL = "select comment comment_text,count(*) counter from ej_parsed_bna_transaction inner join atm on  ej_parsed_bna_transaction.atm_id = atm.atm_id" +
                       " where status = 'Failed' " + GetFilter() +
                       " and comment is not null and len(comment)>1 group by comment";
            DataTable dt = GetDataTable(SQL);

            return dt;
        }
        private string FilterSDM()
        {
            string criteria = null;

            if (CheckBoxList_DisputeStatus.Items[0].Selected && CheckBoxList_DisputeStatus.Items[1].Selected)
                criteria = "";
            else if (CheckBoxList_DisputeStatus.Items[0].Selected)
                criteria = " and dispute_status is not null ";
            else if (CheckBoxList_DisputeStatus.Items[1].Selected)
                criteria = " and (dispute_status is null or dispute_status = '000') ";

            //if (CheckBoxList_Module.Items[0].Selected)
            //    criteria += " and is_sdm = 1 ";
            //else if (CheckBoxList_Module.Items[1].Selected)
            //    criteria += " and is_cdm = 1 ";

            criteria += " and is_sdm = 1 and is_eligible = 1";
            return criteria;
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LogableTask task = LogableTask.NewTask("Search");
            SqlCommand cmd = null;
            DataSet ds = null;
            string reportTitle = null;
            try
            {

                cmd = ConnectionFactory.GetNewCommand(false);
                ds = new dsPNCIncidentsAnalysis();

                if (RadioButtonList_Module.SelectedIndex == 0 || RadioButtonList_Module.SelectedIndex == 3) //Cheque
                {
                    if (RadioButtonList_ReportType.SelectedIndex == 0)
                    {
                        reportTitle = "TOP 5 incidents related to check acceptor";
                        ds.Tables[0].Merge(GetCheckAcceptorGraph());
    
                    }
                    else
                    {//Reject reasons report.
                        int count=0;
                        foreach(ListItem listItem in CheckBoxList_DisputeStatus.Items)
                        {
                            if (listItem.Selected)
                                count++;
                        }
                        if (count==2) 
                        {
                            reportTitle = "CPM Business And Non Business Rule Reject Causes";
                            ds.Tables[0].Merge(GetCPMRejectCausesDataTable());     
                        }
                        else
                        {
                            reportTitle ="CPM "+CheckBoxList_DisputeStatus.SelectedItem.Text+ " Reject Causes";
                            if (CheckBoxList_DisputeStatus.SelectedItem.Text == "Business Rule")
                                ds.Tables[0].Merge(GetCPMBusinessRuleRejectCausesDataTable());     
                            else
                                ds.Tables[0].Merge(GetCPMNonBusinessRuleRejectCausesDataTable());      
                        }
                        
                    }

                    //Check module
                }

                else if (RadioButtonList_Module.SelectedIndex == 2) //Cheque// BNA Selected
                {
                    if (RadioButtonList_ReportType.SelectedIndex == 0)
                    {
                        reportTitle = "TOP 5 incidents related to cash acceptor";
                        ds.Tables[0].Merge(GetCashAcceptorDataTable());

                    }
                    else if (RadioButtonList_ReportType.SelectedIndex == 1)
                    {
                        int count = 0;
                        foreach (ListItem listItem in CheckBoxList_DisputeStatus.Items)
                        {
                            if (listItem.Selected)
                                count++;
                        }
                        if (count == 2)
                        {
                            reportTitle = "BNA Business And Non Business Rule Reject Causes";
                            ds.Tables[0].Merge(GetNoteRejectReasonsDataTable());
                        }
                        else
                        {
                            reportTitle = "BNA " + CheckBoxList_DisputeStatus.SelectedItem.Text + " Reject Causes";
                            if (CheckBoxList_DisputeStatus.SelectedItem.Text == "Business Rule")
                                ds.Tables[0].Merge(GetNoteBusinessRuleRejectCausesDataTable());
                            else
                                ds.Tables[0].Merge(GetNoteNonBusinessRuleRejectCausesDataTable());
                        }
                     }
                }
                else
                {
                    //SDM complete
                    if (RadioButtonList_ReportType.SelectedIndex == 0)
                    {
                        reportTitle = "TOP 5 incidents related to SDM";
                        string SQL = "select top 5 comment comment_text ,count(*) counter from ej_parsed_bna_transaction inner join atm on atm.atm_id =ej_parsed_bna_transaction.atm_id " +
                       " where atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and trxn_datetime>=convert(datetime,'" + this.TextBox_FromDate.Text + "',103)  " +
                       " and trxn_datetime<=convert(datetime,'" + this.TextBox_ToDate.Text + " 23:59:59',103) " + FilterSDM() +
                       " and comment is not null and len(comment)>1 group by comment order by count(*) desc";
                        DataTable dt = GetDataTable(SQL);
                        //ds.Tables[0].Merge(GetCashAcceptorDataTable());
                        SQL = "select top 5 comment comment_text,count(*)  counter from ej_parsed_cpm_transaction inner join atm on atm.atm_id =ej_parsed_cpm_transaction.atm_id  " +
                      " where atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and trxn_datetime>=convert(datetime,'" + this.TextBox_FromDate.Text + "',103)  " +
                      " and trxn_datetime<=convert(datetime,'" + this.TextBox_ToDate.Text + " 23:59:59',103) " + FilterSDM() +
                      " and comment is not null and len(comment)>1 group by comment order by count(*) desc";
                        DataTable dt1 = GetDataTable(SQL);
                        dt.Merge(dt1);
                        dt.DefaultView.Sort = "counter desc";
                        DataView dataview = dt.DefaultView;
                        DataTable newDataTable = dt1.Clone();
                        
                        int toLoop = dt.Rows.Count > 5 ? 5 : dt.Rows.Count;
                        for (int i = 0; i < toLoop; i++)
                        {
                            DataRow newRow = newDataTable.NewRow();
                            newRow[0] = dataview[i]["comment_text"];
                            newRow[1] =dataview[i]["counter"];
                            newDataTable.Rows.Add(newRow);
                        }
                        ds.Tables[0].Merge(newDataTable);
                        
                        
                    }
                    else if (RadioButtonList_ReportType.SelectedIndex == 1)
                    {
                        int count = 0;
                        foreach (ListItem listItem in CheckBoxList_DisputeStatus.Items)
                        {
                            if (listItem.Selected)
                                count++;
                        }
                        if (count == 2)
                        {
                            reportTitle = "SDM Business And Non Business Rule Reject Causes";
                         //   ds.Tables[0].Merge(GetNoteRejectReasonsDataTable());
                        }
                        else
                        {
                            reportTitle = "SDM " + CheckBoxList_DisputeStatus.SelectedItem.Text + " Reject Causes";
                           // if (CheckBoxList_DisputeStatus.SelectedItem.Text == "Business Rule")
                           //     ds.Tables[0].Merge(GetNoteBusinessRuleRejectCausesDataTable());
                           // else
                           //     ds.Tables[0].Merge(GetNoteNonBusinessRuleRejectCausesDataTable());
                        }
                        string SQL = "select comment comment_text,count(*) counter from ej_parsed_cpm_transaction inner join atm on ej_parsed_cpm_transaction.atm_id = atm.atm_id " +
                " where 1=1 "+ GetFilter() + " and len(comment)>1 group by comment";
                        DataTable dt = GetDataTable(SQL);
                        //foreach (DataRow dr in dt.Rows)
                        //{
                        //    comment = dr[0].ToString();
                        //    if (comment.Length > 30)
                        //        comment = comment.Substring(0, 30);

                        //    Chart_BusinessRuleRejectCauses.Series[0].Points.AddXY(comment, dr[1]);
                        //}

                        SQL = "select comment comment_text,count(*) counter  from ej_parsed_bna_transaction inner join atm on  ej_parsed_bna_transaction.atm_id = atm.atm_id" +
                                    " where 1=1 " + GetFilter() + " and len(comment)>1 group by comment";
                        DataTable dt1 = GetDataTable(SQL);

                        //foreach (DataRow dr in dt1.Rows)
                        //{
                        //    comment = dr[0].ToString();
                        //    if (comment.Length > 30)
                        //        comment = comment.Substring(0, 30);
                        //    Chart_BusinessRuleRejectCauses.Series[0].Points.AddXY(comment, dr[1]);
                        //}
                        //Chart_BusinessRuleRejectCauses.Titles[0].Text = "BUSINESS RULE REJECT CAUSES";
                        ////Chart_BusinessRuleRejectCauses.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = true;
                        //Chart_BusinessRuleRejectCauses.Series[0]["PieDrawingStyle"] = "SoftEdge";
                        ////Chart_BusinessRuleRejectCauses.Legends[0].Enabled = true;

                        //Chart_BusinessRuleRejectCauses.Series["Default"]["ShowMarkerLines"] = "true";
                        //Chart_BusinessRuleRejectCauses.Series["Default"]["DrawingStyle"] = "Cylinder";
                        //Chart_BusinessRuleRejectCauses.Series["Default"]["PointWidth"] = "0.8";
                        dt.Merge(dt1);
                        ds.Tables[0].Merge(dt);
                        //GridView_BusinessRuleRejectCauses.DataSource = dt;
                        //GridView_BusinessRuleRejectCauses.DataBind();
      
                    }
                }
               

                
                AppUser objUser = (AppUser)Session[SessionVars.user.ToString()];


                if (ds.Tables["DataTable1"].Rows.Count > 0)
                {
                    Session[SessionVars.dtReportDataSet.ToString()] = ds;
                    string URL = "ReportPopup.aspx?Report=PNCIncidentAnalysisRep.rpt&ReportTitle=" + reportTitle + "&FromDate=" + Utility.GetDateTime(TextBox_FromDate.Text) + "&ToDate=" + Utility.GetDateTime(TextBox_ToDate.Text) + "&GeneratedBy=" + objUser.UserLogin + "&total=" + ds.Tables["DataTable1"].Rows.Count;
                    Literal_ok.Text = "<script>window.open(\"" + URL + "\");</script>";
                }
                else
                    Literal_ok.Text = "<li>No record(s) found</li>";
            }

            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_error.Text = "<li> An error occurred while generating Report::" + ex.Message;
            }
            finally
            {
                if (cmd != null)
                    if (cmd.Connection != null)
                        cmd.Connection.Close();
                task.EndTask();
            }
        }
        private DataTable GetCashAcceptorDataTable()
        {
            string SQL = "select top 5 comment comment_text,count(*) counter from ej_parsed_bna_transaction inner join atm on  ej_parsed_bna_transaction.atm_id = atm.atm_id" +
                       " where 1=1 " + GetFilter() +
                       " and comment is not null and len(comment)>1 group by comment order by count(*) desc";
            DataTable dt = GetDataTable(SQL);
            return dt;
        }
        private void AddTsn(ref DataTable dt,string tableName)
        {
            dt.Columns.Add("trxn_ref_no");
            StringBuilder builder = new StringBuilder();
            foreach (DataRow dr in dt.Rows)
            {
                builder.Append("'" + dr["comment_text"] + "',");
            }
            builder.Append("'-1'");
            DataTable result = GetDataTable("select seq,comment from " + tableName + " where comment in (" + builder.ToString() + ")");
            
            foreach (DataRow dr in dt.Rows)
            {
                builder = new StringBuilder();
                DataRow[] dataRows = result.Select("comment='" + dr["comment_text"] + "'");
                foreach (DataRow datarow in dataRows)
                {
                    if (datarow["seq"]!=DBNull.Value)
                    builder.Append(datarow["seq"] + " ");
                }
                dr["trxn_ref_no"] = builder.ToString();
            }
        }
        private DataTable GetCheckAcceptorGraph()
        {
            string SQL = "select top 5 comment comment_text,count(*) counter from ej_parsed_cpm_transaction inner join atm on  ej_parsed_cpm_transaction.atm_id = atm.atm_id" +
                       " where 1=1 " + GetFilter() +
                       " and comment is not null and len(comment)>1 group by comment order by count(*) desc";
            DataTable dt = GetDataTable(SQL);
            //AddTsn(ref dt, "ej_parsed_cpm_transaction");
            return dt;

        }
        protected void Button_Reset_Click(object sender, EventArgs e)
        {
            TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
        }
        protected void CheckBox_Current_CheckedChanged(object sender, EventArgs e)
        {

            TextBox_FromDate.Enabled = (!TextBox_FromDate.Enabled);
            TextBox_ToDate.Enabled = (!TextBox_ToDate.Enabled);

        }

    }
}