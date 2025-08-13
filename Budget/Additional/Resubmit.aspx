<%@ Page Title="Resubmit Budget Additional" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Resubmit.aspx.cs" Inherits="Prodata.WebForm.Budget.Additional.Resubmit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Panel runat="server" CssClass="card shadow-sm p-4 rounded-3">
        <!-- Header -->
                <div class="card-header card-header-sticky">
            <h2 class="card-title d-none d-sm-inline"><%: Page.Title %></h2>
            <div class="card-tools">
                <asp:LinkButton ID="btnBack" runat="server" CssClass="btn btn-default" PostBackUrl="~/Budget/Additional/Default" CausesValidation="false">
                    <i class="fas fa-angle-double-left"></i> Back
                </asp:LinkButton>
<%--                <asp:LinkButton ID="btnSave" runat="server" CssClass="btn btn-primary" OnClick="btnSave_Click" OnClientClick="collectData();">
                    <i class="fas fa-save"></i> Save
                </asp:LinkButton>--%>
                <asp:LinkButton ID="btnSubmit1" runat="server" CssClass="btn btn-success" OnClick="btnSubmit_Click" OnClientClick="collectData();"> 
                    <i class="fas fa-share"></i> Submit Application
                </asp:LinkButton>
            </div>
        </div>

        <!-- BA Section -->
        <div class="form-group d-flex align-items-center" style="font-size: 2rem;">
            <label class="me-2 mb-0 fw-semibold text-dark">BA :</label>
            <asp:Label ID="LblBA" runat="server" CssClass="fw-bold text-dark me-1" Style="font-size: 2rem;" />
            <span class="fw-bold text-dark" style="font-size: 2rem;">(</span>
            <asp:Label ID="LblBAName" runat="server" CssClass="fw-bold text-dark" Style="font-size: 2rem;" />
            <span class="fw-bold text-dark" style="font-size: 2rem;">)</span>
        </div>

        <!-- Application Info -->
        <div class="row mb-3">
            <div class="col-md-6">
                <label class="form-label">Budget Type</label>
                <div class="form-control-plaintext fw-bold text-dark">
                    <asp:Label runat="server" ID="lblBudgetType" />
                </div>
            </div>
            <div class="col-md-6">
                <label class="form-label">Project / Department</label>
                <div class="form-control-plaintext fw-bold text-dark">
                    <asp:Label ID="lblProject" runat="server" />
                </div>
            </div>
        </div>

        <div class="row mb-3">
            <div class="col-md-6">
                <label class="form-label">Reference No.</label>
                <div class="form-control-plaintext fw-bold text-dark">
                    <asp:Label ID="lblRefNo" runat="server" />
                </div>
            </div>
            <div class="col-md-6">
                <label class="form-label">Application Date</label>
                <div class="form-control-plaintext fw-bold text-dark">
                    <asp:Label ID="lblDate" runat="server" />
                </div>
            </div>
        </div>

        <div class="row mb-3">
            <div class="col-md-6">
                <label class="form-label">Estimated Cost (RM)</label>
                <div class="form-control-plaintext fw-bold text-primary">
                    RM <asp:Label ID="lblBudgetEstimate" runat="server" />
                </div>
            </div>
            <div class="col-md-6">
                <label class="form-label">E-VISA No.</label>
                <div class="form-control-plaintext fw-bold text-dark">
                    <asp:Label ID="lblEVisa" runat="server" />
                </div>
            </div>
        </div>
        
         <div class="mb-4">
            <label class="form-label">Type LOA</label>
            <div class="form-control-plaintext fw-bold text-dark">
                <asp:Label ID="lblCheckType" runat="server" />
            </div>
        </div>

        <!-- Justification -->
         <div class="mb-4">
            <label class="form-label">Application Details</label>
            <div class="form-control-plaintext text-dark">
                <asp:Label ID="lblRequestDetails" runat="server" />
            </div>
        </div>

        <div class="mb-4">
            <label class="form-label">Reason for Application</label>
            <div class="form-control-plaintext text-dark">
                <asp:Label ID="lblReason" runat="server" />
            </div>
        </div>

        <!-- Budget Breakdown -->
        <h4 class="mb-3 border-bottom pb-1">Budget Allocation Breakdown</h4>
        <table class="table table-bordered table-sm text-center align-middle">
            <thead class="table-light">
                <tr>
                    <th>Cost Centre</th>
                    <th>Budget Type</th>
                    <th>Approved Budget (RM)</th>
                    <th>New Budget (RM)</th>
                    <th>Additional Budget (RM)</th>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td><asp:Label ID="lblCostCentre" runat="server" /></td>
                    <td><asp:Label ID="lblTBT" runat="server" /></td>
                    <td class="text-primary"><asp:Label ID="lblApprovedBudget" runat="server" /></td>
                    <td class="text-primary"><asp:Label ID="lblNewTotalBudget" runat="server" /></td>
                    <td class="text-primary fw-bold"><asp:Label ID="lblAdditionalBudget" runat="server" /></td>
                </tr>
            </tbody>
        </table>

        <h4 class="mt-4">Upload Supporting Document</h4>
        <asp:FileUpload ID="fuDocument" runat="server" CssClass="form-control" />
        <asp:RequiredFieldValidator ID="rfvFile" runat="server" ControlToValidate="fuDocument"
            ErrorMessage="Please upload a document" CssClass="text-danger" Display="Dynamic" />

        <h4 class="mt-4">Remark Resubmit</h4>
        <asp:TextBox runat="server" ID="txtResubmit" CssClass="form-control" TextMode="MultiLine" Rows="10" Enabled="true"/>
        <asp:RequiredFieldValidator ID="rfvtxtResubmit" runat="server" ControlToValidate="txtResubmit"
            ErrorMessage="Please Fill Remark" CssClass="text-danger" Display="Dynamic"  />
        
        <!-- Document Uploads -->
        <asp:Panel runat="server" ID="pnlUploadedDocument" CssClass="form-group mt-3" Visible="false">
            <div class="d-flex align-items-center mb-3">
                <div class="bg-success text-white rounded-circle d-flex justify-content-center align-items-center" style="width: 40px; height: 40px;">
                    <i class="fas fa-file-upload"></i>
                </div>
                <div class="ml-3">
                    <h5 class="mb-0">Uploaded Document</h5>
                    <small class="text-muted">You have already uploaded a supporting file for this request.</small>
                </div>
            </div>

            <div class="border p-3 rounded bg-white">
                <asp:PlaceHolder ID="phDocumentList" runat="server" />
            </div>
        </asp:Panel>

        <!-- Hidden Fields -->
        <asp:HiddenField ID="hdncurentRoleApprover" runat="server" />

        <!-- Approval History -->
        <h4 class="mt-4">Approval History</h4>
         <asp:UpdatePanel ID="pnHistoryApproval" runat="server">
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
</asp:Content>