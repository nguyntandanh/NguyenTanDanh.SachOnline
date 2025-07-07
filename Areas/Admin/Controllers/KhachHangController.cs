using System.Linq;
using System.Web.Mvc;
using NguyenTanDanh.SachOnline.Models;

namespace NguyenTanDanh.SachOnline.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class KhachHangController : Controller
    {
        SachOnlineEntities db = new SachOnlineEntities();

        public ActionResult Index()
        {
            var khachHangs = db.KHACHHANGs.ToList();
            return View(khachHangs);
        }

        public ActionResult Details(int id)
        {
            var kh = db.KHACHHANGs.FirstOrDefault(k => k.MaKH == id);
            if (kh == null) return HttpNotFound();
            return View(kh);
        }

        public ActionResult Edit(int id)
        {
            var kh = db.KHACHHANGs.FirstOrDefault(k => k.MaKH == id);
            if (kh == null) return HttpNotFound();
            return View(kh);
        }

        [HttpPost]
        public ActionResult Edit(KHACHHANG khachhang)
        {
            try
            {
                var kh = db.KHACHHANGs.FirstOrDefault(k => k.MaKH == khachhang.MaKH);
                if (kh == null) return HttpNotFound();

                kh.HoTen = khachhang.HoTen;
                kh.DiaChi = khachhang.DiaChi;
                kh.DienThoai = khachhang.DienThoai;
                kh.Email = khachhang.Email;
                kh.TaiKhoan = khachhang.TaiKhoan;
                kh.MatKhau = khachhang.MatKhau;
                kh.LoaiTaiKhoan = khachhang.LoaiTaiKhoan;

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                foreach (var eve in ex.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        ModelState.AddModelError(ve.PropertyName, ve.ErrorMessage);
                    }
                }
                return View(khachhang);
            }
        }


        public ActionResult Delete(int id)
        {
            var kh = db.KHACHHANGs.FirstOrDefault(k => k.MaKH == id);
            if (kh == null) return HttpNotFound();
            return View(kh);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var kh = db.KHACHHANGs.FirstOrDefault(k => k.MaKH == id);
            if (kh == null) return HttpNotFound();
            db.KHACHHANGs.Remove(kh);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
