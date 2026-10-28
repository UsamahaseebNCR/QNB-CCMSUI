<%@ Page Language="C#" AutoEventWireup="true" ValidateRequest="true" CodeBehind="AddEditATM.aspx.cs" EnableViewState="true"
    Inherits="Avanza.iSuite.UI.AddEditATM" Title="CCMS - Add Edit ATM" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Add Edit ATM</title>
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
    <script src="../content/Scripts/jquery.js" type="text/javascript"></script>

    <asp:Literal ID="Literal_GoogleScript" runat="server">
    
        
    </asp:Literal>
    <script type="text/javascript">
        window.parent.window.contentPage = '';



    </script>
    <link href="../content/iSuite.css" rel="stylesheet" />
    <style type="text/css">
        html {
            height: 100%;
        }

        body {
            height: 100%;
            margin: 0;
            padding: 0;
        }

        #map-canvas {
            height: 100%;
        }
    </style>


    <%-- <script type="text/javascript" src="https://maps.googleapis.com/maps/api/js?key=AIzaSyB59hfEUsise1ksMJ0yDOC_8zIV6CznAuE">    </script>
    --%>
    <script type="text/javascript">
        var geocoder;
        var address1;
        function initialize() {
            var lat;
            var lng;
            //ShowLatLong(document.getElementById('<%=TextBox_Location.ClientID%>').value);
            //var myLatlng = new google.maps.LatLng(-25.363882, 131.044922);
            lat = document.getElementById('<%=TextBox_Latitude.ClientID%>').value;
            lng = document.getElementById('<%=TextBox_Longitude.ClientID%>').value;
            if (lat.length == 0) {
                lat = 25.2048493;
                lng = 55.270782800000006;
            }

            var myLatlng = new google.maps.LatLng(lat, lng);

            var mapOptions = { zoom: 14, center: myLatlng };
            var map = new google.maps.Map(document.getElementById('map-canvas'), mapOptions);
            var marker = new google.maps.Marker({ position: myLatlng, title: "ATM Location" });
            marker.setMap(map);
        }





        function loadScript() {
            var script = document.createElement('script');
            script.type = 'text/javascript';
            script.src = 'https://maps.googleapis.com/maps/api/js?key=AIzaSyAvdrOICJ-g2i2ell8yebXii3qP8DKcHHA&callback=initMap&' + 'callback=initialize';
            document.body.appendChild(script);

        }


        function ShowLocation(lat, lng) {

            var myLatlng = new google.maps.LatLng(lat, lng);
            var mapOptions = { zoom: 14, center: myLatlng };
            var map = new google.maps.Map(document.getElementById('map-canvas'), mapOptions);
            var marker = new google.maps.Marker({ position: myLatlng });
            marker.setMap(map);


        }



        function ShowLatLong(loc) {

            if (loc.length == 0) return;
            //            if (loc.toString().len == 0)
            //                return;
            //            
            geocoder = new google.maps.Geocoder(); // creating a new geocode object
            address1 = loc;
            if (geocoder) {
                geocoder.geocode({ 'address': address1 }, function (results, status) {
                    if (status == google.maps.GeocoderStatus.OK) {
                        //alert('f');
                        location1 = results[0].geometry.location;
                        var myLatlng = new google.maps.LatLng(location1.lat(), location1.lng());
                        var mapOptions = { zoom: 14, center: myLatlng };
                        var map = new google.maps.Map(document.getElementById('map-canvas'), mapOptions);
                        var marker = new google.maps.Marker({ position: myLatlng });
                        marker.setMap(map);
                        document.getElementById('<%=TextBox_Latitude.ClientID %>').value = location1.lat();
                        document.getElementById('<%=TextBox_Longitude.ClientID %>').value = location1.lng();

                        //var contentString = location1.lat();
                        //alert('f');
                        //var infowindow = new google.maps.InfoWindow({ content: contentString });
                        //alert('f');
                        //google.maps.event.addListener(marker, 'click', function () { infowindow.open(map, marker); });

                        //map.addOverlay(marker);
                        //alert('D'); 
                        //marker.openInfoWindowHtml(address + "<br>Long: " + location1.lng() + "<br>Lati:  " + location1.lat());


                    }
                    else {
                        alert("Geocode was not successful for the following reason: " + status);
                    }
                });
            }
        }


    </script>
