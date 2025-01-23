using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using NSE.Identity.API.Extensions;
using NSE.Identity.API.Models;

namespace NSE.Identity.API.Controllers;

[Route("api/identity")]
public class AuthController : MainController
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly JwtSettings _jwtSettings;

    public AuthController(
        SignInManager<IdentityUser> signInManager,
        UserManager<IdentityUser> userManager,
        IOptions<JwtSettings> jwtSettigns)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _jwtSettings = jwtSettigns.Value;
    }

    [HttpPost("new-account")]
    public async Task<ActionResult> Register(RegisterUser registerUser)
    {
        if (!ModelState.IsValid) return CustomResponse(ModelState);

        var user = new IdentityUser
        {
            UserName = registerUser.Email,
            Email = registerUser.Email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, registerUser.Password);

        if (result.Succeeded)
        {
            return CustomResponse(await GenerateJwt(registerUser.Email));
        }

        foreach (var error in result.Errors)
        {
            AddProcessingError(error.Description);
        }

        return CustomResponse();
    }

    [HttpPost("authenticate")]
    [ProducesResponseType<LoginUserResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Login(LoginUser loginUser)
    {
        if (!ModelState.IsValid) return CustomResponse(ModelState);

        var result = await _signInManager.PasswordSignInAsync(
            loginUser.Email,
            loginUser.Password,
            isPersistent: false,
            lockoutOnFailure: true);

        if (result.Succeeded)
        {
            return Ok(await GenerateJwt(loginUser.Email));
        }

        if (result.IsLockedOut)
        {
            AddProcessingError("User temporarily blocked after multiple failed attempts");
            return CustomResponse();
        }

        AddProcessingError("Invalid user or password");
        return CustomResponse();
    }

    private async Task<LoginUserResponse> GenerateJwt(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        var claims = await _userManager.GetClaimsAsync(user);

        var identityClaims = await GetUserClaims(claims, user);
        var encodedToken = EncodeToken(identityClaims);

        return GetTokenResponse(encodedToken, user, claims);
    }

    private async Task<ClaimsIdentity> GetUserClaims(ICollection<Claim> claims, IdentityUser user)
    {
        var userRoles = await _userManager.GetRolesAsync(user);

        claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id));
        claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email!));
        claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
        claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, ToUnixEpochDate(DateTime.UtcNow).ToString()));
        claims.Add(new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64));

        foreach (var role in userRoles)
            claims.Add(new Claim("role", role));

        var identityClaims = new ClaimsIdentity();
        identityClaims.AddClaims(claims);

        return identityClaims;
    }

    private string EncodeToken(ClaimsIdentity claims)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

        var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
        {
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            Subject = claims,
            Expires = DateTime.UtcNow.AddHours(_jwtSettings.ExpirationHours),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        });

        return tokenHandler.WriteToken(token);
    }

    private LoginUserResponse GetTokenResponse(string encodedToken, IdentityUser user, IEnumerable<Claim> claims)
    {
        return new LoginUserResponse
        {
            AccessToken = encodedToken,
            ExpiresIn = TimeSpan.FromHours(_jwtSettings.ExpirationHours).TotalSeconds,
            UserToken = new UserToken
            {
                Id = user.Id,
                Email = user.Email,
                Claims = claims.Select(c => new UserClaim { Type = c.Type, Value = c.Value })
            }
        };
    }

    private static long ToUnixEpochDate(DateTime date)
        => new DateTimeOffset(date).ToUniversalTime().ToUnixTimeSeconds();
}
