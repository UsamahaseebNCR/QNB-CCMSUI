<%@ page language="C#" autoeventwireup="true" codebehind="AddEditUser.aspx.cs" inherits="Avanza.iSuite.UI.AddEditUser"
    validaterequest="false" title="iDistribute - Add Edit User" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ register assembly="ComponentArt.Web.UI" namespace="ComponentArt.Web.UI" tagprefix="ComponentArt" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="../content/shortcut.js" type="text/javascript" language="javascript"></script>
    <link href="../content/iSuite.css" rel="stylesheet" />
    <link href="../content/treeStyle.css" rel="stylesheet" />
    <script type="text/javascript">

        function stopRKey(evt) {
            var evt = (evt) ? evt : ((event) ? event : null);
            var node = (evt.target) ? evt.target : ((evt.srcElement) ? evt.srcElement : null);
            if ((evt.keyCode == 13) && (node.type == "text")) { return false; }
        }
        document.onkeypress = stopRKey;

        function setDirtyFlagToUpdatePasswordInDb() {
            document.getElementById('hidden_dirtyFlagForUserPassword').value = 1;
        }

        function setCheck(node, selected) {
            var childary = node.get_nodes();

            for (var i = 0; i < childary.get_length(); i++) {
                if (childary.getNode(i).get_id().substring(0, 1) == 'r') {
                    childary.getNode(i).set_checked(selected);
                    setCheck(childary.getNode(i), selected)
                }
                else
                    childary.getNode(i).set_checked(selected);
            }
        }

        window.parent.window.contentPage = '';

        function check(sender, eventArgs) {
            debugger
            UserATMsTreeView.beginUpdate();
            setCheck(eventArgs.get_node(), eventArgs.get_node().get_checked());
            UserATMsTreeView.endUpdate();
        }

        function SearchTree1(e, textbox) {
            debugger
            var isSearchOnLocation = false;
            var div = document.getElementById('searchDiv1');
            var keyCode = window.event ? event.keyCode : e.which;

            if (keyCode == 13) {
                var item = div.firstChild;
                for (var i = 0; i < selectionIndex; i++)
                    item = item.nextSibling;
                textbox.value = '';
                selectionIndex = -1;
                MakeNodeSelection(item.attributes.getNamedItem('nodeId').value);
            }
            else if (keyCode == 27) {
                textbox.value = '';
                selectionIndex = -1;
            }
            else if (keyCode == 38) {
                count1--;
                if (count1 % 8 == 0) {
                    div.scrollTop -= 100;

                }
                if (selectionIndex == -1)
                    return;
                var item = div.firstChild;
                for (var i = 0; i < selectionIndex; i++)
                    item = item.nextSibling;
                item.style.backgroundColor = 'InfoBackground';
                selectionIndex--;
                if (selectionIndex > -1)
                    item.previousSibling.style.backgroundColor = 'gray';
                return;
            }
            else if (keyCode == 40) {
                //         alert('dd');

                //setTimeout('document.getElementById("'+id+'").scro llTop=5000;', 50);
                // alert(div.ength);
                count++;
                if (count % 8 == 0) {
                    div.scrollTop += 100;

                }
                //alert(count);

                if (!div.firstChild)
                    return;
                var item = div.firstChild;
                for (var i = 0; i < selectionIndex; i++)
                    item = item.nextSibling;
                if (selectionIndex == -1) {
                    item.style.backgroundColor = 'gray';
                    selectionIndex++;
                    return;
                }
                item.style.backgroundColor = 'InfoBackground';
                if (item.nextSibling) {
                    item.nextSibling.style.backgroundColor = 'gray';
                    selectionIndex++;
                }
                else
                    item.style.backgroundColor = 'gray';
                return;
            }

            if (textbox.value.length == 0) {
                div.style.display = 'none';
                selectionIndex = -1;
            }
            else {

                var innerHTML = '';
                var input = textbox.value.toLowerCase(); //.lower();

                if (!isSearchOnLocation) {
                    for (var i = 0; i < ary1.length; i++)
                        if (ary1[i][1].toLowerCase().indexOf(input) > -1)
                            innerHTML += '<div  nodeId="' + ary1[i][0] + '"  onclick="MakeNodeSelection(\'' + ary1[i][0] + '\');">' + ary1[i][1] + '</div>';
                }
                else {
                    for (var i = 0; i < aryLocation1.length; i++)
                        if (aryLocation1[i][1].toLowerCase().indexOf(input) > -1)
                            innerHTML += '<div nodeId="' + aryLocation1[i][0] + '"  onclick="MakeNodeSelection(\'' + aryLocation1[i][0] + '\');">' + aryLocation1[i][1] + '</div>';
                }

                div.innerHTML = innerHTML;
                //alert(innerHTML);
                div.style.display = 'block';
            }
            // }catch(e){alert(e.message);}
        }
        function MakeNodeSelection(id) {
            document.getElementById('searchDiv1').style.display = 'none';
            document.getElementById('searchDiv1').innerHTML = '';
            UserATMsTreeView.selectNodeById(id);
        }

        function SelectNode(id) { UserATMsTreeView.selectNodeById(id); }
    </script>
