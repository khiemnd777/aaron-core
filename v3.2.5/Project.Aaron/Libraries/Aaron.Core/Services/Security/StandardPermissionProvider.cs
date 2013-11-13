using System.Collections.Generic;
using Aaron.Core.Domain.Accounts;
using Aaron.Core.Domain.Security;

namespace Aaron.Core.Services.Security
{
    public partial class StandardPermissionProvider : IPermissionProvider
    {
        //admin area permissions
        public static readonly PermissionRecord AccessAdminPanel = new PermissionRecord { Name = "Aaron.Permission.AccessAdminPanel", SystemName = "AccessAdminPanel", Category = "Standard" };
        public static readonly PermissionRecord ManageGenericCatalogs = new PermissionRecord { Name = "Aaron.Permission.ManageGenericCatalogs", SystemName = "ManageGenericCatalogs", Category = "Catalogs" };
        public static readonly PermissionRecord ManageCatalogs = new PermissionRecord { Name = "Aaron.Permission.ManageCatalogs", SystemName = "ManageCatalogs", Category = "Catalogs" };
        public static readonly PermissionRecord DisplayAccounts = new PermissionRecord { Name = "Aaron.Permission.DisplayAccounts", SystemName = "DisplayAccounts", Category = "Accounts" };
        public static readonly PermissionRecord ViewOnlineAccounts = new PermissionRecord { Name = "Aaron.Permission.ViewOnlineAccounts", SystemName = "ViewOnlineAccounts", Category = "Accounts" };
        public static readonly PermissionRecord ManageAccounts = new PermissionRecord { Name = "Aaron.Permission.ManageAccounts", SystemName = "ManageAccounts", Category = "Accounts" };
        public static readonly PermissionRecord EditAccounts = new PermissionRecord { Name = "Aaron.Permission.EditAccounts", SystemName = "EditAccounts", Category = "Accounts" };
        public static readonly PermissionRecord ManageAccountRoles = new PermissionRecord { Name = "Aaron.Permission.ManageAccountRoles", SystemName = "ManageAccountRoles", Category = "Accounts" };
        public static readonly PermissionRecord AllowRestartApp = new PermissionRecord { Name = "Aaron.Permission.AllowRestartApp", SystemName = "AllowRestartApp", Category = "Common" };
        public static readonly PermissionRecord ManageTemplates = new PermissionRecord { Name = "Aaron.Permission.ManageTemplates", SystemName = "ManageTemplates", Category = "Contents" };
        public static readonly PermissionRecord ManageSettings = new PermissionRecord { Name = "Aaron.Permission.ManageSettings", SystemName = "ManageSettings", Category = "Configuration" };
        public static readonly PermissionRecord ManageACL = new PermissionRecord { Name = "Aaron.Permission.ManageACL", SystemName = "ManageACL", Category = "Configuration" };
        public static readonly PermissionRecord ManageEmailAccounts = new PermissionRecord { Name = "Aaron.Permission.ManageEmailAccounts", SystemName = "ManageEmailAccounts", Category = "Configuration" };
        public static readonly PermissionRecord ManageMessageTemplates = new PermissionRecord { Name = "Aaron.Permission.ManageMessageTemplates", SystemName = "ManageMessageTemplates", 
Category = "Content" };
        public static readonly PermissionRecord ManageCountries = new PermissionRecord { Name = "Aaron.Permission.ManageCountries", SystemName = "ManageCountries", Category = "Configuration" };
        public static readonly PermissionRecord ManageCampaigns = new PermissionRecord { Name = "Aaron.Permission.ManageCampaigns", SystemName = "ManageCampaigns", Category = "Content" };
        public static readonly PermissionRecord ManageNotices = new PermissionRecord { Name = "Aaron.Permission.ManageNotices", SystemName = "ManageNotices", Category = "Content" };
        public static readonly PermissionRecord ManageWidgets = new PermissionRecord { Name = "Aaron.Permission.ManageWidgets", SystemName = "ManageWidgets", Category = "Content" };
        public static readonly PermissionRecord ManageExternalAuthenticationMethods = new PermissionRecord { Name = "Aaron.Permission.ManageExternalAuthenticationMethods", SystemName = "ManageExternalAuthenticationMethods", Category = "Configuration" };
        public static readonly PermissionRecord ManagePlugins = new PermissionRecord { Name = "Aaron.Permission.ManagePlugins", SystemName = "ManagePlugins", Category = "Configuration" };
        public static readonly PermissionRecord ManageSystemLog = new PermissionRecord { Name = "Aaron.Permission.ManageSystemLog", SystemName = "ManageSystemLog", Category = "Configuration" };
        public static readonly PermissionRecord ManageTopics = new PermissionRecord { Name = "Aaron.Permission.ManageTopics", SystemName = "ManageTopics", Category = "Content" };
        public static readonly PermissionRecord ManageLanguages = new PermissionRecord { Name = "Aaron.Permission.ManageLanguages", SystemName = "ManageLanguages", Category = "Configuration" };
        
        public virtual IEnumerable<PermissionRecord> GetPermissions()
        {
            return new[] 
            {
                AccessAdminPanel,
                ManageAccountRoles,
                ManageAccounts,
                ManageACL,
                ManageGenericCatalogs,
                ManageCatalogs,
                ManageSettings,
                ManageTemplates,
                DisplayAccounts,
                ViewOnlineAccounts,
                AllowRestartApp,
                EditAccounts,
                ManageEmailAccounts,
                ManageMessageTemplates,
                ManageCampaigns,
                ManageNotices,
                ManageWidgets,
                ManageExternalAuthenticationMethods,
                ManagePlugins,
                ManageSystemLog,
                ManageTopics,
                ManageLanguages
            };
        }

        public virtual IEnumerable<DefaultPermissionRecord> GetDefaultPermissions()
        {
            return new[] 
            {
                new DefaultPermissionRecord 
                {
                    AccountRoleSystemName = SystemAccountRoleNames.Administrators,
                    PermissionRecords = new[] 
                    {
                        AccessAdminPanel,
                        ManageAccountRoles,
                        ManageAccounts,
                        ManageACL,
                        ManageGenericCatalogs,
                        ManageCatalogs,
                        ManageSettings,
                        ManageTemplates,
                        DisplayAccounts,
                        ViewOnlineAccounts,
                        AllowRestartApp,
                        EditAccounts,
                        ManageEmailAccounts,
                        ManageMessageTemplates,
                        ManageCampaigns,
                        ManageNotices,
                        ManageWidgets,
                        ManageExternalAuthenticationMethods,
                        ManagePlugins,
                        ManageSystemLog,
                        ManageTopics,
                        ManageLanguages
                    }
                },
            };
        }
    }
}