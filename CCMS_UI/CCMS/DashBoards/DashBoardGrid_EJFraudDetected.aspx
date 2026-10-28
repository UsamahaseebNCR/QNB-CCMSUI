<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DashBoardGrid_EJFraudDetected.aspx.cs"
    Inherits="CCMSUI.CCMS.DashBoards.DashBoardGrid_EJFraudDetected" %>

<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>
        <asp:Literal ID="Literal_AddHeading" runat="server"></asp:Literal>
    </title>
    <meta http-equiv="Refresh" content="300" id="metaRefresh" runat="server" />
    <script src="../../content/boxover.js" type="text/javascript" language="javascript">
    </script>
    <link href="../../content/iSuite.css" rel="stylesheet" />
    <link href="../../content/cal/popcalendar.css" type="text/css" rel="stylesheet" />
    <script src="../../content/boxover.js" type="text/javascript" language="javascript">
    </script>
    <script language="javascript" src="../../content/cal/popcalendar.js" type="text/javascript"></script>
    <script type="text/javascript">

        
        //Added by Ali Shah on 28th Sep, 2015
        //To provide feature of Customer Detail Button

        function openDialogCustomerDetail(capturedTrxnID) {
            //

            document.getElementById('trxnID').value = capturedTrxnID;
            // alert('d');
            DialogCallBackCustomerDetail.callback(capturedTrxnID);
            DialogCustomerDetail.Show();
            // alert('d1');

        }

        function receiveCallBackResult(response) {
            document.getElementById('Customer').innerHTML= response;
        }
        //By Ali Shah

//        function onCloseOfDialog() {
//            DialogCallBack.callback(document.getElementById('trxnID').value, document.getElementById('txtUsrComment').value, document.getElementById("ddlStatus").options[document.getElementById("ddlStatus").selectedIndex].text, document.getElementById('txtAmount').value);
//            Dialog2.Close();

        //        }
        function onCloseOfDialog() {
          //  alert(document.getElementById("ddlStatus").options[document.getElementById("ddlStatus").selectedIndex].text);
            CallBack1.callback(true, document.getElementById('trxnID').value, document.getElementById('txtUsrComment').value, document.getElementById("ddlStatus").options[document.getElementById("ddlStatus").selectedIndex].text, document.getElementById('txtAmountClaimed').value, document.getElementById('txtAmountCredited').value, false);
            Dialog1.Close();

        }

        function onClose() {
            CallBack1.callback(true, document.getElementById('trxnID').value, document.getElementById('txtUsrComment').value, document.getElementById("ddlStatus").options[document.getElementById("ddlStatus").selectedIndex].text, document.getElementById('txtAmountClaimed').value, document.getElementById('txtAmountCredited').value, true);
            document.getElementById('ddlStatus').style.display = 'none';
            Dialog1.Close();
        }
        //By Ali Shah
        //edit by shariq on 28oct15
        function receiveCallBackExceptionFormResult(response) {
            document.getElementById('Div1').innerHTML = response;

        }
        //function edited by shariq on 26aug15
//        function openDialog(capturedTrxnID,ruleid) {
//            //
//            document.getElementById('trxnID').value = capturedTrxnID;
//            // alert('d');
//            Dialog2.Show();
//            // alert('d1');//,<%# Eval("transaction_rule_id") %>
//            var select = document.getElementById('ddlStatus');
//            document.getElementById('ddlStatus').style.display = 'block';
//            var a = document.getElementById('AmountRow');

//            if (ruleid == 5) {
//                a.style.display = "none";
//               option = document.createElement('option');
//               option.value = "0";
//               option.text = "ATM Malfunction – Card to be returned";
//               select.add(option);
//               option = document.createElement('option');
//               option.value = "1";
//               option.text = "ATM Malfunction – Card to be replaced";
//               select.add(option);

