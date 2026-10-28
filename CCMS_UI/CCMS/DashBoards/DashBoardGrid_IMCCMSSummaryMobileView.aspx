<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DashBoardGrid_IMCCMSSummaryMobileView.aspx.cs"
    Inherits="CCMSUI.CCMS.DashBoards.DashBoardGrid_IMCCMSSummaryMobileView" %>

<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>
        <asp:Literal ID="Literal_AddHeading" runat="server"></asp:Literal>
    </title>
   <%-- <meta http-equiv="Refresh" content="300" id="metaRefresh" runat="server" />--%>
    <%--<link rel="stylesheet" href="//code.jquery.com/ui/1.11.4/themes/smoothness/jquery-ui.css">
  <script src="//code.jquery.com/jquery-1.10.2.js"></script>
  <script src="//code.jquery.com/ui/1.11.4/jquery-ui.js"></script>
  <link rel="stylesheet" href="/resources/demos/style.css">--%>
    <!-- start: Mobile Specific -->
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <!-- end: Mobile Specific -->
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

        //function cd(minutes) {
        //    mins = 1 * m(minutes); //change minutes here
        //    secs = 0 + s(":01");   //change seconds here (always add an additional second to your total)
        //    redo();
        //}

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

        //function redo() {
        //    secs--;
        //    if (secs == -1) {
        //        secs = 59;
        //        mins--;
        //    }

        //    window.status = "Count Down " + dis(mins, secs); // setup additional displays here.

        //    if ((mins == 0) && (secs == 0)) {
        //    }
        //    else {
        //        cd = setTimeout("redo()", 1000);
        //    }
        //}


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
            //  DialogCallBackCustomerDetail.callback(value, type);

            //  BootstrapDialog.alert('I want banana!');
        }
        function openDialog(value, type) {
            //
            // document.getElementById('trxnID').value = capturedTrxnID;
            // alert('d');
            //   DialogCallBackCustomerDetail.callback(value, type);
            //   DialogCustomerDetail.Show();
            // alert('d1');

        }
        function receiveCallBackResult(response, val) {
            // document.getElementById('Detail').innerHTML = response;
            document.getElementById('modalwindowserver').innerHTML = response;
            $("#registermodal").modal('show');
            if (val == 1) {
                $("#registermodal").css('width', '300');
            }
            else
                $("#registermodal").css('width', '700');

            $('#registermodal').draggable({
                handle: ".modal-header"
            });
        }

        //$(document).ready(function () {
        //    $(".mytext").popover();

        //});
        $(document).ready(function () {

            $.ajax({
                type: "POST",
                url: "DashBoardGrid_IMCCMSSummaryMobileView.aspx/Search",
                dataType: "json",
                data: "{obj:" + JSON.stringify("hghhhhj") + "}",
                contentType: "application/json; charset=utf-8",

                success: function (msg) {
                   
                    var getTotalAvailability = msg.d.getTotalAvailability;
                    var getCashMonitoringSummary = msg.d.getCashMonitoringSummary;
                    var getNoTransactionMonitoring = msg.d.getNoTransactionMonitoring;
                    var getTMDandMachineDownSummary = msg.d.getTMDandMachineDownSummary;
                    var getNoReplenishmentsForMorethan10Days = msg.d.getNoReplenishmentsForMorethan10Days;
                    var getMinOperatingBalance = msg.d.getMinOperatingBalance;
                    var getTMDMonitoringSummary = msg.d.getTMDMonitoringSummary;
                    if (getTotalAvailability.length > 0) {
                        for (var i = 0; i < getTotalAvailability.length; i++) {
                            var template = $($('#temp')[0]).clone();
                            $(template).find('.header').html("<center>" + getTotalAvailability[i].category + "</center>");
                            $(template).find('.circleStat').html(" <input type=\"text\" value=" + getTotalAvailability[i].average + " class=\"whiteCircle\" />");
                            $(template).find('.footer').html(" <i>Agreed SLA:" + getTotalAvailability[i].agreed_sla + "</i><br /><a style=\"color: White\" onclick=" + modalWindow(getTotalAvailability[i].atm_type, 0) + ">Details</a><br /><i style=\"color: White\">MTD:" + getTotalAvailability[i].cumulative_percent + "% </i>");
                            $('#TotAvail').append(template.html());
                            template.show();
                        }
                    }
                    if (getCashMonitoringSummary.length > 0) {
                        for (var i = 0; i < getCashMonitoringSummary.length; i++) {
                            var template = $($('#cashtemp')[0]).clone();
                            $(template).find('.header').html("<center>" + getCashMonitoringSummary[i].title + "</center>");
                            $(template).find('.circleStat').html(" <input type=\"text\" value=" + getCashMonitoringSummary[i].cashout + " class=\"whiteCircle\" />");
                            $(template).find('.footer').html(" <i>Agreed SLA:" + getCashMonitoringSummary[i].agreed_sla + "</i><br /><a style=\"color: White\" onclick=" + modalWindow(getTotalAvailability[i].atm_type, 0) + ">Details</a><br />");
                            $('#CashMon').append(template.html());
                            template.show();
                        }
                    }
                    if (getNoTransactionMonitoring.length > 0) {
                        for (var i = 0; i < getNoTransactionMonitoring.length; i++) {
                            var template = $($('#noTran')[0]).clone();
                            $(template).find('.alertname').html("<p>" + getNoTransactionMonitoring[i].alert_type_name + "</p>");
                            $(template).find('.title').html(" <div class=\"number\">" + getNoTransactionMonitoring[i].total + "</div>");
                            $(template).find('.footer').html("<a onclick=" + modalWindow(getNoTransactionMonitoring[i].alert_type_id, 2) + ">Details</a> ");
                            $('#noTranMain').append(template.html());
                            template.show();
                        }
                    }
                    if (getNoReplenishmentsForMorethan10Days.length > 0) {
                        var template = $($('#machineNotRep')[0]).clone();
                        $(template).find('.title').html(" <div class=\"number\">" + getNoReplenishmentsForMorethan10Days[0].total + "</div>");
                        $(template).find('.footer').html("<a onclick=" + modalWindow(getNoReplenishmentsForMorethan10Days[0].total, 7) + ">Details</a> ");
                        $('#machineNotRepMain').append(template.html());
                    }
                    if (getMinOperatingBalance.length > 0) {
                        var template = $($('#minOprBal')[0]).clone();
                        $(template).find('.title').html(" <div class=\"number\">" + getMinOperatingBalance[0].total + "</div>");
                        $(template).find('.footer').html("<a onclick=" + modalWindow(getMinOperatingBalance[0].total, 6) + ">Details</a> ");
                        $('#minOprBalMain').append(template.html());
                    }
                    if (getTMDandMachineDownSummary.length > 0) {
                        for (var i = 0; i < getTMDandMachineDownSummary.length; i++) {
                            var template = " <tr > <td class=\"blue\">" + getTMDandMachineDownSummary[i].subject + "</td><td class=\"blue\">" + getTMDandMachineDownSummary[i].total + "</td>";
                            template += "<td class=\"blue\"> <a onclick=" + modalWindow(getTMDandMachineDownSummary[i].incident_id, 5) + ">Details </a> </td></tr>";
                            $('#tableMachineDown').append(template);

                        }
                    }
                    if (getTMDMonitoringSummary.length > 0) {
                        for (var i = 0; i < getTMDMonitoringSummary.length; i++) {
                            var template = " <tr > <td class=\"blue\">" + getTMDMonitoringSummary[i].subject + "</td><td class=\"blue\">" + getTMDMonitoringSummary[i].total + "</td>";
                            template += "<td class=\"blue\"> <a onclick=" + modalWindow(getTMDMonitoringSummary[i].incident_id, 5) + ">Details </a> </td></tr>";
                            $('#tableTMDMonitoring').append(template);

                        }
                    }
                    //  circle_progess();  
                    //  widthFunctions();
                    // <script src="../../content/js/custom.js">
                    $.getScript("../../content/js/custom.js");
                    $(".mytext").popover();
                },
                failure: function (msg) {
                    // Replace the div's content with the page method's return.
                    alert(1);
                }

            });

        });

    </script>
    <style>
        .popover {
            width: 100px;
            height: 35px;
        }

        .EU_TableScroll {
        }

        .EU_DataTable {
            border-collapse: collapse;
            width: 100%;
            color: White;
        }

            .EU_DataTable tr th {
                background-color: #e3a21a;
                padding: 3px 5px 10px 5px;
                border: 1px solid #cccccc;
                font-family: Calibri;
                font-size: 12px;
                font-weight: normal;
                text-transform: capitalize;
            }

            .EU_DataTable tr:nth-child(2n+2) {
                background-color: #f3f4f5;
            }

            .EU_DataTable tr:nth-child(2n+1) td {
                background-color: #d6dadf;
                color: #454545;
            }

            .EU_DataTable tr td {
                padding: 5px 10px 5px 10px;
                color: #454545;
                font-family: Arial, Helvetica, sans-serif;
                font-size: 11px;
                border: 1px solid #cccccc;
                vertical-align: middle;
            }

                .EU_DataTable tr td:first-child {
                    text-align: center;
                }
    </style>

