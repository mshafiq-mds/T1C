<%@ Page Title="Additional Budget" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Prodata.WebForm.Budget.AddBudget.Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="hdnDeleteId" runat="server" />
    <asp:Button ID="btnDeleteConfirmed" runat="server" CssClass="d-none" OnClick="btnDeleteConfirmed_Click" />
    <asp:HiddenField ID="hdnDeleteRemarks" runat="server" />

    <asp:Panel runat="server" CssClass="card p-4">
        <div class="d-flex justify-content-between align-items-center mb-3 flex-wrap">
            <div class="form-inline">
                <%--<label for="ddlStatusFilter" class="mr-2 mb-0">Filter by Status:</label>--%>
                <asp:DropDownList ID="ddlStatusFilter" runat="server" AutoPostBack="true" CssClass="form-control select2" Style="width: 300px;"
                    OnSelectedIndexChanged="ddlStatusFilter_SelectedIndexChanged" data-placeholder="All">
                    <asp:ListItem Text="" Value=""/>
                    <%--<asp:ListItem Text="All" Value="All" Selected="True"/>--%>
                    <asp:ListItem Text="Submitted" Value="Submitted" />
                    <asp:ListItem Text="Resubmit" Value="Resubmit" />
                    <asp:ListItem Text="Under Review" Value="Under Review" />
                    <asp:ListItem Text="Completed" Value="Completed" />
                    <asp:ListItem Text="Deleted" Value="Deleted" />
                </asp:DropDownList>
            </div>

            <div class="card-tools">
                <asp:LinkButton ID="btnAdd" runat="server" CssClass="btn btn-primary" PostBackUrl="~/Budget/Additional/Add" CausesValidation="false">
                    <i class="fas fa-plus"></i> Request Additional Budget
                </asp:LinkButton>
            </div>
        </div>

         <div class="mb-3"></div>

        <asp:GridView ID="gvBudgetList" runat="server" CssClass="table table-bordered table-sm" AutoGenerateColumns="False"
                              OnPageIndexChanging="gvList_PageIndexChanging"
            EmptyDataText="No budget applications found." DataKeyNames="Id" PageSize='<%# FGV.Prodata.App.Setting.RecordsPerPage() %>' AllowPaging="true">
            <Columns>
                <asp:BoundField DataField="RefNo" HeaderText="Reference No." />
                <asp:BoundField DataField="CreatedDate" HeaderText="Date" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:BoundField DataField="BA" HeaderText="BA" />
                <asp:BoundField DataField="Project" HeaderText="Project / Department" />
                <asp:BoundField DataField="EstimatedCost" HeaderText="Estimated Cost (RM)" DataFormatString="{0:N2}" />
                <asp:TemplateField HeaderText="Status">
                    <ItemTemplate>
                        <asp:Label ID="lblStatus" runat="server"
                            Text='<%# Eval("Status") %>'
                            CssClass='<%#
                                Eval("Status").ToString() == "Deleted" ? "text-danger font-weight-bold" :
                                Eval("Status").ToString() == "Resubmit" ? "text-warning font-weight-bold" :
                                Eval("Status").ToString() == "Under Review" ? "text-info" :
                                Eval("Status").ToString() == "Completed" ? "text-success" :
                                "text-primary"
                            %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Action">
                    <HeaderStyle CssClass="width-80 text-center align-middle" />
                    <ItemStyle CssClass="width-80 text-center" />
                    <%--<ItemTemplate>
                         <a class="btn btn-info" href='Additional/Edit?Id=1' >
                             <i class="fas fa-edit"></i>
                         </a>
                         <asp:LinkButton ID="btnDelete" runat="server" CssClass="btn btn-danger btn-xs button-delete" Visible='<%# Prodata.WebForm.Auth.Can(Prodata.WebForm.Auth.Id(), "admin-user-delete") %>'>
                             <i class="fas fa-trash-alt"></i>
                         </asp:LinkButton>                          
                    </ItemTemplate>--%>
                    <ItemTemplate>
                        <%# (Eval("Status").ToString() == "Completed" || Eval("Status").ToString() == "Under Review" || Eval("Status").ToString() == "Deleted") ? 
                            "<a class='btn btn-info btn-xs' href='View.aspx?id=" + Eval("Id") + "'><i class='fas fa-eye'></i></a>" : "" %>
                        
                        <%# (Eval("Status").ToString() == "Resubmit") ? 
                            "<a class='btn btn-info btn-xs' href='Resubmit.aspx?id=" + Eval("Id") + "' title='Resubmit Form'><i class='fas fa-sync-alt'></i></a>" : "" %>

                        <%# (Eval("Status").ToString() == "Submitted") ? 
                            "<a class='btn btn-info btn-xs' href='Edit.aspx?id=" + Eval("Id") + "'><i class='fas fa-edit'></i></a>" : "" %>
    
                        <%# (Eval("Status") == null || Eval("Status").ToString() == "Submitted" || Eval("Status").ToString() == "Resubmit") //||
                             //Prodata.WebForm.Auth.Can(Prodata.WebForm.Auth.Id(), "admin-user-delete")) 
                            ? "<a href='#' class='btn btn-danger btn-xs button-delete' commandargument='" + Eval("Id") + "'><i class='fas fa-trash-alt'></i></a>"
                            : "" 
                        %>

                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
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

