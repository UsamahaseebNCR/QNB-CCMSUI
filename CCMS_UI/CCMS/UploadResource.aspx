<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UploadResource.aspx.cs"
    Inherits="CCMSUI.CCMS.UploadResource" Title="CCMS - Upload Resource" %>

<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="../content/ISUITE.css" rel="stylesheet" />
    <%--<link href="../content/styleSheets/iDistribute.css" rel="stylesheet" />--%>
    <link href="../content/styleSheets/treeStyle.css" rel="stylesheet" type="text/css" />

     <script src="../content/DateTimePicker/jquery.min.js"></script>
 <link href="../content/DateTimePicker/jquery.datetimepicker.min.css" rel="stylesheet" />
 <script src="../content/DateTimePicker/jquery.datetimepicker.full.min.js"></script>

    <script type="text/javascript">
        $(function () {
            $(".datepicker").datetimepicker({
                format: 'd/m/Y H:i:s', // format for both date and time
                step: 15 // time step interval
            });
        });

        var SelectedNode;
        var chk;
        function OnNodeSelect(sender, params) {
            SelectedNode = params.get_node();
            var SelectedNodeValue = SelectedNode.get_value().split("|");

            document.getElementById("TdFileProp").style.display = '';
            document.getElementById("DDlOverWriteOptions").value = SelectedNodeValue[1];
            document.getElementById("lblFileName").innerHTML = "<span id='a'>" + SelectedNode.get_text() + "</span>";

            if (document.getElementById("CheckedNodeID").value == SelectedNode.get_id()) {
                document.getElementById("chkExecuteFile").checked = true;
                document.getElementById("chkExecuteFile").disabled = '';
            }
            else {
                document.getElementById("chkExecuteFile").checked = false;
                if (document.getElementById("CheckedNodeID").value != "")
                    document.getElementById("chkExecuteFile").disabled = 'disabled';
            }
        }
        function SetOverWriteOptions() {
            if (SelectedNode) {
                var SelectedNodeValue = SelectedNode.get_value().split("|");
                SelectedNode.set_value(SelectedNodeValue[0] + "|" + document.getElementById("DDlOverWriteOptions").value);

            }
        }
        function SetExecuteOptions() {
            if (SelectedNode) {
                if (document.getElementById("chkExecuteFile").checked) document.getElementById("CheckedNodeID").value = SelectedNode.get_id();
                else document.getElementById("CheckedNodeID").value = "";
            }
        }

        function setCheck(node, selected) {
            var childary = node.get_nodes();

            for (var i = 0; i < childary.get_length(); i++) {
                if (childary.getNode(i).get_id().substring(0, 1) == 'r') {
                    childary.getNode(i).set_checked(selected);
                    setCheck(childary.getNode(i), selected)
                }
                else
                    childary.getNode(i).set_checked(selected);
            }
        }

        window.parent.window.contentPage = '';

        function check(sender, eventArgs) {
            UserATMsTreeView.beginUpdate();
            setCheck(eventArgs.get_node(), eventArgs.get_node().get_checked());
            UserATMsTreeView.endUpdate();
        }

    </script>
