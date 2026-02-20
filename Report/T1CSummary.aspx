<%@ Page Title="T1C Report Summary" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="T1CSummary.aspx.cs" Inherits="Prodata.WebForm.Report.T1CSummary" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div id="pagePreloader" class="page-preloader" style="display: flex; position: fixed; top: 0; left: 0; width: 100%; height: 100%; background: rgba(0,0,0,0.7); z-index: 99999; flex-direction: column; align-items: center; justify-content: center; opacity: 1; transition: opacity 0.3s ease-in-out;">
        <img src="<%= ResolveUrl("~/Images/Felda_Global_Ventures_Logo.png") %>" alt="Loading..." height="200" width="200" />
        <p class="mt-3 text-white" style="font-weight: bold;">Processing...</p>
    </div>

    <style>
        /* Only used for the 'hiding' state */
        .page-preloader.done {
            opacity: 0 !important;
            pointer-events: none;
        }

        @media print {
            .page-preloader { display: none !important; }
        }
    </style>

    <script type="text/javascript">
        function applyInputNumber() {
            if (typeof $ !== 'undefined') {
                // Use .off() to prevent multiple event bindings
                $('.input-number').off('input').on('input', function () {
                    this.value = this.value.replace(/[^0-9.]/g, '').replace(/(\..*)\./g, '$1');
                });
            }
        }

        // Handle initial load
        window.addEventListener("load", function () {
            setTimeout(hidePreloader, 500);
            applyInputNumber();
        });

        // Handle UpdatePanel partial postbacks
        if (typeof Sys !== 'undefined') {
            var prm = Sys.WebForms.PageRequestManager.getInstance();
            prm.add_beginRequest(function () {
                showPreloader();
            });
            prm.add_endRequest(function () {
                hidePreloader();
                applyInputNumber();
            });
        }

        function showPreloader() {
            var loader = document.getElementById('pagePreloader');
            if (loader) {
                loader.classList.remove('done');
                loader.style.display = 'flex';
                loader.style.opacity = '1';
            }
        }

        function hidePreloader() {
            var loader = document.getElementById('pagePreloader');
            if (loader) {
                loader.classList.add('done');
                setTimeout(function () {
                    if (loader.classList.contains('done')) {
                        loader.style.display = 'none';
                    }
                }, 300);
            }
        }
    </script>
    <asp:HiddenField ID="hdnRecordId" runat="server" /> 
    
    <div id="printArea">

        <div class="print-header d-none">
            <img src="<%= ResolveUrl("~/Images/Felda_Global_Ventures_Logo.png") %>" alt="FGV Logo" style="height: 80px;" />
            <h3 class="mt-2">T1C Report Summary</h3>
            <p>Generated on: <%= DateTime.Now.ToString("dd/MM/yyyy HH:mm") %></p>
        </div>

        <div class="row no-print"> 
            <div class="col-md-12">
                <div id="divCardSearch" runat="server" class="card card-outline collapsed-card">
                    <div class="card-header card-header-sticky" data-card-widget="collapse">
                        <h3 class="card-title">Search</h3>
                    </div>
                    <div class="card-body">
                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group row">
                                    <asp:Label ID="lblYear" runat="server" CssClass="col-md-3 col-form-label" AssociatedControlID="ddlYear" Text="Year"></asp:Label>
                                    <div class="col-md-5">
                                        <asp:DropDownList ID="ddlYear" runat="server" CssClass="form-control select2" data-placeholder="Year"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="form-group row">
                                    <asp:Label ID="lblBA" runat="server" CssClass="col-md-3 col-form-label" AssociatedControlID="ddlBA" Text="Business Area"></asp:Label>
                                    <div class="col-md-5">
                                        <asp:DropDownList ID="ddlBA" runat="server" CssClass="form-control select2" data-placeholder="Select BA"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="form-group row">
                                    <asp:Label ID="lblBudget" runat="server" CssClass="col-md-3 col-form-label" AssociatedControlID="ddlBT"  Text="Budget Type"></asp:Label>
                                    <div class="col-md-5">
                                        <asp:DropDownList 
                                            runat="server" 
                                            ID="ddlBT" 
                                            CssClass="form-control select2"
                                            data-placeholder="Select Type">
                                        </asp:DropDownList> 
                                    </div>
                                </div>
                                <div class="form-group row">
                                    <asp:Label ID="lblRef" runat="server" CssClass="col-md-3 col-form-label" AssociatedControlID="txtRef" Text="Reference No"></asp:Label>
                                    <div class="col-md-5">
                                        <asp:TextBox ID="txtRef" runat="server" CssClass="form-control" TextMode="Search" placeholder="Reference No"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="form-group row">
                                    <asp:Label ID="lblKeyword" runat="server" CssClass="col-md-3 col-form-label" AssociatedControlID="txtKeyword" Text="Keyword"></asp:Label>
                                    <div class="col-md-5">
                                        <asp:TextBox ID="txtKeyword" runat="server" CssClass="form-control" placeholder="Search details..."></asp:TextBox>
                                    </div>
                                </div>
                                <div class="form-group row">
                                    <asp:Label ID="lblStatus" runat="server" CssClass="col-md-3 col-form-label" AssociatedControlID="ddlStatus" Text="Status"></asp:Label>
                                    <div class="col-md-5">
                                        <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-control select2" data-placeholder="Status">
                                            <asp:ListItem Text="" Value="" />
                                            <asp:ListItem Text="Draft" Value="Draft" />
                                            <asp:ListItem Text="Pending" Value="Pending" />
                                            <asp:ListItem Text="Approved" Value="Approved" />
                                            <asp:ListItem Text="Rejected" Value="Rejected" />
                                            <asp:ListItem Text="Sent Back" Value="SentBack" />
                                            <asp:ListItem Text="Completed" Value="Completed" />
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="form-group row">
                                    <asp:Label ID="lblStartDate" runat="server" CssClass="col-md-3 col-form-label" AssociatedControlID="txtStartDate" Text="Start Date"></asp:Label>
                                    <div class="col-md-5">
                                        <asp:TextBox ID="txtStartDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="form-group row">
                                    <asp:Label ID="lblEndDate" runat="server" CssClass="col-md-3 col-form-label" AssociatedControlID="txtEndDate" Text="End Date"></asp:Label>
                                    <div class="col-md-5">
                                        <asp:TextBox ID="txtEndDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="form-group row">
                                    <asp:Label ID="lblMinAmount" runat="server" CssClass="col-md-3 col-form-label" AssociatedControlID="txtMinAmount" Text="Min Amount"></asp:Label>
                                    <div class="col-md-5">
                                        <%-- Removed TextMode="Number" to prevent value loss on postback due to browser/server format conflicts --%>
                                        <asp:TextBox ID="txtMinAmount" runat="server" CssClass="form-control input-number" placeholder="Min Amount"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="form-group row">
                                    <asp:Label ID="lblMaxAmount" runat="server" CssClass="col-md-3 col-form-label" AssociatedControlID="txtMaxAmount" Text="Max Amount"></asp:Label>
                                    <div class="col-md-5">
                                        <%-- Removed TextMode="Number" to prevent value loss on postback due to browser/server format conflicts --%>
                                        <asp:TextBox ID="txtMaxAmount" runat="server" CssClass="form-control input-number" placeholder="Max Amount"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="form-group row">
                                    <div class="col-md-9 offset-md-3">
                                        <asp:LinkButton ID="btnSearch" runat="server" CssClass="btn btn-outline-secondary" OnClick="btnSearch_Click" >
                                            <i class="fas fa-search"></i> Search
                                        </asp:LinkButton>
                                        <asp:LinkButton ID="btnReset" runat="server" CssClass="btn btn-link" OnClick="btnReset_Click">
                                            <i class="fas fa-sync"></i> Reset
                                        </asp:LinkButton>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="row">
            <div class="col-md-12">
                <div class="card card-outline">
                    <div class="card-header card-header-sticky">
                        <h3 class="card-title d-none d-sm-inline"><%= Page.Title %></h3>
                        <div class="card-tools">
                            
                            <%-- EXCEL EXPORT BUTTON --%>
                            <asp:LinkButton ID="btnExportExcel" runat="server" CssClass="btn btn-success mr-1" OnClick="btnExportExcel_Click">
                                <i class="fas fa-file-excel"></i> Excel
                            </asp:LinkButton>

                            <%-- PRINT PDF BUTTON (Navigates to Print View) --%>
                            <asp:LinkButton ID="btnExportPDF" runat="server" CssClass="btn btn-default mr-1" OnClick="btnPrintAll_Click">
                                <i class="fas fa-print"></i> Print
                            </asp:LinkButton>
                        </div>
                    </div>
                    <div class="card-body">
                        <div class="row">
                            <div class="col-12">
                                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                    <ContentTemplate>
                                        <div class="table-responsive">
                                            <asp:GridView ID="gvData" runat="server" AutoGenerateColumns="false" CssClass="table table-bordered" PageSize='<%# FGV.Prodata.App.Setting.RecordsPerPage() %>' AllowPaging="true" OnPageIndexChanging="gvData_PageIndexChanging" EmptyDataText="No record." OnClientClick="showPreloader()">
                                                <Columns>
                                                    <asp:TemplateField HeaderText="#">
                                                        <HeaderStyle CssClass="width-30 text-center" />
                                                        <ItemStyle CssClass="width-30 text-center" />
                                                        <ItemTemplate>
                                                            <%# Container.DataItemIndex + 1 %>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    
                                                    <asp:BoundField DataField="BizAreaDisplayName" HeaderText="BA" HeaderStyle-CssClass="align-middle" />

                                                    <asp:BoundField DataField="Ref" HeaderText="Reference No" HeaderStyle-CssClass="align-middle text-nowrap" ItemStyle-CssClass="text-nowrap" />
                                                    <asp:BoundField DataField="Date" HeaderText="Date" HeaderStyle-CssClass="align-middle text-nowrap" />
                                                    <asp:BoundField DataField="Details" HeaderText="Details" HeaderStyle-CssClass="align-middle text-nowrap" />
                                                    <asp:BoundField DataField="Amount" HeaderText="Amount (RM)" HeaderStyle-CssClass="align-middle text-nowrap text-center" ItemStyle-CssClass="text-right" />
                                                    <asp:TemplateField HeaderText="Next Approver">
                                                        <HeaderStyle CssClass="align-middle text-nowrap text-center" />
                                                        <ItemStyle CssClass="text-center" />
                                                        <ItemTemplate>
                                                            <%# Eval("Status") != null && Eval("Status").ToString().Equals("Completed", StringComparison.OrdinalIgnoreCase) 
                                                                ? "<span class='text-success font-weight-bold'>Complete</span>" 
                                                                : Eval("NextApprover") %>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Status">
                                                        <HeaderStyle CssClass="width-80 text-center align-middle" />
                                                        <ItemStyle CssClass="width-80 text-center" />
                                                        <ItemTemplate>
                                                            <asp:Label runat="server" 
                                                                CssClass='<%# 
                                                                    Eval("Status") != null ? (
                                                                        Eval("Status").ToString().ToLower() == "completed" ? "badge badge-dark badge-pill" :
                                                                        Eval("Status").ToString().ToLower() == "approved" ? "badge badge-success badge-pill" :
                                                                        Eval("Status").ToString().ToLower() == "pending" ? "badge badge-warning badge-pill" :
                                                                        Eval("Status").ToString().ToLower() == "rejected" ? "badge badge-danger badge-pill" :
                                                                        Eval("Status").ToString().ToLower() == "draft" ? "badge badge-info badge-pill" :
                                                                        Eval("Status").ToString().ToLower() == "sentback" ? "badge badge-secondary badge-pill" :
                                                                        "badge badge-secondary badge-pill"
                                                                    ) : "badge badge-secondary badge-pill"
                                                                %>' 
                                                                Text='<%# Eval("Status").ToString().Equals("SentBack", StringComparison.OrdinalIgnoreCase) ? "Sent Back" : Eval("Status") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                                <PagerSettings Mode="NumericFirstLast" PageButtonCount="5" Position="TopAndBottom" />
                                                <PagerStyle CssClass="pagination-ys no-print" />
                                            </asp:GridView>
                                        </div>
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:PostBackTrigger ControlID="btnExportExcel" />
                                        <asp:PostBackTrigger ControlID="btnExportPDF" />
                                    </Triggers>
                                </asp:UpdatePanel>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div> 
    
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <script type="text/javascript">
        function applyInputNumber() {
            $('.input-number').on('input', function () {
                this.value = this.value.replace(/[^0-9.]/g, '').replace(/(\..*)\./g, '$1');
            });
        }
    </script>
</asp:Content>