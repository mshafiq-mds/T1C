<%@ Page Title="Upload PO" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="UploadPO.aspx.cs" Inherits="Prodata.WebForm.T1C.PO.Upload.UploadPO" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
    .page-preloader {
        position: fixed;
        z-index: 99999;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        background: rgba(0,0,0,0.6);
        display: flex;
        justify-content: center;
        align-items: center;
        flex-direction: column;
    }

    .page-preloader img {
        animation: shake 1.5s infinite;
    }

    @keyframes shake {
        0% { transform: rotate(0deg); }
        25% { transform: rotate(3deg); }
        50% { transform: rotate(0deg); }
        75% { transform: rotate(-3deg); }
        100% { transform: rotate(0deg); }
    }
</style>
    <!-- Page-specific Preloader -->
<div id="pagePreloader" class="page-preloader" style="display:none;">
    <img src="<%= ResolveUrl("~/Images/Felda_Global_Ventures_Logo.png") %>" 
         alt="Loading..." height="200" width="200" />
    <p class="mt-3 text-white">Processing...</p>
</div>
    <asp:HiddenField ID="hdnFormId" runat="server" />
    <div class="row">
        <div class="col-lg-12">
            <div class="card card-outline">
                <div class="card-header card-header-sticky">
                    <h3 class="card-title">
                        <asp:Label ID="lblTitle" runat="server"></asp:Label>
                    </h3>
                    <div class="card-tools">
                        <asp:LinkButton ID="btnBack" runat="server" CssClass="btn btn-default" PostBackUrl="~/T1C/PO/Upload/Default" CausesValidation="false">
                            <i class="fas fa-angle-double-left"></i> Back
                        </asp:LinkButton>
                        <asp:LinkButton ID="btnSave" runat="server" CssClass="btn btn-primary" OnClick="btnSave_Click" OnClientClick="return beforeSubmitPageload();">
                            <i class="fas fa-save"></i> Save
                        </asp:LinkButton>
                    </div>
                </div>
                <div class="card-body">
                    <div class="row">
                        <div class="col-md-9 border-right">
                            <div class="form-group">
                                <asp:Label runat="server" CssClass="text-bold" Text="BA"></asp:Label>
                                <div class="text-muted">
                                    <asp:Label ID="lblBA" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label runat="server" CssClass="text-bold" Text="Requester Name"></asp:Label>
                                <div class="text-muted">
                                    <asp:Label ID="lblReqName" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label runat="server" CssClass="text-bold" Text="Details"></asp:Label>
                                <div class="text-muted">
                                    <asp:Label ID="lblDetails" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label runat="server" CssClass="text-bold" Text="Justification of Need"></asp:Label>
                                <div class="text-muted">
                                    <asp:Label ID="lblJustificationOfNeed" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label runat="server" CssClass="text-bold" Text="Allocations"></asp:Label>
                                <div class="text-muted">
                                    <asp:Label ID="lblAllocation" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label runat="server" CssClass="text-bold" Text="Contractors"></asp:Label>
                                <div class="text-muted">
                                    <asp:Label ID="lblVendor" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="form-group" id="divJustificationDirectNegotiation" runat="server">
                                <asp:Label runat="server" CssClass="text-bold" Text="Justification (Direct Negotiation)"></asp:Label>
                                <div class="text-muted">
                                    <asp:Label ID="lblJustificationDirectAward" runat="server"></asp:Label>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-3">
                            <div class="form-group">
                                <asp:Label runat="server" CssClass="text-bold" Text="Reference No."></asp:Label>
                                <div class="text-muted">
                                    <asp:Label ID="lblRefNo" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label runat="server" CssClass="text-bold" Text="Date"></asp:Label>
                                <div class="text-muted">
                                    <asp:Label ID="lblDate" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label runat="server" CssClass="text-bold" Text="Estimate Amount"></asp:Label>
                                <div class="text-warning text-bold">
                                    <asp:Label ID="lblAmount" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label runat="server" CssClass="text-bold" Text="Quotation Method"></asp:Label>
                                <div class="text-muted">
                                    <asp:Label ID="lblProcurementType" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label runat="server" CssClass="text-bold" Text="Status"></asp:Label>
                                <div>
                                    <asp:Label ID="lblStatus" runat="server" CssClass="badge badge-pill"></asp:Label>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-9">
                            <div class="form-group">
                                <asp:Label runat="server" CssClass="text-bold" Text="Data Cost"></asp:Label>
                                <div class="table-responsive">
                                    <table id="tblDataKos" runat="server" class="table table-bordered table-sm">
                                        <thead>
                                            <tr>
                                                <th></th>
                                                <th colspan="2" class="text-center"><%= DateTime.Now.Year %> (RM/MT)</th>
                                                <th colspan="3" class="text-center">YTD <%= DateTime.Now.Year - 1 %> (RM/MT)</th>
                                            </tr>
                                            <tr>
                                                <th></th>
                                                <th class="text-center">Actual YTD</th>
                                                <th class="text-center">Annual Budget</th>
                                                <th class="text-center">Actual (RM)</th>
                                                <th class="text-center">Actual</th>
                                                <th class="text-center">Budget</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <tr>
                                                <th class="text-nowrap align-middle">S & M</th>
                                                <td class="text-center">
                                                    <asp:Label ID="lblCurrentYearActualYTD" runat="server" CssClass="text-muted"></asp:Label></td>
                                                <td class="text-center">
                                                    <asp:Label ID="lblCurrentYearBudget" runat="server" CssClass="text-muted"></asp:Label></td>
                                                <td class="text-center">
                                                    <asp:Label ID="lblPreviousYearActualYTD" runat="server" CssClass="text-muted"></asp:Label></td>
                                                <td class="text-center">
                                                    <asp:Label ID="lblPreviousYearActual" runat="server" CssClass="text-muted"></asp:Label></td>
                                                <td class="text-center">
                                                    <asp:Label ID="lblPreviousYearBudget" runat="server" CssClass="text-muted"></asp:Label></td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </div>
                                <table style="width: 100%;">
                                    <tbody>
                                        <tr>
                                            <th class="text-center align-middle pl-1 pr-1">A</th>
                                            <td style="width: 45%;">
                                                <asp:Label ID="lblA" runat="server" CssClass="form-control text-muted"></asp:Label></td>
                                            <th class="text-center align-middle pl-1 pr-1">C</th>
                                            <td style="width: 45%;">
                                                <asp:Label ID="lblC" runat="server" CssClass="form-control text-muted"></asp:Label></td>
                                        </tr>
                                        <tr>
                                            <th class="text-center align-middle pl-1 pr-1">B</th>
                                            <td style="width: 45%;">
                                                <asp:Label ID="lblB" runat="server" CssClass="form-control text-muted"></asp:Label></td>
                                            <th class="text-center align-middle pl-1 pr-1">D</th>
                                            <td style="width: 45%;">
                                                <asp:Label ID="lblD" runat="server" CssClass="form-control text-muted"></asp:Label></td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-12">
                            <div class="card card-success">
                                <div class="card-header" data-card-widget="collapse">
                                    <h3 class="card-title">Upload PO</h3>
                                </div>
                                <div class="card-body">
                                    <div class="row">
                                        <div class="col-md-12">
                                            <div class="form-group">
                                                <asp:Label runat="server" AssociatedControlID="fuPO" Text="Upload PO"></asp:Label>
                                                <div class="custom-file">
                                                    <asp:FileUpload ID="fuPO" runat="server" CssClass="custom-file-input" />
                                                    <asp:Label runat="server" AssociatedControlID="fuPO" CssClass="custom-file-label" Text="Choose PO"></asp:Label>
                                                </div>
                                                <!-- Error label shown when file is missing -->
                                                <asp:Label ID="lblPOError" runat="server" CssClass="text-danger d-none"></asp:Label>
                                                <asp:Panel ID="pnlPOView" runat="server" Visible="false">
                                                    <asp:HyperLink ID="lnkPO" runat="server" Target="_blank" Text="View PO" CssClass="btn btn-link"></asp:HyperLink>
                                                </asp:Panel>
                                            </div>
                                        </div>
                                        <div class="col-md-6">
                                            <div class="form-group">
                                                <asp:Label runat="server" AssociatedControlID="txtActualAmount" Text="Actual Amount"></asp:Label>
                                                <div class="input-group">
                                                    <div class="input-group-prepend">
                                                        <span class="input-group-text">RM</span>
                                                    </div>
                                                    <asp:TextBox ID="txtActualAmount" runat="server" CssClass="form-control input-number2" placeholder="Actual Amount"></asp:TextBox>
                                                </div>
                                                <asp:Label ID="lblActualAmountError" runat="server" CssClass="text-danger d-none" />
                                                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtActualAmount" CssClass="text-danger" Display="Dynamic" ErrorMessage="Please enter actual amount (RM)"></asp:RequiredFieldValidator>
                                            </div>
                                        </div>
                                        <div class="col-md-6">
                                            <div class="form-group">
                                                <asp:Label runat="server" CssClass="text-bold" Text="Estimate Amount"></asp:Label>
                                                <div class="text-warning text-bold mt-2">
                                                    <asp:Label ID="lblAmount2" runat="server" CssClass="text-lg"></asp:Label>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-md-12">
                                            <div id="allocationSection" runat="server" class="form-group d-none">
                                                <asp:Label runat="server" CssClass="text-bold" Text="Allocate Balance"></asp:Label>
                                                <asp:Repeater ID="rptBudgetAllocations" runat="server">
                                                    <ItemTemplate>
                                                        <div class="mb-2">
                                                            <asp:HiddenField ID="hdnBudgetId" runat="server" Value='<%# Eval("BudgetId") %>' />
                                                            <div class="text-muted mb-1">
                                                                <%# Eval("Ref") %> – <%# Eval("Details") %>
                                                            </div>
                                                            <div class="input-group">
                                                                <div class="input-group-prepend">
                                                                    <span class="input-group-text">RM</span>
                                                                </div>
                                                                <asp:TextBox ID="txtAllocateAmount" runat="server"
                                                                    Text='<%# (Convert.ToDecimal(Eval("AllocatedAmount")) == 0 ? "" : string.Format("{0:N2}", Eval("AllocatedAmount"))) %>'
                                                                    CssClass="form-control input-number2 allocation-input"
                                                                    placeholder="Balance" />
                                                            </div>
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                                <asp:Label ID="lblAllocationError" runat="server" CssClass="text-danger d-none"></asp:Label>
                                            </div>

                                            <div id="allocationDropdownSection" runat="server" class="form-group d-none">
                                                <asp:Label runat="server" CssClass="text-bold" Text="Allocation"></asp:Label>
                                                <div id="allocationContainer">
                                                    <div class="input-group mb-2 allocation-group">
                                                        <asp:DropDownList ID="ddlAllocation" runat="server" CssClass="form-control select2 allocation-input col-md-6" data-placeholder="Allocation"></asp:DropDownList>
                                                        <asp:TextBox ID="txtAllocationAmount" runat="server" CssClass="form-control input-number2" placeholder="Amount" Enabled="false"></asp:TextBox>
                                                        <div class="input-group-append">
                                                            <button type="button" class="btn btn-danger btnRemoveAllocation" disabled>
                                                                <i class="fa fa-minus"></i>
                                                            </button>
                                                        </div>
                                                        <span class="balance col-md-3 mt-1 ml-2">Balance: RM 0.00</span>
                                                    </div>
                                                </div>
                                                <button type="button" class="btn btn-info mt-1" id="btnAddAllocation">
                                                    <i class="fa fa-plus"></i>Add Allocation
                                                </button>
                                            </div>

                                            <span id="allocationBalance" runat="server" class="text-info font-weight-bold"></span>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-12">
                            <div class="card card-secondary collapsed-card">
                                <div class="card-header" data-card-widget="collapse">
                                    <h3 class="card-title">Documents</h3>
                                </div>
                                <div class="card-body">
                                    <div class="row">
                                        <div class="col-12">
                                            <!-- Picture -->
                                            <div class="form-group row">
                                                <asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Picture"></asp:Label>
                                                <div class="col-lg-9">
                                                    <asp:Panel ID="pnlPictureView" runat="server" Visible="false">
                                                        <asp:HyperLink ID="lnkPicture" runat="server" Target="_blank" Text="View Picture" CssClass="btn btn-link" />
                                                    </asp:Panel>
                                                    <asp:Label ID="lblPictureDash" runat="server" Text="-" Visible="true" />
                                                </div>
                                            </div>
                                            <hr class="mt-0" />

                                            <!-- Machine Repair History -->
                                            <div class="form-group row">
                                                <asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Machine Repair History"></asp:Label>
                                                <div class="col-lg-9">
                                                    <asp:Panel ID="pnlMachineRepairHistoryView" runat="server" Visible="false">
                                                        <asp:HyperLink ID="lnkMachineRepairHistory" runat="server" Target="_blank" Text="View File" CssClass="btn btn-link" />
                                                    </asp:Panel>
                                                    <asp:Label ID="lblMachineHistoryDash" runat="server" Text="-" Visible="true" />
                                                </div>
                                            </div>
                                            <hr class="mt-0" />

                                            <!-- Job Specification -->
                                            <div class="form-group row">
                                                <asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Job Specification"></asp:Label>
                                                <div class="col-lg-9">
                                                    <asp:Panel ID="pnlJobSpecificationView" runat="server" Visible="false">
                                                        <asp:HyperLink ID="lnkJobSpecification" runat="server" Target="_blank" Text="View File" CssClass="btn btn-link" />
                                                    </asp:Panel>
                                                    <asp:Label ID="lblJobSpecificationDash" runat="server" Text="-" Visible="true" />
                                                </div>
                                            </div>
                                            <hr class="mt-0" />

                                            <!-- Engineer Estimate Price -->
                                            <div class="form-group row">
                                                <asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Engineer's Estimate Price"></asp:Label>
                                                <div class="col-lg-9">
                                                    <asp:Panel ID="pnlEngineerEstimatePriceView" runat="server" Visible="false">
                                                        <asp:HyperLink ID="lnkEngineerEstimatePrice" runat="server" Target="_blank" Text="View File" CssClass="btn btn-link" />
                                                    </asp:Panel>
                                                    <asp:Label ID="lblEngineerEstimatePriceDash" runat="server" Text="-" Visible="true" />
                                                </div>
                                            </div>
                                            <hr class="mt-0" />

                                            <!-- Dec Cost Report Current Year -->
                                            <div class="form-group row">
                                                <asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Dec Cost Report (Current Year)"></asp:Label>
                                                <div class="col-lg-9">
                                                    <asp:Panel ID="pnlDecCostReportCurrentYearView" runat="server" Visible="false">
                                                        <asp:HyperLink ID="lnkDecCostReportCurrentYear" runat="server" Target="_blank" Text="View File" CssClass="btn btn-link" />
                                                    </asp:Panel>
                                                    <asp:Label ID="lblDecCostReportCurrentYearDash" runat="server" Text="-" Visible="true" />
                                                </div>
                                            </div>
                                            <hr class="mt-0" />

                                            <!-- Dec Cost Report Last Year -->
                                            <div class="form-group row">
                                                <asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Dec Cost Report (Last Year)"></asp:Label>
                                                <div class="col-lg-9">
                                                    <asp:Panel ID="pnlDecCostReportLastYearView" runat="server" Visible="false">
                                                        <asp:HyperLink ID="lnkDecCostReportLastYear" runat="server" Target="_blank" Text="View File" CssClass="btn btn-link" />
                                                    </asp:Panel>
                                                    <asp:Label ID="lblDecCostReportLastYearDash" runat="server" Text="-" Visible="true" />
                                                </div>
                                            </div>
                                            <hr class="mt-0" />

                                            <!-- Cost Report Last Month -->
                                            <div class="form-group row">
                                                <asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Cost Report (Last Month)"></asp:Label>
                                                <div class="col-lg-9">
                                                    <asp:Panel ID="pnlCostReportLastMonthView" runat="server" Visible="false">
                                                        <asp:HyperLink ID="lnkCostReportLastMonth" runat="server" Target="_blank" Text="View File" CssClass="btn btn-link" />
                                                    </asp:Panel>
                                                    <asp:Label ID="lblCostReportLastMonthDash" runat="server" Text="-" Visible="true" />
                                                </div>
                                            </div>
                                            <hr class="mt-0" />

                                            <!-- Drawing/Sketching -->
                                            <div class="form-group row">
                                                <asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Drawing/Sketching"></asp:Label>
                                                <div class="col-lg-9">
                                                    <asp:Panel ID="pnlDrawingSketchingView" runat="server" Visible="false">
                                                        <asp:HyperLink ID="lnkDrawingSketching" runat="server" Target="_blank" Text="View File" CssClass="btn btn-link" />
                                                    </asp:Panel>
                                                    <asp:Label ID="lblDrawingSketchingDash" runat="server" Text="-" Visible="true" />
                                                </div>
                                            </div>
                                            <hr class="mt-0" />

                                            <!-- Quotation -->
                                            <div class="form-group row">
                                                <asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Quotation"></asp:Label>
                                                <div class="col-lg-9">
                                                    <asp:Panel ID="pnlQuotationView" runat="server" Visible="false">
                                                        <asp:HyperLink ID="lnkQuotation" runat="server" Target="_blank" Text="View File" CssClass="btn btn-link" />
                                                    </asp:Panel>
                                                    <asp:Label ID="lblQuotationDash" runat="server" Text="-" Visible="true" />
                                                </div>
                                            </div>
                                            <hr class="mt-0" />

                                            <!-- Damage Investigation Report -->
                                            <div class="form-group row">
                                                <asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Damage Investigation Report"></asp:Label>
                                                <div class="col-lg-9">
                                                    <asp:Panel ID="pnlDamageInvestigationReportView" runat="server" Visible="false">
                                                        <asp:HyperLink ID="lnkDamageInvestigationReport" runat="server" Target="_blank" Text="View File" CssClass="btn btn-link" />
                                                    </asp:Panel>
                                                    <asp:Label ID="lblDamageInvestigationReportDash" runat="server" Text="-" Visible="true" />
                                                </div>
                                            </div>
                                            <hr class="mt-0" />

                                            <!-- Vendor Registration Record -->
                                            <div class="form-group row">
                                                <asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Vendor Registration Record"></asp:Label>
                                                <div class="col-lg-9">
                                                    <asp:Panel ID="pnlVendorRegistrationRecordView" runat="server" Visible="false">
                                                        <asp:HyperLink ID="lnkVendorRegistrationRecord" runat="server" Target="_blank" Text="View File" CssClass="btn btn-link" />
                                                    </asp:Panel>
                                                    <asp:Label ID="lblVendorRegistrationRecordDash" runat="server" Text="-" Visible="true" />
                                                </div>
                                            </div>
                                            <hr class="mt-0" />

                                            <!-- Budget Transfer/Add Approval -->
                                            <div class="form-group row">
                                                <asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Budget Transfer/Add Approval"></asp:Label>
                                                <div class="col-lg-9">
                                                    <asp:Panel ID="pnlBudgetTransferAddApprovalView" runat="server" Visible="false">
                                                        <asp:HyperLink ID="lnkBudgetTransferAddApproval" runat="server" Target="_blank" Text="View File" CssClass="btn btn-link" />
                                                    </asp:Panel>
                                                    <asp:Label ID="lblBudgetTransferAddApprovalDash" runat="server" Text="-" Visible="true" />
                                                </div>
                                            </div>
                                            <hr class="mt-0" />

                                            <!-- Other Supporting Document -->
                                            <div class="form-group row">
                                                <asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Other Supporting Document"></asp:Label>
                                                <div class="col-lg-9">
                                                    <asp:Panel ID="pnlOtherSupportingDocumentView" runat="server" Visible="false">
                                                        <asp:HyperLink ID="lnkOtherSupportingDocument" runat="server" Target="_blank" Text="View File" CssClass="btn btn-link" />
                                                    </asp:Panel>
                                                    <asp:Label ID="lblOtherSupportingDocumentDash" runat="server" Text="-" Visible="true" />
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
                            <div class="card card-secondary collapsed-card">
                                <div class="card-header" data-card-widget="collapse">
                                    <h3 class="card-title">Audit Trails</h3>
                                </div>
                                <div class="card-body">
                                    <div class="row">
                                        <div class="col-12">
                                            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                                <ContentTemplate>
                                                    <div class="table-responsive">
                                                        <asp:GridView ID="gvAuditTrails" runat="server" AutoGenerateColumns="false" CssClass="table table-bordered" PageSize='<%# FGV.Prodata.App.Setting.RecordsPerPage() %>' AllowPaging="true" OnPageIndexChanging="gvAuditTrails_PageIndexChanging" EmptyDataText="No record.">
                                                            <Columns>
                                                                <asp:TemplateField HeaderText="#">
                                                                    <HeaderStyle CssClass="width-30 text-center" />
                                                                    <ItemStyle CssClass="width-30 text-center" />
                                                                    <ItemTemplate>
                                                                        <%# Container.DataItemIndex + 1 %>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:BoundField DataField="ActionByName" HeaderText="Name" />
                                                                <asp:BoundField DataField="ActionByRole" HeaderText="Role" />
                                                                <asp:BoundField DataField="Action" HeaderText="Action" />
                                                                <asp:BoundField DataField="Remark" HeaderText="Remark" />
                                                                <asp:BoundField DataField="Datetime" HeaderText="Date & Time" HeaderStyle-CssClass="text-nowrap" ItemStyle-CssClass="text-nowrap" />
                                                            </Columns>
                                                            <PagerSettings Mode="NumericFirstLast" PageButtonCount="5" />
                                                            <PagerStyle CssClass="pagination-ys" />
                                                        </asp:GridView>
                                                    </div>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <script>
        $(document).ready(function () {
            bsCustomFileInput.init();

            // Bind input event to actual amount
            $('#<%= txtActualAmount.ClientID %>').on('input', toggleAllocationSections);

            // Trigger validation when allocation inputs change
            $(document).on('input', '.allocation-input', function () {
                var actualVal = parseDecimal($('#<%= txtActualAmount.ClientID %>').val());
                var estimateVal = parseDecimal($('#<%= lblAmount2.ClientID %>').text());

                if (actualVal < estimateVal) {
                    var expectedTotal = estimateVal - actualVal;
                    validateTotalAllocation(expectedTotal);
                }
            });

            // Run once on page load
            toggleAllocationSections();
        });

        function parseDecimal(value) {
            if (!value) return 0;
            value = value.replace(/,/g, '').replace(/RM/i, '').trim();
            let num = parseFloat(value);
            return isNaN(num) ? 0 : num;
        }

        function toggleAllocationSections() {
            var $actualInput = $('#<%= txtActualAmount.ClientID %>');
            var actualInputVal = $actualInput.val();
            if (!actualInputVal || !actualInputVal.trim()) return;

            var actualVal = parseDecimal(actualInputVal);
            var estimateVal = parseDecimal($('#<%= lblAmount2.ClientID %>').text());

            var $allocationSection = $('#<%= allocationSection.ClientID %>');
            var $allocationDropdownSection = $('#<%= allocationDropdownSection.ClientID %>');
            var $allocationBalance = $('#<%= allocationBalance.ClientID %>');
            var $allocationError = $('#<%= lblAllocationError.ClientID %>');

            $allocationSection.addClass('d-none');
            $allocationDropdownSection.addClass('d-none');
            $allocationBalance.text('');
            $allocationError.addClass('d-none').text('');

            var balance = actualVal - estimateVal;
            var absBalance = Math.abs(balance).toFixed(2);
            var balanceFormatted = 'RM ' + Number(absBalance).toLocaleString('en-MY', { minimumFractionDigits: 2 });

            if (balance < 0) {
                $allocationSection.removeClass('d-none');
                $allocationBalance.text('Unallocated Balance: ' + balanceFormatted);
                validateTotalAllocation(Math.abs(balance));
            } else if (balance > 0) {
                $allocationDropdownSection.removeClass('d-none');
                $allocationBalance.text('Excess Allocation: ' + balanceFormatted);
            }
        }

        function validateTotalAllocation(expectedTotal) {
            let total = 0;

            $('.allocation-input').each(function () {
                total += parseDecimal($(this).val());
            });

            const isValid = Math.abs(total - expectedTotal) < 0.01;

            const $allocationError = $('#<%= lblAllocationError.ClientID %>');
            if (!isValid) {
                $allocationError.removeClass('d-none').text(
                    'Total allocated amount (RM ' + total.toLocaleString('en-MY', { minimumFractionDigits: 2, maximumFractionDigits: 2 }) + ') must equal RM ' + expectedTotal.toLocaleString('en-MY', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
                );
            } else {
                $allocationError.addClass('d-none').text('');
            }
        }

        function beforeSave() {
            var hasError = false;

            // Allocation error check
            var $allocationError = $('#<%= lblAllocationError.ClientID %>');
            if (!$allocationError.hasClass('d-none') && $allocationError.text().trim() !== '') {
                hasError = true;
            }

            // File upload validation
            var $fileInput = $('#<%= fuPO.ClientID %>');
            var fileSelected = $fileInput.val();
            var isPoPreviouslyUploaded = $('#<%= pnlPOView.ClientID %>').is(':visible');
            var $poError = $('#<%= lblPOError.ClientID %>');

            if (!isPoPreviouslyUploaded && (!fileSelected || fileSelected.trim() === '')) {
                $poError.removeClass('d-none').text('Please upload a PO file.');
                hasError = true;
            } else {
                $poError.addClass('d-none').text('');
            }

            // Actual amount validation
            var $actualAmount = $('#<%= txtActualAmount.ClientID %>');
            var actualAmountValue = $actualAmount.val().replace(/,/g, '').trim();
            var $actualError = $('#<%= lblActualAmountError.ClientID %>');

            if (!actualAmountValue) {
                $actualError.removeClass('d-none').text('Actual amount is required.');
                hasError = true;
            } else if (isNaN(actualAmountValue)) {
                $actualError.removeClass('d-none').text('Invalid actual amount.');
                hasError = true;
            } else {
                $actualError.addClass('d-none').text('');
            }

            return !hasError; // prevent save if any error
        }
        function beforeSubmitPageload() {
            // Run validation first
            if (!beforeSave()) {
                return false; // stop if validation fails
            }

            // Show preloader only if validation passes
            $("#pagePreloader").fadeIn(200);

            return true; // continue with ASP.NET postback
        }
    </script>

</asp:Content>

<%--<%@ Page Title="Upload PO" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="UploadPO.aspx.cs" Inherits="Prodata.WebForm.T1C.PO.Upload.UploadPO" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
    .page-preloader {
        position: fixed;
        z-index: 99999;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        background: rgba(0,0,0,0.6);
        display: flex;
        justify-content: center;
        align-items: center;
        flex-direction: column;
    }

    .page-preloader img {
        animation: shake 1.5s infinite;
    }

    @keyframes shake {
        0% { transform: rotate(0deg); }
        25% { transform: rotate(3deg); }
        50% { transform: rotate(0deg); }
        75% { transform: rotate(-3deg); }
        100% { transform: rotate(0deg); }
    }
</style>
    <div id="pagePreloader" class="page-preloader" style="display:none;">
    <img src="<%= ResolveUrl("~/Images/Felda_Global_Ventures_Logo.png") %>" 
         alt="Loading..." height="200" width="200" />
    <p class="mt-3 text-white">Processing...</p>
</div>
    <asp:HiddenField ID="hdnFormId" runat="server" />
    <div class="row">
        <div class="col-lg-12">
            <div class="card card-outline">
                <div class="card-header card-header-sticky">
                    <h3 class="card-title">
                        <asp:Label ID="lblTitle" runat="server"></asp:Label>
                    </h3>
                    <%--<div class="card-tools">
                        <asp:LinkButton ID="LinkButton1" runat="server" CssClass="btn btn-default" PostBackUrl="~/T1C/PO/Upload/Default" CausesValidation="false">
                            <i class="fas fa-angle-double-left"></i> Back
                        </asp:LinkButton>
                        <asp:LinkButton ID="LinkButton2" runat="server" CssClass="btn btn-primary" OnClick="btnSave_Click" OnClientClick="return beforeSubmitPageload();">
                            <i class="fas fa-save"></i> Save
                        </asp:LinkButton>
                    </div>--%><%--
                    <div class="card-tools">
                        <asp:LinkButton ID="btnBack" runat="server" CssClass="btn btn-default" PostBackUrl="~/T1C/PO/Upload/Default" CausesValidation="false">
                            <i class="fas fa-angle-double-left"></i> Back
                        </asp:LinkButton>
                        <asp:LinkButton ID="btnSave" runat="server" CssClass="btn btn-primary" OnClick="btnSave_Click" OnClientClick="return confirmSave(event);">
                            <i class="fas fa-save"></i> Save
                        </asp:LinkButton>
                    </div>
                </div>
                <div class="card-body">
                    <div class="row">
                        <div class="col-md-9 border-right">
                            <div class="form-group">
                                <asp:Label runat="server" CssClass="text-bold" Text="BA"></asp:Label>
                                <div class="text-muted">
                                    <asp:Label ID="lblBA" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label runat="server" CssClass="text-bold" Text="Requester Name"></asp:Label>
                                <div class="text-muted">
                                    <asp:Label ID="lblReqName" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label runat="server" CssClass="text-bold" Text="Details"></asp:Label>
                                <div class="text-muted">
                                    <asp:Label ID="lblDetails" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label runat="server" CssClass="text-bold" Text="Justification of Need"></asp:Label>
                                <div class="text-muted">
                                    <asp:Label ID="lblJustificationOfNeed" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label runat="server" CssClass="text-bold" Text="Allocations"></asp:Label>
                                <div class="text-muted">
                                    <asp:Label ID="lblAllocation" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label runat="server" CssClass="text-bold" Text="Contractors"></asp:Label>
                                <div class="text-muted">
                                    <asp:Label ID="lblVendor" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="form-group" id="divJustificationDirectNegotiation" runat="server">
                                <asp:Label runat="server" CssClass="text-bold" Text="Justification (Direct Negotiation)"></asp:Label>
                                <div class="text-muted">
                                    <asp:Label ID="lblJustificationDirectAward" runat="server"></asp:Label>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-3">
                            <div class="form-group">
                                <asp:Label runat="server" CssClass="text-bold" Text="Reference No."></asp:Label>
                                <div class="text-muted">
                                    <asp:Label ID="lblRefNo" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label runat="server" CssClass="text-bold" Text="Date"></asp:Label>
                                <div class="text-muted">
                                    <asp:Label ID="lblDate" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label runat="server" CssClass="text-bold" Text="Estimate Amount"></asp:Label>
                                <div class="text-warning text-bold">
                                    <asp:Label ID="lblAmount" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label runat="server" CssClass="text-bold" Text="Quotation Method"></asp:Label>
                                <div class="text-muted">
                                    <asp:Label ID="lblProcurementType" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label runat="server" CssClass="text-bold" Text="Status"></asp:Label>
                                <div>
                                    <asp:Label ID="lblStatus" runat="server" CssClass="badge badge-pill"></asp:Label>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-9">
                            <div class="form-group">
                                <asp:Label runat="server" CssClass="text-bold" Text="Data Cost"></asp:Label>
                                <div class="table-responsive">
                                    <table id="tblDataKos" runat="server" class="table table-bordered table-sm">
                                        <thead>
                                            <tr>
                                                <th></th>
                                                <th colspan="2" class="text-center"><%= DateTime.Now.Year %> (RM/MT)</th>
                                                <th colspan="3" class="text-center">YTD <%= DateTime.Now.Year - 1 %> (RM/MT)</th>
                                            </tr>
                                            <tr>
                                                <th></th>
                                                <th class="text-center">Actual YTD</th>
                                                <th class="text-center">Annual Budget</th>
                                                <th class="text-center">Actual (RM)</th>
                                                <th class="text-center">Actual</th>
                                                <th class="text-center">Budget</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <tr>
                                                <th class="text-nowrap align-middle">S & M</th>
                                                <td class="text-center">
                                                    <asp:Label ID="lblCurrentYearActualYTD" runat="server" CssClass="text-muted"></asp:Label></td>
                                                <td class="text-center">
                                                    <asp:Label ID="lblCurrentYearBudget" runat="server" CssClass="text-muted"></asp:Label></td>
                                                <td class="text-center">
                                                    <asp:Label ID="lblPreviousYearActualYTD" runat="server" CssClass="text-muted"></asp:Label></td>
                                                <td class="text-center">
                                                    <asp:Label ID="lblPreviousYearActual" runat="server" CssClass="text-muted"></asp:Label></td>
                                                <td class="text-center">
                                                    <asp:Label ID="lblPreviousYearBudget" runat="server" CssClass="text-muted"></asp:Label></td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </div>
                                <table style="width: 100%;">
                                    <tbody>
                                        <tr>
                                            <th class="text-center align-middle pl-1 pr-1">A</th>
                                            <td style="width: 45%;">
                                                <asp:Label ID="lblA" runat="server" CssClass="form-control text-muted"></asp:Label></td>
                                            <th class="text-center align-middle pl-1 pr-1">C</th>
                                            <td style="width: 45%;">
                                                <asp:Label ID="lblC" runat="server" CssClass="form-control text-muted"></asp:Label></td>
                                        </tr>
                                        <tr>
                                            <th class="text-center align-middle pl-1 pr-1">B</th>
                                            <td style="width: 45%;">
                                                <asp:Label ID="lblB" runat="server" CssClass="form-control text-muted"></asp:Label></td>
                                            <th class="text-center align-middle pl-1 pr-1">D</th>
                                            <td style="width: 45%;">
                                                <asp:Label ID="lblD" runat="server" CssClass="form-control text-muted"></asp:Label></td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-12">
                            <div class="card card-success">
                                <div class="card-header" data-card-widget="collapse">
                                    <h3 class="card-title">Upload PO</h3>
                                </div>
                                <div class="card-body">
                                    
                                    <div class="row">
                                        <div class="col-md-12">
                                            <div class="form-group">
                                                <asp:Label runat="server" AssociatedControlID="fuPO" Text="Upload PO"></asp:Label>
                                                
                                                <asp:Panel ID="pnlFileUpload" runat="server">
                                                    <div class="custom-file">
                                                        <asp:FileUpload ID="fuPO" runat="server" CssClass="custom-file-input" />
                                                        <asp:Label runat="server" AssociatedControlID="fuPO" CssClass="custom-file-label" Text="Choose PO"></asp:Label>
                                                    </div>
                                                </asp:Panel>

                                                <asp:Label ID="lblPOError" runat="server" CssClass="text-danger d-none"></asp:Label>
                                                
                                                <asp:Panel ID="pnlPOView" runat="server" Visible="false">
                                                    <asp:HyperLink ID="lnkPO" runat="server" Target="_blank" Text="View PO" CssClass="btn btn-link"></asp:HyperLink>
                                                </asp:Panel>
                                            </div>
                                        </div>
                                    </div>

                                    <asp:Panel ID="pnlFormFields" runat="server" CssClass="row">
                                        <div class="col-md-6">
                                            <div class="form-group">
                                                <asp:Label runat="server" AssociatedControlID="txtActualAmount" Text="Actual Amount"></asp:Label>
                                                <div class="input-group">
                                                    <div class="input-group-prepend">
                                                        <span class="input-group-text">RM</span>
                                                    </div>
                                                    <asp:TextBox ID="txtActualAmount" runat="server" CssClass="form-control input-number2" placeholder="Actual Amount"></asp:TextBox>
                                                </div>
                                                <asp:Label ID="lblActualAmountError" runat="server" CssClass="text-danger d-none" />
                                                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtActualAmount" CssClass="text-danger" Display="Dynamic" ErrorMessage="Please enter actual amount (RM)"></asp:RequiredFieldValidator>
                                            </div>
                                        </div>
                                        <div class="col-md-6">
                                            <div class="form-group">
                                                <asp:Label runat="server" CssClass="text-bold" Text="Estimate Amount"></asp:Label>
                                                <div class="text-warning text-bold mt-2">
                                                    <asp:Label ID="lblAmount2" runat="server" CssClass="text-lg"></asp:Label>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-md-12">
                                            <div id="allocationSection" runat="server" class="form-group d-none">
                                                <asp:Label runat="server" CssClass="text-bold" Text="Allocate Balance"></asp:Label>
                                                <asp:Repeater ID="rptBudgetAllocations" runat="server">
                                                    <ItemTemplate>
                                                        <div class="mb-2">
                                                            <asp:HiddenField ID="hdnBudgetId" runat="server" Value='<%# Eval("BudgetId") %>' />
                                                            <div class="text-muted mb-1">
                                                                <%# Eval("Ref") %> – <%# Eval("Details") %>
                                                            </div>
                                                            <div class="input-group">
                                                                <div class="input-group-prepend">
                                                                    <span class="input-group-text">RM</span>
                                                                </div>
                                                                <asp:TextBox ID="txtAllocateAmount" runat="server"
                                                                    Text='<%# (Convert.ToDecimal(Eval("AllocatedAmount")) == 0 ? "" : string.Format("{0:N2}", Eval("AllocatedAmount"))) %>'
                                                                    CssClass="form-control input-number2 allocation-input"
                                                                    placeholder="Balance" />
                                                            </div>
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                                <asp:Label ID="lblAllocationError" runat="server" CssClass="text-danger d-none"></asp:Label>
                                            </div>

                                            <div id="allocationDropdownSection" runat="server" class="form-group d-none">
                                                <asp:Label runat="server" CssClass="text-bold" Text="Allocation"></asp:Label>
                                                <div id="allocationContainer">
                                                    <div class="input-group mb-2 allocation-group">
                                                        <asp:DropDownList ID="ddlAllocation" runat="server" CssClass="form-control select2 allocation-input col-md-6" data-placeholder="Allocation"></asp:DropDownList>
                                                        <asp:TextBox ID="txtAllocationAmount" runat="server" CssClass="form-control input-number2" placeholder="Amount" Enabled="false"></asp:TextBox>
                                                        <div class="input-group-append">
                                                            <button type="button" class="btn btn-danger btnRemoveAllocation" disabled>
                                                                <i class="fa fa-minus"></i>
                                                            </button>
                                                        </div>
                                                        <span class="balance col-md-3 mt-1 ml-2">Balance: RM 0.00</span>
                                                    </div>
                                                </div>
                                                <button type="button" class="btn btn-info mt-1" id="btnAddAllocation">
                                                    <i class="fa fa-plus"></i>Add Allocation
                                                </button>
                                            </div>

                                            <span id="allocationBalance" runat="server" class="text-info font-weight-bold"></span>
                                        </div>
                                    </asp:Panel>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-12">
                            <div class="card card-secondary collapsed-card">
                                <div class="card-header" data-card-widget="collapse">
                                    <h3 class="card-title">Documents</h3>
                                </div>
                                <div class="card-body">
                                    <div class="row">
                                        <div class="col-12">
                                            <div class="form-group row">
                                                <asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Picture"></asp:Label>
                                                <div class="col-lg-9">
                                                    <asp:Panel ID="pnlPictureView" runat="server" Visible="false">
                                                        <asp:HyperLink ID="lnkPicture" runat="server" Target="_blank" Text="View Picture" CssClass="btn btn-link" />
                                                    </asp:Panel>
                                                    <asp:Label ID="lblPictureDash" runat="server" Text="-" Visible="true" />
                                                </div>
                                            </div>
                                            <hr class="mt-0" />

                                            <div class="form-group row">
                                                <asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Machine Repair History"></asp:Label>
                                                <div class="col-lg-9">
                                                    <asp:Panel ID="pnlMachineRepairHistoryView" runat="server" Visible="false">
                                                        <asp:HyperLink ID="lnkMachineRepairHistory" runat="server" Target="_blank" Text="View File" CssClass="btn btn-link" />
                                                    </asp:Panel>
                                                    <asp:Label ID="lblMachineHistoryDash" runat="server" Text="-" Visible="true" />
                                                </div>
                                            </div>
                                            <hr class="mt-0" />

                                            <div class="form-group row">
                                                <asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Job Specification"></asp:Label>
                                                <div class="col-lg-9">
                                                    <asp:Panel ID="pnlJobSpecificationView" runat="server" Visible="false">
                                                        <asp:HyperLink ID="lnkJobSpecification" runat="server" Target="_blank" Text="View File" CssClass="btn btn-link" />
                                                    </asp:Panel>
                                                    <asp:Label ID="lblJobSpecificationDash" runat="server" Text="-" Visible="true" />
                                                </div>
                                            </div>
                                            <hr class="mt-0" />

                                            <div class="form-group row">
                                                <asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Engineer's Estimate Price"></asp:Label>
                                                <div class="col-lg-9">
                                                    <asp:Panel ID="pnlEngineerEstimatePriceView" runat="server" Visible="false">
                                                        <asp:HyperLink ID="lnkEngineerEstimatePrice" runat="server" Target="_blank" Text="View File" CssClass="btn btn-link" />
                                                    </asp:Panel>
                                                    <asp:Label ID="lblEngineerEstimatePriceDash" runat="server" Text="-" Visible="true" />
                                                </div>
                                            </div>
                                            <hr class="mt-0" />

                                            <div class="form-group row">
                                                <asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Dec Cost Report (Current Year)"></asp:Label>
                                                <div class="col-lg-9">
                                                    <asp:Panel ID="pnlDecCostReportCurrentYearView" runat="server" Visible="false">
                                                        <asp:HyperLink ID="lnkDecCostReportCurrentYear" runat="server" Target="_blank" Text="View File" CssClass="btn btn-link" />
                                                    </asp:Panel>
                                                    <asp:Label ID="lblDecCostReportCurrentYearDash" runat="server" Text="-" Visible="true" />
                                                </div>
                                            </div>
                                            <hr class="mt-0" />

                                            <div class="form-group row">
                                                <asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Dec Cost Report (Last Year)"></asp:Label>
                                                <div class="col-lg-9">
                                                    <asp:Panel ID="pnlDecCostReportLastYearView" runat="server" Visible="false">
                                                        <asp:HyperLink ID="lnkDecCostReportLastYear" runat="server" Target="_blank" Text="View File" CssClass="btn btn-link" />
                                                    </asp:Panel>
                                                    <asp:Label ID="lblDecCostReportLastYearDash" runat="server" Text="-" Visible="true" />
                                                </div>
                                            </div>
                                            <hr class="mt-0" />

                                            <div class="form-group row">
                                                <asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Cost Report (Last Month)"></asp:Label>
                                                <div class="col-lg-9">
                                                    <asp:Panel ID="pnlCostReportLastMonthView" runat="server" Visible="false">
                                                        <asp:HyperLink ID="lnkCostReportLastMonth" runat="server" Target="_blank" Text="View File" CssClass="btn btn-link" />
                                                    </asp:Panel>
                                                    <asp:Label ID="lblCostReportLastMonthDash" runat="server" Text="-" Visible="true" />
                                                </div>
                                            </div>
                                            <hr class="mt-0" />

                                            <div class="form-group row">
                                                <asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Drawing/Sketching"></asp:Label>
                                                <div class="col-lg-9">
                                                    <asp:Panel ID="pnlDrawingSketchingView" runat="server" Visible="false">
                                                        <asp:HyperLink ID="lnkDrawingSketching" runat="server" Target="_blank" Text="View File" CssClass="btn btn-link" />
                                                    </asp:Panel>
                                                    <asp:Label ID="lblDrawingSketchingDash" runat="server" Text="-" Visible="true" />
                                                </div>
                                            </div>
                                            <hr class="mt-0" />

                                            <div class="form-group row">
                                                <asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Quotation"></asp:Label>
                                                <div class="col-lg-9">
                                                    <asp:Panel ID="pnlQuotationView" runat="server" Visible="false">
                                                        <asp:HyperLink ID="lnkQuotation" runat="server" Target="_blank" Text="View File" CssClass="btn btn-link" />
                                                    </asp:Panel>
                                                    <asp:Label ID="lblQuotationDash" runat="server" Text="-" Visible="true" />
                                                </div>
                                            </div>
                                            <hr class="mt-0" />

                                            <div class="form-group row">
                                                <asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Damage Investigation Report"></asp:Label>
                                                <div class="col-lg-9">
                                                    <asp:Panel ID="pnlDamageInvestigationReportView" runat="server" Visible="false">
                                                        <asp:HyperLink ID="lnkDamageInvestigationReport" runat="server" Target="_blank" Text="View File" CssClass="btn btn-link" />
                                                    </asp:Panel>
                                                    <asp:Label ID="lblDamageInvestigationReportDash" runat="server" Text="-" Visible="true" />
                                                </div>
                                            </div>
                                            <hr class="mt-0" />

                                            <div class="form-group row">
                                                <asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Vendor Registration Record"></asp:Label>
                                                <div class="col-lg-9">
                                                    <asp:Panel ID="pnlVendorRegistrationRecordView" runat="server" Visible="false">
                                                        <asp:HyperLink ID="lnkVendorRegistrationRecord" runat="server" Target="_blank" Text="View File" CssClass="btn btn-link" />
                                                    </asp:Panel>
                                                    <asp:Label ID="lblVendorRegistrationRecordDash" runat="server" Text="-" Visible="true" />
                                                </div>
                                            </div>
                                            <hr class="mt-0" />

                                            <div class="form-group row">
                                                <asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Budget Transfer/Add Approval"></asp:Label>
                                                <div class="col-lg-9">
                                                    <asp:Panel ID="pnlBudgetTransferAddApprovalView" runat="server" Visible="false">
                                                        <asp:HyperLink ID="lnkBudgetTransferAddApproval" runat="server" Target="_blank" Text="View File" CssClass="btn btn-link" />
                                                    </asp:Panel>
                                                    <asp:Label ID="lblBudgetTransferAddApprovalDash" runat="server" Text="-" Visible="true" />
                                                </div>
                                            </div>
                                            <hr class="mt-0" />

                                            <div class="form-group row">
                                                <asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Other Supporting Document"></asp:Label>
                                                <div class="col-lg-9">
                                                    <asp:Panel ID="pnlOtherSupportingDocumentView" runat="server" Visible="false">
                                                        <asp:HyperLink ID="lnkOtherSupportingDocument" runat="server" Target="_blank" Text="View File" CssClass="btn btn-link" />
                                                    </asp:Panel>
                                                    <asp:Label ID="lblOtherSupportingDocumentDash" runat="server" Text="-" Visible="true" />
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
                            <div class="card card-secondary collapsed-card">
                                <div class="card-header" data-card-widget="collapse">
                                    <h3 class="card-title">Audit Trails</h3>
                                </div>
                                <div class="card-body">
                                    <div class="row">
                                        <div class="col-12">
                                            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                                <ContentTemplate>
                                                    <div class="table-responsive">
                                                        <asp:GridView ID="gvAuditTrails" runat="server" AutoGenerateColumns="false" CssClass="table table-bordered" PageSize='<%# FGV.Prodata.App.Setting.RecordsPerPage() %>' AllowPaging="true" OnPageIndexChanging="gvAuditTrails_PageIndexChanging" EmptyDataText="No record.">
                                                            <Columns>
                                                                <asp:TemplateField HeaderText="#">
                                                                    <HeaderStyle CssClass="width-30 text-center" />
                                                                    <ItemStyle CssClass="width-30 text-center" />
                                                                    <ItemTemplate>
                                                                        <%# Container.DataItemIndex + 1 %>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:BoundField DataField="ActionByName" HeaderText="Name" />
                                                                <asp:BoundField DataField="ActionByRole" HeaderText="Role" />
                                                                <asp:BoundField DataField="Action" HeaderText="Action" />
                                                                <asp:BoundField DataField="Remark" HeaderText="Remark" />
                                                                <asp:BoundField DataField="Datetime" HeaderText="Date & Time" HeaderStyle-CssClass="text-nowrap" ItemStyle-CssClass="text-nowrap" />
                                                            </Columns>
                                                            <PagerSettings Mode="NumericFirstLast" PageButtonCount="5" />
                                                            <PagerStyle CssClass="pagination-ys" />
                                                        </asp:GridView>
                                                    </div>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <script>
        $(document).ready(function () {
            bsCustomFileInput.init();

            // Bind input event to actual amount
            $('#<%= txtActualAmount.ClientID %>').on('input', toggleAllocationSections);

        // Trigger validation when allocation inputs change
        $(document).on('input', '.allocation-input', function () {
            var actualVal = parseDecimal($('#<%= txtActualAmount.ClientID %>').val());
            var estimateVal = parseDecimal($('#<%= lblAmount2.ClientID %>').text());

            if (actualVal < estimateVal) {
                var expectedTotal = estimateVal - actualVal;
                validateTotalAllocation(expectedTotal);
            }
        });

        // Run once on page load
        toggleAllocationSections();
    });

        function parseDecimal(value) {
            if (!value) return 0;
            value = value.replace(/,/g, '').replace(/RM/i, '').trim();
            let num = parseFloat(value);
            return isNaN(num) ? 0 : num;
        }

        function toggleAllocationSections() {
            var $actualInput = $('#<%= txtActualAmount.ClientID %>');
        var actualInputVal = $actualInput.val();
        if (!actualInputVal || !actualInputVal.trim()) return;

        var actualVal = parseDecimal(actualInputVal);
        var estimateVal = parseDecimal($('#<%= lblAmount2.ClientID %>').text());

        var $allocationSection = $('#<%= allocationSection.ClientID %>');
        var $allocationDropdownSection = $('#<%= allocationDropdownSection.ClientID %>');
        var $allocationBalance = $('#<%= allocationBalance.ClientID %>');
        var $allocationError = $('#<%= lblAllocationError.ClientID %>');

        $allocationSection.addClass('d-none');
        $allocationDropdownSection.addClass('d-none');
        $allocationBalance.text('');
        
        // Reset general error only if it was an allocation error context
        if($allocationError.text().indexOf('Total allocated') >= 0) {
             $allocationError.addClass('d-none').text('');
        }

        var balance = actualVal - estimateVal;
        var absBalance = Math.abs(balance).toFixed(2);
        var balanceFormatted = 'RM ' + Number(absBalance).toLocaleString('en-MY', { minimumFractionDigits: 2 });

        if (balance < 0) {
            $allocationSection.removeClass('d-none');
            $allocationBalance.text('Unallocated Balance: ' + balanceFormatted);
            validateTotalAllocation(Math.abs(balance));
        } else if (balance > 0) {
            $allocationDropdownSection.removeClass('d-none');
            $allocationBalance.text('Excess Allocation: ' + balanceFormatted);
        }
    }

    function validateTotalAllocation(expectedTotal) {
        let total = 0;

        $('.allocation-input').each(function () {
            total += parseDecimal($(this).val());
        });

        const isValid = Math.abs(total - expectedTotal) < 0.01;

        const $allocationError = $('#<%= lblAllocationError.ClientID %>');
        if (!isValid) {
            $allocationError.removeClass('d-none').text(
                'Total allocated amount (RM ' + total.toLocaleString('en-MY', { minimumFractionDigits: 2, maximumFractionDigits: 2 }) + ') must equal RM ' + expectedTotal.toLocaleString('en-MY', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
            );
        } else {
            $allocationError.addClass('d-none').text('');
        }
    }

    // Main Validation Function
    function beforeSave() {
        var hasError = false;
        var firstErrorElement = null;

        // 1. Allocation Error Check (Strict check for text or visibility)
        var $allocationError = $('#<%= lblAllocationError.ClientID %>');
        // Check if error is visible (doesn't have d-none) AND has text
        if (!$allocationError.hasClass('d-none') && $allocationError.text().trim() !== '') {
            hasError = true;
            if(!firstErrorElement) firstErrorElement = $allocationError;
        }

        // 2. File Upload Validation
        var $fileInput = $('#<%= fuPO.ClientID %>');
        var fileSelected = $fileInput.val();
        var isPoPreviouslyUploaded = $('#<%= pnlPOView.ClientID %>').is(':visible');
        var $poError = $('#<%= lblPOError.ClientID %>');

        // Check if the file upload is enabled (not disabled by "Reviewed PO" logic)
        // If disabled, we assume validation isn't needed or is handled elsewhere
        if (!$('#<%= pnlFileUpload.ClientID %>').prop('disabled')) { 
            if (!isPoPreviouslyUploaded && (!fileSelected || fileSelected.trim() === '')) {
                $poError.removeClass('d-none').text('Please upload a PO file.');
                hasError = true;
                if(!firstErrorElement) firstErrorElement = $fileInput;
            } else {
                $poError.addClass('d-none').text('');
            }
        }

        // 3. Actual Amount Validation
        var $actualAmount = $('#<%= txtActualAmount.ClientID %>');
        var actualAmountValue = $actualAmount.val().replace(/,/g, '').trim();
        var $actualError = $('#<%= lblActualAmountError.ClientID %>');

        if (!actualAmountValue) {
            $actualError.removeClass('d-none').text('Actual amount is required.');
            hasError = true;
            if(!firstErrorElement) firstErrorElement = $actualAmount;
        } else if (isNaN(actualAmountValue)) {
            $actualError.removeClass('d-none').text('Invalid actual amount.');
            hasError = true;
            if(!firstErrorElement) firstErrorElement = $actualAmount;
        } else {
            $actualError.addClass('d-none').text('');
        }

        // If there is an error, scroll to the first one so the user sees it
        if (hasError && firstErrorElement) {
            $('html, body').animate({
                scrollTop: firstErrorElement.offset().top - 150
            }, 500);
        }

        return !hasError;
    }

    // Handles Confirmation and Postback
    function confirmSave(e) {
        // 1. Run Validation
        if (!beforeSave()) {
            return false; // Stop immediately if validation fails
        }

        // 2. Stop the default ASP.NET postback temporarily
        if (e) e.preventDefault();

        // 3. Show SweetAlert Confirmation
        Swal.fire({
            title: 'Are you sure?',
            text: "Do you want to save these details?",
            icon: 'question',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, Save it!'
        }).then((result) => {
            if (result.isConfirmed) {
                // 4. If User Confirms: Show Preloader
                $("#pagePreloader").fadeIn(200);

                // 5. Manually trigger the Server-Side Click
                // This targets the specific Button ID to fire the CodeBehind logic
                __doPostBack('<%= btnSave.UniqueID %>', '');
            }
        });

            return false; // Prevent default click action
        }
    </script>

</asp:Content>--%>