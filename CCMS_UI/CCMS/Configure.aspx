<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Configure.aspx.cs" Inherits="CCMSUI.CCMS.Configure" %>

<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
    <title>Application Configuration</title>
    <script type="text/javascript">
        window.parent.window.contentPage = '';
        function setDirtyFlagToUpdatePasswordInDb() {
            document.getElementById('hidden_dirtyFlagForUserPassword').value = 1;
            // alert('set');
        }     
    </script>
    <link href="../content/iSuite.css" rel="stylesheet" />
    <script src="../content/boxover.js" type="text/javascript" language="javascript"></script>
</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)'
    onload="hideProgressBar();">
    <form id="form1" runat="server">
     <input type="hidden" id="hidden_dirtyFlagForUserPassword" runat="server" />
   
    <table cellpadding="0" cellspacing="0" border="0" width="100%">
        <tr>
            <td class="PageHeading" align="left" colspan="2">
                Application Configuration
            </td>
        </tr>
        <%-- <tr>
                <td class="SearchHeadPaneGradient" colspan="2">
                kjhkjh
                </td>
            </tr>
        --%>
        <tr>
            <td colspan="2" style="color: red; padding-bottom: 10px" align="center">
                <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="False"
                    ShowSummary="True" CssClass="ValidationText"></asp:ValidationSummary>
                <asp:Literal ID="Literal_error" runat="server" EnableViewState="False" />
            </td>
        </tr>
        <tr>
            <td>
                <ComponentArt:TabStrip ID="TabStrip_AddEdit" CssClass="TopGroup" DefaultItemLookId="DefaultTabLook"
                    DefaultSelectedItemLookId="SelectedTabLook" DefaultDisabledItemLookId="DisabledTabLook"
                    DefaultGroupTabSpacing="1px" ImagesBaseUrl="../images/Tabimages/" MultiPageId="MultiPage_isuite"
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
                        <ComponentArt:TabStripTab runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                            ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                            DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                            Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                            SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Currency Management System General">
                        </ComponentArt:TabStripTab>
                        <ComponentArt:TabStripTab ID="TabStripTab2" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                            ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                            DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                            Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                            SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="DashBoards Configuration">
                        </ComponentArt:TabStripTab>
                        <ComponentArt:TabStripTab ID="TabStripTab3" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                            ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                            DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                            Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                            SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Currency And Cash Data Archiving/Purging">
                        </ComponentArt:TabStripTab>
                        <ComponentArt:TabStripTab ID="TabStripTab1" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                            ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                            DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                            Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                            SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Services Management">
                        </ComponentArt:TabStripTab>
                    </Tabs>
                </ComponentArt:TabStrip>
                <ComponentArt:MultiPage ID="MultiPage_isuite" CssClass="MultiPage" runat="server">
                    <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView_iSuite"
                        Width="100%">
                        <table width="100%" border="0">
                            <tr>
                                <td style="width: 50%">
                                    <table>
                                        <tr>
                                            <td>
                                                Server IP
                                            </td>
                                            <td>
                                                <asp:TextBox CssClass="textbox" ID="TextBox_ServerIP" runat="server"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator10" runat="server" ControlToValidate="TextBox_ServerIP"
                                                    ErrorMessage="Please enter server ip">*</asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                Server Port
                                            </td>
                                            <td>
                                                <asp:TextBox CssClass="textbox" ID="TextBox_ServerPort" runat="server"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator9" runat="server" ControlToValidate="TextBox_ServerPort"
                                                    ErrorMessage="Please enter server port">*</asp:RequiredFieldValidator>
                                                <asp:RegularExpressionValidator ID="RegularExpressionValidator6" runat="server" ValidationExpression="\d+"
                                                    ErrorMessage="Only numbers are allowed" ControlToValidate="TextBox_ServerPort">*</asp:RegularExpressionValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                ATM Default Port
                                            </td>
                                            <td>
                                                <asp:TextBox CssClass="textbox" ID="TextBox_Port" runat="server"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" ControlToValidate="TextBox_Port"
                                                    ErrorMessage="Please enter ATM default port">*</asp:RequiredFieldValidator>
                                                <asp:RegularExpressionValidator ID="RegularExpressionValidator5" runat="server" ValidationExpression="\d+"
                                                    ErrorMessage="Only numbers are allowed" ControlToValidate="TextBox_Port">*</asp:RegularExpressionValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                Is Ciphered Comm
                                            </td>
                                            <td>
                                                <asp:CheckBox ID="CheckBox_IsCipheredComm" runat="server" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                Cash Order Execution Time
                                            </td>
                                            <td>
                                                Hours:&nbsp;<asp:DropDownList ID="DropDownList_ExecHour" runat="server" CssClass="dropDown">
                                                </asp:DropDownList>
                                                Minutes:&nbsp;<asp:DropDownList ID="DropDownList_ExecMin" CssClass="dropDown" runat="server" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                Service Refresh Interval(min)
                                            </td>
                                            <td>
                                                <asp:TextBox CssClass="textbox" ID="TextBox_RefreshInterval" runat="server"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="ReqRefInterval" runat="server" ControlToValidate="TextBox_RefreshInterval"
                                                    ErrorMessage="Please enter service refresh interval">*</asp:RequiredFieldValidator>
                                                <asp:RegularExpressionValidator ID="RegRefreshInterval" runat="server" ValidationExpression="\d+"
                                                    ErrorMessage="Only numbers are allowed" ControlToValidate="TextBox_RefreshInterval">*</asp:RegularExpressionValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                Parsing Enabled
                                            </td>
                                            <td>
                                                <asp:CheckBox ID="CheckBox_ParsingEnabled" runat="server" />
                                            </td>
                                        </tr>
                                         <tr>
                                            <td>
                                                Google Maps Enabled
                                            </td>
                                            <td>
                                               <asp:CheckBox ID="CheckBox_GMapEnabled" runat="server" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                Service Logging Level
                                            </td>
                                            <td style="height: 12px">
                                                <asp:DropDownList ID="DropDownList_ServicelogLevel" runat="server" Width="100px"
                                                    CssClass="textbox">
                                                    <asp:ListItem>Error</asp:ListItem>
                                                    <asp:ListItem>Warning</asp:ListItem>
                                                    <asp:ListItem>Info</asp:ListItem>
                                                    <asp:ListItem>Verbose</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                Cash Data Log Stores Location
                                            </td>
                                            <td>
                                                <asp:TextBox CssClass="textbox" ID="TextBox_CashDataLogLocation" runat="server"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="TextBox_CashDataLogLocation"
                                                    ErrorMessage="Please enter cash data log store path">*</asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                Log File Path
                                            </td>
                                            <td>
                                                <asp:TextBox CssClass="textbox" ID="TextBox_LogFilePath" runat="server"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="TextBox_LogFilePath"
                                                    ErrorMessage="Please enter log file path">*</asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                Temporary Folder Path
                                            </td>
                                            <td>
                                                <asp:TextBox CssClass="textbox" ID="TextBox_TemporaryFolder" runat="server"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="TextBox_TemporaryFolder"
                                                    ErrorMessage="Please enter temporary folder path">*</asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                       
                                        <tr>
                                            <td>
                                                Downloaded Folder Path
                                            </td>
                                            <td>
                                                <asp:TextBox CssClass="textbox" ID="TextBox_DownloadedFolderPath" runat="server"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ControlToValidate="TextBox_DownloadedFolderPath"
                                                    ErrorMessage="Please enter downloaded folder path">*</asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                Heart Beat Refresh Interval
                                            </td>
                                            <td>
                                                <asp:TextBox CssClass="textbox" ID="TextBox_HeartBeatInterval" runat="server"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="TextBox_HeartBeatInterval"
                                                    ErrorMessage="Please enter heart beat refresh interval">*</asp:RequiredFieldValidator>
                                                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ValidationExpression="\d+"
                                                    ErrorMessage="Only numbers are allowed" ControlToValidate="TextBox_HeartBeatInterval">*</asp:RegularExpressionValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            
                                        </tr>
                                    </table>
                                </td>
                                <td valign="top">
                                    <table width="100%" border="0">
                                        <tr>
                                            <td style="width: 100%">
                                                <fieldset style="width: 100%">
                                                    <legend><b>Configure Job Retries</b></legend>
                                                    <table width="100%">
                                                        <tr>
                                                            <td>
                                                                Number of retry for Cash Order Upload
                                                            </td>
                                                            <td>
                                                                <asp:TextBox CssClass="textbox" ID="TextBox_NumberOfRetryCashOrderUpload" runat="server"></asp:TextBox>
                                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="TextBox_NumberOfRetryCashOrderUpload"
                                                                    ErrorMessage="Please enter number of retry for cash order upload">*</asp:RequiredFieldValidator>
                                                                <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ValidationExpression="\d+"
                                                                    ErrorMessage="Only numbers are allowed" ControlToValidate="TextBox_NumberOfRetryCashOrderUpload">*</asp:RegularExpressionValidator>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                Number of retry for Cash Order Download
                                                            </td>
                                                            <td>
                                                                <asp:TextBox CssClass="textbox" ID="TextBox_NumberOfRetryCashOrderDownload" runat="server"></asp:TextBox>
                                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" ControlToValidate="TextBox_NumberOfRetryCashOrderDownload"
                                                                    ErrorMessage="Please enter number of retry for cash order download">*</asp:RequiredFieldValidator>
                                                                <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" ValidationExpression="\d+"
                                                                    ErrorMessage="Only numbers are allowed" ControlToValidate="TextBox_NumberOfRetryCashOrderDownload">*</asp:RegularExpressionValidator>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                Number of retry for DFF Upload
                                                            </td>
                                                            <td>
                                                                <asp:TextBox CssClass="textbox" ID="TextBox_NumberOfRetryDFFUpload" runat="server"></asp:TextBox>
                                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator11" runat="server" ControlToValidate="TextBox_NumberOfRetryDFFUpload"
                                                                    ErrorMessage="Please enter number of retry for DFF upload">*</asp:RequiredFieldValidator>
                                                                <asp:RegularExpressionValidator ID="RegularExpressionValidator9" runat="server" ValidationExpression="\d+"
                                                                    ErrorMessage="Only numbers are allowed" ControlToValidate="TextBox_NumberOfRetryDFFUpload">*</asp:RegularExpressionValidator>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                Number of retry for Uploading Configuration
                                                            </td>
                                                            <td>
                                                                <asp:TextBox CssClass="textbox" ID="TextBox_NumberOfRetryConfUpload" runat="server"></asp:TextBox>
                                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator12" runat="server" ControlToValidate="TextBox_NumberOfRetryConfUpload"
                                                                    ErrorMessage="Please enter number of retry for uploading configuration">*</asp:RequiredFieldValidator>
                                                                <asp:RegularExpressionValidator ID="RegularExpressionValidator10" runat="server"
                                                                    ValidationExpression="\d+" ErrorMessage="Only numbers are allowed" ControlToValidate="TextBox_NumberOfRetryConfUpload">*</asp:RegularExpressionValidator>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                Number of retry for Downloading Counter File
                                                            </td>
                                                            <td>
                                                                <asp:TextBox CssClass="textbox" ID="TextBox_NoOfRetryCounterFile" runat="server"></asp:TextBox>
                                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator14" runat="server" ControlToValidate="TextBox_NoOfRetryCounterFile"
                                                                    ErrorMessage="Please enter number of retry for downloading counter file">*</asp:RequiredFieldValidator>
                                                                <asp:RegularExpressionValidator ID="RegularExpressionValidator12" runat="server"
                                                                    ValidationExpression="\d+" ErrorMessage="Only numbers are allowed" ControlToValidate="TextBox_NoOfRetryCounterFile">*</asp:RegularExpressionValidator>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                Number of retry for Executing Restart Schedule
                                                            </td>
                                                            <td>
                                                                <asp:TextBox CssClass="textbox" ID="TextBox_NoOfRetryRestart" runat="server"></asp:TextBox>
                                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator15" runat="server" ControlToValidate="TextBox_NoOfRetryRestart"
                                                                    ErrorMessage="Please enter number of retry for restart schedule">*</asp:RequiredFieldValidator>
                                                                <asp:RegularExpressionValidator ID="RegularExpressionValidator13" runat="server"
                                                                    ValidationExpression="\d+" ErrorMessage="Only numbers are allowed" ControlToValidate="TextBox_NoOfRetryRestart">*</asp:RegularExpressionValidator>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                Number of retry for Executing DateTime Sync Schedule
                                                            </td>
                                                            <td>
                                                                <asp:TextBox CssClass="textbox" ID="TextBox_NoOfRetryDateTimeSync" runat="server"></asp:TextBox>
                                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator16" runat="server" ControlToValidate="TextBox_NoOfRetryDateTimeSync"
                                                                    ErrorMessage="Please enter number of retry for datetime sync schedule">*</asp:RequiredFieldValidator>
                                                                <asp:RegularExpressionValidator ID="RegularExpressionValidator14" runat="server"
                                                                    ValidationExpression="\d+" ErrorMessage="Only numbers are allowed" ControlToValidate="TextBox_NoOfRetryDateTimeSync">*</asp:RegularExpressionValidator>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </fieldset>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </ComponentArt:PageView>
                    <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView_iCam">
                        <table>
                            <tr>
                                <td>
                                    DashBoard Refresh Interval
                                </td>
                                <td>
                                    <asp:TextBox CssClass="textbox" ID="TextBox_DashBoardRefreshInterval" runat="server"></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator8" runat="server" ValidationExpression="\d+"
                                        ErrorMessage="Only numbers are allowed" ControlToValidate="TextBox_DashBoardRefreshInterval">*</asp:RegularExpressionValidator>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2" style="height: 34px" valign="middle">
                                    <span style="color: Navy; font-weight: bold;">Alert Monitoring Dashboard </span>
                                    &nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Display Data
                                </td>
                                <td>
                                    <asp:DropDownList ID="DropDown_AlertMonitoring" runat="server" CssClass="dropDown">
                                        <asp:ListItem Text="Without any filtering" Value="-1"></asp:ListItem>
                                        <asp:ListItem Text="Only For Current Day" Value="0"></asp:ListItem>
                                        <asp:ListItem Text="Older than 1 day" Value="1"></asp:ListItem>
                                        <asp:ListItem Text="Older than 2 days" Value="2"></asp:ListItem>
                                        <asp:ListItem Text="Older than 3 days" Value="3"></asp:ListItem>
                                        <asp:ListItem Text="Older than 4 days" Value="4"></asp:ListItem>
                                        <asp:ListItem Text="Older than 5 days" Value="5"></asp:ListItem>
                                        <asp:ListItem Text="Older than 6 days" Value="6"></asp:ListItem>
                                        <asp:ListItem Text="Older than 7 days" Value="7"></asp:ListItem>
                                        <asp:ListItem Text="Older than 8 days" Value="8"></asp:ListItem>
                                        <asp:ListItem Text="Older than 9 days" Value="9"></asp:ListItem>
                                        <asp:ListItem Text="Older than 10 days" Value="10"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2" style="height: 34px" valign="middle">
                                    <span style="color: Navy; font-weight: bold;">FTP Monitoring Dashboard</span>&nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Display Data older than
                                </td>
                                <td>
                                    <asp:DropDownList ID="DropDownList_FTPDashBoard" runat="server" CssClass="dropDown">
                                        <asp:ListItem Text="Without any filtering" Value="-1"></asp:ListItem>
                                        <asp:ListItem Text="Only For Current Day" Value="0"></asp:ListItem>
                                        <asp:ListItem Text="Older than 1 day" Value="1"></asp:ListItem>
                                        <asp:ListItem Text="Older than 2 days" Value="2"></asp:ListItem>
                                        <asp:ListItem Text="Older than 3 days" Value="3"></asp:ListItem>
                                        <asp:ListItem Text="Older than 4 days" Value="4"></asp:ListItem>
                                        <asp:ListItem Text="Older than 5 days" Value="5"></asp:ListItem>
                                        <asp:ListItem Text="Older than 6 days" Value="6"></asp:ListItem>
                                        <asp:ListItem Text="Older than 7 days" Value="7"></asp:ListItem>
                                        <asp:ListItem Text="Older than 8 days" Value="8"></asp:ListItem>
                                        <asp:ListItem Text="Older than 9 days" Value="9"></asp:ListItem>
                                        <asp:ListItem Text="Older than 10 days" Value="10"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2" style="height: 34px" valign="middle">
                                    <span style="color: Navy; font-weight: bold;">Task Status Monitoring Dashboard</span>&nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Display Data
                                </td>
                                <td>
                                    <asp:DropDownList ID="DropDownList_TaskMonitoring" runat="server" CssClass="dropDown">
                                        <asp:ListItem Text="Without any filtering" Value="-1"></asp:ListItem>
                                        <asp:ListItem Text="Only For Current Day" Value="0"></asp:ListItem>
                                        <asp:ListItem Text="Older than 1 day" Value="1"></asp:ListItem>
                                        <asp:ListItem Text="Older than 2 days" Value="2"></asp:ListItem>
                                        <asp:ListItem Text="Older than 3 days" Value="3"></asp:ListItem>
                                        <asp:ListItem Text="Older than 4 days" Value="4"></asp:ListItem>
                                        <asp:ListItem Text="Older than 5 days" Value="5"></asp:ListItem>
                                        <asp:ListItem Text="Older than 6 days" Value="6"></asp:ListItem>
                                        <asp:ListItem Text="Older than 7 days" Value="7"></asp:ListItem>
                                        <asp:ListItem Text="Older than 8 days" Value="8"></asp:ListItem>
                                        <asp:ListItem Text="Older than 9 days" Value="9"></asp:ListItem>
                                        <asp:ListItem Text="Older than 10 days" Value="10"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2" style="height: 34px" valign="middle">
                                    <span style="color: Navy; font-weight: bold;">Cash Order Monitoring Dashboard</span>&nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Display Data
                                </td>
                                <td>
                                    <asp:DropDownList ID="DropDownList_CashOrderMonitoring" runat="server" CssClass="dropDown">
                                        <asp:ListItem Text="Without any filtering" Value="-1"></asp:ListItem>
                                        <asp:ListItem Text="Only For Current Day" Value="0"></asp:ListItem>
                                        <asp:ListItem Text="Older than 1 day" Value="1"></asp:ListItem>
                                        <asp:ListItem Text="Older than 2 days" Value="2"></asp:ListItem>
                                        <asp:ListItem Text="Older than 3 days" Value="3"></asp:ListItem>
                                        <asp:ListItem Text="Older than 4 days" Value="4"></asp:ListItem>
                                        <asp:ListItem Text="Older than 5 days" Value="5"></asp:ListItem>
                                        <asp:ListItem Text="Older than 6 days" Value="6"></asp:ListItem>
                                        <asp:ListItem Text="Older than 7 days" Value="7"></asp:ListItem>
                                        <asp:ListItem Text="Older than 8 days" Value="8"></asp:ListItem>
                                        <asp:ListItem Text="Older than 9 days" Value="9"></asp:ListItem>
                                        <asp:ListItem Text="Older than 10 days" Value="10"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                        </table>
                    </ComponentArt:PageView>
                    <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView1">
                        <table>
                            <tr>
                                <td colspan="2" style="height: 34px" valign="middle">
                                    <span style="color: Navy; font-weight: bold;">Configuration For Data Archiving </span>
                                    &nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Archive/Purge Data Older Than
                                </td>
                                <td>
                                    <asp:DropDownList ID="DropDownList_MonthsToArchive" runat="server" CssClass="dropDown">
                                        <asp:ListItem Value="3">3 Months</asp:ListItem>
                                        <asp:ListItem Value="4">4 Months</asp:ListItem>
                                        <asp:ListItem Value="5">5 Months</asp:ListItem>
                                        <asp:ListItem Value="6">6 Months</asp:ListItem>
                                        <asp:ListItem Value="7">7 Months</asp:ListItem>
                                        <asp:ListItem Value="8">8 Months</asp:ListItem>
                                        <asp:ListItem Value="9">9 Months</asp:ListItem>
                                        <asp:ListItem Value="10">10 Months</asp:ListItem>
                                        <asp:ListItem Value="11">11 Months</asp:ListItem>
                                        <asp:ListItem Value="12">1 Year</asp:ListItem>
                                        <asp:ListItem Value="18">1.5 Years</asp:ListItem>
                                        <asp:ListItem Value="24">2 Years</asp:ListItem>
                                        <asp:ListItem Value="36">3  Years</asp:ListItem>
                                        <asp:ListItem Value="-1">Never</asp:ListItem>
                                    </asp:DropDownList>
                                    Months
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Purge Database
                                </td>
                                <td>
                                    <asp:CheckBox ID="CheckBox_PurgeDatabase" runat="server" AutoPostBack="true" OnCheckedChanged="CheckBox_PurgeDatabase_CheckChanged" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Server
                                </td>
                                <td>
                                    <asp:TextBox CssClass="textbox" ID="TextBox_DataArchiveServer" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Database
                                </td>
                                <td>
                                    <asp:TextBox CssClass="textbox" ID="TextBox_DataArchiveDatabase" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Username
                                </td>
                                <td>
                                    <asp:TextBox CssClass="textbox" ID="TextBox_DataArchiveUsername" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Password
                                </td>
                                <td>
                                    <asp:TextBox TextMode="Password" CssClass="textbox" ID="TextBox_DataArchivePwd" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                        </table>
                    </ComponentArt:PageView>
                    <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView2">
                        <table>
                            <tr>
                                <td colspan="2" style="height: 34px" valign="middle">
                                    <span style="color: Navy; font-weight: bold;">Services Management </span>&nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <b>Service</b>
                                </td>
                                <td>
                                    <b>Status</b>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    1.Currency Management Server
                                </td>
                                <td>
                                    <asp:Literal ID="Literal_CurrencyMngServer_Status" runat="server"></asp:Literal>
                                </td>
                                <td>
                                    <asp:Button ID="Button_CurrencyMngServer_Start" Text="Start" runat="server" CssClass="button"
                                        OnClick="Button_Click" />
                                    <asp:Button ID="Button_CurrencyMngServer_Stop" Text="Stop" runat="server" CssClass="button"
                                        OnClick="Button_Click" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    2.Currency Parser
                                </td>
                                <td>
                                    <asp:Literal ID="Literal_CurrencyParser_Status" runat="server"></asp:Literal>
                                </td>
                                <td>
                                    <asp:Button ID="Button_CurrencyParser_Start" Text="Start" runat="server" CssClass="button"
                                        OnClick="Button_Click" />
                                    <asp:Button ID="Button_CurrencyParser_Stop" Text="Stop" runat="server" CssClass="button"
                                        OnClick="Button_Click" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    3.Currency Cut Over
                                </td>
                                <td>
                                    <asp:Literal ID="Literal_CurrencyCutOver_Status" runat="server"></asp:Literal>
                                </td>
                                <td>
                                    <asp:Button ID="Button_CurrencyCutOver_Start" Text="Start" runat="server" CssClass="button"
                                        OnClick="Button_Click" />
                                    <asp:Button ID="Button_CurrencyCutOver_Stop" Text="Stop" runat="server" CssClass="button"
                                        OnClick="Button_Click" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    4.Alert Dispatcher
                                </td>
                                <td>
                                    <asp:Literal ID="Literal_CCMSAlertDispatcher_Status" runat="server"></asp:Literal>
                                </td>
                                <td>
                                    <asp:Button ID="Button_CCMSAlertDispatcher_Start" Text="Start" runat="server" CssClass="button"
                                        OnClick="Button_Click" />
                                    <asp:Button ID="Button_CCMSAlertDispatcher_Stop" Text="Stop" runat="server" CssClass="button"
                                        OnClick="Button_Click" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    5.Order Downloader
                                </td>
                                <td>
                                    <asp:Literal ID="Literal_CCMSOrderDownloader_Status" runat="server"></asp:Literal>
                                </td>
                                <td>
                                    <asp:Button ID="Button_CCMSOrderDownloader_Start" Text="Start" runat="server" CssClass="button"
                                        OnClick="Button_Click" />
                                    <asp:Button ID="Button_CCMSOrderDownloader_Stop" Text="Stop" runat="server" CssClass="button"
                                        OnClick="Button_Click" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    6.Transaction Monitor
                                </td>
                                <td>
                                    <asp:Literal ID="Literal_CCMSTransactionMonitor_Status" runat="server"></asp:Literal>
                                </td>
                                <td>
                                    <asp:Button ID="Button_CCMSTransactionMonitor_Start" Text="Start" runat="server"
                                        CssClass="button" OnClick="Button_Click" />
                                    <asp:Button ID="Button_CCMSTransactionMonitor_Stop" Text="Stop" runat="server" CssClass="button"
                                        OnClick="Button_Click" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    7.Monitor
                                </td>
                                <td>
                                    <asp:Literal ID="Literal_CCMSMonitor_Status" runat="server"></asp:Literal>
                                </td>
                                <td>
                                    <asp:Button ID="Button_CCMSMonitor_Start" Text="Start" runat="server" CssClass="button"
                                        OnClick="Button_Click" />
                                    <asp:Button ID="Button_CCMSMonitor_Stop" Text="Stop" runat="server" CssClass="button"
                                        OnClick="Button_Click" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    8.Report Schedular
                                </td>
                                <td>
                                    <asp:Literal ID="Literal_CurrencyReportSchedular_Status" runat="server"></asp:Literal>
                                </td>
                                <td>
                                    <asp:Button ID="Button_CurrencyReportSchedular_Start" Text="Start" runat="server"
                                        CssClass="button" OnClick="Button_Click" />
                                    <asp:Button ID="Button_CurrencyReportSchedular_Stop" Text="Stop" runat="server" CssClass="button"
                                        OnClick="Button_Click" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    9.Vault Manager
                                </td>
                                <td>
                                    <asp:Literal ID="Literal_CCMSVaultManager_Status" runat="server"></asp:Literal>
                                </td>
                                <td>
                                    <asp:Button ID="Button_CCMSVaultManager_Start" Text="Start" runat="server"
                                        CssClass="button" OnClick="Button_Click" />
                                    <asp:Button ID="Button_CCMSVaultManager_Stop" Text="Stop" runat="server" CssClass="button"
                                        OnClick="Button_Click" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    10.EJ Parser
                                </td>
                                <td>
                                    <asp:Literal ID="Literal_CCMSEJParser_Status" runat="server"></asp:Literal>
                                </td>
                                <td>
                                    <asp:Button ID="Button_CCMSEJParser_Start" Text="Start" runat="server" CssClass="button"
                                        OnClick="Button_Click" />
                                    <asp:Button ID="Button_CCMSEJParser_Stop" Text="Stop" runat="server" CssClass="button"
                                        OnClick="Button_Click" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    11.Daily Feed Generator
                                </td>
                                <td>
                                    <asp:Literal ID="Literal_DailyFeedGenerator_Status" runat="server"></asp:Literal>
                                </td>
                                <td>
                                    <asp:Button ID="Button_DailyFeedGenerator_Start" Text="Start" runat="server" CssClass="button"
                                        OnClick="Button_Click" />
                                    <asp:Button ID="Button_DailyFeedGenerator_Stop" Text="Stop" runat="server" CssClass="button"
                                        OnClick="Button_Click" />
                                </td>
                            </tr>
                        </table>
                    </ComponentArt:PageView>
                </ComponentArt:MultiPage>
            </td>
        </tr>
        <tr>
            <td colspan="4" style="padding-top: 20px" align="center">
                &nbsp;<input id="Button_ImportADUsers" runat="server" type="button" value="Import Active Directory User"
                    onclick="window.location ='../admin/LoginToActiveDir.aspx'" class="button" style="width: 183px;
                    visibility: hidden" />
                &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&nbsp;
                &nbsp;
                <asp:Button ID="Button_FIT" runat="server" Text="Update FIT Table" class="button"
                    OnClick="Button_FIT_Click" />
                <asp:Button ID="Button_Save" runat="server" Text="Save" OnClick="Button_Save_Click"
                    CssClass="button" Width="73px" />
                <input id="Button_Cancel" type="button" value="Cancel" onclick="window.location ='../admin/administration.aspx'"
                    class="button" />
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
