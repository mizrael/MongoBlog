using System;
using System.Collections.Generic;
using MongoDB.Bson;

namespace MongoBlog.Web.ViewModels
{
    public class PostVM
    {
        public string Slug { get; set; }
        public string Title { get; set; }
        public string Launch { get; set; }
        public string OriginalLanguage { get; set; }
        public IEnumerable<string> AvailableLanguages { get; set; }
        public IEnumerable<TagVM> Tags { get; set; }
    }
}
