using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using CustomGuid.AspNet.Identity;
using FGV.Prodata.App;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Prodata.WebForm.Models.System;
using Prodata.WebForm.Models.ViewModels;

namespace Prodata.WebForm
{
    public partial class SiteMaster : MasterPage
    {
        private const string AntiXsrfTokenKey = "__AntiXsrfToken";
        private const string AntiXsrfUserNameKey = "__AntiXsrfUserName";
        private string _antiXsrfTokenValue;
        private readonly Class.Module module = new Class.Module();

        protected void Page_Init(object sender, EventArgs e)
        {
            // The code below helps to protect against XSRF attacks
            var requestCookie = Request.Cookies[AntiXsrfTokenKey];
            Guid requestCookieGuidValue;
            if (requestCookie != null && Guid.TryParse(requestCookie.Value, out requestCookieGuidValue))
            {
                // Use the Anti-XSRF token from the cookie
                _antiXsrfTokenValue = requestCookie.Value;
                Page.ViewStateUserKey = _antiXsrfTokenValue;
            }
            else
            {
                // Generate a new Anti-XSRF token and save to the cookie
                _antiXsrfTokenValue = Guid.NewGuid().ToString("N");
                Page.ViewStateUserKey = _antiXsrfTokenValue;

                var responseCookie = new HttpCookie(AntiXsrfTokenKey)
                {
                    HttpOnly = true,
                    Value = _antiXsrfTokenValue
                };
                if (FormsAuthentication.RequireSSL && Request.IsSecureConnection)
                {
                    responseCookie.Secure = true;
                }
                Response.Cookies.Set(responseCookie);
            }

            Page.PreLoad += master_Page_PreLoad;
        }

        protected void master_Page_PreLoad(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Set Anti-XSRF token
                ViewState[AntiXsrfTokenKey] = Page.ViewStateUserKey;
                ViewState[AntiXsrfUserNameKey] = Context.User.Identity.Name ?? String.Empty;
            }
            else
            {
                // Validate the Anti-XSRF token
                if ((string)ViewState[AntiXsrfTokenKey] != _antiXsrfTokenValue
                    || (string)ViewState[AntiXsrfUserNameKey] != (Context.User.Identity.Name ?? String.Empty))
                {
                    throw new InvalidOperationException("Validation of Anti-XSRF token failed.");
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ltrlMenu.Text = RenderMenu();
            ltrlBreadcrumb.Text = !Request.Url.GetCurrentUrl().Equals("/") ? RenderBreadcrumb() : string.Empty;
            SetPageAccess();
        }

        protected void Unnamed_LoggingOut(object sender, LoginCancelEventArgs e)
        {
            Context.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
        }

        private string RenderMenu()
        {
            var menu = string.Empty;
            var modules = module.GetModules(Context.User.Identity.GetUserID(), null, true).ToList();

            for (int i = 0; i < modules.Count; i++)
            {
                menu += RenderMenuItem(modules[i]);
            }

            return menu;
        }

        private string RenderMenuItem(UserModuleListViewModel item, Guid? parentId = null)
        {
            var menuFormat = "<li class='nav-item {0} {1}'><a href='{2}' class='nav-link {3}'><i class='nav-icon {4}'></i><p>{5}</p>{6}</a>";
            var hasChildren = module.HasChildren(item.Id, true);
            var isCurrent = item.Name.Equals(Page.Title);
            var isParent = module.IsParent(item.Id, Page.Title);

            var menu = string.Format(
                menuFormat,
                hasChildren ? "has-treeview" : string.Empty,
                isParent ? "menu-open" : string.Empty,
                string.IsNullOrEmpty(item.Url) ? "#" : Page.ResolveUrl(item.Url),
                isCurrent || isParent ? "active" : string.Empty,
                string.IsNullOrEmpty(item.Icon) ? "fas fa-lock" : item.Icon,
                item.Name,
                hasChildren ? "<i class='right fas fa-angle-left'></i>" : string.Empty);

            if (hasChildren)
            {
                menu += "<ul class='nav nav-treeview'>";
                foreach (var children in module.GetModules(Context.User.Identity.GetUserID(), item.Id, true, true))
                    menu += RenderMenuItem(children, item.Id);
                menu += "</ul>";
            }

            menu += "</li>";
            return menu;
        }

        private string RenderBreadcrumb()
        {
            var format = "{0}{1}<li class='breadcrumb-item active'>{2}</li>";
            var parentFormat = "<li class='breadcrumb-item'><a href='{0}'>{1}</a></li>";

            string breadcrumb = string.Empty;

            // Assuming you have a way to get the current page title
            string currentPageTitle = Page.Title;

            // Define possible prefixes
            string[] prefixes = { "Add", "Edit", "Delete", "Approve", "Approval" };
            string breadcrumbName = currentPageTitle;

            // Check if the title starts with any of the prefixes
            foreach (var prefix in prefixes)
            {
                if (currentPageTitle.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    breadcrumbName = prefix; // Set breadcrumb name to the detected prefix
                    break;
                }
            }

            // Get the breadcrumb trail
            List<string> breadcrumbItems = new List<string>();
            string pageTitle = currentPageTitle;

            while (module.HasParent(pageTitle))
            {
                var parent = module.GetParent(pageTitle);
                string parentTitle = parent.Name;
                string parentUrl = Page.ResolveUrl(parent.Url);

                breadcrumbItems.Insert(0, string.Format(parentFormat, parentUrl, parentTitle));
                pageTitle = parentTitle;
            }

            // Combine breadcrumb items
            breadcrumb = string.Format(format, string.Join("", breadcrumbItems), "", breadcrumbName);

            return breadcrumb;
        }

        private void SetPageAccess()
        {
            string pageName = Page.Title;
            string pageUrl = Request.Url.GetCurrentUrl();
            bool hasAccess = Auth.User().IsSuperadmin();

            if (!Context.User.Identity.IsAuthenticated)
            {
                Response.Redirect("~/");
            }

            if (pageUrl.Equals("/") || pageUrl.Equals("/Account/Profile", StringComparison.OrdinalIgnoreCase) || pageUrl.Equals("/Error/404", StringComparison.OrdinalIgnoreCase))
            {
                hasAccess = true;
            }
            else
            {
                foreach (var access in Auth.UserManager().GetAccess(Auth.Id()))
                {
                    if (pageName.Equals("Edit Role", StringComparison.OrdinalIgnoreCase) || pageName.Equals("Edit User", StringComparison.OrdinalIgnoreCase))
                    {
                        pageName = pageName.Substring(5);
                    }
                    if (pageName.ToLower().Equals(access.ModuleName.ToLower()))
                    {
                        hasAccess = true;
                        break;
                    }
                }
            }

            if (!hasAccess)
            {
                Response.Redirect("~/Error/404");
            }
        }
    }
}