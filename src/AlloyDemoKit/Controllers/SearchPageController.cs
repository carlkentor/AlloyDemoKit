using AlloyDemoKit.Models.Pages;
using AlloyDemoKit.Models.ViewModels;
using System.Web.Mvc;
using VulcanEngine;

namespace AlloyDemoKit.Controllers
{
    public class SearchPageController : PageControllerBase<SearchPage>
    {

        public ActionResult Index(SearchPage currentPage)
        {
            var customVulcan = new VulcanCustom();
            customVulcan.SearchAllContent();
            var model = new SearchContentModel(currentPage)
            {

            };
            return View(model);
        }
    }
}
