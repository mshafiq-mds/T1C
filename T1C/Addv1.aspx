<%@ Page Title="New Budget T1C" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Addv1.aspx.cs" Inherits="Prodata.WebForm.T1C.Addv1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .custom-control.custom-radio {
            margin-bottom: -15px;
        }

        .custom-control-label {
            font-weight: normal !important;
        }

        th[data-bs-toggle="tooltip"] {
            cursor: help; /* help cursor to show it's hoverable */
        } 

        td[data-bs-toggle="tooltip"] {
            cursor: help; /* show help cursor so users know it's hoverable */
        }
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

    <div class="modal fade" id="allocationModal" tabindex="-1" role="dialog">
      <div class="modal-dialog modal-lg" role="document">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">Select Allocations</h5>
            <button type="button" class="close" data-dismiss="modal">
              <span>&times;</span>
            </button>
          </div>
          <div class="modal-body">
              <input type="text" id="allocationSearch" class="form-control mb-2" placeholder="Search allocation...">

              <table class="table table-bordered table-sm">
                <thead>
                  <tr>
                    <th>Select</th>
                    <th>Allocation</th>
                    <th>Balance</th>
                    <th>Amount</th>
                  </tr>
                </thead>
                <tbody id="allocationList">
                  <!-- Filled dynamically -->
                </tbody>
              </table>
            </div>

          <div class="modal-footer">
            <button type="button" id="btnConfirmAllocations" class="btn btn-success">Confirm</button>
          </div>
        </div>
      </div>
    </div>


    <div class="row">
        <div class="col-lg-12">
            <div class="card card-outline">
                <div class="card-header card-header-sticky">
                    <h3 class="card-title d-none d-sm-inline"><%: Page.Title %></h3>
                    <div class="card-tools">
                        <asp:LinkButton ID="btnBack" runat="server" CssClass="btn btn-default" PostBackUrl="~/T1C/Default" CausesValidation="false">
                            <i class="fas fa-angle-double-left"></i> Back
                        </asp:LinkButton>
                        <asp:LinkButton ID="btnSave" runat="server" CssClass="btn btn-primary" OnClick="btnSave_Click" OnClientClick="return beforeSubmit();">
                            <i class="fas fa-save"></i> Save
                        </asp:LinkButton>
                        <asp:LinkButton ID="btnSubmit" runat="server" CssClass="btn btn-success" OnClick="btnSubmit_Click" OnClientClick="return beforeSubmit();">
                            <i class="fas fa-share"></i> Submit
                        </asp:LinkButton>
                    </div>
                </div>
                <div class="card-body">
                    <asp:HiddenField ID="hdnAllocationList" runat="server" />
                    <asp:HiddenField ID="hdnVendorList" runat="server" />
                    <div class="row">
                        <div class="col-lg-12">
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
                                            <asp:TextBox ID="txtDate" runat="server" CssClass="form-control" placeholder="Date" TextMode="Date"></asp:TextBox>
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
                                <asp:Label ID="Label1" runat="server" CssClass="col-lg-2 col-sm-3 col-form-label"  Style="font-weight:bold;" Text="Budget Type"></asp:Label>
                                <div class="col-lg-9 col-sm-8">
                                    <asp:DropDownList 
                                        runat="server" 
                                        ID="ddlBT" 
                                        CssClass="form-control select2"
                                        DataValueField="Code" 
                                        DataTextField="DisplayName"
                                        data-placeholder="Select Type"
                                        AutoPostBack="true"
                                        CausesValidation="false"
                                        OnSelectedIndexChanged="BudgetType_SelectedIndexChanged"  >
                                    </asp:DropDownList> 
                                </div>
                            </div>
                            <div class="form-group row">
                                <asp:Label ID="lblAllocation" runat="server" CssClass="col-lg-2 col-sm-3 col-form-label"  Style="font-weight:bold;" Text="Allocation"></asp:Label>
                                <div class="col-lg-9 col-sm-8">
                                    <asp:Button ID="btnSelectAllocations" runat="server" 
                                        CssClass="btn btn-info" 
                                        Text='Select Allocations' 
                                        OnClientClick="$('#allocationModal').modal('show'); return false;" 
                                        UseSubmitBehavior="false" 
                                        Enabled="false" />

                                    <div id="allocationContainer">
                                        <!-- Selected allocations will appear here -->
                                    </div>
                                </div>
                            </div>

                            <div class="form-group row">
                                <asp:Label ID="lblProcurementType" runat="server" CssClass="col-lg-2 col-sm-3 col-form-label" AssociatedControlID="rblProcurementType" Text="Quotation Method"></asp:Label>
                                <div class="col-lg-10 col-sm-9 mb-3">
                                    <div class="mb-3">
                                        <asp:RadioButtonList ID="rblProcurementType" runat="server" RepeatLayout="Flow">
                                            <asp:ListItem Text="Inclusive Quotation" Value="quotation_inclusive"></asp:ListItem>
                                            <asp:ListItem Text="Selective Quotation" Value="quotation_selective"></asp:ListItem>
                                            <asp:ListItem Text="Direct Negotiation" Value="direct_negotiation"></asp:ListItem>
                                        </asp:RadioButtonList>
                                    </div>
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="rblProcurementType" CssClass="text-danger" Display="Dynamic" ErrorMessage="Please select quotation method" InitialValue=""></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="form-group row">
                                <asp:Label ID="lblVendor" runat="server" CssClass="col-lg-2 col-sm-3 col-form-label" AssociatedControlID="txtVendor" Text="Contractor"></asp:Label>
                                <div class="col-lg-10 col-sm-9">
                                    <div id="vendorContainer">
                                        <div class="input-group mb-2 vendor-group">
                                        
                                            <asp:TextBox ID="txtVendor1" runat="server" CssClass="form-control vendor-input" placeholder="Contractor"></asp:TextBox>
                                        </div>
                                        <div class="input-group mb-2 vendor-group">
                                            <asp:TextBox ID="txtVendor2" runat="server" CssClass="form-control vendor-input" placeholder="Contractor"></asp:TextBox>
                                        </div>
                                        <div class="input-group mb-2 vendor-group">
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
                                                    <th >
                                                         
                                                    </th>
                                                    <th class="text-center"
                                                        data-bs-toggle="tooltip" data-bs-placement="top" data-bs-boundary="window"
                                                        title="Attach copy of Processing Cost Report (ZFPI001A) for the previous month approved by mill management">
                                                        Actual YTD <i class="fas fa-info-circle text-muted"></i>
                                                    </th>
                                                    <th class="text-center"
                                                        data-bs-toggle="tooltip" data-bs-placement="top" data-bs-boundary="window"
                                                        title="Attach copy of 2024 Budget approved by mill management">
                                                        Annual Budget <i class="fas fa-info-circle text-muted"></i>
                                                    </th>
                                                    <th class="text-center"
                                                        data-bs-toggle="tooltip" data-bs-placement="top" data-bs-boundary="window"
                                                        title="Attach copy of Processing Cost Report (ZFPI001A) YTD December 2023 (in RM) approved by mill management">
                                                        Actual (RM) <i class="fas fa-info-circle text-muted"></i>
                                                    </th>
                                                    <th class="text-center"
                                                        data-bs-toggle="tooltip" data-bs-placement="top" data-bs-boundary="window"
                                                        title="Attach copy of Processing Cost Report (ZFPI001A) YTD December 2023 (in RM/MT) approved by mill management">
                                                        Actual <i class="fas fa-info-circle text-muted"></i>
                                                    </th>
                                                    <th class="text-center"
                                                        data-bs-toggle="tooltip" data-bs-placement="top" data-bs-boundary="window"
                                                        title="Attach copy of Processing Cost Report (ZFPI001A) YTD December 2023 (Budget in RM/MT) approved by mill management">
                                                        Budget <i class="fas fa-info-circle text-muted"></i>
                                                    </th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr>
                                                    <td class="text-nowrap align-middle"
                                                        data-bs-toggle="tooltip"
                                                        data-bs-placement="top"
                                                        data-bs-boundary="window"
                                                        title="S&M refers to Maintenance & Repair Costs">
                                                        S & M <i class="fas fa-info-circle text-muted"></i>
                                                    </td>
                                                    <td><asp:TextBox ID="txtCurrentYearActualYTD" runat="server" CssClass="form-control input-number2"></asp:TextBox></td>
                                                    <td><asp:TextBox ID="txtCurrentYearBudget" runat="server" CssClass="form-control input-number2"></asp:TextBox></td>
                                                    <td><asp:TextBox ID="txtPreviousYearActualYTD" runat="server" CssClass="form-control input-number2"></asp:TextBox></td>
                                                    <td><asp:TextBox ID="txtPreviousYearActual" runat="server" CssClass="form-control input-number2"></asp:TextBox></td>
                                                    <td><asp:TextBox ID="txtPreviousYearBudget" runat="server" CssClass="form-control input-number2"></asp:TextBox></td>
                                                </tr>
                                            </tbody>
                                        </table>

                                    </div>
                                    <table style="width: 100%;">
                                        <tbody>
                                            <tr>
                                                <th class="text-center align-middle pl-2 pr-1"
                                                    data-bs-toggle="tooltip" data-bs-placement="top"
                                                    title="Annual Budget Balance for Maintenance & Repair 2024 (RM)">
                                                    A 
                                                    <i class="fas fa-info-circle text-muted ms-1"></i>
                                                </th>
                                                <td>
                                                    <asp:TextBox ID="txtA" runat="server" CssClass="form-control input-number2"></asp:TextBox>
                                                </td>
                                                <th class="text-center align-middle pl-2 pr-1"
                                                    data-bs-toggle="tooltip" data-bs-placement="top"
                                                    title="Attach ZMM30 report copy verified by mill management">
                                                    C 
                                                    <i class="fas fa-info-circle text-muted ms-1"></i>
                                                </th>
                                                <td>
                                                    <asp:TextBox ID="txtC" runat="server" CssClass="form-control input-number2"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <th class="text-center align-middle pl-2 pr-1"
                                                    data-bs-toggle="tooltip" data-bs-placement="top"
                                                    title="Deadline & repair cost of the requested machine from IW38 (include copy verified by mill management)">
                                                    B 
                                                    <i class="fas fa-info-circle text-muted ms-1"></i>
                                                </th>
                                                <td>
                                                    <asp:TextBox ID="txtB" runat="server" CssClass="form-control"></asp:TextBox>
                                                </td>
                                                <th class="text-center align-middle pl-2 pr-1"
                                                    data-bs-toggle="tooltip" data-bs-placement="top"
                                                    title="Attach summary report from Procurehere verified by mill management">
                                                    D 
                                                    <i class="fas fa-info-circle text-muted ms-1"></i>
                                                </th>
                                                <td>
                                                    <asp:TextBox ID="txtD" runat="server" CssClass="form-control input-number2"></asp:TextBox>
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>

                                </div>
                            </div>
                            <div class="form-group row mt-4">
                                <asp:Label ID="lblDocument" runat="server" CssClass="col-lg-2 col-sm-3 col-form-label text-bold" Text="Documents"></asp:Label>
                                <div class="col-lg-10 col-sm-9">
                                    <div class="row">
                                        <div class="col-12">
                                            <div class="form-group row">
                                                <asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Picture"></asp:Label>
                                                <div class="col-lg-9">
                                                    <div class="custom-file">
                                                        <asp:FileUpload ID="fuPicture" runat="server" CssClass="custom-file-input" />
                                                        <asp:Label runat="server" AssociatedControlID="fuPicture" CssClass="custom-file-label" Text="Choose picture"></asp:Label>
                                                    </div>
                                                </div>
                                            </div>
                                            <hr class="mt-0" />
                                            <div class="form-group row">
                                                <asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Machine Repair History"></asp:Label>
                                                <div class="col-lg-9">
                                                    <div class="custom-file">
                                                        <asp:FileUpload ID="fuMachineRepairHistory" runat="server" CssClass="custom-file-input" />
                                                        <asp:Label runat="server" AssociatedControlID="fuMachineRepairHistory" CssClass="custom-file-label" Text="Choose machine repair history (IW38)"></asp:Label>
                                                    </div>
                                                </div>
                                            </div>
                                            <hr class="mt-0" />
                                            <div class="form-group row">
                                                <asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Job Specification"></asp:Label>
                                                <div class="col-lg-9">
                                                    <div class="custom-file">
                                                        <asp:FileUpload ID="fuJobSpecification" runat="server" CssClass="custom-file-input" />
                                                        <asp:Label runat="server" AssociatedControlID="fuJobSpecification" CssClass="custom-file-label" Text="Choose job specification"></asp:Label>
                                                    </div>
                                                </div>
                                            </div>
                                            <hr class="mt-0" />
                                            <div class="form-group row">
                                                <asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Engineer's Estimate Price"></asp:Label>
                                                <div class="col-lg-9">
                                                    <div class="custom-file">
                                                        <asp:FileUpload ID="fuEngineerEstimatePrice" runat="server" CssClass="custom-file-input" />
                                                        <asp:Label runat="server" AssociatedControlID="fuEngineerEstimatePrice" CssClass="custom-file-label" Text="Choose engineer's estimate price"></asp:Label>
                                                    </div>
                                                </div>
                                            </div>
                                            <hr class="mt-0" />
                                            <div class="form-group row">
                                                <asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Dec Cost Report (current year)"></asp:Label>
                                                <div class="col-lg-9">
                                                    <div class="custom-file">
                                                        <asp:FileUpload ID="fuDecCostReportCurrentYear" runat="server" CssClass="custom-file-input" />
                                                        <asp:Label runat="server" AssociatedControlID="fuDecCostReportCurrentYear" CssClass="custom-file-label" Text="Choose dec cost report (current year)"></asp:Label>
                                                    </div>
                                                </div>
                                            </div>
                                            <hr class="mt-0" />
                                            <div class="form-group row">
                                                <asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Dec Cost Report (last year)"></asp:Label>
                                                <div class="col-lg-9">
                                                    <div class="custom-file">
                                                        <asp:FileUpload ID="fuDecCostReportLastYear" runat="server" CssClass="custom-file-input" />
                                                        <asp:Label runat="server" AssociatedControlID="fuDecCostReportLastYear" CssClass="custom-file-label" Text="Choose dec cost report (last year)"></asp:Label>
                                                    </div>
                                                </div>
                                            </div>
                                            <hr class="mt-0" />
                                            <div class="form-group row">
                                                <asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Cost Report (last month)"></asp:Label>
                                                <div class="col-lg-9">
                                                    <div class="custom-file">
                                                        <asp:FileUpload ID="fuCostReportLastMonth" runat="server" CssClass="custom-file-input" />
                                                        <asp:Label runat="server" AssociatedControlID="fuCostReportLastMonth" CssClass="custom-file-label" Text="Choose cost report (last month)"></asp:Label>
                                                    </div>
                                                </div>
                                            </div>
                                            <hr class="mt-0" />
                                            <div class="form-group row">
                                                <asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Drawing / Sketching"></asp:Label>
                                                <div class="col-lg-9">
                                                    <div class="custom-file">
                                                        <asp:FileUpload ID="fuDrawingSketching" runat="server" CssClass="custom-file-input" />
                                                        <asp:Label runat="server" AssociatedControlID="fuDrawingSketching" CssClass="custom-file-label" Text="Choose drawing / sketching"></asp:Label>
                                                    </div>
                                                </div>
                                            </div>
                                            <hr class="mt-0" />
                                            <div class="form-group row">
                                                <asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Quotation (direct negotiation only)"></asp:Label>
                                                <div class="col-lg-9">
                                                    <div class="custom-file">
                                                        <asp:FileUpload ID="fuQuotation" runat="server" CssClass="custom-file-input" />
                                                        <asp:Label runat="server" AssociatedControlID="fuQuotation" CssClass="custom-file-label" Text="Choose quotation (direct negotiation only)"></asp:Label>
                                                    </div>
                                                </div>
                                            </div>
                                            <hr class="mt-0" />
                                            <div class="form-group row">
                                                <asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Damage Investigation Report (Shovel / Prime Mover / Turbine / Alternator / Genset)"></asp:Label>
                                                <div class="col-lg-9">
                                                    <div class="custom-file">
                                                        <asp:FileUpload ID="fuDamageInvestigationReport" runat="server" CssClass="custom-file-input" />
                                                        <asp:Label runat="server" AssociatedControlID="fuDamageInvestigationReport" CssClass="custom-file-label" Text="Choose damage investigation report"></asp:Label>
                                                    </div>
                                                </div>
                                            </div>
                                            <hr class="mt-0" />
                                            <div class="form-group row">
                                                <asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Vendor Registration Record"></asp:Label>
                                                <div class="col-lg-9">
                                                    <div class="custom-file">
                                                        <asp:FileUpload ID="fuVendorRegistrationRecord" runat="server" CssClass="custom-file-input" />
                                                        <asp:Label runat="server" AssociatedControlID="fuVendorRegistrationRecord" CssClass="custom-file-label" Text="Choose vendor registration record"></asp:Label>
                                                    </div>
                                                </div>
                                            </div>
                                            <hr class="mt-0" />
                                            <div class="form-group row">
                                                <asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Approval of Budget Transfer/Add"></asp:Label>
                                                <div class="col-lg-9">
                                                    <div class="custom-file">
                                                        <asp:FileUpload ID="fuBudgetTransferAddApproval" runat="server" CssClass="custom-file-input" />
                                                        <asp:Label runat="server" AssociatedControlID="fuBudgetTransferAddApproval" CssClass="custom-file-label" Text="Choose approval of budget tranfer/add"></asp:Label>
                                                    </div>
                                                </div>
                                            </div>
                                            <hr class="mt-0" />
                                            <div class="form-group row">
                                                <asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Other Supporting Documents (JKKP/JAS/SHO/NDT Report)"></asp:Label>
                                                <div class="col-lg-9">
                                                    <div class="custom-file">
                                                        <asp:FileUpload ID="fuOtherSupportingDocument" runat="server" CssClass="custom-file-input" />
                                                        <asp:Label runat="server" AssociatedControlID="fuOtherSupportingDocument" CssClass="custom-file-label" Text="Choose other supporting document"></asp:Label>
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
    </div>
    <script> 
        // ========================
        // Extra code for Total Allocation
        // ========================

        // Create a container below allocationContainer if not exists
        if ($("#totalAllocationContainer").length === 0) {
            $("#allocationContainer").after(` 
                <div class="form-group row"> 
                    <div class="col-lg-6 col-sm-5">
                        <span id="totalAllocationContainer" class="font-weight-bold">RM 0.00</span>
                    </div>
                </div>
            `);
        }

        // Function to update total allocation
        function updateTotalAllocation() {

            console.log("Function updateTotalAllocation");
            var total = 0;
            $(".allocation-group .input-number2").each(function () {
                var val = $(this).val().replace(/,/g, "").trim();
                if (val) {
                    var num = parseFloat(val);
                    if (!isNaN(num)) {
                        total += num;
                    }
                }
            });

            var formattedTotal = total.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 });

            var estimateVal = $("#<%= txtAmount.ClientID %>").val().replace(/,/g, "").trim();
            var estimate = estimateVal ? parseFloat(estimateVal) : 0;
            console.log("total :", total);
            console.log("formattedTotal :", formattedTotal);
            console.log("estimateVal :", estimateVal);
            console.log("estimate :", estimate);

            $("#totalAllocationContainer").text("Total Allocation: RM " + formattedTotal);

            if (estimate > 0 && total > estimate) {
                $("#totalAllocationContainer").css("color", "red");   // Over allocation
            } else if (estimate > 0 && total === estimate) {
                $("#totalAllocationContainer").css("color", "green"); // Perfect match
            } else {
                $("#totalAllocationContainer").css("color", "blue");  // Under allocation
            }

        }

        // Triggers
        $(document).on("input blur", ".allocation-group .input-number2", updateTotalAllocation);
        $(document).on("input blur", "#<%= txtAmount.ClientID %>", updateTotalAllocation);
        $(document).on("click", ".btnRemoveAllocation", updateTotalAllocation);
        $("#btnAddAllocation").click(function () {
            setTimeout(updateTotalAllocation, 200);
        });

        // Run once on load
        updateTotalAllocation();


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        document.addEventListener("DOMContentLoaded", function () {
            var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
            tooltipTriggerList.map(function (el) {
                return new bootstrap.Tooltip(el, {
                    boundary: 'window',
                    trigger: 'hover',
                    delay: { show: 300, hide: 150 }
                })
            })
        });

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

            //$("#btnAddAllocation").click(function () {
            //    var newInputGroup = $(`
            //        <div class="input-group input-group-sm mb-2 allocation-group">
            //            <select class="form-control form-control-sm select2 allocation-input col-md-6" data-placeholder="Allocation"></select>
            //            <input type="text" class="form-control form-control-sm input-number2" placeholder="Amount" disabled />
            //            <div class="input-group-append">
            //                <button type="button" class="btn btn-danger btnRemoveAllocation">
            //                    <i class="fa fa-minus"></i>
            //                </button>
            //            </div>
            //            <span class="balance col-md-3 mt-1 ml-2">Balance: RM 0.00</span>
            //        </div>
            //    `);

            //    $("#allocationContainer").append(newInputGroup);

            //    var $select = newInputGroup.find("select.select2");
            //    var $amountInput = newInputGroup.find(".input-number2");

            //    // Load allocation options
            //    loadAllocations($select);

            //    // Initialize Select2
            //    $select.select2({
            //        allowClear: true,
            //        theme: "bootstrap4",
            //        dropdownParent: $('body')
            //    }).next('.select2-container').find('.select2-selection').addClass('form-control-sm');

            //    updateRemoveButtons("#allocationContainer", ".btnRemoveAllocation");

            //    // Enable amount input only when allocation is selected
            //    $select.on('change', function () {
            //        if ($(this).val()) {
            //            $amountInput.prop('disabled', false);
            //        } else {
            //            $amountInput.prop('disabled', true).val('');
            //        }
            //    });

            //    // Reinitialize amount input formatting
            //    $amountInput.attr('type', 'text');

            //    $amountInput.on('focus', function () {
            //        $(this).val($(this).val().replace(/,/g, ''));
            //        $(this).attr('type', 'number');
            //    });

            //    $amountInput.on('blur', function () {
            //        $(this).attr('type', 'text');
            //        if ($(this).val()) {
            //            var value = parseFloat($(this).val());
            //            if (!isNaN(value)) {
            //                $(this).val(value.toLocaleString(undefined, {
            //                    minimumFractionDigits: 2,
            //                    maximumFractionDigits: 2
            //                }));
            //            }
            //        }
            //    });
            //});

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

            $(document).on("click", ".btnRemoveAllocation", function () {
                if ($(".allocation-group").length > 1) {
                    $(this).closest(".allocation-group").remove();
                    updateRemoveButtons("#allocationContainer", ".btnRemoveAllocation");
                }
            });

            updateRemoveButtons("#allocationContainer", ".btnRemoveAllocation");

            $("#btnAddVendor").click(function () {
                var newInputGroup = $(`
                    <div class="input-group input-group-sm mb-2 vendor-group">
                        <input type="text" class="form-control form-control-sm vendor-input" placeholder="Contractor">
                        <div class="input-group-append">
                            <button type="button" class="btn btn-danger btnRemoveVendor">
                                <i class="fa fa-minus"></i>
                            </button>
                        </div>
                    </div>
                `);
                $("#vendorContainer").append(newInputGroup);
                updateRemoveButtons("#vendorContainer", ".btnRemoveVendor");
            });

            $(document).on("click", ".btnRemoveVendor", function () {
                if ($(".vendor-group").length > 1) {
                    $(this).closest(".vendor-group").remove();
                    updateRemoveButtons("#vendorContainer", ".btnRemoveVendor");
                }
            });

            updateRemoveButtons("#vendorContainer", ".btnRemoveVendor");

            // Attach to allocation dropdown changes
            $(document).on("change", ".allocation-input", updateAllocationDropdowns);

            // Also call after adding a new allocation row
            //$("#btnAddAllocation").click(function () {
            //    setTimeout(updateAllocationDropdowns, 200);
            //});

            // Call once on page load
            updateAllocationDropdowns();
        });

        function updateRemoveButtons(container, btnClass) {
            var totalInputs = $(container).find(".input-group").length;
            $(btnClass).prop("disabled", totalInputs === 1); // Disable if only one row exists
        }

        function loadAllocations(dropdown) {
            $.ajax({
                type: "POST",
                url: "Add.aspx/GetBudgets", // Replace with your actual endpoint
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    var options = '<option value=""></option>';
                    $.each(response.d, function (index, item) {
                        options += `<option value="${item.Id}" data-max="${item.Amount.replace(/,/g, '')}">${item.DisplayName}</option>`;
                    });
                    dropdown.html(options);
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

        function collectData() {
            console.log("function collectData")

            var vendors = [];
            var allVendorsValid = true;

            $(".vendor-input").each(function () {
                var vendorVal = $(this).val().trim();
                if (vendorVal === "") {
                    allVendorsValid = false;
                    $(this).addClass("is-invalid"); // highlight empty input
                } else {
                    $(this).removeClass("is-invalid");
                    vendors.push(vendorVal);
                }
            });

            if (!allVendorsValid) {
                Swal.fire({
                    icon: 'error',
                    title: 'Missing Contractor',
                    text: 'Please fill in all contractor fields before submitting.',
                    confirmButtonText: 'OK'
                });
                return false;
            }

            var allocations = [];
            var totalAllocation = 0;

            $(".allocation-group").each(function () {
                var allocationId = $(this).find("input[name='AllocationIds']").val();
                //var amountStr = $(this).find(".input-number2").val().replace(/,/g, "").trim();
                var amountStr = $(this).find("input[name='AllocationAmounts']").val().replace(/,/g, "").trim();
                var amount = parseFloat(amountStr);
                console.log("value amount", amount)
                console.log("value allocationId", allocationId)

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
            console.log("value totalAllocation", totalAllocation)

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
        function beforeSubmit() {
            // Run ASP.NET client-side validation first
            if (typeof (Page_ClientValidate) == 'function' && !Page_ClientValidate()) {
                // Validation failed → don’t show preloader, stop postback
                return false;
            }

            // Run custom allocation check
            if (!collectData()) {
                return false;
            }

            // Everything valid → show preloader and continue
            $("#pagePreloader").fadeIn(200);
            return true;
        }

        // Ensure preloader is hidden when page loads/reloads
        $(window).on("load", function () {
            $("#pagePreloader").fadeOut(200);
        });

        // Attach validation to allocation amount fields inside the modal
        $(document).on("input", ".allocationAmount", function () {
            console.log("function allocationAmount");

            var $row = $(this).closest("tr");
            console.log("value $row", $row);

            var balanceCell = $row.find(".allocationBalance");
            if (balanceCell.length === 0) {
                console.warn("No .allocationBalance found in row:", $row);
                return;
            }

            // Get numeric balance safely
            var balanceRaw = balanceCell.data("balance");
            if (balanceRaw === undefined) {
                console.warn("data-balance not set for allocationBalance:", balanceCell);
                return;
            }

            var balance = parseFloat(balanceRaw.toString().replace(/,/g, "")) || 0;
            console.log("value balanceRaw", balanceRaw);
            console.log("value balance", balance);

            var amountRaw = $(this).val().replace(/,/g, "");
            var amount = parseFloat(amountRaw) || 0;
            console.log("value amountRaw", amountRaw);
            console.log("value amount", amount);

            if (amount > balance) {
                Swal.fire({
                    icon: "error",
                    title: "Invalid Amount",
                    text: "Amount cannot exceed balance (RM " + balance.toLocaleString(undefined, {
                        minimumFractionDigits: 2,
                        maximumFractionDigits: 2
                    }) + ")."
                });

                $(this).val(balance.toLocaleString(undefined, {
                    minimumFractionDigits: 2,
                    maximumFractionDigits: 2
                }));
            }
        });



        function loadAllocationsModal(budgets) {
            var tbody = $("#allocationList");
            tbody.empty();

            budgets.forEach(function (b) {
                tbody.append(`
            <tr>
                <td><input type="checkbox" class="chkAllocation" value="${b.Id}" data-display="${b.DisplayName}" data-balance="${b.Amount}" /></td>
                <td>${b.DisplayName}</td>
                <td class="allocationBalance" data-balance="${b.Amount}">RM ${b.Amount}</td>
                <td><input type="number" class="form-control form-control-sm allocationAmount" disabled /></td>
            </tr>
        `);
            });

            // Enable/disable amount input on check
            $(".chkAllocation").change(function () {
                var row = $(this).closest("tr");
                var input = row.find(".allocationAmount");
                if (this.checked) {
                    input.prop("disabled", false);
                } else {
                    input.prop("disabled", true).val("");
                }
            });
        }
        $("#btnConfirmAllocations").click(function () {
            $("#allocationContainer").empty();

            $("#allocationList .chkAllocation:checked").each(function () {
                var id = $(this).val();
                var display = $(this).data("display");

                // Clean commas before parsing
                var balanceRaw = $(this).data("balance").toString().replace(/,/g, "");
                var amountRaw = $(this).closest("tr").find(".allocationAmount").val().replace(/,/g, "");

                var balance = parseFloat(balanceRaw) || 0;
                var amount = parseFloat(amountRaw) || 0;

                // Calculate remaining balance
                var newBalance = balance - amount;

                // 🔍 Debug values
                console.log("Selected Allocation:");
                console.log("ID:", id);
                console.log("Display:", display);
                console.log("Original Balance:", balance);
                console.log("Amount:", amount);
                console.log("New Balance:", newBalance);

                var html = `
            <div class="input-group mb-2 allocation-group">
                <input type="hidden" name="AllocationIds" value="${id}" />
                <span class="form-control flex-grow-2" style="max-width: 100%;">${display}</span>
                <input type="text" class="form-control input-number2" name="AllocationAmounts" style="max-width: 15%;" value="${amount}" readonly />
                <span class="balance col-md-3 mt-1 ml-2" style="max-width: 20%;">
                    Balance: RM ${newBalance.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 })}
                </span>
                <div class="input-group-append">
                    <button type="button" class="btn btn-danger btnRemoveAllocation">
                        <i class="fa fa-minus"></i>
                    </button>
                </div>
            </div>
        `;
                $("#allocationContainer").append(html);
            });

            // Allow removing
            $(".btnRemoveAllocation").click(function () {
                $(this).closest(".allocation-group").remove();
            });

            // Close modal
            $("#allocationModal").modal("hide");
            updateTotalAllocation();

        });
        var budgets = <%= BudgetsJson %>;
        $(document).ready(function () {
            loadAllocationsModal(budgets);
        });
        // Live search filter
        $("#allocationSearch").on("keyup", function () {
            var value = $(this).val().toLowerCase();
            $("#allocationList tr").filter(function () {
                $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1);
            });
        });


    </script>
     <script type="text/javascript">
         $(function () {
             // Attach to dropdown change
             $("#<%= ddlBT.ClientID %>").change(function () {
         // Show preloader
         $("#pagePreloader").fadeIn(200);

         // Trigger ASP.NET postback manually
         __doPostBack('<%= ddlBT.UniqueID %>', '');
         });
     });
     </script>
</asp:Content>
