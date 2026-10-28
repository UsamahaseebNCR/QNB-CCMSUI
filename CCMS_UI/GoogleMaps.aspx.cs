using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Avanza.CCMS.DAL;
using System.Text;
using Avanza.iSuite.UI;
using System.Data.SqlClient;

using System.Diagnostics;
using ComponentArt.Web.UI;
using System.Reflection;

namespace CCMSUI
{
    public partial class GoogleMaps : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //TextBox_FromDate.Text = DateTime.Now.AddDays(-7).ToString("dd/MM/yyyy");
                //TextBox_To.Text = DateTime.Now.ToString("dd/MM/yyyy");
                string[] parts = null;
                this.metaRefresh.Content = (((AppSetting)base.Application[ApplicationVars.AppSettings.ToString()]).DashboardRefreshInterval * 60).ToString();
                //this.Page.RegisterClientScriptBlock("script", "<script>function init() {cd(\"" + (int.Parse(this.metaRefresh.Content) / 60) + "\");} init();</script>");
                DropDownList_HotspotType.Items.Add("Any");
                DropDownList_HotspotType.Items.Add("Display All Atms");
                AlertType.AlertTypeReader alertTypeReader = AlertType.ExecuteReader("1=1");
                while (alertTypeReader.Read())
                    DropDownList_HotspotType.Items.Add(new ListItem(alertTypeReader.CurrentAlertType.AlertTypeName, alertTypeReader.CurrentAlertType.AlertTypeId.ToString()));
                alertTypeReader.Close();
                listBox_Region.Items.Add("-Select Region-");
                Region.RegionReader regionReader = Region.ExecuteReader("is_organization=0");
                while (regionReader.Read())
                {
                    listBox_Region.Items.Add(new ListItem(regionReader.CurrentRegion.RegionName, regionReader.CurrentRegion.RegionId.ToString()));
                }
                regionReader.Close();

