<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PNCMeanTimeToFailureAnalysis.aspx.cs"
    Inherits="CCMSUI.CCMS.PNCMeanTimeToFailureAnalysis" Culture="fr-FR" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>CCMS - Low Balance Report</title>
    <link href="../content/isuite.css" rel="stylesheet" />
    <link href="../content/cal/popcalendar.css" type="text/css" rel="stylesheet" />
    <script src="../content/shortcut.js" type="text/javascript" language="javascript"></script>
    <script language="javascript" src="../content/cal/popcalendar.js" type="text/javascript"></script>
</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)'
    onload="hideProgressBar();">
    <form id="form1" runat="server">
    <div>
        <table style="width: 100%" cellspacing="0" cellpadding="0" border="0" class="mainbox">
            <tr>
                <td class="PageHeading" align="left">
                    PNC Mean Time To Failure Analysis
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
                                        <td style="width: 99px;">
                                            From :
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="TextBox_FromDate" CssClass="textboxNormal" />
                                            <img src="../images/IconCalendar.gif" alt="Click to open calender" onclick="popUpCalendar(this,document.getElementById('<%=TextBox_FromDate.ClientID %>'),'dd/mm/yyyy');"
                                                style="border: 0; cursor: hand" id="IMG1" />
                                            <asp:RegularExpressionValidator ID="ValFromDate" CssClass="Validator" runat="server"
                                                ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                                ControlToValidate="TextBox_FromDate" ErrorMessage="From Date is invalid.">*</asp:RegularExpressionValidator>
                                        </td>
                                        <td style="width: 99px; height: 17px">
                                            To :
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="TextBox_ToDate" CssClass="textboxNormal" />
                                            <img src="../images/IconCalendar.gif" alt="Click to open calender" onclick="popUpCalendar(this,document.getElementById('<%=TextBox_ToDate.ClientID %>'),'dd/mm/yyyy');"
                                                style="border: 0; cursor: hand" id="IMG2" />
                                            <asp:RegularExpressionValidator ID="ValToDate" CssClass="Validator" runat="server"
                                                ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                                ControlToValidate="TextBox_ToDate" ErrorMessage="To Date is invalid.">*</asp:RegularExpressionValidator>
                                        </td>
                                    </tr>
                                      <tr>
                                        <td class="style4">
                                            PAN :
                                        </td>
                                        <td style="width: 190px; height: 17px">
                                            <asp:TextBox runat="server" ID="TextBox_PAN" Width="119px" CssClass="textbox" />
                                        </td>
                                        <td class="style1">
                                            TSN :
                                        </td>
                                        <td style="height: 17px; width: 326px;">
                                            <asp:TextBox runat="server" ID="TextBox_TSN" Width="119px" CssClass="textbox" />
                                            <asp:RegularExpressionValidator ID="RegularExpressionValidator2" CssClass="Validator"
                                                runat="server" ValidationExpression="\d*" ControlToValidate="TextBox_TSN" ErrorMessage="TSN is invalid.">*</asp:RegularExpressionValidator>
                                        </td>
                                    </tr>
                                 <tr>
                                        <td class="style4">
                                            Amount From :
                                        </td>
                                        <td style="width: 190px; height: 17px">
                                            <asp:TextBox runat="server" ID="TextBox_FromAmount" Width="119px" CssClass="textbox" />
                                            <asp:RegularExpressionValidator ID="RegularExpressionValidator3" CssClass="Validator"
                                                runat="server" ValidationExpression="\d*" ControlToValidate="TextBox_FromAmount"
                                                ErrorMessage="Purge From is invalid.">*</asp:RegularExpressionValidator>
                                        </td>
                                        <td class="style1">
                                            Amount To :
                                        </td>
                                        <td style="height: 17px; width: 326px;">
                                            <asp:TextBox runat="server" ID="TextBox_ToAmount" Width="119px" CssClass="textbox" />
                                            <asp:RegularExpressionValidator ID="RegularExpressionValidator4" CssClass="Validator"
                                                runat="server" ValidationExpression="\d*" ControlToValidate="TextBox_ToAmount"
                                                ErrorMessage="Purge To is invalid.">*</asp:RegularExpressionValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Analysis On
                                        </td>
                                        <td>
                                            <asp:CheckBoxList ID="CheckBoxList_DisputeStatus" runat="server" CssClass="dropDown" 
                                                RepeatDirection="Horizontal">
                                                <asp:ListItem Value="0" Selected="True">Business Rule</asp:ListItem>
                                                <asp:ListItem Value="1" Selected="True">Non Business Rule</asp:ListItem>
                                            </asp:CheckBoxList>
                                        </td>
                                        
                                    </tr>
                                    
                                    <tr>
                                        <td valign="top">
                                            Module:
                                        </td>
                                        <td>
                                            <asp:RadioButtonList ID="RadioButtonList_Module" runat="server">
                                                <asp:ListItem Selected="True">SDM CPM</asp:ListItem>
                                                <asp:ListItem>SDM BNA</asp:ListItem>                                               
                                                <asp:ListItem>BNA</asp:ListItem>
                                            </asp:RadioButtonList>
                                        </td>
                                    </tr>
                                   
                                    <tr>
                                        <td>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            &nbsp;
                                        </td>
                                        <td colspan="5" align="left">
                                            <asp:Button ID="btnSearch" runat="server" Width="130px" Text="Generate Report" CssClass="button"
                                                OnClick="btnSearch_Click"></asp:Button>&nbsp;
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
    <input type="button" runat="server" id="refresh" style="display: none" />
    </form>
</body>
</html>
