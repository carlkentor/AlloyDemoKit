using AlloyDemoKit.Models.Pages;
using AlloyDemoKit.Models.ViewModels;
using EPiServer.Core;
using System;
using System.Web.Mvc;
using Volcan;

namespace AlloyDemoKit.Controllers
{
    public class SearchPageController : PageControllerBase<SearchPage>
    {

        public ActionResult Index(SearchPage currentPage)
        {

            var handler = new VolcanHandler(new Uri("http://localhost:9200"), "volcan");
            handler.Index();

            var s = handler.Search<PageData>("a");

            var model = new SearchContentModel(currentPage)
            {

            };
            return View(model);
            //var customVulcan = new VulcanCustom();
            //customVulcan.SearchAllContent();
            //var model = new SearchContentModel(currentPage)
            //{

            //};
            //return View(model);
        }
    }
}
