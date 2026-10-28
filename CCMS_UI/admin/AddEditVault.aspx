<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddEditVault.aspx.cs" Inherits="CCMSUI.admin.AddEditVault" %>

<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Add Edit Vault</title>
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
    <script type="text/javascript">
        function setCheck(node, selected) {
            var childary = node.get_nodes();

            for (var i = 0; i < childary.get_length(); i++) {
                //if (childary.getNode(i).get_id().substring(0, 1) == 'r') {
                childary.getNode(i).set_checked(selected);
                setCheck(childary.getNode(i), selected)
                //}
                //else
                //  childary.getNode(i).set_checked(selected);
            }
        }

        window.parent.window.contentPage = '';

        function check(sender, eventArgs) {
            UserATMsTreeView.beginUpdate();
            setCheck(eventArgs.get_node(), eventArgs.get_node().get_checked());
            UserATMsTreeView.endUpdate();
        }
    </script>
    <script type="text/javascript">
        window.parent.window.contentPage = '';          
        
        
    </script>
    <link href="../content/iSuite.css" rel="stylesheet" />
</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)'
    onload="hideProgressBar();">
    <form id="form1" runat="server">
    <table cellpadding="0" cellspacing="0" border="0" class="MainBox" width="100%">
        <tr>
            <td class="PageHeading" align="left" colspan="2" style="height: 25px">
                <%=Request.QueryString["id"] == null?"Add a New Vault ":"Edit Vault"%>
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
        <tr style="height: 20px">
            <td colspan="2" style="color: red;" align="center" valign="top">
                <asp:Literal ID="Literal_error" runat="server" EnableViewState="False"></asp:Literal>
            </td>
        </tr>
        <tr>
            <td >
                <table id="inner" border="0" cellpadding="0" cellspacing="0" width="50%" style="padding-left: 10px">
                    <tr>
                        <td style="width: 15%">
                            Title <span style="color: Red">*</span>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_Title" runat="server" CssClass="textbox"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="requiredfieldvalidator1" runat="server" ControlToValidate="TextBox_Title"
                                ErrorMessage="Please enter Title" Text="*"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Organization <span style="color: Red">*</span>
                        </td>
                        <td>
                            <asp:DropDownList ID="DropDownList_Organization" runat="server" Width="150px" CssClass="textbox"
                                AutoPostBack="True" OnSelectedIndexChanged="DropDownList_Organization_SelectedIndexChanged">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Note Set Type <span style="color: Red">*</span>
                        </td>
                        <td>
                            <asp:DropDownList ID="DropDownList_NoteSetType" runat="server" Width="150px" CssClass="textbox"
                                AutoPostBack="True" OnSelectedIndexChanged="DropDownList_NoteSetType_SelectedIndexChanged">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            CIT <span style="color: Red">*</span>
                        </td>
                        <td>
                            <asp:DropDownList ID="DropDownList_CIT" runat="server" Width="150px" CssClass="textbox">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Branch
                        </td>
                        <td>
                            <asp:CheckBox ID="CheckBox_Branch" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td style="vertical-align: top">
                            Description
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_Description" runat="server" CssClass="textbox" TextMode="MultiLine"
                                Height="29px" Width="483px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 88px" valign="top">
                            <asp:Label ID="Label_UserATMs" Text="User ATM(s) " runat="server"></asp:Label>
                        </td>
                        <td style="width: 100%;">
                            <div style="height: 200px; background-color: InfoBackground">
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
                            </div>
                        </td>
                    </tr>
                    
                    <tr>
                        <td colspan="2">
                            <fieldset>
                                <legend><b>Vault Denomination Details</b> </legend>
                                <asp:GridView ID="GridView_VaultNoteDetail" runat="server" Width="100%" AutoGenerateColumns="false"
                                    OnRowDataBound="GridView_VaultNoteDetail_RowDataBound">
                                    <RowStyle HorizontalAlign="Center" />
                                    <AlternatingRowStyle BackColor="#eff0ef" />
                                    <Columns>
                                        <asp:BoundField DataField="type" HeaderText="Denomination Type" HeaderStyle-ForeColor="black">
                                            <HeaderStyle CssClass="HeadingCell" />
                                        </asp:BoundField>
                                        <asp:TemplateField>
                                            <HeaderStyle CssClass="HeadingCell" />
                                            <HeaderTemplate>
                                                Emergency Cash
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:TextBox ID="TextBox_EmergencyCash" runat="server" CssClass="textbox" Width="120px"></asp:TextBox>
                                                <asp:RegularExpressionValidator ID="regularexpressionvalidator_EmergencyCash" runat="server"
                                                    ControlToValidate="TextBox_EmergencyCash" ErrorMessage="Only Numbers are allowed"
                                                    ValidationExpression="[0-9]*" Text="*"></asp:RegularExpressionValidator>
                                                <asp:RequiredFieldValidator ID="req_EmergencyCash" ControlToValidate="TextBox_EmergencyCash"
                                                    runat="server" ErrorMessage="Please enter Emergency Cash">*</asp:RequiredFieldValidator>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <HeaderStyle CssClass="HeadingCell" />
                                            <HeaderTemplate>
                                                Threshold Cash
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:TextBox ID="TextBox_ThresholdCash" runat="server" CssClass="textbox" Width="120px"></asp:TextBox>
                                                <asp:RegularExpressionValidator ID="regularexpressionvalidator_ThresholdCash" runat="server"
                                                    ControlToValidate="TextBox_ThresholdCash" ErrorMessage="Only Numbers are allowed"
                                                    ValidationExpression="[0-9]*" Text="*"></asp:RegularExpressionValidator>
                                                <asp:RequiredFieldValidator ID="req_ThresholdCash" ControlToValidate="TextBox_ThresholdCash"
                                                    runat="server" ErrorMessage="Please enter Threshold Cash">*</asp:RequiredFieldValidator>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="vault_note_type_id"></asp:BoundField>
                                    </Columns>
                                </asp:GridView>
                            </fieldset>
                        </td>
                    </tr>
                    
                    
                    <%--<tr style="background-color: #eff0ef;">
                        <td>
                            Region <span style="color: Red">*</span>
                        </td>
                        <td>
                            <asp:DropDownList ID="DropDownList_Region" runat="server" Width="150px" CssClass="textbox">
                            <asp:ListItem Text="Select" Value="Select"></asp:ListItem>
                            </asp:DropDownList>
                            <asp:CompareValidator ID="CompareValidator_Region" runat="server" 
                             ControlToValidate="DropDownList_Region" Operator="NotEqual" Type="String"
                              ValueToCompare="Select" ErrorMessage ="Please select region">*</asp:CompareValidator>
                        </td>
                    </tr>--%>
                    
                    
                    <tr>
                        <td colspan="2" style="font-weight: bold; padding-top: 30px;">
                            <asp:Button ID="Button_SaveUpdateVault" runat="server" Text="Save" OnClick="Button_SaveUpdateVault_Click"
                                CssClass="button" Width="60px" />&nbsp;
                            <input id="Button_Cancel" type="button" value="Cancel" onclick="window.location ='vault.aspx'"
                                class="button" />
                        </td>
                    </tr>
                </table>
            </td>
            
        </tr>
    </table>
    <br />
    <br />
    </form>
</body>
</html>
