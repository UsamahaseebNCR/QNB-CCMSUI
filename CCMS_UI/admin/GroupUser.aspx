<%@ Page Language="C#" AutoEventWireup="true" Codebehind="GroupUser.aspx.cs" Inherits="Avanza.iSuite.UI.GroupUser" %>

<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="../content/iSuite.css" rel="stylesheet" />

    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>

    <script type="text/javascript">
		window.parent.window.contentPage = '';   
   function openDialogAndGetRightList(groupId,isUser,isLink)
    {
      Dialog1.Show();
      DialogCallBack.callback(groupId,isUser,isLink);   
    }
    function receiveCallBackResult(rightList) 
    {
        document.getElementById ('userRightList').innerHTML = rightList;
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
                        Group Management</td>
                    
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
                                                    Width="71px" OnClick="Button_reset_Click" />&nbsp; &nbsp;<asp:Button ID="Button_ExportToPDF" runat="server" CssClass="button"
                                                    OnClick="Button_ExportToPDF_Click" Text="Export To PDF" />
                                            &nbsp; &nbsp;<asp:Button ID="Button_ExportExcel" runat="server" CssClass="button" OnClick="Button_ExportToExcel_Click"
                                                Text="Export To Excel" />
                                                <asp:HyperLink ID="HyperLink_newGroup" runat="server" CssClass="Link" NavigateUrl="AddEditGroup.aspx">Add a new Group ... </asp:HyperLink>
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
                        <asp:GridView ID="GridView_groups" runat="server" AutoGenerateColumns="False" 
                            Width="100%" OnRowCommand="GridView_groups_RowCommand"
                             OnRowDataBound="GridView_groups_RowDataBound"
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
                                    <asp:HyperLink ID="HyperLink_GroupName" runat="server" Text='<%#Eval("GroupName") %>'></asp:HyperLink>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <%--                            <asp:HyperLinkField DataNavigateUrlFields="GroupId" DataNavigateUrlFormatString="AddEditGroup.aspx?group_id={0}"
                                DataTextField="GroupName" HeaderText="Group Name"  >
                                
                            </asp:HyperLinkField>
--%>
                            <asp:BoundField DataField="Description" HeaderText="Description">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Status" HeaderText="Approval Status">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:TemplateField HeaderText="Delete Group" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:ImageButton runat="server" CommandName="deleteGroup" CommandArgument='<%# Eval("GroupId") %>'
                                        ToolTip="Click to delete this group" ImageUrl="~/images/Delete.gif" ID="Button_deleteUserGroup"
                                        CausesValidation="false" CssClass="ShowHand"></asp:ImageButton>
                                </ItemTemplate>
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:TemplateField>
                            <%--                            <asp:TemplateField ShowHeader="False" HeaderText="Users in groups" ItemStyle-HorizontalAlign ="Center" HeaderStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:Repeater runat="server" ID="userList">
                                        <HeaderTemplate>
                                            <div style="overflow: auto; height: 25px">
                                                <ul>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <li><a href='AddEditUser.aspx?u=<%#Eval("user_id") %>' style="border: 0">
                                                <img style="border: 0" width="24px" src="../images/user.jpg" alt="click to edit this user" /><%#Eval("user_full_name")%>
                                            </a></li>
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            </ul> </div>
                                        </FooterTemplate>
                                    </asp:Repeater>
                                </ItemTemplate>
                                <HeaderStyle CssClass="HeadingCell" ForeColor="White" />
                            </asp:TemplateField>
--%>
                            <asp:TemplateField ShowHeader="False" HeaderText="Group Users" ItemStyle-HorizontalAlign="Center"
                                HeaderStyle-HorizontalAlign="Center" HeaderStyle-CssClass="HeadingCell" >
                                <ItemTemplate>
                                    <div id="ShowHideUsersIcon" runat="server">
                                        <img style="border: 0" width="20px" src="../images/user.jpg" alt="click to view users"
                                            onclick="openDialogAndGetRightList(<%#Eval("GroupId") %>,1,1)" class="ShowHand" />
                                    </div>
                                </ItemTemplate>
                                <HeaderStyle ForeColor="black" />
                            </asp:TemplateField>
                            <asp:TemplateField ShowHeader="False" HeaderText="Group Rights" ItemStyle-HorizontalAlign="Center"
                                HeaderStyle-HorizontalAlign="Center"  HeaderStyle-CssClass="HeadingCell">
                                <ItemTemplate>
                                    <div id="ShowHideRightsIcon" runat="server">
                                        <img style="border: 0" width="20px" src="../images/rights.jpg" alt="click to view rights"
                                            onclick="openDialogAndGetRightList(<%#Eval("GroupId") %>,0,0)" class="ShowHand" />
                                    </div>
                                </ItemTemplate>
                                <HeaderStyle ForeColor="black" />
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
             <ComponentArt:CallBack ID="DialogCallBack" runat="server" Width="0" Height="0" OnCallback="DialogCallBack_Callback">
        </ComponentArt:CallBack>
       <ComponentArt:Dialog ContentCssClass="contentCss" FooterCssClass="footerCss" HeaderCssClass="headerCss" 
                        CssClass="dialogCss" Icon="" Value="" Title="AJAX TabStrip Content" CloseTransition="Fade"
                        ModalMaskCssClass="modalMaskCssClass" AllowDrag="false" Alignment="TopCentre"                          
                               Modal="true"                ID="Dialog1" runat="server"  Width="610" ContentClientTemplateId="data"
                        AnimationType="None" AnimationPath="Direct" AnimationSlide="Linear" ShowTransition="Fade"
                        TransitionDuration="0" AnimationDuration="0">
                        <ClientTemplates>
                            <ComponentArt:ClientTemplate ID="data">
                                <table cellpadding="0" style="border-color:#c9c9c9" cellspacing="0" border="1" width="100%" onmousedown="Dialog1.StartDrag(event);">
                                    <tr>
                                        
                                        <td style="background-image: url(../images/DialogImages/top.gif); height:45px;"; >
                                            <table width="100%" cellpadding="0" cellspacing="0" border="0">
                                                <tr>
                                                    <td valign="middle" style="padding-left:4px; color: white; font-size: 15px; font-family:Verdana; font-weight: bold;">
                                                        Information</td>
                                                    <td align="right" style="padding-right:10px;">
                                                        <img src="../images/DialogImages/close.gif" onclick="Dialog1.Close();" /></td>
                                                </tr>
                                            </table>
                                        </td>
                                        
                                    </tr>
                                    <tr>
                                       
                                        <td style="  border-color:#c9c9c9; background-color: white; font-size: 12px; font-family: Arial; padding: 10px;">
                                            <div id="userRightList">
                                            </div>
                                        </td>
                                       
                                    </tr>
                                  
                                </table>
                            </ComponentArt:ClientTemplate>
                        </ClientTemplates>
                    </ComponentArt:Dialog>
        </div>
        
    </form>
</body>
</html>
