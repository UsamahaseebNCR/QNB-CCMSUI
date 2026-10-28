<%@ Page Language="C#" AutoEventWireup="true" Codebehind="ScheduledDownload.aspx.cs"
    Inherits="CCMSUI.CCMS.ScheduledDownload" %>

<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">

    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>

    <title>Schedule EJ Downloads</title>

    <script type="text/javascript">
        window.parent.window.SetContentPage( window.location);
        
        
//		function setCheck(val)
//		{
//		    var inputs = document.getElementsByTagName('input');
//		    for (var i = 0; i < inputs.length; i++)
//            {
//            var typ = inputs[i].type;
//            if (typ=="checkbox")
//                inputs[i].checked = val;    
//            }
//		}
        
        function LabelText(labeltext)
        {
            document.getElementById('Label_errorMsg').innerHTML = "<li>" + labeltext + "</li>";
        }        
    
    </script>

    <link href="../content/iSuite.css" rel="stylesheet" />
</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)' onload="hideProgressBar();">
    <form id="form1" runat="server">
        <div>
                    <div id="notificationArea" style="text-align: center">
        </div>

            <asp:Literal ID="Literal_msg" runat="server" EnableViewState="false">
    
            </asp:Literal>
            <table style="width: 100%" cellspacing="0" cellpadding="0" border="0" class="mainbox">
                <tr>
                    <td class="PageHeading" align="left">
                        Schedule ATM Counter Downloads</td>
                </tr>
                <tr>
                    <td class="ValidationText" align="center">
                        <asp:Literal ID="Literal_error" runat="server" EnableViewState="False"></asp:Literal>
                        <div class="ValidationText">
                            <asp:Label ID="Label_errorMsg" runat="server" EnableViewState="False"></asp:Label></div>
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
                        <b>Search Filters</b></td>
                </tr>
                <tr align="left">
                    <td colspan="2">
                        <table width="100%" class="SEARCHBOX" border="0">
                            <tr>
                                <td>
                                    <table id="search" width="90%" border="0">
                                        <tr>
                                            <td style="padding-left: 20px; width: 40%">
                                                File Type:
                                                <asp:DropDownList ID="Dropdownlist_FileType" runat="server" CssClass="textbox" Width="150px">
                                                </asp:DropDownList></td>
                                            <td>
                                                Show:
                                                <asp:RadioButton ID="RadioAll" runat="server" Text="All" GroupName="ScheduledType"
                                                    Checked="true" />&nbsp;&nbsp;&nbsp;<asp:RadioButton ID="RadioScheduled" runat="server"
                                                        Text="Scheduled" GroupName="ScheduledType" />&nbsp;&nbsp;&nbsp;<asp:RadioButton ID="RadioNotScheduled"
                                                            runat="server" Text="Not Scheduled" GroupName="ScheduledType" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2" style="padding-right: 150px" align="right">
                                                <asp:Button ID="btnSearch" Text="Search" CssClass="button" runat="server" OnClick="ButtonSearch_Click"
                                                    Width="100px" />
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
                        Schedule ATM Counter Downloads
                    </th>
                </tr>
                <tr>
                    <td align="center" colspan="2">
                        <div style="padding-right: 150px; text-align: right">
                            <input type="button" id="button_checkAll" value="Check All" class="button" onclick="setCheck(true)"
                                style="width: 70px" />
                            &nbsp;<input type="button" id="button_uncheckAll" value="Uncheck All" class="button"
                                onclick="setCheck(false)" style="width: 90px" />
                            &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;
                            &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&nbsp; &nbsp;<asp:Button ID="Schedule" runat="server"
                                Text="Save Changes" CssClass="button" OnClick="Schedule_Click" Width="100px" />
                        </div>
                        <asp:GridView ID="DG_ScheduledJobs" runat="server" AutoGenerateColumns="False" AllowSorting="true"
                            Width="90%" OnRowDataBound="DG_ScheduledJobs_RowDataBound" OnSorting="DG_ScheduledJobs_Sorting"
                            DataKeyNames="atm_id,file_type_id,Scheduled_files" RowStyle-HorizontalAlign="center"
                            OnPageIndexChanging="DG_ScheduledJobs_PageIndexChanging">
                            <AlternatingRowStyle BackColor="#eff0ef" />
                            <HeaderStyle CssClass="HeadingCell" HorizontalAlign="Left" ForeColor="black" />
                            <RowStyle HorizontalAlign="Center" />
                            <Columns>
                                <asp:BoundField DataField="title" HeaderText="ATM Name" HeaderStyle-CssClass="HeadingCell" ItemStyle-HorizontalAlign="Center"
                                    SortExpression="title" />
                                <asp:BoundField DataField="atm_type" HeaderText="ATM Type" HeaderStyle-CssClass="HeadingCell" ItemStyle-HorizontalAlign="Center"
                                    SortExpression="atm_type" />
                                <asp:BoundField DataField="file_type_title" HeaderText="File Type" SortExpression="file_type_title" ItemStyle-HorizontalAlign="Center"
                                    HeaderStyle-CssClass="HeadingCell" />
                                <asp:TemplateField HeaderText="Scheduled" HeaderStyle-CssClass="HeadingCell" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chkbx" runat="server"></asp:CheckBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <RowStyle HorizontalAlign="Left" />
                        </asp:GridView>
                        
                        <div style="padding-right: 150px; text-align: right">
                            <input type="button" id="button1" value="Check All" class="button" onclick="setCheck(true)"
                                style="width: 70px" />
                            <input type="button" id="button2" value="Uncheck All" class="button" onclick="setCheck(false)"
                                style="width: 90px" />
                            &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;
                            &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;
                            <asp:Button ID="Button_schedule2" runat="server" Text="Save Changes" CssClass="button"
                                OnClick="Schedule_Click" Width="100px" />
                        </div>
                    </td>
                </tr>
            </table>
        </div>
        <input type="button" runat="server" id="refresh" style="display: none" />
    </form>
</body>
</html>
