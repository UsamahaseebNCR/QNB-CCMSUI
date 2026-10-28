<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AuditLog.aspx.cs" Inherits="CCMSUI.CCMS.AuditLog" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
        <link href="../content/iSuite.css" rel="stylesheet" />
    <link href="../content/cal/popcalendar.css" type="text/css" rel="stylesheet" />
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
    <script language="javascript" src="../content/cal/popcalendar.js" type="text/javascript"></script>

</head>

<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)' onload="hideProgressBar();">
    <form id="form1" runat="server">
        <div>
            <table style="width: 100%" cellspacing="0" cellpadding="0" border="0" class="mainbox">
                <tr>
                    <td class="PageHeading" align="left">
                        Activity Log Listing</td>
                    
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
                                    <asp:TextBox runat="server" ID="TextBox_FromDate" width="155px" CssClass="textbox" />
                                    <img src="../images/IconCalendar.gif" alt="Click to open calender" onclick="popUpCalendar(this,document.getElementById('<%=TextBox_FromDate.ClientID %>'),'dd/mm/yyyy');"
                                        style="border: 0; cursor: hand" id="IMG1" />
                                        
                                                                     <asp:RegularExpressionValidator ID="ValFromDate" CssClass="Validator" runat="server"
                                        ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                        ControlToValidate="TextBox_FromDate" ErrorMessage="From Date is invalid.">*</asp:RegularExpressionValidator>

                                        
                                        </td>
                                <td style="width: 99px; height: 17px">
                                    To :</td>
                                <td style="height: 17px; width: 326px;">
                                    <asp:TextBox runat="server" ID="TextBox_ToDate" Width="155px" CssClass="textbox" />
                                    <img src="../images/IconCalendar.gif" alt="Click to open calender" onclick="popUpCalendar(this,document.getElementById('<%=TextBox_ToDate.ClientID %>'),'dd/mm/yyyy');"
                                        style="border: 0; cursor: hand" id="IMG2" />
                                        
                                                                       <asp:RegularExpressionValidator ID="ValToDate" CssClass="Validator" runat="server"
                                        ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                        ControlToValidate="TextBox_ToDate" ErrorMessage="To Date is invalid.">*</asp:RegularExpressionValidator>

                                        
                                        </td>
                            </tr>
                            <tr>
                                <td>
                                    User</td>
                                <td>
                                    <asp:DropDownList ID="DropDownList_User" runat="server" Width="175px" CssClass="textbox">
                                    </asp:DropDownList>
                                </td>
                                             <td>
                                    Rights
                                </td>
                                <td>
                                    <asp:DropDownList ID="DropDownList_Rights" runat="server" Width="175px" CssClass="textbox">
                                    </asp:DropDownList>
                                </td>
                    
                            </tr>
                            <tr>
                                <td>
                                </td>
                                <td>
                                </td>
                                <td>
                                </td>
                                <td>
                                    <asp:Button ID="Button_search" runat="server" CssClass="button" OnClick="Button_search_Click"
                                        Text="Search" Width="71px" />
                                    &nbsp;<asp:Button ID="Button_reset" runat="server" CssClass="button" Text="Reset"
                                        Width="71px" OnClick="Button_reset_Click" />&nbsp;&nbsp;&nbsp;<asp:Button ID="Button_ExportToPDF"
                                                    runat="server" CssClass="button" OnClick="Button_ExportToPDF_Click" Text="Export To PDF" />
                                            &nbsp; &nbsp;<asp:Button ID="Button1" runat="server" CssClass="button" OnClick="Button_ExportToExcel_Click"
                                                Text="Export To Excel" />
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
                <tr>
                    <td align="left">
                        &nbsp; &nbsp; &nbsp;&nbsp;
                    </td>
                </tr>
                <tr align="left">
                    <th colspan="2" align="left">
                        Selected Activity Logs -
                        <asp:Literal ID="Literal_count" runat="server" EnableViewState="False"></asp:Literal></th>
                </tr>
                <tr>
                    <td valign="top" colspan="2" style="text-align:center">
                        
                        <asp:GridView ID="GridView_AuditLog" runat="server" AutoGenerateColumns="False"
                            AllowSorting="true" OnSorting="GridView_AuditLog_Sorting" Width="100%" 
                            AllowPaging="True" OnPageIndexChanging="GridView_AuditLog_PageIndexChanging"
                             
                            HeaderStyle-ForeColor="Black" PageSize ="25">
                            <AlternatingRowStyle BackColor="#eff0ef" />
                            <RowStyle HorizontalAlign="Center" />
                            <Columns>
                                <asp:BoundField DataField="user_full_name" HeaderText="User" SortExpression="user_full_name" HeaderStyle-CssClass="HeadingCell" />
                                <asp:BoundField DataField="activity_time" HeaderText="Date Time" SortExpression="activity_time" HeaderStyle-CssClass="HeadingCell" DataFormatString="{0:dd/MM/yyyy HH:mm}"/>
                                <asp:BoundField DataField="ip_address" HeaderText="IP Address" HeaderStyle-CssClass="HeadingCell" />
                                <asp:BoundField DataField="field_name" HeaderText="Field Name" HeaderStyle-CssClass="HeadingCell" />
                                <asp:BoundField DataField="old_value" HeaderText="Old Value" HeaderStyle-CssClass="HeadingCell" />
                                <asp:BoundField DataField="new_value" HeaderText="New Value" HeaderStyle-CssClass="HeadingCell" />
                                <asp:BoundField DataField="message" HeaderText="Description" HeaderStyle-CssClass="HeadingCell" ItemStyle-HorizontalAlign="Left"/>
                            </Columns>
                            <EmptyDataTemplate>
                                No Activity Log found!</EmptyDataTemplate>
                        </asp:GridView>
                        
                         
                    </td>
                </tr>
            </table>
        </div>
        <input type="button" runat="server" id="refresh" style="display: none" />
    </form>
</body>

</html>
