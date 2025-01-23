using System.ComponentModel.DataAnnotations;

namespace NSE.Identity.API.Models;

public class RegisterUser
{
    [Required(ErrorMessage = "The field {0} is required")]
    [EmailAddress(ErrorMessage = "The field {0} is in an invalid format")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "The field {0} is required")]
    [StringLength(100, ErrorMessage = "The field {0} must be between {2} and {1} characters", MinimumLength = 6)]
    public required string Password { get; set; }

    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public required string ConfirmPassword { get; set; }
}


public class LoginUser
{
    [Required(ErrorMessage = "The field {0} is required")]
    [EmailAddress(ErrorMessage = "The field {0} is in an invalid format")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "The field {0} is required")]
    [StringLength(100, ErrorMessage = "The field {0} must be between {2} and {1} characters", MinimumLength = 6)]
    public required string Password { get; set; }
}

public class LoginUserResponse
{
    public required string AccessToken { get; set; }
    public required double ExpiresIn { get; set; }
    public required UserToken UserToken { get; set; }
}

public class UserToken
{
    public required string Id { get; set; }
    public required string Email { get; set; }
    public IEnumerable<UserClaim> Claims { get; set; } = [];
}

public class UserClaim
{
    public required string Value { get; set; }
    public required string Type { get; set; }
}