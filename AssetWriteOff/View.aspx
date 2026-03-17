<%@ Page Title="View Asset Write-Off" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="View.aspx.cs" Inherits="Prodata.WebForm.AssetWriteOff.View" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    
    <style>
        /* --- SCREEN STYLES (Unchanged) --- */
        .full-screen-container { width: 100%; padding: 0 15px; }
        .card-custom { border: none; box-shadow: 0 4px 15px rgba(0,0,0,0.08); border-radius: 10px; margin-bottom: 30px; background-color: #fff; }
        .card-header-custom { background-color: #f8f9fa; border-bottom: 2px solid #e9ecef; border-radius: 10px 10px 0 0 !important; padding: 1.25rem 1.5rem; position: sticky; top: 0; z-index: 1000; }
        .section-title { color: #2c3e50; font-weight: 600; border-bottom: 2px solid #3498db; padding-bottom: 8px; margin-bottom: 20px; margin-top: 30px; font-size: 1.25rem; }
        .form-label { font-weight: 600; color: #6c757d; font-size: 0.9rem; text-transform: uppercase; letter-spacing: 0.5px; margin-bottom: 2px; }
        .view-data { font-size: 1.1rem; font-weight: 500; color: #212529; margin-bottom: 15px; display: block; }
        
        .table-custom th { background-color: #343a40 !important; color: #ffffff !important; font-weight: 500; vertical-align: middle !important; text-align: center; font-size: 0.85rem;}
        .table-custom td { vertical-align: middle; font-size: 0.9rem; }
        .totals-row td { background-color: #f8f9fa; font-weight: bold; color: #212529; font-size: 1rem; }
        
        /* Hide Print-Only elements on screen */
        .print-only { display: none !important; }

        /* ==========================================================================
           --- HIGH OFFICIAL PRINT STYLES (Landscape & Black Text) --- 
           ========================================================================== */
        @media print {
            @page { 
                size: A4 landscape; 
                margin: 10mm 15mm; 
            }

            body { 
                font-family: "Times New Roman", Times, serif !important; 
                background-color: #fff; 
                margin: 0; 
                padding: 0; 
                font-size: 10pt; 
                color: #000 !important; 
            }

            /* 1. FORCE ALL TEXT TO BLACK (Strict Overrides) */
            * { 
                color: #000 !important; 
                background-color: transparent !important; 
                box-shadow: none !important;
                text-shadow: none !important;
            }

            /* Explicitly override Bootstrap color classes */
            .text-primary, .text-success, .text-danger, .text-warning, .text-info, .text-muted, a, a:visited { 
                color: #000 !important; 
                text-decoration: none !important;
            }

            /* Strip Badge formatting for print so it looks like normal table text */
            .badge {
                border: none !important;
                padding: 0 !important;
                font-weight: normal !important;
                font-size: 9pt !important;
                background-color: transparent !important;
            }

            /* Hide all web UI elements */
            .btn, .card-tools, .navbar, footer, .sidebar, .d-print-none, .alert, .card-header-custom { 
                display: none !important; 
            }

            /* Reset container styling */
            .full-screen-container, .card-body, .card-custom, .card { 
                padding: 0 !important; 
                margin: 0 !important; 
                border: none !important; 
                box-shadow: none !important; 
                width: 100% !important;
            }

            /* Show Print-Only elements */
            .print-only { display: block !important; }
            .print-only-flex { display: flex !important; }

            /* Top Stamp (Moved from Footer) */
            .print-top-stamp {
                width: 100%;
                text-align: right;
                font-size: 8pt;
                font-family: monospace !important;
                color: #000 !important;
                border-bottom: 1px solid #000 !important;
                padding-bottom: 3px;
                margin-bottom: 15px;
            }
            .print-top-stamp span { float: left; font-family: "Times New Roman", Times, serif !important; color: #000 !important; font-weight: bold; }

            /* Official Header */
            .print-header { 
                border-bottom: 3px double #000 !important; 
                padding-bottom: 10px; 
                margin-bottom: 20px;
                display: flex !important;
                align-items: center;
                justify-content: space-between;
                color: #000 !important;
            }
            .print-header-logo img { width: 100px; height: auto; }
            .print-header-titles { text-align: right; color: #000 !important; }
            .print-header-titles h2 { font-size: 16pt; font-weight: bold; text-transform: uppercase; margin: 0; color: #000 !important; }
            .print-header-titles h4 { font-size: 11pt; margin: 5px 0 0 0; font-weight: normal; color: #000 !important; }

            /* Section Titles */
            .section-title { 
                font-size: 11pt; 
                font-weight: bold; 
                text-transform: uppercase; 
                border-bottom: 1px solid #000 !important; 
                padding-bottom: 2px; 
                margin-top: 15px; 
                margin-bottom: 8px; 
                color: #000 !important;
            }

            /* Labels and Data */
            .form-label { font-size: 8pt; color: #000 !important; font-weight: bold; }
            .view-data { font-size: 10pt; font-weight: bold; margin-bottom: 8px; color: #000 !important; }
            
            /* Justification Box */
            .view-data-box { 
                border: 1px solid #000 !important; 
                padding: 8px !important; 
                min-height: 60px; 
                font-weight: normal; 
                font-style: italic;
                color: #000 !important;
            }

            /* Table Formatting */
            .table-custom { width: 100%; border-collapse: collapse !important; margin-bottom: 15px; table-layout: auto; }
            .table-custom th, .table-custom td { 
                border: 1px solid #000 !important; 
                padding: 4px !important; 
                font-size: 8pt !important; 
                color: #000 !important;
            }
            
            /* Keep grey header backgrounds on paper for contrast, but text remains black */
            .table-custom th, .totals-row td { 
                font-weight: bold; 
                background-color: #f2f2f2 !important; 
                -webkit-print-color-adjust: exact; 
                print-color-adjust: exact;
                color: #000 !important;
            }

            /* Breaks */
            .page-break-inside-avoid { page-break-inside: avoid; }

            /* Signature Blocks */
            .signature-section { margin-top: 30px; page-break-inside: avoid; color: #000 !important;}
            .signature-box { 
                border-top: 1px solid #000 !important; 
                width: 220px; 
                text-align: center; 
                padding-top: 4px; 
                font-size: 9pt; 
                font-weight: bold;
                color: #000 !important;
            }
        }
    </style>

    <div class="full-screen-container">
        <asp:Panel ID="pnlPrintArea" runat="server" CssClass="card card-custom page-break-inside-avoid">

            <div class="card-header card-header-custom d-flex align-items-center d-print-none">
                <h3 class="card-title m-0 fw-bold text-dark">
                    <i class="fas fa-file-alt mr-2 text-primary"></i><%: Page.Title %>
                </h3>
                <div class="card-tools" style="margin-left: auto !important;">
                    
                    <asp:LinkButton ID="btnBack" runat="server" CssClass="btn btn-outline-secondary mr-2" PostBackUrl="~/AssetWriteOff/Default.aspx" CausesValidation="false">
                        <i class="fas fa-angle-double-left"></i> Back
                    </asp:LinkButton>
                    
                    <asp:LinkButton ID="btnExportExcel" runat="server" CssClass="btn btn-success mr-2" CausesValidation="false" OnClick="btnExportExcel_Click">
                        <i class="fas fa-file-excel"></i> Export Excel
                    </asp:LinkButton>

                    <asp:LinkButton ID="btnPrint" runat="server" CssClass="btn btn-primary" CausesValidation="false" OnClientClick="window.print(); return false;">
                        <i class="fas fa-print"></i> Print Document
                    </asp:LinkButton>
                </div>
            </div>

            <div class="card-body p-4">
                
                <div class="print-top-stamp print-only">
                    <span>Generated from CCMS System</span>
                    Printed On: <%= DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss") %>
                </div>

                <div class="print-header print-only">
                    <div class="print-header-logo">
                        <img src="<%= ResolveUrl("~/Images/Felda_Global_Ventures_Logo.png") %>" alt="Company Logo" />
                    </div>
                    <div class="print-header-titles">
                        <h2>Asset Write-Off Application Form</h2>
                        <h4>Internal Financial Control Document</h4>
                    </div>
                </div>

                <div class="alert alert-info d-flex align-items-center shadow-sm mb-4 d-print-none">
                    <i class="fa fa-industry fa-2x mr-3"></i>
                    <div>
                        <h5 class="m-0 font-weight-bold">Business Area (BA)</h5>
                        <asp:Label runat="server" ID="lblGlobalBA" CssClass="font-weight-normal" style="font-size: 1.1rem;" />
                    </div>
                </div>

                <h4 class="section-title mt-0">Application Information</h4>
                
                <div class="row">
                    <div class="col-md-3 col-sm-6 mb-2">
                        <label class="form-label">Reference No.</label>
                        <asp:Label ID="lblRefNo" runat="server" CssClass="view-data text-primary font-weight-bold" />
                    </div>
                    
                    <div class="col-md-3 col-sm-6 mb-2 print-only">
                        <label class="form-label">Business Area</label>
                        <asp:Label ID="lblPrintBA" runat="server" CssClass="view-data" /> 
                    </div>

                    <div class="col-md-3 col-sm-6 mb-2">
                        <label class="form-label">Application Date</label>
                        <asp:Label ID="lblDate" runat="server" CssClass="view-data" />
                    </div>
                    <div class="col-md-3 col-sm-6 mb-2">
                        <label class="form-label">Status</label>
                        <div><asp:Label ID="lblStatus" runat="server" CssClass="view-data" /></div>
                    </div>
                </div>

                <div class="row">
                    <div class="col-md-12 mb-2">
                        <label class="form-label">Project Name / Details</label>
                        <asp:Label ID="lblProject" runat="server" CssClass="view-data font-weight-bold" />
                    </div>
                </div>

                <h4 class="section-title mt-3">Asset Details</h4>
                
                <div class="table-responsive shadow-sm border rounded mb-3 page-break-inside-avoid" style="box-shadow: none !important; border: none !important;">
                    <table class="table table-bordered table-custom mb-0">
                        <thead>
                            <tr>
                                <th rowspan="2" style="width: 30px;">NO</th>
                                <th rowspan="2" style="min-width: 90px;">ASSET CODE</th>
                                <th rowspan="2" style="min-width: 150px;">ITEM DESCRIPTION</th>
                                <th colspan="2">DATE OF ACQUISITION</th>
                                <th rowspan="2" style="width: 70px;">USEFUL<br/>LIFE</th>
                                <th rowspan="2" style="width: 50px;">QTY</th>
                                <th colspan="3">PER UNIT PRICE (RM)</th>
                                <th rowspan="2" style="min-width: 130px;">REASON</th>
                            </tr>
                            <tr>
                                <th style="width: 90px; border-top: 1px solid #454d55;">DATE</th>
                                <th style="width: 50px; border-top: 1px solid #454d55;">AGE</th>
                                <th style="min-width: 90px; border-top: 1px solid #454d55;">ORIGINAL</th>
                                <th style="min-width: 90px; border-top: 1px solid #454d55;">ACC. DEPR.</th>
                                <th style="min-width: 90px; border-top: 1px solid #454d55;">NET BOOK VAL</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rptAssets" runat="server">
                                <ItemTemplate>
                                    <tr class="bg-light">
                                        <td class="text-center font-weight-bold"><%# Container.ItemIndex + 1 %></td>
                                        <td class="text-center"><%# Eval("AssetCode") %></td>
                                        <td><%# Eval("ItemDescription") %></td>
                                        <td class="text-center"><%# Eval("AcqDate", "{0:dd-MMM-yyyy}") %></td>
                                        <td class="text-center"><%# Eval("AgeYears") %></td>
                                        <td class="text-center"><%# Eval("UsefulLife") %></td>
                                        <td class="text-center"><%# Eval("Quantity") %></td>
                                        <td class="text-right font-weight-bold text-primary"><%# Eval("OriginalPrice", "{0:N2}") %></td>
                                        <td class="text-right font-weight-bold text-danger"><%# Eval("AccDepreciation", "{0:N2}") %></td>
                                        <td class="text-right font-weight-bold text-success"><%# Eval("NetBookValue", "{0:N2}") %></td>
                                        <td><%# Eval("Reason") %></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                        <tfoot class="totals-row">
                            <tr>
                                <td colspan="7" class="text-right text-uppercase pr-3 align-middle">Grand Total (RM)</td>
                                <td class="text-right text-primary"><asp:Label ID="lblTotalOrig" runat="server" Text="0.00" /></td>
                                <td class="text-right text-danger"><asp:Label ID="lblTotalAcc" runat="server" Text="0.00" /></td>
                                <td class="text-right text-success"><asp:Label ID="lblTotalNet" runat="server" Text="0.00" /></td>
                                <td></td>
                            </tr>
                        </tfoot>
                    </table>
                </div>

                <div class="row page-break-inside-avoid">
                    <div class="col-md-12 mb-3">
                        <h4 class="section-title mt-0">Justification</h4>
                        <div class="border rounded p-2 bg-light view-data view-data-box">
                            <asp:Literal ID="litJustification" runat="server" Mode="Encode" />
                        </div>
                    </div>
                </div>

                <div class="row page-break-inside-avoid d-print-none">
                    <div class="col-md-6 mb-3">
                        <asp:Panel runat="server" ID="pnlUploadedDocument" CssClass="card bg-light border-success h-100" Visible="false">
                            <div class="card-body">
                                <h5 class="card-title text-success"><i class="fas fa-paperclip"></i> Supporting Documents</h5>
                                <div class="bg-white p-2 rounded border mt-3">
                                    <asp:PlaceHolder ID="phDocumentList" runat="server" />
                                </div>
                            </div>
                        </asp:Panel>
                    </div>
                </div>

                <h4 class="section-title mt-4">Audit Trail / Workflow History</h4>
                <div class="table-responsive shadow-sm border rounded mb-4 page-break-inside-avoid" style="box-shadow: none !important; border: none !important;">
                    <asp:GridView ID="gvHistory" runat="server" CssClass="table table-bordered table-custom mb-0" AutoGenerateColumns="False" GridLines="None">
                        <HeaderStyle CssClass="bg-dark text-white" />
                        <Columns>
                            <asp:BoundField DataField="ActionDate" HeaderText="DATE & TIME" DataFormatString="{0:dd-MMM-yyyy HH:mm}" ItemStyle-CssClass="text-center" />
                            <asp:BoundField DataField="RoleName" HeaderText="ROLE" ItemStyle-CssClass="text-center" />
                            
                            <asp:TemplateField HeaderText="ACTION" ItemStyle-CssClass="text-center">
                                <ItemTemplate>
                                    <span class='badge <%# 
                                        Eval("ActionType").ToString() == "Approve" ? "badge-success" : 
                                        (Eval("ActionType").ToString() == "Reject" ? "badge-danger" : 
                                        (Eval("ActionType").ToString() == "Send Back" ? "badge-info" : "badge-secondary")) %>'>
                                        <%# Eval("ActionType") %>
                                    </span>
                                </ItemTemplate>
                            </asp:TemplateField>
                            
                            <asp:BoundField DataField="Status" HeaderText="STATUS" ItemStyle-CssClass="text-center font-weight-bold" />
                            <asp:BoundField DataField="Remarks" HeaderText="REMARKS" />
                        </Columns>
                    </asp:GridView>
                </div>

            </div>
        </asp:Panel>
    </div>

</asp:Content>