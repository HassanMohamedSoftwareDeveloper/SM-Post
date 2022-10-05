using CQRS.Core.Commands;

namespace Post.CMD.Api.Commands;

public class AddCommentCommand : BaseCommand
{
    public string Comment { get; set; }
    public string Username { get; set; }
}
