using System.Text.Json.Serialization;

namespace PortalPokemon.Models
{
    public class PaginatedPokemonModel
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }
        [JsonPropertyName("next")]
        public String? Next { get; set; }
        [JsonPropertyName("previous")]
        public String? Previous { get; set; }
        [JsonPropertyName("results")]
        public List<PaginatedPokemonResultModel> Results { get; set; } = new List<PaginatedPokemonResultModel>();
    }
}
