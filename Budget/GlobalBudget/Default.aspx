<%@ Page Title="Pool Budget List" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Prodata.WebForm.Budget.GlobalBudget.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    
    <style>
        .card-header-sticky {
            position: sticky;
            top: 0;
            z-index: 100;
            background-color: white;
            border-bottom: 1px solid rgba(0,0,0,.125);
        }
    </style>

    <%-- Hidden Fields for Delete Logic --%>
    <asp:HiddenField ID="hdnRecordId" runat="server" />
    <asp:Button ID="btnDeleteRecord" runat="server" OnClick="btnDeleteRecord_Click" CssClass="d-none" />

    <div class="row">
        <div class="col-md-12">
            <div id="divCardSearch" runat="server" class="card card-primary card-outline">
                <div class="card-header card-header-sticky">
                    <h3 class="card-title">Search & Filter</h3>
                    <div class="card-tools">
                        <asp:LinkButton ID="LinkButton2" runat="server" 
                            CssClass="btn btn-outline-success px-4 fw-bold border-2" 
                            PostBackUrl="~/Budget/GlobalBudget/Default.aspx">
                            <i class="fas fa-file-invoice-dollar me-2"></i> Back
                        </asp:LinkButton>
                        <button type="button" class="btn btn-tool" data-card-widget="collapse"><i class="fas fa-minus"></i></button>
                    </div>
                </div>
                <div class="card-body">
                    <div class="row">
                        <div class="col-md-4">
                            <div class="form-group">
                                <label>Year</label>
                                <asp:DropDownList ID="ddlYear" runat="server" CssClass="form-control select2">
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div class="form-group">
                                <label>Budget Type</label>
                                <asp:DropDownList ID="ddlBudgetType" runat="server" CssClass="form-control select2" DataValueField="Id" DataTextField="Name">
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div class="form-group">
                                <label>&nbsp;</label> <br />
                                <asp:LinkButton ID="btnSearch" runat="server" CssClass="btn btn-primary" OnClick="btnSearch_Click">
                                    <i class="fas fa-search"></i> Search
                                </asp:LinkButton>
                                <asp:LinkButton ID="btnReset" runat="server" CssClass="btn btn-default" OnClick="btnReset_Click">
                                    <i class="fas fa-sync-alt"></i> Reset
                                </asp:LinkButton>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="row">
        <div class="col-md-12">
            <div class="card card-outline card-secondary">
                <div class="card-header">
                    <h3 class="card-title">Pool Budget Records</h3>
                    <div class="card-tools">
                        <asp:HyperLink ID="btnAdd" runat="server" NavigateUrl="~/Budget/GlobalBudget/Add.aspx" CssClass="btn btn-success btn-sm">
                            <i class="fas fa-plus"></i> New Budget
                        </asp:HyperLink>
                    </div>
                </div>
                <div class="card-body">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            <div class="table-responsive">
                                <asp:GridView ID="gvData" runat="server" AutoGenerateColumns="false" 
                                    CssClass="table table-bordered table-hover" 
                                    PageSize='<%# FGV.Prodata.App.Setting.RecordsPerPage() %>' 
                                    AllowPaging="true" OnPageIndexChanging="gvData_PageIndexChanging" 
                                    EmptyDataText="No records found."
                                    DataKeyNames="Id">
                                    <Columns>
                                        <asp:TemplateField HeaderText="#">
                                            <ItemStyle Width="50px" HorizontalAlign="Center" />
                                            <ItemTemplate>
                                                <%# Container.DataItemIndex + 1 + (gvData.PageIndex * gvData.PageSize) %>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:BoundField DataField="Year" HeaderText="Year" SortExpression="Year" ItemStyle-HorizontalAlign="Center" />
                                        <asp:BoundField DataField="BudgetType" HeaderText="Budget Type" SortExpression="BudgetType" />
                                        <asp:BoundField DataField="Details" HeaderText="Details" />
                                        
                                        <asp:TemplateField HeaderText="Total Allocation (RM)">
                                            <ItemStyle HorizontalAlign="Right" />
                                            <ItemTemplate>
                                                <%# Eval("Amount", "{0:N2}") %>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                               <%--         <asp:TemplateField HeaderText="Current Balance (RM)">
                                            <ItemStyle HorizontalAlign="Right" Font-Bold="true" />
                                            <ItemTemplate>
                                                <span class='<%# Convert.ToDecimal(Eval("CurrentBalance")) < 0 ? "text-danger" : "text-success" %>'>
                                                    <%# Eval("CurrentBalance", "{0:N2}") %>
                                                </span>
                                            </ItemTemplate>
                                        </asp:TemplateField>--%>

                                        <asp:BoundField DataField="CreatedDate" HeaderText="Created Date" DataFormatString="{0:dd/MM/yyyy HH:mm}" ItemStyle-HorizontalAlign="Center" />

                                        <asp:TemplateField HeaderText="Action">
                                            <ItemStyle Width="100px" HorizontalAlign="Center" />
                                            <ItemTemplate>
                                                <asp:HyperLink runat="server" 
                                                    NavigateUrl='<%# "Edit.aspx?Id=" + Eval("Id") %>' 
                                                    CssClass='<%# "btn btn-info btn-xs" + (Prodata.WebForm.Auth.Can(Prodata.WebForm.Auth.Id(), "GlobalBudget-Edit") ? "" : " disabled") %>'
                                                    Enabled='<%# Prodata.WebForm.Auth.Can(Prodata.WebForm.Auth.Id(), "GlobalBudget-Edit") %>'
                                                    title="Edit">
                                                    <i class="fas fa-edit"></i>
                                                </asp:HyperLink>
                                                <%-- Edit Button (Points to Create.aspx for now, logic needed there to handle Edit) --%>
                                                <%--<asp:PlaceHolder runat="server" Enabled='<%# Prodata.WebForm.Auth.Can(Prodata.WebForm.Auth.Id(), "GlobalBudget-Edit") %>'>
                                                    <a href='<%# "AddEditView.aspx?Id=" + Eval("Id") %>' class="btn btn-info btn-xs" title="Edit">
                                                        <i class="fas fa-edit"></i>
                                                    </a>
                                                </asp:PlaceHolder>--%>
                                                <%--<a href='<%# "AddEditView.aspx?Id=" + Eval("Id") %>' class="btn btn-info btn-xs" title="Edit"
                                                    Visible='<%# Prodata.WebForm.Auth.Can(Prodata.WebForm.Auth.Id(), "GlobalBudget-Edit") %>'>
                                                    <i class="fas fa-edit"></i>
                                                </a>--%>


                                                <%-- Delete Button --%>
                                                <button type="button" 
                                                    class='<%# "btn btn-danger btn-xs button-delete" + (Prodata.WebForm.Auth.Can(Prodata.WebForm.Auth.Id(), "GlobalBudget-delete") ? "" : " disabled") %>' 
                                                    data-id='<%# Eval("Id") %>' 
                                                    title="Delete"
                                                    <%# Prodata.WebForm.Auth.Can(Prodata.WebForm.Auth.Id(), "GlobalBudget-delete") ? "" : "disabled=\"disabled\"" %>>
                                                    <i class="fas fa-trash-alt"></i>
                                                </button>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <PagerSettings Mode="NumericFirstLast" PageButtonCount="5" Position="Bottom" />
                                    <PagerStyle CssClass="pagination-ys" />
                                </asp:GridView>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </div>

    <!-- SweetAlert Script for Delete -->
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            // Re-bind events after UpdatePanel postback
            var prm = Sys.WebForms.PageRequestManager.getInstance();
            prm.add_endRequest(function () {
                bindDeleteEvent();
                $('.select2').select2({ theme: 'bootstrap4' });
            });

            bindDeleteEvent();
            $('.select2').select2({ theme: 'bootstrap4' });
        });

        function bindDeleteEvent() {
            $(".button-delete").off('click').on("click", function (e) {
                e.preventDefault();
                var recordId = $(this).data("id");

                Swal.fire({
                    title: 'Are you sure?',
                    text: "You won't be able to revert this!",
                    icon: 'warning',
                    showCancelButton: true,
                    confirmButtonColor: '#d33',
                    cancelButtonColor: '#3085d6',
                    confirmButtonText: 'Yes, delete it!'
                }).then((result) => {
                    if (result.isConfirmed) {
                        $('#<%= hdnRecordId.ClientID %>').val(recordId);
                        __doPostBack('<%= btnDeleteRecord.UniqueID %>', '');
                    }
                });
            });
        }
    </script>

</asp:Content>