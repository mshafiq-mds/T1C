<%@ Page Title="T1C Others Budget View" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="View.aspx.cs" Inherits="Prodata.WebForm.T1C.PoolBudget.View" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="hdnFormId" runat="server" />
    <asp:HiddenField ID="hdnRemark" runat="server" />

    <div class="row">
        <div class="col-lg-12">
            <div class="card card-outline">
                <div class="card-header card-header-sticky">
                    <h3 class="card-title d-none d-sm-inline"><%: Page.Title %></h3>
                    <div class="card-tools">
                        <asp:LinkButton ID="btnBack" runat="server" CssClass="btn btn-default" PostBackUrl="~/T1C/PoolBudget/Default" CausesValidation="false">
                            <i class="fas fa-angle-double-left"></i> Back
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

                            <!-- Actual Amount -->
                 <%--           <div class="form-group">
                                <asp:Label runat="server" CssClass="text-bold" Text="Actual Amount"></asp:Label>
                                <div class="text-muted">
                                    <asp:Label ID="lblActualAmount" runat="server"></asp:Label>
                                </div>
                            </div>--%>

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
                    
                        <h4 class="mt-4">Uploaded Document</h4>
                            <asp:Panel runat="server" ID="pnlUploadedDocument" CssClass="form-group" Visible="false">
                                <asp:PlaceHolder ID="phDocumentList" runat="server" />
                            </asp:Panel>
                    <!-- keep your documents, audit trail etc. template unchanged -->
                </div>
            </div>
        </div>
    </div>


</asp:Content>
