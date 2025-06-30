<%@ Page Title="Budget Transfer" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Prodata.WebForm.Budget.Transfer.Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="hdnDeleteId" runat="server" />
    <asp:Button ID="btnDeleteConfirmed" runat="server" CssClass="d-none" OnClick="btnDeleteConfirmed_Click" />
    <asp:HiddenField ID="hdnDeleteRemarks" runat="server" />

    <asp:Panel runat="server" CssClass="card p-4">

        <div class="d-flex justify-content-between align-items-center mb-3 flex-wrap">
            <div class="form-inline">
                <label for="ddlStatusFilter" class="mr-2 mb-0">Filter by Status:</label>
                <asp:DropDownList ID="ddlStatusFilter" runat="server" AutoPostBack="true" CssClass="form-control"
                    OnSelectedIndexChanged="ddlStatusFilter_SelectedIndexChanged">
                    <asp:ListItem Text="All" Value="All" />
                    <asp:ListItem Text="Submitted" Value="Submitted" />
                    <asp:ListItem Text="Resubmit" Value="Resubmit" />
                    <asp:ListItem Text="Under Review" Value="Under Review" />
                    <asp:ListItem Text="Completed" Value="Completed" />
                    <asp:ListItem Text="Finalized" Value="Finalized" />
                    <asp:ListItem Text="Deleted" Value="Deleted" />
                </asp:DropDownList>
            </div>

            <div class="card-tools">
                <asp:LinkButton ID="btnAdd" runat="server" CssClass="btn btn-primary" PostBackUrl="/Budget/Transfer/Add" CausesValidation="false">
                    <i class="fas fa-plus"></i> Request Transfer Budget
                </asp:LinkButton>
            </div>
        </div>

        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <asp:GridView ID="gvTransfers" runat="server"
                              CssClass="table table-bordered table-sm"
                              PageSize='<%# FGV.Prodata.App.Setting.RecordsPerPage() %>'
                              AllowPaging="true"
                              AutoGenerateColumns="False"
                              DataKeyNames="Id"
                              EmptyDataText="No record.">
                    <Columns>
                        <asp:BoundField DataField="BA" HeaderText="BA" />
                        <asp:BoundField DataField="RefNo" HeaderText="Reference No" />
                        <asp:BoundField DataField="Project" HeaderText="Project" />
                        <asp:BoundField DataField="Date" HeaderText="Application Date" DataFormatString="{0:dd/MM/yyyy}" />
                        <asp:BoundField DataField="EstimatedCost" HeaderText="Estimated Cost (RM)" DataFormatString="{0:N2}" />
                        <asp:TemplateField HeaderText="Status">
                            <ItemTemplate>
                                <asp:Label ID="lblStatus" runat="server"
                                    Text='<%# Eval("Status") %>'
                                    CssClass='<%# 
                                          Eval("Status").ToString() == "Deleted" ? "text-danger fw-bold" :
                                          Eval("Status").ToString() == "Resubmit" ? "text-warning fw-bold" :
                                          Eval("Status").ToString() == "Under Review" ? "text-primary" :
                                          Eval("Status").ToString() == "Completed" ? "text-success" :
                                          Eval("Status").ToString() == "Finalized" ? "text-muted fst-italic" :
                                          ""
                                    %>'>
                                </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Action">
                            <HeaderStyle CssClass="width-80 text-center align-middle" />
                            <ItemStyle CssClass="width-80 text-center" />
                            <ItemTemplate>
                                <%# (Eval("Status").ToString() == "Completed" || Eval("Status").ToString() == "Under Review"|| Eval("Status").ToString() == "Deleted" || Eval("Status").ToString() == "Finalized") ? 
                                    "<a class='btn btn-info btn-xs' href='/Budget/Transfer/View.aspx?id=" + Eval("Id") + "' title='View Details'><i class='fas fa-eye'></i></a>" : "" %>

                                <%# (Eval("Status").ToString() == "Resubmit") ? 
                                    "<a class='btn btn-info btn-xs' href='/Budget/Transfer/Resubmit.aspx?id=" + Eval("Id") + "' title='Resubmit Form'><i class='fas fa-sync-alt'></i></a>" : "" %>

                                <%# (Eval("Status").ToString() == "Submitted") ? 
                                    "<a class='btn btn-info btn-xs' href='/Budget/Transfer/Edit.aspx?id=" + Eval("Id") + "' title='Edit Submission'><i class='fas fa-edit'></i></a>" : "" %>

                                <%# (Eval("Status") == null || Eval("Status").ToString() == "Submitted" || Eval("Status").ToString() == "Resubmit") 
                                    ? "<a href='#' class='btn btn-danger btn-xs button-delete' commandargument='" + Eval("Id") + "' title='Delete Entry'><i class='fas fa-trash-alt'></i></a>" 
                                    : "" 
                                %>
                            </ItemTemplate>


                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </ContentTemplate>
        </asp:UpdatePanel>
    </asp:Panel>

    <!-- SweetAlert script -->
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>

    <script type="text/javascript">
        $(document).on("click", ".button-delete", function (e) {
            e.preventDefault();

            var id = $(this).attr("commandargument");

            Swal.fire({
                title: 'Are you sure?',
                text: "This transfer will be deleted!",
                input: 'textarea',
                inputLabel: 'Remarks (required)',
                inputPlaceholder: 'Enter reason for deletion...',
                inputAttributes: {
                    'aria-label': 'Remarks for deletion'
                },
                inputValidator: (value) => {
                    if (!value) {
                        return 'Remarks are required!';
                    }
                },
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#d33',
                cancelButtonColor: '#3085d6',
                confirmButtonText: 'Yes, delete it!'
            }).then((result) => {
                if (result.isConfirmed) {
                    $('#<%= hdnDeleteId.ClientID %>').val(id);
                    $('#<%= hdnDeleteRemarks.ClientID %>').val(result.value);
                    __doPostBack('<%= btnDeleteConfirmed.UniqueID %>', '');
                }
            });
        });

    </script>
</asp:Content>