<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DashBoardGrid_Deposits.aspx.cs"
    Inherits="CCMSUI.CCMS.DashBoardGrid_Deposits" Culture="fr-FR" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>NCR BNA & CPM DashBoard</title>
    <meta http-equiv="Refresh" content="300" id="metaRefresh" runat="server" />
    <link href="../../content/iSuite.css" rel="stylesheet" />
    <link href="../../content/cal/popcalendar.css" type="text/css" rel="stylesheet" />
    <script language="javascript" src="../../content/cal/popcalendar.js" type="text/javascript"></script>
    <script src="../../content/jquery-1.4.4.min.js" type="text/javascript"></script>
    <script src="../../content/ScrollableGridPlugin.js" type="text/javascript"></script>
    <%-- <script type="text/javascript">
        $(document).ready(function () {
            //Invoke Scrollable function.
            $('#<%=GridView_Deposit.ClientID %>').Scrollable({
                ScrollHeight: 800
            });
        });
    </script>--%>
    <script type="text/javascript">
        function ShowDialog(title, fileStatus) {
            Dialog_ShowListOfFiles.Show();
            DialogCallBack_ShowListOfFiles.callback(title, fileStatus);
        }

        var mins; var secs;

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
<body style="margin: 0; background-image: url(../../images/dashboard_bg.jpg);">
    <form id="form1" runat="server">
    <div>
        <%--<table cellpadding="0" cellspacing="0" border="0" width="100%" style="margin: 0px">
                <tr>
                    <td style="width: 10px; height: 23px;" valign="top">
                        <img src="../../images/box_lefttop.jpg" width="10px" height="23px" /></td>
                    <td width="100%" style="background-image: url(../../images/box_tophorizontal.jpg);
                        color: White;">
                        
                 <%--     <div style="text-align: right">
                        
                        
                       Alert DashBoard
                            <img src="../../images/close.jpg" onclick="window.close();" style="cursor: hand"
                                height="18px" width="43px" alt="Click to close this window"/>
                       
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
        <div style="text-align: right">
            <div id="optionsPanel" style="display: none; text-align: left">
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
                            <td style="width: 111px;">
                                From Date :
                            </td>
                            <td style="width: 231px">
                                <asp:TextBox ID="TextBox_fromdate" runat="server" CssClass="textbox" Width="131px"
                                    EnableViewState="true"></asp:TextBox>&nbsp;
                                <img alt="From Date" style="cursor: hand" onclick="popUpCalendar(this,document.getElementById('<% =TextBox_fromdate.ClientID %>'),'dd/mm/yyyy');"
                                    src="../../images/IconCalendar.gif" border="0" align="middle" />
                                <asp:RegularExpressionValidator ID="ValFromDate" CssClass="Validator" runat="server"
                                    ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                    ControlToValidate="TextBox_fromdate" ErrorMessage="From Date is invalid.">*</asp:RegularExpressionValidator>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator_fromdate" runat="server" ControlToValidate="TextBox_fromdate"
                                    ErrorMessage="Please enter From Date">*</asp:RequiredFieldValidator>
                                <asp:CompareValidator ID="CompareValidator_fromdate" runat="server" ControlToValidate="TextBox_fromdate"
                                    ControlToCompare="TextBox_toDate" Operator="LessThanEqual" Type="Date" ErrorMessage="From Date cannot be greater than To Date">*</asp:CompareValidator>
                            </td>
                            <td style="width: 115px;">
                                To Date :
                            </td>
                            <td>
                                <asp:TextBox ID="TextBox_toDate" runat="server" CssClass="textbox" Width="131px"
                                    EnableViewState="true"></asp:TextBox>&nbsp;
                                <img alt="To Date" style="cursor: hand" onclick="popUpCalendar(this,document.getElementById('<% =TextBox_toDate.ClientID %>'),'dd/mm/yyyy');"
                                    src="../../images/IconCalendar.gif" border="0" align="middle" />
                                <asp:RegularExpressionValidator ID="ValToDate" CssClass="Validator" runat="server"
                                    ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                    ControlToValidate="TextBox_toDate" ErrorMessage="To Date is invalid.">*</asp:RegularExpressionValidator>
                                <asp:CompareValidator ID="CompareValidator_toDate" runat="server" Operator="GreaterThanEqual"
                                    ControlToValidate="TextBox_toDate" Type="Date" ControlToCompare="TextBox_fromdate"
                                    ErrorMessage="To date cannot be less than from date">*</asp:CompareValidator>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator_ToDate" runat="server" ControlToValidate="TextBox_toDate"
                                    ErrorMessage="Please enter To Date">*</asp:RequiredFieldValidator>
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
                    <asp:GridView ID="GridView_Deposit" runat="server" AutoGenerateColumns="False" Font-Size="Smaller"
                        Width="100%" AllowSorting="true" OnSorting="GridView_Deposit_Sorting" OnRowDataBound="GridView_Deposit_RowDataBound"
                        DataKeyNames="atm_id">
                        <HeaderStyle ForeColor="black" />
                        <EmptyDataTemplate>
                            <center>
                                No Rows Found</center>
                        </EmptyDataTemplate>
                        <Columns>
                            <asp:TemplateField ItemStyle-Width="1%" HeaderStyle-CssClass="HeadingCell">
                                <ItemTemplate>
                                    <asp:Image ID="img" runat="server" ImageUrl="../../images/healthy.jpg" Width="20"
                                        Height="20" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="Title" HeaderText="ATM" ItemStyle-HorizontalAlign="Center"
                                HeaderStyle-CssClass="HeadingCell" SortExpression="Title" />
                            <asp:BoundField DataField="Region_name" HeaderText="Region" ItemStyle-HorizontalAlign="Center"
                                HeaderStyle-CssClass="HeadingCell" SortExpression="Region_name" />
                            <asp:BoundField DataField="cassette1_deposit" HeaderText="Cassette1" ItemStyle-HorizontalAlign="Center"
                                HeaderStyle-CssClass="HeadingCell" SortExpression="cassette1_deposit" ItemStyle-BackColor="Chocolate"
                                ItemStyle-ForeColor="white" />
                            <asp:TemplateField>
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" Width="20px" />
                                <HeaderTemplate>
                                    Denomination Detail
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <table>
                                        <%#Eval("cassette1_deposit_value")%>
                                    </table>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <%-- <asp:BoundField DataField="cassette1_deposit_value" HeaderText="Cassette1 Detail" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="HeadingCell"
                                 ItemStyle-BackColor="Chocolate" ItemStyle-ForeColor="white" Visible="false" HtmlEncode="false"
                                 />--%>
                            <asp:BoundField DataField="cassette2_deposit" HeaderText="Cassette2" ItemStyle-HorizontalAlign="Center"
                                HeaderStyle-CssClass="HeadingCell" SortExpression="cassette2_deposit" ItemStyle-BackColor="Info" />
                            <asp:TemplateField>
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" Width="20px" />
                                <HeaderTemplate>
                                    Denomination Detail
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <table>
                                        <%#Eval("cassette2_deposit_value")%>
                                    </table>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="cassette3_deposit" HeaderText="Cassette3" ItemStyle-HorizontalAlign="Center"
                                HeaderStyle-CssClass="HeadingCell" SortExpression="cassette3_deposit" ItemStyle-BackColor="gray"
                                ItemStyle-ForeColor="white" />
                            <asp:TemplateField>
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" Width="20px" />
                                <HeaderTemplate>
                                    Denomination Detail
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <table>
                                        <%#Eval("cassette3_deposit_value")%>
                                    </table>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="cassette4_deposit" HeaderText="Cassette4" ItemStyle-HorizontalAlign="Center"
                                HeaderStyle-CssClass="HeadingCell" SortExpression="cassette4_deposit" ItemStyle-BackColor="Brown"
                                ItemStyle-ForeColor="white" />
                            <asp:TemplateField>
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" Width="20px" />
                                <HeaderTemplate>
                                    Denomination Detail
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <table>
                                        <%#Eval("cassette4_deposit_value")%>
                                    </table>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" Width="20px" />
                                <HeaderTemplate>
                                    Total
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <table>
                                        <%#Eval("total")%>
                                    </table>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="purge_deposit" HeaderText="Reject Bin" ItemStyle-HorizontalAlign="Center"
                                HeaderStyle-CssClass="HeadingCell" SortExpression="purge_deposit" ItemStyle-BackColor="Azure" />
                            <asp:TemplateField>
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" Width="20px" />
                                <HeaderTemplate>
                                    Denomination Detail
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <table>
                                        <%#Eval("purge_deposit_value")%>
                                    </table>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="last_bna_deposit_at" DataFormatString="{0:dd/MM/yy HH:mm}"
                                HeaderStyle-CssClass="HeadingCell" HtmlEncode="false" HeaderText="Last BNA Deposit At"
                                ItemStyle-HorizontalAlign="Center" SortExpression="last_bna_deposit_at" />
                            <asp:BoundField DataField="last_bna_cleared_at" DataFormatString="{0:dd/MM/yy HH:mm}"
                                HeaderStyle-CssClass="HeadingCell" HtmlEncode="false" HeaderText="Last BNA Cleared At"
                                ItemStyle-HorizontalAlign="Center" SortExpression="last_bna_cleared_at" />
                            <asp:TemplateField ItemStyle-Width="2%" HeaderStyle-CssClass="HeadingCell">
                                <ItemStyle BackColor="Black" />
                                <ItemTemplate>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="2%" HeaderStyle-CssClass="HeadingCell">
                                <ItemTemplate>
                                    <asp:Image ID="img1" runat="server" ImageUrl="../../images/healthy.jpg" Width="20"
                                        Height="20" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="bin1" HeaderText="CPM Pkt 1" ItemStyle-HorizontalAlign="Center"
                                HeaderStyle-CssClass="HeadingCell" SortExpression="bin1" ItemStyle-BackColor="Chocolate"
                                ItemStyle-ForeColor="white" />
                            <asp:BoundField DataField="bin2" HeaderText="CPM Pkt 2" ItemStyle-HorizontalAlign="Center"
                                HeaderStyle-CssClass="HeadingCell" SortExpression="bin2" ItemStyle-BackColor="Info" />
                            <asp:BoundField DataField="bin3" HeaderText="CPM Pkt 3" ItemStyle-HorizontalAlign="Center"
                                HeaderStyle-CssClass="HeadingCell" SortExpression="bin3" ItemStyle-BackColor="gray"
                                ItemStyle-ForeColor="white" />
                            <asp:BoundField DataField="bin4" HeaderText="CPM Pkt 4" ItemStyle-HorizontalAlign="Center"
                                HeaderStyle-CssClass="HeadingCell" SortExpression="bin4" ItemStyle-BackColor="Brown"
                                ItemStyle-ForeColor="white" />
                            <asp:BoundField DataField="last_cpm_deposit_at" DataFormatString="{0:dd/MM/yy HH:mm}"
                                HeaderStyle-CssClass="HeadingCell" HtmlEncode="false" HeaderText="Last CPM Deposit At"
                                ItemStyle-HorizontalAlign="Center" SortExpression="last_cpm_deposit_at" />
                            <asp:BoundField DataField="last_cpm_cleared_at" DataFormatString="{0:dd/MM/yy HH:mm}"
                                HeaderStyle-CssClass="HeadingCell" HtmlEncode="false" HeaderText="Last CPM Cleared At"
                                ItemStyle-HorizontalAlign="Center" SortExpression="last_cpm_cleared_at" />
                        </Columns>
                    </asp:GridView>
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
