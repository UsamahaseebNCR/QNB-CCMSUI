<%@ Control Language="C#" AutoEventWireup="true" Codebehind="ChildDevices.ascx.cs"
    Inherits="CCMSUI.admin.ChildDevices" %>
<%@ Register TagPrefix="DBWC" Namespace="DBauer.Web.UI.WebControls" Assembly="DBauer.Web.UI.WebControls.HierarGrid" %>
<link href="../content/iwatch.css" rel="stylesheet" />
<asp:GridView ID="Grid_Devices" runat="server" AutoGenerateColumns="false" Width="100%"
    OnDataBinding="Grid_DataBinding" ShowHeader="false"
    ShowFooter="false">
    <RowStyle BackColor="#dcdcdc" />
    <Columns>
        <asp:BoundField DataField="" Visible="true" ItemStyle-Width="10%">
			<ItemStyle CssClass="GridRowCellLeftAlignedClass" />
        </asp:BoundField>
        <asp:BoundField DataField="device_id" Visible="false" ItemStyle-Width="5%">
			<ItemStyle CssClass="GridRowCellLeftAlignedClass" />
        </asp:BoundField>
        <asp:BoundField DataField="device_name" Visible="true" ItemStyle-Width="85%">
			<ItemStyle CssClass="GridRowCellLeftAlignedClass" Wrap="true"/>
        </asp:BoundField>
        <asp:TemplateField ItemStyle-Width="5%">
         <ItemStyle CssClass="GridRowCellCenteredClass" />
            <ItemTemplate>
                <img class="ImageButtonClass" alt="Click to Edit" src="../images/Edit.gif" onclick='javascript:ShowEditDeviceDialog(<%# Eval("device_id") %>)' />
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
