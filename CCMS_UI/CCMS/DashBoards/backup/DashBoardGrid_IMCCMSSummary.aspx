<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DashBoardGrid_IMCCMSSummary.aspx.cs"
    Inherits="CCMSUI.CCMS.DashBoards.DashBoardGrid_IMCCMSSummary" %>

<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>
        <asp:Literal ID="Literal_AddHeading" runat="server"></asp:Literal>
    </title>
    <meta http-equiv="Refresh" content="300" id="metaRefresh" runat="server" />
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
    <%--<link rel="stylesheet" href="//code.jquery.com/ui/1.11.4/themes/smoothness/jquery-ui.css">
  <script src="//code.jquery.com/jquery-1.10.2.js"></script>
  <script src="//code.jquery.com/ui/1.11.4/jquery-ui.js"></script>
  <link rel="stylesheet" href="/resources/demos/style.css">--%>
    <meta http-equiv="Refresh" content="300" id="meta1" runat="server" />
    <script src="../../content/jquery-1.4.4.min.js" type="text/javascript"></script>
    <link href="../../content/css/bootstrap.min.css" rel="stylesheet">
    <link href="../../content/css/bootstrap-responsive.min.css" rel="stylesheet">
    <link href="../../content/css/style.css" rel="stylesheet">
    <link href="../../content/css/style-responsive.css" rel="stylesheet">
    <link href='http://fonts.googleapis.com/css?family=Open+Sans:300italic,400italic,600italic,700italic,800italic,400,300,600,700,800&subset=latin,cyrillic-ext,latin-ext'
        rel='stylesheet' type='text/css'>
    <%--<link href="../../content/css/bootstrap.min.css" rel="stylesheet">
    <link href="../../content/css/bootstrap-responsive.min.css" rel="stylesheet">
    <link href="../../content/css/style.css" rel="stylesheet">
    <link href="../../content/css/style-responsive.css" rel="stylesheet">--%>
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


        function openDialog(value, type) {
            //
            // document.getElementById('trxnID').value = capturedTrxnID;
            // alert('d');
            DialogCallBackCustomerDetail.callback(value, type);
            DialogCustomerDetail.Show();
            // alert('d1');

        }
        function receiveCallBackResult(response) {
            document.getElementById('Detail').innerHTML = response;
        }
  
    </script>
