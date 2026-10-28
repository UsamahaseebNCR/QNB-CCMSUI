<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CallOutAnalysis.aspx.cs"
    Inherits="CCMSUI.CalOutAnalysis" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Register TagPrefix="ofc" Namespace="OpenFlashChart" Assembly="OpenFlashChart" %>
<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Call Out Analysis</title>
    <script language="javascript" src="content/cal/datepicker.js" type="text/javascript"></script>
    <link href="content/iSuite.css" rel="stylesheet" />
     <script src="content/shortcut.js" type="text/javascript" language="javascript">
    </script>
</head>
<body onload="hideProgressBar();">
    <form id="form1" runat="server">
    <div>
        <asp:Literal ID="Literal_Message" runat="server"></asp:Literal>
    </div>
    <ComponentArt:TabStrip ID="TabStrip_CallOut" CssClass="TopGroup" DefaultItemLookId="DefaultTabLook"
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
            <ComponentArt:TabStripTab ID="TabStripTab_WeekAnalysis" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Weekly Analysis">
            </ComponentArt:TabStripTab>
            <ComponentArt:TabStripTab ID="TabStripTab_RegionWiseAnalysis" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Call Out By Problem">
            </ComponentArt:TabStripTab>
            <ComponentArt:TabStripTab ID="TabStripTab1" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Call Out By Location">
            </ComponentArt:TabStripTab>
            <ComponentArt:TabStripTab ID="TabStripTab2" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Call Out Resolution By Actual Time">
            </ComponentArt:TabStripTab>
            <ComponentArt:TabStripTab ID="TabStripTab3" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Call Out Resolution Overall Time">
            </ComponentArt:TabStripTab>
        </Tabs>
    </ComponentArt:TabStrip>
    <ComponentArt:MultiPage ID="MultiPage1" CssClass="MultiPage" runat="server">
        <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView1">
            <div>
                <table width="100%" border="0">
                    <tr>
                        <td colspan="3">
                            <asp:ValidationSummary ID="valSummary" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td align="center" style="font-size: large; background-color: Gray; color: White"
                            colspan="3">
                            Call Out Analysis
                        </td>
                    </tr>
                    <tr id="t" runat="server">
                        <td colspan="3">
                            <table style="background-color: InfoBackground; width: 100%" class="DemoArea">
                                <tr>
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
                                    <%--<td>
                                        <img src="images/IconCalendar.gif" alt="Click to open calender" onclick="popUpCalendar(this,document.getElementById('<%=TextBox_From.ClientID %>'),'dd/mm/yyyy');"
                                                    style="border: 0; cursor: hand" id="IMG2" />
                                                


                                        <%--<ComponentArt:Calendar ID="Picker1" ControlType="Picker" PickerFormat="Custom" PickerCustomFormat="dd/MM/yyyy"
                                            PickerCssClass="picker" runat="server">
                                            <ClientEvents>
                                                <SelectionChanged EventHandler="Picker1_onSelectionChanged" />
                                            </ClientEvents>
                                        </ComponentArt:Calendar>
                                        <img id="calendar_button" alt="" onclick="Calendar1.setSelectedDate(Picker1.getSelectedDate());Calendar1.show();"
                                            class="calendar_button" src="images/btn_calendar.gif" width="25" height="22" />
                                    </td>--%>
                                    <td>
                                        <%--<ComponentArt:Calendar runat="server" ID="Calendar1" PopUp="Custom" PopUpExpandControlId="calendar_button"
                                            AllowMultipleSelection="false" AllowWeekSelection="false" AllowMonthSelection="false"
                                            ControlType="Calendar"  AutoTheming="true" Width="170" Height="100">
                                            <ClientEvents>
                                                <SelectionChanged EventHandler="Calendar1_onSelectionChanged" />
                                            </ClientEvents>
                                        </ComponentArt:Calendar>--%>
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
                    <tr id="chartRow">
                        <td valign="top">
                            <table style="margin-top: 10px;">
                                <tr>
                                    <td style="text-align: center; font-size: medium" onclick="javascript:window.open('DisplayData.aspx?gid=24&data='+this.innerHTML.replace('<B>','').replace('</B>','').replace('<BR/>','').replace('<BR>',''));">
                                        <asp:Literal ID="Literal5" runat="server"></asp:Literal>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <ofc:Chart ID="Chart5" Height="230" Width="330px" runat="server" OnLoad="Chart5_Load" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td>
                            <table style="margin-top: 10px;">
                                <tr>
                                    <td style="text-align: center; font-size: medium" onclick="javascript:window.open('DisplayData.aspx?gid=24&data='+this.innerHTML.replace('<B>','').replace('</B>','').replace('<BR/>','').replace('<BR>',''));">
                                        <asp:Literal ID="Literal1" runat="server"></asp:Literal>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <ofc:Chart ID="Chart1" Height="230" Width="330px" runat="server" OnLoad="Chart1_Load" />
                                    </td>
                                </tr>
                                <tr>
                                    <td style="text-align: center; font-size: medium" onclick="javascript:window.open('DisplayData.aspx?gid=24&data='+this.innerHTML.replace('<B>','').replace('</B>','').replace('<BR/>','').replace('<BR>',''));">
                                        <asp:Literal ID="Literal2" runat="server"></asp:Literal>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <ofc:Chart ID="Chart2" Height="230" Width="330px" runat="server" OnLoad="Chart2_Load" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td valign="top">
                            <table style="margin-top: 10px; margin-bottom: 10px">
                                <tr>
                                    <td style="text-align: center; font-size: medium" onclick="javascript:window.open('DisplayData.aspx?gid=24&data='+this.innerHTML.replace('<B>','').replace('</B>','').replace('<BR/>','').replace('<BR>',''));">
                                        <asp:Literal ID="Literal3" runat="server"></asp:Literal>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <ofc:Chart ID="Chart3" Height="460" Width="330px" runat="server" OnLoad="Chart3_Load" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </div>
        </ComponentArt:PageView>
        <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView2">
            <div>
                <table width="100%" border="0">
                    <tr>
                        <td align="center" style="font-size: large; background-color: Gray; color: White"
                            colspan="3">
                            Call Out Analysis
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
                                        <asp:TextBox ID="TextBox3" runat="server" CssClass="textbox"></asp:TextBox>
                                    </td>
                                    <td style="width: 23px">
                                        <img src="images/IconCalendar.gif" alt="Click to open calender" onclick="javascript:show_calendar('form1.TextBox3');"
                                            style="border: 0; cursor: hand" id="IMG2" />
                                    </td>
                                    <td style="width: 3px">
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator2" CssClass="Validator"
                                            runat="server" ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                            ControlToValidate="TextBox3" ErrorMessage="To Date is invalid.">*</asp:RegularExpressionValidator>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="TextBox3"
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
                                            style="border: 0; cursor: hand" id="IMG3" />
                                    </td>
                                    <td style="width: 3px">
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator3" CssClass="Validator"
                                            runat="server" ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                            ControlToValidate="TextBox4" ErrorMessage="To Date is invalid.">*</asp:RegularExpressionValidator>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="TextBox4"
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
                    <tr id="Tr1">
                        <td valign="top">
                            <table style="margin-top: 10px;">
                                <tr>
                                    <td style="text-align: center; font-size: medium" onclick="javascript:window.open('DisplayData.aspx?gid=24&data='+this.innerHTML.replace('<B>','').replace('</B>','').replace('<BR/>','').replace('<BR>',''));">
                                        <asp:Literal ID="Literal4" runat="server"></asp:Literal>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <ofc:Chart ID="Chart4" Height="230" Width="330px" runat="server" OnLoad="Chart4_Load" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td>
                            <table style="margin-top: 10px;">
                                <tr>
                                    <td style="text-align: center; font-size: medium" onclick="javascript:window.open('DisplayData.aspx?gid=24&data='+this.innerHTML.replace('<B>','').replace('</B>','').replace('<BR/>','').replace('<BR>',''));">
                                        <asp:Literal ID="Literal6" runat="server"></asp:Literal>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <ofc:Chart ID="Chart6" Height="230" Width="330px" runat="server" OnLoad="Chart6_Load" />
                                    </td>
                                </tr>
                                <tr>
                                    <td style="text-align: center; font-size: medium" onclick="javascript:window.open('DisplayData.aspx?gid=24&data='+this.innerHTML.replace('<B>','').replace('</B>','').replace('<BR/>','').replace('<BR>',''));">
                                        <asp:Literal ID="Literal7" runat="server"></asp:Literal>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <ofc:Chart ID="Chart7" Height="230" Width="330px" runat="server" OnLoad="Chart7_Load" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td valign="top">
                            <table style="margin-top: 10px; margin-bottom: 10px">
                                <tr>
                                    <td style="text-align: center; font-size: medium" onclick="javascript:window.open('DisplayData.aspx?gid=24&data='+this.innerHTML.replace('<B>','').replace('</B>','').replace('<BR/>','').replace('<BR>',''));">
                                        <asp:Literal ID="Literal8" runat="server"></asp:Literal>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <ofc:Chart ID="Chart8" Height="460" Width="330px" runat="server" OnLoad="Chart8_Load" />
                                    </td>
                                </tr>
                            </table>
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
                            Call Out Analysis
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
                                            style="border: 0; cursor: hand" id="IMG4" />
                                    </td>
                                    <td style="width: 3px">
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator4" CssClass="Validator"
                                            runat="server" ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                            ControlToValidate="TextBox9" ErrorMessage="To Date is invalid.">*</asp:RegularExpressionValidator>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="TextBox9"
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
                                            style="border: 0; cursor: hand" id="IMG5" />
                                    </td>
                                    <td style="width: 3px">
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator6" CssClass="Validator"
                                            runat="server" ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                            ControlToValidate="TextBox10" ErrorMessage="To Date is invalid.">*</asp:RegularExpressionValidator>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ControlToValidate="TextBox10"
                                            ErrorMessage="To date cannot be empty">*</asp:RequiredFieldValidator>
                                    </td>
                                    <td align="left">
                                        <asp:Button ID="Button7" runat="server" Text="Generate" OnClick="Button_Submit_Click"
                                            CssClass="button" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr id="Tr2">
                        <td valign="top">
                            <table style="margin-top: 10px;">
                                <tr>
                                    <td style="text-align: center; font-size: medium" onclick="javascript:window.open('DisplayData.aspx?gid=24&data='+this.innerHTML.replace('<B>','').replace('</B>','').replace('<BR/>','').replace('<BR>',''));">
                                        <asp:Literal ID="Literal9" runat="server"></asp:Literal>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <ofc:Chart ID="Chart9" Height="230" Width="330px" runat="server" OnLoad="Chart9_Load" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td>
                            <table style="margin-top: 10px;">
                                <tr>
                                    <td style="text-align: center; font-size: medium" onclick="javascript:window.open('DisplayData.aspx?gid=24&data='+this.innerHTML.replace('<B>','').replace('</B>','').replace('<BR/>','').replace('<BR>',''));">
                                        <asp:Literal ID="Literal10" runat="server"></asp:Literal>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <ofc:Chart ID="Chart10" Height="230" Width="330px" runat="server" OnLoad="Chart10_Load" />
                                    </td>
                                </tr>
                                <tr>
                                    <td style="text-align: center; font-size: medium" onclick="javascript:window.open('DisplayData.aspx?gid=24&data='+this.innerHTML.replace('<B>','').replace('</B>','').replace('<BR/>','').replace('<BR>',''));">
                                        <asp:Literal ID="Literal11" runat="server"></asp:Literal>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <ofc:Chart ID="Chart11" Height="230" Width="330px" runat="server" OnLoad="Chart11_Load" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td valign="top">
                            <table style="margin-top: 10px; margin-bottom: 10px">
                                <tr>
                                    <td style="text-align: center; font-size: medium" onclick="javascript:window.open('DisplayData.aspx?gid=24&data='+this.innerHTML.replace('<B>','').replace('</B>','').replace('<BR/>','').replace('<BR>',''));">
                                        <asp:Literal ID="Literal12" runat="server"></asp:Literal>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <ofc:Chart ID="Chart12" Height="460" Width="330px" runat="server" OnLoad="Chart12_Load" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </div>
        </ComponentArt:PageView>
        <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView3">
            <div>
                <table width="100%" border="0">
                    <tr>
                        <td align="center" style="font-size: large; background-color: Gray; color: White"
                            colspan="3">
                            Call Out Analysis
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
                                        <asp:TextBox ID="TextBox1" runat="server" CssClass="textbox"></asp:TextBox>
                                    </td>
                                    <td style="width: 23px">
                                        <img src="images/IconCalendar.gif" alt="Click to open calender" onclick="javascript:show_calendar('form1.TextBox9');"
                                            style="border: 0; cursor: hand" id="IMG7" />
                                    </td>
                                    <td style="width: 3px">
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator7" CssClass="Validator"
                                            runat="server" ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                            ControlToValidate="TextBox1" ErrorMessage="From Date is invalid.">*</asp:RegularExpressionValidator>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" ControlToValidate="TextBox1"
                                            ErrorMessage="From date cannot be empty">*</asp:RequiredFieldValidator>
                                    </td>
                                    <td style="width: 23px">
                                        To
                                    </td>
                                    <td style="width: 23px">
                                        <asp:TextBox ID="TextBox2" runat="server" CssClass="textbox"></asp:TextBox>
                                    </td>
                                    <td style="width: 23px">
                                        <img src="images/IconCalendar.gif" alt="Click to open calender" onclick="javascript:show_calendar('form1.TextBox10');"
                                            style="border: 0; cursor: hand" id="IMG8" />
                                    </td>
                                    <td style="width: 3px">
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator8" CssClass="Validator"
                                            runat="server" ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                            ControlToValidate="TextBox2" ErrorMessage="To Date is invalid.">*</asp:RegularExpressionValidator>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" ControlToValidate="TextBox2"
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
                    <tr id="Tr3">
                        <td valign="top">
                            <table style="margin-top: 10px;">
                                <tr>
                                    <td style="text-align: center; font-size: medium" onclick="javascript:window.open('DisplayData.aspx?gid=24&data='+this.innerHTML.replace('<B>','').replace('</B>','').replace('<BR/>','').replace('<BR>',''));">
                                        <asp:Literal ID="Literal13" runat="server"></asp:Literal>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <ofc:Chart ID="Chart13" Height="230" Width="330px" runat="server" OnLoad="Chart13_Load" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td>
                            <table style="margin-top: 10px;">
                                <tr>
                                    <td style="text-align: center; font-size: medium" onclick="javascript:window.open('DisplayData.aspx?gid=24&data='+this.innerHTML.replace('<B>','').replace('</B>','').replace('<BR/>','').replace('<BR>',''));">
                                        <asp:Literal ID="Literal14" runat="server"></asp:Literal>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <ofc:Chart ID="Chart14" Height="230" Width="330px" runat="server" OnLoad="Chart14_Load" />
                                    </td>
                                </tr>
                                <tr>
                                    <td style="text-align: center; font-size: medium" onclick="javascript:window.open('DisplayData.aspx?gid=24&data='+this.innerHTML.replace('<B>','').replace('</B>','').replace('<BR/>','').replace('<BR>',''));">
                                        <asp:Literal ID="Literal15" runat="server"></asp:Literal>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <ofc:Chart ID="Chart15" Height="230" Width="330px" runat="server" OnLoad="Chart15_Load" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td valign="top">
                            <table style="margin-top: 10px; margin-bottom: 10px">
                                <tr>
                                    <td style="text-align: center; font-size: medium" onclick="javascript:window.open('DisplayData.aspx?gid=24&data='+this.innerHTML.replace('<B>','').replace('</B>','').replace('<BR/>','').replace('<BR>',''));">
                                        <asp:Literal ID="Literal16" runat="server"></asp:Literal>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <ofc:Chart ID="Chart16" Height="460" Width="330px" runat="server" OnLoad="Chart16_Load" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </div>
        </ComponentArt:PageView>
        <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView4">
            <div>
                <table width="100%" border="0">
                    <tr>
                        <td align="center" style="font-size: large; background-color: Gray; color: White"
                            colspan="3">
                            Call Out Analysis
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
                                        <asp:TextBox ID="TextBox5" runat="server" CssClass="textbox"></asp:TextBox>
                                    </td>
                                    <td style="width: 23px">
                                        <img src="images/IconCalendar.gif" alt="Click to open calender" onclick="javascript:show_calendar('form1.TextBox9');"
                                            style="border: 0; cursor: hand" id="IMG9" />
                                    </td>
                                    <td style="width: 3px">
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator9" CssClass="Validator"
                                            runat="server" ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                            ControlToValidate="TextBox5" ErrorMessage="From Date is invalid.">*</asp:RegularExpressionValidator>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator9" runat="server" ControlToValidate="TextBox5"
                                            ErrorMessage="From date cannot be empty">*</asp:RequiredFieldValidator>
                                    </td>
                                    <td style="width: 23px">
                                        To
                                    </td>
                                    <td style="width: 23px">
                                        <asp:TextBox ID="TextBox6" runat="server" CssClass="textbox"></asp:TextBox>
                                    </td>
                                    <td style="width: 23px">
                                        <img src="images/IconCalendar.gif" alt="Click to open calender" onclick="javascript:show_calendar('form1.TextBox10');"
                                            style="border: 0; cursor: hand" id="IMG10" />
                                    </td>
                                    <td style="width: 3px">
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator10" CssClass="Validator"
                                            runat="server" ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                            ControlToValidate="TextBox6" ErrorMessage="To Date is invalid.">*</asp:RegularExpressionValidator>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator10" runat="server" ControlToValidate="TextBox6"
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
                    <tr id="Tr4">
                        <td valign="top">
                            <table style="margin-top: 10px;">
                                <tr>
                                    <td style="text-align: center; font-size: medium" onclick="javascript:window.open('DisplayData.aspx?gid=24&data='+this.innerHTML.replace('<B>','').replace('</B>','').replace('<BR/>','').replace('<BR>',''));">
                                        <asp:Literal ID="Literal17" runat="server"></asp:Literal>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <ofc:Chart ID="Chart17" Height="230" Width="330px" runat="server" OnLoad="Chart17_Load" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td>
                            <table style="margin-top: 10px;">
                                <tr>
                                    <td style="text-align: center; font-size: medium" onclick="javascript:window.open('DisplayData.aspx?gid=24&data='+this.innerHTML.replace('<B>','').replace('</B>','').replace('<BR/>','').replace('<BR>',''));">
                                        <asp:Literal ID="Literal18" runat="server"></asp:Literal>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <ofc:Chart ID="Chart18" Height="230" Width="330px" runat="server" OnLoad="Chart18_Load" />
                                    </td>
                                </tr>
                                <tr>
                                    <td style="text-align: center; font-size: medium" onclick="javascript:window.open('DisplayData.aspx?gid=24&data='+this.innerHTML.replace('<B>','').replace('</B>','').replace('<BR/>','').replace('<BR>',''));">
                                        <asp:Literal ID="Literal19" runat="server"></asp:Literal>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <ofc:Chart ID="Chart19" Height="230" Width="330px" runat="server" OnLoad="Chart19_Load" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td valign="top">
                            <table style="margin-top: 10px; margin-bottom: 10px">
                                <tr>
                                    <td style="text-align: center; font-size: medium" onclick="javascript:window.open('DisplayData.aspx?gid=24&data='+this.innerHTML.replace('<B>','').replace('</B>','').replace('<BR/>','').replace('<BR>',''));">
                                        <asp:Literal ID="Literal20" runat="server"></asp:Literal>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <ofc:Chart ID="Chart20" Height="460" Width="330px" runat="server" OnLoad="Chart20_Load" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </div>
        </ComponentArt:PageView>
    </ComponentArt:MultiPage>
    </form>
</body>
</html>
