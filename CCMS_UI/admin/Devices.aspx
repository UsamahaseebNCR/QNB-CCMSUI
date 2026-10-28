<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Devices.aspx.cs" Inherits="CCMSUI.admin.Devices"
    ValidateRequest="false" %>

<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<%@ Register TagPrefix="DBWC" Namespace="DBauer.Web.UI.WebControls" Assembly="DBauer.Web.UI.WebControls.HierarGrid" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="../content/ISUITE.css" rel="stylesheet" />
    <title>CCMS - Devices</title>
    <script src="../content/shortcut.js" type="text/javascript" language="javascript"></script>
    <script language="javascript" type="text/javascript">

        function ShowErrorMessage(errorMessage) {
            document.getElementById('Label_Dialog_ErrorMessage').innerHTML = errorMessage;
        }

        function RefreshDialog() {
            document.getElementById('Label_Dialog_DeviceId').innerHTML = '';
            document.getElementById('Label_Dialog_DeviceNumber').innerHTML = '';
            document.getElementById('Label_Dialog_ModuleNumber').innerHTML = '';
            document.getElementById('TextBox_Dialog_Name').value = '';
            document.getElementById('DropDownList_Dialog_Vendor').removeAttribute('disabled');
            document.getElementById('Label_Dialog_InternalName').innerHTML = '';
            document.getElementById('Label_Dialog_Replenishable').innerHTML = '';

            SetTabIndex('TextBox_Dialog_Name', 1, true);
            SetTabIndex('DropDownList_Dialog_Vendor', 2, false);
            SetTabIndex('Button_Dialog_Save', 3, false);
            SetTabIndex('Button_Dialog_Back', 4, false);
        }

        function ShowEditDeviceDialog(rowId) {

            Dialog_EditDevice.Show();
            RefreshDialog();
            ShowErrorMessage('');
            //alert(rowId);
            Callback_LoadEditDeviceDialog.Callback(rowId);
        }

        function Button_Dialog_Save_Click() {
            var deviceId = document.getElementById('Label_Dialog_DeviceId').innerHTML;
            var deviceNumber = document.getElementById('Label_Dialog_DeviceNumber').innerHTML;
            var deviceName = document.getElementById('TextBox_Dialog_Name').value;
            var vendorName;
            for (i = 0; i < document.form1.DropDownList_Dialog_Vendor.options.length; i++)
                if (document.form1.DropDownList_Dialog_Vendor.options[i].selected == true)
                    vendorName = document.form1.DropDownList_Dialog_Vendor.options[i].text;

            if (deviceName == '')
                ShowErrorMessage('Please specify Device Name');
            else
                Callback_EditDevice.Callback(deviceId, deviceName, vendorName, deviceNumber);
        }
    </script>
