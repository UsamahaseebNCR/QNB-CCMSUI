<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserTask.aspx.cs" Inherits="CCMSUI.CCMS.UserTask" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Untitled Page</title>
    <link href="../content/iSuite.css" rel="stylesheet" />
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
   
    <style type="text/css">
        .style1
        {
            height: 17px;
            width: 585px;
        }
        .style2
        {
            width: 585px;
        }
    </style>

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
                <td class="PageHeading" align="left" colspan="2">
                    User Task
                </td>
            </tr>
            <tr>
                <td class="ValidationText" align="center" colspan="2">
                    <asp:Literal ID="Literal_error" runat="server" EnableViewState="False"></asp:Literal>
                </td>
            </tr>
            <tr>
                <td class="ConfirmationText" align="center" colspan="2">
                    <asp:ValidationSummary ID="valSummary" runat="server" />
                    <%=Session[SessionVars.notice.ToString()]%>
                    <%Session[SessionVars.notice.ToString()] = "";%>
                    <asp:Literal ID="Literal_ok" runat="server" EnableViewState="False"></asp:Literal>
                </td>
            </tr>
            <tr align="left">
                <td style="width: 70%" valign="top">
                    <table width="100%">
                        <tr align="left">
                            <th colspan="2" align="left">
                                Selected User Tasks -
                                <asp:Literal ID="Literal_count" runat="server" EnableViewState="False"></asp:Literal>
                            </th>
                        </tr>
                        <tr>
                            <td valign="top" style="text-align: center">
                                <asp:GridView ID="GridView_ATMSettlement" runat="server" AutoGenerateColumns="False"
                                    AllowSorting="true" OnSorting="GridView_ATMSettlement_Sorting" Width="100%" AllowPaging="True"
                                    OnPageIndexChanging="GridView_ATMSettlement_PageIndexChanging" HeaderStyle-ForeColor="Black"
                                    OnRowCommand="GridView_ATMSettlement_RowCommand" PageSize="15" DataKeyNames="user_task_id">
                                    <AlternatingRowStyle BackColor="#eff0ef" />
                                    <RowStyle HorizontalAlign="Center" />
                                    <Columns>
                                        <%--<asp:BoundField DataField="title" HeaderText="Title" SortExpression="title" HeaderStyle-CssClass="HeadingCell" />--%>
                                        <asp:BoundField DataField="user_full_name" HeaderText="User" SortExpression="user_full_name"
                                            HeaderStyle-CssClass="HeadingCell" />
                                            <asp:BoundField DataField="title" HeaderText="ATM" SortExpression="title"
                                            HeaderStyle-CssClass="HeadingCell" />
                                        <asp:BoundField DataField="creation_time" HeaderText="Creation Time" SortExpression="creation_time"
                                            HeaderStyle-CssClass="HeadingCell" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" />
                                        <asp:BoundField DataField="status" HeaderText="Status" HeaderStyle-CssClass="HeadingCell"
                                            SortExpression="Status" />
                                        <asp:BoundField DataField="task_type_name" HeaderText="Task" HeaderStyle-CssClass="HeadingCell"
                                            SortExpression="task_type_name" />
                                        <asp:BoundField DataField="task_desc" HeaderText="Description" HeaderStyle-CssClass="HeadingCell" ItemStyle-HorizontalAlign="Left"
                                            SortExpression="task_desc" />
                                        <asp:TemplateField HeaderText="">
                                            <ItemTemplate>
                                                <asp:ImageButton ID="imgbtn_settlement" runat="server" CommandName="Settlement" CommandArgument='<%# Eval("user_task_id") %>'
                                                    ToolTip='Click to view/update settlement' CssClass="ShowHand" ImageUrl="../images/Edit.gif" />
                                                <%-- <a href='ShowSettlementData.aspx?sid=<%# Eval("atm_settlement_id") %>'>
                                                    <img alt="Click to view settlement data" src="../images/Edit.gif" style="border: 0" class="ShowHand" />
                                                </a>--%>
                                            </ItemTemplate>
                                            <HeaderStyle CssClass="HeadingCell" />
                                        </asp:TemplateField>
                                    </Columns>
                                    <EmptyDataTemplate>
                                        No Record(s) Found
                                    </EmptyDataTemplate>
                                </asp:GridView>
                            </td>
                        </tr>
                    </table>
                </td>
                <td valign="top">
                    <table width="90%" class="SEARCHBOX" border="0" align="right">
                        <tr>
                            <td>
                                <table id="search" width="100%" border="0">
                                    <tr>
                                        <td class="style1">
                                            From :
                                        </td>
                                        <td style="width: 100px; height: 17px">
                                            <asp:TextBox runat="server" ID="TextBox_FromDate" Width="100px" CssClass="datepicker" Font-Names="Verdana"
                                                Font-Size="10px" />
                                            
                                            <asp:RegularExpressionValidator ID="ValFromDate" CssClass="Validator" runat="server"
                                                ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                                ControlToValidate="TextBox_FromDate" ErrorMessage="From Date is invalid.">*</asp:RegularExpressionValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="style1">
                                            To :
                                        </td>
                                        <td style="height: 17px; width: 100px;">
                                            <asp:TextBox runat="server" ID="TextBox_ToDate" Width="100px" CssClass="datepicker" Font-Names="Verdana"
                                                Font-Size="10px" />
                                         
                                            <asp:RegularExpressionValidator ID="ValToDate" CssClass="Validator" runat="server"
                                                ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                                ControlToValidate="TextBox_ToDate" ErrorMessage="To Date is invalid.">*</asp:RegularExpressionValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="style2">
                                            Task Type :
                                        </td>
                                        <td style="height: 17px; width: 326px;">
                                            <asp:DropDownList ID="DropDownList_TaskType" runat="server" CssClass="dropDown" AppendDataBoundItems="true">
                                                <asp:ListItem>Select</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="style1">
                                            Status :
                                        </td>
                                        <td style="height: 17px; width: 326px;">
                                            <asp:DropDownList ID="DropDownList_Status" runat="server" CssClass="dropDown">
                                                <asp:ListItem Value="Select">Select</asp:ListItem>
                                                <asp:ListItem Value="Pending" Selected="True">Pending</asp:ListItem>
                                                <asp:ListItem Value="Approved">Approved</asp:ListItem>
                                                <asp:ListItem Value="Rejected">Rejected</asp:ListItem>
                                                <asp:ListItem Value="Closed">Closed</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="style2">
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="3">
                                            <asp:Button ID="Button_Query" runat="server" OnClick="btnSearch_Click" CssClass="button"
                                                Text="Search" Width="71px" />&nbsp;
                                            <asp:Button ID="Button_reset" runat="server" CssClass="button" Text="Reset" Width="71px"
                                                OnClick="Button_reset_Click" />
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
