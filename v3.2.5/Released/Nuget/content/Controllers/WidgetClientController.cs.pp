﻿using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Aaron.Core.Services.Cms;
using $rootnamespace$.Models.Cms;

namespace $rootnamespace$.Controllers
{
    public partial class WidgetClientController : BaseController
    {
		#region Fields

        private readonly IWidgetService _widgetService;

        #endregion

		#region Constructors

        public WidgetClientController(IWidgetService widgetService)
        {
            this._widgetService = widgetService;
        }

        #endregion

        #region Methods

        [ChildActionOnly]
        public ActionResult WidgetsByZone(string widgetZone)
        {
            //model
            var model = new List<RenderWidgetModel>();

            var widgets = _widgetService.LoadActiveWidgetsByWidgetZone(widgetZone);
            foreach (var widget in widgets)
            {
                var widgetModel = new RenderWidgetModel();

                string actionName;
                string controllerName;
                RouteValueDictionary routeValues;
                widget.GetDisplayWidgetRoute(widgetZone, out actionName, out controllerName, out routeValues);
                widgetModel.ActionName = actionName;
                widgetModel.ControllerName = controllerName;
                widgetModel.RouteValues = routeValues;

                model.Add(widgetModel);
            }

            return PartialView(model);
        }

        #endregion
    }
}
