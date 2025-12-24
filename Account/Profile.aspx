<%@ Page Title="Profile" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Profile.aspx.cs" Inherits="Prodata.WebForm.Account.Profile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">


    <asp:PlaceHolder ID="plhMessage" runat="server" Visible="false">
        <div class="row">
            <div class="col-lg-4 col-sm-6">
                <div id="msgContainer" runat="server" class="alert alert-info" role="alert">
                    <asp:Label ID="lblMessage" runat="server"></asp:Label>
                    <button type="button" class="close" data-dismiss="alert" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
            </div>
        </div>
    </asp:PlaceHolder>

    <div class="row">
        <div class="col-lg-4 col-sm-6">
            <div class="card card-primary card-outline">
                <div class="card-body box-profile">
                    <div class="text-center">
                        <img runat="server" class="profile-user-img img-fluid img-circle"
                            src="~/Images/user-alt-solid-darkgray.svg"
                            alt="User profile picture">
                    </div>

                    <h3 class="profile-username text-center">
                        <asp:Label ID="lblName" runat="server"></asp:Label>
                    </h3>

                    <asp:Panel ID="pnlView" runat="server">
                        <p class="text-muted text-center">
                            <asp:Label ID="lblEmail" runat="server"></asp:Label>
                        </p>
                        
                        <asp:Button ID="btnEdit" runat="server" Text="Edit Email" 
                            CssClass="btn btn-primary btn-block" OnClick="btnEdit_Click" />
                    </asp:Panel>

                    <asp:Panel ID="pnlEdit" runat="server" Visible="false">
                        <div class="form-group mt-3">
                            <label for="<%= txtEditEmail.ClientID %>">Email Address</label>
                            <asp:TextBox ID="txtEditEmail" runat="server" CssClass="form-control" TextMode="Email"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEditEmail" 
                                ErrorMessage="Email is required" CssClass="text-danger small" Display="Dynamic" ValidationGroup="ProfileEdit"></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="revEmail" runat="server" ControlToValidate="txtEditEmail"
                                ErrorMessage="Invalid email format" CssClass="text-danger small" Display="Dynamic" 
                                ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ValidationGroup="ProfileEdit"></asp:RegularExpressionValidator>
                        </div>

                        <div class="row">
                            <div class="col-6">
                                <asp:Button ID="btnCancel" runat="server" Text="Cancel" 
                                    CssClass="btn btn-secondary btn-block" OnClick="btnCancel_Click" CausesValidation="false" />
                            </div>
                            <div class="col-6">
                                <asp:Button ID="btnSave" runat="server" Text="Save" 
                                    CssClass="btn btn-success btn-block" OnClick="btnSave_Click" ValidationGroup="ProfileEdit" />
                            </div>
                        </div>
                    </asp:Panel>

                </div>
                </div>
            </div>
        </div>
    </asp:Content>