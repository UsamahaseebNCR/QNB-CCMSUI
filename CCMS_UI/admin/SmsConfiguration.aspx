<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SmsConfiguration.aspx.cs"
    Inherits="Avanza.iSuite.UI.SmsConfiguration" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="../content/ISUITE.css" rel="stylesheet" />
    <title>iSuite - AlertTemplate</title>
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
    <script type="text/javascript">

        //window.parent.window.SetContentPage(window.location);

        function confirmATMDeletion(title) {
            return confirm('Are you sure you want to delete ATM ' + title + '?', 'iJournal');
        }</script>
</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)'
    onload="hideProgressBar();">
    <form id="form1" runat="server">
    <div>
        <table style="width: 100%" cellspacing="0" cellpadding="0" border="0" class="mainbox">
            <tr>
                <td class="PageHeading" align="left">
                    SMS Configuration
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
            <tr align="left" style="background-color: WHITE">
                <td style="height: 20px; padding-left: 5px;" colspan="2">
                    <b>Search Filters</b>
                </td>
            </tr>
            <tr align="left">
                <td colspan="2">
                    <table width="100%" class="SEARCHBOX" border="0">
                        <tr>
                            <td>
                                <table id="search" width="90%" border="0">
                                    <tr>
                                        <td style="height: 20px">
                                            Template :
                                        </td>
                                        <td style="height: 20px">
                                            <asp:DropDownList ID="DropDownList_template" runat="server" Width="150px" CssClass="textbox">
                                            </asp:DropDownList>
                                        </td>
                                        <td style="height: 20px">
                                            Status :
                                        </td>
                                        <td style="height: 20px">
                                            <asp:DropDownList ID="DropDownList_status" runat="server" Style="width: 150px" CssClass="textbox">
                                                <asp:ListItem>Select</asp:ListItem>
                                                <asp:ListItem>None</asp:ListItem>
                                                <asp:ListItem>Successful</asp:ListItem>
                                                <asp:ListItem>Suspicious</asp:ListItem>
                                                <asp:ListItem>Failed</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Channel :&nbsp;
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="DropDownList_channel" runat="server" Width="151px" CssClass="textbox">
                                                <asp:ListItem Value="Any">Any</asp:ListItem>
                                                <asp:ListItem Value="0">SMS</asp:ListItem>
                                                <asp:ListItem Value="1">Email</asp:ListItem>
                                                <asp:ListItem Value="2">Both</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                        </td>
                                        <td>
                                        </td>
                                        <td>
                                        </td>
                                        <td>
                                            <asp:Button ID="Button_search" runat="server" CssClass="button" OnClick="Button_search_Click"
                                                Text="Search" Width="71px" />
                                            &nbsp;<asp:Button ID="Button_reset" runat="server" CssClass="button" Text="Reset"
                                                Width="71px" OnClick="Button_reset_Click" />&nbsp;
                                            <asp:HyperLink ID="HyperLink1" runat="server" CssClass="Link" NavigateUrl="../admin/EditSmsConfiguration.aspx">Add new Configuration ... </asp:HyperLink>
                                            &nbsp;&nbsp;
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td>
                                <table>
                                    <tr>
                                        <td>
                                            <img src="../images/Icon_search.png" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td align="left">
                    &nbsp; &nbsp; &nbsp;&nbsp;
                </td>
            </tr>
            <tr align="left">
                <th colspan="2" align="left">
                    <asp:Literal ID="Literal_count" runat="server" EnableViewState="False"></asp:Literal>
                </th>
            </tr>
            <tr>
                <td>
                </td>
            </tr>
            <tr>
                <td valign="top" colspan="2">
                    <asp:GridView ID="GridView_SMSTemplate" runat="server" AutoGenerateColumns="False"
                        Width="100%" AllowPaging="True" PageSize="15" BackColor="White" BorderStyle="Solid"
                        AllowSorting="true" OnPageIndexChanged="GridView_AlertTemplate_PageIndexChanged"
                        OnPageIndexChanging="GridView_AlertTemplate_PageIndexChanging" OnRowCommand="GridView_SMSTemplate_RowCommand"
                        OnSorting="GridView_SMSTemplate_Sorting"
                        OnRowDataBound ="GridView_SMSTemplate_RowDataBound"
                        >
                        <AlternatingRowStyle BackColor="#eff0ef" />
                        <RowStyle HorizontalAlign="Center" />
                        <Columns>
                            <asp:BoundField DataField="ccms_sms_template_name" HeaderText="Template Name" HeaderStyle-ForeColor="black"
                                SortExpression="ccms_sms_template_name" ItemStyle-HorizontalAlign="Left">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="tibco_template_id" HeaderText="Template ID" HeaderStyle-ForeColor="black"
                                SortExpression="tibco_template_id">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="transaction_type_name" HeaderStyle-ForeColor="black" SortExpression="transaction_type_name"
                                HeaderText="Transaction Type" ItemStyle-HorizontalAlign="Left">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="status" HeaderText="Status" SortExpression="status" HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="action_code" HeaderText="Action Code" HeaderStyle-ForeColor="black"
                                SortExpression="action_code" ItemStyle-HorizontalAlign="Left">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="alert_type_name" HeaderText="Alert Type" SortExpression="alert_type_name" 
                            HeaderStyle-ForeColor="black" ItemStyle-HorizontalAlign="Left">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Channel" HeaderText="Channel" HeaderStyle-ForeColor="black"
                                SortExpression="Channel">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="AlertType" HeaderText="AlertType" HeaderStyle-ForeColor="black"
                                SortExpression="AlertType" ItemStyle-HorizontalAlign="Left">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:TemplateField HeaderText="Edit">
                                <ItemTemplate>
                                    <a href='EditSmsConfiguration.aspx?configuration_id=<%# Eval("sms_template_configuration_id") %>'>
                                        <img alt="Click to edit SMS template" src="../images/Edit.gif" style="border: 0"
                                            class="ShowHand" />
                                    </a>
                                </ItemTemplate>
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Delete">
                                <ItemTemplate>
                                    <asp:ImageButton runat="server" CommandName="deleteSMSConfiguration" CommandArgument='<%# Eval("sms_template_configuration_id") %>'
                                        ToolTip="Click to delete sms configuration" ImageUrl="../images/Delete.gif" ID="Button_deleteConfiguration"
                                        CssClass="ShowHand" />
                                </ItemTemplate>
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                            No SMS Configuration found!</EmptyDataTemplate>
                        <HeaderStyle BackColor="White" ForeColor="White" />
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
