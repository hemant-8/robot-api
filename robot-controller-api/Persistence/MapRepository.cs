using robot_controller_api.Models;

namespace robot_controller_api.Persistence;

public class MapRepository : IMapDataAccess, IRepository
{
    private IRepository _repo => this;

    public List<Map> GetMaps()
    {
        return _repo.ExecuteReader<Map>(
            "SELECT * FROM map"
        ).ToList();
    }
}