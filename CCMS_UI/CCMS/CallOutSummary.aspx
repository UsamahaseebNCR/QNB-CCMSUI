<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CallOutSummary.aspx.cs"
    Inherits="CCMSUI.CCMS.CallOutSummary" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
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
                <td class="PageHeading" align="left" colspan="3">
                    Call Out Summary
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
                    <%=Session[SessionVars.notice.ToString()]%>
                    <%Session[SessionVars.notice.ToString()] = "";%>
                    <asp:Literal ID="Literal_ok" runat="server" EnableViewState="False"></asp:Literal>
                </td>
            </tr>
            <tr align="left" style="background-color: WHITE">
                <td style="height: 20px; padding-left: 5px;" colspan="3">
                    <b>Search Filters</b>
                </td>
            </tr>
            <tr align="left">
                <td colspan="3">
                    <table width="100%" class="SEARCHBOX" border="0">
                        <tr>
                            <td>
                                <table id="search" width="90%" border="0">
                                    <tr>
                                        <td style="width: 120px; height: 17px">
                                            From :
                                        </td>
                                        <td style="width: 190px; height: 17px">
                                            <asp:TextBox runat="server" ID="TextBox_FromDate" Width="155px" CssClass="textbox" />
                                            <img src="../images/IconCalendar.gif" alt="Click to open calender" onclick="popUpCalendar(this,document.getElementById('<%=TextBox_FromDate.ClientID %>'),'dd/mm/yyyy');"
                                                style="border: 0; cursor: hand" id="IMG1" />
                                            <asp:RegularExpressionValidator ID="ValFromDate" CssClass="Validator" runat="server"
                                                ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                                ControlToValidate="TextBox_FromDate" ErrorMessage="From Date is invalid.">*</asp:RegularExpressionValidator>
                                        </td>
                                        <td style="width: 99px; height: 17px">
                                            To :
                                        </td>
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
                                        </td>
                                        <td>
                                        </td>
                                        <td>
                                        </td>
                                        <td>
                                            <asp:Button ID="Button_search" runat="server" CssClass="button" OnClick="btnSearch_Click"
                                                Text="Search" Width="71px" />
                                            &nbsp;<asp:Button ID="Button_reset" runat="server" CssClass="button" Text="Reset"
                                                Width="71px" OnClick="Button_reset_Click" />&nbsp;
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
                </td>
            </tr>
            <tr>
                <td>
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td valign="top" style="text-align: center" width="30%">
                    <asp:GridView ID="GridView_CallOutSummary" runat="server" AutoGenerateColumns="False"
                        Width="100%" HeaderStyle-ForeColor="Black" CssClass="grid" AllowSorting="true"
                        OnSorting="GridView_CallOutSummary_Sorting">
                        <AlternatingRowStyle BackColor="#eff0ef" />
                        <Columns>
                            <%--    <asp:TemplateField SortExpression="atm_location" ItemStyle-HorizontalAlign="Left">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                <HeaderTemplate>
                                    <asp:LinkButton ID="LinkButtonSite" runat="server" Text="Site" CommandName="Sort"
                                        CommandArgument="atm_location">
                                    </asp:LinkButton>
                                </HeaderTemplate>
                                <ItemTemplate>
                                
                                    <asp:HyperLink ID="HyperLink_UserId" runat="server" Text='<%#Eval("atm_location") %>'
                                     NavigateUrl="~/CCMS/CallOutList.aspx?l=<%#Eval('atm_location') %>"></asp:HyperLink>
                                </ItemTemplate>
                            </asp:TemplateField>--%>
                            <asp:BoundField DataField="atm_location" HeaderText="Site" SortExpression="atm_location"
                                HtmlEncode="false" HeaderStyle-CssClass="HeadingCell" ItemStyle-HorizontalAlign="Left" />
                            <asp:BoundField DataField="total" HeaderText="Count" SortExpression="total" HeaderStyle-CssClass="HeadingCell"
                                ItemStyle-HorizontalAlign="Right" />
                            <%--<asp:HyperLinkField Text="Details" DataNavigateUrlFields="atm_location" DataNavigateUrlFormatString="~/CCMS/CallOutList.aspx?l='{0}'"
                                HeaderStyle-CssClass="HeadingCell" />--%>
                            <asp:TemplateField HeaderStyle-CssClass="HeadingCell">
                                <ItemTemplate>
                                    <asp:HyperLink ID="hyperDetails" runat="server" NavigateUrl='<%# "~/CCMS/CallOutList.aspx?l=" + HttpUtility.UrlEncode(Eval("atm_location").ToString())  
