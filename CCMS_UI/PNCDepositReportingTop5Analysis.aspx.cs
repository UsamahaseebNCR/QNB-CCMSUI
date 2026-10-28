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
using Avanza.iSuite.UI;
using Avanza.CCMS.DAL;

using System.Data.SqlClient;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using System.Web.UI.DataVisualization.Charting;

namespace CCMSUI
{
    public partial class PNCDepositReportingTop5Analysis : BasePage
    {
        private string Filter()
        {
            string criteria = null;

            if (CheckBoxList_ErrorAnalysisType.Items[0].Selected && CheckBoxList_ErrorAnalysisType.Items[1].Selected)
                criteria = "";
            else if (CheckBoxList_ErrorAnalysisType.Items[0].Selected)
                criteria = " and dispute_status is not null ";
            else if (CheckBoxList_ErrorAnalysisType.Items[1].Selected)
                criteria = " and (dispute_status is null or dispute_status = '000') ";

            if (CheckBoxList_Module.Items[0].Selected && CheckBoxList_Module.Items[1].Selected)
            {
            }
            else if (CheckBoxList_Module.Items[0].Selected)
                criteria += " and is_sdm = 1 ";
            else if (CheckBoxList_Module.Items[1].Selected)
                criteria += " and is_cdm = 1 ";

            criteria += " and status = 'Failed' and is_eligible=1";
            return criteria;
        }
        protected void Page_PreInit(object sender, EventArgs e)
        {
            this.refresh.ServerClick += new System.EventHandler(this.refresh_ServerClick);
        }
        protected void refresh_ServerClick(object sender, EventArgs e)
        {
            //string regionID = (string)Session[SessionVars.SelectedRegions.ToString()];
            //if (regionID != null /*&& !regionID.Contains(",")*/
            //    && regionID != "1")
            //{
            //    DropDownList_NoteSetType.Items.Clear();
            //    Avanza.CCMS.DAL.NoteSetType.NoteSetTypeReader reader =
            //    Avanza.CCMS.DAL.NoteSetType.ExecuteReader("region_id=" + AddEditATM.GetOrganization(regionID.Contains(",") ? int.Parse(regionID.Substring(0, regionID.IndexOf(","))) : int.Parse(regionID)));
            //    while (reader.Read())
            //    {
            //        DropDownList_NoteSetType.Items.Add(new ListItem(reader.CurrentNoteSetType.NoteSetTypeName, reader.CurrentNoteSetType.NoteSetTypeId.ToString()));
            //    }
            //    reader.Close();

            //}
            //else
            //    Literal_Message.Text = "<span style='color:red'><b>Please select only one organization to do denomination analysis</b></span>";

        }

        protected void Button_Submit_Click(object sender, EventArgs e)
        {
            DrawCashAcceptorGraph();
            DrawCheckAcceptorGraph();
            
            //DrawBusinessRuleRejectCausesGraph();
            //DrawNonBusinessRuleRejectCausesGraph();
            //DrawCheckRejectRateGraph();
        }

        private void DrawCashAcceptorGraph()
        {
            string SQL = "select top 5 comment,count(*) from vEjParsedBNATransaction inner join atm on atm.atm_id =vEjParsedBNATransaction.atm_id " +
                       " where atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and trxn_datetime>=convert(datetime,'" + this.TextBox_From.Text + "',103)  " +
                       " and trxn_datetime<=convert(datetime,'" + this.TextBox_To.Text + " 23:59:59',103) " +Filter()+
                       " and comment is not null and len(comment)>1 group by comment order by count(*) desc";
            DataTable dt = GetDataTable(SQL);

            foreach (DataRow dr in dt.Rows)
                Chart_Top5CashAcceptorIncident.Series[0].Points.AddXY(dr[0], dr[1]);

            Chart_Top5CashAcceptorIncident.Titles[0].Text = "TOP 5 INCIDENTS RELATED TO CASH ACCEPTOR";
            Chart_Top5CashAcceptorIncident.Series[0]["PieDrawingStyle"] = "SoftEdge";


            Chart_Top5CashAcceptorIncident.Series["Default"]["ShowMarkerLines"] = "true";
            Chart_Top5CashAcceptorIncident.Series["Default"]["DrawingStyle"] = "Cylinder";
            Chart_Top5CashAcceptorIncident.Series["Default"]["PointWidth"] = "0.8";


        }
        private void DrawCheckAcceptorGraph()
        {
            string SQL = "select top 5 comment,count(*) from ej_parsed_cpm_transaction inner join atm on atm.atm_id =ej_parsed_cpm_transaction.atm_id  " +
                       " where atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ") and trxn_datetime>=convert(datetime,'" + this.TextBox_From.Text + "',103)  " +
                       " and trxn_datetime<=convert(datetime,'" + this.TextBox_To.Text + " 23:59:59',103) " +Filter()+
                       " and comment is not null and len(comment)>1 group by comment order by count(*) desc";
            DataTable dt = GetDataTable(SQL);

            foreach (DataRow dr in dt.Rows)
                Chart_Top5CheckAcceptorIncident.Series[0].Points.AddXY(dr[0], dr[1]);

            Chart_Top5CheckAcceptorIncident.Titles[0].Text = "TOP 5 INCIDENTS RELATED TO CHECK ACCEPTOR";
            Chart_Top5CheckAcceptorIncident.Series[0]["PieDrawingStyle"] = "SoftEdge";

            Chart_Top5CheckAcceptorIncident.Series["Default"]["ShowMarkerLines"] = "true";
            Chart_Top5CheckAcceptorIncident.Series["Default"]["DrawingStyle"] = "Cylinder";
            Chart_Top5CheckAcceptorIncident.Series["Default"]["PointWidth"] = "0.8";

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
            if (!base.IsPostBack)
            {
                this.TextBox_To.Text = DateTime.Now.ToString("dd/MM/yyyy");
                this.TextBox_From.Text = DateTime.Now.AddDays(-7.0).ToString("dd/MM/yyyy");
                Button1.Attributes.Add("onclick", " this.disabled = true; " + ClientScript.GetPostBackEventReference(Button1, null) + ";");
            }
        }
    }


}