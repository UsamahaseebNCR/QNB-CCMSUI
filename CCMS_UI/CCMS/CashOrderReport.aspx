<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CashOrderReport.aspx.cs" Inherits="CCMSUI.CCMS.CashOrderReport" Culture="fr-FR" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Cash Order Report</title>
    <link href="../content/iSuite.css" rel="stylesheet" />  
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
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
</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)' onload="hideProgressBar();">
    <form id="form1" runat="server" autocomplete="off">
        <div>
            <table cellspacing="0" cellpadding="0" width="100%" border="0">
                <tr>
                    <td class="PageHeading" align="left" colspan="2">
                        Cash Order Report
                    </td>
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
                                    From :
                                </td>
                                <td style="width: 190px; height: 17px">
                                    <asp:TextBox runat="server" ID="TextBox_fromdate" Width="119px" class="datepicker" />
                                   
                                        
                                         <asp:RegularExpressionValidator ID="ValFromDate" CssClass="Validator" runat="server"
                                                         ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d (0[0-9]|1[0-9]|2[0-4]):([0-5]\d):([0-5]\d)"
                                            ControlToValidate="TextBox_fromdate" ErrorMessage="From Date is invalid.">*</asp:RegularExpressionValidator>

                                        
                                        </td>
                                <td style="width: 99px; height: 17px">
                                    To :</td>
                                <td style="height: 17px; width: 326px;">
                                    <asp:TextBox runat="server" ID="TextBox_toDate" Width="119px" class="datepicker" />
                                        
                                           <asp:RegularExpressionValidator ID="ValToDate" CssClass="Validator" runat="server"
                                                         ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d (0[0-9]|1[0-9]|2[0-4]):([0-5]\d):([0-5]\d)"
                                            ControlToValidate="TextBox_toDate" ErrorMessage="To Date is invalid.">*</asp:RegularExpressionValidator>

                                        
                                        </td>
                            </tr>
                                                       <tr>
                                            <td style="height: 20px">
                                                Replenishment Type :</td>
                                            <td style="height: 20px">
                                                <asp:DropDownList ID="DropDownList_repType" runat="server" Style="width: 150px" CssClass="textbox">
                                                    <asp:ListItem Selected="True" Value="Any">Any</asp:ListItem>
                                                    <asp:ListItem Value="RS">RS</asp:ListItem>
                                                    <asp:ListItem Value="UR">UR</asp:ListItem>
                                                </asp:DropDownList></td>
                                                                                            <td style="height: 20px">
                                                Created by :</td>
                                            <td style="height: 20px">
                                                <asp:DropDownList ID="DropDownList_creator" runat="server" Width="150px" CssClass="textbox">
                                                </asp:DropDownList>
                                            </td>

                                                
                                        </tr>
	 <tr>
                                            <td style="height: 20px"> Generate Report for CIT:</td>
                                            <td style="height: 20px">
                                                <asp:ListBox ID="ListBox_CIT_Selection" runat="server" Width="150px" CssClass="textbox" SelectionMode="Multiple">
                                                </asp:ListBox>
                                            </td>
                                        </tr>										
                            <tr style="padding-top:5px"></tr>
                                        <tr >
                                            
                                            <td colspan="4" align="right" style="padding-top:20PX;">
                                                <asp:Button ID="Button_search" runat="server" CssClass="button" OnClick="btnSearch_Click"
                                                    Text="Generate Report" Width="130px" />
                                                &nbsp;
                                                <asp:Button ID="Button_GenerateReportSchedule" runat="server" Width="160px" Text="Generate Report Schedule" CssClass="button"
                                                OnClick="Button_GenerateReportSchedule_Click" OnClientClick="return confirm('Are you sure you want to delete existing schedule and add new schedule for this report?');"></asp:Button>&nbsp;
                                                <input id="ButtonCancel" class="button" onclick="window.location='reportList.aspx'" style="width: 52px"
                                        type="button" value="Close" />&nbsp;
                                                
                                            </td>
                                            
                                        </tr>
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
