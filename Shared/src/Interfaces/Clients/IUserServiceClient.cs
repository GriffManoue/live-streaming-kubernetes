using System;
using System.Threading.Tasks;

namespace StreamService.Services;

public interface IUserServiceClient
{
    Task<UserDTO?> GetUserByIdAsync(Guid userId);
    Task UpdateUserAsync(Guid userId, UserDTO user);
}
