using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookShop.Models
{
    public class ReadmadeBook
    {
        public List<Article> Articles { get; set; }
        public int Article_id { get; set; }
        public int id { get; set; }
        public string ArticleName1 { get; set; }

        // public string[] Array {get;set;}
        //public Array arr { get; set; }




        //public List<string> arr { get; set; }
        public object[] arr { get; set; }
        //public Single[] arr { get; set; }






        // public virtual Book Book { get; set; }
        //public List<Book> BookId { get; set; }
        //public List<ArticleName> Articles_id { get; set; }
        // public List<ArticleId> Articles { get; set; }
    }

    //public class arr
    //{
    //    public int a { get; set; }
    //}

}