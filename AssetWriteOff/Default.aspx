<%@ Page Title="Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Prodata.WebForm.AssetWriteOff.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    
    <style>
        .full-screen-container { width: 100%; padding: 0 15px; }
        .card-custom { border: none; box-shadow: 0 4px 15px rgba(0,0,0,0.08); border-radius: 10px; margin-bottom: 30px; }
        .card-header-custom { background-color: #f8f9fa; border-bottom: 2px solid #e9ecef; border-radius: 10px 10px 0 0 !important; padding: 1.25rem 1.5rem; }
        .table-custom th { background-color: #343a40 !important; color: #ffffff !important; font-weight: 500; vertical-align: middle !important; text-align: center; font-size: 0.9rem;}
        .table-custom td { vertical-align: middle; }
        .badge-custom { font-size: 0.85rem; padding: 0.4em 0.6em; }
    </style>

    <div class="full-screen-container">
        
        <div class="card card-custom mb-4">
            <div class="card-body p-3">
                <div class="row align-items-end">
                    <div class="col-md-3 mb-2 mb-md-0">
                        <label class="form-label text-muted small font-weight-bold">Search (Ref No / Project)</label>
                        <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control form-control-sm" placeholder="Type to search..."></asp:TextBox>
                    </div>
                    <div class="col-md-3 mb-2 mb-md-0">
                        <label class="form-label text-muted small font-weight-bold">Status</label>
                        <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-control form-control-sm">
                            <asp:ListItem Text="All Status" Value=""></asp:ListItem>
                            <asp:ListItem Text="Submitted / Pending" Value="Pending"></asp:ListItem>
                            <asp:ListItem Text="Sent Back" Value="sentback"></asp:ListItem>
                            <asp:ListItem Text="Approved" Value="Approved"></asp:ListItem>
                            <asp:ListItem Text="Rejected" Value="Rejected"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-6 text-right">
                        <asp:LinkButton ID="btnSearch" runat="server" CssClass="btn btn-sm btn-secondary mr-1" OnClick="btnSearch_Click">
                            <i class="fas fa-search"></i> Search
                        </asp:LinkButton>
                        <asp:LinkButton ID="btnClear" runat="server" CssClass="btn btn-sm btn-outline-secondary mr-3" OnClick="btnClear_Click">
                            <i class="fas fa-undo"></i> Clear
                        </asp:LinkButton>
                        
                        <asp:LinkButton ID="btnAddNew" runat="server" CssClass="btn btn-sm btn-primary shadow-sm" PostBackUrl="~/AssetWriteOff/Add.aspx">
                            <i class="fas fa-plus-circle"></i> New Application
                        </asp:LinkButton>
                    </div>
                </div>
            </div>
        </div>

        <div class="card card-custom">
            <div class="card-header card-header-custom d-flex align-items-center">
                <h3 class="card-title m-0 fw-bold text-dark">
                    <i class="fas fa-list-alt mr-2 text-primary"></i><%: Page.Title %>
                </h3>
            </div>
            
            <div class="card-body p-0">
                <div class="table-responsive">
                    <asp:GridView ID="gvAWO" runat="server" CssClass="table table-hover table-striped table-custom mb-0" 
                        AutoGenerateColumns="False" GridLines="None" ShowHeaderWhenEmpty="True" EmptyDataText="No write-off applications found."
                        AllowPaging="True" PageSize="10" OnPageIndexChanging="gvAWO_PageIndexChanging" OnRowDataBound="gvAWO_RowDataBound">
                        <Columns>
                            
                            <asp:TemplateField HeaderText="No.">
                                <ItemStyle CssClass="text-center" Width="50px" />
                                <ItemTemplate>
                                    <%# Container.DataItemIndex + 1 + (gvAWO.PageIndex * gvAWO.PageSize) %>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:BoundField DataField="RequestNo" HeaderText="Ref. No">
                                <ItemStyle CssClass="font-weight-bold text-primary" />
                            </asp:BoundField>

                            <asp:BoundField DataField="Date" HeaderText="Date" DataFormatString="{0:dd-MMM-yyyy}">
                                <ItemStyle CssClass="text-center" />
                            </asp:BoundField>

                            <asp:BoundField DataField="BACode" HeaderText="BA Code">
                                <ItemStyle CssClass="text-center font-weight-bold" />
                            </asp:BoundField>

                            <asp:BoundField DataField="Project" HeaderText="Project Name" />

                            <asp:TemplateField HeaderText="Status">
                                <ItemStyle CssClass="text-center" />
                                <ItemTemplate>
                                    <asp:Label ID="lblStatusBadge" runat="server" Text='<%# Eval("Status") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Action">
                                <ItemStyle CssClass="text-center" Width="100px" />
                                <ItemTemplate>
                                    <asp:HyperLink ID="hlView" runat="server" NavigateUrl='<%# "~/AssetWriteOff/View.aspx?id=" + Eval("Id") %>' CssClass="btn btn-info btn-sm" ToolTip="View Details">
                                        <i class="fas fa-eye"></i>
                                    </asp:HyperLink>

                                    <asp:HyperLink ID="hlEdit" runat="server" NavigateUrl='<%# "~/AssetWriteOff/Edit.aspx?id=" + Eval("Id") %>' CssClass="btn btn-warning btn-sm" ToolTip="Edit & Resubmit" Visible="false">
                                        <i class="fas fa-edit"></i>
                                    </asp:HyperLink>
                                </ItemTemplate>
                            </asp:TemplateField>

                        </Columns>
                        <PagerStyle CssClass="pagination-ys" />
                    </asp:GridView>
                </div>
            </div>
        </div>

    </div>

</asp:Content>