<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Package.aspx.cs" Inherits="CCMSUI.admin.Package" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="../content/ISUITE.css" rel="stylesheet" />
    <title>Package Management</title>
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
    <script src="../content/jquery/jquery.min.js"></script>
    <script type="text/javascript">
        

        //window.parent.window.SetContentPage(window.location);
        $(document).ready(function () {            
            HandleMissingPackage();            
        });

        function HandleMissingPackage() {
            $(".missing-pckg").hide();
            var packageCookies = getAllPackageCookies();
            if (packageCookies != null && packageCookies.length > 0) {
                for (var key in packageCookies) {
                    createPackageRow(packageCookies[key]);
                }
                $(".missing-pckg").show();
            }
        }

        function getCookie(key) {
            var keyValue = document.cookie.match('(^|;) ?' + key + '=([^;]*)(;|$)');
            return keyValue ? keyValue[2] : null;
        }

        function createPackageRow(pckgData) {

            const resumeCookieChunks = pckgData.uploadedChunks.split(',');
            chunkSize = parseInt(pckgData.chunkSize) / (1024 * 1024);
            var uploadedChunks = resumeCookieChunks.map(Number);
            const tableBody = $('#pckgMissingFile tbody');
            const newRow = $('<tr>');

            newRow.append($('<td>').text(pckgData.name));
            newRow.append($('<td>').text(pckgData.path));
            newRow.append($('<td>').text(pckgData.dateCreated));
            newRow.append($('<td>').text(pckgData.totalChunks));
            newRow.append($('<td>').text(Math.max(...uploadedChunks)));

            // Add buttons or functionality for Resume and Delete actions (replace with your implementation)

            const editUrl = "AddEditPackage.aspx?PckgName=" + pckgData.name + "&FileName=" + pckgData.path + "&Conf=" + pckgData.configuration + "&chunkSize=" + chunkSize + "&fileSize=" + pckgData.fileSize;

            const newCell = $('<td>'); // Create a table cell

            const editLink = $('<a>').attr('href', editUrl); // Create anchor tag
            editLink.append($('<img>') // Add image within the anchor tag
                .attr('src', '../images/upload.png')
                .attr('alt', 'Click to resume uploaded file')
                .addClass('ShowHand')
                .css('width', '18px')
                .css('height', '14px'));

            newCell.append(editLink); // Add the anchor tag to the cell
            newRow.append(newCell);
            newRow.append($('<td>').append('<input type="image" id="delete_' + pckgData.Key + '" onclick="DeletePackage(\'' + pckgData.Key + '\', \'' + pckgData.name + '\')" title="Click to delete package" class="ShowHand" src="../images/Delete.png" style="height: 14px; width: 18px; border - width: 0px; ">'));
            tableBody.append(newRow);
        }

        function DeletePackage(cookieKey, pckgName) {
            eraseCookie(cookieKey);
            HandleMissingPackage();
        }


        function getAllPackageCookies() {
            var allCookies = document.cookie.split(';');
            var packageCookies = [];

            for (var cookieString of allCookies) {
                var packageCookie = {};
                var [cookieKey, cookieValue] = cookieString.trim().split('=');
                if (cookieKey.startsWith('pckg_object_')) {
                    packageCookie = JSON.parse(cookieValue); // Parse JSON value
                    packageCookie.Key = cookieKey;
                    packageCookies.push(packageCookie);
                }
                
            }

            return packageCookies;
        }


        function eraseCookie(key) {
            document.cookie = key + '=; Max-Age=0';
        }

        function confirmCITDeletion(title) {
            return confirm('Are you sure you want to delete Package ' + title + '?', 'CCMS');
        }</script>

    <style>
        .hideDelete {display:none;}
    </style>

