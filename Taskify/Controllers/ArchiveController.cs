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
    public class ArchiveController : Controller
    {
        private readonly TaskifyDbContext _context = new TaskifyDbContext();
      
        //ARŞİVLENMİŞ GÖREVLERİ GETİR
        public ActionResult Index(DutyStatus? status, string search)
        {
            var authCookie = HttpContext.Request.Cookies[FormsAuthentication.FormsCookieName];
            int userId = 0;

            if (authCookie != null)
            {
                var authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                int.TryParse(authTicket.UserData, out userId); 
            }

            var duties = _context.Duties
                .Where(d => d.Active == true && d.Archived == true && d.UserId == userId)
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


        //GÖREVİ ARŞİVLE

        [HttpPost]
        public JsonResult Archive(int id)
        {
            var duty = _context.Duties.Find(id);
            if (duty == null)
            {
                return Json(new { success = false });
            }

            duty.Archived = true;
            _context.Entry(duty).State = EntityState.Modified;

            _context.SaveChanges();

            return Json(new { success = true });
        }


        // GÖREVİ ARŞİVDEN ÇIKAR
        [HttpPost]
        public JsonResult Unarchive(int id)
        {
            var duty = _context.Duties.Find(id);
            if (duty == null)
            {
                return Json(new { success = false});
            }

            duty.Archived = false;
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