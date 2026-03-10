<%@ Page Title="New Budget Transfer" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Addv2.aspx.cs" Inherits="Prodata.WebForm.Budget.Transfer.Addv2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>

    <script type="text/javascript">  
        function parseFloatSafe(val) {
            if (!val) return 0;
            const num = parseFloat(val.toString().replace(/[^0-9.-]/g, ''));
            return isNaN(num) ? 0 : num;
        }

        function formatCurrencyInput(input, formatOnBlur = false) {
            let value = input.value.replace(/[^0-9.]/g, '');
            const parts = value.split('.');
            if (parts.length > 2) value = parts[0] + '.' + parts[1];
            if (parts[1]) value = parts[0] + '.' + parts[1].substring(0, 2);
            if (formatOnBlur && value) {
                const number = parseFloat(value);
                if (!isNaN(number)) value = number.toFixed(2);
            }
            input.value = value;
        }

        function updateCalculatedFields() {
            let totalTransfer = 0;
            let isValid = true;

            const transferInputs = document.querySelectorAll('.from-transfer');
            const balanceInputs = document.querySelectorAll('.from-balance');
            const afterInputs = document.querySelectorAll('.from-after');
            const validationMsgs = document.querySelectorAll('.msg-from-after');

            for (let i = 0; i < transferInputs.length; i++) {
                const fromBalance = parseFloatSafe(balanceInputs[i].value);
                const fromTransfer = parseFloatSafe(transferInputs[i].value);
                const fromAfter = fromBalance - fromTransfer;

                afterInputs[i].value = fromAfter.toFixed(2);
                totalTransfer += fromTransfer;

                if (fromAfter < 0) {
                    validationMsgs[i].style.display = "block";
                    isValid = false;
                } else {
                    validationMsgs[i].style.display = "none";
                }
            }

            const txtToBalance = document.getElementById("txtToBalance");
            if (txtToBalance) {
                const toBalance = parseFloatSafe(txtToBalance.value);
                const toAfter = toBalance + totalTransfer;

                document.getElementById("txtToTransfer").value = totalTransfer.toFixed(2);
                document.getElementById("txtToAfter").value = toAfter.toFixed(2);

                const txtEstimated = document.getElementById("<%= txtEstimatedCost.ClientID %>");
                if (txtEstimated) txtEstimated.value = totalTransfer.toFixed(2);

                const messageToAfter = document.getElementById("validationMessagetxtToAfter");
                if (toAfter < 0) {
                    messageToAfter.style.display = "block";
                    isValid = false;
                } else {
                    messageToAfter.style.display = "none";
                }
            }

            return isValid;
        }

        // --- SWEETALERT VALIDATION ---
        function showValidationError(message) {
            Swal.fire({ icon: 'warning', title: 'Validation Error', text: message });
            return false;
        }

        function beforeSubmit() {
            // 1. Math Validation
            if (!updateCalculatedFields()) {
                return showValidationError('Please resolve the negative balance calculations before submitting.');
            }

            // 2. Validate Top Section (Master Info)
            const refNo = document.getElementById('<%= txtRefNo.ClientID %>').value.trim();
            const eVisa = document.getElementById('<%= txtEVisa.ClientID %>').value.trim();
            const date = document.getElementById('<%= txtDate.ClientID %>').value.trim();
            const project = document.getElementById('<%= txtProject.ClientID %>').value.trim();
            const workDetails = document.getElementById('<%= txtWorkDetails.ClientID %>').value.trim();
            const globalFromBa = document.getElementById('<%= ddlGlobalFromBA.ClientID %>').value.trim();

            if (!refNo) return showValidationError('Reference No. is required.');
            if (!eVisa) return showValidationError('E-VISA No. is required.');
            if (!date) return showValidationError('Application Date is required.');
            if (!project) return showValidationError('Project Name is required.');
            if (!workDetails) return showValidationError('Work Details are required.');
            if (!globalFromBa) return showValidationError('Source Business Area (FROM) is required.');

            // 3. Validate Dynamic Table (FROM rows)
            const glCodes = document.querySelectorAll('.from-glcode');
            const budgetTypes = document.querySelectorAll('.from-budgettype');
            const transfers = document.querySelectorAll('.from-transfer');

            if (glCodes.length === 0) {
                return showValidationError('Please add at least one "FROM" budget transfer row.');
            }

            for (let i = 0; i < glCodes.length; i++) {
                const rowNum = i + 1;
                if (!glCodes[i].value.trim()) return showValidationError(`GL Code is required on "FROM" row ${rowNum}.`);
                if (!budgetTypes[i].value.trim()) return showValidationError(`Budget Type is required on "FROM" row ${rowNum}.`);

                const transferAmt = parseFloatSafe(transfers[i].value);
                if (transferAmt <= 0) return showValidationError(`Transfer amount must be greater than RM 0.00 on "FROM" row ${rowNum}.`);
            }

            // 4. Validate Destination Table (TO row)
            const toGLCode = document.getElementById('<%= txtToGLCode.ClientID %>').value.trim();
            const toBudgetType = document.getElementById('<%= txtToBudgetType.ClientID %>').value.trim();

            if (!toGLCode) return showValidationError('Destination GL Code (TO) is required.');
            if (!toBudgetType) return showValidationError('Destination Budget Type (TO) is required.');

            // 5. Validate Bottom Documents
            const justification = document.getElementById('<%= txtJustification.ClientID %>').value.trim();
            const documentUpload = document.getElementById('<%= fuDocument.ClientID %>').value;

            if (!justification) return showValidationError('Request Justification is required.');
            if (!documentUpload) return showValidationError('Supporting document upload is required.');

            // Passed All Validation - Show Loader and Submit!
            $("#pagePreloader").fadeIn(200);
            return true;
        }

        // Handle UpdatePanel partial postbacks gracefully
        if (typeof Sys !== 'undefined') {
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                if ($.fn.select2) {
                    $('.select2').select2({ width: '100%' });
                }
                updateCalculatedFields();
            });
        }
        // --- NEW: Automatically lock all inputs with 'txtreadonly' class ---
        function applyReadOnly() {
            const readOnlyInputs = document.querySelectorAll('.txtreadonly');
            readOnlyInputs.forEach(input => {
                input.setAttribute('readonly', 'readonly');
                // Optional: ensure tab key skips these fields
                input.setAttribute('tabindex', '-1');
            });
        }

        // 1. Run when the page first loads
        document.addEventListener('DOMContentLoaded', function () {
            applyReadOnly(); // Lock fields
            addNewRow();     // (Your existing code)
        });

        // 2. Run again after the UpdatePanel refreshes the table
        if (typeof Sys !== 'undefined') {
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                if ($.fn.select2) {
                    $('.select2').select2({ width: '100%' });
                }
                updateCalculatedFields(); // (Your existing code)
                applyReadOnly();          // Lock fields again after row added
            });
        }
    </script>

    <style>
        .full-screen-container { width: 100%; padding: 0 15px; }
        .card-custom { border: none; box-shadow: 0 4px 15px rgba(0,0,0,0.08); border-radius: 10px; margin-bottom: 30px; }
        .card-header-custom { background-color: #f8f9fa; border-bottom: 2px solid #e9ecef; border-radius: 10px 10px 0 0 !important; padding: 1.25rem 1.5rem; }
        .section-title { color: #2c3e50; font-weight: 600; border-bottom: 2px solid #3498db; padding-bottom: 8px; margin-bottom: 20px; margin-top: 30px; font-size: 1.25rem; }
        .form-label { font-weight: 600; color: #495057; }
        .table-custom th { background-color: #343a40; color: #ffffff; font-weight: 500; vertical-align: middle !important; text-align: center; }
        .table-custom td { vertical-align: middle; }
        .input-group-text { background-color: #e9ecef; font-weight: 600; }
        .min-w-150 { min-width: 150px; }
        .min-w-200 { min-width: 200px; }
        
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
            
            <div class="card-header card-header-sticky d-flex align-items-center position-sticky top-0" style="z-index: 1000; width: 100%;">
                <h3 class="card-title m-0 fw-bold text-dark">
                    <i class="fas fa-exchange-alt mr-2"></i><%: Page.Title %>
                </h3>
    
                <div class="card-tools" style="margin-left: auto !important;">
                    <asp:LinkButton ID="btnBack" runat="server" CssClass="btn btn-outline-secondary mr-2" PostBackUrl="~/Budget/Transfer/Default" CausesValidation="false">
                        <i class="fas fa-angle-double-left"></i> Back
                    </asp:LinkButton>
                    <asp:LinkButton ID="btnSubmit1" runat="server" CssClass="btn btn-success" OnClick="btnSubmit_Click1" OnClientClick="return beforeSubmit();">
                        <i class="fas fa-paper-plane"></i> Submit Application
                    </asp:LinkButton>
                </div>
            </div>

            <div class="card-body p-4">
                
                <div class="alert alert-info d-flex align-items-center shadow-sm mb-4">
                    <i class="fas fa-building fa-2x mr-3"></i>
                    <div>
                        <h5 class="m-0 font-weight-bold">Business Area (BA)</h5>
                        <asp:PlaceHolder runat="server" ID="phBA">
                            <span class="font-weight-normal" style="font-size: 1.1rem;">
                                <asp:Label runat="server" ID="LblBA" ClientIDMode="Static" /> - 
                                <asp:Label runat="server" ID="LblBAName" ClientIDMode="Static" />
                            </span>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder runat="server" ID="phBADropdown">
                            <span class="text-danger font-weight-bold">This ID is not allowed to create a Transfer Budget.</span>
                        </asp:PlaceHolder>
                    </div>
                </div>

                <h4 class="section-title">Application Information</h4>
                
                <div class="row">
                    <div class="col-md-3 mb-3">
                        <label class="form-label">Budget Type</label><br />
                        <div class="btn-group btn-group-toggle w-100" data-toggle="buttons">
                            <label class="btn btn-outline-primary active w-50">
                                <asp:RadioButton runat="server" ID="rdoOpex" GroupName="BudgetType" CssClass="d-none" Checked="true" /> OPEX
                            </label>
                            <label class="btn btn-outline-secondary disabled w-50" title="CAPEX not available">
                                <asp:RadioButton runat="server" ID="rdoCapex" GroupName="BudgetType" CssClass="d-none" Enabled="false"/> CAPEX
                            </label> 
                        </div>
                    </div>
                    
                    <div class="col-md-3 mb-3">
                        <label class="form-label">Reference No.</label>
                        <asp:TextBox runat="server" ID="txtRefNo" CssClass="form-control txtreadonly font-weight-bold text-primary" placeholder="Auto-generated" />
                    </div>

                    <div class="col-md-3 mb-3">
                        <label class="form-label">E-VISA No.</label>
                        <asp:TextBox runat="server" ID="txtEVisa" CssClass="form-control txtreadonly font-weight-bold text-info" placeholder="Auto-generated" />
                    </div>

                    <div class="col-md-3 mb-3">
                        <label class="form-label">Application Date</label>
                        <asp:TextBox runat="server" ID="txtDate" CssClass="form-control txtreadonly" TextMode="Date" />
                    </div>
                </div>

                <div class="row">
                    <div class="col-md-8 mb-3">
                        <label class="form-label">Project Name</label>
                        <asp:TextBox runat="server" ID="txtProject" CssClass="form-control" placeholder="Example: KS Tenggaroh Timur" />
                    </div>
                    
                    <div class="col-md-4 mb-3">
                        <label class="form-label">Estimated Cost</label>
                        <asp:UpdatePanel ID="upEstimatedCost" runat="server">
                            <ContentTemplate>
                                <div class="input-group">
                                    <div class="input-group-prepend"><span class="input-group-text">RM</span></div>
                                    <asp:TextBox runat="server" ID="txtEstimatedCost" CssClass="form-control txtreadonly font-weight-bold" placeholder="0.00" />
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>

                <div class="row">
                    <div class="col-md-12 mb-3">
                        <label class="form-label">Work Details</label>
                        <asp:TextBox runat="server" ID="txtWorkDetails" CssClass="form-control" TextMode="MultiLine" Rows="2" placeholder="Provide a brief summary of the work details..." />
                    </div>
                </div>

                <h4 class="section-title">Budget Flow Details</h4>

                <asp:UpdatePanel ID="upBudgetFlow" runat="server">
                    <ContentTemplate>
                        
                        <div class="row mb-3">
                            <div class="col-md-6">
                                <label class="form-label text-danger font-weight-bold"><i class="fas fa-sign-out-alt mr-1"></i> Source Business Area (FROM)</label>
                                <asp:DropDownList runat="server" ID="ddlGlobalFromBA" CssClass="form-control select2 shadow-sm" AutoPostBack="true" OnSelectedIndexChanged="GlobalFromBA_SelectedIndexChanged" data-placeholder="Select Source BA" />
                            </div>
                            <div class="col-md-6">
                                <label class="form-label text-success font-weight-bold"><i class="fas fa-sign-in-alt mr-1"></i> Destination Business Area (TO)</label>
                                <div class="form-control bg-light font-weight-bold text-dark shadow-sm" style="border-left: 4px solid #28a745; cursor: not-allowed;">
                                    <asp:Label runat="server" ID="lblGlobalToBA" ClientIDMode="Static" />
                                </div>
                            </div>
                        </div>

                        <div class="table-responsive shadow-sm border rounded mb-4">
                            <table class="table table-hover table-bordered table-custom mb-0">
                                <thead>
                                    <tr>
                                        <th style="width: 80px;">Direction</th> 
                                        <th class="min-w-150">GL Code</th> 
                                        <th class="min-w-200">Budget Type</th>
                                        <th class="min-w-150">Original (RM)</th>
                                        <th class="min-w-150">Current Bal (RM)</th>
                                        <th class="min-w-150">Transfer (RM)</th>
                                        <th class="min-w-150">Bal After (RM)</th>
                                        <th style="width: 50px;">Action</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:Repeater ID="rptFromBudgets" runat="server" OnItemDataBound="rptFromBudgets_ItemDataBound" OnItemCommand="rptFromBudgets_ItemCommand">
                                        <ItemTemplate>
                                            <tr class="bg-light">
                                                <td class="font-weight-bold text-danger text-center align-middle"><i class="fas fa-arrow-up mr-1"></i> FROM</td>
                                                <td class="align-middle">
                                                    <asp:TextBox ID="txtFromGLCode" runat="server" CssClass="form-control form-control-sm from-glcode" placeholder="GL Code" MaxLength="20" Text='<%# Eval("GL") %>' />
                                                </td>
                                                <td class="align-middle">
                                                    <asp:DropDownList runat="server" ID="ddlFromBudgetType" CssClass="form-control form-control-sm select2 w-100 from-budgettype" AutoPostBack="true" OnSelectedIndexChanged="From_SelectedIndexChanged" data-placeholder="Select Type" />
                                                </td>
                                                <td class="align-middle">
                                                    <asp:TextBox runat="server" ID="txtFromBudget" CssClass="form-control form-control-sm txtreadonly text-right" Text='<%# Eval("OriginalBudget", "{0:N2}") %>' />
                                                </td>
                                                <td class="align-middle">
                                                    <asp:TextBox ID="txtFromBalance" runat="server" CssClass="form-control form-control-sm txtreadonly text-right from-balance" Text='<%# Eval("CurrentBalance", "{0:N2}") %>' />
                                                </td>
                                                <td class="align-middle">
                                                    <asp:TextBox ID="txtFromTransfer" runat="server" CssClass="form-control form-control-sm text-right font-weight-bold text-danger from-transfer" placeholder="0.00" Text='<%# Eval("TransferAmount") %>' oninput="formatCurrencyInput(this); updateCalculatedFields();" onblur="formatCurrencyInput(this, true); updateCalculatedFields();" />
                                                </td>
                                                <td class="align-middle">
                                                    <asp:TextBox ID="txtFromAfter" runat="server" CssClass="form-control form-control-sm txtreadonly text-right from-after" Text='<%# Eval("BalanceAfter", "{0:N2}") %>'/>
                                                    <span class="text-danger small mt-1 msg-from-after" style="display:none; line-height:1;">Insufficient funds.</span>
                                                </td>
                                                <td class="text-center align-middle">
                                                    <asp:LinkButton ID="btnRemoveRow" runat="server" CommandName="Remove" CommandArgument='<%# Container.ItemIndex %>' CssClass="btn btn-outline-danger btn-sm" CausesValidation="false" title="Remove Row">
                                                        <i class="fas fa-times"></i>
                                                    </asp:LinkButton>
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                    
                                    <tr class="bg-white">
                                        <td colspan="8" class="text-center py-3 border-bottom-0 border-top-0">
                                            <asp:LinkButton ID="btnAddRow" runat="server" CssClass="btn btn-sm btn-primary rounded-pill px-4 shadow-sm" OnClick="btnAddRow_Click" CausesValidation="false">
                                                <i class="fas fa-plus mr-1"></i> Add Another 'From' Budget
                                            </asp:LinkButton>
                                        </td>
                                    </tr>

                                    <tr style="background-color: #e8f4f8; border-top: 3px solid #dee2e6;">
                                        <td class="font-weight-bold text-success text-center align-middle"><i class="fas fa-arrow-down mr-1"></i> TO</td>
                                        <td class="align-middle">
                                            <asp:TextBox ID="txtToGLCode" runat="server" CssClass="form-control form-control-sm" placeholder="GL Code" MaxLength="20" />
                                        </td>
                                        <td class="align-middle">
                                            <asp:DropDownList runat="server" ID="txtToBudgetType" CssClass="form-control form-control-sm select2 w-100" AutoPostBack="true" OnSelectedIndexChanged="To_SelectedIndexChanged" data-placeholder="Select Type" />
                                        </td>
                                        <td class="align-middle"> 
                                            <asp:TextBox runat="server" ID="txtToBudget" CssClass="form-control form-control-sm txtreadonly text-right" Text="0.00"/>
                                        </td>
                                        <td class="align-middle">
                                            <asp:TextBox ID="txtToBalance" runat="server" ClientIDMode="Static" CssClass="form-control form-control-sm txtreadonly text-right" text="0.00" />
                                        </td>
                                        <td class="align-middle">
                                            <asp:TextBox ID="txtToTransfer" runat="server" ClientIDMode="Static" CssClass="form-control form-control-sm txtreadonly text-right font-weight-bold text-success" Text="0.00"/>
                                        </td>
                                        <td class="align-middle">
                                            <asp:TextBox ID="txtToAfter" runat="server" ClientIDMode="Static" CssClass="form-control form-control-sm txtreadonly text-right font-weight-bold" Text="0.00"/> 
                                            <span id="validationMessagetxtToAfter" class="text-danger small mt-1" style="display:none; line-height:1;">Invalid calculation.</span>
                                        </td>
                                        <td></td> 
                                    </tr>
                                </tbody>
                            </table>
                        </div>

                    </ContentTemplate>
                </asp:UpdatePanel>

                <div class="row">
                    <div class="col-md-8 mb-3">
                        <h4 class="section-title mt-0">Justification</h4>
                        <asp:TextBox runat="server" ID="txtJustification" CssClass="form-control" TextMode="MultiLine" Rows="5" placeholder="Provide detailed justification for this budget transfer..." />
                    </div>

                    <div class="col-md-4 mb-3">
                        <h4 class="section-title mt-0">Supporting Document</h4>
                        <div class="custom-file mb-2">
                            <asp:FileUpload ID="fuDocument" runat="server" CssClass="custom-file-input" onchange="$(this).next('.custom-file-label').html($(this).val().split('\\').pop());" />
                            <label class="custom-file-label" for="fuDocument">Choose file...</label>
                        </div>
                        <small class="text-muted d-block mb-2">Upload approvals, cost breakdowns, or related emails.</small>
                        <asp:Label ID="lblUploadResult" runat="server" CssClass="text-success small font-weight-bold" />
                    </div>
                </div>

            </div>
        </asp:Panel>
    </div>

</asp:Content>