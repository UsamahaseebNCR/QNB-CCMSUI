<%@ Page Language="C#" AutoEventWireup="true" Codebehind="RemoteCommand.aspx.cs" Inherits="CCMSUI.CCMS.RemoteCommand" %>

<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
   <link href="../content/iSuite.css" rel="stylesheet" />
 	<title>View360 - Remote Command</title>

	<script src="../content/shortcut.js" type="text/javascript" language="javascript"></script>

	<script language="javascript" type="text/javascript">
        window.parent.window.contentPage = '';

        function check(sender, eventArgs) 
        {
            TreeView_ATM.beginUpdate();
            setCheck(eventArgs.get_node(), eventArgs.get_node().get_checked());
            TreeView_ATM.endUpdate();
        }
        
        function setCheck(node, selected) 
        {
            var selectedChildren = 0;
            var childary = node.get_nodes();

            for (var i = 0; i < childary.get_length(); i++) 
            {
                if (childary.getNode(i).get_id().substring(0, 1) == 'r') 
                {
                    setCheck(childary.getNode(i), selected);

                    if (selectedChildren == i)
                        childary.getNode(i).set_checked(selected);
                }
                else
                    childary.getNode(i).set_checked(selected);
                
                selectedChildren++;
            }
        }
        
        function CheckAll() 
        {
            var inputs = document.forms[0].elements;
        
            for (var i = 0; i < inputs.length; i++)
                inputs[i].checked = true;
        }

        function UncheckAll() 
        {
            var inputs = document.forms[0].elements;
        
            for (var i = 0; i < inputs.length; i++)
                inputs[i].checked = false;
        }
	</script>

</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)' onload="hideProgressBar();">
	<form id="form1" runat="server">
		<div>
			<table style="width: 100%" cellspacing="0" cellpadding="0" border="0" class="mainbox">
				 
				  <tr>
                <td class="PageHeading" align="left" colspan="4">
                    Remote Comamnd
                </td>
            </tr>
				<tr>
					<td class="ValidationText" align="center" colspan="4">
						<asp:Literal ID="Literal_error" runat="server" EnableViewState="False"></asp:Literal>
						<asp:ValidationSummary ID="ValidationSummary_ATM" runat="server" />
					</td>
				</tr>
				<tr>
					<td class="ConfirmationTextClass" align="center">
						<%=Session[SessionVars.notice.ToString()]%>
						<%Session[SessionVars.notice.ToString()] = "";%>
						<asp:Literal ID="Literal_ok" runat="server" EnableViewState="False"></asp:Literal>
					</td>
				</tr>
                <tr>
                <td>
                  <asp:Label ID="Label1" runat="server" Text="Currently NCR ATMs support remote commands" ForeColor = "Green"></asp:Label>
                    <br />
                 <br />
                </td>
                </tr>
               
				<tr style="width: 100%">
					<td class="SearchFiltersPaneClasstbl">
						<table style="width: 100%">
							<tr>
								<td style="width: 15%;">
									Command</td>
								<td style="width: 85%">
									<asp:DropDownList ID="DropDownList_command" runat="server" CssClass="dropDown">
										<asp:ListItem Text="In Service" Value="InService" Selected="True"></asp:ListItem>
										<asp:ListItem Text="Out of Service" Value="OutOfService"></asp:ListItem>
										<asp:ListItem Text="Restart" Value="Restart"></asp:ListItem>
									</asp:DropDownList>
                                    
                                  

								</td>
                              
							</tr>
							<tr>
								<td style="vertical-align: top;">
									ATMs</td>
								<td>
									<a href="#" id="button_checkAll" class="HyperlinkClass" onclick="CheckAll();">Check All</a>&nbsp;&nbsp; 
									<a href="#" id="button_uncheckAll" class="HyperlinkClass" onclick="UncheckAll();">Uncheck All</a>&nbsp;&nbsp;
									<div id="Div_TreeView" runat="server" class="DivClass" style="width: 80%; font-weight: normal; border: solid 1px #d3d3d3; max-height: 300px;">
										<ComponentArt:TreeView ID="TreeView_ATM" runat="server" EContentLoadingImageUrl="spinner.gif"
                                                CssClass="TreeViewMargin" NodeCssClass="TreeNode" HoverNodeCssClass="HoverTreeNode"
                                                MultipleSelectedNodeCssClass="MultipleSelectedTreeNode" SelectedNodeCssClass="SelectedTreeNode"
                                                ImagesBaseUrl="../images/" LineImagesFolderUrl="../images/lines/" DefaultImageHeight="16"
                                                DefaultImageWidth="16" DefaultMarginImageHeight="20" DefaultMarginImageWidth="25"
                                                LineImageHeight="20" LineImageWidth="20" DisplayMargin="true" ShowLines="true"
                                                LoadingFeedbackText="Loading ATMs ..." MultipleSelectEnabled="true" NodeLabelPadding="3"
                                                CausesValidation="False" EnableViewState="true" Height="180px">
											<ClientEvents>
												<NodeCheckChange EventHandler="check" />
											</ClientEvents>
										</ComponentArt:TreeView>
									</div>
								</td>
							</tr>
							<tr>
								<td class="SearchButtonsPaneClass" colspan="2">
									<asp:Button ID="Button_send" runat="server" CssClass="button" Text="Send" OnClick="Button_send_Click" />&nbsp;&nbsp;
									<input id="Button_cancel" type="button" value="Back" onclick="window.location='../Home.aspx'" class="button" />
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</div>
	</form>
</body>
</html>
