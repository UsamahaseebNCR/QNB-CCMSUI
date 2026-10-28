<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VaultAdjustments.aspx.cs" Inherits="Avanza.iSuite.UI.VaultAdjustments" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="../content/ISUITE.css" rel="stylesheet" />
    <title>CCMS - Cheques</title>
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
  
    <script type="text/javascript">

        function confirmATMDeletion(title) {
            return confirm('Are you sure you want to delete ATM ' + title + '?', 'iJournal');
        }</script>

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
                    Vault Adjustments
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
                                            From ( Creation Time ) :
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="TextBox_FromDate" CssClass="datepicker" Font-Names="Verdana"
                                                Font-Size="10px" />
                                            <asp:RegularExpressionValidator ID="ValFromDate" CssClass="Validator" runat="server"
                                                ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                                ControlToValidate="TextBox_FromDate" ErrorMessage="From Date is invalid.">*</asp:RegularExpressionValidator>
                                        </td>
                                        <td>
                                            To ( Creation Time ) :
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
                                            <asp:DropDownList ID="DropDownList_CIT" runat="server" Width="151px" CssClass="dropDown">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <%--<tr>
                                        <td>
                                            Vault :
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="DropDownList_Vault" runat="server" Width="150px" CssClass="dropDown">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>--%>
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
                                                Width="71px" OnClick="Button_reset_Click" CausesValidation="false" />&nbsp;
                                            <asp:HyperLink ID="HyperLink_newVaultAdj" runat="server" CssClass="Link" NavigateUrl="../admin/AddEditVaultAdj.aspx">Post New Vault Adjustment ... </asp:HyperLink>
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
                    Selected Vault Adjustments -
                    <asp:Literal ID="Literal_count" runat="server" EnableViewState="False"></asp:Literal>
                </th>
            </tr>
            <tr>
                <td valign="top" colspan="2">
                    <asp:GridView ID="GridView_VaultAdj" runat="server" AutoGenerateColumns="False" AllowSorting="true"
                        OnSorting="GridView_VaultAdj_Sorting" Width="100%" OnRowCommand="GridView_VaultAdj_RowCommand"
                        AllowPaging="True" OnPageIndexChanging="GridView_VaultAdj_PageIndexChanging" OnRowDataBound="GridView_VaultAdj_RowDataBound"
                        PageSize="15">
                        <AlternatingRowStyle BackColor="#eff0ef" />
                        <RowStyle HorizontalAlign="Center" />
                        <Columns>
                            <asp:BoundField DataField="name" HeaderText="CIT" SortExpression="name" HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="title" HeaderText="Vault" SortExpression="title" HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="created_on" HeaderText="Creation Time" SortExpression="created_on"
                                DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="modified_on" HeaderText="Modification Time" SortExpression="modified_on"
                                DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="amount" HeaderText="Amount" SortExpression="cheque_amount"
                                HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            
                            <asp:TemplateField HeaderText="Edit">
                                <ItemTemplate>
                                    <a href='AddEditVaultAdj.aspx?id=<%# Eval("id") %>'>
                                        <img alt="Click to edit cheques" src="../images/Edit.gif" style="border: 0" class="ShowHand" />
                                    </a>
                                </ItemTemplate>
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Delete">
                                <ItemTemplate>
                                    <asp:ImageButton runat="server" CommandName="deleteVaultAdj" CommandArgument='<%# Eval("id") %>'
                                        ToolTip="Click to delete vault adjustment" ImageUrl="../images/Delete.gif" ID="Button_deleteVaultAdjustment"
                                        CssClass="ShowHand" />
                                </ItemTemplate>
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                            No Cheques found!</EmptyDataTemplate>
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
