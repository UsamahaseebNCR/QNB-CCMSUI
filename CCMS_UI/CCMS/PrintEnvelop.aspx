<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PrintEnvelop.aspx.cs" Inherits="CCMSUI.CCMS.PrintEnvelop" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body >
    <form id="form1" runat="server">
    <div>
        <table border="0" style="font-family: Comic Sans MS" width="70%" align="center">
            <tr>
                <td valign="top">
                    <table border="0" cellpadding="2" cellspacing="2">
                        <tr>
                            <td>
                                <b>ATM ID:</b>
                            </td>
                            <td style="border-style: solid;">
                                <asp:Label ID="Label_ATMID" runat="server" Text="0251" Font-Bold="true"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                ATM Open Date
                            </td>
                            <td style="border-style: solid;">
                                <asp:Label ID="Label_ATMOpenDate" runat="server" ></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                ATM Open Time
                            </td>
                            <td style="border-style: solid;">
                                <asp:Label ID="Label_ATMOpenTime" runat="server" ></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                            </td>
                        </tr>
                        <tr>
                            <td>
                            </td>
                        </tr>
                        <tr>
                            <td>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" align="right">
                                <b>Figures Found During The Replenishment</b>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <table>
                                    <tr>
                                        <td>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Loaded
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Left
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Rejected
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Returned
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td>
                            <br />
                                <table cellpadding="2" cellspacing="0" border="1" style="text-align: center">
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label_CITAdded1" runat="server" Text="0"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_CITAdded2" runat="server" Text="0"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_CITAdded3" runat="server" Text="0"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_CITAdded4" runat="server" Text="0"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label_CITLeft1" runat="server" Text="0"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_CITLeft2" runat="server" Text="0"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_CITLeft3" runat="server" Text="0"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_CITLeft4" runat="server" Text="0"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label_CITRejected1" runat="server" Text="0"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_CITRejected2" runat="server" Text="0"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_CITRejected3" runat="server" Text="0"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_CITRejected4" runat="server" Text="0"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label_Returned1" runat="server" Text="0"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_Returned2" runat="server" Text="0"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_Returned3" runat="server" Text="0"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_Returned4" runat="server" Text="0"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="4">
                                            <b>
                                            <asp:Label ID="Label_CITTotalLeftAmount" runat="server" Text="0"></asp:Label>
                                            
                                            </b>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                &nbsp
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" align="right">
                                <b>Different between physical and printed figures</b>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <table>
                                    <tr>
                                        <td>
                                            Difference
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td>
                                <table cellpadding="2" cellspacing="0" border="1" style="text-align: center">
                                    <tr>
                                        <td> 
                                           <asp:Label ID="Label_NoteSetType1" runat="server"></asp:Label> L.E
                                        </td>
                                        <td>
                                           <asp:Label ID="Label_NoteSetType2" runat="server"></asp:Label> L.E
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_NoteSetType3" runat="server"></asp:Label> L.E
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_NoteSetType4" runat="server"></asp:Label> L.E
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label_DiffType1" runat="server" Text="0"></asp:Label>
                                                                                   </td>
                                        <td>
                                           <asp:Label ID="Label_DiffType2" runat="server" Text="0"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_DiffType3" runat="server" Text="0"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_DiffType4" runat="server" Text="0"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </td>
                <td>
                    <table cellpadding="2" cellspacing="2">
                        <tr>
                            <td>
                                <b>ATM Location:</b>
                            </td>
                            <td style="border-style: solid">
                                <asp:Label ID="Label_ATMLocation" runat="server" Text="Faisal Branch" Font-Bold="true"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                ATM Closed Date
                            </td>
                            <td style="border-style: solid">
                                <asp:Label ID="Label_ATMClosedDate" runat="server" Text="05-May-13"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                ATM Closed Time
                            </td>
                            <td style="border-style: solid">
                                <asp:Label ID="Label_ATMClosedTime" runat="server" Text="13:46"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                            </td>
                        </tr>
                        <tr>
                            <td>
                            </td>
                        </tr>
                        <tr>
                            <td>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" align="right">
                                <b>Figures as printed in the CCMS</b>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <table>
                                    <tr>
                                        <td>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Total
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Left
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Dispensed
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Rejected
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Retract
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            adjusted
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Returned
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td>
                                <table cellpadding="2" cellspacing="0" border="1" style="text-align: center">
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label_DiffNoteSetType1" runat="server"></asp:Label> L.E
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_DiffNoteSetType2" runat="server"></asp:Label> L.E
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_DiffNoteSetType3" runat="server"></asp:Label> L.E
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_DiffNoteSetType4" runat="server"></asp:Label> L.E
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label_CCMSAdded1" runat="server" Text="0"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_CCMSAdded2" runat="server" Text="0"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_CCMSAdded3" runat="server" Text="0"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_CCMSAdded4" runat="server" Text="0"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label_CCMSLeft1" runat="server" Text="0"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_CCMSLeft2" runat="server" Text="0"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_CCMSLeft3" runat="server" Text="0"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_CCMSLeft4" runat="server" Text="0"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label_CCMSDispensed1" runat="server" Text="0"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_CCMSDispensed2" runat="server" Text="0"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_CCMSDispensed3" runat="server" Text="0"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_CCMSDispensed4" runat="server" Text="0"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label_CCMSRejected1" runat="server" Text="0"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_CCMSRejected2" runat="server" Text="0"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_CCMSRejected3" runat="server" Text="0"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_CCMSRejected4" runat="server" Text="0"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            0
                                        </td>
                                        <td>
                                            0
                                        </td>
                                        <td>
                                            0
                                        </td>
                                        <td>
                                            0
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            0
                                        </td>
                                        <td>
                                            0
                                        </td>
                                        <td>
                                            0
                                        </td>
                                        <td>
                                            0
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label_CCMSReturned1" runat="server" Text="0"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_CCMSReturned2" runat="server" Text="0"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_CCMSReturned3" runat="server" Text="0"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_CCMSReturned4" runat="server" Text="0"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="4">
                                           
                                            <b>
                                            <asp:Label ID="Label_CCMSTotalLeftAmount" runat="server" Text="0"></asp:Label>
                                            </b>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Total Difference
                            </td>
                            <td style="border-style: solid; text-align: center">
                              <asp:Label ID="Label_DiffTotalAmount" runat="server" Text="0"></asp:Label>  
                            </td>
                        </tr>
                        <tr>
                            <td>
                            </td>
                        </tr>
                        <tr>
                            <td>
                            </td>
                        </tr>
                        <tr>
                            <td>
                            </td>
                            <td>
                                <table cellpadding="2" cellspacing="0" border="1" style="text-align: center" width="100%">
                                    <tr>
                                        <td colspan="4">
                                            <b>New Loading</b>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <b>
                                                <asp:Label ID="Label_notesAdded1" runat="server" Text="0"></asp:Label>
                                            </b>
                                        </td>
                                        <td>
                                            <b>
                                                <asp:Label ID="Label_notesAdded2" runat="server" Text="0"></asp:Label></b>
                                        </td>
                                        <td>
                                            <b>
                                                <asp:Label ID="Label_notesAdded3" runat="server" Text="0"></asp:Label></b>
                                        </td>
                                        <td>
                                            <b>
                                                <asp:Label ID="Label_notesAdded4" runat="server" Text="0"></asp:Label></b>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="4">
                                            <b>
                                                <asp:Label ID="Label_TotalAdded" runat="server"></asp:Label></b>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
