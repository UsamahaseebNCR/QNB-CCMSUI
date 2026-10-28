<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BasePage.aspx.cs" Inherits="Avanza.iSuite.UI.BasePage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
    <head runat="server">
    <title>Redirect Example</title>
    <script language="javascript" type="text/javascript">
        function openWindow() {
            var open = window.open('http://forums.asp.net', 'NewWindow', 'left=20,top=20,height=600,width=800,scrollbars=1,resizable=0,toolbar=1,status=yes');
        }
    </script>

</head>
<body onload="setTimeout('openWindow()', 3000)">
    <form id="form1" runat="server">
    <div>
        <asp:Literal ID="Literal1" runat="server"></asp:Literal>
    </div>
    </form>
</body>

<%--<head runat="server">
    
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
    </div>
    </form>
</body>--%>
</html>
