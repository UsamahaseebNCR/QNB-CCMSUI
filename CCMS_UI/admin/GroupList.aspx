<%@ Page Language="C#" AutoEventWireup="true" Codebehind="GroupList.aspx.cs" Inherits="Avanza.iSuite.UI.GroupList" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Groups</title>
	<link href="../content/iSuite.css" rel="stylesheet" />
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
	<script type="text/javascript">
		window.parent.window.contentPage = '';   
	</script>
</head>
<body onload="hideProgressBar();">
	<form id="form1" runat="server">
		<table style="width: 100%">
		<tr>
                <td class="PageHeading" align="left">
                    Group Listing
                </td>
            </tr>
			<tr>
				<td style="color: red; text-align: center">
					<asp:Literal ID="Literal_error" runat="server" EnableViewState="False"></asp:Literal>
				</td>
			</tr>
			<tr>
				<td style="color: navy; text-align: center">
					<asp:Literal ID="Literal_ok" runat="server" EnableViewState="False"></asp:Literal></td>
			</tr>
			<tr>
                <th align="left" style="padding-right: 30px">
                    Search Filter
                </th>
            </tr>
            <tr>
                <td align="left" style="padding-right: 30px">
                    <table id="Table3" border="0" cellpadding="0" cellspacing="0" style="padding-left: 8px;
                        width: 100%">
                        <tr>
                            <td style="width: 40px">
                                &nbsp;&nbsp; &nbsp;Group Name :&nbsp;
                            </td>
                            <td>
                                <asp:TextBox ID="TextBox_userId" runat="server" CssClass="textbox"></asp:TextBox></td>
                            <td>
                                </td>
                            <td>
                                </td>
                        </tr>
                        <tr>
                            <td align="right" colspan="4" style="height: 20px">
                                <asp:Button ID="Button_search" runat="server" CssClass="button" OnClick="Button_search_Click"
                                    Text="Search" />
                                <asp:Button ID="Button_reset" runat="server" CssClass="button" OnClick="Button_reset_Click"
                                    Text="Reset" />
                                &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; 
                                <asp:HyperLink ID="HyperLink_newGroup" runat="server" CssClass="Link" NavigateUrl="../admin/AddEditGroup.aspx">Add new Group ... </asp:HyperLink>
					
                                &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&nbsp;</td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr align="left">
                <th>
                    Group Listing -
                    <asp:Literal ID="Literal_count" runat="server" EnableViewState="False"></asp:Literal></th>
            </tr>
			<tr>
				<td>
					<asp:GridView ID="GridView_groups" runat="server" AutoGenerateColumns="False"  
					  Width="100%" OnRowCommand="GridView_groups_RowCommand"
						OnRowDataBound="GridView_groups_RowDataBound"  
						GridLines="None" CellPadding="0" OnSelectedIndexChanged="GridView_groups_SelectedIndexChanged">
						<Columns>
							<asp:HyperLinkField DataNavigateUrlFields="GroupId" DataNavigateUrlFormatString="AddEditGroup.aspx?group_id={0}"
								DataTextField="GroupName" HeaderText="Group Name" />
							<asp:BoundField DataField="Description" HeaderText="Group Description" />
							<asp:TemplateField HeaderText="Delete Group">
								<ItemTemplate>
									<asp:ImageButton runat="server" CommandName="deleteGroup" CommandArgument='<%# Eval("GroupId") %>'
										ToolTip="Click to delete this group" ImageUrl="~/images/Delete.gif" ID="Button_deleteUserGroup"
										CausesValidation="false"></asp:ImageButton>
								</ItemTemplate>
							</asp:TemplateField>
							<asp:TemplateField ShowHeader="False" HeaderText="Group Users">
								<ItemTemplate>
									<asp:Repeater runat="server" ID="userList">
										<HeaderTemplate>
											<ul>
										</HeaderTemplate>
										<ItemTemplate>
											<li><a href='AddEditUser.aspx?userid=<%#Eval("user_id") %>' style="border: 0">
												<img style="border: 0" src="../images/user2small.gif" alt="click to edit this user" /><%#Eval("user_full_name")%>
											</a></li>
										</ItemTemplate>
										<FooterTemplate>
											</ul></FooterTemplate>
									</asp:Repeater>
								</ItemTemplate>
							</asp:TemplateField>
							<asp:TemplateField ShowHeader="False" HeaderText="Group Rights">
								<ItemTemplate>
									<asp:Repeater runat="server" ID="rightList">
										<HeaderTemplate>
											<ul>
										</HeaderTemplate>
										<ItemTemplate>
											<li>
												<%#Eval("name") %>
											</li>
										</ItemTemplate>
										<FooterTemplate>
											</ul></FooterTemplate>
									</asp:Repeater>
								</ItemTemplate>
							</asp:TemplateField>
						</Columns>
						<HeaderStyle HorizontalAlign="Left" />
					</asp:GridView>
				</td>
			</tr>
			<tr>
				<td>
                    &nbsp;</td>
			</tr>
		</table>
	</form>
</body>
</html>