</head>
<body onkeydown='checkForCtrlKey(window.event);' onkeyup='checkForCharKey(window.event)'
    onload="hideProgressBar();">
    <form id="form1" runat="server">
        <table cellpadding="0" cellspacing="0" border="0" class="MainBox" width="100%" style="padding-left: 10px;">
            <tr>
                <td class="PageHeading" align="left" colspan="2" style="height: 25px">
                    <%=Request.QueryString["atm_id"] == null?"Add a New ATM ":Request.QueryString["i"]==null?"Edit an ATM":"ATM Inventory"%>
                </td>
            </tr>
            <tr>
                <td class="SearchHeadPaneGradient" colspan="2"></td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:ValidationSummary ID="ValidationSummary1" runat="server"></asp:ValidationSummary>
                </td>
            </tr>
            <tr>
                <td class="ConfirmationText" align="center">
                    <%=Session[SessionVars.notice.ToString()]%>
                    <%Session[SessionVars.notice.ToString()] = "";%>
                    <asp:Literal ID="Literal_ok" runat="server" EnableViewState="False"></asp:Literal>
                </td>
            </tr>
            <tr style="height: 20px">
                <td colspan="2" style="color: red;" align="center" valign="top">
                    <asp:Literal ID="Literal_error" runat="server" EnableViewState="False"></asp:Literal>
                </td>
            </tr>
            <tr>
                <td>
                    <ComponentArt:TabStrip ID="TabStrip_AddEdit" CssClass="TopGroup" DefaultItemLookId="DefaultTabLook"
                        DefaultSelectedItemLookId="SelectedTabLook" DefaultDisabledItemLookId="DisabledTabLook"
                        DefaultGroupTabSpacing="1px" ImagesBaseUrl="../images/Tabimages/" MultiPageId="MultiPage_AddEdit"
                        runat="server" ScrollDownLookId="DefaultTabLook" ScrollLeftLookId="DefaultTabLook"
                        ScrollRightLookId="DefaultTabLook" ScrollUpLookId="DefaultTabLook" TopGroupTabSpacing="1px">
                        <ItemLooks>
                            <ComponentArt:ItemLook LookId="DefaultTabLook" CssClass="DefaultTab" HoverCssClass="DefaultTabHover"
                                LabelPaddingLeft="10px" LabelPaddingRight="10px" LabelPaddingTop="5px" LabelPaddingBottom="4px"
                                LeftIconUrl="tab_left_icon.gif" RightIconUrl="tab_right_icon.gif" HoverLeftIconUrl="hover_tab_left_icon.gif"
                                HoverRightIconUrl="hover_tab_right_icon.gif" LeftIconWidth="3px" LeftIconHeight="21px"
                                RightIconWidth="3px" RightIconHeight="21px" LeftIconVisibility="Always" RightIconVisibility="Always" />
                            <ComponentArt:ItemLook LookId="SelectedTabLook" CssClass="SelectedReportsTab" LabelPaddingLeft="10px"
                                LabelPaddingRight="10px" LabelPaddingTop="4px" LabelPaddingBottom="4px" LeftIconUrl="selected_tab_left_icon.gif"
                                RightIconUrl="selected_tab_right_icon.gif" LeftIconWidth="3px" LeftIconHeight="21px"
                                RightIconWidth="3px" RightIconHeight="21px" LeftIconVisibility="Always" RightIconVisibility="Always" />
                        </ItemLooks>
                        <Tabs>
                            <ComponentArt:TabStripTab ID="TabStripTab_general" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="General Configuration">
                            </ComponentArt:TabStripTab>
                            <ComponentArt:TabStripTab ID="TabStripTab_physicalConfig" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="ATM Physical Cassette Configuration">
                            </ComponentArt:TabStripTab>
                            <ComponentArt:TabStripTab ID="TabStripTab_CDMConfig" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Cash Deposit Module Configuration">
                            </ComponentArt:TabStripTab>
                            <ComponentArt:TabStripTab ID="TabStripTab_CPMConfig" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Cheque Deposit Module Configuration">
                            </ComponentArt:TabStripTab>
                            <ComponentArt:TabStripTab ID="TabStripTab1" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Configuration">
                            </ComponentArt:TabStripTab>
                            <ComponentArt:TabStripTab ID="TabStripTab6" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Inventory">
                            </ComponentArt:TabStripTab>
                            <ComponentArt:TabStripTab ID="TabStripTab11" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Power Management">
                            </ComponentArt:TabStripTab>
                            <ComponentArt:TabStripTab ID="TabStripTab12" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Cash Management">
                            </ComponentArt:TabStripTab>
                            <ComponentArt:TabStripTab ID="TabStripTab10" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Operational Hour Configuration">
                            </ComponentArt:TabStripTab>
                        </Tabs>
                    </ComponentArt:TabStrip>
                    <ComponentArt:MultiPage ID="MultiPage_AddEdit" CssClass="MultiPage" runat="server"
                        Width="100%">
                        <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView_general"
                            Width="100%">
                            <table width="100%" border="0" style="vertical-align: top">
                                <tr>
                                    <td style="width: 100%">
                                        <table id="Table1" border="0" cellpadding="0" cellspacing="0" style="padding-left: 10px; vertical-align: top"
                                            width="100%">
                                            <%--<tr>
                                            <td colspan="2">
                                            <b>Number of Licensed Terminal : <asp:Label ID="Label_ATMCount" runat="server"></asp:Label></b></td>
                                        </tr>--%>
                                            <tr>
                                                <td colspan="2">
                                                    <strong>Select parent region from Tree<br />
                                                        &nbsp;</strong>
                                                </td>
                                            </tr>
                                            <tr style="background-color: #eff0ef">
                                                <td style="width: 40%">
                                                    <asp:Label ID="Label2" Text="ATM ID" runat="server"></asp:Label>
                                                    <span style="color: Red">*</span>
                                                </td>
                                                <td style="width: 40%">
                                                    <asp:TextBox ID="TextBox_ATMFriendlyName" runat="server" CssClass="textbox"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="requiredfieldvalidator1" runat="server" ControlToValidate="TextBox_ATMFriendlyName"
                                                        ErrorMessage="Please enter ATM ID." Text="*"></asp:RequiredFieldValidator>
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator_ATMID" runat="server"
                                                        ValidationExpression="\w{7,10}" ControlToValidate="TextBox_ATMFriendlyName" SetFocusOnError="true"
                                                        ErrorMessage="ATM ID must be between 7 to 10 characters">*</asp:RegularExpressionValidator>
                                                </td>
                                            </tr>

                                            <%--<tr>
                            <td>
                                <asp:Label ID="Label_Title" Text="ATM Title" runat="server"></asp:Label></td>
                            <td>
                                <asp:TextBox ID="TextBox_Title" runat="server" CssClass="textbox"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="requiredfieldvalidator_Title" runat="server" ControlToValidate="TextBox_Title"
                                    ErrorMessage="Please enter ATM Title." Text="*"></asp:RequiredFieldValidator></td>
                        </tr>--%>
                                            <tr>
                                                    <td>
                                                        <asp:Label ID="Label_IP" Text="IP" runat="server"></asp:Label>
                                                        <span style="color: Red">*</span>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="TextBox_IP" runat="server" CssClass="textbox"></asp:TextBox>
                                                        <asp:RequiredFieldValidator ID="requiredfieldvalidator_IP" runat="server" ControlToValidate="TextBox_IP"
                                                            ErrorMessage="Please enter ATM Network IP.">*</asp:RequiredFieldValidator><asp:RegularExpressionValidator
                                                                ID="regularexpressionvalidator_IP" runat="server" ControlToValidate="TextBox_IP"
                                                                ErrorMessage="Invalid ATM Network IP format." ValidationExpression="\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b"
                                                                Text="*"></asp:RegularExpressionValidator>
                                                    </td>
                                                </tr>
                                            <tr id="trPort" runat="server" style="background-color: #eff0ef">
                                                <td>
                                                    <asp:Label ID="Label_Port" Text="Port" runat="server"></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="TextBox_Port" runat="server" CssClass="textbox"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="requiredfieldvalidator_Port" runat="server" ControlToValidate="TextBox_Port"
                                                        ErrorMessage="Please enter ATM Port.">*</asp:RequiredFieldValidator><asp:RegularExpressionValidator
                                                            ID="regularexpressionvalidator_Port" runat="server" ControlToValidate="TextBox_Port"
                                                            ErrorMessage="Only Numbers are allowed in ATM Port." ValidationExpression="[0-9]*"
                                                            Text="*"></asp:RegularExpressionValidator>
                                                </td>
                                            </tr>
                                             <tr>
                                                    <td>
                                                        <asp:Label ID="Label_Type" Text="Vendor Type" runat="server"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="DropDownList_Type" Enabled="true" runat="server" CssClass="textbox">
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                            <tr style="background-color: #eff0ef">
                                                <td>
                                                    <asp:Label ID="Label1" runat="server" Text="Status"></asp:Label>
                                                </td>
                                                <td>&nbsp;
                                                <asp:RadioButton ID="RadioButton_active" runat="server" Checked="True" GroupName="status"
                                                    Text="active" />&nbsp;
                                                <asp:RadioButton ID="RadioButton_inactive" runat="server" GroupName="status" Text="inactive" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="Label3" Text="ATM Type" runat="server"></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:CheckBox ID="CheckBox_ATM" Text="ATM" runat="server" Checked="true" />
                                                    <asp:CheckBox ID="CheckBox_CDM" Text="BNA" runat="server" />
                                                    <asp:CheckBox ID="CheckBox_CCDM" Text="CPM" runat="server" />
                                                    <asp:CheckBox ID="CheckBox_SDM" Text="SDM" runat="server" />
                                                </td>
                                            </tr>
                                             <tr style="background-color: #eff0ef">
                                                    <td>
                                                        <asp:Label ID="Label_NoteSetType" Text="Note Set Type ID" runat="server"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="DropDownList_NoteSetTypeID" runat="server" CssClass="textbox"
                                                            AutoPostBack="true" OnSelectedIndexChanged="DropDownList_NoteSetType_Change">
                                                            <asp:ListItem>Select</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                             <tr>
                                                    <td>Location
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="TextBox_Location" runat="server" CssClass="textbox"></asp:TextBox>
                                                        <input type="button" value="Find" onclick="ShowLatLong(document.getElementById('<%=TextBox_Location.ClientID%>').value)"
                                                            class="button" style="visibility:hidden; display:none" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>Longitude
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="TextBox_Longitude" runat="server" CssClass="textbox"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>Latitude
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="TextBox_Latitude" runat="server" CssClass="textbox"></asp:TextBox>
                                                        <input type="button" value="Find" onclick="ShowLocation(document.getElementById('<%=TextBox_Latitude.ClientID%>').value,document.getElementById('<%=TextBox_Longitude.ClientID%>').value); return false;"
                                                            class="button" style="visibility:hidden; display:none"/>
                                                    </td>
                                                </tr>
                                             <tr>
                                                                    <td>Bandwidth
                                                                    </td>
                                                                    <td>
                                                                        <asp:DropDownList ID="DropDownList_Bandwidth" runat="server" CssClass="textbox" Width="100px">
                                                                        </asp:DropDownList>
                                                                    </td>
                                                                </tr>
                                            <asp:Panel ID="AdvancedATMConfiguration" runat="server">
                                                
                                                
                                               
                                                <tr style="background-color: #eff0ef">
                                                    <td style="width: 20%">
                                                        <asp:Label ID="Label4" Text="CIT ATM ID" runat="server"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="TextBox_CITATMID" runat="server" CssClass="textbox"></asp:TextBox>
                                                    </td>
                                                </tr>
                                               
                                                <tr style="background-color: #eff0ef">
                                                    <td>Windows 7 OS
                                                    </td>
                                                    <td>
                                                        <asp:CheckBox ID="CheckBox_Win7OS" runat="server" />
                                                    </td>
                                                </tr>
                                                <tr style="background-color: #eff0ef">
                                                    <td>Suspend Cash Orders
                                                    </td>
                                                    <td>
                                                        <asp:CheckBox ID="CheckBox_SuspendCashOrder" runat="server" Checked="true" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>Exclude DFF Generation
                                                    </td>
                                                    <td>
                                                        <asp:CheckBox ID="CheckBox_ExcludeDFFGeneration" runat="server" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>Is it Recycler
                                                    </td>
                                                    <td>
                                                        <asp:CheckBox ID="CheckBox_isRecycler" runat="server" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>Default Replenishment Type
                                                    </td>
                                                    <td>
                                                        <asp:RadioButtonList ID="RadioButtonList_Type" runat="server" RepeatDirection="Horizontal">
                                                            <asp:ListItem Text="Swap" Value="True" Selected="True"></asp:ListItem>
                                                            <asp:ListItem Text="Add" Value="False"></asp:ListItem>
                                                        </asp:RadioButtonList>
                                                    </td>
                                                </tr>

                                                <tr>
                                                    <td>Minimum Amount For Normal Delivery
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="TextBox_minNormalDelivery" runat="server" CssClass="textbox" Text="200000"></asp:TextBox>
                                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator30" runat="server"
                                                            ValidationExpression="\d+" ControlToValidate="TextBox_minNormalDelivery" ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                    </td>
                                                </tr>
                                                <tr style="background-color: #eff0ef">
                                                    <td>Minimum Operating Balance
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="TextBox_minOperatingBalance" runat="server" CssClass="textbox" Text="100000"></asp:TextBox>
                                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator31" runat="server"
                                                            ValidationExpression="\d+" ControlToValidate="TextBox_minOperatingBalance" ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                    </td>
                                                </tr>
                                               
                                                <tr style="background-color: #eff0ef">
                                                    <td>Allowed Inactivity Period
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="TextBox_allowInactivityPeriod" runat="server" CssClass="textbox"></asp:TextBox>
                                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator53" runat="server"
                                                            ValidationExpression="\d+" ControlToValidate="TextBox_allowInactivityPeriod"
                                                            ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>GL#
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="TextBox_GLNo" runat="server" CssClass="textbox"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr style="background-color: #eff0ef">
                                                    <td>
                                                        <asp:Label ID="Label_CIT" Text="CIT" runat="server"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="DropDownList_CIT" runat="server" CssClass="textbox">
                                                            <asp:ListItem Value="">Select</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>Description
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="TextBox_desc" runat="server" CssClass="textbox"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>Disable DFF Generation
                                                    </td>
                                                    <td>
                                                        <asp:CheckBox ID="CheckBox_DisableDFFGeneration" runat="server" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>Default Data Extraction
                                                    </td>
                                                    <td>
                                                        <asp:CheckBox ID="CheckBox_EJ" runat="server" Text="EJ" />
                                                        <asp:CheckBox ID="CheckBox_Counter" runat="server" Checked="true" Text="Counter" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>Priority
                                                    </td>
                                                    <td>
                                                        <asp:RadioButtonList ID="RadioButtonList_Priority" runat="server" RepeatColumns="2">
                                                            <asp:ListItem Text="EJ" Value="0"></asp:ListItem>
                                                            <asp:ListItem Text="Counter" Value="1" Selected="True"> </asp:ListItem>
                                                        </asp:RadioButtonList>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="2" style="height: 34px" valign="middle">
                                                        <span style="color: Navy; font-weight: bold;">Assigned users</span>&nbsp;
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td></td>
                                                    <td>
                                                        <div>
                                                            <input type="button" id="button_checkAll" value="Check All" class="button" onclick="setCheckATMUsers(true, 'checkbox_list')" />
                                                            <input type="button" id="button_uncheckAll" value="Uncheck All" class="button" onclick="setCheckATMUsers(false, 'checkbox_list')" />
                                                        </div>
                                                        <div>

                                                            <%--   <input id="textbox_search" name="textbox_search" type="text" class="textboxNormal" />--%>
                                                            <asp:TextBox ID="textbox_search" runat="server" CssClass="textboxNormal"></asp:TextBox>
                                                            <asp:Button ID="Button1" runat="server" Text="Search" AutoPostback="false" CausesValidation="false" OnClick="Button_Search"
                                                                CssClass="button" Width="60px" />
                                                            <%--  <asp:Button  ID="Button2" runat="server" Text="Reset" CausesValidation="false" OnClick="Button_Reset"
                    CssClass="button" Width="60px" />--%>
                                                            <%--<img alt="Search" src="../images/findText.gif" style="border-width: 0px" 
                                                                <%--onclick ="imageClicked(this);"/>--%>
                                                        </div>
                                                        <div style="overflow: scroll; height: 150px; width: 200px; background-color: InfoBackground">
                                                            <asp:CheckBoxList ID="checkbox_list" runat="server" DataTextField="UserFullName"
                                                                DataValueField="UserId" />
                                                        </div>
                                                    </td>
                                                </tr>
                                            </asp:Panel>
                                        </table>
                                    </td>
                                    <td valign="top" style="width: 50%">
                                        <table width="100%" border="0" id="tdRetryConfiguration" runat="server">
                                            <tr>
                                                <td style="width: 100%">
                                                    <fieldset style="width: 100%">
                                                        <legend><b>Configure Job Retries</b></legend>
                                                        <table width="100%">
                                                            <tr>
                                                                <td>Number of retry for Sending Alert
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox CssClass="textbox" ID="TextBox_retryCountAlert" runat="server" Text="10"></asp:TextBox>
                                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator29" runat="server" ControlToValidate="TextBox_retryCountAlert"
                                                                        ErrorMessage="Please enter number of retry for sending alerts">*</asp:RequiredFieldValidator>
                                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator50" runat="server"
                                                                        ValidationExpression="\d+" ErrorMessage="Only numbers are allowed" ControlToValidate="TextBox_retryCountAlert">*</asp:RegularExpressionValidator>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>Number of retry for Cash Order Upload
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox CssClass="textbox" ID="TextBox_NumberOfRetryCashOrderUpload" runat="server"
                                                                        Text="10"></asp:TextBox>
                                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="TextBox_NumberOfRetryCashOrderUpload"
                                                                        ErrorMessage="Please enter number of retry for cash order upload">*</asp:RequiredFieldValidator>
                                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator40" runat="server"
                                                                        ValidationExpression="\d+" ErrorMessage="Only numbers are allowed" ControlToValidate="TextBox_NumberOfRetryCashOrderUpload">*</asp:RegularExpressionValidator>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>Number of retry for Uploading Configuration
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox CssClass="textbox" ID="TextBox_NumberOfRetryConfUpload" runat="server"
                                                                        Text="10"></asp:TextBox>
                                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator12" runat="server" ControlToValidate="TextBox_NumberOfRetryConfUpload"
                                                                        ErrorMessage="Please enter number of retry for uploading configuration">*</asp:RequiredFieldValidator>
                                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator43" runat="server"
                                                                        ValidationExpression="\d+" ErrorMessage="Only numbers are allowed" ControlToValidate="TextBox_NumberOfRetryConfUpload">*</asp:RegularExpressionValidator>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>Number of retry for Downloading Counter File
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox CssClass="textbox" ID="TextBox_NoOfRetryCounterFile" runat="server"
                                                                        Text="10"></asp:TextBox>
                                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator14" runat="server" ControlToValidate="TextBox_NoOfRetryCounterFile"
                                                                        ErrorMessage="Please enter number of retry for downloading counter file">*</asp:RequiredFieldValidator>
                                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator44" runat="server"
                                                                        ValidationExpression="\d+" ErrorMessage="Only numbers are allowed" ControlToValidate="TextBox_NoOfRetryCounterFile">*</asp:RegularExpressionValidator>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>Number of retry for Executing Restart Schedule
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox CssClass="textbox" ID="TextBox_NoOfRetryRestart" runat="server" Text="10"></asp:TextBox>
                                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator15" runat="server" ControlToValidate="TextBox_NoOfRetryRestart"
                                                                        ErrorMessage="Please enter number of retry for restart schedule">*</asp:RequiredFieldValidator>
                                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator45" runat="server"
                                                                        ValidationExpression="\d+" ErrorMessage="Only numbers are allowed" ControlToValidate="TextBox_NoOfRetryRestart">*</asp:RegularExpressionValidator>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>Number of retry for Executing DateTime Sync Schedule
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox CssClass="textbox" ID="TextBox_NoOfRetryDateTimeSync" runat="server"
                                                                        Text="10"></asp:TextBox>
                                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator16" runat="server" ControlToValidate="TextBox_NoOfRetryDateTimeSync"
                                                                        ErrorMessage="Please enter number of retry for datetime sync schedule">*</asp:RequiredFieldValidator>
                                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator46" runat="server"
                                                                        ValidationExpression="\d+" ErrorMessage="Only numbers are allowed" ControlToValidate="TextBox_NoOfRetryDateTimeSync">*</asp:RegularExpressionValidator>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </fieldset>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <div id="map-canvas" style="width: 450px; height: 250px">
                                                    </div>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </ComponentArt:PageView>
                        <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView_physicalConfig1">
                            <table id="TableConfig" border="0" cellpadding="0" cellspacing="0" style="padding-left: 10px"
                                width="100%">
                                <tr>
                                    <td colspan="2">
                                        <table border="0" cellpadding="0" cellspacing="0" width="95%">
                                            <tr>
                                                <td></td>
                                                <td>
                                                    <b>Denominations</b>
                                                </td>
                                                <td>
                                                    <b>Capacity</b>
                                                </td>
                                                <td>
                                                    <b>Purge Bin Threshold</b>
                                                </td>
                                                <td>
                                                    <b>Min Notes Threshold %</b>
                                                </td>
                                                <td>
                                                    <b>Min Notes Threshold Value</b>
                                                </td>
                                            </tr>
                                            <tr style="background-color: #eff0ef">
                                                <td style="vertical-align: top; padding-right: 35px;">Cassette1
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="TextBox_Cassette1_Denominations" runat="server" CssClass="textbox"
                                                        Width="100px"></asp:TextBox>
                                                    <asp:RegularExpressionValidator ID="ValCas1Denomination" runat="server" ValidationExpression="\d+"
                                                        ControlToValidate="TextBox_Cassette1_Denominations" ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="TextBox_Cassette1_Capacity" runat="server" CssClass="textbox" Width="100px"></asp:TextBox>
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ValidationExpression="\d+"
                                                        ControlToValidate="TextBox_Cassette1_Capacity" ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                    <%--<asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="TextBox_Cassette1_Capacity"
                                                ErrorMessage="Please enter Capacity for Cassette1">*</asp:RequiredFieldValidator>--%>
                                                    <%--                                            <asp:RangeValidator ID="RangeCassette1Capacity" runat="server" ControlToValidate="TextBox_Cassette1_Capacity"
                                                MinimumValue="0" MaximumValue="65535" Type="Integer" ErrorMessage="Invalid capacity entered for cassette1">*</asp:RangeValidator>
                                                    --%>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="TextBox_Purge1_Threshold" runat="server" CssClass="textbox" Width="100px"></asp:TextBox>
                                                    <asp:CheckBox ID="CheckBox_Purge1_Threshold" runat="server" />
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator32" runat="server"
                                                        ValidationExpression="\d+" ControlToValidate="TextBox_Purge1_Threshold" ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="TextBox_Type1MinNotesThreshold" runat="server" CssClass="textbox"
                                                        Width="100px"></asp:TextBox>
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator69" runat="server"
                                                        ValidationExpression="\d+" ControlToValidate="TextBox_Type1MinNotesThreshold"
                                                        ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="TextBox_Type1MinNotesThresholdValue" runat="server" CssClass="textbox"
                                                        Width="100px"></asp:TextBox>
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator78" runat="server"
                                                        ValidationExpression="\d+" ControlToValidate="TextBox_Type1MinNotesThresholdValue"
                                                        ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="vertical-align: top">Cassette2
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="TextBox_Cassette2_Denominations" runat="server" CssClass="textbox"
                                                        Width="100px"></asp:TextBox>
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ValidationExpression="\d+"
                                                        ControlToValidate="TextBox_Cassette2_Denominations" ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                    <%--<asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="TextBox_Cassette2_Denominations"
                                                ErrorMessage="Please enter Denomination for Cassette2">*</asp:RequiredFieldValidator>--%>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="TextBox_Cassette2_Capacity" runat="server" CssClass="textbox" Width="100px"></asp:TextBox>
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" ValidationExpression="\d+"
                                                        ControlToValidate="TextBox_Cassette2_Capacity" ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                    <%--<asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="TextBox_Cassette2_Capacity"
                                                ErrorMessage="Please enter Capacity for Cassette2">*</asp:RequiredFieldValidator>--%>
                                                    <%--<asp:RangeValidator ID="RangeValidator1" runat="server" ControlToValidate="TextBox_Cassette2_Capacity"
                                                MinimumValue="0" MaximumValue="65535" Type="Integer" ErrorMessage="Invalid capacity entered for cassette2">*</asp:RangeValidator>--%>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="TextBox_Purge2_Threshold" runat="server" CssClass="textbox" Width="100px"></asp:TextBox>
                                                    <asp:CheckBox ID="CheckBox_Purge2_Threshold" runat="server" />
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator33" runat="server"
                                                        ValidationExpression="\d+" ControlToValidate="TextBox_Purge2_Threshold" ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="TextBox_Type2MinNotesThreshold" runat="server" CssClass="textbox"
                                                        Width="100px"></asp:TextBox>
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator70" runat="server"
                                                        ValidationExpression="\d+" ControlToValidate="TextBox_Type2MinNotesThreshold"
                                                        ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="TextBox_Type2MinNotesThresholdValue" runat="server" CssClass="textbox"
                                                        Width="100px"></asp:TextBox>
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator79" runat="server"
                                                        ValidationExpression="\d+" ControlToValidate="TextBox_Type2MinNotesThresholdValue"
                                                        ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                </td>
                                            </tr>
                                            <tr style="background-color: #eff0ef">
                                                <td style="vertical-align: top">Cassette3
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="TextBox_Cassette3_Denominations" runat="server" CssClass="textbox"
                                                        Width="100px"></asp:TextBox>
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator4" runat="server" ValidationExpression="\d+"
                                                        ControlToValidate="TextBox_Cassette3_Denominations" ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                    <%--<asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="TextBox_Cassette3_Denominations"
                                                ErrorMessage="Please enter Denomination for Cassette3">*</asp:RequiredFieldValidator>--%>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="TextBox_Cassette3_Capacity" runat="server" CssClass="textbox" Width="100px"></asp:TextBox>
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator5" runat="server" ValidationExpression="\d+"
                                                        ControlToValidate="TextBox_Cassette3_Capacity" ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                    <%--<asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ControlToValidate="TextBox_Cassette3_Capacity"
                                                ErrorMessage="Please enter Capacity for Cassette3">*</asp:RequiredFieldValidator>--%>
                                                    <%--                                            <asp:RangeValidator ID="RangeValidator2" runat="server" ControlToValidate="TextBox_Cassette3_Capacity"
                                                MinimumValue="0" MaximumValue="65535" Type="Integer" ErrorMessage="Invalid capacity entered for cassette3">*</asp:RangeValidator>
                                                    --%>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="TextBox_Purge3_Threshold" runat="server" CssClass="textbox" Width="100px"></asp:TextBox>
                                                    <asp:CheckBox ID="CheckBox_Purge3_Threshold" runat="server" />
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator35" runat="server"
                                                        ValidationExpression="\d+" ControlToValidate="TextBox_Purge3_Threshold" ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="TextBox_Type3MinNotesThreshold" runat="server" CssClass="textbox"
                                                        Width="100px"></asp:TextBox>
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator71" runat="server"
                                                        ValidationExpression="\d+" ControlToValidate="TextBox_Type3MinNotesThreshold"
                                                        ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="TextBox_Type3MinNotesThresholdValue" runat="server" CssClass="textbox"
                                                        Width="100px"></asp:TextBox>
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator80" runat="server"
                                                        ValidationExpression="\d+" ControlToValidate="TextBox_Type3MinNotesThresholdValue"
                                                        ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="vertical-align: top">Cassette4
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="TextBox_Cassette4_Denominations" runat="server" CssClass="textbox"
                                                        Width="100px"></asp:TextBox>
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator6" runat="server" ValidationExpression="\d+"
                                                        ControlToValidate="TextBox_Cassette4_Denominations" ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                    <%--<asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" ControlToValidate="TextBox_Cassette4_Denominations"
                                                ErrorMessage="Please enter Denomination for Cassette4">*</asp:RequiredFieldValidator>--%>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="TextBox_Cassette4_Capacity" runat="server" CssClass="textbox" Width="100px"></asp:TextBox>
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator7" runat="server" ValidationExpression="\d+"
                                                        ControlToValidate="TextBox_Cassette4_Capacity" ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                    <%--<asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" ControlToValidate="TextBox_Cassette4_Capacity"
                                                ErrorMessage="Please enter Capacity for Cassette4">*</asp:RequiredFieldValidator>--%>
                                                    <%--                                            <asp:RangeValidator ID="RangeValidator3" runat="server" ControlToValidate="TextBox_Cassette4_Capacity"
                                                MinimumValue="0" MaximumValue="65535" Type="Integer" ErrorMessage="Invalid capacity entered for cassette4">*</asp:RangeValidator>
                                                    --%>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="TextBox_Purge4_Threshold" runat="server" CssClass="textbox" Width="100px"></asp:TextBox>
                                                    <asp:CheckBox ID="CheckBox_Purge4_Threshold" runat="server" />
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator36" runat="server"
                                                        ValidationExpression="\d+" ControlToValidate="TextBox_Purge4_Threshold" ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="TextBox_Type4MinNotesThreshold" runat="server" CssClass="textbox"
                                                        Width="100px"></asp:TextBox>
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator72" runat="server"
                                                        ValidationExpression="\d+" ControlToValidate="TextBox_Type4MinNotesThreshold"
                                                        ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="TextBox_Type4MinNotesThresholdValue" runat="server" CssClass="textbox"
                                                        Width="100px"></asp:TextBox>
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator81" runat="server"
                                                        ValidationExpression="\d+" ControlToValidate="TextBox_Type4MinNotesThresholdValue"
                                                        ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                </td>
                                            </tr>
                                            <tr style="background-color: #eff0ef">
                                                <td style="vertical-align: top">Cassette5
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="TextBox_Cassette5_Denominations" runat="server" CssClass="textbox"
                                                        Width="100px"></asp:TextBox>
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator8" runat="server" ValidationExpression="\d+"
                                                        ControlToValidate="TextBox_Cassette5_Denominations" ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="TextBox_Cassette5_Capacity" runat="server" CssClass="textbox" Width="100px"></asp:TextBox>
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator9" runat="server" ValidationExpression="\d+"
                                                        ControlToValidate="TextBox_Cassette5_Capacity" ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                    <%--                                            <asp:RangeValidator ID="RangeValidator4" runat="server" ControlToValidate="TextBox_Cassette5_Capacity"
                                                MinimumValue="0" MaximumValue="65535" Type="Integer" ErrorMessage="Invalid capacity entered for cassette5">*</asp:RangeValidator>
                                                    --%>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="TextBox_Purge5_Threshold" runat="server" CssClass="textbox" Width="100px"></asp:TextBox>
                                                    <asp:CheckBox ID="CheckBox_Purge5_Threshold" runat="server" />
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator37" runat="server"
                                                        ValidationExpression="\d+" ControlToValidate="TextBox_Purge5_Threshold" ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="TextBox_Type5MinNotesThreshold" runat="server" CssClass="textbox"
                                                        Width="100px"></asp:TextBox>
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator75" runat="server"
                                                        ValidationExpression="\d+" ControlToValidate="TextBox_Type5MinNotesThreshold"
                                                        ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="TextBox_Type5MinNotesThresholdValue" runat="server" CssClass="textbox"
                                                        Width="100px"></asp:TextBox>
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator82" runat="server"
                                                        ValidationExpression="\d+" ControlToValidate="TextBox_Type5MinNotesThresholdValue"
                                                        ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="vertical-align: top">Cassette6
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="TextBox_Cassette6_Denominations" runat="server" CssClass="textbox"
                                                        Width="100px"></asp:TextBox>
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator10" runat="server"
                                                        ValidationExpression="\d+" ControlToValidate="TextBox_Cassette6_Denominations"
                                                        ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="TextBox_Cassette6_Capacity" runat="server" CssClass="textbox" Width="100px"> </asp:TextBox>
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator11" runat="server"
                                                        ValidationExpression="\d+" ControlToValidate="TextBox_Cassette6_Capacity" ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                    <%--                                            <asp:RangeValidator ID="RangeValidator5" runat="server" ControlToValidate="TextBox_Cassette6_Capacity"
                                                MinimumValue="0" MaximumValue="65535" Type="Integer" ErrorMessage="Invalid capacity entered for cassette6">*</asp:RangeValidator>
                                                    --%>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="TextBox_Purge6_Threshold" runat="server" CssClass="textbox" Width="100px"></asp:TextBox>
                                                    <asp:CheckBox ID="CheckBox_Purge6_Threshold" runat="server" />
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator38" runat="server"
                                                        ValidationExpression="\d+" ControlToValidate="TextBox_Purge6_Threshold" ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="TextBox_Type6MinNotesThreshold" runat="server" CssClass="textbox"
                                                        Width="100px"></asp:TextBox>
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator76" runat="server"
                                                        ValidationExpression="\d+" ControlToValidate="TextBox_Type6MinNotesThreshold"
                                                        ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="TextBox_Type6MinNotesThresholdValue" runat="server" CssClass="textbox"
                                                        Width="100px"></asp:TextBox>
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator83" runat="server"
                                                        ValidationExpression="\d+" ControlToValidate="TextBox_Type6MinNotesThresholdValue"
                                                        ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                </td>
                                            </tr>
                                            <tr style="background-color: #eff0ef">
                                                <td style="vertical-align: top">Cassette7
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="TextBox_Cassette7_Denominations" runat="server" CssClass="textbox"
                                                        Width="100px"></asp:TextBox>
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator12" runat="server"
                                                        ValidationExpression="\d+" ControlToValidate="TextBox_Cassette7_Denominations"
                                                        ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="TextBox_Cassette7_Capacity" runat="server" CssClass="textbox" Width="100px"></asp:TextBox>
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator13" runat="server"
                                                        ValidationExpression="\d+" ControlToValidate="TextBox_Cassette7_Capacity" ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                    <%--                                            <asp:RangeValidator ID="RangeValidator6" runat="server" ControlToValidate="TextBox_Cassette7_Capacity"
                                                MinimumValue="0" MaximumValue="65535" Type="Integer" ErrorMessage="Invalid capacity entered for cassette7">*</asp:RangeValidator>
                                                    --%>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="TextBox_Purge7_Threshold" runat="server" CssClass="textbox" Width="100px"></asp:TextBox>
                                                    <asp:CheckBox ID="CheckBox_Purge7_Threshold" runat="server" />
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator39" runat="server"
                                                        ValidationExpression="\d+" ControlToValidate="TextBox_Purge7_Threshold" ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="TextBox_Type7MinNotesThreshold" runat="server" CssClass="textbox"
                                                        Width="100px"></asp:TextBox>
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator77" runat="server"
                                                        ValidationExpression="\d+" ControlToValidate="TextBox_Type7MinNotesThreshold"
                                                        ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="TextBox_Type7MinNotesThresholdValue" runat="server" CssClass="textbox"
                                                        Width="100px"></asp:TextBox>
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator84" runat="server"
                                                        ValidationExpression="\d+" ControlToValidate="TextBox_Type7MinNotesThresholdValue"
                                                        ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </ComponentArt:PageView>
                        <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView_cashorder">
                            <table id="Table3" border="0" cellpadding="0" cellspacing="0" style="padding-left: 10px"
                                width="70%">
                                <tr style="background-color: #eff0ef">
                                    <td>Capacity of Cassette1
                                    </td>
                                    <td>
                                        <asp:TextBox ID="TextBox_CDM1" runat="server" CssClass="textbox" Width="100px"></asp:TextBox>
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator14" runat="server"
                                            ValidationExpression="\d+" ControlToValidate="TextBox_CDM1" ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                    </td>
                                    <td>Threshold
                                    </td>
                                    <td>
                                        <asp:TextBox ID="TextBox_CDM1Threshold" runat="server" CssClass="textbox" Width="100px"></asp:TextBox>
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator15" runat="server"
                                            ValidationExpression="\d+" ControlToValidate="TextBox_CDM1Threshold" ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td>Capacity of Cassette2
                                    </td>
                                    <td>
                                        <asp:TextBox ID="TextBox_CDM2" runat="server" CssClass="textbox" Width="100px"></asp:TextBox>
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator16" runat="server"
                                            ValidationExpression="\d+" ControlToValidate="TextBox_CDM2" ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                    </td>
                                    <td>Threshold
                                    </td>
                                    <td>
                                        <asp:TextBox ID="TextBox_CDM2Threshold" runat="server" CssClass="textbox" Width="100px"></asp:TextBox>
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator17" runat="server"
                                            ValidationExpression="\d+" ControlToValidate="TextBox_CDM2Threshold" ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                    </td>
                                </tr>
                                <tr style="background-color: #eff0ef">
                                    <td>Capacity of Cassette3
                                    </td>
                                    <td>
                                        <asp:TextBox ID="TextBox_CDM3" runat="server" CssClass="textbox" Width="100px"></asp:TextBox>
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator18" runat="server"
                                            ValidationExpression="\d+" ControlToValidate="TextBox_CDM3" ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                    </td>
                                    <td>Threshold
                                    </td>
                                    <td>
                                        <asp:TextBox ID="TextBox_CDM3Threshold" runat="server" CssClass="textbox" Width="100px"></asp:TextBox>
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator19" runat="server"
                                            ValidationExpression="\d+" ControlToValidate="TextBox_CDM3Threshold" ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td>Capacity of Cassette4
                                    </td>
                                    <td>
                                        <asp:TextBox ID="TextBox_CDM4" runat="server" CssClass="textbox" Width="100px"></asp:TextBox>
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator20" runat="server"
                                            ValidationExpression="\d+" ControlToValidate="TextBox_CDM4" ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                    </td>
                                    <td>Threshold
                                    </td>
                                    <td>
                                        <asp:TextBox ID="TextBox_CDM4Threshold" runat="server" CssClass="textbox" Width="100px"></asp:TextBox>
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator21" runat="server"
                                            ValidationExpression="\d+" ControlToValidate="TextBox_CDM4Threshold" ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                    </td>
                                </tr>
                                <tr style="background-color: #eff0ef">
                                    <td>Allowed Inactivity Period
                                    </td>
                                    <td>
                                        <asp:TextBox ID="TextBox_BNAInactivityPeriod" runat="server" CssClass="textbox" Width="100px"></asp:TextBox>
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator54" runat="server"
                                            Width="100px" ValidationExpression="\d+" ControlToValidate="TextBox_BNAInactivityPeriod"
                                            ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                    </td>
                                </tr>
                            </table>
                        </ComponentArt:PageView>
                        <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView1">
                            <table id="Table4" border="0" cellpadding="0" cellspacing="0" style="padding-left: 10px"
                                width="70%">
                                <tr>
                                    <td colspan="2">
                                        <table>
                                            <tr style="background-color: #eff0ef">
                                                <td>Capacity of Bin 1
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="TextBox_CCDM1Capacity" runat="server" CssClass="textbox" Width="100px"></asp:TextBox>
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator22" runat="server"
                                                        ValidationExpression="\d+" ControlToValidate="TextBox_CCDM1Capacity" ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                </td>
                                                <td>Threshold
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="TextBox_CCDM1Threshold" runat="server" CssClass="textbox" Width="100px"></asp:TextBox>
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator23" runat="server"
                                                        ValidationExpression="\d+" ControlToValidate="TextBox_CCDM1Threshold" ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Capacity of Bin 2
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="TextBox_CCDM2Capacity" runat="server" CssClass="textbox" Width="100px"></asp:TextBox>
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator24" runat="server"
                                                        ValidationExpression="\d+" ControlToValidate="TextBox_CCDM2Capacity" ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                </td>
                                                <td>Threshold
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="TextBox_CCDM2Threshold" runat="server" CssClass="textbox" Width="100px"></asp:TextBox>
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator25" runat="server"
                                                        ValidationExpression="\d+" ControlToValidate="TextBox_CCDM2Threshold" ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                </td>
                                            </tr>
                                            <tr style="background-color: #eff0ef">
                                                <td>Capacity of Bin 3
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="TextBox_CCDM3Capacity" runat="server" CssClass="textbox" Width="100px"></asp:TextBox>
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator26" runat="server"
                                                        ValidationExpression="\d+" ControlToValidate="TextBox_CCDM3Capacity" ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                </td>
                                                <td>Threshold
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="TextBox_CCDM3Threshold" runat="server" CssClass="textbox" Width="100px"></asp:TextBox>
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator27" runat="server"
                                                        ValidationExpression="\d+" ControlToValidate="TextBox_CCDM3Threshold" ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Capacity of Bin 4
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="TextBox_CCDM4Capacity" runat="server" CssClass="textbox" Width="100px"></asp:TextBox>
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator28" runat="server"
                                                        ValidationExpression="\d+" ControlToValidate="TextBox_CCDM4Capacity" ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                </td>
                                                <td>Threshold
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="TextBox_CCDM4Threshold" runat="server" CssClass="textbox" Width="100px"></asp:TextBox>
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator29" runat="server"
                                                        ValidationExpression="\d+" ControlToValidate="TextBox_CCDM4Threshold" ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                </td>
                                            </tr>
                                            <tr style="background-color: #eff0ef">
                                                <td>Allowed Inactivity Period
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="TextBox_ChequeInactivityPeriod" runat="server" CssClass="textbox"
                                                        Width="100px"></asp:TextBox>
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator67" runat="server"
                                                        Width="100px" ValidationExpression="\d+" ControlToValidate="TextBox_ChequeInactivityPeriod"
                                                        ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </ComponentArt:PageView>
                        <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView3">
                            <table id="Table6" border="0" cellpadding="0" cellspacing="0" style="padding-left: 10px"
                                width="100%">
                                <tr>
                                    <td>
                                        <ComponentArt:TabStrip ID="TabStrip_Configuration" CssClass="TopGroup" DefaultItemLookId="DefaultTabLook"
                                            DefaultSelectedItemLookId="SelectedTabLook" DefaultDisabledItemLookId="DisabledTabLook"
                                            DefaultGroupTabSpacing="1px" ImagesBaseUrl="../images/Tabimages/" MultiPageId="MultiPage2"
                                            runat="server" ScrollDownLookId="DefaultTabLook" ScrollLeftLookId="DefaultTabLook"
                                            ScrollRightLookId="DefaultTabLook" ScrollUpLookId="DefaultTabLook" TopGroupTabSpacing="1px"
                                            Width="100%">
                                            <ItemLooks>
                                                <ComponentArt:ItemLook LookId="DefaultTabLook" CssClass="DefaultTab" HoverCssClass="DefaultTabHover"
                                                    LabelPaddingLeft="10px" LabelPaddingRight="10px" LabelPaddingTop="5px" LabelPaddingBottom="4px"
                                                    LeftIconUrl="tab_left_icon.gif" RightIconUrl="tab_right_icon.gif" HoverLeftIconUrl="hover_tab_left_icon.gif"
                                                    HoverRightIconUrl="hover_tab_right_icon.gif" LeftIconWidth="3px" LeftIconHeight="21px"
                                                    RightIconWidth="3px" RightIconHeight="21px" LeftIconVisibility="Always" RightIconVisibility="Always" />
                                                <ComponentArt:ItemLook LookId="SelectedTabLook" CssClass="SelectedReportsTab" LabelPaddingLeft="10px"
                                                    LabelPaddingRight="10px" LabelPaddingTop="4px" LabelPaddingBottom="4px" LeftIconUrl="selected_tab_left_icon.gif"
                                                    RightIconUrl="selected_tab_right_icon.gif" LeftIconWidth="3px" LeftIconHeight="21px"
                                                    RightIconWidth="3px" RightIconHeight="21px" LeftIconVisibility="Always" RightIconVisibility="Always" />
                                            </ItemLooks>
                                            <Tabs>
                                                <ComponentArt:TabStripTab ID="TabStripTab4" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                                    ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                                    DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                                    Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                                    SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Other Configuration">
                                                </ComponentArt:TabStripTab>
                                                <ComponentArt:TabStripTab ID="TabStripTab2" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                                    ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                                    DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                                    Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                                    SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Heart Beat Configuration">
                                                </ComponentArt:TabStripTab>
                                                <ComponentArt:TabStripTab ID="TabStripTab3" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                                    ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                                    DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                                    Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                                    SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Batch Configuration">
                                                </ComponentArt:TabStripTab>
                                                <ComponentArt:TabStripTab ID="TabStripTab5" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                                    ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                                    DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                                    Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                                    SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Dispensed Logic Configuration">
                                                </ComponentArt:TabStripTab>
                                            </Tabs>
                                        </ComponentArt:TabStrip>
                                        <ComponentArt:MultiPage ID="MultiPage2" CssClass="MultiPage" runat="server">
                                            <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView2">
                                                <table id="Table5" border="0" cellpadding="0" cellspacing="0" style="padding-left: 10px"
                                                    width="90%">
                                                    <tr>
                                                        <td colspan="2" valign="top">
                                                            <table>
                                                               
                                                                <tr>
                                                                    <td>Init EJ Execution Time(Hour & Minutes)
                                                                    </td>
                                                                    <td>
                                                                        <asp:DropDownList ID="DropDownList_Hour" runat="server" CssClass="textbox" Width="70px">
                                                                        </asp:DropDownList>
                                                                        <asp:DropDownList ID="DropDownList_Min" runat="server" CssClass="textbox" Width="70px">
                                                                        </asp:DropDownList>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>Debug Level
                                                                    </td>
                                                                    <td>
                                                                        <asp:DropDownList ID="DropDownList_DebugLevel" runat="server" CssClass="textbox"
                                                                            Width="100px">
                                                                            <asp:ListItem>0</asp:ListItem>
                                                                            <asp:ListItem>1</asp:ListItem>
                                                                            <asp:ListItem Selected="True">2</asp:ListItem>
                                                                            <asp:ListItem>3</asp:ListItem>
                                                                            <asp:ListItem>4</asp:ListItem>
                                                                        </asp:DropDownList>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>Startup Sleep Interval
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="TextBox_SleepInterval" Text="60" runat="server" CssClass="textbox"
                                                                            Width="100px"></asp:TextBox>
                                                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator34" runat="server"
                                                                            ValidationExpression="\d+" ControlToValidate="TextBox_SleepInterval" ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator_SleepInterval" runat="server"
                                                                            ControlToValidate="TextBox_SleepInterval" ErrorMessage="Please enter startup sleep interval">*</asp:RequiredFieldValidator>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>Counts Clear Retries
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="TextBox_CountsClearRetries" runat="server" CssClass="textbox" Width="100px"
                                                                            Text="3"></asp:TextBox>
                                                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator41" runat="server"
                                                                            ValidationExpression="\d+" ControlToValidate="TextBox_CountsClearRetries" ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="TextBox_CountsClearRetries"
                                                                            ErrorMessage="Please enter counts clear retries">*</asp:RequiredFieldValidator>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>TCP Timeout
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="TextBox_TCPTimeout" runat="server" CssClass="textbox" Width="100px"
                                                                            Text="20000"></asp:TextBox>
                                                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator42" runat="server"
                                                                            ValidationExpression="\d+" ControlToValidate="TextBox_TCPTimeout" ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="TextBox_TCPTimeout"
                                                                            ErrorMessage="Please enter TCP timeout">*</asp:RequiredFieldValidator>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>Delay between each iteration
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="TextBox_DelayIteration" runat="server" CssClass="textbox" Width="100px"
                                                                            Text="1000"></asp:TextBox>
                                                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator47" runat="server"
                                                                            ValidationExpression="\d+" ControlToValidate="TextBox_DelayIteration" ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="TextBox_DelayIteration"
                                                                            ErrorMessage="Please enter Sleep Interval">*</asp:RequiredFieldValidator>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>CPM Command
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="TextBox_CPMCommand" runat="server" CssClass="textbox" Width="400px"></asp:TextBox>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>CPM Command Wait
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="TextBox_CPMCommandWait" runat="server" CssClass="textbox" Width="100px"
                                                                            Text="30000"></asp:TextBox>
                                                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator48" runat="server"
                                                                            ValidationExpression="\d+" ControlToValidate="TextBox_CPMCommandWait" ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ControlToValidate="TextBox_CPMCommandWait"
                                                                            ErrorMessage="Please enter CPM Command Wait">*</asp:RequiredFieldValidator>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>CPM Command Sleep
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="TextBox_CPMCommandSleep" runat="server" CssClass="textbox" Width="100px"
                                                                            Text="60000"></asp:TextBox>
                                                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator49" runat="server"
                                                                            ValidationExpression="\d+" ControlToValidate="TextBox_CPMCommandSleep" ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" ControlToValidate="TextBox_CPMCommandSleep"
                                                                            ErrorMessage="Please enter CPM Command Sleep">*</asp:RequiredFieldValidator>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>Out Of Cash Threshold
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="TextBox_OutOfCashThreshold" runat="server" CssClass="textbox" Width="100px"
                                                                            Text="100000"></asp:TextBox>
                                                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator68" runat="server"
                                                                            ValidationExpression="\d+" ControlToValidate="TextBox_CPMCommandSleep" ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>AANDCApplications1
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="TextBox_AANDCApplications1" runat="server" CssClass="textbox" Width="400px"
                                                                            Text="SSDSAPP_applicationcore,"></asp:TextBox>
                                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" ControlToValidate="TextBox_AANDCApplications1"
                                                                            ErrorMessage="Please enter AANDCApplications1">*</asp:RequiredFieldValidator>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>AANDCApplications2
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="TextBox_AANDCApplications2" runat="server" CssClass="textbox" Width="400px"
                                                                            Text="SSDSAPP_applicationcore,"></asp:TextBox>
                                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator9" runat="server" ControlToValidate="TextBox_AANDCApplications2"
                                                                            ErrorMessage="Please enter AANDCApplications2">*</asp:RequiredFieldValidator>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>AANDCApplications3
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="TextBox_AANDCApplications3" runat="server" CssClass="textbox" Width="400px"
                                                                            Text="SSDSAPP_applicationcore,"></asp:TextBox>
                                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator10" runat="server" ControlToValidate="TextBox_AANDCApplications3"
                                                                            ErrorMessage="Please enter AANDCApplications2">*</asp:RequiredFieldValidator>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>AANDCApplications4
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="TextBox_AANDCApplications4" runat="server" CssClass="textbox" Width="400px"
                                                                            Text="SSDSAPP_applicationcore,"></asp:TextBox>
                                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator11" runat="server" ControlToValidate="TextBox_AANDCApplications4"
                                                                            ErrorMessage="Please enter AANDCApplications4">*</asp:RequiredFieldValidator>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>AANDCApplications5
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="TextBox_AANDCApplications5" runat="server" CssClass="textbox" Width="400px"
                                                                            Text="SSDSAPP_applicationcore,"></asp:TextBox>
                                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator13" runat="server" ControlToValidate="TextBox_AANDCApplications5"
                                                                            ErrorMessage="Please enter AANDCApplications5">*</asp:RequiredFieldValidator>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>Monitoring Retries
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="TextBox_MonitoringRetries" runat="server" CssClass="textbox" Width="100px"
                                                                            Text="5"></asp:TextBox>
                                                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator55" runat="server"
                                                                            ValidationExpression="\d+" ControlToValidate="TextBox_MonitoringRetries" ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator17" runat="server" ControlToValidate="TextBox_MonitoringRetries"
                                                                            ErrorMessage="Please enter Monitoring Retries">*</asp:RequiredFieldValidator>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>Window Switch Sleep
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="TextBox_WindowSwitch_Sleep" runat="server" CssClass="textbox" Width="100px"
                                                                            Text="500"></asp:TextBox>
                                                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator56" runat="server"
                                                                            ValidationExpression="\d+" ControlToValidate="TextBox_WindowSwitch_Sleep" ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator18" runat="server" ControlToValidate="TextBox_WindowSwitch_Sleep"
                                                                            ErrorMessage="Please enter Window Switch Sleep">*</asp:RequiredFieldValidator>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>Application Switch Sleep
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="TextBox_AppSwitch_Sleep" runat="server" CssClass="textbox" Width="100px"
                                                                            Text="60000"></asp:TextBox>
                                                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator57" runat="server"
                                                                            ValidationExpression="\d+" ControlToValidate="TextBox_AppSwitch_Sleep" ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator19" runat="server" ControlToValidate="TextBox_AppSwitch_Sleep"
                                                                            ErrorMessage="Please enter Application Switch Sleep">*</asp:RequiredFieldValidator>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>Monitoring Cycle Sleep
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="TextBox_MonitoringCycle_Sleep" runat="server" CssClass="textbox"
                                                                            Text="5" Width="100px"></asp:TextBox>
                                                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator58" runat="server"
                                                                            ValidationExpression="\d+" ControlToValidate="TextBox_MonitoringCycle_Sleep"
                                                                            ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator20" runat="server" ControlToValidate="TextBox_MonitoringCycle_Sleep"
                                                                            ErrorMessage="Please enter Monitoring Cycle Sleep">*</asp:RequiredFieldValidator>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>CPM Log Level
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="TextBox_CPMLogLevel" runat="server" CssClass="textbox" Width="100px"
                                                                            Text="4"></asp:TextBox>
                                                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator59" runat="server"
                                                                            ValidationExpression="\d+" ControlToValidate="TextBox_CPMLogLevel" ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator21" runat="server" ControlToValidate="TextBox_CPMLogLevel"
                                                                            ErrorMessage="Please enter CPM Log Level">*</asp:RequiredFieldValidator>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                        <td valign="top">
                                                            <table>
                                                                <tr>
                                                                    <td>Distribution Port
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="TextBox_DistributionPort" runat="server" CssClass="textbox" Text="0"></asp:TextBox>
                                                                        <asp:RegularExpressionValidator ID="regularexpressionvalidator89" runat="server" ControlToValidate="TextBox_DistributionPort"
                                                                            ErrorMessage="Only Numbers are allowed in Port." ValidationExpression="[0-9]*"
                                                                            Text="*"></asp:RegularExpressionValidator>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>On Us Amount
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="TextBox_OnUsAmount" runat="server" CssClass="textbox" Text="0"></asp:TextBox>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>Not On Us Amount
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="TextBox_NotOnUsAmount" runat="server" CssClass="textbox" Text="0"></asp:TextBox>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>Normal Order Cost
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="TextBox_NormalOrderCost" runat="server" Text="0" CssClass="textbox"></asp:TextBox>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>Emergency Order Cost
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="TextBox_EmergencyOrderCost" runat="server" Text="0" CssClass="textbox"></asp:TextBox>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>Reciept Cut Off
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="TextBox_ReceiptCutOff" runat="server" Text="0" CssClass="textbox"></asp:TextBox>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>Dispenser Real Time Notification Enabled
                                                                    </td>
                                                                    <td>
                                                                        <asp:CheckBox ID="CheckBox_DispenserRealTimeEnabled" runat="server" />
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>BNA Real Time Notification Enabled
                                                                    </td>
                                                                    <td>
                                                                        <asp:CheckBox ID="CheckBox_BNARealTimeEnabled" runat="server" />
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>CPM Real Time Notification Enabled
                                                                    </td>
                                                                    <td>
                                                                        <asp:CheckBox ID="CheckBox_CPMRealTimeEnabled" runat="server" />
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>Replenishment Real Time Notification Enabled
                                                                    </td>
                                                                    <td>
                                                                        <asp:CheckBox ID="CheckBox_RepRealTimeEnabled" runat="server" />
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>Out Of Cash Real Time Notification Enabled
                                                                    </td>
                                                                    <td>
                                                                        <asp:CheckBox ID="CheckBox_OutOfCashRealTimeEnabled" runat="server" />
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>Dispenser Mismatch Real Time Notification Enabled
                                                                    </td>
                                                                    <td>
                                                                        <asp:CheckBox ID="CheckBox_DisMismatchRealTimeEnabled" runat="server" />
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>BNA Mismatch Real Time Notification Enabled
                                                                    </td>
                                                                    <td>
                                                                        <asp:CheckBox ID="CheckBox_BNAMismatchRealTimeEnabled" runat="server" />
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>CPM Mismatch Real Time Notification Enabled
                                                                    </td>
                                                                    <td>
                                                                        <asp:CheckBox ID="CheckBox_CPMMismatchRealTimeEnabled" runat="server" />
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>Counter Exploded Real Time Notification Enabled
                                                                    </td>
                                                                    <td>
                                                                        <asp:CheckBox ID="CheckBox_CounterExplodedRealTimeEnabled" runat="server" />
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>Type 1 Minimum Notes
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="TextBox_Type1MinimumNotes" runat="server" CssClass="textbox" Width="100px"
                                                                            Text="1000"></asp:TextBox>
                                                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator60" runat="server"
                                                                            ValidationExpression="\d+" ControlToValidate="TextBox_Type1MinimumNotes" ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator22" runat="server" ControlToValidate="TextBox_Type1MinimumNotes"
                                                                            ErrorMessage="Please enter Type 1 Minimum number of notes">*</asp:RequiredFieldValidator>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>Type 2 Minimum Notes
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="TextBox_Type2MinimumNotes" runat="server" CssClass="textbox" Width="100px"
                                                                            Text="1000"></asp:TextBox>
                                                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator61" runat="server"
                                                                            ValidationExpression="\d+" ControlToValidate="TextBox_Type2MinimumNotes" ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator23" runat="server" ControlToValidate="TextBox_Type2MinimumNotes"
                                                                            ErrorMessage="Please enter Type 2 Minimum number of notes">*</asp:RequiredFieldValidator>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>Type 3 Minimum Notes
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="TextBox_Type3MinimumNotes" runat="server" CssClass="textbox" Width="100px"
                                                                            Text="1000"></asp:TextBox>
                                                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator62" runat="server"
                                                                            ValidationExpression="\d+" ControlToValidate="TextBox_Type3MinimumNotes" ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator24" runat="server" ControlToValidate="TextBox_Type3MinimumNotes"
                                                                            ErrorMessage="Please enter Type 3 Minimum number of notes">*</asp:RequiredFieldValidator>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>Type 4 Minimum Notes
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="TextBox_Type4MinimumNotes" runat="server" CssClass="textbox" Width="100px"
                                                                            Text="1000"></asp:TextBox>
                                                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator63" runat="server"
                                                                            ValidationExpression="\d+" ControlToValidate="TextBox_Type4MinimumNotes" ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator25" runat="server" ControlToValidate="TextBox_Type4MinimumNotes"
                                                                            ErrorMessage="Please enter Type 4 Minimum number of notes">*</asp:RequiredFieldValidator>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>Type 5 Minimum Notes
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="TextBox_Type5MinimumNotes" runat="server" CssClass="textbox" Width="100px"
                                                                            Text="1000"></asp:TextBox>
                                                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator64" runat="server"
                                                                            ValidationExpression="\d+" ControlToValidate="TextBox_Type5MinimumNotes" ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator26" runat="server" ControlToValidate="TextBox_Type5MinimumNotes"
                                                                            ErrorMessage="Please enter Type 5 Minimum number of notes">*</asp:RequiredFieldValidator>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>Type 6 Minimum Notes
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="TextBox_Type6MinimumNotes" runat="server" CssClass="textbox" Width="100px"
                                                                            Text="1000"></asp:TextBox>
                                                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator65" runat="server"
                                                                            ValidationExpression="\d+" ControlToValidate="TextBox_Type6MinimumNotes" ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator27" runat="server" ControlToValidate="TextBox_Type6MinimumNotes"
                                                                            ErrorMessage="Please enter Type 6 Minimum number of notes">*</asp:RequiredFieldValidator>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>Type 7 Minimum Notes
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="TextBox_Type7MinimumNotes" runat="server" CssClass="textbox" Width="100px"
                                                                            Text="1000"></asp:TextBox>
                                                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator66" runat="server"
                                                                            ValidationExpression="\d+" ControlToValidate="TextBox_Type7MinimumNotes" ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator28" runat="server" ControlToValidate="TextBox_Type7MinimumNotes"
                                                                            ErrorMessage="Please enter Type 7 Minimum number of notes">*</asp:RequiredFieldValidator>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </ComponentArt:PageView>
                                            <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView5">
                                                <table id="Table8" border="0" cellpadding="0" cellspacing="0" style="padding-left: 10px"
                                                    width="40%">
                                                    <tr>
                                                        <td>
                                                            <b>Event Type</b>
                                                        </td>
                                                        <td>
                                                            <b>Interval</b>
                                                        </td>
                                                        <td></td>
                                                    </tr>
                                                    <tr>
                                                        <td>&nbsp;
                                                        </td>
                                                    </tr>
                                                    <tr style="background-color: InfoBackground">
                                                        <td>
                                                            <asp:ListBox ID="ListBox_Events" runat="server" CssClass="dropDown" Height="100px">
                                                                <asp:ListItem>CashWithdrawal</asp:ListItem>
                                                                <asp:ListItem>CashDeposit</asp:ListItem>
                                                                <asp:ListItem>ChequeDeposit</asp:ListItem>
                                                                <asp:ListItem>OrderId</asp:ListItem>
                                                                <asp:ListItem>OrderCounters</asp:ListItem>
                                                                <asp:ListItem>OutOfCash</asp:ListItem>
                                                            </asp:ListBox>
                                                        </td>
                                                        <td style="vertical-align: top">
                                                            <asp:TextBox ID="TextBox_Interval" runat="server" CssClass="textbox"></asp:TextBox>
                                                            <asp:RegularExpressionValidator ID="RegularExpressionValidator51" runat="server"
                                                                ValidationExpression="\d+" ControlToValidate="TextBox_Interval" ErrorMessage="Only numbers are allowed"
                                                                ValidationGroup="HeartBeatSchedule">*</asp:RegularExpressionValidator>
                                                        </td>
                                                        <td style="vertical-align: top">
                                                            <asp:Button ID="Button_Add" runat="server" Text="Add Schedule" CssClass="button"
                                                                OnClick="Button_AddHeartBeatSchedule_Click" ValidationGroup="HeartBeatSchedule" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>&nbsp;
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="3">
                                                            <asp:GridView ID="GridView_HeartBeatSchedules" runat="server" AutoGenerateColumns="False"
                                                                AutoGenerateDeleteButton="true" OnRowDeleting="GridView_HeartBeatSchedule_RowDeleting"
                                                                Width="100%" HorizontalAlign="Center">
                                                                <Columns>
                                                                    <asp:BoundField DataField="type" HeaderText="Type" />
                                                                    <asp:BoundField DataField="interval" HeaderText="Interval" ItemStyle-HorizontalAlign="Right" />
                                                                </Columns>
                                                            </asp:GridView>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </ComponentArt:PageView>
                                            <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView4">
                                                <table id="Table7" border="0" cellpadding="0" cellspacing="0" style="padding-left: 10px"
                                                    width="40%">
                                                    <tr>
                                                        <td>
                                                            <b>Type</b>
                                                        </td>
                                                        <td>
                                                            <b>Interval</b>
                                                        </td>
                                                        <td></td>
                                                    </tr>
                                                    <tr>
                                                        <td>&nbsp;
                                                        </td>
                                                    </tr>
                                                    <tr style="background-color: InfoBackground">
                                                        <td>
                                                            <asp:ListBox ID="ListBox_Batch" runat="server" CssClass="dropDown">
                                                                <asp:ListItem>CashWithdrawal</asp:ListItem>
                                                                <asp:ListItem>CashDeposit</asp:ListItem>
                                                                <asp:ListItem>ChequeDeposit</asp:ListItem>
                                                                <asp:ListItem>Replenishment</asp:ListItem>
                                                            </asp:ListBox>
                                                        </td>
                                                        <td style="vertical-align: top">
                                                            <asp:TextBox ID="TextBox_BatchInterval" runat="server" CssClass="textbox"></asp:TextBox>
                                                            <asp:RegularExpressionValidator ID="RegularExpressionValidator52" runat="server"
                                                                ValidationExpression="\d+" ValidationGroup="BatchSchedule" ControlToValidate="TextBox_BatchInterval"
                                                                ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                        </td>
                                                        <td style="vertical-align: top">
                                                            <asp:Button ID="Button_AddBatchSchedule" runat="server" Text="Add Schedule" CssClass="button"
                                                                OnClick="Button_AddBatchSchedule_Click" ValidationGroup="BatchSchedule" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>&nbsp;
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="3">
                                                            <asp:GridView ID="GridView_Batchschedule" runat="server" AutoGenerateColumns="False"
                                                                AutoGenerateDeleteButton="true" OnRowDeleting="GridView_BatchSchedule_RowDeleting"
                                                                HorizontalAlign="Center" Width="100%">
                                                                <Columns>
                                                                    <asp:BoundField DataField="type" HeaderText="Type" />
                                                                    <asp:BoundField DataField="interval" HeaderText="Interval" ItemStyle-HorizontalAlign="Right" />
                                                                </Columns>
                                                            </asp:GridView>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </ComponentArt:PageView>
                                            <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView6">
                                                <table id="Table9" border="0" cellpadding="0" cellspacing="0" style="padding-left: 10px"
                                                    width="80%">
                                                    <tr>
                                                        <td>Sampling Size
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="TextBox_SampleSize" runat="server" CssClass="textbox"></asp:TextBox>
                                                            <asp:RegularExpressionValidator ID="RegularExpressionValidator74" runat="server"
                                                                ValidationExpression="\d+" ValidationGroup="DispensedConf" ControlToValidate="TextBox_SampleSize"
                                                                ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>Operator
                                                        </td>
                                                        <td>
                                                            <asp:DropDownList ID="DropDownList_Operator" runat="server" CssClass="dropDown">
                                                                <%--<asp:ListItem Text="Greator than equal to" Value="Greator than equal to"></asp:ListItem>--%>
                                                                <asp:ListItem Text="Greator than" Value="Greator than"></asp:ListItem>
                                                                <%--<asp:ListItem Text="Less than equal to" Value="Less than equal to"></asp:ListItem>
                                                            <asp:ListItem Text="Less than" Value="Less than"></asp:ListItem>--%>
                                                            </asp:DropDownList>
                                                        </td>
                                                        <td>Amount
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="TextBox_Amount" runat="server" CssClass="textbox"></asp:TextBox>
                                                            <asp:RegularExpressionValidator ID="RegularExpressionValidator73" runat="server"
                                                                ValidationExpression="\d+" ValidationGroup="DispensedConf" ControlToValidate="TextBox_Amount"
                                                                ErrorMessage="Only numbers are allowed">*</asp:RegularExpressionValidator>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <b>Type</b>
                                                        </td>
                                                        <td>
                                                            <b>Notes Can be Dispensed</b>
                                                        </td>
                                                        <td></td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="Label_NoteSetType1Title" runat="server"></asp:Label>
                                                        </td>
                                                        <td>
                                                            <asp:CheckBox ID="CheckBox_NotesDispensedType1" runat="server" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="Label_NoteSetType2Title" runat="server"></asp:Label>
                                                        </td>
                                                        <td>
                                                            <asp:CheckBox ID="CheckBox_NotesDispensedType2" runat="server" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="Label_NoteSetType3Title" runat="server"></asp:Label>
                                                        </td>
                                                        <td>
                                                            <asp:CheckBox ID="CheckBox_NotesDispensedType3" runat="server" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="Label_NoteSetType4Title" runat="server"></asp:Label>
                                                        </td>
                                                        <td>
                                                            <asp:CheckBox ID="CheckBox_NotesDispensedType4" runat="server" />
                                                        </td>
                                                    </tr>
                                                    <tr style="background-color: InfoBackground">
                                                        <td colspan="2" style="vertical-align: top; text-align: left">
                                                            <asp:Button ID="Button_AddDispensedConf" runat="server" Text="Add Schedule" CssClass="button"
                                                                OnClick="Button_AddDispensedConf_Click" ValidationGroup="DispensedConf" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>&nbsp;
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="8">
                                                            <asp:GridView ID="GridView_DispensedConf" runat="server" AutoGenerateColumns="False"
                                                                AutoGenerateDeleteButton="true" OnRowDeleting="GridView_DispensedConf_RowDeleting"
                                                                HorizontalAlign="Center" Width="100%">
                                                                <Columns>
                                                                    <asp:BoundField DataField="operator" HeaderText="Operator" ItemStyle-HorizontalAlign="Left" />
                                                                    <asp:BoundField DataField="amount" HeaderText="Amount" ItemStyle-HorizontalAlign="Right" />
                                                                    <asp:CheckBoxField DataField="type1" HeaderText="Type1" ItemStyle-HorizontalAlign="Right" />
                                                                    <asp:CheckBoxField DataField="type2" HeaderText="Type2" ItemStyle-HorizontalAlign="Right" />
                                                                    <asp:CheckBoxField DataField="type3" HeaderText="Type3" ItemStyle-HorizontalAlign="Right" />
                                                                    <asp:CheckBoxField DataField="type4" HeaderText="Type4" ItemStyle-HorizontalAlign="Right" />
                                                                </Columns>
                                                            </asp:GridView>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </ComponentArt:PageView>
                                        </ComponentArt:MultiPage>
                                    </td>
                                </tr>
                            </table>
                        </ComponentArt:PageView>
                        <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView7">
                            <table id="Table2" border="0" cellpadding="0" cellspacing="0" style="padding-left: 10px"
                                width="100%">
                                <tr>
                                    <td>
                                        <ComponentArt:TabStrip ID="TabStrip1" CssClass="TopGroup" DefaultItemLookId="DefaultTabLook"
                                            DefaultSelectedItemLookId="SelectedTabLook" DefaultDisabledItemLookId="DisabledTabLook"
                                            DefaultGroupTabSpacing="1px" ImagesBaseUrl="../images/Tabimages/" MultiPageId="MultiPageInventory"
                                            runat="server" ScrollDownLookId="DefaultTabLook" ScrollLeftLookId="DefaultTabLook"
                                            ScrollRightLookId="DefaultTabLook" ScrollUpLookId="DefaultTabLook" TopGroupTabSpacing="1px"
                                            Width="100%">
                                            <ItemLooks>
                                                <ComponentArt:ItemLook LookId="DefaultTabLook" CssClass="DefaultTab" HoverCssClass="DefaultTabHover"
                                                    LabelPaddingLeft="10px" LabelPaddingRight="10px" LabelPaddingTop="5px" LabelPaddingBottom="4px"
                                                    LeftIconUrl="tab_left_icon.gif" RightIconUrl="tab_right_icon.gif" HoverLeftIconUrl="hover_tab_left_icon.gif"
                                                    HoverRightIconUrl="hover_tab_right_icon.gif" LeftIconWidth="3px" LeftIconHeight="21px"
                                                    RightIconWidth="3px" RightIconHeight="21px" LeftIconVisibility="Always" RightIconVisibility="Always" />
                                                <ComponentArt:ItemLook LookId="SelectedTabLook" CssClass="SelectedReportsTab" LabelPaddingLeft="10px"
                                                    LabelPaddingRight="10px" LabelPaddingTop="4px" LabelPaddingBottom="4px" LeftIconUrl="selected_tab_left_icon.gif"
                                                    RightIconUrl="selected_tab_right_icon.gif" LeftIconWidth="3px" LeftIconHeight="21px"
                                                    RightIconWidth="3px" RightIconHeight="21px" LeftIconVisibility="Always" RightIconVisibility="Always" />
                                            </ItemLooks>
                                            <Tabs>
                                                <ComponentArt:TabStripTab ID="TabStripTab7" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                                    ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                                    DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                                    Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                                    SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="General">
                                                </ComponentArt:TabStripTab>
                                                <ComponentArt:TabStripTab ID="TabStripTab8" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                                    ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                                    DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                                    Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                                    SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Hardware">
                                                </ComponentArt:TabStripTab>
                                                <ComponentArt:TabStripTab ID="TabStripTab9" runat="server" ChildSelectedLook-LeftIconVisibility="Always"
                                                    ChildSelectedLook-RightIconVisibility="Always" DefaultSubGroupTabSpacing="1px"
                                                    DisabledLook-LeftIconVisibility="Always" DisabledLook-RightIconVisibility="Always"
                                                    Look-LeftIconVisibility="Always" Look-RightIconVisibility="Always" SelectedLook-LeftIconVisibility="Always"
                                                    SelectedLook-RightIconVisibility="Always" SubGroupTabSpacing="1px" Text="Software">
                                                </ComponentArt:TabStripTab>
                                            </Tabs>
                                        </ComponentArt:TabStrip>
                                        <ComponentArt:MultiPage ID="MultiPageInventory" CssClass="MultiPage" runat="server">
                                            <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView8">
                                                <table id="Table10" border="0" cellpadding="0" cellspacing="0" style="padding-left: 10px"
                                                    width="100%">
                                                    <tr>
                                                        <td>
                                                            <table>
                                                                <asp:Literal ID="Literal_GeneralInfo" runat="server"></asp:Literal>
                                                            </table>
                                                        </td>
                                                        <%--<td style="vertical-align: top;" align="right">
                                                        <table>
                                                            <tr>
                                                                <td>
                                                                    <img src="../images/atm.jpg" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>--%>
                                                    </tr>
                                                </table>
                                            </ComponentArt:PageView>
                                            <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView9">
                                                <table id="TableHardware" border="0" cellpadding="0" cellspacing="0" style="padding-left: 10px"
                                                    width="100%">
                                                    <tr>
                                                        <td valign="top">
                                                            <table width="60%">
                                                                <tr>
                                                                    <td style="text-decoration: underline; vertical-align: top">
                                                                        <b>Devices </b>
                                                                    </td>
                                                                </tr>
                                                                <asp:Literal ID="Literal_HardwareInfo" runat="server"></asp:Literal>
                                                            </table>
                                                        </td>
                                                        <%--<td style="vertical-align: top;" align="right">
                                                        <table>
                                                            <tr>
                                                                <td>
                                                                    <img src="../images/hardware.png" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>--%>
                                                    </tr>
                                                </table>
                                            </ComponentArt:PageView>
                                            <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView10">
                                                <table id="TableSoftware" border="0" cellpadding="0" cellspacing="0" style="padding-left: 10px"
                                                    width="100%">
                                                    <tr>
                                                        <td valign="top">
                                                            <table>
                                                                <tr>
                                                                    <td style="text-decoration: underline">
                                                                        <b>Installed Softwares</b>
                                                                    </td>
                                                                </tr>
                                                                <asp:Literal ID="Literal_InstalledSoftware" runat="server"></asp:Literal>
                                                            </table>
                                                        </td>
                                                        <%-- <td style="vertical-align: top;" align="right">
                                                        <img src="../images/installedSoftware.png" />
                                                    </td>--%>
                                                    </tr>
                                                </table>
                                            </ComponentArt:PageView>
                                        </ComponentArt:MultiPage>
                                    </td>
                                </tr>
                            </table>
                        </ComponentArt:PageView>
                        <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView12">
                            <table id="Table12" border="0" cellpadding="0" cellspacing="0" style="padding-left: 10px"
                                width="100%">
                                <tr>
                                    <td>Interface Friendly Name 1
                                    </td>
                                    <td>
                                        <asp:TextBox ID="TextBox_InterfaceFriendlyName1" runat="server" CssClass="textbox"
                                            Width="100px"></asp:TextBox>
                                    </td>
                                    <td>IP 1
                                    </td>
                                    <td>
                                        <asp:TextBox ID="TextBox_IP1" runat="server" CssClass="textbox" Width="100px"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>Interface Friendly Name 2
                                    </td>
                                    <td>
                                        <asp:TextBox ID="TextBox_InterfaceFriendlyName2" runat="server" CssClass="textbox"
                                            Width="100px"></asp:TextBox>
                                    </td>
                                    <td>IP 2
                                    </td>
                                    <td>
                                        <asp:TextBox ID="TextBox_IP2" runat="server" CssClass="textbox" Width="100px"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>Interface Friendly Name 3
                                    </td>
                                    <td>
                                        <asp:TextBox ID="TextBox_InterfaceFriendlyName3" runat="server" CssClass="textbox"
                                            Width="100px"></asp:TextBox>
                                    </td>
                                    <td>IP 3
                                    </td>
                                    <td>
                                        <asp:TextBox ID="TextBox_IP3" runat="server" CssClass="textbox" Width="100px"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>Interface Friendly Name 4
                                    </td>
                                    <td>
                                        <asp:TextBox ID="TextBox_InterfaceFriendlyName4" runat="server" CssClass="textbox"
                                            Width="100px"></asp:TextBox>
                                    </td>
                                    <td>IP 4
                                    </td>
                                    <td>
                                        <asp:TextBox ID="TextBox_IP4" runat="server" CssClass="textbox" Width="100px"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>Interface Friendly Name 5
                                    </td>
                                    <td>
                                        <asp:TextBox ID="TextBox_InterfaceFriendlyName5" runat="server" CssClass="textbox"
                                            Width="100px"></asp:TextBox>
                                    </td>
                                    <td>IP 5
                                    </td>
                                    <td>
                                        <asp:TextBox ID="TextBox_IP5" runat="server" CssClass="textbox" Width="100px"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </ComponentArt:PageView>
                        <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView13" Width="100%">
                            <table width="100%" border="0">
                                <tr>
                                    <td>
                                        <fieldset>
                                            <legend><b>Normal Days</b> </legend>
                                            <table id="Table13" border="0" cellpadding="0" cellspacing="0" style="padding-left: 10px; padding-right: 20px"
                                                width="100%">
                                                <tr>
                                                    <td>Suggested Order For Type 1
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="TextBox_NormalDaySuggestedNotesForType1" runat="server" CssClass="textbox"
                                                            Width="100px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>Suggested Order For Type 2
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="TextBox_NormalDaySuggestedNotesForType2" runat="server" CssClass="textbox"
                                                            Width="100px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>Suggested Order For Type 3
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="TextBox_NormalDaySuggestedNotesForType3" runat="server" CssClass="textbox"
                                                            Width="100px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>Suggested Order For Type 4
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="TextBox_NormalDaySuggestedNotesForType4" runat="server" CssClass="textbox"
                                                            Width="100px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>Suggested Order For Type 5
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="TextBox_NormalDaySuggestedNotesForType5" runat="server" CssClass="textbox"
                                                            Width="100px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>Suggested Order For Type 6
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="TextBox_NormalDaySuggestedNotesForType6" runat="server" CssClass="textbox"
                                                            Width="100px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>Suggested Order For Type 7
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="TextBox_NormalDaySuggestedNotesForType7" runat="server" CssClass="textbox"
                                                            Width="100px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </fieldset>
                                    </td>
                                    <td>
                                        <fieldset>
                                            <legend><b>Salary Days</b> </legend>
                                            <table id="Table14" border="0" cellpadding="0" cellspacing="0" style="padding-left: 10px; padding-right: 20px"
                                                width="100%">
                                                <tr>
                                                    <td>Suggested Order For Type 1
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="TextBox_SalaryDaySuggestedNotesForType1" runat="server" CssClass="textbox"
                                                            Width="100px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>Suggested Order For Type 2
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="TextBox_SalaryDaySuggestedNotesForType2" runat="server" CssClass="textbox"
                                                            Width="100px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>Suggested Order For Type 3
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="TextBox_SalaryDaySuggestedNotesForType3" runat="server" CssClass="textbox"
                                                            Width="100px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>Suggested Order For Type 4
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="TextBox_SalaryDaySuggestedNotesForType4" runat="server" CssClass="textbox"
                                                            Width="100px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>Suggested Order For Type 5
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="TextBox_SalaryDaySuggestedNotesForType5" runat="server" CssClass="textbox"
                                                            Width="100px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>Suggested Order For Type 6
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="TextBox_SalaryDaySuggestedNotesForType6" runat="server" CssClass="textbox"
                                                            Width="100px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>Suggested Order For Type 7
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="TextBox_SalaryDaySuggestedNotesForType7" runat="server" CssClass="textbox"
                                                            Width="100px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </fieldset>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <table>
                                            <tr>
                                                <td>Avg.Dispensed For Normal Days
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="TextBox_AvgDispensed" runat="server" CssClass="textbox" Width="100px"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Avg.Dispensed For Salary Days
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="TextBox_AvgDispensedSalaryDays" runat="server" CssClass="textbox"
                                                        Width="100px"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Spare Cash
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="TextBox_SpareCash" runat="server" CssClass="textbox" Width="100px"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Dispensing Behavior
                                                </td>
                                                <td>
                                                    <asp:DropDownList ID="DropDownList_DispensingBehavior" runat="server" CssClass="dropDown">
                                                        <asp:ListItem Value="0">Fixed</asp:ListItem>
                                                        <asp:ListItem Value="1">Variable</asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Classification
                                                </td>
                                                <td>
                                                    <asp:CheckBoxList ID="CheckBoxList_Classification" runat="server" RepeatDirection="Horizontal"
                                                        RepeatColumns="7">
                                                    </asp:CheckBoxList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Inactivity Period for Normal Days
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="TextBox_InactivityPeriodNormalDays" runat="server" CssClass="textbox"
                                                        Width="100px"></asp:TextBox>
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator85" runat="server"
                                                        ValidationExpression="\d*" ControlToValidate="TextBox_InactivityPeriodNormalDays"
                                                        SetFocusOnError="true" ErrorMessage="Invalid inactivity period entered for normal days">*</asp:RegularExpressionValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Inactivity Period for Salary Days
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="TextBox_InactivityPeriodSalaryDays" runat="server" CssClass="textbox"
                                                        Width="100px"></asp:TextBox>
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator86" runat="server"
                                                        ValidationExpression="\d*" ControlToValidate="TextBox_InactivityPeriodSalaryDays"
                                                        SetFocusOnError="true" ErrorMessage="Invalid inactivity period entered for salary days">*</asp:RegularExpressionValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Minimum Operating Balance for Normal Days
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="TextBox_MinOperatingBalanceNormalDays" runat="server" CssClass="textbox"
                                                        Width="100px"></asp:TextBox>
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator87" runat="server"
                                                        ValidationExpression="\d*\.\d+" ControlToValidate="TextBox_MinOperatingBalanceNormalDays"
                                                        SetFocusOnError="true" ErrorMessage="Invalid operating balance for normal days">*</asp:RegularExpressionValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Minimum Operating Balance for Salary Days
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="TextBox_MinOperatingBalanceSalaryDays" runat="server" CssClass="textbox"
                                                        Width="100px"></asp:TextBox>
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator88" runat="server"
                                                        ValidationExpression="\d*.\d+" ControlToValidate="TextBox_MinOperatingBalanceSalaryDays"
                                                        SetFocusOnError="true" ErrorMessage="Invalid operating balance for salary days">*</asp:RegularExpressionValidator>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </ComponentArt:PageView>
                        <ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageView11">
                            <table>
                                <tr>
                                    <td colspan="2" style="height: 34px" valign="middle">
                                        <span style="color: Navy; font-weight: bold;">Configuration For Operational Hour
                                        </span>&nbsp;
                                    </td>
                                </tr>
                                <tr>
                                    <td>Sunday
                                    </td>
                                    <td>From Hours:&nbsp;<asp:DropDownList ID="DropDownList_FromDay1Hour" runat="server" CssClass="dropDown">
                                    </asp:DropDownList>
                                        From Minutes:&nbsp;<asp:DropDownList ID="DropDownList_FromDay1Min" CssClass="dropDown" runat="server" />

                                        To Hours:&nbsp;<asp:DropDownList ID="DropDownList_ToDay1Hour" runat="server" CssClass="dropDown">
                                        </asp:DropDownList>
                                        To Minutes:&nbsp;<asp:DropDownList ID="DropDownList_ToDay1Min" CssClass="dropDown" runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>Monday
                                    </td>
                                    <td>From Hours:&nbsp;<asp:DropDownList ID="DropDownList_FromDay2Hour" runat="server" CssClass="dropDown">
                                    </asp:DropDownList>
                                        From Minutes:&nbsp;<asp:DropDownList ID="DropDownList_FromDay2Min" CssClass="dropDown" runat="server" />

                                        To Hours:&nbsp;<asp:DropDownList ID="DropDownList_ToDay2Hour" runat="server" CssClass="dropDown">
                                        </asp:DropDownList>
                                        To Minutes:&nbsp;<asp:DropDownList ID="DropDownList_ToDay2Min" CssClass="dropDown" runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>Tuesday
                                    </td>
                                    <td>From Hours:&nbsp;<asp:DropDownList ID="DropDownList_FromDay3Hour" runat="server" CssClass="dropDown">
                                    </asp:DropDownList>
                                        From Minutes:&nbsp;<asp:DropDownList ID="DropDownList_FromDay3Min" CssClass="dropDown" runat="server" />

                                        To Hours:&nbsp;<asp:DropDownList ID="DropDownList_ToDay3Hour" runat="server" CssClass="dropDown">
                                        </asp:DropDownList>
                                        To Minutes:&nbsp;<asp:DropDownList ID="DropDownList_ToDay3Min" CssClass="dropDown" runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>Wednesday
                                    </td>
                                    <td>From Hours:&nbsp;<asp:DropDownList ID="DropDownList_FromDay4Hour" runat="server" CssClass="dropDown">
                                    </asp:DropDownList>
                                        From Minutes:&nbsp;<asp:DropDownList ID="DropDownList_FromDay4Min" CssClass="dropDown" runat="server" />

                                        To Hours:&nbsp;<asp:DropDownList ID="DropDownList_ToDay4Hour" runat="server" CssClass="dropDown">
                                        </asp:DropDownList>
                                        To Minutes:&nbsp;<asp:DropDownList ID="DropDownList_ToDay4Min" CssClass="dropDown" runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>Thursday
                                    </td>
                                    <td>From Hours:&nbsp;<asp:DropDownList ID="DropDownList_FromDay5Hour" runat="server" CssClass="dropDown">
                                    </asp:DropDownList>
                                        From Minutes:&nbsp;<asp:DropDownList ID="DropDownList_FromDay5Min" CssClass="dropDown" runat="server" />

                                        To Hours:&nbsp;<asp:DropDownList ID="DropDownList_ToDay5Hour" runat="server" CssClass="dropDown">
                                        </asp:DropDownList>
                                        To Minutes:&nbsp;<asp:DropDownList ID="DropDownList_ToDay5Min" CssClass="dropDown" runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>Friday
                                    </td>
                                    <td>From Hours:&nbsp;<asp:DropDownList ID="DropDownList_FromDay6Hour" runat="server" CssClass="dropDown">
                                    </asp:DropDownList>
                                        From Minutes:&nbsp;<asp:DropDownList ID="DropDownList_FromDay6Min" CssClass="dropDown" runat="server" />

                                        To Hours:&nbsp;<asp:DropDownList ID="DropDownList_ToDay6Hour" runat="server" CssClass="dropDown">
                                        </asp:DropDownList>
                                        To Minutes:&nbsp;<asp:DropDownList ID="DropDownList_ToDay6Min" CssClass="dropDown" runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>Saturday
                                    </td>
                                    <td>From Hours:&nbsp;<asp:DropDownList ID="DropDownList_FromDay7Hour" runat="server" CssClass="dropDown">
                                    </asp:DropDownList>
                                        From Minutes:&nbsp;<asp:DropDownList ID="DropDownList_FromDay7Min" CssClass="dropDown" runat="server" />

                                        To Hours:&nbsp;<asp:DropDownList ID="DropDownList_ToDay7Hour" runat="server" CssClass="dropDown">
                                        </asp:DropDownList>
                                        To Minutes:&nbsp;<asp:DropDownList ID="DropDownList_ToDay7Min" CssClass="dropDown" runat="server" />
                                    </td>
                                </tr>
                            </table>
                        </ComponentArt:PageView>
                    </ComponentArt:MultiPage>
                </td>
            </tr>
            <tr>
                <td colspan="2" style="font-weight: bold; padding-top: 30px;">
                    <asp:Button ID="Button_SaveUpdateATM" runat="server" Text="Save" OnClick="Button_SaveUpdateATM_Click"
                        CssClass="button" Width="60px" />&nbsp;
                <asp:Button ID="Button_Cancel" runat="server" Text="Cancel" OnClick="Button_Cancel_Click"
                    CssClass="button" Width="60px" CausesValidation="false" />&nbsp;
                <%-- <input id="Button_Cancel" type="button" value="Cancel" onclick="window.location ='ATMs.aspx'"
                    class="button" />--%>
                </td>
            </tr>
        </table>
        <br />
        <br />
        <input type="button" runat="server" id="refresh" style="display: none" />
    </form>
</body>
</html>
