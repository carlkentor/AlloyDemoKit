using EPiServer;
using EPiServer.Core;
using Nest;
using System;
using System.Linq;
using System.Web;
using TcbInternetSolutions.Vulcan.Core;
using TcbInternetSolutions.Vulcan.Core.Extensions;

namespace VulcanEngine
{
    public class VulcanCustom
    {
        private IVulcanHandler _VulcanHandler;
        private IContentLoader _ContentLoader;

        public VulcanCustom()
        {
            _VulcanHandler = EPiServer.ServiceLocation.ServiceLocator.Current.GetInstance<IVulcanHandler>();
            _ContentLoader = EPiServer.ServiceLocation.ServiceLocator.Current.GetInstance<IContentLoader>();
        }

        /// <summary>
        /// Search all types
        /// </summary>
        public void SearchAllContent()
        {
            var client = _VulcanHandler.GetClient();
            string searchText = "online";
            int currentPage = 1;
            int maxResults = 10;
            var searchScope = new ContentReference[] { ContentReference.StartPage };

            var siteHits = client.GetSearchHits(searchText, currentPage, maxResults, searchRoots: searchScope);
            string searchInfo = $"Search for {searchText} found {siteHits.TotalHits}, on page {siteHits.Page}, page size {siteHits.PageSize} ran for {siteHits.ResponseContext.Took}  ms!";
            string typesInfo = string.Join(Environment.NewLine, siteHits.ResponseContext.Aggs?.Terms("types")?.Buckets?.Select(x => $"{x.Key} ({x.DocCount})"));
            string resultsInfo = string.Join(Environment.NewLine + Environment.NewLine, siteHits.Items.Select(x => x.Title + " " + x.Id + " " + x.Url + Environment.NewLine + x.Summary));

            return;
        }

        /// <summary>
        /// Only search within a content reference and descendents
        /// </summary>
        public void SearchSectionContent()
        {
            var client = _VulcanHandler.GetClient();
            string searchText = "online";
            int currentPage = 1;
            int maxResults = 10;

            // Example using first child of site
            var section = _ContentLoader.GetChildren<PageData>(ContentReference.StartPage).FirstOrDefault();

            // Can be multiple areas
            var searchScope = new ContentReference[] { section.ContentLink };

            var siteHits = client.GetSearchHits(searchText, currentPage, maxResults, searchRoots: searchScope);
        }

        public void SearchOnlyMediaContent()
        {
            var typesToSearchFor = typeof(MediaData).GetSearchTypesFor();
            var client = _VulcanHandler.GetClient();
            string searchText = "online";
            int currentPage = 1;
            int maxResults = 10;

            var siteHits = client.GetSearchHits(searchText, currentPage, maxResults, includeTypes: typesToSearchFor);
        }

        /// <summary>
        /// Overload og GetHits example, using a custom QueryContainer
        /// </summary>
        /// <param name="query"></param>
        /// <param name="pageSize"></param>
        /// <param name="page"></param>
        public void SearchContentCustomQueryContainer(string query = "", int pageSize = 10, int page = 1)
        {
            var typesToSearchFor = typeof(MediaData).GetSearchTypesFor();
            var client = _VulcanHandler.GetClient();
            QueryContainer searchTextQuery = new QueryContainerDescriptor<IContent>();

            // only add query string if query has value
            if (!string.IsNullOrWhiteSpace(query))
            {
                searchTextQuery = new QueryContainerDescriptor<IContent>().SimpleQueryString(sqs => sqs
                    .Fields(f => f
                                .AllAnalyzed()
                                //.FieldAnalyzed(x => x.Name)  //  could be field specific too, or just use above for all analyzed strings
                                .Field($"{VulcanFieldConstants.MediaContents}.content")
                                .Field($"{VulcanFieldConstants.MediaContents}.content_type"))
                    .Query(query)
                );
            }

            // also using a workaround for FilterForPublished
            var siteHits = client.GetSearchHits(FilterForPublished<IContent>(searchTextQuery), page, pageSize, includeTypes: typesToSearchFor);
            string requestString, responseString;

            // For debugging request and response body, set "system.web/compilation" debug to true!
            if (HttpContext.Current.IsDebuggingEnabled && siteHits.ResponseContext.ApiCall.RequestBodyInBytes != null && siteHits.ResponseContext.ApiCall.ResponseBodyInBytes != null)
            {
                requestString = System.Text.Encoding.UTF8.GetString(siteHits.ResponseContext.ApiCall.RequestBodyInBytes);
                responseString = System.Text.Encoding.UTF8.GetString(siteHits.ResponseContext.ApiCall.ResponseBodyInBytes);
            }
        }

        /// <summary>
        /// Workaround for image searches that have no expiration date, now includes a Missing filter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public static QueryContainer FilterForPublished<T>(QueryContainer query) where T : class, IContent
        {
            var notDeleted = new QueryContainerDescriptor<T>().Term(t => t.Field(xf => xf.IsDeleted).Value(false));
            var published = new QueryContainerDescriptor<T>().DateRange(dr => dr.LessThanOrEquals(DateTime.Now).Field(xf => (xf as IVersionable).StartPublish));
            var notExpired = new QueryContainerDescriptor<T>().DateRange(dr => dr.GreaterThanOrEquals(DateTime.Now).Field(xf => (xf as IVersionable).StopPublish));
            var notExpiredMissing = new QueryContainerDescriptor<T>().Exists(dr => dr.Field(xf => (xf as IVersionable).StopPublish));

            return new QueryContainerDescriptor<T>().Bool(b => b.Must(query).Filter(notDeleted && published && (notExpired || notExpiredMissing)));
        }
    }
}

