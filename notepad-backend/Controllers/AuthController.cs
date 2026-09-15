using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using notepad_backend.Dto;
using notepad_backend.Entities;
using notepad_backend.Func;
using notepad_backend.Repsitory.Interface;

namespace notepad_backend.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController(IAuthInterface authRepo, ISessionInterface sessionRepo, JwtFunc jwtFunc) : ControllerBase
{
    [HttpPost("login/user-password")]
    public async Task<ActionResult<TokenResponcesDto>> LoginWithUserPassword([FromBody] LoginRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest("Username and password are required.");
        }

        var user = await authRepo.GetUserByUsername(request.Username);
        if (user is null || user.PasswordHash is null || user.PasswordSalt is null)
        {
            return Unauthorized("Invalid username or password.");
        }

        if (!user.IsActive)
        {
            return Unauthorized("This account is deactivated.");
        }

        if (!HashFunc.VerifyPassword(request.Password, user.PasswordSalt, user.PasswordHash))
        {
            return Unauthorized("Invalid username or password.");
        }

        var tokens = await IssueTokensAsync(user);
        return Ok(tokens);
    }

    [HttpPost("signin/user-password")]
    public async Task<ActionResult<TokenResponcesDto>> SigninWithUserPassword([FromBody] RegisterRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest("Username, email and password are required.");
        }

        var salt = HashFunc.GenerateSalt();
        var hash = HashFunc.HashPassword(request.Password, salt);

        var newUser = new AppUserEntity
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = hash,
            PasswordSalt = salt,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            IsActive = true,
        };

        AppUserEntity? createdUser;
        try
        {
            createdUser = await authRepo.CreateUser(newUser);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }

        if (createdUser is null)
        {
            return Problem("Failed to create user.");
        }

        var tokens = await IssueTokensAsync(createdUser);
        return Ok(tokens);
    }

    [HttpPost("logout")]
    public async Task<ActionResult> LogoutUser([FromBody] RefreshTokenRequestDto request)
    {
        var session = await sessionRepo.GetSessionByRefreshToken(request.RefreshToken);
        if (session is not null)
        {
            await sessionRepo.RevokeSessionById(session.Id);
        }

        return NoContent();
    }

    [HttpPost("refrash")]
    public async Task<ActionResult<TokenResponcesDto>> RefrashToken([FromBody] RefreshTokenRequestDto request)
    {
        var session = await sessionRepo.GetSessionByRefreshToken(request.RefreshToken);
        if (session is null)
        {
            return Unauthorized("Invalid or expired refresh token.");
        }

        var user = await authRepo.GetuserbyId(session.UserId);
        if (user is null || !user.IsActive)
        {
            return Unauthorized("Invalid or expired refresh token.");
        }

        await sessionRepo.RevokeSessionById(session.Id);

        var tokens = await IssueTokensAsync(user);
        return Ok(tokens);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<AppUserEntity>> Userinfo()
    {
        var userIdClaim = User.FindFirst("user_id")?.Value;
        if (userIdClaim is null || !int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }
        AppUserEntity? user;
        try
        {
            user = await authRepo.GetuserbyId(userId);
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }
        if (user is null)
        {
            return NotFound();
        }
        user.PasswordHash = null;
        user.PasswordSalt = null;

        return Ok(user);
    }

    private async Task<TokenResponcesDto> IssueTokensAsync(AppUserEntity user)
    {
        var accessToken = jwtFunc.GenerateToken(isRefrash: false, user);
        var refreshToken = jwtFunc.GenerateToken(isRefrash: true, user);
        var expiresAt = DateTime.UtcNow.AddMinutes(30);
        var refreshTokenExpiry = DateTime.UtcNow.AddHours(1);

        await sessionRepo.InsertSession(new SessionEntity
        {
            UserId = user.Id,
            SessionToken = accessToken,
            RefreshToken = refreshToken,
            RefreshTokenExpiry = refreshTokenExpiry,
        });

        return new TokenResponcesDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            TokenType = "Bearer",
            ExpiresIn = 1800,
            ExpiresAt = expiresAt,
        };
    }
}
