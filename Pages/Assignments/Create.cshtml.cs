using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TaskManagement.Data;
using TaskManagement.Models;

namespace TaskManagement.Pages.Assignments
{
    public class CreateModel : PageModel
    {
        private readonly TaskManagement.Data.TaskManagementContext _context;
        private TaskManagement.Models.Session _session;

        public CreateModel(TaskManagement.Data.TaskManagementContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGet()
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

            if (_session.Role != UserRole.Manager)
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

        [BindProperty]
        public Assignment Assignment { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
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

            if (_session.Role != UserRole.Manager)
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
            _context.Task.Add(Assignment);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
