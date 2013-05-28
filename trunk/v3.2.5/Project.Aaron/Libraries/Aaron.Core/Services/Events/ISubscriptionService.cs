using System.Collections.Generic;

namespace Aaron.Core.Services.Events
{
    public interface ISubscriptionService
    {
        IList<IConsumer<T>> GetSubscriptions<T>();
    }
}