//                
//                
//             }
//           else {
//              // a.style.display = "block";
//                option = document.createElement('option');
//                option.value = "0";
//                option.text = "Resolved – Under the limit – Validated Notes";
//                select.add(option);
//                option = document.createElement('option');
//                option.value = "1";
//                option.text = "Resolved – Under the limit – Notes Not Validated";
//                select.add(option);
//                option = document.createElement('option');
//                option.value = "2";
//                option.text = "Pending – Under the limit – Claim Mismatch";
//                select.add(option);
//                option = document.createElement('option');
//                option.value = "3";
//                option.text = "Resolved – Over the limit – Validated Notes";
//                select.add(option);
//                option = document.createElement('option');
//                option.value = "4";
//                option.text = "Pending – Over the limit – Notes Not Validated";
//                select.add(option);
//                option = document.createElement('option');
//                option.value = "5";
//                option.text = "Pending - Further Investigation Required by IT";
//                select.add(option);
//         
//             
//             }
        //        }




        //******

        function openDialog(capturedTrxnID, ruleid) {
            document.getElementById('trxnID').value = capturedTrxnID;
            CallBack1.callback(false, capturedTrxnID);
            Dialog1.Show();
         
        }
        function ShowDialog(title, fileStatus) {
            Dialog_ShowListOfFiles.Show();
            DialogCallBack_ShowListOfFiles.callback(title, fileStatus);
        }

        var mins;
        var secs;

        function cd(minutes) {
            mins = 1 * m(minutes); //change minutes here
            secs = 0 + s(":01");   //change seconds here (always add an additional second to your total)
            redo();
        }

        function m(obj) {
            for (var i = 0; i < obj.length; i++) {
                if (obj.substring(i, i + 1) == ":")
                    break;
            }

            return (obj.substring(0, i));
        }

        function s(obj) {
            for (var i = 0; i < obj.length; i++) {
                if (obj.substring(i, i + 1) == ":")
                    break;
            }

            return (obj.substring(i + 1, obj.length));
        }

        function dis(mins, secs) {
            var disp;
            if (mins <= 9) {
                disp = " 0";
            }
            else {
                disp = " ";
            }

            disp += mins + ":";
            if (secs <= 9) {
                disp += "0" + secs;
            }
            else {
                disp += secs;
            }

            return (disp);
        }

        function redo() {
            secs--;
            if (secs == -1) {
                secs = 59;
                mins--;
            }

            window.status = "Count Down " + dis(mins, secs); // setup additional displays here.

            if ((mins == 0) && (secs == 0)) {
            }
            else {
                cd = setTimeout("redo()", 1000);
            }
        }

        function ToggleOptionView() {
            var optionsPanel = document.getElementById('optionsPanel');

            if (optionsPanel.style.display == 'none') {
                optionsPanel.style.display = 'block';
                document.getElementById('viewButton').value = 'Hide Options';
            }
            else {
                optionsPanel.style.display = 'none';
                document.getElementById('viewButton').value = 'Show Options';
            }
        }
    </script>
