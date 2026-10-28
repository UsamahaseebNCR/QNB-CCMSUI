<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GoogleMaps.aspx.cs" Inherits="CCMSUI.GoogleMaps" %>

<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <meta http-equiv="Refresh" content="30" id="metaRefresh" runat="server" />
    <script src="content/shortcut.js" type="text/javascript" language="javascript">
    </script>
    <script type="text/javascript" src="content/js/jquery.js"></script>
    <script type="text/javascript" src="content/Scripts/jquery.ui.core.js"></script>
    <script src="content/js/jquery.js" type="text/javascript"></script>
    <script src="content/Scripts/jquery-ui-1.8.16.min.js" type="text/javascript"></script>
    <script type="text/javascript" src="content/Scripts/jquery.ui.widget.js"></script>
    <script type="text/javascript" src="content/Scripts/jquery.ui.datepicker.js"></script>
    <script type="text/javascript" src="content/Scripts/jquery-ui-timepicker-addon.js"></script>
    <link href="content/iSuite.css" rel="stylesheet" />
    <link href="content/iSuite.css" rel="stylesheet" />
    <meta name="viewport" content="initial-scale=1.0, user-scalable=no" />
    <style type="text/css">
         html, body, #map-canvas {        height: 100%;        margin: 0px;        padding: 0px      }      .controls {        margin-top: 16px;        border: 1px solid transparent;        border-radius: 2px 0 0 2px;        box-sizing: border-box;        -moz-box-sizing: border-box;        height: 32px;        outline: none;        box-shadow: 0 2px 6px rgba(0, 0, 0, 0.3);      }      #pac-input {        background-color: #fff;        padding: 0 11px 0 13px;        width: 400px;        font-family: Roboto;        font-size: 15px;        font-weight: 300;        text-overflow: ellipsis;      }      #pac-input:focus {        border-color: #4d90fe;        margin-left: -1px;        padding-left: 14px;  /* Regular padding-left + 1. */        width: 401px;      }      .pac-container {        font-family: Roboto;      }      #type-selector {        color: #fff;        background-color: #4d90fe;        padding: 5px 11px 0px 11px;      }      #type-selector label {        font-family: Roboto;        font-size: 13px;        font-weight: 300;      }}
        #leftSide
        {
            height: 500px;
            width: 5px;
            float: left;
            display:none;
        }
        #click
        {
            height: 116px;
            width: 5px;
            background-image: url('images/splitter_horColHover.gif');
            float: left;
            margin-top: 20px;
        }
        html
        {
            height: 100%;
        }
        body
        {
            height: 100%;
            margin: 0;
            padding: 0;
        }
        #map-canvas
        {
            height: 100%;
        }
    </style>
    <script type="text/javascript">

