using System.Text.Json.Serialization;

namespace PortalPokemon.Models
{
    public class PaginatedPokemonSpeciesResultModel
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("url")]
        public string? Url { get; set; }
    }
}
