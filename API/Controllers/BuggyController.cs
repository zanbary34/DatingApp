
using API.Controllers;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;

public class BuggyController(DataContext context): BaseApiController 
{
    [HttpGet("auth")]
    public ActionResult<string> GetAuth()
    {
        return "secret text";
    }

    [HttpGet("not-found")]
    public ActionResult<AppUser> GetNotFound()
    {
        var thing = context.Users.Find(-1);
        
        if (thing == null) return NotFound();
        
        return thing;
    }

    [HttpGet("server-error")]
    public ActionResult<AppUser> GetServiceError()
    {
        var thing = context.Users.Find(-1) ?? throw new Exception("a bad thing happend");

        return thing;
    }

    [HttpGet("bad-request")]
    public ActionResult<AppUser> GetBadrequest()
    {
       return BadRequest("This was not a good request");
    }
}