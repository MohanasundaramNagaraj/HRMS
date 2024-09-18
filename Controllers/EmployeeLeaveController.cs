using Microsoft.AspNetCore.Mvc;

namespace SparkHRMS.Controllers
{
    public class EmployeeLeaveController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AddLeave()
        {
            return View();
        }

        public IActionResult EditLeave()
        {
            return View();
        }

       
    }
}
