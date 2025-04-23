using System;

namespace Shared.src.Models.User;

public class UserWithFollowersDTO
{
    public UserDTO User { get; set; } = null!;
    public IEnumerable<UserDTO> Followers { get; set; } = null!;
    public IEnumerable<UserDTO> Following { get; set; } = null!;
    public UserWithFollowersDTO(UserDTO user, IEnumerable<UserDTO> followers, IEnumerable<UserDTO> following)
    {
        User = user;
        Followers = followers;
        Following = following;
    }
}
