using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
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
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }

            // Seed data
            using (var context = new IssueTrackingContext())
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
                        Assignments = 
                            new List<Assignment>
                            {
                                new Assignment { User = divega },
                                new Assignment { User = smitpatel }
                            },
                    }).Entity;

                var enhancement = context.Enhancements.Add(
                    new Enhancement
                    {
                        Issue = issue,
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
                        .Include(r => r.Issues).ThenInclude(i => i.Bug)
                        .Include(r => r.Issues).ThenInclude(i => i.Enhancement)
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
                if (issue.Enhancement != null)
                {
                    Console.WriteLine($"  Enhancement Votes: {issue.Enhancement.Votes}");
                }

                if (issue.Bug != null)
                {
                    Console.WriteLine($"  Bug Repro: {issue.Bug.ReproSteps}");
                }

                Console.WriteLine("  Assignees:");
                foreach (var assignment in issue.Assignments)
                {
                    Console.WriteLine($"    {assignment.User.Name} ({assignment.User.FullName})");
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
