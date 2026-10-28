<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DashBoardGrid_IMCCMSSummary.aspx.cs"
    Inherits="CCMSUI.CCMS.DashBoards.DashBoardGrid_IMCCMSSummary" %>

<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>
        <asp:Literal ID="Literal_AddHeading" runat="server"></asp:Literal>
    </title>
    <meta http-equiv="Refresh" content="300" id="metaRefresh" runat="server" />
    <%--<link rel="stylesheet" href="//code.jquery.com/ui/1.11.4/themes/smoothness/jquery-ui.css">
  <script src="//code.jquery.com/jquery-1.10.2.js"></script>
  <script src="//code.jquery.com/ui/1.11.4/jquery-ui.js"></script>
  <link rel="stylesheet" href="/resources/demos/style.css">--%>
    <meta http-equiv="Refresh" content="300" id="meta1" runat="server" />
    <link href="../../content/Scripts/bootstrap-dialog.css" rel="stylesheet" type="text/css" />
    <script src="../../content/Scripts/bootstrap-dialog.js" type="text/javascript"></script>
    <%--<script src="../../content/jquery-1.4.4.min.js" type="text/javascript"></script>--%>
    <script src="../../content/Scripts/jquery.min.js" type="text/javascript"></script>
    <%--<script src="//code.jquery.com/jquery-1.10.2.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/bootstrap3-dialog/1.34.7/js/bootstrap-dialog.min.js" type="text/javascript"></script>
    --%>
    <%--  <link href="https://cdnjs.cloudflare.com/ajax/libs/bootstrap3-dialog/1.34.7/css/bootstrap-dialog.min.css" rel="Stylesheet" />--%>
    <link href="../../content/css/bootstrap.min.css" rel="stylesheet">
    <link href="../../content/css/bootstrap-responsive.min.css" rel="stylesheet">
    <link href="../../content/css/style.css" rel="stylesheet">
    <script src="../../content/Scripts/bootstrap.min.js" type="text/javascript"></script>
    <link href="../../content/css/style-responsive.css" rel="stylesheet">
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
        //        $(document).ready(function () {
        //            $('.lnk_MachineDetail').click(function (e) {
        //                var element = $(this);
        //                alert(element.attr("href"));
        //                var data = "type = " + 0 + "&value=" + 1;
        //                $.ajax({
        //                    url: 'DashBordGrid_IMCCMSSummary.aspx/Test?' + data

        //                })
        //                return false;
        //            });
        //            $(function () {
        //                $("#dialog").dialog();
        //            });
        //        });

        function modalWindow(value, type) {
            DialogCallBackCustomerDetail.callback(value, type);

            //  BootstrapDialog.alert('I want banana!');
        }
        function openDialog(value, type) {
            //
            // document.getElementById('trxnID').value = capturedTrxnID;
            // alert('d');
            DialogCallBackCustomerDetail.callback(value, type);
            DialogCustomerDetail.Show();
            // alert('d1');

        }
        function receiveCallBackResult(response) {
            // document.getElementById('Detail').innerHTML = response;
            document.getElementById('modalwindowserver').innerHTML = response;
            $("#registermodal").modal('show');
            $('#registermodal').draggable({
                handle: ".modal-header"
            });
        }
      
    </script>
    <style>
        .EU_TableScroll
        {
        }
        .EU_DataTable
        {
            border-collapse: collapse;
            width: 100%;
            color:White;
            
            
        }
        .EU_DataTable tr th
        {
            background-color:#e3a21a;
          
            padding: 3px 5px 10px 5px;
            border: 1px solid #cccccc;
            font-family:Calibri;
            
            font-size: 12px;
            font-weight: normal;
            text-transform: capitalize;
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
<body style="margin: 0;">
    <form id="form1" runat="server">
    <asp:Literal ID="Literal_errorMsg" runat="server" EnableViewState="false"></asp:Literal>
    <table style="width: 100%" border="0">
        <tr>
            <td style="width:20%">
                <table style="width: 100%">
                    <tr>
                        <td>
                            <h2>
                                <center>
                                   

                                    Machines Availability</center>
                            </h2>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Repeater ID="GridView_MachinesAvailability" runat="server">
                                <HeaderTemplate>
                                        <table >
                                            <tr>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <td style="padding-left:20px">
                                        <div class="container-fluid-full">
                                            <div class="row-fluid">
                                                <div class="span2" ontablet="span4" ondesktop="span2">
                                                    <div class="circleStatsItemBox greenDark" style="height:185px;">
                                                        <div class="header">
                                                            <center>
                                                                <%#Eval("category") %></center>
                                                        </div>
                                                        <span class="percent">percent</span>
                                                        <div class="circleStat">
                                                            <input type="text" value="<%#Eval("average") %>" class="whiteCircle" />
                                                        </div>
                                                        <div class="footer" style="height:40px;" >
                                                            <i>Agreed SLA:<%# Eval("agreed_sla")%></i><br />
                                                            <a style="color: White" onclick='modalWindow(<%# Eval("atm_type") %>,0)'>Details
                                                            </a>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </td>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </tr> </table>
                                </FooterTemplate>
                            </asp:Repeater>
                            <%--<asp:GridView ID="GridView_MachinesAvailability" runat="server" AutoGenerateColumns="False"
                    CssClass="EU_DataTable" OnRowDataBound="GridView_MachinesAvailability_RowDataBound">
                    <Columns>
                        <asp:TemplateField>
                            
                        </asp:TemplateField>
                      <%--  <asp:BoundField DataField="category" HeaderText="Categories" />
                        <asp:BoundField DataField="agreed_sla" HeaderText="Agreed SLA" />
                        <asp:BoundField DataField="average" HeaderText="Current Month Availability(%)" />--%>
                            <%-- <asp:HyperLinkField DataNavigateUrlFields="atm_type" 
                                DataNavigateUrlFormatString="ViewIMCCMSSummary.aspx?value={0}&type=0" ControlStyle-CssClass="lnk_MachineDetail" HeaderText="Bottom Machines" 
                                Text="Click Here For Detail" Target="_blank" />--%>
                            <%--  <asp:TemplateField HeaderText="For Details">
                            <ItemTemplate>
                                <a onclick='modalWindow(<%# Eval("atm_type") %>,0)'>click here </a>
                            </ItemTemplate>
                            <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                        </asp:TemplateField>-
                </Columns> </asp:GridView>--%>
                        </td>
                    </tr>
                </table>
            </td>
            <td style="width:20%">
                <table style="width: 100%">
                    <tr>
                        <td>
                            <h2>
                                <center>
                                    Cash Monitoring System</center>
                            </h2>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Repeater ID="GridView_CashMonitoring" runat="server">
                                <HeaderTemplate>
                                    <center>
                                        <table>
                                            <tr>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <td>


                                        <div class="container-fluid-full">
                                            <div class="row-fluid">
                                                <div class="span2" ontablet="span4" ondesktop="span2">
                                                    <div class="circleStatsItemBox red" style="height:185px;">
                                                        <div class="header">
                                                            <center>
                                                                <%#Eval("title")%></center>
                                                        </div>
                                                        <%--<span class="percent">percent</span>--%>
                                                        <div class="circleStat">
                                                            <input type="text" value="<%# Eval("cashout") %>" class="whiteCircle" />
                                                        </div>
                                                        <div class="footer" style="height:40px;">
                                                            <i>Agreed SLA:<%# Eval("agreed_sla")%></i><br />
                                                            <a style="color: White" onclick='modalWindow(0,3)'>Details</a></a>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </td>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </tr> </table></center>
                                </FooterTemplate>
                            </asp:Repeater>
                            <%--<asp:GridView ID="GridView_CashMonitoring" runat="server" AutoGenerateColumns="False" HeaderStyle-Height="0">
                                <Columns>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <div class="container-fluid-full">
                                                <div class="row-fluid">
                                                    <div class="span2" ontablet="span4" ondesktop="span2">
                                                        <div class="circleStatsItemBox red">
                                                            <div class="header">
                                                                <center>
                                                                    <%#Eval("title")%></center>
                                                            </div>
                                                            <span class="percent">percent</span>
                                                            <div class="circleStat">
                                                                <input type="text" value="<%#Eval("cashout") %>" class="whiteCircle" />
                                                            </div>
                                                            <div class="footer">
                                                                <i>Agreed SLA:<%# Eval("agreed_sla")%></i><br />
                                                                <a onclick='modalWindow(0,3)'>click here </a></a>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>--%>
                        </td>
                    </tr>
                </table>
            </td>   
            <td style="vertical-align:top">
                <table width="100%" cellpadding="0" cellspacing="0">
        <tr>
            <td style="vertical-align: top; padding-right:20px">
                <h2>
                    <center>
                        Overall Cash Positions Summary</center>
                </h2>
                <%--   <asp:GridView ID="GridView_CashPositions" runat="server" AutoGenerateColumns="False"
                            Font-Size="Smaller" Width="99%" AllowSorting="true" OnSorting="GridView_CashPositions_Sorting">
                            <HeaderStyle ForeColor="Black" Width="13" />
                            <EmptyDataTemplate>
                                <center>
                                    No Rows Found</center>
                            </EmptyDataTemplate>
                            <Columns>
                                <asp:BoundField DataField="region_name" HeaderText="Location" SortExpression="region_name"
                                    ItemStyle-HorizontalAlign="Center"></asp:BoundField>
                                <asp:BoundField DataField="total" HeaderText="Total" ItemStyle-HorizontalAlign="Right"
                                    SortExpression="total"></asp:BoundField>
                                <asp:BoundField DataField="type1_notes_Sum" HeaderText="Type 1 No.Notes" ItemStyle-BackColor="Chocolate"
                                    ItemStyle-HorizontalAlign="Right" ItemStyle-ForeColor="white" SortExpression="type1_notes_Sum">
                                </asp:BoundField>
                                <asp:BoundField DataField="type1_Val_Sum" HeaderText="Type 1 Amount" SortExpression="cassette1_notes"
                                    ItemStyle-HorizontalAlign="Right" ItemStyle-BackColor="Chocolate" ItemStyle-ForeColor="white">
                                </asp:BoundField>
                                <asp:BoundField DataField="type2_notes_Sum" HeaderText="Type 2 No.Notes" ItemStyle-BackColor="Aqua"
                                    SortExpression="type2_notes_Sum" ItemStyle-HorizontalAlign="Right"></asp:BoundField>
                                <asp:BoundField DataField="type2_Val_Sum" HeaderText="Type 2 Amount" SortExpression="type2_Val_Sum"
                                    ItemStyle-BackColor="Aqua" ItemStyle-HorizontalAlign="Right"></asp:BoundField>
                                <asp:BoundField DataField="type3_notes_Sum" HeaderText="Type 3 No.Notes" ItemStyle-BackColor="gray"
                                    ItemStyle-ForeColor="white" SortExpression="type2_notes_Sum" ItemStyle-HorizontalAlign="Right">
                                </asp:BoundField>
                                <asp:BoundField DataField="type3_Val_Sum" HeaderText="Type 3 Amount" ItemStyle-HorizontalAlign="Right"
                                    SortExpression="type3_Val_Sum" ItemStyle-BackColor="gray" ItemStyle-ForeColor="white">
                                </asp:BoundField>
                                <asp:BoundField DataField="type4_notes_Sum" HeaderText="Type 4 No.Notes" ItemStyle-HorizontalAlign="Right"
                                    ItemStyle-BackColor="Brown" ItemStyle-ForeColor="white" SortExpression="type4_notes_Sum">
                                </asp:BoundField>
                                <asp:BoundField DataField="type4_Val_Sum" HeaderText="Type 4 Amount" ItemStyle-HorizontalAlign="Right"
                                    SortExpression="type4_Val_Sum" ItemStyle-BackColor="Brown" ItemStyle-ForeColor="white">
                                </asp:BoundField>
                            </Columns>
                        </asp:GridView>--%>
                <table class="EU_DataTable" >
                    <thead>
                        <tr style="color:White">
                            <th rowspan="2" style="width: 180px;font-weight: bold;">
                                Title
                            </th>
                            <th rowspan="2" style="width: 180px;font-weight: bold;">
                                Overall Amount(Millions)
                            </th>
                            <th colspan="2" style="width: 180px;font-weight: bold;">
                                Type1
                            </th>
                            <th colspan="2" style="width: 180px;font-weight: bold;">
                                Type2
                            </th>
                            <th colspan="2" style="width: 180px;font-weight: bold;">
                                Type3
                            </th>
                            <th colspan="2" style="width: 180px;font-weight: bold;">
                                Type4
                            </th>
                        </tr>
                        <tr>
                            <th style="font-weight: bold;" >
                                No.Notes
                            </th>
                            <th style="font-weight: bold;" >
                                Amount
                            </th>
                            <th style="font-weight: bold;" >
                                No.Notes
                            </th>
                            <th style="font-weight: bold;" >
                                Amount
                            </th>
                            <th style="font-weight: bold;" >
                                No.Notes
                            </th>
                            <th style="font-weight: bold;" >
                                Amount
                            </th>
                            <th style="font-weight: bold;" >
                                No.Notes
                            </th>
                            <th style="font-weight: bold;" >
                                Amount
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                        <div id="tableCashPosition" runat="server">
                        </div>
                    </tbody>
                </table>
            </td>
        </tr>
    </table>
            </td>
        </tr>
    </table>
    
    <table style="width: 100%; " border="0" >
        <tr>
            <td>
                <h2>
                    <center>
                        No Transaction Monitoring</center>
                </h2>
            </td>
            <td>
                <h2>
                    <center>
                        Dead ATMs</center>
                </h2>
            </td>
        </tr>
        <tr>
            <td style="width: 50%">
                <asp:Repeater ID="GridView_NoTrxnMonitoring" runat="server" >
                    <HeaderTemplate>
                        <table style="width: 100%" border="0">
                            <tr>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <td style="width:33%; padding-left:15px">
                            <div class="row-fluid">
                                <div class="span3 statbox pink" style="width: 100%">
                                
                                    <i>
                                        <%#Eval("alert_type_name")%></i>
                                    
                                    <div class="title">
                                    
                                        <div class="number">
                                            <%#Eval("total")%>
                                        </div>
                                    </div>
                                    <div class="footer">
                                        <%-- <a onclick="window.open('ViewIMCCMSSummary.aspx?value=<%#Eval("alert_type_id") %>&type=2')">--%>
                                        <a onclick='modalWindow(<%#Eval("alert_type_id") %>,2)'>Details</a>
                                    </div>
                                </div>
                            </div>
                        </td>
                    </ItemTemplate>
                    <FooterTemplate>
                        </tr></table>
                    </FooterTemplate>
                </asp:Repeater>
            </td>
            <td style="width: 100%">
                <asp:Repeater ID="GridView_DeadATMs" runat="server"  >
                    <HeaderTemplate>
                        <table style="width: 100%">
                            <tr>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <td style="width:33% ;padding-right:15px" >
                            <div class="row-fluid">
                                <div class="span3 statbox purple" style="width: 100%">
                                    <i>
                                        <%#Eval("title")%></i>
                                    
                                    <div class="title">
                                        <div class="number">
                                            <%#Eval("Count")%>
                                        </div>
                                    </div>
                                    <div class="footer">
                                        <%-- <a onclick="window.open('ViewIMCCMSSummary.aspx?value=<%#Eval("value") %>&type=1')">--%>
                                        <a onclick='modalWindow(<%#Eval("value") %>,1)'>Details</a>
                                    </div>
                                </div>
                            </div>
                        </td>
                    </ItemTemplate>
                    <FooterTemplate>
                        </tr></table>
                    </FooterTemplate>
                </asp:Repeater>
            </td>
        </tr>
    </table>
    <div class="modal fade" id="registermodal" style="width:700px">
        <div class="modal-dialog" >
            <div class="modal-content" style="width:700px">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">
                        ×</button>
                    <h4 class="modal-title">
                        Details</h4>
                </div>
                <div class="modal-body" id="modalwindowserver" runat="server" style="width:650px">
                    <p>
                    </p>
                </div>
                <div class="modal-footer" >
                </div>
            </div>
            <!-- /.modal-content -->
        </div>
        <!-- /.modal-dialog -->
    </div>
    <!-- /.modal -->
    <table style="width: 100%" border="0">
        <tr>
            <td>
                <h2>
                    <center>
                        ATM/CDM Down Beyond N Hours</center>
                </h2>
            </td>
            <td>
                <h2>
                    <center>
                        TMD Monitoring</center>
                </h2>
            </td>
        </tr>
        <tr>
            <td style="width:50%; vertical-align:top; padding-left:20px">
                <asp:GridView ID="GridView_MachineDown" runat="server" CellPadding="3" BackColor="White" Width="100%"
                    AutoGenerateColumns="false" BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px">
                    <FooterStyle BackColor="White" ForeColor="#000066" />
                    <HeaderStyle BackColor="#006699" Font-Bold="True" ForeColor="White" />
                    <PagerStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Left" />
                    <RowStyle ForeColor="#000066" />
                    <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" />
                    <SortedAscendingCellStyle BackColor="#F1F1F1" />
                    <SortedAscendingHeaderStyle BackColor="#007DBB" />
                    <SortedDescendingCellStyle BackColor="#CAC9C9" />
                    <SortedDescendingHeaderStyle BackColor="#00547E" />
                    <EmptyDataTemplate>
                        No Rows Found
                    </EmptyDataTemplate>
                    <Columns>
                        <asp:BoundField DataField="subject" HeaderText="Incident Type" />
                        <asp:BoundField DataField="total" HeaderText="No. Of Machines" />
                        <%-- <asp:HyperLinkField DataNavigateUrlFields="incident_id" DataNavigateUrlFormatString="ViewIMCCMSSummary.aspx?value={0}&type=5"
                                    HeaderText="Details" Text="Click Here For Detail" Target="_blank" />--%>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <a onclick='modalWindow(<%# Eval("incident_id") %>,5)'>Details </a>
                            </ItemTemplate>
                            <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </td>
            <td style="vertical-align:top; padding-right:15px">
                <asp:GridView ID="GridView_TMDMonitoring" runat="server" CellPadding="3" AutoGenerateColumns="false"
                    BackColor="#DEBA84" BorderColor="#DEBA84" BorderStyle="None" BorderWidth="1px"
                    Width="100%" CellSpacing="2">
                    <FooterStyle BackColor="#F7DFB5" ForeColor="#8C4510" />
                    <HeaderStyle BackColor="#A55129" Font-Bold="True" ForeColor="White" />
                    <PagerStyle ForeColor="#8C4510" HorizontalAlign="Center" />
                    <RowStyle BackColor="#FFF7E7" ForeColor="#8C4510" />
                    <SelectedRowStyle BackColor="#738A9C" Font-Bold="True" ForeColor="White" />
                    <SortedAscendingCellStyle BackColor="#FFF1D4" />
                    <SortedAscendingHeaderStyle BackColor="#B95C30" />
                    <SortedDescendingCellStyle BackColor="#F1E5CE" />
                    <SortedDescendingHeaderStyle BackColor="#93451F" />
                    <EmptyDataTemplate>
                        No Rows Found
                    </EmptyDataTemplate>
                    <Columns>
                        <asp:BoundField DataField="subject" HeaderText="Incident Type" />
                        <asp:BoundField DataField="total" HeaderText="No. Of Machines" />
                        <%--<asp:HyperLinkField DataNavigateUrlFields="incident_id" DataNavigateUrlFormatString="ViewIMCCMSSummary.aspx?value={0}&amp;type=4"
                                    HeaderText="For Details" Text="Click here" Target="_blank" />--%>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <a onclick='modalWindow(<%# Eval("incident_id") %>,4)'>Details </a>
                            </ItemTemplate>
                            <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </td>
        </tr>
    </table>
    <table>
        <tr>
        </tr>
        <tr>
            <td>
            </td>
        </tr>
    </table>
    <ComponentArt:Dialog ContentCssClass="contentCss" FooterCssClass="footerCss" HeaderCssClass="headerCss"
        CssClass="dialogCss" Icon="" Value="" Title="AJAX TabStrip Content" ModalMaskCssClass="modalMaskCssClass"
        AllowDrag="false" Alignment="MiddleCentre" Modal="true" ID="DialogCustomerDetail"
        runat="server" Width="830px" ContentClientTemplateId="ClientTemplateCustomerDetail"
        AnimationType="None" ShowTransition="None" TransitionDuration="0" AnimationDuration="0"
        Height="250">
        <ClientTemplates>
            <ComponentArt:ClientTemplate ID="ClientTemplateCustomerDetail">
                <table cellpadding="0" style="border-color: #c9c9c9" cellspacing="0" border="0" width="100%"
                    onmousedown="DialogCustomerDetail.StartDrag(event);">
                    <tr>
                        <td style="background-image: url(../../images/DialogImages/top.gif); height: 45px;">
                            <table width="100%" cellpadding="0" cellspacing="0" border="0">
                                <tr>
                                    <td valign="middle" style="padding-left: 4px; color: white; font-size: 15px; font-family: Verdana;
                                        font-weight: bold;">
                                        Detail
                                    </td>
                                    <td align="right" style="padding-right: 10px;">
                                        <img src="../../images/DialogImages/close.gif" onclick="DialogCustomerDetail.Close();" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td style="border-color: #c9c9c9; background-color: white; font-size: 12px; font-family: Arial;
                            padding: 10px;" colspan="2" >
                            <div id="Detail" style="overflow: auto; height: 270px">
                            </div>
                        </td>
                    </tr>
                </table>
            </ComponentArt:ClientTemplate>
        </ClientTemplates>
    </ComponentArt:Dialog>
    <%--<div id="dialog" runat="server" title="Basic dialog">
  </div>--%>
    <ComponentArt:CallBack ID="DialogCallBackCustomerDetail" runat="server" Width="0"
        Height="0" OnCallback="DialogCallBackDetail_Callback">
    </ComponentArt:CallBack>
    </div>
    <script src="../../content/js/jquery-1.9.1.min.js"></script>
    <script src="../../content/js/jquery-migrate-1.0.0.min.js"></script>
    <script src="../../content/js/jquery-ui-1.10.0.custom.min.js"></script>
    <script src="../../content/js/jquery.ui.touch-punch.js"></script>
    <script src="../../content/js/modernizr.js"></script>
    <script src="../../content/js/bootstrap.min.js"></script>
    <script src="../../content/js/jquery.cookie.js"></script>
    <script src='../../content/js/fullcalendar.min.js'></script>
    <script src='../../content/js/jquery.dataTables.min.js'></script>
    <script src="../../content/js/excanvas.js"></script>
    <script src="../../content/js/jquery.flot.js"></script>
    <script src="../../content/js/jquery.flot.pie.js"></script>
    <script src="../../content/js/jquery.flot.stack.js"></script>
    <script src="../../content/js/jquery.flot.resize.min.js"></script>
    <script src="../../content/js/jquery.chosen.min.js"></script>
    <script src="../../content/js/jquery.uniform.min.js"></script>
    <script src="../../content/js/jquery.cleditor.min.js"></script>
    <script src="../../content/js/jquery.noty.js"></script>
    <script src="../../content/js/jquery.elfinder.min.js"></script>
    <script src="../../content/js/jquery.raty.min.js"></script>
    <script src="../../content/js/jquery.iphone.toggle.js"></script>
    <script src="../../content/js/jquery.uploadify-3.1.min.js"></script>
    <script src="../../content/js/jquery.gritter.min.js"></script>
    <script src="../../content/js/jquery.imagesloaded.js"></script>
    <script src="../../content/js/jquery.masonry.min.js"></script>
    <script src="../../content/js/jquery.knob.modified.js"></script>
    <script src="../../content/js/jquery.sparkline.min.js"></script>
    <script src="../../content/js/counter.js"></script>
    <script src="../../content/js/retina.js"></script>
    <script src="../../content/js/custom.js"></script>
    </form>
</body>
</html>
