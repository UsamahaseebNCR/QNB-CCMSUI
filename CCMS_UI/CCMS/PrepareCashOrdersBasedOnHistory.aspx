<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PrepareCashOrdersBasedOnHistory.aspx.cs"
    Inherits="CCMSUI.CCMS.PrepareCashOrdersBasedOnHistory" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>
        <asp:Literal ID="Literal_AddHeading" runat="server"></asp:Literal>
    </title>
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
    <link href="../content/iSuite.css" rel="stylesheet" />
    <link href="../content/Scripts/ui-lightness/jquery.ui.all.css" rel="stylesheet" type="text/css" />
    <link href="../content/Scripts/ui-lightness/jquery-ui-1.8.17.custom.css" rel="stylesheet"
        type="text/css" />
    <link href="../content/Scripts/ui-lightness/ui-time-picker.css" rel="stylesheet"
        type="text/css" />
    <script type="text/javascript" src="../content/js/jquery.js"></script>
    <script type="text/javascript" src="../content/Scripts/jquery.ui.core.js"></script>
    <script src="../content/js/jquery.js" type="text/javascript"></script>
    <script src="../content/Scripts/jquery-ui-1.8.16.min.js" type="text/javascript"></script>
    <script type="text/javascript" src="../content/Scripts/jquery.ui.widget.js"></script>
    <script type="text/javascript" src="../content/Scripts/jquery.ui.datepicker.js"></script>
    <script type="text/javascript" src="../content/Scripts/jquery-ui-timepicker-addon.js"></script>
    <script type="text/javascript">

        //        $(document).ready(function () {
        //            GetMsgs();
        //        });

        $(function () {

            $('.datepicker').datetimepicker({ dateFormat: 'dd/mm/yy',

                dateFormat: 'dd/mm/yy',
                timeFormat: 'hh:mm:ss',
                defaultDate: -1,
                minDate: '-2m',
                maxDate: '+2d',
                hourGrid: 4,
                minuteGrid: 10,

                showOn: "button",
                buttonImage: "../images/calendar.gif",
                buttonImageOnly: true

            });
        });

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

        function isAnyRowSelected() {
            var inputs = document.getElementsByTagName('input');
            for (var i = 0; i < inputs.length; i++) {
                var typ = inputs[i].type;
                var id = inputs[i].id;
                if (typ == "checkbox" && id.indexOf("CheckBox_Select") > -1) {
                    if (inputs[i].checked)
                        return true;
                }

            }
            alert('Please select row to generate orders');
            return false;
        }

        function close() {
            $(".divNotifications").hide();
        }

        function getOrderDetail(obj) {
           // var gridViewCtlId = '<%=GridView_CashPositions.ClientID%>';
            //var counter = 0;

            var row = obj.parentNode.parentNode;
            var rowIndex = row.rowIndex - 1;
            var arr = obj.value.split(';');
            $.ajax({
                type: "POST",
                url: 'PrepareCashOrders.aspx/GetOrderDetail',
                data: "{atmID:" + arr[0] + ",type:" + arr[1] + "}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
            //        alert(data.d);
                    var parts = data.d.split('|');
                    row.cells[11].getElementsByTagName("input")[0].value = parts[0];
                    row.cells[12].getElementsByTagName("input")[0].value = parts[1];
                    row.cells[13].getElementsByTagName("input")[0].value = parts[2];
                    row.cells[14].getElementsByTagName("input")[0].value = parts[3];

                    
                    // $(".notifications").append(a);

                }

            });
        }

        
        function GetMsgs() {
            $.ajax({
                type: "POST",
                url: '<%=HttpContext.Current.Request.Url.AbsolutePath%>?GetMsgs=1',
                data: "",
                dataType: "",
                success: function (data) {
                    //alert(data);
                    $.each(data, function (idx, obj) {
                        var content = '<p><b>' + obj.title + "</b> " + obj.type + '</p>';
                        $(".notifications").append(content);
                        window.setTimeout(close, 10000);
                        //alert(obj.msg);
                    });
                    // $(".notifications").append(a);

                }

            });
        }


    </script>