</head>
<body style="margin: 0; background-image: url(../../images/dashboard_bg.jpg)">
    <form id="form1" runat="server">
    <input type="hidden" id="trxnID" />
    <div>
        <asp:Literal ID="Literal_errorMsg" runat="server" EnableViewState="false"></asp:Literal>
        <table width="100%" cellpadding="0" cellspacing="0">
            <tr>
                <td style="vertical-align: top">
                    <asp:GridView runat="server" ID="GridView_Trxn" AutoGenerateColumns="False" BorderStyle="Solid"
                        BorderWidth="1px" CellPadding="3" Width="100%" AllowSorting="true" OnSorting="GridView_Trxn_Sorting"
                      OnRowDataBound="GridView_Trxn_RowDataBound"  >
                        <EmptyDataTemplate>
                            <center>
                                No Records Found</center>
                        </EmptyDataTemplate>
                        <RowStyle HorizontalAlign="Center" />
                        <Columns>
                            <asp:TemplateField >
                                <ItemTemplate>
                               
                          
                                </ItemTemplate>
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:TemplateField>
                            <asp:BoundField DataField="title" HeaderText="ATM" ReadOnly="True" SortExpression="title"
                                ItemStyle-HorizontalAlign="Center">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="bank_name" HeaderText="Bank" ReadOnly="True" SortExpression="bank_name"
                                ItemStyle-HorizontalAlign="Center">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="transaction_rule_friendly_name" HeaderText="Rule" SortExpression="transaction_rule_friendly_name"
                                ItemStyle-HorizontalAlign="Center">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="trxn_datetime" HeaderText="DateTime" SortExpression="trxn_datetime"
                                DataFormatString="{0:dd/MM/yy HH:mm:ss}" HtmlEncode="false">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="pan" HeaderText="PAN" ReadOnly="True" SortExpression="pan"
                                ItemStyle-HorizontalAlign="Center">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="tsn" HeaderText="TSN" ReadOnly="True" SortExpression="tsn"
                                ItemStyle-HorizontalAlign="Center">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="host_tsn" HeaderText="Host TSN" ReadOnly="True" SortExpression="host_tsn"
                                ItemStyle-HorizontalAlign="Center">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="amount" HeaderText="Amount" SortExpression="amount">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>


                            <asp:BoundField DataField="account_no" HeaderText="Account No" SortExpression="account_no">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>

                           <%-- <asp:BoundField DataField="notes_dispensed_type1" HeaderText="Dispensed 1" SortExpression="notes_dispensed_type1" Visible ="false">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="notes_dispensed_type2" HeaderText="Dispensed 2" SortExpression="notes_dispensed_type2" Visible ="false">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="notes_dispensed_type3" HeaderText="Dispensed 3" SortExpression="notes_dispensed_type3" Visible ="false">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:BoundField DataField="notes_dispensed_type4" HeaderText="Dispensed 4" SortExpression="notes_dispensed_type4" Visible ="false">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>--%>
                           <%-- <asp:BoundField DataField="transaction_type_name" HeaderText="Transaction Type" SortExpression="transaction_type_name"
                                ItemStyle-HorizontalAlign="Left">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>--%>
                            <asp:BoundField DataField="status" HeaderText="Transaction Status" SortExpression="status" ItemStyle-HorizontalAlign="Left">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>

                            

                            <asp:BoundField DataField="customer_category" HeaderText="Customer Category" SortExpression="customer_category" ItemStyle-HorizontalAlign="Left">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>


                            <asp:BoundField DataField="comment_text" HeaderText="Failure Reason" SortExpression="comment_text"
                                ItemStyle-HorizontalAlign="Left">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                            <asp:TemplateField HeaderText="View">
                                <ItemTemplate>
                                    <a onclick='window.open("../ViewTransactionChunk.aspx?id=<%# Eval("captured_transactions_id") %>&taskID=<%# Eval("task_id") %>","","location=0,status=1,resizable=yes,scrollbars=yes")'>
                                        <img alt="Click to edit atm" src="../../images/viewfile.jpg" style="border: 0" class="ShowHand"
                                            width="20px" height="20px" />
                                    </a>
                                </ItemTemplate>
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Close">
                                <ItemTemplate>
                                    <a onclick='openDialog(<%# Eval("captured_transactions_id") %>,<%# Eval("transaction_rule_id") %>)'>
                                        <img alt="Click to expire this transaction" src="../../images/edit.png" style="border: 0"
                                            class="ShowHand" width="20px" height="20px" />
                                    </a>
                                </ItemTemplate>
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Customer Inquiry">
                                <ItemTemplate>
                                    
                                 <asp:ImageButton runat="server" ID="anchorInquiry"  ImageUrl ="../../images/Edit.png"  Width="20px" Height="20px"    />
                                    <%--<a onclick='openDialogCustomerDetail(<%# Eval("captured_transactions_id") %>)'>
                                        <img alt="Click to view Customer detail" src="../../images/edit.png" style="border: 0"
                                            class="ShowHand" width="20px" height="20px" />
                                    </a>--%>
                                
                                </ItemTemplate>
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:TemplateField>

                            <asp:BoundField DataField="smsStatus" HeaderText="Alert Status"  ItemStyle-HorizontalAlign="Left">
                                <HeaderStyle CssClass="HeadingCell" ForeColor="black" />
                            </asp:BoundField>
                        </Columns>
                        <RowStyle HorizontalAlign="Center" />
                        <%--<FooterStyle BackColor="#CCCCCC" />--%>
                        <SelectedRowStyle BackColor="#000099" Font-Bold="True" ForeColor="White" />
                        <%--<PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Left" />--%>
                        <HeaderStyle Font-Bold="True" ForeColor="White" />
                        <AlternatingRowStyle BackColor="#eff0ef" />
                    </asp:GridView>
                </td>
            </tr>
        </table>
        <ComponentArt:Dialog ContentCssClass="contentCss" FooterCssClass="footerCss" HeaderCssClass="headerCss"
            CssClass="dialogCss" Icon="" Value="" Title="AJAX TabStrip Content" ModalMaskCssClass="modalMaskCssClass"
            AllowDrag="false" Alignment="MiddleCentre" Modal="false" ID="Dialog2" runat="server"
            Width="430" ContentClientTemplateId="ClientTemplate1" AnimationType="None" ShowTransition="None"
            TransitionDuration="0" AnimationDuration="0" Height="250">
            <ClientTemplates>
                <ComponentArt:ClientTemplate ID="ClientTemplate1">
                    <table cellpadding="0" style="border-color: #c9c9c9" cellspacing="0" border="0" width="100%"
                        onmousedown="Dialog2.StartDrag(event);">
                        <tr>
                            <td style="background-image: url(../../images/DialogImages/top.gif); height: 45px;">
                                <table width="100%" cellpadding="0" cellspacing="0" border="0">
                                    <tr>
                                        <td valign="middle" style="padding-left: 4px; color: white; font-size: 15px; font-family: Verdana;
                                            font-weight: bold;">
                                            Exception Handling Closure Form
                                        </td>
                                        <td align="right" style="padding-right: 10px;">
                                            <img src="../../images/DialogImages/close.gif" onclick="document.getElementById('ddlStatus').style.display = 'none';Dialog2.Close();" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td style="border-color: #c9c9c9; background-color: white; font-size: 12px; font-family: Arial;
                                padding: 10px;" colspan="2">
                                <div id="ATM" style="overflow: auto; height: 270px">
                                    <table>
                                       
                                        <tr>
                                            <td>
                                                Status
                                            </td>
                                            <td>
                                          
                                                <select id="ddlStatus" style="display:none;">
                                                    
                                                </select>
                                            </td>
                                        </tr>
                                       
                                        <tr id = "AmountRow">
                                            <td>
                                            Amount
                                            </td>
                                            <td>
                                            <input id="txtAmount" type="text" />
                                            </td>
                                        </tr>
                                       <tr>
                                            <td valign="top" colspan="2">
                                                Please enter Comments to close this issue.
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                <textarea rows="10" cols="50" id="txtUsrComment"></textarea>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" colspan="2">
                                                <input type="button" value="Save" onclick="onCloseOfDialog();" class="button" />
                                                <input type="button" value="Close" class="button" onclick="document.getElementById('ddlStatus').style.display = 'none';Dialog2.Close();" />
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
        <%--<ComponentArt:CallBack ID="DialogCallBack" runat="server" Width="0" Height="0" OnCallback="DialogCallBack_Callback">
        </ComponentArt:CallBack>
