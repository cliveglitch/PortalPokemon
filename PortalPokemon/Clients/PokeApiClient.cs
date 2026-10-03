using PortalPokemon.Models;
using Microsoft.Extensions.Caching.Memory;

namespace PortalPokemon.Clients
{
    /// <summary>
    /// Cliente para interactuar con la API de Pokémon (PokeAPI).
    /// </summary>
    public class PokeApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;

        public PokeApiClient(HttpClient httpClient, IMemoryCache cache)
        {
            _httpClient = httpClient;
            _cache = cache;
        }

        /// <summary>
        /// Obtienemos la información de un Pokémon por su nombre.
        /// </summary>
        /// <param name="name">El nombre del Pokémon.</param>
        /// <param name="cancellationToken">El token de cancelación.</param>
        /// <returns>La información del Pokémon o null si no se encuentra.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<PokemonModel?> GetPokemonAsync(string name, CancellationToken cancellationToken)
        {
            if(string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Pokemon name cannot be null or whitespace.", nameof(name));
            }

            name = name.Trim().ToLowerInvariant();
            var cacheKey = $"pokemon:{name}";

            cancellationToken.ThrowIfCancellationRequested();

            if (_cache.TryGetValue<PokemonModel>(cacheKey, out var cachedPokemon))
            {
                return cachedPokemon;
            }

            using var response = await _httpClient.GetAsync(
                $"pokemon/{name}", cancellationToken);
            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error fetching Pokemon data: {response.StatusCode} - {content}");
            }

            var pokemon = System.Text.Json.JsonSerializer.Deserialize<PokemonModel>(content);

            if (pokemon is not null)
            {
                _cache.Set(cacheKey, pokemon, TimeSpan.FromHours(6));
            }

            return pokemon;
        }

        /// <summary>
        /// Obtenemos una lista paginada de Pokémon.
        /// </summary>
        /// <param name="limit">El número de Pokémon a obtener.</param>
        /// <param name="offset">El desplazamiento para la paginación.</param>
        /// <param name="cancellationToken">El token de cancelación.</param>
        /// <returns>La lista paginada de Pokémon o null si no se encuentra.</returns>
        /// <exception cref="Exception"></exception>
        public async Task<PaginatedPokemonResponseModel?> GetPaginatedPokemonAsync(int limit, int offset, CancellationToken cancellationToken)
        {
            var cacheKey = $"paginated_pokemon:{limit}:{offset}";
            cancellationToken.ThrowIfCancellationRequested();

            if (_cache.TryGetValue<PaginatedPokemonResponseModel>(cacheKey, out var cachedPaginatedPokemon))
            {
                return cachedPaginatedPokemon;
            }

            using var response = await _httpClient.GetAsync($"pokemon?limit={limit}&offset={offset}", cancellationToken);
            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error fetching paginated Pokemon data: {response.StatusCode} - {content}");
            }

            var paginatedPokemon = System.Text.Json.JsonSerializer.Deserialize<PaginatedPokemonResponseModel>(content);

            if (paginatedPokemon is not null)
            {
                _cache.Set(cacheKey, paginatedPokemon, TimeSpan.FromHours(6));
            }

            return paginatedPokemon;
        }

        public async Task<IReadOnlyList<PaginatedPokemonResultModel>>GetPokemonCatalogAsync(CancellationToken cancellationToken)
        {
            var cacheKey = "pokemon_catalog";
            var catalog = await GetPaginatedPokemonAsync(100_000, 0, cancellationToken);
            if (catalog is not null)
            {
                _cache.Set(cacheKey, catalog, TimeSpan.FromHours(6));
            }

            return catalog?.Results
                ?? throw new InvalidOperationException("PokéAPI returned no Pokémon catalog.");    
        }
    }
}
