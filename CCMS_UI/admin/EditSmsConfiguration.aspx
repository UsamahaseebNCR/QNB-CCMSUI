<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditSmsConfiguration.aspx.cs" Inherits="Avanza.iSuite.UI.EditSmsConfiguration" %>

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
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)' onload="hideProgressBar();">
    <form id="form1" runat="server">
    <table cellpadding="0" cellspacing="0" border="0" class="MainBox" width="50%" >
        <tr>
            <td class="PageHeading" align="left" colspan="2">
                Edit SMS Configuration
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
                        <tr style="height:25px;">
                    
              <td style="padding-left: 10px; " class="style3">
                           Alert Category :
                        </td>
                        <td>
                            <asp:DropDownList ID="DropDownList_Alert" runat="server" AutoPostBack="True"
                                CssClass="dropdown" Height="18px" Width="200px" 
                                onselectedindexchanged="DropDownList_Alert_SelectedIndexChanged">
                                <asp:ListItem Value="Staff">Staff</asp:ListItem>
                                <asp:ListItem Value="Customers" Selected="True">Customers</asp:ListItem>
                            </asp:DropDownList>
                        </td>        
                    
                    </tr>
                    <tr style="height:25px;">
                    
              <td style="padding-left: 10px; " class="style3">
                            <asp:Label ID="Label_Select" runat="server" Text="Alert Type : "></asp:Label>
                        </td>
                        <td>
                              <asp:DropDownList ID="DropDownList_AlertType" runat="server" AutoPostBack="True"
                                CssClass="dropdown" Height="18px" Width="200px">
                            </asp:DropDownList>
                        </td>        
                    
                    </tr>
                    <tr style="height:25px;">
                    
              <td style="padding-left: 10px; " class="style3">
                            <asp:Label ID="Label_TransactionType" runat="server" Text="Transaction Type : "></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="DropDownList_TransactionType" runat="server" AutoPostBack="True"
                                CssClass="dropdown" Height="18px" Width="200px" 
                                onselectedindexchanged="DropDownList_TransactionType_SelectedIndexChanged">
                            </asp:DropDownList>
                        </td>        
                    
                    </tr>
                  
                    <tr style="height:25px;">
                        <td style="padding-left: 10px; " class="style1">
                            <asp:Label ID="Label_ActionCode" runat="server" Text="Action Code:"></asp:Label>
                        </td>
                        <td class="style2" style="width: 65%">
                                                        <asp:DropDownList ID="DropDownList_ActionCode" runat="server" AutoPostBack="True"
                                CssClass="dropdown" Height="18px" Width="200px" OnSelectedIndexChanged="DropDownList_ActionCode_SelectedIndexChanged">
                            </asp:DropDownList>
                        
                        </td>
                    </tr>
                    <br />
                    <tr style="height:25px;">
                        <td style="padding-left: 10px;" class="style1">
                            <asp:Label ID="Label_TemplateName" runat="server" Text="Template Name: "></asp:Label>
                        </td>
                        <td class="style2">
                            <asp:DropDownList ID="DropDownList_TemplateName" runat="server" AutoPostBack="True"
                                CssClass="dropdown" Height="18px" Width="200px">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr style="height:25px;">
                        <td style="padding-left:10px; " class="style1">
                            <asp:Label ID="Label_Status" runat="server" Text="Status : "></asp:Label>
                        </td>
                        <td class="style2">
                          <asp:DropDownList ID="DropDownList_Status" runat="server" AutoPostBack="True"
                                CssClass="dropdown" Height="18px" Width="200px" OnSelectedIndexChanged="DropDownList_Status_SelectedIndexChanged">
                              <asp:ListItem Value="0">None</asp:ListItem>
                              <asp:ListItem Value="1">Successful</asp:ListItem>
                              <asp:ListItem Value="2">Suspicious</asp:ListItem>
                              <asp:ListItem Value="3">Failed</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr style="height:25px;">
                        <td style="padding-left:10px; " class="style1">
                            <asp:Label ID="Label_Channel" runat="server" Text="Channel : "></asp:Label>
                        </td>
                        <td class="style2">
                          <asp:DropDownList ID="DropDownList_Channel" runat="server" AutoPostBack="True"
                                CssClass="dropdown" Height="18px" Width="200px">
                              <asp:ListItem Value="0">SMS</asp:ListItem>
                              <asp:ListItem Value="1">Email</asp:ListItem>
                              <asp:ListItem Value="2">Both</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
               


                </table>
            </td>
            <td style="width: 45%" valign="top">
                <table width="100%" border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td style="padding-left: 10px; vertical-align: top;" class="style3">
                            &nbsp;</td>
                        <td>
                            &nbsp;</td>
                    </tr>
                    <tr>
                        <td style="padding-left: 10px; vertical-align: top;" class="style3">
                            &nbsp;</td>
                        <td>
                            &nbsp;</td>
                    </tr>
                </table>
            </td>
        </tr>
      
    </table>

    <br />
    <table style="margin-left:10px;">
       
                      <tr>
            <td colspan="2" style="font-weight: bold; padding-top: 30px;">
                <asp:Button ID="Button_SaveUpdateEditAlertTemplate" runat="server" Text="Save" OnClick="Button_SaveUpdateEditAlertTemplate_Click"
                    CssClass="button" Width="60px" />&nbsp;
                <input id="Button_Cancel" type="button" value="Cancel" onclick="window.location ='SmsConfiguration.aspx'"
                    class="button" />
            </td>
        </tr>
                    
                  
    </table>
    <br />
    </form>
</body>
</html>