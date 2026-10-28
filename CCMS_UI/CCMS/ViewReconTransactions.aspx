<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewReconTransactions.aspx.cs"
    Inherits="CCMSUI.CCMS.ViewReconTransactions" %>
<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN"
    "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <script src="../content/shortcut.js" type="text/javascript" language="javascript">
    </script>
    <title>View Reconciliation Transactions</title>
    <script type="text/javascript">
        window.parent.window.SetContentPage(window.location);
        function LabelText(labeltext) {
            document.getElementById('Label_errorMsg').innerHTML =
                "<li>" + labeltext + "</li>";
        }
    </script>
    <link href="../content/iSuite.css" rel="stylesheet" />
    <style type="text/css">
        /* ============================================================
           EXISTING / COMPATIBILITY STYLES
           ============================================================ */
        .custom {
            padding: 5px;
        }
        .customText {
            padding: 5px;
            font-weight: bold;
            font-size: small;
        }
        #MultiPage_AddEdit {
            display: table !important;
        }
        .MultiPage {
            padding: 0px;
        }
        /* ============================================================
           RECONCILIATION PAGE - ENTERPRISE UI
           ============================================================ */
        .rd-wrap {
            width: 100%;
            padding: 10px 14px 20px 14px;
            font-family: Verdana, Tahoma, Arial, sans-serif;
            color: #1f2933;
            text-align: left;
            box-sizing: border-box;
        }
        /* ============================================================
           PAGE HEADER
           ============================================================ */
        .rd-page-header {
            position: relative;
            width: 100%;
            margin: 0 0 16px 0;
            padding: 18px 22px 17px 22px;
            background: linear-gradient(
                135deg,
                #ffffff 0%,
                #f7f9fc 100%
            );
            border: 1px solid #dce3ec;
            border-radius: 6px;
            box-shadow:
                0 2px 6px rgba(31, 58, 95, 0.06);
            box-sizing: border-box;
            overflow: hidden;
        }
        /*
         * Enterprise vertical accent.
         */
        .rd-page-header:before {
            content: "";
            position: absolute;
            top: 0;
            left: 0;
            width: 5px;
            height: 100%;
            background: linear-gradient(
                180deg,
                #1f3a5f 0%,
                #2f6fa5 100%
            );
        }
        .rd-page-title {
            margin-left: 4px;
            font-family: Verdana, Tahoma, Arial, sans-serif;
            font-size: 19px;
            line-height: 1.3;
            font-weight: bold;
            color: #172b4d;
            letter-spacing: -0.2px;
        }
        .rd-page-subtitle {
            margin-top: 5px;
            margin-left: 4px;
            font-size: 11px;
            line-height: 1.5;
            color: #718096;
        }
        /*
         * Small icon beside the page title.
         */
        .rd-title-icon {
            display: inline-block;
            width: 27px;
            height: 27px;
            margin-right: 9px;
            line-height: 27px;
            text-align: center;
            font-family: Arial, sans-serif;
            font-size: 14px;
            font-weight: bold;
            color: #ffffff;
            background: #1f3a5f;
            border-radius: 5px;
            vertical-align: middle;
            box-shadow:
                0 2px 4px rgba(31, 58, 95, 0.18);
        }
        /* ============================================================
           TAB STRIP
           ============================================================ */
        .rd-tabs {
            display: flex;
            width: 100%;
            margin: 0;
            padding: 5px;
            background: #eef2f7;
            border: 1px solid #d9e0e8;
            border-radius: 6px;
            box-sizing: border-box;
            box-shadow:
                inset 0 1px 2px rgba(31, 58, 95, 0.04);
        }
        .rd-tabs .rd-tab {
            display: inline-block;
            min-width: 145px;
            background: transparent;
            color: #64748b;
            font-family: Verdana, Tahoma, Arial, sans-serif;
            font-size: 11px;
            font-weight: bold;
            border: 0;
            border-radius: 4px;
            cursor: pointer;
            box-sizing: border-box;
            transition:
                background-color 0.18s ease,
                color 0.18s ease,
                box-shadow 0.18s ease;
        }
        .rd-tabs .rd-tab-hover {
            background: #ffffff;
            color: #1f3a5f;
            box-shadow:
                0 1px 3px rgba(31, 58, 95, 0.08);
        }
        .rd-tabs .rd-tab-selected {
            position: relative;
            background: #ffffff;
            color: #1f3a5f;
            font-family: Verdana, Tahoma, Arial, sans-serif;
            font-size: 11px;
            font-weight: bold;
            border: 0;
            border-radius: 4px;
            box-shadow:
                0 2px 5px rgba(31, 58, 95, 0.12);
            box-sizing: border-box;
        }
        /*
         * Selected tab accent.
         */
        .rd-tabs .rd-tab-selected:after {
            content: "";
            position: absolute;
            left: 18px;
            right: 18px;
            bottom: 0;
            height: 3px;
            background: #2f6fa5;
            border-radius: 3px 3px 0 0;
        }
        /* ============================================================
           SECTION HEADERS
           ============================================================ */
        .rd-section-header {
            position: relative;
            width: 100%;
            padding: 14px 12px 11px 14px;
            margin: 0 0 10px 0;
            background: linear-gradient(
                90deg,
                #f7f9fc 0%,
                #ffffff 100%
            );
            border: 1px solid #e0e6ee;
            border-radius: 5px;
            box-sizing: border-box;
        }
        /*
         * Section accent.
         */
        .rd-section-header:before {
            content: "";
            position: absolute;
            left: 0;
            top: 10px;
            bottom: 10px;
            width: 3px;
            background: #2f6fa5;
            border-radius: 0 3px 3px 0;
        }
        .rd-section-title {
            padding-left: 6px;
            font-family: Verdana, Tahoma, Arial, sans-serif;
            font-size: 14px;
            line-height: 1.3;
            font-weight: bold;
            color: #1f3a5f;
            letter-spacing: -0.1px;
        }
        .rd-section-description {
            padding-left: 6px;
            margin-top: 4px;
            font-size: 10px;
            line-height: 1.45;
            color: #7b8794;
        }
        /*
         * Secondary section such as Dispute Breakdown.
         */
        .rd-source-header {
            margin-top: 14px;
            background: #ffffff;
            border-top: 0;
            border-right: 0;
            border-bottom: 1px solid #dfe5ec;
            border-left: 0;
            border-radius: 0;
            padding-top: 12px;
            padding-bottom: 9px;
        }
        .rd-source-header:before {
            top: 11px;
            bottom: 9px;
            width: 3px;
            background: #718096;
        }
        /* ============================================================
           BATCH INFORMATION
           ============================================================ */
        .rd-info {
            width: 100%;
            margin-bottom: 14px;
            background: #f8fafc;
            border: 1px solid #dfe4ea;
            border-left: 3px solid #1f3a5f;
            border-radius: 4px;
            box-sizing: border-box;
            box-shadow:
                0 1px 2px rgba(31, 41, 51, 0.03);
        }
        .rd-info-row {
            width: 100%;
            display: table;
            table-layout: fixed;
        }
        .rd-info-item {
            display: table-cell;
            width: 33.3333%;
            padding: 11px 15px;
            vertical-align: middle;
            box-sizing: border-box;
        }
        .rd-info-item + .rd-info-item {
            border-left: 1px solid #dfe4ea;
        }
        .rd-info-label {
            margin-bottom: 5px;
            font-size: 9px;
            line-height: 1.3;
            text-transform: uppercase;
            letter-spacing: 0.45px;
            color: #7b8794;
        }
        .rd-info-value {
            font-size: 12px;
            line-height: 1.4;
            font-weight: bold;
            color: #263646;
            word-wrap: break-word;
        }
        /* ============================================================
           GRID
           ============================================================ */
        .rd-row {
            width: 100%;
            margin: 0 -6px;
            zoom: 1;
        }
        .rd-row:after {
            content: "";
            display: table;
            clear: both;
        }
        .rd-col {
            float: left;
            padding: 0 6px;
            margin-bottom: 12px;
            box-sizing: border-box;
        }
        .rd-w-60 {
            width: 60%;
        }
        .rd-w-40 {
            width: 40%;
        }
        .rd-w-33 {
            width: 33.3333%;
        }
        /* ============================================================
           PRIMARY KPI / HERO CARDS
           ============================================================ */
        .rd-hero {
            position: relative;
            min-height: 126px;
            padding: 19px 21px;
            color: #ffffff;
            border-radius: 6px;
            box-sizing: border-box;
            overflow: hidden;
            box-shadow:
                0 3px 8px rgba(31, 58, 95, 0.10);
        }
        /*
         * Top highlight.
         */
        .rd-hero:before {
            content: "";
            position: absolute;
            top: 0;
            left: 0;
            width: 100%;
            height: 3px;
            background: rgba(255, 255, 255, 0.20);
        }
        /*
         * Subtle decorative circle.
         */
        .rd-hero:after {
            content: "";
            position: absolute;
            width: 110px;
            height: 110px;
            right: -35px;
            bottom: -55px;
            border: 1px solid rgba(255, 255, 255, 0.08);
            border-radius: 50%;
        }
        .rd-hero-amount {
            background: linear-gradient(
                135deg,
                #1f3a5f 0%,
                #274e78 100%
            );
        }
        .rd-hero-count {
            background: linear-gradient(
                135deg,
                #52657d 0%,
                #647891 100%
            );
        }
        .rd-hero-label {
            position: relative;
            z-index: 1;
            font-size: 10px;
            line-height: 1.3;
            font-weight: bold;
            text-transform: uppercase;
            letter-spacing: 0.45px;
            color: #dbe4f0;
            margin-bottom: 11px;
        }
        .rd-hero-value {
            position: relative;
            z-index: 1;
            font-family: Tahoma, Verdana, Arial, sans-serif;
            font-size: 34px;
            font-weight: bold;
            line-height: 1.1;
            color: #ffffff;
            word-wrap: break-word;
        }
        .rd-hero-count .rd-hero-value {
            font-size: 32px;
        }
        .rd-hero-meta {
            position: relative;
            z-index: 1;
            margin-top: 8px;
            font-size: 10px;
            line-height: 1.3;
            color: #cbd5e1;
        }
        /* ============================================================
           SOURCE CARDS
           ============================================================ */
        .rd-card {
            width: 100%;
            min-height: 175px;
            background: #ffffff;
            border: 1px solid #dfe4ea;
            border-radius: 5px;
            overflow: hidden;
            box-shadow:
                0 2px 5px rgba(31, 41, 51, 0.06);
            box-sizing: border-box;
            transition:
                box-shadow 0.18s ease,
                transform 0.18s ease;
        }
        .rd-card:hover {
            box-shadow:
                0 4px 10px rgba(31, 41, 51, 0.10);
        }
        .rd-card-head {
            padding: 10px 14px;
            font-size: 11px;
            line-height: 1.3;
            font-weight: bold;
            color: #ffffff;
        }
        /*
         * Muted enterprise source colors.
         */
        .rd-ej .rd-card-head {
            background: #3d6a99;
        }
        .rd-sw .rd-card-head {
            background: #2f7f79;
        }
        .rd-host .rd-card-head {
            background: #5e728b;
        }
        .rd-source-dot {
            display: inline-block;
            width: 7px;
            height: 7px;
            margin-right: 7px;
            vertical-align: middle;
            background: rgba(255, 255, 255, 0.88);
            border-radius: 50%;
        }
        .rd-card-body {
            padding: 16px 14px 14px 14px;
        }
        .rd-metric-label {
            font-size: 9px;
            line-height: 1.3;
            text-transform: uppercase;
            letter-spacing: 0.25px;
            color: #7b8794;
            margin-bottom: 5px;
        }
        .rd-metric-value {
            font-family: Tahoma, Verdana, Arial, sans-serif;
            font-size: 23px;
            font-weight: bold;
            line-height: 1.15;
            color: #263646;
            word-wrap: break-word;
        }
        .rd-metric-count {
            margin-top: 14px;
            padding-top: 12px;
            border-top: 1px solid #e7ebef;
        }
        .rd-metric-count .rd-metric-value {
            font-size: 16px;
            color: #44546a;
        }
        /* ============================================================
           EMPTY STATE
           ============================================================ */
        .rd-empty {
            width: 100%;
            padding: 30px 10px;
            margin-top: 6px;
            border: 1px dashed #c9d0d8;
            border-radius: 5px;
            background: #fafbfc;
            text-align: center;
            font-size: 11px;
            color: #6b7785;
            box-sizing: border-box;
        }
        /* ============================================================
           VALIDATION / MESSAGE AREA
           ============================================================ */
        .rd-message-space {
            min-height: 4px;
        }
        /* ============================================================
           RESPONSIVE
           ============================================================ */
        @media only screen and (max-width: 900px) {
            .rd-w-60,
            .rd-w-40 {
                width: 100%;
            }
            .rd-tabs .rd-tab {
                min-width: 130px;
            }
        }
        @media only screen and (max-width: 720px) {
            .rd-w-33 {
                width: 100%;
            }
            .rd-hero {
                min-height: 0;
            }
            .rd-info-row {
                display: block;
            }
            .rd-info-item {
                display: block;
                width: 100%;
            }
            .rd-info-item + .rd-info-item {
                border-left: 0;
                border-top: 1px solid #dfe4ea;
            }
            .rd-tabs {
                display: block;
            }
            .rd-tabs .rd-tab {
                display: block;
                width: 100%;
                min-width: 0;
                margin-bottom: 3px;
            }
            .rd-tabs .rd-tab:last-child {
                margin-bottom: 0;
            }
            .rd-tabs .rd-tab-selected:after {
                left: 12px;
                right: 12px;
            }
        }
        @media only screen and (max-width: 480px) {
            .rd-wrap {
                padding-left: 6px;
                padding-right: 6px;
            }
            .rd-page-header {
                padding: 15px 16px 15px 17px;
            }
            .rd-page-title {
                font-size: 16px;
            }
            .rd-title-icon {
                width: 24px;
                height: 24px;
                line-height: 24px;
                margin-right: 7px;
                font-size: 12px;
            }
            .rd-hero-value {
                font-size: 28px;
            }
            .rd-hero-count .rd-hero-value {
                font-size: 27px;
            }
        }
    </style>
