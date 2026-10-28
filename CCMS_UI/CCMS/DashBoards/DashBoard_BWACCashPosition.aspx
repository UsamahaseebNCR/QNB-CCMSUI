<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DashBoard_BWACCashPosition.aspx.cs" Inherits="CCMSUI.CCMS.DashBoards.DashBoard_BWACCashPosition" Culture="fr-FR" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>CCMS BWAC Cash Position</title>
    <link href="../../content/isuite.css" rel="stylesheet" />
    <link href="../../content/cal/popcalendar.css" type="text/css" rel="stylesheet" />
    <script src="../../content/shortcut.js" type="text/javascript" language="javascript"></script>
    <script language="javascript" src="../../content/cal/popcalendar.js" type="text/javascript"></script>
</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)'
    onload="hideProgressBar();">
    <form id="form1" runat="server">
    <div>
        <table style="width: 100%" cellspacing="0" cellpadding="0" border="0" class="mainbox">
            <tr>
                <td class="PageHeading" align="left">
                    <asp:Label ID="Label_Title" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Literal ID="Literal_Scr" runat="server" EnableViewState="false"></asp:Literal>
                </td>
            </tr>
            <tr>
                <td class="ConfirmationText" align="center">
                    <asp:ValidationSummary ID="valSummary" runat="server" />
                    <asp:Literal ID="Literal_ok" runat="server" EnableViewState="False"></asp:Literal>
                </td>
            </tr>
            <tr align="left" style="background-color: WHITE">
                <td style="height: 20px; padding-left: 5px; width: 935px;" colspan="2">
                    <b>Search Filters</b>
                </td>
            </tr>
            <tr align="left">
                <td colspan="2" style="width: 100%">
                    <table width="100%" class="SEARCHBOX" border="0">
                        <tr>
                            <td>
                                <table id="search" width="100%" border="0">
                                    <tr>
                                        <td align="left" width="20%">
                                            Group :
                                        </td>
                                        <td style="height: 20px">
                                            <asp:DropDownList ID="DropDownList_NoteSetType" runat="server" CssClass="dropDown">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr id="trMinThreshold" runat="server" visible="false">
                                        <td align="left">
                                            Min Notes Threshold Reached :
                                        </td>
                                        <td style="height: 20px">
                                            <asp:CheckBox ID="CheckBox_ThresholdReached" runat="server" />
                                        </td>
                                    </tr>
                                    <tr id="trTrxnStatus" runat="server" visible="false">
                                        <td align="left">
                                            Status :
                                        </td>
                                        <td style="height: 20px">
                                            <asp:CheckBoxList ID="CheckBoxList_Status" runat="server" RepeatDirection="Horizontal">
                                                
                                            </asp:CheckBoxList>
                                        </td>
                                    </tr>
                                    <tr style="padding-top: 5px">
                                    </tr>
                                    <tr>
                                        <td>
                                        </td>
                                        <td>
                                            <asp:Button ID="btnGenerate" runat="server" Width="130px" Text="Generate Dashboard"
                                                CssClass="button" OnClick="btnSearch_Click"></asp:Button>&nbsp;
                                            <input id="ButtonCancel" class="button" onclick="window.location='../../admin/operations.aspx'"
                                                style="width: 52px" type="button" value="Close" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td align="right">
                                <table>
                                    <tr>
                                        <td>
                                            <img src="../../images/Icon_search.png" alt="" />
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
    <input type="button" runat="server" id="refresh" style="display: none" />
    </form>
</body>
</html>
