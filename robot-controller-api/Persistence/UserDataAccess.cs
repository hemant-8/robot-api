using Npgsql;
using robot_controller_api.Models;

namespace robot_controller_api.Persistence;

public class UserDataAccess
{
    private readonly string _connectionString;

    public UserDataAccess(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public List<UserModel> GetAll()
    {
        var users = new List<UserModel>();

        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();

        var cmd = new NpgsqlCommand("SELECT * FROM \"user\"", conn);
        var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            users.Add(new UserModel
            {
                Id = Convert.ToInt32(reader["id"]),
                Email = reader["email"].ToString(),
                FirstName = reader["firstname"].ToString(),
                LastName = reader["lastname"].ToString(),
                PasswordHash = reader["passwordhash"].ToString(),
                Role = reader["role"].ToString(),
                CreatedDate = Convert.ToDateTime(reader["createddate"]),
                ModifiedDate = Convert.ToDateTime(reader["modifieddate"])
            });
        }

        return users;
    }

    public UserModel? GetByEmail(string email)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();

        var cmd = new NpgsqlCommand("SELECT * FROM \"user\" WHERE email=@email", conn);
        cmd.Parameters.AddWithValue("email", email);

        var reader = cmd.ExecuteReader();

        if (reader.Read())
        {
            return new UserModel
            {
                Id = Convert.ToInt32(reader["id"]),
                Email = reader["email"].ToString(),
                FirstName = reader["firstname"].ToString(),
                LastName = reader["lastname"].ToString(),
                PasswordHash = reader["passwordhash"].ToString(),
                Role = reader["role"].ToString(),
                CreatedDate = Convert.ToDateTime(reader["createddate"]),
                ModifiedDate = Convert.ToDateTime(reader["modifieddate"])
            };
        }

        return null;
    }

    public void Add(UserModel user)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();

        var cmd = new NpgsqlCommand(@"
            INSERT INTO ""user""
            (email, firstname, lastname, passwordhash, role, createddate, modifieddate)
            VALUES (@email, @firstname, @lastname, @passwordhash, @role, @createddate, @modifieddate)
        ", conn);

        cmd.Parameters.AddWithValue("email", user.Email);
        cmd.Parameters.AddWithValue("firstname", user.FirstName ?? "");
        cmd.Parameters.AddWithValue("lastname", user.LastName ?? "");
        cmd.Parameters.AddWithValue("passwordhash", user.PasswordHash);
        cmd.Parameters.AddWithValue("role", user.Role ?? "User");
        cmd.Parameters.AddWithValue("createddate", user.CreatedDate);
        cmd.Parameters.AddWithValue("modifieddate", user.ModifiedDate);

        cmd.ExecuteNonQuery();
    }

    public void Update(UserModel user)
{
    using var conn = new NpgsqlConnection(_connectionString);
    conn.Open();

    var cmd = new NpgsqlCommand(@"
        UPDATE ""user""
        SET firstname=@firstname,
            lastname=@lastname,
            description=@description,
            role=@role,
            modifieddate=@modifieddate
        WHERE id=@id
    ", conn);

    cmd.Parameters.AddWithValue("firstname", user.FirstName ?? "");
    cmd.Parameters.AddWithValue("lastname", user.LastName ?? "");
    cmd.Parameters.AddWithValue("description", user.Description ?? "");
    cmd.Parameters.AddWithValue("role", user.Role ?? "User");
    cmd.Parameters.AddWithValue("modifieddate", user.ModifiedDate);
    cmd.Parameters.AddWithValue("id", user.Id);

    cmd.ExecuteNonQuery();
}

public void Delete(int id)
{
    using var conn = new NpgsqlConnection(_connectionString);
    conn.Open();

    var cmd = new NpgsqlCommand("DELETE FROM \"user\" WHERE id=@id", conn);
    cmd.Parameters.AddWithValue("id", id);

    cmd.ExecuteNonQuery();
}
}