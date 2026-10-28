<%@ Page Language="C#" AutoEventWireup="true" Codebehind="AddActiveDirectoryUsers.aspx.cs"
    Inherits="CCMSUI.AddActiveDirectoryUsers" %>


<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">

    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>

    <title>Schedule EJ Downloads</title>

    <script type="text/javascript">
        window.parent.window.SetContentPage( window.location);
        function LabelText(labeltext)
        {
            document.getElementById('Label_errorMsg').innerHTML = "<li>" + labeltext + "</li>";
        }        
    function checkAnySelected()
    {
        var inputs = document.getElementsByTagName('input');
	    for (var i = 0; i < inputs.length; i++)
        {
           var typ = inputs[i].type;
           if (typ=="checkbox")
              if (inputs[i].checked)
                  return true;
        }
        alert('Please select users from the list');
        return false;
    }
 
    function refresh()
    {
        document.body.removeChild(document.getElementById('loadingScreen'));
    }
    </script>

    <link href="../content/iSuite.css" rel="stylesheet" />
</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)' onload="refresh(); setPageURL('admin/AddActiveDirectoryUsers.aspx');">
    <form id="form1" runat="server">
        <div id="notificationArea" style="text-align: center">
        </div>
        <asp:Literal ID="Literal_msg" runat="server" EnableViewState="false">
    
        </asp:Literal>
        <table id="table2" cellspacing="0" cellpadding="0" width="100%" border="0">
            <tr>
                <td class="PageHeading" align="left" colspan="2">
                    <%=(Request.QueryString["csv"] != null) ? "Import Users From CSV": "Import Active Directory Users"%> 
                    
                    </td>
            </tr>
            <tr>
                <td align="center" colspan="2" style="height: 20px">
                    <div class="ValidationText">
                        <asp:Label ID="Label_errorMsg" runat="server" EnableViewState="False"></asp:Label></div>
                    <div class="ConfirmationText">
                        <asp:Literal ID="Literal_ok" runat="server" EnableViewState="False"></asp:Literal></div>
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
                                <table id="search" width="90%" border="0">
                                    <tr>
                                        <td>
                                            User ID :
                                        </td>
                                        <td>
                                            <asp:TextBox ID="TextBox_userId" runat="server" CssClass="textboxNormal"></asp:TextBox>
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
                                            <asp:Button ID="Button_search" runat="server" CssClass="button" 
                                                Text="Search" Width="71px" OnClick="Button_search_Click" />
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

        </table>
        <table cellspacing="0" cellpadding="0" width="100%" border="0">
            <tr>
                <th align="left" colspan="2" style="height: 20px">
                    Import Users</th>
            </tr>
            <tr>
                <td align="center">
                    <div style="padding-right: 150px; text-align: right" runat="server" id="DivChoices1">
                        <input type="button" id="button_checkAll" value="Check All" class="button" onclick="setCheck(true)"
                            style="width: 70px" />
                        &nbsp;<input type="button" id="button_uncheckAll" value="Uncheck All" class="button"
                            onclick="setCheck(false)" style="width: 90px" />
                        &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;
                        &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&nbsp; &nbsp;<asp:Button ID="Import" runat="server"
                            Text="Assign Group and ATMs" CssClass="Button" OnClick="Import_Click" Width="151px" OnClientClick="return checkAnySelected();"/>
                    </div>
                    
                    <asp:GridView ID="DG_ADUsers" runat="server" AutoGenerateColumns="False" AllowSorting="True"
                        Width="90%" OnSorting="DG_ADUsers_Sorting" DataKeyNames="user_name,user_email,user_id" 
                        OnRowDataBound="DG_ADUsers_RowDataBound">
                        <HeaderStyle CssClass="HeadingCell" HorizontalAlign="Left" ForeColor="White" />
                        <Columns>
                            <asp:BoundField DataField="user_name" HeaderText="Name"
                                SortExpression="user_name" >
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="user_email" HeaderText="Email"
                                SortExpression="user_email" >
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:BoundField DataField="user_id" HeaderText="Login ID"
                                SortExpression="user_id" >
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:BoundField>
                            <asp:TemplateField HeaderText="Import">
                                <ItemTemplate>
                                    <asp:CheckBox ID="chkbx" runat="server"></asp:CheckBox>
                                </ItemTemplate>
                                <HeaderStyle CssClass="HeadingCell" />
                            </asp:TemplateField>
                        </Columns>
                        <RowStyle HorizontalAlign="Left" />
                    </asp:GridView>
                    <div style="padding-right: 150px; text-align: right" runat="server" id="DivChoices2">
                        <input type="button" id="button3" value="Check All" class="button" onclick="setCheck(true)"
                            style="width: 70px" />
                        &nbsp;<input type="button" id="button4" value="Uncheck All" class="button"
                            onclick="setCheck(false)" style="width: 90px" />
                        &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;
                        &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&nbsp; &nbsp;<asp:Button ID="Button5" runat="server"
                            Text="Assign Group and ATMs" CssClass="Button" OnClick="Import_Click" Width="151px" OnClientClick="return checkAnySelected();"/>
                    </div>
                    
                </td>
            </tr>
        </table>
                       
        
        
    </form>
</body>
</html>
