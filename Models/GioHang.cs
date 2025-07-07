using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NguyenTanDanh.SachOnline.Models
{
    public class GioHang
    {
        private SachOnlineEntities data = new SachOnlineEntities();

        public int MaSach { get; set; }
        public string TenSach { get; set; }
        public string AnhBia { get; set; }
        public decimal DonGia { get; set; }
        public int SoLuong { get; set; }

        // Tính thành tiền
        public decimal ThanhTien => SoLuong * DonGia;

        // Constructor: tạo 1 dòng giỏ hàng từ MaSach
        public GioHang(int maSach)
        {
            var sach = data.SACHes
                .Include("KHUYENMAI")
                .Include("GIATIENs")
                .FirstOrDefault(s => s.MaSach == maSach);

            if (sach != null)
            {
                MaSach = maSach;
                TenSach = sach.TenSach;
                AnhBia = sach.AnhBia;
                SoLuong = 1;

                // Lấy giá mới nhất
                decimal giaGoc = sach.GIATIENs
                    .OrderByDescending(g => g.NgayCapNhat)
                    .FirstOrDefault()?.Gia ?? 0;

                // Kiểm tra khuyến mãi hợp lệ
                var km = sach.KHUYENMAI;
                int giam = (km != null && km.TrangThai && DateTime.Now >= km.NgayBatDau && DateTime.Now <= km.NgayKetThuc)
                    ? km.GiaTriPhanTram ?? 0 : 0;

                // Tính giá sau khi giảm
                DonGia = giaGoc * (1 - giam / 100m);
            }
        }
    }
}
