using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SparkHRMS.Data;
using SparkHRMS.Data.Entities;
using SparkHRMS.Services;
using System;

public class LeaveAllocationController : Controller
{
    private readonly ApplicationDbContext _context;

    public LeaveAllocationController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var list = await _context.LeaveAllocations.Include(x => x.LeaveDetails).ToListAsync();
        return View(list);
    }

    public IActionResult Create(int? EmployeeId, int? YearId)
    {
        ViewBag.EmployeeList = _context.Employees.ToList();
        ViewBag.YearList = _context.Year.ToList();
        ViewBag.LeaveTypes = _context.LeaveTypes.ToList();
        ViewBag.FMode = "Add";

        var alloc = new LeaveAllocation();
        alloc.EmployeeId = EmployeeId ?? 0;
        alloc.YearId = YearId ?? 0;

        return View("Maintanance", alloc);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(LeaveAllocation allocation)
    {
        if (ModelState.IsValid)
        {
            var existingAllocation = await _context.LeaveAllocations
                .FirstOrDefaultAsync(x => x.EmployeeId == allocation.EmployeeId && x.YearId == allocation.YearId);

            allocation.IsActive = true;
            allocation.CreatedDate = DateTime.UtcNow;
            allocation.CreatedBy = 1;
            decimal totalAllocatedDays = 0;
            foreach (var detail in allocation.LeaveDetails)
            {
                detail.IsActive = true;
                detail.CreatedBy = 1;
                detail.CreatedDate = DateTime.UtcNow;
                detail.RemainingDays = detail.AllocatedDays - detail.UsedDays;
                totalAllocatedDays += detail.AllocatedDays;
            }

            allocation.TotalLeaveAllocated = totalAllocatedDays;
            allocation.TotalLeaveBalance = totalAllocatedDays;
            _context.Add(allocation);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        return View("Maintanance", allocation);
    }

    public async Task<IActionResult> RedirectToEdit(int EmployeeId, int YearId)
    {
        var existingAllocation = await _context.LeaveAllocations
               .FirstOrDefaultAsync(x => x.EmployeeId == EmployeeId && x.YearId == YearId);

        if (existingAllocation != null)
        {
            return RedirectToAction(nameof(Edit), new { id = existingAllocation.Id });
        }
        else
        {

            return RedirectToAction(nameof(Create), new { EmployeeId, YearId });
        }

    }

    public async Task<IActionResult> Edit(int id)
    {
        var allocation = await _context.LeaveAllocations
                          .Include(x => x.LeaveDetails)
                          .FirstOrDefaultAsync(x => x.Id == id);

        if (allocation == null)
            return NotFound();

        ViewBag.EmployeeList = _context.Employees.ToList();
        ViewBag.LeaveTypes = _context.LeaveTypes.ToList();
        ViewBag.YearList = _context.Year.ToList();
        ViewBag.FMode = "Edit";

        return View("Maintanance", allocation);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, LeaveAllocation allocation)
    {
        if (id != allocation.Id)
            return NotFound();

        if (ModelState.IsValid)
        {
            var existing = await _context.LeaveAllocations.Include(x => x.LeaveDetails).FirstOrDefaultAsync(x => x.Id == id);
            if (existing == null) return NotFound();

            decimal totalAllocated = 0;
            decimal totalUsedDays = 0;
            decimal totalLeaveBalance = 0;
            allocation.LeaveDetails.ForEach(x =>
            {
                totalAllocated += x.AllocatedDays;
                totalUsedDays += x.UsedDays;
            });

            existing.EmployeeId = allocation.EmployeeId;
            existing.YearId = allocation.YearId;
            existing.TotalLeaveAllocated = totalAllocated;
            existing.TotalLeaveBalance = totalAllocated - totalUsedDays;
            existing.TotalUsedDays = totalUsedDays;
            existing.TotalCarriedForwardLeaves = allocation.TotalCarriedForwardLeaves;
            existing.ModifiedDate = DateTime.UtcNow;
            existing.ModifiedBy = 1;

            // Remove old details
            _context.LeaveAllocationDetails.RemoveRange(existing.LeaveDetails);

            // Add new details
            existing.LeaveDetails = allocation.LeaveDetails.Select(d => new LeaveAllocationDetail
            {
                LeaveTypeId = d.LeaveTypeId,
                AllocatedDays = d.AllocatedDays,
                UsedDays = d.UsedDays,
                RemainingDays = d.AllocatedDays - d.UsedDays,
                CreatedDate = DateTime.UtcNow,
                IsActive = true,
                FromDate = d.FromDate,
                ToDate = d.ToDate,
                Description = d.Description
            }).ToList();

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View("Maintanance", allocation);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var allocation = await _context.LeaveAllocations.FindAsync(id);
        if (allocation == null) return NotFound();

        _context.LeaveAllocations.Remove(allocation);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
