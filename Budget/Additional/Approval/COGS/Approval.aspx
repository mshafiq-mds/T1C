<%@ Page Title="Additional Approval COGS Details" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Approval.aspx.cs" Inherits="Prodata.WebForm.Budget.Additional.Approval.COGS.Approval" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Panel runat="server" CssClass="card p-4">

         <!-- Header Bar -->
        <div class="card-header card-header-sticky">
            <h2 class="card-title d-none d-sm-inline"><%: Page.Title %></h2>
            <div class="card-tools">
                <asp:LinkButton ID="btnBack" runat="server" CssClass="btn btn-default" PostBackUrl="/Budget/Additional/Approval/COGS/Default" CausesValidation="false">
                    <i class="fas fa-angle-double-left"></i> Back
                </asp:LinkButton>
                <asp:LinkButton ID="btnSave" runat="server" CssClass="btn btn-warning me-2 btn-revision" OnClick="btnSave_Click">
                    <i class="fas fa-edit"></i> Request Revision
                </asp:LinkButton>
                <asp:LinkButton ID="btnSubmit1" runat="server" CssClass="btn btn-success btn-approve" OnClick="btnSubmit_Click">
                    <i class="fas fa-circle"></i> Approve Transfer
                </asp:LinkButton>
            </div>
        </div>

       <div class="form-group d-flex align-items-center" style="font-size: 2rem;">
            <label class="me-2 mb-0 fw-semibold text-dark">BA :</label>
            <asp:Label ID="LblBA" runat="server" CssClass="fw-bold text-dark me-1" Style="font-size: 2rem;" />
            <span class="fw-bold text-dark" style="font-size: 2rem;">(</span>
            <asp:Label ID="LblBAName" runat="server" CssClass="fw-bold text-dark" Style="font-size: 2rem;" />
            <span class="fw-bold text-dark" style="font-size: 2rem;">)</span>
        </div>

        <!-- Application Info -->
        <div class="row mb-3">
            <div class="col-md-6">
                <label class="form-label">Budget Type</label>
                <div class="form-control-plaintext fw-bold text-dark">
                    <asp:Label runat="server" ID="lblBudgetType" />
                </div>
            </div>
            <div class="col-md-6">
                <label class="form-label">Project / Department</label>
                <div class="form-control-plaintext fw-bold text-dark">
                    <asp:Label ID="lblProject" runat="server" />
                </div>
            </div>
        </div>

        <div class="row mb-3">
            <div class="col-md-6">
                <label class="form-label">Reference No.</label>
                <div class="form-control-plaintext fw-bold text-dark">
                    <asp:Label ID="lblRefNo" runat="server" />
                </div>
            </div>
            <div class="col-md-6">
                <label class="form-label">Application Date</label>
                <div class="form-control-plaintext fw-bold text-dark">
                    <asp:Label ID="lblDate" runat="server" />
                </div>
            </div>
        </div>

        <div class="row mb-3">
            <div class="col-md-6">
                <label class="form-label">Estimated Cost (RM)</label>
                <div class="form-control-plaintext fw-bold text-primary">
                    RM <asp:Label ID="lblBudgetEstimate" runat="server" />
                </div>
            </div>
            <div class="col-md-6">
                <label class="form-label">E-VISA No.</label>
                <div class="form-control-plaintext fw-bold text-dark">
                    <asp:Label ID="lblEVisa" runat="server" />
                </div>
            </div>
        </div>
        
         <div class="mb-4">
            <label class="form-label">Type LOA</label>
            <div class="form-control-plaintext fw-bold text-dark">
                <asp:Label ID="lblCheckType" runat="server" />
            </div>
        </div>

        <!-- Justification -->
         <div class="mb-4">
            <label class="form-label">Application Details</label>
            <div class="form-control-plaintext text-dark">
                <asp:Label ID="lblRequestDetails" runat="server" />
            </div>
        </div>

        <div class="mb-4">
            <label class="form-label">Reason for Application</label>
            <div class="form-control-plaintext text-dark">
                <asp:Label ID="lblReason" runat="server" />
            </div>
        </div>

        <!-- Budget Breakdown -->
        <h4 class="mb-3 border-bottom pb-1">Budget Allocation Breakdown</h4>
        <table class="table table-bordered table-sm text-center align-middle">
            <thead class="table-light">
                <tr>
                    <th>Cost Centre</th>
                    <th>GL Code</th>
                    <th>Approved Budget 2022 (RM)</th>
                    <th>New Budget 2022 (RM)</th>
                    <th>Additional Budget (RM)</th>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td><asp:Label ID="lblCostCentre" runat="server" /></td>
                    <td><asp:Label ID="lblGL" runat="server" /></td>
                    <td class="text-primary"><asp:Label ID="lblApprovedBudget" runat="server" /></td>
                    <td class="text-primary"><asp:Label ID="lblNewTotalBudget" runat="server" /></td>
                    <td class="text-primary fw-bold"><asp:Label ID="lblAdditionalBudget" runat="server" /></td>
                </tr>
            </tbody>
        </table>

        <!-- Uploaded Documents -->
        <h4 class="mt-4">Uploaded Document</h4>
        <asp:Panel runat="server" ID="pnlUploadedDocument" CssClass="form-group" Visible="false">
            <asp:PlaceHolder ID="phDocumentList" runat="server" />
        </asp:Panel>

        <h4 class="mt-4">Remarks</h4>
        <asp:TextBox runat="server" ID="txtRemarks" CssClass="form-control" TextMode="MultiLine" Rows="10" />
        <asp:RequiredFieldValidator ID="rfvtxtRemarks" runat="server" ControlToValidate="txtRemarks" CssClass="text-danger" ErrorMessage="Required Remarks" Display="Dynamic" />


        <!-- Hidden fields and buttons for sweet alert postback -->
        <asp:HiddenField ID="hdnAction" runat="server" />
        <asp:Button ID="btnRevisionConfirmed" runat="server" OnClick="btnSave_Click" Style="display:none;" />
        <asp:Button ID="btnApproveConfirmed" runat="server" OnClick="btnSubmit_Click" Style="display:none;" />
        <asp:HiddenField ID="hdnTransferId" runat="server" />

        
        <h4 class="mt-4">Approval History</h4>
         <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <asp:GridView ID="gvHistory" runat="server"
                              CssClass="table table-bordered table-sm" 
                              AllowPaging="true"
                              AutoGenerateColumns="False"
                              DataKeyNames="Id"
                              EmptyDataText="No record.">
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
    
    <!-- SweetAlert2 Confirmation Scripts -->
    
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <script type="text/javascript">
        $(document).ready(function () {

            // Request Revision
            $(".btn-revision").click(function (e) {
                e.preventDefault();
                Swal.fire({
                    title: 'Request Resubmit?',
                    text: "Do you want to ask the requester to revise or reupload the document?",
                    icon: 'warning',
                    showCancelButton: true,
                    confirmButtonColor: '#007bff',
                    cancelButtonColor: '#6c757d',
                    confirmButtonText: 'Yes, request it'
                }).then((result) => {
                    if (result.isConfirmed) {
                        $('#<%= hdnAction.ClientID %>').val('resubmit');
                        $('#<%= btnRevisionConfirmed.ClientID %>').click();
                    }
                });
            });

            // Approve Transfer
            $(".btn-approve").click(function (e) {
                e.preventDefault();
                Swal.fire({
                    title: 'Approve Transfer?',
                    text: "Are you sure you want to approve this budget transfer?",
                    icon: 'success',
                    showCancelButton: true,
                    confirmButtonColor: '#28a745',
                    cancelButtonColor: '#6c757d',
                    confirmButtonText: 'Yes, approve it'
                }).then((result) => {
                    if (result.isConfirmed) {
                        $('#<%= hdnAction.ClientID %>').val('approve');
                        $('#<%= btnApproveConfirmed.ClientID %>').click();
                    }
                });
            });

        });
    </script>

</asp:Content>