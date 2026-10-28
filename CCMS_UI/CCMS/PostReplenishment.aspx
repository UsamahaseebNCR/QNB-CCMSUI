<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PostReplenishment.aspx.cs"
    Inherits="CCMSUI.CCMS.PostReplenishment" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>CCMS - Post Replenishment</title>
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
    <link href="../content/iSuite.css" rel="stylesheet" />
    <script src="../content/DateTimePicker/jquery.min.js"></script>
    <link href="../content/DateTimePicker/jquery.datetimepicker.min.css" rel="stylesheet" />
    <script src="../content/DateTimePicker/jquery.datetimepicker.full.min.js"></script>


<script>
    $(function () {
        $(".datepicker").datetimepicker({
            format: 'd/m/Y H:i:s', // format for both date and time
            step: 15 // time step interval
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
                <%=Request.QueryString["replenishment_id"] == null?"Add New Replenishment":"Edit Replenishment"%>
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
                <table id="inner" border="0" cellpadding="0" cellspacing="0" style="padding-left: 10px; width:50%">
                    <tr>
                        <td>
                            ATM ID :
                        </td>
                        <td style="font-weight: bold">
                            <asp:Label ID="Label_SelectedATM" runat="server" Text="Please, select an ATM"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:Panel ID="Panel_Denom" runat="server" Visible="false">
                                <fieldset>
                                    <legend><b>Selected ATM Denominations</b></legend>
                                    <table width="100%" border="0">
                                        <tr>
                                            <td style="width: 75%">
                                                Denomination Value of Type 1:
                                            </td>
                                            <td>
                                                <asp:Label ID="Label_Denom1" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                Denomination Value of Type 2:
                                            </td>
                                            <td>
                                                <asp:Label ID="Label_Denom2" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                Denomination Value of Type 3:
                                            </td>
                                            <td>
                                                <asp:Label ID="Label_Denom3" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                Denomination Value of Type 4:
                                            </td>
                                            <td>
                                                <asp:Label ID="Label_Denom4" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </fieldset>
                            </asp:Panel>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 55%">
                            Replenishment Date :
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="TextBox_RepDate" CssClass="datepicker" Font-Names="Verdana"
                                Font-Size="10px" />
                            <asp:RegularExpressionValidator ID="ValFromDate" CssClass="Validator" runat="server"
                                ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d (0[0-9]|1[0-9]|2[0-4]):([0-5]\d):([0-5]\d)"
                                ControlToValidate="TextBox_RepDate" ErrorMessage="Replenishment DateTime is invalid.">*</asp:RegularExpressionValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Counters For Type 1 :
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_Cassette1" runat="server" CssClass="textbox"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="RegTextBox_Cassette1" runat="server" ControlToValidate="TextBox_Cassette1"
                                ValidationExpression="\d+"></asp:RegularExpressionValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Counters For Type 2 :
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_Cassette2" runat="server" CssClass="textbox"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="RegTextBox_Cassette2" runat="server" ControlToValidate="TextBox_Cassette2"
                                ValidationExpression="\d+"></asp:RegularExpressionValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Counters For Type 3 :
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_Cassette3" runat="server" CssClass="textbox"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="TextBox_Cassette3"
                                ValidationExpression="\d+"></asp:RegularExpressionValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Counters For Type 4 :
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_Cassette4" runat="server" CssClass="textbox"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="TextBox_Cassette4"
                                ValidationExpression="\d+"></asp:RegularExpressionValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Is Swap
                        </td>
                        <td>
                            <asp:CheckBox ID="CheckBox_IsSwap" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td valign="top">
                            Reason 
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_Reason" runat="server" TextMode="MultiLine" Height="80px" CssClass="textbox" Width="400px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" style="font-weight: bold; padding-top: 30px;">
                            <asp:Button ID="Button_SaveUpdateCashOrder" runat="server" Text="Save" OnClick="Button_SaveUpdate_Click"
                                CssClass="button" Width="60px" />&nbsp;
                            <input id="Button_Cancel" type="button" value="Cancel" onclick="javascript:history.go(-1);"
                                class="button" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <br />
    <br />
    <input type="button" runat="server" id="refresh" style="width: 0px; height: 0px;
        visibility: hidden" />
    </form>
</body>
</html>
