<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddEditVaultAdj.aspx.cs"
    Inherits="CCMSUI.admin.AddEditVaultAdj" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Add Edit Vault Adjustments</title>
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
    <link href="../content/Scripts/ui-lightness/jquery-ui-1.7.2.custom.css" rel="stylesheet"
        type="text/css" />
    <script type="text/javascript" src="../content/scripts/jquery-1.4.1.js"></script>
    <script type="text/javascript" src="../content/scripts/ui.datepicker.js"></script>
    <script type="text/javascript">

        $(function () {

            $('.datepicker').datepicker({ dateFormat: 'dd/mm/yy',
                changeMonth: true,
                changeYear: true,
                showOn: "button",
                buttonImage: "../images/calendar.gif",
                buttonImageOnly: true

            });
        });
    </script>
    <script type="text/javascript">
        function setCheck(node, selected) {
            var childary = node.get_nodes();

            for (var i = 0; i < childary.get_length(); i++) {
                //if (childary.getNode(i).get_id().substring(0, 1) == 'r') {
                childary.getNode(i).set_checked(selected);
                setCheck(childary.getNode(i), selected)
                //}
                //else
                //  childary.getNode(i).set_checked(selected);
            }
        }

        window.parent.window.contentPage = '';

        function check(sender, eventArgs) {
            regionTreeView.beginUpdate();
            setCheck(eventArgs.get_node(), eventArgs.get_node().get_checked());
            regionTreeView.endUpdate();
        }



        function updateTotalAmount() {
            var total =0 ;
            var arr = new Array();

            var Grid_Table = document.getElementById('<%= GridView_VaultNoteDetail.ClientID %>');

            for (var row = 1; row < Grid_Table.rows.length; row++) {

                for (var col = 0; col < Grid_Table.rows[row].cells.length; col++) {

                    if (col == 0) {
                        if (document.all) {
                            // str = str + Grid_Table.rows[row].cells[col].innerText;
                            //alert(Grid_Table.rows[row].cells[col].innerText.substring(3));
                            arr.push(Grid_Table.rows[row].cells[col].innerText.substring(3));
                        }

                        else {
                            //str = str + Grid_Table.rows[row].cells[col].textContent;
                            //alert(Grid_Table.rows[row].cells[col].textContent.substring(3));
                            arr.push(Grid_Table.rows[row].cells[col].textContent.substring(3));
                        }
                    }

                    else {

                        input = Grid_Table.rows[row].cells[col].getElementsByTagName("input");
                        if (input[0].value > 0) {
                            //alert(arr.pop());
                            total+= arr.pop()*input[0].value;
                        }

                    }

                    //str = str + '\n';

                }
            }
            //alert(total);
            document.getElementById('<%=Label_Amount.ClientID %>').innerHTML = total;
            document.getElementById('<%=amt.ClientID %>').value = total;
            return false;
        }

    </script>
    <script type="text/javascript">
        window.parent.window.contentPage = '';

        
    </script>
    <link href="../content/iSuite.css" rel="stylesheet" />
