<%@ Page Title="Upload PO" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Prodata.WebForm.T1C.PO.Upload.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="col-md-12">
            <div class="card card-outline">
                <div class="card-header">
                    <div class="row">
                        <div class="col-md-3">
                            <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-control select2" AutoPostBack="true" OnSelectedIndexChanged="ddlStatus_SelectedIndexChanged" CausesValidation="false" data-placeholder="All Status">
                                <asp:ListItem Text="" Value="" />
                                <asp:ListItem Text="Approved" Value="Approved" />
                                <asp:ListItem Text="Completed" Value="Completed" />
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>
                <div class="card-body">
                    <div class="row">
                        <div class="col-12">
                            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                <ContentTemplate>
                                    <div class="table-responsive">
                                        <asp:GridView ID="gvData" runat="server" AutoGenerateColumns="false" CssClass="table table-bordered" PageSize='<%# FGV.Prodata.App.Setting.RecordsPerPage() %>' AllowPaging="true" OnPageIndexChanging="gvData_PageIndexChanging" EmptyDataText="No record.">
                                            <Columns>
                                                <asp:TemplateField HeaderText="#">
                                                    <HeaderStyle CssClass="width-30 text-center" />
                                                    <ItemStyle CssClass="width-30 text-center" />
                                                    <ItemTemplate>
                                                        <%# Container.DataItemIndex + 1 %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="BizAreaDisplayName" HeaderText="BA" HeaderStyle-CssClass="align-middle" />
                                                <asp:BoundField DataField="Ref" HeaderText="Reference No" HeaderStyle-CssClass="align-middle text-nowrap" ItemStyle-CssClass="text-nowrap" />
                                                <asp:BoundField DataField="Date" HeaderText="Date" HeaderStyle-CssClass="align-middle text-nowrap" />
                                                <asp:BoundField DataField="Details" HeaderText="Details" HeaderStyle-CssClass="align-middle text-nowrap" />
                                                <asp:BoundField DataField="Amount" HeaderText="Amount (RM)" HeaderStyle-CssClass="align-middle text-nowrap" ItemStyle-CssClass="text-right" />
                                                <asp:TemplateField HeaderText="Status">
                                                    <HeaderStyle CssClass="width-120 text-center align-middle" />
                                                    <ItemStyle CssClass="width-120 text-center" />
                                                    <ItemTemplate>
                                                        <asp:Label runat="server"
                                                            CssClass='<%#
                                                                (bool)Eval("IsPendingUserAction") ? "badge badge-primary badge-pill" :
                                                                Eval("Status") != null ? (
                                                                    Eval("Status").ToString().ToLower() == "approved" ? "badge badge-success badge-pill" :
                                                                    Eval("Status").ToString().ToLower() == "pending" ? "badge badge-warning badge-pill" :
                                                                    Eval("Status").ToString().ToLower() == "rejected" ? "badge badge-danger badge-pill" :
                                                                    Eval("Status").ToString().ToLower() == "draft" ? "badge badge-info badge-pill" :
                                                                    "badge badge-secondary badge-pill"
                                                                ) : "badge badge-secondary badge-pill"
                                                            %>'
                                                            Text='<%# 
                                                                (bool)Eval("IsPendingUserAction") ? "Pending My Action" :
                                                                Eval("Status") != null && Eval("Status").ToString().Equals("SentBack", StringComparison.OrdinalIgnoreCase) ? "Sent Back" :
                                                                Eval("Status")
                                                            %>'>
                                                        </asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Action">
                                                    <HeaderStyle CssClass="width-80 text-center align-middle" />
                                                    <ItemStyle CssClass="width-80 text-center" />
                                                    <ItemTemplate>
                                                        <asp:LinkButton runat="server"
                                                            CssClass='<%# Eval("Status").ToString().ToLower() == "approved" ? "btn btn-outline-secondary btn-xs" : "btn btn-outline-info btn-xs" %>'
                                                            PostBackUrl='<%# $"~/T1C/PO/Upload/Edit?Id={Eval("Id")}" %>'>
                                                            <i class='<%# Eval("Status").ToString().ToLower() == "approved" ? "fas fa-edit" : "fas fa-eye" %>'></i>
                                                        </asp:LinkButton>
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
</asp:Content>
