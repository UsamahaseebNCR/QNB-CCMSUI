<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DashBoardGrid_Profitability.aspx.cs"
    Inherits="CCMSUI.CCMS.DashBoards.DashBoardGrid_Profitability" %>

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
<body style="margin: 0;">
    <form id="form1" runat="server">
    <div>
        <asp:Literal ID="Literal_errorMsg" runat="server" EnableViewState="false"></asp:Literal>
        <table width="100%" cellpadding="0" cellspacing="0" border="0">
             <tr>
                <td align="center" style="font-size: large; background-color: Gray; color: White"
                    colspan="3">
                    ATM Profitability Dashboard
                </td>
            </tr>
            <tr id="t" runat="server">
                <td colspan="3">
                    <table style="background-color: InfoBackground; width: 100%" class="DemoArea">
                        <tr>
                          
                            <td style="width: 23px">
                                From
                            </td>
                            <td style="width: 23px">
                                <asp:TextBox ID="TextBox_From" runat="server" CssClass="textbox"></asp:TextBox>
                            </td>
                            <td style="width: 23px">
                                <img src="../../images/IconCalendar.gif" alt="Click to open calender" onclick="javascript:show_calendar('form1.TextBox_From');"
                                    style="border: 0; cursor: hand" id="IMG6" />
                            </td>
                            <td style="width: 3px">
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator5" CssClass="Validator"
                                    runat="server" ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                    ControlToValidate="TextBox_From" ErrorMessage="To Date is invalid.">*</asp:RegularExpressionValidator>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="TextBox_From"
                                    ErrorMessage="To date cannot be empty">*</asp:RequiredFieldValidator>
                            </td>
                            <td style="width: 23px">
                                To
                            </td>
                            <td style="width: 23px">
                                <asp:TextBox ID="TextBox_To" runat="server" CssClass="textbox"></asp:TextBox>
                            </td>
                            <td style="width: 23px">
                                <img src="../../images/IconCalendar.gif" alt="Click to open calender" onclick="javascript:show_calendar('form1.TextBox_To');"
                                    style="border: 0; cursor: hand" id="IMG1" />
                            </td>
                            <td style="width: 3px">
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" CssClass="Validator"
                                    runat="server" ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                    ControlToValidate="TextBox_To" ErrorMessage="To Date is invalid.">*</asp:RegularExpressionValidator>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="TextBox_To"
                                    ErrorMessage="To date cannot be empty">*</asp:RequiredFieldValidator>
                            </td>
                            <td align="left">
                                <asp:Button ID="Button1" runat="server" Text="Generate" 
                                    CssClass="button" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
           
            <tr>
                <td>
                    <table>
                        <tr>
                            <td style="vertical-align: top">
                                <asp:Panel ID="Panel1" runat="server" ScrollBars="Auto" Width="100%">
                                    <asp:GridView ID="GridView_Profitability" runat="server" AutoGenerateColumns="False"
                                        Font-Size="Smaller" Width="99%">
                                        <HeaderStyle ForeColor="Black" Width="13" />
                                        <Columns>
                                            <asp:BoundField DataField="title" HeaderText="ATM" ItemStyle-HorizontalAlign="Center" />
                                            <asp:BoundField DataField="location" HeaderText="Location" />
                                            <asp:BoundField DataField="atm_cost" HeaderText="ATM Cost" ItemStyle-HorizontalAlign="Right" />
                                            <asp:BoundField DataField="software_cost" HeaderText="Softwate Cost" ItemStyle-HorizontalAlign="Right" />
                                            <asp:BoundField DataField="network_cost" HeaderText="Network Cost" ItemStyle-HorizontalAlign="Right" />
                                            <asp:BoundField DataField="site_preparation_cost" HeaderText="Site Preparation" ItemStyle-HorizontalAlign="Right" />
                                            <asp:BoundField DataField="security_infrastructure_cost" HeaderText="Security Infrastructure Cost"
                                                ItemStyle-HorizontalAlign="Right" />
                                            <asp:BoundField DataField="trxn_count" HeaderText="No.Of Transactions" ItemStyle-HorizontalAlign="Right" />
                                        </Columns>
                                        <EmptyDataTemplate>
                                            <center>
                                                No Rows Found</center>
                                        </EmptyDataTemplate>
                                        <Columns>
                                        </Columns>
                                    </asp:GridView>
                                </asp:Panel>
                            </td>
                        </tr>
                    </table>
                </td>
                <td valign="top">
                    <table>
                        <tr>
                            <td>
                                <table>
                                    <tr>
                                        <td>
                                            <h2>
                                                Key Performance Indicators</h2>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <table>
                                                <tr>
                                                    <td>
                                                        <b>Profitability
                                                            <asp:Label ID="Label_CashUtil" runat="server"></asp:Label>%</b>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <table>
                                                            <tr>
                                                                <td style="background-color: #9ab353; width: 23px">
                                                                </td>
                                                                <td>
                                                                    80% Excellent
                                                                </td>
                                                                <td style="background-color: #8766a5; width: 23px">
                                                                </td>
                                                                <td>
                                                                    70%-75% Fair
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td style="background-color: #527fb8; width: 23px">
                                                                </td>
                                                                <td>
                                                                    75%-79% Good
                                                                </td>
                                                                <td style="background-color: #BF4b4f; width: 23px">
                                                                </td>
                                                                <td>
                                                                    70% < Need Attention
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <br />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Chart ID="ChartPie" runat="server" Width="425">
                                    <Titles>
                                        <asp:Title Font="Trebuchet MS, 14.25pt, style=Bold" Name="Title1" ForeColor="26, 59, 105">
                                        </asp:Title>
                                    </Titles>
                                    <Legends>
                                        <asp:Legend BackColor="Transparent" Alignment="Center" Docking="Bottom" Font="Trebuchet MS, 8.25pt, style=Bold"
                                            IsTextAutoFit="False" Name="Default" LegendStyle="Column" Title="Legends" Enabled="false">
                                        </asp:Legend>
                                    </Legends>
                                    <BorderSkin SkinStyle="None"></BorderSkin>
                                    <Series>
                                        <asp:Series Name="Default" YValuesPerPoint="2" ChartType="Pie" IsValueShownAsLabel="true"
                                            IsVisibleInLegend="true">
                                        </asp:Series>
                                    </Series>
                                    <ChartAreas>
                                        <asp:ChartArea Name="ChartArea1" BackColor="White">
                                            <Area3DStyle Rotation="0" />
                                            <AxisY LineColor="64, 64, 64, 64">
                                                <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                <MajorGrid LineColor="White" />
                                            </AxisY>
                                            <AxisX LineColor="64, 64, 64, 64">
                                                <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                <MajorGrid LineColor="White" />
                                            </AxisX>
                                        </asp:ChartArea>
                                    </ChartAreas>
                                </asp:Chart>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
