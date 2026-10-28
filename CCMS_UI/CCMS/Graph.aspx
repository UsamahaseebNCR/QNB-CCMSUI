<%@ Page Language="C#" AutoEventWireup="true" Codebehind="Graph.aspx.cs" Inherits="iJournalUI.Graph" %>

<%@ Register TagPrefix="ofc" Namespace="OpenFlashChart" Assembly="OpenFlashChart" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Graph List</title>
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
    <link href="../content/iSuite.css" rel="stylesheet" />
</head>
<body onload="hideProgressBar();">
    <form id="form1" runat="server">
        <div>
            <center>
            <table  style="margin-top : 40px;">
                <tr>
                    <td>
                        <asp:Literal ID="Literal_Message" runat="server" ></asp:Literal>
                        <asp:Literal ID="Literal_Title" runat="server" ></asp:Literal>
                        
                    </td>
                </tr>
                <tr>
                    <td>
                        <ofc:Chart ID="Chart_ShowGraph" Height="320" Width="900" runat="server" 
                           />
                    </td>
                   
                </tr>
               <tr>
                    <td align="left">
                        <input type="button"  value="Back to graph list" class="Button"
                         onclick ="history.go(-1);" />
                    </td>
                   
                </tr>
            </table>
            </center>
        </div>
    </form>
</body>
</html>
