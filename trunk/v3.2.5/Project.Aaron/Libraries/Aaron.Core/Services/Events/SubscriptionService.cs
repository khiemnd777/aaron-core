using System.Collections.Generic;
using Aaron.Core.Infrastructure;

namespace Aaron.Core.Services.Events
{
    public class SubscriptionService : ISubscriptionService
    {
        public IList<IConsumer<T>> GetSubscriptions<T>()
        {
            return IoC.ResolveAll<IConsumer<T>>() as List<IConsumer<T>>;
        }
    }
}
