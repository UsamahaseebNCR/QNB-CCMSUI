<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Summary.aspx.cs" Inherits="CCMSUI.CCMS.Summary"
    MaintainScrollPositionOnPostback="true" EnableSessionState="True" %>

<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<!DOCTYPE html>
<%--<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">--%>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Summary</title>
    <script type="text/javascript" src="../content/js/jquery.js"></script>
    <link href="../content/iSuite.css" rel="stylesheet" />
    <link href="../content/theme.css" rel="stylesheet" />
    <link href="../content/tabStyle.css" rel="stylesheet" />
    <script src="../content/shortcut.js" type="text/javascript" language="javascript"></script>
    <script src="../content/js/loading-overlay.min.js"></script>
    <link type="text/css" href="../content/css/demo.css" rel="stylesheet"></link>
    <script>

       
        $(function () {
            $('#divWait').hide();
            
        });

        function displaySpinner() {
            $("html").css("opacity",0.6)
            showProgressBar(); 
        }
    </script>
    <script type="text/javascript">
        function hideProgressBar() {
            parent.document.getElementById('divWait').style.visibility = 'hidden';
        }

        function showProgressBar() {
            $('#divWait').show();
           // document.getElementById('divWait').style.visibility = 'visible';
            //alert('sh');
            //alert(document.documentElement.clientWidth);

            //alert(document.getElementById('menu').style.width);
            //document.getElementById('menu').style.width = document.documentElement.clientWidth-20;
            //alert(document.getElementById('menu').style.width);   
        }

        function displayDFFATM(type) {
            Dialog2.Show();
            if (type == 0)
                DialogCallBack.callback(type, document.getElementById('<%=hidden_NoData.ClientID %>').value, 1);
            else if (type == 1)
                DialogCallBack.callback(type, document.getElementById('<%=hidden_Sending2.ClientID %>').value, 1);
            else if (type == 2)
                DialogCallBack.callback(type, document.getElementById('<%=hidden_SendingSameBalance.ClientID %>').value, 1);
            else if (type == 3)
                DialogCallBack.callback(type, document.getElementById('<%=hidden_InconsistentData.ClientID %>').value, 1);



        }
        function displayATM(type) {
            Dialog2.Show();
            if (type == 0)
                DialogCallBack.callback(type, document.getElementById('<%=hidden_transacting.ClientID %>').value, 1);
            else if (type == 1)
                DialogCallBack.callback(type, document.getElementById('<%=hidden_dispensedFailed.ClientID %>').value, 1);
            else if (type == 2)
                DialogCallBack.callback(type, document.getElementById('<%=hidden_faulty.ClientID %>').value, 1);
            else if (type == 3)
                DialogCallBack.callback(type, document.getElementById('<%=hidden_noActivity.ClientID %>').value, 1);
            else if (type == 4)
                DialogCallBack.callback(type, document.getElementById('<%=hidden_BNANoTransactingATM.ClientID %>').value, 1);
            else if (type == 5)
                DialogCallBack.callback(type, document.getElementById('<%=hidden_BNATransactingATM.ClientID %>').value, 1);
            else if (type == 6)
                DialogCallBack.callback(type, document.getElementById('<%=hidden_ChequeNoTransactingATM.ClientID %>').value, 1);
            else if (type == 7)
                DialogCallBack.callback(type, document.getElementById('<%=hidden_ChequeTransactingATM.ClientID %>').value, 1);
            else if (type == 8)
                DialogCallBack.callback(type, document.getElementById('<%=hidden_noTransacting.ClientID %>').value, 1);





        }

        function openDialog(alertTypeID, entityType) {
            Dialog1.Show(); DialogCallBack.callback(alertTypeID, entityType, 0);
        } function receiveCallBackResult(response) {
            document.getElementById('summaryInfo').innerHTML
= response;
        }
        function receiveCallBackResult1(response) {
            document.getElementById('ATM').innerHTML
= response;
        }




        function hideText() {

            var count = 0;
            var k;
            var obj = document.getElementById('GridView2');
            //alert(obj);
            //alert(obj.rows[1].cells[1].innerHTML);
            //window.setTimeout(showText(), 2000);

            for (k = 1; k < 9; k++) {
                //alert(obj.rows[k].cells[1].innerHTML);
                obj.rows[k].cells[1].innerHTML = "";
                count++;
            }

            alert('hidden');
            if (count > 0) {
                window.setTimeout(showText(), 30000);
            }

        }

        function showText() {
            //alert('exec');
            var k;
            var obj = document.getElementById('GridView2');
            //alert(hidden_toBlink.Value);

            arr = document.getElementById('hidden_toBlink').value.split(',');
            //alert(arr[0]);
            for (k = 1; k <= 9; k++) {
                //alert(obj.rows[k].cells[1].innerHTML);
                obj.rows[k].cells[1].innerHTML = arr[k - 1];
            }
            //window.setTimeout(hideText(), 10000);
            alert('dis');
        }



    </script>
