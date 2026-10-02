using Microsoft.AspNetCore.Mvc;
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

            return View("~/Views/Home/Index.cshtml", pokemon);
        }

    }
}
