<%@ Page Title="Log in" Language="C#" MasterPageFile="~/SiteLogin.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Prodata.WebForm.Account.Login" Async="true" %>

<%@ Register Src="~/Account/OpenAuthProviders.ascx" TagPrefix="uc" TagName="OpenAuthProviders" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="LoginContent">
    <div class="card-body login-card-body">
        <p class="login-box-msg">Use a local account to log in.</p>

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
                CssClass="text-danger" ErrorMessage="The username field is required." Display="Dynamic" />

            <div class="input-group mb-3">
                <asp:TextBox runat="server" ID="Password" TextMode="Password" CssClass="form-control" placeholder="Password" />
                <div class="input-group-append">
                    <div class="input-group-text">
                        <span class="fas fa-lock"></span>
                    </div>
                </div>
            </div>
            <asp:RequiredFieldValidator runat="server" ControlToValidate="Password" CssClass="text-danger" ErrorMessage="The password field is required." Display="Dynamic" />

            <div class="row mt-3">
                <div class="col-7">
                    <div class="icheck-primary">
                        <asp:CheckBox runat="server" ID="RememberMe" />
                        <asp:Label runat="server" AssociatedControlID="RememberMe">Remember me?</asp:Label>
                    </div>
                </div>
                <div class="col-5">
                    <asp:Button ID="btnLogin" runat="server" OnClick="LogIn" Text="Log in" CssClass="btn btn-primary btn-block" />
                </div>
            </div>
        </asp:Panel>

        <%--<p class="mb-1">
            <asp:HyperLink runat="server" ID="RegisterHyperLink" ViewStateMode="Disabled">Register as a new user</asp:HyperLink>
        </p>
        <p>
            <%-- Enable this once you have account confirmation enabled for password reset functionality
            <asp:HyperLink runat="server" ID="ForgotPasswordHyperLink" ViewStateMode="Disabled">Forgot your password?</asp:HyperLink>
            --%
        </p>--%>
        <%--<uc:OpenAuthProviders runat="server" ID="OpenAuthLogin" />--%>
    </div>
</asp:Content>
