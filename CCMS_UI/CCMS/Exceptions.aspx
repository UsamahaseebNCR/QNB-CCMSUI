<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Exceptions.aspx.cs" Inherits="CCMSUI.CCMS.Exceptions" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
<h1><center>Exceptions</center></h1>
    <form id="form1" runat="server">
    <div>
        <table>
            <tr>
                <td>
                    <asp:GridView ID="GridView_MachineType" runat="server" Width="100%" CellPadding="4"
                        ForeColor="#333333" GridLines="None" AutoGenerateColumns="false" AllowSorting="true">
                        <Columns>
                            <asp:BoundField DataField="function" HeaderText="Function" ItemStyle-HorizontalAlign="Center"
                                SortExpression="function" />
                            <asp:BoundField DataField="EI" HeaderText="EI" ItemStyle-HorizontalAlign="Center"
                                SortExpression="EI" />
                            <asp:BoundField DataField="ENBD" HeaderText="ENBD" ItemStyle-HorizontalAlign="Center"
                                SortExpression="ENBD" />
                            <asp:BoundField DataField="total" HeaderText="Grand Total" ItemStyle-HorizontalAlign="Center"
                                SortExpression="total" />
                        </Columns>
                        <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                        <EditRowStyle BackColor="#999999" />
                        <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                        <HeaderStyle BackColor="Green" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                        <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                        <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                        <SortedAscendingCellStyle BackColor="#E9E7E2" />
                        <SortedAscendingHeaderStyle BackColor="#506C8C" />
                        <SortedDescendingCellStyle BackColor="#FFFDF8" />
                        <SortedDescendingHeaderStyle BackColor="#6F8DAE" />
                    </asp:GridView>
                </td>
                <td>
                    <asp:Chart ID="Chart_MachineType" runat="server" Width="951px" BorderColor="#B54001">
                        <Titles>
                            <asp:Title Font="Trebuchet MS, 14.25pt, style=Bold" ForeColor="26, 59, 105" Text="Type of Machines">
                            </asp:Title>
                        </Titles>
                        <Legends>
                            <asp:Legend BackColor="Transparent" Alignment="Near" Docking="Top" Font="Trebuchet MS, 8.25pt, style=Bold"
                                IsTextAutoFit="true" Name="Default" LegendStyle="Row" Title="Legends" Enabled="true">
                            </asp:Legend>
                        </Legends>
                        <BorderSkin SkinStyle="None"></BorderSkin>
                        <Series>
                            <asp:Series ChartArea="ChartArea1" BorderColor="180, 26, 59, 105" ChartType="Column"
                                BorderWidth="4" MarkerSize="4" MarkerStyle="Circle" IsValueShownAsLabel="true"
                                Name="EI">
                            </asp:Series>
                              <asp:Series ChartArea="ChartArea1" BorderColor="180, 26, 59, 105" ChartType="Column"
                                BorderWidth="4" MarkerSize="4" MarkerStyle="Circle" IsValueShownAsLabel="true"
                                Name="ENBD">
                            </asp:Series>
                         
                        </Series>
                        <ChartAreas>
                            <asp:ChartArea Name="ChartArea1" BackColor="White">
                                <%--<Area3DStyle Rotation="5" Perspective="10" Enable3D="false" LightStyle="Realistic"
                                    Inclination="2" PointDepth="200" IsRightAngleAxes="true" WallWidth="0" IsClustered="False" />--%>
                                <%--<Position Y="2" Height="94" Width="94" X="2"></Position>--%>
                                <Area3DStyle Rotation="25" Perspective="9" LightStyle="Realistic" Inclination="40"
                                    IsRightAngleAxes="False" WallWidth="3" IsClustered="False" />
                                <AxisY LineColor="64, 64, 64, 64">
                                    <LabelStyle Font="Trebuchet MS, 11.25pt, style=Bold" />
                                    <MajorGrid LineColor="Gray" />
                                </AxisY>
                                <AxisX LineColor="64, 64, 64, 64">
                                    <LabelStyle Font="Trebuchet MS, 11.25pt, style=Bold" />
                                    <MajorGrid LineColor="Gray" />
                                </AxisX>
                            </asp:ChartArea>
                        </ChartAreas>
                    </asp:Chart>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:GridView ID="GridView_EmiratesWise" runat="server" Width="100%" CellPadding="4"
                        ForeColor="#333333" GridLines="None" AutoGenerateColumns="false" AllowSorting="true">
                        <Columns>
                            <asp:BoundField DataField="emirate" HeaderText="Emirate" ItemStyle-HorizontalAlign="Center"
                                SortExpression="emirate" />
                            <asp:BoundField DataField="branch" HeaderText="Branch" ItemStyle-HorizontalAlign="Center"
                                SortExpression="branch" />
                            <asp:BoundField DataField="offsite" HeaderText="Offsite" ItemStyle-HorizontalAlign="Center"
                                SortExpression="offsite" />
                            <asp:BoundField DataField="total" HeaderText="total" ItemStyle-HorizontalAlign="Center"
                                SortExpression="total" />
                        </Columns>
                        <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                        <EditRowStyle BackColor="#999999" />
                        <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                        <HeaderStyle BackColor="Green" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                        <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                        <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                        <SortedAscendingCellStyle BackColor="#E9E7E2" />
                        <SortedAscendingHeaderStyle BackColor="#506C8C" />
                        <SortedDescendingCellStyle BackColor="#FFFDF8" />
                        <SortedDescendingHeaderStyle BackColor="#6F8DAE" />
                    </asp:GridView>
                </td>
                <td>
                    <asp:Chart ID="Chart_EmiratesWise" runat="server" Width="951px" BorderColor="#B54001">
                        <Titles>
                            <asp:Title Font="Trebuchet MS, 14.25pt, style=Bold" ForeColor="26, 59, 105" Text ="Emirates Wise Distribution">
                            </asp:Title>
                        </Titles>
                        <Legends>
                            <asp:Legend BackColor="Transparent" Alignment="Near" Docking="Top" Font="Trebuchet MS, 8.25pt, style=Bold"
                                IsTextAutoFit="true" Name="Default" LegendStyle="Row" Title="Legends" Enabled="true">
                            </asp:Legend>
                        </Legends>
                        <BorderSkin SkinStyle="None"></BorderSkin>
                        <Series>
                            <asp:Series ChartArea="ChartArea1" BorderColor="180, 26, 59, 105" ChartType="Bar"
                                BorderWidth="4" MarkerSize="4" MarkerStyle="Circle" IsValueShownAsLabel="true"
                                Name="EI">
                            </asp:Series>
                              <asp:Series ChartArea="ChartArea1" BorderColor="180, 26, 59, 105" ChartType="Bar"
                                BorderWidth="4" MarkerSize="4" MarkerStyle="Circle" IsValueShownAsLabel="true"
                                Name="ENBD">
                            </asp:Series>
                        </Series>
                        <ChartAreas>
                            <asp:ChartArea Name="ChartArea1" BackColor="White">
                                <%--<Area3DStyle Rotation="5" Perspective="10" Enable3D="false" LightStyle="Realistic"
                                    Inclination="2" PointDepth="200" IsRightAngleAxes="true" WallWidth="0" IsClustered="False" />--%>
                                <%--<Position Y="2" Height="94" Width="94" X="2"></Position>--%>
                                <Area3DStyle Rotation="25" Perspective="9" LightStyle="Realistic" Inclination="40"
                                    IsRightAngleAxes="False" WallWidth="3" IsClustered="False" />
                                <AxisY LineColor="64, 64, 64, 64">
                                    <LabelStyle Font="Trebuchet MS, 11.25pt, style=Bold" />
                                    <MajorGrid LineColor="Gray" />
                                </AxisY>
                                <AxisX LineColor="64, 64, 64, 64">
                                    <LabelStyle Font="Trebuchet MS, 11.25pt, style=Bold" />
                                    <MajorGrid LineColor="Gray" />
                                </AxisX>
                            </asp:ChartArea>
                        </ChartAreas>
                    </asp:Chart>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
