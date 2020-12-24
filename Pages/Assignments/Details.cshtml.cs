using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Data;
using TaskManagement.Models;

namespace TaskManagement.Pages.Assignments
{
    public class DetailsModel : PageModel
    {
        private readonly TaskManagement.Data.TaskManagementContext _context;
        private TaskManagement.Models.Session _session;

        public DetailsModel(TaskManagement.Data.TaskManagementContext context)
        {
            _context = context;
        }

        public Assignment Assignment { get; set; }

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

            Assignment = await _context.Task.FirstOrDefaultAsync(m => m.ID == id);

            if (Assignment == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
