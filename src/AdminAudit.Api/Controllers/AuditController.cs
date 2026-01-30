using Microsoft.AspNetCore.Mvc;

namespace AdminAudit.Api.Controllers
{
    public class AuditController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
