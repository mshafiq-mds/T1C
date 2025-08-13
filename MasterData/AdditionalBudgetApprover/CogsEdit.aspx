<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CogsEdit.aspx.cs" Inherits="Prodata.WebForm.MasterData.AdditionalBudgetApprover.CogsEdit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="hdnId" runat="server" />
    <div class="row">
        <div class="col-md-12">
            <div class="card card-outline">
                <div class="card-header card-header-sticky">
                    <h3 class="card-title d-none d-sm-inline"><%: Page.Title %></h3>
                    <div class="card-tools">
                        <asp:LinkButton ID="btnBack" runat="server" CssClass="btn btn-default" PostBackUrl="~/MasterData/AdditionalBudgetApprover/Default" CausesValidation="false">
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
                                <asp:Label ID="lblMinValue" runat="server" AssociatedControlID="txtMinValue" CssClass="col-md-3 col-form-label text-md-right" Text="Min Value"></asp:Label>
                                <div class="col-md-7">
                                    <div class="input-group">
                                        <div class="input-group-prepend">
                                            <span class="input-group-text">RM</span>
                                        </div>
                                        <asp:TextBox ID="txtMinValue" runat="server" CssClass="form-control input-number2" placeholder="Min Value"></asp:TextBox>
                                    </div>
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtMinValue" CssClass="text-danger" Display="Dynamic" ErrorMessage="Min Value is required"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="form-group row">
                                <asp:Label ID="lblMaxValue" runat="server" AssociatedControlID="txtMaxValue" CssClass="col-md-3 col-form-label text-md-right" Text="Max Value"></asp:Label>
                                <div class="col-md-7">
                                    <div class="input-group">
                                        <div class="input-group-prepend">
                                            <span class="input-group-text">RM</span>
                                        </div>
                                        <asp:TextBox ID="txtMaxValue" runat="server" CssClass="form-control input-number2" placeholder="Max Value (blank for unlimited)"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group row">
                                <asp:Label ID="lblSection" runat="server" AssociatedControlID="txtSection" CssClass="col-md-3 col-form-label text-md-right" Text="Section"></asp:Label>
                                <div class="col-md-7">
                                    <asp:TextBox ID="txtSection" runat="server" CssClass="form-control" placeholder="Section"></asp:TextBox>
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtSection" CssClass="text-danger" Display="Dynamic" ErrorMessage="Section is required"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="form-group row">
                                <asp:Label ID="lblIPMSRole" runat="server" AssociatedControlID="ddlIPMSRole" CssClass="col-md-3 col-form-label text-md-right" Text="Role"></asp:Label>
                                <div class="col-md-7">
                                    <asp:DropDownList ID="ddlIPMSRole" runat="server" CssClass="form-control select2" data-placeholder="Role"></asp:DropDownList>
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlIPMSRole" CssClass="text-danger" Display="Dynamic" ErrorMessage="Role is required" InitialValue=""></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="form-group row">
                                <asp:Label ID="lblOrder" runat="server" AssociatedControlID="txtOrder" CssClass="col-md-3 col-form-label text-md-right" Text="Order"></asp:Label>
                                <div class="col-md-7">
                                    <asp:TextBox ID="txtOrder" runat="server" CssClass="form-control input-number col-md-4" placeholder="Order"></asp:TextBox>
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtOrder" CssClass="text-danger" Display="Dynamic" ErrorMessage="Order is required"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
