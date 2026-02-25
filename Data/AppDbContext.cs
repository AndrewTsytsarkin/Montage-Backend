 
using Microsoft.EntityFrameworkCore;
 

namespace MontageAPI.Data;
public class User
{
    public int Id { get; set; }
    public string Login { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "Worker"; // Admin или Worker
    public ICollection<UserObjectAssignment> Assignments { get; set; } = new List<UserObjectAssignment>();
}
public class UserObjectAssignment
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int ObjectId { get; set; }
    public ProjectObject Object { get; set; } = null!;
}
public class ProjectObject
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Status { get; set; } = "New";
    public ICollection<UserObjectAssignment> Assignments { get; set; } = new List<UserObjectAssignment>();
}

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<ProjectObject> ProjectObjects => Set<ProjectObject>();
    public DbSet<UserObjectAssignment> UserObjectAssignments => Set<UserObjectAssignment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserObjectAssignment>()
            .HasKey(a => new { a.UserId, a.ObjectId });

        modelBuilder.Entity<UserObjectAssignment>()
            .HasOne(a => a.User)
            .WithMany(u => u.Assignments)
            .HasForeignKey(a => a.UserId);

        modelBuilder.Entity<UserObjectAssignment>()
            .HasOne(a => a.Object)
            .WithMany(o => o.Assignments)
            .HasForeignKey(a => a.ObjectId);
    }
}