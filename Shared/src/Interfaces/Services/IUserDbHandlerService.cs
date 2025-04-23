using System.Linq.Expressions;
using Shared.Models.Domain;
using Shared.src.Models.User;

namespace Shared.Interfaces;

public interface IUserDbHandlerService
{
    Task<UserDTO> GetUserByIdAsync(Guid id);
    Task<UserDTO> GetUserByUsernameAsync(string username);
    Task<UserDTO> UpdateUserAsync(UserDTO user);
    Task<UserWithFollowersDTO> GetUserByIdWithIncludesAsync(Guid id);
    Task<UserDTO> CreateUserAsync(UserDTO user);
    Task<UserDTO?> GetUserByEmailAsync(string email);

}
