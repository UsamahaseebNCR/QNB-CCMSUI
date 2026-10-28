<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DashboardList.aspx.cs" Inherits="CCMSUI.CCMS.DashboardList" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Dashboard List</title>
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
    <link href="../content/iSuite.css" rel="stylesheet" />
</head>
<body onload="hideProgressBar();">
    <form id="form1" runat="server">
    <div>
        <table id="table2" cellspacing="0" cellpadding="0" width="100%" border="0" style="padding-left: 10px">
            <tr>
                <td class="PageHeading" align="left" colspan="2">
                    Dashboard List
                </td>
            </tr>
            <tr>
                <td class="SearchHeadPaneGradient" colspan="2">
                </td>
            </tr>
            <tr>
                <td style="height: 20px" id="dDashboardList" runat="server">
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>