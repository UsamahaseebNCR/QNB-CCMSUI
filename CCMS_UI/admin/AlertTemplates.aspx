<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AlertTemplates.aspx.cs"
    Inherits="Avanza.iSuite.UI.AlertTemplates" %>

<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="../content/iSuite.css" rel="stylesheet" />
    <title>CCMS - Alert Templates</title>
    <script src="../content/shortcut.js" type="text/javascript" language="javascript"></script>
    <script language="javascript" type="text/javascript">
        function ShowErrorMessage(errorMessage) {
            document.getElementById('Label_Dialog_ErrorMessage').innerText = errorMessage;
        }

        function RefreshDialog() {
            document.getElementById('TextBox_Dialog_Name').value = '';
            document.getElementById('TextBox_Dialog_Description').value = '';

            SetTabIndex('TextBox_Dialog_Name', 1, true);
            SetTabIndex('TextBox_Dialog_Description', 2, false);
            SetTabIndex('Button_Dialog_Save', 3, false);
            SetTabIndex('Button_Dialog_Back', 4, false);
        }

        function ShowAddAlertTemplateDialog() {
            Dialog_AddAlertTemplate.Show();
            RefreshDialog();
            ShowErrorMessage('');
        }

        function Button_Dialog_Save_Click() {
            var name = document.getElementById('TextBox_Dialog_Name').value;
            var description = document.getElementById('TextBox_Dialog_Description').value;

            if (name == '')
                ShowErrorMessage('Please specify Name');
            else
                Callback_AddAlertTemplate.Callback(name, description);
        }
    </script>
