<%@ Page Title="Edit Budget T1C" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Prodata.WebForm.T1C.BudgetEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    
    <!-- Page-specific Preloader -->
    <div id="pagePreloader" class="page-preloader" style="display:none;">
        <img src="<%= ResolveUrl("~/Images/Felda_Global_Ventures_Logo.png") %>" 
             alt="Loading..." height="200" width="200" />
        <p class="mt-3 text-white">Processing...</p>
    </div>
    <style>
        .custom-control.custom-radio {
            margin-bottom: -15px;
        }
        .custom-control-label {
            font-weight: normal !important;
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
        .balance {
            font-size: 0.85rem;
            color: #6c757d;
        }
    </style>

    <asp:HiddenField ID="hdnFormId" runat="server" />
    <%-- JSON Data for Allocations Modal --%>
    <asp:HiddenField ID="hdnBudgetsJson" runat="server" />

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

                            <!-- BA Selection -->
                            <div class="form-group row">
                                <asp:Label ID="lblBA" runat="server" CssClass="col-lg-2 col-sm-3 col-form-label" AssociatedControlID="ddlBA" Text="BA"></asp:Label>
                                <div class="col-lg-6 col-sm-5">
                                    <asp:Label ID="lblBAText" runat="server" CssClass="form-control text-muted" Visible="false"></asp:Label>
                                    <asp:DropDownList ID="ddlBA" runat="server" CssClass="form-control select2" data-placeholder="BA"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvBA" runat="server" ControlToValidate="ddlBA" CssClass="text-danger" Display="Dynamic" ErrorMessage="Please select BA" InitialValue=""></asp:RequiredFieldValidator>
                                </div>
                            </div> 

                            <!-- Requester Name -->
                            <div class="form-group row">
                                <asp:Label ID="Label1" runat="server" CssClass="col-lg-2 col-sm-3 col-form-label" AssociatedControlID="txtReqName" Text="Requester Name"></asp:Label>
                                <div class="col-lg-6 col-sm-5">                                    
                                    <asp:Label ID="Label3" runat="server" CssClass="form-control text-muted" Visible="false"></asp:Label>
                                    <asp:TextBox ID="txtReqName" runat="server" CssClass="form-control text-uppercase" placeholder="Full Name"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvRequestorName" runat="server" ControlToValidate="txtReqName" CssClass="text-danger" Display="Dynamic" ErrorMessage="Please Fill Name" InitialValue=""></asp:RequiredFieldValidator>
                                </div>
                            </div>

                            <!-- Ref No & Date -->
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

                            <!-- Details & Justification -->
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

                            <!-- Amount -->
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
                                </div>
                            </div>

                            <!-- Budget Type -->
                            <div class="form-group row">
                                <asp:Label ID="lblBudget" runat="server" CssClass="col-lg-2 col-sm-3 col-form-label"  Style="font-weight:bold;" Text="Budget Type"></asp:Label>
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
                                        OnSelectedIndexChanged="BudgetType_SelectedIndexChanged" >
                                    </asp:DropDownList> 
                                </div>
                            </div>
                            
                            <!-- Allocation Section -->
                            <div class="form-group row">
                                <asp:Label ID="lblAllocation" runat="server" CssClass="col-lg-2 col-sm-3 col-form-label"  Style="font-weight:bold;" Text="Allocation"></asp:Label>
                                <div class="col-lg-9 col-sm-8">
                                    <div class="row">
                                        <div class="col-md-3 mb-2">
                                            <asp:DropDownList ID="ddlYear" runat="server" CssClass="form-control select2" AutoPostBack="true" OnSelectedIndexChanged="ddlYear_SelectedIndexChanged"></asp:DropDownList>
                                        </div>
                                        <div class="col-md-9 mb-2">
                                            <asp:Button ID="btnSelectAllocations" runat="server" 
                                                CssClass="btn btn-info" 
                                                Text='Select Allocations' 
                                                OnClientClick="return false;" 
                                                UseSubmitBehavior="false" 
                                                Enabled="false" />
                                        </div>
                                    </div>

                                    <div id="allocationContainer">
                                        <!-- Selected allocations will appear here via JS -->
                                    </div>
                                    
                                    <div class="mt-2">
                                        <span id="totalAllocationContainer" class="font-weight-bold">RM 0.00</span>
                                    </div>
                                </div>
                            </div>

                            <!-- Quotation Method -->
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

                            <!-- Contractor Section (Dynamic) -->
                            <div class="form-group row vendor-section">
                                <asp:Label ID="lblVendor" runat="server" CssClass="col-lg-2 col-sm-3 col-form-label font-weight-bold fw-bold" Text="Contractor"></asp:Label>
                                <div class="col-lg-10 col-sm-9">
                                    <div id="vendorContainer">
                                        <!-- JS populates this container based on radio selection -->
                                    </div>
                                    <button type="button" class="btn btn-info mt-1" id="btnAddVendor">
                                        <i class="fa fa-plus"></i> Add Contractor
                                    </button>
                                </div>
                            </div>

                            <!-- Justification Direct Negotiation -->
                            <div class="form-group row" id="divJustificationDirectAward" style="display: none;">
                                <asp:Label ID="lblJustificationDirectAward" runat="server" CssClass="col-lg-2 col-sm-3" AssociatedControlID="txtJustificationDirectAward" Text="Justification (Direct Negotiation)"></asp:Label>
                                <div class="col-lg-10 col-sm-9">
                                    <asp:TextBox ID="txtJustificationDirectAward" runat="server" CssClass="form-control" placeholder="Justification direct negotiation..." TextMode="MultiLine" Rows="3"></asp:TextBox>
                                </div>
                            </div>
                            
                            <!-- Cost Data Tables -->
                            
                            <asp:PlaceHolder ID="phDataCost" runat="server">
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
                                                    <th ></th>
                                                    <th class="text-center">Actual YTD</th>
                                                    <th class="text-center">Annual Budget</th>
                                                    <th class="text-center">Actual (RM)</th>
                                                    <th class="text-center">Actual</th>
                                                    <th class="text-center">Budget</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr>
                                                    <td class="text-nowrap align-middle" ><%--S & M--%></td>
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
                                                <th class="text-center align-middle pl-2 pr-1" data-bs-toggle="tooltip" title="Annual Budget Balance for Maintenance & Repair">A</th>
                                                <td><asp:TextBox ID="txtA" runat="server" CssClass="form-control input-number2"></asp:TextBox></td>
                                                <th class="text-center align-middle pl-2 pr-1" data-bs-toggle="tooltip" title="Work Order & Purchase Order Costs Not Yet GR">C</th>
                                                <td><asp:TextBox ID="txtC" runat="server" CssClass="form-control input-number2"></asp:TextBox></td>
                                            </tr>
                                            <tr>
                                                <th class="text-center align-middle pl-2 pr-1" data-bs-toggle="tooltip" title="Final Repair Date & Cost">B</th>
                                                <td><asp:TextBox ID="txtB" runat="server" CssClass="form-control"></asp:TextBox></td>
                                                <th class="text-center align-middle pl-2 pr-1" data-bs-toggle="tooltip" title="RFS/RFQ In-Progress Costs">D</th>
                                                <td><asp:TextBox ID="txtD" runat="server" CssClass="form-control input-number2"></asp:TextBox></td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                            </asp:PlaceHolder>
                            <!-- Documents Section -->
                            <div class="form-group row mt-4">
                                <asp:Label ID="lblDocument" runat="server" CssClass="col-lg-2 col-sm-3 col-form-label text-bold" Text="Documents"></asp:Label>
                                <div class="col-lg-10 col-sm-9">
                                    <div class="row">
                                        <div class="col-12">
                                            
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
                                                            <asp:Label runat="server" AssociatedControlID="fuPicture" CssClass="custom-file-label" Text="Choose picture"></asp:Label>
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
                                                            <asp:Label runat="server" AssociatedControlID="fuMachineRepairHistory" CssClass="custom-file-label" Text="Choose file"></asp:Label>
                                                        </div>
                                                    </asp:Panel>
                                                </div>
                                            </div>
                                            <hr class="mt-0" />

                                             <!-- Job Specification -->
                                             <div class="form-group row"><asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Job Specification"></asp:Label><div class="col-lg-9"><asp:Panel ID="pnlJobSpecificationView" runat="server" Visible="false"><asp:HyperLink ID="lnkJobSpecification" runat="server" Target="_blank" Text="View File" CssClass="btn btn-link"></asp:HyperLink><asp:Button ID="btnDeleteJobSpecification" runat="server" Text="Delete" CssClass="btn btn-danger btn-sm" OnClick="btnDeleteJobSpecification_Click" CausesValidation="false" /></asp:Panel><asp:Panel ID="pnlJobSpecificationUpload" runat="server"><div class="custom-file"><asp:FileUpload ID="fuJobSpecification" runat="server" CssClass="custom-file-input" /><asp:Label runat="server" AssociatedControlID="fuJobSpecification" CssClass="custom-file-label" Text="Choose file"></asp:Label></div></asp:Panel></div></div> <hr class="mt-0" />
                                             
                                             <!-- Engineer Estimate Price -->
                                             <div class="form-group row"><asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Engineer Estimate Price"></asp:Label><div class="col-lg-9"><asp:Panel ID="pnlEngineerEstimatePriceView" runat="server" Visible="false"><asp:HyperLink ID="lnkEngineerEstimatePrice" runat="server" Target="_blank" Text="View File" CssClass="btn btn-link"></asp:HyperLink><asp:Button ID="btnDeleteEngineerEstimatePrice" runat="server" Text="Delete" CssClass="btn btn-danger btn-sm" OnClick="btnDeleteEngineerEstimatePrice_Click" CausesValidation="false" /></asp:Panel><asp:Panel ID="pnlEngineerEstimatePriceUpload" runat="server"><div class="custom-file"><asp:FileUpload ID="fuEngineerEstimatePrice" runat="server" CssClass="custom-file-input" /><asp:Label runat="server" AssociatedControlID="fuEngineerEstimatePrice" CssClass="custom-file-label" Text="Choose file"></asp:Label></div></asp:Panel></div></div> <hr class="mt-0" />
                                             
                                             <!-- Cost Report Current Year -->
                                             <div class="form-group row"><asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Dec Cost Report (Current Year)"></asp:Label><div class="col-lg-9"><asp:Panel ID="pnlDecCostReportCurrentYearView" runat="server" Visible="false"><asp:HyperLink ID="lnkDecCostReportCurrentYear" runat="server" Target="_blank" Text="View File" CssClass="btn btn-link"></asp:HyperLink><asp:Button ID="btnDeleteDecCostReportCurrentYear" runat="server" Text="Delete" CssClass="btn btn-danger btn-sm" OnClick="btnDeleteDecCostReportCurrentYear_Click" CausesValidation="false" /></asp:Panel><asp:Panel ID="pnlDecCostReportCurrentYearUpload" runat="server"><div class="custom-file"><asp:FileUpload ID="fuDecCostReportCurrentYear" runat="server" CssClass="custom-file-input" /><asp:Label runat="server" AssociatedControlID="fuDecCostReportCurrentYear" CssClass="custom-file-label" Text="Choose file"></asp:Label></div></asp:Panel></div></div> <hr class="mt-0" />

                                             <!-- Cost Report Last Year -->
                                             <div class="form-group row"><asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Dec Cost Report (Last Year)"></asp:Label><div class="col-lg-9"><asp:Panel ID="pnlDecCostReportLastYearView" runat="server" Visible="false"><asp:HyperLink ID="lnkDecCostReportLastYear" runat="server" Target="_blank" Text="View File" CssClass="btn btn-link"></asp:HyperLink><asp:Button ID="btnDeleteDecCostReportLastYear" runat="server" Text="Delete" CssClass="btn btn-danger btn-sm" OnClick="btnDeleteDecCostReportLastYear_Click" CausesValidation="false" /></asp:Panel><asp:Panel ID="pnlDecCostReportLastYearUpload" runat="server"><div class="custom-file"><asp:FileUpload ID="fuDecCostReportLastYear" runat="server" CssClass="custom-file-input" /><asp:Label runat="server" AssociatedControlID="fuDecCostReportLastYear" CssClass="custom-file-label" Text="Choose file"></asp:Label></div></asp:Panel></div></div> <hr class="mt-0" />

                                             <!-- Cost Report Last Month -->
                                             <div class="form-group row"><asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Cost Report (Last Month)"></asp:Label><div class="col-lg-9"><asp:Panel ID="pnlCostReportLastMonthView" runat="server" Visible="false"><asp:HyperLink ID="lnkCostReportLastMonth" runat="server" Target="_blank" Text="View File" CssClass="btn btn-link"></asp:HyperLink><asp:Button ID="btnDeleteCostReportLastMonth" runat="server" Text="Delete" CssClass="btn btn-danger btn-sm" OnClick="btnDeleteCostReportLastMonth_Click" CausesValidation="false" /></asp:Panel><asp:Panel ID="pnlCostReportLastMonthUpload" runat="server"><div class="custom-file"><asp:FileUpload ID="fuCostReportLastMonth" runat="server" CssClass="custom-file-input" /><asp:Label runat="server" AssociatedControlID="fuCostReportLastMonth" CssClass="custom-file-label" Text="Choose file"></asp:Label></div></asp:Panel></div></div> <hr class="mt-0" />

                                             <!-- Drawing Sketching -->
                                             <div class="form-group row"><asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Drawing / Sketching"></asp:Label><div class="col-lg-9"><asp:Panel ID="pnlDrawingSketchingView" runat="server" Visible="false"><asp:HyperLink ID="lnkDrawingSketching" runat="server" Target="_blank" Text="View File" CssClass="btn btn-link"></asp:HyperLink><asp:Button ID="btnDeleteDrawingSketching" runat="server" Text="Delete" CssClass="btn btn-danger btn-sm" OnClick="btnDeleteDrawingSketching_Click" CausesValidation="false" /></asp:Panel><asp:Panel ID="pnlDrawingSketchingUpload" runat="server"><div class="custom-file"><asp:FileUpload ID="fuDrawingSketching" runat="server" CssClass="custom-file-input" /><asp:Label runat="server" AssociatedControlID="fuDrawingSketching" CssClass="custom-file-label" Text="Choose file"></asp:Label></div></asp:Panel></div></div> <hr class="mt-0" />

                                             <!-- Quotation -->
                                             <div class="form-group row"><asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Quotation"></asp:Label><div class="col-lg-9"><asp:Panel ID="pnlQuotationView" runat="server" Visible="false"><asp:HyperLink ID="lnkQuotation" runat="server" Target="_blank" Text="View File" CssClass="btn btn-link"></asp:HyperLink><asp:Button ID="btnDeleteQuotation" runat="server" Text="Delete" CssClass="btn btn-danger btn-sm" OnClick="btnDeleteQuotation_Click" CausesValidation="false" /></asp:Panel><asp:Panel ID="pnlQuotationUpload" runat="server"><div class="custom-file"><asp:FileUpload ID="fuQuotation" runat="server" CssClass="custom-file-input" /><asp:Label runat="server" AssociatedControlID="fuQuotation" CssClass="custom-file-label" Text="Choose file"></asp:Label></div></asp:Panel></div></div> <hr class="mt-0" />

                                             <!-- Damage Investigation -->
                                             <div class="form-group row"><asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Damage Investigation Report"></asp:Label><div class="col-lg-9"><asp:Panel ID="pnlDamageInvestigationReportView" runat="server" Visible="false"><asp:HyperLink ID="lnkDamageInvestigationReport" runat="server" Target="_blank" Text="View File" CssClass="btn btn-link"></asp:HyperLink><asp:Button ID="btnDeleteDamageInvestigationReport" runat="server" Text="Delete" CssClass="btn btn-danger btn-sm" OnClick="btnDeleteDamageInvestigationReport_Click" CausesValidation="false" /></asp:Panel><asp:Panel ID="pnlDamageInvestigationReportUpload" runat="server"><div class="custom-file"><asp:FileUpload ID="fuDamageInvestigationReport" runat="server" CssClass="custom-file-input" /><asp:Label runat="server" AssociatedControlID="fuDamageInvestigationReport" CssClass="custom-file-label" Text="Choose file"></asp:Label></div></asp:Panel></div></div> <hr class="mt-0" />

                                             <!-- Vendor Registration -->
                                             <div class="form-group row"><asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Vendor Registration Record"></asp:Label><div class="col-lg-9"><asp:Panel ID="pnlVendorRegistrationRecordView" runat="server" Visible="false"><asp:HyperLink ID="lnkVendorRegistrationRecord" runat="server" Target="_blank" Text="View File" CssClass="btn btn-link"></asp:HyperLink><asp:Button ID="btnDeleteVendorRegistrationRecord" runat="server" Text="Delete" CssClass="btn btn-danger btn-sm" OnClick="btnDeleteVendorRegistrationRecord_Click" CausesValidation="false" /></asp:Panel><asp:Panel ID="pnlVendorRegistrationRecordUpload" runat="server"><div class="custom-file"><asp:FileUpload ID="fuVendorRegistrationRecord" runat="server" CssClass="custom-file-input" /><asp:Label runat="server" AssociatedControlID="fuVendorRegistrationRecord" CssClass="custom-file-label" Text="Choose file"></asp:Label></div></asp:Panel></div></div> <hr class="mt-0" />

                                             <!-- Budget Transfer Approval -->
                                             <div class="form-group row"><asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Budget Transfer/Add Approval"></asp:Label><div class="col-lg-9"><asp:Panel ID="pnlBudgetTransferAddApprovalView" runat="server" Visible="false"><asp:HyperLink ID="lnkBudgetTransferAddApproval" runat="server" Target="_blank" Text="View File" CssClass="btn btn-link"></asp:HyperLink><asp:Button ID="btnDeleteBudgetTransferAddApproval" runat="server" Text="Delete" CssClass="btn btn-danger btn-sm" OnClick="btnDeleteBudgetTransferAddApproval_Click" CausesValidation="false" /></asp:Panel><asp:Panel ID="pnlBudgetTransferAddApprovalUpload" runat="server"><div class="custom-file"><asp:FileUpload ID="fuBudgetTransferAddApproval" runat="server" CssClass="custom-file-input" /><asp:Label runat="server" AssociatedControlID="fuBudgetTransferAddApproval" CssClass="custom-file-label" Text="Choose file"></asp:Label></div></asp:Panel></div></div> <hr class="mt-0" />

                                             <!-- Other Supporting -->
                                             <div class="form-group row"><asp:Label runat="server" CssClass="col-lg-3 col-form-label" Text="Other Supporting Document"></asp:Label><div class="col-lg-9"><asp:Panel ID="pnlOtherSupportingDocumentView" runat="server" Visible="false"><asp:HyperLink ID="lnkOtherSupportingDocument" runat="server" Target="_blank" Text="View File" CssClass="btn btn-link"></asp:HyperLink><asp:Button ID="btnDeleteOtherSupportingDocument" runat="server" Text="Delete" CssClass="btn btn-danger btn-sm" OnClick="btnDeleteOtherSupportingDocument_Click" CausesValidation="false" /></asp:Panel><asp:Panel ID="pnlOtherSupportingDocumentUpload" runat="server"><div class="custom-file"><asp:FileUpload ID="fuOtherSupportingDocument" runat="server" CssClass="custom-file-input" /><asp:Label runat="server" AssociatedControlID="fuOtherSupportingDocument" CssClass="custom-file-label" Text="Choose file"></asp:Label></div></asp:Panel></div></div>
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
    
    <!-- Modal for Allocations -->
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
              <div class="form-row mb-2">
                <div class="col">
                  <input type="text" id="allocationSearch" class="form-control" placeholder="Search allocation...">
                </div>
                <div class="col-auto">
                  <button type="button" id="btnConfirmAllocationsTop" class="btn btn-success">
                    Confirm
                  </button>
                </div>
              </div>

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
                </tbody>
              </table>

            </div>
          <div class="modal-footer">
            <button type="button" id="btnConfirmAllocationsBottom" class="btn btn-success">Confirm</button>
          </div>
        </div>
      </div>
    </div>

    <script src="https://cdn.jsdelivr.net/npm/bs-custom-file-input/dist/bs-custom-file-input.min.js"></script>

    <script> 
        var availableBudgets = [];

        function updateTotalAllocation() {
            var total = 0;
            $(".allocation-group .input-number2").each(function () {
                var val = $(this).val().replace(/,/g, "").trim();
                if (val) {
                    var num = parseFloat(val);
                    if (!isNaN(num)) total += num;
                }
            });

            var formattedTotal = total.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 });
            var estimateVal = $("#<%= txtAmount.ClientID %>").val().replace(/,/g, "").trim();
            var estimate = estimateVal ? parseFloat(estimateVal) : 0;
            $("#totalAllocationContainer").text("Total Allocation: RM " + formattedTotal);

            if (estimate > 0 && total > estimate) {
                $("#totalAllocationContainer").css("color", "red");
            } else if (estimate > 0 && total === estimate) {
                $("#totalAllocationContainer").css("color", "green");
            } else {
                $("#totalAllocationContainer").css("color", "blue");
            }
        }

        $(document).ready(function () {
            bsCustomFileInput.init();

            // 1. Initialize Budgets
            var jsonString = $("#<%= hdnBudgetsJson.ClientID %>").val();
            if (jsonString) {
                try {
                    availableBudgets = JSON.parse(jsonString);
                } catch (e) { console.error("JSON Parse error", e); availableBudgets = []; }
            }

            if (availableBudgets.length > 0) {
                $("#<%= btnSelectAllocations.ClientID %>").prop("disabled", false);
            }

            buildModalTable();
            loadInitialData();
            initContractorLogic();
            
            $("#<%= btnSelectAllocations.ClientID %>").click(function(e) {
                e.preventDefault();
                syncModalWithContainer();
                $("#allocationModal").modal("show");
            });

            $("#allocationSearch").on("keyup", function () {
                var value = $(this).val().toLowerCase();
                $("#allocationList tr").filter(function () {
                    $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1);
                });
            });

            $("#btnConfirmAllocationsTop, #btnConfirmAllocationsBottom").click(function () {
                confirmAllocations();
            });

            // Global delegation
            $(document).on("input blur", ".allocation-group .input-number2", updateTotalAllocation);
            $(document).on("input blur", "#<%= txtAmount.ClientID %>", updateTotalAllocation);
            $(document).on("click", ".btnRemoveAllocation", function () {
                $(this).closest(".allocation-group").remove();
                updateTotalAllocation();
            });
        });

        // Allocation Logic
        function buildModalTable() {
            var tbody = $("#allocationList");
            tbody.empty();

            if (availableBudgets && availableBudgets.length > 0) {
                availableBudgets.forEach(function (b) {
                    tbody.append(`
                        <tr>
                            <td><input type="checkbox" class="chkAllocation" value="${b.Id}" /></td>
                            <td>${b.Display}</td>
                            <td class="allocationBalance" data-balance="${b.Balance}">RM ${b.Amount}</td>
                            <td><input type="number" class="form-control form-control-sm allocationAmount" disabled /></td>
                        </tr>
                    `);
                });
            } else {
                 tbody.append(`<tr><td colspan="4" class="text-center">No budgets available.</td></tr>`);
            }

            $(document).on("change", ".chkAllocation", function () {
                var row = $(this).closest("tr");
                var input = row.find(".allocationAmount");
                if (this.checked) {
                    input.prop("disabled", false);
                } else {
                    input.prop("disabled", true).val("");
                }
            });
            
            $(document).on("input", ".allocationAmount", function () {
                var row = $(this).closest("tr");
                var balance = parseFloat(row.find(".allocationBalance").data("balance")) || 0;
                var val = parseFloat($(this).val()) || 0;
                if(val > balance) $(this).addClass("is-invalid");
                else $(this).removeClass("is-invalid");
            });
        }

        function syncModalWithContainer() {
            $(".chkAllocation").prop("checked", false);
            $(".allocationAmount").val("").prop("disabled", true).removeClass("is-invalid");

            $("#allocationContainer .allocation-group").each(function () {
                var id = $(this).find("input[name='AllocationIds']").val();
                var amt = $(this).find("input[name='AllocationAmounts']").val().replace(/,/g, "");

                var $chk = $(`.chkAllocation[value="${id}"]`);
                if ($chk.length) {
                    $chk.prop("checked", true);
                    var $row = $chk.closest("tr");
                    $row.find(".allocationAmount").val(amt).prop("disabled", false);
                }
            });
        }

        function confirmAllocations() {
            $("#allocationContainer").empty();
            $("#allocationList .chkAllocation:checked").each(function () {
                var id = $(this).val();
                var row = $(this).closest("tr");
                var display = row.find("td:eq(1)").text(); // Name from cell
                var balanceRaw = row.find(".allocationBalance").data("balance");
                var amountRaw = row.find(".allocationAmount").val();
                
                var balance = parseFloat(balanceRaw) || 0;
                var amount = parseFloat(amountRaw) || 0;
                var newBalance = balance - amount;

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
            $("#allocationModal").modal("hide");
            updateTotalAllocation();
        }

        // Contractor Logic
        function initContractorLogic() {
            let $radioList = $("input[name*='rblProcurementType']");
            let $btnAddVendor = $("#btnAddVendor");
            let $vendorContainer = $("#vendorContainer");

            $radioList.on("change", function () {
                $vendorContainer.empty();
                updateContractorFields($(this).val());
            });

            $btnAddVendor.click(function (e) {
                e.preventDefault();
                var count = $vendorContainer.find(".vendor-group").length + 1;
                addVendorInput("", count);
            });

            $(document).on("click", ".btnRemoveVendor", function () {
                if ($vendorContainer.find(".vendor-group").length > 1) {
                    $(this).closest(".vendor-group").remove();
                    updateRemoveButtons();
                }
            });
        }

        function updateContractorFields(type) {
            let $vendorSection = $(".vendor-section");
            let $btnAddVendor = $("#btnAddVendor");
            let $extraDiv = $("#divJustificationDirectAward");
            let $vendorContainer = $("#vendorContainer");

            if (type === "quotation_inclusive") {
                $vendorSection.hide();
                $extraDiv.hide();
            } else if (type === "direct_negotiation") {
                $vendorSection.show();
                $btnAddVendor.hide();
                $extraDiv.show();
                if ($vendorContainer.children().length === 0) addVendorInput("");
            } else { // Selective
                $vendorSection.show();
                $btnAddVendor.show();
                $extraDiv.hide();
                if ($vendorContainer.children().length === 0) {
                     for (let i = 1; i <= 3; i++) addVendorInput("", i);
                }
            }
        }

        function addVendorInput(value, index) {
            var placeholder = index ? "Contractor " + index : "Contractor";
            var html = `
                <div class="input-group input-group-sm mb-2 vendor-group">
                    <input type="text" class="form-control form-control-sm vendor-input" placeholder="${placeholder}" value="${value}">
                    <div class="input-group-append">
                        <button type="button" class="btn btn-danger btnRemoveVendor">
                            <i class="fa fa-minus"></i>
                        </button>
                    </div>
                </div>
            `;
            $("#vendorContainer").append(html);
            updateRemoveButtons();
        }

        function updateRemoveButtons() {
             var count = $("#vendorContainer .vendor-group").length;
             $(".btnRemoveVendor").prop("disabled", count <= 1);
        }

        function loadInitialData() {
            const formId = $("#<%= hdnFormId.ClientID %>").val();
            if (formId) {
                // Allocations
                $.ajax({
                    type: "POST",
                    url: "Edit.aspx/GetSelectedBudgetIds",
                    data: JSON.stringify({ formId: formId }),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (res) {
                        const savedBudgets = res.d;
                        if(savedBudgets.length > 0) {
                            savedBudgets.forEach(s => {
                                var bInfo = availableBudgets.find(b => b.Id === s.BudgetId);
                                var display = bInfo ? bInfo.Display : "Unknown";
                                var balance = bInfo ? bInfo.Balance : 0; 
                                var amountVal = parseFloat(s.Amount.replace(/,/g, "")) || 0;
                                var newBalance = balance - amountVal; 

                                if (display != "Unknown") { 
                                var html = `
                                    <div class="input-group mb-2 allocation-group">
                                        <input type="hidden" name="AllocationIds" value="${s.BudgetId}" />
                                        <span class="form-control flex-grow-2" style="max-width: 100%;">${display}</span>
                                        <input type="text" class="form-control input-number2" name="AllocationAmounts" style="max-width: 15%;" value="${s.Amount}" readonly />
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
                                }
                            });
                            updateTotalAllocation();
                        }
                    }
                });

                // Vendors
                $.ajax({
                    type: "POST",
                    url: "Edit.aspx/GetVendorsByForm",
                    data: JSON.stringify({ formId: formId }),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (res) {
                        const vendorNames = res.d;
                        $("#vendorContainer").empty();

                        if (vendorNames.length > 0) {
                            vendorNames.forEach(name => addVendorInput(name));
                        }
                        
                        var currentType = $("input[name*='rblProcurementType']:checked").val();
                        let $vendorSection = $(".vendor-section");
                        let $btnAddVendor = $("#btnAddVendor");
                        let $extraDiv = $("#divJustificationDirectAward");
                        
                        if (currentType === "quotation_inclusive") {
                             $vendorSection.hide();
                        } else if (currentType === "direct_negotiation") {
                             $vendorSection.show();
                             $btnAddVendor.hide();
                             $extraDiv.show();
                             if(vendorNames.length === 0) addVendorInput("");
                        } else {
                             $vendorSection.show();
                             $btnAddVendor.show();
                             $extraDiv.hide();
                             if(vendorNames.length === 0) for(let i=1;i<=3;i++) addVendorInput("", i);
                        }
                        updateRemoveButtons();
                    }
                });
            } else {
                var type = $("input[name*='rblProcurementType']:checked").val();
                if(type) updateContractorFields(type);
            }
        }
        
        function collectData() {
            var vendors = [];
            $(".vendor-input").each(function () {
                var v = $(this).val().trim();
                if (v) vendors.push(v);
            });
            
            var allocations = [];
            $(".allocation-group").each(function () {
                var id = $(this).find("input[name='AllocationIds']").val();
                var amt = $(this).find("input[name='AllocationAmounts']").val().replace(/,/g, "");
                if (id && amt) allocations.push({ id: id, amount: amt });
            });
            
            // Validate Estimate vs Allocation
             var estimateAmountStr = $("#<%= txtAmount.ClientID %>").val().replace(/,/g, "").trim();
             var estimateAmount = parseFloat(estimateAmountStr) || 0;
             var totalAllocation = 0;
             allocations.forEach(a => totalAllocation += parseFloat(a.amount));
             
             if (Math.abs(totalAllocation - estimateAmount) > 0.01) {
                Swal.fire({
                    icon: 'error',
                    title: 'Allocation Mismatch',
                    html: "Total allocation (RM " + totalAllocation.toLocaleString() + ") must equal estimate (RM " + estimateAmount.toLocaleString() + ").",
                    confirmButtonText: 'OK'
                });
                return false;
            }

            $("#<%= hdnVendorList.ClientID %>").val(vendors.join(","));
            $("#<%= hdnAllocationList.ClientID %>").val(JSON.stringify(allocations));

            return true;
        }

        function beforeSubmit() {
            $("#pagePreloader").fadeIn(200);
            if (!collectData()) {
                $("#pagePreloader").fadeOut(200);
                return false;
            }
            return true;
        }
    </script>

    <script type="text/javascript"> 
        // Run once on full page load
        $(document).ready(function () { 
            $("#<%= ddlBT.ClientID %>, #<%= ddlYear.ClientID %>").on("change", function () {
            // Show the preloader
                $("#pagePreloader").fadeIn(200);
            });
        });

        if (typeof Sys !== 'undefined' && Sys.WebForms && Sys.WebForms.PageRequestManager) {
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                $("#<%= ddlBT.ClientID %>, #<%= ddlYear.ClientID %>").on("change", function () {
                    $("#pagePreloader").fadeIn(200);
                });
            });
        }
    </script>
</asp:Content>