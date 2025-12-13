<%@ Page Title="T1C" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Prodata.WebForm.T1C.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    
    <style>
        @media print {
            /* Hide the Search Card */
            #<%= divCardSearch.ClientID %> {
                display: none !important;
            }

            /* Hide everything by default to reset layout */
            body * {
                visibility: hidden;
            }

            /* Show the specific print area */
            #printArea, #printArea * {
                visibility: visible;
            }

            /* Position the print area */
            #printArea {
                position: absolute;
                left: 0;
                top: 0;
                width: 100%;
                background-color: white;
            }

            /* Hide Buttons and Card Tools (Add/Print buttons) */
            .card-tools, .btn, .no-print {
                display: none !important;
            }

            /* Hide the Action Column in GridView */
            .action-column {
                display: none !important;
            }

            /* Show the Print Header (Logo) */
            .print-header {
                display: block !important;
                text-align: center;
                margin-bottom: 20px;
                border-bottom: 2px solid #000;
                padding-bottom: 10px;
            }

            /* Clean up table styling */
            .table {
                width: 100% !important;
                border-collapse: collapse !important;
            }
            .table th, .table td {
                border: 1px solid #000 !important;
                padding: 5px !important;
                color: #000 !important;
            }

            /* Ensure background colors print */
            -webkit-print-color-adjust: exact; 
            print-color-adjust: exact;
        }
    </style>

    <asp:HiddenField ID="hdnRecordId" runat="server" />
    <asp:Button ID="btnDeleteRecord" runat="server" OnClick="btnDeleteRecord_Click" CssClass="d-none" />
    
    <div id="printArea">

        <div class="print-header d-none">
            <img src="<%= ResolveUrl("~/Images/Felda_Global_Ventures_Logo.png") %>" alt="FGV Logo" style="height: 80px;" />
            <h3 class="mt-2">T1C Budget List</h3>
            <p>Generated on: <%= DateTime.Now.ToString("dd/MM/yyyy HH:mm") %></p>
        </div>

        <div class="row no-print"> <div class="col-md-12">
                <div id="divCardSearch" runat="server" class="card card-outline collapsed-card">
                    <div class="card-header card-header-sticky" data-card-widget="collapse">
                        <h3 class="card-title">Search</h3>
                    </div>
                    <div class="card-body">
                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group row">
                                    <asp:Label ID="lblYear" runat="server" CssClass="col-md-3 col-form-label" AssociatedControlID="ddlYear" Text="Year"></asp:Label>
                                    <div class="col-md-5">
                                        <asp:DropDownList ID="ddlYear" runat="server" CssClass="form-control select2" data-placeholder="Year"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="form-group row">
                                    <asp:Label ID="lblRef" runat="server" CssClass="col-md-3 col-form-label" AssociatedControlID="txtRef" Text="Reference No"></asp:Label>
                                    <div class="col-md-5">
                                        <asp:TextBox ID="txtRef" runat="server" CssClass="form-control" TextMode="Search" placeholder="Reference No"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="form-group row">
                                    <asp:Label ID="lblStartDate" runat="server" CssClass="col-md-3 col-form-label" AssociatedControlID="txtStartDate" Text="Start Date"></asp:Label>
                                    <div class="col-md-5">
                                        <asp:TextBox ID="txtStartDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="form-group row">
                                    <asp:Label ID="lblEndDate" runat="server" CssClass="col-md-3 col-form-label" AssociatedControlID="txtEndDate" Text="End Date"></asp:Label>
                                    <div class="col-md-5">
                                        <asp:TextBox ID="txtEndDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="form-group row">
                                    <asp:Label ID="lblMinAmount" runat="server" CssClass="col-md-3 col-form-label" AssociatedControlID="txtMinAmount" Text="Min Amount"></asp:Label>
                                    <div class="col-md-5">
                                        <asp:TextBox ID="txtMinAmount" runat="server" CssClass="form-control input-number" TextMode="Number" placeholder="Min Amount"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="form-group row">
                                    <asp:Label ID="lblMaxAmount" runat="server" CssClass="col-md-3 col-form-label" AssociatedControlID="txtMaxAmount" Text="Max Amount"></asp:Label>
                                    <div class="col-md-5">
                                        <asp:TextBox ID="txtMaxAmount" runat="server" CssClass="form-control input-number" TextMode="Number" placeholder="Max Amount"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-9 offset-md-3">
                                        <asp:LinkButton ID="btnSearch" runat="server" CssClass="btn btn-outline-secondary" OnClick="btnSearch_Click">
                                            <i class="fas fa-search"></i> Search
                                        </asp:LinkButton>
                                        <asp:LinkButton ID="btnReset" runat="server" CssClass="btn btn-link" PostBackUrl="~/T1C/Default">
                                            <i class="fas fa-sync"></i> Reset
                                        </asp:LinkButton>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="row">
            <div class="col-md-12">
                <div class="card card-outline">
                    <div class="card-header card-header-sticky">
                        <h3 class="card-title d-none d-sm-inline"><%= Page.Title %></h3>
                        <div class="card-tools">
                            <button type="button" class="btn btn-default mr-1" onclick="window.print();">
                                <i class="fas fa-print"></i> Print
                            </button>

                            <asp:LinkButton ID="btnAdd" runat="server" CssClass="btn btn-primary" PostBackUrl="~/T1C/Addv1" CausesValidation="false">
                                <i class="fas fa-plus"></i> New Budget T1C
                            </asp:LinkButton>
                        </div>
                    </div>
                    <div class="card-body">
                        <div class="row">
                            <div class="col-12">
                                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                    <ContentTemplate>
                                        <div class="table-responsive">
                                            <asp:GridView ID="gvData" runat="server" AutoGenerateColumns="false" CssClass="table table-bordered" PageSize='<%# FGV.Prodata.App.Setting.RecordsPerPage() %>' AllowPaging="true" OnPageIndexChanging="gvData_PageIndexChanging" EmptyDataText="No record.">
                                                <Columns>
                                                    <asp:TemplateField HeaderText="#">
                                                        <HeaderStyle CssClass="width-30 text-center" />
                                                        <ItemStyle CssClass="width-30 text-center" />
                                                        <ItemTemplate>
                                                            <%# Container.DataItemIndex + 1 %>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="Ref" HeaderText="Reference No" HeaderStyle-CssClass="align-middle text-nowrap" ItemStyle-CssClass="text-nowrap" />
                                                    <asp:BoundField DataField="Date" HeaderText="Date" HeaderStyle-CssClass="align-middle text-nowrap" />
                                                    <asp:BoundField DataField="Details" HeaderText="Details" HeaderStyle-CssClass="align-middle text-nowrap" />
                                                    <asp:BoundField DataField="Amount" HeaderText="Amount (RM)" HeaderStyle-CssClass="align-middle text-nowrap text-center" ItemStyle-CssClass="text-right" />
                                                    <asp:TemplateField HeaderText="Next Approver">
                                                        <HeaderStyle CssClass="align-middle text-nowrap text-center" />
                                                        <ItemStyle CssClass="text-center" />
                                                        <ItemTemplate>
                                                            <%# Eval("Status") != null && Eval("Status").ToString().Equals("Completed", StringComparison.OrdinalIgnoreCase) 
                                                                ? "<span class='text-success font-weight-bold'>Complete</span>" 
                                                                : Eval("NextApprover") %>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Status">
                                                        <HeaderStyle CssClass="width-80 text-center align-middle" />
                                                        <ItemStyle CssClass="width-80 text-center" />
                                                        <ItemTemplate>
                                                            <asp:Label runat="server" 
                                                                CssClass='<%# 
                                                                    Eval("Status") != null ? (
                                                                        Eval("Status").ToString().ToLower() == "completed" ? "badge badge-dark badge-pill" :
                                                                        Eval("Status").ToString().ToLower() == "approved" ? "badge badge-success badge-pill" :
                                                                        Eval("Status").ToString().ToLower() == "pending" ? "badge badge-warning badge-pill" :
                                                                        Eval("Status").ToString().ToLower() == "rejected" ? "badge badge-danger badge-pill" :
                                                                        Eval("Status").ToString().ToLower() == "draft" ? "badge badge-info badge-pill" :
                                                                        Eval("Status").ToString().ToLower() == "sentback" ? "badge badge-secondary badge-pill" :
                                                                        "badge badge-secondary badge-pill"
                                                                    ) : "badge badge-secondary badge-pill"
                                                                %>' 
                                                                Text='<%# Eval("Status").ToString().Equals("SentBack", StringComparison.OrdinalIgnoreCase) ? "Sent Back" : Eval("Status") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    
                                                    <%-- 5. HIDE ACTION COLUMN IN PRINT --%>
                                                    <asp:TemplateField HeaderText="Action">
                                                        <HeaderStyle CssClass="width-80 text-center align-middle action-column" />
                                                        <ItemStyle CssClass="width-80 text-center text-nowrap action-column" />
                                                        <ItemTemplate>
                                                            <asp:LinkButton ID="btnView" runat="server" CssClass="btn btn-outline-info btn-xs" PostBackUrl='<%# $"~/T1C/View?Id={Eval("Id")}" %>'>
                                                                <i class="fas fa-eye"></i>
                                                            </asp:LinkButton>
                                                            <a class="btn btn-outline-secondary btn-xs<%# (bool)Eval("IsEditable") ? "" : " disabled" %>" href='Edit?Id=<%# Eval("Id") %>' onclick='<%# (bool)Eval("IsEditable") ? "" : "return false;" %>'>
                                                                <i class="fas fa-edit"></i>
                                                            </a>
                                                            <asp:LinkButton ID="btnDelete" runat="server" CssClass='<%# (bool)Eval("IsEditable") ? "btn btn-danger btn-xs button-delete" : "btn btn-danger btn-xs disabled" %>' data-id='<%# Eval("Id") %>' Visible='<%# Prodata.WebForm.Auth.Can(Prodata.WebForm.Auth.Id(), "t1c-delete") %>'>
                                                                <i class="fas fa-trash-alt"></i>
                                                            </asp:LinkButton>
                                                            <asp:HiddenField ID="hdnFormId" runat="server" Value='<%# Eval("Id") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                                <%-- Hide Pagination in Print --%>
                                                <PagerSettings Mode="NumericFirstLast" PageButtonCount="5" Position="TopAndBottom" />
                                                <PagerStyle CssClass="pagination-ys no-print" />
                                            </asp:GridView>
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div> <script>
        $(document).ready(function () {
            // Use event delegation for dynamically rendered buttons
            $(document).on("click", ".button-delete", function (event) {
                event.preventDefault(); // Prevent immediate postback

                var recordId = $(this).data("id");

                Swal.fire({
                    title: "Are you sure?",
                    text: "This record will be deleted permanently!",
                    icon: "warning",
                    showCancelButton: true,
                    confirmButtonColor: "#d33",
                    cancelButtonColor: "#3085d6",
                    confirmButtonText: "Yes, delete it!"
                }).then((result) => {
                    if (result.isConfirmed) {  // ✅ Only proceed if the user confirms
                        $('#<%= hdnRecordId.ClientID %>').val(recordId);
                        __doPostBack("<%= btnDeleteRecord.UniqueID %>", "");
                    }
                });
            });
        });
    </script>
</asp:Content>