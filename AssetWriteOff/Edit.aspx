<%@ Page Title="Edit Asset Write-Off" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Prodata.WebForm.AssetWriteOff.Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <script type="text/javascript">
        // Same calculation logic as Add.aspx
        function parseFloatSafe(val) {
            if (!val) return 0;
            const num = parseFloat(val.toString().replace(/[^0-9.-]/g, ''));
            return isNaN(num) ? 0 : num;
        }

        function formatCurrencyInput(input, formatOnBlur = false) {
            let value = input.value.replace(/[^0-9.]/g, '');
            const parts = value.split('.');
            if (parts.length > 2) value = parts[0] + '.' + parts.slice(1).join('');
            if (value.startsWith('0') && value.length > 1 && value[1] !== '.') {
                value = value.replace(/^0+/, '');
                if (value === '') value = '0';
            }
            if (parts[1] && parts[1].length > 2) {
                value = parts[0] + '.' + parts[1].substring(0, 2);
            }
            if (formatOnBlur && value) {
                const number = parseFloat(value);
                if (!isNaN(number)) value = number.toFixed(2);
            }
            if (input.value !== value) input.value = value;
        }

        function calculateLiveValues() {
            let totalOrig = 0, totalAcc = 0, totalNet = 0;
            const inputsOrig = document.querySelectorAll('.calc-orig');
            const inputsAcc = document.querySelectorAll('.calc-acc');
            const inputsNet = document.querySelectorAll('.calc-net');

            for (let i = 0; i < inputsOrig.length; i++) {
                const orig = parseFloatSafe(inputsOrig[i].value);
                const acc = parseFloatSafe(inputsAcc[i].value);
                const net = orig - acc;
                inputsNet[i].value = net.toFixed(2);

                totalOrig += orig;
                totalAcc += acc;
                totalNet += net;
            }

            document.getElementById('lblTotalOrig').innerText = totalOrig.toLocaleString('en-US', { minimumFractionDigits: 2 });
            document.getElementById('lblTotalAcc').innerText = totalAcc.toLocaleString('en-US', { minimumFractionDigits: 2 });
            document.getElementById('lblTotalNet').innerText = totalNet.toLocaleString('en-US', { minimumFractionDigits: 2 });
        }

        function calculateAge(dateInput) {
            const row = dateInput.closest('tr');
            const ageInput = row.querySelector('input[name="AgeYears"]');

            if (!dateInput.value) {
                ageInput.value = "0";
                return;
            }

            const acqDate = new Date(dateInput.value);
            const today = new Date();
            let age = today.getFullYear() - acqDate.getFullYear();
            const m = today.getMonth() - acqDate.getMonth();

            if (m < 0 || (m === 0 && today.getDate() < acqDate.getDate())) {
                age--;
            }

            if (age < 0) age = 0;
            ageInput.value = age;
        }

        function addNewRow() {
            const tbody = document.getElementById('detailsBody');
            const rowCount = tbody.querySelectorAll('tr').length;

            const newRow = document.createElement('tr');
            newRow.className = "bg-light";

            newRow.innerHTML = `
                <td class="text-center font-weight-bold row-number align-middle">${rowCount + 1}</td>
                <td><input type="text" name="AssetCode" class="form-control-table" oninput="this.value = this.value.replace(/[^0-9]/g, '');" placeholder="e.g. 780001" /></td>
                <td><input type="text" name="ItemDescription" class="form-control-table" style="text-transform: uppercase;" oninput="this.value = this.value.toUpperCase();" placeholder="Item Name" /></td>
                <td><input type="date" name="AcqDate" class="form-control-table" onchange="calculateAge(this)" /></td>
                <td><input type="number" name="AgeYears" class="form-control-table input-number txtreadonly" placeholder="0" readonly /></td>
                <td><input type="number" name="UsefulLife" class="form-control-table input-number" placeholder="0" /></td>
                <td><input type="number" name="Quantity" class="form-control-table input-number" value="1" /></td>
                <td><input type="text" name="OriginalPrice" class="form-control-table input-number calc-orig text-primary font-weight-bold" placeholder="0.00" oninput="formatCurrencyInput(this); calculateLiveValues();" onblur="formatCurrencyInput(this, true); calculateLiveValues();" /></td>
                <td><input type="text" name="AccDepreciation" class="form-control-table input-number calc-acc text-danger font-weight-bold" placeholder="0.00" oninput="formatCurrencyInput(this); calculateLiveValues();" onblur="formatCurrencyInput(this, true); calculateLiveValues();" /></td>
                <td><input type="text" name="NetBookValue" class="form-control-table input-number txtreadonly calc-net text-success font-weight-bold" placeholder="0.00" readonly /></td>
                <td><textarea name="Reason" class="form-control-table" rows="2" style="resize:vertical;" placeholder="Reason..."></textarea></td>
                <td class="text-center align-middle">
                    <button type="button" class="btn btn-outline-danger btn-sm" onclick="removeRow(this)" title="Remove Row">
                        <i class="fas fa-trash"></i>
                    </button>
                </td>
            `;

            tbody.appendChild(newRow);
            reindexRows();
        }

        function removeRow(btn) {
            const tbody = document.getElementById('detailsBody');
            if (tbody.querySelectorAll('tr').length > 1) {
                btn.closest('tr').remove();
                reindexRows();
                calculateLiveValues();
            } else {
                Swal.fire('Error', 'You must have at least one asset listed.', 'error');
            }
        }

        function reindexRows() {
            const rows = document.querySelectorAll('#detailsBody tr');
            rows.forEach((row, index) => {
                row.querySelector('.row-number').innerText = index + 1;
            });
        }

        function beforeSubmit() {
            const project = document.getElementById('<%= txtProject.ClientID %>').value.trim();
            const justification = document.getElementById('<%= txtJustification.ClientID %>').value.trim();

            if (project === "") {
                Swal.fire({ icon: 'warning', title: 'Missing Information', text: 'Please enter a Project Name.' });
                return false;
            }

            const tbody = document.getElementById('detailsBody');
            const rows = tbody.querySelectorAll('tr');

            if (rows.length === 0) {
                Swal.fire({ icon: 'warning', title: 'Empty Table', text: 'Please add at least one asset to the table.' });
                return false;
            }

            for (let i = 0; i < rows.length; i++) {
                const rowNum = i + 1;
                const code = rows[i].querySelector('input[name="AssetCode"]').value.trim();
                const desc = rows[i].querySelector('input[name="ItemDescription"]').value.trim();
                const date = rows[i].querySelector('input[name="AcqDate"]').value.trim();
                const useful = rows[i].querySelector('input[name="UsefulLife"]').value.trim();
                const qty = rows[i].querySelector('input[name="Quantity"]').value.trim();
                const orig = rows[i].querySelector('input[name="OriginalPrice"]').value.trim();
                const reason = rows[i].querySelector('textarea[name="Reason"]').value.trim();

                if (code === "" || desc === "" || date === "" || useful === "" || qty === "" || orig === "" || reason === "" ) {
                    Swal.fire({
                        icon: 'warning',
                        title: 'Incomplete Row',
                        text: `Please fill in all text fields, dates, and descriptions for the Asset on Row ${rowNum}.`
                    });
                    return false;
                }

                if (parseFloatSafe(orig) <= 0) {
                    Swal.fire({ icon: 'warning', title: 'Invalid Amount', text: `Original Price must be greater than RM 0.00 on Row ${rowNum}.`});
                    return false;
                }
            }

            if (justification === "") {
                Swal.fire({ icon: 'warning', title: 'Missing Information', text: 'Request Justification is required.' });
                return false;
            }

            $("#pagePreloader").fadeIn(200);
            return true;
        }

        // Run calculation once on page load to set initial totals
        window.onload = function () {
            calculateLiveValues();
        };

    </script>

    <style>
        .full-screen-container { width: 100%; padding: 0 15px; }
        .card-custom { border: none; box-shadow: 0 4px 15px rgba(0,0,0,0.08); border-radius: 10px; margin-bottom: 30px; background-color: #fff; }
        .card-header-custom { background-color: #f8f9fa; border-bottom: 2px solid #e9ecef; border-radius: 10px 10px 0 0 !important; padding: 1.25rem 1.5rem; }
        .section-title { color: #2c3e50; font-weight: 600; border-bottom: 2px solid #3498db; padding-bottom: 8px; margin-bottom: 20px; margin-top: 30px; font-size: 1.25rem; }
        .form-label { font-weight: 600; color: #495057; margin-bottom: 0.5rem; }
        
        .table-custom th { background-color: #343a40 !important; color: #ffffff !important; font-weight: 500; vertical-align: middle !important; text-align: center; font-size: 0.9rem;}
        .table-custom td { vertical-align: middle; padding: 0.5rem; }
        .form-control-table { border: 1px solid #ced4da; border-radius: 4px; padding: 0.375rem 0.5rem; font-size: 0.85rem; width: 100%; }
        .form-control-table:focus { border-color: #80bdff; outline: none; box-shadow: 0 0 0 0.2rem rgba(0,123,255,.25); }
        
        .input-number { text-align: right; font-family: 'Consolas', monospace; }
        .txtreadonly { background-color: #e9ecef !important; cursor: not-allowed; opacity: 1; }
        .totals-row td { background-color: #f8f9fa; font-weight: bold; color: #212529; font-size: 1rem; }
        
        input[type="number"]::-webkit-outer-spin-button,
        input[type="number"]::-webkit-inner-spin-button { -webkit-appearance: none; margin: 0; }
        input[type="number"] { -moz-appearance: textfield; }

        .page-preloader { position: fixed; z-index: 99999; top: 0; left: 0; width: 100%; height: 100%; background: rgba(0,0,0,0.7); display: flex; justify-content: center; align-items: center; flex-direction: column; }
        .page-preloader img { animation: pulse 1.5s infinite; }
        @keyframes pulse { 0% { transform: scale(0.95); opacity: 0.8; } 50% { transform: scale(1.05); opacity: 1; } 100% { transform: scale(0.95); opacity: 0.8; } }
    </style>

    <div id="pagePreloader" class="page-preloader" style="display:none;">
        <img src="<%= ResolveUrl("~/Images/Felda_Global_Ventures_Logo.png") %>" alt="Loading..." height="150" width="150" />
        <h4 class="mt-3 text-white font-weight-bold">Processing Application...</h4>
    </div>

    <div class="full-screen-container">
        <asp:Panel runat="server" CssClass="card card-custom">
            
            <div class="card-header card-header-sticky d-flex align-items-center position-sticky top-0" style="z-index: 1000; width: 100%;">
                <h3 class="card-title m-0 fw-bold text-dark">
                    <i class="fas fa-edit mr-2 text-warning"></i><%: Page.Title %>
                </h3>
                
                <div class="card-tools" style="margin-left: auto !important;">
                    <asp:LinkButton ID="btnBack" runat="server" CssClass="btn btn-outline-secondary mr-2" PostBackUrl="~/AssetWriteOff/Default.aspx" CausesValidation="false">
                        <i class="fas fa-angle-double-left"></i> Back
                    </asp:LinkButton>
                    <asp:LinkButton ID="btnSubmit" runat="server" CssClass="btn btn-success" OnClick="btnSubmit_Click" OnClientClick="return beforeSubmit();">
                        <i class="fas fa-share"></i> Re-Submit Application
                    </asp:LinkButton>
                </div>
            </div>

            <div class="card-body p-4">
                
                <div class="alert alert-info d-flex align-items-center shadow-sm mb-4">
                    <i class="fas fa-building fa-2x mr-3"></i>
                    <div>
                        <h5 class="m-0 font-weight-bold">Business Area (BA)</h5>
                        <asp:Label runat="server" ID="lblBA" CssClass="font-weight-normal" style="font-size: 1.1rem;" />
                    </div>
                </div>

                <h4 class="section-title mt-0">Application Information</h4>
                
                <div class="row">
                    <div class="col-md-3 mb-3">
                        <label class="form-label">Reference No.</label>
                        <asp:TextBox runat="server" ID="txtRequestNo" CssClass="form-control txtreadonly font-weight-bold text-primary" ReadOnly="true" />
                    </div>
                    <div class="col-md-3 mb-3">
                        <label class="form-label">Application Date</label>
                        <asp:TextBox runat="server" ID="txtDate" CssClass="form-control txtreadonly" TextMode="Date" ReadOnly="true" />
                    </div>
                    <div class="col-md-6 mb-3">
                        <label class="form-label">Project Name</label>
                        <asp:TextBox runat="server" ID="txtProject" CssClass="form-control" placeholder="Example: KILANG SAWIT..." />
                    </div>
                </div>

                <h4 class="section-title">Asset Details</h4>
                
                <div class="table-responsive shadow-sm border rounded mb-4">
                    <table class="table table-bordered table-custom mb-0">
                        <thead>
                            <tr>
                                <th rowspan="2" style="width: 40px;">NO</th>
                                <th rowspan="2" style="min-width: 120px;">ASSET CODE</th>
                                <th rowspan="2" style="min-width: 180px;">ITEM DESCRIPTION</th>
                                <th colspan="2">DATE OF ACQUISITION</th>
                                <th rowspan="2" style="width: 80px;">USEFUL<br/>LIFE</th>
                                <th rowspan="2" style="width: 60px;">QTY</th>
                                <th colspan="3">PER UNIT PRICE (RM)</th>
                                <th rowspan="2" style="min-width: 180px;">REASON FOR<br/>WRITE-OFF</th>
                                <th rowspan="2" style="width: 80px;">ACTION</th>
                            </tr>
                            <tr>
                                <th style="width: 130px; border-top: 1px solid #454d55;">DATE</th>
                                <th style="width: 70px; border-top: 1px solid #454d55;">AGE</th>
                                <th style="min-width: 120px; border-top: 1px solid #454d55;">ORIGINAL</th>
                                <th style="min-width: 120px; border-top: 1px solid #454d55;">ACC. DEPR.</th>
                                <th style="min-width: 120px; border-top: 1px solid #454d55;">NET BOOK VAL</th>
                            </tr>
                        </thead>
                        
                        <tbody id="detailsBody">
                            <asp:Literal ID="litExistingRows" runat="server" />
                        </tbody>
                        
                        <tbody>
                            <tr class="bg-white">
                                <td colspan="13" class="text-center py-3 border-bottom-0 border-top-0">
                                    <button type="button" class="btn btn-sm btn-primary rounded-pill px-4 shadow-sm" onclick="addNewRow()">
                                        <i class="fas fa-plus mr-1"></i> Add Asset Line
                                    </button>
                                </td>
                            </tr>
                        </tbody>

                        <tfoot class="totals-row">
                            <tr>
                                <td colspan="7" class="text-right text-uppercase pr-3 align-middle">Grand Total (RM)</td>
                                <td class="text-right text-primary"><span id="lblTotalOrig">0.00</span></td>
                                <td class="text-right text-danger"><span id="lblTotalAcc">0.00</span></td>
                                <td class="text-right text-success"><span id="lblTotalNet">0.00</span></td>
                                <td colspan="2"></td>
                            </tr>
                        </tfoot>
                    </table>
                </div>

                <div class="row">
                    <div class="col-md-8 mb-3">
                        <h4 class="section-title mt-0">Justification</h4>
                        <asp:TextBox runat="server" ID="txtJustification" CssClass="form-control" TextMode="MultiLine" Rows="5" placeholder="Provide detailed justification for this write-off request..." />
                    </div>

                    <div class="col-md-4 mb-3">
                        <h4 class="section-title mt-0">Supporting Document</h4>
                        
                        <asp:Panel ID="pnlExistingDocs" runat="server" CssClass="mb-3" Visible="false">
                            <label class="text-muted small font-weight-bold">Previously Uploaded:</label>
                            <asp:PlaceHolder ID="phDocumentList" runat="server"></asp:PlaceHolder>
                        </asp:Panel>

                        <div class="custom-file mb-2">
                            <asp:FileUpload ID="fuDocument" runat="server" CssClass="custom-file-input" onchange="$(this).next('.custom-file-label').html($(this).val().split('\\').pop());" />
                            <label class="custom-file-label" for="fuDocument">Choose new file to replace...</label>
                        </div>
                        <small class="text-muted d-block mb-2">Upload a new document ONLY if you want to replace the existing one.</small>
                    </div>
                </div>

                <div class="card shadow-sm border-0 mt-4">
                    <div class="card-header bg-white border-bottom-0 pt-4 pb-0">
                        <h5 class="fw-bold text-dark"><i class="fas fa-history text-secondary mr-2"></i> Workflow History / Approver Remarks</h5>
                    </div>
                    <div class="card-body">
                        <asp:GridView ID="gvHistory" runat="server" CssClass="table table-sm table-bordered" AutoGenerateColumns="False">
                            <HeaderStyle CssClass="bg-dark text-white" />
                            <Columns>
                                <asp:BoundField DataField="ActionDate" HeaderText="Date & Time" DataFormatString="{0:dd-MMM-yyyy HH:mm}" />
                                <asp:BoundField DataField="RoleName" HeaderText="Role" />
                                <asp:TemplateField HeaderText="Action">
                                    <ItemTemplate>
                                        <span class='badge <%# 
                                            Eval("ActionType").ToString() == "Approve" ? "badge-success" : 
                                            (Eval("ActionType").ToString() == "Reject" ? "badge-danger" : 
                                            (Eval("ActionType").ToString() == "Send Back" ? "badge-info" : "badge-secondary")) %>'>
                                            <%# Eval("ActionType") %>
                                        </span>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="Status" HeaderText="Status" />
                                <asp:BoundField DataField="Remarks" HeaderText="Remarks" />
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>

            </div>
        </asp:Panel>
    </div>
</asp:Content>