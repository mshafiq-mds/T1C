<%@ Page Title="Upload Budget" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Prodata.WebForm.Budget.UploadFGVPISB.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .custom-file {
            height: auto;
        }
        .custom-file-input,
        .custom-file-label,
        .input-group-append .btn {
            padding: 0.25rem 0.5rem;
            font-size: 0.875rem;
            height: auto;
        }
        .custom-file-input {
            height: calc(1.5em + 0.5rem + 2px);
        }
        .custom-file-label {
            height: calc(1.5em + 0.5rem + 2px);
            line-height: 1.5em;
        }
        .input-group-append .btn {
            height: calc(1.5em + 0.5rem + 2px);
            line-height: 1.5em;
        }
        .custom-file-sm .custom-file-label::after {
            padding: .3rem .75rem;
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
    <script>
        // Show preloader when clicking GridView pager links or filter buttons
        $(document).on('click', '#<%= gvBudget.ClientID %> a, #<%= btnUpload.ClientID %>', function () {
            $("#pagePreloader").fadeIn(200);
        });

        // Hide preloader once async postback completes
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            $("#pagePreloader").fadeOut(200);
        });
    </script>

<!-- Page-specific Preloader -->
<div id="pagePreloader" class="page-preloader flex-column justify-content-center align-items-center" style="display:none;">
    <img src="<%= ResolveUrl("~/Images/Felda_Global_Ventures_Logo.png") %>" 
         alt="Loading..." height="200" width="200" />
    <p class="mt-3 text-white">Processing...</p>
</div>

    <asp:HiddenField ID="hdnRecordId" runat="server" />
    <asp:Button ID="btnDeleteRecord" runat="server" OnClick="btnDeleteRecord_Click" CssClass="d-none" />
    <div class="row">
        <div class="col-md-12">
            <div class="card card-outline">
                <div class="card-header card-header-sticky">
                    <div class="row align-items-center">
                        <div class="col-md-8">
                            <div class="input-group">
                                <div class="custom-file">
                                    <asp:FileUpload ID="fuBudget" runat="server" CssClass="custom-file-input" />
                                    <asp:Label runat="server" AssociatedControlID="fuBudget" CssClass="custom-file-label" Text="Choose file"></asp:Label>
                                </div>
                                <div class="input-group-append">
                                    <asp:LinkButton ID="btnUpload" runat="server" CssClass="btn btn-primary" OnClick="btnUpload_Click">
                                        <i class="fas fa-upload"></i> Upload
                                    </asp:LinkButton>
                                </div>
                            </div>
                            <div class="mt-2">
                                <asp:DropDownList runat="server" ID="ddlBT" CssClass="form-control select2" 
                                                  DataValueField="Code" DataTextField="DisplayName" data-placeholder="Type" />
                                <%--<asp:RequiredFieldValidator ID="rfhddlBT" runat="server" ControlToValidate="ddlBT" CssClass="text-danger" ErrorMessage="Required" Display="Dynamic" />--%>
                            </div>
                        </div>
                        <div class="col-md-4 text-md-right mt-3 mt-md-0">
                            <asp:LinkButton ID="btnDownloadTemplate" runat="server" CssClass="btn btn-success" OnClick="btnDownloadTemplateMain_Click">
                                <i class="fas fa-download"></i> Download Template Main
                            </asp:LinkButton>
                            
                            <asp:LinkButton ID="LinkButton1" runat="server" CssClass="btn btn-success" OnClick="btnDownloadTemplate_Click">
                                <i class="fas fa-download"></i> Download Template Other Budget
                            </asp:LinkButton>
                        </div>
                    </div>
                </div>

                <div class="card-body">
                    <div class="row">
                        <div class="col-12">
                            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                <ContentTemplate>
                                    <div class="table-responsive">
                                        <asp:GridView ID="gvBudget" runat="server" AutoGenerateColumns="false" CssClass="table table-bordered table-sm"
                                                      PageSize='<%# FGV.Prodata.App.Setting.RecordsPerPage() %>' AllowPaging="true"
                                                      OnPageIndexChanging="gvBudget_PageIndexChanging" EmptyDataText="No record." EnableViewState="true">
                                            <Columns>
                                                <asp:TemplateField HeaderText="#">
                                                    <HeaderStyle CssClass="width-30 text-center align-middle" />
                                                    <ItemStyle CssClass="width-30 text-center" />
                                                    <ItemTemplate>
                                                        <%# Container.DataItemIndex + 1 %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="BizAreaCode" HeaderText="BA" HeaderStyle-CssClass="align-middle" />
                                                <asp:BoundField DataField="BizAreaName" HeaderText="Projek" HeaderStyle-CssClass="align-middle" />
                                                <asp:BoundField DataField="Details" HeaderText="Butir-butir Kerja" HeaderStyle-CssClass="align-middle" />
                                                
                                                <asp:BoundField DataField="Amount" HeaderText="Jumlah (RM)" HeaderStyle-CssClass="align-middle text-center" ItemStyle-CssClass="text-right" />
                                            </Columns>
                                            <PagerSettings Mode="NumericFirstLast" PageButtonCount="5" Position="TopAndBottom" />
                                            <PagerStyle CssClass="pagination-ys" />
                                        </asp:GridView>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </div>
                </div>

                <div class="overlay d-none">
                    <i class="fas fa-3x fa-spinner fa-spin fa-pulse fa-fw"></i> &nbsp;<h1>Processing. Please wait...</h1>
                </div>
            </div>
        </div>
    </div>

<script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
<script>
    $(document).ready(function () {
        bsCustomFileInput.init();

        $('#<%= btnUpload.ClientID %>').click(function (e) {
            var ddlValue = $('#<%= ddlBT.ClientID %>').val();
            var file = $('#<%= fuBudget.ClientID %>').val();

            if (!ddlValue) {
                e.preventDefault(); // stop form submission
                Swal.fire({
                    icon: 'warning',
                    title: 'Type Required',
                    text: 'Please select a type before uploading.',
                });
                return false;
            }

            if (!file) {
                e.preventDefault(); // stop form submission
                Swal.fire({
                    icon: 'warning',
                    title: 'File Required',
                    text: 'Please select a file before uploading.',
                });
                return false;
            }

            // only show loader if both validations pass
            showLoader();
        });
    });

    function showLoader() {
        $('.card').find('.overlay').removeClass('d-none');
    }
</script>



</asp:Content>