</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)'
    onload="hideProgressBar();">
    <form id="form1" runat="server">
    <input type="hidden" id="amt" runat="server" />
    <table cellpadding="0" cellspacing="0" border="0" class="MainBox" width="100%">
        <tr>
            <td class="PageHeading" align="left" colspan="2" style="height: 25px">
                <%=Request.QueryString["id"] == null?"Post Vault Adjustment ":"Edit Vault Adjustment"%>
            </td>
        </tr>
        <tr>
            <td class="SearchHeadPaneGradient" colspan="2">
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="SaveCheque">
                </asp:ValidationSummary>
            </td>
        </tr>
        <tr style="height: 20px">
            <td colspan="2" style="color: red;" align="center" valign="top">
                <div id="msg"></div>
                <asp:Literal ID="Literal_error" runat="server" EnableViewState="False"></asp:Literal>
            </td>
        </tr>
        <tr>
            <td>
                <table id="inner" border="0" cellpadding="0" cellspacing="0" width="100%" style="padding-left: 10px">
                    <tr>
                        <td style="width: 200px">
                            Organization
                        </td>
                        <td>
                            <asp:Label ID="Label_Org" runat="server" Text="Please select Organization" Font-Bold="true"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            CIT
                        </td>
                        <td>
                            <asp:DropDownList ID="DropDownList_CIT" runat="server" Width="151px" CssClass="dropDown"
                                AutoPostBack="true" OnSelectedIndexChanged="DropDownList_CIT_Change">
                            </asp:DropDownList>
                            <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToValidate="DropDownList_CIT"
                                Type="String" ValueToCompare="Select" Operator="NotEqual" ErrorMessage="Please select CIT"
                                ValidationGroup="SaveCheque">*</asp:CompareValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Vault
                        </td>
                        <td>
                            <asp:DropDownList ID="DropDownList_Vault" runat="server" Width="150px" CssClass="dropDown"
                                AutoPostBack="true" OnSelectedIndexChanged="DropDownList_Vault_SelectedIndexChanged">
                            </asp:DropDownList>
                            <asp:CompareValidator ID="CompareValidator2" runat="server" ControlToValidate="DropDownList_Vault"
                                Type="String" ValueToCompare="Select" Operator="NotEqual" ErrorMessage="Please select Vault"
                                ValidationGroup="SaveCheque">*</asp:CompareValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Transaction Type
                        </td>
                        <td>
                            <asp:DropDownList ID="DropDownList_TrxnType" runat="server">
                                <asp:ListItem Text="Cr"></asp:ListItem>
                                <asp:ListItem Text="Dr"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    
                    <tr>
                        <td>
                            Amount
                        </td>
                        <td>
                            <asp:Label ID="Label_Amount" runat="server" Font-Bold="true"  ></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <fieldset style="width: 40%">
                                <legend><b>Denomination Details</b> </legend>
                                <asp:GridView ID="GridView_VaultNoteDetail" runat="server" Width="100%" AutoGenerateColumns="false"
                                    OnRowDataBound="GridView_VaultNoteDetail_RowDataBound">
                                    <RowStyle HorizontalAlign="Center" />
                                    <AlternatingRowStyle BackColor="#eff0ef" />
                                    <Columns>
                                        <asp:BoundField DataField="type" HeaderText="Denomination Type" HeaderStyle-ForeColor="black">
                                            <HeaderStyle CssClass="HeadingCell" />
                                        </asp:BoundField>
                                        <asp:TemplateField>
                                            <HeaderStyle CssClass="HeadingCell" />
                                            <HeaderTemplate>
                                                Quantity
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                            <input type="text" id="TextBox_Quantity" runat="server" class="textbox"
                                             onblur="updateTotalAmount();" />
                                                <%--<asp:TextBox ID="TextBox_Quantity" runat="server" CssClass="textbox" Width="120px"
                                                  ></asp:TextBox>--%>
                                                <asp:RegularExpressionValidator ID="regularexpressionvalidator_EmergencyCash" runat="server"
                                                    ControlToValidate="TextBox_Quantity" ErrorMessage="Only Numbers are allowed"
                                                    ValidationExpression="[0-9]*" Text="*"></asp:RegularExpressionValidator>
                                                <asp:RequiredFieldValidator ID="req_Quantity" ControlToValidate="TextBox_Quantity"
                                                    runat="server" ErrorMessage="Please enter Quantity">*</asp:RequiredFieldValidator>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="vault_adjustment_id"></asp:BoundField>
                                    </Columns>
                                </asp:GridView>
                            </fieldset>
                        </td>
                    </tr>
                    <tr id="trAddEditVaultAdj" runat="server">
                        <td colspan="2" style="font-weight: bold; padding-top: 30px;">
                            <asp:Button ID="Button_SaveUpdateVaultAdj" runat="server" Text="Save" OnClick="Button_SaveUpdateVaultAdj_Click" 
                                CssClass="button" Width="60px" ValidationGroup="SaveCheque" />&nbsp;
                            <input id="Button_Cancel" type="button" value="Cancel" onclick="window.location ='VaultAdjustments.aspx'"
                                class="button" />
                        </td>
                    </tr>
                    <tr id="trApproveRejectVaultAdj" runat="server">
                        <td colspan="2" style="font-weight: bold; padding-top: 30px;">
                            <asp:Button ID="Button_ApproveVaultAdj" runat="server" Text="Approve" OnClick="Button_ApproveVaultAdj_Click"
                                CssClass="button" Width="60px" ValidationGroup="SaveCheque" />&nbsp;
                            <asp:Button ID="Button_RejectVaultAdj" runat="server" Text="Reject" OnClick="Button_Reject_Click"
                                CssClass="button" Width="60px" ValidationGroup="SaveCheque" />&nbsp;
                            <input id="Button2" type="button" value="Cancel" onclick="window.location ='../ccms/usertask.aspx'"
                                class="button" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <br />
    <br />
    <input type="button" runat="server" id="refresh" style="display: none" />
    </form>
</body>
</html>
