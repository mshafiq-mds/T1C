<%@ Page Title="404" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="404.aspx.cs" Inherits="Prodata.WebForm.Error._404" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style type="text/css">
        .error-page>.headline {
            font-size: 200px;
        }
        .error-page>.error-content {
            margin-left: 320px;
            padding-top: 60px;
        }
        @media (max-width:767px) {
            .error-page>.headline {
                font-size: 100px;
            }
            .error-page>.error-content {
                margin-left: 0;
                padding-top: 0;
            }
            .error-page>.error-content p {
                text-align: center;
            }
        }
    </style>
    <div class="error-page">
        <h2 class="headline text-warning">404</h2>
        <div class="error-content">
            <h3><i class="fas fa-exclamation-triangle text-warning"></i>Oops! Page not found.</h3>

            <p>
                We could not find the page you were looking for.
                Meanwhile, you may <a href="~/" runat="server">return to home</a>.
            </p>
        </div>
        <!-- /.error-content -->
    </div>
</asp:Content>
