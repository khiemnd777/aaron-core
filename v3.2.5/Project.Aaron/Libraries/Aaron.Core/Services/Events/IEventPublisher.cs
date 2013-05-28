
namespace Aaron.Core.Services.Events
{
    public interface IEventPublisher
    {
        void Publish<T>(T eventMessage);
    }
}
