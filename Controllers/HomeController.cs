using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SparkHRMS.Data.Entities;
using SparkHRMS.Data;
using SparkHRMS.Models;
using SparkHRMS.ViewModels;
using System.Diagnostics;
using System.Globalization;
using SparkHRMS.Utilities;
using static System.Runtime.InteropServices.JavaScript.JSType;
//using Hangfire;
using SparkHRMS.Interfaces;

namespace SparkHRMS.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
       // private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IEmailSender _emailService;

      
        public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, ApplicationDbContext context, 
           // IBackgroundJobClient backgroundJobClient,
            IEmailSender emailService)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
           // _backgroundJobClient = backgroundJobClient;
            _emailService = emailService;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        public async Task<IActionResult> Dashboard(DateTime? date = null)
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //public IActionResult ScheduleEmail()
        //{
        //    _backgroundJobClient.Enqueue(() => _emailService.SendEmailAsync("recipient@domain.com", "Scheduled Email", "This is a test email."));

        //    return Ok("Email scheduled.");
        //}
    }
}
