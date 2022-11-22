using Microsoft.AspNetCore.Mvc;

namespace test.Areas.AdminPanel.Controllers
{
    public class DashboardController : BaseController
    {        
        public IActionResult Index()
        {
            return View();
        }
    }
}
