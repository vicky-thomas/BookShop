using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookShop.Models
{
    public class ArticleDetails
    {
        public int Article_id { get; set; }
        public int id { get; set; }
        public string ArticleName { get; set; }

         
         public List<BookDetails> books { get; set; }
    }
}