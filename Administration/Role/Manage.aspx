<%@ Page Title="Edit Role" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Manage.aspx.cs" Inherits="Prodata.WebForm.Administration.Role.Manage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="col-md-12">
            <div class="card card-outline">
                <div class="card-header">
                    <h3 class="card-title d-none d-sm-inline"><%: Page.Title %></h3>
                    <div class="card-tools">
                        <asp:LinkButton ID="btnBack" runat="server" CssClass="btn btn-default" PostBackUrl="~/Administration/Role/Default" CausesValidation="false">
                            <i class="fas fa-angle-double-left"></i> Back
                        </asp:LinkButton>
                        <asp:LinkButton ID="btnSave" runat="server" CssClass="btn btn-primary" OnClick="btnSave_Click">
                            <i class="fas fa-save"></i> Save
                        </asp:LinkButton> 
                    </div>
                </div>
                <div class="card-body">
                    <div class="row">
                        <div class="col-12">
                            <div class="card card-outline card-outline-tabs">
                                <div class="card-header p-0 border-bottom-0">
                                    <ul class="nav nav-tabs" id="custom-tabs-three-tab" role="tablist">
                                        <li class="nav-item">
                                            <a class="nav-link active" id="detail-tab" data-toggle="pill" href="#detail-tab-content" role="tab" aria-controls="detail-tab-content" aria-selected="true">Detail</a>
                                        </li>
                                        <li class="nav-item">
                                            <a class="nav-link" id="permission-tab" data-toggle="pill" href="#permission-tab-content" role="tab" aria-controls="permission-tab-content" aria-selected="false">Permission</a>
                                        </li>
                                    </ul>
                                </div>
                                <div class="card-body">
                                    <div class="tab-content" id="custom-tabs-three-tabContent">
                                        <div class="tab-pane fade show active" id="detail-tab-content" role="tabpanel" aria-labelledby="detail-tab">
                                            <asp:HiddenField ID="hdnRoleId" runat="server" />
                                            <div class="form-group row">
                                                <asp:Label runat="server" AssociatedControlID="txtRoleName" CssClass="col-md-3 col-form-label text-md-right" Text="Role Name" />
                                                <div class="col-md-7">
                                                    <asp:TextBox runat="server" ID="txtRoleName" CssClass="form-control" placeholder="Role Name" />
                                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtRoleName" CssClass="text-danger" Display="Dynamic" ErrorMessage="Role name is required" />
                                                    <asp:Label ID="lblRoleErrors" runat="server" CssClass="text-danger" Visible="false"></asp:Label>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="tab-pane fade" id="permission-tab-content" role="tabpanel" aria-labelledby="permission-tab">
                                            <asp:TreeView ID="permission" runat="server" ShowLines="True" ShowCheckBoxes="All"></asp:TreeView>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <script type="text/javascript">
        $(document).ready(function () {
            $('#<%= txtRoleName.ClientID %>').on('input', function () {
                $('#<%= lblRoleErrors.ClientID %>').hide();
            });
        });

        function OnTreeClick(evt) {
            var src = window.event != window.undefined ? window.event.srcElement : evt.target;
            var isChkBoxClick = (src.tagName.toLowerCase() == "input" && src.type == "checkbox");
            if (isChkBoxClick) {
                var parentTable = GetParentByTagName("table", src);
                var nxtSibling = parentTable.nextSibling;
                if (nxtSibling && nxtSibling.nodeType == 1)//check if nxt sibling is not null & is an element node
                {
                    if (nxtSibling.tagName.toLowerCase() == "div") //if node has children
                    {
                        //check or uncheck children at all levels
                        CheckUncheckChildren(parentTable.nextSibling, src.checked);
                    }
                }
                //check or uncheck parents at all levels
                CheckUncheckParents(src, src.checked);
            }
        }

        function CheckUncheckChildren(childContainer, check) {
            var childChkBoxes = childContainer.getElementsByTagName("input");
            var childChkBoxCount = childChkBoxes.length;
            for (var i = 0; i < childChkBoxCount; i++) {
                childChkBoxes[i].checked = check;
            }
        }

        function CheckUncheckParents(srcChild, check) {
            var parentDiv = GetParentByTagName("div", srcChild);
            var parentNodeTable = parentDiv.previousSibling;

            if (!parentNodeTable) return;

            var inpElemsInParentTable = parentNodeTable.getElementsByTagName("input");
            if (inpElemsInParentTable.length > 0) {
                var parentNodeChkBox = inpElemsInParentTable[0];
                parentNodeChkBox.checked = check;
                CheckUncheckParents(parentNodeChkBox, check);
            }
        }

        function AreAllSiblingsChecked(chkBox) {
            var parentDiv = GetParentByTagName("div", chkBox);
            var childCount = parentDiv.childNodes.length;
            for (var i = 0; i < childCount; i++) {
                if (parentDiv.childNodes[i].nodeType == 1) //check if the child node is an element node
                {
                    if (parentDiv.childNodes[i].tagName.toLowerCase() == "table") {
                        var prevChkBox = parentDiv.childNodes[i].getElementsByTagName("input")[0];
                        //if any of sibling nodes are not checked, return false
                        if (!prevChkBox.checked) {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        function AreAllSiblingsNotChecked(chkBox) {
            var parentDiv = GetParentByTagName("div", chkBox);
            var childCount = parentDiv.childNodes.length;
            for (var i = 0; i < childCount; i++) {
                if (parentDiv.childNodes[i].nodeType == 1) //check if the child node is an element node
                {
                    if (parentDiv.childNodes[i].tagName.toLowerCase() == "table") {
                        var prevChkBox = parentDiv.childNodes[i].getElementsByTagName("input")[0];
                        //if any of sibling nodes are not checked, return false
                        if (prevChkBox.checked) {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        //utility function to get the container of an element by tagname
        function GetParentByTagName(parentTagName, childElementObj) {
            var parent = childElementObj.parentNode;
            while (parent.tagName.toLowerCase() != parentTagName.toLowerCase()) {
                parent = parent.parentNode;
            }
            return parent;
        }

        function TreeViewModule_oncheck() {
            var index = event.treeNodeIndex;
            var node = getNodeByIndex(TreeView1, index);
            var checked = eval(node.getAttribute('checked'));
            checkChildren(node, checked);
            checkParent(node);
        }

        function checkChildren(node, checked) {
            var children = node.getChildren();
            for (var i = 0; i < children.length; i++) {
                children[i].setAttribute('checked', checked);
                checkChildren(children[i], checked);
            }
        }

        function checkParent(node) {
            if (node == null) return;
            var parent = node.getParent();
            if (parent == null) return;

            var children = parent.getChildren();

            for (var i = 0; i < children.length; i++) {
                if (!eval(children[i].getAttribute('checked'))) {
                    break;
                }
            }
            parent.setAttribute('checked', i == children.length);
            checkParent(parent);
        }
    </script>
</asp:Content>
