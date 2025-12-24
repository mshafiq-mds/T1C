using CustomGuid.AspNet.Identity;
using FGV.Prodata.Web.UI;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Org.BouncyCastle.Asn1.Cmp;
using System;
using System.Web;
using System.Web.UI;

namespace Prodata.WebForm.Account
{
    public partial class Profile : ProdataPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadUserProfile();
            }
        }

        private void LoadUserProfile()
        {
            var userManager = Context.GetOwinContext().GetUserManager<UserManager>();
            var userId = Context.User.Identity.GetUserID();

            if (userId != null)
            {
                var user = userManager.FindById(userId);
                if (user != null)
                {
                    // View Mode Data
                    lblName.Text = user.Name;
                    lblEmail.Text = user.Email;

                    // Edit Mode Pre-fill
                    txtEditEmail.Text = user.Email;
                }
            }
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            // Hide View Panel, Show Edit Panel
            pnlView.Visible = false;
            pnlEdit.Visible = true;

            // Clear previous messages
            plhMessage.Visible = false;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            // Revert to View Panel
            pnlView.Visible = true;
            pnlEdit.Visible = false;
            plhMessage.Visible = false;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            var userManager = Context.GetOwinContext().GetUserManager<UserManager>();
            var user = userManager.FindById(Context.User.Identity.GetUserID());

            if (user != null)
            {
                try
                {
                    string newEmail = txtEditEmail.Text.Trim();

                    // Check if email actually changed
                    if (user.Email.Equals(newEmail, StringComparison.OrdinalIgnoreCase))
                    {
                        btnCancel_Click(sender, e);
                        return;
                    }

                    // Update the User Object
                    user.Email = newEmail;

                    IdentityResult result = userManager.Update(user);

                    if (result.Succeeded)
                    {
                        // 1. Reload profile data
                        LoadUserProfile();

                        // 2. Switch UI back to View Mode
                        pnlView.Visible = true;
                        pnlEdit.Visible = false;

                        // 3. Hide the Bootstrap alert placeholder (so it doesn't clash)
                        plhMessage.Visible = false;

                        // 4. Trigger SweetAlert Success
                        // This injects the JS command to run immediately after the page renders
                        string script = "Swal.fire('Saved!', 'Your profile has been updated successfully.', 'success');";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "SaveSuccess", script, true);
                    }
                    else
                    {
                        // Keep the Bootstrap alert for Errors (Cleaner for lists of errors)
                        ShowMessage(string.Join(", ", result.Errors), "danger");
                    }
                }
                catch (Exception ex)
                {
                    ShowMessage("An error occurred: " + ex.Message, "danger");
                }
            }
        }

        private void ShowMessage(string message, string type)
        {
            plhMessage.Visible = true;
            lblMessage.Text = message;
            // Apply bootstrap alert classes dynamically
            msgContainer.Attributes["class"] = $"alert alert-{type} alert-dismissible fade show";
        }
    }
}