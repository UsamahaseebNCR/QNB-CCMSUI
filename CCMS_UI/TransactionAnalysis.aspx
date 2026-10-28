<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TransactionAnalysis.aspx.cs"
    Inherits="CCMSUI.TransactionAnalysis" %>

<%@ Register TagPrefix="ofc" Namespace="OpenFlashChart" Assembly="OpenFlashChart" %>
<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<!DOCTYPE html>
<%--<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">--%>
<html>
<%--xmlns="http://www.w3.org/1999/xhtml"--%>
<head>
    <title>Transaction Analysis</title>
    <link href="content/iSuite.css" rel="stylesheet" />
    <link href="content/cal/popcalendar.css" type="text/css" rel="stylesheet" />
    <script src="content/boxover.js" type="text/javascript" language="javascript">
    </script>
    <script src="content/shortcut.js" type="text/javascript" language="javascript"></script>
    <script language="javascript" src="content/cal/datepicker.js" type="text/javascript"></script>
    <script type='text/javascript' src='https://www.google.com/jsapi'></script>
    <asp:Literal ID="Literal_Script" runat="server"></asp:Literal>
    <link href="content/Scripts/ui-lightness/jquery-ui-1.7.2.custom.css" rel="stylesheet"
        type="text/css" />
    <script type="text/javascript" src="content/scripts/jquery-1.4.1.js"></script>
    <script type="text/javascript" src="content/scripts/ui.datepicker.js"></script>
    <script type="text/javascript">
        function receiveCallBackResult(response) {
            document.getElementById('trxnInfo').innerHTML = response;
        }


        $(function () {

            $('.datepicker').datepicker({
                dateFormat: 'dd/mm/yy',
                changeMonth: true,
                changeYear: true,
                showOn: "button",
                buttonImage: "images/calendar.gif",
                buttonImageOnly: true

            });
        });

        function displayTrxn(atmID, fromDate, toDate, taskId) {
            Dialog2.Show();
            DialogCallBack.callback(atmID, fromDate, toDate, taskId);
        }

    </script>
