using System;
using System.Collections.Generic;
using System.Linq;
using FluentMongo.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoBlog.Web.Core.Models;

namespace MongoBlog.Web.Core
{
    public static class MongoDAL
    {
        public static MongoCollection<Post> GetPosts(MongoDatabase database)
        {
            return database.GetCollection<Post>("Posts");
        }

        public static MongoCollection<Tag> GetTags(MongoDatabase database)
        {
            return database.GetCollection<Tag>("TagCloud");
        }

        public static MongoDatabase Connect()
        {
            var settings = new MongoServerSettings();
            settings.Server = new MongoServerAddress("localhost", 27017);

            MongoServer server = new MongoServer(settings);
            return server.GetDatabase("Blog");
        }

        public static void RemoveDocuments(MongoDatabase database, string collectionName, bool drop)
        {            
            var docs = database.GetCollection<Post>(collectionName);
            if (null != docs)
            {
                if (docs.Count() != 0)
                    docs.RemoveAll();

                if(drop)
                    database.DropCollection(collectionName);
            }
        }

        public static void RefreshTagCloud(MongoDatabase database)
        {
            var posts = GetPosts(database);

            string map = @"function() {
                            var post = this;
                            for (index in post.Tags) {
                                var tag = post.Tags[index];
                                //emit(tag.Slug, {count: 1, tag: tag });
                                emit(tag, {count: 1, tag: tag });
                            }
                        }";

            string reduce = @"function(previous, current) {
                    var reduced = {count:0, tag: null };
                   
                    for (index in current) {
                        reduced.tag =  current[index].tag;
                        reduced.count += current[index].count;                        
                    }

                    return reduced;
                }";

            var mr = posts.MapReduce(new BsonJavaScript(map), new BsonJavaScript(reduce));
            var results = mr.GetResults();
            var tagCloud = GetTags(database);
            foreach (var res in results) {
                var tagId = res["value"]["tag"].AsObjectId;

                var tag = tagCloud.FindOneById(tagId);
                tag.PostsCount = res["value"]["count"].ToInt32();
                
                tagCloud.Save(tag);
            }            


          /*  var mr = posts.MapReduce(new BsonJavaScript(map), new BsonJavaScript(reduce));
            var tags = mr.GetResults().OrderByDescending(r => r["value"]["count"])
                                      .Select(r =>
                                      {
                                          var tagBSon = r["value"]["tag"];
                                          var tag = BsonSerializer.Deserialize<Tag>(tagBSon.ToJson());
                                          tag.PostsCount = r["value"]["count"].ToInt32();
                                          return tag;
                                      }).ToArray();

            RemoveDocuments(database, "TagCloud", true);
            var tagCloud = GetTags(database);
            foreach (var tag in tags)
                tagCloud.Save(tag);
        
            foreach (var post in posts.AsQueryable()) {
                if (null == post.Tags || !post.Tags.Any())
                    continue;

                var newTags = tags.Join(post.Tags, t => t.Id, pt => pt, (t, pt) => t.Id).ToArray();
                if (null == newTags || !newTags.Any())
                    continue;

                post.Tags = newTags.ToList();
                posts.Save(post);
            }*/
        }
    }
}