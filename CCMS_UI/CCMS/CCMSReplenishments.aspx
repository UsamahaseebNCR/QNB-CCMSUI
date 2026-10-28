<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CCMSReplenishments.aspx.cs"
    Inherits="CCMSUI.CCMS.CCMSReplenishments" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="../content/isuite.css" rel="stylesheet" />
    <title>CCMS - Replenishments</title>
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
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
                <td class="PageHeading" align="left">
                    Replenishments
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
                <td style="height: 20px; padding-left: 5px; width: 100%;" colspan="2">
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
                                        <td>
                                            From :
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="TextBox_FromDate"  CssClass="datepicker" Font-Names="Verdana"
                                                Font-Size="10px" />
                                            <asp:RegularExpressionValidator ID="ValFromDate" CssClass="Validator" runat="server"
                                                ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                                ControlToValidate="TextBox_FromDate" ErrorMessage="From Date is invalid.">*</asp:RegularExpressionValidator>
                                        </td>
                                        <td style="width: 99px; height: 17px">
                                            To :
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="TextBox_ToDate"  CssClass="datepicker" Font-Names="Verdana"
                                                Font-Size="10px" />
                                            <asp:RegularExpressionValidator ID="ValToDate" CssClass="Validator" runat="server"
                                                ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                                ControlToValidate="TextBox_ToDate" ErrorMessage="To Date is invalid.">*</asp:RegularExpressionValidator>
                                        </td>
                                    </tr>
                                    <%--<tr>
                                        <td style="height: 20px">
                                            Replenishment Type :
                                        </td>
                                        <td style="height: 20px">
                                            <asp:DropDownList ID="DropDownList_repType" runat="server" Style="width: 150px" CssClass="textbox">
                                                <asp:ListItem Selected="True" Value="Any">Any</asp:ListItem>
                                                <asp:ListItem Value="RS">RS</asp:ListItem>
                                                <asp:ListItem Value="UR">UR</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td style="height: 20px">
                                            Created by :
                                        </td>
                                        <td style="height: 20px">
                                            <asp:DropDownList ID="DropDownList_creator" runat="server" Width="150px" CssClass="dropDown">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="height: 20px">
                                            Status :
                                        </td>
                                        <td style="height: 20px">
                                            <asp:DropDownList ID="DropDownList_OrderStatus" runat="server" Width="150px" CssClass="dropDown">
                                                <asp:ListItem>Any</asp:ListItem>
                                                <asp:ListItem>Cancelled</asp:ListItem>
                                                <asp:ListItem>Normal</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>--%>
                                    <tr>
                                        <td>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td >
                                        </td>
                                        <td colspan="2">
                                            <asp:Button ID="Button_search" runat="server" CssClass="button" OnClick="Button_search_Click"
                                                Text="Search" Width="71px" />
                                            &nbsp;<asp:Button ID="Button_Reset" runat="server" CssClass="button" OnClick="Button_Reset_Click"
                                                Text="Reset" />
                                            <asp:HyperLink ID="HyperLink_PostRep" runat="server" CssClass="Link" NavigateUrl="PostReplenishment.aspx" ForeColor="Red" >Post Replenishment ... </asp:HyperLink>
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
                <td align="left">
                    &nbsp; &nbsp; &nbsp;&nbsp;
                </td>
            </tr>
            <tr align="left">
                <th colspan="2" style="width: 100%" align="left">
                    Selected Replenishments -
                    <asp:Literal ID="Literal_count" runat="server" EnableViewState="False"></asp:Literal>
                </th>
            </tr>
            <tr>
                <td valign="top" colspan="2">
                    <asp:GridView ID="GridView_Rep" runat="server" AutoGenerateColumns="False" AllowSorting="true"
                        OnSorting="GridView_Rep_Sorting" Width="100%" AllowPaging="True" OnPageIndexChanging="GridView_Rep_PageIndexChanging"
                        PageSize="10"  DataKeyNames="replenishment_id"
                        OnRowCommand="GridView_Rep_RowCommand" OnRowDataBound ="GridView_Rep_RowDataBound">
                        <RowStyle HorizontalAlign="Center" />
                        <AlternatingRowStyle BackColor="#eff0ef" />
                        <Columns>
                            <asp:BoundField DataField="title" HeaderText="Title" SortExpression="title" HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="rep_datetime" HeaderText="Replenished At" SortExpression="rep_datetime"
                                DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" HtmlEncode="false" HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="cash_added1" HeaderText="Type 1" SortExpression="cash_added1"
                                HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="cash_added2" HeaderText="Type 2" SortExpression="cash_added2"
                                HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="cash_added3" HeaderText="Type 3" SortExpression="cash_added3"
                                HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="cash_added4" HeaderText="Type 4" SortExpression="cash_added4"
                                HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="cash_added5" HeaderText="Type 5" SortExpression="cash_added5"
                                HeaderStyle-ForeColor="black"  Visible="false">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="cash_added6" HeaderText="Type 6" SortExpression="cash_added6"
                                HeaderStyle-ForeColor="black"  Visible="false">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="cash_added7" HeaderText="Type 7" SortExpression="cash_added7"
                                HeaderStyle-ForeColor="black"  Visible="false">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            
                            <asp:BoundField DataField="amt_text" HtmlEncode="false" HeaderText="Amount"
                                HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            

                            <asp:BoundField DataField="rep_status" HeaderText="Status" SortExpression="rep_status"
                                HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="last_tsn" HeaderText="Last TSN" SortExpression="tsn"
                                HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:CheckBoxField  DataField="is_swap" HeaderText="Is Swap" SortExpression="is_swap"
                                HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:CheckBoxField>
                            <asp:BoundField DataField="generated_at" HeaderText="Generation Time" SortExpression="generated_at"
                                DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" HtmlEncode="false" HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="user_full_name" HeaderText="Generated By" SortExpression="user_full_name"
                                HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            
                            <asp:BoundField DataField="modified_datetime" HeaderText="Modification Time" SortExpression="modified_datetime"
                                DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" HtmlEncode="false" HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <%--<asp:TemplateField HeaderText="Edit" HeaderStyle-ForeColor="black">
                                <ItemTemplate>
                                    <asp:HyperLink ID="HyperLink_EditCashOrders" runat="server" ImageUrl="../images/Edit.gif"
                                        NavigateUrl='<%#String.Format("PostReplenishment.aspx?replenishment_id={0}",Eval("replenishment_id"))%>'>
                                    </asp:HyperLink>
                                </ItemTemplate>
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Delete">
                                <ItemTemplate>
                                    <asp:ImageButton runat="server" CommandName="deleteRep" CommandArgument='<%# Eval("replenishment_id") %>'
                                        ToolTip="Click to delete replenishment" ImageUrl="../images/Delete.gif" ID="Button_DeleteRep"
                                        CssClass="ShowHand" />
                                </ItemTemplate>
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:TemplateField>--%>
                        </Columns>
                        <EmptyDataTemplate>
                            No Records Found!</EmptyDataTemplate>
                        <%-- <HeaderStyle BackColor="Black" ForeColor="White" CssClass="HeadingCell" />--%>
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
