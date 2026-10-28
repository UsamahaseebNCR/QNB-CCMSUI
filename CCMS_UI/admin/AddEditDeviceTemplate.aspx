<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddEditDeviceTemplate.aspx.cs"
    ValidateRequest="false" Inherits="CCMSUI.admin.AddEditDeviceTemplate" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link rel="stylesheet" href="../content/iSuite.css" />
    <title>CCMS - Add/Edit Device Template</title>
    <script src="../content/shortcut.js" type="text/javascript" language="javascript"></script>
    <script src="../Scripts/jquery-1.7.1.min.js" type="text/javascript"></script>
    <script src="../Scripts/jquery-ui-1.8.16.min.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        window.parent.window.contentPage = '';

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
    onload="hideProgressBar()">
    <form id="form1" runat="server">
    <div>
        <table cellspacing="0" cellpadding="0" width="100%">
            <tr>
                <th class="PageHeading" align="left">
                     <%=Request.QueryString["device_template_id"] == null?"Add Device Template ":"Edit Device Template"%>
                </th>
            </tr>
            <tr>
                <td class="ValidationTextClass">
                    <asp:Literal ID="Literal_error" runat="server" EnableViewState="False"></asp:Literal>
                    <asp:ValidationSummary ID="ValidationSummary_ATM" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="ConfirmationText">
                    <%=Session[SessionVars.notice.ToString()]%>
                    <%Session[SessionVars.notice.ToString()] = "";%>
                    <asp:Literal ID="Literal_ok" runat="server" EnableViewState="False"></asp:Literal>
                </td>
            </tr>
            <tr style="width: 100%">
                <td class="SearchFiltersPaneClasstbl">
                    <table style="width: 100%; margin-top:10px">
                        <tr>
                            <td style="width: 15%;">
                                Name <span style="color:red">*</span>
                            </td>
                            <td style="width: 85%">
                                <asp:TextBox ID="TextBox_Type" runat="server" CssClass="textbox" MaxLength="50"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator_Type" runat="server" ControlToValidate="TextBox_Type"
                                    ErrorMessage="Please Enter Name">*</asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td valign="top">
                                Description
                            </td>
                            <td>
                                <asp:TextBox ID="TextBox_Description" runat="server" CssClass="textbox"
                                    MaxLength="250" TextMode="MultiLine" Width="250" Height="50"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Status
                            </td>
                            <td>
                                <asp:DropDownList ID="DropDownList_status" runat="server" CssClass="dropDown">
                                    <asp:ListItem Text="Active" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="Inactive" Value="0"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Vendor Type
                            </td>
                            <td>
                                <asp:DropDownList ID="DropDownList_VendorType" runat="server" CssClass="dropDown"
                                    AutoPostBack="true" OnSelectedIndexChanged="DropDownList_VendorType_SelectedIndexChanged">
                                    <asp:ListItem Text="--- Select ---" Value="-1"></asp:ListItem>
                                    <asp:ListItem Text="NCR" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="Wincor" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="Diebold" Value="2"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToValidate="DropDownList_VendorType"
                                    Operator="GreaterThan" ValueToCompare="-1" ErrorMessage="You must select some Vendor Type">*
                                </asp:CompareValidator>
                            </td>
                        </tr>
                        <tr>
                            <td style="vertical-align: top;">
                                Devices
                            </td>
                            <td>
                                
                                <a href="#" id="button_checkAll" class="HyperlinkClass" onclick="CheckAll();">Check
                                    All</a>&nbsp;&nbsp; <a href="#" id="button_uncheckAll" class="HyperlinkClass" onclick="UncheckAll();">
                                        Uncheck All</a>&nbsp;&nbsp;
                                <div id="Div_Devices" runat="server" class="DivClass" style="width: 80%; font-weight: normal;
                                    border: solid 1px #d3d3d3; max-height: 300px; overflow:auto; background-color:InfoBackground">
                                    <asp:TreeView ID="TreeView_Device" runat="server" EnableViewState="true" ShowLines="false"
                                        ShowCheckBoxes="All" ShowExpandCollapse="true" ExpandDepth="10" onclick="Client_OnTreeNodeChecked();"
                                        Height="100%" Width="100%">
                                    </asp:TreeView>
                                </div>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td class="SearchButtonsPaneClass" colspan="2">
                    <asp:Button ID="Button_save" runat="server" CssClass="button" Text="Save" OnClick="Button_save_Click"
                        CausesValidation="true" />&nbsp;&nbsp;
                    <input id="Button_cancel" type="button" value="Back" onclick="window.location='../ccms/DeviceTemplates.aspx'"
                        class="button" causesvalidation="false" />
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
