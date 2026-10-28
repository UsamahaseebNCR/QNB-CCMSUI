<%@ Page Language="C#" AutoEventWireup="true" Codebehind="AddEditNoteSetType.aspx.cs"
    Inherits="CCMSUI.admin.AddEditNoteSetType" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Add Edit Note Set Type</title>

    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>

    <script type="text/javascript">
        window.parent.window.contentPage = '';          
        
        
    </script>

    <link href="../content/iSuite.css" rel="stylesheet" />
</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)' onload="hideProgressBar();">
    <form id="form1" runat="server">
        <table cellpadding="0" cellspacing="0" border="0" class="MainBox" width="100%">
            <tr>
                <td class="PageHeading" align="left" colspan="2" style="height: 25px">
                    <%=Request.QueryString["note_set_type_id"] == null ? "Add a New Note Set Type " : "Edit Note Set Type"%>
                </td>
            </tr>
            <tr>
                <td class="SearchHeadPaneGradient" colspan="2">
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:ValidationSummary ID="ValidationSummary1" runat="server"></asp:ValidationSummary>
                </td>
            </tr>
            <tr style="height: 20px">
                <td colspan="2" style="color: red;" align="center" valign="top">
                    <asp:Literal ID="Literal_error" runat="server" EnableViewState="False"></asp:Literal></td>
            </tr>
            <tr>
                <td>
                    <table id="inner" border="0" cellpadding="0" cellspacing="0" style="padding-left: 10px">
                        <tr>
                            <td>
                                Selected Organization 
                                    
                            </td>
                            <td>
                                <b><asp:Label ID="Label_SelectedOrganization" runat="server"></asp:Label></b>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Note Set Type Name
                            </td>
                            <td>
                                <asp:TextBox ID="TextBox_NoteSetTypeName" runat="server" CssClass="textbox"></asp:TextBox>
                                <%--<asp:RequiredFieldValidator ID="requiredfieldvalidator_name" runat="server" ControlToValidate="TextBox_NoteSetTypeName"
                                    ErrorMessage="Please enter Note Set Type name.">*</asp:RequiredFieldValidator>--%>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Default Organization Note Set 
                            </td>
                            <td>
                                <asp:CheckBox ID="CheckBox_DefaultOrganization" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <fieldset>
                                    <legend><b>Note Set Type Configuration</b></legend>
                                    <table style="background-color: InfoBackground">
                                        <tr>
                                            <td style="padding-right: 10px">
                                                <b>Note Type</b>
                                            </td>
                                            <td>
                                                <b>Note Title</b></td>
                                            <td>
                                                <b>Denominations</b></td>
                                                <td>
                                                <b>Multi Currency</b></td>
                                            <td>
                                                <b>   isRecycler</b></td>
                                        </tr>
                                        <tr>
                                            <td>
                                                Type 1 :
                                            </td>
                                            <td>
                                                <asp:TextBox ID="TextBox_NoteTitleType1" runat="server" CssClass="textbox" Width="100px"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="TextBox_DenominationType1" runat="server" CssClass="textbox" Width="100px"></asp:TextBox>
                                                <asp:RegularExpressionValidator ID="regularexpressionvalidator_Port" runat="server"
                                                    ControlToValidate="TextBox_DenominationType1" ErrorMessage="Only Numbers are allowed in denomination type 1."
                                                    ValidationExpression="[0-9]*" Text="*"></asp:RegularExpressionValidator>
                                            </td>
                                            <td>
                                                <asp:CheckBox ID="CheckBox_isType1MultiCurrency" runat="server" />
                                            </td>
                                            <td>
                                                <asp:CheckBox ID="CheckBox_isType1Recycler" runat="server" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                Type 2 :
                                            </td>
                                            <td>
                                                <asp:TextBox ID="TextBox_NoteTitleType2" runat="server" CssClass="textbox" Width="100px"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="TextBox_DenominationType2" runat="server" CssClass="textbox" Width="100px"></asp:TextBox>
                                                <asp:RegularExpressionValidator ID="regularexpressionvalidator1" runat="server" ControlToValidate="TextBox_DenominationType2"
                                                    ErrorMessage="Only Numbers are allowed in denomination type 2." ValidationExpression="[0-9]*"
                                                    Text="*"></asp:RegularExpressionValidator>
                                            </td>
                                            <td>
                                                <asp:CheckBox ID="CheckBox_isType2MultiCurrency" runat="server" />
                                            </td>
                                            <td>
                                                <asp:CheckBox ID="CheckBox_isType2Recycler" runat="server" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                Type 3 :
                                            </td>
                                            <td>
                                                <asp:TextBox ID="TextBox_NoteTitleType3" runat="server" CssClass="textbox" Width="100px"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="TextBox_DenominationType3" runat="server" CssClass="textbox" Width="100px"></asp:TextBox>
                                                <asp:RegularExpressionValidator ID="regularexpressionvalidator3" runat="server" ControlToValidate="TextBox_DenominationType3"
                                                    ErrorMessage="Only Numbers are allowed in denomination type 3." ValidationExpression="[0-9]*"
                                                    Text="*"></asp:RegularExpressionValidator>
                                            </td>
                                            <td>
                                                <asp:CheckBox ID="CheckBox_isType3MultiCurrency" runat="server" />
                                            </td>
                                            <td>
                                                <asp:CheckBox ID="CheckBox_isType3Recycler" runat="server" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                Type 4 :
                                            </td>
                                            <td>
                                                <asp:TextBox ID="TextBox_NoteTitleType4" runat="server" CssClass="textbox" Width="100px"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="TextBox_DenominationType4" runat="server" CssClass="textbox" Width="100px"></asp:TextBox>
                                                <asp:RegularExpressionValidator ID="regularexpressionvalidator2" runat="server" ControlToValidate="TextBox_DenominationType4"
                                                    ErrorMessage="Only Numbers are allowed in denomination type 4." ValidationExpression="[0-9]*"
                                                    Text="*"></asp:RegularExpressionValidator>
                                            </td>
                                            <td>
                                                <asp:CheckBox ID="CheckBox_isType4MultiCurrency" runat="server" />
                                            </td>
                                            <td>
                                                <asp:CheckBox ID="CheckBox_isType4Recycler" runat="server" />
                                            </td>
                                        </tr>
                                        <tr >
                                            <td>
                                                Type 5 :
                                            </td>
                                            <td>
                                                <asp:TextBox ID="TextBox_NoteTitleType5" runat="server" CssClass="textbox" Width="100px"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="TextBox_DenominationType5" runat="server" CssClass="textbox" Width="100px"></asp:TextBox>
                                                <asp:RegularExpressionValidator ID="regularexpressionvalidator5" runat="server" ControlToValidate="TextBox_DenominationType5"
                                                    ErrorMessage="Only Numbers are allowed in denomination type 5." ValidationExpression="[0-9]*"
                                                    Text="*"></asp:RegularExpressionValidator>
                                            </td>
                                            <td>
                                                <asp:CheckBox ID="CheckBox_isType5MultiCurrency" runat="server" />
                                            </td>
                                            <td>
                                                <asp:CheckBox ID="CheckBox_isType5Recycler" runat="server" />
                                            </td>
                                        </tr>
                                        <%--<tr style="visibility:hidden">--%>
                                        <tr>
                                            <td>
                                                Type 6 :
                                            </td>
                                            <td>
                                                <asp:TextBox ID="TextBox_NoteTitleType6" runat="server" CssClass="textbox" Width="100px"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="TextBox_DenominationType6" runat="server" CssClass="textbox" Width="100px"></asp:TextBox>
                                                <asp:RegularExpressionValidator ID="regularexpressionvalidator6" runat="server" ControlToValidate="TextBox_DenominationType6"
                                                    ErrorMessage="Only Numbers are allowed in denomination type 6." ValidationExpression="[0-9]*"
                                                    Text="*"></asp:RegularExpressionValidator>
                                            </td>
                                            <td>
                                                <asp:CheckBox ID="CheckBox_isType6MultiCurrency" runat="server" />
                                            </td>
                                            <td>
                                                <asp:CheckBox ID="CheckBox_isType6Recycler" runat="server" />
                                            </td>
                                        </tr>
                                        <%--<tr style="visibility:hidden">--%>
                                        <tr>
                                            <td>
                                                Type 7 :
                                            </td>
                                            <td>
                                                <asp:TextBox ID="TextBox_NoteTitleType7" runat="server" CssClass="textbox" Width="100px"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="TextBox_DenominationType7" runat="server" CssClass="textbox" Width="100px"></asp:TextBox>
                                                <asp:RegularExpressionValidator ID="regularexpressionvalidator7" runat="server" ControlToValidate="TextBox_DenominationType7"
                                                    ErrorMessage="Only Numbers are allowed in denomination type 7." ValidationExpression="[0-9]*"
                                                    Text="*"></asp:RegularExpressionValidator>
                                            </td>
                                            <td>
                                                <asp:CheckBox ID="CheckBox_isType7MultiCurrency" runat="server" />
                                            </td>
                                            <td>
                                                <asp:CheckBox ID="CheckBox_isType7Recycler" runat="server" />
                                            </td>
                                        </tr>
                                    </table>
                                </fieldset>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" style="font-weight: bold; padding-top: 30px;">
                                <asp:Button ID="Button_SaveUpdateNoteSetType" runat="server" Text="Save" CssClass="button"
                                    Width="60px" OnClick="Button_SaveUpdateNoteSetType_Click" />&nbsp;
                                <input id="Button_Cancel" type="button" value="Cancel" onclick="window.location ='NoteSetType.aspx'"
                                    class="button" /></td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        <br />
        <br />
        <input type="button" runat="server" id="refresh" style="display: none" />
    </form>
</body>
</html>
