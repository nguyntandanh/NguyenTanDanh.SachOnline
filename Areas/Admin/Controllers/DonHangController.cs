using System.Linq;
using System.Web.Mvc;
using NguyenTanDanh.SachOnline.Models;

namespace NguyenTanDanh.SachOnline.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class DonHangController : Controller
    {
        SachOnlineEntities db = new SachOnlineEntities();

        public ActionResult Index()
        {
            var donhangs = db.DONDATHANGs.OrderByDescending(d => d.NgayDat).ToList();
            return View(donhangs);
        }

        public ActionResult Details(int id)
        {
            var donhang = db.DONDATHANGs.FirstOrDefault(d => d.MaDonHang == id);
            if (donhang == null) return HttpNotFound();

            ViewBag.ChiTiet = db.CTDATHANGs.Where(c => c.MaDonHang == id).ToList();
            return View(donhang);
        }

        public ActionResult Delete(int id)
        {
            var donhang = db.DONDATHANGs.FirstOrDefault(d => d.MaDonHang == id);
            if (donhang == null) return HttpNotFound();
            return View(donhang);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var cts = db.CTDATHANGs.Where(c => c.MaDonHang == id).ToList();
            foreach (var ct in cts)
            {
                db.CTDATHANGs.Remove(ct);
            }

            var dh = db.DONDATHANGs.FirstOrDefault(d => d.MaDonHang == id);
            db.DONDATHANGs.Remove(dh);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
