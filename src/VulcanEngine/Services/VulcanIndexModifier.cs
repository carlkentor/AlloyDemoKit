using EPiServer.Core;
using EPiServer.ServiceLocation;
using System.Collections.Generic;
using System.Linq;
using TcbInternetSolutions.Vulcan.Core;

namespace VulcanEngine.Services
{
    [ServiceConfiguration(typeof(IVulcanIndexingModifier), Lifecycle = ServiceInstanceScope.Singleton)]
    public class VulcanIndexModifier : IVulcanIndexingModifier
    {
        private readonly List<IVulcanContentAncestorLoader> _vulcanContentAncestorLoaders;

        public VulcanIndexModifier()
        {
        }
        public void ProcessContent(IVulcanIndexingModifierArgs args)
        {
            // index ancestors
            var ancestors = new List<ContentReference>();

            // constructor injected service
            if (_vulcanContentAncestorLoaders?.Any() == true)
            {
                foreach (var ancestorLoader in _vulcanContentAncestorLoaders)
                {
                    IEnumerable<ContentReference> ancestorsFound = ancestorLoader.GetAncestors(args.Content);

                    if (ancestorsFound?.Any() == true)
                    {
                        ancestors.AddRange(ancestorsFound);
                    }
                }
            }
            args.AdditionalItems[VulcanFieldConstants.Ancestors] = ancestors.Select(x => x.ToReferenceWithoutVersion()).Distinct();
        }
    }
}
