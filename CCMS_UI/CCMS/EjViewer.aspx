<%@ Page Language="C#" AutoEventWireup="true" Codebehind="EjViewer.aspx.cs" Inherits="CCMSUI.CCMS.EjViewer" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>EJ Log Viewer</title>
    <link href="../content/iSuite.css" rel="stylesheet" />
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
    <script language="javascript" type="text/javascript">
	function ToggleOptionView()
	{
		var optionsPanel = document.getElementById('optionsPanel');
		if (optionsPanel.style.display == 'none')
		{
			optionsPanel.style.display = 'block';
			document.getElementById('viewButton').value = 'Hide Options';
			
		}
		else
		{
			optionsPanel.style.display = 'none';
			document.getElementById('viewButton').value = 'Show Options';
		}	
	}
	
	function CheckAll()
	{
		for (var i=0; i<23; i++)
			document.getElementById('CHECKBOXLIST_devices_'+i).checked = true;
	}
	
	function UnCheckAll()
	{
			for (var i=0; i<23; i++)
			document.getElementById('CHECKBOXLIST_devices_'+i).checked = false;
	}
	

    </script>

</head>
<body style="margin-left: 0; margin-top: 0; margin-right: 0" onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)'>
    <form id="frmEJLogViewer" method="post" runat="server">
        <table id="maintable" style="width: 100%">
            <tr>
                <td class="PageHeading" style="vertical-align: middle">
                    EJ Viewer</td>
            </tr>
            <tr>
                <td width="100%">
                    <div align="right">
                        <input id="viewButton" onclick="ToggleOptionView()" type="button" value="Show Options"
                            class="button">

                        <input id="Button_Back" onclick="window.location='TerminalFileStatusMonitoring.aspx'" type="button" value="Back"
                            class="button">

                    </div>
                    <div id="optionsPanel" style="display: none">
                        <div style="color: navy">
                            <h4>
                                Device Exceptions</h4>
                        </div>
                        <asp:CheckBoxList ID="CHECKBOXLIST_devices" runat="server" Width="100%" RepeatColumns="3"
                            Font-Size="Small">
                            <asp:ListItem Value="E" Selected="True">Cash Handler</asp:ListItem>
                            <asp:ListItem Value="F" Selected="True">Envelope Depository</asp:ListItem>
                            <asp:ListItem Value="D" Selected="True">Card Reader/Writer</asp:ListItem>
                            <asp:ListItem Value="R" Selected="True">Operator Keyboard</asp:ListItem>
                            <asp:ListItem Value="G" Selected="True">Receipt Printer</asp:ListItem>
                            <asp:ListItem Value="q" Selected="True">Check Processing System</asp:ListItem>
                            <asp:ListItem Value="Y" Selected="True">Coin Dispenser</asp:ListItem>
                            <asp:ListItem Value="L" Selected="True">Encryptor</asp:ListItem>
                            <asp:ListItem Value="C" Selected="True">System Disk</asp:ListItem>
                            <asp:ListItem Value="K" Selected="True">Night Safe Depository</asp:ListItem>
                            <asp:ListItem Value="N" Selected="True">Door Access System</asp:ListItem>
                            <asp:ListItem Value="M" Selected="True">Security Camera</asp:ListItem>
                            <asp:ListItem Value="P" Selected="True">Tamper/Alarms/Sensors</asp:ListItem>
                            <asp:ListItem Value="H" Selected="True">Journal Printer</asp:ListItem>
                            <asp:ListItem Value="B" Selected="True">Power Failure</asp:ListItem>
                            <asp:ListItem Value="Q" Selected="True">Cardholder Keyboard</asp:ListItem>
                            <asp:ListItem Value="O" Selected="True">Offline Flex Disk</asp:ListItem>
                            <asp:ListItem Value="S" Selected="True">Cardholder Display</asp:ListItem>
                            <asp:ListItem Value="V" Selected="True">Statement Printer</asp:ListItem>
                            <asp:ListItem Value="Z" Selected="True">System Display</asp:ListItem>
                            <asp:ListItem Value="\[" Selected="True">Media Entry Indicator</asp:ListItem>
                            <asp:ListItem Value="\\" Selected="True">Envelope Dispenser</asp:ListItem>
                            <asp:ListItem Value="w" Selected="True">Bunch Notes Acceptor</asp:ListItem>
                        </asp:CheckBoxList>
                        <hr />
                        <div>
                            <input class="button" id="button_check" onclick="CheckAll()" type="button" value="Check All" />&nbsp;&nbsp;<input
                                class="button" id="button_uncheck" onclick="UnCheckAll()" type="button" value="UnCheck All" />&nbsp;
                                <input type ="submit"  value ="Apply"  class ="button" style="width: 72px" id="Button_Submit"   />
                        </div>
                    </div>
                </td>
            </tr>
            <tr>
                <td>
                    <table width="100%" cellpadding="0" cellspacing="0">
                        <tr>
                            <td width="100%" colspan="2">
                                <div id="selectedTrxPanel" style="display: none">
                                    <table width="100%" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td>
                                                <span style="font-size: medium; color: navy">Selected Transaction</span></td>
                                        </tr>
                                    </table>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td  valign="top" style="width:50%" >
                                <div >
                                    <span style="font-size: medium; color: navy">EJ File Text</span></div>
                                <div style=" cursor: text; font-family: 'Courier new'; background-color:  skyblue;
                                    word-wrap: break-all; padding-left:5px">
                                    <ol>
                                    <%  
                                        Response.Write("<li>");                                    
                                        for (int counter = 0; counter < formattedStr.Length - 1; counter++)
                                        {
                                            
                                            if (formattedStr[counter] == ' ' && formattedStr[counter + 1] == ' ')
                                                Response.Write("&nbsp;");
                                            else
                                            {
                                                if (formattedStr[counter] == '\r'  )
                                                    Response.Write("</li><br/><li>");
                                                else
                                                    Response.Write(formattedStr[counter]);
                                            }
                                        }					
                                    %>
                                    </ol>
                                </div>
                            </td>
                            <td valign="top" style="width:50%">
                                <div>
                                    <span style="font-size: medium; color: navy;">Exceptions found.</span></div>
                                <div style="cursor: text; background-color: infobackground;" >
                                    <ol style="list-style-position: outside; list-style-type: decimal">
                                        <%=result.ToString()%>
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
