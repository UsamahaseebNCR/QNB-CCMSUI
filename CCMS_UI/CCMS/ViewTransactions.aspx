<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewTransactions.aspx.cs"
    Inherits="CCMSUI.CCMS.ViewTransactions" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Untitled Page</title>
    <script src="../content/boxover.js" type="text/javascript" language="javascript">
    </script>
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
    <link href="../content/iSuite.css" rel="stylesheet" />

    <style type="text/css">
        .style1
        {
            height: 17px;
            width: 130px;
        }
        .style3
        {
            width: 141px;
        }
        .style4
        {
            height: 17px;
            width: 141px;
        }
    </style>

        <link href="../content/DatePicker/jquery-ui.css" rel="stylesheet" />
    <script src="../content/DatePicker/jquery-3.7.1.js"></script>
    <script src="../content/DatePicker/jquery-ui.js"></script>

<script>
    $(function () {
        $(".datepicker").datepicker({
            dateFormat: "dd/mm/yy",
            showOn: "button",
            buttonImage: "../images/calendar.gif",
            buttonImageOnly: true,
            buttonText: "Select date",
            showButtonPanel: true
        });
    });
</script>
</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)'
    onload="hideProgressBar();">
    <form id="form1" runat="server">
    <div>
        <table style="width: 100%" cellspacing="0" cellpadding="0" border="0" class="mainbox">
            <tr>
                <td class="PageHeading" align="left">
                    View Transactions
                </td>
            </tr>
            <tr>
                <td class="ValidationText" align="center">
                    <asp:Literal ID="Literal_error" runat="server" EnableViewState="False"></asp:Literal>
                </td>
            </tr>
            <tr>
                <td class="ConfirmationText" align="center">
                    <asp:ValidationSummary ID="valSummary" runat="server" />
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
                            <td>
                                <table id="search" width="90%" border="0">
                                    <tr>
                                        <td colspan="6">
                                            <asp:RadioButtonList ID="RadioButtonList_AnaylsisType" runat="server" RepeatDirection="Horizontal"
                                                AutoPostBack="true" OnSelectedIndexChanged="RadioButtonList_AnaylsisType_SelectedIndexChanged">
                                                <asp:ListItem>All Transactions</asp:ListItem>
                                                <asp:ListItem>Last Replenishment Till Now</asp:ListItem>
                                                <asp:ListItem>Transactions Between Replenishments</asp:ListItem>
                                            </asp:RadioButtonList>
                                        </td>
                                    </tr>
                                    <tr id="trTransactionRow" runat="server">
                                        <td class="style4">
                                            From :
                                        </td>
                                        <td style="width: 190px; height: 17px">
                                            <asp:TextBox runat="server" ID="TextBox_FromDate" Width="119px"  CssClass="datepicker" Font-Names="Verdana"
                                                Font-Size="10px" />
                                            <asp:RegularExpressionValidator ID="ValFromDate" CssClass="Validator" runat="server"
                                                ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                                ControlToValidate="TextBox_FromDate" ErrorMessage="From Date is invalid.">*</asp:RegularExpressionValidator>
                                        </td>
                                        <td class="style1">
                                            To :
                                        </td>
                                        <td style="height: 17px; width: 326px;">
                                            <asp:TextBox runat="server" ID="TextBox_ToDate" Width="119px"  CssClass="datepicker" Font-Names="Verdana"
                                                Font-Size="10px" />
                                            <asp:RegularExpressionValidator ID="ValToDate" CssClass="Validator" runat="server"
                                                ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                                ControlToValidate="TextBox_ToDate" ErrorMessage="To Date is invalid.">*</asp:RegularExpressionValidator>
                                        </td>
                                    </tr>
                                    <tr id="trReplenishmentRow" runat="server">
                                        <td>
                                            Selected ATM :
                                        </td>
                                        <td>
                                            <b>
                                                <asp:Label ID="Label_SelectedATM" runat="server" Text="Please, select an ATM"></asp:Label></b>
                                        </td>
                                    </tr>
                                    <tr id="trCycles" runat="server">
                                        <td class="style4">
                                            Number Of Cycles:
                                        </td>
                                        <td style="width: 190px; height: 17px">
                                            <asp:TextBox runat="server" ID="TextBox_NoOfCycles" Width="119px" CssClass="textbox"
                                                Text="2" />
                                        </td>
                                        <td>
                                            Sort Order
                                        </td>
                                        <td>
                                            <asp:RadioButtonList ID="RadioButtonList_Order" runat="server" RepeatDirection="Horizontal">
                                                <asp:ListItem Selected="True">Oldest</asp:ListItem>
                                                <asp:ListItem>Latest</asp:ListItem>
                                            </asp:RadioButtonList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="style4">
                                            Purge Note From :
                                        </td>
                                        <td style="width: 190px; height: 17px">
                                            <asp:TextBox runat="server" ID="TextBox_purgeFrom" Width="119px" CssClass="textbox" />
                                            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" CssClass="Validator"
                                                runat="server" ValidationExpression="\d*" ControlToValidate="TextBox_purgeFrom"
                                                ErrorMessage="Purge From is invalid.">*</asp:RegularExpressionValidator>
                                        </td>
                                        <td class="style1">
                                            Purge Note To :
                                        </td>
                                        <td style="height: 17px; width: 326px;">
                                            <asp:TextBox runat="server" ID="TextBox_purge_to" Width="119px" CssClass="textbox" />
                                            <asp:RegularExpressionValidator ID="RegularExpressionValidator2" CssClass="Validator"
                                                runat="server" ValidationExpression="\d*" ControlToValidate="TextBox_purge_to"
                                                ErrorMessage="Purge To is invalid.">*</asp:RegularExpressionValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="style4">
                                            Amount From :
                                        </td>
                                        <td style="width: 190px; height: 17px">
                                            <asp:TextBox runat="server" ID="TextBox_FromAmount" Width="119px" CssClass="textbox" />
                                            <asp:RegularExpressionValidator ID="RegularExpressionValidator3" CssClass="Validator"
                                                runat="server" ValidationExpression="\d*" ControlToValidate="TextBox_FromAmount"
                                                ErrorMessage="Purge From is invalid.">*</asp:RegularExpressionValidator>
                                        </td>
                                        <td class="style1">
                                            Amount To :
                                        </td>
                                        <td style="height: 17px; width: 326px;">
                                            <asp:TextBox runat="server" ID="TextBox_ToAmount" Width="119px" CssClass="textbox" />
                                            <asp:RegularExpressionValidator ID="RegularExpressionValidator4" CssClass="Validator"
                                                runat="server" ValidationExpression="\d*" ControlToValidate="TextBox_ToAmount"
                                                ErrorMessage="Purge To is invalid.">*</asp:RegularExpressionValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="style4">
                                            Dispensed[Type 1]:
                                        </td>
                                        <td style="width: 190px; height: 17px">
                                            <asp:TextBox runat="server" ID="TextBox_Dispensed1" Width="119px" CssClass="textbox" />
                                            <asp:RegularExpressionValidator ID="RegularExpressionValidator5" CssClass="Validator"
                                                runat="server" ValidationExpression="\d*" ControlToValidate="TextBox_Dispensed1"
                                                ErrorMessage="Dispensed[Type 1] is invalid.">*</asp:RegularExpressionValidator>
                                        </td>
                                        <td class="style1">
                                            Dispensed[Type 2]:
                                        </td>
                                        <td style="height: 17px; width: 326px;">
                                            <asp:TextBox runat="server" ID="TextBox_Dispensed2" Width="119px" CssClass="textbox" />
                                            <asp:RegularExpressionValidator ID="RegularExpressionValidator6" CssClass="Validator"
                                                runat="server" ValidationExpression="\d*" ControlToValidate="TextBox_Dispensed2"
                                                ErrorMessage="Dispensed[Type 2] is invalid.">*</asp:RegularExpressionValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="style4">
                                            Dispensed[Type 3]:
                                        </td>
                                        <td style="width: 190px; height: 17px">
                                            <asp:TextBox runat="server" ID="TextBox_Dispensed3" Width="119px" CssClass="textbox" />
                                            <asp:RegularExpressionValidator ID="RegularExpressionValidator7" CssClass="Validator"
                                                runat="server" ValidationExpression="\d*" ControlToValidate="TextBox_Dispensed3"
                                                ErrorMessage="Dispensed[Type 3] is invalid.">*</asp:RegularExpressionValidator>
                                        </td>
                                        <td class="style1">
                                            Dispensed[Type 4]:
                                        </td>
                                        <td style="height: 17px; width: 326px;">
                                            <asp:TextBox runat="server" ID="TextBox_Dispensed4" Width="119px" CssClass="textbox" />
                                            <asp:RegularExpressionValidator ID="RegularExpressionValidator8" CssClass="Validator"
                                                runat="server" ValidationExpression="\d*" ControlToValidate="TextBox_Dispensed4"
                                                ErrorMessage="Dispensed[Type 4] is invalid.">*</asp:RegularExpressionValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="style4">
                                            Group:
                                        </td>
                                        <td style="width: 190px; height: 17px">
                                            <asp:DropDownList ID="DropDownList_NoteSetTypeID" runat="server" CssClass="textbox" AppendDataBoundItems="true" Width="125px">
                                                <asp:ListItem>Select</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="4" align="right" style="padding-right: 80px">
                                            <asp:Button CssClass="button" ID="button_search" runat="server" Text="Search" OnClick="button_search_Click" />
                                            &nbsp;<asp:Button ID="Button_Reset" runat="server" CssClass="button" OnClick="Button_Reset_Click"
                                                Text="Reset" />&nbsp; &nbsp;<asp:Button ID="Button_ExportToPDF" runat="server" CssClass="button"
                                                    OnClick="Button_ExportToPDF_Click" Text="Export To PDF" />
                                            &nbsp; &nbsp;<asp:Button ID="Button1" runat="server" CssClass="button" OnClick="Button_ExportToExcel_Click"
                                                Text="Export To Excel" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td valign="top">
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
                    &nbsp; &nbsp; &nbsp;&nbsp;
                </td>
            </tr>
            <tr align="left">
                <th colspan="2" align="left">
                    Selected Transactions -
                    <asp:Literal ID="Literal_count" runat="server" EnableViewState="False"></asp:Literal>
                </th>
            </tr>
            <tr>
                <td valign="top" colspan="2">
                    <asp:GridView runat="server" ID="GridView_Trxn" AutoGenerateColumns="False" BorderStyle="Solid"
                        BorderWidth="1px" CellPadding="3" PageSize="15" Width="100%" AllowPaging="true"
                        AllowSorting="true" OnPageIndexChanging="GridView_Trxn_PageIndexChanging" OnRowDataBound="GridView_Trxn_RowDataBound"
                        OnSorting="GridView_Trxn_Sorting">
                        <EmptyDataTemplate>
                            <center>
                                No Records Found</center>
                        </EmptyDataTemplate>
                        <RowStyle HorizontalAlign="Center" />
                        <Columns>
                            <asp:TemplateField HeaderText="History">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                <ItemTemplate>
                                    <asp:Image ID="image_history" runat="server" ImageUrl="~/images/icon_region.jpg"
                                        Width="10" Height="10" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="title" HeaderText="ATM" ReadOnly="True" SortExpression="title"
                                ItemStyle-HorizontalAlign="Center">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="location" HeaderText="Location" ReadOnly="True" SortExpression="location"
                                ItemStyle-HorizontalAlign="Center">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="note_set_type_name" HeaderText="Group"   SortExpression="note_set_type_name">
                            <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="trxn_datetime" HeaderText="DateTime" SortExpression="trxn_datetime"
                                DataFormatString="{0:dd/MM/yy HH:mm:ss}" HtmlEncode="false">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                             <asp:BoundField DataField="processing_datetime" HeaderText="Processing DateTime" SortExpression="processing_datetime" 
                                DataFormatString="{0:dd/MM/yy HH:mm:ss}" HtmlEncode="false">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="amount" HeaderText="Amount" SortExpression="amount">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="cash_dispensed1" HeaderText="Dispensed 1" SortExpression="cash_dispensed1">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="cash_dispensed2" HeaderText="Dispensed 2" SortExpression="cash_dispensed2">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="cash_dispensed3" HeaderText="Dispensed 3" SortExpression="cash_dispensed3">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="cash_dispensed4" HeaderText="Dispensed 4" SortExpression="cash_dispensed4">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField Visible="false" DataField="cash_dispensed5" HeaderText="Dispensed 5" SortExpression="cash_dispensed5">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField Visible="false" DataField="cash_dispensed6" HeaderText="Dispensed 6" SortExpression="cash_dispensed6">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField Visible="false" DataField="cash_dispensed7" HeaderText="Dispensed 7" SortExpression="cash_dispensed7">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="cash_purged1" HeaderText="Purged 1" SortExpression="cash_purged1"
                                Visible="false">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="cash_purged2" HeaderText="Purged 2" SortExpression="cash_purged2"
                                Visible="false">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="cash_purged3" HeaderText="Purged 3" SortExpression="cash_purged3"
                                Visible="false">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="cash_purged4" HeaderText="Purged 4" SortExpression="cash_purged4"
                                Visible="false">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField Visible="false" DataField="cash_purged5" HeaderText="Purged 5" SortExpression="cash_purged5">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField Visible="false" DataField="cash_purged6" HeaderText="Purged 6" SortExpression="cash_purged6">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField Visible="false" DataField="cash_purged7" HeaderText="Purged 7" SortExpression="cash_purged7">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                </asp:BoundField>
                            <asp:BoundField DataField="cash_purgedTotal" HeaderText="Purged Notes" SortExpression="cash_purgedTotal">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="cash_remaining1" HeaderText="Remaining 1" SortExpression="cash_remaining1">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="cash_remaining2" HeaderText="Remaining 2" SortExpression="cash_remaining2">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="cash_remaining3" HeaderText="Remaining 3" SortExpression="cash_remaining3">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="cash_remaining4" HeaderText="Remaining 4" SortExpression="cash_remaining4">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                                <asp:BoundField Visible="false" DataField="cash_remaining5" HeaderText="Remaining 5" SortExpression="cash_remaining5">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField Visible="false" DataField="cash_remaining6" HeaderText="Remaining 6" SortExpression="cash_remaining6">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField Visible="false" DataField="cash_remaining7" HeaderText="Remaining 7" SortExpression="cash_remaining7">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                </asp:BoundField>
                        </Columns>
                        <RowStyle HorizontalAlign="Center" />
                        <%--<FooterStyle BackColor="#CCCCCC" />--%>
                        <SelectedRowStyle BackColor="#000099" Font-Bold="True" ForeColor="White" />
                        <%--<PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Left" />--%>
                        <HeaderStyle Font-Bold="True" ForeColor="White" />
                        <AlternatingRowStyle BackColor="#eff0ef" />
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