</head>
<body onload="hideProgressBar(); ">
    <form id="Form1" method="post" runat="server">
    <input type="hidden" id="hidden_NoData" runat="server" />
    <input type="hidden" id="hidden_Sending2" runat="server" />
    <input type="hidden" id="hidden_SendingSameBalance" runat="server" />
    <input type="hidden" id="hidden_InconsistentData" runat="server" />
    <input type="hidden" id="hidden_transacting" runat="server" />
    <input type="hidden" id="hidden_faulty" runat="server" />
    <input type="hidden" id="hidden_noTransacting" runat="server" />
    <input type="hidden" id="hidden_dispensedFailed" runat="server" />
    <input type="hidden" id="hidden_noActivity" runat="server" />
    <input type="hidden" id="hidden_BNATransactingATM" runat="server" />
    <input type="hidden" id="hidden_BNANoTransactingATM" runat="server" />
    <input type="hidden" id="hidden_ChequeTransactingATM" runat="server" />
    <input type="hidden" id="hidden_ChequeNoTransactingATM" runat="server" />
    <input type="hidden" id="hidden_toBlink" runat="server" />
    <table style="height: 100%" cellspacing="0" cellpadding="0" border="0" width="100%">
        <tr>
            <td class="PageHeading" align="left">
                Summary Generated For
                <asp:Label ID="Label_org" runat="server"></asp:Label>
            </td>
            <td class="PageHeading" align="right">
                <asp:Image ID="Image_CurrencyServer_Working" runat="server" ImageUrl="~/images/working.png"
                    Width="23px" Height="23px" ToolTip="Currency Server is running" Visible="false" />
                <asp:Image ID="Image_CurrencyServer_NotWorking" runat="server" ImageUrl="~/images/notworking.png"
                    Width="23px" Height="23px" ToolTip="Currency Server is stopped" Visible="false" />
                <asp:Image ID="Image_CurrencyParser_Working" runat="server" ImageUrl="~/images/working.png"
                    Width="23px" Height="23px" ToolTip="Currency Parser is running" Visible="false" />
                <asp:Image ID="Image_CurrencyParser_NotWorking" runat="server" ImageUrl="~/images/notworking.png"
                    Width="23px" Height="23px" ToolTip="Currency Parser is stoppped" Visible="false" />
                <asp:Image ID="Image_CurrencyCutOver_Working" runat="server" ImageUrl="~/images/working.png"
                    Width="23px" Height="23px" ToolTip="Currency Cut Over is running" Visible="false" />
                <asp:Image ID="Image_CurrencyCutOver_NotWorking" runat="server" ImageUrl="~/images/notworking.png"
                    Width="23px" Height="23px" ToolTip="Currency Cut Over is stopped" Visible="false" />
                <asp:Image ID="Image_AlertDispatcher_Working" runat="server" ImageUrl="~/images/working.png"
                    Width="23px" Height="23px" ToolTip="Alert Dispatcher is running" Visible="false" />
                <asp:Image ID="Image_AlertDispatcher_NotWorking" runat="server" ImageUrl="~/images/notworking.png"
                    Width="23px" Height="23px" ToolTip="Alert Dispatcher is stopped" Visible="false" />
                <asp:Image ID="Image_OrderDownloader_Working" runat="server" ImageUrl="~/images/working.png"
                    Width="23px" Height="23px" ToolTip="Order Downloader is running" Visible="false" />
                <asp:Image ID="Image_OrderDownloader_NotWorking" runat="server" ImageUrl="~/images/notworking.png"
                    Width="23px" Height="23px" ToolTip="Order Downloader is stopped" Visible="false" />
                <asp:Image ID="Image_TrxnMonitor_Working" runat="server" ImageUrl="~/images/working.png"
                    Width="23px" Height="23px" ToolTip="Transaction Monitor is running" Visible="false" />
                <asp:Image ID="Image_TrxnMonitor_NotWorking" runat="server" ImageUrl="~/images/notworking.png"
                    Width="23px" Height="23px" ToolTip="Transaction Monitor is stopped" Visible="false" />
                <asp:Image ID="Image_Monitor_Working" runat="server" ImageUrl="~/images/working.png"
                    Width="23px" Height="23px" ToolTip="Monitor is running" Visible="false" />
                <asp:Image ID="Image_Monitor_NotWorking" runat="server" ImageUrl="~/images/notworking.png"
                    Width="23px" Height="23px" ToolTip="Monitor is stopped" Visible="false" />
                <asp:Image ID="Image_ReportSchedular_Working" runat="server" ImageUrl="~/images/working.png"
                    Width="23px" Height="23px" ToolTip="Report Schedular is running" Visible="false" />
                <asp:Image ID="Image_ReportSchedular_NotWorking" runat="server" ImageUrl="~/images/notworking.png"
                    Width="23px" Height="23px" ToolTip="Report Schedular is stopped" Visible="false" />
                <asp:Image ID="Image_EJParser_Working" runat="server" ImageUrl="~/images/working.png"
                    Width="23px" Height="23px" ToolTip="EJ Parser is running" Visible="false" />
                <asp:Image ID="Image_EJParser_NotWorking" runat="server" ImageUrl="~/images/notworking.png"
                    Width="23px" Height="23px" ToolTip="EJ Parser is stopped" Visible="false" />
                <asp:Image ID="Image_CCMSVaultManager_Working" runat="server" ImageUrl="~/images/working.png"
                    Width="23px" Height="23px" ToolTip="CCMSVaultManager is running" Visible="false" />
                <asp:Image ID="Image_CCMSVaultManager_NotWorking" runat="server" ImageUrl="~/images/notworking.png"
                    Width="23px" Height="23px" ToolTip="CCMSVaultManager is stopped" Visible="false" />
                <asp:Image ID="Image_DailyFeedGenerator_Working" runat="server" ImageUrl="~/images/working.png"
                    Width="23px" Height="23px" ToolTip="DailyFeedGenerator is running" Visible="false" />
                <asp:Image ID="Image_DailyFeedGenerator_NotWorking" runat="server" ImageUrl="~/images/notworking.png"
                    Width="23px" Height="23px" ToolTip="DailyFeedGenerator is stopped" Visible="false" />
            </td>
        </tr>
        <tr>
            <td class="SearchHeadPaneGradient" colspan="2">
            </td>
        </tr>
        <tr>
            <td colspan="2" valign="top">
                <table width="100%" border="0">
                    <tr>
                        <td valign="top">
                            <asp:Literal ID="literal_script" runat="server" EnableViewState="false"></asp:Literal>
                            <%--  <div class="cart-tabstrip-outer-wrapper" id="container" runat="server">
                                <div class="cart-tabstrip-inner-wrapper">--%>
                            <table width="100%" style="background: ../images/PatternHomeBG.jpg;" border="0">
                                <tr>
                                    <td id="tableHeaderRecentActivity" runat="server" style="background-color: #54b948;
                                        color: white; text-align: center; font-family: Arial; font-size: 20px">
                                        <b>
                                            <asp:Label ID="Label_generatedAt" runat="server" Text="Click on refresh to generate Summary"></asp:Label>
                                        </b><span style="text-align: right">
                                            <asp:Button ID="Button_Regenerate" runat="server" CssClass="button" OnClick="Button_Regenerate_Click" OnClientClick="displaySpinner();"
                                                Text="Refresh" />
                                        </span>
                                    </td>
                                </tr>
                                <tr>
                                    <td valign="top" align="center">
                                        <table width="100%" border="0">
                                            <tr align="left">
                                                <td valign="top" style="width: 10%">
                                                    <table width="100%" border="0">
                                                        <tr>
                                                            <td valign="top">
                                                                <asp:Chart ID="Chart1" runat="server" Width="385" Height="299px" Palette="None" EnableViewState="true"
                                                                    PaletteCustomColors="92, 156, 40; 252, 180, 65;235, 20, 20; 0,0,0">
                                                                    <Titles>
                                                                        <asp:Title Font="Trebuchet MS, 12.25pt, style=Bold" Name="Title1" ForeColor="26, 59, 105"
                                                                            Text="ATM Status">
                                                                        </asp:Title>
                                                                    </Titles>
                                                                    <Legends>
                                                                        <asp:Legend BackColor="Transparent" Alignment="Near" Docking="Bottom" Font="Trebuchet MS, 8.25pt, style=Bold"
                                                                            IsTextAutoFit="False" Name="Default" LegendStyle="Column" Title="Legends" Enabled="false">
                                                                        </asp:Legend>
                                                                    </Legends>
                                                                    <BorderSkin SkinStyle="None"></BorderSkin>
                                                                    <Series>
                                                                        <asp:Series Name="Default" YValuesPerPoint="2" ChartType="Pie" IsValueShownAsLabel="true"
                                                                            IsVisibleInLegend="false">
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
                                                        <tr>
                                                            <td valign="top">
                                                                <fieldset>
                                                                    <legend style="font-family: Verdana; font-size: large; color: Black"><b>ATM Status Detail
                                                                    </b></legend>
                                                                    <table border="0" width="350px" style="color: Black">
                                                                        <tr>
                                                                            <td>
                                                                            </td>
                                                                            <td>
                                                                                <b><i>Analysis ONLY on the basis of Transacting ATM</i></b>
                                                                            </td>
                                                                            <td>
                                                                                <asp:CheckBox ID="CheckBox_AnalysisOnlyOnTransactingATM" runat="server" Checked="true" />
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td style="background-color: Green">
                                                                            </td>
                                                                            <td>
                                                                                <b>Transacting ATMs 00:00-23:59</b>
                                                                            </td>
                                                                            <td>
                                                                                <asp:Label ID="Label_ATMsTrxn" runat="server" Text="0" Font-Bold="true"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                        <tr id="trNoTransactionATMRow" runat="server">
                                                                            <td style="background-color: red">
                                                                            </td>
                                                                            <td>
                                                                                <b>No Transacting ATMs 00:00-23:59</b>
                                                                            </td>
                                                                            <td>
                                                                                <asp:Label ID="Label_NonTransactingATM" runat="server" Font-Bold="true" Text="0"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                        <tr id="trDispensingFailedRow" runat="server">
                                                                            <td style="background-color: Black">
                                                                            </td>
                                                                            <td>
                                                                                <b>ATMs Dispensing Failed Unresolved Count</b>
                                                                            </td>
                                                                            <td>
                                                                                <asp:Label ID="Label_ATMsDispensingFailed" runat="server" Font-Bold="true" Text="0"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                        <tr id="trCassetteFaultyRow" runat="server">
                                                                            <td style="background-color: Orange">
                                                                            </td>
                                                                            <td>
                                                                                <b>ATMs Cassettes Might Be Faulty Unresolved Count</b>
                                                                            </td>
                                                                            <td>
                                                                                <asp:Label ID="Label_CassettesFaulty" runat="server" Font-Bold="true" Text="0"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                        <tr id="trNoActivityRow" runat="server">
                                                                            <td style="background-color: Red">
                                                                            </td>
                                                                            <td>
                                                                                <b>ATMs With No Activity Unresolved Count</b>
                                                                            </td>
                                                                            <td>
                                                                                <asp:Label ID="Label_NoActivity" runat="server" Font-Bold="true" Text="0"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td colspan="2">
                                                                                <b>Total No.Of Active ATMs</b>
                                                                            </td>
                                                                            <td>
                                                                                <asp:Label ID="Label_TotalATMs" runat="server" Text="0" Font-Bold="true"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td colspan="2">
                                                                                <b>Total No.Of Transactions As of Current Day</b>
                                                                            </td>
                                                                            <td>
                                                                                <asp:Label ID="Label_CurrentDayTrxnCount" runat="server" Text="0" Font-Bold="true"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td colspan="2">
                                                                                <b>Total No.Of Transactions As of Previous Day</b>
                                                                            </td>
                                                                            <td>
                                                                                <asp:Label ID="Label_PreviousDayTrxnCount" runat="server" Text="0" Font-Bold="true"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </fieldset>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                                <td valign="top" style="width: 10%">
                                                    <table width="100%" border="0">
                                                        <tr>
                                                            <td valign="top">
                                                                <asp:Chart ID="Chart_ChequeDeposit" runat="server" Width="385" Height="299px" Palette="None"
                                                                    EnableViewState="true" PaletteCustomColors="92, 156, 40;235, 20, 20; 0,0,0">
                                                                    <Titles>
                                                                        <asp:Title Font="Trebuchet MS, 12.25pt, style=Bold" Name="Title1" ForeColor="26, 59, 105"
                                                                            Text="Cheque Status 00:00-23:59 ( Current Day )">
                                                                        </asp:Title>
                                                                    </Titles>
                                                                    <Legends>
                                                                        <asp:Legend BackColor="Transparent" Alignment="Near" Docking="Bottom" Font="Trebuchet MS, 8.25pt, style=Bold"
                                                                            IsTextAutoFit="False" Name="Default" LegendStyle="Column" Title="Legends" Enabled="false">
                                                                        </asp:Legend>
                                                                    </Legends>
                                                                    <BorderSkin SkinStyle="None"></BorderSkin>
                                                                    <Series>
                                                                        <asp:Series Name="Default" YValuesPerPoint="2" ChartType="Pie" IsValueShownAsLabel="true"
                                                                            IsVisibleInLegend="false">
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
                                                        <tr>
                                                            <td valign="top">
                                                                <fieldset>
                                                                    <legend style="font-family: Verdana; font-size: large; color: Black"><b>Cheque Status
                                                                        Detail</b></legend>
                                                                    <table border="0" width="350px" style="color: Black">
                                                                        <tr>
                                                                            <td style="background-color: Green">
                                                                            </td>
                                                                            <td>
                                                                                <b>Cheque Deposit At CDMs</b>
                                                                            </td>
                                                                            <td>
                                                                                <asp:Label ID="Label_ChequeTrxnCount" runat="server" Text="0" Font-Bold="true"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td style="background-color: Red">
                                                                            </td>
                                                                            <td>
                                                                                <b>CDMs With No Cheque Deposit Activity</b>
                                                                            </td>
                                                                            <td>
                                                                                <asp:Label ID="Label_ChequeNoActivity" runat="server" Font-Bold="true" Text="0"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td colspan="2">
                                                                                <b>Total No.Of Cheque Deposit Machines</b>
                                                                            </td>
                                                                            <td>
                                                                                <asp:Label ID="Label_ChequeDepositMachinesCount" runat="server" Text="0" Font-Bold="true"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </fieldset>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                                <td valign="top" style="width: 10%">
                                                    <table width="100%" border="0">
                                                        <tr>
                                                            <td valign="top">
                                                                <asp:Chart ID="Chart_BNA" runat="server" Width="385" Height="299px" Palette="None"
                                                                    EnableViewState="true" PaletteCustomColors="92, 156, 40; 235, 20, 20; 0,0,0">
                                                                    <Titles>
                                                                        <asp:Title Font="Trebuchet MS, 12.25pt, style=Bold" Name="Title1" ForeColor="26, 59, 105"
                                                                            Text="BNA Status 00:00-23:59 ( Current Day )">
                                                                        </asp:Title>
                                                                    </Titles>
                                                                    <Legends>
                                                                        <asp:Legend BackColor="Transparent" Alignment="Near" Docking="Bottom" Font="Trebuchet MS, 8.25pt, style=Bold"
                                                                            IsTextAutoFit="False" Name="Default" LegendStyle="Column" Title="Legends" Enabled="false">
                                                                        </asp:Legend>
                                                                    </Legends>
                                                                    <BorderSkin SkinStyle="None"></BorderSkin>
                                                                    <Series>
                                                                        <asp:Series Name="Default" YValuesPerPoint="2" ChartType="Pie" IsValueShownAsLabel="true"
                                                                            IsVisibleInLegend="false">
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
                                                        <tr>
                                                            <td valign="top">
                                                                <fieldset>
                                                                    <legend style="font-family: Verdana; font-size: large; color: Black"><b>BNA Status Detail
                                                                    </b></legend>
                                                                    <table border="0" width="350px" style="color: Black">
                                                                        <tr>
                                                                            <td style="background-color: Green">
                                                                            </td>
                                                                            <td>
                                                                                <b>Cash Deposited At CDMs</b>
                                                                            </td>
                                                                            <td>
                                                                                <asp:Label ID="Label_BNATotalTrxn" runat="server" Text="0" Font-Bold="true"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td style="background-color: Red">
                                                                            </td>
                                                                            <td>
                                                                                <b>CDMs With No Deposit Activity</b>
                                                                            </td>
                                                                            <td>
                                                                                <asp:Label ID="Label_BNANoActivity" runat="server" Font-Bold="true" Text="0"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td colspan="2">
                                                                                <b>Total No.Of Cash Deposit Machines</b>
                                                                            </td>
                                                                            <td>
                                                                                <asp:Label ID="Label_totalBNAAtms" runat="server" Text="0" Font-Bold="true"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </fieldset>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <table border="0">
                                                        <tr>
                                                            <td>
                                                                <table>
                                                                    <tr>
                                                                        <td style="color: Black">
                                                                            <b>Note Set Type</b>
                                                                        </td>
                                                                        <td>
                                                                            <asp:DropDownList ID="DropDownList_NoteSetType" runat="server" AutoPostBack="true"
                                                                                OnSelectedIndexChanged="Dropdownlist_SelectedIndexChanged" CssClass="dropDown">
                                                                            </asp:DropDownList>
                                                                        </td>
                                                                        <td style="color: Black">
                                                                            <asp:Label ID="Label_AnalysisOnEj" runat="server" Text="Analysis On EJ"></asp:Label>
                                                                        </td>
                                                                        <td>
                                                                            <asp:CheckBox ID="CheckBox_AnalysisOnEJ" runat="server" AutoPostBack="true" OnCheckedChanged="Dropdownlist_SelectedIndexChanged"
                                                                                Checked="true" />
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:Chart ID="Chart3" runat="server" Width="385px" EnableViewState="true" Height="300px">
                                                                    <Titles>
                                                                        <asp:Title Font="Trebuchet MS, 14.25pt, style=Bold" Name="Title1" ForeColor="26, 59, 105"
                                                                            Text="Remaining Denomination Quantities">
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
                                                                            <AxisY LineColor="64, 64, 64, 64" Title="Quantity">
                                                                                <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                                                <MajorGrid LineColor="White" />
                                                                            </AxisY>
                                                                            <AxisX LineColor="64, 64, 64, 64" Title="Denomination">
                                                                                <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                                                <MajorGrid LineColor="White" />
                                                                            </AxisX>
                                                                        </asp:ChartArea>
                                                                    </ChartAreas>
                                                                </asp:Chart>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <fieldset>
                                                                    <legend style="font-family: Verdana; font-size: large; color: Black"><b>Denomination
                                                                        Detail</b> </legend>
                                                                    <table border="0" width="250px" style="color: Black; text-align: left" align="center">
                                                                        <tr>
                                                                            <td style="font-size: 13px; font-family: Arial; background-color: Gray; color: White">
                                                                                <b>Denomination</b>
                                                                            </td>
                                                                            <td style="font-size: 13px; font-family: Arial; background-color: Gray; color: White">
                                                                                <b>Quantity</b>
                                                                            </td>
                                                                            <td style="font-size: 13px; font-family: Arial; background-color: Gray; color: White">
                                                                                <b>Total</b>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td>
                                                                                <asp:Label ID="Label_Denomination1" runat="server" Text="N/A" Font-Bold="true"></asp:Label>
                                                                            </td>
                                                                            <td>
                                                                                <asp:Label ID="Label_Quantity1" runat="server" Text="0" Font-Bold="true"></asp:Label>
                                                                            </td>
                                                                            <td>
                                                                                <asp:Label ID="Label_TotalDenomination1" runat="server" Text="0" Font-Bold="true"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td>
                                                                                <asp:Label ID="Label_Denomination2" runat="server" Text="N/A" Font-Bold="true"></asp:Label>
                                                                            </td>
                                                                            <td>
                                                                                <asp:Label ID="Label_Quantity2" runat="server" Font-Bold="true" Text="0"></asp:Label>
                                                                            </td>
                                                                            <td>
                                                                                <asp:Label ID="Label_TotalDenomination2" runat="server" Text="0" Font-Bold="true"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td>
                                                                                <asp:Label ID="Label_Denomination3" runat="server" Text="N/A" Font-Bold="true"></asp:Label>
                                                                            </td>
                                                                            <td>
                                                                                <asp:Label ID="Label_Quantity3" runat="server" Font-Bold="true" Text="0"></asp:Label>
                                                                            </td>
                                                                            <td>
                                                                                <asp:Label ID="Label_TotalDenomination3" runat="server" Text="0" Font-Bold="true"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td>
                                                                                <asp:Label ID="Label_Denomination4" runat="server" Text="N/A" Font-Bold="true"></asp:Label>
                                                                            </td>
                                                                            <td>
                                                                                <asp:Label ID="Label_Quantity4" runat="server" Font-Bold="true" Text="0"></asp:Label>
                                                                            </td>
                                                                            <td>
                                                                                <asp:Label ID="Label_TotalDenomination4" runat="server" Text="0" Font-Bold="true"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td style="font-size: 13px; font-family: Arial; background-color: Gray; color: White">
                                                                                <b>Total</b>
                                                                            </td>
                                                                            <td style="font-size: 13px; font-family: Arial; background-color: Gray; color: White">
                                                                            </td>
                                                                            <td style="font-size: 13px; font-family: Arial; background-color: Gray; color: White">
                                                                                <asp:Label ID="Label_TotalRemAmount" runat="server" Font-Bold="true" Text="1000"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                    <table border="0" width="90%" style="color: Black; font-weight: bold; text-align: left"
                                                                        align="center">
                                                                        <tr>
                                                                            <td>
                                                                                Snapshot captured completed at
                                                                            </td>
                                                                            <td>
                                                                                <asp:Label ID="Label_QueryAt" runat="server"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </fieldset>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                                <td valign="top">
                                                    <table border="0" style="margin-top: 27px">
                                                        <tr>
                                                            <td>
                                                                <asp:Chart ID="Chart2" runat="server" Width="385" Palette="Grayscale" EnableViewState="true"
                                                                    Height="300px">
                                                                    <Titles>
                                                                        <asp:Title Font="Trebuchet MS, 14.25pt, style=Bold" Name="Title1" ForeColor="26, 59, 105"
                                                                            Text="Call Out By Code">
                                                                        </asp:Title>
                                                                    </Titles>
                                                                    <Legends>
                                                                        <asp:Legend BackColor="Transparent" Alignment="Center" Docking="Right" Font="Trebuchet MS, 8.25pt, style=Bold"
                                                                            IsTextAutoFit="False" Name="Default" LegendStyle="Column" Title="Legends" Enabled="false">
                                                                        </asp:Legend>
                                                                    </Legends>
                                                                    <BorderSkin SkinStyle="None"></BorderSkin>
                                                                    <Series>
                                                                        <asp:Series Name="Default" YValuesPerPoint="1" ChartType="Column" IsValueShownAsLabel="true"
                                                                            IsVisibleInLegend="true">
                                                                        </asp:Series>
                                                                    </Series>
                                                                    <ChartAreas>
                                                                        <asp:ChartArea Name="ChartArea1" BackColor="White">
                                                                            <Area3DStyle Rotation="0" />
                                                                            <AxisY LineColor="64, 64, 64, 64" Title="No.Of Activity">
                                                                                <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                                                <MajorGrid LineColor="White" />
                                                                            </AxisY>
                                                                            <AxisX LineColor="64, 64, 64, 64" Title="Problem Type" Interval="1">
                                                                                <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                                                <MajorGrid LineColor="White" />
                                                                            </AxisX>
                                                                        </asp:ChartArea>
                                                                    </ChartAreas>
                                                                </asp:Chart>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td valign="top">
                                                                <fieldset>
                                                                    <legend style="font-family: Verdana; font-size: large; color: Black"><b>Call Out Analysis</b></legend>
                                                                    <table style="color: Black" align="center">
                                                                        <tr>
                                                                            <td align="left">
                                                                                <b>Total No.Of Calls For Current Month</b>
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="Label_CurrentMonthCalls" runat="server" Text="10" Font-Bold="true"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td align="left">
                                                                                <b>No.Of Calls Not Reported To Bank</b>
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="Label_NotReportedToBank" runat="server" Text="10" Font-Bold="true"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td align="left">
                                                                                <b>No.Of Pending Calls</b>
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:Label ID="Label_PendingCalls" runat="server" Text="10" Font-Bold="true"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td>
                                                                                <b>Last Call Out Activity Record Datetime</b>
                                                                            </td>
                                                                            <td>
                                                                                <asp:Label ID="Label_LastActivityAt" runat="server" Font-Bold="true"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </fieldset>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                                <td valign="top">
                                                    <table border="0" style="margin-top: 27px">
                                                        <tr>
                                                            <td>
                                                                <asp:Chart ID="Chart_DFFStatus" runat="server" Width="385px" EnableViewState="true"
                                                                    Height="300px">
                                                                    <Titles>
                                                                        <asp:Title Font="Trebuchet MS, 11.25pt, style=Bold" Name="Title1" ForeColor="26, 59, 105"
                                                                            Text="DFF ATM Status">
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
                                                                            <AxisY LineColor="64, 64, 64, 64" Title="No.Of ATMs">
                                                                                <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                                                <MajorGrid LineColor="White" />
                                                                            </AxisY>
                                                                            <AxisX LineColor="64, 64, 64, 64" Title="Status">
                                                                                <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                                                                <MajorGrid LineColor="White" />
                                                                            </AxisX>
                                                                        </asp:ChartArea>
                                                                    </ChartAreas>
                                                                </asp:Chart>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td valign="top">
                                                                <fieldset>
                                                                    <legend style="font-family: Verdana; font-size: large; color: Black"><b>DFF Detail</b>
                                                                    </legend>
                                                                    <table border="0" width="250px" style="color: Black; text-align: left" align="center">
                                                                        <tr>
                                                                            <td style="font-size: 13px; font-family: Arial; background-color: Gray; color: White">
                                                                                <b>Status</b>
                                                                            </td>
                                                                            <td style="font-size: 13px; font-family: Arial; background-color: Gray; color: White">
                                                                                <b>Total</b>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td>
                                                                                <b>ATMs Sending No Data</b>
                                                                            </td>
                                                                            <td>
                                                                                <asp:Label ID="Label_NoData" runat="server" Text="0" Font-Bold="true"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td>
                                                                                <b>ATMs Sending 2</b>
                                                                            </td>
                                                                            <td>
                                                                                <asp:Label ID="Label_Sending2" runat="server" Text="0" Font-Bold="true"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td>
                                                                                <b>ATMs Sending Same Balance</b>
                                                                            </td>
                                                                            <td>
                                                                                <asp:Label ID="Label_SameBalance" runat="server" Text="0" Font-Bold="true"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td>
                                                                                <b>ATMs having Inconsistent Data</b>
                                                                            </td>
                                                                            <td>
                                                                                <asp:Label ID="Label_InconsistentData" runat="server" Text="0" Font-Bold="true"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td style="font-size: 13px; font-family: Arial; background-color: Gray; color: White">
                                                                                <b>Total</b>
                                                                            </td>
                                                                            <td style="font-size: 13px; font-family: Arial; background-color: Gray; color: White">
                                                                                <asp:Label ID="Label_total" runat="server" Font-Bold="true" Text="0"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </fieldset>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td valign="top">
                                                    <table width="100%" style="color: Black" cellpadding="0" cellspacing="0">
                                                        <tr>
                                                            <td style="background-color: Gray; color: white; text-align: center; font-family: Calibri;
                                                                font-size: 20px">
                                                                <b>Current Orders</b>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td valign="top">
                                                                <div style="height: 200px; width: 100%; overflow: auto">
                                                                    <asp:GridView ID="GridView_CurrentOrders" runat="server" Width="100%" AutoGenerateColumns="false"
                                                                        HorizontalAlign="Center" Font-Names="Calabri" Font-Size="15px" OnRowDataBound="GridView_CurrentOrder_RowDataBound">
                                                                        <HeaderStyle BackColor="Gray" ForeColor="White" />
                                                                        <Columns>
                                                                            <asp:BoundField DataField="title" HeaderText="Title" ItemStyle-HorizontalAlign="Center"
                                                                                HeaderStyle-CssClass="HeadingCell" ItemStyle-Font-Bold="true" />
                                                                            <asp:BoundField DataField="status" HeaderText="Status" ItemStyle-HorizontalAlign="Center"
                                                                                HeaderStyle-CssClass="HeadingCell" />
                                                                            <asp:BoundField DataField="current_order_executed_at" HeaderText="Execution Time"
                                                                                DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" HeaderStyle-CssClass="HeadingCell"
                                                                                ItemStyle-HorizontalAlign="Center" />
                                                                            <asp:BoundField DataField="current_order_remaining_amount" HeaderText="Remaining Amount"
                                                                                HeaderStyle-CssClass="HeadingCell" ItemStyle-HorizontalAlign="Center" />
                                                                        </Columns>
                                                                        <EmptyDataTemplate>
                                                                            No records found
                                                                        </EmptyDataTemplate>
                                                                    </asp:GridView>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                                <td valign="top">
                                                    <table width="100%" style="color: Black" border="0" cellpadding="0" cellspacing="0">
                                                        <tr>
                                                            <td valign="top">
                                                                <table width="100%" border="0" cellpadding="0" cellspacing="0">
                                                                    <tr>
                                                                        <td style="background-color: Gray; color: white; text-align: center; font-family: Calibri;
                                                                            font-size: 20px" colspan="2" valign="top">
                                                                            <b>Top 10 ATMs</b>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td>
                                                                            <table width="100%">
                                                                                <tr>
                                                                                    <td>
                                                                                        <asp:GridView ID="GridView_Transacting" runat="server" Width="100%" AutoGenerateColumns="false"
                                                                                            HorizontalAlign="Center" Font-Names="Calabri" Font-Size="15px">
                                                                                            <HeaderStyle BackColor="Gray" ForeColor="White" />
                                                                                            <Columns>
                                                                                                <asp:TemplateField>
                                                                                                    <ItemTemplate>
                                                                                                        <img src="../images/treeatm1.jpg" />
                                                                                                    </ItemTemplate>
                                                                                                </asp:TemplateField>
                                                                                                <asp:BoundField DataField="title" HeaderText="High Transacting" ItemStyle-HorizontalAlign="Center"
                                                                                                    HeaderStyle-CssClass="HeadingCell" ItemStyle-Font-Bold="true" />
                                                                                                <asp:BoundField DataField="total" HeaderText="Total" ItemStyle-HorizontalAlign="Center"
                                                                                                    HeaderStyle-CssClass="HeadingCell" ItemStyle-Font-Bold="true" />
                                                                                            </Columns>
                                                                                            <EmptyDataTemplate>
                                                                                                No records found
                                                                                            </EmptyDataTemplate>
                                                                                        </asp:GridView>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                        </td>
                                                                        <td>
                                                                            <table width="100%">
                                                                                <tr>
                                                                                    <td>
                                                                                        <asp:GridView ID="GridView_NoTransactingATM" runat="server" Width="100%" AutoGenerateColumns="false"
                                                                                            HorizontalAlign="Center" Font-Names="Calabri" Font-Size="15px">
                                                                                            <HeaderStyle BackColor="Gray" ForeColor="White" />
                                                                                            <Columns>
                                                                                                <asp:TemplateField>
                                                                                                    <ItemTemplate>
                                                                                                        <img src="../images/treeatm1.jpg" />
                                                                                                    </ItemTemplate>
                                                                                                </asp:TemplateField>
                                                                                                <asp:BoundField DataField="title" HeaderText="Low Transacting" ItemStyle-HorizontalAlign="Center"
                                                                                                    HeaderStyle-CssClass="HeadingCell" ItemStyle-Font-Bold="true" />
                                                                                                <asp:BoundField DataField="total" HeaderText="Total" ItemStyle-HorizontalAlign="Center"
                                                                                                    HeaderStyle-CssClass="HeadingCell" ItemStyle-Font-Bold="true" />
                                                                                            </Columns>
                                                                                            <EmptyDataTemplate>
                                                                                                No records found
                                                                                            </EmptyDataTemplate>
                                                                                        </asp:GridView>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                                <td valign="top" style="width: 10%; color: Black">
                                                    <table width="100%" cellpadding="0" cellspacing="0">
                                                        <tr>
                                                            <td style="background-color: Gray; color: white; text-align: center; font-family: Calibri;
                                                                font-size: 20px">
                                                                <b>Today's Activity</b>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td valign="top">
                                                                <div style="height: 200px; width: 100%; overflow: auto">
                                                                    <asp:GridView ID="GridView_RecentActivity" runat="server" Width="100%" AutoGenerateColumns="false"
                                                                        HorizontalAlign="Center" Font-Names="Calabri" Font-Size="15px">
                                                                        <HeaderStyle BackColor="Gray" ForeColor="White" />
                                                                        <%-- <AlternatingRowStyle BackColor="Silver" />--%>
                                                                        <Columns>
                                                                            <asp:BoundField DataField="Task" HeaderText="Task" ItemStyle-HorizontalAlign="Left"
                                                                                HeaderStyle-CssClass="HeadingCell" ItemStyle-Font-Bold="true" />
                                                                            <asp:BoundField DataField="Status" HeaderText="Status" ItemStyle-HorizontalAlign="Left"
                                                                                HeaderStyle-CssClass="HeadingCell" />
                                                                            <asp:BoundField DataField="Count" HeaderText="Count" ItemStyle-HorizontalAlign="Right"
                                                                                HeaderStyle-CssClass="HeadingCell" />
                                                                        </Columns>
                                                                        <EmptyDataTemplate>
                                                                            No records found
                                                                        </EmptyDataTemplate>
                                                                    </asp:GridView>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td valign="top">
                                                    <table width="100%" border="0" style="color: Black" cellpadding="0" cellspacing="0">
                                                        <tr>
                                                            <td style="background-color: Gray; color: white; text-align: center; font-family: Calibri;
                                                                font-size: 20px">
                                                                <b>Current Logged In User</b>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <div style="height: 200px; width: 100%; overflow: auto">
                                                                    <asp:GridView ID="GridView_CurrentLoggedInUser" runat="server" Width="100%" AutoGenerateColumns="false"
                                                                        HorizontalAlign="Center" Font-Names="Calabri" Font-Size="15px">
                                                                        <HeaderStyle BackColor="Gray" ForeColor="White" />
                                                                        <Columns>
                                                                            <asp:BoundField DataField="id" HeaderText="User" ItemStyle-HorizontalAlign="Left"
                                                                                HeaderStyle-CssClass="HeadingCell" ItemStyle-Font-Bold="true" />
                                                                            <asp:BoundField DataField="loggedInAt" HeaderText="Logged In At" ItemStyle-HorizontalAlign="Center"
                                                                                HeaderStyle-CssClass="HeadingCell" ItemStyle-Font-Bold="true" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" />
                                                                            <asp:BoundField DataField="lastRequestAt" HeaderText="Last Request At" ItemStyle-HorizontalAlign="Center"
                                                                                HeaderStyle-CssClass="HeadingCell" ItemStyle-Font-Bold="true" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" />
                                                                        </Columns>
                                                                        <EmptyDataTemplate>
                                                                            No records found
                                                                        </EmptyDataTemplate>
                                                                    </asp:GridView>
                                                                </div>
                                                                <div>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                                <td valign="top">
                                                    <table width="100%" style="color: Black" cellpadding="0" cellspacing="0">
                                                        <tr>
                                                            <td style="background-color: Gray; color: white; text-align: center; font-family: Calibri;
                                                                font-size: 20px">
                                                                <b>Summary 00:00-23:59 (Current Day)</b>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td valign="top">
                                                                <div style="height: 200px; width: 100%; overflow: auto;">
                                                                    <asp:GridView ID="GridView_SummaryOfAlerts" runat="server" Width="100%" AutoGenerateColumns="false"
                                                                        HorizontalAlign="Center" Font-Names="Calabri" Font-Size="15px">
                                                                        <HeaderStyle BackColor="Gray" ForeColor="White" HorizontalAlign="Center" CssClass="HeadingCell" />
                                                                        <%-- <AlternatingRowStyle BackColor="#eff0ef" />--%>
                                                                        <%--<AlternatingRowStyle BackColor="LightSeaGreen" />--%>
                                                                        <Columns>
                                                                            <asp:BoundField DataField="Task" HeaderText="Task" ItemStyle-HorizontalAlign="Left"
                                                                                HeaderStyle-CssClass="HeadingCell" ItemStyle-Font-Bold="true" />
                                                                            <asp:BoundField DataField="unresolved" HeaderText="Unresolved" ItemStyle-HorizontalAlign="Right"
                                                                                HeaderStyle-CssClass="HeadingCell" />
                                                                            <asp:BoundField DataField="resolved" HeaderText="Resolved" ItemStyle-HorizontalAlign="Right"
                                                                                HeaderStyle-CssClass="HeadingCell" />
                                                                            <asp:BoundField DataField="total" HeaderText="Total" ItemStyle-HorizontalAlign="Right"
                                                                                HeaderStyle-CssClass="HeadingCell" />
                                                                            <asp:TemplateField ShowHeader="False" HeaderText="Details" ItemStyle-HorizontalAlign="Center"
                                                                                HeaderStyle-CssClass="HeadingCell">
                                                                                <ItemTemplate>
                                                                                    <div id="ShowHideRightsIcon" runat="server">
                                                                                        <img style="border: 0" src="../images/edit.gif" alt="click to view details" onclick="openDialog('<%#Eval("alert_type_id") %>','<%#Eval("entity_type") %>')"
                                                                                            class="ShowHand" />
                                                                                    </div>
                                                                                </ItemTemplate>
                                                                                <HeaderStyle ForeColor="black" />
                                                                            </asp:TemplateField>
                                                                        </Columns>
                                                                        <EmptyDataTemplate>
                                                                            No records found
                                                                        </EmptyDataTemplate>
                                                                    </asp:GridView>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                                <td valign="top">
                                                    <table width="100%" style="color: Black" cellpadding="0" cellspacing="0">
                                                        <tr>
                                                            <td style="background-color: Gray; color: white; text-align: center; font-family: Calibri;
                                                                font-size: 20px">
                                                                <b>My Tasks</b>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <div style="height: 200px; width: 100%; overflow: auto">
                                                                    <asp:GridView ID="GridView_UserTasks" runat="server" Width="100%" AutoGenerateColumns="false"
                                                                        HorizontalAlign="Center" Font-Names="Calabri" Font-Size="15px">
                                                                        <HeaderStyle BackColor="Gray" ForeColor="White" />
                                                                        <Columns>
                                                                            <asp:HyperLinkField HeaderText="Title" HeaderStyle-CssClass="HeadingCell" DataTextField="Title"
                                                                                DataNavigateUrlFormatString="~/CCMS/UserTask.aspx?a={0}" DataNavigateUrlFields="atm_id"
                                                                                ItemStyle-Font-Bold="true" />
                                                                            <asp:BoundField DataField="user_full_name" HeaderText="User" SortExpression="user_full_name"
                                                                                HeaderStyle-CssClass="HeadingCell" />
                                                                            <asp:BoundField DataField="creation_time" HeaderText="Creation Time" SortExpression="creation_time"
                                                                                HeaderStyle-CssClass="HeadingCell" DataFormatString="{0:dd/MM/yyyy}" />
                                                                            <asp:BoundField DataField="status" HeaderText="Status" SortExpression="Status" HeaderStyle-CssClass="HeadingCell" />
                                                                            <asp:BoundField DataField="task_type_name" HeaderText="Task" SortExpression="task_type_name"
                                                                                HeaderStyle-CssClass="HeadingCell" />
                                                                        </Columns>
                                                                        <EmptyDataTemplate>
                                                                            No records found
                                                                        </EmptyDataTemplate>
                                                                    </asp:GridView>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <table width="100%" style="color: Black" cellspacing="0">
                                                        <tr>
                                                            <td style="background-color: Gray; color: white; text-align: right; font-family: Calibri;
                                                                font-size: 20px">
                                                                <b>DFF Stopped</b>
                                                            </td>
                                                            <td style="background-color: Gray; color: white; text-align: center; font-family: Calibri;
                                                                font-size: 20px">
                                                                <asp:Button ID="Button_UpdateATM" runat="server" Text="Update" CssClass="button"
                                                                    OnClick="Button_UpdateATM_Click" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td colspan="2">
                                                                <div style="height: 200px; width: 100%; overflow: auto">
                                                                    <asp:GridView ID="GridView_ATM" runat="server" Width="100%" AutoGenerateColumns="false"
                                                                        DataKeyNames="atm_id" HorizontalAlign="Center" Font-Names="Calabri" Font-Size="15px">
                                                                        <HeaderStyle BackColor="Gray" ForeColor="White" />
                                                                        <Columns>
                                                                            <asp:HyperLinkField HeaderText="Title" HeaderStyle-CssClass="HeadingCell" DataTextField="Title"
                                                                                DataNavigateUrlFormatString="~/CCMS/UserTask.aspx?a={0}" DataNavigateUrlFields="atm_id" />
                                                                            <%--<asp:BoundField DataField="title" HeaderText="Title" ItemStyle-HorizontalAlign="Center"
                                                                                                HeaderStyle-CssClass="HeadingCell" ItemStyle-Font-Bold="true" />--%>
                                                                            <asp:TemplateField HeaderText="Enable" HeaderStyle-CssClass="HeadingCell" ItemStyle-HorizontalAlign="Center">
                                                                                <ItemTemplate>
                                                                                    <asp:CheckBox ID="chkbx" runat="server" Checked="true"></asp:CheckBox>
                                                                                </ItemTemplate>
                                                                            </asp:TemplateField>
                                                                        </Columns>
                                                                        <EmptyDataTemplate>
                                                                            No records found
                                                                        </EmptyDataTemplate>
                                                                    </asp:GridView>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                                <td>
                                                    <table width="100%" style="color: Black">
                                                        <tr>
                                                            <td style="background-color: Gray; color: white; text-align: center; font-family: Calibri;
                                                                font-size: 20px">
                                                                <b>Replenishments Without Order</b>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <div style="height: 200px; width: 100%; overflow: auto">
                                                                    <asp:GridView ID="GridView_Rep_Without_Order" runat="server" Width="100%" AutoGenerateColumns="false"
                                                                        HorizontalAlign="Center" Font-Names="Calabri" Font-Size="15px" OnRowDataBound="GridView_CurrentOrder_RowDataBound">
                                                                        <HeaderStyle BackColor="Gray" ForeColor="White" />
                                                                        <Columns>
                                                                            <asp:BoundField DataField="title" HeaderText="Title" ItemStyle-HorizontalAlign="Center"
                                                                                HeaderStyle-CssClass="HeadingCell" ItemStyle-Font-Bold="true" />
                                                                            <asp:BoundField DataField="rep_datetime" HeaderText="Execution Time" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}"
                                                                                HeaderStyle-CssClass="HeadingCell" ItemStyle-HorizontalAlign="Center" />
                                                                            <asp:BoundField DataField="amount" HeaderText="Amount" HeaderStyle-CssClass="HeadingCell"
                                                                                ItemStyle-HorizontalAlign="Center" />
                                                                        </Columns>
                                                                        <EmptyDataTemplate>
                                                                            No records found
                                                                        </EmptyDataTemplate>
                                                                    </asp:GridView>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                                <td valign="top">
                                                    <table width="100%" style="color: Black">
                                                        <tr>
                                                            <td style="background-color: Gray; color: white; text-align: center; font-family: Calibri;
                                                                font-size: 20px">
                                                                <b>CIT Tasks</b>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <div style="height: 200px; width: 100%; overflow: auto">
                                                                    <asp:GridView ID="GridView_CITTasks" runat="server" Width="100%" AutoGenerateColumns="false"
                                                                        HorizontalAlign="Center" Font-Names="Calabri" Font-Size="15px" OnRowDataBound="GridView_CurrentOrder_RowDataBound">
                                                                        <HeaderStyle BackColor="Gray" ForeColor="White" />
                                                                        <Columns>
                                                                            <asp:BoundField DataField="task" HeaderText="Task" ItemStyle-HorizontalAlign="Left"
                                                                                HeaderStyle-CssClass="HeadingCell" ItemStyle-Font-Bold="true" />
                                                                            <asp:BoundField DataField="count" HeaderText="Count" HeaderStyle-CssClass="HeadingCell"
                                                                                ItemStyle-HorizontalAlign="Right" />
                                                                        </Columns>
                                                                        <EmptyDataTemplate>
                                                                            No records found
                                                                        </EmptyDataTemplate>
                                                                    </asp:GridView>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <%--  </div>
                            </div>--%>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <ComponentArt:CallBack ID="DialogCallBack" runat="server" Width="0" Height="0" OnCallback="DialogCallBack_Callback">
    </ComponentArt:CallBack>
    <ComponentArt:Dialog ContentCssClass="contentCss" FooterCssClass="footerCss" HeaderCssClass="headerCss"
        CssClass="dialogCss" Icon="" Value="" Title="AJAX TabStrip Content" CloseTransition="None"
        ModalMaskCssClass="modalMaskCssClass" AllowDrag="false" Alignment="TopCentre"
        Modal="true" ID="Dialog1" runat="server" Width="810" ContentClientTemplateId="data"
        AnimationType="None" AnimationPath="Direct" AnimationSlide="Linear" ShowTransition="None"
        TransitionDuration="0" AnimationDuration="0">
        <ClientTemplates>
            <ComponentArt:ClientTemplate ID="data">
                <table cellpadding="0" style="border-color: #c9c9c9" cellspacing="0" border="0" width="100%"
                    onmousedown="Dialog1.StartDrag(event);">
                    <tr>
                        <td style="background-image: url(../images/DialogImages/top.gif); height: 45px;">
                            <table width="100%" cellpadding="0" cellspacing="0" border="0">
                                <tr>
                                    <td valign="middle" style="padding-left: 4px; color: white; font-size: 15px; font-family: Verdana;
                                        font-weight: bold;">
                                        Information
                                    </td>
                                    <td align="right" style="padding-right: 10px;">
                                        <img src="../images/DialogImages/close.gif" onclick="Dialog1.Close();" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td style="border-color: #c9c9c9; background-color: white; font-size: 12px; font-family: Arial;
                            padding: 10px;">
                            <div id="summaryInfo" style="overflow: auto; height: 400px">
                            </div>
                        </td>
                    </tr>
                </table>
            </ComponentArt:ClientTemplate>
        </ClientTemplates>
    </ComponentArt:Dialog>
    <ComponentArt:Dialog ContentCssClass="contentCss" FooterCssClass="footerCss" HeaderCssClass="headerCss"
        CssClass="dialogCss" Icon="" Value="" Title="AJAX TabStrip Content" CloseTransition="None"
        ModalMaskCssClass="modalMaskCssClass" AllowDrag="false" Alignment="TopCentre"
        Modal="true" ID="Dialog2" runat="server" Width="610" ContentClientTemplateId="ClientTemplate1"
        AnimationType="None" AnimationPath="Direct" AnimationSlide="Linear" ShowTransition="None"
        TransitionDuration="0" AnimationDuration="0">
        <ClientTemplates>
            <ComponentArt:ClientTemplate ID="ClientTemplate1">
                <table cellpadding="0" style="border-color: #c9c9c9" cellspacing="0" border="0" width="100%"
                    onmousedown="Dialog2.StartDrag(event);">
                    <tr>
                        <td style="background-image: url(../images/DialogImages/top.gif); height: 45px;">
                            <table width="100%" cellpadding="0" cellspacing="0" border="0">
                                <tr>
                                    <td valign="middle" style="padding-left: 4px; color: white; font-size: 15px; font-family: Verdana;
                                        font-weight: bold;">
                                        Information
                                    </td>
                                    <td align="right" style="padding-right: 10px;">
                                        <img src="../images/DialogImages/close.gif" onclick="Dialog2.Close();" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td style="border-color: #c9c9c9; background-color: white; font-size: 12px; font-family: Arial;
                            padding: 10px;">
                            <div id="ATM" style="overflow: auto; height: 400px">
                            </div>
                        </td>
                    </tr>
                </table>
            </ComponentArt:ClientTemplate>
        </ClientTemplates>
    </ComponentArt:Dialog>
    </form>
    <div id="divWait" runat="server" style="position: absolute; top: 200px; right: 45.5em;">
        <asp:Image ID="Image_loading" runat="server" ImageUrl="../images/busy.gif" />
    </div>
</body>
</html>
