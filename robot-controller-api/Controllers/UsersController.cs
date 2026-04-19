using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using robot_controller_api.Models;
using robot_controller_api.Persistence;

namespace robot_controller_api.Controllers;

[ApiController]
[Route("users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly UserDataAccess _userData;

    public UsersController(UserDataAccess userData)
    {
        _userData = userData;
    }

    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult GetAll()
    {
        return Ok(_userData.GetAll());
    }

    [HttpGet("admin")]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult GetAdmins()
    {
        var users = _userData.GetAll()
            .Where(u => u.Role == "Admin");

        return Ok(users);
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "UserOnly")]
    public IActionResult GetById(int id)
    {
        var user = _userData.GetAll()
            .FirstOrDefault(u => u.Id == id);

        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [HttpPost]
    [AllowAnonymous]
    public IActionResult Register(UserModel user)
    {
        var hasher = new PasswordHasher<UserModel>();

        user.PasswordHash = hasher.HashPassword(user, user.PasswordHash);
        user.CreatedDate = DateTime.Now;
        user.ModifiedDate = DateTime.Now;

        if (string.IsNullOrEmpty(user.Role))
            user.Role = "User";

        _userData.Add(user);

        return Ok(user);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult Update(int id, UserModel updatedUser)
    {
        var existing = _userData.GetAll()
            .FirstOrDefault(u => u.Id == id);

        if (existing == null)
            return NotFound();

        existing.FirstName = updatedUser.FirstName;
        existing.LastName = updatedUser.LastName;
        existing.Description = updatedUser.Description;
        existing.Role = updatedUser.Role;
        existing.ModifiedDate = DateTime.Now;

        _userData.Update(existing);

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult Delete(int id)
    {
        var existing = _userData.GetAll()
            .FirstOrDefault(u => u.Id == id);

        if (existing == null)
            return NotFound();

        _userData.Delete(id);

        return NoContent();
    }

    [HttpPatch("{id}")]
    [Authorize(Policy = "UserOnly")]
    public IActionResult UpdateCredentials(int id, LoginModel login)
    {
        var user = _userData.GetAll()
            .FirstOrDefault(u => u.Id == id);

        if (user == null)
            return NotFound();

        var hasher = new PasswordHasher<UserModel>();

        user.Email = login.Email;
        user.PasswordHash = hasher.HashPassword(user, login.Password);
        user.ModifiedDate = DateTime.Now;

        _userData.Update(user);

        return NoContent();
    }
}