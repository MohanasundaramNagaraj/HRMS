using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SparkHRMS.Data;
using SparkHRMS.Data.Entities;
using SparkHRMS.Data.Masters;
using SparkHRMS.Services;

namespace SparkHRMS.Controllers
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class ActivityController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly UserResolverService _userResolverService;
        private readonly IConfiguration _configuration;
        public ActivityController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IConfiguration configuration, UserResolverService userResolverService)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
            _userResolverService = userResolverService;
        }

        // GET: Activity
        public async Task<IActionResult> Index()    
        {
            return View(await _context.MST_Activities.Where(v=>v.IsActive == true).ToListAsync());
        }
        [HttpPost]
      
        public async Task<string> CreateAsync(MST_Activity activity)
        {
            if(_context.MST_Activities.Any(v=>v.ActivityName == activity.ActivityName && v.IsActive == true) != true)
            {
               
                MST_Activity act = new MST_Activity();
                act.ActivityName = activity.ActivityName;
                act.CreatedDate = DateTime.Now;
                act.IsActive = true;
                act.CreatedBy =(int)_userResolverService.GetUserId();
                _context.MST_Activities.Add(act);
                _context.SaveChanges();

                return "OK";
            }
            else
            {
                return "The Activity Name already in exist"; 
            }
            
            
            
        }
        public string EditData(MST_Activity activity)
        {
            if(activity.ActivityId != null && activity.ActivityId != 0)
            {
                var act = _context.MST_Activities.Where(b => b.ActivityId == activity.ActivityId).FirstOrDefault();
                if(act != null)
                {
                    if (_context.MST_Activities.Any(v => v.ActivityName == activity.ActivityName && v.IsActive == true) != true)
                    {
                        act.ActivityName = activity.ActivityName;
                        act.ModifiedDate = DateTime.Now;
                        act.ModifiedBy=(int)_userResolverService.GetUserId();
                        act.IsActive = true;
                        _context.Update(act);
                        _context.SaveChanges();

                        return "OK";
                    }
                    else
                    {
                        return "The Activity Name already in exist";
                    }

                }
                else
                {
                    return "NotOK";
                }

            }
            else
            {
                return "NotOK";
            }
            



        }
        [HttpPost]
        public string DeleteData(int id)
        {
            var act = _context.MST_Activities.FirstOrDefault(b => b.ActivityId == id);
            if (act != null)
            {
                act.ModifiedDate = DateTime.Now;
                act.IsActive = false;
                act.ModifiedBy = _userResolverService.GetUserId();

                _context.Update(act);
                _context.SaveChanges();

                return "OK";
            }

            return "NotOK";
        }

        [HttpGet]
        public IActionResult GetEditData(int id)
        {
            var activity = _context.MST_Activities
                .Where(a => a.ActivityId == id)
                .Select(a => new {
                    activityId = a.ActivityId,
                    activityName = a.ActivityName
                }).FirstOrDefault();

            if (activity == null)
                return NotFound();

            return Json(activity);
        }
    }
}
