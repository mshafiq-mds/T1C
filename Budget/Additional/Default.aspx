<%@ Page Title="Additional Budget" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Prodata.WebForm.Budget.AddBudget.Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="hdnDeleteId" runat="server" />
    <asp:Button ID="btnDeleteConfirmed" runat="server" CssClass="d-none" OnClick="btnDeleteConfirmed_Click" />

    <asp:Panel runat="server" CssClass="card p-4">
        <div class="card-tools">
            <asp:LinkButton ID="btnAdd" runat="server" CssClass="btn btn-primary" PostBackUrl="/Budget/Additional/Add" CausesValidation="false">
                <i class="fas fa-plus"></i>  Request Additional Budget
            </asp:LinkButton>
        </div>

         <div class="mb-3"></div>

        <asp:GridView ID="gvBudgetList" runat="server" CssClass="table table-bordered table-sm" AutoGenerateColumns="False"
            EmptyDataText="No budget applications found." DataKeyNames="Id" PageSize='<%# FGV.Prodata.App.Setting.RecordsPerPage() %>' AllowPaging="true">
            <Columns>
                <asp:BoundField DataField="RefNo" HeaderText="Reference No." />
                <asp:BoundField DataField="CreatedDate" HeaderText="Date" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:BoundField DataField="BA" HeaderText="BA" />
                <asp:BoundField DataField="Project" HeaderText="Project / Department" />
                <asp:BoundField DataField="EstimatedCost" HeaderText="Estimated Cost (RM)" DataFormatString="{0:N2}" />
                <asp:BoundField DataField="Status" HeaderText="Status"/>
                <asp:TemplateField HeaderText="Action">
                    <HeaderStyle CssClass="width-80 text-center align-middle" />
                    <ItemStyle CssClass="width-80 text-center" />
                    <%--<ItemTemplate>
                         <a class="btn btn-info" href='/Additional/Edit?Id=1' >
                             <i class="fas fa-edit"></i>
                         </a>
                         <asp:LinkButton ID="btnDelete" runat="server" CssClass="btn btn-danger btn-xs button-delete" Visible='<%# Prodata.WebForm.Auth.Can(Prodata.WebForm.Auth.Id(), "admin-user-delete") %>'>
                             <i class="fas fa-trash-alt"></i>
                         </asp:LinkButton>                          
                    </ItemTemplate>--%>
                    <ItemTemplate>
                        <%# (Eval("Status").ToString() == "Completed") ? 
                            "<a class='btn btn-info btn-xs' href='/Budget/Additional/View.aspx?id=" + Eval("Id") + "'><i class='fas fa-eye'></i></a>" : "" %>

                        <%# (Eval("Status") == null || Eval("Status").ToString() == "Resubmit" || Eval("Status").ToString() == "Submitted") ? 
                            "<a class='btn btn-info btn-xs' href='/Budget/Additional/Edit.aspx?id=" + Eval("Id") + "'><i class='fas fa-edit'></i></a>" : "" %>
    
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
    $(document).ready(function () {
        $(document).on("click", ".button-delete", function (e) {
            e.preventDefault();

            var id = $(this).attr("commandargument");

            Swal.fire({
                title: 'Are you sure?',
                text: "This transfer will be deleted!",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#d33',
                cancelButtonColor: '#3085d6',
                confirmButtonText: 'Yes, delete it!'
            }).then((result) => {
                if (result.isConfirmed) {
                    $('#<%= hdnDeleteId.ClientID %>').val(id);
                    __doPostBack('<%= btnDeleteConfirmed.UniqueID %>', '');
                }
            });
        });
    });
</script>
</asp:Content>

