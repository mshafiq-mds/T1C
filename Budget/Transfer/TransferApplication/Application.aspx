<%@ Page Title="Transfer Application Form" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Application.aspx.cs" Inherits="Prodata.WebForm.Budget.Transfer.TransferApplication.Application" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Panel runat="server" CssClass="card shadow rounded p-4">

        <!-- Header Bar -->
        <div class="card-header card-header-sticky">
            <h5 class="card-title d-none d-sm-inline"><%: Page.Title %></h5>
            <div class="card-tools">
                <asp:LinkButton ID="btnBack" runat="server" CssClass="btn btn-outline-secondary me-2"
                    PostBackUrl="/Budget/Transfer/TransferApplication/Default" CausesValidation="false">
                    <i class="fas fa-angle-double-left"></i> Back
                </asp:LinkButton>
         <%--       <asp:LinkButton ID="btnSave" runat="server" CssClass="btn btn-warning me-2">
                    <i class="fas fa-edit"></i> Request Revision
                </asp:LinkButton>--%>
                <asp:LinkButton ID="btnSubmit1" runat="server" CssClass="btn btn-success"
                    OnClick="btnSubmit_Click"
                    OnClientClick="return validateAllocations() && validateBeforeSubmit();">
                    <i class="fas fa-circle"></i> Approve Transfer
                </asp:LinkButton>
            </div>
        </div>

        <!-- Info Table -->
        <div class="table-responsive my-4">
            <table class="table table-borderless">
                <tbody>
                    <tr><td><strong>Status:</strong></td><td><asp:Label ID="lblStatus" runat="server" /></td></tr>
                    <tr><td><strong>Date:</strong></td><td><asp:Label ID="lblDate" runat="server" /></td></tr>
                    <tr><td><strong>Ref No:</strong></td><td><asp:Label ID="lblRef" runat="server" /></td></tr>
                    <tr><td><strong>Applicant:</strong></td><td><asp:Label ID="lblApplicantName" runat="server" /></td></tr>
                    <tr><td><strong>Mill:</strong></td><td>(<asp:Label ID="lblBA" runat="server" />) <asp:Label ID="LblBAName" runat="server" /></td></tr>
                    <tr><td><strong>Amount Request (RM):</strong></td><td><asp:Label ID="lblAmount" runat="server" /></td></tr>
                    <tr><td><strong>Reason:</strong></td><td><asp:Label ID="lblReason" runat="server" /></td></tr>
                    <tr><td><strong>Transfer:</strong></td><td><asp:Label ID="lblTransferFrom" runat="server" /><strong> To </strong><asp:Label ID="lblTransferTo" runat="server" /></td></tr>
                </tbody>
            </table>
        </div>

        <!-- Allocation Table -->
        <h5 class="mt-4"><strong>Budget Transfer Allocation</strong></h5>
        <div class="table-responsive">
            <table class="table table-bordered table-sm align-middle" id="allocationTable">
                <thead class="table-light text-center">
                    <tr>
                        <th style="width: 40%;">Budget Ref No</th>
                        <th style="width: 25%;">Balance (RM)</th>
                        <th style="width: 25%;">Amount (RM)</th>
                        <th style="width: 10%;">Action</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td>
                            <select class="form-control ddl-budget select2" required onchange="updateBalance(this)">
                                <option value="">Select Ref</option>
                            </select>
                            <input type="hidden" class="hidden-budget-ref" name="allocationTable[0][budgetRef]" value="" />
                            <input type="hidden" class="hidden-budget-id" name="allocationTable[0][budgetId]" value="" />
                            <small class="text-danger d-none ddl-budget-error">Required Ref</small>
                        </td>
                        <td>
                            <input type="text" class="form-control balance txtreadonly" value="RM 0.00" />
                        </td>
                        <td>
                            <input type="text" class="form-control amount" placeholder="0.00" required
                                oninput="validateDecimalInput(this); syncHiddenInputs();"
                                onblur="formatDecimal(this); syncHiddenInputs();"
                                onchange="calculateTotal(); syncHiddenInputs();" />
                            <input type="hidden" class="hidden-amount" name="allocationTable[0][amount]" value="" />
                            <small class="text-danger d-none ddl-budget-error">Required Amount</small>
                        </td>
                        <td class="text-center">
                            <button type="button" class="btn btn-sm btn-success" onclick="addRow(this)">
                                <i class="fas fa-plus"></i>
                            </button>
                        </td>
                    </tr>
                </tbody>

                <tfoot>
                    <tr>
                        <td colspan="2" class="text-end fw-bold">Total Amount (RM):</td>
                        <td>
                            <asp:TextBox ID="totalAmount" runat="server" CssClass="form-control bg-light txtreadonly"  Text="RM 0.00" />
    
                            <small id="amountMismatchNote" runat="server" visible="false" class="text-danger">
                                Total amount must match the requested amount.
                            </small>

                            <%--<asp:HiddenField ID="hdnServerAmount" runat="server" />--%>
                        </td>
                        <td></td>
                    </tr>
                </tfoot>
            </table>
        </div>

        <!-- Remarks Section -->
        <div class="form-group mt-4">
            <label for="txtRemarks" class="form-label"><strong>Remarks <span class="text-danger">*</span></strong></label>
            <asp:TextBox ID="txtRemarks" runat="server" TextMode="MultiLine" CssClass="form-control" Rows="5"  />
            <asp:RequiredFieldValidator ID="rfvtxtRemarks" runat="server" ControlToValidate="txtRemarks" CssClass="text-danger" ErrorMessage="Remarks is required." Display="Dynamic" />
        </div>

    </asp:Panel>

    <!-- JS Scripts -->
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script> <%--declare extantion for js sweetalert--%>

    <script type="text/javascript">
        function validateBeforeSubmit() {
            if (!calculateTotal()) {
                Swal.fire({
                    icon: 'warning',
                    title: 'Allocation Issue',
                    text: 'Some amounts exceed their balances or are invalid.',
                    confirmButtonText: 'OK'
                });
                return false;
            }

            const totalText = $("#<%= totalAmount.ClientID %>").val().replace("RM", "").replace(",", "").trim();
        const expectedText = $("#<%= lblAmount.ClientID %>").text().replace("RM", "").replace(",", "").trim();

        const total = parseFloat(totalText);
        const expected = parseFloat(expectedText);

        const formattedTotal = $("#<%= totalAmount.ClientID %>").val().trim();
        const formattedExpected = $("#<%= lblAmount.ClientID %>").text().trim();

        if (isNaN(total) || isNaN(expected) || Math.abs(total - expected) > 0.009 || formattedTotal !== formattedExpected) {
            Swal.fire({
                icon: 'warning',
                title: 'Allocation Issue',
                text: 'Total allocation amount does not match the requested amount.',
                confirmButtonText: 'OK'
            });
            return false;
        }

        return true;
    }

    function validateAllocations() {
        let isValid = true;

        $(".ddl-budget").each(function () {
            const $row = $(this).closest("tr");
            const $select = $row.find(".ddl-budget");
            const selected = $select.val();
            const $hiddenBudgetRef = $row.find(".hidden-budget-ref");
            const $budgetError = $row.find(".ddl-budget-error").first();

            if (!selected) {
                isValid = false;
                $budgetError.removeClass("d-none");
                $select.addClass("is-invalid");
            } else {
                $budgetError.addClass("d-none");
                $select.removeClass("is-invalid");
                $hiddenBudgetRef.val(selected);
            }

            const $amount = $row.find(".amount");
            const amountVal = $amount.val().trim();
            const $hiddenAmount = $row.find(".hidden-amount");
            const $amountError = $row.find(".ddl-budget-error").last();

            const parsedAmount = parseFloat(amountVal.replace(/,/g, ''));

            if (!amountVal || isNaN(parsedAmount) || parsedAmount <= 0) {
                isValid = false;
                $amountError.removeClass("d-none");
                $amount.addClass("is-invalid");
            } else {
                $amountError.addClass("d-none");
                $amount.removeClass("is-invalid");
                $hiddenAmount.val(parsedAmount.toFixed(2));
            }
        });

        return isValid;
    }

    function syncHiddenInputs() {
        $("#allocationTable tbody tr").each(function (index, row) {
            const $row = $(row);
            const budgetVal = $row.find(".ddl-budget").val();
            const amountVal = $row.find(".amount").val();
            const budgetidVal = $row.find(".hidden-budget-id").val();

            $row.find(".hidden-budget-ref").attr("name", `allocationTable[${index}][budgetRef]`).val(budgetVal);
            $row.find(".hidden-amount").attr("name", `allocationTable[${index}][amount]`).val(amountVal);
            $row.find(".hidden-budget-id").attr("name", `allocationTable[${index}][budgetId]`).val(budgetidVal);
        });
    }

    function populateBudgetDropdown($select) {
        const selectedRefs = $(".ddl-budget").map(function () {
            return $(this).val();
        }).get().filter(val => val !== "");

        const currentVal = $select.val();
        $select.empty().append('<option value="">Select Ref</option>');

        if (Array.isArray(budgetList)) {
            budgetList.forEach(item => {
                if (!selectedRefs.includes(item.Ref) || item.Ref === currentVal) {
                    $select.append(`<option value="${item.Ref}" data-balance="${item.Balance}">${item.Display}</option>`);
                }
            });
        }
    }

    function updateBalance(selectElement) {
        const $select = $(selectElement);
        const $row = $select.closest("tr");
        const selectedOption = $select.find(":selected");
        const balance = parseFloat(selectedOption.data("balance")) || 0;
        $row.find(".balance").val("RM " + balance.toFixed(2));
    }

    function addRow(button) {
        const $row = $(button).closest("tr");
        const selectedValue = $row.find(".ddl-budget").val();
        $row.find(".ddl-budget").select2("destroy");

        const $newRow = $row.clone();
        $row.find(".ddl-budget").val(selectedValue).select2();

        $newRow.find("input").val("");
        $newRow.find(".balance").val("RM 0.00");
        $newRow.find(".ddl-budget-error").addClass("d-none");

        $newRow.find("button")
            .removeClass("btn-success").addClass("btn-danger")
            .html('<i class="fas fa-minus"></i>')
            .attr("onclick", "removeRow(this)");

        $("#allocationTable tbody").append($newRow);

        const $ddl = $newRow.find(".ddl-budget");
        populateBudgetDropdown($ddl);
        $ddl.select2();

        syncHiddenInputs();
    }

    function removeRow(button) {
        $(button).closest("tr").remove();
        calculateTotal();
        syncHiddenInputs();
        refreshAllDropdowns();
    }

    function refreshAllDropdowns() {
        $(".ddl-budget").each(function () {
            const $ddl = $(this);
            const currentVal = $ddl.val();
            $ddl.select2("destroy");
            populateBudgetDropdown($ddl);
            $ddl.val(currentVal).select2();
        });
    }

    function validateDecimalInput(input) {
        let value = input.value.replace(/[^0-9.]/g, '');
        const parts = value.split('.');
        if (parts.length > 2) value = parts[0] + '.' + parts[1];
        if (parts.length === 2) value = parts[0] + '.' + parts[1].substring(0, 2);
        if (value.startsWith('.')) value = '0' + value;
        input.value = value;
    }

    function formatDecimal(input) {
        let val = input.value;
        if (val !== "") {
            const parsed = parseFloat(val);
            if (!isNaN(parsed)) {
                input.value = parsed.toFixed(2);
            }
        }
    }

    function calculateTotal() {
        let total = 0;
        let hasInvalidAmount = false;

        $(".amount").each(function () {
            const $amountInput = $(this);
            const val = parseFloat($amountInput.val());
            const $row = $amountInput.closest("tr");
            const balanceText = $row.find(".balance").val().replace("RM", "").replace(/,/g, "").trim();
            const balance = parseFloat(balanceText);

            if (!isNaN(val)) {
                total += val;
                if (!isNaN(balance) && val > balance) {
                    $amountInput.addClass("is-invalid");
                    $amountInput.attr("title", "Amount exceeds balance (RM " + balance.toFixed(2) + ").");
                    hasInvalidAmount = true;
                } else {
                    $amountInput.removeClass("is-invalid");
                    $amountInput.removeAttr("title");
                }
            }
        });

        const formattedTotal = "RM " + total.toFixed(2);
        $("#<%= totalAmount.ClientID %>").val(formattedTotal);

        const serverAmountText = $("#<%= lblAmount.ClientID %>").text().replace("RM", "").replace(/,/g, "").trim();
        const serverAmount = parseFloat(serverAmountText);
        const $totalAmount = $("#<%= totalAmount.ClientID %>");

            if (!isNaN(serverAmount) && Math.abs(total - serverAmount) > 0.009) {
                $totalAmount.addClass("is-invalid");
                $totalAmount.attr("title", "Total mismatch (Expected RM " + serverAmount.toFixed(2) + ").");
            } else {
                $totalAmount.removeClass("is-invalid");
                $totalAmount.removeAttr("title");
            }

            return !hasInvalidAmount;
        }

        $(document).ready(function () {
            $(".ddl-budget").each(function () {
                populateBudgetDropdown($(this));
                $(this).select2();
            });

            $(document).on("change", ".ddl-budget", function () {
                const $row = $(this).closest("tr");
                const selectedValue = $(this).val();
                const budgetObj = budgetList.find(x => x.Ref === selectedValue);
                if (budgetObj) {
                    $row.find(".hidden-budget-id").val(budgetObj.Id);
                }
                updateBalance(this);
                syncHiddenInputs();
                refreshAllDropdowns();
            });

            $(document).on("input change", ".amount", function () {
                validateDecimalInput(this);
                calculateTotal();
                syncHiddenInputs();
            });

            $(document).on("blur", ".amount", function () {
                formatDecimal(this);
                syncHiddenInputs();
            });
        });
    </script>
    
    <style>
        #totalAmount.is-invalid {
            border: 2px solid #dc3545;
            background-color: #fff5f5;
        }
    </style>

