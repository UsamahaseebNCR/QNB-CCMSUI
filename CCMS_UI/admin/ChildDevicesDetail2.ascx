<%@ Control Language="C#" AutoEventWireup="true" Codebehind="ChildDevicesDetail2.ascx.cs"
    Inherits="CCMSUI.admin.ChildDevicesDetail2" %>
<%@ Register TagPrefix="DBWC" Namespace="DBauer.Web.UI.WebControls" Assembly="DBauer.Web.UI.WebControls.HierarGrid" %>
<link href="../content/iWatch.css" rel="stylesheet" />
<asp:GridView ID="Grid_Devices2" runat="server" AutoGenerateColumns="false" Width="100%"
    OnDataBinding="Grid_DataBinding" ShowHeader="false" AlternatingRowStyle-BackColor="#e9e9e9"
    ShowFooter="false" onrowdatabound="Grid_Devices_RowDataBound">
    <RowStyle BackColor="#dcdcdc" />
    <Columns>
        <asp:BoundField DataField="" Visible="true" ItemStyle-Width="0%">
			<ItemStyle CssClass="GridRowCellLeftAlignedClass" />
        </asp:BoundField>
        <asp:BoundField DataField="device_id" Visible="false" ItemStyle-Width="0%">
			<ItemStyle CssClass="GridRowCellLeftAlignedClass" />
        </asp:BoundField>
        <asp:BoundField DataField="device_name" Visible="true" ItemStyle-Width="20%">
			<ItemStyle CssClass="GridRowCellLeftAlignedClass" Wrap="true"/>
        </asp:BoundField>
		<asp:BoundField DataField="vendor_name" Visible="false" ItemStyle-Width="10%">
			<ItemStyle CssClass="GridRowCellLeftAlignedClass" Wrap="true"/>
		</asp:BoundField>
		<asp:BoundField DataField="device_state_str" Visible="true" ItemStyle-Width="25%">
			<ItemStyle CssClass="GridRowCellLeftAlignedClass" Wrap="true"/>
		</asp:BoundField>
           <asp:TemplateField  ItemStyle-Width="5%"  >
                                    <ItemTemplate>
                                     <%--Use Device Status as Image-Name.Gif--%>
                                        <div class="<%#DataBinder.Eval(Container.DataItem, "device_state_str").ToString().Replace(" ","-").ToLower()%>">
                                            <asp:Image ID="Image1" runat="server" ImageUrl='<%#DataBinder.Eval(Container.DataItem, "device_state_str").ToString().Replace(" ","-").ToLower().Equals(string.Empty)?"../images/"+"default.gif":"../images/"+DataBinder.Eval(Container.DataItem, "device_state_str").ToString().ToLower()+".gif"%>' />
                                           </div>
      <%--  <asp:ImageField ControlStyle-BorderStyle="None" ItemStyle-CssClass="GridRowCellLeftAlignedClass statusIcon"  ControlStyle-CssClass="GridRowCellLeftAlignedClass"  dataimageurlfield="device_state_str"
            dataimageurlformatstring="~\Images\{0}.gif"
            alternatetext=""
            nulldisplaytext=""
            headertext=""  ItemStyle-BorderWidth=""
            readonly="true"/>--%>
            </ItemTemplate>
            </asp:TemplateField>
		<asp:BoundField DataField="device_state_desc" Visible="true" ItemStyle-Width="50%">
			<ItemStyle CssClass="GridRowCellLeftAlignedClass" Wrap="true"/>
		</asp:BoundField>
    </Columns>
</asp:GridView>