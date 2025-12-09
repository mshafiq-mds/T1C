<%@ Page Title="Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="Prodata.WebForm.Dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>

    <div class="form-group row mb-4 align-items-center">
        <asp:Label ID="lblBA" runat="server" CssClass="col-lg-2 col-sm-3 col-form-label fw-bold"
            AssociatedControlID="ddBA" Text="Business Area:"></asp:Label>

        <div class="col-lg-4 col-sm-6">
            <div class="input-group">
                <asp:DropDownList runat="server" ID="ddBA" CssClass="form-control form-select select2"
                    AutoPostBack="true" OnSelectedIndexChanged="ddBA_SelectedIndexChanged"
                    DataValueField="Code" DataTextField="DisplayName" />
                <div class="input-group-append">
                    <asp:LinkButton ID="btnClearBA" runat="server" CssClass="btn btn-outline-secondary"
                        OnClick="btnClearBA_Click" UseSubmitBehavior="false" ToolTip="Reset Filter">
                        <i class="fas fa-times"></i>
                    </asp:LinkButton>
                </div>
            </div>
        </div>

        <asp:Label ID="lblYear" runat="server" CssClass="col-lg-1 col-sm-3 col-form-label fw-bold text-lg-right"
            AssociatedControlID="ddYear" Text="Year:"></asp:Label>

        <div class="col-lg-2 col-sm-6">
            <asp:DropDownList runat="server" ID="ddYear" CssClass="form-control form-select"
                AutoPostBack="true" OnSelectedIndexChanged="ddYear_SelectedIndexChanged">
            </asp:DropDownList>
        </div>
    </div>

    <asp:UpdatePanel ID="UpdatePanelDashboard" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:HiddenField ID="hfModalCategory" runat="server" />
            <asp:HiddenField ID="hfModalStatus" runat="server" />

            <div class="container-fluid px-0">
                
                <div class="row mb-3 mt-4">
                    <div class="col-md-12"><h5 class="border-bottom pb-2 mb-3 text-secondary"><i class="fas fa-tachometer-alt text-primary me-2"></i> T1C Budget</h5></div>
                </div>
                
                <div class="row mb-4 g-3">
                    <div class="col-md-2 col-6th"><asp:LinkButton ID="BtnT1CSubmitted" runat="server" OnClick="Card_Click" CommandArgument="T1C|Submitted" CssClass="card-link"><div class="info-box bg-info"><span class="info-box-icon"><i class="fas fa-paper-plane"></i></span><div class="info-box-content"><span class="info-box-text">Submitted</span><asp:Label ID="LblT1CSubmitted" runat="server" CssClass="info-box-number">0</asp:Label></div></div></asp:LinkButton></div>
                    <div class="col-md-2 col-6th"><asp:LinkButton ID="BtnT1CReview" runat="server" OnClick="Card_Click" CommandArgument="T1C|Review" CssClass="card-link"><div class="info-box bg-warning"><span class="info-box-icon"><i class="fas fa-eye"></i></span><div class="info-box-content"><span class="info-box-text">Review</span><asp:Label ID="LblT1CReview" runat="server" CssClass="info-box-number">0</asp:Label></div></div></asp:LinkButton></div>
                    <div class="col-md-2 col-6th"><asp:LinkButton ID="BtnT1CResubmit" runat="server" OnClick="Card_Click" CommandArgument="T1C|Resubmit" CssClass="card-link"><div class="info-box bg-secondary"><span class="info-box-icon"><i class="fas fa-undo"></i></span><div class="info-box-content"><span class="info-box-text">Resubmit</span><asp:Label ID="LblT1CResubmit" runat="server" CssClass="info-box-number">0</asp:Label></div></div></asp:LinkButton></div>
                    <div class="col-md-2 col-6th"><asp:LinkButton ID="BtnT1CComplete" runat="server" OnClick="Card_Click" CommandArgument="T1C|Complete" CssClass="card-link"><div class="info-box bg-success"><span class="info-box-icon"><i class="fas fa-check-circle"></i></span><div class="info-box-content"><span class="info-box-text">Complete</span><asp:Label ID="LblT1CComplete" runat="server" CssClass="info-box-number">0</asp:Label></div></div></asp:LinkButton></div>
                    <div class="col-md-2 col-6th"><asp:LinkButton ID="BtnT1CDeleted" runat="server" OnClick="Card_Click" CommandArgument="T1C|Deleted" CssClass="card-link"><div class="info-box bg-danger"><span class="info-box-icon"><i class="fas fa-trash"></i></span><div class="info-box-content"><span class="info-box-text">Deleted</span><asp:Label ID="LblT1CDeleted" runat="server" CssClass="info-box-number">0</asp:Label></div></div></asp:LinkButton></div>
                    <div class="col-md-2 col-6th"><asp:LinkButton ID="BtnT1CFinalized" runat="server" OnClick="Card_Click" CommandArgument="T1C|Finalized" CssClass="card-link"><div class="info-box bg-dark"><span class="info-box-icon"><i class="fas fa-flag-checkered"></i></span><div class="info-box-content"><span class="info-box-text">Finalized</span><asp:Label ID="LblT1CFinalized" runat="server" CssClass="info-box-number">0</asp:Label></div></div></asp:LinkButton></div>
                </div>

                <div class="row mb-5">
                    <div class="col-md-12">
                        <div class="card shadow-sm">
                            <div class="card-body" style="height: 300px;">
                                <canvas id="chartT1C"></canvas>
                            </div>
                        </div>
                    </div>
                </div>


                <div class="row mb-3">
                    <div class="col-md-12"><h5 class="border-bottom pb-2 mb-3 text-secondary"><i class="fas fa-wallet text-info me-2"></i> Additional Budget Requests</h5></div>
                </div>

                <div class="row mb-4 g-3">
                    <div class="col-md-2 col-6th"><asp:LinkButton ID="BtnAddSubmitted" runat="server" OnClick="Card_Click" CommandArgument="Additional|Submitted" CssClass="card-link"><div class="info-box bg-info"><span class="info-box-icon"><i class="fas fa-paper-plane"></i></span><div class="info-box-content"><span class="info-box-text">Submitted</span><asp:Label ID="LblAdditionalSubmitted" runat="server" CssClass="info-box-number">0</asp:Label></div></div></asp:LinkButton></div>
                    <div class="col-md-2 col-6th"><asp:LinkButton ID="BtnAddReview" runat="server" OnClick="Card_Click" CommandArgument="Additional|Review" CssClass="card-link"><div class="info-box bg-warning"><span class="info-box-icon"><i class="fas fa-eye"></i></span><div class="info-box-content"><span class="info-box-text">Review</span><asp:Label ID="LblAdditionalReview" runat="server" CssClass="info-box-number">0</asp:Label></div></div></asp:LinkButton></div>
                    <div class="col-md-2 col-6th"><asp:LinkButton ID="BtnAddResubmit" runat="server" OnClick="Card_Click" CommandArgument="Additional|Resubmit" CssClass="card-link"><div class="info-box bg-secondary"><span class="info-box-icon"><i class="fas fa-undo"></i></span><div class="info-box-content"><span class="info-box-text">Resubmit</span><asp:Label ID="LblAdditionalResubmit" runat="server" CssClass="info-box-number">0</asp:Label></div></div></asp:LinkButton></div>
                    <div class="col-md-2 col-6th"><asp:LinkButton ID="BtnAddComplete" runat="server" OnClick="Card_Click" CommandArgument="Additional|Complete" CssClass="card-link"><div class="info-box bg-success"><span class="info-box-icon"><i class="fas fa-check-circle"></i></span><div class="info-box-content"><span class="info-box-text">Complete</span><asp:Label ID="LblAdditionalComplete" runat="server" CssClass="info-box-number">0</asp:Label></div></div></asp:LinkButton></div>
                    <div class="col-md-2 col-6th"><asp:LinkButton ID="BtnAddDeleted" runat="server" OnClick="Card_Click" CommandArgument="Additional|Deleted" CssClass="card-link"><div class="info-box bg-danger"><span class="info-box-icon"><i class="fas fa-trash"></i></span><div class="info-box-content"><span class="info-box-text">Deleted</span><asp:Label ID="LblAdditionalDeleted" runat="server" CssClass="info-box-number">0</asp:Label></div></div></asp:LinkButton></div>
                    <div class="col-md-2 col-6th"><asp:LinkButton ID="BtnAddFinalized" runat="server" OnClick="Card_Click" CommandArgument="Additional|Finalized" CssClass="card-link"><div class="info-box bg-dark"><span class="info-box-icon"><i class="fas fa-flag-checkered"></i></span><div class="info-box-content"><span class="info-box-text">Finalized</span><asp:Label ID="LblAdditionalFinalized" runat="server" CssClass="info-box-number">0</asp:Label></div></div></asp:LinkButton></div>
                </div>

                <div class="row mb-5">
                    <div class="col-md-12">
                        <div class="card shadow-sm">
                            <div class="card-body" style="height: 300px;">
                                <canvas id="chartAdd"></canvas>
                            </div>
                        </div>
                    </div>
                </div>


                <div class="row mb-3">
                    <div class="col-md-12"><h5 class="border-bottom pb-2 mb-3 text-secondary"><i class="fas fa-exchange-alt text-primary me-2"></i> Transfer Transactions</h5></div>
                </div>

                <div class="row g-3">
                    <div class="col-md-2 col-6th"><asp:LinkButton ID="BtnTransSubmitted" runat="server" OnClick="Card_Click" CommandArgument="Transfer|Submitted" CssClass="card-link"><div class="info-box bg-info"><span class="info-box-icon"><i class="fas fa-paper-plane"></i></span><div class="info-box-content"><span class="info-box-text">Submitted</span><asp:Label ID="LblTransferSubmitted" runat="server" CssClass="info-box-number">0</asp:Label></div></div></asp:LinkButton></div>
                    <div class="col-md-2 col-6th"><asp:LinkButton ID="BtnTransReview" runat="server" OnClick="Card_Click" CommandArgument="Transfer|Review" CssClass="card-link"><div class="info-box bg-warning"><span class="info-box-icon"><i class="fas fa-eye"></i></span><div class="info-box-content"><span class="info-box-text">Review</span><asp:Label ID="LblTransferReview" runat="server" CssClass="info-box-number">0</asp:Label></div></div></asp:LinkButton></div>
                    <div class="col-md-2 col-6th"><asp:LinkButton ID="BtnTransResubmit" runat="server" OnClick="Card_Click" CommandArgument="Transfer|Resubmit" CssClass="card-link"><div class="info-box bg-secondary"><span class="info-box-icon"><i class="fas fa-undo"></i></span><div class="info-box-content"><span class="info-box-text">Resubmit</span><asp:Label ID="LblTransferResubmit" runat="server" CssClass="info-box-number">0</asp:Label></div></div></asp:LinkButton></div>
                    <div class="col-md-2 col-6th"><asp:LinkButton ID="BtnTransComplete" runat="server" OnClick="Card_Click" CommandArgument="Transfer|Complete" CssClass="card-link"><div class="info-box bg-success"><span class="info-box-icon"><i class="fas fa-check-circle"></i></span><div class="info-box-content"><span class="info-box-text">Complete</span><asp:Label ID="LblTransferComplete" runat="server" CssClass="info-box-number">0</asp:Label></div></div></asp:LinkButton></div>
                    <div class="col-md-2 col-6th"><asp:LinkButton ID="BtnTransDeleted" runat="server" OnClick="Card_Click" CommandArgument="Transfer|Deleted" CssClass="card-link"><div class="info-box bg-danger"><span class="info-box-icon"><i class="fas fa-trash"></i></span><div class="info-box-content"><span class="info-box-text">Deleted</span><asp:Label ID="LblTransferDeleted" runat="server" CssClass="info-box-number">0</asp:Label></div></div></asp:LinkButton></div>
                    <div class="col-md-2 col-6th"><asp:LinkButton ID="BtnTransFinalized" runat="server" OnClick="Card_Click" CommandArgument="Transfer|Finalized" CssClass="card-link"><div class="info-box bg-dark"><span class="info-box-icon"><i class="fas fa-flag-checkered"></i></span><div class="info-box-content"><span class="info-box-text">Finalized</span><asp:Label ID="LblTransferFinalized" runat="server" CssClass="info-box-number">0</asp:Label></div></div></asp:LinkButton></div>
                </div>

                <div class="row mb-5 mt-4">
                    <div class="col-md-12">
                        <div class="card shadow-sm">
                            <div class="card-body" style="height: 300px;">
                                <canvas id="chartTrans"></canvas>
                            </div>
                        </div>
                    </div>
                </div>

            </div>

            <div class="modal fade" id="detailModal" tabindex="-1" role="dialog" aria-hidden="true">
               <div class="modal-dialog modal-xl modal-dialog-centered modal-dialog-scrollable" role="document">
                    <div class="modal-content">
                        <div class="modal-header bg-primary text-white">
                            <h5 class="modal-title"><asp:Label ID="lblModalTitle" runat="server" Text="Details"></asp:Label></h5>
                            <button type="button" class="close text-white" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                        </div>
                        <div class="modal-body">
                            <div class="row mb-3"><div class="col-md-6 col-sm-8"><div class="input-group"><asp:TextBox ID="txtSearchDetail" runat="server" CssClass="form-control" placeholder="Search by Reference No..."></asp:TextBox><div class="input-group-append"><asp:LinkButton ID="btnSearchDetail" runat="server" CssClass="btn btn-primary" OnClick="btnSearchDetail_Click"><i class="fas fa-search"></i></asp:LinkButton></div></div></div></div>
                            <div class="table-responsive"><asp:GridView ID="gvDetails" runat="server" CssClass="table table-striped table-hover table-bordered table-sm" AutoGenerateColumns="true" EmptyDataText="No records found." GridLines="None"><HeaderStyle CssClass="thead-dark" /></asp:GridView></div>
                        </div>
                        <div class="modal-footer"><button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button></div>
                    </div>
                </div>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>

    <script type="text/javascript">
        function openDetailModal() {
            $('.modal-backdrop').remove();
            $('body').removeClass('modal-open');
            $('#detailModal').modal('show');
        }

        // Global variables to hold chart instances (so we can destroy them before re-creating)
        var myChartT1C = null;
        var myChartAdd = null;
        var myChartTrans = null;

        function renderDashboardCharts(t1cData, addData, transData) {
            // Colors matching your cards (Info, Warning, Secondary, Success, Danger, Dark)
            var bgColors = [
                'rgba(23, 162, 184, 0.7)',  // Info (Submitted)
                'rgba(255, 193, 7, 0.7)',   // Warning (Review)
                'rgba(108, 117, 125, 0.7)', // Secondary (Resubmit)
                'rgba(40, 167, 69, 0.7)',   // Success (Complete)
                'rgba(220, 53, 69, 0.7)',   // Danger (Deleted)
                'rgba(52, 58, 64, 0.7)'     // Dark (Finalized)
            ];

            var borderColors = [
                'rgba(23, 162, 184, 1)', 'rgba(255, 193, 7, 1)', 'rgba(108, 117, 125, 1)',
                'rgba(40, 167, 69, 1)', 'rgba(220, 53, 69, 1)', 'rgba(52, 58, 64, 1)'
            ];

            var labels = ['Submitted', 'Review', 'Resubmit', 'Complete', 'Deleted', 'Finalized'];

            // Function to build/update a specific chart
            function createChart(canvasId, dataValues, label, existingChartInstance) {
                var ctx = document.getElementById(canvasId).getContext('2d');
                if (existingChartInstance) { existingChartInstance.destroy(); }

                return new Chart(ctx, {
                    type: 'bar',
                    data: {
                        labels: labels,
                        datasets: [{
                            label: label,
                            data: dataValues,
                            backgroundColor: bgColors,
                            borderColor: borderColors,
                            borderWidth: 1
                        }]
                    },
                    options: {
                        responsive: true,
                        maintainAspectRatio: false,
                        scales: {
                            y: { beginAtZero: true, ticks: { precision: 0 } }
                        },
                        plugins: {
                            legend: { display: false } // Hide legend since colors explain it
                        }
                    }
                });
            }

            // Render all 3 charts
            if (document.getElementById('chartT1C'))
                myChartT1C = createChart('chartT1C', t1cData, 'T1C Budget Count', myChartT1C);

            if (document.getElementById('chartAdd'))
                myChartAdd = createChart('chartAdd', addData, 'Additional Budget Count', myChartAdd);

            if (document.getElementById('chartTrans'))
                myChartTrans = createChart('chartTrans', transData, 'Transfer Count', myChartTrans);
        }
    </script>

    <style>
        /* ... existing styles ... */
        .select2-selection__clear { display: none !important; }
        .col-6th { flex: 0 0 16.66%; max-width: 16.66%; }
        /* ... etc ... */
    </style>
</asp:Content>