--%>


         <ComponentArt:Dialog ContentCssClass="contentCss" FooterCssClass="footerCss" HeaderCssClass="headerCss"
            CssClass="dialogCss" Icon="" Value="" Title="AJAX TabStrip Content" ModalMaskCssClass="modalMaskCssClass"
            AllowDrag="true" Alignment="MiddleCentre" Modal="false" ID="DialogCustomerDetail" runat="server"
            Width="430" ContentClientTemplateId="ClientTemplateCustomerDetail" AnimationType="None" ShowTransition="None"
            TransitionDuration="0" AnimationDuration="0" Height="250">
            <ClientTemplates>
                <ComponentArt:ClientTemplate ID="ClientTemplateCustomerDetail">
                    <table cellpadding="0" style="border-color: #c9c9c9" cellspacing="0" border="0" width="100%"
                        onmousedown="DialogCustomerDetail.StartDrag(event);">
                        <tr>
                            <td style="background-image: url(../../images/DialogImages/top.gif); height: 45px;">
                                <table width="100%" cellpadding="0" cellspacing="0" border="0">
                                    <tr>
                                        <td valign="middle" style="padding-left: 4px; color: white; font-size: 15px; font-family: Verdana;
                                            font-weight: bold;">
                                            SMS Customer Detail
                                        </td>
                                        <td align="right" style="padding-right: 10px;">
                                            <img src="../../images/DialogImages/close.gif" onclick="document.getElementById('ddlStatus').style.display = 'none';DialogCustomerDetail.Close();" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td style="border-color: #c9c9c9; background-color: white; font-size: 12px; font-family: Arial;
                                padding: 10px;" colspan="2">
                                <div id="Customer" style="overflow: auto; height: 270px">
                                    
                                </div>
                            </td>
                        </tr>
                    </table>
                </ComponentArt:ClientTemplate>
            </ClientTemplates>
        </ComponentArt:Dialog>
        <ComponentArt:CallBack ID="DialogCallBackCustomerDetail" runat="server" Width="0" Height="0" OnCallback="DialogCallBackCustomerDetail_Callback">
        </ComponentArt:CallBack>



         <ComponentArt:Dialog ContentCssClass="contentCss" FooterCssClass="footerCss" HeaderCssClass="headerCss"
            CssClass="dialogCss" Icon="" Value="" Title="AJAX TabStrip Content" ModalMaskCssClass="modalMaskCssClass"
            AllowDrag="false" Alignment="MiddleCentre" Modal="true" ID="Dialog1" runat="server"
            Width="430" ContentClientTemplateId="ClientTemplate2" AnimationType="None" ShowTransition="None"
            TransitionDuration="0" AnimationDuration="0" Height="250">
            <ClientTemplates>
                <ComponentArt:ClientTemplate ID="ClientTemplate2">
                    <table cellpadding="0" style="border-color: #c9c9c9" cellspacing="0" border="0" width="100%"
                        onmousedown="Dialog1.StartDrag(event);">
                        <tr>
                            <td style="background-image: url(../../images/DialogImages/top.gif); height: 45px;">
                                <table width="100%" cellpadding="0" cellspacing="0" border="0">
                                    <tr>
                                        <td valign="middle" style="padding-left: 4px; color: white; font-size: 15px; font-family: Verdana;
                                            font-weight: bold;">
                                            Exception Handling Closure Form
                                        </td>
                                        <td align="right" style="padding-right: 10px;">
                                            <img src="../../images/DialogImages/close.gif" onclick="document.getElementById('ddlStatus').style.display = 'none';Dialog1.Close();" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td style="border-color: #c9c9c9; background-color: white; font-size: 12px; font-family: Arial;
                                padding: 10px;" colspan="2">
                                <div id="Div1" style="overflow: auto; height: 270px">
                                    
                                </div>
                            </td>
                        </tr>
                    </table>
                </ComponentArt:ClientTemplate>
            </ClientTemplates>
        </ComponentArt:Dialog>
        <ComponentArt:CallBack ID="CallBack1" runat="server" Width="0" Height="0" OnCallback="CallBack1_Callback">
        </ComponentArt:CallBack>



    </div>
    </form>
</body>
</html>
