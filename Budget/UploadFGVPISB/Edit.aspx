<%@ Page Title="Edit Budget" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Prodata.WebForm.Budget.UploadFGVPISB.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .form-label {
            font-weight: 600;
        }
        .card-header-sticky {
            position: sticky;
            top: 0;
            z-index: 100;
            background-color: white;
            border-bottom: 1px solid rgba(0,0,0,.125);
        }
        .audit-badge {
            font-size: 0.85rem;
            padding: 5px 10px;
        }
    </style>

    <div class="row justify-content-center">
        <div class="col-md-10">
            <!-- MAIN FORM CARD -->
            <div class="card card-outline">
                <div class="card-header card-header-sticky">
                    <h3 class="card-title">
                        <i class="fas fa-edit mr-2"></i> Edit Budget Details
                    </h3>
                    <div class="card-tools">
                        <asp:LinkButton ID="btnBack" runat="server" CssClass="btn btn-sm btn-secondary" OnClick="btnCancel_Click">
                            <i class="fas fa-arrow-left"></i> Back
                        </asp:LinkButton>
                    </div>
                </div>

                <div class="card-body">
                    <asp:HiddenField ID="hdnId" runat="server" />
                    
                    <!-- Row 1: Reference & Business Area -->
                    <div class="row">
                        <div class="col-md-6">
                            <div class="form-group">
                                <label class="form-label">No. Rujukan / Reference</label>
                                <asp:TextBox ID="txtRef" runat="server" CssClass="form-control" placeholder="Enter Reference No"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvRef" runat="server" ControlToValidate="txtRef" 
                                    CssClass="text-danger small" ErrorMessage="Reference is required" Display="Dynamic" />
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-group">
                                <label class="form-label">Business Area Code (BA)</label>
                                <asp:TextBox ID="txtBACode" runat="server" CssClass="form-control" placeholder="e.g. 1234"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvBA" runat="server" ControlToValidate="txtBACode" 
                                    CssClass="text-danger small" ErrorMessage="BA Code is required" Display="Dynamic" />
                            </div>
                        </div>
                    </div>

                    <!-- Row 2: Project & Details -->
                    <div class="row">
                        <div class="col-md-6">
                            <div class="form-group">
                                <label class="form-label">Project / Biz Area Name</label>
                                <asp:TextBox ID="txtProject" runat="server" CssClass="form-control" placeholder="Project Name"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-group">
                                <label class="form-label">Butir-butir Kerja / Details</label>
                                <asp:TextBox ID="txtDetails" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" placeholder="Work details..."></asp:TextBox>
                            </div>
                        </div>
                    </div>

                    <hr />

                    <!-- Row 3: Month & Year -->
                    <div class="row">
                        <div class="col-md-6">
                            <div class="form-group">
                                <label class="form-label">Month</label>
                                <asp:DropDownList ID="ddlMonth" runat="server" CssClass="form-control select2">
                                    <asp:ListItem Value="" Text="-- Select Month --" />
                                    <asp:ListItem Value="1" Text="January" />
                                    <asp:ListItem Value="2" Text="February" />
                                    <asp:ListItem Value="3" Text="March" />
                                    <asp:ListItem Value="4" Text="April" />
                                    <asp:ListItem Value="5" Text="May" />
                                    <asp:ListItem Value="6" Text="June" />
                                    <asp:ListItem Value="7" Text="July" />
                                    <asp:ListItem Value="8" Text="August" />
                                    <asp:ListItem Value="9" Text="September" />
                                    <asp:ListItem Value="10" Text="October" />
                                    <asp:ListItem Value="11" Text="November" />
                                    <asp:ListItem Value="12" Text="December" />
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="rfvMonth" runat="server" ControlToValidate="ddlMonth" 
                                    CssClass="text-danger small" ErrorMessage="Month is required" Display="Dynamic" />
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-group">
                                <label class="form-label">Year</label>
                                <asp:DropDownList ID="ddlYear" runat="server" CssClass="form-control select2">
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="rfvYear" runat="server" ControlToValidate="ddlYear" 
                                    CssClass="text-danger small" ErrorMessage="Year is required" Display="Dynamic" />
                            </div>
                        </div>
                    </div>

                    <!-- Row 4: Vendor -->
                    <div class="row">
                        <div class="col-md-12">
                            <div class="form-group">
                                <label class="form-label">Vendor</label>
                                <asp:TextBox ID="txtVendor" runat="server" CssClass="form-control" placeholder="Vendor Name"></asp:TextBox>
                            </div>
                        </div>
                    </div>

                    <hr />

                    <!-- Row 5: Financials -->
                    <div class="row">
                        <div class="col-md-4">
                            <div class="form-group">
                                <label class="form-label">Wages (RM)</label>
                                <asp:TextBox ID="txtWages" runat="server" CssClass="form-control text-right calc-input" placeholder="0.00" onkeypress="return isNumberKey(event)"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div class="form-group">
                                <label class="form-label">Purchase / Belian (RM)</label>
                                <asp:TextBox ID="txtPurchase" runat="server" CssClass="form-control text-right calc-input" placeholder="0.00" onkeypress="return isNumberKey(event)"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div class="form-group">
                                <label class="form-label text-primary">Total Amount (RM)</label>
                                <asp:TextBox ID="txtAmount" runat="server" CssClass="form-control text-right font-weight-bold" placeholder="0.00" onkeypress="return isNumberKey(event)"></asp:TextBox>
                                <small class="text-muted">Total Budget Amount</small>
                            </div>
                        </div>
                    </div>

                </div>

                <div class="card-footer text-right">
                    <asp:LinkButton ID="btnCancel" runat="server" CssClass="btn btn-default mr-2" OnClick="btnCancel_Click" CausesValidation="false">
                        <i class="fas fa-times"></i> Cancel
                    </asp:LinkButton>
                    <asp:LinkButton ID="btnSave" runat="server" CssClass="btn btn-success" OnClick="btnSave_Click">
                        <i class="fas fa-save"></i> Save Changes
                    </asp:LinkButton>
                </div>
            </div>

            <!-- AUDIT HISTORY CARD -->
            <div class="card card-outline card-secondary mt-4">
                <div class="card-header">
                    <h3 class="card-title">
                        <i class="fas fa-history mr-2"></i> Audit History
                    </h3>
                </div>
                <div class="card-body p-0 table-responsive">
                    <asp:GridView ID="gvAudit" runat="server" CssClass="table table-hover table-striped mb-0" 
                        AutoGenerateColumns="false" GridLines="None" ShowHeaderWhenEmpty="true" EmptyDataText="No audit history found.">
                        <Columns>
                            <asp:TemplateField HeaderText="#" ItemStyle-Width="50px">
                                <ItemTemplate>
                                    <%# Container.DataItemIndex + 1 %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="ActionDate" HeaderText="Date/Time" DataFormatString="{0:dd-MMM-yyyy HH:mm:ss}" />
                            <asp:TemplateField HeaderText="Action">
                                <ItemTemplate>
                                    <span class='badge badge-info audit-badge'>
                                        <%# Eval("ActionType") %>
                                    </span>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="ActionBy" HeaderText="User" />
                            <asp:BoundField DataField="Remarks" HeaderText="Details" />
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
            <!-- END AUDIT HISTORY -->
            
            <div class="mb-5"></div> <!-- Bottom Spacer -->

        </div>
    </div>

    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <script>
        // Allow only numbers and dot
        function isNumberKey(evt) {
            var charCode = (evt.which) ? evt.which : evt.keyCode;
            if (charCode != 46 && charCode > 31 && (charCode < 48 || charCode > 57))
                return false;
            return true;
        }

        // Optional: Auto-calculate Total if user edits Wages or Purchase
        $(document).ready(function () {
            $('.calc-input').on('keyup change', function () {
                var wages = parseFloat($('#<%= txtWages.ClientID %>').val()) || 0;
                var purchase = parseFloat($('#<%= txtPurchase.ClientID %>').val()) || 0;
                
                // Only auto-calc if both are 0 or user hasn't manually overridden the Total yet
                // For simplicity, let's just sum them if fields exist
                var total = wages + purchase;
                
                // If the sum is greater than 0, suggest it in the Total box
                if(total > 0) {
                   // $('#<%= txtAmount.ClientID %>').val(total.toFixed(2));
                }
            });
        });
    </script>
</asp:Content>