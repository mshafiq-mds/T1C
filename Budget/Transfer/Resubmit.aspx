<%@ Page Title="Resubmit Budget Transfer" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Resubmit.aspx.cs" Inherits="Prodata.WebForm.Budget.Transfer.Resubmit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>

    <script type="text/javascript">
        // Formatting function for currency fields (Read-only view mainly)
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

        // Main Logic: Validate inputs, Show SweetAlert, Trigger Server Click
        function confirmSubmit(e) {
            // Prevent default button behavior
            e.preventDefault();

            // 1. Get Values
            var remarksControl = document.getElementById('<%= txtResubmit.ClientID %>');
            var fileUploadControl = document.getElementById('<%= fuDocument.ClientID %>');
            
            var remarks = remarksControl ? remarksControl.value : "";
            var hasFile = fileUploadControl && fileUploadControl.files.length > 0;
            var fileName = hasFile ? fileUploadControl.files[0].name : "No new file selected (Previous file will be kept)";

            // 2. Validation: Remarks are MANDATORY for resubmission
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

            // 3. Build Summary HTML for SweetAlert
            var summaryHtml = `
                <div class="text-left" style="font-size: 0.95rem;">
                    <p><strong>Remarks:</strong><br/> <i class="text-muted">${remarks}</i></p>
                    <p><strong>Document:</strong><br/> <span class="text-primary">${fileName}</span></p>
                </div>
                <hr/>
                <p>Are you sure you want to proceed?</p>
            `;

            // 4. Show SweetAlert
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
                    // 5. Show Preloader
                    $("#pagePreloader").fadeIn(200);
                    
                    // 6. Trigger the HIDDEN ASP.NET Button to handle PostBack
                    document.getElementById('<%= btnConfirmSubmit.ClientID %>').click();
                }
            });

            return false;
        }
    </script>

    <style>
        .fixed-dropdown { width: 300px !important; }
        
        /* Preloader Styles */
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
        .page-preloader img { animation: shake 1.5s infinite; }
        @keyframes shake {
            0% { transform: rotate(0deg); }
            25% { transform: rotate(3deg); }
            50% { transform: rotate(0deg); }
            75% { transform: rotate(-3deg); }
            100% { transform: rotate(0deg); }
        }
        
        .txtreadonly { background-color: #e9ecef; cursor: not-allowed; }
    </style>

    <div id="pagePreloader" class="page-preloader" style="display:none;">
        <img src="<%= ResolveUrl("~/Images/Felda_Global_Ventures_Logo.png") %>" alt="Loading..." height="200" width="200" />
        <p class="mt-3 text-white">Processing...</p>
    </div>

    <asp:Panel runat="server" CssClass="card p-4 shadow-sm">
        <div class="card-header card-header-sticky">
            <h2 class="card-title d-none d-sm-inline"><%: Page.Title %></h2>
            <div class="card-tools">
                <asp:LinkButton ID="btnBack" runat="server" CssClass="btn btn-default" PostBackUrl="~/Budget/Transfer/Default" CausesValidation="false">
                    <i class="fas fa-angle-double-left"></i> Back
                </asp:LinkButton>
                
                <button id="btnTriggerSweetAlert" class="btn btn-success" onclick="return confirmSubmit(event);">
                    <i class="fas fa-share"></i> Submit Application
                </button>

                <asp:Button ID="btnConfirmSubmit" runat="server" OnClick="btnSubmit_Click" style="display:none;" />
            </div>
        </div>

        <div class="form-group mt-3" style="font-size: 1.2rem;">
            <label>BA : </label>
            <asp:Label runat="server" ID="LblBA" CssClass="font-weight-bold" />
        </div>

        <div class="form-group">
            <label class="d-block mb-2 font-weight-bold">BUDGET TYPE</label>
            <div class="btn-group btn-group-toggle">
                <label class="btn btn-outline-primary disabled">
                    <asp:RadioButton runat="server" ID="rdoOpex" GroupName="BudgetType" CssClass="d-none" Enabled="false" />
                    <span>OPEX</span>
                </label>
                <label class="btn btn-outline-primary ml-2 disabled">
                    <asp:RadioButton runat="server" ID="rdoCapex" GroupName="BudgetType" CssClass="d-none" Enabled="false"/>
                    <span>CAPEX</span>
                </label>
            </div>
        </div>

        <div class="form-group">
            <label>Project</label>
            <asp:TextBox runat="server" ID="txtProject" CssClass="form-control txtreadonly" ReadOnly="true" />
        </div>

        <div class="form-group">
            <label>Reference No.</label>
            <asp:TextBox runat="server" ID="txtRefNo" CssClass="form-control txtreadonly" ReadOnly="true" />
        </div>

        <div class="form-group">
            <label>Application Date</label>
            <asp:TextBox runat="server" ID="txtDate" CssClass="form-control txtreadonly" ReadOnly="true" />
        </div>

        <div class="form-group">
            <label>Estimated Cost (RM)</label>
            <asp:TextBox runat="server" ID="txtEstimatedCost" CssClass="form-control txtreadonly" ReadOnly="true" />
        </div>

        <div class="form-group">
            <label>E-VISA No.</label>
            <asp:TextBox runat="server" ID="txtEVisa" CssClass="form-control txtreadonly" ReadOnly="true" />
        </div>

        <div class="form-group">
            <label>Work Details</label>
            <asp:TextBox runat="server" ID="txtWorkDetails" CssClass="form-control txtreadonly" TextMode="MultiLine" Rows="3" ReadOnly="true" />
        </div>

        <h4 class="mt-4">Budget Details</h4>
        <div class="table-responsive">
            <table class="table table-bordered table-sm">
                <thead class="thead-light">
                    <tr>
                        <th>Item</th>
                        <th>GL</th>
                        <th>Asset Class</th>
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
                        <td><asp:Label runat="server" ID="txtFromGL" /></td>
                        <td><asp:Label runat="server" ID="txtFromBudgetType" /></td>
                        <td><asp:Label runat="server" ID="ddFromBA" /></td>
                        <td><asp:Label runat="server" ID="txtFromBudget" /></td>
                        <td><asp:Label runat="server" ID="txtFromBalance" /></td>
                        <td><asp:Label runat="server" ID="txtFromTransfer" CssClass="font-weight-bold" /></td>
                        <td><asp:Label runat="server" ID="txtFromAfter" /></td>
                    </tr>
                    <tr>
                        <td>To Budget</td>
                        <td><asp:Label runat="server" ID="txtToGL" /></td>
                        <td><asp:Label runat="server" ID="txtToBudgetType" /></td>
                        <td><asp:Label runat="server" ID="ddToBA" /></td>
                        <td><asp:Label runat="server" ID="txtToBudget" /></td>
                        <td><asp:Label runat="server" ID="txtToBalance" /></td>
                        <td><asp:Label runat="server" ID="txtToTransfer" CssClass="font-weight-bold" /></td>
                        <td><asp:Label runat="server" ID="txtToAfter" /></td>
                    </tr>
                </tbody>
            </table>
        </div>
        
        <h4 class="mt-4">Justification</h4>
        <asp:TextBox runat="server" ID="txtJustification" CssClass="form-control txtreadonly" TextMode="MultiLine" Rows="5" ReadOnly="true" />

        <div class="card bg-light mt-4 border-warning">
            <div class="card-body">
                <h4 class="card-title text-warning font-weight-bold"><i class="fas fa-edit"></i> Resubmission Details</h4>
                <br /><hr />
                
                <div class="form-group">
                    <label>Remark Resubmit <span class="text-danger">*</span></label>
                    <asp:TextBox runat="server" ID="txtResubmit" CssClass="form-control border-warning" TextMode="MultiLine" Rows="5" Enabled="true" placeholder="Please enter remarks regarding the changes made..."/>
                </div>

                <div class="form-group">
                    <label>Upload Supporting Document <span class="text-muted small">(Optional)</span></label>
                    <div class="custom-file">
                         <asp:FileUpload ID="fuDocument" runat="server" CssClass="form-control-file" />
                    </div>
                    <small class="form-text text-muted">Uploading a new file will add to the document list.</small>
                </div>
            </div>
        </div>

    </asp:Panel>
    
    <asp:HiddenField ID="hdncurentRoleApprover" runat="server" />

    <asp:Panel runat="server" ID="pnlUploadedDocument" CssClass="card mt-3 p-3" Visible="false">
        <h5 class="card-title"><i class="fas fa-paperclip"></i> Uploaded Documents</h5>
        <div class="border p-3 rounded bg-white">
            <asp:PlaceHolder ID="phDocumentList" runat="server" />
        </div>
    </asp:Panel>

    <asp:Panel runat="server" ID="pnHistoryApproval" CssClass="card mt-3 p-3" Visible="false">
        <h5 class="card-title"><i class="fas fa-history"></i> Approval History</h5>
        <div class="table-responsive">
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <asp:GridView ID="gvHistory" runat="server"
                                  CssClass="table table-bordered table-sm table-striped" 
                                  AllowPaging="true"
                                  AutoGenerateColumns="False"
                                  DataKeyNames="Id"
                                  EmptyDataText="No Approval record.">
                        <Columns>
                            <asp:BoundField DataField="ActionDate" HeaderText="Action Date" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                            <asp:BoundField DataField="ActionType" HeaderText="Action" />
                            <asp:BoundField DataField="RoleName" HeaderText="Role" /> 
                            <asp:BoundField DataField="Status" HeaderText="Status"/>
                            <asp:BoundField DataField="Remarks" HeaderText="Remarks"/>
                        </Columns>
                    </asp:GridView>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </asp:Panel>

</asp:Content>