using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class PhotoRepository(AppDbContext context) : IPhotoRepository
{
    public async Task<IReadOnlyList<PhotoForApprovalDto>> GetUnapprovedPhotos()
    {
        var query = context.Photos
            .IgnoreQueryFilters()
            .Where(p => p.IsApproved == false)
            .Select(p => new PhotoForApprovalDto
            {
                Id = p.Id,
                Url = p.Url,
                UserId = p.MemberId,
                IsApproved = p.IsApproved
            });
        
        return await query.ToListAsync();
    }

    public async Task<Photo?> GetPhotoByIdAsync(int photoId)
    {
        return await context.Photos
            .IgnoreQueryFilters()
            .SingleOrDefaultAsync(x => x.Id == photoId);
    }

    public void RemovePhoto(Photo photo)
    {
        context.Photos.Remove(photo);
    }
}