using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MongoBlog.Core;
using MongoBlog.Models;
using MongoBlog.Web.ViewModels;
using MongoBlog.Web.ViewModels.Adapters;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;

namespace MongoBlog.Web.Controllers
{
    public class HomeController : Controller
    {
       // [OutputCache(Duration = 60 * 10)]
        public ActionResult Index()
        {
            var database = MongoDAL.Connect();

            var tags = database.GetCollection<Tag>("Tags");            

            var posts = database.GetCollection<Post>("Posts");
            var models = posts.AsQueryable().OrderByDescending(p => p.Date).Take(5).ToArray();
            
            IEnumerable<PostVM> viewModels = null;
            if (null != models)
                viewModels = models.Select(m => PostAdapter.Adapt(this.HttpContext, m)).ToArray();

            return View(viewModels);
        }

     //   [OutputCache(Duration = 60 * 10, VaryByParam = "any")]
        public ActionResult Posts(string tag, int? page)
        {
            var database = MongoDAL.Connect();          
            
            var sortBy = SortBy.Descending("Date");

            var query = Query.Null;

            if (!string.IsNullOrWhiteSpace(tag))
            {
                var tagCloud = MongoDAL.GetTags(database);

                var filterTag = tagCloud.FindOne(Query.EQ("Slug", tag));
                if (null != filterTag)
                {
                    this.ViewBag.Tag = TagAdapter.Adapt(this.HttpContext, filterTag);
                    query = Query.In("Tags", new BsonArray(new[] { filterTag.Id }));
                }
            }
           
            var posts = MongoDAL.GetPosts(database);

            var pageSize = 10;

            var filteredPosts = posts.Find(query);
            var viewModels = filteredPosts.SetSortOrder(sortBy)
                                          .SetSkip(page.GetValueOrDefault(0) * 10)
                                          .Take(pageSize)
                                          .Select(m => PostAdapter.Adapt(this.HttpContext, m))
                                          .ToArray();

            this.ViewBag.TotalCount = filteredPosts.Count();
            this.ViewBag.Page = page.GetValueOrDefault(0);

            this.ViewBag.TotalPages = (int)Math.Ceiling((decimal)this.ViewBag.TotalCount / pageSize);

            return View(viewModels);
        }

        public ActionResult Details(string title, string lang)
        {
            var database = MongoDAL.Connect();
            var posts = MongoDAL.GetPosts(database);

            var model = posts.Find(Query.EQ("Slug", title))
                             .FirstOrDefault();
            if (null == model)
                return RedirectToAction("Index");


            if (null != model.RelatedPosts && model.RelatedPosts.Any())
            {
                var related = posts.Find(Query.In("_id", new BsonArray(model.RelatedPosts)));
                if (null != related && related.Any())
                {
                    this.ViewBag.Related = related.Select(rp => PostAdapter.Adapt(this.HttpContext, rp)).ToArray();
                }
            }

            if (null != model.Tags && model.Tags.Any())
            {
                var tagsQuery = Query.In("Tags", new BsonArray(model.Tags));
                var skipModelQuery = Query.Not(Query.EQ("_id", model.Id));
                var finalQuery = Query.And(skipModelQuery, tagsQuery);

                var related = posts.Find(finalQuery);
                this.ViewBag.RelatedByTag = related.SetSortOrder(SortBy.Descending("Date"))                    
                                                    .Take(10)
                                                    .Select(rp => PostAdapter.Adapt(this.HttpContext, rp))
                                                    .ToArray();
            }

            if (string.IsNullOrWhiteSpace(lang))
                lang = LanguageProvider.GetUICurrentLanguageCode(this.HttpContext);

            var vm = PostAdapter.Adapt(this.HttpContext, model, lang);
            return View(vm);
        }

        public ActionResult SetLanguage(string lang, string returnUrl)
        {
            LanguageProvider.SetUILanguageCode(this.HttpContext, lang);

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction("Index");
        }
    }
}
