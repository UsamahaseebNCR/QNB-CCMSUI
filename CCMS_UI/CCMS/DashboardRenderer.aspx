<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DashboardRenderer.aspx.cs" Inherits="CCMSUI.CCMS.DashboardRenderer" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="refresh" content="300" id="metaRefresh" runat="server" />
    <link href="../content/styleSheets/bootstrap.min.css" rel="stylesheet" />
    <link href="../content/styleSheets/bootstrap-theme.min.css" rel="stylesheet" />
    <script src="../content/shortcut.js" type="text/javascript" language="javascript"></script>
    <title id="Title" runat="server">Dashboard</title>
    <link href="../content/iSuite.css" rel="stylesheet" />
    <style>
        .navbar-inverse {
            background-image: -webkit-linear-gradient(top,#1D6F35 0,#77BF5C 100%);
            background-image: -o-linear-gradient(top,#1D6F35 0,#77BF5C 100%);
            background-image: -webkit-gradient(linear,left top,left bottom,from(#1D6F35),to(#77BF5C));
            background-image: linear-gradient(to bottom,#1D6F35 0,#77BF5C 100%);
            filter: progid:DXImageTransform.Microsoft.gradient(startColorstr='#ff1D6F35', endColorstr='#ff77BF5C', GradientType=0);
            filter: progid:DXImageTransform.Microsoft.gradient(enabled=false);
            background-repeat: repeat-x;
            border-color: #1D6F35;
        }
    </style>
</head>
<body style="margin: 0;">
    <form id="form1" runat="server">
        <div>
            <asp:Literal ID="Literal_Message" runat="server"></asp:Literal>
        </div>
        <div class="navbar navbar-inverse navbar-fixed-top" role="navigation">
            <div class="container">
                <div class="navbar-header" style="padding: 10px 0px 5px 10px">
                    <span style="color: white; font-size: 30px; text-transform: uppercase;"><asp:Label ID="Label_Title" runat="server"></asp:Label> </span>
                </div>
            </div>
        </div>
        <div class="container" id="chart" runat="server" style="margin-top:60px"></div>
    </form>
</body>
</html>