</head>
<body onload="hideProgressBar();">
    <form id="form1" runat="server">
    <div>
        <asp:Literal ID="Literal_Message" runat="server"></asp:Literal>
        <asp:Literal ID="Literal_Title" runat="server"></asp:Literal>
    </div>
    <ComponentArt:TabStrip ID="TabStrip_Transaction" CssClass="TopGroup" DefaultItemLookId="DefaultTabLook"
        DefaultSelectedItemLookId="SelectedTabLook" DefaultDisabledItemLookId="DisabledTabLook"
        DefaultGroupTabSpacing="1px" ImagesBaseUrl="images/Tabimages/" MultiPageId="MultiPage1"
        runat="server" ScrollDownLookId="DefaultTabLook" ScrollLeftLookId="DefaultTabLook"
        ScrollRightLookId="DefaultTabLook" ScrollUpLookId="DefaultTabLook" TopGroupTabSpacing="1px"
        Width="100%">
        <ItemLooks>
            <ComponentArt:ItemLook LookId="DefaultTabLook" CssClass="DefaultTab" HoverCssClass="DefaultTabHover"
                LabelPaddingLeft="10px" LabelPaddingRight="10px" LabelPaddingTop="5px" LabelPaddingBottom="4px"
                LeftIconUrl="tab_left_icon.gif" RightIconUrl="tab_right_icon.gif" HoverLeftIconUrl="hover_tab_left_icon.gif"
                HoverRightIconUrl="hover_tab_right_icon.gif" LeftIconWidth="3px" LeftIconHeight="21px"
                RightIconWidth="3px" RightIconHeight="21px" LeftIconVisibility="Always" RightIconVisibility="Always" />
            <ComponentArt:ItemLook LookId="SelectedTabLook" CssClass="SelectedReportsTab" LabelPaddingLeft="10px"
                LabelPaddingRight="10px" LabelPaddingTop="4px" LabelPaddingBottom="4px" LeftIconUrl="selected_tab_left_icon.gif"
                RightIconUrl="selected_tab_right_icon.gif" LeftIconWidth="3px" LeftIconHeight="21px"
                RightIconWidth="3px" RightIconHeight="21px" LeftIconVisibility="Always" RightIconVisibility="Always" />
        </ItemLooks>
        <Tabs>
            <ComponentArt:TabStripTab ID="TabStripTab3" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="KPI & Summary">
            </ComponentArt:TabStripTab>
            <ComponentArt:TabStripTab ID="TabStripTab4" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Incidents">
            </ComponentArt:TabStripTab>
            <ComponentArt:TabStripTab ID="TabStripTab2" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Enhanced EJ Analysis">
            </ComponentArt:TabStripTab>
            <ComponentArt:TabStripTab ID="TabStripTab_ATMsHourWise" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Hourly Analysis" >
            </ComponentArt:TabStripTab>
            <ComponentArt:TabStripTab ID="TabStripTab_WeeklyAnalysis" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Weekly Analysis">
            </ComponentArt:TabStripTab>
            <ComponentArt:TabStripTab ID="TabStripTab_MostLeastTransactingATMs" runat="server"
                ChildSelectedLook-LeftIconVisibility="Always" ChildSelectedLook-RightIconVisibility="Always"
                DefaultSubGroupTabSpacing="1px" DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Most & Least Transacting ATMs">
            </ComponentArt:TabStripTab>
            <ComponentArt:TabStripTab ID="TabStripTab_RegionTransactions" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Region wise analysis">
            </ComponentArt:TabStripTab>
            <ComponentArt:TabStripTab ID="TabStripTab_RegionTransactionHourlyAnalysis" runat="server"
                ChildSelectedLook-LeftIconVisibility="Always" ChildSelectedLook-RightIconVisibility="Always"
                DefaultSubGroupTabSpacing="1px" DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Region Transaction Hourly Analysis">
            </ComponentArt:TabStripTab>
            <ComponentArt:TabStripTab ID="TabStripTab1" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Multi Region Transaction Analysis">
            </ComponentArt:TabStripTab>
        </Tabs>
    </ComponentArt:TabStrip>
    <ComponentArt:MultiPage ID="MultiPage1" CssClass="MultiPage" runat="server">
        <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView8">
            <div>
                <table style="width: 100%; text-align: center" border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td colspan="3">
                            <asp:ValidationSummary ID="ValidationSummary2" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td align="center" style="font-size: large; background-color: Gray; color: White"
                            colspan="3">
                            KPI & Summary
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3">
                            <table style="background-color: #F3F3F3;" border="0" width="100%">
                                <tr>
                                    <td style="width: 23px">
                                        From
                                    </td>
                                    <td style="width: 190px; height: 17px">
                                        <asp:TextBox ID="TextBox13" runat="server" CssClass="datepicker"></asp:TextBox>
                                    </td>
                                    <%--<td style="width: 23px">
                                        <img src="images/IconCalendar.gif" alt="Click to open calender" onclick="javascript:show_calendar('form1.TextBox_From');"
                                            style="border: 0; cursor: hand" id="IMG15" />
                                    </td>--%>
                                    <td style="width: 3px">
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator14" CssClass="Validator"
                                            runat="server" ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                            ControlToValidate="TextBox_From" ErrorMessage="From Date is invalid.">*</asp:RegularExpressionValidator>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator14" runat="server" ControlToValidate="TextBox_From"
                                            ErrorMessage="From date cannot be empty">*</asp:RequiredFieldValidator>
                                    </td>
                                    <td style="width: 23px">
                                        To
                                    </td>
                                    <td style="width: 190px; height: 17px">
                                        <asp:TextBox ID="TextBox14" runat="server" CssClass="datepicker"></asp:TextBox>
                                    </td>
                                    <%--<td style="width: 23px">
                                        <img src="images/IconCalendar.gif" alt="Click to open calender" onclick="javascript:show_calendar('form1.TextBox_To');"
                                            style="border: 0; cursor: hand" id="IMG16" />
                                    </td>--%>
                                    <td style="width: 3px">
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator15" CssClass="Validator"
                                            runat="server" ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                            ControlToValidate="TextBox_To" ErrorMessage="To Date is invalid.">*</asp:RegularExpressionValidator>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator15" runat="server" ControlToValidate="TextBox_To"
                                            ErrorMessage="To date cannot be empty">*</asp:RequiredFieldValidator>
                                    </td>
                                    <td align="left">
                                        <asp:Button ID="Button10" runat="server" Text="Generate" OnClick="Button_Submit_Click"
                                            CssClass="button" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3">
                            <hr />
                        </td>
                    </tr>
                    <tr>
                        <td valign="top" >
                            <table border="0" width="100%">
                                <tr>
                                    <td align="left" valign="top" width="30%" >
                                        <table width="100%" border="0">
                                            <tr>
                                                <td style="font-size: large" colspan="2">
                                                    <b>ATM Transactions Summary</b>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="3">
                                                    <hr />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="width: 100px">
                                                    <div id='chart_div'>
                                                    </div>
                                                </td>
                                                </tr>

                                                <tr>
                                                <td>
                                                    <table>
                                                        <tr>
                                                            <td style="background-color: #DC3912; width: 25px">
                                                            </td>
                                                            <td>
                                                                Need Attention
                                                            </td>
                                                       
                                                            <td style="background-color: #FF9900; width: 25px"">
                                                            </td>
                                                            <td>
                                                                OK
                                                            </td>
                                                       
                                                            <td style="background-color: #109618; width: 25px"">
                                                            </td>
                                                            <td>
                                                                Good
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                        <%--<asp:Chart ID="ChartPIE" runat="server" Width="425" EnableViewState="true">
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
                                        </asp:Chart>--%>
                                    </td>
                                    <td valign="top"  >
                                        <table width="100%">
                                            <tr>
                                                <td style="font-size: medium; text-align: left">
                                                    <b>Service Availability</b>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td valign="top">
                                                    <div style="height: 200px; overflow: auto; vertical-align: top">
                                                        <asp:GridView ID="GridView_ServiceAvailability" runat="server" Width="100%" AutoGenerateColumns="true"
                                                            HorizontalAlign="Center" Font-Names="Calabri" Font-Size="15px" HeaderStyle-CssClass="HeadingCell"
                                                            OnRowDataBound="GridView_ServiceAvailability_RowDataBound">
                                                            <RowStyle HorizontalAlign="Left" />
                                                            <HeaderStyle CssClass="HeadingCell" />
                                                            <EmptyDataTemplate>
                                                                No records found
                                                            </EmptyDataTemplate>
                                                        </asp:GridView>
                                                    </div>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                <td>
                                    <b>KPI Summary</b>
                                </td>
                                    </tr>
                                <tr>
                                    <td align="left" valign="top">
                                        <b>Filter</b>
                                        <asp:DropDownList ID="DropDownList_Criteria" runat="server" CssClass="dropDown" AutoPostBack="true"
                                            OnSelectedIndexChanged="DropDownList_Criteria_Changed">
                                            <asp:ListItem Text="For Selected Criteria" Value="-1"></asp:ListItem>
                                            <asp:ListItem Text="Last 10 Days" Value="0"></asp:ListItem>
                                            <asp:ListItem Text="One Week" Value="1"></asp:ListItem>
                                            <asp:ListItem Text="Monthly" Value="2"></asp:ListItem>
                                            <asp:ListItem Text="Yearly" Value="3"></asp:ListItem>
                                        </asp:DropDownList>
                                        <div>
                                            <table>
                                                <tr>
                                                    <td>
                                                        <asp:Chart ID="ChartKPISummary" runat="server" Width="325" EnableViewState="true"
                                                            Height="200" Palette="BrightPastel">
                                                            <Titles>
                                                                <asp:Title Font="Trebuchet MS, 14.25pt, style=Bold" Name="Title1" ForeColor="26, 59, 105"
                                                                    Text="">
                                                                </asp:Title>
                                                            </Titles>
                                                            <Legends>
                                                                <%--<asp:Legend BackColor="Transparent" Alignment="Center" Docking="Right" Font="Trebuchet MS, 8.25pt, style=Bold"
                                                    IsTextAutoFit="False" Name="Default" LegendStyle="Column" Title="Legends" Enabled="true">
                                                </asp:Legend>--%>
                                                                <asp:Legend TitleFont="Microsoft Sans Serif, 8pt, style=Bold" Docking="Bottom" BackColor="Transparent"
                                                                    Font="Trebuchet MS, 8.25pt, style=Bold" IsTextAutoFit="False" Enabled="false"
                                                                    LegendStyle="Column" Name="Default">
                                                                </asp:Legend>
                                                            </Legends>
                                                            <BorderSkin SkinStyle="None"></BorderSkin>
                                                            <Series>
                                                                <asp:Series Name="Successful" ChartType="StackedColumn" IsValueShownAsLabel="true">
                                                                </asp:Series>
                                                                <asp:Series Name="Failed" ChartType="StackedColumn" IsValueShownAsLabel="true">
                                                                </asp:Series>
                                                                <asp:Series Name="Suspicious" ChartType="StackedColumn" IsValueShownAsLabel="true">
                                                                </asp:Series>
                                                            </Series>
                                                            <ChartAreas>
                                                                <asp:ChartArea Name="ChartArea1" BackColor="Transparent">
                                                                    <Area3DStyle Rotation="10" Inclination="15" WallWidth="0" Enable3D="true" />
                                                                    <Position Y="3" Height="92" Width="92" X="2"></Position>
                                                                    <AxisY LineColor="64, 64, 64, 64" LabelAutoFitMaxFontSize="8">
                                                                        <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                                        <MajorGrid LineColor="Transparent" />
                                                                    </AxisY>
                                                                    <AxisX LineColor="64, 64, 64, 64" LabelAutoFitMaxFontSize="8">
                                                                        <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                                        <MajorGrid LineColor="Transparent" />
                                                                    </AxisX>
                                                                </asp:ChartArea>
                                                            </ChartAreas>
                                                        </asp:Chart>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <table>
                                                            <tr>
                                                                <td style="background-color: #418cF0; width: 25px">
                                                                </td>
                                                                <td>
                                                                    Succesful
                                                                </td>
                                                                <td style="background-color: #FCB441; width: 25px">
                                                                </td>
                                                                <td>
                                                                    Failed
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                    </td>
                                    <td>
                                        <table width="100%">
                                            <tr>
                                                <td style="font-size: medium; text-align: left; vertical-align: top">
                                                    <b>Service Utilization</b>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <div style="height: 200px; overflow: auto; vertical-align: top">
                                                        <asp:GridView ID="GridView_ServiceUtil" runat="server" Width="100%" AutoGenerateColumns="true"
                                                            HorizontalAlign="Center" Font-Names="Calabri" Font-Size="15px">
                                                            <HeaderStyle BackColor="Gray" ForeColor="White" />
                                                            <%-- <Columns>
                                                            <asp:BoundField DataField="region_name" HeaderText="Region" ItemStyle-HorizontalAlign="Center"
                                                                HeaderStyle-CssClass="HeadingCell" ItemStyle-Font-Bold="true" />
                                                            <asp:BoundField DataField="dispensing" HeaderText="Cash Dispensing" ItemStyle-HorizontalAlign="Center"
                                                                HeaderStyle-CssClass="HeadingCell" ItemStyle-Font-Bold="true" />
                                                            <asp:BoundField DataField="statement" HeaderText="Mini Statement" ItemStyle-HorizontalAlign="Center"
                                                                HeaderStyle-CssClass="HeadingCell" ItemStyle-Font-Bold="true" />
                                                            <asp:BoundField DataField="balance" HeaderText="Balance Inquiry" ItemStyle-HorizontalAlign="Center"
                                                                HeaderStyle-CssClass="HeadingCell" ItemStyle-Font-Bold="true" />
                                                            <asp:BoundField DataField="cashDeposit" HeaderText="Cash Deposit" ItemStyle-HorizontalAlign="Center"
                                                                HeaderStyle-CssClass="HeadingCell" ItemStyle-Font-Bold="true" />
                                                            <asp:BoundField DataField="chqDeposit" HeaderText="Cheque Deposit" ItemStyle-HorizontalAlign="Center"
                                                                HeaderStyle-CssClass="HeadingCell" ItemStyle-Font-Bold="true" />
                                                        </Columns>--%>
                                                            <EmptyDataTemplate>
                                                                No records found
                                                            </EmptyDataTemplate>
                                                        </asp:GridView>
                                                    </div>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td valign="top" width="70%" align="left">
                            <table border="0" width="100%">
                                <tr>
                                    <td colspan="2">
                                        <table width="100%">
                                            <tr>
                                                <td style="font-size: medium; text-align: left;">
                                                    <b>ATM with incidents</b>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <div style="height: 200px; overflow: auto">
                                                        <asp:GridView ID="GridView_ATMIncidents" runat="server" Width="100%" AutoGenerateColumns="false"
                                                            HorizontalAlign="Center" Font-Names="Calabri" Font-Size="15px" OnRowDataBound="GridView_ATMIncidents_RowDataBound">
                                                            <Columns>
                                                                <asp:TemplateField>
                                                                    <ItemTemplate>
                                                                        <img src="images/notWorking.png" width="13" height="13" />
                                                                    </ItemTemplate>
                                                                    <HeaderStyle CssClass="HeadingCell" />
                                                                </asp:TemplateField>
                                                                <asp:BoundField DataField="title" HeaderText="Title" ItemStyle-HorizontalAlign="Left"
                                                                    HeaderStyle-CssClass="HeadingCell" ItemStyle-Font-Bold="true" />
                                                                <asp:BoundField DataField="location" HeaderText="Location" ItemStyle-HorizontalAlign="Left"
                                                                    HeaderStyle-CssClass="HeadingCell" ItemStyle-Font-Bold="true" />
                                                                <asp:BoundField DataField="alert_type" HeaderText="Type" ItemStyle-HorizontalAlign="Left"
                                                                    HeaderStyle-CssClass="HeadingCell" ItemStyle-Font-Bold="true" />
                                                                <asp:BoundField DataField="generated_at" HeaderText="Since" ItemStyle-HorizontalAlign="Left"
                                                                    HeaderStyle-CssClass="HeadingCell" ItemStyle-Font-Bold="true" />
                                                                <asp:BoundField DataField="outage" HeaderText="Duration Of Outage" ItemStyle-HorizontalAlign="Left"
                                                                    HeaderStyle-CssClass="HeadingCell" ItemStyle-Font-Bold="true" />
                                                            </Columns>
                                                            <EmptyDataTemplate>
                                                                No records found
                                                            </EmptyDataTemplate>
                                                        </asp:GridView>
                                                    </div>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <%--<tr>
                        <td align="left">
                            <input type="button" value="Back to graph list" class="button" onclick="history.go(-1);" />
                        </td>
                    </tr>--%>
                </table>
                <input type="button" runat="server" id="Button11" style="display: none" />
            </div>
        </ComponentArt:PageView>
        <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView9">
            <div>
                <table style="width: 100%; text-align: center" border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td colspan="3">
                            <asp:ValidationSummary ID="ValidationSummary3" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td align="center" style="font-size: large; background-color: Gray; color: White"
                            colspan="3">
                            Incidents
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3">
                            <table style="background-color: #F3F3F3;" border="0" width="100%" cellpadding="0"
                                cellspacing="0">
                                <tr>
                                    <td style="width: 23px">
                                        From
                                    </td>
                                    <td style="width: 190px">
                                        <asp:TextBox ID="TextBox15" runat="server" CssClass="datepicker"></asp:TextBox>
                                    </td>
                                    <td style="width: 3px">
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator16" CssClass="Validator"
                                            runat="server" ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                            ControlToValidate="TextBox_From" ErrorMessage="From Date is invalid.">*</asp:RegularExpressionValidator>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator16" runat="server" ControlToValidate="TextBox_From"
                                            ErrorMessage="From date cannot be empty">*</asp:RequiredFieldValidator>
                                    </td>
                                    <td style="width: 23px">
                                        To
                                    </td>
                                    <td style="width: 190px">
                                        <asp:TextBox ID="TextBox16" runat="server" CssClass="datepicker"></asp:TextBox>
                                    </td>
                                    <td style="width: 3px">
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator17" CssClass="Validator"
                                            runat="server" ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                            ControlToValidate="TextBox_To" ErrorMessage="To Date is invalid.">*</asp:RegularExpressionValidator>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator17" runat="server" ControlToValidate="TextBox_To"
                                            ErrorMessage="To date cannot be empty">*</asp:RequiredFieldValidator>
                                    </td>
                                    <td align="left">
                                        <asp:Button ID="Button12" runat="server" Text="Generate" OnClick="Button_Submit_Click"
                                            CssClass="button" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3">
                            <hr />
                        </td>
                    </tr>
                    <tr>
                        <td align="left">
                            <table border="0">
                                <tr>
                                    <td valign="top">
                                        <div style="height: 300px; overflow: auto">
                                            <asp:GridView ID="GridView_EventsSummary" runat="server" Width="100%" AutoGenerateColumns="false"
                                                HorizontalAlign="Center" Font-Names="Calabri" Font-Size="15px" OnRowCommand="GridView_EventsSummary_RowCommand">
                                                <HeaderStyle BackColor="Gray" ForeColor="White" />
                                                <Columns>
                                                    <asp:TemplateField HeaderText="Details" ItemStyle-HorizontalAlign="Center">
                                                        <ItemTemplate>
                                                            <asp:ImageButton runat="server" CommandName="viewDetails" CommandArgument='<%# Eval("keylookup_id") %>'
                                                                ToolTip="Click to view details" ImageUrl="images/file_info.png" ID="Button_viewDetails"
                                                                CssClass="ShowHand" Width="15" Height="15" />
                                                        </ItemTemplate>
                                                        <HeaderStyle CssClass="HeadingCell" />
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="key_name" HeaderText="Type" ItemStyle-HorizontalAlign="Left"
                                                        HeaderStyle-CssClass="HeadingCell" ItemStyle-Font-Bold="true" />
                                                    <asp:BoundField DataField="total" HeaderText="Total" ItemStyle-HorizontalAlign="Center"
                                                        HeaderStyle-CssClass="HeadingCell" ItemStyle-Font-Bold="true" />
                                                </Columns>
                                                <EmptyDataTemplate>
                                                    No records found
                                                </EmptyDataTemplate>
                                            </asp:GridView>
                                        </div>
                                    </td>
                                    <td valign="top">
                                        <div style="height: 300px; overflow: auto">
                                            <asp:GridView ID="GridView_EventDetails" runat="server" Width="100%" AutoGenerateColumns="false"
                                                HorizontalAlign="Center" Font-Names="Calabri" Font-Size="15px" OnRowCommand="GridView_EventsSummary_RowCommand"
                                                OnRowDataBound="GridView_EventDetails_RowDataBound">
                                                <HeaderStyle BackColor="Gray" ForeColor="White" />
                                                <Columns>
                                                    <asp:BoundField DataField="title" HeaderText="Title" ItemStyle-HorizontalAlign="Center"
                                                        HeaderStyle-CssClass="HeadingCell" ItemStyle-Font-Bold="true" />
                                                    <asp:BoundField DataField="key_name" HeaderText="Type" ItemStyle-HorizontalAlign="Center"
                                                        HeaderStyle-CssClass="HeadingCell" ItemStyle-Font-Bold="true" />
                                                    <asp:BoundField DataField="generated_at" HeaderText="Generated At" ItemStyle-HorizontalAlign="Center"
                                                        HeaderStyle-CssClass="HeadingCell" ItemStyle-Font-Bold="true" />
                                                    <asp:BoundField DataField="outage" HeaderText="Outage" ItemStyle-HorizontalAlign="Center"
                                                        HeaderStyle-CssClass="HeadingCell" ItemStyle-Font-Bold="true" />
                                                </Columns>
                                                <EmptyDataTemplate>
                                                    No records found
                                                </EmptyDataTemplate>
                                            </asp:GridView>
                                        </div>
                                    </td>
                                    <td valign="top">
                                        <asp:Chart ID="ChartAlertCountsByHour" runat="server" Width="400px" Height="300px"
                                            BorderColor="#B54001">
                                            <Titles>
                                                <asp:Title Font="Trebuchet MS, 14.25pt, style=Bold" ForeColor="26, 59, 105" Text="Incidence occurence analysis by hour">
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
                                                    BorderWidth="4" MarkerSize="10" MarkerStyle="Circle" IsValueShownAsLabel="true"
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
                            </table>
                        </td>
                    </tr>
                    <%--<tr>
                        <td>
                            <iframe id="iframeMap" runat="server" src="GoogleMaps.aspx" width="100%" height="400px">
                            </iframe>
                        </td>
                    </tr>--%>
                </table>
                <input type="button" runat="server" id="Button13" style="display: none" />
            </div>
        </ComponentArt:PageView>
        <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView7">
            <div>
                <table style="width: 100%; text-align: center" border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td colspan="3">
                            <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td align="center" style="font-size: large; background-color: Gray; color: White"
                            colspan="3">
                            Enhanced EJ Analysis
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3">
                            <table style="background-color: #F3F3F3;" border="0" width="100%" cellpadding="0"
                                cellspacing="0">
                                <tr>
                                    <td style="width: 23px">
                                        From
                                    </td>
                                    <td style="width: 190px">
                                        <asp:TextBox ID="TextBox11" runat="server" CssClass="datepicker"></asp:TextBox>
                                    </td>
                                    <td style="width: 3px">
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator12" CssClass="Validator"
                                            runat="server" ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                            ControlToValidate="TextBox_From" ErrorMessage="From Date is invalid.">*</asp:RegularExpressionValidator>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator12" runat="server" ControlToValidate="TextBox_From"
                                            ErrorMessage="From date cannot be empty">*</asp:RequiredFieldValidator>
                                    </td>
                                    <td style="width: 23px">
                                        To
                                    </td>
                                    <td style="width: 190px">
                                        <asp:TextBox ID="TextBox12" runat="server" CssClass="datepicker"></asp:TextBox>
                                    </td>
                                    <td style="width: 3px">
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator13" CssClass="Validator"
                                            runat="server" ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                            ControlToValidate="TextBox_To" ErrorMessage="To Date is invalid.">*</asp:RegularExpressionValidator>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator13" runat="server" ControlToValidate="TextBox_To"
                                            ErrorMessage="To date cannot be empty">*</asp:RequiredFieldValidator>
                                    </td>
                                    <td align="left">
                                        <asp:Button ID="Button8" runat="server" Text="Generate" OnClick="Button_Submit_Click"
                                            CssClass="button" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3">
                            <hr />
                        </td>
                    </tr>
                    <tr>
                        <td valign="top">
                            <ComponentArt:TabStrip ID="TabStrip_Summary" ScrollingEnabled="true" ScrollLeftLookId="ScrollItem"
                                EnableTheming="true" ScrollRightLookId="ScrollItem" CssClass="TopGroup" DefaultItemLookId="DefaultTabLook"
                                DefaultSelectedItemLookId="SelectedTabLook" DefaultDisabledItemLookId="DisabledTabLook"
                                DefaultGroupTabSpacing="1px" ImagesBaseUrl="../images/Tabimages/" MultiPageId="MultiPage_Summary"
                                Width="100%" runat="server">
                                <ItemLooks>
                                    <%-- <ComponentArt:ItemLook LookId="ScrollItem" CssClass="ScrollItem" HoverCssClass="ScrollItemHover"
                                                LabelPaddingLeft="5" LabelPaddingRight="5" LabelPaddingTop="0" LabelPaddingBottom="0" />--%>
                                    <ComponentArt:ItemLook LookId="DefaultTabLook" CssClass="DefaultTab" HoverCssClass="DefaultTabHover"
                                        LabelPaddingLeft="10px" LabelPaddingRight="10px" LabelPaddingTop="5px" LabelPaddingBottom="4px"
                                        LeftIconUrl="tab_left_icon.gif" RightIconUrl="tab_right_icon.gif" HoverLeftIconUrl="hover_tab_left_icon.gif"
                                        HoverRightIconUrl="hover_tab_right_icon.gif" LeftIconWidth="3px" LeftIconHeight="21px"
                                        RightIconWidth="3px" RightIconHeight="21px" LeftIconVisibility="Always" RightIconVisibility="Always" />
                                    <ComponentArt:ItemLook LookId="SelectedTabLook" CssClass="SelectedReportsTab" LabelPaddingLeft="10px"
                                        LabelPaddingRight="10px" LabelPaddingTop="4px" LabelPaddingBottom="4px" LeftIconUrl="selected_tab_left_icon.gif"
                                        RightIconUrl="selected_tab_right_icon.gif" LeftIconWidth="3px" LeftIconHeight="21px"
                                        RightIconWidth="3px" RightIconHeight="21px" LeftIconVisibility="Always" RightIconVisibility="Always" />
                                </ItemLooks>
                                <Tabs>
                                    <ComponentArt:TabStripTab ID="TabStripTab_RecentActivity" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                        ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                        DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                        Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                        SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Network">
                                    </ComponentArt:TabStripTab>
                                    <ComponentArt:TabStripTab ID="TabStripTab_Replenishments" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                        ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                        DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                        Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                        SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Top Performing">
                                    </ComponentArt:TabStripTab>
                                    <ComponentArt:TabStripTab ID="TabStripTab5" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                        ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                        DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                        Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                        SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Others">
                                    </ComponentArt:TabStripTab>
                                </Tabs>
                            </ComponentArt:TabStrip>
                            <ComponentArt:MultiPage ID="MultiPage_Summary" CssClass="MultiPage" runat="server"
                                Width="100%">
                                <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView_RecentActivity"
                                    Width="100%">
                                    <table border="0" width="100%">
                                        <tr>
                                            <td align="left" style="font-size: large">
                                                <b>Network Analysis</b>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="top" style="width: 20%">
                                                <asp:GridView ID="GridView_Summary" runat="server" Width="100%" AutoGenerateColumns="false"
                                                    HorizontalAlign="Center" Font-Names="Calabri" Font-Size="15px">
                                                    <HeaderStyle BackColor="Gray" ForeColor="White" />
                                                    <Columns>
                                                        <asp:BoundField DataField="type" HeaderText="Summary" ItemStyle-HorizontalAlign="Left"
                                                            HeaderStyle-CssClass="HeadingCell" ItemStyle-Font-Bold="true" />
                                                        <asp:BoundField DataField="val" HeaderText="" ItemStyle-HorizontalAlign="Center"
                                                            HeaderStyle-CssClass="HeadingCell" ItemStyle-Font-Bold="true" />
                                                    </Columns>
                                                    <EmptyDataTemplate>
                                                        No records found
                                                    </EmptyDataTemplate>
                                                </asp:GridView>
                                            </td>
                                            <td valign="top">
                                                <asp:GridView ID="GridView_NetworkStatistics" runat="server" Width="100%" AutoGenerateColumns="false"
                                                    HorizontalAlign="Center" Font-Names="Calabri" Font-Size="15px">
                                                    <HeaderStyle BackColor="Gray" ForeColor="White" />
                                                    <Columns>
                                                        <asp:BoundField DataField="pan_prefix" HeaderText="Network" ItemStyle-HorizontalAlign="Center"
                                                            HeaderStyle-CssClass="HeadingCell" ItemStyle-Font-Bold="true" />
                                                        <asp:BoundField DataField="total" HeaderText="%" ItemStyle-HorizontalAlign="Center"
                                                            HeaderStyle-CssClass="HeadingCell" ItemStyle-Font-Bold="true" />
                                                    </Columns>
                                                    <EmptyDataTemplate>
                                                        No records found
                                                    </EmptyDataTemplate>
                                                </asp:GridView>
                                            </td>
                                            <td>
                                                <asp:Chart ID="ChartNetworkStats" runat="server" Width="625" Height="300" EnableViewState="true">
                                                    <Titles>
                                                        <asp:Title Font="Trebuchet MS, 14.25pt, style=Bold" Name="Title1" ForeColor="26, 59, 105"
                                                            Text="Region Wise Network Analysis">
                                                        </asp:Title>
                                                    </Titles>
                                                    <Legends>
                                                        <%--<asp:Legend BackColor="Transparent" Alignment="Center" Docking="Right" Font="Trebuchet MS, 8.25pt, style=Bold"
                                                    IsTextAutoFit="False" Name="Default" LegendStyle="Column" Title="Legends" Enabled="true">
                                                </asp:Legend>--%>
                                                        <asp:Legend TitleFont="Microsoft Sans Serif, 8pt, style=Bold" Docking="Right" BackColor="Transparent"
                                                            Font="Trebuchet MS, 8.25pt, style=Bold" IsTextAutoFit="False" Enabled="true"
                                                            Name="Default">
                                                        </asp:Legend>
                                                    </Legends>
                                                    <BorderSkin SkinStyle="None"></BorderSkin>
                                                    <Series>
                                                        <%--<asp:Series Name="Default" ChartType="StackedColumn" BorderColor="180, 26, 59, 105"
                                                    Color="220, 65, 140, 240">
                                                </asp:Series>--%>
                                                    </Series>
                                                    <ChartAreas>
                                                        <asp:ChartArea Name="ChartArea1" BackColor="White">
                                                            <Area3DStyle Rotation="10" Inclination="15" WallWidth="0" />
                                                            <Position Y="3" Height="92" Width="92" X="2"></Position>
                                                            <AxisY LineColor="64, 64, 64, 64" LabelAutoFitMaxFontSize="8">
                                                                <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                                <MajorGrid LineColor="Transparent" />
                                                            </AxisY>
                                                            <AxisX LineColor="64, 64, 64, 64" LabelAutoFitMaxFontSize="8">
                                                                <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                                <MajorGrid LineColor="Transparent" />
                                                            </AxisX>
                                                        </asp:ChartArea>
                                                    </ChartAreas>
                                                </asp:Chart>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="font-size: large" colspan="3" align="left">
                                                <b>Region Wise Network Analysis Details</b>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="top" colspan="3">
                                                <div style="height: 400px; overflow: auto">
                                                    <asp:GridView ID="GridView_NetworkStatisticsDetails" runat="server" Width="100%"
                                                        AutoGenerateColumns="true" HorizontalAlign="Center" Font-Names="Calabri" Font-Size="15px">
                                                        <HeaderStyle BackColor="Gray" ForeColor="White" />
                                                        <EmptyDataTemplate>
                                                            No records found
                                                        </EmptyDataTemplate>
                                                    </asp:GridView>
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </ComponentArt:PageView>
                                <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView10" Width="100%">
                                    <table width="100%" style="color: Black" border="0" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td valign="top">
                                                <table width="100%" border="0" cellpadding="0" cellspacing="0">
                                                    <tr>
                                                        <td style="background-color: Gray; color: white; text-align: center; font-family: Calibri;
                                                            font-size: 20px" colspan="2" valign="top">
                                                            <b>Top 10 Performing and Lowest Performing ATMs</b>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <table width="100%">
                                                                <tr>
                                                                    <td>
                                                                        <asp:GridView ID="GridView_Transacting" runat="server" Width="100%" AutoGenerateColumns="false"
                                                                            HorizontalAlign="Center" Font-Names="Calabri" Font-Size="15px">
                                                                            <HeaderStyle BackColor="Gray" ForeColor="White" />
                                                                            <Columns>
                                                                                <asp:TemplateField>
                                                                                    <ItemTemplate>
                                                                                        <img src="images/treeatm1.jpg" />
                                                                                    </ItemTemplate>
                                                                                    <HeaderStyle CssClass="HeadingCell" Width="23px" />
                                                                                </asp:TemplateField>
                                                                                <asp:BoundField DataField="title" HeaderText="Title" ItemStyle-HorizontalAlign="Center"
                                                                                    HeaderStyle-CssClass="HeadingCell" ItemStyle-Font-Bold="true" ItemStyle-Width="43px" />
                                                                                <asp:BoundField DataField="location" HeaderText="Location" ItemStyle-HorizontalAlign="Center"
                                                                                    HeaderStyle-CssClass="HeadingCell" ItemStyle-Font-Bold="true" ItemStyle-Width="23px" />
                                                                                <asp:BoundField DataField="total" HeaderText="Total Transactions Count" ItemStyle-HorizontalAlign="Center"
                                                                                    HeaderStyle-CssClass="HeadingCell" ItemStyle-Font-Bold="true" ItemStyle-Width="23px" />
                                                                                <asp:BoundField DataField="totalDispensedCount" HeaderText="Total Withdrawal" ItemStyle-HorizontalAlign="Center"
                                                                                    HeaderStyle-CssClass="HeadingCell" ItemStyle-Font-Bold="true" ItemStyle-Width="23px" />
                                                                                <asp:BoundField DataField="totalDispensedAmount" HeaderText="Dispense Amount" ItemStyle-HorizontalAlign="Center"
                                                                                    HeaderStyle-CssClass="HeadingCell" ItemStyle-Font-Bold="true" ItemStyle-Width="23px" />
                                                                            </Columns>
                                                                            <EmptyDataTemplate>
                                                                                No records found
                                                                            </EmptyDataTemplate>
                                                                        </asp:GridView>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                        <td>
                                                            <table width="100%">
                                                                <tr>
                                                                    <td>
                                                                        <asp:GridView ID="GridView_NoTransactingATM" runat="server" Width="100%" AutoGenerateColumns="false"
                                                                            HorizontalAlign="Center" Font-Names="Calabri" Font-Size="15px">
                                                                            <HeaderStyle BackColor="Gray" ForeColor="White" />
                                                                            <Columns>
                                                                                <asp:TemplateField>
                                                                                    <ItemTemplate>
                                                                                        <img src="images/treeatm1.jpg" />
                                                                                    </ItemTemplate>
                                                                                    <HeaderStyle CssClass="HeadingCell" Width="23px" />
                                                                                </asp:TemplateField>
                                                                                <asp:BoundField DataField="title" HeaderText="Title" ItemStyle-HorizontalAlign="Center"
                                                                                    HeaderStyle-CssClass="HeadingCell" ItemStyle-Font-Bold="true" ItemStyle-Width="43px" />
                                                                                <asp:BoundField DataField="location" HeaderText="Location" ItemStyle-HorizontalAlign="Center"
                                                                                    HeaderStyle-CssClass="HeadingCell" ItemStyle-Font-Bold="true" ItemStyle-Width="33px" />
                                                                                <asp:BoundField DataField="total" HeaderText="Total Transactions Count" ItemStyle-HorizontalAlign="Center"
                                                                                    HeaderStyle-CssClass="HeadingCell" ItemStyle-Font-Bold="true" ItemStyle-Width="23px" />
                                                                                <asp:BoundField DataField="totalDispensedCount" HeaderText="Total Withdrawal" ItemStyle-HorizontalAlign="Center"
                                                                                    HeaderStyle-CssClass="HeadingCell" ItemStyle-Font-Bold="true" ItemStyle-Width="23px" />
                                                                                <asp:BoundField DataField="totalDispensedAmount" HeaderText="Dispense Amount" ItemStyle-HorizontalAlign="Center"
                                                                                    HeaderStyle-CssClass="HeadingCell" ItemStyle-Font-Bold="true" ItemStyle-Width="23px" />
                                                                            </Columns>
                                                                            <EmptyDataTemplate>
                                                                                No records found
                                                                            </EmptyDataTemplate>
                                                                        </asp:GridView>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                    <%--<tr>
                                                        <td colspan="2">
                                                            <iframe id="iframe1" runat="server" src="GoogleMaps.aspx" width="100%" height="400px">
                                                            </iframe>
                                                        </td>
                                                    </tr>--%>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </ComponentArt:PageView>
                                <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView11" Width="100%">
                                    <table width="100%" style="color: Black" border="0" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td>
                                                <asp:Chart ID="ChartTransactionTypeBar" runat="server" Width="425" EnableViewState="true">
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
                                                        <asp:ChartArea Name="ChartArea1" BackColor="White">
                                                            <Area3DStyle Rotation="0" />
                                                            <AxisY LineColor="64, 64, 64, 64">
                                                                <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                                <MajorGrid LineColor="White" />
                                                            </AxisY>
                                                            <AxisX LineColor="64, 64, 64, 64"  Interval="1">
                                                                <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                                <MajorGrid LineColor="White" />
                                                            </AxisX>
                                                        </asp:ChartArea>
                                                    </ChartAreas>
                                                </asp:Chart>
                                            </td>
                                            <td>
                                                <asp:Chart ID="ChartFailureReasons" runat="server" Width="425" EnableViewState="true">
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
                                                </asp:Chart>
                                            </td>
                                            <td>
                                                <table width="100%" style="color: Black" border="0" cellpadding="0" cellspacing="0">
                                                    <tr>
                                                        <td valign="top">
                                                            <table width="100%" border="0" cellpadding="0" cellspacing="0">
                                                                <tr>
                                                                    <td>
                                                                        <asp:Chart ID="ChartProfitablityAnalysis" runat="server" Width="350" EnableViewState="true">
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
                                                                        </asp:Chart>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="3">
                                                <table width="100%" style="color: Black" border="0">
                                                    <tr>
                                                        <td valign="top">
                                                            <table width="100%" border="0" cellpadding="0" cellspacing="0">
                                                                <tr>
                                                                    <td style="background-color: Gray; color: white; text-align: center; font-family: Calibri;
                                                                        font-size: 20px" colspan="2" valign="top">
                                                                        <b>Transaction Analysis</b>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td valign="top" colspan="2">
                                                                        <table width="100%">
                                                                            <tr>
                                                                                <td valign="top">
                                                                                    <div style="height: 300px; overflow: auto; vertical-align: top">
                                                                                        <asp:GridView ID="GridView_TrxnStatus" runat="server" Width="100%" AutoGenerateColumns="false"
                                                                                            HorizontalAlign="Center" Font-Names="Calabri" Font-Size="15px">
                                                                                            <HeaderStyle BackColor="Gray" ForeColor="White" />
                                                                                            <Columns>
                                                                                                <asp:TemplateField>
                                                                                                    <ItemTemplate>
                                                                                                        <img src="images/treeatm1.jpg" />
                                                                                                    </ItemTemplate>
                                                                                                    <HeaderStyle CssClass="HeadingCell" />
                                                                                                </asp:TemplateField>
                                                                                                <asp:BoundField DataField="title" HeaderText="Title" ItemStyle-HorizontalAlign="Center"
                                                                                                    HeaderStyle-CssClass="HeadingCell" ItemStyle-Font-Bold="true" />
                                                                                                <asp:TemplateField>
                                                                                                    <ItemTemplate>
                                                                                                        <a style="text-decoration: underline; cursor: hand" onclick="displayTrxn(document.getElementById('<%=TextBox11.ClientID %>').value
                                                                                                        ,document.getElementById('<%=TextBox12.ClientID %>').value,<%#Eval("atm_id")%>,1 );">
                                                                                                            <b>
                                                                                                                <%#Eval("totalTrxn")%></b> </a>
                                                                                                    </ItemTemplate>
                                                                                                    <HeaderStyle CssClass="HeadingCell" />
                                                                                                    <HeaderTemplate>
                                                                                                        Total Transactions
                                                                                                    </HeaderTemplate>
                                                                                                </asp:TemplateField>
                                                                                                <asp:TemplateField>
                                                                                                    <ItemTemplate>
                                                                                                        <a style="text-decoration: underline; cursor: hand" onclick="displayTrxn(document.getElementById('<%=TextBox11.ClientID %>').value
                                                                                                        ,document.getElementById('<%=TextBox12.ClientID %>').value,<%#Eval("atm_id")%>,2 );">
                                                                                                            <b>
                                                                                                                <%#Eval("successfulTotalTrxn")%></b> </a>
                                                                                                    </ItemTemplate>
                                                                                                    <HeaderStyle CssClass="HeadingCell" />
                                                                                                    <HeaderTemplate>
                                                                                                        Total Successful Transactions
                                                                                                    </HeaderTemplate>
                                                                                                </asp:TemplateField>
                                                                                                <asp:TemplateField>
                                                                                                    <ItemTemplate>
                                                                                                        <a style="text-decoration: underline; cursor: hand" onclick="displayTrxn(document.getElementById('<%=TextBox11.ClientID %>').value
                                                                                                        ,document.getElementById('<%=TextBox12.ClientID %>').value,<%#Eval("atm_id")%>,3 );">
                                                                                                            <b>
                                                                                                                <%#Eval("failedTotalTrxn")%></b> </a>
                                                                                                    </ItemTemplate>
                                                                                                    <HeaderStyle CssClass="HeadingCell" />
                                                                                                    <HeaderTemplate>
                                                                                                        Total Failed Transactions
                                                                                                    </HeaderTemplate>
                                                                                                </asp:TemplateField>
                                                                                                <asp:TemplateField>
                                                                                                    <ItemTemplate>
                                                                                                        <a style="text-decoration: underline; cursor: hand" onclick="displayTrxn(document.getElementById('<%=TextBox11.ClientID %>').value
                                                                                                        ,document.getElementById('<%=TextBox12.ClientID %>').value,<%#Eval("atm_id")%>,4 );">
                                                                                                            <b>
                                                                                                                <%#Eval("totalCashOut")%></b> </a>
                                                                                                    </ItemTemplate>
                                                                                                    <HeaderStyle CssClass="HeadingCell" />
                                                                                                    <HeaderTemplate>
                                                                                                        Total Cash Out
                                                                                                    </HeaderTemplate>
                                                                                                </asp:TemplateField>
                                                                                                <asp:TemplateField>
                                                                                                    <ItemTemplate>
                                                                                                        <a style="text-decoration: underline; cursor: hand" onclick="displayTrxn(document.getElementById('<%=TextBox11.ClientID %>').value
                                                                                                        ,document.getElementById('<%=TextBox12.ClientID %>').value,<%#Eval("atm_id")%>,5 );">
                                                                                                            <b>
                                                                                                                <%#Eval("totalRep")%></b> </a>
                                                                                                    </ItemTemplate>
                                                                                                    <HeaderStyle CssClass="HeadingCell" />
                                                                                                    <HeaderTemplate>
                                                                                                        Total Replenishments Amount
                                                                                                    </HeaderTemplate>
                                                                                                </asp:TemplateField>
                                                                                                <asp:TemplateField>
                                                                                                    <ItemTemplate>
                                                                                                        <a style="text-decoration: underline; cursor: hand" onclick="displayTrxn(document.getElementById('<%=TextBox11.ClientID %>').value
                                                                                                        ,document.getElementById('<%=TextBox12.ClientID %>').value,<%#Eval("atm_id")%>,6 );">
                                                                                                            <b>
                                                                                                                <%#Eval("onUs")%></b> </a>
                                                                                                    </ItemTemplate>
                                                                                                    <HeaderStyle CssClass="HeadingCell" />
                                                                                                    <HeaderTemplate>
                                                                                                        Total On Us Transactions
                                                                                                    </HeaderTemplate>
                                                                                                </asp:TemplateField>
                                                                                                <asp:TemplateField>
                                                                                                    <ItemTemplate>
                                                                                                        <a style="text-decoration: underline; cursor: hand" onclick="displayTrxn(document.getElementById('<%=TextBox11.ClientID %>').value
                                                                                                        ,document.getElementById('<%=TextBox12.ClientID %>').value,<%#Eval("atm_id")%>,7 );">
                                                                                                            <b>
                                                                                                                <%#Eval("OnUsAmount")%></b> </a>
                                                                                                    </ItemTemplate>
                                                                                                    <HeaderStyle CssClass="HeadingCell" />
                                                                                                    <HeaderTemplate>
                                                                                                        Amount
                                                                                                    </HeaderTemplate>
                                                                                                </asp:TemplateField>
                                                                                                <asp:TemplateField>
                                                                                                    <ItemTemplate>
                                                                                                        <a style="text-decoration: underline; cursor: hand" onclick="displayTrxn(document.getElementById('<%=TextBox11.ClientID %>').value
                                                                                                        ,document.getElementById('<%=TextBox12.ClientID %>').value,<%#Eval("atm_id")%>,8 );">
                                                                                                            <b>
                                                                                                                <%#Eval("notOnUs")%></b> </a>
                                                                                                    </ItemTemplate>
                                                                                                    <HeaderStyle CssClass="HeadingCell" />
                                                                                                    <HeaderTemplate>
                                                                                                        Total Not On Us Transactions
                                                                                                    </HeaderTemplate>
                                                                                                </asp:TemplateField>
                                                                                                <asp:TemplateField>
                                                                                                    <ItemTemplate>
                                                                                                        <a style="text-decoration: underline; cursor: hand" onclick="displayTrxn(document.getElementById('<%=TextBox11.ClientID %>').value
                                                                                                        ,document.getElementById('<%=TextBox12.ClientID %>').value,<%#Eval("atm_id")%>,9 );">
                                                                                                            <b>
                                                                                                                <%#Eval("notOnUsAmount")%></b> </a>
                                                                                                    </ItemTemplate>
                                                                                                    <HeaderStyle CssClass="HeadingCell" />
                                                                                                    <HeaderTemplate>
                                                                                                        Amount
                                                                                                    </HeaderTemplate>
                                                                                                </asp:TemplateField>
                                                                                                <asp:TemplateField>
                                                                                                    <ItemTemplate>
                                                                                                        <a style="text-decoration: underline; cursor: hand" onclick="displayTrxn(document.getElementById('<%=TextBox11.ClientID %>').value
                                                                                                        ,document.getElementById('<%=TextBox12.ClientID %>').value,<%#Eval("atm_id")%>,10 );">
                                                                                                            <b>
                                                                                                                <%#Eval("onUsDisputed")%></b> </a>
                                                                                                    </ItemTemplate>
                                                                                                    <HeaderStyle CssClass="HeadingCell" />
                                                                                                    <HeaderTemplate>
                                                                                                        Total Disputed On Us Transactions
                                                                                                    </HeaderTemplate>
                                                                                                </asp:TemplateField>
                                                                                                <asp:TemplateField>
                                                                                                    <ItemTemplate>
                                                                                                        <a style="text-decoration: underline; cursor: hand" onclick="displayTrxn(document.getElementById('<%=TextBox11.ClientID %>').value
                                                                                                        ,document.getElementById('<%=TextBox12.ClientID %>').value,<%#Eval("atm_id")%>,11 );">
                                                                                                            <b>
                                                                                                                <%#Eval("onUsAmountDisputed")%></b> </a>
                                                                                                    </ItemTemplate>
                                                                                                    <HeaderStyle CssClass="HeadingCell" />
                                                                                                    <HeaderTemplate>
                                                                                                        Amount
                                                                                                    </HeaderTemplate>
                                                                                                </asp:TemplateField>
                                                                                                <asp:TemplateField>
                                                                                                    <ItemTemplate>
                                                                                                        <a style="text-decoration: underline; cursor: hand" onclick="displayTrxn(document.getElementById('<%=TextBox11.ClientID %>').value
                                                                                                        ,document.getElementById('<%=TextBox12.ClientID %>').value,<%#Eval("atm_id")%>,12 );">
                                                                                                            <b>
                                                                                                                <%#Eval("notOnUsDisputed")%></b> </a>
                                                                                                    </ItemTemplate>
                                                                                                    <HeaderStyle CssClass="HeadingCell" />
                                                                                                    <HeaderTemplate>
                                                                                                        Total Not On Us Disputed Transactions
                                                                                                    </HeaderTemplate>
                                                                                                </asp:TemplateField>
                                                                                                <asp:TemplateField>
                                                                                                    <ItemTemplate>
                                                                                                        <a style="text-decoration: underline; cursor: hand" onclick="displayTrxn(document.getElementById('<%=TextBox11.ClientID %>').value
                                                                                                        ,document.getElementById('<%=TextBox12.ClientID %>').value,<%#Eval("atm_id")%>,13 );">
                                                                                                            <b>
                                                                                                                <%#Eval("notOnUsDisputedAmount")%></b> </a>
                                                                                                    </ItemTemplate>
                                                                                                    <HeaderStyle CssClass="HeadingCell" />
                                                                                                    <HeaderTemplate>
                                                                                                        Amount
                                                                                                    </HeaderTemplate>
                                                                                                </asp:TemplateField>
                                                                                            </Columns>
                                                                                            <EmptyDataTemplate>
                                                                                                No records found
                                                                                            </EmptyDataTemplate>
                                                                                        </asp:GridView>
                                                                                    </div>
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </ComponentArt:PageView>
                            </ComponentArt:MultiPage>
                        </td>
                    </tr>
                    <%--<tr>
                        <td align="left">
                            <input type="button" value="Back to graph list" class="button" onclick="history.go(-1);" />
                        </td>
                    </tr>--%>
                </table>
                <input type="button" runat="server" id="Button9" style="display: none" />
            </div>
        </ComponentArt:PageView>
        <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView1">
            <div>
                <table style="width: 100%; text-align: center" border="0">
                    <tr>
                        <td colspan="3">
                            <asp:ValidationSummary ID="valSummary" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td align="center" style="font-size: large; background-color: Gray; color: White"
                            colspan="3">
                            Transaction Analysis
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3">
                            <table style="background-color: InfoBackground;" border="0" width="100%">
                                <tr>
                                    <td style="width: 23px">
                                        From
                                    </td>
                                    <td style="width: 23px">
                                        <asp:TextBox ID="TextBox_From" runat="server" CssClass="textbox"></asp:TextBox>
                                    </td>
                                    <td style="width: 23px">
                                        <img src="images/IconCalendar.gif" alt="Click to open calender" onclick="javascript:show_calendar('form1.TextBox_From');"
                                            style="border: 0; cursor: hand" id="IMG1" />
                                    </td>
                                    <td style="width: 3px">
                                        <asp:RegularExpressionValidator ID="ValFromDate" CssClass="Validator" runat="server"
                                            ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                            ControlToValidate="TextBox_From" ErrorMessage="From Date is invalid.">*</asp:RegularExpressionValidator>
                                        <asp:RequiredFieldValidator ID="ReqFromDate" runat="server" ControlToValidate="TextBox_From"
                                            ErrorMessage="From date cannot be empty">*</asp:RequiredFieldValidator>
                                    </td>
                                    <td style="width: 23px">
                                        To
                                    </td>
                                    <td style="width: 23px">
                                        <asp:TextBox ID="TextBox_To" runat="server" CssClass="textbox"></asp:TextBox>
                                    </td>
                                    <td style="width: 23px">
                                        <img src="images/IconCalendar.gif" alt="Click to open calender" onclick="javascript:show_calendar('form1.TextBox_To');"
                                            style="border: 0; cursor: hand" id="IMG2" />
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
                        <td colspan="3">
                            <hr />
                        </td>
                    </tr>
                    <tr>
                        <td valign="top">
                            <table border="0" width="100%">
                                <tr>
                                    <td style="font-size: medium; text-align: center" onclick="javascript:window.open('DisplayData.aspx?gid=1&data='+this.innerHTML.replace('<B>','').replace('</B>','').replace('<BR/>','').replace('<BR>',''));">
                                        <asp:Literal ID="Literal1" runat="server"></asp:Literal>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                       <%-- <asp:Chart ID="Chart_ShowGraph" OnLoad="Chart_ShowGraph_Load" Height="460" Width="330px"
                                            runat="server" EnableViewState="true">  
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
    </asp:Chart>--%>
                                        <asp:Chart ID="Chart_ShowGraph" OnLoad="Chart_ShowGraph_Load" Height="230" Width="330px" runat="server" BackColor="#ffffff"   
        BackGradientStyle="LeftRight" Palette="None"   
        PaletteCustomColors="#00A77E">  
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

                                            </asp:Chart>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td>
                            <table border="0" width="100%">
                                <tr>
                                    <td>
                                        <table border="0" width="100%">
                                            <tr>
                                                <td style="font-size: medium; text-align: center" onclick="javascript:window.open('DisplayData.aspx?gid=1&data='+this.innerHTML.replace('<B>','').replace('</B>','').replace('<BR/>','').replace('<BR>',''));">
                                                    <asp:Literal ID="Literal2" runat="server"></asp:Literal>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Chart ID="Chart1" OnLoad="Chart1_Load" Height="230" Width="330px" runat="server" BackColor="#ffffff"   
        BackGradientStyle="LeftRight" Palette="None"   
        PaletteCustomColors="#008D9B">  
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

                                            </asp:Chart>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="font-size: medium; text-align: center" onclick="javascript:window.open('DisplayData.aspx?gid=1&data='+this.innerHTML.replace('<B>','').replace('</B>','').replace('<BR/>','').replace('<BR>',''));">
                                                    <asp:Literal ID="Literal3" runat="server"></asp:Literal>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Chart ID="Chart2" OnLoad="Chart2_Load" Height="230" Width="330px" runat="server" BackColor="#ffffff"   
        BackGradientStyle="LeftRight" Palette="None"   
        PaletteCustomColors="#008D98">  
         <Titles>
                                                    <asp:Title Font="Trebuchet MS, 14.25pt, style=Bold" Name="Title1" ForeColor="26, 59, 105">
                                                    </asp:Title>
                                                </Titles>
                                                <Legends>
                                                    <asp:Legend BackColor="Transparent" Alignment="Center" Docking="Right" Font="Trebuchet MS, 8.25pt, style=Bold"
                                                        IsTextAutoFit="False" Name="Default" LegendStyle="Column" Title="Legends" Enabled="false">
                                                    </asp:Legend>
                                                </Legends>
                                                <BorderSkin SkinStyle="None" BackColor ="#D4DDE0"></BorderSkin>
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
                                                            <MajorGrid LineColor="White" />
                                                        </AxisY>
                                                        <AxisX LineColor="64, 64, 64, 64">
                                                            <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
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
                        <td valign="top">
                            <table>
                                <tr>
                                    <td style="font-size: medium; text-align: center" onclick="javascript:window.open('DisplayData.aspx?gid=1&data='+this.innerHTML.replace('<B>','').replace('</B>','').replace('<BR/>','').replace('<BR>',''));">
                                        <asp:Literal ID="Literal4" runat="server"></asp:Literal>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Chart ID="Chart3" OnLoad="Chart3_Load" Height="460" Width="330px" runat="server" BackColor="#ffffff"   
        BackGradientStyle="LeftRight" Palette="None"   
        PaletteCustomColors="#008F9B">  
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

                                            </asp:Chart>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <%--<tr>
                        <td align="left">
                            <input type="button" value="Back to graph list" class="button" onclick="history.go(-1);" />
                        </td>
                    </tr>--%>
                </table>
                <input type="button" runat="server" id="refresh" style="display: none" />
            </div>
        </ComponentArt:PageView>
        <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView4">
            <div>
                <table style="width: 100%; text-align: center" border="0">
                    <tr>
                        <td align="center" style="font-size: large; background-color: Gray; color: White"
                            colspan="3">
                            Transaction Analysis
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3">
                            <table style="background-color: InfoBackground;" border="0" width="100%">
                                <tr>
                                    <td style="width: 23px">
                                        From
                                    </td>
                                    <td style="width: 23px">
                                        <asp:TextBox ID="TextBox5" runat="server" CssClass="textbox"></asp:TextBox>
                                    </td>
                                    <td style="width: 23px">
                                        <img src="images/IconCalendar.gif" alt="Click to open calender" onclick="javascript:show_calendar('form1.TextBox5');"
                                            style="border: 0; cursor: hand" id="IMG3" />
                                    </td>
                                    <td style="width: 3px">
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator2" CssClass="Validator"
                                            runat="server" ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                            ControlToValidate="TextBox5" ErrorMessage="From Date is invalid.">*</asp:RegularExpressionValidator>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="TextBox5"
                                            ErrorMessage="From date cannot be empty">*</asp:RequiredFieldValidator>
                                    </td>
                                    <td style="width: 23px">
                                        To
                                    </td>
                                    <td style="width: 23px">
                                        <asp:TextBox ID="TextBox6" runat="server" CssClass="textbox"></asp:TextBox>
                                    </td>
                                    <td style="width: 23px">
                                        <img src="images/IconCalendar.gif" alt="Click to open calender" onclick="javascript:show_calendar('form1.TextBox6');"
                                            style="border: 0; cursor: hand" id="IMG4" />
                                    </td>
                                    <td style="width: 3px">
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator3" CssClass="Validator"
                                            runat="server" ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                            ControlToValidate="TextBox6" ErrorMessage="To Date is invalid.">*</asp:RegularExpressionValidator>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="TextBox6"
                                            ErrorMessage="To date cannot be empty">*</asp:RequiredFieldValidator>
                                    </td>
                                    <td align="left">
                                        <asp:Button ID="Button4" runat="server" Text="Generate" OnClick="Button_Submit_Click"
                                            CssClass="button" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3">
                            <hr />
                        </td>
                    </tr>
                    <tr>
                        <%--<td valign="top">
                            <table border="0" width="100%">
                                <tr>
                                    <td style="font-size: medium; text-align: center" onclick="javascript:window.open('DisplayData.aspx?gid=1&data='+this.innerHTML.replace('<B>','').replace('</B>','').replace('<BR/>','').replace('<BR>',''));">
                                        <asp:Literal ID="Literal5" runat="server"></asp:Literal>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <ofc:Chart ID="Chart8" OnLoad="Chart8_Load" Height="460" Width="330px" runat="server" />
                                    </td>
                                </tr>
                            </table>
                        </td>--%>
                        <td>
                            <table border="0" width="100%">
                                <tr>
                                    <td>
                                        <table border="0" width="100%">
                                            <tr>
                                                <td style="font-size: medium; text-align: center" onclick="javascript:window.open('DisplayData.aspx?gid=1&data='+this.innerHTML.replace('<B>','').replace('</B>','').replace('<BR/>','').replace('<BR>',''));">
                                                    <asp:Literal ID="Literal16" runat="server"></asp:Literal>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Chart ID="Chart10" OnLoad="Chart10_Load" Height="230" Width="330px" runat="server" BackColor="#ffffff"   
        BackGradientStyle="LeftRight" Palette="None"   
        PaletteCustomColors="#00A77E">  
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
                                            </asp:Chart>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td>
                            <table border="0" width="100%">
                                <tr>
                                    <td>
                                        <table border="0" width="100%">
                                            <tr>
                                                <td style="font-size: medium; text-align: center" onclick="javascript:window.open('DisplayData.aspx?gid=1&data='+this.innerHTML.replace('<B>','').replace('</B>','').replace('<BR/>','').replace('<BR>',''));">
                                                    <asp:Literal ID="Literal6" runat="server"></asp:Literal>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Chart ID="Chart9" OnLoad="Chart9_Load" Height="230" Width="330px" runat="server" BackColor="#ffffff"   
        BackGradientStyle="LeftRight" Palette="None"   
        PaletteCustomColors="#008D9B">  
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

                                            </asp:Chart>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td valign="top">
                            <table>
                                <tr>
                                    <td style="font-size: medium; text-align: center" onclick="javascript:window.open('DisplayData.aspx?gid=1&data='+this.innerHTML.replace('<B>','').replace('</B>','').replace('<BR/>','').replace('<BR>',''));">
                                        <asp:Literal ID="Literal8" runat="server"></asp:Literal>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Chart ID="Chart11" OnLoad="Chart11_Load" Height="460" Width="330px" runat="server" BackColor="#ffffff"   
        BackGradientStyle="LeftRight" Palette="None"   
        PaletteCustomColors="#31A9E0">  
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

                                            </asp:Chart>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <%--<tr>
                        <td align="left">
                            <input type="button" value="Back to graph list" class="button" onclick="history.go(-1);" />
                        </td>
                    </tr>--%>
                </table>
                <input type="button" runat="server" id="Button5" style="display: none" />
            </div>
        </ComponentArt:PageView>
        <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView2">
            <div>
                <table style="width: 100%">
                    <tr>
                        <td align="center" style="font-size: large; background-color: Gray; color: White"
                            colspan="3">
                            Transaction Analysis
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <table style="background-color: InfoBackground; width: 100%">
                                <tr>
                                    <td style="width: 23px">
                                        From
                                    </td>
                                    <td style="width: 23px">
                                        <asp:TextBox ID="TextBox1" runat="server" CssClass="textbox"></asp:TextBox>
                                    </td>
                                    <td style="width: 23px">
                                        <img src="images/IconCalendar.gif" alt="Click to open calender" onclick="javascript:show_calendar('form1.TextBox1');"
                                            style="border: 0; cursor: hand" id="IMG5" />
                                    </td>
                                    <td style="width: 3px">
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator4" CssClass="Validator"
                                            runat="server" ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                            ControlToValidate="TextBox1" ErrorMessage="To Date is invalid.">*</asp:RegularExpressionValidator>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="TextBox1"
                                            ErrorMessage="To date cannot be empty">*</asp:RequiredFieldValidator>
                                    </td>
                                    <td style="width: 23px">
                                        To
                                    </td>
                                    <td style="width: 23px">
                                        <asp:TextBox ID="TextBox2" runat="server" CssClass="textbox"></asp:TextBox>
                                    </td>
                                    <td style="width: 23px">
                                        <img src="images/IconCalendar.gif" alt="Click to open calender" onclick="javascript:show_calendar('form1.TextBox2');"
                                            style="border: 0; cursor: hand" id="IMG6" />
                                    </td>
                                    <td style="width: 3px">
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator5" CssClass="Validator"
                                            runat="server" ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                            ControlToValidate="TextBox2" ErrorMessage="To Date is invalid.">*</asp:RegularExpressionValidator>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="TextBox2"
                                            ErrorMessage="To date cannot be empty">*</asp:RequiredFieldValidator>
                                    </td>
                                    <td align="left">
                                        <asp:Button ID="Button2" runat="server" Text="Generate" OnClick="Button_Submit_Click"
                                            CssClass="button" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <table>
                                <tr>
                                    <td>
                                        <table>
                                            <tr>
                                                <td style="font-size: medium; text-align: center">
                                                    <asp:Literal ID="Literal9" runat="server"></asp:Literal>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Chart ID="Chart4" OnLoad="Chart4_Load" Height="230" Width="1130px" runat="server" BackColor="#ffffff"   
        BackGradientStyle="LeftRight" Palette="None"   
        PaletteCustomColors="#008D9B">  
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
                                                <td style="font-size: medium; text-align: center">
                                                    <asp:Literal ID="Literal10" runat="server"></asp:Literal>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Chart ID="Chart5" OnLoad="Chart5_Load" Height="230" Width="1130px" runat="server" BackColor="#ffffff"   
        BackGradientStyle="LeftRight" Palette="None"   
        PaletteCustomColors="#00A77E">  
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
                                                <td style="font-size: medium; text-align: center">
                                                    <asp:Literal ID="Literal11" runat="server"></asp:Literal>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Chart ID="Chart6" OnLoad="Chart6_Load" Height="230" Width="1130px" runat="server" BackColor="#ffffff"   
        BackGradientStyle="LeftRight" Palette="None"   
        PaletteCustomColors="#31A9E0"> 
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

                                            </asp:Chart>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <%--<td valign="top">
                    <asp:GridView ID="Gridview_Data" runat="server">
                    </asp:GridView>
                </td>--%>
                    </tr>
                </table>
            </div>
        </ComponentArt:PageView>
        <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView3">
            <div>
                <table width="100%" border="0">
                    <tr>
                        <td align="center" style="font-size: large; background-color: Gray; color: White"
                            colspan="2">
                            Transaction Analysis
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <table style="background-color: InfoBackground; width: 100%" border="0">
                                <tr>
                                    <td style="width: 23px">
                                        From
                                    </td>
                                    <td style="width: 23px">
                                        <asp:TextBox ID="TextBox3" runat="server" CssClass="textbox"></asp:TextBox>
                                    </td>
                                    <td style="width: 23px">
                                        <img src="images/IconCalendar.gif" alt="Click to open calender" onclick="javascript:show_calendar('form1.TextBox3');"
                                            style="border: 0; cursor: hand" id="IMG7" />
                                    </td>
                                    <td style="width: 3px">
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator6" CssClass="Validator"
                                            runat="server" ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                            ControlToValidate="TextBox3" ErrorMessage="To Date is invalid.">*</asp:RegularExpressionValidator>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ControlToValidate="TextBox3"
                                            ErrorMessage="To date cannot be empty">*</asp:RequiredFieldValidator>
                                    </td>
                                    <td style="width: 23px">
                                        To
                                    </td>
                                    <td style="width: 23px">
                                        <asp:TextBox ID="TextBox4" runat="server" CssClass="textbox"></asp:TextBox>
                                    </td>
                                    <td style="width: 23px">
                                        <img src="images/IconCalendar.gif" alt="Click to open calender" onclick="javascript:show_calendar('form1.TextBox4');"
                                            style="border: 0; cursor: hand" id="IMG8" />
                                    </td>
                                    <td style="width: 3px">
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator7" CssClass="Validator"
                                            runat="server" ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                            ControlToValidate="TextBox4" ErrorMessage="To Date is invalid.">*</asp:RegularExpressionValidator>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" ControlToValidate="TextBox4"
                                            ErrorMessage="To date cannot be empty">*</asp:RequiredFieldValidator>
                                    </td>
                                    <td align="left">
                                        <asp:Button ID="Button3" runat="server" Text="Generate" OnClick="Button_Submit_Click"
                                            CssClass="button" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 800px">
                            <center>
                                <table style="margin-top: 20px;">
                                    <tr>
                                        <td valign="top">
                                            <asp:Chart ID="Chart7" OnLoad="Chart7_Load" Width="530px" runat="server"  EnableViewState="true">  
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
    </asp:Chart>
                                        </td>
                                    </tr>
                                </table>
                            </center>
                        </td>
                        <td valign="top">
                            <div style="width: 100%; overflow: auto" runat="server" id="divRegionComparison">
                                <asp:GridView ID="GridView_RegionTrxnDetail" runat="server" Width="100%" AutoGenerateColumns="false"
                                    HorizontalAlign="Center" AllowSorting="true" OnSorting="GridView_RegionTrxnDetail_Sorting"
                                    HeaderStyle-ForeColor="Black">
                                    <AlternatingRowStyle BackColor="#eff0ef" />
                                    <Columns>
                                        <asp:BoundField DataField="title" HeaderText="Title" ItemStyle-HorizontalAlign="Center"
                                            SortExpression="title" />
                                        <asp:BoundField DataField="region_name" HeaderText="Region" ItemStyle-HorizontalAlign="Center"
                                            SortExpression="region_name" />
                                        <asp:BoundField DataField="totalTrxn" HeaderText="Count" ItemStyle-HorizontalAlign="Center"
                                            SortExpression="totalTrxn" />
                                    </Columns>
                                    <EmptyDataTemplate>
                                        No records found
                                    </EmptyDataTemplate>
                                </asp:GridView>
                            </div>
                        </td>
                    </tr>
                </table>
            </div>
        </ComponentArt:PageView>
        <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView5">
            <div>
                <table width="100%" border="0">
                    <tr>
                        <td align="center" style="font-size: large; background-color: Gray; color: White"
                            colspan="3">
                            Transaction Analysis
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3">
                            <table style="background-color: InfoBackground; width: 100%" border="0">
                                <tr>
                                    <td style="width: 23px">
                                        From
                                    </td>
                                    <td style="width: 23px">
                                        <asp:TextBox ID="TextBox7" runat="server" CssClass="textbox"></asp:TextBox>
                                    </td>
                                    <td style="width: 23px">
                                        <img src="images/IconCalendar.gif" alt="Click to open calender" onclick="javascript:show_calendar('form1.TextBox7');"
                                            style="border: 0; cursor: hand" id="IMG9" />
                                    </td>
                                    <td style="width: 3px">
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator8" CssClass="Validator"
                                            runat="server" ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                            ControlToValidate="TextBox7" ErrorMessage="To Date is invalid.">*</asp:RegularExpressionValidator>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" ControlToValidate="TextBox7"
                                            ErrorMessage="To date cannot be empty">*</asp:RequiredFieldValidator>
                                    </td>
                                    <td style="width: 23px">
                                        To
                                    </td>
                                    <td style="width: 23px">
                                        <asp:TextBox ID="TextBox8" runat="server" CssClass="textbox"></asp:TextBox>
                                    </td>
                                    <td style="width: 23px">
                                        <img src="images/IconCalendar.gif" alt="Click to open calender" onclick="javascript:show_calendar('form1.TextBox8');"
                                            style="border: 0; cursor: hand" id="IMG10" />
                                    </td>
                                    <td style="width: 3px">
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator9" CssClass="Validator"
                                            runat="server" ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                            ControlToValidate="TextBox8" ErrorMessage="To Date is invalid.">*</asp:RegularExpressionValidator>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator9" runat="server" ControlToValidate="TextBox8"
                                            ErrorMessage="To date cannot be empty">*</asp:RequiredFieldValidator>
                                    </td>
                                    <td align="left">
                                        <asp:Button ID="Button6" runat="server" Text="Generate" OnClick="Button_Submit_Click"
                                            CssClass="button" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 800px">
                            <center>
                                <table style="margin-top: 20px;">
                                    <tr>
                                        <td style="font-size: medium" onclick="javascript:window.open('DisplayData.aspx?gid=21&data='+this.innerHTML.replace('<B>','').replace('</B>','').replace('<BR/>','').replace('<BR>',''));">
                                            <asp:Literal ID="Literal12" runat="server"></asp:Literal>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td valign="top">
                                            <asp:Chart ID="Chart12" OnLoad="Chart12_Load" Width="530px" runat="server"  EnableViewState="true" BackColor="#ffffff"   
        BackGradientStyle="LeftRight" Palette="None"   
        PaletteCustomColors="#00A77E">  
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
    </asp:Chart>
                                        </td>
                                    </tr>
                                </table>
                            </center>
                        </td>
                        <td style="width: 800px">
                            <center>
                                <table style="margin-top: 20px;">
                                    <tr>
                                        <td style="font-size: medium" onclick="javascript:window.open('DisplayData.aspx?gid=21&data='+this.innerHTML.replace('<B>','').replace('</B>','').replace('<BR/>','').replace('<BR>',''));">
                                            <asp:Literal ID="Literal13" runat="server"></asp:Literal>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td valign="top">
                                            <asp:Chart ID="Chart13" OnLoad="Chart13_Load" Width="530px" runat="server" EnableViewState="true" BackColor="#ffffff"   
        BackGradientStyle="LeftRight" Palette="None"   
        PaletteCustomColors="#008D9B">  
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
    </asp:Chart>
                                        </td>
                                    </tr>
                                </table>
                            </center>
                        </td>
                        <td style="width: 800px">
                            <center>
                                <table style="margin-top: 20px;">
                                    <tr>
                                        <td style="font-size: medium" onclick="javascript:window.open('DisplayData.aspx?gid=21&data='+this.innerHTML.replace('<B>','').replace('</B>','').replace('<BR/>','').replace('<BR>',''));">
                                            <asp:Literal ID="Literal14" runat="server"></asp:Literal>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td valign="top">
                                            <asp:Chart ID="Chart14" OnLoad="Chart14_Load" Width="530px" runat="server" EnableViewState="true" BackColor="#ffffff"   
        BackGradientStyle="LeftRight" Palette="None"   
        PaletteCustomColors="#0099BF">  
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
    </asp:Chart>
                                        </td>
                                    </tr>
                                </table>
                            </center>
                        </td>
                    </tr>
                </table>
            </div>
        </ComponentArt:PageView>
        <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView6">
            <div>
                <table width="100%" border="0">
                    <tr>
                        <td align="center" style="font-size: large; background-color: Gray; color: White"
                            colspan="3">
                            Transaction Analysis
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3">
                            <table style="background-color: InfoBackground; width: 100%" border="0">
                                <tr>
                                    <td style="width: 23px">
                                        From
                                    </td>
                                    <td style="width: 23px">
                                        <asp:TextBox ID="TextBox9" runat="server" CssClass="textbox"></asp:TextBox>
                                    </td>
                                    <td style="width: 23px">
                                        <img src="images/IconCalendar.gif" alt="Click to open calender" onclick="javascript:show_calendar('form1.TextBox9');"
                                            style="border: 0; cursor: hand" id="IMG11" />
                                    </td>
                                    <td style="width: 3px">
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator10" CssClass="Validator"
                                            runat="server" ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                            ControlToValidate="TextBox9" ErrorMessage="To Date is invalid.">*</asp:RegularExpressionValidator>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator10" runat="server" ControlToValidate="TextBox9"
                                            ErrorMessage="To date cannot be empty">*</asp:RequiredFieldValidator>
                                    </td>
                                    <td style="width: 23px">
                                        To
                                    </td>
                                    <td style="width: 23px">
                                        <asp:TextBox ID="TextBox10" runat="server" CssClass="textbox"></asp:TextBox>
                                    </td>
                                    <td style="width: 23px">
                                        <img src="images/IconCalendar.gif" alt="Click to open calender" onclick="javascript:show_calendar('form1.TextBox10');"
                                            style="border: 0; cursor: hand" id="IMG12" />
                                    </td>
                                    <td style="width: 3px">
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator11" CssClass="Validator"
                                            runat="server" ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                            ControlToValidate="TextBox10" ErrorMessage="To Date is invalid.">*</asp:RegularExpressionValidator>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator11" runat="server" ControlToValidate="TextBox10"
                                            ErrorMessage="To date cannot be empty">*</asp:RequiredFieldValidator>
                                    </td>
                                    <td align="left">
                                        <asp:Button ID="Button7" runat="server" Text="Generate" OnClick="Button_Submit_Click"
                                            CssClass="button" />
                                        
                                        
                                    </td>
                                    <td align="left"> 
                                        <asp:CheckBoxList ID="Checkbox_region" runat="server" RepeatColumns="2"
                                    RepeatDirection="Horizontal" RepeatLayout="Table"></asp:CheckBoxList>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <center>
                                <table style="margin-top: 20px;">
                                    <tr>
                                        <td style="font-size: medium" onclick="javascript:window.open('DisplayData.aspx?gid=21&data='+this.innerHTML.replace('<B>','').replace('</B>','').replace('<BR/>','').replace('<BR>',''));">
                                            <asp:Literal ID="Literal15" runat="server"></asp:Literal>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td valign="top">
                                            <%--<ofc:Chart ID="Chart15" OnLoad="Chart15_Load" Width="530px" runat="server" />--%>
                               <asp:Chart ID="Chart15" OnLoad="Chart15_Load" runat="server" Width="951px" BorderColor="#B54001">
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
<%--                                        <asp:Series ChartArea="ChartArea1" BorderColor="180, 26, 59, 105" ChartType="Spline"
                                            BorderWidth="4" MarkerSize="4" MarkerStyle="Circle" IsValueShownAsLabel="true"
                                            Name="">
                                        </asp:Series>--%>
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
                                </table>
                            </center>
                        </td>
                    </tr>
                </table>
            </div>
        </ComponentArt:PageView>
    </ComponentArt:MultiPage>
    <ComponentArt:CallBack ID="DialogCallBack" runat="server" Width="0" Height="0" OnCallback="DialogCallBack_Callback">
    </ComponentArt:CallBack>
    <ComponentArt:Dialog ContentCssClass="contentCss" FooterCssClass="footerCss" HeaderCssClass="headerCss"
        CssClass="dialogCss" Icon="" Value="" Title="AJAX TabStrip Content" CloseTransition="Fade"
        ModalMaskCssClass="modalMaskCssClass" AllowDrag="false" Alignment="TopCentre"
        Modal="true" ID="Dialog2" runat="server" Width="710" ContentClientTemplateId="ClientTemplate1"
        AnimationType="None" AnimationPath="Direct" AnimationSlide="Linear" ShowTransition="None"
        TransitionDuration="0" AnimationDuration="0">
        <ClientTemplates>
            <ComponentArt:ClientTemplate ID="ClientTemplate1">
                <table cellpadding="0" style="border-color: #c9c9c9" cellspacing="0" border="0" width="100%"
                    onmousedown="Dialog2.StartDrag(event);">
                    <tr>
                        <td style="background-image: url(images/DialogImages/top.gif); height: 45px;">
                            <table width="100%" cellpadding="0" cellspacing="0" border="0">
                                <tr>
                                    <td valign="middle" style="padding-left: 4px; color: white; font-size: 15px; font-family: Verdana;
                                        font-weight: bold;">
                                        Information
                                    </td>
                                    <td align="right" style="padding-right: 10px;">
                                        <img src="images/DialogImages/close.gif" onclick="Dialog2.Close();" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td style="border-color: #c9c9c9; background-color: white; font-size: 12px; font-family: Arial;
                            padding: 10px;">
                            <div id="trxnInfo" style="overflow: auto; height: 400px">
                            </div>
                        </td>
                    </tr>
                </table>
            </ComponentArt:ClientTemplate>
        </ClientTemplates>
    </ComponentArt:Dialog>
    </form>
</body>
</html>
