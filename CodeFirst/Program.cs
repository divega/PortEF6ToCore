using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
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
        public ICollection<User> Assignees { get; set; }
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

    public class MyIssueTrackingContext : DbContext
    {

        public MyIssueTrackingContext() : base()
        {
        }
        public MyIssueTrackingContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Repo> Repos { get; set; }
        public DbSet<Issue> Issues { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Repo>()
                .ToTable("Repos");

            modelBuilder.Entity<Issue>()
                .HasMany(i => i.Assignees)
                .WithMany()
                .Map(linkTable => linkTable
                    .ToTable("Assignments")
                    .MapLeftKey("IssueId")
                    .MapRightKey("UserName"));

            modelBuilder.Entity<Issue>()
                .HasMany(i => i.Comments)
                .WithRequired(c => c.Issue)
                .HasForeignKey(c => c.IssueId)
                .WillCascadeOnDelete(true);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = @"server=.;database=MyIssueTracking;Integrated Security=true;ConnectRetryCount=0";
            string repoDetails = "AwsomeRepo";

            // Re-create database
            using (var context = new MyIssueTrackingContext(connectionString))
            {
                context.Database.Delete();
                context.Database.CreateIfNotExists();
            }

            // populated data
            using (var context = new MyIssueTrackingContext(connectionString))
            {
                var divega = context.Users.Add(
                    new User
                    {
                        Name = "giulianop",
                        FullName = "Giuliano Pizzocaro"
                    });

                var smitpatel = context.Users.Add(
                    new User
                    {
                        Name = "tinusv",
                        FullName = "Tinus Van Eck"
                    });

                var repo = context.Repos.Add(
                    new Repo
                    {
                        Name = repoDetails,
                        CreatedBy = divega,
                        CreatedOn = DateTime.Now
                    });

                var issue = context.Issues.Add(
                    new Issue
                    {
                        Repo = repo,
                        Title = "Consider porting to EF Core",
                        CreatedBy = divega,
                        CreatedOn = DateTime.Now,
                        Assignees = new List<User> { divega, smitpatel }
                    });

                var comment = context.Comments.Add(
                    new Comment
                    {
                        Issue = issue,
                        Text = "Are we done yet?",
                        CreatedBy = divega,
                        CreatedOn = DateTime.Now
                    });

                context.SaveChanges();
            }

            // Execute query and show results
            using (var context = new MyIssueTrackingContext(connectionString))
            {
                var query =
                    context.Repos.Where(r => r.Name == repoDetails)
                        .Include(r => r.CreatedBy)
                        .Include(r => r.Issues.Select(i => i.Assignees))
                        .Include(r => r.Issues.Select(i => i.Comments));

                var results = query.ToList();

                WriteOutData(context.Issues.Local);
            }
        }

        static void WriteOutData(ObservableCollection<Issue> issues)
        {
            foreach (var issue in issues)
            {
                Console.WriteLine($"Issue #{issue.Id}: {issue.Title} (Created by {issue.CreatedByName} on {issue.CreatedOn})");

                Console.WriteLine("  Assignees:");
                foreach (var assignee in issue.Assignees)
                {
                    Console.WriteLine($"    {assignee.Name} ({assignee.FullName})");
                }

                Console.WriteLine("  Comments:");
                foreach (var comment in issue.Comments)
                {
                    Console.WriteLine($"    {comment.Text} (Created by {comment.CreatedByName} on {comment.CreatedOn})");
                }
            }

            Console.ReadLine();
        }
    }
}
