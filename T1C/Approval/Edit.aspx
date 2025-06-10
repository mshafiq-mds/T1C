<%@ Page Title="T1C Approval" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Prodata.WebForm.T1C.Approval.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="hdnFormId" runat="server" />
    <div class="row">
        <div class="col-lg-12">
            <div class="card card-outline">
                <div class="card-header card-header-sticky">
                    <h3 class="card-title d-none d-sm-inline"><%: Page.Title %></h3>
                    <div class="card-tools">
                        <asp:LinkButton ID="btnBack" runat="server" CssClass="btn btn-default" PostBackUrl="/T1C/Approval/Default" CausesValidation="false">
                        <i class="fas fa-angle-double-left"></i> Back
                        </asp:LinkButton>
                        <asp:LinkButton ID="btnApprove" runat="server" CssClass="btn btn-success" OnClick="btnApprove_Click">
                        <i class="fas fa-check"></i> Approve
                        </asp:LinkButton>
                        <asp:LinkButton ID="btnReject" runat="server" CssClass="btn btn-danger" OnClick="btnReject_Click">
                        <i class="fas fa-times"></i> Reject
                        </asp:LinkButton>
                    </div>
                </div>
                <div class="card-body">
                    <dl class="row">
                        <dt class="col-lg-2 col-sm-3">BA</dt>
                        <dd class="col-lg-10 col-sm-9">
                            <asp:Label ID="lblBA" runat="server" CssClass="text-muted"></asp:Label>
                        </dd>
                        <dt class="col-lg-2 col-sm-3">Nombor Rujukan</dt>
                        <dd class="col-lg-5 col-sm-4">
                            <asp:Label ID="lblRefNo" runat="server" CssClass="text-muted"></asp:Label>
                        </dd>
                        <dt class="col-lg-1 col-sm-2">Tarikh</dt>
                        <dd class="col-lg-4 col-sm-3">
                            <asp:Label ID="lblDate" runat="server" CssClass="text-muted"></asp:Label>
                        </dd>
                        <dt class="col-lg-2 col-sm-3">Butir-butir</dt>
                        <dd class="col-lg-10 col-sm-9">
                            <asp:Label ID="lblDetails" runat="server" CssClass="text-muted"></asp:Label>
                        </dd>
                        <dt class="col-lg-2 col-sm-3">Justifikasi Keperluan</dt>
                        <dd class="col-lg-10 col-sm-9">
                            <asp:Label ID="lblJustificationOfNeed" runat="server" CssClass="text-muted"></asp:Label>
                        </dd>
                        <dt class="col-lg-2 col-sm-3">Anggaran Kerja</dt>
                        <dd class="col-lg-10 col-sm-9">
                            <asp:Label ID="lblAmount" runat="server" CssClass="text-muted"></asp:Label>
                        </dd>
                        <dt class="col-lg-2 col-sm-3">Peruntukan</dt>
                        <dd class="col-lg-10 col-sm-9">
                            <asp:Label ID="lblAllocation" runat="server" CssClass="text-muted"></asp:Label>
                        </dd>
                        <dt class="col-lg-2 col-sm-3">Syor Panggilan Tender</dt>
                        <dd class="col-lg-10 col-sm-9">
                            <asp:Label ID="lblProcurementType" runat="server" CssClass="text-muted"></asp:Label>
                        </dd>
                        <dt class="col-lg-2 col-sm-3">Kontraktor</dt>
                        <dd class="col-lg-10 col-sm-9">
                            <asp:Label ID="lblVendor" runat="server" CssClass="text-muted"></asp:Label>
                        </dd>
                        <dt class="col-lg-2 col-sm-3">Justifikasi Rundingan Terus</dt>
                        <dd class="col-lg-10 col-sm-9">
                            <asp:Label ID="lblJustificationDirectAward" runat="server" CssClass="text-muted"></asp:Label>
                        </dd>
                        <dt class="col-lg-2 col-sm-3">Data Kos</dt>
                        <dd class="col-lg-8 col-sm-7">
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
                                            <th class="text-center">Sebenar YTD</th>
                                            <th class="text-center">Bajet Tahunan</th>
                                            <th class="text-center">Sebenar (RM)</th>
                                            <th class="text-center">Sebenar</th>
                                            <th class="text-center">Bajet</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <th class="text-nowrap align-middle">S & M</th>
                                            <td class="text-center">
                                                <asp:Label ID="lblCurrentYearActualYTD" runat="server" CssClass="form-control text-muted"></asp:Label></td>
                                            <td class="text-center">
                                                <asp:Label ID="lblCurrentYearBudget" runat="server" CssClass="form-control text-muted"></asp:Label></td>
                                            <td class="text-center">
                                                <asp:Label ID="lblPreviousYearActualYTD" runat="server" CssClass="form-control text-muted"></asp:Label></td>
                                            <td class="text-center">
                                                <asp:Label ID="lblPreviousYearActual" runat="server" CssClass="form-control text-muted"></asp:Label></td>
                                            <td class="text-center">
                                                <asp:Label ID="lblPreviousYearBudget" runat="server" CssClass="form-control text-muted"></asp:Label></td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                            <table style="width: 100%;">
                                <tbody>
                                    <tr>
                                        <th class="text-center align-middle pl-1 pr-1">A</th>
                                        <td>
                                            <asp:Label ID="lblA" runat="server" CssClass="form-control text-muted"></asp:Label></td>
                                        <th class="text-center align-middle pl-1 pr-1">C</th>
                                        <td>
                                            <asp:Label ID="lblC" runat="server" CssClass="form-control text-muted"></asp:Label></td>
                                    </tr>
                                    <tr>
                                        <th class="text-center align-middle pl-1 pr-1">B</th>
                                        <td>
                                            <asp:Label ID="lblB" runat="server" CssClass="form-control text-muted"></asp:Label></td>
                                        <th class="text-center align-middle pl-1 pr-1">D</th>
                                        <td>
                                            <asp:Label ID="lblD" runat="server" CssClass="form-control text-muted"></asp:Label></td>
                                    </tr>
                                </tbody>
                            </table>
                        </dd>
                    </dl>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
