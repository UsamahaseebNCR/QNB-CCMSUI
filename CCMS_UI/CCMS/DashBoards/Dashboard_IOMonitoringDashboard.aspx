<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Dashboard_IOMonitoringDashboard.aspx.cs"
    Inherits="CCMSUI.CCMS.Dashboard_IOMonitoringDashboard" %>

<%--<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">  --%>
<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<!DOCTYPE html>
<!--
This is a starter template page. Use this page to start your new project from
scratch. This page gets rid of all links and provides the needed markup only.
-->
<html>
<head>
    <meta http-equiv="Refresh" content="300" id="meta1" runat="server" />
    <script src="../../content/Scripts/jquery.min.js" type="text/javascript"></script>
    <link href="../../content/Scripts/bootstrap-dialog.css" rel="stylesheet" type="text/css" />
    <script src="../../content/Scripts/bootstrap-dialog.js" type="text/javascript"></script>
    <script type="text/javascript">

        var geocoder;
        var address1;


        function initMap() {
            
            var myLatlng = new google.maps.LatLng(40.779502, -73.967857);
            var myOptions = {
                zoom: 1,
                center: myLatlng,
                mapTypeId: google.maps.MapTypeId.ROADMAP
            }
            map = new google.maps.Map(document.getElementById("map-canvas"), myOptions);
            map.setTilt(45);

            setMarkers(map);
            var boundsListener = google.maps.event.addListener((map), 'bounds_changed', function (event) {
                this.setZoom(14);
                google.maps.event.removeListener(boundsListener);
            });
        }
        //google.maps.event.addDomListener(window, "load", initialize);

        function addMarker(location) {
            marker = new google.maps.Marker({
                position: location,
                map: map

            });
        }

        function setMarkers(map1) {

            var markers = [];
            index = 0;
            for (i = 0; i < ary.length; i++) {

                obj = new google.maps.LatLng(ary[i][0], ary[i][1]);
                marker = new google.maps.Marker(
                {
                position: obj,
                map:map1,
                title: ary[i][2] 
                });
                
            // alert(ary[i][2]);
              // marker.setMap(map);
                //alert(ary[i][2]);
                markers[index] = marker;
            }
            var bounds = new google.maps.LatLngBounds();
            for (var i = 0; i < markers.length; i++) {
                bounds.extend(markers[i].getPosition());
            }
            map1.fitBounds(bounds);
            
        }


    </script>
    <script type="text/javascript">
        function loadScript() {
            var script = document.createElement('script');
            script.type = 'text/javascript';
            script.src = 'https://maps.googleapis.com/maps/api/js?v=3.exp&key=AIzaSyDsfUqmbm0Kk8r-xn3p2KPlQiutiNmaUL4&' + 'callback=initMap';
            document.body.appendChild(script);

        }

        //google.maps.event.addDomListener(window, 'load', LoadGoogleMAP);  
  
    </script>
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <title>NCR Comms Vs Power COMPONENTS Monitoring Dashboard</title>
    <!-- Tell the browser to be responsive to screen width -->
    <meta content="width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no"
        name="viewport">
    <!-- Bootstrap 3.3.6 -->
    <link rel="stylesheet" href="../../content/bootstrap/css/bootstrap.min.css">
    <!-- Font Awesome -->
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.5.0/css/font-awesome.min.css">
    <!-- Ionicons -->
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/ionicons/2.0.1/css/ionicons.min.css">
    <!-- Theme style -->
    <link rel="stylesheet" href="../../content/css/AdminLTE.min.css">
    <!-- AdminLTE Skins. We have chosen the skin-blue for this starter
        page. However, you can choose any other skin. Make sure you
        apply the skin class to the body tag so the changes take effect.
  -->
    <link rel="stylesheet" href="../../content/skins/skin-green.min.css">
    <!-- HTML5 Shim and Respond.js IE8 support of HTML5 elements and media queries -->
    <!-- WARNING: Respond.js doesn't work if you view the page via file:// -->
    <!--[if lt IE 9]>
  <script src="https://oss.maxcdn.com/html5shiv/3.7.3/html5shiv.min.js"></script>
  <script src="https://oss.maxcdn.com/respond/1.4.2/respond.min.js"></script>
  <![endif]-->
    <style type="text/css">
        html, body, #canvasMap
        {
            margin: 0 auto;
            height: 100%;
            
            padding: 0px;
        }

