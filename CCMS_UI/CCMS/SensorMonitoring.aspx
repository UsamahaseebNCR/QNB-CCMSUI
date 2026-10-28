<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SensorMonitoring.aspx.cs"
    Inherits="CCMSUI.CCMS.SensorMonitoring" %>

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
    <script type="text/javascript" src="../content/Scripts/jquery-1.7.1.js"></script>
    <script type="text/javascript" src="../content/Scripts/jquery.ui.core.js"></script>
    <script src="../content/Scripts/jquery-1.7.1.min.js" type="text/javascript"></script>
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
                    Sensor Monitoring ( History )
                </td>
            </tr>
            <tr>
                <td class="ValidationText" align="center">
                    <asp:literal id="Literal_error" runat="server" enableviewstate="False"></asp:literal>
                </td>
            </tr>
            <tr>
                <td class="ConfirmationText" align="center">
                    <asp:validationsummary id="valSummary" runat="server" />
                    <%=Session[SessionVars.notice.ToString()]%>
                    <%Session[SessionVars.notice.ToString()] = "";%>
                    <asp:literal id="Literal_ok" runat="server" enableviewstate="False"></asp:literal>
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
                                            From:
                                        </td>
                                        <td style="width: 190px; height: 17px">
                                            <asp:textbox runat="server" id="TextBox_FromDate" cssclass="datepicker" />
                                            <%--<img src="../images/IconCalendar.gif" alt="Click to open calender" onclick="popUpCalendar(this,document.getElementById('<%=TextBox_FromDate.ClientID %>'),'dd/mm/yyyy');"
                                                style="border: 0; cursor: hand" id="IMG1" />--%>
                                            <asp:regularexpressionvalidator id="ValFromDate" cssclass="Validator" runat="server"
                                                validationexpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d (0[0-9]|1[0-9]|2[0-4]):([0-5]\d):([0-5]\d)"
                                                controltovalidate="TextBox_FromDate" errormessage="Creation Time From Date is invalid.">*</asp:regularexpressionvalidator>
                                        </td>
                                        <td style="width: 99px; height: 17px">
                                            To:
                                        </td>
                                        <td style="height: 17px; width: 326px;">
                                            <asp:textbox runat="server" id="TextBox_ToDate" cssclass="datepicker" />
                                            <asp:regularexpressionvalidator id="ValToDate" cssclass="Validator" runat="server"
                                                validationexpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d (0[0-9]|1[0-9]|2[0-4]):([0-5]\d):([0-5]\d)"
                                                controltovalidate="TextBox_ToDate" errormessage="Creation Time To Date is invalid.">*</asp:regularexpressionvalidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 120px">
                                            Sensor :
                                        </td>
                                        <td style="width: 190px">
                                            <asp:dropdownlist id="Dropdownlist_Sensor" runat="server" enableviewstate="true"
                                                 cssclass="textbox" width="144px" >
                                            </asp:dropdownlist>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2" align="center" >
                                            <asp:button cssclass="button" id="button_search" runat="server" text="Search" onclick="button_search_Click" />
                                            &nbsp;<asp:button id="Button_Reset" runat="server" cssclass="button" onclick="Button_Reset_Click"
                                                text="Reset" />
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
                    Selected -
                    <asp:literal id="Literal_count" runat="server" enableviewstate="False"></asp:literal>
                </th>
            </tr>
            <tr>
                <td valign="top" colspan="2">
                    <asp:gridview runat="server" id="GridView_Sensor" autogeneratecolumns="False" backcolor="White"
                        borderstyle="Solid" borderwidth="1px" cellpadding="3" forecolor="Black" gridlines="Vertical"
                        pagesize="10" width="100%" allowpaging="true" allowsorting="true" onpageindexchanging="GridView_Sensor_PageIndexChanging"
                        onsorting="GridView_Sensor_Sorting">
                    <AlternatingRowStyle BackColor="#eff0ef" />
                    <EmptyDataTemplate>
                        <center>
                            No Records Found</center>
                    </EmptyDataTemplate>
                    <RowStyle HorizontalAlign="Center" />
                    <Columns>
                        <asp:BoundField DataField="title" HeaderText="Title" ReadOnly="True" SortExpression="title"
                            ItemStyle-HorizontalAlign="Center">
                            <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                        </asp:BoundField>
                        <asp:BoundField DataField="name" HeaderText="Sensor Name" SortExpression="name">
                            <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                        </asp:BoundField>
                         <asp:BoundField DataField="from_time" HeaderText="From Time" SortExpression="from_time"
                            DataFormatString="{0:dd/MM/yy HH:mm:ss}" HtmlEncode="false">
                            <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                        </asp:BoundField>
                        <asp:BoundField DataField="to_time" HeaderText="To Time" SortExpression="to_time"
                            DataFormatString="{0:dd/MM/yy HH:mm:ss}" HtmlEncode="false">
                            <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                        </asp:BoundField>
                         <asp:BoundField DataField="val" HeaderText="Value" SortExpression="val">
                            <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                        </asp:BoundField>
                    </Columns>
                    <RowStyle HorizontalAlign="Center" />
                    <FooterStyle BackColor="#CCCCCC" />
                    <SelectedRowStyle BackColor="#000099" Font-Bold="True" ForeColor="White" />
                    <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Left" />
                    <HeaderStyle Font-Bold="True" ForeColor="White" />
                    <%--<AlternatingRowStyle BackColor="#CCCCCC" />--%>
                </asp:gridview>
                    &nbsp;
                </td>
            </tr>
        </table>
    </div>
    <input type="button" runat="server" id="refresh" style="display: none" />
    </form>
</body>
</html>
