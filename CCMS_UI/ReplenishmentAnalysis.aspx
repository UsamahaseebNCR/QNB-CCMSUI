<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReplenishmentAnalysis.aspx.cs"
    Inherits="CCMSUI.ReplenishmentAnalysis" %>

<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Replenishment Analysis</title>
    <link href="content/iSuite.css" rel="stylesheet" />
    <script language="javascript" src="content/cal/datepicker.js" type="text/javascript"></script>
     <script src="content/shortcut.js" type="text/javascript" language="javascript">
    </script>
</head>
<body onload="hideProgressBar();" >
    <form id="form1" runat="server">
    <div>
        <asp:Literal ID="Literal_Message" runat="server"></asp:Literal>
    </div>
    <ComponentArt:TabStrip ID="TabStrip_Replenishment" CssClass="TopGroup" DefaultItemLookId="DefaultTabLook"
        DefaultSelectedItemLookId="SelectedTabLook" DefaultDisabledItemLookId="DisabledTabLook"
        DefaultGroupTabSpacing="1px" ImagesBaseUrl="images/Tabimages/" MultiPageId="MultiPage1"
        runat="server" ScrollDownLookId="DefaultTabLook" ScrollLeftLookId="DefaultTabLook"
        ScrollRightLookId="DefaultTabLook" ScrollUpLookId="DefaultTabLook" TopGroupTabSpacing="1px"
        Width="100%">
        <ItemLooks>
            <ComponentArt:ItemLook LookId="DefaultTabLook" CssClass="DefaultTab" HoverCssClass="DefaultTabHover"
                LabelPaddingLeft="10px" LabelPaddingRight="10px" LabelPaddingTop="5px" LabelPaddingBottom="4px"
                LeftIconUrl="tab_left_icon.gif" RightIconUrl="tab_right_icon.gif" HoverLeftIconUrl="hover_tab_left_icon.gif"
                HoverRightIconUrl="hover_tab_right_icon.gif" LeftIconWidth="3px" LeftIconHeight="21px"
                RightIconWidth="3px" RightIconHeight="21px" LeftIconVisibility="Always" RightIconVisibility="Always" />
            <ComponentArt:ItemLook LookId="SelectedTabLook" CssClass="SelectedReportsTab" LabelPaddingLeft="10px"
                LabelPaddingRight="10px" LabelPaddingTop="4px" LabelPaddingBottom="4px" LeftIconUrl="selected_tab_left_icon.gif"
                RightIconUrl="selected_tab_right_icon.gif" LeftIconWidth="3px" LeftIconHeight="21px"
                RightIconWidth="3px" RightIconHeight="21px" LeftIconVisibility="Always" RightIconVisibility="Always" />
        </ItemLooks>
        <Tabs>
            <ComponentArt:TabStripTab ID="TabStripTab_WeekAnalysis" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Weekly Analysis">
            </ComponentArt:TabStripTab>
            <ComponentArt:TabStripTab ID="TabStripTab_RegionWiseAnalysis" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Region Wise Analysis">
            </ComponentArt:TabStripTab>
            <ComponentArt:TabStripTab ID="TabStripTab1" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Multi Region Analysis">
            </ComponentArt:TabStripTab>
        </Tabs>
    </ComponentArt:TabStrip>
    <ComponentArt:MultiPage ID="MultiPage1" CssClass="MultiPage" runat="server">
        <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView1">
            <div>
                <table width="100%" border="0">
                    <tr>
                        <td colspan="3">
                            <asp:ValidationSummary ID="valSummary" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td align="center" style="font-size: large; background-color: Gray; color: White"
                            colspan="3">
                            Replenishment Analysis
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3">
                            <table style="background-color: InfoBackground; width: 100%">
                                <tr>
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
                                            style="border: 0; cursor: hand" id="IMG5" />
                                    </td>
                                    <td style="width: 3px">
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator6" CssClass="Validator"
                                            runat="server" ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                            ControlToValidate="TextBox_To" ErrorMessage="To Date is invalid.">*</asp:RegularExpressionValidator>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ControlToValidate="TextBox_To"
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
                    <tr>
                        <td>
                            <table style="margin-top: 10px; margin-bottom: 10px">
                                <tr>
                                    <td style="text-align: center; font-size:medium " onclick="javascript:window.open('DisplayData.aspx?gid=22&data='+this.innerHTML.replace('<B>','').replace('</B>','').replace('<BR/>','').replace('<BR>',''));">
                                        <asp:Literal ID="Literal1" runat="server"></asp:Literal>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Chart ID="ChartWeeklyLast" runat="server" Width="425" EnableViewState="true">
                                                <Titles>
                                                    <asp:Title Font="Trebuchet MS, 14.25pt, style=Bold" Name="Title1" ForeColor="26, 59, 105">
                                                    </asp:Title>
                                                </Titles>
                                                <Legends>
                                                    <asp:Legend BackColor="Transparent" Alignment="Center" Docking="Right" Font="Trebuchet MS, 8.25pt, style=Bold"
                                                        IsTextAutoFit="False" Name="Default" LegendStyle="Column" Title="Legends" Enabled="false">
                                                    </asp:Legend>
                                                </Legends>
                                                <BorderSkin SkinStyle="None"></BorderSkin>
                                                <Series>
                                                    <asp:Series Name="Default" YValuesPerPoint="2" ChartType="Column" IsValueShownAsLabel="true"
                                                        IsVisibleInLegend="true">
                                                    </asp:Series>
                                                </Series>
                                                <ChartAreas>
                                                    <asp:ChartArea Name="ChartArea1" BackColor="White">
                                                        <Area3DStyle Rotation="0" />
                                                        <AxisY LineColor="64, 64, 64, 64">
                                                            <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                            <MajorGrid LineColor="White" />
                                                        </AxisY>
                                                        <AxisX LineColor="64, 64, 64, 64">
                                                            <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                            <MajorGrid LineColor="White" />
                                                        </AxisX>
                                                    </asp:ChartArea>
                                                </ChartAreas>
                                            </asp:Chart>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="text-align: center; font-size:medium" onclick="javascript:window.open('DisplayData.aspx?gid=22&data='+this.innerHTML.replace('<B>','').replace('</B>','').replace('<BR/>','').replace('<BR>',''));">
                                        <asp:Literal ID="Literal2" runat="server"></asp:Literal>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Chart ID="ChartWeeklyPrevious" runat="server" Width="425" EnableViewState="true">
                                            <Titles>
                                                <asp:Title Font="Trebuchet MS, 14.25pt, style=Bold" Name="Title1" ForeColor="26, 59, 105">
                                                </asp:Title>
                                            </Titles>
                                            <Legends>
                                                <asp:Legend BackColor="Transparent" Alignment="Center" Docking="Right" Font="Trebuchet MS, 8.25pt, style=Bold"
                                                    IsTextAutoFit="False" Name="Default" LegendStyle="Column" Title="Legends" Enabled="false">
                                                </asp:Legend>
                                            </Legends>
                                            <BorderSkin SkinStyle="None"></BorderSkin>
                                            <Series>
                                                <asp:Series Name="Default" YValuesPerPoint="2" ChartType="Column" IsValueShownAsLabel="true"
                                                    IsVisibleInLegend="true">
                                                </asp:Series>
                                            </Series>
                                            <ChartAreas>
                                                <asp:ChartArea Name="ChartArea1" BackColor="White">
                                                    <Area3DStyle Rotation="0" />
                                                    <AxisY LineColor="64, 64, 64, 64">
                                                        <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                        <MajorGrid LineColor="White" />
                                                    </AxisY>
                                                    <AxisX LineColor="64, 64, 64, 64">
                                                        <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                        <MajorGrid LineColor="White" />
                                                    </AxisX>
                                                </asp:ChartArea>
                                            </ChartAreas>
                                        </asp:Chart>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td valign="top">
                            <table style="margin-top: 10px; margin-bottom: 10px">
                                <tr>
                                    <td style="text-align: center; font-size:medium" onclick="javascript:window.open('DisplayData.aspx?gid=22&data='+this.innerHTML.replace('<B>','').replace('</B>','').replace('<BR/>','').replace('<BR>',''));">
                                        <asp:Literal ID="Literal3" runat="server"></asp:Literal>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Chart ID="ChartWeeklyDuration" runat="server" Width="425" EnableViewState="true">
                                            <Titles>
                                                <asp:Title Font="Trebuchet MS, 14.25pt, style=Bold" Name="Title1" ForeColor="26, 59, 105">
                                                </asp:Title>
                                            </Titles>
                                            <Legends>
                                                <asp:Legend BackColor="Transparent" Alignment="Center" Docking="Right" Font="Trebuchet MS, 8.25pt, style=Bold"
                                                    IsTextAutoFit="False" Name="Default" LegendStyle="Column" Title="Legends" Enabled="false">
                                                </asp:Legend>
                                            </Legends>
                                            <BorderSkin SkinStyle="None"></BorderSkin>
                                            <Series>
                                                <asp:Series Name="Default" YValuesPerPoint="2" ChartType="line" IsValueShownAsLabel="true"
                                                    IsVisibleInLegend="true">
                                                </asp:Series>
                                            </Series>
                                            <ChartAreas>
                                                <asp:ChartArea Name="ChartArea1" BackColor="White">
                                                    <Area3DStyle Rotation="0" />
                                                    <AxisY LineColor="64, 64, 64, 64">
                                                        <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                        <MajorGrid LineColor="White" />
                                                    </AxisY>
                                                    <AxisX LineColor="64, 64, 64, 64">
                                                        <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                        <MajorGrid LineColor="White" />
                                                    </AxisX>
                                                </asp:ChartArea>
                                            </ChartAreas>
                                        </asp:Chart>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td valign="top">
                            <div style="width: 100%; overflow: auto" id="divRegionComparison" runat="server">
                                <asp:GridView ID="GridView1" runat="server" Width="100%" AutoGenerateColumns="false"
                                    HorizontalAlign="Center" AllowSorting="true" OnSorting="GridView1_Sorting" HeaderStyle-ForeColor="Black">
                                    <AlternatingRowStyle BackColor="#eff0ef" />
                                    <Columns>
                                        <asp:BoundField DataField="title" HeaderText="Title" ItemStyle-HorizontalAlign="Center"
                                            SortExpression="title" />
                                        <asp:BoundField DataField="rep_datetime" HeaderText="Replenishment Date" ItemStyle-HorizontalAlign="Center"
                                            SortExpression="rep_datetime" />
                                        <asp:BoundField DataField="amount" HeaderText="Amount" ItemStyle-HorizontalAlign="Center"
                                            SortExpression="amount" />
                                    </Columns>
                                    <EmptyDataTemplate>
                                        No records found
                                    </EmptyDataTemplate>
                                </asp:GridView>
                            </div>
                        </td>
                    </tr>
                </table>
            </div>
        </ComponentArt:PageView>
        <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView2">
            <div>
                <table width="100%" border="0">
                    <tr>
                        <td align="center" style="font-size: large; background-color: Gray; color: White"
                            colspan="3">
                            Replenishment Analysis
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <table style="background-color: InfoBackground; width: 100%" border="0">
                                <tr>
                                    <td style="width: 23px">
                                        From
                                    </td>
                                    <td style="width: 23px">
                                        <asp:TextBox ID="TextBox3" runat="server" CssClass="textbox"></asp:TextBox>
                                    </td>
                                    <td style="width: 23px">
                                        <img src="images/IconCalendar.gif" alt="Click to open calender" onclick="javascript:show_calendar('form1.TextBox3');"
                                            style="border: 0; cursor: hand" id="IMG1" />
                                    </td>
                                    <td style="width: 3px">
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" CssClass="Validator"
                                            runat="server" ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                            ControlToValidate="TextBox3" ErrorMessage="To Date is invalid.">*</asp:RegularExpressionValidator>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="TextBox3"
                                        ErrorMessage="To date cannot be empty">*</asp:RequiredFieldValidator>
                                    </td>
                                    <td style="width: 23px">
                                        To
                                    </td>
                                    <td style="width: 23px">
                                        <asp:TextBox ID="TextBox4" runat="server" CssClass="textbox"></asp:TextBox>
                                    </td>
                                    <td style="width: 23px">
                                        <img src="images/IconCalendar.gif" alt="Click to open calender" onclick="javascript:show_calendar('form1.TextBox4');"
                                            style="border: 0; cursor: hand" id="IMG2" />
                                    </td>
                                    <td style="width: 3px">
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator2" CssClass="Validator"
                                            runat="server" ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                            ControlToValidate="TextBox4" ErrorMessage="To Date is invalid.">*</asp:RegularExpressionValidator>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="TextBox4"
                                        ErrorMessage="To date cannot be empty">*</asp:RequiredFieldValidator>
                                    </td>
                                    <td align="left">
                                        <asp:Button ID="Button2" runat="server" Text="Generate" OnClick="Button_Submit_Click"
                                            CssClass="button" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td >
                            <center>
                                <table style="margin-top: 10px; margin-bottom: 10px">
                                    <tr>
                                        <td style="text-align:center;font-size:medium" onclick="javascript:window.open('DisplayData.aspx?gid=23&data='+this.innerHTML.replace('<B>','').replace('</B>','').replace('<BR/>','').replace('<BR>',''));">
                                            <asp:Literal ID="Literal4" runat="server"></asp:Literal>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Chart ID="ChartRegion" runat="server" Width="425" EnableViewState="true">
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
                                                    <asp:Series Name="Default" YValuesPerPoint="2" ChartType="Pie" IsValueShownAsLabel="true" >
                                                    </asp:Series>
                                                </Series>
                                                <ChartAreas>
                                                    <asp:ChartArea Name="ChartArea1" BackColor="White">
                                                        <Area3DStyle Rotation="0" />
                                                        <AxisY LineColor="64, 64, 64, 64">
                                                            <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                            <MajorGrid LineColor="White" />
                                                        </AxisY>
                                                        <AxisX LineColor="64, 64, 64, 64">
                                                            <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                            <MajorGrid LineColor="White" />
                                                        </AxisX>
                                                    </asp:ChartArea>
                                                </ChartAreas>
                                            </asp:Chart>
                                        </td>
                                    </tr>
                                </table>
                            </center>
                        </td>
                        <td valign="top">
                            <div style="width: 100%; overflow: auto" id="divRepAnalysis" runat="server">
                                <asp:GridView ID="GridView_RepAnalysis" runat="server" Width="100%" AutoGenerateColumns="false"
                                    HorizontalAlign="Center" AllowSorting="true" OnSorting="GridView_RepAnalysis_Sorting"
                                    HeaderStyle-ForeColor="Black">
                                    <AlternatingRowStyle BackColor="#eff0ef" />
                                    <Columns>
                                        <asp:BoundField DataField="title" HeaderText="Title" ItemStyle-HorizontalAlign="Center"
                                            SortExpression="title" />
                                        <asp:BoundField DataField="region_name" HeaderText="Region" ItemStyle-HorizontalAlign="Center"
                                            SortExpression="region_name" />
                                        <asp:BoundField DataField="rep_datetime" HeaderText="Datetime" ItemStyle-HorizontalAlign="Center"
                                            SortExpression="rep_datetime" />
                                        <asp:BoundField DataField="amount" HeaderText="Amount" ItemStyle-HorizontalAlign="Center"
                                            SortExpression="amount" />
                                    </Columns>
                                    <EmptyDataTemplate>
                                        No records found
                                    </EmptyDataTemplate>
                                </asp:GridView>
                            </div>
                        </td>
                    </tr>
                </table>
            </div>
        </ComponentArt:PageView>
        <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView6">
            <div>
                <table width="100%" border="0">
                    <tr>
                        <td align="center" style="font-size: large; background-color: Gray; color: White"
                            colspan="3">
                            Replenishment Analysis
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3">
                            <table style="background-color: InfoBackground; width: 100%" border="0">
                                <tr>
                                    <td style="width: 23px">
                                        From
                                    </td>
                                    <td style="width: 23px">
                                        <asp:TextBox ID="TextBox9" runat="server" CssClass="textbox"></asp:TextBox>
                                    </td>
                                    <td style="width: 23px">
                                        <img src="images/IconCalendar.gif" alt="Click to open calender" onclick="javascript:show_calendar('form1.TextBox9');"
                                            style="border: 0; cursor: hand" id="IMG3" />
                                    </td>
                                    <td style="width: 3px">
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator3" CssClass="Validator"
                                            runat="server" ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                            ControlToValidate="TextBox9" ErrorMessage="To Date is invalid.">*</asp:RegularExpressionValidator>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="TextBox9"
                                        ErrorMessage="To date cannot be empty">*</asp:RequiredFieldValidator>
                                    </td>
                                    <td style="width: 23px">
                                        To
                                    </td>
                                    <td style="width: 23px">
                                        <asp:TextBox ID="TextBox10" runat="server" CssClass="textbox"></asp:TextBox>
                                    </td>
                                    <td style="width: 23px">
                                        <img src="images/IconCalendar.gif" alt="Click to open calender" onclick="javascript:show_calendar('form1.TextBox10');"
                                            style="border: 0; cursor: hand" id="IMG4" />
                                    </td>
                                    <td style="width: 3px">
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator4" CssClass="Validator"
                                            runat="server" ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                            ControlToValidate="TextBox10" ErrorMessage="To Date is invalid.">*</asp:RegularExpressionValidator>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="TextBox10"
                                        ErrorMessage="To date cannot be empty">*</asp:RequiredFieldValidator>
                                    </td>
                                    <td align="left">
                                        <asp:Button ID="Button7" runat="server" Text="Generate" OnClick="Button_Submit_Click"
                                            CssClass="button" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <center>
                                <table style="margin-top: 20px;">
                                    <tr>
                                        <td style="font-size: medium" onclick="javascript:window.open('DisplayData.aspx?gid=21&data='+this.innerHTML.replace('<B>','').replace('</B>','').replace('<BR/>','').replace('<BR>',''));">
                                            <asp:Literal ID="Literal15" runat="server"></asp:Literal>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td valign="top">
                                            <asp:Chart ID="ChartMultiRegion" runat="server" Width="425" EnableViewState="true" OnLoad="Chart15_Load">
                                                <Titles>
                                                    <asp:Title Font="Trebuchet MS, 14.25pt, style=Bold" Name="Title1" ForeColor="26, 59, 105"></asp:Title>
                                                </Titles>
                                                <Legends>
                                                    <asp:Legend BackColor="Transparent" Alignment="Center" Docking="Right" Font="Trebuchet MS, 8.25pt, style=Bold"
                                                        IsTextAutoFit="False" Name="Default" LegendStyle="Column" Title="Legends">
                                                    </asp:Legend>
                                                </Legends>
                                                <BorderSkin SkinStyle="None"></BorderSkin>
                                                <Series>
                                                    <asp:Series Name="Default" YValuesPerPoint="2" ChartType="Column"></asp:Series>
                                                </Series>
                                                <ChartAreas>
                                                    <asp:ChartArea Name="ChartArea1" BackColor="White">
                                                        <Area3DStyle Rotation="0" />
                                                        <AxisY LineColor="64, 64, 64, 64">
                                                            <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                            <MajorGrid LineColor="White" />
                                                        </AxisY>
                                                        <AxisX LineColor="64, 64, 64, 64">
                                                            <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                            <MajorGrid LineColor="White" />
                                                        </AxisX>
                                                    </asp:ChartArea>
                                                </ChartAreas>
                                            </asp:Chart>
                                        </td>
                                    </tr>
                                </table>
                            </center>
                        </td>
                    </tr>
                </table>
            </div>
        </ComponentArt:PageView>
    </ComponentArt:MultiPage>
    </form>
</body>
</html>
