using Globals.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class Backend_DormScoutContext : IdentityDbContext<
        User, 
        Role, 
        Guid, 
        IdentityUserClaim<Guid>, 
        UserRole, 
        IdentityUserLogin<Guid>, 
        IdentityRoleClaim<Guid>, 
        IdentityUserToken<Guid>
        >
    {
        public Backend_DormScoutContext() { }

        public Backend_DormScoutContext(DbContextOptions<Backend_DormScoutContext> options) : base(options) { }

        public DbSet<User> Users {  get; set; }
        public DbSet<Role> Roles {  get; set; }
        public DbSet<UserRole> UserRoles {  get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Place> Places { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Assessment> Assessments { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<PossibleDate> PossibleDates { get; set; }
        public DbSet<Coordinate> Coordinates { get; set; }
        public DbSet<ReviewImage> ReviewImages { get; set; }
        public DbSet<PlaceImage> PlaceImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Email)
                .IsUnique();

                // Each User can have many UserClaims
                entity.HasMany(e => e.Claims)
                    .WithOne()
                    .HasForeignKey(uc => uc.UserId)
                    .IsRequired();

                // Each User can have many UserLogins
                entity.HasMany(e => e.Logins)
                    .WithOne()
                    .HasForeignKey(ul => ul.UserId)
                    .IsRequired();

                // Each User can have many UserTokens
                entity.HasMany(e => e.Tokens)
                    .WithOne()
                    .HasForeignKey(ut => ut.UserId)
                    .IsRequired();

                // Each User can have many entries in the UserRole join table
                entity.HasMany(e => e.UserRoles)
                    .WithOne()
                    .HasForeignKey(ur => ur.UserId)
                .IsRequired();
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasMany(e => e.UserRoles)
                .WithOne(e => e.Role)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });

                entity.HasOne(x => x.User)
                .WithMany(x => x.UserRoles)
                .HasForeignKey(x => x.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Role)
                .WithMany(x => x.UserRoles)
                .HasForeignKey(x => x.RoleId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Place>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(x => x.Requester)
                .WithMany(x => x.YourPlaces)
                .HasForeignKey(x => x.RequesterId)
                .IsRequired();

                entity.HasOne(x => x.Reviewer)
                .WithMany(x => x.YourReviewPlaces)
                .HasForeignKey(x => x.ReviewerId)
                .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(x => x.Notes)
                .WithOne(x => x.Place)
                .HasForeignKey(x => x.PlaceId);

                entity.HasMany(x => x.Dates)
                .WithOne(x => x.Place)
                .HasForeignKey(x => x.PlaceId)
                .IsRequired();

                entity.HasOne(x => x.Coordinate)
                .WithMany(x => x.Places)
                .HasForeignKey(x => x.CoordinateId)
                .IsRequired();

                entity.HasOne(x => x.Review)
                .WithOne(x => x.Place)
                .HasForeignKey<Place>(x => x.ReviewId)
                .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(x => x.Images)
                .WithOne(x => x.Place)
                .HasForeignKey(x => x.PlaceId);

            });

            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(x => x.Place)
                .WithOne(x => x.Review)
                .HasForeignKey<Review>(x => x.PlaceId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Assessment)
                .WithOne(x => x.Review)
                .HasForeignKey<Review>(x => x.AssessmentId)
                .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(x => x.Images)
                .WithOne(x => x.Review)
                .HasForeignKey(x => x.ReviewId);
            });

            modelBuilder.Entity<Assessment>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(x => x.Review)
                .WithOne(x => x.Assessment)
                .HasForeignKey<Assessment>(x => x.ReviewId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Note>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(x => x.Place)
                .WithMany(x => x.Notes)
                .HasForeignKey(x => x.PlaceId)
                .IsRequired();
            });

            modelBuilder.Entity<PossibleDate>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(x => x.Place)
                .WithMany(x => x.Dates)
                .HasForeignKey(x => x.PlaceId)
                .IsRequired();
            });

            modelBuilder.Entity<Coordinate>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasMany(x => x.Places)
                .WithOne(x => x.Coordinate)
                .HasForeignKey(x => x.CoordinateId)
                .IsRequired();
            });

            modelBuilder.Entity<ReviewImage>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(x => x.Review)
                .WithMany(x => x.Images)
                .HasForeignKey(x => x.ReviewId)
                .IsRequired();
            });

            modelBuilder.Entity<PlaceImage>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(x => x.Place)
                .WithMany(x => x.Images)
                .HasForeignKey(x => x.PlaceId)
                .IsRequired();
            });
        }
    }
}
