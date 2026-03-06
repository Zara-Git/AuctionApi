using AuctionApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuctionApi.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _service;
    public AdminController(IAdminService service) => _service = service;

    // PUT /api/admin/auctions/{id}/disable
    [HttpPut("auctions/{id:int}/disable")]
    public async Task<IActionResult> DisableAuction(int id)
    {
        var (ok, status, error) = await _service.DisableAuctionAsync(id);
        if (!ok) return StatusCode(status, error);
        return NoContent();
    }

    // PUT /api/admin/auctions/{id}/enable
    [HttpPut("auctions/{id:int}/enable")]
    public async Task<IActionResult> EnableAuction(int id)
    {
        var (ok, status, error) = await _service.EnableAuctionAsync(id);
        if (!ok) return StatusCode(status, error);
        return NoContent();
    }

    // PUT /api/admin/users/{id}/disable
    [HttpPut("users/{id:int}/disable")]
    public async Task<IActionResult> DisableUser(int id)
    {
        var (ok, status, error) = await _service.DisableUserAsync(id);
        if (!ok) return StatusCode(status, error);
        return NoContent();
    }

    // PUT /api/admin/users/{id}/enable
    [HttpPut("users/{id:int}/enable")]
    public async Task<IActionResult> EnableUser(int id)
    {
        var (ok, status, error) = await _service.EnableUserAsync(id);
        if (!ok) return StatusCode(status, error);
        return NoContent();
    }
}