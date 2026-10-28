<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ATMTransactionAnalysis.aspx.cs"
    Inherits="CCMSUI.CCMS.ATMTransactionAnalysis" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>ATM Transaction Analysis</title>
    <link href="../content/iSuite.css" rel="stylesheet" />
       <link href="../content/DatePicker/jquery-ui.css" rel="stylesheet" />
    <script src="../content/DatePicker/jquery-3.7.1.js"></script>
    <script src="../content/DatePicker/jquery-ui.js"></script>

<script>
    $(function () {
        $(".datepicker").datepicker({
            dateFormat: "dd/mm/yy",
            showOn: "button",
            buttonImage: "../images/calendar.gif",
            buttonImageOnly: true,
            buttonText: "Select date",
            showButtonPanel: true
        });
    });
</script>
</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)'
    onload="hideProgressBar();">
    <form id="form1" runat="server">
    <div>
        <table style="width: 100%" cellspacing="0" cellpadding="0" border="0" class="mainbox">
            <tr>
                <td class="PageHeading" align="left" colspan="2">
                    Transaction Analysis
                </td>
            </tr>
            <tr>
                <td class="ValidationText" align="center">
                    <asp:Literal ID="Literal1" runat="server" EnableViewState="False"></asp:Literal>
                </td>
            </tr>
            <tr>
                <td class="ConfirmationText" align="center">
                    <asp:ValidationSummary ID="valSummary" runat="server" />
                    <asp:Literal ID="Literal2" runat="server" EnableViewState="False"></asp:Literal>
                </td>
            </tr>
            <tr align="left" style="background-color: WHITE">
                <td style="height: 20px; padding-left: 5px; width: 100%;" colspan="2">
                    <b>Search Filters</b>
                </td>
            </tr>
            <tr align="left">
                <td colspan="2" style="width: 100%">
                    <table width="100%" class="SEARCHBOX" border="0">
                        <tr>
                            <td>
                                <table id="search" width="90%" border="0">
                                    <tr>
                                        <td>
                                            From :
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="TextBox_FromDate"  CssClass="datepicker" Font-Names="Verdana"
                                                Font-Size="10px" />
                                            <asp:RegularExpressionValidator ID="ValFromDate" CssClass="Validator" runat="server"
                                                ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                                ControlToValidate="TextBox_FromDate" ErrorMessage="From Date is invalid.">*</asp:RegularExpressionValidator>
                                        </td>
                                        <td style="width: 99px; height: 17px">
                                            To :
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="TextBox_ToDate"  CssClass="datepicker" Font-Names="Verdana"
                                                Font-Size="10px" />
                                            <asp:RegularExpressionValidator ID="ValToDate" CssClass="Validator" runat="server"
                                                ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                                ControlToValidate="TextBox_ToDate" ErrorMessage="To Date is invalid.">*</asp:RegularExpressionValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Group:
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="DropDownList_NoteSetTypeID" runat="server" CssClass="textbox"
                                                AppendDataBoundItems="true" Width="125px">
                                                <asp:ListItem>Select</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                        </td>
                                        <td>
                                        </td>
                                        <td colspan="2">
                                            <asp:Button ID="Button_search" runat="server" CssClass="button" OnClick="Button_search_Click"
                                                Text="Search" Width="71px" />&nbsp;
                                            <asp:Button ID="Button_Reset" runat="server" CssClass="button" OnClick="Button_Reset_Click"
                                                Text="Reset" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td>
                                <table>
                                    <tr>
                                        <td>
                                            <img src="../images/Icon_search.png" alt="" />
                                        </td>
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
                            <td valign="top">
                                <table>
                                    <thead>
                                        <b>Summary Of Activities</b>
                                        <hr />
                                    </thead>
                                    <tr>
                                        <td>
                                            <b>Total No. Of Transactions</b>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_TotalTrxn" runat="server" Font-Bold="true"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <b>Total No. Of Replenishments</b>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_TotalReplenishments" runat="server" Font-Bold="true"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <b>Total Cash Out</b>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_TotalCashOut" runat="server" Font-Bold="true"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <b>Total No. Of Call Out</b>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_CallOut" runat="server" Font-Bold="true"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <b>Cash Utilization</b>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_CashUtilization" runat="server" Font-Bold="true"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td>
                                <asp:Chart ID="Chart1" runat="server" Width="951px" BorderColor="#B54001">
                                    <Titles>
                                        <asp:Title Font="Trebuchet MS, 14.25pt, style=Bold" ForeColor="26, 59, 105">
                                        </asp:Title>
                                    </Titles>
                                    <Legends>
                                        <asp:Legend BackColor="Transparent" Alignment="Near" Docking="Top" Font="Trebuchet MS, 8.25pt, style=Bold"
                                            IsTextAutoFit="true" Name="Default" LegendStyle="Row" Title="Legends" Enabled="false">
                                        </asp:Legend>
                                    </Legends>
                                    <BorderSkin SkinStyle="None"></BorderSkin>
                                    <Series>
                                        <asp:Series ChartArea="ChartArea1" BorderColor="180, 26, 59, 105" ChartType="Spline"
                                            BorderWidth="4" MarkerSize="4" MarkerStyle="Circle" IsValueShownAsLabel="true"
                                            Name="">
                                        </asp:Series>
                                    </Series>
                                    <ChartAreas>
                                        <asp:ChartArea Name="ChartArea1" BackColor="White">
                                            <%--<Area3DStyle Rotation="5" Perspective="10" Enable3D="false" LightStyle="Realistic"
                                    Inclination="2" PointDepth="200" IsRightAngleAxes="true" WallWidth="0" IsClustered="False" />--%>
                                            <%--<Position Y="2" Height="94" Width="94" X="2"></Position>--%>
                                            <Area3DStyle Rotation="25" Perspective="9" LightStyle="Realistic" Inclination="40"
                                                IsRightAngleAxes="False" WallWidth="3" IsClustered="False" />
                                            <AxisY LineColor="64, 64, 64, 64">
                                                <LabelStyle Font="Trebuchet MS, 11.25pt, style=Bold" />
                                                <MajorGrid LineColor="White" />
                                            </AxisY>
                                            <AxisX LineColor="64, 64, 64, 64">
                                                <LabelStyle Font="Trebuchet MS, 11.25pt, style=Bold" />
                                                <MajorGrid LineColor="White" />
                                            </AxisX>
                                        </asp:ChartArea>
                                    </ChartAreas>
                                </asp:Chart>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                &nbsp;
                            </td>
                            <td>
                                <table>
                                    <tr>
                                        <td>
                                            <asp:Chart ID="Chart2" runat="server" Width="450px" Palette="BrightPastel">
                                                <Titles>
                                                    <asp:Title Font="Trebuchet MS, 14.25pt, style=Bold" ForeColor="26, 59, 105">
                                                    </asp:Title>
                                                </Titles>
                                                <Legends>
                                                    <asp:Legend BackColor="Transparent" Alignment="Near" Docking="Top" Font="Trebuchet MS, 8.25pt, style=Bold"
                                                        IsTextAutoFit="true" Name="Default" LegendStyle="Row" Title="Legends" Enabled="false">
                                                    </asp:Legend>
                                                </Legends>
                                                <BorderSkin SkinStyle="None"></BorderSkin>
                                                <Series>
                                                    <asp:Series ChartArea="ChartArea1" BorderColor="180, 26, 59, 105" ChartType="Column"
                                                        BorderWidth="1" IsValueShownAsLabel="true" Name="Default">
                                                    </asp:Series>
                                                </Series>
                                                <ChartAreas>
                                                    <asp:ChartArea Name="ChartArea1" BackColor="White">
                                                        <Area3DStyle Rotation="25" Perspective="9" LightStyle="Realistic" Inclination="40"
                                                            IsRightAngleAxes="False" WallWidth="3" IsClustered="False" />
                                                        <AxisY LineColor="64, 64, 64, 64">
                                                            <LabelStyle Font="Trebuchet MS, 11.25pt, style=Bold" />
                                                            <MajorGrid LineColor="White" />
                                                        </AxisY>
                                                        <AxisX LineColor="64, 64, 64, 64">
                                                            <LabelStyle Font="Trebuchet MS, 11.25pt, style=Bold" IsEndLabelVisible="false" />
                                                            <MajorGrid LineColor="White" />
                                                        </AxisX>
                                                    </asp:ChartArea>
                                                </ChartAreas>
                                            </asp:Chart>
                                        </td>
                                        <td>
                                            <asp:Chart ID="Chart3" runat="server" Width="450px" BorderColor="#B54001">
                                                <Titles>
                                                    <asp:Title Font="Trebuchet MS, 14.25pt, style=Bold" ForeColor="26, 59, 105">
                                                    </asp:Title>
                                                </Titles>
                                                <Legends>
                                                    <asp:Legend BackColor="Transparent" Alignment="Near" Docking="Top" Font="Trebuchet MS, 8.25pt, style=Bold"
                                                        IsTextAutoFit="true" Name="Default" LegendStyle="Row" Title="Legends" Enabled="false">
                                                    </asp:Legend>
                                                </Legends>
                                                <BorderSkin SkinStyle="None"></BorderSkin>
                                                <Series>
                                                    <asp:Series ChartArea="ChartArea1" BorderColor="180, 26, 59, 105" ChartType="Column"
                                                        BorderWidth="1" IsValueShownAsLabel="true" Name="Default">
                                                    </asp:Series>
                                                </Series>
                                                <ChartAreas>
                                                    <asp:ChartArea Name="ChartArea1" BackColor="White">
                                                        <Area3DStyle Rotation="25" Perspective="9" LightStyle="Realistic" Inclination="40"
                                                            IsRightAngleAxes="False" WallWidth="3" IsClustered="False" />
                                                        <AxisY LineColor="64, 64, 64, 64">
                                                            <LabelStyle Font="Trebuchet MS, 11.25pt, style=Bold" />
                                                            <MajorGrid LineColor="White" />
                                                        </AxisY>
                                                        <AxisX LineColor="64, 64, 64, 64">
                                                            <LabelStyle Font="Trebuchet MS, 11.25pt, style=Bold" IsEndLabelVisible="false" />
                                                            <MajorGrid LineColor="White" />
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
