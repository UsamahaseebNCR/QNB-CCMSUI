<%--<%@ OutputCache Duration="60" VaryByParam="None" VaryByCustom="browser" %>        --%>
<%@ Page Language="C#" AutoEventWireup="true" Codebehind="Home.aspx.cs" Inherits="CCMSUI.Home" EnableViewState="false"
    Title="iJournal - Home" %>
<%--<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>--%>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>iJournal 3 Home</title>
    <link href="content/iSuite.css" rel="stylesheet" />
      <script src="content/shortcut.js" type="text/javascript" language="javascript">
    </script>
    <script type="text/javascript">
        window.parent.window.contentPage = '';  


    </script>

</head>
<body onload="hideProgressBar();" >
   <%--  <ComponentArt:TabStrip ID="TabStrip_AddEdit" CssClass="TopGroup" DefaultItemLookId="DefaultTabLook"
                    DefaultSelectedItemLookId="SelectedTabLook" DefaultDisabledItemLookId="DisabledTabLook"
                    DefaultGroupTabSpacing="1px" ImagesBaseUrl="../images/Tabimages/" MultiPageId="MultiPage_AddEdit"
                    runat="server" ScrollDownLookId="DefaultTabLook" ScrollLeftLookId="DefaultTabLook"
                    ScrollRightLookId="DefaultTabLook" ScrollUpLookId="DefaultTabLook" TopGroupTabSpacing="1px">
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
                        <ComponentArt:TabStripTab ID="TabStripTab_general" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                            ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                            DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                            Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                            SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Home">
                        </ComponentArt:TabStripTab>
                        <ComponentArt:TabStripTab ID="TabStripTab_physicalConfig" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                            ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                            DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                            Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                            SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Administration">
                        </ComponentArt:TabStripTab>
                        <ComponentArt:TabStripTab ID="TabStripTab_Operations" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                            ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                            DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                            Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                            SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Operations">
                        </ComponentArt:TabStripTab>
                          <ComponentArt:TabStripTab ID="TabStripTab_Reports" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                            ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                            DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                            Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                            SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Reports">

                        </ComponentArt:TabStripTab>
                          <ComponentArt:TabStripTab ID="TabStripTab_Others" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                            ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                            DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                            Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                            SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Others">
                        </ComponentArt:TabStripTab>
                        </Tabs>
                </ComponentArt:TabStrip>
                 <ComponentArt:MultiPage ID="MultiPage_AddEdit" CssClass="MultiPage" runat="server"
                    Width="100%">
                    <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView_general"
                        Width="100%">
                        test
                    </ComponentArt:PageView>
                    </ComponentArt:MultiPage>
    
    --%>
    
    <div class="PageHeading" >
       <div style="padding-top:5px;">
        Please select your option
        </div>
        </div>
    <br />
        <div id="menus" style="padding-right: 10px; padding-left: 10px">
        &nbsp;
        <div class="DIVHomePage" id="DivSummary" runat="server">
            <table cellpadding="0" class="DIVHomePage" cellspacing="0" border="0">
                <tr style="background-image: url(images/welcomeButtonsBG.jpg); height: 100px; width: 158px;">
                    <td style="width: 158px; cursor: pointer; height: 100px;" onclick="location.href='ccms/summary.aspx'">
                        Summary<br />
                        <img alt="" src="images/icon_administration.jpg" style="cursor: pointer" /></td>
                </tr>
            </table>
        </div>
        &nbsp;
        <div class="DIVHomePage" id="DivAdmin" runat="server">
            <table cellpadding="0" class="DIVHomePage" cellspacing="0" border="0">
                <tr style="background-image: url(images/welcomeButtonsBG.jpg); height: 100px; width: 158px;">
                    <td style="width: 158px; cursor: pointer; height: 100px;" onclick="location.href='admin/administration.aspx'">
                        Administration<br />
                        <img alt="" src="images/icon_administration.jpg" style="cursor: pointer" /></td>
                </tr>
            </table>
        </div>
        
        &nbsp;
        <div class="DIVHomePage" id="DivOperations" runat="server">
            <table cellpadding="0" class="DIVHomePage" cellspacing="0" border="0">
                <tr style="background-image: url(images/welcomeButtonsBG.jpg); height: 100px; width: 158px;">
                    <td style="width: 158px; cursor: pointer; height: 100px;" onclick="location.href='admin/operations.aspx'">
                        Operations<br />
                        <img alt="" src="images/icon_operations.jpg" style="cursor: pointer" /></td>
                </tr>
            </table>
        </div>
       
        &nbsp;
        <div class="DIVHomePage" id="DivReports" runat="server">
            <table cellpadding="0" class="DIVHomePage" cellspacing="0" border="0">
                <tr style="background-image: url(images/welcomeButtonsBG.jpg); height: 100px; width: 158px;">
                    <td style="width: 158px; cursor: pointer; height: 100px;" onclick="location.href='CCMS/reportList.aspx'">
                        Reports<br />
                        <img alt="" src="images/icon_reports.jpg" style="cursor: pointer" /></td>
                </tr>
            </table>
        </div>
        &nbsp;
        <div class="DIVHomePage" id="DivOthers" runat="server">
            <table cellpadding="0" class="DIVHomePage" cellspacing="0" border="0">
                <tr style="background-image: url(images/welcomeButtonsBG.jpg); height: 100px; width: 158px">
                    <td style="width: 158px; cursor: pointer; height: 100px;" onclick="location.href='admin/Other.aspx'">
                        Others<br />
                        <img alt="" src="images/icon_other.jpg" style="cursor: pointer" /></td>
                </tr>
            </table>
        </div>
        
        
        
       <%-- &nbsp;
        <div class="DIVHomePage" id="DivGroups" runat="server">
            <table cellpadding="0" class="DIVHomePage" cellspacing="0" border="0">
                <tr style="background-image: url(images/welcomeButtonsBG.jpg); height: 100px; width: 158px">
                    <td style="width: 158px; cursor: pointer" onclick="location.href='admin/GroupUser.aspx'">
                        Groups<br />
                        <img alt="" src="images/icon_welcome_groups.jpg" style="cursor: pointer" /></td>
                </tr>
            </table>
        </div>
        &nbsp;
              <div class="DIVHomePage" id="DivAppConfig" runat="server">
            <table cellpadding="0" cellspacing="0" class="DIVHomePage" border="0">
                <tr style="background-image: url(images/welcomeButtonsBG.jpg); height: 100px; width: 158px">
                    <td style="width: 158px; cursor: pointer" onclick="location.href='CCMS/Configure.aspx'">
                        Application Configuration<br />
                        <img alt="" src="images/icon_welcome_applicationconfiguration.jpg" style="cursor: pointer" 
                          
                           /></td>
                          
                </tr>
            </table>
        </div>
       &nbsp;
       <div class="DIVHomePage" id="DivScheduleDownload" runat="server">
            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                <tr style="background-image: url(images/welcomeButtonsBG.jpg); height: 100px; width: 158px">
                    <td style="width: 158px; cursor: pointer" onclick="location.href='ccms/ScheduledDownload.aspx'">
                        Schedule Downloads<br />
                        <img alt="" src="images/icon_welcome_filedownload.jpg" /></td>
                </tr>
            </table>
        </div>
        &nbsp;
        <div class="DIVHomePage" id="DivReports" runat="server">
            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                <tr style="background-image: url(images/welcomeButtonsBG.jpg); height: 100px; width: 158px">
                    <td style="width: 158px; cursor: pointer" onclick="location.href='CCMS/ReportList.aspx'">
                        Reports<br />
                        <img alt="" src="images/reports.jpg" />
                    </td>
                </tr>
            </table>
        </div>
        <div class="DIVHomePage" id="DivAuditLog" runat="server">
            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                <tr style="background-image: url(images/welcomeButtonsBG.jpg); height: 100px; width: 158px">
                    <td style="width: 158px; cursor: pointer" onclick="location.href='CCMS/AuditLog.aspx'">
                        Activity Log<br />
                        <img alt="" src="images/icon_ActivityLog.jpg"  />
                    </td>
                </tr>
            </table>
        </div>
      &nbsp;
        <div class="DIVHomePage" id="DivAgentMonitoring" runat="server">
            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                <tr style="background-image: url(images/welcomeButtonsBG.jpg); height: 100px; width: 158px">
                    <td style="width: 158px; cursor: pointer" onclick="location.href='CCMS/AgentMonitoring.aspx'">
                        Agent Monitoring<br />
                        <img alt="" src="images/Icon_AgentMonitoring.jpg"  />
                    </td>
                </tr>
            </table>
        </div>&nbsp;
        <div class="DIVHomePage" id="DivTerminalFileStatusMonitoring" runat="server">
            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                <tr style="background-image: url(images/welcomeButtonsBG.jpg); height: 100px; width: 158px">
                    <td style="width: 158px; cursor: pointer" onclick="location.href='CCMS/TerminalFileStatusMonitoring.aspx'">
                        Task Status Monitoring<br />
                        <img alt="" src="images/Icon_taskStatusMonitoring.jpg"  />
                    </td>
                </tr>
            </table>
        </div>
        &nbsp;
         <div class="DIVHomePage" id="DivFtpStatus" runat="server">
            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                <tr style="background-image: url(images/welcomeButtonsBG.jpg); height: 100px; width: 158px">
                    <td style="width: 158px; cursor: pointer" onclick="location.href='CCMS/FtpFileTransferStatus.aspx'">
                        FTP File Transfer Status<br />
                        <img alt="" src="images/Icon_fileTransferStatus.jpg"  />
                    </td>
                </tr>
            </table>
        </div>
        &nbsp;
        <div class="DIVHomePage" id="DivCashOrderMonitoring" runat="server">
            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                <tr style="background-image: url(images/welcomeButtonsBG.jpg); height: 100px; width: 158px">

                    <td style="width: 158px; cursor: pointer" onclick="location.href='CCMS/CashOrderMonitoring.aspx'">
                        Cash Order Monitoring<br />
                        <img alt="" src="images/Icon_cashOrderMonitoring.jpg" id="IMG3"  />

                    </td>
                </tr>
            </table>
        </div>
        &nbsp;
        <div class="DIVHomePage" id="DivOutOfBalanceInvestigation" runat="server">
            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                <tr style="background-image: url(images/welcomeButtonsBG.jpg); height: 100px; width: 158px">
                    <td style="width: 158px; cursor: pointer" onclick="location.href='CCMS/OutOfBalanceInvestigation.aspx'">
                        Out Of Balance Investigation<br />
                        <img alt="" src="images/icon_outOfBalanceInvestigat.jpg" id="IMG4"  />
                    </td>
                </tr>
            </table>
        </div>
        
        &nbsp;
        <div class="DIVHomePage" id="Div1" runat="server">
            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                <tr style="background-image: url(images/welcomeButtonsBG.jpg); height: 100px; width: 158px">
                    <td style="width: 158px; cursor: pointer" onclick="window.open('CCMS/Dashboards/DashBoardGrid_Alerts.aspx','','location=0,status=1,scrollbars=0,resizable=yes,fullscreen=yes, scrollbars=auto');">
                        Alert Dashboard<br />
                        <img alt="" src="images/icon_outOfBalanceInvestigat.jpg" id="IMG1"  />
                    </td>
                </tr>
            </table>
        </div> &nbsp;
        <div class="DIVHomePage" id="Div2" runat="server">
            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                <tr style="background-image: url(images/welcomeButtonsBG.jpg); height: 100px; width: 158px">
                    <td style="width: 158px; cursor: pointer" onclick="window.open('CCMS/Dashboards/DashBoardGrid_FTP.aspx','','location=0,status=1,scrollbars=0,resizable=yes,fullscreen=yes, scrollbars=auto');">
                        FTP Dashboard<br />
                        <img alt="" src="images/icon_outOfBalanceInvestigat.jpg" id="IMG2"  />
                    </td>
                </tr>
            </table>
        </div>&nbsp;
        <div class="DIVHomePage" id="Div3" runat="server">
            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                <tr style="background-image: url(images/welcomeButtonsBG.jpg); height: 100px; width: 158px">
                    <td style="width: 158px; cursor: pointer" onclick="window.open('CCMS/Dashboards/DashBoardGrid_TAsks.aspx','','location=0,status=1,scrollbars=0,resizable=yes,fullscreen=yes, scrollbars=auto');">
                        Task Dashboard<br />
                        <img alt="" src="images/icon_outOfBalanceInvestigat.jpg" id="IMG5"  />
                    </td>
                </tr>
            </table>
        </div>
        
        &nbsp;
        <div class="DIVHomePage" id="Div4" runat="server">
            <table cellpadding="0" cellspacing="0" border="0" class="DIVHomePage">
                <tr style="background-image: url(images/welcomeButtonsBG.jpg); height: 100px; width: 158px">
                    <td style="width: 158px; cursor: pointer" onclick="window.open('CCMS/Dashboards/DashBoardGrid_CashOrderMonitoring.aspx','','location=0,status=1,scrollbars=0,resizable=yes,fullscreen=yes, scrollbars=auto');">
                        Cash Order Dashboard<br />
                        <img alt="" src="images/icon_outOfBalanceInvestigat.jpg" id="IMG6"  />
                    </td>
                </tr>
            </table>
        </div>
--%>
    </div>
    <form id="form1" runat="server">
    </form>
</body>
</html>
