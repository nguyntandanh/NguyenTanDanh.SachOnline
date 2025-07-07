using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using NguyenTanDanh.SachOnline.Models;

namespace NguyenTanDanh.SachOnline.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class KhuyenMaiController : Controller
    {
        private SachOnlineEntities db = new SachOnlineEntities();

        public ActionResult Index()
        {
            var km = db.KHUYENMAIs.ToList();
            return View(km);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(KHUYENMAI km)
        {
            if (ModelState.IsValid)
            {
                db.KHUYENMAIs.Add(km);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(km);
        }

        public ActionResult Edit(int id)
        {
            var km = db.KHUYENMAIs.Find(id);
            if (km == null) return HttpNotFound();
            return View(km);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(KHUYENMAI km)
        {
            if (ModelState.IsValid)
            {
                var kmInDb = db.KHUYENMAIs.FirstOrDefault(k => k.MaKM == km.MaKM);
                if (kmInDb == null)
                    return HttpNotFound();

                // Cập nhật thủ công từng trường
                kmInDb.TenKM = km.TenKM;
                kmInDb.MoTa = km.MoTa;
                kmInDb.GiaTriPhanTram = km.GiaTriPhanTram;
                kmInDb.NgayBatDau = km.NgayBatDau;
                kmInDb.NgayKetThuc = km.NgayKetThuc;
                kmInDb.TrangThai = km.TrangThai;

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(km);
        }


        // GET: Xác nhận xóa
        [HttpGet]
        public ActionResult Delete(int id)
        {
            var km = db.KHUYENMAIs.Include(k => k.SACHes).FirstOrDefault(k => k.MaKM == id);
            if (km == null)
                return HttpNotFound();

            // Nếu đang được áp dụng
            if (km.SACHes != null && km.SACHes.Any())
            {
                ViewBag.CanForceDelete = true;
                ViewBag.SachApDung = km.SACHes.Count;
                ModelState.AddModelError("", "Không thể xóa vì khuyến mãi đang áp dụng cho sách.");
            }

            return View(km);
        }

        // POST: Xác nhận xóa nếu không liên kết sách
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmDelete(int id)
        {
            var km = db.KHUYENMAIs.Include(k => k.SACHes).FirstOrDefault(k => k.MaKM == id);
            if (km == null)
                return HttpNotFound();

            if (km.SACHes != null && km.SACHes.Any())
            {
                ModelState.AddModelError("", "Không thể xóa vì khuyến mãi đang được áp dụng cho sách.");
                ViewBag.CanForceDelete = true;
                ViewBag.SachApDung = km.SACHes.Count;
                return View("Delete", km);
            }

            db.KHUYENMAIs.Remove(km);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // POST: Xóa cưỡng bức
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForceDelete(int MaKM)
        {
            var km = db.KHUYENMAIs.Include(k => k.SACHes).FirstOrDefault(k => k.MaKM == MaKM);
            if (km == null) return HttpNotFound();

            // Gỡ khuyến mãi khỏi sách
            foreach (var sach in km.SACHes.ToList())
            {
                sach.MaKM = null;
            }

            db.KHUYENMAIs.Remove(km);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
