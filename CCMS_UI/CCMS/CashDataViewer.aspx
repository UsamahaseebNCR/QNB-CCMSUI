<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CashDataViewer.aspx.cs"
    Inherits="CCMSUI.CCMS.CashDataViewer" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>EJ Log Viewer</title>
    <link href="../content/iSuite.css" rel="stylesheet" />
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
</head>
<body style="margin-left: 0; margin-top: 0; margin-right: 0" onload="hideProgressBar();">
    <form id="frmEJLogViewer" method="post" runat="server">
    <table id="maintable" style="width: 100%">
        <tr>
            <td class="PageHeading" style="vertical-align: middle">
                Cash Data Viewer
            </td>
        </tr>
        <tr>
            <td width="100%">
                <div align="right">
                    <asp:Button ID="Button_ShowOriginalView" runat="server" CssClass="button" Text="Raw File"
                        OnClick="Button_ShowOriginalView_Click" />&nbsp;
                    <asp:Button ID="Button_ShowFriendlyView" runat="server" CssClass="button" Text="Friendly View"
                        OnClick="Button_ShowFriendlyView_Click" />&nbsp;
                    <input type="button" class="button" style="width: 100px" value="Back" onclick="window.location='TerminalFileStatusMonitoring.aspx'" />
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <div id="message">
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <table width="100%" cellpadding="0" cellspacing="0">
                    <tr>
                        <td valign="top">
                            <div id="Div_DisplayCounterFile" style=" word-wrap:break-word;  overflow:auto;  font-family: 'Verdana'; background-color: #FFFFCA;
                                padding-left: 5px;" runat="server" >
                                <asp:Literal ID="Literal_data" runat="server" ></asp:Literal>
                            </div>
                        </td>
                    </tr>
                </table>
                <asp:Literal ID="Literal_js" runat="server" EnableViewState="False" ></asp:Literal>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
