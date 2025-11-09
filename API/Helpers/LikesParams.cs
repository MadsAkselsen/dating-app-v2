namespace API.Helpers;

public class LikesParams : PagingParams
{
    // public string? Gender { get; set; }
    public required string CurrentMemberId { get; set; } = "";
    // TODO: Do we need these props?
    // public int MinAge { get; set; } = 18;
    // public int MaxAge { get; set; } = 100;
    // public string OrderBy { get; set; } = "lastActive";
    public string Predicate { get; set; } = "likes";
}