<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PNCMeanTimeToIncidentFailureAnalysisForBNA.aspx.cs"
    Inherits="CCMSUI.PNCMeanTimeToIncidentFailureAnalysisForBNA" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Mean Time To Failure Analysis</title>
    <script language="javascript" src="../content/cal/datepicker.js" type="text/javascript"></script>
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
    <link href="../content/iSuite.css" rel="stylesheet" />
    <link href="../content/Scripts/ui-lightness/jquery-ui-1.7.2.custom.css" rel="stylesheet"
        type="text/css" />
    <script type="text/javascript" src="../content/js/jquery.js"></script>
    <script type="text/javascript" src="../content/scripts/ui.datepicker.js"></script>
    <script type="text/javascript">
        $(function () {

            $('.datepicker').datepicker({ dateFormat: 'dd/mm/yy',
                changeMonth: true,
                changeYear: true,
                showOn: "button",
                buttonImage: "../images/calendar.gif",
                buttonImageOnly: true

            });
        });
    
    </script>
</head>
<body onload="hideProgressBar();">
    <form id="form1" runat="server">
    <div>
        <asp:literal id="Literal_Message" runat="server"></asp:literal>
    </div>
    <div>
        <table width="100%" border="0">
            <tr>
                <td colspan="3" align="center">
                    <asp:validationsummary id="valSummary" runat="server" />
                </td>
            </tr>
            <tr>
                <td align="center" style="font-size: large; background-color: Gray; color: White"
                    colspan="3">
                    Mean Time To Incident Failure Analysis For BNA
                </td>
            </tr>
            <tr id="t" runat="server">
                <td colspan="3">
                    <table style="background-color: Silver; width: 100%" class="DemoArea" border="0">
                        <tr>
                            <td >
                                From
                            </td>
                            <td >
                                <asp:TextBox ID="TextBox_From" runat="server" CssClass="textbox"></asp:TextBox>
                                <img src="../images/IconCalendar.gif" alt="Click to open calender" onclick="javascript:show_calendar('form1.TextBox_From');"
                                    style="border: 0; cursor: hand" id="IMG6" />
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator5" CssClass="Validator"
                                    runat="server" ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                    ControlToValidate="TextBox_From" ErrorMessage="To Date is invalid.">*</asp:RegularExpressionValidator>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="TextBox_From"
                                    ErrorMessage="From date cannot be empty">*</asp:RequiredFieldValidator>
                            </td>
                            <td>
                                To
                            </td>
                            <td>
                                <asp:TextBox ID="TextBox_To" runat="server" CssClass="textbox"></asp:TextBox>
                                <img src="../images/IconCalendar.gif" alt="Click to open calender" onclick="javascript:show_calendar('form1.TextBox_To');"
                                    style="border: 0; cursor: hand" id="IMG1" />
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" CssClass="Validator"
                                    runat="server" ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                    ControlToValidate="TextBox_To" ErrorMessage="To Date is invalid.">*</asp:RegularExpressionValidator>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="TextBox_To"
                                    ErrorMessage="To date cannot be empty">*</asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr style="width: 100%">
                            <td valign="top" style="width: 10%">
                                Error Analysis On
                            </td>
                            <td valign="top" style="width: 90%" colspan="6">
                                <asp:CheckBoxList ID="CheckBoxList_ErrorAnalysisType" runat="server" Width="100%">
                                    <asp:ListItem Text="Business Rule" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="Non Business Rule" Selected="True"></asp:ListItem>
                                </asp:CheckBoxList>
                            </td>
                        </tr>
                         <tr>
                            <td valign="top">
                                Module:
                            </td>
                            <td>
                                <asp:CheckBoxList  ID="CheckBoxList_Module" runat="server" RepeatDirection="Horizontal">
                                    <asp:ListItem Selected="True">SDM</asp:ListItem>
                                     <asp:ListItem>BNA</asp:ListItem>
                                    
                                </asp:CheckBoxList>
                            </td>
                        </tr>
                        <tr>
                            <td>
                            </td>
                            <td align="left" colspan="6">
                                <asp:Button ID="Button1" runat="server" Text="Generate" OnClick="Button_Submit_Click"
                                    CssClass="button" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr id="chartRow">
                <td valign="top">
                    <table style="margin-top: 10px; border: 1px;" border="0" width="100%">
                        <tr>
                            <td valign="top" width="100%">
                                <table width="100%">
                                    <tr>
                                        <td>
                                            <center>
                                                <asp:chart id="Chart_MeanTimeToFailure1" runat="server" bordercolor="#B54001" width="1000px">
                                                    <Titles>
                                                        <asp:Title Font="Trebuchet MS, 14.25pt, style=Bold" ForeColor="26, 59, 105" Text="Mean Time To Incident Failure Analysis For BNA">
                                                        </asp:Title>
                                                    </Titles>
                                                    <Legends>
                                                        <asp:Legend BackColor="Transparent" Alignment="Near" Docking="Bottom" Font="Trebuchet MS, 8.25pt, style=Bold"
                                                            IsTextAutoFit="true" Name="Default" LegendStyle="Row" Title="Legends" Enabled="true">
                                                        </asp:Legend>
                                                    </Legends>
                                                    <BorderSkin SkinStyle="None"></BorderSkin>
                                                    <Series>
                                                        <asp:Series Name="Business Rule" ChartType="Column">
                                                        </asp:Series>
                                                        <asp:Series Name="Non Business Rule" ChartType="Column">
                                                        </asp:Series>
                                                    </Series>
                                                    <ChartAreas>
                                                        <asp:ChartArea Name="ChartArea1" BackColor="White">
                                                            <Area3DStyle Rotation="0" />
                                                            <AxisY LineColor="64, 64, 64, 64" Title="Notes Deposited">
                                                                <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                                <MajorGrid LineColor="Black" />
                                                            </AxisY>
                                                            <AxisX LineColor="64, 64, 64, 64" Title="Day"  IntervalType="Days" IntervalOffset="5" IntervalOffsetType="Days"  IsStartedFromZero="true" >
                                                                <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                                <MajorGrid LineColor="Black" />
                                                            </AxisX>
                                                        </asp:ChartArea>
                                                    </ChartAreas>
                                                </asp:chart>
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
    <input type="button" runat="server" id="refresh" style="display: none" />
    </form>
</body>
</html>
