<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DirectoryLookupAndFileDownloader.aspx.cs"
    Inherits="CCMSUI.admin.DirectoryLookupAndFileDownloader" EnableEventValidation="false" %>

<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Directory Lookup and File Downloader</title>
    <script type="text/javascript" src="../content/js/jquery.js"></script>
    <script type="text/javascript" src="../content/Scripts/jquery-ui-1.8.16.min.js"></script>
    <script  type="text/javascript" src="../content/Scripts/jquery.tablesorter.min.js"></script>
    <script type="text/javascript">
        function onTreeContextMenu(sender, eventArgs) {
            ContextMenuFolders.showContextMenu(eventArgs.get_event(), eventArgs.get_node());
        }
        
        $(function () {
            $("[id*=GridView_LookupFile] td").bind("click", function () {
                var row = $(this).parent();
                $("[id*=GridView_LookupFile] tr").each(function () {
                    if ($(this)[0] != row[0]) {
                        $("td", this).removeClass("selected_row");
                    }
                });
                $("td", row).each(function () {
                    if (!$(this).hasClass("selected_row")) {
                        $(this).addClass("selected_row");
                    } 
                });
            });
            
            $("#GridView_LookupFile").tablesorter();
            //$("th").click(function () {
            //    var columnIndex = $(this).index();
            //    var tdArray = $(this).closest("table").find("tr td:nth-child(" + (columnIndex + 1) + ")");
            //    tdArray.sort(function (p, n) {
            //        var pData = $(p).text();
            //        var nData = $(n).text();
            //        return pData < nData ? -1 : 1;
            //    });
            //    tdArray.each(function () {
            //        var row = $(this).parent();
            //        $("#GridView_LookupFile").append(row);
            //    });
            //});
            
            $("#myMenu").hide();
            $("table[id$='GridView_LookupFile'] > tbody > tr").bind('contextmenu', function (e) {
                $("#myMenu").hide();
                e.preventDefault();
                filename = $(this).children(':first-child').next().text();
                filetype = $(this).children(':first-child').next().next().next().text();

                if (filename != null) {
                    //call context menu here
                    $("#myMenu").css({
                        top: e.pageY + "px",
                        left: e.pageX + "px",
                        position: 'absolute'
                    });
                    $("#myMenu").show();
                }
            });
            $(document).bind('click', function (e) {
                $("#myMenu").hide();
            });

        });

        function timedRefresh(timeoutPeriod) {
            setTimeout("location.reload(true);", timeoutPeriod);
        }

        function Menu_onItemSelect(sender, eventArgs) {

            var menuItem = eventArgs.get_item();
            var contextDataNode = menuItem.get_parentMenu().get_contextData();

            //var feedback = '"' + menuItem.get_id() + '" command was issued on the "' + contextDataNode.get_text() + '" node from "' + contextDataNode.get_id() + '".';
            //alert(feedback);
            switch (menuItem.get_id()) {
<%--                case 'download':
                    TreeView_Dir.selectNodeById(contextDataNode.get_id());
                    document.getElementById('<%=Button_Download.ClientID%>').click();
                    break;

                case 'getFiles':
                    TreeView_Dir.selectNodeById(contextDataNode.get_id());
                    document.getElementById('<%=Button_GetFileSize.ClientID%>').click();
                    break;--%>
            }

            return true;
        }


        function fnDownload()
        {
            //debugger
            var lnkView = document.getElementById('lnkView');
            var hiddenField = document.getElementById('hdnFileName');
            var hiddenField1 = document.getElementById('hdnFileType');
            hiddenField.value = filename;
            hiddenField1.value = filetype;
            lnkView.click();
        }

        function setCheck(node, selected) {
            var childary = node.get_nodes();

            for (var i = 0; i < childary.get_length(); i++) {
                if (childary.getNode(i).get_id().substring(0, 1) == 'r') {
                    childary.getNode(i).set_checked(selected);
                    setCheck(childary.getNode(i), selected)
                }
                else
                    childary.getNode(i).set_checked(selected);
            }
        }

        window.parent.window.contentPage = '';

        function check(sender, eventArgs) {
            TreeView_Dir.beginUpdate();
            setCheck(eventArgs.get_node(), eventArgs.get_node().get_checked());
            TreeView_Dir.endUpdate();
        }
    </script>
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
    <script type="text/javascript">
        window.parent.window.contentPage = '';          
        
        
    </script>
    <link href="../content/iSuite.css" rel="stylesheet" />
    <link href="../content/treeStyle.css" rel="stylesheet" />
    <link href="../content/menuStyle.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .style1
        {
            width: 140px;
        }
        .style2
        {
            height: 27px;
        }
    </style>
