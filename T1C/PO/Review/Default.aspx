<%@ Page Title="PO Review List" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Prodata.WebForm.T1C.PO.Review.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    
    <style>
        @media print {
            /* Hide elements marked as no-print */
            .no-print {
                display: none !important;
            }

            /* Hide general page wrapper elements */
            body * {
                visibility: hidden;
            }

            /* Display the specific print area */
            #printArea, #printArea * {
                visibility: visible;
            }

            /* Reset position for the print area */
            #printArea {
                position: absolute;
                left: 0;
                top: 0;
                width: 100%;
                background-color: white;
            }

            /* Force display of the logo header */
            .print-header {
                display: block !important;
                text-align: center;
                margin-bottom: 20px;
                border-bottom: 2px solid #000;
                padding-bottom: 10px;
            }

            /* Clean up table borders for printing */
            .table {
                width: 100% !important;
                border-collapse: collapse !important;
            }
            .table th, .table td {
                border: 1px solid #000 !important;
                padding: 5px !important;
                color: #000 !important;
            }

            /* Hide the Action column (buttons) */
            .action-column {
                display: none !important;
            }

            /* Ensure background colors (badges) print correctly */
            -webkit-print-color-adjust: exact; 
            print-color-adjust: exact;
        }
    </style>

    <div id="printArea">

        <div class="print-header d-none">
            <img src="<%= ResolveUrl("~/Images/Felda_Global_Ventures_Logo.png") %>" alt="FGV Logo" style="height: 80px;" />
            <h3 class="mt-2">PO Review List</h3>
            <p>Generated on: <%= DateTime.Now.ToString("dd/MM/yyyy HH:mm") %></p>
        </div>

        <div class="row">
            <div class="col-md-12">
                <div class="card card-outline">
                    
                    <div class="card-header no-print">
                        <div class="row">
                            <div class="col-md-3">
                                <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-control select2" AutoPostBack="true" OnSelectedIndexChanged="ddlStatus_SelectedIndexChanged" CausesValidation="false" data-placeholder="All Status">
                                    <asp:ListItem Text="" Value="" />
                                    <asp:ListItem Text="Approved" Value="Approved" />
                                    <asp:ListItem Text="Completed" Value="Completed" Selected="True"/>
                                </asp:DropDownList>
                            </div>
                            
                            <div class="col-md-9 text-right">
                                <button type="button" class="btn btn-default" onclick="window.print();">
                                    <i class="fas fa-print"></i> Print
                                </button>
                            </div>
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
                                                    <asp:BoundField DataField="BizAreaDisplayName" HeaderText="BA" HeaderStyle-CssClass="align-middle" />
                                                    <asp:BoundField DataField="Ref" HeaderText="Reference No" HeaderStyle-CssClass="align-middle text-nowrap" ItemStyle-CssClass="text-nowrap" />
                                                    <asp:BoundField DataField="Date" HeaderText="Date" HeaderStyle-CssClass="align-middle text-nowrap" />
                                                    <asp:BoundField DataField="Details" HeaderText="Details" HeaderStyle-CssClass="align-middle text-nowrap" />
                                                    <asp:BoundField DataField="Amount" HeaderText="Amount (RM)" HeaderStyle-CssClass="align-middle text-nowrap" ItemStyle-CssClass="text-right" />
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
                                                        <HeaderStyle CssClass="width-120 text-center align-middle" />
                                                        <ItemStyle CssClass="width-120 text-center" />
                                                        <ItemTemplate>
                                                            <asp:Label runat="server"
                                                                CssClass='<%#
                                                                    (bool)Eval("IsPendingUserAction") ? "badge badge-primary badge-pill" :
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
                                                                Text='<%# 
                                                                    (bool)Eval("IsPendingUserAction") ? "Pending My Action" :
                                                                    Eval("Status") != null && Eval("Status").ToString().Equals("SentBack", StringComparison.OrdinalIgnoreCase) ? "Sent Back" :
                                                                    Eval("Status")
                                                                %>'>
                                                            </asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    
                                                    <asp:TemplateField HeaderText="PO Reviewed">
                                                        <HeaderStyle CssClass="width-80 text-center align-middle" />
                                                        <ItemStyle CssClass="width-80 text-center" />
                                                        <ItemTemplate>
                                                            <asp:PlaceHolder runat="server" Visible='<%# IsPoReviewed(Eval("Id")) %>'>
                                                                <i class="fas fa-check text-success" title="PO Reviewed"></i>
                                                            </asp:PlaceHolder>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <%-- 6. ADDED 'action-column' CLASS TO HIDE THIS IN PRINT --%>
                                                    <asp:TemplateField HeaderText="Action">
                                                        <HeaderStyle CssClass="width-80 text-center align-middle action-column" />
                                                        <ItemStyle CssClass="width-80 text-center action-column" />
                                                        <ItemTemplate>
                                                            <asp:LinkButton runat="server"
                                                                CssClass='<%# Eval("Status").ToString().ToLower() == "completed" ? "btn btn-outline-secondary btn-xs" : "btn btn-outline-info btn-xs" %>'
                                                                PostBackUrl='<%# $"~/T1C/PO/Review/Review?Id={Eval("Id")}" %>'>
                                                                <i class='<%# Eval("Status").ToString().ToLower() == "completed" ? "fas fa-edit" : "fas fa-eye" %>'></i>
                                                            </asp:LinkButton>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                                <%-- Added no-print to pagination --%>
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
    </div> </asp:Content>