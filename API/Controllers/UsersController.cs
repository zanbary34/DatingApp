using System.Security.Claims;
using API.DTOs;
using API.Entities;
using API.Extenstions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class UsersController(IUserRepository userRepository, IMapper mapper,
    IPhotoService photoService) : BaseApiController
{
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDTO>>> GetUsers()
    {
        var users = await userRepository.GetMembersAsync();
        
        return Ok(users);
    }
    [Authorize]
    [HttpGet("{name}")] // api/users/1
    public async Task<ActionResult<MemberDTO>> GetUserByName(string name)
    {
        var user = await userRepository.GetMemberAsync(name);

        if (user == null) return NotFound();
        
        return Ok(user);
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberupdateDTO memberupdateDto)
    { 
        var user = await userRepository.GetUserByNameAsync(User.GetUsername());

        if (user == null) return BadRequest("Dont find user");

        mapper.Map(memberupdateDto, user);

        if (await userRepository.SaveAllAsync()) return NoContent();

        return BadRequest("Failed to update the user");
    }

    [HttpPost("add-photo")]
    public async Task<ActionResult<PhotoDTO>> AddPhoto(IFormFile file)
    {
        var user = await userRepository.GetUserByNameAsync(User.GetUsername());

        if (user == null) return BadRequest("Cannot update user");

        var result = await photoService.addPhotoAsync(file);

        if (result.Error != null) return BadRequest(result.Error.Message);

        var photo = new Photo
        {
            Url = result.SecureUrl.AbsoluteUri,
            PublicId = result.PublicId
        };
        if (user.Photos?.Count == 0) photo.IsMain = true;
        
        user.Photos?.Add(photo);

        if (await userRepository.SaveAllAsync())
            return CreatedAtAction(nameof(GetUserByName), 
                new {name = user.UserName}, mapper.Map<PhotoDTO>(photo));
        
        return BadRequest("Problem adding photo");
    }

    [HttpPut("set-main-photo/{photoId:int}")]
    public async Task<ActionResult> SetMainPhoto(int photoId)
    {
        var user = await userRepository.GetUserByNameAsync(User.GetUsername());

        if (user == null) return BadRequest("Could not find user");

        var photo = user.Photos?.FirstOrDefault(x => x.Id == photoId);

        if (photo == null || photo.IsMain) return BadRequest("Cannot put this as main photo");

        var currentMain = user.Photos?.FirstOrDefault(x => x.IsMain);

        if (currentMain != null) currentMain.IsMain = false;

        photo.IsMain = true;

        if (await userRepository.SaveAllAsync()) return NoContent();

        return BadRequest("Problem setting main photo");
    }

    [HttpDelete("delete-photo/{photoId:int}")]
    public async Task<ActionResult> DeletePhoto(int photoId)
    {
        var user = await userRepository.GetUserByNameAsync(User.GetUsername());

        if (user == null) return BadRequest("Could not find user");

        var photoToDelete = user.Photos?.FirstOrDefault(x => x.Id == photoId);

        if (photoToDelete == null || photoToDelete.IsMain) return BadRequest("Photo not found");

        if (photoToDelete.PublicId != null)
        {
            var result = await photoService.DeletePhotoAsync(photoToDelete.PublicId);
            if (result.Error != null) return BadRequest(result.Error.Message);
        } 
        user.Photos?.Remove(photoToDelete);

        if (await userRepository.SaveAllAsync()) return NoContent();

        return BadRequest("Problem remove this photo");
    }

}
