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
using Avanza.iSuite.UI;

namespace CCMSUI.CCMS
{
    public partial class Dashboard_PowerCommsDashboard : BasePage
    {

        string jpsDBName = System.Web.Configuration.WebConfigurationManager.AppSettings["jpsDBName"];
        static string ccmsDBName = System.Web.Configuration.WebConfigurationManager.AppSettings["ccmsDBName"];

        string networkSQLPinger1 = @"select * from [tbl_network] outerNetwork inner join tbl_devices on outerNetwork.host = tbl_devices.hostname
                              where INSERT_TIME in (select MAX(INSERT_TIME) from [tbl_network] where objects ='PINGER 1' AND host = outerNetwork.HOST ) " +
                              " and outerNetwork.host in (select title from " + ccmsDBName + "..atm where atm_id in ({0})) union all ";

        string networkSQLPinger2 = @"select * from [tbl_network] outerNetwork inner join tbl_devices on outerNetwork.host = tbl_devices.hostname
                              where INSERT_TIME in (select MAX(INSERT_TIME) from [tbl_network] where objects ='PINGER 2' AND host = outerNetwork.HOST ) " +
                              " and outerNetwork.host in (select title from " + ccmsDBName + "..atm where atm_id in ({0}))";



        string powerSQL = @"select * from [tbl_power] outerPower inner join tbl_devices on outerPower.host = tbl_devices.hostname 
                             where INSERT_TIME in (select MAX(INSERT_TIME) from [tbl_power] where objects like 'IO 1%'  and host = outerPower.host) " +
                             " and outerPower.host in (select title from " + ccmsDBName + "..atm where atm_id in ({0}))";

        string networkHourlySQL = @"select datepart(hh,down),COUNT(*)  from [vw_network_consolidated_latest]
                                    where objects in ('PINGER 1','PINGER 2') and INSERTion_date ='" + DateTime.Today.ToString("yyyy-MM-dd") + "' " +
                                    " and host in (select title from " + ccmsDBName + "..atm where atm_id in ({0})) " +
                                    " group by  datepart(hh,down) order by datepart(hh,down)";

        string powerHourlySQL = @"select datepart(hh,down),COUNT(*)  from [vw_power_consolidated_latest]
                                    where INSERTion_date ='" + DateTime.Today.ToString("yyyy-MM-dd") + "'  and objects = 'IO 1' " +
                                    " and host in (select title from " + ccmsDBName + "..atm where atm_id in ({0})) " +
                                  " group by  datepart(hh,down) order by datepart(hh,down)";



        string temperatureSQL = @"select * from [tbl_environment] inner join tbl_devices on tbl_environment.host = tbl_devices.hostname 
                                  where INSERT_TIME = (select MAX(INSERT_TIME) from [tbl_environment] where objects like 'temp%')
                                 and tbl_environment.host in (select title from " + ccmsDBName + "..atm where atm_id in ({0})) " +
                                " and READING >= UPPERVAL";

        string humiditySQL = @" select * from [tbl_environment] outerEnvironment inner join tbl_devices on outerEnvironment.host = tbl_devices.hostname 
                                where INSERT_TIME = (select MAX(INSERT_TIME) from [tbl_environment] where objects like 'HUMIDITY%' and host = outerEnvironment.host)
                                 and outerEnvironment.host in (select title from " + ccmsDBName + "..atm where atm_id in ({0})) " +
                                " and READING >= UPPERVAL";


        string generatorSQL = @" select count(*) from [tbl_environment]
                                where INSERT_TIME = (select MAX(INSERT_TIME) from [tbl_environment] where objects like 'gen%')
                                and tbl_environment.host in (select title from " + ccmsDBName + "..atm where atm_id in ({0})) " +
                               " and READING >= UPPERVAL";

        string cameraSQL = @" select count(*) from [tbl_environment]
                                where INSERT_TIME = (select MAX(INSERT_TIME) from [tbl_environment] where objects like 'cam%')
                             and tbl_environment.host in (select title from " + ccmsDBName + "..atm where atm_id in ({0})) " +
                            " and READING >= UPPERVAL";

