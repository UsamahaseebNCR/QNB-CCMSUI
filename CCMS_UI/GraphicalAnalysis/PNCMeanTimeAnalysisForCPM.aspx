<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PNCMeanTimeAnalysisForCPM.aspx.cs"
    Inherits="CCMSUI.PNCMeanTimeAnalysisForCPM" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Mean Time To Failure Analysis</title>
    <script language="javascript" src="../content/cal/datepicker.js" type="text/javascript"></script>
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
    <link href="../content/iSuite.css" rel="stylesheet" />
    <link href="../content/Scripts/ui-lightness/jquery-ui-1.7.2.custom.css" rel="stylesheet"
        type="text/css" />
    <script type="text/javascript" src="../content/js/jquery.js"></script>
    <script type="text/javascript" src="../content/scripts/ui.datepicker.js"></script>
    <script type="text/javascript">
        $(function () {

            $('.datepicker').datepicker({ dateFormat: 'dd/mm/yy',
                changeMonth: true,
                changeYear: true,
                showOn: "button",
                buttonImage: "../images/calendar.gif",
                buttonImageOnly: true

            });
        });
    
    </script>
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
                <td colspan="2" class="PageHeading" align="left">
                    Mean Time To Failure Analysis For CPM
                </td>
            </tr>
            <tr align="left" style="background-color: WHITE">
                <td style="height: 20px; padding-left: 5px;" colspan="2">
                    <b>Search Filters</b>
                </td>
            </tr>
            <tr>
                <td>
                    <table width="100%" class="SEARCHBOX" border="0">
                        <tr>
                            <td>
                                <table>
                                    <tr>
                                        <td>
                                            From
                                        </td>
                                        <td>
                                            <asp:TextBox ID="TextBox_From" runat="server" CssClass="datepicker"></asp:TextBox>
                                            <asp:RegularExpressionValidator ID="RegularExpressionValidator5" CssClass="Validator"
                                                runat="server" ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                                ControlToValidate="TextBox_From" ErrorMessage="To Date is invalid.">*</asp:RegularExpressionValidator>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="TextBox_From"
                                                ErrorMessage="From date cannot be empty">*</asp:RequiredFieldValidator>
                                        </td>
                                        <td>
                                            To
                                        </td>
                                        <td>
                                            <asp:TextBox ID="TextBox_To" runat="server" CssClass="datepicker"></asp:TextBox>
                                            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" CssClass="Validator"
                                                runat="server" ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                                ControlToValidate="TextBox_To" ErrorMessage="To Date is invalid.">*</asp:RegularExpressionValidator>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="TextBox_To"
                                                ErrorMessage="To date cannot be empty">*</asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                     <tr>
                                        <td valign="top">
                                            Sampling Size:
                                        </td>
                                        <td>
                                            <asp:TextBox ID="TextBox_SamplingSize" runat="server" Text="1000" CssClass="textbox" ></asp:TextBox>
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator_SamplingSize" runat="server"
                                             ValidationExpression="\d+" ControlToValidate="TextBox_SamplingSize" ErrorMessage="Invalid sampling size entered"></asp:RegularExpressionValidator>
                                        </td>
                                    </tr>
                                   <%--<tr style="width: 100%">
                                        <td>
                                            Error Analysis On
                                        </td>
                                        <td valign="top" style="width: 40%">
                                            <asp:CheckBoxList ID="CheckBoxList_ErrorAnalysisType" runat="server" Width="100%"
                                                RepeatDirection="Horizontal">
                                                <asp:ListItem Text="Business Rule" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="Non Business Rule" Selected="True"></asp:ListItem>
                                            </asp:CheckBoxList>
                                        </td>
                                    </tr>--%>
                                    <tr>
                                        <td>
                                        </td>
                                        <td align="left" colspan="6">
                                            <asp:Button ID="Button1" runat="server" Text="Generate" OnClick="Button_Submit_Click"
                                                CssClass="button" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td valign="top">
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
            <tr id="chartRow">
                <td valign="top">
                    <table style="margin-top: 10px; border: 1px;" border="0" width="100%">
                        <tr>
                            <td valign="top" width="100%">
                                <table width="100%">
                                    <tr>
                                        <td>
                                            <center>
                                                <asp:Chart ID="Chart_MeanTimeToFailure1" runat="server" BorderColor="#B54001" Width="1200px">
                                                    <Titles>
                                                        <asp:Title Font="Trebuchet MS, 14.25pt, style=Bold" ForeColor="26, 59, 105" Text="Mean Time To Failure Analysis For CPM">
                                                        </asp:Title>
                                                    </Titles>
                                                    <Legends>
                                                        <asp:Legend BackColor="Transparent" Alignment="Near" Docking="Bottom" Font="Trebuchet MS, 8.25pt, style=Bold"
                                                            IsTextAutoFit="true" Name="Default" LegendStyle="Row" Title="Legends" Enabled="true">
                                                        </asp:Legend>
                                                    </Legends>
                                                    <BorderSkin SkinStyle="None"></BorderSkin>
                                                    <Series>
                                                        <asp:Series Name="Per Media" ChartType="Column" IsValueShownAsLabel="true">
                                                        </asp:Series>
                                                        <asp:Series Name="Processed Transactions" ChartType="Column" IsValueShownAsLabel="true">
                                                        </asp:Series>
                                                    </Series>
                                                    <ChartAreas>
                                                        <asp:ChartArea Name="ChartArea1" BackColor="White">
                                                            <Area3DStyle Rotation="0" />
                                                            <AxisY LineColor="64, 64, 64, 64" Title="Cheques Deposited">
                                                                <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                                <MajorGrid LineColor="GhostWhite" />
                                                            </AxisY>
                                                            <AxisX LineColor="64, 64, 64, 64" Title="Day" IntervalType="Days" IntervalOffset="5"
                                                                IntervalOffsetType="Days" IsStartedFromZero="true">
                                                                <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                                <MajorGrid LineColor="GhostWhite" />
                                                            </AxisX>
                                                        </asp:ChartArea>
                                                    </ChartAreas>
                                                </asp:Chart>
                                            </center>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td >
                                   
                                <asp:GridView ID="GridView_MeanTime" runat="server" AutoGenerateColumns="false"
                                    CellPadding="4" ForeColor="#333333" GridLines="None">
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
                                    <Columns>
                                       <asp:BoundField DataField="datetime" HeaderText="Error Occured At" />
                                        <asp:BoundField DataField="TotalDeposits" HeaderText="Per Media" />
                                        <asp:BoundField DataField="TotalTrxns" HeaderText="Total Processed Transactions"  />
                                        <asp:BoundField DataField="FailureCount" HeaderText="FailureCount" />
                                    </Columns>
                                </asp:GridView>
                                
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
