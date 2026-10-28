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
using System.Reflection;
using System.Diagnostics;

namespace CCMSUI.CCMS
{
    public partial class CardCaptured : BasePage
    {
        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!CheckRight((int)Permissions.ViewCardCaptured))
                    Response.Redirect("../admin/AccessDenied.aspx");

                TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                ViewState["sortDirection"] = " asc";
                ViewState["SortExpression"] = "title";
                BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
            }

        }
        private string buildSearchCriteria()
        {
            StringBuilder builder = new StringBuilder();
            string selectQuery = String.Empty;
            builder.Append(" AND");

            if (TextBox_FromDate.Text.Length > 0)
                builder.Append(" capture_time >= Convert(datetime,'" + TextBox_FromDate.Text + "',103) AND");

            if (TextBox_ToDate.Text.Length > 0)
                builder.Append(@"  capture_time <=Convert(datetime,'" + TextBox_ToDate.Text + " 23:59:59',103) AND");

            if (TextBox_TSN.Text.Length > 0)
                builder.Append(" tsn = '" + TextBox_TSN.Text + "' AND");

            if (TextBox_PAN.Text.Length > 0)
                builder.Append(" PAN like '%" + TextBox_PAN.Text + "%' AND");

            selectQuery = builder.ToString();
            selectQuery = selectQuery.Trim();
            if (selectQuery.EndsWith("AND"))
                selectQuery = selectQuery.Substring(0, selectQuery.LastIndexOf("AND"));
            return selectQuery;
        }

        void BindGrid(string orderBy)
        {
            LogableTask task = LogableTask.NewTask("BindGrid");
            SqlCommand cmd = ConnectionFactory.GetNewCommand(false);
            try
            {
                if (TextBox_PAN.Text.Length > 0)
                    if (TextBox_PAN.Text.Length < 4)
                        throw new Exception("Please enter at least 4 characters for PAN search."); 
                string ATMIds = Convert.ToString(Session[SessionVars.SelectedATMs.ToString()]);

                cmd.CommandTimeout = 100;
                cmd.CommandText = @"select top 1000 pan, capture_time, tsn,atm.title,location,reason
                                    from ATM inner join ej_captured_card ON ATM.ATM_ID = ej_captured_card.ATM_ID 
                                     where ATM.atm_id in ("
                    + ATMIds + ") " + buildSearchCriteria() + " Order by " + orderBy;


                DataTable dt = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
                GridView_CapturedCard.DataSource = dt;
                GridView_CapturedCard.DataBind();
                Literal_count.Text = dt.Rows.Count + " records.";
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_error.Text = "<li>An Error occured. " + ex.Message + "</li>";
            }
            finally
            {
                task.EndTask();
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
               ValidateInput();
               
                   BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
               
            }
            catch (Exception ex)
            {
                Literal_error.Text = ex.Message;
            }
            
        }


        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            btnSearch_Click(this, null);
        }

        protected void Button_reset_Click(object sender, EventArgs e)
        {
            TextBox_FromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            TextBox_ToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            TextBox_TSN.Text = "";
            TextBox_PAN.Text = "";
        }


        protected void GridView_CapturedCard_Sorting(object sender, GridViewSortEventArgs e)
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
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }




        protected void GridView_CapturedCard_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.GridView_CapturedCard.PageIndex = e.NewPageIndex;
            BindGrid(ViewState["SortExpression"].ToString() + ViewState["sortDirection"].ToString());
        }


        public void ValidateInput()
        {
            
            bool isTSNContainSpecialChars = CheckSpecialChars(TextBox_TSN.Text);
            if (isTSNContainSpecialChars == true)
            {
                throw new Exception("TSN contain invalid character");
            
            }
            bool isPANContainSpecialChars = CheckSpecialChars(TextBox_PAN.Text);
            if (isPANContainSpecialChars == true)
            {
                throw new Exception("PAN contain invalid character");
                
            }
            
        }
    }
}