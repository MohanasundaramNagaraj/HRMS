using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SparkHRMS.Data;
using SparkHRMS.Data.Entities;

namespace SparkHRMS.Controllers
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class EmployeeAttendanceRequestsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployeeAttendanceRequestsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: EmployeeAttendanceRequests
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.EmployeeAttendanceRequest.Include(e => e.Employee);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: EmployeeAttendanceRequests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeAttendanceRequest = await _context.EmployeeAttendanceRequest
                .Include(e => e.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employeeAttendanceRequest == null)
            {
                return NotFound();
            }

            return View(employeeAttendanceRequest);
        }

        // GET: EmployeeAttendanceRequests/Create
        public IActionResult Create()
        {
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "Email");
            return View();
        }

        // POST: EmployeeAttendanceRequests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EmployeeId,RequestType,StartDate,EndDate,StartTime,EndTime,SubStatusId,CreatedBy,CreatedTime,StatusUpdatedBy,StatusUpdatedTime")] EmployeeAttendanceRequest employeeAttendanceRequest)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employeeAttendanceRequest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "Email", employeeAttendanceRequest.EmployeeId);
            return View(employeeAttendanceRequest);
        }

        // GET: EmployeeAttendanceRequests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeAttendanceRequest = await _context.EmployeeAttendanceRequest.FindAsync(id);
            if (employeeAttendanceRequest == null)
            {
                return NotFound();
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "Email", employeeAttendanceRequest.EmployeeId);
            return View(employeeAttendanceRequest);
        }

        // POST: EmployeeAttendanceRequests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EmployeeId,RequestType,StartDate,EndDate,StartTime,EndTime,SubStatusId,CreatedBy,CreatedTime,StatusUpdatedBy,StatusUpdatedTime")] EmployeeAttendanceRequest employeeAttendanceRequest)
        {
            if (id != employeeAttendanceRequest.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employeeAttendanceRequest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeAttendanceRequestExists(employeeAttendanceRequest.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "Email", employeeAttendanceRequest.EmployeeId);
            return View(employeeAttendanceRequest);
        }

        // GET: EmployeeAttendanceRequests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeAttendanceRequest = await _context.EmployeeAttendanceRequest
                .Include(e => e.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employeeAttendanceRequest == null)
            {
                return NotFound();
            }

            return View(employeeAttendanceRequest);
        }

        // POST: EmployeeAttendanceRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employeeAttendanceRequest = await _context.EmployeeAttendanceRequest.FindAsync(id);
            if (employeeAttendanceRequest != null)
            {
                _context.EmployeeAttendanceRequest.Remove(employeeAttendanceRequest);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeAttendanceRequestExists(int id)
        {
            return _context.EmployeeAttendanceRequest.Any(e => e.Id == id);
        }
    }
}
