
<%@ Page Language="C#" AutoEventWireup="true" ValidateRequest="true" CodeBehind="EditAlertTemplate.aspx.cs"
    Inherits="Avanza.iSuite.UI.EditAlertTemplate" Title="Alert Template" %>

<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Alert Template</title>
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
    <script type="text/javascript">
        window.parent.window.contentPage = '';          
        
        
    </script>
    <script type="text/javascript">
        function setCheck(node, selected) {
            var childary = node.get_nodes();

            for (var i = 0; i < childary.get_length(); i++) {
                childary.getNode(i).set_checked(selected);
            }
        }

        window.parent.window.contentPage = '';

        function check(sender, eventArgs) {
            AlertNotificationUsersTreeView.beginUpdate();
            setCheck(eventArgs.get_node(), eventArgs.get_node().get_checked());
            AlertNotificationUsersTreeView.endUpdate();
        }
    </script>
    <link href="../content/iSuite.css" rel="stylesheet" />
    <style type="text/css">
        .style1
        {
            width: 32%;
        }
        .style2
        {
            width: 91%;
        }
        .style3
        {
            width: 145px;
        }
    </style>
</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)' onload="hideProgressBar();">
    <form id="form1" runat="server">
    <table cellpadding="0" cellspacing="0" border="0" class="MainBox" width="50%" style="padding-left: 10px;">
        <tr>
            <td class="PageHeading" align="left" colspan="2">
                Edit Alert Template
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
            <td style="width: 55%" valign="top">
                <table id="inner" border="0" cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td class="style1">
                            <asp:Label ID="Label_selectedTemplate" runat="server" Text="Selected Template:"></asp:Label>
                        </td>
                        <td class="style2" style="width: 65%">
                            <asp:Label ID="Label_AlertType" runat="server" Font-Bold="true"></asp:Label>
                        </td>
                    </tr>
                    <tr style="padding-top: 20px;">
                        <td style="vertical-align: top;" class="style1">
                            <asp:Label ID="Label_default" runat="server" Text="Default Notification Text: "></asp:Label>
                        </td>
                        <td class="style2">
                            <asp:TextBox ID="TextBox_default" runat="server" CssClass="textbox" TextMode="MultiLine"
                                Enabled="false" Height="80px" Width="400px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr style="padding-top: 20px;">
                        <td style="vertical-align: top;" class="style1">
                            <asp:Label ID="Label_additional" runat="server" Text="Additional Notification Text: "></asp:Label>
                        </td>
                        <td class="style2">
                            <asp:TextBox ID="TextBox_additional" runat="server" CssClass="textbox" TextMode="MultiLine"
                                Height="80px" MaxLength="250" Width="400px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Send Email Notification
                        </td>
                        <td>
                        
                            <asp:CheckBox ID="CheckBox_EmailNotification" runat="server" />
                        </td>
                    </tr>
                     <tr>
                        <td>
                            Open Ticket In Gasper
                        </td>
                        <td>
                        
                            <asp:CheckBox ID="CheckBox_OpenTicket" runat="server" />
                        </td>
                    </tr>

                    <tr>
                        <td>
                            TPA Code
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_TPACode" runat="server" CssClass="textbox"></asp:TextBox>
                        </td>
                    </tr>

                    
                    <tr>
                        <td>
                            TPA Value
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_TPAValue" runat="server" CssClass="textbox"></asp:TextBox>
                        </td>
                    </tr>


                </table>
            </td>
            <td style="width: 45%" valign="top">
                <table width="100%" border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td style="padding-left: 10px; vertical-align: top;" class="style3">
                            <asp:Label ID="Label_Organization" runat="server" Text="Organization: "></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="DropDownList_Organization" runat="server" AutoPostBack="True"
                                OnSelectedIndexChanged="DropDownList_Organization_SelectedIndexChanged" CssClass="dropdown">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td style="padding-left: 10px; vertical-align: top;" class="style3">
                            <asp:Label ID="Label_Notification_Users" runat="server" Text="Send Notifications To: " ></asp:Label>
                        </td>
                        <td>
                            <asp:ListBox ID="ListBox1" runat="server" SelectionMode="Multiple"></asp:ListBox>
                            <%--list box dalna ha--%>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td colspan="2" style="font-weight: bold; padding-top: 30px;">
                <asp:Button ID="Button_SaveUpdateEditAlertTemplate" runat="server" Text="Save" OnClick="Button_SaveUpdateEditAlertTemplate_Click"
                    CssClass="button" Width="60px" />&nbsp;
                <input id="Button_Cancel" type="button" value="Cancel" onclick="window.location ='AlertTemplate.aspx'"
                    class="button" />
            </td>
        </tr>
    </table>
    <br />
    <br />
    </form>
</body>
</html>
