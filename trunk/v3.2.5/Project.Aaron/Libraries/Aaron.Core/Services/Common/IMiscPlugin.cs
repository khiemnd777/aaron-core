﻿using System.Web.Routing;
using Aaron.Core.Plugins;

namespace Aaron.Core.Services.Common
{
    /// <summary>
    /// Misc plugin interface. 
    /// It's used by the plugins that have a configuration page.
    /// </summary>
    public partial interface IMiscPlugin : IPlugin
    {
        /// <summary>
        /// Gets a route for plugin configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues);
    }
}