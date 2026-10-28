<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Reconciliation.aspx.cs"
    Inherits="CCMSUI.CCMS.Reconciliation" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Untitled Page</title>
    <script src="../content/boxover.js" type="text/javascript" language="javascript">
    </script>
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
    <link href="../content/iSuite.css" rel="stylesheet" />
    <script type="text/javascript">
        //window.parent.window.SetContentPage( window.location);
    </script>
    <script language="javascript" src="../content/cal/popcalendar.js" type="text/javascript" />
    <script type="text/javascript">      
        	
		
    </script>
    <link href="../content/cal/popcalendar.css" type="text/css" rel="stylesheet" />
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
</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)'
    onload="hideProgressBar();">
    <form id="form1" runat="server">
    <div>
        <table style="width: 100%" cellspacing="0" cellpadding="0" border="0" class="mainbox">
            <tr>
                <td class="PageHeading" align="left" colspan="4">
                    Reconciliation Batch (History)
                </td>
            </tr>
            <tr>
                <td class="ValidationText" align="center" colspan="4">
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
                <td style="height: 20px; padding-left: 5px;" colspan="4">
                    <b>Search Filters</b>
                </td>
            </tr>
            <tr align="left">
                <td colspan="4">
                    <table width="100%" class="SEARCHBOX" border="0">
                        <tr>
                            <td>
                                <table id="search" width="90%" border="0">
                                    <tr id="trTransactionRow" runat="server">
                                        <td class="style4">
                                            From(Creation Time) :
                                        </td>
                                        <td style="height: 17px; width: 170px">
                                            <asp:TextBox runat="server" ID="TextBox_FromDate" Width="119px" CssClass="textbox" />
                                            <img src="../images/IconCalendar.gif" alt="Click to open calender" onclick="popUpCalendar(this,document.getElementById('<%=TextBox_FromDate.ClientID %>'),'dd/mm/yyyy');"
                                                style="border: 0; cursor:pointer" id="IMG1" />
                                            <asp:RegularExpressionValidator ID="ValFromDate" CssClass="Validator" runat="server"
                                                ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                                ControlToValidate="TextBox_FromDate" ErrorMessage="From Date is invalid.">*</asp:RegularExpressionValidator>
                                        </td>
                                        <td class="style1">
                                            To(Creation Time) :
                                        </td>
                                        <td style="height: 17px; width: 170px">
                                            <asp:TextBox runat="server" ID="TextBox_ToDate" Width="119px" CssClass="textbox" />
                                            <img src="../images/IconCalendar.gif" alt="Click to open calender" onclick="popUpCalendar(this,document.getElementById('<%=TextBox_ToDate.ClientID %>'),'dd/mm/yyyy');"
                                                style="border: 0; cursor: pointer" id="IMG2" />
                                            <asp:RegularExpressionValidator ID="ValToDate" CssClass="Validator" runat="server"
                                                ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                                ControlToValidate="TextBox_ToDate" ErrorMessage="To Date is invalid.">*</asp:RegularExpressionValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 120px">
                                            Batch Status :
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="DropDownList_status" runat="server" EnableViewState="true"
                                                CssClass="textbox" Width="144px">
                                                <asp:ListItem Text="Any"></asp:ListItem>
                                                <asp:ListItem Text="Failed"></asp:ListItem>
                                                <asp:ListItem Text="Scheduled"></asp:ListItem>
                                                <asp:ListItem Text="Dispense Completed"></asp:ListItem>
                                                <asp:ListItem Text="Deposit Completed"></asp:ListItem>
                                                <asp:ListItem Text="Completed"></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <%--<td style="width: 99px">
                                            User :
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="Dropdownlist_user" runat="server" Width="144px" CssClass="textbox">
                                            </asp:DropDownList>
                                        </td>
                                        <td>
                                            <asp:LinkButton ID="LinkButton_ScheduleBatch" runat="server" Text="Schedule Batch ..."
                                                PostBackUrl="~/CCMS/ScheduleReconBatch.aspx" CssClass="Link"></asp:LinkButton>
                                        </td>--%>
                                    </tr>
                                    <tr>
                                        <td colspan="5" align="right" style="padding-right: 80px">
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
            <tr align="left">
                <th colspan="2" align="left">
                    Selected Batches -
                    <asp:Literal ID="Literal_count" runat="server" EnableViewState="False"></asp:Literal>
                </th>
            </tr>
            <tr>
                <td valign="top" colspan="2">
                    <asp:GridView runat="server" ID="GridView_Batch" AutoGenerateColumns="False" BackColor="White"
                        BorderStyle="Solid" BorderWidth="1px" CellPadding="3" ForeColor="Black" GridLines="Vertical"
                        PageSize="25" Width="100%" AllowPaging="true" AllowSorting="true" OnPageIndexChanging="GridView_Batch_PageIndexChanging"
                        OnSorting="GridView_Batch_Sorting" OnRowCommand="GridView_Batch_RowCommand">
                        <AlternatingRowStyle BackColor="#eff0ef" />
                        <EmptyDataTemplate>
                            <center>
                                No Records Found</center>
                        </EmptyDataTemplate>
                        <RowStyle HorizontalAlign="Center" />
                        <Columns>
                            <asp:BoundField DataField="reconciliation_batch_id" HeaderText="Batch ID"
                                SortExpression="reconciliation_batch_id">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="title" HeaderText="ATM"
                                SortExpression="title">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="creation_time" HeaderText="Batch Creation Time" SortExpression="creation_time"
                                DataFormatString="{0:dd/MM/yy  HH:mm:ss}" HtmlEncode="false">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="transaction_start_date" HeaderText="Transaction Start Date"
                                SortExpression="transaction_start_date" DataFormatString="{0:dd/MM/yy}" HtmlEncode="false">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="transaction_end_date" HeaderText="Transaction End Date"
                                SortExpression="transaction_end_date" DataFormatString="{0:dd/MM/yy}" HtmlEncode="false">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                             <asp:BoundField DataField="Status" SortExpression="Status" HeaderText="Status">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="no_of_records_processed" SortExpression="no_of_records_processed"
                                HeaderText="Processed" >
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="no_of_records_reconciled" SortExpression="no_of_records_reconciled"
                                HeaderText="Reconciled">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <%--<asp:HyperLinkField DataTextField="no_of_records_failed_to_reconciled" DataNavigateUrlFields="reconciliation_batch_id"
                                DataNavigateUrlFormatString="ViewReconTransactions.aspx?i={0}&r=1" SortExpression="no_of_records_failed_to_reconciled"
                                HeaderStyle-CssClass="HeadingCell" HeaderText="Disputed" HeaderStyle-ForeColor="Black"
                                ItemStyle-ForeColor="Black" />--%>
                            <asp:BoundField DataField="no_of_records_failed_to_reconciled" SortExpression="no_of_records_failed_to_reconciled"
                                HeaderText="Disputed">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="Black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="retry_count" SortExpression="retry_count"
                                HeaderText="Retry Remaining">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:TemplateField HeaderText="Failure Reason">
                                <HeaderStyle CssClass="HeadingCell" />
                                <ItemTemplate>
                                    <a href="Reconciliation_details.aspx?id=<%#Eval("reconciliation_batch_id") %>">
                                        <%#Eval("failure_reason")%>
                                    </a>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <%--<asp:TemplateField HeaderText="View">
                                <ItemTemplate>
                                    <a style="border: 0" href="ViewUploadedData.aspx?id=<%#Eval("reconciliation_batch_id") %>">
                                        <img alt="View" src="../images/viewfile.jpg" style="border: 0; width: 18px; height: 14px" /></a>
                                </ItemTemplate>
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:TemplateField>--%>
                            <asp:TemplateField HeaderText="View">
                                <ItemTemplate>
                                    <a style="border: 0" href="ViewReconTransactions.aspx?i=<%#Eval("reconciliation_batch_id") %>&r=2">
                                        <img alt="View" src="../images/viewfile.jpg" style="border: 0; width: 18px; height: 14px" /></a>
                                </ItemTemplate>
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:TemplateField>
                           <%-- <asp:TemplateField HeaderText="View GL">
                                <ItemTemplate>
                                    <a style="border: 0" href="ViewGLSummary.aspx?r=<%#Eval("reconciliation_batch_id") %>">
                                        <img alt="View" src="../images/viewfile.jpg" style="border: 0; width: 18px; height: 14px" /></a>
                                </ItemTemplate>
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:TemplateField>--%>
                            <asp:TemplateField HeaderText="Details">
                                <ItemTemplate>
                                    <a style="border: 0" href="Reconciliation_details.aspx?id=<%#Eval("reconciliation_batch_id") %>">
                                        <img alt="View Details" src="../images/description.jpg" style="border: 0; width: 18px;
                                            height: 14px" /></a>
                                </ItemTemplate>
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:TemplateField>
                            <%--<asp:TemplateField HeaderText="Export">
                                <ItemTemplate>
                                    <asp:ImageButton runat="server" CommandName="export" CommandArgument='<%# Eval("reconciliation_batch_id") %>'
                                        ToolTip="Click to export reconciled transactions" ImageUrl="~/images/description.JPG"
                                        Width="18px" Height="14px" ID="Button_export" CssClass="ShowHand" />
                                </ItemTemplate>
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:TemplateField>--%>
                            <asp:TemplateField HeaderText="Rerun">
                                <ItemTemplate>
                                    <asp:ImageButton runat="server" CommandName="reExecute" CommandArgument='<%# Eval("reconciliation_batch_id") %>'
                                        ToolTip="Click to execute batch again" ImageUrl="~/images/description.JPG" Width="18px"
                                        Height="14px" ID="Button_reExecute" CssClass="ShowHand" />
                                </ItemTemplate>
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:TemplateField>
                        </Columns>
                        <RowStyle HorizontalAlign="Center" />
                        <FooterStyle BackColor="#CCCCCC" />
                        <SelectedRowStyle BackColor="#000099" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Left" />
                        <HeaderStyle Font-Bold="True" ForeColor="White" />
                        <%--<AlternatingRowStyle BackColor="#CCCCCC" />--%>
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
