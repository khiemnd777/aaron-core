using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aaron.Core.Domain.Accounts;
using Aaron.Core.Data;

namespace Aaron.Core.Services.Accounts
{
    public class InstallAccountFirstService : IInstallAccountFirstService
    {
        private readonly IAccountService _accountService;
        private readonly IAccountRegistrationService _accountRegistrationService;

        public InstallAccountFirstService(IAccountService accountService,
            IAccountRegistrationService accountRegistrationService)
        {
            _accountService = accountService;
            _accountRegistrationService = accountRegistrationService;
        }

        private void CreateDefaultAdmin(string defaultUserEmail, string defaultUserPassword)
        {
            var admin = new Account()
            {
                AccountGuid = Guid.NewGuid(),
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
            };

            var arAdmin = _accountService.GetAccountRoleBySystemName(SystemAccountRoleNames.Administrators);
            admin.AccountRoles.Add(arAdmin);

            _accountService.InsertAccount(admin);

            var registrarRequest = new AccountRegistrationRequest(admin, defaultUserEmail, defaultUserEmail, defaultUserPassword, PasswordFormat.Hashed);
            var registrarResult = _accountRegistrationService.RegisterAccount(registrarRequest);
        }

        public void Install(string defaultUserEmail, string defaultUserPassword)
        {
            CreateDefaultAdmin(defaultUserEmail, defaultUserPassword);
        }
    }
}
