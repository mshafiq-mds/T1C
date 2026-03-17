<%@ Page Title="AWO Approver" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Prodata.WebForm.MasterData.AOW.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>

    <script type="text/javascript">
        // Custom function to handle SweetAlert confirmation for ASP.NET PostBacks
        function confirmDelete(btn) {
            // If already confirmed, allow the postback to happen
            if (btn.dataset.confirmed) {
                return true;
            }

            // Otherwise, prevent postback and show SweetAlert
            Swal.fire({
                title: 'Are you sure?',
                text: "You are about to delete this approval limit. This cannot be undone.",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#dc3545', // Red color for delete
                cancelButtonColor: '#6c757d',  // Gray for cancel
                confirmButtonText: '<i class="fas fa-trash"></i> Yes, delete it!',
                cancelButtonText: 'Cancel'
            }).then((result) => {
                if (result.isConfirmed) {
                    // Set a flag to bypass this popup on the next click
                    btn.dataset.confirmed = true;
                    // Trigger the button click again to execute the server-side code
                    btn.click();
                }
            });

            return false; // Prevent the default postback on the first click
        }
    </script>

    <style>
        .full-screen-container { width: 100%; padding: 0 15px; }
        .card-custom { border: none; box-shadow: 0 4px 15px rgba(0,0,0,0.08); border-radius: 10px; margin-bottom: 30px; }
        .card-header-custom { background-color: #f8f9fa; border-bottom: 2px solid #e9ecef; border-radius: 10px 10px 0 0 !important; padding: 1.25rem 1.5rem; }
        .table-custom th { background-color: #343a40 !important; color: #ffffff !important; font-weight: 500; vertical-align: middle !important; text-align: center; }
        .table-custom td { vertical-align: middle; }
    </style>

    <div class="full-screen-container">
        
        <div class="card card-custom">                
            <div class="card-header card-header-sticky">
                <h3 class="card-title m-0 fw-bold text-dark">
                    <i class="fas fa-sitemap mr-2 text-primary"></i> Configure Approval Limits
                </h3>
                <div class="card-tools">
                    <asp:LinkButton ID="btnAddNew" runat="server" CssClass="btn btn-sm btn-primary shadow-sm" PostBackUrl="~/MasterData/AOW/Add.aspx">
                        <i class="fas fa-plus-circle"></i> Add New Limit
                    </asp:LinkButton>
                </div>
            </div>
            
            <div class="card-body p-0">
                <div class="table-responsive">
                    <asp:GridView ID="gvLimit" runat="server" CssClass="table table-hover table-striped table-custom mb-0" 
                        AutoGenerateColumns="False" GridLines="None" ShowHeaderWhenEmpty="True" EmptyDataText="No approval limits configured."
                        DataKeyNames="Id" OnRowCommand="gvLimit_RowCommand">
                        <Columns>
                            
                            <asp:TemplateField HeaderText="#">
                                <ItemStyle CssClass="text-center font-weight-bold" Width="50px" />
                                <ItemTemplate>
                                    <%# Container.DataItemIndex + 1 %>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:BoundField DataField="AmountMin" HeaderText="Min (RM)" DataFormatString="{0:N2}">
                                <ItemStyle CssClass="text-center" />
                            </asp:BoundField>

                            <asp:TemplateField HeaderText="Max (RM)">
                                <ItemStyle CssClass="text-center" />
                                <ItemTemplate>
                                    <%# Eval("AmountMax") != null ? Eval("AmountMax", "{0:N2}") : "<span class='badge badge-secondary'>Above</span>" %>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:BoundField DataField="Section" HeaderText="Section">
                                <ItemStyle CssClass="text-center" />
                            </asp:BoundField>

                            <asp:BoundField DataField="AWOApproverCode" HeaderText="Role">
                                <ItemStyle CssClass="font-weight-bold text-center" />
                            </asp:BoundField>

                            <asp:BoundField DataField="Order" HeaderText="Order">
                                <ItemStyle CssClass="text-center font-weight-bold text-primary" Width="100px" />
                            </asp:BoundField>

                            <asp:TemplateField HeaderText="Action">
                                <ItemStyle CssClass="text-center" Width="120px" />
                                <ItemTemplate>
                                    <asp:HyperLink runat="server" NavigateUrl='<%# "~/MasterData/AOW/Edit.aspx?id=" + Eval("Id") %>' CssClass="btn btn-info btn-sm mr-1" ToolTip="Edit">
                                        <i class="fas fa-edit"></i>
                                    </asp:HyperLink>
                                    
                                    <asp:LinkButton runat="server" CommandName="DeleteRule" CommandArgument='<%# Eval("Id") %>' 
                                        CssClass="btn btn-danger btn-sm" ToolTip="Delete" 
                                        OnClientClick="return confirmDelete(this);">
                                        <i class="fas fa-trash"></i>
                                    </asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>

                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </div>

    </div>
</asp:Content>