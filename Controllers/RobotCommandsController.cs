using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using robot_controller_api.Persistence;
using robot_controller_api.Models;

namespace robot_controller_api.Controllers;

[ApiController]
[Route("api/robot-commands")]
[Authorize]
public class RobotCommandsController : ControllerBase
{
    private readonly IRobotCommandDataAccess _robotCommandsRepo;

    public RobotCommandsController(IRobotCommandDataAccess robotCommandsRepo)
    {
        _robotCommandsRepo = robotCommandsRepo;
    }

    [HttpGet]
    [Authorize(Policy = "UserOnly")]
    public IEnumerable<RobotCommand> GetAllRobotCommands()
    {
        return _robotCommandsRepo.GetRobotCommands();
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "UserOnly")]
    public IActionResult GetRobotCommandById(int id)
    {
        var command = _robotCommandsRepo.GetRobotCommands()
            .FirstOrDefault(c => c.Id == id);

        if (command == null)
            return NotFound();

        return Ok(command);
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult AddRobotCommand(RobotCommand newCommand)
    {
        if (newCommand == null)
            return BadRequest();

        _robotCommandsRepo.InsertRobotCommand(newCommand);

        return Ok(newCommand);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult UpdateRobotCommand(int id, RobotCommand updatedCommand)
    {
        if (updatedCommand == null)
            return BadRequest();

        var existing = _robotCommandsRepo.GetRobotCommands()
            .FirstOrDefault(c => c.Id == id);

        if (existing == null)
            return NotFound();

        _robotCommandsRepo.UpdateRobotCommand(id, updatedCommand);

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult DeleteRobotCommand(int id)
    {
        var existing = _robotCommandsRepo.GetRobotCommands()
            .FirstOrDefault(c => c.Id == id);

        if (existing == null)
            return NotFound();

        _robotCommandsRepo.DeleteRobotCommand(id);

        return NoContent();
    }
}