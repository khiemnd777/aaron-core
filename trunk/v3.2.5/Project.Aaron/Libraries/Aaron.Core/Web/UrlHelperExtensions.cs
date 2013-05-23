using System.Web.Mvc;

namespace Aaron.Core.Web
{
    public static class UrlHelperExtensions
    {
        public static string LogOn(this UrlHelper urlHelper, string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl))
                return urlHelper.Action("Login", "Account", new { ReturnUrl = returnUrl });
            return urlHelper.Action("Login", "Account");
        }

        public static string LogOff(this UrlHelper urlHelper, string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl))
                return urlHelper.Action("Logout", "Account", new { ReturnUrl = returnUrl });
            return urlHelper.Action("Logout", "Account");
        }
    }
}
