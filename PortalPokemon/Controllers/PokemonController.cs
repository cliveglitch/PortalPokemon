using Microsoft.AspNetCore.Mvc;
using PortalPokemon.Models;
using PortalPokemon.Services;

namespace PortalPokemon.Controllers
{
    public class PokemonController : Controller
    {
        private readonly PokemonService _pokemonService;
        private readonly ExcelExportService _excelExportService;

        public PokemonController(PokemonService pokemonService, ExcelExportService excelExportService)
        {
            _pokemonService = pokemonService;
            _excelExportService = excelExportService;
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

            var items = new PokePortalModel
            {
                PokemonGrid = new PaginatedPokemonModel
                {
                    Count = 1,
                    Pokemons = new List<PokemonModel> { pokemon }
                }
            };

            return View("~/Views/Home/Index.cshtml", items);
        }

        public async Task<IActionResult> Paginated(int limit = 30, int offset = 0, string? nameFilter = null, string? speciesFilter = null, CancellationToken cancellationToken = default)
        {
            if (limit <= 0)
            {
                return BadRequest("Limit must be a positive integer.");
            }
            if (limit > 200)
            {
                return BadRequest("Limit cannot exceed 200.");
            }
            if (offset < 0)
            {
                return BadRequest("Offset must be a non-negative integer.");
            }

            var paginatedPokemon = await _pokemonService.GetPaginatedPokemonAsync(limit, offset, nameFilter, speciesFilter, cancellationToken);

            return View("~/Views/Home/Index.cshtml", paginatedPokemon);
        }

        [HttpGet]
        public async Task<IActionResult> ExportToExcel(int limit = 30, int offset = 0, string? nameFilter = null, string? speciesFilter = null, CancellationToken cancellationToken = default)
        {
            if (limit <= 0)
            {
                return BadRequest("Limit must be a positive integer.");
            }
            if (limit > 200)
            {
                return BadRequest("Limit cannot exceed 200.");
            }
            if (offset < 0)
            {
                return BadRequest("Offset must be a non-negative integer.");
            }

            var paginatedPokemon = await _pokemonService.GetPaginatedPokemonAsync(limit, offset, nameFilter, speciesFilter, cancellationToken);
            var excelData = await _excelExportService.ExportAsync(paginatedPokemon.PokemonGrid.Pokemons, cancellationToken);

            return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Pokemons.xlsx");
        }
    }
}
