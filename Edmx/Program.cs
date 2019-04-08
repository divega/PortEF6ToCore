using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;

namespace PortEF6toCore.Edmx
{
    class Program
    {
        static void Main(string[] args)
        {
            // Re - create database
            using (var context = new IssueTrackingContext())
            {
                context.Database.Delete();
                context.Database.CreateIfNotExists();
            }

            // Seed data
            using (var context = new IssueTrackingContext())
            {
                var divega = context.Users.Add(
                    new User
                    {
                        Name = "divega",
                        FullName = "Diego Vega"
                    });

                var smitpatel = context.Users.Add(
                    new User
                    {
                        Name = "smitpatel",
                        FullName = "Smit Patel"
                    });

                var repo = context.Repos.Add(
                    new Repo
                    {
                        Name = "PortEF6ToCore",
                        CreatedBy = divega,
                        CreatedOn = DateTime.Now
                    });

                var issue = context.Issues.Add(
                    new Enhancement
                    {
                        Repo = repo,
                        Title = "Consider porting to EF Core",
                        CreatedBy = divega,
                        CreatedOn = DateTime.Now,
                        Assignees = new List<User> { divega, smitpatel },
                        Votes = 1
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
            using (var context = new IssueTrackingContext())
            {
                var query =
                    context.Repos.Where(r => r.Name == "PortEF6ToCore")
                        .Include(r => r.CreatedBy)
                        .Include(r => r.Issues.Select(i => i.Assignees))
                        .Include(r => r.Issues.Select(i => i.Comments));

                var results = query.ToList();

                Show(context.Issues.Local);
            }
        }

        static void Show(ObservableCollection<Issue> issues)
        {
            foreach (var issue in issues)
            {
                Console.WriteLine($"Issue #{issue.Id}: {issue.Title} (Created by {issue.CreatedByName} on {issue.CreatedOn})");
                switch (issue)
                {
                    case Enhancement enhancement:
                        Console.WriteLine($"  Enhancement Votes: {enhancement.Votes}");
                        break;
                    case Bug bug:
                        Console.WriteLine($"  Bug Repro: {bug.ReproSteps}");
                        break;
                }

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
        }
    }
}
