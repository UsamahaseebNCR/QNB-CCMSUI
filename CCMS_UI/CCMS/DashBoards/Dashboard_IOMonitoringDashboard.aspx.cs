using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Avanza.iSuite.DAL;
using System.Data.SqlClient;
using System.Data;
using ComponentArt.Web.UI;
using System.Text;

namespace CCMSUI.CCMS
{
    public partial class Dashboard_IOMonitoringDashboard : System.Web.UI.Page
    {

        string jpsDBName = System.Web.Configuration.WebConfigurationManager.AppSettings["jpsDBName"];
        static string ccmsDBName = System.Web.Configuration.WebConfigurationManager.AppSettings["ccmsDBName"];

        string Link1SQL = @"select * from tbl_network
                            where OBJECTS = 'LINK 1'  and INSERT_TIME in 
                            (select MAX(INSERT_TIME) from tbl_network where OBJECTS = 'LINK 1' and tbl_network.host in (select title from " + ccmsDBName + "..atm where atm_id in ({0})) ) " +
                            " and tbl_network.host in (select title from " + ccmsDBName + "..atm where atm_id in ({0})) ";

        
        string Link2SQL = @"select * from tbl_network
                              where OBJECTS = 'LINK 2' And INSERT_TIME in 
                              (select MAX(INSERT_TIME) from tbl_network where OBJECTS = 'LINK 2' and tbl_network.host in (select title from " + ccmsDBName + "..atm where atm_id in ({0}))) " + 
                            " and tbl_network.host in (select title from " + ccmsDBName + "..atm where atm_id in ({0}))";

        string pinger1SQL = @"select * from tbl_network
                            where OBJECTS = 'PINGER 1'  and INSERT_TIME in 
                            (select MAX(INSERT_TIME) from tbl_network where OBJECTS = 'PINGER 1' and tbl_network.host in (select title from " + ccmsDBName + "..atm where atm_id in ({0}))) " +
                            " and tbl_network.host in (select title from " + ccmsDBName + "..atm where atm_id in ({0}))";

        string pinger2SQL = @"select * from tbl_network
                              where OBJECTS = 'PINGER 2'  and INSERT_TIME in 
                              (select MAX(INSERT_TIME) from tbl_network where OBJECTS = 'PINGER 2' and tbl_network.host in (select title from " + ccmsDBName + "..atm where atm_id in ({0})) ) "+
                             " and tbl_network.host in (select title from " + ccmsDBName + "..atm where atm_id in ({0}))";



        string IO1 = @"select * from tbl_power
                        where OBJECTS = 'IO 1'  and INSERT_TIME in 
                        (select MAX(INSERT_TIME) from tbl_power where OBJECTS = 'IO 1' and tbl_network.host in (select title from " + ccmsDBName + "..atm where atm_id in ({0}))) " +
                        " and tbl_power.host in (select title from " + ccmsDBName + "..atm where atm_id in ({0}))";

        string IO2 = @"
                        select * from tbl_power
                        where OBJECTS = 'IO 2'  and INSERT_TIME in 
                        (select MAX(INSERT_TIME) from tbl_power where OBJECTS = 'IO 2' and tbl_network.host in (select title from " + ccmsDBName + "..atm where atm_id in ({0}))) " +
                        " and tbl_power.host in (select title from " + ccmsDBName + "..atm where atm_id in ({0}))";

        string IO3 = @"select * from tbl_power
                        where OBJECTS = 'IO 3'  and INSERT_TIME in 
                        (select MAX(INSERT_TIME) from tbl_power where OBJECTS = 'IO 3' and tbl_network.host in (select title from " + ccmsDBName + "..atm where atm_id in ({0}))) " +
                        " and tbl_power.host in (select title from " + ccmsDBName + "..atm where atm_id in ({0}))";

