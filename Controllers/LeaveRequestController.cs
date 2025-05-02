using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SparkHRMS.Data;
using SparkHRMS.Data.Entities;
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

        public LeaveRequestController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IConfiguration configuration, Utility utility)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
            this.utility = utility;
        }

        public async Task<IActionResult> Index(int? EmployeeId, int? YearId, int? MonthId)
        {
            ViewBag.EmployeeList = _context.Employees.ToList();
            ViewBag.YearList = _context.Year.ToList();
            ViewBag.MonthList = _context.Month.ToList();

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
                utility.GenerateDocumentNumber("Leave_Request",false, out documentNumber);
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
            }
            else
            {

            }

            request.RequestNumber = documentNumber;

            ViewBag.EmployeeList = _context.Employees.ToList();
            ViewBag.LeaveTypes = new SelectList(_context.LeaveTypes, "LeaveTypeId", "Name");
            ViewBag.LeaveReasons = new SelectList(_context.LeaveReasons, "LeaveReasonId", "LeaveReasonName");
            return View(request);
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

                _context.LeaveRequestHistory.Add(new LeaveRequestHistory
                {
                    LeaveRequestId = model.Id,
                    Status = "PEN",
                    ChangeDate = DateTime.Now,
                    ChangedBy = model.EmployeeId,
                    Comments = model.Comments
                });
                await _context.SaveChangesAsync();

                var year = _context.Year.Where(x => x.Year == DateTime.Now.Year).FirstOrDefault();

                string documentNumber = "";
                utility.GenerateDocumentNumber("Leave_Request", true, out documentNumber);

                return RedirectToAction("Index", new { employeeId = model.EmployeeId, YearId = year.Id, MonthId = DateTime.Now.Month});
            }
            return View(model);
        }

        // Admin: Approve request
        public async Task<IActionResult> Approve(int id, string Status, string Comments)
        {
            var request = await _context.LeaveRequest.FindAsync(id);
            if (request != null)
            {
                request.Status = Status;

                _context.LeaveRequestHistory.Add(new LeaveRequestHistory
                {
                    LeaveRequestId = request.Id,
                    Status = Status,
                    ChangeDate = DateTime.Now,
                    ChangedBy = 1, 
                    Comments = Comments
                });

                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        // Admin or Employee: Cancel request
        public async Task<IActionResult> Cancel(int id, int changedBy)
        {
            var request = await _context.LeaveRequest.FindAsync(id);
            if (request != null && request.Status != "CAN-REQ")
            {
                request.Status = "CAN-REQ";

                _context.LeaveRequestHistory.Add(new LeaveRequestHistory
                {
                    LeaveRequestId = request.Id,
                    Status = "CAN-REQ",
                    ChangeDate = DateTime.Now,
                    ChangedBy = changedBy,
                    Comments = "Cancelled by user"
                });

                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        // Admin: Re-approve a cancelled request
        public async Task<IActionResult> ReApprove(int id, string Status, string Comments)
        {
            var request = await _context.LeaveRequest.FindAsync(id);
            if (request != null && request.Status == "CAN-REQ")
            {
                request.Status = Status;

                _context.LeaveRequestHistory.Add(new LeaveRequestHistory
                {
                    LeaveRequestId = request.Id,
                    Status = Status,
                    ChangeDate = DateTime.Now,
                    ChangedBy = 1, 
                    Comments = Comments
                });

                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}
