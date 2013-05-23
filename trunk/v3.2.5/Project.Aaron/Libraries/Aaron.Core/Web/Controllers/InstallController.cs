using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Configuration;
using Aaron.Core.Data;
using Aaron.Core.Web.Models.Install;
using System.Data.SqlClient;
using Aaron.Core.Infrastructure;
using Aaron.Core.Security;
using System.Security.Principal;
using System.Threading;
using Aaron.Core.Installation;
using Aaron.Core.Services.Accounts;
using Aaron.Core.SysConfiguration;
using Aaron.Core.Plugins;

namespace Aaron.Core.Web.Controllers
{
    public class InstallController : Controller
    {
        private string[] RedirectToActionAfterInstall
        {
            get
            {
                var settingCa = ConfigurationManager.AppSettings["redirectToActionAfterInstall"] ?? "Home/Index";
                return settingCa.Split('/');
            }
        }

        public ActionResult Index()
        {
            if (DataHelper.TableIsExisted()) return RedirectToAction(RedirectToActionAfterInstall[1], RedirectToActionAfterInstall[0]);

            // set page timeout to 5 minutes
            this.Server.ScriptTimeout = 300;

            var model = new InstallModel()
            {
                AdminEmail = "admin@cdnvn.com",
                SqlServerName = @".\SQLExpress",
                DatabaseConnectionString = "",
                DataProvider = "sqlserver",
                SqlAuthenticationType = "sqlauthentication",
                SqlServerCreateDatabase = false,
                UseCustomCollation = false,
                Collation = "SQL_Latin1_General_CP1_CI_AS",
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(InstallModel model)
        {
            if (DataHelper.TableIsExisted()) return RedirectToAction(RedirectToActionAfterInstall[1], RedirectToActionAfterInstall[0]);

            if (model.DatabaseConnectionString != null)
                model.DatabaseConnectionString = model.DatabaseConnectionString.Trim();
            //try
            //{
                var webHelper = IoC.Resolve<IWebHelper>();
                if (!DataHelper.HasSettingsFileOrNotNull()) // check Settings.txt file is existed.
                {
                    if (model.DataProvider.Equals("sqlserver", StringComparison.InvariantCultureIgnoreCase))
                    {
                        //values
                        if (string.IsNullOrEmpty(model.SqlServerName))
                            ModelState.AddModelError("", "Server name is required!");
                        if (string.IsNullOrEmpty(model.SqlDatabaseName))
                            ModelState.AddModelError("", "Database name is required!");

                        //authentication type
                        if (model.SqlAuthenticationType.Equals("sqlauthentication", StringComparison.InvariantCultureIgnoreCase))
                        {
                            //SQL authentication
                            if (string.IsNullOrEmpty(model.SqlServerUsername))
                                ModelState.AddModelError("", "Sqlserver Username is required!");
                            if (string.IsNullOrEmpty(model.SqlServerPassword))
                                ModelState.AddModelError("", "Sqlserver Password is required!");
                            if(model.SqlServerPassword.Trim().Equals(model.ConfirmPassword))
                                ModelState.AddModelError("", "Sqlserver Password is not duplicated!");
                        }
                    }

                    //validate permissions
                    var dirsToCheck = FilePermissionHelper.GetDirectoriesWrite(webHelper);
                    foreach (string dir in dirsToCheck)
                        if (!FilePermissionHelper.CheckPermissions(dir, false, true, true, false))
                            ModelState.AddModelError("", string.Format("{0} cannot be writable for {1}", WindowsIdentity.GetCurrent().Name, dir));

                    var filesToCheck = FilePermissionHelper.GetFilesWrite(webHelper);
                    foreach (string file in filesToCheck)
                        if (!FilePermissionHelper.CheckPermissions(file, false, true, true, true))
                            ModelState.AddModelError("", string.Format("{0} cannot be writable for {1}", WindowsIdentity.GetCurrent().Name, file));

                    if (ModelState.IsValid)
                    {
                        DataHelper.SaveSettings("SqlServer",
                            model.SqlAuthenticationType == "windowsauthentication",
                            model.SqlServerName,
                            model.SqlDatabaseName,
                            model.SqlServerUsername,
                            model.SqlServerPassword);
                    }
                }
                if (DataHelper.HasSettingsFileOrNotNull())
                {
                    if (model.SqlServerCreateDatabase)
                    {
                        var collation = model.UseCustomCollation ? model.Collation : "";
                        DataHelper.CreateDatabase(collation);
                    }
                    else
                    {
                        if (!DataHelper.DatabaseIsExisted())
                            throw new Exception("Database is not existed!");
                    }

                    // init tables or instace db.
                    var dataProviderInstance = IoC.Resolve<IDataProvider>();
                    dataProviderInstance.InitDatabase();
                    
                    // install all providers
                    InstallationManager.Install();

                    // install default admin
                    var installAccountFirstService = IoC.Resolve<IInstallAccountFirstService>();
                    installAccountFirstService.Install(model.AdminEmail, model.AdminPassword);

                    DataHelper.ResetCache();

                    //install plugins
                    PluginManager.MarkAllPluginsAsUninstalled();
                    var pluginFinder = IoC.Resolve<IPluginFinder>();
                    var plugins = pluginFinder.GetPlugins<IPlugin>(false)
                        .ToList()
                        .OrderBy(x => x.PluginDescriptor.Group)
                        .ThenBy(x => x.PluginDescriptor.DisplayOrder)
                        .ToList();
                    foreach (var plugin in plugins)
                    {
                        plugin.Install();
                    }

                    webHelper.RestartAppDomain(true, "~/install/success");
                }
            //}
            //catch (Exception ex)
            //{
            //    ViewBag.ErrorMessage = ex.Message;
            //    DataHelper.RemoveSettingsFile();
            //    DataHelper.ResetCache();
            //}
            return View(model);
        }

        public ActionResult Success()
        {
            var tableExisted = DataHelper.TableIsExisted();
            if (tableExisted)
            {
                var settingProvider = IoC.Resolve<ISysConfigurationProvider<InstallSuccessPageSettings>>();
                var settings = settingProvider.Settings;
                var visited = settings.Visited;
                if (visited) 
                    return RedirectToAction(RedirectToActionAfterInstall[1], RedirectToActionAfterInstall[0]);
                else
                    settingProvider.SaveSettings(new InstallSuccessPageSettings()
                    {
                        Visited = true
                    });
                return View();
            }
            return RedirectToAction("Index", "Install");
        }
    }
}
