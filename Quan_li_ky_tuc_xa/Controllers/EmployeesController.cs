using Microsoft.AspNetCore.Mvc;

namespace Quan_li_ky_tuc_xa.Controllers
{
    public class EmployeesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
