using System.Collections.Generic;
using MongoDB.Bson;

namespace MongoBlog.Web.Core.Models
{
    public class Tag
    {
        public ObjectId Id { get; set; }
        public string Slug { get; set; }
        public int PostsCount { get; set; }
        public List<TagLanguage> Translations { get; set; }
    }

    public class TagLanguage {
        public string LanguageCode { get; set; }
        public string Text { get; set; }
    }
}