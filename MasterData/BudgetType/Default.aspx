﻿<%@ Page Title="Budget Type" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Prodata.WebForm.MasterData.BudgetType.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="hdnRecordId" runat="server" />
    <asp:Button ID="btnDeleteRecord" runat="server" OnClick="btnDeleteRecord_Click" CssClass="d-none" />
    <div class="row">
        <div class="col-md-12">
            <div class="card card-outline">
                <div class="card-header card-header-sticky">
                    <h3 class="card-title d-none d-sm-inline"><%= Page.Title %></h3>
                    <div class="card-tools">
                        <asp:LinkButton ID="btnAdd" runat="server" CssClass="btn btn-primary" PostBackUrl="/MasterData/BudgetType/Add" CausesValidation="false">
                            <i class="fas fa-plus"></i> Add Budget Type
                        </asp:LinkButton>
                    </div>
                </div>
                <div class="card-body">
                    <div class="row">
                        <div class="col-12">
                            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                <ContentTemplate>
                                    <div class="table-responsive">
                                        <asp:GridView ID="gvBudgetType" runat="server" AutoGenerateColumns="false" CssClass="table table-bordered table-sm" PageSize='<%# FGV.Prodata.App.Setting.RecordsPerPage() %>' AllowPaging="true" OnPageIndexChanging="gvBudgetType_PageIndexChanging" EmptyDataText="No record.">
                                            <Columns>
                                                <asp:TemplateField HeaderText="#">
                                                    <HeaderStyle CssClass="width-30 text-center" />
                                                    <ItemStyle CssClass="width-30 text-center" />
                                                    <ItemTemplate>
                                                        <%# Container.DataItemIndex + 1 %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="Code" HeaderText="Code" />
                                                <asp:BoundField DataField="Name" HeaderText="Name" />
                                                <asp:TemplateField HeaderText="Action">
                                                    <HeaderStyle CssClass="width-80 text-center" />
                                                    <ItemStyle CssClass="width-80 text-center" />
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="btnEdit" runat="server" CssClass="btn btn-info btn-xs" OnClick="btnEdit_Click" Visible='<%# Prodata.WebForm.Auth.Can(Prodata.WebForm.Auth.Id(), "budget-type-edit") %>'>
                                                            <i class="fas fa-edit"></i>
                                                        </asp:LinkButton>
                                                        <asp:LinkButton ID="btnDelete" runat="server" CssClass="btn btn-danger btn-xs button-delete" data-id='<%# Eval("Id") %>' Visible='<%# Prodata.WebForm.Auth.Can(Prodata.WebForm.Auth.Id(), "budget-type-delete") %>'>
                                                            <i class="fas fa-trash-alt"></i>
                                                        </asp:LinkButton>
                                                        <asp:HiddenField ID="hdnId" runat="server" Value='<%# Eval("Id") %>' />
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
            </div>
        </div>
    </div>
    <script>
        $(document).ready(function () {
            // Use event delegation for dynamically rendered buttons
            $(document).on("click", ".button-delete", function (event) {
                event.preventDefault(); // Prevent immediate postback

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
                    if (result.isConfirmed) {  // ✅ Only proceed if the user confirms
                        $('#<%= hdnRecordId.ClientID %>').val(recordId);
                        __doPostBack("<%= btnDeleteRecord.UniqueID %>", "");
                    }
                });
            });
        });
    </script>
</asp:Content>
