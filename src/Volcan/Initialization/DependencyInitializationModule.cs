using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using Volcan.Interfaces;
using Volcan.Services;

namespace Volcan.Initialization
{
    [ModuleDependency(typeof(ServiceContainerInitialization))]
    public class DependencyInitializationModule : IConfigurableModule, IInitializableModule
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
            context.Services.AddSingleton<IVolcanIndex, VolcanIndex>();
        }

    }
}
