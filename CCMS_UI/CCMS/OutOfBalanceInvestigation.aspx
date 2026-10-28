<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OutOfBalanceInvestigation.aspx.cs"
    Inherits="CCMSUI.CCMS.OutOfBalanceInvestigation" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="../content/iSuite.css" rel="stylesheet" />
    <title>CCMS - Out Of Balance Investigation</title>
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
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
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)' onload="hideProgressBar();">
    <form id="form1" runat="server">
    <div>
        <table style="width: 100%" cellspacing="0" cellpadding="0" border="0" class="mainbox">
            <tr>
                <td class="PageHeading" align="left">
                    Balance Investigation
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
                                <table id="search" width="100%" border="0">
                                    <tr>
                                        <td style="width: 120px; height: 17px">
                                            From :
                                        </td>
                                        <td style="width: 190px; height: 17px">
                                            <asp:TextBox runat="server" ID="TextBox_FromDate" Width="119px"  CssClass="datepicker" Font-Names="Verdana"
                                                Font-Size="10px" />
                                            <asp:RegularExpressionValidator ID="ValFromDate" CssClass="Validator" runat="server"
                                                ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                                ControlToValidate="TextBox_FromDate" ErrorMessage="Creation Time From Date is invalid.">*</asp:RegularExpressionValidator>
                                        </td>
                                        <td style="width: 99px; height: 17px">
                                            To :
                                        </td>
                                        <td style="height: 17px; width: 326px;">
                                            <asp:TextBox runat="server" ID="TextBox_ToDate" Width="119px"  CssClass="datepicker" Font-Names="Verdana"
                                                Font-Size="10px" />
                                            <asp:RegularExpressionValidator ID="ValToDate" CssClass="Validator" runat="server"
                                                ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                                ControlToValidate="TextBox_ToDate" ErrorMessage="Creation Time To Date is invalid.">*</asp:RegularExpressionValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left">
                                            ATM IP :
                                        </td>
                                        <td>
                                            <asp:TextBox ID="TextBox_ip" runat="server" CssClass="textboxNormal"></asp:TextBox>
                                            <%--<asp:RequiredFieldValidator ID="requiredfieldvalidator1" runat="server" ControlToValidate="TextBox_ip"
                                    ErrorMessage="Please enter ATM IP." Text="*"></asp:RequiredFieldValidator>--%>
                                        </td>
                                        <td align="left">
                                            Group :
                                        </td>
                                        <td style="height: 20px">
                                            <asp:DropDownList ID="DropDownList_NoteSetType" runat="server" CssClass="dropDown">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    
                                    <tr>
                                        <td>
                                        </td>
                                        <td>
                                            <asp:Button ID="Button_search" runat="server" CssClass="button" OnClick="Button_search_Click"
                                                Text="Search" Width="71px" />
                                            &nbsp;<asp:Button ID="Button_reset" runat="server" CssClass="button" Text="Reset"
                                                Width="71px" OnClick="Button_reset_Click" />
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
                    Selected Summary -
                    <asp:Literal ID="Literal_summary" runat="server" EnableViewState="False"></asp:Literal>
                </th>
            </tr>
            <tr>
                <td valign="top" colspan="2">
                    <asp:GridView ID="GridView_OutOfBalanceInvestigation" runat="server" AutoGenerateColumns="False"
                        AllowSorting="true" OnSorting="GridView_OutOfBalanceInvestigation_Sorting" Width="100%"
                        AllowPaging="True" OnPageIndexChanging="GridView_OutOfBalanceInvestigation_PageIndexChanging"
                        onrowdatabound="GridView_OutOfBalanceInvestigation_RowDataBound"
                        
                        HeaderStyle-ForeColor="#ffffff" PageSize="15">
                        <AlternatingRowStyle BackColor="#eff0ef" />
                        <RowStyle HorizontalAlign="Center" />
                        <Columns>
                            <asp:BoundField DataField="title" HeaderText="ATM" SortExpression="title" ItemStyle-HorizontalAlign="Left">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="trxn_datetime" HeaderText="Date" SortExpression="trxn_datetime" DataFormatString="{0:dd/MM/yyyy}">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                           
                             <asp:TemplateField HeaderText="Opening Balance" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign="Right" SortExpression="opening_balance" >
                                <HeaderStyle CssClass="HeadingCell" />
                                <ItemTemplate>
                                    <%#parseValueIntoCurrency(decimal.Parse(Eval("opening_balance").ToString()))%>
                                </ItemTemplate>
                            </asp:TemplateField>

                          
                            <asp:TemplateField HeaderText="Replenishment" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign="Right" SortExpression="replenishment_amount">
                                <HeaderStyle CssClass="HeadingCell" />
                                <ItemTemplate>
                                    <%#parseValueIntoCurrency(decimal.Parse(Eval("replenishment_amount").ToString()))%>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Pre Withdrawals" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign="Right" SortExpression="pre_withdrawals">
                                <HeaderStyle CssClass="HeadingCell" />
                                <ItemTemplate>
                                    <%#parseValueIntoCurrency(decimal.Parse(Eval("pre_withdrawals").ToString()))%>
                                </ItemTemplate>
                            </asp:TemplateField> 

                             <asp:TemplateField HeaderText="Returns" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign="Right" SortExpression="return_amount">
                                <HeaderStyle CssClass="HeadingCell" />
                                <ItemTemplate>
                                    <%#parseValueIntoCurrency(decimal.Parse(Eval("return_amount").ToString()))%>
                                </ItemTemplate>
                            </asp:TemplateField> 
                           

                             <asp:TemplateField HeaderText="Withdrawals" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign="Right" SortExpression="withdrawals">
                                <HeaderStyle CssClass="HeadingCell" />
                                <ItemTemplate>
                                    <%#parseValueIntoCurrency(decimal.Parse(Eval("withdrawals").ToString()))%>
                                </ItemTemplate>
                            </asp:TemplateField> 


                             <asp:TemplateField HeaderText="Closing Balance" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign="Right" SortExpression="closing_balance">
                                <HeaderStyle CssClass="HeadingCell" />
                                <ItemTemplate>
                                    <%#parseValueIntoCurrency(decimal.Parse(Eval("closing_balance").ToString()))%>
                                </ItemTemplate>
                            </asp:TemplateField> 
                              <asp:TemplateField HeaderText="Cash Position Balance" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign="Right" SortExpression="cash_pos_balance">
                                <HeaderStyle CssClass="HeadingCell" />
                                <ItemTemplate>
                                    <%#parseValueIntoCurrency(decimal.Parse(Eval("cash_pos_balance").ToString()))%>
                                </ItemTemplate>
                            </asp:TemplateField> 

                            <asp:TemplateField HeaderText="Post Replenishment" >
                            <ItemTemplate>
                                <a style="border: 0" href="PostReplenishment.aspx?summary_id=<%#Eval("summary_id") %>">
                                    <img alt="Post Replenishment" src="../images/description.jpg" style="border: 0; width: 18px;
                                        height: 14px" /></a>
                            </ItemTemplate>
                            <HeaderStyle CssClass="HeadingCell" />
                        </asp:TemplateField>
                            


                           
                        </Columns>
                        <EmptyDataTemplate>
                            No Records Found!</EmptyDataTemplate>
                        <HeaderStyle BackColor="Black" ForeColor="Black" CssClass="HeadingCell" />
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
