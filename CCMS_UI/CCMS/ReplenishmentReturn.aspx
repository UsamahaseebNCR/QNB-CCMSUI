<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReplenishmentReturn.aspx.cs"
    Inherits="CCMS.ReplenishmentReturn" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head2" runat="server">
    <link href="../content/isuite.css" rel="stylesheet" />
    <title>Replenishments Return</title>
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
                    <td class="PageHeading" align="left">Replenishment Return
                    </td>
                     <td align="right" class="PageHeading">
                        <img src="../images/back.png" onclick="window.location = '../admin/operations.aspx'" style="cursor:pointer" />

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
                                            <td>From :
                                            </td>
                                            <td>
                                                <asp:TextBox runat="server" ID="TextBox_FromDate" CssClass="datepicker" />
                                                 <asp:RegularExpressionValidator ID="ValFromDate" CssClass="Validator" runat="server"
                                                    ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                                    ControlToValidate="TextBox_FromDate" ErrorMessage="From Date is invalid.">*</asp:RegularExpressionValidator>
                                            </td>
                                            <td style="width: 99px; height: 17px">To :
                                            </td>
                                            <td>
                                                <asp:TextBox runat="server" ID="TextBox_ToDate" CssClass="datepicker" />
                                                 <asp:RegularExpressionValidator ID="ValToDate" CssClass="Validator" runat="server"
                                                    ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                                    ControlToValidate="TextBox_ToDate" ErrorMessage="To Date is invalid.">*</asp:RegularExpressionValidator>
                                            </td>
                                        </tr>
                                     
                                        <tr>
                                            <td>&nbsp;
                                            </td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td colspan="2">
                                                <asp:Button ID="Button_search" runat="server" CssClass="button" OnClientClick="window.setTimeout(showProgressBar(),1000);" OnClick="Button_search_Click"
                                                    Text="Search" Width="71px" />
                                                &nbsp;<asp:Button ID="Button_Reset" runat="server" CssClass="button" OnClientClick="window.setTimeout(showProgressBar(),1000);" OnClick="Button_Reset_Click"
                                                    Text="Reset" />&nbsp;
                                                   <asp:Button runat="server" ID="btnExportReplenishments" Text="Export Replenishment Return Data" CssClass="button" OnClientClick="window.setTimeout(showProgressBar(),1000);" OnClick="btnExportReplenishments_Click" />
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
                    <td align="left">&nbsp; &nbsp; &nbsp;&nbsp;
                    </td>
                </tr>
                <tr align="left">
                    <th colspan="2" style="width: 100%" align="left">Selected Replenishment Return -
                    <asp:Literal ID="Literal_count" runat="server" EnableViewState="False"></asp:Literal>
                    </th>
                </tr>

                <tr>
                    <td valign="top" colspan="2">
                        <asp:GridView ID="GridView_Rep" runat="server" AutoGenerateColumns="False" AllowSorting="true"
                            OnSorting="GridView_Rep_Sorting" Width="100%" AllowPaging="True" OnPageIndexChanging="GridView_Rep_PageIndexChanging"
                            PageSize="10" DataKeyNames="Replenishment Date and Time">
                            <RowStyle HorizontalAlign="Center" />
                            <AlternatingRowStyle BackColor="#eff0ef" />
                            <Columns>
                                <asp:BoundField DataField="ATM Title" HeaderText="ATM Title" SortExpression="ATM Title" HeaderStyle-ForeColor="black">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>

                                <asp:BoundField DataField="Replenishment Date and Time" HeaderText="Replenished At" SortExpression="Replenishment Date and Time" HeaderStyle-ForeColor="black"
                                    DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" HtmlEncode="false">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Denomination Type 1" HeaderText="Denomination Type 1" SortExpression="Denomination Type 1" HeaderStyle-ForeColor="black">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>

                                <asp:BoundField DataField="Cash Added Type 1" HeaderText="Cash Added Type 1" SortExpression="Cash Added Type 1" HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                                  </asp:BoundField>

                                 <asp:BoundField DataField="Cash Added RC Type 1" HeaderText="Cash Added RC Type 1" SortExpression="Cash Added RC Type 1" HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                                  </asp:BoundField>

                                <asp:BoundField DataField="Return Amount Type 1" HeaderText="Return Amount Type 1" SortExpression="Return Amount Type 1" HeaderStyle-ForeColor="black">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>

                                <asp:BoundField DataField="Return RC Amount Type 1" HeaderText="Return RC Amount Type 1" SortExpression="Return RC Amount Type 1" HeaderStyle-ForeColor="black">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>

                              
                                  <asp:BoundField DataField="Denomination Type 2" HeaderText="Denomination Type 2" SortExpression="Denomination Type 2" HeaderStyle-ForeColor="black">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>

                                <asp:BoundField DataField="Cash Added Type 2" HeaderText="Cash Added Type 2" SortExpression="Cash Added Type 2" HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                                  </asp:BoundField>

                                 <asp:BoundField DataField="Cash Added RC Type 2" HeaderText="Cash Added RC Type 2" SortExpression="Cash Added RC Type 2" HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                                  </asp:BoundField>

                                <asp:BoundField DataField="Return Amount Type 2" HeaderText="Return Amount Type 2" SortExpression="Return Amount Type 2" HeaderStyle-ForeColor="black">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>

                                <asp:BoundField DataField="Return RC Amount Type 2" HeaderText="Return RC Amount Type 2" SortExpression="Return RC Amount Type 2" HeaderStyle-ForeColor="black">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>


                                 <asp:BoundField DataField="Denomination Type 3" HeaderText="Denomination Type 3" SortExpression="Denomination Type 3" HeaderStyle-ForeColor="black">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>

                                <asp:BoundField DataField="Cash Added Type 3" HeaderText="Cash Added Type 3" SortExpression="Cash Added Type 3" HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                                  </asp:BoundField>

                                 <asp:BoundField DataField="Cash Added RC Type 3" HeaderText="Cash Added RC Type 3" SortExpression="Cash Added RC Type 3" HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                                  </asp:BoundField>

                                <asp:BoundField DataField="Return Amount Type 3" HeaderText="Return Amount Type 3" SortExpression="Return Amount Type 3" HeaderStyle-ForeColor="black">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>

                                <asp:BoundField DataField="Return RC Amount Type 3" HeaderText="Return RC Amount Type 3" SortExpression="Return RC Amount Type 3" HeaderStyle-ForeColor="black">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>



                                <asp:BoundField DataField="Denomination Type 4" HeaderText="Denomination Type 4" SortExpression="Denomination Type 4" HeaderStyle-ForeColor="black">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>

                                <asp:BoundField DataField="Cash Added Type 4" HeaderText="Cash Added Type 4" SortExpression="Cash Added Type 4" HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                                  </asp:BoundField>

                                 <asp:BoundField DataField="Cash Added RC Type 4" HeaderText="Cash Added RC Type 4" SortExpression="Cash Added RC Type 4" HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                                  </asp:BoundField>

                                <asp:BoundField DataField="Return Amount Type 4" HeaderText="Return Amount Type 4" SortExpression="Return Amount Type 4" HeaderStyle-ForeColor="black">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>

                                <asp:BoundField DataField="Return RC Amount Type 4" HeaderText="Return RC Amount Type 4" SortExpression="Return RC Amount Type 4" HeaderStyle-ForeColor="black">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>


                                   <asp:BoundField DataField="Total Cash Added Amount" HeaderText="Total Cash Added Amount" SortExpression="Total Cash Added Amount" HeaderStyle-ForeColor="black">
 <HeaderStyle CssClass="HeadingCell" />
</asp:BoundField>

<asp:BoundField DataField="Total RC Cash Added Amount" HeaderText="Total RC Cash Added Amount" SortExpression="Total RC Cash Added Amount" HeaderStyle-ForeColor="black">
<HeaderStyle CssClass="HeadingCell" />
     </asp:BoundField>

                                   <asp:BoundField DataField="Return Amount CD" HeaderText="Return Amount CD" SortExpression="Return Amount CD" HeaderStyle-ForeColor="black">
                                 <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>

                                <asp:BoundField DataField="Return Amount RC" HeaderText="Return Amount RC" SortExpression="Return Amount RC" HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                                     </asp:BoundField>

                                   <asp:BoundField DataField="Total Return Amount" HeaderText="Total Return Amount" SortExpression="Total Return Amount" HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                             </asp:BoundField>


                            </Columns>
                            <EmptyDataTemplate>
                                No Records Found!
                            </EmptyDataTemplate>
                          
                            <%-- <HeaderStyle BackColor="Black" ForeColor="White"/>--%>
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
