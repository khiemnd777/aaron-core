
using Aaron.Core.Infrastructure;
namespace Aaron.Core.Installation
{
    public interface IInstallationProvider
    {
        void Install(ITypeFinder typeFinder);
        int Order { get; }
    }
}
