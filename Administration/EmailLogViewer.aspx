<%@ Page Title="Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EmailLogViewer.aspx.cs" Inherits="Prodata.WebForm.Administration.EmailLogViewer" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        /* Scoped Font & Light Theme */
        .email-app-wrapper { font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, Helvetica, Arial, sans-serif; background-color: #f8f9fa; color: #333333; }
        
        /* Adjusted container to fit inside your Master Page */
        .app-container { display: flex; height: 80vh; min-height: 600px; width: 100%; border: 1px solid #e9ecef; border-radius: 8px; overflow: hidden; text-align: left; margin-top: 20px; box-shadow: 0 4px 6px rgba(0,0,0,0.05); }
        
        /* Left Sidebar (Email List) */
        .email-item { padding: 15px; border-bottom: 1px solid #e9ecef; cursor: pointer; transition: background-color 0.2s; }
        .email-item:hover { background-color: #f1f3f5; }
        .email-item.active { background-color: #0d6efd; color: white; }
        .email-item.active .text-muted { color: #e2e8f0; }
        .email-subject { font-weight: 600; font-size: 14px; margin-bottom: 5px; white-space: nowrap; overflow: hidden; text-overflow: ellipsis; }
        .email-meta { display: flex; justify-content: space-between; font-size: 12px; }
        .text-muted { color: #6c757d; }
        
        /* Right Main Content */
        .main-content { flex: 1; display: flex; flex-direction: column; background-color: #f8f9fa; }
        
        /* Email Header Details */
        .email-header { padding: 20px; border-bottom: 1px solid #e9ecef; background-color: #ffffff; }
        .header-row { margin-bottom: 5px; font-size: 14px; }
        .header-label { font-weight: bold; color: #495057; width: 50px; display: inline-block; }
        
        /* Tabs */
        .tabs { display: flex; border-bottom: 1px solid #e9ecef; padding: 0 20px; margin-top: 15px; }
        .tab { padding: 10px 15px; cursor: pointer; font-size: 13px; font-weight: 600; color: #6c757d; border-bottom: 2px solid transparent; }
        .tab:hover { color: #212529; }
        .tab.active { color: #0d6efd; border-bottom: 2px solid #0d6efd; }
        
        /* Preview Area */
        .preview-container { flex: 1; padding: 20px; display: flex; justify-content: center; background-color: #f8f9fa; overflow: hidden; }
        .preview-wrapper { width: 100%; max-width: 800px; background: white; border-radius: 8px; overflow: hidden; display: flex; flex-direction: column; box-shadow: 0 2px 4px rgba(0,0,0,0.05); border: 1px solid #e9ecef; }
        
        #iframePreview { width: 100%; height: 100%; border: none; flex: 1; background: white; }
        #sourcePreview { width: 100%; height: 100%; border: none; flex: 1; background: #f8f9fa; color: #212529; padding: 15px; font-family: Consolas, monospace; display: none; white-space: pre-wrap; overflow-y: auto; box-sizing: border-box; }
        
        /* Status Badges */
        .status-success { color: #198754; border: 1px solid #198754; padding: 2px 6px; border-radius: 10px; font-size: 11px; background-color: #eafaf1; }
        .status-failed { color: #dc3545; border: 1px solid #dc3545; padding: 2px 6px; border-radius: 10px; font-size: 11px; background-color: #fce8e6; }
    </style>

    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>

    <div class="email-app-wrapper">
        <div class="app-container">
            <div class="sidebar" id="emailListContainer">
                </div>

            <div class="main-content" style="display: none;" id="mainContent">
                <div class="email-header">
                    <div class="header-row" style="display: flex; justify-content: space-between;">
                        <div>
                            <span class="header-label">From:</span> 
                            &lt;siserver.fps@fgvholdings.com&gt;
                        </div>
                        <div id="lblDate" class="text-muted"></div>
                    </div>
                    <div class="header-row">
                        <span class="header-label">To:</span> 
                        <span id="lblTo"></span>
                    </div>
                    <div class="header-row" style="margin-top: 10px;">
                        <span id="lblStatus"></span>
                    </div>

                    <div class="tabs">
                        <div class="tab active" data-target="html">HTML</div>
                        <div class="tab" data-target="source">HTML Source</div>
                    </div>
                </div>

                <div class="preview-container">
                    <div class="preview-wrapper">
                        <iframe id="iframePreview"></iframe>
                        <textarea id="sourcePreview" readonly="readonly"></textarea>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        $(document).ready(function () {
            LoadEmailList();

            // Tab Switching Logic
            $('.tab').click(function () {
                $('.tab').removeClass('active');
                $(this).addClass('active');

                var target = $(this).data('target');
                if (target === 'html') {
                    $('#iframePreview').show();
                    $('#sourcePreview').hide();
                } else {
                    $('#iframePreview').hide();
                    $('#sourcePreview').show();
                }
            });
        });

        function LoadEmailList() {
            $.ajax({
                type: "POST",
                // Make sure this URL matches your actual file path structure if it's inside /Administration/
                url: "EmailLogViewer.aspx/GetEmailList",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    var emails = response.d;
                    var container = $('#emailListContainer');
                    container.empty();

                    if (!emails || emails.length === 0) {
                        container.append('<div style="padding: 20px; text-align: center; color: #8b949e;">No email logs found.</div>');
                        return;
                    }

                    $.each(emails, function (i, email) {
                        var html = `
                            <div class="email-item" onclick="LoadEmailDetail(${email.LogID}, this)">
                                <div class="email-subject">${email.Subject}</div>
                                <div class="email-meta">
                                    <span>to: &lt;${email.RecipientEmail}&gt;</span>
                                    <span class="text-muted">${email.FormattedDate}</span>
                                </div>
                            </div>
                        `;
                        container.append(html);
                    });

                    // Auto-load the first email
                    $('.email-item').first().click();
                },
                error: function (err) {
                    console.error("Error loading email list:", err);
                }
            });
        }

        function LoadEmailDetail(logId, element) {
            // Update UI Selection
            $('.email-item').removeClass('active');
            $(element).addClass('active');

            $.ajax({
                type: "POST",
                url: "EmailLogViewer.aspx/GetEmailDetail",
                data: JSON.stringify({ logId: logId }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    var detail = response.d;

                    // Show right pane
                    $('#mainContent').show();

                    // Update Headers
                    $('#lblTo').text('<' + detail.RecipientEmail + '>');
                    $('#lblDate').text(detail.ExactDate);

                    var statusClass = detail.Status === 'Success' ? 'status-success' : 'status-failed';
                    $('#lblStatus').html(`<span class="${statusClass}">${detail.Status}</span>`);

                    // Ensure defaults if body is null
                    var bodyContent = detail.Body || '<i>No body content.</i>';

                    // Update HTML Preview
                    $('#iframePreview').attr('srcdoc', bodyContent);

                    // Update Source Preview
                    $('#sourcePreview').val(bodyContent);
                },
                error: function (err) {
                    console.error("Error loading detail:", err);
                }
            });
        }
    </script>
</asp:Content>