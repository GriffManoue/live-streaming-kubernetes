using System;
using System.Threading.Tasks;
using Shared.Models.User;

namespace StreamService.Services;

public interface IUserServiceClient
{
    Task<UserDto?> GetUserByIdAsync(Guid userId);
}
