using FGV.Prodata.Web.UI;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Prodata.WebForm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Prodata.WebForm.Administration.Role
{
	public partial class Manage : ProdataPage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
            if (!IsPostBack)
            {
                string roleId = string.Empty;

                if (Request.QueryString["Action"] != null)
                {
                    if (Request.QueryString["Action"].ToString().ToLower().Equals("edit"))
                    {
                        Page.Title = "Edit Role";
                        if (Request.QueryString["Id"] != null)
                        {
                            btnSave.Visible = Auth.User().Can("admin-role-edit");
                            roleId = Request.QueryString["Id"].ToString();
                            hdnRoleId.Value = roleId;
                        }
                        else
                        {
                            Response.Redirect("/Administration/Role");
                        }
                    }
                    else if (Request.QueryString["Action"].ToString().ToLower().Equals("add"))
                    {
                        Page.Title = "Add Role";
                        if (!Auth.User().Can("admin-role-add"))
                        {
                            Response.Redirect("/Administration/Role");
                        }
                    }
                    else
                    {
                        Response.Redirect("/Administration/Role");
                    }
                }
                else
                {
                    Response.Redirect("/Administration/Role");
                }

                BindPermissionTree();
                BindData(roleId);
            }
            else
            {
                if (Request.QueryString["Action"] != null)
                {
                    if (Request.QueryString["Action"].ToString().ToLower().Equals("edit"))
                    {
                        if (Request.QueryString["Id"] != null)
                        {
                            btnSave.Visible = Auth.User().Can("admin-role-edit");
                        }
                    }
                }
            }
		}

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (IsValid)
            {
                var roleManager = Context.GetOwinContext().Get<RoleManager>();

                bool isSuccess = false;
                string roleId = string.Empty;
                string roleName = string.Empty;

                if (!string.IsNullOrEmpty(hdnRoleId.Value))
                {
                    #region Edit role
                    roleId = hdnRoleId.Value;
                    roleName = txtRoleName.Text.Trim();
                    var guidRoleId = Guid.Parse(roleId);

                    if (!roleManager.RoleExists(guidRoleId, roleName))
                    {
                        using (var db = new AppDbContext())  // Single instance
                        {
                            using (var trans = db.Database.BeginTransaction())  // Transaction within the same context
                            {
                                try
                                {
                                    var role = roleManager.FindById(guidRoleId);
                                    role.Name = roleName;

                                    var result = roleManager.Update(role);
                                    if (result.Succeeded)
                                    {
                                        var existingRoleModules = db.RoleModules
                                                                    .Where(p => p.RoleId == role.Id)
                                                                    .ToDictionary(p => p.ModuleId, p => p);

                                        List<Models.Auth.RoleModule> modulesToAdd = new List<Models.Auth.RoleModule>();
                                        List<Models.Auth.RoleModule> modulesToRemove = new List<Models.Auth.RoleModule>();

                                        void ProcessNodes(TreeNode node)
                                        {
                                            Guid moduleId = Guid.Parse(node.Value);

                                            if (node.Checked)
                                            {
                                                if (!existingRoleModules.ContainsKey(moduleId))
                                                {
                                                    modulesToAdd.Add(new Models.Auth.RoleModule
                                                    {
                                                        RoleId = role.Id,
                                                        ModuleId = moduleId
                                                    });
                                                }
                                            }
                                            else
                                            {
                                                if (existingRoleModules.ContainsKey(moduleId))
                                                {
                                                    modulesToRemove.Add(existingRoleModules[moduleId]);
                                                }
                                            }

                                            foreach (TreeNode childNode in node.ChildNodes)
                                            {
                                                ProcessNodes(childNode);
                                            }
                                        }

                                        foreach (TreeNode node in permission.Nodes)
                                        {
                                            ProcessNodes(node);
                                        }

                                        if (modulesToAdd.Any())
                                        {
                                            db.RoleModules.AddRange(modulesToAdd);
                                        }

                                        if (modulesToRemove.Any())
                                        {
                                            db.RoleModules.RemoveRange(modulesToRemove);
                                        }

                                        db.SaveChanges();  // Save changes inside transaction
                                        trans.Commit();    // Commit only if everything succeeds
                                        isSuccess = true;
                                    }
                                    else
                                    {
                                        lblRoleErrors.Text = string.Join(" ", result.Errors);
                                        lblRoleErrors.Visible = true;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    trans.Rollback();
                                    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, string.Join("\n", ex.Message));
                                    Response.Redirect("/Administration/Role/Manage?Action=Edit&Id=" + roleId);
                                }
                            }
                        }
                    }
                    else
                    {
                        SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Role already exists.");
                        Response.Redirect("/Administration/Role/Manage?Action=Edit&Id=" + roleId);
                    }

                    if (isSuccess)
                    {
                        SweetAlert.SetAlert(SweetAlert.SweetAlertType.Success, "Role updated.");
                        Response.Redirect("/Administration/Role");
                    }
                    #endregion
                }
                else
                {
                    #region Add role
                    roleName = txtRoleName.Text.Trim();

                    if (!roleManager.RoleExists(roleName))
                    {
                        using (var db = new AppDbContext()) // Single instance
                        {
                            using (var trans = db.Database.BeginTransaction()) // Transaction within the same context
                            {
                                try
                                {
                                    var role = new Models.Auth.Role { Name = roleName };
                                    var result = roleManager.Create(role);

                                    if (result.Succeeded)
                                    {
                                        List<Models.Auth.RoleModule> roleModules = new List<Models.Auth.RoleModule>();

                                        void AddCheckedNodes(TreeNode node)
                                        {
                                            if (node.Checked)
                                            {
                                                roleModules.Add(new Models.Auth.RoleModule
                                                {
                                                    RoleId = role.Id,
                                                    ModuleId = Guid.Parse(node.Value)
                                                });
                                            }

                                            foreach (TreeNode childNode in node.ChildNodes)
                                            {
                                                AddCheckedNodes(childNode);
                                            }
                                        }

                                        // Collect checked nodes recursively
                                        foreach (TreeNode node in permission.Nodes)
                                        {
                                            AddCheckedNodes(node);
                                        }

                                        // Bulk insert to improve performance
                                        if (roleModules.Any())
                                        {
                                            db.RoleModules.AddRange(roleModules);
                                            db.SaveChanges();
                                        }

                                        trans.Commit();
                                        isSuccess = true;
                                    }
                                    else
                                    {
                                        lblRoleErrors.Text = string.Join(" ", result.Errors);
                                        lblRoleErrors.Visible = true;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    trans.Rollback();
                                    SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, string.Join("\n", ex.Message));
                                    Response.Redirect("/Administration/Role/Manage?Action=Add");
                                }
                            }
                        }
                    }
                    else
                    {
                        SweetAlert.SetAlert(SweetAlert.SweetAlertType.Error, "Role already exists.");
                        Response.Redirect("/Administration/Role/Manage?Action=Add");
                    }

                    if (isSuccess)
                    {
                        SweetAlert.SetAlert(SweetAlert.SweetAlertType.Success, "New role added.");
                        Response.Redirect("/Administration/Role");
                    }
                    #endregion
                }
            }
        }

		private void BindPermissionTree()
		{
			var module = new Class.Module();

            permission.Attributes.Add("oncheck", "TreeViewModule_oncheck();");
            permission.Attributes.Add("onclick", "OnTreeClick(event)");
            permission.Nodes.Clear();

            void AddChildNodes(TreeNode parentNode, Guid parentId)
            {
                foreach (var child in module.GetModules(parentId, true))
                {
                    TreeNode childNode = new TreeNode
                    {
                        Text = " " + child.Name,
                        Value = child.Id.ToString(),
                        SelectAction = TreeNodeSelectAction.None
                    };
                    parentNode.ChildNodes.Add(childNode);

                    // Recursively add child nodes
                    AddChildNodes(childNode, child.Id);
                }
            }

            // Create root nodes
            foreach (var m in module.GetModules(null, true))
            {
                TreeNode rootNode = new TreeNode
                {
                    Text = " " + m.Name,
                    Value = m.Id.ToString(),
                    SelectAction = TreeNodeSelectAction.None
                };
                permission.Nodes.Add(rootNode);

                // Recursively add child nodes
                AddChildNodes(rootNode, m.Id);
            }
        }

        private void BindData(string roleId = null)
        {
            var roleManager = Context.GetOwinContext().Get<RoleManager>();

            if (!string.IsNullOrEmpty(roleId))
            {
                var guidRoleId = Guid.Parse(roleId);
                var role = roleManager.FindById(guidRoleId);
                txtRoleName.Text = role.Name;

                using (var db = new AppDbContext())
                {
                    var permissions = db.RoleModules
                                        .Where(p => p.RoleId == guidRoleId)
                                        .Select(p => p.ModuleId.ToString())
                                        .ToHashSet(); // Use HashSet for faster lookups

                    void CheckNodesRecursively(TreeNode node)
                    {
                        if (permissions.Contains(node.Value))
                        {
                            node.Checked = true;
                        }
                        foreach (TreeNode childNode in node.ChildNodes)
                        {
                            CheckNodesRecursively(childNode);
                        }
                    }

                    foreach (TreeNode node in permission.Nodes)
                    {
                        CheckNodesRecursively(node);
                    }
                }
            }
        }
    }
}