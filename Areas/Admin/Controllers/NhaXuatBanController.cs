using System.Linq;
using System.Web.Mvc;
using NguyenTanDanh.SachOnline.Models;

namespace NguyenTanDanh.SachOnline.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class NhaXuatBanController : Controller
    {
        private SachOnlineEntities db = new SachOnlineEntities();

        public ActionResult Index()
        {
            var ds = db.NHAXUATBANs.ToList();
            return View(ds);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(NHAXUATBAN nxb)
        {
            if (ModelState.IsValid)
            {
                db.NHAXUATBANs.Add(nxb);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(nxb);
        }

        public ActionResult Edit(int id)
        {
            var nxb = db.NHAXUATBANs.Find(id);
            if (nxb == null) return HttpNotFound();
            return View(nxb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(NHAXUATBAN nxb)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nxb).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(nxb);
        }

        public ActionResult Delete(int id)
        {
            var nxb = db.NHAXUATBANs.Find(id);
            if (nxb == null) return HttpNotFound();
            return View(nxb);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult ConfirmDelete(int id)
        {
            var nxb = db.NHAXUATBANs.Find(id);
            db.NHAXUATBANs.Remove(nxb);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
