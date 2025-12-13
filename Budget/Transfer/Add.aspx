
<%@ Page Title="New Budget Transfer" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Add.aspx.cs" Inherits="Prodata.WebForm.Budget.Transfer.Add" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">  

        function parseFloatSafe(val) {
            if (!val) return 0;
            const num = parseFloat(val.replace(/[^0-9.]/g, ''));
            return isNaN(num) ? 0 : num;
        }

        function formatCurrencyInput(input, formatOnBlur = false) {
            let value = input.value.replace(/[^0-9.]/g, ''); // Remove invalid chars
            const parts = value.split('.');
            if (parts.length > 2) value = parts[0] + '.' + parts[1]; // Only first decimal
            if (parts[1]) value = parts[0] + '.' + parts[1].substring(0, 2); // Max 2 decimals
            if (formatOnBlur && value) {
                const number = parseFloat(value);
                if (!isNaN(number)) value = number.toFixed(2);
            }
            input.value = value;
        }

        function updateCalculatedFields() {
            const fromBalance = parseFloatSafe(document.getElementById("txtFromBalance").value);
            const fromTransfer = parseFloatSafe(document.getElementById("txtFromTransfer").value);
            const toBalance = parseFloatSafe(document.getElementById("txtToBalance").value);

            const fromAfter = fromBalance - fromTransfer;
            const toAfter = toBalance + fromTransfer;

            // Update calculated fields
            document.getElementById("txtFromAfter").value = fromAfter.toFixed(2);
            document.getElementById("txtToTransfer").value = fromTransfer.toFixed(2);
            document.getElementById("txtToAfter").value = toAfter.toFixed(2);

            // Live validation: show/hide message immediately
            const messageFromAfter = document.getElementById("validationMessagetxtFromAfter");
            if (fromAfter < 0) {
                messageFromAfter.style.display = "block";
            } else {
                messageFromAfter.style.display = "none";
            }

            const messageToAfter = document.getElementById("validationMessagetxtToAfter");
            if (toAfter < 0) {
                messageToAfter.style.display = "block";
            } else {
                messageToAfter.style.display = "none";
            }
        }

        function validateBeforeSubmit() {
            updateCalculatedFields();
            const fromAfter = document.getElementById("txtFromAfter").value;
            const toAfter = document.getElementById("txtToAfter").value;
            const messageFromAfter = document.getElementById("validationMessagetxtFromAfter");
            const messageToAfter = document.getElementById("validationMessagetxtToAfter");
            //alert("validateBeforeSubmit: txtFromAfter = " + fromAfter + ", txtToAfter = " + toAfter);

            let isValid = true;

            if (fromAfter < 0) {
                messageFromAfter.style.display = "block";
                isValid = false;
            }

            if (toAfter < 0) {
                messageToAfter.style.display = "block";
                isValid = false;
            }

            return isValid;
        }

        document.addEventListener("DOMContentLoaded", function () {
            const fields = ["txtFromTransfer"];
            fields.forEach(fieldId => {
                const input = document.getElementById(fieldId);
                if (input) {
                    input.addEventListener("input", function () {
                        formatCurrencyInput(input, false);
                        updateCalculatedFields();
                    });
                    input.addEventListener("blur", function () {
                        formatCurrencyInput(input, true);
                        updateCalculatedFields();
                    });
                }
            });
        });
        function beforeSubmit() {
            // 1. Run your custom math validation (Calculated fields)
            if (!validateBeforeSubmit()) {
                return false; // Stop if math is wrong (Negative balance)
            }

            // 2. Run ASP.NET Standard Validators (RequiredFieldValidators)
            // This checks all your <asp:RequiredFieldValidator> controls
            if (typeof (Page_ClientValidate) == 'function') {
                if (!Page_ClientValidate()) {
                    return false; // Stop if any required field is empty
                }
            }

            // 3. If everything is valid, show the preloader and allow Postback
            $("#pagePreloader").fadeIn(200);
            return true;
        }
    </script>


    <style>
        .fixed-dropdown {
            width: 300px !important;
        }
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
    <asp:Panel runat="server" CssClass="card p-4">
        <div class="card-header card-header-sticky">
            <h2 class="card-title d-none d-sm-inline"><%: Page.Title %></h2>
            <div class="card-tools">
                <asp:LinkButton ID="btnBack" runat="server" CssClass="btn btn-default" PostBackUrl="~/Budget/Transfer/Default" CausesValidation="false">
                    <i class="fas fa-angle-double-left"></i> Back
                </asp:LinkButton>
                <asp:LinkButton ID="btnSubmit1" runat="server" CssClass="btn btn-success" OnClick="btnSubmit_Click1" OnClientClick="return beforeSubmit();">
                    <i class="fas fa-share"></i> Submit Application
                </asp:LinkButton>
            </div>
        </div>

        <div class="form-group d-flex align-items-center" style="font-size: 2rem;">
            <label class="me-2 mb-0 fw-semibold text-dark">BA :</label>
            <asp:PlaceHolder runat="server" ID="phBA">
                <asp:Label 
                    runat="server" 
                    ID="LblBA" 
                    CssClass="fw-bold text-dark me-1" 
                    Style="font-size: 2rem;" 
                    ClientIDMode="Static" />
                <span class="fw-bold text-dark" style="font-size: 2rem;">(</span>
                <asp:Label 
                    runat="server" 
                    ID="LblBAName" 
                    CssClass="fw-bold text-dark" 
                    Style="font-size: 2rem;" 
                    ClientIDMode="Static" />
                <span class="fw-bold text-dark" style="font-size: 2rem;">)</span>
            </asp:PlaceHolder>

            <asp:PlaceHolder runat="server" ID="phBADropdown">
                This ID Not Allow to Create Transfer Budget
            </asp:PlaceHolder>
        </div>

        <div class="form-group">
            <label class="d-block mb-2 font-weight-bold">BUDGET TYPE</label>
            <div class="btn-group btn-group-toggle" data-toggle="buttons">
                <label class="btn btn-outline-primary">
                    <asp:RadioButton runat="server" ID="rdoOpex" GroupName="BudgetType" CssClass="d-none" Checked="true" />
                    <span>OPEX</span>
                </label>
                <label class="btn btn-outline-primary ml-2">
                    <asp:RadioButton runat="server" ID="rdoCapex" GroupName="BudgetType" CssClass="d-none" Enabled="false"/>
                    <span>CAPEX</span> 
                </label> 
            </div>
        </div>
        <span class="text-danger">(CAPEX not available)</span>

        <div class="form-group">
            <label>Project</label>
            <asp:TextBox runat="server" ID="txtProject" CssClass="form-control" placeholder="Example: KS Tenggaroh Timur" />
            <asp:RequiredFieldValidator ID="rfvProject" runat="server" ControlToValidate="txtProject" CssClass="text-danger" ErrorMessage="Project is required." Display="Dynamic" />
        </div>

        <div class="form-group">
            <label>Reference No.</label>
            <asp:TextBox runat="server" ID="txtRefNo" CssClass="form-control txtreadonly" placeholder="Example: (03)PB/4067/TGHT/820/2" />
            <asp:RequiredFieldValidator ID="rfvRefNo" runat="server" ControlToValidate="txtRefNo" CssClass="text-danger" ErrorMessage="Reference No. is required." Display="Dynamic" />
        </div>

        <div class="form-group">
            <label>Application Date</label>
            <asp:TextBox runat="server" ID="txtDate" CssClass="form-control txtreadonly" TextMode="Date" />
            <asp:RequiredFieldValidator ID="rfvDate" runat="server" ControlToValidate="txtDate" CssClass="text-danger" ErrorMessage="Date is required." Display="Dynamic" />
        </div>

        <div class="form-group">
            <label>Estimated Cost (RM)</label>
            <div class="input-group">
                <div class="input-group-prepend">
                    <span class="input-group-text">RM</span>
                </div>
                <asp:TextBox runat="server" ID="txtEstimatedCost" CssClass="form-control" placeholder="0.00" oninput="formatCurrencyInput(this)"/>
            </div>
            <asp:RequiredFieldValidator ID="rfvEstimatedCost" runat="server" ControlToValidate="txtEstimatedCost" CssClass="text-danger" ErrorMessage="Estimated Cost is required." Display="Dynamic" />
        </div>

        <div class="form-group">
            <label>E-VISA No.</label>
            <asp:TextBox runat="server" ID="txtEVisa" CssClass="form-control txtreadonly" placeholder="Example: 2025-00025" />
            <asp:RequiredFieldValidator ID="rfvEVisa" runat="server" ControlToValidate="txtEVisa" CssClass="text-danger" ErrorMessage="E-VISA No. is required." Display="Dynamic" />
        </div>

        <div class="form-group">
            <label>Work Details</label>
            <asp:TextBox runat="server" ID="txtWorkDetails" CssClass="form-control" TextMode="MultiLine" Rows="3" placeholder="Example: Budget transfer request..." />
            <asp:RequiredFieldValidator ID="rfvWorkDetails" runat="server" ControlToValidate="txtWorkDetails" CssClass="text-danger" ErrorMessage="Work details are required." Display="Dynamic" />
        </div>

        <h4 class="mt-4">Budget Details</h4>

        <table class="table table-bordered">
            <thead>
                <tr>
                    <th>Item</th> 
                    <th>GL</th> 
                    <th>Budget Type </th>
                    <th style="min-width: 120px;">BA</th>
                    <th>Original Budget (RM)</th>
                    <th>Current Balance (RM)</th>
                    <th>Transfer Amount (RM)</th>
                    <th>Balance After Transfer (RM)</th>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td>From Budget</td>
                    <td>
                        <asp:TextBox ID="txtFromGLCode" runat="server" CssClass="form-control" 
                            placeholder="Enter GL Code" MaxLength="20" />
                        <asp:RequiredFieldValidator ID="rfvFromGLCode" runat="server" 
                            ControlToValidate="txtFromGLCode" CssClass="text-danger" 
                            ErrorMessage="GL required" Display="Dynamic" />
                    </td>
                    <td>
                        <asp:DropDownList runat="server" ID="txtFromBudgetType" CssClass="form-control fixed-dropdown select2"
                            AutoPostBack="true" OnSelectedIndexChanged="From_SelectedIndexChanged" data-placeholder="Type"
                            DataValueField="Code" DataTextField="DisplayName" />
                        <asp:RequiredFieldValidator ID="rfvFromBudgetType" runat="server" ControlToValidate="txtFromBudgetType" CssClass="text-danger" ErrorMessage="Required" Display="Dynamic" />
                    </td>
                    <td>
                        <asp:DropDownList runat="server" ID="ddFromBA" CssClass="form-control fixed-dropdown select2"
                            AutoPostBack="true" OnSelectedIndexChanged="From_SelectedIndexChanged" data-placeholder="BA"
                            DataValueField="Code" DataTextField="DisplayName" />
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="ddFromBA" CssClass="text-danger" Display="Dynamic" ErrorMessage="Required BA" InitialValue=""></asp:RequiredFieldValidator>
                    </td>
                    <td>
                        <div class="input-group">
                            <div class="input-group-prepend">
                                <span class="input-group-text">RM</span>
                            </div>
                            <asp:TextBox runat="server" ID="txtFromBudget" CssClass="form-control txtreadonly" placeholder="0.00" oninput="formatCurrencyInput(this)"  />
                        </div>
                    </td>
                    <td>
                        <div class="input-group">
                            <div class="input-group-prepend">
                                <span class="input-group-text">RM</span>
                            </div>
                            <asp:TextBox ID="txtFromBalance" runat="server" ClientIDMode="Static" CssClass="form-control txtreadonly" text="0.00" />
                        </div>
                    </td>
                    <td>
                        <div class="input-group">
                            <div class="input-group-prepend">
                                <span class="input-group-text">RM</span>
                            </div>
                            <asp:TextBox ID="txtFromTransfer" runat="server" ClientIDMode="Static" CssClass="form-control" placeholder="0.00" />
                        </div>
                        <asp:RequiredFieldValidator ID="rfvFromTransfer" runat="server" ControlToValidate="txtFromTransfer" CssClass="text-danger" ErrorMessage="Required" Display="Dynamic" />
                    </td>
                    <td>
                        <div class="input-group">
                            <div class="input-group-prepend">
                                <span class="input-group-text">RM</span>
                            </div>
                            <asp:TextBox ID="txtFromAfter" runat="server" ClientIDMode="Static" CssClass="form-control txtreadonly"   Text="0.00"/>
                        </div>
                        <span id="validationMessagetxtFromAfter" class="text-danger mt-2" style="display:none;">
                            Balance after transfer cannot be zero or negative. Please adjust the transfer amount.
                        </span>
                    </td>
                </tr>
                <tr>
                    <td>To Budget</td>
                    <td>
                        <asp:TextBox ID="txtToGLCode" runat="server" CssClass="form-control" 
                            placeholder="Enter GL Code" MaxLength="20" />
                        <asp:RequiredFieldValidator ID="rfvToGLCode" runat="server" 
                            ControlToValidate="txtToGLCode" CssClass="text-danger" 
                            ErrorMessage="GL required" Display="Dynamic" />
                    </td>
                    <td>
                        <asp:DropDownList runat="server" ID="txtToBudgetType" CssClass="form-control fixed-dropdown select2" AutoPostBack="true" OnSelectedIndexChanged="To_SelectedIndexChanged" 
                                                DataValueField="Code" DataTextField="DisplayName" data-placeholder="Type" />
                        <asp:RequiredFieldValidator ID="rfvtxtToBudgetType" runat="server" ControlToValidate="txtToBudgetType" CssClass="text-danger" ErrorMessage="Required" Display="Dynamic" />
                    </td>
                    <td>
                        <asp:Label runat="server" ID="lblToBA"></asp:Label>
                    </td>
                    <td> 
                        <div class="input-group">
                            <div class="input-group-prepend">
                                <span class="input-group-text">RM</span>
                            </div>
                            <asp:TextBox runat="server" ID="txtToBudget" CssClass="form-control txtreadonly" placeholder="0.00" oninput="formatCurrencyInput(this)" Text="0.00"/>
                        </div>
                    </td>
                    <td>
                        <div class="input-group">
                            <div class="input-group-prepend">
                                <span class="input-group-text">RM</span>
                            </div>
                            <asp:TextBox ID="txtToBalance" runat="server" ClientIDMode="Static" CssClass="form-control txtreadonly" text="0.00" />
                        </div>
                    </td>
                    <td>
                        <div class="input-group">
                            <div class="input-group-prepend">
                                <span class="input-group-text">RM</span>
                            </div>
                            <asp:TextBox ID="txtToTransfer" runat="server" ClientIDMode="Static" CssClass="form-control txtreadonly" Text="0.00"/>
                        </div>
                    </td>
                    <td>
                        <div class="input-group">
                            <div class="input-group-prepend">
                                <span class="input-group-text">RM</span>
                            </div>
                            <asp:TextBox ID="txtToAfter" runat="server" ClientIDMode="Static" CssClass="form-control txtreadonly"  Text="0.00"/> 
                        </div>
                        <span id="validationMessagetxtToAfter" class="text-danger mt-2" style="display:none;">
                            Balance after transfer cannot be zero or negative. Please adjust the transfer amount.
                        </span>
                    </td>
                </tr>
            </tbody>
        </table>

        <h4 class="mt-4">Justification</h4>
        <asp:TextBox runat="server" ID="txtJustification" CssClass="form-control" TextMode="MultiLine" Rows="10" />
        <asp:RequiredFieldValidator ID="rfvtxtJustification" runat="server" ControlToValidate="txtJustification" CssClass="text-danger" ErrorMessage="Required Justification" Display="Dynamic" />

        <h4 class="mt-4">Upload Supporting Document</h4>
        <asp:FileUpload ID="fuDocument" runat="server" CssClass="form-control" />
        <asp:RequiredFieldValidator ID="rfvFile" runat="server" ControlToValidate="fuDocument"
            ErrorMessage="Please upload a document" CssClass="text-danger" Display="Dynamic" />
        <asp:Label ID="lblUploadResult" runat="server" CssClass="text-success mt-2" />

    </asp:Panel>
</asp:Content>



