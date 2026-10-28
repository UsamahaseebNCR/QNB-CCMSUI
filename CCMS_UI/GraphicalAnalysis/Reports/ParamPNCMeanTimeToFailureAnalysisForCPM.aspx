<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ParamPNCMeanTimeToFailureAnalysisForCPM.aspx.cs"
    Inherits="CCMSUI.ParamPNCMeanTimeToFailureAnalysisForCPM" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Mean Time To Failure Analysis</title>
    <script src="../../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
    <link href="../../content/iSuite.css" rel="stylesheet" />
</head>
<body onload="hideProgressBar();">
    <form id="form1" runat="server">
    <div>
        <asp:Literal ID="Literal_Message" runat="server"></asp:Literal>
    </div>
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
                    <asp:Literal ID="Literal_Title" runat="server" Text="PNC Mean Time to Failure Analysis "></asp:Literal>
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
                <td valign="top" colspan="2">
                    <table style="margin-top: 10px; border: 1px;" border="0" width="100%">
                        <tr>
                            <td valign="top" width="100%">
                                <table width="100%">
                                    <tr>
                                        <td>
                                            <center>
                                                <asp:Chart ID="Chart_MeanTimeToFailure1" runat="server" BorderColor="#B54001" Width="1200px" Height="800px">
                                                    <Titles>
                                                        <asp:Title Font="Trebuchet MS, 14.25pt, style=Bold" ForeColor="Black" Text="Mean Time To Failure Analysis">
                                                        </asp:Title>
                                                    </Titles>
                                                    <Legends>
                                                        <asp:Legend BackColor="Transparent" Alignment="Near" Docking="Bottom" Font="Trebuchet MS, 8.25pt, style=Bold"
                                                            IsTextAutoFit="true" Name="Default" LegendStyle="Row" Title="Legends" Enabled="true">
                                                        </asp:Legend>
                                                    </Legends>
                                                    <BorderSkin SkinStyle="None"></BorderSkin>
                                                    <Series>
                                                        <asp:Series Name="Business Rules" ChartType="Column">
                                                        </asp:Series>
                                                        <asp:Series Name="Non Business Rules" ChartType="Column">
                                                        </asp:Series>
                                                    </Series>
                                                    <ChartAreas>
                                                        <asp:ChartArea Name="ChartArea1" BackColor="White">
                                                            <Area3DStyle Rotation="0" />
                                                            <AxisY LineColor="64, 64, 64, 64" Title="Cheques Deposited">
                                                                <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                                <MajorGrid LineColor="Black" />
                                                            </AxisY>
                                                            <AxisX LineColor="64, 64, 64, 64" Title="Day" IntervalType="Days" IntervalOffset="5"
                                                                IntervalOffsetType="Days" IsStartedFromZero="true">
                                                                <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                                <MajorGrid LineColor="Black" />
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
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
