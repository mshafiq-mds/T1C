using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Owin;
using Prodata.WebForm.Models;
using Prodata.WebForm.Models.Auth;

namespace Prodata.WebForm.Account
{
    public partial class Login : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Page.Title = "Login";

            //RegisterHyperLink.NavigateUrl = "Register";
            // Enable this once you have account confirmation enabled for password reset functionality
            //ForgotPasswordHyperLink.NavigateUrl = "Forgot";
            //OpenAuthLogin.ReturnUrl = Request.QueryString["ReturnUrl"];
            var returnUrl = HttpUtility.UrlEncode(Request.QueryString["ReturnUrl"]);
            //if (!String.IsNullOrEmpty(returnUrl))
            //{
            //    RegisterHyperLink.NavigateUrl += "?ReturnUrl=" + returnUrl;
            //}
        }

        protected void LogIn(object sender, EventArgs e)
        {
            if (IsValid)
            {
                // Validate the user password
                var manager = Context.GetOwinContext().GetUserManager<UserManager>();
                var signinManager = Context.GetOwinContext().GetUserManager<SignInManager>();
                var user = new User();

                string username = Username.Text.Trim();
                string password = Encryptor2.Encode(Password.Text.Trim());
                bool userExistsInIPMS = false; // Flag to track if user exists

                if (username.Equals("superadmin", StringComparison.OrdinalIgnoreCase))
                {
                    // This doen't count login failures towards account lockout
                    // To enable password failures to trigger lockout, change to shouldLockout: true
                    var result = signinManager.PasswordSignIn(Username.Text, Password.Text, RememberMe.Checked, shouldLockout: false);

                    switch (result)
                    {
                        case SignInStatus.Success:
                            IdentityHelper.RedirectToReturnUrl(Request.QueryString["ReturnUrl"], Response);
                            break;
                        case SignInStatus.LockedOut:
                            Response.Redirect("/Account/Lockout");
                            break;
                        case SignInStatus.RequiresVerification:
                            Response.Redirect(String.Format("/Account/TwoFactorAuthenticationSignIn?ReturnUrl={0}&RememberMe={1}",
                                                            Request.QueryString["ReturnUrl"],
                                                            RememberMe.Checked),
                                              true);
                            break;
                        case SignInStatus.Failure:
                        default:
                            FailureText.Text = "Invalid login attempt";
                            ErrorMessage.Visible = true;
                            break;
                    }
                }
                else
                {
                    // Login from iPMS database
                    using (var con = new SqlConn().GetSqlConnection("iPMSConnection"))
                    {
                        con.Open();
                        string sql = "execute SP_ValidateUserLogin @userName, @password;";
                        SqlCommand cmd = new SqlCommand(sql, con);
                        cmd.Parameters.AddWithValue("@userName", username);
                        cmd.Parameters.AddWithValue("@password", password);

                        using (SqlDataReader rd = cmd.ExecuteReader())
                        {
                            if (rd.Read()) // If user is found in iPMS
                            {
                                userExistsInIPMS = true;
                                user = new User
                                {
                                    Name = Convert.ToString(rd["FullName"]),
                                    UserName = Convert.ToString(rd["UserName"]),
                                    Email = Convert.ToString(rd["UserName"]) + "@example.com",
                                    iPMSRoleCode = Convert.ToString(rd["RoleCode"]),
                                    iPMSBizAreaCode = Convert.ToString(rd["BizAreaCode"]),
                                };
                            }
                        }
                    }

                    // **If user does not exist in iPMS, return an error**
                    if (!userExistsInIPMS)
                    {
                        FailureText.Text = "Invalid login attempt";
                        ErrorMessage.Visible = true;
                        return; // Stop execution
                    }

                    // **Check if User Exists in t1c**
                    var existingUser = manager.FindByName(user.UserName);
                    if (existingUser == null) // User doesn't exist, create in t1c
                    {
                        var result = manager.Create(user);
                        if (result.Succeeded)
                        {
                            var createdUser = manager.FindByName(user.UserName);

                            // Assign role to the user based on iPMSRoleCode
                            if (!string.IsNullOrEmpty(createdUser.iPMSRoleCode))
                            {
                                var roleCode = createdUser.iPMSRoleCode.ToUpperInvariant();

                                if (roleCode == "ADMIN")
                                {
                                    if (!manager.IsInRole(createdUser.Id, "HQ"))
                                    {
                                        manager.AddToRole(createdUser.Id, "HQ");
                                    }
                                }
                                else
                                {
                                    var approverRoles = new HashSet<string>
                                    {
                                        "AM", "MM", "DRC", "RC", "KZ", "HPMBP", "UKK", "VP", "CEO"
                                    };

                                    if (approverRoles.Contains(roleCode))
                                    {
                                        if (!manager.IsInRole(createdUser.Id, "Approver"))
                                        {
                                            manager.AddToRole(createdUser.Id, "Approver");
                                        }

                                        if (roleCode == "KB" || roleCode == "MM")
                                        {
                                            if (!manager.IsInRole(createdUser.Id, "Kilang"))
                                            {
                                                manager.AddToRole(createdUser.Id, "Kilang");
                                            }
                                        }
                                    }
                                }
                            }

                            // **Manually login user**
                            signinManager.SignIn(createdUser, RememberMe.Checked, false);
                        }
                        else
                        {
                            FailureText.Text = "User creation failed.";
                            ErrorMessage.Visible = true;
                        }
                    }
                    else
                    {
                        // **User already exists, update their Name and iPMSRoleCode from iPMS**
                        existingUser.Name = user.Name;
                        existingUser.iPMSRoleCode = user.iPMSRoleCode;
                        existingUser.iPMSBizAreaCode = user.iPMSBizAreaCode;

                        var updateResult = manager.Update(existingUser);
                        if (updateResult.Succeeded)
                        {
                            // Assign role to existing user based on iPMSRoleCode
                            if (!string.IsNullOrEmpty(existingUser.iPMSRoleCode))
                            {
                                var roleCode = existingUser.iPMSRoleCode.ToUpperInvariant();

                                if (roleCode == "ADMIN")
                                {
                                    if (!manager.IsInRole(existingUser.Id, "HQ"))
                                    {
                                        manager.AddToRole(existingUser.Id, "HQ");
                                    }
                                }
                                else
                                {
                                    var approverRoles = new HashSet<string>
                                    {
                                        "AM", "MM", "DRC", "RC", "KZ", "HPMBP", "UKK", "VP", "CEO"
                                    };

                                    if (approverRoles.Contains(roleCode))
                                    {
                                        if (!manager.IsInRole(existingUser.Id, "Approver"))
                                        {
                                            manager.AddToRole(existingUser.Id, "Approver");
                                        }

                                        if (roleCode == "KB" || roleCode == "MM")
                                        {
                                            if (!manager.IsInRole(existingUser.Id, "Kilang"))
                                            {
                                                manager.AddToRole(existingUser.Id, "Kilang");
                                            }
                                        }
                                    }
                                }
                            }

                            // **Then sign in the user**
                            signinManager.SignIn(existingUser, RememberMe.Checked, false);
                        }
                        else
                        {
                            FailureText.Text = "Failed to update user info.";
                            ErrorMessage.Visible = true;
                            return;
                        }
                    }
                }

                // Manually login user
                //var user = manager.FindByName(Username.Text.Trim());
                //signinManager.SignIn(user, RememberMe.Checked, false);
            }
        }
    }
}