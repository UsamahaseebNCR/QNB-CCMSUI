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
    public partial class Dashboard_WorstPerforminigDeviceDashboard : System.Web.UI.Page
    {

        string jpsDBName = System.Web.Configuration.WebConfigurationManager.AppSettings["jpsDBName"];
        static string ccmsDBName = System.Web.Configuration.WebConfigurationManager.AppSettings["ccmsDBName"];

        string networkSQL = @"select * from [tbl_network] inner join tbl_devices on tbl_network.host = tbl_devices.hostname 
                              where INSERT_TIME = (select MAX(INSERT_TIME) from [tbl_network] where objects like 'LINK%')
                              and STATUS = 'DOWN' --and tbl_network.host in (select title from " + ccmsDBName + "..atm where atm_id in ({0}))";

        string powerSQL = @"select * from [tbl_power] inner join tbl_devices on tbl_power.host = tbl_devices.hostname 
                             where INSERT_TIME = (select MAX(INSERT_TIME) from [tbl_power] where objects like 'IO 1%')
                             and STATUS = 'DOWN' --and tbl_power.host in (select title from " + ccmsDBName + "..atm where atm_id in ({0}))";

        string networkHourlySQL = @"select datepart(hh,insert_time),COUNT(*)  from [tbl_network]
                                    where INSERT_TIME >= convert(datetime,convert(varchar,GETDATE(),103),103)
                                    and INSERT_TIME <= convert(datetime,convert(varchar,GETDATE(),103)+ ' 23:59:59',103)
                                    group by  datepart(hh,insert_time) order by datepart(hh,insert_time)";

        string powerHourlySQL = @"select datepart(hh,insert_time),COUNT(*)  from [tbl_power]
                                    where INSERT_TIME >= convert(datetime,convert(varchar,GETDATE(),103),103)
                                    and INSERT_TIME <= convert(datetime,convert(varchar,GETDATE(),103)+ ' 23:59:59',103)
                                    group by  datepart(hh,insert_time) order by datepart(hh,insert_time)";

       

        string temperatureSQL = @"select * from [tbl_environment] inner join tbl_devices on tbl_environment.host = tbl_devices.hostname 
                                  where INSERT_TIME = (select MAX(INSERT_TIME) from [tbl_environment] where objects like 'temp%')
                                  and READING >= UPPERVAL";

        string humiditySQL = @" select * from [tbl_environment] inner join tbl_devices on tbl_environment.host = tbl_devices.hostname 
                                where INSERT_TIME = (select MAX(INSERT_TIME) from [tbl_environment] where objects like 'HUMIDITY%')
                                and READING >= UPPERVAL";

        
        string generatorSQL = @" select count(*) from [tbl_environment]
                                where INSERT_TIME = (select MAX(INSERT_TIME) from [tbl_environment] where objects like 'gen%')
                                and READING >= UPPERVAL";

        string cameraSQL = @" select count(*) from [tbl_environment]
                                where INSERT_TIME = (select MAX(INSERT_TIME) from [tbl_environment] where objects like 'cam%')
                                and READING >= UPPERVAL";

        string solarSQL = @" select count(*) from [tbl_environment]
                                where INSERT_TIME = (select MAX(INSERT_TIME) from [tbl_environment] where objects like 'sol%')
                                and READING > UPPERVAL";

        private void DrawGoogleMap()
        {
            DataTable dt = GetDeviceDetail();
            StringBuilder jsStr = new StringBuilder();
            jsStr.Append("<script>var ary = [");
            
            foreach (DataRow dr in dt.Rows)
                jsStr.Append("['" + dr["lat"] + "','" + dr["lng"] + "','" + dr["hostname"] + "'],");
            jsStr[jsStr.Length - 1] = ']';
            jsStr.Append("</script>");
            if (dt.Rows.Count == 0)
                literal_jsArray.Text = "<script>var ary = new Array();</script>";
            else
                literal_jsArray.Text = jsStr.ToString();
        }


        private DataTable GetDeviceDetail()
        {
            SqlConnection conn = ConnectionFactory.GetNewConnection();
            DataTable dt = new DataTable();
            try
            {
                conn.Open();
                conn.ChangeDatabase(jpsDBName);
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "select * from tbl_devices";
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(dt);
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
            return dt;
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

        private void GetNetworkPowerDownCount(ref int powerDown, ref int networkDown, ref string powerDownTitle, ref string networkDownTitle)
        {
            int machineDownOnNetwork = 0;
            int machineDownOnPower = 0;
            SqlConnection conn = ConnectionFactory.GetNewConnection();
            try
            {
                conn.Open();
                conn.ChangeDatabase(jpsDBName);
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = string.Format(networkSQL, Session[SessionVars.SelectedATMs.ToString()]);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dtNetwork = new DataTable();
                adapter.Fill(dtNetwork);

                cmd.CommandText = string.Format(powerSQL, Session[SessionVars.SelectedATMs.ToString()]);
                adapter = new SqlDataAdapter(cmd);
                DataTable dtPower = new DataTable();
                adapter.Fill(dtPower);

                if (dtNetwork.Rows.Count == 0 && dtPower.Rows.Count == 0)
                {
                    networkDown = powerDown = 0;
                }
                else
                {
                    DataTable dtNetworkHost = dtNetwork.DefaultView.ToTable(true, new string[] { "host" });
                    DataTable dtPowerHost = dtPower.DefaultView.ToTable(true, new string[] { "host" });

                    if (dtPower.Rows.Count == 0)
                    {
                        machineDownOnNetwork = dtNetworkHost.Rows.Count;
                        foreach(DataRow dr in dtNetwork.Rows)
                            networkDownTitle += dr["host"] + ","+dr["location"]+";";
                    }
                    else if (dtNetwork.Rows.Count == 0)
                    {
                        machineDownOnPower = dtPowerHost.Rows.Count;
                        foreach (DataRow dr in dtPowerHost.Rows)
                            powerDownTitle += dr["host"] + "," + dr["location"] + ";"; 
                    }
                    else
                    {
                        for (int i = 0; i < dtNetworkHost.Rows.Count; i++)
                        {
                            DataRow[] drNetworkHost = dtNetwork.Select("host='" + dtNetworkHost.Rows[i]["host"]+"'");
                            DataRow[] drPowerHost = dtPower.Select("host='" + dtNetworkHost.Rows[i]["host"]+"'");

                            if (drPowerHost.Length > 0 && drNetworkHost.Length > 0)
                            {
                                if (DateTime.Parse(drPowerHost[0]["insert_time"].ToString()) > DateTime.Parse(drNetworkHost[0]["insert_time"].ToString()))
                                {
                                    machineDownOnPower++;
                                    powerDownTitle += drPowerHost[0]["host"] + "," + drPowerHost[0]["location"] + ";"; 
                                }
                                else
                                {
                                    machineDownOnNetwork++;
                                    networkDownTitle += drNetworkHost[0]["host"] + "," + drNetworkHost[0]["location"] + ";"; 
                                }
                            }
                            else if (drPowerHost.Length == 0)
                            {
                                machineDownOnNetwork++;
                                networkDownTitle += drNetworkHost[0]["host"] + "," + drNetworkHost[0]["location"] + ";"; 
                            }
                            else if (drNetworkHost.Length == 0)
                            {
                                machineDownOnPower++;
                                powerDownTitle += drPowerHost[0]["host"] + "," + drPowerHost[0]["location"] + ";"; 
                            }
                        }
                    }
                    networkDown = machineDownOnNetwork;
                    powerDown = machineDownOnPower;
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
        }
        private int GetATMsExceedTempLimit()
        {
            int result = 0;
            StringBuilder builder = new StringBuilder();
            SqlConnection conn = ConnectionFactory.GetNewConnection();
            try
            {
                conn.Open();
                conn.ChangeDatabase(jpsDBName);
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = temperatureSQL;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                foreach (DataRow dr in dt.Rows)
                    builder.Append(dr["host"] + "," + dr["location"] + ";");
                ViewState["temperatureTitles"] = builder.ToString();
                result = dt.Rows.Count;
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


        private int GetATMsExceedHumidityLimit()
        {
            int result = 0;
            StringBuilder builder = new StringBuilder();
            SqlConnection conn = ConnectionFactory.GetNewConnection();
            try
            {
                conn.Open();
                conn.ChangeDatabase(jpsDBName);
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = humiditySQL;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                foreach (DataRow dr in dt.Rows)
                    builder.Append(dr["host"] + "," + dr["location"] + ";");
                ViewState["humidityTitles"] = builder.ToString();
                result = dt.Rows.Count;
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


        private int GetATMsOnGenerator()
        {
            int result = 0;
            SqlConnection conn = ConnectionFactory.GetNewConnection();
            try
            {
                conn.Open();
                conn.ChangeDatabase(jpsDBName);
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = generatorSQL;
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

        private int GetATMsOnSolar()
        {
            int result = 0;
            SqlConnection conn = ConnectionFactory.GetNewConnection();
            try
            {
                conn.Open();
                conn.ChangeDatabase(jpsDBName);
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = solarSQL;
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

        private int GetATMsCameraDisconnected()
        {
            int result = 0;
            SqlConnection conn = ConnectionFactory.GetNewConnection();
            try
            {
                conn.Open();
                conn.ChangeDatabase(jpsDBName);
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = cameraSQL;
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
                //if (DialogCallBackCustomerDetail.CausedCallback)
                //    return;
                string powerDownTitles = "";
                string networkDownTitles = "";
                int powerDownCount = 0;
                int networkDownCount = 0;
                int totalDevices = GetTotalDevicesCount();
                GetNetworkPowerDownCount(ref powerDownCount, ref networkDownCount, ref powerDownTitles, ref networkDownTitles);
                ViewState["powerDownTitles"] = powerDownTitles;
                ViewState["networkDownTitles"] = networkDownTitles;

                //Label_Gen.Text = GetATMsOnGenerator().ToString();
                //Label_Cam.Text = GetATMsCameraDisconnected().ToString();
                //Label_Solar.Text = GetATMsOnSolar().ToString();
                //Label_PowerDown.Text = powerDownCount.ToString();
                //Label_CommsDown.Text = networkDownCount.ToString();
                //Label_TotalDevices.Text = totalDevices.ToString();
                //Label_Humidity.Text = GetATMsExceedHumidityLimit().ToString();
                //Label_Temp.Text = GetATMsExceedTempLimit().ToString();
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
                Chart_Link1.Series[0].Points.AddXY("Up", Math.Round(100 - ((double)networkDownCount / totalDevices) * 100, 2));
                Chart_Link1.Series[0].Points.AddXY("Down", Math.Round(((double)networkDownCount / totalDevices) * 100, 2));
                Chart_Link1.Series[0].Points[0].Color = System.Drawing.Color.Green;
                Chart_Link1.Series[0].Points[1].Color = System.Drawing.Color.Red;
                Chart_Link1.Series[0]["DoughnutRadius"] = "70";
                Chart_Link1.Series["Default"].Points[1]["Exploded"] = "true";
                Chart_Link1.Series[0]["PieDrawingStyle"] = "SoftEdge";
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                Chart_Link2.Series[0].Points.AddXY("Up", Math.Round(100 - ((double)powerDownCount / totalDevices) * 100, 2));
                Chart_Link2.Series[0].Points.AddXY("Down", Math.Round(((double)powerDownCount / totalDevices) * 100, 2));
                Chart_Link2.Series[0].Points[0].Color = System.Drawing.Color.Green;
                Chart_Link2.Series[0].Points[1].Color = System.Drawing.Color.Red;
                Chart_Link2.Series[0]["DoughnutRadius"] = "70";
                Chart_Link2.Series["Default"].Points[1]["Exploded"] = "true";
                Chart_Link2.Series[0]["PieDrawingStyle"] = "SoftEdge";

                //DrawPowerHourlyAnalysisGraph();
                //DrawNetworkHourlyAnalysisGraph();


                Chart_IO1.Series[0].Points.AddXY("Up", Math.Round(100 - ((double)networkDownCount / totalDevices) * 100, 2));
                Chart_IO1.Series[0].Points.AddXY("Down", Math.Round(((double)networkDownCount / totalDevices) * 100, 2));
                Chart_IO1.Series[0].Points[0].Color = System.Drawing.Color.Green;
                Chart_IO1.Series[0].Points[1].Color = System.Drawing.Color.Red;
                Chart_IO1.Series[0]["DoughnutRadius"] = "70";
                Chart_IO1.Series["Default"].Points[1]["Exploded"] = "true";
                Chart_IO1.Series[0]["PieDrawingStyle"] = "SoftEdge";
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                Chart_IO2.Series[0].Points.AddXY("Up", Math.Round(100 - ((double)powerDownCount / totalDevices) * 100, 2));
                Chart_IO2.Series[0].Points.AddXY("Down", Math.Round(((double)powerDownCount / totalDevices) * 100, 2));
                Chart_IO2.Series[0].Points[0].Color = System.Drawing.Color.Green;
                Chart_IO2.Series[0].Points[1].Color = System.Drawing.Color.Red;
                Chart_IO2.Series[0]["DoughnutRadius"] = "70";
                Chart_IO2.Series["Default"].Points[1]["Exploded"] = "true";
                Chart_IO2.Series[0]["PieDrawingStyle"] = "SoftEdge";
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
             
            
            
            }




        }
        protected void DialogCallBackDetail_Callback(object sender, CallBackEventArgs e)
        {
            StringBuilder builder = new StringBuilder();
            int type = Convert.ToInt32(e.Parameters[1]);
            if (type == 1)
            {
                builder.Append("    <table class=\"EU_DataTable\"><tbody><tr><th scope=\"col\">ATM</th><th scope=\"col\">Location</th></tr>");
                DataTable dt = GetDeviceDetail();
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