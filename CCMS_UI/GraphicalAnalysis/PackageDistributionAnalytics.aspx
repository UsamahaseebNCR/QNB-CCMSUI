<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PackageDistributionAnalytics.aspx.cs"
    Inherits="CCMSUI.PackageDistributionAnalytics" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Denomination Utilization Analysis</title>
    <script language="javascript" src="content/cal/datepicker.js" type="text/javascript"></script>
    <script src="content/shortcut.js" type="text/javascript" language="javascript">
    </script>
    <link href="content/iSuite.css" rel="stylesheet" />
    <link href="content/Scripts/ui-lightness/jquery-ui-1.7.2.custom.css" rel="stylesheet"
        type="text/css" />
    <script type="text/javascript" src="content/js/jquery.js"></script>
    <script type="text/javascript" src="content/scripts/ui.datepicker.js"></script>
    <script type="text/javascript">


        $(function () {

            $('.datepicker').datepicker({ dateFormat: 'dd/mm/yy',
                changeMonth: true,
                changeYear: true,
                showOn: "button",
                buttonImage: "images/calendar.gif",
                buttonImageOnly: true

            });
        });

        
    </script>
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
                <td align="center" style="font-size: large; background-color: black; color: White"
                    colspan="3">
                    Package Distribution Analytics
                </td>
            </tr>
            <tr id="t" runat="server">
                <td colspan="3">
                    <table style="background-color: #F3F3F3;" border="0" width="100%">
                        <tr>
                            <td style="width: 23px">
                                From
                            </td>
                            <td style="width: 220px">
                                <asp:TextBox ID="TextBox_From" runat="server" CssClass="datepicker"></asp:TextBox>
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator5" CssClass="Validator"
                                    runat="server" ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                    ControlToValidate="TextBox_From" ErrorMessage="To Date is invalid.">*</asp:RegularExpressionValidator>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="TextBox_From"
                                    ErrorMessage="To date cannot be empty">*</asp:RequiredFieldValidator>
                            </td>
                            <td style="width: 23px">
                                To
                            </td>
                            <td style="width: 220px">
                                <asp:TextBox ID="TextBox_To" runat="server" CssClass="datepicker"></asp:TextBox>
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" CssClass="Validator"
                                    runat="server" ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                    ControlToValidate="TextBox_To" ErrorMessage="To Date is invalid.">*</asp:RegularExpressionValidator>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="TextBox_To"
                                    ErrorMessage="To date cannot be empty">*</asp:RequiredFieldValidator>
                            </td>
                            <td style="width: 20px">
                                Package
                            </td>
                            <td style="width: 20px">
                                <asp:DropDownList ID="DropDownList_Package" runat="server" CssClass="dropDown">
                                </asp:DropDownList>
                                </td>
                            <td align="left">
                                <asp:Button ID="Button1" runat="server" Text="Generate" OnClick="Button_Submit_Click"
                                    CssClass="button" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr id="chartRow">
                <td valign="top" width="80%">
                    <table style="margin-top: 10px; border: 1px">
                        <tr>
                            <td valign="top">
                                <table>
                                    <tr>
                                        <td>
                                            <asp:Chart ID="Chart2" runat="server" Width="600px">
                                                <Titles>
                                                    <asp:Title Font="Trebuchet MS, 14.25pt, style=Bold" Name="Title1" ForeColor="26, 59, 105">
                                                    </asp:Title>
                                                </Titles>
                                                <Legends>
                                                    <asp:Legend BackColor="Transparent" Alignment="Center" Docking="Right" Font="Trebuchet MS, 8.25pt, style=Bold"
                                                        IsTextAutoFit="False" Name="Default" LegendStyle="Column" Title="Legends">
                                                    </asp:Legend>
                                                </Legends>
                                                <BorderSkin SkinStyle="None"></BorderSkin>
                                                <Series>
                                                    <asp:Series Name="Default" YValuesPerPoint="2" ChartType="Pie" IsValueShownAsLabel="true"
                                                        IsVisibleInLegend="true">
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
                                            </asp:Chart>
                                        </td>
                                        <td>
                                            <asp:Chart ID="Chart3" runat="server" Width="600">
                                                <Titles>
                                                    <asp:Title Font="Trebuchet MS, 14.25pt, style=Bold" Name="Title1" ForeColor="26, 59, 105">
                                                    </asp:Title>
                                                </Titles>
                                                <Legends>
                                                    <asp:Legend BackColor="Transparent" Alignment="Center" Docking="Right" Font="Trebuchet MS, 8.25pt, style=Bold"
                                                        IsTextAutoFit="False" Name="Default" LegendStyle="Column" Title="Legends" Enabled="false">
                                                    </asp:Legend>
                                                </Legends>
                                                <BorderSkin SkinStyle="None"></BorderSkin>
                                                <Series>
                                                    <asp:Series Name="Default" YValuesPerPoint="2" ChartType="Bar" IsValueShownAsLabel="true"
                                                        IsVisibleInLegend="true">
                                                    </asp:Series>
                                                </Series>
                                                <ChartAreas>
                                                    <asp:ChartArea Name="ChartArea1" Area3DStyle-Enable3D="true">
                                                        <Area3DStyle Rotation="0" />
                                                        <AxisY LineColor="64, 64, 64, 64">
                                                            <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                            <MajorGrid LineColor="64, 64, 64, 64" />
                                                        </AxisY>
                                                        <AxisX LineColor="64, 64, 64, 64" Interval="1">
                                                            <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                            <MajorGrid LineColor="64, 64, 64, 64" />
                                                        </AxisX>
                                                    </asp:ChartArea>
                                                </ChartAreas>
                                            </asp:Chart>
                                        </td>
                                        <td>
                                            <asp:Chart ID="ChartFailureAnalysis" runat="server" Width="600">
                                                <Titles>
                                                    <asp:Title Font="Trebuchet MS, 14.25pt, style=Bold" Name="Title1" ForeColor="26, 59, 105">
                                                    </asp:Title>
                                                </Titles>
                                                <Legends>
                                                    <asp:Legend BackColor="Transparent" Alignment="Center" Docking="Right" Font="Trebuchet MS, 8.25pt, style=Bold"
                                                        IsTextAutoFit="False" Name="Default" LegendStyle="Column" Title="Legends" Enabled="false">
                                                    </asp:Legend>
                                                </Legends>
                                                <BorderSkin SkinStyle="None"></BorderSkin>
                                                <Series>
                                                    <asp:Series Name="Default" YValuesPerPoint="2" ChartType="Bar" IsValueShownAsLabel="true"
                                                        IsVisibleInLegend="true">
                                                    </asp:Series>
                                                </Series>
                                                <ChartAreas>
                                                    <asp:ChartArea Name="ChartArea1" Area3DStyle-Enable3D="true">
                                                        <Area3DStyle Rotation="0" />
                                                        <AxisY LineColor="64, 64, 64, 64">
                                                            <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                            <MajorGrid LineColor="64, 64, 64, 64" />
                                                        </AxisY>
                                                        <AxisX LineColor="64, 64, 64, 64" Interval="1">
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
                            <td valign="top" colspan="2">
                                <table width="100%">
                                    <tr>
                                        <td>
                                            <asp:Chart ID="ChartWorstATMsByExecution" runat="server" Width="800px">
                                                <Titles>
                                                    <asp:Title Font="Trebuchet MS, 14.25pt, style=Bold" Name="Title1" ForeColor="26, 59, 105">
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
                                                    <asp:ChartArea Name="ChartArea1" Area3DStyle-Enable3D="true">
                                                        <Area3DStyle Rotation="0" />
                                                        <AxisY LineColor="64, 64, 64, 64">
                                                            <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                            <MajorGrid LineColor="64, 64, 64, 64" />
                                                        </AxisY>
                                                        <AxisX LineColor="64, 64, 64, 64" Interval="1">
                                                            <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                            <MajorGrid LineColor="64, 64, 64, 64" />
                                                        </AxisX>
                                                    </asp:ChartArea>
                                                </ChartAreas>
                                            </asp:Chart>
                                        </td>
                                        <td>
                                            <asp:Chart ID="ChartWorstATMsByNetwork" runat="server" Width="800">
                                                <Titles>
                                                    <asp:Title Font="Trebuchet MS, 14.25pt, style=Bold" Name="Title1" ForeColor="26, 59, 105">
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
                                                    <asp:ChartArea Name="ChartArea1" Area3DStyle-Enable3D="true">
                                                        <Area3DStyle Rotation="0" />
                                                        <AxisY LineColor="64, 64, 64, 64">
                                                            <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                            <MajorGrid LineColor="64, 64, 64, 64" />
                                                        </AxisY>
                                                        <AxisX LineColor="64, 64, 64, 64" Interval="1">
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
                    </table>
                </td>
            </tr>
        </table>
    </div>
    <input type="button" runat="server" id="refresh" style="display: none" />
    </form>
</body>
</html>
