using NguyenTanDanh.SachOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Net;
using System.Net.Mail;
using PagedList;
using System.Configuration;
using System.Data.Entity;


namespace NguyenTanDanh.SachOnline.Controllers
{
    public class SachOnlineController : Controller
    {
        SachOnlineEntities data = new SachOnlineEntities();

        // Trang Chủ
        public ActionResult Index(int? page)
        {
            int pageSize = 4;
            int pageNum = page ?? 1;

            var sachMoi = data.SACHes
                .Where(s => s.MaKM != null)
                .Include(s => s.KHUYENMAI)
                .Include(s => s.GIATIENs)
                .OrderByDescending(s => s.NgayCapNhat);

            return View(sachMoi.ToPagedList(pageNum, pageSize));
        }

        //Lấy sách mới
        private List<Models.SACH> LaySachMoi(int count)
        {
            return data.SACHes.OrderByDescending(s => s.NgayCapNhat).Take(count).ToList();
        }


//CATEGORY

        //Tất cả sách mới
        public ActionResult TatCaSachMoi(int? page)
        {
            int pageSize = 8;
            int pageNum = page ?? 1;

            var sachMoi = data.SACHes
                .Include(s => s.KHUYENMAI)
                .Include(s => s.GIATIENs)
                .OrderByDescending(s => s.NgayCapNhat);

            return View(sachMoi.ToPagedList(pageNum, pageSize));
        }



        //Lấy sách bán nhiều
        private List<Models.SACH> LaySachBanNhieu(int count)
        {
            return data.SACHes.OrderByDescending(s => s.SoLuongBan).Take(count).ToList();
        }

        public PartialViewResult SachBanNhieuPartial()
        {
            var list = data.SACHes
                .Include(s => s.KHUYENMAI)
                .Include(s => s.GIATIENs)
                .OrderByDescending(s => s.SoLuongBan)
                .Take(4);

            return PartialView(list);
        }



        //Sách theo chủ đề
        public ActionResult SachTheoChuDe(int id, int? page)
        {
            int pageSize = 4;
            int pageNum = page ?? 1;

            var list = data.SACHes
                .Include(s => s.KHUYENMAI)
                .Include(s => s.GIATIENs)
                .Where(s => s.MaCD == id)
                .OrderByDescending(s => s.NgayCapNhat);

            var chuDe = data.CHUDEs.FirstOrDefault(c => c.MaCD == id);
            ViewBag.TenChuDe = chuDe?.TenChuDe;

            return View(list.ToPagedList(pageNum, pageSize));
        }

        public ActionResult ChuDePartial()
        {
            var listChuDe = from cd in data.CHUDEs select cd;
            return PartialView(listChuDe);
        }



        //Sách theo nhà xuất bản
        public ActionResult SachTheoNXB(int id, int? page)
        {
            int pageSize = 4;
            int pageNum = page ?? 1;

            var list = data.SACHes
                .Include(s => s.KHUYENMAI)
                .Include(s => s.GIATIENs)
                .Where(s => s.MaNXB == id)
                .OrderByDescending(s => s.NgayCapNhat);

            var nxb = data.NHAXUATBANs.FirstOrDefault(n => n.MaNXB == id);
            ViewBag.TenNXB = nxb?.TenNXB;

            return View(list.ToPagedList(pageNum, pageSize));
        }

        public ActionResult NhaXuatBanPartial()
        {
            var listNXB = data.NHAXUATBANs.ToList();
            return PartialView(listNXB);
        }



        //chi tiết sách
        public ActionResult ChiTietSach(int id)
        {
            var sach = data.SACHes
                .Include(s => s.HINHANHSACHes)
                .Include(s => s.NHAXUATBAN)
                .Include(s => s.KHUYENMAI)
                .Include(s => s.GIATIENs)
                .FirstOrDefault(s => s.MaSach == id);

            if (sach == null)
            {
                return HttpNotFound();
            }
            return View(sach);
        }



