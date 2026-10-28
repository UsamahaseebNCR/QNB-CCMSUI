<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="SignIn.aspx.cs" Inherits="CCMSUI.SignIn"
    EnableViewState="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="content/styleSheets/bootstrap.min.css" rel="stylesheet">
    <link href="content/styleSheets/bootstrap-theme.min.css" rel="stylesheet">
    <script src="content/Scripts/aes.js" type="text/javascript"></script>
    <script src="content/Scripts/signin.js" type="text/javascript"></script>


    <title>Cash And Currency Management System</title>
    <%--<script src="content/shortcut.js" type="text/javascript" language="javascript">
    </script>--%>
    <script type="text/javascript">

        function trim10(str) {
            var whitespace = ' \n\r\t\f\x0b\xa0\u2000\u2001\u2002\u2003\u2004\u2005\u2006\u2007\u2008\u2009\u200a\u200b\u2028\u2029\u3000';
            for (var i = 0; i < str.length; i++) {
                if (whitespace.indexOf(str.charAt(i)) === -1) {
                    str = str.substring(i);
                    break;
                }
            }
            for (i = str.length - 1; i >= 0; i--) {
                if (whitespace.indexOf(str.charAt(i)) === -1) {
                    str = str.substring(0, i + 1);
                    break;
                }
            }
            return whitespace.indexOf(str.charAt(0)) === -1 ? str : '';
        }

        function SubmitsEncry() {
            //var txtpassword = document.getElementById("<%=TextBox_password.ClientID %>").value.trim();
            var txtpassword = trim10(document.getElementById("<%=TextBox_password.ClientID %>").value);
            document.getElementById("<%=TextBox_password.ClientID %>").value = generateId(txtpassword.length);
            document.getElementById("<%=xpass.ClientID %>").value = encryp(txtpassword);;
            //alert(encryp(txtpassword));    
            var sid = window.localStorage.getItem('sid');
            if (sid == null) {
                sid = "sid" + Math.random().toString(16).slice(2);
                window.localStorage.setItem("sid", sid);
                //console.log('localStorage:' + window.localStorage.getItem('id'));
                //alert(window.localStorage.getItem('id'));
            }
            document.getElementById("<%=_t.ClientID %>").value = sid;

        }

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
        body {
            margin: 0px;
            padding: 0px;
            background: #EAE9ED;
            font-family: Arial, Helvetica, sans-serif;
            font-size: 12px;
        }

        .dashboardwrap {
            background: url(bg.jpg) top left no-repeat;
            height: 552px;
            position: absolute;
            top: 0px;
            width: 100%;
        }

        .loginbox {
            position: absolute;
            top: 35%;
            left: 35%;
            padding: 10px;
            background: #fff;
            -moz-border-radius: 10px;
            -webkit-border-radius: 10px;
            -khtml-border-radius: 10px;
            border-radius: 10px;
        }

        .logo {
            margin-bottom: 20px;
        }

        .bar {
            padding: 10px;
            height: auto;
            overflow: hidden;
        }

        .txtfld {
            height: auto;
            overflow: hidden;
            width: 150px;
        }

        .bar label {
            display: block;
            float: left;
            overflow: hidden;
        }

        .copyright {
            font-size: 10px;
            color: #666;
            margin: 10px 0px;
        }
        .banner {
            background-color: #fffdd0; /* Light gray background for the banner */
            color: #000000; /* Black text color for contrast */
            text-align: center;
            padding: 15px;
            position: fixed;
            width: 100%;
            bottom: 0;
            left: 0;
            box-shadow: 0 -2px 5px rgba(0, 0, 0, 0.3); /* Shadow for better visibility */
            font-family: 'Courier New', Courier, monospace; /* Typewriter-like font */
            font-size: 14px;
            font-weight: bold;
            z-index: 1000; /* Ensure the banner is above other content */
        }

        .disclaimer {
            margin: 0;
            font-size: 10px;
            font-weight: bold;
        }

    </style>