//        function init() {
//            
//            alert('d2');
//            alert(document.getElementById('metaRefresh').value);
//            cd('60');
//        }

        var mins;
        var secs;

        function cd(minutes) {
            //alert(minutes);
            //alert(minutes);
            mins = 1 * m(minutes); //change minutes here
            //alert(mins);
            secs = 0 + s(":01");   //change seconds here (always add an additional second to your total)
            //alert('d1');
            redo();
        }

        function m(obj) {
            for (var i = 0; i < obj.length; i++) {
                if (obj.substring(i, i + 1) == ":")
                    break;
            }

            return (obj.substring(0, i));
        }

        function s(obj) {
            for (var i = 0; i < obj.length; i++) {
                if (obj.substring(i, i + 1) == ":")
                    break;
            }

            return (obj.substring(i + 1, obj.length));
        }

        function dis(mins, secs) {
            var disp;
            if (mins <= 9) {
                disp = " 0";
            }
            else {
                disp = " ";
            }

            disp += mins + ":";
            if (secs <= 9) {
                disp += "0" + secs;
            }
            else {
                disp += secs;
            }

            return (disp);
        }

        function redo() {
            secs--;
            if (secs == -1) {
                secs = 59;
                mins--;
            }

            document.getElementById('status').innerHTML = "Count Down " + dis(mins, secs); // setup additional displays here.

            if ((mins == 0) && (secs == 0)) {
            }
            else {
                cd = setTimeout("redo()", 1000);
            }
        }


        function toggleBounce(marker) { if (marker.getAnimation() != null) { marker.setAnimation(null); } else { marker.setAnimation(google.maps.Animation.BOUNCE); } }

        $(function () {

            $('.datepicker').datepicker({ dateFormat: 'dd/mm/yy',
                changeMonth: true,
                changeYear: true,
                showOn: "button",
                buttonImage: "images/calendar.gif",
                buttonImageOnly: true

            });
        });
        function rad(x) { return x * Math.PI / 180; }

        //var gvar;

        //        function removeLines(poly) {
        //            alert(poly);
        //            if (poly != null) {
        //                for (var i = 0; i < poly.length; i++) {
        //                    poly[i].setMap(null);
        //                    // alert('d');
        //                }
        //            }
        //        }
        var service;
        var render;
        function drawRoute(lat, lng, dlat, dlng) {

            if (render != null)
                render.setMap(null);
            //removeLines(gvar);
            //removeLines();
            //directionDisplay.setMap(null);

            //var path = new google.maps.MVCArray();
            service = new google.maps.DirectionsService();
            //var poly = new google.maps.Polyline({ map: map, strokeColor: '#4986E7' });
            //alert(markers.length);
            var renderOpt = { map: map };

            render = new google.maps.DirectionsRenderer(renderOpt);
            //alert(lat);
            //alert(dlat);
            //alert(lng);
            //alert(dlng);
            var src = new google.maps.LatLng(lat, lng);
            var des = new google.maps.LatLng(dlat, dlng);
            //for (var i = 0; i < markers.length; i++) {
            //if ((i + 1) < markers.length) {
            // var src = markers[i].position;
            //var des = markers[i + 1].position;
            //                var des = markers[i].position;
            //                if (des.lat() == lat && des.lng() == lng) {
            //                    //alert('missed');
            //                    continue;
            //                }
            //path.push(src);
            //poly.setPath(path);

            service.route({

                origin: src,

                destination: des,

                travelMode: google.maps.DirectionsTravelMode.DRIVING

            }, function (result, status) {
                if (status == google.maps.DirectionsStatus.OK) {
                    render.setDirections(result);
                    //alert('set');
                    //                        for (var i = 0, len = result.routes[0].overview_path.length; i < len; i++) {
                    //                            path.push(result.routes[0].overview_path[i]);
                    //                            //alert('f');
                    //                        }


                }
            });
            //}

            //}

            //    gvar = path;
        }
        (function () {

            /**
            * Decimal adjustment of a number.
            *
            * @param	{String}	type	The type of adjustment.
            * @param	{Number}	value	The number.
            * @param	{Integer}	exp		The exponent (the 10 logarithm of the adjustment base).
            * @returns	{Number}			The adjusted value.
            */
            function decimalAdjust(type, value, exp) {
                // If the exp is undefined or zero...
                if (typeof exp === 'undefined' || +exp === 0) {
                    return Math[type](value);
                }
                value = +value;
                exp = +exp;
                // If the value is not a number or the exp is not an integer...
                if (isNaN(value) || !(typeof exp === 'number' && exp % 1 === 0)) {
                    return NaN;
                }
                // Shift
                value = value.toString().split('e');
                value = Math[type](+(value[0] + 'e' + (value[1] ? (+value[1] - exp) : -exp)));
                // Shift back
                value = value.toString().split('e');
                return +(value[0] + 'e' + (value[1] ? (+value[1] + exp) : exp));
            }

            // Decimal round
            if (!Math.round10) {
                Math.round10 = function (value, exp) {
                    return decimalAdjust('round', value, exp);
                };
            }
            // Decimal floor
            if (!Math.floor10) {
                Math.floor10 = function (value, exp) {
                    return decimalAdjust('floor', value, exp);
                };
            }
            // Decimal ceil
            if (!Math.ceil10) {
                Math.ceil10 = function (value, exp) {
                    return decimalAdjust('ceil', value, exp);
                };
            }

        })();

        function find_closest_marker(event) {
            var k;
            var idx;
            var atmTitles = document.getElementById('hiddenfield_title').value.split(',');            
            var atmStates = document.getElementById('hiddenfield_state').value.split(',');
            var minDistance = document.getElementById('distance').value;
            //alert('c');
            //alert(event);
            var text = "";
            var lat = event.latLng.lat();
            var lng = event.latLng.lng();

            //alert(lat);
            //alert(lng);
            var R = 6371; // radius of earth in km
            var distances = [];
            var closest = -1;
            //alert(map);
            //alert(markers[0]);

            for (i = 0; i < markers.length; i++) {
                var mlat = markers[i].position.lat();
                var mlng = markers[i].position.lng();
                if (mlat == lat && mlng == lng)
                    continue;
                var dLat = rad(mlat - lat);
                var dLong = rad(mlng - lng);
                var a = Math.sin(dLat / 2) * Math.sin(dLat / 2) +
            Math.cos(rad(lat)) * Math.cos(rad(lat)) * Math.sin(dLong / 2) * Math.sin(dLong / 2);
                var c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
                var d = R * c;
                if (minDistance.length > 0)
                    if (d > minDistance) {
                        //alert('ignored');
                        continue;
                    }
                distances[i] = d;
                if ( closest == -1 || d < distances[closest] ) {
                    //alert(atmTitles.length);
                    //alert(minDistance);
                   
                    for ( k = 0; k < atmTitles.length; k++) {
                       // alert(atmTitles[k]);
                        //alert(markers[i].title);
                        if (atmTitles[k] == markers[i].title) {
                            //alert('set');
                            idx = k;
                            break;
                        }
                    }

                    //alert(idx);                     //alert(atmStates);
             //       alert(atmStates[idx]);
                   if (atmStates[idx] == 1)
                        closest = i;
               //     else
                 //       alert('not set because atm is down');
                }
            }
            //distances.sort();
            document.getElementById('map-info').innerHTML = "<ol>";
            ///document.getElementById('map-info').innerHTML = "<h2 style='color:green'>ATM Nearest Locations</h2><br/>";
            for (var j = 0; j < distances.length; j++)
            // str += distances[j];
                if (distances[j] != null) {
                    //alert(markers[j].title);
                    //alert(atmTitles.length);
                    
                    //alert(atmTitle[0].indexOf("D"));
                    
                    //alert(atmTitles);
                    text += '<li><b>' + markers[j].title + ' is ' + Math.round10(distances[j], -1) + ' KM away' + '</b></li>';
                    //alert('c');
                
                }
            document.getElementById('map-info').innerHTML = text;

            document.getElementById('map-info').innerHTML += "</ol>";
            //alert(markers[closest].title);
            if (distances.length > 0)
                drawRoute(lat, lng, markers[closest].position.lat(), markers[closest].position.lng());
            else {
                if (render != null)
                    render.setMap(null);
            }
            //toggleBounce(markers[0]);
            //map.setZoom(9);
        }

        function getNearest(event) {
            var result = [];
            //we can easily skip document/placemark/point
            //var coords = kml.getElementsByTagName('coordinates');
            var lat = event.latLng.lat();
            var lng = event.latLng.lng();

            for (var i = 0; i < markers.length; i++) {
                //var points = coords[i].textContent.split(',');
                var mlat = markers[i].position.lat();
                var mlng = markers[i].position.lng();

                if ((Math.abs(mlat - lat) < 0.1) && (Math.abs(mlng - lng) < 0.1)) {
                    result.push(new google.maps.LatLng(mlat, mlng));
                    //alert(markers[i].title);
                }
            }
            return result;
        }
        function hidePanel
        () {

            $("#leftSide").animate({ width: 'toggle' }, 500);
            //document.getElementById('leftSide').click();
            //alert('click');
        }
        function setTimer() {
            window.setTimeout(hidePanel, 1000 * 20);
        }
        $(function () {

            $('#click').click(function () {
                //alert('d');
                $("#leftSide").animate({ width: 'toggle' }, 500);
                //$('map-canvas').width('1000px');
                //     pageRefresh();
            }

        );
        });
        //        function pageRefresh() {

        //            $('<%=imageButton_Search.ClientID %>').click();
        //            
        //        }
    </script>
    <asp:Literal ID="Literal_Script" runat="server"></asp:Literal>
    
    <link href="content/Scripts/ui-lightness/jquery-ui-1.7.2.custom.css" rel="stylesheet"
        type="text/css" />
    <script type="text/javascript" src="content/js/jquery.js"></script>
    <script type="text/javascript" src="content/scripts/ui.datepicker.js"></script>
