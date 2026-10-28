<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UploadStatus.aspx.cs" Inherits="CCMSUI.CCMS.UploadStatus"
    Culture="en-GB" Title="CCMS - Upload Status" %>

<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Upload Status</title>
    <link href="../content/ISUITE.css" rel="stylesheet" />

    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
        <link href="../content/DatePicker/jquery-ui.css" rel="stylesheet" />
<script src="../content/DatePicker/jquery-3.7.1.js"></script>
<script src="../content/DatePicker/jquery-ui.js"></script>
    <script type="text/javascript">
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


        function search() {
            Page_ClientValidate();
            if (Page_IsValid) {

                var status = document.getElementById('DDLUploadStatus').value;
                var ip = document.getElementById('txtATMIP').value;
                var fromDate = document.getElementById('txtFromDate').value;
                var toDate = document.getElementById('txtToDate').value;
                GridCallBack.callback(status, ip, fromDate, toDate);
            }
        }
    </script>

     


</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)'
    onload="hideProgressBar();">
    <form id="form1" runat="server">
    <div>
        <table cellspacing="0" cellpadding="0" border="0" class="mainbox">
            <tr style="width: 100%">
                <td class="PageHeading" align="left">
                    Package Distribution Status
                </td>
            </tr>
            <tr>
                <td class="ValidationText" align="center">
                    <asp:Literal ID="Literal_error" runat="server" EnableViewState="False"></asp:Literal>
                </td>
            </tr>
            <tr>
                <td class="ConfirmationText" align="center">
                    <%=Session[SessionVars.notice.ToString()]%>
                    <%Session[SessionVars.notice.ToString()] = "";%>
                    <asp:Literal ID="Literal_ok" runat="server" EnableViewState="False"></asp:Literal>
                </td>
            </tr>
            <tr align="left" style="background-color: WHITE">
                <td style="height: 20px; padding-left: 5px;" colspan="2">
                    <b>Search Filters</b>
                </td>
            </tr>
            <tr>
                <tr align="left">
                    <td colspan="2">
                        <table width="100%" class="SEARCHBOX" border="0">
                            <tr>
                                <td>
                                    <table id="search" width="90%" border="0">
                                        <tr>
                                            <td>
                                                From :
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtFromDate" runat="server"  CssClass="datepicker" Font-Names="Verdana"
                                                Font-Size="10px" />
                                                <asp:RegularExpressionValidator ID="ValFromDate" CssClass="Validator" runat="server"
                                                    ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                                    ControlToValidate="txtFromDate" ErrorMessage="From Date is invalid.">*</asp:RegularExpressionValidator>
                                            </td>
                                            <td>
                                                To :
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtToDate" runat="server"  CssClass="datepicker" Font-Names="Verdana"
                                                Font-Size="10px" />
                                                <asp:RegularExpressionValidator ID="Regularexpressionvalidator1" CssClass="Validator"
                                                    runat="server" ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                                    ControlToValidate="txtToDate" ErrorMessage="To Date is invalid.">*</asp:RegularExpressionValidator>
                                                <asp:CompareValidator ID="CompareValidator2" runat="server" ControlToCompare="txtFromDate"
                                                    ControlToValidate="txtToDate" ErrorMessage="ToDate Must be less than equal to FromDate."
                                                    Operator="GreaterThanEqual" Type="Date">*</asp:CompareValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                Status:
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="DDLUploadStatus" runat="server" CssClass="dropDown" Width="113px">
                                                    <asp:ListItem Value="Any" Text="Any" Selected="True"></asp:ListItem>
                                                    <asp:ListItem Value="scheduled" Text="Scheduled"></asp:ListItem>
                                                    <asp:ListItem Value="uploading" Text="Uploading"></asp:ListItem>
                                                    <asp:ListItem Value="uploading-disconnected" Text="Uploading disconnected"></asp:ListItem>
                                                    <asp:ListItem Value="package-executed-with-exceptions" Text="Package Executed With Exceptions"></asp:ListItem>
                                                    <asp:ListItem Value="package-executed-successfully" Text="Package Executed Successfully"></asp:ListItem>
                                                    <asp:ListItem Value="uploading-failed" Text="Uploading Failed"></asp:ListItem>
                                                    <asp:ListItem Value="upload-completed" Text="Upload Completed"></asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                            <td style="width: 10%">
                                                Package
                                            </td>
                                            
                                                <td>
                                                    <asp:DropDownList ID="DropDownList_Package" runat="server" CssClass="dropDown">
                                                    </asp:DropDownList>
                                                </td>
                                                <%--<td>
                              ATM IP:
                                            </td>
                                            <td>
                                                <asp:textbox id="txtATMIP" runat="server" cssclass="textbox"></asp:textbox>
                                                <asp:regularexpressionvalidator id="Regularexpressionvalidator6" runat="server" cssclass="ValidationText"
                                                    validationexpression="^\b(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b$"
                                                    errormessage="Incorrect ATM IP Address." controltovalidate="txtATMIP">*</asp:regularexpressionvalidator>
                                            </td>--%>
                                        </tr>
                                        <tr>
                                            <td>
                                            </td>
                                            <td>
                                                <asp:Button ID="Button_Search" runat="server" CssClass="button" OnClick="Button_Search_Click"
                                                    Width="71px" Text="Search" />
                                                <asp:Button ID="Button_Reset" runat="server" CssClass="button" OnClick="Button_Reset_Click"
                                                    Width="71px" Text="Reset" />
                                            </td>
                                            <td>
                                            </td>
                                            <td>
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
                        Selected Schedules -
                        <asp:Literal ID="Literal_count" runat="server" EnableViewState="False"></asp:Literal>
                    </th>
                </tr>
                <tr>
                    <td colspan="3">
                        <asp:GridView ID="GridView_Uploads" runat="server" AutoGenerateColumns="False" AllowSorting="true"
                            OnSorting="GridView_Uploads_Sorting" Width="100%" AllowPaging="True" OnPageIndexChanging="GridView_Uploads_PageIndexChanging"
                            PageSize="15" OnRowCommand ="GridView_Uploads_RowCommand" OnRowDataBound="GridView_Uploads_RowDataBound">
                            <AlternatingRowStyle BackColor="#eff0ef" />
                            <Columns>
                                <asp:BoundField DataField="atmtitle" HeaderText="ATM" SortExpression="atmtitle" HeaderStyle-ForeColor="black"
                                    ItemStyle-HorizontalAlign="Center">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>
                                <asp:BoundField DataField="atm_bandwidth_name" HeaderText="Link Type" SortExpression="atm_bandwidth_name" HeaderStyle-ForeColor="black"
                                    ItemStyle-HorizontalAlign="Center">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>
                                
                                <asp:BoundField DataField="package_title" HeaderText="Package Title" SortExpression="package_title"
                                    ItemStyle-HorizontalAlign="Center" HeaderStyle-ForeColor="black">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>
                                
                                <asp:BoundField DataField="creation_time" HeaderText="Creation time" ItemStyle-HorizontalAlign="Center"
                                    SortExpression="creation_time" HeaderStyle-ForeColor="black">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>
                                <asp:BoundField DataField="source_file_path" HeaderText="File To Upload" SortExpression="source_path"
                                    HeaderStyle-ForeColor="black" Visible="false">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>
                                <asp:BoundField DataField="status" HeaderText="Status" SortExpression="status" HeaderStyle-ForeColor="black"
                                    ItemStyle-HorizontalAlign="Center">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>
                                <asp:BoundField DataField="last_invoked" HeaderText="Last Invoked" SortExpression="last_invoked"
                                    HeaderStyle-ForeColor="black" ItemStyle-HorizontalAlign="Center">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>
                                <asp:BoundField DataField="execution_time" HeaderText="Execution Time" SortExpression="execution_time"
                                    HeaderStyle-ForeColor="black" ItemStyle-HorizontalAlign="Center">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>
                                <asp:BoundField DataField="retry_remaining" HeaderText="Retry Remaining" SortExpression="retry_remaining"
                                    HeaderStyle-ForeColor="black" ItemStyle-HorizontalAlign="Right">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>
                                <asp:BoundField DataField="package_size" HeaderText="Package Size" SortExpression="file_size"
                                    HeaderStyle-ForeColor="black" ItemStyle-HorizontalAlign="Right">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>
                                <asp:BoundField DataField="uploaded_bytes" HeaderText="Uploaded Bytes" SortExpression="uploaded_bytes"
                                    HeaderStyle-ForeColor="black" ItemStyle-HorizontalAlign="Right">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>
                                <asp:BoundField DataField="end_time" HeaderText="Package Upload Time" ItemStyle-HorizontalAlign="Center"
                                    SortExpression="end_time" HeaderStyle-ForeColor="black">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>
                                <asp:BoundField DataField="user_login" HeaderText="Created By" SortExpression="user_login"
                                    HeaderStyle-ForeColor="black" ItemStyle-HorizontalAlign="Center">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>
                                <asp:BoundField DataField="failure_reason" HeaderText="Failure Reason" SortExpression="failure_reason"
                                    HeaderStyle-ForeColor="black">
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:BoundField>
                              <%--  <asp:TemplateField HeaderText="Edit">
                                    <ItemTemplate>
                                        <asp:ImageButton runat="server" CommandName="editUpload" CommandArgument='<%# Eval("upload_id") %>'
                                            ToolTip="Click to edit upload" ImageUrl="../images/Edit.gif" ID="Button_editUpload"
                                            CssClass="ShowHand" />
                                    </ItemTemplate>
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:TemplateField>
                                --%>
                                <asp:TemplateField HeaderText="Delete">
                                    <ItemTemplate>
                                        <asp:ImageButton runat="server" CommandName="deleteUpload" CommandArgument='<%# Eval("upload_id") %>'
                                            ToolTip="Click to delete uploading job" ImageUrl="../images/Delete.gif" ID="Button_deleteUpload"
                                            CssClass="ShowHand" />
                                    </ItemTemplate>
                                    <HeaderStyle CssClass="HeadingCell" />
                                </asp:TemplateField>
                            </Columns>
                            <EmptyDataTemplate>
                                No Packages found!</EmptyDataTemplate>
                            <HeaderStyle BackColor="Black" ForeColor="White" CssClass="HeadingCell" />
                        </asp:GridView>
                        <%--    <componentart:callback id="GridCallBack" runat="server" oncallback="GridCallBack_Callback">
                            <Content>
                                <ComponentArt:Grid ID="GridUpload" RunningMode="Client" EnableViewState="true" CssClass="Grid"
                                    FooterCssClass="GridFooter" PagerTextCssClass="PagerText" PagerStyle="Numbered"
                                    SliderHeight="20" SliderWidth="150" SliderGripWidth="9" SliderPopupOffsetX="35"
                                    SliderPopupClientTemplateId="SliderTemplate" PagerButtonWidth="41" PagerButtonHeight="22"
                                    PageSize="15" ImagesBaseUrl="../images/Grid" PagerImagesFolderUrl="../images/Grid/pager/"
                                    Width="100%" Height="460" runat="server" EmptyGridText="No Record Found">
                                    <Levels>
                                        <ComponentArt:GridLevel RowCssClass="Row" AlternatingRowCssClass="AlternatingRow"
                                            DataCellCssClass="DataCell" HeadingCellCssClass="HeadingCell" HeadingTextCssClass="HeadingCellTextCA"
                                            SortAscendingImageUrl="asc.gif" SortDescendingImageUrl="desc.gif" SortImageWidth="9"
                                            SortImageHeight="5">
                                            <Columns>
                                                <ComponentArt:GridColumn DataField="atmtitle" HeadingText="ATM" Width="50" />
                                                <ComponentArt:GridColumn DataField="title" HeadingText="Upload Title" Width="100"   />
                                                
                                                <ComponentArt:GridColumn DataField="status" HeadingText="Upload Status" Width="50" />
                                                <ComponentArt:GridColumn DataField="source_path" HeadingText="Source File Path" Width="250" />
                                                <ComponentArt:GridColumn DataField="uploaded_bytes" HeadingText="Bytes Uploaded" Width="40" />
                                                <ComponentArt:GridColumn DataField="connection_breaks" HeadingText="Connection Breaks" Width="50" />
                                                <ComponentArt:GridColumn DataField="creation_time" HeadingText="Creation Time"
                                                    FormatString="dd/MM/yyyy HH:mm:ss" Width="60" />
                                                    <ComponentArt:GridColumn DataField="end_time" HeadingText="End Time"
                                                    FormatString="dd/MM/yyyy HH:mm:ss" Width="60" />
                                                <ComponentArt:GridColumn DataField="user_login" HeadingText="Created By" />
                                            </Columns>
                                        </ComponentArt:GridLevel>
                                    </Levels>
                                    <ClientTemplates>
                                        <ComponentArt:ClientTemplate ID="SliderTemplate">
                                            <table class="SliderPopup" cellspacing="0" cellpadding="0" border="0">
                                                <tr>
                                                    <td valign="top" style="padding: 5px;">
                                                        <table width="100%" cellspacing="0" cellpadding="0" border="0">
                                                            <tr>
                                                                <td>
                                                                    <table cellspacing="0" cellpadding="1" border="0" style="width: 255px;">
                                                                        <tr>
                                                                            <td colspan="2" style="font-family: verdana; font-size: 11px; font-weight: bold;">
                                                                                <div style="overflow: hidden; width: 250px;">
                                                                                    <nobr>Upload: ## DataItem.GetMember('title').Value ##</nobr>
                                                                                </div>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td style="font-family: verdana; font-size: 11px;">
                                                                                <div style="overflow: hidden; width: 115px;">
                                                                                    <nobr>by ## DataItem.GetMember('user_login').Value ##</nobr>
                                                                                </div>
                                                                            </td>
                                                                            <td style="font-family: verdana; font-size: 11px; color: #918B7A;" align="right">
                                                                                <div style="overflow: hidden; width: 135px;">
                                                                                    <nobr>ATM: ## DataItem.GetMember('atmtitle').Text ##</nobr>
                                                                                </div>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td style="font-family: verdana; font-size: 11px;" colspan="2">
                                                                                <nobr>## DataItem.GetMember('status').Text ##</nobr>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td style="font-family: verdana; font-size: 11px;" colspan="2">
                                                                                <b>## DataItem.GetMember('creation_time').Text ##</b></td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="height: 14px; background-color: #757598;">
                                                        <table width="100%" cellspacing="0" cellpadding="0" border="0">
                                                            <tr>
                                                                <td style="padding-left: 5px; color: white; font-family: verdana; font-size: 10px;">
                                                                    Page <b>## DataItem.PageIndex + 1 ##</b> of <b>## GridUpload.PageCount ##</b>
                                                                </td>
                                                                <td style="padding-right: 5px; color: white; font-family: verdana; font-size: 10px;"
                                                                    align="right">
                                                                    Upload <b>## DataItem.Index + 1 ##</b> of <b>## GridUpload.RecordCount ##</b>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                            </table>
                                        </ComponentArt:ClientTemplate>
                                    </ClientTemplates>
                                </ComponentArt:Grid>
                            </Content>
                            <LoadingPanelClientTemplate>
                                <table width="100%" height="400" cellspacing="0" cellpadding="0" border="0">
                                    <tr>
                                        <td align="center">
                                            <table cellspacing="0" cellpadding="0" border="0">
                                                <tr>
                                                    <td style="font-size: 10px;">
                                                        Loading...&nbsp;</td>
                                                    <td>
                                                        <img src="../images/spinner.gif" width="16" height="16" border="0"></td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </LoadingPanelClientTemplate>
                        </componentart:callback>--%>
                    </td>
                </tr>
        </table>
        <input type="button" runat="server" id="refresh" style="display: none" />
    </div>
    </form>
</body>
</html>
