using Coursera_Submission.Models;
using Coursera_Submission.Services;
using Microsoft.AspNetCore.Mvc;

namespace Coursera_Submission.Controllers;

/// <summary>
/// UsersController — provides full CRUD operations for managing users.
/// Copilot was used to scaffold the controller structure, suggest route conventions,
/// populate validation responses, and craft XML doc comments.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger      = logger;
    }

    // ── GET /api/users ────────────────────────────────────────────────────────

    /// <summary>Retrieves all users in the system.</summary>
    /// <returns>A list of all users.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllUsers()
    {
        _logger.LogInformation("Fetching all users.");
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    // ── GET /api/users/{id} ───────────────────────────────────────────────────

    /// <summary>Retrieves a single user by their ID.</summary>
    /// <param name="id">The unique user ID.</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserById(int id)
    {
        _logger.LogInformation("Fetching user with ID {Id}.", id);
        var user = await _userService.GetUserByIdAsync(id);

        if (user is null)
        {
            _logger.LogWarning("User with ID {Id} not found.", id);
            return NotFound(new { message = $"User with ID {id} was not found." });
        }

        return Ok(user);
    }

    // ── POST /api/users ───────────────────────────────────────────────────────

    /// <summary>Creates a new user after validating the request body.</summary>
    /// <param name="dto">The user data to create.</param>
    [HttpPost]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
    {
        // ModelState is validated automatically via [ApiController] — returns 400 on failure

        // Additional business-rule validation: email uniqueness
        if (await _userService.EmailExistsAsync(dto.Email))
        {
            _logger.LogWarning("Attempted to create user with duplicate email: {Email}", dto.Email);
            return Conflict(new { message = $"A user with email '{dto.Email}' already exists." });
        }

        var created = await _userService.CreateUserAsync(dto);
        _logger.LogInformation("Created new user with ID {Id}.", created.Id);

        return CreatedAtAction(nameof(GetUserById), new { id = created.Id }, created);
    }

    // ── PUT /api/users/{id} ───────────────────────────────────────────────────

    /// <summary>Updates an existing user. Only fields that are provided are updated.</summary>
    /// <param name="id">The ID of the user to update.</param>
    /// <param name="dto">Fields to update.</param>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto dto)
    {
        // Check if the user exists first
        if (!await _userService.UserExistsAsync(id))
        {
            _logger.LogWarning("Update failed — user with ID {Id} not found.", id);
            return NotFound(new { message = $"User with ID {id} was not found." });
        }

        // Validate email uniqueness if a new email is being set
        if (dto.Email is not null && await _userService.EmailExistsAsync(dto.Email, excludeId: id))
        {
            _logger.LogWarning("Update failed — email {Email} is already taken.", dto.Email);
            return Conflict(new { message = $"A user with email '{dto.Email}' already exists." });
        }

        var updated = await _userService.UpdateUserAsync(id, dto);
        _logger.LogInformation("Updated user with ID {Id}.", id);

        return Ok(updated);
    }

    // ── DELETE /api/users/{id} ────────────────────────────────────────────────

    /// <summary>Deletes a user by their ID.</summary>
    /// <param name="id">The ID of the user to delete.</param>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var deleted = await _userService.DeleteUserAsync(id);

        if (!deleted)
        {
            _logger.LogWarning("Delete failed — user with ID {Id} not found.", id);
            return NotFound(new { message = $"User with ID {id} was not found." });
        }

        _logger.LogInformation("Deleted user with ID {Id}.", id);
        return NoContent();
    }
}
