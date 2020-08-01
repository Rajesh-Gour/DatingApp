using DatingApp.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using DatingApp.API.Helper;
using System;
using System.Collections.Specialized;
using DatingApp.API.Dtos;
using Microsoft.AspNetCore.Identity;
using CloudinaryDotNet;
using Microsoft.Extensions.Options;
using CloudinaryDotNet.Actions;

namespace DatingApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly UserManager<User> _userManager;
        private readonly IOptions<CloudinarySettings> _cloudinarySettings;
        private Cloudinary _cloudnary;
        
        public AdminController(DataContext dataContext, 
                               UserManager<User> userManager,
                               IOptions<CloudinarySettings> cloudinarySettings)
        {
            _userManager = userManager;
            _dataContext = dataContext;

            this._cloudinarySettings = cloudinarySettings;

            Account acc=new Account(

                _cloudinarySettings.Value.CloudName,
                _cloudinarySettings.Value.ApiKey,
                _cloudinarySettings.Value.ApiSecret

            );

            _cloudnary = new Cloudinary(acc);
            
        }

        [Authorize(Policy="ModeratePhotoRole")]
        [HttpGet("photosForModeration")]
        public async Task<IActionResult> GetPhotosForModeration()
        {
            var photos = await _dataContext.Photos
                                .Include(u => u.User)
                                .IgnoreQueryFilters()
                                .Where(p => p.IsApproved == false)
                                .Select(u => new {
                                        Id=u.Id,
                                        UserName=u.User.UserName,
                                        Url = u.Url,
                                        IsApproved = u.IsApproved
                                        }).ToListAsync();

            return Ok(photos);
        }

        [Authorize(Policy="ModeratePhotoRole")]
        [HttpPost("approvePhoto/{photoId}")]
        public async Task<IActionResult> ApprovePhoto(int photoId)
        {
            var photo = await _dataContext.Photos
                                .IgnoreQueryFilters()
                                .FirstOrDefaultAsync(p => p.Id == photoId);
                photo.IsApproved = true;
                await _dataContext.SaveChangesAsync();
            return Ok();
        }

        [Authorize(Policy="ModeratePhotoRole")]
        [HttpPost("rejectPhoto/{photoId}")]
        public async Task<IActionResult> RejectPhoto(int photoId)
        {
            var photo = await _dataContext.Photos
                                .IgnoreQueryFilters()
                                .FirstOrDefaultAsync(p => p.Id == photoId);
                
                if(photo.IsMain)
                    return BadRequest("You cannot reject the main photo");
                
                if (photo.PublicId!=null)
                {
                    var deleteParam = new DeletionParams(photo.PublicId);

                    var result = _cloudnary.Destroy(deleteParam);

                    if (result.Result =="ok")
                    {
                        _dataContext.Photos.Remove(photo);
                    }
                }

                if (photo.PublicId == null)
                {
                    _dataContext.Photos.Remove(photo);
                }

                await _dataContext.SaveChangesAsync();
                
            return Ok("Photo rejected");
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("usersWithRoles")]
        public async Task<IActionResult> GetUserwithRoles()
        {
            var userList = await _dataContext.Users
                .OrderBy(x => x.UserName)
                .Select(user => new
                {

                    Id = user.Id,
                    UserName = user.UserName,
                    Roles = (from userRole in user.UserRoles
                             join role in _dataContext.Roles
                             on userRole.RoleId
                             equals role.Id
                             select role.Name).ToList()
                }).ToListAsync();

            return Ok(userList);
        }

         [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("editRoles/{userName}")]
        public async Task<IActionResult> EditRoles(string userName, RoleEditDto roleEditDto)
        {
            var user = await _userManager.FindByNameAsync(userName);

            var userRoles = await _userManager.GetRolesAsync(user);

            var selectedRoles = roleEditDto.RoleNames;

            selectedRoles = selectedRoles ??  new string[] {};

            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

            if (!result.Succeeded)
            {
                return BadRequest("Failed to add to roles");
            }

            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

            if (!result.Succeeded)
            {
                return BadRequest("Failed to remove the roles");
            }

            return Ok(await _userManager.GetRolesAsync(user));
        }

        // [Authorize(Policy = "ModeratePhotoRole")]
        // [HttpGet("photosForModeration")]
        // public IActionResult GetPhotosForModeration()
        // {
        //     return Ok("Admin and Moderator can see this method");
        // }
    }
}