        string device = @"select * from tbl_power
                        where OBJECTS = 'DEVICE'  and INSERT_TIME in 
                        (select MAX(INSERT_TIME) from tbl_power where OBJECTS = 'DEVICE' and tbl_network.host in (select title from " + ccmsDBName + "..atm where atm_id in ({0}))) " +
                        " and tbl_power.host in (select title from " + ccmsDBName + "..atm where atm_id in ({0}))";


       private int GetCountForDownEvents(string _sql)
        {
            int result = 0;
            SqlConnection conn = ConnectionFactory.GetNewConnection();
            try
            {
                conn.Open();
                conn.ChangeDatabase(jpsDBName);
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = string.Format(_sql, Session[SessionVars.SelectedATMs.ToString()]); 
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["status"].ToString() == "DOWN" || dr["status"].ToString() == "FAILED")
                        result++;
                }
                
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
            return result;
        }

       private int GetTotalDevicesCount()
       {
           int result = 0;
           SqlConnection conn = ConnectionFactory.GetNewConnection();
           try
           {
               conn.Open();
               conn.ChangeDatabase(jpsDBName);
               SqlCommand cmd = conn.CreateCommand();
               cmd.CommandText = "select count(*) from tbl_devices";
               result = (int)cmd.ExecuteScalar();
           }
           catch (Exception ex)
           {
           }
           finally
           {
               if (conn != null)
                   conn.Close();
           }
           return result;
       }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                int totalDevices = GetTotalDevicesCount();
                
                int link1DownCount = GetCountForDownEvents(Link1SQL);
                int link2DownCount = GetCountForDownEvents(Link2SQL);
                int pinger1DownCount = GetCountForDownEvents(pinger1SQL); 
                int pinger2DownCount = GetCountForDownEvents(pinger2SQL);

                int IO1DownCount = GetCountForDownEvents(IO1);
                int IO2DownCount = GetCountForDownEvents(IO2);
                int IO3DownCount = GetCountForDownEvents(IO3);
                int DeviceDownCount = GetCountForDownEvents(device);


                //if (DialogCallBackCustomerDetail.CausedCallback)
                //    return;
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
                Chart_Link1.Series[0].Points.AddXY("Up", Math.Round(100 - ((double)link1DownCount / totalDevices) * 100, 2));
                Chart_Link1.Series[0].Points.AddXY("Down", Math.Round(((double)link1DownCount / totalDevices) * 100, 2));
                Chart_Link1.Series[0].Points[0].Color = System.Drawing.ColorTranslator.FromHtml("#817598");
                Chart_Link1.Series[0].Points[1].Color = System.Drawing.ColorTranslator.FromHtml("#FF5733");
                Chart_Link1.Series[0]["DoughnutRadius"] = "70";
                Chart_Link1.Series["Default"].Points[1]["Exploded"] = "true";
                Chart_Link1.Series[0]["PieDrawingStyle"] = "SoftEdge";
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                Chart_Link2.Series[0].Points.AddXY("Up", Math.Round(100 - ((double)link2DownCount / totalDevices) * 100, 2));
                Chart_Link2.Series[0].Points.AddXY("Down", Math.Round(((double)link2DownCount / totalDevices) * 100, 2));
                Chart_Link2.Series[0].Points[0].Color = System.Drawing.ColorTranslator.FromHtml("#9E72F5");
                Chart_Link2.Series[0].Points[1].Color = System.Drawing.ColorTranslator.FromHtml("#FF5733");
                Chart_Link2.Series[0]["DoughnutRadius"] = "70";
                Chart_Link2.Series["Default"].Points[1]["Exploded"] = "true";
                Chart_Link2.Series[0]["PieDrawingStyle"] = "SoftEdge";

