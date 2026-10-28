<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProblemBucketPriority.aspx.cs"
    Inherits="Avanza.iSuite.UI.ProblemBucketPriority" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="../content/ISUITE.css" rel="stylesheet" />
    <title>CCMS - Problem Bucket Priority</title>
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
    <script type="text/javascript">
   
    </script>
</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)'
    onload="hideProgressBar();">
    <form id="form1" runat="server">
    <div>
        <table style="width: 100%" cellspacing="0" cellpadding="0" border="0" class="mainbox">
            <tr>
                <td class="PageHeading" align="left">
                    Setup Priority
                </td>
            </tr>
            <tr>
                <td class="ValidationText" align="center">
                    <asp:literal id="Literal_error" runat="server" enableviewstate="False"></asp:literal>
                </td>
            </tr>
            <tr>
                <td class="ConfirmationText" align="center">
                    <%=Session[SessionVars.notice.ToString()]%>
                    <%Session[SessionVars.notice.ToString()] = "";%>
                    <asp:literal id="Literal_ok" runat="server" enableviewstate="False"></asp:literal>
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
                                            Description :
                                        </td>
                                        <td>
                                            <asp:textbox id="TextBox_title" runat="server" width="148px" cssclass="textboxNormal"></asp:textbox>
                                        </td>
                                        <td>
                                            Priority :
                                        </td>
                                        <td>
                                            <asp:textbox id="TextBox_ip" runat="server" cssclass="textboxNormal"></asp:textbox>
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
                                            <asp:button id="Button_search" runat="server" cssclass="button" onclick="Button_search_Click"
                                                text="Search" width="71px" />
                                            &nbsp;<asp:button id="Button_reset" runat="server" cssclass="button" text="Reset"
                                                width="71px" onclick="Button_reset_Click" />&nbsp;
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
                    Selected -
                    <asp:literal id="Literal_count" runat="server" enableviewstate="False"></asp:literal>
                </th>
            </tr>
            <tr>
                <td valign="top" colspan="2">
                    <asp:gridview id="GridView_ProblemBucketPriority" runat="server" autogeneratecolumns="False" OnRowCommand="GridView_ProblemBucketPriority_RowCommand"
                    width="100%"
                        pagesize="15">
                            <AlternatingRowStyle BackColor="#eff0ef" />
                            <RowStyle HorizontalAlign="Center" />

                            <Columns>
                                <asp:BoundField DataField="problem_desc" HeaderText="Problem Type" SortExpression="problem_desc" HeaderStyle-ForeColor="black" 
                                ItemStyle-HorizontalAlign="Left">
                                    <HeaderStyle CssClass="HeadingCell"  />
                                </asp:BoundField>
                                <asp:BoundField DataField="problem_priority" HeaderText="Priority" SortExpression="problem_priority"
                                    HeaderStyle-ForeColor="black">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>
                               

                                <asp:TemplateField HeaderText="Edit">
                                    <ItemTemplate>
                                        <%--<a href='AddEditATM.aspx?atm_id=<%# Eval("atm_id") %>'>
                                            <img alt="Click to edit atm" src="../images/Edit.gif" style="border: 0" class="ShowHand" />
                                        </a>--%>
                                        <asp:ImageButton runat="server" CommandName="editATM" CommandArgument='<%# Eval("problem_bucket_priority_id") %>'
                                            ToolTip="Click to edit atm" ImageUrl="../images/Edit.gif" ID="Button_editPriority"
                                            CssClass="ShowHand"      />
                                    </ItemTemplate>
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Delete">
                                    <ItemTemplate>
                                        <asp:ImageButton runat="server" CommandName="deleteATM" CommandArgument='<%# Eval("problem_bucket_priority_id") %>'
                                            ToolTip="Click to delete atm" ImageUrl="../images/Delete.gif" ID="Button_deletePriority"
                                            CssClass="ShowHand" />
                                    </ItemTemplate>
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:TemplateField>

                            </Columns>
                            <EmptyDataTemplate>
                                No ATM found!</EmptyDataTemplate>
                            <HeaderStyle BackColor="Black" ForeColor="White" CssClass="HeadingCell" />
                        </asp:gridview>
                    &nbsp;
                </td>
            </tr>
        </table>
    </div>
    <input type="button" runat="server" id="refresh" style="display: none" />
    </form>
</body>
</html>
