<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ServiceRequestTask.aspx.cs"
    Inherits="CCMSUI.CCMS.ServiceRequestTask" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
    <link rel="stylesheet" href="../content/iSuite.css" />
    <script type="text/javascript">
        window.parent.window.contentPage = window.location;           
    </script>
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)'
    onload="hideProgressBar();">
    <form id="form1" runat="server">
    <table style="width: 100%" cellspacing="0" cellpadding="0" border="0" class="mainbox">
        <tr>
            <td class="PageHeading" align="left">
                &nbsp;Service
            </td>
        </tr>
        <tr>
            <td class="SearchHeadPaneGradient" style="height: 15px">
            </td>
        </tr>
        <tr style="height: 20px">
            <td class="ValidationText" align="center">
                <asp:literal id="Literal_error" runat="server" enableviewstate="False"></asp:literal>
            </td>
        </tr>
        <tr>
            <td align="center" class="ValidationText" style="height: 13px">
            </td>
        </tr>
        <tr>
            <td style="padding-left: 10px">
                Service Name :
                <asp:textbox id="Textbox_ServiceName" runat="server"></asp:textbox>
            </td>
        </tr>
        <tr>
            <td style="padding-left: 10px">
                Selected ATM :
                <asp:label id="Label_atm" runat="server" enableviewstate="False"></asp:label>
            </td>
        </tr>
        <tr>
            <td style="padding-left: 100px">
                
                <asp:button id="Button_Schedule" runat="server" cssclass="button" onclick="Button_Schedule_Click"
                    text="Schedule" />
                &nbsp;
                <input type="button" class="button" value="Cancel" onclick="window.location='TerminalFileStatusMonitoring.aspx'" />
            </td>
        </tr>
    </table>
    <input type="button" runat="server" id="refresh" style="wdth: 0px; height: 0px; visibility: hidden" />
    </form>
</body>
</html>
