namespace StreamDbHandler.Services;

public interface IUserServiceClient
{
    Task<UserDTO?> GetUserByIdAsync(Guid userId);
    Task UpdateUserAsync(Guid userId, UserDTO user);
}
