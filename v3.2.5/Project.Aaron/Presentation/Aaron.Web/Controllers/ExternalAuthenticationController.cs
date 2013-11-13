﻿using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Aaron.Core.Services.Authentication.External;
using Aaron.Web.Models.Accounts;

namespace Aaron.Web.Controllers
{

    public partial class ExternalAuthenticationController : BaseController
    {
		#region Fields

        private readonly IOpenAuthenticationService _openAuthenticationService;

        #endregion

		#region Constructors

        public ExternalAuthenticationController(IOpenAuthenticationService openAuthenticationService)
        {
            this._openAuthenticationService = openAuthenticationService;
        }

        #endregion

        #region Methods

        public RedirectResult RemoveParameterAssociation(string returnUrl)
        {
            ExternalAuthorizerHelper.RemoveParameters();
            return Redirect(returnUrl);
        }

        [ChildActionOnly]
        public ActionResult ExternalMethods()
        {
            //model
            var model = new List<ExternalAuthenticationMethodModel>();

            var externalAuthenticationMethods = _openAuthenticationService.LoadActiveExternalAuthenticationMethods();
            foreach (var eam in externalAuthenticationMethods)
            {
                var eamModel = new ExternalAuthenticationMethodModel();

                string actionName;
                string controllerName;
                RouteValueDictionary routeValues;
                eam.GetPublicInfoRoute(out actionName, out controllerName, out routeValues);
                eamModel.ActionName = actionName;
                eamModel.ControllerName = controllerName;
                eamModel.RouteValues = routeValues;

                model.Add(eamModel);
            }

            return PartialView(model);
        }

        #endregion
    }
}