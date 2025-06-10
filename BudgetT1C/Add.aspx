<%@ Page Title="New Budget T1C" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Add.aspx.cs" Inherits="Prodata.WebForm.BudgetT1C.Add" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .custom-control.custom-radio {
            margin-bottom: -15px;
        }

        .custom-control-label {
            font-weight: normal !important;
        }
    </style>
    <div class="row">
        <div class="col-md-12">
            <div class="card card-outline">
                <div class="card-header card-header-sticky">
                    <h3 class="card-title d-none d-sm-inline"><%: Page.Title %></h3>
                    <div class="card-tools">
                        <asp:LinkButton ID="btnBack" runat="server" CssClass="btn btn-default" PostBackUrl="/BudgetT1C/Default" CausesValidation="false">
                            <i class="fas fa-angle-double-left"></i> Back
                        </asp:LinkButton>
                        <asp:LinkButton ID="btnSave" runat="server" CssClass="btn btn-primary" OnClick="btnSave_Click">
                            <i class="fas fa-save"></i> Save
                        </asp:LinkButton>
                        <asp:LinkButton ID="btnSubmit" runat="server" CssClass="btn btn-success" OnClick="btnSubmit_Click">
                            <i class="fas fa-share"></i> Submit
                        </asp:LinkButton>
                    </div>
                </div>
                <div class="card-body">
                    <div class="row">
                        <div class="col-md-6">
                            <div class="form-group">
                                <asp:Label ID="lblKilang" runat="server" AssociatedControlID="txtKilang" Text="Kilang"></asp:Label>
                                <asp:TextBox ID="txtKilang" runat="server" CssClass="form-control" placeholder="Kilang"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-6">
                            <div class="form-group">
                                <asp:Label ID="lblRefNo" runat="server" AssociatedControlID="txtRefNo" Text="Nombor Rujukan"></asp:Label>
                                <asp:TextBox ID="txtRefNo" runat="server" CssClass="form-control" placeholder="Nombor Rujukan"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-md-3">
                            <div class="form-group">
                                <asp:Label ID="lblDate" runat="server" AssociatedControlID="txtDate" Text="Tarikh"></asp:Label>
                                <asp:TextBox ID="txtDate" runat="server" CssClass="form-control" placeholder="Tarikh" TextMode="Date"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-md-12">
                            <div class="form-group">
                                <asp:Label ID="lblDetails" runat="server" AssociatedControlID="txtDetails" Text="Butir-Butir"></asp:Label>
                                <asp:TextBox ID="txtDetails" runat="server" CssClass="form-control" placeholder="Butir-butir..." TextMode="MultiLine" Rows="3"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-md-12">
                            <div class="form-group">
                                <asp:Label ID="lblJustificationOfNeed" runat="server" AssociatedControlID="txtJustificationOfNeed" Text="Justifikasi Keperluan"></asp:Label>
                                <asp:TextBox ID="txtJustificationOfNeed" runat="server" CssClass="form-control" placeholder="Justifikasi keperluan..." TextMode="MultiLine" Rows="3"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-12">
                            <div class="form-group">
                                <asp:Label ID="lblAllocation" runat="server" AssociatedControlID="gvAllocation" Text="Peruntukan"></asp:Label>
                                <asp:GridView ID="gvAllocation" runat="server"></asp:GridView>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-4">
                            <div class="form-group">
                                <asp:Label ID="lblProcurementType" runat="server" AssociatedControlID="rblProcurementType" Text="Syor Panggilan Tender"></asp:Label>
                                <asp:RadioButtonList ID="rblProcurementType" runat="server" RepeatLayout="Flow" AutoPostBack="false">
                                    <asp:ListItem Text="Kaedah Sebutharga" Value="quotation"></asp:ListItem>
                                    <asp:ListItem Text="Tender Selektif" Value="selective_tender"></asp:ListItem>
                                    <asp:ListItem Text="Tender Terbuka" Value="open_tender"></asp:ListItem>
                                    <asp:ListItem Text="Rundingan Terus" Value="direct_award"></asp:ListItem>
                                </asp:RadioButtonList>
                            </div>
                        </div>
                        <div class="col-md-8">
                            <div class="form-group">
                                <asp:Label ID="lblDataKos" runat="server" AssociatedControlID="tblDataKos" Text="Data Kos"></asp:Label>
                                <table id="tblDataKos" runat="server" class="table table-bordered table-sm">
                                    <thead>
                                        <tr>
                                            <th></th>
                                            <th colspan="2" class="text-center"><%= DateTime.Now.Year %> (RM/MT)</th>
                                            <th colspan="3" class="text-center">YTD <%= DateTime.Now.Year - 1 %> (RM/MT)</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <th class="text-nowrap align-middle">S & M</th>
                                            <td>
                                                <asp:TextBox ID="txtCurrentYearActualYTD" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtCurrentYearBudget" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtPreviousYearActualYTD" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtPreviousYearActual" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtPreviousYearBudget" runat="server" CssClass="form-control"></asp:TextBox></td>
                                        </tr>
                                    </tbody>
                                </table>
                                <table style="width: 100%;">
                                    <tbody>
                                        <tr>
                                            <th class="text-center align-middle pl-2 pr-1">A</th>
                                            <td>
                                                <asp:TextBox ID="txtA" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <th class="text-center align-middle pl-2 pr-1">C</th>
                                            <td>
                                                <asp:TextBox ID="txtC" runat="server" CssClass="form-control"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <th class="text-center align-middle pl-2 pr-1">B</th>
                                            <td>
                                                <asp:TextBox ID="txtB" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            <th class="text-center align-middle pl-2 pr-1">D</th>
                                            <td>
                                                <asp:TextBox ID="txtD" runat="server" CssClass="form-control"></asp:TextBox></td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-12">
                            <div class="form-group">
                                <asp:Label ID="lblVendor" runat="server" AssociatedControlID="gvVendor" Text="Kontraktor"></asp:Label>
                                <asp:GridView ID="gvVendor" runat="server"></asp:GridView>
                            </div>
                        </div>
                    </div>
                    <div class="row" id="divJustificationDirectAward" style="display: none;">
                        <div class="col-md-12">
                            <div class="form-group">
                                <asp:Label ID="lblJustificationDirectAward" runat="server" AssociatedControlID="txtJustificationDirectAward" Text="Justifikasi Rundingan Terus"></asp:Label>
                                <asp:TextBox ID="txtJustificationDirectAward" runat="server" CssClass="form-control" placeholder="Justifikasi rundingan terus..." TextMode="MultiLine" Rows="3"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <script>
        $(document).ready(function () {
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
                if ($(this).val() === "direct_award") {
                    $extraDiv.show(); // Show div
                } else {
                    $extraDiv.hide(); // Hide div
                }
            });
        });
    </script>
</asp:Content>
