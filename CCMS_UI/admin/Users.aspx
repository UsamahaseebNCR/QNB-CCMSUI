<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Users.aspx.cs" Inherits="Avanza.iSuite.UI.User" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="../content/ISUITE.css" rel="stylesheet" />
    <title>iSuite - ATMs</title>
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
    <script type="text/javascript">

        //window.parent.window.SetContentPage(window.location);
   
    </script>
</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)'
    onload="hideProgressBar();">
    <form id="form1" runat="server">
    <div>
        <table style="width: 100%" cellspacing="0" cellpadding="0" border="0" class="mainbox">
            <tr>
                <td class="PageHeading" align="left">
                    User Management
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
                                        <td>
                                            User ID :
                                        </td>
                                        <td>
                                            <asp:TextBox ID="TextBox_userId" runat="server" CssClass="textboxNormal"></asp:TextBox>
                                        </td>
                                        <td>
                                            Full Name :
                                        </td>
                                        <td>
                                            <asp:TextBox ID="TextBox_fullname" runat="server" CssClass="textboxNormal"></asp:TextBox>
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
                                                Width="71px" OnClick="Button_reset_Click" />&nbsp; &nbsp; &nbsp;<asp:Button ID="Button_ExportToPDF"
                                                    runat="server" CssClass="button" OnClick="Button_ExportToPDF_Click" Text="Export To PDF" />
                                            &nbsp; &nbsp;<asp:Button ID="Button1" runat="server" CssClass="button" OnClick="Button_ExportToExcel_Click"
                                                Text="Export To Excel" />
                                            <asp:HyperLink ID="HyperLink_newUser" runat="server" CssClass="Link" NavigateUrl="../admin/AddEditUser.aspx">Add a new User ... </asp:HyperLink>
                                            &nbsp;<asp:HyperLink ID="HyperLink_AddADUser" runat="server" CssClass="Link" NavigateUrl="../admin/LoginToActiveDir.aspx">Add Active Diretory User ... </asp:HyperLink>
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
                    Selected Users -
                    <asp:Literal ID="Literal_count" runat="server" EnableViewState="False"></asp:Literal>
                </th>
            </tr>
            <tr>
                <td valign="top" colspan="2">
                    <asp:GridView ID="GridView_users" runat="server" AutoGenerateColumns="False" AllowSorting="true"
                        OnSorting="GridView_users_Sorting" Width="100%" OnRowCommand="GridView_users_RowCommand"
                        AllowPaging="True" OnPageIndexChanging="GridView_users_PageIndexChanging" OnRowDataBound="GridView_users_RowDataBound"
                        PageSize="15">
                        <RowStyle HorizontalAlign="Center" />
                        <AlternatingRowStyle BackColor="#eff0ef" />
                        <Columns>
                            <asp:TemplateField SortExpression="user_login">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                <HeaderTemplate>
                                    User Id
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:HyperLink ID="HyperLink_UserId" runat="server" Text='<%#Eval("user_login") %>'></asp:HyperLink>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="user_full_name" HeaderText="Full Name" SortExpression="user_full_name">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="user_email" HeaderText="Email" SortExpression="user_email">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                             <asp:BoundField DataField="approval_status" HeaderText="Approval Status" SortExpression="approval_status">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    Status
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:ImageButton ID="imgbtn_activate" runat="server" CommandName="activateUser" CommandArgument='<%# Eval("user_id") %>'
                                        ToolTip='Click to activate user' CssClass="ShowHand" />
                                    <asp:ImageButton ID="imgbtn_deActivate" runat="server" CommandName="deactivateUser"
                                        CommandArgument='<%# Eval("user_id") %>' ToolTip='Click to deactivate user' CssClass="ShowHand" />
                                </ItemTemplate>
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:TemplateField>
                           
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    Delete
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:ImageButton ID="imgbtn_delete" runat="server" CommandName="deleteUser" CommandArgument='<%# Eval("user_id") %>'
                                        ToolTip='Click to delete user' ImageUrl="../images/deactivate.gif" CssClass="ShowHand" />
                                </ItemTemplate>
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:TemplateField>
                             <asp:TemplateField>
                             <HeaderTemplate>
                                    Edit
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:ImageButton ID="imgbtn_edituser" CommandName="editUser" ImageUrl="../images/Edit.gif"  runat="server" CommandArgument='<%# Eval("user_id") %>'
                                    ToolTip='Click to edit user' CssClass="ShowHand" />
                                </ItemTemplate>
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                            No User(s) found!</EmptyDataTemplate>
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
