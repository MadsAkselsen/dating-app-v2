using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class UnitOfWork(AppDbContext context) : IUnitOFWork
{
    private IMemberRepository? _memberRepository;
    private IMessageRepository? _messageRepository;
    private ILikesRepository? _likesRepository;
    
    public IMemberRepository MemberRepository => _memberRepository ??= new MemberRepository(context);
    public IMessageRepository MessageRepository => _messageRepository ??= new MessageRepository(context);
    public ILikesRepository LikesRepository => _likesRepository ??= new LikesRepository(context);
    
    public async Task<bool> Complete()
    {
        try
        {
            return await context.SaveChangesAsync() > 0;
        }
        catch (DbUpdateException ex)
        {
            throw new Exception("There was a problem updating the database", ex); 
        }

    }

    public bool hasChanges()
    {
        return context.ChangeTracker.HasChanges();
    }
}