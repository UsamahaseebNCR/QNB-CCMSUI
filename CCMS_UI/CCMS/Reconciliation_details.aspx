<%@ Page Language="C#" AutoEventWireup="true" Codebehind="Reconciliation_details.aspx.cs"
    Inherits="CCMSUI.CCMS.Reconciliation_details" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Reconciliation Batch Details</title>
    <link href="../content/iSuite.css" rel="stylesheet" />
    <script src="../content/shortcut.js" type="text/javascript"></script>
    <style>
        .detail-table {
            width: 90%;
            margin: 10px auto;
            border-collapse: collapse;
        }
        .detail-table td {
            padding: 5px 10px;
        }
        .detail-table td.label {
            font-weight: bold;
            width: 180px;
            text-align: right;
        }
        .detail-table td.value {
            width: 250px;
            text-align: left;
        }
        .button-row {
            text-align: center;
            margin-top: 15px;
        }
    </style>
</head>
<body onkeydown="checkForCtrlKey(window.event);" onkeyup="checkForCharKey(window.event)" onload="hideProgressBar();">
    <form id="form1" runat="server">
        <div class="PageHeading" style="margin:10px;">
            <asp:Literal ID="Literal_Heading" runat="server" EnableViewState="false"></asp:Literal>
        </div>

        <table class="detail-table" border="0">
            <tr>
                <td class="label">Creation Time:</td>
                <td class="value"><asp:Label ID="Label_CreationTimestamp" runat="server"></asp:Label></td>
                <td class="label">Retries Remaining:</td>
                <td class="value"><asp:Label ID="Label_RetryCount2" runat="server"></asp:Label></td>
            </tr>
            <tr>
                <td class="label">Status:</td>
                <td class="value"><asp:Label ID="Label_Status" runat="server"></asp:Label></td>
                <td class="label">Last Invoked:</td>
                <td class="value"><asp:Label ID="Label_LastInvoked2" runat="server"></asp:Label></td>
            </tr>
            <tr>
                <td class="label">Transaction Start Date:</td>
                <td class="value"><asp:Label ID="Label_TrxnStartDate" runat="server"></asp:Label></td>
                <td class="label">Transaction End Date:</td>
                <td class="value"><asp:Label ID="Label_TrxnEndDate" runat="server"></asp:Label></td>
            </tr>
            <tr>
                <td class="label">End Time:</td>
                <td class="value"><asp:Label ID="Label_FinishedTimestamp2" runat="server"></asp:Label></td>
                <td class="label">Last Attempt Failed Reason:</td>
                <td class="value"><asp:Label ID="Label_LastAttemptFailedReason2" runat="server"></asp:Label></td>
            </tr>
        </table>

        <div class="button-row">
            <input type="button" value="Back" onclick="window.location='Reconciliation.aspx'" class="button" style="width:80px;" />
            <asp:Button ID="Button_Cancel" runat="server" CssClass="button" OnClick="Button_Cancel_Click" Text="Cancel" />
        </div>
    </form>
</body>
</html>