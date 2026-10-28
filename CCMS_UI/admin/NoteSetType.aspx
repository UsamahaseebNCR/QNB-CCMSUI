<%@ Page Language="C#" AutoEventWireup="true" Codebehind="NoteSetType.aspx.cs" Inherits="CCMSUI.admin.NoteSetType" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="../content/ISUITE.css" rel="stylesheet" />
    <title>iSuite - Note Set Type</title>

    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>



    <script type="text/javascript">

//window.parent.window.SetContentPage(window.location);
   
function confirmNoteSetTypeDeletion(title)
{
 return confirm('Are you sure you want to delete Note Set Type '+title+'?','CCMS');
}</script>

</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)' onload="hideProgressBar();">
    <form id="form1" runat="server">
        <div>
            <table style="width: 100%" cellspacing="0" cellpadding="0" border="0" class="mainbox">
                <tr>
                    <td class="PageHeading" align="left">
                        Note Set Type Management</td>
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
                <tr align="left" style="background-color: WHITE">
                    <td style="height: 20px; padding-left: 5px;" colspan="2">
                        <b>Search Filters</b></td>
                </tr>
                <tr align="left">
                    <td colspan="2">
                        <table width="100%" class="SEARCHBOX" border="0">
                            <tr>
                                <td>
                                    <table id="search" width="100%" border="0">
                                        <tr>
                                            <td>
                                                Note Set Type ID :</td>
                                            <td>
                                                <asp:TextBox ID="TextBox_NoteSetTypeID" runat="server" Width="148px" CssClass="textboxNormal"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
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
                                                <asp:HyperLink ID="HyperLink_newNoteSetType" runat="server" CssClass="Link" NavigateUrl="../admin/AddEditNoteSetType.aspx">Add new Note Set Type ... </asp:HyperLink>
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
                        Selected Note Set Types -
                        <asp:Literal ID="Literal_count" runat="server" EnableViewState="False"></asp:Literal></th>
                </tr>
                <tr>
                    <td valign="top" colspan="2">
                        <asp:GridView ID="GridView_NoteSetType" runat="server" AutoGenerateColumns="False"
                            AllowSorting="true" OnSorting="GridView_NoteSetType_Sorting" Width="100%" OnRowCommand="GridView_NoteSetType_RowCommand"
                            AllowPaging="True" OnPageIndexChanging="GridView_NoteSetType_PageIndexChanging"
                            OnRowDataBound="GridView_NoteSetType_RowDataBound"  PageSize="15"
                            HeaderStyle-CssClass="HeadingCell">
                            <AlternatingRowStyle BackColor="#eff0ef" />
                            <RowStyle HorizontalAlign="Center" />

                            
                            <Columns>

                                <asp:BoundField DataField="region_name" HeaderText="Organization" SortExpression="region_name"
                                    HeaderStyle-ForeColor="black">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>


                                <asp:BoundField DataField="Note_Set_Type_name" HeaderText="Note Set Type ID" SortExpression="Note_Set_Type_name"
                                    HeaderStyle-ForeColor="black">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>
                                
                                <asp:BoundField Visible="false" DataField="denomination_type_1_title" HeaderText="Type 1 Title" SortExpression="denomination_type_1"
                                    HeaderStyle-ForeColor="black">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>
                                
                                
                                <asp:BoundField DataField="denomination_type_1" HeaderText="Type 1" SortExpression="denomination_type_1"
                                    HeaderStyle-ForeColor="black">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>
                                
                                <asp:BoundField Visible="false" DataField="denomination_type_2_title" HeaderText="Type 2 Title" SortExpression="denomination_type_1"
                                    HeaderStyle-ForeColor="black">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>
                                
                                <asp:BoundField DataField="denomination_type_2" HeaderText="Type 2" SortExpression="denomination_type_2"
                                    HeaderStyle-ForeColor="black">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>
                                
                                <asp:BoundField Visible="false" DataField="denomination_type_3_title" HeaderText="Type 3 Title" SortExpression="denomination_type_1"
                                    HeaderStyle-ForeColor="black">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>
                                
                                
                                <asp:BoundField DataField="denomination_type_3" HeaderText="Type 3" SortExpression="denomination_type_3"
                                    HeaderStyle-ForeColor="black">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>
                                
                                
                                <asp:BoundField Visible="false" DataField="denomination_type_4_title" HeaderText="Type 4 Title" SortExpression="denomination_type_1"
                                    HeaderStyle-ForeColor="black">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>
                                
                                
                                <asp:BoundField DataField="denomination_type_4" HeaderText="Type 4" SortExpression="denomination_type_4"
                                    HeaderStyle-ForeColor="black">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>
                                
                                <asp:BoundField Visible="false" DataField="denomination_type_5_title" HeaderText="Type 5 Title" SortExpression="denomination_type_1"
                                    HeaderStyle-ForeColor="black">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>
                    
                                <asp:BoundField DataField="denomination_type_5" HeaderText="Type 5" SortExpression="denomination_type_5" Visible="false"
                                    HeaderStyle-ForeColor="black">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>
                                
                                
                                <asp:BoundField Visible="false" DataField="denomination_type_6_title" HeaderText="Type 6 Title" SortExpression="denomination_type_1" 
                                    HeaderStyle-ForeColor="black">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>
                                
                                
                                <asp:BoundField DataField="denomination_type_6" HeaderText="Type 6" SortExpression="denomination_type_6" Visible="false"
                                    HeaderStyle-ForeColor="black">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>
                                
                                <asp:BoundField Visible="false" DataField="denomination_type_7_title" HeaderText="Type 7 Title" SortExpression="denomination_type_1" 
                                    HeaderStyle-ForeColor="black">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>
                                
                                
                                <asp:BoundField DataField="denomination_type_7" HeaderText="Type 7" SortExpression="denomination_type_7" Visible="false"
                                    HeaderStyle-ForeColor="black">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>
                                
                                <asp:TemplateField HeaderText="Edit">
                                    <ItemTemplate>
                                        <a href='AddEditNoteSetType.aspx?note_set_type_id=<%# Eval("note_set_type_id") %>'>
                                            <img alt="Click to edit note set type" src="../images/Edit.gif" style="border: 0" class="ShowHand" />
                                        </a>
                                    </ItemTemplate>
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Delete">
                                    <ItemTemplate>
                                        <asp:ImageButton runat="server" CommandName="deleteNoteSetType" CommandArgument='<%# Eval("note_set_type_id") %>'
                                            ToolTip="Click to delete note set type" ImageUrl="../images/Delete.gif" ID="Button_deleteNoteSetType"
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
