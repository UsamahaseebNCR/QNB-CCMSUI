<%@ Page Language="C#" AutoEventWireup="true" ValidateRequest="true" CodeBehind="AddEditInvoice.aspx.cs"
    Inherits="Avanza.iSuite.UI.AddEditInvoice" Title="CCMS - Add Edit Invoice" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Add Edit Invoice</title>
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
    <script type="text/javascript">
        window.parent.window.contentPage = '';          
    </script>
    <link href="../content/Scripts/ui-lightness/jquery-ui-1.7.2.custom.css" rel="stylesheet"
        type="text/css" />
    <script type="text/javascript" src="../content/js/jquery.js"></script>
    <script type="text/javascript" src="../content/scripts/ui.datepicker.js"></script>
    <script type="text/javascript">
        function receiveCallBackResult(response) {
            document.getElementById('orderInfo').innerHTML = response;
        }

        function openDialog(orderID) {
            DialogCallBack.callback(orderID);
            Dialog_OrderDetails.Show();
        }
        $(function () {

            $('.datepicker').datepicker({ dateFormat: 'dd/mm/yy',
                changeMonth: true,
                changeYear: true,
                showOn: "button",
                buttonImage: "../images/calendar.gif",
                buttonImageOnly: true

            });
        });
    </script>
    <link href="../content/iSuite.css" rel="stylesheet" />
