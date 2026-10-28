<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DashBoardGrid_FTP.aspx.cs"
    Inherits="CCMSUI.CCMS.DashBoards.DashBoardGrid_FTP" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>NCR Order Download And Daily Feed Status DashBoard</title>
    <meta http-equiv="Refresh" content="300" id="metaRefresh" runat="server" />
    <link href="../../content/iSuite.css" rel="stylesheet" />
    <link href="../../content/cal/popcalendar.css" type="text/css" rel="stylesheet" />
    <script language="javascript" src="../../content/cal/popcalendar.js" type="text/javascript"></script>
    <script src="../../content/jquery-1.4.4.min.js" type="text/javascript"></script>
    <script src="../../content/ScrollableGridPlugin.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            //Invoke Scrollable function.
            $('#<%=GridView_Ftp.ClientID %>').Scrollable({
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
                            FTP DashBoard
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
                    <asp:GridView ID="GridView_Ftp" runat="server" AutoGenerateColumns="False" Font-Size="Smaller"
                        AllowSorting="true" OnSorting="GridView_Ftp_Sorting" Width="100%">
                        <HeaderStyle ForeColor="white" />
                        <EmptyDataTemplate>
                            <center>
                                No Rows Found</center>
                        </EmptyDataTemplate>
                        <Columns>
                            <asp:BoundField DataField="region_name" HeaderText="Organization" ItemStyle-HorizontalAlign="Center"
                                SortExpression="region_name" ItemStyle-Width="5%"></asp:BoundField>
                            <%--<asp:BoundField DataField="server_name" HeaderText="Server" ItemStyle-HorizontalAlign="Center" />--%>
                            <asp:BoundField DataField="creation_time" HeaderText="Creation Time" ItemStyle-HorizontalAlign="Center"
                                SortExpression="creation_time" ItemStyle-Width="7%" DataFormatString="{0:dd/MM/yy HH:mm:ss}"
                                HtmlEncode="false" />
                            <asp:BoundField DataField="task_type_name" HeaderText="Task type" ItemStyle-HorizontalAlign="Center"
                                SortExpression="task_type_name" ItemStyle-Width="7%" />
                            <asp:BoundField DataField="status" HeaderText="Status" ItemStyle-HorizontalAlign="Center"
                                SortExpression="status" ItemStyle-Width="7%" />
                            <asp:BoundField DataField="end_time" HeaderText="End Time" ItemStyle-HorizontalAlign="Center"
                                SortExpression="end_time" ItemStyle-Width="4%" DataFormatString="{0:dd/MM/yy HH:mm:ss}"
                                HtmlEncode="false"></asp:BoundField>
                            <asp:BoundField DataField="last_invoked_at" HeaderText="Last Invoked" ItemStyle-HorizontalAlign="Center"
                                SortExpression="last_invoked_at" ItemStyle-Width="4%" DataFormatString="{0:dd/MM/yy HH:mm:ss}"
                                HtmlEncode="false"></asp:BoundField>
                            <asp:BoundField DataField="retry_count" HeaderText="Retry" ItemStyle-HorizontalAlign="Center"
                                SortExpression="retry_count" ItemStyle-Width="4%"></asp:BoundField>
                            <asp:BoundField DataField="reason" HeaderText="Failure Reason" ItemStyle-HorizontalAlign="Center"
                                SortExpression="reason" ItemStyle-Width="20%" />
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
                                <asp:BoundField DataField="region_name" HeaderText="Organization" SortExpression="region_name">
                                    <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                </asp:BoundField>
                                <asp:BoundField DataField="server_name" HeaderText="Server" ItemStyle-HorizontalAlign="Center" />
                                <asp:BoundField DataField="creation_time" HeaderText="Creation Time" ItemStyle-HorizontalAlign="Center"
                                    DataFormatString="{0:dd/MM/yy HH:mm}" HtmlEncode="false" />
                                <asp:BoundField DataField="task_type_name" HeaderText="Task type" ItemStyle-HorizontalAlign="Center" />
                                <asp:BoundField DataField="status" HeaderText="Status" ItemStyle-HorizontalAlign="Center" />
                                <asp:BoundField DataField="reason" HeaderText="Failure Reason" ItemStyle-HorizontalAlign="Center" />
                            </Columns>
                        </asp:GridView>
                    </td>--%>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
