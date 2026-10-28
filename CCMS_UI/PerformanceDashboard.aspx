<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PerformanceDashboard.aspx.cs"
    Inherits="CCMSUI.PerformanceDashboard" %>

<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Performance Dashboard</title>
    <script language="javascript" src="content/cal/datepicker.js" type="text/javascript"></script>
    <script src="content/shortcut.js" type="text/javascript" language="javascript">
    </script>
    <link href="content/iSuite.css" rel="stylesheet" />
</head>
<body onload="hideProgressBar();">
    <form id="form1" runat="server">
    <div>
        <asp:Literal ID="Literal_Message" runat="server"></asp:Literal>
    </div>
    <div>
        <table width="100%" border="0">
            <tr>
                <td colspan="3" align="center">
                    <asp:ValidationSummary ID="valSummary" runat="server" />
                </td>
            </tr>
            <tr>
                <td align="center" style="font-size: large; background-color: Gray; color: White"
                    colspan="3">
                    Performance Dashboard
                </td>
            </tr>
            <tr id="t" runat="server">
                <td colspan="3">
                    <table style="background-color: InfoBackground; width: 100%" class="DemoArea">
                        <tr>
                            <%--<td style="width: 23px">
                                Organization
                            </td>
                            <td style="width: 43px">
                                <asp:Label ID="Label_Org" runat="server" Text="Please select Organization" Font-Bold="true"></asp:Label>
                            </td>--%>
                            <td style="width: 23px">
                                From
                            </td>
                            <td style="width: 23px">
                                <asp:TextBox ID="TextBox_From" runat="server" CssClass="textbox"></asp:TextBox>
                            </td>
                            <td style="width: 23px">
                                <img src="images/IconCalendar.gif" alt="Click to open calender" onclick="javascript:show_calendar('form1.TextBox_From');"
                                    style="border: 0; cursor: hand" id="IMG6" />
                            </td>
                            <td style="width: 3px">
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator5" CssClass="Validator"
                                    runat="server" ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                    ControlToValidate="TextBox_From" ErrorMessage="To Date is invalid.">*</asp:RegularExpressionValidator>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="TextBox_From"
                                    ErrorMessage="To date cannot be empty">*</asp:RequiredFieldValidator>
                            </td>
                            <td style="width: 23px">
                                To
                            </td>
                            <td style="width: 23px">
                                <asp:TextBox ID="TextBox_To" runat="server" CssClass="textbox"></asp:TextBox>
                            </td>
                            <td style="width: 23px">
                                <img src="images/IconCalendar.gif" alt="Click to open calender" onclick="javascript:show_calendar('form1.TextBox_To');"
                                    style="border: 0; cursor: hand" id="IMG1" />
                            </td>
                            <td style="width: 3px">
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" CssClass="Validator"
                                    runat="server" ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                    ControlToValidate="TextBox_To" ErrorMessage="To Date is invalid.">*</asp:RegularExpressionValidator>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="TextBox_To"
                                    ErrorMessage="To date cannot be empty">*</asp:RequiredFieldValidator>
                            </td>
                            <td align="left">
                                <asp:Button ID="Button1" runat="server" Text="Generate" OnClick="Button_Submit_Click"
                                    CssClass="button" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td valign="top">
                    <table width="100%" cellpadding="0" cellspacing="0" border="0">
                        <tr>
                            <td valign="top">
                                <table width="100%">
                                    <tr>
                                        <td valign="top" style="padding-left: 30px">
                                            <table>
                                                <tr>
                                                    <td>
                                                        <h2>
                                                            Key Performance Indicators</h2>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    
                                                    <td style="padding-left: 30px; width:300px">
                                                        <table>
                                                            <tr>
                                                                <td>
                                                                    <b>Out Of Cash Incidents Count =
                                                                        <asp:Label ID="label_totalOutOfIncidents" runat="server" Text="0"></asp:Label></b>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <table>
                                                                        <tr>
                                                                            <td style="background-color: #9ab353; width: 23px">
                                                                            </td>
                                                                            <td>
                                                                                5 < Excellent
                                                                            </td>
                                                                            <td style="background-color: #8766a5; width: 23px">
                                                                            </td>
                                                                            <td>
                                                                                10 - 15 Fair
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td style="background-color: #527fb8; width: 23px">
                                                                            </td>
                                                                            <td>
                                                                                5 - 10 Good
                                                                            </td>
                                                                            <td style="background-color: #BF4b4f; width: 23px">
                                                                            </td>
                                                                            <td>
                                                                                > 15 Need Attention
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td style="padding-left: 30px; width:300px">
                                                        <table>
                                                            <tr>
                                                                <td>
                                                                    <b>CIT Disputes Count = <asp:Label ID="Label_TotalExceptions" runat="server" Text="0"></asp:Label></b>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <table>
                                                                        <tr>
                                                                            <td style="background-color: #9ab353; width: 23px">
                                                                            </td>
                                                                            <td>
                                                                                5 < Excellent
                                                                            </td>
                                                                            <td style="background-color: #8766a5; width: 23px">
                                                                            </td>
                                                                            <td>
                                                                                10 - 15 Fair
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td style="background-color: #527fb8; width: 23px">
                                                                            </td>
                                                                            <td>
                                                                                5 - 10 Good
                                                                            </td>
                                                                            <td style="background-color: #BF4b4f; width: 23px">
                                                                            </td>
                                                                            <td>
                                                                                > 15 Need Attention
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td style="padding-left: 15px; width:300px">
                                                        <table>
                                                            <tr>
                                                                <td>
                                                                    <b>Cash Utilization
                                                                        <asp:Label ID="Label_CashUtil" runat="server"></asp:Label>%</b>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <table>
                                                                        <tr>
                                                                            <td style="background-color: #9ab353; width: 23px">
                                                                            </td>
                                                                            <td>
                                                                                80% Excellent
                                                                            </td>
                                                                            <td style="background-color: #8766a5; width: 23px">
                                                                            </td>
                                                                            <td>
                                                                                70%-75% Fair
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td style="background-color: #527fb8; width: 23px">
                                                                            </td>
                                                                            <td>
                                                                                75%-79% Good
                                                                            </td>
                                                                            <td style="background-color: #BF4b4f; width: 23px">
                                                                            </td>
                                                                            <td>
                                                                                70% < Need Attention
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                </tr>
                                                <tr >
                                                </tr>
                                                <tr>
                                                    <td>
                                                    </td>
                                                </tr>
                                                <tr>
                                                </tr>
                                                <tr>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <table>
                                    <tr>
                                        
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <table>
                                    <tr>
                                        <td style="padding: 2%">
                                            <asp:Chart ID="ChartOutOfCash" runat="server" Width="380px" Height="300px" BorderColor="#B54001"
                                                Palette="BrightPastel">
                                                <Titles>
                                                    <asp:Title Font="Trebuchet MS, 14.25pt, style=Bold" ForeColor="26, 59, 105" Text="Out Of Cash Incidents">
                                                    </asp:Title>
                                                </Titles>
                                                <Legends>
                                                    <asp:Legend BackColor="Transparent" Alignment="Near" Docking="Bottom" Font="Trebuchet MS, 8.25pt, style=Bold"
                                                        IsTextAutoFit="true" Name="Default" LegendStyle="Row" Title="Legends" Enabled="true">
                                                    </asp:Legend>
                                                </Legends>
                                                <BorderSkin SkinStyle="Emboss"></BorderSkin>
                                                <Series>
                                                </Series>
                                                <ChartAreas>
                                                    <asp:ChartArea Name="ChartArea1" BorderColor="64, 64, 64, 64" BorderDashStyle="Solid"
                                                        BackSecondaryColor="Transparent" BackColor="64, 165, 191, 228" ShadowColor="Transparent"
                                                        BackGradientStyle="TopBottom">
                                                        <Area3DStyle Rotation="10" Inclination="15" WallWidth="0" />
                                                        <%--<Position Y="3" Height="75  " Width="92" X="2"></Position>--%>
                                                        <AxisY LineColor="64, 64, 64, 64" LabelAutoFitMaxFontSize="8">
                                                            <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                            <MajorGrid LineColor="64, 64, 64, 64" />
                                                        </AxisY>
                                                        <AxisX LineColor="64, 64, 64, 64" LabelAutoFitMaxFontSize="8">
                                                            <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                            <MajorGrid LineColor="64, 64, 64, 64" />
                                                        </AxisX>
                                                    </asp:ChartArea>
                                                </ChartAreas>
                                            </asp:Chart>
                                        </td>
                                        <td style="padding: 2%">
                                            <table>
                                                <%--<tr>
                                                    <td align="right">
                                                        <b>CIT</b>
                                                        <asp:DropDownList ID="DropDownList_RepPerfCIT" runat="server" CssClass="dropDown">
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>--%>
                                                <tr>
                                                    <td colspan="2">
                                                        <asp:Chart ID="ChartRepPerf" runat="server" Width="380px" Height="300px" BorderColor="#B54001"
                                                            Palette="BrightPastel">
                                                            <Titles>
                                                                <asp:Title Font="Trebuchet MS, 14.25pt, style=Bold" ForeColor="26, 59, 105" Text="CIT Replenishment Performance">
                                                                </asp:Title>
                                                            </Titles>
                                                            <Legends>
                                                                <asp:Legend BackColor="Transparent" Alignment="Near" Docking="Bottom" Font="Trebuchet MS, 7.25pt, style=Bold"
                                                                    IsTextAutoFit="true" Name="Default" LegendStyle="Column" Title="Legends" Enabled="true">
                                                                </asp:Legend>
                                                            </Legends>
                                                            <BorderSkin SkinStyle="Emboss"></BorderSkin>
                                                            <Series>
                                                                <asp:Series ChartArea="ChartArea1" BorderColor="180, 26, 59, 105" ChartType="StackedColumn"
                                                                    IsValueShownAsLabel="true" BorderWidth="4" MarkerSize="4" MarkerStyle="Circle"
                                                                    Name="Correct" ToolTip="Correct">
                                                                </asp:Series>
                                                                <asp:Series ChartArea="ChartArea1" BorderColor="180, 26, 59, 105" ChartType="StackedColumn"
                                                                    IsValueShownAsLabel="true" BorderWidth="4" MarkerSize="4" MarkerStyle="Circle"
                                                                    Name="Time Exceptions" ToolTip="Time Exception">
                                                                </asp:Series>
                                                                <asp:Series ChartArea="ChartArea1" BorderColor="180, 26, 59, 105" ChartType="StackedColumn"
                                                                    IsValueShownAsLabel="true" BorderWidth="4" MarkerSize="4" MarkerStyle="Circle"
                                                                    Name="Amount Exceptions" ToolTip="Amount Exception">
                                                                </asp:Series>
                                                            </Series>
                                                            <ChartAreas>
                                                                <asp:ChartArea Name="ChartArea1" BorderColor="64, 64, 64, 64" BorderDashStyle="Solid"
                                                                    BackSecondaryColor="Transparent" BackColor="64, 165, 191, 228" ShadowColor="Transparent"
                                                                    BackGradientStyle="TopBottom">
                                                                    <Area3DStyle Rotation="10" Inclination="15" WallWidth="0" />
                                                                    <%--<Position Y="3" Height="75  " Width="92" X="2"></Position>--%>
                                                                    <AxisY LineColor="64, 64, 64, 64" LabelAutoFitMaxFontSize="8">
                                                                        <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                                        <MajorGrid LineColor="64, 64, 64, 64" />
                                                                    </AxisY>
                                                                    <AxisX LineColor="64, 64, 64, 64" LabelAutoFitMaxFontSize="8">
                                                                        <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                                        <MajorGrid LineColor="64, 64, 64, 64" />
                                                                    </AxisX>
                                                                </asp:ChartArea>
                                                            </ChartAreas>
                                                        </asp:Chart>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td style="padding: 2%">
                                            <asp:Chart ID="ChartCashUtil" runat="server" Width="380px" Height="300px" BorderColor="#B54001">
                                                <Titles>
                                                    <asp:Title Font="Trebuchet MS, 14.25pt, style=Bold" ForeColor="26, 59, 105" Text="Cash Utilization">
                                                    </asp:Title>
                                                </Titles>
                                                <Legends>
                                                    <asp:Legend BackColor="Transparent" Alignment="Near" Docking="Bottom" Font="Trebuchet MS, 8.25pt, style=Bold"
                                                        IsTextAutoFit="true" Name="Default" LegendStyle="Row" Title="Legends" Enabled="true">
                                                    </asp:Legend>
                                                </Legends>
                                                <BorderSkin SkinStyle="Emboss"></BorderSkin>
                                                <Series>
                                                    <%--<asp:Series ChartArea="ChartArea1" BorderColor="180, 26, 59, 105" ChartType="Column"
                                                        BorderWidth="4" MarkerSize="4" MarkerStyle="Circle" IsValueShownAsLabel="true"
                                                        Name="Replenished">
                                                    </asp:Series>
                                                    <asp:Series ChartArea="ChartArea1" BorderColor="180, 26, 59, 105" ChartType="Column"
                                                        BorderWidth="4" MarkerSize="4" MarkerStyle="Circle" IsValueShownAsLabel="true"
                                                        Name="Returned">
                                                    </asp:Series>--%>
                                                    <asp:Series ChartArea="ChartArea1" BorderColor="180, 26, 59, 105" ChartType="Column"
                                                        BorderWidth="4" MarkerSize="4" MarkerStyle="Circle" IsValueShownAsLabel="true"
                                                        Name="Utilization" Color="#BF4b4f">
                                                    </asp:Series>
                                                </Series>
                                                <ChartAreas>
                                                    <asp:ChartArea Name="ChartArea1" BorderColor="64, 64, 64, 64" BorderDashStyle="Solid"
                                                        BackSecondaryColor="Transparent" BackColor="64, 165, 191, 228" ShadowColor="Transparent"
                                                        BackGradientStyle="TopBottom">
                                                        <Area3DStyle Rotation="10" Inclination="15" WallWidth="0" />
                                                        <%--<Position Y="3" Height="75  " Width="92" X="2"></Position>--%>
                                                        <AxisY LineColor="64, 64, 64, 64" LabelAutoFitMaxFontSize="8">
                                                            <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                            <MajorGrid LineColor="64, 64, 64, 64" />
                                                        </AxisY>
                                                        <AxisX LineColor="64, 64, 64, 64" LabelAutoFitMaxFontSize="8">
                                                            <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                            <MajorGrid LineColor="64, 64, 64, 64" />
                                                        </AxisX>
                                                    </asp:ChartArea>
                                                </ChartAreas>
                                            </asp:Chart>
                                        </td>

                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <table>
                                    <tr>
                                    </tr>
                                </table>
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
