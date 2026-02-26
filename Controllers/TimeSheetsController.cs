using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SparkHRMS.Data.Entities;
using SparkHRMS.Data;
using SparkHRMS.Data.Setting;
using SparkHRMS.ViewModels;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Org.BouncyCastle.Pqc.Crypto.Falcon;

namespace SparkHRMS.Controllers
{
    [Authorize(Roles = "SuperAdmin,Admin,Employee")]
    public class TimeSheetsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public TimeSheetsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
        }
        // GET: TimeSheetsController
        public ActionResult Index()
        {
            return View();
        }
        public async Task<ActionResult> Report(int? EmployeeID, DateTime? FromDate, DateTime? ToDate)
        {
            var emp = new Employee();
            if (EmployeeID == null)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return NotFound();
                emp = _context.Employees.Where(x => x.ApplicationUserId == user.Id).FirstOrDefault();

                if (emp == null)
                {
                    emp = _context.Employees.FirstOrDefault();
                }
                EmployeeID = emp.EmployeeId;
            }
            else
            {
                emp = _context.Employees.Where(x => x.EmployeeId == EmployeeID).FirstOrDefault();
            }

            var employeeDetails = new EmployeeDetailsDto
            {
                EmployeeId = emp.EmployeeId,
                Name = emp.Name,
                PhoneNumber = emp.PhoneNumber,
                Email = emp.Email,
                ImageUrl = emp.ImageUrl,
                DateOfJoining = emp.DateOfJoining,
                Address = emp.Address,
                Designation = emp.Designation,
                EmpCode = emp.EmployeeCode
            };

            var today = DateTime.Today;

            var startOfMonth = new DateTime(today.Year, today.Month, 1);

            if (!FromDate.HasValue)
            {
                FromDate = new DateTime(today.Year, today.Month, 1);
            }

            if (!ToDate.HasValue)
            {
                ToDate = startOfMonth.AddMonths(1).AddDays(-1);
            }

            ViewBag.EmployeeList = _context.Employees.ToList();
            ViewBag.YearList = _context.Year.ToList();
            ViewBag.SelectedEmployeeID = employeeDetails.EmployeeId;

            ViewBag.SelectedFromDate = FromDate;
            ViewBag.SelectedToDate = ToDate;

            ViewBag.Activities = getActivityData();

            var timeSheetRecords = await _context.Timesheets
                                    .Where(t => t.Date >= FromDate && t.Date <= ToDate && t.EmployeeId == EmployeeID && !t.IsDeleted)
                                    .OrderBy(x => x.Date)
                                    .ToListAsync();

            var response = new EmployeeTimeSheetViewModel
            {
                EmployeeDetails = employeeDetails,
                TimeSheetRecords = timeSheetRecords
            };
            return View(response);
        }

        // GET: TimeSheetsController/Create
        public async Task<ActionResult> EntryAsync(int? EmployeeID, int? Month, int? Year)
        {
            var emp = new Employee();
            if (EmployeeID == null)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return NotFound();
                emp = _context.Employees.Where(x => x.ApplicationUserId == user.Id).FirstOrDefault();

                if (emp == null)
                {
                    emp = _context.Employees.FirstOrDefault();
                }
                EmployeeID = emp.EmployeeId;
            }
            else
            {
                emp = _context.Employees.Where(x => x.EmployeeId == EmployeeID).FirstOrDefault();
            }

            // Fetch Employee Details
            var employeeDetails = new EmployeeDetailsDto
            {
                EmployeeId = emp.EmployeeId,
                Name = emp.Name,
                PhoneNumber = emp.PhoneNumber,
                Email = emp.Email,
                ImageUrl = emp.ImageUrl,
                DateOfJoining = emp.DateOfJoining,
                Address = emp.Address,
                Designation = emp.Designation,
                EmpCode = emp.EmployeeCode
            };

            var today = DateTime.Today;
            var yearid = _context.Year.Where(x => x.Year == today.Year).Select(x => x.Id).FirstOrDefault();
            var year = Year.HasValue ? (int)Year : yearid;

            var startOfMonth = Month.HasValue ? new DateTime(year, Month.Value, 1) : new DateTime(today.Year, today.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            ViewBag.EmployeeList = _context.Employees.ToList();
            ViewBag.YearList = _context.Year.ToList();
            ViewBag.SelectedEmployeeID = employeeDetails.EmployeeId;
            ViewBag.SelectedMonth = startOfMonth.Month;
            ViewBag.SelectedYear = startOfMonth.Year;
            ViewBag.Activities = getActivityData();
            ViewBag.ItemType = _context.WorkItemType.Where(v => v.IsActive == true).Select(c => c.WorkItemTypeName).ToList();
            //if(EmployeeID == null)
            //{
            //    EmployeeID = _context.Employees.FirstOrDefault().EmployeeId;
            //}

            var timeSheetRecords = await _context.Timesheets
                                    .Where(t => t.MonthId == startOfMonth.Month && t.YearId == year && t.EmployeeId == EmployeeID && !t.IsDeleted)
                                    .OrderBy(x=>x.Date)
                                    .ToListAsync();

            var response = new EmployeeTimeSheetViewModel
            {
                EmployeeDetails = employeeDetails,
                TimeSheetRecords = timeSheetRecords
            };

            return View(response);
        }

        [HttpPost]
        public async Task Update(string timesheet)
        {
            try
            {
                var updatedTimesheets = JsonConvert.DeserializeObject<List<Timesheet>>(timesheet);
                foreach(var updatedTimesheet in updatedTimesheets)
                {
                    if (updatedTimesheet.EmployeeId == 0)
                    {
                        var user = await _userManager.GetUserAsync(User);

                        var emp = _context.Employees.Where(x => x.ApplicationUserId == user.Id).FirstOrDefault();
                        updatedTimesheet.EmployeeId = emp.EmployeeId;
                    }
                    var uniqueId = updatedTimesheet.UniqueId;

                    var yearid = updatedTimesheet.YearId;
                    var isExistingTimesheet = _context.Timesheets.Where(x => x.UniqueId == uniqueId).Any();

                    if (!isExistingTimesheet)
                    {
                        updatedTimesheet.YearId = yearid;
                        _context.Timesheets.Add(updatedTimesheet);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        var existingTimesheet = _context.Timesheets.Where(x => x.UniqueId == uniqueId).FirstOrDefault();
                        if (existingTimesheet != null)
                        {
                            existingTimesheet.YearId = yearid;
                            existingTimesheet.MonthId = updatedTimesheet.MonthId;
                            existingTimesheet.Date = updatedTimesheet.Date;
                            existingTimesheet.Day = updatedTimesheet.Day;
                            existingTimesheet.EmployeeId = updatedTimesheet.EmployeeId;
                            existingTimesheet.TaskType = updatedTimesheet.TaskType;
                            existingTimesheet.Task = updatedTimesheet.Task;
                            existingTimesheet.Activity = updatedTimesheet.Activity;
                            existingTimesheet.Descreption = updatedTimesheet.Descreption;
                            existingTimesheet.HoursWorked = updatedTimesheet.HoursWorked;

                            _context.Entry(existingTimesheet).State = EntityState.Modified;
                            await _context.SaveChangesAsync();
                        }
                        else
                        {
                            throw new Exception("Timesheet not found!");
                        }
                    }
                }
                
            
            }
  
            catch(Exception ex)
            {
                throw ex.InnerException;
            }
        }
        [HttpPost]
        public async Task Delete(string uniqueId)
        {
            var timesheet = _context.Timesheets.Where(x => x.UniqueId == uniqueId).FirstOrDefault();
            if (timesheet != null)
            {
                timesheet.IsDeleted = true;
                await _context.SaveChangesAsync();
            }
            //else
            //{
            //    return null;
            //    //throw new Exception("Timesheet not found!");
            //}
        }
        private List<string> getActivityData()
        {
            List<string> acts = new List<string>();
            acts = _context.MST_Activities.Where(v=>v.IsActive == true).Select(c=>c.ActivityName).ToList();
            return acts;
        }

        public async Task<ActionResult> AttendanceTimeSheetSummary(int? EmployeeID, DateTime? FromDate, DateTime? ToDate)
        {
            var emp = new Employee();
            if (EmployeeID == null)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return NotFound();
                emp = _context.Employees.Where(x => x.ApplicationUserId == user.Id).FirstOrDefault();

                if (emp == null)
                {
                    emp = _context.Employees.FirstOrDefault();
                }
                EmployeeID = emp.EmployeeId;
            }
            else
            {
                emp = _context.Employees.Where(x => x.EmployeeId == EmployeeID).FirstOrDefault();
            }

            var employeeDetails = new EmployeeDetailsDto
            {
                EmployeeId = emp.EmployeeId,
                Name = emp.Name,
                PhoneNumber = emp.PhoneNumber,
                Email = emp.Email,
                ImageUrl = emp.ImageUrl,
                DateOfJoining = emp.DateOfJoining,
                Address = emp.Address,
                Designation = emp.Designation,
                EmpCode = emp.EmployeeCode
            };

            var today = DateTime.Today;

            var startOfMonth = new DateTime(today.Year, today.Month, 1);

            if (!FromDate.HasValue)
            {
                FromDate = new DateTime(today.Year, today.Month, 1);
            }

            if (!ToDate.HasValue)
            {
                ToDate = startOfMonth.AddMonths(1).AddDays(-1);
            }

            ViewBag.EmployeeList = _context.Employees.ToList();
            ViewBag.YearList = _context.Year.ToList();
            ViewBag.SelectedEmployeeID = employeeDetails.EmployeeId;

            ViewBag.SelectedFromDate = FromDate;
            ViewBag.SelectedToDate = ToDate;

            ViewBag.Activities = getActivityData();

            var timeSheetRecords = await _context.Timesheets
                                    .Where(t => t.Date >= FromDate && t.Date <= ToDate && t.EmployeeId == EmployeeID && !t.IsDeleted)
                                    .OrderBy(x => x.Date)
                                    .ToListAsync();

            var checkInRecords = await _context.EmployeeAttendance.Where(t => t.Date >= FromDate && t.Date <= ToDate && t.EmployeeId == EmployeeID && !t.IsDeleted)
                                    .OrderBy(x => x.Date)
                                    .ToListAsync();

            var response = new EmployeeTimeSheetViewModel
            {
                EmployeeDetails = employeeDetails,
                TimeSheetRecords = timeSheetRecords
            };
            return View(response);
        }
    }
}
