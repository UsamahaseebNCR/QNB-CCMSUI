<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExecDashboard.aspx.cs" Inherits="CCMSUI.CCMS.ExecDashboard" %>
<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="description" content="" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <meta name="robots" content="all,follow" />
    <title>Executive Dashboard</title>
    <!-- Bootstrap CSS-->
    <link rel="stylesheet" href="../content/vendor/bootstrap/css/bootstrap.min.css" />
    
    <link rel="stylesheet" href="../content/vendor/sb-admin-2.min.css" />
    <!-- Font Awesome CSS-->
    <link rel="stylesheet" href="../content/vendor/font-awesome/css/font-awesome.min.css" />
    <!-- Fontastic Custom icon font-->
    <link rel="stylesheet" href="../content/styleSheets/fontastic.css" />
    <!-- Google fonts - Roboto -->
    <link rel="stylesheet" href="https://fonts.googleapis.com/css?family=Roboto:300,400,500,700" />
    <!-- jQuery Circle-->
    <link rel="stylesheet" href="../content/styleSheets/percircle.css" />
    <!-- Custom Scrollbar-->
    <link rel="stylesheet" href="../content/vendor/malihu-custom-scrollbar-plugin/jquery.mCustomScrollbar.css" />
    <!-- theme stylesheet-->
    <link rel="stylesheet" href="../content/styleSheets/style.default.css" />
    <!-- Custom stylesheet - for your changes-->
    <link rel="stylesheet" href="../content/styleSheets/custom.css" />
    <!-- Favicon-->
    <link rel="shortcut icon" href="img/favicon.ico" />

    <link rel="stylesheet" href="https://cdn.datatables.net/1.10.19/css/dataTables.bootstrap4.min.css" />
    <!-- Tweaks for older IEs--><!--[if lt IE 9]>
        <script src="https://oss.maxcdn.com/html5shiv/3.7.3/html5shiv.min.js"></script>
        <script src="https://oss.maxcdn.com/respond/1.4.2/respond.min.js"></script><![endif]-->

    <script src="../content/Scripts/jquery.min.js"></script>
    <script src="../content/Scripts/datatable/jquery.dataTables.min.js" type="text/javascript"></script>
    <script type="text/javascript" src="https://cdn.datatables.net/1.10.19/js/dataTables.bootstrap4.min.js"></script>
    <script src="../content/vendor/popper.js/umd/popper.min.js"> </script>
    <script src="../content/vendor/bootstrap/js/bootstrap.min.js"></script>
    <script src="../content/Scripts/percircle.js"></script>
    <script src="../content/vendor/jquery.cookie/jquery.cookie.js"> </script>
    <script src="../content/vendor/chart.js/Chart.min.js"></script>
    <script src="../content/vendor/jquery-validation/jquery.validate.min.js"></script>
    <script src="../content/vendor/malihu-custom-scrollbar-plugin/jquery.mCustomScrollbar.concat.min.js"></script>
    <script type="text/javascript" src="https://maps.googleapis.com/maps/api/js?key=AIzaSyB59hfEUsise1ksMJ0yDOC_8zIV6CznAuE"></script>
    
    <script>
        var map = null;
        function getChartOptions(title, suggestedMinVal) {
            if (suggestedMinVal == undefined || suggestedMinVal == null)
                suggestedMinVal = 0;
            return {
                legend: {
                    display: false,
                },
                title: {
                    display: true,
                    text: title
                },
                scales: {
                    yAxes: [{
                        display: true,
                        ticks: {
                            suggestedMin: suggestedMinVal,    // minimum will be 0, unless there is a lower value.
                        }
                    }]
                }
            };
        }

        function getChartData(label, data) {
            return {
                labels: label,
                datasets: [
                    {
                        fill: true,
                        backgroundColor: "rgba(0,143,155, 0.7)",
                        data: data,
                    },
                ]
            }
        }

        function showModal(html) {
            document.getElementById('modalwindowserver').innerHTML = html;
            $("#registermodal").modal('show');
        }

        function getCashOutAtms() {
            $.ajax({
                url: "ExecDashboard.aspx/GetCashOutAtms",
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                success: function (data) {
                    var bounds = new google.maps.LatLngBounds();
                    $.each(data["d"], function (i, item) {
                        var marker = new google.maps.Marker({
                            position: { lat: item.Lat, lng: item.Long },
                            map: map,
                            title: item.Title
                        });
                        bounds.extend(marker.position);

                        var infowindow = new google.maps.InfoWindow();
                        var content = "<h4>" + item.Title + "</h4>" +
                                        "<p>" +
                                            "Location: " + item.Location + "<br/>" +
                                            "Ticket Generated At: " + item.AlertGenerateDateString + "<br/>" +
                                        "</p>";
                        google.maps.event.addListener(marker, 'click', (function (marker, content, infowindow) {
                            return function () {
                                infowindow.setContent(content);
                                infowindow.open(map, marker);
                            };
                        })(marker, content, infowindow));
                    });

                    //now fit the map to the newly inclusive bounds
                    map.fitBounds(bounds);

                    //(optional) restore the zoom level after the map is done scaling
                    var listener = google.maps.event.addListener(map, "idle", function () {
                        map.setZoom(3);
                        google.maps.event.removeListener(listener);
                    });
                }
            });
        }

        $(document).ready(function () {
            var lat = 25.2048493;
            var lng = 55.270782800000006;
            var myLatlng = new google.maps.LatLng(lat, lng);
            var mapOptions = { zoom: 14, center: myLatlng };
            map = new google.maps.Map(document.getElementById('map'), mapOptions);

            getCashOutAtms();

            $("#btnWithdrawalDetail").on('click', function () {
                $.ajax({
                    type: "GET",
                    url: "ExecDashboard.aspx/GetWithdrawalTransactionDetail",
                    contentType: 'application/json; charset=utf-8',
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        alert("Request: " + XMLHttpRequest.toString() + "\n\nStatus: " + textStatus + "\n\nError: " + errorThrown);
                    },
                    success: function (result) {
                        showModal(result["d"]);
                        $("#modalTable").DataTable({
                            searching: false,
                            "lengthMenu": [[5, 10, 20], [5, 10, 20]]
                        });
                    }
                });
            });

            $("#btnCashDepositDetail").on('click', function () {
                $.ajax({
                    type: "GET",
                    url: "ExecDashboard.aspx/GetCashDepositTransactionDetail",
                    contentType: 'application/json; charset=utf-8',
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        alert("Request: " + XMLHttpRequest.toString() + "\n\nStatus: " + textStatus + "\n\nError: " + errorThrown);
                    },
                    success: function (result) {
                        showModal(result["d"][2]["Value"]);
                        $("#modalTable").DataTable({
                            searching: false,
                            "lengthMenu": [[5, 10, 20], [5, 10, 20]]
                        });
                        var LINECHART = $('#modalChart');
                        new Chart(LINECHART, {
                            type: 'bar',
                            options: getChartOptions('Last 6 Months Transactions'),
                            data: getChartData(result["d"][1]["Value"], result["d"][0]["Value"])
                        });
                    }
                });
            });

            $("#btnChequeDepositDetail").on('click', function () {
                $.ajax({
                    type: "GET",
                    url: "ExecDashboard.aspx/GetCPMTransactionDetail",
                    contentType: 'application/json; charset=utf-8',
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        alert("Request: " + XMLHttpRequest.toString() + "\n\nStatus: " + textStatus + "\n\nError: " + errorThrown);
                    },
                    success: function (result) {
                        showModal(result["d"][2]["Value"]);
                        $("#modalTable").DataTable({
                            searching: false,
                            "lengthMenu": [[5, 10, 20], [5, 10, 20]]
                        });
                        var LINECHART = $('#modalChart');
                        new Chart(LINECHART, {
                            type: 'bar',
                            options: getChartOptions('Last 6 Months Transactions'),
                            data: getChartData(result["d"][1]["Value"], result["d"][0]["Value"])
                        });
                    }
                });
            });

            $("#btnBalanceInquiryDetail").on('click', function () {
                $.ajax({
                    type: "GET",
                    url: "ExecDashboard.aspx/GetBalanceInquiryDetail",
                    contentType: 'application/json; charset=utf-8',
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        alert("Request: " + XMLHttpRequest.toString() + "\n\nStatus: " + textStatus + "\n\nError: " + errorThrown);
                    },
                    success: function (result) {
                        showModal(result["d"][2]["Value"]);

                        var LINECHART = $('#modalChart');
                        new Chart(LINECHART, {
                            type: 'bar',
                            options: getChartOptions('Last 6 Months Transactions'),
                            data: getChartData(result["d"][1]["Value"], result["d"][0]["Value"])
                        });
                    }
                });
            });

            $("#btnCardlessTransactionDetail").on('click', function () {
                $.ajax({
                    type: "GET",
                    url: "ExecDashboard.aspx/GetCardlessTransactionDetail",
                    contentType: 'application/json; charset=utf-8',
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        alert("Request: " + XMLHttpRequest.toString() + "\n\nStatus: " + textStatus + "\n\nError: " + errorThrown);
                    },
                    success: function (result) {
                        showModal(result["d"][2]["Value"]);

                        var LINECHART = $('#modalChart');
                        new Chart(LINECHART, {
                            type: 'bar',
                            options: getChartOptions('Last 6 Months Transactions'),
                            data: getChartData(result["d"][1]["Value"], result["d"][0]["Value"])
                        });
                    }
                });
            });

            $("#btnOtherTransactionDetail").on('click', function () {
                $.ajax({
                    type: "GET",
                    url: "ExecDashboard.aspx/GetOtherTransactionDetail",
                    contentType: 'application/json; charset=utf-8',
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        alert("Request: " + XMLHttpRequest.toString() + "\n\nStatus: " + textStatus + "\n\nError: " + errorThrown);
                    },
                    success: function (result) {
                        showModal(result["d"][2]["Value"]);

                        var LINECHART = $('#modalChart');
                        new Chart(LINECHART, {
                            type: 'bar',
                            options: getChartOptions('Last 6 Months Transactions'),
                            data: getChartData(result["d"][1]["Value"], result["d"][0]["Value"])
                        });
                    }
                });
            });

            $("#btnAtmAvailabilityDetail").on('click', function () {
                getAtmCdmAvailability(0);
            });

            $("#btnCdmAvailabilityDetail").on('click', function () {
                getAtmCdmAvailability(1);
            });

            $("#btnAtmDownDetail").on('click', function () {
                $.ajax({
                    type: "GET",
                    url: "ExecDashboard.aspx/GetAtmDownDetail",
                    contentType: 'application/json; charset=utf-8',
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        alert("Request: " + XMLHttpRequest.toString() + "\n\nStatus: " + textStatus + "\n\nError: " + errorThrown);
                    },
                    success: function (result) {
                        showModal(result["d"]);
                        $("#modalTable").DataTable({
                            searching: false
                        });
                    }
                });
            });
        });

        function getAtmCdmAvailability(type) {
            $.ajax({
                type: "GET",
                url: "ExecDashboard.aspx/GetAtmCdmAvailabilityDetail?type=" + type,
                contentType: 'application/json; charset=utf-8',
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    alert("Request: " + XMLHttpRequest.toString() + "\n\nStatus: " + textStatus + "\n\nError: " + errorThrown);
                },
                success: function (result) {
                    showModal(result["d"][2]["Value"]);
                    
                    var LINECHART = $('#modalChart');
                    new Chart(LINECHART, {
                        type: 'bar',
                        options: getChartOptions('Last 6 Months Availability'),
                        data: getChartData(result["d"][1]["Value"], result["d"][0]["Value"])
                    });
                }
            });
        }

        top3AtmData = [];
        top3AtmLabel = [];
        top3CdmData = [];
        top3CdmLabel = [];
        top3CpmData = [];
        top3CpmLabel = [];
        
        function loadTop3CpmChart() {
            var LINECHART = $('#top3Cpms');
            var myLineChart = new Chart(LINECHART, {
                type: 'bar',
                options: getChartOptions('Top 3 CPMs'),
                data: getChartData(top3CpmLabel, top3CpmData)
            });
        }

        function loadTop3CdmChart() {
            var LINECHART = $('#top3Cdms');
            var myLineChart = new Chart(LINECHART, {
                type: 'bar',
                options: getChartOptions('Top 3 CDMs'),
                data: getChartData(top3CdmLabel, top3CdmData)
            });
        }

        function loadTop3AtmChart() {
            var LINECHART = $('#top3Atms');
            var myLineChart = new Chart(LINECHART, {
                type: 'bar',
                options: getChartOptions('Top 3 ATMs'),
                data: getChartData(top3AtmLabel, top3AtmData)
            });
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager" runat="server"></asp:ScriptManager>
        <asp:Literal ID="Literal_errorMsg" runat="server" EnableViewState="false"></asp:Literal>
    
        <div class="page">
            <!-- navbar-->
            <header class="header">
                <nav class="navbar">
                    <div class="container-fluid">
                        <div class="navbar-holder d-flex align-items-center justify-content-between">
                            <div class="navbar-header">
                                <span style="color: white; font-size: 30px; text-transform: uppercase;">Executive Dashboard</span>
                            </div>
                        </div>
                    </div>
                </nav>
            </header>

            <div class="container-fluid">
                <div class="row" style="margin-top:5px">
                <div class="col-lg-2 col-md-6">
                    <div class="panel panel-green">
                        <div class="panel-heading">
                            <div class="row">
                                <div class="col-md-2">
                                    <i class="fa fa-credit-card fa-3x"></i>
                                </div>
                                <div class="col-md-10 text-right">
                                    <div class="huge" id="WithdrawalTransactionToday" runat="server">0</div>
                                    <div>Cash Withdrawals</div>
                                </div>
                            </div>
                        </div>
                        <a href="#" id="btnWithdrawalDetail">
                            <div class="panel-footer">
                                <span class="pull-left">View Details</span>
                                <span class="pull-right"><i class="fa fa-arrow-circle-right"></i></span>
                                <div class="clearfix"></div>
                            </div>
                        </a>
                    </div>
                </div>
                <div class="col-lg-2 col-md-6">
                    <div class="panel panel-green">
                        <div class="panel-heading">
                            <div class="row">
                                <div class="col-md-2">
                                    <i class="fa fa-bank fa-3x"></i>
                                </div>
                                <div class="col-md-10 text-right">
                                    <div class="huge" id="CashDepositTransactionToday" runat="server">0</div>
                                    <div>Cash Deposit</div>
                                </div>
                            </div>
                        </div>
                        <a href="#" id="btnCashDepositDetail">
                            <div class="panel-footer">
                                <span class="pull-left">View Details</span>
                                <span class="pull-right"><i class="fa fa-arrow-circle-right"></i></span>
                                <div class="clearfix"></div>
                            </div>
                        </a>
                    </div>
                </div>
                <div class="col-lg-2 col-md-6">
                    <div class="panel panel-green">
                        <div class="panel-heading">
                            <div class="row">
                                <div class="col-md-2">
                                    <i class="fa fa-cc fa-3x"></i>
                                </div>
                                <div class="col-md-10 text-right">
                                    <div class="huge" id="CpmTransactionToday" runat="server">0</div>
                                    <div>Cheque Deposit</div>
                                </div>
                            </div>
                        </div>
                        <a href="#" id="btnChequeDepositDetail">
                            <div class="panel-footer">
                                <span class="pull-left">View Details</span>
                                <span class="pull-right"><i class="fa fa-arrow-circle-right"></i></span>
                                <div class="clearfix"></div>
                            </div>
                        </a>
                    </div>
                </div>
                <div class="col-lg-2 col-md-6">
                    <div class="panel panel-green">
                        <div class="panel-heading">
                            <div class="row">
                                <div class="col-md-2">
                                    <i class="fa fa-question-circle-o fa-3x"></i>
                                </div>
                                <div class="col-md-10 text-right">
                                    <div class="huge" id="BalanceInquiryToday" runat="server">0</div>
                                    <div>Balance Inquiry</div>
                                </div>
                            </div>
                        </div>
                        <a href="#" id="btnBalanceInquiryDetail">
                            <div class="panel-footer">
                                <span class="pull-left">View Details</span>
                                <span class="pull-right"><i class="fa fa-arrow-circle-right"></i></span>
                                <div class="clearfix"></div>
                            </div>
                        </a>
                    </div>
                </div>
                <div class="col-lg-2 col-md-6">
                    <div class="panel panel-green">
                        <div class="panel-heading">
                            <div class="row">
                                <div class="col-md-2">
                                    <i class="fa fa-thumbs-up fa-3x"></i>
                                </div>
                                <div class="col-md-10 text-right">
                                    <div class="huge" id="CardlessTransactionsCount" runat="server">0</div>
                                    <div>Cardless Transactions</div>
                                </div>
                            </div>
                        </div>
                        <a href="#" id="btnCardlessTransactionDetail">
                            <div class="panel-footer">
                                <span class="pull-left">View Details</span>
                                <span class="pull-right"><i class="fa fa-arrow-circle-right"></i></span>
                                <div class="clearfix"></div>
                            </div>
                        </a>
                    </div>
                </div>
                <div class="col-lg-2 col-md-6">
                    <div class="panel panel-green">
                        <div class="panel-heading">
                            <div class="row">
                                <div class="col-md-2">
                                    <i class="fa fa-circle-o-notch fa-3x"></i>
                                </div>
                                <div class="col-md-10 text-right">
                                    <div class="huge" id="OtherTransactionsCount" runat="server">13</div>
                                    <div>Others</div>
                                </div>
                            </div>
                        </div>
                        <a href="#" id="btnOtherTransactionDetail">
                            <div class="panel-footer">
                                <span class="pull-left">View Details</span>
                                <span class="pull-right"><i class="fa fa-arrow-circle-right"></i></span>
                                <div class="clearfix"></div>
                            </div>
                        </a>
                    </div>
                </div>   
                </div>
            </div>

            <section class="statistics" style="margin-top:5px">
                <div class="container-fluid row">
                    <div class="col-lg-8">
                        <div class="row d-flex">
                            <div class="col-lg-4">
                                <!-- ATM Availablity-->
                                <div class="panel">
                                    <div class="panel-heading">
                                        <div class="row">
                                            <div class="col-md-6">
                                                <div id="progress-circle-atm"></div>
                                            </div>
                                            <div class="col-md-6 text-right data-usage">
                                                <div>ATM Availability</div>
                                                <br />
                                                <div>
                                                    <small>Agreed SLA</small>
                                                    <span id="AtmAvailabilityAgreedSla" runat="server">0%</span>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <a href="#" id="btnAtmAvailabilityDetail">
                                        <div class="panel-footer">
                                            <span class="pull-left">View Details</span>
                                            <span class="pull-right"><i class="fa fa-arrow-circle-right"></i></span>
                                            <div class="clearfix"></div>
                                        </div>
                                    </a>
                                </div>
                            </div>
                            <div class="col-lg-4">
                                <!-- CDM Availablity-->
                                <div class="panel">
                                    <div class="panel-heading">
                                        <div class="row">
                                            <div class="col-md-6">
                                                <div id="progress-circle-cdm"></div>
                                            </div>
                                            <div class="col-md-6 text-right data-usage">
                                                <div>CDM Availability</div>
                                                <br />
                                                <div>
                                                    <small>Agreed SLA</small>
                                                    <span id="CdmAvailabilityAgreedSla" runat="server">0%</span>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <a href="#" id="btnCdmAvailabilityDetail" class="panel-green">
                                        <div class="panel-footer">
                                            <span class="pull-left">View Details</span>
                                            <span class="pull-right"><i class="fa fa-arrow-circle-right"></i></span>
                                            <div class="clearfix"></div>
                                        </div>
                                    </a>
                                </div>
                            </div>
                            <div class="col-lg-4">
                                <!-- ATM Down Count-->
                                <div class="panel">
                                    <div class="panel-heading">
                                        <div class="row">
                                            <div class="col-md-6">
                                                <div id="progress-circle-atm-down"></div>
                                            </div>
                                            <div class="col-md-6 text-right data-usage">
                                                <div>ATM Down</div>
                                                <br />
                                                <div>
                                                    <small>Total ATMs</small>
                                                    <span id="TotalAtmCount" runat="server">0</span>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <a href="#" id="btnAtmDownDetail">
                                        <div class="panel-footer">
                                            <span class="pull-left">View Details</span>
                                            <span class="pull-right"><i class="fa fa-arrow-circle-right"></i></span>
                                            <div class="clearfix"></div>
                                        </div>
                                    </a>
                                </div>
                            </div>
                        </div>
                        <div class="row d-flex">
                            <div class="col-lg-12">
                                <div class="card income">
                                    <h3 class="display h4">Cash Out</h3>
                                    <div id="map" style="width: 100%; height: 250px"></div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-lg-4">
                        <div>
                            <div class="card user-activity">
                                <canvas id="top3Atms" height="114"></canvas>
                            </div>
                        </div>
                        <br clear="all" />
                        <div>
                            <div class="card user-activity">
                                <canvas id="top3Cdms" height="114"></canvas>
                            </div>
                        </div>
                        <br clear="all" />    
                        <div>
                            <div class="card user-activity">
                                <canvas id="top3Cpms" height="114"></canvas>
                            </div>
                        </div>
                    </div>
                 </div>
             </section>

            <footer class="main-footer">
                <div class="container-fluid">
                    <div class="row">
                        <div class="col-sm-6">
                            <p>NCR Corporation &copy; 2017-2019</p>
                        </div>
                    </div>
                </div>
            </footer>
        </div>
        
        <div class="modal fade" id="registermodal" >
            <div class="modal-dialog" >
                <div class="modal-content" >
                    <div class="modal-header">
                        <h4 class="modal-title">Details</h4>
                        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button>
                    </div>
                    <div class="modal-body" id="modalwindowserver" runat="server" ></div>
                    <div class="modal-footer" ></div>
                </div>
                <!-- /.modal-content -->
            </div>
            <!-- /.modal-dialog -->
        </div>
        <!-- /.modal -->
    </form>
</body>
</html>