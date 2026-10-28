<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ForecastingList.aspx.cs"
    MaintainScrollPositionOnPostback="true" Culture="fr-FR" Inherits="CCMSUI.CCMS.ForecastingList" %>

<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="../content/ISUITE.css" rel="stylesheet" />
    <title>Forecasting</title>
    <link href="../content/iSuite.css" rel="stylesheet" />
    <script src="../content/shortcut.js" type="text/javascript" language="javascript"></script>
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

        function displaySummary() {
            //alert(document.getElementById('<%=hidden_info.ClientID %>'));
            Dialog1.Show();
            var arr = document.getElementById('<%=hidden_info.ClientID %>').value.split(',');

            var htmlString = "<h1>CIT Trips Summary</h1><hr/>";
            htmlString += "<table border=1>";
            htmlString += "<tr><td style='background-color:gray;color:white;'><b>Frequency</b></td><td style='background-color:gray;color:white;'><b>Count</b></td></tr>";
            htmlString += "<tr><td><b>Daily</b></td><td><b>" + arr[0] + "</b></td></tr>";
            htmlString += "<tr><td><b>Once A Week</b></td><td><b>" + arr[1] + "</b></td></tr>";
            htmlString += "<tr><td><b>Twice A Week</b></td><td><b>" + arr[2] + "</b></td></tr>";
            htmlString += "<tr><td><b>Thrice A Week</b></td><td><b>" + arr[3] + "</b></td></tr>";
            htmlString += "<tr><td><b>Biweekly</b></td><td><b>" + arr[4] + "</b></td></tr>";
            htmlString += "<tr><td><b>Monthly</b></td><td><b>" + arr[5] + "</b></td></tr></table>";
            htmlString += "<br/><br/>";
            htmlString += "<h1>Expected Amount Summary</h1><hr/>";
            htmlString += "<table border=1>";
            htmlString += "<tr><td style='background-color:gray;color:white;'><b>Frequency</b></td><td style='background-color:gray;color:white;'><b>Count</b></td></tr>";
            htmlString += "<tr><td><b>Daily</b></td><td><b>" + arr[6] + "</b></td></tr>";
            htmlString += "<tr><td><b>Once A Week</b></td><td><b>" + arr[7] + "</b></td></tr>";
            htmlString += "<tr><td><b>Twice A Week</b></td><td><b>" + arr[8] + "</b></td></tr>";
            htmlString += "<tr><td><b>Thrice A Week</b></td><td><b>" + arr[9] + "</b></td></tr>";
            htmlString += "<tr><td><b>Biweekly</b></td><td><b>" + arr[10] + "</b></td></tr>";
            htmlString += "<tr><td><b>Monthly</b></td><td><b>" + arr[11] + "</b></td></tr>";
            htmlString += "</table>";

            document.getElementById('summaryInfo').innerHTML = htmlString;

        }


        function openDialog(atmID) {
            Dialog1.Show(); DialogCallBack.callback(atmID);
        } function receiveCallBackResult(response) {
            document.getElementById('summaryInfo').innerHTML
    = response;
        }

    </script>