</head>
<body style="border: 0; padding: 0">
    
    <%-- <ComponentArt:CallBack ID="DialogCallBack" runat="server" Width="0" Height="0" OnCallback="DialogCallBack_Callback">
    </ComponentArt:CallBack>--%>
    <div id="main" runat="server">
        <%--<div id="map-info" style="float:right">
            
    </div>--%>
        <%--<div id="header">
        <table style="width:100%"> 
            <tr>
                <td>
                    <img src="images/ncr.png" width="204" height="96" />
                </td>
                <td >
                    <b>NCR ATM Network Monitoring</b>
                </td>
                <td align="right" >
                <img src="images/adcb.png" />
                </td>
            </tr>
        </table>
    </div>--%>
        <div id="leftSide" style="width: 350px;" runat="server">
            <table width="100%" cellpadding="0" cellspacing="0" border="0">
                <tr>
                    <td style="background-color: #51A148; color: White; height: 30px" align="left" colspan="7">
                        GOOGLE Maps
                    </td>
                </tr>
                <tr>
                    <td>
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td>
                                    <form runat="server" id="form1">
                                    <asp:HiddenField ID="hiddenfield_title" runat="server" />
    <asp:HiddenField ID="hiddenfield_state" runat="server" />
    
                                    <table class="SEARCHBOX" width="100%">
                                        <tr>
                                            <td>
                                                <b>Language</b>
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="DropDownList_Language" runat="server" CssClass="dropDown" Width="150px">
                                                    <asp:ListItem Value="en">English</asp:ListItem>
                                                    <asp:ListItem Value="ar">Arabic</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <b>Hotspot Type</b>
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="DropDownList_HotspotType" runat="server" CssClass="dropDown"
                                                    Width="150px">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <b>Region</b>
                                            </td>
                                            <td>
                                                <asp:ListBox ID="listBox_Region" runat="server" CssClass="dropDown" AutoPostBack="true"
                                                    OnSelectedIndexChanged="listBox_Region_SelectedIndexChanged" SelectionMode="Multiple"
                                                    Width="150px"></asp:ListBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <b>ATM</b>
                                            </td>
                                            <td>
                                                <asp:ListBox ID="listBox_ATM" runat="server" CssClass="dropDown" SelectionMode="Multiple"
                                                    Width="150px"></asp:ListBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <b>From</b>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="TextBox_FromDate" runat="server" CssClass="datepicker"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <b>To</b>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="TextBox_To" runat="server" CssClass="datepicker"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <b>Distance</b>
                                            </td>
                                            <td>
                                            <input type="text" id="distance" class="textbox"
                                                <%--<asp:TextBox ID="TextBox_Distance" runat="server"  CssClass="textbox"></asp:TextBox>--%>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td colspan="2">
                                                <asp:ImageButton ID="imageButton_Search" runat="server" ImageUrl="images/bt-find.png"
                                                    OnClick="Button_RefereshHotSpot_Click" />
                                                <%--<asp:Button ID="Button_RefereshHotSpot" runat="server" Text="Refresh Hotspot Map"
                                                OnClick="Button_RefereshHotSpot_Click" CssClass="button" />--%>
                                            </td>
                                        </tr>
                                    </table>
                                    </form>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td style="font-family: Trebuchet MS">
                        <span style="font-size: small">ATM Nearest Locations</span>
                        <hr />
                        <span style="color: Green; font-size: small">Click on any ATM to find nearest locations</span>
                        <div id="map-info" style="overflow: auto; height: 100px; width: 325px">
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td style="font-family: Trebuchet MS">
                        <span style="font-size: small">Legends</span>
                        <hr />
                    </td>
                </tr>
                <tr>
                    <td>
                        <table>
                            <tr>
                                <td>
                                    <img src="images/atm_icon3.gif" />
                                </td>
                                <td>
                                    - Operational
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table>
                            <tr>
                                <td>
                                    <img src="images/atm_icon2.gif" />
                                </td>
                                <td>
                                    - Idle But Operational
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table>
                            <tr>
                                <td>
                                    <img src="images/atm_icon1.gif" />
                                </td>
                                <td>
                                    - Need Attention
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
       
    </div>
     <div id="click" runat="server">
        </div>
        
    <div id="status" style="position: relative; top: 0; left: 0;">
        hello</div>
   <div id="map-canvas" />
</body>
</html>
