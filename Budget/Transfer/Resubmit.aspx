<%@ Page Title="Resubmit Budget Transfer" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Resubmit.aspx.cs" Inherits="Prodata.WebForm.Budget.Transfer.Resubmit" %>
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
<%--                <asp:LinkButton ID="btnSave" runat="server" CssClass="btn btn-primary" OnClick="btnSave_Click" OnClientClick="collectData();"> OnClientClick="collectData();"
                    <i class="fas fa-save"></i> Save
                </asp:LinkButton>--%>
                <asp:LinkButton ID="btnSubmit1" runat="server" CssClass="btn btn-success" OnClick="btnSubmit_Click">  
                    <i class="fas fa-share"></i> Submit Application
                </asp:LinkButton>
            </div>
        </div>

        <div class="form-group" style="font-size: 1.5rem;">
            <label>BA : </label>
            <asp:Label runat="server" ID="LblBA" CssClass="form-control" Style="white-space: pre-wrap;" />
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
            <asp:TextBox runat="server" ID="txtProject" CssClass="form-control txtreadonly" placeholder="Example: KS Tenggaroh Timur" />
        </div>

        <div class="form-group">
            <label>Reference No.</label>
            <asp:TextBox runat="server" ID="txtRefNo" CssClass="form-control txtreadonly" placeholder="Example: (03)PB/4067/TGHT/820/2" />
        </div>

        <div class="form-group">
            <label>Application Date</label>
            <asp:TextBox runat="server" ID="txtDate" CssClass="form-control txtreadonly" TextMode="Date" />
        </div>

        <div class="form-group">
            <label>Estimated Cost (RM)</label>
            <asp:TextBox runat="server" ID="txtEstimatedCost" CssClass="form-control txtreadonly" placeholder="0.00" />
        </div>

        <div class="form-group">
            <label>E-VISA No.</label>
            <asp:TextBox runat="server" ID="txtEVisa" CssClass="form-control txtreadonly" placeholder="Example: 2025-00025" />
        </div>

        <div class="form-group">
            <label>Work Details</label>
            <asp:TextBox runat="server" ID="txtWorkDetails" CssClass="form-control txtreadonly" TextMode="MultiLine" Rows="3" placeholder="Example: Budget transfer request..." />
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
                    <td><asp:TextBox runat="server" ID="txtFromGL" CssClass="form-control txtreadonly" placeholder="Example: 53000060" /></td>
                    <td>
                        <asp:DropDownList runat="server" ID="ddFromBA" CssClass="form-control fixed-dropdown select2 txtreadonly" data-placeholder="BA" ></asp:DropDownList>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="ddFromBA" CssClass="text-danger" Display="Dynamic" ErrorMessage="Sila pilih BA" InitialValue=""></asp:RequiredFieldValidator>
                    </td>
                    <td><asp:TextBox runat="server" ID="txtFromBudget" CssClass="form-control txtreadonly" placeholder="0.00" oninput="formatCurrencyInput(this)" /></td>
                    <td><asp:TextBox runat="server" ID="txtFromBalance" CssClass="form-control txtreadonly" placeholder="0.00" oninput="formatCurrencyInput(this)" /></td>
                    <td><asp:TextBox runat="server" ID="txtFromTransfer" CssClass="form-control txtreadonly" placeholder="0.00" oninput="formatCurrencyInput(this)" /></td>
                    <td><asp:TextBox runat="server" ID="txtFromAfter" CssClass="form-control txtreadonly" placeholder="0.00" oninput="formatCurrencyInput(this)" /></td>
                </tr>
                <tr>
                    <td>To Budget</td>
                    <td><asp:TextBox runat="server" ID="txtToGL" CssClass="form-control txtreadonly" placeholder="Example: 55200060" /></td> 
                    <td>
                        <asp:DropDownList runat="server" ID="ddToBA" CssClass="form-control fixed-dropdown select2 txtreadonly" data-placeholder="BA" ></asp:DropDownList>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="ddToBA" CssClass="text-danger" Display="Dynamic" ErrorMessage="Sila pilih BA" InitialValue=""></asp:RequiredFieldValidator>
                    </td>
                    <td><asp:TextBox runat="server" ID="txtToBudget" CssClass="form-control txtreadonly" placeholder="0.00" oninput="formatCurrencyInput(this)" /></td>
                    <td><asp:TextBox runat="server" ID="txtToBalance" CssClass="form-control txtreadonly" placeholder="0.00" oninput="formatCurrencyInput(this)" /></td>
                    <td><asp:TextBox runat="server" ID="txtToTransfer" CssClass="form-control txtreadonly" placeholder="0.00" oninput="formatCurrencyInput(this)" /></td>
                    <td><asp:TextBox runat="server" ID="txtToAfter" CssClass="form-control txtreadonly" placeholder="0.00" oninput="formatCurrencyInput(this)" /></td>
                </tr>
            </tbody>
        </table>
        
        <h4 class="mt-4">Justification</h4>
        <asp:TextBox runat="server" ID="txtJustification" CssClass="form-control txtreadonly" TextMode="MultiLine" Rows="10" />

        <h4 class="mt-4">Remark Resubmit</h4>
        <asp:TextBox runat="server" ID="txtResubmit" CssClass="form-control" TextMode="MultiLine" Rows="10" Enabled="true"/>
        <asp:RequiredFieldValidator ID="rfvtxtResubmit" runat="server" ControlToValidate="txtResubmit"
            ErrorMessage="Please Fill Remark" CssClass="text-danger" Display="Dynamic"  />


        <h4 class="mt-4">Upload Supporting Document</h4>
        <asp:FileUpload ID="fuDocument" runat="server" CssClass="form-control" />
        <asp:RequiredFieldValidator ID="rfvFile" runat="server" ControlToValidate="fuDocument"
            ErrorMessage="Please upload a document" CssClass="text-danger" Display="Dynamic"  />


    </asp:Panel>
    
    <!-- Hidden Fields -->
    <asp:HiddenField ID="hdncurentRoleApprover" runat="server" />

    <asp:Panel runat="server" ID="pnlUploadedDocument" CssClass="form-group mt-3" Visible="false">
        <div class="d-flex align-items-center mb-3">
            <div class="bg-success text-white rounded-circle d-flex justify-content-center align-items-center" style="width: 40px; height: 40px;">
                <i class="fas fa-file-upload"></i>
            </div>
            <div class="ml-3">
                <h5 class="mb-0">Uploaded Document</h5>
                <small class="text-muted">You have already uploaded a supporting file for this request.</small>
            </div>
        </div>

        <div class="border p-3 rounded bg-white">
            <asp:PlaceHolder ID="phDocumentList" runat="server" />
        </div>
    </asp:Panel>

    <asp:Panel runat="server" ID="pnHistoryApproval" CssClass="border p-3 rounded bg-white" Visible="false">
        <h4 class="mt-4">Approval History</h4>
         <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <asp:GridView ID="gvHistory" runat="server"
                              CssClass="table table-bordered table-sm" 
                              AllowPaging="true"
                              AutoGenerateColumns="False"
                              DataKeyNames="Id"
                              EmptyDataText="No Approval record.">
                    <Columns>
                        <asp:BoundField DataField="ActionDate" HeaderText="Action Date" />
                        <asp:BoundField DataField="ActionType" HeaderText="Role Action" />
                        <asp:BoundField DataField="RoleName" HeaderText="Role" /> 
                        <asp:BoundField DataField="Status" HeaderText="Status"/>
                        <asp:BoundField DataField="Remarks" HeaderText="Remarks"/>
                    </Columns>
                </asp:GridView>
            </ContentTemplate>
        </asp:UpdatePanel>
    </asp:Panel>



</asp:Content>
