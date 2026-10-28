<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddEditDashboardGraph.aspx.cs" Inherits="CCMSUI.admin.AddEditDashboardGraph" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Add Edit Template</title>
    <script src="../content/Scripts/jquery-1.7.1.min.js" type="text/javascript"></script>
    <script src="../content/shortcut.js" type="text/javascript" language="javascript"></script>
    <script src="../content/farbtastic/farbtastic.js" type="text/javascript" language="javascript"></script>
    <script type="text/javascript">
        window.parent.window.contentPage = '';
        $(document).ready(function () {
            $('#colorpicker').farbtastic('#TextBox_BackColor');
            //$('#colorpicker').farbtastic(function (color) {
            //    console.log('The user has just selected the following color: ' + color);
            //    // setting input value
            //    $('#TextBox_BackColor').text(color);
            //    $('#TextBox_BackColor').val(color);
            //});
        });
    </script>
    <link href="../content/iSuite.css" rel="stylesheet" />
    <link href="../content/farbtastic/farbtastic.css" rel="stylesheet" />
</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)' onload="hideProgressBar();">
    <form id="form1" runat="server">
    <table cellpadding="0" cellspacing="0" border="0" class="MainBox" width="100%" >
        <tr>
            <td class="PageHeading" align="left" colspan="2" style="height: 25px">
                <%=Request.QueryString["id"] == null?"Add New Graph ":"Edit Graph"%>
            </td>
        </tr>
        <tr>
            <td class="SearchHeadPaneGradient" colspan="2">
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:ValidationSummary ID="ValidationSummary1" runat="server"></asp:ValidationSummary>
            </td>
        </tr>
        <tr style="height: 20px">
            <td colspan="2" style="color: red;" align="center" valign="top">
                <asp:Literal ID="Literal_error" runat="server" EnableViewState="False"></asp:Literal>
            </td>
        </tr>
        <tr>
            <td>
                <table id="inner" border="0" cellpadding="0" cellspacing="0" width ="100%" style="padding-left:10px">
                    <tr" >
                        <td>
                            Dashboard <span style="color: Red">*</span>
                        </td>
                        <td>
                            <asp:DropDownList ID="DropDownList_Dashboard" runat="server" CssClass="textbox">
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator ID="requiredfieldvalidator_Dashboard" runat="server" ControlToValidate="DropDownList_Dashboard"
                                ErrorMessage="Please select dashboard." Text="*"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr style="background-color: #eff0ef;" >
                        <td style="width:15%">
                            Title <span style="color: Red">*</span>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_Name" runat="server" CssClass="textbox"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="requiredfieldvalidator_Name" runat="server" ControlToValidate="TextBox_Name"
                                ErrorMessage="Please enter chart Name" Text="*"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            SQL Query <span style="color: Red">*</span>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_Query" runat="server" CssClass="textbox" TextMode="MultiLine" Rows="5" Width="40%"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="requiredfieldvalidator_Query" runat="server" ControlToValidate="TextBox_Query"
                                ErrorMessage="Please enter Query." Text="*"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr style="background-color: #eff0ef;" >
                        <td>
                            Type <span style="color: Red">*</span>
                        </td>
                        <td>
                            <asp:DropDownList ID="DropDownList_Type" runat="server" CssClass="textbox">
                                                <asp:ListItem Selected="True" Value="">Select</asp:ListItem>
                                                <asp:ListItem Value="BarGraph">Bar Graph</asp:ListItem>
                                                <asp:ListItem Value="PieChart">Pie Chart</asp:ListItem>
                                                <asp:ListItem Value="ColumnChart">Column Chart</asp:ListItem>
                                            </asp:DropDownList>
                            <asp:RequiredFieldValidator ID="requiredfieldvalidator_Type" runat="server" ControlToValidate="DropDownList_Type"
                                ErrorMessage="Please select Type." Text="*"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td style="width:15%">
                            X Value <span style="color: Red">*</span>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_XValue" runat="server" CssClass="textbox"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="requiredfieldvalidator_XValue" runat="server" ControlToValidate="TextBox_XValue"
                                ErrorMessage="Please enter Report Name" Text="*"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr style="background-color: #eff0ef;" >
                        <td style="width:15%">
                            Y Value <span style="color: Red">*</span>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_YValue" runat="server" CssClass="textbox"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="requiredfieldvalidator_YValue" runat="server" ControlToValidate="TextBox_YValue"
                                ErrorMessage="Please enter Report Name" Text="*"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Row <span style="color: Red">*</span>
                        </td>
                        <td>
                            <asp:DropDownList ID="DropDownList_RowNumber" runat="server" CssClass="textbox">
                                                <asp:ListItem Selected="True" Value="">Select</asp:ListItem>
                                                <asp:ListItem Value="1">1</asp:ListItem>
                                                <asp:ListItem Value="2">2</asp:ListItem>
                                                <asp:ListItem Value="3">3</asp:ListItem>
                                            </asp:DropDownList>
                            <asp:RequiredFieldValidator ID="requiredfieldvalidator_RowNumber" runat="server" ControlToValidate="DropDownList_RowNumber"
                                ErrorMessage="Please select Row Number." Text="*"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr style="background-color: #eff0ef;" >
                        <td style="width:15%">
                            Background Color <span style="color: Red">*</span>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_BackColor" runat="server" CssClass="textbox"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="requiredfieldvalidator_BackColor" runat="server" ControlToValidate="TextBox_BackColor"
                                ErrorMessage="Please select color" Text="*"></asp:RequiredFieldValidator>
                            <div id="colorpicker"></div>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Active
                        </td>
                        <td>
                            <asp:RadioButtonList ID="RadioButton_Active" runat="server" RepeatDirection="Horizontal">
                                <asp:ListItem Value="1">Active</asp:ListItem>
                                <asp:ListItem Value="0">Inactive</asp:ListItem>
                            </asp:RadioButtonList>
                        </td>
                    </tr>
                    
                    <tr>
                        <td colspan="2" style="font-weight: bold; padding-top: 30px;">
                            <asp:Button ID="Button_SaveUpdateTemplate" runat="server" Text="Save" OnClick="Button_SaveUpdateTemplate_Click"
                                CssClass="button" Width="60px" />&nbsp;
                            <asp:Button ID="Button_PreviewQuery" UseSubmitBehavior="false" runat="server" Text="Preview" 
                                OnClick="Button_PreviewQuery_Click" CausesValidation="false" CssClass="button" Width="60px" />&nbsp;
                            <input id="Button_Cancel" type="button" value="Cancel" onclick="window.location = 'dashboardGraphtemplate.aspx'"
                                class="button" />
                        </td>
                    </tr>
                    <tr id="preview" runat="server" visible="false"> 
                        <td>
                            Preview Data
                        </td>
                        <td id="preview_data" runat="server" style="overflow:auto">
                            <div id="chart" runat="server"></div>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <br />
    <br />
    </form>
</body>
</html>