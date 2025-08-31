using HairAI.Application.Features.Auth.Commands.Register;
using HairAI.Application.Features.Auth.Queries.Login;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using HairAI.Application.Common.Interfaces;
using Microsoft.AspNetCore.Identity;
using HairAI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HairAI.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[EnableRateLimiting("AuthPolicy")] // Enable rate limiting for security
public class AuthController : BaseController
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IApplicationDbContext _context;

    public AuthController(UserManager<ApplicationUser> userManager, IApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }
    
    /// <summary>
    /// Register a new user account
    /// </summary>
    /// <param name="command">User registration details</param>
    /// <returns>Registration result with user information</returns>
    [HttpPost("register")]
    [EnableRateLimiting("AuthPolicy")] // Add rate limiting for security
    public async Task<ActionResult<RegisterCommandResponse>> Register(RegisterCommand command)
    {
        var response = await Mediator.Send(command);
        return Ok(response);
    }

    /// <summary>
    /// Login and obtain JWT token
    /// </summary>
    /// <param name="query">Login credentials</param>
    /// <returns>JWT token and user information</returns>
    [HttpPost("login")]
    [EnableRateLimiting("AuthPolicy")] // Add rate limiting for security
    public async Task<ActionResult<LoginQueryResponse>> Login(LoginQuery query)
    {
        var response = await Mediator.Send(query);
        return Ok(response);
    }

    /// <summary>
    /// Bootstrap first SuperAdmin user (Development only)
    /// </summary>
    /// <returns>Admin creation result</returns>
    [HttpPost("bootstrap-admin")]
    [EnableRateLimiting("AuthPolicy")] // Add rate limiting for security
    public async Task<IActionResult> BootstrapAdmin()
    {
        try
        {
            // Remove any existing admin@hairai.com user to create fresh one
            var existingUser = await _userManager.FindByEmailAsync("admin@hairai.com");
            if (existingUser != null)
            {
                await _userManager.DeleteAsync(existingUser);
            }

            // Create SuperAdmin user
            var adminUser = new ApplicationUser
            {
                Email = "admin@hairai.com",
                UserName = "admin@hairai.com",
                FirstName = "Super",
                LastName = "Admin",
                EmailConfirmed = true,
                ClinicId = null // SuperAdmin not tied to specific clinic
            };

            var result = await _userManager.CreateAsync(adminUser, "SuperAdmin123!");
            if (!result.Succeeded)
            {
                return BadRequest(new { 
                    success = false, 
                    message = "Failed to create SuperAdmin user",
                    errors = result.Errors.Select(e => e.Description) 
                });
            }

            // Create SuperAdmin role if it doesn't exist
            var roleManager = HttpContext.RequestServices.GetRequiredService<RoleManager<IdentityRole>>();
            if (!await roleManager.RoleExistsAsync("SuperAdmin"))
            {
                await roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
            }
            if (!await roleManager.RoleExistsAsync("ClinicAdmin"))
            {
                await roleManager.CreateAsync(new IdentityRole("ClinicAdmin"));
            }
            if (!await roleManager.RoleExistsAsync("Doctor"))
            {
                await roleManager.CreateAsync(new IdentityRole("Doctor"));
            }

            // Assign SuperAdmin role
            await _userManager.AddToRoleAsync(adminUser, "SuperAdmin");

            return Ok(new { 
                success = true, 
                message = "SuperAdmin created successfully",
                data = new {
                    email = adminUser.Email,
                    userId = adminUser.Id,
                    defaultPassword = "SuperAdmin123!"
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { 
                success = false, 
                message = "Internal server error during bootstrap",
                errors = new[] { ex.Message }
            });
        }
    }
}