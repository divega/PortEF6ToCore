using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace PortEF6ToCore.CodeFirst
{
    public class User
    {
        [Key]
        public string Name { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
    }

    public class Repo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public ICollection<Issue> Issues { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedByName { get; set; }
        public User CreatedBy { get; set; }
    }

    public class Issue
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public ICollection<Assignment> Assignments { get; set; }
        public Repo Repo { get; set; }
        public int RepoId { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedByName { get; set; }
        public User CreatedBy { get; set; }
    }

    public class Comment
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public Issue Issue { get; set; }
        public int IssueId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedByName { get; set; }
        public User CreatedBy { get; set; }
    }

    public class Assignment
    {
        public string UserName { get; set; }
        public int IssueId { get; set; }
        public User User { get; set; }
        public Issue Issue { get; set; }
    }

    public class MyIssueTrackingContext : DbContext
    {
        private readonly string _connectionString;

        public MyIssueTrackingContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Repo> Repos { get; set; }
        public DbSet<Issue> Issues { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Assignment> Assignments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Repo>()
                .ToTable("Repos");

            modelBuilder.Entity<Issue>()
                .HasMany(i => i.Comments)
                .WithOne(c => c.Issue)
                .IsRequired(true)
                .HasForeignKey(c => c.IssueId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Assignment>()
                .HasKey(a => new { a.UserName, a.IssueId });
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = @"server=(localdb)\mssqllocaldb;database=MyIssueTracking;Integrated Security=true;ConnectRetryCount=0";

            // Re-create database
            using (var context = new MyIssueTrackingContext(connectionString))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }

            // Seed data
            using (var context = new MyIssueTrackingContext(connectionString))
            {
                var divega = context.Users.Add(
                    new User
                    {
                        Name = "divega",
                        FullName = "Diego Vega"
                    }).Entity;

                var smitpatel = context.Users.Add(
                    new User
                    {
                        Name = "smitpatel",
                        FullName = "Smit Patel"
                    }).Entity;

                var repo = context.Repos.Add(
                    new Repo
                    {
                        Name = "PortEF6ToCore",
                        CreatedBy = divega,
                        CreatedOn = DateTime.Now
                    }).Entity;

                var issue = context.Issues.Add(
                    new Issue
                    {
                        Repo = repo,
                        Title = "Consider porting to EF Core",
                        CreatedBy = divega,
                        CreatedOn = DateTime.Now,

                    }).Entity;

                context.Assignments.Add(
                    new Assignment
                    {
                        User = divega,
                        Issue = issue
                    });

                context.Assignments.Add(
                    new Assignment
                    {
                        User = smitpatel,
                        Issue = issue
                    });

                var comment = context.Comments.Add(
                    new Comment
                    {
                        Issue = issue,
                        Text = "Are we done yet?",
                        CreatedBy = divega,
                        CreatedOn = DateTime.Now
                    }).Entity;

                context.SaveChanges();
            }

            // Execute query and show results
            using (var context = new MyIssueTrackingContext(connectionString))
            {
                var query =
                    context.Repos.Where(r => r.Name == "PortEF6ToCore")
                        .Include(r => r.CreatedBy)
                        .Include(r => r.Issues).ThenInclude(i => i.Assignments).ThenInclude(a => a.User)
                        .Include(r => r.Issues).ThenInclude(i => i.Comments);

                var results = query.ToList();

                Show(context.Issues.Local.ToObservableCollection());
            }
        }

        static void Show(ObservableCollection<Issue> issues)
        {
            foreach (var issue in issues)
            {
                Console.WriteLine($"Issue #{issue.Id}: {issue.Title} (Created by {issue.CreatedByName} on {issue.CreatedOn})");

                Console.WriteLine("  Assignees:");
                foreach (var assignment in issue.Assignments)
                {
                    var assignee = assignment.User;
                    Console.WriteLine($"    {assignee.Name} ({assignee.FullName})");
                }

                Console.WriteLine("  Comments:");
                foreach (var comment in issue.Comments)
                {
                    Console.WriteLine($"    {comment.Text} (Created by {comment.CreatedByName} on {comment.CreatedOn})");
                }
            }
        }
    }
}