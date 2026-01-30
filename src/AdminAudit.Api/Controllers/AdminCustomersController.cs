using Microsoft.AspNetCore.Mvc;

namespace AdminAudit.Api.Controllers
{
    public class AdminCustomersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
