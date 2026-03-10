<%@ Page Title="Transfer Application Form" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ApplicationV2.aspx.cs" Inherits="Prodata.WebForm.Budget.Transfer.TransferApplication.ApplicationV2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    
    <link href="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/css/select2.min.css" rel="stylesheet" />
    <link href="https://cdn.jsdelivr.net/npm/select2-bootstrap-5-theme@1.3.0/dist/select2-bootstrap-5-theme.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/js/select2.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>

    <style>
        .full-screen-container { width: 100%; padding: 0 15px; }
        .card-custom { border: none; box-shadow: 0 4px 15px rgba(0,0,0,0.08); border-radius: 10px; margin-bottom: 30px; background-color: #fff; }
        .card-header-custom { background-color: #f8f9fa; border-bottom: 2px solid #e9ecef; border-radius: 10px 10px 0 0 !important; padding: 1.25rem 1.5rem; position: sticky; top: 0; z-index: 1000; }
        .section-title { color: #2c3e50; font-weight: 600; border-bottom: 2px solid #3498db; padding-bottom: 8px; margin-bottom: 20px; font-size: 1.25rem; }
        
        .txt-readonly { background-color: #e9ecef !important; cursor: not-allowed; text-align: right; font-family: 'Consolas', monospace; font-weight: bold;}
        .txt-amount { text-align: right; font-family: 'Consolas', monospace; font-weight: bold; color: #dc3545;}
        
        /* Select2 Tweak for Table Alignment */
        .select2-container .select2-selection--single { height: 38px; border: 1px solid #ced4da; }
        .select2-container--default .select2-selection--single .select2-selection__rendered { line-height: 36px; }
        .select2-container--default .select2-selection--single .select2-selection__arrow { height: 36px; }
        
        /* Preloader */
        .page-preloader { position: fixed; z-index: 99999; top: 0; left: 0; width: 100%; height: 100%; background: rgba(0,0,0,0.7); display: flex; justify-content: center; align-items: center; flex-direction: column; }
        .page-preloader img { animation: pulse 1.5s infinite; }
        @keyframes pulse { 0% { transform: scale(0.95); opacity: 0.8; } 50% { transform: scale(1.05); opacity: 1; } 100% { transform: scale(0.95); opacity: 0.8; } }
    </style>

    <div id="pagePreloader" class="page-preloader" style="display:none;">
        <img src="<%= ResolveUrl("~/Images/Felda_Global_Ventures_Logo.png") %>" alt="Loading..." height="150" width="150" />
        <h4 class="mt-3 text-white font-weight-bold">Processing Application...</h4>
    </div>

    <div class="full-screen-container">
        <asp:Panel runat="server" CssClass="card card-custom">
            
            <div class="card-header card-header-sticky d-flex align-items-center">
                <h3 class="card-title m-0 fw-bold text-dark"><i class="fas fa-file-invoice-dollar mr-2 text-primary"></i> Budget Allocation</h3>
                
                <div class="card-tools" style="margin-left: auto !important;">
                    <asp:HiddenField ID="hdnRemarks" runat="server" />
                    
                    <asp:LinkButton ID="btnBack" runat="server" CssClass="btn btn-outline-secondary mr-3" PostBackUrl="~/Budget/Transfer/TransferApplication/Default" CausesValidation="false">
                        <i class="fas fa-arrow-left"></i> Back
                    </asp:LinkButton>
                    
                    <button type="button" id="btnClientReject" class="btn btn-danger shadow-sm px-4 font-weight-bold mr-2" onclick="initiateRejection()">
                        <i class="fas fa-times-circle mr-1"></i> Reject
                    </button>
                    <button type="button" id="btnClientSubmit" class="btn btn-success shadow-sm px-4 font-weight-bold" onclick="initiateSubmission()">
                        <i class="fas fa-check-circle mr-1"></i> Finalize Transfer
                    </button>

                    <asp:Button ID="btnReject" runat="server" OnClick="btnReject_Click" style="display:none;" />
                    <asp:Button ID="btnServerSubmit" runat="server" OnClick="btnSubmit_Click" style="display:none;" />
                </div>
            </div>

            <div class="card-body p-4">
                
                <div class="alert alert-light border shadow-sm rounded mb-4 p-3">
                    <div class="row align-items-center">
                        <div class="col-md-7 border-right">
                            <h5 class="text-secondary mb-3"><i class="fas fa-info-circle"></i> Request Information</h5>
                            <table class="table table-borderless table-sm m-0">
                                <tr><td class="text-muted font-weight-bold" style="width:140px;">Reference No:</td><td class="font-weight-bold text-primary"><asp:Label ID="lblRef" runat="server" /></td></tr>
                                <tr><td class="text-muted font-weight-bold">Applicant:</td><td><asp:Label ID="lblApplicantName" runat="server" /></td></tr>
                                <tr><td class="text-muted font-weight-bold">Date:</td><td><asp:Label ID="lblDate" runat="server" /></td></tr>
                                <tr><td class="text-muted font-weight-bold">Project / Details:</td><td class="font-italic"><asp:Label ID="lblReason" runat="server" /></td></tr>
                            </table>
                        </div>
                        <div class="col-md-5 pl-4">
                            <h5 class="text-secondary mb-3"><i class="fas fa-exchange-alt"></i> Transfer Route</h5>
                            <div class="mb-3">
                                <span class="badge badge-danger p-2 mb-1" style="font-size:0.9rem;">FROM: <asp:Label ID="lblFromBA" runat="server" /></span><br/>
                                <i class="fas fa-arrow-down text-muted ml-3 mb-1"></i><br/>
                                <span class="badge badge-success p-2" style="font-size:0.9rem;">TO: <asp:Label ID="lblToBA" runat="server" /></span>
                            </div>
                            <div>
                                <span class="text-muted font-weight-bold d-block">Total Requested Amount:</span>
                                <span class="font-weight-bold text-success" style="font-size: 2rem;"><asp:Label ID="lblAmount" runat="server" /></span>
                            </div>
                        </div>
                    </div>
                </div>

                <h4 class="section-title mt-4">Source Budget Allocation (HQ)</h4>
                <p class="text-muted mb-3"><i class="fas fa-magic text-warning"></i> <strong>Auto-Set Applied:</strong> The allocation table below has been automatically pre-filled based on the specific budgets the user requested. Please verify the available balances before finalizing.</p>
                
                <div class="table-responsive mb-4 shadow-sm border rounded">
                    <table class="table table-bordered table-hover align-middle m-0" id="allocationTable">
                        <thead class="thead-dark text-center">
                            <tr>
                                <th style="width: 20%;">Requested Type</th>
                                <th style="width: 35%;">Source Budget Reference</th>
                                <th style="width: 15%;">Available Balance (RM)</th>
                                <th style="width: 20%;">Deduct Amount (RM)</th>
                                <th style="width: 10%;">Action</th>
                            </tr>
                        </thead>
                        <tbody>
                        </tbody>
                        <tfoot class="bg-light">
                            <tr>
                                <td colspan="3" class="text-right align-middle font-weight-bold" style="font-size: 1.1rem;">Total Allocated:</td>
                                <td>
                                    <input type="text" id="txtTotalAllocated" class="form-control txt-readonly text-success" style="font-size: 1.1rem;" value="0.00" readonly />
                                </td>
                                <td></td>
                            </tr>
                            <tr id="rowError" style="display:none;">
                                <td colspan="5" class="text-center text-danger font-weight-bold py-3 bg-white">
                                    <i class="fas fa-exclamation-triangle fa-lg mr-2"></i> Total allocation must exactly match the requested transfer amount.
                                </td>
                            </tr>
                        </tfoot>
                    </table>
                </div>
                
               <%-- <div class="text-right">
                    <button type="button" class="btn btn-outline-primary shadow-sm" onclick="addNewRow('', 0)">
                        <i class="fas fa-plus-circle mr-1"></i> Add Manual Source Line
                    </button>
                </div>--%>

            </div>
        </asp:Panel>
    </div>

    <asp:HiddenField ID="hdnTargetAmount" runat="server" />
    <asp:HiddenField ID="hdnBudgetJson" runat="server" />
    <asp:HiddenField ID="hdnRequestedAllocationsJson" runat="server" />

    <script type="text/javascript">
        var budgetList = [];
        var requestedAllocations = [];
        var targetAmount = 0;

        $(document).ready(function () {
            // 1. Load Available Budgets Database
            var jsonString = $('#<%= hdnBudgetJson.ClientID %>').val();
            if (jsonString) {
                try { budgetList = JSON.parse(jsonString); } catch (e) { console.error("Error parsing budget JSON"); }
            }

            // 2. Load User's Requested Allocations
            var reqJsonString = $('#<%= hdnRequestedAllocationsJson.ClientID %>').val();
            if (reqJsonString) {
                try { requestedAllocations = JSON.parse(reqJsonString); } catch (e) { console.error("Error parsing requested JSON"); }
            }

            var targetStr = $('#<%= hdnTargetAmount.ClientID %>').val();
            targetAmount = parseFloat(targetStr) || 0;

            // 3. Auto-Set Initial Rows strictly based on requested types
            if (budgetList.length > 0) {
                if (requestedAllocations.length > 0) {
                    requestedAllocations.forEach(function (req) {
                        // Find matching budget by TypeId.
                        var matchingBudget = budgetList.find(b => b.TypeId === req.TypeId && b.Balance >= req.Amount);
                        if (!matchingBudget) matchingBudget = budgetList.find(b => b.TypeId === req.TypeId);

                        // Pass the whole req object to lock the row
                        addNewRow(req, matchingBudget ? matchingBudget.Ref : "", req.Amount);
                    });
                }
            } else {
                Swal.fire({
                    icon: 'warning', title: 'No Funds Available', text: 'There are no active budgets to cover this transfer.', confirmButtonColor: '#d33'
                });
            }
        });

        // --- Core Functions ---
        function addNewRow(reqObj, preSelectedRef, preFilledAmount) {
            var index = $("#allocationTable tbody tr").length;

            // Lock variables
            var typeId = reqObj ? reqObj.TypeId : null;
            var typeName = reqObj ? reqObj.TypeName : "Unknown Type";

            // IMPORTANT: Filter dropdown to ONLY show Budgets matching this row's TypeId
            var filteredBudgets = typeId ? budgetList.filter(b => b.TypeId === typeId) : budgetList;

            var optionsHtml = '<option value="">-- Select Budget Ref --</option>';
            filteredBudgets.forEach(function (b) {
                optionsHtml += `<option value="${b.Ref}">${b.Display}</option>`;
            });

            var html = `
            <tr data-index="${index}" data-typeid="${typeId}" data-typename="${typeName}">
                <td class="p-2 align-middle text-center">
                    <span class="badge badge-info p-2 w-100" style="font-size:0.85rem; white-space:normal;">${typeName}</span>
                </td>
                <td class="p-2">
                    <select class="form-control select2 ddl-budget" style="width:100%" onchange="onBudgetChange(this)">
                        ${optionsHtml}
                    </select>
                    <input type="hidden" name="allocations[${index}][BudgetRef]" class="hdn-ref" />
                    <input type="hidden" name="allocations[${index}][BudgetId]" class="hdn-id" />
                </td>
                <td class="p-2">
                    <input type="text" class="form-control txt-readonly txt-balance" value="0.00" readonly />
                </td>
                <td class="p-2">
                    <input type="text" name="allocations[${index}][Amount]" class="form-control txt-amount" 
                           placeholder="0.00" oninput="formatAndCalc(this)" />
                </td>
                <td class="text-center p-2 align-middle">
                    <button type="button" class="btn btn-sm btn-outline-primary mb-1" onclick="addSplitRow(this)" title="Split into another budget of same type">
                        <i class="fas fa-code-branch"></i>
                    </button>
                    <button type="button" class="btn btn-sm btn-outline-danger mb-1" onclick="removeRow(this)" title="Remove Line">
                        <i class="fas fa-trash-alt"></i>
                    </button>
                </td>
            </tr>`;

            $("#allocationTable tbody").append(html);

            var $newRow = $("#allocationTable tbody tr").last();
            var $select = $newRow.find('.ddl-budget');

            $select.select2({ placeholder: "Select Budget", allowClear: true, theme: 'bootstrap-5' });

            if (preSelectedRef) {
                $select.val(preSelectedRef).trigger('change.select2');
                var budget = filteredBudgets.find(b => b.Ref === preSelectedRef);
                if (budget) {
                    $newRow.find('.hdn-ref').val(budget.Ref);
                    $newRow.find('.hdn-id').val(budget.Id);
                    $newRow.find('.txt-balance').val(budget.Balance.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 }));
                }
            }

            if (preFilledAmount && preFilledAmount > 0) {
                $newRow.find('.txt-amount').val(preFilledAmount.toFixed(2));
            }

            recalculateTotal();
        }
        function addSplitRow(btn) {
            var $row = $(btn).closest('tr');
            var typeId = $row.data('typeid');
            var typeName = $row.data('typename');

            // Create a new row strictly bound to the exact same Type
            var reqObj = { TypeId: typeId, TypeName: typeName };
            addNewRow(reqObj, "", 0);
        }
        function removeRow(btn) {
            var $row = $(btn).closest('tr');
            var typeId = $row.data('typeid');

            // Check how many rows exist for this specific type
            var typeCount = $("#allocationTable tbody tr").filter(function () {
                return $(this).data('typeid') === typeId;
            }).length;

            if (typeCount > 1) {
                $row.remove();
                recalculateTotal();
            } else {
                // Prevent user from accidentally deleting the ONLY row representing a requested type
                Swal.fire({ icon: 'warning', title: 'Action Denied', text: 'You must allocate at least one budget for this requested type.' });
            }
        }

        function onBudgetChange(select) {
            var $select = $(select);
            var $row = $select.closest('tr');
            var currentVal = $select.val();

            if (!currentVal) {
                clearRowData($row);
                recalculateTotal();
                return;
            }

            var isDuplicate = false;
            $(".ddl-budget").not($select).each(function () {
                if ($(this).val() === currentVal) {
                    isDuplicate = true;
                    return false; 
                }
            });

            if (isDuplicate) {
                Swal.fire({
                    icon: 'warning',
                    title: 'Duplicate Selection',
                    text: 'This Budget Reference is already selected in another row.',
                    confirmButtonColor: '#f39c12'
                });
                $select.val(null).trigger('change.select2');
                return;
            }

            var budget = budgetList.find(b => b.Ref === currentVal);
            if (budget) {
                $row.find('.hdn-ref').val(budget.Ref);
                $row.find('.hdn-id').val(budget.Id);
                $row.find('.txt-balance').val(budget.Balance.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 }));
            } else {
                clearRowData($row);
            }

            $row.find('.txt-amount').val(''); // Clear amount only if user manually changes the dropdown
            recalculateTotal();
        }

        function clearRowData($row) {
            $row.find('.hdn-ref').val('');
            $row.find('.hdn-id').val('');
            $row.find('.txt-balance').val('0.00');
        }

        function formatAndCalc(input) {
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

            var $txtTotal = $("#txtTotalAllocated");
            $txtTotal.val(total.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 }));

            if (Math.abs(total - targetAmount) > 0.01) {
                $txtTotal.removeClass('text-success').addClass('is-invalid text-danger');
                $("#rowError").show();
                isValid = false;
            } else {
                $txtTotal.removeClass('is-invalid text-danger').addClass('text-success');
                $("#rowError").hide();
            }

            return isValid;
        }

        function initiateSubmission() {
            if (!recalculateTotal()) {
                Swal.fire({
                    icon: 'error',
                    title: 'Allocation Error',
                    text: 'Please check your amounts. Total allocated must exactly match the requested amount, and no line can exceed its balance.'
                });
                return;
            }

            var empty = false;
            $(".ddl-budget").each(function () {
                if (!$(this).val()) empty = true;
            });

            if (empty) {
                Swal.fire({ icon: 'error', title: 'Missing Data', text: 'Please select a source budget for all rows.' });
                return;
            }

            Swal.fire({
                title: 'Finalize Transfer',
                text: "Are you sure you want to finalize this transfer and execute the budget deductions?",
                icon: 'warning',
                input: 'textarea',
                inputPlaceholder: 'Add optional approval remarks...',
                showCancelButton: true,
                confirmButtonColor: '#28a745',
                cancelButtonColor: '#6c757d',
                confirmButtonText: 'Yes, Finalize Transfer'
            }).then((result) => {
                if (result.isConfirmed) {
                    document.getElementById('<%= hdnRemarks.ClientID %>').value = result.value || "Approved and Finalized";
                    $("#pagePreloader").fadeIn(200);
                    document.getElementById('<%= btnServerSubmit.ClientID %>').click();
                }
            });
        }

        function initiateRejection() {
            Swal.fire({
                title: 'Reject Application',
                text: "Please provide a reason for rejecting this budget transfer:",
                icon: 'error',
                input: 'textarea',
                inputPlaceholder: 'Enter rejection reason (Mandatory)...',
                showCancelButton: true,
                confirmButtonColor: '#d33', 
                cancelButtonColor: '#6c757d',
                confirmButtonText: 'Reject Application',
                inputValidator: (value) => {
                    if (!value) { return 'Rejection reason is mandatory!' }
                }
            }).then((result) => {
                if (result.isConfirmed) {
                    document.getElementById('<%= hdnRemarks.ClientID %>').value = result.value;
                    $("#pagePreloader").fadeIn(200);
                    document.getElementById('<%= btnReject.ClientID %>').click();
                }
            });
        }
    </script>
</asp:Content>