<%@ Page Language="C#" AutoEventWireup="true" Codebehind="ProblemBucketDescription.aspx.cs" Inherits="Avanza.iSuite.UI.ProblemBucketDescription" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
  <link href="../content/ISUITE.css" rel="stylesheet" />
  <title>CCMS - Problem Bucket Description</title>

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
                     
                     Setup Problem Bucket  </td>
                    
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
                                                Problem bucket description :</td>
                                            <td>
                                                <asp:TextBox ID="TextBox_title" runat="server" Width="148px" CssClass="textboxNormal"></asp:TextBox>
                                            </td>
                                            <td>
                                                Priority ID :</td>
                                            <td>
                                                <asp:TextBox ID="TextBox_ip" runat="server" CssClass="textboxNormal"></asp:TextBox></td>
                                        </tr>
                                       
                                        <tr>
                                            <td style="height: 20px">
                                                Status :</td>
                                            <td style="height: 20px">
                                                <asp:TextBox ID="TextBox_Status" runat="server" CssClass="textboxNormal"></asp:TextBox>
                                            </td>
                                            <td style="height: 20px">
                                                Category :</td>
                                            <td style="height: 20px">
                                                <asp:TextBox ID="TextBox_Category" runat="server" CssClass="textboxNormal"></asp:TextBox>
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
                        Selected -
                        <asp:Literal ID="Literal_count" runat="server" EnableViewState="False"></asp:Literal></th>
                </tr>
                <tr>
                    <td valign="top" colspan="2">
                        <asp:GridView ID="GridView_ATMs" runat="server" AutoGenerateColumns="False" AllowSorting="true"
                            OnSorting="GridView_ATMs_Sorting" Width="100%" OnRowCommand="GridView_ATMs_RowCommand"
                            AllowPaging="True" OnPageIndexChanging="GridView_ATMs_PageIndexChanging" 
                             PageSize="15" >
                            <AlternatingRowStyle BackColor="#eff0ef" />
                            <RowStyle HorizontalAlign="Center" />

                            <Columns>
                                <asp:BoundField DataField="problem_bucket_desc" HeaderText="Problem Bucket Description" SortExpression="problem_bucket_desc" HeaderStyle-ForeColor="black"
                                ItemStyle-HorizontalAlign = "Left">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>
                                <asp:BoundField DataField="problem_bucket_priority_id" HeaderText="Proirity ID" SortExpression="problem_bucket_priority_id"
                                    HeaderStyle-ForeColor="black">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>
                                <asp:BoundField DataField="status_name" HeaderText="Status" SortExpression="status_name" HeaderStyle-ForeColor="black">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>
                                
                                 <asp:BoundField DataField="category" HeaderText="Category" SortExpression="category" HeaderStyle-ForeColor="black">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>
                             
                                <asp:TemplateField HeaderText="Edit">
                                    <ItemTemplate>
                                        <%--<a href='AddEditATM.aspx?atm_id=<%# Eval("atm_id") %>'>
                                            <img alt="Click to edit atm" src="../images/Edit.gif" style="border: 0" class="ShowHand" />
                                        </a>--%>
                                        <asp:ImageButton runat="server" CommandName="editATM" CommandArgument='<%# Eval("problem_bucket_desc_id") %>'
                                            ToolTip="Click to edit atm" ImageUrl="../images/Edit.gif" ID="Button_editATM"
                                            CssClass="ShowHand"      />
                                    </ItemTemplate>
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Delete">
                                    <ItemTemplate>
                                        <asp:ImageButton runat="server" CommandName="deleteATM" CommandArgument='<%# Eval("problem_bucket_desc_id") %>'
                                            ToolTip="Click to delete atm" ImageUrl="../images/Delete.gif" ID="Button_deleteATM"
                                            CssClass="ShowHand" />
                                    </ItemTemplate>
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:TemplateField>
                            </Columns>
                            <EmptyDataTemplate>
                                No ATM found!</EmptyDataTemplate>
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
