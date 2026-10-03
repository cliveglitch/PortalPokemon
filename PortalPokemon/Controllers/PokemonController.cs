using Microsoft.AspNetCore.Mvc;
using PortalPokemon.Models;
using PortalPokemon.Services;

namespace PortalPokemon.Controllers
{
    public class PokemonController : Controller
    {
        private readonly PokemonService _pokemonService;
        private readonly ExcelExportService _excelExportService;
        private readonly EmailService _emailService;

        public PokemonController(PokemonService pokemonService, ExcelExportService excelExportService, EmailService emailService)
        {
            _pokemonService = pokemonService;
            _excelExportService = excelExportService;
            _emailService = emailService;
        }

        public async Task<IActionResult> Details(string name, CancellationToken cancellationToken, string? returnUrl = null)
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

            if (pokemon.Species == null || string.IsNullOrWhiteSpace(pokemon.Species.Name))
            {
                return NotFound("Pokemon species information is missing.");
            }

            var species = await _pokemonService.GetPokemonSpeciesAsync(pokemon.Species.Name, cancellationToken);

            if (species == null)
            {
                return NotFound();
            }

            return View(new PokemonDetailModel { 
                Pokemon = pokemon, 
                Species = species,
                ReturnUrl = Url.IsLocalUrl(returnUrl)
                    ? returnUrl!
                    : Url.Action(nameof(Paginated)) ?? "/"
            });
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendEmail(string recipientEmail, string pokemonName, string? returnUrl = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(recipientEmail))
            {
                return BadRequest("Recipient email is required.");
            }
            if (string.IsNullOrWhiteSpace(pokemonName))
            {
                return BadRequest("Pokemon name is required.");
            }

            TempData["RecipientEmail"] = recipientEmail;

            try
            {
                var pokemon = await _pokemonService.GetPokemonAsync(pokemonName.Trim(), cancellationToken);
                if (pokemon == null)
                {
                    return NotFound($"Pokemon '{pokemonName}' not found.");
                }
                var species = await _pokemonService.GetPokemonSpeciesAsync(pokemon.Species?.Name ?? string.Empty, cancellationToken);
                if (species == null)
                {
                    return NotFound($"Species for Pokemon '{pokemonName}' not found.");
                }

                var pokemonDetailModel = new PokemonDetailModel
                {
                    Pokemon = pokemon,
                    Species = species
                };

                await _emailService.SendDetailPokemonAsync(recipientEmail, pokemonDetailModel, cancellationToken);

                TempData["ToastMessage"] = "Correo enviado exitosamente.";
                TempData["ToastType"] = "success";
            }
            catch (Exception ex) when (ex is not OperationCanceledException || ex is TaskCanceledException)
            {
                TempData["ToastMessage"] = $"Error al enviar el correo: {ex.Message}";
                TempData["ToastType"] = "error";
            }

            return RedirectToAction(nameof(Details), new { name = pokemonName, returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendManyEmail(string recipientEmail, int limit = 30, int offset = 0, string? nameFilter = null, string? speciesFilter = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(recipientEmail))
            {
                return BadRequest("Recipient email is required.");
            }

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

            TempData["RecipientEmail"] = recipientEmail;

            try
            {
                var page = await _pokemonService.GetPaginatedPokemonAsync(
                    limit, offset, nameFilter, speciesFilter, cancellationToken);

                await _emailService.SendPokemonAsync(
                    recipientEmail, page.PokemonGrid.Pokemons, cancellationToken);

                TempData["ToastMessage"] = "Correo enviado exitosamente.";
                TempData["ToastType"] = "success";
            }
            catch (Exception ex) when (ex is not OperationCanceledException  || ex is TaskCanceledException)
            {
                TempData["ToastMessage"] = $"Error al enviar el correo: {ex.Message}";
                TempData["ToastType"] = "error";
            }

            return RedirectToAction(nameof(Paginated), new
            {
                limit,
                offset,
                nameFilter,
                speciesFilter
            });
        }
    }
}
