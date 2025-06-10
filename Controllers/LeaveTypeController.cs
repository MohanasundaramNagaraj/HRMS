using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SparkHRMS.Data;
using SparkHRMS.Data.Setting;
using System.Linq;
using System.Threading.Tasks;

[Authorize(Roles = "SuperAdmin,Admin")]
public class LeaveTypeController : Controller
{
   
    private readonly ApplicationDbContext _context;
   
    public LeaveTypeController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: LeaveType
    public async Task<IActionResult> Index()
    {
        return View(await _context.LeaveTypes.ToListAsync());
    }

    // GET: LeaveType/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: LeaveType/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(LeaveType leaveType)
    {
        if (ModelState.IsValid)
        {
            _context.Add(leaveType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(leaveType);
    }

    // GET: LeaveType/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var leaveType = await _context.LeaveTypes.FindAsync(id);
        if (leaveType == null) return NotFound();
        return View(leaveType);
    }

    // POST: LeaveType/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, LeaveType leaveType)
    {
        if (id != leaveType.LeaveTypeId) return NotFound();

        if (ModelState.IsValid)
        {
            _context.Update(leaveType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(leaveType);
    }

    // GET: LeaveType/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var leaveType = await _context.LeaveTypes.FindAsync(id);
        if (leaveType == null) return NotFound();
        return View(leaveType);
    }

    // POST: LeaveType/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var leaveType = await _context.LeaveTypes.FindAsync(id);
        if (leaveType != null)
        {
            _context.LeaveTypes.Remove(leaveType);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
