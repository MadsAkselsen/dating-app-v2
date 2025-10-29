namespace API.Entities;

// The Id for the table is created in AppDbContext.cs. It is a combination of 
// SourceMemberId and TargetMemberId
public class MemberLike
{
    public required string SourceMemberId { get; set; }
    public Member SourceMember { get; set; } = null!;
    
    public required string TargetMemberId { get; set; }
    public Member TargetMember { get; set; } = null!;
}