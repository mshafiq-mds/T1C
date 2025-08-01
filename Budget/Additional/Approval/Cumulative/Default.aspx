﻿<%@ Page Title="Additional Approval Cumulative" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Prodata.WebForm.Budget.Additional.Approval.Cumulative.Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <asp:Panel runat="server" CssClass="card p-4">
        <div class="d-flex justify-content-between align-items-center mb-3 flex-wrap">
            <div class="form-inline">
                <%--<label for="ddlStatusFilter" class="mr-2 mb-0">Filter by Status:</label>--%>
                <asp:DropDownList ID="ddlStatusFilter" runat="server" AutoPostBack="true" CssClass="form-control select2" Style="width: 300px;"
                    OnSelectedIndexChanged="ddlStatusFilter_SelectedIndexChanged" data-placeholder="All">
                    <asp:ListItem Text="" Value=""/>
                    <%--<asp:ListItem Text="All" Value="All" Selected="True"/>--%>
                    <asp:ListItem Text="Submitted" Value="Submitted" />
                    <asp:ListItem Text="Resubmit" Value="Resubmit" />
                    <asp:ListItem Text="Under Review" Value="Under Review" />
                    <asp:ListItem Text="Completed" Value="Completed" />
                    <asp:ListItem Text="Finalized" Value="Finalized" />
                    <asp:ListItem Text="Deleted" Value="Deleted" />
                    <asp:ListItem Text="User Action" Value="EditableOnly" Selected="True"/>
                </asp:DropDownList>

                <div class="d-inline-block ms-3">
                    <span class="fw-bold text-dark">Cumulative Balance:</span>
                    <asp:Label runat="server" ID="lblUsed" CssClass="badge bg-success text-white me-1">0</asp:Label>
                    <span class="text-muted">/</span>
                    <asp:Label runat="server" ID="lblcumulative" CssClass="badge bg-secondary text-white ms-1">0</asp:Label>
                </div>

            </div>
        </div>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <asp:GridView ID="gvAdditionalBudgetList" runat="server"
                              CssClass="table table-bordered table-sm"
                              PageSize='<%# FGV.Prodata.App.Setting.RecordsPerPage() %>'
                              AllowPaging="true"
                              AutoGenerateColumns="False"
                              DataKeyNames="Id"
                              OnPageIndexChanging="gvList_PageIndexChanging"
                              EmptyDataText="No record.">
                    <Columns>
                        <asp:BoundField DataField="BA" HeaderText="BA" />
                        <asp:BoundField DataField="RefNo" HeaderText="Reference No" />
                        <asp:BoundField DataField="Project" HeaderText="Project" />
                        <asp:BoundField DataField="ApplicationDate" HeaderText="Application Date" DataFormatString="{0:dd/MM/yyyy}" />
                        <asp:BoundField DataField="AdditionalBudget" HeaderText="Additional Budget (RM)" DataFormatString="{0:N2}" />
                        <asp:TemplateField HeaderText="Status">
                            <ItemTemplate>
                                <asp:Label ID="lblStatus" runat="server"
                                    Text='<%# Eval("Status") %>'
                                    CssClass='<%# 
                                          Eval("Status").ToString() == "Deleted" ? "text-danger fw-bold" :
                                          Eval("Status").ToString() == "Resubmit" ? "text-warning fw-bold" :
                                          Eval("Status").ToString() == "Under Review" ? "text-primary" :
                                          Eval("Status").ToString() == "Completed" ? "text-success" :
                                          Eval("Status").ToString() == "Finalized" ? "text-muted fst-italic" :
                                          Eval("Status").ToString() == "User Action" ? "text-info fw-bold" :
                                          ""
                                    %>'>
                                </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Action">
                             <HeaderStyle CssClass="text-center align-middle" Width="100px" />
                             <ItemStyle CssClass="text-center align-middle" />
                             <ItemTemplate>
                                 <div class="btn-group" role="group">
                                     <asp:PlaceHolder ID="phEditButton" runat="server" Visible='<%# Eval("CanEdit") %>'>
                                         <a class="btn btn-outline-primary btn-sm" 
                                            href='/Budget/Additional/Approval/Cumulative/Approval?Id=<%# Eval("Id") %>'
                                            data-toggle="tooltip" data-placement="top" title="Edit">
                                             <i class="fas fa-edit"></i>
                                         </a>
                                     </asp:PlaceHolder>
                                     <a class="btn btn-outline-secondary btn-sm" 
                                        href='/Budget/Additional/Approval/Cumulative/View?Id=<%# Eval("Id") %>'
                                        data-toggle="tooltip" data-placement="top" title="View">
                                         <i class="fas fa-eye"></i>
                                     </a>
                                 </div>
                             </ItemTemplate>
                         </asp:TemplateField>  
                    </Columns>
                </asp:GridView>
            </ContentTemplate>
        </asp:UpdatePanel>
    </asp:Panel>
     
</asp:Content>