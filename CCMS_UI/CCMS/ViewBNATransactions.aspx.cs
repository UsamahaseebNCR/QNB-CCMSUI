using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Avanza.iSuite.UI;
using System.Data.SqlClient;

using System.Data;
using System.Reflection;
using System.Diagnostics;

namespace CCMSUI.CCMS
{
    public partial class ViewBNATransactions : BasePage
    {
        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        }
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!Page.IsPostBack)
            {
                if (!CheckRight((int)Permissions.ViewBNATransactions, true))
                    Response.Redirect("~/admin/AccessDenied.aspx");

                ViewState["SortExpression"] = "last_bna_deposit_at";
                ViewState["sortDirection"] = " desc";
                ViewState["orderby"] = ViewState["SortExpression"] + " " + ViewState["sortDirection"];

                TextBox_FromDate.Text = DateTime.Today.ToString("dd/MM/yyyy HH:mm:ss");
                TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
                if (System.Web.Configuration.WebConfigurationManager.AppSettings["IsDisplayDepositDenomination"] == "true")
                     GridView_Trxn.Columns[3].Visible = GridView_Trxn.Columns[5].Visible = GridView_Trxn.Columns[7].Visible = GridView_Trxn.Columns[9].Visible = GridView_Trxn.Columns[11].Visible = true;
                else
                    GridView_Trxn.Columns[3].Visible = GridView_Trxn.Columns[5].Visible = GridView_Trxn.Columns[7].Visible = GridView_Trxn.Columns[9].Visible = GridView_Trxn.Columns[11].Visible = false;
             

             }
        }


        protected void GridView_Trxn_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView_Trxn.PageIndex = e.NewPageIndex;
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
            //BindGridWithCache();
        }

        protected void GridView_Trxn_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (ViewState["SortExpression"].ToString().IndexOf(e.SortExpression) >= 0)
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
            //if (ViewState["SortExpression"].ToString() == "creation_time")
            //    ViewState["SortExpression"] = "f.creation_time";

            ViewState["orderby"] = ViewState["SortExpression"] + " " + ViewState["sortDirection"];
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
            //BindGridWithCache();
        }



        void BindGrid(string orderBy)
        {
            LogableTask task = LogableTask.NewTask("BindGrid");
            try
            {
                SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
                cmd.CommandTimeout = 500;

//                cmd.CommandText = @"select title,bna_cassette1,bna_cassette2,bna_cassette3,bna_cassette4,bna_cassette5,last_bna_deposit_at
//                                    from vDepositPosition
//                                    where atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") " + GetFilter()
//                + " order by " + ViewState["orderby"];

                cmd.CommandText = @"select title,bna_cassette1,bna_cassette2,bna_cassette3,bna_cassette4,bna_cassette5,last_bna_deposit_at,
cassette1_denomination_detail,cassette2_denomination_detail,cassette3_denomination_detail,cassette4_denomination_detail,purge_denomination_detail,'' total

                                    from vDepositPosition
                                    where atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") " + GetFilter()
                + " order by " + ViewState["orderby"];

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dtTrxn = new DataTable();
                adapter.Fill(dtTrxn);

                SetDepositsAmount(dtTrxn);

                Literal_count.Text = dtTrxn.Rows.Count + " records.";
                this.GridView_Trxn.DataSource = dtTrxn;
                this.GridView_Trxn.DataBind();
            }
            catch (Exception ex)
            {
                Literal_error.Text = ex.Message;
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            { task.EndTask(); }
        }

        private void SetDepositsAmount(DataTable pDataTable)
        {
            for (int i = 0; i < pDataTable.Rows.Count; i++)
            {
                pDataTable.Rows[i]["total"] = ExtractDepositAmount(pDataTable.Rows[i]["cassette1_denomination_detail"].ToString(), pDataTable.Rows[i]["cassette2_denomination_detail"].ToString(), pDataTable.Rows[i]["cassette3_denomination_detail"].ToString(), pDataTable.Rows[i]["cassette4_denomination_detail"].ToString());
            }
        }
        //Edited by Muhammad Shaharyar for QIB
        private string ExtractDepositAmount(string pCassette1Detail, string pCassette2Detail, string pCassette3Detail, string pCassette4Detail)
        {

            int[] cassette1 = !string.IsNullOrEmpty(pCassette1Detail) ? ParseCassetteDetail(pCassette1Detail) : new int[0];
            int[] cassette2 = !string.IsNullOrEmpty(pCassette2Detail) ? ParseCassetteDetail(pCassette2Detail) : new int[0];
            int[] cassette3 = !string.IsNullOrEmpty(pCassette3Detail) ? ParseCassetteDetail(pCassette3Detail) : new int[0];
            int[] cassette4 = !string.IsNullOrEmpty(pCassette4Detail) ? ParseCassetteDetail(pCassette4Detail) : new int[0];
            int cassette1total = !string.IsNullOrEmpty(pCassette1Detail) ? ParseCassettetotal(pCassette1Detail) : 0;
            int cassette2total = !string.IsNullOrEmpty(pCassette2Detail) ? ParseCassettetotal(pCassette2Detail) : 0;
            int cassette3total = !string.IsNullOrEmpty(pCassette3Detail) ? ParseCassettetotal(pCassette3Detail) : 0;
            int cassette4total = !string.IsNullOrEmpty(pCassette4Detail) ? ParseCassettetotal(pCassette4Detail) : 0;
            int[] cassette = ParseCassette(pCassette1Detail);

            int[] cassetteDetailTotal = new int[cassette1.Length];
            for (int i = 0; i < cassette1.Length; i++)
            {
                cassetteDetailTotal[i] = cassette1[i] + cassette2[i] + cassette3[i] + cassette4[i];
            }

            int cassettetotal = cassette1total + cassette2total + cassette3total + cassette4total;

            string data = null;

            if (System.Web.Configuration.WebConfigurationManager.AppSettings["IsDisplayDepositDenomination"] == "true")
            {

                for (int i = 0; i < cassetteDetailTotal.Length - 1; i++)
                {
                    data += cassette[i].ToString() + "*" + cassetteDetailTotal[i].ToString() + "<br>";
                }

                data += "=" + cassettetotal.ToString();
            }
            else
            {
                data = cassettetotal.ToString();
            }


            return data;



        }
        //Added by Muhammad Shaharyar for QIB
        private int[] ParseCassetteDetail(string pCassetteDetail)
        {

            string[] cassetteDetails = null;
            string[] seperator = { "<br>" };
            cassetteDetails = pCassetteDetail.Split(seperator, StringSplitOptions.RemoveEmptyEntries);
            int[] cassettecount = new int[cassetteDetails.Length];
            for (int i = 0; i < cassetteDetails.Length - 1; i++)
            {

                string[] temp = cassetteDetails[i].Split('*');
                cassettecount[i] = int.Parse(temp[1]);
            }

            return cassettecount;
        }
        //Added by Muhammad Shaharyar for QIB
        private int ParseCassettetotal(string pCassetteDetail)
        {
            int cassetteAmount = 0;
            string[] cassetteDetails = null;

            if (pCassetteDetail.Contains("="))
            {
                cassetteDetails = pCassetteDetail.Split('=');
                cassetteAmount = int.Parse(cassetteDetails[1].Trim());
            }


            return cassetteAmount;
        }

        //Added by Muhammad Shaharyar for QIB
        private int[] ParseCassette(string pCassetteDetail)
        {

            string[] cassetteDetails = null;
            string[] seperator = { "<br>" };
            cassetteDetails = pCassetteDetail.Split(seperator, StringSplitOptions.RemoveEmptyEntries);
            int[] cassettecount = new int[cassetteDetails.Length];
            for (int i = 0; i < cassetteDetails.Length - 1; i++)
            {

                string[] temp = cassetteDetails[i].Split('*');
                cassettecount[i] = int.Parse(temp[0]);
            }

            return cassettecount;
        }

        protected void Button_Reset_Click(object sender, EventArgs e)
        {
            TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
        

        }
        
        string GetFilter()
        {
            string filter = "";
            if (TextBox_FromDate.Text.Length > 0)
                filter += " and last_bna_deposit_at >= convert(datetime,'" + TextBox_FromDate.Text + "',103)";

            if (TextBox_ToDate.Text.Length > 0)
                filter += " and last_bna_deposit_at<= convert(datetime,'" + TextBox_ToDate.Text+"',103)";
            
            return filter;
        }
        protected void button_search_Click(object sender, EventArgs e)
        {
            try
            {
                BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
            }
            catch (Exception ex)
            {
                Literal_error.Text = ex.Message;
            }

        }
        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            button_search_Click(this, null);
        }

        
    }
}