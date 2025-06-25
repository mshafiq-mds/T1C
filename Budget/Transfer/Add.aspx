<%@ Page Title="New Budget Transfer" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Add.aspx.cs" Inherits="Prodata.WebForm.Budget.Transfer.Add" %>
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
            const rmInputs = document.querySelectorAll("input[id*=txtEstimatedCost], input[id*=txtFromBudget], input[id*=txtFromBalance], input[id*=txtFromTransfer], input[id*=txtFromAfter], input[id*=txtToBudget], input[id*=txtToBalance], input[id*=txtToTransfer], input[id*=txtToAfter]");

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
    </script>

    <style>
        .fixed-dropdown {
            width: 300px !important;
        }
    </style>
    <asp:Panel runat="server" CssClass="card p-4">
        <div class="card-header card-header-sticky">
            <h2 class="card-title d-none d-sm-inline"><%: Page.Title %></h2>
            <div class="card-tools">
                <asp:LinkButton ID="btnBack" runat="server" CssClass="btn btn-default" PostBackUrl="/Budget/Transfer/Default" CausesValidation="false">
                    <i class="fas fa-angle-double-left"></i> Back
                </asp:LinkButton>
<%--                <asp:LinkButton ID="btnSave" runat="server" CssClass="btn btn-primary" OnClick="btnSave_Click" OnClientClick="collectData();">
                    <i class="fas fa-save"></i> Save
                </asp:LinkButton>--%>
                <asp:LinkButton ID="btnSubmit1" runat="server" CssClass="btn btn-success" OnClick="btnSubmit_Click1" OnClientClick="collectData();"> <%----%>
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
            <asp:TextBox runat="server" ID="txtRefNo" CssClass="form-control" placeholder="Example: (03)PB/4067/TGHT/820/2" />
            <asp:RequiredFieldValidator ID="rfvRefNo" runat="server" ControlToValidate="txtRefNo" CssClass="text-danger" ErrorMessage="Reference No. is required." Display="Dynamic" />
        </div>

        <div class="form-group">
            <label>Application Date</label>
            <asp:TextBox runat="server" ID="txtDate" CssClass="form-control" TextMode="Date"  Enabled="false"/>
            <asp:RequiredFieldValidator ID="rfvDate" runat="server" ControlToValidate="txtDate" CssClass="text-danger" ErrorMessage="Date is required." Display="Dynamic" />
        </div>

        <div class="form-group">
            <label>Estimated Cost (RM)</label>
            <asp:TextBox runat="server" ID="txtEstimatedCost" CssClass="form-control" placeholder="0.00" />
            <asp:RequiredFieldValidator ID="rfvEstimatedCost" runat="server" ControlToValidate="txtEstimatedCost" CssClass="text-danger" ErrorMessage="Estimated Cost is required." Display="Dynamic" />
        </div>

        <div class="form-group">
            <label>E-VISA No.</label>
            <asp:TextBox runat="server" ID="txtEVisa" CssClass="form-control" placeholder="Example: 2025-00025" />
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
                    <th>GL/Asset Class</th>
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
                        <asp:TextBox runat="server" ID="txtFromGL" CssClass="form-control" placeholder="Example: 53000060" />
                        <asp:RequiredFieldValidator ID="rfvFromGL" runat="server" ControlToValidate="txtFromGL" CssClass="text-danger" ErrorMessage="Required" Display="Dynamic" />
                    </td>
                    <td>
                        <asp:DropDownList runat="server" ID="ddFromBA" CssClass="form-control fixed-dropdown select2" data-placeholder="BA"></asp:DropDownList>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="ddFromBA" CssClass="text-danger" Display="Dynamic" ErrorMessage="Required BA" InitialValue=""></asp:RequiredFieldValidator>
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="txtFromBudget" CssClass="form-control" placeholder="0.00" oninput="formatCurrencyInput(this)" />
                        <asp:RequiredFieldValidator ID="rfvFromBudget" runat="server" ControlToValidate="txtFromBudget" CssClass="text-danger" ErrorMessage="Required" Display="Dynamic" />
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="txtFromBalance" CssClass="form-control" placeholder="0.00" oninput="formatCurrencyInput(this)" />
                        <asp:RequiredFieldValidator ID="rfvFromBalance" runat="server" ControlToValidate="txtFromBalance" CssClass="text-danger" ErrorMessage="Required" Display="Dynamic" />
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="txtFromTransfer" CssClass="form-control" placeholder="0.00" oninput="formatCurrencyInput(this)" />
                        <asp:RequiredFieldValidator ID="rfvFromTransfer" runat="server" ControlToValidate="txtFromTransfer" CssClass="text-danger" ErrorMessage="Required" Display="Dynamic" />
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="txtFromAfter" CssClass="form-control" placeholder="0.00" oninput="formatCurrencyInput(this)" />
                        <asp:RequiredFieldValidator ID="rfvFromAfter" runat="server" ControlToValidate="txtFromAfter" CssClass="text-danger" ErrorMessage="Required" Display="Dynamic" />
                    </td>
                </tr>
                <tr>
                    <td>To Budget</td>
                    <td>
                        <asp:TextBox runat="server" ID="txtToGL" CssClass="form-control" placeholder="Example: 55200060" />
                        <asp:RequiredFieldValidator ID="rfvToGL" runat="server" ControlToValidate="txtToGL" CssClass="text-danger" ErrorMessage="Required" Display="Dynamic" />
                    </td>
                    <td>
                        <asp:Label runat="server" ID="lblToBA"></asp:Label>
<%--                        <asp:DropDownList runat="server" ID="ddToBA" CssClass="form-control fixed-dropdown select2" data-placeholder="BA"></asp:DropDownList>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="ddToBA" CssClass="text-danger" Display="Dynamic" ErrorMessage="Required BA" InitialValue=""></asp:RequiredFieldValidator>--%>
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="txtToBudget" CssClass="form-control" placeholder="0.00" oninput="formatCurrencyInput(this)" />
                        <asp:RequiredFieldValidator ID="rfvToBudget" runat="server" ControlToValidate="txtToBudget" CssClass="text-danger" ErrorMessage="Required" Display="Dynamic" />
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="txtToBalance" CssClass="form-control" placeholder="0.00" oninput="formatCurrencyInput(this)" />
                        <asp:RequiredFieldValidator ID="rfvToBalance" runat="server" ControlToValidate="txtToBalance" CssClass="text-danger" ErrorMessage="Required" Display="Dynamic" />
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="txtToTransfer" CssClass="form-control" placeholder="0.00" oninput="formatCurrencyInput(this)" />
                        <asp:RequiredFieldValidator ID="rfvToTransfer" runat="server" ControlToValidate="txtToTransfer" CssClass="text-danger" ErrorMessage="Required" Display="Dynamic" />
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="txtToAfter" CssClass="form-control" placeholder="0.00" oninput="formatCurrencyInput(this)" />
                        <asp:RequiredFieldValidator ID="rfvToAfter" runat="server" ControlToValidate="txtToAfter" CssClass="text-danger" ErrorMessage="Required" Display="Dynamic" />
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