</asp:Content>

    <%--<script type="text/javascript">
        function validateBeforeSubmit() {
            // First, recalculate total and check for invalid entries
            if (!calculateTotal()) {
                Swal.fire({
                    icon: 'warning',
                    title: 'Allocation Issue',
                    text: 'Some amounts exceed their balances or are invalid.',
                    confirmButtonText: 'OK'
                });
                return false; // Prevent postback
            }

            // Then, compare total with expected amount
            const totalText = $("#<%= totalAmount.ClientID %>").val().replace("RM", "").replace(",", "").trim();
            const expectedText = $("#<%= lblAmount.ClientID %>").text().replace("RM", "").replace(",", "").trim();

            const total = parseFloat(totalText);
            const expected = parseFloat(expectedText);

            const formattedTotal = $("#<%= totalAmount.ClientID %>").val().trim();
            const formattedExpected = $("#<%= lblAmount.ClientID %>").text().trim();

            if (isNaN(total) || isNaN(expected) || Math.abs(total - expected) > 0.009 || formattedTotal !== formattedExpected) {
                Swal.fire({
                    icon: 'warning',
                    title: 'Allocation Issue',
                    text: 'Total allocation amount does not match the requested amount.',
                    confirmButtonText: 'OK'
                });
                return false;
            }

            return true; // All good — allow postback
        }



        function validateAllocations() {
            let isValid = true;

            $(".ddl-budget").each(function () {
                const $row = $(this).closest("tr");

                // --- Validate Budget Ref ---
                const $select = $row.find(".ddl-budget");
                const selected = $select.val();
                const $hiddenBudgetRef = $row.find(".hidden-budget-ref");
                const $budgetError = $row.find(".ddl-budget-error").first();

                if (!selected || selected === "") {
                    isValid = false;
                    $budgetError.removeClass("d-none");
                    $select.addClass("is-invalid");
                } else {
                    $budgetError.addClass("d-none");
                    $select.removeClass("is-invalid");
                    $hiddenBudgetRef.val(selected);
                }

                // --- Validate Amount ---
                const $amount = $row.find(".amount");
                const amountVal = $amount.val().trim();
                const $hiddenAmount = $row.find(".hidden-amount");
                const $amountError = $row.find(".ddl-budget-error").last(); // assumes order: ref error, amount error

                const parsedAmount = parseFloat(amountVal.replace(/,/g, ''));

                if (!amountVal || isNaN(parsedAmount) || parsedAmount <= 0) {
                    isValid = false;
                    $amountError.removeClass("d-none");
                    $amount.addClass("is-invalid");
                } else {
                    $amountError.addClass("d-none");
                    $amount.removeClass("is-invalid");
                    $hiddenAmount.val(parsedAmount.toFixed(2)); // sync for server
                }
            });

            return isValid;
        }


        function syncHiddenInputs() {
            $("#allocationTable tbody tr").each(function (index, row) {
                const $row = $(row);
                const budgetVal = $row.find(".ddl-budget").val();
                const amountVal = $row.find(".amount").val();
                const budgetidVal = $row.find(".hidden-budget-id").val(); 


                $row.find(".hidden-budget-ref")
                    .attr("name", `allocationTable[${index}][budgetRef]`)
                    .val(budgetVal);

                $row.find(".hidden-amount")
                    .attr("name", `allocationTable[${index}][amount]`)
                    .val(amountVal);

                $row.find(".hidden-budget-id")  
                    .attr("name", `allocationTable[${index}][budgetId]`)
                    .val(budgetidVal);

            });
        }

        $(".ddl-budget, .amount").on("change input", function () {
            syncHiddenInputs();
        });

        function populateBudgetDropdown($select) {
            $select.empty().append('<option value="">Select Ref</option>');
            if (Array.isArray(budgetList)) {
                budgetList.forEach(item => {
                    $select.append(`<option value="${item.Ref}" data-balance="${item.Balance}">${item.Display}</option>`);
                });
            }
            $(document).on("change", ".ddl-budget", function () {
                const $row = $(this).closest("tr");
                const selectedValue = $(this).val();
                const budgetObj = budgetList.find(x => x.Ref === selectedValue);
                if (budgetObj) {
                    $row.find(".hidden-budget-id").val(budgetObj.Id); // ✅ set budgetId
                }
                updateBalance(this);
                syncHiddenInputs();
            });

        }

        function updateBalance(selectElement) {
            const $select = $(selectElement);
            const $row = $select.closest("tr");
            const selectedOption = $select.find(":selected");
            const balance = parseFloat(selectedOption.data("balance")) || 0;
            const balanceFormatted = "RM " + balance.toFixed(2);
            $row.find(".balance").val(balanceFormatted);
        }

        $(document).ready(function () {
            $(".ddl-budget").each(function () {
                populateBudgetDropdown($(this));
                $(this).select2();
            });

            $(document).on("change", ".ddl-budget", function () {
                updateBalance(this);
                syncHiddenInputs();
            });
        });

        function addRow(button) {
            var $row = $(button).closest("tr");

            // Save the current selected value BEFORE destroying select2
            var selectedValue = $row.find(".ddl-budget").val();

            // Destroy select2 before clone to avoid clone issues
            $row.find(".ddl-budget").select2("destroy");

            // Clone the row
            var $newRow = $row.clone();

            // Restore select2 to original row (without repopulating)
            $row.find(".ddl-budget").val(selectedValue).select2();

            // Reset new row input values
            $newRow.find("input").val("");
            $newRow.find(".balance").val("RM 0.00");
            $newRow.find(".ddl-budget-error").addClass("d-none");

            // Change button to remove
            $newRow.find("button")
                .removeClass("btn-success").addClass("btn-danger")
                .html('<i class="fas fa-minus"></i>')
                .attr("onclick", "removeRow(this)");

            // Append new row
            $("#allocationTable tbody").append($newRow);

            // Reinitialize select2 and populate dropdown only for new row
            const $ddl = $newRow.find(".ddl-budget");
            populateBudgetDropdown($ddl);
            $ddl.select2();

            // Sync names
            syncHiddenInputs();
        }


        function removeRow(button) {
            $(button).closest("tr").remove();
            calculateTotal();
            syncHiddenInputs();
        }

        function validateDecimalInput(input) {
            let value = input.value;
            value = value.replace(/[^0-9.]/g, '');
            const parts = value.split('.');
            if (parts.length > 2) {
                value = parts[0] + '.' + parts[1];
            }
            if (parts.length === 2) {
                parts[1] = parts[1].substring(0, 2);
                value = parts[0] + '.' + parts[1];
            }
            if (value.startsWith('.')) {
                value = '0' + value;
            }
            input.value = value;
        }

        function formatDecimal(input) {
            let val = input.value;
            if (val !== "") {
                let parsed = parseFloat(val);
                if (!isNaN(parsed)) {
                    input.value = parsed.toFixed(2);
                }
            }
        }

        function calculateTotal() {
            let total = 0;
            let hasInvalidAmount = false;

            $(".amount").each(function () {
                const $amountInput = $(this);
                const val = parseFloat($amountInput.val());
                const $row = $amountInput.closest("tr");
                const $balanceInput = $row.find(".balance");

                // Extract balance (e.g., "RM 1,234.56" → 1234.56)
                const balanceText = $balanceInput.val().replace("RM", "").replace(/,/g, "").trim();
                const balance = parseFloat(balanceText);

                if (!isNaN(val)) {
                    total += val;

                    if (!isNaN(balance) && val > balance) {
                        $amountInput.addClass("is-invalid");
                        $amountInput.attr("title", "Amount exceeds available balance (RM " + balance.toFixed(2) + ").");
                        hasInvalidAmount = true;
                    } else {
                        $amountInput.removeClass("is-invalid");
                        $amountInput.removeAttr("title");
                    }
                }
            });

            // Update total amount field
            const formattedTotal = "RM " + total.toFixed(2);
            $("#<%= totalAmount.ClientID %>").val(formattedTotal);

            // Compare with requested amount
            const serverAmountText = $("#<%= lblAmount.ClientID %>").text().replace("RM", "").replace(/,/g, "").trim();
            const serverAmount = parseFloat(serverAmountText);
            const $totalAmount = $("#<%= totalAmount.ClientID %>");

            if (!isNaN(serverAmount) && Math.abs(total - serverAmount) > 0.009) {
                $totalAmount.addClass("is-invalid");
                $totalAmount.attr("title", "Total amount does not match the requested amount (RM " + serverAmount.toFixed(2) + ").");
            } else {
                $totalAmount.removeClass("is-invalid");
                $totalAmount.removeAttr("title");
            }

            return !hasInvalidAmount; // Optional: return whether all values are valid
        }

    </script>--%>
