<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ScheduleReconBatch.aspx.cs"
    Inherits="CCMSUI.CCMS.ScheduleReconBatch" %>

<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
    <title>Schedule Reconciliation Batch</title>
    <link href="../content/Scripts/ui-lightness/jquery-ui-1.7.2.custom.css" rel="stylesheet"
        type="text/css" />
    <script type="text/javascript" src="../content/scripts/jquery-1.4.1.js"></script>
    <script type="text/javascript" src="../content/scripts/ui.datepicker.js"></script>
    <script type="text/javascript">
        $(function () {

            $('.datepicker').datepicker({ dateFormat: 'dd/mm/yy',
                changeMonth: true,
                changeYear: true,
                showOn: "button",
                buttonImage: "../images/calendar.gif",
                buttonImageOnly: true

            });
        });
    
    </script>
    <script type="text/javascript">



        window.parent.window.SetContentPage(window.location);


        //		function setCheck(val)
        //		{
        //		    var inputs = document.getElementsByTagName('input');
        //		    for (var i = 0; i < inputs.length; i++)
        //            {
        //            var typ = inputs[i].type;
        //            if (typ=="checkbox")
        //                inputs[i].checked = val;    
        //            }
        //		}

        function LabelText(labeltext) {
            document.getElementById('Label_errorMsg').innerHTML = "<li>" + labeltext + "</li>";
        }        
    
    </script>
    <link href="../content/iSuite.css" rel="stylesheet" />
</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)'
    onload="hideProgressBar();">
    <form id="form1" runat="server">
    <div>
        <div id="notificationArea" style="text-align: center">
        </div>
        <asp:Literal ID="Literal_msg" runat="server" EnableViewState="false">
    
        </asp:Literal>
        <table style="width: 100%" cellspacing="0" cellpadding="0" border="0" class="mainbox">
            <tr>
                <td class="PageHeading" align="left" colspan="2">
                    Schedule Reconciliation Batch
                </td>
            </tr>
            <tr>
                <td class="ValidationText" align="center">
                    <asp:Literal ID="Literal_error" runat="server" EnableViewState="False"></asp:Literal>
                    <div class="ValidationText">
                        <asp:Label ID="Label_errorMsg" runat="server" EnableViewState="False"></asp:Label></div>
                </td>
            </tr>
            <tr>
                <td class="ConfirmationText" align="center">
                    <%=Session[SessionVars.notice.ToString()]%>
                    <%Session[SessionVars.notice.ToString()] = "";%>
                    <asp:Literal ID="Literal_ok" runat="server" EnableViewState="False"></asp:Literal>
                </td>
            </tr>
            <tr align="left">
                <td valign="top">
                    <table id="search" width="85%" border="0">
                        <tr>
                            <td class="style2">
                                From :
                            </td>
                            <td class="style1">
                                <asp:TextBox runat="server" ID="TextBox_FromDate" CssClass="datepicker" Font-Names="Verdana"
                                    Font-Size="10px" />
                                <asp:RegularExpressionValidator ID="ValFromDate" CssClass="Validator" runat="server"
                                    ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                    ControlToValidate="TextBox_FromDate" ErrorMessage="From Date is invalid.">*</asp:RegularExpressionValidator>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator_From" ControlToValidate="TextBox_FromDate"
                                    runat="server" ErrorMessage="From Date cannot be empty" Text="*"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td class="style2">
                                To :
                            </td>
                            <td class="style1">
                                <asp:TextBox runat="server" ID="TextBox_ToDate" CssClass="datepicker" Font-Names="Verdana"
                                    Font-Size="10px" />
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" CssClass="Validator"
                                    runat="server" ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                    ControlToValidate="TextBox_FromDate" ErrorMessage="From Date is invalid.">*</asp:RegularExpressionValidator>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator_ToDate" ControlToValidate="TextBox_ToDate"
                                    runat="server" ErrorMessage="To Date cannot be empty" Text="*"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td class="style2">
                                Acceptable Difference :
                            </td>
                            <td>
                                <asp:DropDownList ID="DropDownList_Diff" runat="server" CssClass="dropDown">
                                    <asp:ListItem Text="Fixed" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="Percentage" Value="1"></asp:ListItem>
                                </asp:DropDownList>
                                &nbsp;
                                <asp:TextBox runat="server" ID="TextBox_AcceptableDifference" Font-Names="Verdana"
                                    Font-Size="10px" />
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator_AccDiff" CssClass="Validator"
                                    runat="server" ValidationExpression="\d+(\.\d+)?" ControlToValidate="TextBox_AcceptableDifference"
                                    ErrorMessage="Acceptable difference only contains decimal/integer values.">*</asp:RegularExpressionValidator>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                ATM Switch Report
                            </td>
                            <td>
                                <asp:FileUpload ID="FileUpload_SwitchData" runat="server" />
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator_Data" runat="server" ControlToValidate="FileUpload_SwitchData">Please select CSV file to upload</asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator_Data" runat="server"
                                    ControlToValidate="FileUpload_SwitchData" ValidationExpression=".*\.csv">Only CSV file is allowed</asp:RegularExpressionValidator>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                General Ledger (GL) Host File
                            </td>
                            <td>
                                <asp:FileUpload ID="FileUpload_GLData" runat="server" />
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="FileUpload_GLData">Please select CSV file to upload</asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="FileUpload_GLData"
                                    ValidationExpression=".*\.csv">Only CSV file is allowed</asp:RegularExpressionValidator>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" style="padding-right: 150px" align="left">
                                <asp:Button ID="btnSchedule" Text="Schedule" CssClass="button" runat="server" OnClick="ButtonSchedule_Click"
                                    Width="80px" />
                                <input type="button" value="Cancel" onclick="window.location='Reconciliation.aspx'"
                                    class="button" style="width: 80px" />
                            </td>
                        </tr>
                    </table>
                </td>
                <td>
                    <table>
                        <tr>
                            <td colspan="2">
                                <b>Swich And GL Files Should have following columns. </b>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <table>
                                    <tr>
                                        <td>
                                            <span style="color: Red">*</span>ATM ID
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <span style="color: Red">*</span>Card Number
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Customer Account Number
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <span style="color: Red">*</span>Transaction Date
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <span style="color: Red">*</span>Transaction Time(HH24:MI:SS)
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <span style="color: Red">*</span>Transaction Amount
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Transaction Currency(ISO Code)
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <span style="color: Red">*</span>Transaction Sequence
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Transaction Type
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Transaction Response
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Transaction Settlement Date
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Card Network
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Card Issuer
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <b>Fields marked with <span style="color: Red">*</span> are mandatory.</b>
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