                //DrawPowerHourlyAnalysisGraph();
                //DrawNetworkHourlyAnalysisGraph();

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                Chart_Pinger1.Series[0].Points.AddXY("Up", Math.Round(100 - ((double)pinger1DownCount / totalDevices) * 100, 2));
                Chart_Pinger1.Series[0].Points.AddXY("Down", Math.Round(((double)pinger1DownCount / totalDevices) * 100, 2));
                Chart_Pinger1.Series[0].Points[0].Color = System.Drawing.ColorTranslator.FromHtml("#B8ABD3");
                Chart_Pinger1.Series[0].Points[1].Color = System.Drawing.ColorTranslator.FromHtml("#FF5733");
                Chart_Pinger1.Series[0]["DoughnutRadius"] = "70";
                Chart_Pinger1.Series["Default"].Points[1]["Exploded"] = "true";
                Chart_Pinger1.Series[0]["PieDrawingStyle"] = "SoftEdge";

                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                Chart_Pinger2.Series[0].Points.AddXY("Up", Math.Round(100 - ((double)pinger2DownCount / totalDevices) * 100, 2));
                Chart_Pinger2.Series[0].Points.AddXY("Down", Math.Round(((double)pinger2DownCount / totalDevices) * 100, 2));
                Chart_Pinger2.Series[0].Points[0].Color = System.Drawing.ColorTranslator.FromHtml("#BD9FF9  ");
                Chart_Pinger2.Series[0].Points[1].Color = System.Drawing.ColorTranslator.FromHtml("#FF5733");
                Chart_Pinger2.Series[0]["DoughnutRadius"] = "70";
                Chart_Pinger2.Series["Default"].Points[1]["Exploded"] = "true";
                Chart_Pinger2.Series[0]["PieDrawingStyle"] = "SoftEdge";
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //All
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                Chart_Comms.Series[0].Points.AddXY("Down", Math.Round(((double)link1DownCount / totalDevices) * 100, 2));
                Chart_Comms.Series[0].Points[0].Color = System.Drawing.Color.Red;
                Chart_Comms.Series[0]["DoughnutRadius"] = "70";
                Chart_Comms.Series["Default"].Points[0]["Exploded"] = "true";
                Chart_Comms.Series[0]["PieDrawingStyle"] = "SoftEdge";
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                Chart_Comms.Series[0].Points.AddXY("Down", Math.Round(((double)link2DownCount / totalDevices) * 100, 2));
                Chart_Comms.Series[0].Points[1].Color = System.Drawing.Color.Red;
                Chart_Comms.Series[0]["DoughnutRadius"] = "70";
                Chart_Comms.Series["Default"].Points[0]["Exploded"] = "true";
                Chart_Comms.Series[0]["PieDrawingStyle"] = "SoftEdge";

                //DrawPowerHourlyAnalysisGraph();
                //DrawNetworkHourlyAnalysisGraph();

                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                Chart_Comms.Series[0].Points.AddXY("Down", Math.Round(((double)pinger1DownCount / totalDevices) * 100, 2));
                Chart_Comms.Series[0].Points[2].Color = System.Drawing.Color.Red;
                Chart_Comms.Series[0]["DoughnutRadius"] = "70";
                Chart_Comms.Series["Default"].Points[0]["Exploded"] = "true";
                Chart_Comms.Series[0]["PieDrawingStyle"] = "SoftEdge";

                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                Chart_Comms.Series[0].Points.AddXY("Down", Math.Round(((double)pinger2DownCount / totalDevices) * 100, 2));
                Chart_Comms.Series[0].Points[3].Color = System.Drawing.Color.Red;
                Chart_Comms.Series[0]["DoughnutRadius"] = "70";
                Chart_Comms.Series["Default"].Points[0]["Exploded"] = "true";
                Chart_Comms.Series[0]["PieDrawingStyle"] = "SoftEdge";
               
                /////Device

