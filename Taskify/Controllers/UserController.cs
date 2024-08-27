using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Taskify.Context;
using Taskify.Models;
using System.Web.Security;
using System.Security.Cryptography;
using System.Text;


namespace Taskify.Controllers
{
    [AllowAnonymous]

    public class UserController : Controller
    {
        private readonly TaskifyDbContext _context = new TaskifyDbContext();

        //KAYIT OL
        [AllowAnonymous]

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        [AllowAnonymous]

        [HttpPost]
        public JsonResult Register(User model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false});
            }

            try
            {
                var existingUser = _context.Users.FirstOrDefault(u => u.Email == model.Email);
                if (existingUser != null)
                {
                    return Json(new { success = false});
                }
                model.Password = HashPassword(model.Password);
                _context.Users.Add(model);
                _context.SaveChanges();

                return Json(new { success = true, redirectUrl = Url.Action("Login", "User") });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }





        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }

        //GİRİŞ YAP

        [AllowAnonymous]

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [AllowAnonymous]

        [HttpPost]
        public JsonResult Login(LoginUser model)
        {
           

            if (!ModelState.IsValid)
            {
                return Json(new { success = false});
            }

            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);

                if (user == null || !VerifyPassword(user.Password, model.Password))
                {
                    return Json(new { success = false });
                }

                var authTicket = new FormsAuthenticationTicket(
              1, // Ticket sürümü
              user.Email, // Kullanıcı adı
              DateTime.Now, // Oluşturulma zamanı
              DateTime.Now.AddMinutes(30), // Bitiş zamanı
              false, // Kalıcı değil
              user.Id.ToString(), // UserData olarak kullanıcı ID'sini string olarak saklıyoruz
              FormsAuthentication.FormsCookiePath // Cookie yolu
          );

                string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                Response.Cookies.Add(authCookie);


                return Json(new { success = true,  redirectUrl = Url.Action("Index", "Duties") });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }


        [HttpPost]
        public JsonResult Logout()
        {
            try
            {
                FormsAuthentication.SignOut();

                HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, string.Empty)
                {
                    Expires = DateTime.Now.AddDays(-1)
                };
                Response.Cookies.Add(authCookie);

                return Json(new { success = true, redirectUrl = Url.Action("Login", "User") }); 
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }


        //KRİPTOLAMA METODLARI
        public static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Şifre boş olamaz.", nameof(password));
            }

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder sb = new StringBuilder();
                foreach (byte b in bytes)
                {
                    sb.Append(b.ToString("x2")); 
                }
                return sb.ToString();
            }
        }

        public static bool VerifyPassword(string hashedPassword, string plainPassword)
        {
            if (string.IsNullOrEmpty(hashedPassword))
            {
                throw new ArgumentException("Kriptolanmış şifre boş olamaz.", nameof(hashedPassword));
            }

            if (string.IsNullOrEmpty(plainPassword))
            {
                throw new ArgumentException("Şifre boş olamaz.", nameof(plainPassword));
            }

            string hashOfInput = HashPassword(plainPassword);
            return hashOfInput.Equals(hashedPassword, StringComparison.OrdinalIgnoreCase);
        }

        public class LoginUser
        {
            public string Email { get; set; }
            public string Password { get; set; }

        }
    }
}