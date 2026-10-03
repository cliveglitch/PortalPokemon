using System.Text.Json.Serialization;

namespace PortalPokemon.Models
{
    public class PaginatedPokemonSpeciesResponseModel
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }
        [JsonPropertyName("next")]
        public String? Next { get; set; }
        [JsonPropertyName("previous")]
        public String? Previous { get; set; }
        [JsonPropertyName("results")]
        public List<PaginatedPokemonSpeciesResultModel> Results { get; set; } = new List<PaginatedPokemonSpeciesResultModel>();
    }
}
