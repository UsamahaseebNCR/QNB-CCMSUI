<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExceptionHandlingCriteria.aspx.cs"
    Inherits="Avanza.iSuite.UI.ExceptionHandlingCriteria" %>

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
                    Exception Handling Criteria
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
                                            Message :
                                        </td>
                                        <td style="height: 20px">
                                            <asp:TextBox ID="TextBox_message" runat="server" CssClass="textboxNormal"></asp:TextBox>
                                        </td>
                                        <td style="height: 20px">
                                            <asp:CheckBoxList ID="CheckBox_Conditions" runat="server" RepeatDirection="Horizontal">
                                                <asp:ListItem Value="0">PreCondition</asp:ListItem>
                                                <asp:ListItem Value="1">Disputed</asp:ListItem>
                                                <asp:ListItem Value="2">SMSTaskRequired</asp:ListItem>
                                            </asp:CheckBoxList>
                                        </td>
                                        <td style="height: 20px">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            &nbsp;
                                        </td>
                                        <td>
                                            &nbsp;
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
                                            <asp:HyperLink ID="HyperLink1" runat="server" CssClass="Link" NavigateUrl="../admin/EditExceptionHandlingCriteria.aspx">Add new Criteria ... </asp:HyperLink>
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
                    <asp:GridView ID="GridView_ExceptionCriteria" runat="server" AutoGenerateColumns="False"
                        Width="100%" AllowPaging="True" PageSize="15" OnPageIndexChanged="GridView_AlertTemplate_PageIndexChanged"
                        OnPageIndexChanging="GridView_ExceptionCriteria_PageIndexChanging" OnRowCommand="GridView_ExceptionCriteria_RowCommand"
                        AllowSorting="True" BackColor="White" BorderStyle="Solid" OnSorting="GridView_SMSTemplate_Sorting"
                        OnRowDataBound="GridView_Criteria_RowDataBound"
                        >
                        <AlternatingRowStyle BackColor="#eff0ef" />
                        <Columns>
                            <asp:BoundField DataField="transaction_type_name" HeaderText="Transaction Type" SortExpression="transaction_type_name"
                                HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                           
                            <asp:BoundField DataField="message" HeaderText="Message" SortExpression="message"
                                HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:CheckBoxField DataField="is_precondition" HeaderText="PreCondition" SortExpression="is_precondition">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="Black" />
                            </asp:CheckBoxField>
                            <asp:CheckBoxField DataField="is_disputed" HeaderText="Disputed" SortExpression="is_disputed">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="Black" />
                            </asp:CheckBoxField>
                            <asp:CheckBoxField DataField="is_sms_task_required" HeaderText="SMSTaskRequired"
                                SortExpression="is_sms_task_required">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="Black" />
                            </asp:CheckBoxField>
                            <asp:TemplateField HeaderText="Edit">
                                <ItemTemplate>
                                   <%-- <a href='EditExceptionHandlingCriteria.aspx?exceptionCriteria_id=<%# Eval("exception_handling_criteria_id") %>'>
                                        <img alt="Click to edit SMS template" src="../images/Edit.gif" style="border: 0"
                                            class="ShowHand" />
                                    </a>--%>
                                                                         <asp:ImageButton runat="server" CommandName="editTemplate" CommandArgument='<%# Eval("exception_handling_criteria_id") %>'
                                            ToolTip="Click to edit Template" ImageUrl="../images/Edit.gif" ID="Button_editCriteria"
                                            CssClass="ShowHand" OnCommand="Button_editCriteria_Click"/>

                                    <%-- <asp:ImageButton runat="server" CommandName="editTemplate" CommandArgument='<%# Eval("ccms_sms_template_id") %>'
                                            ToolTip="Click to edit Template" ImageUrl="../images/Edit.gif" ID="Button_editATM"
                                            CssClass="ShowHand"      />--%>
                                </ItemTemplate>
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Delete">
                                <ItemTemplate>
                                    <asp:ImageButton runat="server" CommandName="deleteCriteria" CommandArgument='<%# Eval("exception_handling_criteria_id") %>'
                                        ToolTip="Click to delete Template" ImageUrl="../images/Delete.gif" ID="Button_deleteTemplate" 
                                        CssClass="ShowHand" />
                                </ItemTemplate>
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                            No Exception Criteria found!</EmptyDataTemplate>
                        <HeaderStyle BackColor="White" ForeColor="White" />
                        <RowStyle HorizontalAlign="Center" />
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
