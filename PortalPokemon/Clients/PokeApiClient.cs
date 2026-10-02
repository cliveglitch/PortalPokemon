using PortalPokemon.Models;

namespace PortalPokemon.Clients
{
    public class PokeApiClient
    {
        private readonly HttpClient _httpClient;

        public PokeApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<PokemonModel?> GetPokemonAsync(string name, CancellationToken cancellationToken)
        {
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
