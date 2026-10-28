<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ConfigureDailyFeed.aspx.cs" Inherits="CCMSUI.CCMS.ConfigureDailyFeed" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
  <link href="../content/ISUITE.css" rel="stylesheet" />
  <title>iSuite - ATMs</title>

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
                        Configure Daily Feed</td>
                    
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
                                                MCN :</td>
                                            <td>
                                                <asp:TextBox ID="TextBox_MCN" runat="server" Width="148px" CssClass="textboxNormal"></asp:TextBox>
                                            </td>
                                                                                        <td style="height: 20px">
                                                Created by :</td>
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
                        Selected MCNs -
                        <asp:Literal ID="Literal_count" runat="server" EnableViewState="False"></asp:Literal></th>
                </tr>
                <tr>
                    <td valign="top" colspan="2">
                        <asp:GridView ID="GridView_MCNs" runat="server" AutoGenerateColumns="False" AllowSorting="true"
                            OnSorting="GridView_MCNs_Sorting" Width="100%" 
                            AllowPaging="True" OnPageIndexChanging="GridView_MCNs_PageIndexChanging"
                            RowStyle-CssClass="Row" PageSize="15" HeaderStyle-CssClass="HeadingCell"
                            OnRowCommand="GridView_MCNs_RowCommand">
                            <RowStyle HorizontalAlign="Center" CssClass="Row" />
                            <Columns>
                                <asp:BoundField DataField="mcn" HeaderText="Master Customer Number" SortExpression="mcn" HeaderStyle-ForeColor="black">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>
                          
                                <asp:TemplateField HeaderText="Edit">
                                    <ItemTemplate>
                                        <%--<a href='ConfigureDailyFeedStep2.aspx?mcn=<%# Eval("mcn") %>'>
                                            <img alt="Click to configure mcn" src="../images/Edit.gif" style="border: 0" class="ShowHand" />
                                        </a>--%>
                                        <asp:ImageButton ID="imgbtn_editMCN" CommandName="editMCN" ImageUrl="../images/Edit.gif"  runat="server" CommandArgument='<%# Eval("mcn") %>'
                                    ToolTip='Click to edit MCN' CssClass="ShowHand" />
                                    </ItemTemplate>
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:TemplateField>
                            </Columns>
                            <EmptyDataTemplate>
                                No Organizations found!</EmptyDataTemplate>
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
