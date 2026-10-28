<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FTPFileMonitoringReport.aspx.cs" Inherits="CCMSUI.CCMS.FTPFileMonitoringReport" Culture="fr-FR" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Order Download and Daily Feed Status Report</title>
    <link href="../content/iSuite.css" rel="stylesheet" />
    <link href="../content/cal/popcalendar.css" type="text/css" rel="stylesheet" />

    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>

    <script language="javascript" src="../content/cal/popcalendar.js" type="text/javascript"></script>

</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)' onload="hideProgressBar();">
    <form id="form1" runat="server" autocomplete="off">
        <div>
            <table cellspacing="0" cellpadding="0" width="100%" border="0">
                <tr>
                    <td class="PageHeading" align="left" colspan="2">
                        Order Download and Daily Feed Status Report</td>
                </tr>
                <tr>
                    <td class="SearchHeadPaneGradient" colspan="2">
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="False"
                            ShowSummary="True" CssClass="ValidationText"></asp:ValidationSummary>
                    </td>
                </tr>
                <tr style="height: 20px">
                    <td align="center" colspan="2">
                        <div class="ValidationText">
                            <asp:Literal ID="Literal_errorMsg" runat="server"></asp:Literal></div>
                        <div class="ConfirmationText">
                            <asp:Literal ID="Literal_ok" runat="server" EnableViewState="False"></asp:Literal></div>
                    </td>
                </tr>
                <tr align="left" style="background-color:WHITE">
                    <td style="height: 20px; padding-left:5px;" colspan="2"  >
                        <b>Search Filters</b></td>
                </tr>
                 <tr align="left" >
                    <tD colspan="2" >
                        <table width="100%" class ="SEARCHBOX" border="0"  >
                         <tr>
                                <td>
                                    <table id="search" width="90%" border="0">
                                  <tr>
                                            <td style="width: 120px; height: 17px">
                                                From(Creation Time) :
                                            </td>
                                            <td style="width: 190px; height: 17px">
                                                <asp:TextBox runat="server" ID="TextBox_FromDate" Width="119px" CssClass="textbox" />
                                                <img src="../images/IconCalendar.gif" alt="Click to open calender" onclick="popUpCalendar(this,document.getElementById('<%=TextBox_FromDate.ClientID %>'),'dd/mm/yyyy');"
                                                    style="border: 0; cursor: hand" id="IMG1" />
                                                <asp:RegularExpressionValidator ID="ValFromDate" CssClass="Validator" runat="server"
                                                    ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                                    ControlToValidate="TextBox_FromDate" ErrorMessage="Creation Time From Date is invalid.">*</asp:RegularExpressionValidator>
                                            </td>
                                            <td style="width: 99px; height: 17px">
                                                To(Creation Time) :</td>
                                            <td style="height: 17px; width: 326px;">
                                                <asp:TextBox runat="server" ID="TextBox_ToDate" Width="119px" CssClass="textbox" />
                                                <img src="../images/IconCalendar.gif" alt="Click to open calender" onclick="popUpCalendar(this,document.getElementById('<%=TextBox_ToDate.ClientID %>'),'dd/mm/yyyy');"
                                                    style="border: 0; cursor: hand" id="IMG2" />
                                                <asp:RegularExpressionValidator ID="ValToDate" CssClass="Validator" runat="server"
                                                    ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                                    ControlToValidate="TextBox_ToDate" ErrorMessage="Creation Time To Date is invalid.">*</asp:RegularExpressionValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width: 120px; height: 17px">
                                                From(End Time) :
                                            </td>
                                            <td style="width: 190px; height: 17px">
                                                <asp:TextBox runat="server" ID="TextBox_EndTimeFromDate" Width="119px" CssClass="textbox" />
                                                <img src="../images/IconCalendar.gif" alt="Click to open calender" onclick="popUpCalendar(this,document.getElementById('<%=TextBox_EndTimeFromDate.ClientID %>'),'dd/mm/yyyy');"
                                                    style="border: 0; cursor: hand" id="IMG3" />
                                                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" CssClass="Validator"
                                                    runat="server" ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                                    ControlToValidate="TextBox_EndTimeFromDate" ErrorMessage="End Time From Date is invalid.">*</asp:RegularExpressionValidator>
                                            </td>
                                            <td style="width: 99px; height: 17px">
                                                To(End Time) :</td>
                                            <td style="height: 17px; width: 326px;">
                                                <asp:TextBox runat="server" ID="TextBox_EndTimeToDate" Width="119px" CssClass="textbox" />
                                                <img src="../images/IconCalendar.gif" alt="Click to open calender" onclick="popUpCalendar(this,document.getElementById('<%=TextBox_EndTimeToDate.ClientID %>'),'dd/mm/yyyy');"
                                                    style="border: 0; cursor: hand" id="IMG4" />
                                                <asp:RegularExpressionValidator ID="RegularExpressionValidator2" CssClass="Validator"
                                                    runat="server" ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                                    ControlToValidate="TextBox_EndTimeToDate" ErrorMessage="End Time To Date is invalid.">*</asp:RegularExpressionValidator>
                                            </td>
                                        </tr>
                            <tr>
                                <td style="height: 20px">
                                    Status :</td>
                                <td style="height: 20px">
                                    <asp:DropDownList ID="DropDownList_status" runat="server" Width="144px" CssClass="textbox">
                                    </asp:DropDownList>
                                </td>
                                <td style="height: 20px">
                                    Task Type :</td>
                                <td style="height: 20px">
                                    <asp:DropDownList ID="DropDownList_TaskType" runat="server"  CssClass="textboxNormal">
                                    <asp:ListItem Value="2,3">Any</asp:ListItem>
                                    <asp:ListItem Value="2">Daily Feed Upload</asp:ListItem>
                                    <asp:ListItem Value="3">Cash Order Download</asp:ListItem>
                                    </asp:DropDownList></td>
                            </tr>
                            
                           
                            <tr>
                                <td colspan="6" align="right" style="padding-top:20PX;">
                                    <asp:Button ID="btnSearch" runat="server" Width="130px" Text="Generate Report" CssClass="button"
                                        OnClick="btnSearch_Click"></asp:Button>&nbsp;
                                    <input id="ButtonCancel" class="button" onclick="window.location='reportList.aspx'" style="width: 52px"
                                        type="button" value="Close" />
                                    
                                </td>
                                </table>
                                </td>
                                  <td>
                                    <table>
                                        <tr>
                                            <td>
                                                <img src="../images/Icon_search.png" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </tD>
                    
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
