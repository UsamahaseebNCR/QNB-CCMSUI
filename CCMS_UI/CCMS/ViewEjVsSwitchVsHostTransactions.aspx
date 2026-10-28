<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewEjVsSwitchVsHostTransactions.aspx.cs"
    Inherits="CCMSUI.CCMS.ViewEjVsSwitchVsHostTransactions" %>

<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <script src="../content/shortcut.js" type="text/javascript" language="javascript"></script>
    <script src="../content/cal/popcalendar.js" type="text/javascript"></script>

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

        function toggleDisSusOn(statusDropdown) {
            var show = statusDropdown.value === "Disputed" ||
                statusDropdown.value === "Suspected";

            document.getElementById("td_DisSusOnLabel").style.display =
                show ? "" : "none";

            document.getElementById("td_DisSusOn").style.display =
                show ? "" : "none";
        }

        function initializeDisSusOn() {
            var statusDropdown = document.getElementById("<%= DropDownList_TransactionStatus.ClientID %>");

            if (statusDropdown) {
                toggleDisSusOn(statusDropdown);
            }
        }

        function LabelText(labeltext) {
            document.getElementById('Label_errorMsg').innerHTML = "<li>" + labeltext + "</li>";
        }
        function onCloseOfDialog() {
            DialogCallBack.callback(document.getElementById('reconTrxnID').value, document.getElementById('comment').value, document.getElementById('operationType').value, document.getElementById('amount').value, document.getElementById('gridID').value);
            Dialog2.Close();
        }

        function openDialog(recondTrxnID, amount, type, selectedGrid) {
            document.getElementById('reconTrxnID').value = recondTrxnID;
            document.getElementById('operationType').value = type;
            document.getElementById('amount').value = amount;
            document.getElementById('gridID').value = selectedGrid;
            Dialog2.Show();
        }

    </script>
    <link href="../content/iSuite.css" rel="stylesheet" />
    <link href="../content/cal/popcalendar.css" type="text/css" rel="stylesheet" />
    <style type="text/css">
        .custom {
            padding: 5px;
        }

        .customText {
            padding: 5px;
            font-weight: bold;
            font-size: small;
        }
    </style>
    <style type="text/css">
        .style1 {
            height: 17px;
            width: 130px;
        }

        .style3 {
            width: 141px;
        }

        .style4 {
            height: 17px;
            width: 141px;
        }
    </style>
    


