<%@ Page Title="Edit Budget Approver" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EditV2.aspx.cs" Inherits="Prodata.WebForm.Budget.Transfer.EditV2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>

    <script type="text/javascript">
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

        // Handle Resubmit Confirmation
        function confirmSubmit(e) {
            e.preventDefault();

            var remarksControl = document.getElementById('<%= txtResubmit.ClientID %>');
            var fileUploadControl = document.getElementById('<%= fuDocument.ClientID %>');
            
            var remarks = remarksControl ? remarksControl.value : "";
            var hasFile = fileUploadControl && fileUploadControl.files.length > 0;
            var fileName = hasFile ? fileUploadControl.files[0].name : "No new file selected (Previous files kept)";

            if (!remarks.trim()) {
                Swal.fire({
                    icon: 'warning',
                    title: 'Missing Remarks',
                    text: 'Please provide remarks explaining the resubmission.',
                    confirmButtonColor: '#d33'
                });
                if(remarksControl) remarksControl.focus();
                return false;
            }

            var summaryHtml = `
                <div class="text-left" style="font-size: 0.95rem;">
                    <p><strong>Remarks:</strong><br/> <i class="text-muted">${remarks}</i></p>
                    <p><strong>Document:</strong><br/> <span class="text-primary">${fileName}</span></p>
                </div>
                <hr/>
                <p>Are you sure you want to resubmit this application?</p>
            `;

            Swal.fire({
                title: 'Confirm Resubmission',
                html: summaryHtml,
                icon: 'question',
                showCancelButton: true,
                confirmButtonColor: '#28a745',
                cancelButtonColor: '#6c757d',
                confirmButtonText: '<i class="fas fa-check"></i> Yes, Submit',
                cancelButtonText: 'Cancel'
            }).then((result) => {
                if (result.isConfirmed) {
                    $("#pagePreloader").fadeIn(200);
                    document.getElementById('<%= btnConfirmSubmit.ClientID %>').click();
                }
            });

            return false;
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
        .txtreadonly { background-color: #e9ecef !important; cursor: not-allowed; opacity: 1; }
        .min-w-150 { min-width: 150px; }
        .min-w-200 { min-width: 200px; }
        
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
            
            <div class="card-header card-header-sticky d-flex align-items-center position-sticky top-0" style="z-index: 1000; width: 100%;">
                <h3 class="card-title m-0 fw-bold text-dark"><i class="fas fa-edit mr-2"></i><%: Page.Title %></h3>
                <div class="card-tools" style="margin-left: auto !important;">
                    <asp:LinkButton ID="btnBack" runat="server" CssClass="btn btn-outline-secondary mr-2" PostBackUrl="~/Budget/Transfer/Default" CausesValidation="false">
                        <i class="fas fa-angle-double-left"></i> Back
                    </asp:LinkButton>
                    <button id="btnTriggerSweetAlert" class="btn btn-success" onclick="return confirmSubmit(event);">
                        <i class="fas fa-paper-plane"></i> Submit Application
                    </button>
                    <asp:Button ID="btnConfirmSubmit" runat="server" OnClick="btnSubmit_Click" style="display:none;" />
                </div>
            </div>

            <div class="card-body p-4">
                
                <h4 class="section-title mt-0">Application Information</h4>
                
                <div class="row">
                    <div class="col-md-3 mb-3">
                        <label class="form-label">Budget Type</label><br />
                        <div class="btn-group btn-group-toggle w-100">
                            <label class="btn btn-outline-primary active w-50 disabled">
                                <asp:RadioButton runat="server" ID="rdoOpex" GroupName="BudgetType" CssClass="d-none" Enabled="false" /> OPEX
                            </label>
                            <label class="btn btn-outline-secondary w-50 disabled">
                                <asp:RadioButton runat="server" ID="rdoCapex" GroupName="BudgetType" CssClass="d-none" Enabled="false"/> CAPEX
                            </label> 
                        </div>
                    </div>
                    
                    <div class="col-md-3 mb-3">
                        <label class="form-label">Reference No.</label>
                        <asp:TextBox runat="server" ID="txtRefNo" CssClass="form-control txtreadonly font-weight-bold text-primary" ReadOnly="true" />
                    </div>

                    <div class="col-md-3 mb-3">
                        <label class="form-label">E-VISA No.</label>
                        <asp:TextBox runat="server" ID="txtEVisa" CssClass="form-control txtreadonly font-weight-bold text-info" ReadOnly="true" />
                    </div>

                    <div class="col-md-3 mb-3">
                        <label class="form-label">Application Date</label>
                        <asp:TextBox runat="server" ID="txtDate" CssClass="form-control txtreadonly" ReadOnly="true" />
                    </div>
                </div>

                <div class="row">
                    <div class="col-md-8 mb-3">
                        <label class="form-label">Project Name</label>
                        <asp:TextBox runat="server" ID="txtProject" CssClass="form-control txtreadonly" ReadOnly="true" />
                    </div>
                    
                    <div class="col-md-4 mb-3">
                        <label class="form-label">Estimated Cost</label>
                        <div class="input-group">
                            <div class="input-group-prepend"><span class="input-group-text">RM</span></div>
                            <asp:TextBox runat="server" ID="txtEstimatedCost" CssClass="form-control txtreadonly font-weight-bold" ReadOnly="true" />
                        </div>
                    </div>
                </div>

                <div class="row">
                    <div class="col-md-12 mb-3">
                        <label class="form-label">Work Details</label>
                        <asp:TextBox runat="server" ID="txtWorkDetails" CssClass="form-control txtreadonly" TextMode="MultiLine" Rows="2" ReadOnly="true" />
                    </div>
                </div>

                <h4 class="section-title">Budget Flow Details</h4>
                
                <div class="row mb-3">
                    <div class="col-md-6">
                        <label class="form-label text-danger font-weight-bold"><i class="fas fa-sign-out-alt mr-1"></i> Source Business Area (FROM)</label>
                        <div class="form-control bg-light font-weight-bold text-dark shadow-sm txtreadonly border-danger" style="border-left: 4px solid #dc3545;">
                            <asp:Label runat="server" ID="lblGlobalFromBA" ClientIDMode="Static" />
                        </div>
                    </div>
                    <div class="col-md-6">
                        <label class="form-label text-success font-weight-bold"><i class="fas fa-sign-in-alt mr-1"></i> Destination Business Area (TO)</label>
                        <div class="form-control bg-light font-weight-bold text-dark shadow-sm txtreadonly border-success" style="border-left: 4px solid #28a745;">
                            <asp:Label runat="server" ID="lblGlobalToBA" ClientIDMode="Static" />
                        </div>
                    </div>
                </div>

                <div class="table-responsive shadow-sm border rounded mb-4">
                    <table class="table table-bordered table-custom mb-0">
                        <thead>
                            <tr>
                                <th style="width: 80px;">Direction</th> 
                                <th class="min-w-150">GL Code</th> 
                                <th class="min-w-200">Budget Type</th>
                                <th class="min-w-150">Original (RM)</th>
                                <th class="min-w-150">Current Bal (RM)</th>
                                <th class="min-w-150">Transfer (RM)</th>
                                <th class="min-w-150">Bal After (RM)</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rptFromBudgets" runat="server">
                                <ItemTemplate>
                                    <tr class="bg-light">
                                        <td class="font-weight-bold text-danger text-center align-middle"><i class="fas fa-arrow-up mr-1"></i> FROM</td>
                                        <td class="align-middle"><asp:Label ID="txtFromGLCode" runat="server" CssClass="form-control form-control-sm txtreadonly" Text='<%# Eval("FromGL") %>' /></td>
                                        <td class="align-middle"><asp:Label ID="txtFromBudgetTypeName" runat="server" CssClass="form-control form-control-sm txtreadonly" Text='<%# GetBudgetTypeName(Eval("FromBudgetType")) %>' /></td>
                                        <td class="align-middle"><asp:Label runat="server" CssClass="form-control form-control-sm txtreadonly text-right" Text='<%# Eval("FromBudget", "{0:N2}") %>' /></td>
                                        <td class="align-middle"><asp:Label runat="server" CssClass="form-control form-control-sm txtreadonly text-right" Text='<%# Eval("FromBalance", "{0:N2}") %>' /></td>
                                        <td class="align-middle"><asp:Label runat="server" CssClass="form-control form-control-sm txtreadonly text-right font-weight-bold text-danger" Text='<%# Eval("FromTransfer", "{0:N2}") %>' /></td>
                                        <td class="align-middle"><asp:Label runat="server" CssClass="form-control form-control-sm txtreadonly text-right" Text='<%# Eval("FromAfter", "{0:N2}") %>'/></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>

                            <tr style="background-color: #e8f4f8; border-top: 3px solid #dee2e6;">
                                <td class="font-weight-bold text-success text-center align-middle"><i class="fas fa-arrow-down mr-1"></i> TO</td>
                                <td class="align-middle"><asp:Label ID="txtToGL" runat="server" CssClass="form-control form-control-sm txtreadonly" /></td>
                                <td class="align-middle"><asp:Label ID="txtToBudgetType" runat="server" CssClass="form-control form-control-sm txtreadonly" /></td>
                                <td class="align-middle"><asp:Label ID="txtToBudget" runat="server" CssClass="form-control form-control-sm txtreadonly text-right" /></td>
                                <td class="align-middle"><asp:Label ID="txtToBalance" runat="server" CssClass="form-control form-control-sm txtreadonly text-right" /></td>
                                <td class="align-middle"><asp:Label ID="txtToTransfer" runat="server" CssClass="form-control form-control-sm txtreadonly text-right font-weight-bold text-success" /></td>
                                <td class="align-middle"><asp:Label ID="txtToAfter" runat="server" CssClass="form-control form-control-sm txtreadonly text-right font-weight-bold" /></td>
                            </tr>
                        </tbody>
                    </table>
                </div>

                <div class="row">
                    <div class="col-md-12 mb-3">
                        <h4 class="section-title mt-0">Justification</h4>
                        <asp:TextBox runat="server" ID="txtJustification" CssClass="form-control txtreadonly" TextMode="MultiLine" Rows="4" ReadOnly="true" />
                    </div>
                </div>

                <div class="row">
                    <div class="col-md-6">
                        <asp:Panel runat="server" ID="pnlUploadedDocument" CssClass="card bg-light border-success mb-3 h-100" Visible="false">
                            <div class="card-body">
                                <h5 class="card-title text-success"><i class="fas fa-paperclip"></i> Uploaded Documents</h5>
                                <div class="bg-white p-2 rounded border">
                                    <asp:PlaceHolder ID="phDocumentList" runat="server" />
                                </div>
                            </div>
                        </asp:Panel>
                    </div>
                    
                    <div class="col-md-6">
                        <asp:Panel runat="server" ID="pnHistoryApproval" CssClass="card bg-light border-info mb-3 h-100" Visible="false">
                            <div class="card-body p-2">
                                <h5 class="card-title text-info mb-2 px-2"><i class="fas fa-history"></i> Approval History</h5>
                                <div class="table-responsive bg-white">
                                    <asp:GridView ID="gvHistory" runat="server" CssClass="table table-bordered table-sm mb-0" AutoGenerateColumns="False" EmptyDataText="No Approval record.">
                                        <Columns>
                                            <asp:BoundField DataField="ActionDate" HeaderText="Date" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                                            <asp:BoundField DataField="ActionType" HeaderText="Action" />
                                            <asp:BoundField DataField="RoleName" HeaderText="Role" /> 
                                            <asp:BoundField DataField="Remarks" HeaderText="Remarks"/>
                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </div>
                        </asp:Panel>
                    </div>
                </div>

                <div class="card mt-4 border-warning shadow-sm">
                    <div class="card-header bg-warning text-dark font-weight-bold">
                        <i class="fas fa-exclamation-triangle"></i> Resubmission Action Required
                    </div>
                    <div class="card-body">
                        <div class="row">
                            <div class="col-md-8">
                                <label class="font-weight-bold">Remarks for Resubmission <span class="text-danger">*</span></label>
                                <asp:TextBox runat="server" ID="txtResubmit" CssClass="form-control" TextMode="MultiLine" Rows="3" placeholder="Please enter remarks regarding the changes or justification for resubmit..."/>
                            </div>
                            <div class="col-md-4">
                                <label class="font-weight-bold">Upload Additional Document <span class="text-muted small">(Optional)</span></label>
                                <div class="custom-file">
                                    <asp:FileUpload ID="fuDocument" runat="server" CssClass="custom-file-input" onchange="$(this).next('.custom-file-label').html($(this).val().split('\\').pop());" />
                                    <label class="custom-file-label" for="fuDocument">Choose file...</label>
                                </div>
                                <small class="form-text text-muted mt-2">Uploading a new file will add it to the existing document list.</small>
                            </div>
                        </div>
                    </div>
                </div>

                <asp:HiddenField ID="hdncurentRoleApprover" runat="server" />

            </div>
        </asp:Panel>
    </div>

</asp:Content>