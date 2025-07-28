<%@ Page Title="Additional Budget Approver" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Prodata.WebForm.MasterData.AdditionalBudgetApprover.Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="hdnRecordId" runat="server" />
    <asp:Button ID="btnDeleteRecordFinance" runat="server" OnClick="btnDeleteRecordFinance_Click" CssClass="d-none" />
    <asp:Button ID="btnDeleteRecordCogs" runat="server" OnClick="btnDeleteRecordCogs_Click" CssClass="d-none" />
    <asp:Button ID="btnDeleteRecordCum" runat="server" OnClick="btnDeleteRecordCum_Click" CssClass="d-none" />

    <div class="row">
        <div class="col-md-12">
            <div class="card card-outline">
                <div class="card-header">
                    <h3 class="card-title d-none d-sm-inline"><%: Page.Title %></h3>
                    <%--<div class="card-tools">
                        <asp:LinkButton ID="btnAdd" runat="server" CssClass="btn btn-primary" PostBackUrl="~/MasterData/BudgetApprover/Add" CausesValidation="false">
                            <i class="fas fa-plus"></i> Add Budget Approver
                        </asp:LinkButton>
                    </div>--%>
                </div>
                <div class="card-body">
                    <div class="card card-outline card-outline-tabs">
                        <div class="card-header p-0 border-bottom-0">
                            <ul class="nav nav-tabs" id="custom-tab" role="tablist">
                                <li class="nav-item">
                                    <a class="nav-link active" id="tab-loa-fin-tab" data-toggle="pill" href="#tab-loa-fin" role="tab" aria-controls="tab-loa-fin" aria-selected="true">LOA (Finance Group)</a>
                                </li>
                                <li class="nav-item">
                                    <a class="nav-link" id="tab-loa-cogs-tab" data-toggle="pill" href="#tab-loa-cogs" role="tab" aria-controls="tab-loa-cogs" aria-selected="false">LOA (COGS)</a>
                                </li>
                                <li class="nav-item">
                                    <a class="nav-link" id="tab-loa-cum-tab" data-toggle="pill" href="#tab-loa-cum" role="tab" aria-controls="tab-loa-cum" aria-selected="false">Final Approver</a>
                                </li>
                            </ul>
                        </div>
                        <div class="card-body">
                            <div class="tab-content" id="custom-tabContent">
                                <!-- LOA Finance Group Tab -->
                                <div class="tab-pane fade show active" id="tab-loa-fin" role="tabpanel" aria-labelledby="tab-loa-fin-tab">
                                    <asp:UpdatePanel ID="updLoaFinance" runat="server">
                                        <ContentTemplate>
                                            <div class="table-responsive">
                                                <div class="card-tools">
                                                    <asp:LinkButton ID="btnAdd" runat="server" CssClass="btn btn-primary" PostBackUrl="~/MasterData/AdditionalBudgetApprover/FinanceAdd" CausesValidation="false">
                                                        <i class="fas fa-plus"></i> Add Finance Group Approver
                                                    </asp:LinkButton>
                                                </div>
                                                <asp:GridView ID="gvLoaFinance" runat="server" AutoGenerateColumns="false" CssClass="table table-bordered table-sm"
                                                    PageSize='<%# FGV.Prodata.App.Setting.RecordsPerPage() %>' AllowPaging="true"
                                                    OnPageIndexChanging="gvLoaFinance_PageIndexChanging" EmptyDataText="No record.">
                                                    <Columns>
                                                        <asp:TemplateField HeaderText="#">
                                                            <HeaderStyle CssClass="width-30 text-center" />
                                                            <ItemStyle CssClass="width-30 text-center" />
                                                            <ItemTemplate>
                                                                <%# Container.DataItemIndex + 1 %>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="AmountMin" HeaderText="Min (RM)" HeaderStyle-CssClass="text-center" ItemStyle-CssClass="text-right" />
                                                        <asp:BoundField DataField="AmountMax" HeaderText="Max (RM)" HeaderStyle-CssClass="text-center" ItemStyle-CssClass="text-right" />
                                                        <asp:BoundField DataField="Section" HeaderText="Section" />
                                                        <asp:BoundField DataField="ApproverName" HeaderText="Role" />
                                                        <asp:BoundField DataField="Order" HeaderText="Order" HeaderStyle-CssClass="text-center" ItemStyle-CssClass="text-center" />
                                                        <asp:TemplateField HeaderText="Action">
                                                            <HeaderStyle CssClass="width-80 text-center" />
                                                            <ItemStyle CssClass="width-80 text-center" />
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="btnEdit1" runat="server" CssClass="btn btn-info btn-xs" OnClick="btnEdit1_Click" Visible='<%# Prodata.WebForm.Auth.Can(Prodata.WebForm.Auth.Id(), "budget-approver-edit") %>'>
                                                                    <i class="fas fa-edit"></i>
                                                                </asp:LinkButton>
                                                                <asp:LinkButton ID="btnDelete1" runat="server" CssClass="btn btn-danger btn-xs button-delete-finance" data-id='<%# Eval("Id") %>' Visible='<%# Prodata.WebForm.Auth.Can(Prodata.WebForm.Auth.Id(), "budget-approver-delete") %>'>
                                                                    <i class="fas fa-trash-alt"></i>
                                                                </asp:LinkButton>
                                                                <asp:HiddenField ID="hdnId1" runat="server" Value='<%# Eval("Id") %>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                    <PagerSettings Mode="NumericFirstLast" PageButtonCount="5" Position="TopAndBottom" />
                                                    <PagerStyle CssClass="pagination-ys" />
                                                </asp:GridView>
                                            </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </div>

                                <!-- LOA COGS Tab -->
                                <div class="tab-pane fade" id="tab-loa-cogs" role="tabpanel" aria-labelledby="tab-loa-cogs-tab">
                                    <asp:UpdatePanel ID="updLoaCogs" runat="server">
                                        <ContentTemplate>
                                            <div class="table-responsive">
                                                <div class="card-tools">
                                                    <asp:LinkButton ID="LinkButton1" runat="server" CssClass="btn btn-primary" PostBackUrl="~/MasterData/AdditionalBudgetApprover/CogsAdd" CausesValidation="false">
                                                        <i class="fas fa-plus"></i> Add COGS Approver
                                                    </asp:LinkButton>
                                                </div>
                                                <asp:GridView ID="gvLoaCogs" runat="server" AutoGenerateColumns="false" CssClass="table table-bordered table-sm"
                                                    PageSize='<%# FGV.Prodata.App.Setting.RecordsPerPage() %>' AllowPaging="true"
                                                    OnPageIndexChanging="gvLoaCogs_PageIndexChanging" EmptyDataText="No record.">
                                                    <Columns>
                                                        <asp:TemplateField HeaderText="#">
                                                            <HeaderStyle CssClass="width-30 text-center" />
                                                            <ItemStyle CssClass="width-30 text-center" />
                                                            <ItemTemplate>
                                                                <%# Container.DataItemIndex + 1 %>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="AmountMin" HeaderText="Min (RM)" HeaderStyle-CssClass="text-center" ItemStyle-CssClass="text-right" />
                                                        <asp:BoundField DataField="AmountMax" HeaderText="Max (RM)" HeaderStyle-CssClass="text-center" ItemStyle-CssClass="text-right" />
                                                        <asp:BoundField DataField="Section" HeaderText="Section" />
                                                        <asp:BoundField DataField="ApproverName" HeaderText="Role" />
                                                        <asp:BoundField DataField="Order" HeaderText="Order" HeaderStyle-CssClass="text-center" ItemStyle-CssClass="text-center" />
                                                        <asp:TemplateField HeaderText="Action">
                                                            <HeaderStyle CssClass="width-80 text-center" />
                                                            <ItemStyle CssClass="width-80 text-center" />
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="btnEdit2" runat="server" CssClass="btn btn-info btn-xs" OnClick="btnEdit2_Click" Visible='<%# Prodata.WebForm.Auth.Can(Prodata.WebForm.Auth.Id(), "budget-approver-edit") %>'>
                                                                    <i class="fas fa-edit"></i>
                                                                </asp:LinkButton>
                                                                <asp:LinkButton ID="btnDelete2" runat="server" CssClass="btn btn-danger btn-xs button-delete-cogs" data-id='<%# Eval("Id") %>' Visible='<%# Prodata.WebForm.Auth.Can(Prodata.WebForm.Auth.Id(), "budget-approver-delete") %>'>
                                                                    <i class="fas fa-trash-alt"></i>
                                                                </asp:LinkButton>
                                                                <asp:HiddenField ID="hdnId2" runat="server" Value='<%# Eval("Id") %>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                    <PagerSettings Mode="NumericFirstLast" PageButtonCount="5" Position="TopAndBottom" />
                                                    <PagerStyle CssClass="pagination-ys" />
                                                </asp:GridView>
                                            </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </div>

                                <!-- Cumm Group Tab -->
                                <div class="tab-pane fade show active" id="tab-loa-cum" role="tabpanel" aria-labelledby="tab-loa-cum-tab">
                                    <asp:UpdatePanel ID="updLoaCum" runat="server">
                                        <ContentTemplate>
                                            <div class="table-responsive">
                                                <div class="card-tools">
                                                    <asp:LinkButton ID="LinkButton2" runat="server" CssClass="btn btn-primary" PostBackUrl="~/MasterData/AdditionalBudgetApprover/CumAdd" CausesValidation="false">
                                                        <i class="fas fa-plus"></i> Add Final Group Approver
                                                    </asp:LinkButton>
                                                </div>
                                                <asp:GridView ID="gvLoaCum" runat="server" AutoGenerateColumns="false" CssClass="table table-bordered table-sm"
                                                    PageSize='<%# FGV.Prodata.App.Setting.RecordsPerPage() %>' AllowPaging="true"
                                                    OnPageIndexChanging="gvLoaCum_PageIndexChanging" EmptyDataText="No record.">
                                                    <Columns>
                                                        <asp:TemplateField HeaderText="#">
                                                            <HeaderStyle CssClass="width-30 text-center" />
                                                            <ItemStyle CssClass="width-30 text-center" />
                                                            <ItemTemplate>
                                                                <%# Container.DataItemIndex + 1 %>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="AmountMax" HeaderText="Max (RM)" HeaderStyle-CssClass="text-center" ItemStyle-CssClass="text-right" />
                                                        <asp:BoundField DataField="AmmountCummulative" HeaderText="Cummulative" HeaderStyle-CssClass="text-center" ItemStyle-CssClass="text-right" />
                                                        <asp:BoundField DataField="AmountMin" HeaderText="Balance" HeaderStyle-CssClass="text-center" ItemStyle-CssClass="text-right" />
                                                        <asp:BoundField DataField="Section" HeaderText="Section" />
                                                        <asp:BoundField DataField="ApproverName" HeaderText="Role" />
                                                        <asp:BoundField DataField="Order" HeaderText="Order" HeaderStyle-CssClass="text-center" ItemStyle-CssClass="text-center" />
                                                        <asp:TemplateField HeaderText="Action">
                                                            <HeaderStyle CssClass="width-80 text-center" />
                                                            <ItemStyle CssClass="width-80 text-center" />
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="btnEdit3" runat="server" CssClass="btn btn-info btn-xs" OnClick="btnEdit3_Click" Visible='<%# Prodata.WebForm.Auth.Can(Prodata.WebForm.Auth.Id(), "budget-approver-edit") %>'>
                                                                    <i class="fas fa-edit"></i>
                                                                </asp:LinkButton>
                                                                <asp:LinkButton ID="btnDelete3" runat="server" CssClass="btn btn-danger btn-xs button-delete-cum" data-id='<%# Eval("Id") %>' Visible='<%# Prodata.WebForm.Auth.Can(Prodata.WebForm.Auth.Id(), "budget-approver-delete") %>'>
                                                                    <i class="fas fa-trash-alt"></i>
                                                                </asp:LinkButton>
                                                                <asp:HiddenField ID="hdnId3" runat="server" Value='<%# Eval("Id") %>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                    <PagerSettings Mode="NumericFirstLast" PageButtonCount="5" Position="TopAndBottom" />
                                                    <PagerStyle CssClass="pagination-ys" />
                                                </asp:GridView>
                                            </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </div>

                            </div>
                        </div>
                    </div> <!-- End Tabs -->
                </div>
            </div>
        </div>
    </div>

    <script>
        $(document).ready(function () {
            $(document).on("click", ".button-delete-finance", function (event) {
                event.preventDefault();
                var recordId = $(this).data("id");
                Swal.fire({
                    title: "Are you sure?",
                    text: "This record will be deleted permanently!",
                    icon: "warning",
                    showCancelButton: true,
                    confirmButtonColor: "#d33",
                    cancelButtonColor: "#3085d6",
                    confirmButtonText: "Yes, delete it!"
                }).then((result) => {
                    if (result.isConfirmed) {
                        $('#<%= hdnRecordId.ClientID %>').val(recordId);
                        __doPostBack("<%= btnDeleteRecordFinance.UniqueID %>", "");
                    }
                });
            });
            $(document).on("click", ".button-delete-cogs", function (event) {
                event.preventDefault();
                var recordId = $(this).data("id");
                Swal.fire({
                    title: "Are you sure?",
                    text: "This record will be deleted permanently!",
                    icon: "warning",
                    showCancelButton: true,
                    confirmButtonColor: "#d33",
                    cancelButtonColor: "#3085d6",
                    confirmButtonText: "Yes, delete it!"
                }).then((result) => {
                    if (result.isConfirmed) {
                        $('#<%= hdnRecordId.ClientID %>').val(recordId);
                        __doPostBack("<%= btnDeleteRecordCogs.UniqueID %>", "");
                    }
                });
            });
            $(document).on("click", ".button-delete-cum", function (event) {
                event.preventDefault();
                var recordId = $(this).data("id");
                Swal.fire({
                    title: "Are you sure?",
                    text: "This record will be deleted permanently!",
                    icon: "warning",
                    showCancelButton: true,
                    confirmButtonColor: "#d33",
                    cancelButtonColor: "#3085d6",
                    confirmButtonText: "Yes, delete it!"
                }).then((result) => {
                    if (result.isConfirmed) {
                        $('#<%= hdnRecordId.ClientID %>').val(recordId);
                        __doPostBack("<%= btnDeleteRecordCum.UniqueID %>", "");
                    }
                });
            });
        });
    </script>
</asp:Content>