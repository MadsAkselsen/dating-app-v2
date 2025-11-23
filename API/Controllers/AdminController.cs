using API.Entities;
using API.Interfaces;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AdminController(IUnitOfWork unitOfWork, UserManager<AppUser> userManager, IPhotoService photoService) : BaseApiController
{
    [Authorize(Policy = "RequireAdminRole")]
    [HttpGet("users-with-roles")]
    public async Task<ActionResult> GetUsersWithRoles()
    {
        var users = await userManager.Users.ToListAsync();
        var userList = new List<object>();

        foreach (var user in users)
        {
            var roles = await userManager.GetRolesAsync(user);
            userList.Add(new
            {
                user.Id,
                user.Email,
                Roles = roles.ToList()
            });
        }
        
        return Ok(userList);
    }
    
    [Authorize(Policy = "RequireAdminRole")]
    [HttpPost("edit-roles/{userId}")]
    public async Task<ActionResult<IList<string>>> EditRoles(string userId, [FromQuery]string roles)
    {
        if (string.IsNullOrEmpty(roles)) return BadRequest("You must select at least one role");
        
        var selectedRoles = roles.Split(",").ToArray();
        
        var user = await userManager.FindByIdAsync(userId);
        
        if (user == null) return BadRequest("User not found");
        
        var userRoles = await userManager.GetRolesAsync(user);
        
        var result = await userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));
        
        if (!result.Succeeded) return BadRequest("Failed to add roles to user");
        
        result = await userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
        
        if (!result.Succeeded) return BadRequest("Failed to remove roles from user");
        
        return Ok(await userManager.GetRolesAsync(user));
    }
    
    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpGet("photos-to-moderate")]
    public async Task<ActionResult<IList<Photo>>> GetPhotosForModeration()
    {
        var photos = await unitOfWork.PhotoRepository.GetUnapprovedPhotos();
        
        return Ok(photos);
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpPost("approve-photo/{photoId}")]
    public async Task<ActionResult> ApprovePhoto(int photoId)
    {
        var photo = await unitOfWork.PhotoRepository.GetPhotoByIdAsync(photoId);
        
        if (photo == null) return BadRequest("Photo not found");
        
        var member = await unitOfWork.MemberRepository.GetMemberForUpdate(photo.MemberId);
        
        if (member == null) return BadRequest("Member not found");

        photo.IsApproved = true;
        
        if (member.ImageUrl == null)
        {
            member.ImageUrl = photo.Url;
            member.User.ImageUrl = photo.Url;
        }

        if (await unitOfWork.Complete()) return Ok();
        
        return BadRequest("Failed to approve photo");
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpPost("reject-photo/{photoId}")]
    public async Task<ActionResult> RejectPhoto(int photoId)
    {
        var photo = await unitOfWork.PhotoRepository.GetPhotoByIdAsync(photoId);
        if (photo == null) return BadRequest("Could not get photo from db");

        if (photo.PublicId != null)
        {
            var result = await photoService.DeletePhotoAsync(photo.PublicId);

            if (result.Result == "ok")
            {
                unitOfWork.PhotoRepository.RemovePhoto(photo);
            }
        }
        else
        {
            unitOfWork.PhotoRepository.RemovePhoto(photo);
        }
        
        if (await unitOfWork.Complete()) return Ok();
        
        return BadRequest("Failed to delete photo");
    }
}