using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MyProject01.Models;

namespace MyProject01.Controllers
{
    public class AccountController : Controller
    {
        MyProjectDBEntities entity = new MyProjectDBEntities();

        // GET: Account
        public ActionResult Login()
        {
            return View();
        }
        public ActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel credentials)
        {
            // check if user exist, and put the user information to UserTable u
            bool userExist = entity.UserTable.Any
                (x => x.Email == credentials.Email && x.Passcode == credentials.Password);
            UserTable u = entity.UserTable.FirstOrDefault
                (x => x.Email == credentials.Email && x.Passcode == credentials.Password);
            // if database find the user, set up cookies, I set up another cookie for getting UserId from another controller
            if (userExist)
            {
                FormsAuthentication.SetAuthCookie(u.Username, false);

                HttpCookie cookie = new HttpCookie("UserInfo");
                cookie["Name"] = u.Username;
                cookie["Email"] = u.Email;
                cookie["UserId"] = u.UserId.ToString();
                Response.Cookies.Add(cookie);

                return RedirectToAction("Index" , "Home");   
            }
            // if not, return the error message to the same view 
            ModelState.AddModelError("", "Username or password is wrong");
            return View();
        }

        [HttpPost]
        public ActionResult Signup(UserTable userinfo)
        {
            // find if this email has been signed up or not
            bool userExist = entity.UserTable.Any(x => x.Email == userinfo.Email);
            // if user exist, tell the client that this email has been registered
            if (userExist)
            {
                ModelState.AddModelError("", "This Email has been signed up already.");
                return View();
            }
            // if not, update the UserTable and add this user info(after validation), transfer the page to login
            else
            {
                entity.UserTable.Add(userinfo);
                entity.SaveChanges();
                return RedirectToAction("Login");
            }
            
        }

        public ActionResult Logout()
        {
            // click logout then connect to login page 
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
    }
}