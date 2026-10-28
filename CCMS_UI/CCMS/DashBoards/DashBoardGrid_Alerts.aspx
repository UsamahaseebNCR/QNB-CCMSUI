<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DashBoardGrid_Alerts.aspx.cs"
    Inherits="CCMSUI.CCMS.DashBoardGrid_Alerts" Culture="fr-FR" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>NCR Alert DashBoard</title>
    <script language="javascript" src="../../content/cal/popcalendar.js" type="text/javascript"></script>
    <meta http-equiv="Refresh" content="300" id="metaRefresh" runat="server" />
    <link href="../../content/iSuite.css" rel="stylesheet" />
    <link href="../../content/cal/popcalendar.css" type="text/css" rel="stylesheet" />
    <script language="javascript" src="../../content/cal/datepicker.js" type="text/javascript"></script>
    <script type="text/javascript">
        var imgDir = "../../content/cal/"
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
                document.getElementById('viewButton').value = 'Hide Search Panel';

            }
            else {
                optionsPanel.style.display = 'none';
                document.getElementById('viewButton').value = 'Show Search Panel';
            }
        }
    </script>
</head>
<body style="margin: 0; background-image: url(../../images/dashboard_bg.jpg)">
    <form id="form1" runat="server">
    <div>
        <asp:Literal ID="Literal_errorMsg" runat="server" EnableViewState="false"></asp:Literal>
        <div style="text-align: right">
        <input type="button" value="Show Search Panel" id="viewButton" onclick="ToggleOptionView()"  class="button"/>
        </div>
        <div style="text-align: right">
            <div id="optionsPanel" style="text-align: left; display:none">
                <div>
                    <table id="table3" style="width: 100%;" cellspacing="0" cellpadding="0" width="100%"
                        border="0">
                        <tr>
                            <td colspan="4" style="text-align: center">
                                <asp:ValidationSummary ID="ValidationSummary_Search" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td class="PageHeading" colspan="4">
                                DashBoard Search Criteria
                            </td>
                        </tr>
                        <tr>
                            <td style="width:100px">
                                From :
                            </td>
                            <td style="width:30%">
                                <asp:TextBox runat="server" ID="TextBox_FromDate" CssClass="textboxNormal" />
                                <img src="../../images/IconCalendar.gif" alt="Click to open calender" onclick="javascript:show_calendar('form1.TextBox_FromDate');"
                                    style="border: 0; cursor: hand" id="IMG1" />
                                <asp:RegularExpressionValidator ID="ValFromDate" CssClass="Validator" runat="server"
                                    ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                    ControlToValidate="TextBox_FromDate" ErrorMessage="From Date is invalid.">*</asp:RegularExpressionValidator>
                            </td>
                            <td style="width: 99px; height: 17px">
                                To :
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="TextBox_ToDate" CssClass="textboxNormal" />
                                <img src="../../images/IconCalendar.gif" alt="Click to open calender" onclick="javascript:show_calendar('form1.TextBox_ToDate');"
                                    style="border: 0; cursor: hand" id="IMG2" />
                                <asp:RegularExpressionValidator ID="ValToDate" CssClass="Validator" runat="server"
                                    ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                    ControlToValidate="TextBox_ToDate" ErrorMessage="To Date is invalid.">*</asp:RegularExpressionValidator>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Alert Type:
                            </td>
                            <td>
                                <asp:DropDownList ID="DropDownList_AlertType" runat="server" CssClass="textbox">
                                </asp:DropDownList>
                            </td>
                            <td>
                                Bind To:
                            </td>
                            <td>
                                <asp:DropDownList ID="DropDownList_Grid" runat="server" CssClass="textbox">
                                    <asp:ListItem Text="Select"></asp:ListItem>
                                    <asp:ListItem Text="Grid1"></asp:ListItem>
                                    <asp:ListItem Text="Grid2"></asp:ListItem>
                                    <asp:ListItem Text="Grid3"></asp:ListItem>
                                    <asp:ListItem Text="Grid4"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td></td><td></td><td></td>
                            <td >
                                <asp:Button ID="Button_Submit" runat="server" Text="Submit" 
                                    onclick="Button_Submit_Click" CssClass="button" />
                                    
                            </td>
                        </tr>
                    </table>
                </div>
                <hr />
            </div>
        </div>
        <table width="100%" cellpadding="0" cellspacing="0">
            <tr>
                <td style="vertical-align: top">
                    <div style="overflow:auto; width:635px;height:330px">
                    <asp:GridView ID="GridView_Grid1" runat="server" AutoGenerateColumns="False" Font-Size="Smaller"
                        AllowSorting="true" Width="615px" OnSorting="GridView_Grid1_Sorting">
                        <HeaderStyle ForeColor="white" />
                        <EmptyDataTemplate>
                            <center>
                                No Rows Found</center>
                        </EmptyDataTemplate>
                        <Columns>
                            <asp:BoundField DataField="Title" HeaderText="ATM" ItemStyle-HorizontalAlign="Center"
                                SortExpression="Title" />
                            <asp:BoundField DataField="ip" HeaderText="Network Address" ItemStyle-HorizontalAlign="Center"
                                SortExpression="ip" />
                            <asp:BoundField DataField="Region_name" HeaderText="Region" ItemStyle-HorizontalAlign="Center"
                                SortExpression="Region_name" />
                            <asp:BoundField DataField="alert_type_name" HeaderText="Alert Type" ItemStyle-HorizontalAlign="Center"
                                SortExpression="alert_type_name" />
                            <asp:BoundField DataField="alert_msg" HeaderText="Alert Message" ItemStyle-HorizontalAlign="Center"
                                SortExpression="alert_msg" />
                            <asp:BoundField DataField="Generated_at" HeaderText="Generated At" ItemStyle-HorizontalAlign="Center"
                                SortExpression="Generated_at" DataFormatString="{0:dd/MM/yy HH:mm}" HtmlEncode="false" />
                        </Columns>
                    </asp:GridView>
                    </div>
                </td>
                <td>
                <div style="overflow:auto; width:635px;height:330px">
                    <asp:GridView ID="GridView_Grid2" runat="server" AutoGenerateColumns="False" Font-Size="Smaller"
                        AllowSorting="true" Width="615px" OnSorting="GridView_Grid2_Sorting">
                        <HeaderStyle ForeColor="white" />
                        <EmptyDataTemplate>
                            <center>
                                No Rows Found</center>
                        </EmptyDataTemplate>
                        <Columns>
                            <asp:BoundField DataField="Title" HeaderText="ATM" ItemStyle-HorizontalAlign="Center"
                                SortExpression="Title" />
                            <asp:BoundField DataField="ip" HeaderText="Network Address" ItemStyle-HorizontalAlign="Center"
                                SortExpression="ip" />
                            <asp:BoundField DataField="Region_name" HeaderText="Region" ItemStyle-HorizontalAlign="Center"
                                SortExpression="Region_name" />
                            <asp:BoundField DataField="alert_type_name" HeaderText="Alert Type" ItemStyle-HorizontalAlign="Center"
                                SortExpression="alert_type_name" />
                            <asp:BoundField DataField="alert_msg" HeaderText="Alert Message" ItemStyle-HorizontalAlign="Center"
                                SortExpression="alert_msg" />
                            <asp:BoundField DataField="Generated_at" HeaderText="Generated At" ItemStyle-HorizontalAlign="Center"
                                SortExpression="Generated_at" DataFormatString="{0:dd/MM/yy HH:mm}" HtmlEncode="false" />
                        </Columns>
                    </asp:GridView>
                    </div>
                </td>
            </tr>
            <tr>
                <td>
                 <div style="overflow:auto; width:635px;height:330px">
                    <asp:GridView ID="GridView_Grid3" runat="server" AutoGenerateColumns="False" Font-Size="Smaller"
                        AllowSorting="true" Width="615px" OnSorting="GridView_Grid3_Sorting">
                        <HeaderStyle ForeColor="white" />
                        <EmptyDataTemplate>
                            <center>
                                No Rows Found</center>
                        </EmptyDataTemplate>
                        <Columns>
                            <asp:BoundField DataField="Title" HeaderText="ATM" ItemStyle-HorizontalAlign="Center"
                                SortExpression="Title" />
                            <asp:BoundField DataField="ip" HeaderText="Network Address" ItemStyle-HorizontalAlign="Center"
                                SortExpression="ip" />
                            <asp:BoundField DataField="Region_name" HeaderText="Region" ItemStyle-HorizontalAlign="Center"
                                SortExpression="Region_name" />
                            <asp:BoundField DataField="alert_type_name" HeaderText="Alert Type" ItemStyle-HorizontalAlign="Center"
                                SortExpression="alert_type_name" />
                            <asp:BoundField DataField="alert_msg" HeaderText="Alert Message" ItemStyle-HorizontalAlign="Center"
                                SortExpression="alert_msg" />
                            <asp:BoundField DataField="Generated_at" HeaderText="Generated At" ItemStyle-HorizontalAlign="Center"
                                SortExpression="Generated_at" DataFormatString="{0:dd/MM/yy HH:mm}" HtmlEncode="false" />
                        </Columns>
                    </asp:GridView>
                    </div>
                </td>
                <td>
                     <div style="overflow:auto; width:635px;height:330px">
                    <asp:GridView ID="GridView_Grid4" runat="server" AutoGenerateColumns="False" Font-Size="Smaller"
                        AllowSorting="true" Width="615px" OnSorting="GridView_Grid4_Sorting">
                        <HeaderStyle ForeColor="white" />
                        <EmptyDataTemplate>
                            <center>
                                No Rows Found</center>
                        </EmptyDataTemplate>
                        <Columns>
                            <asp:BoundField DataField="Title" HeaderText="ATM" ItemStyle-HorizontalAlign="Center"
                                SortExpression="Title" />
                            <asp:BoundField DataField="ip" HeaderText="Network Address" ItemStyle-HorizontalAlign="Center"
                                SortExpression="ip" />
                            <asp:BoundField DataField="Region_name" HeaderText="Region" ItemStyle-HorizontalAlign="Center"
                                SortExpression="Region_name" />
                            <asp:BoundField DataField="alert_type_name" HeaderText="Alert Type" ItemStyle-HorizontalAlign="Center"
                                SortExpression="alert_type_name" />
                            <asp:BoundField DataField="alert_msg" HeaderText="Alert Message" ItemStyle-HorizontalAlign="Center"
                                SortExpression="alert_msg" />
                            <asp:BoundField DataField="Generated_at" HeaderText="Generated At" ItemStyle-HorizontalAlign="Center"
                                SortExpression="Generated_at" DataFormatString="{0:dd/MM/yy HH:mm}" HtmlEncode="false" />
                        </Columns>
                    </asp:GridView>
                    </div>
                </td>
            </tr>
            <%-- <td>&nbsp;&nbsp;</td>
                    <td style="vertical-align: top">
                        <asp:GridView ID="GridView2" runat="server" 
                            AutoGenerateColumns="False" Font-Size="Smaller" Width="100%">
                            <EmptyDataTemplate>
                                <center>
                                No Rows Found</center>
                            </EmptyDataTemplate>
                            
                            <Columns>
                                
                                <asp:BoundField DataField="Title" HeaderText="Title"  ItemStyle-HorizontalAlign="Center" />
                                <asp:BoundField DataField="ip" HeaderText="Network Address"  ItemStyle-HorizontalAlign="Center" />
                                <asp:BoundField DataField="Region_name" HeaderText="Region"  ItemStyle-HorizontalAlign="Center" />
                                <asp:BoundField DataField="Generated_at" HeaderText="Generated At"  ItemStyle-HorizontalAlign="Center" 
                                 DataFormatString="{0:dd/MM/yy HH:mm}" HtmlEncode ="false"/>
                                
                            </Columns>
                        </asp:GridView>
                    </td>
                </tr>--%>
        </table>
    </div>
    </form>
</body>
</html>
