<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ApproveGroup.aspx.cs" Inherits="Avanza.iSuite.UI.ApproveGroup" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="../content/iSuite.css" rel="stylesheet" />

    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>

    <script type="text/javascript">
        window.parent.window.contentPage = '';
        function openDialogAndGetRightList(groupId, isUser, isLink) {
            Dialog1.Show();
            DialogCallBack.callback(groupId, isUser, isLink);
        }
        function receiveCallBackResult(rightList) {
            document.getElementById('userRightList').innerHTML = rightList;
        }

    </script>

    <%--<script src="../content/main.js" type="text/javascript" language="javascript">
</script>
--%>
</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)' onload="hideProgressBar();">
    <form id="form1" runat="server">
        <div>
            <table style="width: 100%" cellspacing="0" cellpadding="0" border="0" class="mainbox">
                <tr>
                    <td class="PageHeading" align="left">
                        Group Approval</td>
                    
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
               
                <tr align="left" style="background-color:WHITE">
                    <td style="height: 20px; padding-left:5px;" colspan="2"  >
                        <b>Search Filters</b></td>
                </tr>
                <tr align="left" >
                    <tD colspan="2" >
                        <table width="100%" class ="SEARCHBOX" border="0"  >
                            <tr>
                                <td>
                                    <table id="search" width="90%" border="0">
                                        <tr>
                                            <td>
                                                Group Name :</td>
                                            <td>
                                                <asp:TextBox ID="TextBox_groupname" runat="server" CssClass="textboxNormal"></asp:TextBox>
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
                                                    Width="71px" OnClick="Button_reset_Click" />&nbsp; &nbsp;
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
                    </tD>
                </tr>
                <tr>
                    <td align="left">
                        &nbsp; &nbsp; &nbsp;&nbsp;
                    </td>
                </tr>
                <tr align="left">
                    <th colspan="2" align="left">
                        Group Listing
                        </th>
                </tr>
                <tr>
                    <td valign="top" colspan="2">
                        <asp:GridView ID="GridView_approveGroups" runat="server" AutoGenerateColumns="False" 
                            Width="100%" OnRowCommand="GridView_approveGroups_RowCommand"
                            >
                             <AlternatingRowStyle BackColor="#eff0ef" />
                            <RowStyle HorizontalAlign="Center" />
                            <Columns>
                            <asp:TemplateField>
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                <HeaderTemplate>
                                    Group Name
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:HyperLink ID="HyperLink_GroupName" runat="server" Text='<%#Eval("group_name") %>'></asp:HyperLink>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <%--                            <asp:HyperLinkField DataNavigateUrlFields="GroupId" DataNavigateUrlFormatString="AddEditGroup.aspx?group_id={0}"
                                DataTextField="GroupName" HeaderText="Group Name"  >
                                
                            </asp:HyperLinkField>
--%>
                            <asp:BoundField DataField="Description" HeaderText="Description">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Status" HeaderText="Status">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Request_Mode" HeaderText="Request_Mode">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:TemplateField>
                             <HeaderTemplate>
                                    View
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:ImageButton ID="imgbtn_editgroup" CommandName="ViewGroup" ImageUrl="../images/Edit.gif"  runat="server" CommandArgument='<%# Eval("group_name") %>'
                                    ToolTip='Click to View Group' CssClass="ShowHand" />
                                </ItemTemplate>
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:TemplateField>
                            
                        </Columns>
                            <EmptyDataTemplate>
                                No Group(s) found!</EmptyDataTemplate>
                            <HeaderStyle BackColor="Black" ForeColor="black" CssClass="HeadingCell" />
                        </asp:GridView>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                <td style="height: 54px">
                    
                </td>
            </tr>
            </table>
           
        </div>
        
    </form>
</body>
</html>