</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)'
    onload="hideProgressBar();">
    <form id="form1" runat="server">
    <div>
        <table style="width: 100%" cellspacing="0" cellpadding="0" border="0" class="mainbox">
            <tr>
                <td class="PageHeading" align="left">
                    Device Alert Templates Management
                </td>
            </tr>
            <tr>
                <td class="ValidationText" align="center">
                    <asp:Literal ID="Literal_error" runat="server" EnableViewState="False"></asp:Literal>
                    <asp:ValidationSummary ID="ValidationSummary_ATM" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="ConfirmationText">
                    <%=Session[SessionVars.notice.ToString()]%>
                    <%Session[SessionVars.notice.ToString()] = "";%>
                    <asp:Literal ID="Literal_ok" runat="server" EnableViewState="False"></asp:Literal>
                </td>
            </tr>
            <tr align="left" style="background-color: WHITE">
                <td style="height: 20px; padding-left: 5px;" colspan="2">
                    <b>Search Filters</b>
                </td>
            </tr>
            <tr align="left">
                <td colspan="2">
                    <table width="100%" class="SEARCHBOX" border="0">
                        <tr>
                            <td class="SearchFiltersPaneClass">
                                <table id="search" width="90%" border="0">
                                    <tr>
                                        <td style="width: 15%;">
                                            Name
                                        </td>
                                        <td style="width: 85%;">
                                            <asp:TextBox ID="TextBox_name" runat="server" CssClass="textboxNormal"></asp:TextBox>&nbsp;
                                            <asp:RegularExpressionValidator ControlToValidate="TextBox_name" ID="RegularExpressionValidator_TextBox_name"
                                                runat="server" ValidationExpression="[^']+" ErrorMessage="Name Is Invalid">*</asp:RegularExpressionValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="4" align="right">
                                            <asp:Button ID="Button_search" runat="server" CssClass="button" Text="Search" OnClick="Button_search_Click" />&nbsp;&nbsp;
                                            <asp:Button ID="Button_reset" runat="server" CssClass="button" Text="Reset" OnClick="Button_reset_Click"
                                                CausesValidation="false" />&nbsp;&nbsp;
                                            <asp:HyperLink ID="HyperLink_AddAlertTemplate" runat="server" NavigateUrl="javascript:ShowAddAlertTemplateDialog()">Add Template </asp:HyperLink>
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
                    &nbsp
                </td>
            </tr>
            <tr align="left">
                <th colspan="2" align="left">
                    Selected Alert Templates -
                    <asp:Literal ID="Literal_count" runat="server" EnableViewState="False"></asp:Literal>
                </th>
            </tr>
            <tr style="width: 100%">
                <td valign="top" colspan="2">
                    <asp:GridView ID="GridView_AlertTemplates" runat="server" AutoGenerateColumns="False"
                        Width="100%" AllowSorting="true" OnSorting="GridView_Sorting" AllowPaging="True"
                        OnPageIndexChanging="GridView_PageIndexChanging" PageSize="15" OnRowCreated="GridView_RowCreated"
                        OnRowDataBound="GridView_RowDataBound" OnRowCommand="GridView_RowCommand">
                        <HeaderStyle CssClass="HeadingCell" />
                        <EmptyDataTemplate>
                            No data found!</EmptyDataTemplate>
                        <PagerStyle CssClass="GridFooterCellClass" />
                        <Columns>
                            <asp:BoundField DataField="alert_template_name" HeaderText="Name" SortExpression="alert_template_name"
                                ItemStyle-Width="45%" HeaderStyle-CssClass="HeadingCell"></asp:BoundField>
                            <asp:BoundField DataField="alert_template_desc" HeaderText="Description" SortExpression="alert_template_desc"
                                ItemStyle-Width="45%" HeaderStyle-CssClass="HeadingCell"></asp:BoundField>
                            <asp:TemplateField HeaderText="Edit" ItemStyle-Width="5%" HeaderStyle-CssClass="HeadingCell">
                                <ItemTemplate>
                                    <a href='AddEditAlertTemplate.aspx?alert_template_id=<%# Eval("alert_template_id") %>'>
                                        <img alt="Click to Edit" src="../images/Edit.gif" class="ImageButtonClass" style="border: 0" />
                                    </a>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Delete" ItemStyle-Width="5%" HeaderStyle-CssClass="HeadingCell">
                                <ItemTemplate>
                                    <asp:ImageButton ID="Button_Delete" class="ImageButtonClass" runat="server" CommandName="DeleteTemplate"
                                        CommandArgument='<%# Eval("alert_template_id") %>' ImageUrl="../images/deactivate.gif"
                                        ToolTip='Click to Delete' />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
        </table>
    </div>
    <div>
        <ComponentArt:Dialog ContentCssClass="contentCss" FooterCssClass="footerCss" HeaderCssClass="headerCss"
            CssClass="dialogCss" Icon="" Value="" Title="AJAX TabStrip Content" CloseTransition="Fade"
            ModalMaskCssClass="modalMaskCssClass" AllowDrag="false" Alignment="TopCentre"
            Modal="true" ID="Dialog_AddAlertTemplate" runat="server" Width="210" Height="500px"
            ContentClientTemplateId="ClientTemplate1" AnimationType="None" AnimationPath="Direct"
            AnimationSlide="Linear" ShowTransition="None" TransitionDuration="0" AnimationDuration="0">
            <ClientTemplates>
                <ComponentArt:ClientTemplate ID="ClientTemplate1">
                    <table cellpadding="0" style="border-color: #c9c9c9" cellspacing="0" border="0" width="100%"
                        onmousedown="Dialog2.StartDrag(event);">
                        <tr>
                            <td style="background-image: url(../images/DialogImages/top.gif); height: 45px;">
                                <table width="100%" cellpadding="0" cellspacing="0" border="0">
                                    <tr>
                                        <td valign="middle" style="padding-left: 4px; color: white; font-size: 15px; font-family: Verdana;
                                            font-weight: bold;">
                                            Add New Alert Template
                                        </td>
                                        <td align="right" style="padding-right: 10px;">
                                            <img src="../images/DialogImages/close.gif" onclick="Dialog_AddAlertTemplate.Close();" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td style="border-color: #c9c9c9; background-color: white; font-size: 12px; font-family: Arial;
                                padding: 10px;">
                                <div>
                                    <table>
                                        <tr class="ValidationTextClass">
                                            <td colspan="2">
                                                <label id="Label_Dialog_ErrorMessage">
                                                </label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="DialogContentHeadingCellClass">
                                                Name*
                                            </td>
                                            <td class="DialogContentRowCellClass">
                                                <input type="text" id="TextBox_Dialog_Name" class="TextBoxClass" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="DialogContentHeadingCellClass">
                                                Description
                                            </td>
                                            <td class="DialogContentRowCellClass">
                                                <input type="text" id="TextBox_Dialog_Description" class="TextBoxLargeClass" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2" align="right">
                                                <input id="Button_Dialog_Save" type="button" class="button" value="Save" onclick="javascript:Button_Dialog_Save_Click()" />&nbsp;&nbsp;
                                                <input id="Button_Dialog_Back" type="button" class="button" value="Cancel" onclick="Dialog_AddAlertTemplate.Close();document.body.scroll='no';" />
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </td>
                        </tr>
                    </table>
                </ComponentArt:ClientTemplate>
            </ClientTemplates>
            <%--  <Header>
            
                <table  style="background-image: url(../images/DialogImages/top.gif); height: 45px;" >
                    <tr>
                        <td style="width: 95%;">
                            Alert Template
                        </td>
                        <td style="width: 5%; text-align: right;">
                            <img class="ImageButtonClass" alt="Close" src="../images/dialogimages/close.png"
                                onmousedown="this.src='../images/dialogimages/close-down.png';" onmouseup="this.src='../images/dialogimages/close-hover.png';"
                                onmouseover="this.src='../images/dialogimages/close-hover.png';" onmouseout="this.src='../images/dialogimages/close.png';"
                                onclick="Dialog_AddAlertTemplate.Close();document.body.scroll='no';" />
                        </td>
                    </tr>
                </table>
            </Header>
            <Content>
                <table style="width: 100%;">
                    <tr class="ValidationTextClass">
                        <td colspan="2">
                            <label id="Label_Dialog_ErrorMessage">
                            </label>
                        </td>
                    </tr>
                    <tr>
                        <td class="DialogContentHeadingCellClass">
                            Name*
                        </td>
                        <td class="DialogContentRowCellClass">
                            <input type="text" id="TextBox_Dialog_Name" class="TextBoxClass" />
                        </td>
                    </tr>
                    <tr>
                        <td class="DialogContentHeadingCellClass">
                            Description
                        </td>
                        <td class="DialogContentRowCellClass">
                            <input type="text" id="TextBox_Dialog_Description" class="TextBoxLargeClass" />
                        </td>
                    </tr>
                    <tr class="SearchButtonsPaneClass">
                        <td colspan="2">
                            <input id="Button_Dialog_Save" runat="server" type="button" class="button" value="Save"
                                onclick="javascript:Button_Dialog_Save_Click()" />&nbsp;&nbsp;
                            <input id="Button_Dialog_Back" runat="server" type="button" class="button" value="Cancel"
                                onclick="Dialog_AddAlertTemplate.Close();document.body.scroll='no';" />
                        </td>
                    </tr>
                </table>
            </Content>--%>
        </ComponentArt:Dialog>
    </div>
    <ComponentArt:CallBack ID="Callback_AddAlertTemplate" runat="server" OnCallback="AddAlertTemplate"
        Height="0px" Width="0px">
    </ComponentArt:CallBack>
    </form>
</body>
</html>
