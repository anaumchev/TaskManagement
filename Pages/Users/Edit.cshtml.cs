using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Data;
using TaskManagement.Models;

namespace TaskManagement.Pages.Users
{
    public class EditModel : PageModel
    {
        private readonly TaskManagement.Data.TaskManagementContext _context;
        private TaskManagement.Models.Session _session;

        public EditModel(TaskManagement.Data.TaskManagementContext context)
        {
            _context = context;
        }

        [BindProperty]
        public User User { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            var sessid = Request.Cookies.Where(c => (c.Key == "sessid"));
            if (sessid.Count() == 0)
            {
                return Redirect("/Login");
            }
            var valid_sessions = _context.Session.Where(s => (s.ID == sessid.First().Value));
            if (valid_sessions.Count() == 0)
            {
                return Redirect("/Login");
            }

            _session = valid_sessions.First();

            if (_session.Role != UserRole.Admin)
            {
                return Redirect("/Login");
            }
            if (_session.Expires.CompareTo(DateTime.Now) <= 0)
            {
                _context.Session.Remove(_session);
                await _context.SaveChangesAsync();
                _session = null;
                return Redirect("/Login");
            }
            if (id == null)
            {
                return NotFound();
            }

            User = await _context.User.FirstOrDefaultAsync(m => m.ID == id);

            if (User == null)
            {
                return NotFound();
            }
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            var sessid = Request.Cookies.Where(c => (c.Key == "sessid"));
            if (sessid.Count() == 0)
            {
                return Redirect("/Login");
            }
            var valid_sessions = _context.Session.Where(s => (s.ID == sessid.First().Value));
            if (valid_sessions.Count() == 0)
            {
                return Redirect("/Login");
            }

            _session = valid_sessions.First();

            if (_session.Role != UserRole.Admin)
            {
                return Redirect("/Login");
            }
            if (_session.Expires.CompareTo(DateTime.Now) <= 0)
            {
                _context.Session.Remove(_session);
                await _context.SaveChangesAsync();
                _session = null;
                return Redirect("/Login");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(User).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(User.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.ID == id);
        }
    }
}
