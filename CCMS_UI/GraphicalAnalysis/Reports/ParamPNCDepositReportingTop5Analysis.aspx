<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ParamPNCDepositReportingTop5Analysis.aspx.cs"
    Inherits="CCMSUI.ParamPNCDepositReportingTop5Analysis" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Denomination Utilization Analysis</title>
    <script language="javascript" src="../../content/cal/datepicker.js" type="text/javascript"></script>
    <script src="../../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
    <link href="../../content/iSuite.css" rel="stylesheet" />
</head>
<body onload="hideProgressBar();">
    <form id="form1" runat="server">
    <div>
        <table width="100%" border="0">
           <tr>
                <td colspan="2">
                    <hr />
                </td>
            </tr>
            <tr style="font-size: large; font-family: Arial; color: White">
                <td style="width:100px">
                    <img src="../../images/pnc.jpg" />
                </td>
                <td align="right" style="font-size: large; font-weight: bold; color: Black">
                    <asp:Literal ID="Literal_Title" runat="server" Text="Test"></asp:Literal>
                </td>
            </tr>
            <tr>
                <td style="text-align: right; font-weight: bold" colspan="2">
                    Print Date & Time :
                    <asp:Label ID="Label_currentTime" runat="server" Font-Bold="true"></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="text-align: right; font-weight: bold" colspan="2">
                    Generated For the Day :<asp:Label ID="Label_scheduleDate" runat="server" Font-Bold="true"></asp:Label>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <hr />
                </td>
            </tr>
            
            
            <tr id="chartRow">
                <td valign="top" colspan="2" >
                    <table style="margin-top: 10px; border: 1px;" border="0" width="100%">
                        <tr>
                            <td valign="top" style="width:50%; text-align:center">
                                <asp:Chart ID="Chart_Top5CashAcceptorIncident" runat="server" BorderColor="#B54001" Height="500px"
                                    Width="580px">
                                    <Titles>
                                        <asp:Title Font="Trebuchet MS, 14.25pt, style=Bold" ForeColor="Black">
                                        </asp:Title>
                                    </Titles>
                                    <Legends>
                                        <asp:Legend BackColor="Transparent" Alignment="Near" Docking="Top" Font="Trebuchet MS, 8.25pt, style=Bold"
                                            IsTextAutoFit="true" Name="Default" LegendStyle="Row" Title="Legends" Enabled="false">
                                        </asp:Legend>
                                    </Legends>
                                    <BorderSkin SkinStyle="None"></BorderSkin>
                                    <Series>
                                        <asp:Series Name="Default" YValuesPerPoint="2" ChartType="Column" IsValueShownAsLabel="true"
                                            IsVisibleInLegend="true">
                                        </asp:Series>
                                    </Series>
                                    <ChartAreas>
                                        <asp:ChartArea Name="ChartArea1" BackColor="White">
                                            <Area3DStyle Rotation="0" />
                                            <AxisY LineColor="64, 64, 64, 64">
                                                <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                <MajorGrid LineColor="Black" />
                                            </AxisY>
                                            <AxisX LineColor="64, 64, 64, 64">
                                                <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                <MajorGrid LineColor="Black" />
                                            </AxisX>
                                        </asp:ChartArea>
                                    </ChartAreas>
                                </asp:Chart>
                                
                            </td>
                            <td>
                                <asp:Chart ID="Chart_Top5CheckAcceptorIncident" runat="server" Width="580px" EnableViewState="false" Height="500px"
                                    Palette="BrightPastel">
                                    <Titles>
                                        <asp:Title Font="Trebuchet MS, 14.25pt, style=Bold" Name="Title1" ForeColor="Black"
                                            Text="">
                                        </asp:Title>
                                    </Titles>
                                    <Legends>
                                        <%--<asp:Legend BackColor="Transparent" Alignment="Center" Docking="Right" Font="Trebuchet MS, 8.25pt, style=Bold"
                                                    IsTextAutoFit="False" Name="Default" LegendStyle="Column" Title="Legends" Enabled="true">
                                                </asp:Legend>--%>
                                        <asp:Legend BackColor="Transparent" Alignment="Near" Docking="Top" Font="Trebuchet MS, 8.25pt, style=Bold"
                                            IsTextAutoFit="true" Name="Default" LegendStyle="Row" Title="Legends" Enabled="false">
                                        </asp:Legend>
                                    </Legends>
                                    <BorderSkin SkinStyle="None"></BorderSkin>
                                    <Series>
                                        <asp:Series Name="Default" YValuesPerPoint="2" ChartType="Column" IsValueShownAsLabel="true"
                                            IsVisibleInLegend="true">
                                        </asp:Series>
                                    </Series>
                                    <ChartAreas>
                                        <asp:ChartArea Name="ChartArea1" BackColor="White">
                                            <Area3DStyle Rotation="0" />
                                            <AxisY LineColor="64, 64, 64, 64">
                                                <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                <MajorGrid LineColor="Black" />
                                            </AxisY>
                                            <AxisX LineColor="64, 64, 64, 64">
                                                <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                <MajorGrid LineColor="Black" />
                                            </AxisX>
                                        </asp:ChartArea>
                                    </ChartAreas>
                                </asp:Chart>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
    <input type="button" runat="server" id="refresh" style="display: none" />
    </form>
</body>
</html>