</head>

<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)'
    onload="hideProgressBar();">

    <form id="form1" runat="server">
        <div id="UserDiv" runat="server"></div>
        <input type="hidden" id="hidden_dirtyFlagForUserPassword" runat="server" />
        <table style="width: 100%;" cellpadding="0" cellspacing="0" border="0" class="MainBox"
            id="TABLE1">
            <tr>
                <td class="PageHeading" align="left" colspan="2">
                    <%=Request.Params["u"]==null?"Add User":"Edit User" %>
                </td>
            </tr>
            <tr>
                <td class="SearchHeadPaneGradient" colspan="2"></td>
            </tr>
            <tr style="height: 20px">
                <td class="ValidationText" align="center" colspan="2">
                    <asp:Literal ID="Literal_error" runat="server" EnableViewState="False"></asp:Literal>
                    <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
                </td>
            </tr>
        </table>
        <table style="padding-left: 5px" cellpadding="0" cellspacing="0" border="0" width="100%">
            <tr>
                <td valign="top">
                    <table>
                        <tr>
                            <td style="width: 88px;">
                                <asp:Label ID="Label_User_Type" Text="User Type" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:DropDownList ID="DropDownList_User_Type" runat="server" AutoPostBack="True"
                                    CssClass="textbox" OnSelectedIndexChanged="DropDownList_User_Type_SelectedIndexChanged">
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 88px; font-weight: bold;">
                                <asp:Label ID="Label_Login_Id" Text="Login Id" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="TextBox_Login_Id" runat="server" CssClass="textboxNormal"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator_TextBox_Login_Id" runat="server"
                                    Text="*" ControlToValidate="TextBox_Login_Id" Display="Dynamic" ErrorMessage="A required field is missing">
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 88px">
                                <asp:Label ID="Label_Password" Text="Password" runat="server"></asp:Label>
                            </td>
                            <td>

                                <asp:TextBox ID="TextBox_Password" runat="server" TextMode="Password" CssClass="textboxNormal" oncopy="return false"></asp:TextBox>
                                <%--<asp:RequiredFieldValidator ID="RequiredFieldValidator_TextBox_Password" runat="server"
                                Text="*" ControlToValidate="TextBox_Password" Display="Dynamic" ErrorMessage="A required field is missing"></asp:RequiredFieldValidator>--%>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 88px">
                                <asp:Label ID="Label1" runat="server" Text="Confirm Password" Width="106px"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="TextBox_confirmpass" runat="server" CssClass="textboxNormal" TextMode="Password" oncopy="return false"></asp:TextBox>
                                <%--<asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="TextBox_Password"
                                ControlToValidate="TextBox_confirmpass" Display="Dynamic" ErrorMessage="The two passwords do not match">*</asp:CompareValidator>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="TextBox_confirmpass"
                                Display="Dynamic" ErrorMessage="A required field is missing">*</asp:RequiredFieldValidator>--%>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 88px">
                                <asp:Label ID="Label_Fullname" Text="Full Name" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="TextBox_Fullname" runat="server" CssClass="textboxNormal"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator_TextBox_Fullname" runat="server"
                                    Text="*" ControlToValidate="TextBox_Fullname" Display="Dynamic" ErrorMessage="A required field is missing">
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 88px">
                                <asp:Label ID="Label_Email" Text="Email" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="TextBox_Email" runat="server" CssClass="textboxNormal"></asp:TextBox>
                                <%--                            <asp:RegularExpressionValidator ID="RegularExpressionValidator_TextBox_Email" runat="server"
                                ControlToValidate="TextBox_Email" ErrorMessage="Email address is not in correct format"
                                ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*;?" Display="Dynamic">*</asp:RegularExpressionValidator>--%>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 88px">
                                <asp:Label ID="Label_Mobile" Text="Mobile" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="TextBox_MobileNo" runat="server" CssClass="textboxNormal"></asp:TextBox>

                            </td>
                        </tr>
                        <tr>
                            <td>Manager
                            </td>
                            <td>
                                <asp:DropDownList ID="DropDownList_Manager" runat="server" CssClass="dropDown">
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr style="background-color: InfoBackground">
                            <td style="width: 88px">
                                <asp:Label ID="Label_UserGroups" Text="User Group(s) " runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:CheckBoxList ID="CheckBoxList_UserGroups" runat="Server" RepeatColumns="2" RepeatDirection="Horizontal">
                                </asp:CheckBoxList>
                            </td>
                        </tr>
                        <%--  <tr>
                <td style="width: 88px; vertical-align: top">
                    <asp:Label ID="Label2" Text="CIT(s)" runat="server"></asp:Label>
                </td>
                <td>
                    <asp:ListBox ID="ListBox_CITs" runat="server" Width="125px"></asp:ListBox>
                </td>
            </tr>--%>
                        <tr id="Organization_Row" runat="server">
                            <td>
                                <asp:Label ID="Label3" Text="Organization" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:CheckBoxList ID="CheckBoxList_Organization" runat="Server" RepeatColumns="5"
                                    RepeatDirection="Horizontal">
                                </asp:CheckBoxList>
                                <asp:Button ID="Button_Load" runat="server" Text="Load Tree" OnClick="Button_Load_Click"
                                    CssClass="button" Width="80px" CausesValidation="false" />
                            </td>
                        </tr>
                        <tr id="CIT_Row" runat="server">
                            <td>
                                <asp:Label ID="Label2" Text="CIT" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:CheckBoxList ID="CheckBoxList_CIT" runat="Server" RepeatColumns="5" RepeatDirection="Horizontal">
                                </asp:CheckBoxList>
                                <asp:Button ID="Button_Load_CIT" runat="server" Text="Load Tree" OnClick="Button_Load_CIT_Click"
                                    CssClass="button" Width="80px" CausesValidation="false" />
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 88px" valign="top">
                                <asp:Label ID="Label_UserATMs" Text="User ATM(s) " runat="server"></asp:Label>
                            </td>
                            <td class="TreeSearch" height="2px" valign="top">
                                <table cellpadding="5" cellspacing="0" border="0" width="100%">
                                    <tr>
                                        <td>
                                            <input id="textbox_search" name="textbox_search" type="text" class="textboxNormal"
                                                accesskey="s" onkeyup="SearchTree1(event,this)" />
                                        </td>
                                        <td style="width: 100%" align="left">
                                            <img alt="Search" src="../images/findText.gif" style="border-width: 0px"
                                                onclick="imageClicked(this);" />
                                        <div id="searchDiv1" style="opacity: .8; filter: alpha(opacity=80); display: none; background-color: InfoBackground; height: 100px; width: 154px; border-right: orange thin solid; border-top: orange thin solid; z-index: 101; overflow: auto; border-left: orange thin solid; border-bottom: orange thin solid; position: absolute; top: 390px; left: 122px;" />
                                            <asp:Literal ID="literal_jsArray1" runat="server" EnableViewState="false" />
                                    
                                        </td>
                                       
                                    </tr>
                                    <tr>
                                         <td colspan="2" style="background-color:white">
                                            
                                             <componentart:treeview id="UserATMsTreeView" enableviewstate="true" runat="server"
                                                collapsetransition="None" contentloadingimageurl="spinner.gif" cssclass="TreeViewMargin"
                                                cutnodecssclass="CutTreeNode" defaultimageheight="16" defaultimagewidth="16"
                                                defaultmarginimageheight="20" defaultmarginimagewidth="24" displaymargin="true"
                                                draganddropenabled="true" expandtransition="None" hovernodecssclass="HoverTreeNode"
                                                imagesbaseurl="../images/" keyboardcutcopypasteenabled="true" keyboardenabled="true"
                                                lineimageheight="20" lineimagesfolderurl="../images/lines/" lineimagewidth="19"
                                                loadingfeedbacktext="Loading ATMs" marginwidth="24" multipleselectednodecssclass="MultipleSelectedTreeNode"
                                                multipleselectenabled="true" nodecssclass="TreeNode" nodeeditcssclass="NodeEdit"
                                                nodeeditingenabled="true" nodelabelpadding="3" selectednodecssclass="SelectedTreeNode"
                                                showlines="true" height="91%" allowtextselection="True" autoscroll="true" collapsenodeonselect="false"
                                                expandnodeonselect="false">
                                                <clientevents>
                                                    <nodecheckchange eventhandler="check" />
                                                </clientevents>
                                            </componentart:treeview>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                           
                        </tr>
                        <tr>
                            <td style="width: 167px; vertical-align: top" align="center" colspan="2">
                                <asp:Button ID="Button_SaveUpdateUser" runat="server" Text="Save" OnClick="Button_SaveUpdate_Click"
                                    CssClass="button" Width="57px" />
                                <asp:Button ID="Button_Cancel" runat="server" Visible="true" Text="Cancel"
                                    CssClass="button" Width="57px" OnClick="Button_Cancel_Click" CausesValidation="false" />
                                <asp:Button ID="Button_ApproveUser" runat="server" Visible="false" Text="Approve"
                                    CssClass="button" Width="57px" OnClick="Button_ApproveUser_Click" />
                                &nbsp;
                            <asp:Button ID="Button_DeclineUser" runat="server" Visible="false" Text="Decline"
                                CssClass="button" Width="57px" OnClick="Button_DeclineUser_Click" />
                            </td>
                        </tr>
                    </table>
                </td>
                <td valign="top">
                    <fieldset>
                        <legend><b>Configure Alert Notifications</b></legend>
                        <table>
                            <tr>
                                <td>
                                    <div>
                                        <input type="button" id="button_checkAll" value="Check All" class="button" onclick="setCheckATMUsers(true,'checkbox_list')" />
                                        <input type="button" id="button_uncheckAll" value="Uncheck All" class="button" onclick="setCheckATMUsers(false,'checkbox_list')" />
                                    </div>
                                    <div style="overflow: scroll; height: 300px; width: 300px; background-color: InfoBackground">
                                        <asp:CheckBoxList ID="checkbox_list" runat="server" DataTextField="AlertTypeName"
                                            DataValueField="AlertTypeId" />
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </fieldset>
                </td>
            </tr>
        </table>
    </form>
</body>
</html>
