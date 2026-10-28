<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Main.aspx.cs" Inherits="iSuite.Main" %>

<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
   <script type="text/javascript">

       window.onunload = function () {
           
       }
       var IDLE_TIMEOUT =  <%= Session.Timeout %> * 60; //seconds
       var _idleSecondsTimer = null;
       var _idleSecondsCounter = 0;

       document.onclick = function () {
           _idleSecondsCounter = 0;
       };

       document.onmousemove = function () {
           _idleSecondsCounter = 0;
       };

       document.onkeypress = function () {
           _idleSecondsCounter = 0;
       };

       _idleSecondsTimer = window.setInterval(CheckIdleTime, 1000);

       function CheckIdleTime() {
           _idleSecondsCounter++;
           if (_idleSecondsCounter >= IDLE_TIMEOUT) {
               window.clearInterval(_idleSecondsTimer);
               //alert("Time expired!");
               document.location.href = "SignIn.aspx";
           }
       }


      <%-- var iddletine = 0;
       var timelimit = <%= Session.Timeout %> * 60;
       var timeinterval = "";
       document.onclick = function () {
           iddletine = 0;
       }
       document.onkeyup = function () {
           iddletine = 0;
       }
       document.onkeydown = function () {
           iddletine = 0;
       }
       document.onmousemove = function () {
           iddletine = 0;
       }
       document.onmousewheel = function () {
           iddletine = 0;
       }
       function CheckIddle() {
           timeinterval = setInterval(function () {
               iddletine += 60;
               if (iddletine == timelimit || iddletine > timelimit) {
                   clearInterval(timeinterval);
                   window.location.href = "SignIn.aspx";
               }
           }, 60000);
       }
       CheckIddle();--%>
</script>
    <title>ATM View 360</title>
    <link href="content/menuStyle.css" rel="stylesheet" type="text/css" />
    <link href="content/iSuite.css" rel="stylesheet" />
    <link href="content/treeStyle.css" rel="stylesheet" />
    <link href="content/demos.css" rel="stylesheet" />
    <script src="content/boxover.js" type="text/javascript" language="javascript">
    </script>
    <script src="content/shortcut.js" type="text/javascript" language="javascript">
    </script>
    <script type="text/javascript">
        // Preload CSS-referenced images
        (new Image()).src = 'images/item_bg.gif';
        (new Image()).src = 'images/item_bg_hover.gif';
    </script>
    <script src="content/main.js" type="text/javascript" language="javascript">
    </script>
    <script type="text/javascript" language="javascript">
        function windowClosed() {

            if (event.clientY < 0) {
                Callback_SessionEnd.callback();
                //  alert('win closed');
            }

        }

        function SetDialogTextBoxFocus() {
            //document.getElementById('NewRegionName').focus();
            ALERT('D');
        }
        var counter = 1; //Done to avoid executing this function only for the first body load event;
        function hide() {
            if (counter != 1) {
                if (document.getElementById('contentFrame').keyCode == 89)
                    hideSplitter();
                document.getElementById('contentFrame').keyCode = 0;
            }
            counter = 2;
        }
        function loadLastVisitedPage() {
            document.getElementById('contentFrame').src = document.getElementById('lastVisitedPage').value;
        }
    
    </script>
    <%--<style type="text/css">
        a:link
        {
            color: #000000;
            text-decoration: none;
        }
        a:visited
        {
            color: #000000;
            text-decoration: none;
        }
        a:visited:hover
        {
            text-decoration: underline;
        }
        a:hover
        {
            text-decoration: underline;
        }
        a:active
        {
            color: #ffffff;
            text-decoration: underline;
        }
        
        .test
        {
        }
    </style>--%>
