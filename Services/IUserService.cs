using Coursera_Submission.Models;

namespace Coursera_Submission.Services;

/// <summary>
/// Defines the contract for user management operations.
/// Copilot was used to suggest this interface structure based on CRUD requirements.
/// </summary>
public interface IUserService
{
    Task<IEnumerable<UserResponseDto>> GetAllUsersAsync();
    Task<UserResponseDto?> GetUserByIdAsync(int id);
    Task<UserResponseDto?> GetUserByEmailAsync(string email);
    Task<UserResponseDto> CreateUserAsync(CreateUserDto dto);
    Task<UserResponseDto?> UpdateUserAsync(int id, UpdateUserDto dto);
    Task<bool> DeleteUserAsync(int id);
    Task<bool> UserExistsAsync(int id);
    Task<bool> EmailExistsAsync(string email, int? excludeId = null);
}