.col-xs-15,
.col-sm-15,
.col-md-15,
.col-lg-15 {
    position: relative;
    min-height: 1px;
    padding-right: 10px;
    padding-left: 10px;
}

.col-xs-15 {
    width: 20%;
    float: left;
}

@media (min-width: 768px) {
.col-sm-15 {
        width: 20%;
        float: left;
    }
}

@media (min-width: 992px) {
    .col-md-15 {
        width: 20%;
        float: left;
    }
}

@media (min-width: 1200px) {
    .col-lg-15 {
        width: 20%;
        float: left;
    }
}
    </style>
    
    <script type="text/javascript">
        function modalWindow(value, type) {
            DialogCallBackCustomerDetail.callback(value, type);

            //  BootstrapDialog.alert('I want banana!');
        }
        function openDialog(value, type) {
            //
            // document.getElementById('trxnID').value = capturedTrxnID;
            // alert('d');
            DialogCallBackCustomerDetail.callback(value, type);
            DialogCustomerDetail.Show();
            // alert('d1');

        }
        function receiveCallBackResult(response, val) {
            // document.getElementById('Detail').innerHTML = response;
            document.getElementById('modalwindowserver').innerHTML = response;
            $("#registermodal").modal('show');
            if (val == 1) {
                $("#registermodal").css('width', '500');
            }
            else
                $("#registermodal").css('width', '700');

            $('#registermodal').draggable({
                handle: ".modal-header"
            });
        }
        $(document).ready(function () {
            $(".mytext").popover();

        });

    </script>
    <style type="text/css">
        .popover
        {
            width: 100px;
            height: 35px;
        }
        .EU_TableScroll
        {
        }
        .EU_DataTable
        {
            border-collapse: collapse;
            width: 100%;
            color: White;
            
        }
        .EU_DataTable tr th
        {
            background-color: green;
            padding: 3px 5px 10px 5px;
            border: 1px solid #cccccc;
            font-family: Calibri;
            font-size: 12px;
            font-weight: normal;
            text-transform: capitalize;
        }
        .EU_DataTable tr:nth-child(2n+2)
        {
            background-color: #f3f4f5;
        }
        
        .EU_DataTable tr:nth-child(2n+1) td
        {
            background-color: #d6dadf;
            color: #454545;
        }
        .EU_DataTable tr td
        {
            padding: 5px 10px 5px 10px;
            color: #454545;
            font-family: Arial, Helvetica, sans-serif;
            font-size: 11px;
            border: 1px solid #cccccc;
            vertical-align: middle;
        }
        .EU_DataTable tr td:first-child
        {
            text-align: center;
        }
    </style>
