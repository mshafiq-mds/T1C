<%@ Page Title="Add Budget Type" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Add.aspx.cs" Inherits="Prodata.WebForm.MasterData.BudgetType.Add" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="col-md-12">
            <div class="card card-outline">
                <div class="card-header card-header-sticky">
                    <h3 class="card-title d-none d-sm-inline"><%: Page.Title %></h3>
                    <div class="card-tools">
                        <asp:LinkButton ID="btnBack" runat="server" CssClass="btn btn-default" PostBackUrl="~/MasterData/BudgetType/Default" CausesValidation="false">
                            <i class="fas fa-angle-double-left"></i> Back
                        </asp:LinkButton>
                        <asp:LinkButton ID="btnSave" runat="server" CssClass="btn btn-primary" OnClick="btnSave_Click">
                            <i class="fas fa-save"></i> Save
                        </asp:LinkButton>
                    </div>
                </div>
                <div class="card-body">
                    <div class="row">
                        <div class="col-md-8 offset-md-2">
                            <div class="form-group row">
                                <asp:Label ID="lblCode" runat="server" AssociatedControlID="txtCode" CssClass="col-md-3 col-form-label text-md-right" Text="Code"></asp:Label>
                                <div class="col-md-7">
                                    <asp:TextBox ID="txtCode" runat="server" CssClass="form-control input-uppercase" placeholder="Code"></asp:TextBox>
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCode" CssClass="text-danger" Display="Dynamic" ErrorMessage="Code is required"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="form-group row">
                                <asp:Label ID="lblName" runat="server" AssociatedControlID="txtName" CssClass="col-md-3 col-form-label text-md-right" Text="Name"></asp:Label>
                                <div class="col-md-7">
                                    <asp:TextBox ID="txtName" runat="server" CssClass="form-control" placeholder="Name"></asp:TextBox>
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtName" CssClass="text-danger" Display="Dynamic" ErrorMessage="Name is required"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
