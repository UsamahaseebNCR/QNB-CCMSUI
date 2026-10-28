<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ParamPNCDepositReportingForBNA.aspx.cs"
    Inherits="CCMSUI.ParamPNCDepositReportingForBNA" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="../../content/styleSheets/bootstrap.min.css" rel="stylesheet">
    <link href="../../content/styleSheets/bootstrap-theme.min.css" rel="stylesheet">
    <title>Denomination Utilization Analysis</title>
    <script src="../../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
    <link href="../../content/iSuite.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
    <div style="text-align: center; font-size: xx-large; color: #FFFFFF; background-color: #003366;">
        PNC Deposit Reporting
    </div>
    <div>
        <table width="100%" border="0" cellpadding="0" cellspacing="0">
            <tr style="font-size: larger; font-weight: bold; font-family: Arial; color: White">
                <td align="center" style="font-size: large; font-weight: bold; margin-top: 50px;
                    color: Black; background-color: #CCCCCC;" colspan="2">
                    <asp:Literal ID="Literal_Title" runat="server" Text="BNA Business And Non Business Rule Analysis "></asp:Literal>
                </td>
            </tr>
            <tr style="background-color: #CCCCCC;">
                <td>
                    <table style="font-weight: bold">
                        <tr>
                            <td>
                                No. Of Units :
                            </td>
                            <td>
                                <asp:Label ID="Label_totalATMs" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Items Processed :
                            </td>
                            <td>
                                <asp:Label ID="Label_ItemsProcessed" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                % Acceptance :
                            </td>
                            <td>
                                <asp:Label ID="Label_AccPerc" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                % Rejection :
                            </td>
                            <td>
                                <asp:Label ID="Label_RejPerc" runat="server"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </td>
                <td colspan="2" align="right">
                    <table>
                        <tr>
                            <td style="text-align: right; font-weight: bold" colspan="2">
                                Print Date & Time :
                            </td>
                            <td>
                                <asp:Label ID="Label_currentTime" runat="server" Font-Bold="true"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align: right; font-weight: bold" colspan="2" class="style2">
                                From Date :
                            </td>
                            <td>
                                <asp:Label ID="Label_FromDate" runat="server" Font-Bold="true"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align: right; font-weight: bold" colspan="2">
                                To Date :
                            </td>
                            <td>
                                <asp:Label ID="Label_ToDate" runat="server" Font-Bold="true"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr id="chartRow">
                <td valign="top" colspan="2">
                    <table style="margin-top: 10px; border: 1px;" border="0" width="100%">
                        <tr>
                            <td valign="top">
                                <center>
                                    <table>
                                        <tr>
                                            <td valign="top">
                                                <center>
                                                    <asp:Chart ID="Chart_AccCount" runat="server" BorderColor="#B54001" Width="550px"
                                                        BackGradientStyle="TopBottom" Height="300px">
                                                        <Titles>
                                                            <asp:Title Font="Trebuchet MS, 14.25pt, style=Bold" ForeColor="Black">
                                                            </asp:Title>
                                                        </Titles>
                                                        <Legends>
                                                            <asp:Legend BackColor="Transparent" Alignment="Near" Docking="Bottom" Font="Trebuchet MS, 8.25pt, style=Bold"
                                                                IsTextAutoFit="true" Name="Default" LegendStyle="Row" Title="Legends" Enabled="false">
                                                            </asp:Legend>
                                                        </Legends>
                                                        <BorderSkin SkinStyle="None"></BorderSkin>
                                                        <Series>
                                                            <asp:Series Name="Default" YValuesPerPoint="2" ChartType="Pie" IsValueShownAsLabel="true"
                                                                IsVisibleInLegend="true">
                                                            </asp:Series>
                                                        </Series>
                                                        <ChartAreas>
                                                            <asp:ChartArea Name="ChartArea1" BackGradientStyle="TopBottom" BorderWidth="3" BackColor="Silver">
                                                                <Area3DStyle Rotation="0" />
                                                                <AxisY LineColor="64, 64, 64, 64">
                                                                    <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                                    <MajorGrid LineColor="GhostWhite" Enabled="true" LineDashStyle="Dash" />
                                                                </AxisY>
                                                                <AxisX LineColor="64, 64, 64, 64">
                                                                    <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                                    <MajorGrid LineColor="GhostWhite" Enabled="true" LineDashStyle="Dash" />
                                                                </AxisX>
                                                            </asp:ChartArea>
                                                        </ChartAreas>
                                                    </asp:Chart>
                                                </center>
                                            </td>
                                            <td valign="top">
                                                <asp:Chart ID="Chart_RejectRate" runat="server" Width="500" EnableViewState="false"
                                                    Palette="BrightPastel" Height="300">
                                                    <Titles>
                                                        <asp:Title Font="Trebuchet MS, 14.25pt, style=Bold" Name="Title1" ForeColor="Black"
                                                            Text="">
                                                        </asp:Title>
                                                    </Titles>
                                                    <Legends>
                                                        <%--<asp:Legend BackColor="Transparent" Alignment="Center" Docking="Right" Font="Trebuchet MS, 8.25pt, style=Bold"
                                                    IsTextAutoFit="False" Name="Default" LegendStyle="Column" Title="Legends" Enabled="true">
                                                </asp:Legend>--%>
                                                        <asp:Legend BackColor="Transparent" Alignment="Near" Docking="Bottom" Font="Trebuchet MS, 8.25pt, style=Bold"
                                                            IsTextAutoFit="true" Name="Default" LegendStyle="Row" Title="Legends">
                                                        </asp:Legend>
                                                    </Legends>
                                                    <BorderSkin SkinStyle="None"></BorderSkin>
                                                    <Series>
                                                        <asp:Series Name="Successful" ChartType="StackedColumn" IsValueShownAsLabel="true">
                                                        </asp:Series>
                                                        <asp:Series Name="Failed" ChartType="StackedColumn" IsValueShownAsLabel="true">
                                                        </asp:Series>
                                                        <asp:Series Name="RejectRate%" ChartType="StackedColumn" IsValueShownAsLabel="true">
                                                        </asp:Series>
                                                    </Series>
                                                    <ChartAreas>
                                                        <asp:ChartArea Name="ChartArea1" BackGradientStyle="TopBottom" BorderWidth="3" BackColor="Orange">
                                                            <Area3DStyle Rotation="0" />
                                                            <AxisY LineColor="64, 64, 64, 64">
                                                                <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                                <MajorGrid LineColor="GhostWhite" />
                                                            </AxisY>
                                                            <AxisX LineColor="64, 64, 64, 64">
                                                                <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" IntervalType="Days" />
                                                                <MajorGrid LineColor="GhostWhite" />
                                                            </AxisX>
                                                        </asp:ChartArea>
                                                    </ChartAreas>
                                                </asp:Chart>
                                            </td>
                                        </tr>
                                    </table>
                                </center>
                            </td>
                        </tr>
                        <tr>
                            <td valign="top" colspan="2">
                                <center>
                                    <asp:Chart ID="Chart_NoteRejectReasons" runat="server" Width="1100" Height="300px">
                                        <Titles>
                                            <asp:Title Font="Trebuchet MS, 14.25pt, style=Bold" Name="Title1" ForeColor="Black">
                                            </asp:Title>
                                        </Titles>
                                        <Legends>
                                            <asp:Legend BackColor="Transparent" Alignment="Center" Docking="Right" Font="Trebuchet MS, 8.25pt, style=Bold"
                                                IsTextAutoFit="False" Name="Default" LegendStyle="Column" Title="Legends" Enabled="false">
                                            </asp:Legend>
                                        </Legends>
                                        <BorderSkin SkinStyle="None"></BorderSkin>
                                        <Series>
                                            <asp:Series Name="Default" YValuesPerPoint="2" ChartType="Column" IsValueShownAsLabel="true"
                                                IsVisibleInLegend="true">
                                            </asp:Series>
                                        </Series>
                                        <ChartAreas>
                                            <asp:ChartArea Name="ChartArea1" BackGradientStyle="TopBottom" BorderWidth="3">
                                                <Area3DStyle Rotation="0" />
                                                <AxisY LineColor="64, 64, 64, 64">
                                                    <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                    <MajorGrid LineColor="GhostWhite" />
                                                </AxisY>
                                                <AxisX LineColor="64, 64, 64, 64">
                                                    <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" Interval="1" />
                                                    <MajorGrid LineColor="GhostWhite" />
                                                </AxisX>
                                            </asp:ChartArea>
                                        </ChartAreas>
                                    </asp:Chart>
                                </center>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
    <div>
        <table width="100%">
            <tr>
                <td align="right">
                    <img src="../../images/pnclogo.png" height="100px" />
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
