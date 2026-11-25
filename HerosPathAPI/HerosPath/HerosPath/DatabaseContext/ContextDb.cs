using HerosPath.Models;
using HerosPath.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace HerosPath.DatabaseContext
{
    public class ContextDb : DbContext
    {
        public ContextDb(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Level> Levels { get; set; }
        public DbSet<LevelProgress> LevelsProgress { get; set; }
    }
}