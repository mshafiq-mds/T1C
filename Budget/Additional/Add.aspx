<%@ Page Title="New Additional Budget" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Add.aspx.cs" Inherits="Prodata.WebForm.Budget.AddBudget.Add" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript">
        function formatCurrencyInput(input, formatOnBlur = false) {
            let value = input.value.replace(/[^0-9.]/g, ''); // Remove all except digits and dot
            const parts = value.split('.');

            if (parts.length > 2) {
                // More than one dot — keep only the first two parts
                value = parts[0] + '.' + parts[1];
            }

            if (parts[1]) {
                // Limit to two decimal places
                value = parts[0] + '.' + parts[1].substring(0, 2);
            }

            // Format to two decimal places only on blur or final format
            if (formatOnBlur && value) {
                const number = parseFloat(value);
                if (!isNaN(number)) {
                    value = number.toFixed(2); // Ensures 345 => 345.00
                }
            }

            input.value = value;
        }

        document.addEventListener("DOMContentLoaded", function () {
            const rmInputs = document.querySelectorAll("input[id*=txtBudgetEstimate], input[id*=txtCorporateCost], input[id*=txtOpsDriverCost], input[id*=txtTotalCost], input[id*=txtApprovedBudget], input[id*=txtNewTotalBudget], input[id*=txtAdditionalBudget]");

            rmInputs.forEach(input => {
                input.addEventListener("input", function () {
                    formatCurrencyInput(input, false);
                });

                // Format as currency when user finishes typing (on blur)
                input.addEventListener("blur", function () {
                    formatCurrencyInput(input, true);
                });
            });
        });

        function beforeSubmit() {
            // Disable all buttons to prevent multiple clicks
            $("button, .btn").prop("disabled", true);

            // Show the page-specific preloader
            $("#pagePreloader").fadeIn(200);
             
            // Continue with ASP.NET postback
            return true;
        }
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
    <div id="pagePreloader" class="page-preloader flex-column justify-content-center align-items-center" style="display:none;">
        <img src="<%= ResolveUrl("~/Images/Felda_Global_Ventures_Logo.png") %>" 
             alt="Loading..." height="200" width="200" />
        <p class="mt-3 text-white">Processing...</p>
    </div>

    <asp:Panel runat="server" CssClass="card p-4">
        <div class="card-header card-header-sticky">
            <h2 class="card-title d-none d-sm-inline"><%: Page.Title %></h2>
            <div class="card-tools">
                <asp:LinkButton ID="btnBack" runat="server" CssClass="btn btn-default" PostBackUrl="~/Budget/Additional/Default" CausesValidation="false">
                    <i class="fas fa-angle-double-left"></i> Back
                </asp:LinkButton>
