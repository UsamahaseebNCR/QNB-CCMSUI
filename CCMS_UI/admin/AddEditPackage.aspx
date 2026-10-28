<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddEditPackage.aspx.cs"
    Inherits="CCMSUI.CCMS.AddEditPackage" Title="CCMS - Package Managment" EnableEventValidation="false" %>

<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
     <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
   
    <link href="../content/ISUITE.css" rel="stylesheet" />
    <link href="../content/styleSheets/treeStyle.css" rel="stylesheet" type="text/css" />
<%--    <link href="../content/Scripts/ui-lightness/jquery.ui.all.css" rel="stylesheet" type="text/css" />
    <link href="../content/Scripts/ui-lightness/jquery-ui-1.8.17.custom.css" rel="stylesheet" type="text/css" />
    <link href="../content/Scripts/ui-lightness/ui-time-picker.css" rel="stylesheet" type="text/css" />--%>
    <link href="../content/css/bootstrap-responsive.min.css" rel="stylesheet" />
    <link href="../content/css/style.css" rel="stylesheet" />
    <%--<link href="../content/css/progressBar_bootstrap.min.css" rel="stylesheet" />--%>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@4.6.2/dist/css/bootstrap.min.css" />

<%--    <script src="../content/Scripts/jquery.min.js" type="text/javascript"></script>--%>
    <script src="../content/jquery/jquery.min.js"></script>
    <%--<script src="../content/js/jquery.js" type="text/javascript"></script>--%>
   <%-- <script src="../content/Scripts/jquery-ui-1.8.16.min.js" type="text/javascript"></script>--%>
    <%--<script src="../content/Scripts/bootstrap.min.js" type="text/javascript"></script>--%>

    <script src="https://cdn.jsdelivr.net/npm/bootstrap@4.6.2/dist/js/bootstrap.bundle.min.js" type="text/javascript"></script>

<%--    <script type="text/javascript" src="../content/Scripts/jquery.ui.core.js"></script>
    <script type="text/javascript" src="../content/Scripts/jquery.ui.widget.js"></script>
    <script type="text/javascript" src="../content/Scripts/jquery.ui.datepicker.js"></script>
    <script type="text/javascript" src="../content/Scripts/jquery-ui-timepicker-addon.js"></script>--%>
    <script type="text/javascript">

        $(function () {
            //$('input[type=radio][name=InstallRetrive]').change();
            if ($("#hdnRadioFirstTime").val() == "true") {
                $('input[type=radio][name=InstallRetrive]')[0].checked = true;
                $("#Table2").hide()
            }

            if ($('input[type=radio][name=InstallRetrive]')[1].checked == true) {
                $("#Table1").hide()
            }
            if ($('input[type=radio][name=InstallRetrive]')[0].checked == true) {
                $("#Table2").hide()
            }

            $('input[type=radio][name=InstallRetrive]').change(function () {

                if (this.value == "1") {
                    $("#ExtractReplacePath").val("");
                    $("#Table2").show()
                    $("#Table1").hide()



                }
                else if (this.value == "2") {
                    $("#TextBox_InstallCommand1").val("");
                    $("#TextBox_InstallCommandConfirmationLogFilePath1").val("");
                    $("#TextBox_KeywordToSearchForSuccessfulInstallation1").val("");
                    $("#TextBox_UninstallCommand1").val("");
                    $("#TextBox_UninstallCommandConfirmationLogFilePath1").val("");
                    $("#TextBox_KeywordToSearchForUnsuccessfulInstallation1").val("");
                    $("#Checkbox_RequiresRestartBeforeExecuting").val("");
                    $("#Table1").show()
                    $("#Table2").hide()
                }

            });
        });
        var SelectedNode;
        var chk;
        function OnNodeSelect(sender, params) {
            SelectedNode = params.get_node();
            var SelectedNodeValue = SelectedNode.get_value().split("|");

            document.getElementById("TdFileProp").style.display = '';
            document.getElementById("DDlOverWriteOptions").value = SelectedNodeValue[1];
            document.getElementById("lblFileName").innerHTML = "<span id='a'>" + SelectedNode.get_text() + "</span>";

            if (document.getElementById("CheckedNodeID").value == SelectedNode.get_id()) {
                document.getElementById("chkExecuteFile").checked = true;
                document.getElementById("chkExecuteFile").disabled = '';
            }
            else {
                document.getElementById("chkExecuteFile").checked = false;
                if (document.getElementById("CheckedNodeID").value != "")
                    document.getElementById("chkExecuteFile").disabled = 'disabled';
            }
        }
        function SetOverWriteOptions() {
            if (SelectedNode) {
                var SelectedNodeValue = SelectedNode.get_value().split("|");
                SelectedNode.set_value(SelectedNodeValue[0] + "|" + document.getElementById("DDlOverWriteOptions").value);

            }
        }

        function SetExecuteOptions() {
            if (SelectedNode) {
                if (document.getElementById("chkExecuteFile").checked) document.getElementById("CheckedNodeID").value = SelectedNode.get_id();
                else document.getElementById("CheckedNodeID").value = "";
            }
        }

        function setCheck(node, selected) {
            var childary = node.get_nodes();

            for (var i = 0; i < childary.get_length(); i++) {
                if (childary.getNode(i).get_id().substring(0, 1) == 'r') {
                    childary.getNode(i).set_checked(selected);
                    setCheck(childary.getNode(i), selected)
                }
                else
                    childary.getNode(i).set_checked(selected);
            }
        }

        window.parent.window.contentPage = '';

        function check(sender, eventArgs) {
            UserATMsTreeView.beginUpdate();
            setCheck(eventArgs.get_node(), eventArgs.get_node().get_checked());
            UserATMsTreeView.endUpdate();
        }



    </script>
