using Shared.DTO;

namespace Service.Contracts;

public interface IUserService
{
    Task<bool> CreateUser(CreateUserDTO request);
    Task<IEnumerable<GetUsersListDTO>> GetUsers();
    Task<IEnumerable<GetUsersListDTO>> GetUsersByPattern(string searchPattern);
    Task<GetUserDetailsDTO> GetUserDetails(int userId);
    Task<bool> DeleteUser(int userId);
    Task<bool> UpdateUser(int userId, UpdateUserDTO request);
}