</head>
<!--
BODY TAG OPTIONS:
=================
Apply one or more of the following classes to get the
desired effect
|---------------------------------------------------------|
| SKINS         | skin-blue                               |
|               | skin-black                              |
|               | skin-purple                             |
|               | skin-yellow                             |
|               | skin-red                                |
|               | skin-green                              |
|---------------------------------------------------------|
|LAYOUT OPTIONS | fixed                                   |
|               | layout-boxed                            |
|               | layout-top-nav                          |
|               | sidebar-collapse                        |
|               | sidebar-mini                            |
|---------------------------------------------------------|
-->
<body class="hold-transition skin-green sidebar-mini" onload="loadScript();">
<asp:Literal ID="literal_jsArray" runat="server" EnableViewState="false" />
    <div class="navbar navbar-inverse navbar-fixed-top" role="navigation">
        <div>
            <div class="navbar-header">
                <span style="color: white; font-size: xx-large">
                    <asp:Label ID="Label_Title" runat="server" Text="NCR Comms Vs Power COMPONENTS Monitoring Dashboard"></asp:Label>
                </span>
            </div>
        </div>
    </div>
    <div style="margin-top: 50px">
        <!-- Main Header -->
        <!-- Left side column. contains the logo and sidebar -->
        <!-- Content Wrapper. Contains page content -->
        <div>
            <!-- Content Header (Page header) -->
            <!-- Main content -->
            <section class="content">

	 

   <center><b><h1 style="font-family:Verdana; font-weight:bold">Comms Component Monitoring</h1></b></center>  
      	  <div class="row">
        <div class="col-md-15 col-xs-3">
          <!-- small box -->
         
      <asp:Chart ID="Chart_Link1" runat="server" >
                                                <Titles>
                                                    <asp:Title Font="Trebuchet MS, 14.25pt, style=Bold" Text="Link 1 Current Status" Name="Title1" ForeColor="26, 59, 105">
                                                    </asp:Title>
                                                </Titles>
                                                <Legends>
                                                    <asp:Legend BackColor="Transparent" Alignment="Center" Docking="Right" Font="Trebuchet MS, 8.25pt, style=Bold"
                                                        IsTextAutoFit="False" Name="Default" LegendStyle="Column" Title="Legends" Enabled="false">
                                                    </asp:Legend>
                                                </Legends>
                                                <BorderSkin SkinStyle="None"></BorderSkin>
                                                <Series>
                                                    <asp:Series Name="Default" YValuesPerPoint="2" ChartType="Doughnut" IsValueShownAsLabel="true"
                                                        IsVisibleInLegend="true" >
                                                    </asp:Series>
                                                </Series>
                                                <ChartAreas>
                                                    <asp:ChartArea Name="ChartArea1" BackColor="GhostWhite" Area3DStyle-Enable3D="true">
                                                        <Area3DStyle Rotation="5" />
                                                        <AxisY LineColor="64, 64, 64, 64">
                                                            <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                            <MajorGrid LineColor="LightGray" />
                                                        </AxisY>
                                                        <AxisX LineColor="64, 64, 64, 64">
                                                            <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                            <MajorGrid LineColor="LightGray" />
                                                        </AxisX>
                                                    </asp:ChartArea>
                                                </ChartAreas>
                                            </asp:Chart>

        </div>
        <!-- ./col -->
        
        <!-- ./col -->
        <div class="col-md-15 col-xs-3">
          <!-- small box -->
      <asp:Chart ID="Chart_Link2" runat="server" >
                                                <Titles>
                                                    <asp:Title Font="Trebuchet MS, 14.25pt, style=Bold" Name="Title1" Text="Link 2 Current Status" ForeColor="26, 59, 105">
                                                    
                                                    </asp:Title>
                                                </Titles>
                                                <Legends>
                                                    <asp:Legend BackColor="Transparent" Alignment="Center" Docking="Right" Font="Trebuchet MS, 8.25pt, style=Bold"
                                                        IsTextAutoFit="False" Name="Default" LegendStyle="Row" Title="Legends" Enabled="false">
                                                    </asp:Legend>
                                                </Legends>
                                                <BorderSkin SkinStyle="None"></BorderSkin>
                                                <Series>
                                                    <asp:Series Name="Default" YValuesPerPoint="2" ChartType="Doughnut" IsValueShownAsLabel="true"
                                                        IsVisibleInLegend="true">
                                                    </asp:Series>
                                                </Series>
                                                <ChartAreas>
                                                    <asp:ChartArea Name="ChartArea1" BackColor="White">
                                                        <Area3DStyle Rotation="0" />
                                                        <AxisY LineColor="64, 64, 64, 64" Interval="1">
                                                            <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                            <MajorGrid LineColor="LightGray" />
                                                        </AxisY>
                                                        <AxisX LineColor="64, 64, 64, 64" Interval="1">
                                                            <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                            <MajorGrid LineColor="LightGray" />
                                                        </AxisX>
                                                    </asp:ChartArea>
                                                </ChartAreas>
                                            </asp:Chart>    
        </div>
        <!-- ./col -->
        
         <div class="col-md-15 col-xs-3">
          <!-- small box -->
          <asp:Chart ID="Chart_Pinger1" runat="server" >
                                                <Titles>
                                                    <asp:Title Font="Trebuchet MS, 14.25pt, style=Bold" Name="Title1" Text=" Pinger 1 Current Status" ForeColor="26, 59, 105">
                                                    </asp:Title>
                                                </Titles>
                                                <Legends>
                                                    <asp:Legend BackColor="Transparent" Alignment="Center" Docking="Right" Font="Trebuchet MS, 8.25pt, style=Bold"
                                                        IsTextAutoFit="False" Name="Default" LegendStyle="Column" Title="Legends" Enabled="false">
                                                    </asp:Legend>
                                                </Legends>
                                                <BorderSkin SkinStyle="None"></BorderSkin>
                                                <Series>
                                                    <asp:Series Name="Default" YValuesPerPoint="2" ChartType="Doughnut" IsValueShownAsLabel="true"
                                                        IsVisibleInLegend="true">
                                                    </asp:Series>
                                                </Series>
                                                <ChartAreas>
                                                    <asp:ChartArea Name="ChartArea1" BackColor="GhostWhite" Area3DStyle-Enable3D="true" >
                                                        <Area3DStyle Rotation="0" />
                                                        <AxisY LineColor="64, 64, 64, 64" Interval="3">
                                                            <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                            
                                                            <MajorGrid LineColor="LightGray" />
                                                        </AxisY>
                                                        <AxisX LineColor="64, 64, 64, 64" Interval="3">
                                                            <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                            <MajorGrid LineColor="LightGray" />
                                                        </AxisX>
                                                    </asp:ChartArea>
                                                </ChartAreas>
                                            </asp:Chart>
        </div>
      
        <div 
        class="col-md-15 col-xs-3">
          <!-- small box -->
          <asp:Chart ID="Chart_Pinger2" runat="server"  >
                                                <Titles>
                                                    <asp:Title Font="Trebuchet MS, 14.25pt, style=Bold" Name="Title1" Text="Pinger 2 Current Status" ForeColor="26, 59, 105">
                                                    </asp:Title>
                                                </Titles>
                                                <Legends>
                                                    <asp:Legend BackColor="Transparent" Alignment="Center" Docking="Right" Font="Trebuchet MS, 8.25pt, style=Bold"
                                                        IsTextAutoFit="False" Name="Default" LegendStyle="Column" Title="Legends" Enabled="false">
                                                    </asp:Legend>
                                                </Legends>
                                                <BorderSkin SkinStyle="None"></BorderSkin>
                                                <Series>
                                                    <asp:Series Name="Default" YValuesPerPoint="2" ChartType="Doughnut" IsValueShownAsLabel="true"
                                                        IsVisibleInLegend="true">
                                                    </asp:Series>
                                                </Series>
                                                <ChartAreas>
                                                    <asp:ChartArea Name="ChartArea1" BackColor="White">
                                                        <Area3DStyle Rotation="0" />
                                                        <AxisY LineColor="64, 64, 64, 64" Interval="3">
                                                            <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                            <MajorGrid LineColor="LightGray" />
                                                        </AxisY>
                                                        <AxisX LineColor="64, 64, 64, 64" Interval="3">
                                                            <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                            <MajorGrid LineColor="LightGray" />
                                                        </AxisX>
                                                    </asp:ChartArea>
                                                </ChartAreas>
                                            </asp:Chart>
      
          
      
        </div>
      <div 
        class="col-md-15 col-xs-3">
          <!-- small box -->
          <asp:Chart ID="Chart_Comms" runat="server"  >
                                                <Titles>
                                                    <asp:Title Font="Trebuchet MS, 14.25pt, style=Bold" Name="Title1" Text="Comms Current Status" ForeColor="26, 59, 105">
                                                    </asp:Title>
                                                </Titles>
                                                <Legends>
                                                    <asp:Legend BackColor="Transparent" Alignment="Center" Docking="Right" Font="Trebuchet MS, 8.25pt, style=Bold"
                                                        IsTextAutoFit="False" Name="Default" LegendStyle="Column" Title="Legends" Enabled="false">
                                                    </asp:Legend>
                                                </Legends>
                                                <BorderSkin SkinStyle="None"></BorderSkin>
                                                <Series>
                                                    <asp:Series Name="Default" YValuesPerPoint="2" ChartType="Doughnut" IsValueShownAsLabel="true"
                                                        IsVisibleInLegend="true">
                                                    </asp:Series>
                                                </Series>
                                                <ChartAreas>
                                                    <asp:ChartArea Name="ChartArea1" BackColor="White">
                                                        <Area3DStyle Rotation="0" />
                                                        <AxisY LineColor="64, 64, 64, 64" Interval="3">
                                                            <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                            <MajorGrid LineColor="LightGray" />
                                                        </AxisY>
                                                        <AxisX LineColor="64, 64, 64, 64" Interval="3">
                                                            <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                            <MajorGrid LineColor="LightGray" />
                                                        </AxisX>
                                                    </asp:ChartArea>
                                                </ChartAreas>
                                            </asp:Chart>
      
          
      
        </div>
        <!-- ./col -->
      
      </div>

   <center><b><h1 style="font-family:Verdana; font-weight:bold">Power Component Monitoring</h1></b></center>  
     

      	  <div class="row">
        
        <div class="col-md-15 col-xs-3">
          <!-- small box -->
         
      <asp:Chart ID="Chart_IO1" runat="server" >
                                                <Titles>
                                                    <asp:Title Font="Trebuchet MS, 14.25pt, style=Bold" Text="IO 1 Current Status" Name="Title1" ForeColor="26, 59, 105">
                                                    </asp:Title>
                                                </Titles>
                                                <Legends>
                                                    <asp:Legend BackColor="Transparent" Alignment="Center" Docking="Right" Font="Trebuchet MS, 8.25pt, style=Bold"
                                                        IsTextAutoFit="False" Name="Default" LegendStyle="Column" Title="Legends" Enabled="false">
                                                    </asp:Legend>
                                                </Legends>
                                                <BorderSkin SkinStyle="None"></BorderSkin>
                                                <Series>
                                                    <asp:Series Name="Default" YValuesPerPoint="2" ChartType="Doughnut" IsValueShownAsLabel="true"
                                                        IsVisibleInLegend="true" >
                                                    </asp:Series>
                                                </Series>
                                                <ChartAreas>
                                                    <asp:ChartArea Name="ChartArea1" BackColor="GhostWhite" Area3DStyle-Enable3D="true">
                                                        <Area3DStyle Rotation="5" />
                                                        <AxisY LineColor="64, 64, 64, 64">
                                                            <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                            <MajorGrid LineColor="LightGray" />
                                                        </AxisY>
                                                        <AxisX LineColor="64, 64, 64, 64">
                                                            <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                            <MajorGrid LineColor="LightGray" />
                                                        </AxisX>
                                                    </asp:ChartArea>
                                                </ChartAreas>
                                            </asp:Chart>

        </div>
        <!-- ./col -->
        
        <!-- ./col -->
        <div class="col-md-15 col-xs-3">
          <!-- small box -->
      <asp:Chart ID="Chart_IO2" runat="server" >
                                                <Titles>
                                                    <asp:Title Font="Trebuchet MS, 14.25pt, style=Bold" Name="Title1" Text="IO 2 Current Status" ForeColor="26, 59, 105">
                                                    
                                                    </asp:Title>
                                                </Titles>
                                                <Legends>
                                                    <asp:Legend BackColor="Transparent" Alignment="Center" Docking="Right" Font="Trebuchet MS, 8.25pt, style=Bold"
                                                        IsTextAutoFit="False" Name="Default" LegendStyle="Row" Title="Legends" Enabled="false">
                                                    </asp:Legend>
                                                </Legends>
                                                <BorderSkin SkinStyle="None"></BorderSkin>
                                                <Series>
                                                    <asp:Series Name="Default" YValuesPerPoint="2" ChartType="Doughnut" IsValueShownAsLabel="true"
                                                        IsVisibleInLegend="true">
                                                    </asp:Series>
                                                </Series>
                                                <ChartAreas>
                                                    <asp:ChartArea Name="ChartArea1" BackColor="White">
                                                        <Area3DStyle Rotation="0" />
                                                        <AxisY LineColor="64, 64, 64, 64" Interval="1">
                                                            <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                            <MajorGrid LineColor="LightGray" />
                                                        </AxisY>
                                                        <AxisX LineColor="64, 64, 64, 64" Interval="1">
                                                            <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                            <MajorGrid LineColor="LightGray" />
                                                        </AxisX>
                                                    </asp:ChartArea>
                                                </ChartAreas>
                                            </asp:Chart>    
        </div>
        <!-- ./col -->
        
         <div class="col-md-15 col-xs-3">
          <!-- small box -->
          <asp:Chart ID="Chart_IO3" runat="server" >
                                                <Titles>
                                                    <asp:Title Font="Trebuchet MS, 14.25pt, style=Bold" Name="Title1" Text=" IO 3 Current Status" ForeColor="26, 59, 105">
                                                    </asp:Title>
                                                </Titles>
                                                <Legends>
                                                    <asp:Legend BackColor="Transparent" Alignment="Center" Docking="Right" Font="Trebuchet MS, 8.25pt, style=Bold"
                                                        IsTextAutoFit="False" Name="Default" LegendStyle="Column" Title="Legends" Enabled="false">
                                                    </asp:Legend>
                                                </Legends>
                                                <BorderSkin SkinStyle="None"></BorderSkin>
                                                <Series>
                                                    <asp:Series Name="Default" YValuesPerPoint="2" ChartType="Doughnut" IsValueShownAsLabel="true"
                                                        IsVisibleInLegend="true">
                                                    </asp:Series>
                                                </Series>
                                                <ChartAreas>
                                                    <asp:ChartArea Name="ChartArea1" BackColor="GhostWhite" Area3DStyle-Enable3D="true" >
                                                        <Area3DStyle Rotation="0" />
                                                        <AxisY LineColor="64, 64, 64, 64" Interval="3">
                                                            <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                            
                                                            <MajorGrid LineColor="LightGray" />
                                                        </AxisY>
                                                        <AxisX LineColor="64, 64, 64, 64" Interval="3">
                                                            <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                            <MajorGrid LineColor="LightGray" />
                                                        </AxisX>
                                                    </asp:ChartArea>
                                                </ChartAreas>
                                            </asp:Chart>
        </div>
      
        <div class="col-md-15 col-xs-3">
          <!-- small box -->
          <asp:Chart ID="Chart_Device" runat="server"  >
                                                <Titles>
                                                    <asp:Title Font="Trebuchet MS, 14.25pt, style=Bold" Name="Title1" Text=" Device Current Status" ForeColor="26, 59, 105">
                                                    </asp:Title>
                                                </Titles>
                                                <Legends>
                                                    <asp:Legend BackColor="Transparent" Alignment="Center" Docking="Right" Font="Trebuchet MS, 8.25pt, style=Bold"
                                                        IsTextAutoFit="False" Name="Default" LegendStyle="Column" Title="Legends" Enabled="false">
                                                    </asp:Legend>
                                                </Legends>
                                                <BorderSkin SkinStyle="None"></BorderSkin>
                                                <Series>
                                                    <asp:Series Name="Default" YValuesPerPoint="2" ChartType="Doughnut" IsValueShownAsLabel="true"
                                                        IsVisibleInLegend="true">
                                                    </asp:Series>
                                                </Series>
                                                <ChartAreas>
                                                    <asp:ChartArea Name="ChartArea1" BackColor="White">
                                                        <Area3DStyle Rotation="0" />
                                                        <AxisY LineColor="64, 64, 64, 64" Interval="3">
                                                            <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                            <MajorGrid LineColor="LightGray" />
                                                        </AxisY>
                                                        <AxisX LineColor="64, 64, 64, 64" Interval="3">
                                                            <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                            <MajorGrid LineColor="LightGray" />
                                                        </AxisX>
                                                    </asp:ChartArea>
                                                </ChartAreas>
                                            </asp:Chart>
      
          
      
        </div>
        <!-- ./col -->
  <div class="col-md-15 col-xs-3">
          <!-- small box -->
          <asp:Chart ID="Chart_Power" runat="server"  >
                                                <Titles>
                                                    <asp:Title Font="Trebuchet MS, 14.25pt, style=Bold" Name="Title1" Text=" Power Current Status" ForeColor="26, 59, 105">
                                                    </asp:Title>
                                                </Titles>
                                                <Legends>
                                                    <asp:Legend BackColor="Transparent" Alignment="Center" Docking="Right" Font="Trebuchet MS, 8.25pt, style=Bold"
                                                        IsTextAutoFit="False" Name="Default" LegendStyle="Column" Title="Legends" Enabled="false">
                                                    </asp:Legend>
                                                </Legends>
                                                <BorderSkin SkinStyle="None"></BorderSkin>
                                                <Series>
                                                    <asp:Series Name="Default" YValuesPerPoint="2" ChartType="Doughnut" IsValueShownAsLabel="true"
                                                        IsVisibleInLegend="true">
                                                    </asp:Series>
                                                </Series>
                                                <ChartAreas>
                                                    <asp:ChartArea Name="ChartArea1" BackColor="White">
                                                        <Area3DStyle Rotation="0" />
                                                        <AxisY LineColor="64, 64, 64, 64" Interval="3">
                                                            <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                            <MajorGrid LineColor="LightGray" />
                                                        </AxisY>
                                                        <AxisX LineColor="64, 64, 64, 64" Interval="3">
                                                            <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                            <MajorGrid LineColor="LightGray" />
                                                        </AxisX>
                                                    </asp:ChartArea>
                                                </ChartAreas>
                                            </asp:Chart>
      
          
      
        </div>    
      </div>
  
   
      <!-- /.row -->
      <!-- Main row -->
      <!-- Your Page Content Here -->
     
	  
     
    </section>
   
            <!-- /.content -->
        </div>
        <!-- /.content-wrapper -->
        <!-- Control Sidebar -->
        <!-- /.control-sidebar -->
        <!-- Add the sidebar's background. This div must be placed
       immediately after the control sidebar -->
   
    </div>
    <!-- ./wrapper -->
    <!-- REQUIRED JS SCRIPTS -->
    <!-- jQuery 2.2.3 -->
    
        
    <script src="../../content/js/jquery-2.2.3.min.js"></script>
    <!-- Bootstrap 3.3.6 -->
    <script src="../../content/bootstrap/js/bootstrap.min.js"></script>
    <!-- AdminLTE App -->
    <script src="../../content/js/app.min.js"></script>
    <!-- Optionally, you can add Slimscroll and FastClick plugins.
     Both of these plugins are recommended to enhance the
     user experience. Slimscroll is required when using the
     fixed layout. -->
