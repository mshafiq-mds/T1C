﻿<%@ Page Title="Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="Prodata.WebForm.Dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container-fluid mt-4">
        <div class="row mb-4">
            <div class="col-md-12">
                <div class="card bg-secondary shadow-sm">
                    <div class="card-body">
                        <h3 class="mb-0 text-white"><i class="fas fa-tachometer-alt me-2"></i> Dashboard Overview</h3>
                    </div>
                </div>
            </div>
        </div>

        <!-- T1C Forms -->
        <div class="row mb-5">
            <div class="col-md-4">
                <div class="small-box bg-primary shadow">
                    <div class="inner text-white">
                        <h4>T1C Forms Total</h4>
                        <h2><asp:Label ID="LblT1CTotal" runat="server" CssClass="fw-bold"></asp:Label></h2>
                    </div>
                    <div class="icon">
                        <i class="fas fa-file-alt"></i>
                    </div>
                </div>
            </div>
        </div>

        <!-- Additional Budget Section -->
        <div class="row mb-3">
            <div class="col-md-12">
                <h4 class="border-bottom pb-2"><i class="fas fa-wallet text-info me-2"></i>    Additional Budget Requests</h4>
            </div>
        </div>
        <div class="row mb-4 g-3">
            <div class="col-md-2">
                <div class="info-box bg-info">
                    <span class="info-box-icon"><i class="fas fa-paper-plane"></i></span>
                    <div class="info-box-content">
                        <span class="info-box-text">Submitted</span>
                        <asp:Label ID="LblAdditionalSubmitted" runat="server" CssClass="info-box-number"></asp:Label>
                    </div>
                </div>
            </div>
            <div class="col-md-2">
                <div class="info-box bg-warning">
                    <span class="info-box-icon"><i class="fas fa-eye"></i></span>
                    <div class="info-box-content">
                        <span class="info-box-text">Review</span>
                        <asp:Label ID="LblAdditionalReview" runat="server" CssClass="info-box-number"></asp:Label>
                    </div>
                </div>
            </div>
            <div class="col-md-2">
                <div class="info-box bg-secondary">
                    <span class="info-box-icon"><i class="fas fa-undo"></i></span>
                    <div class="info-box-content">
                        <span class="info-box-text">Resubmit</span>
                        <asp:Label ID="LblAdditionalResubmit" runat="server" CssClass="info-box-number"></asp:Label>
                    </div>
                </div>
            </div>
            <div class="col-md-2">
                <div class="info-box bg-success">
                    <span class="info-box-icon"><i class="fas fa-check-circle"></i></span>
                    <div class="info-box-content">
                        <span class="info-box-text">Complete</span>
                        <asp:Label ID="LblAdditionalComplete" runat="server" CssClass="info-box-number"></asp:Label>
                    </div>
                </div>
            </div>
            <div class="col-md-2">
                <div class="info-box bg-danger">
                    <span class="info-box-icon"><i class="fas fa-trash"></i></span>
                    <div class="info-box-content">
                        <span class="info-box-text">Deleted</span>
                        <asp:Label ID="LblAdditionalDeleted" runat="server" CssClass="info-box-number"></asp:Label>
                    </div>
                </div>
            </div>
            <div class="col-md-2">
                <div class="info-box bg-dark">
                    <span class="info-box-icon"><i class="fas fa-flag-checkered"></i></span>
                    <div class="info-box-content">
                        <span class="info-box-text">Finalized</span>
                        <asp:Label ID="LblAdditionalFinalized" runat="server" CssClass="info-box-number"></asp:Label>
                    </div>
                </div>
            </div>
        </div>

        <!-- Transfer Transactions Section -->
        <div class="row mb-3">
            <div class="col-md-12">
                <h4 class="border-bottom pb-2"><i class="fas fa-exchange-alt text-primary me-2"></i>    Transfer Transactions</h4>
            </div>
        </div>
        <div class="row g-3">
            <div class="col-md-2">
                <div class="info-box bg-info">
                    <span class="info-box-icon"><i class="fas fa-paper-plane"></i></span>
                    <div class="info-box-content">
                        <span class="info-box-text">Submitted</span>
                        <asp:Label ID="LblTransferSubmitted" runat="server" CssClass="info-box-number"></asp:Label>
                    </div>
                </div>
            </div>
            <div class="col-md-2">
                <div class="info-box bg-warning">
                    <span class="info-box-icon"><i class="fas fa-eye"></i></span>
                    <div class="info-box-content">
                        <span class="info-box-text">Review</span>
                        <asp:Label ID="LblTransferReview" runat="server" CssClass="info-box-number"></asp:Label>
                    </div>
                </div>
            </div>
            <div class="col-md-2">
                <div class="info-box bg-secondary">
                    <span class="info-box-icon"><i class="fas fa-undo"></i></span>
                    <div class="info-box-content">
                        <span class="info-box-text">Resubmit</span>
                        <asp:Label ID="LblTransferResubmit" runat="server" CssClass="info-box-number"></asp:Label>
                    </div>
                </div>
            </div>
            <div class="col-md-2">
                <div class="info-box bg-success">
                    <span class="info-box-icon"><i class="fas fa-check-circle"></i></span>
                    <div class="info-box-content">
                        <span class="info-box-text">Complete</span>
                        <asp:Label ID="LblTransferComplete" runat="server" CssClass="info-box-number"></asp:Label>
                    </div>
                </div>
            </div>
            <div class="col-md-2">
                <div class="info-box bg-danger">
                    <span class="info-box-icon"><i class="fas fa-trash"></i></span>
                    <div class="info-box-content">
                        <span class="info-box-text">Deleted</span>
                        <asp:Label ID="LblTransferDeleted" runat="server" CssClass="info-box-number"></asp:Label>
                    </div>
                </div>
            </div>
            <div class="col-md-2">
                <div class="info-box bg-dark">
                    <span class="info-box-icon"><i class="fas fa-flag-checkered"></i></span>
                    <div class="info-box-content">
                        <span class="info-box-text">Finalized</span>
                        <asp:Label ID="LblTransferFinalized" runat="server" CssClass="info-box-number"></asp:Label>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
