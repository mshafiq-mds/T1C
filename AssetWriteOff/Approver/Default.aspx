<%@ Page Title="AWO Approval" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Prodata.WebForm.AssetWriteOff.Approver.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="card shadow-sm border-0 mt-4">
        <div class="card-header bg-white border-bottom-0 pt-4 pb-3">
            <h4 class="mb-3 text-dark fw-bold"><i class="fas fa-clipboard-check text-primary mr-2"></i> Asset Write-Off Approvals</h4>
            
            <div class="row">
                <div class="col-md-2 mb-2 mb-md-0">
                    <label class="form-label fw-bold">Filter by Year:</label>
                    <asp:DropDownList ID="ddlYear" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlFilter_SelectedIndexChanged">
                    </asp:DropDownList>
                </div>

                <div class="col-md-3 mb-2 mb-md-0">
                    <label class="form-label fw-bold">Filter by Status:</label>
                    <asp:DropDownList ID="ddlStatusFilter" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlFilter_SelectedIndexChanged">
                        <asp:ListItem Text="Pending My Action" Value="MyAction" Selected="True"></asp:ListItem>
                        <asp:ListItem Text="All Pending" Value="Pending"></asp:ListItem>
                        <asp:ListItem Text="Approved" Value="Approved"></asp:ListItem>
                        <asp:ListItem Text="Rejected" Value="Rejected"></asp:ListItem>
                        <asp:ListItem Text="All Transactions" Value="All"></asp:ListItem>
                    </asp:DropDownList>
                </div>
            </div>
        </div>

        <div class="card-body p-0">
            <asp:GridView ID="gvRequests" runat="server" CssClass="table table-hover table-striped align-middle mb-0" 
                AutoGenerateColumns="False" GridLines="None" EmptyDataText="No requests found for the selected criteria."
                DataKeyNames="Id" OnRowDataBound="gvRequests_RowDataBound">
                <Columns>
                    <asp:TemplateField HeaderText="#">
                        <ItemStyle CssClass="text-center fw-bold" Width="50px" />
                        <ItemTemplate><%# Container.DataItemIndex + 1 %></ItemTemplate>
                    </asp:TemplateField>

                    <asp:BoundField DataField="RequestNo" HeaderText="Ref No" />
                    <%--<asp:BoundField DataField="BACode" HeaderText="BA Code" />--%>
                    <asp:BoundField DataField="Date" HeaderText="Date" />
                    <asp:BoundField DataField="Project" HeaderText="Project Name" />
                    
                    <asp:TemplateField HeaderText="Highest Value (RM)">
                        <ItemStyle CssClass="text-left fw-bold text-primary" />
                        <ItemTemplate>
                            <%# Eval("NetBookValue", "{0:N2}") %>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Status">
                        <HeaderStyle CssClass="text-center" />
                        <ItemStyle CssClass="text-center" />
                        <ItemTemplate>
                            <span class='badge <%# 
                                Eval("Status").ToString() == "Approved" ? "badge-success" : 
                                (Eval("Status").ToString() == "Rejected" ? "badge-danger" : "badge-warning") %> px-2 py-1'>
                                <%# Eval("Status") %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Pending With">
                        <HeaderStyle CssClass="text-center" />
                        <ItemStyle CssClass="text-center font-weight-bold text-muted" />
                        <ItemTemplate>
                            <%# string.IsNullOrEmpty(Eval("NextApprover")?.ToString()) || Eval("NextApprover")?.ToString() == "-" ? "-" : Eval("NextApprover") %>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Action">
                        <HeaderStyle CssClass="text-center" />
                        <ItemStyle CssClass="text-center" Width="160px" />
                        <ItemTemplate>
                            <asp:HyperLink ID="hlReview" runat="server" 
                                NavigateUrl='<%# "~/AssetWriteOff/Approver/Edit.aspx?id=" + Eval("Id") %>' 
                                CssClass="btn btn-primary btn-sm font-weight-bold">
                                <i class="fas fa-edit"></i> Review
                            </asp:HyperLink>

                            <asp:HyperLink ID="hlViewOnly" runat="server" 
                                NavigateUrl='<%# "~/AssetWriteOff/Approver/Edit.aspx?id=" + Eval("Id") %>' 
                                CssClass="btn btn-outline-secondary btn-sm">
                                <i class="fas fa-eye"></i> View
                            </asp:HyperLink>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>
</asp:Content>