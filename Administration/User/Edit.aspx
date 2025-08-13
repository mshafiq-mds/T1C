<%@ Page Title="Edit User" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Prodata.WebForm.Administration.User.Edit" %>

<%@ Import Namespace="CustomGuid.AspNet.Identity" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style type="text/css">
        .profile-user-img {
            width: auto !important;
        }

        label:not(.form-check-label):not(.custom-file-label) {
            padding-left: 5px;
        }
    </style>
    <div class="row">
        <div class="col-md-12">
            <div class="card card-outline">
                <div class="card-header">
                    <h3 class="card-title d-none d-sm-inline"><%: Page.Title %></h3>
                    <div class="card-tools">
                        <asp:LinkButton ID="btnBack" runat="server" CssClass="btn btn-default" PostBackUrl="~/Administration/User/Default" CausesValidation="false">
                            <i class="fas fa-angle-double-left"></i> Back
                        </asp:LinkButton>
                        <asp:LinkButton ID="btnSave" runat="server" CssClass="btn btn-primary" OnClick="btnSave_Click" Visible='<%# Prodata.WebForm.Auth.Can(Context.User.Identity.GetUserID(), "admin-user-edit") %>'>
                            <i class="fas fa-save"></i> Save
                        </asp:LinkButton>
                    </div>
                </div>
                <div class="card-body">
                    <div class="row">
                        <div class="col-12">
                            <div class="card card-outline card-outline-tabs">
                                <div class="card-header p-0 border-bottom-0">
                                    <ul class="nav nav-tabs" id="custom-tab" role="tablist">
                                        <li class="nav-item">
                                            <a class="nav-link active" id="detail-tab" data-toggle="pill" href="#detail-tab-content" role="tab" aria-controls="detail-tab-content" aria-selected="true">Detail</a>
                                        </li>
                                        <li class="nav-item">
                                            <a class="nav-link" id="role-tab" data-toggle="pill" href="#role-tab-content" role="tab" aria-controls="role-tab-content" aria-selected="false">Role</a>
                                        </li>
                                    </ul>
                                </div>
                                <div class="card-body">
                                    <div class="tab-content" id="custom-tabContent">
                                        <div class="tab-pane fade show active" id="detail-tab-content" role="tabpanel" aria-labelledby="detail-tab">
                                            <asp:HiddenField ID="hdnUserId" runat="server" />
                                            <div class="row">
                                                <div class="col-md-3">
                                                    <div class="row">
                                                        <div class="col-md-8 offset-md-4 col-12 text-center">
                                                            <img class="profile-user-img img-fluid" src="~/Images/user-alt-solid-darkgray.svg" runat="server" />
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="col-md-9">
                                                    <div class="form-group row">
                                                        <asp:Label ID="lblName" runat="server" AssociatedControlID="txtName" CssClass="col-md-4 col-form-label text-md-right" Text="Name"></asp:Label>
                                                        <div class="col-md-8">
                                                            <asp:TextBox ID="txtName" runat="server" CssClass="form-control" placeholder="Name"></asp:TextBox>
                                                            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtName" CssClass="text-danger" Display="Dynamic" ErrorMessage="Name is required"></asp:RequiredFieldValidator>
                                                            <asp:Label ID="lblNameErrors" runat="server" CssClass="text-danger" Visible="false"></asp:Label>
                                                        </div>
                                                    </div>
                                                    <div class="form-group row">
                                                        <asp:Label ID="lblEmail" runat="server" AssociatedControlID="txtEmail" CssClass="col-md-4 col-form-label text-md-right" Text="Email"></asp:Label>
                                                        <div class="col-md-8">
                                                            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" placeholder="Email"></asp:TextBox>
                                                            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtEmail" CssClass="text-danger" Display="Dynamic" ErrorMessage="Email is required"></asp:RequiredFieldValidator>
                                                        </div>
                                                    </div>
                                                    <div class="form-group row">
                                                        <asp:Label ID="lblUsername" runat="server" AssociatedControlID="txtUsername" CssClass="col-md-4 col-form-label text-md-right" Text="Username"></asp:Label>
                                                        <div class="col-md-8">
                                                            <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" placeholder="Username"></asp:TextBox>
                                                            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtUsername" CssClass="text-danger" Display="Dynamic" ErrorMessage="Username is required"></asp:RequiredFieldValidator>
                                                        </div>
                                                    </div>
                                                    <div class="form-group row">
                                                        <asp:Label ID="lblPassword" runat="server" AssociatedControlID="txtPassword" CssClass="col-md-4 col-form-label text-md-right" Text="New Password"></asp:Label>
                                                        <div class="col-md-8">
                                                            <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" placeholder="New Password" TextMode="Password"></asp:TextBox>
                                                            <asp:Label ID="lblPasswordErrors" runat="server" CssClass="text-danger" Visible="false"></asp:Label>
                                                        </div>
                                                    </div>
                                                    <div class="form-group row">
                                                        <asp:Label ID="lblConfirmPassword" runat="server" AssociatedControlID="txtConfirmPassword" CssClass="col-md-4 col-form-label text-md-right" Text="New Password Confirmation" />
                                                        <div class="col-md-8">
                                                            <asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="form-control" placeholder="New Password Confirmation" TextMode="Password" />
                                                            <asp:CompareValidator runat="server" ControlToCompare="txtPassword" ControlToValidate="txtConfirmPassword" CssClass="text-danger" Display="Dynamic" ErrorMessage="Password Confirmation do not match." />
                                                        </div>
                                                    </div>
                                                    <div class="form-group row">
                                                        <asp:Label ID="lblIPMSRole" runat="server" AssociatedControlID="ddlIPMSRole" CssClass="col-md-4 col-form-label text-md-right" Text="iPMS Role"></asp:Label>
                                                        <div class="col-md-8">
                                                            <asp:DropDownList ID="ddlIPMSRole" runat="server" CssClass="form-control select2" data-placeholder="iPMS Role"></asp:DropDownList>
                                                        </div>
                                                    </div>
                                                    <div class="form-group row">
                                                        <asp:Label ID="lblIPMSBizArea" runat="server" AssociatedControlID="ddlIPMSBizArea" CssClass="col-md-4 col-form-label text-md-right" Text="iPMS Biz Area"></asp:Label>
                                                        <div class="col-md-8">
                                                            <asp:DropDownList ID="ddlIPMSBizArea" runat="server" CssClass="form-control select2" data-placeholder="iPMS Biz Area"></asp:DropDownList>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="tab-pane fade" id="role-tab-content" role="tabpanel" aria-labelledby="role-tab">
                                            <asp:CheckBoxList ID="cblRoles" runat="server"></asp:CheckBoxList>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <script type="text/javascript">
        $(document).ready(function () {
            $('#<%= txtName.ClientID %>').on('input', function () {
                $('#<%= lblNameErrors.ClientID %>').hide();
            });
            $('#<%= txtPassword.ClientID %>').on('input', function () {
                $('#<%= lblPasswordErrors.ClientID %>').hide();
            });
        });
    </script>
</asp:Content>
