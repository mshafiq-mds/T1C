<%@ Page Title="Upload Budget" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Prodata.WebForm.Budget.Upload.Default" %>

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
    </style>
    <asp:HiddenField ID="hdnRecordId" runat="server" />
    <asp:Button ID="btnDeleteRecord" runat="server" OnClick="btnDeleteRecord_Click" CssClass="d-none" />
    <div class="row">
        <div class="col-md-12">
            <div class="card card-outline">
                <div class="card-header card-header-sticky">
                    <div class="row">
                        <div class="col-md-6">
                            <div class="input-group">
                                <div class="custom-file">
                                    <asp:FileUpload ID="fuBudget" runat="server" CssClass="custom-file-input" />
                                    <asp:Label runat="server" AssociatedControlID="fuBudget" CssClass="custom-file-label" Text="Choose file"></asp:Label>
                                </div>
                                <div class="input-group-append">
                                    <asp:LinkButton ID="btnUpload" runat="server" CssClass="btn btn-primary" OnClick="btnUpload_Click" OnClientClick="showLoader();">
                                        <i class="fas fa-upload"></i> Upload
                                    </asp:LinkButton>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-6 text-md-right mt-3 mt-sm-0">
                            <asp:LinkButton ID="btnDownloadTemplate" runat="server" CssClass="btn btn-success" OnClick="btnDownloadTemplate_Click">
                                <i class="fas fa-download"></i> Download Template
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
                                        <asp:GridView ID="gvBudget" runat="server" AutoGenerateColumns="false" CssClass="table table-bordered table-sm" PageSize='<%# FGV.Prodata.App.Setting.RecordsPerPage() %>' AllowPaging="true" OnPageIndexChanging="gvBudget_PageIndexChanging" EmptyDataText="No record." EnableViewState="true">
                                            <Columns>
                                                <asp:TemplateField HeaderText="#">
                                                    <HeaderStyle CssClass="width-30 text-center align-middle" />
                                                    <ItemStyle CssClass="width-30 text-center" />
                                                    <ItemTemplate>
                                                        <%# Container.DataItemIndex + 1 %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="Ref" HeaderText="No. Rujukan" HeaderStyle-CssClass="align-middle" />
                                                <asp:BoundField DataField="BizAreaCode" HeaderText="BA" HeaderStyle-CssClass="align-middle" />
                                                <asp:BoundField DataField="BizAreaName" HeaderText="Projek" HeaderStyle-CssClass="align-middle" />
                                                <asp:BoundField DataField="Details" HeaderText="Butir-butir Kerja" HeaderStyle-CssClass="align-middle" />
                                                <asp:BoundField DataField="Month" HeaderText="Bulan" HeaderStyle-CssClass="align-middle text-center" ItemStyle-CssClass="text-center" />
                                                <asp:BoundField DataField="Vendor" HeaderText="Vendor" HeaderStyle-CssClass="align-middle" />
                                                <asp:BoundField DataField="Wages" HeaderText="Upah (RM)" HeaderStyle-CssClass="align-middle text-center" ItemStyle-CssClass="text-right" />
                                                <asp:BoundField DataField="Purchase" HeaderText="Belian Alat Ganti (RM)" HeaderStyle-CssClass="align-middle text-center" ItemStyle-CssClass="text-right" />
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
    <script>
        $(document).ready(function () {
            bsCustomFileInput.init();
        });

        function showLoader() {
            $('.card').find('.overlay').removeClass('d-none');
        }
    </script>
</asp:Content>
