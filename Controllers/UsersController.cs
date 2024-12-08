using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PfeProject.Dtos;
using PfeProject.Models;
using PfeProject.Utils;

namespace PfeProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<User> _userManager;

        public UsersController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        // Login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (loginDto == null)
                return BadRequest("Invalid client request");
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
                return NotFound();
            var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);

            if (!result)
                return Unauthorized("Invalid username or password.");


            var roles = await _userManager.GetRolesAsync(user);

            var response = new LoginResponseDto
            {
                UserId = user.Id,
                UserName = user.UserName,
                Role = roles.FirstOrDefault()
            };

            return Ok(response);
        }

        // Create User
        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto userDto)
        {
            var user = new User
            {
                UserName = userDto.Username,
                Email = userDto.Email,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName
            };

            var result = await _userManager.CreateAsync(user, userDto.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, userDto.Role.ToUpper());
                return Ok(new { Message = "User created successfully", UserId = user.Id });
            }


            return BadRequest(result.Errors);
        }

        // Edit User
        [HttpPut("edit/{id}")]
        public async Task<IActionResult> EditUser(string id, [FromBody] EditUserDto userDto)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound("User not found");

            // Update user details
            user.FirstName = userDto.FirstName;
            user.LastName = userDto.LastName;
            user.Email = userDto.Email;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // Update user role if specified
            if (!string.IsNullOrEmpty(userDto.Role.ToUpper()))
            {
                var currentRoles = await _userManager.GetRolesAsync(user);
                var roleUpdateResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);

                if (!roleUpdateResult.Succeeded)
                    return BadRequest(roleUpdateResult.Errors);

                var addRoleResult = await _userManager.AddToRoleAsync(user, userDto.Role.ToUpper());
                if (!addRoleResult.Succeeded)
                    return BadRequest(addRoleResult.Errors);
            }

            return Ok("User updated successfully");
        }


        // Delete User
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound("User not found");

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
                return Ok("User deleted successfully");

            return BadRequest(result.Errors);
        }

        // Get User By Id
        [HttpGet("getbyid/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
                return NotFound("User not found");

            var roles = await _userManager.GetRolesAsync(user);
            return Ok(new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = roles.ToList()
            });
        }

        // Get All Users
        [HttpGet("send-email")]
        public async Task<IActionResult> SendEmail()
        {
            EmailService emailService = new EmailService();
            await emailService.SendEmailAsync("alaa.boukchina@gmail.com","test","test");
            return Ok(true);
        }
        // Get All Users
        [HttpGet("getall")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userManager.Users.ToListAsync();

            var userDtos = new List<UserDto>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userDtos.Add(new UserDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Roles = roles.ToList()
                });
            }

            return Ok(userDtos);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> Filter([FromQuery] string? username, [FromQuery] string? email, [FromQuery] string? role)
        {
            var query = _userManager.Users.AsQueryable();
            if (!string.IsNullOrEmpty(username))
                query = query.Where(u => u.UserName.Contains(username));
            if (!string.IsNullOrEmpty(email))
                query = query.Where(u => u.Email.Contains(email));
            if (!string.IsNullOrEmpty(role))
            {
             
                var roleUsers = await _userManager.GetUsersInRoleAsync(role);
                var roleUserIds = roleUsers.Select(u => u.Id).ToList();
                query = query.Where(u => roleUserIds.Contains(u.Id));
            }

            // Fetch the filtered users
            var filteredUsers = await query.ToListAsync();

            // Map the filtered users to include roles
            var userDtos = new List<UserDto>();
            foreach (var user in filteredUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userDtos.Add(new UserDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Roles = [.. roles]
                });
            }

            return Ok(userDtos);
        }

    }
}
