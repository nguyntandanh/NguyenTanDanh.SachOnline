using System.Web;
using System.Web.Mvc;
using NguyenTanDanh.SachOnline.Models;

public class AdminAuthorizeAttribute : AuthorizeAttribute
{
    protected override bool AuthorizeCore(HttpContextBase httpContext)
    {
        var user = httpContext.Session["TaiKhoan"] as KHACHHANG;
        if (user != null && user.LoaiTaiKhoan == "admin")
            return true;
        return false;
    }

    protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
    {
        filterContext.Result = new RedirectResult("~/SachOnline/DangNhap");
    }
}
