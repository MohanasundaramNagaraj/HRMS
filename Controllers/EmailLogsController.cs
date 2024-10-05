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
    [Authorize]
    public class EmailLogsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmailLogsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: EmailLogs
        public async Task<IActionResult> Index()
        {
            return View(await _context.EmailLogs.OrderByDescending(x=>x.Id).ToListAsync());
        }

        // GET: EmailLogs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emailLogs = await _context.EmailLogs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (emailLogs == null)
            {
                return NotFound();
            }

            return View(emailLogs);
        }

        // GET: EmailLogs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EmailLogs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Recipient,Cc,Subject,Body,SentDate,IsSuccessful,ErrorMessage")] EmailLogs emailLogs)
        {
            if (ModelState.IsValid)
            {
                _context.Add(emailLogs);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(emailLogs);
        }

        // GET: EmailLogs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emailLogs = await _context.EmailLogs.FindAsync(id);
            if (emailLogs == null)
            {
                return NotFound();
            }
            return View(emailLogs);
        }

        // POST: EmailLogs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Recipient,Cc,Subject,Body,SentDate,IsSuccessful,ErrorMessage")] EmailLogs emailLogs)
        {
            if (id != emailLogs.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(emailLogs);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmailLogsExists(emailLogs.Id))
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
            return View(emailLogs);
        }

        // GET: EmailLogs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emailLogs = await _context.EmailLogs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (emailLogs == null)
            {
                return NotFound();
            }

            return View(emailLogs);
        }

        // POST: EmailLogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var emailLogs = await _context.EmailLogs.FindAsync(id);
            if (emailLogs != null)
            {
                _context.EmailLogs.Remove(emailLogs);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmailLogsExists(int id)
        {
            return _context.EmailLogs.Any(e => e.Id == id);
        }
    }
}
