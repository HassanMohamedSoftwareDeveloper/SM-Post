using CQRS.Core.Commands;

namespace Post.CMD.Api.Commands
{
    public class DeletePostCommand : BaseCommand
    {
        public string Username { get; set; }
    }
}
