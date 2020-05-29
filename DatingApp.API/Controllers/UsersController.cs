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

namespace DatingApp.API.Controllers
{
    [Authorize]
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
        public async Task<IActionResult> GetUsers()
        {

            var users = await _repo.GetUsers();

            var usersToReturn=_mapper.Map<IEnumerable<UserForDetailedDto>>(users);

            return Ok(usersToReturn);
        }

        [HttpGet("GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {

            var user = await _repo.GetUser(id);

            var userToReturn=_mapper.Map<UserForDetailedDto>(user);

            return Ok(userToReturn);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserForUpdateDto userUpdateDto)
        {
            if (id!=int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
             return Unauthorized();   
            }
            var userFromRepo = await _repo.GetUser(id);

            //var userFromRepo=_mapper.Map<UserForDetailedDto>(user);

           _mapper.Map(userUpdateDto,userFromRepo);

           if (await _repo.SaveAll())
           {
             return NoContent();   
           }

           throw new Exception($"Updating user {id} failed on save");
        }

    }
}