using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Models;

namespace TaskManagement.Data
{
    public class TaskManagementContext : DbContext
    {
        public TaskManagementContext (DbContextOptions<TaskManagementContext> options)
            : base(options)
        {
        }

        public DbSet<TaskManagement.Models.Assignment> Task { get; set; }
        public DbSet<TaskManagement.Models.User> User { get; set; }
        public DbSet<TaskManagement.Models.Session> Session { get; set; }
    }
}
