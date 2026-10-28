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
    public partial class PNCDepositWeeklyAnalysis : BasePage
    {
        private DataTable GetDataTable(string SQL)
        {
            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            cmd.CommandText = SQL;
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }
        string GetFilter()
        {
            string filter = "";

            filter = " and atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ")";


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

            //if (CheckBoxList_Status.Items[0].Selected && CheckBoxList_Status.Items[1].Selected)
            //{
            //}
            //else if (CheckBoxList_Status.Items[0].Selected)
            //    filter += " and status = 'Successful'";
            //else if (CheckBoxList_Status.Items[1].Selected)
            //    filter += " and status = 'Failed'";

            //if (RadioButtonList_Module.SelectedIndex == 0)
            //    filter += " and (is_ccdm = 1 or is_sdm = 1) ";
            //else if (RadioButtonList_Module.SelectedIndex == 1)
            //    filter += " and is_ccdm = 1 ";
            //else if (RadioButtonList_Module.SelectedIndex == 2)
            //    filter += " and is_sdm = 1 ";
            //else if (RadioButtonList_Module.SelectedIndex == 3)
            //    filter += " and is_cdm = 1 ";

            ////if (DropDownList_Comment.SelectedIndex>0)
            ////    filter += " and comment = '"+DropDownList_Comment.SelectedItem.Text+"'";

            //filter += " and is_eligible=1  ";
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

            if (CheckBoxList_Status.Items[0].Selected && CheckBoxList_Status.Items[1].Selected)
            {
            }
            else if (CheckBoxList_Status.Items[0].Selected)
                filter += " and status = 'Successful'";
            else if (CheckBoxList_Status.Items[1].Selected)
                filter += " and status = 'Failed'";

            if (RadioButtonList_Module.SelectedIndex == 0 || RadioButtonList_Module.SelectedIndex == 1)
                filter += " and is_sdm = 1 ";
            else if (RadioButtonList_Module.SelectedIndex == 2)
                filter += " and is_cdm = 1 ";

            //if (DropDownList_Comment.SelectedIndex>0)
            //    filter += " and comment = '"+DropDownList_Comment.SelectedItem.Text+"'";



            filter += " and is_eligible=1  ";
            return filter;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (!CheckRight((int)Permissions.PNCDepositSummary))
                    Response.Redirect("~/admin/AccessDenied.aspx");

                int yr = DateTime.Today.Year;
                int month = DateTime.Today.Month;
                DateTime from = new DateTime(yr, month, 1);
                TextBox_FromDate.Text = from.ToString("dd/MM/yyyy");
                TextBox_ToDate.Text = from.AddMonths(1).AddDays(-1).ToString("dd/MM/yyyy");

            }
        }


        protected void btnSearch_Click(object sender, EventArgs e)
        {
            int totalDeposits = 0;
            int totalRejects = 0;

            DataRow newRow = null;
            LogableTask task = LogableTask.NewTask("Search");
            SqlCommand cmd = null;
            DataSet ds = null;
            try
            {
                DateTime fromByUser = DateTime.ParseExact(TextBox_FromDate.Text, "dd/MM/yyyy", null);
                DateTime toByUser = DateTime.ParseExact(TextBox_ToDate.Text, "dd/MM/yyyy", null);
                DateTime temp = new DateTime(toByUser.Year, toByUser.Month, 1);
                temp = temp.AddMonths(1).AddDays(-1);

                if (fromByUser.Day != 1 || toByUser != temp)
                    throw new Exception("Invalid date entered for weekly analysis.From Date should be start date of month and end data should be end data of the same/other month");

                DataTable dtGrid = new DataTable();
                dtGrid.Columns.Add("title");
                dtGrid.Columns.Add("month_year");
                dtGrid.Columns.Add("acc_perc_week1", typeof(decimal));
                dtGrid.Columns.Add("acc_perc_week2", typeof(decimal));
                dtGrid.Columns.Add("acc_perc_week3", typeof(decimal));
                dtGrid.Columns.Add("acc_perc_week4", typeof(decimal));
                dtGrid.Columns.Add("rej_perc_week1", typeof(decimal));
                dtGrid.Columns.Add("rej_perc_week2", typeof(decimal));
                dtGrid.Columns.Add("rej_perc_week3", typeof(decimal));
                dtGrid.Columns.Add("rej_perc_week4", typeof(decimal));
                dtGrid.Columns.Add("items_accepted_week1", typeof(decimal));
                dtGrid.Columns.Add("items_rejected_week1", typeof(decimal));
                dtGrid.Columns.Add("items_processed_week1", typeof(decimal));
                dtGrid.Columns.Add("items_accepted_week2", typeof(decimal));
                dtGrid.Columns.Add("items_rejected_week2", typeof(decimal));
                dtGrid.Columns.Add("items_processed_week2", typeof(decimal));
                dtGrid.Columns.Add("items_accepted_week3", typeof(decimal));
                dtGrid.Columns.Add("items_rejected_week3", typeof(decimal));
                dtGrid.Columns.Add("items_processed_week3", typeof(decimal));
                dtGrid.Columns.Add("items_accepted_week4", typeof(decimal));
                dtGrid.Columns.Add("items_rejected_week4", typeof(decimal));
                dtGrid.Columns.Add("items_processed_week4", typeof(decimal));


                cmd = ConnectionFactory.GetNewCommand(false);
                List<string> listDateTime = new List<string>();
                DateTime dtFromDate = DateTime.ParseExact(TextBox_FromDate.Text, "dd/MM/yyyy", null);
                DateTime dtToDate = DateTime.ParseExact(TextBox_ToDate.Text, "dd/MM/yyyy", null);
                DateTime dtTemp = dtFromDate;
                //listDateTime.Add(dtFromDate);
                dtFromDate = dtFromDate.AddDays(-1);
                TimeSpan diff = dtToDate - dtFromDate;
                //   int weeks = (int)Math.Round(((diff.TotalDays-6) / 7));
                int weeks = (int)Math.Round((diff.TotalDays / 31 * 4));
                while (weeks % 4 != 0)
                    weeks++;

                for (int i = 0; i < weeks; i++)
                {
                    dtFromDate = dtFromDate.AddDays(1);
                    listDateTime.Add(dtFromDate.ToString("MM/dd/yyyy"));

                    if ((i + 1) % 4 == 0)
                    {
                        dtFromDate = dtFromDate.AddDays(6);//Days till End of month
                        dtTemp = dtTemp.AddMonths(1);
                        dtFromDate = dtTemp.AddDays(-1);
                    }
                    else
                        dtFromDate = dtFromDate.AddDays(6);
                    listDateTime.Add(dtFromDate.ToString("MM/dd/yyyy"));
                }


                if (RadioButtonList_Module.SelectedIndex == 0  )
                {

                    cmd.CommandTimeout = 500;

                    string SQL = "select atm.atm_id,title, trxn_datetime,dispute_status,status from ej_parsed_cpm_transaction left join ej_parsed_cpm_transaction_detail on ej_parsed_cpm_transaction.ej_parsed_cpm_transaction_id = ej_parsed_cpm_transaction_detail.ej_parsed_cpm_transaction_id " +
                                 " inner join atm on ej_parsed_cpm_transaction.atm_id = atm.atm_id  where atm.is_active=1 " + GetFilter();

                    DataTable dt = GetDataTable(SQL);

                    DataTable dtATMs = dt.DefaultView.ToTable(true, new string[] { "atm_id", "title" });
                    int counter = 0;
                    int counter1 = 0;
                    foreach (DataRow dr in dtATMs.Rows)
                    {
                        counter = 0;
                        for (int j = 0; j < listDateTime.Count; j = j + 2)
                        {

                            if (counter % 4 == 0)
                            {
                                if (newRow != null)
                                    dtGrid.Rows.Add(newRow);
                                newRow = dtGrid.NewRow();
                                newRow["title"] = dr["title"];
                                newRow["month_year"] = DateTime.ParseExact(listDateTime[j], "MM/dd/yyyy", null).ToString("MM-yyyy");
                                counter1 = 0;

                            }
                            counter++;
                            counter1++;
                            string criteria = "trxn_datetime>=#" + listDateTime[j] + "# and trxn_datetime<=#" + listDateTime[j + 1] + " 23:59:59#  and atm_id = " + dr["atm_id"] + " and dispute_status ='000' and status = 'Successful'";
                            //LogableTask.LogMonoActivityTask("Check", MethodBase.GetCurrentMethod(), TraceLevel.Info, "criteria: "+criteria);
                            DataRow[] drAccepted = dt.Select(criteria);
                            DataRow[] drRejected = dt.Select("trxn_datetime>=#" + listDateTime[j] + "# and trxn_datetime<=#" + listDateTime[j + 1] + " 23:59:59#  and atm_id = " + dr["atm_id"] + " and status = 'Failed'");
                            //LogableTask.LogMonoActivityTask("Check", MethodBase.GetCurrentMethod(), TraceLevel.Info, "rej: " + "trxn_datetime>=#" + listDateTime[j] + "# and trxn_datetime<=#" + listDateTime[j + 1] + "#  and atm_id = " + dr["atm_id"] + " and status = 'Failed'");


                            int totalChecks = drRejected.Length + drAccepted.Length;
                            //LogableTask.LogMonoActivityTask("Check", MethodBase.GetCurrentMethod(), TraceLevel.Info, "rej: " + "total chq:" + totalChecks);


                            if (totalChecks != 0)
                            {
                                newRow["acc_perc_week" + counter1] = Math.Round(((decimal)drAccepted.Length / totalChecks) * 100, 2);
                                newRow["rej_perc_week" + counter1] = Math.Round((((decimal)drRejected.Length) / totalChecks) * 100, 2);
                            }
                            else
                            {
                                newRow["acc_perc_week" + counter1] = 0;
                                newRow["rej_perc_week" + counter1] = 0;
                            }

                        }
                        dtGrid.Rows.Add(newRow);
                        newRow = null;
                        counter1 = 0;

                        //if (totalChecks > 0)
                        //    Label_AccPerc.Text = Math.Round((decimal)drAccepted.Length / totalChecks * 100).ToString();
                        //else
                        //    Label_AccPerc.Text = "0";


                    }
                    ds = new dsPNCNotesDepWeeklyAnalysis();
                    ds.Tables[0].Merge(dtGrid);
                }
                else
                {

                    //SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                    cmd.CommandTimeout = 500;
                    int depositCount = 0;
                    int rej1 = 0;
                    int rej2 = 0;
                    int total = 0;

                    string SQL = @"select atm.atm_id,title, trxn_datetime,seq,account_type,pan,amount_authorized,dispute_status,status,comment,note_type,notes_count,ej_parsed_bna_transaction.ej_parsed_bna_transaction_Id
                                    from ej_parsed_bna_transaction inner join atm on ej_parsed_bna_transaction.atm_id = atm.atm_id
                                    inner join ej_parsed_bna_transaction_detail on  ej_parsed_bna_transaction_detail.ej_parsed_bna_transaction_id = 
                                    ej_parsed_bna_transaction.ej_parsed_bna_transaction_id
                                     where atm.is_active=1 " + GetFilter();


                    //string SQL = "select atm.atm_id,title,convert(varchar,trxn_datetime,103) trxn_datetime,dispute_status,status from ej_parsed_cpm_transaction left join ej_parsed_cpm_transaction_detail on ej_parsed_cpm_transaction.ej_parsed_cpm_transaction_id = ej_parsed_cpm_transaction_detail.ej_parsed_cpm_transaction_id " +
                    //             " inner join atm on ej_parsed_cpm_transaction.atm_id = atm.atm_id  where atm.is_active=1 and trxn_datetime>=convert(datetime,'" + fromDate + "',103)  " +
                    //             " and trxn_datetime<=convert(datetime,'" + toDate + " 23:59:59',103) ";

                    DataTable dt = GetDataTable(SQL);

                    DataTable dtATMs = dt.DefaultView.ToTable(true, new string[] { "atm_id", "title" });
                    //      DataTable dtATMsTrxnDatetime = dt.DefaultView.ToTable(true, new string[] { "atm_id", "trxn_datetime" });
                    int counter = 0;
                    int counter1 = 0;
                    foreach (DataRow dr in dtATMs.Rows)
                    {
                        counter = 0;
                        for (int j = 0; j < listDateTime.Count; j = j + 2)
                        {

                            if (counter % 4 == 0)
                            {
                                if (newRow != null)
                                    dtGrid.Rows.Add(newRow);
                                newRow = dtGrid.NewRow();
                                newRow["title"] = dr["title"];
                                newRow["month_year"] = DateTime.ParseExact(listDateTime[j], "MM/dd/yyyy", null).ToString("MM-yyyy");
                                counter1 = 0;
                               
                            }
                            depositCount = 0;
                            rej1 = 0;
                            rej2 = 0;
                            counter++;
                            counter1++;
                            string criteria = "trxn_datetime>=#" + listDateTime[j]+ "# and trxn_datetime<=#" + listDateTime[j+1] + " 23:59:59#  and atm_id = " + dr["atm_id"] + " and dispute_status ='000' and status = 'Successful'";
                            //LogableTask.LogMonoActivityTask("Check", MethodBase.GetCurrentMethod(), TraceLevel.Info, "criteria: " + criteria);
                            
                            //DataRow[] drTrxnDates = dtATMsTrxnDatetime.Select("atm_id = " + dr["atm_id"]);

                            //foreach (DataRow drInner in drTrxnDates)
                            //{
                            //    rej1 = 0;
                            //    rej2 = 0;
                            //    total = 0;
                            //    depositCount = 0;

                            DataRow[] drAllTrxnForTheDay = dt.Select(criteria);
                                //"atm_id = " + dr["atm_id"] + " and trxn_datetime='" + drInner["trxn_datetime"] + "' and status = 'Successful'");
                            if (drAllTrxnForTheDay.Length > 0)
                            {

                                DataTable dtBNATrxnIds = drAllTrxnForTheDay.CopyToDataTable().DefaultView.ToTable(true, new string[] { "ej_parsed_bna_transaction_id" });

                                foreach (DataRow drRow in dtBNATrxnIds.Rows)
                                {

                                    StringBuilder builder = new StringBuilder();
                                    DataRow[] dr1 = dt.Select("ej_parsed_bna_transaction_id=" + drRow["ej_parsed_bna_transaction_id"]);
                                    if (dr1.Length > 0)
                                    {
                                        DataTable dtUniqueNoteType = dr1.CopyToDataTable().DefaultView.ToTable(true, new string[] { "note_type" });
                                        foreach (DataRow drUniqueNoteType in dtUniqueNoteType.Rows)
                                        {
                                            DataRow[] innerTemp = dt.Select("ej_parsed_bna_transaction_id=" + drRow["ej_parsed_bna_transaction_id"] + " and note_type=" + drUniqueNoteType["note_type"], "notes_count");
                                            builder.Append(drUniqueNoteType["note_type"].ToString() + "*" + int.Parse(innerTemp[innerTemp.Length - 1]["notes_count"].ToString()) + "=" + int.Parse(drUniqueNoteType["note_type"].ToString()) * int.Parse(innerTemp[innerTemp.Length - 1]["notes_count"].ToString()) + "<br/>");
                                            depositCount += int.Parse(innerTemp[innerTemp.Length - 1]["notes_count"].ToString());
                                        }

                                    }
                                }
                            }

                            //DataRow newRow = dtGrid.NewRow();
                            //newRow["title"] = dr["title"];
                            //newRow["month_year"] = DateTime.ParseExact(drInner["trxn_datetime"].ToString(), "MM/dd/yyyy", null).ToString("MM-yyyy");
                            ////newRow["notes_accepted"] = depositCount;



                            drAllTrxnForTheDay = dt.Select("atm_id = " + dr["atm_id"] + " and trxn_datetime>=#" + listDateTime[j] + "# and trxn_datetime<=#" + listDateTime[j + 1] + " 23:59:59# and status = 'Failed' and dispute_status is not null");
                            if (drAllTrxnForTheDay.Length > 0)
                            {
                                DataTable dtBNATrxnIds = drAllTrxnForTheDay.CopyToDataTable().DefaultView.ToTable(true, new string[] { "ej_parsed_bna_transaction_id" });

                                foreach (DataRow drRow in dtBNATrxnIds.Rows)
                                {

                                    StringBuilder builder = new StringBuilder();
                                    DataRow[] dr1 = dt.Select("ej_parsed_bna_transaction_id=" + drRow["ej_parsed_bna_transaction_id"]);
                                    if (dr1.Length > 0)
                                    {
                                        DataTable dtUniqueNoteType = dr1.CopyToDataTable().DefaultView.ToTable(true, new string[] { "note_type" });
                                        foreach (DataRow drUniqueNoteType in dtUniqueNoteType.Rows)
                                        {
                                            DataRow[] innerTemp = dt.Select("ej_parsed_bna_transaction_id=" + drRow["ej_parsed_bna_transaction_id"] + " and note_type=" + drUniqueNoteType["note_type"], "notes_count");
                                            builder.Append(drUniqueNoteType["note_type"].ToString() + "*" + int.Parse(innerTemp[innerTemp.Length - 1]["notes_count"].ToString()) + "=" + int.Parse(drUniqueNoteType["note_type"].ToString()) * int.Parse(innerTemp[innerTemp.Length - 1]["notes_count"].ToString()) + "<br/>");
                                            rej1 += int.Parse(innerTemp[innerTemp.Length - 1]["notes_count"].ToString());
                                        }

                                    }
                                }
                            }



                            //newRow["notes_rejected1"] = rej1;

                            drAllTrxnForTheDay = dt.Select("atm_id = " + dr["atm_id"] + " and trxn_datetime>=#" + listDateTime[j] + "# and trxn_datetime<=#" + listDateTime[j + 1] + " 23:59:59# and status = 'Failed' and dispute_status is null");
                            if (drAllTrxnForTheDay.Length > 0)
                            {
                                DataTable dtBNATrxnIds = drAllTrxnForTheDay.CopyToDataTable().DefaultView.ToTable(true, new string[] { "ej_parsed_bna_transaction_id" });

                                foreach (DataRow drRow in dtBNATrxnIds.Rows)
                                {

                                    StringBuilder builder = new StringBuilder();
                                    DataRow[] dr1 = dt.Select("ej_parsed_bna_transaction_id=" + drRow["ej_parsed_bna_transaction_id"]);
                                    if (dr1.Length > 0)
                                    {
                                        DataTable dtUniqueNoteType = dr1.CopyToDataTable().DefaultView.ToTable(true, new string[] { "note_type" });
                                        foreach (DataRow drUniqueNoteType in dtUniqueNoteType.Rows)
                                        {
                                            DataRow[] innerTemp = dt.Select("ej_parsed_bna_transaction_id=" + drRow["ej_parsed_bna_transaction_id"] + " and note_type=" + drUniqueNoteType["note_type"], "notes_count");
                                            builder.Append(drUniqueNoteType["note_type"].ToString() + "*" + int.Parse(innerTemp[innerTemp.Length - 1]["notes_count"].ToString()) + "=" + int.Parse(drUniqueNoteType["note_type"].ToString()) * int.Parse(innerTemp[innerTemp.Length - 1]["notes_count"].ToString()) + "<br/>");
                                            rej2 += int.Parse(innerTemp[innerTemp.Length - 1]["notes_count"].ToString());
                                        }

                                    }
                                }


                            }
                           // newRow["notes_rejected2"] = rej2;
                            //newRow["total_rejected"] = rej1 + rej2;
                            total = rej1 + rej2 + depositCount;
                           // LogableTask.LogMonoActivityTask("Check", MethodBase.GetCurrentMethod(), TraceLevel.Info, "total: " + total);
                            
                            if (total != 0)
                            {
                                newRow["acc_perc_week" + counter1] = Math.Round(((decimal)depositCount / total) * 100, 2);
                                newRow["rej_perc_week" + counter1] = Math.Round((((decimal)rej1+rej2) / total) * 100, 2);
                                newRow["items_accepted_week" + counter1] = depositCount;
                                newRow["items_rejected_week" + counter1] = rej1 + rej2;
                                newRow["items_processed_week" + counter1] = total;
                                totalDeposits += depositCount;
                                totalRejects += rej1 + rej2;
                            }
                            else
                            {
                                newRow["acc_perc_week" + counter1] = 0;
                                newRow["rej_perc_week" + counter1] = 0;
                                newRow["items_accepted_week" + counter1] = 0;
                                newRow["items_rejected_week" + counter1] = 0;
                                newRow["items_processed_week" + counter1] = 0;
                            }


                           

                            //}

                        }
                        dtGrid.Rows.Add(newRow);
                        newRow = null;
                        counter1 = 0;
                        depositCount = 0;
                        rej1 = 0;
                        rej2 = 0;

                    }
                    ds = new DsPNCNoteDepSummary();
                    ds.Tables[0].Merge(dtGrid);
                }

                //task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Info, "query ->"+cmd.CommandText);

                //SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                //adapter.Fill(ds.Tables["DataTable1"]);
                ////SplitAlertMessageIntoColumns(ds.Tables["DataTable1"]);

                AppUser objUser = (AppUser)Session[SessionVars.user.ToString()];


                if (ds.Tables["DataTable1"].Rows.Count > 0)
                {
                    string title = null;
                    Session[SessionVars.dtReportDataSet.ToString()] = ds;

                    if (RadioButtonList_Module.SelectedIndex == 0)
                        title = "CPM Weekly Analysis";
                    else
                        title = "BNA Weekly Analysis";

                    decimal accPerc = 0;
                    decimal rejPerc = 0;

                    if (totalDeposits + totalRejects != 0)
                    {
                        accPerc = Math.Round((decimal)totalDeposits / (totalDeposits + totalRejects) * 100, 2);
                        rejPerc = Math.Round((decimal)totalRejects / (totalDeposits + totalRejects) * 100, 2);

                            //rptDoc.SetParameterValue("accPerc", Math.Round((decimal)totalDeposits / (totalDeposits + totalRejects) * 100, 2));
                        //rptDoc.SetParameterValue("rejPerc", Math.Round((decimal)totalRejects / (totalDeposits + totalRejects) * 100, 2));
                    }
                    


                    string URL = "ReportPopup.aspx?Report=PNCNotesDepWeeklyAnalysis.rpt&ReportTitle=" + title + "&FromDate=" + Utility.GetDateTime(TextBox_FromDate.Text) + "&ToDate=" + Utility.GetDateTime(TextBox_ToDate.Text) + "&GeneratedBy=" + objUser.UserLogin + "&total=" + ds.Tables["DataTable1"].Rows.Count+"&accPerc="+accPerc+"&rejPerc="+rejPerc;
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