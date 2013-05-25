using System.Linq;
using System.Collections.Generic;
using Aaron.Core.Infrastructure;
using Aaron.Core.Data;
using Aaron.Core.Domain.Accounts;
using Aaron.Core.SysConfiguration;
using Aaron.Core.Domain.Messages;
using Aaron.Core.Domain.Localization;
using Aaron.Core.Domain.Security;
using Aaron.Core.Domain.Utilities;
using Aaron.Core.Domain.Tasks;
using Aaron.Core.Web;
using System.IO;
using Aaron.Core.Services.Localization;
using System.Text;
using Aaron.Core.Domain.Common;

namespace Aaron.Core.Installation
{
    public class StandardInstallationProvider : IInstallationProvider
    {
        private readonly IRepository<AccountRole> _accountRoleRepository;
        private readonly IRepository<EmailAccount> _emailAccountRepository;
        private readonly IRepository<Language> _languageRepository;
        private readonly IRepository<ScheduleTask> _scheduleTaskRepository;
        private readonly IRepository<MessageTemplate> _messageTemplateRepository;
        private readonly IWebHelper _webHelper;

        public StandardInstallationProvider(IRepository<AccountRole> accountRoleRepository,
            IRepository<EmailAccount> emailAccountRepository,
            IRepository<Language> languageRepository,
            IRepository<ScheduleTask> scheduleTaskRepository,
            IRepository<MessageTemplate> messageTemplateRepository,
            IWebHelper webHelper)
        {
            _accountRoleRepository = accountRoleRepository;
            _emailAccountRepository = emailAccountRepository;
            _languageRepository = languageRepository;
            _scheduleTaskRepository = scheduleTaskRepository;
            _webHelper = webHelper;
            _messageTemplateRepository = messageTemplateRepository;
        }

        private void InstallEmailAccounts()
        {
            _emailAccountRepository.Insert(new EmailAccount()
            {
                Email = "info@domain.com",
                DisplayName = "General contact",
                Host = "smtp.mail.com",
                Port = 25,
                Username = "username",
                Password = "password",
                EnableSsl = false,
                UseDefaultCredentials = false
            });
        }

        private void InstallLanguages()
        {
            var language = new Language
            {
                Name = "Vietnamese",
                LanguageCulture = "vi-VN",
                UniqueSeoCode = "vi",
                FlagImageFileName = "vn.png",
                Published = true,
                DisplayOrder = 1
            };
            _languageRepository.Insert(language);
        }

        protected virtual void InstallLocaleResources()
        {
            //'Vietnamese' language
            var language = _languageRepository.Table.Where(l => l.Name == "Vietnamese").Single();

            //save resoureces
            foreach (var filePath in System.IO.Directory.EnumerateFiles(_webHelper.MapPath("~/App_Data/Localization/"), "*.aaron.xml", SearchOption.TopDirectoryOnly))
            {
                //now we have a parsed XML file (the same structure as exported language packs)
                //let's save resources
                var localizationService = IoC.Resolve<ILocalizationService>();
                using (var sr = new StreamReader(filePath, Encoding.UTF8))
                {
                    localizationService.ImportResourcesFromXml(language, sr.ReadToEnd());
                }
            }
        }

