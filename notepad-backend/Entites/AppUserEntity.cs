namespace notepad_backend.Entities;


public class AppUserEntity
{
    public int Id { get; set; }
    public string Username { get; set; }

    public string Email { get; set; }

    public string? PasswordHash { get; set; }

    public string? PasswordSalt { get; set; }

    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public bool IsGoogleSignIn { get; set; }
    public bool IsEmailVerified { get; set; }
    public bool IsActive { get; set; }
    public int FailedAttempts { get; set; }

    public DateTime LastLoginAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime DeletedAt { get; set; }
}