<%@ Page Language="C#" AutoEventWireup="true" Codebehind="FtpFileTransfer_details.aspx.cs" Inherits="CCMSUI.CCMS.FtpFileTransfer_details" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
    <title>Untitled Page</title>
    <script src="../content/shortcut.js" type="text/javascript" language="javascript"></script>
    <link href="../content/iSuite.css" rel="stylesheet" />
    <script type="text/javascript" language="javascript">
    window.parent.window.contentPage = '';   
    </script>
</head>

<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)' onload="hideProgressBar();">
    <form id="form1" runat="server">
        <table cellpadding="0" cellspacing="0" border="0" width="100%">
            <tr>
                <td class="PageHeading" align="left" colspan="6">
                    <asp:Literal ID="Literal_Heading" runat="server" EnableViewState="false">Order Download and Daily Feed Status Details</asp:Literal>
                </td>
            </tr>
            <tr><td class="SearchHeadPaneGradient" colspan="6"></td></tr>          
        </table>
        
        <table style ="padding-left:10px" cellpadding="0" cellspacing="0" border="0" width="100%">
            <tr>
                <td style="width : 20%"><b>Organization:</b></td>
                <td style="width : 30%">
                    <asp:Label ID="Label_organization" runat="server"></asp:Label>
                </td>
                <td style="width : 20%"><b>Task Type:</b></td>
                <td style="width : 30%">
                    <asp:Label ID="Label_tasktype" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="width : 20%"><b>File Name:</b></td>
                <td style="width : 30%">
                    <asp:Label ID="Label_filename" runat="server"></asp:Label>
                </td>
                <td style="width : 20%"><b>Status:</b></td>
                <td style="width : 30%">
                    <asp:Label ID="Label_status" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="width : 20%"><b>Creation Time:</b></td>
                <td style="width : 30%">
                    <asp:Label ID="Label_creationtime" runat="server"></asp:Label>
                </td>
                <td style="width : 20%"><b>End Time:</b></td>
                <td style="width : 30%">
                    <asp:Label ID="Label_endtime" runat="server"></asp:Label>
                </td>
            </tr>            
            <tr>
                <td style="width : 20%"><b>Last Invoked At:</b></td>
                <td style="width : 30%">
                    <asp:Label ID="Label_lastinvoked" runat="server"></asp:Label>
                </td>
                <td style="width : 20%"><b>Retry Count:</b></td>
                <td style="width : 30">
                    <asp:Label ID="Label_retry" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="width : 20%"><b>Failure Reason:</b></td>
                <td style="width : 80%" colspan="3">
                    <asp:Label ID="Label_reason" runat="server"></asp:Label>
                </td>                
            </tr>            
            <asp:Literal ID="literal_trial" runat="server"></asp:Literal>
            <tr>
                <td colspan="2" style="width : 50%"></td>
                <td colspan="2" style="width : 50%; height: 20px" align="right">
                    <input id="Button1" type="button" value="Back" onclick="window.location='FtpFileTransferStatus.aspx';" class="button" style="width: 63px"/>                    
                </td>
            </tr>            
        </table>
    </form>
</body>

</html>
