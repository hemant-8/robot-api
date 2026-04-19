using Npgsql;
using robot_controller_api.Models;

namespace robot_controller_api.Persistence;

public class RobotCommandRepository : IRobotCommandDataAccess, IRepository
{
    private IRepository _repo => this;

    public List<RobotCommand> GetRobotCommands()
    {
        return _repo.ExecuteReader<RobotCommand>(
            "SELECT * FROM robotcommand"
        ).ToList();
    }

    public void InsertRobotCommand(RobotCommand command)
    {
        var sqlParams = new NpgsqlParameter[]
        {
            new("@name", command.Name),
            new("@description", command.Description ?? (object)DBNull.Value),
            new("@ismovecommand", command.IsMoveCommand),
            new("@createddate", command.CreatedDate),
            new("@modifieddate", command.ModifiedDate)
        };

        _repo.ExecuteReader<RobotCommand>(
            @"INSERT INTO robotcommand
            (name, description, ismovecommand, createddate, modifieddate)
            VALUES (@name,@description,@ismovecommand,@createddate,@modifieddate)
            RETURNING *",
            sqlParams
        );
    }

    public void UpdateRobotCommand(int id, RobotCommand command)
    {
        var sqlParams = new NpgsqlParameter[]
        {
            new("@id", id),
            new("@name", command.Name),
            new("@description", command.Description ?? (object)DBNull.Value),
            new("@ismovecommand", command.IsMoveCommand),
            new("@createddate", command.CreatedDate),
            new("@modifieddate", command.ModifiedDate)
        };

        _repo.ExecuteReader<RobotCommand>(
            @"UPDATE robotcommand
            SET name=@name,
                description=@description,
                ismovecommand=@ismovecommand,
                createddate=@createddate,
                modifieddate=@modifieddate
            WHERE id=@id
            RETURNING *",
            sqlParams
        );
    }

    public void DeleteRobotCommand(int id)
    {
        var sqlParams = new NpgsqlParameter[]
        {
            new("@id", id)
        };

        _repo.ExecuteReader<RobotCommand>(
            "DELETE FROM robotcommand WHERE id=@id RETURNING *",
            sqlParams
        );
    }
}