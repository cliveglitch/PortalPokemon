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
                    items.Add(new PokemonModel());
                    continue;
                }

                var pokemon = await _pokeApiClient.GetPokemonAsync(result.Name, cancellationToken);
                items.Add(pokemon ?? new PokemonModel());
            }

            return items;
        }
    }
}
