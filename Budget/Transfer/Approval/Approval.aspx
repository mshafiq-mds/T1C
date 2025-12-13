<%@ Page Title="Transfer Approval Details" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Approval.aspx.cs" Inherits="Prodata.WebForm.Budget.Transfer.Approval.Approval" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Panel runat="server" CssClass="card p-4 shadow-sm rounded">

        <div class="card-header card-header-sticky">
            <h2 class="card-title d-none d-sm-inline"><%: Page.Title %></h2>
            <div class="card-tools">
                <asp:LinkButton ID="btnBack" runat="server" CssClass="btn btn-default" PostBackUrl="~/Budget/Transfer/Approval/Default" CausesValidation="false">
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

        <div class="row mt-4">
            <div class="col-md-6 mb-3">
                <label class="fw-bold">Reference No:</label>
                <div><asp:Label ID="lblRefNo" runat="server" /></div>
            </div>
            <div class="col-md-6 mb-3">
                <label class="fw-bold">Application Date:</label>
                <div><asp:Label ID="lblDate" runat="server" /></div>
            </div>
            <div class="col-md-6 mb-3">
                <label class="fw-bold">Project:</label>
                <div><asp:Label ID="lblProject" runat="server" /></div>
            </div>
            <div class="col-md-6 mb-3">
                <label class="fw-bold">BA:</label>
                <div><asp:Label ID="lblBA" runat="server" /></div>
            </div>
            <div class="col-md-6 mb-3">
                <label class="fw-bold">Budget Type:</label>
                <div><asp:Label ID="lblBudgetType" runat="server" /></div>
            </div>
            <div class="col-md-6 mb-3">
                <label class="fw-bold">Estimated Cost (RM):</label>
                <div><asp:Label ID="lblEstimatedCost" runat="server" /></div>
            </div>
            <div class="col-md-6 mb-3">
                <label class="fw-bold">E-VISA No.:</label>
                <div><asp:Label ID="lblEVisa" runat="server" /></div>
            </div>
        </div>

        <h4 class="mt-4">Budget Transfer Details</h4>
        <table class="table table-striped table-bordered table-hover">
            <thead class="table-primary text-center">
                <tr>
                    <th>Item</th>
                    <th>GL</th>
                    <th>Asset Class</th>
                    <th>BA</th>
                    <th>Original Budget (RM)</th>
                    <th>Current Balance (RM)</th>
                    <th>Transfer Amount (RM)</th>
                    <th>Balance After Transfer (RM)</th>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td>From Budget</td>
                    <td><asp:Label ID="lblFromGL" runat="server" /></td>
                    <td><asp:Label ID="lblFromBudgetType" runat="server" /></td>
                    <td><asp:Label ID="lblFromBA" runat="server" /></td>
                    <td><asp:Label ID="lblFromBudget" runat="server" /></td>
                    <td><asp:Label ID="lblFromBalance" runat="server" /></td>
                    <td><asp:Label ID="lblFromTransfer" runat="server" /></td>
                    <td><asp:Label ID="lblFromAfter" runat="server" /></td>
                </tr>
                <tr>
                    <td>To Budget</td>
                    <td><asp:Label ID="lblToGL" runat="server" /></td>
                    <td><asp:Label ID="lblToBudgetType" runat="server" /></td>
                    <td><asp:Label ID="lblToBA" runat="server" /></td>
                    <td><asp:Label ID="lblToBudget" runat="server" /></td>
                    <td><asp:Label ID="lblToBalance" runat="server" /></td>
                    <td><asp:Label ID="lblToTransfer" runat="server" /></td>
                    <td><asp:Label ID="lblToAfter" runat="server" /></td>
                </tr>
            </tbody>
        </table>

        <h4 class="mt-4">Justification</h4>
        <div class="border rounded p-3 bg-light">
            <asp:Literal ID="litJustification" runat="server" Mode="Encode" />
        </div>

        <h4 class="mt-4">Uploaded Document</h4>
        <asp:Panel runat="server" ID="pnlUploadedDocument" CssClass="form-group" Visible="false">
            <asp:PlaceHolder ID="phDocumentList" runat="server" />
        </asp:Panel>

        <asp:HiddenField ID="hdnAction" runat="server" />
        <asp:HiddenField ID="hdnRemarks" runat="server" /> 
        
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

    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <script type="text/javascript">
        $(document).ready(function () {

            // Request Revision Logic
            $(".btn-revision").click(function (e) {
                e.preventDefault();
                Swal.fire({
                    title: 'Request Resubmit?',
                    text: "Please provide remarks for the revision request.",
                    icon: 'warning',
                    input: 'textarea', // Add textarea input
                    inputPlaceholder: 'Enter your remarks here...',
                    inputAttributes: {
                        'aria-label': 'Type your remarks here'
                    },
                    showCancelButton: true,
                    confirmButtonColor: '#ffc107', // Warning color
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
                        // Store value in HiddenField
                        $('#<%= hdnRemarks.ClientID %>').val(result.value);
                        // Set Action
                        $('#<%= hdnAction.ClientID %>').val('resubmit');
                        // Trigger Server Click
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
                    input: 'textarea', // Add textarea input
                    inputPlaceholder: 'Enter remarks here...',
                    showCancelButton: true,
                    confirmButtonColor: '#28a745', // Success color
                    cancelButtonColor: '#6c757d',
                    confirmButtonText: 'Yes, approve it',
                    preConfirm: (remarks) => {
                         // Remove this block if remarks are optional for approval
                        if (!remarks) {
                             Swal.showValidationMessage('Remarks are required');
                        }
                        return remarks;
                    }
                }).then((result) => {
                    if (result.isConfirmed) {
                        // Store value in HiddenField
                        $('#<%= hdnRemarks.ClientID %>').val(result.value);
                        // Set Action
                        $('#<%= hdnAction.ClientID %>').val('approve');
                        // Trigger Server Click
                        $('#<%= btnApproveConfirmed.ClientID %>').click();
                    }
                });
            });

        });
    </script>

</asp:Content>