        //mục tìm kiếm sách
        public ActionResult TimKiem(string q, int? page)
        {
            int pageSize = 4;
            int pageNum = page ?? 1;

            var ketQua = data.SACHes
                .Include(s => s.KHUYENMAI)
                .Include(s => s.GIATIENs)
                .Where(s => s.TenSach.Contains(q))
                .OrderByDescending(s => s.NgayCapNhat);

            ViewBag.TuKhoa = q;
            return View(ketQua.ToPagedList(pageNum, pageSize));
        }


// TÀI KHOẢN

        //GET: DangKy
        [HttpGet]
        public ActionResult DangKy()
        {
            return View();
        }

        //POST:DangKy
        [HttpPost]
        public ActionResult DangKy(KHACHHANG kh)
        {
            if (ModelState.IsValid)
            {
                var existing = data.KHACHHANGs.FirstOrDefault(x => x.TaiKhoan == kh.TaiKhoan);
                if (existing != null)
                {
                    ViewBag.ThongBao = "Tài khoản đã tồn tại!";
                    return View();
                }

                data.KHACHHANGs.Add(kh);
                data.SaveChanges();

                ViewBag.ThongBao = "Đăng ký thành công!";
                ModelState.Clear();
            }
            return View();
        }



        //GET:DangNhap
        [HttpGet]
        public ActionResult DangNhap()
        {
            return View();
        }

        //POST:DangNhap
        [HttpPost]
        public ActionResult DangNhap(string TaiKhoan, string MatKhau)
        {
            var kh = data.KHACHHANGs.FirstOrDefault(k => k.TaiKhoan == TaiKhoan && k.MatKhau == MatKhau);
            if (kh != null)
            {
                ViewBag.ThongBao = "Đăng nhập thành công!";
                Session["TaiKhoan"] = kh;
                if (kh.LoaiTaiKhoan == "admin")
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                else
                    return RedirectToAction("Index", "SachOnline");

            }
            ViewBag.ThongBao = "Tài khoản hoặc mật khẩu không đúng!";
            return View();
        }



        //Đăng xuất
        public ActionResult DangXuat()
        {
            Session["TaiKhoan"] = null;
            return RedirectToAction("Index", "SachOnline");
        }



// GIỎ HÀNG

        //Mục giỏ hàng
        public ActionResult GioHang()
        {
            List<GioHang> gioHang = LayGioHang();
            ViewBag.TongSoLuong = gioHang.Sum(s => s.SoLuong);
            ViewBag.TongTien = gioHang.Sum(s => s.ThanhTien);
            return View(gioHang);
        }



        //Lấy Giỏ Hàng
        public List<GioHang> LayGioHang()
        {
            List<GioHang> gioHang = Session["GioHang"] as List<GioHang>;
            if (gioHang == null)
            {
                gioHang = new List<GioHang>();
                Session["GioHang"] = gioHang;
            }
            return gioHang;
        }



        //Thêm giỏ hàng
        public ActionResult ThemGioHang(int id, string returnUrl)
        {
            List<GioHang> gioHang = LayGioHang();
            GioHang sp = gioHang.Find(s => s.MaSach == id);
            if (sp == null)
            {
                sp = new GioHang(id);
                gioHang.Add(sp);
            }
            else
            {
                sp.SoLuong++;
            }
            return Redirect(returnUrl);
        }
        


        //Cập nhật giỏ hàng
        [HttpPost]
        public ActionResult CapNhatGioHang(int MaSach, int SoLuong)
        {
            var gioHang = Session["GioHang"] as List<GioHang>;
            if (gioHang != null)
            {
                var item = gioHang.FirstOrDefault(g => g.MaSach == MaSach);
                if (item != null && SoLuong > 0)
                {
                    item.SoLuong = SoLuong;
                }
            }

            ViewBag.TongSoLuong = gioHang?.Sum(g => g.SoLuong) ?? 0;
            ViewBag.TongTien = gioHang?.Sum(g => g.ThanhTien) ?? 0;

            return RedirectToAction("GioHang");
        }



