using System.Collections.Generic;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;

namespace DatingApp.API.Data
{
    public static class Seed
    {
        public static void SeedUser(DataContext context) {

            if (!context.Users.Any())
            {
                var userData=System.IO.File.ReadAllText("Data/UserSeedData.json");
                var users= JsonConvert.DeserializeObject<List<User>>(userData);
                foreach (var user in users)
                {
                    byte[] passwordHash, PasswordSalt;

                    CreatePasswordHash("password", out passwordHash, out PasswordSalt);
                    
                    user.PasswordHash=passwordHash;
                    user.PasswordSalt=PasswordSalt;
                    user.Username=user.Username.ToLower();
                    context.Users.Add(user);
                }
                
                context.SaveChanges();
            }
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            
            
        
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
           
        }
    }
}