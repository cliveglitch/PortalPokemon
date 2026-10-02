using System.Text.Json.Serialization;

namespace PortalPokemon.Models
{
    public class PaginatedPokemonResultModel
    {
        [JsonPropertyName("name")]
        public String? Name { get; set; }
        [JsonPropertyName("url")]
        public String? Url { get; set; }
    }
}
