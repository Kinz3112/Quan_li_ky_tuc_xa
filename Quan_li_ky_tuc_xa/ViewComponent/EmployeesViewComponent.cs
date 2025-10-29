using Microsoft.AspNetCore.Mvc;
using Quan_li_ky_tuc_xa.Models;
namespace Quan_li_ky_tuc_xa.ViewComponents
{
    public class EmployeesViewComponent : ViewComponent
    {
        public EmployeesViewComponent() { }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View("EmployeesLeftMenu");
        }
    }
}
