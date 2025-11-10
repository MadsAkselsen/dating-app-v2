namespace API.Extensions;

public class CreateMessageDto
{
    public required string RecipientId {set; get;}
    public required string Content  {set; get;}
}