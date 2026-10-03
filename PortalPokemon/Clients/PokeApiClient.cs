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

        /// <summary>
        /// Obtenemos el catálogo completo de Pokémon. Este método obtiene todos los Pokémon disponibles en la API y los devuelve como una lista de resultados paginados.
        /// </summary>
        /// <param name="cancellationToken">El token de cancelación.</param>
        /// <returns>La lista de resultados paginados de Pokémon.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<IReadOnlyList<PaginatedPokemonResultModel>>GetPokemonCatalogAsync(CancellationToken cancellationToken)
        {
            var catalog = await GetPaginatedPokemonAsync(100_000, 0, cancellationToken);

            return catalog?.Results
                ?? throw new InvalidOperationException("PokéAPI returned no Pokémon catalog.");    
        }

        /// <summary>
        /// Obtenemos una lista paginada de especies de Pokémon.
        /// </summary>
        /// <param name="limit">El límite de resultados.</param>
        /// <param name="offset">El desplazamiento de resultados.</param>
        /// <param name="cancellationToken">El token de cancelación.</param>
        /// <returns>La lista de resultados paginados de especies de Pokémon.</returns>
        /// <exception cref="Exception"></exception>
        public async Task<PaginatedPokemonSpeciesResponseModel?> GetPaginatedPokemonSpeciesAsync(int limit, int offset, CancellationToken cancellationToken)
        {
            var cacheKey = $"paginated_pokemon_species:{limit}:{offset}";
            cancellationToken.ThrowIfCancellationRequested();

            if (_cache.TryGetValue<PaginatedPokemonSpeciesResponseModel>(cacheKey, out var cachedPaginatedPokemonSpecies))
            {
                return cachedPaginatedPokemonSpecies;
            }

            using var response = await _httpClient.GetAsync($"pokemon-species?limit={limit}&offset={offset}", cancellationToken);
            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error fetching paginated Pokemon species data: {response.StatusCode} - {content}");
            }

            var paginatedPokemonSpecies = System.Text.Json.JsonSerializer.Deserialize<PaginatedPokemonSpeciesResponseModel>(content);
            if (paginatedPokemonSpecies is not null)
            {
                _cache.Set(cacheKey, paginatedPokemonSpecies, TimeSpan.FromHours(6));
            }

            return paginatedPokemonSpecies;
        }

        /// <summary>
        /// Obtenemos el catálogo completo de especies de Pokémon. Este método obtiene todas las especies de Pokémon disponibles en la API y las devuelve como una lista de resultados paginados.
        /// </summary>
        /// <param name="cancellationToken">El token de cancelación.</param>
        /// <returns>La lista de especies de Pokémon.</returns>
        /// <exception cref="InvalidOperationException"></exception>

        public async Task<IReadOnlyList<PaginatedPokemonSpeciesResultModel>> GetPokemonSpeciesCatalogAsync(CancellationToken cancellationToken)
        {
            var catalog = await GetPaginatedPokemonSpeciesAsync(100_000, 0, cancellationToken);

            return catalog?.Results
                ?? throw new InvalidOperationException("PokéAPI returned no Pokémon species catalog.");
        }

        /// <summary>
        /// Obtenemos todos los Pokémon que pertenecen a una especie específica. Este método consulta la API para obtener la información de la especie y luego extrae la lista de variedades de Pokémon asociadas a esa especie.
        /// </summary>
        /// <param name="speciesName">El nombre de la especie de Pokémon.</param>
        /// <param name="cancellationToken">El token de cancelación.</param>
        /// <returns>La lista de Pokémon de la especie especificada.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<IReadOnlyList<PaginatedPokemonResultModel>> GetAllPokemonBySpeciesAsync(string speciesName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(speciesName))
            {
                throw new ArgumentException("Species name cannot be null or whitespace.", nameof(speciesName));
            }

            speciesName = speciesName.Trim().ToLowerInvariant();
            var cacheKey = $"pokemon_by_species:{speciesName}";

            cancellationToken.ThrowIfCancellationRequested();

            if (_cache.TryGetValue<IReadOnlyList<PaginatedPokemonResultModel>>(cacheKey, out var cachedPokemonBySpecies) && cachedPokemonBySpecies is not null)
            {
                return cachedPokemonBySpecies;
            }

            using var response = await _httpClient.GetAsync($"pokemon-species/{speciesName}", cancellationToken);
            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new InvalidOperationException($"Pokémon species '{speciesName}' not found.");
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error fetching Pokémon species data: {response.StatusCode} - {content}");
            }

            var speciesData = System.Text.Json.JsonSerializer.Deserialize<PokemonSpeciesModel>(content);

            if(speciesData is null)
            {
                throw new InvalidOperationException($"Pokémon species '{speciesName}' data could not be deserialized.");
            }

            var pokemonList = speciesData.Varieties
                .Select(variety => new PaginatedPokemonResultModel
                {
                    Name = variety.Pokemon.Name,
                    Url = variety.Pokemon.Url
                })
                .ToList();

            _cache.Set(cacheKey, pokemonList, TimeSpan.FromHours(6));

            return pokemonList;
        }
    }
}
