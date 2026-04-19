using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using robot_controller_api.Persistence;
using robot_controller_api.Models;

namespace robot_controller_api.Controllers;

[ApiController]
[Route("api/maps")]
[Authorize]
public class MapsController : ControllerBase
{
    private readonly IMapDataAccess _mapRepo;

    public MapsController(IMapDataAccess mapRepo)
    {
        _mapRepo = mapRepo;
    }

    [HttpGet]
    [Authorize(Policy = "UserOnly")]
    public IEnumerable<Map> GetAllMaps()
    {
        return _mapRepo.GetMaps();
    }

    [HttpGet("square")]
    [Authorize(Policy = "UserOnly")]
    public IEnumerable<Map> GetSquareMaps()
    {
        return _mapRepo.GetMaps().Where(m => m.Columns == m.Rows);
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "UserOnly")]
    public IActionResult GetMapById(int id)
    {
        var map = _mapRepo.GetMaps()
            .FirstOrDefault(m => m.Id == id);

        if (map == null)
            return NotFound();

        return Ok(map);
    }

    [HttpGet("{id}/{x}-{y}")]
    [Authorize(Policy = "UserOnly")]
    public IActionResult CheckCoordinate(int id, int x, int y)
    {
        if (x < 0 || y < 0)
            return BadRequest();

        var map = _mapRepo.GetMaps()
            .FirstOrDefault(m => m.Id == id);

        if (map == null)
            return NotFound();

        bool isOnMap = x < map.Columns && y < map.Rows;

        return Ok(isOnMap);
    }
}