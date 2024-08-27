using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Taskify.Context;
using Taskify.Models;

namespace Taskify.Controllers
{
    [Authorize]
    public class DutiesController : Controller
    {
        private readonly TaskifyDbContext _context = new TaskifyDbContext();

        //GÖREVLERİ GETİR
        public ActionResult Index(DutyStatus? status, string search)
        {
            var authCookie = HttpContext.Request.Cookies[FormsAuthentication.FormsCookieName];
            int userId = 0;

            if (authCookie != null)
            {
                var authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                int.TryParse(authTicket.UserData, out userId); // UserData'dan int'e dönüştürme
            }

            // Kullanıcı ID'sine göre görevleri filtreleyin
            var duties = _context.Duties
                .Where(d => d.Active && d.Archived == false && d.UserId == userId)
                .AsQueryable();

            // Status'a göre filtreleme
            if (status.HasValue)
                {
                    duties = duties.Where(t => t.Status == status.Value);
                }

                // DutyName ve Description'a göre arama
                if (!string.IsNullOrEmpty(search))
                {
                    duties = duties.Where(t => t.DutyName.Contains(search) || t.Description.Contains(search));
                }

                return View(duties.ToList());
            
         
        }


        // GÖREV EKLE
        [HttpPost]
        public JsonResult Create(Duty model)
        {
            if (ModelState.IsValid)
            {
                var authCookie = HttpContext.Request.Cookies[FormsAuthentication.FormsCookieName];
                int userId = 0;

                if (authCookie != null)
                {
                    var authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                    int.TryParse(authTicket.UserData, out userId); // UserData'dan int'e dönüştürme
                }

                _context.Duties.Add(model);
                model.Active = true;
                model.UserId = userId;
                _context.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }


        // GÖREVİN DETAYINI GETİR
        [HttpGet]
        public JsonResult Details(int id)
        {
            if (id == null)
            {
                return Json(new { success = false}, JsonRequestBehavior.AllowGet);
            }
            var duty = _context.Duties.Find(id);
            if (duty == null)
            {
                return Json(new { success = false}, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {
                Id = duty.Id,
                DutyName = duty.DutyName,
                Description = duty.Description,
                Status = (int)duty.Status, 
                DueDate = duty.DueDate
            }, JsonRequestBehavior.AllowGet);
        }


        //GÖREVİ GÜNCELLE
        [HttpGet]
        public JsonResult Edit(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false}, JsonRequestBehavior.AllowGet);
            }
            var duty = _context.Duties.Find(id);
            if (duty == null)
            {
                return Json(new { success = false}, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {
                Id = duty.Id,
                DutyName = duty.DutyName,
                Description = duty.Description,
                Status = (int)duty.Status, 
                DueDate = duty.DueDate
            }, JsonRequestBehavior.AllowGet);
        }

        //GÖREVİ GÜNCELLE
        [HttpPost]
        public JsonResult Edit(Duty model)
        {
            if (ModelState.IsValid)
            {
                var authCookie = HttpContext.Request.Cookies[FormsAuthentication.FormsCookieName];
                int userId = 0;

                if (authCookie != null)
                {
                    var authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                    int.TryParse(authTicket.UserData, out userId); // UserData'dan int'e dönüştürme
                }

                model.Active = true;
                model.UserId = userId;
                _context.Entry(model).State = EntityState.Modified;

                _context.SaveChanges(); 
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        

        //GÖREVİ SİL

        [HttpPost]
        public JsonResult Delete(int id)
        {
            var duty = _context.Duties.Find(id);
            if (duty == null)
            {
                return Json(new { success = false  });
            }

            duty.Active = false;
            _context.Entry(duty).State = EntityState.Modified;
            
            _context.SaveChanges();

            return Json(new { success = true});
        }


        


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}