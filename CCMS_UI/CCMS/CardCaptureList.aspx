<%@ Page Language="C#" AutoEventWireup="true" Codebehind="CardCaptureList.aspx.cs"
    Inherits="CCMSUI.CCMS.CardCaptureList" Culture="fr-FR" %>

<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Card Capture List</title>

    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>

    <script type="text/javascript">
        window.parent.window.SetContentPage( window.location);
        
        //window.parent.window.contentPage = '';  
    </script>

    <%--<script src="../content/main.js" type="text/javascript" language="javascript" >
</script>
--%>
    <link href="../content/iSuite.css" rel="stylesheet" />
    <link href="../content/cal/popcalendar.css" type="text/css" rel="stylesheet" />

    <script language="javascript" src="../content/cal/popcalendar.js" type="text/javascript"></script>

</head>
<body onkeydown="checkForCtrlKey(window.event);" onkeyup="checkForCharKey(window.event)">
    <form id="form1" runat="server" autocomplete="off">
        <div>
            <table id="table2" cellspacing="0" cellpadding="0" width="100%" border="0" <%--style="padding-left: 20px"--%>>
                <tr>
                    <td class="PageHeading" align="left" colspan="2">
                        Card Capture Information
                    </td>
                </tr>
                <tr style="height: 20px">
                    <td class="SearchHeadPaneGradient" align="center" colspan="2">
                        <div class="ValidationText">
                            <asp:Literal ID="Literal_errorMsg" runat="server" EnableViewState="False"></asp:Literal></div>
                        <div class="ConfirmationText">
                            <asp:Literal ID="Literal_ok" runat="server" EnableViewState="False"></asp:Literal></div>
                        <asp:ValidationSummary ID="Validation_Replenishment" runat="server" />
                    </td>
                </tr>
                <tr>
                    <th align="left">
                        Search Filters - Please, Select ATM or region first from the&nbsp;tree&nbsp;to the
                        Left of this page.
                    </th>
                </tr>
                <tr>
                    <td valign="top">
                        <table id="Table3" style="width: 100%; padding-left: 8px;" cellspacing="0" cellpadding="0"
                            border="0">
                            <tr>
                                <td style="width: 15%">
                                    From Date:</td>
                                <td style="width: 313px">
                                    <asp:TextBox ID="txtFromDate" runat="server" CssClass="textbox" Font-Size="X-Small"></asp:TextBox>&nbsp;
                                    <img alt="From Date" style="cursor: hand" onclick="popUpCalendar(this,document.getElementById('<% =txtFromDate.ClientID %>'),'dd/mm/yyyy');"
                                        src="../images/IconCalendar.gif" border="0" align="middle" />                                    
                                    
                                </td>
                                <td style="width: 15%">
                                    To Date:</td>
                                <td style="width: 35%">
                                    <asp:TextBox ID="txtToDate" runat="server" CssClass="textbox" Font-Size="X-Small"></asp:TextBox>&nbsp;
                                    <img alt="To Date" style="cursor: hand" onclick="popUpCalendar(this,document.getElementById('<% =txtToDate.ClientID %>'),'dd/mm/yyyy');"
                                        src="../images/IconCalendar.gif" border="0" align="middle" />
                                    
                                    <asp:CompareValidator ID="CompareValidator_ToDateValidity" runat="server" ControlToCompare="txtFromDate"
                                        ControlToValidate="txtToDate" ErrorMessage="To Date cannot be less than From Date"
                                        Operator="GreaterThanEqual" Type="Date">*</asp:CompareValidator>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    From Time (HH:MM:SS) :</td>
                                <td>
                                    <asp:TextBox ID="txt_FromTime" runat="server" CssClass="textbox" MaxLength="8" Font-Size="X-Small">00:00:00</asp:TextBox><asp:RegularExpressionValidator
                                        ID="regValtxtFromTime" runat="server" CssClass="ValidationText" ValidationExpression="[0-2][0-9]:[0-5][0-9]:[0-5][0-9]"
                                        ErrorMessage="Only 8 characters are allowed in From Time." ControlToValidate="txt_FromTime">«</asp:RegularExpressionValidator></td>
                                <td>
                                    To Time (HH:MM:SS) :</td>
                                <td>
                                    <asp:TextBox ID="txt_ToTime" runat="server" CssClass="textbox" MaxLength="8" Font-Size="X-Small">23:59:59</asp:TextBox><asp:RegularExpressionValidator
                                        ID="regValtxtToTime" runat="server" CssClass="ValidationText" ValidationExpression="[0-2][0-9]:[0-5][0-9]:[0-5][0-9]"
                                        ErrorMessage="Only 8 characters are allowed in To Time." ControlToValidate="txt_ToTime">«</asp:RegularExpressionValidator></td>
                            </tr>
                            <tr>
                                <td>
                                    PAN :</td>
                                <td>
                                    <asp:TextBox ID="txtPAN" runat="server" CssClass="textbox" Font-Size="X-Small"></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" CssClass="Validator"
                                        runat="server" ValidationExpression="[0-9]{16,19}" ControlToValidate="txtPAN"
                                        ErrorMessage="Invalid PAN Format.">*</asp:RegularExpressionValidator></td>
                                     <td style="height: 18px">
                                    ATM Type :</td>
                                <td style="height: 18px">
                                    <asp:DropDownList ID="DropDownList_ATMType" runat="server" Width="116px"  CssClass="textbox" Font-Size="X-Small">
                                    </asp:DropDownList>
                                </td>
                                
                            </tr>
                            
                             <tr>
                           <td>
                                    From TSN :</td>
                                <td>
                                    <asp:TextBox ID="txt_FromTSN" runat="server" CssClass="textbox" Font-Size="X-Small"></asp:TextBox><asp:RegularExpressionValidator
                                        ID="RegularExpressionValidator2" CssClass="Validator" runat="server" ValidationExpression="[0-9]*"
                                        ControlToValidate="txt_FromTSN" ErrorMessage="Invalid TSN Format.">*</asp:RegularExpressionValidator></td>
                                        <td>
                                    To TSN :</td>
                                <td>
                                    <asp:TextBox ID="txt_ToTSN" runat="server" CssClass="textbox" Font-Size="X-Small"></asp:TextBox><asp:RegularExpressionValidator
                                        ID="RegularExpressionValidator3" CssClass="Validator" runat="server" ValidationExpression="[0-9]*"
                                        ControlToValidate="txt_ToTSN" ErrorMessage="Invalid TSN Format.">*</asp:RegularExpressionValidator></td>
                                        
                            </tr>
                            <tr>
                            <td>
                            Capture Reason:
                            </td>
                            <td>
                            <asp:DropDownList ID="ddlCaptureReason" runat="server" Width="200px" Font-Size="X-Small"></asp:DropDownList>
                            </td>
                            <td>
                            PAN Prefix
                            </td>
                            <td>
                            <asp:DropDownList ID="ddlPANPrefix" runat="server" Width="114px" Font-Size="X-Small"></asp:DropDownList>
                            </td>
                            </tr>
                            <tr>
                            <td>
                            Bank Name:
                            </td>
                            <td>
                            <asp:DropDownList ID="ddlBankName" runat="server" Width="114px" Font-Size="X-Small"></asp:DropDownList>
                            </td>
                            <td>
                            Switch Name:
                            </td>
                            <td>
                            <asp:DropDownList ID="ddlSwitchName" runat="server" Width="114px" Font-Size="X-Small"></asp:DropDownList>
                            </td>
                            </tr>
                            <tr>
                                <td colspan="4" align="right" style="height: 20px">
                                    <asp:Button ID="btnSearch" runat="server" class="button" Width="95px" Text="Search"
                                        OnClick="btnSearch_Click"></asp:Button>
                                    &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;</td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <th align="left">
                        Search Result -
                        <asp:Literal ID="Literal_count" runat="server" EnableViewState="False"></asp:Literal></th>
                </tr>
                <tr>
                    <td valign="top" width="100%">
                    </td>
                </tr>
                <tr>
                    <td align="left">
                        <table width="100%" cellpadding="0" cellspacing="0" border="0">
                            <tr>
                                <td style="width:100%">
                                    <ComponentArt:Grid ID="GridCapturedCardList" RunningMode="Client" EnableViewState="true"
                                        AllowPaging="true" CssClass="Grid" FooterCssClass="GridFooter" PagerTextCssClass="PagerText"
                                        PagerStyle="Numbered" PageSize="25" ImagesBaseUrl="images/Grid" Width="100%"
                                        runat="server" EmptyGridText="No Record Found" AllowHorizontalScrolling="true"
                                        AutoAdjustPageSize="True" ColumnResizeDistributeWidth="true">
                                        <Levels>
                                            <ComponentArt:GridLevel RowCssClass="Row" AlternatingRowCssClass="AlternatingRow"
                                                DataCellCssClass="DataCell" HeadingCellCssClass="HeadingCell" HeadingTextCssClass="HeadingCellText"
                                                SortAscendingImageUrl="asc.gif" SortDescendingImageUrl="desc.gif" SortImageWidth="10"
                                                SortImageHeight="9" SelectedRowCssClass="SelectedRow">
                                                <Columns>
                                                    <ComponentArt:GridColumn DataField="title" HeadingText="ATM" />
                                                    <ComponentArt:GridColumn DataField="atm_type" HeadingText="Type" />
                                                    <ComponentArt:GridColumn DataField="capture_time" HeadingText="DateTime" FormatString="dd/MM/yy HH:mm"  />
                                                    <ComponentArt:GridColumn DataField="PAN" HeadingText="PAN" />
                                                    <ComponentArt:GridColumn DataField="TSN" HeadingText="TSN"/>
                                                    <ComponentArt:GridColumn DataField="capture_reason" HeadingText="Capture Reason"/>
                                                    <ComponentArt:GridColumn DataField="pan_prefix" HeadingText="PAN Prefix"/>
                                                    <ComponentArt:GridColumn DataField="bank_name" HeadingText="Bank Name"/>
                                                    <ComponentArt:GridColumn DataField="switch_name" HeadingText="Switch Name"/>
                                                </Columns>
                                            </ComponentArt:GridLevel>
                                        </Levels>
                                    </ComponentArt:Grid>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
        <input type="button" runat="server" id="refresh" style="width: 0px; height: 0px;visibility:hidden" />
    </form>
</body>
</html>
