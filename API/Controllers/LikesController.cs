using API.Data;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class LikesController(ILikesRepository likesRepository) : BaseApiController
{
    [HttpPost("{targetMemberId}")]
    public async Task<ActionResult> ToggleLike(string targetMemberId)
    {
        var sourceMemberID = User.GetMemberId();

        if (sourceMemberID == targetMemberId) return BadRequest("You cannot like yourself");
        
        var existingLike = await likesRepository.GetMemberLike(sourceMemberID, targetMemberId);

        if (existingLike == null)
        {
            var like = new MemberLike()
            {
                SourceMemberId = sourceMemberID,
                TargetMemberId = targetMemberId
            };
            
            likesRepository.AddLike(like);
        }
        else
        {
            likesRepository.DeleteLike(existingLike);
        }
        
        if (await likesRepository.SaveAllChanges()) return Ok();
        
        return BadRequest("Failed to update like");
    }

    [HttpGet("list")]
    public async Task<ActionResult<IEnumerable<string>>> GetCurrentMemberLikeIds()
    {
        return Ok(await likesRepository.GetCurrentMemberLikeIds(User.GetMemberId()));
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Member>>> GetMemberLikes(string predicate)
    {
        return Ok(await likesRepository.GetMemberLikes(predicate, User.GetMemberId()));
    }
}