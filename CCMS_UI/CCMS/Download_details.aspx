<%@ Page Language="C#" AutoEventWireup="true" Codebehind="Download_details.aspx.cs"
    Inherits="CCMSUI.CCMS.Download_details" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">

    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>

    <title>Untitled Page</title>
    <link href="../content/iSuite.css" rel="stylesheet" />

    <script type="text/javascript" language="javascript">        
        window.parent.window.contentPage = '';   
//        document.getElementById('div_trial').innerHtml='<table width=75% border=1><tr> <td>Trial Label</td></tr></table>';
    </script>

</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)' onload="hideProgressBar();">
    <form id="form1" runat="server">
        <table cellpadding="0" cellspacing="0" border="0" width="100%" >
            <tr>
                <td class="PageHeading" align="left" colspan="6">
                    <asp:Literal ID="Literal_Heading" runat="server" EnableViewState="false"></asp:Literal>
                </td>
            </tr>
            <tr>
                <td class="SearchHeadPaneGradient" colspan="6">
                </td>
            </tr>
          
        </table>
        <table style ="padding-left:10px" cellpadding="0" cellspacing="0" border="0" width="100%" >
        
          <tr>
                <td style="width : 125px;">
                    Creation Time:
                </td>
                <td style="width : 180px;">
                    <asp:Label ID="Label_CreationTimestamp" runat="server"></asp:Label>
                </td>
                <td style="width : 140px;">
                    <asp:Label ID="Label_RetryCount1" runat="server" Text="Retries Remaining:"></asp:Label>
                </td>
                <td style="width : 150px;">
                    <asp:Label ID="Label_RetryCount2" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    Status:
                </td>
                <td>
                    <asp:Label ID="Label_Status" runat="server"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="Label_LastInvoked1" runat="server" Text="Last Invoked:"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="Label_LastInvoked2" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label8" runat="server">Bytes Transferred:</asp:Label>
                </td>
                <td>
                    <asp:Label ID="Label_BytesDownloaded" runat="server"></asp:Label>
                </td>
                <td>
                    Duration:
                </td>
                <td>
                    <asp:Label ID="Label_Duration" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label_FinishedTimestamp1" runat="server" Text="Job Finished Time:"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="Label_FinishedTimestamp2" runat="server"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="Label_LastAttemptFailedReason1" runat="server">Last Attempt Failed Reason:</asp:Label>
                </td>
                <td>
                    <asp:Label ID="Label_LastAttemptFailedReason2" runat="server"></asp:Label>
                </td>
            </tr>
            
            <asp:Literal ID="literal_trial" runat="server"></asp:Literal><tr>
                <td colspan="3" style="height: 20px">
                    &nbsp;<input id="Button1" type="button" value="Back" onclick="window.location='TerminalFileStatusMonitoring.aspx'" class="button"
                        style="width: 63px"  accesskey="BS"/>
                    <asp:Button ID="Button_Cancel" runat="server" class="button" OnClick="Button_Cancel_Click" />
                </td>
            </tr>
            
        </table>
    </form>
</body>
</html>
