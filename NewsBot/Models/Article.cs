using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewsBot.Models
{
    public class Article
    {
        //generated from RESPONSE @ newsapi.org/ /w json2csharp.com/
        public string Author { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Uri Url { get; set; }
        public Uri UrlToImage { get; set; }
        public string PublishedAt { get; set; }
    }
}