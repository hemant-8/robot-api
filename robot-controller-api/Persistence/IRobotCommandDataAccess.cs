using robot_controller_api.Models;

namespace robot_controller_api.Persistence;

public interface IRobotCommandDataAccess
{
    List<RobotCommand> GetRobotCommands();

    void InsertRobotCommand(RobotCommand command);

    void UpdateRobotCommand(int id, RobotCommand command);

    void DeleteRobotCommand(int id);
}