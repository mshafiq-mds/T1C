<%@ Page Title="T1C Pool Budget Add" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Add.aspx.cs" Inherits="Prodata.WebForm.T1C.PoolBudget.Add" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="hdnGuidBelian" runat="server" />
    <asp:HiddenField ID="hdnGuidBekalan" runat="server" />
    

    <asp:RadioButton runat="server" ID="RBbekalan" GroupName="BudgetType" CssClass="d-none"
        AutoPostBack="true" OnCheckedChanged="BudgetType_CheckedChanged" />

    <asp:RadioButton runat="server" ID="RBbelian" GroupName="BudgetType" CssClass="d-none"
        AutoPostBack="true" OnCheckedChanged="BudgetType_CheckedChanged" />

        <div class="card-header ">
             <div class="form-group">
                 
                <label class="d-block mb-2 font-weight-bold">BUDGET TYPE</label>
                <div class="btn-group btn-group-toggle" data-toggle="buttons">
                    <label class="btn btn-outline-primary">
                        <asp:RadioButton runat="server" ID="rdoPeralatan" GroupName="BudgetType" CssClass="d-none"  />
                        <span>Various Supplies & Equipment</span>
                    </label>
                    <label class="btn btn-outline-primary ml-2">
                        <asp:RadioButton runat="server" ID="rdoBelian" GroupName="BudgetType" CssClass="d-none" />
                        <span>Purchase of Spare Parts</span>
                    </label>
                </div>
            </div>  
             <!-- Reference No + Back button -->
            <div class="form-group d-flex justify-content-between align-items-center">
                <div>
                    <label class="fw-bold mb-0">REFERENCE NUMBER :</label>
                    &nbsp;&nbsp;
                    <asp:Label runat="server" ID="Label1" CssClass="fw-semibold" ClientIDMode="Static" />
                </div>
                <div>
                    <asp:LinkButton ID="LinkButton3" runat="server" CssClass="btn btn-default" PostBackUrl="~/T1C/PoolBudget/Default" CausesValidation="false">
                        <i class="fas fa-angle-double-left"></i> Back
                    </asp:LinkButton>
                </div>
            </div>


        </div>
    
    <%--// ======================================================================================
        // ✅ For new requests Bekalan
        // ======================================================================================--%>
    <asp:Panel runat="server" CssClass="card p-4" ID="PnlBekalan" Style="display:none;">
    
        <!-- Card Header --> 
        <div class="card-header card-header-sticky">
            <h2 class="card-title d-none d-sm-inline">Various Supplies & Equipment</h2>
            <div class="card-tools">
              <%--  <asp:LinkButton ID="LinkButton5" runat="server" CssClass="btn btn-default" PostBackUrl="~/T1C/PoolBudget/Approval/Default" CausesValidation="false">
                    <i class="fas fa-angle-double-left"></i> Back
                </asp:LinkButton>--%> 
                <asp:LinkButton ID="LinkButton6" runat="server" CssClass="btn btn-success" OnClick="btnPnlBekalan_Click" ValidationGroup="BekalanGroup" OnClientClick="return validateBudgetBekalan();">  
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
                    RM&nbsp;<asp:Label runat="server" ID="LblBalanceBekalan" CssClass="fw-semibold" ClientIDMode="Static" Text="0.00"/>
                     &nbsp;&nbsp;/&nbsp;&nbsp;
                    RM&nbsp;<asp:Label runat="server" ID="LblBugetBekalan" CssClass="fw-semibold" ClientIDMode="Static" Text="0.00"/>
                 </div>
            </div>

            <!-- Two-column layout -->
            <div class="row">
                <!-- Purchase Type -->
                <div class="col-md-6 mb-3">
                    <label for="ddlPT" class="fw-semibold">Select Purchase Type</label>
                    <asp:DropDownList runat="server" ID="ddlPT" 
                        CssClass="form-select select2" 
                        data-placeholder="Choose Purchase Type"
                        DataValueField="Code" DataTextField="Name" />
                    <asp:RequiredFieldValidator ID="rfvddlPT" runat="server" ControlToValidate="ddlPT" ValidationGroup="BekalanGroup"
                        CssClass="text-danger small" ErrorMessage="Purchase Type is required." Display="Dynamic" />
                </div>

                <!-- Justification -->
                <div class="col-md-6 mb-3">
                    <label for="txtJustification" class="fw-semibold">Justification</label>
                    <asp:TextBox runat="server" ID="txtJustification" 
                        CssClass="form-control" TextMode="MultiLine" Rows="3" 
                        placeholder="Example: Human Resources - urgent requirement" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtJustification" ValidationGroup="BekalanGroup"
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
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtAmount" ValidationGroup="BekalanGroup"
                        ErrorMessage="Amount is required." CssClass="text-danger small" Display="Dynamic" />
                </div>

                <!-- File Upload -->
                <div class="col-md-6 mb-3">
                    <label for="fuDocumentBekalan" class="fw-semibold">Upload Supporting Document</label>
                    <asp:FileUpload ID="fuDocumentBekalan" runat="server" CssClass="form-control" />
                    <asp:RequiredFieldValidator ID="rfvFile" runat="server" ControlToValidate="fuDocumentBekalan" ValidationGroup="BekalanGroup"
                        ErrorMessage="Please upload a document." CssClass="text-danger small" Display="Dynamic" />
                    <asp:Label ID="lblUploadResult" runat="server" CssClass="text-success small mt-1 d-block" />
                </div>
            </div>

        </div>

    </asp:Panel>

    
    <%--// ======================================================================================
        // ✅ For new requests Belian
        // ======================================================================================--%>
    <asp:Panel runat="server" CssClass="card p-4" ID="PnlBelian" style="display:none;">
        <div class="card-header card-header-sticky">
            <h2 class="card-title d-none d-sm-inline">Purchase of Spare Parts</h2>
            <div class="card-tools">
              <%--  <asp:LinkButton ID="LinkButton1" runat="server" CssClass="btn btn-default" PostBackUrl="~/T1C/PoolBudget/Approval/Default" CausesValidation="false">
                    <i class="fas fa-angle-double-left"></i> Back
                </asp:LinkButton> --%>
                <asp:LinkButton ID="LinkButton2" runat="server" CssClass="btn btn-success" OnClick="btnPnlBelian_Click" ValidationGroup="BelianGroup" OnClientClick="return validateBudgetBelian();">   
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
                     <asp:PlaceHolder runat="server" ID="phBABelian">
                         <asp:Label runat="server" ID="lblBABelian" CssClass="me-1" ClientIDMode="Static" />
                         <span>(</span>
                         <asp:Label runat="server" ID="lblBANameBelian" ClientIDMode="Static" />
                         <span>)</span>
                     </asp:PlaceHolder>
                     <div class="text-danger ms-2 fw-semibold">
                         <asp:PlaceHolder runat="server" ID="phBABelianph">
                             This ID is not allowed to create a budget transfer.
                         </asp:PlaceHolder>
                     </div>
                 </div>
                </div>
                 
                
                <!-- Balance Budget -->
                 <div class="col-md-6 mb-3">
                    <label class="fw-bold mb-0">Budget Balance :</label>
                    &nbsp;&nbsp;
                    RM&nbsp;<asp:Label runat="server" ID="LblBalanceBelian" CssClass="fw-semibold" ClientIDMode="Static" Text="0.00"/>
                     &nbsp;&nbsp;/&nbsp;&nbsp;
                    RM&nbsp;<asp:Label runat="server" ID="LblBudgeteBelian" CssClass="fw-semibold" ClientIDMode="Static" Text="0.00"/>
                 </div>
             </div>

             <!-- Two-column layout -->
             <div class="row">
                 <!-- Purchase Type -->
                 <div class="col-md-6 mb-3">
                     <label for="ddlBelian" class="fw-semibold">Select Purchase Type</label>
                     <asp:DropDownList runat="server" ID="ddlBelian" 
                         CssClass="form-select select2" 
                         data-placeholder="Choose Purchase Type"
                         DataValueField="Code" DataTextField="Name" />
                     <asp:RequiredFieldValidator ID="rfdddlBelian" runat="server" ControlToValidate="ddlBelian"  ValidationGroup="BelianGroup"
                         CssClass="text-danger small" ErrorMessage="Purchase Type is required." Display="Dynamic" />
                 </div>

                 <!-- Justification -->
                 <div class="col-md-6 mb-3">
                     <label for="txtJustificationBelian" class="fw-semibold">Justification</label>
                     <asp:TextBox runat="server" ID="txtJustificationBelian" 
                         CssClass="form-control" TextMode="MultiLine" Rows="3" 
                         placeholder="Example: Human Resources - urgent requirement" />
                     <asp:RequiredFieldValidator runat="server" ControlToValidate="txtJustificationBelian"  ValidationGroup="BelianGroup"
                         ErrorMessage="Justification is required." CssClass="text-danger small" Display="Dynamic" />
                 </div>
             </div>

             <div class="row">
                 <!-- Amount -->
                 <div class="col-md-6 mb-3">
                     <label for="txtAmountBelian" class="fw-semibold">Estimated Amount</label>
                     <div class="input-group">
                         <span class="input-group-text">RM</span>
                         <asp:TextBox ID="txtAmountBelian" runat="server" 
                             CssClass="form-control text-end input-number2" 
                             placeholder="0.00"></asp:TextBox>
                     </div>
                     <asp:RequiredFieldValidator runat="server" ControlToValidate="txtAmountBelian"  ValidationGroup="BelianGroup"
                         ErrorMessage="Amount is required." CssClass="text-danger small" Display="Dynamic" />
                 </div>

                 <!-- File Upload -->
                 <div class="col-md-6 mb-3">
                     <label for="fuDocumentBelian" class="fw-semibold">Upload Supporting Document</label>
                     <asp:FileUpload ID="fuDocumentBelian" runat="server" CssClass="form-control" />
                     <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="fuDocumentBelian" ValidationGroup="BelianGroup"
                         ErrorMessage="Please upload a document." CssClass="text-danger small" Display="Dynamic" />
                     <asp:Label ID="Label3" runat="server" CssClass="text-success small mt-1 d-block" />
                 </div>
             </div>
         </div>

    </asp:Panel>


    <script>
        function updateBalanceBekalan() {
            var balanceEl = document.getElementById("<%= LblBalanceBekalan.ClientID %>");
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
                amountEl.addEventListener("input", updateBalanceBekalan);
            }
        });

        function validateBudgetBekalan() {
            function parseNumber(id, isInput = false) {
                var el = document.getElementById(id);
                if (!el) return 0;
                var raw = isInput ? el.value : el.innerText;
                if (!raw) return 0;
                raw = raw.replace(/,/g, '').trim(); // remove commas & spaces
                return parseFloat(raw) || 0;
            }

            var balance = parseNumber("<%= LblBalanceBekalan.ClientID %>");
            var amount = parseNumber("<%= txtAmount.ClientID %>", true);

        if (balance < 0) {
            Swal.fire({
                icon: 'error',
                title: 'Insufficient Balance',
                text: 'The requested amount exceeds the available budget.'
            });

                document.getElementById("<%= rdoPeralatan.ClientID %>").checked = false;
                return false;
            }
            return true;
        }


        function updateBalanceBelian() {
            var balanceEl = document.getElementById("<%= LblBalanceBelian.ClientID %>");
            var amountEl = document.getElementById("<%= txtAmountBelian.ClientID %>");

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
            var amountEl = document.getElementById("<%= txtAmountBelian.ClientID %>");
            if (amountEl) {
                amountEl.addEventListener("input", updateBalanceBelian);
            }
        });

        function validateBudgetBelian() {
            function parseNumber(id, isInput = false) {
                var el = document.getElementById(id);
                if (!el) return 0;
                var raw = isInput ? el.value : el.innerText;
                if (!raw) return 0;
                raw = raw.replace(/,/g, '').trim(); // remove commas & spaces
                return parseFloat(raw) || 0;
            }

            var balance = parseNumber("<%= LblBalanceBelian.ClientID %>");
            var amount = parseNumber("<%= txtAmountBelian.ClientID %>", true);

        if (balance < 0) {
            Swal.fire({
                icon: 'error',
                title: 'Insufficient Balance',
                text: 'The requested amount exceeds the available budget.'
            });

                document.getElementById("<%= rdoBelian.ClientID %>").checked = false;
                return false;
            }
            return true;
        }

        $(document).ready(function () {
            $("input[id$='rdoPeralatan']").change(function () {
                if ($(this).is(":checked")) {
                    $("#<%= PnlBekalan.ClientID %>").show();
                    $("#<%= PnlBelian.ClientID %>").hide();
            }
        });

        $("input[id$='rdoBelian']").change(function () {
            if ($(this).is(":checked")) {
                $("#<%= PnlBelian.ClientID %>").show();
                $("#<%= PnlBekalan.ClientID %>").hide();
            }
        });
        });
    </script>

</asp:Content>
