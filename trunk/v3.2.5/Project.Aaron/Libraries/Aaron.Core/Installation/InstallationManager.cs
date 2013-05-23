using Aaron.Core.Infrastructure;

namespace Aaron.Core.Installation
{
    public class InstallationManager
    {
        private static IInstallationPublisher _publisher
        {
            get 
            {
                if (Singleton<IInstallationPublisher>.Instance == null)
                {
                    Singleton<IInstallationPublisher>.Instance = IoC.Resolve<IInstallationPublisher>();
                }

                return Singleton<IInstallationPublisher>.Instance;
            }
        }

        public static void Install()
        {
            _publisher.Install();
        }
    }
}
