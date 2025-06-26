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
using SparkHRMS.Utilities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SparkHRMS.Controllers
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public EmployeeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
        }

        // GET: Employee
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Employees.Include(e => e.ApplicationUser);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Employee/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.ApplicationUser)
                .FirstOrDefaultAsync(m => m.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Employee/Create
        public IActionResult Create()
        {
            ViewBag.ActiveUsers = _context.Users.Where(x => x.IsActive == true).ToList();
            return View();
        }

        // POST: Employee/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmployeeId,EmployeeCode,Name,Email,PhoneNumber,DOB,Gender,Designation,ImageUrl,DateOfJoining,Address,ReportingHeadMailID")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                var applicationUser = new ApplicationUser
                {
                    UserName = employee.Email,
                    Email = employee.Email,
                    IsActive = true,
                    PhoneNumber = employee.PhoneNumber,
                };
                var defaultPassword = _configuration["AppSettings:DefaultUserPassword"];
                var result = await _userManager.CreateAsync(applicationUser, defaultPassword);

                if (result.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(applicationUser, Roles.RoleType.Employee.ToString());

                    if (!roleResult.Succeeded)
                    {
                        foreach (var error in roleResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }

                        await _userManager.DeleteAsync(applicationUser);
                        return View(employee);
                    }

                    try
                    {
                        employee.ApplicationUserId = applicationUser.Id;
                        employee.ImageUrl = _configuration["AppSettings:ImagePath"] + "/" + employee.EmployeeCode + ".jpg";
                        if (employee.ImageUrl == null)
                        {
                            employee.ImageUrl = "https://www.freeiconspng.com/thumbs/no-image-icon/no-image-icon-6.png";
                        }
                        if (employee.DateOfJoining == null)
                        {
                            employee.DateOfJoining = DateTime.Now;
                        }
                        _context.Add(employee);
                        await _context.SaveChangesAsync();

                        return RedirectToAction(nameof(Index));
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError(string.Empty, "An error occurred while saving the employee. Please try again.");
                    }

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            else
            {
                foreach (var key in ModelState.Keys)
                {
                    if (ModelState[key].Errors.Count > 0)
                    {
                        ModelState.AddModelError(string.Empty, $"{key} is required.");
                    }
                }

                ModelState.AddModelError(string.Empty, "Please fill all required fields.");
            }
            ViewBag.ActiveUsers = _context.Users.Where(x => x.IsActive == true).ToList();
            return View(employee);
        }

        // GET: Employee/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.ActiveUsers = _context.Users;
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", employee.ApplicationUserId);
            return View(employee);
        }

        // POST: Employee/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EmployeeId,EmployeeCode,Name,Email,PhoneNumber,DOB,Gender,Designation,ImageUrl,DateOfJoining,Address,ApplicationUserId,IsActive")] Employee employee)
        {
            if (id != employee.EmployeeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                employee.ApplicationUserId = _context.Employees.Where(x => x.EmployeeId == employee.EmployeeId).Select(x => x.ApplicationUserId).FirstOrDefault();
                if (employee.ImageUrl == null)
                {
                    employee.ImageUrl = "https://www.freeiconspng.com/thumbs/no-image-icon/no-image-icon-6.png";
                }
                
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.EmployeeId))
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
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", employee.ApplicationUserId);
            return View(employee);
        }

        // GET: Employee/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.ApplicationUser)
                .FirstOrDefaultAsync(m => m.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employee/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.EmployeeId == id);
        }
    }
}
