using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace PortEF6toCore.Edmx
{
    public partial class IssueTrackingContext : DbContext
    {
        public IssueTrackingContext()
        {
        }

        public IssueTrackingContext(DbContextOptions<IssueTrackingContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Assignment> Assignments { get; set; }
        public virtual DbSet<Bug> Bugs { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<Enhancement> Enhancements { get; set; }
        public virtual DbSet<Issue> Issues { get; set; }
        public virtual DbSet<Repo> Repos { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // TODO: Move connection string to configuration
                optionsBuilder.UseSqlServer("server=(localdb)\\mssqllocaldb;database=IssueTracking;Integrated Security=true;ConnectRetryCount=0");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.0-rtm-35687");

            modelBuilder.Entity<Assignment>(entity =>
            {
                entity.HasKey(e => new { e.IssueId, e.UserName })
                    .HasName("PK__Assignme__00193E41E3908724");

                entity.Property(e => e.UserName).HasMaxLength(128);

                entity.HasOne(d => d.Issue)
                    .WithMany(p => p.Assignments)
                    .HasForeignKey(d => d.IssueId)
                    .HasConstraintName("Issue_Assignees_Source");

                entity.HasOne(d => d.User)
                    .WithMany()
                    .HasForeignKey(d => d.UserName)
                    .HasConstraintName("Issue_Assignees_Target");
            });

            modelBuilder.Entity<Bug>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.ReproSteps).IsRequired();

                entity.HasOne(d => d.Issue)
                    .WithOne(p => p.Bug)
                    .HasForeignKey<Bug>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Bugs_Issues");
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.Property(e => e.CreatedByName).HasMaxLength(128);

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.HasOne(d => d.CreatedBy)
                    .WithMany()
                    .HasForeignKey(d => d.CreatedByName)
                    .HasConstraintName("Comment_CreatedBy");

                entity.HasOne(d => d.Issue)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.IssueId)
                    .HasConstraintName("Issue_Comments");
            });

            modelBuilder.Entity<Enhancement>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.Issue)
                    .WithOne(p => p.Enhancement)
                    .HasForeignKey<Enhancement>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Enhancements_Issues");
            });

            modelBuilder.Entity<Issue>(entity =>
            {
                entity.Property(e => e.CreatedByName).HasMaxLength(128);

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.HasOne(d => d.CreatedBy)
                    .WithMany()
                    .HasForeignKey(d => d.CreatedByName)
                    .HasConstraintName("Issue_CreatedBy");

                entity.HasOne(d => d.Repo)
                    .WithMany(p => p.Issues)
                    .HasForeignKey(d => d.RepoId)
                    .HasConstraintName("Repo_Issues");
            });

            modelBuilder.Entity<Repo>(entity =>
            {
                entity.Property(e => e.CreatedByName).HasMaxLength(128);

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.HasOne(d => d.CreatedBy)
                    .WithMany()
                    .HasForeignKey(d => d.CreatedByName)
                    .HasConstraintName("Repo_CreatedBy");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Name)
                    .HasName("PK__Users__737584F7204B6071");

                entity.Property(e => e.Name)
                    .HasMaxLength(128)
                    .ValueGeneratedNever();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}