</head>
<body>
    <form id="form1" runat="server">
    <input id="CheckedNodeID" type="hidden" value="" runat="server" />
    <div>
        <table cellspacing="0" cellpadding="0" border="0" class="mainbox" width="100%">
            <tr>
                <td class="PageHeading" align="left">
                    Package Scheduling
                </td>
            </tr>
            <tr>
                <td class="SearchHeadPaneGradient">
                </td>
            </tr>
            <tr>
                <td align="left">
                    <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="False"
                        ShowSummary="True" Width="100%" CssClass="ValidationText"></asp:ValidationSummary>
                </td>
            </tr>
            <tr>
                <td>
                    <table cellspacing="0" cellpadding="0" width="100%" border="0">
                        <tr>
                            <td align="center" style="color: red">
                                <asp:Label ID="Label_Error" runat="server" CssClass="ValidationText"></asp:Label>
                                <asp:Literal ID="Literal_error" runat="server" EnableViewState="False"></asp:Literal>
                            </td>
                        </tr>
                        <%--<tr>
                            <td class="ValidationText" align="right">
                                <span style="color: #ff0000">Fields marked as (*) are mandatory</span>
                            </td>
                        </tr>--%>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <table style="width: 100%" border="0">
                        <tr>
                            <td style="width:10%">
                                Package
                            </td>
                            <td >
                                <asp:DropDownList ID="DropDownList_Package" runat="server" CssClass="dropDown">
                                </asp:DropDownList>
                                <%--<asp:TextBox ID="txtUploadtitle" runat="server" Width="200px" CssClass="textbox"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="requiredfieldvalidator_Title" runat="server" ControlToValidate="txtUploadtitle"
                                                ErrorMessage="Please enter Upload Title." Text="*"></asp:RequiredFieldValidator>
                                            <asp:RegularExpressionValidator ID="regularexpressionvalidator_Title" runat="server"
                                                ControlToValidate="txtUploadtitle" ErrorMessage="There are some invalid characters in the Title"
                                                ValidationExpression="[0-9|A-Z|a-z|\-|_|\s|,|.|&amp;]*" Text="*"></asp:RegularExpressionValidator>--%>
                            </td>
                        </tr>
                        <%-- <tr>
                                        <td style="width: 40%">Target ATM Path<span style="color: #ff0000">(*)</span>
                                        </td>
                                        <td style="width: 60%">
                                            <asp:TextBox ID="txtATMPath" runat="server" Width="313px" CssClass="textbox" value="C:\Temp"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="requiredfieldvalidator_ATMPath" runat="server" ControlToValidate="txtATMPath"
                                                ErrorMessage="Please enter Target ATM Path." Text="*"></asp:RequiredFieldValidator>
                                            <asp:RegularExpressionValidator ID="regularexpressionvalidator_ATMPath" runat="server"
                                                ControlToValidate="txtATMPath" ErrorMessage="Please Enter Correct ATM Path" ValidationExpression='\b[A-Za-z]:\\[^./:*?&quot;&lt;&gt;|\r\n]*'
                                                Text="*"></asp:RegularExpressionValidator>
                                        </td>
                                    </tr>--%>
                        <tr>
                            <td>
                                Package Upload Time
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="TextBox_UploadTime" CssClass="datepicker" />
                                <%--<img src="../images/IconCalendar.gif" alt="Click to open calender" onclick="popUpCalendar(this,document.getElementById('<%=TextBox_FromDate.ClientID %>'),'dd/mm/yyyy');"
                                                style="border: 0; cursor: hand" id="IMG1" />--%>
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" CssClass="Validator"
                                    runat="server" ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d (0[0-9]|1[0-9]|2[0-4]):([0-5]\d):([0-5]\d)"
                                    ControlToValidate="TextBox_UploadTime" ErrorMessage="Upload Time is invalid.">*</asp:RegularExpressionValidator>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Package Execution Time
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="TextBox_ExecutionTime" CssClass="datepicker" />
                                <%--<img src="../images/IconCalendar.gif" alt="Click to open calender" onclick="popUpCalendar(this,document.getElementById('<%=TextBox_FromDate.ClientID %>'),'dd/mm/yyyy');"
                                                style="border: 0; cursor: hand" id="IMG1" />--%>
                                <asp:RegularExpressionValidator ID="ValFromDate" CssClass="Validator" runat="server"
                                    ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d (0[0-9]|1[0-9]|2[0-4]):([0-5]\d):([0-5]\d)"
                                    ControlToValidate="TextBox_ExecutionTime" ErrorMessage="Execution Time is invalid.">*</asp:RegularExpressionValidator>
                            </td>
                        </tr>
                        <tr>
                                <td>Package Execution Timeout (Seconds)
                                </td>
                                <td>
                                    <asp:TextBox runat="server" ID="TextBox_ExecutionTimeout" Text="10" />
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator2" CssClass="Validator" runat="server"
                                        ValidationExpression="^\d+$"
                                        ControlToValidate="TextBox_ExecutionTimeout" ErrorMessage="Execution Timeout is invalid.">*</asp:RegularExpressionValidator>
                                </td>
                            </tr>
                            
                        <tr>
                            <td style="width: 88px;" valign="top">
                                <asp:Label ID="Label_UserATMs" Text="ATM(s) " runat="server"></asp:Label>
                            </td>
                            <td style="width: 100%; height: 500px; vertical-align: top">
                                <ComponentArt:TreeView ID="UserATMsTreeView" EnableViewState="true" runat="server"
                                    CollapseTransition="None" ContentLoadingImageUrl="spinner.gif" CssClass="TreeViewMargin"
                                    CutNodeCssClass="CutTreeNode" DefaultImageHeight="16" DefaultImageWidth="16"
                                    DefaultMarginImageHeight="20" DefaultMarginImageWidth="24" DisplayMargin="true"
                                    DragAndDropEnabled="true" ExpandTransition="None" HoverNodeCssClass="HoverTreeNode"
                                    ImagesBaseUrl="../images/" KeyboardCutCopyPasteEnabled="true" KeyboardEnabled="true"
                                    LineImageHeight="20" LineImagesFolderUrl="../images/lines/" LineImageWidth="19"
                                    LoadingFeedbackText="Loading ATMs" MarginWidth="24" MultipleSelectedNodeCssClass="MultipleSelectedTreeNode"
                                    MultipleSelectEnabled="true" NodeCssClass="TreeNode" NodeEditCssClass="NodeEdit"
                                    NodeEditingEnabled="true" NodeLabelPadding="3" SelectedNodeCssClass="SelectedTreeNode"
                                    ShowLines="true" Height="500px" AllowTextSelection="True" AutoScroll="true" CollapseNodeOnSelect="false"
                                    ExpandNodeOnSelect="false">
                                    <ClientEvents>
                                        <NodeCheckChange EventHandler="check" />
                                    </ClientEvents>
                                </ComponentArt:TreeView>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Button ID="Button_Submit" runat="server" CssClass="button" OnClick="Button_Submit_Click"
                        Text="Submit" />
                    <input type="button" class="button" value="Cancel" onclick="javascript: history.go(-1);" />
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
