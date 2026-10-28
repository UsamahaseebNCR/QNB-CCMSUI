<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Graph.aspx.cs" Inherits="CCMSUI.Graph" %>

<%@ Register TagPrefix="ofc" Namespace="OpenFlashChart" Assembly="OpenFlashChart" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Graph List</title>
    <link href="content/iSuite.css" rel="stylesheet" />
     <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
</head>
<body onload="hideProgressBar();">
    <form id="form1" runat="server">
    <div>
        <table style="width: 100%; text-align:center" border="0" >
            <tr>
                <td>
                    <asp:Literal ID="Literal_Message" runat="server"></asp:Literal>
                    <asp:Literal ID="Literal_Title" runat="server"></asp:Literal>
                </td>
            </tr>
            <tr>
                <td colspan="3">
                    <table style="background-color:InfoBackground; width:100%">
                        <tr>
                            <td style="width:23px">
                                From
                            </td>
                            <td style="width:23px">
                                <asp:TextBox ID="TextBox_From" runat="server"></asp:TextBox>
                            </td>
                            <td style="width:23px">
                                To
                            </td>
                            <td style="width:23px">
                                <asp:TextBox ID="TextBox_To" runat="server"></asp:TextBox>
                            </td>
                            <td align="left">
                                <asp:Button ID="Button_Submit" runat="server" Text="Generate" OnClick="Button_Submit_Click" />
                            </td>
                        </tr>
                        
                    </table>
                </td>
            </tr>
            <tr>
                <td colspan="3">
                    <hr  />
                </td>
            </tr>
            <tr>
                <td valign="top">
                    <table border="0" width="100%">
                        <tr>
                            <td style="text-align: center">
                                <asp:Literal ID="Literal1" runat="server"></asp:Literal>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <ofc:Chart ID="Chart_ShowGraph" Height="460" Width="330px" runat="server" />
                            </td>
                        </tr>
                    </table>
                </td>
                <td>
                    <table border="0" width="100%">
                        <tr>
                            <td>
                                <table border="0" width="100%">
                                    <tr>
                                        <td style="text-align: center">
                                            <asp:Literal ID="Literal2" runat="server"></asp:Literal>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <ofc:Chart ID="Chart1" Height="230" Width="330px" runat="server" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="text-align: center">
                                            <asp:Literal ID="Literal3" runat="server"></asp:Literal>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <ofc:Chart ID="Chart2" Height="230" Width="330px" runat="server" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </td>
                <td valign="top">
                    <table>
                        <tr>
                            <td>
                                <asp:Literal ID="Literal4" runat="server"></asp:Literal>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <ofc:Chart ID="Chart3" Height="460" Width="330px" runat="server" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <input type="button" value="Back to graph list" class="Button" onclick="history.go(-1);" />
                </td>
            </tr>
        </table>
    <input type="button" runat="server" id="refresh" style="display: none" />
    </div>
    </form>
</body>
</html>
