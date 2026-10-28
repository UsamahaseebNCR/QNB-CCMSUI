<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditSmsTemplate.aspx.cs"
    Inherits="Avanza.iSuite.UI.EditSmsTemplate" %>

<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
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
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)'
    onload="hideProgressBar();">
    <form id="form1" runat="server">
    <table cellpadding="0" cellspacing="0" border="0" class="MainBox" width="50%" style="padding-left: 10px;">
        <tr>
            <td class="PageHeading" align="left" colspan="2">
                Edit SMS Template
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
                        <td>
                            Template ID:
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_TemplateID" runat="server" CssClass="textbox" Enabled="True"
                                Height="20px" Width="400px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr style="padding-top: 20px;">
                        <td style="vertical-align: top;" class="style1">
                            <asp:Label ID="Label_TemplateName" runat="server" Text="Template Name: "></asp:Label>
                        </td>
                        <td class="style2">
                            <asp:TextBox ID="TextBox_TemplateName" runat="server" CssClass="textbox" Enabled="True"
                                Height="20px" Width="400px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr style="padding-top: 20px;">
                        <td style="vertical-align: top;" class="style1">
                            <asp:Label ID="Label_additional" runat="server" Text="Template Message : "></asp:Label>
                        </td>
                        <br />
                        <td class="style2">
                            <asp:TextBox ID="TextBox_TemplateMessage" runat="server" CssClass="textbox" TextMode="MultiLine"
                                Height="80px" MaxLength="250" Width="400px"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </td>
            <td style="width: 45%" valign="top">
                <table width="100%" border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td style="padding-left: 10px; vertical-align: top;" class="style3">
                            &nbsp;
                        </td>
                        <td>
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td style="padding-left: 10px; vertical-align: top;" class="style3">
                            &nbsp;
                        </td>
                        <td>
                            &nbsp;
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <br />
    <table style="margin-left: 10px;">
        <tr>
            <td>
                @P1 : AccountNo Notification
            </td>
            <td>
                @P2 : Atm ID Notification
            </td>
            <td>
                @P3 : ATM Location Notification
            </td>
            <td>
                @P4 : Branch Notification
            </td>
        </tr>
        <tr>
            <td>
                @P5 : Transaction Date Notification
            </td>
            <td>
                @P6 : ReferenceNo Notification
            </td>
            <td>
                @P7 : Customer Claim Amount Notification
            </td>
            <td>
                @P8 : Escalation Type Notification
            </td>
        </tr>
        <tr>
            <td>
                @P9 : Problem Start Date Notification
            </td>
            <td>
                @P10 : Problem Desc Notification
            </td>
            <td>
                @P11 : Cash Monitoring Alert Type Notification
            </td>
            <td>
                @P12 : Amount Remaining Notification
            </td>
        </tr>
        <tr>
            <td>
                @P13 : Next Replenishment Date Notification
            </td>
            <td>
                @P14 : Nonpeak Dispensation Amount Notification
            </td>
            <td>
                @P15 : Peak Dispensation Amount Notification
            </td>
            <td>
                @P16 : Cassette Wise Position Notification
            </td>
        </tr>
        <tr>
            <td>
                @P17 : Transaction Type Notification
            </td>
            <td>
                @P18 : Occurence Notification
            </td>
            <td>
                @P19 : Idle Period Notification
            </td>
            <td>
                @P20 : Last Transaction Time Notification
            </td>
        </tr>
        <tr>
            <td colspan="2" style="font-weight: bold; padding-top: 30px;">
                <asp:Button ID="Button_SaveUpdateEditAlertTemplate" runat="server" Text="Save" OnClick="Button_SaveUpdateEditAlertTemplate_Click"
                    CssClass="button" Width="60px" />&nbsp;
                <input id="Button_Cancel" type="button" value="Cancel" onclick="window.location ='SmsTemplate.aspx'"
                    class="button" />
            </td>
        </tr>
    </table>
    <br />
    </form>
</body>
</html>