</head>
<body style="font-family: Verdana; color: #000000; font-size: 12px; margin: 0" onload="initFrame();loadLastVisitedPage();"
    onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)'
    onbeforeunload="windowClosed();" onfocus="hide();" oncontextmenu="return false;">
    <form id="form1" runat="server">
    <div id="Header">
        <input type="hidden" id="lastVisitedPage" runat="server" value="home.aspx" />
        <table cellpadding="0" cellspacing="0" border="0" width="100%">
            <tr style="background-image: url(images/background.png); width:100%;z-index:1">
                <td >
                   <%-- <img   src="images/bg-brand.png" height="114px" width="900" />--%>
                     <asp:Literal ID="Literal_script" runat="server" EnableViewState="false"></asp:Literal>
                    <div style="position: absolute; top: 78px; left: 50px; width: 1000px; padding: 4px;
                    
                        font-family: Verdana; font-size: 11px;">
                        <table width="100%" border="0" cellpadding="0" cellspacing="0">
                            <tr>
                                <td style="color: Yellow; font-weight: bold">
                                    <b>
                                        <asp:Label ID="Label_DateTime" runat="server"></asp:Label>:: <b>Welcome
                                            <asp:Label ID="Label_LoggedInUser" runat="server"></asp:Label>
                                            &nbsp;:: Last Logged In:<asp:Label ID="Label_LastLoggedInAt" runat="server"/>
                                                                                                     </b>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <center>
                        <div style="position: absolute; top:120px; left: 430px; width: 300px; padding: 4px;
                            font-family: Verdana; font-size: 11px;">
                            <table width="100%" border="0" cellpadding="0" cellspacing="0">
                                <tr>
                                    <td>
                                        <div id="notificationArea" style="height: 2px; text-align: right;">
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </center>
                </td>
                <td>
                    <div>
                        <table border="0" cellpadding="0" cellspacing="0" style="width:100%; height: 120px; ">
                            <tr>
                                <td style="font-family:Arial Narrow; color: White; font-size:48px; text-align:right; vertical-align:top;text-align:left; padding-left:50px;padding-top:20px">
                                    <b>VIEW360</b>

                                </td>
                                <td style="font-family:Arial Narrow; color: White; font-size:48px; vertical-align:top; text-align:right; padding-top:20px; padding-right:50px;">
                                     <img src="images/logo.png" width="200px" height="70px" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <div id="menu" style="position: absolute; top: 90px; right: 0.5em; padding: 4px;
                                        width: 98.5%">
                                        <table width="100%" border="0" cellpadding="0" cellspacing="0">
                                            <tr>
                                                <%-- <td style="text-align:left;  padding-left:15px; width:50px">--%>
                                                <%--<div id="Divsummary" runat="server">
                                                             
                                                                <a onclick="document.getElementById('contentFrame').src = 'ccms/summary.aspx';"
                                                                 style="cursor:hand; padding-left:3px" >Summary</a>
                                                            </div>--%>
                                                <%--   </td>--%>
                                                <%--                                           <td class="container" style="padding-left:5px" >--%>
                                                <td style="padding-left: 15px;">
                                                    <div style="float: left">
                                                        <ComponentArt:Menu ID="MenuHome" runat="server" Orientation="Horizontal" DefaultGroupCssClass="MenuGroup"
                                                            DefaultItemLookId="DefaultItemLook" TopGroupItemSpacing="0" DefaultGroupItemSpacing="1"
                                                            CssClass="MenuGroup" ExpandTransition="Fade" CollapseTransition="Fade" ExpandDelay="100"
                                                            ImagesBaseUrl="images/" EnableViewState="false">
                                                            <Items>
                                                            </Items>
                                                            <ItemLooks>
                                                                <ComponentArt:ItemLook LookId="TopItemLook" CssClass="MenuItem" HoverCssClass="MenuItemHover"
                                                                    ActiveCssClass="MenuItemActive" ExpandedCssClass="MenuItemExpanded" LeftIconUrl="grayDots.gif"
                                                                    HoverLeftIconUrl="orangeDots.gif" ExpandedLeftIconUrl="grayDots.gif" LeftIconWidth="12"
                                                                    LeftIconHeight="8" LabelPaddingLeft="5" LabelPaddingRight="15" LabelPaddingTop="1"
                                                                    LabelPaddingBottom="1" />
                                                                <ComponentArt:ItemLook LookId="DefaultItemLook" CssClass="MenuItem" HoverCssClass="MenuItemHover"
                                                                    ActiveCssClass="MenuItemActive" ExpandedCssClass="MenuItemExpanded" LeftIconUrl="grayDots.gif"
                                                                    HoverLeftIconUrl="orangeDots.gif" ExpandedLeftIconUrl="grayDots.gif" LeftIconWidth="12"
                                                                    LeftIconHeight="8" LabelPaddingLeft="5" LabelPaddingRight="15" LabelPaddingTop="1"
                                                                    LabelPaddingBottom="1" RightIconUrl="arrow_white.gif" RightIconWidth="15" RightIconHeight="10"
                                                                    RightIconVisibility="WhenExpandable" />
                                                                <ComponentArt:ItemLook LookId="ItemBreak" ImageUrl="break.gif" CssClass="MenuBreak" />
                                                            </ItemLooks>
                                                        </ComponentArt:Menu>
                                                        <%--</td><td>--%>
                                                    </div>
                                                    <div style="float: left">
                                                        <ComponentArt:Menu ID="MenuAdmin" runat="server" DefaultGroupCssClass="MenuGroup"
                                                            DefaultItemLookId="DefaultItemLook" ExpandDelay="100" ImagesBaseUrl="images/"
                                                            EnableViewState="false">
                                                            <Items>
                                                            </Items>
                                                            <ItemLooks>
                                                                <ComponentArt:ItemLook LookId="TopItemLook" CssClass="MenuItem" HoverCssClass="MenuItemHover"
                                                                    ActiveCssClass="MenuItemActive" ExpandedCssClass="MenuItemExpanded" LeftIconUrl="grayDots.gif"
                                                                    HoverLeftIconUrl="orangeDots.gif" ExpandedLeftIconUrl="grayDots.gif" LeftIconWidth="12"
                                                                    LeftIconHeight="8" LabelPaddingLeft="5" LabelPaddingRight="15" LabelPaddingTop="1"
                                                                    LabelPaddingBottom="1" />
                                                                <ComponentArt:ItemLook LookId="DefaultItemLook" CssClass="MenuItem" HoverCssClass="MenuItemHover"
                                                                    ActiveCssClass="MenuItemActive" ExpandedCssClass="MenuItemExpanded" LeftIconUrl="grayDots.gif"
                                                                    HoverLeftIconUrl="orangeDots.gif" ExpandedLeftIconUrl="grayDots.gif" LeftIconWidth="12"
                                                                    LeftIconHeight="8" LabelPaddingLeft="5" LabelPaddingRight="15" LabelPaddingTop="1"
                                                                    LabelPaddingBottom="1" RightIconUrl="arrow_white.gif" RightIconWidth="15" RightIconHeight="10"
                                                                    RightIconVisibility="WhenExpandable" />
                                                                <ComponentArt:ItemLook LookId="ItemBreak" ImageUrl="break.gif" CssClass="MenuBreak" />
                                                            </ItemLooks>
                                                        </ComponentArt:Menu>
                                                    </div>
                                                    <%--</td><td>--%>
                                                    <div class="DemoArea" style="float: left">
                                                        <ComponentArt:Menu ID="MenuOperations" runat="server" Orientation="Horizontal" DefaultGroupCssClass="MenuGroup"
                                                            DefaultItemLookId="DefaultItemLook" ExpandDelay="100" ImagesBaseUrl="images/"
                                                            EnableViewState="false">
                                                            <Items>
                                                            </Items>
                                                            <ItemLooks>
                                                                <ComponentArt:ItemLook LookId="TopItemLook" CssClass="MenuItem" HoverCssClass="MenuItemHover"
                                                                    ActiveCssClass="MenuItemActive" ExpandedCssClass="MenuItemExpanded" LeftIconUrl="grayDots.gif"
                                                                    HoverLeftIconUrl="orangeDots.gif" ExpandedLeftIconUrl="grayDots.gif" LeftIconWidth="12"
                                                                    LeftIconHeight="8" LabelPaddingLeft="5" LabelPaddingRight="15" LabelPaddingTop="1"
                                                                    LabelPaddingBottom="1" />
                                                                <ComponentArt:ItemLook LookId="DefaultItemLook" CssClass="MenuItem" HoverCssClass="MenuItemHover"
                                                                    ActiveCssClass="MenuItemActive" ExpandedCssClass="MenuItemExpanded" LeftIconUrl="grayDots.gif"
                                                                    HoverLeftIconUrl="orangeDots.gif" ExpandedLeftIconUrl="grayDots.gif" LeftIconWidth="12"
                                                                    LeftIconHeight="8" LabelPaddingLeft="5" LabelPaddingRight="15" LabelPaddingTop="1"
                                                                    LabelPaddingBottom="1" RightIconUrl="arrow_white.gif" RightIconWidth="15" RightIconHeight="10"
                                                                    RightIconVisibility="WhenExpandable" />
                                                                <ComponentArt:ItemLook LookId="ItemBreak" ImageUrl="break.gif" CssClass="MenuBreak" />
                                                            </ItemLooks>
                                                        </ComponentArt:Menu>
                                                        <%--</td><td>--%>
                                                    </div>
                                                    <div class="DemoArea" style="float: left">
                                                        <ComponentArt:Menu ID="MenuReports" runat="server" Orientation="Horizontal" DefaultGroupCssClass="MenuGroup"
                                                            DefaultItemLookId="DefaultItemLook" ExpandDelay="100" ImagesBaseUrl="images/"
                                                            EnableViewState="false">
                                                            <Items>
                                                            </Items>
                                                            <ItemLooks>
                                                                <ComponentArt:ItemLook LookId="TopItemLook" CssClass="MenuItem" HoverCssClass="MenuItemHover"
                                                                    ActiveCssClass="MenuItemActive" ExpandedCssClass="MenuItemExpanded" LeftIconUrl="grayDots.gif"
                                                                    HoverLeftIconUrl="orangeDots.gif" ExpandedLeftIconUrl="grayDots.gif" LeftIconWidth="12"
                                                                    LeftIconHeight="8" LabelPaddingLeft="5" LabelPaddingRight="15" LabelPaddingTop="1"
                                                                    LabelPaddingBottom="1" />
                                                                <ComponentArt:ItemLook LookId="DefaultItemLook" CssClass="MenuItem" HoverCssClass="MenuItemHover"
                                                                    ActiveCssClass="MenuItemActive" ExpandedCssClass="MenuItemExpanded" LeftIconUrl="grayDots.gif"
                                                                    HoverLeftIconUrl="orangeDots.gif" ExpandedLeftIconUrl="grayDots.gif" LeftIconWidth="12"
                                                                    LeftIconHeight="8" LabelPaddingLeft="5" LabelPaddingRight="15" LabelPaddingTop="1"
                                                                    LabelPaddingBottom="1" RightIconUrl="arrow_white.gif" RightIconWidth="15" RightIconHeight="10"
                                                                    RightIconVisibility="WhenExpandable" />
                                                                <ComponentArt:ItemLook LookId="ItemBreak" ImageUrl="break.gif" CssClass="MenuBreak" />
                                                            </ItemLooks>
                                                        </ComponentArt:Menu>
                                                        <%--</td><td>--%>
                                                    </div>
                                                    <div class="DemoArea" style="float: left">
                                                        <ComponentArt:Menu ID="MenuOthers" runat="server" Orientation="Horizontal" DefaultGroupCssClass="MenuGroup"
                                                            DefaultItemLookId="DefaultItemLook" ExpandDelay="100" ImagesBaseUrl="images/"
                                                            EnableViewState="false">
                                                            <Items>
                                                            </Items>
                                                            <ItemLooks>
                                                                <ComponentArt:ItemLook LookId="TopItemLook" CssClass="MenuItem" HoverCssClass="MenuItemHover"
                                                                    ActiveCssClass="MenuItemActive" ExpandedCssClass="MenuItemExpanded" LeftIconUrl="grayDots.gif"
                                                                    HoverLeftIconUrl="orangeDots.gif" ExpandedLeftIconUrl="grayDots.gif" LeftIconWidth="12"
                                                                    LeftIconHeight="8" LabelPaddingLeft="5" LabelPaddingRight="15" LabelPaddingTop="1"
                                                                    LabelPaddingBottom="1" />
                                                                <ComponentArt:ItemLook LookId="DefaultItemLook" CssClass="MenuItem" HoverCssClass="MenuItemHover"
                                                                    ActiveCssClass="MenuItemActive" ExpandedCssClass="MenuItemExpanded" LeftIconUrl="grayDots.gif"
                                                                    HoverLeftIconUrl="orangeDots.gif" ExpandedLeftIconUrl="grayDots.gif" LeftIconWidth="12"
                                                                    LeftIconHeight="8" LabelPaddingLeft="5" LabelPaddingRight="15" LabelPaddingTop="1"
                                                                    LabelPaddingBottom="1" RightIconUrl="arrow_white.gif" RightIconWidth="15" RightIconHeight="10"
                                                                    RightIconVisibility="WhenExpandable" />
                                                                <ComponentArt:ItemLook LookId="ItemBreak" ImageUrl="break.gif" CssClass="MenuBreak" />
                                                            </ItemLooks>
                                                        </ComponentArt:Menu>
                                                    </div>
                                                    <%--                                  <asp:Menu ID="Menu1" runat="server" Orientation="Horizontal" Target="contentForm">
                <Items>
                    
                </Items>
                <DynamicMenuStyle CssClass="IE8Fix" />
            </asp:Menu>
                                                    --%>
                                                    <%--  <img src="images/icon_Home.png" 
                                                    onmouseout="DivNavigationOptions.style.display='none'"/>--%>
                                                </td>
                                                <%-- <td align="center">
                                                        <img src="images/icon_changepassword.png" /></td>--%>
                                                <td valign="top" style="text-align: right;" id="tdChangePassword" runat="server">
                                                    <img src="images/icon_changepassword.png" style="cursor: hand" onclick="document.getElementById('contentFrame').src = 'admin/changepassword.aspx'"
                                                        alt="Click to change password" title="header=[Change Password] body=[Click to change password.]" />
                                                </td>
                                                <td style="text-align: left">
                                                    <img src="images/logout.gif" alt="Click to log off" onclick="window.location='LogOff.aspx'"
                                                        style="cursor: hand" title="header=[Log Off] body=[Click to log off.]" />
                                                </td>
                                                <%--<asp:ImageButton ID="ImageButtonChangePwd_Logoff" runat="server"   
                                         ImageUrl="~/images/ChangePwd.gif"
                                           AlternateText ="Click to change password" 
                                            ToolTip ="Click to change password"/>
                                        

                                        <asp:ImageButton ID="ImageButton_Logoff" runat="server" OnClick ="ImageButton_Click"
                                         ImageUrl="~/images/logout.gif" 
                                          AlternateText="Click to log off" ToolTip="Click to log off"/>--%>
                                            </tr>
                                        </table>
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </div>
                </td>
               
            </tr>
        </table>
    </div>
    <div style="padding: 0 9px 0 6px">
        <%--<div id="Menu">
                <table cellpadding="0" cellspacing="0" border="0" width="100%" style="margin: 0px">
                    <tr>
                        <td style="width: 10px; height: 23px;" valign="top">
                            <img src="images/box_lefttop.jpg" width="10px" height="23px" /></td>
                        <td width="100%" style="background8px-image: url(images/box_tophorizontal.jpg); height: 23px">
                            <table cellpadding="0" cellspacing="0" border="0" width="300px" align="right" height="19px">
                                <tr height="19px">
                                    <td style="width: 21px" valign="middle">
                                        <img title="Home" src="images/Home.jpg" onclick="document.getElementById('contentFrame').src = 'home.aspx';"
                                            height="16px" style="cursor: hand" /></td>
                                    <td style="width: 21px" valign="middle">
                                        <img title="CCMS Version Information" src="images/version.jpg" onclick="document.getElementById('contentFrame').src = 'home.aspx';"
                                            height="15px" style="display: none" />&nbsp;&nbsp;&nbsp;&nbsp;</td>
                                    <td>
                                        <a style="visibility: hidden" href="javascript: void(0)" onclick="document.getElementById('contentFrame').src = 'ij/summary.aspx';">
                                            Summary</a></td>
                                    <td>
                                        
                                    </td>
                                    <td>
                                       
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td style="width: 10px; height: 23px;" valign="top">
                            <img src="images/box_righttop.jpg" width="10px" height="23px" /></td>
                    </tr>
                    <tr valign="top">
                        <td style="width: 10px" valign="top" height="2px">
                            <img src="images/box_leftvertical_10px.jpg" width="10px" height="2px" /></td>
                        <td style="width: 100%" valign="top" height="2px">
                            <img src="images/menulinebottom.jpg" width="100%" height="2px" /></td>
                        <td style="width: 10px" valign="top" height="2px">
                            <img src="images/box_rightvertical_10px.jpg" width="10px" height="2px" /></td>
                    </tr>
                </table>
            </div>--%>
        <table cellpadding="0" cellspacing="0" border="0" width="100%" style="background-color: white">
            <tr style="height: 100%">
                <td>
                    <div style="padding-left: 10px; padding-right: 10px">
                        <ComponentArt:Splitter runat="server" ID="Splitter1" FillHeight="true" ImagesBaseUrl="images/"
                            Width="100%">
                            <ClientEvents>
                                <ResizeEnd EventHandler="OnTreePanelResized" />
                            </ClientEvents>
                            <Layouts>
                                <ComponentArt:SplitterLayout>
                                    <Panes Orientation="Horizontal" LiveResize="false" SplitterBarCollapseImageUrl="menuHandel_1.jpg"
                                        SplitterBarCollapseHoverImageUrl="splitter_horColHover.gif" SplitterBarExpandImageUrl="menuHandel_0.jpg"
                                        SplitterBarExpandHoverImageUrl="splitter_horExpHover.gif" SplitterBarCollapseImageWidth="5"
                                        SplitterBarCollapseImageHeight="88" SplitterBarCssClass="HorizontalSplitterBar"
                                        SplitterBarCollapsedCssClass="CollapsedHorizontalSplitterBar" SplitterBarActiveCssClass="ActiveSplitterBar"
                                        SplitterBarWidth="10" Width="100%">
                                        <ComponentArt:SplitterPane PaneContentId="ChartingContent" CssClass="SplitterPane"
                                            Width="25%" Height="100%" Collapsed="false" />
                                        <ComponentArt:SplitterPane PaneContentId="TelligentContent" CssClass="SplitterPane" Width="100%"
                                            AllowScrolling="false"  />
                                    </Panes>
                                </ComponentArt:SplitterLayout>
                            </Layouts>
                            <Content>
                                <ComponentArt:SplitterPaneContent ID="ChartingContent" Height="100%">
                                    <div id="leftContent">
                                        <table border="0" cellpadding="0" cellspacing="0" width="100%">
                                            <tr>
                                                <td class="PageHeading" align="left" colspan="2" valign="middle">
                                                    <img alt="Search" src="images/icon_organization.png" style="border-width: 0px" />
                                                    <span style="color: #000000">Organizations List</span>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="TreeSearch" valign="top">
                                                    <table cellpadding="5" cellspacing="0" border="0" width="100%">
                                                        <tr>
                                                            <td>
                                                                <input id="textbox_search" name="textbox_search" type="text" class="textboxNormal"
                                                                    accesskey="s" onkeyup="SearchTree(event,this)" />
                                                            </td>
                                                            <td style="width: 100%" align="left">
                                                                <img alt="Search" src="images/findText.gif" style="border-width: 0px" 
                                                                onclick ="imageClicked(this);"/>
                                                            </td>
                                                        </tr>
                                                        <tr style="display: none">
                                                            <td colspan="2">
                                                                <img id="showEmptyRegion_button" onclick="set_showEmptyRegion()" alt="Show empty regions"
                                                                    src="images/Home.jpg" style="cursor: hand" /><img id="autoUpdateContent_button" alt="update contents when selection is changed"
                                                                        onclick="set_autoUpdateContent()" src="images/Home.jpg" style="cursor: hand" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <ComponentArt:CallBack ID="Callback_RegionAdd" runat="server" OnCallback="Callback_RegionAdd_Callback">
                                                                    <LoadingPanelClientTemplate>
                                                                        <span style="color: Red">New region is being added...</span><img alt="" src="images/spinner.gif" />
                                                                    </LoadingPanelClientTemplate>
                                                                    <Content>
                                                                        <img src="images/add-a-globe.jpg" width="20px" height="20px" />
                                                                        <a href="javascript: void(0)" onclick="Dialog_AddRegion.Show();document.getElementById('NewRegionName').focus();"
                                                                            id="AddRegion"><span style="color: #000000; text-decoration: underline;">Add a region</span></a></Content>
                                                                </ComponentArt:CallBack>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                    <%--<div style="background-color: #FFFFFF; height: 100%">--%>
                                    <ComponentArt:TreeView ID="ATMTreeView" runat="server" CollapseTransition="None"
                                        ContentLoadingImageUrl="spinner.gif" CssClass="TreeViewMargin" CutNodeCssClass="CutTreeNode"
                                        DefaultImageHeight="16" DefaultImageWidth="16" DefaultMarginImageHeight="20"
                                        DefaultMarginImageWidth="24" DisplayMargin="false" DragAndDropEnabled="true"
                                        EnableViewState="false" ExpandTransition="none" HoverNodeCssClass="HoverTreeNode"
                                        ImagesBaseUrl="images/" KeyboardCutCopyPasteEnabled="true" KeyboardEnabled="false"
                                        LineImageHeight="20" LineImagesFolderUrl="images/lines/" LineImageWidth="19"
                                        LoadingFeedbackText="Loading ATMs" MarginWidth="24" MultipleSelectedNodeCssClass="MultipleSelectedTreeNode"
                                        MultipleSelectEnabled="true" NodeCssClass="TreeNode" NodeEditCssClass="NodeEdit"
                                        NodeEditingEnabled="true" NodeLabelPadding="3" SelectedNodeCssClass="SelectedTreeNode"
                                        ShowLines="true" Height="82.5%" AllowTextSelection="true" AutoScroll="true" CollapseNodeOnSelect="false"
                                        ExpandNodeOnSelect="false" Width="100%" BackColor="White">
                                        <ClientEvents>
                                            <ContextMenu EventHandler="onTreeContextMenu" />
                                            <NodeKeyboardNavigate EventHandler="NodeSelected" />
                                            <NodeSelect EventHandler="NodeSelected" />
                                            <NodeMove EventHandler="NodeMoved" />
                                            <NodeRename EventHandler="NodeRenamed" />
                                            <NodeBeforeMove EventHandler="BeforeMove" />
                                        </ClientEvents>
                                    </ComponentArt:TreeView>
                                    <!--<img src="images/pagebottomgradient.jpg" height="37px" width="100%" />
                                            -->
                                    <%--</div>--%>
                                </ComponentArt:SplitterPaneContent>
                                <ComponentArt:SplitterPaneContent ID="TelligentContent" >
                                    <iframe id="contentFrame" frameborder="0" height="100%" width="100%" name="contentForm"
                                    >
                                    </iframe>
                                    <!--                                            <img src="images/pagebottomgradient.jpg" alt="" height="35px" width="100%" />
    -->
    <script type="text/javascript">


        function resizeIframe() {

            var height = document.documentElement.clientHeight;
            if (height < 600) {

                document.getElementById('ATMTreeView').style.height = '83%';
            }
            else {
                document.getElementById('ATMTreeView').style.height = "88%"

            }
            gHeight = height;
            height -= document.getElementById('contentFrame').offsetTop;

            height -= 140;

            document.getElementById('contentFrame').style.height = height + "px";
            // SECURITY FIX: Updated frame access to be more secure and handle CSP restrictions
            if (window.frames.length > 0) {
                try {
                    // Check if the frame is accessible (same-origin)
                    if (window.frames[0].location.href && window.frames[0].resizeIframe) {
                        window.frames[0].resizeIframe();
                    }
                } catch (e) {
                    // Frame access blocked by CSP or cross-origin - this is expected behavior
                    console.log('Frame access restricted for security');
                }
            }
        };
        window.onresize = resizeIframe;
    </script>


                                </ComponentArt:SplitterPaneContent>
                            </Content>
                        </ComponentArt:Splitter>
                    </div>
                </td>
                <%--<td valign="top" align="right" style="background-image: url(images/box_rightvertical.jpg)"
                        width="3px">
                        &nbsp;</td>--%>
            </tr>
            <%-- <tr style="height: 2px">
                    <td style="width: 3px; background-image: url(images/box_leftbottom.jpg)">
                    </td>
                    <td style="width: 100%; background-image: url(images/menulinebottom.jpg)">
                    </td>
                    <td style="width: 3px; background-image: url(images/box_rightbottom.jpg)" />
                </tr>--%>
        </table>
    </div>
    <ComponentArt:Dialog ContentCssClass="contentCss" FooterCssClass="footerCss" HeaderCssClass="headerCss"
        CssClass="dialogCss" Icon="" Value="" Title="AJAX TabStrip Content" CloseTransition="None"
        ModalMaskCssClass="modalMaskCssClass" AllowDrag="false" Alignment="MiddleCentre"
        Modal="true" ID="Dialog_AddRegion" runat="server" Width="310" ContentClientTemplateId="data"
        AnimationType="None" ShowTransition="None"
        TransitionDuration="0" AnimationDuration="0">
        <ClientTemplates>
            <ComponentArt:ClientTemplate ID="data">
                <table cellpadding="0" style="border-color: #c9c9c9" cellspacing="0" border="0" width="100%">
                    <tr>
                        <td colspan="2" style="background-image: url(images/DialogImages/top.gif); height: 45px;">
                            <table width="100%" cellpadding="0" cellspacing="0" border="0">
                                <tr>
                                    <td valign="middle" style="padding-left: 4px; color: white; font-size: 15px; font-family: Verdana;
                                        font-weight: bold;">
                                        Add a Region
                                    </td>
                                    <td align="right" style="padding-right: 10px;">
                                        <img src="images/DialogImages/close.gif" onclick="Dialog_AddRegion.Close();" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr style="background-color: White;">
                        <td valign="top" style="padding: 10px; width: 30px;">
                            <img alt="" src="images/DialogImages/pencil.gif" style="padding: 5px;" />
                        </td>
                        <td style="padding: 5px;" align="left" valign="middle">
                            <span>New region name </span>
                            <input type="text" id="NewRegionName" name="NewRegionName" onkeyup="if (event.keyCode ==13 && NewRegionProvided(document.getElementById('NewRegionName').value)) Dialog_AddRegion.close();" />
                        </td>
                    </tr>
                    <tr style="background-color: White;">
                        <td colspan="2" align="right">
                            <input type="button" onclick="if (NewRegionProvided(document.getElementById('NewRegionName').value)) Dialog_AddRegion.close();"
                                value=" Add " class="button" />
                            <input type="button" value="Cancel" onclick="Dialog_AddRegion.close()" class="button" />
                        </td>
                    </tr>
                </table>
            </ComponentArt:ClientTemplate>
        </ClientTemplates>
    </ComponentArt:Dialog>
    <%--        <div id="footer" style="margin:0px">
            <table border="0" style="background-color: #c6dce8; width: 100%; margin: 0; padding-left: 0">
                <tr>
                    <td>
                        &nbsp;
                    </td>
                </tr>
            </table>
        </div>
    --%>
    <div>
        <%--<ComponentArt:Dialog ContentCssClass="contentCss" FooterCssClass="footerCss" HeaderCssClass="headerCss"
                        CssClass="dialogCss" Icon="" Value="" Title="AJAX TabStrip Content" CloseTransition="Fade"
                        ModalMaskCssClass="modalMaskCssClass" AllowDrag="false" Alignment="MiddleRight"
                                  Modal="true"                ID="Dialog_AddRegion" runat="server" Height="400" Width="510" ContentClientTemplateId="data"
                        AnimationType="Live" AnimationPath="Direct" AnimationSlide="None" ShowTransition="Dissolve"
                        TransitionDuration="700" AnimationDuration="100" >
                        <ClientTemplates>
                            <ComponentArt:ClientTemplate ID="data" >
                                <table cellpadding="0" style="border-color:#c9c9c9" cellspacing="0" border="0" width="100%" >
                                    <tr>
                                        
                                        <td  colspan="2" style="background-image: url(images/DialogImages/top.gif); height:45px;"; >
                                            <table width="100%" cellpadding="0" cellspacing="0" border="0">
                                                <tr>
                                                    <td valign="middle" style="padding-left:4px; color: white; font-size: 15px; font-family:Verdana; font-weight: bold;">
                                                        Add a Region</td>
                                                    <td align="right" style="padding-right:10px;">
                                                        <img src="images/DialogImages/close.gif" onclick="Dialog_AddRegion.Close();" /></td>
                                                </tr>
                                            </table>
                                        </td>
                                        
                                    </tr>
                                        <tr style="background-color:White;">
                        <td valign="top" style="padding: 10px; width: 30px;">
                            <img alt="" src="images/DialogImages/pencil.gif" style="padding: 5px;" /></td>
                        <td style="padding: 5px;" align="left" valign="middle">
                            <span>New region name </span>
                            <input type="text" id="NewRegionName"  name="NewRegionName" onkeyup="if (event.keyCode ==13 && NewRegionProvided(document.getElementById('NewRegionName').value)) Dialog_AddRegion.close();" />
                        </td>
                    </tr>
                    <tr style="background-color:White;">
                        <td colspan="2" align="right">
                            <input type="button" onclick="if (NewRegionProvided(document.getElementById('NewRegionName').value)) Dialog_AddRegion.close();"
                                value=" Add " class="button" />
                            <input type="button" value="Cancel" onclick="Dialog_AddRegion.close()" class="button" />
                        </td>
                    </tr>
                                  
                                </table>
                            </ComponentArt:ClientTemplate>
                        </ClientTemplates>
                    </ComponentArt:Dialog>--%>
    </div>
    <%--<ComponentArt:Dialog ID="Dialog_AddRegion" runat="server" Modal="true" TransitionDuration="500" HeaderCssClass="test"
            ShowTransition="Dissolve" CloseTransition="Dissolve" Width="300px" Height="200px" >
            <Header>
                <table cellpadding="0" cellspacing="0" border="5">
                    <tr style="height: 25px; color:White" >
                        <td style="width: 90%; ">
                            <span style="color: White; font-size: 15px; font-family: Arial; font-weight: bold;">
                                Add New Region</span>
                        </td>
                        <td>
                            <img alt="" src="images/dialogimages/close.png" style="margin-top: 4px; cursor: default;
                                padding-top: 4px;" width="32" height="25" onmousedown="this.src='images/dialogimages/close-down.png';"
                                onmouseup="this.src='images/dialogimages/close-hover.png';" onclick="Dialog_AddRegion.Close('Close click');"
                                onmouseover="this.src='images/dialogimages/close-hover.png';" onmouseout="this.src='images/dialogimages/close.png';" />
                        </td>
                    </tr>
                </table>
            </Header>
            <Content>
                <table width="100%" cellpadding="0" cellspacing="0" border="7">
                    <tr>
                        <td valign="top" style="padding: 10px; width: 30px;">
                            <img alt="" src="images/DialogImages/pencil.gif" style="padding: 5px;" /></td>
                        <td style="padding: 5px;" align="left" valign="middle">
                            <span>New region name </span>
                            <input type="text" id="NewRegionName" name="NewRegionName" onkeyup="if (event.keyCode ==13 && NewRegionProvided(document.getElementById('NewRegionName').value)) Dialog_AddRegion.close();" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" align="right">
                            <input type="button" onclick="if (NewRegionProvided(document.getElementById('NewRegionName').value)) Dialog_AddRegion.close();"
                                value=" Add " class="button" />
                            <input type="button" value="Cancel" onclick="Dialog_AddRegion.close()" class="button" />
                        </td>
                    </tr>
                </table>
            </Content>
        </ComponentArt:Dialog>--%>
    <%--<div style="background-color:Blue">
            <ComponentArt:Menu ID="ContextMenuATM" Orientation="Vertical" DefaultGroupCssClass="MenuGroup"
                SiteMapXmlFile="content/ContextMenuATM.xml" DefaultItemLookId="DefaultItemLook2"
                DefaultGroupItemSpacing="1" ImagesBaseUrl="images/menu_images/" EnableViewState="false"
                ContextMenu="Custom" runat="server">
                <ClientEvents>
                    <ItemSelect EventHandler="Menu_onItemSelect" />
                </ClientEvents>
                <ItemLooks>
                    <ComponentArt:ItemLook LookId="DefaultItemLook2" CssClass="MenuItem" HoverCssClass="MenuItemHover"
                        ExpandedCssClass="MenuItemHover" LeftIconWidth="20" LeftIconHeight="18" LabelPaddingLeft="10"
                        LabelPaddingRight="10" LabelPaddingTop="3" LabelPaddingBottom="4" />
                    <ComponentArt:ItemLook LookId="BreakItem" CssClass="MenuBreak" />
                </ItemLooks>
            </ComponentArt:Menu>
            <ComponentArt:Menu ID="ContextMenuRegion" Orientation="Vertical" DefaultGroupCssClass="MenuGroup"
                SiteMapXmlFile="content/ContextMenuRegion.xml" DefaultItemLookId="DefaultItemLook2"
                DefaultGroupItemSpacing="1" ImagesBaseUrl="images/menu_images/" EnableViewState="false"
                ContextMenu="Custom" runat="server">
                <ClientEvents>
                    <ItemSelect EventHandler="Menu_onItemSelect" />
                </ClientEvents>
                <ItemLooks>
                    <ComponentArt:ItemLook LookId="DefaultItemLook2" CssClass="MenuItem" HoverCssClass="MenuItemHover"
                        ExpandedCssClass="MenuItemHover" LeftIconWidth="20" LeftIconHeight="18" LabelPaddingLeft="10"
                        LabelPaddingRight="10" LabelPaddingTop="3" LabelPaddingBottom="4" />
                    <ComponentArt:ItemLook LookId="BreakItem" CssClass="MenuBreak" />
                </ItemLooks>
            </ComponentArt:Menu>
    --%>
    <div id="searchDiv" style="
        opacity:.95;
        filter:alpha(opacity=95);
        display:none;
        background-color:InfoBackground;
        height:200px;
        width:150px;
        border:1px solid orange;
        z-index:9999;
        overflow:auto;
        position:absolute;
        cursor:pointer;" />
    <asp:Literal ID="literal_jsArray" runat="server" EnableViewState="false" />
    <ComponentArt:Menu ID="ContextMenuRegion" Orientation="Horizontal" DefaultGroupCssClass="MenuGroup"
        SiteMapXmlFile="content/ContextMenuRegion.xml" DefaultItemLookId="DefaultItemLook2"
        DefaultGroupItemSpacing="1" ImagesBaseUrl="images/menu_images/" EnableViewState="false" 
        ContextMenu="Custom"  runat="server">
        <ClientEvents>
            <ItemSelect EventHandler="Menu_onItemSelect" />
        </ClientEvents>
        <ItemLooks>
            <ComponentArt:ItemLook LookId="DefaultItemLook2" CssClass="MenuItem" HoverCssClass="MenuItemHover"
                ExpandedCssClass="MenuItemHover" LeftIconWidth="20" LeftIconHeight="18" LabelPaddingLeft="10"
                LabelPaddingRight="10" LabelPaddingTop="3" LabelPaddingBottom="4" />
            <ComponentArt:ItemLook LookId="BreakItem" CssClass="MenuBreak" />
        </ItemLooks>
    </ComponentArt:Menu>
    </div>
    <ComponentArt:CallBack runat="server" ID="CallBack_TreePanelWidthChange" OnCallback="PanelWidthChanged"
        Height="0" Width="0" />
    <ComponentArt:CallBack runat="server" ID="Callback_Move" OnCallback="MoveNode" Height="0"
        Width="0" />
    <ComponentArt:CallBack ID="Callback_Rename" runat="server" Height="0" OnCallback="Callback_Rename_Callback"
        Width="0" />
    <ComponentArt:CallBack ID="Callback_nodeSelected" runat="server" Height="0" OnCallback="Callback_nodeSelected_Callback"
        Width="0" />
    <ComponentArt:CallBack ID="Callback_multiNodeSelection" runat="server" Height="0"
        OnCallback="Callback_multiNodeSelection_Callback" Width="0" />
    <ComponentArt:CallBack ID="Callback_deleteNode" runat="server" Height="0" OnCallback="Callback_deleteNode_Callback"
        Width="0" />
    <ComponentArt:CallBack ID="Callback_SessionEnd" runat="server" Height="0" OnCallback="Callback_SessionEnd_Callback"
        Width="0" />
    <div id="divWait" runat="server" style="position: absolute; top: 200px; right: 45.5em;">
        <asp:Image ID="Image_loading" runat="server" ImageUrl="images/busy.gif" /><b>Loading...</b>
    </div>
    </form>
</body>
</html>
