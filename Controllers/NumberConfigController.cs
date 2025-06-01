using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SparkHRMS.Data; 
using SparkHRMS.Data.Setting; 
using System.Linq;
using System.Threading.Tasks;

[Authorize]
public class NumberConfigController : Controller
{
    private readonly ApplicationDbContext _context;

    public NumberConfigController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _context.SET_NumberConfig.ToListAsync());
    }

    public IActionResult Create()
    {
        ViewBag.Mode = "Create";
        return View("Maintanance", new SET_NumberConfig());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SET_NumberConfig model)
    {
        if (ModelState.IsValid)
        {
            model.CreatedDate = DateTime.Now;
            _context.SET_NumberConfig.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "NumberConfig");
            // return RedirectToAction(nameof(Index));
        }

        ViewBag.Mode = "Create";
        return View("Maintanance", model);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var model = await _context.SET_NumberConfig.FindAsync(id);
        if (model == null) return NotFound();

        ViewBag.Mode = "Edit";
        return View("Maintanance", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SET_NumberConfig model)
    {
        if (id != model.NConfigID) return NotFound();

        if (ModelState.IsValid)
        {
            model.LastUpdatedDate = DateTime.Now;
            _context.Update(model);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "NumberConfig");
            //return RedirectToAction(nameof(Index));
        }

        ViewBag.Mode = "Edit";
        return View("Maintanance", model);
    }

    public async Task<IActionResult> Details(int id)
    {
        var model = await _context.SET_NumberConfig.FindAsync(id);
        if (model == null) return NotFound();

        ViewBag.Mode = "Details";
        return View("Maintanance", model);
    }


    public async Task<IActionResult> Delete(int id)
    {
        var model = await _context.SET_NumberConfig.FindAsync(id);
        if (model == null)
            return NotFound();

        _context.SET_NumberConfig.Remove(model);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

   
}
