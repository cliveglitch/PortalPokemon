using System.Text.Json.Serialization;

namespace PortalPokemon.Models
{
    public class PokemonSpeciesVarietiesModel
    {
        [JsonPropertyName("is_default")]
        public bool IsDefault { get; set; }
        [JsonPropertyName("pokemon")]
        public PaginatedPokemonResultModel Pokemon { get; set; } = new PaginatedPokemonResultModel();
    }
}
