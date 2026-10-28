<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ChildDevicesDetail.ascx.cs"
    Inherits="CCMSUI.admin.ChildDevicesDetail" %>
<%@ Register TagPrefix="DBWC" Namespace="DBauer.Web.UI.WebControls" Assembly="DBauer.Web.UI.WebControls.HierarGrid" %>
<link href="../content/iWatch.css" rel="stylesheet" />
<asp:GridView ID="Grid_Devices" runat="server" AutoGenerateColumns="false" Width="978px"
    OnDataBinding="Grid_DataBinding" ShowHeader="false" AlternatingRowStyle-BackColor="#e9e9e9"
    ShowFooter="false" OnRowDataBound="Grid_Devices_RowDataBound">
    <RowStyle BackColor="#dcdcdc" />
    <Columns>
        <asp:BoundField DataField="" Visible="true" ItemStyle-Width="30px">
            <ItemStyle CssClass="GridRowCellLeftAlignedClass" />
        </asp:BoundField>
        <asp:BoundField DataField="device_id" Visible="false" ItemStyle-Width="5px">
            <ItemStyle CssClass="GridRowCellLeftAlignedClass" />
        </asp:BoundField>
        <asp:BoundField DataField="device_name" Visible="true" ItemStyle-Width="250px">
            <ItemStyle CssClass="GridRowCellLeftAlignedClass" Wrap="true" />
        </asp:BoundField>
        <asp:BoundField DataField="vendor_name" Visible="false" ItemStyle-Width="0px">
            <ItemStyle CssClass="GridRowCellLeftAlignedClass" Wrap="true" />
        </asp:BoundField>
        <asp:BoundField DataField="device_state_str" Visible="true" ItemStyle-Width="100px">
            <ItemStyle CssClass="GridRowCellLeftAlignedClass" Wrap="true" />
        </asp:BoundField>
        <asp:TemplateField ItemStyle-Width="15px">
            <ItemTemplate>
                <%--Use Device Status as Image-Name.Gif--%>
                <div class="<%#DataBinder.Eval(Container.DataItem, "device_state_str").ToString().Replace(" ","-").ToLower()%>">
                    <%--<asp:Image ID="Image1" runat="server" ImageUrl='<%#DataBinder.Eval(Container.DataItem, "device_state_str").ToString().Replace(" ","-").ToLower().Equals(string.Empty)?"../images/"+"default.gif":"../images/"+DataBinder.Eval(Container.DataItem, "device_state_str").ToString().ToLower()+".gif"%>' />--%>
                    <asp:Image ID="Image1" runat="server"/>
                </div>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:BoundField DataField="device_state_desc" Visible="true" ItemStyle-Width="420">
            <ItemStyle CssClass="GridRowCellLeftAlignedClass" Wrap="true" />
        </asp:BoundField>
    </Columns>
</asp:GridView>
