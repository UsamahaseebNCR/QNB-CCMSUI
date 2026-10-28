<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AssignATMsToADUsers.aspx.cs"
    Inherits="Avanza.iSuite.UI.AssignATMsToADUsers" ValidateRequest="false" Title="iDistribute - Add Edit User" %>

<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
        <link href="../content/treeStyle.css" rel="stylesheet" />
    <script type="text/javascript">

        function stopRKey(evt) {
            var evt = (evt) ? evt : ((event) ? event : null);
            var node = (evt.target) ? evt.target : ((evt.srcElement) ? evt.srcElement : null);
            if ((evt.keyCode == 13) && (node.type == "text")) { return false; }
        }
        document.onkeypress = stopRKey;

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

        function SelectNode(id)
        { UserATMsTreeView.selectNodeById(id); }

    </script>
    <link href="../content/iSuite.css" rel="stylesheet" />
</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)'
    onload="setPageURL('admin/AssignATMsToADUsers.aspx')">
    <form id="form1" runat="server">
    <table style="width: 100%; padding-left: 10px;" cellpadding="0" cellspacing="0" border="0"
        class="MainBox" id="TABLE1">
        <tr>
            <td class="PageHeading" align="left" colspan="2">
                <%=Request.Params["fromCSV"] == null ? "Assign ATMs and Groups to Active Directory Users" : "Assign ATMs and Groups to Users"%>
            </td>
        </tr>
        <tr>
            <td class="SearchHeadPaneGradient" colspan="2">
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <div id="notificationArea" style="text-align: center">
                </div>
            </td>
        </tr>
        <tr>
            <td class="ValidationText" align="center" colspan="2">
                <asp:Literal ID="Literal_error" runat="server" EnableViewState="False"></asp:Literal>
                <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
            </td>
        </tr>
        <tr>
            <td style="width: 1765px">
                <%=Request.Params["fromCSV"] == null ? "Selected Active Directory Users" : "Selected CSV Users"%>
            </td>
            <td>
                <asp:ListBox ID="ListBox_ADUsers" runat="server" Width="50%"></asp:ListBox>
            </td>
        </tr>
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
                        <td>
                            Manager
                        </td>
                        <td>
                            <asp:DropDownList ID="DropDownList_Manager" runat="server" CssClass="dropDown">
                            </asp:DropDownList>
                        </td>
                    </tr>
        <tr style="background-color: InfoBackground">
            <td style="width: 1765px">
                <asp:Label ID="Label_UserGroups" Text="User Group(s) " runat="server" Width="182px"></asp:Label>
            </td>
            <td>
                <asp:CheckBoxList ID="CheckBoxList_UserGroups" runat="Server" RepeatColumns="2" RepeatDirection="Horizontal">
                </asp:CheckBoxList>
            </td>
        </tr>
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
                        <td class="TreeSearch" valign="top">
                                                    <table cellpadding="5" cellspacing="0" border="0" width="100%">
                                                        <tr>
                                                            <td>
                                                                <input id="textbox_search" name="textbox_search" type="text" class="textboxNormal"
                                                                    accesskey="s" onkeyup="SearchTree1(event,this)" />
                                                            </td>
                                                            <td style="width: 100%" align="left">
                                                                <img alt="Search" src="../images/findText.gif" style="border-width: 0px" 
                                                                onclick ="imageClicked(this);"/>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                        <div id="searchDiv1" style="opacity: .8; filter: alpha(opacity=80); display: none;
        background-color: InfoBackground; height: 100px; width: 149px; border-right: orange thin solid;
        border-top: orange thin solid; z-index: 101; overflow: auto; border-left: orange thin solid;
        border-bottom: orange thin solid; position: absolute; top: 300px; left: 196px;" />
                        <asp:Literal ID="literal_jsArray1" runat="server" EnableViewState="false" />
                        <td style="width: 100%; vertical-align:top; overflow:auto;height:300px;">
                <%--<div style="width: 200px; height: 300px; overflow: auto;">--%>
                <ComponentArt:TreeView ID="UserATMsTreeView" EnableViewState="true" runat="server"
                    CollapseTransition="None" ContentLoadingImageUrl="spinner.gif" CssClass="TreeViewMargin"
                    CutNodeCssClass="CutTreeNode" DefaultImageHeight="16" DefaultImageWidth="16"
                    DefaultMarginImageHeight="20" DefaultMarginImageWidth="24" DisplayMargin="true"
                    DragAndDropEnabled="true" ExpandTransition="None" HoverNodeCssClass="HoverTreeNode"
                    ImagesBaseUrl="../images/" KeyboardCutCopyPasteEnabled="true" KeyboardEnabled="true"
                    LineImageHeight="20" LineImagesFolderUrl="../images/lines/" LineImageWidth="19"
                    LoadingFeedbackText="Loading ATMs" MarginWidth="24" MultipleSelectedNodeCssClass="MultipleSelectedTreeNode"
                    MultipleSelectEnabled="true" NodeCssClass="TreeNode" NodeEditCssClass="NodeEdit"
                    NodeEditingEnabled="true" NodeLabelPadding="3" SelectedNodeCssClass="SelectedTreeNode"
                    ShowLines="true" Height="91%" AllowTextSelection="True" AutoScroll="true" CollapseNodeOnSelect="false"
                    ExpandNodeOnSelect="false">
                    <ClientEvents>
                        <NodeCheckChange EventHandler="check" />
                    </ClientEvents>
                </ComponentArt:TreeView>
                <%--</div>--%>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Button ID="Button_SaveUpdateUser" runat="server" Text="Save & Load Next User"
                    OnClick="Button_SaveUpdate_Click" CssClass="button" Width="148px" />
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