        //Xóa giỏ hàng
        public ActionResult XoaGioHang()
        {
            Session["GioHang"] = null;
            return RedirectToAction("GioHang");
        }

        public ActionResult GioHangPartial()
        {
            List<GioHang> gioHang = Session["GioHang"] as List<GioHang>;
            if (gioHang == null)
            {
                gioHang = new List<GioHang>();
            }
            ViewBag.TongSoLuong = gioHang.Sum(g => g.SoLuong);
            ViewBag.TongTien = gioHang.Sum(g => g.ThanhTien);

            return PartialView();
        }



//SẢN PHẨM

        //Xóa Sản Phẩm
        public ActionResult XoaSanPham(int id)
        {
            List<GioHang> gioHang = Session["GioHang"] as List<GioHang>;
            if (gioHang != null)
            {
                var item = gioHang.SingleOrDefault(s => s.MaSach == id);
                if (item != null)
                {
                    gioHang.Remove(item);
                }
            }

            if (gioHang == null || !gioHang.Any())
            {
                Session["GioHang"] = null;
            }

            return RedirectToAction("GioHang");
        }



//ĐẶT HÀNG

        //POST:DATHANG
        [HttpPost]
        public ActionResult DatHang()
        {
            if (Session["TaiKhoan"] == null)
            {
                return RedirectToAction("DangNhap", "SachOnline");
            }

            var kh = (KHACHHANG)Session["TaiKhoan"];
            var gioHang = LayGioHang();

            if (gioHang == null || !gioHang.Any())
            {
                return RedirectToAction("GioHang");
            }

            DONDATHANG ddh = new DONDATHANG
            {
                MaKH = kh.MaKH,
                NgayDat = DateTime.Now,
                TinhTrangGiaoHang = false,
                DaThanhToan = false
            };

            data.DONDATHANGs.Add(ddh);
            data.SaveChanges();

            foreach (var item in gioHang)
            {
                CTDATHANG ct = new CTDATHANG
                {
                    MaDonHang = ddh.MaDonHang,
                    MaSach = item.MaSach,
                    SoLuong = item.SoLuong,
                    DonGia = item.DonGia
                };
                data.CTDATHANGs.Add(ct);
            }

            data.SaveChanges();

            Session["GioHang"] = null; // Xóa giỏ hàng sau khi đặt hàng

            TempData["ThongBao"] = "Đặt hàng thành công!";
            return RedirectToAction("XacNhanDonHang");
        }



