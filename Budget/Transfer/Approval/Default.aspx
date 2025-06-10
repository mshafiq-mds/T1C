<%@ Page Title="Transfer Approval" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Prodata.WebForm.Budget.Transfer.Approval.Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="hdnDeleteId" runat="server" />
    <asp:Button ID="btnDeleteConfirmed" runat="server" CssClass="d-none" OnClick="btnDeleteConfirmed_Click" />

    <asp:Panel runat="server" CssClass="card p-4">
        <div class="card-tools mb-3">
     <%--       <asp:LinkButton ID="btnAdd" runat="server" CssClass="btn btn-primary" PostBackUrl="/Budget/Transfer/Add" CausesValidation="false">
                <i class="fas fa-plus"></i> Request Transfer Budget
            </asp:LinkButton>--%>
        </div>

        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <asp:GridView ID="gvTransfers" runat="server"
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
                        <asp:BoundField DataField="Status" HeaderText="Status"/>

                        <asp:TemplateField HeaderText="Action">
                            <HeaderStyle CssClass="width-80 text-center align-middle" />
                            <ItemStyle CssClass="width-80 text-center" />
                            <ItemTemplate>
                                <asp:PlaceHolder ID="phEditButton" runat="server" Visible='<%# Eval("CanEdit") %>'>
                                    <a class="btn btn-default btn-xs" 
                                       href='/Budget/Transfer/Approval/Approval?Id=<%# Eval("Id") %>'>
                                       <i class="fas fa-check"></i>
                                    </a>
                                </asp:PlaceHolder>
                                <a class="btn btn-default btn-xs" href='/Budget/Transfer/Approval/View?Id=<%# Eval("Id") %>'>
                                    <i class="fas fa-eye"></i>
                                </a>
                            </ItemTemplate> 
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </ContentTemplate>
        </asp:UpdatePanel>
    </asp:Panel>

    <!-- SweetAlert script -->
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>

    <script type="text/javascript">
        $(document).ready(function () {
            $(document).on("click", ".button-delete", function (e) {
                e.preventDefault();

                var id = $(this).attr("commandargument");

                Swal.fire({
                    title: 'Are you sure?',
                    text: "This transfer will be deleted!",
                    icon: 'warning',
                    showCancelButton: true,
                    confirmButtonColor: '#d33',
                    cancelButtonColor: '#3085d6',
                    confirmButtonText: 'Yes, delete it!'
                }).then((result) => {
                    if (result.isConfirmed) {
                        $('#<%= hdnDeleteId.ClientID %>').val(id);
                        __doPostBack('<%= btnDeleteConfirmed.UniqueID %>', '');
                    }
                });
            });
        });
    </script>
</asp:Content>