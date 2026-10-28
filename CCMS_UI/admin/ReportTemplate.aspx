<%@ Page Language="C#" AutoEventWireup="true" Codebehind="ReportTemplate.aspx.cs" Inherits="CCMSUI.admin.ReportTemplate" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
  <link href="../content/ISUITE.css" rel="stylesheet" />
  <title>CCMS - Report Template</title>

    <script src="../content/shortcut.js" type="text/javascript" language="javascript"></script>
</head>

<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)' onload="hideProgressBar();">
    <form id="form1" runat="server">
        <div>
            <table style="width: 100%" cellspacing="0" cellpadding="0" border="0" class="mainbox">
                <tr>
                    <td class="PageHeading" align="left">Report Templates</td>
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
                        &nbsp; &nbsp; &nbsp;
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:HyperLink ID="HyperLink_newTemplate" runat="server" CssClass="Link" NavigateUrl="../admin/AddEditTemplate.aspx">Add new Template... </asp:HyperLink>&nbsp;&nbsp;
                    </td>
                </tr>
                <tr align="left">
                    <th colspan="2" align="left"></th>
                </tr>
                <tr>
                    <td valign="top" colspan="2">
                        <asp:GridView ID="GridView_Templates" runat="server" AutoGenerateColumns="False" AllowSorting="true"
                            OnSorting="GridView_Templates_Sorting" Width="100%" OnRowCommand="GridView_Templates_RowCommand"
                            AllowPaging="True" OnPageIndexChanging="GridView_Templates_PageIndexChanging" OnRowDataBound="GridView_Templates_RowDataBound"
                             PageSize="15" >
                            <AlternatingRowStyle BackColor="#eff0ef" />
                            <RowStyle HorizontalAlign="Center" />
                            <Columns>
                                <asp:BoundField DataField="report_name" HeaderText="Title" SortExpression="report_name" HeaderStyle-ForeColor="black">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>
                                <asp:CheckBoxField DataField="is_active" HeaderText="Active">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:CheckBoxField>
                                <asp:TemplateField HeaderText="Edit">
                                    <ItemTemplate>
                                        <a href='AddEditTemplate.aspx?id=<%# Eval("report_id") %>'>
                                            <img alt="Click to edit template" src="../images/Edit.gif" style="border: 0" class="ShowHand" />
                                        </a>
                                    </ItemTemplate>
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:TemplateField>
                            </Columns>
                            <EmptyDataTemplate>
                                No Template found!</EmptyDataTemplate>
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
