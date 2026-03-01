using AuctionApi.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionApi.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly AuctionDbContext _db;
    public AdminController(AuctionDbContext db) => _db = db;

    // Inaktivera auktion (syns ej i sök)
    [HttpPut("auctions/{id:int}/disable")]
    public async Task<IActionResult> DisableAuction(int id)
    {
        var auction = await _db.Auctions.FirstOrDefaultAsync(a => a.Id == id);
        if (auction == null) return NotFound("Auction not found.");

        auction.IsDisabled = true;
        await _db.SaveChangesAsync();

        return NoContent();
    }

    // Aktivera igen (valfritt men bra)
    [HttpPut("auctions/{id:int}/enable")]
    public async Task<IActionResult> EnableAuction(int id)
    {
        var auction = await _db.Auctions.FirstOrDefaultAsync(a => a.Id == id);
        if (auction == null) return NotFound("Auction not found.");

        auction.IsDisabled = false;
        await _db.SaveChangesAsync();

        return NoContent();
    }

    // Inaktivera användare (kan ej logga in)
    [HttpPut("users/{id:int}/disable")]
    public async Task<IActionResult> DisableUser(int id)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null) return NotFound("User not found.");

        user.IsActive = false;
        await _db.SaveChangesAsync();

        return NoContent();
    }

    // Aktivera igen (valfritt men bra)
    [HttpPut("users/{id:int}/enable")]
    public async Task<IActionResult> EnableUser(int id)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null) return NotFound("User not found.");

        user.IsActive = true;
        await _db.SaveChangesAsync();

        return NoContent();
    }
}