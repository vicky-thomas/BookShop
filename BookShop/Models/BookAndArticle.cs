using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookShop.Models
{
    public class BookAndArticle
    {
        public List<BookDetails> Books = new List<BookDetails>();
        public List<ArticleDetails> Articles = new List<ArticleDetails>();
    }
}