</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)'
    onload="hideProgressBar();">
    <form id="form1" runat="server">
    <table cellpadding="0" cellspacing="0" border="0" class="MainBox" width="100%" style="padding-left: 10px;">
        <tr>
            <td class="PageHeading" align="left" colspan="2" style="height: 25px">
                <%=Request.QueryString["invoice_id"] == null?"Add a New Invoice ":"Edit Invoice"%>
            </td>
        </tr>
        <tr>
            <td class="SearchHeadPaneGradient" colspan="2">
            </td>
        </tr>
        <tr>
            <td colspan="2" align="center">
                <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="SaveInvoice"></asp:ValidationSummary>
            </td>
        </tr>
        <tr>
            <td class="ConfirmationText" align="center">
                <%=Session[SessionVars.notice.ToString()]%>
                <%Session[SessionVars.notice.ToString()] = "";%>
                <asp:Literal ID="Literal_ok" runat="server" EnableViewState="False"></asp:Literal>
            </td>
        </tr>
        <tr style="height: 20px">
            <td colspan="2" style="color: red;" align="center" valign="top">
                <asp:Literal ID="Literal_error" runat="server" EnableViewState="False"></asp:Literal>
            </td>
        </tr>
        <tr>
            <td valign="top">
                <table>
                    <tr>
                        <td style="width: 200px">
                            Organization
                        </td>
                        <td>
                            <asp:Label ID="Label_Org" runat="server" Text="Please select Organization" Font-Bold="true"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            CIT
                        </td>
                        <td>
                            <asp:DropDownList ID="DropDownList_CIT" runat="server" Width="151px" CssClass="dropDown"
                                AutoPostBack="true" OnSelectedIndexChanged="DropDownList_CIT_Change">
                            </asp:DropDownList>
                            <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToValidate="DropDownList_CIT"
                                Type="String" ValueToCompare="Select" Operator="NotEqual" ErrorMessage="Please select CIT"
                                ValidationGroup="SaveInvoice">*</asp:CompareValidator>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 200px">
                            Invoice Number
                        </td>
                        <td>
                            <asp:Label ID="Label_InvoiceNo" runat="server" Font-Bold="true"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 200px">
                            Invoice Date :
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="TextBox_InvoiceDate" CssClass="datepicker" />
                            <asp:RegularExpressionValidator ID="ValToDate" CssClass="Validator" runat="server"
                                ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                ControlToValidate="TextBox_InvoiceDate" ErrorMessage="Invoice Date is invalid." ValidationGroup="SaveInvoice">*</asp:RegularExpressionValidator>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="TextBox_InvoiceDate"
                                    Text="*" ErrorMessage="Please enter Invoice Date" ValidationGroup="SaveInvoice"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Status
                        </td>
                        <td>
                            <asp:DropDownList ID="DropDownList_Status" runat="server" Width="151px" CssClass="dropDown">
                            </asp:DropDownList>
                            <asp:CompareValidator ID="CompareValidator_Status" runat="server" ControlToValidate="DropDownList_Status"
                                ValidationGroup="SaveInvoice" Type="String" ValueToCompare="Select" Operator="NotEqual"
                                ErrorMessage="Please select Invoice status">*</asp:CompareValidator>
                        </td>
                    </tr>
                    <tr>
                        <td style="vertical-align: top">
                            Remarks
                        </td>
                        <td align="left">
                            <asp:TextBox ID="TextBox_Desc" runat="server" TextMode="MultiLine" CssClass="textbox"
                                Width="300" Height="50"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </td>
            <td valign="top">
                <fieldset>
                    <legend><b>Payment Details</b> </legend>
                    <table>
                        <tr>
                            <td>
                                Cheque No.
                            </td>
                            <td>
                                <asp:TextBox ID="TextBox_ChequeNo" runat="server" CssClass="textbox"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator_ChequeNo" runat="server" ControlToValidate="TextBox_ChequeNo"
                                    Text="*" ErrorMessage="Please enter Cheque No." ValidationGroup="SaveInvoice"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Cheque Date
                            </td>
                            <td>
                                <asp:TextBox ID="TextBox_ChequeDate" runat="server" CssClass="datepicker"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator_Date" runat="server" ControlToValidate="TextBox_ChequeDate"
                                    Text="*" ErrorMessage="Please enter Cheque Date" ValidationGroup="SaveInvoice"></asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="ValFromDate" CssClass="Validator" runat="server"
                                    ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                    ControlToValidate="TextBox_ChequeDate" ErrorMessage="Invalid Cheque Date entered"
                                    ValidationGroup="SaveInvoice">*</asp:RegularExpressionValidator>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Amount
                            </td>
                            <td>
                                <asp:TextBox ID="TextBox_Amount" runat="server" CssClass="textbox"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator_Amount" runat="server" ControlToValidate="TextBox_Amount"
                                    Text="*" ErrorMessage="Please enter Invoice Amount" ValidationGroup="SaveInvoice"></asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator_Amount" runat="server"
                                    ValidationGroup="SaveInvoice" ControlToValidate="TextBox_Amount" ValidationExpression="\d+(\.\d{2})?"
                                    Text="*" ErrorMessage="Invalid amount entered"></asp:RegularExpressionValidator>
                            </td>
                        </tr>
                    </table>
                </fieldset>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <ComponentArt:TabStrip ID="TabStrip_AddEdit" CssClass="TopGroup" DefaultItemLookId="DefaultTabLook"
                    DefaultSelectedItemLookId="SelectedTabLook" DefaultDisabledItemLookId="DisabledTabLook"
                    DefaultGroupTabSpacing="1px" ImagesBaseUrl="../images/Tabimages/" MultiPageId="MultiPage_AddEdit"
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
                        <ComponentArt:TabStripTab ID="TabStripTab_general" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                            ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                            DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                            Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                            SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Order Details">
                        </ComponentArt:TabStripTab>
                        <ComponentArt:TabStripTab ID="TabStripTab_physicalConfig" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                            ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                            DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                            Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                            SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Other Item Details">
                        </ComponentArt:TabStripTab>
                    </Tabs>
                </ComponentArt:TabStrip>
                <ComponentArt:MultiPage ID="MultiPage_AddEdit" CssClass="MultiPage" runat="server"
                    Width="100%">
                    <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView_general"
                        Width="100%">
                        <table width="100%" border="0">
                            <tr>
                                <td>
                                    <table style="background-color: InfoBackground" width="100%">
                                        <tr>
                                            <td style="width: 53px">
                                                From
                                            </td>
                                            <td style="width: 220px">
                                                <asp:TextBox ID="TextBox_FromDate" runat="server" CssClass="datepicker"></asp:TextBox>
                                                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" CssClass="Validator"
                                                    runat="server" ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                                    ControlToValidate="TextBox_FromDate" ErrorMessage="Invalid From Date entered">*</asp:RegularExpressionValidator>
                                            </td>
                                            <td style="width: 53px">
                                                To
                                            </td>
                                            <td style="width: 220px">
                                                <asp:TextBox ID="TextBox_ToDate" runat="server" CssClass="datepicker"></asp:TextBox>
                                                <asp:RegularExpressionValidator ID="RegularExpressionValidator2" CssClass="Validator"
                                                    runat="server" ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                                    ControlToValidate="TextBox_ToDate" ErrorMessage="Invalid To Date entered">*</asp:RegularExpressionValidator>
                                            </td>
                                            <td>
                                                <asp:Button ID="Button_SearchOrders" runat="server" Text="Search" CssClass="button"
                                                    OnClick="Button_SearchOrders_Click" />
                                                <asp:Button ID="Button_Reset" runat="server" Text="Reset" CssClass="button" OnClick="Button_Reset_Click" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td style="vertical-align: top" colspan="5">
                                    <asp:GridView ID="GridView_CashOrders" runat="server" AutoGenerateColumns="False"
                                        Font-Size="Smaller" Width="100%" AllowSorting="true" OnSorting="GridView_CashOrders_Sorting"
                                        DataKeyNames="cash_order_id,invoice_id" OnRowDataBound="GridView_CashOrders_RowDataBound">
                                        <HeaderStyle ForeColor="white" />
                                        <AlternatingRowStyle BackColor="#eff0ef" />
                                        <EmptyDataTemplate>
                                            <center>
                                                No Rows Found</center>
                                        </EmptyDataTemplate>
                                        <Columns>
                                            <asp:TemplateField HeaderText="Select" HeaderStyle-CssClass="HeadingCell" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="chkbx" runat="server"></asp:CheckBox>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="cash_order_datetime" HeaderText="Order Date" ItemStyle-HorizontalAlign="Center"
                                                HeaderStyle-CssClass="HeadingCell" DataFormatString="{0:dd/MM/yyyy}" SortExpression="cash_order_datetime"
                                                HeaderStyle-ForeColor="Black" />
                                            <asp:TemplateField HeaderText="Order Number" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <img src="../images/alert.jpg" id="imgAlert" runat="server" />
                                                    <a onclick='openDialog(<%# Eval("cash_order_id") %>)' style="text-decoration: underline;
                                                        cursor: hand">
                                                        <%#Eval("order_number")%>
                                                    </a>
                                                </ItemTemplate>
                                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                            </asp:TemplateField>
                                            <%--<asp:BoundField DataField="order_number" HeaderText="Order Number" ItemStyle-HorizontalAlign="Center"
                                                SortExpression="order_number" HeaderStyle-CssClass="HeadingCell" HeaderStyle-ForeColor="Black" />--%>
                                            <asp:TemplateField HeaderText="Amount" HeaderStyle-CssClass="HeadingCell" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="TextBox_Amount" runat="server" CssClass="textbox"></asp:TextBox>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Description" HeaderStyle-CssClass="HeadingCell" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="TextBox_OrderDescription" runat="server" CssClass="textbox" TextMode="SingleLine"
                                                        Width="300"></asp:TextBox>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </td>
                            </tr>
                        </table>
                    </ComponentArt:PageView>
                    <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView_physicalConfig1">
                        <table width="100%" border="0">
                            <tr>
                                <td colspan="7" align="center">
                                    <asp:ValidationSummary ID="ValidationSummary_OtherItems" runat="server" ValidationGroup="OtherItems" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <table style="background-color: InfoBackground" width="100%">
                                        <tr>
                                            <td style="width: 50px">
                                                Item
                                            </td>
                                            <td style="width: 140px">
                                                <asp:TextBox ID="TextBox_Item" runat="server" CssClass="textbox"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator_Item" runat="server" ControlToValidate="TextBox_Item"
                                                    Text="*" ErrorMessage="Please enter Item" ValidationGroup="OtherItems"></asp:RequiredFieldValidator>
                                            </td>
                                            <td style="width: 70px">
                                                Description
                                            </td>
                                            <td style="width: 250px">
                                                <asp:TextBox ID="TextBox_Description" runat="server" CssClass="textbox" Width="220"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator_Desc" runat="server" ControlToValidate="TextBox_Description"
                                                    Text="*" ErrorMessage="Please enter Description" ValidationGroup="OtherItems"></asp:RequiredFieldValidator>
                                            </td>
                                            <td style="width: 50px">
                                                Cost
                                            </td>
                                            <td style="width: 200px">
                                                <asp:TextBox ID="TextBox_Cost" runat="server" CssClass="textbox"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="TextBox_Cost"
                                                    Text="*" ErrorMessage="Please enter Cost" ValidationGroup="OtherItems"></asp:RequiredFieldValidator>
                                                <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" ControlToValidate="TextBox_Cost"
                                                    ValidationExpression="\d+" Text="*" ErrorMessage="Invalid Cost entered" ValidationGroup="OtherItems"></asp:RegularExpressionValidator>
                                            </td>
                                            <td>
                                                <asp:Button ID="Button_AddOtherItem" runat="server" Text="Add" CssClass="button"
                                                    OnClick="Button_AddOtherItem_Click" ValidationGroup="OtherItems" />
                                                <asp:Button ID="Button_Clear" runat="server" Text="Clear" CssClass="button" OnClick="Button_Clear_Click" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td style="vertical-align: top" colspan="7">
                                    <asp:GridView ID="GridView_OtherItems" runat="server" AutoGenerateColumns="False"
                                        Font-Size="Smaller" Width="100%" AllowSorting="true" OnSorting="GridView_OtherItems_Sorting"
                                        OnRowDeleting="GridView_OtherItems_RowDeleting" HeaderStyle-CssClass="HeadingCell">
                                        <HeaderStyle ForeColor="white" />
                                        <AlternatingRowStyle BackColor="#eff0ef" />
                                        <EmptyDataTemplate>
                                            <center>
                                                No Rows Found</center>
                                        </EmptyDataTemplate>
                                        <Columns>
                                            <asp:BoundField DataField="item" HeaderText="Item" SortExpression="item" ItemStyle-HorizontalAlign="Center"
                                                HeaderStyle-CssClass="HeadingCell" HeaderStyle-ForeColor="Black" />
                                            <asp:BoundField DataField="description" HeaderText="Description" SortExpression="description"
                                                ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="HeadingCell" HeaderStyle-ForeColor="Black" />
                                            <asp:BoundField DataField="cost" HeaderText="Cost" SortExpression="cost" ItemStyle-HorizontalAlign="Center"
                                                HeaderStyle-CssClass="HeadingCell" HeaderStyle-ForeColor="Black" />
                                            <asp:CommandField ShowDeleteButton="True" HeaderStyle-CssClass="HeadingCell" />
                                            <%--<asp:TemplateField HeaderText="Delete">
                                                <ItemTemplate>
                                                    <asp:ImageButton runat="server" CommandName="deleteOtherItem" CommandArgument='<%# Eval("id") %>'
                                                        ToolTip="Click to delete other item" ImageUrl="../images/Delete.gif" ID="Button_deleteOtherItem"
                                                        CssClass="ShowHand" />
                                                </ItemTemplate>
                                                <HeaderStyle CssClass="HeadingCell" />
                                            </asp:TemplateField>--%>
                                        </Columns>
                                    </asp:GridView>
                                </td>
                            </tr>
                        </table>
                    </ComponentArt:PageView>
                </ComponentArt:MultiPage>
            </td>
        </tr>
        <tr>
            <td colspan="2" style="font-weight: bold; padding-top: 30px;">
                <asp:Button ID="Button_SaveUpdateInvoice" runat="server" Text="Save" OnClick="Button_SaveUpdateInvoice_Click"
                    CssClass="button" Width="60px" ValidationGroup="SaveInvoice" />&nbsp;
                <input id="Button_Cancel" type="button" value="Cancel" onclick="window.location ='Invoices.aspx'"
                    class="button" />
            </td>
        </tr>
    </table>
    <br />
    <br />
    <input type="button" runat="server" id="refresh" style="display: none" />
    <ComponentArt:Dialog ContentCssClass="contentCss" FooterCssClass="footerCss" HeaderCssClass="headerCss"
        CssClass="dialogCss" Icon="" Value="" Title="AJAX TabStrip Content" CloseTransition="Fade"
        ModalMaskCssClass="modalMaskCssClass" AllowDrag="false" Alignment="MiddleCentre"
        Modal="true" ID="Dialog_OrderDetails" runat="server" Width="430" ContentClientTemplateId="ClientTemplate1"
        AnimationType="None" AnimationPath="Direct" AnimationSlide="Linear" ShowTransition="None"
        TransitionDuration="0" AnimationDuration="0" Height="250">
        <ClientTemplates>
            <ComponentArt:ClientTemplate ID="ClientTemplate1">
                <table cellpadding="0" style="border-color: #c9c9c9" cellspacing="0" border="0" width="100%"
                    onmousedown="Dialog_OrderDetails.StartDrag(event);">
                    <tr>
                        <td style="background-image: url(../images/DialogImages/top.gif); height: 45px;">
                            <table width="100%" cellpadding="0" cellspacing="0" border="0">
                                <tr>
                                    <td valign="middle" style="padding-left: 4px; color: white; font-size: 15px; font-family: Verdana;
                                        font-weight: bold;">
                                        Order Details
                                    </td>
                                    <td align="right" style="padding-right: 10px;">
                                        <img src="../images/DialogImages/close.gif" onclick="Dialog_OrderDetails.Close();" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td style="border-color: #c9c9c9; background-color: white; font-size: 12px; font-family: Arial;
                            padding: 10px;">
                            <div id="orderInfo" style="overflow: auto; height: 400px">
                            </div>
                        </td>
                    </tr>
                </table>
            </ComponentArt:ClientTemplate>
        </ClientTemplates>
    </ComponentArt:Dialog>
    <ComponentArt:CallBack ID="DialogCallBack" runat="server" Width="0" Height="0" OnCallback="DialogCallBack_Callback">
    </ComponentArt:CallBack>
    </form>
</body>
</html>
