using System.Collections.Generic;
using System.Linq;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
//using DatingApp.API.Models;
namespace DatingApp.API.Data
{
    public class DataContext:IdentityDbContext<User,Role,int,IdentityUserClaim<int>,
    UserRole,IdentityUserLogin<int>,IdentityRoleClaim<int>,IdentityUserToken<int>>
    {
        //My sql connection string
        //"DefaultConnection":"Server=localhost;Database=datingapp;Uid=appuser;pwd=password"
        //Sql server connection string
        //"DefaultConnection":"Server=localhost;Database=datingapp;User Id=appuser;Password=password"
        public DataContext(DbContextOptions<DataContext> options):base(options)
        {


        }

        public DbSet<Value> Values {get;set;}
        //public DbSet<User> Users{ get; set; }
        public DbSet<Photo> Photos{ get; set; }
        public DbSet<Like> Likes{ get; set; }
        public DbSet<Message> Messages { get; set; }
    
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<UserRole>(userRole =>
            {
                userRole.HasKey(ur => new {ur.UserId, ur.RoleId});

                userRole.HasOne(ur => ur.Role)
                        .WithMany(r => r.UserRoles)
                        .HasForeignKey(ur =>ur.RoleId)
                        .IsRequired();

                userRole.HasOne(ur => ur.User)
                        .WithMany(r => r.UserRoles)
                        .HasForeignKey(ur =>ur.UserId)
                        .IsRequired();

            });
           // form primary key with likeid and likerid 
              builder.Entity<Like>() 
                 .HasKey(k => new {k.LikerId, k.LikeeId}); 
 
 
             builder.Entity<Like>() 
                 .HasOne( u => u.Likee) 
                 .WithMany( u => u.Likers) 
                 .HasForeignKey(u => u.LikeeId) 
                 .OnDelete(DeleteBehavior.Restrict); 
              
             builder.Entity<Like>() 
                 .HasOne( u => u.Liker) 
                 .WithMany( u => u.Likees) 
                 .HasForeignKey(u => u.LikerId) 
                 .OnDelete(DeleteBehavior.Restrict); 

                 //Message relationship
                 builder.Entity<Message>() 
                 .HasOne( u => u.Sender) 
                 .WithMany( m => m.MessagesSent) 
                 .OnDelete(DeleteBehavior.Restrict); 

                 builder.Entity<Message>() 
                 .HasOne( u => u.Recipient) 
                 .WithMany( m => m.MessagesRecieved) 
                 .OnDelete(DeleteBehavior.Restrict); 

                 builder.Entity<Photo>().HasQueryFilter(p=>p.IsApproved);
 
        }
    }

    internal static class DbSetExtensions
    {
    public static IAsyncEnumerable<T> AsAsyncEnumerable<T>(this DbSet<T> dbSet)
        where T : class
    {
        return dbSet;
    }

    public static IQueryable<T> AsQueryable<T>(this DbSet<T> dbSet)
        where T : class
    {
        return dbSet;
    }
}
}