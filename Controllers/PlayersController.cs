using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayersController : ControllerBase
    {
        private readonly IMongoCollection<Player> _players;

        public PlayersController(IMongoCollection<Player> players)
        {
            _players = players;
        }

        // GET: api/Players
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Player>>> GetPlayers()
        {
            var players = await _players.Find(_ => true).ToListAsync();
            return players;
        }

        // GET: api/Players/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Player>> GetPlayer(string id)
        {
            var player = await _players.Find(p => p.Id == id).FirstOrDefaultAsync();
            if (player == null)
            {
                return NotFound();
            }
            return player;
        }

        // PUT: api/Players/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlayer(string id, Player player)
        {
            if (id != player.Id)
            {
                return BadRequest();
            }

            var result = await _players.ReplaceOneAsync(p => p.Id == id, player);
            if (result.ModifiedCount == 0)
            {
                return NotFound();
            }

            return NoContent();
        }

        // POST: api/Players
        [HttpPost]
        public async Task<ActionResult<Player>> PostPlayer(Player player)
        {
            await _players.InsertOneAsync(player);
            return CreatedAtAction(nameof(GetPlayer), new { id = player.Id }, player);
        }

        // DELETE: api/Players/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlayer(string id)
        {
            var result = await _players.DeleteOneAsync(p => p.Id == id);
            if (result.DeletedCount == 0)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}