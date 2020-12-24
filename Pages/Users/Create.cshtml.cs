using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TaskManagement.Data;
using TaskManagement.Models;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Net.Http;

namespace TaskManagement.Pages.Users
{
    public class CreateModel : PageModel
    {
        private readonly TaskManagement.Data.TaskManagementContext _context;
        private TaskManagement.Models.Session _session;

        public CreateModel(TaskManagement.Data.TaskManagementContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (_context.User.Any()) {
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
            }
            return Page();
        }

        [BindProperty]
        public string Password { get; set; }


        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public UserRole Role { get; set; }
        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (_context.User.Any())
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
            }
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (_context.User.Count(s => s.Username == Username) > 0)
            {
                return Page();
            }

            if (Password.Replace(" ","").Length < 12)
            {
                return Page();
            }

            if (Password.Length > 128)
            {
                return Page();
            }

            byte[] salt = new byte[128 / 8];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: Password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            if (!_context.User.Any())
            {
                Role = UserRole.Admin;
            }

            _context.User.Add(new User { Username = Username, Password = hashed, Role = Role, Salt = Convert.ToBase64String(salt)});
            await _context.SaveChangesAsync();

            return RedirectToPage("./Users/Index");
        }
    }
}
