using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aaron.Core.Threading;

namespace Aaron.Core.Services.Accounts
{
    public class DeleteGuestAccountsTask : ITask
    {
        private readonly IAccountService _accountService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteGuestAccountsTask" /> class.
        /// </summary>
        /// <param name="accountService">The account service.</param>
        public DeleteGuestAccountsTask(IAccountService accountService)
        {
            _accountService = accountService;
        }

        /// <summary>
        /// Execute task
        /// </summary>
        public void Execute()
        {
            var olderThanMinutes = 1440;
            _accountService.DeleteGuestAccounts(null, DateTime.UtcNow.AddMinutes(-olderThanMinutes));
        }
    }
}
