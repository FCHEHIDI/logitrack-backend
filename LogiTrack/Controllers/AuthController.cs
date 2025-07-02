using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using LogiTrack.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;


[ApiController]
[Route("api/[controller]")]

public class AuthController : ControllerBase
{
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly SignInManager<ApplicationUser> _signInManager;
	private readonly IConfiguration _configuration;

	public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration)
	{
		_userManager = userManager;
		_signInManager = signInManager;
		_configuration = configuration;
	}

// POST: /api/auth/register – registers a user
[HttpPost("register")]
public async Task<IActionResult> Register([FromBody] RegisterModel model)
{
	// Input validation
	if (string.IsNullOrWhiteSpace(model.UserName) || string.IsNullOrWhiteSpace(model.Password) || string.IsNullOrWhiteSpace(model.Email))
	{
		return BadRequest("Username, email, and password are required.");
	}
	if (!System.Text.RegularExpressions.Regex.IsMatch(model.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
	{
		return BadRequest("Invalid email format.");
	}
	if (model.Password.Length < 8 || !model.Password.Any(char.IsDigit) || !model.Password.Any(char.IsUpper))
	{
		return BadRequest("Password must be at least 8 characters, contain a digit and an uppercase letter.");
	}

	var user = new ApplicationUser { UserName = model.UserName, Email = model.Email };
	var result = await _userManager.CreateAsync(user, model.Password);
	if (result.Succeeded)
	{
		// Assign default role
		await _userManager.AddToRoleAsync(user, "User");
		// Generate email confirmation token and (for demo) return it in response
		var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
		// In production, send this link via email
		var confirmationLink = Url.Action("ConfirmEmail", "Auth", new { userId = user.Id, token = token }, Request.Scheme);
		return Ok(new { message = "Registration successful. Please confirm your email.", confirmationLink });
	}
	return BadRequest(result.Errors);
}
// GET: /api/auth/confirmemail?userId=...&token=...
[HttpGet("confirmemail")]
public async Task<IActionResult> ConfirmEmail(string userId, string token)
{
	var user = await _userManager.FindByIdAsync(userId);
	if (user == null)
		return BadRequest("Invalid user.");
	var result = await _userManager.ConfirmEmailAsync(user, token);
	if (result.Succeeded)
		return Ok("Email confirmed. You can now log in.");
	return BadRequest("Email confirmation failed.");
}

// POST: /api/auth/login – logs in and returns a JWT
[HttpPost("login")]
public async Task<IActionResult> Login([FromBody] LoginModel model)
{
	if (string.IsNullOrWhiteSpace(model.UserName) || string.IsNullOrWhiteSpace(model.Password))
	{
		return BadRequest("Username and password are required.");
	}
	var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);
	if (result.Succeeded)
	{
		var user = await _userManager.FindByNameAsync(model.UserName);
		if (user == null)
		{
			return Unauthorized();
		}

		// Get roles for the user
		var roles = await _userManager.GetRolesAsync(user);

		// Build claims
		var claims = new List<Claim>
		{
			new Claim(JwtRegisteredClaimNames.Sub, user.UserName ?? string.Empty),
			new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
			new Claim(ClaimTypes.NameIdentifier, user.Id)
		};
		foreach (var role in roles)
		{
			claims.Add(new Claim(ClaimTypes.Role, role));
		}

		// Read JWT settings from configuration
		var jwtKey = _configuration["Jwt:Key"];
		var jwtIssuer = _configuration["Jwt:Issuer"];
		var jwtAudience = _configuration["Jwt:Audience"] ?? jwtIssuer;
		if (string.IsNullOrWhiteSpace(jwtKey) || string.IsNullOrWhiteSpace(jwtIssuer))
		{
			return StatusCode(500, "JWT configuration is missing.");
		}
		var expires = DateTime.UtcNow.AddHours(2);

		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
		var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

		var token = new JwtSecurityToken(
			issuer: jwtIssuer,
			audience: jwtAudience,
			claims: claims,
			expires: expires,
			signingCredentials: creds
		);

		var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

		return Ok(new { token = tokenString });
	}
	return Unauthorized();
}
}

// Models for registration and login
// You can fine-grain the authorization by adding multiple roles and claims to users and using [Authorize(Roles = "Role1,Role2")] or [Authorize(Policy = "CustomPolicy")] on your endpoints.
public class RegisterModel
{
	public string? UserName { get; set; }
	public string? Email { get; set; }
	public string? Password { get; set; }
}

public class LoginModel
{
	public string? UserName { get; set; }
	public string? Password { get; set; }
}