        protected virtual void InstallMessageTemplates()
        {
            var eaGeneral = _emailAccountRepository.Table.Where(ea => ea.DisplayName.Equals("General contact")).FirstOrDefault();
            var messageTemplates = new List<MessageTemplate>
            {
                new MessageTemplate
                {
                    Name = "Account.EmailValidationMessage",
                    Subject = "%Store.Name%. Email validation",
                    Body = "<a href=\"%Store.URL%\">%Store.Name%</a>  <br />  <br />  To activate your account <a href=\"%Customer.AccountActivationURL%\">click here</a>.     <br />  <br />  %Store.Name%",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
                new MessageTemplate
                {
                    Name = "Account.PasswordRecovery",
                    Subject = "%Store.Name%. Password recovery",
                    Body = "<a href=\"%Store.URL%\">%Store.Name%</a>  <br />  <br />  To change your password <a href=\"%Customer.PasswordRecoveryURL%\">click here</a>.     <br />  <br />  %Store.Name%",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id,
                },
            };

            messageTemplates.ForEach(mt => _messageTemplateRepository.Insert(mt));
        }

        private void InstallSettings()
        {
            IoC.Resolve<ISysConfigurationProvider<AccountSettings>>()
                .SaveSettings(new AccountSettings()
                {
                    UsernamesEnabled = false,
                    AccountNameFormat = AccountNameFormat.ShowEmails,
                    AllowUploadAvatars = false,
                    AllowUsersToChangeUsernames = false,
                    AllowViewingProfiles = false,
                    AvatarMaximumSizeBytes = 20000,
                    CheckUsernameAvailabilityEnabled = false,
                    CityEnabled = false,
                    CountryEnabled = false,
                    DateOfBirthEnabled = false,
                    DefaultAvatarEnabled = false,
                    FaxEnabled = false,
                    GenderEnabled = false,
                    DefaultPasswordFormat = PasswordFormat.Hashed,
                    HashedPasswordFormat = "SHA1",
                    HideNewsletterBlock = true,
                    LastVisitedPage = true,
                    NewsletterEnabled = false,
                    NotifyNewRegistration = false,
                    OnlineMinutes = 20,
                    PasswordMinLength = 6,
                    PhoneEnabled = false,
                    ShowJoinDate = false,
                    ShowLocation = false,
                    StateProvinceEnabled = false,
                    StreetAddress2Enabled = false,
                    StreetAddressEnabled = false,
                    UserRegistrationType = UserRegistrationType.Standard,
                    ZipPostalCodeEnabled = false,
                });

            IoC.Resolve<ISysConfigurationProvider<WebInformationSettings>>()
                .SaveSettings(new WebInformationSettings()
                {
                    AllowAccountToSelectTheme = false,
                    DefaultWebThemeForDesktops = "default",
                    UseMiniProfiler = false,
                    WebClosed = false,
                    WebClosedAllowForAdmins = true,
                    WebName = "Your web name",
                    WebUrl = "http://www.company.com"
                });

            IoC.Resolve<ISysConfigurationProvider<AdminAreaSettings>>()
                .SaveSettings(new AdminAreaSettings()
                {
                    GridPageSize = 15
                });

            IoC.Resolve<ISysConfigurationProvider<InstallSuccessPageSettings>>()
                .SaveSettings(new InstallSuccessPageSettings()
                {
                    Visited = false
                });

            IoC.Resolve<ISysConfigurationProvider<EmailAccountSettings>>()
                .SaveSettings(new EmailAccountSettings()
                {
                    DefaultEmailAccountId = _emailAccountRepository.Table.FirstOrDefault().Id
                });

            IoC.Resolve<ISysConfigurationProvider<LocalizationSettings>>()
                .SaveSettings(new LocalizationSettings()
                {
                    DefaultAdminLanguageId = _languageRepository.Table.Where(l => l.Name == "Vietnamese").Single().Id,
                    UseImagesForLanguageSelection = false,
                });

            IoC.Resolve<ISysConfigurationProvider<MessageTemplatesSettings>>()
                .SaveSettings(new MessageTemplatesSettings()
                {
                    CaseInvariantReplacement = false,
                });

            IoC.Resolve<ISysConfigurationProvider<SecuritySettings>>()
                .SaveSettings(new SecuritySettings()
                {
                    ForceSslForAllPages = false,
                    EncryptionKey = "273ece6f97dd844d",
                    AdminAreaAllowedIpAddresses = null
                });

            IoC.Resolve<ISysConfigurationProvider<UtilitySettings>>()
                .SaveSettings(new UtilitySettings()
                {
                    UseSystemEmailForContactUsForm = true,
                    UseStoredProceduresIfSupported = true,
                    SitemapEnabled = true,
                    SitemapIncludeCategories = true,
                    SitemapIncludeManufacturers = true,
                    SitemapIncludeProducts = false,
                    SitemapIncludeTopics = true,
                    DisplayJavaScriptDisabledWarning = false,
                    UseFullTextSearch = false,
                    FullTextMode = Aaron.Core.Domain.Utilities.FulltextSearchMode.ExactMatch,
                });
        }

        private void InstallAccountRole()
        {
            var listAccountRole = new List<AccountRole>() 
            { 
                new AccountRole() 
                { 
                    Name = "Administrtors",
                    SystemName = SystemAccountRoleNames.Administrators,
                    Active = true,
                    IsSystemRole = true
                },
                new AccountRole() 
                { 
                    Name = "Registered",
                    SystemName = SystemAccountRoleNames.Registered,
                    Active = true,
                    IsSystemRole = true
                },
                new AccountRole() 
                { 
                    Name = "Guests",
                    SystemName = SystemAccountRoleNames.Guests,
                    Active = true,
                    IsSystemRole = true
                }
            };

            listAccountRole.ForEach(x => _accountRoleRepository.Insert(x));
        }

        private void InstallScheduleTasks()
        {
            var tasks = new List<ScheduleTask>()
            {
                new ScheduleTask()
                {
                    Name = "Delete guests",
                    Type = "Aaron.Core.Services.Accounts.DeleteGuestAccountsTask, Aaron.Core",
                    Seconds = 600,
                    Enabled = true,
                    StopOnError = false
                },

                new ScheduleTask()
                {
                    Name = "Keep alive",
                    Type = "Aaron.Core.Web.KeepAliveTask, Aaron.Core",
                    Seconds = 300,
                    Enabled = true,
                    StopOnError = false
                }
            };

            tasks.ForEach(t => _scheduleTaskRepository.Insert(t));
        }

        public void Install(ITypeFinder typeFinder)
        {
            InstallAccountRole();
            InstallLanguages();
            InstallLocaleResources();
            InstallEmailAccounts();
            InstallMessageTemplates();
            InstallSettings();
            InstallScheduleTasks();
        }

        public int Order
        {
            get { return 0; }
        }
    }
}
