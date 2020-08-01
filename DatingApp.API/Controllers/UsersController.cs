using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DatingApp.API.Models;
using DatingApp.API.Dtos;
using System.Collections.Generic;
using System.Security.Claims;
using System;
using DatingApp.API.Helper;

namespace DatingApp.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        public UsersController(IDatingRepository repo, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;
        }


        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetUsers([FromQuery]UserParams userParams)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var isCurrentUser = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value) == userParams.UserId;
            userParams.UserId = currentUserId;

            var userFromRepo = await _repo.GetUser(currentUserId,isCurrentUser);

            if (string.IsNullOrEmpty(userParams.Gender))
            {
                userParams.Gender = userFromRepo.Gender == "male" ? "female" : "male";
            }

            var users = await _repo.GetUsers(userParams);

            var usersToReturn=_mapper.Map<IEnumerable<UserForDetailedDto>>(users);

            Response.AddPagination(users.CurrentPage,users.PageSize,users.TotalCount,users.TotalPages);
            return Ok(usersToReturn);
        }

        [HttpGet("{id}", Name="GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var isCurrentUser = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value) == id;

            var user = await _repo.GetUser(id,isCurrentUser);

            var userToReturn=_mapper.Map<UserForDetailedDto>(user);

            return Ok(userToReturn);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserForUpdateDto userUpdateDto)
        {
            var isCurrentUser = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value) == id;
            

            if (id!=int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
             return Unauthorized();   
            }
            var userFromRepo = await _repo.GetUser(id,isCurrentUser);

            //var userFromRepo=_mapper.Map<UserForDetailedDto>(user);

           _mapper.Map(userUpdateDto,userFromRepo);

           if (await _repo.SaveAll())
           {
             return NoContent();   
           }

           throw new Exception($"Updating user {id} failed on save");
        }

       [HttpPost("{id}/like/{recipientId}")]
        public async Task<IActionResult> LikeUser(int id, int recipientId)
        {
          if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
          {
             return Unauthorized();  
          }

          var like = await _repo.GetLike(id, recipientId);

          if (like != null)
          {
              return BadRequest("You already liked this user");
          }

          if (await _repo.GetUser(id,false) == null)
          {
              return NotFound();
          }

          like = new Like{
              LikerId = id,
              LikeeId = recipientId
          };

          _repo.Add(like);

          if (await _repo.SaveAll())
          {
              return Ok();
          }

          return BadRequest("Failed to like user");

        }
    }
}