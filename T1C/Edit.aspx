<%@ Page Title="Edit Budget T1C" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Prodata.WebForm.T1C.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .custom-control.custom-radio {
            margin-bottom: -15px;
        }

        .custom-control-label {
            font-weight: normal !important;
        }
    </style>
    <asp:HiddenField ID="hdnFormId" runat="server" />
    <div class="row">
        <div class="col-lg-12">
            <div class="card card-outline">
                <div class="card-header card-header-sticky">
                    <h3 class="card-title d-none d-sm-inline"><%: Page.Title %></h3>
                    <div class="card-tools">
                        <asp:LinkButton ID="btnBack" runat="server" CssClass="btn btn-default" PostBackUrl="~/T1C/Default" CausesValidation="false">
                            <i class="fas fa-angle-double-left"></i> Back
                        </asp:LinkButton>
                        <asp:LinkButton ID="btnSave" runat="server" CssClass="btn btn-primary" OnClick="btnSave_Click" OnClientClick="return collectData();">
                            <i class="fas fa-save"></i> Save
                        </asp:LinkButton>
                        <asp:LinkButton ID="btnSubmit" runat="server" CssClass="btn btn-success" OnClick="btnSubmit_Click" OnClientClick="return collectData();">
                            <i class="fas fa-share"></i> <asp:Label ID="btnSubmitLabel" runat="server" Text="Submit"></asp:Label>
                        </asp:LinkButton>
                    </div>
                </div>
                <div class="card-body">
                    <asp:HiddenField ID="hdnAllocationList" runat="server" />
                    <asp:HiddenField ID="hdnVendorList" runat="server" />
                    <div class="row">
                        <div class="col-lg-12">
                            <div id="alert" class="alert alert-danger alert-dismissable" role="alert" runat="server">
			                    <button href="#" class="close" data-dismiss="alert" aria-label="close">&times;</button>
                                <asp:Label ID="lblAlert" runat="server"></asp:Label>
                            </div>
                            <div class="form-group row">
                                <asp:Label ID="lblBA" runat="server" CssClass="col-lg-2 col-sm-3 col-form-label" AssociatedControlID="ddlBA" Text="BA"></asp:Label>
                                <div class="col-lg-6 col-sm-5">
                                    <asp:Label ID="lblBAText" runat="server" CssClass="form-control text-muted" Visible="false"></asp:Label>
                                    <asp:DropDownList ID="ddlBA" runat="server" CssClass="form-control select2" data-placeholder="BA"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvBA" runat="server" ControlToValidate="ddlBA" CssClass="text-danger" Display="Dynamic" ErrorMessage="Please select BA" InitialValue=""></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-lg-8 col-sm-7">
                                    <div class="form-group row">
                                        <asp:Label ID="lblRefNo" runat="server" CssClass="col-lg-3 col-sm-5 col-form-label" AssociatedControlID="txtRefNo" Text="Reference No"></asp:Label>
                                        <div class="col-lg-9 col-sm-7">
                                            <asp:TextBox ID="txtRefNo" runat="server" CssClass="form-control" placeholder="Reference No" ReadOnly="true"></asp:TextBox>
                                            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtRefNo" CssClass="text-danger" Display="Dynamic" ErrorMessage="Please enter reference no"></asp:RequiredFieldValidator>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-lg-4 col-sm-5">
                                    <div class="form-group row">
                                        <asp:Label ID="lblDate" runat="server" CssClass="col-lg-3 col-sm-4 col-form-label" AssociatedControlID="txtDate" Text="Date"></asp:Label>
                                        <div class="col-lg-9 col-sm-8">
                                            <asp:TextBox ID="txtDate" runat="server" CssClass="form-control" placeholder="Tarikh" TextMode="Date"></asp:TextBox>
                                            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtDate" CssClass="text-danger" Display="Dynamic" ErrorMessage="Please enter date"></asp:RequiredFieldValidator>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group row">
                                <asp:Label ID="lblDetails" runat="server" CssClass="col-lg-2 col-sm-3 col-form-label" AssociatedControlID="txtDetails" Text="Details"></asp:Label>
                                <div class="col-lg-10 col-sm-9">
                                    <asp:TextBox ID="txtDetails" runat="server" CssClass="form-control" placeholder="Details..." TextMode="MultiLine" Rows="3"></asp:TextBox>
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtDetails" CssClass="text-danger" Display="Dynamic" ErrorMessage="Please enter details"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="form-group row">
                                <asp:Label ID="lblJustificationOfNeed" runat="server" CssClass="col-lg-2 col-sm-3 col-form-label" AssociatedControlID="txtJustificationOfNeed" Text="Justification of Need"></asp:Label>
                                <div class="col-lg-10 col-sm-9">
                                    <asp:TextBox ID="txtJustificationOfNeed" runat="server" CssClass="form-control" placeholder="Justification of need..." TextMode="MultiLine" Rows="3"></asp:TextBox>
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtJustificationOfNeed" CssClass="text-danger" Display="Dynamic" ErrorMessage="Please enter justification of need"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="form-group row">
                                <asp:Label ID="lblAmount" runat="server" CssClass="col-lg-2 col-sm-3 col-form-label" AssociatedControlID="txtAmount" Text="Estimate Amount"></asp:Label>
                                <div class="col-lg-6 col-sm-5">
                                    <div class="input-group">
                                        <div class="input-group-prepend">
                                            <span class="input-group-text">RM</span>
                                        </div>
                                        <asp:TextBox ID="txtAmount" runat="server" CssClass="form-control input-number2" placeholder="Estimate Amount"></asp:TextBox>
                                    </div>
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtAmount" CssClass="text-danger" Display="Dynamic" ErrorMessage="Please enter estimate amount (RM)"></asp:RequiredFieldValidator>
                                    <asp:CustomValidator ID="cvAmountLimit" runat="server"
                                        ControlToValidate="txtAmount"
                                        OnServerValidate="cvAmountLimit_ServerValidate"
                                        ClientValidationFunction="validateAmountLimit"
                                        EnableClientScript="true"
                                        CssClass="text-danger" Display="Dynamic"
                                        ErrorMessage="Amount is outside the allowed range.">
                                    </asp:CustomValidator>
                                </div>
                            </div>
                            <div class="form-group row">
                                <asp:Label ID="lblAllocation" runat="server" CssClass="col-lg-2 col-sm-3 col-form-label text-bold" Text="Allocation"></asp:Label>
                                <div class="col-lg-10 col-sm-9">
                                    <div id="allocationContainer">
                                        <%--<div class="input-group mb-2 allocation-group">
                                            <asp:DropDownList ID="ddlAllocation" runat="server" CssClass="form-control select2 allocation-input" data-placeholder="Peruntukan"></asp:DropDownList>
                                            <asp:TextBox ID="txtAllocationAmount" runat="server" CssClass="form-control input-number2" placeholder="Amount"></asp:TextBox>
                                            <div class="input-group-append">
                                                <button type="button" class="btn btn-danger btnRemoveAllocation" disabled>
                                                    <i class="fa fa-minus"></i>
                                                </button>
                                            </div>
                                        </div>--%>
                                    </div>

                                    <!-- Plus button below dropdowns -->
                                    <button type="button" class="btn btn-info mt-1" id="btnAddAllocation">
                                        <i class="fa fa-plus"></i> Add Allocation
                                    </button>
                                </div>
                            </div>
                            <div class="form-group row">
                                <asp:Label ID="lblProcurementType" runat="server" CssClass="col-lg-2 col-sm-3 col-form-label" AssociatedControlID="rblProcurementType" Text="Quotation Method"></asp:Label>
                                <div class="col-lg-10 col-sm-9 mb-3">
                                    <asp:RadioButtonList ID="rblProcurementType" runat="server" RepeatLayout="Flow">
                                        <asp:ListItem Text="Inclusive Quotation" Value="quotation_inclusive"></asp:ListItem>
                                        <asp:ListItem Text="Selective Quotation" Value="quotation_selective"></asp:ListItem>
                                        <asp:ListItem Text="Direct Negotiation" Value="direct_negotiation"></asp:ListItem>
                                    </asp:RadioButtonList>
                                </div>
                            </div>
                            <div class="form-group row">
                                <asp:Label ID="lblVendor" runat="server" CssClass="col-lg-2 col-sm-3 col-form-label" AssociatedControlID="txtVendor" Text="Contractor"></asp:Label>
                                <div class="col-lg-10 col-sm-9">
                                    <div id="vendorContainer">
                                        <div class="input-group mb-2 vendor-group d-none">
                                            <asp:TextBox ID="txtVendor" runat="server" CssClass="form-control vendor-input" placeholder="Contractor"></asp:TextBox>
                                            <div class="input-group-append">
                                                <button type="button" class="btn btn-danger btnRemoveVendor" disabled>
                                                    <i class="fa fa-minus"></i>
                                                </button>
                                            </div>
                                        </div>
                                    </div>

                                    <!-- Plus button below textboxes -->
                                    <button type="button" class="btn btn-info mt-1" id="btnAddVendor">
                                        <i class="fa fa-plus"></i> Add Contractor
                                    </button>
                                </div>
                            </div>
                            <div class="form-group row" id="divJustificationDirectAward" style="display: none;">
                                <asp:Label ID="lblJustificationDirectAward" runat="server" CssClass="col-lg-2 col-sm-3" AssociatedControlID="txtJustificationDirectAward" Text="Justification (Direct Negotiation)"></asp:Label>
                                <div class="col-lg-10 col-sm-9">
                                    <asp:TextBox ID="txtJustificationDirectAward" runat="server" CssClass="form-control" placeholder="Justification direct negotiation..." TextMode="MultiLine" Rows="3"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group row">
                                <asp:Label ID="lblDataKos" runat="server" CssClass="col-lg-2 col-sm-3 col-form-label" AssociatedControlID="tblDataKos" Text="Data Cost"></asp:Label>
                                <div class="col-lg-8 col-sm-7 mb-3">
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
                                                    <td>
                                                        <asp:TextBox ID="txtCurrentYearActualYTD" runat="server" CssClass="form-control input-number2"></asp:TextBox></td>
                                                    <td>
                                                        <asp:TextBox ID="txtCurrentYearBudget" runat="server" CssClass="form-control input-number2"></asp:TextBox></td>
                                                    <td>
                                                        <asp:TextBox ID="txtPreviousYearActualYTD" runat="server" CssClass="form-control input-number2"></asp:TextBox></td>
                                                    <td>
                                                        <asp:TextBox ID="txtPreviousYearActual" runat="server" CssClass="form-control input-number2"></asp:TextBox></td>
                                                    <td>
                                                        <asp:TextBox ID="txtPreviousYearBudget" runat="server" CssClass="form-control input-number2"></asp:TextBox></td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                    <table style="width: 100%;">
                                        <tbody>
                                            <tr>
                                                <th class="text-center align-middle pl-2 pr-1">A</th>
                                                <td>
                                                    <asp:TextBox ID="txtA" runat="server" CssClass="form-control input-number2"></asp:TextBox></td>
                                                <th class="text-center align-middle pl-2 pr-1">C</th>
                                                <td>
                                                    <asp:TextBox ID="txtC" runat="server" CssClass="form-control input-number2"></asp:TextBox></td>
                                            </tr>
                                            <tr>
                                                <th class="text-center align-middle pl-2 pr-1">B</th>
                                                <td>
                                                    <asp:TextBox ID="txtB" runat="server" CssClass="form-control"></asp:TextBox></td>
                                                <th class="text-center align-middle pl-2 pr-1">D</th>
                                                <td>
                                                    <asp:TextBox ID="txtD" runat="server" CssClass="form-control input-number2"></asp:TextBox></td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                            <div class="form-group row">
                                <asp:Label ID="lblDocuments" runat="server" CssClass="col-lg-2 col-sm-3 col-form-label text-bold" Text="Documents"></asp:Label>
                                <div class="col-lg-10 col-sm-9">
                                    <div class="row">
                                        <div class="col-12">

                                            <%-- Utility function for each document type --%>
                                            <%-- Repeat this block for each document --%>
                                            <%-- Replace XXX with each file identifier --%>

                                            <!-- Picture -->
                                            <div class="form-group row">
                                                <asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Picture"></asp:Label>
                                                <div class="col-lg-9">
                                                    <asp:Panel ID="pnlPictureView" runat="server" Visible="false">
                                                        <asp:HyperLink ID="lnkPicture" runat="server" Target="_blank" Text="View Picture" CssClass="btn btn-link" />
                                                        <asp:Button ID="btnDeletePicture" runat="server" Text="Delete" CssClass="btn btn-danger btn-sm" OnClick="btnDeletePicture_Click" CausesValidation="false" />
                                                    </asp:Panel>
                                                    <asp:Panel ID="pnlPictureUpload" runat="server" Visible="true">
                                                        <div class="custom-file">
                                                            <asp:FileUpload ID="fuPicture" runat="server" CssClass="custom-file-input" />
                                                            <asp:Label runat="server" AssociatedControlID="fuPicture" CssClass="custom-file-label" Text="Choose picture" />
                                                        </div>
                                                    </asp:Panel>
                                                </div>
                                            </div>
                                            <hr class="mt-0" />

                                            <!-- Machine Repair History -->
                                            <div class="form-group row">
                                                <asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Machine Repair History"></asp:Label>
                                                <div class="col-lg-9">
                                                    <asp:Panel ID="pnlMachineRepairHistoryView" runat="server" Visible="false">
                                                        <asp:HyperLink ID="lnkMachineRepairHistory" runat="server" Target="_blank" Text="View File" CssClass="btn btn-link" />
                                                        <asp:Button ID="btnDeleteMachineRepairHistory" runat="server" Text="Delete" CssClass="btn btn-danger btn-sm" OnClick="btnDeleteMachineRepairHistory_Click" CausesValidation="false" />
                                                    </asp:Panel>
                                                    <asp:Panel ID="pnlMachineRepairHistoryUpload" runat="server" Visible="true">
                                                        <div class="custom-file">
                                                            <asp:FileUpload ID="fuMachineRepairHistory" runat="server" CssClass="custom-file-input" />
                                                            <asp:Label runat="server" AssociatedControlID="fuMachineRepairHistory" CssClass="custom-file-label" Text="Choose machine repair history (IW38)" />
                                                        </div>
                                                    </asp:Panel>
                                                </div>
                                            </div>
                                            <hr class="mt-0" />

                                            <!-- Job Specification -->
                                            <div class="form-group row">
                                                <asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Job Specification"></asp:Label>
                                                <div class="col-lg-9">
                                                    <asp:Panel ID="pnlJobSpecificationView" runat="server" Visible="false">
                                                        <asp:HyperLink ID="lnkJobSpecification" runat="server" Target="_blank" Text="View File" CssClass="btn btn-link" />
                                                        <asp:Button ID="btnDeleteJobSpecification" runat="server" Text="Delete" CssClass="btn btn-danger btn-sm" OnClick="btnDeleteJobSpecification_Click" CausesValidation="false" />
                                                    </asp:Panel>
                                                    <asp:Panel ID="pnlJobSpecificationUpload" runat="server" Visible="true">
                                                        <div class="custom-file">
                                                            <asp:FileUpload ID="fuJobSpecification" runat="server" CssClass="custom-file-input" />
                                                            <asp:Label runat="server" AssociatedControlID="fuJobSpecification" CssClass="custom-file-label" Text="Choose job specification" />
                                                        </div>
                                                    </asp:Panel>
                                                </div>
                                            </div>
                                            <hr class="mt-0" />

                                            <!-- Engineer Estimate Price -->
                                            <div class="form-group row">
                                                <asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Engineer's Estimate Price"></asp:Label>
                                                <div class="col-lg-9">
                                                    <asp:Panel ID="pnlEngineerEstimatePriceView" runat="server" Visible="false">
                                                        <asp:HyperLink ID="lnkEngineerEstimatePrice" runat="server" Target="_blank" Text="View File" CssClass="btn btn-link" />
                                                        <asp:Button ID="btnDeleteEngineerEstimatePrice" runat="server" Text="Delete" CssClass="btn btn-danger btn-sm" OnClick="btnDeleteEngineerEstimatePrice_Click" CausesValidation="false" />
                                                    </asp:Panel>
                                                    <asp:Panel ID="pnlEngineerEstimatePriceUpload" runat="server" Visible="true">
                                                        <div class="custom-file">
                                                            <asp:FileUpload ID="fuEngineerEstimatePrice" runat="server" CssClass="custom-file-input" />
                                                            <asp:Label runat="server" AssociatedControlID="fuEngineerEstimatePrice" CssClass="custom-file-label" Text="Choose engineer's estimate price" />
                                                        </div>
                                                    </asp:Panel>
                                                </div>
                                            </div>
                                            <hr class="mt-0" />

                                            <!-- Dec Cost Report Current Year -->
                                            <div class="form-group row">
                                                <asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Dec Cost Report (current year)"></asp:Label>
                                                <div class="col-lg-9">
                                                    <asp:Panel ID="pnlDecCostReportCurrentYearView" runat="server" Visible="false">
                                                        <asp:HyperLink ID="lnkDecCostReportCurrentYear" runat="server" Target="_blank" Text="View File" CssClass="btn btn-link" />
                                                        <asp:Button ID="btnDeleteDecCostReportCurrentYear" runat="server" Text="Delete" CssClass="btn btn-danger btn-sm" OnClick="btnDeleteDecCostReportCurrentYear_Click" CausesValidation="false" />
                                                    </asp:Panel>
                                                    <asp:Panel ID="pnlDecCostReportCurrentYearUpload" runat="server" Visible="true">
                                                        <div class="custom-file">
                                                            <asp:FileUpload ID="fuDecCostReportCurrentYear" runat="server" CssClass="custom-file-input" />
                                                            <asp:Label runat="server" AssociatedControlID="fuDecCostReportCurrentYear" CssClass="custom-file-label" Text="Choose dec cost report (current year)" />
                                                        </div>
                                                    </asp:Panel>
                                                </div>
                                            </div>
                                            <hr class="mt-0" />

                                            <!-- Dec Cost Report Last Year -->
                                            <div class="form-group row">
                                                <asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Dec Cost Report (last year)"></asp:Label>
                                                <div class="col-lg-9">
                                                    <asp:Panel ID="pnlDecCostReportLastYearView" runat="server" Visible="false">
                                                        <asp:HyperLink ID="lnkDecCostReportLastYear" runat="server" Target="_blank" Text="View File" CssClass="btn btn-link" />
                                                        <asp:Button ID="btnDeleteDecCostReportLastYear" runat="server" Text="Delete" CssClass="btn btn-danger btn-sm" OnClick="btnDeleteDecCostReportLastYear_Click" CausesValidation="false" />
                                                    </asp:Panel>
                                                    <asp:Panel ID="pnlDecCostReportLastYearUpload" runat="server" Visible="true">
                                                        <div class="custom-file">
                                                            <asp:FileUpload ID="fuDecCostReportLastYear" runat="server" CssClass="custom-file-input" />
                                                            <asp:Label runat="server" AssociatedControlID="fuDecCostReportLastYear" CssClass="custom-file-label" Text="Choose dec cost report (last year)" />
                                                        </div>
                                                    </asp:Panel>
                                                </div>
                                            </div>
                                            <hr class="mt-0" />

                                            <!-- Cost Report Last Month -->
                                            <div class="form-group row">
                                                <asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Cost Report (last month)"></asp:Label>
                                                <div class="col-lg-9">
                                                    <asp:Panel ID="pnlCostReportLastMonthView" runat="server" Visible="false">
                                                        <asp:HyperLink ID="lnkCostReportLastMonth" runat="server" Target="_blank" Text="View File" CssClass="btn btn-link" />
                                                        <asp:Button ID="btnDeleteCostReportLastMonth" runat="server" Text="Delete" CssClass="btn btn-danger btn-sm" OnClick="btnDeleteCostReportLastMonth_Click" CausesValidation="false" />
                                                    </asp:Panel>
                                                    <asp:Panel ID="pnlCostReportLastMonthUpload" runat="server" Visible="true">
                                                        <div class="custom-file">
                                                            <asp:FileUpload ID="fuCostReportLastMonth" runat="server" CssClass="custom-file-input" />
                                                            <asp:Label runat="server" AssociatedControlID="fuCostReportLastMonth" CssClass="custom-file-label" Text="Choose cost report (last month)" />
                                                        </div>
                                                    </asp:Panel>
                                                </div>
                                            </div>
                                            <hr class="mt-0" />

                                            <!-- Drawing/Sketching -->
                                            <div class="form-group row">
                                                <asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Drawing / Sketching"></asp:Label>
                                                <div class="col-lg-9">
                                                    <asp:Panel ID="pnlDrawingSketchingView" runat="server" Visible="false">
                                                        <asp:HyperLink ID="lnkDrawingSketching" runat="server" Target="_blank" Text="View File" CssClass="btn btn-link" />
                                                        <asp:Button ID="btnDeleteDrawingSketching" runat="server" Text="Delete" CssClass="btn btn-danger btn-sm" OnClick="btnDeleteDrawingSketching_Click" CausesValidation="false" />
                                                    </asp:Panel>
                                                    <asp:Panel ID="pnlDrawingSketchingUpload" runat="server" Visible="true">
                                                        <div class="custom-file">
                                                            <asp:FileUpload ID="fuDrawingSketching" runat="server" CssClass="custom-file-input" />
                                                            <asp:Label runat="server" AssociatedControlID="fuDrawingSketching" CssClass="custom-file-label" Text="Choose drawing / sketching" />
                                                        </div>
                                                    </asp:Panel>
                                                </div>
                                            </div>
                                            <hr class="mt-0" />

                                            <!-- Quotation -->
                                            <div class="form-group row">
                                                <asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Quotation (direct negotiation only)"></asp:Label>
                                                <div class="col-lg-9">
                                                    <asp:Panel ID="pnlQuotationView" runat="server" Visible="false">
                                                        <asp:HyperLink ID="lnkQuotation" runat="server" Target="_blank" Text="View File" CssClass="btn btn-link" />
                                                        <asp:Button ID="btnDeleteQuotation" runat="server" Text="Delete" CssClass="btn btn-danger btn-sm" OnClick="btnDeleteQuotation_Click" CausesValidation="false" />
                                                    </asp:Panel>
                                                    <asp:Panel ID="pnlQuotationUpload" runat="server" Visible="true">
                                                        <div class="custom-file">
                                                            <asp:FileUpload ID="fuQuotation" runat="server" CssClass="custom-file-input" />
                                                            <asp:Label runat="server" AssociatedControlID="fuQuotation" CssClass="custom-file-label" Text="Choose quotation (direct negotiation only)" />
                                                        </div>
                                                    </asp:Panel>
                                                </div>
                                            </div>
                                            <hr class="mt-0" />

                                            <!-- Damage Investigation Report -->
                                            <div class="form-group row">
                                                <asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Damage Investigation Report (Shovel / Prime Mover / Turbine / Alternator / Genset)"></asp:Label>
                                                <div class="col-lg-9">
                                                    <asp:Panel ID="pnlDamageInvestigationReportView" runat="server" Visible="false">
                                                        <asp:HyperLink ID="lnkDamageInvestigationReport" runat="server" Target="_blank" Text="View File" CssClass="btn btn-link" />
                                                        <asp:Button ID="btnDeleteDamageInvestigationReport" runat="server" Text="Delete" CssClass="btn btn-danger btn-sm" OnClick="btnDeleteDamageInvestigationReport_Click" CausesValidation="false" />
                                                    </asp:Panel>
                                                    <asp:Panel ID="pnlDamageInvestigationReportUpload" runat="server" Visible="true">
                                                        <div class="custom-file">
                                                            <asp:FileUpload ID="fuDamageInvestigationReport" runat="server" CssClass="custom-file-input" />
                                                            <asp:Label runat="server" AssociatedControlID="fuDamageInvestigationReport" CssClass="custom-file-label" Text="Choose damage investigation report" />
                                                        </div>
                                                    </asp:Panel>
                                                </div>
                                            </div>
                                            <hr class="mt-0" />

                                            <!-- Vendor Registration Record -->
                                            <div class="form-group row">
                                                <asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Vendor Registration Record"></asp:Label>
                                                <div class="col-lg-9">
                                                    <asp:Panel ID="pnlVendorRegistrationRecordView" runat="server" Visible="false">
                                                        <asp:HyperLink ID="lnkVendorRegistrationRecord" runat="server" Target="_blank" Text="View File" CssClass="btn btn-link" />
                                                        <asp:Button ID="btnDeleteVendorRegistrationRecord" runat="server" Text="Delete" CssClass="btn btn-danger btn-sm" OnClick="btnDeleteVendorRegistrationRecord_Click" CausesValidation="false" />
                                                    </asp:Panel>
                                                    <asp:Panel ID="pnlVendorRegistrationRecordUpload" runat="server" Visible="true">
                                                        <div class="custom-file">
                                                            <asp:FileUpload ID="fuVendorRegistrationRecord" runat="server" CssClass="custom-file-input" />
                                                            <asp:Label runat="server" AssociatedControlID="fuVendorRegistrationRecord" CssClass="custom-file-label" Text="Choose vendor registration record" />
                                                        </div>
                                                    </asp:Panel>
                                                </div>
                                            </div>
                                            <hr class="mt-0" />

                                            <!-- Budget Transfer/Add Approval -->
                                            <div class="form-group row">
                                                <asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Approval of Budget Transfer/Add"></asp:Label>
                                                <div class="col-lg-9">
                                                    <asp:Panel ID="pnlBudgetTransferAddApprovalView" runat="server" Visible="false">
                                                        <asp:HyperLink ID="lnkBudgetTransferAddApproval" runat="server" Target="_blank" Text="View File" CssClass="btn btn-link" />
                                                        <asp:Button ID="btnDeleteBudgetTransferAddApproval" runat="server" Text="Delete" CssClass="btn btn-danger btn-sm" OnClick="btnDeleteBudgetTransferAddApproval_Click" CausesValidation="false" />
                                                    </asp:Panel>
                                                    <asp:Panel ID="pnlBudgetTransferAddApprovalUpload" runat="server" Visible="true">
                                                        <div class="custom-file">
                                                            <asp:FileUpload ID="fuBudgetTransferAddApproval" runat="server" CssClass="custom-file-input" />
                                                            <asp:Label runat="server" AssociatedControlID="fuBudgetTransferAddApproval" CssClass="custom-file-label" Text="Choose approval of budget tranfer/add" />
                                                        </div>
                                                    </asp:Panel>
                                                </div>
                                            </div>
                                            <hr class="mt-0" />

                                            <!-- Other Supporting Document -->
                                            <div class="form-group row">
                                                <asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Other Supporting Documents (JKKP/JAS/SHO/NDT Report)"></asp:Label>
                                                <div class="col-lg-9">
                                                    <asp:Panel ID="pnlOtherSupportingDocumentView" runat="server" Visible="false">
                                                        <asp:HyperLink ID="lnkOtherSupportingDocument" runat="server" Target="_blank" Text="View File" CssClass="btn btn-link" />
                                                        <asp:Button ID="btnDeleteOtherSupportingDocument" runat="server" Text="Delete" CssClass="btn btn-danger btn-sm" OnClick="btnDeleteOtherSupportingDocument_Click" CausesValidation="false" />
                                                    </asp:Panel>
                                                    <asp:Panel ID="pnlOtherSupportingDocumentUpload" runat="server" Visible="true">
                                                        <div class="custom-file">
                                                            <asp:FileUpload ID="fuOtherSupportingDocument" runat="server" CssClass="custom-file-input" />
                                                            <asp:Label runat="server" AssociatedControlID="fuOtherSupportingDocument" CssClass="custom-file-label" Text="Choose other supporting document" />
                                                        </div>
                                                    </asp:Panel>
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
        </div>
    </div>
    <script>
        $(document).ready(function () {
            bsCustomFileInput.init();

            let radioListId = "<%= rblProcurementType.ClientID %>";
            let $radioList = $("#" + radioListId);
            let $extraDiv = $("#divJustificationDirectAward");

            if (!$radioList.length) return; // Ensure the control exists

            // Convert to Bootstrap custom radio buttons
            $radioList.find("input[type='radio']").each(function () {
                let $radio = $(this);
                let $labelSpan = $radio.next(); // ASP.NET renders text inside <span>
                let labelText = $labelSpan.text().trim();

                if (labelText === "") return;

                let radioId = $radio.attr("id");
                let wrapper = $("<div>").addClass("custom-control custom-radio");
                let label = $("<label>").addClass("custom-control-label").attr("for", radioId).text(labelText);

                $radio.addClass("custom-control-input");
                $labelSpan.hide(); // Hide default ASP.NET label

                $radio.before(wrapper);
                wrapper.append($radio).append(label);
            });

            // Handle selection change
            $radioList.on("change", "input[type='radio']", function () {
                if ($(this).val() === "direct_negotiation") {
                    $extraDiv.show(); // Show div
                } else {
                    $extraDiv.hide(); // Hide div
                }
            });

            $("#btnAddAllocation").click(function () {
                addAllocationDropdown("");
            });

            $("#btnAddVendor").click(function () {
                addVendorInput("");
            });

            $(document).on("click", ".btnRemoveAllocation", function () {
                if ($(".allocation-group").length > 1) {
                    $(this).closest(".allocation-group").remove();
                    updateRemoveButtons("#allocationContainer", ".btnRemoveAllocation");
                }
            });

            $(document).on("click", ".btnRemoveVendor", function () {
                if ($(".vendor-group").length > 1) {
                    $(this).closest(".vendor-group").remove();
                    updateRemoveButtons("#vendorContainer", ".btnRemoveVendor");
                }
            });

            $(document).on("change", ".allocation-input", function () {
                var $input = $(this);
                var $row = $input.closest(".allocation-group");
                var $amountInput = $row.find(".input-number2");
                var selectedOption = $input.find("option:selected");
                var max = selectedOption.data("max");

                // Enable or disable the amount input based on selection
                if ($input.val()) {
                    $amountInput.prop("disabled", false);
                } else {
                    $amountInput.prop("disabled", true).val('');
                }

                // Set or remove data-max attribute on amount input
                if (max !== undefined && max !== null && max !== "") {
                    $amountInput.attr("data-max", max);
                } else {
                    $amountInput.removeAttr("data-max");
                }

                // Calculate and display balance
                var enteredAmount = parseFloat($amountInput.val().replace(/,/g, '')) || 0;
                var balance = parseFloat(max) - enteredAmount;

                if (isNaN(balance)) {
                    balance = 0;
                }

                $row.find("span.balance").text("Balance: RM " + balance.toLocaleString(undefined, {
                    minimumFractionDigits: 2,
                    maximumFractionDigits: 2
                }));
            });

            // On blur: format value, show error if over max, update balance
            $(document).on('blur', '.allocation-group .input-number2', function () {
                var $input = $(this);
                var $row = $input.closest('.allocation-group');
                $input.attr('type', 'text');

                var maxAttr = $input.attr('data-max');
                var raw = $input.val().replace(/,/g, '');
                var value = parseFloat(raw);
                $row.find('.validation-error').remove(); // remove existing error span

                if (!isNaN(value)) {
                    //$input.val(value.toLocaleString(undefined, {
                    //    minimumFractionDigits: 2,
                    //    maximumFractionDigits: 2
                    //}));

                    if (maxAttr && value > parseFloat(maxAttr)) {
                        var $errorSpan = $('<span class="text-danger validation-error d-block mb-1"></span>');
                        $errorSpan.text('Amount exceeds the allowed maximum of RM ' +
                            parseFloat(maxAttr).toLocaleString(undefined, {
                                minimumFractionDigits: 2,
                                maximumFractionDigits: 2
                            }));
                        $row.append($errorSpan); // <-- append inside row instead of after input-group
                    }
                }

                updateRowBalance($row);
            });

            // On input: clear error if any, update balance
            $(document).on('input', '.allocation-group .input-number2', function () {
                var $input = $(this);
                var $row = $input.closest('.allocation-group');

                $row.find('.validation-error').remove();
                updateRowBalance($row);
            });

            // Attach to allocation dropdown changes
            $(document).on("change", ".allocation-input", updateAllocationDropdowns);

            // Also call after adding a new allocation row
            $("#btnAddAllocation").click(function () {
                setTimeout(updateAllocationDropdowns, 200);
            });

            // Call once on page load
            loadInitialData();
            updateAllocationDropdowns();
        });

        function updateRemoveButtons(container, btnClass) {
            var totalInputs = $(container).find(".input-group").length;
            $(btnClass).prop("disabled", totalInputs === 1);
        }

        function loadAllocations(dropdown) {
            var formId = $('#<%= hdnFormId.ClientID %>').val();
            $.ajax({
                type: "POST",
                url: "Edit.aspx/GetBudgets",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify({ excludedFormId: formId }),
                dataType: "json",
                success: function (response) {
                    var options = '<option value=""></option>';
                    $.each(response.d, function (index, item) {
                        options += `<option value="${item.Id}" data-max="${item.Amount.replace(/,/g, '')}">${item.DisplayName}</option>`;
                    });
                    dropdown.html(options).trigger("loadComplete");
                }
            });
        }

        function addAllocationDropdown(selectedId, amount = "", isFirst) {
            var $group = $(`
                <div class="input-group input-group-sm mb-2 allocation-group">
                    <select class="form-control form-control-sm select2 allocation-input col-md-6" data-placeholder="Allocation"></select>
                    <input type="text" class="form-control form-control-sm input-number2" placeholder="Amount" disabled />
                    <div class="input-group-append">
                        <button type="button" class="btn btn-danger btnRemoveAllocation">
                            <i class="fa fa-minus"></i>
                        </button>
                    </div>
                    <span class="balance col-md-3 mt-1 ml-2">Balance: RM 0.00</span>
                </div>
            `);

            $("#allocationContainer").append($group);

            var $select = $group.find("select");
            var $amountInput = $group.find("input.input-number2");

            $amountInput.val(amount);

            // Load allocation options
            loadAllocations($select);

            $select.one("loadComplete", function () {
                $select.val(selectedId).trigger("change");
            });

            $select.select2({
                allowClear: true,
                theme: "bootstrap4",
                dropdownParent: $('body')
            }).next('.select2-container').find('.select2-selection').addClass('form-control-sm');

            updateRemoveButtons("#allocationContainer", ".btnRemoveAllocation");

            // Enable amount input only when allocation is selected
            $select.on('change', function () {
                if ($(this).val()) {
                    $amountInput.prop('disabled', false);
                } else {
                    $amountInput.prop('disabled', true);
                }
            })

            // Reinitialize amount input formatting
            $amountInput.attr('type', 'text');

            $amountInput.on('focus', function () {
                $(this).val($(this).val().replace(/,/g, ''));
                $(this).attr('type', 'number');
            });

            $amountInput.on('blur', function () {
                $(this).attr('type', 'text');
                if ($(this).val()) {
                    var value = parseFloat($(this).val());
                    if (!isNaN(value)) {
                        $(this).val(value.toLocaleString(undefined, {
                            minimumFractionDigits: 2,
                            maximumFractionDigits: 2
                        }));
                    }
                }
            });
        }

        // Prevent duplicate allocation selection
        function updateAllocationDropdowns() {
            var selected = [];
            $(".allocation-input").each(function () {
                var val = $(this).val();
                if (val) selected.push(val);
            });

            $(".allocation-input").each(function () {
                var $this = $(this);
                var currentVal = $this.val();
                $this.find("option").each(function () {
                    var optVal = $(this).val();
                    if (optVal === "" || optVal === currentVal) {
                        $(this).prop("disabled", false);
                    } else if (selected.includes(optVal)) {
                        $(this).prop("disabled", true);
                    } else {
                        $(this).prop("disabled", false);
                    }
                });
                // Refresh Select2 if used
                if ($this.hasClass("select2-hidden-accessible")) {
                    $this.trigger("change.select2");
                }
            });
        }

        function addVendorInput(value) {
            var $group = $(`
                <div class="input-group input-group-sm mb-2 vendor-group">
                    <input type="text" class="form-control form-control-sm vendor-input" placeholder="Kontraktor">
                    <div class="input-group-append">
                        <button type="button" class="btn btn-danger btnRemoveVendor">
                            <i class="fa fa-minus"></i>
                        </button>
                    </div>
                </div>
            `);

            $group.find(".vendor-input").val(value);
            $("#vendorContainer").append($group);
            updateRemoveButtons("#vendorContainer", ".btnRemoveVendor");
        }

        function loadInitialData() {
            const formId = $("#<%= hdnFormId.ClientID %>").val();
            if (formId) {
                $.ajax({
                    type: "POST",
                    url: "Edit.aspx/GetSelectedBudgetIds",
                    data: JSON.stringify({ formId: formId }),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (res) {
                        const selectedBudgets = res.d;
                        if (selectedBudgets.length === 0) {
                            addAllocationDropdown("", "", true);
                        } else {
                            selectedBudgets.forEach(item => {
                                addAllocationDropdown(item.BudgetId, item.Amount);
                            });
                        }
                    }
                });

                $.ajax({
                    type: "POST",
                    url: "Edit.aspx/GetVendorsByForm",
                    data: JSON.stringify({ formId: formId }),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (res) {
                        const vendorNames = res.d;
                        if (vendorNames.length === 0) {
                            addVendorInput("");
                        } else {
                            vendorNames.forEach(name => addVendorInput(name));
                        }
                    }
                });
            } else {
                addAllocationDropdown("", true);
                addVendorInput("");
            }
        }

        function validateAmountLimit(source, args) {
            var role = '<%= Prodata.WebForm.Auth.User().iPMSRoleCode?.ToLower() %>';
            var rawValue = args.Value.replace(/,/g, ''); // remove commas
            var value = parseFloat(rawValue);

            if (isNaN(value)) {
                args.IsValid = false;
                return;
            }

            if (role === "kb") {
                args.IsValid = value <= 10000;
            } else if (role === "mm") {
                args.IsValid = value >= 10000.01;
            } else {
                args.IsValid = false;
            }
        }

        function updateRowBalance($row) {
            var $select = $row.find(".allocation-input");
            var $amountInput = $row.find(".input-number2");
            var $balanceSpan = $row.find("span.balance");

            var selectedOption = $select.find("option:selected");
            var maxAmount = parseFloat(selectedOption.data("max")) || 0;

            var enteredAmount = parseFloat($amountInput.val().replace(/,/g, '')) || 0;

            var balance = maxAmount - enteredAmount;

            // If balance is NaN (e.g., invalid inputs), show 0.00
            if (isNaN(balance)) {
                balance = 0;
            }

            $balanceSpan.text("Balance: RM " + balance.toLocaleString(undefined, {
                minimumFractionDigits: 2,
                maximumFractionDigits: 2
            }));
        }

        // Collect values and store into hidden fields
        function collectData() {
            var vendors = [];
            $(".vendor-input").each(function () {
                var vendorVal = $(this).val().trim();
                if (vendorVal !== "") {
                    vendors.push(vendorVal);
                }
            });

            var allocations = [];
            var totalAllocation = 0;

            $(".allocation-group").each(function () {
                var allocationId = $(this).find(".allocation-input").val();
                var amountStr = $(this).find(".input-number2").val().replace(/,/g, "").trim();
                var amount = parseFloat(amountStr);

                if (allocationId && !isNaN(amount)) {
                    allocations.push({
                        id: allocationId,
                        amount: amount.toLocaleString(undefined, {
                            minimumFractionDigits: 2,
                            maximumFractionDigits: 2
                        })
                    });

                    totalAllocation += amount;
                }
            });

            // Get estimate amount
            var estimateAmountStr = $("#<%= txtAmount.ClientID %>").val().replace(/,/g, "").trim();
            var estimateAmount = parseFloat(estimateAmountStr);

            if (!isNaN(estimateAmount) && Math.abs(totalAllocation - estimateAmount) > 0.01) {
                Swal.fire({
                    icon: 'error',
                    title: 'Allocation Mismatch',
                    html: "The total allocation amount (RM <b>" + totalAllocation.toLocaleString(undefined, {
                        minimumFractionDigits: 2,
                        maximumFractionDigits: 2
                    }) + "</b>) must equal the estimate amount (RM <b>" + estimateAmount.toLocaleString(undefined, {
                        minimumFractionDigits: 2,
                        maximumFractionDigits: 2
                    }) + "</b>).",
                    confirmButtonText: 'OK'
                });
                return false;
            }

            $("#<%= hdnVendorList.ClientID %>").val(vendors.join(","));
            $("#<%= hdnAllocationList.ClientID %>").val(JSON.stringify(allocations));

            return true;
        }
    </script>
</asp:Content>
