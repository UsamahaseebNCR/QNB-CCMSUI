<%@ Page Language="C#" AutoEventWireup="true" Codebehind="newDownload.aspx.cs" Inherits="CCMSUI.CCMS.newDownload" %>

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
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)' onload="hideProgressBar();">
    <form id="form1" runat="server">
        <table style="width: 100%" cellspacing="0" cellpadding="0" border="0" class="mainbox">
            <tr>
                <td class="PageHeading" align="left">
                    &nbsp;Download a new File</td>
            </tr>
            <tr>
                <td class="SearchHeadPaneGradient" style="height: 15px">
                </td>
            </tr>
            <tr style="height: 20px">
                <td class="ValidationText" align="center">
                    <asp:Literal ID="Literal_error" runat="server" EnableViewState="False"></asp:Literal></td>
            </tr>
            <tr>
                <td align="center" class="ValidationText" style="height: 13px">
                </td>
            </tr>
            <tr>
                <td style="padding-left: 10px">
                    File Type :
                    <asp:DropDownList ID="DropDownList_fileType" runat="server" Width="148px" CssClass="dropDown">
                    </asp:DropDownList></td>
            </tr>
            <tr>
                <td style="padding-left: 10px">
                    Selected ATM :
                    <asp:Label ID="Label_atm" runat="server" EnableViewState="False"></asp:Label></td>
            </tr>
            <tr>
                <td style="padding-left: 100px">
                    <asp:Button ID="Button_Download" runat="server" CssClass="button" OnClick="Button_Download_Click"
                        Text="Download" />&nbsp;
                    <input type="button" class="button" value="Cancel" onclick="window.location='TerminalFileStatusMonitoring.aspx'" />
                </td>
            </tr>
        </table>
        <input type="button" runat="server" id="refresh" style="wdth: 0px; height: 0px;
            visibility: hidden" />
    </form>
</body>
</html>
