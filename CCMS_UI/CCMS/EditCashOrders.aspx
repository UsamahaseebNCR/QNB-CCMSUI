<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditCashOrders.aspx.cs"
    Inherits="CCMSUI.CCMS.EditCashOrders" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Add Edit Cash Orders</title>
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
    <script type="text/javascript">
        window.parent.window.contentPage = '';          
        
        
    </script>
    <link href="../content/iSuite.css" rel="stylesheet" />
       <script type="text/javascript" src="../content/Scripts/jquery.ui.widget.js"></script>
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
</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)'
    onload="hideProgressBar();">
    <form id="form1" runat="server">
    <table cellpadding="0" cellspacing="0" border="0" class="MainBox" width="100%">
        <tr>
            <td class="PageHeading" align="left" colspan="2" style="height: 25px">
                <%=Request.QueryString["cash_order_id"] == null?"Add New Cash Order":"Edit Cash Order"%>
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
                <table id="inner" border="0" cellpadding="0" cellspacing="0" style="padding-left: 10px">
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
                                        <tr style="visibility: hidden">
                                            <td>
                                                Denomination Value of Type 5:
                                            </td>
                                            <td>
                                                <asp:Label ID="Label_Denom5" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr style="visibility: hidden">
                                            <td>
                                                Denomination Value of Type 6:
                                            </td>
                                            <td>
                                                <asp:Label ID="Label_Denom6" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr style="visibility: hidden">
                                            <td>
                                                Denomination Value of Type 7:
                                            </td>
                                            <td>
                                                <asp:Label ID="Label_Denom7" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </fieldset>
                            </asp:Panel>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 120px; height: 17px">
                            Cash Order Date
                        </td>
                        <td style="width: 190px; height: 17px">
                            <asp:TextBox runat="server" ID="TextBox_CashOrderDate" CssClass="datepicker" />

                                 <asp:RequiredFieldValidator ID="RequiredFieldValidator_CashOrderDate" runat="server"
                             ErrorMessage="Cash order date is invalid" ControlToValidate="TextBox_CashOrderDate" >*</asp:RequiredFieldValidator>
                       
                            <%--<img src="../images/IconCalendar.gif" alt="Click to open calender" onclick="popUpCalendar(this,document.getElementById('<%=TextBox_FromDate.ClientID %>'),'dd/mm/yyyy');"
                                                style="border: 0; cursor: hand" id="IMG1" />--%>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator6" CssClass="Validator" runat="server"
                                ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d (0[0-9]|1[0-9]|2[0-4]):([0-5]\d):([0-5]\d)"
                                ControlToValidate="TextBox_CashOrderDate" ErrorMessage="Creation Time From Date is invalid.">*</asp:RegularExpressionValidator>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 120px; height: 17px">
                            Replenishment Date
                        </td>
                        <td style="width: 190px; height: 17px">
                            <asp:TextBox runat="server" ID="TextBox_RepDate" CssClass="datepicker" />
                            <%--<img src="../images/IconCalendar.gif" alt="Click to open calender" onclick="popUpCalendar(this,document.getElementById('<%=TextBox_FromDate.ClientID %>'),'dd/mm/yyyy');"
                                                style="border: 0; cursor: hand" id="IMG1" />--%>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator_RepDate" runat="server"
                             ErrorMessage="Replenishment date is invalid" ControlToValidate="TextBox_RepDate" >*</asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="ValFromDate" CssClass="Validator" runat="server"
                                ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d (0[0-9]|1[0-9]|2[0-4]):([0-5]\d):([0-5]\d)"
                                ControlToValidate="TextBox_RepDate" ErrorMessage="Creation Time From Date is invalid.">*</asp:RegularExpressionValidator>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 55%">
                            Replenishment Type :
                        </td>
                        <td>
                            <asp:DropDownList ID="DropDownList_RepType" runat="server" CssClass="dropDown">
                                <asp:ListItem Text="RS"></asp:ListItem>
                                <asp:ListItem Text="UR" Selected="True"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Hold Cash Order
                        </td>
                        <td>
                            <asp:CheckBox ID="CheckBox_HoldCashOrder" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Suggested Notes For Type 1 :
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_Cassette1" runat="server" CssClass="textbox"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="RegTextBox_Cassette1" runat="server" ControlToValidate="TextBox_Cassette1"
                                ValidationExpression="\d+"></asp:RegularExpressionValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Suggested Notes For Type 2 :
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_Cassette2" runat="server" CssClass="textbox"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="RegTextBox_Cassette2" runat="server" ControlToValidate="TextBox_Cassette2"
                                ValidationExpression="\d+"></asp:RegularExpressionValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Suggested Notes For Type 3 :
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_Cassette3" runat="server" CssClass="textbox"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="TextBox_Cassette3"
                                ValidationExpression="\d+"></asp:RegularExpressionValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Suggested Notes For Type 4 :
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_Cassette4" runat="server" CssClass="textbox"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="TextBox_Cassette4"
                                ValidationExpression="\d+"></asp:RegularExpressionValidator>
                        </td>
                    </tr>
                    <tr style="visibility: hidden">
                        <td>
                            Suggested Notes For Type 5 :
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_Cassette5" runat="server" CssClass="textbox"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" ControlToValidate="TextBox_Cassette5"
                                ValidationExpression="\d+"></asp:RegularExpressionValidator>
                        </td>
                    </tr>
                    <tr style="visibility: hidden">
                        <td>
                            Suggested Notes For Type 6 :
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_Cassette6" runat="server" CssClass="textbox"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator4" runat="server" ControlToValidate="TextBox_Cassette6"
                                ValidationExpression="\d+"></asp:RegularExpressionValidator>
                        </td>
                    </tr>
                    <tr style="visibility: hidden">
                        <td>
                            Suggested Notes For Type 7 :
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_Cassette7" runat="server" CssClass="textbox"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator5" runat="server" ControlToValidate="TextBox_Cassette7"
                                ValidationExpression="\d+"></asp:RegularExpressionValidator>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" style="font-weight: bold; padding-top: 30px;">
                            <asp:Button ID="Button_SaveUpdateCashOrder" runat="server" Text="Save" OnClick="Button_SaveUpdateCashOrder_Click"
                                CssClass="button" Width="60px" />&nbsp;
                            <input id="Button_Cancel" type="button" value="Cancel" onclick="window.location ='cashorders.aspx'"
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
