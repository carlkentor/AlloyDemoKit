using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using TcbInternetSolutions.Vulcan.Core;

namespace VulcanEngine.Initialization
{
    [ModuleDependency(typeof(ServiceContainerInitialization))]
    public class RegisterImplementations : IConfigurableModule, IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
        }

        public void Uninitialize(InitializationEngine context)
        {
        }

        void IConfigurableModule.ConfigureContainer(ServiceConfigurationContext context)
        {
            // hack: using manual registration as scheduled job doesn't inject otherwise
            context.Services.AddSingleton<IVulcanIndexer, TcbInternetSolutions.Vulcan.Core.Implementation.VulcanCmsIndexer>();
        }

    }
}
