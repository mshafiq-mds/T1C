<%@ Page Title="T1C" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Prodata.WebForm.T1C.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="hdnRecordId" runat="server" />
    <asp:Button ID="btnDeleteRecord" runat="server" OnClick="btnDeleteRecord_Click" CssClass="d-none" />
    <div class="row">
        <div class="col-md-12">
            <div class="card card-outline">
                <div class="card-header">
                    <h3 class="card-title d-none d-sm-inline"><%= Page.Title %></h3>
                    <div class="card-tools">
                        <asp:LinkButton ID="btnAdd" runat="server" CssClass="btn btn-primary" PostBackUrl="/T1C/Add" CausesValidation="false">
                            <i class="fas fa-plus"></i> New Budget T1C
                        </asp:LinkButton>
                    </div>
                </div>
                <div class="card-body">
                    <div class="row">
                        <div class="col-12">
                            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                <ContentTemplate>
                                    <div class="table-responsive">
                                        <asp:GridView ID="gvData" runat="server" AutoGenerateColumns="false" CssClass="table table-bordered table-sm" PageSize='<%# FGV.Prodata.App.Setting.RecordsPerPage() %>' AllowPaging="true" OnPageIndexChanging="gvData_PageIndexChanging" EmptyDataText="No record.">
                                            <Columns>
                                                <asp:TemplateField HeaderText="#">
                                                    <HeaderStyle CssClass="width-30 text-center" />
                                                    <ItemStyle CssClass="width-30 text-center" />
                                                    <ItemTemplate>
                                                        <%# Container.DataItemIndex + 1 %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="Ref" HeaderText="No. Rujukan" HeaderStyle-CssClass="align-middle text-nowrap" ItemStyle-CssClass="text-nowrap" />
                                                <asp:BoundField DataField="Date" HeaderText="Tarikh" HeaderStyle-CssClass="align-middle text-nowrap" />
                                                <asp:BoundField DataField="Details" HeaderText="Butir-butir" HeaderStyle-CssClass="align-middle text-nowrap" />
                                                <asp:BoundField DataField="Amount" HeaderText="Anggaran Kerja (RM)" HeaderStyle-CssClass="align-middle text-nowrap" ItemStyle-CssClass="text-right" />
                                                <asp:BoundField DataField="Status" HeaderText="Status" HeaderStyle-CssClass="align-middle text-center" ItemStyle-CssClass="text-center" />
                                                <asp:TemplateField HeaderText="Action">
                                                    <HeaderStyle CssClass="width-80 text-center align-middle" />
                                                    <ItemStyle CssClass="width-80 text-center" />
                                                    <ItemTemplate>
                                                        <a class="btn btn-info btn-xs<%# (bool)Eval("IsEditable") ? "" : " disabled" %>" href='/T1C/Edit?Id=<%# Eval("Id") %>' onclick='<%# (bool)Eval("IsEditable") ? "" : "return false;" %>'>
                                                            <i class="fas fa-edit"></i>
                                                        </a>
                                                        <asp:LinkButton ID="btnDelete" runat="server" CssClass="btn btn-danger btn-xs button-delete" data-id='<%# Eval("Id") %>' Visible='<%# Prodata.WebForm.Auth.Can(Prodata.WebForm.Auth.Id(), "admin-user-delete") %>'>
                                                            <i class="fas fa-trash-alt"></i>
                                                        </asp:LinkButton>
                                                        <asp:HiddenField ID="hdnFormId" runat="server" Value='<%# Eval("Id") %>' />
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
