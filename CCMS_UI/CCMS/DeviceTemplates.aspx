<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DeviceTemplates.aspx.cs"
    ValidateRequest="false" Inherits="CCMSUI.CCMS.DeviceTemplates" %>

<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="../content/ISUITE.css" rel="stylesheet" />
    <title>CCMS - Device Templates</title>
    <script src="../content/shortcut.js" type="text/javascript" language="javascript"></script>
</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)'
    onload="hideProgressBar();">
    <form id="form1" runat="server">
    <div>
        <table class="mainbox" style="width: 100%" cellspacing="0" cellpadding="0" border="0">
            <tr>
                <th class="PageHeading" align="left">
                    Device Templates Management
                </th>
            </tr>
            <tr>
                <td class="ValidationText">
                    <asp:Literal ID="Literal_error" runat="server" EnableViewState="False"></asp:Literal>
                    <asp:ValidationSummary ID="ValidationSummary_ATM" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="ConfirmationText">
                    <%=Session["msg"]%>
                    <%Session["msg"] = "";%>
                    <asp:Literal ID="Literal_ok" runat="server" EnableViewState="False"></asp:Literal>
                </td>
            </tr>
            <tr align="left" style="background-color: WHITE">
                <td style="height: 20px; padding-left: 5px;" colspan="2">
                    <b>Search Filters</b>
                </td>
            </tr>
            <tr align="left">
                <td colspan="2">
                    <table style="width: 100%;" class="SEARCHBOX" border="0">
                        <tr>
                            <td>
                                <table style="width: 100%;">
                                    <tr>
                                        <td style="width: 15%;">
                                            Name
                                        </td>
                                        <td style="width: 35%;">
                                            <asp:TextBox ID="TextBox_name" runat="server" CssClass="textbox"></asp:TextBox>&nbsp;
                                            <asp:RegularExpressionValidator ControlToValidate="TextBox_name" ID="RegularExpressionValidator_TextBox_name"
                                                runat="server" ValidationExpression="[^']+" ErrorMessage="Name Is Invalid">*</asp:RegularExpressionValidator>
                                        </td>
                                        <td style="width: 15%;">
                                            Status
                                        </td>
                                        <td style="width: 35%;">
                                            <asp:DropDownList ID="DropDownList_status" runat="server" CssClass="dropDown">
                                                <asp:ListItem Text="All" Value="0,1" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="Active" Value="1"></asp:ListItem>
                                                <asp:ListItem Text="Inactive" Value="0"></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right" colspan="4">
                                            <asp:Button ID="Button_search" runat="server" CssClass="button" Text="Search" OnClick="Button_search_Click" />&nbsp;&nbsp;
                                            <asp:Button ID="Button_reset" runat="server" CssClass="button" Text="Reset" OnClick="Button_reset_Click"
                                                CausesValidation="false" />&nbsp;&nbsp;
                                            <asp:HyperLink ID="HyperLink_newDeviceTemplate" runat="server" CssClass="HyperlinkClass"
                                                NavigateUrl="../admin/AddEditDeviceTemplate.aspx" ForeColor="Red">Add Device Template... </asp:HyperLink>
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
            <tr align="left">
                <th colspan="2" align="left">
                    Selected Devices Templates-
                    <asp:Literal ID="Literal_count" runat="server" EnableViewState="False"></asp:Literal>
                </th>
            </tr>
            <tr style="width: 100%">
                <td>
                    <asp:GridView ID="GridView_DeviceTemplates" runat="server" AutoGenerateColumns="False"
                        Width="100%" AllowSorting="true" OnSorting="GridView_Sorting" AllowPaging="True"
                        OnPageIndexChanging="GridView_PageIndexChanging" PageSize="15" OnRowCreated="GridView_RowCreated"
                        OnRowDataBound="GridView_RowDataBound" OnRowCommand="GridView_RowCommand" HeaderStyle-CssClass="HeadingCell1">
                        <AlternatingRowStyle BackColor="#eff0ef" />
                        <EmptyDataTemplate>
                            No data found!</EmptyDataTemplate>
                        <PagerStyle CssClass="GridFooterCellClass" />
                        <Columns>
                            <asp:BoundField DataField="device_template_name" HeaderText="Name" SortExpression="device_template_name"
                                ItemStyle-Width="40%" HeaderStyle-CssClass="HeadingCell"></asp:BoundField>
                            <asp:BoundField DataField="device_template_desc" HeaderText="Description" SortExpression="device_template_desc"
                                ItemStyle-Width="45%" HeaderStyle-CssClass="HeadingCell"></asp:BoundField>
                            <asp:TemplateField HeaderText="Status" ItemStyle-Width="10%" HeaderStyle-CssClass="HeadingCell" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:ImageButton ID="Button_Activate" runat="server" CommandName="Activate" CommandArgument='<%# Eval("device_template_id") %>'
                                        ImageUrl="../images/deactivate.jpg" ToolTip='Click to Activate' CssClass="ImageButtonClass" />
                                    <asp:ImageButton ID="Button_Deactivate" runat="server" CommandName="Deactivate" CommandArgument='<%# Eval("device_template_id") %>'
                                        ImageUrl="../images/activate.jpg" ToolTip='Click to Deactivate' CssClass="ImageButtonClass" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Edit" ItemStyle-Width="5%" HeaderStyle-CssClass="HeadingCell" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <a href='../admin/AddEditDeviceTemplate.aspx?device_template_id=<%# Eval("device_template_id") %>'>
                                        <img alt="Click to Edit" src="../images/Edit.gif" class="ImageButtonClass" style="border: 0" />
                                    </a>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Delete" HeaderStyle-HorizontalAlign="Left" HeaderStyle-CssClass="HeadingCell" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:ImageButton ID="img_delete" runat="server" CommandName="deleteDeviceTemplate"
                                        CommandArgument='<%# Eval("device_template_id") %>' ToolTip='Click to delete device template'
                                        ImageUrl="../images/deactivate.gif" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
