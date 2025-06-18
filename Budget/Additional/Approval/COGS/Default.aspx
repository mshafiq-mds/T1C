<%@ Page Title="Additional Approval COGS" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Prodata.WebForm.Budget.Additional.Approval.COGS.Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <asp:Panel runat="server" CssClass="card p-4">
        <div class="card-tools mb-3"> 
        </div>

        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <asp:GridView ID="gvAdditionalBudgetList" runat="server"
                              CssClass="table table-bordered table-sm"
                              PageSize='<%# FGV.Prodata.App.Setting.RecordsPerPage() %>'
                              AllowPaging="true"
                              AutoGenerateColumns="False"
                              DataKeyNames="Id"
                              EmptyDataText="No record.">
                    <Columns>
                        <asp:BoundField DataField="BA" HeaderText="BA" />
                        <asp:BoundField DataField="RefNo" HeaderText="Reference No" />
                        <asp:BoundField DataField="Project" HeaderText="Project" />
                        <asp:BoundField DataField="Date" HeaderText="Application Date" DataFormatString="{0:dd/MM/yyyy}" />
                        <asp:BoundField DataField="EstimatedCost" HeaderText="Estimated Cost (RM)" DataFormatString="{0:N2}" />
                        <asp:TemplateField HeaderText="Status">
                            <ItemTemplate>
                                <asp:Label ID="lblStatus" runat="server"
                                    Text='<%# Eval("Status") %>'
                                    CssClass='<%#
                                        Eval("Status").ToString() == "Deleted" ? "text-danger font-weight-bold" :
                                        Eval("Status").ToString() == "Resubmit" ? "text-warning font-weight-bold" :
                                        Eval("Status").ToString() == "Under Review" ? "text-info" :
                                        Eval("Status").ToString() == "Completed" ? "text-success" :
                                        "text-primary"
                                    %>'>
                                </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Action">
                            <HeaderStyle CssClass="width-80 text-center align-middle" />
                            <ItemStyle CssClass="width-80 text-center" />
                            <ItemTemplate>
                                <asp:PlaceHolder ID="phEditButton" runat="server" Visible='<%# Eval("CanEdit") %>'>
                                    <a class="btn btn-default btn-xs" 
                                       href='/Budget/Additional/Approval/COGS/Approval?Id=<%# Eval("Id") %>'>
                                       <i class="fas fa-check"></i>
                                    </a>
                                </asp:PlaceHolder>
                                <a class="btn btn-default btn-xs" href='/Budget/Additional/Approval/COGS/View?Id=<%# Eval("Id") %>'>
                                    <i class="fas fa-eye"></i>
                                </a>
                            </ItemTemplate> 
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </ContentTemplate>
        </asp:UpdatePanel>
    </asp:Panel>
     
</asp:Content>