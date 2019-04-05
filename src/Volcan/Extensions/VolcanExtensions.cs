using EPiServer.Core;
using EPiServer.SpecializedProperties;
using Nest;
using System.Collections.Generic;

namespace Volcan.Extensions
{
    public static class VolcanExtensions
    {
        //public static ISearchResponse<T> SearchContent<T>(this ElasticClient client, string query) where T : PageData
        //{
        //    var searchResponse = client.Search<T>(s => s.From(0).Size(10).Query(q => q.Match(m => m.Field(f => f.Name).Query(query))));
        //    return searchResponse;
        //}

        /// <summary>
        /// This method converts a PageData into  Dictionary objects. This methods is convinient when the serializer chokes on a PageData
        /// </summary>
        /// <param name="content"></param>
        /// <param name="result"></param>
        /// <returns>A key/value dictionary</returns>
        public static Dictionary<string, object> GetAllContentProperties(this IContentData content, Dictionary<string, object> result)
        {
            foreach (var prop in content.Property)
            {
                if (prop.GetType().IsGenericType && prop.GetType().GetGenericTypeDefinition() == typeof(PropertyBlock<>))
                {
                    var newStruct = new Dictionary<string, object>();
                    result.Add(prop.Name, newStruct);
                    GetAllContentProperties((IContentData)prop, newStruct);
                    continue;
                }
                if (prop.Value != null)
                    result.Add(prop.Name, prop.Value.ToString());
            }
            return result;
        }
    }
}
