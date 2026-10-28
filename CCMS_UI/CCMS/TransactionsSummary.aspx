<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TransactionsSummary.aspx.cs" Inherits="CCMSUI.TransactionsSummary" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="../content/iSuite.css" rel="stylesheet" />
    <title>CCMS - View ATM Transaction Summary</title>
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
      
     <script src="../content/DateTimePicker/jquery.min.js"></script>
 <link href="../content/DateTimePicker/jquery.datetimepicker.min.css" rel="stylesheet" />
 <script src="../content/DateTimePicker/jquery.datetimepicker.full.min.js"></script>

<script>
    $(function () {
        $(".datepicker").datetimepicker({
            format: 'd/m/Y H:i:s', // format for both date and time
            step: 15 // time step interval
        });
    });
</script>
</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)' onload="hideProgressBar();">
    <form id="form2" runat="server">
    <div>
        <table style="width: 100%" cellspacing="0" cellpadding="0" border="0" class="mainbox">
            <tr>
                <td class="PageHeading" align="left">
                    Transactions Summary
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
                                              <asp:regularexpressionvalidator id="ValFromDate" cssclass="Validator" runat="server"
       validationexpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d (0[0-9]|1[0-9]|2[0-4]):([0-5]\d):([0-5]\d)"
       controltovalidate="TextBox_FromDate" errormessage="Creation Time From Date is invalid.">*</asp:regularexpressionvalidator>
                                        </td>
                                        <td style="width: 99px; height: 17px">
                                            To :
                                        </td>
                                        <td style="height: 17px; width: 326px;">
                                            <asp:TextBox runat="server" ID="TextBox_ToDate" Width="119px"  CssClass="datepicker" Font-Names="Verdana"
                                                Font-Size="10px" />
                         <asp:regularexpressionvalidator id="ValToDate" cssclass="Validator" runat="server"
     validationexpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d (0[0-9]|1[0-9]|2[0-4]):([0-5]\d):([0-5]\d)"
     controltovalidate="TextBox_ToDate" errormessage="Creation Time To Date is invalid.">*</asp:regularexpressionvalidator>
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
                     Dispensed Summary
                    <asp:Literal ID="literal_dispensedSummaryCount" runat="server" EnableViewState="False"></asp:Literal>
                </th>
            </tr>
            <tr>
                <td valign="top" colspan="2">
                    <asp:GridView ID="GridView_DispensedSummary" runat="server" AutoGenerateColumns="False"
                        AllowSorting="true" OnSorting="GridView_DispensedSummary_Sorting" Width="100%"
                        AllowPaging="True" OnPageIndexChanging="GridView_DispensedSummary_PageIndexChanging"
                        onrowdatabound="GridView_DispensedSummary_RowDataBound"
                        
                        HeaderStyle-ForeColor="#ffffff" PageSize="15">
                        <AlternatingRowStyle BackColor="#eff0ef" />
                        <RowStyle HorizontalAlign="Center" />
                        <Columns>
                            <asp:BoundField DataField="title" HeaderText="ATM" SortExpression="title" ItemStyle-HorizontalAlign="Left">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                           
                             <asp:TemplateField HeaderText="cash_dispensed1" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign="Right" SortExpression="cash_dispensed1" >
                                <HeaderStyle CssClass="HeadingCell" />
                                <ItemTemplate>
                                    <%#parseValueIntoCurrency(decimal.Parse(Eval("cash_dispensed1").ToString()))%>
                                </ItemTemplate>
                            </asp:TemplateField>

                          
                            <asp:TemplateField HeaderText="cash_dispensed2" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign="Right" SortExpression="cash_dispensed2">
                                <HeaderStyle CssClass="HeadingCell" />
                                <ItemTemplate>
                                    <%#parseValueIntoCurrency(decimal.Parse(Eval("cash_dispensed2").ToString()))%>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="cash_dispensed3" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign="Right" SortExpression="cash_dispensed3">
                                <HeaderStyle CssClass="HeadingCell" />
                                <ItemTemplate>
                                    <%#parseValueIntoCurrency(decimal.Parse(Eval("cash_dispensed3").ToString()))%>
                                </ItemTemplate>
                            </asp:TemplateField> 

                             <asp:TemplateField HeaderText="cash_dispensed4" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign="Right" SortExpression="cash_dispensed4">
                                <HeaderStyle CssClass="HeadingCell" />
                                <ItemTemplate>
                                    <%#parseValueIntoCurrency(decimal.Parse(Eval("cash_dispensed4").ToString()))%>
                                </ItemTemplate>
                            </asp:TemplateField> 
                        </Columns>
                        <EmptyDataTemplate>
                            No Records Found!</EmptyDataTemplate>
                        <HeaderStyle BackColor="Black" ForeColor="Black" CssClass="HeadingCell" />
                    </asp:GridView>
                    &nbsp;
                </td>
            </tr>

              <tr align="left">
      <th colspan="2" align="left">
          Desposit Summary 
          <asp:Literal ID="literal_depositSummaryCount" runat="server" EnableViewState="False"></asp:Literal>
      </th>
  </tr>
  <tr>
      <td valign="top" colspan="2">
          <asp:GridView ID="GridView_DepositSummary" runat="server" AutoGenerateColumns="False"
              AllowSorting="true" OnSorting="GridView_DepositSummary_Sorting" Width="100%"
              AllowPaging="True" OnPageIndexChanging="GridView_DepositSummary_PageIndexChanging"
              onrowdatabound="GridView_DepositSummary_RowDataBound"
              
              HeaderStyle-ForeColor="#ffffff" PageSize="15">
              <AlternatingRowStyle BackColor="#eff0ef" />
              <RowStyle HorizontalAlign="Center" />
              <Columns>
                  <asp:BoundField DataField="title" HeaderText="ATM" SortExpression="title" ItemStyle-HorizontalAlign="Left">
                      <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                  </asp:BoundField>

                 
                   <asp:TemplateField HeaderText="amount" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign="Right" SortExpression="amount" >
                      <HeaderStyle CssClass="HeadingCell" />
                      <ItemTemplate>
                          <%#parseValueIntoCurrency(decimal.Parse(Eval("amount").ToString()))%>
                      </ItemTemplate>
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

