using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
namespace LilCloudServerConsole.Database

{
    public class CloudContext : DbContext
    {
        public DbSet<FileData> Files { get; set; }
        public DbSet<User> Users { get; set; }
        private string _dbPath { get; }

        public CloudContext()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            _dbPath = Path.Join(path, "lilcloud.db");
        }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={_dbPath}");
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FileData>()
                .HasOne(f => f.Owner)
                .WithMany(u => u.Files)
                .HasForeignKey(f => f.UserId);
            //popraw na appsettings
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Name = "admin",
                Password = "admin",
                IsAdmin = true
            });

        }
    }
}
public class FileData
{
    public int Id { get; set; } 
    public string FileName { get; set; } = string.Empty;
    public string FileSavePath { get; set; } = string.Empty;
    public int UserId { get; set; }
    public User? Owner { get; set; }

    public FileData() { }

    public override string ToString()
    {
        string s = $"File name: {FileName}" +
            $"To save at: {FileSavePath}" +
            $"Belonging to User with Id: {UserId}";
        return s;
    }
}

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }
    public ICollection<FileData>? Files { get; set; }

    public User() { }

}