</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)'
    onload="hideProgressBar();">
    <form id="form1" runat="server">
    <div>
        <table style="width: 100%" cellspacing="0" cellpadding="0" border="0" class="mainbox">
            <tr>
                <td class="PageHeading" align="left">
                    Device Management
                </td>
            </tr>
            <tr>
                <td class="ValidationText">
                    <asp:Literal ID="Literal_error" runat="server" EnableViewState="False"></asp:Literal>
                    <asp:ValidationSummary ID="ValidationSummary_ATM" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="ConfirmationText">
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
            <tr align="left">
                <td colspan="2">
                    <table width="100%" class="SEARCHBOX" border="0">
                        <tr>
                            <td>
                                <table id="search" width="100%" border="0">
                                    <tr>
                                        <td style="width: 15%;">
                                            Device Name
                                        </td>
                                        <td style="width: 35%;">
                                            <asp:TextBox ID="TextBox_name" runat="server" CssClass="textbox"></asp:TextBox>
                                            <asp:RegularExpressionValidator ControlToValidate="TextBox_name" ID="RegularExpressionValidator_TextBox_name"
                                                runat="server" ValidationExpression="[^']+" ErrorMessage="Name Is Invalid">*</asp:RegularExpressionValidator>
                                        </td>
                                         <td style="width: 15%;">
                                            Device Number
                                        </td>
                                        <td style="width: 35%;">
                                            <asp:TextBox ID="TextBox_DeviceNo" runat="server" CssClass="textbox"></asp:TextBox>
                                            <asp:RegularExpressionValidator ControlToValidate="TextBox_DeviceNo" ID="RegularExpressionValidator1"
                                                runat="server" ValidationExpression="\d*" ErrorMessage="Invalid Number entered">*</asp:RegularExpressionValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                       <td style="width: 15%;">
                                            Vendor
                                        </td>
                                        <td style="width: 35%;">
                                            <asp:DropDownList ID="DropDownList_vendor" runat="server" CssClass="dropDown" Width="151px">
                                            </asp:DropDownList>
                                       
                                    </tr>
                                   
                                    <tr>
                                        <td>
                                        </td>
                                        <td>
                                            <asp:Button ID="Button_search" runat="server" CssClass="button" Text="Search" OnClick="Button_search_Click" />&nbsp;&nbsp;
                                            <asp:Button ID="Button_reset" runat="server" CssClass="button" Text="Reset" OnClick="Button_reset_Click"
                                                CausesValidation="false" />
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
                    Selected Devices -
                    <asp:Literal ID="Literal_count" runat="server" EnableViewState="False"></asp:Literal>
                </th>
            </tr>
            <tr style="width: 100%">
                <td valign="top">
                    <DBWC:HierarGrid ID="Grid_Devices" runat="server" AutoGenerateColumns="False" Width="100%"
                        AllowSorting="True" OnSortCommand="GridView_Sorting" AllowPaging="True" OnPageIndexChanged="GridView_PageIndexChanging"
                        PageSize="15" BorderStyle="None" TemplateCachingBase="Tablename" LoadControlMode="UserControl"
                        TemplateDataMode="Table" OnTemplateSelection="Grid_Devices_TemplateSelection"
                        HeaderStyle-HorizontalAlign="Center">
                        <PagerStyle CssClass="GridFooterCellClass" Position="Bottom" Mode="NumericPages"
                            PrevPageText="<" NextPageText=">" />
                        <AlternatingItemStyle BackColor="#eff0ef" />
                        <Columns>
                            <asp:BoundColumn DataField="device_id" HeaderText="Id" Visible="false" ItemStyle-Width="10%"
                                SortExpression="device_id"></asp:BoundColumn>
                            <asp:BoundColumn DataField="device_name" HeaderText="Device Name" SortExpression="device_name"
                                ItemStyle-Width="45%"></asp:BoundColumn>
                            <asp:BoundColumn DataField="device_number" HeaderText="Device Number" SortExpression="device_number"
                                ItemStyle-Width="5%" ItemStyle-HorizontalAlign="Center"></asp:BoundColumn>
                            <asp:BoundColumn DataField="vendor_name" HeaderText="Vendor" SortExpression="vendor_name"
                                ItemStyle-Width="12%" ItemStyle-HorizontalAlign="Center"></asp:BoundColumn>
                                <asp:BoundColumn DataField="device_service_1" HeaderText="Device Service 1" SortExpression="device_service_1"
                                ItemStyle-Width="12%" ItemStyle-HorizontalAlign="Left"></asp:BoundColumn>
                                <asp:BoundColumn DataField="device_service_2" HeaderText="Device Service 2" SortExpression="device_service_2"
                                ItemStyle-Width="12%" ItemStyle-HorizontalAlign="Left"></asp:BoundColumn>
                                <asp:BoundColumn DataField="device_service_3" HeaderText="Device Service 3" SortExpression="device_service_3"
                                ItemStyle-Width="12%" ItemStyle-HorizontalAlign="Left"></asp:BoundColumn>

                            <asp:BoundColumn DataField="is_replenishable" HeaderText="Replenishable" SortExpression="is_replenishable"
                                ItemStyle-HorizontalAlign="Center"></asp:BoundColumn>
                            <asp:TemplateColumn HeaderText="Edit" ItemStyle-Width="15%">
                                <ItemTemplate>
                                    <img class="ImageButtonClass" alt="Click to Edit" src="../images/Edit.gif" onclick='javascript:ShowEditDeviceDialog(<%# Eval("device_id") %>)' />
                                </ItemTemplate>
                            </asp:TemplateColumn>
                        </Columns>
                        <HeaderStyle CssClass="HeadingCell1" />
                    </DBWC:HierarGrid>
                </td>
            </tr>
        </table>
    </div>
    <div>
        <ComponentArt:Dialog ID="Dialog_EditDevice" runat="server" Modal="true" AllowDrag="true"
            AllowResize="True" ShowTransition="None" CloseTransition="Fade" TransitionDuration="500"
            Alignment="TopCentre" Width="40%" HeaderCssClass="DialogClass" ContentCssClass="DialogContentClass">
            <Header>
                <table style=" width: 100%;">
                    <tr>
                        <td style="color: White; height: 10px">
                            Device
                        </td>
                        <td style="width: 5%; text-align: right;">
                            <img class="ImageButtonClass" alt="Close" src="../images/dialogimages/close.gif"
                                onclick="Dialog_EditDevice.Close();" />
                        </td>
                    </tr>
                </table>
            </Header>
            <Content>
                <table style="width: 100%;" >
                    <tr class="ValidationTextClass">
                        <td colspan="2">
                            <label id="Label_Dialog_ErrorMessage">
                            </label>
                        </td>
                    </tr>
                    <tr style="height: 0px;">
                        <td style="width: 40%;">
                        </td>
                        <td style="width: 60%;">
                            <%-- <div id="Label_Dialog_DeviceId" style="visibility: hidden;">
                                </div>--%>
                            <label id="Label_Dialog_DeviceId" style="display: none;">
                            </label>
                        </td>
                    </tr>
                    <tr>
                        <td class="DialogContentHeadingCellClass">
                            Device Number
                        </td>
                        <td class="DialogContentRowCellClass">
                            <label id="Label_Dialog_DeviceNumber">
                            </label>
                        </td>
                    </tr>
                    <tr>
                        <td class="DialogContentHeadingCellClass">
                            Module Number
                        </td>
                        <td class="DialogContentRowCellClass">
                            <label id="Label_Dialog_ModuleNumber">
                            </label>
                        </td>
                    </tr>
                    <tr>
                        <td class="DialogContentHeadingCellClass">
                            Name*
                        </td>
                        <td class="DialogContentRowCellClass">
                            <input type="text" id="TextBox_Dialog_Name" class="TextBoxClass" maxlength="256" />
                        </td>
                    </tr>
                    <tr>
                        <td class="DialogContentHeadingCellClass">
                            Vendor Name
                        </td>
                        <td class="DialogContentRowCellClass">
                            <select id="DropDownList_Dialog_Vendor" class="DropDownBoxClass">
                            </select>
                        </td>
                    </tr>
                    <tr>
                        <td class="DialogContentHeadingCellClass">
                            Internal Name
                        </td>
                        <td class="DialogContentRowCellClass">
                            <label id="Label_Dialog_InternalName">
                            </label>
                        </td>
                    </tr>
                    <tr>
                        <td class="DialogContentHeadingCellClass">
                            Replenishable
                        </td>
                        <td class="DialogContentRowCellClass">
                            <label id="Label_Dialog_Replenishable">
                            </label>
                        </td>
                    </tr>
                    <tr class="SearchButtonsPaneClass">
                        <td colspan="2">
                            <input id="Button_Dialog_Save" runat="server" type="button" class="button" value="Save"
                                onclick="javascript:Button_Dialog_Save_Click()" />&nbsp;&nbsp;
                            <input id="Button_Dialog_Back" runat="server" type="button" class="button" value="Cancel"
                                onclick="Dialog_EditDevice.Close();" />
                        </td>
                    </tr>
                </table>
            </Content>
        </ComponentArt:Dialog>
    </div>
    <ComponentArt:CallBack ID="Callback_LoadEditDeviceDialog" runat="server" OnCallback="LoadEditDeviceDialog"
        Height="0px" Width="0px">
    </ComponentArt:CallBack>
    <ComponentArt:CallBack ID="Callback_EditDevice" runat="server" OnCallback="EditDevice"
        Height="0px" Width="0px">
    </ComponentArt:CallBack>
    </form>
</body>
</html>
