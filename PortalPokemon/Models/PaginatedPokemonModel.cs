using System.Text.Json.Serialization;

namespace PortalPokemon.Models
{
    public class PaginatedPokemonModel
    {
        public int Count { get; set; }
        public String? Next { get; set; }
        public String? Previous { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public IReadOnlyList<PokemonModel> Pokemons { get; set; } = new List<PokemonModel>();
    }
}
