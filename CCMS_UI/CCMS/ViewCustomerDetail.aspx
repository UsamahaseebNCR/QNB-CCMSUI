<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewCustomerDetail.aspx.cs"
    Inherits="CCMSUI.CCMS.ViewCustomerDetail" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>CCMS-View Transaction Chunk</title>
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
    <link href="../content/iSuite.css" rel="stylesheet" />
    <script type="text/javascript">
        //window.parent.window.SetContentPage( window.location);
    </script>
</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)'
    onload="hideProgressBar();">
    <form id="form1" runat="server">
    <div>
        <table style="width: 100%" cellspacing="0" cellpadding="0" border="0" class="mainbox">
            <tr>
                <td class="PageHeading" align="left">
                    Customer Detail
                </td>
            </tr>
            <tr>
                <td class="ValidationText" align="center">
                    <asp:Literal ID="Literal_error" runat="server" EnableViewState="False"></asp:Literal>
                </td>
            </tr>
            <tr>
                <td class="ConfirmationText" align="center">
                    <asp:ValidationSummary ID="valSummary" runat="server" />
                    <%=Session[SessionVars.notice.ToString()]%>
                    <%Session[SessionVars.notice.ToString()] = "";%>
                    <asp:Literal ID="Literal_ok" runat="server" EnableViewState="False"></asp:Literal>
                </td>
            </tr>
            <tr>
                <td>
                    <div>
                        <table width="100%" id="chunk" runat="server">
                            <tr>
                                <td style="background-color: #DADADA">
                                    <fieldset>
                                        <legend><b>Customer Detail</b></legend>
                                        <table>
                                            <tr>
                                                <td>
                                                    <div style="width: 260px">
                                                        <asp:Literal ID="Literal_EJ" runat="server"></asp:Literal>
                                                    </div>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    Customer ID:
                                                </td>
                                                <td>
                                                    <asp:Label ID="Label_CustomerID" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    Customer Name:
                                                </td>
                                                <td>
                                                    <asp:Label ID="Label_CustomerName" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    Customer Telephone:
                                                </td>
                                                <td>
                                                    <asp:Label ID="Label_CustomerTelephone" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    Mobile No:
                                                </td>
                                                <td>
                                                    <asp:Label ID="Label_MobileNo" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    Customer Email:
                                                </td>
                                                <td>
                                                    <asp:Label ID="Label_CustomerEmail" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    Customer Category:
                                                </td>
                                                <td>
                                                    <asp:Label ID="Label_CustomerCategory" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                   Available Balance:
                                                </td>
                                                <td>
                                                    <asp:Label ID="Label_AvailableBalance" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                             <tr>
                                                <td>
                                                   RM Name:
                                                </td>
                                                <td>
                                                    <asp:Label ID="Label_RMName" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                              <tr>
                                                <td>
                                                   RM Contact:
                                                </td>
                                                <td>
                                                    <asp:Label ID="Label_RMContact" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                              <tr>
                                                <td>
                                                   RM Email:
                                                </td>
                                                <td>
                                                    <asp:Label ID="Label_RMEmail" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    Reference No
                                                </td>
                                                <td>
                                                    <asp:Label ID="Label_refNo" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                           
                                        </table>
                                    </fieldset>
                                </td>
                            </tr>
                        </table>
                        <table width="100%" id="completeFile" runat="server">
                            <tr>
                            </tr>
                        </table>
                    </div>
                </td>
            </tr>
            <tr>
                <td colspan="2" align="center">
                    <input type="button" value="Close" class="button" onclick="window.close();" />
                    <input type="button" value="Print" class="button" onclick="window.print();" />
                </td>
            </tr>
        </table>
    </div>
    <input type="button" runat="server" id="refresh" style="display: none" />
    </form>
</body>
</html>
