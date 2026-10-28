<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ErrorPage.aspx.cs" Inherits="CCMSUI.ErrorPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <h1>An Error Has Occurred</h1>
    <p>
    There is an error occured while processing your request.</p>
        <p>
            Please click the link below to get back to Home page
        <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/Home.aspx">Return to the Homepage</asp:HyperLink>
    </p>
    </div>
    </form>
</body>
</html>
