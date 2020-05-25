using System.Collections.Generic;
using System.Linq;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;
//using DatingApp.API.Models;
namespace DatingApp.API.Data
{
    public class DataContext:DbContext
    {
        public DataContext(DbContextOptions<DataContext> options):base(options)
        {


        }

    public DbSet<Value> Values {get;set;}
    public DbSet<User> Users{ get; set; }
    public DbSet<Photo> Photos{ get; set; }

    
    
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