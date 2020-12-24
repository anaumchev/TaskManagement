using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TaskManagement.Pages
{
    public abstract class PageModelWithSession: PageModel
    {
        private TaskManagement.Models.Session _session;
        
    }
}
