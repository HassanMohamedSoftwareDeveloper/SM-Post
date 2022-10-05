using Post.Common.DTOs;

namespace Post.CMD.Api.DTOs;

public class NewPostResponse : BaseResponse
{
    public Guid Id { get; set; }
}
