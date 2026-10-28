<%@ Page Language="C#" AutoEventWireup="true" Codebehind="ImportFromIMAC.aspx.cs"
    Inherits="CCMSUI.admin.ImportFromIMAC" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="../content/ISUITE.css" rel="stylesheet" />
    <title>iSuite - ATMs</title>
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
    <script language="javascript" type="text/javascript">    
    function setCheckbox(selected)
    {
	    var inputs = document.getElementsByTagName('input');
		
		for (var i = 0; i < inputs.length; i++)
        {
            if (inputs[i].type == "checkbox") 
                inputs[i].checked = selected;
        }
    }
    </script>
</head>

<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)' onload="hideProgressBar();">
    <form id="form1" runat="server">
        <div>
            <table style="width: 100%" cellspacing="0" cellpadding="0" border="0" class="mainbox">
                <tr>
                    <td class="PageHeading" align="left" colspan="2">Import ATM From IMAC</td>
                </tr>
                <tr>
                    <td class="ValidationText" align="center" colspan="2">
                        <asp:Literal ID="Literal_error" runat="server" EnableViewState="False"></asp:Literal>
                    </td>
                </tr>
                <tr>
                    <td class="ConfirmationText" align="center" colspan="2">
                        <%=Session[SessionVars.notice.ToString()]%>
                        <%Session[SessionVars.notice.ToString()] = "";%>
                        <asp:Literal ID="Literal_ok" runat="server" EnableViewState="False"></asp:Literal>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table id="inner" border="0" cellpadding="2" cellspacing="1" width="100%">
                            <tr>
                                <td colspan="2">&nbsp;</td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <strong>The columns ATM_ID, Region_ID, IP Address and MCN are mandatory.</strong>                        
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">&nbsp;</td>
                            </tr>
                            <tr>
                                <td>File Path *</td>
                                <td>
                                    <asp:FileUpload ID="FileUpload_imac" runat="server" CssClass="textbox" Width="375px"/>
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator_imac" runat="server" ControlToValidate="FileUpload_imac" ValidationExpression=".*\.xls(x)?">Only an xls/xlsx file is allowed</asp:RegularExpressionValidator>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator_imac" runat="server" ControlToValidate="FileUpload_imac">Please specify a file to upload</asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td>Columns</td>
                                <td>
                                    <asp:CheckBoxList ID="CheckBoxList_columns" runat="server" CssClass="textbox" Width="100%" Height="300px" AutoPostBack="false" RepeatColumns="5" RepeatDirection="Horizontal" RepeatLayout="Table">
                                        <asp:ListItem Text="Effective Date" Value="Effective Date" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="ATM Location Name" Value="ATM location name" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="ATM Address 1" Value="ATM address 1" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="ATM Address 2" Value="ATM address 2_(optional)" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="ATM City" Value="ATM city" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="ATM Country" Value="ATM country" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="ATM Postal/Zip Code" Value="ATM postal/zip code" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="Location Type" Value="Location Type" Selected="True"></asp:ListItem>                                        
                                        <asp:ListItem Text="ATM CM Service Status" Value="ATM CM Service Status" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="Holiday Status" Value="Holiday Status" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="ATM Business Days" Value="ATM Business-Days" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="Time Zone Offset" Value="Time-Zone-Offset" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="Maximum Number of Notes per Cassette" Value="Maximum_Number of Notes per Cassette" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="Denomination 1" Value="Denomination 1" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="Capacity Denomination 1" Value="Capacity Denomination 1" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="Split Percentage 1" Value="Split percentage 1" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="Denomination 2" Value="Denomination 2" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="Capacity Denomination 2" Value="Capacity Denomination 2" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="Split Percentage 2" Value="Split percentage 2" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="Denomination 3" Value="Denomination 3" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="Capacity Denomination 3" Value="Capacity Denomination 3" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="Split Percentage 3" Value="Split percentage 3" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="Denomination 4" Value="Denomination 4" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="Capacity Denomination 4" Value="Capacity Denomination 4" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="Split Percentage 4" Value="Split percentage 4" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="Interest Rate" Value="Interest rate" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="Insurance Rate" Value="Insurance rate" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="Max Holding Amount" Value="Max- Holding Amount" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="Min Operating Balance" Value="Min Operating Balance _(optional)" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="Min Amount for Normal Delivery" Value="Min-amount for normal Delivery _(optional)" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="Bank Cash Center ID" Value="Bank Cash Center ID" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="CIT Cash Center/Servicer" Value="CIT Cash Center / Servicer" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="Depot ID" Value="Depot ID" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="Secondary Depot Vault ID" Value="secondary depot vault id" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="New ATM Scenario" Value="New ATM Scenario" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="Cash Swap days" Value="Cash Swap days" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="Mandatory Cash Swap Days" Value="Mandatory Cash Swap days" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="Cash Swap Cycle" Value="Cash Swap cycle" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="Cash Swap Lead Time" Value="Cash Swap Lead-Time" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="Cash Swap Start Date" Value="Cash Swap Start Date" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="Cash Swap Handling Costs" Value="Cash Swap Handling Costs" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="Cash Swap Costs" Value="Cash Swap Costs" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="Emergency Days" Value="Emergency days" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="Emergency Lead Time" Value="Emergency Lead-Time" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="Emergency Costs" Value="Emergency Costs" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="Contact A" Value="Contact A" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="Contact A Phone" Value="Contact A Phone" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="Contact B" Value="Contact B" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="Contact B Phone" Value="Contact B Phone" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="Contact C" Value="Contact C" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="Contact C Phone" Value="Contact C Phone" Selected="True"></asp:ListItem>                                        
                                        <asp:ListItem Text="Longitude" Value="Longitude" Selected="True"></asp:ListItem>  
                                        <asp:ListItem Text="Latitude" Value="Latitude" Selected="True"></asp:ListItem>  
                                        <asp:ListItem Text="ATM Type" Value="ATM Type" Selected="True"></asp:ListItem>  
                                        <asp:ListItem Text="Distribution Port" Value="Distribution Port" Selected="True"></asp:ListItem>  
                                        <asp:ListItem Text="Vendor" Value="Vendor" Selected="True"></asp:ListItem>  
                                                                              

                                    </asp:CheckBoxList>
                                </td>
                            </tr>
                            <tr>
                                <td></td>
                                <td align="right"> 
                                    <input id="Button_CheckAll" type="button" value="Check All" onclick="setCheckbox(true)" class="button" style="width:90px" />
                                    <input id="Button_UncheckAll" type="button" value="Uncheck All" onclick="setCheckbox(false)" class="button" style="width:90px" />
                                </td>
                            </tr>
                            <tr>
                                <td>Empty Character</td>
                                <td>
                                    <asp:DropDownList ID="DropDownList_specialCharacter" runat="server" CssClass="textbox" Width="50px">
                                        <asp:ListItem Text="#" Value="#" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="~" Value="~"></asp:ListItem>
                                        <asp:ListItem Text="!" Value="!"></asp:ListItem>
                                        <asp:ListItem Text="@" Value="@"></asp:ListItem>                                                        
                                        <asp:ListItem Text="$" Value="$"></asp:ListItem>
                                        <asp:ListItem Text="%" Value="%"></asp:ListItem>
                                        <asp:ListItem Text="^" Value="^"></asp:ListItem>
                                        <asp:ListItem Text="&" Value="&"></asp:ListItem>
                                        <asp:ListItem Text="|" Value="|"></asp:ListItem>
                                        <asp:ListItem Text="?" Value="?"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>                                            
                            <tr>
                                <td></td>
                                <td>
                                    <asp:Button ID="Button_Submit" runat="server" Text="Upload" CssClass="button" OnClick="Button_Submit_Click" Width="100px" CausesValidation="true" />
                                    <input id="Button_Cancel" type="button" value="Cancel" onclick="window.location='ATMs.aspx'" class="button" style="width:100px" />
                                </td>                        
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
