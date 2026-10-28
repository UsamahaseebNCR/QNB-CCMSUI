<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddEditOrganization.aspx.cs"
    Inherits="CCMSUI.admin.AddEditOrganization" %>

<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Add Edit ATM</title>
    <link href="../content/iSuite.css" rel="stylesheet" />
    <script src="../content/shortcut.js" type="text/javascript" language="javascript"></script>
    <script src="../content/main.js" type="text/javascript" language="javascript"></script>
    <script src="../content/boxover.js" type="text/javascript" language="javascript">
    </script>
    <script type="text/javascript">
        window.parent.window.contentPage = '';
        function setDirtyFlagToUpdateDffPasswordInDb() {
            document.getElementById('hidden_dirtyFlagForDffFTPPassword').value = 1;
        }
        function setDirtyFlagToUpdateOCPasswordInDb() {
            document.getElementById('hidden_dirtyFlagForCashOrderPassword').value = 1;
        }
        function setDirtyFlagToUpdateSMTPPasswordInDb() {
            document.getElementById('hidden_dirtyFlagForSMTPPassword').value = 1;
        }
        
    </script>
</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)'
    onload="hideProgressBar();">
    <form id="form1" runat="server">

    <input type="hidden" id="hidden_dirtyFlagForDffFTPPassword" runat="server" />
    <input type="hidden" id="hidden_dirtyFlagForCashOrderPassword" runat="server" />
    <input type="hidden" id="hidden_dirtyFlagForSMTPPassword" runat="server" />
    

    <table cellpadding="0" cellspacing="0" border="0" class="MainBox" width="100%">
        <tr>
            <td class="PageHeading" align="left" colspan="2" style="height: 25px">
                <%=Request.QueryString["organization_id"] == null?"Add a New Organization ":"Edit an Organization"%>
            </td>
        </tr>
        <tr>
            <td class="SearchHeadPaneGradient" colspan="2">
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:ValidationSummary ID="ValidationSummary1" runat="server"></asp:ValidationSummary>
            </td>
        </tr>
        <tr>
            <td class="ConfirmationText" align="center">
                <%=Session[SessionVars.notice.ToString()]%>
                <%Session[SessionVars.notice.ToString()] = "";%>
                <asp:Literal ID="Literal_ok" runat="server" EnableViewState="False"></asp:Literal>
            </td>
        </tr>
        <tr style="height: 20px">
            <td colspan="2" style="color: red; height: 20px;" align="center" valign="top">
                <asp:Literal ID="Literal_error" runat="server" EnableViewState="False"></asp:Literal>
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
                        <ComponentArt:TabStripTab ID="TabStripTab_general" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                            ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                            DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                            Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                            SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="General Configuration">
                        </ComponentArt:TabStripTab>
                        <ComponentArt:TabStripTab ID="TabStripTab_dailyfeed" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                            ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                            DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                            Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                            SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Daily Feed Configuration">
                        </ComponentArt:TabStripTab>
                        <ComponentArt:TabStripTab ID="TabStripTab_cashorder" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                            ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                            DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                            Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                            SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Cash Order Configuration">
                        </ComponentArt:TabStripTab>
                        <ComponentArt:TabStripTab ID="TabStripTab1" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                            ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                            DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                            Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                            SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Downloading Configuration">
                        </ComponentArt:TabStripTab>
                        <ComponentArt:TabStripTab ID="TabStripTab2" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                            ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                            DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                            Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                            SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Smtp Configuration">
                        </ComponentArt:TabStripTab>

                      
                    </Tabs>
                </ComponentArt:TabStrip>
                <ComponentArt:MultiPage ID="MultiPage_AddEdit" CssClass="MultiPage" runat="server">
                    <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView_general">
                        <table id="Table1" border="0" cellpadding="0" cellspacing="0" style="padding-left: 10px"
                            width="60%">
                            <tr>
                                <td>
                                    Name
                                </td>
                                <td>
                                    <asp:TextBox ID="TextBox_OrganizationName" runat="server" CssClass="textbox"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="requiredfieldvalidator_OrganizationName" runat="server"
                                        ControlToValidate="TextBox_OrganizationName" ErrorMessage="Please enter Organization Name"
                                        Text="*"></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Location
                                </td>
                                <td>
                                    <asp:TextBox ID="TextBox_Location" runat="server" CssClass="textbox"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="requiredfieldvalidator_Title" runat="server" ControlToValidate="TextBox_Location"
                                        ErrorMessage="Please enter location." Text="*"></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Country
                                </td>
                                <td>
                                    <asp:TextBox ID="TextBox_Country" runat="server" CssClass="textbox"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="requiredfieldvalidator2" runat="server" ControlToValidate="TextBox_Country"
                                        ErrorMessage="Please enter country." Text="*"></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Master Customer Number
                                </td>
                                <td>
                                    <asp:TextBox ID="TextBox_MCN" runat="server" CssClass="textbox"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="requiredfieldvalidator_mcn" runat="server" ControlToValidate="TextBox_MCN"
                                        ErrorMessage="Please enter MCN.">*</asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Status
                                </td>
                                <td>
                                    <asp:RadioButton ID="RadioButton_active" runat="server" Checked="True" GroupName="status"
                                        Text="Active" />&nbsp;
                                    <asp:RadioButton ID="RadioButton_inactive" runat="server" GroupName="status" Text="Inactive" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Suspend Cash Orders
                                </td>
                                <td>
                                    <asp:CheckBox ID="CheckBox_SuspendCashOrders" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Suspend DFF
                                </td>
                                <td>
                                    <asp:CheckBox ID="CheckBox_SuspendDFF" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Allow Secured Access
                                </td>
                                <td>
                                    <asp:CheckBox ID="CheckBox_SecuredAccess" runat="server" Checked="true" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Default Data Extraction
                                </td>
                                <td>
                                    <asp:CheckBox ID="CheckBox_EJ" runat="server" Text="EJ" />
                                    <asp:CheckBox ID="CheckBox_Counter" runat="server" Checked="true" Text="Counter" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Priority
                                </td>
                                <td>
                                    <asp:RadioButtonList ID="RadioButtonList_Priority" runat="server" RepeatColumns="2">
                                        <asp:ListItem Text="EJ" Value="0"></asp:ListItem>
                                        <asp:ListItem Text="Counter" Value="1" Selected="True"></asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Offline folder path
                                </td>
                                <td>
                                    <asp:TextBox ID="TextBox_OfflineFolderPath" runat="server" CssClass="textbox"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Offline folder path for Multicurrency
                                </td>
                                <td>
                                    <asp:TextBox ID="TextBox_OfflineFolderPathForMulticurrency" runat="server" CssClass="textbox"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td style="vertical-align: top;">
                                    CIT
                                </td>
                                <td style="vertical-align: top;">
                                    <asp:ListBox ID="ListBox_Cit" runat="server" SelectionMode="Multiple" Width="204px"
                                        Height="70px" CssClass="textbox"></asp:ListBox>
                                </td>
                            </tr>
                            <tr>
                                <td style="vertical-align: top;">
                                    Recipients
                                </td>
                                <td style="vertical-align: top;">
                                    <asp:ListBox ID="ListBox_UsersEmail" runat="server" SelectionMode="Multiple" Width="204px"
                                        Height="70px" CssClass="textbox"></asp:ListBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    
                                </td>
                                <td>
                                   
                                </td>
                            </tr>
                        </table>
                    </ComponentArt:PageView>
                    <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView_dailyfeed">
                        <table id="Table2" border="0" cellpadding="0" cellspacing="0" style="padding-left: 10px"
                            width="70%">
                            <tr>
                                <td>
                                    FTP Uri
                                </td>
                                <td>
                                    <asp:TextBox CssClass="textbox" ID="TextBox_DFUri" runat="server"></asp:TextBox>
                                    <img src="../images/info.gif" title="header=[FTP Uri] body=[This is the ftp location on which daily feed files will be uploaded.]" />
                                    <%-- <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="TextBox_DFUri"
                                        ErrorMessage="Please enter ftp uri for daily feed file">*</asp:RequiredFieldValidator>--%>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    FTP Username
                                </td>
                                <td>
                                    <asp:TextBox CssClass="textbox" ID="TextBox_DFUsername" runat="server"></asp:TextBox>
                                    <img src="../images/info.gif" title="header=[FTP Username] body=[This is the username for the specified ftp location.]" />
                                    <%-- <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="TextBox_DFUsername"
                                        ErrorMessage="Please enter ftp username for daily feed file">*</asp:RequiredFieldValidator>--%>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    FTP Password
                                </td>
                                <td>
                                    <asp:TextBox TextMode="Password" CssClass="textbox" ID="TextBox_DFPassword" runat="server"></asp:TextBox>
                                    <img src="../images/info.gif" title="header=[FTP Password] body=[This is the password for the specified ftp location.]" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    DFs Generation Time
                                </td>
                                <td>
                                    Hours &nbsp;<asp:DropDownList ID="DropDownList_DFGenTime_Hour" CssClass="dropDown"
                                        runat="server" Width="53px">
                                    </asp:DropDownList>
                                    Minutes &nbsp;<asp:DropDownList ID="DropDownList_DFGenTime_Min" CssClass="dropDown"
                                        runat="server" Width="53px">
                                    </asp:DropDownList>
                                    &nbsp;<img src="../images/info.gif" title="header=[DFs Generation Time] body=[This will be time at which daily feed file will be generated.]" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    DFs File Path
                                </td>
                                <td>
                                    <asp:TextBox CssClass="textbox" ID="TextBox_DFFilePath" runat="server"></asp:TextBox>
                                    <img src="../images/info.gif" title="header=[DFs File Path] body=[This is the path where daily feed files will be generated.]" />
                                    <%--<asp:RequiredFieldValidator ID="RequiredFieldValidator11" runat="server" ControlToValidate="TextBox_DFFilePath"
                                        ErrorMessage="Please enter folder path for daily feed file">*</asp:RequiredFieldValidator>--%>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Lag Interval
                                </td>
                                <td>
                                    <asp:TextBox CssClass="textbox" ID="TextBox_LagInterval" runat="server"></asp:TextBox>
                                    <img src="../images/info.gif" title="header=[Lag Interval] body=[This specify the delay between current system time and summary generation time.]" />
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" ValidationExpression="\d+"
                                        ErrorMessage="Only numbers are allowed" ControlToValidate="TextBox_LagInterval">*</asp:RegularExpressionValidator>
                                    <%-- <asp:RequiredFieldValidator ID="RequiredFieldValidator12" runat="server" ControlToValidate="TextBox_LagInterval"
                                        ErrorMessage="Please enter lag interval for daily feed file">*</asp:RequiredFieldValidator>--%>
                                </td>
                            </tr>
                        </table>
                    </ComponentArt:PageView>
                    <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView_cashorder">
                        <table id="Table3" border="0" cellpadding="0" cellspacing="0" style="padding-left: 10px"
                            width="70%">
                            <tr>
                                <td>
                                    FTP Uri
                                </td>
                                <td>
                                    <asp:TextBox CssClass="textbox" ID="TextBox_OptiFTPUri" runat="server"></asp:TextBox>
                                    <img src="../images/info.gif" title="header=[FTP Uri] body=[This is the ftp location on which cash orders are generated by Opticash.]" />
                                    <%-- <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ControlToValidate="TextBox_OptiFTPUri"
                                        ErrorMessage="Please enter ftp uri for downloading cash orders">*</asp:RequiredFieldValidator>--%>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    FTP Username
                                </td>
                                <td>
                                    <asp:TextBox CssClass="textbox" ID="TextBox_OptiFTPUsername" runat="server"></asp:TextBox>
                                    <img src="../images/info.gif" title="header=[FTP Username] body=[This is the username for the specified ftp location.]" />
                                    <%--  <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" ControlToValidate="TextBox_OptiFTPUsername"
                                        ErrorMessage="Please enter ftp username for downloading cash orders">*</asp:RequiredFieldValidator>--%>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    FTP Password
                                </td>
                                <td>
                                    <asp:TextBox TextMode="Password" CssClass="textbox" ID="TextBox_OptiFTPPassword"
                                        runat="server"></asp:TextBox>
                                    <img src="../images/info.gif" title="header=[FTP Password] body=[This is the password for the specified ftp location.]" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    FTP Archive Folder Uri
                                </td>
                                <td>
                                    <asp:TextBox CssClass="textbox" ID="TextBox_OptiFTPArchiveFolder" runat="server"></asp:TextBox>
                                    <img src="../images/info.gif" title="header=[FTP Archive Folder Uri] body=[This is the remote path where cash orders will be archived.]" />
                                    <%--  <asp:RequiredFieldValidator ID="RequiredFieldValidator13" runat="server" ControlToValidate="TextBox_OptiFTPArchiveFolder"
                                        ErrorMessage="Please enter archive folder uri for downloaded cash orders">*</asp:RequiredFieldValidator>--%>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Downloaded Cash Orders File Path
                                </td>
                                <td>
                                    <asp:TextBox CssClass="textbox" ID="TextBox_CashOrderFilePath" runat="server"></asp:TextBox>
                                    <img src="../images/info.gif" title="header=[Downloaded Cash Orders File Path] body=[This is the local path where cash orders will be downloaded.]" />
                                    <%--<asp:RequiredFieldValidator ID="RequiredFieldValidator14" runat="server" ControlToValidate="TextBox_CashOrderFilePath"
                                        ErrorMessage="Please enter downloaded cash orders file path">*</asp:RequiredFieldValidator>--%>
                                </td>
                            </tr>
                        </table>
                    </ComponentArt:PageView>
                    <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView1">
                        <table id="Table4" border="0" cellpadding="0" cellspacing="0" style="padding-left: 10px"
                            width="70%">
                            <tr>
                                <td>
                                    Cash Data Log Start Time
                                </td>
                                <td>
                                    Hours:&nbsp;<asp:DropDownList ID="DropDownList_CashDataHour" runat="server" CssClass="dropDown">
                                    </asp:DropDownList>
                                    Minutes:&nbsp;<asp:DropDownList ID="DropDownList_CashDataMin" CssClass="dropDown"
                                        runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                </td>
                                <td>
                                    <asp:RadioButtonList ID="RadioButtonList_DownloadTime" runat="server" AutoPostBack="true"
                                        OnSelectedIndexChanged="RadioButtonList_DownloadTime_Selected" RepeatColumns="6"
                                        RepeatDirection="Horizontal" BackColor="Info">
                                        <asp:ListItem Text="3 Minutes" Value="3"></asp:ListItem>
                                        <asp:ListItem Text="5 Minutes" Value="5"></asp:ListItem>
                                        <asp:ListItem Text="15 Minutes" Value="15"></asp:ListItem>
                                        <asp:ListItem Text="30 Minutes" Value="30"></asp:ListItem>
                                        <asp:ListItem Text="45 Minutes" Value="45"></asp:ListItem>
                                        <asp:ListItem Text="60 Minutes" Value="60"></asp:ListItem>
                                        <asp:ListItem Text="120 Minutes" Value="120"></asp:ListItem>
                                        <asp:ListItem Text="240 Minutes" Value="240"></asp:ListItem>
                                        <asp:ListItem Text="480 Minutes" Value="480"></asp:ListItem>
                                        <asp:ListItem Text="640 Minutes" Value="640"></asp:ListItem>
                                        <asp:ListItem Text="960 Minutes" Value="960"></asp:ListItem>
                                        <asp:ListItem Text="1440 Minutes" Value="1440"></asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                </td>
                                <td style="vertical-align: top">
                                    <asp:ListBox ID="ListBox_CashDataSchedules" runat="server" SelectionMode="Multiple"
                                        Width="125px"></asp:ListBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                </td>
                                <td style="vertical-align: top">
                                    <asp:Button CssClass="button" ID="Button_AddSchedule" runat="server" Text="Add Schedule"
                                        OnClick="Button_AddSchedule_Click" CausesValidation="false" />
                                    <asp:Button CssClass="button" ID="Button_RemoveSchedule" runat="server" Text="Remove Schedule"
                                        OnClick="Button_RemoveSchedule_Click" CausesValidation="false" />
                                </td>
                            </tr>
                        </table>
                    </ComponentArt:PageView>
                    <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView2">
                        <table>
                            <tr>
                                <td colspan="2" style="height: 34px" valign="middle">
                                    <span style="color: Navy; font-weight: bold;">Configuration For SMTP </span>&nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Server
                                </td>
                                <td>
                                    <asp:TextBox CssClass="textbox" ID="TextBox_SMTPServer" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Port
                                </td>
                                <td>
                                    <asp:TextBox CssClass="textbox" ID="TextBox_SmtpPort" runat="server"></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator4" runat="server" ValidationExpression="\d+"
                                        ErrorMessage="Only numbers are allowed" ControlToValidate="TextBox_SmtpPort">*</asp:RegularExpressionValidator>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Username
                                </td>
                                <td>
                                    <asp:TextBox CssClass="textbox" ID="TextBox_Username" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Password
                                </td>
                                <td>
                                    <asp:TextBox CssClass="textbox" ID="TextBox_Password" TextMode="Password" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                        </table>
                    </ComponentArt:PageView>
                    
                </ComponentArt:MultiPage>
            </td>
        </tr>
        <tr>
            <td style="font-weight: bold; padding-top: 30px; width: 100%" colspan="2">
                <asp:Button ID="Button_SaveUpdateOrganization" runat="server" Text="Save" OnClick="Button_SaveUpdateOrganization_Click"
                    CssClass="button" Width="60px" />&nbsp;
                <input id="Button_Cancel" type="button" value="Cancel" onclick="window.location ='Organization.aspx'"
                    class="button" />
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
