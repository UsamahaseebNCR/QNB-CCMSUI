<%@ Page Language="C#" AutoEventWireup="true" Codebehind="FtpFileTransferStatus.aspx.cs" Inherits="CCMSUI.CCMS.FtpFileTransferStatus" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >

<head runat="server">
    <title>iSuite - FTP File Transfer Status</title>
    
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
    <form id="form1" runat="server">
        <div>
            <table style="width: 100%" cellspacing="0" cellpadding="0" border="0" class="mainbox">
                <tr>
                    <td class="PageHeading" align="left">
                        Order Download And Daily Feed Status</td>
                    
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
                                                From(Creation Time) :
                                            </td>
                                            <td style="width: 190px; height: 17px">
                                                <asp:TextBox runat="server" ID="TextBox_FromDate" Width="119px" CssClass="datepicker" Font-Names="Verdana"
                                                Font-Size="10px" /> 
                                                <asp:RegularExpressionValidator ID="ValFromDate" CssClass="Validator" runat="server"
                                                    ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                                    ControlToValidate="TextBox_FromDate" ErrorMessage="Creation Time From Date is invalid.">*</asp:RegularExpressionValidator>
                                            </td>
                                            <td style="width: 99px; height: 17px">
                                                To(Creation Time) :</td>
                                            <td style="height: 17px; width: 326px;">
                                                <asp:TextBox runat="server" ID="TextBox_ToDate" Width="119px" CssClass="datepicker" Font-Names="Verdana"
                                                Font-Size="10px" /> 
                                              
                                                <asp:RegularExpressionValidator ID="ValToDate" CssClass="Validator" runat="server"
                                                    ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                                    ControlToValidate="TextBox_ToDate" ErrorMessage="Creation Time To Date is invalid.">*</asp:RegularExpressionValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width: 120px; height: 17px">
                                                From(End Time) :
                                            </td>
                                            <td style="width: 190px; height: 17px">
                                                <asp:TextBox runat="server" ID="TextBox_EndTimeFromDate" Width="119px" CssClass="datepicker" Font-Names="Verdana"
                                                Font-Size="10px" /> 
                                               
                                                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" CssClass="Validator"
                                                    runat="server" ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                                    ControlToValidate="TextBox_EndTimeFromDate" ErrorMessage="End Time From Date is invalid.">*</asp:RegularExpressionValidator>
                                            </td>
                                            <td style="width: 99px; height: 17px">
                                                To(End Time) :</td>
                                            <td style="height: 17px; width: 326px;">
                                                <asp:TextBox runat="server" ID="TextBox_EndTimeToDate" Width="119px" CssClass="datepicker" Font-Names="Verdana"
                                                Font-Size="10px" /> 
                                              
                                                <asp:RegularExpressionValidator ID="RegularExpressionValidator2" CssClass="Validator"
                                                    runat="server" ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                                    ControlToValidate="TextBox_EndTimeToDate" ErrorMessage="End Time To Date is invalid.">*</asp:RegularExpressionValidator>
                                            </td>
                                        </tr>
                            <tr>
                                <td style="height: 20px">
                                    Status :</td>
                                <td style="height: 20px">
                                    <asp:DropDownList ID="DropDownList_status" runat="server"  CssClass="textboxNormal">
                                    </asp:DropDownList>
                                </td>
                                <td style="height: 20px">
                                    Task Type :</td>
                                <td style="height: 20px">
                                    <asp:DropDownList ID="DropDownList_TaskType" runat="server"  CssClass="textboxNormal">
                                    <asp:ListItem Value="2,3,13">Any</asp:ListItem>
                                    <asp:ListItem Value="2,13">Daily Feed Upload</asp:ListItem>
                                    <asp:ListItem Value="3">Cash Order Download</asp:ListItem>
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
                    </tD>
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
                            HeaderStyle-ForeColor="black" OnRowDataBound="GridView_Files_RowDataBound"
                            >
                            <AlternatingRowStyle BackColor="#eff0ef" />
                            <RowStyle HorizontalAlign="Center"  />
                            <Columns>
                                <asp:BoundField DataField="region_name" HeaderText="Organization" SortExpression="region_name">
                                    <HeaderStyle CssClass="HeadingCell"  ForeColor="black" />
                                </asp:BoundField>
                                <asp:BoundField DataField="creation_time" HeaderText="Creation Time" SortExpression="creation_time"  DataFormatString="{0:dd/MM/yy HH:mm:ss}" HtmlEncode ="false">
                                    <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                </asp:BoundField>
                                
                                
                                <asp:BoundField DataField="task_type_name" HeaderText="Type" SortExpression="task_type_name">
                                    <HeaderStyle CssClass="HeadingCell"  ForeColor="black"/>
                                </asp:BoundField>
                                
                                <asp:BoundField DataField="status" HeaderText="Status" SortExpression="status">
                                    <HeaderStyle CssClass="HeadingCell"  ForeColor="black"/>
                                </asp:BoundField>
                  
                               
                                <asp:BoundField DataField="end_time" HeaderText="End Time" SortExpression="end_time"  DataFormatString="{0:dd/MM/yy HH:mm:ss}" HtmlEncode ="false">
                                    <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                </asp:BoundField>
                                <asp:TemplateField HeaderText="Failure Reason">
                                    <HeaderStyle CssClass="HeadingCell" />
                                    <ItemTemplate>
                                        <a href="FtpFileTransfer_details.aspx?file_id=<%#Eval("ftp_file_info_id") %>">
                                            <%#Eval("reason") %>
                                        </a>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                
                                <asp:TemplateField HeaderText="Details">
                                    <ItemTemplate>
                                        <a style="border: 0" href="FtpFileTransfer_details.aspx?file_id=<%#Eval("ftp_file_info_id") %>">
                                            <img alt="View Details" src="../images/description.jpg" style="border: 0;width:18px;height:14px" /></a>
                                    </ItemTemplate>
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="View">
                                    <ItemTemplate>
                                        <asp:HyperLink runat="server" ID="hyperlink_view" NavigateUrl="FtpFileTransferViewer.aspx?file_id=<%#Eval('ftp_file_info_id') %>">
                                        <img alt="View File" src="../images/viewfile.jpg" style="border: 0;width:18px;height:14px" /></asp:HyperLink>
                                    </ItemTemplate>
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:TemplateField>
                                                                
                                <asp:TemplateField HeaderText="Save">
                                    <ItemTemplate>
                                        <asp:ImageButton ImageUrl="../images/saveas.gif" ID="ImageButton_SaveAsTXT" runat="server"
                                            OnClick="SaveAsTXT_Click" CommandArgument='<%#Eval("ftp_file_info_id")%>'></asp:ImageButton>
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
