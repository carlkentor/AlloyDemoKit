using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using TcbInternetSolutions.Vulcan.Core;

namespace VulcanEngine.Initialization
{
    [ModuleDependency(typeof(ServiceContainerInitialization))]
    public class TestConfigure : IConfigurableModule
    {
        public void Initialize(InitializationEngine context) { }

        public void Uninitialize(InitializationEngine context) { }

        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            context.Services.AddSingleton<IVulcanIndexContentJobSettings, ParallelIndexing>();
        }
    }

    public class ParallelIndexing : IVulcanIndexContentJobSettings
    {
        public bool EnableParallelIndexers => true;
        public bool EnableParallelContent => true;

        public bool EnableAlwaysUp => true;

        public int ParallelDegree => -1; // Grabs everything
    }
}
