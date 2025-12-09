<%@ Page Title="Edit Budget Type" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Prodata.WebForm.MasterData.BudgetType.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/sweetalert2@11/dist/sweetalert2.min.css" rel="stylesheet"/>
<script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
        <style>
        .custom-control.custom-radio {
            margin-bottom: -15px;
        }

        .custom-control-label {
            font-weight: normal !important;
        }

        th[data-bs-toggle="tooltip"] {
            cursor: help; /* help cursor to show it's hoverable */
        } 

        td[data-bs-toggle="tooltip"] {
            cursor: help; /* show help cursor so users know it's hoverable */
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
    <!-- Page-specific Preloader -->
<div id="pagePreloader" class="page-preloader" style="display:none;">
    <img src="<%= ResolveUrl("~/Images/Felda_Global_Ventures_Logo.png") %>" 
         alt="Loading..." height="200" width="200" />
    <p class="mt-3 text-white">Processing...</p>
</div>

    <!-- Modal for Add/Edit -->
    <div class="modal fade" id="purchaseTypeModal" tabindex="-1" role="dialog">
        <div class="modal-dialog modal-md" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="modalTitle">Add Purchase Type</h5>
                    <button type="button" class="close" data-dismiss="modal"><span>&times;</span></button>
                </div>
                <div class="modal-body">
                    <asp:HiddenField ID="hdnEditId" runat="server" />
                    <div class="form-group">
                        <label>Code</label>
                        <asp:TextBox ID="txtptcode" runat="server" CssClass="form-control"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvtxtptcode" runat="server" ControlToValidate="txtptcode"
                            CssClass="text-danger" Display="Dynamic" ErrorMessage="Code is required"
                            ValidationGroup="FormPT"></asp:RequiredFieldValidator>
                    </div>
                    <div class="form-group">
                        <label>Purchase Type</label>
                        <asp:TextBox ID="txtptname" runat="server" CssClass="form-control"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvtxtptname" runat="server" ControlToValidate="txtptname"
                            CssClass="text-danger" Display="Dynamic" ErrorMessage="Purchase Type is required"
                            ValidationGroup="FormPT"></asp:RequiredFieldValidator>
                    </div>
                </div>
                <div class="modal-footer">
                    <asp:Button ID="Button1" runat="server" Text="Save" CssClass="btn btn-success" OnClick="btnSave_ClickPT" ValidationGroup="FormPT"/>
                    <button type="button" class="btn btn-secondary" data-dismiss="modal">Cancel</button>
                </div>
            </div>
        </div>
    </div>


    <div class="row">
        <div class="col-md-12">
            <div class="card card-outline">
                <div class="card-header card-header-sticky">
                    <h3 class="card-title d-none d-sm-inline"><%: Page.Title %></h3>
                    <div class="card-tools">
                        <asp:LinkButton ID="btnBack" runat="server" CssClass="btn btn-default" PostBackUrl="~/MasterData/BudgetType/Default" CausesValidation="false">
                            <i class="fas fa-angle-double-left"></i> Back
                        </asp:LinkButton>
                        <asp:LinkButton ID="btnSave" runat="server" CssClass="btn btn-primary" OnClick="btnSave_Click" ValidationGroup="MainForm">
                            <i class="fas fa-save"></i> Save
                        </asp:LinkButton>
                    </div>
                </div>

                <div class="card-body">
                    <asp:HiddenField ID="hdnId" runat="server" />
                    <div class="row">
                        <div class="col-md-8 offset-md-2">

                            <div class="form-group row">
                                <asp:Label ID="lblCode" runat="server" AssociatedControlID="txtCode"
                                    CssClass="col-md-3 col-form-label text-md-right" Text="Code"></asp:Label>
                                <div class="col-md-7">
                                    <asp:TextBox ID="txtCode" runat="server" CssClass="form-control input-uppercase" placeholder="Code"></asp:TextBox>
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCode"
                                        CssClass="text-danger" Display="Dynamic"
                                        ErrorMessage="Code is required" ValidationGroup="MainForm"></asp:RequiredFieldValidator>
                                </div>
                            </div>

                            <div class="form-group row">
                                <asp:Label ID="lblName" runat="server" AssociatedControlID="txtName"
                                    CssClass="col-md-3 col-form-label text-md-right" Text="Name"></asp:Label>
                                <div class="col-md-7">
                                    <asp:TextBox ID="txtName" runat="server" CssClass="form-control" placeholder="Name"></asp:TextBox>
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtName"
                                        CssClass="text-danger" Display="Dynamic"
                                        ErrorMessage="Name is required" ValidationGroup="MainForm"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            
                            <div class="form-group row">
                                <asp:Label ID="lblFormCategories" runat="server" AssociatedControlID="ddlFormCategories"
                                    CssClass="col-md-3 col-form-label text-md-right" Text="Form Category"></asp:Label>
                                <div class="col-md-7">
                                    <asp:DropDownList ID="ddlFormCategories" runat="server" CssClass="form-control">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlFormCategories"
                                        InitialValue="" CssClass="text-danger" Display="Dynamic"
                                        ErrorMessage="Form category is required"></asp:RequiredFieldValidator>
                                </div>
                            </div>

                            <div class="form-group row">
                                <asp:Label ID="Label1" runat="server" AssociatedControlID="ddlBudgetCategories"
                                    CssClass="col-md-3 col-form-label text-md-right" Text="Budget Category"></asp:Label>
                                <div class="col-md-7">
                                    <asp:DropDownList ID="ddlBudgetCategories" runat="server" CssClass="form-control">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlBudgetCategories"
                                        InitialValue="" CssClass="text-danger" Display="Dynamic"
                                        ErrorMessage="Budget category is required"></asp:RequiredFieldValidator>
                                </div>
                            </div>

                            <div class="form-group row">
                                <asp:Label ID="Label2" runat="server"  AssociatedControlID="btnAddNew"
                                    CssClass="col-md-3 col-form-label text-md-right" Text="Purchase Type"></asp:Label>
                                <div class="col-md-7">
                                    <asp:Button ID="btnAddNew" runat="server" Text="Add New Purchase Type" CssClass="btn btn-primary mb-2" OnClick="btnAddNew_Click" />
                                      
                                </div>
                            </div>
                            <div class="form-group row">
                            <asp:Label ID="Label3" runat="server"  
                                CssClass="col-md-3 col-form-label text-md-right" Text=""></asp:Label>
                            <div class="col-md-7">

                                <asp:GridView ID="gvBudgetType" runat="server" AutoGenerateColumns="false"
                                    CssClass="table table-bordered table-sm"
                                    PageSize='<%# FGV.Prodata.App.Setting.RecordsPerPage() %>'
                                    AllowPaging="true"
                                    OnPageIndexChanging="gvBudgetType_PageIndexChanging"
                                    OnRowDataBound="gvBudgetType_RowDataBound"
                                    EmptyDataText="No record.">

                                    <Columns> 
                                        <asp:TemplateField HeaderText="#">
                                            <HeaderStyle CssClass="width-30 text-center" />
                                            <ItemStyle CssClass="width-30 text-center" />
                                            <ItemTemplate>
                                                <%# Container.DataItemIndex + 1 %>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:BoundField DataField="Code" HeaderText="Code" />
                                        <asp:BoundField DataField="Name" HeaderText="Purchase Type" />

                                        <asp:TemplateField HeaderText="Action">
                                            <HeaderStyle CssClass="width-120 text-center" />
                                            <ItemStyle CssClass="width-120 text-center" />
                                            <ItemTemplate>
                                                <asp:LinkButton ID="btnEdit" runat="server" CssClass="btn btn-info btn-xs"
                                                    OnCommand="btnEdit_Command" CommandArgument='<%# Eval("Id") %>'
                                                    Visible='<%# Prodata.WebForm.Auth.Can(Prodata.WebForm.Auth.Id(), "budget-type-edit") %>'>
                                                    <i class="fas fa-edit"></i>
                                                </asp:LinkButton>

                                                <asp:LinkButton ID="btnDelete" runat="server" CssClass="btn btn-danger btn-xs btn-delete"
                                                    OnCommand="btnDelete_Command" CommandArgument='<%# Eval("Id") %>'
                                                    Visible='<%# Prodata.WebForm.Auth.Can(Prodata.WebForm.Auth.Id(), "budget-type-delete") %>'>
                                                    <i class="fas fa-trash-alt"></i>
                                                </asp:LinkButton>

                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>

                                    <PagerSettings Mode="NumericFirstLast" PageButtonCount="5" Position="TopAndBottom" />
                                    <PagerStyle CssClass="pagination-ys" />
                                </asp:GridView>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

            </div>
        </div>
    </div> 
    <script>
        document.addEventListener("DOMContentLoaded", function () {

            // Preloader buttons
            const preloaderButtons = document.querySelectorAll('.btn-preload');
            preloaderButtons.forEach(btn => {
                btn.addEventListener('click', function () {
                    document.getElementById('pagePreloader').style.display = 'flex';
                });
            });

            // SweetAlert Delete buttons
            const deleteButtons = document.querySelectorAll('.btn-delete');
            deleteButtons.forEach(btn => {
                btn.addEventListener('click', function (e) {
                    e.preventDefault();

                    const commandArgument = this.getAttribute('commandargument');
                    const uniqueID = this.getAttribute('data-uniqueid');

                    Swal.fire({
                        title: 'Are you sure?',
                        text: "This action cannot be undone!",
                        icon: 'warning',
                        showCancelButton: true,
                        confirmButtonColor: '#d33',
                        cancelButtonColor: '#3085d6',
                        confirmButtonText: 'Yes, delete it!'
                    }).then((result) => {
                        if (result.isConfirmed) {
                            // Show preloader and trigger server-side delete
                            document.getElementById('pagePreloader').style.display = 'flex';
                            __doPostBack(uniqueID, commandArgument);
                        }
                    });
                });
            });

        });
    </script> 
</asp:Content>