</head>
<body style="margin: 0;">
    <form id="form1" runat="server">
        <asp:Literal ID="Literal_errorMsg" runat="server" EnableViewState="false"></asp:Literal>
        <div style="min-height: 336px;">

            <div class="container-fluid-full mytext" data-toggle="popover" data-trigger="hover" data-placement="right" data-content="99.9">
               
                <div class="row-fluid">

                    <div class="span6">
                        <h2 style="padding-left: 112px">Machines Availability</h2>
                        <div id="TotAvail" class=" circleStats">

                            <div id="temp" style="display: none;">

                                <div class="span3">

                                    <div class="circleStatsItemBox greenDark" style="height: 185px;">
                                        <div class="header">
                                        </div>
                                        <span class="percent">percent</span>
                                        <div class="circleStat">
                                        </div>
                                        <div class="footer" style="height: 55px;">
                                        </div>
                                    </div>
                                </div>
                            </div>

                        </div>
                    </div>

                    <div class="span2">
                        <h2>Cash Monitoring</h2>
                        <div id="CashMon" class="circleStats">
                            <div id="cashtemp" style="display: none;">
                                <div class="span2">
                                    <div class="circleStatsItemBox red" style="height: 185px;">
                                        <div class="header">
                                        </div>
                                        <span class="percent">percent</span>
                                        <div class="circleStat">
                                        </div>
                                        <div class="footer" style="height: 55px;">
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                     <div class="span4">
                        <h2>Min Operating Balance</h2>
                     <div id="minOprBalMain">
                        <div id="minOprBal" style="display: none;">
                                        <div class="span5 statbox pink" style="height:220px;">

                                            <i>Machines Below Min Operating Bal</i>

                                            <div class="title">

                                            </div>
                                            <div class="footer">
                                            </div>
                                        </div></div></div>
                                   </div>
                </div>
                <div class="row-fluid">
                     <div class="span8">
                        <h2 style="padding-left: 112px">No Transaction Monitoring</h2>
                    <div id="noTranMain">
                        <div id="noTran" style="display: none;">
                            <div class="span3 statbox purple">

                                <i class="alertname"></i>

                                <div class="title">
                                </div>
                                <div class="footer">
                                </div>
                            </div>
                        </div>
                    </div>
                         </div>
                     <div class="span4">
                        <h2 style="padding-left: 10px">Machines Not Replenished</h2>
                    <div id="machineNotRepMain">
                        <div id="machineNotRep" style="display: none;">
                            <div class="span6 statbox blue" >

                                <i>Machines Not Replenished For More than 10 Days</i>

                                <div class="title">
                                </div>
                                <div class="footer">
                                </div>
                            </div>
                        </div>
                    </div>
                         </div>
                </div>
            </div>

            <div class="box span6">
                <div class="box-header">
                    <h2><i class="halflings-icon align-justify"></i><span class="break"></span><b> ATM/CDM Down Beyond 3 Hours</b></h2>

                </div>
                <div class="box-content" style="display: block;">
                    <table class="table table-striped">
                        <thead>
                            <tr>
                                <th>Incident Type</th>
                                <th>No. Of Machines</th>

                            </tr>
                        </thead>
                        <tbody id="tableMachineDown">
                        </tbody>
                    </table>

                </div>
            </div>
              <div class="box span6">
                <div class="box-header">
                    <h2><i class="halflings-icon align-justify"></i><span class="break"></span><b> TMD Monitoring </b></h2>

                </div>
                <div class="box-content" style="display: block;">
                    <table class="table table-striped">
                        <thead>
                            <tr>
                                <th>Incident Type</th>
                                <th>No. Of Machines</th>

                            </tr>
                        </thead>
                        <tbody id="tableTMDMonitoring">
                        </tbody>
                    </table>

                </div>
            </div>
         
           
            <div class="modal fade" id="registermodal">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">
                                ×</button>
                            <h4 class="modal-title">Details</h4>
                        </div>
                        <div class="modal-body" id="modalwindowserver" runat="server">
                            <p>
                            </p>
                        </div>
                        <div class="modal-footer">
                        </div>
                    </div>
                    <!-- /.modal-content -->
                </div>
                <!-- /.modal-dialog -->
            </div>
            <!-- /.modal -->
           
            <table>
                <tr>
                </tr>
                <tr>
                    <td></td>
                </tr>
            </table>
            <%--
    added by shariq
            --%>

            <%--<ComponentArt:Dialog ContentCssClass="contentCss" FooterCssClass="footerCss" HeaderCssClass="headerCss"
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
    </ComponentArt:Dialog>--%>
            <%--<div id="dialog" runat="server" title="Basic dialog">
  </div>--%>
            <%-- <ComponentArt:CallBack ID="DialogCallBackCustomerDetail" runat="server" Width="0"
        Height="0" OnCallback="DialogCallBackDetail_Callback">
    </ComponentArt:CallBack>--%>
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
        <%--<script src="../../content/js/custom.js"></script>--%>
    </form>
</body>
</html>
