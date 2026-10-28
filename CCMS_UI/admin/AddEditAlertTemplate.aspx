<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddEditAlertTemplate.aspx.cs"
    Inherits="Avanza.iSuite.UI.AddEditAlertTemplate" EnableEventValidation="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link rel="stylesheet" href="../content/iSuite.css" />
    <title>CCMS - Edit Alert Template</title>
   
    <script src="../content/shortcut.js" type="text/javascript" language="javascript"></script>
    <script language="javascript" type="text/javascript">
        window.parent.window.contentPage = '';

        function ShowErrorMessage(errorMessage) {
            document.getElementById('Label_Dialog_ErrorMessage').innerText = errorMessage;
        }

        function RefreshDialog() {
            SetTabIndex('TextBox_Level0_Duration_Days', 1, true);
            SetTabIndex('TextBox_Level0_Duration_Hours', 2, false);
            SetTabIndex('TextBox_Level0_Duration_Mins', 3, false);
            SetTabIndex('TextBox_Level0_Notifications', 4, false);
            SetTabIndex('TextBox_Level0_ReminderInterval_Days', 5, false);
            SetTabIndex('TextBox_Level0_ReminderInterval_Hours', 6, false);
            SetTabIndex('TextBox_Level0_ReminderInterval_Mins', 7, false);
            SetTabIndex('CheckBox_Level0_Email', 8, false);
            SetTabIndex('CheckBox_Level0_Sms', 9, false);

            SetTabIndex('TextBox_Level1_Duration_Days', 10, false);
            SetTabIndex('TextBox_Level1_Duration_Hours', 11, false);
            SetTabIndex('TextBox_Level1_Duration_Mins', 12, false);
            SetTabIndex('TextBox_Level1_Notifications', 13, false);
            SetTabIndex('TextBox_Level1_ReminderInterval_Days', 14, false);
            SetTabIndex('TextBox_Level1_ReminderInterval_Hours', 15, false);
            SetTabIndex('TextBox_Level1_ReminderInterval_Mins', 16, false);
            SetTabIndex('CheckBox_Level1_Email', 17, false);
            SetTabIndex('CheckBox_Level1_Sms', 18, false);

            SetTabIndex('TextBox_Level2_Duration_Days', 19, false);
            SetTabIndex('TextBox_Level2_Duration_Hours', 20, false);
            SetTabIndex('TextBox_Level2_Duration_Mins', 21, false);
            SetTabIndex('TextBox_Level2_Notifications', 22, false);
            SetTabIndex('TextBox_Level2_ReminderInterval_Days', 23, false);
            SetTabIndex('TextBox_Level2_ReminderInterval_Hours', 24, false);
            SetTabIndex('TextBox_Level2_ReminderInterval_Mins', 25, false);
            SetTabIndex('CheckBox_Level2_Email', 26, false);
            SetTabIndex('CheckBox_Level2_Sms', 27, false);

            SetTabIndex('TextBox_Level3_Duration_Days', 28, false);
            SetTabIndex('TextBox_Level3_Duration_Hours', 29, false);
            SetTabIndex('TextBox_Level3_Duration_Mins', 30, false);
            SetTabIndex('TextBox_Level3_Notifications', 31, false);
            SetTabIndex('TextBox_Level3_ReminderInterval_Days', 32, false);
            SetTabIndex('TextBox_Level3_ReminderInterval_Hours', 33, false);
            SetTabIndex('TextBox_Level3_ReminderInterval_Mins', 34, false);
            SetTabIndex('CheckBox_Level3_Email', 35, false);
            SetTabIndex('CheckBox_Level3_Sms', 36, false);

            SetTabIndex('TextBox_Level4_Duration_Days', 37, false);
            SetTabIndex('TextBox_Level4_Duration_Hours', 38, false);
            SetTabIndex('TextBox_Level4_Duration_Mins', 39, false);
            SetTabIndex('TextBox_Level4_Notifications', 40, false);
            SetTabIndex('TextBox_Level4_ReminderInterval_Days', 41, false);
            SetTabIndex('TextBox_Level4_ReminderInterval_Hours', 42, false);
            SetTabIndex('TextBox_Level4_ReminderInterval_Mins', 43, false);
            SetTabIndex('CheckBox_Level4_Email', 44, false);
            SetTabIndex('CheckBox_Level4_Sms', 45, false);

            SetTabIndex('TextBox_Level5_Duration_Days', 46, false);
            SetTabIndex('TextBox_Level5_Duration_Hours', 47, false);
            SetTabIndex('TextBox_Level5_Duration_Mins', 48, false);
            SetTabIndex('TextBox_Level5_Notifications', 49, false);
            SetTabIndex('TextBox_Level5_ReminderInterval_Days', 50, false);
            SetTabIndex('TextBox_Level5_ReminderInterval_Hours', 51, false);
            SetTabIndex('TextBox_Level5_ReminderInterval_Mins', 52, false);
            SetTabIndex('CheckBox_Level5_Email', 53, false);
            SetTabIndex('CheckBox_Level5_Sms', 54, false);

            SetTabIndex('Button_Dialog_Save', 55, false);
            SetTabIndex('Button_Dialog_Back', 56, false);
        }

        function ShowDialogEditAlertEscalation(severityId) {
            Dialog_EditAlertEscalation.Show();
            RefreshDialog();
            ShowErrorMessage('');

            Callback_LoadEditAlertEscalationDialog.Callback(severityId);
        }

        function Button_Dialog_Save_Click() {
            var severityId = window.form1.TextBox_Dialog_SeverityId.value;

            var level0_duration_days = window.form1.TextBox_Level0_Duration_Days.value;
            var level0_duration_hours = window.form1.TextBox_Level0_Duration_Hours.value;
            var level0_duration_mins = window.form1.TextBox_Level0_Duration_Mins.value;
            var level1_duration_days = window.form1.TextBox_Level1_Duration_Days.value;
            var level1_duration_hours = window.form1.TextBox_Level1_Duration_Hours.value;
            var level1_duration_mins = window.form1.TextBox_Level1_Duration_Mins.value;
            var level2_duration_days = window.form1.TextBox_Level2_Duration_Days.value;
            var level2_duration_hours = window.form1.TextBox_Level2_Duration_Hours.value;
            var level2_duration_mins = window.form1.TextBox_Level2_Duration_Mins.value;
            var level3_duration_days = window.form1.TextBox_Level3_Duration_Days.value;
            var level3_duration_hours = window.form1.TextBox_Level3_Duration_Hours.value;
            var level3_duration_mins = window.form1.TextBox_Level3_Duration_Mins.value;
            var level4_duration_days = window.form1.TextBox_Level4_Duration_Days.value;
            var level4_duration_hours = window.form1.TextBox_Level4_Duration_Hours.value;
            var level4_duration_mins = window.form1.TextBox_Level4_Duration_Mins.value;

            var level1_notifications = window.form1.TextBox_Level1_Notifications.value;
            var level2_notifications = window.form1.TextBox_Level2_Notifications.value;
            var level3_notifications = window.form1.TextBox_Level3_Notifications.value;
            var level4_notifications = window.form1.TextBox_Level4_Notifications.value;
            var level5_notifications = window.form1.TextBox_Level5_Notifications.value;

            var level1_reminder_days = window.form1.TextBox_Level1_ReminderInterval_Days.value;
            var level1_reminder_hours = window.form1.TextBox_Level1_ReminderInterval_Hours.value;
            var level1_reminder_mins = window.form1.TextBox_Level1_ReminderInterval_Mins.value;
            var level2_reminder_days = window.form1.TextBox_Level2_ReminderInterval_Days.value;
            var level2_reminder_hours = window.form1.TextBox_Level2_ReminderInterval_Hours.value;
            var level2_reminder_mins = window.form1.TextBox_Level2_ReminderInterval_Mins.value;
            var level3_reminder_days = window.form1.TextBox_Level3_ReminderInterval_Days.value;
            var level3_reminder_hours = window.form1.TextBox_Level3_ReminderInterval_Hours.value;
            var level3_reminder_mins = window.form1.TextBox_Level3_ReminderInterval_Mins.value;
            var level4_reminder_days = window.form1.TextBox_Level4_ReminderInterval_Days.value;
            var level4_reminder_hours = window.form1.TextBox_Level4_ReminderInterval_Hours.value;
            var level4_reminder_mins = window.form1.TextBox_Level4_ReminderInterval_Mins.value;
            var level5_reminder_days = window.form1.TextBox_Level5_ReminderInterval_Days.value;
            var level5_reminder_hours = window.form1.TextBox_Level5_ReminderInterval_Hours.value;
            var level5_reminder_mins = window.form1.TextBox_Level5_ReminderInterval_Mins.value;

            var level1_email_enabled = window.form1.CheckBox_Level1_Email.checked;
            var level2_email_enabled = window.form1.CheckBox_Level2_Email.checked;
            var level3_email_enabled = window.form1.CheckBox_Level3_Email.checked;
            var level4_email_enabled = window.form1.CheckBox_Level4_Email.checked;
            var level5_email_enabled = window.form1.CheckBox_Level5_Email.checked;

            var level1_sms_enabled = window.form1.CheckBox_Level1_Sms.checked;
            var level2_sms_enabled = window.form1.CheckBox_Level2_Sms.checked;
            var level3_sms_enabled = window.form1.CheckBox_Level3_Sms.checked;
            var level4_sms_enabled = window.form1.CheckBox_Level4_Sms.checked;
            var level5_sms_enabled = window.form1.CheckBox_Level5_Sms.checked;

            if (level0_duration_days < 0 || level0_duration_days > 365 || level1_duration_days < 0 || level1_duration_days > 365 || level2_duration_days < 0 || level2_duration_days > 365 || level3_duration_days < 0 || level3_duration_days > 365 || level4_duration_days < 0 || level4_duration_days > 365 || level1_reminder_days < 0 || level1_reminder_days > 365 || level2_reminder_days < 0 || level2_reminder_days > 365 || level3_reminder_days < 0 || level3_reminder_days > 365 || level4_reminder_days < 0 || level4_reminder_days > 365 || level5_reminder_days < 0 || level5_reminder_days > 365)
                ShowErrorMessage('Days should be between 0 and 365.');
            else if (level1_notifications < 0 || level1_notifications > 255 || level2_notifications < 0 || level2_notifications > 255 || level3_notifications < 0 || level3_notifications > 255 || level4_notifications < 0 || level4_notifications > 255 || level5_notifications < 0 || level5_notifications > 255)
                ShowErrorMessage('Reminders should be between 0 and 255.');
            else
                Callback_EditAlertEscalation.Callback(severityId, level0_duration_days, level0_duration_hours, level0_duration_mins, level1_duration_days, level1_duration_hours, level1_duration_mins, level2_duration_days, level2_duration_hours, level2_duration_mins, level3_duration_days, level3_duration_hours, level3_duration_mins, level4_duration_days, level4_duration_hours, level4_duration_mins, level1_notifications, level2_notifications, level3_notifications, level4_notifications, level5_notifications, level1_reminder_days, level1_reminder_hours, level1_reminder_mins, level2_reminder_days, level2_reminder_hours, level2_reminder_mins, level3_reminder_days, level3_reminder_hours, level3_reminder_mins, level4_reminder_days, level4_reminder_hours, level4_reminder_mins, level5_reminder_days, level5_reminder_hours, level5_reminder_mins, level1_email_enabled, level2_email_enabled, level3_email_enabled, level4_email_enabled, level5_email_enabled, level1_sms_enabled, level2_sms_enabled, level3_sms_enabled, level4_sms_enabled, level5_sms_enabled);
        }

        $(function () {
            var treeView1 = $('#<%= TreeView_Device.ClientID %>');
            var treeNodes = treeView1.find('div[id$=Nodes]');
            var treeImages = treeView1.find('img').not('img[alt=\'\']');

            $('#aToggleNodes').click(function () {
                if (this.innerHTML == 'Expand All') {
                    treeNodes.css({ 'display': 'block' });
                    treeImages.attr('src', '../images/lines/minus.gif')
                    this.innerHTML = 'Collapse All';
                }
                else {
                    treeNodes.css({ 'display': 'none' });
                    treeImages.attr('src', '../images/lines/plus.gif')
                    this.innerHTML = 'Expand All';
                }

                return false;
            });
        });

        function CheckAll() {
            var inputs = document.forms[0].elements;

            for (var i = 0; i < inputs.length; i++)
                inputs[i].checked = true;
        }

        function UncheckAll() {
            var inputs = document.forms[0].elements;

            for (var i = 0; i < inputs.length; i++)
                inputs[i].checked = false;
        }

        function Client_OnTreeNodeChecked() {
            var obj = window.event.srcElement;
            var treeNodeFound = false;
            var checkedState;

            if (obj.tagName == "INPUT" && obj.type == "checkbox") {
                var treeNode = obj;
                checkedState = treeNode.checked;

                do {
                    obj = obj.parentElement;
                } while (obj.tagName != "TABLE")

                if (obj.rows[0].cells.length == 2) {
                    var parentTreeLevel = obj.rows[0].cells.length;
                    var parentTreeNode = obj.rows[0].cells[0];
                    var tables = obj.parentElement.getElementsByTagName("TABLE");
                    var numTables = tables.length

                    if (numTables >= 1) {
                        for (i = 0; i < numTables; i++) {
                            if (tables[i] == obj) {
                                treeNodeFound = true;
                                i++;

                                if (i == numTables)
                                    return;
                            }

                            if (treeNodeFound == true) {
                                var childTreeLevel = tables[i].rows[0].cells.length;

                                if (childTreeLevel > parentTreeLevel) {
                                    var cell = tables[i].rows[0].cells[childTreeLevel - 1];
                                    var inputs = cell.getElementsByTagName("INPUT");
                                    inputs[0].checked = checkedState;
                                }
                                else
                                    return;
                            }
                        }
                    }
                }
                else if (checkedState == true) {
                    var obj1 = obj;
                    obj = obj.parentElement;
                    var tables = obj.parentElement.getElementsByTagName("TABLE");
                    var numTables = tables.length;
                    var check = 0;

                    if (numTables >= 1) {
                        for (i = 0; i < numTables; i++) {
                            if (tables[i] == obj1) {
                                treeNodeFound = true;
                                while (tables[i].rows[0].cells.length != 2)
                                    i--;
                                if (i == numTables)
                                    return;
                            }
                            else
                                check++;

                            if (treeNodeFound == true) {
                                var cell = tables[i].rows[0].cells[1];
                                var inputs = cell.getElementsByTagName("INPUT");
                                inputs[0].checked = checkedState;
                                return;
                            }
                        }
                    }
                }
            }
        }
    </script>
