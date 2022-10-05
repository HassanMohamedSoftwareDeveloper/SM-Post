using CQRS.Core.Commands;

namespace Post.CMD.Api.Commands;

public class RemoveCommentCommand : BaseCommand
{
    public Guid CommentId { get; set; }
    public string Username { get; set; }
}
