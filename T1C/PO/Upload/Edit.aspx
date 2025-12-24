<%@ Page Title="Upload PO" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="UploadPO.aspx.cs" Inherits="Prodata.WebForm.T1C.PO.Upload.UploadPO" %>



<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>

    <script src="https://cdn.jsdelivr.net/npm/bs-custom-file-input/dist/bs-custom-file-input.min.js"></script>



    <style>

        .page-preloader { position: fixed; z-index: 99999; top: 0; left: 0; width: 100%; height: 100%; background: rgba(0,0,0,0.6); display: flex; justify-content: center; align-items: center; flex-direction: column; }

        .page-preloader img { animation: shake 1.5s infinite; }

        @keyframes shake { 0% { transform: rotate(0deg); } 25% { transform: rotate(3deg); } 50% { transform: rotate(0deg); } 75% { transform: rotate(-3deg); } 100% { transform: rotate(0deg); } }



        .select2-container .select2-selection--single { height: calc(2.25rem + 2px) !important; padding: 0.375rem 0.75rem; }

        .select2-container--bootstrap4 .select2-selection--single .select2-selection__arrow { top: 4px !important; }

        .select2-container--bootstrap4 .select2-selection--single .select2-selection__rendered { line-height: 1.5 !important; padding-left: 0 !important; }

        .btnRemoveAllocation { width: 100%; }

    </style>



    <div id="pagePreloader" class="page-preloader" style="display:none;">

        <img src="<%= ResolveUrl("~/Images/Felda_Global_Ventures_Logo.png") %>" alt="Loading..." height="200" width="200" />

        <p class="mt-3 text-white">Processing...</p>

    </div>



    <asp:HiddenField ID="hdnFormId" runat="server" />

    <asp:HiddenField ID="hdnAllocationData" runat="server" />



    <div class="row">

        <div class="col-lg-12">

            <div class="card card-outline">

                <div class="card-header card-header-sticky">

                    <h3 class="card-title"><asp:Label ID="lblTitle" runat="server"></asp:Label></h3>

                    <div class="card-tools">

                        <asp:LinkButton ID="btnBack" runat="server" CssClass="btn btn-default" PostBackUrl="~/T1C/PO/Upload/Default" CausesValidation="false"><i class="fas fa-angle-double-left"></i> Back</asp:LinkButton>

                        <asp:LinkButton ID="btnSave" runat="server" CssClass="btn btn-primary" OnClick="btnSave_Click" OnClientClick="return beforeSubmitPageload();"><i class="fas fa-save"></i> Save</asp:LinkButton>

                    </div>

                </div>

                <div class="card-body">

                    <div class="row">

                        <div class="col-md-9 border-right">

                            <div class="form-group"><label class="text-bold">BA</label><div class="text-muted"><asp:Label ID="lblBA" runat="server"></asp:Label></div></div>

                            <div class="form-group"><label class="text-bold">Requester Name</label><div class="text-muted"><asp:Label ID="lblReqName" runat="server"></asp:Label></div></div>

                            <div class="form-group"><label class="text-bold">Details</label><div class="text-muted"><asp:Label ID="lblDetails" runat="server"></asp:Label></div></div>

                            <div class="form-group"><label class="text-bold">Justification of Need</label><div class="text-muted"><asp:Label ID="lblJustificationOfNeed" runat="server"></asp:Label></div></div>

                            <div class="form-group"><label class="text-bold">Allocations</label><div class="text-muted"><asp:Label ID="lblAllocation" runat="server"></asp:Label></div></div>

                            <div class="form-group"><label class="text-bold">Contractors</label><div class="text-muted"><asp:Label ID="lblVendor" runat="server"></asp:Label></div></div>

                            <div class="form-group" id="divJustificationDirectNegotiation" runat="server">

                                <label class="text-bold">Justification (Direct Negotiation)</label>

                                <div class="text-muted"><asp:Label ID="lblJustificationDirectAward" runat="server"></asp:Label></div>

                            </div>

                        </div>

                        <div class="col-md-3">

                            <div class="form-group"><label class="text-bold">Ref No.</label><div class="text-muted"><asp:Label ID="lblRefNo" runat="server"></asp:Label></div></div>

                            <div class="form-group"><label class="text-bold">Date</label><div class="text-muted"><asp:Label ID="lblDate" runat="server"></asp:Label></div></div>

                            <div class="form-group"><label class="text-bold">Estimate Amount</label><div class="text-warning text-bold"><asp:Label ID="lblAmount" runat="server"></asp:Label></div></div>

                            <div class="form-group"><label class="text-bold">Quotation Method</label><div class="text-muted"><asp:Label ID="lblProcurementType" runat="server"></asp:Label></div></div>

                            <div class="form-group"><label class="text-bold">Status</label><div><asp:Label ID="lblStatus" runat="server" CssClass="badge badge-pill"></asp:Label></div></div>

                        </div>

                    </div>



                    <hr />



                    <div class="row">

                        <div class="col-md-9">

                            <div class="form-group">

                                <label class="text-bold">Data Cost</label>

                                <div class="table-responsive">

                                    <table id="tblDataKos" runat="server" class="table table-bordered table-sm">

                                        <thead>

                                            <tr>

                                                <th></th><th colspan="2" class="text-center"><%= DateTime.Now.Year %> (RM/MT)</th><th colspan="3" class="text-center">YTD <%= DateTime.Now.Year - 1 %> (RM/MT)</th>

                                            </tr>

                                            <tr>

                                                <th></th><th class="text-center">Actual YTD</th><th class="text-center">Annual Budget</th><th class="text-center">Actual (RM)</th><th class="text-center">Actual</th><th class="text-center">Budget</th>

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

                                            <th class="text-center align-middle pl-1 pr-1">A</th><td style="width: 45%;"><asp:Label ID="lblA" runat="server" CssClass="form-control text-muted"></asp:Label></td>

                                            <th class="text-center align-middle pl-1 pr-1">C</th><td style="width: 45%;"><asp:Label ID="lblC" runat="server" CssClass="form-control text-muted"></asp:Label></td>

                                        </tr>

                                        <tr>

                                            <th class="text-center align-middle pl-1 pr-1">B</th><td style="width: 45%;"><asp:Label ID="lblB" runat="server" CssClass="form-control text-muted"></asp:Label></td>

                                            <th class="text-center align-middle pl-1 pr-1">D</th><td style="width: 45%;"><asp:Label ID="lblD" runat="server" CssClass="form-control text-muted"></asp:Label></td>

                                        </tr>

                                    </tbody>

                                </table>

                            </div>

                        </div>

                    </div>



                    <hr />



                    <div class="row">

                        <div class="col-md-12">

                            <div class="card card-success">

                                <div class="card-header" data-card-widget="collapse"><h3 class="card-title">Upload PO</h3></div>

                                <div class="card-body">

                                    <div class="row">

                                        <div class="col-md-12">

                                            <div class="form-group">

                                                <label>Upload PO</label>

                                                <div class="custom-file">

                                                    <asp:FileUpload ID="fuPO" runat="server" CssClass="custom-file-input" />

                                                    <label class="custom-file-label">Choose File</label>

                                                </div>

                                                <asp:Label ID="lblPOError" runat="server" CssClass="text-danger d-none"></asp:Label>

                                                <asp:Panel ID="pnlPOView" runat="server" Visible="false">

                                                    <asp:HyperLink ID="lnkPO" runat="server" Target="_blank" CssClass="btn btn-link">View PO</asp:HyperLink>

                                                </asp:Panel>

                                            </div>

                                        </div>

                                        <div class="col-md-6">

                                            <div class="form-group">

                                                <label>Actual Amount</label>

                                                <div class="input-group">

                                                    <div class="input-group-prepend"><span class="input-group-text">RM</span></div>

                                                    <asp:TextBox ID="txtActualAmount" runat="server" CssClass="form-control input-number2" placeholder="0.00"></asp:TextBox>

                                                </div>

                                                <asp:Label ID="lblActualAmountError" runat="server" CssClass="text-danger d-none" />

                                                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtActualAmount" CssClass="text-danger" Display="Dynamic" ErrorMessage="Please enter actual amount (RM)"></asp:RequiredFieldValidator>

                                            </div>

                                        </div>

                                        <div class="col-md-6">

                                            <div class="form-group">

                                                <label>Original Estimate</label>

                                                <div class="text-warning text-bold mt-2"><asp:Label ID="lblAmount2" runat="server" CssClass="text-lg"></asp:Label></div>

                                            </div>

                                        </div>



                                        <div class="col-md-12">

                                            <div id="allocationSection" runat="server" class="form-group d-none">

                                                <div class="d-flex justify-content-between align-items-center mb-2">

                                                    <asp:Label runat="server" CssClass="text-bold" Text="Allocate Balance (Savings)"></asp:Label>

                                                    <div>

                                                        Balance to Allocate: <span id="lblSavingsTarget" class="text-danger font-weight-bold">RM 0.00</span> | 

                                                        Allocated: <span id="lblSavingsAllocated" class="text-success font-weight-bold">RM 0.00</span>

                                                    </div>

                                                </div>



                                                <div class="alert alert-info py-2 small">

                                                    <i class="fas fa-info-circle"></i> Actual amount is less than estimate. Please allocate the savings back to the budgets.

                                                </div>
                                                <asp:Repeater ID="rptBudgetAllocations" runat="server">
                                                    <ItemTemplate>
                                                        <div class="mb-2">
                                                            <asp:HiddenField ID="hdnBudgetId" runat="server" Value='<%# Eval("BudgetId") %>' />
                                                            <div class="text-muted small"><%# Eval("Ref") %> – <%# Eval("Details") %></div>
                                                            <div class="input-group">
                                                                <div class="input-group-prepend"><span class="input-group-text">RM</span></div>
                                                                <asp:TextBox ID="txtAllocateAmount" runat="server" 
                                                                    Text='<%# Eval("AllocatedAmount", "{0:F2}") %>' 
                                                                    CssClass="form-control allocation-input" /> 
                                                            </div>
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:Repeater>

                                                <asp:Label ID="lblAllocationError" runat="server" CssClass="text-danger d-none"></asp:Label>

                                            </div>



                                            <div id="allocationDropdownSection" runat="server" class="form-group d-none">

                                                <hr />

                                                <div class="d-flex justify-content-between align-items-center mb-2">

                                                    <label class="text-bold">Allocate Excess Cost (Overrun)</label>

                                                    <div>Deficit: <span id="lblTotalDeficit" class="text-danger font-weight-bold">0.00</span> | Allocated: <span id="lblTotalAllocated" class="text-success font-weight-bold">0.00</span></div>

                                                </div>

                                                <div class="alert alert-warning py-2"><i class="fas fa-exclamation-triangle"></i> Actual amount exceeds estimate. Please select budget(s) to cover the difference.</div>

                                                <div id="allocationContainer">

                                                    <asp:Repeater ID="rptOverrunAllocations" runat="server" OnItemDataBound="rptOverrunAllocations_ItemDataBound">

                                                        <ItemTemplate>

                                                            <div class="row mb-2 allocation-group align-items-center">

                                                                <div class="col-md-8">

                                                                    <asp:HiddenField ID="hdnSelectedBudgetId" runat="server" Value='<%# Eval("BudgetId") %>' />

                                                                    <asp:DropDownList ID="ddlAllocationItem" runat="server" CssClass="form-control select2 allocation-dropdown" Width="100%"></asp:DropDownList>

                                                                    <small class="text-info budget-balance-lbl">Available Balance: RM 0.00</small>

                                                                </div>

                                                                <div class="col-md-3">

                                                                    <div class="input-group">

                                                                        <div class="input-group-prepend"><span class="input-group-text">RM</span></div>

                                                                        <asp:TextBox ID="txtAllocationAmountItem" runat="server" Text='<%# Eval("Amount", "{0:F2}") %>' CssClass="form-control allocation-amount" placeholder="0.00"></asp:TextBox>

                                                                    </div>

                                                                </div>

                                                                <div class="col-md-1"><button type="button" class="btn btn-danger btnRemoveAllocation"><i class="fa fa-minus"></i></button></div>

                                                            </div>

                                                        </ItemTemplate>

                                                    </asp:Repeater>

                                                </div>

                                                <div class="mt-2"><button type="button" class="btn btn-info btn-sm" id="btnAddAllocation"><i class="fa fa-plus"></i> Add Allocation Row</button></div>

                                                <asp:Label ID="lblDynamicError" runat="server" CssClass="text-danger d-none mt-2 d-block"></asp:Label>

                                            </div>

                                        </div>

                                    </div>

                                </div>

                            </div>

                        </div>

                    </div>



                    <div class="row">

                        <div class="col-md-12">

                            <div class="card card-secondary collapsed-card">

                                <div class="card-header" data-card-widget="collapse"><h3 class="card-title">Documents</h3></div>

                                <div class="card-body">

                                    <div class="row">

                                        <div class="col-12">

                                            <div class="form-group row"><label class="col-lg-3 col-form-label">Picture</label><div class="col-lg-9"><asp:Panel ID="pnlPictureView" runat="server" Visible="false"><asp:HyperLink ID="lnkPicture" runat="server" Target="_blank" Text="View Picture" CssClass="btn btn-link" /></asp:Panel><asp:Label ID="lblPictureDash" runat="server" Text="-" Visible="true" /></div></div><hr class="mt-0" />

                                            <div class="form-group row"><label class="col-lg-3 col-form-label">Machine Repair History</label><div class="col-lg-9"><asp:Panel ID="pnlMachineRepairHistoryView" runat="server" Visible="false"><asp:HyperLink ID="lnkMachineRepairHistory" runat="server" Target="_blank" Text="View File" CssClass="btn btn-link" /></asp:Panel><asp:Label ID="lblMachineHistoryDash" runat="server" Text="-" Visible="true" /></div></div><hr class="mt-0" />

                                            <div class="form-group row"><label class="col-lg-3 col-form-label">Job Specification</label><div class="col-lg-9"><asp:Panel ID="pnlJobSpecificationView" runat="server" Visible="false"><asp:HyperLink ID="lnkJobSpecification" runat="server" Target="_blank" Text="View File" CssClass="btn btn-link" /></asp:Panel><asp:Label ID="lblJobSpecificationDash" runat="server" Text="-" Visible="true" /></div></div><hr class="mt-0" />

                                            <div class="form-group row"><label class="col-lg-3 col-form-label">Engineer's Estimate Price</label><div class="col-lg-9"><asp:Panel ID="pnlEngineerEstimatePriceView" runat="server" Visible="false"><asp:HyperLink ID="lnkEngineerEstimatePrice" runat="server" Target="_blank" Text="View File" CssClass="btn btn-link" /></asp:Panel><asp:Label ID="lblEngineerEstimatePriceDash" runat="server" Text="-" Visible="true" /></div></div><hr class="mt-0" />

                                            <div class="form-group row"><label class="col-lg-3 col-form-label">Dec Cost Report (Current Year)</label><div class="col-lg-9"><asp:Panel ID="pnlDecCostReportCurrentYearView" runat="server" Visible="false"><asp:HyperLink ID="lnkDecCostReportCurrentYear" runat="server" Target="_blank" Text="View File" CssClass="btn btn-link" /></asp:Panel><asp:Label ID="lblDecCostReportCurrentYearDash" runat="server" Text="-" Visible="true" /></div></div><hr class="mt-0" />

                                            <div class="form-group row"><label class="col-lg-3 col-form-label">Dec Cost Report (Last Year)</label><div class="col-lg-9"><asp:Panel ID="pnlDecCostReportLastYearView" runat="server" Visible="false"><asp:HyperLink ID="lnkDecCostReportLastYear" runat="server" Target="_blank" Text="View File" CssClass="btn btn-link" /></asp:Panel><asp:Label ID="lblDecCostReportLastYearDash" runat="server" Text="-" Visible="true" /></div></div><hr class="mt-0" />

                                            <div class="form-group row"><label class="col-lg-3 col-form-label">Cost Report (Last Month)</label><div class="col-lg-9"><asp:Panel ID="pnlCostReportLastMonthView" runat="server" Visible="false"><asp:HyperLink ID="lnkCostReportLastMonth" runat="server" Target="_blank" Text="View File" CssClass="btn btn-link" /></asp:Panel><asp:Label ID="lblCostReportLastMonthDash" runat="server" Text="-" Visible="true" /></div></div><hr class="mt-0" />

                                            <div class="form-group row"><label class="col-lg-3 col-form-label">Drawing/Sketching</label><div class="col-lg-9"><asp:Panel ID="pnlDrawingSketchingView" runat="server" Visible="false"><asp:HyperLink ID="lnkDrawingSketching" runat="server" Target="_blank" Text="View File" CssClass="btn btn-link" /></asp:Panel><asp:Label ID="lblDrawingSketchingDash" runat="server" Text="-" Visible="true" /></div></div><hr class="mt-0" />

                                            <div class="form-group row"><label class="col-lg-3 col-form-label">Quotation</label><div class="col-lg-9"><asp:Panel ID="pnlQuotationView" runat="server" Visible="false"><asp:HyperLink ID="lnkQuotation" runat="server" Target="_blank" Text="View File" CssClass="btn btn-link" /></asp:Panel><asp:Label ID="lblQuotationDash" runat="server" Text="-" Visible="true" /></div></div><hr class="mt-0" />

                                            <div class="form-group row"><label class="col-lg-3 col-form-label">Damage Investigation Report</label><div class="col-lg-9"><asp:Panel ID="pnlDamageInvestigationReportView" runat="server" Visible="false"><asp:HyperLink ID="lnkDamageInvestigationReport" runat="server" Target="_blank" Text="View File" CssClass="btn btn-link" /></asp:Panel><asp:Label ID="lblDamageInvestigationReportDash" runat="server" Text="-" Visible="true" /></div></div><hr class="mt-0" />

                                            <div class="form-group row"><label class="col-lg-3 col-form-label">Vendor Registration Record</label><div class="col-lg-9"><asp:Panel ID="pnlVendorRegistrationRecordView" runat="server" Visible="false"><asp:HyperLink ID="lnkVendorRegistrationRecord" runat="server" Target="_blank" Text="View File" CssClass="btn btn-link" /></asp:Panel><asp:Label ID="lblVendorRegistrationRecordDash" runat="server" Text="-" Visible="true" /></div></div><hr class="mt-0" />

                                            <div class="form-group row"><label class="col-lg-3 col-form-label">Budget Transfer/Add Approval</label><div class="col-lg-9"><asp:Panel ID="pnlBudgetTransferAddApprovalView" runat="server" Visible="false"><asp:HyperLink ID="lnkBudgetTransferAddApproval" runat="server" Target="_blank" Text="View File" CssClass="btn btn-link" /></asp:Panel><asp:Label ID="lblBudgetTransferAddApprovalDash" runat="server" Text="-" Visible="true" /></div></div><hr class="mt-0" />

                                            <div class="form-group row"><label class="col-lg-3 col-form-label">Other Supporting Document</label><div class="col-lg-9"><asp:Panel ID="pnlOtherSupportingDocumentView" runat="server" Visible="false"><asp:HyperLink ID="lnkOtherSupportingDocument" runat="server" Target="_blank" Text="View File" CssClass="btn btn-link" /></asp:Panel><asp:Label ID="lblOtherSupportingDocumentDash" runat="server" Text="-" Visible="true" /></div></div>

                                        </div>

                                    </div>

                                </div>

                            </div>

                        </div>

                    </div>



                    <div class="row">

                        <div class="col-md-12">

                            <div class="card card-secondary collapsed-card">

                                <div class="card-header" data-card-widget="collapse"><h3 class="card-title">Audit Trails</h3></div>

                                <div class="card-body">

                                    <div class="row">

                                        <div class="col-12">

                                            <asp:UpdatePanel ID="UpdatePanel1" runat="server">

                                                <ContentTemplate>

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

                                                </ContentTemplate>

                                            </asp:UpdatePanel>

                                        </div>

                                    </div>

                                </div>

                            </div>

                        </div>

                    </div>

                </div>

            </div>

        </div>

    </div>



    <script>
        // Global variable containing budget data from C#
        var availableBudgets = <%= BudgetsJson %>;

        $(document).ready(function () {
            // Initialize existing plugins
            bsCustomFileInput.init();
            initPlugins();

            // 1. Trigger logic on Actual Amount change
            $('#<%= txtActualAmount.ClientID %>').on('input', function () {
            toggleAllocationSections();
            updateSavingsSummary();
            calculateDynamicTotal();
        });

        // 2. Add row for Overrun (Dynamic rows)
        $('#btnAddAllocation').click(function (e) {
            e.preventDefault();
            addNewRow();
        });

        // 3. Remove row logic (Delegated for dynamic elements)
        $('#allocationContainer').on('click', '.btnRemoveAllocation', function () {
            $(this).closest('.allocation-group').fadeOut(200, function () {
                $(this).remove();
                calculateDynamicTotal();
            });
        });

        // 4. Update balance display when a budget is selected in dropdown
        $('#allocationContainer').on('change', '.allocation-dropdown', function () {
            updateBalanceDisplay($(this));
        });

        // 5. Input Masking & Real-time Totals (Delegated)
        $('#allocationContainer, #allocationSection').on('input', '.allocation-amount, .allocation-input', function () {
            var val = $(this).val().replace(/[^0-9.]/g, '');
            if ((val.match(/\./g) || []).length > 1) val = val.substring(0, val.lastIndexOf('.'));
            $(this).val(val);

            // If it's an overrun amount, check if it exceeds the selected budget's balance
            if ($(this).hasClass('allocation-amount')) {
                checkBalanceLimit($(this));
            }

            calculateDynamicTotal();
            updateSavingsSummary();
        });

        // 6. Formatting on blur (force two decimal places)
        $('#allocationContainer, #allocationSection').on('blur', '.allocation-amount, .allocation-input', function () {
            var val = $(this).val();
            if (val && !isNaN(parseFloat(val))) {
                $(this).val(parseFloat(val).toFixed(2));
            }
            calculateDynamicTotal();
            updateSavingsSummary();
        });

        // Initial setup on page load
        toggleAllocationSections();
        $('.allocation-dropdown').each(function () { updateBalanceDisplay($(this)); });
    });

        function initPlugins() {
            $('.select2').select2({ theme: 'bootstrap4', width: '100%' });
        }

        // --- NEW ROW LOGIC ---
        function addNewRow() {
            var options = '<option value="">Select Budget Source</option>';
            availableBudgets.forEach(function (item) {
                options += '<option value="' + item.BudgetId + '">' + item.DisplayText + '</option>';
            });

            var html = `
            <div class="row mb-2 allocation-group align-items-center">
                <div class="col-md-8">
                    <select class="form-control select2 allocation-dropdown" style="width:100%">${options}</select>
                    <small class="text-info budget-balance-lbl">Available Balance: RM 0.00</small>
                </div>
                <div class="col-md-3">
                    <div class="input-group">
                        <div class="input-group-prepend"><span class="input-group-text">RM</span></div>
                        <input type="text" class="form-control allocation-amount" placeholder="0.00" />
                    </div>
                </div>
                <div class="col-md-1">
                    <button type="button" class="btn btn-danger btnRemoveAllocation"><i class="fa fa-minus"></i></button>
                </div>
            </div>`;

            var $newRow = $(html);
            $('#allocationContainer').append($newRow);

            // Re-initialize Select2 for the new row
            $newRow.find('.select2').select2({ theme: 'bootstrap4', width: '100%' });
        }

        // --- BALANCE DISPLAY LOGIC ---
        function updateBalanceDisplay($dropdown) {
            var selectedId = $dropdown.val();
            var budget = availableBudgets.find(b => b.BudgetId == selectedId);
            var $label = $dropdown.closest('.allocation-group').find('.budget-balance-lbl');

            if (budget) {
                $label.text('Available Balance: RM ' + budget.Balance.toLocaleString('en-MY', { minimumFractionDigits: 2 }));
                $dropdown.data('max', budget.Balance);
            } else {
                $label.text('Available Balance: RM 0.00');
                $dropdown.data('max', 0);
            }
        }

        function checkBalanceLimit($input) {
            var $row = $input.closest('.allocation-group');
            var max = parseFloat($row.find('.allocation-dropdown').data('max')) || 0;
            var currentVal = parseDecimal($input.val());

            if (currentVal > max) {
                $input.addClass('is-invalid');
                // Optional: Auto-correct to max if you want
                // $input.val(max.toFixed(2));
            } else {
                $input.removeClass('is-invalid');
            }
        }

        // --- SUMMARY LOGIC ---
        function updateSavingsSummary() {
            var actualAmt = parseDecimal($('#<%= txtActualAmount.ClientID %>').val());
            var estimateAmt = parseDecimal($('#<%= lblAmount2.ClientID %>').text());

            // Calculate how much we NEED to give back
            var targetSavings = estimateAmt - actualAmt;
            if (targetSavings < 0) targetSavings = 0;

            var currentAllocated = 0;

            // Use a more specific selector to find inputs inside the Savings section
            $('.allocation-input').each(function () {
                var val = parseDecimal($(this).val());
                currentAllocated += val;
            });

            // Update the UI
            $('#lblSavingsTarget').text('RM ' + targetSavings.toLocaleString('en-MY', { minimumFractionDigits: 2 }));
            $('#lblSavingsAllocated').text('RM ' + currentAllocated.toLocaleString('en-MY', { minimumFractionDigits: 2 }));

            // Toggle Colors
            if (Math.abs(currentAllocated - targetSavings) < 0.01) {
                $('#lblSavingsAllocated').removeClass('text-danger').addClass('text-success');
            } else {
                $('#lblSavingsAllocated').removeClass('text-success').addClass('text-danger');
            }
        }
        // This catches typing in the Repeater textboxes
        $(document).on('input', '.allocation-input', function () {
            updateSavingsSummary();
        });

        function calculateDynamicTotal() {
            var totalAllocated = 0;
            $('#allocationContainer .allocation-amount').each(function () {
                totalAllocated += parseDecimal($(this).val());
            });

            var actual = parseDecimal($('#<%= txtActualAmount.ClientID %>').val());
        var estimate = parseDecimal($('#<%= lblAmount2.ClientID %>').text());
            var deficit = Math.max(0, actual - estimate);

            $('#lblTotalAllocated').text('RM ' + totalAllocated.toLocaleString('en-MY', { minimumFractionDigits: 2 }));
            $('#lblTotalDeficit').text('RM ' + deficit.toLocaleString('en-MY', { minimumFractionDigits: 2 }));

            var isMatch = Math.abs(totalAllocated - deficit) < 0.01;
            $('#lblTotalAllocated').toggleClass('text-success', isMatch).toggleClass('text-danger', !isMatch);
        }

        function toggleAllocationSections() {
            var actual = parseDecimal($('#<%= txtActualAmount.ClientID %>').val());
        var estimate = parseDecimal($('#<%= lblAmount2.ClientID %>').text());

        $('#<%= allocationSection.ClientID %>').addClass('d-none');
        $('#<%= allocationDropdownSection.ClientID %>').addClass('d-none');

        if (actual > 0) {
            if (actual < estimate) {
                $('#<%= allocationSection.ClientID %>').removeClass('d-none');
            } else if (actual > estimate) {
                $('#<%= allocationDropdownSection.ClientID %>').removeClass('d-none');
                if ($('#allocationContainer .allocation-group').length === 0) addNewRow();
            }
        }
    }

    // --- DATA COLLECTION FOR POSTBACK ---
    function collectAllocationData() {
        var allocations = [];
        $('#allocationContainer .allocation-group').each(function () {
            var id = $(this).find('.allocation-dropdown').val();
            var amt = $(this).find('.allocation-amount').val();
            if (id && amt) {
                allocations.push({ BudgetId: id, Amount: parseDecimal(amt) });
            }
        });
        $('#<%= hdnAllocationData.ClientID %>').val(JSON.stringify(allocations));
    }

    function parseDecimal(value) {
        if (!value) return 0;
        var cleanValue = value.toString().replace(/,/g, '').replace(/RM/i, '').trim();
        return isNaN(parseFloat(cleanValue)) ? 0 : parseFloat(cleanValue);
    }

    function beforeSave() {
        collectAllocationData();
        var actualVal = parseDecimal($('#<%= txtActualAmount.ClientID %>').val());
        var estimateVal = parseDecimal($('#<%= lblAmount2.ClientID %>').text());

        // 1. Validate Savings
        if (!$('#<%= allocationSection.ClientID %>').hasClass('d-none')) {
            var target = estimateVal - actualVal;
            var total = 0;
            $('#<%= allocationSection.ClientID %> .allocation-input').each(function() { total += parseDecimal($(this).val()); });
            if (Math.abs(total - target) > 0.01) {
                Swal.fire('Allocation Incomplete', 'Savings allocation must match the balance (RM ' + target.toFixed(2) + ').', 'error');
                return false;
            }
        }
        
        // 2. Validate Overrun
        if (!$('#<%= allocationDropdownSection.ClientID %>').hasClass('d-none')) {
                var deficit = actualVal - estimateVal;
                var totalOverrun = 0;
                var balanceExceeded = false;

                $('#allocationContainer .allocation-group').each(function () {
                    var amt = parseDecimal($(this).find('.allocation-amount').val());
                    var max = parseFloat($(this).find('.allocation-dropdown').data('max')) || 0;
                    totalOverrun += amt;
                    if (amt > max) balanceExceeded = true;
                });

                if (balanceExceeded) {
                    Swal.fire('Budget Exceeded', 'One or more allocations exceed the available budget balance.', 'error');
                    return false;
                }

                if (Math.abs(totalOverrun - deficit) > 0.01) {
                    Swal.fire('Allocation Incomplete', 'Overrun allocations must match the total deficit (RM ' + deficit.toFixed(2) + ').', 'error');
                    return false;
                }
            }
            return true;
        }

        function beforeSubmitPageload() {
            if (!beforeSave()) return false;
            $("#pagePreloader").fadeIn(200);
            return true;
        }
    </script>

</asp:Content>