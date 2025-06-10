<%@ Page Title="Additional Budget" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Prodata.WebForm.Budget.AddBudget.Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <asp:Panel runat="server" CssClass="card p-4">
        <div class="card-tools">
            <asp:LinkButton ID="btnAdd" runat="server" CssClass="btn btn-primary" PostBackUrl="/Budget/Additional/Add" CausesValidation="false">
                <i class="fas fa-plus"></i>  Request Additional Budget
            </asp:LinkButton>
        </div>

         <div class="mb-3"></div>

        <asp:GridView ID="gvBudgetList" runat="server" CssClass="table table-bordered table-sm" AutoGenerateColumns="False"
            EmptyDataText="No budget applications found." DataKeyNames="Id" PageSize='<%# FGV.Prodata.App.Setting.RecordsPerPage() %>' AllowPaging="true">
            <Columns>
                <asp:BoundField DataField="RefNo" HeaderText="Reference No." />
                <asp:BoundField DataField="ApplicationDate" HeaderText="Date" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:BoundField DataField="BA" HeaderText="BA" />
                <asp:BoundField DataField="Project" HeaderText="Project / Department" />
                <asp:BoundField DataField="EstimatedCost" HeaderText="Estimated Cost (RM)" DataFormatString="{0:N2}" />
                <asp:BoundField DataField="EVisa" HeaderText="E-VISA No." />
                <asp:BoundField DataField="TotalCost" HeaderText="Total Cost (RM)" DataFormatString="{0:N2}" />
                <asp:BoundField DataField="AdditionalBudget" HeaderText="Additional Budget (RM)" DataFormatString="{0:N2}" />
           <%--     <asp:TemplateField HeaderText="Actions">
                    <ItemTemplate>
                        <asp:HyperLink ID="lnkView" runat="server" Text="View" NavigateUrl='<%# Eval("Id", "View.aspx?id={0}") %>' CssClass="btn btn-sm btn-info" />
                    </ItemTemplate>
                </asp:TemplateField>--%>

                <asp:TemplateField HeaderText="Action">
                    <HeaderStyle CssClass="width-80 text-center align-middle" />
                    <ItemStyle CssClass="width-80 text-center" />
                    <ItemTemplate>
                        <%--<a class="btn btn-info btn-xs<%# (bool)Eval("IsEditable") ? "" : " disabled" %>" href='/T1C/Edit?Id=<%# Eval("Id") %>' onclick='<%# (bool)Eval("IsEditable") ? "" : "return false;" %>'>
                            <i class="fas fa-edit"></i>
                        </a>
                        <asp:LinkButton ID="btnDelete" runat="server" CssClass="btn btn-danger btn-xs button-delete" data-id='<%# Eval("Id") %>' Visible='<%# Prodata.WebForm.Auth.Can(Prodata.WebForm.Auth.Id(), "admin-user-delete") %>'>
                            <i class="fas fa-trash-alt"></i>
                        </asp:LinkButton>--%>
                         <a class="btn btn-info" href='/Additional/Edit?Id=1' >
                             <i class="fas fa-edit"></i>
                         </a>
                         <asp:LinkButton ID="btnDelete" runat="server" CssClass="btn btn-danger btn-xs button-delete" Visible='<%# Prodata.WebForm.Auth.Can(Prodata.WebForm.Auth.Id(), "admin-user-delete") %>'>
                             <i class="fas fa-trash-alt"></i>
                         </asp:LinkButton>                          
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </asp:Panel>
</asp:Content>

