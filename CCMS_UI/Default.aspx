<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CCMSUI.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">

        function doPostBack() {
            document.getElementById('refresh').click();
        }
        function call() {
            window.setTimeout(doPostBack, 2000);
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:Literal ID="Literal_Script" runat="server"></asp:Literal>
    <div>
    </div>
    <input type="button" runat="server" id="refresh" style="display: none" />
    </form>
</body>
</html>
