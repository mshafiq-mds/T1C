<%@ Page Title="Transfer Application List" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Prodata.WebForm.Budget.Transfer.TransferApplication.Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">


    <asp:Panel runat="server" CssClass="card p-4">
        <div class="card-tools mb-3">
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
            </div>
        </div>

        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <asp:GridView ID="gvTransfers" runat="server"
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
                        <asp:BoundField DataField="Date" HeaderText="Application Date" DataFormatString="{0:dd/MM/yyyy}" />
                        <asp:BoundField DataField="EstimatedCost" HeaderText="Estimated Cost (RM)" DataFormatString="{0:N2}" />
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
                                           href='Application?Id=<%# Eval("Id") %>'
                                           data-toggle="tooltip" data-placement="top" title="Edit">
                                            <i class="fas fa-edit"></i>
                                        </a>
                                    </asp:PlaceHolder>
                                    <a class="btn btn-outline-secondary btn-sm" 
                                       href='View?Id=<%# Eval("Id") %>'
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