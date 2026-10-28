<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DashBoardGrid_PowerMonitoring.aspx.cs"
    Inherits="CCMSUI.CCMS.DashBoards.DashBoardGrid_PowerMonitoring" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <!-- Bootstrap theme -->
    <link href="../../content/styleSheets/bootstrap-theme.min.css" rel="stylesheet">
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <title>
        <asp:Literal ID="Literal_AddHeading" runat="server"></asp:Literal>
    </title>
    <style type="text/css">
        .mGrid
        {
            width: 100%;
            background-color: #fff;
            margin: 5px 0 10px 0;
            border: solid 1px #525252;
            border-collapse: collapse;
        }
        .mGrid td
        {
            padding: 2px;
            border: solid 1px #c1c1c1;
            color: #717171;
        }
        .mGrid th
        {
            padding: 4px 2px;
            color: #fff;
            background: #424242 url(grd_head.png) repeat-x top;
            border-left: solid 1px #525252;
            font-size: 0.9em;
        }
        .mGrid .alt
        {
            background: #fcfcfc url(grd_alt.png) repeat-x top;
        }
        .mGrid .pgr
        {
            background: #424242 url(grd_pgr.png) repeat-x top;
        }
        .mGrid .pgr table
        {
            margin: 5px 0;
        }
        .mGrid .pgr td
        {
            border-width: 0;
            padding: 0 6px;
            border-left: solid 1px #666;
            font-weight: bold;
            color: #fff;
            line-height: 12px;
        }
        .mGrid .pgr a
        {
            color: #666;
            text-decoration: none;
        }
        .mGrid .pgr a:hover
        {
            color: #000;
            text-decoration: none;
        }
        .mGrid
        {
            width: 100%;
            background-color: #fff;
            margin: 5px 0 10px 0;
            border: solid 1px #525252;
            border-collapse: collapse;
        }
        .mGrid td
        {
            padding: 2px;
            border: solid 1px #c1c1c1;
            color: #717171;
        }
        .mGrid th
        {
            padding: 4px 2px;
            color: #fff;
            background: #424242 url(grd_head.png) repeat-x top;
            border-left: solid 1px #525252;
            font-size: 0.9em;
        }
        .mGrid .alt
        {
            background: #fcfcfc url(grd_alt.png) repeat-x top;
        }
        .mGrid .pgr
        {
            background: #424242 url(grd_pgr.png) repeat-x top;
        }
        .mGrid .pgr table
        {
            margin: 5px 0;
        }
        .mGrid .pgr td
        {
            border-width: 0;
            padding: 0 6px;
            border-left: solid 1px #666;
            font-weight: bold;
            color: #fff;
            line-height: 12px;
        }
        .mGrid .pgr a
        {
            color: #666;
            text-decoration: none;
        }
        .mGrid .pgr a:hover
        {
            color: #000;
            text-decoration: none;
        }
    </style>
    <meta http-equiv="Refresh" content="300" id="metaRefresh" runat="server" />
    <script src="../../content/boxover.js" type="text/javascript" language="javascript">
    </script>
    <link href="../../content/styleSheets/bootstrap.min.css" rel="stylesheet">
    <link href="../../content/styleSheets/bootstrap-theme.min.css" rel="stylesheet">
    <link href="../../content/iSuite.css" rel="stylesheet" />
    <link href="../../content/cal/popcalendar.css" type="text/css" rel="stylesheet" />
    <script src="../../content/boxover.js" type="text/javascript" language="javascript">
    </script>
    <script language="javascript" src="../../content/cal/popcalendar.js" type="text/javascript"></script>
    <script src="../../content/jquery-1.4.4.min.js" type="text/javascript"></script>
    <script src="../../content/ScrollableGridPlugin.js" type="text/javascript"></script>
    <script type="text/javascript">
        function ShowDialog(title, fileStatus) {
            Dialog_ShowListOfFiles.Show();
            DialogCallBack_ShowListOfFiles.callback(title, fileStatus);
        }

        var mins;
        var secs;

        function cd(minutes) {
            mins = 1 * m(minutes); //change minutes here
            secs = 0 + s(":01");   //change seconds here (always add an additional second to your total)
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

            window.status = "Count Down " + dis(mins, secs); // setup additional displays here.

            if ((mins == 0) && (secs == 0)) {
            }
            else {
                cd = setTimeout("redo()", 1000);
            }
        }

        function ToggleOptionView() {
            var optionsPanel = document.getElementById('optionsPanel');

            if (optionsPanel.style.display == 'none') {
                optionsPanel.style.display = 'block';
                document.getElementById('viewButton').value = 'Hide Options';
            }
            else {
                optionsPanel.style.display = 'none';
                document.getElementById('viewButton').value = 'Show Options';
            }
        }
    </script>
