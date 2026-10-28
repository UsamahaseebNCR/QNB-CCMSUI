<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VaultLedger.aspx.cs" Inherits="Avanza.iSuite.UI.VaultLedger" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="../content/ISUITE.css" rel="stylesheet" />
    <title>CCMS - Vault Ledger</title>
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
    <link href="../content/Scripts/ui-lightness/jquery-ui-1.7.2.custom.css" rel="stylesheet"
        type="text/css" />
    <script type="text/javascript" src="../content/js/jquery.js"></script>
    <script type="text/javascript" src="../content/scripts/ui.datepicker.js"></script>
    <script type="text/javascript">
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
    <script type="text/javascript">

        //window.parent.window.SetContentPage(window.location);

        function confirmATMDeletion(title) {
            return confirm('Are you sure you want to delete ATM ' + title + '?', 'iJournal');
        }</script>
</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)'
    onload="hideProgressBar();">
    <form id="form1" runat="server">
    <div>
        <table style="width: 100%" cellspacing="0" cellpadding="0" border="0" class="mainbox">
            <tr>
                <td class="PageHeading" align="left">
                    Vault Ledger
                </td>
            </tr>
            <tr>
                <td class="ValidationText" align="center">
                    <asp:Literal ID="Literal_error" runat="server" EnableViewState="False"></asp:Literal>
                </td>
            </tr>
            <tr>
                <td class="ConfirmationText" align="center">
                    <%=Session[SessionVars.notice.ToString()]%>
                    <%Session[SessionVars.notice.ToString()] = "";%>
                    <asp:Literal ID="Literal_ok" runat="server" EnableViewState="False"></asp:Literal>
                </td>
            </tr>
            <tr>
                <td align="center">
                    <asp:ValidationSummary ID="ValidationSummary_Chq" runat="server" />
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
                                        <td>
                                            From :
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="TextBox_FromDate" CssClass="datepicker" Font-Names="Verdana"
                                                Font-Size="10px" />
                                            <asp:RegularExpressionValidator ID="ValFromDate" CssClass="Validator" runat="server"
                                                ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                                ControlToValidate="TextBox_FromDate" ErrorMessage="From Date is invalid.">*</asp:RegularExpressionValidator>
                                        </td>
                                        <td>
                                            To :
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="TextBox_ToDate" CssClass="datepicker" Font-Names="Verdana"
                                                Font-Size="10px" />
                                            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" CssClass="Validator"
                                                runat="server" ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                                ControlToValidate="TextBox_ToDate" ErrorMessage="To Date is invalid.">*</asp:RegularExpressionValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            CIT :
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="DropDownList_CIT" runat="server" CssClass="dropDown" Width="150px">
                                            </asp:DropDownList>
                                        </td>
                                        <td>
                                            Vault Transaction Type :
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="DropDownList_VaultTrxnType" runat="server" CssClass="dropDown"
                                                Width="150px" AppendDataBoundItems="true">
                                                <asp:ListItem>Select</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Transaction Type :
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="DropDownList_TransactionType" runat="server" CssClass="dropDown"
                                                Width="150px">
                                                <asp:ListItem>Select</asp:ListItem>
                                                <asp:ListItem>Cr</asp:ListItem>
                                                <asp:ListItem>Dr</asp:ListItem>
                                            </asp:DropDownList>
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
                                                Width="71px" OnClick="Button_reset_Click" CausesValidation="false" />
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
                    &nbsp; &nbsp; &nbsp;&nbsp;
                </td>
            </tr>
            <tr align="left">
                <th colspan="2" align="left">
                    Selected Ledgers -
                    <asp:Literal ID="Literal_count" runat="server" EnableViewState="False"></asp:Literal>
                </th>
            </tr>
            <tr>
                <td valign="top" colspan="2">
                    <asp:GridView ID="GridView_VaultLedger" runat="server" AutoGenerateColumns="False"
                        AllowSorting="true" OnSorting="GridView_VaultLedger_Sorting" Width="100%" AllowPaging="True"
                        OnPageIndexChanging="GridView_VaultLedger_PageIndexChanging" PageSize="15">
                        <AlternatingRowStyle BackColor="#eff0ef" />
                        <RowStyle HorizontalAlign="Center" />
                        <Columns>
                            <asp:BoundField DataField="name" HeaderText="CIT" SortExpression="name" HeaderStyle-ForeColor="black"
                                ItemStyle-VerticalAlign="Top">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="title" HeaderText="Vault" SortExpression="title" HeaderStyle-ForeColor="black"
                                ItemStyle-VerticalAlign="Top">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="transaction_date" HeaderText="Transaction Date" SortExpression="transaction_date"
                                ItemStyle-VerticalAlign="Top" HeaderStyle-ForeColor="black" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="description" HeaderText="Description" SortExpression="description"
                                ItemStyle-VerticalAlign="Top" HeaderStyle-ForeColor="black" ItemStyle-HorizontalAlign="Left">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:TemplateField ItemStyle-HorizontalAlign="Left" >
                                <HeaderTemplate>
                                    Details
                                </HeaderTemplate>
                                <HeaderStyle CssClass="HeadingCell" />
                                <ItemTemplate>
                                    <%# Eval("ledger_amount_detail").ToString().Replace(",", "<br/>")%>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Dr" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign="Right">
                                <HeaderStyle CssClass="HeadingCell" />
                                <ItemTemplate>
                                    <%#Eval("Debit").ToString().Length > 0 ? parseValueIntoCurrency(decimal.Parse(Eval("Debit").ToString())) : ""%>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Cr" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign="Right">
                                <HeaderStyle CssClass="HeadingCell" />
                                <ItemTemplate>
                                    <%#Eval("credit").ToString().Length > 0 ? parseValueIntoCurrency(decimal.Parse(Eval("credit").ToString())) : ""%>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Balance" ItemStyle-VerticalAlign="Top" SortExpression="Balance" HeaderStyle-ForeColor="Black" ItemStyle-HorizontalAlign="Right">
                                <HeaderStyle CssClass="HeadingCell" />
                                <ItemTemplate>
                                    <%#Eval("balance").ToString().Length > 0 ? parseValueIntoCurrency(decimal.Parse(Eval("balance").ToString())) : ""%>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:BoundField DataField="user_full_name" HeaderText="Posted By" SortExpression="user_full_name"
                                ItemStyle-VerticalAlign="Top" HeaderStyle-ForeColor="black" ItemStyle-HorizontalAlign="Left">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                           <%--<asp:BoundField DataField="credit" HeaderText="Cr" HeaderStyle-ForeColor="black" DataFormatString="{0:f2}" 
                                ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign="Right">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="balance" HeaderText="Balance" SortExpression="balance"
                                ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign="Right" HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>--%>
                        </Columns>
                        <EmptyDataTemplate>
                            No Records found!</EmptyDataTemplate>
                        <HeaderStyle BackColor="Black" ForeColor="White" CssClass="HeadingCell" />
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
