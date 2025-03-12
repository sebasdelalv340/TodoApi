using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using TodoApi.Controllers.Models;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayersController : ControllerBase
    {
        private readonly IMongoCollection<Player> _players;

        public PlayersController(IMongoDatabase database)
        {
            _players = database.GetCollection<Player>("Players");
        }

        // GET: api/Players
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Player>>> GetPlayers()
        {
            var players = await _players.Find(_ => true).ToListAsync();
            return players;
        }
        
        // GET: api/Players/top-scores
        [HttpGet("top-scores")]
        public async Task<ActionResult<IEnumerable<Player>>> GetTopScores()
        {
            try
            {
                // Obtener los 10 jugadores con las mejores puntuaciones
                var topPlayers = await _players.Find(_ => true)
                    .SortByDescending(p => p.MaxScore)  // Ordenar por MaxScore de mayor a menor
                    .Limit(10)                          // Limitar a 10 resultados
                    .ToListAsync();

                // Devolver la lista de jugadores
                return topPlayers;
            }
            catch (MongoException ex)
            {
                // Manejar errores de MongoDB
                return StatusCode(500, $"Error al consultar la base de datos: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Manejar otros errores inesperados
                return StatusCode(500, $"Ocurrió un error inesperado: {ex.Message}");
            }
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
        public async Task<ActionResult<Player>> PostPlayer([FromBody] Player player)
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