</head>
<body role="document">
    <form id="form1" runat="server">
    <div class="navbar navbar-inverse navbar-fixed-top" role="navigation">
        <div class="container">
            <div class="navbar-header">
                <span style="color: white; font-size: xx-large"><asp:Label ID="Label_Title" runat="server" Text="NCR Power Monitoring Dashboard"></asp:Label> </span>
            </div>
        </div>
    </div>
    <div class="container theme-showcase" role="main">
        <table width="100%" border="0" style="margin-top: 50px">
            <tr>
                <td style="width: 300px">
                    <table width="100%" border="0" style="color: Black">
                        <tr>
                            <td>
                                
                                <asp:literal id="Literal_progressBar" runat="server"></asp:literal>
                                                           </td>
                        </tr>
                    </table>
                </td>
                <td align="center" valign="top">
                    <asp:chart id="ChartBar" runat="server" width="425px" palette="BrightPastel">
                                                <Titles>
                                                    <asp:Title Font="Trebuchet MS, 14.25pt, style=Bold" Name="Title1" ForeColor="26, 59, 105">
                                                    </asp:Title>
                                                </Titles>
                                                <Legends>
                                                    <asp:Legend BackColor="Transparent" Alignment="Center" Docking="Bottom" Font="Trebuchet MS, 8.25pt, style=Bold"
                                                        IsTextAutoFit="False" Name="Default" LegendStyle="Column" Title="Legends">
                                                    </asp:Legend>
                                                </Legends>
                                                <BorderSkin SkinStyle="None"></BorderSkin>
                                                <Series>
                                                    <asp:Series Name="Default" YValuesPerPoint="2" ChartType="Column" IsValueShownAsLabel="true"
                                                        IsVisibleInLegend="true"  >
                                                    </asp:Series>
                                                </Series>
                                                <ChartAreas>
                                                    <asp:ChartArea Name="ChartArea1" Area3DStyle-Enable3D="true">
                                                        <Area3DStyle Rotation="0" />
                                                        <AxisY LineColor="64, 64, 64, 64">
                                                            <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                            <MajorGrid LineColor="64, 64, 64, 64" />
                                                        </AxisY>
                                                        <AxisX LineColor="64, 64, 64, 64">
                                                            <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                            <MajorGrid LineColor="64, 64, 64, 64" />
                                                        </AxisX>
                                                    </asp:ChartArea>
                                                </ChartAreas>
                                            </asp:chart>
                </td>
                <td>
                    <asp:chart id="ChartPie" runat="server" width="425">
                                                <Titles>
                                                    <asp:Title Font="Trebuchet MS, 14.25pt, style=Bold" Name="Title1" ForeColor="26, 59, 105">
                                                    </asp:Title>
                                                </Titles>
                                                <Legends>
                                                    <asp:Legend BackColor="Transparent" Alignment="Center" Docking="Bottom" Font="Trebuchet MS, 8.25pt, style=Bold"
                                                        IsTextAutoFit="False" Name="Default" LegendStyle="Column" Title="Legends" Enabled="false">
                                                    </asp:Legend>
                                                </Legends>
                                                <BorderSkin SkinStyle="None"></BorderSkin>
                                                <Series>
                                                    <asp:Series Name="Default" YValuesPerPoint="2" ChartType="Pie" IsValueShownAsLabel="true"
                                                        IsVisibleInLegend="true">
                                                    </asp:Series>
                                                </Series>
                                                <ChartAreas>
                                                    <asp:ChartArea Name="ChartArea1" BackColor="White">
                                                        <Area3DStyle Rotation="0" />
                                                        <AxisY LineColor="64, 64, 64, 64">
                                                            <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                            <MajorGrid LineColor="White" />
                                                        </AxisY>
                                                        <AxisX LineColor="64, 64, 64, 64">
                                                            <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                            <MajorGrid LineColor="White" />
                                                        </AxisX>
                                                    </asp:ChartArea>
                                                </ChartAreas>
                                            </asp:chart>
                </td>
            </tr>
        </table>
        <asp:literal id="Literal_errorMsg" runat="server" enableviewstate="false"></asp:literal>
        <table width="100%" cellpadding="0" cellspacing="0">
            <tr>
                <td style="vertical-align: top; width: 100%">
                    <asp:gridview id="GridView_PowerMonitoring" runat="server" autogeneratecolumns="False"
                        font-size="Smaller" width="100%" onrowdatabound="GridView_PowerMonitoring_RowDataBound"
                        rowstyle-horizontalalign="Center" cssclass="mGrid" alternatingrowstyle-cssclass="alt"
                        gridlines="None" AllowSorting="true" OnSorting="GridView_PowerMonitoring_Sorting">   <%--Edited by Ali Shah on 6th April, 2016   Sorting Column in Communication Dashboard--%>
                        <HeaderStyle ForeColor="white" Width="13"  Font-Size="10px" HorizontalAlign="Center"/>
                        <EmptyDataTemplate>
                            <center>
                                No Rows Found</center>
                        </EmptyDataTemplate>
                        <Columns>
                            <asp:BoundField DataField="title" HeaderText="ATM" SortExpression="title" ItemStyle-Width="5%">
                            </asp:BoundField>
                            <%--  <asp:TemplateField HeaderText="Interface1" ItemStyle-Width="3%" >
                                    <ItemTemplate>
                                        <asp:Image ID="img" runat="server" Width="20" Height="20" />
                                    </ItemTemplate>
                                </asp:TemplateField>--%>
                            <asp:BoundField DataField="interface1" HeaderText="ATM IP" SortExpression="Interface1Status"></asp:BoundField>  <%--Edited by Ali Shah on 6th April, 2016   Sorting Column in Communication Dashboard--%>
                            <asp:BoundField DataField="interface2" HeaderText="Gateway" SortExpression="Interface2Status"></asp:BoundField> <%--Edited by Ali Shah on 6th April, 2016   Sorting Column in Communication Dashboard--%>
                            <asp:BoundField DataField="interface3" HeaderText="Hotline"  SortExpression="Interface3Status"></asp:BoundField> <%--Edited by Ali Shah on 6th April, 2016   Sorting Column in Communication Dashboard--%>
                            <asp:BoundField DataField="interface4" HeaderText="Branch"></asp:BoundField>
                            <asp:BoundField DataField="interface5" HeaderText="DVR"></asp:BoundField>
                            <asp:BoundField DataField="sensor1"></asp:BoundField>
                            <asp:BoundField DataField="sensor2"></asp:BoundField>
                            <asp:BoundField DataField="sensor3"></asp:BoundField>
                            <asp:BoundField DataField="sensor4"></asp:BoundField>
                            <asp:BoundField DataField="sensor5"></asp:BoundField>
                            <asp:BoundField DataField="sensor6"></asp:BoundField>
                            <asp:BoundField DataField="sensor7"></asp:BoundField>
                            <asp:BoundField DataField="sensor8"></asp:BoundField>
                            <asp:BoundField DataField="sensor9"></asp:BoundField>
                            <asp:BoundField DataField="sensor10"></asp:BoundField>
                            <asp:BoundField DataField="sensor11"></asp:BoundField>
                            <asp:BoundField DataField="sensor12"></asp:BoundField>
                            <asp:BoundField DataField="ccmsagent_last_reported_heartbeat" HeaderText="Agent HB" SortExpression="ccmsagent_last_reported_heartbeat">     <%--Edited by Ali Shah on 6th April, 2016   Sorting Column in Communication Dashboard--%>
                            </asp:BoundField>
                            <asp:BoundField DataField="ccmsservicemanager_last_reported_heartbeat" HeaderText="ServiceMgr HB" SortExpression="ccmsservicemanager_last_reported_heartbeat">      <%--Edited by Ali Shah on 6th April, 2016   Sorting Column in Communication Dashboard--%>
                            </asp:BoundField>
                            <%--<asp:TemplateField>
                                    
                                    <HeaderTemplate>
                                        <div title="header=[Purged Notes] body=[Number of notes purged during dispensed.]"
                                            style="color: White" >
                                            Purged Notes
                                        </div>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <%#Eval("purged_counts")%>
                                    </ItemTemplate>
                                </asp:TemplateField>--%>
                        </Columns>
                    </asp:gridview>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
