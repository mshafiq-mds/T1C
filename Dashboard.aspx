<%@ Page Title="Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="Prodata.WebForm.Dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-4">
        <h2 class="mb-4">Dashboard Overview</h2>

        <div class="row mb-4">
            <div class="col-md-4">
                <div class="card text-white bg-primary shadow">
                    <div class="card-body">
                        <h5 class="card-title">T1C Forms Total</h5>
                        <asp:Label ID="LblT1CTotal" runat="server" CssClass="display-4"></asp:Label>
                    </div>
                </div>
            </div>
        </div>

        <!-- Additional Budget Requests -->
        <h4 class="mt-5">Additional Budget Requests</h4>
        <div class="row">
            <div class="col-md-2">
                <div class="card bg-info text-white mb-3 shadow">
                    <div class="card-body">
                        <h6>Submitted</h6>
                        <asp:Label ID="LblAdditionalSubmitted" runat="server" CssClass="h4"></asp:Label>
                    </div>
                </div>
            </div>
            <div class="col-md-2">
                <div class="card bg-warning text-white mb-3 shadow">
                    <div class="card-body">
                        <h6>Review</h6>
                        <asp:Label ID="LblAdditionalReview" runat="server" CssClass="h4"></asp:Label>
                    </div>
                </div>
            </div>
            <div class="col-md-2">
                <div class="card bg-secondary text-white mb-3 shadow">
                    <div class="card-body">
                        <h6>Resubmit</h6>
                        <asp:Label ID="LblAdditionalResubmit" runat="server" CssClass="h4"></asp:Label>
                    </div>
                </div>
            </div>
            <div class="col-md-2">
                <div class="card bg-success text-white mb-3 shadow">
                    <div class="card-body">
                        <h6>Complete</h6>
                        <asp:Label ID="LblAdditionalComplete" runat="server" CssClass="h4"></asp:Label>
                    </div>
                </div>
            </div>
            <div class="col-md-2">
                <div class="card bg-danger text-white mb-3 shadow">
                    <div class="card-body">
                        <h6>Deleted</h6>
                        <asp:Label ID="LblAdditionalDeleted" runat="server" CssClass="h4"></asp:Label>
                    </div>
                </div>
            </div>
        </div>

        <!-- Transfer Transactions -->
        <h4 class="mt-5">Transfer Transactions</h4>
        <div class="row">
            <div class="col-md-2">
                <div class="card bg-info text-white mb-3 shadow">
                    <div class="card-body">
                        <h6>Submitted</h6>
                        <asp:Label ID="LblTransferSubmitted" runat="server" CssClass="h4"></asp:Label>
                    </div>
                </div>
            </div>
            <div class="col-md-2">
                <div class="card bg-warning text-white mb-3 shadow">
                    <div class="card-body">
                        <h6>Review</h6>
                        <asp:Label ID="LblTransferReview" runat="server" CssClass="h4"></asp:Label>
                    </div>
                </div>
            </div>
            <div class="col-md-2">
                <div class="card bg-secondary text-white mb-3 shadow">
                    <div class="card-body">
                        <h6>Resubmit</h6>
                        <asp:Label ID="LblTransferResubmit" runat="server" CssClass="h4"></asp:Label>
                    </div>
                </div>
            </div>
            <div class="col-md-2">
                <div class="card bg-success text-white mb-3 shadow">
                    <div class="card-body">
                        <h6>Complete</h6>
                        <asp:Label ID="LblTransferComplete" runat="server" CssClass="h4"></asp:Label>
                    </div>
                </div>
            </div>
            <div class="col-md-2">
                <div class="card bg-danger text-white mb-3 shadow">
                    <div class="card-body">
                        <h6>Deleted</h6>
                        <asp:Label ID="LblTransferDeleted" runat="server" CssClass="h4"></asp:Label>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <%--disable scrolling--%>
    <style> 
        html, body {
            height: 100%;
        }

        body.dashboard-lock {
            overflow: hidden; /* Disable scrolling */
        }

        .container-fixed-dashboard {
            height: 100%;
            overflow: hidden;
        }

        .dashboard-scroll-area {
            overflow: auto;
            flex-grow: 1;
        }
    </style>
</asp:Content>
