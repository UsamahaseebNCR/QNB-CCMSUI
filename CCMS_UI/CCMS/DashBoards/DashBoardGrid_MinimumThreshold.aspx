<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DashBoardGrid_MinimumThreshold.aspx.cs"
    Inherits="CCMSUI.CCMS.DashBoards.DashBoardGird_MinimumThreshold" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>NCR Cash Positions Monitoring DashBoard</title>
    <meta http-equiv="Refresh" content="300" id="metaRefresh" runat="server" />
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
                    <asp:GridView ID="GridView_ATM" runat="server" AutoGenerateColumns="False" Font-Size="Smaller"
                        Width="100%" AllowSorting="true" OnSorting="GridView_ATM_Sorting">
                        <HeaderStyle ForeColor="white" />
                        <EmptyDataTemplate>
                            <center>
                                No Rows Found</center>
                        </EmptyDataTemplate>
                        <Columns>
                            <asp:BoundField DataField="title" HeaderText="ATM" ReadOnly="True" ItemStyle-HorizontalAlign="Center"
                                SortExpression="title"></asp:BoundField>
                            <asp:BoundField DataField="min_operating_balance" HeaderText="Minimum Operating Balance"
                                ItemStyle-HorizontalAlign="Center" SortExpression="total"></asp:BoundField>
                            <asp:BoundField DataField="total" HeaderText="Remaining Amount" ItemStyle-HorizontalAlign="Center"
                                SortExpression="total"></asp:BoundField>
                        </Columns>
                    </asp:GridView>
                </td>
                <td>
                    <table>
                        <tr>
                            <td>
                                <asp:GridView ID="GridView_BNA" runat="server" AutoGenerateColumns="False" Font-Size="Smaller"
                                    Width="100%" AllowSorting="true">
                                    <HeaderStyle ForeColor="white" />
                                    <EmptyDataTemplate>
                                        <center>
                                            No Rows Found</center>
                                    </EmptyDataTemplate>
                                    <Columns>
                                        <asp:BoundField DataField="title" HeaderText="ATM" ReadOnly="True" ItemStyle-HorizontalAlign="Center"
                                            SortExpression="title"></asp:BoundField>
                                        <asp:BoundField DataField="total" HeaderText="Total" ItemStyle-HorizontalAlign="Center"
                                            SortExpression="total"></asp:BoundField>
                                    </Columns>
                                </asp:GridView>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:GridView ID="GridView_CDM" runat="server" AutoGenerateColumns="False" Font-Size="Smaller"
                                    Width="100%" AllowSorting="true">
                                    <HeaderStyle ForeColor="white" />
                                    <EmptyDataTemplate>
                                        <center>
                                            No Rows Found</center>
                                    </EmptyDataTemplate>
                                    <Columns>
                                        <asp:BoundField DataField="title" HeaderText="ATM" ReadOnly="True" ItemStyle-HorizontalAlign="Center"
                                            SortExpression="title"></asp:BoundField>
                                        <asp:BoundField DataField="total" HeaderText="Total" ItemStyle-HorizontalAlign="Center"
                                            SortExpression="total"></asp:BoundField>
                                    </Columns>
                                </asp:GridView>
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
