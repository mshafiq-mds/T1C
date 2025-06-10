using Microsoft.AspNet.Identity;
using Prodata.WebForm.Models.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using CustomGuid.AspNet.Identity;
using Prodata.WebForm.Models.ViewModels;

namespace Prodata.WebForm
{
	public partial class Iframe : MasterPage
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
            var menuFormat = "<li class='nav-item {0}'><a href='{1}' class='nav-link'><i class='nav-icon {2}'></i><p>{3}</p>{4}</a>";
            var hasChildren = module.HasChildren(item.Id, true);

            var menu = string.Format(
                menuFormat,
                hasChildren ? "has-treeview" : string.Empty,
                hasChildren || string.IsNullOrEmpty(item.Url) ? "#" : Page.ResolveUrl(item.Url),
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
    }
}