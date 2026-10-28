<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Other.aspx.cs" Inherits="CCMSUI.admin.Other" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>CCMS</title>
    <link href="../content/iSuite.css" rel="stylesheet" />
    <script type="text/javascript">
        window.parent.window.contentPage = '';
        

    </script>
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
</head>
<body onload="hideProgressBar();">
    <form id="form1" runat="server">
    <div>
        <div class="PageHeading">
            <div style="padding-top: 5px;">
                Please select your option
            </div>
        </div>
        <br />
        <div id="menus" style="padding-left: 10px" runat="server">
            <div class="DIVHomePage" id="DivGraph" runat="server">
                <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                    <tr style="background-image: url(../images/welcomeButtonsBG.jpg); height: 100px;
                        width: 158px">
                        <td style="width: 158px; cursor: pointer" onclick="location.href='../GraphList.aspx'">
                            Graphs<br />
                            <img alt="" src="../images/reports.jpg" />
                        </td>
                    </tr>
                </table>
            </div>
            &nbsp;
            <div class="DIVHomePage" id="DivCustomDashboard" runat="server">
            </div>
        </div>
    </div>
    </form>
</body>
</html>
