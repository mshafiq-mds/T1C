<%@ Page Title="T1C Approval" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Prodata.WebForm.T1C.Approval.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="col-md-12">
            <div class="card card-outline">
                <div class="card-header">
                    <h3 class="card-title"><%= Page.Title %></h3>
                </div>
                <div class="card-body">
                    <div class="row">
                        <div class="col-12">
                            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                <ContentTemplate>
                                    <div class="table-responsive">
                                        <asp:GridView ID="gvData" runat="server" AutoGenerateColumns="false" CssClass="table table-bordered table-sm" PageSize='<%# FGV.Prodata.App.Setting.RecordsPerPage() %>' AllowPaging="true" OnPageIndexChanging="gvData_PageIndexChanging" EmptyDataText="No record.">
                                            <Columns>
                                                <asp:TemplateField HeaderText="#">
                                                    <HeaderStyle CssClass="width-30 text-center" />
                                                    <ItemStyle CssClass="width-30 text-center" />
                                                    <ItemTemplate>
                                                        <%# Container.DataItemIndex + 1 %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="Ref" HeaderText="No. Rujukan" HeaderStyle-CssClass="align-middle text-nowrap" ItemStyle-CssClass="text-nowrap" />
                                                <asp:BoundField DataField="Date" HeaderText="Tarikh" HeaderStyle-CssClass="align-middle text-nowrap" />
                                                <asp:BoundField DataField="Details" HeaderText="Butir-butir" HeaderStyle-CssClass="align-middle text-nowrap" />
                                                <asp:BoundField DataField="Amount" HeaderText="Anggaran Kerja (RM)" HeaderStyle-CssClass="align-middle text-nowrap" ItemStyle-CssClass="text-right" />
                                                <asp:TemplateField HeaderText="Action">
                                                    <HeaderStyle CssClass="width-80 text-center align-middle" />
                                                    <ItemStyle CssClass="width-80 text-center" />
                                                    <ItemTemplate>
                                                        <a class="btn btn-default btn-xs" href='/T1C/Approval/Edit?Id=<%# Eval("Id") %>'>
                                                            <i class="fas fa-check"></i>
                                                        </a>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                            <PagerSettings Mode="NumericFirstLast" PageButtonCount="5" Position="TopAndBottom" />
                                            <PagerStyle CssClass="pagination-ys" />
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
</asp:Content>
