<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddEditGroup.aspx.cs" Inherits="Avanza.iSuite.UI.AddEditGroup"
    ValidateRequest="false" Title="iDistribute - Add Edit Group" %>

<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>iSuite - Add Edit Group</title>
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
    <script type="text/javascript">
        window.parent.window.contentPage = '';
        //		function setCheck(val)
        //		{
        //		    var inputs = document.getElementsByTagName('input');
        //		    for (var i = 0; i < inputs.length; i++)
        //            {
        //            var typ = inputs[i].type;
        //            if (typ=="checkbox")
        //                inputs[i].checked = val;    
        //            }
        //		}
    </script>
    <link href="../content/iSuite.css" rel="stylesheet" />
</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)'
    onload="hideProgressBar();">
    <form id="form1" runat="server">
    <table cellpadding="0" cellspacing="0" border="0" class="MainBox" width="100%">
        <tr>
            <td class="PageHeading" align="left">
                <%=Request.Params["group_id"]==null?"Add Group":"Edit Group" %>
                Add/Edit Group
            </td>
        </tr>
        <tr>
            <td class="SearchHeadPaneGradient">
            </td>
        </tr>
        <tr style="height: 20px">
            <td class="ValidationText" align="center">
                <asp:Literal ID="Literal_error" runat="server" EnableViewState="False">
                </asp:Literal>
            </td>
        </tr>
        <tr>
            <td style="padding-left: 10px">
                <table width="100%">
                    <tr>
                        <td valign="top" style="padding-left: 10px;">
                            Group Name
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_GroupName" runat="server" CssClass="textboxNormal"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator_TextBox_GroupName" ControlToValidate="TextBox_GroupName"
                                ErrorMessage="*" Enabled="false" runat="server"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td valign="top" style="padding-left: 10px">
                           <%-- Group Type--%>
                        </td>
                        <td>
                            <asp:DropDownList ID="DropDownList_Group_Type" visible =false runat="server" AutoPostBack="True"
                                OnSelectedIndexChanged="DropDownList_Group_Type_SelectedIndexChanged">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td valign="top" style="padding-left: 10px;">
                            Organization
                        </td>
                        <td>
                            <b>
                                <asp:Label ID="Label_SelectedOrganization" runat="server"></asp:Label></b>
                        </td>
                    </tr>
                    <tr>
                        <td style="padding-left: 10px; height: 61px;">
                            <asp:Label ID="Label_GroupDescription" Text="Description" runat="server"></asp:Label>
                        </td>
                        <td style="height: 61px">
                            <asp:TextBox ID="TextBox_GroupDescription" runat="server" TextMode="MultiLine" MaxLength="256"
                                Height="27px" Width="356px" CssClass="MultiLineTextBox"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td valign="top" style="padding-left: 10px;">
                            Group Email
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_GroupEmail" runat="server" MaxLength="256" Width="356px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td valign="top" style="padding-left: 10px;">
                            Send Individual Alert
                        </td>
                        <td>
                            <asp:CheckBox ID="CheckBox_IndividualAlert" runat="server" />
                        </td>
                    </tr>
                     <tr>
                        <td style="padding-left: 10px; vertical-align: top">
                            Group Rights
                        </td>
                       
                    </tr>
                    <tr>
                        <td colspan="2">
                            <ComponentArt:TabStrip ID="TabStrip_AddEdit" CssClass="TopGroup" DefaultItemLookId="DefaultTabLook"
                                DefaultSelectedItemLookId="SelectedTabLook" DefaultDisabledItemLookId="DisabledTabLook"
                                DefaultGroupTabSpacing="1px" ImagesBaseUrl="../images/Tabimages/" MultiPageId="MultiPage_isuite"
                                runat="server" ScrollDownLookId="DefaultTabLook" ScrollLeftLookId="DefaultTabLook"
                                ScrollRightLookId="DefaultTabLook" ScrollUpLookId="DefaultTabLook" TopGroupTabSpacing="1px">
                                <ItemLooks>
                                    <ComponentArt:ItemLook LookId="DefaultTabLook" CssClass="DefaultTab" HoverCssClass="DefaultTabHover"
                                        LabelPaddingLeft="10px" LabelPaddingRight="10px" LabelPaddingTop="5px" LabelPaddingBottom="4px"
                                        LeftIconUrl="tab_left_icon.gif" RightIconUrl="tab_right_icon.gif" HoverLeftIconUrl="hover_tab_left_icon.gif"
                                        HoverRightIconUrl="hover_tab_right_icon.gif" LeftIconWidth="3px" LeftIconHeight="21px"
                                        RightIconWidth="3px" RightIconHeight="21px" LeftIconVisibility="Always" RightIconVisibility="Always" />
                                    <ComponentArt:ItemLook LookId="SelectedTabLook" CssClass="SelectedReportsTab" LabelPaddingLeft="10px"
                                        LabelPaddingRight="10px" LabelPaddingTop="4px" LabelPaddingBottom="4px" LeftIconUrl="selected_tab_left_icon.gif"
                                        RightIconUrl="selected_tab_right_icon.gif" LeftIconWidth="3px" LeftIconHeight="21px"
                                        RightIconWidth="3px" RightIconHeight="21px" LeftIconVisibility="Always" RightIconVisibility="Always" />
                                </ItemLooks>
                                <Tabs>
                                    <ComponentArt:TabStripTab runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                        ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                        DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                        Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                        SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Administation">
                                    </ComponentArt:TabStripTab>
                                    <ComponentArt:TabStripTab ID="TabStripTab2" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                        ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                        DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                        Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                        SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Operations">
                                    </ComponentArt:TabStripTab>
                                    <ComponentArt:TabStripTab ID="TabStripTab3" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                        ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                        DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                        Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                        SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Reports">
                                    </ComponentArt:TabStripTab>
                                </Tabs>
                            </ComponentArt:TabStrip>
                            <ComponentArt:MultiPage ID="MultiPage_isuite" CssClass="MultiPage" runat="server">
                                <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView_iSuite"
                                    Width="100%">
                                    <div style="text-align: right">
                                        <input type="button" id="button_checkAll" value="Check All" class="button"
                                            onclick="SetCheckBoxStatus(true, 'CheckBoxList_GroupRights_Administration')" />
                                        <input type="button" id="button_uncheckAll" value="Uncheck All" class="button"
                                            onclick="SetCheckBoxStatus(false, 'CheckBoxList_GroupRights_Administration')" />
                                    </div>
                                    <div style="overflow: scroll; height: 150px; vertical-align: top; text-align: left;
                                        background-color: InfoBackground">
                                        <b>Administration</b>
                                        <asp:CheckBoxList ID="CheckBoxList_GroupRights_Administration" runat="Server" RepeatColumns="5"
                                            RepeatDirection="Horizontal" RepeatLayout="Table">
                                        </asp:CheckBoxList>
                                    </div>
                                </ComponentArt:PageView>
                                <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView1" Width="100%">
                                     <div style="text-align: right">
                                <input type="button" id="button1" value="Check All" class="button" onclick="SetCheckBoxStatus(true, 'CheckBoxList_GroupRights_Operations')" />
                                <input type="button" id="button2" value="Uncheck All" class="button" onclick="SetCheckBoxStatus(false, 'CheckBoxList_GroupRights_Operations')" />
                            </div>
                            <div style="overflow: scroll; height: 150px; vertical-align: top; text-align: left;
                                background-color: Menu">
                                <b>Operations</b>
                                <asp:CheckBoxList ID="CheckBoxList_GroupRights_Operations" runat="Server" RepeatColumns="4"
                                    RepeatDirection="Horizontal" RepeatLayout="Table">
                                </asp:CheckBoxList>
                            </div>
                                </ComponentArt:PageView>
                                <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView2" Width="100%">
                                
                            <div style="text-align: right">
                                <input type="button" id="button3" value="Check All" class="button" onclick="SetCheckBoxStatus(true, 'CheckBoxList_GroupRights_Reports')" />
                                <input type="button" id="button4" value="Uncheck All" class="button" onclick="SetCheckBoxStatus(false, 'CheckBoxList_GroupRights_Reports')" />
                            </div>
                            <div style="overflow: scroll; height: 150px; vertical-align: top; text-align: left;
                                background-color: InfoBackground">
                                <b>Reports</b>
                                <asp:CheckBoxList ID="CheckBoxList_GroupRights_Reports" runat="Server" RepeatColumns="5"
                                    RepeatDirection="Horizontal" RepeatLayout="Table">
                                </asp:CheckBoxList>
                            </div>
                                </ComponentArt:PageView>
                            </ComponentArt:MultiPage>
                        </td>
                    </tr>
                   
                    <tr>
                        <td colspan="2" style="padding-left: 100px">
                            <asp:Button ID="Button_SaveUpdateGroup" runat="server" Visible =true Text="Save" OnClick="Button_SaveUpdate_Click"
                                CssClass="button" Width="57px" />&nbsp;
                            <%--<input id="Button_Cancel" type="button" value="Cancel" Visible =true onclick="window.location ='GroupUser.aspx'"
                                class="button" />--%>
                                <asp:Button ID="Button_Cancel" runat="server" Visible =true Text="Cancel"
                                CssClass="button" Width="57px" onclick="Button_Cancel_Click" />
                                 <asp:Button ID="Button_ApproveGroup" runat="server" Visible =false Text="Approve"
                                CssClass="button" Width="57px" onclick="Button_ApproveGroup_Click" />&nbsp;
                            <asp:Button ID="Button_DeclineGroup" runat="server" Visible =false Text="Decline"
                                CssClass="button" Width="57px" onclick="Button_DeclineGroup_Click" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <input type="button" runat="server" id="refresh" style="display: none" />
    </form>
</body>
</html>
