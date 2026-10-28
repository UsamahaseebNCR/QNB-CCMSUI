<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CashPositions.aspx.cs"
    Inherits="CCMSUI.CCMS.CashPositions" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="../content/isuite.css" rel="stylesheet" />
    <title>Cash Positions</title>
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
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)'
    onload="hideProgressBar();">
    <form id="form1" runat="server">
    <div>
        <table style="width: 100%" cellspacing="0" cellpadding="0" border="0" class="mainbox">
            <tr>
                <td class="PageHeading" align="left">
                    <%= Request.QueryString["ej"]==null?"Cash Positions":"EJ Cash Positions" %>
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
                <td style="height: 20px; padding-left: 5px; width: 100%" colspan="2">
                    <b>Search Filters</b>
                </td>
            </tr>
            <tr align="left">
                <td colspan="2" style="width: 100%">
                    <table width="100%" class="SEARCHBOX" border="0">
                        <tr>
                            <td>
                                <table id="search" width="100%" border="0">
                                    <tr>
                                        <td>
                                            From :
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="TextBox_FromDate" CssClass="datepicker" />
                                            <asp:RegularExpressionValidator ID="ValFromDate" CssClass="Validator" runat="server"
                                                ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d (0[0-9]|1[0-9]|2[0-4]):([0-5]\d):([0-5]\d)"
                                                ControlToValidate="TextBox_FromDate" ErrorMessage="From Date is invalid.">*</asp:RegularExpressionValidator>
                                        </td>
                                        <td style="width: 99px; height: 17px">
                                            To :
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="TextBox_ToDate" CssClass="datepicker" />
                                            <asp:RegularExpressionValidator ID="ValToDate" CssClass="Validator" runat="server"
                                                ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d (0[0-9]|1[0-9]|2[0-4]):([0-5]\d):([0-5]\d)"
                                                ControlToValidate="TextBox_ToDate" ErrorMessage="To Date is invalid.">*</asp:RegularExpressionValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                       
                                        <td class="style4">
                                            Group:
                                        </td>
                                        <td >
                                            <%-- <asp:DropDownList ID="DropDownList_NoteSetTypeID" runat="server" CssClass="textbox" AppendDataBoundItems="true" Width="125px">
                                                <asp:ListItem>Select</asp:ListItem>
                                            </asp:DropDownList>--%>
                                            <div style="overflow: auto; background-color:InfoBackground">
                                                <asp:CheckBoxList ID="groupList" runat="server" RepeatDirection="Horizontal" RepeatColumns="4">
                                                </asp:CheckBoxList>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                        </td>
                                        <td>
                                        </td>
                                        <td colspan="2">
                                            <asp:Button ID="Button_search" runat="server" CssClass="button" OnClick="Button_search_Click"
                                                Text="Search" Width="71px" />
                                            &nbsp;<asp:Button ID="Button_Reset" runat="server" CssClass="button" OnClick="Button_Reset_Click"
                                                Text="Reset" />&nbsp; &nbsp;<asp:Button ID="Button_ExportToPDF" runat="server" CssClass="button"
                                                    OnClick="Button_ExportToPDF_Click" Text="Export To PDF" />
                                            &nbsp; &nbsp;<asp:Button ID="Button_ExportToXcel" runat="server" CssClass="button"
                                                OnClick="Button_ExportToExcel_Click" Text="Export To Excel" />
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
                <th colspan="2" style="width: 100%" align="left">
                    Selected Cash Positions -
                    <asp:Literal ID="Literal_count" runat="server" EnableViewState="False"></asp:Literal>
                </th>
            </tr>
            <tr>
                <td valign="top" colspan="2" style="width: 100%">
                    <asp:GridView ID="GridView_CashPositions" runat="server" AutoGenerateColumns="False"
                        AllowSorting="true" OnSorting="GridView_CashPositions_Sorting" Width="100%" AllowPaging="True"
                        OnPageIndexChanging="GridView_CashPositions_PageIndexChanging" HeaderStyle-CssClass="HeadingCell"
                        HeaderStyle-ForeColor="black" PageSize="25">
                        <RowStyle HorizontalAlign="Center" />
                        <AlternatingRowStyle BackColor="#eff0ef" />
                        <Columns>
                            <asp:BoundField DataField="title" HeaderText="ATM" ReadOnly="True" ItemStyle-HorizontalAlign="Center"
                                SortExpression="title"></asp:BoundField>
                            <asp:BoundField DataField="location" HeaderText="Location" ReadOnly="True" ItemStyle-HorizontalAlign="Center"
                                SortExpression="location"></asp:BoundField>
                            <asp:BoundField DataField="note_set_type_name" HeaderText="Group" ReadOnly="True"
                                ItemStyle-HorizontalAlign="Center" SortExpression="note_set_type_name"></asp:BoundField>
                            <asp:BoundField DataField="last_trxn_at" HeaderText="Datetime" ItemStyle-HorizontalAlign="Center"
                                DataFormatString="{0:dd/MM/yy HH:mm:ss}" HtmlEncode="false" SortExpression="last_trxn_at">
                            </asp:BoundField>
                            <asp:BoundField DataField="last_successful_trxn_at" HeaderText="Last Transaction"
                                ItemStyle-HorizontalAlign="Center" DataFormatString="{0:dd/MM/yy HH:mm:ss}" HtmlEncode="false"
                                SortExpression="last_successful_trxn_at" Visible="false"></asp:BoundField>
                            <%--<asp:BoundField DataField="last_invoked" HeaderText="Last Invoked" ItemStyle-HorizontalAlign="Center" DataFormatString="{0:dd/MM/yy HH:mm:ss}" HtmlEncode="false"
                                 SortExpression="last_invoked"></asp:BoundField>--%>
                            <asp:BoundField DataField="last_replenishment_at" HeaderText="Last Replenished" ItemStyle-HorizontalAlign="Center"
                                DataFormatString="{0:dd/MM/yy HH:mm:ss}" HtmlEncode="false" SortExpression="last_replenishment_at">
                            </asp:BoundField>
                            <asp:BoundField DataField="denomination_type_1" HeaderText="Type 1" ItemStyle-HorizontalAlign="Center"
                                SortExpression="denomination_type_1"></asp:BoundField>
                            <asp:BoundField DataField="cassette1_notes" HeaderText="Remaining" ItemStyle-HorizontalAlign="Center"
                                SortExpression="cassette1_notes"></asp:BoundField>
                            <asp:BoundField DataField="purge_cassette1_notes" HeaderText="Purged" ItemStyle-HorizontalAlign="Center"
                                SortExpression="purge_cassette1_notes"></asp:BoundField>
                            <asp:BoundField DataField="denomination_type_2" HeaderText="Type 2" ItemStyle-HorizontalAlign="Center"
                                SortExpression="denomination_type_2"></asp:BoundField>
                            <asp:BoundField DataField="cassette2_notes" HeaderText="Remaining" ItemStyle-HorizontalAlign="Center"
                                SortExpression="cassette2_notes"></asp:BoundField>
                            <asp:BoundField DataField="purge_cassette2_notes" HeaderText="Purged" ItemStyle-HorizontalAlign="Center"
                                SortExpression="purge_cassette2_notes"></asp:BoundField>
                            <asp:BoundField DataField="denomination_type_3" HeaderText="Type 3" ItemStyle-HorizontalAlign="Center"
                                SortExpression="denomination_type_3"></asp:BoundField>
                            <asp:BoundField DataField="cassette3_notes" HeaderText="Remaining" ItemStyle-HorizontalAlign="Center"
                                SortExpression="cassette3_notes"></asp:BoundField>
                            <asp:BoundField DataField="purge_cassette3_notes" HeaderText="Purged" ItemStyle-HorizontalAlign="Center"
                                SortExpression="purge_cassette3_notes"></asp:BoundField>
                            <asp:BoundField DataField="denomination_type_4" HeaderText="Type 4" ItemStyle-HorizontalAlign="Center"
                                SortExpression="denomination_type_4"></asp:BoundField>
                            <asp:BoundField DataField="cassette4_notes" HeaderText="Remaining" ItemStyle-HorizontalAlign="Center"
                                SortExpression="cassette4_notes"></asp:BoundField>
                            <asp:BoundField DataField="purge_cassette4_notes" HeaderText="Purged" ItemStyle-HorizontalAlign="Center"
                                SortExpression="purge_cassette4_notes"></asp:BoundField>
                            <asp:BoundField DataField="denomination_type_5" HeaderText="Type 5" ItemStyle-HorizontalAlign="Center"
                                Visible="false" SortExpression="denomination_type_5"></asp:BoundField>
                            <asp:BoundField DataField="cassette5_notes" HeaderText="Remaining" ItemStyle-HorizontalAlign="Center"
                                Visible="false" SortExpression="cassette5_notes"></asp:BoundField>
                            <asp:BoundField DataField="purge_cassette5_notes" HeaderText="Purged" ItemStyle-HorizontalAlign="Center"
                                Visible="false" SortExpression="purge_cassette5_notes"></asp:BoundField>
                            <asp:BoundField DataField="denomination_type_6" HeaderText="Type 6" ItemStyle-HorizontalAlign="Center"
                                Visible="false" SortExpression="denomination_type_6"></asp:BoundField>
                            <asp:BoundField DataField="cassette6_notes" HeaderText="Remaining" ItemStyle-HorizontalAlign="Center"
                                Visible="false" SortExpression="cassette6_notes"></asp:BoundField>
                            <asp:BoundField DataField="purge_cassette6_notes" HeaderText="Purged" ItemStyle-HorizontalAlign="Center"
                                Visible="false" SortExpression="purge_cassette6_notes"></asp:BoundField>
                            <asp:BoundField DataField="denomination_type_7" HeaderText="Type 7" ItemStyle-HorizontalAlign="Center"
                                Visible="false" SortExpression="denomination_type_7"></asp:BoundField>
                            <asp:BoundField DataField="cassette7_notes" HeaderText="Remaining" ItemStyle-HorizontalAlign="Center"
                                Visible="false" SortExpression="cassette7_notes"></asp:BoundField>
                            <asp:BoundField DataField="purge_cassette7_notes" HeaderText="Purged" ItemStyle-HorizontalAlign="Center"
                                Visible="false" SortExpression="purge_cassette7_notes"></asp:BoundField>
                            <%--<asp:BoundField DataField="total" HeaderText="Total Remaining" ItemStyle-HorizontalAlign="Center"
                                SortExpression="total"></asp:BoundField>--%>

                            <asp:BoundField DataField="total_text" HeaderText="Total Remaining" ItemStyle-HorizontalAlign="Center"
                                SortExpression="total" HtmlEncode="false"></asp:BoundField>

                            <%--<asp:BoundField DataField="denomination_type_1" HeaderText="Type 1" ItemStyle-HorizontalAlign="Center"
                                SortExpression="denomination_type_1"></asp:BoundField>
                            <asp:BoundField DataField="purge_cassette1_notes" HeaderText="Purged" ItemStyle-HorizontalAlign="Center"
                                SortExpression="purge_cassette1_notes"></asp:BoundField>
                            <asp:BoundField DataField="denomination_type_2" HeaderText="Type 2" ItemStyle-HorizontalAlign="Center"
                                SortExpression="denomination_type_2"></asp:BoundField>
                            <asp:BoundField DataField="purge_cassette2_notes" HeaderText="Purged" ItemStyle-HorizontalAlign="Center"
                                SortExpression="purge_cassette2_notes"></asp:BoundField>
                            <asp:BoundField DataField="denomination_type_3" HeaderText="Type 3" ItemStyle-HorizontalAlign="Center"
                                SortExpression="denomination_type_3"></asp:BoundField>
                            <asp:BoundField DataField="purge_cassette3_notes" HeaderText="Purged" ItemStyle-HorizontalAlign="Center"
                                SortExpression="purge_cassette3_notes"></asp:BoundField>
                            <asp:BoundField DataField="denomination_type_4" HeaderText="Type 4" ItemStyle-HorizontalAlign="Center"
                                SortExpression="denomination_type_4"></asp:BoundField>
                            <asp:BoundField DataField="purge_cassette4_notes" HeaderText="Purged" ItemStyle-HorizontalAlign="Center"
                                SortExpression="purge_cassette4_notes"></asp:BoundField>--%>
                            <asp:BoundField DataField="totalPurged" HeaderText="Total Purged" ItemStyle-HorizontalAlign="Center"
                                SortExpression="totalPurged"></asp:BoundField>
                            <asp:BoundField DataField="next_replenishment_at" HeaderText="Next Replenishment At"
                                ItemStyle-HorizontalAlign="Center" DataFormatString="{0:dd/MM/yy HH:mm:ss}" HtmlEncode="false"
                                SortExpression="next_replenishment_at" Visible="false"></asp:BoundField>
                            <asp:BoundField DataField="amount" HeaderText="Amount" ItemStyle-HorizontalAlign="Center"
                                SortExpression="amount" Visible="false"></asp:BoundField>
                            
                        </Columns>
                        <EmptyDataTemplate>
                            No Records Found!</EmptyDataTemplate>
                        <%-- <HeaderStyle BackColor="Black" ForeColor="White" CssClass="HeadingCell" />--%>
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
