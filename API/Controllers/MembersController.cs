using System.Security.Claims;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace API.Controllers;

[Authorize]
public class MembersController(
    IUnitOFWork unitOfWork,
    IPhotoService photoService
    ) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Member>>> GetMembers([FromQuery]MemberParams memberParams)
    {
        memberParams.CurrentMemberId = User.GetMemberId();
        
        return Ok(await unitOfWork.MemberRepository.GetMembersAsync(memberParams));
    }
    
    [HttpGet("{id}")] // localhost:5001/api/members/bob-id
    public async Task<ActionResult<AppUser>> GetMember(string id)
    {
        var member = await unitOfWork.MemberRepository.GetMemberByIdAsync(id);
        
        if (member == null) return NotFound();
            
        return Ok(member);
    }

    [HttpGet("{id}/photos")]
    public async Task<ActionResult<IReadOnlyList<Photo>>> GetMemberPhotos(string id)
    {
        return Ok(await unitOfWork.MemberRepository.GetPhotosForMemberAsync(id));
    }

    [HttpPut]
    public async Task<ActionResult<AppUser>> UpdateMember(MemberUpdateDto memberUpdateDto)
    {
        var memberId = User.GetMemberId();
        
        if (memberId == null) return BadRequest("Oops - no id found in token");
        
        var member = await unitOfWork.MemberRepository.GetMemberForUpdate(memberId);
        
        if (member == null) return NotFound("Could not get member");
        
        member.DisplayName = memberUpdateDto.DisplayName ?? member.DisplayName;
        member.Description = memberUpdateDto.Description ?? member.Description;
        member.City = memberUpdateDto.City ?? member.City;
        member.Country = memberUpdateDto.Country ?? member.Country;
        
        member.User.DisplayName = memberUpdateDto.DisplayName ?? member.User.DisplayName;
        
        // memberRepository.Update(member); // optional

        if (await unitOfWork.Complete()) return NoContent();
        
        return BadRequest("Failed to update member");
    }

    [HttpPost("add-photo")]
    public async Task<ActionResult<Photo>> AddPhoto([FromForm]IFormFile file)
    {
        var member = await unitOfWork.MemberRepository.GetMemberForUpdate(User.GetMemberId());
        
        if (member == null) return NotFound("Could not get member");

        var result = await photoService.UploadPhotoAsync(file);
        
        if (result.Error != null) return BadRequest(result.Error.Message);

        var photo = new Photo
        {
            Url = result.SecureUrl.AbsoluteUri,
            PublicId = result.PublicId,
            MemberId = member.Id
        };

        if (member.ImageUrl != null)
        {
            member.ImageUrl = photo.Url;
            member.User.ImageUrl = photo.Url;
        }
        
        member.Photos.Add(photo);
        
        if (await unitOfWork.Complete()) return Ok(photo);
        
        return BadRequest("Failed to add photo");
    }

    [HttpPut("set-main-photo/{photoId}")]
    public async Task<ActionResult<Photo>> SetMainPhoto(int photoId)
    {
        var member = await unitOfWork.MemberRepository.GetMemberForUpdate(User.GetMemberId());
        
        if (member == null) return BadRequest("Could not get member");
        
        var photo = member.Photos.SingleOrDefault(x => x.Id == photoId);

        if (member.ImageUrl == photo?.Url || photo == null)
        {
            return BadRequest("Cannot set this as main image");
        }
        
        member.ImageUrl = photo.Url;
        member.User.ImageUrl = photo.Url;
        
        if (await unitOfWork.Complete()) return NoContent();
        
        return BadRequest("Cannot set main photo");
    }

    [HttpDelete("delete-photo/{photoId}")]
    public async Task<ActionResult<Photo>> DeletePhoto(int photoId)
    {
        var member = await unitOfWork.MemberRepository.GetMemberForUpdate(User.GetMemberId());
        
        if (member == null) return NotFound("Could not get member");
        
        var photo = member.Photos.SingleOrDefault(x => x.Id == photoId);
        
        if (photo == null || photo.Url == member.ImageUrl) return NotFound("This photo cannot be deleted");

        if (photo.PublicId != null)
        {
            var result = await photoService.DeletePhotoAsync(photo.PublicId);
            if (result.Error != null) return BadRequest(result.Error.Message);
        }
        
        member.Photos.Remove(photo);
        
        if (await unitOfWork.Complete()) return Ok();
        
        return BadRequest("Problem deleting the photo");
    }
}