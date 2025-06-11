<%@ Page Title="Log in" Language="C#" MasterPageFile="~/SiteLogin.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Prodata.WebForm.Account.Login" Async="true" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="LoginContent">
    <div class="card-body login-card-body">
        <p class="login-box-msg font-weight-bold">Sign in to start your session</p>

        <asp:PlaceHolder runat="server" ID="ErrorMessage" Visible="false">
            <p class="text-danger">
                <asp:Literal runat="server" ID="FailureText" />
            </p>
        </asp:PlaceHolder>

        <asp:Panel ID="pnlLogin" runat="server" DefaultButton="btnLogin">
            <div class="input-group mb-3">
                <asp:TextBox runat="server" ID="Username" CssClass="form-control" placeholder="Username" />
                <div class="input-group-append">
                    <div class="input-group-text">
                        <span class="fas fa-user"></span>
                    </div>
                </div>
            </div>
            <asp:RequiredFieldValidator runat="server" ControlToValidate="Username"
                CssClass="text-danger" ErrorMessage="Username is required." Display="Dynamic" />

            <div class="input-group mb-3">
                <asp:TextBox runat="server" ID="Password" TextMode="Password" CssClass="form-control" placeholder="Password" />
                <div class="input-group-append">
                    <div class="input-group-text">
                        <span class="fas fa-lock"></span>
                    </div>
                </div>
            </div>
            <asp:RequiredFieldValidator runat="server" ControlToValidate="Password"
                CssClass="text-danger" ErrorMessage="Password is required." Display="Dynamic" />

            <div class="row mt-3">
                <div class="col-8">
                    <div class="icheck-primary">
                        <asp:CheckBox runat="server" ID="RememberMe" />
                        <asp:Label runat="server" AssociatedControlID="RememberMe">Remember Me</asp:Label>
                    </div>
                </div>
                <div class="col-4">
                    <asp:Button ID="btnLogin" runat="server" OnClick="LogIn" Text="Log In"
                        CssClass="btn btn-primary btn-block" />
                </div>
            </div>
        </asp:Panel>
    </div>
</asp:Content>
