using System.Linq;
using System.Web.Mvc;
using MongoBlog.Core;
using MongoBlog.Web.ViewModels.Adapters;
using MongoDB.Driver.Builders;

namespace MongoBlog.Web.Controllers
{
    public class TagController : Controller
    {   
        [OutputCache(Duration=60 * 10, VaryByCustom="lang" )]
        public PartialViewResult TagCloud() {
            var database = MongoDAL.Connect();
            var tagCloud = MongoDAL.GetTags(database);

            var sortBy = SortBy.Descending("PostsCount");
            var results = tagCloud.Find(Query.Null).SetSortOrder(sortBy).Take(10).Select(t => TagAdapter.Adapt(this.HttpContext, t)).ToArray();

            return PartialView(results);
        }

        public ActionResult Regen() {
            var db = MongoDAL.Connect();
            MongoDAL.RefreshTagCloud(db);
            return RedirectToAction("Index");
        }

  
    }
}
