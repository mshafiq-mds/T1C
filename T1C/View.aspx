<%@ Page Title="T1C" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="View.aspx.cs" Inherits="Prodata.WebForm.T1C.View" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="hdnFormId" runat="server" />

    <div class="row">
        <div class="col-lg-12">
            <div class="card card-outline">
                <div class="card-header card-header-sticky">
                    <h3 class="card-title">
                        <asp:Label ID="lblTitle" runat="server"></asp:Label>
                    </h3>
                    <div class="card-tools">
                        <asp:LinkButton ID="btnBack" runat="server" CssClass="btn btn-default" PostBackUrl="~/T1C/Default" CausesValidation="false">
                            <i class="fas fa-angle-double-left"></i> Back
                        </asp:LinkButton>
                        <asp:LinkButton ID="btnEdit" runat="server" CssClass="btn btn-outline-secondary" OnClick="btnEdit_Click" CausesValidation="false">
                            <i class="fas fa-edit"></i> Edit
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
                                                <th colspan="2" class="text-center">Current Year (RM/MT)</th>
                                                <th colspan="3" class="text-center">YTD Past Year (RM/MT)</th>
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
</asp:Content>
