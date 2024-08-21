using Microsoft.AspNetCore.Mvc;

namespace IMStatus.Controllers;

public class ProjectController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}