<%@ Page Title="View Budget" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="View.aspx.cs" Inherits="Prodata.WebForm.Budget.UploadFGVPISB.View" %>

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
        $(document).on('click', '#<%= gvBudget.ClientID %> a, #<%= btnApplyFilter.ClientID %>, #<%= btnClearFilter.ClientID %>', function () {
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
    <div class="row">
        <div class="col-md-12">
            <div class="card card-outline">
                <div class="card-header card-header-sticky">
                    <div class="row">
                        <div class="col-md-4 mb-3">
                            <label class="form-label font-weight-bold">Budget Type</label>
                            <asp:DropDownList runat="server" ID="ddlBT" CssClass="form-control select2"
                                DataValueField="Code" DataTextField="DisplayName"
                                AutoPostBack="false" AppendDataBoundItems="true">
                                <asp:ListItem Text="-- Select Budget Type --" Value="" />
                            </asp:DropDownList>
                        </div>

                        <div class="col-md-4 mb-3">
                            <label class="form-label font-weight-bold">Year</label>
                            <asp:DropDownList runat="server" ID="ddlYear" CssClass="form-control select2"
                                AutoPostBack="false" AppendDataBoundItems="true">
                                <asp:ListItem Text="-- Select Year --" Value="" />
                            </asp:DropDownList>
                        </div>

                        <div class="col-md-4 mb-3">
                            <label class="form-label font-weight-bold">Business Area</label>
                            <asp:DropDownList runat="server" ID="ddlBA" CssClass="form-control select2"
                                DataValueField="Code" DataTextField="DisplayName"
                                AutoPostBack="false" AppendDataBoundItems="true" >
                                <asp:ListItem Text="-- Select Biz Area --" Value="" />
                            </asp:DropDownList>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-md-12 text-left">
                            <asp:Button ID="btnApplyFilter" runat="server" CssClass="btn btn-primary mr-2"
                                Text="Apply Filter" OnClick="ddlFilter_SelectedIndexChanged" />
                            <asp:Button ID="btnClearFilter" runat="server" CssClass="btn btn-outline-secondary"
                                Text="Clear Filter" OnClick="btnClearFilter_Click" />
                        </div>
                    </div>
                </div>


                <div class="card-body">
                    <div class="row">
                        <div class="col-12">
                            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                <ContentTemplate>
                                    <div class="table-responsive">
                                        <asp:GridView ID="gvBudget" AllowSorting="true" OnSorting="gvBudget_Sorting" runat="server" AutoGenerateColumns="false" CssClass="table table-bordered table-sm" PageSize='100' AllowPaging="true" OnPageIndexChanging="gvBudget_PageIndexChanging" EmptyDataText="No record." EnableViewState="true">
                                            <HeaderStyle BackColor="White" ForeColor="Black" Font-Bold="true" Font-Underline="true" />
                                            <Columns>
                                                <asp:TemplateField HeaderText="#">
                                                    <HeaderStyle CssClass="width-30 text-center align-middle" />
                                                    <ItemStyle CssClass="width-30 text-center" />
                                                    <ItemTemplate>
                                                        <%# Container.DataItemIndex + 1 %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="Ref" SortExpression="Ref" HeaderText="No. Rujukan" HeaderStyle-CssClass="align-middle" />
                                                <asp:BoundField DataField="BizAreaCode" SortExpression="BizAreaCode" HeaderText="BA" HeaderStyle-CssClass="align-middle" />
                                                <asp:BoundField DataField="BizAreaName" SortExpression="BizAreaName" HeaderText="Projek" HeaderStyle-CssClass="align-middle" />
                                                <asp:BoundField DataField="Details" SortExpression="Details" HeaderText="Butir-butir Kerja" HeaderStyle-CssClass="align-middle" />
                                                <asp:BoundField DataField="Month" SortExpression="Month" HeaderText="Bulan" HeaderStyle-CssClass="align-middle text-center" ItemStyle-CssClass="text-center" />
                                                <asp:BoundField DataField="Vendor" SortExpression="Vendor" HeaderText="Vendor" HeaderStyle-CssClass="align-middle" />
                                                <asp:BoundField DataField="Wages" SortExpression="Wages" HeaderText="Upah (RM)" HeaderStyle-CssClass="align-middle text-center" ItemStyle-CssClass="text-right" />
                                                <asp:BoundField DataField="Purchase" SortExpression="Purchase" HeaderText="Belian Alat Ganti (RM)" HeaderStyle-CssClass="align-middle text-center" ItemStyle-CssClass="text-right" />
                                                <asp:BoundField DataField="Amount" SortExpression="Amount" HeaderText="Budget (RM)" HeaderStyle-CssClass="align-middle text-center" ItemStyle-CssClass="text-right" />
                                                <asp:BoundField DataField="Balance" SortExpression="Balance" HeaderText="Balance (RM)" HeaderStyle-CssClass="align-middle text-center" ItemStyle-CssClass="text-right" />
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

</asp:Content>