</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)'
    onload="hideProgressBar();">
    <form id="form1" runat="server">
    <input type="hidden" id="hidden_info" runat="server" />
    <div>
        <table style="width: 100%" cellspacing="0" cellpadding="0" border="0" class="mainbox">
            <tr>
                <td class="PageHeading" align="left" colspan="4">
                    Forecasting
                </td>
            </tr>
            <tr>
                <td class="ValidationText" align="center" colspan="2">
                    <asp:Literal ID="Literal_error" runat="server" EnableViewState="False"></asp:Literal>
                </td>
            </tr>
            <tr>
                <td class="ConfirmationText" align="center" colspan="2">
                    <%=Session[SessionVars.notice.ToString()]%>
                    <%Session[SessionVars.notice.ToString()] = "";%>
                    <asp:Literal ID="Literal_ok" runat="server" EnableViewState="False"></asp:Literal>
                </td>
            </tr>
            <tr>
                <td>
                    <ComponentArt:TabStrip ID="TabStrip_AddEdit" CssClass="TopGroup" DefaultItemLookId="DefaultTabLook"
                        DefaultSelectedItemLookId="SelectedTabLook" DefaultDisabledItemLookId="DisabledTabLook"
                        DefaultGroupTabSpacing="1px" ImagesBaseUrl="../images/Tabimages/" MultiPageId="MultiPage_AddEdit"
                        runat="server" ScrollDownLookId="DefaultTabLook" ScrollLeftLookId="DefaultTabLook"
                        ScrollRightLookId="DefaultTabLook" ScrollUpLookId="DefaultTabLook" TopGroupTabSpacing="1px">
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
                            <ComponentArt:TabStripTab ID="TabStripTab_analysis" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Data Analysis">
                            </ComponentArt:TabStripTab>
                            <ComponentArt:TabStripTab ID="TabStripTab1" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Trend Analysis">
                            </ComponentArt:TabStripTab>
                            <ComponentArt:TabStripTab ID="TabStripTab_Parameters" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Parameters">
                            </ComponentArt:TabStripTab>
                            <ComponentArt:TabStripTab ID="TabStripTab_ForecastedData" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Forecasted Data">
                            </ComponentArt:TabStripTab>
                            <ComponentArt:TabStripTab ID="TabStripTab2" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Forecasted Data Lead Days">
                            </ComponentArt:TabStripTab>
                            <ComponentArt:TabStripTab ID="TabStripTab3" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Forecasting Adjustment">
                            </ComponentArt:TabStripTab>
                            <ComponentArt:TabStripTab ID="TabStripTab28" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Forecasted Vs Actual Analysis"
                                AutoPostBackOnSelect="true" Width="100%">
                            </ComponentArt:TabStripTab>
                            <ComponentArt:TabStripTab ID="TabStripTab29" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Forecasting Quality Analysis"
                                AutoPostBackOnSelect="true" Width="100%">
                            </ComponentArt:TabStripTab>
                        </Tabs>
                    </ComponentArt:TabStrip>
                    <ComponentArt:MultiPage ID="MultiPage_AddEdit" CssClass="MultiPage" runat="server"
                        Width="100%">
                        <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView2" Width="100%">
                            <table width="100%" border="0">
                                <tr>
                                    <td colspan="5">
                                        <table width="100%" border="0">
                                            <tr>
                                                <td colspan="5">
                                                    <asp:Panel ID="Panel1" runat="server" Width="100%">
                                                        <table width="100%" border="0">
                                                            <tr>
                                                                <td colspan="5" style="background-color: #54b948; color: white; text-align: center;
                                                                    font-family: Arial; font-size: 20px">
                                                                    Data Analysis History ( Step 1 )
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <table width="50%">
                                                                        <tr>
                                                                            <td style="font-weight: bold">
                                                                                <asp:RadioButtonList ID="RadioButton_AnalysisOpt" runat="server" AutoPostBack="true"
                                                                                    Width="100%" OnSelectedIndexChanged="RadioButton_AnalysisOpt_Click">
                                                                                    <asp:ListItem Selected="True">Import 13 Months History</asp:ListItem>
                                                                                    <asp:ListItem>Exising Customer</asp:ListItem>
                                                                                    <asp:ListItem>No History Available</asp:ListItem>
                                                                                </asp:RadioButtonList>
                                                                            </td>
                                                                            <td id="fileUploadCtrlRow" runat="server" valign="top">
                                                                                <asp:FileUpload ID="FileUpload_ImportHistory" runat="server" />
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <hr />
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td align="left" colspan="2">
                                                                    <asp:Button ID="Button_Next" runat="server" Text="Next >>" OnClick="Button_Next_Click"
                                                                        CssClass="button" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </asp:Panel>
                                                    <asp:Panel ID="Panel_DisplayHistoryOfMissingATMs" runat="server">
                                                        <table width="100%">
                                                            <tr>
                                                                <td colspan="4" style="background-color: #54b948; color: white; text-align: center;
                                                                    font-family: Arial; font-size: 20px">
                                                                    ATMs With Missing History
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td colspan="5" align="right">
                                                                    <asp:Button ID="Button_Next_1" runat="server" Text="Start Analysis" OnClick="Button_Next1_Click"
                                                                        CssClass="button" />
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td valign="top">
                                                                    <asp:GridView ID="GridView_MissingHistory" runat="server" AutoGenerateColumns="False"
                                                                        Width="100%" AllowPaging="false">
                                                                        <AlternatingRowStyle BackColor="#eff0ef" />
                                                                        <Columns>
                                                                            <asp:TemplateField HeaderStyle-CssClass="HeadingCell">
                                                                                <ItemTemplate>
                                                                                    <img src="../images/alert.jpg" />
                                                                                </ItemTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:BoundField DataField="title" HeaderText="Title" HeaderStyle-ForeColor="black"
                                                                                ItemStyle-HorizontalAlign="Left">
                                                                                <HeaderStyle CssClass="HeadingCell" />
                                                                            </asp:BoundField>
                                                                            <asp:BoundField DataField="location" HeaderText="Location" HeaderStyle-ForeColor="black"
                                                                                ItemStyle-HorizontalAlign="Left">
                                                                                <HeaderStyle CssClass="HeadingCell" />
                                                                            </asp:BoundField>
                                                                            <asp:BoundField DataField="missingCount" HeaderText="Count" HeaderStyle-ForeColor="black"
                                                                                ItemStyle-HorizontalAlign="Right" ItemStyle-Width="40px">
                                                                                <HeaderStyle CssClass="HeadingCell" />
                                                                            </asp:BoundField>
                                                                            <asp:TemplateField ShowHeader="False" HeaderText="Details" ItemStyle-HorizontalAlign="Center"
                                                                                HeaderStyle-CssClass="HeadingCell">
                                                                                <ItemTemplate>
                                                                                    <div id="ShowHideRightsIcon" runat="server">
                                                                                        <img style="border: 0" src="../images/atmDetail.jpg" alt="click to view details"
                                                                                            onclick="openDialog('<%#Eval("atm_id") %>,0')" class="ShowHand" />
                                                                                    </div>
                                                                                </ItemTemplate>
                                                                                <HeaderStyle ForeColor="black" />
                                                                            </asp:TemplateField>
                                                                        </Columns>
                                                                        <HeaderStyle BackColor="Black" ForeColor="White" CssClass="HeadingCell" />
                                                                    </asp:GridView>
                                                                </td>
                                                                <td valign="top">
                                                                    <asp:GridView ID="GridView_MissingHistory_1" runat="server" AutoGenerateColumns="False"
                                                                        Width="100%" AllowPaging="false">
                                                                        <AlternatingRowStyle BackColor="#eff0ef" />
                                                                        <Columns>
                                                                            <asp:TemplateField HeaderStyle-CssClass="HeadingCell">
                                                                                <ItemTemplate>
                                                                                    <img src="../images/alert.jpg" />
                                                                                </ItemTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:BoundField DataField="title" HeaderText="Title" HeaderStyle-ForeColor="black"
                                                                                ItemStyle-HorizontalAlign="Left">
                                                                                <HeaderStyle CssClass="HeadingCell" />
                                                                            </asp:BoundField>
                                                                            <asp:BoundField DataField="location" HeaderText="Location" HeaderStyle-ForeColor="black"
                                                                                ItemStyle-HorizontalAlign="Left">
                                                                                <HeaderStyle CssClass="HeadingCell" />
                                                                            </asp:BoundField>
                                                                            <asp:BoundField DataField="missingCount" HeaderText="Count" HeaderStyle-ForeColor="black"
                                                                                ItemStyle-HorizontalAlign="Right" ItemStyle-Width="40px">
                                                                                <HeaderStyle CssClass="HeadingCell" />
                                                                            </asp:BoundField>
                                                                            <asp:TemplateField ShowHeader="False" HeaderText="Details" ItemStyle-HorizontalAlign="Center"
                                                                                HeaderStyle-CssClass="HeadingCell">
                                                                                <ItemTemplate>
                                                                                    <div id="ShowHideRightsIcon" runat="server">
                                                                                        <img style="border: 0" src="../images/atmDetail.jpg" alt="click to view details"
                                                                                            onclick="openDialog('<%#Eval("atm_id") %>,0')" class="ShowHand" />
                                                                                    </div>
                                                                                </ItemTemplate>
                                                                                <HeaderStyle ForeColor="black" />
                                                                            </asp:TemplateField>
                                                                        </Columns>
                                                                        <HeaderStyle BackColor="Black" ForeColor="White" CssClass="HeadingCell" />
                                                                    </asp:GridView>
                                                                </td>
                                                                <td valign="top">
                                                                    <asp:GridView ID="GridView_MissingHistory_2" runat="server" AutoGenerateColumns="False"
                                                                        Width="100%" AllowPaging="false">
                                                                        <AlternatingRowStyle BackColor="#eff0ef" />
                                                                        <Columns>
                                                                            <asp:TemplateField HeaderStyle-CssClass="HeadingCell">
                                                                                <ItemTemplate>
                                                                                    <img src="../images/alert.jpg" />
                                                                                </ItemTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:BoundField DataField="title" HeaderText="Title" HeaderStyle-ForeColor="black"
                                                                                ItemStyle-HorizontalAlign="Left">
                                                                                <HeaderStyle CssClass="HeadingCell" />
                                                                            </asp:BoundField>
                                                                            <asp:BoundField DataField="location" HeaderText="Location" HeaderStyle-ForeColor="black"
                                                                                ItemStyle-HorizontalAlign="Left">
                                                                                <HeaderStyle CssClass="HeadingCell" />
                                                                            </asp:BoundField>
                                                                            <asp:BoundField DataField="missingCount" HeaderText="Count" HeaderStyle-ForeColor="black"
                                                                                ItemStyle-HorizontalAlign="Right" ItemStyle-Width="40px">
                                                                                <HeaderStyle CssClass="HeadingCell" />
                                                                            </asp:BoundField>
                                                                            <asp:TemplateField ShowHeader="False" HeaderText="Details" ItemStyle-HorizontalAlign="Center"
                                                                                HeaderStyle-CssClass="HeadingCell">
                                                                                <ItemTemplate>
                                                                                    <div id="ShowHideRightsIcon" runat="server">
                                                                                        <img style="border: 0" src="../images/atmDetail.jpg" alt="click to view details"
                                                                                            onclick="openDialog('<%#Eval("atm_id") %>,0')" class="ShowHand" />
                                                                                    </div>
                                                                                </ItemTemplate>
                                                                                <HeaderStyle ForeColor="black" />
                                                                            </asp:TemplateField>
                                                                        </Columns>
                                                                        <HeaderStyle BackColor="Black" ForeColor="White" CssClass="HeadingCell" />
                                                                    </asp:GridView>
                                                                </td>
                                                                <td valign="top">
                                                                    <asp:GridView ID="GridView_MissingHistory_3" runat="server" AutoGenerateColumns="False"
                                                                        Width="100%" AllowPaging="false">
                                                                        <AlternatingRowStyle BackColor="#eff0ef" />
                                                                        <Columns>
                                                                            <asp:TemplateField HeaderStyle-CssClass="HeadingCell">
                                                                                <ItemTemplate>
                                                                                    <img src="../images/alert.jpg" />
                                                                                </ItemTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:BoundField DataField="title" HeaderText="Title" HeaderStyle-ForeColor="black"
                                                                                ItemStyle-HorizontalAlign="Left">
                                                                                <HeaderStyle CssClass="HeadingCell" />
                                                                            </asp:BoundField>
                                                                            <asp:BoundField DataField="location" HeaderText="Location" HeaderStyle-ForeColor="black"
                                                                                ItemStyle-HorizontalAlign="Left">
                                                                                <HeaderStyle CssClass="HeadingCell" />
                                                                            </asp:BoundField>
                                                                            <asp:BoundField DataField="missingCount" HeaderText="Count" HeaderStyle-ForeColor="black"
                                                                                ItemStyle-HorizontalAlign="Right" ItemStyle-Width="40px">
                                                                                <HeaderStyle CssClass="HeadingCell" />
                                                                            </asp:BoundField>
                                                                            <asp:TemplateField ShowHeader="False" HeaderText="Details" ItemStyle-HorizontalAlign="Center"
                                                                                HeaderStyle-CssClass="HeadingCell">
                                                                                <ItemTemplate>
                                                                                    <div id="ShowHideRightsIcon" runat="server">
                                                                                        <img style="border: 0" src="../images/atmDetail.jpg" alt="click to view details"
                                                                                            onclick="openDialog('<%#Eval("atm_id") %>,0')" class="ShowHand" />
                                                                                    </div>
                                                                                </ItemTemplate>
                                                                                <HeaderStyle ForeColor="black" />
                                                                            </asp:TemplateField>
                                                                        </Columns>
                                                                        <HeaderStyle ForeColor="White" CssClass="HeadingCell" />
                                                                    </asp:GridView>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </asp:Panel>
                                                    <asp:Panel ID="Panel_ATMZone" runat="server" Width="100%">
                                                        <table width="100%">
                                                            <tr>
                                                                <td colspan="4" style="background-color: #54b948; color: white; text-align: center;
                                                                    font-family: Arial; font-size: 20px">
                                                                    ATMs Grouping Based On Variance
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td align="right" colspan="3">
                                                                    <asp:Button ID="Button1" runat="server" Text="Step 2" OnClick="Button_Next2_Click"
                                                                        CssClass="button" />
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td style="background-color: Green; width: 33%">
                                                                    &nbsp;
                                                                </td>
                                                                <td style="background-color: Orange; width: 33%">
                                                                    &nbsp;
                                                                </td>
                                                                <td style="background-color: Red; width: 34%">
                                                                    &nbsp;
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td valign="top">
                                                                    <asp:GridView ID="GridView_GreenZone" runat="server" AutoGenerateColumns="False"
                                                                        Width="100%" AllowPaging="false" OnRowDataBound="GridView_GreenZone_RowDataBound"
                                                                        OnRowCommand="GridView_Zone_RowCommand">
                                                                        <AlternatingRowStyle BackColor="#eff0ef" />
                                                                        <Columns>
                                                                            <asp:BoundField DataField="title" HeaderText="Title" HeaderStyle-ForeColor="black"
                                                                                ItemStyle-HorizontalAlign="Left">
                                                                                <HeaderStyle CssClass="HeadingCell" />
                                                                            </asp:BoundField>
                                                                            <asp:BoundField DataField="location" HeaderText="Location" HeaderStyle-ForeColor="black"
                                                                                ItemStyle-HorizontalAlign="Left">
                                                                                <HeaderStyle CssClass="HeadingCell" />
                                                                            </asp:BoundField>
                                                                            <asp:TemplateField ShowHeader="False" HeaderText="Details" ItemStyle-HorizontalAlign="Center"
                                                                                HeaderStyle-CssClass="HeadingCell">
                                                                                <ItemTemplate>
                                                                                    <div id="ShowHideRightsIcon" runat="server">
                                                                                        <img style="border: 0" src="../images/atmDetail.jpg" alt="click to view details"
                                                                                            onclick="openDialog('<%#Eval("atm_id") %>,1')" class="ShowHand" />
                                                                                    </div>
                                                                                </ItemTemplate>
                                                                                <HeaderStyle ForeColor="black" />
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Move To Orange Zone">
                                                                                <ItemTemplate>
                                                                                    <asp:ImageButton runat="server" CommandName="MoveToOrangeZoneFromGreenZone" CommandArgument='<%# Eval("atm_id") %>'
                                                                                        ToolTip="Click to Move this ATM to Orange Zone" ImageUrl="../images/orangebullet.png"
                                                                                        ID="Button_OrangeZoneFromGreenZone" CssClass="ShowHand" Width="16" Height="16" />
                                                                                </ItemTemplate>
                                                                                <HeaderStyle CssClass="HeadingCell" />
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Move To Red Zone">
                                                                                <ItemTemplate>
                                                                                    <asp:ImageButton runat="server" CommandName="MoveToRedZoneFromGreenZone" CommandArgument='<%# Eval("atm_id") %>'
                                                                                        ToolTip="Click to Move this ATM to Red Zone" ImageUrl="../images/redbullet.png"
                                                                                        ID="Button_RedZoneFromGreenZone" CssClass="ShowHand" Width="16" Height="16" />
                                                                                </ItemTemplate>
                                                                                <HeaderStyle CssClass="HeadingCell" />
                                                                            </asp:TemplateField>
                                                                        </Columns>
                                                                        <HeaderStyle BackColor="Black" ForeColor="White" CssClass="HeadingCell" />
                                                                    </asp:GridView>
                                                                </td>
                                                                <td valign="top">
                                                                    <asp:GridView ID="GridView_Orange" runat="server" AutoGenerateColumns="False" Width="100%"
                                                                        AllowPaging="false" OnRowDataBound="GridView_OrangeZone_RowDataBound" OnRowCommand="GridView_Zone_RowCommand">
                                                                        <AlternatingRowStyle BackColor="#eff0ef" />
                                                                        <Columns>
                                                                            <asp:BoundField DataField="title" HeaderText="Title" HeaderStyle-ForeColor="black"
                                                                                ItemStyle-HorizontalAlign="Left">
                                                                                <HeaderStyle CssClass="HeadingCell" />
                                                                            </asp:BoundField>
                                                                            <asp:BoundField DataField="location" HeaderText="Location" HeaderStyle-ForeColor="black"
                                                                                ItemStyle-HorizontalAlign="Left">
                                                                                <HeaderStyle CssClass="HeadingCell" />
                                                                            </asp:BoundField>
                                                                            <asp:TemplateField ShowHeader="False" HeaderText="Details" ItemStyle-HorizontalAlign="Center"
                                                                                HeaderStyle-CssClass="HeadingCell">
                                                                                <ItemTemplate>
                                                                                    <div id="ShowHideRightsIcon" runat="server">
                                                                                        <img style="border: 0" src="../images/atmDetail.jpg" alt="click to view details"
                                                                                            onclick="openDialog('<%#Eval("atm_id") %>,1')" class="ShowHand" />
                                                                                    </div>
                                                                                </ItemTemplate>
                                                                                <HeaderStyle ForeColor="black" />
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Move To Green Zone">
                                                                                <ItemTemplate>
                                                                                    <asp:ImageButton runat="server" CommandName="MoveToGreenZoneFromOrangeZone" CommandArgument='<%# Eval("atm_id") %>'
                                                                                        ToolTip="Click to Move this ATM to Green Zone" ImageUrl="../images/greenbullet.jpg"
                                                                                        ID="Button_GreenZoneFromOrangeZone" CssClass="ShowHand" Width="16" Height="16" />
                                                                                </ItemTemplate>
                                                                                <HeaderStyle CssClass="HeadingCell" />
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Move To Red Zone">
                                                                                <ItemTemplate>
                                                                                    <asp:ImageButton runat="server" CommandName="MoveToRedZoneFromOrangeZone" CommandArgument='<%# Eval("atm_id") %>'
                                                                                        ToolTip="Click to Move this ATM to Red Zone" ImageUrl="../images/redbullet.png"
                                                                                        ID="Button_RedZoneFromOrangeZone" CssClass="ShowHand" Width="16" Height="16" />
                                                                                    </a>
                                                                                </ItemTemplate>
                                                                                <HeaderStyle CssClass="HeadingCell" />
                                                                            </asp:TemplateField>
                                                                        </Columns>
                                                                        <HeaderStyle BackColor="Black" ForeColor="White" CssClass="HeadingCell" />
                                                                    </asp:GridView>
                                                                </td>
                                                                <td valign="top">
                                                                    <asp:GridView ID="GridView_RedZone" runat="server" AutoGenerateColumns="False" Width="100%"
                                                                        AllowPaging="false" OnRowDataBound="GridView_RedZone_RowDataBound" OnRowCommand="GridView_Zone_RowCommand">
                                                                        <AlternatingRowStyle BackColor="#eff0ef" />
                                                                        <Columns>
                                                                            <asp:BoundField DataField="title" HeaderText="Title" HeaderStyle-ForeColor="black"
                                                                                ItemStyle-HorizontalAlign="Left">
                                                                                <HeaderStyle CssClass="HeadingCell" />
                                                                            </asp:BoundField>
                                                                            <asp:BoundField DataField="location" HeaderText="Location" HeaderStyle-ForeColor="black"
                                                                                ItemStyle-HorizontalAlign="Left">
                                                                                <HeaderStyle CssClass="HeadingCell" />
                                                                            </asp:BoundField>
                                                                            <asp:TemplateField ShowHeader="False" HeaderText="Details" ItemStyle-HorizontalAlign="Center"
                                                                                HeaderStyle-CssClass="HeadingCell">
                                                                                <ItemTemplate>
                                                                                    <div id="ShowHideRightsIcon" runat="server">
                                                                                        <img style="border: 0" src="../images/atmDetail.jpg" alt="click to view details"
                                                                                            onclick="openDialog('<%#Eval("atm_id") %>,1')" class="ShowHand" />
                                                                                    </div>
                                                                                </ItemTemplate>
                                                                                <HeaderStyle ForeColor="black" />
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Move To Green Zone">
                                                                                <ItemTemplate>
                                                                                    <asp:ImageButton runat="server" CommandName="MoveToGreenZoneFromRedZone" CommandArgument='<%# Eval("atm_id") %>'
                                                                                        ToolTip="Click to Move this ATM to Green Zone" ImageUrl="../images/greenbullet.jpg"
                                                                                        ID="Button_GreenZoneFromRedZone" CssClass="ShowHand" Width="16" Height="16" />
                                                                                </ItemTemplate>
                                                                                <HeaderStyle CssClass="HeadingCell" />
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Move To Orange Zone">
                                                                                <ItemTemplate>
                                                                                    <asp:ImageButton runat="server" CommandName="MoveToOrangeZoneFromRedZone" CommandArgument='<%# Eval("atm_id") %>'
                                                                                        ToolTip="Click to Move this ATM to Orange Zone" ImageUrl="../images/orangebullet.png"
                                                                                        ID="Button_OrangeZoneFromRedZone" CssClass="ShowHand" Width="16" Height="16" />
                                                                                    </a>
                                                                                </ItemTemplate>
                                                                                <HeaderStyle CssClass="HeadingCell" />
                                                                            </asp:TemplateField>
                                                                        </Columns>
                                                                        <HeaderStyle BackColor="Black" ForeColor="White" CssClass="HeadingCell" />
                                                                    </asp:GridView>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </asp:Panel>
                                                    <asp:Panel ID="Panel_RepGroup" runat="server" Width="100%">
                                                        <table width="100%">
                                                            <tr>
                                                                <td colspan="3" style="background-color: #54b948; color: white; text-align: center;
                                                                    font-family: Arial; font-size: 20px">
                                                                    ATMs Replenishment Schedule
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td style="background-color: Gray; color: white; text-align: center; font-family: Calibri;
                                                                    font-size: 15px" valign="top">
                                                                    <b>Daily Replenishment ( Withdrawal >= 600,000 )</b>
                                                                </td>
                                                                <td style="background-color: Gray; color: white; text-align: center; font-family: Calibri;
                                                                    font-size: 15px" valign="top">
                                                                    <b>Once a Week ( Withdrawal [100,000-199,999] )</b>
                                                                </td>
                                                                <td style="background-color: Gray; color: white; text-align: center; font-family: Calibri;
                                                                    font-size: 15px" valign="top">
                                                                    <b>Twice a Week ( Withdrawal [200,000-399,999] )</b>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td valign="top" width="33%">
                                                                    <asp:GridView ID="GridView_DailyRep" runat="server" AutoGenerateColumns="False" Width="100%"
                                                                        AllowPaging="false">
                                                                        <AlternatingRowStyle BackColor="#eff0ef" />
                                                                        <Columns>
                                                                            <asp:BoundField DataField="title" HeaderText="Title" HeaderStyle-ForeColor="black"
                                                                                ItemStyle-HorizontalAlign="Left">
                                                                                <HeaderStyle CssClass="HeadingCell" />
                                                                            </asp:BoundField>
                                                                            <asp:BoundField DataField="location" HeaderText="Location" HeaderStyle-ForeColor="black"
                                                                                ItemStyle-HorizontalAlign="Left">
                                                                                <HeaderStyle CssClass="HeadingCell" />
                                                                            </asp:BoundField>
                                                                            <asp:BoundField DataField="efrequency" HeaderText="Frequency" HeaderStyle-ForeColor="black"
                                                                                ItemStyle-HorizontalAlign="Left">
                                                                                <HeaderStyle CssClass="HeadingCell" />
                                                                            </asp:BoundField>
                                                                            <asp:TemplateField ShowHeader="False" HeaderText="Details" ItemStyle-HorizontalAlign="Center"
                                                                                HeaderStyle-CssClass="HeadingCell">
                                                                                <ItemTemplate>
                                                                                    <div id="ShowHideRightsIcon" runat="server">
                                                                                        <img style="border: 0" src="../images/atmDetail.jpg" alt="click to view details"
                                                                                            onclick="openDialog('<%#Eval("atm_id") %>,2')" class="ShowHand" />
                                                                                    </div>
                                                                                </ItemTemplate>
                                                                                <HeaderStyle ForeColor="black" />
                                                                            </asp:TemplateField>
                                                                        </Columns>
                                                                        <HeaderStyle BackColor="Black" ForeColor="White" CssClass="HeadingCell" />
                                                                        <EmptyDataTemplate>
                                                                            No Rows Found!
                                                                        </EmptyDataTemplate>
                                                                    </asp:GridView>
                                                                </td>
                                                                <td valign="top" width="33%">
                                                                    <asp:GridView ID="GridView_1perweek" runat="server" AutoGenerateColumns="False" Width="100%"
                                                                        AllowPaging="false">
                                                                        <AlternatingRowStyle BackColor="#eff0ef" />
                                                                        <Columns>
                                                                            <asp:BoundField DataField="title" HeaderText="Title" HeaderStyle-ForeColor="black"
                                                                                ItemStyle-HorizontalAlign="Left">
                                                                                <HeaderStyle CssClass="HeadingCell" />
                                                                            </asp:BoundField>
                                                                            <asp:BoundField DataField="location" HeaderText="Location" HeaderStyle-ForeColor="black"
                                                                                ItemStyle-HorizontalAlign="Left">
                                                                                <HeaderStyle CssClass="HeadingCell" />
                                                                            </asp:BoundField>
                                                                            <asp:BoundField DataField="efrequency" HeaderText="Frequency" HeaderStyle-ForeColor="black"
                                                                                ItemStyle-HorizontalAlign="Left">
                                                                                <HeaderStyle CssClass="HeadingCell" />
                                                                            </asp:BoundField>
                                                                            <asp:TemplateField ShowHeader="False" HeaderText="Details" ItemStyle-HorizontalAlign="Center"
                                                                                HeaderStyle-CssClass="HeadingCell">
                                                                                <ItemTemplate>
                                                                                    <div id="ShowHideRightsIcon" runat="server">
                                                                                        <img style="border: 0" src="../images/atmDetail.jpg" alt="click to view details"
                                                                                            onclick="openDialog('<%#Eval("atm_id") %>,2')" class="ShowHand" />
                                                                                    </div>
                                                                                </ItemTemplate>
                                                                                <HeaderStyle ForeColor="black" />
                                                                            </asp:TemplateField>
                                                                        </Columns>
                                                                        <HeaderStyle BackColor="Black" ForeColor="White" CssClass="HeadingCell" />
                                                                        <EmptyDataTemplate>
                                                                            No Rows Found!
                                                                        </EmptyDataTemplate>
                                                                    </asp:GridView>
                                                                </td>
                                                                <td valign="top" width="34%">
                                                                    <asp:GridView ID="GridView_2perweek" runat="server" AutoGenerateColumns="False" Width="100%"
                                                                        AllowPaging="false">
                                                                        <AlternatingRowStyle BackColor="#eff0ef" />
                                                                        <Columns>
                                                                            <asp:BoundField DataField="title" HeaderText="Title" HeaderStyle-ForeColor="black"
                                                                                ItemStyle-HorizontalAlign="Left">
                                                                                <HeaderStyle CssClass="HeadingCell" />
                                                                            </asp:BoundField>
                                                                            <asp:BoundField DataField="location" HeaderText="Location" HeaderStyle-ForeColor="black"
                                                                                ItemStyle-HorizontalAlign="Left">
                                                                                <HeaderStyle CssClass="HeadingCell" />
                                                                            </asp:BoundField>
                                                                            <asp:BoundField DataField="efrequency" HeaderText="Frequency" HeaderStyle-ForeColor="black"
                                                                                ItemStyle-HorizontalAlign="Left">
                                                                                <HeaderStyle CssClass="HeadingCell" />
                                                                            </asp:BoundField>
                                                                            <asp:TemplateField ShowHeader="False" HeaderText="Details" ItemStyle-HorizontalAlign="Center"
                                                                                HeaderStyle-CssClass="HeadingCell">
                                                                                <ItemTemplate>
                                                                                    <div id="ShowHideRightsIcon" runat="server">
                                                                                        <img style="border: 0" src="../images/atmDetail.jpg" alt="click to view details"
                                                                                            onclick="openDialog('<%#Eval("atm_id") %>,2')" class="ShowHand" />
                                                                                    </div>
                                                                                </ItemTemplate>
                                                                                <HeaderStyle ForeColor="black" />
                                                                            </asp:TemplateField>
                                                                        </Columns>
                                                                        <HeaderStyle BackColor="Black" ForeColor="White" CssClass="HeadingCell" />
                                                                        <EmptyDataTemplate>
                                                                            No Rows Found!
                                                                        </EmptyDataTemplate>
                                                                    </asp:GridView>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td style="background-color: Gray; color: white; text-align: center; font-family: Calibri;
                                                                    font-size: 15px" valign="top">
                                                                    <b>Thrice a Week ( Withdrawal [400,000-599,999] )</b>
                                                                </td>
                                                                <td style="background-color: Gray; color: white; text-align: center; font-family: Calibri;
                                                                    font-size: 15px" valign="top">
                                                                    <b>Biweekly ( Withdrawal [50,000-99,999] )</b>
                                                                </td>
                                                                <td style="background-color: Gray; color: white; text-align: center; font-family: Calibri;
                                                                    font-size: 15px" valign="top">
                                                                    <b>Monthly ( Withdrawal [49,999] )</b>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td valign="top">
                                                                    <asp:GridView ID="GridView_3perweek" runat="server" AutoGenerateColumns="False" Width="100%"
                                                                        AllowPaging="false">
                                                                        <AlternatingRowStyle BackColor="#eff0ef" />
                                                                        <Columns>
                                                                            <asp:BoundField DataField="title" HeaderText="Title" HeaderStyle-ForeColor="black"
                                                                                ItemStyle-HorizontalAlign="Left">
                                                                                <HeaderStyle CssClass="HeadingCell" />
                                                                            </asp:BoundField>
                                                                            <asp:BoundField DataField="location" HeaderText="Location" HeaderStyle-ForeColor="black"
                                                                                ItemStyle-HorizontalAlign="Left">
                                                                                <HeaderStyle CssClass="HeadingCell" />
                                                                            </asp:BoundField>
                                                                            <asp:BoundField DataField="efrequency" HeaderText="Frequency" HeaderStyle-ForeColor="black"
                                                                                ItemStyle-HorizontalAlign="Left">
                                                                                <HeaderStyle CssClass="HeadingCell" />
                                                                            </asp:BoundField>
                                                                            <asp:TemplateField ShowHeader="False" HeaderText="Details" ItemStyle-HorizontalAlign="Center"
                                                                                HeaderStyle-CssClass="HeadingCell">
                                                                                <ItemTemplate>
                                                                                    <div id="ShowHideRightsIcon" runat="server">
                                                                                        <img style="border: 0" src="../images/atmDetail.jpg" alt="click to view details"
                                                                                            onclick="openDialog('<%#Eval("atm_id") %>,2')" class="ShowHand" />
                                                                                    </div>
                                                                                </ItemTemplate>
                                                                                <HeaderStyle ForeColor="black" />
                                                                            </asp:TemplateField>
                                                                        </Columns>
                                                                        <HeaderStyle BackColor="Black" ForeColor="White" CssClass="HeadingCell" />
                                                                        <EmptyDataTemplate>
                                                                            No Rows Found!
                                                                        </EmptyDataTemplate>
                                                                    </asp:GridView>
                                                                </td>
                                                                <td valign="top">
                                                                    <asp:GridView ID="GridView_biweekly" runat="server" AutoGenerateColumns="False" Width="100%"
                                                                        AllowPaging="false">
                                                                        <AlternatingRowStyle BackColor="#eff0ef" />
                                                                        <Columns>
                                                                            <asp:BoundField DataField="title" HeaderText="Title" HeaderStyle-ForeColor="black"
                                                                                ItemStyle-HorizontalAlign="Left">
                                                                                <HeaderStyle CssClass="HeadingCell" />
                                                                            </asp:BoundField>
                                                                            <asp:BoundField DataField="location" HeaderText="Location" HeaderStyle-ForeColor="black"
                                                                                ItemStyle-HorizontalAlign="Left">
                                                                                <HeaderStyle CssClass="HeadingCell" />
                                                                            </asp:BoundField>
                                                                            <asp:BoundField DataField="efrequency" HeaderText="Frequency" HeaderStyle-ForeColor="black"
                                                                                ItemStyle-HorizontalAlign="Left">
                                                                                <HeaderStyle CssClass="HeadingCell" />
                                                                            </asp:BoundField>
                                                                            <asp:TemplateField ShowHeader="False" HeaderText="Details" ItemStyle-HorizontalAlign="Center"
                                                                                HeaderStyle-CssClass="HeadingCell">
                                                                                <ItemTemplate>
                                                                                    <div id="ShowHideRightsIcon" runat="server">
                                                                                        <img style="border: 0" src="../images/atmDetail.jpg" alt="click to view details"
                                                                                            onclick="openDialog('<%#Eval("atm_id") %>,2')" class="ShowHand" />
                                                                                    </div>
                                                                                </ItemTemplate>
                                                                                <HeaderStyle ForeColor="black" />
                                                                            </asp:TemplateField>
                                                                        </Columns>
                                                                        <HeaderStyle BackColor="Black" ForeColor="White" CssClass="HeadingCell" />
                                                                        <EmptyDataTemplate>
                                                                            No Rows Found!
                                                                        </EmptyDataTemplate>
                                                                    </asp:GridView>
                                                                </td>
                                                                <td valign="top">
                                                                    <asp:GridView ID="GridView_monthly" runat="server" AutoGenerateColumns="False" Width="100%"
                                                                        AllowPaging="false">
                                                                        <AlternatingRowStyle BackColor="#eff0ef" />
                                                                        <Columns>
                                                                            <asp:BoundField DataField="title" HeaderText="Title" HeaderStyle-ForeColor="black"
                                                                                ItemStyle-HorizontalAlign="Left">
                                                                                <HeaderStyle CssClass="HeadingCell" />
                                                                            </asp:BoundField>
                                                                            <asp:BoundField DataField="location" HeaderText="Location" HeaderStyle-ForeColor="black"
                                                                                ItemStyle-HorizontalAlign="Left">
                                                                                <HeaderStyle CssClass="HeadingCell" />
                                                                            </asp:BoundField>
                                                                            <asp:BoundField DataField="efrequency" HeaderText="Frequency" HeaderStyle-ForeColor="black"
                                                                                ItemStyle-HorizontalAlign="Left">
                                                                                <HeaderStyle CssClass="HeadingCell" />
                                                                            </asp:BoundField>
                                                                            <asp:TemplateField ShowHeader="False" HeaderText="Details" ItemStyle-HorizontalAlign="Center"
                                                                                HeaderStyle-CssClass="HeadingCell">
                                                                                <ItemTemplate>
                                                                                    <div id="ShowHideRightsIcon" runat="server">
                                                                                        <img style="border: 0" src="../images/atmDetail.jpg" alt="click to view details"
                                                                                            onclick="openDialog('<%#Eval("atm_id") %>,2')" class="ShowHand" />
                                                                                    </div>
                                                                                </ItemTemplate>
                                                                                <HeaderStyle ForeColor="black" />
                                                                            </asp:TemplateField>
                                                                        </Columns>
                                                                        <HeaderStyle BackColor="Black" ForeColor="White" CssClass="HeadingCell" />
                                                                        <EmptyDataTemplate>
                                                                            No Rows Found!
                                                                        </EmptyDataTemplate>
                                                                    </asp:GridView>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td valign="top" colspan="3">
                                                                    <asp:GridView ID="GridView5" runat="server" AutoGenerateColumns="False" Width="100%"
                                                                        AllowPaging="false" OnRowEditing="GridView5_RowEditing" OnRowUpdating="GridView5_RowUpdating"
                                                                        OnRowCancelingEdit="GridView5_RowCanceling" DataKeyNames="atm_id">
                                                                        <AlternatingRowStyle BackColor="#eff0ef" />
                                                                        <Columns>
                                                                            <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="HeadingCell">
                                                                                <HeaderTemplate>
                                                                                    <input type="checkbox" id="selectUnselectFaultyRepCurrent" onclick="setCheckATMUsers(selectUnselectFaultyRepCurrent.checked,'GridView5')" />
                                                                                </HeaderTemplate>
                                                                                <ItemTemplate>
                                                                                    <asp:CheckBox ID="chkbx" runat="server" CssClass="textbox"></asp:CheckBox>
                                                                                </ItemTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:BoundField DataField="title" HeaderText="Title" HeaderStyle-ForeColor="black"
                                                                                ItemStyle-HorizontalAlign="Left">
                                                                                <HeaderStyle CssClass="HeadingCell" />
                                                                            </asp:BoundField>
                                                                            <asp:BoundField DataField="location" HeaderText="Location" HeaderStyle-ForeColor="black"
                                                                                ItemStyle-HorizontalAlign="Left">
                                                                                <HeaderStyle CssClass="HeadingCell" />
                                                                            </asp:BoundField>
                                                                            <asp:BoundField DataField="efrequency" HeaderText="Frequency" HeaderStyle-ForeColor="black"
                                                                                ItemStyle-HorizontalAlign="Left">
                                                                                <HeaderStyle CssClass="HeadingCell" />
                                                                            </asp:BoundField>
                                                                            <asp:TemplateField HeaderText="Required Amount" ItemStyle-HorizontalAlign="Right"
                                                                                HeaderStyle-CssClass="HeadingCell">
                                                                                <EditItemTemplate>
                                                                                    <asp:TextBox ID="TextBox_Amount" runat="server" Text='<%# Bind("amount") %>'></asp:TextBox>
                                                                                </EditItemTemplate>
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="Label_amount" runat="server" Text='<%# Bind("amount") %>'></asp:Label>
                                                                                </ItemTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Accept" HeaderStyle-CssClass="HeadingCell" ItemStyle-HorizontalAlign="Center"
                                                                                Visible="false">
                                                                                <ItemTemplate>
                                                                                    <a href='AddEditATM.aspx?atm_id=<%# Eval("atm_id") %>'>
                                                                                        <img alt="Click to edit atm" src="../images/accept.jpg" style="border: 0" class="ShowHand"
                                                                                            width="16" height="16" />
                                                                                    </a>
                                                                                </ItemTemplate>
                                                                                <HeaderStyle CssClass="HeadingCell" />
                                                                            </asp:TemplateField>
                                                                            <asp:CommandField ShowEditButton="True" HeaderStyle-CssClass="HeadingCell" />
                                                                        </Columns>
                                                                        <HeaderStyle BackColor="Black" ForeColor="White" CssClass="HeadingCell" />
                                                                        <EmptyDataTemplate>
                                                                            No Rows Found!
                                                                        </EmptyDataTemplate>
                                                                    </asp:GridView>
                                                                    <input type="button" value="End Analysis" onclick="displaySummary();" class="button" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </asp:Panel>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </ComponentArt:PageView>
                        <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView3" Width="100%">
                            <table width="100%" border="0">
                                <tr>
                                    <td valign="top">
                                        <table border="0" width="100%">
                                            <tr>
                                                <td width="50%">
                                                    <asp:Chart ID="Chart7" runat="server" Width="325" EnableViewState="true" OnClick="Chart7_Click">
                                                        <Titles>
                                                            <asp:Title Font="Trebuchet MS, 14.25pt, style=Bold" Name="Title1" ForeColor="Black"
                                                                Text="ATM Transaction Trend">
                                                            </asp:Title>
                                                        </Titles>
                                                        <Legends>
                                                            <asp:Legend BackColor="Transparent" Alignment="Center" Docking="Right" Font="Trebuchet MS, 8.25pt, style=Bold"
                                                                IsTextAutoFit="False" Name="Default" LegendStyle="Column" Title="Legends" Enabled="false">
                                                            </asp:Legend>
                                                        </Legends>
                                                        <BorderSkin SkinStyle="None"></BorderSkin>
                                                        <Series>
                                                            <asp:Series Name="Default" YValuesPerPoint="2" ChartType="Spline" IsValueShownAsLabel="false"
                                                                IsVisibleInLegend="true" BorderWidth="3">
                                                            </asp:Series>
                                                        </Series>
                                                        <ChartAreas>
                                                            <asp:ChartArea Name="ChartArea1" BackColor="White">
                                                                <Area3DStyle Rotation="0" />
                                                                <AxisY LineColor="64, 64, 64, 64">
                                                                    <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                                    <MajorGrid LineColor="White" />
                                                                </AxisY>
                                                                <AxisX LineColor="64, 64, 64, 64" IntervalType="Days">
                                                                    <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" Format="dd/MM/yyyy" />
                                                                    <MajorGrid LineColor="White" />
                                                                </AxisX>
                                                            </asp:ChartArea>
                                                        </ChartAreas>
                                                    </asp:Chart>
                                                </td>
                                                <td>
                                                    <asp:Chart ID="Chart10" runat="server" Width="325" EnableViewState="true">
                                                        <Titles>
                                                            <asp:Title Font="Trebuchet MS, 14.25pt, style=Bold" Name="Title1" ForeColor="Black"
                                                                Text="ATM Transaction Trend Selected Month">
                                                            </asp:Title>
                                                        </Titles>
                                                        <Legends>
                                                            <asp:Legend BackColor="Transparent" Alignment="Center" Docking="Right" Font="Trebuchet MS, 8.25pt, style=Bold"
                                                                IsTextAutoFit="False" Name="Default" LegendStyle="Column" Title="Legends" Enabled="false">
                                                            </asp:Legend>
                                                        </Legends>
                                                        <BorderSkin SkinStyle="None"></BorderSkin>
                                                        <Series>
                                                            <asp:Series Name="Default" YValuesPerPoint="2" ChartType="Spline" IsValueShownAsLabel="false"
                                                                IsVisibleInLegend="true" BorderWidth="3">
                                                            </asp:Series>
                                                        </Series>
                                                        <ChartAreas>
                                                            <asp:ChartArea Name="ChartArea1" BackColor="White">
                                                                <Area3DStyle Rotation="0" />
                                                                <AxisY LineColor="64, 64, 64, 64">
                                                                    <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                                    <MajorGrid LineColor="White" />
                                                                </AxisY>
                                                                <AxisX LineColor="64, 64, 64, 64" IntervalType="Days">
                                                                    <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" Format="dd/MM/yyyy" />
                                                                    <MajorGrid LineColor="White" />
                                                                </AxisX>
                                                            </asp:ChartArea>
                                                        </ChartAreas>
                                                    </asp:Chart>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="center" colspan="2">
                                                    <asp:Chart ID="Chart8" runat="server" Width="825" EnableViewState="true">
                                                        <Titles>
                                                            <asp:Title Font="Trebuchet MS, 14.25pt, style=Bold" Name="Title1" ForeColor="Black"
                                                                Text="ATM Transaction Trend By Denomination">
                                                            </asp:Title>
                                                        </Titles>
                                                        <Legends>
                                                            <asp:Legend BackColor="Transparent" Alignment="Center" Docking="Right" Font="Trebuchet MS, 8.25pt, style=Bold"
                                                                IsTextAutoFit="False" Name="Default" LegendStyle="Column" Title="Legends" Enabled="false">
                                                            </asp:Legend>
                                                        </Legends>
                                                        <BorderSkin SkinStyle="None"></BorderSkin>
                                                        <Series>
                                                            <asp:Series Name="Default50" YValuesPerPoint="2" ChartType="Spline" IsValueShownAsLabel="false"
                                                                IsVisibleInLegend="true" BorderWidth="3">
                                                            </asp:Series>
                                                            <asp:Series Name="Default100" YValuesPerPoint="2" ChartType="Spline" IsValueShownAsLabel="false"
                                                                IsVisibleInLegend="true" BorderWidth="3">
                                                            </asp:Series>
                                                            <asp:Series Name="Default500" YValuesPerPoint="2" ChartType="Spline" IsValueShownAsLabel="false"
                                                                IsVisibleInLegend="true" BorderWidth="3">
                                                            </asp:Series>
                                                        </Series>
                                                        <ChartAreas>
                                                            <asp:ChartArea Name="ChartArea1" BackColor="White">
                                                                <Area3DStyle Rotation="0" />
                                                                <AxisY LineColor="64, 64, 64, 64">
                                                                    <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                                    <MajorGrid LineColor="White" />
                                                                </AxisY>
                                                                <AxisX LineColor="64, 64, 64, 64" IntervalType="Days">
                                                                    <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" Format="dd/MM/yyyy" />
                                                                    <MajorGrid LineColor="White" />
                                                                </AxisX>
                                                            </asp:ChartArea>
                                                        </ChartAreas>
                                                    </asp:Chart>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td valign="top" width="24%">
                                        <table width="100%" class="SEARCHBOX" border="0" align="right">
                                            <tr>
                                                <td colspan="2">
                                                    <table>
                                                        <tr>
                                                            <td>
                                                                <table>
                                                                    <tr>
                                                                        <td>
                                                                            <b>Selected ATM</b>
                                                                        </td>
                                                                        <td>
                                                                            <asp:Label ID="Label1" runat="server" Font-Bold="true"></asp:Label>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td>
                                                                            From
                                                                        </td>
                                                                        <td>
                                                                            <asp:TextBox ID="TextBox_fromTrendAnalysis" runat="server" CssClass="datepicker"
                                                                                Font-Names="Verdana" Font-Size="10px"></asp:TextBox>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td>
                                                                            To
                                                                        </td>
                                                                        <td>
                                                                            <asp:TextBox ID="TextBox_toTrendAnalysis" runat="server" CssClass="datepicker" Font-Names="Verdana"
                                                                                Font-Size="10px"></asp:TextBox>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td>
                                                                            <asp:Button ID="Button_GenerateATMTrend" runat="server" CssClass="button" Text="Generate Trend"
                                                                                OnClick="Button_GenerateATMTrend_Click" />
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td>
                                                                            <b>Selected Date</b>
                                                                        </td>
                                                                        <td>
                                                                            <asp:Label ID="Label_Date" runat="server"></asp:Label>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td>
                                                                            <b>Amount</b>
                                                                        </td>
                                                                        <td>
                                                                            <asp:Label ID="Label_Amount" runat="server"></asp:Label>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td colspan="2">
                                                                <asp:Chart ID="Chart6" runat="server" Width="325" EnableViewState="true">
                                                                    <Titles>
                                                                        <asp:Title Font="Trebuchet MS, 14.25pt, style=Bold" Name="Title1" ForeColor="Black"
                                                                            Text="ATM Transaction Trend Selected Month Last Week">
                                                                        </asp:Title>
                                                                    </Titles>
                                                                    <Legends>
                                                                        <asp:Legend BackColor="Transparent" Alignment="Center" Docking="Right" Font="Trebuchet MS, 8.25pt, style=Bold"
                                                                            IsTextAutoFit="False" Name="Default" LegendStyle="Column" Title="Legends" Enabled="false">
                                                                        </asp:Legend>
                                                                    </Legends>
                                                                    <BorderSkin SkinStyle="None"></BorderSkin>
                                                                    <Series>
                                                                        <asp:Series Name="Default" YValuesPerPoint="2" ChartType="Column" IsValueShownAsLabel="false"
                                                                            IsVisibleInLegend="true" BorderWidth="3">
                                                                        </asp:Series>
                                                                    </Series>
                                                                    <ChartAreas>
                                                                        <asp:ChartArea Name="ChartArea1" BackColor="White">
                                                                            <Area3DStyle Rotation="0" />
                                                                            <AxisY LineColor="64, 64, 64, 64">
                                                                                <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                                                <MajorGrid LineColor="White" />
                                                                            </AxisY>
                                                                            <AxisX LineColor="64, 64, 64, 64" IntervalType="Days">
                                                                                <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" Format="dd/MM/yyyy" />
                                                                                <MajorGrid LineColor="White" />
                                                                            </AxisX>
                                                                        </asp:ChartArea>
                                                                    </ChartAreas>
                                                                </asp:Chart>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td colspan="2">
                                                                <asp:Chart ID="Chart9" runat="server" Width="325" EnableViewState="true">
                                                                    <Titles>
                                                                        <asp:Title Font="Trebuchet MS, 14.25pt, style=Bold" Name="Title1" ForeColor="Black"
                                                                            Text="ATM Transaction Trend Selected Month Next Week">
                                                                        </asp:Title>
                                                                    </Titles>
                                                                    <Legends>
                                                                        <asp:Legend BackColor="Transparent" Alignment="Center" Docking="Right" Font="Trebuchet MS, 8.25pt, style=Bold"
                                                                            IsTextAutoFit="False" Name="Default" LegendStyle="Column" Title="Legends" Enabled="false">
                                                                        </asp:Legend>
                                                                    </Legends>
                                                                    <BorderSkin SkinStyle="None"></BorderSkin>
                                                                    <Series>
                                                                        <asp:Series Name="Default" YValuesPerPoint="2" ChartType="Column" IsValueShownAsLabel="false"
                                                                            IsVisibleInLegend="true" BorderWidth="3">
                                                                        </asp:Series>
                                                                    </Series>
                                                                    <ChartAreas>
                                                                        <asp:ChartArea Name="ChartArea1" BackColor="White">
                                                                            <Area3DStyle Rotation="0" />
                                                                            <AxisY LineColor="64, 64, 64, 64">
                                                                                <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                                                <MajorGrid LineColor="White" />
                                                                            </AxisY>
                                                                            <AxisX LineColor="64, 64, 64, 64" IntervalType="Days">
                                                                                <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" Format="dd/MM/yyyy" />
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
                        </ComponentArt:PageView>
                        <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView1" Width="100%">
                            <table width="60%">
                                <tr>
                                    <td>
                                        <ComponentArt:TabStrip ID="TabStrip5" CssClass="TopGroup" DefaultItemLookId="DefaultTabLook"
                                            DefaultSelectedItemLookId="SelectedTabLook" DefaultDisabledItemLookId="DisabledTabLook"
                                            DefaultGroupTabSpacing="1px" ImagesBaseUrl="../images/Tabimages/" MultiPageId="MultiPage30"
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
                                                <ComponentArt:TabStripTab ID="TabStripTab17" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                                    ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                                    DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                                    Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                                    SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="System Suggested"
                                                    AutoPostBackOnSelect="true">
                                                </ComponentArt:TabStripTab>
                                                <ComponentArt:TabStripTab ID="TabStripTab18" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                                    ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                                    DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                                    Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                                    SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Forced Suggestion"
                                                    AutoPostBackOnSelect="true">
                                                </ComponentArt:TabStripTab>
                                            </Tabs>
                                        </ComponentArt:TabStrip>
                                        <ComponentArt:MultiPage ID="MultiPage30" CssClass="MultiPage" runat="server" Width="100%">
                                            <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView33">
                                                <table width="60%">
                                                    <tr>
                                                        <td>
                                                            Expected Denomination Quantity QAR50
                                                        </td>
                                                        <td>
                                                            <asp:Label ID="Label2" runat="server" Text=""></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            Expected Denomination Quantity QAR100
                                                        </td>
                                                        <td>
                                                            <asp:Label ID="Label3" runat="server" Text=""></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            Expected Denomination Quantity QAR500
                                                        </td>
                                                        <td>
                                                            <asp:Label ID="Label4" runat="server" Text=""></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            Denomination Type Higher Utilization
                                                        </td>
                                                        <td>
                                                            <asp:Label ID="Label_DenomHigher" runat="server" Text="QAR500"></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            Denomination Type Lower Utilization
                                                        </td>
                                                        <td>
                                                            <asp:Label ID="Label_DenomLower" runat="server" Text="QAR100"></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <fieldset>
                                                                <legend><b>Suggested Events</b></legend>
                                                                <table>
                                                                    <tr>
                                                                        <td>
                                                                            <asp:ListBox ID="ListBox1" runat="server" Width="250px" CssClass="textbox">
                                                                                <asp:ListItem Text="22/09/2012"></asp:ListItem>
                                                                                <asp:ListItem Text="23/09/2012"></asp:ListItem>
                                                                                <asp:ListItem Text="24/09/2012"></asp:ListItem>
                                                                            </asp:ListBox>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </fieldset>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </ComponentArt:PageView>
                                            <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView6" Width="100%">
                                                <table width="100%">
                                                    <tr>
                                                        <td>
                                                            ATM Business Days
                                                        </td>
                                                        <td>
                                                            <asp:CheckBoxList ID="Checkboxlist_days" runat="server" RepeatDirection="Horizontal"
                                                                Width="100%">
                                                                <asp:ListItem>Sunday</asp:ListItem>
                                                                <asp:ListItem>Monday</asp:ListItem>
                                                                <asp:ListItem>Tuesday</asp:ListItem>
                                                                <asp:ListItem>Wednesday</asp:ListItem>
                                                                <asp:ListItem>Thursday</asp:ListItem>
                                                                <asp:ListItem>Friday</asp:ListItem>
                                                                <asp:ListItem>Saturday</asp:ListItem>
                                                            </asp:CheckBoxList>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            ATM Emergency Days
                                                        </td>
                                                        <td>
                                                            <asp:CheckBoxList ID="Checkboxlist1" runat="server" RepeatDirection="Horizontal">
                                                                <asp:ListItem>Sunday</asp:ListItem>
                                                                <asp:ListItem>Monday</asp:ListItem>
                                                                <asp:ListItem>Tuesday</asp:ListItem>
                                                                <asp:ListItem>Wednesday</asp:ListItem>
                                                                <asp:ListItem>Thursday</asp:ListItem>
                                                                <asp:ListItem>Friday</asp:ListItem>
                                                                <asp:ListItem>Saturday</asp:ListItem>
                                                            </asp:CheckBoxList>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            Normal Order Limit
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="TextBox10" runat="server" CssClass="textbox"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            Emergency Order Limit
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="TextBox11" runat="server" CssClass="textbox"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            Cancel Normal Order If Emergency Is Placed
                                                        </td>
                                                        <td>
                                                            <asp:CheckBox ID="CheckBox_CancelNormalOrder" runat="server" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            Buffer %
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="TextBox_BufferPerc" runat="server" CssClass="textbox"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            Minimum Operating Balance Total
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="TextBox_MOBTotal" runat="server" CssClass="textbox"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            Minimum Operating Balance For Type 1
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="TextBox2" runat="server" CssClass="textbox"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            Minimum Operating Balance For Type 2
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="TextBox3" runat="server" CssClass="textbox"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            Minimum Operating Balance For Type 3
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="TextBox4" runat="server" CssClass="textbox"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            Minimum Operating Balance For Type 4
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="TextBox5" runat="server" CssClass="textbox"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            Cost Of Cash
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="TextBox6" runat="server" CssClass="textbox"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            Cost Of CIT Per Trip
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="TextBox7" runat="server" CssClass="textbox"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            Capacity Of CIT Per Day
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="TextBox8" runat="server" CssClass="textbox"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            No.Of Lead Days
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="TextBox9" runat="server" CssClass="textbox"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            No.Of Trips per week
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="TextBox_NoOfTripsPerWeek" runat="server" CssClass="textbox" Text="3"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="2">
                                                            <fieldset>
                                                                <legend><b>Events Configuration</b></legend>
                                                                <table>
                                                                    <tr>
                                                                        <td>
                                                                            To
                                                                        </td>
                                                                        <td>
                                                                            <asp:TextBox ID="TextBox1" runat="server" CssClass="datepicker" Font-Names="Verdana"
                                                                                Font-Size="10px"></asp:TextBox>
                                                                        </td>
                                                                        <td>
                                                                            <asp:Button ID="Button_Add" runat="server" Text="Add" CssClass="button" OnClick="Button_Add_Click" />
                                                                        </td>
                                                                        <td>
                                                                            <asp:Button ID="Button_Remove" runat="server" Text="Remove" CssClass="button" OnClick="Button_Remove_Click" />
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td colspan="3">
                                                                            <asp:ListBox ID="ListBox_Events" runat="server" Width="250px" CssClass="textbox">
                                                                                <asp:ListItem Text="22/09/2012"></asp:ListItem>
                                                                                <asp:ListItem Text="23/09/2012"></asp:ListItem>
                                                                                <asp:ListItem Text="24/09/2012"></asp:ListItem>
                                                                            </asp:ListBox>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </fieldset>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <fieldset>
                                                                <legend><b>Standard Order Configuration</b></legend>
                                                                <table>
                                                                    <tr>
                                                                        <td>
                                                                            QAR50
                                                                        </td>
                                                                        <td>
                                                                            <asp:TextBox ID="Textbox_type1" runat="server" CssClass="textbox" Width="50px"> </asp:TextBox>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td>
                                                                            QAR100
                                                                        </td>
                                                                        <td>
                                                                            <asp:TextBox ID="Textbox_type2" runat="server" CssClass="textbox" Width="50px"></asp:TextBox>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td>
                                                                            QAR500
                                                                        </td>
                                                                        <td>
                                                                            <asp:TextBox ID="Textbox_type3" runat="server" CssClass="textbox" Width="50px"></asp:TextBox>
                                                                        </td>
                                                                    </tr>
                                                                    <tr style="visibility: hidden">
                                                                        <td>
                                                                            Type4
                                                                        </td>
                                                                        <td>
                                                                            <asp:TextBox ID="Textbox_type4" runat="server" CssClass="textbox" Width="50px"></asp:TextBox>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </fieldset>
                                                        </td>
                                                        <td>
                                                            <fieldset>
                                                                <legend><b>Split % By Denomination</b></legend>
                                                                <table>
                                                                    <tr>
                                                                        <td>
                                                                            QAR50%
                                                                        </td>
                                                                        <td>
                                                                            <asp:TextBox ID="Textbox_Den1" runat="server" CssClass="textbox" Width="50px"> </asp:TextBox>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td>
                                                                            QAR100%
                                                                        </td>
                                                                        <td>
                                                                            <asp:TextBox ID="Textbox_Den2" runat="server" CssClass="textbox" Width="50px"></asp:TextBox>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td>
                                                                            QAR500%
                                                                        </td>
                                                                        <td>
                                                                            <asp:TextBox ID="Textbox_Den3" runat="server" CssClass="textbox" Width="50px"></asp:TextBox>
                                                                        </td>
                                                                    </tr>
                                                                    <tr style="visibility: hidden">
                                                                        <td>
                                                                            Type4
                                                                        </td>
                                                                        <td>
                                                                            <asp:TextBox ID="Textbox_Den4" runat="server" CssClass="textbox" Width="50px"></asp:TextBox>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </fieldset>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </ComponentArt:PageView>
                                        </ComponentArt:MultiPage>
                                    </td>
                                </tr>
                            </table>
                        </ComponentArt:PageView>
                        <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView_general"
                            Width="100%">
                            <table>
                                <tr>
                                    <td valign="top" colspan="2" style="width: 80%">
                                        <asp:GridView ID="GridView_forecasting" runat="server" AutoGenerateColumns="False"
                                            Width="100%" AllowPaging="false" OnRowDataBound="GridView_forecasting_rowdatabound"
                                            DataKeyNames="atm_id">
                                            <AlternatingRowStyle BackColor="#eff0ef" />
                                            <Columns>
                                                <asp:TemplateField HeaderStyle-CssClass="HeadingCell" ItemStyle-HorizontalAlign="Center">
                                                    <HeaderTemplate>
                                                        <input type="checkbox" id="selectUnselect" onclick="setCheckATMUsers(selectUnselect.checked,'GridView_forecasting')" />
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="chkbx" runat="server" CssClass="textbox"></asp:CheckBox>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderStyle-CssClass="HeadingCell" ItemStyle-Width="180px" HeaderText="Date">
                                                    <ItemTemplate>
                                                        <img src="../images/events.jpg" id="imgEvent" runat="server" width="15" height="15" />
                                                        <%#Eval("date") %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="opening_balance" HeaderText="Opening Balance" HeaderStyle-ForeColor="black"
                                                    ItemStyle-HorizontalAlign="Right">
                                                    <HeaderStyle CssClass="HeadingCell" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="replenishment" HeaderText="Replenishment" HeaderStyle-ForeColor="black"
                                                    ItemStyle-HorizontalAlign="Right">
                                                    <HeaderStyle CssClass="HeadingCell" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="pre_withdrawals" HeaderText="PreWithdrawals" HeaderStyle-ForeColor="black"
                                                    ItemStyle-HorizontalAlign="Right">
                                                    <HeaderStyle CssClass="HeadingCell" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="withdrawals" HeaderText="Withdrawals" HeaderStyle-ForeColor="black"
                                                    ItemStyle-HorizontalAlign="Right">
                                                    <HeaderStyle CssClass="HeadingCell" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="returns" HeaderText="Returns" HeaderStyle-ForeColor="black"
                                                    ItemStyle-HorizontalAlign="Right">
                                                    <HeaderStyle CssClass="HeadingCell" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="closing_balance" HeaderText="Closing Balance" HeaderStyle-ForeColor="black"
                                                    ItemStyle-HorizontalAlign="Right">
                                                    <HeaderStyle CssClass="HeadingCell" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="type1" HeaderText="QAR50" HeaderStyle-ForeColor="black"
                                                    ItemStyle-HorizontalAlign="Right">
                                                    <HeaderStyle CssClass="HeadingCell" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="type2" HeaderText="QAR100" HeaderStyle-ForeColor="black"
                                                    ItemStyle-HorizontalAlign="Right">
                                                    <HeaderStyle CssClass="HeadingCell" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="type3" HeaderText="QAR500" HeaderStyle-ForeColor="black"
                                                    ItemStyle-HorizontalAlign="Right">
                                                    <HeaderStyle CssClass="HeadingCell" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="type4" HeaderText="Type4" HeaderStyle-ForeColor="black"
                                                    Visible="false" ItemStyle-HorizontalAlign="Right">
                                                    <HeaderStyle CssClass="HeadingCell" />
                                                </asp:BoundField>
                                            </Columns>
                                            <HeaderStyle BackColor="Black" ForeColor="White" CssClass="HeadingCell" />
                                        </asp:GridView>
                                        <br />
                                        <asp:Button ID="Button_GenerateOrder" runat="server" Text="Generate Orders" CssClass="button"
                                            OnClick="Button_GenerateOrder_Click" />
                                    </td>
                                    <td valign="top">
                                        <table width="100%" class="SEARCHBOX" border="0" align="right">
                                            <tr>
                                                <td colspan="2">
                                                    <table>
                                                        <tr>
                                                            <td>
                                                                <b>Selected ATM</b>
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="Label_ATMTitle" runat="server" Font-Bold="true"></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                From
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="TextBox_FromDate" runat="server" CssClass="datepicker" Font-Names="Verdana"
                                                                    Font-Size="10px"></asp:TextBox>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                To
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="TextBox_ToDate" runat="server" CssClass="datepicker" Font-Names="Verdana"
                                                                    Font-Size="10px"></asp:TextBox>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Button ID="Button_Generate" Text="Forecast" runat="server" CssClass="button"
                                                        OnClick="Button_Generate_Click" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </ComponentArt:PageView>
                        <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView4" Width="100%">
                            <table>
                                <tr>
                                    <td valign="top" colspan="2" style="width: 84%">
                                        <asp:GridView ID="GridView4" runat="server" AutoGenerateColumns="False" Width="100%"
                                            AllowPaging="false" DataKeyNames="atm_id">
                                            <AlternatingRowStyle BackColor="#eff0ef" />
                                            <Columns>
                                                <asp:TemplateField HeaderText="Scheduled" HeaderStyle-CssClass="HeadingCell" ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="chkbx" runat="server" CssClass="textbox"></asp:CheckBox>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="title" HeaderText="Title" HeaderStyle-ForeColor="black">
                                                    <HeaderStyle CssClass="HeadingCell" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="date" HeaderText="Date" HeaderStyle-ForeColor="black">
                                                    <HeaderStyle CssClass="HeadingCell" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="opening_balance" HeaderText="Opening Balance" HeaderStyle-ForeColor="black"
                                                    ItemStyle-HorizontalAlign="Right">
                                                    <HeaderStyle CssClass="HeadingCell" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="replenishment" HeaderText="Replenishment" HeaderStyle-ForeColor="black"
                                                    ItemStyle-HorizontalAlign="Right">
                                                    <HeaderStyle CssClass="HeadingCell" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="pre_withdrawals" HeaderText="PreWithdrawals" HeaderStyle-ForeColor="black"
                                                    ItemStyle-HorizontalAlign="Right">
                                                    <HeaderStyle CssClass="HeadingCell" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="withdrawals" HeaderText="Withdrawals" HeaderStyle-ForeColor="black"
                                                    ItemStyle-HorizontalAlign="Right">
                                                    <HeaderStyle CssClass="HeadingCell" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="returns" HeaderText="Returns" HeaderStyle-ForeColor="black"
                                                    ItemStyle-HorizontalAlign="Right">
                                                    <HeaderStyle CssClass="HeadingCell" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="closing_balance" HeaderText="Closing Balance" HeaderStyle-ForeColor="black"
                                                    ItemStyle-HorizontalAlign="Right">
                                                    <HeaderStyle CssClass="HeadingCell" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="type1" HeaderText="QAR50" HeaderStyle-ForeColor="black"
                                                    ItemStyle-HorizontalAlign="Right">
                                                    <HeaderStyle CssClass="HeadingCell" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="type2" HeaderText="QAR100" HeaderStyle-ForeColor="black"
                                                    ItemStyle-HorizontalAlign="Right">
                                                    <HeaderStyle CssClass="HeadingCell" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="type3" HeaderText="QAR500" HeaderStyle-ForeColor="black"
                                                    ItemStyle-HorizontalAlign="Right">
                                                    <HeaderStyle CssClass="HeadingCell" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="type4" HeaderText="Type4" HeaderStyle-ForeColor="black"
                                                    Visible="false" ItemStyle-HorizontalAlign="Right">
                                                    <HeaderStyle CssClass="HeadingCell" />
                                                </asp:BoundField>
                                            </Columns>
                                            <HeaderStyle BackColor="Black" ForeColor="White" CssClass="HeadingCell" />
                                        </asp:GridView>
                                        <br />
                                        <asp:Button ID="Button_GenerateOrderWithLeadTime" runat="server" Text="Generate Orders"
                                            CssClass="button" OnClick="Button_GenerateOrderWithLeadTime_Click" />
                                    </td>
                                    <td valign="top">
                                        <table width="100%" class="SEARCHBOX" border="0" align="right">
                                            <tr>
                                                <td colspan="2">
                                                    <table>
                                                        <tr>
                                                            <td>
                                                                Lead Time
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="TextBox_LeadTime" runat="server" CssClass="textbox"></asp:TextBox>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Button ID="Button_ForecastOrderWithLeadTime" Text="Search" runat="server" CssClass="button"
                                                        OnClick="Button_ForecastOrderWithLeadTime_Click" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </ComponentArt:PageView>
                        <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView5" Width="100%">
                            <table>
                                <tr>
                                    <td valign="top">
                                        <fieldset>
                                            <legend>
                                                <h2>
                                                    Order Adjustments</h2>
                                            </legend>
                                            <table>
                                                <tr>
                                                    <td>
                                                        <asp:RadioButtonList ID="RadioBtn_Adjust" runat="server" RepeatDirection="Vertical">
                                                            <asp:ListItem>Adjust Order Based On Higher Denomination Utilization</asp:ListItem>
                                                            <asp:ListItem>Adjust Order Based On Available Cash</asp:ListItem>
                                                            <asp:ListItem>Generate / Cancel Order Because of CIT Limitations</asp:ListItem>
                                                        </asp:RadioButtonList>
                                                    </td>
                                                </tr>
                                            </table>
                                        </fieldset>
                                    </td>
                                </tr>
                            </table>
                        </ComponentArt:PageView>
                        <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView37">
                            <asp:Chart ID="Chart4" runat="server" Width="1050px" BorderColor="#B54001" EnableViewState="true">
                                <Titles>
                                    <asp:Title Font="Trebuchet MS, 14.25pt, style=Bold" ForeColor="26, 59, 105" Text="Forecasted Vs Actual (Last Week) Analysis">
                                    </asp:Title>
                                </Titles>
                                <Legends>
                                    <asp:Legend BackColor="Transparent" Alignment="Near" Docking="Top" Font="Trebuchet MS, 8.25pt, style=Bold"
                                        IsTextAutoFit="true" Name="Default" LegendStyle="Row" Title="Legends" Enabled="true">
                                    </asp:Legend>
                                </Legends>
                                <BorderSkin SkinStyle="None"></BorderSkin>
                                <Series>
                                    <asp:Series ChartArea="ChartArea1" BorderColor="180, 26, 59, 105" ChartType="Spline"
                                        BorderWidth="4" MarkerSize="4" MarkerStyle="Circle" IsValueShownAsLabel="true"
                                        Name="Forecast">
                                    </asp:Series>
                                    <asp:Series ChartArea="ChartArea1" BorderColor="180, 26, 59, 105" ChartType="Line"
                                        BorderWidth="4" MarkerSize="4" MarkerStyle="Circle" IsValueShownAsLabel="true"
                                        Name="Actual">
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
                        </ComponentArt:PageView>
                        <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView38">
                            <asp:Chart ID="Chart5" runat="server" Width="1050px" BorderColor="#B54001" EnableViewState="true">
                                <Titles>
                                    <asp:Title Font="Trebuchet MS, 14.25pt, style=Bold" ForeColor="26, 59, 105" Text="Forecasting Quality Analysis ">
                                    </asp:Title>
                                </Titles>
                                <Legends>
                                    <asp:Legend BackColor="Transparent" Alignment="Near" Docking="Top" Font="Trebuchet MS, 8.25pt, style=Bold"
                                        IsTextAutoFit="true" Name="Default" LegendStyle="Row" Title="Legends" Enabled="true">
                                    </asp:Legend>
                                </Legends>
                                <BorderSkin SkinStyle="None"></BorderSkin>
                                <Series>
                                    <asp:Series ChartArea="ChartArea1" BorderColor="180, 26, 59, 105" ChartType="Spline"
                                        BorderWidth="4" MarkerSize="4" MarkerStyle="Circle" IsValueShownAsLabel="true"
                                        Name="Forecasting Quality%">
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
                                        <AxisX LineColor="64, 64, 64, 64" IntervalType="Days">
                                            <LabelStyle Font="Trebuchet MS, 11.25pt, style=Bold" />
                                            <MajorGrid LineColor="White" />
                                        </AxisX>
                                    </asp:ChartArea>
                                </ChartAreas>
                            </asp:Chart>
                        </ComponentArt:PageView>
                    </ComponentArt:MultiPage>
                </td>
            </tr>
        </table>
    </div>
    <input type="button" runat="server" id="refresh" style="display: none" />
    <ComponentArt:CallBack ID="DialogCallBack" runat="server" Width="0" Height="0" OnCallback="DialogCallBack_Callback">
    </ComponentArt:CallBack>
    <ComponentArt:Dialog ContentCssClass="contentCss" FooterCssClass="footerCss" HeaderCssClass="headerCss"
        CssClass="dialogCss" Icon="" Value="" Title="AJAX TabStrip Content" CloseTransition="Fade"
        ModalMaskCssClass="modalMaskCssClass" AllowDrag="false" Alignment="TopCentre"
        Modal="true" ID="Dialog1" runat="server" Width="810" ContentClientTemplateId="data"
        AnimationType="None" AnimationPath="Direct" AnimationSlide="Linear" ShowTransition="None"
        TransitionDuration="0" AnimationDuration="0">
        <ClientTemplates>
            <ComponentArt:ClientTemplate ID="data">
                <table cellpadding="0" style="border-color: #c9c9c9" cellspacing="0" border="0" width="100%"
                    onmousedown="Dialog1.StartDrag(event);">
                    <tr>
                        <td style="background-image: url(../images/bg-brandn.png); height: 45px;">
                            <table width="100%" cellpadding="0" cellspacing="0" border="0">
                                <tr>
                                    <td valign="middle" style="padding-left: 4px; color: white; font-size: 15px; font-family: Verdana;
                                        font-weight: bold;">
                                        Information
                                    </td>
                                    <td align="right" style="padding-right: 10px;">
                                        <img src="../images/DialogImages/close.gif" onclick="Dialog1.Close();" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td style="border-color: #c9c9c9; background-color: white; font-size: 12px; font-family: Arial;
                            padding: 10px;">
                            <div id="summaryInfo" style="overflow: auto; height: 400px">
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
