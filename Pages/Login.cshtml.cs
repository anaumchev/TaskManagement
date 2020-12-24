using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TaskManagement.Data;
using TaskManagement.Models;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text;

namespace TaskManagement.Pages.Sessions
{
    public class LoginModel : PageModel
    {
        private readonly TaskManagement.Data.TaskManagementContext _context;

        public LoginModel(TaskManagement.Data.TaskManagementContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            _context.Session.RemoveRange(_context.Session.Where(s => (s.Expires.CompareTo(DateTime.Now) <= 0)));
            await _context.SaveChangesAsync();
            return Page();
        }

        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            _context.Session.RemoveRange(_context.Session.Where(s => (s.Expires.CompareTo(DateTime.Now) <= 0)));
            await _context.SaveChangesAsync();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var users = _context.User.Where(s => (s.Username == Username));

            if (users.Count() == 0)
            {
                return Page();
            }

            var salt = Convert.FromBase64String(users.First().Salt);
            var hash = users.First().Password;


            // derive a 256-bit subkey (use HMACSHA1 with 10,000 iterations)
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: Password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            if (String.Compare(hashed, hash) != 0)
            {
                await Task.Delay(5000);
                return Page();
            }

            var role = users.First().Role;
            string sessid = BitConverter.ToString(new HMACSHA256().ComputeHash(Encoding.UTF8.GetBytes(DateTime.Now.ToString()))).Replace("-", "").ToLower();
            DateTime expiration = DateTime.Now.AddMinutes(30);


            _context.Session.Add(new Session { ID = sessid, Role = role, Expires = expiration});
            await _context.SaveChangesAsync();

            var cookieOptions = new CookieOptions
            {
                Secure = true,
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                Expires = new DateTimeOffset(expiration)
            };

            Response.Cookies.Append("sessid", sessid, cookieOptions);
            switch (role) {
                case UserRole.Admin:
                    return RedirectToPage("./Users/Index");
                default:
                    return RedirectToPage("./Assignments/Index");
            }

        }
    }
}
