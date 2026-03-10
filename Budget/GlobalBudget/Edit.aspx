<%@ Page Title="Edit Pool Budget" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Prodata.WebForm.Budget.GlobalBudget.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    
    <div class="row">
        <div class="col-md-12">
            <div class="card card-primary card-outline">
                <div class="card-header">
                    <h3 class="card-title"><i class="fas fa-coins mr-2"></i> Pool Budget Entry</h3>
                </div>
                <div class="card-body">
                    
                    <div class="alert alert-info">
                        <i class="icon fas fa-info-circle"></i> 
                        This form initializes or updates the shared pool budget. 
                        This amount will be available for all BizAreas to request budgets.
                    </div>

                    <div class="form-group row">
                        <label class="col-sm-3 col-form-label">Budget Year <span class="text-danger">*</span></label>
                        <div class="col-sm-4">
                            <asp:DropDownList ID="ddlYear" runat="server" CssClass="form-control select2" Enabled="false"></asp:DropDownList>
                        </div>
                    </div>

                    <div class="form-group row">
                        <label class="col-sm-3 col-form-label">Budget Type <span class="text-danger">*</span></label>
                        <div class="col-sm-6">
                            <asp:DropDownList ID="ddlBudgetType" runat="server" CssClass="form-control select2" DataValueField="Id" DataTextField="Name" Enabled="false">
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator ID="rfvBudgetType" runat="server" ControlToValidate="ddlBudgetType" 
                                CssClass="text-danger small" ErrorMessage="Budget Type is required." Display="Dynamic" ValidationGroup="Save"></asp:RequiredFieldValidator>
                        </div>
                    </div>

                    <div class="form-group row">
                        <label class="col-sm-3 col-form-label">Total Allocation (RM) <span class="text-danger">*</span></label>
                        <div class="col-sm-6">
                            <div class="input-group">
                                <div class="input-group-prepend">
                                    <span class="input-group-text">RM</span>
                                </div>
                                <%-- Added onkeypress to restrict input --%>
                                <asp:TextBox ID="txtAmount" runat="server" CssClass="form-control" placeholder="0.00" onkeypress="return isNumberKey(event)"></asp:TextBox>
                            </div>
                            <asp:RequiredFieldValidator ID="rfvAmount" runat="server" ControlToValidate="txtAmount" 
                                CssClass="text-danger small" ErrorMessage="Amount is required." Display="Dynamic" ValidationGroup="Save"></asp:RequiredFieldValidator>
                        </div>
                    </div>

                    <div class="form-group row">
                        <label class="col-sm-3 col-form-label">Description</label>
                        <div class="col-sm-9">
                            <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" placeholder="e.g. Pool Budget For All Mill..."></asp:TextBox>
                        </div>
                    </div>

                    <div class="form-group row">
                        <label class="col-sm-3 col-form-label">Remarks</label>
                        <div class="col-sm-9">
                            <asp:TextBox ID="txtRemarks" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" placeholder="e.g. Approved Pool Budget for this year..."></asp:TextBox>
                        </div>
                    </div>

                    <div class="row mt-4">
                        <div class="col-sm-3"></div>
                        <div class="col-sm-9 d-flex justify-content-end">
                            <%-- Changed OnClientClick to call confirmSave() --%>
                            <asp:Button ID="btnSave" runat="server" Text="Save Budget" CssClass="btn btn-success" 
                                OnClick="btnSave_Click" ValidationGroup="Save" 
                                OnClientClick="return confirmSave();" />
                            
                            <asp:HyperLink ID="lnkCancel" runat="server" NavigateUrl="~/Budget/GlobalBudget/Default" CssClass="btn btn-default ml-2">Cancel</asp:HyperLink>
                        </div>
                    </div>

                    <div class="row mt-4">
                        <div class="col-md-12">
                            <div class="card card-secondary collapsed-card">
                                <div class="card-header">
                                    <h3 class="card-title"><i class="fas fa-history mr-2"></i> View Change History</h3>
                                    <div class="card-tools">
                                        <button type="button" class="btn btn-tool" data-card-widget="collapse"><i class="fas fa-plus"></i></button>
                                    </div>
                                </div>
                                <div class="card-body p-0" style="max-height: 400px; overflow-y: auto;">
                                    <table class="table table-sm table-striped table-hover mb-0">
                                        <thead class="bg-light">
                                            <tr>
                                                <th>Action</th>
                                                <th>Date</th>
                                                <th>By</th>
                                                <th>Amount Before Edited (RM)</th>
                                                <th>Remarks</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <asp:Repeater ID="rptAudit" runat="server">
                                                <ItemTemplate>
                                                    <tr>
                                                        <td>
                                                            <span class='badge <%# Eval("Action").ToString() == "UPDATE" ? "badge-info" : "badge-success" %>'>
                                                                <%# Eval("Action") %>
                                                            </span>
                                                        </td>
                                                        <td><%# Eval("ActionDate", "{0:dd/MM/yyyy HH:mm}") %></td>
                                                        <td><%# Eval("ActionBy") %></td>
                                                        <td><%# Eval("Amount", "{0:N2}") %></td>
                                                        <td><%# Eval("Remarks") %></td>
                                                    </tr>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                            <asp:PlaceHolder ID="phNoAudit" runat="server" Visible="false">
                                                <tr>
                                                    <td colspan="5" class="text-center p-4">No audit history found.</td>
                                                </tr>
                                            </asp:PlaceHolder>
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                        </div>
                    </div>

                </div>
            </div>
        </div>
    </div>
    
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <script>
        $(document).ready(function () {
            $('.select2').select2({
                theme: 'bootstrap4'
            });
        });

        // 1. Function to restrict input to numbers and decimal point
        function isNumberKey(evt) {
            var charCode = (evt.which) ? evt.which : evt.keyCode;
            // 46 is dot (.)
            if (charCode != 46 && charCode > 31 && (charCode < 48 || charCode > 57))
                return false;
            return true;
        }

        // 2. Function to handle SweetAlert confirmation
        function confirmSave() {
            // Trigger ASP.NET Validation first
            if (typeof (Page_ClientValidate) == 'function') {
                if (Page_ClientValidate("Save") == false) {
                    return false;
                }
            }

            Swal.fire({
                title: 'Confirm Save?',
                text: "Confirm initialize budget?",
                icon: 'question',
                showCancelButton: true,
                confirmButtonColor: '#28a745',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Yes, confirm!'
            }).then((result) => {
                if (result.isConfirmed) {
                    // Manually trigger the server-side click
                    // We use __doPostBack with the Button's UniqueID
                    __doPostBack('<%= btnSave.UniqueID %>', '');
                }
            });

            // Always return false to stop the default button submission (which would bypass the alert wait)
            return false;
        }
    </script>

</asp:Content>