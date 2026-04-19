using Npgsql;
using robot_controller_api.Models;

namespace robot_controller_api.Persistence;

public class RobotCommandADO : IRobotCommandDataAccess
{
    private const string CONNECTION_STRING = "Host=localhost;Username=postgres;Password=Hemant@123;Database=sit331";

    public List<RobotCommand> GetRobotCommands()
    {
        var robotCommands = new List<RobotCommand>();

        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();

        using var cmd = new NpgsqlCommand("SELECT * FROM robotcommand", conn);
        using var dr = cmd.ExecuteReader();

        while (dr.Read())
        {
            var robotCommand = new RobotCommand
            {
                Id = (int)dr["id"],
                Name = (string)dr["name"],
                IsMoveCommand = (bool)dr["ismovecommand"],
                CreatedDate = (DateTime)dr["createddate"],
                ModifiedDate = (DateTime)dr["modifieddate"],
                Description = dr["description"] as string
            };

            robotCommands.Add(robotCommand);
        }

        return robotCommands;
    }

    public void InsertRobotCommand(RobotCommand command)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();

        using var cmd = new NpgsqlCommand(
            @"INSERT INTO robotcommand
            (name, description, ismovecommand, createddate, modifieddate)
            VALUES (@name, @description, @ismovecommand, @createddate, @modifieddate)",
            conn);

        cmd.Parameters.AddWithValue("@name", command.Name);
        cmd.Parameters.AddWithValue("@description", (object?)command.Description ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ismovecommand", command.IsMoveCommand);
        cmd.Parameters.AddWithValue("@createddate", command.CreatedDate);
        cmd.Parameters.AddWithValue("@modifieddate", command.ModifiedDate);

        cmd.ExecuteNonQuery();
    }

    public void UpdateRobotCommand(int id, RobotCommand command)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();

        using var cmd = new NpgsqlCommand(
            @"UPDATE robotcommand
              SET name=@name,
                  description=@description,
                  ismovecommand=@ismovecommand,
                  createddate=@createddate,
                  modifieddate=@modifieddate
              WHERE id=@id",
            conn);

        cmd.Parameters.AddWithValue("@id", id);
        cmd.Parameters.AddWithValue("@name", command.Name);
        cmd.Parameters.AddWithValue("@description", (object?)command.Description ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ismovecommand", command.IsMoveCommand);
        cmd.Parameters.AddWithValue("@createddate", command.CreatedDate);
        cmd.Parameters.AddWithValue("@modifieddate", command.ModifiedDate);

        cmd.ExecuteNonQuery();
    }

    public void DeleteRobotCommand(int id)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();

        using var cmd = new NpgsqlCommand(
            "DELETE FROM robotcommand WHERE id=@id",
            conn);

        cmd.Parameters.AddWithValue("@id", id);

        cmd.ExecuteNonQuery();
    }
}