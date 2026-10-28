<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DashBoardGrid_EJCashPositions.aspx.cs"
    Inherits="CCMSUI.CCMS.DashBoards.DashBoardGrid_EJCashPositions" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title><asp:Literal ID="Literal_AddHeading" runat="server"></asp:Literal>
    </title>
    <meta http-equiv="Refresh" content="300" id="metaRefresh" runat="server" />
    <script src="../../content/boxover.js" type="text/javascript" language="javascript">
    </script>
    <link href="../../content/iSuite.css" rel="stylesheet" />
    <link href="../../content/cal/popcalendar.css" type="text/css" rel="stylesheet" />
    <script src="../../content/boxover.js" type="text/javascript" language="javascript">
    </script>
    <script language="javascript" src="../../content/cal/popcalendar.js" type="text/javascript"></script>
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
</head>
<body style="margin: 0; background-image: url(../../images/dashboard_bg.jpg)">
    <form id="form1" runat="server">
    <div>
        <asp:Literal ID="Literal_errorMsg" runat="server" EnableViewState="false"></asp:Literal>
        <table width="100%" cellpadding="0" cellspacing="0">
            <tr>
                <td style="vertical-align: top">
                    <asp:GridView ID="GridView_CashPositions" runat="server" AutoGenerateColumns="False"
                        Font-Size="Smaller" Width="100%" AllowSorting="true" OnSorting="GridView_CashPositions_Sorting" 
                        DataKeyNames="atm_id" OnRowDataBound="GridView_CashPositions_RowDataBound">
                        <HeaderStyle ForeColor="white" />
                        <EmptyDataTemplate>
                            <center>
                                No Rows Found</center>
                        </EmptyDataTemplate>
                        <Columns>
                            <asp:TemplateField SortExpression="min_operating_balance" HeaderText="MOB">
                                <ItemTemplate>
                                    <asp:Image ID="img" runat="server" Width="20" Height="20" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="title" HeaderText="ATM" ReadOnly="True" ItemStyle-HorizontalAlign="Center"
                                SortExpression="title"></asp:BoundField>
                            <asp:BoundField DataField="last_trxn_at" HeaderText="Last Transaction" ItemStyle-HorizontalAlign="Center"
                                DataFormatString="{0:dd/MM/yy HH:mm:ss}" HtmlEncode="false" SortExpression="last_trxn_at">
                            </asp:BoundField>
                           <%-- <asp:BoundField DataField="last_invoked" HeaderText="Last Invoked" ItemStyle-HorizontalAlign="Center"
                                DataFormatString="{0:dd/MM/yy HH:mm:ss}" HtmlEncode="false" SortExpression="last_invoked">
                            </asp:BoundField>--%>
                            <asp:BoundField DataField="last_replenished_at" HeaderText="Last Replenished At"
                                ItemStyle-HorizontalAlign="Center" DataFormatString="{0:dd/MM/yy HH:mm:ss}" HtmlEncode="false"
                                SortExpression="last_invoked"></asp:BoundField>
                            <asp:BoundField DataField="denomination_type_1" HeaderText="Type 1" ItemStyle-HorizontalAlign="Center"
                                ItemStyle-BackColor="Chocolate" ItemStyle-ForeColor="white" SortExpression="denomination_type_1">
                            </asp:BoundField>
                            <asp:BoundField DataField="cassette1_notes" HeaderText="Remaining" ItemStyle-HorizontalAlign="Center"
                                SortExpression="cassette1_notes" ItemStyle-BackColor="Chocolate" ItemStyle-ForeColor="white"
                                HeaderStyle-BackColor="Chocolate" HeaderStyle-ForeColor="white"></asp:BoundField>
                            <asp:BoundField DataField="denomination_type_2" HeaderText="Type 2" ItemStyle-HorizontalAlign="Center"
                                ItemStyle-BackColor="Info" SortExpression="denomination_type_2"></asp:BoundField>
                            <asp:BoundField DataField="cassette2_notes" HeaderText="Remaining" ItemStyle-HorizontalAlign="Center"
                                SortExpression="cassette2_notes" ItemStyle-BackColor="info"></asp:BoundField>
                            <asp:BoundField DataField="denomination_type_3" HeaderText="Type 3" ItemStyle-HorizontalAlign="Center"
                                ItemStyle-BackColor="gray" ItemStyle-ForeColor="white" SortExpression="denomination_type_3">
                            </asp:BoundField>
                            <asp:BoundField DataField="cassette3_notes" HeaderText="Remaining" ItemStyle-HorizontalAlign="Center"
                                SortExpression="cassette3_notes" ItemStyle-BackColor="gray" ItemStyle-ForeColor="white">
                            </asp:BoundField>
                            <asp:BoundField DataField="denomination_type_4" HeaderText="Type 4" ItemStyle-HorizontalAlign="Center"
                                ItemStyle-BackColor="Brown" ItemStyle-ForeColor="white" SortExpression="denomination_type_4">
                            </asp:BoundField>
                            <asp:BoundField DataField="cassette4_notes" HeaderText="Remaining" ItemStyle-HorizontalAlign="Center"
                                SortExpression="cassette4_notes" ItemStyle-BackColor="Brown" ItemStyle-ForeColor="white">
                            </asp:BoundField>
                            <asp:BoundField DataField="denomination_type_5" HeaderText="Type 5" ItemStyle-HorizontalAlign="Center"
                                Visible="false" SortExpression="denomination_type_5"></asp:BoundField>
                            <asp:BoundField DataField="cassette5_notes" HeaderText="Remaining" ItemStyle-HorizontalAlign="Center"
                                Visible="false" SortExpression="cassette5_notes"></asp:BoundField>
                            <asp:BoundField DataField="denomination_type_6" HeaderText="Type 6" ItemStyle-HorizontalAlign="Center"
                                Visible="false" SortExpression="denomination_type_6"></asp:BoundField>
                            <asp:BoundField DataField="cassette6_notes" HeaderText="Remaining" ItemStyle-HorizontalAlign="Center"
                                Visible="false" SortExpression="cassette6_notes"></asp:BoundField>
                            <asp:BoundField DataField="denomination_type_7" HeaderText="Type 7" ItemStyle-HorizontalAlign="Center"
                                Visible="false" SortExpression="denomination_type_7"></asp:BoundField>
                            <asp:BoundField DataField="cassette7_notes" HeaderText="Remaining" ItemStyle-HorizontalAlign="Center"
                                Visible="false" SortExpression="cassette7_notes"></asp:BoundField>
                            <asp:BoundField DataField="total" HeaderText="Total" ItemStyle-HorizontalAlign="Center"
                                SortExpression="total"></asp:BoundField>
                            <%--<asp:TemplateField>
                                    
                                    <HeaderTemplate>
                                        <div title="header=[Purged Notes] body=[Number of notes purged during dispensed.]"
                                            style="color: White" >
                                            Purged Notes
                                        </div>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <%#Eval("purged_counts")%>
                                    </ItemTemplate>
                                </asp:TemplateField>--%>
                            <asp:BoundField DataField="purged_counts" HeaderText="<span title='Number Of Notes Rejected During Dispensed'>Purged Notes</span>"
                                ItemStyle-HorizontalAlign="Center" SortExpression="purged_counts" HtmlEncode="false">
                            </asp:BoundField>
                            <asp:BoundField DataField="purged_amount" HeaderText="Purged Amount" ItemStyle-HorizontalAlign="Center"
                                SortExpression="purged_amount"></asp:BoundField>
                            <asp:BoundField DataField="next_replenished_at" HeaderText="Next Replenishment At"
                                ItemStyle-HorizontalAlign="Center" DataFormatString="{0:dd/MM/yy HH:mm:ss}" HtmlEncode="false"
                                SortExpression="next_replenished_at"></asp:BoundField>
                            <asp:BoundField DataField="amount" HeaderText="Amount" ItemStyle-HorizontalAlign="Center"
                                SortExpression="amount"></asp:BoundField>
                        </Columns>
                      
                    </asp:GridView>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
