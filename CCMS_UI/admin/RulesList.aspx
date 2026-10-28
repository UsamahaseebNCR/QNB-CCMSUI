<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RulesList.aspx.cs" Inherits="Avanza.iSuite.UI.RulesList" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="../content/ISUITE.css" rel="stylesheet" />
    <title>CCMS - Rules Management</title>
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
                <td class="PageHeading" align="left" colspan="4">
                    Rules Management
                </td>
            </tr>
            <tr>
                <td class="ValidationText" align="center" colspan="4">
                    <asp:Literal ID="Literal_error" runat="server" EnableViewState="False"></asp:Literal>
                </td>
            </tr>
            <tr>
                <td class="ConfirmationText" align="center">
                    <asp:ValidationSummary ID="valSummary" runat="server" />
                    <%=Session[SessionVars.notice.ToString()]%>
                    <%Session[SessionVars.notice.ToString()] = "";%>
                    <asp:Literal ID="Literal_ok" runat="server" EnableViewState="False"></asp:Literal>
                </td>
            </tr>
            <tr align="left" style="background-color: WHITE">
                <td style="height: 20px; padding-left: 5px;" colspan="4">
                    <b>Search Filters</b>
                </td>
            </tr>
            <tr align="left">
                <td colspan="4">
                    <table width="100%" class="SEARCHBOX" border="0">
                        <tr>
                            <td>
                                <table id="search" width="30%" border="0">
                                    <tr>
                                        <td>
                                            Rule Friendly Name
                                        </td>
                                        <td>
                                            <asp:TextBox ID="TextBox_RuleFriendlyName" runat="server" CssClass="textbox"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2" align="right" style="padding-right: 35px">
                                            <asp:Button CssClass="button" ID="button_search" runat="server" Text="Search" OnClick="button_search_Click" />
                                            &nbsp;<asp:Button ID="Button_Reset" runat="server" CssClass="button" OnClick="Button_Reset_Click"
                                                Text="Reset" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td valign="top">
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
            <tr align="left">
                <th colspan="2" align="left">
                    Selected Rules -
                    <asp:Literal ID="Literal_count" runat="server" EnableViewState="False"></asp:Literal>
                </th>
            </tr>
            <tr>
                <td valign="top" colspan="2">
                    <asp:GridView ID="GridView_Rules" runat="server" AutoGenerateColumns="False" Width="100%"
                        AllowPaging="True" PageSize="15" OnPageIndexChanging="GridView_Rules_PageIndexChanging"
                        OnSorting="GridView_Rules_Sorting" AllowSorting="true">
                        <AlternatingRowStyle BackColor="#eff0ef" />
                        <Columns>
                            <asp:BoundField DataField="transaction_rule_name" HeaderText="Rule Name"
                                SortExpression="transaction_rule_name" HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="transaction_rule_friendly_name" HeaderText="Rule Friendly Name"
                                SortExpression="transaction_rule_friendly_name" HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="creation_time" HeaderText="Creation Time" SortExpression="creation_time"
                                HeaderStyle-ForeColor="black" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" ItemStyle-HorizontalAlign="Center">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    Criteria
                                </HeaderTemplate>
                                <HeaderStyle CssClass="HeadingCell" />
                                <ItemTemplate>
                                    <%# Eval("transaction_rule_filtering_criteria").ToString().Replace(",","<br/>")%>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="color" HeaderText="Color" HeaderStyle-ForeColor="black"
                                ItemStyle-HorizontalAlign="Center">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:CheckBoxField DataField="is_active" HeaderText="Active" ItemStyle-HorizontalAlign="Center">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:CheckBoxField>
                            <asp:TemplateField HeaderText="Edit" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <a href='AddEditRule.aspx?r=<%# Eval("transaction_rule_id") %>'>
                                        <img alt="Click to edit rule" src="../images/Edit.gif" style="border: 0" class="ShowHand" />
                                    </a>
                                </ItemTemplate>
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
