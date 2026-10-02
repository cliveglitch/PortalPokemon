using PortalPokemon.Clients;
using PortalPokemon.Models;

namespace PortalPokemon.Services
{
    public class PokemonService
    {
        private readonly PokeApiClient _pokeApiClient;

        public PokemonService(PokeApiClient pokeApiClient)
        {
            _pokeApiClient = pokeApiClient;
        }

        public async Task<PokemonModel?> GetPokemonAsync(string name, CancellationToken cancellationToken)
        {
            var pokemon = await _pokeApiClient.GetPokemonAsync(name, cancellationToken);
            return pokemon;
        }
    }
}
