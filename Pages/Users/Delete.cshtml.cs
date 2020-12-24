using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Data;
using TaskManagement.Models;

namespace TaskManagement.Pages.Users
{
    public class DeleteModel : PageModel
    {
        private readonly TaskManagement.Data.TaskManagementContext _context;
        private TaskManagement.Models.Session _session;


        public DeleteModel(TaskManagement.Data.TaskManagementContext context)
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
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
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

            User = await _context.User.FindAsync(id);

            if (User != null)
            {
                _context.User.Remove(User);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
