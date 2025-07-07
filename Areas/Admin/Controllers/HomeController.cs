using System.Web.Mvc;
using NguyenTanDanh.SachOnline.Models;

namespace NguyenTanDanh.SachOnline.Areas.Admin.Controllers
{
    [AdminAuthorize] 
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