</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)' onload="hideProgressBar();">
    
    <form id="form1" runat="server">
    <asp:Literal ID="literal_script" runat="server"></asp:Literal>
    <table cellpadding="0" cellspacing="0" border="0" class="MainBox" width="100%">
        <tr>
            <td class="PageHeading" align="left" colspan="2" style="height: 25px">
                Directory Lookup and File Downloader
            </td>
        </tr>
        <tr>
            <td class="SearchHeadPaneGradient" colspan="2">
            </td>
        </tr>
        <tr>
            <td class="ConfirmationText" align="center">
                <%=Session[SessionVars.notice.ToString()]%>
                <%Session[SessionVars.notice.ToString()] = "";%>
                <asp:Literal ID="Literal_ok" runat="server" EnableViewState="False"></asp:Literal>
            </td>
        </tr>
        <tr>
            <td colspan="2" align="center">
                <asp:ValidationSummary ID="ValidationSummary1" runat="server"></asp:ValidationSummary>
            </td>
        </tr>
        <tr style="height: 20px">
            <td colspan="2" style="color: red;" align="center" valign="top">
                <asp:Literal ID="Literal_error" runat="server" EnableViewState="False"></asp:Literal>
            </td>
        </tr>
        <tr>
            <td valign="top">
                <table id="inner" border="0" cellpadding="0" cellspacing="0" style="padding-left: 10px;width: 80%;padding-right: 10px;">
                    <tr style="padding-left: 10px">
                        <td class="style2">
                            Selected ATM
                        </td>
                        <td>
                            <b>
                                <asp:Label ID="Label_SelectedATM" runat="server"></asp:Label></b>
                        </td>
                    </tr>
                    
                    </tr>
                    <tr>
                        <td class="style1">
                            Folder Path
                        </td>
                        <td style="width: 35%;">
                            <asp:TextBox ID="TextBox_FolderPath" runat="server" CssClass="textbox" Font-Size="12px"
                                Width="395px" Height="18px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton_GetFiles" runat="server" ImageUrl="~/images/search.jpg"
                                OnClick="Button_GetFiles_Click" Width="29px" Height="30px" OnClientClick="parent.document.getElementById('divWait').style.visibility = 'visible';" />
                        </td>
                        
                        <td>
                        </td>
                    </tr>
                    <tr>
                        <td valign="top" class="style1">
                            Files
                        </td>
                        <td valign="top" colspan="2">
                    <asp:GridView ID="GridView_LookupFile" runat="server" AutoGenerateColumns="False" AllowSorting="false"
                         Width="100%"
                        AllowPaging="false" 
                        OnRowDataBound="GridView_LookupFile_RowDataBound"
                        RowStyle-CssClass="GvGrid"
                        GridLines="None"
                        PageSize="15" OnSelectedIndexChanged="GridView_LookupFile_SelectedIndexChanged">
                        
                        <Columns>
                            
                            <asp:TemplateField>
                                <HeaderStyle CssClass="FileGridview" ForeColor="black" BackColor="White"  HorizontalAlign="Left"/>
                                <ItemTemplate>
                                    <asp:Image ID="file_type" runat="server" ImageUrl='<%#Eval("file_type")%>'/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="file_name" HeaderText="Name" SortExpression="file_name">
                                <HeaderStyle CssClass="FileGridview" ForeColor="#4c99d5" BackColor="White"  HorizontalAlign="Left"/>
                            </asp:BoundField>
                            <asp:BoundField DataField="file_date_modified" HeaderText="Date Modified" dataformatstring="{0:MM/dd/yyyy HH:mm:ss}" htmlencode="false" SortExpression="file_date_modified">
                                <HeaderStyle CssClass="FileGridview" ForeColor="#4c99d5" BackColor="White" HorizontalAlign="Left"/>
                            </asp:BoundField>
                             <asp:BoundField DataField="file_size" HeaderText="Size" SortExpression="file_size">
                                <HeaderStyle CssClass="FileGridview" ForeColor="#4c99d5" BackColor="White" HorizontalAlign="Left"/>
                            </asp:BoundField>
                            <asp:CommandField SelectText="Select" ShowSelectButton="true" Visible="false" />
                           
                            
                             
                        </Columns>
                        <EmptyDataTemplate>
                            No File found!</EmptyDataTemplate>
                        <HeaderStyle BackColor="Black" ForeColor="White" CssClass="HeadingCell" />
                    </asp:GridView>
                    &nbsp;
                </td>
                        
                        
                        
                        
                    </tr>
                    <tr>
                        <td colspan="3" style="font-weight: bold; padding-top: 30px;">
                            <input id="Button_Cancel" type="button" value="Return to Home Page" onclick="window.location ='../Home.aspx'"
                                class="button" />
                        </td>
                    </tr>
                </table>
        <asp:LinkButton ID="lnkView" runat="server"
    style="display:none" OnClick="lnkView_Click" />
<asp:HiddenField ID="hdnFileName" runat="server"  />
<asp:HiddenField ID="hdnFileType" runat="server" value="" />
    <br />
    <br />
        <div id="myMenu" class="contextMenu">
            <table style='width:100%;'>
            <tr><td onclick="fnDownload();">Download</td></tr>
            </table>
            </div>


    <input type="button" runat="server" id="refresh" style="display: none" />
    <ComponentArt:Menu ID="ContextMenuFolders" Orientation="Vertical" DefaultGroupCssClass="MenuGroup"
        SiteMapXmlFile="../content/ContextMenuFolder.xml" DefaultItemLookId="DefaultItemLook2"
        DefaultGroupItemSpacing="1" ImagesBaseUrl="../images/menu_images/" EnableViewState="false"
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
        
    </form>
</body>
</html>
