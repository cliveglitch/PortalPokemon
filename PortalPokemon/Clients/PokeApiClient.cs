using PortalPokemon.Models;

namespace PortalPokemon.Clients
{
    /// <summary>
    /// Cliente para interactuar con la API de Pokémon (PokeAPI).
    /// </summary>
    public class PokeApiClient
    {
        private readonly HttpClient _httpClient;

        public PokeApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
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

            using var response = await _httpClient.GetAsync($"pokemon/{name}", cancellationToken);
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
        public async Task<PaginatedPokemonModel?> GetPaginatedPokemonAsync(int limit, int offset, CancellationToken cancellationToken)
        {
            using var response = await _httpClient.GetAsync($"pokemon?limit={limit}&offset={offset}", cancellationToken);
            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error fetching paginated Pokemon data: {response.StatusCode} - {content}");
            }

            var paginatedPokemon = System.Text.Json.JsonSerializer.Deserialize<PaginatedPokemonModel>(content);
            return paginatedPokemon;
        }
    }
}
