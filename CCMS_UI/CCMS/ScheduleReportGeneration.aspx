<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ScheduleReportGeneration.aspx.cs" Inherits="Avanza.iSuite.UI.ScheduleReportGeneration" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
  <link href="../content/ISUITE.css" rel="stylesheet" />
  <title>Schedule Report Generation</title>

    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>

    <script type="text/javascript">

    </script>
</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)' onload="hideProgressBar();">
    <form id="form1" runat="server">
        <div>
            <table style="width: 100%" cellspacing="0" cellpadding="0" border="0" class="mainbox">
                <tr>
                    <td class="PageHeading" align="left">
                        Schedule Report Generation</td>
                    
                </tr>
               
                <tr>
                    <td class="ValidationText" align="center">
                        <asp:Literal ID="Literal_error" runat="server" EnableViewState="False"></asp:Literal></td>
                </tr>
                <tr>
                    <td class="ConfirmationText" align="center">
                        <%=Session[SessionVars.notice.ToString()]%>
                        <%Session[SessionVars.notice.ToString()] = "";%>
                        <asp:Literal ID="Literal_ok" runat="server" EnableViewState="False"></asp:Literal></td>
                </tr>
               
               
                <tr>
                    <td align="left">
                        &nbsp; &nbsp; &nbsp;&nbsp;
                    </td>
                </tr>
                <tr align="left">
                    <th colspan="2" align="left">
                       
                        <asp:Literal ID="Literal_count" runat="server" EnableViewState="False"></asp:Literal></th>
                </tr>
                <tr>
                    <td valign="top" colspan="2">
                        <asp:GridView ID="GridView_ReportSchedule" runat="server" AutoGenerateColumns="False" 
                            Width="100%" OnRowCommand="GridView_ReportSchedule_RowCommand"  CssClass="grid"
                            OnRowDataBound="GridView_ReportSchedule_RowDataBound">
                           
                            <AlternatingRowStyle BackColor="#eff0ef" />
                            <Columns>
                                <asp:BoundField DataField="region_name" HeaderText="Organization" SortExpression="report_name" HeaderStyle-ForeColor="black">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>
							  <asp:BoundField DataField="cit_name" HeaderText="CIT" SortExpression="cit_name" HeaderStyle-ForeColor="black">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>																																																																										   
                                <asp:BoundField DataField="report_name" HeaderText="Report Name" SortExpression="report_name" HeaderStyle-ForeColor="black">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>
                                <asp:BoundField DataField="report_friendly_name" HeaderText="Report Friendly Name" SortExpression="report_friendly_name" HeaderStyle-ForeColor="black">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>
                                <asp:TemplateField HeaderText="Edit" >
                                    <ItemTemplate>
                                        <a href='EditReportGenerationSchedule.aspx?report_schedule_id=<%# Eval("report_schedule_id") %>'>
                                            <img alt="Click to edit report generation" src="../images/Edit.gif" style="border: 0" class="ShowHand" />
                                        </a>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" />
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Delete">
                                    <ItemTemplate>
                                        <asp:ImageButton runat="server" CommandName="deleteSchedule" CommandArgument='<%# Eval("report_schedule_id") %>'
                                            ToolTip="Click to delete schedule" ImageUrl="../images/Delete.gif" ID="Button_deleteSchedule"
                                            CssClass="ShowHand" />
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" />
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:TemplateField>
                            </Columns>
                            <EmptyDataTemplate>
                                No Report Template found!</EmptyDataTemplate>
                            <HeaderStyle BackColor="Black" ForeColor="White" CssClass="HeadingCell" />
                        </asp:GridView>
                        &nbsp;
                    </td>
                </tr>
            </table>
        </div>
        <input type="button" runat="server" id="refresh" style="display: none" />
    </form>
</body>
</html>
