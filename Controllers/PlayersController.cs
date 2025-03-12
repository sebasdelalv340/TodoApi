using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
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

        [HttpPost]
        public async Task<ActionResult<Player>> PostPlayer([FromBody] Player? player)
        {
            try
            {
                // Validar el objeto Player recibido
                if (player == null)
                {
                    return BadRequest("El objeto Player no puede ser nulo.");
                }

                if (string.IsNullOrWhiteSpace(player.Name))
                {
                    return BadRequest("El nombre del jugador es obligatorio.");
                }

                if (player.MaxScore < 0)
                {
                    return BadRequest("La puntuación máxima no puede ser negativa.");
                }

                // Asignar un Id si no está definido
                if (string.IsNullOrEmpty(player.Id))
                {
                    player.Id = ObjectId.GenerateNewId().ToString();
                }

                // Insertar el jugador en la base de datos
                await _players.InsertOneAsync(player);

                // Devolver una respuesta 201 (Created) con la ubicación del nuevo recurso
                return CreatedAtAction(nameof(GetPlayer), new { id = player.Id }, player);
            }
            catch (MongoWriteException ex)
            {
                // Manejar errores de escritura en MongoDB
                return StatusCode(500, $"Error al insertar el jugador en la base de datos: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Manejar otros errores inesperados
                return StatusCode(500, $"Ocurrió un error inesperado: {ex.Message}");
            }
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