<%--                <asp:LinkButton ID="btnSave" runat="server" CssClass="btn btn-primary" OnClick="btnSave_Click" OnClientClick="collectData();">
                    <i class="fas fa-save"></i> Save
                </asp:LinkButton>--%>
                <asp:LinkButton ID="btnSubmit" runat="server" CssClass="btn btn-success" OnClick="btnSubmit_Click" OnClientClick="return beforeSubmit();"> 
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
            <label class="d-block mb-2 font-weight-bold">LETTER OF AUTHORITY</label>
            <div class="btn-group btn-group-toggle" data-toggle="buttons">
                <label class="btn btn-outline-primary">
                    <asp:RadioButton runat="server" ID="rdoFinance" GroupName="CheckType" CssClass="d-none" Checked="true" />
                    <span>FINANCE</span>
                </label>
                <label class="btn btn-outline-primary ml-2">
                    <asp:RadioButton runat="server" ID="rdoCOGS" GroupName="CheckType" CssClass="d-none" Checked="false"/>
                    <span>COST OF GOOD SOLD</span>
                </label>
            </div>
        </div>

        <div class="form-group">
            <label>Project / Department</label>
            <asp:TextBox runat="server" ID="txtProject" CssClass="form-control" placeholder="Example: HUMAN RESOURCES" />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtProject" ErrorMessage="Project/Department is required." CssClass="text-danger" Display="Dynamic" />
        </div>

        <div class="form-group">
            <label>Reference No.</label>
            <asp:TextBox runat="server" ID="txtRefNo" CssClass="form-control" placeholder="Example: 2022-00283" />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtRefNo" ErrorMessage="Reference No. is required." CssClass="text-danger" Display="Dynamic" />
        </div>

        <div class="form-group">
            <label>Application Date</label>
            <asp:TextBox runat="server" ID="txtDate" CssClass="form-control txtreadonly" TextMode="Date"  />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtDate" ErrorMessage="Application Date is required." CssClass="text-danger" Display="Dynamic" />
        </div>

        <div class="form-group">
            <label>Estimated Cost (RM)</label>
            <asp:TextBox runat="server" ID="txtBudgetEstimate" CssClass="form-control" placeholder="0.00" oninput="formatCurrencyInput(this)" />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtBudgetEstimate" ErrorMessage="Estimated Cost is required." CssClass="text-danger" Display="Dynamic" />
        </div>

        <div class="form-group">
            <label>E-VISA No.</label>
            <asp:TextBox runat="server" ID="txtEVisa" CssClass="form-control" />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtEVisa" ErrorMessage="E-VISA No. is required." CssClass="text-danger" Display="Dynamic" />
        </div>

        <div class="form-group">
            <label>Application Details</label>
            <asp:TextBox runat="server" ID="txtRequestDetails" CssClass="form-control" TextMode="MultiLine" Rows="3" placeholder="Example: Additional Budget..." />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtRequestDetails" ErrorMessage="Application Details are required." CssClass="text-danger" Display="Dynamic" />
        </div>

        <div class="form-group">
            <label>Reason for Application</label>
            <asp:TextBox runat="server" ID="txtReason" CssClass="form-control" TextMode="MultiLine" Rows="10" />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtReason" ErrorMessage="Reason is required." CssClass="text-danger" Display="Dynamic" />
        </div>

        <h4 class="mt-4">Additional Budget Allocation Details</h4>
        <table class="table table-bordered">
            <thead>
                <tr>
                    <th>Cost Centre</th>
                    <th class="width-80" style="width: 195px">BudgetType</th>
                    <th>Approved Budget</th>
                    <th>New Budget</th>
                    <th>Additional Budget Value</th>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td>
                        <asp:TextBox runat="server" ID="txtCostCentre" CssClass="form-control" placeholder="Example: 40FPS8990 UNIFORM" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCostCentre" ErrorMessage="Cost Centre is required." CssClass="text-danger" Display="Dynamic" />
                    </td>
                    <td class="width-80" style="width: 195px"> 
                        <asp:DropDownList runat="server" ID="txtToBudgetType" CssClass="form-control fixed-dropdown select2" data-placeholder="Type"
                            DataValueField="Code" DataTextField="DisplayName" />
                        <asp:RequiredFieldValidator ID="rfvToBudgetType" runat="server" ControlToValidate="txtToBudgetType" CssClass="text-danger" ErrorMessage="Required" Display="Dynamic" />
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="txtApprovedBudget" CssClass="form-control" placeholder="0.00" oninput="formatCurrencyInput(this)" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtApprovedBudget" ErrorMessage="Approved Budget is required." CssClass="text-danger" Display="Dynamic" />
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="txtNewTotalBudget" CssClass="form-control" placeholder="0.00" oninput="formatCurrencyInput(this)" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtNewTotalBudget" ErrorMessage="New Budget is required." CssClass="text-danger" Display="Dynamic" />
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="txtAdditionalBudget" CssClass="form-control" placeholder="0.00" oninput="formatCurrencyInput(this)" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtAdditionalBudget" ErrorMessage="Additional Budget Value is required." CssClass="text-danger" Display="Dynamic" />
                    </td>
                </tr>
            </tbody>
        </table>


        <h4 class="mt-4">Upload Supporting Document</h4>
        <asp:FileUpload ID="fuDocument" runat="server" CssClass="form-control" />
        <asp:RequiredFieldValidator ID="rfvFile" runat="server" ControlToValidate="fuDocument"
            ErrorMessage="Please upload a document" CssClass="text-danger" Display="Dynamic" />
        <asp:Label ID="lblUploadResult" runat="server" CssClass="text-success mt-2" />

    </asp:Panel>
</asp:Content>

