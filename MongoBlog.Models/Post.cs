using System;
using System.Collections.Generic;
using MongoDB.Bson;

namespace MongoBlog.Models
{
    public class Post
    {
        public ObjectId Id { get; set; }
        public DateTime Date { get; set; }        
        public string Slug { get; set; }
        public string OriginalLanguageCode { get; set; }
        public List<PostLanguage> Translations { get; set; }
        public List<ObjectId> Tags { get; set; }
        public List<ObjectId> RelatedPosts { get; set; }
    }

    public class PostLanguage {
        public string LanguageCode { get; set; }
        public string Title { get; set; }
        public string Launch { get; set; }
    }
}