</head>
<body style="background-image: url(images/background.png); font-family: Tahoma; background-repeat: no-repeat; margin: 0; overflow: hidden" onload="setWidthAndHeight();if (self != top) top.location = self.location">
    <form id="form1" runat="server">
        <input type="hidden" id="screenWidth" runat="server" />
        <input type="hidden" id="screenHeight" runat="server" />
                <input type="hidden" id="_t" runat="server" />

        <div class="dashboardwrap">
        </div>
        <div class="loginbox">
            <table>
                <tr>
                    <%-- <td>
                    <div>
                        <img src="images/signin.png" />
                    </div>
                </td>--%>
                    <td>
                        <div class="logo">
                            <div style="font-family: Arial Narrow; font-size: 48px; text-align:center; vertical-align: top; ">

                                <img src="images/NcrAtleosLogo.png" width="320px" height="60px" /><br />
                                <b>VIEW360</b>
                            </div>
                            <table border="0" style="width: 379px">
                                <tr>
                                    <td>
                                        <table border="0" width="370px">
                                            <tr>
                                                <td colspan="2">
                                                    <div id="notification">
                                                        <asp:Literal ID="Literal_ErrMsg" runat="server"></asp:Literal>
                                                        <asp:ValidationSummary ID="ValidationSummary_LoginPage" runat="server" />
                                                    </div>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="heading" colspan="2" style="text-align:center">
                                                    <asp:Label ID="Label_Info" runat="server" Text="Please enter your user id and password to login"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>&nbsp;</td>
                                            </tr>
                                            <tr>
                                                <td align="right" style="width: 150px; padding-right: 20px">Username:
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="TextBox_signin" runat="server" CssClass="txtfld"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator_SignIn" runat="server" SetFocusOnError="true"
                                                        ControlToValidate="TextBox_signin" Text="*" ErrorMessage="Please enter username" autocomplete="off"></asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right" style="padding-right: 20px">
                                                    <asp:Label ID="Label_Password" runat="server" Text="Password:"></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="TextBox_password" TextMode="Password" CssClass="txtfld" runat="server" autocomplete="off"></asp:TextBox>
                                                    <asp:HiddenField ID="xpass" runat="server"></asp:HiddenField>
                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" SetFocusOnError="true"
                                                        ControlToValidate="TextBox_password" ErrorMessage="Please enter password" Text="*"></asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                            <tr runat="server" id="trConfirmPassword">
                                                <td align="right">
                                                    <asp:Label ID="Label_ConfirmPassword" runat="server" Text="Confirm Password:"></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="TextBox_ConfirmPassword" TextMode="Password" CssClass="txtfld" runat="server" autocomplete="off"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>&nbsp;</td>
                                            </tr>
                                            <tr>
                                                <td colspan="2" align="center" style="padding-left: 30px">

                                                    <asp:Button ID="Button_SignIn" runat="server" Text="Sign In" CssClass="btn-success" OnClientClick="return SubmitsEncry();" OnClick="Button_SignIn_Click" />
                                                    <asp:Button ID="Button_Reset" runat="server" Text="Reset" CssClass="btn-success" OnClick="ImageButton_Reset_Click" CausesValidation="false" />



                                                    <%-- <asp:ImageButton ID="ImageButton_RequestChangePassword" runat="server" ImageUrl="images/key_info.png"
                                                    OnClick="ImageButton_RequestChangePassword_Click" />--%>
                                                    <%-- <img src="images/btn_signin.jpg" onclick="Submit()" alt="Click to SignIn"  />
                    <img src="images/btn_reset.jpg" onclick="reset()"  />--%>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            

                            <div class="copyright">
                                @2025 NCR Atleos. All rights reserved.
                            </div>
                        </div>
                    </td>
                </tr>
                                                            <%-- <div class="banner">
        <p class="disclaimer">
This is an official computer system and is the property of the QNB Egypt. It is for authorized users only. Unauthorized users are prohibited. Users (authorized or unauthorized) have no explicit or implicit expectation of privacy. Any or all uses of this system may be subject to one or more of the following actions: interception, monitoring, recording, auditing, inspection and disclosing to security personnel and law enforcement personnel, as well as authorized officials of other agencies, both domestic and foreign. By using this system, the user consents to these actions. Unauthorized or improper use of this system may result in administrative disciplinary action and civil and criminal penalties. By accessing this system you indicate your awareness of and consent to these terms and conditions of use. Discontinue access immediately if you do not agree to the conditions stated in this notice.</p>
    </div>--%>
            </table>
            <input type="button" runat="server" id="refresh" style="display: none" />

    </form>
                                
</body>
</html>
