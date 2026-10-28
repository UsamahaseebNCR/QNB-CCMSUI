<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TransactionDashboard.aspx.cs" 
    Inherits="CCMSUI.CCMS.DashBoards.TransactionDashboard" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="description" content="" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <meta name="robots" content="all,follow" />
    <title>Transaction Monitoring Dashboard</title>
    <!-- Bootstrap CSS-->
    <link rel="stylesheet" href="../../content/vendor/bootstrap/css/bootstrap.min.css" />
    
    <link rel="stylesheet" href="../../content/vendor/sb-admin-2.min.css" />
    <!-- Font Awesome CSS-->
    <link rel="stylesheet" href="../../content/vendor/font-awesome/css/font-awesome.min.css" />
    <!-- Fontastic Custom icon font-->
    <link rel="stylesheet" href="../../content/styleSheets/fontastic.css" />
    <!-- Google fonts - Roboto -->
    <link rel="stylesheet" href="https://fonts.googleapis.com/css?family=Roboto:300,400,500,700" />
    <!-- jQuery Circle-->
    <link rel="stylesheet" href="../../content/styleSheets/percircle.css" />
    <!-- Custom Scrollbar-->
    <link rel="stylesheet" href="../../content/vendor/malihu-custom-scrollbar-plugin/jquery.mCustomScrollbar.css" />
    <!-- theme stylesheet-->
    <link rel="stylesheet" href="../../content/styleSheets/style.default.css" />
    <!-- Custom stylesheet - for your changes-->
    <link rel="stylesheet" href="../../content/styleSheets/custom.css" />
    <!-- Favicon-->
    <link rel="shortcut icon" href="img/favicon.ico" />
    
    <link rel="stylesheet" href="https://cdn.datatables.net/1.10.19/css/dataTables.bootstrap4.min.css" />
    <!-- Tweaks for older IEs--><!--[if lt IE 9]>
        <script src="https://oss.maxcdn.com/html5shiv/3.7.3/html5shiv.min.js"></script>
        <script src="https://oss.maxcdn.com/respond/1.4.2/respond.min.js"></script><![endif]-->

    <script src="../../content/Scripts/jquery.min.js"></script>
    <script src="../../content/Scripts/datatable/jquery.dataTables.min.js" type="text/javascript"></script>
    <script type="text/javascript" src="https://cdn.datatables.net/1.10.19/js/dataTables.bootstrap4.min.js"></script>
    <script src="../../content/vendor/popper.js/umd/popper.min.js"> </script>
    <script src="../../content/vendor/bootstrap/js/bootstrap.min.js"></script>
    <script src="../../content/Scripts/percircle.js"></script>
    <script src="../../content/vendor/jquery.cookie/jquery.cookie.js"> </script>
    <script src="../../content/vendor/chart.js/Chart.min.js"></script>
    <script src="../../content/vendor/jquery-validation/jquery.validate.min.js"></script>
    <script src="../../content/vendor/malihu-custom-scrollbar-plugin/jquery.mCustomScrollbar.concat.min.js"></script>

    <script>
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

        function getChartData(label, data, backgroundColor) {
            if (backgroundColor == undefined || backgroundColor == null)
                backgroundColor = "rgba(77, 193, 75, 0.4)";
            return {
                labels: label,
                datasets: [
                    {
                        fill: true,
                        backgroundColor: backgroundColor,
                        data: data,
                    },
                ]
            }
        }

        function getHourlyTransactionSummary() {
            $.ajax({
                type: "GET",
                url: "TransactionDashboard.aspx/GetHourlyTransactionSummary",
                contentType: 'application/json; charset=utf-8',
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    alert("Request: " + XMLHttpRequest.toString() + "\n\nStatus: " + textStatus + "\n\nError: " + errorThrown);
                },
                success: function (result) {
                    var LINECHART = $('#HourlyTransaction');
                    new Chart(LINECHART, {
                        type: 'bar',
                        options: getChartOptions('Hourly Transactions'),
                        data: getChartData(result["d"][1]["Value"], result["d"][0]["Value"])
                    });

                    $('#progress-circle-total-transactions').percircle({
                        progressBarColor: '#33b35a',
                        text: result["d"][2]["Value"] == 0 ? '0' : result["d"][2]["Value"],
                        percent: 100
                    });
                }
            });
        }

        function getNoTransactionSummary() {
            $.ajax({
                type: "GET",
                url: "TransactionDashboard.aspx/GetNoTransactionSummary",
                contentType: 'application/json; charset=utf-8',
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    alert("Request: " + XMLHttpRequest.toString() + "\n\nStatus: " + textStatus + "\n\nError: " + errorThrown);
                },
                success: function (result) {
                    var LINECHART = $('#noTransactionAtm');
                    new Chart(LINECHART, {
                        type: 'bar',
                        options: getChartOptions('No Transaction Monitoring'),
                        data: getChartData(result["d"][1]["Value"], result["d"][0]["Value"], "rgba(192, 37, 36, 0.5)")
                    });
                }
            });
        }

        function getTopTransactingAtms() {
            $.ajax({
                type: "GET",
                url: "TransactionDashboard.aspx/GetTopTransactingAtms",
                contentType: 'application/json; charset=utf-8',
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    alert("Request: " + XMLHttpRequest.toString() + "\n\nStatus: " + textStatus + "\n\nError: " + errorThrown);
                },
                success: function (result) {
                    var LINECHART = $('#topAtms');
                    new Chart(LINECHART, {
                        type: 'bar',
                        options: getChartOptions('Top Transacting ATMs'),
                        data: getChartData(result["d"][1]["Value"], result["d"][0]["Value"])
                    });
                }
            });
        }

        function getOnUsVsOffUsTransactionSummary() {
            $.ajax({
                type: "GET",
                url: "TransactionDashboard.aspx/OnUsVsOffUsTransactionSummary",
                contentType: 'application/json; charset=utf-8',
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    alert("Request: " + XMLHttpRequest.toString() + "\n\nStatus: " + textStatus + "\n\nError: " + errorThrown);
                },
                success: function (result) {
                    var LINECHART = $('#OnUsVsOffUsTransaction');
                    new Chart(LINECHART, {
                        type: 'pie',
                        options: {
                            legend: {
                                display: true,
                                position: 'left',
                            },
                            title: {
                                display: true,
                                text: 'On Us VS Off Us'
                            },
                        },
                        data: getChartData(result["d"][1]["Value"], result["d"][0]["Value"], 
                            ["rgba(77, 193, 75, 0.4)", "rgba(48, 37, 186, 0.4)"])
                    });
                }
            });
        }

        function getTransactionsByStatus(){
            $.ajax({
                type: "GET",
                url: "TransactionDashboard.aspx/GetTransactionsByStatus",
                contentType: 'application/json; charset=utf-8',
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    alert("Request: " + XMLHttpRequest.toString() + "\n\nStatus: " + textStatus + "\n\nError: " + errorThrown);
                },
                success: function (result) {
                    var LINECHART = $('#TransactionsByStatus');
                    new Chart(LINECHART, {
                        type: 'pie',
                        options: {
                            legend: {
                                display: true,
                                position: 'left',
                            },
                            title: {
                                display: true,
                                text: 'Transactions By Status'
                            },
                        },
                        data: getChartData(result["d"][1]["Value"], result["d"][0]["Value"],
                            ["rgba(77, 193, 75, 0.4)", "rgba(192, 37, 36, 0.4)", "rgba(48, 37, 186, 0.4)"])
                    });
                }
            });
        }

        function getTransactionsByType() {
            $.ajax({
                type: "GET",
                url: "TransactionDashboard.aspx/GetTransactionsByType",
                contentType: 'application/json; charset=utf-8',
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    alert("Request: " + XMLHttpRequest.toString() + "\n\nStatus: " + textStatus + "\n\nError: " + errorThrown);
                },
                success: function (result) {
                    var LINECHART = $('#TransactionsByType');
                    debugger;
                    new Chart(LINECHART, {
                        type: 'horizontalBar',
                        options: getChartOptions('Transactions By Type(Successful)'),
                        data: getChartData(result["d"][1]["Value"], result["d"][0]["Value"])
                    });
                }
            });
        }

        function showModal(html) {
            document.getElementById('modalwindowserver').innerHTML = html;
            $("#registermodal").modal('show');
        }

        $(document).ready(function () {
            getHourlyTransactionSummary();
            getOnUsVsOffUsTransactionSummary();
            getNoTransactionSummary();
            getTopTransactingAtms();
            getTransactionsByStatus();
            getTransactionsByType();

            $("#btnNoTransactionAtmDetail").on('click', function () {
                $.ajax({
                    type: "GET",
                    url: "TransactionDashboard.aspx/GetNoTransactionAtmDetail",
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
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Literal ID="Literal_errorMsg" runat="server" EnableViewState="false"></asp:Literal>
        <div class="page">
            <!-- navbar-->
            <header class="header">
                <nav class="navbar">
                    <div class="container-fluid">
                        <div class="navbar-holder d-flex align-items-center justify-content-between">
                            <div class="navbar-header">
                                <span style="color: white; font-size: 30px; text-transform: uppercase;">Transaction Monitoring Dashboard</span>
                            </div>
                        </div>
                    </div>
                </nav>
            </header>

            <section class="statistics" style="margin-top:5px">
                <div class="container-fluid row">
                    <div class="col-lg-12">
                        <div class="row d-flex">
                            <div class="col-lg-8">
                                <!-- Hourly Transaction-->
                                <div class="panel">
                                    <div class="panel-heading">
                                        <div class="row">
                                            <div class="col-lg-9">
                                                <canvas id='HourlyTransaction'></canvas>
                                            </div>
                                            <div class="col-lg-3 data-usage" style="margin-top:5%">
                                                <div>Total Transactions</div>
                                                <br />
                                                <div id="progress-circle-total-transactions"></div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-4">
                                <div class="panel">
                                    <div class="panel-heading">
                                        <div class="row">
                                            <canvas id="noTransactionAtm" height="180"></canvas>
                                        </div>
                                    </div>
                                    <a href="#" id="btnNoTransactionAtmDetail">
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
                            <div class="col-lg-3">
                                <!-- Transactions By Status-->
                                <div class="panel">
                                    <div class="panel-heading">
                                        <div class="row">
                                            <canvas id='TransactionsByStatus' height='180'></canvas>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-3">
                                <!-- Transactions By Type-->
                                <div class="panel">
                                    <div class="panel-heading">
                                        <div class="row">
                                            <canvas id='TransactionsByType' height='180'></canvas>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-3">
                                <!-- Transactions By Type-->
                                <div class="panel">
                                    <div class="panel-heading">
                                        <div class="row">
                                            <canvas id='topAtms' height='180'></canvas>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-3">
                                <!-- Transactions By Type-->
                                <div class="panel">
                                    <div class="panel-heading">
                                        <div class="row">
                                            <canvas id='OnUsVsOffUsTransaction' height='180'></canvas>
                                        </div>
                                    </div>
                                    
                                </div>
                            </div>
                        </div>
                    </div>
                 </div>
             </section>
        </div>
        
        <div class="modal fade" id="registermodal">
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
