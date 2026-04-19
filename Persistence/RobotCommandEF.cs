using robot_controller_api.Models;

namespace robot_controller_api.Persistence;

public class RobotCommandEF : IRobotCommandDataAccess
{
    private readonly RobotContext _context;

    public RobotCommandEF(RobotContext context)
    {
        _context = context;
    }

    public List<RobotCommand> GetRobotCommands()
    {
        return _context.RobotCommands.ToList();
    }

    public void InsertRobotCommand(RobotCommand command)
    {
        _context.RobotCommands.Add(command);
        _context.SaveChanges();
    }

    public void UpdateRobotCommand(int id, RobotCommand command)
    {
        var existing = _context.RobotCommands.Find(id);

        if (existing == null) return;

        existing.Name = command.Name;
        existing.Description = command.Description;
        existing.IsMoveCommand = command.IsMoveCommand;
        existing.ModifiedDate = DateTime.Now;

        _context.SaveChanges();
    }

    public void DeleteRobotCommand(int id)
    {
        var command = _context.RobotCommands.Find(id);

        if (command == null) return;

        _context.RobotCommands.Remove(command);
        _context.SaveChanges();
    }
}