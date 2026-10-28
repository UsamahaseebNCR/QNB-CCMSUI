<%@ Page Language="C#" AutoEventWireup="true" Codebehind="ConfigureDailyFeedStep2.aspx.cs"
    Inherits="CCMSUI.CCMS.ConfigureDailyFeedStep2" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Add Edit ATM</title>

    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>

    <script type="text/javascript">
        window.parent.window.contentPage = '';          
        
        
    </script>

    <style type="text/css">
        .TH {
        }
    </style>
    <link href="../content/iSuite.css" rel="stylesheet" />
</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)' onload="hideProgressBar();">
    <form id="form1" runat="server">
        <table cellpadding="0" cellspacing="0" border="0" class="MainBox" width="100%">
            <tr>
                <td class="PageHeading" align="left" colspan="2" style="height: 25px">
                    Configure Daily Feed Step 2
                </td>
            </tr>
            <tr>
                <td class="SearchHeadPaneGradient" colspan="2">
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:ValidationSummary ID="ValidationSummary1" runat="server"></asp:ValidationSummary>
                </td>
            </tr>
            <tr style="height: 20px">
                <td colspan="2" style="color: red;" align="center" valign="top">
                    <asp:Literal ID="Literal_error" runat="server" EnableViewState="False"></asp:Literal></td>
            </tr>
            <tr>
                <td>
                    <table id="inner" border="0" cellpadding="0" cellspacing="0">
                        <tr>
                            <td>
                                Selected MCN :
                            </td>
                            <td>
                                <asp:Label ID="Label_SelectedMcn" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <%--<tr>
                            <td>
                                Daily Feed Split Scheme :
                            </td>
                            <td>
                                <asp:DropDownList ID="DropDownList_SplitType" runat="server" CssClass="dropDown"
                                    AutoPostBack="True" OnSelectedIndexChanged="DropDownList_SplitType_SelectedIndexChanged">
                                    <asp:ListItem Text="SplitByMCN" Selected="True" Value="false"></asp:ListItem>
                                    <asp:ListItem Text="SplitByMCNAndCountry" Value="true"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        --%>
                        <tr>
                            <td style="vertical-align: top">
                                Daily Feed Split Scheme :
                            </td>
                            <td style="background-color: InfoBackground">
                                <asp:RadioButtonList ID="RadioButton_SchemeType" runat="server" AutoPostBack="true"
                                    OnSelectedIndexChanged="RadioButton_SchemeType_SelectedIndexChanged">
                                    <asp:ListItem Text="Split By Master Customer Number" Selected="True" Value="false">
                                    </asp:ListItem>
                                    <asp:ListItem Text="Split By Master Customer Number And Country" Value="true">
                                    </asp:ListItem>
                                </asp:RadioButtonList>
                                <%-- <asp:DropDownList ID="DropDownList1" runat="server" CssClass="dropDown"
                                 
                                    AutoPostBack="True" OnSelectedIndexChanged="DropDownList_SplitType_SelectedIndexChanged">
                                    <asp:ListItem Text="SplitByMCN" Selected="True" Value="false"></asp:ListItem>
                                    <asp:ListItem Text="SplitByMCNAndCountry" Value="true"></asp:ListItem>
                                </asp:DropDownList>--%>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Generate DFF Version 2 Format
                            </td>
                            <td>
                                <asp:CheckBox ID="Checkbox_DFFVersion2Configured" runat="server" />
                            </td>
                        </tr>
                        <%--<tr>
                            <td>
                                Ftp Prefix
                            </td>
                            <td>
                                <asp:TextBox ID="TextBox_FtpUri" runat="server" CssClass="textbox"></asp:TextBox>
                            </td>
                        </tr>--%>
                        <tr>
                            <td>
                            </td>
                            <td>
                                <div>
                                    <asp:GridView ID="GridView_DFConfig" runat="server" AutoGenerateColumns="False">
                                        <Columns>
                                            <asp:TemplateField>
                                                <HeaderTemplate>
                                                    <table width="100%">
                                                        <tr>
                                                            <td>
                                                                Organization
                                                            </td>
                                                            <td>
                                                                Daily Feed File Prefix</td>
                                                            <%--<td>
                                                            Daily Feed Folder Name</td>--%>
                                                        </tr>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="Label_Organization" runat="server" Text='<%#Eval("organization") %>'></asp:Label>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="TextBox_DailyFeedFilePrefix" runat="server" Text='<%#Eval("dailyfeedfileprefix")%>'>
                                                            </asp:TextBox>
                                                        </td>
                                                        <%--<td>
                                                        <asp:TextBox ID="TextBox_DailyFeedFolderPath" runat="server"
                                                        Text='<%#Eval("dailyfeedfolderpath")%>'></asp:TextBox>
                                                    </td>--%>
                                                    </tr>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    </table>
                                                </FooterTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                    <asp:GridView ID="GridView_Single" runat="server" AutoGenerateColumns="False">
                                        <Columns>
                                            <asp:TemplateField>
                                                <HeaderTemplate>
                                                    <table width="100%">
                                                        <tr>
                                                            <td>
                                                                Daily Feed File Prefix</td>
                                                            <%--                                                        <td>
                                                            Daily Feed Folder Path</td>
--%>
                                                        </tr>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <tr>
                                                        <td>
                                                            <asp:TextBox ID="TextBox_DailyFeedFilePrefix" runat="server" Text='<%#Eval("dailyfeedfileprefix")%>'>
                                                            </asp:TextBox>
                                                        </td>
                                                        <%--                                                    <td>
                                                        <asp:TextBox ID="TextBox_DailyFeedFolderPath" runat="server"
                                                        Text='<%#Eval("dailyfeedfolderpath")%>'></asp:TextBox>
                                                    </td>
--%>
                                                    </tr>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    </table>
                                                </FooterTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </div>
                                <%--<asp:Panel ID="Panel_Ctrl" runat="server" Width="100%" BorderStyle="Dotted"></asp:Panel>--%>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td style="font-weight: bold; padding-top: 30px; height: 49px;">
                    <asp:Button ID="Button_SaveUpdateDailyFeedScheme" runat="server" Text="Save" OnClick="Button_SaveUpdateDailyFeedScheme_Click"
                        CssClass="button" Width="60px" />&nbsp;
                    <input id="Button_Cancel" type="button" value="Cancel" onclick="window.location ='ConfigureDailyFeed.aspx'"
                        class="button" /></td>
            </tr>
        </table>
        <br />
        <br />
        <input type="button" runat="server" id="refresh" style="width: 0px; height: 0px;
            visibility: hidden" />
    </form>
</body>
</html>