</body>
<ComponentArt:Dialog ContentCssClass="contentCss" FooterCssClass="footerCss" HeaderCssClass="headerCss"
    CssClass="dialogCss" Icon="" Value="" Title="AJAX TabStrip Content" ModalMaskCssClass="modalMaskCssClass"
    AllowDrag="false" Alignment="MiddleCentre" Modal="true" ID="DialogCustomerDetail"
    runat="server" Width="830px" ContentClientTemplateId="ClientTemplateCustomerDetail"
    AnimationType="None" ShowTransition="None" TransitionDuration="0" AnimationDuration="0"
    Height="250">
    <ClientTemplates>
        <ComponentArt:ClientTemplate ID="ClientTemplateCustomerDetail">
            <table cellpadding="0" style="border-color: #c9c9c9" cellspacing="0" border="0" width="100%"
                onmousedown="DialogCustomerDetail.StartDrag(event);">
                <tr>
                    <td style="background-image: url(../../images/DialogImages/top.gif); height: 45px;">
                        <table width="100%" cellpadding="0" cellspacing="0" border="0">
                            <tr>
                                <td valign="middle" style="padding-left: 4px; color: white; font-size: 15px; font-family: Verdana;
                                    font-weight: bold;">
                                    Detail
                                </td>
                                <td align="right" style="padding-right: 10px;">
                                    <img src="../../images/DialogImages/close.gif" onclick="DialogCustomerDetail.Close();" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td style="border-color: #c9c9c9; background-color: white; font-size: 12px; font-family: Arial;
                        padding: 10px;" colspan="2">
                        <div id="Detail" style="overflow: auto; height: 270px">
                        </div>
                    </td>
                </tr>
            </table>
        </ComponentArt:ClientTemplate>
    </ClientTemplates>
</ComponentArt:Dialog>
<ComponentArt:CallBack ID="DialogCallBackCustomerDetail" runat="server" Width="0"
    Height="0" OnCallback="DialogCallBackDetail_Callback">
</ComponentArt:CallBack>
</html>
