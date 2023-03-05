using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MyProject01.Models;
using PagedList;
using PagedList.Mvc;


namespace MyProject01.Controllers
{
    [Authorize]
    public class BooksController : Controller
    {
        private MyProjectDBEntities db = new MyProjectDBEntities();
        // GET: Books
        [AllowAnonymous]
        public ActionResult Index(string searchString, string sortOrder, int ? page)
        {
            // find the books which has not been reserved by any user 
            var show = from t1 in db.Books
                       where !(from t2 in db.Reserve
                               select t2.BookId.ToString()).Contains(t1.BookId.ToString())
                       select t1;
            // find the search string from the results
            if (!String.IsNullOrEmpty(searchString))
            {
                show = show.Where(s => s.Title.Contains(searchString));
            }
            // these ViewBages are the sort parameters, it would help the clients to sort the columns by click the table header
            ViewBag.IDSortParm = String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            ViewBag.BookIdSortParm = sortOrder == "bookId" ? "bookId_desc" : "bookId";
            ViewBag.TitleSortParm = sortOrder == "title" ? "title_desc" : "title";
            ViewBag.CategorySortParm = sortOrder == "category" ? "category_desc" : "category";
            ViewBag.AuthorSortParm = sortOrder == "author" ? "author_desc" : "author";
            ViewBag.PriceSortParm = sortOrder == "price" ? "price_desc" : "price";
            ViewBag.LanguageSortParm = sortOrder == "language" ? "language_desc" : "language";
            
            switch (sortOrder)
            {
                case "id_desc":
                    show = show.OrderByDescending(s => s.ID);
                    break;
                case "bookId_desc":
                    show = show.OrderByDescending(s => s.BookId);
                    break;
                case "bookId":
                    show = show.OrderBy(s => s.BookId);
                    break;
                case "title_desc":
                    show = show.OrderByDescending(s => s.Title);
                    break;
                case "title":
                    show = show.OrderBy(s => s.Title);
                    break;
                case "category_desc":
                    show = show.OrderByDescending(s => s.Category);
                    break;
                case "category":
                    show = show.OrderBy(s => s.Category);
                    break;
                case "author_desc":
                    show = show.OrderByDescending(s => s.Author);
                    break;
                case "author":
                    show = show.OrderBy(s => s.Author);
                    break;
                case "price_desc":
                    show = show.OrderByDescending(s => s.Price);
                    break;
                case "price":
                    show = show.OrderBy(s => s.Price);
                    break;
                case "language_desc":
                    show = show.OrderByDescending(s => s.Language);
                    break;
                case "language":
                    show = show.OrderBy(s => s.Language);
                    break;

                default:
                    show = show.OrderBy(s => s.ID);
                    break;
            }
            // return the view by pagelist(need using pagelist.mvc, and 1 page should have 3 rows)
            return View(show.ToPagedList(page ?? 1, 3));
        }

        public ActionResult Reserve(Guid id)
        {   
            // if id can not be found, then the url should be the bad request.
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // try to find the book
            Books book = db.Books.Find(id);
            // if the book not exist, it means the url is wrong
            if (book == null)
            {
                return HttpNotFound();
            }
            // if this book can be found, the return this <Books>book data to the view
            return View(book);
        }

        public ActionResult Submit(Guid id)
        {
            // send the bookId to this functin, if id is null, the request is bad
            if (id == null)
            {
                
            }
            // get the user cookie, you must make sure this is an available user who want to reserve the book
            HttpCookie userInfo = Request.Cookies["UserInfo"];
            Reserve reserve = new Reserve();
            // set up UserId, BookId, generate a new OrderId and add the information to the database 
            if (userInfo != null)
            {
                reserve.UserId = userInfo["UserId"];
                reserve.BookId = id.ToString();
                reserve.OrderId = Guid.NewGuid().ToString();
                ViewBag.OrderId = reserve.OrderId.ToString();
                db.Reserve.Add(reserve);
                db.SaveChanges();
                return View();
            }
            // if you can not find the user, which means the request is bad
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        public ActionResult Order()
        {
            // this function kind of like the detail to show the books which the current person ordered before
            HttpCookie userInfo = Request.Cookies["UserInfo"];
            string userId = "";
            if (userInfo != null)
            {
                userId = userInfo["UserId"].ToString();
            }

            var books = from m in db.Books
                        select m;

            var reserve = from a in db.Reserve
                          select a;
            // make sure the data is coming from current user Id
            reserve = reserve.Where(s => s.UserId.Contains(userId));
            // show the full data(need info from Books table and orderId from Reserve table) to the customer
            var show =
                from t1 in books
                from t2 in reserve
                where t1.BookId.ToString() == t2.BookId.ToString()
                select new Order
                {
                    ID = t2.ID,
                    BookId = t2.BookId,
                    Title = t1.Title,
                    Category = t1.Category,
                    Author = t1.Author,
                    Price = t1.Price,
                    Language = t1.Language,
                    OrderId = t2.OrderId
                };
            // return the list to this page
            return View(show.OrderBy(m => m.ID).ToList());
        }

        public ActionResult Delete(int? id)
        {
            // showing the details the clients want to delete
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var books = from m in db.Books
                        select m;

            var reserve = from a in db.Reserve
                          select a;
            // same details like the order page, only different is that it would return only 1 row here
            reserve = reserve.Where(s => s.ID.ToString().Contains(id.ToString()));
            
            IQueryable <MyProject01.Models.Order> show =
                from t1 in books
                from t2 in reserve
                where t1.BookId.ToString() == t2.BookId.ToString()
                select new Order
                {
                    ID = t2.ID,
                    BookId = t2.BookId,
                    Title = t1.Title,
                    Category = t1.Category,
                    Author = t1.Author,
                    Price = t1.Price,
                    Language = t1.Language,
                    OrderId = t2.OrderId
                };

            if (show == null)
            {
                return HttpNotFound();
            }

            // becuase it is 1 row result, so we need to change the model from  IQueryable <Order> to Order model by using SingleOrDefault() function
            return View(show.SingleOrDefault());
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            // once the client click delete button. first validate the reserve id
            Reserve reserve = db.Reserve.Find(id);
            if (reserve == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // if this reserve id has been found, then remove it directly
            else
            {
                db.Reserve.Remove(reserve);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
        }
        // memory cleanup function
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}