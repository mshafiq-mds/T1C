<%@ Page Title="T1C Others Budget Details Approval" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Prodata.WebForm.T1C.PoolBudget.Approval.Edit" %>

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

        /* --- PRINT STYLES --- */
        @media print {
            /* Hide non-printable elements */
            .no-print, .card-tools, .btn, .page-preloader, .modal {
                display: none !important;
            }
            
            /* Reset visibility for the print container */
            body * {
                visibility: hidden;
            }

            #printArea, #printArea * {
                visibility: visible;
            }

            #printArea {
                position: absolute;
                left: 0;
                top: 0;
                width: 100%;
                background-color: white;
            }

            /* Show Print Header */
            .print-header {
                display: block !important;
                text-align: center;
                margin-bottom: 20px;
                border-bottom: 2px solid #000;
                padding-bottom: 10px;
            }

            /* Clean up Card Styling */
            .card {
                box-shadow: none !important;
                border: none !important;
            }

            .card-header-sticky {
                position: static !important;
                border-bottom: 1px solid #000 !important;
            }
        }
    </style>
    
    <!-- Page-specific Preloader -->
    <div id="pagePreloader" class="page-preloader" style="display:none;">
        <img src="<%= ResolveUrl("~/Images/Felda_Global_Ventures_Logo.png") %>" 
             alt="Loading..." height="200" width="200" />
        <p class="mt-3 text-white">Processing...</p>
    </div>

    <asp:HiddenField ID="hdnFormId" runat="server" />
    <asp:HiddenField ID="hdnRemark" runat="server" />

    <asp:Button ID="btnApproveConfirm" runat="server" CssClass="d-none" OnClick="btnApprove_Click" /> 

    <div id="printArea">
        
        <!-- Print Header (Hidden on screen, visible on print) -->
        <div class="print-header d-none">
            <img src="<%= ResolveUrl("~/Images/Felda_Global_Ventures_Logo.png") %>" alt="FGV Logo" style="height: 80px;" />
            <h3 class="mt-2">T1C Others Budget Details Approval</h3>
            <p>Generated on: <%= DateTime.Now.ToString("dd/MM/yyyy HH:mm") %></p>
        </div>

        <div class="row">
            <div class="col-lg-12">
                <div class="card card-outline">
                    <div class="card-header card-header-sticky">
                        <h3 class="card-title d-none d-sm-inline"><%: Page.Title %></h3>
                        <div class="card-tools"> 

                            <asp:LinkButton ID="btnBack" runat="server" CssClass="btn btn-default" PostBackUrl="~/T1C/PoolBudget/Approval/Default" CausesValidation="false">
                                <i class="fas fa-angle-double-left"></i> Back
                            </asp:LinkButton>
                            <button type="button" class="btn btn-default mr-1" onclick="window.print();">
                                <i class="fas fa-print"></i> Print
                            </button>
                            <asp:LinkButton ID="btnApprove" runat="server" CssClass="btn btn-success" OnClientClick="showApproveModal(); return false;">
                                <i class="fas fa-check"></i> Approve
                            </asp:LinkButton>
                        </div>
                    </div>

                    <div class="card-body">
                        <div class="row">
                            <div class="col-md-7 border-right">
                                <!-- Biz Area -->
                                <div class="form-group">
                                    <asp:Label runat="server" CssClass="text-bold" Text="BA"></asp:Label>
                                    <div class="text-muted">
                                        <asp:Label ID="lblBA" runat="server"></asp:Label>
                                    </div>
                                </div>

                                <!-- Details -->
                                <div class="form-group">
                                    <asp:Label runat="server" CssClass="text-bold" Text="Details"></asp:Label>
                                    <div class="text-muted">
                                        <asp:Label ID="lblDetails" runat="server"></asp:Label>
                                    </div>
                                </div>

                                <!-- Justification of Need -->
                                <div class="form-group">
                                    <asp:Label runat="server" CssClass="text-bold" Text="Justification of Need"></asp:Label>
                                    <div class="text-muted">
                                        <asp:Label ID="lblJustificationOfNeed" runat="server"></asp:Label>
                                    </div>
                                </div>

                                <!-- Remarks -->
                                <div class="form-group">
                                    <asp:Label runat="server" CssClass="text-bold" Text="Remarks"></asp:Label>
                                    <div class="text-muted">
                                        <asp:Label ID="lblRemarks" runat="server"></asp:Label>
                                    </div>
                                </div>
                            
                                <!-- Uploaded Documents -->
                                <div class="form-group">
                                    <h4 class="mt-4">Uploaded Document</h4>
                                    <asp:Panel runat="server" ID="pnlUploadedDocument" CssClass="form-group" Visible="false">
                                        <asp:PlaceHolder ID="phDocumentList" runat="server" />
                                    </asp:Panel>
                                </div>

                            </div>

                            <div class="col-md-3">
                                <!-- Reference No -->
                                <div class="form-group">
                                    <asp:Label runat="server" CssClass="text-bold" Text="Reference No."></asp:Label>
                                    <div class="text-muted">
                                        <asp:Label ID="lblRefNo" runat="server"></asp:Label>
                                    </div>
                                </div>

                                <!-- Date -->
                                <div class="form-group">
                                    <asp:Label runat="server" CssClass="text-bold" Text="Date"></asp:Label>
                                    <div class="text-muted">
                                        <asp:Label ID="lblDate" runat="server"></asp:Label>
                                    </div>
                                </div>

                                <!-- Estimate Amount -->
                                <div class="form-group">
                                    <asp:Label runat="server" CssClass="text-bold" Text="Amount"></asp:Label>
                                    <div class="text-success text-bold">
                                        <asp:Label ID="lblAmount" runat="server"></asp:Label>
                                    </div>
                                </div>

                                <!-- Procurement Type -->
                                <div class="form-group">
                                    <asp:Label runat="server" CssClass="text-bold" Text="Budget Type"></asp:Label>
                                    <div class="text-muted">
                                        <asp:Label ID="lblProcurementType" runat="server"></asp:Label>
                                    </div>
                                </div>

                                <!-- Status -->
                                <div class="form-group">
                                    <asp:Label runat="server" CssClass="text-bold" Text="Status"></asp:Label>
                                    <div>
                                        <asp:Label ID="lblStatus" runat="server" CssClass="badge badge-pill"></asp:Label>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- Remark Modal -->
    <div class="modal fade no-print" id="remarkModal" tabindex="-1" role="dialog" aria-labelledby="remarkModalLabel" aria-hidden="true">
      <div class="modal-dialog modal-dialog-centered" role="document">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title" id="remarkModalLabel">Enter Remark</h5>
            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
              <span aria-hidden="true">&times;</span>
            </button>
          </div>
          <div class="modal-body">
            <textarea id="txtRemark" class="form-control" rows="3" placeholder="Enter remark here..."></textarea>
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-secondary btn-sm" data-dismiss="modal">Cancel</button>
            <button type="button" id="modalSubmit" class="btn btn-success btn-sm" onclick="return beforeSubmit()">Submit</button>
          </div>
        </div>
      </div>
    </div>

    <script src="~/Scripts/jquery-3.6.0.min.js"></script>
    <script src="~/Scripts/bootstrap.bundle.min.js"></script>

    <script>
        $(document).ready(function () {
            $('#remarkModal').on('hidden.bs.modal', function () {
                // Clear the textarea when the modal is fully hidden
                $('#txtRemark').val('');
            });
        });

        function showApproveModal() {
            $('#remarkModal').modal('show');
            $('#remarkModal').find('.modal-title').text('Enter remark to approve application');
            $('#remarkModal').find('#modalSubmit').removeClass().addClass('btn btn-success btn-sm');
            $('#remarkModal').find('#modalSubmit').removeAttr('onclick').attr('onclick', 'return beforeSubmit();');
        } 
        function submitApprove() {
            var remark = document.getElementById("txtRemark").value;
            document.getElementById("<%= hdnRemark.ClientID %>").value = remark;

            // Trigger hidden button for server postback
            document.getElementById("<%= btnApproveConfirm.ClientID %>").click();
        }

        function beforeSubmit() {
            // Show the page-specific preloader
            $("#pagePreloader").fadeIn(200);

            // Run your existing collectData() if defined
            submitApprove();

            // Continue with ASP.NET postback
            return false;
        }

    </script>

</asp:Content>