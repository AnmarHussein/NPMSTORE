using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NPMSTORE.Models;

namespace NPMSTORE.Controllers
{
    
    public class ContactUsController : Controller
    {
        private readonly ModelContext _context;

        public ContactUsController(ModelContext context)
        {
            _context = context;
        }

        // GET: ContactUs
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var modelContext = _context.ContactUs.Include(c => c.Customer);
            return View(await modelContext.ToListAsync());
        }

        // GET: ContactUs/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contactU = await _context.ContactUs
                .Include(c => c.Customer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contactU == null)
            {
                return NotFound();
            }

            return View(contactU);
        }

        // GET: ContactUs/Create
        
        public IActionResult Create()
        {
            return View();
        }

        // POST: ContactUs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Subject,Message,FullName,Email")] ContactU contactU)
        {
            if (ModelState.IsValid)
            {
                contactU.Customerid = 15;
                _context.Add(contactU);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Create));
            }
            ViewData["Customerid"] = new SelectList(_context.Customers, "Id", "Email", contactU.Customerid);
            return View(contactU);
        }

        // GET: ContactUs/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contactU = await _context.ContactUs.FindAsync(id);
            if (contactU == null)
            {
                return NotFound();
            }
            ViewData["Customerid"] = new SelectList(_context.Customers, "Id", "Email", contactU.Customerid);
            return View(contactU);
        }

        // POST: ContactUs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(decimal id, [Bind("Id,Subject,Message,FullName,Email,Customerid")] ContactU contactU)
        {
            if (id != contactU.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contactU);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContactUExists(contactU.Id))
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
            ViewData["Customerid"] = new SelectList(_context.Customers, "Id", "Email", contactU.Customerid);
            return View(contactU);
        }

        // GET: ContactUs/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contactU = await _context.ContactUs
                .Include(c => c.Customer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contactU == null)
            {
                return NotFound();
            }

            return View(contactU);
        }

        // POST: ContactUs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            var contactU = await _context.ContactUs.FindAsync(id);
            _context.ContactUs.Remove(contactU);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContactUExists(decimal id)
        {
            return _context.ContactUs.Any(e => e.Id == id);
        }
    }
}
