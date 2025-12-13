<%@ Page Title="Transfer Application Form" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Application.aspx.cs" Inherits="Prodata.WebForm.Budget.Transfer.TransferApplication.Application" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    
    <link href="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/css/select2.min.css" rel="stylesheet" />
    <link href="https://cdn.jsdelivr.net/npm/select2-bootstrap-5-theme@1.3.0/dist/select2-bootstrap-5-theme.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/js/select2.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>

    <style>
        .card-header-sticky { position: sticky; top: 0; z-index: 1000; background-color: #fff; border-bottom: 1px solid rgba(0,0,0,.125); }
        .form-label { font-weight: 600; color: #495057; }
        .txt-readonly { background-color: #e9ecef !important; cursor: not-allowed; text-align: right; font-family: 'Consolas', monospace; }
        .txt-amount { text-align: right; font-family: 'Consolas', monospace; }
        
        /* Select2 Tweak for Table Alignment */
        .select2-container .select2-selection--single { height: 38px; border: 1px solid #ced4da; }
        .select2-container--default .select2-selection--single .select2-selection__rendered { line-height: 36px; }
        .select2-container--default .select2-selection--single .select2-selection__arrow { height: 36px; }
    </style>

    <asp:Panel runat="server" CssClass="card shadow-sm mb-4">
        
        <div class="card-header card-header-sticky d-flex align-items-center py-3">
            <div class="d-flex align-items-center flex-grow-1">
                <asp:LinkButton ID="btnBack" runat="server" CssClass="btn btn-outline-secondary me-3"
                    PostBackUrl="~/Budget/Transfer/TransferApplication/Default" CausesValidation="false">
                    <i class="fas fa-arrow-left"></i> Back
                </asp:LinkButton>
                <h5 class="card-title m-0 text-primary"><i class="fas fa-file-invoice-dollar me-2"></i> Budget Transfer Application</h5>
            </div>

            <div class="ms-auto">
                <asp:LinkButton ID="btnSubmit" runat="server" CssClass="btn btn-success shadow-sm px-4 fw-bold"
                    OnClick="btnSubmit_Click" OnClientClick="return validateAndSubmit();">
                    <i class="fas fa-check-circle me-2"></i> Finalize Transfer
                </asp:LinkButton>
            </div>
        </div>

        <div class="card-body">
            
            <div class="row mb-4">
                <div class="col-md-12">
                    <div class="alert alert-light border">
                        <div class="row">
                            <div class="col-md-6">
                                <table class="table table-borderless table-sm m-0">
                                    <tr><td class="text-muted" style="width:120px;">Ref No:</td><td class="fw-bold"><asp:Label ID="lblRef" runat="server" /></td></tr>
                                    <tr><td class="text-muted">Applicant:</td><td><asp:Label ID="lblApplicantName" runat="server" /></td></tr>
                                    <tr><td class="text-muted">Mill/BA:</td><td><asp:Label ID="lblBA" runat="server" /> - <asp:Label ID="LblBAName" runat="server" /></td></tr>
                                    <tr><td class="text-muted">Date:</td><td><asp:Label ID="lblDate" runat="server" /></td></tr>
                                </table>
                            </div>
                            <div class="col-md-6 border-start">
                                <table class="table table-borderless table-sm m-0">
                                    <tr><td class="text-muted" style="width:120px;">Status:</td><td><asp:Literal ID="litStatusHtml" runat="server" /></td></tr>
                                    <tr><td class="text-muted">Transfer:</td><td><i class="fas fa-exchange-alt text-muted me-1"></i> <asp:Label ID="lblTransferInfo" runat="server" /></td></tr>
                                    <tr><td class="text-muted">Requested:</td><td class="fw-bold text-success fs-5"><asp:Label ID="lblAmount" runat="server" /></td></tr>
                                    <tr><td class="text-muted">Justification:</td><td><small class="fst-italic"><asp:Label ID="lblReason" runat="server" /></small></td></tr>
                                </table>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <h5 class="border-bottom pb-2 mb-3">Budget Allocation Source</h5>
            <div class="table-responsive mb-4">
                <table class="table table-bordered table-hover align-middle" id="allocationTable">
                    <thead class="table-light">
                        <tr class="text-center">
                            <th style="width: 45%;">Source Budget Reference</th>
                            <th style="width: 20%;">Available Balance</th>
                            <th style="width: 25%;">Allocate Amount (RM)</th>
                            <th style="width: 10%;">Action</th>
                        </tr>
                    </thead>
                    <tbody>
                        </tbody>
                    <tfoot class="table-light">
                        <tr>
                            <td colspan="2" class="text-end align-middle fw-bold">Total Allocated:</td>
                            <td>
                                <input type="text" id="txtTotalAllocated" class="form-control txt-readonly fw-bold" value="0.00" readonly />
                            </td>
                            <td></td>
                        </tr>
                        <tr id="rowError" style="display:none;">
                            <td colspan="4" class="text-center text-danger fw-bold">
                                <i class="fas fa-exclamation-triangle"></i> Total allocation must exactly match the requested amount.
                            </td>
                        </tr>
                    </tfoot>
                </table>
                <button type="button" class="btn btn-sm btn-outline-primary mt-2" onclick="addNewRow()">
                    <i class="fas fa-plus"></i> Add Source
                </button>
            </div>

            <div class="form-group">
                <label for="txtRemarks" class="form-label">Approval Remarks <span class="text-danger">*</span></label>
                <asp:TextBox ID="txtRemarks" runat="server" TextMode="MultiLine" CssClass="form-control" Rows="3" placeholder="Enter remarks for this approval..." />
                <asp:RequiredFieldValidator ID="rfvRemarks" runat="server" ControlToValidate="txtRemarks" CssClass="text-danger small" ErrorMessage="Remarks are required." Display="Dynamic" />
            </div>

        </div>
    </asp:Panel>

    <asp:HiddenField ID="hdnTargetAmount" runat="server" />
    <asp:HiddenField ID="hdnBudgetJson" runat="server" />

    <script type="text/javascript">
        var budgetList = [];
        var targetAmount = 0;

        $(document).ready(function () {
            // 1. Load Data
            var jsonString = $('#<%= hdnBudgetJson.ClientID %>').val();
            if (jsonString) {
                try {
                    budgetList = JSON.parse(jsonString);
                } catch (e) {
                    console.error("Error parsing budget JSON", e);
                }
            }

            var targetStr = $('#<%= hdnTargetAmount.ClientID %>').val();
            targetAmount = parseFloat(targetStr) || 0;

            // 2. Add Initial Row
            if (budgetList.length > 0) {
                addNewRow();
            } else {
                Swal.fire('Warning', 'No budget sources found with available balance.', 'warning');
            }
        });

        // --- Core Functions ---

        function addNewRow() {
            var index = $("#allocationTable tbody tr").length;

            // 1. Pre-build Option String (Fixes "No data" issue)
            var optionsHtml = '<option value="">-- Select Budget Ref --</option>';
            budgetList.forEach(function (b) {
                optionsHtml += `<option value="${b.Ref}">${b.Display}</option>`;
            });

            // 2. Build Row HTML with Options Pre-injected
            var html = `
                <tr data-index="${index}">
                    <td>
                        <select class="form-control select2 ddl-budget" style="width:100%" onchange="onBudgetChange(this)">
                            ${optionsHtml}
                        </select>
                        <input type="hidden" name="allocations[${index}][BudgetRef]" class="hdn-ref" />
                        <input type="hidden" name="allocations[${index}][BudgetId]" class="hdn-id" />
                    </td>
                    <td>
                        <input type="text" class="form-control txt-readonly txt-balance" value="0.00" readonly />
                    </td>
                    <td>
                        <input type="text" name="allocations[${index}][Amount]" class="form-control txt-amount" 
                               placeholder="0.00" oninput="formatAndCalc(this)" />
                    </td>
                    <td class="text-center">
                        <button type="button" class="btn btn-sm btn-outline-danger" onclick="removeRow(this)">
                            <i class="fas fa-trash-alt"></i>
                        </button>
                    </td>
                </tr>`;

            // 3. Append & Initialize
            $("#allocationTable tbody").append(html);

            // Init Select2 on the LAST added row only
            var $newRow = $("#allocationTable tbody tr").last();
            $newRow.find('.ddl-budget').select2({
                placeholder: "Select Budget",
                allowClear: true,
                theme: 'bootstrap-5'
            });
        }

        function removeRow(btn) {
            if ($("#allocationTable tbody tr").length > 1) {
                $(btn).closest('tr').remove();
                recalculateTotal();
            } else {
                Swal.fire({ icon: 'info', title: 'Info', text: 'At least one allocation row is required.' });
            }
        }

        // --- Logic: Prevent Duplicates & Populate Data ---
        function onBudgetChange(select) {
            var $select = $(select);
            var $row = $select.closest('tr');
            var currentVal = $select.val();

            // 1. Exit if cleared
            if (!currentVal) {
                clearRowData($row);
                recalculateTotal();
                return;
            }

            // 2. Duplicate Check
            var isDuplicate = false;
            $(".ddl-budget").not($select).each(function () {
                if ($(this).val() === currentVal) {
                    isDuplicate = true;
                    return false; // Break loop
                }
            });

            if (isDuplicate) {
                Swal.fire({
                    icon: 'warning',
                    title: 'Duplicate Selection',
                    text: 'This Budget Reference is already selected in another row.',
                    confirmButtonColor: '#f39c12'
                });

                // Clear the selection safely
                $select.val(null).trigger('change.select2');
                return;
            }

            // 3. Populate Data
            var budget = budgetList.find(b => b.Ref === currentVal);
            if (budget) {
                $row.find('.hdn-ref').val(budget.Ref);
                $row.find('.hdn-id').val(budget.Id);
                $row.find('.txt-balance').val(budget.Balance.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 }));
            } else {
                clearRowData($row);
            }

            // Reset Amount to force user verification
            $row.find('.txt-amount').val('');
            recalculateTotal();
        }

        function clearRowData($row) {
            $row.find('.hdn-ref').val('');
            $row.find('.hdn-id').val('');
            $row.find('.txt-balance').val('0.00');
        }

        // --- Math & Validation ---
        function formatAndCalc(input) {
            // Allow numbers and one dot
            var val = input.value.replace(/[^0-9.]/g, '');
            var parts = val.split('.');
            if (parts.length > 2) val = parts[0] + '.' + parts.slice(1).join('');
            input.value = val;
            recalculateTotal();
        }

        function recalculateTotal() {
            var total = 0;
            var isValid = true;

            $(".txt-amount").each(function () {
                var rowVal = parseFloat($(this).val()) || 0;
                var $row = $(this).closest('tr');

                // Get balance from text input (remove commas)
                var balStr = $row.find('.txt-balance').val().replace(/,/g, '');
                var balance = parseFloat(balStr) || 0;

                if (rowVal > balance) {
                    $(this).addClass('is-invalid');
                    isValid = false;
                } else {
                    $(this).removeClass('is-invalid');
                }
                total += rowVal;
            });

            $("#txtTotalAllocated").val(total.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 }));

            // Check against Target (+/- 0.01 tolerance)
            if (Math.abs(total - targetAmount) > 0.01) {
                $("#txtTotalAllocated").addClass('is-invalid text-danger');
                $("#rowError").show();
                isValid = false;
            } else {
                $("#txtTotalAllocated").removeClass('is-invalid text-danger');
                $("#txtTotalAllocated").addClass('text-success');
                $("#rowError").hide();
            }

            return isValid;
        }

        function validateAndSubmit() {
            // 1. Math Check (Allocations vs Total)
            if (!recalculateTotal()) {
                Swal.fire({
                    icon: 'warning',
                    title: 'Validation Error',
                    text: 'Please check amounts: Allocations must not exceed balance, and Total must match request.'
                });
                return false;
            }

            // 2. Empty Selection Check
            var empty = false;
            $(".ddl-budget").each(function () {
                if (!$(this).val()) empty = true;
            });

            if (empty) {
                Swal.fire({ icon: 'error', title: 'Error', text: 'Please select a budget source for all rows.' });
                return false;
            }

            // 3. ASP.NET Client Side Validation (Fixes Stuck Loader)
            // If the Remarks field (RequiredFieldValidator) is invalid, Page_ClientValidate will return false.
            if (typeof (Page_ClientValidate) == 'function') {
                if (!Page_ClientValidate()) {
                    // Do NOT show loader, just return false so user can see the error message on the textbox
                    return false;
                }
            }

            // 4. Show loading state (Only if everything is valid)
            Swal.fire({
                title: 'Processing...',
                text: 'Submitting transfer...',
                allowOutsideClick: false,
                didOpen: () => { Swal.showLoading(); }
            });

            return true;
        }
    </script>
</asp:Content>