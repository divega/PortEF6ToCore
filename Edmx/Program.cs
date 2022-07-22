﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;

namespace PortEF6toCore.Edmx
{
    class Program
    {
        readonly static string repoDetails = "AwsomeEDMXRepo";

        static void Main(string[] args)
        {
            //string connectionString = @"server=.;database=MyIssueTracking;Integrated Security=true;ConnectRetryCount=0"

            // Re - create database
            using (var context = new IssueTrackingContext())
            {
                context.Database.Delete();
                context.Database.CreateIfNotExists();
            }

            PopulatedDataToDB();

            // Execute query and show results
            using (var context = new IssueTrackingContext())
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

        static void PopulatedDataToDB()
        {  
            using (var context = new IssueTrackingContext())
            {
                var giulianop = context.Users.Add(
                   new User
                   {
                       Name = "giulianop",
                       FullName = "Giuliano Pizzocaro"
                   });

                var tinusv = context.Users.Add(
                    new User
                    {
                        Name = "tinusv",
                        FullName = "Tinus Van Eck"
                    });

                var repo = context.Repos.Add(
                    new Repo
                    {
                        Name = repoDetails,
                        CreatedBy = giulianop,
                        CreatedOn = DateTime.Now
                    });

                var issue = context.Issues.Add(
                    new Enhancement
                    {
                        Repo = repo,
                        Title = "Consider porting to EF Core",
                        CreatedBy = giulianop,
                        CreatedOn = DateTime.Now,
                        Assignees = new List<User> { giulianop, tinusv },
                        Votes = 1
                    });

                var comment = context.Comments.Add(
                    new Comment
                    {
                        Issue = issue,
                        Text = "Are we done yet?",
                        CreatedBy = giulianop,
                        CreatedOn = DateTime.Now
                    });

                context.SaveChanges();
            }
        }

        static void WriteOutData(ObservableCollection<Issue> issues)
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

            Console.ReadLine();
        }
    }
}
