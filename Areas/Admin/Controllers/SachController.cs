using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NguyenTanDanh.SachOnline.Models;

namespace NguyenTanDanh.SachOnline.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class SachController : Controller
    {
        private SachOnlineEntities db = new SachOnlineEntities();

        public ActionResult Index()
        {
            var sach = db.SACHes
                .Include(s => s.CHUDE)
                .Include(s => s.NHAXUATBAN)
                .Include(s => s.KHUYENMAI)
                .ToList();
            return View(sach);
        }

        public ActionResult Create()
        {
            ViewBag.MaNXB = new SelectList(db.NHAXUATBANs, "MaNXB", "TenNXB");
            ViewBag.MaCD = new SelectList(db.CHUDEs, "MaCD", "TenChuDe");
            ViewBag.MaKM = new SelectList(db.KHUYENMAIs, "MaKM", "TenKM");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SACH s, HttpPostedFileBase fileUpload, IEnumerable<HttpPostedFileBase> anhPhu)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string imageFolder = Server.MapPath("~/Images");
                    if (!Directory.Exists(imageFolder))
                        Directory.CreateDirectory(imageFolder);

                    // Xử lý ảnh bìa
                    if (fileUpload != null && fileUpload.ContentLength > 0)
                    {
                        string fileName = Path.GetFileName(fileUpload.FileName);
                        string newFileName = Path.GetFileNameWithoutExtension(fileName) + "_" +
                                             Guid.NewGuid().ToString("N").Substring(0, 6) +
                                             Path.GetExtension(fileName);

                        string path = Path.Combine(imageFolder, newFileName);
                        fileUpload.SaveAs(path);
                        s.AnhBia = newFileName;
                    }

                    // Lưu sách trước khi thêm ảnh phụ (để có MaSach)
                    db.SACHes.Add(s);
                    db.SaveChanges();

                    // Xử lý ảnh phụ
                    if (anhPhu != null)
                    {
                        foreach (var img in anhPhu)
                        {
                            if (img != null && img.ContentLength > 0)
                            {
                                string fileName = Path.GetFileName(img.FileName);
                                string newFileName = Path.GetFileNameWithoutExtension(fileName) + "_" +
                                                     Guid.NewGuid().ToString("N").Substring(0, 6) +
                                                     Path.GetExtension(fileName);

                                string path = Path.Combine(imageFolder, newFileName);
                                img.SaveAs(path);

                                db.HINHANHSACHes.Add(new HINHANHSACH
                                {
                                    MaSach = s.MaSach,
                                    DuongDan = newFileName
                                });
                            }
                        }
                        db.SaveChanges();
                    }

                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "❌ Lỗi hệ thống khi thêm sách: " + ex.Message);
            }

            // Gắn lại dropdown nếu có lỗi
            ViewBag.MaNXB = new SelectList(db.NHAXUATBANs, "MaNXB", "TenNXB", s.MaNXB);
            ViewBag.MaCD = new SelectList(db.CHUDEs, "MaCD", "TenChuDe", s.MaCD);
            ViewBag.MaKM = new SelectList(db.KHUYENMAIs, "MaKM", "TenKM");
            return View(s);
        }


        public ActionResult Edit(int id)
        {
            var s = db.SACHes.Include(x => x.HINHANHSACHes).FirstOrDefault(x => x.MaSach == id);
            if (s == null) return HttpNotFound();

            // ✅ Gán lại giá trị mặc định được chọn
            ViewBag.MaNXB = new SelectList(db.NHAXUATBANs, "MaNXB", "TenNXB", s.MaNXB);
            ViewBag.MaCD = new SelectList(db.CHUDEs, "MaCD", "TenChuDe", s.MaCD);
            ViewBag.MaKM = new SelectList(db.KHUYENMAIs, "MaKM", "TenKM", s.MaKM);

            if (s.KHUYENMAI != null)
                ViewBag.GiaTriGiam = s.KHUYENMAI.GiaTriPhanTram;

            return View(s);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SACH s, HttpPostedFileBase fileUpload, IEnumerable<HttpPostedFileBase> anhPhu)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var sach = db.SACHes.Find(s.MaSach);
                    if (sach == null) return HttpNotFound();

                    sach.TenSach = s.TenSach;
                    sach.MoTa = s.MoTa;
                    sach.TacGia = s.TacGia;
                    sach.NgayCapNhat = s.NgayCapNhat;
                    sach.SoLuongBan = s.SoLuongBan;
                    sach.MaCD = s.MaCD;
                    sach.MaNXB = s.MaNXB;
                    sach.MaKM = s.MaKM;

                    string imageFolder = Server.MapPath("~/Images");
                    if (!Directory.Exists(imageFolder))
                        Directory.CreateDirectory(imageFolder);

                    // Ảnh bìa
                    if (fileUpload != null && fileUpload.ContentLength > 0)
                    {
                        string fileName = Path.GetFileName(fileUpload.FileName);
                        string newFileName = Path.GetFileNameWithoutExtension(fileName) + "_" +
                                             Guid.NewGuid().ToString("N").Substring(0, 6) +
                                             Path.GetExtension(fileName);

                        string path = Path.Combine(imageFolder, newFileName);
                        fileUpload.SaveAs(path);
                        sach.AnhBia = newFileName;
                    }

                    // Ảnh phụ
                    if (anhPhu != null)
                    {
                        foreach (var file in anhPhu)
                        {
                            if (file != null && file.ContentLength > 0)
                            {
                                string fileName = Path.GetFileName(file.FileName);
                                string newFileName = Path.GetFileNameWithoutExtension(fileName) + "_" +
                                                     Guid.NewGuid().ToString("N").Substring(0, 6) +
                                                     Path.GetExtension(fileName);

                                string path = Path.Combine(imageFolder, newFileName);

                                // Lưu ảnh mới
                                file.SaveAs(path);

                                // Lưu vào DB
                                db.HINHANHSACHes.Add(new HINHANHSACH
                                {
                                    MaSach = sach.MaSach,
                                    DuongDan = newFileName
                                });
                            }
                        }
                    }

                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "❌ Lỗi hệ thống khi cập nhật sách: " + ex.Message);
            }

            // Gán lại ViewBag nếu có lỗi để hiển thị lại view
            ViewBag.MaNXB = new SelectList(db.NHAXUATBANs, "MaNXB", "TenNXB", s.MaNXB);
            ViewBag.MaCD = new SelectList(db.CHUDEs, "MaCD", "TenChuDe", s.MaCD);
            ViewBag.MaKM = new SelectList(db.KHUYENMAIs, "MaKM", "TenKM");
            return View(s);
        }


        public ActionResult Delete(int id)
        {
            var s = db.SACHes.Find(id);
            if (s == null) return HttpNotFound();
            return View(s);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmDelete(int id)
        {
            var s = db.SACHes.Include(x => x.HINHANHSACHes).FirstOrDefault(x => x.MaSach == id);
            if (s == null) return HttpNotFound();

            if (s.HINHANHSACHes.Any())
                db.HINHANHSACHes.RemoveRange(s.HINHANHSACHes);

            db.SACHes.Remove(s);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}
