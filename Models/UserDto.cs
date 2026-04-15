using System.ComponentModel.DataAnnotations;

namespace Coursera_Submission.Models;

/// <summary>
/// DTO used when creating a new user (POST).
/// Contains validation attributes to ensure only valid data is processed.
/// </summary>
public class CreateUserDto
{
    [Required(ErrorMessage = "First name is required.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 50 characters.")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "A valid email address is required.")]
    [StringLength(100, ErrorMessage = "Email must not exceed 100 characters.")]
    public string Email { get; set; } = string.Empty;

    [RegularExpression("^(Admin|User)$", ErrorMessage = "Role must be either 'Admin' or 'User'.")]
    public string Role { get; set; } = "User";
}

/// <summary>
/// DTO used when updating an existing user (PUT).
/// All fields are optional — only provided fields are updated.
/// </summary>
public class UpdateUserDto
{
    [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters.")]
    public string? FirstName { get; set; }

    [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 50 characters.")]
    public string? LastName { get; set; }

    [EmailAddress(ErrorMessage = "A valid email address is required.")]
    [StringLength(100, ErrorMessage = "Email must not exceed 100 characters.")]
    public string? Email { get; set; }

    [RegularExpression("^(Admin|User)$", ErrorMessage = "Role must be either 'Admin' or 'User'.")]
    public string? Role { get; set; }

    public bool? IsActive { get; set; }
}

/// <summary>
/// DTO returned to the caller — never exposes internal fields.
/// </summary>
public class UserResponseDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
