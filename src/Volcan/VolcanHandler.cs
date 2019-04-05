using Elasticsearch.Net;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using Nest;
using Nest.JsonNetSerializer;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using Volcan.Extensions;
namespace Volcan
{
    public class VolcanHandler
    {
        private ElasticClient Client { get; set; }

        private ElasticLowLevelClient LowLevelClient { get; set; }

        private IContentLoader _contentLoader { get; set; }


        private ContentReference Root { get { return ContentReference.StartPage; } }

        private string LanguageBranch { get; set; } //sv-SE

        public VolcanHandler(Uri uri, string index)
        {
            //"http://example.com:9200"
            var pool = new SingleNodeConnectionPool(uri);
            var settings = new ConnectionSettings(pool, new InMemoryConnection(), sourceSerializer: (builtin, s) => new VanillaSerializer(builtin, s)).DefaultIndex(index).PrettyJson().DisableDirectStreaming()
                .DefaultMappingFor<PageData>(m => m
                .TypeName("PageData")
                .Ignore(p => p.ParentLink)
                .Ignore(p => p.ArchiveLink)
                .Ignore(x => x.ContentLink)
                .Ignore(x => x.Category)
                .Ignore(x => x.ContentAssetsID)
                .Ignore(x => x.ACL)
                .Ignore(x => x.ChildSortOrder)
                .Ignore(x => x.ExistingLanguages)
                .Ignore(x => x.WorkPageID));


            Client = new ElasticClient(settings);
            LowLevelClient = new ElasticLowLevelClient(settings);

            var createIndexResponse = Client.CreateIndex("volcan", c => c.Mappings(ms => ms
            .Map<PageData>(m => m.AutoMap()
            )
        )
    );
            _contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
        }
        public VolcanHandler(IEnumerable<Uri> uris, string index)
        {
            var connectionPool = new SniffingConnectionPool(uris);
            var settings = new ConnectionSettings(connectionPool, sourceSerializer: (builtin, s) => new VanillaSerializer(builtin, s)).DefaultIndex(index);
            Client = new ElasticClient(settings);
            _contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
        }

        //TOdo: Fix this!! See: https://www.elastic.co/guide/en/elasticsearch/client/net-api/current/auto-map.html
        public void Index()
        {
            var desc = _contentLoader.GetChildren<PageData>(ContentReference.StartPage).ToList();
            for (var i = 0; i < desc.Count; i++)
            {
                var child = desc[i];
                var modded = child.GetAllContentProperties(new Dictionary<string, object>());

                var indexResponse = LowLevelClient.Index<BytesResponse>("people", "person", i.ToString(), PostData.Serializable(modded));
                byte[] responseStream = indexResponse.Body;
            }


            var searchResponse = LowLevelClient.Search<BytesResponse>("people", "person", PostData.Serializable(new
            {
                from = 0,
                size = 10,
                query = new
                {
                    match = new
                    {
                        field = "PageName",
                        query = "VulcanSearch"
                    }
                }
            }));

            var successful = searchResponse.Success;
            var responseJson = searchResponse.Body;
        }

        public ISearchResponse<T> Search<T>(string query) where T : PageData
        {
            var test = Client.Search<T>(s => s.From(0).Size(10).Query(q => q.Match(m => m.Field(f => f.PageTypeName).Query("PageData"))));
            return test;
        }

        public void InspectIndex()
        {
            var client = Client;

            var singleString = Nest.Indices.Index("volcan");
            Nest.Indices singleIndexFromString = "name";
            Nest.Indices multipleIndicesFromString = "name1, name2";
            Nest.Indices multipleIndicesFromStringArray = new[] { "name1", "name2" };
            Nest.Indices allFromString = "_all";

            Nest.Indices allWithOthersFromString = "_all, name2";

            //Nest.Indices singleIndexFromType = typeof();

            //Nest.Indices singleIndexFromIndexName = IndexName.From<Project>();


            ISearchRequest singleStringRequest = new SearchDescriptor<PageData>().Index(singleString);
            var index = singleStringRequest.Index;
            //ISearchRequest singleTypedRequest = new SearchDescriptor<PageData>().Index(singleTyped);
        }

    }

    public class VanillaSerializer : ConnectionSettingsAwareSerializerBase
    {
        public VanillaSerializer(IElasticsearchSerializer builtinSerializer, IConnectionSettingsValues connectionSettings)
            : base(builtinSerializer, connectionSettings) { }

        protected override JsonSerializerSettings CreateJsonSerializerSettings() =>
            new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

        protected override void ModifyContractResolver(ConnectionSettingsAwareContractResolver resolver) =>
            resolver.NamingStrategy = new SnakeCaseNamingStrategy();
    }

}
