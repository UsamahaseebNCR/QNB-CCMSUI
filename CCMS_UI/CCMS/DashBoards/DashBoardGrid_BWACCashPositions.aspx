<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DashBoardGrid_BWACCashPositions.aspx.cs"
    Inherits="CCMSUI.CCMS.DashBoards.DashBoardGrid_BWACCashPositions" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>
        <asp:Literal ID="Literal_AddHeading" runat="server"></asp:Literal>
    </title>
    <meta http-equiv="Refresh" content="300" id="metaRefresh" runat="server" />
    <script src="../../content/boxover.js" type="text/javascript" language="javascript">
    </script>
    <link href="../../content/iSuite.css" rel="stylesheet" />
    <link href="../../content/cal/popcalendar.css" type="text/css" rel="stylesheet" />
    <script src="../../content/boxover.js" type="text/javascript" language="javascript">
    </script>
    <script language="javascript" src="../../content/cal/popcalendar.js" type="text/javascript"></script>
    <script src="../../content/jquery-1.4.4.min.js" type="text/javascript"></script>
    <script src="../../content/ScrollableGridPlugin.js" type="text/javascript"></script>
    <%-- <script type="text/javascript">
        $(document).ready(function () {
            //Invoke Scrollable function.
            $('#<%=GridView_CashPositions.ClientID %>').Scrollable({
                ScrollHeight: 800
            });
        });
    </script>--%>
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
                    <asp:Panel ID="Panel1" runat="server" ScrollBars="Auto" Width="100%">
                        <asp:GridView ID="GridView_CashPositions" runat="server" AutoGenerateColumns="False"
                            Font-Size="Smaller" Width="99%" AllowSorting="true" OnSorting="GridView_CashPositions_Sorting"
                            >
                            <HeaderStyle ForeColor="Black" Width="13" />
                            <EmptyDataTemplate>
                                <center>
                                    No Rows Found</center>
                            </EmptyDataTemplate>
                            <Columns>
                                <asp:BoundField DataField="title" HeaderText="ATM" SortExpression="title" ItemStyle-HorizontalAlign="Center">
                                </asp:BoundField>
                                <asp:BoundField DataField="region_name" HeaderText="Region" SortExpression="region_name" ItemStyle-HorizontalAlign="Center">
                                </asp:BoundField>
                                 <asp:BoundField DataField="location" HeaderText="Location" SortExpression="location" ItemStyle-HorizontalAlign="Left">
                                </asp:BoundField>
                               
                                <asp:BoundField DataField="print_date" HeaderText="Print Date" ItemStyle-HorizontalAlign="Center"
                                    DataFormatString="{0:dd/MM/yy}" HtmlEncode="false" SortExpression="print_date">
                                </asp:BoundField>
                               
                                <asp:BoundField DataField="rep_datetime" HeaderText="Replenishment DateTime" ItemStyle-HorizontalAlign="Center"
                                    DataFormatString="{0:dd/MM/yy HH:mm:ss}" HtmlEncode="false" SortExpression="rep_datetime">
                                </asp:BoundField>
                                <asp:BoundField DataField="loaded" HeaderText="Loaded" ItemStyle-BackColor="gray"
                                    ItemStyle-HorizontalAlign="Right" ItemStyle-ForeColor="white" SortExpression="loaded">
                                </asp:BoundField>
                                <asp:BoundField DataField="remaining" HeaderText="Remaining" SortExpression="Remaining"
                                    ItemStyle-HorizontalAlign="Right" ItemStyle-BackColor="Chocolate" ItemStyle-ForeColor="white">
                                </asp:BoundField>
                                <asp:BoundField DataField="dispensed" HeaderText="Dispensed" ItemStyle-BackColor="Info"
                                    SortExpression="dispensed" ItemStyle-HorizontalAlign="Right"></asp:BoundField>
                            </Columns>
                        </asp:GridView>
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