        //POST:XACNHANDONHANG
        [HttpPost]
        public ActionResult XacNhanDonHang(string HoTen, string DiaChi, string DienThoai, DateTime NgayGiao)
        {
            if (Session["TaiKhoan"] == null)
            {
                return RedirectToAction("DangNhap", "SachOnline");
            }

            var kh = (KHACHHANG)Session["TaiKhoan"];
            var gioHang = Session["GioHang"] as List<GioHang>;
            if (gioHang == null || !gioHang.Any())
            {
                return RedirectToAction("GioHang");
            }

            DONDATHANG dh = new DONDATHANG
            {
                MaKH = kh.MaKH,
                NgayDat = DateTime.Now,
                NgayGiao = NgayGiao,
                TinhTrangGiaoHang = false,
                DaThanhToan = false
            };

            data.DONDATHANGs.Add(dh);
            data.SaveChanges();

            // Lưu chi tiết từng sản phẩm
            foreach (var item in gioHang)
            {
                CTDATHANG ct = new CTDATHANG
                {
                    MaDonHang = dh.MaDonHang,
                    MaSach = item.MaSach,
                    SoLuong = item.SoLuong,
                    DonGia = (decimal)item.DonGia
                };
                data.CTDATHANGs.Add(ct);
            }

            data.SaveChanges();
            try
            {
                string emailNguoiNhan = kh.Email;
                if (!string.IsNullOrEmpty(emailNguoiNhan))
                {
                    MailMessage mail = new MailMessage();
                    mail.From = new MailAddress("nguyentandanh2458@gmail.com", "SáchOnline");
                    mail.To.Add(emailNguoiNhan);
                    mail.Subject = $"[SáchOnline] Xác nhận đơn hàng #{dh.MaDonHang}";
                    mail.Body = $@"
                    Xin chào {HoTen},
                    Bạn đã đặt hàng thành công vào ngày {DateTime.Now:dd/MM/yyyy}.
                    Ngày giao dự kiến: {NgayGiao:dd/MM/yyyy}
                    Địa chỉ: {DiaChi}
                    Tổng số lượng: {gioHang.Sum(g => g.SoLuong)}
                    Tổng tiền: {gioHang.Sum(g => g.ThanhTien):N0} VNĐ
                    Cảm ơn bạn đã mua sắm tại SáchOnline!";
                    mail.IsBodyHtml = false;

                    SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                    smtp.Credentials = new NetworkCredential("nguyentandanh2458@gmail.com", "mwvf gijw ejxq phtq");
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }
            }
            catch (Exception ex)
            {
                return Content("Gửi lỗi: " + ex.Message);
            }
            Session["GioHang"] = null;
            TempData["ThongBaoDatHang"] = "Đơn hàng của bạn đã được đặt thành công!";

            return RedirectToAction("ThongBaoDonHang");
        }



        //Thông báo đơn hàng
        public ActionResult ThongBaoDonHang()
        {
            return View();
        }


// THANH TOÁN

        //Mục thanh toán VnPay
        [HttpPost]
        public ActionResult PaymentVnPay(int? amount)
        {
            if (amount == null || amount <= 0)
                return RedirectToAction("GioHang");

            int actualAmount = amount.Value;

            string vnpUrl = ConfigurationManager.AppSettings["vnp_Url"];
            string returnUrl = ConfigurationManager.AppSettings["vnp_ReturnUrl"];
            string tmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"];
            string secretKey = ConfigurationManager.AppSettings["vnp_HashSecret"];

            var vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", "2.1.0");
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", tmnCode);
            vnpay.AddRequestData("vnp_Amount", (actualAmount * 100).ToString());
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Request.UserHostAddress);
            vnpay.AddRequestData("vnp_OrderInfo", $"Thanh toán đơn hàng trên SáchOnline: {actualAmount:N0}");
            vnpay.AddRequestData("vnp_ReturnUrl", returnUrl);
            vnpay.AddRequestData("vnp_TxnRef", DateTime.Now.Ticks.ToString());

            string paymentUrl = vnpay.CreateRequestUrl(vnpUrl, secretKey);
            return Redirect(paymentUrl);
        }



        public ActionResult VnPayReturn()
        {
            var vnpay = new VnPayLibrary();
            foreach (string key in Request.QueryString.Keys)
                if (key.StartsWith("vnp_"))
                    vnpay.AddRequestData(key, Request.QueryString[key]);

            string secureHash = Request.QueryString["vnp_SecureHash"];
            string secretKey = ConfigurationManager.AppSettings["vnp_HashSecret"];
            bool valid = VnPayLibrary.ComputeSha256(secretKey + string.Join("&",
                           vnpay.RequireField.Select(kv => $"{kv.Key}={kv.Value}")))
                         == secureHash;

            if (valid && Request.QueryString["vnp_ResponseCode"] == "00")
            {
                ViewBag.Message = "Thanh toán thành công!";
            }
            else
            {
                ViewBag.Message = "Thanh toán thất bại hoặc không xác thực.";
            }
            return View();
        }


//FORM
        //Mục Slider
        public ActionResult SliderPartial()
        {
            return PartialView();
        }
        //Mục Footer
        public ActionResult FooterPartial()
        {
            return PartialView();
        }

    }
}