using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace FinanceManagement
{
  public class User
  {
    public int Id { get; set; }
    public string Username { get; set; }
    public UserProfile Profile { get; set; }
  }

  public class UserProfile
  {
    public int Id { get; set; }
    public string Email { get; set; }
    public string Language { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
  }

  public class FinanceContext : DbContext
  {
    public DbSet<User> Users { get; set; }
    public DbSet<UserProfile> Profiles { get; set; }

    public FinanceContext(DbContextOptions<FinanceContext> options) : base(options)
        {
        }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<User>()
          .HasOne(u => u.Profile)
          .WithOne(p => p.User)
          .HasForeignKey<UserProfile>(p => p.UserId);
    }
  }

  public class DatabaseManager
  {
    private readonly FinanceContext _context;

    public DatabaseManager(FinanceContext context)
    {
      _context = context;
    }

    public async Task<User> GetUserWithProfile(int userId)
    {
      var user = await _context.Users
          .Include(u => u.Profile)
          .FirstOrDefaultAsync(u => u.Id == userId);

      return user;
    }

    public async Task<bool> DeleteUser(int userId)
    {
      var user = await _context.Users.FindAsync(userId);
      if (user == null)
        return false;

      _context.Users.Remove(user);
      await _context.SaveChangesAsync();
      return true;
    }
  }
  public class DatabaseManager
  {
    private readonly FinanceContext _context;

    public DatabaseManager(FinanceContext context)
    {
      _context = context;
    }

    public async Task<bool> AddUser(User user)
    {
      try
      {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return true;
      }
      catch
      {
        return false;
      }
    }

    public async Task<bool> UpdateUser(User user)
    {
      try
      {
        _context.Entry(user).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return true;
      }
      catch
      {
        return false;
      }
    }

    public async Task<User> GetUserById(int userId)
    {
      return await _context.Users.FindAsync(userId);
    }

    public async Task<List<User>> GetAllUsers()
    {
      return await _context.Users.ToListAsync();
    }
  }
