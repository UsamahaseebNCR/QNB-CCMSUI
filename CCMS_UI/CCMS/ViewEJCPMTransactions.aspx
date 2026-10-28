<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewEJCPMTransactions.aspx.cs"
    Inherits="CCMSUI.CCMS.ViewEJCPMTransactions" %>

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
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)' onload="hideProgressBar();">
    <form id="form1" runat="server">
    <div>
        <table style="width: 100%" cellspacing="0" cellpadding="0" border="0" class="mainbox">
            <tr>
                <td class="PageHeading" align="left">
                    View CPM Transactions
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
                                    <tr>
                                        <td class="style4">
                                            PAN :
                                        </td>
                                        <td style="width: 190px; height: 17px">
                                            <asp:TextBox runat="server" ID="TextBox_PAN" Width="119px" CssClass="textbox" />
                                        </td>
                                        <td class="style1">
                                            TSN :
                                        </td>
                                        <td style="height: 17px; width: 326px;">
                                            <asp:TextBox runat="server" ID="TextBox_TSN" Width="119px" CssClass="textbox" />
                                            <asp:RegularExpressionValidator ID="RegularExpressionValidator2" CssClass="Validator"
                                                runat="server" ValidationExpression="\d*" ControlToValidate="TextBox_TSN" ErrorMessage="TSN is invalid.">*</asp:RegularExpressionValidator>
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
                                            <asp:Label ID="Label_status" runat="server" Text="Status:"></asp:Label>
                                        </td>
                                        <td style="width: 190px; height: 17px">
                                            <asp:CheckBoxList ID="CheckBoxList_Status" runat="server" CssClass="dropDown"
                                                RepeatDirection="Horizontal">
                                                <asp:ListItem Value="0" Selected="True">Successful</asp:ListItem>                                              
                                                <asp:ListItem Value="1" Selected="True">Failed</asp:ListItem>
                                            </asp:CheckBoxList>
                                        </td>
                                    <td id="tdAnalysisOn" runat="server">
                                            Analysis On
                                        </td>
                                        <td id="tdAnalysisOnCheckBoxList" runat="server">
                                            <asp:CheckBoxList ID="CheckBoxList_DisputeStatus" runat="server" CssClass="dropDown" 
                                                RepeatDirection="Horizontal">
                                                <asp:ListItem Value="0" Selected="True">Business Rule</asp:ListItem>
                                                <asp:ListItem Value="1" Selected="True">Non Business Rule</asp:ListItem>
                                            </asp:CheckBoxList>
                                        </td>
                                    </tr>
                                   <%-- <tr>
                                        <td class="style4">
                                            Comment:
                                        </td>
                                        <td style="width: 190px; height: 17px" colspan="3">
                                            <asp:DropDownList ID="DropDownList_Comment" runat="server" AppendDataBoundItems="true"
                                                CssClass="dropDown">
                                                <asp:ListItem>Any</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>--%>
                                    
                                    <tr>
                                        <td>
                                            Display only disputed
                                        </td>
                                        <td>
                                            <asp:CheckBoxList ID="CheckBoxList_IsDisputed" runat="server" CssClass="dropDown"
                                                RepeatDirection="Horizontal">
                                                <asp:ListItem Value="0">Disputed</asp:ListItem>                                              
                                                
                                            </asp:CheckBoxList>
                                        </td>
                                        <td>
                                            Host TSN.:
                                        </td>
                                        <td>
                                        
                                            <asp:TextBox ID="TextBox_HostTSN" runat="server" CssClass="textbox"></asp:TextBox>
                                        </td>
                                    </tr>
                                        <tr>
                                            <td style="height: 20px">
                                                Reference No:</td>
                                            <td style
                                                ="height: 20px">
                                             <asp:TextBox ID="TextBox_referenceNo" runat="server" CssClass="textboxNormal"></asp:TextBox>
                                            </td>
                                            <td style="height: 20px">
                                                Customer ID :</td>
                                            <td style="height: 20px">
                                              <asp:TextBox ID="TextBox_customerID" runat="server" CssClass="textboxNormal"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <td>
                                                Customer Mobile:</td>
                                            <td>
                                                 <asp:TextBox ID="TextBox_customerMobile" runat="server" CssClass="textboxNormal"></asp:TextBox></td>
                                      </td>
                                       <td>
                                                Customer Telephone:</td>
                                            <td>
                                                 <asp:TextBox ID="TextBox_customerTelephone" runat="server" CssClass="textboxNormal"></asp:TextBox></td>
                                      </td>
                                        </tr>
                                 
                                    <tr>
                                    <tr id="bank_name" runat="server" visible="false">
                                        <td>
                                            Bank name:</td>
                                        <td>
                                            <asp:TextBox ID="TextBox_bankName" runat="server" CssClass="textboxNormal"></asp:TextBox></td>
                                    </tr>
                                    <td></td>
                                        <td align="left" style="padding-right: 80px">
                                            <asp:Button CssClass="button" ID="button_search" runat="server" Text="Search" OnClick="button_search_Click" />
                                            &nbsp;<asp:Button ID="Button_Reset" runat="server" CssClass="button" OnClick="Button_Reset_Click"
                                                Text="Reset" />
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
                        BorderWidth="1px" CellPadding="3" PageSize="25" Width="100%" AllowPaging="true"
                        AllowSorting="true" OnPageIndexChanging="GridView_Trxn_PageIndexChanging" OnSorting="GridView_Trxn_Sorting"
                         OnRowDataBound="GridView_Trxn_RowDataBound" OnRowCommand="GridView_Trxn_RowCommand">
                        <EmptyDataTemplate>
                            <center>
                                No Records Found</center>
                        </EmptyDataTemplate>
                        <RowStyle HorizontalAlign="Center" />
                        <Columns>
                            <asp:BoundField DataField="title" HeaderText="ATM" ReadOnly="True" SortExpression="title"
                                ItemStyle-HorizontalAlign="Center">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="bank_name" HeaderText="Bank Name" SortExpression="bank_name" Visible="false">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="trxn_datetime" HeaderText="DateTime" SortExpression="trxn_datetime"
                                DataFormatString="{0:dd/MM/yy HH:mm:ss}" HtmlEncode="false" ItemStyle-Width="140px">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="seq" HeaderText="TSN" SortExpression="seq">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="account_type" HeaderText="Account Type" SortExpression="account_type" ItemStyle-HorizontalAlign="Left">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            
                              <asp:BoundField DataField="account_no" HeaderText="Account No." SortExpression="account_no"
                                ItemStyle-HorizontalAlign="Left" ItemStyle-Width="140px">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                               <asp:BoundField DataField="host_TSN" HeaderText="HOST TSN" SortExpression="host_TSN"
                                ItemStyle-HorizontalAlign="Left" ItemStyle-Width="140px">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>

                            
                            <asp:BoundField DataField="pan" HeaderText="PAN" SortExpression="pan" ItemStyle-Width="140px">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="result" HeaderText="Result" SortExpression="result">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField ="detail" ItemStyle-HorizontalAlign="Left" HeaderText="Detail" HtmlEncode="false" ItemStyle-Width="140px">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black"/>
                           </asp:BoundField>
                            
                            <asp:BoundField DataField ="deposit_count" ItemStyle-HorizontalAlign="Left" HeaderText="Cheques <br/>Deposited" HtmlEncode="false" >
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black"/>
                           </asp:BoundField>
                            


                            <asp:BoundField DataField="deposit_amount" HeaderText="Amount" SortExpression="deposit_amount" ItemStyle-HorizontalAlign="Right">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="dispute_status" HeaderText="Dispute Status" SortExpression="dispute_status" ItemStyle-Width="50px">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="status" HeaderText="Status" SortExpression="status" ItemStyle-Width="80px">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="comment" HeaderText="Comment" SortExpression="comment" ItemStyle-HorizontalAlign="Left"
                                >
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="micr" HeaderText="MICR" SortExpression="micr" ItemStyle-HorizontalAlign="Left"   HtmlEncode="false" >
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>


                             <asp:BoundField DataField="trxn_status" HeaderText="Status" SortExpression="trxn_status"
                                ItemStyle-HorizontalAlign="Left">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>

                             <asp:BoundField DataField="customer_claim_amount" HeaderText="Customer Claim Amount" SortExpression="amount"
                                ItemStyle-HorizontalAlign="Left">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>

                             <asp:BoundField DataField="user_comments" HeaderText="User Comments" SortExpression="user_comments"
                                ItemStyle-HorizontalAlign="Left">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>



                             <asp:TemplateField HeaderText="Details">
                                    <ItemTemplate>
                                        <asp:ImageButton runat="server" CommandName="viewDetails" CommandArgument='<%# Eval("ej_parsed_cpm_transaction_id") %>'
                                            ToolTip="Click to view details" ImageUrl="../images/edit.gif" ID="Button_ViewDetails"
                                            CssClass="ShowHand" />
                                    </ItemTemplate>
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:TemplateField>


                            <%--<asp:TemplateField HeaderText="Details">
                                <ItemTemplate>
                                    <a onclick="javascript:window.open('TransactionComparison.aspx?ej=<%#Eval("ej_parsed_cpm_transaction_id")%>&d=2')">
                                        <img alt="Click to view details" src="../images/Edit.gif" style="border: 0" class="ShowHand" />
                                    </a>
                                </ItemTemplate>
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:TemplateField>--%>
                            <asp:TemplateField HeaderText="Customer Inquiry">
                                <ItemTemplate>
                                   <asp:ImageButton runat="server" ID="anchorInquiry"  ImageUrl ="../images/Edit.gif"
                                  />
                                    <%--<a onclick="javascript:window.open('CustomerDetail.aspx?id=<%#Eval("ej_parsed_cpm_transaction_id")%>&d=1')">
                                        <img alt="Click to view details" src="../images/Edit.gif" style="border: 0" class="ShowHand" />
                                    </a>--%>
                                </ItemTemplate>
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:TemplateField>

                             <asp:BoundField DataField="alert_Status" HeaderText="Alert Status"  ItemStyle-HorizontalAlign="Left">
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
