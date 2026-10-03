using PortalPokemon.Clients;
using PortalPokemon.Models;

namespace PortalPokemon.Services
{
    /// <summary>
    /// Servicio para interactuar con la API de Pokémon (PokeAPI) y obtener información sobre Pokémon.
    /// </summary>
    public class PokemonService
    {
        private readonly PokeApiClient _pokeApiClient;

        public PokemonService(PokeApiClient pokeApiClient)
        {
            _pokeApiClient = pokeApiClient;
        }

        /// <summary>
        /// Obtiene un Pokémon por su nombre desde la API de PokeAPI.
        /// </summary>
        /// <param name="name">El nombre del Pokémon.</param>
        /// <param name="cancellationToken">El token de cancelación.</param>
        /// <returns>La información del Pokémon o null si no se encuentra.</returns>
        public async Task<PokemonModel?> GetPokemonAsync(string name, CancellationToken cancellationToken)
        {
            var pokemon = await _pokeApiClient.GetPokemonAsync(name, cancellationToken);
            return pokemon;
        }

        /// <summary>
        /// Obtiene una lista paginada de Pokémon desde la API de PokeAPI, con la opción de filtrar por nombre.
        /// </summary>
        /// <param name="limit">El número de Pokémon a obtener.</param>
        /// <param name="offset">Desplazamiento para la paginación</param>
        /// <param name="nameFilter">Filtro por nombre.</param>
        /// <param name="cancellationToken">El token de cancelación.</param>
        /// <returns>La lista paginada de Pokémon.</returns>
        public async Task<PokePortalModel> GetPaginatedPokemonAsync(int limit, int offset, string? nameFilter, string? speciesFilter, CancellationToken cancellationToken)
        {
            var catalogTask = _pokeApiClient.GetPokemonCatalogAsync(cancellationToken);
            var speciesCatalogTask = _pokeApiClient.GetPokemonSpeciesCatalogAsync(cancellationToken);

            await Task.WhenAll(catalogTask, speciesCatalogTask);

            var catalog = await catalogTask;
            var speciesCatalog = await speciesCatalogTask;

            var nameSearch = nameFilter?.Trim() ?? "";
            var candidates = catalog.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(speciesFilter))
            {
                // Filtro por especia
                candidates = await _pokeApiClient.GetAllPokemonBySpeciesAsync(
                    speciesFilter, cancellationToken);
            }

            if (nameSearch.Length > 0)
            {
                // Filtro por nombre
                candidates = candidates.Where(p =>
                    p.Name?.Contains(
                        nameSearch, StringComparison.OrdinalIgnoreCase) == true);
            }

            var matches = candidates.ToList();
            var pageResults = matches.Skip(offset).Take(limit);

            var items = new List<PokemonModel>();

            // Obtener los detalles de cada Pokémon en la página actual
            foreach (var result in pageResults)
            {
                if (string.IsNullOrWhiteSpace(result.Name))
                {
                    items.Add(new PokemonModel { Name = result.Name });
                    continue;
                }

                var pokemon = await _pokeApiClient.GetPokemonAsync(result.Name, cancellationToken);
                items.Add(pokemon ?? new PokemonModel { Name = result.Name });
            }

            var availableSpecies = speciesCatalog
                .Where(s => !string.IsNullOrWhiteSpace(s.Name))
                .Select(s => s.Name!)
                .ToList();

            return new PokePortalModel
            {
                PokemonGrid = new PaginatedPokemonModel
                {
                    Count = matches.Count,
                    HasNext = (long)offset + limit < matches.Count,
                    HasPrevious = offset > 0,
                    Limit = limit,
                    Offset = offset,
                    Pokemons = items
                },
                NameFilter = nameFilter,
                SpeciesFilter = speciesFilter,
                AvailableSpecies = availableSpecies
            };
        }
    }
}
