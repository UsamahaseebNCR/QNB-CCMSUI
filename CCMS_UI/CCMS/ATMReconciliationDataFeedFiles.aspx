<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ATMReconciliationDataFeedFiles.aspx.cs" Inherits="CCMSUI.CCMS.ATMReconciliationDataFeedFiles" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >

<head runat="server">
    <title>Reconciliation Data Feed Files</title>
    
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>

    <link href="../content/iSuite.css" rel="stylesheet" />

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
<body onload="hideProgressBar();">
    <form id="form2" runat="server">
        <div>
            <table style="width: 100%" cellspacing="0" cellpadding="0" border="0" class="mainbox">
                <tr>
                    <td class="PageHeading" align="left">
                       ATM Reconciliation Data Feed Files</td>
                    
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
                    <td colspan="2" >
                        <table width="100%" class ="SEARCHBOX" border="0"  >
                            <tr>
                                <td>
                                    <table id="search" width="90%" border="0">
                              <tr>
                                            <td style="width: 120px; height: 17px">
                                                From :
                                            </td>
                                            <td style="width: 190px; height: 17px">
                                                <asp:TextBox runat="server" ID="TextBox_FromDate" Width="119px"  CssClass="datepicker" Font-Names="Verdana"
                                                Font-Size="10px" />
                                                <asp:RegularExpressionValidator ID="ValFromDate" CssClass="Validator" runat="server"
                                                    ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                                    ControlToValidate="TextBox_FromDate" ErrorMessage="Creation Time From Date is invalid.">*</asp:RegularExpressionValidator>
                                            </td>
                                            <td style="width: 99px; height: 17px">
                                                To :</td>
                                            <td style="height: 17px; width: 326px;">
                                                <asp:TextBox runat="server" ID="TextBox_ToDate" Width="119px"  CssClass="datepicker" Font-Names="Verdana"
                                                Font-Size="10px" />
                                                <asp:RegularExpressionValidator ID="ValToDate" CssClass="Validator" runat="server"
                                                    ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                                    ControlToValidate="TextBox_ToDate" ErrorMessage="Creation Time To Date is invalid.">*</asp:RegularExpressionValidator>
                                            </td>
                                        </tr>
                            <tr>
                                <td style="height: 20px">
                                    File Type :</td>
                                <td style="height: 20px">
                                    <asp:DropDownList ID="DropDownList_TaskType" runat="server"  CssClass="textboxNormal">
                                    <asp:ListItem Value="Switch,Host">Any</asp:ListItem>
                                    <asp:ListItem Value="Switch">Switch</asp:ListItem>
                                    <asp:ListItem Value="Host">Host</asp:ListItem>
                                    </asp:DropDownList></td>
                            </tr>
                            <tr>
                                <td></td>
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
                                    <asp:Button ID="Button_reset" runat="server" CssClass="button" Text="Reset"
                                        Width="71px" OnClick="Button_reset_Click" />
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
                    <th colspan="2" align="left">
                        Selected                        <asp:Literal ID="Literal_count" runat="server" EnableViewState="False"></asp:Literal></th>
                </tr>
                <tr>
                    <td valign="top" colspan="2">
                        <asp:GridView ID="GridView_FileTransferStatus" runat="server" AutoGenerateColumns="False" AllowSorting="true"
                            OnSorting="GridView_FileTransferStatus_Sorting" Width="100%"  PageSize="15"
                            AllowPaging="True" OnPageIndexChanging="GridView_FileTransferStatus_PageIndexChanging"
                            HeaderStyle-ForeColor="black"
                            >
                            <AlternatingRowStyle BackColor="#eff0ef" />
                            <RowStyle HorizontalAlign="Center"  />
                            <Columns>
                                <asp:BoundField DataField="file_type" HeaderText="File Type" SortExpression="file_type">
                                    <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                </asp:BoundField>
                                <asp:BoundField DataField="source_file_path" HeaderText="Source File Path" SortExpression="source_file_path">
                                    <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                </asp:BoundField>
                                <asp:BoundField DataField="creation_time" HeaderText="Creation Time" SortExpression="creation_time" DataFormatString="{0:dd/MM/yy HH:mm:ss}" HtmlEncode="false">
                                    <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                </asp:BoundField>
                                
                                
                                <asp:BoundField DataField="failure_reason" HeaderText="Failure Reason" SortExpression="failure_reason">
                                    <HeaderStyle CssClass="HeadingCell"  ForeColor="black"/>
                                </asp:BoundField>
                                  <asp:BoundField DataField="status" HeaderText="Status" SortExpression="status">
                                    <HeaderStyle CssClass="HeadingCell"  ForeColor="black"/>
                                </asp:BoundField>
                                <asp:BoundField DataField="retry_count" HeaderText="Retry Count" SortExpression="retry_count">
                                    <HeaderStyle CssClass="HeadingCell"  ForeColor="black"/>
                                </asp:BoundField>
                               
                                
                                <asp:BoundField DataField="last_invoked" HeaderText="Last Invoked" SortExpression="last_invoked">
                                    <HeaderStyle CssClass="HeadingCell"  ForeColor="black"/>
                                </asp:BoundField>

                             
<%--     To Do: File View 
    <asp:TemplateField HeaderText="View">
                                    <ItemTemplate>
                                        <asp:HyperLink runat="server" ID="hyperlink_view" NavigateUrl="FtpFileTransferViewer.aspx?file_id=<%#Eval('ftp_file_info_id') %>">
                                        <img alt="View File" src="../images/viewfile.jpg" style="border: 0;width:18px;height:14px" /></asp:HyperLink>
                                    </ItemTemplate>
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:TemplateField>--%>
                                                                
                                <asp:TemplateField HeaderText="Save">
                                    <ItemTemplate>
                                        <asp:ImageButton ImageUrl="../images/saveas.gif" ID="ImageButton_SaveAsTXT" runat="server"
                                            OnClick="SaveAsTXT_Click" CommandArgument='<%#Eval("source_file_path")%>'></asp:ImageButton>
                                    </ItemTemplate>
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:TemplateField>
                                
                             </Columns>
                            <EmptyDataTemplate>
                                No File Transfer found!</EmptyDataTemplate>
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

