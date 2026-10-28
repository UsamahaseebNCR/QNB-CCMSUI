<%@ Page AutoEventWireup="true" CodeBehind="ReportBuilder.aspx.cs" Inherits="CCMSUI.CCMS.ReportBuilder" EnableViewState="false" 
    Language="C#" %>

<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
    <title>Report Builder</title>
    <link href="../content/iSuite.css" rel="stylesheet" />

    <link href="//cdn.datatables.net/1.10.16/css/jquery.dataTables.min.css" rel="stylesheet" />
    <script src="//cdn.datatables.net/1.10.16/js/jquery.dataTables.min.js" type="text/javascript" language="javascript"></script>
</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)'
    onload="hideProgressBar();">
    <form id="form1" runat="server">
     <div style="color: red; margin-top:20px">
        <asp:Literal ID="Literal_err" runat="server" ></asp:Literal>
    </div>
    <div>
        Query
        <asp:TextBox ID="Textbox_SQL"  runat="server" Height="167px" Width="100%" 
            TextMode="MultiLine"></asp:TextBox><br />
        
        Timeout<asp:TextBox ID="TextBox_Timeout" runat="server" Text="30"></asp:TextBox>
        
        <asp:Button ID="Button_Execute" runat="server" Text="Execute" OnClick="Button_Execute_Click" OnClientClick="return checkForAsterik();" CssClass="btn-success" />
        <input type="button" value="Clear" onclick="clearQueryTextBox();" />
    </div>
    <div style="width:1330px; overflow:auto">
    <asp:GridView ID="GridView_Result" runat="server" CellPadding="3" BackColor="White" 
            BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px">
        <FooterStyle BackColor="White" ForeColor="#000066" />
        <HeaderStyle BackColor="#006699" Font-Bold="True" ForeColor="White" />
        <PagerStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Left" />
        <RowStyle ForeColor="#000066" />
        <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" />
        <SortedAscendingCellStyle BackColor="#F1F1F1" />
        <SortedAscendingHeaderStyle BackColor="#007DBB" />
        <SortedDescendingCellStyle BackColor="#CAC9C9" />
        <SortedDescendingHeaderStyle BackColor="#00547E" />
        <EmptyDataTemplate>
            No Rows Found
        </EmptyDataTemplate>
    </asp:GridView>
    </div>
    </form>
</body>
</html>
