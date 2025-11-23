using API.Data;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class LikesController(IUnitOfWork unitOfWork) : BaseApiController
{
    [HttpPost("{targetMemberId}")]
    public async Task<ActionResult> ToggleLike(string targetMemberId)
    {
        var sourceMemberID = User.GetMemberId();

        if (sourceMemberID == targetMemberId) return BadRequest("You cannot like yourself");
        
        var existingLike = await unitOfWork.LikesRepository.GetMemberLike(sourceMemberID, targetMemberId);

        if (existingLike == null)
        {
            var like = new MemberLike()
            {
                SourceMemberId = sourceMemberID,
                TargetMemberId = targetMemberId
            };
            
            unitOfWork.LikesRepository.AddLike(like);
        }
        else
        {
            unitOfWork.LikesRepository.DeleteLike(existingLike);
        }
        
        if (await unitOfWork.Complete()) return Ok();
        
        return BadRequest("Failed to update like");
    }

    [HttpGet("list")]
    public async Task<ActionResult<IEnumerable<string>>> GetCurrentMemberLikeIds()
    {
        return Ok(await unitOfWork.LikesRepository.GetCurrentMemberLikeIds(User.GetMemberId()));
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Member>>> GetMemberLikes([FromQuery]LikesParams likesParams)
    {
        likesParams.CurrentMemberId = User.GetMemberId();
        return Ok(await unitOfWork.LikesRepository.GetMemberLikes(likesParams));
    }
}