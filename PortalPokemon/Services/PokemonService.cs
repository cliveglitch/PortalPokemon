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
        /// Obtiene una lista paginada de Pokémon desde la API de PokeAPI.
        /// </summary>
        /// <param name="limit">El número de Pokémon a obtener.</param>
        /// <param name="offset">Desplazamiento para la paginación</param>
        /// <param name="cancellationToken">El token de cancelación.</param>
        /// <returns>La lista paginada de Pokémon.</returns>
        public async Task<IReadOnlyList<PokemonModel>> GetPaginatedPokemonAsync(int limit, int offset, CancellationToken cancellationToken)
        {
            var page = await _pokeApiClient.GetPaginatedPokemonAsync(limit, offset, cancellationToken);
            if(page == null || page.Results == null)
            {
                return new List<PokemonModel>();
            }

            var items = new List<PokemonModel>();

            foreach(var result in page.Results)
            {
                if(string.IsNullOrWhiteSpace(result.Name))
                {
                    items.Add(new PokemonModel { Name = result.Name });
                    continue;
                }

                var pokemon = await _pokeApiClient.GetPokemonAsync(result.Name, cancellationToken);
                items.Add(pokemon ?? new PokemonModel { Name = result.Name });
            }

            return items;
        }
    }
}