                Chart_IO1.Series[0].Points.AddXY("Up", Math.Round(100 - ((double)IO1DownCount / totalDevices) * 100, 2));
                Chart_IO1.Series[0].Points.AddXY("Down", Math.Round(((double)IO1DownCount / totalDevices) * 100, 2));
                Chart_IO1.Series[0].Points[0].Color = System.Drawing.ColorTranslator.FromHtml("#4BEA40");
                Chart_IO1.Series[0].Points[1].Color = System.Drawing.ColorTranslator.FromHtml("#FF5733");
                Chart_IO1.Series[0]["DoughnutRadius"] = "70";
                Chart_IO1.Series["Default"].Points[1]["Exploded"] = "true";
                Chart_IO1.Series[0]["PieDrawingStyle"] = "SoftEdge";
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                Chart_IO2.Series[0].Points.AddXY("Up", Math.Round(100 - ((double)IO2DownCount / totalDevices) * 100, 2));
                Chart_IO2.Series[0].Points.AddXY("Down", Math.Round(((double)IO2DownCount / totalDevices) * 100, 2));
                Chart_IO2.Series[0].Points[0].Color = System.Drawing.ColorTranslator.FromHtml("#65CF5E");
                Chart_IO2.Series[0].Points[1].Color = System.Drawing.ColorTranslator.FromHtml("#FF5733");
                Chart_IO2.Series[0]["DoughnutRadius"] = "70";
                Chart_IO2.Series["Default"].Points[1]["Exploded"] = "true";
                Chart_IO2.Series[0]["PieDrawingStyle"] = "SoftEdge";
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                Chart_IO3.Series[0].Points.AddXY("Up", Math.Round(100 - ((double)IO3DownCount / totalDevices) * 100, 2));
                Chart_IO3.Series[0].Points.AddXY("Down", Math.Round(((double)IO3DownCount / totalDevices) * 100, 2));
                Chart_IO3.Series[0].Points[0].Color = System.Drawing.ColorTranslator.FromHtml("#DAF7A6");
                Chart_IO3.Series[0].Points[1].Color = System.Drawing.ColorTranslator.FromHtml("#FF5733");
                Chart_IO3.Series[0]["DoughnutRadius"] = "70";
                Chart_IO3.Series["Default"].Points[1]["Exploded"] = "true";
                Chart_IO3.Series[0]["PieDrawingStyle"] = "SoftEdge";
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                Chart_Device.Series[0].Points.AddXY("Up", Math.Round(100 - ((double)DeviceDownCount / totalDevices) * 100, 2));
                Chart_Device.Series[0].Points.AddXY("Down", Math.Round(((double)DeviceDownCount / totalDevices) * 100, 2));
                Chart_Device.Series[0].Points[0].Color = System.Drawing.Color.Green;
                Chart_Device.Series[0].Points[1].Color = System.Drawing.ColorTranslator.FromHtml("#FF5733");
                Chart_Device.Series[0]["DoughnutRadius"] = "70";
                Chart_Device.Series["Default"].Points[1]["Exploded"] = "true";
                Chart_Device.Series[0]["PieDrawingStyle"] = "SoftEdge";
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

               

