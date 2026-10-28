<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ScheduleDailyFeed.aspx.cs" Inherits="CCMSUI.CCMS.ScheduleDailyFeed" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Untitled Page</title>

    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>

    <link href="../content/iSuite.css" rel="stylesheet" />

    <script type="text/javascript">      
        //window.parent.window.SetContentPage( window.location);
    </script>

    <script language="javascript" src="../content/cal/popcalendar.js" type="text/javascript" />

    <script type="text/javascript">      
        	
		
    </script>
    

    <link href="../content/cal/popcalendar.css" type="text/css" rel="stylesheet" />
</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)' onload="hideProgressBar();">
    <form id="form1" runat="server">
        <div>
            <table style="width: 100%" cellspacing="0" cellpadding="0" border="0" class="mainbox">
                <tr>
                    <td class="PageHeading" align="left">
                         Schedule Daily Feed</td>
                    
                </tr>
               
                <tr>
                    <td class="ValidationText" align="center">
                        <asp:Literal ID="Literal_error" runat="server" EnableViewState="False"></asp:Literal></td>
                </tr>
                <tr>
                    <td class="ConfirmationText" align="center">
                        <asp:ValidationSummary ID="valSummary" runat="server" />
                        <%=Session[SessionVars.notice.ToString()]%>
                        <%Session[SessionVars.notice.ToString()] = "";%>
                        <asp:Literal ID="Literal_ok" runat="server" EnableViewState="False"></asp:Literal></td>
                </tr>
               
                <tr align="left" style="background-color:WHITE">
                    <td style="height: 20px; padding-left:5px;" colspan="2"  >
                        <b>Schedule</b></td>
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
                        <asp:TextBox runat="server" ID="TextBox_FromDate" Width="119px" CssClass="textbox" />
                        <img src="../images/IconCalendar.gif" alt="Click to open calender" onclick="popUpCalendar(this,document.getElementById('<%=TextBox_FromDate.ClientID %>'),'dd/mm/yyyy');"
                            style="border: 0; cursor: hand" id="IMG1" />
                            
                            <asp:RegularExpressionValidator ID="ValFromDate" CssClass="Validator" runat="server"
                                        ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                        ControlToValidate="TextBox_FromDate" ErrorMessage="From Date is invalid.">*</asp:RegularExpressionValidator>
                            
                            </td>
                    <td style="width: 99px; height: 17px">
                        To :</td>
                    <td style="height: 17px; width: 326px;">
                        <asp:TextBox runat="server" ID="TextBox_ToDate" Width="119px" CssClass="textbox" />
                        <img src="../images/IconCalendar.gif" alt="Click to open calender" onclick="popUpCalendar(this,document.getElementById('<%=TextBox_ToDate.ClientID %>'),'dd/mm/yyyy');"
                            style="border: 0; cursor: hand" id="IMG2" />
                            
                            <asp:RegularExpressionValidator ID="ValToDate" CssClass="Validator" runat="server"
                                        ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                        ControlToValidate="TextBox_ToDate" ErrorMessage="To Date is invalid.">*</asp:RegularExpressionValidator>
                            
                            </td>
                </tr>
                <tr>
                    <td style="vertical-align:top">
                        Organization
                    </td>
                    <td>
                        <asp:ListBox ID="ListBox_Organization" runat="server"
                         CssClass="textbox" SelectionMode ="Multiple" Height="200px"></asp:ListBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        Delete Existing Data Before Generation
                    </td>
                    <td>
                        <asp:CheckBox ID="CheckBox_DeleteExistingData" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td>
                        Enable DFF Generation
                    </td>
                    <td>
                        <asp:CheckBox ID="CheckBox_EnableDFFGeneration" runat="server"  Checked="true"/>
                    </td>
                </tr>
                <tr>
                    <td colspan="4" align="right" style="padding-right: 60px">
                        <asp:Button CssClass="button" ID="button_create_schedule" runat="server" Text="Create Schedule" OnClick="button_create_schedule_Click"  />
                        &nbsp;<asp:Button ID="Button_Reset" runat="server" CssClass="button"
                                                 OnClick ="Button_Reset_Click" Text="Reset" />&nbsp;
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
        <input type="button" runat="server" id="refresh" style="display: none" />
    </form>
</body>
</html>