</head>
<body style="margin: 0;">
    <form id="form1" runat="server">
    <asp:Literal ID="Literal_errorMsg" runat="server" EnableViewState="false"></asp:Literal>
    <table width="100%">
        <tr>
            <td>
                <table style="width: 100%" border="0">
                    <tr>
                        <td>
                            <h1>
                                Machines Availability</h1>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:GridView ID="GridView_MachinesAvailability" runat="server" AutoGenerateColumns="False"
                                OnRowDataBound="GridView_MachinesAvailability_RowDataBound" Width="100%">
                                <Columns>
                                    <%-- <asp:BoundField DataField="category" HeaderText="Categories" />
                        <asp:BoundField DataField="agreed_sla" HeaderText="Agreed SLA" />
                        <asp:BoundField DataField="average" HeaderText="Current Month Availability(%)" />
                        <asp:TemplateField HeaderText="For Details">
                            <ItemTemplate>
                                <a onclick='openDialog(<%# Eval("atm_type") %>,0)'>click here </a>
                            </ItemTemplate>
                            <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                        </asp:TemplateField>
                                    --%>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <div class="row-fluid">
                                                <div class="span3 statbox purple" style="width: 100%">
                                                    <i>Category:<%#Eval("category")%></i>
                                                    <div>
                                                        Agreed SLA :
                                                        <%#Eval("agreed_sla")%><br />
                                                        Availability:<%#Eval("average")%>
                                                        <%#Eval("atm_type")%>
                                                    </div>
                                                    <div class="footer">
                                                        <a onclick="window.open('ViewIMCCMSSummary.aspx?value=%>&type=2')">Details</a>
                                                    </div>
                                                </div>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:GridView ID="GridView_CashMonitoring" runat="server" AutoGenerateColumns="False"
                                Width="100%">
                                <Columns>
                                    <asp:TemplateField ItemStyle-Width="100%">
                                        <ItemTemplate>
                                            <div class="row-fluid" style="width: 100%">
                                                <div class="span3 statbox red" style="width: 100%">
                                                    <i>Category:<%#Eval("title")%></i>
                                                    <div>
                                                        Agreed SLA :
                                                        <%#Eval("agreed_sla")%><br />
                                                        Cash Out:<%#Eval("cashout")%>
                                                    </div>
                                                    <div class="footer">
                                                        <a onclick="window.open('ViewIMCCMSSummary.aspx?value=%>&type=3')">Details</a>
                                                    </div>
                                                </div>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <%-- <asp:BoundField DataField="title" HeaderText="Title" />
                        <asp:BoundField DataField="agreed_sla" HeaderText="Agreed SLA" />
                        <asp:BoundField DataField="cashout" HeaderText="Cash Out" />
                        <asp:HyperLinkField DataNavigateUrlFormatString="ViewIMCCMSSummary.aspx?type=3" HeaderText="Details"
                            Text="Click Here For Detail" Target="_blank" DataNavigateUrlFields="cashout" />--%>
                                </Columns>
                            </asp:GridView>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table style="width: 100%">
                                <tr>
                                    <td>
                                        <h1>
                                            No Transaction Monitoring</h1>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Repeater ID="GridView_NoTrxnMonitoring" runat="server">
                                            <HeaderTemplate>
                                                <table style="width: 100%" border="1">
                                                    <tr>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <td>
                                                    <div class="row-fluid">
                                                        <div class="span3 statbox green" style="width: 100%">
                                                            <i>
                                                                <%#Eval("alert_type_name")%></i>
                                                            <div class="number">
                                                                <%#Eval("total")%>
                                                            </div>
                                                            <div class="footer">
                                                                <a onclick="window.open('ViewIMCCMSSummary.aspx?value=<%#Eval("alert_type_id") %>&type=2')">
                                                                    Details</a>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </td>
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                </tr> </table>
                                            </FooterTemplate>
                                        </asp:Repeater>
                                        <%--<asp:GridView ID="GridView_NoTrxnMonitoring" runat="server" AutoGenerateColumns="false"
                    Width="100%">
                    <Columns>
                        <asp:TemplateField ItemStyle-Width="100%">
                            <ItemTemplate>
                                
                            </ItemTemplate>
                        </asp:TemplateField>
                        <%--   <asp:BoundField DataField="alert_type_name" HeaderText="Type" ItemStyle-HorizontalAlign="Right"
                                    SortExpression="alert_type_name" ItemStyle-BackColor="Brown" ItemStyle-ForeColor="white">
                                </asp:BoundField>
                                <asp:BoundField DataField="total" HeaderText="No.Of Machines" ItemStyle-HorizontalAlign="Right"
                                    SortExpression="total" ItemStyle-BackColor="Brown" ItemStyle-ForeColor="white">
                                </asp:BoundField>
                                <asp:HyperLinkField DataNavigateUrlFields="alert_type_id" DataNavigateUrlFormatString="ViewIMCCMSSummary.aspx?value={0}&type=2"
                                    HeaderText="For Details" Text="Click Here For Detail" Target="_blank" />
                    </Columns>
                </asp:GridView>--%>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table style="width: 100%">
                                <tr>
                                    <td>
                                        <h1>
                                            Dead ATMs</h1>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Repeater ID="GridView_DeadATMs" runat="server">
                                            <HeaderTemplate>
                                                <table style="width: 100%" border="1">
                                                    <tr>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <td>
                                                    <div class="row-fluid">
                                                        <div class="span3 statbox green" style="width: 100%">
                                                            <i>
                                                                <%#Eval("title")%></i>
                                                            <div class="number">
                                                                <%#Eval("Count")%>
                                                            </div>
                                                            <div class="footer">
                                                                <a onclick="window.open('ViewIMCCMSSummary.aspx?value=<%#Eval("value") %>&type=1')">
                                                                    Details</a>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </td>
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                </tr> </table>
                                            </FooterTemplate>
                                        </asp:Repeater>
                                        <%--<asp:GridView ID="GridView_DeadATMs" runat="server" AutoGenerateColumns="False" Width="100%">
                    <Columns>
                        <asp:TemplateField ItemStyle-Width="100%">
                            <ItemTemplate>
                                <div class="row-fluid">
                                    <div class="span3 statbox purple">
                                        <i>
                                            <%#Eval("title")%></i>
                                        <div class="number">
                                            <%#Eval("Count")%>
                                        </div>
                                        <div class="title">
                                            Down</div>
                                        <div class="footer">
                                            <a onclick="window.open('ViewIMCCMSSummary.aspx?value=<%#Eval("value") %>&type=1')">
                                                Details</a>
                                        </div>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>--%>
                                        <%-- <asp:GridView ID="GridView_DeadATMs" runat="server" AutoGenerateColumns="False" CssClass="EU_DataTable">
                            <Columns>
                                <asp:BoundField DataField="title" HeaderText="Machine Types" />
                                <asp:BoundField DataField="Count" HeaderText="Number of Machines" />
                                <asp:HyperLinkField DataNavigateUrlFields="value" DataNavigateUrlFormatString="ViewIMCCMSSummary.aspx?value={0}&type=1"
                                    HeaderText="Details" Text="Click Here For Detail" Target="_blank" />
                            </Columns>
                        </asp:GridView>--%>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
            <td style="vertical-align:top"  >
                <table >
                    <tr>
                        <td>
                            <h1>
                                ATM/CDM Down Beyond N Hours</h1>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:GridView ID="GridView_MachineDown" runat="server" AutoGenerateColumns="False">
                                <Columns>
                                    <asp:BoundField DataField="subject" HeaderText="Type" />
                                    <asp:BoundField DataField="total" HeaderText="Number Of Machines" />
                                    <asp:HyperLinkField DataNavigateUrlFields="incident_id" DataNavigateUrlFormatString="ViewIMCCMSSummary.aspx?value={0}&type=5"
                                        HeaderText="Details" Text="Click Here For Detail" Target="_blank" />
                                </Columns>
                            </asp:GridView>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <h1>
                                TMD Monitoring</h1>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:GridView ID="GridView_TMDMonitoring" runat="server" AutoGenerateColumns="False">
                                <Columns>
                                    <asp:BoundField DataField="subject" HeaderText="Type" />
                                    <asp:BoundField DataField="total" HeaderText="Number Of Machines" />
                                    <asp:HyperLinkField DataNavigateUrlFields="incident_id" DataNavigateUrlFormatString="ViewIMCCMSSummary.aspx?value={0}&amp;type=4"
                                        HeaderText="For Details" Text="Click here" Target="_blank" />
                                </Columns>
                            </asp:GridView>
                        </td>
                    </tr>
                </table>
                <table width="100%" cellpadding="0" cellspacing="0">
                    <tr>
                        <td style="vertical-align: top">
                            <h1>
                                Overall Cash Positions Summary</h1>
                        </td>
                        <tr>
                            <asp:GridView ID="GridView_CashPositions" runat="server" AutoGenerateColumns="False"
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
                            </asp:GridView>
            </td>
        </tr>
    </table>
    </td> </tr> </table>
    <ComponentArt:Dialog ContentCssClass="contentCss" FooterCssClass="footerCss" HeaderCssClass="headerCss"
        CssClass="dialogCss" Icon="" Value="" Title="AJAX TabStrip Content" ModalMaskCssClass="modalMaskCssClass"
        AllowDrag="false" Alignment="MiddleCentre" Modal="true" ID="DialogCustomerDetail"
        runat="server" Width="430" ContentClientTemplateId="ClientTemplateCustomerDetail"
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
                            padding: 10px;" colspan="2">
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
    </form>
</body>
</html>
