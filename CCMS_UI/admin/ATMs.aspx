<%@ Page Language="C#" AutoEventWireup="true" Codebehind="ATMs.aspx.cs" Inherits="Avanza.iSuite.UI.ATMs" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
  <link href="../content/ISUITE.css" rel="stylesheet" />
  <title>CCMS - ATMs</title>

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
                     <%=Request.QueryString["i"]== null ? "ATM Management":"View ATM Inventory" %>   </td>
                    
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
                                                ATM Title :</td>
                                            <td>
                                                <asp:TextBox ID="TextBox_title" runat="server" Width="148px" CssClass="textboxNormal"></asp:TextBox>
                                            </td>
                                            <td>
                                                IP :</td>
                                            <td>
                                                <asp:TextBox ID="TextBox_ip" runat="server" CssClass="textboxNormal"></asp:TextBox></td>
                                        </tr>
                                       
                                        <tr>
                                            <td style="height: 20px">
                                                Created by :</td>
                                            <td style="height: 20px">
                                                <asp:DropDownList ID="DropDownList_creator" runat="server" Width="150px" CssClass="textbox">
                                                </asp:DropDownList>
                                            </td>
                                            <td style="height: 20px">
                                                Status :</td>
                                            <td style="height: 20px">
                                                <asp:DropDownList ID="DropDownList_status" runat="server" Style="width: 150px" CssClass="textbox">
                                                    <asp:ListItem Selected="True" Value="1">Active</asp:ListItem>
                                                    <asp:ListItem Value="0">InActive</asp:ListItem>
                                                    <asp:ListItem Value="0,1">All</asp:ListItem>
                                                </asp:DropDownList></td>
                                        </tr>
                                        <tr>
                                            <td>
                                                ATM Type :&nbsp;</td>
                                            <td>
                                                <asp:DropDownList ID="DropDownList_ATMType" runat="server" Width="151px" CssClass="textbox">
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
                                                <asp:HyperLink ID="HyperLink_newATM2" runat="server" CssClass="Link" NavigateUrl="../admin/AddEditATM.aspx">Add new ATM ... </asp:HyperLink>&nbsp;&nbsp;
                                                <asp:HyperLink ID="HyperLink_importFromIMAC" runat="server" CssClass="Link" NavigateUrl="../admin/ImportFromIMAC.aspx">Import ATM from IMAC ... </asp:HyperLink>
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
                        Selected ATMs -
                        <asp:Literal ID="Literal_count" runat="server" EnableViewState="False"></asp:Literal></th>
                </tr>
                <tr>
                    <td valign="top" colspan="2">
                        <asp:GridView ID="GridView_ATMs" runat="server" AutoGenerateColumns="False" AllowSorting="true"
                            OnSorting="GridView_ATMs_Sorting" Width="100%" OnRowCommand="GridView_ATMs_RowCommand"
                            AllowPaging="True" OnPageIndexChanging="GridView_ATMs_PageIndexChanging" OnRowDataBound="GridView_ATMs_RowDataBound"
                             PageSize="15" >
                            <AlternatingRowStyle BackColor="#eff0ef" />
                            <RowStyle HorizontalAlign="Center" />

                            <Columns>
                                <asp:BoundField DataField="title" HeaderText="Title" SortExpression="title" HeaderStyle-ForeColor="black">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>
                                <asp:BoundField DataField="atm_type" HeaderText="Type" SortExpression="atm_type"
                                    HeaderStyle-ForeColor="black">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>
                                <asp:BoundField DataField="location" HeaderText="Location" SortExpression="location" HeaderStyle-ForeColor="black">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>
                                
                                <asp:BoundField DataField="ip" HeaderText="IP Address" SortExpression="ip" HeaderStyle-ForeColor="black">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>
                                 <asp:BoundField DataField="port" HeaderText="Port" SortExpression="port" HeaderStyle-ForeColor="black">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>
                                <asp:BoundField DataField="note_set_type_name" HeaderText="Note Set Type ID" SortExpression="note_set_type_name" HeaderStyle-ForeColor="black">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>

                                 <asp:CheckBoxField DataField="is_atm" HeaderText="ATM">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:CheckBoxField>
                                 
                                 <asp:CheckBoxField DataField="is_cdm" HeaderText="BNA">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:CheckBoxField>

                                 <asp:CheckBoxField DataField="is_ccdm" HeaderText="CPM">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:CheckBoxField>
                                 
                                 <asp:BoundField DataField="latitude" HeaderText="Latitude" SortExpression="latitude" HeaderStyle-ForeColor="black">
                                        <HeaderStyle CssClass="HeadingCell" />
                                    </asp:BoundField>
                                 <asp:BoundField DataField="longitude" HeaderText="Longitude" SortExpression="longitude" HeaderStyle-ForeColor="black">
                                        <HeaderStyle CssClass="HeadingCell" />
                                 </asp:BoundField>
                                <asp:BoundField DataField="Port" HeaderText="Port" SortExpression="Port" Visible="false">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>
                                <asp:CheckBoxField DataField="is_active" HeaderText="Active">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:CheckBoxField>
                                <asp:TemplateField HeaderText="Edit" >
                                    <ItemTemplate>
                                        <%--<a href='AddEditATM.aspx?atm_id=<%# Eval("atm_id") %>'>
                                            <img alt="Click to edit atm" src="../images/Edit.gif" style="border: 0" class="ShowHand" />
                                        </a>--%>
                                        <asp:ImageButton runat="server" CommandName="editATM" CommandArgument='<%# Eval("atm_id") %>'
                                            ToolTip="Click to edit atm" ImageUrl="../images/Edit.gif" ID="Button_editATM"
                                            CssClass="ShowHand"      />
                                    </ItemTemplate>
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Delete">
                                    <ItemTemplate>
                                        <asp:ImageButton runat="server" CommandName="deleteATM" CommandArgument='<%# Eval("atm_id") %>'
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