                Chart_Power.Series[0].Points.AddXY("Down", Math.Round(((double)IO1DownCount / totalDevices) * 100, 2));
                Chart_Power.Series[0].Points[0].Color = System.Drawing.Color.Green;
                Chart_Power.Series[0]["DoughnutRadius"] = "70";
                Chart_Power.Series["Default"].Points[0]["Exploded"] = "true";
                Chart_Power.Series[0]["PieDrawingStyle"] = "SoftEdge";
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                Chart_Power.Series[0].Points.AddXY("Down", Math.Round(((double)IO2DownCount / totalDevices) * 100, 2));
                Chart_Power.Series[0].Points[0].Color = System.Drawing.Color.Orange;
                Chart_Power.Series[0]["DoughnutRadius"] = "70";
                Chart_Power.Series["Default"].Points[0]["Exploded"] = "true";
                Chart_Power.Series[0]["PieDrawingStyle"] = "SoftEdge";
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                Chart_Power.Series[0].Points.AddXY("Down", Math.Round(((double)IO3DownCount / totalDevices) * 100, 2));
                Chart_Power.Series[0].Points[0].Color = System.Drawing.Color.Yellow;
                Chart_Power.Series[0]["DoughnutRadius"] = "70";
                Chart_Power.Series["Default"].Points[0]["Exploded"] = "true";
                Chart_Power.Series[0]["PieDrawingStyle"] = "SoftEdge";
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                Chart_Power.Series[0].Points.AddXY("Down", Math.Round(((double)DeviceDownCount / totalDevices) * 100, 2));
                Chart_Power.Series[0].Points[0].Color = System.Drawing.Color.Red;
                Chart_Power.Series[0]["DoughnutRadius"] = "70";
                Chart_Power.Series["Default"].Points[0]["Exploded"] = "true";
                Chart_Power.Series[0]["PieDrawingStyle"] = "SoftEdge";
            
            }




        }
        protected void DialogCallBackDetail_Callback(object sender, CallBackEventArgs e)
        {
            StringBuilder builder = new StringBuilder();
            int type = Convert.ToInt32(e.Parameters[1]);
            if (type == 1)
            {
                builder.Append("    <table class=\"EU_DataTable\"><tbody><tr><th scope=\"col\">ATM</th><th scope=\"col\">Location</th></tr>");
                DataTable dt = null;// GetDeviceDetail();
                foreach (DataRow dr in dt.Rows)
                    builder.Append("<tr><td>" + dr["hostname"] + "</td><td>" + dr["location"] + "</td></tr>");
                builder.Append("</tbody></table>");
            }
            else if (type == 2)
            {
                builder.Append("    <table class=\"EU_DataTable\"><tbody><tr><th scope=\"col\">ATM</th><th scope=\"col\">Location</th></tr>");

                //ViewState["powerDownTitles"] = powerDownTitles;
                string[] parts = ViewState["networkDownTitles"].ToString().Split(';');
                foreach (String part in parts)
                {
                    string[] subParts = part.Split(',');
                    if (subParts.Length>1)
                    builder.Append("<tr><td>" + subParts[0] + "</td><td>"+subParts[1]+"</td></tr>");
                }
                builder.Append("</tbody></table>");
            }
            else if (type == 3)
            {
                builder.Append("    <table class=\"EU_DataTable\"><tbody><tr><th scope=\"col\">ATM</th><th scope=\"col\">Location</th></tr>");

                //ViewState["powerDownTitles"] = powerDownTitles;
                string[] parts = ViewState["powerDownTitles"].ToString().Split(';');
                foreach (String part in parts)
                {
                    string[] subParts = part.Split(',');
                    if (subParts.Length > 1)
                        builder.Append("<tr><td>" + subParts[0] + "</td><td>" + subParts[1] + "</td></tr>");
                }
                builder.Append("</tbody></table>");
            }
            else if (type == 4)
            {
                builder.Append("    <table class=\"EU_DataTable\"><tbody><tr><th scope=\"col\">ATM</th><th scope=\"col\">Location</th></tr>");

                //ViewState["powerDownTitles"] = powerDownTitles;
                string[] parts = ViewState["humidityTitles"].ToString().Split(';');
                foreach (String part in parts)
                {
                    string[] subParts = part.Split(',');
                    if (subParts.Length > 1)
                        builder.Append("<tr><td>" + subParts[0] + "</td><td>" + subParts[1] + "</td></tr>");
                }
                builder.Append("</tbody></table>");
            }
            else if (type == 5)
            {
                builder.Append("    <table class=\"EU_DataTable\"><tbody><tr><th scope=\"col\">ATM</th><th scope=\"col\">Location</th></tr>");

                //ViewState["powerDownTitles"] = powerDownTitles;
                string[] parts = ViewState["temperatureTitles"].ToString().Split(';');
                foreach (String part in parts)
                {
                    string[] subParts = part.Split(',');
                    if (subParts.Length > 1)
                        builder.Append("<tr><td>" + subParts[0] + "</td><td>" + subParts[1] + "</td></tr>");
                }
                builder.Append("</tbody></table>");
            }

            e.Output.Write("<script> receiveCallBackResult('<table border=1 width=100%>" + builder.ToString() + "</table>','2');</script>");
        }
    }
}