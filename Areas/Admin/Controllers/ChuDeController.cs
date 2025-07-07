using System.Linq;
using System.Web.Mvc;
using NguyenTanDanh.SachOnline.Models;

namespace NguyenTanDanh.SachOnline.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class ChuDeController : Controller
    {
        SachOnlineEntities db = new SachOnlineEntities();

        public ActionResult Index()
        {
            return View(db.CHUDEs.ToList());
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(CHUDE cd)
        {
            if (ModelState.IsValid)
            {
                db.CHUDEs.Add(cd);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cd);
        }

        public ActionResult Edit(int id)
        {
            var cd = db.CHUDEs.Find(id);
            return View(cd);
        }

        [HttpPost]
        public ActionResult Edit(CHUDE cd)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cd).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cd);
        }

        public ActionResult Delete(int id)
        {
            var cd = db.CHUDEs.Find(id);
            return View(cd);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var cd = db.CHUDEs.Find(id);
            db.CHUDEs.Remove(cd);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
