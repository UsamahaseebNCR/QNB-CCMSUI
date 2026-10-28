<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewIMCCMSSummary.aspx.cs"
    Inherits="CCMSUI.CCMS.DashBoards.ViewIMCCMSSummary" %>

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
    <style type="text/css">
        .style2
        {
            width: 350px;
        }
    </style>
    <style>
          .EU_TableScroll
        {
          
           
        }
        .EU_DataTable
        {
            border-collapse: collapse;
            width:100%;
        }
            .EU_DataTable tr th
            {
                background-color: #3c454f;
             <%--   color: #ffffff;--%>
                padding: 3px 5px 10px 5px;
                border: 1px solid #cccccc;
                font-family: Arial, Helvetica, sans-serif;
                font-size: 12px;
                font-weight: normal;
                text-transform:capitalize;
            }
            .EU_DataTable tr:nth-child(2n+2)
            {
                background-color: #f3f4f5;
            }

            .EU_DataTable tr:nth-child(2n+1) td
            {
                background-color: #d6dadf;
                color: #454545;
            }
            .EU_DataTable tr td
            {
                padding: 5px 10px 5px 10px;
                color: #454545;
                font-family: Arial, Helvetica, sans-serif;
                font-size: 11px;
                border: 1px solid #cccccc;
                vertical-align: middle;
            }
                .EU_DataTable tr td:first-child
                {
                    text-align: center;
                }
    </style>
</head>
<body style="margin: 0; background-image: url(../../images/dashboard_bg.jpg)">
    <form id="form1" runat="server">
    <div>
        <asp:Literal ID="Literal_errorMsg" runat="server" EnableViewState="false"></asp:Literal>
        <asp:Panel ID="Panel1" runat="server">
       
        <table>
            <tr>
                <td>
                    <h1>
                        Details</h1>
                </td>
                <td>
                    <asp:GridView ID="GridView_MachinesAvailability" runat="server" 
                        AutoGenerateColumns="False"  CssClass="EU_DataTable">
                        <Columns>
                            <asp:BoundField DataField="title" HeaderText="Machine ID" />
                            <asp:BoundField DataField="address" HeaderText="Address" />
                            <asp:BoundField DataField="comms_failure" HeaderText="Comm" />
                            <asp:BoundField DataField="host_down" HeaderText="Host Down" />
                            <asp:BoundField DataField="flm_p1" HeaderText="FLM" />
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
        </table>
        </asp:Panel>
        <asp:Panel ID="Panel2" runat="server">
        <table>
            <tr>
                <td class="style2">
                    <h1 >Dead ATMs Details</h1></td>
                <td>
                    <asp:GridView ID="GridView_DeadATMsDetail" runat="server" 
                        AutoGenerateColumns="False"  CssClass="EU_DataTable">
                        <Columns>
                            <asp:BoundField DataField="title" HeaderText="Machine ID" />
                            <asp:BoundField DataField="location" HeaderText="Location" />
                            <asp:BoundField DataField="generated_at" HeaderText="Generated Time" />
                            <asp:BoundField DataField="hoursdiff" HeaderText="Duration" />
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
        </table>
        </asp:Panel>
           <asp:Panel ID="Panel3" runat="server">
        <table>
            <tr>
                <td class="style2">
                    <h1 >No Transaction Monitoring Details</h1></td>
                <td>
                    <asp:GridView ID="GridView_NoTransactionMonitoringDetails" runat="server" 
                        AutoGenerateColumns="False"  CssClass="EU_DataTable">
                        <Columns>
                            <asp:BoundField DataField="atm_id" HeaderText="Machine ID" />
                            <asp:BoundField DataField="location" HeaderText="Location" />
                            <asp:BoundField DataField="lasttrxn" HeaderText="Last Transaction" />
                            <asp:BoundField DataField="inactivitycount" HeaderText="Occurrence" />
                            <asp:BoundField DataField="inactivityduration" 
                                HeaderText="No Transaction Monitoring Duration" />
                            <asp:BoundField DataField="operationalHours" HeaderText="Operational Hours" />
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
        </table>
        </asp:Panel>
         <asp:Panel ID="Panel4" runat="server">
       
        <table>
            <tr>
                <td>
                    <h1>
                    Cash Monitoring Details</h1>
                </td>
                <td>
                    <asp:GridView ID="GridView_CashMonitoringDetail" runat="server" 
                        AutoGenerateColumns="False"  CssClass="EU_DataTable">
                        <Columns>
                            <asp:BoundField DataField="title" HeaderText="Machine ID" />
                            <asp:BoundField DataField="address" HeaderText="Address" />
                            <asp:BoundField DataField="comms_failure" HeaderText="Comm" />
                            <asp:BoundField DataField="host_down" HeaderText="Host Down" />
                            <asp:BoundField DataField="flm_p1" HeaderText="FLM" />
                            <asp:BoundField DataField="cash_out" HeaderText="Cash Out" />
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
        </table>
        </asp:Panel>
         <asp:Panel ID="Panel5" runat="server">
       
        <table>
            <tr>
                <td>
                    <h1>
                    TMD Monitoring Details</h1>
                </td>
                <td>
                    <asp:GridView ID="GridView_TMDMonitoringDetails" runat="server" 
                        AutoGenerateColumns="False"  CssClass="EU_DataTable">
                        <Columns>
                            <asp:BoundField DataField="title" HeaderText="Machine ID" />
                            <asp:BoundField DataField="location" HeaderText="Address" />
                            <asp:BoundField DataField="generated_at" HeaderText="Start Time" />
                            <asp:BoundField DataField="duration" HeaderText="Duration" />
                            
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
        </table>
        </asp:Panel>
         <asp:Panel ID="Panel6" runat="server">
       
        <table>
            <tr>
                <td>
                    <h1>
                    Machines Down Beyond N Hours Details</h1>
                </td>
                <td>
                    <asp:GridView ID="GridView_MachineDownNHoursDetails" runat="server" 
                        AutoGenerateColumns="False"  CssClass="EU_DataTable">
                        <Columns>
                         <asp:BoundField DataField="title" HeaderText="Machine ID" />
                            <asp:BoundField DataField="location" HeaderText="Address" />
                            <asp:BoundField DataField="generated_at" HeaderText="Start Time" />
                            <asp:BoundField DataField="duration" HeaderText="Duration" />
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
        </table>
        </asp:Panel>

    </div>
    </form>
</body>
</html>
