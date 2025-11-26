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

                            <!-- Code -->
                            <div class="form-group row">
                                <asp:Label ID="lblCode" runat="server" AssociatedControlID="txtCode"
                                    CssClass="col-md-3 col-form-label text-md-right" Text="Code"></asp:Label>
                                <div class="col-md-7">
                                    <asp:TextBox ID="txtCode" runat="server" CssClass="form-control input-uppercase" placeholder="Code"></asp:TextBox>
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCode" CssClass="text-danger" Display="Dynamic" ErrorMessage="Code is required"></asp:RequiredFieldValidator>
                                </div>
                            </div>

                            <!-- Name -->
                            <div class="form-group row">
                                <asp:Label ID="lblName" runat="server" AssociatedControlID="txtName"
                                    CssClass="col-md-3 col-form-label text-md-right" Text="Name"></asp:Label>
                                <div class="col-md-7">
                                    <asp:TextBox ID="txtName" runat="server" CssClass="form-control" placeholder="Name"></asp:TextBox>
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtName" CssClass="text-danger" Display="Dynamic" ErrorMessage="Name is required"></asp:RequiredFieldValidator>
                                </div>
                            </div>

                            <!-- Budget Type Dropdown -->
                            <div class="form-group row">
                                <asp:Label ID="lblBudgetType" runat="server" AssociatedControlID="ddlBudgetType"
                                    CssClass="col-md-3 col-form-label text-md-right" Text="Budget Form Type"></asp:Label>
                                <div class="col-md-7">
                                    <asp:DropDownList ID="ddlBudgetType" runat="server" CssClass="form-control">
                                        <asp:ListItem Value="">-- Select Form Type --</asp:ListItem>
                                        <asp:ListItem Value="1">Details Form</asp:ListItem>
                                        <asp:ListItem Value="2">Others Form</asp:ListItem>
                                        <asp:ListItem Value="3">Pool Form</asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlBudgetType"
                                        InitialValue="" CssClass="text-danger" Display="Dynamic"
                                        ErrorMessage="Please select a Form Type."></asp:RequiredFieldValidator>
                                </div>
                            </div>

                            <!-- Budget Type Dropdown -->
                            <div class="form-group row">
                                <asp:Label ID="lblFormType" runat="server" AssociatedControlID="ddlFormType"
                                    CssClass="col-md-3 col-form-label text-md-right" Text="Budget Details"></asp:Label>
                                <div class="col-md-7">
                                    <asp:DropDownList ID="ddlFormType" runat="server" CssClass="form-control">
                                        <asp:ListItem Value="">-- Select Upload Budget Details --</asp:ListItem>
                                        <asp:ListItem Value="1">Details Budget</asp:ListItem>
                                        <asp:ListItem Value="2">Others Budget</asp:ListItem>
                                        <asp:ListItem Value="3">Pool Budget</asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlFormType"
                                        InitialValue="" CssClass="text-danger" Display="Dynamic"
                                        ErrorMessage="Please select a Budget Type."></asp:RequiredFieldValidator>
                                </div>
                            </div>

                        </div>
                    </div>
                </div>

            </div>
        </div>
    </div>
</asp:Content>
