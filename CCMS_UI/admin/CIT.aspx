<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CIT.aspx.cs" Inherits="CCMSUI.admin.CIT" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="../content/ISUITE.css" rel="stylesheet" />
    <title>iSuite - CIT</title>
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
    <script type="text/javascript">

        //window.parent.window.SetContentPage(window.location);

        function confirmCITDeletion(title) {
            return confirm('Are you sure you want to delete CIT ' + title + '?', 'iJournal');
        }</script>
</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)'
    onload="hideProgressBar();">
    <form id="form1" runat="server">
    <div>
        <table style="width: 100%" cellspacing="0" cellpadding="0" border="0" class="mainbox">
            <tr>
                <td class="PageHeading" align="left">
                    CIT Management
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
                                <table id="search" width="100%" border="0">
                                    <tr>
                                        <td>
                                            Name :
                                        </td>
                                        <td>
                                            <asp:TextBox ID="TextBox_name" runat="server" Width="148px" CssClass="textboxNormal"></asp:TextBox>
                                        </td>
                                        <td>
                                            Code :
                                        </td>
                                        <td>
                                            <asp:TextBox ID="TextBox_Code" runat="server" CssClass="textboxNormal"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <%--<tr>
                                            <td style="height: 20px">
                                                ID :</td>
                                            <td style="height: 20px">
                                                <asp:TextBox ID="TextBox_ID" runat="server" CssClass="textboxNormal"></asp:TextBox>
                                            </td>
                                            <td style="height: 20px">
                                                Cit Team ID :</td>
                                            <td style="height: 20px">
                                                <asp:TextBox ID="TextBox_teamID" runat="server" CssClass="textboxNormal"></asp:TextBox>
                                            </td>
                                        </tr>--%>
                                    <tr>
                                        <td style="height: 20px">
                                            Status :
                                        </td>
                                        <td style="height: 20px">
                                            <asp:DropDownList ID="DropDownList_status" runat="server" Style="width: 150px" CssClass="textbox">
                                                <asp:ListItem Selected="True" Value="0,1">Any</asp:ListItem>
                                                <asp:ListItem Value="1">Active</asp:ListItem>
                                                <asp:ListItem Value="0">InActive</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td style="height: 20px">
                                            Created by :
                                        </td>
                                        <td style="height: 20px">
                                            <asp:DropDownList ID="DropDownList_creator" runat="server" Width="150px" CssClass="textbox">
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
                                            <asp:HyperLink ID="HyperLink_newCIT" runat="server" CssClass="Link" NavigateUrl="../admin/AddEditCIT.aspx">Add new CIT ... </asp:HyperLink>
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
                    Selected CITs -
                    <asp:Literal ID="Literal_count" runat="server" EnableViewState="False"></asp:Literal>
                </th>
            </tr>
            <tr>
                <td valign="top" colspan="2">
                    <asp:GridView ID="GridView_CIT" runat="server" AutoGenerateColumns="False" AllowSorting="true"
                        OnSorting="GridView_CIT_Sorting" Width="100%" OnRowCommand="GridView_CIT_RowCommand"
                        AllowPaging="True" OnPageIndexChanging="GridView_CIT_PageIndexChanging" OnRowDataBound="GridView_CIT_RowDataBound"
                        PageSize="15" HeaderStyle-CssClass="HeadingCell">
                        <RowStyle HorizontalAlign="Center" />
                        <AlternatingRowStyle BackColor="#eff0ef" />
                        <Columns>
                            <asp:BoundField DataField="name" HeaderText="Name" SortExpression="name" HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            
                            <asp:BoundField DataField="code" HeaderText="Code" SortExpression="code" HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="email_id" HeaderText="Email" SortExpression="email_id"
                                HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="order_dispatch_time" HeaderText="Order Dispatch Time"
                                SortExpression="order_dispatch_time" DataFormatString="{0:HH:mm:ss}">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="normal_order_cost" HeaderText="Normal Order Cost" SortExpression="normal_order_cost"
                                HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="emergency_order_cost" HeaderText="Emergency Order Cost"
                                SortExpression="emergency_order_cost" HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="created_on" HeaderText="Creation Time" SortExpression="created_on"
                                DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="modified_on" HeaderText="Modification Time" SortExpression="modified_on"
                                DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:CheckBoxField DataField="is_active" HeaderText="Active">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:CheckBoxField>
                            <asp:TemplateField HeaderText="Edit">
                                <ItemTemplate>
                                    <a href='AddEditCIT.aspx?id=<%# Eval("id") %>'>
                                        <img alt="Click to edit cit" src="../images/Edit.gif" style="border: 0" class="ShowHand" />
                                    </a>
                                </ItemTemplate>
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Delete">
                                <ItemTemplate>
                                    <asp:ImageButton runat="server" CommandName="deleteCIT" CommandArgument='<%# Eval("id") %>'
                                        ToolTip="Click to delete cit" ImageUrl="../images/Delete.gif" ID="Button_deleteCIT"
                                        CssClass="ShowHand" />
                                </ItemTemplate>
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                            No Record(s) found!</EmptyDataTemplate>
                        <HeaderStyle BackColor="Black" ForeColor="Black" CssClass="HeadingCell" />
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
