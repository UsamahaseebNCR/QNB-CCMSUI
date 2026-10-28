<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SMSTaskStatusMonitoring.aspx.cs"
    Inherits="Avanza.iSuite.UI.SMSTaskStatusMonitoring" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Untitled Page</title>
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
    <link href="../content/iSuite.css" rel="stylesheet" />
    <link href="../content/Scripts/ui-lightness/jquery.ui.all.css" rel="stylesheet" type="text/css" />
    <link href="../content/Scripts/ui-lightness/jquery-ui-1.8.17.custom.css" rel="stylesheet"
        type="text/css" />
    <link href="../content/Scripts/ui-lightness/ui-time-picker.css" rel="stylesheet"
        type="text/css" />
    <script type="text/javascript" src="../content/js/jquery.js"></script>
    <script type="text/javascript" src="../content/Scripts/jquery.ui.core.js"></script>
    <script src="../content/js/jquery.js" type="text/javascript"></script>
    <script src="../content/Scripts/jquery-ui-1.8.16.min.js" type="text/javascript"></script>
    <script type="text/javascript" src="../content/Scripts/jquery.ui.widget.js"></script>
    <script type="text/javascript" src="../content/Scripts/jquery.ui.datepicker.js"></script>
    <script type="text/javascript" src="../content/Scripts/jquery-ui-timepicker-addon.js"></script>
    <script type="text/javascript">
        $(function () {

            $('.datepicker').datetimepicker({ dateFormat: 'dd/mm/yy',

                dateFormat: 'dd/mm/yy',
                timeFormat: 'hh:mm:ss',
                defaultDate: -1,
                minDate: '-2m',
                maxDate: '+2d',
                hourGrid: 4,
                minuteGrid: 10,

                showOn: "button",
                buttonImage: "../images/calendar.gif",
                buttonImageOnly: true

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
                    SMS Task Status Monitoring ( History )
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
                                            From(Creation Time) :
                                        </td>
                                        <td style="width: 190px; height: 17px">
                                            <asp:TextBox runat="server" ID="TextBox_FromDate" CssClass="datepicker" />
                                            <%--<img src="../images/IconCalendar.gif" alt="Click to open calender" onclick="popUpCalendar(this,document.getElementById('<%=TextBox_FromDate.ClientID %>'),'dd/mm/yyyy');"
                                                style="border: 0; cursor: hand" id="IMG1" />--%>
                                            <asp:RegularExpressionValidator ID="ValFromDate" CssClass="Validator" runat="server"
                                                ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d (0[0-9]|1[0-9]|2[0-4]):([0-5]\d):([0-5]\d)"
                                                ControlToValidate="TextBox_FromDate" ErrorMessage="Creation Time From Date is invalid.">*</asp:RegularExpressionValidator>
                                        </td>
                                        <td style="width: 99px; height: 17px">
                                            To(Creation Time) :
                                        </td>
                                        <td style="height: 17px; width: 326px;">
                                            <asp:TextBox runat="server" ID="TextBox_ToDate" CssClass="datepicker" />
                                            <asp:RegularExpressionValidator ID="ValToDate" CssClass="Validator" runat="server"
                                                ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d (0[0-9]|1[0-9]|2[0-4]):([0-5]\d):([0-5]\d)"
                                                ControlToValidate="TextBox_ToDate" ErrorMessage="Creation Time To Date is invalid.">*</asp:RegularExpressionValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 120px; height: 17px">
                                            From(End Time) :
                                        </td>
                                        <td style="width: 190px; height: 17px">
                                            <asp:TextBox runat="server" ID="TextBox_EndTimeFromDate" CssClass="datepicker" />
                                            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" CssClass="Validator"
                                                runat="server" ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d (0[0-9]|1[0-9]|2[0-4]):([0-5]\d):([0-5]\d)"
                                                ControlToValidate="TextBox_EndTimeFromDate" ErrorMessage="End Time From Date is invalid.">*</asp:RegularExpressionValidator>
                                        </td>
                                        <td style="width: 99px; height: 17px">
                                            To(End Time) :
                                        </td>
                                        <td style="height: 17px; width: 326px;">
                                            <asp:TextBox runat="server" ID="TextBox_EndTimeToDate" CssClass="datepicker" />
                                            <asp:RegularExpressionValidator ID="RegularExpressionValidator2" CssClass="Validator"
                                                runat="server" ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d (0[0-9]|1[0-9]|2[0-4]):([0-5]\d):([0-5]\d)"
                                                ControlToValidate="TextBox_EndTimeToDate" ErrorMessage="End Time To Date is invalid.">*</asp:RegularExpressionValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Status
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="DropDownList_Status" runat="server" CssClass="dropDown">
                                                <asp:ListItem>Any</asp:ListItem>
                                                <asp:ListItem>Scheduled</asp:ListItem>
                                                <asp:ListItem>InquiryPending</asp:ListItem>
                                                <asp:ListItem>InquiryCompleted</asp:ListItem>
                                                <asp:ListItem>InquiryFailed</asp:ListItem>
                                                <asp:ListItem>SmsPending</asp:ListItem>
                                                <asp:ListItem>SmsFailed</asp:ListItem>
                                                <asp:ListItem>Completed</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td>
                                            Alert Type
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="DropDownList_AlertType" runat="server" CssClass="dropDown">
                                                <asp:ListItem>Any</asp:ListItem>
                                                <asp:ListItem>Customer Alert</asp:ListItem>
                                                <asp:ListItem>Staff Alert</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                        </td>
                                        <td>
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
                    Selected Tasks -
                    <asp:Literal ID="Literal_count" runat="server" EnableViewState="False"></asp:Literal>
                </th>
            </tr>
            <tr>
                <td valign="top" colspan="2">
                    <asp:GridView runat="server" ID="GridView_Files" AutoGenerateColumns="False" BackColor="White"
                        BorderStyle="Solid" BorderWidth="1px" CellPadding="3" ForeColor="Black" GridLines="Vertical"
                        PageSize="10" Width="100%" OnRowDataBound="GridView_Files_RowDataBound" AllowPaging="true"
                        AllowSorting="true" OnPageIndexChanging="GridView_Files_PageIndexChanging" 
                        OnSorting="GridView_Files_Sorting" 
                        onselectedindexchanged="GridView_Files_SelectedIndexChanged">
                        <AlternatingRowStyle BackColor="#eff0ef" />
                        <EmptyDataTemplate>
                            <center>
                                No Records Found</center>
                        </EmptyDataTemplate>
                        <RowStyle HorizontalAlign="Center" />
                        <Columns>
                            <asp:BoundField DataField="title" HeaderText="Title" SortExpression="title" ItemStyle-HorizontalAlign="Center">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            
                            <asp:BoundField DataField="transaction_friendly_name" HeaderText="Type" ReadOnly="True"
                                SortExpression="transaction_friendly_name" ItemStyle-HorizontalAlign="Center">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            

                             <asp:BoundField DataField="pan" HeaderText="PAN" ReadOnly="True"
                                SortExpression="pan" ItemStyle-HorizontalAlign="Center">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>

                             <asp:BoundField DataField="account_no" HeaderText="Account No" ReadOnly="True"
                                SortExpression="account_no" ItemStyle-HorizontalAlign="Center">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>

                             <asp:BoundField DataField="tsn" HeaderText="TSN" ReadOnly="True"
                                SortExpression="tsn" ItemStyle-HorizontalAlign="Center">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>

                            <asp:BoundField DataField="refrence_no" HeaderText="Inquiry Reference No" ReadOnly="True"
                                SortExpression="refrence_no" ItemStyle-HorizontalAlign="Center">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>

                            
                            <asp:BoundField DataField="sms_service_ref_no" HeaderText="SMS Reference No" 
                                SortExpression="sms_service_ref_no" ItemStyle-HorizontalAlign="Center">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>

                             <asp:BoundField DataField="user_full_name" HeaderText="User" SortExpression="user_full_name">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                             
                            <asp:BoundField DataField="status" HeaderText="Status" SortExpression="Status">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="retry_remaining" HeaderText="Retry Remaining" SortExpression="retry_remaining">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="creation_time" HeaderText="Creation Time" SortExpression="creation_time"
                                DataFormatString="{0:dd/MM/yy HH:mm:ss}" HtmlEncode="false">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="last_invoked_at" HeaderText="Last Invoked" SortExpression="last_invoked_at"
                                DataFormatString="{0:dd/MM/yy HH:mm:ss}" HtmlEncode="false">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="end_time" HeaderText="End Time" SortExpression="end_time"
                                DataFormatString="{0:dd/MM/yy HH:mm:ss}" HtmlEncode="false">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="failure_reason" HeaderText="Failure Reason" SortExpression="failure_reason">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
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
    </div>
    <input type="button" runat="server" id="refresh" style="display: none" />
    </form>
</body>
</html>
