
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using DatingApp.API.Data;
using DatingApp.API.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using System.Security.Claims;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;
using CloudinaryDotNet.Actions;
using System.Linq;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Microsoft.AspNetCore.Mvc.Route("api/users/{userid}/photos")]
    [ApiController]
    public class PhotosController: ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinarySettings;
        private Cloudinary _cloudnary;

        public PhotosController(
            IDatingRepository repo, 
            IMapper mapper, 
            IOptions<CloudinarySettings> cloudinarySettings)
        {
            this._repo = repo;
            this._mapper = mapper;
            this._cloudinarySettings = cloudinarySettings;

            Account acc=new Account(

                _cloudinarySettings.Value.CloudName,
                _cloudinarySettings.Value.ApiKey,
                _cloudinarySettings.Value.ApiSecret

            );

            _cloudnary = new Cloudinary(acc);
        }

        
        [HttpGet("{id}", Name="GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            
            //return Ok("id from GetPhoto" + id);
            var photoFromRepo= await _repo.GetPhoto(id);
            var photo =  _mapper.Map<PhotosForReturnDto>(photoFromRepo);
            return Ok(photo);
        }
        
        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId, 
        [FromForm] PhotoForCreationDto photoForCreationDto)
        {
            
            if (userId!=int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
             return Unauthorized();   
            }
            var userFromRepo = await _repo.GetUser(userId);
            
            var file=photoForCreationDto.File;
            var uploadResult= new ImageUploadResult();
            
            if (file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    ImageUploadParams uploadParams= new ImageUploadParams()
                    {
                        File= new FileDescription(file.Name, stream),
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                    };
                    
                    uploadResult =_cloudnary.Upload(uploadParams);
                }
            }
 
            photoForCreationDto.Url=uploadResult.Url.ToString();
            photoForCreationDto.PublicId = uploadResult.PublicId;

            var photo = _mapper.Map<Photo>(photoForCreationDto);
            if (!userFromRepo.Photos.Any(u => u.IsMain))
            {
                photo.IsMain = true; 
            }

            userFromRepo.Photos.Add(photo);

            if (await _repo.SaveAll())
            {
                
                    var photoToReturn=_mapper.Map<PhotosForReturnDto>(photo);
                    //return CreatedAtRoute("api/users/1/photos/GetPhoto", new {id=photo.Id});
                    return Ok(photoToReturn);
            }
            return BadRequest("Could not add the photo");
        }

        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMainInPhoto(int userId, int id)
        {
            if (userId!=int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
             return Unauthorized();   
            }
            var user = await _repo.GetUser(userId);

            if (!user.Photos.Any(p=>p.Id == id))
            {
                return Unauthorized();
            }

            var photoFromRepo = await _repo.GetPhoto(id);

            if(photoFromRepo.IsMain)
            return BadRequest("This is already the main photo");

            var currentMainPhoto= await _repo.GetMainPhotoForUser(userId);
            currentMainPhoto.IsMain = false;

            photoFromRepo.IsMain = true;

            if(await _repo.SaveAll())
            return NoContent();

            return BadRequest("Could not set photo to main");
        }

        [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePhoto(int userId, int id)
    {

         if (userId!=int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
             return Unauthorized();   
            }
            var user = await _repo.GetUser(userId);

            if (!user.Photos.Any(p=>p.Id == id))
            {
                return Unauthorized();
            }

            var photoFromRepo = await _repo.GetPhoto(id);

            if(photoFromRepo.IsMain)
            return BadRequest("You can't delete your main photo");

            if (photoFromRepo.PublicId !=null)
            {
                var deleteParams = new DeletionParams(photoFromRepo.PublicId);

                var result= _cloudnary.Destroy(deleteParams);

                if (result.Result == "ok") {

                    _repo.Delete(photoFromRepo);
                } 
            }
            
            if (photoFromRepo.PublicId == null)
            {
                _repo.Delete(photoFromRepo);
            }
            if (await  _repo.SaveAll())
            {
                return Ok();
            }
            
            return BadRequest("Failed to delete the photo");       
    }
    }
}
