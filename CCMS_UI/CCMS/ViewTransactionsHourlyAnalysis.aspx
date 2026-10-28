<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewTransactionsHourlyAnalysis.aspx.cs"
    Inherits="CCMSUI.CCMS.ViewTransactionsHourlyAnalysis" %>

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
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)' onload="hideProgressBar();">
    <form id="form1" runat="server">
    <div>
        <table style="width: 100%" cellspacing="0" cellpadding="0" border="0" class="mainbox">
            <tr>
                <td class="PageHeading" align="left">
                    View Transactions Hourly Analysis
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
                <td>
                    <table width="100%" class="SEARCHBOX" border="0">
                        <tr>
                            <td>
                                <table id="search" width="40%" border="0">
                                    <tr id="trCycles" runat="server">
                                        <td >
                                            Inactivity Threshold(Hour):
                                        </td>
                                        <td style="width: 190px; height: 17px">
                                            <asp:TextBox runat="server" ID="TextBox_TrxnThreshold" Width="119px" CssClass="textbox" />
                                            <asp:RegularExpressionValidator ID="RegularExpressionValidator7" CssClass="Validator"
                                                runat="server" ValidationExpression="\d*" ControlToValidate="TextBox_TrxnThreshold"
                                                ErrorMessage="Transaction threshold is invalid.">*</asp:RegularExpressionValidator>
                                            <asp:RequiredFieldValidator ID="requiredfieldvalidator_InactivityThreshold" runat="server"
                                                ControlToValidate="TextBox_TrxnThreshold" ErrorMessage="Please enter Inactivity threshold in hour.">*</asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="4" align="right" style="padding-right: 80px">
                                            <asp:Button CssClass="button" ID="button_search" runat="server" Text="Search" OnClick="button_search_Click" />
                                            &nbsp;<asp:Button ID="Button_Reset" runat="server" CssClass="button" OnClick="Button_Reset_Click"
                                                Text="Reset" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td valign="top" align="right">
                                <table>
                                    <tr>
                                        <td >
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
                    Selected ATMs -
                    <asp:Literal ID="Literal_count" runat="server" EnableViewState="False"></asp:Literal>
                </th>
            </tr>
            <tr>
                <td valign="top" colspan="2">
                    <asp:GridView ID="GridView_Threshold" runat="server" Width="100%" AutoGenerateColumns="false"
                        OnRowDataBound="GridView_Threshold_RowDataBound" AllowSorting="true" OnSorting="GridView_Threshold16_Sorting" CssClass="grid">
                        <AlternatingRowStyle BackColor="#eff0ef" />
                        <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                        <Columns>
                            <asp:BoundField DataField="title" HeaderText="Title" ItemStyle-HorizontalAlign="Center"
                                SortExpression="title" />
                            <asp:BoundField DataField="generated_at" HeaderText="Last Transaction" ItemStyle-HorizontalAlign="Center"
                                DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" SortExpression="generated_at" />
                            <asp:BoundField DataField="last_heart_beat_at" HeaderText="Last Heart Beat" ItemStyle-HorizontalAlign="Center"
                                DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" SortExpression="last_heart_beat_at" />
                            <asp:BoundField DataField="amount" HeaderText="Amount" ItemStyle-HorizontalAlign="Right"
                                SortExpression="amount" />
                        </Columns>
                        <EmptyDataTemplate>
                            No records found
                        </EmptyDataTemplate>
                    </asp:GridView>
                </td>
            </tr>
        </table>
    </div>
    <input type="button" runat="server" id="refresh" style="display: none" />
    </form>
</body>
</html>
