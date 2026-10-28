<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewTransactionChunk.aspx.cs"
    Inherits="CCMSUI.CCMS.ViewTransactionChunk" %>

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
    <style type="text/css">
        .style1
        {
            height: 17px;
            width: 130px;
        }
        .style3
        {
            width: 141px;
        }
        .style4
        {
            height: 17px;
            width: 141px;
        }
    </style>
</head>

    <form id="form1" runat="server">
    <div>
        <table style="width: 100%" cellspacing="0" cellpadding="0" border="0" class="mainbox">
            <tr>
                <td class="PageHeading" align="left">
                    View Transaction
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
                                        <legend><b>Transaction Recorded by EJ</b></legend>
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
                                                    Datetime
                                                </td>
                                                <td>
                                                    <asp:Label ID="Label_Datetime" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    PAN
                                                </td>
                                                <td>
                                                    <asp:Label ID="Label_PAN" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    TSN
                                                </td>
                                                <td>
                                                    <asp:Label ID="Label_TSN" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    Host TSN
                                                </td>
                                                <td>
                                                    <asp:Label ID="Label_HostTSN" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    Amount
                                                </td>
                                                <td>
                                                    <asp:Label ID="Label_Amount" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    Status
                                                </td>
                                                <td>
                                                    <asp:Label ID="Label_Status" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                    </fieldset>
                                </td>
                            </tr>
                        </table>
                        <table width="100%" id="completeFile" runat="server">
                            <tr>
                                <td style="background-color: #DADADA">
                                    <fieldset>
                                        <legend><b>EJ File</b></legend>
                                        <table>
                                            <tr>
                                                <td>
                                                    <div style="width: 260px">
                                                        <asp:Literal ID="Literal_CompleteEJFile" runat="server"></asp:Literal>
                                                    </div>
                                                </td>
                                            </tr>
                                        </table>
                                    </fieldset>
                                </td>
                            </tr>
                        </table>
                    </div>
                </td>
            </tr>
            <tr>
                <td colspan="2" align="center">
                    <input type="button" value="Close" class="button" onclick="window.close();" />
                    <input type="button" value="Print" class="button" onclick="window.print();" />
                    <asp:Button ID="Button_ViewEJ" runat="server" Text="View Complete EJ File" CssClass="button"
                        OnClick="Button_ViewEJ_Click" />
                    <asp:Button ID="Button_ViewChunk" runat="server" Text="View Chunk" CssClass="button"
                        OnClick="Button_ViewChunk_Click" />
                </td>
            </tr>
        </table>
    </div>
    <input type="button" runat="server" id="refresh" style="display: none" />
    </form>
</body>
</html>
