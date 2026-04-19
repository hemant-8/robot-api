using Npgsql;
using robot_controller_api.Models;

namespace robot_controller_api.Persistence;

public class MapADO : IMapDataAccess
{
    private readonly string _connectionString;

    public MapADO(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection");
    }

    public List<Map> GetMaps()
    {
        var maps = new List<Map>();

        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();

        using var cmd = new NpgsqlCommand("SELECT * FROM map", conn);
        using var dr = cmd.ExecuteReader();

        while (dr.Read())
        {
            var map = new Map
            {
                Id = (int)dr["id"],
                Columns = (int)dr["columns"],
                Rows = (int)dr["rows"],
                Name = (string)dr["name"],
                CreatedDate = (DateTime)dr["createddate"],
                ModifiedDate = (DateTime)dr["modifieddate"],
                Description = dr["description"] as string
            };

            maps.Add(map);
        }

        return maps;   
    }
}