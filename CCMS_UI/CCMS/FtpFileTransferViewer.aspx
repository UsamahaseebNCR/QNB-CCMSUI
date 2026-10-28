<%@ Page Language="C#" AutoEventWireup="true" Codebehind="FtpFileTransferViewer.aspx.cs" Inherits="CCMSUI.CCMS.FtpFileTransferViewer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
    <title>Cash Order Viewer</title>
    <link href="../content/iSuite.css" rel="stylesheet" />
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
    <script language="javascript" type="text/javascript">	
 function checkForBackSpace(e)
    {
    
        if (e.keyCode == 8 ) 
        {	
        document.getElementById('message').innerHTML='Please use the back button to go back to previous page.';
            
            return false;
            
        }
    }    
    function Button_CashOrder_Click()
    {   
        document.getElementById('Div_CashOrder_Label').style.visibility = "visible";
        document.getElementById('Div_CashOrder_Label').style.height = "auto";
        document.getElementById('Div_CashOrder').style.visibility = "visible";
        document.getElementById('Div_CashOrder').style.height = "auto";
        document.getElementById('Div_Exception_Label').style.visibility = "hidden";
        document.getElementById('Div_Exception_Label').style.height = "0px";
        document.getElementById('Div_Exception').style.visibility = "hidden";
        document.getElementById('Div_Exception').style.height = "0px";
    }
    
    function Button_Exception_Click()
    {
        document.getElementById('Div_CashOrder_Label').style.visibility = "hidden";
        document.getElementById('Div_CashOrder_Label').style.height = "0px";
        document.getElementById('Div_CashOrder').style.visibility = "hidden";        
        document.getElementById('Div_CashOrder').style.height = "0px";
        document.getElementById('Div_Exception_Label').style.visibility = "visible";        
        document.getElementById('Div_Exception_Label').style.height = "auto";
        document.getElementById('Div_Exception').style.visibility = "visible";
        document.getElementById('Div_Exception').style.height = "auto";
    }
    </script>
</head>

<body style="margin-left: 0; margin-top: 0; margin-right: 0" onkeydown=' return checkForBackSpace(window.event);' onkeyup='return checkForBackSpace(window.event)' onload='Button_CashOrder_Click();hideProgressBar();'>
    <form id="frmEJLogViewer" method="post" runat="server">
        <table id="maintable" style="width: 100%" border="0">
            <tr>
                <td class="PageHeading" style="vertical-align: middle" runat="server" id="tdHeading">Cash Order Imported from OptiCash</td>
            </tr>
            <tr>
                <td>
                
                    <div id="message"></div>
                </td>
            </tr>
            <tr>
                <td width="100%">
                    <div align="right">
                        <input  runat="server" id="Button_DisplayCashOrder" type="button" class="button" style="width:100px" value="Cash Order" onclick="Button_CashOrder_Click()" />&nbsp;
                        <input runat="server" id="Button_DisplayException" type="button" class="button" style="width:100px" value="Exceptions" onclick="Button_Exception_Click()" />&nbsp;
                        <input type="button" class="button" style="width:100px" value="Back" accesskey="BS" onclick="window.location='FTPFileTransferStatus.aspx'" />
                    </div>                   
                </td>
            </tr>
            <tr>
                <td>
                    <table width="100%" cellpadding="0" cellspacing="0">
                        <tr>
                            <td valign="top" style="width:100%;">
                                <div id="Div_CashOrder_Label" runat="server"><span runat="server" id="spanTitle" style='font-size: medium; color: navy'>Cash Order</span></div>
                                <div id="Div_CashOrder" runat="server" style="word-wrap: break-word ;cursor: text; font-family: 'Verdana'; background-color: #FFFFCA; padding-left:2px; ">
                                    <ol>
                                    <%  
                                        Response.Write("<li>");
                                        for (int counter = 0; counter < formattedStr_RawView.Length; counter++)
                                        {
                                            //if (counter % 200 == 0)
                                            //    Response.Write("<br/>");
                                            if (formattedStr_RawView[counter] == ' ' && formattedStr_RawView[counter + 1] == ' ')
                                                Response.Write("&nbsp;");
                                            else
                                            {
                                                if (formattedStr_RawView[counter] == '\r')
                                                    Response.Write("</li><br/><li>");
                                                else
                                                    Response.Write(formattedStr_RawView[counter]);
                                            }
                                        }					
                                    %>
                                    </ol>
                                </div>
                                <div id="Div_Exception_Label" runat="server"><span style='font-size: medium; color: navy'>Exceptions</span></div>
                                <div id="Div_Exception" runat="server" style=" word-wrap: break-word ; cursor: text; font-family: 'Verdana'; background-color: #FFFFCA; padding-left:5px;">
                                    <ol>
                                    <%  
                                            Response.Write("<li>");
                                        for (int counter = 0; counter < formattedStr_Exception.Length; counter++)
                                        {
                                            if (formattedStr_Exception[counter] == ' ' && formattedStr_Exception[counter + 1] == ' ')
                                                Response.Write("&nbsp;");
                                            else
                                            {
                                                if (formattedStr_Exception[counter] == '\r')
                                                    Response.Write("</li><br/><li>");
                                                else
                                                    Response.Write(formattedStr_Exception[counter]);
                                            }
                                        }					
                                    %>
                                    </ol>
                                </div>
                            </td>                            
                        </tr>
                    </table>
                    <asp:Literal ID="Literal_js" runat="server" EnableViewState="False"></asp:Literal></td>
            </tr>
        </table>
    </form>
</body>

</html>
