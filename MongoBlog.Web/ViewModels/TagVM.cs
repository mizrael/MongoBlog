using System.Collections.Generic;
using MongoDB.Bson;

namespace MongoBlog.Web.ViewModels
{
    public class TagVM
    {     
        public string Text { get; set; }
        public string Slug { get; set; }
        public int PostsCount { get; set; }        
    }
}