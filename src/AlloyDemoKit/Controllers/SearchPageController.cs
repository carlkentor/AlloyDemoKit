using AlloyDemoKit.Models.Pages;
using AlloyDemoKit.Models.ViewModels;
using System.Web.Mvc;

namespace AlloyDemoKit.Controllers
{
    public class SearchPageController : PageControllerBase<SearchPage>
    {

        public ActionResult Index(SearchPage currentPage)
        {
            var model = new SearchContentModel(currentPage)
            {

            };
            return View(model);
        }
    }
}
