using Hangfire;
using Microsoft.AspNetCore.Identity;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using SparkHRMS.Data;
using SparkHRMS.Data.Entities;
using SparkHRMS.Interfaces;
using SparkHRMS.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SparkHRMS.Controllers
{
    public class LeaveRequestController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly Utility utility;
        private readonly IEmailSender _emailService;
        private readonly IBackgroundJobClient _backgroundJobClient;
        public LeaveRequestController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IConfiguration configuration, Utility utility, IEmailSender emailService, IBackgroundJobClient backgroundJobClient)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
            this.utility = utility;
            _emailService = emailService;
            _backgroundJobClient = backgroundJobClient;
        }

        public async Task<IActionResult> Index(int? EmployeeId, int? YearId, int? MonthId)
        {
            ViewBag.EmployeeList = _context.Employees.ToList();
            ViewBag.YearList = _context.Year.ToList();
            ViewBag.MonthList = _context.Month.ToList();

            var user = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            var isSuperAdmin = await _userManager.IsInRoleAsync(user, "SuperAdmin");

            ViewBag.IsUserAdmin = isAdmin || isSuperAdmin;

            if (!isAdmin && !isSuperAdmin)
            {
                EmployeeId = _context.Employees.FirstOrDefault(x => x.ApplicationUserId == user.Id).EmployeeId;
            }

            var query = (from req in _context.LeaveRequest
                         select req);

            if (EmployeeId.HasValue)
            {
                query = query.Where(x => x.EmployeeId == EmployeeId);
            }

            if (YearId.HasValue)
            {
                var year = _context.Year.Where(x => x.Id == YearId).FirstOrDefault();
                query = query.Where(x => x.RequestedDate.Year == year.Year);
            }

            if (MonthId.HasValue)
            {
                var month = _context.Month.Where(x => x.Id == MonthId).FirstOrDefault();
                query = query.Where(x => x.RequestedDate.Month == month.Number);
            }

            var requests = await query.ToListAsync();

            ViewBag.SelectedEmployeeId = EmployeeId;
            ViewBag.SelectedYearId = YearId;
            ViewBag.SelectedMonth = MonthId;

            return View(requests);
        }

        // For Employee: Request Leave
        public async Task<IActionResult> MaintananceAsync(string FMode, int? EntryID)
        {
            LeaveRequest request = new LeaveRequest();
            string documentNumber = "";
            if (FMode == "ADD")
            {
                utility.GenerateDocumentNumber("Leave_Request", false, out documentNumber);
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return NotFound();
                var emp = _context.Employees.Where(x => x.ApplicationUserId == user.Id).FirstOrDefault();
                if (emp != null)
                {
                    request.EmployeeId = emp.EmployeeId;
                }
                request.StartDate = DateTime.Now;
                request.EndDate = DateTime.Now;
                request.Status = "Pending";
                request.RequestNumber = documentNumber;
            }
            else
            {
                request = _context.LeaveRequest.Where(x => x.Id == EntryID).FirstOrDefault();
            }

            ViewBag.FMode = FMode;
            ViewBag.EntryID = EntryID;
            ViewBag.EmployeeList = _context.Employees.ToList();
            ViewBag.LeaveTypes = new SelectList(_context.LeaveTypes, "LeaveTypeId", "Name");
            ViewBag.LeaveReasons = new SelectList(_context.LeaveReasons, "LeaveReasonId", "LeaveReasonName");
            return View(request);
        }
        public IActionResult GetLeaveTypeDetails(int employeeId, int LeaveRequestId)
        {
            if (LeaveRequestId != 0)
            {
                var leaveTypes = (from r in _context.LeaveRequestDetail
                                  join lt in _context.LeaveTypes on r.LeaveTypeId equals lt.LeaveTypeId
                                  where r.LeaveRequestId == LeaveRequestId
                                  select new
                                  {
                                      r.LeaveTypeId,
                                      LeaveTypeName = lt.Name,
                                      r.AllocatedDays,
                                      r.UsedDays,
                                      RemainingDays = r.BalanceDays,
                                      r.RequiredDays
                                  }).Distinct().ToList();
                return Json(leaveTypes);
            }
            else
            {
                var leaveTypes = (from a in _context.LeaveAllocations
                                  join d in _context.LeaveAllocationDetails on a.Id equals d.LeaveAllocationId
                                  join lt in _context.LeaveTypes on d.LeaveTypeId equals lt.LeaveTypeId
                                  where a.EmployeeId == employeeId && d.IsActive && lt.IsActive
                                  select new
                                  {
                                      d.LeaveTypeId,
                                      LeaveTypeName = lt.Name,
                                      d.AllocatedDays,
                                      d.UsedDays,
                                      d.RemainingDays,
                                      RequiredDays = 0
                                  }).ToList();
                return Json(leaveTypes);
            }


        }

        [HttpPost]
        public async Task<IActionResult> Create(LeaveRequest model)
        {

            model.Comments = model.Comments == null ? "" : model.Comments;
            if (ModelState.IsValid)
            {
                model.RequestedDate = DateTime.Now;
                model.Status = "PEN";
                _context.LeaveRequest.Add(model);
                await _context.SaveChangesAsync();
                var currentUserId = Convert.ToInt32(_userManager.GetUserId(User));
                _context.LeaveRequestHistory.Add(new LeaveRequestHistory
                {
                    LeaveRequestId = model.Id,
                    Status = "PEN",
                    ChangeDate = DateTime.Now,
                    ChangedBy = currentUserId,
                    Comments = model.Comments
                });
                await _context.SaveChangesAsync();

                var year = _context.Year.Where(x => x.Year == DateTime.Now.Year).FirstOrDefault();

                string documentNumber = "";
                utility.GenerateDocumentNumber("Leave_Request", true, out documentNumber);

                if (model.LeaveRequestDetails != null && model.LeaveRequestDetails.Any())
                {
                    foreach (var detail in model.LeaveRequestDetails)
                    {
                        detail.LeaveRequestId = model.Id;
                        detail.Id = 0;
                        _context.LeaveRequestDetail.Add(detail);
                    }

                }
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.ToString());
                }

                var adminUsers = await GetAdminsAndSuperAdminsAsync();
                string subject = "Leave Request by " + utility.GetEmployeeCodeById(model.EmployeeId) + "-" + utility.GetEmployeeNameById(model.EmployeeId);
                foreach (var user in adminUsers)
                {
                    _backgroundJobClient.Enqueue(() => _emailService.SendEmailAsync(user.Email, subject, getHtmlContent(model.Id)));
                }

                return Ok(true);
            }
            else
            {
                var errors = ModelState
                               .Where(x => x.Value.Errors.Count > 0)
                               .ToDictionary(
                                   kvp => kvp.Key,
                                   kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                               );

                return BadRequest(errors);
            }
            
        }

        private string getHtmlContent(int LeaveRequestID)
        {
            var leave = _context.LeaveRequest.FirstOrDefault(r => r.Id == LeaveRequestID);

            string status = "";
            if (leave.Status == "PEN")
            {
                status = "<div class=\"badge col-cyan\">Approval Pending</div>";
            }
            else if (leave.Status == "CAN-REQ")
            {
                status = " <div class=\"badge col-purple\" > Cancel Requested</div>";
            }
            else if (leave.Status == "CAN-ACC")
            {
                status = "<div class=\"badge col-orange\">Cancelled</div>";
            }
            else if (leave.Status == "CAN-REJ")
            {
                status = "<div class= \"badge col-red\"> Cancel Request Rejected</div>";
            }
            else if (leave.Status == "ACC")
            {
                status = " <div class= \"badge col-green\" > Accepted </div>";
            }
            else if (leave.Status == "REJ")
            {
                status = "<div class= \"badge col-red\" > Rejected </div>";
            }

            string approvalData = "";
            if (leave.Status != "PEN")
            {
                var approval = _context.LeaveRequestHistory.Where(x => x.LeaveRequestId == leave.Id).OrderByDescending(x => x.Id).FirstOrDefault();
                approvalData = $@"
                    <tr><td style='padding: 8px 15px; background: #f7f7f7;'>Approval Date</td><td style='padding: 8px 15px;'>{approval.ChangeDate:dd MMM yyyy}</td></tr>
                    <tr><td style='padding: 8px 15px; background: #f7f7f7;'>Approval Comments</td><td style='padding: 8px 15px;'>{approval.Comments}</td></tr>
                ";
            }

            var leaveRequestDetails = (from det in _context.LeaveRequestDetail
                                      join type in _context.LeaveTypes on det.LeaveTypeId equals type.LeaveTypeId
                                      where det.LeaveRequestId == LeaveRequestID
                                      select new
                                      {
                                          type.Name,
                                          type.Code,
                                          det.RequiredDays
                                      }).Distinct().ToList();

            string siteUrl = _configuration["AppSettings:ThisSiteUrl"] + "/LeaveRequest";

            string leaveCountBasedOnLeaveType = "";
            foreach(var det in leaveRequestDetails)
            {
                leaveCountBasedOnLeaveType += "<span>" + det.Code + " : " + det.RequiredDays + " days" + "</span></br>";
            }
            string htmlContent = $@"
                    <table style='width: 100%; border-collapse: collapse; font-family: Arial, sans-serif; font-size: 14px;'>
                        <tr>
                            <td colspan='2' style='background-color: #007BFF; color: white; padding: 15px; text-align: center;'>
                                <h2>Leave Request Details({leave.RequestNumber})</h2>
                            </td>
                        </tr>
                        <tr><td colspan='2' style='padding: 10px 15px;'>A leave request has been updated with the following details:</td></tr>

                        <tr><td style='padding: 8px 15px; background: #f7f7f7; width: 40%;'>Request Number</td><td style='padding: 8px 15px;'>{leave.RequestNumber}</td></tr>
                        <tr><td style='padding: 8px 15px; background: #f7f7f7;'>Employee Name</td><td style='padding: 8px 15px;'>{utility.GetEmployeeCodeById(leave.EmployeeId)} - {utility.GetEmployeeNameById(leave.EmployeeId)}</td></tr>
                        <tr><td style='padding: 8px 15px; background: #f7f7f7;'>Requested Date</td><td style='padding: 8px 15px;'>{leave.RequestedDate:dd MMM yyyy}</td></tr>
                        <tr><td style='padding: 8px 15px; background: #f7f7f7;'>Start Date</td><td style='padding: 8px 15px;'>{leave.StartDate:dd MMM yyyy} {(leave.IsStartDateHalfDay ? "(Half Day)" : "")}</td></tr>
                        <tr><td style='padding: 8px 15px; background: #f7f7f7;'>End Date</td><td style='padding: 8px 15px;'>{leave.EndDate:dd MMM yyyy} {(leave.IsEndDateHalfDay ? "(Half Day)" : "")}</td></tr>
                        <tr><td style='padding: 8px 15px; background: #f7f7f7;'>Total Leave Days</td><td style='padding: 8px 15px;'>{leave.TotalLeaveDays} ({leaveCountBasedOnLeaveType})</td></tr>
                        <tr><td style='padding: 8px 15px; background: #f7f7f7;'>Leave Reason</td><td style='padding: 8px 15px;'>{utility.GetLeaveReasonById(leave.LeaveReasonId).LeaveReasonName}</td></tr>
                        <tr><td style='padding: 8px 15px; background: #f7f7f7;'>Comments</td><td style='padding: 8px 15px;'>{leave.Comments}</td></tr>
                        <tr><td style='padding: 8px 15px; background: #f7f7f7;'>Status</td><td style='padding: 8px 15px;'>{status}</td></tr>
                        {approvalData}
                        <tr><td colspan='2' style='padding: 15px; text-align: center; background-color: #f1f1f1;'>Please visit <a href='{siteUrl}'>{_configuration["AppSettings:SiteTitle"]}</a></td></tr>
                    </table>
                    ";

            return htmlContent;
        }
        public async Task<List<ApplicationUser>> GetAdminsAndSuperAdminsAsync()
        {
            var superAdmins = await _userManager.GetUsersInRoleAsync("SuperAdmin");
            var admins = await _userManager.GetUsersInRoleAsync("Admin");

            // Combine the lists (remove duplicates if necessary)
            var result = superAdmins.Union(admins).ToList();

            return result;
        }
        // Admin: Approve request
        public async Task<IActionResult> Approve(int id, string Status, string Comments)
        {
            var request = await _context.LeaveRequest.FindAsync(id);
            if (request != null)
            {
                request.Status = Status;
                var user = await _userManager.GetUserAsync(User);

                if(request.Status == "ACC")
                {
                    var year = _context.Year.Where(x => x.Year == DateTime.Now.Year).FirstOrDefault();
                    var allocHeader = _context.LeaveAllocations.Where(x => x.EmployeeId == request.EmployeeId && x.YearId == year.Id).FirstOrDefault();

                    var leaveDetail = _context.LeaveRequestDetail.Where(x => x.LeaveRequestId == id).ToList();
                    foreach (var detail in leaveDetail)
                    {
                        var aloc_detail = _context.LeaveAllocationDetails.Where(x => x.LeaveAllocationId == allocHeader.Id && x.LeaveTypeId == detail.LeaveTypeId).FirstOrDefault();

                        aloc_detail.UsedDays += detail.RequiredDays;

                        _context.Entry(aloc_detail).State = EntityState.Modified;
                    }
                }
              
                _context.LeaveRequestHistory.Add(new LeaveRequestHistory
                {
                    LeaveRequestId = request.Id,
                    Status = Status,
                    ChangeDate = DateTime.Now,
                    ChangedBy = user.Id,
                    Comments = Comments
                });

                await _context.SaveChangesAsync();

                var email = _context.Employees.FirstOrDefault(x => x.EmployeeId == request.EmployeeId).Email;
                string subject = "Approval Status of Request - " + request.RequestNumber;
                _backgroundJobClient.Enqueue(() => _emailService.SendEmailAsync(email, subject, getHtmlContent(request.Id)));
            }
            return RedirectToAction("Index");
        }

        // Admin or Employee: Cancel request
        public async Task<IActionResult> Cancel(int id, int changedBy, string Comments)
        {
            var request = await _context.LeaveRequest.FindAsync(id);
            if (request != null && request.Status != "CAN-REQ")
            {
                request.Status = "CAN-REQ";
                var user = await _userManager.GetUserAsync(User);
                _context.LeaveRequestHistory.Add(new LeaveRequestHistory
                {
                    LeaveRequestId = request.Id,
                    Status = "CAN-REQ",
                    ChangeDate = DateTime.Now,
                    ChangedBy = user.Id,
                    Comments = Comments
                });

                await _context.SaveChangesAsync();

                var adminUsers = await GetAdminsAndSuperAdminsAsync();
                string subject = "Cancel Leave Request by " + utility.GetEmployeeCodeById(request.EmployeeId) + "-" + utility.GetEmployeeNameById(request.EmployeeId);
                foreach (var _user in adminUsers)
                {
                    _backgroundJobClient.Enqueue(() => _emailService.SendEmailAsync(_user.Email, subject, getHtmlContent(request.Id)));
                }

            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> GetTimeline(int id)
        {
            var currentUserId = _userManager.GetUserId(User);
            var history = await _context.LeaveRequestHistory
                .Where(h => h.LeaveRequestId == id)
                .OrderByDescending(h => h.ChangeDate)
                .Select(h => new LeaveRequestTimelineViewModel
                {
                    Status = h.Status,
                    Comments = h.Comments,
                    ChangeDate = h.ChangeDate,
                    ChangedByName = _context.Users
                                         .Where(e => e.Id == h.ChangedBy)
                                         .Select(e => e.UserName)
                                         .FirstOrDefault(),
                    IsCurrentUser = h.ChangedBy.ToString() == currentUserId,
                    RequestNumber = h.LeaveRequest.RequestNumber
                })
                .ToListAsync();

            return PartialView("LeaveRequestTimeline", history);
        }

        public async Task<ActionResult> LeaveRequestForm(LeaveRequest model)
        {
            return PartialView("LeaveRequestForm", model);
        }

        [HttpPost]
        public IActionResult GetNonWorkingDays([FromBody] DateRequest request)
        {
            DateTime startDate = DateTime.Parse(request.StartDate);
            DateTime endDate = startDate.AddMonths(1);

            var holidays = _context.Holiday
                .Where(h => h.Date >= startDate && h.Date <= endDate)
                .Select(h => h.Date.ToString("yyyy-MM-dd"))
                .ToList();

            var weekendDayNames = _configuration.GetSection("AttendanceSettings:WeekendDays").Get<List<string>>();
            var weekOffDays = weekendDayNames.Select(day =>
            {
                return (int)Enum.Parse<DayOfWeek>(day);
            }).ToList();

            return Json(new
            {
                holidays = holidays,
                weekOffDays = weekOffDays
            });
        }

        public class DateRequest
        {
            public string StartDate { get; set; }
        }

    }
}

public class LeaveRequestTimelineViewModel
{
    public string RequestNumber { get; set; }
    public string EmployeeName { get; set; }

    public string Status { get; set; }
    public string Comments { get; set; }
    public DateTime ChangeDate { get; set; }
    public string ChangedByName { get; set; }
    public bool IsCurrentUser { get; set; }
}

