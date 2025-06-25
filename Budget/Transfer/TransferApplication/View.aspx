<%@ Page Title="Transfer Application View" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="View.aspx.cs" Inherits="Prodata.WebForm.Budget.Transfer.TransferApplication.View" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Panel runat="server" CssClass="card p-4 shadow-sm rounded">

        <!-- Header Bar -->
        <div class="card-header card-header-sticky">
            <h2 class="card-title d-none d-sm-inline"><%: Page.Title %></h2>
            <div class="card-tools">
                <asp:LinkButton ID="btnBack" runat="server" CssClass="btn btn-default" PostBackUrl="/Budget/Transfer/TransferApplication/Default" CausesValidation="false">
                    <i class="fas fa-angle-double-left"></i> Back
                </asp:LinkButton>
                <asp:LinkButton ID="btnPrint" runat="server" CssClass="btn btn-print" CausesValidation="false"
                    OnClientClick="printPanel(); return false;">
                    <i class="fas fa-print"></i> Print
                </asp:LinkButton>

                <%--<asp:LinkButton ID="btnSave" runat="server" CssClass="btn btn-primary btn-revision">
                    <i class="fas fa-edit"></i> Request Revision
                </asp:LinkButton>
                <asp:LinkButton ID="btnSubmit1" runat="server" CssClass="btn btn-success btn-approve">
                    <i class="fas fa-circle"></i> Approve Transfer
                </asp:LinkButton>--%>
            </div>
        </div>

        <!-- Transfer Info Section -->
        <div class="row mt-4">
            <div class="col-md-6 mb-3">
                <label class="fw-bold">Reference No:</label>
                <div><asp:Label ID="lblRefNo" runat="server" /></div>
            </div>
            <div class="col-md-6 mb-3">
                <label class="fw-bold">Application Date:</label>
                <div><asp:Label ID="lblDate" runat="server" /></div>
            </div>
            <div class="col-md-6 mb-3">
                <label class="fw-bold">Project:</label>
                <div><asp:Label ID="lblProject" runat="server" /></div>
            </div>
            <div class="col-md-6 mb-3">
                <label class="fw-bold">BA:</label>
                <div><asp:Label ID="lblBA" runat="server" /></div>
            </div>
            <div class="col-md-6 mb-3">
                <label class="fw-bold">Budget Type:</label>
                <div><asp:Label ID="lblBudgetType" runat="server" /></div>
            </div>
            <div class="col-md-6 mb-3">
                <label class="fw-bold">Estimated Cost (RM):</label>
                <div><asp:Label ID="lblEstimatedCost" runat="server" /></div>
            </div>
            <div class="col-md-6 mb-3">
                <label class="fw-bold">E-VISA No.:</label>
                <div><asp:Label ID="lblEVisa" runat="server" /></div>
            </div>
        </div>

        <!-- Budget Table -->
        <h4 class="mt-4">Budget Transfer Details</h4>
        <table class="table table-striped table-bordered table-hover">
            <thead class="table-primary text-center">
                <tr>
                    <th>Item</th>
                    <th>GL/Asset Class</th>
                    <th>BA</th>
                    <th>Original Budget (RM)</th>
                    <th>Current Balance (RM)</th>
                    <th>Transfer Amount (RM)</th>
                    <th>Balance After Transfer (RM)</th>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td>From Budget</td>
                    <td><asp:Label ID="lblFromGL" runat="server" /></td>
                    <td><asp:Label ID="lblFromBA" runat="server" /></td>
                    <td><asp:Label ID="lblFromBudget" runat="server" /></td>
                    <td><asp:Label ID="lblFromBalance" runat="server" /></td>
                    <td><asp:Label ID="lblFromTransfer" runat="server" /></td>
                    <td><asp:Label ID="lblFromAfter" runat="server" /></td>
                </tr>
                <tr>
                    <td>To Budget</td>
                    <td><asp:Label ID="lblToGL" runat="server" /></td>
                    <td><asp:Label ID="lblToBA" runat="server" /></td>
                    <td><asp:Label ID="lblToBudget" runat="server" /></td>
                    <td><asp:Label ID="lblToBalance" runat="server" /></td>
                    <td><asp:Label ID="lblToTransfer" runat="server" /></td>
                    <td><asp:Label ID="lblToAfter" runat="server" /></td>
                </tr>
            </tbody>
        </table>

        <!-- Justification -->
        <h4 class="mt-4">Justification</h4>
        <div class="border rounded p-3 bg-light">
            <asp:Literal ID="litJustification" runat="server" Mode="Encode" />
        </div>

        <!-- Uploaded Documents -->
        <h4 class="mt-4">Uploaded Document</h4>
        <asp:Panel runat="server" ID="pnlUploadedDocument" CssClass="form-group" Visible="false">
            <asp:PlaceHolder ID="phDocumentList" runat="server" />
        </asp:Panel>
         
        <h4 class="mt-4">Approval History</h4>
         <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <asp:GridView ID="gvHistory" runat="server"
                              CssClass="table table-bordered table-sm" 
                              AllowPaging="true"
                              AutoGenerateColumns="False"
                              DataKeyNames="Id"
                              EmptyDataText="No record.">
                    <Columns>
                        <asp:BoundField DataField="ActionDate" HeaderText="Action Date" />
                        <asp:BoundField DataField="ActionType" HeaderText="Role Action" />
                        <asp:BoundField DataField="RoleName" HeaderText="Role" /> 
                        <asp:BoundField DataField="Status" HeaderText="Status"/>
                        <asp:BoundField DataField="Remarks" HeaderText="Remarks"/>
                    </Columns>
                </asp:GridView>
            </ContentTemplate>
        </asp:UpdatePanel>

    </asp:Panel>

  <!-- PRINT SCRIPT -->
<script type="text/javascript">
    function printPanel() {
        var panel = document.querySelector('.card');
        var printWindow = window.open('', '', 'height=800,width=1000');
        printWindow.document.write('<html><head><title>Print Report</title>');

        // Copy all linked stylesheets and inline styles
        var styles = document.querySelectorAll('link[rel="stylesheet"], style');
        styles.forEach(function (style) {
            printWindow.document.write(style.outerHTML);
        });

        // Print-specific styles
        printWindow.document.write(`
            <style>
                .btn, .card-header-sticky, .navbar, .footer {
                    display: none !important;
                }
                body, .card {
                    margin: 0;
                    padding: 0;
                    box-shadow: none;
                }
            </style>
        `);

        printWindow.document.write('</head><body>');
        printWindow.document.write(panel.outerHTML);
        printWindow.document.write('</body></html>');
        printWindow.document.close();
        printWindow.focus();
        printWindow.print();

        printWindow.onafterprint = function () {
            printWindow.close();
        };
    }
</script>
   
</asp:Content>

