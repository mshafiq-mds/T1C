<%@ Page Title="Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DashboardV2.aspx.cs" Inherits="Prodata.WebForm.DashboardV2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script src="https://cdn.jsdelivr.net/npm/chart.js@4.4.1/dist/chart.umd.min.js"></script>

    <div class="d-flex justify-content-between align-items-center mb-4">
        <h3 class="mb-0 text-dark font-weight-bold">CCMS Management Dashboard</h3>
        
        <div class="d-flex align-items-center gap-3">
            <div class="input-group bg-white rounded shadow-sm border" style="width: auto;">
                <div class="input-group-prepend">
                    <span class="input-group-text bg-transparent border-0 text-secondary font-weight-bold">Business Area:</span>
                </div>
                <asp:DropDownList runat="server" ID="ddBA" CssClass="form-control border-0 custom-select"
                    AutoPostBack="true" OnSelectedIndexChanged="ddBA_SelectedIndexChanged"
                    DataValueField="Code" DataTextField="DisplayName" />
            </div>

            <div class="input-group bg-white rounded shadow-sm border ms-3" style="width: 150px;">
                <div class="input-group-prepend">
                    <span class="input-group-text bg-transparent border-0 text-secondary font-weight-bold">Year:</span>
                </div>
                <asp:DropDownList runat="server" ID="ddYear" CssClass="form-control border-0 custom-select"
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
            <div class="row mb-4">
                <div class="col-md-4">
                    <div class="card kpi-card h-100">
                        <div class="card-body d-flex align-items-center">
                            <div class="icon-circle bg-light-blue text-primary me-3"><i class="fas fa-dollar-sign"></i></div>
                            <div>
                                <h6 class="text-secondary font-weight-bold mb-1">Total T1C Request</h6>
                                <div class="d-flex align-items-baseline">
                                    <h3 class="font-weight-bold text-dark mb-0 me-2"><asp:Label ID="lblTotalBudget" runat="server" Text="$125.5M"></asp:Label></h3>
                                    <span class="badge trend-badge bg-light-green text-success"><%--<i class="fas fa-arrow-up"></i> 5.2% YoY</span>--%>
                                </div>
                                <small class="text-muted">Allocated across 4 Category </small>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="col-md-4">
                    <div class="card kpi-card h-100">
                        <div class="card-body d-flex align-items-center">
                            <div class="icon-circle bg-light-blue text-primary me-3"><i class="fas fa-percentage"></i></div>
                            <div>
                                <h6 class="text-secondary font-weight-bold mb-1">Overall Approvals Completion Rate</h6>
                                <div class="d-flex align-items-baseline">
                                    <h3 class="font-weight-bold text-dark mb-0 me-2"><asp:Label ID="lblCompletionRate" runat="server" Text="88.7%"></asp:Label></h3>
                                    <span class="badge trend-badge bg-light-green text-success"><%--<i class="fas fa-arrow-up"></i> 1.5%</span>--%>
                                </div>
                                <small class="text-muted">Compared to last quarter</small>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="col-md-4">
                    <div class="card kpi-card h-100">
                        <div class="card-body d-flex align-items-center">
                            <div class="icon-circle bg-light-blue text-primary me-3"><i class="far fa-clock"></i></div>
                            <div>
                                <h6 class="text-secondary font-weight-bold mb-1">Pending Approvals</h6>
                                <div class="d-flex align-items-baseline">
                                    <h3 class="font-weight-bold text-dark mb-0 me-2"><asp:Label ID="lblPendingApprovals" runat="server" Text="142"></asp:Label></h3>
                                    <span class="badge trend-badge bg-light-yellow text-warning"><%--<i class="fas fa-arrow-down"></i> 3.8%</span>--%>
                                </div>
                                <small class="text-muted">Requires immediate attention</small>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <h5 class="font-weight-bold text-dark mb-3">Category Status Overview</h5>

            <div class="row mb-4">
                <div class="col-md-3">
                    <div class="card status-card">
                        <div class="card-header bg-white border-0 pt-3 pb-0"><h6 class="font-weight-bold text-dark mb-0">T1C Budget</h6></div>
                        <div class="card-body pt-2">
                            <asp:LinkButton ID="BtnT1CSubmitted" runat="server" OnClick="Card_Click" CommandArgument="T1C|Submitted" CssClass="status-row text-decoration-none"><div class="status-label"><i class="far fa-paper-plane text-primary"></i> Submitted</div><div class="status-val"><asp:Label ID="LblT1CSubmitted" runat="server" Text="0"></asp:Label> <span class="trend text-primary"><i class="fas fa-arrow-up"></i></span></div></asp:LinkButton>
                            <asp:LinkButton ID="BtnT1CReview" runat="server" OnClick="Card_Click" CommandArgument="T1C|Under Review" CssClass="status-row text-decoration-none"><div class="status-label"><i class="far fa-eye text-warning"></i> Under Review</div><div class="status-val"><asp:Label ID="LblT1CReview" runat="server" Text="0"></asp:Label> <span class="trend text-warning"><i class="fas fa-arrow-right"></i></span></div></asp:LinkButton>
                            <asp:LinkButton ID="BtnT1CResubmit" runat="server" OnClick="Card_Click" CommandArgument="T1C|Resubmit" CssClass="status-row text-decoration-none"><div class="status-label"><i class="fas fa-sync-alt text-secondary"></i> Resubmit</div><div class="status-val"><asp:Label ID="LblT1CResubmit" runat="server" Text="0"></asp:Label> <span class="trend text-secondary"><i class="fas fa-arrow-up"></i></span></div></asp:LinkButton>
                            <asp:LinkButton ID="BtnT1CComplete" runat="server" OnClick="Card_Click" CommandArgument="T1C|Approved" CssClass="status-row text-decoration-none"><div class="status-label"><i class="far fa-check-circle text-info"></i> Approved</div><div class="status-val"><asp:Label ID="LblT1CComplete" runat="server" Text="0"></asp:Label> <span class="trend text-info"><i class="fas fa-arrow-up"></i></span></div></asp:LinkButton>
                            <asp:LinkButton ID="BtnT1CFinalized" runat="server" OnClick="Card_Click" CommandArgument="T1C|Completed" CssClass="status-row text-decoration-none"><div class="status-label"><i class="fas fa-flag-checkered text-success"></i> Completed</div><div class="status-val"><asp:Label ID="LblT1CFinalized" runat="server" Text="0"></asp:Label> <span class="trend text-success"><i class="fas fa-check"></i></span></div></asp:LinkButton>
                            
                            <asp:LinkButton ID="BtnT1CDeleted" runat="server" OnClick="Card_Click" CommandArgument="T1C|Deleted" CssClass="status-row text-decoration-none border-0"><div class="status-label"><i class="far fa-trash-alt text-danger"></i> Deleted</div><div class="status-val"><asp:Label ID="LblT1CDeleted" runat="server" Text="0"></asp:Label> <span class="trend text-danger"><i class="fas fa-arrow-down"></i></span></div></asp:LinkButton>
                        </div>
                    </div>
                </div>

                <div class="col-md-3">
                    <div class="card status-card">
                        <div class="card-header bg-white border-0 pt-3 pb-0"><h6 class="font-weight-bold text-dark mb-0">T1C Others</h6></div>
                        <div class="card-body pt-2">
                            <asp:LinkButton ID="BtnT1COthersSubmitted" runat="server" OnClick="Card_Click" CommandArgument="T1COthers|Submitted" CssClass="status-row text-decoration-none"><div class="status-label"><i class="far fa-paper-plane text-primary"></i> Submitted</div><div class="status-val"><asp:Label ID="LblT1COthersSubmitted" runat="server" Text="0"></asp:Label> <span class="trend text-primary"><i class="fas fa-arrow-up"></i></span></div></asp:LinkButton>
                            <asp:LinkButton ID="BtnT1COthersReview" runat="server" OnClick="Card_Click" CommandArgument="T1COthers|Under Review" CssClass="status-row text-decoration-none"><div class="status-label"><i class="far fa-eye text-warning"></i> Under Review</div><div class="status-val"><asp:Label ID="LblT1COthersReview" runat="server" Text="0"></asp:Label> <span class="trend text-warning"><i class="fas fa-arrow-right"></i></span></div></asp:LinkButton>
                            <asp:LinkButton ID="BtnT1COthersResubmit" runat="server" OnClick="Card_Click" CommandArgument="T1COthers|Resubmit" CssClass="status-row text-decoration-none"><div class="status-label"><i class="fas fa-sync-alt text-secondary"></i> Resubmit</div><div class="status-val"><asp:Label ID="LblT1COthersResubmit" runat="server" Text="0"></asp:Label> <span class="trend text-secondary"><i class="fas fa-arrow-up"></i></span></div></asp:LinkButton>
                            <asp:LinkButton ID="BtnT1COthersComplete" runat="server" OnClick="Card_Click" CommandArgument="T1COthers|Approved" CssClass="status-row text-decoration-none"><div class="status-label"><i class="far fa-check-circle text-info"></i> Approved</div><div class="status-val"><asp:Label ID="LblT1COthersComplete" runat="server" Text="0"></asp:Label> <span class="trend text-info"><i class="fas fa-arrow-down"></i></span></div></asp:LinkButton>
                            <asp:LinkButton ID="BtnT1COthersDeleted" runat="server" OnClick="Card_Click" CommandArgument="T1COthers|Deleted" CssClass="status-row text-decoration-none border-0"><div class="status-label"><i class="far fa-trash-alt text-danger"></i> Deleted</div><div class="status-val"><asp:Label ID="LblT1COthersDeleted" runat="server" Text="0"></asp:Label> <span class="trend text-danger"><i class="fas fa-arrow-down"></i></span></div></asp:LinkButton>
                        </div>
                    </div>
                </div>

                <div class="col-md-3">
                    <div class="card status-card">
                        <div class="card-header bg-white border-0 pt-3 pb-0"><h6 class="font-weight-bold text-dark mb-0">Additional Budget</h6></div>
                        <div class="card-body pt-2">
                            <asp:LinkButton ID="BtnAddSubmitted" runat="server" OnClick="Card_Click" CommandArgument="Additional|Submitted" CssClass="status-row text-decoration-none"><div class="status-label"><i class="far fa-paper-plane text-primary"></i> Submitted</div><div class="status-val"><asp:Label ID="LblAdditionalSubmitted" runat="server" Text="0"></asp:Label> <span class="trend text-primary"><i class="fas fa-arrow-up"></i></span></div></asp:LinkButton>
                            <asp:LinkButton ID="BtnAddReview" runat="server" OnClick="Card_Click" CommandArgument="Additional|Under Review" CssClass="status-row text-decoration-none"><div class="status-label"><i class="far fa-eye text-warning"></i> Under Review</div><div class="status-val"><asp:Label ID="LblAdditionalReview" runat="server" Text="0"></asp:Label> <span class="trend text-warning"><i class="fas fa-arrow-up"></i></span></div></asp:LinkButton>
                            <asp:LinkButton ID="BtnAddResubmit" runat="server" OnClick="Card_Click" CommandArgument="Additional|Resubmit" CssClass="status-row text-decoration-none"><div class="status-label"><i class="fas fa-sync-alt text-secondary"></i> Resubmit</div><div class="status-val"><asp:Label ID="LblAdditionalResubmit" runat="server" Text="0"></asp:Label> <span class="trend text-secondary"><i class="fas fa-arrow-down"></i></span></div></asp:LinkButton>
                            <asp:LinkButton ID="BtnAddComplete" runat="server" OnClick="Card_Click" CommandArgument="Additional|Approved" CssClass="status-row text-decoration-none"><div class="status-label"><i class="far fa-check-circle text-info"></i> Approved</div><div class="status-val"><asp:Label ID="LblAdditionalComplete" runat="server" Text="0"></asp:Label> <span class="trend text-info"><i class="fas fa-arrow-down"></i></span></div></asp:LinkButton>
                            <asp:LinkButton ID="BtnAddDeleted" runat="server" OnClick="Card_Click" CommandArgument="Additional|Deleted" CssClass="status-row text-decoration-none border-0"><div class="status-label"><i class="far fa-trash-alt text-danger"></i> Deleted</div><div class="status-val"><asp:Label ID="LblAdditionalDeleted" runat="server" Text="0"></asp:Label> <span class="trend text-danger"><i class="fas fa-arrow-down"></i></span></div></asp:LinkButton>
                        </div>
                    </div>
                </div>

                <div class="col-md-3">
                    <div class="card status-card">
                        <div class="card-header bg-white border-0 pt-3 pb-0"><h6 class="font-weight-bold text-dark mb-0">Transfer Transactions</h6></div>
                        <div class="card-body pt-2" style="position: relative;">
                            
                            <asp:LinkButton ID="BtnTransSubmitted" runat="server" OnClick="Card_Click" CommandArgument="Transfer|SUBMITTED" CssClass="status-row text-decoration-none"><div class="status-label"><i class="far fa-paper-plane text-primary"></i> Submitted</div><div class="status-val"><asp:Label ID="LblTransferSubmitted" runat="server" Text="0"></asp:Label> <span class="trend text-primary"><i class="fas fa-arrow-up"></i></span></div></asp:LinkButton>

                            <asp:LinkButton ID="BtnTransReview" runat="server" OnClick="Card_Click" CommandArgument="Transfer|BUDGET ALLOCATE" CssClass="status-row text-decoration-none"><div class="status-label"><i class="fas fa-wallet text-warning"></i> Budget Allocate</div><div class="status-val"><asp:Label ID="LblTransferReview" runat="server" Text="0"></asp:Label> <span class="trend text-warning"><i class="fas fa-arrow-down"></i></span></div></asp:LinkButton>
                            <asp:LinkButton ID="BtnTransResubmit" runat="server" OnClick="Card_Click" CommandArgument="Transfer|UNDER REVIEW" CssClass="status-row text-decoration-none"><div class="status-label"><i class="far fa-eye text-warning"></i> Under Review</div><div class="status-val"><asp:Label ID="LblTransferResubmit" runat="server" Text="0"></asp:Label> <span class="trend text-warning"><i class="fas fa-arrow-up"></i></span></div></asp:LinkButton>
                            <asp:LinkButton ID="BtnTransComplete" runat="server" OnClick="Card_Click" CommandArgument="Transfer|RESUBMIT" CssClass="status-row text-decoration-none"><div class="status-label"><i class="fas fa-sync-alt text-secondary"></i> Resubmit</div><div class="status-val"><asp:Label ID="LblTransferComplete" runat="server" Text="0"></asp:Label> <span class="trend text-secondary"><i class="fas fa-arrow-right"></i></span></div></asp:LinkButton>
                            <asp:LinkButton ID="BtnTransFinalized" runat="server" OnClick="Card_Click" CommandArgument="Transfer|COMPLETED" CssClass="status-row text-decoration-none"><div class="status-label"><i class="far fa-check-circle text-success"></i> Completed</div><div class="status-val"><asp:Label ID="LblTransferFinalized" runat="server" Text="0"></asp:Label> <span class="trend text-success"><i class="fas fa-arrow-down"></i></span></div></asp:LinkButton>
                            <asp:LinkButton ID="BtnTransDeleted" runat="server" OnClick="Card_Click" CommandArgument="Transfer|DELETED" CssClass="status-row text-decoration-none border-0"><div class="status-label"><i class="far fa-trash-alt text-danger"></i> Deleted</div><div class="status-val"><asp:Label ID="LblTransferDeleted" runat="server" Text="0"></asp:Label> <span class="trend text-danger"><i class="fas fa-arrow-down"></i></span></div></asp:LinkButton>
                        </div>
                    </div>
                </div>
            </div>

            <h5 class="font-weight-bold text-dark mb-3">Analytics Overview</h5>

            <div class="row mb-4">
                <div class="col-md-7">
                    <div class="card h-100 chart-card shadow-sm border-0">
                        <div class="card-header bg-white border-0 pt-3 pb-0">
                            <h6 class="font-weight-bold text-dark mb-0">Budget Submission vs. Completion by Category</h6>
                        </div>
                        <div class="card-body">
                            <div style="height: 280px;"><canvas id="chartGroupedBar"></canvas></div>
                        </div>
                    </div>
                </div>
                <div class="col-md-5">
                    <div class="card h-100 chart-card shadow-sm border-0">
                        <div class="card-header bg-white border-0 pt-3 pb-0">
                            <h6 class="font-weight-bold text-dark mb-0">Top 10 T1C Amount by Business Area</h6>
                        </div>
                        <div class="card-body d-flex justify-content-center align-items-center">
                            <div style="height: 280px; width: 100%; position: relative;">
                                <canvas id="chartDoughnut"></canvas>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="card shadow-sm border-0 mb-5">
                <div class="card-header bg-white border-bottom pt-3 pb-3 d-flex justify-content-between align-items-center">
                    <h6 class="font-weight-bold text-dark mb-0">Recent Activity / Details</h6>
            <%--        <div class="input-group" style="width: 250px;">
                        <div class="input-group-prepend"><span class="input-group-text bg-white border-right-0"><i class="fas fa-search text-muted"></i></span></div>
                        <asp:TextBox ID="txtSearchGlobal" runat="server" CssClass="form-control border-left-0" placeholder="Search..."></asp:TextBox>
                    </div>--%>
                </div>
                <div class="card-body p-0 table-responsive">
                    <asp:GridView ID="gvRecentActivity" runat="server" CssClass="table table-hover mb-0 custom-grid" 
                        AutoGenerateColumns="false" EmptyDataText="No recent activity found." GridLines="None">
                        <Columns>
                            <asp:BoundField DataField="RefNo" HeaderText="Ref No" />
                            <asp:BoundField DataField="Date" HeaderText="Date" DataFormatString="{0:yyyy-MM-dd}" />
                            <asp:BoundField DataField="BA" HeaderText="Business Area" />
                            <asp:TemplateField HeaderText="Status">
                                <ItemTemplate>
                                    <span class='<%# GetStatusBadgeClass(Eval("Status").ToString()) %>'>
                                        <%# Eval("Status") %>
                                    </span>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="Amount" HeaderText="Amount" DataFormatString="{0:C}" />
                        </Columns>
                    </asp:GridView>
                </div>
            </div>

            <div class="modal fade" id="detailModal" tabindex="-1" role="dialog" aria-hidden="true">
                <div class="modal-dialog modal-xl modal-dialog-centered modal-dialog-scrollable" role="document">
                    <div class="modal-content shadow-lg border-0 rounded-lg">
                        <div class="modal-header bg-white border-bottom">
                            <h5 class="modal-title font-weight-bold text-dark"><asp:Label ID="lblModalTitle" runat="server" Text="Details"></asp:Label></h5>
                            <button type="button" class="close text-dark opacity-75" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                        </div>
                        <div class="modal-body p-4 bg-light">
                            <div class="row mb-4 align-items-center bg-white p-3 rounded shadow-sm">
                                <div class="col-md-6 col-sm-6">
                                    <div class="input-group">
                                        <asp:TextBox ID="txtSearchDetail" runat="server" CssClass="form-control border-end-0" placeholder="Search by Reference No..."></asp:TextBox>
                                        <div class="input-group-append">
                                            <asp:LinkButton ID="btnSearchDetail" runat="server" CssClass="btn btn-outline-secondary border-start-0" OnClick="btnSearchDetail_Click">
                                                <i class="fas fa-search"></i>
                                            </asp:LinkButton>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-6 col-sm-6 text-end text-right">
                                    <h5 class="m-0 text-secondary font-weight-bold">
                                        Total Amount: 
                                        <asp:Label ID="lblModalTotal" runat="server" CssClass="text-dark" Text="RM 0.00"></asp:Label>
                                    </h5>
                                </div>
                            </div>
                            
                            <div class="table-responsive shadow-sm rounded bg-white p-2">
                                <asp:GridView ID="gvDetails" runat="server" CssClass="table table-hover mb-0 custom-grid" 
                                    AutoGenerateColumns="true" EmptyDataText="No records found." GridLines="None">
                                </asp:GridView>
                            </div>
                        </div>
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

        var myGroupedChart = null;
        var myDoughnutChart = null;

        function pageLoad() {
            if (typeof Chart === 'undefined') { console.error("Chart.js not loaded."); return; }

            var t1c = JSON.parse(document.getElementById('<%= hfChartDataT1C.ClientID %>').value || "[]");
            var t1cO = JSON.parse(document.getElementById('<%= hfChartDataT1COthers.ClientID %>').value || "[]");
            var add = JSON.parse(document.getElementById('<%= hfChartDataAdd.ClientID %>').value || "[]");
            var trans = JSON.parse(document.getElementById('<%= hfChartDataTrans.ClientID %>').value || "[]");
            
            // New Doughnut Data
            var doughnutObjStr = document.getElementById('<%= hfChartDataDoughnut.ClientID %>').value;
            var doughnutObj = doughnutObjStr ? JSON.parse(doughnutObjStr) : {};

            if (t1c.length > 0) renderDashboardCharts(t1c, t1cO, add, trans, doughnutObj);
        }

        function renderDashboardCharts(t1c, t1cO, add, trans, doughnutObj) {

            // 1. Grouped Bar Chart (Submitted vs Completed)
            var submittedData = [t1c[0] || 0, t1cO[0] || 0, add[0] || 0, trans[0] || 0];
            var completedData = [t1c[3] || 0, t1cO[3] || 0, add[3] || 0, trans[5] || 0];

            if (myGroupedChart) myGroupedChart.destroy();
            var ctxBar = document.getElementById('chartGroupedBar').getContext('2d');
            myGroupedChart = new Chart(ctxBar, {
                type: 'bar',
                data: {
                    labels: ['T1C Budget', 'T1C Others', 'Additional Budget', 'Transfer Transactions'],
                    datasets: [
                        { label: 'Submitted', data: submittedData, backgroundColor: '#a0c4ff', borderRadius: 4, barPercentage: 0.6 },
                        { label: 'Completed', data: completedData, backgroundColor: '#003f88', borderRadius: 4, barPercentage: 0.6 }
                    ]
                },
                options: {
                    responsive: true, maintainAspectRatio: false,
                    plugins: { legend: { position: 'top', align: 'center', labels: { usePointStyle: true, boxWidth: 8 } } },
                    scales: {
                        y: { border: { display: false }, grid: { color: '#f1f3f5', borderDash: [5, 5] }, beginAtZero: true },
                        x: { border: { display: false }, grid: { display: false } }
                    }
                }
            });

            // 2. Dynamic Doughnut Chart
            var dLabels = doughnutObj.labels && doughnutObj.labels.length > 0 ? doughnutObj.labels : ['No Data'];
            var dData = doughnutObj.data && doughnutObj.data.length > 0 ? doughnutObj.data : [1];
            var dCenterText = doughnutObj.centerText || 'RM 0';

            if (myDoughnutChart) myDoughnutChart.destroy();
            var ctxDoughnut = document.getElementById('chartDoughnut').getContext('2d');

            const centerTextPlugin = {
                id: 'centerText',
                beforeDraw: function (chart) {
                    var width = chart.width, height = chart.height, ctx = chart.ctx;
                    ctx.restore();
                    // Adjust font size based on height
                    var fontSize = (height / 150).toFixed(2);
                    ctx.font = "bold " + fontSize + "em sans-serif";
                    ctx.textBaseline = "middle";
                    ctx.fillStyle = "#333";
                    var text = dCenterText,
                        textX = Math.round((width - ctx.measureText(text).width) / 2),
                        // Center is shifted left to account for legend area on the right
                        chartArea = chart.chartArea;

                    var actualCenterX = chartArea.left + ((chartArea.right - chartArea.left) / 2);
                    textX = actualCenterX - (ctx.measureText(text).width / 2);

                    var textY = height / 2;
                    ctx.fillText(text, textX, textY);
                    ctx.save();
                }
            };

            // Modern robust palette for up to 10 entries
            var topColors = ['#003f88', '#005b9f', '#0077b6', '#0096c7', '#48cae4', '#ade8f4', '#f4a261', '#e76f51', '#2a9d8f', '#e9c46a'];

            myDoughnutChart = new Chart(ctxDoughnut, {
                type: 'doughnut',
                data: {
                    labels: dLabels,
                    datasets: [{
                        data: dData,
                        backgroundColor: topColors.slice(0, dData.length),
                        borderWidth: 2, borderColor: '#ffffff'
                    }]
                },
                options: {
                    responsive: true, maintainAspectRatio: false, cutout: '75%',
                    plugins: {
                        legend: {
                            display: true,
                            position: 'right', // Place legend to the right
                            labels: {
                                usePointStyle: true,
                                boxWidth: 8,
                                font: { size: 11 }
                            }
                        },
                        tooltip: {
                            callbacks: {
                                label: function (context) {
                                    let label = context.label || '';
                                    if (label) label += ': ';
                                    // Format number to currency style
                                    label += 'RM ' + context.parsed.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
                                    return label;
                                }
                            }
                        }
                    }
                },
                plugins: [centerTextPlugin]
            });
        }
    </script>

    <style>
        body { background-color: #f4f7f6; font-family: 'Segoe UI', system-ui, -apple-system, sans-serif; }
        
        /* Typography & Utilities */
        .text-dark { color: #2c3e50 !important; }
        .text-secondary { color: #6c757d !important; }
        .bg-light-blue { background-color: #eef2fb; }
        .bg-light-green { background-color: #eafaf1; }
        .bg-light-yellow { background-color: #fff9e6; }
        .shadow-sm { box-shadow: 0 2px 12px rgba(0,0,0,0.04) !important; }
        .rounded-lg { border-radius: 12px !important; }

        /* KPI Cards */
        .kpi-card { border: none; border-radius: 12px; box-shadow: 0 4px 15px rgba(0,0,0,0.03); transition: transform 0.2s; }
        .kpi-card:hover { transform: translateY(-3px); }
        .icon-circle { width: 50px; height: 50px; border-radius: 50%; display: flex; align-items: center; justify-content: center; font-size: 1.3rem; }
        .trend-badge { font-size: 0.75rem; padding: 4px 8px; border-radius: 20px; font-weight: 600; }

        /* Status Cards */
        .status-card { border: none; border-radius: 12px; box-shadow: 0 4px 15px rgba(0,0,0,0.03); margin-bottom: 20px; height: 100%; }
        .status-row { 
            display: flex; justify-content: space-between; align-items: center; 
            padding: 10px 0; border-bottom: 1px dashed #e9ecef; color: #495057; transition: background 0.2s; 
        }
        .status-row:hover { background-color: #f8f9fa; border-radius: 4px; padding-left: 5px; padding-right: 5px; margin-left: -5px; margin-right: -5px; }
        .status-row:last-child { border-bottom: none; }
        .status-label { font-size: 0.85rem; font-weight: 500; display: flex; align-items: center; gap: 8px; }
        .status-val { font-weight: 700; font-size: 1rem; color: #212529; display: flex; align-items: center; gap: 10px; }
        .trend { font-size: 0.7rem; padding: 2px 5px; border-radius: 4px; background: #f8f9fa; }
        
        /* Grid Customization */
        .custom-grid { border-collapse: separate; border-spacing: 0; width: 100%; }
        .custom-grid th { background-color: #f8f9fa; color: #6c757d; font-size: 0.8rem; text-transform: uppercase; border-top: none; padding: 12px 15px; }
        .custom-grid td { padding: 12px 15px; vertical-align: middle; font-size: 0.9rem; color: #495057; border-bottom: 1px solid #f1f3f5; }
        
        /* Badge Styles for Table */
        .badge-approved { background-color: #eafaf1; color: #198754; padding: 5px 10px; border-radius: 6px; font-weight: 600; font-size: 0.75rem; border: 1px solid #c3e6cb;}
        .badge-pending { background-color: #fff9e6; color: #ffc107; padding: 5px 10px; border-radius: 6px; font-weight: 600; font-size: 0.75rem; border: 1px solid #ffeeba;}
        .badge-resubmit { background-color: #fce8e6; color: #dc3545; padding: 5px 10px; border-radius: 6px; font-weight: 600; font-size: 0.75rem; border: 1px solid #f5c6cb;}
        .badge-default { background-color: #eef2fb; color: #003f88; padding: 5px 10px; border-radius: 6px; font-weight: 600; font-size: 0.75rem; border: 1px solid #b8daff;}
    </style>
</asp:Content>