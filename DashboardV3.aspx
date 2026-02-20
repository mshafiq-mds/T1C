<%@ Page Title="Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DashboardV3.aspx.cs" Inherits="Prodata.WebForm.DashboardV3" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" />
    <script src="https://cdn.jsdelivr.net/npm/chart.js@4.4.1/dist/chart.umd.min.js"></script>

    <div class="dashboard-container p-3 p-md-4">
        <div class="d-flex flex-column flex-md-row justify-content-between align-items-start align-items-md-center mb-4 gap-3">
            <div> 
                <h3 class="mb-0 text-dark font-weight-bold">CCMS Management Dashboard</h3>
            </div>
            
            <div class="d-flex flex-wrap align-items-center gap-2 w-200 w-md-auto">
                <div class="filter-group d-flex align-items-center bg-white rounded shadow-sm px-3 py-1 border flex-grow-1 flex-md-grow-0">
                    <span class="text-secondary small font-weight-bold me-2">Business Area:</span>
                    <asp:DropDownList runat="server" ID="ddBA" CssClass="form-select border-0 shadow-none font-weight-bold" style="width: auto; min-width: 120px;"
                        AutoPostBack="true" OnSelectedIndexChanged="ddBA_SelectedIndexChanged"
                        DataValueField="Code" DataTextField="DisplayName" />
                </div>

                <div class="filter-group d-flex align-items-center bg-white rounded shadow-sm px-3 py-1 border flex-grow-1 flex-md-grow-0">
                    <span class="text-secondary small font-weight-bold me-2">Year:</span>
                    <asp:DropDownList runat="server" ID="ddYear" CssClass="form-select border-0 shadow-none font-weight-bold" style="width: auto; min-width: 80px;"
                        AutoPostBack="true" OnSelectedIndexChanged="ddYear_SelectedIndexChanged">
                    </asp:DropDownList>
                </div>
            </div>
        </div>

        <asp:UpdatePanel ID="UpdatePanelDashboard" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:HiddenField ID="hfModalCategory" runat="server" />
                <asp:HiddenField ID="hfModalStatus" runat="server" />
                <asp:Button ID="btnTriggerChartClick" runat="server" OnClick="Card_Click" style="display:none;" />
                <asp:HiddenField ID="hfChartDataT1C" runat="server" Value="[]" />
                <asp:HiddenField ID="hfChartDataT1COthers" runat="server" Value="[]" />
                <asp:HiddenField ID="hfChartDataAdd" runat="server" Value="[]" />
                <asp:HiddenField ID="hfChartDataTrans" runat="server" Value="[]" />
                <asp:HiddenField ID="hfChartDataDoughnut" runat="server" Value="{}" /> 

                <div class="row g-3 mb-4">
                    <div class="col-12 col-md-4">
                        <div class="card kpi-card border-0 shadow-sm card-blue h-100">
                            <div class="card-body p-3 p-xl-4 d-flex align-items-center">
                                <div class="icon-box bg-white text-primary rounded-circle me-3">
                                    <i class="fas fa-dollar-sign"></i>
                                </div>
                                <div>
                                    <div class="text-label">Total T1C Request</div>
                                    <div class="d-flex align-items-center">
                                        <h2 class="mb-0 font-weight-bold"><asp:Label ID="lblTotalBudget" runat="server" Text="RM 0.00"></asp:Label></h2>
                                    </div>
                                    <div class="sub-label">Allocated across 4 Category</div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-12 col-md-4">
                        <div class="card kpi-card border-0 shadow-sm card-green h-100">
                            <div class="card-body p-3 p-xl-4 d-flex align-items-center">
                                <div class="icon-box bg-white text-success rounded-circle me-3">
                                    <i class="fas fa-percentage"></i>
                                </div>
                                <div>
                                    <div class="text-label">Overall Approvals Completion Rate</div>
                                    <div class="d-flex align-items-center flex-wrap">
                                        <h2 class="mb-0 font-weight-bold"><asp:Label ID="lblCompletionRate" runat="server" Text="0.0%"></asp:Label></h2>
                                    </div>
                                    <div class="sub-label">Compared to last quarter</div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-12 col-md-4">
                        <div class="card kpi-card border-0 shadow-sm card-yellow h-100">
                            <div class="card-body p-3 p-xl-4 d-flex align-items-center">
                                <div class="icon-box bg-white text-warning rounded-circle me-3">
                                    <i class="far fa-clock"></i>
                                </div>
                                <div>
                                    <div class="text-label">Pending Approvals</div>
                                    <h2 class="mb-0 font-weight-bold"><asp:Label ID="lblPendingApprovals" runat="server" Text="0"></asp:Label></h2>
                                    <div class="sub-label">requires immediate attention</div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <h5 class="section-title mb-3 mt-4">Category Status Overview</h5>
                
                <div class="row g-3 mb-4">
                    <div class="col-12 col-md-6 col-xl-3">
                        <div class="card status-card border-0 shadow-sm h-100">
                            <div class="card-header bg-white border-0 py-3"><h6 class="mb-0 font-weight-bold">T1C Budget</h6></div>
                            <div class="card-body py-0">
                                <asp:LinkButton ID="BtnT1CSubmitted" runat="server" OnClick="Card_Click" CommandArgument="T1C|Submitted" CssClass="status-item active-row">
                                    <span><i class="fas fa-paper-plane me-2 text-primary"></i> Submitted</span>
                                    <span class="fw-bold"><asp:Label ID="LblT1CSubmitted" runat="server" Text="0" /> <i class="fas fa-chevron-right ms-2 text-primary small opacity-75"></i></span>
                                </asp:LinkButton><asp:LinkButton ID="BtnT1CReview" runat="server" OnClick="Card_Click" CommandArgument="T1C|Under Review" CssClass="status-item">
                                    <span><i class="fas fa-hourglass-half me-2 text-warning"></i> Under Review</span>
                                    <span class="fw-bold"><asp:Label ID="LblT1CReview" runat="server" Text="0" /> <i class="fas fa-chevron-right ms-2 text-warning small opacity-75"></i></span>
                                </asp:LinkButton><asp:LinkButton ID="BtnT1CResubmit" runat="server" OnClick="Card_Click" CommandArgument="T1C|Resubmit" CssClass="status-item">
                                    <span><i class="fas fa-rotate-left me-2 text-secondary"></i> Resubmit</span>
                                    <span class="fw-bold"><asp:Label ID="LblT1CResubmit" runat="server" Text="0" /> <i class="fas fa-chevron-right ms-2 text-secondary small opacity-75"></i></span>
                                </asp:LinkButton><asp:LinkButton ID="BtnT1CComplete" runat="server" OnClick="Card_Click" CommandArgument="T1C|Approved" CssClass="status-item">
                                    <span><i class="fas fa-circle-check me-2 text-info"></i> Approved</span>
                                    <span class="fw-bold"><asp:Label ID="LblT1CComplete" runat="server" Text="0" /> <i class="fas fa-chevron-right ms-2 text-info small opacity-75"></i></span>
                                </asp:LinkButton><asp:LinkButton ID="BtnT1CFinalized" runat="server" OnClick="Card_Click" CommandArgument="T1C|Completed" CssClass="status-item">
                                    <span><i class="fas fa-flag-checkered me-2 text-success"></i> Completed</span>
                                    <span class="fw-bold"><asp:Label ID="LblT1CFinalized" runat="server" Text="0" /> <i class="fas fa-chevron-right ms-2 text-success small opacity-75"></i></span>
                                </asp:LinkButton><asp:LinkButton ID="BtnT1CDeleted" runat="server" OnClick="Card_Click" CommandArgument="T1C|Deleted" CssClass="status-item border-0">
                                    <span><i class="fas fa-trash-can me-2 text-danger"></i> Deleted</span>
                                    <span class="fw-bold"><asp:Label ID="LblT1CDeleted" runat="server" Text="0" /> <i class="fas fa-chevron-right ms-2 text-danger small opacity-75"></i></span>
                                </asp:LinkButton></div></div></div><div class="col-12 col-md-6 col-xl-3">
                        <div class="card status-card border-0 shadow-sm h-100">
                            <div class="card-header bg-white border-0 py-3"><h6 class="mb-0 font-weight-bold">T1C Others</h6></div><div class="card-body py-0">
                                <asp:LinkButton ID="BtnT1COthersSubmitted" runat="server" OnClick="Card_Click" CommandArgument="T1COthers|Submitted" CssClass="status-item active-row">
                                    <span><i class="fas fa-paper-plane me-2 text-primary"></i> Submitted</span>
                                    <span class="fw-bold"><asp:Label ID="LblT1COthersSubmitted" runat="server" Text="0" /> <i class="fas fa-chevron-right ms-2 text-primary small opacity-75"></i></span>
                                </asp:LinkButton><asp:LinkButton ID="BtnT1COthersReview" runat="server" OnClick="Card_Click" CommandArgument="T1COthers|Under Review" CssClass="status-item ">
                                    <span><i class="fas fa-hourglass-half me-2 text-warning"></i> Under Review</span>
                                    <span class="fw-bold"><asp:Label ID="LblT1COthersReview" runat="server" Text="0" /> <i class="fas fa-chevron-right ms-2 text-warning small opacity-75"></i></span>
                                </asp:LinkButton><asp:LinkButton ID="BtnT1COthersResubmit" runat="server" OnClick="Card_Click" CommandArgument="T1COthers|Resubmit" CssClass="status-item">
                                    <span><i class="fas fa-rotate-left me-2 text-secondary"></i> Resubmit</span>
                                    <span class="fw-bold"><asp:Label ID="LblT1COthersResubmit" runat="server" Text="0" /> <i class="fas fa-chevron-right ms-2 text-secondary small opacity-75"></i></span>
                                </asp:LinkButton><asp:LinkButton ID="BtnT1COthersComplete" runat="server" OnClick="Card_Click" CommandArgument="T1COthers|Approved" CssClass="status-item">
                                    <span><i class="fas fa-circle-check me-2 text-info"></i> Approved</span>
                                    <span class="fw-bold"><asp:Label ID="LblT1COthersComplete" runat="server" Text="0" /> <i class="fas fa-chevron-right ms-2 text-info small opacity-75"></i></span>
                                </asp:LinkButton><asp:LinkButton ID="BtnT1COthersDeleted" runat="server" OnClick="Card_Click" CommandArgument="T1COthers|Deleted" CssClass="status-item border-0">
                                    <span><i class="fas fa-trash-can me-2 text-danger"></i> Deleted</span>
                                    <span class="fw-bold"><asp:Label ID="LblT1COthersDeleted" runat="server" Text="0" /> <i class="fas fa-chevron-right ms-2 text-danger small opacity-75"></i></span>
                                </asp:LinkButton></div></div></div><div class="col-12 col-md-6 col-xl-3">
                        <div class="card status-card border-0 shadow-sm h-100">
                            <div class="card-header bg-white border-0 py-3"><h6 class="mb-0 font-weight-bold">Additional Budget</h6></div><div class="card-body py-0">
                                <asp:LinkButton ID="BtnAddSubmitted" runat="server" OnClick="Card_Click" CommandArgument="Additional|Submitted" CssClass="status-item active-row">
                                    <span><i class="fas fa-paper-plane me-2 text-primary"></i> Submitted</span>
                                    <span class="fw-bold"><asp:Label ID="LblAdditionalSubmitted" runat="server" Text="0" /> <i class="fas fa-chevron-right ms-2 text-primary small opacity-75"></i></span>
                                </asp:LinkButton><asp:LinkButton ID="BtnAddReview" runat="server" OnClick="Card_Click" CommandArgument="Additional|Under Review" CssClass="status-item">
                                    <span><i class="fas fa-hourglass-half me-2 text-warning"></i> Under Review</span>
                                    <span class="fw-bold"><asp:Label ID="LblAdditionalReview" runat="server" Text="0" /> <i class="fas fa-chevron-right ms-2 text-warning small opacity-75"></i></span>
                                </asp:LinkButton><asp:LinkButton ID="BtnAddResubmit" runat="server" OnClick="Card_Click" CommandArgument="Additional|Resubmit" CssClass="status-item">
                                    <span><i class="fas fa-rotate-left me-2 text-secondary"></i> Resubmit</span>
                                    <span class="fw-bold"><asp:Label ID="LblAdditionalResubmit" runat="server" Text="0" /> <i class="fas fa-chevron-right ms-2 text-secondary small opacity-75"></i></span>
                                </asp:LinkButton><asp:LinkButton ID="BtnAddComplete" runat="server" OnClick="Card_Click" CommandArgument="Additional|Approved" CssClass="status-item">
                                    <span><i class="fas fa-circle-check me-2 text-info"></i> Approved</span>
                                    <span class="fw-bold"><asp:Label ID="LblAdditionalComplete" runat="server" Text="0" /> <i class="fas fa-chevron-right ms-2 text-info small opacity-75"></i></span>
                                </asp:LinkButton><asp:LinkButton ID="BtnAddDeleted" runat="server" OnClick="Card_Click" CommandArgument="Additional|Deleted" CssClass="status-item border-0">
                                    <span><i class="fas fa-trash-can me-2 text-danger"></i> Deleted</span>
                                    <span class="fw-bold"><asp:Label ID="LblAdditionalDeleted" runat="server" Text="0" /> <i class="fas fa-chevron-right ms-2 text-danger small opacity-75"></i></span>
                                </asp:LinkButton></div></div></div><div class="col-12 col-md-6 col-xl-3">
                        <div class="card status-card border-0 shadow-sm h-100">
                            <div class="card-header bg-white border-0 py-3"><h6 class="mb-0 font-weight-bold">Transfer Transactions</h6></div><div class="card-body py-0">
                                <asp:LinkButton ID="BtnTransSubmitted" runat="server" OnClick="Card_Click" CommandArgument="Transfer|SUBMITTED" CssClass="status-item active-row">
                                    <span><i class="fas fa-paper-plane me-2 text-primary"></i> Submitted</span>
                                    <span class="fw-bold"><asp:Label ID="LblTransferSubmitted" runat="server" Text="0" /> <i class="fas fa-chevron-right ms-2 text-primary small opacity-75"></i></span>
                                </asp:LinkButton><asp:LinkButton ID="BtnTransReview" runat="server" OnClick="Card_Click" CommandArgument="Transfer|BUDGET ALLOCATE" CssClass="status-item">
                                    <span><i class="fas fa-wallet me-2 text-warning"></i> Budget Allocate</span>
                                    <span class="fw-bold"><asp:Label ID="LblTransferReview" runat="server" Text="0" /> <i class="fas fa-chevron-right ms-2 text-warning small opacity-75"></i></span>
                                </asp:LinkButton><asp:LinkButton ID="BtnTransResubmit" runat="server" OnClick="Card_Click" CommandArgument="Transfer|UNDER REVIEW" CssClass="status-item">
                                    <span><i class="fas fa-hourglass-half me-2 text-warning"></i> Under Review</span>
                                    <span class="fw-bold"><asp:Label ID="LblTransferResubmit" runat="server" Text="0" /> <i class="fas fa-chevron-right ms-2 text-warning small opacity-75"></i></span>
                                </asp:LinkButton><asp:LinkButton ID="BtnTransComplete" runat="server" OnClick="Card_Click" CommandArgument="Transfer|RESUBMIT" CssClass="status-item">
                                    <span><i class="fas fa-rotate-left me-2 text-secondary"></i> Resubmit</span>
                                    <span class="fw-bold"><asp:Label ID="LblTransferComplete" runat="server" Text="0" /> <i class="fas fa-chevron-right ms-2 text-secondary small opacity-75"></i></span>
                                </asp:LinkButton><asp:LinkButton ID="BtnTransFinalized" runat="server" OnClick="Card_Click" CommandArgument="Transfer|COMPLETED" CssClass="status-item">
                                    <span><i class="fas fa-flag-checkered me-2 text-success"></i> Completed</span>
                                    <span class="fw-bold"><asp:Label ID="LblTransferFinalized" runat="server" Text="0" /> <i class="fas fa-chevron-right ms-2 text-success small opacity-75"></i></span>
                                </asp:LinkButton><asp:LinkButton ID="BtnTransDeleted" runat="server" OnClick="Card_Click" CommandArgument="Transfer|DELETED" CssClass="status-item border-0">
                                    <span><i class="fas fa-trash-can me-2 text-danger"></i> Deleted</span>
                                    <span class="fw-bold"><asp:Label ID="LblTransferDeleted" runat="server" Text="0" /> <i class="fas fa-chevron-right ms-2 text-danger small opacity-75"></i></span>
                                </asp:LinkButton></div></div></div></div><h5 class="section-title mb-3">Analytics Overview</h5><div class="row g-4 mb-4">
                    <div class="col-12 col-xl-7">
                        <div class="card border-0 shadow-sm chart-card h-100">
                            <div class="card-header bg-white border-0 py-3">
                                <h6 class="mb-0 font-weight-bold">Budget Submission vs. Completion by Category</h6></div><div class="card-body">
                                <div style="position: relative; height: 320px; width: 100%;">
                                    <canvas id="chartGroupedBar"></canvas></div></div></div></div><div class="col-12 col-xl-5">
                        <div class="card border-0 shadow-sm chart-card h-100">
                            <div class="card-header bg-white border-0 py-3">
                                <h6 class="mb-0 font-weight-bold">Top 10 T1C Amount by Business Area</h6></div><div class="card-body">
                                <div style="position: relative; height: 320px; width: 100%;">
                                    <canvas id="chartDoughnut"></canvas></div></div></div></div></div><div class="card border-0 shadow-sm">
                    <div class="card-header bg-white py-3 border-bottom">
                        <h6 class="mb-0 font-weight-bold">Recent Activity / Details</h6></div><div class="card-body p-0">
                        <div class="table-responsive">
                            <asp:GridView ID="gvRecentActivity" runat="server" CssClass="table table-hover align-middle mb-0 text-nowrap" 
                                AutoGenerateColumns="false" EmptyDataText="No recent activity found." GridLines="None">
                                <HeaderStyle CssClass="bg-light-header text-secondary small text-uppercase fw-bold" />
                                <Columns>
                                    <asp:BoundField DataField="RefNo" HeaderText="REF NO" ItemStyle-CssClass="ps-4 fw-bold text-primary" />
                                    <asp:BoundField DataField="Date" HeaderText="DATE" DataFormatString="{0:yyyy-MM-dd}" />
                                    <asp:BoundField DataField="BA" HeaderText="BUSINESS AREA" />
                                    <asp:TemplateField HeaderText="STATUS">
                                        <ItemTemplate>
                                            <span class='<%# GetStatusBadgeClass(Eval("Status")?.ToString()) %> badge-pill px-3 py-1'>
                                                <%# Eval("Status") %>
                                            </span>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Amount" HeaderText="AMOUNT" DataFormatString="{0:C}" ItemStyle-CssClass="pe-4 text-end fw-bold" HeaderStyle-CssClass="text-end pe-4" />
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </div>

                <div class="modal fade" id="detailModal" tabindex="-1">
                    <div class="modal-dialog modal-xl modal-dialog-centered">
                        <div class="modal-content border-0 shadow rounded-lg">
                            <div class="modal-header border-bottom">
                                <h5 class="modal-title font-weight-bold"><asp:Label ID="lblModalTitle" runat="server" /></h5>
                                <button type="button" class="btn-close close" data-dismiss="modal" data-bs-dismiss="modal" aria-label="Close">
                                    <span aria-hidden="true" class="d-none d-md-inline">&times;</span></button></div><div class="modal-body bg-light p-3 p-md-4">
                                <div class="d-flex flex-column flex-md-row justify-content-between mb-3 bg-white p-3 rounded shadow-sm align-items-md-center gap-3">
                                     <div class="input-group w-100" style="max-width: 400px;">
                                         <asp:TextBox ID="txtSearchDetail" runat="server" CssClass="form-control" placeholder="Search Reference No..." />
                                         <asp:LinkButton ID="btnSearchDetail" runat="server" CssClass="btn btn-primary" OnClick="btnSearchDetail_Click"><i class="fas fa-search"></i></asp:LinkButton></div><h5 class="mb-0 fw-bold text-secondary text-md-end">Total: <asp:Label ID="lblModalTotal" runat="server" CssClass="text-dark" /></h5>
                                </div>
                                <div class="table-responsive bg-white rounded shadow-sm">
                                    <asp:GridView ID="gvDetails" runat="server" CssClass="table table-hover mb-0 text-nowrap" AutoGenerateColumns="true" GridLines="None" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

            </ContentTemplate>
        </asp:UpdatePanel>
    </div>

    <style> 
        body { background-color: #f8fafc; font-family: 'Inter', sans-serif; overflow-x: hidden; } 
        .dashboard-container { width: 100%; max-width: 100%; margin: 0; }
        
        .kpi-card { border-radius: 16px; transition: 0.3s; }
        .card-blue { background: linear-gradient(135deg, #eef2fb 0%, #dce7f9 100%); color: #003f88; }
        .card-green { background: linear-gradient(135deg, #eafaf1 0%, #d5f5e3 100%); color: #198754; }
        .card-yellow { background: linear-gradient(135deg, #fff9e6 0%, #fef1c7 100%); color: #856404; }
        .icon-box { width: 52px; height: 52px; display: flex; align-items: center; justify-content: center; font-size: 1.4rem; box-shadow: 0 4px 10px rgba(0,0,0,0.05); flex-shrink: 0; }
        .text-label { font-size: 0.9rem; font-weight: 600; opacity: 0.8; }
        .sub-label { font-size: 0.8rem; margin-top: 4px; opacity: 0.7; }

        .status-item { 
            display: flex; justify-content: space-between; padding: 12px 15px; 
            text-decoration: none; color: #4b5563; font-size: 0.88rem; 
            border-bottom: 1px solid #f3f4f6; transition: 0.2s;
        }
        .status-item:hover { background-color: #f9fafb; color: #111827; }
        .active-row { background-color: #f0f7ff !important; border-left: 4px solid #3b82f6; }
        .active-row-yellow { background-color: #fffbeb !important; border-left: 4px solid #f59e0b; }

        .bg-light-header { background-color: #f9fafb; }
        .badge-pill { border-radius: 50px; font-weight: 600; font-size: 0.75rem; border: 1px solid transparent; }
        .badge-pending { background-color: #fff9e6; color: #ffc107; border-color: #ffeeba; }
        .badge-approved { background-color: #eafaf1; color: #198754; border-color: #c3e6cb; }
        .badge-resubmit { background-color: #fce8e6; color: #dc3545; border-color: #f5c6cb; }
        .badge-default { background-color: #eef2fb; color: #003f88; border-color: #b8daff; }

        .section-title { font-weight: 800; color: #1f2937; letter-spacing: -0.02em; }
        
        /* Prevent table text wrap on small screens */
        .text-nowrap td, .text-nowrap th { white-space: nowrap; }
    </style>

    <script type="text/javascript">
        var myGroupedChart = null;
        var myDoughnutChart = null;

        // ASP.NET AJAX standard function triggered after Partial Postbacks (UpdatePanel).
        function pageLoad() {
            if (typeof Chart === 'undefined') {
                console.error("Chart.js is not loaded.");
                return;
            }

            // Retrieve dynamically updated data
            var t1c = JSON.parse(document.getElementById('<%= hfChartDataT1C.ClientID %>').value || "[0,0,0,0,0,0]");
            var t1cO = JSON.parse(document.getElementById('<%= hfChartDataT1COthers.ClientID %>').value || "[0,0,0,0,0,0]");
            var add = JSON.parse(document.getElementById('<%= hfChartDataAdd.ClientID %>').value || "[0,0,0,0,0,0]");
            var trans = JSON.parse(document.getElementById('<%= hfChartDataTrans.ClientID %>').value || "[0,0,0,0,0,0]");
            var doughnutObj = JSON.parse(document.getElementById('<%= hfChartDataDoughnut.ClientID %>').value || "{}");

            renderDashboardCharts(t1c, t1cO, add, trans, doughnutObj);
        }

        function renderDashboardCharts(t1c, t1cO, add, trans, doughnutObj) {
            const blueLight = '#a0c4ff';
            const blueDark = '#003f88';

            // 1. Grouped Bar Chart
            var ctxBar = document.getElementById('chartGroupedBar');
            if (ctxBar) {
                if (myGroupedChart) myGroupedChart.destroy();
                myGroupedChart = new Chart(ctxBar.getContext('2d'), {
                    type: 'bar',
                    data: {
                        labels: ['T1C Budget', 'T1C Others', 'Additional', 'Transfer'],
                        datasets: [
                            { label: 'Submitted', data: [t1c[0], t1cO[0], add[0], trans[0]], backgroundColor: blueLight, borderRadius: 6 },
                            { label: 'Completed', data: [t1c[5], t1cO[5], add[5], trans[5]], backgroundColor: blueDark, borderRadius: 6 }
                        ]
                    },
                    options: { responsive: true, maintainAspectRatio: false }
                });
            }

            // 2. Doughnut Chart
            var ctxDoughnut = document.getElementById('chartDoughnut');
            if (ctxDoughnut) {
                if (myDoughnutChart) myDoughnutChart.destroy();
                myDoughnutChart = new Chart(ctxDoughnut.getContext('2d'), {
                    type: 'doughnut',
                    data: {
                        labels: doughnutObj.labels || [],
                        datasets: [{
                            data: doughnutObj.data || [],
                            backgroundColor: ['#003f88', '#005b9f', '#0077b6', '#0096c7', '#48cae4', '#ade8f4', '#f4a261', '#e76f51', '#2a9d8f']
                        }]
                    },
                    options: {
                        cutout: '70%',
                        responsive: true,
                        maintainAspectRatio: false,
                        plugins: { legend: { position: 'right' } }
                    }
                });
            }
        }

        function openDetailModal() {
            // BUG FIX: Clean up any rogue Bootstrap modal backdrops left over from the UpdatePanel replacing the DOM
            $('.modal-backdrop').remove();
            $('body').removeClass('modal-open').css('padding-right', '');

            // Re-bind and Show the modal safely
            var $modal = $('#detailModal');
            if ($modal.length) {
                $modal.modal('show');
            } else {
                console.error("Modal element not found.");
            }
        }
    </script>
</asp:Content>