<%@ Page Title="Edit AWO Approver" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Prodata.WebForm.MasterData.AOW.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>

    <style>
        .full-screen-container { width: 100%; padding: 0 15px; }
        .card-custom { border: none; box-shadow: 0 4px 15px rgba(0,0,0,0.08); border-radius: 10px; margin-bottom: 30px; }
        .card-header-custom { background-color: #f8f9fa; border-bottom: 2px solid #e9ecef; border-radius: 10px 10px 0 0 !important; padding: 1.25rem 1.5rem; }
        .form-label { font-weight: 600; color: #495057; }
        .asterisk { color: red; }
    </style>

    <script type="text/javascript">
        // Client-side validation using SweetAlert before submitting
        function validateForm() {
            var minAmt = document.getElementById('<%= txtMinAmount.ClientID %>').value.trim();
            var level = document.getElementById('<%= txtLevel.ClientID %>').value.trim();
            var role = document.getElementById('<%= ddlRoleCode.ClientID %>').value;

            if (minAmt === "" || level === "" || role === "") {
                Swal.fire({
                    icon: 'warning',
                    title: 'Validation Error',
                    text: 'Please fill in all required fields marked with an asterisk (*).'
                });
                return false;
            }
            return true;
        }
    </script>

    <div class="full-screen-container row justify-content-center">
        <div class="col-md-8">
            <div class="card card-custom">
                
                <div class="card-header card-header-custom d-flex align-items-center">
                    <h3 class="card-title m-0 fw-bold text-dark">
                        <i class="fas fa-edit mr-2 text-primary"></i><%: Page.Title %>
                    </h3>
                    <div class="card-tools" style="margin-left: auto !important;">
                        <asp:LinkButton ID="btnBack" runat="server" CssClass="btn btn-outline-secondary mr-2" PostBackUrl="~/MasterData/AOW/Default.aspx" CausesValidation="false">
                            <i class="fas fa-angle-double-left"></i> Back
                        </asp:LinkButton>
                        <asp:LinkButton ID="btnUpdate" runat="server" CssClass="btn btn-success" OnClick="btnUpdate_Click" OnClientClick="return validateForm();">
                            <i class="fas fa-save"></i> Update Rule
                        </asp:LinkButton>
                    </div>
                </div>
                
                <div class="card-body p-4">
                    
                    <asp:HiddenField ID="hdnId" runat="server" />

                    <div class="alert alert-info py-2 small mb-4">
                        <i class="fas fa-info-circle"></i> Update the minimum and maximum amount threshold, or change the designated role and sequence level.
                    </div>

                    <div class="row">
                        <div class="col-md-6 mb-3">
                            <label class="form-label">Min Amount (RM) <span class="asterisk">*</span></label>
                            <div class="input-group">
                                <div class="input-group-prepend"><span class="input-group-text">RM</span></div>
                                <asp:TextBox ID="txtMinAmount" runat="server" CssClass="form-control" type="number" step="0.01" placeholder="0.00"></asp:TextBox>
                            </div>
                        </div>

                        <div class="col-md-6 mb-3">
                            <label class="form-label">Max Amount (RM)</label>
                            <div class="input-group">
                                <div class="input-group-prepend"><span class="input-group-text">RM</span></div>
                                <asp:TextBox ID="txtMaxAmount" runat="server" CssClass="form-control" type="number" step="0.01" placeholder="Leave blank for 'Above'"></asp:TextBox>
                            </div>
                            <small class="text-muted d-block mt-1">Leave blank if this is the highest tier (e.g., Above RM5,000,000).</small>
                        </div>
                    </div>

                    <div class="row mt-2">
                        <div class="col-md-4 mb-3">
                            <label class="form-label">Sequence Level <span class="asterisk">*</span></label>
                            <asp:TextBox ID="txtLevel" runat="server" CssClass="form-control" type="number" placeholder="1"></asp:TextBox>
                            <small class="text-muted d-block mt-1">1 = First Person, 2 = Second Person, etc.</small>
                        </div>

                        <div class="col-md-4 mb-3">
                            <label class="form-label">Role <span class="asterisk">*</span></label>
                            <asp:DropDownList ID="ddlRoleCode" runat="server" CssClass="form-control select2" data-placeholder="Select Role"></asp:DropDownList>
                        </div>

                    </div>

                </div>
            </div>
        </div>
    </div>

    <script>
        $(document).ready(function () {
            $('.select2').select2({
                theme: 'bootstrap4',
                width: '100%'
            });
        });
    </script>
</asp:Content>