<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ShowSettlementData.aspx.cs"
    Inherits="CCMSUI.CCMS.ShowSettlementData" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>EJ Log Viewer</title>
    <link href="../content/iSuite.css" rel="stylesheet" />
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
</head>
<body style="margin-left: 0; margin-top: 0; margin-right: 0">
    <form id="frmEJLogViewer" method="post" runat="server">
    <table id="maintable" style="width: 100%">
        <tr>
            <td class="PageHeading" style="vertical-align: middle">
                Settlement
            </td>
        </tr>
        <tr>
            <td>
                <table>
                    <tr>
                        <td>
                            ATM Site And Number
                        </td>
                        <td>
                            <b>
                                <asp:Label ID="Label_ATMSiteAndNumber" runat="server"></asp:Label>
                            </b>
                        </td>
                    </tr>
                   <tr>
                        <td>
                            Replenishment Date
                        </td>
                        <td>
                            <b>
                                <asp:Label ID="Label_ReplenishmentDate" runat="server"></asp:Label>
                            </b>
                        </td>
                    </tr>
                     <tr>
                        <td>
                            Seal Number
                        </td>
                        <td>
                            <b>
                                <asp:Label ID="Label_SealNo" runat="server"></asp:Label>
                            </b>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            GL Number
                        </td>
                        <td>
                            <b>
                                <asp:Label ID="Label_GLNo" runat="server"></asp:Label>
                            </b>
                        </td>
                    </tr>
                   
                    
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <table width="80%" cellpadding="0" cellspacing="0">
                    <tr>
                        <td valign="top">
                            <span style="font-size: large;"><b>SYSTEM</b></span>
                            <table border="1">
                                <tr>
                                    <td>
                                    </td>
                                    <td>
                                        <b>
                                            <asp:Label ID="Label_type1" runat="server" Text="Type 1"></asp:Label></b>
                                    </td>
                                    <td>
                                        <b>
                                            <asp:Label ID="Label_type2" runat="server" Text="Type 2"></asp:Label></b>
                                    </td>
                                    <td>
                                        <b>
                                            <asp:Label ID="Label_type3" runat="server" Text="Type 3"></asp:Label></b>
                                    </td>
                                    <td>
                                        <b>
                                            <asp:Label ID="Label_type4" runat="server" Text="Type 4"></asp:Label></b>
                                    </td>
                                    <td>
                                        <b>Total</b>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <b>Cash Replenished</b>
                                    </td>
                                    <td align="right">
                                        <asp:Label ID="Label_Added1" runat="server"></asp:Label>
                                    </td>
                                    <td align="right">
                                        <asp:Label ID="Label_Added2" runat="server"></asp:Label>
                                    </td>
                                    <td align="right">
                                        <asp:Label ID="Label_Added3" runat="server"></asp:Label>
                                    </td>
                                    <td align="right">
                                        <asp:Label ID="Label_Added4" runat="server"></asp:Label>
                                    </td>
                                    <td align="right">
                                        <asp:Label ID="Label_TotalAdded" runat="server"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <b>Cash Dispensed</b>
                                    </td>
                                    <td align="right">
                                        <asp:Label ID="Label_Dispensed1" runat="server"></asp:Label>
                                    </td>
                                    <td align="right">
                                        <asp:Label ID="Label_Dispensed2" runat="server"></asp:Label>
                                    </td>
                                    <td align="right">
                                        <asp:Label ID="Label_Dispensed3" runat="server"></asp:Label>
                                    </td>
                                    <td align="right">
                                        <asp:Label ID="Label_Dispensed4" runat="server"></asp:Label>
                                    </td>
                                    <td align="right">
                                        <asp:Label ID="Label_TotalDispensed" runat="server"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <b>Cash Returned</b>
                                    </td>
                                    <td align="right">
                                        <asp:Label ID="Label_Returned1" runat="server"></asp:Label>
                                    </td>
                                    <td align="right">
                                        <asp:Label ID="Label_Returned2" runat="server"></asp:Label>
                                    </td>
                                    <td align="right">
                                        <asp:Label ID="Label_Returned3" runat="server"></asp:Label>
                                    </td>
                                    <td align="right">
                                        <asp:Label ID="Label_Returned4" runat="server"></asp:Label>
                                    </td>
                                    <td align="right">
                                        <asp:Label ID="Label_TotalReturned" runat="server"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <b>Cash Remaining</b>
                                    </td>
                                    <td align="right">
                                        <asp:Label ID="Label_Remaining1" runat="server"></asp:Label>
                                    </td>
                                    <td align="right">
                                        <asp:Label ID="Label_Remaining2" runat="server"></asp:Label>
                                    </td>
                                    <td align="right">
                                        <asp:Label ID="Label_Remaining3" runat="server"></asp:Label>
                                    </td>
                                    <td align="right">
                                        <asp:Label ID="Label_Remaining4" runat="server"></asp:Label>
                                    </td>
                                    <td align="right">
                                        <asp:Label ID="Label_TotalRemaining" runat="server"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td>
                            <span style="font-size: large"><b>CIT</b></span>
                            <table border="1">
                                <tr>
                                    <td>
                                    </td>
                                    <td>
                                        <b>
                                            <asp:Label ID="Label_ctype1" runat="server" Text="Type 1"></asp:Label></b>
                                    </td>
                                    <td>
                                        <b>
                                            <asp:Label ID="Label_ctype2" runat="server" Text="Type 2"></asp:Label></b>
                                    </td>
                                    <td>
                                        <b>
                                            <asp:Label ID="Label_ctype3" runat="server" Text="Type 3"></asp:Label></b>
                                    </td>
                                    <td>
                                        <b>
                                            <asp:Label ID="Label_ctype4" runat="server" Text="Type 4"></asp:Label></b>
                                    </td>
                                    <td>
                                        <b>Total</b>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <b>Cash Replenished</b>
                                    </td>
                                    <td align="right">
                                        <asp:Label ID="Label_CITAdded1" runat="server"></asp:Label>
                                    </td>
                                    <td align="right">
                                        <asp:Label ID="Label_CITAdded2" runat="server"></asp:Label>
                                    </td>
                                    <td align="right">
                                        <asp:Label ID="Label_CITAdded3" runat="server"></asp:Label>
                                    </td>
                                    <td align="right">
                                        <asp:Label ID="Label_CITAdded4" runat="server"></asp:Label>
                                    </td>
                                    <td align="right">
                                        <asp:Label ID="Label_CITTotalAdded" runat="server"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <b>Cash Dispensed</b>
                                    </td>
                                    <td align="right">
                                        <asp:Label ID="Label_CITDispensed1" runat="server"></asp:Label>
                                    </td>
                                    <td align="right">
                                        <asp:Label ID="Label_CITDispensed2" runat="server"></asp:Label>
                                    </td>
                                    <td align="right">
                                        <asp:Label ID="Label_CITDispensed3" runat="server"></asp:Label>
                                    </td>
                                    <td align="right">
                                        <asp:Label ID="Label_CITDispensed4" runat="server"></asp:Label>
                                    </td>
                                    <td align="right">
                                        <asp:Label ID="Label_CITTotalDispensed" runat="server"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <b>Cash Returned</b>
                                    </td>
                                    <td align="right">
                                        <asp:Label ID="Label_CITReturned1" runat="server"></asp:Label>
                                    </td>
                                    <td align="right">
                                        <asp:Label ID="Label_CITReturned2" runat="server"></asp:Label>
                                    </td>
                                    <td align="right">
                                        <asp:Label ID="Label_CITReturned3" runat="server"></asp:Label>
                                    </td>
                                    <td align="right">
                                        <asp:Label ID="Label_CITReturned4" runat="server"></asp:Label>
                                    </td>
                                    <td align="right">
                                        <asp:Label ID="Label_CITTotalReturned" runat="server"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <b>Cash Rejected</b>
                                    </td>
                                    <td align="right">
                                        <asp:Label ID="Label_CITRejected1" runat="server"></asp:Label>
                                    </td>
                                    <td align="right">
                                        <asp:Label ID="Label_CITRejected2" runat="server"></asp:Label>
                                    </td>
                                    <td align="right">
                                        <asp:Label ID="Label_CITRejected3" runat="server"></asp:Label>
                                    </td>
                                    <td align="right">
                                        <asp:Label ID="Label_CITRejected4" runat="server"></asp:Label>
                                    </td>
                                    <td align="right">
                                        <asp:Label ID="Label_CITTotalRejected" runat="server"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                            <div>
                                <table>
                                    <tr>
                                        <td style="font-size: large">
                                            ATM Status:
                                        </td>
                                        <td style="font-size: large">
                                            <b>
                                                <asp:Label ID="Label_CITATMStatus" runat="server"></asp:Label></b>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" style="font-size: large; font-weight: bold">
                            Reason
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:TextBox ID="Textbox_Reason" runat="server" TextMode="MultiLine" Height="117px"
                                Width="680px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="Button_Approve" runat="server" OnClick="btnApprove_Click" CssClass="button"
                                Text="Approve" Width="71px" />&nbsp;
                            <asp:Button ID="Button_reset" runat="server" CssClass="button" Text="Reject" Width="71px"
                                OnClick="Button_reset_Click" />
                            <asp:Button ID="Button_Cancel" runat="server" Text="Cancel" CssClass="button" OnClick="Button_Cancel_Click" />
                            
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
