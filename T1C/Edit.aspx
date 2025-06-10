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
                        <asp:LinkButton ID="btnBack" runat="server" CssClass="btn btn-default" PostBackUrl="/T1C/Default" CausesValidation="false">
                            <i class="fas fa-angle-double-left"></i> Back
                        </asp:LinkButton>
                        <asp:LinkButton ID="btnSave" runat="server" CssClass="btn btn-primary" OnClick="btnSave_Click" OnClientClick="collectData();">
                            <i class="fas fa-save"></i> Save
                        </asp:LinkButton>
                        <asp:LinkButton ID="btnSubmit" runat="server" CssClass="btn btn-success" OnClick="btnSubmit_Click" OnClientClick="collectData();">
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
                                    <asp:RequiredFieldValidator ID="rfvBA" runat="server" ControlToValidate="ddlBA" CssClass="text-danger" Display="Dynamic" ErrorMessage="Sila pilih BA" InitialValue=""></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-lg-8 col-sm-7">
                                    <div class="form-group row">
                                        <asp:Label ID="lblRefNo" runat="server" CssClass="col-lg-3 col-sm-5 col-form-label" AssociatedControlID="txtRefNo" Text="Nombor Rujukan"></asp:Label>
                                        <div class="col-lg-9 col-sm-7">
                                            <asp:TextBox ID="txtRefNo" runat="server" CssClass="form-control" placeholder="Nombor Rujukan"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-lg-4 col-sm-5">
                                    <div class="form-group row">
                                        <asp:Label ID="lblDate" runat="server" CssClass="col-lg-3 col-sm-4 col-form-label" AssociatedControlID="txtDate" Text="Tarikh"></asp:Label>
                                        <div class="col-lg-9 col-sm-8">
                                            <asp:TextBox ID="txtDate" runat="server" CssClass="form-control" placeholder="Tarikh" TextMode="Date"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group row">
                                <asp:Label ID="lblDetails" runat="server" CssClass="col-lg-2 col-sm-3 col-form-label" AssociatedControlID="txtDetails" Text="Butir-butir"></asp:Label>
                                <div class="col-lg-10 col-sm-9">
                                    <asp:TextBox ID="txtDetails" runat="server" CssClass="form-control" placeholder="Butir-butir..." TextMode="MultiLine" Rows="3"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group row">
                                <asp:Label ID="lblJustificationOfNeed" runat="server" CssClass="col-lg-2 col-sm-3 col-form-label" AssociatedControlID="txtJustificationOfNeed" Text="Justifikasi Keperluan"></asp:Label>
                                <div class="col-lg-10 col-sm-9">
                                    <asp:TextBox ID="txtJustificationOfNeed" runat="server" CssClass="form-control" placeholder="Justifikasi keperluan..." TextMode="MultiLine" Rows="3"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group row">
                                <asp:Label ID="lblAmount" runat="server" CssClass="col-lg-2 col-sm-3 col-form-label" AssociatedControlID="txtAmount" Text="Anggaran Kerja"></asp:Label>
                                <div class="col-lg-6 col-sm-5">
                                    <div class="input-group">
                                        <div class="input-group-prepend">
                                            <span class="input-group-text">RM</span>
                                        </div>
                                        <asp:TextBox ID="txtAmount" runat="server" CssClass="form-control input-number2" placeholder="Anggaran Kerja"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group row">
                                <asp:Label ID="lblAllocation" runat="server" CssClass="col-lg-2 col-sm-3 col-form-label text-bold" Text="Peruntukan"></asp:Label>
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
                                        <i class="fa fa-plus"></i>Tambah Peruntukan
                                    </button>
                                </div>
                            </div>
                            <div class="form-group row">
                                <asp:Label ID="lblProcurementType" runat="server" CssClass="col-lg-2 col-sm-3 col-form-label" AssociatedControlID="rblProcurementType" Text="Syor Panggilan Tender"></asp:Label>
                                <div class="col-lg-10 col-sm-9 mb-3">
                                    <asp:RadioButtonList ID="rblProcurementType" runat="server" RepeatLayout="Flow">
                                        <asp:ListItem Text="Kaedah Sebutharga" Value="quotation"></asp:ListItem>
                                        <asp:ListItem Text="Tender Selektif" Value="selective_tender"></asp:ListItem>
                                        <asp:ListItem Text="Tender Terbuka" Value="open_tender"></asp:ListItem>
                                        <asp:ListItem Text="Rundingan Terus" Value="direct_award"></asp:ListItem>
                                    </asp:RadioButtonList>
                                </div>
                            </div>
                            <div class="form-group row">
                                <asp:Label ID="lblVendor" runat="server" CssClass="col-lg-2 col-sm-3 col-form-label" AssociatedControlID="txtVendor" Text="Kontraktor"></asp:Label>
                                <div class="col-lg-10 col-sm-9">
                                    <div id="vendorContainer">
                                        <div class="input-group mb-2 vendor-group d-none">
                                            <asp:TextBox ID="txtVendor" runat="server" CssClass="form-control vendor-input" placeholder="Kontraktor"></asp:TextBox>
                                            <div class="input-group-append">
                                                <button type="button" class="btn btn-danger btnRemoveVendor" disabled>
                                                    <i class="fa fa-minus"></i>
                                                </button>
                                            </div>
                                        </div>
                                    </div>

                                    <!-- Plus button below textboxes -->
                                    <button type="button" class="btn btn-info mt-1" id="btnAddVendor">
                                        <i class="fa fa-plus"></i>Tambah Vendor
                                    </button>
                                </div>
                            </div>
                            <div class="form-group row" id="divJustificationDirectAward" style="display: none;">
                                <asp:Label ID="lblJustificationDirectAward" runat="server" CssClass="col-lg-2 col-sm-3" AssociatedControlID="txtJustificationDirectAward" Text="Justifikasi Rundingan Terus"></asp:Label>
                                <div class="col-lg-10 col-sm-9">
                                    <asp:TextBox ID="txtJustificationDirectAward" runat="server" CssClass="form-control" placeholder="Justifikasi rundingan terus..." TextMode="MultiLine" Rows="3"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group row">
                                <asp:Label ID="lblDataKos" runat="server" CssClass="col-lg-2 col-sm-3 col-form-label" AssociatedControlID="tblDataKos" Text="Data Kos"></asp:Label>
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

            // Function to toggle div based on selection
            function toggleJustification() {
                let selectedVal = $radioList.find("input[type='radio']:checked").val();
                if (selectedVal === "direct_award") {
                    $extraDiv.show();
                } else {
                    $extraDiv.hide();
                }
            }

            // Handle selection change
            $radioList.on("change", "input[type='radio']", toggleJustification);

            // Trigger on load
            toggleJustification();

            function updateRemoveButtons(container, btnClass) {
                var totalInputs = $(container).find(".input-group").length;
                $(btnClass).prop("disabled", totalInputs === 1);
            }

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
                var selectedOption = $(this).find("option:selected");
                var max = selectedOption.data("max");
                var $amountInput = $(this).closest(".allocation-group").find(".input-number2");

                if (max) {
                    $amountInput.attr("max", max);
                } else {
                    $amountInput.removeAttr("max");
                }
            });

            function loadAllocations(dropdown) {
                $.ajax({
                    type: "POST",
                    url: "Edit.aspx/GetBudgets",
                    contentType: "application/json; charset=utf-8",
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
                        <select class="form-control form-control-sm select2 allocation-input" data-placeholder="Peruntukan"></select>
                        <input type="text" class="form-control form-control-sm input-number2" placeholder="Amount" />
                        <div class="input-group-append">
                            <button type="button" class="btn btn-danger btnRemoveAllocation">
                                <i class="fa fa-minus"></i>
                            </button>
                        </div>
                    </div>
                `);

                $("#allocationContainer").append($group);
                var $dropdown = $group.find("select");
                var $amountInput = $group.find("input.input-number2");

                $amountInput.val(amount);

                loadAllocations($dropdown);

                $dropdown.one("loadComplete", function () {
                    $dropdown.val(selectedId).trigger("change");
                });

                $('.select2').select2({
                    allowClear: true,
                    theme: "bootstrap4",
                    dropdownParent: $('body')
                }).next('.select2-container').find('.select2-selection').addClass('form-control-sm');

                $('.input-number2').attr('type', 'text');
                $('.input-number2').on('focus', function () {
                    $(this).val($(this).val().replace(/,/g, ''));
                    $(this).attr('type', 'number');
                });
                $('.input-number2').on('blur', function () {
                    $(this).attr('type', 'text');
                    if ($(this).val()) {
                        var value = parseFloat($(this).val());
                        $(this).val(value.toLocaleString(undefined, {
                            minimumFractionDigits: 2,
                            maximumFractionDigits: 2
                        }));
                    }
                });

                updateRemoveButtons("#allocationContainer", ".btnRemoveAllocation");
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

            loadInitialData();
        });

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
            $(".allocation-group").each(function () {
                var allocationId = $(this).find(".allocation-input").val();
                var amount = $(this).find(".input-number2").val().replace(/,/g, "").trim();

                if (allocationId && amount) {
                    allocations.push({
                        id: allocationId,
                        amount: amount
                    });
                }
            });

            $("#<%= hdnVendorList.ClientID %>").val(vendors.join(","));
            $("#<%= hdnAllocationList.ClientID %>").val(JSON.stringify(allocations));
        }
    </script>
</asp:Content>