</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)'
    onload="setPageURL('iwatch/AddEditAlertTemplate.aspx')">
    <form id="form1" runat="server">
    <div>
        <table style="width: 100%" cellspacing="0" cellpadding="0" border="0">
            <tr>
                <th class="PageHeading" align="left">
                    Define Device Alert Template
                </th>
            </tr>
            <tr>
                <td class="ValidationText">
                    <asp:Literal ID="Literal_error" runat="server" EnableViewState="False"></asp:Literal>
                    <asp:ValidationSummary ID="ValidationSummary_ATM" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="ConfirmationTextClass">
                    <%=Session[SessionVars.notice.ToString()]%>
                    <%Session[SessionVars.notice.ToString()] = "";%>
                    <asp:Literal ID="Literal_ok" runat="server" EnableViewState="False"></asp:Literal>
                </td>
            </tr>
            <tr style="width: 100%">
                <td class="SearchFiltersPaneClass">
                    <table width="100%">
                        <tr>
                            <td>
                                <table>
                                    <tr>
                                        <td style="width: 15%;">
                                            Name
                                        </td>
                                        <td style="width: 85%">
                                            <asp:TextBox ID="TextBox_Type" runat="server" CssClass="textbox"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator_Type" runat="server" ControlToValidate="TextBox_Type"
                                                ErrorMessage="Please Enter Name">*</asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td valign="top">
                                            Description
                                        </td>
                                        <td>
                                            <asp:TextBox ID="TextBox_Description" runat="server" CssClass="textbox" TextMode="MultiLine"
                                                Width="300" Height="100"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td valign="top">
                                            Devices
                                        </td>
                                        <td>
                                            <a href="#" id="aToggleNodes" class="HyperlinkClass">Collapse All</a>&nbsp;&nbsp;
                                            <a href="#" id="button_checkAll" class="HyperlinkClass" onclick="CheckAll();">Check
                                                All</a>&nbsp;&nbsp; <a href="#" id="button_uncheckAll" class="HyperlinkClass" onclick="UncheckAll();">
                                                    Uncheck All</a>&nbsp;&nbsp;
                                            <div id="Div_Devices" style="overflow: auto; height: 300px; background-color: InfoBackground">
                                                <asp:TreeView ID="TreeView_Device" runat="server" EnableViewState="true" ShowLines="false"
                                                    ShowCheckBoxes="All" ShowExpandCollapse="true" ExpandDepth="2" onclick="Client_OnTreeNodeChecked();"
                                                    Height="100%" Width="100%">
                                                </asp:TreeView>
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td valign="top">
                                <table width="100%">
                                    <tr>
                                        <td valign="top">
                                            Alert Escalation
                                        </td>
                                        <td>
                                            <asp:GridView ID="GridView_AlertEscalation" runat="server" AutoGenerateColumns="False"
                                                Width="60%" AllowPaging="False" PageSize="4">
                                                <EmptyDataRowStyle CssClass="GridEmptyRowClass" />
                                                <EmptyDataTemplate>
                                                    No data found!</EmptyDataTemplate>
                                                <Columns>
                                                    <asp:BoundField DataField="level_name" HeaderText="Level Name" ItemStyle-Width="85%"
                                                        HeaderStyle-CssClass="HeadingCell"></asp:BoundField>
                                                    <asp:TemplateField HeaderText="Edit" ItemStyle-Width="15%" HeaderStyle-CssClass="HeadingCell">
                                                        <ItemTemplate>
                                                            <img class="ImageButtonClass" alt="Click to Edit" onclick="ShowDialogEditAlertEscalation('<%# Eval("severity_id") %>')"
                                                                src="../images/Edit.gif" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                    <table style="width: 100%">
                        <tr>
                            <td class="SearchButtonsPaneClass" colspan="2">
                                <asp:Button ID="Button_Save" runat="server" Text="Save" CssClass="button" OnClick="Button_Save_Click" />&nbsp;&nbsp;
                                <input id="Button_back" runat="server" type="button" class="button" value="Back"
                                    causesvalidation="false" onclick="window.location='../iwatch/AlertTemplates.aspx'" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
    <div>
        <ComponentArt:Dialog ID="Dialog_EditAlertEscalation" runat="server" Modal="true"
            AllowDrag="False" AllowResize="True" ShowTransition="Dissolve" CloseTransition="Fade"
            TransitionDuration="500" Alignment="TopCentre" Width="95%"   HeaderCssClass="DialogClass" ContentCssClass="DialogContentClass">
            <Header>
                <table style="width: 100%;">
                    <tr>
                        <td style="width: 95%;">
                            Alert Escalation
                        </td>
                        <td style="width: 5%; text-align: right;">
                            <img class="ImageButtonClass" alt="Close" src="../images/dialogimages/close.gif"
                                onclick="Dialog_EditAlertEscalation.Close();document.body.scroll='no';" />
                        </td>
                    </tr>
                </table>
            </Header>
            <Content >
                
                <table style="width: 100%;" cellspacing="0">
                    <tr class="ValidationText">
                        <td colspan="4">
                            <label id="Label_Dialog_ErrorMessage">
                            </label>
                        </td>
                    </tr>
                    <tr style="height: 0px;">
                        <td>
                        </td>
                        <td colspan="4">
                            <div style="visibility: hidden;">
                                <asp:TextBox ID="TextBox_Dialog_SeverityId" runat="server" CssClass="textbox"></asp:TextBox>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Name
                        </td>
                        <td colspan="4">
                            <asp:Label ID="Label_Name" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <b>Level</b>
                        </td>
                        <td>
                            <b>Duration</b>
                        </td>
                        <td>
                            <b>Reminder</b>
                        </td>
                        <td>
                            <b>Reminder Interval</b>
                        </td>
                        <td>
                            <b>Adapter</b>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Level 0
                        </td>
                        <td>
                            Days
                            <asp:TextBox ID="TextBox_Level0_Duration_Days" runat="server" Width="30px" CssClass="textbox"
                                MaxLength="3">0</asp:TextBox>
                            Hrs
                            <asp:DropDownList ID="TextBox_Level0_Duration_Hours" runat="server" Width="40px"
                                CssClass="dropDown">
                            </asp:DropDownList>
                            Mins
                            <asp:DropDownList ID="TextBox_Level0_Duration_Mins" runat="server" Width="40px" CssClass="dropDown">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_Level0_Notifications" runat="server" Width="40px" CssClass="textbox"
                                MaxLength="3" Enabled="false">0</asp:TextBox>
                        </td>
                        <td>
                            Days
                            <asp:TextBox ID="TextBox_Level0_ReminderInterval_Days" runat="server" Width="30px"
                                CssClass="textbox" MaxLength="3" Enabled="false">0</asp:TextBox>
                            Hrs
                            <asp:DropDownList ID="TextBox_Level0_ReminderInterval_Hours" runat="server" Width="40px"
                                CssClass="dropDown" Enabled="false">
                            </asp:DropDownList>
                            Mins
                            <asp:DropDownList ID="TextBox_Level0_ReminderInterval_Mins" runat="server" Width="40px"
                                CssClass="dropDown" Enabled="false">
                            </asp:DropDownList>
                        </td>
                        <td style="border-right: solid 1px #696969;">
                            <asp:CheckBox ID="CheckBox_Level0_Email" runat="server" Text="Email" CssClass="CheckBoxClass"
                                Width="60px" Enabled="false" />
                            <asp:CheckBox ID="CheckBox_Level0_Sms" runat="server" Text="SMS" CssClass="CheckBoxClass"
                                Width="55px" Enabled="false" />
                        </td>
                    </tr>
                    <tr>
                        <td class="">
                            Level 1
                        </td>
                        <td>
                            Days
                            <asp:TextBox ID="TextBox_Level1_Duration_Days" runat="server" Width="30px" CssClass="textbox"
                                MaxLength="3">0</asp:TextBox>
                            Hrs
                            <asp:DropDownList ID="TextBox_Level1_Duration_Hours" runat="server" Width="40px"
                                CssClass="dropDown">
                            </asp:DropDownList>
                            Mins
                            <asp:DropDownList ID="TextBox_Level1_Duration_Mins" runat="server" Width="40px" CssClass="dropDown">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_Level1_Notifications" runat="server" Width="40px" CssClass="textbox"
                                MaxLength="3">0</asp:TextBox>
                        </td>
                        <td>
                            Days
                            <asp:TextBox ID="TextBox_Level1_ReminderInterval_Days" runat="server" Width="30px"
                                CssClass="textbox" MaxLength="3">0</asp:TextBox>
                            Hrs
                            <asp:DropDownList ID="TextBox_Level1_ReminderInterval_Hours" runat="server" Width="40px"
                                CssClass="dropDown">
                            </asp:DropDownList>
                            Mins
                            <asp:DropDownList ID="TextBox_Level1_ReminderInterval_Mins" runat="server" Width="40px"
                                CssClass="dropDown">
                            </asp:DropDownList>
                        </td>
                        <td style="border-right: solid 1px #696969;">
                            <asp:CheckBox ID="CheckBox_Level1_Email" runat="server" Text="Email" CssClass="CheckBoxClass"
                                Width="60px" />
                            <asp:CheckBox ID="CheckBox_Level1_Sms" runat="server" Text="SMS" CssClass="CheckBoxClass"
                                Width="55px" />
                        </td>
                    </tr>
                    <tr>
                        <td class="">
                            Level 2
                        </td>
                        <td>
                            Days
                            <asp:TextBox ID="TextBox_Level2_Duration_Days" runat="server" Width="30px" CssClass="textbox"
                                MaxLength="3">0</asp:TextBox>
                            Hrs
                            <asp:DropDownList ID="TextBox_Level2_Duration_Hours" runat="server" Width="40px"
                                CssClass="dropDown">
                            </asp:DropDownList>
                            Mins
                            <asp:DropDownList ID="TextBox_Level2_Duration_Mins" runat="server" Width="40px" CssClass="dropDown">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_Level2_Notifications" runat="server" Width="40px" CssClass="textbox"
                                MaxLength="3">0</asp:TextBox>
                        </td>
                        <td>
                            Days
                            <asp:TextBox ID="TextBox_Level2_ReminderInterval_Days" runat="server" Width="30px"
                                CssClass="textbox" MaxLength="3">0</asp:TextBox>
                            Hrs
                            <asp:DropDownList ID="TextBox_Level2_ReminderInterval_Hours" runat="server" Width="40px"
                                CssClass="dropDown">
                            </asp:DropDownList>
                            Mins
                            <asp:DropDownList ID="TextBox_Level2_ReminderInterval_Mins" runat="server" Width="40px"
                                CssClass="dropDown">
                            </asp:DropDownList>
                        </td>
                        <td style="border-right: solid 1px #696969;">
                            <asp:CheckBox ID="CheckBox_Level2_Email" runat="server" Text="Email" CssClass="CheckBoxClass"
                                Width="60px" />
                            <asp:CheckBox ID="CheckBox_Level2_Sms" runat="server" Text="SMS" CssClass="CheckBoxClass"
                                Width="55px" />
                        </td>
                    </tr>
                    <tr>
                        <td class="">
                            Level 3
                        </td>
                        <td>
                            Days
                            <asp:TextBox ID="TextBox_Level3_Duration_Days" runat="server" Width="30px" CssClass="textbox"
                                MaxLength="3">0</asp:TextBox>
                            Hrs
                            <asp:DropDownList ID="TextBox_Level3_Duration_Hours" runat="server" Width="40px"
                                CssClass="dropDown">
                            </asp:DropDownList>
                            Mins
                            <asp:DropDownList ID="TextBox_Level3_Duration_Mins" runat="server" Width="40px" CssClass="dropDown">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_Level3_Notifications" runat="server" Width="40px" CssClass="textbox"
                                MaxLength="3">0</asp:TextBox>
                        </td>
                        <td>
                            Days
                            <asp:TextBox ID="TextBox_Level3_ReminderInterval_Days" runat="server" Width="30px"
                                CssClass="textbox" MaxLength="3">0</asp:TextBox>
                            Hrs
                            <asp:DropDownList ID="TextBox_Level3_ReminderInterval_Hours" runat="server" Width="40px"
                                CssClass="dropDown">
                            </asp:DropDownList>
                            Mins
                            <asp:DropDownList ID="TextBox_Level3_ReminderInterval_Mins" runat="server" Width="40px"
                                CssClass="dropDown">
                            </asp:DropDownList>
                        </td>
                        <td style="border-right: solid 1px #696969;">
                            <asp:CheckBox ID="CheckBox_Level3_Email" runat="server" Text="Email" CssClass="CheckBoxClass"
                                Width="60px" />
                            <asp:CheckBox ID="CheckBox_Level3_Sms" runat="server" Text="SMS" CssClass="CheckBoxClass"
                                Width="55px" />
                        </td>
                    </tr>
                    <tr>
                        <td class="">
                            Level 4
                        </td>
                        <td>
                            Days
                            <asp:TextBox ID="TextBox_Level4_Duration_Days" runat="server" Width="30px" CssClass="textbox"
                                MaxLength="3">0</asp:TextBox>
                            Hrs
                            <asp:DropDownList ID="TextBox_Level4_Duration_Hours" runat="server" Width="40px"
                                CssClass="dropDown">
                            </asp:DropDownList>
                            Mins
                            <asp:DropDownList ID="TextBox_Level4_Duration_Mins" runat="server" Width="40px" CssClass="dropDown">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_Level4_Notifications" runat="server" Width="40px" CssClass="textbox"
                                MaxLength="3">0</asp:TextBox>
                        </td>
                        <td>
                            Days
                            <asp:TextBox ID="TextBox_Level4_ReminderInterval_Days" runat="server" Width="30px"
                                CssClass="textbox" MaxLength="3">0</asp:TextBox>
                            Hrs
                            <asp:DropDownList ID="TextBox_Level4_ReminderInterval_Hours" runat="server" Width="40px"
                                CssClass="dropDown">
                            </asp:DropDownList>
                            Mins
                            <asp:DropDownList ID="TextBox_Level4_ReminderInterval_Mins" runat="server" Width="40px"
                                CssClass="dropDown">
                            </asp:DropDownList>
                        </td>
                        <td style="border-right: solid 1px #696969;">
                            <asp:CheckBox ID="CheckBox_Level4_Email" runat="server" Text="Email" CssClass="CheckBoxClass"
                                Width="60px" />
                            <asp:CheckBox ID="CheckBox_Level4_Sms" runat="server" Text="SMS" CssClass="CheckBoxClass"
                                Width="55px" />
                        </td>
                    </tr>
                    <tr>
                        <td class="">
                            Level 5
                        </td>
                        <td>
                            Days
                            <asp:TextBox ID="TextBox_Level5_Duration_Days" runat="server" Width="30px" CssClass="textbox"
                                MaxLength="3" Enabled="false">0</asp:TextBox>
                            Hrs
                            <asp:DropDownList ID="TextBox_Level5_Duration_Hours" runat="server" Width="40px"
                                CssClass="dropDown" Enabled="false">
                            </asp:DropDownList>
                            Mins
                            <asp:DropDownList ID="TextBox_Level5_Duration_Mins" runat="server" Width="40px" CssClass="dropDown"
                                Enabled="false">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_Level5_Notifications" runat="server" Width="40px" CssClass="textbox"
                                MaxLength="3">0</asp:TextBox>
                        </td>
                        <td>
                            Days
                            <asp:TextBox ID="TextBox_Level5_ReminderInterval_Days" runat="server" Width="30px"
                                CssClass="textbox" MaxLength="3">0</asp:TextBox>
                            Hrs
                            <asp:DropDownList ID="TextBox_Level5_ReminderInterval_Hours" runat="server" Width="40px"
                                CssClass="dropDown">
                            </asp:DropDownList>
                            Mins
                            <asp:DropDownList ID="TextBox_Level5_ReminderInterval_Mins" runat="server" Width="40px"
                                CssClass="dropDown">
                            </asp:DropDownList>
                        </td>
                        <td style="border-right: solid 1px #696969;">
                            <asp:CheckBox ID="CheckBox_Level5_Email" runat="server" Text="Email" CssClass="CheckBoxClass"
                                Width="60px" />
                            <asp:CheckBox ID="CheckBox_Level5_Sms" runat="server" Text="SMS" CssClass="CheckBoxClass"
                                Width="55px" />
                        </td>
                    </tr>
                    <tr style="height: 10px;">
                    </tr>
                    <tr>
                        <td class="SearchButtonsPaneClass" colspan="5">
                            <input id="Button_Dialog_Save" runat="server" type="button" class="button" value="Save"
                                onclick="javascript:Button_Dialog_Save_Click()" />&nbsp;&nbsp;
                            <input id="Button_Dialog_Back" runat="server" type="button" class="button" value="Cancel"
                                onclick="Dialog_EditAlertEscalation.Close();document.body.scroll='no';" />
                        </td>
                    </tr>
                </table>
            </Content>
        </ComponentArt:Dialog>
    </div>
    <div>
        <ComponentArt:CallBack ID="Callback_LoadEditAlertEscalationDialog" runat="server"
            OnCallback="LoadEditAlertEscalationDialog" Height="0px" Width="0px">
        </ComponentArt:CallBack>
        <ComponentArt:CallBack ID="Callback_EditAlertEscalation" runat="server" OnCallback="EditAlertEscalation"
            Height="0px" Width="0px">
        </ComponentArt:CallBack>
    </div>
    </form>
</body>
</html>