</head>
<body
    onkeydown="checkForCtrlKey(window.event);"
    onkeyup="checkForCharKey(window.event)"
    onload="hideProgressBar();">
    <form id="form1" runat="server">
        <input type="hidden" id="amount" />
        <input type="hidden" id="reconTrxnID" />
        <input type="hidden" id="operationType" />
        <input type="hidden" id="gridID" />
        <input type="hidden" id="batchID" runat="server" />
        <div>
            <table
                style="width: 100%"
                cellspacing="0"
                cellpadding="0"
                border="0"
                class="mainbox">
                <tr>
                    <td align="left" colspan="2">
                        <div class="rd-page-header">
                            <div class="rd-page-title">
                                <span class="rd-title-icon">
                                    ↔
                                </span>
                                Reconciliation Summary
                            </div>
                            <div class="rd-page-subtitle">
                                Review disputed transaction activity and reconciliation results
                            </div>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td class="ValidationText" align="center">
                        <asp:Literal
                            ID="Literal_error"
                            runat="server"
                            EnableViewState="False">
                        </asp:Literal>
                        <div class="ValidationText">
                            <asp:Label
                                ID="Label_errorMsg"
                                runat="server"
                                EnableViewState="False">
                            </asp:Label>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>
                        <div
                            id="notificationArea"
                            style="text-align: center">
                        </div>
                        <asp:Literal
                            ID="Literal_msg"
                            runat="server"
                            EnableViewState="false">
                        </asp:Literal>
                    </td>
                    <td
                        class="ConfirmationText"
                        align="center">
                        <%=Session[SessionVars.notice.ToString()]%>
                        <%Session[SessionVars.notice.ToString()] = "";%>
                        <asp:Literal
                            ID="Literal_ok"
                            runat="server"
                            EnableViewState="False">
                        </asp:Literal>
                    </td>
                </tr>
                <tr>
                    <td>
                        <ComponentArt:TabStrip
                            ID="TabStrip_AddEdit"
                            CssClass="rd-tabs"
                            DefaultItemLookId="DefaultTabLook"
                            DefaultSelectedItemLookId="SelectedTabLook"
                            DefaultDisabledItemLookId="DisabledTabLook"
                            DefaultGroupTabSpacing="0px"
                            ImagesBaseUrl="../images/Tabimages/"
                            MultiPageId="MultiPage_AddEdit"
                            runat="server"
                            ScrollDownLookId="DefaultTabLook"
                            ScrollLeftLookId="DefaultTabLook"
                            ScrollRightLookId="DefaultTabLook"
                            ScrollUpLookId="DefaultTabLook"
                            TopGroupTabSpacing="0px">
                            <ItemLooks>
                                <ComponentArt:ItemLook
                                    LookId="DefaultTabLook"
                                    CssClass="rd-tab"
                                    HoverCssClass="rd-tab-hover"
                                    LabelPaddingLeft="18px"
                                    LabelPaddingRight="18px"
                                    LabelPaddingTop="10px"
                                    LabelPaddingBottom="10px"
                                    LeftIconVisibility="WhenChecked"
                                    RightIconVisibility="WhenChecked" />
                                <ComponentArt:ItemLook
                                    LookId="SelectedTabLook"
                                    CssClass="rd-tab-selected"
                                    LabelPaddingLeft="18px"
                                    LabelPaddingRight="18px"
                                    LabelPaddingTop="10px"
                                    LabelPaddingBottom="10px"
                                    LeftIconVisibility="WhenChecked"
                                    RightIconVisibility="WhenChecked" />
                            </ItemLooks>
                            <Tabs>
                                <ComponentArt:TabStripTab
                                    ID="TabStripTab_general"
                                    runat="server"
                                    Text="Dispense Transaction">
                                </ComponentArt:TabStripTab>
                                <ComponentArt:TabStripTab
                                    ID="TabStripTab_physicalConfig"
                                    runat="server"
                                    Text="Deposit Transaction">
                                </ComponentArt:TabStripTab>
                            </Tabs>
                        </ComponentArt:TabStrip>
                        <ComponentArt:MultiPage
                            ID="MultiPage_AddEdit"
                            CssClass="MultiPage"
                            runat="server"
                            Width="100%">
                            <ComponentArt:PageView
                                CssClass="PageContent"
                                runat="server"
                                ID="PageView_general">
                                <table width="100%">
                                    <tr>
                                        <td>
                                            <div class="rd-section-header">
                                                <div class="rd-section-title">
                                                    Batch Summary
                                                </div>
                                                <div class="rd-section-description">
                                                    Transaction batch details and dispute overview
                                                </div>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td valign="top">
                                            <asp:Repeater
                                                ID="RepeaterWithdrawal"
                                                runat="server">
                                                <HeaderTemplate>
                                                    <div class="rd-wrap">
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <div class="rd-info">
                                                        <div class="rd-info-row">
                                                            <div class="rd-info-item">
                                                                <div class="rd-info-label">
                                                                    ATM Title
                                                                </div>
                                                                <div class="rd-info-value">
                                                                    <%#: Eval("atm_Id") %>
                                                                </div>
                                                            </div>
                                                            <div class="rd-info-item">
                                                                <div class="rd-info-label">
                                                                    Processing Date &amp; Time
                                                                </div>
                                                                <div class="rd-info-value">
                                                                    <%#: Eval("processing_DateTime") %>
                                                                </div>
                                                            </div>
                                                            <div class="rd-info-item">
                                                                <div class="rd-info-label">
                                                                    Transaction Date
                                                                </div>
                                                                <div class="rd-info-value">
                                                                    <%#: FormatTranDate(Eval("tran_Date")) %>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="rd-row">
                                                        <div class="rd-col rd-w-60">
                                                            <div class="rd-hero rd-hero-amount">
                                                                <div class="rd-hero-label">
                                                                    Total Disputed Amount
                                                                </div>
                                                                <div class="rd-hero-value">
                                                                    <%#: FormatAmount(Eval("disputed_Amount")) %>
                                                                </div>
                                                                <div class="rd-hero-meta">
                                                                    Across all disputed sources
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="rd-col rd-w-40">
                                                            <div class="rd-hero rd-hero-count">
                                                                <div class="rd-hero-label">
                                                                    Disputed Transactions
                                                                </div>
                                                                <div class="rd-hero-value">
                                                                    <%#: FormatCount(Eval("Disputed_OnUs_Transactions")) %>
                                                                </div>
                                                                <div class="rd-hero-meta">
                                                                    Total disputed transaction count
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="rd-section-header rd-source-header">
                                                        <div class="rd-section-title">
                                                            Dispute Breakdown
                                                        </div>
                                                        <div class="rd-section-description">
                                                            Disputed amounts and transaction counts by source
                                                        </div>
                                                    </div>
                                                    <div class="rd-row">
                                                        <div class="rd-col rd-w-33">
                                                            <div class="rd-card rd-ej">
                                                                <div class="rd-card-head">
                                                                    <span class="rd-source-dot"></span>
                                                                    EJ
                                                                </div>
                                                                <div class="rd-card-body">
                                                                    <div class="rd-metric-label">
                                                                        Disputed Amount
                                                                    </div>
                                                                    <div class="rd-metric-value">
                                                                        <%#: FormatAmount(Eval("Disputed_OnUs_EJAmount")) %>
                                                                    </div>
                                                                    <div class="rd-metric-count">
                                                                        <div class="rd-metric-label">
                                                                            Disputed Transactions
                                                                        </div>
                                                                        <div class="rd-metric-value">
                                                                            <%#: FormatCount(Eval("Disputed_OnUs_EJ_Transactions")) %>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="rd-col rd-w-33">
                                                            <div class="rd-card rd-sw">
                                                                <div class="rd-card-head">
                                                                    <span class="rd-source-dot"></span>
                                                                    Switch
                                                                </div>
                                                                <div class="rd-card-body">
                                                                    <div class="rd-metric-label">
                                                                        Disputed Amount
                                                                    </div>
                                                                    <div class="rd-metric-value">
                                                                        <%#: FormatAmount(Eval("Disputed_OnUs_SwitchAmount")) %>
                                                                    </div>
                                                                    <div class="rd-metric-count">
                                                                        <div class="rd-metric-label">
                                                                            Disputed Transactions
                                                                        </div>
                                                                        <div class="rd-metric-value">
                                                                            <%#: FormatCount(Eval("Disputed_OnUs_Switch_Transactions")) %>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="rd-col rd-w-33">
                                                            <div class="rd-card rd-host">
                                                                <div class="rd-card-head">
                                                                    <span class="rd-source-dot"></span>
                                                                    Host
                                                                </div>
                                                                <div class="rd-card-body">
                                                                    <div class="rd-metric-label">
                                                                        Disputed Amount
                                                                    </div>
                                                                    <div class="rd-metric-value">
                                                                        <%#: FormatAmount(Eval("Disputed_OnUs_HostAmount")) %>
                                                                    </div>
                                                                    <div class="rd-metric-count">
                                                                        <div class="rd-metric-label">
                                                                            Disputed Transactions
                                                                        </div>
                                                                        <div class="rd-metric-value">
                                                                            <%#: FormatCount(Eval("Disputed_OnUs_Host_Transactions")) %>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Panel
                                                        ID="Panel_EmptyWithdrawal"
                                                        runat="server"
                                                        CssClass="rd-empty"
                                                        Visible='<%# ((Repeater)Container.NamingContainer).Items.Count == 0 %>'>
                                                        No Rows Found
                                                    </asp:Panel>
                                                    </div>
                                                </FooterTemplate>
                                            </asp:Repeater>
                                        </td>
                                    </tr>
                                </table>
                            </ComponentArt:PageView>
                            <ComponentArt:PageView
                                CssClass="PageContent"
                                runat="server"
                                ID="PageView1">
                                <table width="100%">
                                    <tr>
                                        <td>
                                            <div class="rd-section-header">
                                                <div class="rd-section-title">
                                                    Batch Summary
                                                </div>
                                                <div class="rd-section-description">
                                                    Transaction batch details and dispute overview
                                                </div>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td valign="top">
                                            <asp:Repeater
                                                ID="RepeaterDeposit"
                                                runat="server">
                                                <HeaderTemplate>
                                                    <div class="rd-wrap">
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <div class="rd-info">
                                                        <div class="rd-info-row">
                                                            <div class="rd-info-item">
                                                                <div class="rd-info-label">
                                                                    ATM Title
                                                                </div>
                                                                <div class="rd-info-value">
                                                                    <%#: Eval("atm_Id") %>
                                                                </div>
                                                            </div>
                                                            <div class="rd-info-item">
                                                                <div class="rd-info-label">
                                                                    Processing Date &amp; Time
                                                                </div>
                                                                <div class="rd-info-value">
                                                                    <%#: Eval("processing_DateTime") %>
                                                                </div>
                                                            </div>
                                                            <div class="rd-info-item">
                                                                <div class="rd-info-label">
                                                                    Transaction Date
                                                                </div>
                                                                <div class="rd-info-value">
                                                                    <%#: FormatTranDate(Eval("tran_Date")) %>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="rd-row">
                                                        <div class="rd-col rd-w-60">
                                                            <div class="rd-hero rd-hero-amount">
                                                                <div class="rd-hero-label">
                                                                    Total Disputed Amount
                                                                </div>
                                                                <div class="rd-hero-value">
                                                                    <%#: FormatAmount(Eval("Disputed_Amount_BNA")) %>
                                                                </div>
                                                                <div class="rd-hero-meta">
                                                                    Across all disputed sources
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="rd-col rd-w-40">
                                                            <div class="rd-hero rd-hero-count">
                                                                <div class="rd-hero-label">
                                                                    Disputed Transactions
                                                                </div>
                                                                <div class="rd-hero-value">
                                                                    <%#: FormatCount(Eval("Disputed_BNA_Transactions")) %>
                                                                </div>
                                                                <div class="rd-hero-meta">
                                                                    Total disputed transaction count
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="rd-section-header rd-source-header">
                                                        <div class="rd-section-title">
                                                            Dispute Breakdown
                                                        </div>
                                                        <div class="rd-section-description">
                                                            Disputed amounts and transaction counts by source
                                                        </div>
                                                    </div>
                                                    <div class="rd-row">
                                                        <div class="rd-col rd-w-33">
                                                            <div class="rd-card rd-ej">
                                                                <div class="rd-card-head">
                                                                    <span class="rd-source-dot"></span>
                                                                    EJ
                                                                </div>
                                                                <div class="rd-card-body">
                                                                    <div class="rd-metric-label">
                                                                        Disputed Amount
                                                                    </div>
                                                                    <div class="rd-metric-value">
                                                                        <%#: FormatAmount(Eval("Disputed_BNA_EJAmount")) %>
                                                                    </div>
                                                                    <div class="rd-metric-count">
                                                                        <div class="rd-metric-label">
                                                                            Disputed Transactions
                                                                        </div>
                                                                        <div class="rd-metric-value">
                                                                            <%#: FormatCount(Eval("Disputed_BNA_EJ_Transactions")) %>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="rd-col rd-w-33">
                                                            <div class="rd-card rd-sw">
                                                                <div class="rd-card-head">
                                                                    <span class="rd-source-dot"></span>
                                                                    Switch
                                                                </div>
                                                                <div class="rd-card-body">
                                                                    <div class="rd-metric-label">
                                                                        Disputed Amount
                                                                    </div>
                                                                    <div class="rd-metric-value">
                                                                        <%#: FormatAmount(Eval("Disputed_BNA_SwitchAmount")) %>
                                                                    </div>
                                                                    <div class="rd-metric-count">
                                                                        <div class="rd-metric-label">
                                                                            Disputed Transactions
                                                                        </div>
                                                                        <div class="rd-metric-value">
                                                                            <%#: FormatCount(Eval("Disputed_BNA_Switch_Transactions")) %>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="rd-col rd-w-33">
                                                            <div class="rd-card rd-host">
                                                                <div class="rd-card-head">
                                                                    <span class="rd-source-dot"></span>
                                                                    Host
                                                                </div>
                                                                <div class="rd-card-body">
                                                                    <div class="rd-metric-label">
                                                                        Disputed Amount
                                                                    </div>
                                                                    <div class="rd-metric-value">
                                                                        <%#: FormatAmount(Eval("Disputed_BNA_HostAmount")) %>
                                                                    </div>
                                                                    <div class="rd-metric-count">
                                                                        <div class="rd-metric-label">
                                                                            Disputed Transactions
                                                                        </div>
                                                                        <div class="rd-metric-value">
                                                                            <%#: FormatCount(Eval("Disputed_BNA_Host_Transactions")) %>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Panel
                                                        ID="Panel_EmptyDeposit"
                                                        runat="server"
                                                        CssClass="rd-empty"
                                                        Visible='<%# ((Repeater)Container.NamingContainer).Items.Count == 0 %>'>
                                                        No Rows Found
                                                    </asp:Panel>
                                                    </div>
                                                </FooterTemplate>
                                            </asp:Repeater>
                                        </td>
                                    </tr>
                                </table>
                            </ComponentArt:PageView>
                        </ComponentArt:MultiPage>
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp;
                    </td>
                </tr>
            </table>
        </div>
        <input
            type="button"
            runat="server"
            id="refresh"
            style="display: none" />
    </form>
</body>
</html>