</head>
<body style="margin: 0; background-image: url(../../images/dashboard_bg.jpg)" onload="hideProgressBar();">
    <form id="form1" runat="server">
    <div>
        <table style="width: 100%" cellspacing="0" cellpadding="0" border="0" class="mainbox">
            <tr>
                <td class="PageHeading" align="left">
                    Prepare Cash Orders
                </td>
            </tr>
            <tr>
                <td class="ValidationText" align="center">
                    <asp:Literal ID="Literal_error" runat="server" EnableViewState="False"></asp:Literal>
                </td>
            </tr>
            <tr>
                <td class="ConfirmationText" align="center">
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
                                        <td>
                                            Last Replenishment(Days):
                                        </td>
                                        <td>
                                            <asp:TextBox ID="TextBox_LastReplenishment" runat="server" CssClass="textboxNormal"></asp:TextBox>
                                        </td>
                                        <td>
                                            Period:
                                        </td>
                                        <td>
                                            <asp:TextBox ID="TextBox_Period" runat="server" CssClass="textboxNormal"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Classification:
                                        </td>
                                        <td colspan="3">
                                            <asp:CheckBoxList ID="CheckBoxList_Classification" runat="server" RepeatDirection="Horizontal"
                                                BackColor="BlanchedAlmond" RepeatColumns="7">
                                            </asp:CheckBoxList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Minimum Threshold Reached(Types)
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="DropDownList_Types" runat="server" CssClass="textboxNormal">
                                                <asp:ListItem Value="46,47,48,49">Any</asp:ListItem>
                                                <asp:ListItem Value="46">Type 1</asp:ListItem>
                                                <asp:ListItem Value="47">Type 2</asp:ListItem>
                                                <asp:ListItem Value="48">Type 3</asp:ListItem>
                                                <asp:ListItem Value="49">Type 4</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                        </td>
                                        <td>
                                        </td>
                                        <td>
                                        </td>
                                        <td>
                                            <asp:Button ID="Button_search" runat="server" CssClass="button" Text="Search" Width="71px"
                                                OnClick="Button_Search_Click" />
                                            &nbsp;<asp:Button ID="Button_reset" runat="server" CssClass="button" Text="Reset"
                                                Width="71px" />&nbsp;
                                            <asp:Button ID="Button_GenerateOrder" runat="server" CssClass="button" Text="Generate Order"
                                                OnClick="Button_GenerateOrder_Click" OnClientClick="return isAnyRowSelected();" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td>
                                <table>
                                    <tr>
                                        <td>
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
                    <asp:GridView ID="GridView_CashPositions" runat="server" AutoGenerateColumns="False"
                        Font-Size="Smaller" Width="99%" AllowSorting="true" OnSorting="GridView_CashPositions_Sorting"
                        DataKeyNames="atm_id" OnRowDataBound="GridView_CashPositions_RowDataBound">
                        <AlternatingRowStyle BackColor="#eff0ef" />
                        <HeaderStyle CssClass="HeadingCell" />
                        <EmptyDataTemplate>
                            <center>
                                No Rows Found</center>
                        </EmptyDataTemplate>
                        <Columns>
                            <asp:TemplateField HeaderStyle-CssClass="HeadingCell">
                                <ItemTemplate>
                                    <asp:Image ID="img" runat="server" Width="20" Height="20" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Select" ItemStyle-Width="3%" HeaderStyle-CssClass="HeadingCell"
                                ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:CheckBox ID="CheckBox_Select" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="title" HeaderText="ATM" HeaderStyle-CssClass="HeadingCell"
                                SortExpression="title" ItemStyle-HorizontalAlign="Center"></asp:BoundField>
                            <asp:BoundField DataField="last_trxn_at" HeaderText="Last Transaction" ItemStyle-HorizontalAlign="Center"
                                DataFormatString="{0:dd/MM/yy HH:mm:ss}" HtmlEncode="false" SortExpression="last_trxn_at"
                                HeaderStyle-CssClass="HeadingCell"></asp:BoundField>
                            <%-- <asp:BoundField DataField="last_invoked" HeaderText="Last Invoked" ItemStyle-HorizontalAlign="Center"
                                DataFormatString="{0:dd/MM/yy HH:mm:ss}" HtmlEncode="false" SortExpression="last_invoked">
                            </asp:BoundField>--%>
                            <asp:BoundField DataField="last_replenished_at" HeaderText="Last Replenished At"
                                HeaderStyle-CssClass="HeadingCell" ItemStyle-HorizontalAlign="Center" DataFormatString="{0:dd/MM/yy HH:mm:ss}"
                                HtmlEncode="false" SortExpression="last_replenished_at"></asp:BoundField>
                            <asp:BoundField DataField="denomination_type_1" HeaderText="01/11/2017" HeaderStyle-CssClass="HeadingCell"
                                Visible="false" ItemStyle-BackColor="#ccff00" ItemStyle-HorizontalAlign="Right"
                                ItemStyle-ForeColor="black" SortExpression="denomination_type_1"></asp:BoundField>
                            <asp:BoundField DataField="cassette1_notes" HeaderText="02/11/2017" HeaderStyle-CssClass="HeadingCell"
                                SortExpression="cassette1_notes" ItemStyle-HorizontalAlign="Right" ItemStyle-BackColor="#00ff33"
                                ItemStyle-ForeColor="black"></asp:BoundField>
                            <asp:BoundField DataField="denomination_type_2" HeaderText="03/11/2017" HeaderStyle-CssClass="HeadingCell"
                                Visible="false" ItemStyle-BackColor="#ccff66" SortExpression="denomination_type_2"
                                ItemStyle-HorizontalAlign="Right"></asp:BoundField>
                            <asp:BoundField DataField="cassette2_notes" HeaderText="Remaining" HeaderStyle-CssClass="HeadingCell"
                                SortExpression="cassette2_notes" ItemStyle-BackColor="#33ff66" ItemStyle-HorizontalAlign="Right">
                            </asp:BoundField>
                            <asp:BoundField DataField="denomination_type_3" HeaderText="Type 3" HeaderStyle-CssClass="HeadingCell"
                                Visible="false" ItemStyle-BackColor="#99cc33" ItemStyle-ForeColor="white" SortExpression="denomination_type_3"
                                ItemStyle-HorizontalAlign="Right"></asp:BoundField>
                            <asp:BoundField DataField="cassette3_notes" HeaderText="Remaining" ItemStyle-HorizontalAlign="Right"
                                HeaderStyle-CssClass="HeadingCell" SortExpression="cassette3_notes" ItemStyle-BackColor="#009966"
                                ItemStyle-ForeColor="white"></asp:BoundField>
                            <asp:BoundField DataField="denomination_type_4" HeaderText="Type 4" ItemStyle-HorizontalAlign="Right"
                                Visible="false" HeaderStyle-CssClass="HeadingCell" ItemStyle-BackColor="#666633"
                                ItemStyle-ForeColor="black" SortExpression="denomination_type_4"></asp:BoundField>
                            <asp:BoundField DataField="cassette4_notes" HeaderText="Remaining" ItemStyle-HorizontalAlign="Right"
                                HeaderStyle-CssClass="HeadingCell" SortExpression="cassette4_notes" ItemStyle-BackColor="#66ffff"
                                ItemStyle-ForeColor="black"></asp:BoundField>
                            <asp:BoundField DataField="total" HeaderText="Total" ItemStyle-HorizontalAlign="Right"
                                HeaderStyle-CssClass="HeadingCell" SortExpression="total"></asp:BoundField>
                            <asp:TemplateField HeaderText="Select" ItemStyle-Width="3%" HeaderStyle-CssClass="HeadingCell">
                                <ItemTemplate>
                                    <select id="orderType" class="textbox"  onchange="getOrderDetail(this);">
                                        <option value=""></option>
                                        <option value='<%#Eval("atm_id")%>;1'>N</option>
                                        <option value='<%#Eval("atm_id")%>;2'>S</option>
                                    </select>
                                    <%--
                                    <asp:DropDownList ID="DropDownList_OrderType" runat="server CssClass="textbox">
                                        <asp:ListItem Text=""></asp:ListItem>
                                        <asp:ListItem Text="N"></asp:ListItem>
                                        <asp:ListItem Text="S"></asp:ListItem>
                                    </asp:DropDownList>--%>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Type 1" ItemStyle-Width="5%" HeaderStyle-CssClass="HeadingCell">
                                <ItemTemplate>
                                    <asp:TextBox ID="TextBox_Type1" runat="server" CssClass="textbox" Width="50px"></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator_Type1" runat="server"
                                        ValidationExpression="\d*" ControlToValidate="TextBox_Type1" SetFocusOnError="true"
                                        ErrorMessage="Invalid quantity entered for Type1">*</asp:RegularExpressionValidator>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Type 2" ItemStyle-Width="5%" HeaderStyle-CssClass="HeadingCell">
                                <ItemTemplate>
                                    <asp:TextBox ID="TextBox_Type2" runat="server" CssClass="textbox" Width="50px"></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator_Type2" runat="server"
                                        ValidationExpression="\d*" ControlToValidate="TextBox_Type2" SetFocusOnError="true"
                                        ErrorMessage="Invalid quantity entered for Type2">*</asp:RegularExpressionValidator>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Type 3" ItemStyle-Width="5%" HeaderStyle-CssClass="HeadingCell">
                                <ItemTemplate>
                                    <asp:TextBox ID="TextBox_Type3" runat="server" CssClass="textbox" Width="50px"></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator_Type3" runat="server"
                                        ValidationExpression="\d*" ControlToValidate="TextBox_Type3" SetFocusOnError="true"
                                        ErrorMessage="Invalid quantity entered for Type3">*</asp:RegularExpressionValidator>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Type 4" ItemStyle-Width="5%" HeaderStyle-CssClass="HeadingCell">
                                <ItemTemplate>
                                    <asp:TextBox ID="TextBox_Type4" runat="server" CssClass="textbox" Width="50px"></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator_Type4" runat="server"
                                        ValidationExpression="\d*" ControlToValidate="TextBox_Type4" SetFocusOnError="true"
                                        ErrorMessage="Invalid quantity entered for Type4">*</asp:RegularExpressionValidator>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Order Date" ItemStyle-Width="150px" HeaderStyle-CssClass="HeadingCell">
                                <ItemTemplate>
                                    <asp:TextBox ID="TextBox_OrderDate" runat="server" CssClass="datepicker" Width="120px"></asp:TextBox>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Replenishment Date" ItemStyle-Width="150px" HeaderStyle-CssClass="HeadingCell">
                                <ItemTemplate>
                                    <asp:TextBox ID="TextBox_ReplenishmentDate" runat="server" CssClass="datepicker"
                                        Width="120px"></asp:TextBox>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="next_replenished_at" HeaderText="Next Replenishment At"
                                HeaderStyle-CssClass="HeadingCell" ItemStyle-HorizontalAlign="Center" DataFormatString="{0:dd/MM/yy}"
                                HtmlEncode="false" SortExpression="next_replenished_at"></asp:BoundField>
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
                        </Columns>
                    </asp:GridView>
                    &nbsp;
                </td>
            </tr>
        </table>
    </div>
    <%--<div style="position: fixed; bottom: 0; right: 0" class="divNotifications">
        <table>
            <tr>
                <td style="background-color: Green; color: White; font-family: Verdana; font-size: large;
                    padding-right: 40px">
                    <img src="../images/warning.gif" />
                    Notifications
                </td>
            </tr>
            <tr>
                <td style="background-color: #f2f2f2">
                    <div class="notifications" style="overflow: auto; height: 100px">
                    </div>
                </td>
            </tr>
        </table>
    </div>--%>
    <input type="button" runat="server" id="refresh" style="display: none" />
    </form>
</body>
</html>
