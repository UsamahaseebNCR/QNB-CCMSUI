<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditExceptionHandlingCriteria.aspx.cs"
    Inherits="Avanza.iSuite.UI.EditExceptionHandlingCriteria" %>

<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Alert Template</title>
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
    <script type="text/javascript">
        window.parent.window.contentPage = '';          
    </script>
    <link href="../content/iSuite.css" rel="stylesheet" />
</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)'
    onload="hideProgressBar();">
    <form id="form1" runat="server">
    <table cellpadding="0" cellspacing="0" border="0" class="MainBox" width="50%">
        <tr>
            <td class="PageHeading" align="left" colspan="2">
                Edit Exception Handling Criteria
            </td>
        </tr>
        <tr>
            <td class="SearchHeadPaneGradient" colspan="2">
            </td>
        </tr>
        <tr style="height: 20px">
            <td colspan="2" style="color: red;" align="center" valign="top">
                <asp:Literal ID="Literal_error" runat="server" EnableViewState="False"></asp:Literal>
            </td>
        </tr>
        <tr>
            <td>
                <table border="0" cellpadding="0" cellspacing="0" width="50%" style="padding-left: 20px">
                    <tr>
                        <td>
                            Transaction Type
                        </td>
                        <td>
                            <asp:DropDownList ID="DropDownList_TrxnType" runat="server" CssClass="dropDown">
                                <asp:ListItem Text="Cash Deposit Transaction" Value="0"></asp:ListItem>
                                <asp:ListItem Text="Cheque Deposit Transaction" Value="1"></asp:ListItem>
                                <asp:ListItem Text="Normal Transaction" Value="2"></asp:ListItem>
                                <asp:ListItem Text="Captured Card Transaction" Value="3"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Message:
                        </td>
                        <td class="style2">
                            <asp:TextBox ID="TextBox_Message" runat="server" CssClass="textbox" TextMode="MultiLine"
                                Height="80px" MaxLength="250" Width="400px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Type:
                        </td>
                        <td>
                            <asp:CheckBoxList ID="CheckBox_Conditions" runat="server" RepeatDirection="Horizontal">
                                <asp:ListItem Value="0">PreCondition</asp:ListItem>
                                <asp:ListItem Value="1">Disputed</asp:ListItem>
                                <asp:ListItem Value="2">SMSTaskRequired</asp:ListItem>
                            </asp:CheckBoxList>
                        </td>
                    </tr>
                    <tr>
                        <td style="font-weight: bold; padding-top: 30px;">
                            <asp:Button ID="Button_SaveUpdateCriteria" runat="server" Text="Save" OnClick="Button_SaveUpdateCriteria_Click"
                                CssClass="button" Width="60px" />&nbsp;
                            <input id="Button_Cancel" type="button" value="Cancel" onclick="window.location ='ExceptionHandlingCriteria.aspx'"
                                class="button" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
