<%@ Page Title="PO Review" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Review.aspx.cs" Inherits="Prodata.WebForm.T1C.PO.Review.Review" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>

    <style>
        /* --- PRELOADER STYLES --- */
        .page-preloader {
            position: fixed;
            z-index: 99999;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background: rgba(0,0,0,0.6);
            display: flex;
            justify-content: center;
            align-items: center;
            flex-direction: column;
        }
        .page-preloader img { animation: shake 1.5s infinite; }
        @keyframes shake {
            0% { transform: rotate(0deg); }
            25% { transform: rotate(3deg); }
            50% { transform: rotate(0deg); }
            75% { transform: rotate(-3deg); }
            100% { transform: rotate(0deg); }
        }

        /* --- PRINT SPECIFIC STYLES --- */
        @media print {
            /* 1. Hide everything by default (Sidebar, Navbar, Buttons) */
            body * {
                visibility: hidden;
            }
            
            /* 2. Unhide the specific print container and its contents */
            #printContainer, #printContainer * {
                visibility: visible;
            }

            /* 3. Position the print container at the top left of the paper */
            #printContainer {
                position: absolute;
                left: 0;
                top: 0;
                width: 100%;
                background-color: white;
            }

            /* 4. Hide Buttons, Tools, and specific 'no-print' elements within the container */
            .card-tools, .no-print, .btn {
                display: none !important;
            }

            /* 5. Clean up Card Styling for Print (Remove shadows/borders) */
            .card {
                border: none !important;
                box-shadow: none !important;
                margin-bottom: 20px !important;
            }
            .card-header {
                border-bottom: 2px solid #000 !important;
                background-color: transparent !important;
                padding-left: 0;
            }
            .card-title {
                font-weight: bold;
                font-size: 18pt;
            }

            /* 6. Show the Logo (It is d-none on screen) */
            .print-logo-header {
                display: block !important;
                text-align: center;
                margin-bottom: 30px;
                border-bottom: 3px solid #333;
                padding-bottom: 20px;
            }
            
            /* 7. Adjust columns for print width */
            .col-md-9, .col-md-3, .col-md-6, .col-md-12 {
                flex: 0 0 100%;
                max-width: 100%;
            }
            
            /* 8. Ensure background colors (like badges) print */
            -webkit-print-color-adjust: exact; 
            print-color-adjust: exact;
        }
    </style>

    <div id="pagePreloader" class="page-preloader" style="display:none;">
        <img src="<%= ResolveUrl("~/Images/Felda_Global_Ventures_Logo.png") %>" alt="Loading..." height="200" width="200" />
        <p class="mt-3 text-white">Processing...</p>
    </div>

    <asp:HiddenField ID="hdnFormId" runat="server" />
    <asp:HiddenField ID="hdnRemark" runat="server" />
    <asp:Button ID="btnApproveConfirm" runat="server" CssClass="d-none" OnClick="btnReviewed_Click" />

    <div id="printContainer">
        
        <div class="print-logo-header d-none">
            <img src="<%= ResolveUrl("~/Images/Felda_Global_Ventures_Logo.png") %>" alt="FGV Logo" style="height: 80px;" />
            <h2 class="mt-3">Purchase Order Review</h2>
            <p>Generated on: <%= DateTime.Now.ToString("dd/MM/yyyy HH:mm") %></p>
        </div>

        <div class="row">
            <div class="col-lg-12">
                <div class="card card-outline">
                    <div class="card-header card-header-sticky">
                        <h3 class="card-title">
                            <asp:Label ID="lblTitle" runat="server"></asp:Label>
                        </h3>
                        <div class="card-tools">
                            <button type="button" class="btn btn-default mr-1" onclick="window.print(); return false;">
                                <i class="fas fa-print"></i> Print
                            </button>

                            <asp:LinkButton ID="btnBack" runat="server" CssClass="btn btn-default" PostBackUrl="~/T1C/PO/Review/Default" CausesValidation="false">
                                <i class="fas fa-angle-double-left"></i> Back
                            </asp:LinkButton>
                            
                            <button id="btnReviewTrigger" runat="server" type="button" class="btn btn-primary" onclick="showApproveModal(); return false;">
                                <i class="fas fa-lock"></i> Reviewed
                            </button>
                        </div>
                    </div>
                    
                    <div class="card-body">
                        <div class="row">
                            <div class="col-md-9 border-right">
                                <div class="form-group">
                                    <asp:Label runat="server" CssClass="text-bold" Text="BA"></asp:Label>
                                    <div class="text-muted"><asp:Label ID="lblBA" runat="server"></asp:Label></div>
                                </div>
                                <div class="form-group">
                                    <asp:Label runat="server" CssClass="text-bold" Text="Requester Name"></asp:Label>
                                    <div class="text-muted"><asp:Label ID="lblReqName" runat="server"></asp:Label></div>
                                </div>
                                <div class="form-group">
                                    <asp:Label runat="server" CssClass="text-bold" Text="Details"></asp:Label>
                                    <div class="text-muted"><asp:Label ID="lblDetails" runat="server"></asp:Label></div>
                                </div>
                                <div class="form-group">
                                    <asp:Label runat="server" CssClass="text-bold" Text="Justification of Need"></asp:Label>
                                    <div class="text-muted"><asp:Label ID="lblJustificationOfNeed" runat="server"></asp:Label></div>
                                </div>
                                <div class="form-group">
                                    <asp:Label runat="server" CssClass="text-bold" Text="Allocations (Original)"></asp:Label>
                                    <div class="text-muted"><asp:Label ID="lblAllocation" runat="server"></asp:Label></div>
                                </div>
                                <div class="form-group">
                                    <asp:Label runat="server" CssClass="text-bold" Text="Contractors"></asp:Label>
                                    <div class="text-muted"><asp:Label ID="lblVendor" runat="server"></asp:Label></div>
                                </div>
                                <div class="form-group" id="divJustificationDirectNegotiation" runat="server" visible="false">
                                    <asp:Label runat="server" CssClass="text-bold" Text="Justification (Direct Negotiation)"></asp:Label>
                                    <div class="text-muted"><asp:Label ID="lblJustificationDirectAward" runat="server"></asp:Label></div>
                                </div>
                            </div>
                            
                            <div class="col-md-3">
                                <div class="form-group">
                                    <asp:Label runat="server" CssClass="text-bold" Text="Reference No."></asp:Label>
                                    <div class="text-muted"><asp:Label ID="lblRefNo" runat="server"></asp:Label></div>
                                </div>
                                <div class="form-group">
                                    <asp:Label runat="server" CssClass="text-bold" Text="Date"></asp:Label>
                                    <div class="text-muted"><asp:Label ID="lblDate" runat="server"></asp:Label></div>
                                </div>
                                <div class="form-group">
                                    <asp:Label runat="server" CssClass="text-bold" Text="Estimate Amount"></asp:Label>
                                    <div class="text-warning text-bold"><asp:Label ID="lblAmount" runat="server"></asp:Label></div>
                                </div>
                                <div class="form-group">
                                    <asp:Label runat="server" CssClass="text-bold" Text="Actual Amount"></asp:Label>
                                    <div class="text-success text-bold"><asp:Label ID="lblActualAmountView" runat="server" Text="-"></asp:Label></div>
                                </div>
                                <div class="form-group">
                                    <asp:Label runat="server" CssClass="text-bold" Text="Quotation Method"></asp:Label>
                                    <div class="text-muted"><asp:Label ID="lblProcurementType" runat="server"></asp:Label></div>
                                </div>
                                <div class="form-group">
                                    <asp:Label runat="server" CssClass="text-bold" Text="Status"></asp:Label>
                                    <div><asp:Label ID="lblStatus" runat="server" CssClass="badge badge-pill"></asp:Label></div>
                                </div>
                            </div>
                        </div>

                        <div class="row mt-3">
                            <div class="col-md-9">
                                <div class="form-group">
                                    <asp:Label runat="server" CssClass="text-bold" Text="Data Cost"></asp:Label>
                                    <div class="table-responsive">
                                        <table class="table table-bordered table-sm">
                                            <thead>
                                                <tr>
                                                    <th></th>
                                                    <th colspan="2" class="text-center"><%= DateTime.Now.Year %> (RM/MT)</th>
                                                    <th colspan="3" class="text-center">YTD <%= DateTime.Now.Year - 1 %> (RM/MT)</th>
                                                </tr>
                                                <tr>
                                                    <th></th>
                                                    <th class="text-center">Actual YTD</th>
                                                    <th class="text-center">Annual Budget</th>
                                                    <th class="text-center">Actual (RM)</th>
                                                    <th class="text-center">Actual</th>
                                                    <th class="text-center">Budget</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr>
                                                    <th class="text-nowrap align-middle">S & M</th>
                                                    <td class="text-center"><asp:Label ID="lblCurrentYearActualYTD" runat="server" CssClass="text-muted"></asp:Label></td>
                                                    <td class="text-center"><asp:Label ID="lblCurrentYearBudget" runat="server" CssClass="text-muted"></asp:Label></td>
                                                    <td class="text-center"><asp:Label ID="lblPreviousYearActualYTD" runat="server" CssClass="text-muted"></asp:Label></td>
                                                    <td class="text-center"><asp:Label ID="lblPreviousYearActual" runat="server" CssClass="text-muted"></asp:Label></td>
                                                    <td class="text-center"><asp:Label ID="lblPreviousYearBudget" runat="server" CssClass="text-muted"></asp:Label></td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                    <table style="width: 100%;">
                                        <tbody>
                                            <tr>
                                                <th class="text-center align-middle pl-1 pr-1">A</th>
                                                <td style="width: 45%;"><asp:Label ID="lblA" runat="server" CssClass="form-control text-muted"></asp:Label></td>
                                                <th class="text-center align-middle pl-1 pr-1">C</th>
                                                <td style="width: 45%;"><asp:Label ID="lblC" runat="server" CssClass="form-control text-muted"></asp:Label></td>
                                            </tr>
                                            <tr>
                                                <th class="text-center align-middle pl-1 pr-1">B</th>
                                                <td style="width: 45%;"><asp:Label ID="lblB" runat="server" CssClass="form-control text-muted"></asp:Label></td>
                                                <th class="text-center align-middle pl-1 pr-1">D</th>
                                                <td style="width: 45%;"><asp:Label ID="lblD" runat="server" CssClass="form-control text-muted"></asp:Label></td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                        </div>

                        <div class="row mt-3">
                            <div class="col-md-12">
                                <div class="card card-outline card-primary">
                                    <div class="card-header">
                                        <h3 class="card-title">Financials & Allocations</h3>
                                    </div>
                                    <div class="card-body">
                                        <div class="row">
                                            <div class="col-md-6">
                                                <div class="form-group">
                                                    <label>Actual Amount</label>
                                                    <div class="form-control-plaintext font-weight-bold text-success" style="font-size: 1.2em;">
                                                        <asp:Label ID="lblActualAmount" runat="server" Text="-"></asp:Label>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="col-md-6">
                                                <div class="form-group">
                                                    <label>Estimate Amount</label>
                                                    <div class="form-control-plaintext font-weight-bold text-warning" style="font-size: 1.2em;">
                                                        <asp:Label ID="lblEstimateAmount" runat="server" Text="-"></asp:Label>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <hr />
                                        <div class="row">
                                            <div class="col-md-12">
                                                <label>Allocation Breakdown</label>
                                                <asp:Repeater ID="rptAllocationView" runat="server">
                                                    <ItemTemplate>
                                                        <div class="d-flex justify-content-between align-items-center border-bottom py-2">
                                                            <div>
                                                                <span class="text-bold"><%# Eval("Ref") %></span>
                                                                <span class="text-muted mx-2">-</span>
                                                                <span class="text-muted"><%# Eval("Details") %></span>
                                                            </div>
                                                            <div class="font-weight-bold">
                                                                RM <%# Eval("AllocatedAmount", "{0:N2}") %>
                                                            </div>
                                                        </div>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:Label ID="lblEmpty" runat="server" Visible='<%# rptAllocationView.Items.Count == 0 %>' Text="No allocations made." CssClass="text-muted font-italic"></asp:Label>
                                                    </FooterTemplate>
                                                </asp:Repeater>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="row mt-3">
                            <div class="col-md-12">
                                <div class="card card-success">
                                    <div class="card-header">
                                        <h3 class="card-title">Uploaded PO</h3>
                                    </div>
                                    <div class="card-body">
                                        <div class="form-group">
                                            <asp:Label runat="server" Text="Purchase Order File: " CssClass="font-weight-bold mr-2"></asp:Label>
                                            
                                            <asp:Panel ID="pnlPOView" runat="server" Visible="false" CssClass="d-inline">
                                                <asp:HyperLink ID="lnkPO" runat="server" Target="_blank" CssClass="btn btn-outline-primary btn-sm"></asp:HyperLink>
                                            </asp:Panel>
                                            
                                            <asp:Label ID="lblPONotUploaded" runat="server" Text="No PO uploaded yet." CssClass="text-muted font-italic" Visible="false"></asp:Label>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="row mt-3 no-print">
                            <div class="col-md-12">
                                <div class="card card-secondary collapsed-card">
                                    <div class="card-header" data-card-widget="collapse" style="cursor:pointer;">
                                        <h3 class="card-title">Supporting Documents</h3>
                                        <div class="card-tools">
                                            <button type="button" class="btn btn-tool" data-card-widget="collapse"><i class="fas fa-plus"></i></button>
                                        </div>
                                    </div>
                                    <div class="card-body" style="display:none;">
                                        <div class="form-group row"><asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Picture"></asp:Label><div class="col-lg-9"><asp:Panel ID="pnlPictureView" runat="server" Visible="false"><asp:HyperLink ID="lnkPicture" runat="server" Target="_blank" Text="View" CssClass="btn btn-link" /></asp:Panel><asp:Label ID="lblPictureDash" runat="server" Text="-" Visible="true" /></div></div><hr class="mt-0" />
                                        <div class="form-group row"><asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Machine Repair History"></asp:Label><div class="col-lg-9"><asp:Panel ID="pnlMachineRepairHistoryView" runat="server" Visible="false"><asp:HyperLink ID="lnkMachineRepairHistory" runat="server" Target="_blank" Text="View" CssClass="btn btn-link" /></asp:Panel><asp:Label ID="lblMachineHistoryDash" runat="server" Text="-" Visible="true" /></div></div><hr class="mt-0" />
                                        <div class="form-group row"><asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Job Specification"></asp:Label><div class="col-lg-9"><asp:Panel ID="pnlJobSpecificationView" runat="server" Visible="false"><asp:HyperLink ID="lnkJobSpecification" runat="server" Target="_blank" Text="View" CssClass="btn btn-link" /></asp:Panel><asp:Label ID="lblJobSpecificationDash" runat="server" Text="-" Visible="true" /></div></div><hr class="mt-0" />
                                        <div class="form-group row"><asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Engineer's Estimate Price"></asp:Label><div class="col-lg-9"><asp:Panel ID="pnlEngineerEstimatePriceView" runat="server" Visible="false"><asp:HyperLink ID="lnkEngineerEstimatePrice" runat="server" Target="_blank" Text="View" CssClass="btn btn-link" /></asp:Panel><asp:Label ID="lblEngineerEstimatePriceDash" runat="server" Text="-" Visible="true" /></div></div><hr class="mt-0" />
                                        <div class="form-group row"><asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Dec Cost Report (Current Year)"></asp:Label><div class="col-lg-9"><asp:Panel ID="pnlDecCostReportCurrentYearView" runat="server" Visible="false"><asp:HyperLink ID="lnkDecCostReportCurrentYear" runat="server" Target="_blank" Text="View" CssClass="btn btn-link" /></asp:Panel><asp:Label ID="lblDecCostReportCurrentYearDash" runat="server" Text="-" Visible="true" /></div></div><hr class="mt-0" />
                                        <div class="form-group row"><asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Dec Cost Report (Last Year)"></asp:Label><div class="col-lg-9"><asp:Panel ID="pnlDecCostReportLastYearView" runat="server" Visible="false"><asp:HyperLink ID="lnkDecCostReportLastYear" runat="server" Target="_blank" Text="View" CssClass="btn btn-link" /></asp:Panel><asp:Label ID="lblDecCostReportLastYearDash" runat="server" Text="-" Visible="true" /></div></div><hr class="mt-0" />
                                        <div class="form-group row"><asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Cost Report (Last Month)"></asp:Label><div class="col-lg-9"><asp:Panel ID="pnlCostReportLastMonthView" runat="server" Visible="false"><asp:HyperLink ID="lnkCostReportLastMonth" runat="server" Target="_blank" Text="View" CssClass="btn btn-link" /></asp:Panel><asp:Label ID="lblCostReportLastMonthDash" runat="server" Text="-" Visible="true" /></div></div><hr class="mt-0" />
                                        <div class="form-group row"><asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Drawing/Sketching"></asp:Label><div class="col-lg-9"><asp:Panel ID="pnlDrawingSketchingView" runat="server" Visible="false"><asp:HyperLink ID="lnkDrawingSketching" runat="server" Target="_blank" Text="View" CssClass="btn btn-link" /></asp:Panel><asp:Label ID="lblDrawingSketchingDash" runat="server" Text="-" Visible="true" /></div></div><hr class="mt-0" />
                                        <div class="form-group row"><asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Quotation"></asp:Label><div class="col-lg-9"><asp:Panel ID="pnlQuotationView" runat="server" Visible="false"><asp:HyperLink ID="lnkQuotation" runat="server" Target="_blank" Text="View" CssClass="btn btn-link" /></asp:Panel><asp:Label ID="lblQuotationDash" runat="server" Text="-" Visible="true" /></div></div><hr class="mt-0" />
                                        <div class="form-group row"><asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Damage Investigation Report"></asp:Label><div class="col-lg-9"><asp:Panel ID="pnlDamageInvestigationReportView" runat="server" Visible="false"><asp:HyperLink ID="lnkDamageInvestigationReport" runat="server" Target="_blank" Text="View" CssClass="btn btn-link" /></asp:Panel><asp:Label ID="lblDamageInvestigationReportDash" runat="server" Text="-" Visible="true" /></div></div><hr class="mt-0" />
                                        <div class="form-group row"><asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Vendor Registration Record"></asp:Label><div class="col-lg-9"><asp:Panel ID="pnlVendorRegistrationRecordView" runat="server" Visible="false"><asp:HyperLink ID="lnkVendorRegistrationRecord" runat="server" Target="_blank" Text="View" CssClass="btn btn-link" /></asp:Panel><asp:Label ID="lblVendorRegistrationRecordDash" runat="server" Text="-" Visible="true" /></div></div><hr class="mt-0" />
                                        <div class="form-group row"><asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Budget Transfer/Add Approval"></asp:Label><div class="col-lg-9"><asp:Panel ID="pnlBudgetTransferAddApprovalView" runat="server" Visible="false"><asp:HyperLink ID="lnkBudgetTransferAddApproval" runat="server" Target="_blank" Text="View" CssClass="btn btn-link" /></asp:Panel><asp:Label ID="lblBudgetTransferAddApprovalDash" runat="server" Text="-" Visible="true" /></div></div><hr class="mt-0" />
                                        <div class="form-group row"><asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Other Supporting Document"></asp:Label><div class="col-lg-9"><asp:Panel ID="pnlOtherSupportingDocumentView" runat="server" Visible="false"><asp:HyperLink ID="lnkOtherSupportingDocument" runat="server" Target="_blank" Text="View" CssClass="btn btn-link" /></asp:Panel><asp:Label ID="lblOtherSupportingDocumentDash" runat="server" Text="-" Visible="true" /></div></div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="row mt-3 no-print">
                            <div class="col-md-12">
                                <div class="card card-secondary collapsed-card">
                                    <div class="card-header" data-card-widget="collapse" style="cursor:pointer;">
                                        <h3 class="card-title">Audit Trails</h3>
                                        <div class="card-tools">
                                            <button type="button" class="btn btn-tool" data-card-widget="collapse"><i class="fas fa-plus"></i></button>
                                        </div>
                                    </div>
                                    <div class="card-body" style="display:none;">
                                        <div class="table-responsive">
                                            <asp:GridView ID="gvAuditTrails" runat="server" AutoGenerateColumns="false" CssClass="table table-bordered" PageSize='<%# FGV.Prodata.App.Setting.RecordsPerPage() %>' AllowPaging="true" OnPageIndexChanging="gvAuditTrails_PageIndexChanging" EmptyDataText="No record.">
                                                <Columns>
                                                    <asp:TemplateField HeaderText="#"><HeaderStyle CssClass="width-30 text-center" /><ItemStyle CssClass="width-30 text-center" /><ItemTemplate><%# Container.DataItemIndex + 1 %></ItemTemplate></asp:TemplateField>
                                                    <asp:BoundField DataField="ActionByName" HeaderText="Name" />
                                                    <asp:BoundField DataField="ActionByRole" HeaderText="Role" />
                                                    <asp:BoundField DataField="Action" HeaderText="Action" />
                                                    <asp:BoundField DataField="Remark" HeaderText="Remark" />
                                                    <asp:BoundField DataField="Datetime" HeaderText="Date & Time" HeaderStyle-CssClass="text-nowrap" ItemStyle-CssClass="text-nowrap" />
                                                </Columns>
                                                <PagerSettings Mode="NumericFirstLast" PageButtonCount="5" />
                                                <PagerStyle CssClass="pagination-ys" />
                                            </asp:GridView>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                    </div>
                </div>
            </div>
        </div>
        
    </div> <script type="text/javascript">
        function showApproveModal() {
            Swal.fire({
                title: 'Confirm Review',
                text: "Please enter a remark for this review action. \n\n(This action will lock the PO for editing)",

                // --- USE NATIVE INFO ANIMATION WITH LOCK ICON ---
                icon: 'info', 
                iconHtml: '<i class="fas fa-lock"></i>', 
                // ---------------------------------

                input: 'textarea',
                inputPlaceholder: 'Enter your remark here... (Required)',
                showCancelButton: true,
                confirmButtonColor: '#28a745',
                cancelButtonColor: '#6c757d',
                confirmButtonText: 'Confirm Review',
                preConfirm: (remark) => {
                    if (!remark || !remark.trim()) {
                        Swal.showValidationMessage('Please enter a remark to proceed.');
                        return false;
                    }
                    return remark;
                }
            }).then((result) => {
                if (result.isConfirmed) {
                    document.getElementById('<%= hdnRemark.ClientID %>').value = result.value;
                    $("#pagePreloader").fadeIn(200);
                    document.getElementById('<%= btnApproveConfirm.ClientID %>').click();
                }
            });
               }
    </script>

</asp:Content>