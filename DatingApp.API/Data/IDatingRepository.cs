using System.Threading.Tasks;
using DatingApp.API.Helper;
using DatingApp.API.Models;
using System.Collections.Generic;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Specialized;
namespace DatingApp.API.Data
{
    public interface IDatingRepository
    {
        void Add<T> (T entity) where T: class;
        void Delete<T> (T entity) where T : class;
        Task<bool> SaveAll();
        Task<PagedList<User>> GetUsers(UserParams userParams);
        Task<User> GetUser(int id);
        Task<Photo> GetPhoto(int id);
        Task<Photo> GetMainPhotoForUser(int userId);

        Task<Like> GetLike(int userId, int recipientId); 
    }
}