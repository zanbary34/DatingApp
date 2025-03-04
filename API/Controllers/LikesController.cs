using API.DTOs;
using API.Entities;
using API.Extenstions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{

    public class LikesController(ILikesRepository likesRepository) : BaseApiController
    {
        [HttpPost("{targetUserId:int}")]
        public async Task<ActionResult> ToggleLike(int targetUserId)
        {
            var sourceUserId = User.GetUserId();

            if (sourceUserId == targetUserId) return BadRequest("You cannot like yourself");

            var existinglike = await likesRepository.GetUserLike(sourceUserId, targetUserId);

            if (existinglike == null)
            {
                var like = new UserLike
                {
                    SourceUserId = sourceUserId,
                    TargetUserId = targetUserId
                };

                likesRepository.AddLike(like);
            }
            else
            {
                likesRepository.DeleteLike(existinglike);
            }

            if (await likesRepository.SaveChanges()) return Ok();

            return BadRequest("failed to update like");
        }

        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<int>>> GetCurrentUserLikeIds()
        {
            
            return Ok(await likesRepository.GetCurrentUserLikeIds(User.GetUserId()));
        }

        [HttpGet()]
        public async Task<ActionResult<IEnumerable<MemberDTO>>> GetUserLike([FromQuery]LikesParams likesParams)
        {
            var users = await likesRepository.GetUserLikes(likesParams, User.GetUserId());
            
            Response.AddPaginationHeader(users);

            return Ok(users);
        }
    }
}
