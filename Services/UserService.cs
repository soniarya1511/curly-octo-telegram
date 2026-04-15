using Coursera_Submission.Models;

namespace Coursera_Submission.Services;

/// <summary>
/// In-memory implementation of IUserService.
/// Uses a static list to simulate a database — ready to swap with EF Core later.
/// Copilot was used to assist in generating the mapping logic and LINQ queries.
/// </summary>
public class UserService : IUserService
{
    // Thread-safe in-memory store (simulates a DB for this demo)
    private static readonly List<User> _users = new()
    {
        new User { Id = 1, FirstName = "Alice",   LastName = "Johnson", Email = "alice@example.com",   Role = "Admin", CreatedAt = DateTime.UtcNow },
        new User { Id = 2, FirstName = "Bob",     LastName = "Smith",   Email = "bob@example.com",     Role = "User",  CreatedAt = DateTime.UtcNow },
        new User { Id = 3, FirstName = "Charlie", LastName = "Brown",   Email = "charlie@example.com", Role = "User",  CreatedAt = DateTime.UtcNow },
    };

    private static int _nextId = 4;
    private static readonly object _lock = new();

    // ── Helpers ─────────────────────────────────────────────────────────────

    private static UserResponseDto MapToResponse(User u) => new()
    {
        Id        = u.Id,
        FirstName = u.FirstName,
        LastName  = u.LastName,
        Email     = u.Email,
        Role      = u.Role,
        IsActive  = u.IsActive,
        CreatedAt = u.CreatedAt,
        UpdatedAt = u.UpdatedAt,
    };

    // ── Read ─────────────────────────────────────────────────────────────────

    public Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
    {
        lock (_lock)
        {
            var result = _users.Select(MapToResponse);
            return Task.FromResult(result);
        }
    }

    public Task<UserResponseDto?> GetUserByIdAsync(int id)
    {
        lock (_lock)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);
            return Task.FromResult(user is null ? null : MapToResponse(user));
        }
    }

    public Task<UserResponseDto?> GetUserByEmailAsync(string email)
    {
        lock (_lock)
        {
            var user = _users.FirstOrDefault(u =>
                u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(user is null ? null : MapToResponse(user));
        }
    }

    // ── Create ───────────────────────────────────────────────────────────────

    public Task<UserResponseDto> CreateUserAsync(CreateUserDto dto)
    {
        lock (_lock)
        {
            var user = new User
            {
                Id        = _nextId++,
                FirstName = dto.FirstName.Trim(),
                LastName  = dto.LastName.Trim(),
                Email     = dto.Email.Trim().ToLowerInvariant(),
                Role      = dto.Role,
                CreatedAt = DateTime.UtcNow,
                IsActive  = true,
            };

            _users.Add(user);
            return Task.FromResult(MapToResponse(user));
        }
    }

    // ── Update ───────────────────────────────────────────────────────────────

    public Task<UserResponseDto?> UpdateUserAsync(int id, UpdateUserDto dto)
    {
        lock (_lock)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);
            if (user is null) return Task.FromResult<UserResponseDto?>(null);

            // Only apply fields that were provided (partial update)
            if (dto.FirstName is not null) user.FirstName = dto.FirstName.Trim();
            if (dto.LastName  is not null) user.LastName  = dto.LastName.Trim();
            if (dto.Email     is not null) user.Email     = dto.Email.Trim().ToLowerInvariant();
            if (dto.Role      is not null) user.Role      = dto.Role;
            if (dto.IsActive  is not null) user.IsActive  = dto.IsActive.Value;

            user.UpdatedAt = DateTime.UtcNow;

            return Task.FromResult<UserResponseDto?>(MapToResponse(user));
        }
    }

    // ── Delete ───────────────────────────────────────────────────────────────

    public Task<bool> DeleteUserAsync(int id)
    {
        lock (_lock)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);
            if (user is null) return Task.FromResult(false);
            _users.Remove(user);
            return Task.FromResult(true);
        }
    }

    // ── Existence checks ─────────────────────────────────────────────────────

    public Task<bool> UserExistsAsync(int id)
    {
        lock (_lock)
        {
            return Task.FromResult(_users.Any(u => u.Id == id));
        }
    }

    public Task<bool> EmailExistsAsync(string email, int? excludeId = null)
    {
        lock (_lock)
        {
            var exists = _users.Any(u =>
                u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)
                && (excludeId is null || u.Id != excludeId));
            return Task.FromResult(exists);
        }
    }
}
