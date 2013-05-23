using System;
using System.Web;
using System.Web.Mvc;
using Aaron.Core;
using Aaron.Core.Data;
using Aaron.Core.Infrastructure;
using Aaron.Core.Web;
using Aaron.Core.Services.Accounts;

namespace Aaron.Core.Web
{
    public class WebClosedAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext == null || filterContext.HttpContext == null)
                return;

            HttpRequestBase request = filterContext.HttpContext.Request;
            if (request == null)
                return;

            string actionName = filterContext.ActionDescriptor.ActionName;
            if (String.IsNullOrEmpty(actionName))
                return;

            string controllerName = filterContext.Controller.ToString();
            if (String.IsNullOrEmpty(controllerName))
                return;

            //don't apply filter to child methods
            if (filterContext.IsChildAction)
                return;

            if (!DataHelper.DatabaseIsExisted())
                return;

            var webInformationSettings = IoC.Resolve<WebInformationSettings>();

            string[] controllerNameArray = controllerName.Split('.');
            string controllerNameShort = controllerNameArray[controllerNameArray.Length - 1];

            if (webInformationSettings.WebClosed &&
                //ensure it's not the Login page
                !(controllerNameShort.Equals("AccountController", StringComparison.InvariantCultureIgnoreCase) && actionName.Equals("Login", StringComparison.InvariantCultureIgnoreCase)) &&
                //ensure it's not the Logout page
                !(controllerNameShort.Equals("AccountController", StringComparison.InvariantCultureIgnoreCase) && actionName.Equals("Logout", StringComparison.InvariantCultureIgnoreCase)))
            {
                if (webInformationSettings.WebClosedAllowForAdmins && IoC.Resolve<ICurrentActivity>().CurrentAccount.IsAdmin())
                {
                    //do nothing - allow admin access
                }
                else
                {
                    var webHelper = IoC.Resolve<IWebHelper>();
                    filterContext.Result = new RedirectResult(webHelper.GetWebLocation() + "WebClosed.htm");
                }
            }
        }
    }
}
