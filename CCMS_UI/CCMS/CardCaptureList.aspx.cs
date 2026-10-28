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
using System.Text;
using Avanza.iSuite.UI;
using Avanza.iSuite.DAL;
using System.Data.SqlClient;
using System.Reflection;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace CCMSUI.CCMS
{
    public partial class CardCaptureList : BasePage
    {
        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!CheckRight((int)Permissions.CardCaptureList))
                    Response.Redirect("../admin/AccessDenied.aspx");

                txtFromDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                txtToDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                DropDownList_ATMType.Items.Add(new ListItem("Any", "Any"));
                AddEditATM.PopulateATMTypes(DropDownList_ATMType);

                SqlCommand cmd = ConnectionFactory.GetNewCommand(true);
                cmd.CommandText = @"select distinct capture_reason from captured_card order by capture_reason";
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.SelectCommand = cmd;
                DataTable dtReason = new DataTable();
                adapter.Fill(dtReason);
                ddlCaptureReason.DataValueField = "capture_reason";
                ddlCaptureReason.DataTextField = "capture_reason";
                dtReason.Rows.Add("Any");
                ddlCaptureReason.DataSource = dtReason;
                ddlCaptureReason.DataBind();
                ddlCaptureReason.Text = "Any";

                cmd.CommandText = @"select distinct pan_prefix from fitmapping order by pan_prefix";
                DataTable dtPANPrefix = new DataTable();
                adapter.Fill(dtPANPrefix);
                ddlPANPrefix.DataValueField = "pan_prefix";
                ddlPANPrefix.DataTextField = "pan_prefix";
                dtPANPrefix.Rows.Add("Any");
                ddlPANPrefix.DataSource = dtPANPrefix;
                ddlPANPrefix.DataBind();
                ddlPANPrefix.Text = "Any";

                cmd.CommandText = @"select distinct bank_name from bankmapping order by bank_name";
                DataTable dtBank = new DataTable();
                adapter.Fill(dtBank);
                ddlBankName.DataValueField = "bank_name";
                ddlBankName.DataTextField = "bank_name";
                dtBank.Rows.Add("Any");
                ddlBankName.DataSource = dtBank;
                ddlBankName.DataBind();
                ddlBankName.Text = "Any";

                cmd.CommandText = @"select distinct switch_name from switchmapping order by switch_name";
                DataTable dtSwitchName = new DataTable();
                adapter.Fill(dtSwitchName);
                ddlSwitchName.DataValueField = "switch_name";
                ddlSwitchName.DataTextField = "switch_name";
                dtSwitchName.Rows.Add("Any");
                ddlSwitchName.DataSource = dtSwitchName;
                ddlSwitchName.DataBind();
                ddlSwitchName.Text = "Any";
            }
        }
        private string buildSearchCriteria()
        {
            StringBuilder builder = new StringBuilder();
            string selectQuery = String.Empty;
            builder.Append(" AND");

            if ((txtFromDate.Text.Length > 0) && (txt_FromTime.Text.Length > 0))
                builder.Append(@" capture_time >= Convert(datetime,'" + txtFromDate.Text + " " + txt_FromTime.Text + "',103) AND ");

            if ((txtToDate.Text.Length > 0) && (txt_ToTime.Text.Length > 0))
                builder.Append(@"  capture_time <=Convert(datetime,'" + txtToDate.Text + " " + txt_ToTime.Text + "',103) AND ");

            if (txt_FromTSN.Text.Length > 0)
                builder.Append(" convert(float,TSN) >= " + txt_FromTSN.Text + " AND");

            if (txt_ToTSN.Text.Length > 0)
                builder.Append(" convert(float,TSN) <= " + txt_ToTSN.Text + " AND");

            if (txtPAN.Text.Length > 0)
                builder.Append(" PAN like '" + txtPAN.Text + "%' AND");

            if (DropDownList_ATMType.SelectedIndex > 0)
                builder.Append(" atm_type = '" + DropDownList_ATMType.SelectedValue + "' AND");
            if (ddlCaptureReason.SelectedIndex < ddlCaptureReason.Items.Count - 1)
                builder.Append(" capture_reason = '" + ddlCaptureReason.SelectedValue + "' AND");
            if (ddlPANPrefix.SelectedIndex < ddlPANPrefix.Items.Count - 1)
                builder.Append(" pan_prefix = '" + ddlPANPrefix.SelectedValue + "' AND");
            if (ddlBankName.SelectedIndex < ddlBankName.Items.Count - 1)
                builder.Append(" bank_name = '" + ddlBankName.SelectedValue + "' AND");
            if (ddlSwitchName.SelectedIndex < ddlSwitchName.Items.Count - 1)
                builder.Append(" switch_name = '" + ddlSwitchName.SelectedValue + "' AND");

            selectQuery = builder.ToString();
            selectQuery = selectQuery.Trim();
            if (selectQuery.EndsWith("AND"))
                selectQuery = selectQuery.Substring(0, selectQuery.LastIndexOf("AND"));
            return selectQuery;
        }

        void BindGrid()
        {
            LogableTask task = LogableTask.NewTask("BindGrid");
            SqlCommand cmd = ConnectionFactory.GetNewCommand(true);
            try
            {
                string ATMIds = Convert.ToString(Session[SessionVars.SelectedATMs.ToString()]);

                cmd.CommandTimeout = 100;
                cmd.CommandText = @"select top 1000 pan, capture_time, tsn,atm.title, atm_type,capture_reason,pan_prefix,bank_name,switch_name
                                    from ATM inner join captured_card ON ATM.ATM_ID = captured_card.ATM_ID 
                                    left join fitmapping on fitmapping.pan_prefix=substring(pan,1,6) 
                                    left join bankmapping on fitmapping.bank_id=bankmapping.bank_id
                                    left join switchmapping on bankmapping.switch_id=switchmapping.switch_id
                                    
                                    where ATM.atm_id in ("
                    + ATMIds + ") " + buildSearchCriteria() + " Order by capture_time desc";

                if (Session["SearchResult"] != null)
                    ((DataTable)Session["SearchResult"]).Dispose();

                DataTable dt = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
                GridCapturedCardList.CurrentPageIndex = 0;
                GridCapturedCardList.DataSource = dt;
                Session["SearchResult"] = dt;
                GridCapturedCardList.DataBind();
                Literal_count.Text = dt.Rows.Count + " records.";
            }
            catch (Exception ex)
            {
                task.Log(MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
                Literal_errorMsg.Text = "<li>An Error occured. " + ex.Message + "</li>";
            }
            finally
            {
                task.EndTask();
            }
        }
       
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if (!Utility.isSqlInjectionFree(txt_FromTime.Text + txt_FromTSN.Text+txt_ToTime.Text+txt_ToTSN.Text+txtFromDate.Text+txtPAN.Text+txtToDate.Text))
                Literal_errorMsg.Text = "Error: Invalid characters entered.";
            else
            BindGrid();
        }


        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            btnSearch_Click(this, null);
        }

    }
}