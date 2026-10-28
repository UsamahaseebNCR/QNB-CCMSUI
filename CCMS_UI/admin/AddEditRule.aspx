<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddEditRule.aspx.cs" Inherits="Avanza.iSuite.UI.AddEditRule" %>

<%@ Register Assembly="obout_ColorPicker" Namespace="OboutInc.ColorPicker" TagPrefix="obout" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="../content/ISUITE.css" rel="stylesheet" />
    <title>CCMS - AddEditRule</title>
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)'
    onload="hideProgressBar();">
    <form id="form1" runat="server">
    <div>
        <table style="width: 100%" cellspacing="0" cellpadding="0" border="0" class="mainbox">
            <tr>
                <td class="PageHeading" align="left">
                    Add Edit Rule
                </td>
            </tr>
            <tr>
                <td class="ValidationText" align="center">
                    <asp:Literal ID="Literal_error" runat="server" EnableViewState="False"></asp:Literal>
                </td>
            </tr>
            <tr>
                <td class="ConfirmationText" align="center">
                    <%=Session[SessionVars.notice.ToString()]%>
                    <%Session[SessionVars.notice.ToString()] = "";%>
                    <asp:Literal ID="Literal_ok" runat="server" EnableViewState="False"></asp:Literal>
                </td>
            </tr>
            <tr>
                <td align="left">
                    &nbsp; &nbsp; &nbsp;&nbsp;
                </td>
            </tr>
            <tr>
                <td>
                    <table >
                        <tr>
                            <td valign="top" >
                                Rule Friendly Name
                            </td>
                            <td>
                                <asp:TextBox ID="TextBox_RuleName" runat="server" CssClass="textbox" Width="220px"   ></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td >
                                Criteria
                            </td>
                            <td>
                                <asp:TextBox ID="TextBox_Criteria" runat="server" CssClass="textbox" Width="220px" ></asp:TextBox>
                                &nbsp;
                                <asp:Button ID="Button_Add" runat="server" Text="Add" CssClass="button" 
                                    OnClick="Button_Add_Click" />
                                &nbsp;
                                <asp:Button ID="Button_Remove" runat="server" Text="Remove" CssClass="button"
                                    OnClick="Button_Remove_Click" />
                            </td>
                        </tr>
                        <tr>
                        <td>&nbsp;</td>
                            <td  >
                                <asp:ListBox ID="ListBox_Items" runat="server" Width="220px" CssClass="textbox" >
                                </asp:ListBox>
                            </td>
                        </tr>
                        <tr>
                            <td valign="top" >
                                Color
                            </td>
                            <td>
                                <asp:TextBox ID="TextBox_RuleColor" Style="vertical-align: middle;" runat="server" 
                                    CssClass="textbox" />
                                &nbsp;
                                <obout:ColorPicker ID="ColorPicker_Rule" runat="server" TargetId="TextBox_RuleColor">
                                </obout:ColorPicker>
                            </td>
                        </tr>
                         <tr>
                            <td valign="top" >
                                Active
                            </td>
                            <td>
                                <asp:CheckBox ID="CheckBox_Active" runat="server" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:Button ID="Button_Save" runat="server" Text="Save" CssClass="button" OnClick="Button_Save_Click" />
                    <input type="button" value="Cancel" class="button" onclick="window.location='RulesList.aspx'" />
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