                if (Session["regionId"] != null)
                {
                    parts = Session["regionId"].ToString().Split(',');
                    for (int i = 0; i < parts.Length - 1; i++)
                    {
                        foreach (ListItem listItem in listBox_Region.Items)
                        {
                            if (listItem.Value == parts[i])
                            {
                                listItem.Selected = true;
                                break;
                            }
                        }
                    }
                    listBox_Region_SelectedIndexChanged(this, null);

                    if (Session["atmId"] != null)
                    {
                        parts = Session["atmId"].ToString().Split(',');
                        for (int i = 0; i < parts.Length - 1; i++)
                        {
                            foreach (ListItem listItem in listBox_ATM.Items)
                            {
                                if (listItem.Value == parts[i])
                                {
                                    listItem.Selected = true;
                                    break;
                                }
                            }
                        }
                    }

                }
                if (Session["from"] != null)
                    TextBox_FromDate.Text = Session["from"].ToString();
                if (Session["to"] != null)
                    TextBox_To.Text = Session["to"].ToString();
                if (Session["hotspot"] != null)
                    DropDownList_HotspotType.SelectedValue = Session["hotspot"].ToString();
                else
                    DropDownList_HotspotType.SelectedIndex = 1;
                Button_RefereshHotSpot_Click(this, null);
            }
            //    Button_RefereshHotSpot_Click(this, null);
            //                  Literal_Script.Text= 
            //    @"<script type='text/javascript' src='https://maps.googleapis.com/maps/api/js?key=AIzaSyDiajbE-EYDux7RUgHfHljdV-vymbuO3_0&sensor=false'>    </script> " +
            //    " <script type='text/javascript'> " +
            //    "        function initialize() { " +
            //        " var myLatlng = new google.maps.LatLng(-34.397, 150.644); var myLatlng1 = new google.maps.LatLng(-33.397, 149.644); " + 
            //        " var mapOptions = { center: myLatlng, zoom: 8, mapTypeId: google.maps.MapTypeId.ROADMAP }; " +
            //        " var map = new google.maps.Map(document.getElementById('map-canvas'), mapOptions); "+ 
            //        " var marker = new google.maps.Marker({ position: myLatlng, map: map, title: 'Hello World!' }); " +
            //        " var marker1 = new google.maps.Marker({ position: myLatlng1, map: map, title: 'Hello World From Code!' }); " +
            //    " } " + 
            //    " google.maps.event.addDomListener(window, 'load', initialize);        "+
            //" </script>";


        }
        protected string Filter()
        {
            StringBuilder builderATM = new StringBuilder();
            StringBuilder builderRegion = new StringBuilder();
            StringBuilder builder = new StringBuilder();

            // string filter = "";
            //bool isAnyATMSelected = false;
            //bool isAnyRegionSelected = false;
            if (TextBox_FromDate.Text.Length > 0)
            {
                builder.Append(" and generated_at >= convert(datetime, '" + TextBox_FromDate.Text + "', 103)");
                Session["from"] = TextBox_FromDate.Text;
            }
            else
                Session["from"] = null;
            if (TextBox_To.Text.Length > 0)
            {
                builder.Append(" and generated_at <= convert(datetime, '" + TextBox_To.Text + " 23:59:59', 103)");
                Session["to"] = TextBox_To.Text;
            }
            else
                Session["to"] = null;
            if (DropDownList_HotspotType.SelectedIndex > 1)
            {
                builder.Append(" and alert_type.alert_type_id = " + DropDownList_HotspotType.SelectedValue);
                Session["hotspot"] = DropDownList_HotspotType.SelectedValue;
            }
            else
                Session["hotspot"] = null;
            /*atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ")"*/

            if (listBox_Region.SelectedIndex > 0)
            {
                foreach (ListItem listItem in listBox_Region.Items)
                {
                    if (listItem.Selected)
                        builderRegion.Append(listItem.Value + ",");
                }
            }
            else
                Session["regionId"] = null;

            if (listBox_ATM.SelectedIndex > 0)
            {
                foreach (ListItem listItem in listBox_ATM.Items)
                {
                    if (listItem.Selected)
                    {
                        builderATM.Append(listItem.Value + ",");
                    }
                }
            }
            else
                Session["atmId"] = null;


            if (builderRegion.Length > 0)
            {
                builder.Append(" and atm.region_id in (" + builderRegion.ToString() + "-1)");
                Session["regionId"] = builderRegion.ToString();

                if (builderATM.Length > 0)
                {
                    builder.Append(" and atm.atm_id in (" + builderATM.ToString() + "-1)");
                    Session["atmId"] = builderATM.ToString();
                }
            }
            else
                builder.Append(" and atm.atm_id in (" + Session[SessionVars.SelectedATMs.ToString()] + ")");
            return builder.ToString();
        }

        private string GetMaxTrxn(SqlCommand cmd, int atmID)
        {
            cmd.CommandText = "select max(trxn_datetime) from parsed_transaction where atm_id = " + atmID;
            object result = cmd.ExecuteScalar();
            if (result != DBNull.Value)
                return DateTime.Parse(result.ToString()).ToString("dd/MM/yyyy HH:mm:ss");
            else
                return "";
        }
        private string GetLastHeartBeat(SqlCommand cmd, int atmID)
        {
            cmd.CommandText = "select max(heart_beat_received_at) from heart_beat where atm_id = " + atmID;
            object result = cmd.ExecuteScalar();
            if (result != DBNull.Value)
            {
                DateTime temp = DateTime.Parse(result.ToString());
                //return DateTime.Parse(result.ToString()).ToString("dd/MM/yyyy HH:mm:ss");
                return temp.Date != DateTime.Today.Date ? "" : temp.ToString("dd/MM/yyyy HH:mm:ss");
            }
            else
                return "";
        }
        private decimal GetCashPosition(SqlCommand cmd, int atmID)
        {
            cmd.CommandText = @"select cassette1_notes*denomination_type_1+
                                cassette2_notes*denomination_type_2+
                                cassette3_notes*denomination_type_3+
                                cassette4_notes*denomination_type_4
                                from cash_position inner join atm outerATM on  cash_position.atm_id = outerATM.atm_id
                                inner join note_set_type on note_set_type.note_set_type_id = outerATM.note_set_type_id " +
                                " where cash_position.atm_id = " + atmID + " and last_trxn_at = (select MAX(last_trxn_at) from cash_position where atm_id =outerATM.atm_id )                                ";
            object result = cmd.ExecuteScalar();
            if (result != null)
                return decimal.Parse(result.ToString());
            else
                return -1;
        }

        private int IsATMInactivityAlertPresent(SqlCommand cmd, int atmID)
        {
            cmd.CommandText = @"select count(atm_alert_id) from atm_alert where alert_type_id = 31 and resolve_at is null and atm_id = " + atmID;
            return int.Parse(cmd.ExecuteScalar().ToString());
        }

        private void ProcessReader(ref Atm.AtmReader reader,ref SqlCommand cmd, ref StringBuilder positions,ref StringBuilder marker,ref int counter)
        {
         
            string heartBeat = "";
            int isInactivityAlertFound = 0;
            string imgUrl = "";
          
            SqlCommand tempCommand = null;
            try
            {
                while (reader.Read())
                {
                    if (reader.CurrentAtm.Latitude.Length > 0 && reader.CurrentAtm.Longitude.Length > 0)
                    {
                        //positions.Append("var myLatlng" + counter + " = new google.maps.LatLng(" + reader.CurrentAtm.Latitude + ", " + reader.CurrentAtm.Longitude + "); ");
                        //marker.Append("  markers[" + counter + "] = new google.maps.Marker({ icon: 'http://maps.google.com/mapfiles/ms/icons/red-dot.png',position: myLatlng" + counter + ", map: map, title: '" + reader.CurrentAtm.Title + "' }); ");
                        //counter++;


                        hiddenfield_title.Value += reader.CurrentAtm.Title + ",";
                        heartBeat = GetLastHeartBeat(cmd, reader.CurrentAtm.ATMId);
                        isInactivityAlertFound = IsATMInactivityAlertPresent(cmd, reader.CurrentAtm.ATMId);

                        if (isInactivityAlertFound == 1 && heartBeat.Length > 0)
                        {//Idle but heart beat received.
                            imgUrl = "images/atm_icon2.gif";
                            hiddenfield_state.Value += 1 + ",";
                        }
                        else if (isInactivityAlertFound == 0 && heartBeat.Length > 0) //Operational
                        {
                            imgUrl = "images/atm_icon3.gif";
                            hiddenfield_state.Value += 1 + ",";

                        }
                        else if (isInactivityAlertFound == 1 && heartBeat.Length == 0)
                        { //Need Attention
                            imgUrl = "images/atm_icon1.gif";
                            hiddenfield_state.Value += 0 + ",";
                        }
                        else
                        {
                            imgUrl = "images/atm_icon1.gif";
                            hiddenfield_state.Value += 0 + ",";
                        }
                        positions.Append("var myLatlng" + counter + " = new google.maps.LatLng(" + reader.CurrentAtm.Latitude + ", " + reader.CurrentAtm.Longitude + "); ");
                        marker.Append(" var marker" + counter + " =  new google.maps.Marker({ icon: '" + imgUrl + "',position: myLatlng" + counter + ", map: map, title: '" + reader.CurrentAtm.Title + "' }); ");

                        if (tempCommand == null)
                            tempCommand = ConnectionFactory.GetNewCommand(true);
                        StringBuilder tempBuilder = new StringBuilder();
                        tempCommand.CommandText = @"select name,val from sensor_current_status inner join sensor_oid_mapping on sensor_oid_mapping.sensor_oid_mapping_id=sensor_current_status.sensor_oid_mapping_id
                                                    where atm_id =" + reader.CurrentAtm.ATMId;
                        SqlDataReader tempReader = tempCommand.ExecuteReader();
                        while (tempReader.Read())
                        {
                            tempBuilder.Append(tempReader[0] + ":" + tempReader[1] + "<br/>");
                        }
                        tempReader.Close();


                        marker.Append("  var infowindow = new google.maps.InfoWindow();var html" + counter + "='<div>Title:" + reader.CurrentAtm.Title + "<br/>Cash Position:" + GetCashPosition(cmd, reader.CurrentAtm.ATMId) + "<br/>Last Transaction:" + GetMaxTrxn(cmd, reader.CurrentAtm.ATMId) + "<br/>Last Heart beat Received at:" + heartBeat + "<br/>" + tempBuilder.ToString() + " <\\/div>'; ");
                        //marker.Append(" google.maps.event.addListener(marker,'click',function() {map.setZoom(9); map.setCenter(marker.getPosition());   }); ");
                        marker.Append(" google.maps.event.addListener(marker" + counter + ",'click',function() {infowindow.setContent(html" + counter + ");infowindow.open(map, marker" + counter + "); infowindow.getContent();   }); ");

                        marker.Append("  markers[" + counter + "] = marker" + counter + " ;");

                        //                            marker.Append("  markers[" + counter + "] = new google.maps.Marker({ icon: 'images/icon_ATM.jpg',position: myLatlng" + counter + ", map: map, title: '" + reader.CurrentAtm.Title + "' }); ");
                        counter++;

                    }
                }
            }
            catch (Exception ex)
            {
                LogableTask.LogMonoActivityTask("ProcessReader", MethodBase.GetCurrentMethod(), TraceLevel.Error, ex);
            }
            finally
            {
                reader.Close();
                 if (tempCommand != null)
                    if (tempCommand.Connection != null)
                        tempCommand.Connection.Close();
            }
            //reader.Close();
        }
        protected void Button_RefereshHotSpot_Click(object sender, EventArgs e)
        {


            SqlCommand cmd = null;
            int counter = 0;
            StringBuilder positions = new StringBuilder();
            StringBuilder marker = new StringBuilder();
            try
            {
                StringBuilder atmIDs = new StringBuilder();
                //StringBuilder builder = new StringBuilder();
               
                if (DropDownList_Language.SelectedIndex == 0)
                {
                    main.Style.Add("direction", "ltr");
                    leftSide.Style.Add("float", "left");
                    click.Style.Add("float", "left");
                    //tbl.Style.Add("dir", "ltr");
                }
                else
                {
                    main.Style.Add("direction", "rtl");
                    leftSide.Style.Add("float", "right");
                    click.Style.Add("float", "right");
                    //tbl.Style.Add("dir", "rtl");
                }

                if (DropDownList_HotspotType.SelectedIndex == 1)
                {
                    cmd = ConnectionFactory.GetNewCommand(true);
                    cmd.CommandTimeout = 100;

                    Atm.AtmReader reader = Atm.ExecuteReader("is_active=1 and Latitude is not null and Longitude is not null");
                    ProcessReader(ref reader, ref cmd, ref positions, ref marker, ref counter);
                    //                     
//                    while (reader.Read())
//                    {
//                        if (reader.CurrentAtm.Latitude.Length > 0 && reader.CurrentAtm.Longitude.Length > 0)
//                        {
//                            //positions.Append("var myLatlng" + counter + " = new google.maps.LatLng(" + reader.CurrentAtm.Latitude + ", " + reader.CurrentAtm.Longitude + "); ");
//                            //marker.Append("  markers[" + counter + "] = new google.maps.Marker({ icon: 'http://maps.google.com/mapfiles/ms/icons/red-dot.png',position: myLatlng" + counter + ", map: map, title: '" + reader.CurrentAtm.Title + "' }); ");
//                            //counter++;


//                            hiddenfield_title.Value += reader.CurrentAtm.Title + ",";
//                            heartBeat = GetLastHeartBeat(cmd, reader.CurrentAtm.ATMId);
//                            isInactivityAlertFound = IsATMInactivityAlertPresent(cmd, reader.CurrentAtm.ATMId);

//                            if (isInactivityAlertFound == 1 && heartBeat.Length > 0)
//                            {//Idle but heart beat received.
//                                imgUrl = "images/atm_icon2.gif";
//                                hiddenfield_state.Value += 1 + ",";
//                            }
//                            else if (isInactivityAlertFound == 0 && heartBeat.Length > 0) //Operational
//                            {
//                                imgUrl = "images/atm_icon3.gif";
//                                hiddenfield_state.Value += 1 + ",";

//                            }
//                            else if (isInactivityAlertFound == 1 && heartBeat.Length == 0)
//                            { //Need Attention
//                                imgUrl = "images/atm_icon1.gif";
//                                hiddenfield_state.Value += 0 + ",";
//                            }
//                            else
//                            {
//                                imgUrl = "images/atm_icon1.gif";
//                                hiddenfield_state.Value += 0 + ",";
//                            }
//                            positions.Append("var myLatlng" + counter + " = new google.maps.LatLng(" + reader.CurrentAtm.Latitude + ", " + reader.CurrentAtm.Longitude + "); ");
//                            marker.Append(" var marker" + counter + " =  new google.maps.Marker({ icon: '" + imgUrl + "',position: myLatlng" + counter + ", map: map, title: '" + reader.CurrentAtm.Title + "' }); ");

//                            if (tempCommand == null)
//                                tempCommand = ConnectionFactory.GetNewCommand(true);
//                            StringBuilder tempBuilder = new StringBuilder();
//                            tempCommand.CommandText = @"select name,val from sensor_current_status inner join sensor_oid_mapping on sensor_oid_mapping.sensor_oid_mapping_id=sensor_current_status.sensor_oid_mapping_id
//                                                    where atm_id =" + reader.CurrentAtm.ATMId;
//                            SqlDataReader tempReader = tempCommand.ExecuteReader();
//                            while (tempReader.Read())
//                            {
//                                tempBuilder.Append(tempReader[0] + ":" + tempReader[1] + "<br/>");
//                            }
//                            tempReader.Close();


//                            marker.Append("  var infowindow = new google.maps.InfoWindow();var html" + counter + "='<div>Title:" + reader.CurrentAtm.Title + "<br/>Cash Position:" + GetCashPosition(cmd, reader.CurrentAtm.ATMId) + "<br/>Last Transaction:" + GetMaxTrxn(cmd, reader.CurrentAtm.ATMId) + "<br/>Last Heart beat Received at:" + heartBeat + "<br/>" + tempBuilder.ToString() + " <\\/div>'; ");
//                            //marker.Append(" google.maps.event.addListener(marker,'click',function() {map.setZoom(9); map.setCenter(marker.getPosition());   }); ");
//                            marker.Append(" google.maps.event.addListener(marker" + counter + ",'click',function() {infowindow.setContent(html" + counter + ");infowindow.open(map, marker" + counter + "); infowindow.getContent();   }); ");

//                            marker.Append("  markers[" + counter + "] = marker" + counter + " ;");

//                            //                            marker.Append("  markers[" + counter + "] = new google.maps.Marker({ icon: 'images/icon_ATM.jpg',position: myLatlng" + counter + ", map: map, title: '" + reader.CurrentAtm.Title + "' }); ");
//                            counter++;
                        
//                        }
//                    }
//                    reader.Close();
                }
                else
                {
                    cmd = ConnectionFactory.GetNewCommand(true);
                    cmd.CommandTimeout = 100;

                    cmd.CommandText = @"
                                                    select distinct(atm_alert.atm_id) from 
                                                    atm_alert,atm,alert_type
                                                    where atm.atm_id = atm_alert.atm_id 
                                                    and atm_alert.alert_type_id = alert_type.alert_type_id
                                                    and atm.is_active=1" + Filter();


//                    cmd.CommandText = @"
//                                select distinct(atm.atm_id) from 
//                                atm left join atm_alert,atm,alert_type
//                                where atm.atm_id = atm_alert.atm_id 
//                                and atm_alert.alert_type_id = alert_type.alert_type_id
//                                and atm.is_active=1" + Filter();


                    DataTable dt = new DataTable();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            atmIDs.Append(dr[0] + ",");
                        }
                        atmIDs.Append("-1");
                        Atm.AtmReader reader = Atm.ExecuteReader("is_active=1 and Latitude is not null and Longitude is not null and atm_id in (" + atmIDs.ToString() + ")");
                        ProcessReader(ref reader, ref cmd, ref positions, ref marker, ref counter);
//                        while (reader.Read())
//                        {
//                            if (reader.CurrentAtm.Latitude.Length > 0 && reader.CurrentAtm.Longitude.Length > 0)
//                            {
//                                hiddenfield_title.Value += reader.CurrentAtm.Title + ",";
//                                heartBeat = GetLastHeartBeat(cmd, reader.CurrentAtm.ATMId);
//                                isInactivityAlertFound = IsATMInactivityAlertPresent(cmd, reader.CurrentAtm.ATMId);

//                                if (isInactivityAlertFound == 1 && heartBeat.Length > 0)
//                                {//Idle but heart beat received.
//                                    imgUrl = "images/atm_icon2.gif";
//                                    hiddenfield_state.Value += 1 + ",";
//                                }
//                                else if (isInactivityAlertFound == 0 && heartBeat.Length > 0) //Operational
//                                {
//                                    imgUrl = "images/atm_icon3.gif";
//                                    hiddenfield_state.Value += 1 + ",";

//                                }
//                                else if (isInactivityAlertFound == 1 && heartBeat.Length == 0)
//                                { //Need Attention
//                                    imgUrl = "images/atm_icon1.gif";
//                                    hiddenfield_state.Value += 0 + ",";
//                                }
//                                positions.Append("var myLatlng" + counter + " = new google.maps.LatLng(" + reader.CurrentAtm.Latitude + ", " + reader.CurrentAtm.Longitude + "); ");
//                                marker.Append(" var marker" + counter + " =  new google.maps.Marker({ icon: '" + imgUrl + "',position: myLatlng" + counter + ", map: map, title: '" + reader.CurrentAtm.Title + "' }); ");
                            
//                                if (tempCommand == null)
//                                    tempCommand = ConnectionFactory.GetNewCommand(true);
//                                StringBuilder tempBuilder = new StringBuilder();
//                                tempCommand.CommandText = @"select name,val from sensor_current_status inner join sensor_oid_mapping on sensor_oid_mapping.sensor_oid_mapping_id=sensor_current_status.sensor_oid_mapping_id
//                                                    where atm_id =" + reader.CurrentAtm.ATMId;
//                                SqlDataReader tempReader = tempCommand.ExecuteReader();
//                                while (tempReader.Read())
//                                {
//                                    tempBuilder.Append(tempReader[0] + ":" + tempReader[1] + "<br/>");
//                                }
//                                tempReader.Close();


//                                marker.Append("  var infowindow = new google.maps.InfoWindow();var html" + counter + "='<div>Title:" + reader.CurrentAtm.Title + "<br/>Cash Position:" + GetCashPosition(cmd, reader.CurrentAtm.ATMId) + "<br/>Last Transaction:" + GetMaxTrxn(cmd, reader.CurrentAtm.ATMId) + "<br/>Last Heart beat Received at:" + heartBeat + "<br/>" + tempBuilder.ToString() + " <\\/div>'; ");
//                                //marker.Append(" google.maps.event.addListener(marker,'click',function() {map.setZoom(9); map.setCenter(marker.getPosition());   }); ");
//                                marker.Append(" google.maps.event.addListener(marker" + counter + ",'click',function() {infowindow.setContent(html" + counter + ");infowindow.open(map, marker" + counter + "); infowindow.getContent();   }); ");

//                                marker.Append("  markers[" + counter + "] = marker" + counter + " ;");

//                                //                            marker.Append("  markers[" + counter + "] = new google.maps.Marker({ icon: 'images/icon_ATM.jpg',position: myLatlng" + counter + ", map: map, title: '" + reader.CurrentAtm.Title + "' }); ");
//                                counter++;
//                            }
//                        }
                        //if (hiddenfield_title.Value.Length > 0)
                        //{
                        //    hiddenfield_title.Value += "'";
                        //  //  builder.Append("''");
                        //}
                        //js_array.Text = "<script>var atmTitle = ["+ builder.ToString()+"]</script>";
                        //reader.Close();

                        //reader = Atm.ExecuteReader("is_active=1 and Latitude is not null and Longitude is not null and atm_id not in (" + atmIDs.ToString() + ")");
                        //while (reader.Read())
                        //{
                        //    if (reader.CurrentAtm.Latitude.Length > 0 && reader.CurrentAtm.Longitude.Length > 0)
                        //    {
                        //        heartBeat = GetLastHeartBeat(cmd, reader.CurrentAtm.ATMId);
                        //        isInactivityAlertFound = IsATMInactivityAlertPresent(cmd, reader.CurrentAtm.ATMId);

                        //        if (isInactivityAlertFound == 1 && heartBeat.Length > 0) //Idle but heart beat received.
                        //            imgUrl = "images/atm_icon2.gif";
                        //        else if (isInactivityAlertFound == 0 && heartBeat.Length > 0) //Idle but heart beat received.
                        //            imgUrl = "images/atm_icon3.gif";
                        //        else if (isInactivityAlertFound == 0 && heartBeat.Length == 0) //Idle but heart beat received.
                        //            imgUrl = "images/atm_icon1.gif";



                        //        positions.Append("var myLatlng" + counter + " = new google.maps.LatLng(" + reader.CurrentAtm.Latitude + ", " + reader.CurrentAtm.Longitude + "); ");
                        //        //marker.Append(" var marker" + counter + " = new google.maps.Marker({ icon: 'http://maps.google.com/mapfiles/ms/icons/blue-dot.png',position: myLatlng" + counter + ", map: map, title: '" + reader.CurrentAtm.Title + "[OK]' }); ");
                        //        // marker.Append("  markers[" + counter + "] = new google.maps.Marker({ icon: 'http://maps.google.com/mapfiles/ms/icons/blue-dot.png',position: myLatlng" + counter + ", map: map, title: '" + reader.CurrentAtm.Title + "[OK]' }); ");
                        //        // marker.Append(" markers[" + counter + "] = new google.maps.Marker({ icon: 'images/icon_ATM.jpg',position: myLatlng" + counter + ", map: map, title: '" + reader.CurrentAtm.Title + "' }); ");
                        //        marker.Append(" var marker" + counter + " =  new google.maps.Marker({ icon: '" + imgUrl + "',position: myLatlng" + counter + ", map: map, title: '" + reader.CurrentAtm.Title + "' }); ");
                        //        //marker.Append("  var infowindow = new google.maps.InfoWindow({  content:\"" + reader.CurrentAtm.Title + "\"  });  ");
                        //        marker.Append("  var infowindow = new google.maps.InfoWindow();var html" + counter + "='<div>Title:" + reader.CurrentAtm.Title + "<br/>Cash Position:" + GetCashPosition(cmd, reader.CurrentAtm.ATMId) + "<br/>Last Transaction:" + GetMaxTrxn(cmd, reader.CurrentAtm.ATMId) + "<br/>Last Heart beat Received at:" + GetLastHeartBeat(cmd, reader.CurrentAtm.ATMId) + " <\\/div>'; ");
                        //        marker.Append(" google.maps.event.addListener(marker" + counter + ",'click',function() {infowindow.setContent(html" + counter + ");infowindow.open(map, marker" + counter + "); infowindow.getContent();   }); ");
                        //        marker.Append("  markers[" + counter + "] = marker" + counter + " ;");
                        //        //addListener(marker, "click", function() { marker.openInfoWindowTabsHtml(html); });

                        //        counter++;
                        //    }
                        //}
                        //                       reader.Close();

                    }

                }
                if (counter > 0)
                {
                    //this.metaRefresh.Content = (((AppSetting)base.Application[ApplicationVars.AppSettings.ToString()]).DashboardRefreshInterval * 60).ToString();
                    int zoom = int.Parse(System.Configuration.ConfigurationManager.AppSettings["googleMapZoom"]);
                    //if (counter == 1)
                    //   zoom = 3;
                    string temp = "";
                    for (int i = 0; i < counter; i++)
                        temp += "bounds.extend(myLatlng" + i + ");";

                    Literal_Script.Text =
                                      @"
                                        <script type='text/javascript' src='https://maps.googleapis.com/maps/api/js?key=AIzaSyAvdrOICJ-g2i2ell8yebXii3qP8DKcHHA&sensor=false&language=" + DropDownList_Language.SelectedItem.Value + "'>    </script> " +
                                      " <script type='text/javascript'> " +
                        //
                                      "var markers = new Array(); var map;   function initialize() {cd('" + ((((AppSetting)base.Application[ApplicationVars.AppSettings.ToString()]).DashboardRefreshInterval * 60) / 60) + "');" +
                                          positions.ToString() + //google.maps.visualRefresh = true; 
                                          " var mapOptions = { center: new google.maps.LatLng(45.4555729, 9.169236),zoomControl: true,zoomControlOptions: {style: google.maps.ZoomControlStyle.LARGE,   position: google.maps.ControlPosition.RIGHT_CENTER},scaleControl: false,streetViewControl: false,streetViewControlOptions: {position: google.maps.ControlPosition.RIGHT_CENTER},panControl: true,mapTypeControl: false,panControlOptions: {position: google.maps.ControlPosition.RIGHT_CENTER        },zoom: " + zoom + ", mapTypeId: google.maps.MapTypeId.ROADMAP }; " +
                                          " map = new google.maps.Map(document.getElementById('map-canvas'), mapOptions); " +
                                          marker.ToString();
                    // "  google.maps.event.addListener(map,'click',function() {map.setZoom(9)}); google.maps.event.addListener(markers[0],'click',function() {map.setZoom(9); map.setCenter(marker.getPosition());   });       "+
                    for (int i = 0; i < counter; i++)
                        Literal_Script.Text += "google.maps.event.addListener(markers[" + i + "], 'click', find_closest_marker);";
                    //if (counter > 1)
                    Literal_Script.Text += "var bounds = new google.maps.LatLngBounds();" + temp + "  map.fitBounds(bounds);var listener = google.maps.event.addListener(map, 'idle', function() {   if (map.getZoom() > 16) map.setZoom(16); google.maps.event.removeListener(listener); });";
                    //else
                    //   Literal_Script.Text += "map.fitBounds();";
                    Literal_Script.Text += "} google.maps.event.addDomListener(window, 'load', initialize); </script>";
                    //google.maps.event.addListener(map, 'click', find_closest_marker);
                    /* var bounds = new google.maps.LatLngBounds();
            bounds.extend(myPlace);
            bounds.extend(Item_1);
            map.fitBounds(bounds);*/
                }
                else
                    Literal_Script.Text = "<script>alert('No records found.')</script>";
            }
            catch (Exception ex)
            {
                LogableTask.LogMonoActivityTask("DisplayMap", System.Reflection.MethodBase.GetCurrentMethod(), TraceLevel.Info, ex);
                Literal_Script.Text = "<script>alert('" + ex.Message + "')</script>";
            }
            finally
            {
                if (cmd != null)
                    if (cmd.Connection != null)
                        cmd.Connection.Close();
               


            }
        }

        protected void DialogCallBack_Callback(object sender, CallBackEventArgs e)
        {

        }
        protected void listBox_Region_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool isAnyRegionSelected = false;
            StringBuilder builder = new StringBuilder();
            listBox_ATM.Items.Clear();

            if (listBox_Region.SelectedIndex > 0)
            {
                listBox_ATM.Items.Add("-Select ATM-");

                foreach (ListItem item in listBox_Region.Items)
                {
                    if (item.Selected)
                    {
                        isAnyRegionSelected = true;
                        builder.Append(item.Value + ",");
                    }
                }
                if (isAnyRegionSelected)
                {
                    //                   Session["regionId"] = builder.ToString();
                    builder.Append("-1");
                    Atm.AtmReader atmReader = Atm.ExecuteReader("region_id in (" + builder.ToString() + ")");
                    while (atmReader.Read())
                        listBox_ATM.Items.Add(new ListItem(atmReader.CurrentAtm.Title, atmReader.CurrentAtm.ATMId.ToString()));
                    atmReader.Close();
                }
                //             else
                //               Session["regionId"] = null;
            }
        }



    }
}