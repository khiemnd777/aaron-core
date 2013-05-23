using System.Collections.Generic;
using Aaron.Core.Domain.Accounts;
using Aaron.Core.Domain.Messages;
using Aaron.Core.Services.Messages;

namespace Aaron.Core.Services.Messages
{
    public partial interface IMessageTokenProvider
    {
        void AddWebTokens(IList<Token> tokens);

        void AddAccountTokens(IList<Token> tokens, Account account);

        void AddNewsLetterSubscriptionTokens(IList<Token> tokens, NewsLetterSubscription subscription);

        string[] GetListOfCampaignAllowedTokens();

        string[] GetListOfAllowedTokens();
    }
}
