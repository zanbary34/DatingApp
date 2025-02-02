using System.Security.Claims;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class UsersController(IUserRepository userRepository, IMapper mapper) : BaseApiController
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
        var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (username == null) return BadRequest("No username found in the token");

        var user = await userRepository.GetUserByNameAsync(username);

        if (user == null) return BadRequest("Dont find user");

        mapper.Map(memberupdateDto, user);

        if (await userRepository.SaveAllAsync()) return NoContent();

        return BadRequest("Failed to update the user");
    }


    


}
