<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Classification.aspx.cs" Inherits="CCMSUI.admin.Classification" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="../content/ISUITE.css" rel="stylesheet" />
    <title>CCMS-Classification</title>
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
    <script type="text/javascript">

        //window.parent.window.SetContentPage(window.location);

        function confirmDeletion(title) {
            return confirm('Are you sure you want to delete File Type ' + title + '?', 'CCMS');
        }</script>
</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)' onload="hideProgressBar();">
    <form id="form1" runat="server">
    <div>
        <table style="width: 100%" cellspacing="0" cellpadding="0" border="0" class="mainbox">
            <tr>
                <td class="PageHeading" align="left">
                    ATM Classification Management
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
                                            Name :
                                        </td>
                                        <td>
                                            <asp:TextBox ID="TextBox_name" runat="server" Width="148px" CssClass="textboxNormal"></asp:TextBox>
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
                                            <asp:HyperLink ID="HyperLink_newClassfication" runat="server" CssClass="Link" NavigateUrl="../admin/AddEditATMClassification.aspx">Add new Classfication ... </asp:HyperLink>
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
                    Selected ATM Classification -
                    <asp:Literal ID="Literal_count" runat="server" EnableViewState="False"></asp:Literal>
                </th>
            </tr>
            <tr>
                <td valign="top" colspan="2">
                    <asp:GridView ID="GridView_Classification" runat="server" AutoGenerateColumns="False" AllowSorting="true"
                        OnSorting="GridView_Classification_Sorting" Width="100%" OnRowCommand="GridView_Classification_RowCommand"
                        AllowPaging="True" OnPageIndexChanging="GridView_Classification_PageIndexChanging" OnRowDataBound="GridView_Classification_RowDataBound"
                        PageSize="15">
                        <AlternatingRowStyle BackColor="#eff0ef" />
                        <RowStyle HorizontalAlign="Center" />
                        <Columns>
                            <asp:BoundField DataField="classification_name" HeaderText="Classification Name" SortExpression="classification_name"
                                HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>

                            <asp:BoundField DataField="creation_time" HeaderText="Creation Time" SortExpression="creation_time"
                                HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                           
                            <asp:BoundField DataField="user_full_name" HeaderText="Created By" SortExpression="user_full_name"
                                HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                           


                            <asp:TemplateField HeaderText="Edit">
                                <ItemTemplate>
                                    <a href='AddEditATMClassification.aspx?id=<%# Eval("classification_id") %>'>
                                        <img alt="Click to edit atm classfication" src="../images/Edit.gif" style="border: 0" class="ShowHand" />
                                    </a>
                                </ItemTemplate>
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Delete">
                                <ItemTemplate>
                                    <asp:ImageButton runat="server" CommandName="deleteClassification" CommandArgument='<%# Eval("classification_id") %>'
                                        ToolTip="Click to delete atm classfication" ImageUrl="../images/Delete.gif" ID="Button_deleteClassification"
                                        CssClass="ShowHand" />
                                </ItemTemplate>
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                            No ATM Classification Found!</EmptyDataTemplate>
                        <HeaderStyle BackColor="Black" ForeColor="White" CssClass="HeadingCell" />
                    </asp:GridView>
                    &nbsp;
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
