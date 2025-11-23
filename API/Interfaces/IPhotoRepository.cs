using API.DTOs;
using API.Entities;

namespace API.Interfaces;

public interface IPhotoRepository
{
    Task<IReadOnlyList<PhotoForApprovalDto>> GetUnapprovedPhotos();
    Task<Photo?> GetPhotoByIdAsync(int photoId);
    void RemovePhoto(Photo photo);
}