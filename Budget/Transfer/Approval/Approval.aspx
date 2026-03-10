<%@ Page Title="Transfer Approval Details" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Approval.aspx.cs" Inherits="Prodata.WebForm.Budget.Transfer.Approval.Approval" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>

    <style>
        .full-screen-container { width: 100%; padding: 0 15px; }
        .card-custom { border: none; box-shadow: 0 4px 15px rgba(0,0,0,0.08); border-radius: 10px; margin-bottom: 30px; background-color: #fff; }
        .card-header-custom { background-color: #f8f9fa; border-bottom: 2px solid #e9ecef; border-radius: 10px 10px 0 0 !important; padding: 1.25rem 1.5rem; position: sticky; top: 0; z-index: 1000; }
        .section-title { color: #2c3e50; font-weight: 600; border-bottom: 2px solid #3498db; padding-bottom: 8px; margin-bottom: 20px; margin-top: 30px; font-size: 1.25rem; }
        .form-label { font-weight: 600; color: #6c757d; font-size: 0.9rem; text-transform: uppercase; letter-spacing: 0.5px; margin-bottom: 2px; }
        .view-data { font-size: 1.1rem; font-weight: 500; color: #212529; margin-bottom: 15px; display: block; }
        .table-custom th { background-color: #343a40 !important; color: #ffffff !important; font-weight: 500; vertical-align: middle !important; text-align: center; }
        .table-custom td { vertical-align: middle; }
        .min-w-150 { min-width: 150px; }
        .min-w-200 { min-width: 200px; }
        
        /* Preloader */
        .page-preloader { position: fixed; z-index: 99999; top: 0; left: 0; width: 100%; height: 100%; background: rgba(0,0,0,0.7); display: flex; justify-content: center; align-items: center; flex-direction: column; }
        .page-preloader img { animation: pulse 1.5s infinite; }
        @keyframes pulse { 0% { transform: scale(0.95); opacity: 0.8; } 50% { transform: scale(1.05); opacity: 1; } 100% { transform: scale(0.95); opacity: 0.8; } }
    </style>

    <div id="pagePreloader" class="page-preloader" style="display:none;">
        <img src="<%= ResolveUrl("~/Images/Felda_Global_Ventures_Logo.png") %>" alt="Loading..." height="150" width="150" />
        <h4 class="mt-3 text-white font-weight-bold">Processing...</h4>
    </div>

    <div class="full-screen-container">
        <asp:Panel runat="server" CssClass="card card-custom">

            <div class="card-header card-header-custom d-flex align-items-center">
                <h3 class="card-title m-0 fw-bold text-dark"><i class="fas fa-clipboard-check mr-2"></i><%: Page.Title %></h3>
                <div class="card-tools" style="margin-left: auto !important;">
                    <asp:LinkButton ID="btnBack" runat="server" CssClass="btn btn-outline-secondary mr-2" PostBackUrl="~/Budget/Transfer/Approval/Default" CausesValidation="false">
                        <i class="fas fa-angle-double-left"></i> Back
                    </asp:LinkButton>
                    <asp:LinkButton ID="btnSave" runat="server" CssClass="btn btn-warning mr-2 btn-revision" OnClick="btnSave_Click" OnClientClick="return false;">
                        <i class="fas fa-edit"></i> Request Revision
                    </asp:LinkButton>
                    <asp:LinkButton ID="btnSubmit1" runat="server" CssClass="btn btn-success btn-approve" OnClick="btnSubmit_Click" OnClientClick="return false;">
                        <i class="fas fa-check-circle"></i> Approve Transfer
                    </asp:LinkButton>
                    
                    <asp:HiddenField ID="hdnAction" runat="server" />
                    <asp:HiddenField ID="hdnRemarks" runat="server" /> 
                    <asp:HiddenField ID="hdnTransferId" runat="server" />
                    <asp:Button ID="btnRevisionConfirmed" runat="server" OnClick="btnSave_Click" Style="display:none;" />
                    <asp:Button ID="btnApproveConfirmed" runat="server" OnClick="btnSubmit_Click" Style="display:none;" />
                </div>
            </div>

            <div class="card-body p-4">
                
                <h4 class="section-title mt-0">Application Information</h4>
                
                <div class="row">
                    <div class="col-md-3">
                        <label class="form-label">Reference No.</label>
                        <asp:Label ID="lblRefNo" runat="server" CssClass="view-data text-primary font-weight-bold" />
                    </div>
                    <div class="col-md-3">
                        <label class="form-label">E-VISA No.</label>
                        <asp:Label ID="lblEVisa" runat="server" CssClass="view-data text-info font-weight-bold" />
                    </div>
                    <div class="col-md-3">
                        <label class="form-label">Application Date</label>
                        <asp:Label ID="lblDate" runat="server" CssClass="view-data" />
                    </div>
                    <div class="col-md-3">
                        <label class="form-label">Budget Type</label>
                        <asp:Label ID="lblBudgetType" runat="server" CssClass="view-data" />
                    </div>
                </div>

                <div class="row">
                    <div class="col-md-9">
                        <label class="form-label">Project Name</label>
                        <asp:Label ID="lblProject" runat="server" CssClass="view-data" />
                    </div>
                    <div class="col-md-3">
                        <label class="form-label">Total Estimated Cost</label>
                        <asp:Label ID="lblEstimatedCost" runat="server" CssClass="view-data font-weight-bold text-success" />
                    </div>
                </div>

                <h4 class="section-title">Budget Flow Details</h4>
                
                <div class="row mb-3">
                    <div class="col-md-6">
                        <label class="form-label text-danger"><i class="fas fa-sign-out-alt mr-1"></i> Source Business Area (FROM)</label>
                        <div class="form-control bg-light font-weight-bold text-dark shadow-sm border-danger" style="border-left: 4px solid #dc3545; height: auto;">
                            <asp:Label runat="server" ID="lblGlobalFromBA" />
                        </div>
                    </div>
                    <div class="col-md-6">
                        <label class="form-label text-success"><i class="fas fa-sign-in-alt mr-1"></i> Destination Business Area (TO)</label>
                        <div class="form-control bg-light font-weight-bold text-dark shadow-sm border-success" style="border-left: 4px solid #28a745; height: auto;">
                            <asp:Label runat="server" ID="lblGlobalToBA" />
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
                                <th class="min-w-150 text-right">Original (RM)</th>
                                <th class="min-w-150 text-right">Current Bal (RM)</th>
                                <th class="min-w-150 text-right">Transfer (RM)</th>
                                <th class="min-w-150 text-right">Bal After (RM)</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rptFromBudgets" runat="server">
                                <ItemTemplate>
                                    <tr class="bg-light">
                                        <td class="font-weight-bold text-danger text-center align-middle"><i class="fas fa-arrow-up mr-1"></i> FROM</td>
                                        <td class="align-middle"><%# Eval("FromGL") %></td>
                                        <td class="align-middle"><%# GetBudgetTypeName(Eval("FromBudgetType")) %></td>
                                        <td class="align-middle text-right"><%# Eval("FromBudget", "{0:N2}") %></td>
                                        <td class="align-middle text-right"><%# Eval("FromBalance", "{0:N2}") %></td>
                                        <td class="align-middle text-right font-weight-bold text-danger"><%# Eval("FromTransfer", "{0:N2}") %></td>
                                        <td class="align-middle text-right"><%# Eval("FromAfter", "{0:N2}") %></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>

                            <tr style="background-color: #e8f4f8; border-top: 3px solid #dee2e6;">
                                <td class="font-weight-bold text-success text-center align-middle"><i class="fas fa-arrow-down mr-1"></i> TO</td>
                                <td class="align-middle"><asp:Label ID="lblToGL" runat="server" /></td>
                                <td class="align-middle"><asp:Label ID="lblToBudgetType" runat="server" /></td>
                                <td class="align-middle text-right"><asp:Label ID="lblToBudget" runat="server" /></td>
                                <td class="align-middle text-right"><asp:Label ID="lblToBalance" runat="server" /></td>
                                <td class="align-middle text-right font-weight-bold text-success"><asp:Label ID="lblToTransfer" runat="server" /></td>
                                <td class="align-middle text-right font-weight-bold"><asp:Label ID="lblToAfter" runat="server" /></td>
                            </tr>
                        </tbody>
                    </table>
                </div>

                <div class="row">
                    <div class="col-md-12 mb-4">
                        <h4 class="section-title mt-0">Justification</h4>
                        <div class="border rounded p-3 bg-light view-data" style="min-height: 80px;">
                            <asp:Literal ID="litJustification" runat="server" Mode="Encode" />
                        </div>
                    </div>
                </div>

                <div class="row">
                    <div class="col-md-6 mb-3">
                        <asp:Panel runat="server" ID="pnlUploadedDocument" CssClass="card bg-light border-success h-100" Visible="false">
                            <div class="card-body">
                                <h5 class="card-title text-success"><i class="fas fa-paperclip"></i> Uploaded Documents</h5>
                                <div class="bg-white p-2 rounded border">
                                    <asp:PlaceHolder ID="phDocumentList" runat="server" />
                                </div>
                            </div>
                        </asp:Panel>
                    </div>
                    
                    <div class="col-md-6 mb-3">
                        <asp:Panel runat="server" ID="pnHistoryApproval" CssClass="card bg-light border-info h-100" Visible="false">
                            <div class="card-body p-2">
                                <h5 class="card-title text-info mb-2 px-2"><i class="fas fa-history"></i> Approval History</h5>
                                <div class="table-responsive bg-white">
                                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                        <ContentTemplate>
                                            <asp:GridView ID="gvHistory" runat="server" CssClass="table table-bordered table-sm mb-0" AllowPaging="true" AutoGenerateColumns="False" DataKeyNames="Id" EmptyDataText="No Approval record.">
                                                <Columns>
                                                    <asp:BoundField DataField="ActionDate" HeaderText="Date" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                                                    <asp:BoundField DataField="ActionType" HeaderText="Action" />
                                                    <asp:BoundField DataField="RoleName" HeaderText="Role" /> 
                                                    <asp:BoundField DataField="Status" HeaderText="Status"/>
                                                    <asp:BoundField DataField="Remarks" HeaderText="Remarks"/>
                                                </Columns>
                                            </asp:GridView>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </div>
                            </div>
                        </asp:Panel>
                    </div>
                </div>

            </div>
        </asp:Panel>
    </div>

    <script type="text/javascript">
        $(document).ready(function () {

            // Request Revision Logic
            $(".btn-revision").click(function (e) {
                e.preventDefault();
                Swal.fire({
                    title: 'Request Resubmit?',
                    text: "Please provide remarks for the revision request.",
                    icon: 'warning',
                    input: 'textarea',
                    inputPlaceholder: 'Enter your remarks here...',
                    inputAttributes: {
                        'aria-label': 'Type your remarks here'
                    },
                    showCancelButton: true,
                    confirmButtonColor: '#ffc107',
                    cancelButtonColor: '#6c757d',
                    confirmButtonText: 'Yes, request revision',
                    preConfirm: (remarks) => {
                        if (!remarks) {
                            Swal.showValidationMessage('Remarks are required for revision requests');
                        }
                        return remarks;
                    }
                }).then((result) => {
                    if (result.isConfirmed) {
                        $('#<%= hdnRemarks.ClientID %>').val(result.value);
                        $('#<%= hdnAction.ClientID %>').val('resubmit');
                        $("#pagePreloader").fadeIn(200);
                        $('#<%= btnRevisionConfirmed.ClientID %>').click();
                    }
                });
            });

            // Approve Transfer Logic
            $(".btn-approve").click(function (e) {
                e.preventDefault();
                Swal.fire({
                    title: 'Approve Transfer?',
                    text: "Please provide remarks (optional) and confirm approval.",
                    icon: 'success',
                    input: 'textarea',
                    inputPlaceholder: 'Enter remarks here...',
                    showCancelButton: true,
                    confirmButtonColor: '#28a745',
                    cancelButtonColor: '#6c757d',
                    confirmButtonText: 'Yes, approve it'
                }).then((result) => {
                    if (result.isConfirmed) {
                        $('#<%= hdnRemarks.ClientID %>').val(result.value || "Approved");
                        $('#<%= hdnAction.ClientID %>').val('approve');
                        $("#pagePreloader").fadeIn(200);
                        $('#<%= btnApproveConfirmed.ClientID %>').click();
                    }
                });
            });

        });
    </script>
</asp:Content>