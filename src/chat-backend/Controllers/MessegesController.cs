using ChatApp;
using ChatApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class MessegeController : ControllerBase
{
    private readonly ChatDbContext dbContext;

    public MessegeController(ChatDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    // [HttpPost]
    // public async Task<IActionResult> AddMessege()
    // {
        
    // } 

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteMessege(int id)
    {
        Message? messege = await dbContext.Messeges.FindAsync(id);

        if (messege == null)
        {
            return NotFound();
        }

        dbContext.Messeges.Remove(messege);

        await dbContext.SaveChangesAsync();

        return NoContent();
    }
}