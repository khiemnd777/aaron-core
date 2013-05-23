using System.Collections.Generic;
using Aaron.Core.Domain.Accounts;
using Aaron.Core.Domain.Security;

namespace Aaron.Core.Services.Security
{
    public partial class StandardPermissionProvider : IPermissionProvider
    {
        //admin area permissions
        public static readonly PermissionRecord AccessAdminPanel = new PermissionRecord { Name = "Access admin area", SystemName = "AccessAdminPanel", Category = "Standard" };
        public static readonly PermissionRecord ManageGenericCatalogs = new PermissionRecord { Name = "Quản lý danh mục động", SystemName = "ManageGenericCatalogs", Category = "Catalogs" };
        public static readonly PermissionRecord ManageCatalogs = new PermissionRecord { Name = "Quản lý danh mục", SystemName = "ManageCatalogs", Category = "Catalogs" };
        public static readonly PermissionRecord DisplayAccounts = new PermissionRecord { Name = "Xem tài khoản", SystemName = "DisplayAccounts", Category = "Accounts" };
        public static readonly PermissionRecord ViewOnlineAccounts = new PermissionRecord { Name = "Xem tài khoản đang online", SystemName = "ViewOnlineAccounts", Category = "Accounts" };
        public static readonly PermissionRecord ManageAccounts = new PermissionRecord { Name = "Quuản lý tài khoản", SystemName = "ManageAccounts", Category = "Accounts" };
        public static readonly PermissionRecord EditAccounts = new PermissionRecord { Name = "Chỉnh sửa tài khoản", SystemName = "EditAccounts", Category = "Accounts" };
        public static readonly PermissionRecord ManageAccountRoles = new PermissionRecord { Name = "Quản lý phân quyền", SystemName = "ManageAccountRoles", Category = "Accounts" };
        public static readonly PermissionRecord AllowRestartApp = new PermissionRecord { Name = "Sử dụng tính năng 'Khởi động hệ thống'", SystemName = "AllowRestartApp", Category = "Common" };
        public static readonly PermissionRecord ManageTemplates = new PermissionRecord { Name = "Quản lý Templates", SystemName = "ManageTemplates", Category = "Contents" };
        public static readonly PermissionRecord ManageSettings = new PermissionRecord { Name = "Quản lý thiết lập", SystemName = "ManageSettings", Category = "Configuration" };
        public static readonly PermissionRecord ManageACL = new PermissionRecord { Name = "Quản lý Quyền truy xuất", SystemName = "ManageACL", Category = "Configuration" };
        public static readonly PermissionRecord ManageEmailAccounts = new PermissionRecord { Name = "Quản lý Tài khoản Email", SystemName = "ManageEmailAccounts", Category = "Configuration" };
        public static readonly PermissionRecord ManageMessageTemplates = new PermissionRecord { Name = "Quản lý Mẫu tin nhắn", SystemName = "ManageMessageTemplates", Category = "Content" };
        public static readonly PermissionRecord ManageCampaigns = new PermissionRecord { Name = "Quản lý Thư giới thiệu", SystemName = "ManageCampaigns", Category = "Content" };
        public static readonly PermissionRecord ManageNotices = new PermissionRecord { Name = "Quản lý Thông báo", SystemName = "ManageNotices", Category = "Content" };
        public static readonly PermissionRecord ManageWidgets = new PermissionRecord { Name = "Quản lý Widget", SystemName = "ManageWidgets", Category = "Content" };
        public static readonly PermissionRecord ManageExternalAuthenticationMethods = new PermissionRecord { Name = "Quản lý phương thức xác thực mở rộng", SystemName = "ManageExternalAuthenticationMethods", Category = "Configuration" };
        public static readonly PermissionRecord ManagePlugins = new PermissionRecord { Name = "Quản lý Plugins", SystemName = "ManagePlugins", Category = "Configuration" };
        public static readonly PermissionRecord ManageSystemLog = new PermissionRecord { Name = "Nhật ký hệ thống", SystemName = "ManageSystemLog", Category = "Configuration" };
        public static readonly PermissionRecord ManageTopics = new PermissionRecord { Name = "Quản lý chủ đề", SystemName = "ManageTopics", Category = "Content" };
        public static readonly PermissionRecord ManageLanguages = new PermissionRecord { Name = "Quản lý ngôn ngữ", SystemName = "ManageLanguages", Category = "Configuration" };
        
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