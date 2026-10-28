<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DashboardGrid_CashPosVsDFF.aspx.cs"
    Inherits="CCMSUI.CCMS.DashBoards.DashboardGrid_CashPosVsDFF" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Cash Positions Vs DFF DashBoard</title>
    <link href="../../content/iSuite.css" rel="stylesheet" />
    <script src="../../content/shortcut.js" type="text/javascript" language="javascript"></script>
    <link href="../../content/Scripts/ui-lightness/jquery-ui-1.7.2.custom.css" rel="stylesheet"  type="text/css" />
    <script type="text/javascript" src="../../content/js/jquery.js"></script>
    <script type="text/javascript" src="../../content/scripts/ui.datepicker.js"></script>
    <script type="text/javascript">
        $(function () {

            $('.datepicker').datepicker({ dateFormat: 'dd/mm/yy',
                changeMonth: true,
                changeYear: true,
                showOn: "button",
                buttonImage: "../../images/calendar.gif",
                buttonImageOnly: true

            });
        });
    
    </script>
    <script type="text/javascript">
        function ShowDialog(title, fileStatus) {
            Dialog_ShowListOfFiles.Show();
            DialogCallBack_ShowListOfFiles.callback(title, fileStatus);
        }

        var mins;
        var secs;

        function cd(minutes) {
            mins = 1 * m(minutes); //change minutes here
            secs = 0 + s(":01");   //change seconds here (always add an additional second to your total)
            redo();
        }

        function m(obj) {
            for (var i = 0; i < obj.length; i++) {
                if (obj.substring(i, i + 1) == ":")
                    break;
            }

            return (obj.substring(0, i));
        }

        function s(obj) {
            for (var i = 0; i < obj.length; i++) {
                if (obj.substring(i, i + 1) == ":")
                    break;
            }

            return (obj.substring(i + 1, obj.length));
        }

        function dis(mins, secs) {
            var disp;
            if (mins <= 9) {
                disp = " 0";
            }
            else {
                disp = " ";
            }

            disp += mins + ":";
            if (secs <= 9) {
                disp += "0" + secs;
            }
            else {
                disp += secs;
            }

            return (disp);
        }

        function redo() {
            secs--;
            if (secs == -1) {
                secs = 59;
                mins--;
            }

            window.status = "Count Down " + dis(mins, secs); // setup additional displays here.

            if ((mins == 0) && (secs == 0)) {
            }
            else {
                cd = setTimeout("redo()", 1000);
            }
        }

        function ToggleOptionView() {
            var optionsPanel = document.getElementById('optionsPanel');

            if (optionsPanel.style.display == 'none') {
                optionsPanel.style.display = 'block';
                document.getElementById('viewButton').value = 'Hide Options';
            }
            else {
                optionsPanel.style.display = 'none';
                document.getElementById('viewButton').value = 'Show Options';
            }
        }
    </script>
    <style type="text/css">
        .style1
        {
            width: 15%;
        }
        .style2
        {
            width: 5%;
        }
    </style>
</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)'>
    <form id="form1" runat="server">
    <div>
        <table id="table3" style="width: 100%;" cellspacing="0" cellpadding="0" border="0">
            <tr>
                <td class="PageHeading" colspan="2">
                    Cash Positions Vs DFF
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
                                        <td class="style2">
                                            From :
                                        </td>
                                        <td class="style1">
                                            <asp:TextBox runat="server" ID="TextBox_FromDate" CssClass="datepicker" Font-Names="Verdana"
                                                Font-Size="10px" />
                                            <asp:RegularExpressionValidator ID="ValFromDate" CssClass="Validator" runat="server"
                                                ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                                ControlToValidate="TextBox_FromDate" ErrorMessage="From Date is invalid.">*</asp:RegularExpressionValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            &nbsp;
                                        </td>
                                        <td>
                                            <asp:Button ID="Button_Submit" runat="server" Text="Submit" OnClick="Button_Submit_Click"
                                                CssClass="button" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td valign="top" align="right">
                                <table>
                                    <tr>
                                        <td>
                                            <img src="../../images/Icon_search.png" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        <table>
            <tr>
                <td style="vertical-align: top; width: 87%">
                    <asp:GridView ID="GridView_CashPositionVsDFF" runat="server" AutoGenerateColumns="False"
                        Font-Size="Smaller" Width="100%" AllowSorting="true" OnSorting="GridView_CashPositionVsDFF_Sorting"
                        OnRowDataBound="GridView_CashPositionVsDFF_RowDataBound" >
                        <HeaderStyle ForeColor="Black" />
                        <EmptyDataTemplate>
                            <center>
                                No Rows Found</center>
                        </EmptyDataTemplate>
                        <Columns>
                            <asp:BoundField DataField="title" HeaderText="ATM" ReadOnly="True" ItemStyle-HorizontalAlign="Center"
                                SortExpression="title"></asp:BoundField>
                            <asp:BoundField DataField="isSwap" HeaderText="Def Rep" ReadOnly="True"
                                ItemStyle-HorizontalAlign="Center" SortExpression="title"></asp:BoundField>
                            <asp:BoundField DataField="last_trxn_at" HeaderText="Last Transaction At" ItemStyle-HorizontalAlign="Center"
                                DataFormatString="{0:dd/MM/yy HH:mm:ss}" HtmlEncode="false" SortExpression="p.last_trxn_at">
                            </asp:BoundField>
                            <asp:BoundField DataField="last_replenished_at" HeaderText="Last Replenishement At"
                                ItemStyle-HorizontalAlign="Center" DataFormatString="{0:dd/MM/yy HH:mm:ss}" HtmlEncode="false"
                                SortExpression="last_replenished_at"></asp:BoundField>
                            <asp:BoundField DataField="total" HeaderText="Total" ItemStyle-HorizontalAlign="Center"
                                SortExpression="total"></asp:BoundField>
                            <asp:BoundField DataField="closing_balance" HeaderText="Closing Balance in DFF" ItemStyle-HorizontalAlign="Center"
                                SortExpression="closing_balance"></asp:BoundField>
                            <asp:BoundField DataField="diff" HeaderText="Difference" ItemStyle-HorizontalAlign="Center"
                                SortExpression="diff"></asp:BoundField>
                            <asp:BoundField DataField="test_cash_amount" HeaderText="Test Cash Rejected Bin Amount"
                                ItemStyle-HorizontalAlign="Center" SortExpression="test_cash_amount"></asp:BoundField>
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <a onclick="window.open('../CashPositionVsDFFDetail.aspx?a=<%#Eval("atm_id")%>')"><img src="../../images/Edit.gif" /></a>
                                    </ItemTemplate>
                                </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </td>
                <td valign="top">
                    <table>
                        <tr>
                            <td>
                                <b>Matched</b>
                            </td>
                            <td>
                                <asp:Label ID="Label_Matched" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <b>Positive Suspect</b>
                            </td>
                            <td>
                                <asp:Label ID="Label_PosSuspect" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <b>Negative Suspect</b>
                            </td>
                            <td>
                                <asp:Label ID="Label_NegSuspect" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                ONLY for Swap Machines
                            </td>
                        </tr>

                        <tr>
                            <td>
                                <b>Matched</b>
                            </td>
                            <td>
                                <asp:Label ID="Label_SwapMatched" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <b>Positive Suspect</b>
                            </td>
                            <td>
                                <asp:Label ID="Label_SwapPosSuspect" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <b>Negative Suspect</b>
                            </td>
                            <td>
                                <asp:Label ID="Label_SwapNegSuspect" runat="server"></asp:Label>
                            </td>
                        </tr>

                    </table>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
