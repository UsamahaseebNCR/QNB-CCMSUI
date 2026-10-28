<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CustomReport.aspx.cs" Inherits="CCMSUI.CCMS.CustomReport" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <script src="../content/Scripts/jquery-1.7.1.min.js" type="text/javascript"></script>
    <title id="ReportTitle" runat="server">Report Builder</title>
    <link href="../content/iSuite.css" rel="stylesheet" />

    <link href="../content/Scripts/datatable/jquery.dataTables.min.css" rel="stylesheet" />
    <link href="../content/styleSheets/bootstrap.min.css" rel="stylesheet" />
    <link href="../content/styleSheets/bootstrap-theme.min.css" rel="stylesheet" />
    <script src="../content/Scripts/datatable/jquery.dataTables.min.js" type="text/javascript" language="javascript"></script>
    <script src="../content/Scripts/datatable/dataTables.buttons.min.js"></script>
    <script src="../content/Scripts/datatable/buttons.flash.min.js"></script>
    <script src="../content/Scripts/datatable/jszip.min.js"></script>
    <script src="../content/Scripts/datatable/pdfmake.min.js"></script>
    <script src="../content/Scripts/datatable/vfs_fonts.js"></script>
    <script src="../content/Scripts/datatable/buttons.html5.min.js"></script>
    <script src="../content/Scripts/datatable/buttons.print.min.js"></script>
    <style>
        #GridView_Result_paginate {
            float: left;
            text-align: left;
        }

        #GridView_Result_info {
            float: right;
            text-align: right;
        }
    </style>
    <script>
        $(document).ready(function () {
            var myGlyph = new Image();
            myGlyph.src = '../images/logoNCR.jpg';
            var reportName = $('#ReportName').text().trim();

            $('#GridView_Result').DataTable({
                dom: 'Bfrtip',
                buttons: [
                    {
                        className: 'btn btn-sm btn-success',
                        extend: 'csv',
                        title: reportName
                    },
                    {
                        className: 'btn btn-sm btn-success',
                        extend: 'excel',
                        title: reportName
                    },
                    {
                        className: 'btn btn-sm btn-success',
                        extend: 'pdfHtml5',
                        //orientation: 'landscape',
                        customize: function (doc) {
                            doc.content.splice(0, 1);
                            doc.defaultStyle.fontSize = 8;
                            doc['header'] = (function (page, pages) {
                                return {
                                    columns: [
                                         {
                                             alignment: 'left',
                                             image: getBase64Image(myGlyph),
                                             //margin: [10, 0, 10, 0],
                                             //width: 24
                                         },
                                         {
                                             alignment: 'right',
                                             text: reportName,
                                             fontSize :20,
                                         }
                                    ],
                                    margin: [10, 10]
                                }
                            });

                            // Change dataTable layout (Table styling)
                            // To use predefined layouts uncomment the line below and comment the custom lines below
                            // doc.content[0].layout = 'lightHorizontalLines'; // noBorders , headerLineOnly
                            var objLayout = {};
                            objLayout['hLineWidth'] = function (i) { return .5; };
                            objLayout['vLineWidth'] = function (i) { return .5; };
                            objLayout['hLineColor'] = function (i) { return '#aaa'; };
                            objLayout['vLineColor'] = function (i) { return '#aaa'; };
                            objLayout['paddingLeft'] = function (i) { return 4; };
                            objLayout['paddingRight'] = function (i) { return 4; };
                            doc.content[0].layout = objLayout;
                        },
                        //title: ''
                    },
                    {
                        className: 'btn btn-sm btn-success',
                        extend: 'print',
                        title: reportName
                    },

                ],
                searching: false
            });
        });

        function getBase64Image(img) {
            var canvas = document.createElement("canvas");
            canvas.width = img.width;
            canvas.height = img.height;
            var ctx = canvas.getContext("2d");
            ctx.drawImage(img, 0, 0);
            return canvas.toDataURL("image/png");
        }
    </script>
    <style>
        
        table > thead > tr {
            background-color : #444444 !important;
        }

        table > thead > tr > th {
            color : #FFFFE4 !important;
        }
        
        .heading {
            font-weight: bold
        }
    </style>
</head>
<body style="padding:10px">
    <form id="form1" runat="server">
        <div class="container">
            <div class="col-lg-12" style="border-bottom: 2px solid #800000">
                <div class="col-lg-6" style="margin-top:15px"><span><img src="../images/logoReport.png" /></span></div>
                <div class="col-lg-6" style="float:right; text-align:right">
                    <h3 class="heading" id="ReportName">
                         <asp:Literal id="ReportHeader" runat="server" />
                    </h3>
                </div>
            </div>
            <div class="col-lg-12" style="margin-top:10px">
                <div style="overflow:auto">
                    <asp:GridView CssClass="table-hover" ID="GridView_Result" runat="server" CellPadding="3" BackColor="White" 
                            BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px">
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
                    </asp:GridView>
                </div>
            </div>
        </div>
    </form>
</body>
</html>