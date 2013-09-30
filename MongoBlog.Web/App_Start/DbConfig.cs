using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoBlog.Core;
using MongoBlog.Models;

namespace MongoBlog.Web
{
    public class DbConfig
    {
        public static void SetupDb()
        {
#if DEBUG
            var database = MongoDAL.Connect();

            CleanDocuments(database);

            CreateTestData(database);
#endif
        }

        private static void CreateTestData(MongoDatabase database)
        {
            var rand = new Random();
            var languageCodes = new[] { "eng", "ita", "fre", "deu" };

            var tags = MongoDAL.GetTags(database);
            for (int i = 0; i != 100; ++i)
            {
                var text = "Tag " + (i + 1).ToString();

                var translations = (from l in languageCodes orderby Guid.NewGuid() select l)                                   
                                    .Select(l => new TagLanguage() { LanguageCode = l, Text = string.Format("{0} ({1})", text, l) })
                                    .ToList();

                var tag = new Tag() { Slug = "tag-" + (i + 1).ToString(), Translations = translations };
                tags.Save(tag);
            }            

            var posts = MongoDAL.GetPosts(database);

            posts.EnsureIndex(new IndexKeysBuilder().Ascending("Slug"));
            posts.EnsureIndex(new IndexKeysBuilder().Ascending("Date"));            
            posts.EnsureIndex(new IndexKeysBuilder().Ascending("Tags.Slug"));

            var tagArray = tags.FindAll().ToArray();

            for (int i = 0; i != 50000; ++i)
            {
                var title =  "Post " + (i + 1).ToString();
                var text = "lorem ipsum " + (i + 1).ToString();

                var randLangCodes = (from l in languageCodes orderby Guid.NewGuid() select l).Take(rand.Next(1, languageCodes.Length)).ToArray();

                var translations = randLangCodes.Select(l => new PostLanguage() { LanguageCode = l, Title = string.Format("{0} ({1})", text, l), Launch = text })
                                                .ToList();

                var postTags = (from t in tagArray
                                orderby Guid.NewGuid()
                                select t.Id).Take(rand.Next(10)).ToList();

                var post = new Post()
                {
                    Date = DateTime.Now,                    
                    Slug = title.Replace(" ", "_"),
                    OriginalLanguageCode = randLangCodes.First(),
                    Translations = translations,
                    Tags = postTags
                };               

                posts.Save(post);
            }
            
            // WARNING!! MAY TAKE A LOT OF TIME!
            //var staticPosts = posts.AsQueryable().ToArray();
            //foreach (var post in staticPosts)
            //{
            //    var relatedPosts = staticPosts.OrderBy(p => Guid.NewGuid())
            //                                .Take(rand.Next(0, 10))
            //                                .Select(p => p.Id)
            //                                .ToList();
            //    post.RelatedPosts = relatedPosts;
            //    posts.Save(post);
            //}

            MongoDAL.RefreshTagCloud(database);
        }

        private static void CleanDocuments(MongoDatabase database)
        {
            MongoDAL.RemoveDocuments(database, "Posts", true);
            MongoDAL.RemoveDocuments(database, "TagCloud", true);
        }       
    }
}