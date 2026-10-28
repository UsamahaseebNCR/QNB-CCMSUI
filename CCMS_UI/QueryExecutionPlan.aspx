<%@ page language="C#" autoeventwireup="true" codebehind="QueryExecutionPlan.aspx.cs" inherits="CCMSUI.QueryExecutionPlan" %>

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <!-- The above 3 meta tags *must* come first in the head; any other head content must come *after* these tags -->
    <meta name="description" content="">
    <meta name="author" content="">
    <link rel="icon" href="favicon.ico">

    <title>Query Execution Plan Generator</title>

    <!-- Bootstrap core CSS -->
    <link href="content/bootstrap/css/bootstrap.min.css" rel="stylesheet" />

    <!-- Custom CSS -->
    <link href="content/css/queryexecutionplan.css" rel="stylesheet" />

    <!-- IE10 viewport hack for Surface/desktop Windows 8 bug -->
    <link href="content/bootstrap/css/ie10-viewport-bug-workaround.css" rel="stylesheet" />

    <!-- HTML5 shim and Respond.js for IE8 support of HTML5 elements and media queries -->
    <!--[if lt IE 9]>
      <script src="https://oss.maxcdn.com/html5shiv/3.7.2/html5shiv.min.js"></script>
      <script src="https://oss.maxcdn.com/respond/1.4.2/respond.min.js"></script>
    <![endif]-->
</head>

<body>

    <nav class="navbar navbar-inverse navbar-fixed-top">
        <div class="container">
            <div class="navbar-header">
                <div class="navbar-brand">Query Execution Plan Generator</div>
            </div>
        </div>
    </nav>

    <div class="container">
        <div class="page-header">
            <h1>Query Execution Plan Generator</h1>
        </div>
        <p class="lead">Use this tool to generate planned and actual query execution plan of a SQL Server query.</p>
        <%--<div id="successMessage" class="alert alert-success" runat="server">
            <strong>Success!</strong> Plan is successfully generated.
        </div>--%>
        <div id="errorMessage" class="alert alert-danger" runat="server">
            <strong>Error!</strong>
        </div>
        <form id="queryForm" runat="server">
            <div class="form-group">
                <label for="query">Enter your query</label>
                <asp:textbox id="query" textmode="multiline" class="form-control" rows="10" runat="server"></asp:textbox>
            </div>
            <asp:button id="getEstimatedPlan" runat="server" class="btn btn-warning" text="Get Estimated Plan" OnClick="getEstimatedPlan_Click" />
            <asp:button id="getActualPlan" runat="server" class="btn btn-success" text="Get Actual Plan" OnClick="getActualPlan_Click" />
        </form>
    </div>
    <!-- /.container -->

    <footer class="footer">
        <div class="container">
            <p class="text-muted">Developed at NCR Corporation.</p>
        </div>
    </footer>

    <!-- Bootstrap core JavaScript
    ================================================== -->
    <!-- Placed at the end of the document so the pages load faster -->
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.11.3/jquery.min.js"></script>
    <script>window.jQuery || document.write('<script src="content/bootstrap/js/jquery.min.js"><\/script>')</script>
    <script src="content/bootstrap/js/bootstrap.min.js"></script>
    <!-- IE10 viewport hack for Surface/desktop Windows 8 bug -->
    <script src="content/bootstrap/js/ie10-viewport-bug-workaround.js"></script>
</body>
</html>