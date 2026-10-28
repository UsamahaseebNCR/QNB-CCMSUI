<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Web2SQL.aspx.cs" Inherits="CCMSUI.CCMS.Web2SQL" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="content/styleSheets/bootstrap.min.css" rel="stylesheet">
    <link href="content/styleSheets/bootstrap-theme.min.css" rel="stylesheet">
   
    <script type="text/javascript">
        function clearQueryTextBox() {
            document.getElementById('<%=Textbox_SQL.ClientID %>').value = '';
        }

        function checkForAsterik() {
            if (document.getElementById('<%=Textbox_SQL.ClientID %>').value.indexOf('*') > 0) {
                var r = confirm('Are you select to execute query to get all records');
                if (r == 1)
                    return true;
                else
                    return false;

            } 
        }
    </script>
    <title></title>
</head>
<body role="document">
    
    <form id="form1" runat="server">
   
    <div class="navbar navbar-inverse navbar-fixed-top" role="navigation">
        <div class="container">
            <div class="navbar-header">
                <span style="color: white; font-size: xx-large" class="glyphicon glyphicon-check">WEB2SQL</span>
            </div>
        </div>
    </div>
     <div style="color: red; margin-top:50px">
        <asp:Literal ID="Literal_err" runat="server" ></asp:Literal>
    </div>
    <div  >
        <asp:Literal ID="Literal_ok" runat="server"></asp:Literal>
    </div>
    <div >
        Executed Queries
        <asp:ListBox ID="ListBox_ExcecutedQueries" runat="server" Width="100%" 
             Height="156px" ></asp:ListBox>
            <asp:Button ID="Button_ReExecute" Text="ReExecute" runat="server" 
            onclick="Button_ReExecute_Click" CssClass ="btn-danger" />
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
