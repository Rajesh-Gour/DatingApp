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
    [Authorize]
    [Route("api/users/{userId}/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        public MessagesController(IDatingRepository repo, IMapper mapper)
        {
            this._mapper = mapper;
            this._repo = repo;

        }

        [HttpGet]
        public async Task<ActionResult> GetMessageForUser(int userId,
                                                          [FromQuery]MessageParams messageParams)
        {
           if (userId!=int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
             return Unauthorized();   
            }

            messageParams.UserId= userId;

            var messagesFromRepo = await _repo.GetMessagesForUser(messageParams);

            var messages = _mapper.Map<IEnumerable<MessageToReturn>>(messagesFromRepo);

            Response.AddPagination(messagesFromRepo.CurrentPage,
                                   messagesFromRepo.PageSize,
                                   messagesFromRepo.TotalCount,
                                   messagesFromRepo.TotalPages);
            return Ok(messages);     
        }

        [HttpGet("thread/{recipientId}")]
        public async Task<IActionResult> GetMessageThread(int userId, int recipientId)
        {
            if (userId!=int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
             return Unauthorized();   
            }

            var messagesFromRepo = await _repo.GetMessageThread(userId,recipientId);

            var messageThread= _mapper.Map<IEnumerable<MessageToReturn>>(messagesFromRepo);

            return Ok(messageThread);
        }

        [HttpGet("{Id}",Name ="GetMessage")]
        public async Task<IActionResult> GetMessage(int userId, int Id)
        {
            if (userId!=int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
             return Unauthorized();   
            }

            var messageForRepo = await _repo.GetMessage(Id);

            if (messageForRepo == null)
            {
                return NotFound();
            }

            return Ok(messageForRepo);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userId, MessageForCreationDto messageForCreationDto)
        {
            var sender = await _repo.GetUser(userId);

            if (sender.Id!=int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
             return Unauthorized();   
            }

            messageForCreationDto.SenderId = userId;

            var recipient = await _repo.GetUser(userId);

            if (recipient == null)
            {
                return BadRequest("Could not find user");
            }

            var message= _mapper.Map<Message>(messageForCreationDto);

            _repo.Add(message);

            if (await _repo.SaveAll())
            {
                var messageToReturn = _mapper.Map<MessageToReturn>(message);
                //return CreatedAtRoute(nameof(MessagesController.GetMessage), new {Id = message.Id}, message);
                var messageForRepo = await _repo.GetMessage(message.Id);
                return Ok(messageToReturn);
            }

            throw new Exception("Creating the message failed on save");
        }

        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkMessageAsRead(int userId , int Id) {

            if (userId!=int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
             return Unauthorized();   
            }

            DatingApp.API.Models.Message message = await _repo.GetMessage(Id);

            if (message.RecipientId != userId)
            {
                return Unauthorized();
            }

            message.IsRead = true ;
            message.DateRead = DateTime.Now;

            await _repo.SaveAll();

            return NoContent();
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> DeleteMessage(int id, int userId) {

             if (userId!=int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
             return Unauthorized();   
            }

            var messageFromRepo = await _repo.GetMessage(id);

            if (messageFromRepo.SenderId == userId)
            {
                messageFromRepo.SenderDeleted = true;
            }

            if (messageFromRepo.RecipientId == userId)
            {
                messageFromRepo.RecipientDeleted = true;
            }

            if (messageFromRepo.SenderDeleted && messageFromRepo.RecipientDeleted)
            {
                _repo.Delete(messageFromRepo);
            }

            if (await _repo.SaveAll())
            {
                return NoContent();
            }

            throw new Exception("Error deleting the message");
        }
    }
}