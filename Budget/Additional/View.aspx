<%@ Page Title="View Additional Budget" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="View.aspx.cs" Inherits="Prodata.WebForm.Budget.AddBudget.View" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Panel runat="server" CssClass="card p-4">
        <div class="card-header card-header-sticky">
            <h2 class="card-title d-none d-sm-inline"><%: Page.Title %></h2>
            <div class="card-tools">
                <asp:LinkButton ID="btnBack" runat="server" CssClass="btn btn-default" PostBackUrl="/Budget/Additional/Default" CausesValidation="false">
                    <i class="fas fa-angle-double-left"></i> Back
                </asp:LinkButton>
                <asp:Button ID="btnPrint" runat="server" CssClass="btn btn-info" Text="Print" OnClientClick="printPanel(); return false;" />
            </div>
        </div>

        <div class="form-group d-flex align-items-center" style="font-size: 2rem;">
            <label class="me-2 mb-0 fw-semibold text-dark">BA :</label>
            <asp:Label ID="LblBA" runat="server" CssClass="fw-bold text-dark me-1" Style="font-size: 2rem;" />
            <span class="fw-bold text-dark" style="font-size: 2rem;">(</span>
            <asp:Label ID="LblBAName" runat="server" CssClass="fw-bold text-dark" Style="font-size: 2rem;" />
            <span class="fw-bold text-dark" style="font-size: 2rem;">)</span>
        </div>

        <div class="form-group">
            <label>Budget Type</label>
            <p class="form-control-plaintext fw-bold">
                <asp:Label runat="server" ID="lblBudgetType" />
            </p>
        </div>

        <div class="form-group">
            <label>Project / Department</label>
            <asp:Label ID="lblProject" runat="server" CssClass="form-control-plaintext" />
        </div>

        <div class="form-group">
            <label>Reference No.</label>
            <asp:Label ID="lblRefNo" runat="server" CssClass="form-control-plaintext" />
        </div>

        <div class="form-group">
            <label>Application Date</label>
            <asp:Label ID="lblDate" runat="server" CssClass="form-control-plaintext" />
        </div>

        <div class="form-group">
            <label>Estimated Cost (RM)</label>
            <asp:Label ID="lblBudgetEstimate" runat="server" CssClass="form-control-plaintext" />
        </div>

        <div class="form-group">
            <label>E-VISA No.</label>
            <asp:Label ID="lblEVisa" runat="server" CssClass="form-control-plaintext" />
        </div>

        <div class="form-group">
            <label>Application Details</label>
            <asp:Label ID="lblRequestDetails" runat="server" CssClass="form-control-plaintext" />
        </div>

        <div class="form-group">
            <label>Reason for Application</label>
            <asp:Label ID="lblReason" runat="server" CssClass="form-control-plaintext" />
        </div>

        <h4 class="mt-4">Additional Budget Allocation Details</h4>
        <table class="table table-bordered">
            <thead>
                <tr>
                    <th>Cost Centre</th>
                    <th>GL</th>
                    <th>Approved Budget 2022</th>
                    <th>New Budget 2022</th>
                    <th>Additional Budget Value</th>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td><asp:Label ID="lblCostCentre" runat="server" /></td>
                    <td><asp:Label ID="lblGL" runat="server" /></td>
                    <td><asp:Label ID="lblApprovedBudget" runat="server" /></td>
                    <td><asp:Label ID="lblNewTotalBudget" runat="server" /></td>
                    <td><asp:Label ID="lblAdditionalBudget" runat="server" /></td>
                </tr>
            </tbody>
        </table>

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
                        <asp:BoundField DataField="ActionType" HeaderText="Action" />
                        <asp:BoundField DataField="RoleName" HeaderText="Role" /> 
                        <asp:BoundField DataField="Status" HeaderText="Status"/>
                        <asp:BoundField DataField="Remarks" HeaderText="Remarks"/>
                    </Columns>
                </asp:GridView>
            </ContentTemplate>
        </asp:UpdatePanel>

    </asp:Panel>

    <script type="text/javascript">
        function printPanel() {
            var panel = document.querySelector('.card');
            var printWindow = window.open('', '', 'height=800,width=1000');
            printWindow.document.write('<html><head><title>Print Report</title>');

            var styles = document.querySelectorAll('link[rel="stylesheet"], style');
            styles.forEach(function (style) {
                printWindow.document.write(style.outerHTML);
            });

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
