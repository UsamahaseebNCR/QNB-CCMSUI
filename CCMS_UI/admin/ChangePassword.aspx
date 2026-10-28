<%@ Page Language="C#" AutoEventWireup="true" Codebehind="ChangePassword.aspx.cs" ValidateRequest="true"
    Inherits="Avanza.iSuite.UI.ChangePassword" Title="iDistribute - Change your password" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
    <link href="../content/iSuite.css" rel="stylesheet" />
</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)' onload="hideProgressBar();">
    <form runat="server">
        <table width="100%" cellpadding="0" cellspacing="0" border="0">
            <tr>
                <td class="PageHeading" align="left">
                    Change your password
                </td>
            </tr>
            <tr>
                <td class="SearchHeadPaneGradient">
                </td>
            </tr>
            <tr>
                <td align="left">
                    <asp:ValidationSummary ID="ValidationSummary_ChangePassword" runat="server" ShowSummary="true" />
                </td>
            </tr>
          
        </table>
        
        <table width="100%" cellpadding="0" cellspacing="0" border="0" style="padding-left:10px;">
          <tr style="height: 20px">
            <td>
                <div  style="text-align:center;color:red">
                    <asp:Literal ID="Literal_error" runat="server" EnableViewState="false"></asp:Literal>
                 </div>
                  <div  style="text-align:center;color: navy">
                    <asp:Literal ID="Literal_Info" runat="server" EnableViewState="false"></asp:Literal>
                    </div>
                </td>
            </tr>
            <tr>
                <td>
                    <table width="100%" cellpadding="0" cellspacing="0" border="0">
                        <tr>
                            <td style="width: 239px">
                                <b>Change your password</b>
                            </td>
                            <td>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Current Password :
                            </td>
                            <td>
                                <asp:TextBox ID="TextBox_CurrentPassword" runat="server" CssClass="textbox" TextMode="Password"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator_CurrentPassword" runat="server"
                                    ControlToValidate="TextBox_CurrentPassword" Text="*" ErrorMessage="Please provide old password"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                New Password :
                            </td>
                            <td>
                                <asp:TextBox ID="TextBox_NewPassword" runat="server" CssClass="textbox" TextMode="Password"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator_NewPassword" runat="server"
                                    ControlToValidate="TextBox_NewPassword" Text="*" ErrorMessage="Please provide new password"></asp:RequiredFieldValidator>
                                <asp:CompareValidator ID="CompareValidator_NewPassword" runat="server" ControlToValidate="TextBox_NewPassword"
                                    Operator ="NotEqual"  Type="String" ControlToCompare="TextBox_CurrentPassword" Text="*" ErrorMessage="Old and new password cannot be same"></asp:CompareValidator>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Confirm Password :
                            </td>
                            <td>
                                <asp:TextBox ID="TextBox_ConfirmPassword" runat="server" CssClass="textbox" TextMode="Password"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server"
                                    ControlToValidate="TextBox_ConfirmPassword"  ErrorMessage="Please provide Confirm password">*</asp:RequiredFieldValidator>
                                <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToValidate="TextBox_ConfirmPassword"
                                    Operator ="Equal"  Type="String" ControlToCompare="TextBox_NewPassword"  ErrorMessage="New and Confirm password should be same.">*</asp:CompareValidator>
                            </td>
                        </tr>
                        <tr style=" padding-top : 20px">
                            <td style="width: 239px;">
                            </td>
                            <td align="left">
                                <asp:Button ID="Button_ChangePassword" runat="server" Text="Change" OnClick="Button_ChangePassword_Click"
                                    CssClass="button"  />
                                <input id="Button1" class="button" type="button" value="Cancel" onclick="history.back()" />
                                
                            </td>
                        </tr>
                       
                    </table>
                </td>
            </tr>
        </table>
    </form>
</body>
</html>