</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)'
    onload="hideProgressBar(); initializeDisSusOn();">
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
                    <td class="PageHeading" align="left" colspan="2">Reconciliation Transactions
                    </td>
                </tr>

                <tr>
                    <td class="ValidationText" align="center">
                        <asp:Literal ID="Literal_error" runat="server" EnableViewState="False"></asp:Literal>
                        <div class="ValidationText">
                            <asp:Label ID="Label_errorMsg" runat="server" EnableViewState="False"></asp:Label>
                        </div>
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
                <tr align="left" style="background-color: WHITE">
                    <td style="height: 20px; padding-left: 5px;" colspan="4">
                        <b>Search Filters</b>
                    </td>
                </tr>
                <tr align="left">
                    <td colspan="4">
                        <table width="100%" class="SEARCHBOX" border="0">
                            <tr>
                                <td>
                                    <table id="search" border="0">
                                        <tr id="trTransactionRow" runat="server">
                                            <td class="style4">From :
                                            </td>
                                            <td>
                                                <asp:TextBox runat="server" ID="TextBox_FromDate" Width="119px" CssClass="textbox" />
                                                <img src="../images/IconCalendar.gif" alt="Click to open calender" onclick="popUpCalendar(this,document.getElementById('<%=TextBox_FromDate.ClientID %>'),'dd/mm/yyyy');"
                                                    style="border: 0; cursor: pointer" id="IMG1" />
                                                <asp:RegularExpressionValidator ID="ValFromDate" CssClass="Validator" runat="server"
                                                    ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                                    ControlToValidate="TextBox_FromDate" ErrorMessage="From Date is invalid.">*</asp:RegularExpressionValidator>
                                            </td>
                                            <td class="style1">To :
                                            </td>
                                            <td>
                                                <asp:TextBox runat="server" ID="TextBox_ToDate" Width="119px" CssClass="textbox" />
                                                <img src="../images/IconCalendar.gif" alt="Click to open calender" onclick="popUpCalendar(this,document.getElementById('<%=TextBox_ToDate.ClientID %>'),'dd/mm/yyyy');"
                                                    style="border: 0; cursor: pointer" id="IMG2" />
                                                <asp:RegularExpressionValidator ID="ValToDate" CssClass="Validator" runat="server"
                                                    ValidationExpression="(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d"
                                                    ControlToValidate="TextBox_ToDate" ErrorMessage="To Date is invalid.">*</asp:RegularExpressionValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="4" style="height: 10px;"></td>
                                        </tr>
                                        <tr>
                                            <td style="width: 120px">Transaction Type :</td>
                                            <td>
                                                <asp:DropDownList ID="DropDownList_TransactionType" runat="server"
                                                    CssClass="textbox" Width="144px">
                                                    <asp:ListItem Text="Select Transaction Type" Value="" Selected="True"></asp:ListItem>
                                                    <asp:ListItem Text="Dispense" Value="1"></asp:ListItem>
                                                    <asp:ListItem Text="Deposit" Value="2"></asp:ListItem>
                                                </asp:DropDownList>
                                            </td>

                                            <td style="width: 120px">Transaction Status :</td>
                                            <td>
                                                <asp:DropDownList ID="DropDownList_TransactionStatus" runat="server"
                                                    CssClass="textbox" Width="144px"
                                                    onchange="toggleDisSusOn(this);">
                                                    <asp:ListItem Text="All" Value="All" Selected="True"></asp:ListItem>
                                                    <asp:ListItem Text="Disputed" Value="Disputed"></asp:ListItem>
                                                    <asp:ListItem Text="Suspected" Value="Suspected"></asp:ListItem>
                                                    <asp:ListItem Text="Matched" Value="Matched"></asp:ListItem>
                                                </asp:DropDownList>
                                            </td>

                                            <td id="td_DisSusOnLabel" style="width: 120px; display: none;">Dis/Sus On :
                                            </td>

                                            <td id="td_DisSusOn" style="display: none;">
                                                <asp:DropDownList ID="DropDownList_DisSusOn" runat="server"
                                                    CssClass="textbox" Width="144px">
                                                    <asp:ListItem Text="All" Value="All" Selected="True"></asp:ListItem>
                                                    <asp:ListItem Text="EJ" Value="EJ"></asp:ListItem>
                                                    <asp:ListItem Text="Switch" Value="Switch"></asp:ListItem>
                                                    <asp:ListItem Text="Host" Value="Host"></asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="4" style="height: 10px;"></td>
                                        </tr>
                                        <tr>
                                            <td colspan="5" align="right" style="padding-right: 80px">
                                                <asp:Button CssClass="button" ID="button_search" runat="server" Text="Search" OnClick="button_search_Click" />
                                                &nbsp;<asp:Button ID="Button_Reset" runat="server" CssClass="button" OnClick="Button_Reset_Click"
                                                    Text="Reset" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td valign="top">
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
                <tr align="left">
                    <th colspan="2" align="left">Search Results -
                        <asp:Literal ID="Literal_count" runat="server" EnableViewState="False"></asp:Literal>
                    </th>
                </tr>
                <tr id="trSearchResults" runat="server">
                    <td>

                        <!-- DISPENSE -->
                        <div id="divDispense" runat="server">
                            <table width="100%">
                                <tr>
                                    <td class="PageHeading">EJ Vs Switch Vs Host

                        <span style="float: right;">
                            <asp:Button ID="Button_ExportToExcel"
                                runat="server"
                                CssClass="button"
                                OnClientClick="window.setTimeout(showProgressBar(),1000);"
                                OnClick="Button_ExportToExcel_Click"
                                Text="Export To Excel" />
                        </span>
                                    </td>
                                </tr>

                                <tr>
                                    <td valign="top">

                                        <asp:GridView
                                            ID="GridView_EJSwitch"
                                            runat="server"
                                            AutoGenerateColumns="False"
                                            AllowSorting="true"
                                            Width="100%"
                                            OnSorting="GridView_EJSwitch_Sorting"
                                            DataKeyNames="reconciliation_transactions_id"
                                            RowStyle-HorizontalAlign="center"
                                            OnPageIndexChanging="GridView_EJSwitch_PageIndexChanging"
                                            OnRowDataBound="GridView_OnRowDataBound">

                                            <Columns>
                                                <%--<asp:TemplateField HeaderText="Select" HeaderStyle-CssClass="HeadingCell" ItemStyle-HorizontalAlign="Center">
                                                                <ItemTemplate>
                                                                    <asp:CheckBox ID="chkbx" runat="server"></asp:CheckBox>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>--%>
                                                <asp:BoundField DataField="title" HeaderText="Title"
                                                    ItemStyle-HorizontalAlign="Center" SortExpression="title">
                                                    <HeaderStyle CssClass="HeadingCell" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="trxn_datetime" HeaderText="Datetime" HeaderStyle-CssClass="HeadingCell"
                                                    ItemStyle-HorizontalAlign="Center" SortExpression="trxn_datetime" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" />
                                                <asp:BoundField DataField="PAN" HeaderText="PAN" SortExpression="PAN" ItemStyle-HorizontalAlign="Center"
                                                    HeaderStyle-CssClass="HeadingCell" />
                                                <asp:BoundField DataField="tsn" HeaderText="TSN" SortExpression="tsn" ItemStyle-HorizontalAlign="Center"
                                                    HeaderStyle-CssClass="HeadingCell" />


                                                <asp:BoundField DataField="ej_amount"
                                                    HeaderText="EJ Amount"
                                                    SortExpression="ej_amount"
                                                    ItemStyle-HorizontalAlign="Center"
                                                    HeaderStyle-CssClass="HeadingCell"
                                                    DataFormatString="{0:N3}" />

                                                <asp:BoundField DataField="switch_amount"
                                                    HeaderText="Switch Amount"
                                                    SortExpression="switch_amount"
                                                    ItemStyle-HorizontalAlign="Center"
                                                    HeaderStyle-CssClass="HeadingCell"
                                                    DataFormatString="{0:N3}" />

                                                <asp:BoundField DataField="host_amount"
                                                    HeaderText="Host Amount"
                                                    SortExpression="host_amount"
                                                    ItemStyle-HorizontalAlign="Center"
                                                    HeaderStyle-CssClass="HeadingCell"
                                                    DataFormatString="{0:N3}" />


                                                <asp:BoundField DataField="status_on_ej" HeaderText="Status on EJ" ItemStyle-HorizontalAlign="Center"
                                                    HeaderStyle-CssClass="HeadingCell" />

                                                <asp:BoundField DataField="status_on_switch" HeaderText="Status on SWITCH" SortExpression="status" ItemStyle-HorizontalAlign="Center"
                                                    HeaderStyle-CssClass="HeadingCell" />

                                                <asp:BoundField DataField="status_on_host" HeaderText="Status on HOST" SortExpression="status" ItemStyle-HorizontalAlign="Center"
                                                    HeaderStyle-CssClass="HeadingCell" />

                                                <asp:BoundField DataField="status" HeaderText="Reconciliation Status" SortExpression="status" ItemStyle-HorizontalAlign="Center"
                                                    HeaderStyle-CssClass="HeadingCell" />


                                                <asp:BoundField DataField="reason" HeaderText="Reason" SortExpression="reason" ItemStyle-HorizontalAlign="Center"
                                                    HeaderStyle-CssClass="HeadingCell" />

                                                <asp:BoundField DataField="accountno" HeaderText="Customer Account" SortExpression="accountno" ItemStyle-HorizontalAlign="Center"
                                                    HeaderStyle-CssClass="HeadingCell" />


                                            </Columns>
                                            <EmptyDataTemplate>
                                                No Rows Found
                                            </EmptyDataTemplate>

                                        </asp:GridView>

                                    </td>
                                </tr>
                            </table>
                        </div>
                        <!-- DEPOSIT / BNA -->
                        <div id="divDeposit" runat="server">
                            <table width="100%">
                                <tr>
                                    <td class="PageHeading">EJ BNA Vs Switch Vs Host

                        <span style="float: right;">
                            <asp:Button ID="Button_ExportToExcelBNA"
                                runat="server"
                                CssClass="button"
                                OnClientClick="window.setTimeout(showProgressBar(),1000);"
                                OnClick="Button_ExportToExcelBNA_Click"
                                Text="Export To Excel" />
                        </span>
                                    </td>
                                </tr>

                                <tr>
                                    <td valign="top">

                                        <asp:GridView
                                            ID="GridView_EJBNASwitch"
                                            runat="server"
                                            AutoGenerateColumns="False"
                                            AllowSorting="true"
                                            Width="100%"
                                            OnSorting="GridView_EJBNASwitch_Sorting"
                                            DataKeyNames="reconciliation_transactions_id"
                                            RowStyle-HorizontalAlign="center"
                                            OnPageIndexChanging="GridView_EJBNASwitch_PageIndexChanging"
                                            OnRowDataBound="GridView_OnRowDataBound">

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
                                                <asp:BoundField DataField="seq" HeaderText="SEQ" SortExpression="seq" ItemStyle-HorizontalAlign="Center"
                                                    HeaderStyle-CssClass="HeadingCell" />


                                                <asp:BoundField DataField="ej_amount"
                                                    HeaderText="EJ Amount"
                                                    SortExpression="ej_amount"
                                                    ItemStyle-HorizontalAlign="Center"
                                                    HeaderStyle-CssClass="HeadingCell"
                                                    DataFormatString="{0:N3}" />

                                                <asp:BoundField DataField="switch_amount"
                                                    HeaderText="Switch Amount"
                                                    SortExpression="switch_amount"
                                                    ItemStyle-HorizontalAlign="Center"
                                                    HeaderStyle-CssClass="HeadingCell"
                                                    DataFormatString="{0:N3}" />

                                                <asp:BoundField DataField="host_amount"
                                                    HeaderText="Host Amount"
                                                    SortExpression="host_amount"
                                                    ItemStyle-HorizontalAlign="Center"
                                                    HeaderStyle-CssClass="HeadingCell"
                                                    DataFormatString="{0:N3}" />


                                                <asp:BoundField DataField="status_on_ej" HeaderText="Status on EJ" ItemStyle-HorizontalAlign="Center"
                                                    HeaderStyle-CssClass="HeadingCell" />
                                                <asp:BoundField DataField="status_on_switch" HeaderText="Status on SWITCH" ItemStyle-HorizontalAlign="Center"
                                                    HeaderStyle-CssClass="HeadingCell" />
                                                <asp:BoundField DataField="status_on_host" HeaderText="Status on HOST" ItemStyle-HorizontalAlign="Center"
                                                    HeaderStyle-CssClass="HeadingCell" />


                                                <asp:BoundField DataField="status" HeaderText="Reconciliation Status" SortExpression="status" ItemStyle-HorizontalAlign="Center"
                                                    HeaderStyle-CssClass="HeadingCell" />
                                                <asp:BoundField DataField="reason" HeaderText="Reason" SortExpression="reason" ItemStyle-HorizontalAlign="Center"
                                                    HeaderStyle-CssClass="HeadingCell" />
                                                <%--<asp:TemplateField HeaderText="Drop" ItemStyle-HorizontalAlign="Center">
                                                                    <ItemTemplate>
                                                                        <a onclick='openDialog(<%# Eval("reconciliation_transactions_id") %>,<%# Eval("amount") %>,0,1)'>
                                                                            <img alt="Click to drop this transaction" src="../images/edit.png" style="border: 0"
                                                                                class="ShowHand" width="20px" height="20px" />
                                                                        </a>
                                                                    </ItemTemplate>
                                                                    <HeaderStyle CssClass="HeadingCell"  />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Reconcile" ItemStyle-HorizontalAlign="Center">
                                                                    <ItemTemplate>
                                                                        <a onclick='openDialog(<%# Eval("reconciliation_transactions_id") %>,<%# Eval("amount") %>,1,1)'>
                                                                            <img alt="Click to reconcile this transaction" src="../images/recon.jpg" style="border: 0"
                                                                                class="ShowHand" width="20px" height="20px" />
                                                                        </a>
                                                                    </ItemTemplate>
                                                                    <HeaderStyle CssClass="HeadingCell"  />
                                                                </asp:TemplateField>--%>

                                                <asp:BoundField DataField="accountno" HeaderText="Customer Account" SortExpression="accountno" ItemStyle-HorizontalAlign="Center"
                                                    HeaderStyle-CssClass="HeadingCell" />
                                            </Columns>
                                            <EmptyDataTemplate>
                                                No Rows Found
                                            </EmptyDataTemplate>

                                        </asp:GridView>

                                    </td>
                                </tr>
                            </table>
                        </div>
                    </td>
                </tr>
                <tr>
                </tr>
                <tr>
                    <td>&nbsp;
                    </td>
                </tr>
                <tr>
                    <td>
                        <div style="padding-right: 150px;">
                            <%--  <asp:Button ID="Button_Reconcile" runat="server" Text="Reconcile Forcefully" CssClass="button" onClientClick="window.setTimeout(showProgressBar(),1000);"
                            OnClick="Button_Reconcile_Click" Width="120px" OnClientClick="return confirm('Are you sure you want to reconcile selected transactions')" />&nbsp;--%>
                            <%--<asp:Button ID="Button_Drop" runat="server" Text="Drop" CssClass="button" onClientClick="window.setTimeout(showProgressBar(),1000);" OnClick="Button_Drop_Click"
                            Width="120px" OnClientClick="return openDialog()" />&nbsp;--%>
                            <%--<input type="button" value="Cancel" onclick="window.location = 'Reconciliation.aspx'"
                                class="button" />--%>
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
                                            <td valign="middle" style="padding-left: 4px; color: white; font-size: 15px; font-family: Verdana; font-weight: bold;">Manual Transaction Handling
                                            </td>
                                            <td align="right" style="padding-right: 10px;">
                                                <img src="../images/DialogImages/close.gif" onclick="Dialog2.Close();" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td style="border-color: #c9c9c9; background-color: white; font-size: 12px; font-family: Arial; padding: 10px;">
                                    <div id="ATM" style="overflow: auto; height: 270px">
                                        <table>
                                            <tr>
                                                <td valign="top" colspan="2">Please enter Comments.
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
