using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using MongoBlog.Core;
using MongoBlog.Models;

namespace MongoBlog.Web.ViewModels.Adapters
{
    public static class PostAdapter
    {
        public static PostVM Adapt(HttpContextBase context, Post m) {
            var currLang = LanguageProvider.GetUICurrentLanguageCode(context);
            return Adapt(context, m, currLang);
        }

        public static PostVM Adapt(HttpContextBase context, Post m, string langCode)
        {
            var translation = m.Translations.FirstOrDefault(t => t.LanguageCode == langCode) ??
                              m.Translations.FirstOrDefault(t => t.LanguageCode == m.OriginalLanguageCode);

            var vm  = new PostVM()
            {
                Slug = m.Slug,
                Launch = translation.Launch,
                Title = translation.Title,
                OriginalLanguage = m.OriginalLanguageCode,
                AvailableLanguages = m.Translations.Select(pt => pt.LanguageCode).ToArray()
            };

            if (null != m.Tags && m.Tags.Any()) {
                var db = MongoDAL.Connect();
                var tags = MongoDAL.GetTags(db);
                var postTags = tags.Find(Query.In("_id", new BsonArray(m.Tags)));
                vm.Tags = postTags.Select(t => TagAdapter.Adapt(context, t)).ToArray();
            }

            return vm;
        }
    }
}