using System.Linq.Expressions;
using Shared.Models.Domain;
using Shared.src.Models.User;

namespace StreamDbHandler.Services;

public interface IUserDbHandlerClient
{
    Task<UserDTO?> GetUserByIdAsync(Guid id);
    Task<UserDTO> GetUserByUsernameAsync(string username);
    Task<UserDTO> UpdateUserAsync(Guid userId,UserDTO user);
    Task<UserWithFollowersDTO> GetUserByIdWithFollowersAsync(Guid id);
    Task<UserDTO> CreateUserAsync(UserDTO user);
    Task<UserDTO?> GetUserByEmailAsync(string email);
}
