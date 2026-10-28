<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewDailyFeed.aspx.cs"
    Inherits="CCMSUI.CCMS.ViewDailyFeed" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Untitled Page</title>
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
    <link href="../content/iSuite.css" rel="stylesheet" />
    <script type="text/javascript">
        //window.parent.window.SetContentPage( window.location);
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
                    Recreate Daily Feed
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
                                        <td style="width: 120px; height: 17px">
                                            From :
                                        </td>
                                        <td style="width: 190px; height: 17px">
                                           <asp:TextBox runat="server" ID="TextBox_FromDate" CssClass="datepicker" Font-Names="Verdana"
    Font-Size="10px" />
                                            <asp:RegularExpressionValidator ID="ValFromDate" CssClass="Validator" runat="server"
                                                ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                                ControlToValidate="TextBox_FromDate" ErrorMessage="From Date is invalid.">*</asp:RegularExpressionValidator>
                                        </td>
                                        <td style="width: 99px; height: 17px">
                                            To :
                                        </td>
                                        <td style="height: 17px; width: 326px;">
                                             <asp:TextBox runat="server" ID="TextBox_ToDate" CssClass="datepicker" Font-Names="Verdana"
     Font-Size="10px" />
                                            <asp:RegularExpressionValidator ID="ValToDate" CssClass="Validator" runat="server"
                                                ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                                ControlToValidate="TextBox_ToDate" ErrorMessage="To Date is invalid.">*</asp:RegularExpressionValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="4" align="right" style="padding-right: 160px; padding-top: 10px;">
                                            <asp:Button CssClass="button" ID="button_search" runat="server" Text="Search" OnClick="button_search_Click" />
                                            &nbsp;<asp:Button ID="Button_Reset" runat="server" CssClass="button" OnClick="Button_Reset_Click"
                                                Text="Reset" />&nbsp;
                                            <asp:HyperLink ID="HyperLink_newSchedules" runat="server" CssClass="Link" NavigateUrl="ScheduleDailyFeed.aspx">Create New Schedule... </asp:HyperLink>
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
                    Selected Schedules -
                    <asp:Literal ID="Literal_count" runat="server" EnableViewState="False"></asp:Literal>
                </th>
            </tr>
            <tr>
                <td valign="top" colspan="2">
                    <asp:GridView runat="server" ID="GridView_Schedules" AutoGenerateColumns="False"
                        BackColor="White" BorderStyle="Solid" BorderWidth="1px" CellPadding="3" ForeColor="Black"
                        GridLines="Vertical" PageSize="25" Width="100%" AllowPaging="true" AllowSorting="true"
                        OnPageIndexChanging="GridView_Schedules_PageIndexChanging" OnSorting="GridView_Schedules_Sorting"
                        OnRowCommand="GridView_Schedules_RowCommand" OnRowDataBound="GridView_Schedules_RowDataBound">
                        <EmptyDataTemplate>
                            <center>
                                No Records Found</center>
                        </EmptyDataTemplate>
                        <RowStyle HorizontalAlign="Center" />
                        <Columns>
                            <asp:BoundField DataField="mcn" HeaderText="MCN" ReadOnly="True" SortExpression="mcn"
                                ItemStyle-HorizontalAlign="Center">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="date_from" HeaderText="Date From" ReadOnly="True" SortExpression="date_from"
                                DataFormatString="{0:dd/MM/yyyy}" HtmlEncode="false">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="date_to" HeaderText="Date To" SortExpression="date_to"
                                DataFormatString="{0:dd/MM/yyyy}" HtmlEncode="false">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="retry_count" HeaderText="Retry Remaining" SortExpression="retry_count">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="failure_reason" HeaderText="Failure Reason" SortExpression="failure_reason">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:CheckBoxField DataField="is_executed" HeaderText="Executed">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:CheckBoxField>
                            <asp:TemplateField HeaderText="Delete">
                                <ItemTemplate>
                                    <asp:ImageButton runat="server" CommandName="deleteDailyFeedSchedule" CommandArgument='<%# Eval("daily_feed_schedule_id") %>'
                                        ToolTip="Click to delete schedule" ImageUrl="../images/Delete.gif" ID="Button_deleteSchedule"
                                        CssClass="ShowHand" />
                                </ItemTemplate>
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:TemplateField>
                        </Columns>
                        <RowStyle HorizontalAlign="Center" />
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
