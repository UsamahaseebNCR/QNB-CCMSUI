<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RegionWiseComparison.aspx.cs"
    Inherits="CCMSUI.RegionWiseComparison" %>

<%@ Register TagPrefix="ofc" Namespace="OpenFlashChart" Assembly="OpenFlashChart" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
      <script src="content/shortcut.js" type="text/javascript" language="javascript">
    </script>
</head>
<body onload="hideProgressBar();" >
    <form id="form1" runat="server">
    <div>
        <table width="100%">
            <tr>
                <td>
                    <table style="background-color: InfoBackground; width: 100%">
                        <tr>
                            <td style="width: 23px">
                                From
                            </td>
                            <td style="width: 23px">
                                <asp:TextBox ID="TextBox_From" runat="server"></asp:TextBox>
                            </td>
                            <td style="width: 23px">
                                        <img src="images/IconCalendar.gif" alt="Click to open calender" onclick="javascript:show_calendar('form1.TextBox_From');"
                                            style="border: 0; cursor: hand" id="IMG6" />
                                    </td>
                                    <td style="width: 3px">
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator5" CssClass="Validator"
                                            runat="server" ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                            ControlToValidate="TextBox_From" ErrorMessage="To Date is invalid.">*</asp:RegularExpressionValidator>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="TextBox_From"
                                        ErrorMessage="To date cannot be empty">*</asp:RequiredFieldValidator>
                                    </td>
                            <td style="width: 23px">
                                To
                            </td>
                            <td style="width: 23px">
                                <asp:TextBox ID="TextBox_To" runat="server"></asp:TextBox>
                            </td>
                            <td style="width: 23px">
                                        <img src="images/IconCalendar.gif" alt="Click to open calender" onclick="javascript:show_calendar('form1.TextBox_To');"
                                            style="border: 0; cursor: hand" id="IMG1" />
                                    </td>
                                    <td style="width: 3px">
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" CssClass="Validator"
                                            runat="server" ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                            ControlToValidate="TextBox_To" ErrorMessage="To Date is invalid.">*</asp:RegularExpressionValidator>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="TextBox_To"
                                        ErrorMessage="To date cannot be empty">*</asp:RequiredFieldValidator>
                                    </td>
                            <td align="left">
                                <asp:Button ID="Button_Submit" runat="server" Text="Generate" OnClick="Button_Submit_Click" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <ofc:Chart ID="Chart1" Height="230" Width="1130px" runat="server" 
                        onload="Chart1_Load" />
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
