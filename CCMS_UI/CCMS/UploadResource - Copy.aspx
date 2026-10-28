<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UploadResource.aspx.cs"
    Inherits="CCMSUI.CCMS.UploadResource" Title="CCMS - Upload Resource" %>

<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="../content/ISUITE.css" rel="stylesheet" />
    <%--<link href="../content/styleSheets/iDistribute.css" rel="stylesheet" />--%>
    <link href="../content/styleSheets/treeStyle.css" rel="stylesheet" type="text/css" />
    <link href="../content/Scripts/ui-lightness/jquery.ui.all.css" rel="stylesheet" type="text/css" />
    <link href="../content/Scripts/ui-lightness/jquery-ui-1.8.17.custom.css" rel="stylesheet"
        type="text/css" />
    <link href="../content/Scripts/ui-lightness/ui-time-picker.css" rel="stylesheet"
        type="text/css" />
    <script type="text/javascript" src="../content/Scripts/jquery-1.7.1.js"></script>
    <script type="text/javascript" src="../content/Scripts/jquery.ui.core.js"></script>
    <script src="../content/Scripts/jquery-1.7.1.min.js" type="text/javascript"></script>
    <script src="../content/Scripts/jquery-ui-1.8.16.min.js" type="text/javascript"></script>
    <script type="text/javascript" src="../content/Scripts/jquery.ui.widget.js"></script>
    <script type="text/javascript" src="../content/Scripts/jquery.ui.datepicker.js"></script>
    <script type="text/javascript" src="../content/Scripts/jquery-ui-timepicker-addon.js"></script>
    <script type="text/javascript">
        $(function () {

            $('.datepicker').datetimepicker({ dateFormat: 'dd/mm/yy',

                dateFormat: 'dd/mm/yy',
                timeFormat: 'hh:mm:ss',
                defaultDate: -1,
                minDate: '-2m',
                maxDate: '+2d',
                hourGrid: 4,
                minuteGrid: 10,

                showOn: "button",
                buttonImage: "../images/calendar.gif",
                buttonImageOnly: true

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
                    Upload Resource
                </td>
            </tr>
            <tr>
                <td class="SearchHeadPaneGradient">
                </td>
            </tr>
            <tr>
                <td align="left">
                    <asp:validationsummary id="ValidationSummary1" runat="server" showmessagebox="False"
                        showsummary="True" width="100%" cssclass="ValidationText"></asp:validationsummary>
                </td>
            </tr>
            <tr>
                <td>
                    <table cellspacing="0" cellpadding="0" width="100%" border="0">
                        <tr>
                            <td align="center" style="color: red">
                                <asp:label id="Label_Error" runat="server" cssclass="ValidationText"></asp:label>
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
                <td colspan="3">
                    <asp:wizard id="Wizard" runat="server" width="100%" finishpreviousbuttontext="Back"
                        onfinishbuttonclick="Wizard_FinishButtonClick" onnextbuttonclick="Wizard_NextButtonClick"
                        displaysidebar="false" onpreviousbuttonclick="Wizard_PreviousButtonClick" onactivestepchanged="Wizard_ActiveStepChanged"
                        activestepindex="0" >
                            <WizardSteps>
                                <asp:WizardStep ID="WizardStep1" runat="server">
                                    <table cellpadding="0" cellspacing="0" width="100%">
                                        <tr>
                                            <td style="width: 20%">
                                                &nbsp;<b>Upload Title</b>:&nbsp;<span style="color: #ff0000">(*)</span>
                                            </td>
                                            <td style="width: 80%">
                                                <asp:TextBox ID="txtUploadtitle" runat="server" Width="200px" CssClass="textbox"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="requiredfieldvalidator_Title" runat="server" ControlToValidate="txtUploadtitle"
                                                    ErrorMessage="Please enter Upload Title." Text="*"></asp:RequiredFieldValidator>
                                                <asp:RegularExpressionValidator ID="regularexpressionvalidator_Title" runat="server"
                                                    ControlToValidate="txtUploadtitle" ErrorMessage="There are some invalid characters in the Title"
                                                    ValidationExpression="[0-9|A-Z|a-z|\-|_|\s|,|.|&amp;]*" Text="*"></asp:RegularExpressionValidator>
                                            </td>
                                        </tr>
<%--                                        <tr>
                                            <td style="height: 10px;" colspan="2">
                                            </td>
                                        </tr>
--%>                                        <tr>
                                            <td colspan="2">
                                                <table cellpadding="0" cellspacing="0" border="0" width="100%">
                                                    <tr>
                                                        <td valign="top">
                                                            <asp:RadioButton ID="RadioSingleFile" runat="server" Text="Upload Single File" GroupName="ResourcePath"
                                                                Checked="True" AutoPostBack="True" OnCheckedChanged="RadioSingleFile_CheckedChanged"
                                                                Font-Bold="True" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <table cellpadding="0" cellspacing="0" border="0" width="100%">
                                                                <tr>
                                                                    <td style="width: 6%">
                                                                    </td>
                                                                    <td>
                                                                        <asp:Panel ID="panelSingleFile" runat="server" Width="100%">
                                                                            <table cellpadding="0" cellspacing="0" border="0" width="100%">
                                                                                <tr>
                                                                                    <td style="width: 15%">
                                                                                        File To Upload:&nbsp;<span style="color: #ff0000">(*)&nbsp;&nbsp;</span>
                                                                                    </td>
                                                                                    <td>
                                                                                        <asp:FileUpload ID="FileUploadCtrl" runat="server" Width="400px" CssClass="textbox" />
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td colspan="2" style="width: 100%">
                                                                                        <asp:Label ID="lblPreviousUploadedFile" runat="server"></asp:Label>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                        </asp:Panel>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                   <%-- <tr>
                                                        <td valign="top" style="width: 100%">
                                                            <asp:RadioButton ID="RadioNetworkFile" runat="server" Text="Upload Network File"
                                                                GroupName="ResourcePath" AutoPostBack="True" OnCheckedChanged="RadioNetworkFile_CheckedChanged"
                                                                Font-Bold="True" />
                                                        </td>
                                                    </tr>--%>
                                                  <%--  <tr>
                                                        <td style="width: 100%">
                                                            <table cellpadding="0" cellspacing="0" width="100%">
                                                                <tr>
                                                                    <td style="width: 6%">
                                                                    </td>
                                                                    <td>
                                                                        <asp:Panel ID="PanelNetworkFile" runat="server" Width="100%">
                                                                            <table cellpadding="0" cellspacing="0" border="0" width="100%">
                                                                                <tr>
                                                                                    <td style="width: 15%">
                                                                                        Resource Path:&nbsp;<span style="color: #ff0000">(*)</span>
                                                                                    </td>
                                                                                    <td>
                                                                                        <asp:TextBox ID="txtResourcePath" runat="server" Width="313px" CssClass="inputform"></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                        </asp:Panel>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>--%>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="height: 10px;" colspan="2">
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width: 20%">
                                                &nbsp;<b>Target ATM Path</b>:&nbsp;<span style="color: #ff0000">(*)</span>
                                            </td>
                                            <td style="width: 80%">
                                                <asp:TextBox ID="txtATMPath" runat="server" Width="313px" CssClass="textbox"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="requiredfieldvalidator_ATMPath" runat="server" ControlToValidate="txtATMPath"
                                                    ErrorMessage="Please enter Target ATM Path." Text="*"></asp:RequiredFieldValidator>
                                                <asp:RegularExpressionValidator ID="regularexpressionvalidator_ATMPath" runat="server"
                                                    ControlToValidate="txtATMPath" ErrorMessage="Please Enter Correct ATM Path" ValidationExpression='\b[A-Za-z]:\\[^./:*?&quot;&lt;&gt;|\r\n]*'
                                                    Text="*"></asp:RegularExpressionValidator>
                                            </td>
                                        </tr>
                                        <tr style="visibility:hidden">
                                            <td colspan="2" style="width: 100%">
                                                <table cellspacing="0" cellpadding="0" border="0" width="100%">
                                                    <tr>
                                                        <td style="height: 10px;">
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="width: 20%" valign="top">
                                                            &nbsp;<b>ATM Restart Options</b>:&nbsp;</td>
                                                        <td>
                                                            <asp:RadioButtonList ID="RadRestartATM" runat="server">
                                                                <asp:ListItem Text="Never restart ATM" Value="never-restart" Selected="True"></asp:ListItem>
                                                                <asp:ListItem Text="Restart ATM if file is in use" Value="restart-if-file-in-use"></asp:ListItem>
                                                                <asp:ListItem Text="Restart ATM after upload is completed" Value="always-restart"></asp:ListItem>
                                                            </asp:RadioButtonList>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </asp:WizardStep>
                                <asp:WizardStep ID="WizardStep2" runat="server">
                                    <table width="100%" cellpadding="0" cellspacing="0" border ="0">
                                        <tr>
                                            <td width="48%">
                                                <table width="100%" cellpadding="0" cellspacing="0" border ="0">
                                                    <tr>
                                                        <td valign="top" style="width:500px">
                                                           Upload Resource
                                                        </td>
                                                   
                                                        <td >
                                                            <ComponentArt:TreeView ID="TreeView1" Height="130px" KeyboardEnabled="False" CssClass="TreeView"
                                                                NodeCssClass="TreeNode" SelectedNodeCssClass="SelectedTreeNode" HoverNodeCssClass="HoverTreeNode"
                                                                NodeEditCssClass="NodeEdit" LineImageWidth="19" LineImageHeight="20" DefaultImageWidth="16"
                                                                DefaultImageHeight="16" NodeLabelPadding="3" ParentNodeImageUrl="../images/Tree/files/folder.gif"
                                                                LeafNodeImageUrl="../images/Tree/files/file.gif" ShowLines="True" LineImagesFolderUrl="../images/Tree/lines/"
                                                                ExpandedParentNodeImageUrl="../images/Tree/files/folder_open.gif" runat="server"
                                                                Width="100%">
                                                                <%--<ClientEvents>
                                                                    <NodeSelect EventHandler="OnNodeSelect" />
                                                                </ClientEvents>--%>
                                                            </ComponentArt:TreeView>
                                                        
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                    <td>
                                                        Execution Time 
                                                    </td>
                                                    <td>
                                                     <asp:textbox runat="server" id="TextBox_FromDate" cssclass="datepicker" />
                                            <%--<img src="../images/IconCalendar.gif" alt="Click to open calender" onclick="popUpCalendar(this,document.getElementById('<%=TextBox_FromDate.ClientID %>'),'dd/mm/yyyy');"
                                                style="border: 0; cursor: hand" id="IMG1" />--%>
                                            <asp:regularexpressionvalidator id="ValFromDate" cssclass="Validator" runat="server"
                                                validationexpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d (0[0-9]|1[0-9]|2[0-4]):([0-5]\d):([0-5]\d)"
                                                controltovalidate="TextBox_FromDate" errormessage="Creation Time From Date is invalid.">*</asp:regularexpressionvalidator>
                                                    </td></tr>
                                                    <tr>
                        <td style="width: 88px;">
                            <asp:Label ID="Label_User_Type" Text="User Type" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="DropDownList_User_Type" runat="server" AutoPostBack="True"
                                CssClass="textbox" OnSelectedIndexChanged="DropDownList_User_Type_SelectedIndexChanged">
                            </asp:DropDownList>
                        </td>
                    </tr>
                                                     <tr id="Organization_Row" runat="server">
                        <td>
                            <asp:Label ID="Label3" Text="Organization" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:CheckBoxList ID="CheckBoxList_Organization" runat="Server" RepeatColumns="5"
                                RepeatDirection="Horizontal">
                            </asp:CheckBoxList>
                            <asp:Button ID="Button_Load" runat="server" Text="Load Tree" OnClick="Button_Load_Click"
                                CssClass="button" Width="80px" CausesValidation="false" />
                        </td>
                    </tr>
                                                     <tr>
                        <td style="width: 88px" valign="top">
                            <asp:Label ID="Label_UserATMs" Text="User ATM(s) " runat="server"></asp:Label>
                        </td>
                        <td style="width: 100%;">
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
                                ShowLines="true" Height="150px" AllowTextSelection="True" AutoScroll="true" CollapseNodeOnSelect="false"
                                ExpandNodeOnSelect="false">
                                <ClientEvents>
                                    <NodeCheckChange EventHandler="check" />
                                </ClientEvents>
                            </ComponentArt:TreeView>
                      
                        </td>
                    </tr>
                                                </table>
                                            </td>
                                           <%-- <td width="2%">
                                            </td>
                                            <td width="50%" valign="top">
                                                <table cellpadding="0" cellspacing="0" border="0" width="100%">
                                                    <tr>
                                                        <td id="TdFileProp" style="display: none">
                                                            <table cellpadding="0" cellspacing="0" border="0" width="100%">
                                                                <tr>
                                                                    <td colspan="2">
                                                                        <b>File Properties</b>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td style="height: 10px" colspan="2">
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td style="width: 38%">
                                                                        File:
                                                                    </td>
                                                                    <td>
                                                                        <div id="lblFileName">
                                                                        </div>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td style="width: 38%">
                                                                        File OverWrite Option:
                                                                    </td>
                                                                    <td>
                                                                        <select id="DDlOverWriteOptions" class="textbox" onchange="SetOverWriteOptions();">
                                                                            <option value="overwrite" selected="selected">Overwrite</option>
                                                                            <option value="overwrite-after-backup">Overwrite after Backup</option>
                                                                            <option value="no-replace-if-exists">Don't replace if exists</option>
                                                                        </select>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td style="width: 38%">
                                                                        File Execute Option:
                                                                    </td>
                                                                    <td>
                                                                        <input type="checkbox" class="textbox" id="chkExecuteFile" onchange="SetExecuteOptions();" />
                                                                    </td>
                                                                </tr>
<%--                                                                <tr>
                                                                    <td style="height: 20px" colspan="2">
                                                                   </td>
                                                                </tr>
--%>                                                            </table>
                                                        </td>
                                                    </tr>
                                                    <tr style="visibility:hidden">
                                                        <td>
                                                            <table cellpadding="0" cellspacing="0" border="0" width="100%">
                                                                <tr>
                                                                    <td colspan="2">
                                                                        <b>Instructions:</b>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                        <li>Select each File and set Properties.</li>
                                                                        <li>Default Value will be set if not set properties.</li>
                                                                        <li>Execute option is set for only one File.</li>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>--%>
                                        </tr>
                                    </table>                                
                                </asp:WizardStep>
                            </WizardSteps>
                            <StartNextButtonStyle CssClass="button"  />
                            <FinishCompleteButtonStyle CssClass="button" />
                            <FinishPreviousButtonStyle CssClass="button" />
                        </asp:wizard>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
