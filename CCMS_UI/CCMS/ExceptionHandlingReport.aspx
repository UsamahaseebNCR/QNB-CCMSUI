<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExceptionHandlingReport.aspx.cs"
    Inherits="CCMSUI.CCMS.ExceptionHandlingReport" Culture="fr-FR" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script src="../content/boxover.js" type="text/javascript" language="javascript">
    </script>
    <link href="../content/isuite.css" rel="stylesheet" />
    <link href="../content/DatePicker/jquery-ui.css" rel="stylesheet" />
    <script src="../content/DatePicker/jquery-3.7.1.js"></script>
    <script src="../content/DatePicker/jquery-ui.js"></script>

<script>
    $(function () {
        $(".datepicker").datepicker({
            dateFormat: "dd/mm/yy",
            showOn: "button",
            buttonImage: "../images/calendar.gif",
            buttonImageOnly: true,
            buttonText: "Select date",
            showButtonPanel: true
        });
    });
</script>
   
</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)'
    onload="hideProgressBar();">
    <form id="form1" runat="server">
    <div>
        <table style="width: 100%" cellspacing="0" cellpadding="0" border="0" class="mainbox">
            <tr>
                <td class="PageHeading" align="left">
                    Exception Handling Report
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
                                        <td>
                                            From :
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="TextBox_FromDate"  CssClass="datepicker" Font-Names="Verdana"
                                                Font-Size="10px" />
                                            <asp:RegularExpressionValidator ID="ValFromDate" CssClass="Validator" runat="server"
                                                ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                                ControlToValidate="TextBox_FromDate" ErrorMessage="From Date is invalid.">*</asp:RegularExpressionValidator>
                                        </td>
                                        <td>
                                            To :
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="TextBox_ToDate"  CssClass="datepicker" Font-Names="Verdana"
                                                Font-Size="10px" />
                                            <asp:RegularExpressionValidator ID="ValToDate" CssClass="Validator" runat="server"
                                                ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                                ControlToValidate="TextBox_ToDate" ErrorMessage="To Date is invalid.">*</asp:RegularExpressionValidator>
                                        </td>
                                        <td>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label_TransactionType" runat="server" Text="Transaction Type : "></asp:Label>
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="DropDownList_TransactionType" runat="server" AutoPostBack="True"
                                                CssClass="dropDown" Height="18px" Width="200px" OnSelectedIndexChanged="DropDownList_TransactionType_SelectedIndexChanged">
                                            </asp:DropDownList>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_ActionCode" runat="server" Text="Action Code:"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="DropDownList_ActionCode" runat="server"   CssClass="dropDown" Height="18px" Width="200px">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left">
                                            Type :
                                        </td>
                                        <td style="height: 20px">
                                            <%--                                                <asp:DropDownList ID="DropDownList_type" runat="server" CssClass="textboxNormal"
                                                    Width="280px" >
                                                    <asp:ListItem>Select</asp:ListItem>
                                                    <asp:ListItem>Expired</asp:ListItem>
                                                    <asp:ListItem>Non Expired</asp:ListItem>
                                                </asp:DropDownList>--%>
                                            <asp:CheckBox ID="isCheckExpired" runat="server" Text="Resolved" Checked="true" />
                                            <asp:CheckBox ID="isCheckNonExpired" runat="server" Text="Not Resolved" />
                                        </td>
                                        <td>
                                        </td>
                                        <td>
                                            <asp:CheckBoxList ID="CheckBoxList_Status" runat="server" RepeatDirection="Horizontal"
                                                ValidationGroup="check">
                                                <asp:ListItem Text="BNA Disputed" Value="0" Selected ="True"></asp:ListItem>
                                                <asp:ListItem Text="BNA Failed" Value="1"></asp:ListItem>
                                                <asp:ListItem Text="CPM Disputed" Value="2"></asp:ListItem>
                                                <asp:ListItem Text="CPM Failed" Value="3"></asp:ListItem>
                                                <asp:ListItem Text="Captured Card" Value="4"></asp:ListItem>
                                                <asp:ListItem Text="Normal Transactions" Value="5"></asp:ListItem>
                                            </asp:CheckBoxList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="6" align="right" style="padding-top: 20PX;">
                                            <asp:Button ID="btnSearch" runat="server" Width="130px" Text="Generate Report" CssClass="button"
                                                OnClick="btnSearch_Click"></asp:Button>&nbsp; &nbsp;
                                            <input id="ButtonCancel" class="button" onclick="window.location='reportList.aspx'"
                                                style="width: 52px" type="button" value="Close" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td>
                                <table>
                                    <tr>
                                        <td>
                                            <img src="../images/Icon_search.png" alt="" />
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
