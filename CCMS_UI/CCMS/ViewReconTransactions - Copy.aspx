<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewReconTransactions.aspx.cs"
    Inherits="CCMSUI.CCMS.ViewReconTransactions" %>

<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
    <title>View Reconciliation Transactions</title>
    <script type="text/javascript">
        window.parent.window.SetContentPage(window.location);


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

        function LabelText(labeltext) {
            document.getElementById('Label_errorMsg').innerHTML = "<li>" + labeltext + "</li>";
        }
        function onCloseOfDialog() {
            DialogCallBack.callback(document.getElementById('reconTrxnID').value, document.getElementById('comment').value, document.getElementById('operationType').value, document.getElementById('amount').value, document.getElementById('gridID').value);
            Dialog2.Close();
        }

        function openDialog(recondTrxnID, amount, type,selectedGrid) {
            document.getElementById('reconTrxnID').value = recondTrxnID;
            document.getElementById('operationType').value = type;
            document.getElementById('amount').value = amount;
            document.getElementById('gridID').value = selectedGrid;
            Dialog2.Show();
        }

    </script>
    <link href="../content/iSuite.css" rel="stylesheet" />
</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)'
    onload="hideProgressBar();">
    <form id="form1" runat="server">
    <%--<input type="hidden" name="commentsForServer" runat="server" />--%>
    <input type="hidden" id="amount" />
    <input type="hidden" id="reconTrxnID" />
    <input type="hidden" id="operationType" />
    <input type="hidden" id="gridID" />
    <input type="hidden" id="batchID" runat="server" />
    <div>
        <table style="width: 100%" cellspacing="0" cellpadding="0" border="0" class="mainbox">
            <tr>
                <td class="PageHeading" align="left" colspan="2">
                    View Reconciliation Transactions
                </td>
            </tr>
            <tr>
                <td class="ValidationText" align="center">
                    <asp:Literal ID="Literal_error" runat="server" EnableViewState="False"></asp:Literal>
                    <div class="ValidationText">
                        <asp:Label ID="Label_errorMsg" runat="server" EnableViewState="False"></asp:Label></div>
                </td>
            </tr>
            <tr>
                <td>
                    <div id="notificationArea" style="text-align: center">
                    </div>
                    <asp:Literal ID="Literal_msg" runat="server" EnableViewState="false">
                    </asp:Literal>
                </td>
                <td class="ConfirmationText" align="center">
                    <%=Session[SessionVars.notice.ToString()]%>
                    <%Session[SessionVars.notice.ToString()] = "";%>
                    <asp:Literal ID="Literal_ok" runat="server" EnableViewState="False"></asp:Literal>
                </td>
            </tr>
            <tr>
                <td>
                    <table>
                        <tr>
                            <td>
                                <h2 style="text-decoration: underline">
                                    Batch Summary</h2>
                                <table>
                                    <tr>
                                        <td>
                                            Batch Auto Reconciled Amount
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_AutoReconciledAmount" runat="server" ></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Batch Manually Reconciled Amount
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_ManuallyReconciledAmount" runat="server" ></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Batch Amount For Dropped Transactions
                                        </td>
                                        <td>
                                            <asp:Label ID="Label_DroppedTrxnAmount" runat="server" ></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>&nbsp;</td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <ComponentArt:TabStrip ID="TabStrip_AddEdit" CssClass="TopGroup" DefaultItemLookId="DefaultTabLook"
                        DefaultSelectedItemLookId="SelectedTabLook" DefaultDisabledItemLookId="DisabledTabLook"
                        DefaultGroupTabSpacing="1px" ImagesBaseUrl="../images/Tabimages/" MultiPageId="MultiPage_AddEdit"
                        runat="server" ScrollDownLookId="DefaultTabLook" ScrollLeftLookId="DefaultTabLook"
                        ScrollRightLookId="DefaultTabLook" ScrollUpLookId="DefaultTabLook" TopGroupTabSpacing="1px">
                        <ItemLooks>
                            <ComponentArt:ItemLook LookId="DefaultTabLook" CssClass="DefaultTab" HoverCssClass="DefaultTabHover"
                                LabelPaddingLeft="10px" LabelPaddingRight="10px" LabelPaddingTop="5px" LabelPaddingBottom="4px"
                                LeftIconUrl="tab_left_icon.gif" RightIconUrl="tab_right_icon.gif" HoverLeftIconUrl="hover_tab_left_icon.gif"
                                HoverRightIconUrl="hover_tab_right_icon.gif" LeftIconWidth="3px" LeftIconHeight="21px"
                                RightIconWidth="3px" RightIconHeight="21px" LeftIconVisibility="Always" RightIconVisibility="Always" />
                            <ComponentArt:ItemLook LookId="SelectedTabLook" CssClass="SelectedReportsTab" LabelPaddingLeft="10px"
                                LabelPaddingRight="10px" LabelPaddingTop="4px" LabelPaddingBottom="4px" LeftIconUrl="selected_tab_left_icon.gif"
                                RightIconUrl="selected_tab_right_icon.gif" LeftIconWidth="3px" LeftIconHeight="21px"
                                RightIconWidth="3px" RightIconHeight="21px" LeftIconVisibility="Always" RightIconVisibility="Always" />
                        </ItemLooks>
                        <Tabs>
                            <ComponentArt:TabStripTab ID="TabStripTab_general" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Electronic Journal">
                            </ComponentArt:TabStripTab>
                            <ComponentArt:TabStripTab ID="TabStripTab_physicalConfig" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Currency And Cash Management System">
                            </ComponentArt:TabStripTab>
                            <ComponentArt:TabStripTab ID="TabStripTab_CDMConfig" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Electronic Journal VS Currency And Cash Management System">
                            </ComponentArt:TabStripTab>
                        </Tabs>
                    </ComponentArt:TabStrip>
                    <ComponentArt:MultiPage ID="MultiPage_AddEdit" CssClass="MultiPage" runat="server"
                        Width="100%">
                        <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView_general"
                            Width="100%">
                            <table width="100%">
                                <tr>
                                    <td style="vertical-align: top" width="50%">
                                        <table width="100%">
                                            <tr>
                                                <td align="center" valign="top" width="50%" style="background-color: Gray; color: White;
                                                    font-size: medium; font-weight: bold">
                                                    EJ VS Switch
                                                </td>
                                            </tr>
                                            <tr>
                                                <td valign="top">
                                                    <asp:GridView ID="GridView_EJSwitch" runat="server" AutoGenerateColumns="False" AllowSorting="true"
                                                        Width="100%" OnSorting="GridView_EJSwitch_Sorting" DataKeyNames="reconciliation_transactions_id"
                                                        RowStyle-HorizontalAlign="center" OnPageIndexChanging="GridView_EJSwitch_PageIndexChanging"
                                                         OnRowDataBound ="GridView_OnRowDataBound">
                                                        <AlternatingRowStyle BackColor="#eff0ef" />
                                                        <HeaderStyle CssClass="HeadingCell" HorizontalAlign="Left" ForeColor="black" />
                                                        <RowStyle HorizontalAlign="Center" />
                                                        <Columns>
                                                            <%--<asp:TemplateField HeaderText="Select" HeaderStyle-CssClass="HeadingCell" ItemStyle-HorizontalAlign="Center">
                                                                <ItemTemplate>
                                                                    <asp:CheckBox ID="chkbx" runat="server"></asp:CheckBox>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>--%>
                                                            <asp:BoundField DataField="title" HeaderText="Title" HeaderStyle-CssClass="HeadingCell"
                                                                ItemStyle-HorizontalAlign="Center" SortExpression="title" />
                                                            <asp:BoundField DataField="trxn_datetime" HeaderText="Datetime" HeaderStyle-CssClass="HeadingCell"
                                                                ItemStyle-HorizontalAlign="Center" SortExpression="trxn_datetime" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" />
                                                            <asp:BoundField DataField="PAN" HeaderText="PAN" SortExpression="PAN" ItemStyle-HorizontalAlign="Center"
                                                                HeaderStyle-CssClass="HeadingCell" />
                                                            <asp:BoundField DataField="tsn" HeaderText="TSN" SortExpression="tsn" ItemStyle-HorizontalAlign="Center"
                                                                HeaderStyle-CssClass="HeadingCell" />
                                                            <asp:BoundField DataField="amount" HeaderText="Amount" SortExpression="amount" ItemStyle-HorizontalAlign="Center"
                                                                HeaderStyle-CssClass="HeadingCell" />
                                                            <asp:BoundField DataField="status" HeaderText="Status" SortExpression="status" ItemStyle-HorizontalAlign="Left"
                                                                HeaderStyle-CssClass="HeadingCell" />
                                                            <asp:BoundField DataField="reason" HeaderText="Reason" SortExpression="reason" ItemStyle-HorizontalAlign="Left"
                                                                HeaderStyle-CssClass="HeadingCell" />
                                                            <asp:TemplateField HeaderText="Drop" ItemStyle-HorizontalAlign="Center">
                                                                <ItemTemplate>
                                                                    <a onclick='openDialog(<%# Eval("reconciliation_transactions_id") %>,<%# Eval("amount") %>,0,1)'>
                                                                        <img alt="Click to drop this transaction" src="../images/edit.png" style="border: 0"
                                                                            class="ShowHand" width="20px" height="20px" />
                                                                    </a>
                                                                </ItemTemplate>
                                                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Reconcile" ItemStyle-HorizontalAlign="Center">
                                                                <ItemTemplate>
                                                                    <a onclick='openDialog(<%# Eval("reconciliation_transactions_id") %>,<%# Eval("amount") %>,1,1)'>
                                                                        <img alt="Click to reconcile this transaction" src="../images/recon.jpg" style="border: 0"
                                                                            class="ShowHand" width="20px" height="20px" />
                                                                    </a>
                                                                </ItemTemplate>
                                                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                                            </asp:TemplateField>
                                                        </Columns>
                                                        <RowStyle HorizontalAlign="Left" />
                                                        <EmptyDataTemplate>
                                                            No Rows Found
                                                        </EmptyDataTemplate>
                                                    </asp:GridView>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                     <td style="vertical-align: top" width="50%">
                                        <table width="100%">
                                            <tr>
                                                <td align="center" valign="top" width="50%" style="background-color: Gray; color: White;
                                                    font-size: medium; font-weight: bold">
                                                    EJ BNA VS Switch
                                                </td>
                                            </tr>
                                            <tr>
                                                <td valign="top">
                                                    <asp:GridView ID="GridView_EJBNASwitch" runat="server" AutoGenerateColumns="False" AllowSorting="true"
                                                        Width="100%" OnSorting="GridView_EJBNASwitch_Sorting" DataKeyNames="reconciliation_transactions_id"
                                                        RowStyle-HorizontalAlign="center" OnPageIndexChanging="GridView_EJBNASwitch_PageIndexChanging"
                                                         OnRowDataBound ="GridView_OnRowDataBound">
                                                        <AlternatingRowStyle BackColor="#eff0ef" />
                                                        <HeaderStyle CssClass="HeadingCell" HorizontalAlign="Left" ForeColor="black" />
                                                        <RowStyle HorizontalAlign="Center" />
                                                        <Columns>
                                                            <%--<asp:TemplateField HeaderText="Select" HeaderStyle-CssClass="HeadingCell" ItemStyle-HorizontalAlign="Center">
                                                                <ItemTemplate>
                                                                    <asp:CheckBox ID="chkbx" runat="server"></asp:CheckBox>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>--%>
                                                            <asp:BoundField DataField="title" HeaderText="Title" HeaderStyle-CssClass="HeadingCell"
                                                                ItemStyle-HorizontalAlign="Center" SortExpression="title" />
                                                            <asp:BoundField DataField="trxn_datetime" HeaderText="Datetime" HeaderStyle-CssClass="HeadingCell"
                                                                ItemStyle-HorizontalAlign="Center" SortExpression="trxn_datetime" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" />
                                                            <asp:BoundField DataField="PAN" HeaderText="PAN" SortExpression="PAN" ItemStyle-HorizontalAlign="Center"
                                                                HeaderStyle-CssClass="HeadingCell" />
                                                            <asp:BoundField DataField="tsn" HeaderText="TSN" SortExpression="tsn" ItemStyle-HorizontalAlign="Center"
                                                                HeaderStyle-CssClass="HeadingCell" />
                                                            <asp:BoundField DataField="amount" HeaderText="Amount" SortExpression="amount" ItemStyle-HorizontalAlign="Center"
                                                                HeaderStyle-CssClass="HeadingCell" />
                                                            <asp:BoundField DataField="status" HeaderText="Status" SortExpression="status" ItemStyle-HorizontalAlign="Left"
                                                                HeaderStyle-CssClass="HeadingCell" />
                                                            <asp:BoundField DataField="reason" HeaderText="Reason" SortExpression="reason" ItemStyle-HorizontalAlign="Left"
                                                                HeaderStyle-CssClass="HeadingCell" />
                                                            <asp:TemplateField HeaderText="Drop" ItemStyle-HorizontalAlign="Center">
                                                                <ItemTemplate>
                                                                    <a onclick='openDialog(<%# Eval("reconciliation_transactions_id") %>,<%# Eval("amount") %>,0,1)'>
                                                                        <img alt="Click to drop this transaction" src="../images/edit.png" style="border: 0"
                                                                            class="ShowHand" width="20px" height="20px" />
                                                                    </a>
                                                                </ItemTemplate>
                                                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Reconcile" ItemStyle-HorizontalAlign="Center">
                                                                <ItemTemplate>
                                                                    <a onclick='openDialog(<%# Eval("reconciliation_transactions_id") %>,<%# Eval("amount") %>,1,1)'>
                                                                        <img alt="Click to reconcile this transaction" src="../images/recon.jpg" style="border: 0"
                                                                            class="ShowHand" width="20px" height="20px" />
                                                                    </a>
                                                                </ItemTemplate>
                                                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                                            </asp:TemplateField>
                                                        </Columns>
                                                        <RowStyle HorizontalAlign="Left" />
                                                        <EmptyDataTemplate>
                                                            No Rows Found
                                                        </EmptyDataTemplate>
                                                    </asp:GridView>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td style="vertical-align: top">
                                        <table width="100%">
                                            <tr>
                                                <td align="center" valign="top" width="50%" style="background-color: Silver; color: Black;
                                                    font-size: medium; font-weight: bold">
                                                    Switch VS Host
                                                </td>
                                            </tr>
                                            <tr>
                                                <td valign="top">
                                                    <asp:GridView ID="GridView_SwitchHost" runat="server" AutoGenerateColumns="False"
                                                        AllowSorting="true" Width="100%" OnSorting="GridView_SwitchHost_Sorting" DataKeyNames="reconciliation_transactions_id"
                                                        RowStyle-HorizontalAlign="center" OnPageIndexChanging="GridView_SwitchHost_PageIndexChanging"
                                                        OnRowDataBound ="GridView_OnRowDataBound">
                                                        <AlternatingRowStyle BackColor="#eff0ef" />
                                                        <HeaderStyle CssClass="HeadingCell" HorizontalAlign="Left" ForeColor="black" />
                                                        <RowStyle HorizontalAlign="Center" />
                                                        <Columns>
                                                            <%--<asp:TemplateField HeaderText="Select" HeaderStyle-CssClass="HeadingCell" ItemStyle-HorizontalAlign="Center">
                                                                <ItemTemplate>
                                                                    <asp:CheckBox ID="chkbx" runat="server"></asp:CheckBox>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>--%>
                                                            <asp:BoundField DataField="title" HeaderText="Title" HeaderStyle-CssClass="HeadingCell"
                                                                ItemStyle-HorizontalAlign="Center" SortExpression="title" />
                                                            <asp:BoundField DataField="trxn_datetime" HeaderText="Datetime" HeaderStyle-CssClass="HeadingCell"
                                                                ItemStyle-HorizontalAlign="Center" SortExpression="trxn_datetime" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}"/>
                                                            <asp:BoundField DataField="PAN" HeaderText="PAN" SortExpression="PAN" ItemStyle-HorizontalAlign="Center"
                                                                HeaderStyle-CssClass="HeadingCell" />
                                                            <asp:BoundField DataField="tsn" HeaderText="TSN" SortExpression="tsn" ItemStyle-HorizontalAlign="Center"
                                                                HeaderStyle-CssClass="HeadingCell" />
                                                            <asp:BoundField DataField="amount" HeaderText="Amount" SortExpression="amount" ItemStyle-HorizontalAlign="Center"
                                                                HeaderStyle-CssClass="HeadingCell" />
                                                            <asp:BoundField DataField="status" HeaderText="Status" SortExpression="status" ItemStyle-HorizontalAlign="Left"
                                                                HeaderStyle-CssClass="HeadingCell" />
                                                            <asp:BoundField DataField="reason" HeaderText="Reason" SortExpression="reason" ItemStyle-HorizontalAlign="Left"
                                                                HeaderStyle-CssClass="HeadingCell" />
                                                            <asp:TemplateField HeaderText="Drop" ItemStyle-HorizontalAlign="Center">
                                                                <ItemTemplate>
                                                                    <a onclick='openDialog(<%# Eval("reconciliation_transactions_id")%>,<%# Eval("amount") %>,0,2)'>
                                                                        <img alt="Click to drop this transaction" src="../images/edit.png" style="border: 0"
                                                                            class="ShowHand" width="20px" height="20px" />
                                                                    </a>
                                                                </ItemTemplate>
                                                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Reconcile" ItemStyle-HorizontalAlign="Center">
                                                                <ItemTemplate>
                                                                    <a onclick='openDialog(<%# Eval("reconciliation_transactions_id") %>,<%# Eval("amount") %>,1,2)'>
                                                                        <img alt="Click to reconcile this transaction" src="../images/recon.jpg" style="border: 0"
                                                                            class="ShowHand" width="20px" height="20px" />
                                                                    </a>
                                                                </ItemTemplate>
                                                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                                            </asp:TemplateField>
                                                        </Columns>
                                                        <RowStyle HorizontalAlign="Left" />
                                                        <EmptyDataTemplate>
                                                            No Rows Found
                                                        </EmptyDataTemplate>
                                                    </asp:GridView>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </ComponentArt:PageView>
                        <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView1" Width="100%" Visible ="false">
                            <table width="100%">
                                <tr>
                                    <td style="vertical-align: top" width="50%">
                                        <table width="100%">
                                            <tr>
                                                <td align="center" valign="top" width="50%" style="background-color: Gray; color: White;
                                                    font-size: medium; font-weight: bold">
                                                    CCMS VS Switch
                                                </td>
                                            </tr>
                                            <tr>
                                                <td valign="top">
                                                    <asp:GridView ID="GridView_CCMSSwitch" runat="server" AutoGenerateColumns="False" AllowSorting="true"
                                                        Width="100%" OnSorting="GridView_CCMSSwitch_Sorting" DataKeyNames="reconciliation_transactions_id"
                                                        RowStyle-HorizontalAlign="center" OnPageIndexChanging="GridView_CCMSSwitch_PageIndexChanging"
                                                        OnRowDataBound ="GridView_OnRowDataBound">
                                                        <AlternatingRowStyle BackColor="#eff0ef" />
                                                        <HeaderStyle CssClass="HeadingCell" HorizontalAlign="Left" ForeColor="black" />
                                                        <RowStyle HorizontalAlign="Center" />
                                                        <Columns>
                                                            <%--<asp:TemplateField HeaderText="Select" HeaderStyle-CssClass="HeadingCell" ItemStyle-HorizontalAlign="Center">
                                                                <ItemTemplate>
                                                                    <asp:CheckBox ID="chkbx" runat="server"></asp:CheckBox>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>--%>
                                                            <asp:BoundField DataField="title" HeaderText="Title" HeaderStyle-CssClass="HeadingCell"
                                                                ItemStyle-HorizontalAlign="Center" SortExpression="title" />
                                                            <asp:BoundField DataField="trxn_datetime" HeaderText="Datetime" HeaderStyle-CssClass="HeadingCell"
                                                                ItemStyle-HorizontalAlign="Center" SortExpression="trxn_datetime"  DataFormatString="{0:dd/MM/yyyy HH:mm:ss}"/>
                                                            <asp:BoundField DataField="PAN" HeaderText="PAN" SortExpression="PAN" ItemStyle-HorizontalAlign="Center"
                                                                HeaderStyle-CssClass="HeadingCell" />
                                                            <asp:BoundField DataField="tsn" HeaderText="TSN" SortExpression="tsn" ItemStyle-HorizontalAlign="Center"
                                                                HeaderStyle-CssClass="HeadingCell" />
                                                            <asp:BoundField DataField="amount" HeaderText="Amount" SortExpression="amount" ItemStyle-HorizontalAlign="Center"
                                                                HeaderStyle-CssClass="HeadingCell" />
                                                            <asp:BoundField DataField="status" HeaderText="Status" SortExpression="status" ItemStyle-HorizontalAlign="Left"
                                                                HeaderStyle-CssClass="HeadingCell" />
                                                            <asp:BoundField DataField="reason" HeaderText="Reason" SortExpression="reason" ItemStyle-HorizontalAlign="Left"
                                                                HeaderStyle-CssClass="HeadingCell" />
                                                            <asp:TemplateField HeaderText="Drop" ItemStyle-HorizontalAlign="Center">
                                                                <ItemTemplate>
                                                                    <a onclick='openDialog(<%# Eval("reconciliation_transactions_id") %>,<%# Eval("amount") %>,0,3)'>
                                                                        <img alt="Click to drop this transaction" src="../images/edit.png" style="border: 0"
                                                                            class="ShowHand" width="20px" height="20px" />
                                                                    </a>
                                                                </ItemTemplate>
                                                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Reconcile" ItemStyle-HorizontalAlign="Center">
                                                                <ItemTemplate>
                                                                    <a onclick='openDialog(<%# Eval("reconciliation_transactions_id") %>,<%# Eval("amount") %>,1,3)'>
                                                                        <img alt="Click to reconcile this transaction" src="../images/recon.jpg" style="border: 0"
                                                                            class="ShowHand" width="20px" height="20px" />
                                                                    </a>
                                                                </ItemTemplate>
                                                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                                            </asp:TemplateField>
                                                        </Columns>
                                                        <RowStyle HorizontalAlign="Left" />
                                                        <EmptyDataTemplate>
                                                            No Rows Found
                                                        </EmptyDataTemplate>
                                                    </asp:GridView>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td style="vertical-align: top">
                                        <table width="100%">
                                            <tr>
                                                <td align="center" valign="top" width="50%" style="background-color: Silver; color: Black;
                                                    font-size: medium; font-weight: bold">
                                                    Switch VS Host
                                                </td>
                                            </tr>
                                            <tr>
                                                <td valign="top">
                                                    <asp:GridView ID="GridView_SwitchHost2" runat="server" AutoGenerateColumns="False"
                                                        AllowSorting="true" Width="100%" OnSorting="GridView_SwitchHost2_Sorting" DataKeyNames="reconciliation_transactions_id"
                                                        RowStyle-HorizontalAlign="center" OnPageIndexChanging="GridView_SwitchHost2_PageIndexChanging"
                                                        OnRowDataBound ="GridView_OnRowDataBound">
                                                        <AlternatingRowStyle BackColor="#eff0ef" />
                                                        <HeaderStyle CssClass="HeadingCell" HorizontalAlign="Left" ForeColor="black" />
                                                        <RowStyle HorizontalAlign="Center" />
                                                        <Columns>
                                                            <%--<asp:TemplateField HeaderText="Select" HeaderStyle-CssClass="HeadingCell" ItemStyle-HorizontalAlign="Center">
                                                                <ItemTemplate>
                                                                    <asp:CheckBox ID="chkbx" runat="server"></asp:CheckBox>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>--%>
                                                            <asp:BoundField DataField="title" HeaderText="Title" HeaderStyle-CssClass="HeadingCell"
                                                                ItemStyle-HorizontalAlign="Center" SortExpression="title" />
                                                            <asp:BoundField DataField="trxn_datetime" HeaderText="Datetime" HeaderStyle-CssClass="HeadingCell"
                                                                ItemStyle-HorizontalAlign="Center" SortExpression="trxn_datetime" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}"/>
                                                            <asp:BoundField DataField="PAN" HeaderText="PAN" SortExpression="PAN" ItemStyle-HorizontalAlign="Center"
                                                                HeaderStyle-CssClass="HeadingCell" />
                                                            <asp:BoundField DataField="tsn" HeaderText="TSN" SortExpression="tsn" ItemStyle-HorizontalAlign="Center"
                                                                HeaderStyle-CssClass="HeadingCell" />
                                                            <asp:BoundField DataField="amount" HeaderText="Amount" SortExpression="amount" ItemStyle-HorizontalAlign="Center"
                                                                HeaderStyle-CssClass="HeadingCell" />
                                                            <asp:BoundField DataField="status" HeaderText="Status" SortExpression="status" ItemStyle-HorizontalAlign="Left"
                                                                HeaderStyle-CssClass="HeadingCell" />
                                                            <asp:BoundField DataField="reason" HeaderText="Reason" SortExpression="reason" ItemStyle-HorizontalAlign="Left"
                                                                HeaderStyle-CssClass="HeadingCell" />
                                                            <asp:TemplateField HeaderText="Drop" ItemStyle-HorizontalAlign="Center">
                                                                <ItemTemplate>
                                                                    <a onclick='openDialog(<%# Eval("reconciliation_transactions_id")%>,<%# Eval("amount") %>,0,2)'>
                                                                        <img alt="Click to drop this transaction" src="../images/edit.png" style="border: 0"
                                                                            class="ShowHand" width="20px" height="20px" />
                                                                    </a>
                                                                </ItemTemplate>
                                                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Reconcile" ItemStyle-HorizontalAlign="Center">
                                                                <ItemTemplate>
                                                                    <a onclick='openDialog(<%# Eval("reconciliation_transactions_id") %>,<%# Eval("amount") %>,1,2)'>
                                                                        <img alt="Click to reconcile this transaction" src="../images/recon.jpg" style="border: 0"
                                                                            class="ShowHand" width="20px" height="20px" />
                                                                    </a>
                                                                </ItemTemplate>
                                                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                                            </asp:TemplateField>
                                                        </Columns>
                                                        <RowStyle HorizontalAlign="Left" />
                                                        <EmptyDataTemplate>
                                                            No Rows Found
                                                        </EmptyDataTemplate>
                                                    </asp:GridView>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </ComponentArt:PageView>
                        <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView2" Width="100%" Visible="false">
                            <table width="100%">
                                <tr>
                                    <td style="vertical-align: top" width="50%">
                                        <table width="100%">
                                            <tr>
                                                <td align="center" valign="top" width="50%" style="background-color: Gray; color: White;
                                                    font-size: medium; font-weight: bold">
                                                    EJ VS CCMS
                                                </td>
                                            </tr>
                                            <tr>
                                                <td valign="top">
                                                    <asp:GridView ID="GridView_EJCCMS" runat="server" AutoGenerateColumns="False" AllowSorting="true"
                                                        Width="100%" OnSorting="GridView_EJCCMS_Sorting" DataKeyNames="reconciliation_transactions_id"
                                                        RowStyle-HorizontalAlign="center" OnPageIndexChanging="GridView_EJCCMS_PageIndexChanging">
                                                        <AlternatingRowStyle BackColor="#eff0ef" />
                                                        <HeaderStyle CssClass="HeadingCell" HorizontalAlign="Left" ForeColor="black" />
                                                        <RowStyle HorizontalAlign="Center" />
                                                        <Columns>
                                                            <%--<asp:TemplateField HeaderText="Select" HeaderStyle-CssClass="HeadingCell" ItemStyle-HorizontalAlign="Center">
                                                                <ItemTemplate>
                                                                    <asp:CheckBox ID="chkbx" runat="server"></asp:CheckBox>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>--%>
                                                            <asp:BoundField DataField="title" HeaderText="Title" HeaderStyle-CssClass="HeadingCell"
                                                                ItemStyle-HorizontalAlign="Center" SortExpression="title" />
                                                            <asp:BoundField DataField="trxn_datetime" HeaderText="Datetime" HeaderStyle-CssClass="HeadingCell"
                                                                ItemStyle-HorizontalAlign="Center" SortExpression="trxn_datetime" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}"/>
                                                            <asp:BoundField DataField="PAN" HeaderText="PAN" SortExpression="PAN" ItemStyle-HorizontalAlign="Center"
                                                                HeaderStyle-CssClass="HeadingCell" />
                                                            <asp:BoundField DataField="tsn" HeaderText="TSN" SortExpression="tsn" ItemStyle-HorizontalAlign="Center"
                                                                HeaderStyle-CssClass="HeadingCell" />
                                                            <asp:BoundField DataField="amount" HeaderText="Amount" SortExpression="amount" ItemStyle-HorizontalAlign="Center"
                                                                HeaderStyle-CssClass="HeadingCell" />
                                                            <asp:BoundField DataField="status" HeaderText="Status" SortExpression="status" ItemStyle-HorizontalAlign="Left"
                                                                HeaderStyle-CssClass="HeadingCell" />
                                                            <asp:BoundField DataField="reason" HeaderText="Reason" SortExpression="reason" ItemStyle-HorizontalAlign="Left"
                                                                HeaderStyle-CssClass="HeadingCell" />
                                                            <%--<asp:TemplateField HeaderText="Drop" ItemStyle-HorizontalAlign="Center">
                                                                <ItemTemplate>
                                                                    <a onclick='openDialog(<%# Eval("reconciliation_transactions_id") %>,<%# Eval("amount") %>,0)'>
                                                                        <img alt="Click to drop this transaction" src="../images/edit.png" style="border: 0"
                                                                            class="ShowHand" width="20px" height="20px" />
                                                                    </a>
                                                                </ItemTemplate>
                                                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Reconcile" ItemStyle-HorizontalAlign="Center">
                                                                <ItemTemplate>
                                                                    <a onclick='openDialog(<%# Eval("reconciliation_transactions_id") %>,<%# Eval("amount") %>,1)'>
                                                                        <img alt="Click to reconcile this transaction" src="../images/recon.jpg" style="border: 0"
                                                                            class="ShowHand" width="20px" height="20px" />
                                                                    </a>
                                                                </ItemTemplate>
                                                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                                                            </asp:TemplateField>--%>
                                                        </Columns>
                                                        <RowStyle HorizontalAlign="Left" />
                                                        <EmptyDataTemplate>
                                                            No Rows Found
                                                        </EmptyDataTemplate>
                                                    </asp:GridView>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    
                                </tr>
                            </table>
                        </ComponentArt:PageView>
                    </ComponentArt:MultiPage>
                </td>
            </tr>
            <tr>
            </tr>
            <tr>
                <td>
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td>
                    <div style="padding-right: 150px;">
                        <%--  <asp:Button ID="Button_Reconcile" runat="server" Text="Reconcile Forcefully" CssClass="button"
                            OnClick="Button_Reconcile_Click" Width="120px" OnClientClick="return confirm('Are you sure you want to reconcile selected transactions')" />&nbsp;--%>
                        <%--<asp:Button ID="Button_Drop" runat="server" Text="Drop" CssClass="button" OnClick="Button_Drop_Click"
                            Width="120px" OnClientClick="return openDialog()" />&nbsp;--%>
                        <input type="button" value="Cancel" onclick="window.location='Reconciliation.aspx'"
                            class="button" />
                    </div>
                </td>
            </tr>
        </table>
        <ComponentArt:Dialog ContentCssClass="contentCss" FooterCssClass="footerCss" HeaderCssClass="headerCss"
            CssClass="dialogCss" Icon="" Value="" Title="AJAX TabStrip Content" CloseTransition="Fade"
            ModalMaskCssClass="modalMaskCssClass" AllowDrag="false" Alignment="MiddleCentre"
            Modal="true" ID="Dialog2" runat="server" Width="430" ContentClientTemplateId="ClientTemplate1"
            AnimationType="None" AnimationPath="Direct" AnimationSlide="Linear" ShowTransition="None"
            TransitionDuration="0" AnimationDuration="0" Height="250">
            <ClientTemplates>
                <ComponentArt:ClientTemplate ID="ClientTemplate1">
                    <table cellpadding="0" style="border-color: #c9c9c9" cellspacing="0" border="0" width="100%"
                        onmousedown="Dialog2.StartDrag(event);">
                        <tr>
                            <td style="background-image: url(../images/DialogImages/top.gif); height: 45px;">
                                <table width="100%" cellpadding="0" cellspacing="0" border="0">
                                    <tr>
                                        <td valign="middle" style="padding-left: 4px; color: white; font-size: 15px; font-family: Verdana;
                                            font-weight: bold;">
                                            Manual Transaction Handling
                                        </td>
                                        <td align="right" style="padding-right: 10px;">
                                            <img src="../images/DialogImages/close.gif" onclick="Dialog2.Close();" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td style="border-color: #c9c9c9; background-color: white; font-size: 12px; font-family: Arial;
                                padding: 10px;">
                                <div id="ATM" style="overflow: auto; height: 270px">
                                    <table>
                                        <tr>
                                            <td valign="top" colspan="2">
                                                Please enter Comments.
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <input type="text" class="textbox" maxlength="1000" style="width: 400px; height: 200px;"
                                                    name="comment" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left">
                                                <input type="button" value="Save" onclick="onCloseOfDialog();" class="button" />
                                                <input type="button" value="Close" class="button" onclick="Dialog2.Close();" />
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </td>
                        </tr>
                    </table>
                </ComponentArt:ClientTemplate>
            </ClientTemplates>
        </ComponentArt:Dialog>
        <ComponentArt:CallBack ID="DialogCallBack" runat="server" Width="0" Height="0" OnCallback="DialogCallBack_Callback">
        </ComponentArt:CallBack>
    </div>
    <input type="button" runat="server" id="refresh" style="display: none" />
    </form>
</body>
</html>
