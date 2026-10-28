<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditReportGenerationSchedule.aspx.cs"
    Inherits="Avanza.iSuite.UI.EditReportGenerationSchedule" %>

<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Edit Report Generation</title>
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
    <script type="text/javascript">
        window.parent.window.contentPage = '';          
        
        
    </script>
    <link href="../content/iSuite.css" rel="stylesheet" />
</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)' onload="hideProgressBar();">
    <form id="form1" runat="server">
    <table cellpadding="0" cellspacing="0" border="0" class="MainBox" width="100%" style="padding-left: 2px;">
        <tr>
            <td class="PageHeading" align="left">
                Edit Report Generation
            </td>
        </tr>
        <tr>
            <td class="SearchHeadPaneGradient" colspan="2">
            </td>
        </tr>
        <tr style="height: 20px">
            <td colspan="2" style="color: red;" align="center" valign="top">
                <asp:Literal ID="Literal_error" runat="server" EnableViewState="False"></asp:Literal>
            </td>
        </tr>
        <tr>
            <td>
                <asp:ValidationSummary ID="valSummary" runat="server" />
            </td>
        </tr>
        <tr>
            <td>
                <table id="inner" border="0" cellpadding="0" cellspacing="0" width="60%">
                    <tr>
                        <td>
                            <asp:Label ID="Label_ReportName_Caption" runat="server" Text="Report Name:"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="Label_ReportName" runat="server" Font-Bold="true"></asp:Label>
                        </td>
                    </tr>
					<tr>
                        <td>
                            <asp:Label ID="Label_CIT_Caption" runat="server" Text="CIT:"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="Label_CIT" runat="server" Font-Bold="true"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label_ReportFriendlyName_Caption" runat="server" Text="Report Friendly Name:"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_ReportFriendlyName" runat="server" Width="300px" CssClass="textbox"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="reqReportFriendlyName" runat="server" ControlToValidate="TextBox_ReportFriendlyName"
                                ErrorMessage="Please enter report friendly name">*</asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label_PhysicalPath_Caption" runat="server" Text="Physical Path:"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_PhysicalPath" runat="server" Width="500px" CssClass="textbox"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator_physicalpath" runat="server"
                                ControlToValidate="TextBox_PhysicalPath" ErrorMessage="Please enter report physical path">*</asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td valign="top">
                            <asp:Label ID="Label_ReportRecipients_Caption" runat="server" Text="Recipients:"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_ReportRecipients" runat="server" Rows="6" TextMode="MultiLine"
                                Width="500px" CssClass="textbox"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="TextBox_ReportRecipients"
                                ErrorMessage="Please enter report receipients">*</asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label_TempPath_Caption" runat="server" Text="Temp Path:"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_TempPath" runat="server" Width="300px" CssClass="textbox"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator_temppath" runat="server" ControlToValidate="TextBox_TempPath"
                                ErrorMessage="Please enter report temporary path">*</asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Schedule Type
                        </td>
                        <td>
                            <asp:RadioButtonList ID="RadioButtonList_ScheduleType" runat="server" AutoPostBack="true"
                                RepeatDirection="Horizontal" OnSelectedIndexChanged="RadioButtonList_ScheduleType_SelectedIndexChanged">
                                <asp:ListItem Text="Relative" Value="0" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Absolute" Value="1"></asp:ListItem>
                            </asp:RadioButtonList>
                        </td>
                    </tr>
                    <tr id="trRelative" runat="server">
                        <td>
                            <asp:Label ID="Label_MinToDownloadAgain" runat="server" Text="Minutes to schedule again:"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_MinToScheduleAgain" runat="server" CssClass="textbox"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="TextBox_MinToScheduleAgain"
                                ErrorMessage="Please enter minutes to schedule again">*</asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="TextBox_MinToScheduleAgain"
                                ValidationExpression="\d+" ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                        </td>
                    </tr>
                    <tr id="trAbsolute3" runat="server">
                        <td>
                            Schedule Time
                        </td>
                        <td>
                            Hours:&nbsp;<asp:DropDownList ID="DropDownList_Hour" runat="server" CssClass="dropDown">
                            </asp:DropDownList>
                            Minutes:&nbsp;<asp:DropDownList ID="DropDownList_Min" CssClass="dropDown"
                                runat="server" />
                        </td>
                    </tr>
                    <tr id="trAbsolute1" runat="server">
                        <td>
                        </td>
                        <td>
                            <asp:ListBox ID="Listbox_schedule" runat="server" CssClass="textbox"></asp:ListBox>
                        </td>
                    </tr>
                    <tr id="trAbsolute2" runat="server">
                        <td>
                        </td>
                        <td>
                            <asp:Button ID="Button_Add" runat="server" Text="Add Schedule" CssClass="button" OnClick="Button_AddSchedule_Click" />
                            <asp:Button ID="Button_Remove" runat="server" Text="Remove Schedule" CssClass="button" OnClick="Button_RemoveSchedule_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label_RetryCount_Caption" runat="server" Text="Retry Count:"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_RetryCount" runat="server" CssClass="textbox"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="TextBox_RetryCount"
                                ErrorMessage="Please enter retry count">*</asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="RegRetryCount" runat="server" ControlToValidate="TextBox_RetryCount"
                                ValidationExpression="\d+" ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Export Type
                        </td>
                        <td>
                            <asp:CheckBoxList ID="CheckBoxList_ExportType" runat="server" RepeatDirection="Horizontal">
                                <asp:ListItem Value="1">Portable Document Format(PDF)</asp:ListItem>
                                <asp:ListItem Value="2">Excel</asp:ListItem>
                            </asp:CheckBoxList>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Export Data Older than
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_ExportDataOlderThan" runat="server" CssClass="textbox"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="TextBox_ExportDataOlderThan"
                                ErrorMessage="Please enter export data value">*</asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="TextBox_ExportDataOlderThan"
                                ValidationExpression="^[-]?\d*$" ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" style="font-weight: bold; padding-top: 30px;">
                            <asp:Button ID="Button_SaveUpdateEditReportGenerationSchedule" runat="server" Text="Save"
                                OnClick="Button_SaveUpdateEditReportGenerationSchedule_Click" CssClass="button"
                                Width="60px" />&nbsp;
                            <input id="Button_Cancel" type="button" value="Cancel" onclick="window.location ='ScheduleReportGeneration.aspx'"
                                class="button" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