        string solarSQL = @" select count(*) from [tbl_environment]
                                where INSERT_TIME = (select MAX(INSERT_TIME) from [tbl_environment] where objects like 'sol%')
                            and tbl_environment.host in (select title from " + ccmsDBName + "..atm where atm_id in ({0})) " +
                            " and READING > UPPERVAL";

        private void DrawGoogleMap(string atmTitles, bool isDisplayingDownAtms)
        {
            string[] parts = null;
            if (atmTitles != null)
                parts = atmTitles.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

            DataTable dt = GetDeviceDetail();
            StringBuilder jsStr = new StringBuilder();
            jsStr.Append("<script>var ary = [");

            if (atmTitles != null)
            {
                foreach (string part in parts)
                {
                    DataRow[] dr = dt.Select("hostname='" + part.Split(',')[0] + "'");

                    //foreach (DataRow dr in dt.Rows)
                    //{
                    jsStr.Append("['" + dr[0]["lat"] + "','" + dr[0]["lng"] + "','" + dr[0]["hostname"] + "','" + (isDisplayingDownAtms?"1":"0") + "'],");
                    //}
                }
            }
            else
            {
                foreach (DataRow dr in dt.Rows)
                    jsStr.Append("['" + dr["lat"] + "','" + dr["lng"] + "','" + dr["hostname"] + "','2'],");

            }

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
                LogableTask.LogMonoActivityTask("err", System.Reflection.MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Error, ex);
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
                LogableTask.LogMonoActivityTask("err", System.Reflection.MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Error, ex);
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
            string comment = null;
            string rowPart = null;
                                
            SqlConnection conn = ConnectionFactory.GetNewConnection();
            try
            {
                conn.Open();
                conn.ChangeDatabase(jpsDBName);
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = string.Format(networkSQLPinger1 + networkSQLPinger2, Session[SessionVars.SelectedATMs.ToString()]);
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
                        for (int k = 0; k < dtNetworkHost.Rows.Count; k++)
                        {
                            DataRow[] drArray = dtNetwork.Select("host='" + dtNetworkHost.Rows[k][0] + "'");
                            foreach (DataRow dr in drArray)
                            {
                                if (dr["status"].ToString() == "DOWN" || dr["status"].ToString() == "FAILED")
                                {
                                    //networkDownTitle += dr["host"] + "," + dr["location"] + "," + dr["insert_time"] + "," + dr["objects"] + " " + dr["status"] + ";";
                                    //machineDownOnNetwork++;
                                    //break;

                                    if (comment == null)
                                    {
                                        machineDownOnNetwork++;
                                        rowPart = dr["host"] + "," + dr["location"] + "," + dr["insert_time"];
                                    }
                                    comment += dr["objects"] + " " + dr["status"] + "<br/>";



                                }
                            }
                            if (comment != null)
                                networkDownTitle += rowPart + "," + comment + ";";

                        }
                    }
                    else if (dtNetwork.Rows.Count == 0)
                    {
                        for (int k = 0; k < dtPowerHost.Rows.Count; k++)
                        {
                            DataRow[] drArray = dtPower.Select("host='" + dtNetworkHost.Rows[k][0] + "'");
                            foreach (DataRow dr in drArray)
                            {
                                if (dr["status"].ToString() == "DOWN" || dr["status"].ToString() == "FAILED")
                                {
                                    powerDownTitle += dr["host"] + "," + dr["location"] + "," + dr["insert_time"] + "," + dr["objects"] + " " + dr["status"] + ";";
                                    machineDownOnPower++;
                                    break;

                                }
                            }

                        }
                        //foreach (DataRow dr in dtPower.Rows)
                        //{



                        //    //if (dr["status"].ToString() == "DOWN")
                        //    //{
                        //    //    powerDownTitle += dr["host"] + "," + dr["location"] + "," + dr["insert_time"] + "," + dr["objects"] + " " + dr["status"] + ";";
                        //    //    machineDownOnPower++;
                        //    //}
                        //}
                    }
                    else
                    {
                        for (int k = 0; k < dtPowerHost.Rows.Count; k++)
                        {
                            if (dtNetworkHost.Select("host='" + dtPowerHost.Rows[k][0] + "'").Length == 0)
                            {
                                DataRow newRow = dtNetworkHost.NewRow();
                                newRow[0] = dtPowerHost.Rows[k][0];
                                dtNetworkHost.Rows.Add(newRow);
                            }
                        }


                        for (int i = 0; i < dtNetworkHost.Rows.Count; i++)
                        {
                            DataRow[] drNetworkHost = dtNetwork.Select("host='" + dtNetworkHost.Rows[i]["host"] + "'");
                            DataRow[] drPowerHost = dtPower.Select("host='" + dtNetworkHost.Rows[i]["host"] + "'");
                            comment = rowPart = null;

                            if (drPowerHost.Length > 0 && drNetworkHost.Length > 0)
                            {
                                if (drPowerHost[0]["status"].ToString() == "DOWN" || drPowerHost[0]["status"].ToString() == "FAILED")
                                {
                                    machineDownOnPower++;
                                    powerDownTitle += drPowerHost[0]["host"] + "," + drPowerHost[0]["location"] + "," + drPowerHost[0]["insert_time"] + "," + drNetworkHost[0]["objects"] + " " + drNetworkHost[0]["status"] + ";";
                                }

                                else
                                {
                                    for (int m = 0; m < drNetworkHost.Length; m++)
                                    {
                                        if (drNetworkHost[m]["status"].ToString() == "DOWN" || drNetworkHost[m]["status"].ToString() == "FAILED")
                                        {

                                            if (comment == null)
                                            {
                                                machineDownOnNetwork++;
                                                rowPart = drNetworkHost[m]["host"] + "," + drNetworkHost[m]["location"] + "," + drNetworkHost[m]["insert_time"];
                                            }
                                            comment += drNetworkHost[m]["objects"] + " " + drNetworkHost[m]["status"] + "<br/>";


                                            //machineDownOnNetwork++;
                                            //networkDownTitle += drNetworkHost[m]["host"] + "," + drNetworkHost[m]["location"] + "," + drNetworkHost[m]["insert_time"] + "," + drNetworkHost[m]["objects"] + " " + drNetworkHost[m]["status"] + ";";
                                        }

                                    }
                                    if (comment != null)
                                        networkDownTitle += rowPart + "," + comment + ";";
                                }
                            }
                            else if (drPowerHost.Length == 0)
                            {
                                for (int j = 0; j < drNetworkHost.Length; j++)
                                {
                                    if (drNetworkHost[j]["status"].ToString() == "DOWN" || drNetworkHost[j]["status"].ToString() == "FAILED")
                                    {
                                        if (comment == null)
                                        {
                                            machineDownOnNetwork++;
                                            rowPart = drNetworkHost[j]["host"] + "," + drNetworkHost[j]["location"] + "," + drNetworkHost[j]["insert_time"];
                                        }
                                        comment += drNetworkHost[j]["objects"] + " " + drNetworkHost[j]["status"] + "<br/>";
                                    }
                                }
                                if (comment != null)
                                    networkDownTitle += rowPart + "," + comment + ";";
                            }
                            else if (drNetworkHost.Length == 0)
                            {
                                for (int j = 0; j < drPowerHost.Length; j++)
                                {
                                    if (drPowerHost[j]["status"].ToString() == "DOWN" || drPowerHost[j]["status"].ToString() == "FAILED")
                                    {
                                        machineDownOnPower++;
                                        powerDownTitle += drPowerHost[j]["host"] + "," + drPowerHost[j]["location"] + "," + drNetworkHost[j]["insert_time"] + "," + drNetworkHost[j]["objects"] + " " + drNetworkHost[j]["status"] + ";";
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    networkDown = machineDownOnNetwork;
                    powerDown = machineDownOnPower;
                }

            }
            catch (Exception ex)
            {
                LogableTask.LogMonoActivityTask("err", System.Reflection.MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Error, ex);
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
                cmd.CommandText = string.Format(temperatureSQL, Session[SessionVars.SelectedATMs.ToString()]);
                //cmd.CommandText = temperatureSQL;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                foreach (DataRow dr in dt.Rows)
                    builder.Append(dr["host"] + "," + dr["location"] + "," + dr["insert_time"] + ";");
                Session["temperatureTitles"] = builder.ToString();
                result = dt.Rows.Count;
            }
            catch (Exception ex)
            {
                LogableTask.LogMonoActivityTask("err", System.Reflection.MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Error, ex);
                //literal_Error.Text = "<b>" + ex.Message + "</b>";
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
                cmd.CommandText = string.Format(humiditySQL, Session[SessionVars.SelectedATMs.ToString()]);
                // cmd.CommandText = humiditySQL;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                foreach (DataRow dr in dt.Rows)
                    builder.Append(dr["host"] + "," + dr["location"] + "," + dr["insert_time"] + ";");
                Session["humidityTitles"] = builder.ToString();
                result = dt.Rows.Count;
            }
            catch (Exception ex)
            {
                LogableTask.LogMonoActivityTask("err", System.Reflection.MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Error, ex);
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
                LogableTask.LogMonoActivityTask("err", System.Reflection.MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Error, ex);
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
                LogableTask.LogMonoActivityTask("err", System.Reflection.MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Error, ex);
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
                LogableTask.LogMonoActivityTask("err", System.Reflection.MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Error, ex);
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
            return result;
        }

        private void DrawPowerHourlyAnalysisGraph()
        {
            SqlConnection conn = ConnectionFactory.GetNewConnection();
            try
            {
                conn.Open();
                conn.ChangeDatabase(jpsDBName);
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = string.Format(powerHourlySQL, Session[SessionVars.SelectedATMs.ToString()]);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dtPower = new DataTable();
                adapter.Fill(dtPower);
                for (int i = 0; i < dtPower.Rows.Count; i++)
                {
                    Chart4.Series[0].Points.AddXY(dtPower.Rows[i][0], dtPower.Rows[i][1]);
                    Chart4.Series[0].Points[i].Color = System.Drawing.Color.Red;
                }
                this.Chart4.Series["Default"]["ShowMarkerLines"] = "true";
                this.Chart4.Series["Default"]["DrawingStyle"] = "Cylinder";
                this.Chart4.Series["Default"]["PointWidth"] = "0.8";
            }
            catch (Exception ex)
            {
                LogableTask.LogMonoActivityTask("err", System.Reflection.MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Error, ex);
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }
        private void DrawNetworkHourlyAnalysisGraph()
        {
            SqlConnection conn = ConnectionFactory.GetNewConnection();
            try
            {
                conn.Open();
                conn.ChangeDatabase(jpsDBName);
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = string.Format(networkHourlySQL, Session[SessionVars.SelectedATMs.ToString()]);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dtNetwork = new DataTable();
                adapter.Fill(dtNetwork);
                for (int i = 0; i < dtNetwork.Rows.Count; i++)
                {
                    Chart1.Series[0].Points.AddXY(dtNetwork.Rows[i][0], dtNetwork.Rows[i][1]);
                    Chart1.Series[0].Points[i].Color = System.Drawing.Color.Orange;
                }
                this.Chart1.Series["Default"]["ShowMarkerLines"] = "true";
                this.Chart1.Series["Default"]["DrawingStyle"] = "Cylinder";
                this.Chart1.Series["Default"]["PointWidth"] = "0.8";
            }
            catch (Exception ex)
            {
                LogableTask.LogMonoActivityTask("err", System.Reflection.MethodBase.GetCurrentMethod(), System.Diagnostics.TraceLevel.Error, ex);
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }
        private void DrawAll()
        {

            string powerDownTitles = "";
            string networkDownTitles = "";
            int powerDownCount = 0;
            int networkDownCount = 0;
            int totalDevices = GetTotalDevicesCount();
            GetNetworkPowerDownCount(ref powerDownCount, ref networkDownCount, ref powerDownTitles, ref networkDownTitles);
            Session["powerDownTitles"] = powerDownTitles;
            Session["networkDownTitles"] = networkDownTitles;

            Label_Gen.Text = GetATMsOnGenerator().ToString();
            Label_Cam.Text = GetATMsCameraDisconnected().ToString();
            Label_Solar.Text = GetATMsOnSolar().ToString();
            Label_PowerDown.Text = powerDownCount.ToString();
            Label_CommsDown.Text = networkDownCount.ToString();
            Label_TotalDevices.Text = totalDevices.ToString();
            Label_Humidity.Text = GetATMsExceedHumidityLimit().ToString();
            Label_Temp.Text = GetATMsExceedTempLimit().ToString();
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
            Chart3.Series[0].Points.AddXY("Up", Math.Round(100 - ((double)networkDownCount / totalDevices) * 100, 2));
            Chart3.Series[0].Points.AddXY("Down", Math.Round(((double)networkDownCount / totalDevices) * 100, 2));
            Chart3.Series[0].Points[0].Color = System.Drawing.Color.Green;
            Chart3.Series[0].Points[1].Color = System.Drawing.Color.Red;
            //Label_DownOnComms.Text = networkDownCount.ToString();

            //StringBuilder builder = new StringBuilder();
            //builder.Append("    <table class=\"EU_DataTable\"><tbody><tr><th scope=\"col\">ATM</th><th scope=\"col\">Location</th><th scope=\"col\">Recorded At</th></tr>");

            //Label_DownOnComms.Attributes.Add("onclick", "receiveCallBackResult('<table border=1 width=100%>" + builder.ToString() + "</table>','2');");
            Chart3.Series[0]["DoughnutRadius"] = "70";
            Chart3.Series["Default"].Points[1]["Exploded"] = "true";
            Chart3.Series[0]["PieDrawingStyle"] = "SoftEdge";
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            Chart2.Series[0].Points.AddXY("Up", Math.Round(100 - ((double)powerDownCount / totalDevices) * 100, 2));
            Chart2.Series[0].Points.AddXY("Down", Math.Round(((double)powerDownCount / totalDevices) * 100, 2));
            Chart2.Series[0].Points[0].Color = System.Drawing.Color.Green;
            Chart2.Series[0].Points[1].Color = System.Drawing.Color.Red;
            Chart2.Series[0]["DoughnutRadius"] = "70";
            Chart2.Series["Default"].Points[1]["Exploded"] = "true";
            Chart2.Series[0]["PieDrawingStyle"] = "SoftEdge";

            DrawPowerHourlyAnalysisGraph();
            DrawNetworkHourlyAnalysisGraph();

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (DialogCallBackCustomerDetail.CausedCallback) return;
                DrawAll();
                DrawGoogleMap(null,false);
            }




        }

        protected void Button_DownOnComms_Click(object sender, EventArgs e)
        {
            // StringBuilder builder = new StringBuilder();
            //string[] parts =  Session["networkDownTitles"].ToString().Split(';');
            //foreach (String part in parts)
            //    builder.Append(part + ",");
            DrawGoogleMap(Session["networkDownTitles"].ToString(),false);
            DrawAll();
        }


        protected void Button_DownOnPower_Click(object sender, EventArgs e)
        {

            DrawGoogleMap(Session["powerDownTitles"].ToString(),true);
            DrawAll();
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
                builder.Append("    <table class=\"EU_DataTable\"><tbody><tr><th scope=\"col\">ATM</th><th scope=\"col\">Location</th><th scope=\"col\">Downtime</th><th scope=\"col\">Comments</th></tr>");

                //Session["powerDownTitles"] = powerDownTitles;
                string[] parts = Session["networkDownTitles"].ToString().Split(';');
                foreach (String part in parts)
                {
                    string[] subParts = part.Split(',');
                    if (subParts.Length > 1)
                        builder.Append("<tr><td>" + subParts[0] + "</td><td>" + subParts[1] + "</td><td>" + subParts[2] + "</td><td>" + subParts[3] + "</td></tr>");
                }
                builder.Append("</tbody></table>");
            }
            else if (type == 3)
            {
                builder.Append("    <table class=\"EU_DataTable\"><tbody><tr><th scope=\"col\">ATM</th><th scope=\"col\">Location</th><th scope=\"col\">Downtime</th></tr>");

                //Session["powerDownTitles"] = powerDownTitles;
                string[] parts = Session["powerDownTitles"].ToString().Split(';');
                foreach (String part in parts)
                {
                    string[] subParts = part.Split(',');
                    if (subParts.Length > 1)
                        builder.Append("<tr><td>" + subParts[0] + "</td><td>" + subParts[1] + "</td><td>" + subParts[2] + "</td></tr>");
                }
                builder.Append("</tbody></table>");
            }
            else if (type == 4)
            {
                builder.Append("    <table class=\"EU_DataTable\"><tbody><tr><th scope=\"col\">ATM</th><th scope=\"col\">Location</th><th scope=\"col\">Recorded At</th></tr>");

                //Session["powerDownTitles"] = powerDownTitles;
                string[] parts = Session["humidityTitles"].ToString().Split(';');
                foreach (String part in parts)
                {
                    string[] subParts = part.Split(',');
                    if (subParts.Length > 1)
                        builder.Append("<tr><td>" + subParts[0] + "</td><td>" + subParts[1] + "</td><td>" + subParts[2] + "</td></tr>");
                }
                builder.Append("</tbody></table>");
            }
            else if (type == 5)
            {
                builder.Append("    <table class=\"EU_DataTable\"><tbody><tr><th scope=\"col\">ATM</th><th scope=\"col\">Location</th><th scope=\"col\">Recorded At</th></tr>");

                //Session["powerDownTitles"] = powerDownTitles;
                string[] parts = Session["temperatureTitles"].ToString().Split(';');
                foreach (String part in parts)
                {
                    string[] subParts = part.Split(',');
                    if (subParts.Length > 1)
                        builder.Append("<tr><td>" + subParts[0] + "</td><td>" + subParts[1] + "</td><td>" + subParts[2] + "</td></tr>");
                }
                builder.Append("</tbody></table>");
            }


            else if (type == 6)
            {
                builder.Append("    <table class=\"EU_DataTable\"><tbody><tr><th scope=\"col\">ATM</th><th scope=\"col\">Location</th><th scope=\"col\">Recorded At</th></tr>");

                //Session["powerDownTitles"] = powerDownTitles;
                string[] parts = Session["temperatureTitles"].ToString().Split(';');
                foreach (String part in parts)
                {
                    string[] subParts = part.Split(',');
                    if (subParts.Length > 1)
                        builder.Append("<tr><td>" + subParts[0] + "</td><td>" + subParts[1] + "</td><td>" + subParts[2] + "</td></tr>");
                }
                builder.Append("</tbody></table>");
            }

            else if (type == 7)
            {
                builder.Append("    <table class=\"EU_DataTable\"><tbody><tr><th scope=\"col\">ATM</th><th scope=\"col\">Location</th><th scope=\"col\">Recorded At</th></tr>");

                //Session["powerDownTitles"] = powerDownTitles;
                string[] parts = Session["temperatureTitles"].ToString().Split(';');
                foreach (String part in parts)
                {
                    string[] subParts = part.Split(',');
                    if (subParts.Length > 1)
                        builder.Append("<tr><td>" + subParts[0] + "</td><td>" + subParts[1] + "</td><td>" + subParts[2] + "</td></tr>");
                }
                builder.Append("</tbody></table>");
            }

            else if (type == 8)
            {
                builder.Append("    <table class=\"EU_DataTable\"><tbody><tr><th scope=\"col\">ATM</th><th scope=\"col\">Location</th><th scope=\"col\">Recorded At</th></tr>");

                //Session["powerDownTitles"] = powerDownTitles;
                string[] parts = Session["temperatureTitles"].ToString().Split(';');
                foreach (String part in parts)
                {
                    string[] subParts = part.Split(',');
                    if (subParts.Length > 1)
                        builder.Append("<tr><td>" + subParts[0] + "</td><td>" + subParts[1] + "</td><td>" + subParts[2] + "</td></tr>");
                }
                builder.Append("</tbody></table>");
            }


            e.Output.Write("<script> receiveCallBackResult('<table border=1 width=100%>" + builder.ToString() + "</table>','2');</script>");
        }
    }
}