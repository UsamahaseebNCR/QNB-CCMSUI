<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VaultUtilizationAnalysis.aspx.cs"
    Inherits="CCMSUI.VaultUtilizationAnalysis" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Vault Utilization Analysis</title>
    <script language="javascript" src="content/cal/datepicker.js" type="text/javascript"></script>
    <script src="content/shortcut.js" type="text/javascript" language="javascript">
    </script>
    <link href="content/iSuite.css" rel="stylesheet" />
</head>
<body onload="hideProgressBar();">
    <form id="form1" runat="server">
    <div>
        <asp:Literal ID="Literal_Message" runat="server"></asp:Literal>
    </div>
    <div>
        <table width="100%" border="0">
            <tr>
                <td colspan="3" align="center">
                    <asp:ValidationSummary ID="valSummary" runat="server" />
                </td>
            </tr>
            <tr>
                <td align="center" style="font-size: large; background-color: Gray; color: White"
                    colspan="3">
                    Vault Utilization Analysis
                </td>
            </tr>
            <tr id="t" runat="server">
                <td colspan="3">
                    <table style="background-color: InfoBackground; width: 100%" class="DemoArea">
                        <tr>
                            <td>
                                Organization
                            </td>
                            <td>
                                <asp:Label ID="Label_Org" runat="server" Text="Please select Organization" Font-Bold="true"></asp:Label>
                            </td>
                            <td>
                                CIT
                            </td>
                            <td>
                                <asp:DropDownList ID="DropDownList_CIT" runat="server" Width="151px" CssClass="dropDown"
                                    AutoPostBack="true" OnSelectedIndexChanged="DropDownList_CIT_Change">
                                </asp:DropDownList>
                                <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToValidate="DropDownList_CIT"
                                    Type="String" ValueToCompare="Select" Operator="NotEqual" ErrorMessage="Please select CIT"
                                    ValidationGroup="SaveCheque">*</asp:CompareValidator>
                            </td>
                            <td>
                                Vault
                            </td>
                            <td>
                                <asp:DropDownList ID="DropDownList_Vault" runat="server" Width="150px" CssClass="dropDown">
                                </asp:DropDownList>
                                <asp:CompareValidator ID="CompareValidator2" runat="server" ControlToValidate="DropDownList_Vault"
                                    Type="String" ValueToCompare="Select" Operator="NotEqual" ErrorMessage="Please select Vault"
                                    ValidationGroup="SaveCheque">*</asp:CompareValidator>
                            </td>
                            <td style="width: 23px">
                                From
                            </td>
                            <td style="width: 23px">
                                <asp:TextBox ID="TextBox_From" runat="server" CssClass="textbox"></asp:TextBox>
                            </td>
                            <td style="width: 23px">
                                <img src="images/IconCalendar.gif" alt="Click to open calender" onclick="javascript:show_calendar('form1.TextBox_From');"
                                    style="border: 0; cursor: hand" id="IMG6" />
                            </td>
                            <td style="width: 3px">
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator5" CssClass="Validator"
                                    runat="server" ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                    ControlToValidate="TextBox_From" ErrorMessage="To Date is invalid.">*</asp:RegularExpressionValidator>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="TextBox_From"
                                    ErrorMessage="To date cannot be empty">*</asp:RequiredFieldValidator>
                            </td>
                            <td style="width: 23px">
                                To
                            </td>
                            <td style="width: 23px">
                                <asp:TextBox ID="TextBox_To" runat="server" CssClass="textbox"></asp:TextBox>
                            </td>
                            <td style="width: 23px">
                                <img src="images/IconCalendar.gif" alt="Click to open calender" onclick="javascript:show_calendar('form1.TextBox_To');"
                                    style="border: 0; cursor: hand" id="IMG1" />
                            </td>
                            <td style="width: 3px">
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" CssClass="Validator"
                                    runat="server" ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                    ControlToValidate="TextBox_To" ErrorMessage="To Date is invalid.">*</asp:RegularExpressionValidator>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="TextBox_To"
                                    ErrorMessage="To date cannot be empty">*</asp:RequiredFieldValidator>
                            </td>
                            <td align="left">
                                <asp:Button ID="Button1" runat="server" Text="Generate" OnClick="Button_Submit_Click"
                                    CssClass="button" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr id="chartRow">
                <td valign="top" width="80%">
                    <table style="margin-top: 10px; border: 1px">
                        <tr>
                            <td valign="top" width="20%">
                                <asp:GridView ID="GridView_VaultATMs" runat="server" Width="100%" CellPadding="4"
                                    ForeColor="#333333" GridLines="None" AutoGenerateColumns="false" AllowSorting="true">
                                    <Columns>
                                        <asp:BoundField DataField="title" HeaderText="ATM" ItemStyle-HorizontalAlign="Center" />
                                        <asp:BoundField DataField="current_order_received_at" HeaderText="Order Date" ItemStyle-HorizontalAlign="Center"
                                            DataFormatString="{0:dd/MM/yyyy}" />
                                        <asp:BoundField DataField="total" HeaderText="Total Order" ItemStyle-HorizontalAlign="Center" />
                                    </Columns>
                                    <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                                    <EditRowStyle BackColor="#999999" />
                                    <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                                    <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                                    <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                                    <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                                    <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                                    <SortedAscendingCellStyle BackColor="#E9E7E2" />
                                    <SortedAscendingHeaderStyle BackColor="#506C8C" />
                                    <SortedDescendingCellStyle BackColor="#FFFDF8" />
                                    <SortedDescendingHeaderStyle BackColor="#6F8DAE" />
                                </asp:GridView>
                            </td>
                            <td valign="top">
                                <asp:Chart ID="ChartVaultUtil" runat="server" Width="425px">
                                    <Titles>
                                        <asp:Title Font="Trebuchet MS, 14.25pt, style=Bold" Name="Title1" ForeColor="26, 59, 105">
                                        </asp:Title>
                                    </Titles>
                                    <Legends>
                                        <asp:Legend BackColor="Transparent" Alignment="Center" Docking="Right" Font="Trebuchet MS, 8.25pt, style=Bold"
                                            IsTextAutoFit="False" Name="Default" LegendStyle="Column" Title="Legends">
                                        </asp:Legend>
                                    </Legends>
                                    <BorderSkin SkinStyle="None"></BorderSkin>
                                    <Series>
                                        <asp:Series Name="Default" YValuesPerPoint="2" ChartType="Pie" IsValueShownAsLabel="true"
                                            IsVisibleInLegend="true">
                                        </asp:Series>
                                    </Series>
                                    <ChartAreas>
                                        <asp:ChartArea Name="ChartArea1" Area3DStyle-Enable3D="true">
                                            <Area3DStyle Rotation="0" />
                                            <AxisY LineColor="64, 64, 64, 64">
                                                <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                <MajorGrid LineColor="64, 64, 64, 64" />
                                            </AxisY>
                                            <AxisX LineColor="64, 64, 64, 64">
                                                <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                <MajorGrid LineColor="64, 64, 64, 64" />
                                            </AxisX>
                                        </asp:ChartArea>
                                    </ChartAreas>
                                </asp:Chart>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
    <input type="button" runat="server" id="refresh" style="display: none" />
    </form>
</body>
</html>
