<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddEditCIT.aspx.cs" Inherits="CCMSUI.admin.AddEditCIT" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">

    <title>Add Edit CIT</title>
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
            regionTreeView.beginUpdate();
            setCheck(eventArgs.get_node(), eventArgs.get_node().get_checked());
            regionTreeView.endUpdate();
        }
    </script>
    <script type="text/javascript">
        window.parent.window.contentPage = '';          
        
        
    </script>
    <link href="../content/iSuite.css" rel="stylesheet" />
</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)' onload="hideProgressBar();">
    <form id="form1" runat="server">
    <table cellpadding="0" cellspacing="0" border="0" class="MainBox" width="100%" >
        <tr>
            <td class="PageHeading" align="left" colspan="2" style="height: 25px">
                <%=Request.QueryString["id"] == null?"Add a New CIT ":"Edit CIT"%>
                
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
            <td>
                <table id="inner" border="0" cellpadding="0" cellspacing="0" width ="100%" style="padding-left:10px">
                    <tr style="background-color: #eff0ef;" >
                        <td style="width:15%">
                            Name <span style="color: Red">*</span>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_Name" runat="server" CssClass="textbox"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="requiredfieldvalidator1" runat="server" ControlToValidate="TextBox_Name"
                                ErrorMessage="Please enter Organization Name" Text="*"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Code <span style="color: Red">*</span>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_Code" runat="server" CssClass="textbox"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="requiredfieldvalidator_Title" runat="server" ControlToValidate="TextBox_Code"
                                ErrorMessage="Please enter Code." Text="*"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr style="background-color: #eff0ef">
                        <td>
                            Email <span style="color: Red">*</span>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_Email" runat="server" CssClass="textbox"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="requiredfieldvalidator2" runat="server" ControlToValidate="TextBox_Email"
                                ErrorMessage="Please enter EmailD." Text="*"></asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator_TextBox_Email" runat="server" ControlToValidate="TextBox_Email" ErrorMessage="Email address is not in correct format" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" Display="Dynamic">*</asp:RegularExpressionValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Order Dispatch Time
                        </td>
                        <td>
                            Hours:&nbsp;<asp:DropDownList ID="DropDownList_DispatchHour" runat="server" CssClass="dropDown">
                            </asp:DropDownList>
                            Minutes:&nbsp;<asp:DropDownList ID="DropDownList_DispatchMin" CssClass="dropDown"
                                runat="server" />
                        </td>
                    </tr>
                    <tr style="background-color: #eff0ef">
                        <td>
                            Active
                        </td>
                        <td>
                            <asp:RadioButtonList ID="RadioButton_Active" runat="server" RepeatDirection="Horizontal">
                                <asp:ListItem Value="1">Active</asp:ListItem>
                                <asp:ListItem Value="0">Inactive</asp:ListItem>
                            </asp:RadioButtonList>
                        </td>
                    </tr>
                    
                    <tr>
                        <td colspan="2" style="font-weight: bold; padding-top: 30px;">
                            <asp:Button ID="Button_SaveUpdateCIT" runat="server" Text="Save" OnClick="Button_SaveUpdateCIT_Click"
                                CssClass="button" Width="60px" />&nbsp;
                            <input id="Button_Cancel" type="button" value="Cancel" onclick="window.location ='cit.aspx'"
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
