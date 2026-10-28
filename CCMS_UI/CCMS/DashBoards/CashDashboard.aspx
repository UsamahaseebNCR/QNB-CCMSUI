<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CashDashboard.aspx.cs" Inherits="CCMSUI.CCMS.DashBoards.CashDashboard" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="description" content="" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <meta name="robots" content="all,follow" />
    <title>Cash Management Dashboard</title>
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
        $(document).ready(function () {
            getMachinesNotReplenishedSummary();
            getCashOutSummary();
            getLowBalanceDetail();
            getReplenishments();

            $("#btnMachinesNotReplenishedDetail").on('click', function () {
                $.ajax({
                    type: "GET",
                    url: "CashDashboard.aspx/GetMachinesNotReplenishedDetail",
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

            $("#btnCashOutDetail").on('click', function () {
                $.ajax({
                    type: "GET",
                    url: "CashDashboard.aspx/GetCashOutDetail",
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

            $("#btnReplenishmentDetail").on('click', function () {
                $.ajax({
                    type: "GET",
                    url: "CashDashboard.aspx/GetReplenishmentsByDayDetail",
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
        })

        function showModal(html) {
            document.getElementById('modalwindowserver').innerHTML = html;
            $("#registermodal").modal('show');
        }

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

        function getMachinesNotReplenishedSummary() {
            $.ajax({
                type: "GET",
                url: "CashDashboard.aspx/GetMachinesNotReplenishedSummary",
                contentType: 'application/json; charset=utf-8',
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    alert("Request: " + XMLHttpRequest.toString() + "\n\nStatus: " + textStatus + "\n\nError: " + errorThrown);
                },
                success: function (result) {
                    $('#progress-circle-machines-not-replenished').percircle({
                        progressBarColor: '#D40C0C',
                        text: result["d"] == 0 ? '0' : result["d"],
                        percent: 100
                    });
                }
            });
        }

        function getCashOutSummary() {
            $.ajax({
                type: "GET",
                url: "CashDashboard.aspx/GetCashOutSummary",
                contentType: 'application/json; charset=utf-8',
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    alert("Request: " + XMLHttpRequest.toString() + "\n\nStatus: " + textStatus + "\n\nError: " + errorThrown);
                },
                success: function (result) {
                    $('#progress-circle-cash-out').percircle({
                        progressBarColor: '#D40C0C',
                        text: result["d"][0]["Value"] == 0 ? '0' : result["d"][0]["Value"],
                        percent: 100
                    });
                    $("#cashOutSla").text(result["d"][1]["Value"])
                }
            });
        }

        function getLowBalanceDetail() {
            $.ajax({
                type: "GET",
                url: "CashDashboard.aspx/GetMinOperatingBalanceDetail",
                contentType: 'application/json; charset=utf-8',
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    alert("Request: " + XMLHttpRequest.toString() + "\n\nStatus: " + textStatus + "\n\nError: " + errorThrown);
                },
                success: function (result) {
                    $("#lowBalance").html(result["d"])
                    $("#tblLowBalance").DataTable({
                        searching: false,
                        "lengthMenu": [[5, 10, 20], [5, 10, 20]]
                    });
                }
            });
        }

        function getReplenishments() {
            $.ajax({
                type: "GET",
                url: "CashDashboard.aspx/GetReplenishmentsByDay",
                contentType: 'application/json; charset=utf-8',
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    alert("Request: " + XMLHttpRequest.toString() + "\n\nStatus: " + textStatus + "\n\nError: " + errorThrown);
                },
                success: function (result) {
                    var LINECHART = $('#replenishments');
                    var myLineChart = new Chart(LINECHART, {
                        type: 'bar',
                        options: getChartOptions('Replenishments By Day'),
                        data: getChartData(result["d"][1]["Value"], result["d"][0]["Value"])
                    });
                }
            });
        }
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
                                <span style="color: white; font-size: 30px; text-transform: uppercase;">Cash Management Dashboard</span>
                            </div>
                        </div>
                    </div>
                </nav>
            </header>

            <section class="statistics" style="margin-top:5px">
                <div class="container-fluid">
                    <div class="row">
                        <div class="col-lg-8">
                            <div class="panel panel-green">
                                <div class="panel-heading" style="min-height:0px">
                                    Low Balance ATMs
                                </div>
                                <div class="panel-body" id="lowBalance" style="overflow:auto">

                                </div>
                            </div>
                        </div>
                         <div class="col-lg-4">
                            <div class="panel panel-green">
                                <div class="panel-heading" style="min-height:0px">Replenishment</div>
                                <div class="panel-body">
                                    <div class="row">
                                        <div class="col-md-6">
                                            <div id="progress-circle-machines-not-replenished"></div>
                                        </div>
                                        <div class="col-md-6 text-right data-usage">
                                            <div>Machines Not Replenishment For More Than 10 Days</div>
                                        </div>
                                    </div>
                                </div>
                                 <a href="#" id="btnMachinesNotReplenishedDetail">
                                    <div class="panel-footer">
                                        <span class="pull-left">View Details</span>
                                        <span class="pull-right"><i class="fa fa-arrow-circle-right"></i></span>
                                        <div class="clearfix"></div>
                                    </div>
                                </a>
                            </div>
                        </div>
                        
                    </div>
                    <div class="row">
                       <div class="col-lg-8">
                            <div class="panel panel-green">
                                <div class="panel-heading" style="min-height:0px">
                                    Replenishments Summary(Month to Date)
                                </div>
                                <div class="panel-body">
                                    <canvas id="replenishments" height="80"></canvas>
                                </div>
                                <a href="#" id="btnReplenishmentDetail">
                                    <div class="panel-footer">
                                        <span class="pull-left">View Details</span>
                                        <span class="pull-right"><i class="fa fa-arrow-circle-right"></i></span>
                                        <div class="clearfix"></div>
                                    </div>
                                </a>
                            </div>
                        </div>
                        <div class="col-lg-4">
                            <div class="panel panel-green">
                                <div class="panel-heading" style="min-height:0px">Cash Outs</div>
                                <div class="panel-body">
                                    <div class="row">
                                        <div class="col-md-6">
                                            <div id="progress-circle-cash-out"></div>
                                        </div>
                                        <div class="col-md-6 text-right data-usage">
                                            <div>No. Of Cash Outs</div>
                                            <br />
                                            <div>
                                                <small>Agreed SLA</small>
                                                <span id="cashOutSla">0</span>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <a href="#" id="btnCashOutDetail">
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
             </section>
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