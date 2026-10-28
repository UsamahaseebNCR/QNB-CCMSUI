<%@ Page Language="C#" AutoEventWireup="true" Codebehind="AlertTemplate.aspx.cs" Inherits="Avanza.iSuite.UI.AlertTemplate" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
  <link href="../content/ISUITE.css" rel="stylesheet" />
  <title>iSuite - AlertTemplate</title>

    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>

    <script type="text/javascript">

//window.parent.window.SetContentPage(window.location);
   
function confirmATMDeletion(title)
{
 return confirm('Are you sure you want to delete ATM '+title+'?','iJournal');
}</script>

</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)' onload="hideProgressBar();">
    <form id="form1" runat="server">
        <div>
            <table style="width: 100%" cellspacing="0" cellpadding="0" border="0" class="mainbox">
                <tr>
                    <td class="PageHeading" align="left">
                        Alert Template</td>
                    
                </tr>
               
                <tr>
                    <td class="ValidationText" align="center">
                        <asp:Literal ID="Literal_error" runat="server" EnableViewState="False"></asp:Literal></td>
                </tr>
                <tr>
                    <td class="ConfirmationText" align="center">
                        <%=Session[SessionVars.notice.ToString()]%>
                        <%Session[SessionVars.notice.ToString()] = "";%>
                        <asp:Literal ID="Literal_ok" runat="server" EnableViewState="False"></asp:Literal></td>
                </tr>
               
               
                <tr>
                    <td align="left">
                        &nbsp; &nbsp; &nbsp;&nbsp;
                    </td>
                </tr>
                <tr align="left">
                    <th colspan="2" align="left">
                       
                        <asp:Literal ID="Literal_count" runat="server" EnableViewState="False"></asp:Literal></th>
                </tr>
                <tr>
                    <td valign="top" colspan="2">
                        <asp:GridView ID="GridView_AlertTemplate" runat="server" AutoGenerateColumns="False" 
                            Width="100%" AllowPaging="True" PageSize="15" 
                            onpageindexchanged="GridView_AlertTemplate_PageIndexChanged" 
                            onpageindexchanging="GridView_AlertTemplate_PageIndexChanging" OnRowCommand="GridView_AlertTemplate_RowCommand" >
                           
                            <AlternatingRowStyle BackColor="#eff0ef" />
                            <Columns>
                                <asp:BoundField DataField="alert_type_name" HeaderText="Alert Type" SortExpression="alert_type_name" HeaderStyle-ForeColor="black">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>
                                <asp:BoundField DataField="alert_additional_text" HeaderText="Alert Additional Text" SortExpression="alert_additional_text" 
                                    HeaderStyle-ForeColor="black">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>
                               

                                <asp:CheckBoxField DataField="send_email_notification" HeaderText="Email Notification">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:CheckBoxField>
                                 <asp:CheckBoxField DataField="open_ticket_in_gasper" HeaderText="Open Ticket In Gasper">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:CheckBoxField>

                                <asp:BoundField DataField="tpa_code" HeaderText="TPA Code" SortExpression="tpa_code" 
                                    HeaderStyle-ForeColor="black">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>
                                 <asp:BoundField DataField="tpa_value" HeaderText="TPA Value" SortExpression="tpa_value" 
                                    HeaderStyle-ForeColor="black">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>



                                <asp:TemplateField HeaderText="Edit">
                                <ItemTemplate>
                                    <asp:ImageButton ID="imgbtn_editalerttype" CommandName="editAlertType" ImageUrl="../images/Edit.gif"  runat="server" CommandArgument='<%# Eval("alert_type_id") %>'
                                    ToolTip='Click to edit alert type' CssClass="ShowHand" />
                                </ItemTemplate>
                                    <%--<ItemTemplate>
                                        <a href='EditAlertTemplate.aspx?alert_type_id=<%# Eval("alert_type_id") %>'>
                                            <img alt="Click to edit alert template" src="../images/Edit.gif" style="border: 0" class="ShowHand" />
                                        </a>
                                    </ItemTemplate>--%>
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:TemplateField>
                                
                            </Columns>
                            <EmptyDataTemplate>
                                No Alert Template found!</EmptyDataTemplate>
                            <HeaderStyle BackColor="Black" ForeColor="White" CssClass="HeadingCell" />
                        </asp:GridView>
                        &nbsp;
                    </td>
                </tr>
            </table>
        </div>
        <input type="button" runat="server" id="refresh" style="display: none" />
    </form>
</body>
</html>
