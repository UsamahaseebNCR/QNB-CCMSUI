<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="SignIn.aspx.cs" Inherits="CCMSUI.SignIn"
    EnableViewState="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Cash And Currency Management System</title>
    <%--<script src="content/shortcut.js" type="text/javascript" language="javascript">
    </script>--%>
    <script type="text/javascript">

        window.moveTo(0, 0);
        window.resizeTo(screen.width, screen.height);


        function CheckSubmit(e) {
            if ((window.event ? event.keyCode : e.which) == 13)
                Submit();
            else
                document.getElementById('notification').innerHTML = '';
        }

        function Submit() {
            alert('postback');
            if (document.getElementById('TextBox_signin').value.length > 0) {
                document.getElementById('refresh').click();
            }
            else
                document.getElementById('notification').innerHTML = "<img src='images/icon_err.gif'></img>&nbsp;Please, provide User Id!";
        }

        function reset() {

            document.getElementById('TextBox_signin').value = '';
            document.getElementById('TextBox_password').value = '';
        }

        function setWidthAndHeight() {
            document.getElementById('screenWidth').value = window.screen.availWidth;
            document.getElementById('screenHeight').value = window.screen.availHeight;

        }
    </script>
    <style type="text/css">
        body
        {
            margin: 0px;
            padding: 0px;
            background: #EAE9ED;
            font-family: Arial, Helvetica, sans-serif;
            font-size: 12px;
        }
        .dashboardwrap
        {
            background: url(bg.jpg) top left no-repeat;
            height: 552px;
            position: absolute;
            top: 0px;
            width: 100%;
        }
        .loginbox
        {
            position: absolute;
            top: 25%;
            left: 35%;
            padding: 10px;
            background: #fff;
            -moz-border-radius: 10px;
            -webkit-border-radius: 10px;
            -khtml-border-radius: 10px;
            border-radius: 10px;
        }
        .logo
        {
            margin-bottom: 20px;
        }
        .bar
        {
            padding: 10px;
            height: auto;
            overflow: hidden;
        }
        .txtfld
        {
            height: auto;
            overflow: hidden;
            width: 150px;
        }
        .bar label
        {
            display: block;
            float: left;
            overflow: hidden;
        }
        .copyright
        {
            font-size: 10px;
            color: #666;
            margin: 10px 0px;
        }
    </style>
</head>
<body style="background-image: url(images/bg.png); background-repeat: no-repeat;
    margin: 0; overflow: hidden" onload="setWidthAndHeight();if (self != top) top.location = self.location"
    onload="Page_Load">
    <form id="form1" runat="server">
    <input type="hidden" id="screenWidth" runat="server" />
    <input type="hidden" id="screenHeight" runat="server" />
    <div class="dashboardwrap">
    </div>
    <div class="loginbox">
        <div class="logo">
            <img src="images/logo_currencyModule.png" /></div>
        <table border="0" style="width: 379px">
            <tr>
                <td>
                    <table border="0" width="370px">
                        <tr>
                            <td colspan="2" >
                                <div id="notification">
                                    <asp:Literal ID="Literal_ErrMsg" runat="server"></asp:Literal>
                                    <asp:ValidationSummary ID="ValidationSummary_LoginPage" runat="server" />
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td class="heading" colspan="2">
                                <asp:Label ID="Label_Info" runat="server" Text="Use your user id and password to login"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td align="right" style="width:150px">
                                Username:
                            </td>
                            <td>
                                <asp:TextBox ID="TextBox_signin" runat="server" CssClass="txtfld"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator_SignIn" runat="server" SetFocusOnError="true"
                                    ControlToValidate="TextBox_signin" Text="*" ErrorMessage="Please enter username"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td align="right">
                                <asp:Label ID="Label_Password" runat="server" Text="Password:"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="TextBox_password" TextMode="Password" CssClass="txtfld" runat="server"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" SetFocusOnError="true"
                                    ControlToValidate="TextBox_password" ErrorMessage="Please enter password" Text="*"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr runat="server" id="trConfirmPassword">
                            <td align="right">
                                <asp:Label ID="Label_ConfirmPassword" runat="server" Text="Confirm Password:"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="TextBox_ConfirmPassword" TextMode="Password" CssClass="txtfld" runat="server"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" align="right" style="padding-right: 30px">
                                <asp:ImageButton ID="ImageButton_Login" runat="server" ImageUrl="images/btn_signin.jpg"
                                    OnClick="ImageButton_SignIn_Click" />
                                <asp:ImageButton ID="ImageButton_Reset" runat="server" ImageUrl="images/btn_reset.jpg"
                                    OnClick="ImageButton_Reset_Click" CausesValidation="false" />
                                <asp:ImageButton ID="ImageButton_RequestChangePassword" runat="server" ImageUrl="images/key_info.png"
                                    OnClick="ImageButton_RequestChangePassword_Click" />
                                <%-- <img src="images/btn_signin.jpg" onclick="Submit()" alt="Click to SignIn"  />
                    <img src="images/btn_reset.jpg" onclick="reset()"  />--%>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        <%--  <div class="bar"><label>User</label>
    
    </div>
    <div class="bar"><label></label></div>--%>
        <%--<div class="bar" style="text-align:right">
    
    </div>--%>
        <div class="copyright">
            Copyright 2011-2015</div>
    </div>
    <%-- <div>
            <table style=" width: 1023px; height: 615px; " border="0">
                <tr>
                    <td style="width: 480px;">
                    </td>
                    <td>
                        
                        <div style="height:130px;">
                           <img src="images/logo.png" />
                        </div>
                        <table border="0"; style="font-family:Verdana">
                            <tr>
                                <td colspan="2"; style="font-size:small">
                        <div id="Error_msg" style="color: Red; text-align:right">
                            <asp:Literal runat="server" ID="err_msg" EnableViewState="false"  />
                        </div>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    User ID:
                                </td>
                                <td>
                                    
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    Password:
                                </td>
                                <td>
                                    
                                </td>
                            </tr>
                            <tr>
                                <td></td>
                            </tr>
                            <tr>
                                <td>
                                </td><td  align="right">
                                    <img src="images/btn_signin.jpg" onclick="Submit()" alt="Click to SignIn" />
                                    <img src="images/btn_reset.jpg" onclick ="reset()" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            <table border="0" style="background-color: #c6dce8; width: 100%; margin: 0; padding-left: 0">
                <tr>
                    <td>
                    </td>
                </tr>
            </table>
        </div>--%>
    <input type="button" runat="server" id="refresh" style="display: none" />
    </form>
</body>
</html>
