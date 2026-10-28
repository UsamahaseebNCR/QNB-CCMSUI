<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DashBoardGrid_Tasks.aspx.cs"
    Inherits="CCMSUI.CCMS.DashBoards.DashBoardGrid_Tasks" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>NCR Task Status Monitoring DashBoard</title>
    <meta http-equiv="Refresh" content="300" id="metaRefresh" runat="server" />
    <link href="../../content/iSuite.css" rel="stylesheet" />
    <link href="../../content/cal/popcalendar.css" type="text/css" rel="stylesheet" />
    <script language="javascript" src="../../content/cal/popcalendar.js" type="text/javascript"></script>
    <script src="../../content/jquery-1.4.4.min.js" type="text/javascript"></script>
    <script src="../../content/ScrollableGridPlugin.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            //Invoke Scrollable function.
            $('#<%=GridView_Tasks.ClientID %>').Scrollable({
                ScrollHeight: 800
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
            mins = 1 * m(minutes); // change minutes here
            secs = 0 + s(":01"); // change seconds here (always add an additional second to your total)
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
            } else {
                disp = " ";
            }
            disp += mins + ":";
            if (secs <= 9) {
                disp += "0" + secs;
            } else {
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
                //window.alert("Time is up. Press OK to continue."); // change timeout message as required
                // window.location = "yourpage.htm" // redirects to specified page once timer ends and ok button is pressed
            } else {
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
        <%--<table cellpadding="0" cellspacing="0" border="0" width="100%" style="margin: 0px">
                <tr>
                    <td style="width: 10px; height: 23px;" valign="top">
                        <img src="../../images/box_lefttop.jpg" width="10px" height="23px" /></td>
                    <td width="100%" style="background-image: url(../../images/box_tophorizontal.jpg);
                        color: White;">
                        <div style="text-align: right">
                            Task DashBoard
                            <img src="../../images/close.jpg" onclick="window.close();" style="cursor: hand"
                                height="18px" width="43px" alt="Click to close this window" />
                        </div>
                    </td>
                    <td style="width: 10px; height: 23px;" valign="top">
                        <img src="../../images/box_righttop.jpg" width="10px" height="23px" /></td>
                </tr>
                <tr valign="top">
                    <td style="width: 10px" valign="top" height="2px">
                        <img src="../../images/box_leftvertical_10px.jpg" width="10px" height="2px" /></td>
                    <td style="width: 100%" valign="top" height="2px">
                        <img src="../../images/menulinebottom.jpg" width="100%" height="2px" /></td>
                    <td style="width: 10px" valign="top" height="2px">
                        <img src="../../images/box_rightvertical_10px.jpg" width="10px" height="2px" /></td>
                </tr>
            </table>--%>
        <asp:Literal ID="Literal_errorMsg" runat="server" EnableViewState="false"></asp:Literal>
        <table width="100%" cellpadding="0" cellspacing="0">
            <tr>
                <td style="vertical-align: top">
                    <asp:GridView ID="GridView_Tasks" runat="server" AutoGenerateColumns="False" Font-Size="Smaller"
                        Width="99%" AllowSorting="true" OnSorting="GridView_Task_Sorting">
                        <HeaderStyle ForeColor="white" Width="13" />
                        <EmptyDataTemplate>
                            <center>
                                No Rows Found</center>
                        </EmptyDataTemplate>
                        <Columns>
                            <asp:BoundField DataField="title" HeaderText="ATM" ReadOnly="True" ItemStyle-HorizontalAlign="Center"
                                SortExpression="title" ItemStyle-Width="10%"></asp:BoundField>
                            <asp:BoundField DataField="atm_type" HeaderText="Type" ItemStyle-HorizontalAlign="Center"
                                SortExpression="atm_type" ItemStyle-Width="10%"></asp:BoundField>
                            <asp:BoundField DataField="creation_time" HeaderText="Creation Time" ItemStyle-HorizontalAlign="Center"
                                DataFormatString="{0:dd/MM/yy HH:mm:ss}" HtmlEncode="false" SortExpression="creation_time"
                                ItemStyle-Width="10%"></asp:BoundField>
                            <asp:BoundField DataField="status" HeaderText="Status" ItemStyle-HorizontalAlign="Center"
                                ItemStyle-Width="10%" SortExpression="status"></asp:BoundField>
                            <asp:BoundField DataField="task_type_name" SortExpression="task_type_name" HeaderText="Task Type"
                                ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center"></asp:BoundField>
                            <asp:BoundField DataField="last_invoked" HeaderText="Last Invoked" ItemStyle-HorizontalAlign="Center"
                                ItemStyle-Width="10%" DataFormatString="{0:dd/MM/yy HH:mm:ss}" HtmlEncode="false"
                                SortExpression="last_invoked"></asp:BoundField>
                            <asp:BoundField DataField="end_time" HeaderText="End Time" ItemStyle-HorizontalAlign="Center"
                                ItemStyle-Width="10%" DataFormatString="{0:dd/MM/yy HH:mm:ss}" HtmlEncode="false"
                                SortExpression="end_time"></asp:BoundField>
                            <asp:BoundField DataField="retry_remaining" HeaderText="Retry" ItemStyle-HorizontalAlign="Center"
                                ItemStyle-Width="10%" SortExpression="retry_remaining"></asp:BoundField>
                            <asp:BoundField DataField="failure_reason" HeaderText="Reason" ItemStyle-HorizontalAlign="Center"
                                ItemStyle-Width="10%" SortExpression="failure_reason"></asp:BoundField>
                        </Columns>
                    </asp:GridView>
                </td>
                <%--  <td>
                        &nbsp;&nbsp;</td>
                    <td style="vertical-align: top">
                        <asp:GridView ID="GridView2" runat="server" AutoGenerateColumns="False" Font-Size="Smaller"
                            Width="100%">
                            <EmptyDataTemplate>
                                <center>
                                    No Rows Found</center>
                            </EmptyDataTemplate>
                            <Columns>
                                <asp:BoundField DataField="title" HeaderText="ATM" ReadOnly="True" SortExpression="title"
                                    ItemStyle-HorizontalAlign="Center">
                                    <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                </asp:BoundField>
                                <asp:BoundField DataField="atm_type" HeaderText="Type" SortExpression="atm_type" ItemStyle-HorizontalAlign="Center">
                                    <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                </asp:BoundField>
                                <asp:BoundField DataField="ip" HeaderText="ATM IP" ReadOnly="True" SortExpression="ip"
                                    ItemStyle-HorizontalAlign="Center">
                                    <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                </asp:BoundField>
                                <asp:BoundField DataField="creation_time" HeaderText="Creation Time" SortExpression="creation_time" ItemStyle-HorizontalAlign="Center"
                                    DataFormatString="{0:dd/MM/yy HH:mm:ss}" HtmlEncode="false">
                                    <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                </asp:BoundField>
                                <asp:BoundField DataField="status" SortExpression="status" HeaderText="Status" ItemStyle-HorizontalAlign="Center">
                                    <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                </asp:BoundField>
                                <asp:BoundField DataField="task_type_name" SortExpression="task_type_name" HeaderText="Task Type" ItemStyle-HorizontalAlign="Center">
                                    <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                </asp:BoundField>
                            </Columns>
                        </asp:GridView>
                    </td>--%>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
