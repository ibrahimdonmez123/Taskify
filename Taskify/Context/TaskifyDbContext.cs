using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Taskify.Models;

namespace Taskify.Context
{
    public class TaskifyDbContext : DbContext
    {
        public TaskifyDbContext() : base("name=TaskifyConnection") { }

        public DbSet<Duty> Duties { get; set; }
        public DbSet<User> Users { get; set; }

    }
}