</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)'
    onload="hideProgressBar();">
    <form id="form1" runat="server">
    <div>
        <table style="width: 100%" cellspacing="0" cellpadding="0" border="0" class="mainbox">
            <tr>
                <td class="PageHeading" align="left">
                    Package Management
                </td>
            </tr>
            <tr>
                <td class="ValidationText" align="center">
                    <asp:Literal ID="Literal_error" runat="server" EnableViewState="False"></asp:Literal>
                </td>
            </tr>
            <tr>
                <td class="ConfirmationText" align="center">
                    <%=Session[SessionVars.notice.ToString()]%>
                    <%Session[SessionVars.notice.ToString()] = "";%>
                    <asp:Literal ID="Literal_ok" runat="server" EnableViewState="False"></asp:Literal>
                </td>
            </tr>
            <tr align="left" style="background-color: WHITE">
                <td style="height: 20px; padding-left: 5px;" colspan="2">
                    <b>Search Filters</b>
                </td>
            </tr>
            <tr align="left">
                <td colspan="2">
                    <table width="100%" class="SEARCHBOX" border="0">
                        <tr>
                            <td>
                                <table id="search" width="100%" border="0">
                                    <tr>
                                        <td>
                                            Title :
                                        </td>
                                        <td>
                                            <asp:TextBox ID="TextBox_name" runat="server" Width="148px" CssClass="textboxNormal"></asp:TextBox>
                                        </td>
                                        <td style="height: 20px">
                                            Created by :
                                        </td>
                                        <td style="height: 20px">
                                            <asp:DropDownList ID="DropDownList_creator" runat="server" Width="150px" CssClass="textbox">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                        </td>
                                        <td>
                                        </td>
                                        <td>
                                        </td>
                                        <td>
                                            <asp:Button ID="Button_search" runat="server" CssClass="button" OnClick="Button_search_Click"
                                                Text="Search" Width="71px" />
                                            &nbsp;<asp:Button ID="Button_reset" runat="server" CssClass="button" Text="Reset"
                                                Width="71px" OnClick="Button_reset_Click" />&nbsp;
                                            <asp:HyperLink ID="HyperLink_newPackage" runat="server" CssClass="Link" NavigateUrl="../admin/AddEditPackage.aspx" ForeColor="Red">Create New Package ... </asp:HyperLink>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td>
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
            <tr>
                <td align="left">
                    &nbsp; &nbsp; &nbsp;&nbsp;
                </td>
            </tr>
            <tr align="left">
                <th colspan="2" align="left">
                    Selected Packages -
                    <asp:Literal ID="Literal_count" runat="server" EnableViewState="False"></asp:Literal>
                </th>
            </tr>
            <tr>
                <td valign="top" colspan="2">
                    <asp:GridView ID="GridView_Packages" runat="server" AutoGenerateColumns="False" AllowSorting="true"
                        OnSorting="GridView_Packages_Sorting" Width="100%" OnRowCommand="GridView_Packages_RowCommand"
                        AllowPaging="True" OnPageIndexChanging="GridView_Packages_PageIndexChanging"
                        OnRowDataBound="GridView_Packages_RowDataBound" PageSize="30" HeaderStyle-CssClass="HeadingCell">
                        <RowStyle HorizontalAlign="Left" />
                        <AlternatingRowStyle BackColor="#eff0ef" />
                        <Columns>
                            <asp:BoundField DataField="package_title" HeaderText="Package Title" SortExpression="package_title"
                                HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            
                             <asp:BoundField DataField="package_physcial_path" HeaderText="Physical Path" SortExpression="package_physcial_path"
                                HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="package_creation_time" HeaderText="Creation Time" SortExpression="package_creation_time"
                                DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" HeaderStyle-ForeColor="black" ItemStyle-HorizontalAlign="Center">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                             <asp:BoundField DataField="user_full_name" HeaderText="Created By" SortExpression="user_full_name"
                                HeaderStyle-ForeColor="black">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="package_size" HeaderText="Package Size" SortExpression="package_size"
                               HeaderStyle-ForeColor="black" ItemStyle-HorizontalAlign="Right">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="uploaded_bytes" HeaderText="Uploaded Bytes" SortExpression="uploaded_bytes"
                               HeaderStyle-ForeColor="black" ItemStyle-HorizontalAlign="Right">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="progress" HeaderText="Status" SortExpression="progress"
                               HeaderStyle-ForeColor="black" ItemStyle-HorizontalAlign="Center">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>                            
                            
                            
                           <%-- <asp:BoundField DataField="package_modification_time" HeaderText="Modification Time"
                                SortExpression="modified_on" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" HeaderStyle-ForeColor="black" ItemStyle-HorizontalAlign="Center">
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>--%>
                            <asp:TemplateField HeaderText="View" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:HyperLink ID="ViewLink" runat="server" CssClass="package-link"
                                        NavigateUrl='<%# "AddEditPackage.aspx?id=" + Eval("package_id") %>'>
                                        <img alt="Click to edit package" src="../images/viewfile.jpg" style="border: 0" class="ShowHand"  Width="18px" Height="14px" />
                                    </asp:HyperLink>
                                    <%--<a id="ViewLink" href='AddEditPackage.aspx?id=<%# Eval("package_id") %>'>
                                        <img alt="Click to edit package" src="../images/viewfile.jpg" style="border: 0" class="ShowHand"  Width="18px" Height="14px" />
                                    </a>--%>
                                </ItemTemplate>
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Download" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:ImageButton runat="server" CommandName="downloadPackage" CommandArgument='<%# Eval("package_id") %>'
                                        ToolTip="Click to download package" ImageUrl="../images/download.png" ID="Button_downloadPackage"  Width="18px" Height="14px"
                                        CssClass="ShowHand" />
                                </ItemTemplate>
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Delete" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:ImageButton runat="server" CommandName="deletePackage" PckgId='<%# Eval("package_id") %>' CommandArgument='<%# Eval("package_id") %>'
                                        ToolTip="Click to delete package" ImageUrl="../images/Delete.png" ID="Button_deletePackage" Width="18px" Height="14px"
                                        CssClass="ShowHand" />
                                </ItemTemplate>
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                            No Record(s) found!</EmptyDataTemplate>
                        <HeaderStyle BackColor="Black" ForeColor="Black" CssClass="HeadingCell" />
                    </asp:GridView>
                    &nbsp;
                    <h2 class="missing-pckg">Packages with pending uploaded file</h2>
                    <table class="missing-pckg" id="pckgMissingFile" border="1" style="border-collapse: collapse; width: 100%;">
                    <thead>
                    <tr>
                        <th>Package</th>
                        <th>File</th>
                        <th>Creation Time</th>
                        <th>Total Chunks</th>
                        <th>Uploaded Chunks</th>
                        <th>Resume</th>
                        <th>Delete</th>
                    </tr>
                    </thead>
                    <tbody></tbody>
                </table>                                      
                </td>
            </tr>
        </table>
    </div>
    <input type="button" runat="server" id="refresh" style="display: none" />
    </form>
</body>
</html>
