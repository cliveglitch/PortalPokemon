using Microsoft.AspNetCore.Mvc;
using PortalPokemon.Models;
using PortalPokemon.Services;

namespace PortalPokemon.Controllers
{
    public class PokemonController : Controller
    {
        private readonly PokemonService _pokemonService;

        public PokemonController(PokemonService pokemonService)
        {
            _pokemonService = pokemonService;
        }

        public async Task<IActionResult> Details(string name, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Pokemon name is required.");
            }

            var pokemon = await _pokemonService.GetPokemonAsync(name.Trim(), cancellationToken);
            if (pokemon == null)
            {
                return NotFound();
            }

            var items = new List<PokemonModel> { pokemon };

            return View("~/Views/Home/Index.cshtml", items);
        }

        public async Task<IActionResult> Paginated(int limit = 30, int offset = 0, CancellationToken cancellationToken = default)
        {
            var paginatedPokemon = await _pokemonService.GetPaginatedPokemonAsync(limit, offset, cancellationToken);

            return View("~/Views/Home/Index.cshtml", paginatedPokemon);
        }
    }
}