</head>
<body onload="hideProgressBar();">
    <form id="form1" runat="server">
        <input id="CheckedNodeID" type="hidden" value="" runat="server" />
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true">
        </asp:ScriptManager>
        <div>
            <asp:HiddenField ID="hdnRadioFirstTime" Value="false" runat="server" />
            <table cellspacing="0" cellpadding="0" border="0" class="mainbox" width="100%">
                <tr>
                    <td class="PageHeading" align="left">Package Management
                    </td>
                </tr>
                <tr>
                    <td class="SearchHeadPaneGradient"></td>
                </tr>
                <tr>
                    <td align="left">
                        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="False"
                            ShowSummary="True" Width="100%" CssClass="ValidationText"></asp:ValidationSummary>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table cellspacing="0" cellpadding="0" width="100%" border="0">
                            <tr>
                                <td align="center" style="color: red">
                                    <asp:Label ID="Label_Error" runat="server" CssClass="ValidationText"></asp:Label>
                                    <asp:Literal ID="Literal_error" runat="server" EnableViewState="False"></asp:Literal>
                                </td>
                            </tr>
                            <%--<tr>
                            <td class="ValidationText" align="right">
                                <span style="color: #ff0000">Fields marked as (*) are mandatory</span>
                            </td>
                        </tr>--%>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table style="width: 100%">
                            <tr>
                                <td style="width: 20%">Package Title<span style="color: #ff0000">(*)</span>
                                </td>
                                <td style="width: 80%">
                                    <asp:TextBox ID="TextBox_PackageTitle" runat="server" Width="200px" CssClass="textbox"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="requiredfieldvalidator_Title" runat="server" ControlToValidate="TextBox_PackageTitle"
                                        ErrorMessage="Please enter Upload Title." Text="*"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="regularexpressionvalidator_Title" runat="server"
                                        ControlToValidate="TextBox_PackageTitle" ErrorMessage="There are some invalid characters in the Title"
                                        ValidationExpression="[0-9|A-Z|a-z|\-|_|\s|,|.|&amp;]*" Text="*"></asp:RegularExpressionValidator>
                                </td>
                            </tr>
                            <tr>
                                <%--<div id="lblMessage" style="color: Green"></div>--%>
                                <asp:Label ID="lblMessage" runat="server" ForeColor="Green"></asp:Label>
                                <script type="text/javascript">                                   
                                    var startTime
                                    let chunkIndex = 0;
                                    var chunkSize = 0;
                                    var uploadedBytes = 0;
                                    var uploadedTotalChunks = 0;
                                    var totalChunks = 0;
                                    var uploadedChunks = [];
                                    const pendingUploadPromises = [];
                                    var serverUploadedBytes = 0;
                                    var pckgTitle = '';
                                    // Check for resume cookies
                                    var resumeCookieChunks = '';
                                    var resumeCookieChunkSize = '';
                                    var file;
                                    var totalSize = 0;
                                    var fileSize = '';

                                    $("body").on("click", "#btnUpload", function () {
                                        ProcessUpload();
                                    });

                                    $(window).on('keypress', function (e) {
                                        if (e.which == 13) {
                                            e.preventDefault();
                                            ProcessUpload();
                                        }
                                    });

                                    function SaveUploadedChunk(chunk) {
                                        var pckg = getCookie('pckg_object_' + pckgTitle);
                                        if (pckg != null) {
                                            pckg = JSON.parse(pckg);
                                            pckg.uploadedChunks = chunk;
                                            SetCookie('pckg_object_' + pckgTitle, JSON.stringify(pckg));
                                        }
                                    }
                                    function SetCookie(key, value) {
                                        document.cookie = key + '=' + value;
                                    }

                                    function getCookie(key) {
                                        var keyValue = document.cookie.match('(^|;) ?' + key + '=([^;]*)(;|$)');
                                        return keyValue ? keyValue[2] : null;
                                    }

                                    function eraseCookie(key) {
                                        document.cookie = key + '=; Max-Age=0';
                                    }

                                    function GetPckgObject() {
                                        var pckg = getCookie('pckg_object_' + pckgTitle);
                                        if (pckg != null) {
                                            return JSON.parse(pckg);
                                        }
                                    }

                                    function SavePackageInCookie() {

                                        if (getCookie('pckg_object_' + pckgTitle) == null) {
                                            var configuration = '';
                                            var extractPath = '';
                                            if ($('#SelectionProcess2').is(':checked')) {
                                                configuration = 'extract';
                                                extractPath = $("#ExtractReplacePath").val();
                                            }
                                            if ($('#SelectionProcess1').is(':checked')) {
                                                configuration = 'install';
                                            }
                                            const formattedDateTime = getFormattedDate();
                                            var pckg = { name: pckgTitle, path: file.name, fileSize: totalSize, totalChunks: totalChunks, chunkSize: chunkSize, uploadedChunks: '', dateCreated: formattedDateTime, configuration: configuration };
                                            SetCookie('pckg_object_' + pckgTitle, JSON.stringify(pckg));
                                        }
                                    }

                                    function generateChunkFilename(fileName, totalChunks, chunkIndex) {
                                        var extension = fileName.split('.').pop();
                                        var baseName = fileName.replace('.' + extension, '');
                                        //var chunkNumber = String(chunkIndex + 1).padStart(3, '0'); // Pad with leading zeros

                                        return baseName + "_chunk_" + totalChunks + "_" + chunkIndex + "." + extension;
                                    }

                                    function calculateUploadedBytes(uploadedChunks, totalSize, chunkSize) {
                                        // Handle invalid or zero uploadedChunks value
                                        if (uploadedChunks <= 0 || isNaN(uploadedChunks)) {
                                            return 0;
                                        }

                                        // Calculate total uploaded bytes based on number of chunks and chunk size
                                        const uploadedBytes = uploadedChunks * chunkSize;

                                        // Ensure uploadedBytes doesn't exceed total size
                                        return Math.min(uploadedBytes, totalSize);
                                    }

                                    function getFormattedDate() {
                                        const today = new Date();
                                        const day = String(today.getDate()).padStart(2, '0'); // Ensure two-digit day
                                        const month = String(today.getMonth() + 1).padStart(2, '0'); // Months are zero-indexed (January = 0)
                                        const year = today.getFullYear();
                                        const hour = String(today.getHours()).padStart(2, '0');
                                        const minute = String(today.getMinutes()).padStart(2, '0');
                                        const second = String(today.getSeconds()).padStart(2, '0');

                                        return `${day}/${month}/${year} ${hour}:${minute}:${second}`;
                                    }


                                    function ProcessUpload() {
                                        $("#ValidationSummary1").text('');
                                        $("#lblMessage").html('');
                                        $("#ValidationSummary1").hide();
                                        $(".ValidationText").text('');
                                        $(".progress").hide();
                                        $('.progress-bar').text('');
                                        $('.progress-bar').width(0 + '%');

                                        

                                        var errorInFile = false;
                                        var files = document.getElementById("fileContent").files;
                                        if (files.length > 0) {
                                            file = files[0];
                                            var extension = (file.name.substr((file.name.lastIndexOf('.') + 1))).toLowerCase();
                                            pckgTitle = $("#TextBox_PackageTitle").val();
                                            chunkSize = $("#ChunkSize").val();
                                            var maxChunkLimit = $("#ChunkSize").attr("maxchunklimit");
                                            totalSize = file.size;
                                            if (pckgTitle.length > 0) {
                                                if ($('#SelectionProcess2').is(':checked')) {
                                                    if (extension != 'zip') {
                                                        $("#ValidationSummary1").text('Invalid package format. Only zip format is allowed');
                                                        errorInFile = true;
                                                    }
                                                    else if ($("#ExtractReplacePath").val().length <= 0) {
                                                        $("#ValidationSummary1").text('Please enter Extract and Replace Path.');
                                                        errorInFile = true;
                                                    }
                                                }
                                                if ($('#SelectionProcess1').is(':checked')) {
                                                    if (extension != 'bat' && extension != 'msi' && extension != 'exe') {
                                                        $("#ValidationSummary1").text('Invalid package format. Only msi,bat,exe formats are allowed');
                                                        errorInFile = true;
                                                    }
                                                    else if ($("#TextBox_InstallCommand1").val().length <= 0) {
                                                        $("#ValidationSummary1").text('Please enter command for the execution of this package.');
                                                        errorInFile = true;
                                                    }
                                                }
                                            }
                                            else {
                                                $("#ValidationSummary1").text('Please enter Package Title');
                                                errorInFile = true;
                                            }

                                            if (chunkSize.length > 0) {
                                                if ($('#TextBox_PackageTitle').prop('disabled') == false) {
                                                    chunkSize = parseInt(chunkSize) * 1024 * 1024;
                                                    totalChunks = Math.ceil(totalSize / chunkSize);
                                                    if (totalChunks > parseInt(maxChunkLimit)) {
                                                        $("#ValidationSummary1").text("Please increase Chunk Size as maximum limit of total chunks is = " + maxChunkLimit);
                                                        errorInFile = true;
                                                    }
                                                }
                                            }
                                            else {
                                                $("#ValidationSummary1").text('Please enter Chunk size in MBs');
                                                errorInFile = true;
                                            }

                                            if ($('#TextBox_PackageTitle').prop('disabled') == true) {
                                                if ($('#fileLabel').attr('class') != file.name) {
                                                    $("#ValidationSummary1").text("Please upload the same file '" + $('#fileLabel').attr('class') + "' to resume..");
                                                    errorInFile = true;
                                                }
                                                if (parseInt($('#fileLabel').attr('FileSize')) != parseInt(totalSize)) {
                                                    $("#ValidationSummary1").text("Please upload the same file having size '" + $('#fileLabel').attr('FileSize') + "' to resume..");
                                                    errorInFile = true;
                                                }
                                            }

                                            if (!errorInFile) {
                                                startTime = performance.now();

                                                if (file) {
                                                    var pckgObject = GetPckgObject();
                                                    if (pckgObject == null) {
                                                        SavePackageInCookie();
                                                    }
                                                    uploadChunk();
                                                }

                                               
                                                

                                              


                                                function uploadChunk(respMsg) {

                                                    if (file.size >= (1024 * 1024)) {
                                                        fileSize = (file.size / (1024 * 1024)).toFixed(2) + ' MB'; // convert to MB with two decimal places
                                                    } else {
                                                        fileSize = (file.size / 1024).toFixed(2) + ' KB'; // convert to KB with no decimal places
                                                    }
                                              

                                                    function showFileDetailBox() {

                                                        var progress = 0;
                                                        $(".progress").show();
                                                        $('.progress-bar').text(progress + '%');
                                                        $('.progress-bar').width(progress + '%');

                                                        var resultDiv = document.getElementById('result');
                                                        resultDiv.innerHTML = 'File Size: ' + fileSize + ' <br>';
                                                        resultDiv.innerHTML += 'Total Chunk/s: ' + totalChunks + ' <br>';
                                                        $("#result").show();
                                                    }

                                                    showFileDetailBox();

                                                    if (pckgObject != null) {
                                                        resumeCookieChunks = pckgObject.uploadedChunks.split(',');
                                                        resumeCookieChunkSize = pckgObject.chunkSize;
                                                    }

                                                    if (resumeCookieChunks && resumeCookieChunkSize) {

                                                        chunkSize = parseInt(resumeCookieChunkSize);
                                                        uploadedChunks = resumeCookieChunks.map(Number); // Convert string values to numbers
                                                        uploadedBytes = serverUploadedBytes = calculateUploadedBytes(Math.max(...uploadedChunks) + 1, parseInt(totalSize), chunkSize);
                                                        totalChunks = Math.ceil(totalSize / chunkSize);
                                                        uploadedTotalChunks = Math.max(...uploadedChunks);

                                                        // Update UI to reflect resumed progress
                                                        const chunkProgress = (uploadedTotalChunks / totalChunks) * 100;
                                                        var progress = Math.round(chunkProgress); // Round progress percentage
                                                        $(".progress").show();
                                                        $('.progress-bar').text(progress + '%');
                                                        $('.progress-bar').width(progress + '%');
                                                        $('.progress-detail').text('Resuming upload...');
                                                    } else {
                                                        // No resume cookies, fresh upload
                                                        uploadedBytes = 0;
                                                        uploadedChunks = [];
                                                    }
                                                    listForms = [];
                                                    listAsyncs = [];
                                                    for (chunkIndex = uploadedTotalChunks; chunkIndex < totalChunks; chunkIndex++) {
                                                        // for (let chunkIndex = Math.ceil(uploadedBytes / chunkSize); chunkIndex < totalChunks; chunkIndex++) {

                                                        if (uploadedChunks.includes(chunkIndex)) {
                                                            // Skip uploading chunks already uploaded (resume functionality)
                                                            continue;
                                                        }

                                                        var startByte = uploadedBytes;
                                                        var endByte = Math.min(startByte + chunkSize, totalSize);
                                                        var chunk = file.slice(startByte, endByte);
                                                        console.log('statrt byte = ' + startByte + ' ,end byte = ' + endByte)
                                                        var chunkName = generateChunkFilename(file.name, totalChunks, chunkIndex);

                                                        var formData = new FormData();
                                                        formData.append("file", chunk);
                                                        formData.append("fileName", chunkName);
                                                        formData.append("totalChunks", totalChunks);
                                                        formData.append("uploadedBytes", chunk.size);
                                                        formData.append("fileSize", totalSize);
                                                        formData.append("CreatedBy", $("#fileLabel").attr("CreatedBy"));
                                                        //listForms.push(formData);
                                                        console.log('total attached:' + chunk.size)

                                                        //window.setTimeout(() => {
                                                        listAsyncs.push($.ajax({
                                                            url: 'Handler.ashx?packageTitle=' + pckgTitle + '&filename=' + chunkName,
                                                            type: "POST",
                                                            data: formData,
                                                            processData: false,
                                                            contentType: false,
                                                            success: function (r) {
                                                                NotifySuccess();
                                                            }
                                                        }));
                                                        //}, 3000);


                                                        uploadedBytes += chunk.size; // Update uploaded bytes for next iteration

                                                        $(".progress-detail").show();
                                                        $('.progress-detail').text('Uploading chunk/s..');

                                                        //uploadPromise.done(function (data) {

                                                        //    serverUploadedBytes += chunkSize;
                                                        //    uploadedChunks.push(chunkIndex);
                                                        //    uploadedTotalChunks++;

                                                        //    SaveUploadedChunk(uploadedChunks.join(','));

                                                        //    $('.progress-detail').text('Uploaded ' + uploadedTotalChunks + ' of ' + totalChunks + ' chunk/s');

                                                        //    const chunkProgress = (uploadedTotalChunks / totalChunks) * 100;
                                                        //    var progress = Math.round(chunkProgress); // Round progress percentage
                                                        //    $(".progress").show();
                                                        //    $('.progress-bar').text(progress + '%');
                                                        //    $('.progress-bar').width(progress + '%');
                                                        //    if (progress === 100) {                                                                
                                                        //        $('.progress-bar').text('100%');
                                                        //    }

                                                        //    var currentTime = performance.now();
                                                        //    var elapsedTime = (currentTime - startTime) / 1000; // in seconds
                                                        //    var speed = uploadedBytes / elapsedTime;

                                                        //    // Convert speed to kB/s
                                                        //    speed = speed / 1024;
                                                        //    var remainingTime = ((elapsedTime / (progress / 100)) - elapsedTime) / 60;
                                                        //    var resultDiv = document.getElementById('result');
                                                        //    resultDiv.innerHTML = 'File Size: ' + fileSize + ' <br>';
                                                        //    resultDiv.innerHTML += 'Total Chunk/s: ' + totalChunks + ' <br>';
                                                        //    resultDiv.innerHTML += 'Remaining Time: ' + remainingTime.toFixed(2) + ' minutes<br>';
                                                        //    resultDiv.innerHTML += 'Upload Speed: ' + speed.toFixed(2) + ' kB/s';
                                                        //    $("#result").show();
                                                        //}).fail(function (a) {
                                                        //    $("#lblMessage").html(a.responseText);
                                                        //    // Handle individual chunk upload error
                                                        //});
                                                    }
                                                    //$.when(
                                                    //    $.ajax({
                                                    //        url: 'Handler.ashx?packageTitle=' + pckgTitle,
                                                    //        type: "POST",
                                                    //        data: listForms[0],
                                                    //        processData: false,
                                                    //        contentType: false,
                                                    //        success: function (r) {
                                                    //            NotifySuccess();
                                                    //        }
                                                    //    }),
                                                    //    $.ajax({
                                                    //        url: 'Handler.ashx?packageTitle=' + pckgTitle,
                                                    //        type: "POST",
                                                    //        data: listForms[1],
                                                    //        processData: false,
                                                    //        contentType: false,
                                                    //        success: function (r) {
                                                    //            NotifySuccess();
                                                    //        }
                                                    //    }),
                                                    //    $.ajax({
                                                    //        url: 'Handler.ashx?packageTitle=' + pckgTitle,
                                                    //        type: "POST",
                                                    //        data: listForms[2],
                                                    //        processData: false,
                                                    //        contentType: false,
                                                    //        success: function (r) {
                                                    //            NotifySuccess();
                                                    //        }
                                                    //    }),
                                                    //    $.ajax({
                                                    //        url: 'Handler.ashx?packageTitle=' + pckgTitle,
                                                    //        type: "POST",
                                                    //        data: listForms[3],
                                                    //        processData: false,
                                                    //        contentType: false,
                                                    //        success: function (r) {
                                                    //            NotifySuccess();
                                                    //        }
                                                    //    }), $.ajax({
                                                    //        url: 'Handler.ashx?packageTitle=' + pckgTitle,
                                                    //        type: "POST",
                                                    //        data: listForms[4],
                                                    //        processData: false,
                                                    //        contentType: false,
                                                    //        success: function (r) {
                                                    //            NotifySuccess();
                                                    //        }
                                                    //    }),
                                                    //    $.ajax({
                                                    //        url: 'Handler.ashx?packageTitle=' + pckgTitle,
                                                    //        type: "POST",
                                                    //        data: listForms[5],
                                                    //        processData: false,
                                                    //        contentType: false,
                                                    //        success: function (r) {
                                                    //            NotifySuccess();
                                                    //        }
                                                    //    }),
                                                    //    $.ajax({
                                                    //        url: 'Handler.ashx?packageTitle=' + pckgTitle,
                                                    //        type: "POST",
                                                    //        data: listForms[6],
                                                    //        processData: false,
                                                    //        contentType: false,
                                                    //        success: function (r) {
                                                    //            NotifySuccess();
                                                    //        }
                                                    //    }),
                                                    //    $.ajax({
                                                    //        url: 'Handler.ashx?packageTitle=' + pckgTitle,
                                                    //        type: "POST",
                                                    //        data: listForms[7],
                                                    //        processData: false,
                                                    //        contentType: false,
                                                    //        success: function (r) {
                                                    //            NotifySuccess();
                                                    //        }
                                                    //    }),
                                                    //    $.ajax({
                                                    //        url: 'Handler.ashx?packageTitle=' + pckgTitle,
                                                    //        type: "POST",
                                                    //        data: listForms[8],
                                                    //        processData: false,
                                                    //        contentType: false,
                                                    //        success: function (r) {
                                                    //            NotifySuccess();
                                                    //        }
                                                    //    }),
                                                    //    $.ajax({
                                                    //        url: 'Handler.ashx?packageTitle=' + pckgTitle,
                                                    //        type: "POST",
                                                    //        data: listForms[9],
                                                    //        processData: false,
                                                    //        contentType: false,
                                                    //        success: function (r) {
                                                    //            NotifySuccess();
                                                    //        }
                                                    //    }),
                                                    //    $.ajax({
                                                    //        url: 'Handler.ashx?packageTitle=' + pckgTitle,
                                                    //        type: "POST",
                                                    //        data: listForms[10],
                                                    //        processData: false,
                                                    //        contentType: false,
                                                    //        success: function (r) {
                                                    //            NotifySuccess();
                                                    //        }
                                                    //    })






                                                    //    //pendingUploadPromises.push(uploadPromise);

                                                    //).then(function (data) { console.log(data); });

                                                    // Wait for all chunks to upload (if more chunks are available)

                                                    window.setTimeout(() => {

                                                        Promise.all(listAsyncs).then(() => {
                                                            // Check for any errors during uploads
                                                            const hasUploadError = pendingUploadPromises.some(promise => promise.state() === 'rejected');
                                                            if (hasUploadError) {
                                                                // Handle overall upload failure
                                                                return;
                                                            }
                                                            // All chunks uploaded successfully, proceed with next steps
                                                            if (uploadedBytes >= totalSize) {
                                                                $("#lblMessage").html("<b>" + "file/s merging and package creation in progress.." + "</b>");
                                                                $("#fileLabel").text('')
                                                                console.log("Upload complete!");
                                                                $("#TextBox_FileName").val(file.name);
                                                                eraseCookie('pckg_object_' + pckgTitle);

                                                                $("#Button_Submit").trigger("click");
                                                                $(".progress").hide();
                                                                $(".progress-detail").hide();
                                                                $('.progress-bar').text('');
                                                                $('.progress-bar').width(0 + '%');
                                                                return;
                                                            }
                                                        })
                                                    }, 3000);
                                                }
                                            }
                                        }
                                        else {
                                            $("#ValidationSummary1").text('Incorrect File Path or File does not exists.');
                                            errorInFile = true;
                                        }

                                        if (errorInFile) {
                                            $("#ValidationSummary1").show();
                                        }
                                    }

                                    function NotifySuccess() {
                                        
                                        serverUploadedBytes += chunkSize;
                                        uploadedChunks.push(chunkIndex);
                                        uploadedTotalChunks++;

                                        SaveUploadedChunk(uploadedChunks.join(','));

                                        $('.progress-detail').text('Uploaded ' + uploadedTotalChunks + ' of ' + totalChunks + ' chunk/s');

                                        const chunkProgress = (uploadedTotalChunks / totalChunks) * 100;
                                        var progress = Math.round(chunkProgress); // Round progress percentage
                                        $(".progress").show();
                                        $('.progress-bar').text(progress + '%');
                                        $('.progress-bar').width(progress + '%');
                                        if (progress === 100) {
                                            $('.progress-bar').text('100%');
                                        }

                                        var currentTime = performance.now();
                                        var elapsedTime = (currentTime - startTime) / 1000; // in seconds
                                        var speed = uploadedBytes / elapsedTime;

                                        // Convert speed to kB/s
                                        speed = speed / 1024;
                                        var remainingTime = ((elapsedTime / (progress / 100)) - elapsedTime) / 60;
                                        var resultDiv = document.getElementById('result');
                                        resultDiv.innerHTML = 'File Size: ' + fileSize + ' <br>';
                                        resultDiv.innerHTML += 'Total Chunk/s: ' + totalChunks + ' <br>';
                                        resultDiv.innerHTML += 'Remaining Time: ' + remainingTime.toFixed(2) + ' minutes<br>';
                                        resultDiv.innerHTML += 'Upload Speed: ' + speed.toFixed(2) + ' kB/s';
                                        $("#result").show();
                                    }
                                </script>


                                <td>Package File To Upload:&nbsp;<span style="color: #ff0000">(*)&nbsp;&nbsp;</span>
                                </td>

                                <td>
                                    <asp:Label ID="fileLabel" Class="dynamic" runat="server"></asp:Label>
                                    <input type="file" name="postedFile" style="height: 100%;" id="fileContent" accept=".zip, .bat , .msi, .exe" />
                                    <div class="progress" style="display: none; width: 400px; margin-bottom: 0px !important">
                                        <div class="progress-bar" role="progressbar"></div>
                                    </div>
                                    <div class="progress-detail" style="display: none; margin-bottom: 2px; color: green; font-weight: bold;"></div>
                                    <div id="result" class="border border-success" style="display: none; margin-bottom: 1%; border-style: groove; width: 16%; background-color: lightyellow;">
                                    </div>
                                </td>
                            </tr>

                            <tr>
                                <td style="width: 20%">File Chunk Size In MBs<span style="color: #ff0000">(*)</span>
                                </td>
                                <td style="width: 80%">
                                    <asp:TextBox ID="ChunkSize" MaxLength="3" oninput="javascript: if (this.value.length > this.maxLength) this.value = this.value.slice(0, this.maxLength);" MaxChunkLimit="30" title="The file will be uploaded in chunks. The recommended size for a chunk is 500MBs." Text="500" runat="server" Width="200px" TextMode="Number" CssClass="textbox" Enabled="false"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="requiredfieldvalidator1" runat="server" ControlToValidate="ChunkSize"
                                        ErrorMessage="Please enter a number." Text="*"></asp:RequiredFieldValidator>
                                </td>
                            </tr>

                            <tr>
                                <td valign="top">
                                    <b>Configuration for Installations</b>
                                </td>
                                <td>
                                    <table>
                                        <tr>
                                            <td>
                                                <div style="float: left;">
                                                    <asp:RadioButton ID="SelectionProcess2" runat="server" value="2" GroupName="InstallRetrive" />
                                                    <asp:Label>Extract and Replace</asp:Label>
                                                </div>
                                                <div style="float: right; padding-left: 20px;">
                                                    <asp:RadioButton ID="SelectionProcess1" runat="server" value="1" GroupName="InstallRetrive" />
                                                    <asp:Label>Install</asp:Label>
                                                </div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <table id="Table1" runat="server">
                                                    <tr>
                                                        <td>Path : 
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="ExtractReplacePath" runat="server"
                                                                Width="200px" CssClass="textbox"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <%--<tr>
                                                                <td>Requires Backup                                                       </td>
                                                                <td>
                                                                    <asp:CheckBox ID="Checkbox_RequiresBackup1" runat="server"></asp:CheckBox>

                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>Owerwrite Old Files  </td>
                                                                <td>
                                                                    <asp:CheckBox ID="Checkbox_OverwriteOldFiles1" runat="server"></asp:CheckBox>

                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>Owerwrite Latest Version with this package  </td>
                                                                <td>
                                                                    <asp:CheckBox ID="Checkbox_OverwriteLatestVersion1" runat="server"></asp:CheckBox>

                                                                </td>
                                                            </tr>--%>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>

                                    <table id="Table2" runat="server">
                                        <%--<tr>
                                                                <td>Process Name                                                        </td>
                                                                <td>
                                                                    <asp:TextBox ID="TextBox_ProcessName1" runat="server" Width="200px" CssClass="textbox"></asp:TextBox>

                                                                </td>
                                                            </tr>--%>
                                        <tr>
                                            <td>Install Command
                                            </td>
                                            <td>
                                                <asp:TextBox ID="TextBox_InstallCommand1" runat="server" Width="200px" CssClass="textbox"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>Install Confirmation Log file path
                                            </td>
                                            <td>
                                                <asp:TextBox ID="TextBox_InstallCommandConfirmationLogFilePath1" runat="server" Width="200px"
                                                    CssClass="textbox"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>Keyword to search for successful installation in log file
                                            </td>
                                            <td>
                                                <asp:TextBox ID="TextBox_KeywordToSearchForSuccessfulInstallation1" runat="server"
                                                    Width="200px" CssClass="textbox"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>Uninstall Command
                                            </td>
                                            <td>
                                                <asp:TextBox ID="TextBox_UninstallCommand1" runat="server" Width="200px" CssClass="textbox"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>Uninstall Confirmation Log file path
                                            </td>
                                            <td>
                                                <asp:TextBox ID="TextBox_UninstallCommandConfirmationLogFilePath1" runat="server"
                                                    Width="200px" CssClass="textbox"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>Keyword to search for successful uninstallation in log file
                                            </td>
                                            <td>
                                                <asp:TextBox ID="TextBox_KeywordToSearchForUnsuccessfulInstallation1" runat="server"
                                                    Width="200px" CssClass="textbox"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>Requires Restart Before Executing
                                            </td>
                                            <td>
                                                <asp:CheckBox ID="Checkbox_RequiresRestartBeforeExecuting" runat="server"></asp:CheckBox>
                                            </td>
                                        </tr>
                                        <tr hidden="hidden">
                                            <td>Requires Restart After Executing
                                            </td>
                                            <td>
                                                <asp:CheckBox ID="Checkbox_RequiresRestartAfterExecuting" runat="server"></asp:CheckBox>
                                            </td>
                                        </tr>
                                    </table>

                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <div style="display: none;">
                                        <asp:TextBox ID="TextBox_FileName" runat="server" Width="200px" CssClass="textbox"></asp:TextBox>
                                    </div>
                                    <input type="button" class="btn btn-success" id="btnUpload" value="Upload" style="margin-left: 1%;" />
                                    <div style="display: none;">
                                        <asp:Button ID="Button_Submit" runat="server" CssClass="button" OnClick="Button_Submit_Click"
                                            Text="Submit" />
                                    </div>
                                    <input type="button" class="btn btn-secondary" value="Cancel" onclick="javascript: window.location = 'package.aspx';" />
                                </td>
                            </tr>
                        </table>
        </div>
    </form>

</body>
</html>
