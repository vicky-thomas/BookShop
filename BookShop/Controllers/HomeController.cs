using BookShop.Models;
using Dapper;
using System;
using System.Activities.Expressions;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookShop.Controllers
{
    public class HomeController : Controller
    {
        SchemaEntities1 ctx = new SchemaEntities1();
        public static string constr = ConfigurationManager.ConnectionStrings["getconn"].ToString();
        IDbConnection con = new SqlConnection(constr);
        public ActionResult About()
        {
            List<Article> articles = ctx.Articles.ToList();
            return View(articles);
        }
        public ActionResult Contact()
        {
            return View();
        }

        public JsonResult BookValues(Book book)
        {
            if (book != null)
            {
                ctx.Books.Add(book);
                ctx.SaveChanges();
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult Article()
        {
            return View();
        }
        public JsonResult Drop()
        {
            var books = ctx.Books.Select(d=>new { d.BookName ,d.id}).ToList();
            return Json(books, JsonRequestBehavior.AllowGet);
        }
        public ActionResult saved(Article article)
        {
            if (article.ArticleName != null)
            {
                ctx.Articles.Add(article);
                ctx.SaveChanges();
                return RedirectToAction("Sample");
            }
            else
            {
                return Content("not found");
            }
        }
        public ActionResult Sample()
        {
            var finalvalue = ctx.Articles.ToList();
            var values = finalvalue.Select(x => new { x.Article_id, x.ArticleName, x.Book.BookName, x.id }).ToList();
            //var JoinTable = ctx.Books.Include("Articlenames").Select( b=>new { b.BookName, b.ArticleNames.Select(d=>new { d.Article_id,d.ArticleName1})});
            return View(finalvalue);
        }
        public ActionResult Edit(int id)
        {
            var getvalues = ctx.Articles.ToList();
            var cb = getvalues.Find(d => d.Article_id == id);
            if (cb != null)
            {
                //var book = cb.Book.BookName;
                //ViewBag.bookname = book;
                return View(cb);
            }
            else
            {
                RedirectToAction("EditDone");
            }
            return RedirectToAction("EditDrop");
        }
        public ActionResult EditDone(Article article)
        {
           
            Article update = ctx.Articles.Where(a => a.Article_id == article.Article_id).SingleOrDefault();
            update.ArticleName = article.ArticleName;
            update.id = article.id;
            ctx.SaveChanges();
            return RedirectToAction("EditDrop");
        }
        public ActionResult Delete(int id)
        {
            //bool val = false;
                var delete = ctx.Articles.Where(a => a.Article_id == id).FirstOrDefault();
                ctx.Articles.Remove(delete);
                ctx.SaveChanges();
                return RedirectToAction("EditDrop");
        }
        public ActionResult EditAll()
        {
            var finalvalue = ctx.Articles.ToList();
            return View();
        }
        public JsonResult Page()
        {
            List<ArticleDetails> authorDetailsList = new List<ArticleDetails>();
            using (System.Data.SqlClient.SqlConnection MyConnection = new SqlConnection("data source=AMX-VIGNESH;database=Schema;integrated security=True"))
            {
                string query = "select * from Article";
                using (SqlCommand MyCommand = new SqlCommand(query))
                {
                    MyCommand.Connection = MyConnection;
                    MyConnection.Open();
                    SqlDataReader sdr = MyCommand.ExecuteReader();
                    while (sdr.Read())
                    {
                        ArticleDetails authorDetails = new ArticleDetails
                        {
                            id = Convert.ToInt32(sdr["id"]),
                            Article_id = Convert.ToInt32(sdr["Article_id"]),
                            ArticleName = Convert.ToString(sdr["ArticleName"])
                            
                        };
                        using (System.Data.SqlClient.SqlConnection MyConnection1 = new SqlConnection("data source=AMX-VIGNESH;database=Schema;integrated security=True"))
                        {
                            List<BookDetails> BookList = new List<BookDetails>();
                            string queryForBookDetails = "select * from Book where id = " + authorDetails.id;
                            SqlCommand MyCommand1 = new SqlCommand(queryForBookDetails)
                            {
                                Connection = MyConnection1
                            };
                            MyConnection1.Open();
                            using (SqlDataReader sdrBooks = MyCommand1.ExecuteReader())
                            {
                                while (sdrBooks.Read())
                                {
                                    BookDetails bookDetails = new BookDetails
                                    {
                                        id = Convert.ToInt32(sdrBooks["id"]),
                                        BookName = sdrBooks["BookName"].ToString()
                                    };
                                    BookList.Add(bookDetails);
                                }
                                if (BookList.Count !=0)
                                {
                                    authorDetails.books = BookList;
                                }
                               
                                MyConnection1.Close();
                            }
                        }
                        authorDetailsList.Add(authorDetails);
                    }
                    MyConnection.Close();
                }
            }
            return Json(authorDetailsList, JsonRequestBehavior.AllowGet);
        }
    
        public JsonResult EditDoneAll(ReadmadeBook article)
      {
            #region Article Table value
            List<ArticleDetails> Article = new List<ArticleDetails>();
            using (System.Data.SqlClient.SqlConnection MyConnection = new SqlConnection("data source=AMX-VIGNESH;database=Schema;integrated security=True"))
            {
                string query = "select * from Article";
                using (SqlCommand MyCommand = new SqlCommand(query))
                {
                    MyCommand.Connection = MyConnection;
                    MyConnection.Open();
                    SqlDataReader sdr = MyCommand.ExecuteReader();
                    while (sdr.Read())
                    {
                        ArticleDetails authorDetails = new ArticleDetails
                        {
                            id = Convert.ToInt32(sdr["id"]),
                            Article_id = Convert.ToInt32(sdr["Article_id"]),
                            ArticleName = Convert.ToString(sdr["ArticleName"])

                        };
                        Article.Add(authorDetails);
                    }
                    MyConnection.Close();
                }
            }
            #endregion
            int steps = article.Articles.Count;
            //int count = ctx.Articles.Count();
            if (steps > 1)
            {
                for (int i = 0; i < steps; i++)
                {

                    int id = article.Articles[i].Article_id;
                    var ArticleName = article.Articles[i].ArticleName;
                    var Bookid = article.Articles[i].id;

                    var finalvalue = Article.Where(df => df.Article_id == id).SingleOrDefault();
                    if (finalvalue.ArticleName != ArticleName || finalvalue.id != Bookid)
                    {
                        using (System.Data.SqlClient.SqlConnection MyConnection = new SqlConnection("data source=AMX-VIGNESH;database=Schema;integrated security=True"))
                        {
                            string query = "UPDATE Article SET ArticleName = '" + ArticleName + "',id = " + Bookid + " WHERE Article_id = " + id;
                            using (SqlCommand MyCommand = new SqlCommand(query))
                            {
                                MyCommand.Connection = MyConnection;
                                MyConnection.Open();
                                MyCommand.ExecuteNonQuery();
                                MyConnection.Close();
                            }
                        }
                    }
                }
            }
            else
            {
                int id = article.Articles[0].Article_id;
                var ArticleName = article.Articles[0].ArticleName;
                var Bookid = article.Articles[0].id;
                Bookid++;
                var finalvalue = Article.Where(df => df.Article_id == id).SingleOrDefault();
                if (finalvalue.ArticleName != ArticleName || finalvalue.id != Bookid)
                {
                    using (System.Data.SqlClient.SqlConnection MyConnection = new SqlConnection("data source=AMX-VIGNESH;database=Schema;integrated security=True"))
                    {
                        string query = "UPDATE Article SET ArticleName = '" + ArticleName + "',id = " + Bookid + " WHERE Article_id = " + id;
                        using (SqlCommand MyCommand = new SqlCommand(query))
                        {
                            MyCommand.Connection = MyConnection;
                            MyConnection.Open();
                            MyCommand.ExecuteNonQuery();
                            MyConnection.Close();
                        }
                    }
                }
            }
            return Json(Article, JsonRequestBehavior.AllowGet);
        }
        public ActionResult EditMultipleRecord()
        {
             return View();
        }

        public JsonResult getdropdownlist()
        {

            //var details = ctx.Articles.Select(d => new { d.Book.BookName, d.Book.id ,d.Article_id}).ToList();
            //var detail = ctx.Books.Select(d => new { d.id, d.BookName }).ToList();
            //return Json(details, JsonRequestBehavior.AllowGet);
            List<ArticleDetails> authorDetailsList = new List<ArticleDetails>();
            using (System.Data.SqlClient.SqlConnection MyConnection = new SqlConnection("data source=AMX-VIGNESH;database=Schema;integrated security=True"))
            {
                string query = "select * from Article";
                using (SqlCommand MyCommand = new SqlCommand(query))
                {
                    MyCommand.Connection = MyConnection;
                    MyConnection.Open();
                    SqlDataReader sdr = MyCommand.ExecuteReader();
                    while (sdr.Read())
                    {
                        ArticleDetails authorDetails = new ArticleDetails
                        {
                            id = Convert.ToInt32(sdr["id"]),
                            Article_id = Convert.ToInt32(sdr["Article_id"]),
                            ArticleName = Convert.ToString(sdr["ArticleName"])

                        };
                        using (System.Data.SqlClient.SqlConnection MyConnection1 = new SqlConnection("data source=AMX-VIGNESH;database=Schema;integrated security=True"))
                        {
                            List<BookDetails> BookList = new List<BookDetails>();
                            string queryForBookDetails = "select * from Book where id = " + authorDetails.id;
                            SqlCommand MyCommand1 = new SqlCommand(queryForBookDetails)
                            {
                                Connection = MyConnection1
                            };
                            MyConnection1.Open();
                            using (SqlDataReader sdrBooks = MyCommand1.ExecuteReader())
                            {
                                while (sdrBooks.Read())
                                {
                                    BookDetails bookDetails = new BookDetails
                                    {
                                        id = Convert.ToInt32(sdrBooks["id"]),
                                        BookName = sdrBooks["BookName"].ToString()
                                    };
                                    BookList.Add(bookDetails);
                                }
                                if (BookList.Count != 0)
                                {
                                    authorDetails.books = BookList;
                                }

                                MyConnection1.Close();
                            }
                        }
                        authorDetailsList.Add(authorDetails);
                    }
                    MyConnection.Close();
                }
            }
            return Json(authorDetailsList, JsonRequestBehavior.AllowGet);
        }


        //[OutputCache(CacheProfile = "MyData", Location = System.Web.UI.OutputCacheLocation.Server)]
        public JsonResult bookdetails()
        {
            List<BookDetails> List = new List<BookDetails>();
            using (var connection = new SqlConnection(constr))
            {
                List = connection.Query<BookDetails>("select * from Book").ToList();
                return Json(List, JsonRequestBehavior.AllowGet);
            }
        }
        //public ActionResult bookAction()
        //{
        //    List<ArticleDetails> articleDetails = new List<ArticleDetails>();
        //    List<BookDetails> bookDetails = new List<BookDetails>();
        //    using (var connection = new SqlConnection(constr))
        //    {
        //        articleDetails = connection.Query<ArticleDetails>("select * from Article").ToList();
        //        bookDetails = connection.Query<BookDetails>("select * from Book").ToList();

        //        //ArticleDetails obj = new ArticleDetails();
        //        //obj.books = bookDetails;
        //        //obj.Articles = articleDetails;
        //        List<BookAndArticle> bothList = new List<BookAndArticle>();
        //        BookAndArticle both = new BookAndArticle();
        //        both.Books = bookDetails;
        //        both.Articles = articleDetails;
        //        bothList.Add(both);
        //        return View(articleDetails);
        //    }
        //}
        public ActionResult bookAction()
        {
            return View();
        }
            public ActionResult BookAction1()
        {
            List<ArticleDetails> articleDetails = new List<ArticleDetails>();
            List<BookDetails> bookDetails = new List<BookDetails>();
            using (var connection = new SqlConnection(constr))
            {
                articleDetails = connection.Query<ArticleDetails>("select * from Article").ToList();
                bookDetails = connection.Query<BookDetails>("select * from Book").ToList();

                //ArticleDetails obj = new ArticleDetails();
                //obj.books = bookDetails;
                //obj.Articles = articleDetails;
                List<BookAndArticle> bothList = new List<BookAndArticle>();
                BookAndArticle both = new BookAndArticle();
                both.Books = bookDetails;
                both.Articles = articleDetails;
                bothList.Add(both);
                return Json(articleDetails, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult BookAction2()
        {
            List<ArticleDetails> articleDetails = new List<ArticleDetails>();
            List<BookDetails> bookDetails = new List<BookDetails>();
            using (var connection = new SqlConnection(constr))
            {
                articleDetails = connection.Query<ArticleDetails>("select * from Article").ToList();
                bookDetails = connection.Query<BookDetails>("select * from Book").ToList();

                //ArticleDetails obj = new ArticleDetails();
                //obj.books = bookDetails;
                //obj.Articles = articleDetails;
                List<BookAndArticle> bothList = new List<BookAndArticle>();
                BookAndArticle both = new BookAndArticle();
                both.Books = bookDetails;
                both.Articles = articleDetails;
                bothList.Add(both);
                return Json(bookDetails, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult getlist()
        {

            var details = ctx.Articles.Select(d => new { d.Book.id, d.Article_id ,d.ArticleName}).ToList();
            return Json(details, JsonRequestBehavior.AllowGet);
        }
        public JsonResult EditSingleRow(int id)
        {

            var singledetails = ctx.Articles.Where(d => d.Article_id == id).ToList();
            return Json(singledetails, JsonRequestBehavior.AllowGet);
        }

    }
}