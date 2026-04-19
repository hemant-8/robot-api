namespace robot_controller_api.Persistence;
using robot_controller_api.Models;

public interface IMapDataAccess
{
    List<Map> GetMaps();
    
}