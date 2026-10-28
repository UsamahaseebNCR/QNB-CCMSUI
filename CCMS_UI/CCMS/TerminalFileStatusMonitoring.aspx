<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TerminalFileStatusMonitoring.aspx.cs"
    Inherits="CCMSUI.CCMS.TerminalFileStatusMonitoring" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Untitled Page</title>
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
    <link href="../content/iSuite.css" rel="stylesheet" />

    <script src="../content/DateTimePicker/jquery.min.js"></script>
    <link href="../content/DateTimePicker/jquery.datetimepicker.min.css" rel="stylesheet" />
    <script src="../content/DateTimePicker/jquery.datetimepicker.full.min.js"></script>


<script>
    $(function () {
        $(".datepicker").datetimepicker({
            format: 'd/m/Y H:i:s', // format for both date and time
            step: 15 // time step interval
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
                    Task Status Monitoring ( History )
                </td>
            </tr>
            <tr>
                <td class="ValidationText" align="center">
                    <asp:literal id="Literal_error" runat="server" enableviewstate="False"></asp:literal>
                </td>
            </tr>
            <tr>
                <td class="ConfirmationText" align="center">
                    <asp:validationsummary id="valSummary" runat="server" />
                    <%=Session[SessionVars.notice.ToString()]%>
                    <%Session[SessionVars.notice.ToString()] = "";%>
                    <asp:literal id="Literal_ok" runat="server" enableviewstate="False"></asp:literal>
                </td>
            </tr>
            <tr align="left" style="background-color: WHITE">
                <td style="height: 20px; padding-left: 5px;" colspan="2">
                    <b>Search Filters</b>
                </td>
            </tr>
            <tr align="left">
                <td colspan="2">
                    <table width="100%" class="SEARCHBOX" border="0">
                        <tr>
                            <td>
                                <table id="search" width="100%" border="0">
                                    <tr>
                                        <td style="width: 120px; height: 17px">
                                            From(Creation Time) :
                                        </td>
                                        <td style="width: 190px; height: 17px">
                                            <asp:textbox runat="server" id="TextBox_FromDate" cssclass="datepicker" />
                                            <%--<img src="../images/IconCalendar.gif" alt="Click to open calender" onclick="popUpCalendar(this,document.getElementById('<%=TextBox_FromDate.ClientID %>'),'dd/mm/yyyy');"
                                                style="border: 0; cursor: hand" id="IMG1" />--%>
                                            <asp:regularexpressionvalidator id="ValFromDate" cssclass="Validator" runat="server"
                                                validationexpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d (0[0-9]|1[0-9]|2[0-4]):([0-5]\d):([0-5]\d)"
                                                controltovalidate="TextBox_FromDate" errormessage="Creation Time From Date is invalid.">*</asp:regularexpressionvalidator>
                                        </td>
                                        <td style="width: 99px; height: 17px">
                                            To(Creation Time) :
                                        </td>
                                        <td style="height: 17px; width: 326px;">
                                            <asp:textbox runat="server" id="TextBox_ToDate" cssclass="datepicker" />
                                            <asp:regularexpressionvalidator id="ValToDate" cssclass="Validator" runat="server"
                                                validationexpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d (0[0-9]|1[0-9]|2[0-4]):([0-5]\d):([0-5]\d)"
                                                controltovalidate="TextBox_ToDate" errormessage="Creation Time To Date is invalid.">*</asp:regularexpressionvalidator>
                                        </td>
                                        <td>
                                            <asp:hyperlink id="HyperLink_newDownload" runat="server" cssclass="Link" navigateurl="newDownload.aspx">Download a new file ...</asp:hyperlink>
                                            &nbsp;
                                        </td>
                                        <td>
                                            <asp:linkbutton id="LinkButton_CaptureScreen" runat="server" text="Capture Screen..."
                                                cssclass="Link" onclick="LinkButton_CaptureScreen_Click"></asp:linkbutton>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 120px; height: 17px">
                                            From(End Time) :
                                        </td>
                                        <td style="width: 190px; height: 17px">
                                            <asp:textbox runat="server" id="TextBox_EndTimeFromDate" cssclass="datepicker" />
                                            <asp:regularexpressionvalidator id="RegularExpressionValidator1" cssclass="Validator"
                                                runat="server" validationexpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d (0[0-9]|1[0-9]|2[0-4]):([0-5]\d):([0-5]\d)"
                                                controltovalidate="TextBox_EndTimeFromDate" errormessage="End Time From Date is invalid.">*</asp:regularexpressionvalidator>
                                        </td>
                                        <td style="width: 99px; height: 17px">
                                            To(End Time) :
                                        </td>
                                        <td style="height: 17px; width: 326px;">
                                            <asp:textbox runat="server" id="TextBox_EndTimeToDate" cssclass="datepicker" />
                                            <asp:regularexpressionvalidator id="RegularExpressionValidator2" cssclass="Validator"
                                                runat="server" validationexpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d (0[0-9]|1[0-9]|2[0-4]):([0-5]\d):([0-5]\d)"
                                                controltovalidate="TextBox_EndTimeToDate" errormessage="End Time To Date is invalid.">*</asp:regularexpressionvalidator>
                                        </td>
                                        <td>
                                            <asp:linkbutton id="LinkButton_ScheduleRestart" runat="server" text="Schedule Restart ..."
                                                cssclass="Link" onclick="LinkButton_ScheduleRestart_Click"></asp:linkbutton>
                                        </td>
                                        <td>
                                            <asp:linkbutton id="LinkButton_GetRunningApp" runat="server" text="Taskbar Running Applications..."
                                                cssclass="Link" onclick="LinkButton_GetRunningApp_Click"></asp:linkbutton>
                                        </td>
                                    </tr>
                                    <tr id="trFileTypes" runat="server">
                                        <td style="width: 120px">
                                            Task Types :
                                        </td>
                                        <td style="width: 190px">
                                            <asp:dropdownlist id="Dropdownlist_taskType" runat="server" enableviewstate="true"
                                                autopostback="true" cssclass="textbox" width="144px" onselectedindexchanged="Dropdownlist_taskType_SelectedIndexChanged">
                                            </asp:dropdownlist>
                                        </td>
                                        <td style="width: 99px">
                                            User :
                                        </td>
                                        <td style="width: 326px">
                                            <asp:dropdownlist id="Dropdownlist_user" runat="server" width="144px" cssclass="textbox">
                                            </asp:dropdownlist>
                                        </td>
                                        <td>
                                            <asp:linkbutton id="LinkButton_DateTimeSync" runat="server" text="Schedule Date Time Sync ..."
                                                cssclass="Link" onclick="LinkButton_DateTimeSync_Click"></asp:linkbutton>
                                        </td>
                                        <td>
                                            <asp:hyperlink id="HyperLink_StartService" runat="server" cssclass="Link" navigateurl="ServiceRequestTask.aspx?t=21">Start Service on ATM...</asp:hyperlink>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 120px">
                                            Task Status :
                                        </td>
                                        <td style="width: 190px">
                                            <asp:dropdownlist id="DropDownList_status" runat="server" enableviewstate="true"
                                                cssclass="textbox" width="144px">
                                            </asp:dropdownlist>
                                        </td>
                                        <td>
                                            ATM Type :
                                        </td>
                                        <td>
                                            <asp:dropdownlist id="DropDownList_ATMType" runat="server" width="144px" cssclass="textbox">
                                            </asp:dropdownlist>
                                        </td>
                                        <td>
                                            <asp:linkbutton id="LinkButton_Configuration" runat="server" text="Schedule Configuration ..."
                                                cssclass="Link" onclick="LinkButton_Configuration_Click"></asp:linkbutton>
                                        </td>
                                        <td>
                                            <asp:hyperlink id="HyperLink_StopService" runat="server" cssclass="Link" navigateurl="ServiceRequestTask.aspx?t=22">Stop Service on ATM...</asp:hyperlink>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 120px">
                                            File Type :
                                        </td>
                                        <td style="width: 190px">
                                            <asp:dropdownlist id="DropDownList_fileType" runat="server" enableviewstate="true"
                                                cssclass="textbox" width="144px">
                                            </asp:dropdownlist>
                                        </td>
                                        <td style="width: 120px">
                                            Specific Word :
                                        </td>
                                        <td style="width: 190px">
                                           <asp:TextBox ID="TextBox_SpecificWord" runat="server" CssClass="textbox"></asp:TextBox>
                                        </td>
                                    </tr>
                                  
                                    <tr>
                                        <td colspan="4" align="right" style="padding-right: 80px">
                                            <asp:button cssclass="button" id="button_search" runat="server" text="Search" onclick="button_search_Click" />
                                            &nbsp;<asp:button id="Button_Reset" runat="server" cssclass="button" onclick="Button_Reset_Click"
                                                text="Reset" />
 
 &nbsp;<asp:button id="Button_MergeAllIntoPDF" runat="server" cssclass="button" onclick="Button_MergeAllIntoPDF_Click"
                                                text="Merge Result As PDF"  />
 
 &nbsp;<asp:button id="Button_MergeAllIntoTxt" runat="server" cssclass="button" onclick="Button_MergeAllIntoTxt_Click"
                                                text="Merge Result As TXT" />
 

                                        </td>
                                        <td>
                                            <asp:linkbutton id="LinkButton_HeartBeatConfiguration" runat="server" text="Schedule Heart Beat Configuration ..."
                                                cssclass="Link" onclick="LinkButton_HeartBeatConfiguration_Click"></asp:linkbutton>
                                        </td>
                                        <td>
                                            <asp:linkbutton id="LinkButton_GetServiceStatus" runat="server" text="Services Status on ATM..."
                                                cssclass="Link" onclick="LinkButton_ServiceStatus_Click"></asp:linkbutton>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            &nbsp;
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                        <td>
                                            <asp:linkbutton id="LinkButton_BatchConf" runat="server" text="Schedule Batch Configuration ..."
                                                cssclass="Link" onclick="LinkButton_BatchConf_Click"></asp:linkbutton>
                                        </td>
                                        <td>
                                            <asp:linkbutton id="LinkButton_initEJ" runat="server" text="Execute initEJ on ATM..."
                                                cssclass="Link" onclick="LinkButton_initEj_Click"></asp:linkbutton>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            &nbsp;
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                        <td>
                                            <asp:linkbutton id="LinkButton_ScheduleInventory" runat="server" text="Schedule Inventory Collection ..."
                                                cssclass="Link" onclick="LinkButton_ScheduleInventory_Click"></asp:linkbutton>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td valign="top">
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
        </table>
        </td> </tr>
        <tr>
            <td align="left">
                &nbsp; &nbsp; &nbsp;&nbsp;
            </td>
        </tr>
        <tr align="left">
            <th colspan="2" align="left">
                Selected Tasks -
                <asp:literal id="Literal_count" runat="server" enableviewstate="False"></asp:literal>
            </th>
        </tr>
        <tr>
            <td valign="top" colspan="2">
                <asp:gridview runat="server" id="GridView_Files" autogeneratecolumns="False" backcolor="White"
                    borderstyle="Solid" borderwidth="1px" cellpadding="3" forecolor="Black" gridlines="Vertical"
                    pagesize="10" width="100%" onrowdatabound="GridView_Files_RowDataBound" allowpaging="true" 
                    allowsorting="true" onpageindexchanging="GridView_Files_PageIndexChanging" onsorting="GridView_Files_Sorting">
                    <AlternatingRowStyle BackColor="#eff0ef" />
                    <EmptyDataTemplate>
                        <center>
                            No Records Found</center>
                    </EmptyDataTemplate>
                    <RowStyle HorizontalAlign="Center" />
                    <Columns>
                        <asp:BoundField DataField="title" HeaderText="ATM" ReadOnly="True" SortExpression="title"
                            ItemStyle-HorizontalAlign="Center">
                            <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                        </asp:BoundField>
                        <asp:BoundField DataField="atm_type" HeaderText="Type" SortExpression="atm_type">
                            <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                        </asp:BoundField>
                        <%--<asp:BoundField DataField="ip" HeaderText="ATM IP" ReadOnly="True" SortExpression="ip"
                            ItemStyle-HorizontalAlign="Center">
                            <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                        </asp:BoundField>--%>
                        <asp:BoundField DataField="file_type_title" HeaderText="Type" SortExpression="file_type_title"
                            Visible="false">
                            <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                        </asp:BoundField>
                        <asp:BoundField DataField="creation_time" HeaderText="Creation Time" SortExpression="creation_time"
                            DataFormatString="{0:dd/MM/yy HH:mm:ss}" HtmlEncode="false">
                            <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                        </asp:BoundField>
                        <asp:BoundField DataField="end_time" HeaderText="End Time" SortExpression="end_time"
                            DataFormatString="{0:dd/MM/yy HH:mm:ss}" HtmlEncode="false">
                            <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                        </asp:BoundField>
                        <asp:BoundField DataField="bytes_transferred" HeaderText="Bytes Transferred" SortExpression="bytes_transferred">
                            <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Status" SortExpression="Status" HeaderText="Status">
                            <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                        </asp:BoundField>
                        <asp:BoundField DataField="task_type_name" SortExpression="task_type_name" HeaderText="Task Type">
                            <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                        </asp:BoundField>
                         <asp:BoundField DataField="file_type_title" SortExpression="file_type_title" HeaderText="File Type">
                            <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                        </asp:BoundField>

                        <asp:TemplateField HeaderText="Failure Reason">
                            <HeaderStyle CssClass="HeadingCell" />
                            <ItemTemplate>
                                <a href="Download_details.aspx?file_id=<%#Eval("task_id") %>">
                                    <%#Eval("reason") %>
                                </a>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Details">
                            <ItemTemplate>
                                <a style="border: 0" href="Download_details.aspx?file_id=<%#Eval("task_id") %>">
                                    <img alt="View Details" src="../images/description.jpg" style="border: 0; width: 18px;
                                        height: 14px" /></a>
                            </ItemTemplate>
                            <HeaderStyle CssClass="HeadingCell" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="View">
                            <ItemTemplate>
                                 <asp:ImageButton ImageUrl="../images/viewfile.jpg" ID="ImageButton_ViewFile" runat="server"
                                    Width="18px" Height="14px" OnClick="ViewFile_Click" CommandArgument='<%#Eval("task_id")%>'>
                                </asp:ImageButton>
                     <%--                       <asp:HyperLink runat="server" ID="hyperlink_view" NavigateUrl='CashDataViewer.aspx?file_id=<%#Eval("file_info_id") %>'>
                                <img alt="View File" src="../images/viewfile.jpg" style="border: 0;width:18px;height:14px"" /></asp:HyperLink>--%>
                            </ItemTemplate>
                            <HeaderStyle CssClass="HeadingCell" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Save as PDF">
                            <ItemTemplate>
                                <asp:ImageButton ImageUrl="../images/saveaspdf.jpg" ID="ImageButton_SaveAsPDF" runat="server"
                                    Width="18px" Height="14px" OnClick="SaveAsPDF_Click" CommandArgument='<%#Eval("task_id")%>'>
                                </asp:ImageButton>
                            </ItemTemplate>
                            <HeaderStyle CssClass="HeadingCell" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Save">
                            <ItemTemplate>
                                <asp:ImageButton ImageUrl="../images/saveastext.jpg" ID="ImageButton_SaveAsTXT" runat="server"
                                    Width="18px" Height="14px" OnClick="SaveAsTXT_Click" CommandArgument='<%#Eval("task_id")%>'>
                                </asp:ImageButton>
                            </ItemTemplate>
                            <HeaderStyle CssClass="HeadingCell" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Reparse">
                            <ItemTemplate>
                                <asp:ImageButton ImageUrl="../images/saveastext.jpg" ID="ImageButton_Reparse" runat="server"
                                    Width="18px" Height="14px" OnClick="ImageButton_Reparse_Click" CommandArgument='<%#Eval("task_id")%>'>
                                </asp:ImageButton>
                            </ItemTemplate>
                            <HeaderStyle CssClass="HeadingCell" />
                        </asp:TemplateField>

                    </Columns>
                    <RowStyle HorizontalAlign="Center" />
                    <FooterStyle BackColor="#CCCCCC" />
                    <SelectedRowStyle BackColor="#000099" Font-Bold="True" ForeColor="White" />
                    <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Left" />
                    <HeaderStyle Font-Bold="True" ForeColor="White" />
                    <%--<AlternatingRowStyle BackColor="#CCCCCC" />--%>
                </asp:gridview>
                &nbsp;
            </td>
        </tr>
    </div>
    <input type="button" runat="server" id="refresh" style="display: none" />
    </form>
</body>
</html>
