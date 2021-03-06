﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aaron.Core.Web;
using Aaron.Core.Infrastructure;
using $rootnamespace$.Models.Common;

namespace $rootnamespace$.Controllers
{
    [WebClosed]
    public abstract class BaseController : Controller
    {
        // Write more here as Controllers or Methods.

	#region Feature Notification
        public ActionResult UpgradeNotification(string featureName)
        {
            var webHelper = IoC.Resolve<IWebHelper>();
            var model = new FeatureNotificationModel
            {
                BackUrl = webHelper.GetWebLocation(),
                Name = featureName,
                Type = FeatureNotificationType.Upgrade
            };
            return PartialView("_FeatureNotification", model); //RedirectToAction("Notification", "Common", new { model = model });
        }

        public ActionResult ConstructionNotification(string featureName)
        {
            var webHelper = IoC.Resolve<IWebHelper>();
            var model = new FeatureNotificationModel
            {
                BackUrl = webHelper.GetWebLocation(),
                Name = featureName,
                Type = FeatureNotificationType.Construction
            };
            return PartialView("_FeatureNotification", model); //RedirectToAction("Notification", "Common", new { model = model });
        } 
        #endregion
    }
}
