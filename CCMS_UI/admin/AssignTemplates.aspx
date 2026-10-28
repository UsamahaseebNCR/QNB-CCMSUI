<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AssignTemplates.aspx.cs"
    ValidateRequest="false" Inherits="CCMSUI.admin.AssignTemplates" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Assign Templates</title>
    <link href="../content/ISUITE.css" rel="stylesheet" />
    <link href="../content/cal/popcalendar.css" type="text/css" rel="stylesheet" />
    <script src="../content/shortcut.js" type="text/javascript" language="javascript"></script>
    <link href="../content/jquery-ui-1.8.16.css" rel="stylesheet" type="text/css" />
    <link href="../content/ui-time-picker.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/jquery-1.7.1.min.js" type="text/javascript"></script>
    <script src="../Scripts/jquery-ui-1.8.16.min.js" type="text/javascript"></script>
    <script src="../Scripts/jquery-ui-timepicker-addon.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        window.parent.window.contentPage = '';

        function check(sender, eventArgs) {
            TreeView_atms.beginUpdate();
            setCheck1(eventArgs.get_node(), eventArgs.get_node().get_checked());
            TreeView_atms.endUpdate();
        }

        function setCheck1(node, selected) {
            var selectedChildren = 0;
            var childary = node.get_nodes();

            for (var i = 0; i < childary.get_length(); i++) {
                if (childary.getNode(i).get_id().substring(0, 1) == 'r') {
                    setCheck1(childary.getNode(i), selected)

                    if (selectedChildren == i)
                        childary.getNode(i).set_checked(selected);
                }
                else
                    childary.getNode(i).set_checked(selected);

                selectedChildren++;
            }

        }

    </script>
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)'
    onload="hideProgressBar()">
    <form id="form1" runat="server" autocomplete="off">
    <input type="hidden" runat="server" id="currentState" value="AssignTemplates.aspx" />
    <table style="width: 100%" cellspacing="0" cellpadding="0" border="0">
        <tr>
            <th class="PageHeading" align="left">
                Assign Device Templates
            </th>
        </tr>
        <tr>
            <td class="ValidationText">
                <asp:Literal ID="Literal_error" runat="server" EnableViewState="False"></asp:Literal>
                <asp:ValidationSummary ID="ValidationSummary_err" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="ConfirmationText">
                <asp:Literal ID="Literal_ok" runat="server" EnableViewState="False"></asp:Literal>
            </td>
        </tr>
        <tr style="width: 100%">
            <td>
                <table style="width: 100%; padding: 0px; margin: 0px;">
                    <tr>
                        <td>
                            <table style="width: 100%;">
                                <tr>
                                    <td style="width: 10%">
                                        <asp:Label ID="Label_template" runat="server" Text="Template Name:"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="DropDownList_template" runat="server" CssClass="dropDown" AutoPostBack="true"
                                            Width="160" OnSelectedIndexChanged="DropDownList_template_SelectedIndexChanged">
                                        </asp:DropDownList>
                                        <asp:RequiredFieldValidator ID="DDL_Template_Validator" runat="server" ControlToValidate="DropDownList_template"
                                            SetFocusOnError="true" ErrorMessage="Please enter Template Name" Enabled="false">*</asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr style="padding-left: 10px">
                                    <td style=" vertical-align: top">
                                        <asp:Label ID="Label_atms" runat="server" Text="ATMs"></asp:Label>
                                    </td>
                                    <td>
                                    <div style="overflow:auto;height:370px; background-color:InfoBackground">
                                        <ComponentArt:TreeView ID="TreeView_atms" runat="server" ContentLoadingImageUrl="spinner.gif"
                                            CssClass="TreeViewMargin" NodeCssClass="TreeNode" HoverNodeCssClass="HoverTreeNode"
                                            MultipleSelectedNodeCssClass="MultipleSelectedTreeNode" SelectedNodeCssClass="SelectedTreeNode"
                                            ImagesBaseUrl="../images/" LineImagesFolderUrl="../images/lines/" DefaultImageHeight="16"
                                            DefaultImageWidth="16" DefaultMarginImageHeight="20" DefaultMarginImageWidth="25"
                                            LineImageHeight="20" LineImageWidth="20" DisplayMargin="true" ShowLines="true"
                                            LoadingFeedbackText="Loading ATMs ..." MultipleSelectEnabled="true" NodeLabelPadding="3"
                                            CausesValidation="False" EnableViewState="true" Height="100%">
                                            <ClientEvents>
                                                <NodeCheckChange EventHandler="check" />
                                            </ClientEvents>
                                        </ComponentArt:TreeView>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                    </td>
                                    <td>
                                        <asp:Button ID="Button_Save" runat="server" CssClass="button" OnClick="Button_Save_Click"
                                            Text="Save" />&nbsp;
                                        <asp:Button ID="Button_back" runat="server" CssClass="button" Text="Back" OnClick="Button_back_Click" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <input type="button" runat="server" id="refresh" style="width: 0px; height: 0px;
        visibility: hidden" />
    <div>
    </div>
    </form>
</body>
</html>
