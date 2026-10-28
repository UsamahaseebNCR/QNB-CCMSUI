<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CashDataViewer.aspx.cs"
    Inherits="CCMSUI.CCMS.CashDataViewer" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>EJ Log Viewer</title>
    <link href="../content/iSuite.css" rel="stylesheet" />
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
    <script language="javascript" type="text/javascript">


        function checkForBackSpace(e) {

            if (e.keyCode == 8) {
                document.getElementById('message').innerHTML = 'Please use the back button to go back to previous page.';

                return false;

            }
        }
        function Button_RawView_Click() {
            document.getElementById('Div_RawView').style.visibility = "visible";
            document.getElementById('Div_RawView').style.height = "auto";
            document.getElementById('Div_FriendlyView').style.visibility = "hidden";
            document.getElementById('Div_FriendlyView').style.height = "0px";
        }

        function Button_FriendlyView_Click() {
            document.getElementById('Div_RawView').style.visibility = "hidden";
            document.getElementById('Div_RawView').style.height = "0px";
            document.getElementById('Div_FriendlyView').style.visibility = "visible";
            document.getElementById('Div_FriendlyView').style.height = "auto";
        }
    </script>
</head>
<body style="margin-left: 0; margin-top: 0; margin-right: 0" onkeydown='return checkForBackSpace(window.event);'
    onkeyup='return checkForBackSpace(window.event);' onload='Button_RawView_Click()'>
    <form id="frmEJLogViewer" method="post" runat="server">
    <table id="maintable" style="width: 100%">
        <tr>
            <td class="PageHeading" style="vertical-align: middle">
                Cash Data Viewer
            </td>
        </tr>
        <tr>
            <td width="100%">
                <div align="right">
                    <input type="button" class="button" style="width: 100px" value="Original View" onclick="Button_RawView_Click()" />&nbsp;
                    <input type="button" class="button" style="width: 100px" value="Friendly View" onclick="Button_FriendlyView_Click()" />&nbsp;
                    <input type="button" class="button" style="width: 100px" value="Back" onclick="window.location='TerminalFileStatusMonitoring.aspx'" />
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <div id="message">
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <table width="100%" cellpadding="0" cellspacing="0">
                    <tr>
                        <td valign="top" >
                            <div>
                                <span style="font-size: medium; color: navy">Cash Data</span></div>
                            <div id="Div_RawView" style="overflow:auto; cursor: text; font-family: 'Verdana';
                                background-color: #FFFFCA; padding-left: 5px;">
                                <%--<ol>--%>
                                    <%  
                                        //Response.Write("<li>");
                                        //int len = 0;
                                        //for (int counter = 0; counter < formattedStr_RawView.Length; counter++)
                                        //{
                                        //    if (formattedStr_RawView[counter] == ' ' && formattedStr_RawView[counter + 1] == ' ')
                                        //        Response.Write("&nbsp;");
                                        //    else
                                        //    {
                                        //        if (formattedStr_RawView[counter] == '\r')
                                        //        {
                                        //            Response.Write("<br/>");
                                        //            len = 0;
                                        //        }
                                        //        else
                                        //        {
                                        //            if (len == 100)
                                        //            {
                                        //                Response.Write("<br/>");
                                        //                len = 0;

                                        //            }
                                        //            Response.Write(formattedStr_RawView[counter]);
                                        //        }
                                        //    }
                                        //    len++;
                                        //}					
                                    %>
                                <%--//</ol>--%>
                            </div>
                            <div id="Div_FriendlyView" runat="server" style="cursor: text; font-family: 'Verdana';
                                background-color: #FFFFCA; padding-left: 5px;">
                                <ol>
                                    <%  
                                        Response.Write("<li>");
                                        int len = 0;
                                        for (int counter = 0; counter < formattedStr_FriendlyView.Length; counter++)
                                        {
                                            if (formattedStr_FriendlyView[counter] == ' ' && formattedStr_FriendlyView[counter + 1] == ' ')
                                                Response.Write("&nbsp;");
                                            else
                                            {
                                                if (formattedStr_FriendlyView[counter] == '\r')
                                                {
                                                    Response.Write("</li><br/><li>");
                                                    len = 0;
                                                }

                                                else
                                                {
                                                    if (len == 150)
                                                    {
                                                        Response.Write("<br/>");
                                                        len = 0;

                                                    }
                                                    Response.Write(formattedStr_FriendlyView[counter]);
                                                }
                                            }
                                            len++;
                                        }					
                                    %>
                                </ol>
                            </div>
                                                    </td>
                    </tr>
                </table>
                <asp:Literal ID="Literal_js" runat="server" EnableViewState="False"></asp:Literal>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
