<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditATMSettlement.aspx.cs" Inherits="CCMSUI.admin.EditATMSettlement" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Edit ATM Settlement</title>
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
       <link href="../content/Scripts/ui-lightness/jquery.ui.all.css" rel="stylesheet" type="text/css" />
    <link href="../content/Scripts/ui-lightness/jquery-ui-1.8.17.custom.css" rel="stylesheet"
        type="text/css" />
    <link href="../content/Scripts/ui-lightness/ui-time-picker.css" rel="stylesheet"
        type="text/css" />
    <script type="text/javascript" src="../content/js/jquery.js"></script>
    <script type="text/javascript" src="../content/Scripts/jquery.ui.core.js"></script>
    <script src="../content/js/jquery.js" type="text/javascript"></script>
    <script src="../content/Scripts/jquery-ui-1.8.16.min.js" type="text/javascript"></script>
    <script type="text/javascript" src="../content/Scripts/jquery.ui.widget.js"></script>
    <script type="text/javascript" src="../content/Scripts/jquery.ui.datepicker.js"></script>
    <script type="text/javascript" src="../content/Scripts/jquery-ui-timepicker-addon.js"></script>
    <script type="text/javascript">
        $(function () {

            $('.datepicker').datetimepicker({ dateFormat: 'dd/mm/yy',

                dateFormat: 'dd/mm/yy',
                timeFormat: 'hh:mm:ss',
                defaultDate: -1,
                minDate: '-2m',
                maxDate: '+2d',
                hourGrid: 4,
                minuteGrid: 10,

                showOn: "button",
                buttonImage: "../images/calendar.gif",
                buttonImageOnly: true

            });
        });
    
    </script>

    <script type="text/javascript">
        window.parent.window.contentPage = '';                  
    </script>
    <link href="../content/iSuite.css" rel="stylesheet" />
</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)'
    onload="hideProgressBar();">
    <form id="form1" runat="server">
    <table cellpadding="0" cellspacing="0" border="0" class="MainBox" width="100%">
        <tr>
            <td class="PageHeading" align="left" colspan="2" style="height: 25px">
                Edit ATM Settlements
            </td>
        </tr>
        <tr>
            <td class="SearchHeadPaneGradient" colspan="2">
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:ValidationSummary ID="ValidationSummary1" runat="server"></asp:ValidationSummary>
            </td>
        </tr>
        <tr style="height: 20px">
            <td colspan="2" style="color: red;" align="center" valign="top">
                <asp:Literal ID="Literal_error" runat="server" EnableViewState="False"></asp:Literal>
            </td>
        </tr>
        <tr>
            <td>
                <table id="inner" border="0" cellpadding="0" cellspacing="0" width="100%" style="padding-left: 10px">
                    <tr>
                        <td>
                            <b>Replenishment Counters</b>
                        </td>
                    </tr>
                    <tr style="background-color: #eff0ef;">
                        <td style="width: 15%">
                            Denomination Type 1 <span style="color: Red">*</span>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_DenomType1" runat="server" CssClass="textbox"></asp:TextBox>
                        </td>
                        <td>
                            Number Of Notes
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_NoOfNotes1" runat="server" CssClass="textbox"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 15%">
                            Denomination Type 2 <span style="color: Red">*</span>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_DenomType2" runat="server" CssClass="textbox"></asp:TextBox>
                        </td>
                        <td>
                            Number Of Notes
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_NoOfNotes2" runat="server" CssClass="textbox"></asp:TextBox>
                        </td>
                    </tr>
                    <tr style="background-color: #eff0ef;">
                        <td style="width: 15%">
                            Denomination Type 3 <span style="color: Red">*</span>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_DenomType3" runat="server" CssClass="textbox"></asp:TextBox>
                        </td>
                        <td>
                            Number Of Notes
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_NoOfNotes3" runat="server" CssClass="textbox"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 15%">
                            Denomination Type 4 <span style="color: Red">*</span>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_DenomType4" runat="server" CssClass="textbox"></asp:TextBox>
                        </td>
                        <td>
                            Number Of Notes
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_NoOfNotes4" runat="server" CssClass="textbox"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 15%">
                            Replenisment Datetime <span style="color: Red">*</span>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_RepDateTime" runat="server" CssClass="datepicker"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                    <td>
                        <b>Rejected Denominations</b>
                    </td>
                    </tr>
                     <tr style="background-color: #eff0ef;">
                        <td style="width: 15%">
                            Denomination Type 1 <span style="color: Red">*</span>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_RejDenom1" runat="server" CssClass="textbox"></asp:TextBox>
                        </td>
                        <td>
                            Number Of Notes
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_RejNoOfNotes1" runat="server" CssClass="textbox"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 15%">
                            Denomination Type 2 <span style="color: Red">*</span>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_RejDenom2" runat="server" CssClass="textbox"></asp:TextBox>
                        </td>
                        <td>
                            Number Of Notes
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_RejNoOfNotes2" runat="server" CssClass="textbox"></asp:TextBox>
                        </td>
                    </tr>
                    <tr style="background-color: #eff0ef;">
                        <td style="width: 15%">
                            Denomination Type 3 <span style="color: Red">*</span>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_RejDenom3" runat="server" CssClass="textbox"></asp:TextBox>
                        </td>
                        <td>
                            Number Of Notes
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_RejNoOfNotes3" runat="server" CssClass="textbox"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 15%">
                            Denomination Type 4 <span style="color: Red">*</span>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_RejDenom4" runat="server" CssClass="textbox"></asp:TextBox>
                        </td>
                        <td>
                            Number Of Notes
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_RejNoOfNotes4" runat="server" CssClass="textbox"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 15%">
                           Previous Replenisment Datetime <span style="color: Red">*</span>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_PreviousRepDate" runat="server" CssClass="datepicker"></asp:TextBox>
                        </td>
                    </tr>
                   
                    <tr>
                        <td colspan="2" style="font-weight: bold; padding-top: 30px;">
                            <asp:Button ID="Button_UpdateATMSettlement" runat="server" Text="Save" OnClick="Button_UpdateATMSettlement_Click"
                                CssClass="button" Width="60px" />&nbsp;
                            <input id="Button_Cancel" type="button" value="Cancel" onclick="window.location ='../ccms/ATMSettlementList.aspx'"
                                class="button" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <br />
    <br />
    </form>
</body>
</html>