%>' Text="Details" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                            No Record(s) Found
                        </EmptyDataTemplate>
                    </asp:GridView>
                </td>
                <td valign="top" style="padding-left: 30px;" width="30%">
                    <asp:GridView ID="GridView_CodeSummary" runat="server" AutoGenerateColumns="False"
                        AllowSorting="true" OnSorting="GridView_CodeSummary_Sorting" Width="30%" HeaderStyle-ForeColor="Black"
                        CssClass="grid">
                        <AlternatingRowStyle BackColor="#eff0ef" />
                        <RowStyle HorizontalAlign="Center" />
                        <Columns>
                            <asp:BoundField DataField="code" HeaderText="Code" SortExpression="code" HeaderStyle-CssClass="HeadingCell"
                                ItemStyle-HorizontalAlign="Left" />
                            <asp:BoundField DataField="total" HeaderText="Count" SortExpression="total" HeaderStyle-CssClass="HeadingCell"
                                ItemStyle-HorizontalAlign="Right" />
                            <%--<asp:HyperLinkField Text="Details" DataNavigateUrlFields="code" DataNavigateUrlFormatString="~/CCMS/CallOutList.aspx?c={0}"
                                HeaderStyle-CssClass="HeadingCell" />--%>
                            <asp:TemplateField HeaderStyle-CssClass="HeadingCell">
                                <ItemTemplate>
                                    <asp:HyperLink ID="hyperDetails" runat="server" NavigateUrl='<%# "~/CCMS/CallOutList.aspx?c=" + HttpUtility.UrlEncode(Eval("code").ToString())  %>'
                                        Text="Details" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                            No Record(s) Found
                        </EmptyDataTemplate>
                    </asp:GridView>
                </td>
                <td valign="top" style="padding-left: 30px;" width="40%">
                    <asp:GridView ID="GridView_problemSummary" runat="server" AutoGenerateColumns="False"
                        AllowSorting="true" OnSorting="GridView_ProblemSummary_Sorting" Width="100%" HeaderStyle-ForeColor="Black"
                        CssClass="grid">
                        <AlternatingRowStyle BackColor="#eff0ef" />
                        <RowStyle HorizontalAlign="Center" />
                        <Columns>
                            <asp:BoundField DataField="actual_problem_found" HeaderText="Issue" SortExpression="actual_problem_found"
                                HeaderStyle-CssClass="HeadingCell" ItemStyle-HorizontalAlign="Left" />
                            <asp:BoundField DataField="atm_id_no" HeaderText="ATM" SortExpression="atm_id_no"
                                HeaderStyle-CssClass="HeadingCell" ItemStyle-HorizontalAlign="Right" />
                            <asp:BoundField DataField="total" HeaderText="Count" SortExpression="total" HeaderStyle-CssClass="HeadingCell"
                                ItemStyle-HorizontalAlign="Right" />
                            <%--<asp:HyperLinkField Text="Details" DataNavigateUrlFields="code" DataNavigateUrlFormatString="~/CCMS/CallOutList.aspx?c={0}"
                                HeaderStyle-CssClass="HeadingCell" />--%>
                            <%--<asp:TemplateField HeaderStyle-CssClass="HeadingCell">
                                <ItemTemplate>
                                    <asp:HyperLink ID="hyperDetails" runat="server" NavigateUrl='<%# "~/CCMS/CallOutList.aspx?p=" + HttpUtility.UrlEncode(Eval("actual_problem_found").ToString())  %>'
                                        Text="Details" />
                                </ItemTemplate>
                            </asp:TemplateField>--%>
                        </Columns>
                        <EmptyDataTemplate>
                            No Record(s) Found
                        </EmptyDataTemplate>
                    </asp:GridView>
                </td>
            </tr>
        </table>
    </div>
    <input type="button" runat="server" id="refresh" style="display: none" />
    </form>
</body>
</html>
