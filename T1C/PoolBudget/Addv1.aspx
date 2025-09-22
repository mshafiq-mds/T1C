<%@ Page Title="T1C Others Budget Add" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Addv1.aspx.cs" Inherits="Prodata.WebForm.T1C.PoolBudget.Addv1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="hdnGuidBudgetType" runat="server" />
    <script>
        function validateBudgetBekalan() {
            function parseNumber(id, isInput = false) {
                var el = document.getElementById(id);
                if (!el) return 0;
                var raw = isInput ? el.value : el.innerText;
                if (!raw) return 0;
                raw = raw.replace(/,/g, '').trim(); // remove commas & spaces
                return parseFloat(raw) || 0;
            }

            var balance = parseNumber("<%= LblBalance.ClientID %>");
            var amount = parseNumber("<%= txtAmount.ClientID %>", true);

            if (balance < 0) {
                Swal.fire({
                    icon: 'error',
                    title: 'Insufficient Balance',
                    text: 'The requested amount exceeds the available budget.'
                });

                return false;
            }
            return true;
        }

        function updateBalanceBelian() {
            var balanceEl = document.getElementById("<%= LblBalance.ClientID %>");
            var amountEl = document.getElementById("<%= txtAmount.ClientID %>");

            if (!balanceEl || !amountEl) return;

            // Get original balance (store it in data-original if not set yet)
            if (!balanceEl.dataset.original) {
                balanceEl.dataset.original = balanceEl.innerText.replace(/,/g, '').trim();
            }

            var originalBalance = parseFloat(balanceEl.dataset.original) || 0;
            var amount = parseFloat(amountEl.value.replace(/,/g, '').trim()) || 0;

            var newBalance = originalBalance - amount;

            // Format with commas + 2 decimals
            balanceEl.innerText = newBalance.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 });
        }

        // Attach event
        document.addEventListener("DOMContentLoaded", function () {
            var amountEl = document.getElementById("<%= txtAmount.ClientID %>");
            if (amountEl) {
                amountEl.addEventListener("input", updateBalanceBelian);
            }
        });

        function validateAndSubmitAll() {
            // 1. Run ASP.NET validation
            if (typeof (Page_ClientValidate) === 'function') {
                Page_ClientValidate('FormGroup');
            }

            if (!Page_IsValid) {
                return false; // Stop if ASP.NET validation failed
            }

            // 2. Run custom budget validation
            if (!validateBudgetBekalan()) {
                return false; // Stop if balance check failed
            }

            // 3. Everything passed → show preloader
            $("#pagePreloader").fadeIn(200);
            return true; // Allow postback
        }


    </script>
    <script type="text/javascript">
        $(function () {
            // Attach to dropdown change
            $("#<%= ddlBT.ClientID %>").change(function () {
            // Show preloader
            $("#pagePreloader").fadeIn(200);

            // Trigger ASP.NET postback manually
            __doPostBack('<%= ddlBT.UniqueID %>', '');
        });
    });
    </script>


    <style>
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

        .page-preloader img {
            animation: shake 1.5s infinite;
        }

        @keyframes shake {
            0% { transform: rotate(0deg); }
            25% { transform: rotate(3deg); }
            50% { transform: rotate(0deg); }
            75% { transform: rotate(-3deg); }
            100% { transform: rotate(0deg); }
        }
    </style>

        <!-- Page-specific Preloader -->
    <div id="pagePreloader" class="page-preloader" style="display:none;">
        <img src="<%= ResolveUrl("~/Images/Felda_Global_Ventures_Logo.png") %>" 
             alt="Loading..." height="200" width="200" />
        <p class="mt-3 text-white">Processing...</p>
    </div> 

        <div class="card-header">
            <div class="row align-items-center">
                <!-- Budget Type inline -->
                <div class="col-md-6 mb-2 mb-md-0">
                    <div class="d-flex align-items-center">
                        <label for="ddlBT" class="form-label fw-bold me-2 mb-0">Budget Type:</label>&nbsp;&nbsp;
                        <asp:DropDownList 
                            runat="server" 
                            ID="ddlBT" 
                            CssClass="form-control select2"
                            DataValueField="Code" 
                            DataTextField="DisplayName"
                            data-placeholder="Select Type"
                            AutoPostBack="true"
                            CausesValidation="false"
                            OnSelectedIndexChanged="BudgetType_SelectedIndexChanged"  >
                        </asp:DropDownList>

                    </div>
                </div>

                <!-- Reference Number + Buttons -->
                <div class="col-md-6 d-flex justify-content-between align-items-end">
                    <div>
                        <label class="form-label fw-bold mb-0">Reference Number:</label>&nbsp;&nbsp;
                        <asp:Label runat="server" ID="Label1" CssClass="fw-semibold ms-2" ClientIDMode="Static" />
                    </div>
                    <div class="d-flex gap-2"> 
                        <asp:LinkButton 
                            ID="btnBack" 
                            runat="server" 
                            CssClass="btn btn-outline-secondary" 
                            PostBackUrl="~/T1C/PoolBudget/Default" 
                            CausesValidation="false">
                            <i class="fas fa-angle-double-left"></i> Back
                        </asp:LinkButton>
                    </div>
                </div>
            </div>
        </div>




    
    <%--// ======================================================================================  Style="display:none;"
        // ✅ For new requests 
        // ======================================================================================--%>
    <asp:Panel runat="server" CssClass="card p-4" ID="PnlForm" Style="display:none;">
    
        <!-- Card Header --> 
        <div class="card-header card-header-sticky">
            <asp:Label ID="CardTitle" runat="server" CssClass="card-title d-none d-sm-inline" Text="Title"></asp:Label>
            <div class="card-tools"> 
                <asp:LinkButton ID="LinkButton6" runat="server" CssClass="btn btn-success" OnClick="btnPnlForm_Click" ValidationGroup="FormGroup" OnClientClick="return validateAndSubmitAll();">
                    <i class="fas fa-share"></i> Submit Application
                </asp:LinkButton>
            </div>
        </div>

        <!-- Card Body -->
        <div class="card-body">

            <!-- BA Info (full width) -->
            <div class="row">
                
                <div class="col-md-6 mb-3">
                    <label class="fw-semibold text-dark">Business Area:</label>
                    <div class="d-flex align-items-center fs-5 fw-bold text-primary">
                        <asp:PlaceHolder runat="server" ID="phBA">
                            <asp:Label runat="server" ID="LblBA" CssClass="me-1" ClientIDMode="Static" />
                            <span>(</span>
                            <asp:Label runat="server" ID="LblBAName" ClientIDMode="Static" />
                            <span>)</span>
                        </asp:PlaceHolder>

                        <div class="text-danger ms-2 fw-semibold">
                            <asp:PlaceHolder runat="server" ID="phBAPh">
                                This ID is not allowed to create a budget transfer.
                            </asp:PlaceHolder>
                        </div>
                    </div>
                </div>
                
                <!-- Balance Budget -->
                 <div class="col-md-6 mb-3">
                    <label class="fw-bold mb-0">Budget Balance :</label>
                    &nbsp;&nbsp;
                    RM&nbsp;<asp:Label runat="server" ID="LblBalance" CssClass="fw-semibold" ClientIDMode="Static" Text="0.00"/>
                     &nbsp;&nbsp;/&nbsp;&nbsp;
                    RM&nbsp;<asp:Label runat="server" ID="LblBuget" CssClass="fw-semibold" ClientIDMode="Static" Text="0.00"/>
                 </div>
            </div>

            <!-- Two-column layout -->
            <div class="row">
                <!-- Purchase Type -->
                <div class="col-md-6 mb-3">
                    <label for="ddlPT" class="fw-semibold">Select Purchase Type</label>
                    <asp:DropDownList runat="server" ID="ddlPT" 
                        CssClass="form-select select2 w-100" 
                        data-placeholder="Choose Purchase Type"
                        DataValueField="Code" DataTextField="Name" />
                    <asp:RequiredFieldValidator ID="rfvddlPT" runat="server" ControlToValidate="ddlPT" ValidationGroup="FormGroup"
                        CssClass="text-danger small" ErrorMessage="Purchase Type is required." Display="Dynamic" />
                </div>

                <!-- Justification -->
                <div class="col-md-6 mb-3">
                    <label for="txtJustification" class="fw-semibold">Justification</label>
                    <asp:TextBox runat="server" ID="txtJustification" 
                        CssClass="form-control" TextMode="MultiLine" Rows="3" 
                        placeholder="Example: Human Resources - urgent requirement" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtJustification" ValidationGroup="FormGroup"
                        ErrorMessage="Justification is required." CssClass="text-danger small" Display="Dynamic" />
                </div>
            </div>

            <div class="row">
                <!-- Amount -->
                <div class="col-md-6 mb-3">
                    <label for="txtAmount" class="fw-semibold">Estimated Amount</label>
                    <div class="input-group">
                        <span class="input-group-text">RM</span>
                        <asp:TextBox ID="txtAmount" runat="server" 
                            CssClass="form-control text-end input-number2" 
                            placeholder="0.00"></asp:TextBox>
                    </div>
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtAmount" ValidationGroup="FormGroup"
                        ErrorMessage="Amount is required." CssClass="text-danger small" Display="Dynamic" />
                </div>

                <!-- File Upload -->
                <div class="col-md-6 mb-3">
                    <label for="fuDocument" class="fw-semibold">Upload Supporting Document</label>
                    <asp:FileUpload ID="fuDocument" runat="server" CssClass="form-control" />
                    <asp:RequiredFieldValidator ID="rfvFile" runat="server" ControlToValidate="fuDocument" ValidationGroup="FormGroup"
                        ErrorMessage="Please upload a document." CssClass="text-danger small" Display="Dynamic" />
                    <asp:Label ID="lblUploadResult" runat="server" CssClass="text-success small mt-1 d-block" />
                </div>
            </div>

        </div>

    </asp:Panel>
     
</asp:Content>
