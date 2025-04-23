using System;

namespace Shared.src.Models.User;

public class FollowRequest
{
    public Guid FollowerId { get; set; }
    public Guid FollowingId { get; set; }
}
